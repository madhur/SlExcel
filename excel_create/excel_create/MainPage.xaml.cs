using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.SharePoint.Client;
using excel_create.Controls;
using System.Collections.ObjectModel;
using System.IO;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Resources;
using System.Windows.Browser;
using System.Text;
using Common;


namespace excel_create
{
  
    public partial class MainPage : UserControl
    {
        Web oWebsite;
        ListCollection collList;
        private List Idea;
        User user;
        private const string libName = "Idea Attachments";
        string folderName, newFolderName;
        private ClientContext myClContext;
        public SelectedFiles selectedFiles;
        String AimName;
        List<MyItem> items = new List<MyItem>();
        private const Int32 FILE_SIZE_LIMIT = 3145728;
        public SelectedAccounts selectedAccounts;
        public bool AllowMultiple { get; set; }

        public MainPage()
        {

            InitializeComponent();
            selectedFiles = new SelectedFiles();

            ConnectToSP();
            FileListBox.DataContext = selectedFiles;
            FileListBox.ItemsSource = selectedFiles;

            SilverlightOM();
            LoadComboItems();
            btn_draft.IsEnabled = true;
            btn_fp.IsEnabled = true;
            btn_next.IsEnabled = true;
            btn_approve.IsEnabled = true;
            btn_inprogress.IsEnabled = true;

            SinglePeopleChooser.UserTextBox.TextChanged+=UserTextBox_TextChanged;
            SinglePeopleChooser1.UserTextBox.TextChanged += UserTextBox_TextChanged;
            SinglePeopleChooser2.UserTextBox.TextChanged += UserTextBox_TextChanged;

        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
           // LoadItems();
        }


        public void SilverlightOM()
        {
            ClientContext clientContext = ClientContext.Current;

            oWebsite = clientContext.Web;


            clientContext.Load(oWebsite, s => s.CurrentUser);
            clientContext.ExecuteQueryAsync(loadonQuerySucceeded, loadonQueryFailed);
        }

        private void loadonQuerySucceeded(object sender, ClientRequestSucceededEventArgs args)
        {

            user = oWebsite.CurrentUser;

            Dispatcher.BeginInvoke(() => createdby.Text = user.Title);

        }

        private void loadonQueryFailed(object sender, ClientRequestFailedEventArgs args)
        {
            MessageBox.Show("Request failed");
        }

        private delegate void UpdateUIMethod();
        ChildWindow1 childwin;
        FTEMsgBox ftewin;
        close closewin;
        Messages msgwin;


       #region V A L I D A T I O N

        private void Ideanametextchanged(object sender, TextChangedEventArgs e)
        {
            if (ideaname.Text.Length > 0)
            {
                ideanameTxt.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void Descriptiontextchanged(object sender, TextChangedEventArgs e)
        {
            if (description.Text.Length > 0)
            {
                descriptionTxt.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
        private void Techimpacttextchanged(object sender, TextChangedEventArgs e)
        {
            if (tech_impact.Text.Length > 0)
            {
                techTxt.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
        private void LOB2textchanged(object sender, TextChangedEventArgs e)
        {

            if (resultsLOB2.Text.Length > 0)
            {
                gbsTxt.Foreground = new SolidColorBrush(Colors.Black);
                hrTxt.Foreground = new SolidColorBrush(Colors.Black);
                gbtTxt.Foreground = new SolidColorBrush(Colors.Black);
                wsgcatTxt.Foreground = new SolidColorBrush(Colors.Black);
                pbmtTxt.Foreground = new SolidColorBrush(Colors.Black);
            }

        }

        

        private void UserTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            Dictionary<TextBox, TextBlock> peoplePickers = GetPeoplePickerTextCollection();

            foreach (TextBox txtBox in peoplePickers.Keys)
            {
                if (txtBox.Text.Length != 0)
                    ResetFormatting(peoplePickers[txtBox]);
                //  else
                //      FormatControlForValidation(peoplePickers[txtBox]);
            }

        }

        #endregion 


        #region R A D I O  B U T T O N  LOGIC

        private void LOBRadioButtons_Checked(object sender, RoutedEventArgs e)
        {
            resultsLOB2.ClearValue(TextBox.TextProperty);
            lobt1Txt.Foreground = new SolidColorBrush(Colors.Black);


                if (gbs_radio.IsChecked == true)
                {
                    lobgbs.Visibility = Visibility.Visible;
                    lobpbmt.Visibility = Visibility.Collapsed;
                    lobwsgcat.Visibility = Visibility.Collapsed;
                    lobhr.Visibility = Visibility.Collapsed;
                    lobgbt.Visibility = Visibility.Collapsed;
                }
                else   if (gbt_radio.IsChecked == true)
                {
                    lobpbmt.Visibility = Visibility.Collapsed;
                    lobwsgcat.Visibility = Visibility.Collapsed;
                    lobhr.Visibility = Visibility.Collapsed;
                    lobgbs.Visibility = Visibility.Collapsed;
                    lobgbt.Visibility = Visibility.Visible;


                }
                else if (gcp_radio.IsChecked == true)
                {
                    lobpbmt.Visibility = Visibility.Collapsed;
                    lobwsgcat.Visibility = Visibility.Collapsed;
                    lobhr.Visibility = Visibility.Collapsed;
                    lobgbs.Visibility = Visibility.Collapsed;
                    lobgbt.Visibility = Visibility.Collapsed;
                    resultsLOB2.Text = "GCP";
                    

                }
                else if (hr_radio.IsChecked == true)
                {
                    lobhr.Visibility = Visibility.Visible;

                    lobpbmt.Visibility = Visibility.Collapsed;
                    lobwsgcat.Visibility = Visibility.Collapsed;
                    lobgbs.Visibility = Visibility.Collapsed;
                    lobgbt.Visibility = Visibility.Collapsed;

                }
                else if (pbmt_radio.IsChecked == true)
                {
                    lobpbmt.Visibility = Visibility.Visible;
                    lobwsgcat.Visibility = Visibility.Collapsed;
                    lobhr.Visibility = Visibility.Collapsed;
                    lobgbs.Visibility = Visibility.Collapsed;
                    lobgbt.Visibility = Visibility.Collapsed;

                }
                else if (wsgcat_radio.IsChecked == true)
                {
                    lobwsgcat.Visibility = Visibility.Visible;
                    lobpbmt.Visibility = Visibility.Collapsed;
                    lobhr.Visibility = Visibility.Collapsed;
                    lobgbs.Visibility = Visibility.Collapsed;
                    lobgbt.Visibility = Visibility.Collapsed;

                }
                //ButtonsEnableDisable();

            }




        private void LOBRadioButtons_Unchecked(object sender, RoutedEventArgs e)
        {


            if (gbs_radio.IsChecked == false)
            {
                lobgbs_gbt.IsChecked = false;
                lobgbs_gfo.IsChecked = false;
                lobgbs_grewe.IsChecked = false;
                lobgbs_gsm.IsChecked = false;
                lobgbs_other.IsChecked = false;
                lobgbs_tech.IsChecked = false;

                lobhr_benefits.IsChecked = false;
                lobhr_hr.IsChecked = false;
                lobhr_other.IsChecked = false;
                lobhr_pmo.IsChecked = false;
                lobhr_tech.IsChecked = false;

                lobpbmt_busmgmt.IsChecked = false;
                lobpbmt_pegasus.IsChecked = false;

                lobwsgcat_both.IsChecked = false;
                lobwsgcat_gca.IsChecked = false;
                lobwsgcat_ws.IsChecked = false;

                lobgbt_gbtjv.IsChecked = false;
                lobgbt_gcp.IsChecked = false;
                lobgbt_qms.IsChecked = false;

            }
            if (gbt_radio.IsChecked == false)
            {
                lobgbs_gbt.IsChecked = false;
                lobgbs_gfo.IsChecked = false;
                lobgbs_grewe.IsChecked = false;
                lobgbs_gsm.IsChecked = false;
                lobgbs_other.IsChecked = false;
                lobgbs_tech.IsChecked = false;

                lobhr_benefits.IsChecked = false;
                lobhr_hr.IsChecked = false;
                lobhr_other.IsChecked = false;
                lobhr_pmo.IsChecked = false;
                lobhr_tech.IsChecked = false;

                lobpbmt_busmgmt.IsChecked = false;
                lobpbmt_pegasus.IsChecked = false;

                lobwsgcat_both.IsChecked = false;
                lobwsgcat_gca.IsChecked = false;
                lobwsgcat_ws.IsChecked = false;

                lobgbt_gbtjv.IsChecked = false;
                lobgbt_gcp.IsChecked = false;
                lobgbt_qms.IsChecked = false;

            }
            if (gcp_radio.IsChecked == false)
            {
                lobgbs_gbt.IsChecked = false;
                lobgbs_gfo.IsChecked = false;
                lobgbs_grewe.IsChecked = false;
                lobgbs_gsm.IsChecked = false;
                lobgbs_other.IsChecked = false;
                lobgbs_tech.IsChecked = false;

                lobhr_benefits.IsChecked = false;
                lobhr_hr.IsChecked = false;
                lobhr_other.IsChecked = false;
                lobhr_pmo.IsChecked = false;
                lobhr_tech.IsChecked = false;

                lobpbmt_busmgmt.IsChecked = false;
                lobpbmt_pegasus.IsChecked = false;

                lobwsgcat_both.IsChecked = false;
                lobwsgcat_gca.IsChecked = false;
                lobwsgcat_ws.IsChecked = false;

                lobgbt_gbtjv.IsChecked = false;
                lobgbt_gcp.IsChecked = false;
                lobgbt_qms.IsChecked = false;

            }
            if (hr_radio.IsChecked == false)
            {
                lobgbs_gbt.IsChecked = false;
                lobgbs_gfo.IsChecked = false;
                lobgbs_grewe.IsChecked = false;
                lobgbs_gsm.IsChecked = false;
                lobgbs_other.IsChecked = false;
                lobgbs_tech.IsChecked = false;

                lobhr_benefits.IsChecked = false;
                lobhr_hr.IsChecked = false;
                lobhr_other.IsChecked = false;
                lobhr_pmo.IsChecked = false;
                lobhr_tech.IsChecked = false;

                lobpbmt_busmgmt.IsChecked = false;
                lobpbmt_pegasus.IsChecked = false;

                lobwsgcat_both.IsChecked = false;
                lobwsgcat_gca.IsChecked = false;
                lobwsgcat_ws.IsChecked = false;

                lobgbt_gbtjv.IsChecked = false;
                lobgbt_gcp.IsChecked = false;
                lobgbt_qms.IsChecked = false;

            }
            if (pbmt_radio.IsChecked == false)
            {
                lobgbs_gbt.IsChecked = false;
                lobgbs_gfo.IsChecked = false;
                lobgbs_grewe.IsChecked = false;
                lobgbs_gsm.IsChecked = false;
                lobgbs_other.IsChecked = false;
                lobgbs_tech.IsChecked = false;

                lobhr_benefits.IsChecked = false;
                lobhr_hr.IsChecked = false;
                lobhr_other.IsChecked = false;
                lobhr_pmo.IsChecked = false;
                lobhr_tech.IsChecked = false;

                lobpbmt_busmgmt.IsChecked = false;
                lobpbmt_pegasus.IsChecked = false;

                lobwsgcat_both.IsChecked = false;
                lobwsgcat_gca.IsChecked = false;
                lobwsgcat_ws.IsChecked = false;

                lobgbt_gbtjv.IsChecked = false;
                lobgbt_gcp.IsChecked = false;
                lobgbt_qms.IsChecked = false;

            }
            if (wsgcat_radio.IsChecked == false)
            {
                lobgbs_gbt.IsChecked = false;
                lobgbs_gfo.IsChecked = false;
                lobgbs_grewe.IsChecked = false;
                lobgbs_gsm.IsChecked = false;
                lobgbs_other.IsChecked = false;
                lobgbs_tech.IsChecked = false;

                lobhr_benefits.IsChecked = false;
                lobhr_hr.IsChecked = false;
                lobhr_other.IsChecked = false;
                lobhr_pmo.IsChecked = false;
                lobhr_tech.IsChecked = false;

                lobpbmt_busmgmt.IsChecked = false;
                lobpbmt_pegasus.IsChecked = false;

                lobwsgcat_both.IsChecked = false;
                lobwsgcat_gca.IsChecked = false;
                lobwsgcat_ws.IsChecked = false;

                lobgbt_gbtjv.IsChecked = false;
                lobgbt_gcp.IsChecked = false;
                lobgbt_qms.IsChecked = false;

            }
        }


        private void LOB2RadioButtons_Checked(object sender, RoutedEventArgs e)
        {
            if (gbs_radio.IsChecked == true)
            {
                for (int i = 0; i < this.lobgbs.Children.Count; i++)
                {

                    if (this.lobgbs.Children[i].GetType().Name == "RadioButton")
                    {
                        RadioButton radio = (RadioButton)this.lobgbs.Children[i];
                        if ((bool)radio.IsChecked)
                        {
                            resultsLOB2.Text = radio.Name.ToString();
                            gbsTxt.Foreground = new SolidColorBrush(Colors.Black);

                        }
                    }
                }
            }

            if (gbt_radio.IsChecked == true)
            {
                for (int i = 0; i < this.lobgbt.Children.Count; i++)
                {

                    if (this.lobgbt.Children[i].GetType().Name == "RadioButton")
                    {
                        RadioButton radio = (RadioButton)this.lobgbt.Children[i];
                        if ((bool)radio.IsChecked)
                        {
                            resultsLOB2.Text = radio.Name.ToString();
                            gbtTxt.Foreground = new SolidColorBrush(Colors.Black);

                        }
                    }
                }
            }

            if (pbmt_radio.IsChecked == true)
            {
                for (int i = 0; i < this.lobpbmt.Children.Count; i++)
                {

                    if (this.lobpbmt.Children[i].GetType().Name == "RadioButton")
                    {
                        RadioButton radio = (RadioButton)this.lobpbmt.Children[i];
                        if ((bool)radio.IsChecked)
                        {
                            resultsLOB2.Text = radio.Name.ToString();
                            pbmtTxt.Foreground = new SolidColorBrush(Colors.Black);

                        }
                    }
                }
            }
            if (wsgcat_radio.IsChecked == true)
            {
                for (int i = 0; i < this.lobwsgcat.Children.Count; i++)
                {

                    if (this.lobwsgcat.Children[i].GetType().Name == "RadioButton")
                    {
                        RadioButton radio = (RadioButton)this.lobwsgcat.Children[i];
                        if ((bool)radio.IsChecked)
                        {
                            resultsLOB2.Text = radio.Name.ToString();
                            wsgcatTxt.Foreground = new SolidColorBrush(Colors.Black);

                        }
                    }
                }
            }
            if (hr_radio.IsChecked == true)
            {
                for (int i = 0; i < this.lobhr.Children.Count; i++)
                {

                    if (this.lobhr.Children[i].GetType().Name == "RadioButton")
                    {
                        RadioButton radio = (RadioButton)this.lobhr.Children[i];
                        if ((bool)radio.IsChecked)
                        {
                            resultsLOB2.Text = radio.Name.ToString();
                            hrTxt.Foreground = new SolidColorBrush(Colors.Black);

                        }
                    }
                }
            }

            //ButtonsEnableDisable();
        }


                 //<<<~~~~~~~~Scope RADIO BUTTONS

        private void ScopeRadioButtons_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this.risk_stack.Children.Count; i++)
            {

                if (this.risk_stack.Children[i].GetType().Name == "RadioButton")
                {
                    RadioButton radio = (RadioButton)this.risk_stack.Children[i];
                    if ((bool)radio.IsChecked)
                    {
                        Risk.Text = radio.Name.ToString();
                        riskTxt.Foreground = new SolidColorBrush(Colors.Black);

                    }
                }
            }
            for (int i = 0; i < this.save_stack.Children.Count; i++)
            {

                if (this.save_stack.Children[i].GetType().Name == "RadioButton")
                {
                    RadioButton radio = (RadioButton)this.save_stack.Children[i];
                    if ((bool)radio.IsChecked)
                    {
                        Save.Text = radio.Name.ToString();
                        vendorTxt.Foreground = new SolidColorBrush(Colors.Black);

                    }
                }
            }
            for (int i = 0; i < this.cost_stack.Children.Count; i++)
            {

                if (this.cost_stack.Children[i].GetType().Name == "RadioButton")
                {
                    RadioButton radio = (RadioButton)this.cost_stack.Children[i];
                    if ((bool)radio.IsChecked)
                    {
                        CT.Text = radio.Name.ToString();
                        costTxt.Foreground = new SolidColorBrush(Colors.Black);

                    }
                }
            }
     
            for (int i = 0; i < this.identify_stack.Children.Count; i++)
            {

                if (this.identify_stack.Children[i].GetType().Name == "RadioButton")
                {
                    RadioButton radio = (RadioButton)this.identify_stack.Children[i];
                    if ((bool)radio.IsChecked)
                    {
                        Identify.Text = radio.Name.ToString();
                        identifyTxt.Foreground = new SolidColorBrush(Colors.Black);

                    }
                }
            }

            if (type_reEngineer.IsChecked == true)
            {
                tech_impact1.Visibility = Visibility.Visible;

            }

            if (type_Avoid.IsChecked == true || type_Growth.IsChecked == true || type_Reduction.IsChecked == true)
            {
                tech_impact1.Visibility = Visibility.Collapsed;
                tech_impact.ClearValue(TextBox.TextProperty);
            }

            //ButtonsEnableDisable();

        }
       

        #endregion

        #region H E L P  P O P U P S



        private void PopUpButton1_Click(object sender, RoutedEventArgs e)
        {
            myPopup_descrip.IsOpen = false;
        }

        private void PopUpButton2_Click(object sender, RoutedEventArgs e)
        {
            myPopup_executor.IsOpen = false;
        }

        private void PopUpButton3_Click(object sender, RoutedEventArgs e)
        {
            myPopup_lob1.IsOpen = false;
        }

        private void PopUpButton4_Click(object sender, RoutedEventArgs e)
        {
            myPopup_fte.IsOpen = false;
        }

        private void PopUpButton5_Click(object sender, RoutedEventArgs e)
        {
            myPopup_identify.IsOpen = false;
        }

        private void PopUpButton6_Click(object sender, RoutedEventArgs e)
        {
            myPopup_risk.IsOpen = false;
        }

        private void PopUpButton7_Click(object sender, RoutedEventArgs e)
        {
            myPopup_vendor.IsOpen = false;
        }

        private void PopUpButton8_Click(object sender, RoutedEventArgs e)
        {
            myPopup_tech.IsOpen = false;
        }

        private void PopUpButton9_Click(object sender, RoutedEventArgs e)
        {
            myPopup_firstmonth.IsOpen = false;
        }

        private void PopUpButton10_Click(object sender, RoutedEventArgs e)
        {
            myPopup_attach.IsOpen = false;
        }

        private void PopUpButton11_Click(object sender, RoutedEventArgs e)
        {
            myPopup_es.IsOpen = false;
        }

        private void PopUpButtonRole_Click(object sender, RoutedEventArgs e)
        {
            myPopup_role.IsOpen = false;
        }
       
        private void imghelp_description_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            PopUpText_descrip.Text = "Please provide high level description where someone not familiar with the idea can understand it.  Please do not include AXP Restricted or AXP Secret Information." + "\n" +
                "(*) indicates Required field";
            myPopup_descrip.IsOpen = true;
        }

        private void imghelp_executor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            PopUpText_executor.Text = "Individual responsible for execution of the idea and overall project contact." + "\n" +
                "(*) indicates Required field";
            myPopup_executor.IsOpen = true;
        }

        private void imghelp_lob1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            PopUpText_lob1.Text = "Please choose the Line of Business that will directly benefit from the savings.  If more than one, please enter separate ideas." + "\n" +
                "(*) indicates Required field";
            myPopup_lob1.IsOpen = true;
        }



        private void imghelp_fte_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Paragraph pgraph = new Paragraph();

            //create some text and add to the paragraph

            Run myText = new Run();
            myText.Text = "It’s important that all FTE’s who have contributed to the project are listed to ensure accurate recognition thru the EXCEL Rewards Program. Executor is already included. (*) Required if Applicable." + "\n" + "For more information:";
            pgraph.Inlines.Add(myText);

            //create the hyperlink
            Hyperlink hype = new Hyperlink();
            hype.Inlines.Add("Click Here");
            hype.NavigateUri = new Uri(Utils.GetSiteUrl()+"/Shared%20Documents/EXCEL%20Rewards.pptx");
            hype.Foreground = new SolidColorBrush(Colors.Blue);

            pgraph.Inlines.Add(hype);
            PopUpText_fte.Blocks.Add(pgraph);

            myPopup_fte.IsOpen = true;
        }

        private void imghelp_identify_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            PopUpText_identify.Text = "E - Excessive Demand - Wants more than needs.  Goal:  Eliminating non value-add requests." + "\n" + "\n" +
                             "X - eXpense Reduction - Over-Payment. Goal: Reduce total cost of technology ownership to open up new investment spend. Eliminate environments not providing ROI. CEM Midrange and Mainframe reductions. TIMS and Direct Tech reductions." + "\n" + "\n" +
                             "C - Customization Reduction - Over Customization. Goal: Aspire to achieve 90% standardized solutions." + "\n" + "\n" +
                             "E - Effective Talent Utilization - Under-utilization of resources. Goal: Maintain the optimal mix of variable and permanent resources to minimize cost of routine work and maximize longer term benefit of employee base. Direct Tech reductions." + "\n" + "\n" +
                             "L - Less Duplication - Duplicaton and Rework. Goal: Eliminate duplicate work demand." + "\n" + "\n" +
                             "(*) indicates Required field";
            myPopup_identify.IsOpen = true;
        }

        private void imghelp_risk_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            PopUpText_risk.Text = "What's the risk of you delivering on this initiative?" + "\n" +
                "(*) indicates Required field";
            myPopup_risk.IsOpen = true;
        }

        private void imghelp_vendor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            PopUpText_vendor.Text = "Save that originates from and managed by the Vendor that goes towards their contracted goal." + "\n" +
                "(*) indicates Required field";
            myPopup_vendor.IsOpen = true;
        }

        private void imghelp_role_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            PopUpText_role.Text = "Role Family is used by roles that have additional role-based EXCEL goals. This is a subset of the overall portfolio EXCEL goals. For typical end user, selection will be 'None'.";
            myPopup_role.IsOpen = true;
        }

        private void imghelp_tech_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            PopUpText_tech.Text = "Please list all Midrange Servers, Mainframe HLQ's or Direct Tech Tier 3 names that will be impacted." + "\n" +
                "(*) indicates Required field if Technology is impacted";
            myPopup_tech.IsOpen = true;
        }

        private void imghelp_firstmonth_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            PopUpText_firstmonth.Text = "The first month when the savings takes effect (i.e. when billing stops, etc.).  Field is locked after initial save.  If entering Today's date or previous date, user can only Save as Draft or Submit for Approval.";
            myPopup_firstmonth.IsOpen = true;
        }

       

        private void imghelp_attach_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            PopUpText_attach.Text = "Objective is to justify the savings benefit of the idea.  Amounts in attachment must match amounts in Estimated Savings section.  Special characters (&, %, etc) in file name not allowed." + "\n" +
                "(*) indicates Required field to Submit for Approval";
            myPopup_attach.IsOpen = true;
        }

        private void imghelp_estimatedsavings_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            PopUpText_es.Text = "Should not exceed 12 rolling months including future year.  Dependent on how cost works - could be monthly or one time save based on how cost is applied." + "\n" +
                "(*) indicates Required field";
            myPopup_es.IsOpen = true;
        }

        ///COST TYPE CHILD WINDOW BEGIN
        
    
        private void help_type_Click(object sender, RoutedEventArgs e)
        {
            childwin = new ChildWindow1();
            childwin.Height = 300;
            childwin.Width = 600;
            childwin.Show();
            childwin.SubmitClicked += new EventHandler(type_SubmitClicked);
            
        }

        void type_SubmitClicked(object sender, EventArgs e)
        {
            if (childwin.result.Text == "1")
            {
                type_Avoid.IsChecked = true;
            }

            else if (childwin.result.Text == "2")
            {
                type_reEngineer.IsChecked = true;
            }
            else if (childwin.result.Text == "3")
            {
                type_Reduction.IsChecked = true;
            }
            else if (childwin.result.Text == "4")
            {
                type_Growth.IsChecked = true;

            }

        }

      

        #endregion

        #region N A V   B U T T O N S

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {

             closewin = new close();

                closewin.Height = 200;
                closewin.Width = 500;
                closewin.Show();             
                
                closewin.SubmitClicked += new EventHandler(UserControl_Unloaded_1);

        }
        
    
        private void btn_next_Click(object sender, System.Windows.RoutedEventArgs e)
        {
          
                this.tabcontrol1.SelectedIndex = 1;
                btnstack_overview.Visibility = Visibility.Collapsed;
                btnstack_overviewClose.Visibility = Visibility.Collapsed;

                overview_image.Visibility = Visibility.Collapsed;

                scope_image.Visibility = Visibility.Visible;
                btnstack_scope.Visibility = Visibility.Visible;
                btnstack_scopeBack.Visibility = Visibility.Visible;

                btn_fp.Visibility = Visibility.Collapsed;
                btn_inprogress.Visibility = Visibility.Collapsed;
                btn_approve.Visibility = Visibility.Collapsed;

        }


        private void btn_next2_Click(object sender, RoutedEventArgs e)
        {

             if ( firstmonth.Text.Length == 0)
            {
                NavigateFinancialTab();

              


            }
             else     if (firstmonth.SelectedDate < DateTime.Now)
             {
                 NavigateFinancialTab();

             }

                      else     if ( firstmonth.SelectedDate > DateTime.Now)
             {

                 NavigateFinancialTab();
          

             }

             else if (firstmonth.SelectedDate < DateTime.Now)
             {
                 NavigateFinancialTab();


             }


          

        }



        private void btn_next3_Click(object sender, RoutedEventArgs e)
        {


            this.tabcontrol1.SelectedIndex = 3;
            btnstack_scope.Visibility = Visibility.Collapsed;
            btnstack_scopeBack.Visibility = Visibility.Collapsed;

            btnstack_overview.Visibility = Visibility.Collapsed;
            btnstack_financial.Visibility = Visibility.Collapsed;
            btnstack_comments.Visibility = Visibility.Visible;            
            btn_comments.IsEnabled = true;
            comments_image.Visibility = Visibility.Visible;
            financials_image.Visibility = Visibility.Collapsed;
            overview_image.Visibility = Visibility.Collapsed;
            scope_image.Visibility = Visibility.Collapsed;



        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {

            

            NavigateOverviewTab();
        }

        private void btn_back2_Click(object sender, RoutedEventArgs e)
        {

           

            NavigateScopeTab();


        }

        private void btn_back3_Click(object sender, RoutedEventArgs e)
        {

            if (ideaname.Text.Length == 0 || description.Text.Length == 0 || SinglePeopleChooser.UserTextBox.Text.Length == 0 || SinglePeopleChooser1.UserTextBox.Text.Length == 0 || SinglePeopleChooser2.UserTextBox.Text.Length == 0 || resultsLOB2.Text.Length == 0)
            {
                NavigateOverviewTab();

           
            }

            else if (Identify.Text.Length == 0 || Risk.Text.Length == 0 || Save.Text.Length == 0 || CT.Text.Length == 0)
            {
                
                NavigateScopeTab();
           

            }
            else if (Identify.Text.Length == 0 || Risk.Text.Length == 0 || Save.Text.Length == 0 || CT.Text == "type_reEngineer" && tech_impact.Text.Length == 0)
            {
                NavigateScopeTab();


            }
            else
            {

                if ( firstmonth.Text.Length == 0)
                {

                    NavigateFinancialTab();
                  


                }
                else if (firstmonth.SelectedDate < DateTime.Now)
                {
                    NavigateFinancialTab();

                 


                }

                else if ( firstmonth.SelectedDate > DateTime.Now)
                {
                    NavigateFinancialTab();

                   


                }

                else if ( firstmonth.SelectedDate < DateTime.Now)
                {

                    NavigateFinancialTab();
                


                }


            }
        }

        private void btn_comments_Click(object sender, RoutedEventArgs e)
        {

            this.tabcontrol1.SelectedIndex = 3;

            btnstack_scope.Visibility = Visibility.Collapsed;
            btnstack_scopeBack.Visibility = Visibility.Collapsed;

            btnstack_overview.Visibility = Visibility.Collapsed;
            btnstack_overviewClose.Visibility = Visibility.Collapsed;

            btnstack_financial.Visibility = Visibility.Collapsed;

            btnstack_comments.Visibility = Visibility.Visible;

            btn_comments.IsEnabled = false;

            comments_image.Visibility = Visibility.Visible;
            financials_image.Visibility = Visibility.Collapsed;
            overview_image.Visibility = Visibility.Collapsed;
            scope_image.Visibility = Visibility.Collapsed;

      

        }




        #endregion

        #region ///R E Q U I R E D     L O G I C ///
        private ValidateResult ValidateForDraft()
        {
            ValidateResult result = new ValidateResult();
            Dictionary<TextBox, TextBlock> allControls = GetOverviewTabCollection();
            TextBox[] peoplePickers = GetPeoplePickerCollection();

            if (SinglePeopleChooser.selectedAccounts.Count > 0 && SinglePeopleChooser1.selectedAccounts.Count > 0 && SinglePeopleChooser2.selectedAccounts.Count > 0)
            {
                // Resolve any pending changes on people picker
                if (SinglePeopleChooser.UserTextBox.Text.Equals(SinglePeopleChooser.selectedAccounts[0].DisplayName) && SinglePeopleChooser1.UserTextBox.Text.Equals(SinglePeopleChooser1.selectedAccounts[0].DisplayName) &&
                    SinglePeopleChooser2.UserTextBox.Text.Equals(SinglePeopleChooser2.selectedAccounts[0].DisplayName))
                {
                    ;
                }
                else
                {
                    SetFalseResult(result, TAB.OVERVIEW);
                }
            }


            foreach (TextBox txtBox in peoplePickers)
            {
                SolidColorBrush brush = txtBox.BorderBrush as SolidColorBrush;
                if (brush != null)
                {
                    if (brush.Color == Colors.Red)
                        SetFalseResult(result, TAB.OVERVIEW);
                }

            }

            foreach (TextBox txtBox in allControls.Keys)
            {
                if (txtBox.Text.Length == 0)
                {
                    SetFalseResult(result, TAB.OVERVIEW);
                    FormatControlForValidation(allControls[txtBox]);
                }
            }



            result = CheckLobMapping(result);



            return result;
        }

        private ValidateResult CheckLobMapping(ValidateResult result)
        {
            TextBlock[] radioBlocks = new TextBlock[] { gbsTxt, gbtTxt, wsgcatTxt, hrTxt, pbmtTxt };
            Dictionary<RadioButton, List<RadioButton>> lobMapping = GetLOBMapping();
            bool isLob1set = false, isLob2set = false;
            RadioButton selectedRadio = null;

            foreach (RadioButton radio in lobMapping.Keys)
            {
                if (radio.IsChecked == true)
                {
                    isLob1set = true;
                    selectedRadio = radio;
                }

            }



            if (!isLob1set)
            {
                SetFalseResult(result, TAB.OVERVIEW);
                FormatControlForValidation(lobt1Txt);
                return result;

            }

            // GCP radio button has no mapping
            if (selectedRadio == gcp_radio)
                return result;


            List<RadioButton> mappedRadios = lobMapping[selectedRadio];

            foreach (RadioButton mappedRadio in mappedRadios)
            {
                if (mappedRadio.IsChecked == true)
                {

                    isLob2set = true;

                }
            }


            if (!isLob2set)
            {
                SetFalseResult(result, TAB.OVERVIEW);

                foreach (TextBlock txtBlock in radioBlocks)
                    FormatControlForValidation(txtBlock);

            }

            return result;

        }

        private Dictionary<RadioButton, List<RadioButton>> GetLOBMapping()
        {

            Dictionary<RadioButton, List<RadioButton>> lobMapping = new Dictionary<RadioButton, List<RadioButton>>()
        {
            { gbs_radio, new List<RadioButton> {lobgbs_gbt, lobgbs_gfo , lobgbs_grewe, lobgbs_gsm, lobgbs_tech, lobgbs_other   }},
            {gbt_radio, new List<RadioButton> { lobgbt_gcp, lobgbt_qms, lobgbt_gbtjv}},
            {gcp_radio, new List<RadioButton> { }},
            {hr_radio, new List<RadioButton> {lobhr_hr, lobhr_tech, lobhr_pmo, lobhr_benefits, lobhr_other }},
            {pbmt_radio, new List<RadioButton> {lobpbmt_pegasus, lobpbmt_busmgmt    }},
            {wsgcat_radio, new List<RadioButton> { lobwsgcat_ws, lobwsgcat_gca, lobwsgcat_both   }}



        };

            return lobMapping;

        }


        private void SetFalseResult(ValidateResult result, TAB tab)
        {
            result.IsValid = false;

            if (!result.FaultTab.Contains(tab))
            {
                result.FaultTab.Add(tab);
                result.FaultTab.Sort(new SortTab());
            }
        }


        private ValidateResult ValidateForInProgress()
        {
            return ValidateForInProgress(null);
        }

        private ValidateResult ValidateForInProgress(ValidateResult previousResult)
        {
            ValidateResult result;

            if (previousResult == null)
                result = new ValidateResult();
            else
                result = previousResult;

            /* Validate Financial Tabs values first and then scope tab values*/

            if (totalText.Text == "$0.00" || totalText.Text == "0.00" || totalText.Text == "0")
            {
                SetFalseResult(result, TAB.FINANCIAL);
                FormatControlForValidation(savingsTxt);
            }

            if (firstmonth.Text.Length == 0)
            {
                SetFalseResult(result, TAB.FINANCIAL);
                FormatControlForValidation(firstmonthTxt);

            }

            /* Validate scope tabs values */

            if (CT.Text == "type_reEngineer" && tech_impact.Text.Length == 0)
            {
                SetFalseResult(result, TAB.SCOPE);
                FormatControlForValidation(techTxt);
            }

            Dictionary<StackPanel, TextBlock> scopeControls = GetScopeTabCollection();

            foreach (StackPanel panel in scopeControls.Keys)
            {
                RadioButton selectedRadio = GetCheckedRadio(panel);
                if (selectedRadio == null)
                {
                    SetFalseResult(result, TAB.SCOPE);
                    FormatControlForValidation(scopeControls[panel]);
                }


            }

            return result;

        }

        private ValidateResult ValidateForApproval()
        {
            return ValidateForApproval(null);
        }

        private ValidateResult ValidateForApproval(ValidateResult previousResult)
        {
            ValidateResult result;

            if (previousResult == null)
                result = new ValidateResult();
            else
                result = previousResult;



            if (FileListBox.Items.Count == 0)
            {
                SetFalseResult(result, TAB.FINANCIAL);
                FormatControlForValidation(attachTxt);
            }

            return result;
        }


        private void FormatControlForValidation(TextBlock txtBlock)
        {
            txtBlock.Foreground = new SolidColorBrush(Colors.Red);
        }

        private void ResetFormatting(TextBlock txtBlock)
        {
            txtBlock.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void ResetControls()
        {
            TextBlock[] allControls = GetAllControlsCollection();

            foreach (TextBlock txtBlock in allControls)
                ResetFormatting(txtBlock);
        }

        private TextBlock[] GetAllControlsCollection()
        {
            TextBlock[] allControls = new TextBlock[] { ideanameTxt, descriptionTxt, executorTxt, directorTxt, vpTxt, identifyTxt, riskTxt, vendorTxt, costTxt, gbsTxt, gbtTxt, wsgcatTxt, hrTxt, pbmtTxt, savingsTxt, attachTxt, techTxt, lobt1Txt, firstmonthTxt };
            return allControls;

        }

        private Dictionary<TextBox, TextBlock> GetOverviewTabCollection()
        {
            Dictionary<TextBox, TextBlock> allControls = new Dictionary<TextBox, TextBlock>()
            {
                { ideaname, ideanameTxt },
                {description, descriptionTxt}, 
                {SinglePeopleChooser.UserTextBox , executorTxt},
                {SinglePeopleChooser1.UserTextBox, directorTxt},
                {SinglePeopleChooser2.UserTextBox, vpTxt},
                
        };
            return allControls;

        }

        private Dictionary<TextBox, TextBlock> GetPeoplePickerTextCollection()
        {
            Dictionary<TextBox, TextBlock> allControls = new Dictionary<TextBox, TextBlock>()
            {
                
                {SinglePeopleChooser.UserTextBox , executorTxt},
                {SinglePeopleChooser1.UserTextBox, directorTxt},
                {SinglePeopleChooser2.UserTextBox, vpTxt},
                
        };
            return allControls;

        }



        private TextBox[] GetPeoplePickerCollection()
        {
            TextBox[] textBoxes = new TextBox[] { SinglePeopleChooser.UserTextBox, SinglePeopleChooser1.UserTextBox, SinglePeopleChooser2.UserTextBox };
            return textBoxes;
        }

        private Dictionary<StackPanel, TextBlock> GetScopeTabCollection()
        {
            Dictionary<StackPanel, TextBlock> scopeControls = new Dictionary<StackPanel, TextBlock>()
            {
                { identify_stack, identifyTxt  },
                {risk_stack, riskTxt }, 
                {save_stack, vendorTxt },
                {cost_stack, costTxt }
             
                
        };

            return scopeControls;

        }

        private void NavigateOverviewTab()
        {
            this.tabcontrol1.SelectedIndex = 0;
            btnstack_overview.Visibility = Visibility.Visible;
            btnstack_overviewClose.Visibility = Visibility.Visible;
            overview_image.Visibility = Visibility.Visible;

            scope_image.Visibility = Visibility.Collapsed;
            btnstack_scope.Visibility = Visibility.Collapsed;
            btnstack_scopeBack.Visibility = Visibility.Collapsed;

            btnstack_financial.Visibility = Visibility.Collapsed;
            financials_image.Visibility = Visibility.Collapsed;


            comments_image.Visibility = Visibility.Collapsed;
            btnstack_comments.Visibility = Visibility.Collapsed;
            btn_comments.IsEnabled = true;



            btn_inprogress.Visibility = Visibility.Collapsed;
            btn_approve.Visibility = Visibility.Collapsed;
            btn_fp.Visibility = Visibility.Collapsed;



        }

        private void NavigateScopeTab()
        {
            this.tabcontrol1.SelectedIndex = 1;

            btnstack_overview.Visibility = Visibility.Collapsed;
            btnstack_overviewClose.Visibility = Visibility.Collapsed;
            overview_image.Visibility = Visibility.Collapsed;

            scope_image.Visibility = Visibility.Visible;
            btnstack_scope.Visibility = Visibility.Visible;
            btnstack_scopeBack.Visibility = Visibility.Visible;

            btnstack_financial.Visibility = Visibility.Collapsed;
            financials_image.Visibility = Visibility.Collapsed;


            comments_image.Visibility = Visibility.Collapsed;
            btnstack_comments.Visibility = Visibility.Collapsed;
            btn_comments.IsEnabled = true;



            btn_inprogress.Visibility = Visibility.Collapsed;
            btn_approve.Visibility = Visibility.Collapsed;
            btn_fp.Visibility = Visibility.Collapsed;



        }

        private void NavigateFinancialTab()
        {
          
                btn_fp.Visibility = Visibility.Visible;
                btn_inprogress.Visibility = Visibility.Visible;
                btn_approve.Visibility = Visibility.Visible;


                this.tabcontrol1.SelectedIndex = 2;
                btnstack_scope.Visibility = Visibility.Collapsed;
                btnstack_scopeBack.Visibility = Visibility.Collapsed;
                btnstack_financial.Visibility = Visibility.Visible;


                btnstack_comments.Visibility = Visibility.Collapsed;
                btn_comments.IsEnabled = true;

                scope_image.Visibility = Visibility.Collapsed;
                financials_image.Visibility = Visibility.Visible;

        }


        private RadioButton GetCheckedRadio(StackPanel panel)
        {
            for (int i = 0; i < panel.Children.Count; i++)
            {
                if (panel.Children[i].GetType().Name == "RadioButton")
                {
                    RadioButton radio = (RadioButton)panel.Children[i];
                    if ((bool)radio.IsChecked)
                    {
                        return radio;
                    }
                }
            }

            return null;

        }


        private String GetErrorMessage(ValidateResult result)
        {
            StringBuilder errorMsg = new StringBuilder(Consts.ERROR_MSG);

            foreach (TAB tab in result.FaultTab)
            {
                switch (tab)
                {
                    case TAB.OVERVIEW:
                        errorMsg.Append(Consts.OVERVIEW_TAB);
                        break;

                    case TAB.SCOPE:
                        errorMsg.Append(Consts.SCOPE_TAB);
                        break;

                    case TAB.FINANCIAL:
                        errorMsg.Append(Consts.FINANCIALS_TAB);
                        break;

                }
            }

            return errorMsg.ToString();

        }

        private void NavigateTab(ValidateResult result)
        {

            if (result.FaultTab.Contains(TAB.OVERVIEW))
                NavigateOverviewTab();

            else if (result.FaultTab.Contains(TAB.SCOPE))
                NavigateScopeTab();
            else
                NavigateFinancialTab();


        }

        private Messages GetErrorWindow(ValidateResult result)
        {
            Messages msgwin = new Messages();
            msgwin.msgtxt.Text = GetErrorMessage(result);
            msgwin.alert.Visibility = Visibility.Visible;
            msgwin.RequiredOKButton.Visibility = Visibility.Visible;
            return msgwin;

        }

        #endregion REQUIRED LOGIC 


        #region S T A T U S   B U T T O N S

        //Begin Status Buttons

        User Singleuser;
        User Singleuser1;
        User Singleuser2;

 


         private void btn_draft_Click(object sender, RoutedEventArgs e)
         {
             ResetControls();

             ValidateResult result = ValidateForDraft();
             if (!result.IsValid)
             {

                 GetErrorWindow(result).Show();
                 NavigateTab(result);
             }

             else
             {
                 btn_draft.IsEnabled = false;
                 btn_approve.IsEnabled = false;
                 btn_fp.IsEnabled = false;
                 btn_inprogress.IsEnabled = false;
                 //Get the current context 
                 ClientContext context = ClientContext.Current;
                 //Get the Idea list and add a new item 
                 List Idea = context.Web.Lists.GetByTitle("Idea");
                 ListItem newItem = Idea.AddItem(new ListItemCreationInformation());
                 //Set the new item's properties 


                 if (SinglePeopleChooser.selectedAccounts.Count > 0 || MultiplePeopleChooser.selectedAccounts.Count > 0)
                 {

                     if (SinglePeopleChooser.selectedAccounts.Count > 0)
                     {
                         Singleuser = context.Web.EnsureUser(SinglePeopleChooser.selectedAccounts[0].AccountName);
                         newItem["Executor"] = Singleuser;

                         Singleuser1 = context.Web.EnsureUser(SinglePeopleChooser1.selectedAccounts[0].AccountName);
                         newItem["Director"] = Singleuser1;

                         Singleuser2 = context.Web.EnsureUser(SinglePeopleChooser2.selectedAccounts[0].AccountName);
                         newItem["VP"] = Singleuser2;

                     }
                     if (MultiplePeopleChooser.selectedAccounts.Count > 0)
                     {
                         List<FieldUserValue> usersList = new List<FieldUserValue>();
                         foreach (AccountList ac in MultiplePeopleChooser.selectedAccounts)
                         {
                             usersList.Add(FieldUserValue.FromUser(ac.AccountName));
                         }

                         newItem["FTE_x0020_Contributors"] = usersList;
                     }

                 }


                 //<-----Project Overview Tab ------>
                 newItem["Idea_x0020_Name"] = ideaname.Text;
                 newItem["EXCEL_x0020_Idea_x0020_Descripti"] = description.Text;
              
                 newItem["scale"] = "1";

                 SaveRadios(newItem);
                
                 //<------Scope Tab------>

                 MyItem item = aimcombo.SelectedItem as MyItem;
                 if (item != null)
                     newItem["AIM_x0020_Application_x0020_Name"] = item.AIM_NAME;

                // newItem["AIM_x0020_Application_x0020_Name"] = aimcombo.SelectionBoxItem;
                 newItem["AIM_x0020_Application_x0020_ID"] = AIM_ID.Text;
                 newItem["_x0031_st_x0020_Mo_x0020_Saves_x"] = firstmonth.SelectedDate;


                 if (identify_e.IsChecked == true)
                 {
                     newItem["EXCEL_x0020_Identifier"] = "1. E-Excessive Demand";

                 }
                 else if (identify_x.IsChecked == true)
                 {
                     newItem["EXCEL_x0020_Identifier"] = "2. X-eXpense Reduction";
                 }
                 else if (identify_c.IsChecked == true)
                 {
                     newItem["EXCEL_x0020_Identifier"] = "3. C–Customization Reduction";
                 }
                 else if (identify_e2.IsChecked == true)
                 {
                     newItem["EXCEL_x0020_Identifier"] = "4. E–Effective Talent Utilization";
                 }
                 else if (identify_l.IsChecked == true)
                 {
                     newItem["EXCEL_x0020_Identifier"] = "5. L–Less Duplication";
                 }

                 //Assumptions, Risk, Business Capability, SDLC
                 newItem["Assumptions_x0020_or_x0020_Depen"] = assump_depend.Text;

                 if (risk_high.IsChecked == true)
                 {
                     newItem["Risk_x0020_of_x0020_Implementati"] = "High";
                 }

                 else if (risk_med.IsChecked == true)
                 {
                     newItem["Risk_x0020_of_x0020_Implementati"] = "Medium";
                 }
                 else if (risk_low.IsChecked == true)
                 {
                     newItem["Risk_x0020_of_x0020_Implementati"] = "Low";
                 }

                 newItem["Business_x0020_Capability"] = biz_capability.Text;
                 newItem["SDLC_x0020_Project_x0020_ID"] = sdlc_projID.Text;
                 newItem["SDLC_x0020_Project_x0020_Name"] = sdlc_projName.Text;

                 //vendor save
                 if (vendorSave_yes.IsChecked == true)
                 {
                     newItem["Vendor_Save"] = "Yes";
                 }

                 else if (vendorSave_no.IsChecked == true)
                 {
                     newItem["Vendor_Save"] = "No";
                 }

                 //Cost Type
                 if (type_Avoid.IsChecked == true)
                 {
                     newItem["Cost_x0020_Type1"] = "Cost Avoidance";
                 }

                 else if (type_reEngineer.IsChecked == true)
                 {
                     newItem["Cost_x0020_Type1"] = "Re-engineering (REE)";
                     newItem["Tech_Impact"] = tech_impact.Text;
                 }

                 else if (type_Reduction.IsChecked == true)
                 {
                     newItem["Cost_x0020_Type1"] = "Cost Reduction";
                 }
                 else if (type_Growth.IsChecked == true)
                 {
                     newItem["Cost_x0020_Type1"] = "Growth Reduction";
                 }

                 //<-----estimated savings----->

                 newItem["SavingsHeader1"] = header1.Text;
                 newItem["SavingsHeader2"] = header2.Text;
                 newItem["SavingsHeader3"] = header3.Text;
                 newItem["SavingsHeader4"] = header4.Text;
                 newItem["SavingsHeader5"] = header5.Text;

                 newItem["Savings1"] = es1.Value;
                 newItem["Savings2"] = es2.Value;
                 newItem["Savings3"] = es3.Value;
                 newItem["Savings4"] = es4.Value;
                 newItem["Savings5"] = es5.Value;

                 newItem["Total_x0020_Savings"] = es_Total.Value;

                 //<-----comments, status & audit----->

                 newItem["Project_x0020_Comments"] = projcomText.Text;
                 newItem["Idea_x0020_Status"] = "Draft";
                 newItem["Audit"] = createdby.Text + " - " + DateTime.Now + " - " + "swuccessfully submitted the idea as a draft.";

                 newItem.Update();
                 //Load the list 
                 context.Load(Idea, list => list.Title);

                 //Execute the query to create the new item 
                 context.ExecuteQueryAsync((s, ee) =>
                 {
                     string itemId = newItem.Id.ToString();

                     RenameFolder(Utils.GetSiteUrl(), libName, string.Empty, folderName, itemId);

                     Dispatcher.BeginInvoke(() =>
                     {
                         newFolderName = itemId;
                         msgwin= new Messages();
                         msgwin.RequiredOKButton.Visibility = Visibility.Collapsed;
                         msgwin.alert.Visibility = Visibility.Collapsed;
                         msgwin.msgtxt.Text = "Your idea was successfully saved as a draft.";
                         msgwin.Show();


                     }
                         );



                 },
     (s, ee) =>
     {
         Console.WriteLine(ee.Message);

     });


             }
         }


         private void SaveRadios(ListItem updateItem)
         {

             if (gbs_radio.IsChecked == true)
             {

                 updateItem["Line_x0020_Of_x0020_Business_x001"] = "GBS";

                 if (lobgbs_gbt.IsChecked == true)
                 {
                     updateItem["LOB_Tier2"] = "GBT";

                 }
                 else if (lobgbs_gfo.IsChecked == true)
                 {
                     updateItem["LOB_Tier2"] = "GFO";
                 }
                 else if (lobgbs_grewe.IsChecked == true)
                 {
                     updateItem["LOB_Tier2"] = "GREWE";
                 }
                 else if (lobgbs_gsm.IsChecked == true)
                 {
                     updateItem["LOB_Tier2"] = "GSM";
                 }
                 else if (lobgbs_tech.IsChecked == true)
                 {
                     updateItem["LOB_Tier2"] = "Tech";
                 }
                 else if (lobgbs_other.IsChecked == true)
                 {
                     updateItem["LOB_Tier2"] = "Other External Group";

                 }

             }
             else if (gbt_radio.IsChecked == true)
             {
                 updateItem["Line_x0020_Of_x0020_Business_x001"] = "GBT";

                 if (lobgbt_gbtjv.IsChecked == true)
                 {
                     updateItem["LOB_Tier2"] = "GBT/JV";
                 }
                 else if (lobgbt_gcp.IsChecked == true)
                 {
                     updateItem["LOB_Tier2"] = "GCP";
                 }
                 else if (lobgbt_qms.IsChecked == true)
                 {
                     updateItem["LOB_Tier2"] = "QMS";
                 }

             }
             else if (gcp_radio.IsChecked == true)
             {
                 updateItem["Line_x0020_Of_x0020_Business_x001"] = "GCP";
                 updateItem["LOB_Tier2"] = "";

             }
             else if (hr_radio.IsChecked == true)
             {
                 updateItem["Line_x0020_Of_x0020_Business_x001"] = "HR";

                 if (lobhr_hr.IsChecked == true)
                 {
                     updateItem["LOB_Tier2"] = "HR";

                 }
                 else if (lobhr_benefits.IsChecked == true)
                 {
                     updateItem["LOB_Tier2"] = "HR Benefits";
                 }
                 else if (lobhr_pmo.IsChecked == true)
                 {
                     updateItem["LOB_Tier2"] = "PMO Only";
                 }
                 else if (lobhr_tech.IsChecked == true)
                 {
                     updateItem["LOB_Tier2"] = "Tech";
                 }

                 else if (lobhr_other.IsChecked == true)
                 {
                     updateItem["LOB_Tier2"] = "Other External Group";

                 }
             }
             else if (pbmt_radio.IsChecked == true)
             {
                 updateItem["Line_x0020_Of_x0020_Business_x001"] = "PBMT";

                 if (lobpbmt_pegasus.IsChecked == true)
                 {
                     updateItem["LOB_Tier2"] = "GCST Pegasus";
                 }
                 else if (lobpbmt_busmgmt.IsChecked == true)
                 {
                     updateItem["LOB_Tier2"] = "Business Management and Transformation";
                 }

             }
             else if (wsgcat_radio.IsChecked == true)
             {
                 updateItem["Line_x0020_Of_x0020_Business_x001"] = "WSGCAT";

                 if (lobwsgcat_ws.IsChecked == true)
                 {
                     updateItem["LOB_Tier2"] = "WS";
                 }
                 else if (lobwsgcat_gca.IsChecked == true)
                 {
                     updateItem["LOB_Tier2"] = "GCA";
                 }
                 else if (lobwsgcat_both.IsChecked == true)
                 {
                     updateItem["LOB_Tier2"] = "WS and GCA";
                 }

             }

         }

        //<~~~~~~~~FUTURE PIPELINE~~~~~~~~>
         private void btn_future_Click(object sender, RoutedEventArgs e)
         {
             if (firstmonth.SelectedDate <= DateTime.Today.AddDays(-1))
             {

                 ShowPastDateError();
             }

             else
             {


                 ResetControls();
                 ValidateResult draftResult = ValidateForDraft();
                 ValidateResult result = ValidateForInProgress(draftResult);

                 if (!result.IsValid)
                 {

                     GetErrorWindow(result).Show();
                     NavigateTab(result);
                 }

                 else
                 {
                     btn_draft.IsEnabled = false;
                     btn_approve.IsEnabled = false;
                     btn_fp.IsEnabled = false;
                     btn_inprogress.IsEnabled = false;                 //Get the current context 
                     ClientContext context = ClientContext.Current;
                     //Get the Idea list and add a new item 
                     Idea = context.Web.Lists.GetByTitle("Idea");
                     ListItem newItem = Idea.AddItem(new ListItemCreationInformation());
                     //Set the new item's properties 

                     if (SinglePeopleChooser.selectedAccounts.Count > 0 || MultiplePeopleChooser.selectedAccounts.Count > 0)
                     {

                         if (SinglePeopleChooser.selectedAccounts.Count > 0)
                         {
                             Singleuser = context.Web.EnsureUser(SinglePeopleChooser.selectedAccounts[0].AccountName);
                             newItem["Executor"] = Singleuser;

                             Singleuser1 = context.Web.EnsureUser(SinglePeopleChooser1.selectedAccounts[0].AccountName);
                             newItem["Director"] = Singleuser1;

                             Singleuser2 = context.Web.EnsureUser(SinglePeopleChooser2.selectedAccounts[0].AccountName);
                             newItem["VP"] = Singleuser2;

                         }
                         if (MultiplePeopleChooser.selectedAccounts.Count > 0)
                         {
                             List<FieldUserValue> usersList = new List<FieldUserValue>();
                             foreach (AccountList ac in MultiplePeopleChooser.selectedAccounts)
                             {
                                 usersList.Add(FieldUserValue.FromUser(ac.AccountName));
                             }

                             newItem["FTE_x0020_Contributors"] = usersList;
                         }

                     }

                     //<-----Project Overview Tab ------>
                     newItem["Idea_x0020_Name"] = ideaname.Text;
                     newItem["EXCEL_x0020_Idea_x0020_Descripti"] = description.Text;
                    

                     newItem["scale"] = "8";

                     SaveRadios(newItem);
                     //<------Scope Tab------>


                     MyItem item = aimcombo.SelectedItem as MyItem;
                     if (item != null)
                         newItem["AIM_x0020_Application_x0020_Name"] = item.AIM_NAME;

                     //newItem["AIM_x0020_Application_x0020_Name"] = aimcombo.SelectionBoxItem;
                     newItem["AIM_x0020_Application_x0020_ID"] = AIM_ID.Text;
                     newItem["_x0031_st_x0020_Mo_x0020_Saves_x"] = firstmonth.SelectedDate;


                     if (identify_e.IsChecked == true)
                     {
                         newItem["EXCEL_x0020_Identifier"] = "1. E-Excessive Demand";
                     }
                     else if (identify_x.IsChecked == true)
                     {
                         newItem["EXCEL_x0020_Identifier"] = "2. X-eXpense Reduction";
                     }
                     else if (identify_c.IsChecked == true)
                     {
                         newItem["EXCEL_x0020_Identifier"] = "3. C–Customization Reduction";
                     }
                     else if (identify_e2.IsChecked == true)
                     {
                         newItem["EXCEL_x0020_Identifier"] = "4. E–Effective Talent Utilization";
                     }
                     else if (identify_l.IsChecked == true)
                     {
                         newItem["EXCEL_x0020_Identifier"] = "5. L–Less Duplication";
                     }

                     //Assumptions, Risk, Business Capability, SDLC
                     newItem["Assumptions_x0020_or_x0020_Depen"] = assump_depend.Text;

                     if (risk_high.IsChecked == true)
                     {
                         newItem["Risk_x0020_of_x0020_Implementati"] = "High";
                     }

                     else if (risk_med.IsChecked == true)
                     {
                         newItem["Risk_x0020_of_x0020_Implementati"] = "Medium";
                     }
                     else if (risk_low.IsChecked == true)
                     {
                         newItem["Risk_x0020_of_x0020_Implementati"] = "Low";
                     }

                     newItem["Business_x0020_Capability"] = biz_capability.Text;
                     newItem["SDLC_x0020_Project_x0020_ID"] = sdlc_projID.Text;
                     newItem["SDLC_x0020_Project_x0020_Name"] = sdlc_projName.Text;

                     //vendor save
                     if (vendorSave_yes.IsChecked == true)
                     {
                         newItem["Vendor_Save"] = "Yes";
                     }

                     else if (vendorSave_no.IsChecked == true)
                     {
                         newItem["Vendor_Save"] = "No";
                     }

                     //Cost Type
                     if (type_Avoid.IsChecked == true)
                     {
                         newItem["Cost_x0020_Type1"] = "Cost Avoidance";
                     }

                     else if (type_reEngineer.IsChecked == true)
                     {
                         newItem["Cost_x0020_Type1"] = "Re-engineering (REE)";
                         newItem["Tech_Impact"] = tech_impact.Text;
                     }

                     else if (type_Reduction.IsChecked == true)
                     {
                         newItem["Cost_x0020_Type1"] = "Cost Reduction";
                     }
                     else if (type_Growth.IsChecked == true)
                     {
                         newItem["Cost_x0020_Type1"] = "Growth Reduction";
                     }

                     //<-----estimated savings----->

                     newItem["SavingsHeader1"] = header1.Text;
                     newItem["SavingsHeader2"] = header2.Text;
                     newItem["SavingsHeader3"] = header3.Text;
                     newItem["SavingsHeader4"] = header4.Text;
                     newItem["SavingsHeader5"] = header5.Text;

                     newItem["Savings1"] = es1.Value;
                     newItem["Savings2"] = es2.Value;
                     newItem["Savings3"] = es3.Value;
                     newItem["Savings4"] = es4.Value;
                     newItem["Savings5"] = es5.Value;

                     newItem["Total_x0020_Savings"] = es_Total.Value;

                     //<-----comments, status & audit----->

                     newItem["Project_x0020_Comments"] = projcomText.Text;
                     newItem["Idea_x0020_Status"] = "Future Pipeline";
                     newItem["Audit"] = createdby.Text + " - " + DateTime.Now + " - " + "successfully submitted the idea as future pipeline.";




                     newItem.Update();
                     //Load the list 
                     context.Load(Idea, list => list.Title);
                     //Execute the query to create the new item 
                     context.ExecuteQueryAsync((s, ee) =>
                     {
                         string itemId = newItem.Id.ToString();

                         RenameFolder(Utils.GetSiteUrl(), libName, string.Empty, folderName, itemId);

                         Dispatcher.BeginInvoke(() =>
                         {
                             newFolderName = itemId;
                             msgwin = new Messages();
                             msgwin.RequiredOKButton.Visibility = Visibility.Collapsed;
                             msgwin.alert.Visibility = Visibility.Collapsed;

                             msgwin.msgtxt.Text = "Your idea was successfully submitted as future pipeline.";
                             msgwin.Show();


                         }
                             );

                     },
         (s, ee) =>
         {
             Console.WriteLine(ee.Message);

         });
                 }
             }
         }

         private void ShowPastDateError()
         {

             msgwin = new Messages();
             msgwin.RequiredOKButton.Visibility = Visibility.Visible;
             msgwin.msgtxt.Text = Consts.PAST_DATE_ERROR;
             msgwin.alert.Visibility = Visibility.Collapsed;
             msgwin.Show();

         }


        //<~~~~~~BEGIN IN PROGRESS~~~~~>
         private void btn_inprogress_Click(object sender, RoutedEventArgs e)
         {

             if (firstmonth.SelectedDate <= DateTime.Today.AddDays(-1))
             {
                 ShowPastDateError();
             }

             else 
             {

                 ResetControls();
                 ValidateResult draftResult = ValidateForDraft();
                 ValidateResult result = ValidateForInProgress(draftResult);

                 if (!result.IsValid)
                 {

                     GetErrorWindow(result).Show();
                     NavigateTab(result);
                 }

                 else
                 {
                     btn_draft.IsEnabled = false;
                     btn_approve.IsEnabled = false;
                     btn_fp.IsEnabled = false;
                     btn_inprogress.IsEnabled = false;

                     //Get the current context 
                     ClientContext context = ClientContext.Current;
                     //Get the Idea list and add a new item 
                     Idea = context.Web.Lists.GetByTitle("Idea");
                     ListItem newItem = Idea.AddItem(new ListItemCreationInformation());
                     //Set the new item's properties 

                     if (SinglePeopleChooser.selectedAccounts.Count > 0 || MultiplePeopleChooser.selectedAccounts.Count > 0)
                     {

                         if (SinglePeopleChooser.selectedAccounts.Count > 0)
                         {
                             Singleuser = context.Web.EnsureUser(SinglePeopleChooser.selectedAccounts[0].AccountName);
                             newItem["Executor"] = Singleuser;

                             Singleuser1 = context.Web.EnsureUser(SinglePeopleChooser1.selectedAccounts[0].AccountName);
                             newItem["Director"] = Singleuser1;

                             Singleuser2 = context.Web.EnsureUser(SinglePeopleChooser2.selectedAccounts[0].AccountName);
                             newItem["VP"] = Singleuser2;

                         }
                         if (MultiplePeopleChooser.selectedAccounts.Count > 0)
                         {
                             List<FieldUserValue> usersList = new List<FieldUserValue>();
                             foreach (AccountList ac in MultiplePeopleChooser.selectedAccounts)
                             {
                                 usersList.Add(FieldUserValue.FromUser(ac.AccountName));
                             }

                             newItem["FTE_x0020_Contributors"] = usersList;
                         }


                     }

                     //<-----Project Overview Tab ------>
                     newItem["Idea_x0020_Name"] = ideaname.Text;
                     newItem["EXCEL_x0020_Idea_x0020_Descripti"] = description.Text;

                     newItem["scale"] = "2";

                     SaveRadios(newItem);

                     //<------Scope Tab------>

                     MyItem item = aimcombo.SelectedItem as MyItem;
                     if (item != null)
                         newItem["AIM_x0020_Application_x0020_Name"] = item.AIM_NAME;

                     newItem["AIM_x0020_Application_x0020_ID"] = AIM_ID.Text;
                     newItem["_x0031_st_x0020_Mo_x0020_Saves_x"] = firstmonth.SelectedDate;


                     if (identify_e.IsChecked == true)
                     {
                         newItem["EXCEL_x0020_Identifier"] = "1. E-Excessive Demand";
                     }
                     else if (identify_x.IsChecked == true)
                     {
                         newItem["EXCEL_x0020_Identifier"] = "2. X-eXpense Reduction";
                     }
                     else if (identify_c.IsChecked == true)
                     {
                         newItem["EXCEL_x0020_Identifier"] = "3. C–Customization Reduction";
                     }
                     else if (identify_e2.IsChecked == true)
                     {
                         newItem["EXCEL_x0020_Identifier"] = "4. E–Effective Talent Utilization";
                     }
                     else if (identify_l.IsChecked == true)
                     {
                         newItem["EXCEL_x0020_Identifier"] = "5. L–Less Duplication";
                     }

                     //Assumptions, Risk, Business Capability, SDLC
                     newItem["Assumptions_x0020_or_x0020_Depen"] = assump_depend.Text;

                     if (risk_high.IsChecked == true)
                     {
                         newItem["Risk_x0020_of_x0020_Implementati"] = "High";
                     }

                     else if (risk_med.IsChecked == true)
                     {
                         newItem["Risk_x0020_of_x0020_Implementati"] = "Medium";
                     }
                     else if (risk_low.IsChecked == true)
                     {
                         newItem["Risk_x0020_of_x0020_Implementati"] = "Low";
                     }

                     newItem["Business_x0020_Capability"] = biz_capability.Text;
                     newItem["SDLC_x0020_Project_x0020_ID"] = sdlc_projID.Text;
                     newItem["SDLC_x0020_Project_x0020_Name"] = sdlc_projName.Text;

                     //vendor save
                     if (vendorSave_yes.IsChecked == true)
                     {
                         newItem["Vendor_Save"] = "Yes";
                     }

                     else if (vendorSave_no.IsChecked == true)
                     {
                         newItem["Vendor_Save"] = "No";
                     }

                     //Cost Type
                     if (type_Avoid.IsChecked == true)
                     {
                         newItem["Cost_x0020_Type1"] = "Cost Avoidance";
                     }

                     else if (type_reEngineer.IsChecked == true)
                     {
                         newItem["Cost_x0020_Type1"] = "Re-engineering (REE)";
                         newItem["Tech_Impact"] = tech_impact.Text;
                     }

                     else if (type_Reduction.IsChecked == true)
                     {
                         newItem["Cost_x0020_Type1"] = "Cost Reduction";
                     }
                     else if (type_Growth.IsChecked == true)
                     {
                         newItem["Cost_x0020_Type1"] = "Growth Reduction";
                     }

                     //<-----estimated savings----->

                     newItem["SavingsHeader1"] = header1.Text;
                     newItem["SavingsHeader2"] = header2.Text;
                     newItem["SavingsHeader3"] = header3.Text;
                     newItem["SavingsHeader4"] = header4.Text;
                     newItem["SavingsHeader5"] = header5.Text;

                     newItem["Savings1"] = es1.Value;
                     newItem["Savings2"] = es2.Value;
                     newItem["Savings3"] = es3.Value;
                     newItem["Savings4"] = es4.Value;
                     newItem["Savings5"] = es5.Value;

                     newItem["Total_x0020_Savings"] = es_Total.Value;

                     //<-----comments, status & audit----->

                     newItem["Project_x0020_Comments"] = projcomText.Text;
                     newItem["Idea_x0020_Status"] = "In Progress";
                     newItem["Audit"] = createdby.Text + " - " + DateTime.Now + " - " + "successfully submitted the idea in progress.";




                     newItem.Update();
                     //Load the list 
                     context.Load(Idea, list => list.Title);
                     //Execute the query to create the new item 
                     context.ExecuteQueryAsync((s, ee) =>
                   {

                       string itemId = newItem.Id.ToString();

                       RenameFolder(Utils.GetSiteUrl(), libName, string.Empty, folderName, itemId);

                       Dispatcher.BeginInvoke(() =>
                       {
                           newFolderName = itemId;
                           msgwin = new Messages();
                           msgwin.RequiredOKButton.Visibility = Visibility.Collapsed;
                           msgwin.alert.Visibility = Visibility.Collapsed;

                           msgwin.msgtxt.Text = "Your idea was successfully submitted in progress";
                           msgwin.Show();


                       }
                           );


                   },
       (s, ee) =>
       {
           Console.WriteLine(ee.Message);

       });


                 }
             }
         }

        

        //<~~~~~~BEGIN SUBMIT FOR APPROVAL~~~~~>
         private void btn_approval_Click(object sender, RoutedEventArgs e)
         {
             ResetControls();
             ValidateResult draftResult = ValidateForDraft();
             ValidateResult progressResult = ValidateForInProgress(draftResult);
             ValidateResult result = ValidateForApproval(progressResult);
             if (!result.IsValid)
             {

                 GetErrorWindow(result).Show();
                 NavigateTab(result);
             }
             else
             {
                 ftewin = new FTEMsgBox();
                 ftewin.Show();
                 ftewin.NoClicked += new EventHandler(NoClicked);
                 ftewin.YesClicked += new EventHandler(YesClicked);
             }
         }
            
                
        void NoClicked(object sender, EventArgs e)
        {
            
            //nav to FTE Contributors tab
            this.tabcontrol1.SelectedIndex = 1;
            btnstack_overview.Visibility = Visibility.Collapsed;
            overview_image.Visibility = Visibility.Collapsed;
            scope_image.Visibility = Visibility.Visible;
            btnstack_scope.Visibility = Visibility.Visible;
            btnstack_scopeBack.Visibility= Visibility.Visible;
         
        }

        void YesClicked(object sender, EventArgs e)
        {
          
                btn_draft.IsEnabled = false;
                btn_approve.IsEnabled = false;
                btn_fp.IsEnabled = false;
                btn_inprogress.IsEnabled = false;   
                //Get the current context 
                ClientContext context = ClientContext.Current;
                //Get the Idea list and add a new item 
                Idea = context.Web.Lists.GetByTitle("Idea");
                ListItem newItem = Idea.AddItem(new ListItemCreationInformation());
                //Set the new item's properties 

                if (SinglePeopleChooser.selectedAccounts.Count > 0 || MultiplePeopleChooser.selectedAccounts.Count > 0)
                {

                    if (SinglePeopleChooser.selectedAccounts.Count > 0)
                    {
                        Singleuser = context.Web.EnsureUser(SinglePeopleChooser.selectedAccounts[0].AccountName);
                        newItem["Executor"] = Singleuser;

                        Singleuser1 = context.Web.EnsureUser(SinglePeopleChooser1.selectedAccounts[0].AccountName);
                        newItem["Director"] = Singleuser1;

                        Singleuser2 = context.Web.EnsureUser(SinglePeopleChooser2.selectedAccounts[0].AccountName);
                        newItem["VP"] = Singleuser2;

                    }
                    if (MultiplePeopleChooser.selectedAccounts.Count > 0)
                    {
                        List<FieldUserValue> usersList = new List<FieldUserValue>();
                        foreach (AccountList ac in MultiplePeopleChooser.selectedAccounts)
                        {
                            usersList.Add(FieldUserValue.FromUser(ac.AccountName));
                        }

                        newItem["FTE_x0020_Contributors"] = usersList;
                    }


                }

                //<-----Project Overview Tab ------>
                newItem["Idea_x0020_Name"] = ideaname.Text;
                newItem["EXCEL_x0020_Idea_x0020_Descripti"] = description.Text;


                newItem["scale"] = "3";

                SaveRadios(newItem);

                //<------Scope Tab------>

                MyItem item = aimcombo.SelectedItem as MyItem;
                if (item != null)
                    newItem["AIM_x0020_Application_x0020_Name"] = item.AIM_NAME;

                //newItem["AIM_x0020_Application_x0020_Name"] = aimcombo.SelectionBoxItem;
                newItem["AIM_x0020_Application_x0020_ID"] = AIM_ID.Text;
                newItem["_x0031_st_x0020_Mo_x0020_Saves_x"] = firstmonth.SelectedDate;


                if (identify_e.IsChecked == true)
                {
                    newItem["EXCEL_x0020_Identifier"] = "1. E-Excessive Demand";
                }
                else if (identify_x.IsChecked == true)
                {
                    newItem["EXCEL_x0020_Identifier"] = "2. X-eXpense Reduction";
                }
                else if (identify_c.IsChecked == true)
                {
                    newItem["EXCEL_x0020_Identifier"] = "3. C–Customization Reduction";
                }
                else if (identify_e2.IsChecked == true)
                {
                    newItem["EXCEL_x0020_Identifier"] = "4. E–Effective Talent Utilization";
                }
                else if (identify_l.IsChecked == true)
                {
                    newItem["EXCEL_x0020_Identifier"] = "5. L–Less Duplication";
                }

                //Assumptions, Risk, Business Capability, SDLC
                newItem["Assumptions_x0020_or_x0020_Depen"] = assump_depend.Text;

                if (risk_high.IsChecked == true)
                {
                    newItem["Risk_x0020_of_x0020_Implementati"] = "High";
                }

                else if (risk_med.IsChecked == true)
                {
                    newItem["Risk_x0020_of_x0020_Implementati"] = "Medium";
                }
                else if (risk_low.IsChecked == true)
                {
                    newItem["Risk_x0020_of_x0020_Implementati"] = "Low";
                }

                newItem["Business_x0020_Capability"] = biz_capability.Text;
                newItem["SDLC_x0020_Project_x0020_ID"] = sdlc_projID.Text;
                newItem["SDLC_x0020_Project_x0020_Name"] = sdlc_projName.Text;

                //vendor save
                if (vendorSave_yes.IsChecked == true)
                {
                    newItem["Vendor_Save"] = "Yes";
                }

                else if (vendorSave_no.IsChecked == true)
                {
                    newItem["Vendor_Save"] = "No";
                }

                //Cost Type
                if (type_Avoid.IsChecked == true)
                {
                    newItem["Cost_x0020_Type1"] = "Cost Avoidance";
                }

                else if (type_reEngineer.IsChecked == true)
                {
                    newItem["Cost_x0020_Type1"] = "Re-engineering (REE)";
                    newItem["Tech_Impact"] = tech_impact.Text;
                }

                else if (type_Reduction.IsChecked == true)
                {
                    newItem["Cost_x0020_Type1"] = "Cost Reduction";
                }
                else if (type_Growth.IsChecked == true)
                {
                    newItem["Cost_x0020_Type1"] = "Growth Reduction";
                }

                //<-----estimated savings----->

                newItem["SavingsHeader1"] = header1.Text;
                newItem["SavingsHeader2"] = header2.Text;
                newItem["SavingsHeader3"] = header3.Text;
                newItem["SavingsHeader4"] = header4.Text;
                newItem["SavingsHeader5"] = header5.Text;

                newItem["Savings1"] = es1.Value;
                newItem["Savings2"] = es2.Value;
                newItem["Savings3"] = es3.Value;
                newItem["Savings4"] = es4.Value;
                newItem["Savings5"] = es5.Value;

                newItem["Total_x0020_Savings"] = es_Total.Value;

                //<-----comments, status & audit----->

                newItem["Project_x0020_Comments"] = projcomText.Text;
                newItem["Idea_x0020_Status"] = "Submit for Approval";
                newItem["Audit"] = createdby.Text + " - " + DateTime.Now + " - " + "successfully submitted the idea for approval.";



                newItem.Update();
                //Load the list 
                context.Load(Idea, list => list.Title);
                //Execute the query to create the new item 
                context.ExecuteQueryAsync((s, ee) =>
                {
                    string itemId = newItem.Id.ToString();

                    RenameFolder(Utils.GetSiteUrl(), libName, string.Empty, folderName, itemId);

                    Dispatcher.BeginInvoke(() =>
                    {
                        newFolderName = itemId;
                        msgwin = new Messages();
                        msgwin.msgtxt.Text = "Your idea was successfully submitted for approval.";
                        msgwin.RequiredOKButton.Visibility = Visibility.Collapsed;
                        msgwin.alert.Visibility = Visibility.Collapsed;

                        msgwin.Show();
                    }
                        );


                },
  (s, ee) =>
  {
      Console.WriteLine(ee.Message);

  });


            
        }
        



        #endregion


        #region A I M  D R O P D O W N

        private void LoadComboItems()
        {
            using (ClientContext context = new ClientContext(Utils.GetSiteUrl()))
            {
                Web web = context.Web;
                context.Load(web);
                List list = context.Web.Lists.GetByTitle("AIM");
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View><Query><FieldRef Name='Title' /><FieldRef Name='AIM_x0020_Application_x0020_ID' /><OrderBy> <FieldRef Name='Title'/></OrderBy></Query></View>";
                ListItemCollection listItems = list.GetItems(camlQuery);
                // context.Load(list);
                context.Load(listItems);
                //  context.ExecuteQueryAsync(webSucceededCallback, OnSiteLoadFailure);

                context.ExecuteQueryAsync((s, ee) =>
                {


                    foreach (ListItem listitem in listItems)
                    {
                        items.Add(new MyItem { AIM_ID = listitem.FieldValues["AIM_x0020_Application_x0020_ID"].ToString(), AIM_NAME = listitem.FieldValues["Title"].ToString() });

                    }

                    Dispatcher.BeginInvoke(() =>
                    {
                        aimcombo.DisplayMemberPath = "AIM_NAME";
                        aimcombo.SelectedValuePath = "AIM_ID";
                        aimcombo.SelectedValue = "{Binding AIM_ID}";
                        aimcombo.ItemsSource = items;
                        aimcombo.DataContext = items;

                        foreach (MyItem item in items)
                        {
                            if (item.AIM_NAME.Equals(AimName))
                            {
                                if (aimcombo.Items.Contains(item))
                                    aimcombo.SelectedValue = item.AIM_ID;
                            }
                        }
                    }

                        );




                },



    (s, ee) =>
    {
        Console.WriteLine(ee.Message);

    });
            }


        }

        private void aimcombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            MyItem item = aimcombo.SelectedItem as MyItem;
            AIM_ID.Text = item.AIM_ID;

        }



        #endregion


        #region P E O P L E P I C K E R

        private void LoadUser(ClientContext ctx, FieldUserValue singleValue, FieldUserValue[] multValue)
        {
            List userList = ctx.Web.SiteUserInfoList;
            ctx.Load(userList);

            ListItemCollection users = userList.GetItems(CamlQuery.CreateAllItemsQuery());

            ctx.Load(users, items => items.Include(
                item => item.Id, item => item["Name"]));



            ctx.ExecuteQueryAsync((ss, eee) =>
            {
                ListItem principal = users.GetById(singleValue.LookupId);

                ctx.Load(principal);



                ctx.ExecuteQueryAsync((sss, eeee) =>
                {
                    string username = principal["Name"] as string;

                    string decodedName = Utils.checkClaimsUser(username);
                    string dispName = principal["Title"] as string;

                    Dispatcher.BeginInvoke(() =>
                    {
                        SinglePeopleChooser.selectedAccounts.Clear();

                        SinglePeopleChooser.selectedAccounts.Add(new AccountList(decodedName, dispName));
                        SinglePeopleChooser.UserTextBox.Text = dispName;


                    }
    );

                },
                  (sss, eeee) =>
                  {
                      Console.WriteLine(eeee.Message);

                  });


            },
             (sss, eeee) =>
             {
                 Console.WriteLine(eeee.Message);

             });



            userList = ctx.Web.SiteUserInfoList;
            ctx.Load(userList);

            users = userList.GetItems(CamlQuery.CreateAllItemsQuery());

            ctx.Load(users, items => items.Include(
                item => item.Id, item => item["Name"]));


            ctx.ExecuteQueryAsync((s, ee) =>
            {
                ListItem[] principals = new ListItem[multValue.Length];

                for (int i = 0; i < multValue.Length; i++)
                {
                    principals[i] = users.GetById(multValue[i].LookupId);
                    ctx.Load(principals[i]);
                }

                ctx.ExecuteQueryAsync((ssss, eeeee) =>
                {
                    string username;

                    for (int i = 0; i < multValue.Length; i++)
                    {


                        try
                        {
                            username = principals[i]["Name"] as string;
                        }
                        catch (IndexOutOfRangeException ii)
                        {
                            return;
                        }

                        string decodedName = Utils.checkClaimsUser(username);
                        string dispName = principals[i]["Title"] as string;

                        Dispatcher.BeginInvoke(() =>
                        {


                            MultiplePeopleChooser.selectedAccounts.Add(new AccountList(decodedName, dispName));


                        }
                        );
                    }


                },
               (ssss, eeeee) =>
               {
                   Console.WriteLine(eeeee.Message);

               });





            },

             (ssss, eeeee) =>
             {
                 Console.WriteLine(eeeee.Message);

             });
        }






        #endregion*/


        #region E S T I M A T E D  S A V I N G S


        private void firstmonth_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            firstmonthText.ClearValue(TextBox.TextProperty);
            firstmonthText.Text = firstmonth.SelectedDate.ToString();
            firstmonthTxt.Foreground = new SolidColorBrush(Colors.Black);

                int mth;
                int yr;

                if (firstmonth.SelectedDate != null)
                {

                    mth = firstmonth.SelectedDate.Value.Month;
                    yr = firstmonth.SelectedDate.Value.Year;


                    if (mth <= 3)
                    {
                        header1.Text = "Q1" + "-" + yr;
                        header2.Text = "Q2" + "-" + yr;
                        header3.Text = "Q3" + "-" + yr;
                        header4.Text = "Q4" + "-" + yr;
                        header5.Text = "Q1" + "-" + (yr + 1);

                    }
                    else if (mth > 3 && mth <= 6)
                    {
                        header1.Text = "Q2" + "-" + yr;
                        header2.Text = "Q3" + "-" + yr;
                        header3.Text = "Q4" + "-" + yr;
                        header4.Text = "Q1" + "-" + (yr + 1);
                        header5.Text = "Q2" + "-" + (yr + 1);

                    }
                    else if (mth > 6 && mth <= 9)
                    {
                        header1.Text = "Q3" + "-" + yr;
                        header2.Text = "Q4" + "-" + yr;
                        header3.Text = "Q1" + "-" + (yr + 1);
                        header4.Text = "Q2" + "-" + (yr + 1);
                        header5.Text = "Q3" + "-" + (yr + 1);

                    }
                    else if (mth > 9 && mth <= 12)
                    {
                        header1.Text = "Q4" + "-" + yr;
                        header2.Text = "Q1" + "-" + (yr + 1);
                        header3.Text = "Q2" + "-" + (yr + 1);
                        header4.Text = "Q3" + "-" + (yr + 1);
                        header5.Text = "Q4" + "-" + (yr + 1);

                    }
                }

             
   
            }
 
        private void es_ValueChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            totalText.ClearValue(TextBox.TextProperty);

            totalText.Text = (Convert.ToInt32(es1.Value) + Convert.ToInt32(es2.Value) + Convert.ToInt32(es3.Value) + Convert.ToInt32(es4.Value) + Convert.ToInt32(es5.Value)).ToString();

            es_Total.Value = totalText.Text;

            if (Convert.ToInt32(totalText.Text) > 0)
            {
                savingsTxt.Foreground = new SolidColorBrush(Colors.Black);
            }


           
        }


        #endregion

        #region C O M M E N T S


        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            //make sure a comment was entered
            if (string.IsNullOrEmpty(pcomments.Text))
            {
                MessageBox.Show("You must enter a comment before adding to Comments History.", "Error", MessageBoxButton.OK);

                return;
            }
            else
            {

                ClientContext clientContext = ClientContext.Current;

                oWebsite = clientContext.Web;
                collList = oWebsite.Lists;

                clientContext.Load(oWebsite, s => s.CurrentUser);
                clientContext.ExecuteQueryAsync(pconQuerySucceeded, pconQueryFailed);
            }
        }

        private void pconQuerySucceeded(object sender, ClientRequestSucceededEventArgs args)
        {
            user = oWebsite.CurrentUser;
            UpdateUIMethod updateUI = pcDisplayInfo;
            this.Dispatcher.BeginInvoke(updateUI);

        }

        private void pconQueryFailed(object sender, ClientRequestFailedEventArgs args)
        {
            MessageBox.Show("Request failed");
        }


        private void pcDisplayInfo()
        {
            if (projcomText.Text.Length == 0)
            {

                projcomText.Text = (user.Title + " " + "(" + DateTime.Now + ")" + " - " + pcomments.Text).ToString();

                pcomments.Text = string.Empty;
            }
            else if(projcomText.Text.Length > 0)
            {
                projcomText.Text = projcomText.Text + Environment.NewLine + (user.Title + " " + "(" + DateTime.Now + ")" + " - " + pcomments.Text).ToString();
                pcomments.Text = string.Empty;
            }
        }




        private void PopUpButton_Click(object sender, RoutedEventArgs e)
        {
            myPopup_comments.IsOpen = false;
        }
        private void imghelp_comments_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            PopUpText_comments.Text = "Please provide any comments or questions for the EXCEL Admins. Similarly, EXCEL Admins can provide comments or questions for you. ";
            myPopup_comments.IsOpen = true;
        }
        #endregion

        #region A T T A C H M E N T S

        private void UploadFile(FileInfo fileToUpload, string libraryTitle, string folderName)
        {
            var web = myClContext.Web;
            List destinationList = web.Lists.GetByTitle(libraryTitle);

            var fciFileToUpload = new FileCreationInformation();

            Stream streamToUpload = fileToUpload.OpenRead();
            int length = (int)streamToUpload.Length;  // get file length

            fciFileToUpload.Content = new byte[length];

            int count = 0;                        // actual number of bytes read
            int sum = 0;                          // total number of bytes read

            while ((count = streamToUpload.Read(fciFileToUpload.Content, sum, length - sum)) > 0)
                sum += count;  // sum is a buffer offset for next reading
            streamToUpload.Close();

            fciFileToUpload.Url = fileToUpload.Name;

            Microsoft.SharePoint.Client.File clFileToUpload = null;
            if (string.IsNullOrEmpty(folderName))
            {
                clFileToUpload = destinationList.RootFolder.Files.Add(fciFileToUpload);

                myClContext.Load(clFileToUpload);

                myClContext.ExecuteQueryAsync((s, ee) =>
                {

                    Dispatcher.BeginInvoke(() =>
                    {
                        selectedFiles.Add(new FileEntry(fileToUpload.Name, fileToUpload.Name));
                        Remove.IsEnabled = true;
                        attachTxt.Foreground = new SolidColorBrush(Colors.Black);
                    }
                    );

                },
                (s, ee) =>
                {
                    Console.WriteLine(ee.Message);

                });

            }
            else
            {
                FolderCollection folderCol = destinationList.RootFolder.Folders;
                //myClContext.Load(folderCol, items => items.Include(fldr => fldr.Name.Equals(folderName, StringComparison.OrdinalIgnoreCase)));

                myClContext.Load(folderCol);
                busyIndicator.IsBusy = true;

                myClContext.ExecuteQueryAsync((s, ee) =>
                {

                    for (int i = 0; i < folderCol.Count; ++i)
                    {
                        if (folderCol[i].Name.Equals(folderName, StringComparison.OrdinalIgnoreCase))
                        {
                            clFileToUpload = folderCol[i].Files.Add(fciFileToUpload);

                            myClContext.Load(clFileToUpload);
                            break;
                        }

                    }

                    myClContext.ExecuteQueryAsync((ss, eee) =>
                    {

                        Dispatcher.BeginInvoke(() =>
                        {
                            selectedFiles.Add(new FileEntry(fileToUpload.Name, fileToUpload.Name));
                            Remove.IsEnabled = true;
                            attachTxt.Foreground = new SolidColorBrush(Colors.Black);
                            busyIndicator.IsBusy = false;

                        }
                            );



                    },
              (ss, eee) =>
              {
                  Dispatcher.BeginInvoke(() =>
                        {
                            MessageBox.Show(eee.Message);
                            busyIndicator.IsBusy = false;
                        });

              });



                },
              (s, ee) =>
              {
                  Dispatcher.BeginInvoke(() =>
                 {
                      MessageBox.Show(ee.Message);
                      busyIndicator.IsBusy = false;
                  });

              });
            }
        }

        private void ConnectToSP()
        {
            myClContext = ClientContext.Current;


        }

        public void CreateFolder(string siteUrl, string listName, string relativePath, string folderName)
        {
            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                Web web = clientContext.Web;
                List list = web.Lists.GetByTitle(listName);

                Folder rootFolder = list.RootFolder;

                clientContext.Load(rootFolder);



                ListItemCreationInformation newItem = new ListItemCreationInformation();
                newItem.UnderlyingObjectType = FileSystemObjectType.Folder;
                //newItem.FolderUrl = siteUrl + listName;
                if (!relativePath.Equals(string.Empty))
                {
                    newItem.FolderUrl += "/" + relativePath;
                }
                newItem.LeafName = folderName;

                ListItem item = list.AddItem(newItem);
                item["Title"] = folderName;
                item.Update();

                clientContext.Load(list);

                clientContext.ExecuteQueryAsync((s, ee) =>
                {

                    Folder newFolder = rootFolder.Folders.Add(folderName);


                    Dispatcher.BeginInvoke(() =>
                    {


                        // MessageBox.Show("Created", "Created", MessageBoxButton.OK);
                    });

                },
          (s, ee) =>
          {
              Console.WriteLine(ee.Message);

          });
            }
        }

        public void RenameFolder(string siteUrl, string listName, string relativePath, string folderName, string folderNewName)
        {
            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                Web web = clientContext.Web;
                List list = web.Lists.GetByTitle(listName);

                //  string FolderFullPath = GetFullPath(listName, relativePath, folderName);

                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View Scope=\"RecursiveAll\"> " +
                                "<Query>" +
                                    "<Where>" +
                     "<And>" +
                                            "<Eq>" +
                                                "<FieldRef Name=\"FSObjType\" />" +
                                                "<Value Type=\"Integer\">1</Value>" +
                                             "</Eq>" +
                     "<Eq>" +
                       "<FieldRef Name=\"Title\"/>" +
                       "<Value Type=\"Text\">" + folderName + "</Value>" +
                     "</Eq>" +
                     "</And>" +
                                     "</Where>" +
                                "</Query>" +
                                "</View>";

                /* if (relativePath.Equals(string.Empty))
                 {
                     query.FolderServerRelativeUrl = "/lists/" + listName;
                 }
                 else
                 {
                     query.FolderServerRelativeUrl = "/lists/" + listName + "/" + relativePath;
                 }*/

                //query.FolderServerRelativeUrl = "/"+listName;

                var folders = list.GetItems(query);

                clientContext.Load(list);
                clientContext.Load(list.Fields);
                clientContext.Load(folders, fs => fs.Include(fi => fi["Title"],
                    fi => fi["DisplayName"],
                    fi => fi["FileLeafRef"]));
                // clientContext.ExecuteQuery();

                clientContext.ExecuteQueryAsync((s, ee) =>
                {

                    if (folders.Count == 1)
                    {

                        folders[0]["Title"] = folderNewName;
                        folders[0]["FileLeafRef"] = folderNewName;
                        folders[0].Update();
                        clientContext.ExecuteQueryAsync((ss, eee) =>
                        {

                            Dispatcher.BeginInvoke(() =>
                            {

                                //  MessageBox.Show("Success", "Success", MessageBoxButton.OK);
                            });




                        },
          (ss, eee) =>
          {
              Console.WriteLine(eee.Message);

          });

                    }
                },
          (s, ee) =>
          {
              Console.WriteLine(ee.Message);

          });


            }
        }


        public void SearchFolder(string siteUrl, string listName, string relativePath)
        {
            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                Web web = clientContext.Web;
                List list = web.Lists.GetByTitle(listName);

                string FolderFullPath = null;

                CamlQuery query = CamlQuery.CreateAllFoldersQuery();

                if (relativePath.Equals(string.Empty))
                {
                    FolderFullPath = "/lists/" + listName;
                }
                else
                {
                    FolderFullPath = "/lists/" + listName + "/" + relativePath;
                }
                if (!string.IsNullOrEmpty(FolderFullPath))
                {
                    query.FolderServerRelativeUrl = FolderFullPath;
                }
                IList<Folder> folderResult = new List<Folder>();

                var listItems = list.GetItems(query);

                clientContext.Load(list);
                clientContext.Load(listItems, litems => litems.Include(
                    li => li["DisplayName"],
                    li => li["Id"]
                    ));

                clientContext.ExecuteQuery();

                foreach (var item in listItems)
                {

                    Console.WriteLine("{0}----------{1}", item.Id, item.DisplayName);
                }
            }
        }


        public void DeleteFile(string siteUrl, string listName, string relativePath, string folderName, FileEntry fileName)
        {
            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                Web web = clientContext.Web;
                List list = web.Lists.GetByTitle(listName);

                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View>"
                + "<Query>"
                + "<Where><Eq><FieldRef Name='FileLeafRef'/><Value Type='File'>" + fileName.FileName + "</Value></Eq></Where>"
                + "</Query>"
                + "</View>";

                if (!string.IsNullOrEmpty(folderName))
                {
                    query.FolderServerRelativeUrl = new Uri(siteUrl).AbsolutePath +"/" + libName + "/" + folderName + "/";
                }

                ListItemCollection listItems = list.GetItems(query);
                clientContext.Load(listItems);

                clientContext.ExecuteQueryAsync((s, ee) =>
                {

                    foreach (ListItem listitem in listItems)
                    {


                        listitem.DeleteObject();


                        Dispatcher.BeginInvoke(() =>
                        {

                            selectedFiles.Remove(fileName);
                            if (selectedFiles.Count == 0)
                                Remove.IsEnabled = false;
                            btn_approve.IsEnabled = false;

                        });


                        clientContext.ExecuteQueryAsync((ss, eee) =>
                        {

                        },
                        (ss, eee) =>
                        {
                            Console.WriteLine(eee.Message);

                        });


                    }

                },
         (s, ee) =>
         {
             Console.WriteLine(ee.Message);

         });



            }

        }

        public void DeleteFolder(string siteUrl, string listName, string relativePath, string folderName)
        {
            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                Web web = clientContext.Web;
                List list = web.Lists.GetByTitle(listName);

                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View Scope=\"RecursiveAll\"> " +
                               "<Query>" +
                                   "<Where>" +
                     "<And>" +
                                           "<Eq>" +
                                               "<FieldRef Name=\"FSObjType\" />" +
                                               "<Value Type=\"Integer\">1</Value>" +
                                            "</Eq>" +
                     "<Eq>" +
                       "<FieldRef Name=\"Title\"/>" +
                       "<Value Type=\"Text\">" + folderName + "</Value>" +
                     "</Eq>" +
                     "</And>" +
                                    "</Where>" +
                               "</Query>" +
                               "</View>";

                /*if (relativePath.Equals(string.Empty))
                {
                    query.FolderServerRelativeUrl = "/lists/" + listName;
                }
                else
                {
                    query.FolderServerRelativeUrl = "/lists/" + listName + "/" + relativePath;
                }*/

                var folders = list.GetItems(query);

                clientContext.Load(list);
                clientContext.Load(folders);

                clientContext.ExecuteQueryAsync((s, ee) =>
                {

                    if (folders.Count == 1)
                    {
                        folders[0].DeleteObject();
                        clientContext.ExecuteQueryAsync((ss, eee) =>
                        {

                            Dispatcher.BeginInvoke(() =>
                            {

                                //  MessageBox.Show("Deleted" + folderName, "Deleted", MessageBoxButton.OK);
                                selectedFiles.Clear();


                                Remove.IsEnabled = false;

                            });

                        },
         (ss, eee) =>
         {
             Console.WriteLine(eee.Message);

         });
                    }


                    Dispatcher.BeginInvoke(() =>
                    {



                    });

                },
         (s, ee) =>
         {
             Console.WriteLine(ee.Message);

         });



            }
        }

        private string GetFolderName()
        {
            if (string.IsNullOrEmpty(folderName))
            {
                folderName = DateTime.Now.ToString("TyyyyMMddHHmmssfff");
                return folderName;
            }
            else if (!string.IsNullOrEmpty(newFolderName))
                return newFolderName;
            else if (!string.IsNullOrEmpty(folderName))
                return folderName;

            return string.Empty;
        }

        private void FileUpload_Click(object sender, RoutedEventArgs e)
        {

            string folderName = GetFolderName();
            if (!string.IsNullOrEmpty(folderName))
            {
                CreateFolder(Utils.GetSiteUrl(), libName, string.Empty, folderName);
            }
            //this.txtProgress.Text = string.Empty;

            OpenFileDialog oFileDialog = new OpenFileDialog();
            oFileDialog.Filter = "All Files|*.*";
            oFileDialog.FilterIndex = 1;
            oFileDialog.Multiselect = true;

            string data = string.Empty;

            if (oFileDialog.ShowDialog() == true && !string.IsNullOrEmpty(folderName))
            {

                foreach (FileInfo file in oFileDialog.Files)
                {
                    if (!CheckFileLimit(file))
                    {
                        MessageBox.Show(Consts.FILE_SIZE_ERROR);
                        continue;
                    }

                    UploadFile(file, libName, GetFolderName());

                }
            }
        }

        private bool CheckFileLimit(FileInfo file)
        {
            try
            {
                if (file.Length < FILE_SIZE_LIMIT)
                    return true;
                return false;
            }
            catch (IOException)
            {
                return true;
            }

        }


        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            string folderName = GetFolderName();
            if (!string.IsNullOrEmpty(folderName))
            {
                CreateFolder(Utils.GetSiteUrl(), libName, string.Empty, folderName);
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // User Singleuser;

            ClientContext context = ClientContext.Current;
            List MadhurList = context.Web.Lists.GetByTitle("Idea");
            ListItem newItem = MadhurList.AddItem(new ListItemCreationInformation());

            //Singleuser = context.Web.EnsureUser("ads\\mahuj4");
            newItem["Idea_x0020_Status"] = "Draft";
            newItem.Update();
            context.Load(MadhurList, list => list.Title);

            context.ExecuteQueryAsync((s, ee) =>
            {
                string itemId = newItem.Id.ToString();

                RenameFolder(Utils.GetSiteUrl(), libName, string.Empty, folderName, itemId);

                Dispatcher.BeginInvoke(() =>
                {
                    newFolderName = itemId;
                    MessageBox.Show("Item created and folder renamed", "Item created and folder renamed", MessageBoxButton.OK);
                }
                    );


            },
     (s, ee) =>
     {
         Console.WriteLine(ee.Message);

     });


        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFolder(Utils.GetSiteUrl(), libName, string.Empty, GetFolderName());
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {

            FileEntry selFile = FileListBox.SelectedItem as FileEntry;
            DeleteFile(Utils.GetSiteUrl(), libName, string.Empty, GetFolderName(), selFile);
            

        }


        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            FileEntry selFile = FileListBox.SelectedItem as FileEntry;
            if (selFile != null)
            {
                Uri fileUrl = new Uri(Utils.GetSiteUrl() + "/" + libName + "/" + GetFolderName() + "/" + selFile.FileName);


                HtmlPage.PopupWindow(fileUrl, "_blank", null);
            }

        }


        void UserControl_Unloaded_1(object sender, EventArgs e)
        {
            // cleanup if there is a temp folder
            if (!string.IsNullOrEmpty(folderName))
            {
                DeleteFolder(Utils.GetSiteUrl(), libName, string.Empty, folderName);
            }

            Uri redirect = new Uri(Utils.GetSiteUrl()+"/SitePages/manage.aspx");
            System.Windows.Browser.HtmlPage.Window.Navigate(redirect, "_parent");

        }

        #endregion
    }
}


      