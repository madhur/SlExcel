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
using System.Windows.Navigation;
using Telerik.Windows.Controls;
using System.Threading;
using Microsoft.SharePoint.Client;
using System.Windows.Controls.Primitives;
using manage.Controls;
using manage.PeopleWS;
using System.ComponentModel;
using manage;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Resources;
using System.Windows.Browser;
using System.Threading.Tasks;
using System.Text;
using Common;



namespace manage.Controls
{
    public partial class EditForm
    {
        User user;
        private List Idea;
        private const string libName = "Idea Attachments";
        string folderName, newFolderName;
        private ClientContext myClContext;
        // This collection will contain all files shown to the user
        public SelectedFiles selectedFiles;
        // This collection will contain all files including the temp ones.
        // All files in this collection will be deleted, whenever user clicks cancel. This helps 
        // even delete the orphan objects
        public SelectedFiles allFiles; string mainID;
        string itemId;
        List<MyItem> items = new List<MyItem>();
        ILoadable mainPage;
        String status, NewStatus;
        bool formLoad;
        bool isContractor, isEmployee, isReadOnly, isAdmin, isIdeaOwner;
        String createdBy;
        String varaudit;
        private const Int32 FILE_SIZE_LIMIT = 3145728;



        public EditForm(string id, ILoadable mainPage)
        {

            InitializeComponent();

            this.mainPage = mainPage;
            selectedFiles = new SelectedFiles();
            allFiles = new SelectedFiles();
            ideaID.Text = id;
            mainID = id;
            folderName = id;
            newFolderName = id;
            itemId = id;

            ConnectToSP();
            FileListBox.DataContext = selectedFiles;
            FileListBox.ItemsSource = selectedFiles;


            LoadItems1();

            SinglePeopleChooser.UserTextBox.TextChanged+=UserTextBox_TextChanged;
            SinglePeopleChooser1.UserTextBox.TextChanged += UserTextBox_TextChanged;
            SinglePeopleChooser2.UserTextBox.TextChanged += UserTextBox_TextChanged;
            

        }

        public EditForm(string id)
        {

            InitializeComponent();

            selectedFiles = new SelectedFiles();
            allFiles = new SelectedFiles();
            ideaID.Text = id;
            mainID = id;
            folderName = id;
            newFolderName = id;
            itemId = id;

            ConnectToSP();
            FileListBox.DataContext = selectedFiles;
            FileListBox.ItemsSource = selectedFiles;


            LoadItems1();

            SinglePeopleChooser.UserTextBox.TextChanged += UserTextBox_TextChanged;
            SinglePeopleChooser1.UserTextBox.TextChanged += UserTextBox_TextChanged;
            SinglePeopleChooser2.UserTextBox.TextChanged += UserTextBox_TextChanged;


        }

      

        #region ////// F O R M    L O A D ////////
      private void LoadItems1()
        {
            tabcontrol1.IsEnabled = false;
            btnAdmin_comments.Visibility = Visibility.Collapsed;
         
            formLoad = true;

            using (ClientContext context = new ClientContext(Utils.GetSiteUrl()))
            {
                Web web = context.Web;
                context.Load(web, s => s.CurrentUser);
                List list = context.Web.Lists.GetByTitle("Idea");
                context.Load(list);
                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View><Query><Where><Eq><FieldRef Name = 'ID'/><Value Type='Number'>" + ideaID.Text + "</Value></Eq></Where></Query></View>";

                ListItemCollection listitems = list.GetItems(query);
                context.Load(listitems);

                busyIndicator.IsBusy = true;

                context.ExecuteQueryAsync((ss, eee) =>
                {
                    user = web.CurrentUser;
                    Dispatcher.BeginInvoke(() =>
                    {
                        currUser.Text = user.Title;
                        context.Load(listitems[0]);



                        context.ExecuteQueryAsync((sss, eeee) =>
                        {

                            Dispatcher.BeginInvoke(() =>
                            {
                                statusLevel.Text = getItem("Idea_x0020_Status", listitems[0]);
                               

                                FieldUserValue val = listitems[0].FieldValues["Executor"] as FieldUserValue;
                                FieldUserValue[] values = new FieldUserValue[3];

                                if (val != null)
                                {
                                    values[0] = val;

                                    SinglePeopleChooser.UserTextBox.FontStyle = FontStyles.Italic;


                                }

                                FieldUserValue val3 = listitems[0].FieldValues["Author"] as FieldUserValue;
                                if (val3 != null)
                                {
                                    createdby.Text = val3.LookupValue;
                                    createdby.FontStyle = FontStyles.Italic;

                                    GetLoginName(context, val3);
                                }



                                FieldUserValue val1 = listitems[0].FieldValues["Director"] as FieldUserValue;
                                if (val1 != null)
                                {
                                    values[1] = val1;
                                    SinglePeopleChooser1.UserTextBox.FontStyle = FontStyles.Italic;
                                }



                                FieldUserValue val2 = listitems[0].FieldValues["VP"] as FieldUserValue;
                                if (val2 != null)
                                {
                                    values[2] = val2;
                                    SinglePeopleChooser2.UserTextBox.FontStyle = FontStyles.Italic;
                                }

                                if (val1 != null || val2 != null || val != null)
                                    LoadSingleUser(context, values);



                                SelectedAccounts sels = new SelectedAccounts();
                                FieldUserValue[] vals = listitems[0].FieldValues["FTE_x0020_Contributors"] as FieldUserValue[];
                                if (vals != null)
                                {
                                    LoadUser(context, vals);
                                }


                            }
                    );

                        },




        (sss, eeee) =>
        {
            Console.WriteLine(eeee.Message);

        });
                    }
                    );
                },
                       (ss, eee) =>
                       {
                           Console.WriteLine(eee.Message);

                       });
            }

           
        }

      private void LoadRoles()
      {

          ClientContext client = ClientContext.Current;
          GroupCollection groupCollection = client.Web.SiteGroups;

          Microsoft.SharePoint.Client.Group contractorGroup = groupCollection.GetById(38);
          Microsoft.SharePoint.Client.Group financeGroup = groupCollection.GetById(39);
          Microsoft.SharePoint.Client.Group adminGroup = groupCollection.GetById(40);
          Microsoft.SharePoint.Client.Group ownersGroup = groupCollection.GetById(6);

          User currentUser = client.Web.CurrentUser;
          UserCollection contactUsers = contractorGroup.Users;
          UserCollection financeUsers = financeGroup.Users;
          UserCollection adminUsers = adminGroup.Users;
          UserCollection ownerUsers = ownersGroup.Users;

          client.Load(currentUser);
          client.Load(contactUsers);
          client.Load(financeUsers);
          client.Load(adminUsers);
          client.Load(ownerUsers);

          client.ExecuteQueryAsync((s, ee) =>
          {

              foreach (User groupUser in financeUsers)
              {
                  if (groupUser.LoginName.Equals(currentUser.LoginName, StringComparison.OrdinalIgnoreCase))
                  {
                      isAdmin = true;
                      break;

                  }
              }

              if (!isAdmin)
              {
                  foreach (User groupUser in adminUsers)
                  {
                      if (groupUser.LoginName.Equals(currentUser.LoginName, StringComparison.OrdinalIgnoreCase))
                      {
                          isAdmin = true;
                          break;

                      }
                  }
              }

              if (!isAdmin)
              {
                  foreach (User groupUser in ownerUsers)
                  {
                      if (groupUser.LoginName.Equals(currentUser.LoginName, StringComparison.OrdinalIgnoreCase))
                      {
                          isAdmin = true;
                          break;

                      }
                  }
              }

              if (!isAdmin)
              {
                  foreach (User groupUser in contactUsers)
                  {
                      if (groupUser.LoginName.Equals(currentUser.LoginName, StringComparison.OrdinalIgnoreCase))
                      {

                          isContractor = true;
                          break;

                      }
                  }
              }

              if (!(isAdmin || isContractor))
                  isEmployee = true;

              String parsedLogin = Utils.checkClaimsUser(currentUser.LoginName);

              Dispatcher.BeginInvoke(() =>
              {

                  try
                  {
                      if (isContractor && (parsedLogin.Equals(SinglePeopleChooser.selectedAccounts[0].AccountName) || parsedLogin.Equals(createdBy, StringComparison.OrdinalIgnoreCase)))
                      {
                          isIdeaOwner = true;
                      }
                      else if (isContractor)
                      {
                          ShowPastDateError("You are not authorized to view this idea.");
                          this.Close();

                          return;
                      }
                      else if (isEmployee && (parsedLogin.Equals(SinglePeopleChooser.selectedAccounts[0].AccountName) || parsedLogin.Equals(createdBy, StringComparison.OrdinalIgnoreCase)))
                      {
                          isIdeaOwner = true;
                      }
                      else if (isEmployee)
                      {
                          MakeReadOnly(true);
                          isReadOnly = true;
                      }
                      else
                      {
                          //is admin, do nothing
                          btnAdmin_comments.Visibility = Visibility.Visible;

                      }

                      SilverlightOM();

                  }
                  catch (ArgumentOutOfRangeException e)
                  {
                      MessageBox.Show("Error loading roles");
                  }
              });


          },



 (s, ee) =>
 {
     Console.WriteLine(ee.Message);
 });
         
      }



      public void SilverlightOM()
      {
         
          tabcontrol1.IsEnabled = true;
          ClientContext context = ClientContext.Current;
          Web web = context.Web;
          context.Load(web, s => s.CurrentUser);
          List list = context.Web.Lists.GetByTitle("Idea");
          context.Load(list);
          CamlQuery query = new CamlQuery();
          query.ViewXml = "<View><Query><Where><Eq><FieldRef Name = 'ID'/><Value Type='Number'>" + ideaID.Text + "</Value></Eq></Where></Query></View>";

          ListItemCollection listitems = list.GetItems(query);
          context.Load(listitems);

          LoadFiles(Utils.GetSiteUrl(), "Idea Attachments", string.Empty, folderName);



          context.ExecuteQueryAsync((s, ee) =>
          {
              Dispatcher.BeginInvoke(() =>
              {
                  context.Load(listitems[0]);


                  context.ExecuteQueryAsync((ss, eee) =>
                  {

                      Dispatcher.BeginInvoke(() =>
                      {
                          status = getItem("Idea_x0020_Status", listitems[0]);
                        
                          if (statusLevel.Text == "Future Pipeline")
                          {
                             
                                  btn_save.Visibility = Visibility.Visible;
                                  btn_save1.Visibility = Visibility.Visible;
                                  btn_save2.Visibility = Visibility.Visible;
                                  btn_save3.Visibility = Visibility.Visible;
                                  btn_cancel.Visibility = Visibility.Visible;

                                  firstmonth.IsEnabled = false;
                                  //fp_check.IsChecked = true;

                                  revisedmonth.Visibility = Visibility.Visible;
                                  imghelp_revisedmonth.Visibility = Visibility.Visible;
                                  revisedTxt.Visibility = Visibility.Visible;
                                  ItemCollection ic = statusCombo.Items;
                                  statusCombo.SelectedItem = ic[2];
                              }

                          

                          else if (statusLevel.Text == "Draft")
                          {

                                  btn_save.Visibility = Visibility.Visible;
                                  btn_save1.Visibility = Visibility.Visible;
                                  btn_save2.Visibility = Visibility.Visible;
                                  btn_save3.Visibility = Visibility.Visible;
                                  btn_cancel.Visibility = Visibility.Visible;

                                  revisedmonth.Visibility = Visibility.Collapsed;
                                  imghelp_revisedmonth.Visibility = Visibility.Collapsed;
                                  revisedTxt.Visibility = Visibility.Collapsed;
                                  ItemCollection ic = statusCombo.Items;
                                  statusCombo.SelectedItem = ic.First();
                              

                          }

                          else if (statusLevel.Text == "In Progress")
                          {
                                  btn_save.Visibility = Visibility.Visible;
                                  btn_save1.Visibility = Visibility.Visible;
                                  btn_save2.Visibility = Visibility.Visible;
                                  btn_save3.Visibility = Visibility.Visible;
                                  btn_cancel.Visibility = Visibility.Visible;

                                  firstmonth.IsEnabled = false;

                                  ItemCollection ic = statusCombo.Items;
                                  statusCombo.SelectedItem = ic[1];

                          }
                          else if (statusLevel.Text == "Canceled")
                          {
                              btn_cancel.Visibility = Visibility.Collapsed;
                              btn_save.Visibility = Visibility.Collapsed;
                              btn_save1.Visibility = Visibility.Collapsed;
                              btn_save2.Visibility = Visibility.Collapsed;
                              btn_save3.Visibility = Visibility.Collapsed;
                              btn_inprogress.Visibility = Visibility.Collapsed;
                              btn_approve.Visibility = Visibility.Collapsed;
                              btn_draft.Visibility = Visibility.Collapsed;
                              btn_fp.Visibility = Visibility.Collapsed;
                              ItemCollection ic = statusCombo.Items;
                              statusCombo.SelectedItem = ic[5];

                              FormMsg.Text = "To reactivate this Idea, please contact your EXCEL Admin.";

                              if ((isEmployee || isContractor) && isIdeaOwner)
                              {
                                  MakeReadOnly(false);
                                  isReadOnly = true;
                              }
                              else if (isEmployee || isContractor)
                              {
                                  MakeReadOnly(true);
                                  isReadOnly = true;
                              }
                          }

                          else if (statusLevel.Text == "Approved")
                          {
                              savingsTxt.Text = "Savings - 12 Months Only";

                              btn_cancel.Visibility = Visibility.Collapsed;
                              btn_save.Visibility = Visibility.Collapsed;
                              btn_save1.Visibility = Visibility.Collapsed;
                              btn_save2.Visibility = Visibility.Collapsed;
                              btn_save3.Visibility = Visibility.Collapsed;

                              btn_inprogress.Visibility = Visibility.Collapsed;
                              btn_approve.Visibility = Visibility.Collapsed;
                              btn_draft.Visibility = Visibility.Collapsed;
                              btn_fp.Visibility = Visibility.Collapsed;
                              firstmonth.IsEnabled = false;
                              // Revised month to be unlocked

                              ItemCollection ic = statusCombo.Items;
                              statusCombo.SelectedItem = ic[4];

                              FormMsg.Text = "Idea is no longer Editable (except Project Comments).";

                              if ((isEmployee || isContractor) && isIdeaOwner)
                              {
                                  MakeReadOnly(false);
                                  isReadOnly = true;
                              }
                              else if (isEmployee || isContractor)
                              {
                                  MakeReadOnly(true);
                                  isReadOnly = true;
                              }
                          }

                          else if (statusLevel.Text == "Submit for Approval")
                          {
                              // Fast follower fix, cancel button visible
                              btn_cancel.Visibility = Visibility.Visible ;
                              
                              btn_inprogress.Visibility = Visibility.Collapsed;
                              btn_approve.Visibility = Visibility.Collapsed;
                              btn_draft.Visibility = Visibility.Collapsed;
                              btn_fp.Visibility = Visibility.Collapsed;
                              firstmonth.IsEnabled = false;

                              ItemCollection ic = statusCombo.Items;

                              statusCombo.SelectedItem = ic[3];

                              // Fast follower ... Do not make form read only at this status
                             // FormMsg.Text = "Idea is no longer Editable.";

                              /*if (isEmployee || isContractor)
                              {
                                 // MakeReadOnly(false);
                                  isReadOnly = true;
                              }*/

                              // Fast follower .. Save button becomes visible.
                              btn_save1.Visibility = Visibility.Visible;
                              btn_save2.Visibility = Visibility.Visible;
                              btn_save3.Visibility = Visibility.Visible;
                              btn_save.Visibility = Visibility.Visible;


                          }
                          else if (statusLevel.Text == "Ready for Finance Review")
                          {
                              btn_cancel.Visibility = Visibility.Collapsed;
                              btn_save.Visibility = Visibility.Collapsed;
                              btn_save1.Visibility = Visibility.Collapsed;
                              btn_save2.Visibility = Visibility.Collapsed;
                              btn_save3.Visibility = Visibility.Collapsed;
                              btn_inprogress.Visibility = Visibility.Collapsed;
                              btn_approve.Visibility = Visibility.Collapsed;
                              btn_draft.Visibility = Visibility.Collapsed;
                              btn_fp.Visibility = Visibility.Collapsed;
                              firstmonth.IsEnabled = false;

                              ItemCollection ic = statusCombo.Items;
                              statusCombo.SelectedItem = ic[6];

                              FormMsg.Text = "Idea is no longer Editable (except Project Comments).";

                              if ((isEmployee || isContractor) && isIdeaOwner)
                              {
                                  MakeReadOnly(false);
                                  isReadOnly = true;
                              }
                              else if (isEmployee || isContractor)
                              {
                                  MakeReadOnly(true);
                                  isReadOnly = true;
                              }

                          }
                          else if (statusLevel.Text == "Finance Review Completed")
                          {

                              btn_cancel.Visibility = Visibility.Collapsed;
                              btn_save.Visibility = Visibility.Collapsed;
                              btn_save1.Visibility = Visibility.Collapsed;
                              btn_save2.Visibility = Visibility.Collapsed;
                              btn_save3.Visibility = Visibility.Collapsed;
                              btn_inprogress.Visibility = Visibility.Collapsed;
                              btn_approve.Visibility = Visibility.Collapsed;
                              btn_draft.Visibility = Visibility.Collapsed;
                              btn_fp.Visibility = Visibility.Collapsed;
                              firstmonth.IsEnabled = false;

                              ItemCollection ic = statusCombo.Items;
                              statusCombo.SelectedItem = ic[7];

                              FormMsg.Text = "Idea is no longer Editable (except Project Comments).";


                              if ((isEmployee || isContractor) && isIdeaOwner)
                              {
                                  MakeReadOnly(false);
                                  isReadOnly = true;
                              }
                              else if (isEmployee || isContractor)
                              {
                                  MakeReadOnly(true);
                                  isReadOnly = true;
                              }

                          }
                          else if (statusLevel.Text == "Pending Actuals")
                          {

                              btn_cancel.Visibility = Visibility.Collapsed;
                              btn_save.Visibility = Visibility.Collapsed;
                              btn_save1.Visibility = Visibility.Collapsed;
                              btn_save2.Visibility = Visibility.Collapsed;
                              btn_save3.Visibility = Visibility.Collapsed;
                              btn_inprogress.Visibility = Visibility.Collapsed;
                              btn_approve.Visibility = Visibility.Collapsed;
                              btn_draft.Visibility = Visibility.Collapsed;
                              btn_fp.Visibility = Visibility.Collapsed;
                              firstmonth.IsEnabled = false;

                              savingsTxt.Text = "Savings - 12 Months Only";
                              ItemCollection ic = statusCombo.Items;
                              statusCombo.SelectedItem = ic.Last();

                              FormMsg.Text = "Idea is no longer Editable (except Project Comments).";

                              if ((isEmployee || isContractor) && isIdeaOwner)
                              {
                                  MakeReadOnly(false);
                                  isReadOnly = true;
                              }
                              else if (isEmployee || isContractor)
                              {
                                  MakeReadOnly(true);
                                  isReadOnly = true;
                              }


                          }

                          ideaname.Text = getItem("Idea_x0020_Name", listitems[0]);
                          description.Text = getItem("EXCEL_x0020_Idea_x0020_Descripti", listitems[0]);
                          Identify.Text = getItem("EXCEL_x0020_Identifier", listitems[0]);

                          if (Identify.Text == "1. E-Excessive Demand")
                          {
                              identify_e.IsChecked = true;
                          }
                          else if (Identify.Text == "2. X-eXpense Reduction")
                          {
                              identify_x.IsChecked = true;
                          }
                          else if (Identify.Text == "3. C–Customization Reduction")
                          {
                              identify_c.IsChecked = true;
                          }
                          else if (Identify.Text == "4. E–Effective Talent Utilization")
                          {
                              identify_e2.IsChecked = true;
                          }
                          else if (Identify.Text == "5. L–Less Duplication")
                          {
                              identify_l.IsChecked = true;
                          }

                          LoadRadios(getItem("Line_x0020_Of_x0020_Business_x001", listitems[0]), getItem("LOB_Tier2", listitems[0]) );
                        

                          assump_depend.Text = getItem("Assumptions_x0020_or_x0020_Depen", listitems[0]);
                          Risk.Text = getItem("Risk_x0020_of_x0020_Implementati", listitems[0]);

                          if (Risk.Text == "High")
                          {
                              risk_high.IsChecked = true;
                          }
                          else if (Risk.Text == "Medium")
                          {
                              risk_med.IsChecked = true;
                          }
                          else if (Risk.Text == "Low")
                          {
                              risk_low.IsChecked = true;
                          }


                          biz_capability.Text = getItem("Business_x0020_Capability", listitems[0]);
                          sdlc_projID.Text = getItem("SDLC_x0020_Project_x0020_ID", listitems[0]);
                          sdlc_projName.Text = getItem("SDLC_x0020_Project_x0020_Name", listitems[0]);


                          Save.Text = getItem("Vendor_Save", listitems[0]);
                          if (Save.Text == "Yes")
                          {
                              vendorSave_yes.IsChecked = true;
                          }
                          else if (Save.Text == "No")
                          {
                              vendorSave_no.IsChecked = true;
                          }

                          //ROLE FAMILY COMBO CODE



                          CT.Text = getItem("Cost_x0020_Type1", listitems[0]);

                          if (CT.Text == "Cost Avoidance")
                          {
                              type_Avoid.IsChecked = true;
                          }
                          else if (CT.Text == "Re-engineering (REE)")
                          {
                              type_reEngineer.IsChecked = true;
                              tech_impact1.Visibility = Visibility.Visible;
                              tech_impact.Text = getItem("Tech_Impact", listitems[0]);
                          }
                          else if (CT.Text == "Cost Reduction")
                          {
                              type_Reduction.IsChecked = true;
                          }
                          else if (CT.Text == "Growth Reduction")
                          {
                              type_Growth.IsChecked = true;
                          }




                          firstmonth.Text = getDateItem("_x0031_st_x0020_Mo_x0020_Saves_x", listitems[0]);

                          if (statusLevel.Text == "Draft" || statusLevel.Text == "Future Pipeline")
                          {
                              firstmonthText.Text = firstmonthText.Text.ToString();
                          }

                          revisedmonth.Text = getDateItem("Revised_x0020_1st_x0020_Mo_x0020", listitems[0]);

                          projcomText.Text = getItem("Project_x0020_Comments", listitems[0]);

                          aCommHistoryText.Text = getItem("admin_comments", listitems[0]);

                          Audit.Text = getItem("Audit", listitems[0]);

                          tp.Text = getItem("Top_x0020_Project", listitems[0]);

                          if (tp.Text == "Yes")
                          {
                              topprojectCheckBox.IsChecked = true;
                          }


                          cem.Text = getItem("CEM_x0020_Approved", listitems[0]);

                          if (cem.Text == "Yes")
                          {
                              CEM_Yes.IsChecked = true;
                          }
                          else if (cem.Text == "No")
                          {
                              CEM_No.IsChecked = true;
                          }



                          cc.Text = getItem("Cost_Classification", listitems[0]);

                          if (cc.Text == "MR")
                          {
                              ItemCollection CC = costClarify_combo.Items;
                              costClarify_combo.SelectedItem = CC.First();
                          }
                          if (cc.Text == "MF")
                          {
                              ItemCollection CC = costClarify_combo.Items;
                              costClarify_combo.SelectedItem = CC[1];
                          }
                          if (cc.Text == "Direct Tech")
                          {
                              ItemCollection CC = costClarify_combo.Items;
                              costClarify_combo.SelectedItem = CC[2];
                          }
                          if (cc.Text == "End User Computing")
                          {
                              ItemCollection CC = costClarify_combo.Items;
                              costClarify_combo.SelectedItem = CC[3];

                          }
                          if (cc.Text == "Other")
                          {
                              ItemCollection CC = costClarify_combo.Items;
                              costClarify_combo.SelectedItem = CC[4];
                          }

                          header1.Text = getItem("SavingsHeader1", listitems[0]);
                          header2.Text = getItem("SavingsHeader2", listitems[0]);
                          header3.Text = getItem("SavingsHeader3", listitems[0]);
                          header4.Text = getItem("SavingsHeader4", listitems[0]);
                          header5.Text = getItem("SavingsHeader5", listitems[0]);

                          es1.Value = Convert.ToDecimal(listitems[0].FieldValues["Savings1"]);
                          es2.Value = Convert.ToDecimal(listitems[0].FieldValues["Savings2"]);
                          es3.Value = Convert.ToDecimal(listitems[0].FieldValues["Savings3"]);
                          es4.Value = Convert.ToDecimal(listitems[0].FieldValues["Savings4"]);

                          es5.Value = Convert.ToDecimal(listitems[0].FieldValues["Savings5"]);

                          totalText.Text = Convert.ToDecimal(listitems[0].FieldValues["Total_x0020_Savings"]).ToString();
                          es_Total.Value = Convert.ToDecimal(totalText.Text);
                          

                          AIM_ID.Text = getItem("AIM_x0020_Application_x0020_ID", listitems[0]);
                          LoadComboItems(getItem("AIM_x0020_Application_x0020_Name", listitems[0]), getItem("AIM_x0020_Application_x0020_ID", listitems[0]));

                          LoadRoleItems(getItem(GlobalConsts.ROLEFAMILY_COLUMN, listitems[0]));

                      }



              );




                  },




(ss, eee) =>
{
    Console.WriteLine(eee.Message);

});



              }

              );

          },



 (s, ee) =>
 {
     Console.WriteLine(ee.Message);

 });

      }



      private void LoadRadios(String lob1, String lob2)
      {
          Dictionary<String, List<RadioButton>> lob2Radios = new Dictionary<string, List<RadioButton>>()
          {
              {"WS", new List<RadioButton> {lobwsgcat_ws}},
              {"GCA", new List<RadioButton> { lobwsgcat_gca}},
              {"WS and GCA", new List<RadioButton> {lobwsgcat_both}},
              {"GCST Pegasus", new List<RadioButton> {lobpbmt_pegasus}},
              {"Business Management and Transformation", new List<RadioButton> {lobpbmt_busmgmt}},
              {"GBT", new List<RadioButton> {lobgbs_gbt}},
              {"GFO", new List<RadioButton> {lobgbs_gfo}},
              {"GREWE", new List<RadioButton> {lobgbs_grewe}},
              {"GSM", new List<RadioButton> {lobgbs_gsm}},
              {"Tech", new List<RadioButton> {lobgbs_tech, lobhr_tech}},
              {"Other External Group", new List<RadioButton> {lobgbs_other, lobhr_other}},
              {"HR", new List<RadioButton> {lobhr_hr}},
              {"HR Benefits", new List<RadioButton> {lobhr_benefits}},
              {"PMO Only", new List<RadioButton> {lobhr_pmo}},
              {"GBT/JV", new List<RadioButton> {lobgbt_gbtjv}},
              {"GCP", new List<RadioButton> {lobgbt_gcp}},
              {"QMS", new List<RadioButton> {lobgbt_qms}}
          };

          Dictionary<String, RadioButton> lob1Radios = new Dictionary<string, RadioButton>()
          {
              {"GBS", gbs_radio},
              {"GCP", gcp_radio},
              {"PBMT", pbmt_radio},
              {"GBT", gbt_radio},
              {"WSGCAT", wsgcat_radio},
              {"HR", hr_radio}
             
          };

          Dictionary<RadioButton, List<RadioButton>> radioMapping = GetLOBMapping();


          foreach (String lob1Val in lob1Radios.Keys)
          {

              if (lob1Val.Equals(lob1, StringComparison.OrdinalIgnoreCase))
              {
                  lob1Radios[lob1Val].IsChecked = true;

                  List<RadioButton> lob2Radiolist = radioMapping[lob1Radios[lob1Val]];

                  foreach (String lob2Val in lob2Radios.Keys)
                  {
                      if (lob2Val.Equals(lob2, StringComparison.OrdinalIgnoreCase))
                      {
                          List<RadioButton> lob2mappedVals = lob2Radios[lob2Val];
                          foreach (RadioButton radio in lob2mappedVals)
                          {
                              if (lob2Radiolist.Contains(radio))
                              {
                                  radio.IsChecked = true;
                                  break;
                              }

                          }
                      }
                     
                  }


                  
              }
          }
      }

        #endregion



      private delegate void UpdateUIMethod();
        ChildWindow1 childwin;
        FTEMsgBox ftewin;
        close closewin;
        Messages msgwin;
        ChildWindow2 cancelwin;



        #region V A L I D A T I O N

        private void UserTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            Dictionary<TextBox, TextBlock> peoplePickers = GetPeoplePickerTextCollection();

            foreach (TextBox txtBox in peoplePickers.Keys)
            {
                if (txtBox.Text.Length != 0)
                    ResetFormatting(peoplePickers[txtBox]);
            }

        }

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
            else
            {
                SetFalseResult(result, TAB.OVERVIEW);
            }
          

            foreach (TextBox txtBox in peoplePickers)
            {
                SolidColorBrush brush=txtBox.BorderBrush as SolidColorBrush;
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
            RadioButton selectedRadio=null;

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

            //if (totalText.Text == "$0.00" || totalText.Text == "0.00" || totalText.Text == "0")
            //{
            //    SetFalseResult(result, TAB.FINANCIAL);
            //    FormatControlForValidation(savingsTxt);
            //}

            if (es_Total.Value == 0)
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


      

        private void MakeReadOnly(bool isAddButtonReadOnly)
        {
            TextBox[] allTexts = new TextBox[] { tech_impact, assump_depend, ideaname, description, SinglePeopleChooser.UserTextBox, SinglePeopleChooser1.UserTextBox, SinglePeopleChooser2.UserTextBox, sdlc_projID, sdlc_projName, biz_capability };
            RadMaskedCurrencyInput[] allMarks = new RadMaskedCurrencyInput[] { es1, es2, es3, es4, es5, es_Total };
            Dictionary<RadioButton, List<RadioButton>> radios = GetLOBMapping();
            RadioButton[] radioButtons = new RadioButton[] { type_Avoid, type_reEngineer, type_Reduction, type_Growth, vendorSave_yes, vendorSave_no, risk_high, risk_med, risk_low, identify_e, identify_x, identify_c, identify_e2, identify_l };
            foreach (TextBox txtBox in allTexts)
                txtBox.IsEnabled = false;
            foreach (RadMaskedCurrencyInput radMask in allMarks)
                radMask.IsEnabled = false;

            foreach (RadioButton radio in radios.Keys)
            {
                radio.IsEnabled = false;
                List<RadioButton> allRadio = radios[radio];
                foreach (RadioButton radio1 in allRadio)
                    radio1.IsEnabled = false;
            }

            foreach (RadioButton button in radioButtons)
            {
                button.IsEnabled = false;

            }

            firstmonth.IsEnabled = false;
            revisedmonth.IsEnabled = false;

            btn_approve.IsEnabled = false;
            btn_fp.IsEnabled = false;
            btn_draft.IsEnabled = false;
            btn_inprogress.IsEnabled = false;

            //Add button does not need to be disabled as per fast follower
            if (isAddButtonReadOnly)
                btn_add.IsEnabled = false;
            else
                btn_add.IsEnabled = true;

            Remove.IsEnabled = false;
            View.IsEnabled = true;
            Add.IsEnabled = false;
            btn_save.IsEnabled = false;
            btn_save1.IsEnabled = false;
            btn_save2.IsEnabled = false;
            btn_save3.IsEnabled = false;

            aimcombo.IsEnabled = false;

            SinglePeopleChooser.SetDisabled();
            SinglePeopleChooser1.SetDisabled();
            SinglePeopleChooser2.SetDisabled();
            MultiplePeopleChooser.SetDisabled();


            btn_cancel.IsEnabled = false;

            rolecombo.IsEnabled = false;

       
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

        private TextBox[] GetPeoplePickerCollection()
        {
            TextBox[] textBoxes = new TextBox[] {SinglePeopleChooser.UserTextBox, SinglePeopleChooser1.UserTextBox, SinglePeopleChooser2.UserTextBox };
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
            btnstack_financialSave.Visibility = Visibility.Collapsed;
            financials_image.Visibility = Visibility.Collapsed;


            comments_image.Visibility = Visibility.Collapsed;
            btnstack_comments.Visibility = Visibility.Collapsed;
            btnstack_commentsSave.Visibility = Visibility.Collapsed;

            btnstack_acomments.Visibility = Visibility.Collapsed;
            btn_comments.IsEnabled = true;
            btnAdmin_comments.IsEnabled = true;

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
            btnstack_financialSave.Visibility = Visibility.Collapsed;
            financials_image.Visibility = Visibility.Collapsed;


            comments_image.Visibility = Visibility.Collapsed;
            btnstack_comments.Visibility = Visibility.Collapsed;
            btnstack_commentsSave.Visibility = Visibility.Collapsed;

            btnstack_acomments.Visibility = Visibility.Collapsed;
            btn_comments.IsEnabled = true;
            btnAdmin_comments.IsEnabled = true;


            btn_inprogress.Visibility = Visibility.Collapsed;
            btn_approve.Visibility = Visibility.Collapsed;
            btn_fp.Visibility = Visibility.Collapsed;



        }

        private void NavigateFinancialTab()
        {

            this.tabcontrol1.SelectedIndex = 2;
            btnstack_scope.Visibility = Visibility.Collapsed;
            btnstack_scopeBack.Visibility = Visibility.Collapsed;

            btnstack_financial.Visibility = Visibility.Visible;
            btnstack_financialSave.Visibility = Visibility.Visible;

            btnstack_acomments.Visibility = Visibility.Collapsed;

            btnstack_comments.Visibility = Visibility.Collapsed;
            btnstack_commentsSave.Visibility = Visibility.Collapsed;
            btn_comments.IsEnabled = true;
            btnAdmin_comments.IsEnabled = true;

            scope_image.Visibility = Visibility.Collapsed;
            financials_image.Visibility = Visibility.Visible;

            if (statusLevel.Text == "In Progress")
            {
                btn_fp.Visibility = Visibility.Visible;
                btn_inprogress.Visibility = Visibility.Collapsed;
                btn_approve.Visibility = Visibility.Visible;
            }
         

            else if (statusLevel.Text == "Future Pipeline" )
            {
                btn_inprogress.Visibility = Visibility.Visible;
                btn_approve.Visibility = Visibility.Visible;
                btn_fp.Visibility = Visibility.Collapsed;

            }

        

            else if (statusLevel.Text == "Draft" )
            {
                btn_approve.Visibility = Visibility.Visible;
                btn_inprogress.Visibility = Visibility.Visible;
                btn_fp.Visibility = Visibility.Visible;
                
             
            }
          
        



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
            Messages msgwin = new Messages(this);
            msgwin.msgtxt.Text = GetErrorMessage(result);
            msgwin.alert.Visibility = Visibility.Visible;
            msgwin.RequiredOKButton.Visibility = Visibility.Visible;
            return msgwin;

        }

        #endregion REQUIRED LOGIC 

        #region R A D I O  B U T T O N  LOGIC

        private void adminButtons_Checked(object sender, RoutedEventArgs e)
        {
            cem.ClearValue(TextBox.TextProperty);
            tp.ClearValue(TextBox.TextProperty);

            if (CEM_No.IsChecked == true)
            {
                cem.Text = "No";
            }
            else if (CEM_Yes.IsChecked == true)
            {
                cem.Text = "Yes";
            }

            if (topprojectCheckBox.IsChecked == true)
            {
                tp.Text = "Yes";
            }
        }

        private void LOBRadioButtons_Checked(object sender, RoutedEventArgs e)
        {

            if (gbs_radio.IsChecked == true)
            {
                lobgbs.Visibility = Visibility.Visible;
                lobpbmt.Visibility = Visibility.Collapsed;
                lobwsgcat.Visibility = Visibility.Collapsed;
                lobhr.Visibility = Visibility.Collapsed;
                lobgbt.Visibility = Visibility.Collapsed;




            }
            else if (gbt_radio.IsChecked == true)
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
                            ResetFormatting(gbsTxt);

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
                            ResetFormatting(gbtTxt);

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
                            ResetFormatting(pbmtTxt);

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
                            ResetFormatting(wsgcatTxt);
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
                            ResetFormatting(hrTxt);

                        }
                    }
                }
            }

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
//                        savingsTxt.Foreground = new SolidColorBrush(Colors.Black);
                        ResetFormatting(vendorTxt);


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

        private void PopUpButton12_Click(object sender, RoutedEventArgs e)
        {
            myPopup_revised.IsOpen = false;
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

        private void imghelp_revisedmonth_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            PopUpText_revised.Text = "Used for adjustments to the 1st Months Saves Date after it has been locked.";
            myPopup_revised.IsOpen = true;
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
            if (statusLevel.Text == "Draft" || statusLevel.Text == "Future Pipeline" || statusLevel.Text == "In Progress")
            {

                closewin = new close();
                closewin.closeTxt.Text = " Any changes you made have not been saved. You can click 'No', then click either 'Save' or the next status button.";
                closewin.Width = 400;
                closewin.Height = 175;
                closewin.Show();
                closewin.SubmitClicked += new EventHandler(UserControl_Unloaded_1);
            }
            else
            {

                closewin = new close();
                closewin.Height = 175;
                closewin.Width = 400;
                closewin.Show();
                closewin.SubmitClicked += new EventHandler(UserControl_Unloaded_1);
            }
        }


        private void btn_next_Click(object sender, System.Windows.RoutedEventArgs e)
        {

          

            NavigateScopeTab();

        }


        private void btn_next2_Click(object sender, RoutedEventArgs e)
        {

            NavigateFinancialTab();
       
            
            
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
                NavigateFinancialTab();

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
            btnstack_financialSave.Visibility = Visibility.Collapsed;

            btnstack_comments.Visibility = Visibility.Visible;
            btnstack_commentsSave.Visibility = Visibility.Visible;
            btnstack_acomments.Visibility = Visibility.Collapsed;

            btn_comments.IsEnabled = false;

            comments_image.Visibility = Visibility.Visible;
            financials_image.Visibility = Visibility.Collapsed;
            overview_image.Visibility = Visibility.Collapsed;
            scope_image.Visibility = Visibility.Collapsed;

            btn_inprogress.Visibility = Visibility.Collapsed;
            btn_approve.Visibility = Visibility.Collapsed;
            btn_fp.Visibility = Visibility.Collapsed;
            btnAdmin_comments.IsEnabled = true;

        }

        private void btn_back4_Click(object sender, RoutedEventArgs e)
        {
            NavigateFinancialTab();
                       
              

        }


        private void btn_acomments_Click(object sender, RoutedEventArgs e)
        {

            this.tabcontrol1.SelectedIndex = 4;

            btnstack_scope.Visibility = Visibility.Collapsed;
            btnstack_scopeBack.Visibility = Visibility.Collapsed;

            btnstack_overview.Visibility = Visibility.Collapsed;
            btnstack_overviewClose.Visibility = Visibility.Collapsed;

            btnstack_financial.Visibility = Visibility.Collapsed;
            btnstack_financialSave.Visibility = Visibility.Collapsed;

            btnstack_acomments.Visibility = Visibility.Visible;
            btnstack_comments.Visibility = Visibility.Collapsed;
            btnstack_commentsSave.Visibility = Visibility.Collapsed;

            btn_comments.IsEnabled = true;
            btnAdmin_comments.IsEnabled = false;

            comments_image.Visibility = Visibility.Visible;
            financials_image.Visibility = Visibility.Collapsed;
            overview_image.Visibility = Visibility.Collapsed;
            scope_image.Visibility = Visibility.Collapsed;

            btn_inprogress.Visibility = Visibility.Collapsed;
            btn_approve.Visibility = Visibility.Collapsed;
            btn_fp.Visibility = Visibility.Collapsed;

        }


        #endregion


        #region S T A T U S   B U T T O N S


        //Begin Status Buttons

        User Singleuser;
        User Singleuser1;
        User Singleuser2;
        private string id;

        void required_OKClicked(object sender, EventArgs e)
        {
            if (SinglePeopleChooser.UserTextBox.Text.Length == 0 || SinglePeopleChooser1.UserTextBox.Text.Length == 0 || SinglePeopleChooser2.UserTextBox.Text.Length == 0 || description.Text.Length == 0 || ideaname.Text.Length == 0 || resultsLOB2.Text.Length == 0)
            {
                this.tabcontrol1.SelectedIndex = 0;
                btnstack_overview.Visibility = Visibility.Visible;
                overview_image.Visibility = Visibility.Visible;
                scope_image.Visibility = Visibility.Collapsed;
                btnstack_scope.Visibility = Visibility.Collapsed;
                btnstack_financial.Visibility = Visibility.Visible;
                financials_image.Visibility = Visibility.Visible;
                comments_image.Visibility = Visibility.Collapsed;
                btnstack_comments.Visibility = Visibility.Collapsed;


            }
            if (Identify.Text.Length == 0 || Risk.Text.Length == 0 || Save.Text.Length == 0 || CT.Text == "type_reEngineer" && tech_impact.Text.Length == 0)
            {
                this.tabcontrol1.SelectedIndex = 1;
                btnstack_overview.Visibility = Visibility.Collapsed;
                overview_image.Visibility = Visibility.Collapsed;
                scope_image.Visibility = Visibility.Visible;
                btnstack_scope.Visibility = Visibility.Visible;
                btnstack_financial.Visibility = Visibility.Collapsed;
                financials_image.Visibility = Visibility.Collapsed;
                comments_image.Visibility = Visibility.Collapsed;
                btnstack_comments.Visibility = Visibility.Collapsed;

            }
            if (Identify.Text.Length == 0 || Risk.Text.Length == 0 || Save.Text.Length == 0 || CT.Text.Length == 0)
            {
                this.tabcontrol1.SelectedIndex = 1;
                btnstack_overview.Visibility = Visibility.Collapsed;
                overview_image.Visibility = Visibility.Collapsed;
                scope_image.Visibility = Visibility.Visible;
                btnstack_scope.Visibility = Visibility.Visible;
                btnstack_financial.Visibility = Visibility.Collapsed;
                financials_image.Visibility = Visibility.Collapsed;
                comments_image.Visibility = Visibility.Collapsed;
                btnstack_comments.Visibility = Visibility.Collapsed;

            }

            if (firstmonthText.Text.Length == 0 || totalText.Text.Length == 0 || FileListBox.Items.Count == 0)
            {
                this.tabcontrol1.SelectedIndex = 2;
                btnstack_overview.Visibility = Visibility.Collapsed;
                overview_image.Visibility = Visibility.Collapsed;
                scope_image.Visibility = Visibility.Collapsed;
                btnstack_scope.Visibility = Visibility.Collapsed;
                btnstack_financial.Visibility = Visibility.Visible;
                financials_image.Visibility = Visibility.Visible;
                comments_image.Visibility = Visibility.Collapsed;
                btnstack_comments.Visibility = Visibility.Collapsed;
            }


        }

        private void SetFields(ListItem updateItem, ClientContext context)
        {

            if (SinglePeopleChooser.selectedAccounts.Count > 0 || MultiplePeopleChooser.selectedAccounts.Count > 0)
            {

                if (SinglePeopleChooser.selectedAccounts.Count > 0)
                {
                    Singleuser = context.Web.EnsureUser(SinglePeopleChooser.selectedAccounts[0].AccountName);
                    updateItem["Executor"] = Singleuser;

                    Singleuser1 = context.Web.EnsureUser(SinglePeopleChooser1.selectedAccounts[0].AccountName);
                    updateItem["Director"] = Singleuser1;

                    Singleuser2 = context.Web.EnsureUser(SinglePeopleChooser2.selectedAccounts[0].AccountName);
                    updateItem["VP"] = Singleuser2;

                }
                if (MultiplePeopleChooser.selectedAccounts.Count > 0)
                {
                    List<FieldUserValue> usersList = new List<FieldUserValue>();
                    foreach (AccountList ac in MultiplePeopleChooser.selectedAccounts)
                    {
                        usersList.Add(FieldUserValue.FromUser(ac.AccountName));
                    }

                    updateItem["FTE_x0020_Contributors"] = usersList;
                }
                else
                {
                    updateItem["FTE_x0020_Contributors"] = null;
                }

            }

            //<-----Project Overview Tab ------>
            updateItem["Idea_x0020_Name"] = ideaname.Text;
            updateItem["EXCEL_x0020_Idea_x0020_Descripti"] = description.Text;


            SetRadioStatus(updateItem);
            //<------Scope Tab------>

            MyItem item = aimcombo.SelectedItem as MyItem;
            if (item != null)
                updateItem["AIM_x0020_Application_x0020_Name"] = item.AIM_NAME;

            updateItem["AIM_x0020_Application_x0020_ID"] = AIM_ID.Text;
            updateItem["_x0031_st_x0020_Mo_x0020_Saves_x"] = firstmonth.SelectedDate;
            updateItem["Revised_x0020_1st_x0020_Mo_x0020"] = revisedmonth.SelectedDate;


            RoleItem roleItem = rolecombo.SelectedItem as RoleItem;
            if (roleItem != null)
                updateItem[GlobalConsts.ROLEFAMILY_COLUMN] = roleItem.Name;

            if (identify_e.IsChecked == true)
            {
                updateItem["EXCEL_x0020_Identifier"] = "1. E-Excessive Demand";
            }
            else if (identify_x.IsChecked == true)
            {
                updateItem["EXCEL_x0020_Identifier"] = "2. X-eXpense Reduction";
            }
            else if (identify_c.IsChecked == true)
            {
                updateItem["EXCEL_x0020_Identifier"] = "3. C–Customization Reduction";
            }
            else if (identify_e2.IsChecked == true)
            {
                updateItem["EXCEL_x0020_Identifier"] = "4. E–Effective Talent Utilization";
            }
            else if (identify_l.IsChecked == true)
            {
                updateItem["EXCEL_x0020_Identifier"] = "5. L–Less Duplication";
            }

            //Assumptions, Risk, Business Capability, SDLC
            updateItem["Assumptions_x0020_or_x0020_Depen"] = assump_depend.Text;

            if (risk_high.IsChecked == true)
            {
                updateItem["Risk_x0020_of_x0020_Implementati"] = "High";
            }

            else if (risk_med.IsChecked == true)
            {
                updateItem["Risk_x0020_of_x0020_Implementati"] = "Medium";
            }
            else if (risk_low.IsChecked == true)
            {
                updateItem["Risk_x0020_of_x0020_Implementati"] = "Low";
            }

            updateItem["Business_x0020_Capability"] = biz_capability.Text;
            updateItem["SDLC_x0020_Project_x0020_ID"] = sdlc_projID.Text;
            updateItem["SDLC_x0020_Project_x0020_Name"] = sdlc_projName.Text;

            //vendor save
            if (vendorSave_yes.IsChecked == true)
            {
                updateItem["Vendor_Save"] = "Yes";
            }

            else if (vendorSave_no.IsChecked == true)
            {
                updateItem["Vendor_Save"] = "No";
            }

            //Cost Type
            if (type_Avoid.IsChecked == true)
            {
                updateItem["Cost_x0020_Type1"] = "Cost Avoidance";
            }

            else if (type_reEngineer.IsChecked == true)
            {
                updateItem["Cost_x0020_Type1"] = "Re-engineering (REE)";
                updateItem["Tech_Impact"] = tech_impact.Text;
            }

            else if (type_Reduction.IsChecked == true)
            {
                updateItem["Cost_x0020_Type1"] = "Cost Reduction";
            }
            else if (type_Growth.IsChecked == true)
            {
                updateItem["Cost_x0020_Type1"] = "Growth Reduction";
            }

            //<-----estimated savings----->

            updateItem["SavingsHeader1"] = header1.Text;
            updateItem["SavingsHeader2"] = header2.Text;
            updateItem["SavingsHeader3"] = header3.Text;
            updateItem["SavingsHeader4"] = header4.Text;
            updateItem["SavingsHeader5"] = header5.Text;

            updateItem["Savings1"] = es1.Value;
            updateItem["Savings2"] = es2.Value;
            updateItem["Savings3"] = es3.Value;
            updateItem["Savings4"] = es4.Value;
            updateItem["Savings5"] = es5.Value;

            updateItem["Total_x0020_Savings"] = es_Total.Value;

            //<-----comments, status & audit----->

            updateItem["Project_x0020_Comments"] = projcomText.Text;
        }


        private void btn_draft_Click(object sender, RoutedEventArgs e)
        {
        }



        private void HandleSaveException(String message)
        {
            if (message.Equals(GlobalConsts.USER_MISSING_MSG, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(GlobalConsts.USER_MISSING_MSG_NEW);
                NavigateScopeTab();
            }
            else
            {
                MessageBox.Show(message);
            }

          
        }

        //<~~~~~~~~FUTURE PIPELINE~~~~~~~~>
        private void btn_future_Click(object sender, RoutedEventArgs e)
        {

            if (!DoDateCheck())
                return;
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
                    //Get the current context 
                    ClientContext context = ClientContext.Current;
                    //Get the Idea list and add a new item 
                    Idea = context.Web.Lists.GetByTitle("Idea");
                    context.Load(Idea);
                    MarkFilesPermanent();
                    RemoveDeletedFiles();
                    ListItem updateItem = Idea.GetItemById(ideaID.Text);
                    //Set the new item's properties 
                    MarkFilesPermanent();
                    RemoveDeletedFiles();

                    SetFields(updateItem, context);

                    updateItem["scale"] = "8";
                    updateItem["Idea_x0020_Status"] = "Future Pipeline";
                    varaudit = Audit.Text + Environment.NewLine + currUser.Text + " (" + DateTime.Now.ToString("M/d/yyyy - h:mm:ss tt") + ")" + " - " + "successfully submitted the idea as future pipeline";
                    updateItem["Audit"] = varaudit;

                    updateItem.Update();
                    //Load the list 
                    context.Load(Idea, list => list.Title);


                    //Execute the query to update the new item 
                    busyIndicator.IsBusy = true;
                    context.ExecuteQueryAsync((s, ee) =>
                    {

                        RenameFolder(Utils.GetSiteUrl(), libName, string.Empty, folderName, itemId, "Your idea was successfully submitted as future pipeline.");
                     
                    },
        (s, ee) =>
        {
            Dispatcher.BeginInvoke(() =>
            {
                busyIndicator.IsBusy = false;
                HandleSaveException(ee.Message);
            }
                        );

        });
                }
            }
        }

        private void ShowMessage(String message)
        {
            msgwin = new Messages(this);
            msgwin.msgtxt.Text = message;
            msgwin.SubmitOKButton.Visibility = Visibility.Visible;
            msgwin.RequiredOKButton.Visibility = Visibility.Collapsed;
            msgwin.alert.Visibility = Visibility.Collapsed;

            msgwin.Show();
            msgwin.Closed += msgwin_Closed;

        }


        //<~~~~~~BEGIN IN PROGRESS~~~~~>
        private void btn_inprogress_Click(object sender, RoutedEventArgs e)
        {
            if (!DoDateCheck())
                return;
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

                    //Get the current context 
                    ClientContext context = ClientContext.Current;
                    //Get the Idea list and add a new item 
                    Idea = context.Web.Lists.GetByTitle("Idea");

                    context.Load(Idea);
                    ListItem updateItem = Idea.GetItemById(ideaID.Text);
                    MarkFilesPermanent();
                    RemoveDeletedFiles();

                    SetFields(updateItem, context);

                    updateItem["Idea_x0020_Status"] = "In Progress";
                    updateItem["scale"] = "2";

                    varaudit = Audit.Text + Environment.NewLine + currUser.Text + " (" + DateTime.Now.ToString("M/d/yyyy - h:mm:ss tt") + ")" + " - " + "successfully submitted the idea as in progress";
                    updateItem["Audit"] = varaudit;
                    updateItem.Update();
                    //Load the list 
                    context.Load(Idea, list => list.Title);
                    //Execute the query to create the new item 

                    busyIndicator.IsBusy = true;
                    context.ExecuteQueryAsync((s, ee) =>
                    {
                        RenameFolder(Utils.GetSiteUrl(), libName, string.Empty, folderName, itemId, "Your idea was successfully submitted as in progress.");
                    },
             (s, ee) =>
             {
                 Dispatcher.BeginInvoke(() =>
                 {
                     busyIndicator.IsBusy = false;
                     HandleSaveException(ee.Message);
                 }
                       );

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

        }

        void YesClicked(object sender, EventArgs e)
        {

            //Get the current context 
            ClientContext context = ClientContext.Current;
            //Get the Idea list and add a new item 
            Idea = context.Web.Lists.GetByTitle("Idea");

            context.Load(Idea);

            ListItem updateItem = Idea.GetItemById(ideaID.Text);

            MarkFilesPermanent();
            RemoveDeletedFiles();
            //Set the new item's properties 

            SetFields(updateItem, context);

            updateItem["Idea_x0020_Status"] = "Submit for Approval";
            updateItem["scale"] = "3";


            varaudit = Audit.Text + Environment.NewLine + currUser.Text + " (" + DateTime.Now.ToString("M/d/yyyy - h:mm:ss tt") + ")" + " - " + "successfully submitted the idea for approval";
            updateItem["Audit"] = varaudit;

            updateItem.Update();
            //Load the list 
            context.Load(Idea, list => list.Title);
            //Execute the query to create the new item 
            busyIndicator.IsBusy = true;
            context.ExecuteQueryAsync((s, ee) =>
            {

                RenameFolder(Utils.GetSiteUrl(), libName, string.Empty, folderName, itemId, "Your idea was successfully submitted for approval.");
            },
(s, ee) =>
{
    Dispatcher.BeginInvoke(() =>
    {
        busyIndicator.IsBusy = false;
        HandleSaveException(ee.Message);
    }
                        );

});


        }

        private ValidateResult GetValidationResultOnStatus(String status)
        {
            ValidateResult draftResult, progressResult, result;

            if (status.Equals(Status.APPROVED) || status.Equals(Status.FINANCE_REVIEW) || status.Equals(Status.READY_FINANCE_REVIEW) || status.Equals(Status.SUBMIT_APPROVAL))
            {
                draftResult = ValidateForDraft();
                progressResult = ValidateForInProgress(draftResult);
                result = ValidateForApproval(progressResult);
                return result;
            }
            else if (status.Equals(Status.DRAFT))
            {
                draftResult = ValidateForDraft();
                return draftResult;
            }
            else if (status.Equals(Status.IN_PROGRESS) || status.Equals(Status.PENDING_ACTUALS) || status.Equals(Status.FUTURE_PIPELINE) )
            {
                draftResult = ValidateForDraft();
                progressResult = ValidateForInProgress(draftResult);
                return progressResult;
            }


            draftResult = ValidateForDraft(); ;
            return draftResult;
        }


        private void ShowPastDateError(String errorMsg)
        {

            msgwin = new Messages(this);
            msgwin.SubmitOKButton.Visibility = Visibility.Collapsed;
            msgwin.RequiredOKButton.Visibility = Visibility.Visible;
            msgwin.msgtxt.Text = errorMsg;
            msgwin.alert.Visibility = Visibility.Collapsed;

            msgwin.Show();
            msgwin.Closed += msgwin_Closed;

        }

        private bool DoDateCheck()
        {

            if (revisedmonth.SelectedDate != null)
            {
                if (revisedmonth.SelectedDate <= DateTime.Today.AddDays(-1) )
                {
                    ShowPastDateError(Consts.PAST_DATE_ERROR_REVISED);
                    return false;
                }
            }

            else if (firstmonth.SelectedDate != null)
            {
                if (firstmonth.SelectedDate <= DateTime.Today.AddDays(-1) )
                {
                    ShowPastDateError(Consts.PAST_DATE_ERROR);
                    return false;
                }
            }

            return true;

        }

        //<-------------BEGIN SAVE BUTTON ------------->>>

        private void SetRadioStatus(ListItem updateItem)
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



        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (statusLevel.Text.Equals(Status.IN_PROGRESS) || statusLevel.Text.Equals(Status.FUTURE_PIPELINE))
            {
                if (!DoDateCheck())
                    return;
            }

            ResetControls();

            ValidateResult result = GetValidationResultOnStatus(statusLevel.Text);
            if (!result.IsValid)
            {

                GetErrorWindow(result).Show();
                NavigateTab(result);
            }

            else
            {

                //Get the current context 
                ClientContext context = ClientContext.Current;
                //Get the Idea list and add a new item 
                Idea = context.Web.Lists.GetByTitle("Idea");


                context.Load(Idea);

                ListItem updateItem = Idea.GetItemById(ideaID.Text);

                MarkFilesPermanent();
                RemoveDeletedFiles();
                //Set the new item's properties 

                SetFields(updateItem, context);

                updateItem["Idea_x0020_Status"] = statusLevel.Text;
                updateItem["admin_comments"] = aCommHistoryText.Text;

                if (!status.Equals(NewStatus) && !String.IsNullOrEmpty(NewStatus))
                {
                    varaudit = Audit.Text + Environment.NewLine + currUser.Text + " (" + DateTime.Now.ToString("M/d/yyyy - h:mm:ss tt") + ")" + " - " + "successfully changed the status of the idea to" + " " + NewStatus;

                }

                else
                {

                    varaudit = Audit.Text + Environment.NewLine + currUser.Text + " (" + DateTime.Now.ToString("M/d/yyyy - h:mm:ss tt") + ")" + " - " + "successfully saved changes to the idea";

                }
                updateItem["Audit"] = varaudit;

                updateItem.Update();
                //Load the list 
                context.Load(Idea, list => list.Title);
                //Execute the query to create the new item 
                busyIndicator.IsBusy = true;
                context.ExecuteQueryAsync((s, ee) =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        busyIndicator.IsBusy = false;
                        ShowMessage("Your changes were successfully saved.");
                    }
                        );


                },
    (s, ee) =>
    {
        Dispatcher.BeginInvoke(() =>
                   {
                       busyIndicator.IsBusy = false;
                       HandleSaveException(ee.Message);
                   }
                       );


    });

            }
        }
    

        void msgwin_Closed(object sender, EventArgs e)
        {
            try
            {
                if (mainPage != null)
                    mainPage.Refresh() ;
            }
            catch (Exception e1)
            {
                Console.WriteLine(e1.Message);
            }

        }


        //<-----------BEGIN CANCEL BUTTON ------------->>>

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            cancelwin = new ChildWindow2();
            cancelwin.Show();
            cancelwin.cancelSaveClicked += new EventHandler(Cancel_Message);
        }

        private void Cancel_Message(object sender, EventArgs e)
        {
            //Get the current context 
            using (ClientContext context = new ClientContext(Utils.GetSiteUrl()))
            {
                Idea = context.Web.Lists.GetByTitle("Idea");


                context.Load(Idea);

                ListItem updateItem = Idea.GetItemById(ideaID.Text);

                updateItem["Canceled_Comments"] = cancelwin.cancelComments.Text;
                updateItem["Idea_x0020_Status"] = "Canceled";
                updateItem["scale"] = "6";
                varaudit = Audit.Text + Environment.NewLine + currUser.Text + " (" + DateTime.Now.ToString("M/d/yyyy - h:mm:ss tt") + ")" + " - " + "canceled this idea" + " (" + "Reason:" + "-" + cancelwin.cancelComments.Text.ToString() + ")";
                updateItem["Audit"] = varaudit;


                updateItem.Update();
                //Load the list 
                context.Load(Idea, list => list.Title);
                //Execute the query to create the new item 
                busyIndicator.IsBusy = true;
                context.ExecuteQueryAsync((s, ee) =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        busyIndicator.IsBusy = false;
                        ShowMessage("Your idea was successfully canceled.");

                    }
                        );


                },
    (s, ee) =>
    {
        Dispatcher.BeginInvoke(() =>
        {
            busyIndicator.IsBusy = false;
            HandleSaveException(ee.Message);

        }
                        );

    });
            }

        }



        #endregion


        #region A I M  D R O P D O W N

        private void LoadComboItems(String aimName, String aimID)
        {
            using (ClientContext context = new ClientContext(Utils.GetSiteUrl()))
            {
                Web web = context.Web;
                context.Load(web);
                List list = context.Web.Lists.GetByTitle("AIM");
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View><Query><FieldRef Name='Title' /><FieldRef Name='AIM_x0020_Application_x0020_ID' /><OrderBy> <FieldRef Name='Title'/></OrderBy></Query></View>";
                ListItemCollection listItems = list.GetItems(camlQuery);
                context.Load(listItems);

                context.ExecuteQueryAsync((s, ee) =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        foreach (ListItem listitem in listItems)
                        {
                            items.Add(new MyItem { AIM_ID = listitem.FieldValues["AIM_x0020_Application_x0020_ID"].ToString(), AIM_NAME = listitem.FieldValues["Title"].ToString() });

                        }

                        aimcombo.DisplayMemberPath = "AIM_NAME";
                        aimcombo.SelectedValuePath = "AIM_ID";
                        aimcombo.SelectedValue = "{Binding AIM_ID}";
                        aimcombo.ItemsSource = items;
                        aimcombo.DataContext = items;

                        try
                        {
                            if (!String.IsNullOrEmpty(aimID))
                            {
                                aimcombo.SelectedValue = aimID;                           
                            }
                        }
                        finally
                        {
                            busyIndicator.IsBusy = false;
                            formLoad = false;
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

        private void LoadRoleItems(String selectedRole)
        {
            ClientContext context = ClientContext.Current;

            Web web = context.Web;
            context.Load(web);
            List list = context.Web.Lists.GetByTitle(GlobalConsts.ROLE_LIST);
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = GlobalConsts.ROLE_QUERY;
            ListItemCollection listItems = list.GetItems(camlQuery);
            context.Load(listItems);
            List<RoleItem> roleItems = new List<RoleItem>();

            context.ExecuteQueryAsync((s, ee) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    foreach (ListItem listitem in listItems)
                    {
                        if (listitem.FieldValues[GlobalConsts.TITLE_COLUMN] != null)
                        {
                            roleItems.Add(new RoleItem { Name = listitem.FieldValues[GlobalConsts.TITLE_COLUMN].ToString() });
                        }

                    }


                    rolecombo.DisplayMemberPath = GlobalConsts.NAME_FIELD;
                    rolecombo.SelectedValuePath = GlobalConsts.NAME_FIELD;
                    rolecombo.SelectedValue = "{Binding Name}";
                    rolecombo.ItemsSource = roleItems;
                    rolecombo.DataContext = roleItems;

                    try
                    {
                        if (!String.IsNullOrEmpty(selectedRole))
                        {
                            rolecombo.SelectedValue = selectedRole;
                        }
                    }
                    finally
                    {

                    }

                }

                    );

            },



(s, ee) =>
{
    Console.WriteLine(ee.Message);

});
        }

        private void aimcombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            MyItem item = aimcombo.SelectedItem as MyItem;
            if (item != null)
            {
                AIM_ID.Text = item.AIM_ID;
            }

        }


        #endregion


        #region P E O P L E P I C K E R

        private void GetLoginName(ClientContext ctx, FieldUserValue singleValue)
        {
            ListItem principal = null;

            List userList = ctx.Web.SiteUserInfoList;
            ctx.Load(userList);

            ListItemCollection users = userList.GetItems(CamlQuery.CreateAllItemsQuery());

            ctx.Load(users, items => items.Include(
                item => item.Id, item => item["Name"]));


            //busyIndicator.IsBusy = true;
            ctx.ExecuteQueryAsync((ss, eee) =>
            {
                if (singleValue != null)
                {
                    principal = users.GetById(singleValue.LookupId);
                    ctx.Load(principal);
                }

                ctx.ExecuteQueryAsync((sss, eeee) =>
                {
                    if (singleValue != null && principal != null)
                    {
                        string username = principal["Name"] as string;

                        createdBy = Utils.checkClaimsUser(username);
                        string dispName = principal["Title"] as string;
                    }
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


        }


        private void LoadSingleUser(ClientContext ctx, FieldUserValue[] singleValue)
        {


            List userList = ctx.Web.SiteUserInfoList;
            ctx.Load(userList);

            ListItemCollection users = userList.GetItems(CamlQuery.CreateAllItemsQuery());

            ctx.Load(users, items => items.Include(
                item => item.Id, item => item["Name"]));


            busyIndicator.IsBusy = true;
            ctx.ExecuteQueryAsync((ss, eee) =>
            {
                ListItem[] principals = new ListItem[3];
                for (int i = 0; i < 3; ++i)
                {
                    if (singleValue[i] != null)
                    {
                        principals[i] = users.GetById(singleValue[i].LookupId);
                        ctx.Load(principals[i]);
                    }
                }

                ctx.ExecuteQueryAsync((sss, eeee) =>
                {


                    Dispatcher.BeginInvoke(() =>
                    {
                        PeopleChooser[] chooser = new PeopleChooser[] { SinglePeopleChooser, SinglePeopleChooser1, SinglePeopleChooser2 };
                        for (int i = 0; i < 3; ++i)
                        {
                            if (singleValue[i] != null && principals[i] != null)
                            {
                                string username = principals[i]["Name"] as string;

                                string decodedName = Utils.checkClaimsUser(username);
                                string dispName = principals[i]["Title"] as string;

                                chooser[i].selectedAccounts.Clear();

                                chooser[i].selectedAccounts.Add(new AccountList(decodedName, dispName));
                                chooser[i].UserTextBox.Text = dispName;
                            }
                        }

                        LoadRoles();
                        

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


        }

        private void LoadUser(ClientContext ctx, FieldUserValue[] multValue)
        {
            List userList = ctx.Web.SiteUserInfoList;
            ctx.Load(userList);

            ListItemCollection users = userList.GetItems(CamlQuery.CreateAllItemsQuery());

            ctx.Load(users, items => items.Include(
                item => item.Id, item => item["Name"]));

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
                    Dispatcher.BeginInvoke(() =>
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

                            MultiplePeopleChooser.selectedAccounts.Add(new AccountList(decodedName, dispName));
                            //MessageBox.Show(dispName);
                        }


                    }
            );



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

        #endregion


        #region E S T I M A T E D  S A V I N G S


        private void firstmonth_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (formLoad == true)
            {
                //do nothing
            }
          else  if (formLoad == false)
            {
                firstmonthTxt.Foreground = new SolidColorBrush(Colors.Black);
                int mth;
                int yr;

                if (firstmonth.SelectedDate != null)
                {

                    mth = firstmonth.SelectedDate.Value.Month;
                    yr = firstmonth.SelectedDate.Value.Year;

                    SetHeaders(mth, yr);
                  
                }
                else 
               {
                   header1.Text = "";
                   header2.Text = "";
                   header3.Text = "";
                   header4.Text = "";
                   header5.Text = "";
              }



            }
        }


        private void SetHeaders(int mth, int yr)
        {
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
                        
        private void es_ValueChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RadMaskedCurrencyInput[] controls = new RadMaskedCurrencyInput[] { es1, es2, es3, es4, es5 };

            decimal? total = 0;
            foreach (RadMaskedCurrencyInput control in controls)
            {
                if(control.Value!=null)
                    total=total+control.Value;
            }
            totalText.Text = total.Value.ToString();

            es_Total.Value = total;
            savingsTxt.Foreground = new SolidColorBrush(Colors.Black);

        }

        //<------------BEGIN REVISED FIRST MONTH

        private void revisedmonth_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int mth=0;
            int yr=0;

            if (revisedmonth.SelectedDate != null)
            {
                mth = revisedmonth.SelectedDate.Value.Month;
                yr = revisedmonth.SelectedDate.Value.Year;
            }
            else if (firstmonth.SelectedDate != null)
            {
                mth = firstmonth.SelectedDate.Value.Month;
                yr = firstmonth.SelectedDate.Value.Year;
            }

            if(mth!=0 && yr!=0)
                SetHeaders(mth, yr);
      
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
                ClientContext context = ClientContext.Current;
                //Get the Idea list and add a new item 
                Idea = context.Web.Lists.GetByTitle("Idea");


                context.Load(Idea);

                ListItem updateItem = Idea.GetItemById(ideaID.Text);

                if (projcomText.Text.Length == 0)
                {

                    projcomText.Text = (user.Title + " (" + DateTime.Now.ToString("M/d/yyyy - h:mm:ss tt") + ")" + " - " + pcomments.Text).ToString();

                    pcomments.Text = string.Empty;
                }
                else if (projcomText.Text.Length > 0)
                {
                    projcomText.Text = projcomText.Text + Environment.NewLine + (user.Title + " (" + DateTime.Now.ToString("M/d/yyyy - h:mm:ss tt") + ")" + " - " + pcomments.Text).ToString();
                    pcomments.Text = string.Empty;
                }
                updateItem["Project_x0020_Comments"] = projcomText.Text;

                updateItem.Update();
                //Load the list 
                context.Load(Idea, list => list.Title);
                //Execute the query to create the new item 
                context.ExecuteQueryAsync((s, ee) =>
                {

                },
    (s, ee) =>
    {
        Console.WriteLine(ee.Message);

    });
            }
        }


        private void btn_AdminAdd_Click(object sender, RoutedEventArgs e)
        {
            //make sure a comment was entered
            if (string.IsNullOrEmpty(acomments.Text))
            {
                MessageBox.Show("You must enter a comment before adding to Admin Comment History.", "Error", MessageBoxButton.OK);

                return;
            }
            else
            {

                ClientContext context = ClientContext.Current;
                //Get the Idea list and add a new item 
                Idea = context.Web.Lists.GetByTitle("Idea");


                context.Load(Idea);

                ListItem updateItem = Idea.GetItemById(ideaID.Text);

                if (aCommHistoryText.Text.Length == 0)
                {

                    aCommHistoryText.Text = (user.Title + " (" + DateTime.Now.ToString("M/d/yyyy - h:mm:ss tt") + ")" + " - " + acomments.Text).ToString();

                    acomments.Text = string.Empty;
                }
                else if (aCommHistoryText.Text.Length > 0)
                {
                    aCommHistoryText.Text = aCommHistoryText.Text + Environment.NewLine + (user.Title + " (" + DateTime.Now.ToString("M/d/yyyy - h:mm:ss tt") + ")" + " - " + acomments.Text).ToString();
                    acomments.Text = string.Empty;
                }

                updateItem["admin_comments"] = aCommHistoryText.Text;

                updateItem.Update();
                //Load the list 
                context.Load(Idea, list => list.Title);
                //Execute the query to create the new item 
                context.ExecuteQueryAsync((s, ee) =>
                {

                },
    (s, ee) =>
    {
        Console.WriteLine(ee.Message);

    });
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

        private void PopUpButtonAdmin_Click(object sender, RoutedEventArgs e)
        {
            myPopup_acomments.IsOpen = false;
        }
        private void imghelp_acomments_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            PopUpText_acomments.Text = "EXCEL Admins and Finance can leave comments or questions for each other. Can also be used to notate other helpful information. ";
            myPopup_acomments.IsOpen = true;
        }
        #endregion

        #region A T T A C H M E N T S

        private void MarkFilePermanent(string siteUrl, string listName, string relativePath, string folderName, FileEntry fileName)
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

                        // Set isTemp to false
                        listitem["IsTemp"] = false;
                        listitem.Update();

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

        public void DeleteTempFiles()
        {
            foreach (FileEntry fileEntry in selectedFiles)
            {
                if (fileEntry.IsTemp)
                    DeleteFile(Utils.GetSiteUrl(), libName, string.Empty, GetFolderName(), fileEntry);
            }

            foreach (FileEntry fileEntry in allFiles)
            {
                if (fileEntry.IsTemp)
                    DeleteFile(Utils.GetSiteUrl(), libName, string.Empty, GetFolderName(), fileEntry);
            }


        }

        private void MarkFilesPermanent()
        {

            foreach (FileEntry fileEntry in selectedFiles)
            {
                if (fileEntry.IsTemp)
                    MarkFilePermanent(Utils.GetSiteUrl(), libName, string.Empty, GetFolderName(), fileEntry);
            }


        }
        public void LoadFiles(string siteUrl, string listName, string relativePath, string folderName)
        {

            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                Web web = clientContext.Web;
                List list = web.Lists.GetByTitle(listName);

                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View>"
                + "<Query>"
                    //+ "<Where><Eq><FieldRef Name='FileLeafRef'/><Value Type='File'>" + fileName.FileName + "</Value></Eq></Where>"
                + "</Query>"
                + "</View>";

                if (!string.IsNullOrEmpty(folderName))
                {
                    query.FolderServerRelativeUrl = new Uri(siteUrl).AbsolutePath + "/" + listName + "/" + folderName + "/";
                }

                ListItemCollection listItems = list.GetItems(query);
                clientContext.Load(listItems);

                clientContext.ExecuteQueryAsync((s, ee) =>
                {

                    foreach (ListItem listitem in listItems)
                    {
                        clientContext.Load(listitem);

                        clientContext.ExecuteQueryAsync((ss, eee) =>
                        {


                            Dispatcher.BeginInvoke(() =>
                            {
                                // If IsTemp is not set, assume its false.
                                if (listitem["IsTemp"] == null || (bool)listitem["IsTemp"] == false)
                                {
                                    FileEntry fileEntry = new FileEntry(listitem.FieldValues["FileLeafRef"].ToString(), listitem.FieldValues["FileLeafRef"].ToString(), false);
                                    selectedFiles.Add(fileEntry);
                                    allFiles.Add(fileEntry);
                                }
                                else if ((bool)listitem["IsTemp"])
                                {
                                    allFiles.Add(new FileEntry(listitem.FieldValues["FileLeafRef"].ToString(), listitem.FieldValues["FileLeafRef"].ToString(), true));
                                }

                                if(!isReadOnly)
                                    Remove.IsEnabled = true;

                                
                            });

                        },
                      (ss, eee) =>
                      {
                          Console.WriteLine(eee.Message);

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
            fciFileToUpload.Overwrite = true;

            Microsoft.SharePoint.Client.File clFileToUpload = null;
            if (string.IsNullOrEmpty(folderName))
            {
                clFileToUpload = destinationList.RootFolder.Files.Add(fciFileToUpload);
                clFileToUpload.ListItemAllFields["IsTemp"] = true;

                clFileToUpload.ListItemAllFields.Update();

                myClContext.Load(clFileToUpload);

                myClContext.ExecuteQueryAsync((s, ee) =>
                {

                    Dispatcher.BeginInvoke(() =>
                    {
                        // File will be added as temporary file, It will be marked permanent on submit or deleted on cancel
                        //bool alreadyExist=false;


                        selectedFiles.Add(new FileEntry(fileToUpload.Name, fileToUpload.Name, true));                                               
                        attachTxt.Foreground = new SolidColorBrush(Colors.Black);
                        Remove.IsEnabled = true;
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
                            clFileToUpload.ListItemAllFields["IsTemp"] = true;
                            clFileToUpload.ListItemAllFields.Update();


                            myClContext.Load(clFileToUpload);
                            break;
                        }

                    }

                    myClContext.ExecuteQueryAsync((ss, eee) =>
                    {

                        Dispatcher.BeginInvoke(() =>
                        {
                          
                            selectedFiles.Add(new FileEntry(fileToUpload.Name, fileToUpload.Name, true));
                            attachTxt.Foreground = new SolidColorBrush(Colors.Black);
                            Remove.IsEnabled = true;
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



                ListItemCreationInformation updateItem = new ListItemCreationInformation();
                updateItem.UnderlyingObjectType = FileSystemObjectType.Folder;
                //updateItem.FolderUrl = siteUrl + listName;
                if (!relativePath.Equals(string.Empty))
                {
                    updateItem.FolderUrl += "/" + relativePath;
                }
                updateItem.LeafName = folderName;

                ListItem item = list.AddItem(updateItem);
                item["Title"] = folderName;


                item.Update();

                clientContext.Load(list);

                clientContext.ExecuteQueryAsync((s, ee) =>
                {

                    Folder newFolder = rootFolder.Folders.Add(folderName);
                },
          (s, ee) =>
          {
              Console.WriteLine(ee.Message);

          });
            }
        }

        public void RenameFolder(string siteUrl, string listName, string relativePath, string folderName, string folderNewName, string message)
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

                clientContext.ExecuteQueryAsync((s, ee) =>
                {

                    if (folders.Count == 1)
                    {

                        folders[0]["Title"] = folderNewName;
                        folders[0]["FileLeafRef"] = folderNewName;
                        folders[0].Update();
                        clientContext.ExecuteQueryAsync((ss, eee) =>
                        {
                            newFolderName = folderNewName;

                            Dispatcher.BeginInvoke(() =>
                           {
                               //newFolderName = itemId;
                               busyIndicator.IsBusy = false;
                               ShowMessage(message);


                           });


                        },
          (ss, eee) =>
          {
              busyIndicator.IsBusy = false;
              HandleSaveException(eee.Message);

          });

                    }
                    else
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            //newFolderName = itemId;
                            busyIndicator.IsBusy = false;
                            ShowMessage(message);


                        });
                    }

                },
          (s, ee) =>
          {
              busyIndicator.IsBusy = false;
              HandleSaveException(ee.Message);

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
                    query.FolderServerRelativeUrl = new Uri(siteUrl).AbsolutePath + "/"+ libName + "/" + folderName + "/";
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

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            FileEntry selFile = FileListBox.SelectedItem as FileEntry;
            if (selFile != null)
            {
                Uri fileUrl = new Uri(Utils.GetSiteUrl() + "/" + libName + "/" + itemId + "/" + selFile.FileName);


                HtmlPage.PopupWindow(fileUrl, "_blank", null);
            }

        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {

            FileEntry selFile = FileListBox.SelectedItem as FileEntry;
            if (selFile != null)
            {
                selectedFiles.Remove(selFile);
                selFile.IsTempDelete = true;

            }

        }

        private void RemoveDeletedFiles()
        {
            foreach (FileEntry fileEntry in allFiles)
            {
                if (fileEntry.IsTempDelete)
                    DeleteFile(Utils.GetSiteUrl(), libName, string.Empty, GetFolderName(), fileEntry);

            }

        }


        void UserControl_Unloaded_1(object sender, EventArgs e)
        {
            DeleteTempFiles();
            this.Close();

        }

        #endregion

        private void statusCombo_SelectionChanged(object sender, EventArgs e)
        {
            statusLevel.Text = statusCombo.SelectionBoxItem.ToString();
            NewStatus = statusCombo.SelectionBoxItem.ToString();
        }

        private static string getItem(string colName, ListItem item)
        {
            string val = item.FieldValues[colName] as string;
            if (string.IsNullOrEmpty(val))
                return string.Empty;

            return val;

        }

        private static string getDateItem(string colName, ListItem item)
        {
            try
            {
                DateTime val = (DateTime)item.FieldValues[colName];
                if (val == null)
                    return string.Empty;

                return val.ToShortDateString(); ;
            }
            catch (NullReferenceException e)
            {
                return string.Empty;
            }

        }

        private void costClarify_combo_SelectionChanged(object sender, EventArgs e)
        {
            cc.Text = costClarify_combo.SelectionBoxItem.ToString();

        }

        private void tabcontrol1_SelectionChanged(object sender, RadSelectionChangedEventArgs e)
        {

        }
   
    }
}
