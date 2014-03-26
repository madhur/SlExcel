using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.SharePoint.Client;

using Telerik.Windows.Data;
using Telerik.Windows.Controls;

namespace excel_report
{
    public partial class MainPage : UserControl
    {

        private List<Idea> ideas = new List<Idea>();
        private IEnumerable<ListItem> returnedItems = null;

        public MainPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ClientContext context = ClientContext.Current;
            returnedItems = context.LoadQuery(
                context.Web.Lists.GetByTitle("Idea").GetItems(new CamlQuery())
                );

            dataGrid1.IsBusy = true;
            context.ExecuteQueryAsync(succeededCallback, failedCallback);
        }

        private void failedCallback(object sender, ClientRequestFailedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show(e.ErrorDetails.ToString(), "error", MessageBoxButton.OK);
                dataGrid1.IsBusy = false;
            }
           );

          
        }

        private void succeededCallback(object sender, ClientRequestSucceededEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                ideas.Clear();
                foreach (var item in returnedItems)
                {
                    ideas.Add(new Idea(item));
                }

                dataGrid1.ItemsSource = ideas;
                dataGrid1.IsBusy = false;
            }
           );
        }

        private void dataGrid1_Sorting(object sender, GridViewSortingEventArgs e)
        {
            //Gets the value of the ItemsSource property as IEnumerable.
            IEnumerable<Idea> ideas = e.DataControl.ItemsSource as IEnumerable<Idea>;

            //checks if the value of the collection is null
            if (ideas == null)
            {
                e.Cancel = true;
                return;
            }

            if (e.OldSortingState == SortingState.None)
            {
                e.NewSortingState = SortingState.Ascending;
                ideas = ideas.OrderBy(idea => idea.GetType()
                                                                   .GetProperty((e.Column as GridViewDataColumn).GetDataMemberName())
                                                                   .GetValue(idea, null));
            }
            //If the sorting state is none, sort the items descending.
            else if (e.OldSortingState == SortingState.Ascending)
            {
                e.NewSortingState = SortingState.Descending;
                ideas = ideas.OrderByDescending(idea => idea.GetType()
                                                                    .GetProperty((e.Column as GridViewDataColumn).GetDataMemberName())
                                                                    .GetValue(idea, null));
            }
            //If the sorting state is descending, apply default sorting to the items.
            else
            {
                e.NewSortingState = SortingState.Ascending;
                ideas = ideas.OrderBy(idea => idea.ideaID);
                ideas = ideas.OrderBy(idea => idea.ideaStatus);
                ideas = ideas.OrderBy(idea => idea.costType);
                ideas = ideas.OrderBy(idea => idea.totalSave);

            }
            //Set the sorted collection as source of the RadGridView
            e.DataControl.ItemsSource = ideas.ToList();
            e.Cancel = true;
        }

        private void btn_export_Click(object sender, RoutedEventArgs e)
        {
            string extension = "xls";
            SaveFileDialog dialog = new SaveFileDialog()
            {
                DefaultExt = extension,
                Filter = String.Format("{1} files (*.{0})|*.{0}|All files (*.*)|*.*", extension, "Excel"),
                FilterIndex = 1
            };
            if (dialog.ShowDialog() == true)
            {
                using (System.IO.Stream stream = dialog.OpenFile())
                {
                    dataGrid1.Export(stream,
                        new GridViewExportOptions()
                        {
                            Format = ExportFormat.ExcelML,
                            ShowColumnHeaders = true,
                            ShowColumnFooters = true,
                            ShowGroupFooters = false,
                        });
                }
            }
        }



    }

}

      
