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
using System.Threading;
using Microsoft.SharePoint.Client;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Controls.Primitives;
using Telerik.Windows.Data;
using Telerik.Windows.Controls;
using manage.Controls;
using Telerik.Windows.Controls.GridView;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Windows.Browser;
using Common;


namespace manage
{
    public partial class MainPage : UserControl
    {
        //private const string siteUrl = "https://teams.aexp.com/sites/excel/";

        private List<Idea> ideas = new List<Idea>();

        public MainPage()
        {
            InitializeComponent();
            SilverlightOM();


        }


        #region --------R O L E S -----------

        private void SilverlightOM()
        {
            admintab.Visibility = Visibility.Collapsed;
            teamtab.Visibility = Visibility.Collapsed;
            myideas.Visibility = Visibility.Visible;
            financetab.Visibility = Visibility.Collapsed;
            btn_siteadmin.Visibility = Visibility.Collapsed;


            ClientContext client = ClientContext.Current;
            GroupCollection groupCollection = client.Web.SiteGroups;

            Microsoft.SharePoint.Client.Group adminGroup = groupCollection.GetById(40);
            User currentUser = client.Web.CurrentUser;
            UserCollection userCol = adminGroup.Users;
            client.Load(currentUser);
            client.Load(userCol);

            client.ExecuteQueryAsync((s, ee) =>
            {

                foreach (User groupUser in userCol)
                {
                    if (groupUser.LoginName.Equals(currentUser.LoginName))
                    {


                        Dispatcher.BeginInvoke(() =>
                        {

                            teamtab.Visibility = Visibility.Visible;
                            admintab.Visibility = Visibility.Visible;
                            financetab.Visibility = Visibility.Visible;
                            btn_siteadmin.Visibility = Visibility.Visible;


                        }

                        );
                    }
                }
            },



   (s, ee) =>
   {
       Console.WriteLine(ee.Message);

   });

            Microsoft.SharePoint.Client.Group financeGroup = groupCollection.GetById(39);

            UserCollection userCol2 = financeGroup.Users;
            client.Load(currentUser);

            client.Load(userCol2);



            client.ExecuteQueryAsync((ss, eee) =>
            {

                foreach (User groupUser in userCol2)

                    if (groupUser.LoginName.Equals(currentUser.LoginName))
                    {
                        Dispatcher.BeginInvoke(() =>
                        {

                            financetab.Visibility = Visibility.Visible;
                            btn_siteadmin.Visibility = Visibility.Collapsed;


                        }

                      );
                    }
            },



(ss, eee) =>
{
    Console.WriteLine(eee.Message);

});
            Microsoft.SharePoint.Client.Group employeeGroup = groupCollection.GetById(37);
            UserCollection userCol3 = employeeGroup.Users;

            client.Load(currentUser);
            client.Load(userCol3);



            client.ExecuteQueryAsync((sss, eeee) =>
            {


                foreach (User groupUser in userCol3)
                    if (groupUser.LoginName.Equals(currentUser.LoginName))
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            teamtab.Visibility = Visibility.Visible;
                            btn_siteadmin.Visibility = Visibility.Collapsed;

                        }

                  );
                    }
            },



(sss, eeee) =>
{
    Console.WriteLine(eeee.Message);

});

            Microsoft.SharePoint.Client.Group ownersGroup = groupCollection.GetById(6);
            UserCollection userCol4 = ownersGroup.Users;
            client.Load(currentUser);

            client.Load(userCol4);


            client.ExecuteQueryAsync((ssss, eeeee) =>
            {

                foreach (User groupUser in userCol4)
                    if (groupUser.LoginName.Equals(currentUser.LoginName))
                    {
                        Dispatcher.BeginInvoke(() =>
                        {

                            teamtab.Visibility = Visibility.Visible;
                            admintab.Visibility = Visibility.Visible;
                            financetab.Visibility = Visibility.Visible;
                            btn_siteadmin.Visibility = Visibility.Visible;


                        }

                  );
                    }
            },



(ssss, eeeee) =>
{
    Console.WriteLine(eeeee.Message);

});
        }


        #endregion

        #region L O A D   M A I N P A G E

        public void ReloadTabs()
        {
            LoadMyIdeasTab();
            if (teamtab.Visibility == Visibility.Visible) ;
            LoadMyTeamTab();
            if (financetab.Visibility == Visibility.Visible)
                LoadFinanceTab();
            if (admintab.Visibility == Visibility.Visible)
                LoadAdminTab();

        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
        }
        #endregion

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            int id;


            if (HtmlPage.Document.QueryString.ContainsKey("q"))
            {
                string queryStringValue = HtmlPage.Document.QueryString["q"];
                if (Int32.TryParse(queryStringValue, out id))
                {

                    ChildWindow edit = new EditForm(id.ToString(), this); 
                    edit.Show();

                }
                else
                {

                    // the result was not successful                    
                }
            }

            LoadMyIdeasTab();
        }

        private void LoadFinanceTab()
        {

            List<Idea> ideas = new List<Idea>();

            ClientContext context = ClientContext.Current;
            CamlQuery query = new CamlQuery();
            query.ViewXml = "<View><Query>" +
                "<Where>" +
                                "<Eq>" +
                                    "<FieldRef Name='Idea_x0020_Status' /><Value Type='Choice'>" + Status.READY_FINANCE_REVIEW + "</Value>" +
                                 "</Eq>" +

                 "</Where>" +
                "</Query></View>";



            ListItemCollection returnedItems = context.Web.Lists.GetByTitle("Idea").GetItems(query);
            context.Load(returnedItems);
            financeGrid.IsBusy = true;
            context.ExecuteQueryAsync((ssss, eeeee) =>
            {
                Dispatcher.BeginInvoke(() =>
                {

                    foreach (var item in returnedItems)
                    {
                        ideas.Add(new Idea(item, true));
                    }

                    financeGrid.ItemsSource = ideas;
                    financeGrid.DataContext = ideas;
                    financeGrid.IsBusy = false;
                }

          );

            },



(ssss, eeeee) =>
{
    Dispatcher.BeginInvoke(() =>
    {


        MessageBox.Show(eeeee.Message, "error", MessageBoxButton.OK);
        financeGrid.IsBusy = false;
    }

         );

});

        }

        private void LoadMyIdeasTab()
        {


            List<Idea> ideas = new List<Idea>();
            ClientContext context = ClientContext.Current;
            CamlQuery query = new CamlQuery();
            query.ViewXml = "<View><Query>" +
                "<Where>" +
                        "<Or>" +
                            
                                "<Eq>" +
                                    "<FieldRef Name='Executor' LookupId='TRUE'/><Value Type='Integer'><UserID/></Value>" +
                                 "</Eq>" +
                                 "<Eq>" +
                                    "<FieldRef Name='Author' LookupId='TRUE'/><Value Type='Integer'><UserID/></Value>" +
                                 "</Eq>" +
                                                   
                        "</Or>" +
                 "</Where>" +
                "</Query></View>";
            ListItemCollection returnedItems = context.Web.Lists.GetByTitle("Idea").GetItems(query);
            context.Load(returnedItems);
            myideasGrid.IsBusy = true;

            context.ExecuteQueryAsync((ssss, eeeee) =>
            {


                Dispatcher.BeginInvoke(() =>
                {

                    foreach (var item in returnedItems)
                    {
                        ideas.Add(new Idea(item));
                    }

                    myideasGrid.ItemsSource = ideas;
                    myideasGrid.DataContext = ideas;
                    myideasGrid.IsBusy = false;

                }

          );

            },



(ssss, eeeee) =>
{
    Dispatcher.BeginInvoke(() =>
    {


        MessageBox.Show(eeeee.Message, "error", MessageBoxButton.OK);
        myideasGrid.IsBusy = false;
    }

        );

});


        }

        private void LoadMyTeamTab()
        {

            List<Idea> ideas = new List<Idea>();

            ClientContext context = ClientContext.Current;
            CamlQuery query = new CamlQuery();
            //query.ViewXml = "<View><Query>" +
            //    "<Where>" +
            //            "<Or>" +
            //                "<Or>" +
            //                    "<Eq>" +
            //                        "<FieldRef Name='Executor' LookupId='TRUE'/><Value Type='Integer'><UserID/></Value>" +
            //                     "</Eq>" +
            //                     "<Eq>" +
            //                        "<FieldRef Name='Director' LookupId='TRUE'/><Value Type='Integer'><UserID/></Value>" +
            //                     "</Eq>" +
            //                "</Or>" +
            //                "<Or>" +
            //                    "<Eq>" +
            //                        "<FieldRef Name='VP' LookupId='TRUE'/><Value Type='Integer'><UserID/></Value>" +
            //                     "</Eq>" +
            //                     "<Eq>" +
            //                        "<FieldRef Name='FTE_x0020_Contributors' LookupId='TRUE'/><Value Type='Integer'><UserID/></Value>" +
            //                     "</Eq>" +
            //                "</Or>" +
            //            "</Or>" +
            //     "</Where>" +
            //    "</Query></View>";

            query.ViewXml = "<View><Query>" +
              "<Where>" +
                      "<Or>" +
                                "<Eq>" +
                                  "<FieldRef Name='Author' LookupId='TRUE'/><Value Type='Integer'><UserID/></Value>" +
                               "</Eq>" +
                                 "<Or>" +
                                      "<Eq>" +
                                          "<FieldRef Name='Executor' LookupId='TRUE'/><Value Type='Integer'><UserID/></Value>" +
                                       "</Eq>" +
                                            "<Or>" +
                                                   "<Eq>" +
                                                      "<FieldRef Name='Director' LookupId='TRUE'/><Value Type='Integer'><UserID/></Value>" +
                                                   "</Eq>" +
                         
                          "<Or>" +
                              "<Eq>" +
                                  "<FieldRef Name='VP' LookupId='TRUE'/><Value Type='Integer'><UserID/></Value>" +
                               "</Eq>" +
                               "<Eq>" +
                                  "<FieldRef Name='FTE_x0020_Contributors' LookupId='TRUE'/><Value Type='Integer'><UserID/></Value>" +
                               "</Eq>" +
                          "</Or>" +
                      "</Or>" +
                       "</Or>" +
                        "</Or>" +
               "</Where>" +
              "</Query></View>";



            ListItemCollection returnedItems = context.Web.Lists.GetByTitle("Idea").GetItems(query);
            context.Load(returnedItems);
            myteamGrid.IsBusy = true;

            context.ExecuteQueryAsync((ssss, eeeee) =>
            {
                Dispatcher.BeginInvoke(() =>
                {

                    foreach (var item in returnedItems)
                    {
                        ideas.Add(new Idea(item));
                    }

                    myteamGrid.ItemsSource = ideas;
                    myteamGrid.DataContext = ideas;
                    myteamGrid.IsBusy = false;
                }

          );

            },



(ssss, eeeee) =>
{

    Dispatcher.BeginInvoke(() =>
    {


        MessageBox.Show(eeeee.Message, "error", MessageBoxButton.OK);
        myteamGrid.IsBusy = false;
    }

         );


});

        }

        private void LoadAdminTab()
        {

            BindGrid(Consts.LOB1.GBSHR);

}

        #region //-------------MY IDEAS B U T T O N S ------------////

        public void btn_All_Click(object sender, RoutedEventArgs e)
        {
            FormatControls(txtall, TAB.MY_IDEAS);
            ApplyFilters(new String[] { }, TAB.MY_IDEAS, Status.STATUS_COLUMN);
        }

        private void FormatControls(TextBlock activeBlock, TAB activeTab)
        {
            TextBlock[] textBlocks = null;

            if (activeTab == TAB.MY_IDEAS)
                textBlocks = new TextBlock[] { txtdraft, txtall, txtfuture, txtapproved, txtactive };
            else if (activeTab == TAB.TEAM_IDEAS)
                textBlocks = new TextBlock[] { teamDraft, teamAll, teamFuture, teamApproved, teamActive };
            else if (activeTab == TAB.ADMIN)
                textBlocks = new TextBlock[] { btn_adminGBS, btn_adminGBT, btn_adminGCP, btn_adminPBMT, btn_adminWSGCAT, btn_adminALL };
            else if (activeTab == TAB.FINANCE)
                textBlocks = new TextBlock[] { btn_financeGBS, btn_financeGBT, btn_financeGCP, btn_financePBMT, btn_financeWSGCAT, btn_financeALL };

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



        private void ApplyFilters(String[] statuses, TAB activeTab, string colName)
        {
            GridViewColumn statusColumn = null;
            RadGridView radGrid = null;
            if (activeTab == TAB.MY_IDEAS)
                radGrid = myideasGrid;
            else if (activeTab == TAB.TEAM_IDEAS)
                radGrid = myteamGrid;
            else if (activeTab == TAB.FINANCE)
                radGrid = financeGrid;
            else if (activeTab == TAB.ADMIN)
            {
                //radGrid = adminGrid;
                BindGrid(statuses);
                return;
            }

            statusColumn = radGrid.Columns[colName];

            IColumnFilterDescriptor columnDescriptor = statusColumn.ColumnFilterDescriptor;
            columnDescriptor.SuspendNotifications();
            //radGrid.FilterDescriptors.Clear();
            columnDescriptor.DistinctFilter.Clear();
            foreach (String status in statuses)
                columnDescriptor.DistinctFilter.AddDistinctValue(status);
            columnDescriptor.ResumeNotifications();

        }

        private void BindGrid(string[] statuses)
        {

            List<Idea> ideas = new List<Idea>();

            ClientContext context = ClientContext.Current;
            CamlQuery query = new CamlQuery();

            if (statuses.Contains(Consts.LOB1.GBS) && statuses.Contains(Consts.LOB1.HR))
            {
                query.ViewXml = "<View><Query>" +
                  "<Where>" +
                  "<Or>" +
                      "<Eq>" +
                             "<FieldRef Name='Line_x0020_Of_x0020_Business_x001' /><Value Type='Choice'>" + Consts.LOB1.GBS + "</Value>" +
                      "</Eq>" +
                               "<Eq>" +
                                      "<FieldRef Name='Line_x0020_Of_x0020_Business_x001' /><Value Type='Choice'>" + Consts.LOB1.HR + "</Value>" +
                               "</Eq>" +
                            "</Or>" +

                "</Where>" +
                  "</Query></View>";
            }
            else if (statuses.Contains(Consts.LOB1.GCP))
            {
                query.ViewXml = "<View><Query>" +
                 "<Where>" +
                     "<Eq>" +
                            "<FieldRef Name='Line_x0020_Of_x0020_Business_x001' /><Value Type='Choice'>" + Consts.LOB1.GCP + "</Value>" +
                     "</Eq>" +

               "</Where>" +
                 "</Query></View>";
            }
            else if (statuses.Contains(Consts.LOB1.GBT))
            {

                query.ViewXml = "<View><Query>" +
                "<Where>" +
                    "<Eq>" +
                           "<FieldRef Name='Line_x0020_Of_x0020_Business_x001' /><Value Type='Choice'>" + Consts.LOB1.GBT + "</Value>" +
                    "</Eq>" +

              "</Where>" +
                "</Query></View>";
            }
            else if (statuses.Contains(Consts.LOB1.PBMT))
            {

                query.ViewXml = "<View><Query>" +
                "<Where>" +
                    "<Eq>" +
                           "<FieldRef Name='Line_x0020_Of_x0020_Business_x001' /><Value Type='Choice'>" + Consts.LOB1.PBMT + "</Value>" +
                    "</Eq>" +

              "</Where>" +
                "</Query></View>";
            }
            else if (statuses.Contains(Consts.LOB1.WSGCAT))
            {

                query.ViewXml = "<View><Query>" +
                "<Where>" +
                    "<Eq>" +
                           "<FieldRef Name='Line_x0020_Of_x0020_Business_x001' /><Value Type='Choice'>" + Consts.LOB1.WSGCAT + "</Value>" +
                    "</Eq>" +

              "</Where>" +
                "</Query></View>";

            }

            ListItemCollection returnedItems = context.Web.Lists.GetByTitle("Idea").GetItems(CamlQuery.CreateAllItemsQuery());
            context.Load(returnedItems);
            adminGrid.IsBusy = true;
            context.ExecuteQueryAsync((ssss, eeeee) =>
            {
                Dispatcher.BeginInvoke(() =>
                {

                    foreach (var item in returnedItems)
                    {
                        ideas.Add(new Idea(item));
                    }

                    adminGrid.ItemsSource = ideas;
                    adminGrid.DataContext = ideas;

                    ApplyStatusFilter();

                    adminGrid.IsBusy = false;


                }

          );

            },



(ssss, eeeee) =>
{
    Console.WriteLine(eeeee.Message);
    Dispatcher.BeginInvoke(() =>
    {


        adminGrid.IsBusy = false;
    }

          );
});

            
        }

        private void ApplyStatusFilter()
        {
            ApplyFilters(new String[] { Status.SUBMIT_APPROVAL, Status.PENDING_ACTUALS, Status.FINANCE_REVIEW }, TAB.ADMIN, Status.STATUS_COLUMN);
        }

        public void btn_draft_Click(object sender, RoutedEventArgs e)
        {

            ApplyFilters(Status.Draft, TAB.MY_IDEAS, Status.STATUS_COLUMN);
            FormatControls(txtdraft, TAB.MY_IDEAS);

        }

        public void btn_Future_Click(object sender, RoutedEventArgs e)
        {

            FormatControls(txtfuture, TAB.MY_IDEAS);
            ApplyFilters(new String[] { Status.FUTURE_PIPELINE }, TAB.MY_IDEAS, Status.STATUS_COLUMN);

        }

        public void btn_Active_Click(object sender, RoutedEventArgs e)
        {
            FormatControls(txtactive, TAB.MY_IDEAS);
            ApplyFilters(Status.Active, TAB.MY_IDEAS, Status.STATUS_COLUMN);

        }

        public void btn_Approved_Click(object sender, RoutedEventArgs e)
        {
            FormatControls(txtapproved, TAB.MY_IDEAS);
            ApplyFilters(new String[] { Status.APPROVED }, TAB.MY_IDEAS, Status.STATUS_COLUMN);
        }



        #endregion

        #region ///----------------TEAM IDEAS BUTTONS----------------/////


        void financetab_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFinanceTab();

        }


        void admintab_Loaded(object sender, RoutedEventArgs e)
        {

            LoadAdminTab();

        }

        void teamtab_Loaded(object sender, RoutedEventArgs e)
        {

            LoadMyTeamTab();
        }

        private void failedCallbackTEAM(object sender, ClientRequestFailedEventArgs e)
        {
            MessageBox.Show(e.ErrorDetails.ToString(), "error", MessageBoxButton.OK);
        }



        //---------BEGIN BUTTONS------------//

        public void btn_teamall_Click(object sender, RoutedEventArgs e)
        {

            FormatControls(teamAll, TAB.TEAM_IDEAS);
            ApplyFilters(new String[] { }, TAB.TEAM_IDEAS, Status.STATUS_COLUMN);

        }


        public void btn_teamdraft_Click(object sender, RoutedEventArgs e)
        {
            FormatControls(teamDraft, TAB.TEAM_IDEAS);
            ApplyFilters(Status.Draft, TAB.TEAM_IDEAS, Status.STATUS_COLUMN);
        }

        public void btn_teamfuture_Click(object sender, RoutedEventArgs e)
        {

            FormatControls(teamFuture, TAB.TEAM_IDEAS);
            ApplyFilters(new String[] { Status.FUTURE_PIPELINE }, TAB.TEAM_IDEAS, Status.STATUS_COLUMN);

        }

        public void btn_teamactive_Click(object sender, RoutedEventArgs e)
        {
            FormatControls(teamActive, TAB.TEAM_IDEAS);
            ApplyFilters(Status.Active, TAB.TEAM_IDEAS, Status.STATUS_COLUMN);

        }

        public void btn_teamapproved_Click(object sender, RoutedEventArgs e)
        {
            FormatControls(teamApproved, TAB.TEAM_IDEAS);
            ApplyFilters(new String[] { Status.APPROVED }, TAB.TEAM_IDEAS, Status.STATUS_COLUMN);
        }

        #endregion

        #region B U T T O N S


        private void myidea_add_Click(object sender, RoutedEventArgs e)
        {
            Uri redirect = new Uri(Utils.GetSiteUrl()+"/SitePages/create.aspx");
            System.Windows.Browser.HtmlPage.Window.Navigate(redirect, "_parent");
        }

        private void btn_export_Click(object sender, RoutedEventArgs e)
        {
            RadGridView gridView = null;
            int index = radTabControl.SelectedIndex;

            switch (index)
            {
                case 0:
                    gridView = myideasGrid;
                    break;
                case 1:
                    gridView = myteamGrid;
                    break;
                case 2:
                    gridView = adminGrid;
                    break;
                case 3:
                    gridView = financeGrid;
                    break;

                default:
                    gridView = myideasGrid;
                    break;
            }

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
                    gridView.Export(stream,
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

            ChildWindow edit = new EditForm(id, this);
            edit.Show();


        }



        #endregion



        #region <~~~~~~~~~~~~~ADMIN FINANCE~~~~~~~~~~~~~~~~~~>

        private void AdmGBSLink_Click(object sender, RoutedEventArgs e)
        {

            FormatControls(btn_adminGBS, TAB.ADMIN);
            ApplyFilters(Consts.LOB1.GBSHR, TAB.ADMIN, Consts.LOB_COLUMN);
        }

        private void AdmGBTLink_Click(object sender, RoutedEventArgs e)
        {
            FormatControls(btn_adminGBT, TAB.ADMIN);
            ApplyFilters(new String[] { Consts.LOB1.GBT }, TAB.ADMIN, Consts.LOB_COLUMN);

        }

        private void AdmGCPLink_Click(object sender, RoutedEventArgs e)
        {
            FormatControls(btn_adminGCP, TAB.ADMIN);
            ApplyFilters(new String[] { Consts.LOB1.GCP }, TAB.ADMIN, Consts.LOB_COLUMN);


        }

        private void AdmPBMTLink_Click(object sender, RoutedEventArgs e)
        {
            FormatControls(btn_adminPBMT, TAB.ADMIN);
            ApplyFilters(new String[] { Consts.LOB1.PBMT }, TAB.ADMIN, Consts.LOB_COLUMN);


        }

        private void AdmWSGCATLink_Click(object sender, RoutedEventArgs e)
        {
            FormatControls(btn_adminWSGCAT, TAB.ADMIN);
            ApplyFilters(new String[] { Consts.LOB1.WSGCAT }, TAB.ADMIN, Consts.LOB_COLUMN);
        }

        private void AdmAllLink_Click(object sender, RoutedEventArgs e)
        {
            FormatControls(btn_adminALL, TAB.ADMIN);
            ApplyFilters(new String[] { }, TAB.ADMIN, Consts.LOB_COLUMN);
        }

        private void FinGBSLink_Click(object sender, RoutedEventArgs e)
        {
            FormatControls(btn_financeGBS, TAB.FINANCE);
            ApplyFilters(Consts.LOB1.GBSHR, TAB.FINANCE, Consts.LOB_COLUMN);
        }

        private void FinGBTLink_Click(object sender, RoutedEventArgs e)
        {
            FormatControls(btn_financeGBT, TAB.FINANCE);
            ApplyFilters(new String[] { Consts.LOB1.GBT }, TAB.FINANCE, Consts.LOB_COLUMN);


        }

        private void FinGCPLink_Click(object sender, RoutedEventArgs e)
        {
            FormatControls(btn_financeGCP, TAB.FINANCE);
            ApplyFilters(new String[] { Consts.LOB1.GCP }, TAB.FINANCE, Consts.LOB_COLUMN);


        }

        private void FinPBMTLink_Click(object sender, RoutedEventArgs e)
        {
            FormatControls(btn_financePBMT, TAB.FINANCE);
            ApplyFilters(new String[] { Consts.LOB1.PBMT }, TAB.FINANCE, Consts.LOB_COLUMN);


        }

        private void FinWSGCATLink_Click(object sender, RoutedEventArgs e)
        {
            FormatControls(btn_financeWSGCAT, TAB.FINANCE);
            ApplyFilters(new String[] { Consts.LOB1.WSGCAT }, TAB.FINANCE, Consts.LOB_COLUMN);
        }

        private void FinAllLink_Click(object sender, RoutedEventArgs e)
        {
            FormatControls(btn_financeALL, TAB.FINANCE);
            ApplyFilters(new String[] { }, TAB.FINANCE, Consts.LOB_COLUMN);

        }


        #endregion

        private void btn_siteadmin_Click(object sender, RoutedEventArgs e)
        {
            Uri siteadmin = new Uri(Utils.GetSiteUrl()+"/SitePages/SiteAdmin.aspx");
            System.Windows.Browser.HtmlPage.Window.Navigate(siteadmin, "_parent");

        }

    }
    }

