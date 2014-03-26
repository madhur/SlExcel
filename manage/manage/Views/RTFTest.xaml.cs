using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using Microsoft.SharePoint.Client;

namespace manage.Views
{
    public static class RichTextBoxBinder
    {
        #region RichTextBox attached properties

        public static readonly DependencyProperty XamlSourceProperty =
          DependencyProperty.RegisterAttached(
            "XamlSource",
            typeof(string),
            typeof(RichTextBox),
            new PropertyMetadata(OnXamlSourcePropertyChanged));

        private static void OnXamlSourcePropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            var rtb = d as RichTextBox;
            if (rtb == null) throw new ArgumentException(
              "Expected a dependency object of type RichTextBox.", "d");

            string xaml = null;
            if (e.NewValue != null)
            {
                xaml = e.NewValue as string;
                if (xaml == null) throw new ArgumentException("Expected a value of type string.", "e.NewValue");
            }

            // Set the xaml and reset selection

            Object o = XamlReader.Load(xaml);
            if (o is Section)
            {
                // Make sure its a section and clear out the old stuff in the rtb
                Section s = o as Section;
                rtb.Blocks.Clear();

                // Remove the blocks from the section first as adding them straight away
                // to the rtb will throw an exception because they are a child of two controls.
                List<Block> tempBlocks = new List<Block>();
                foreach (Block block in s.Blocks)
                {
                    tempBlocks.Add(block);
                }
                s.Blocks.Clear();

                // Add them block by block to the RTB
                foreach (Block block in tempBlocks)
                {
                    rtb.Blocks.Add(block);
                }
            }

            //rtb.Xaml = xaml ?? string.Empty;
            rtb.Selection.Select(rtb.ContentStart, rtb.ContentStart);
        }

        #endregion

        public static void SetXamlSource(this RichTextBox rtb, string xaml)
        {
            rtb.SetValue(XamlSourceProperty, xaml);
        }

        public static string GetXamlSource(this RichTextBox rtb)
        {
            return (string)rtb.GetValue(XamlSourceProperty);
        }
    }

    public partial class RTFTest : UserControl
    {
        private ClientContext context;
        private Microsoft.SharePoint.Client.Web spWeb;
        private List projectList;
        private ListItemCollection allProjectItems;
        private List<string> projectDetailsList;

        public RTFTest()
        {
            InitializeComponent();
            this.Loaded += RTFTest_Loaded;
        }

        void RTFTest_Loaded(object sender, RoutedEventArgs e)
        {
            context = new ClientContext("https://teams.aexp.com/sites/excel/");
            spWeb = context.Web;
            context.Load(spWeb);
            projectList = spWeb.Lists.GetByTitle("Manage Alerts");
            context.Load(projectList);
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml =
            @"<View>
                  <Query>                   
                  </Query>                            
                          </View>";
            allProjectItems = projectList.GetItems(camlQuery);
            context.Load(allProjectItems);
            context.ExecuteQueryAsync(new ClientRequestSucceededEventHandler(OnRequestSucceeded), new ClientRequestFailedEventHandler(OnRequestFailed));
        }

        private void OnRequestSucceeded(object sender, ClientRequestSucceededEventArgs args)
        {
            Dispatcher.BeginInvoke(DisplayAlerts);
        }

        private void OnRequestFailed(object sender, ClientRequestFailedEventArgs args)
        {
        }

        private void DisplayAlerts()
        {
            var alerts = (from s in allProjectItems.ToList()
                          select new {Title = s.FieldValues["Title"], Alert = s.FieldValues["Alert"].ToString()}).ToList();
            dgAlerts.ItemsSource = alerts;
        }
    }
}
