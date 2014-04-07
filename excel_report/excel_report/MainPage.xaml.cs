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
using manage.Controls;
using Telerik.Windows.Controls.GridView;

namespace excel_report
{
    public partial class MainPage : UserControl
    {

        private List<Idea> ideas = new List<Idea>();
        IEnumerable<ListItem> returnedItems = null;
       // private const String siteUrl = "https://teams.aexp.com/sites/excel";

        public MainPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

            BindGrid(QueryType.IN_PROGRESS);
            FormatControls(txtinprogress);
        }

        private void hyperlinkbutton_Click(object sender, RoutedEventArgs e)
        {
            var parent = (sender as HyperlinkButton).ParentOfType<GridViewRow>();
            var Item = parent.Item as Idea;
            var id = Item.ideaID.ToString();

            //MessageBox.Show(id, id, MessageBoxButton.OK);
            ClientContext context = ClientContext.Current;
            List list = context.Web.Lists.GetByTitle("Idea");
            context.Load(list);

            CamlQuery query = new CamlQuery();
            query.ViewXml = "<View><Query><Where><Eq><FieldRef Name='ID' LookupId='TRUE'/><Value Type='Integer'>" + Item.ideaID +
                "</Value></Eq></Where></Query></View>";


            ListItemCollection listitems = list.GetItems(query);
            context.Load(listitems);

            ChildWindow edit = new EditForm(id);
            edit.Show();


        }

        private void BindGrid(QueryType type)
        {
            ClientContext context = ClientContext.Current;

         

            returnedItems = context.LoadQuery(
                   context.Web.Lists.GetByTitle("Idea").GetItems(GetQuery(type))
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
                dataGrid1.DataContext = ideas;
                dataGrid1.Rebind();
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

        private CamlQuery GetQuery(QueryType type)
        {
            CamlQuery query = new CamlQuery();

            if (type == QueryType.IN_PROGRESS)
            {
                query.ViewXml = "<View><Query>" +
                 "<Where>" +

                     "<Eq>" +
                            "<FieldRef Name='Idea_x0020_Status' /><Value Type='Choice'>" + Status.IN_PROGRESS + "</Value>" +
                     "</Eq>" +

               "</Where>" +
                 "</Query></View>";
            }
            else if (type == QueryType.FUTURE)
            {
                query.ViewXml = "<View><Query>" +
                "<Where>" +

                    "<Eq>" +
                           "<FieldRef Name='Idea_x0020_Status' /><Value Type='Choice'>" + Status.FUTURE_PIPELINE + "</Value>" +
                    "</Eq>" +

              "</Where>" +
                "</Query></View>";


            }
            else if (type == QueryType.APPROVED)
            {
                query.ViewXml = "<View><Query>" +
               "<Where>" +

                   "<Eq>" +
                          "<FieldRef Name='Idea_x0020_Status' /><Value Type='Choice'>" + Status.APPROVED + "</Value>" +
                   "</Eq>" +

             "</Where>" +
               "</Query></View>";


            }
            else if (type == QueryType.UNDER_REVIEW)
            {


                query.ViewXml = "<View><Query>" +
                  "<Where>" +
                  "<Or>" +
                  "<Or>" +
                      "<Eq>" +
                             "<FieldRef Name='Idea_x0020_Status' /><Value Type='Choice'>" + Status.SUBMIT_APPROVAL + "</Value>" +
                      "</Eq>" +
                               "<Eq>" +
                                      "<FieldRef Name='Idea_x0020_Status' /><Value Type='Choice'>" + Status.FINANCE_REVIEW + "</Value>" +
                               "</Eq>" +
                                       "</Or>" +
                                       "<Or>" +
                                             "<Eq>" +
                                                "<FieldRef Name='Idea_x0020_Status' /><Value Type='Choice'>" + Status.READY_FINANCE_REVIEW + "</Value>" +
                                             "</Eq>" +
                                              "<Eq>" +
                                                "<FieldRef Name='Idea_x0020_Status' /><Value Type='Choice'>" + Status.PENDING_ACTUALS + "</Value>" +
                                             "</Eq>" +

                                          "</Or>" +
                                           "</Or>" +

                "</Where>" +
                  "</Query></View>";
            }
            else if (type == QueryType.ALL)
            {

                query = CamlQuery.CreateAllItemsQuery();
            }
            else
                query = CamlQuery.CreateAllItemsQuery();

            return query;

        }

        private void btn_InProgress_Click(object sender, RoutedEventArgs e)
        {
            BindGrid(QueryType.IN_PROGRESS);
            FormatControls(txtinprogress);
        }

        private void btn_underreview_Click(object sender, RoutedEventArgs e)
        {
            BindGrid(QueryType.UNDER_REVIEW);
            FormatControls(txtunderreview);
        }

        private void btn_Future_Click(object sender, RoutedEventArgs e)
        {
            BindGrid(QueryType.FUTURE);
            FormatControls(txtfuture);

        }

        private void btn_All_Click(object sender, RoutedEventArgs e)
        {
            BindGrid(QueryType.ALL);
            FormatControls(txtall);
        }

        private void btn_Approved_Click(object sender, RoutedEventArgs e)
        {
            BindGrid(QueryType.APPROVED);
            FormatControls(txtapproved);

        }

        private void FormatControls(TextBlock activeBlock)
        {
            TextBlock[] textBlocks = null;

            textBlocks = new TextBlock[] { txtinprogress, txtall, txtfuture, txtapproved, txtfuture, txtunderreview };

            foreach (TextBlock textBlock in textBlocks)
            {
                textBlock.TextDecorations = TextDecorations.Underline;
                textBlock.FontWeight = FontWeights.Normal;
                textBlock.Foreground = new SolidColorBrush(Colors.Black);

            }

            activeBlock.FontWeight = FontWeights.Bold;
            activeBlock.TextDecorations = null;
            activeBlock.Foreground = new SolidColorBrush(Colors.Green);
        }



    }

    public enum QueryType
    {
        ALL,
        IN_PROGRESS,
        UNDER_REVIEW,
        APPROVED,
        FUTURE

    };
}

      
