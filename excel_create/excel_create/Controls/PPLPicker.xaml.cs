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
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Windows.Browser;
using System.Threading;
using Microsoft.SharePoint.Client;
using excel_create;
using excel_create.PeopleWS;
using excel_create.Controls;
using System.Text;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Common;


namespace excel_create.Controls
{
    public partial class PPLPicker : ChildWindow
    {

        #region Event Handler

        public event EventHandler SubmitClicked;



        #endregion

        public string HostName { get; set; }
        public string SelectedAccountName { get; set; }
        public MainPage HostControl { get; set; }
        
        public SelectedAccounts selectedAccounts;

        public bool AllowMultiple { get; set; }

        public enum AddressType
        {
            executor,
            director,
            vp,
            fte_contribute

        }


        public AddressType PickerAddressType { get; set; } 


        public PPLPicker()
        {
            InitializeComponent();

            selectedAccounts = new SelectedAccounts();

            Binding binding = new Binding()
            {
                Source = selectedAccounts,
                Path = new PropertyPath("DisplayName"),
                Mode = BindingMode.TwoWay
            };

            AccountListBox.DataContext = selectedAccounts;
            AccountListBox.ItemsSource = selectedAccounts;

        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {

            //plug in the values
            if (SubmitClicked != null)
            {

                SubmitClicked(this, new EventArgs());

            }

            this.DialogResult = true;
        }

        private string GetDisplayNames(ObservableCollection<AccountList> accountList)
        {
            StringBuilder dispString = new StringBuilder();

            foreach (AccountList account in accountList)
            {
                dispString = dispString.Append(account.DisplayName + ";");
            }

            return dispString.ToString();

        }

        private string GetDisplayAccounts(ObservableCollection<AccountList> accountList)
        {
            StringBuilder dispString = new StringBuilder();

            foreach (AccountList account in accountList)
            {
                dispString = dispString.Append(account.AccountName + ";");
                
               
            }

            return dispString.ToString();

        }

        private void CloseDialog()
        {
            //clear out selections for next time
            SearchTxt.Text = string.Empty;
            ResultsLst.Items.Clear();
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            //make sure a search value was entered
            if (string.IsNullOrEmpty(SearchTxt.Text))
            {
                MessageBox.Show("You must enter a search term.", "Missing Search Term",
                    MessageBoxButton.OK);
                SearchTxt.Focus();
                return;
            }
            try
            {
                //change the cursor to hourglass
                this.Cursor = Cursors.Wait;


                PeopleSoapClient ps = new PeopleSoapClient();
                //use the host name property to configure the request against the site in 
                //which the control is hosted
                ps.Endpoint.Address =
               new System.ServiceModel.EndpointAddress(Utils.GetSiteUrl() + "/_vti_bin/People.asmx");



                //create the handler for when the call completes
                ps.SearchPrincipalsCompleted += new EventHandler<SearchPrincipalsCompletedEventArgs>(ps_SearchPrincipalsCompleted);
                //execute the search
                ps.SearchPrincipalsAsync(SearchTxt.Text, 50, SPPrincipalType.User);
            }
            catch (Exception ex)
            {
                //ERROR LOGGING HERE
                Debug.WriteLine(ex.Message);

                MessageBox.Show("There was a problem executing the search; please try again " +
                     "later.", "Search Error",
                    MessageBoxButton.OK);
                //reset cursor
                this.Cursor = Cursors.Arrow;
            }
        }
        void ps_SearchPrincipalsCompleted(object sender, SearchPrincipalsCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                    MessageBox.Show("An error was returned: " + e.Error.Message, "Search Error",
                       MessageBoxButton.OK);
                else
                {
                    System.Collections.ObjectModel.ObservableCollection<PrincipalInfo>
                        results = e.Result;
                    //clear the search results listbox
                    System.Collections.Generic.Dictionary<String, PickerEntry> values = new Dictionary<string, PickerEntry>();

                    foreach (PrincipalInfo pi in results)
                    {
                        String decodedAccount = Utils.checkClaimsUser(pi.AccountName);
                        if (!values.ContainsKey(decodedAccount))
                            values.Add(decodedAccount, new PickerEntry(pi.DisplayName, decodedAccount, pi.Email, pi.Department));
                    }

                    ResultsLst.Items.Clear();

                    foreach (PickerEntry pi in values.Values)
                    {
                        ResultsLst.Items.Add(new PickerEntry(pi.DisplayName, pi.AccountName, pi.Email, pi.Department));


                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error processing the search results: " + ex.Message,
                   "Search Error", MessageBoxButton.OK);
            }
            finally
            {
                //reset cursor
                this.Cursor = Cursors.Arrow;
            }
        }



        private void AddNameBtn_Click(object sender, RoutedEventArgs e)
        {
            //see if an item is selected
            if ((ResultsLst.Items.Count == 0) || (ResultsLst.SelectedItem == null))
            {
                MessageBox.Show("You must run a search and select a name first.",
                   "Add User Error", MessageBoxButton.OK);
                return;
            }

            AddPickerEntry();
        }

        private void AddPickerEntry()
        {

            //cast the selected name as a PickerEntry
            PickerEntry pe = (PickerEntry)ResultsLst.SelectedItem;
            // UserNameTxt.Text = pe.DisplayName;
            SelectedAccountName = pe.AccountName;

            bool contains = selectedAccounts.Any(p => p.AccountName.Equals(SelectedAccountName));

            if (!contains)
            {
                if (AllowMultiple)
                {

                    selectedAccounts.Add(new AccountList(pe.AccountName, pe.DisplayName));
                }
                else
                {
                    selectedAccounts.Clear();
                    selectedAccounts.Add(new AccountList(pe.AccountName, pe.DisplayName));

                }
            }

        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }


        private void SearchTxt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SearchBtn_Click(sender, new RoutedEventArgs());
        }



        public string Email { get; set; }

        public string Department { get; set; }

        private void RemoveAccountButton_click(object sender, RoutedEventArgs e)
        {
            if (this.AccountListBox.SelectedIndex >= 0)
            {
                AccountList account = AccountListBox.SelectedItem as AccountList;
                bool removed = selectedAccounts.Remove(account);

            }

        }
    }


}

