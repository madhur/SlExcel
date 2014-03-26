
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using excel_create.PeopleWS;




namespace excel_create.Controls
{
    public partial class PeopleChooser : UserControl
    {

        //  MenuItem mnuItem;
        public SelectedAccounts selectedAccounts;
        public bool AllowMultiple { get; set; }
        PPLPicker peoplePicker;
        Dictionary<String, PickerEntry> values;
        //    AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        public PeopleChooser()
        {

            this.Loaded += PeopleChooser_Loaded;
            InitializeComponent();
            peoplePicker = new PPLPicker();

            peoplePicker.SubmitClicked += peoplePicker_SubmitClicked;
            selectedAccounts = new SelectedAccounts();


        }
        void PeopleChooser_Loaded(object sender, RoutedEventArgs e)
        {
            if (AllowMultiple)
            {
                UsersListBox.Visibility = System.Windows.Visibility.Visible;
                UserTextBox.Visibility = System.Windows.Visibility.Collapsed;
                ResolveButton.Visibility = Visibility.Collapsed;

                UsersListBox.DataContext = selectedAccounts;
                UsersListBox.ItemsSource = selectedAccounts;

            }
            else
            {
                UsersListBox.Visibility = System.Windows.Visibility.Collapsed;
                UserTextBox.Visibility = System.Windows.Visibility.Visible;
                ResolveButton.Visibility = Visibility.Visible;


            }

            peoplePicker.AllowMultiple = AllowMultiple;

        }




        void peoplePicker_SubmitClicked(object sender, EventArgs e)
        {
            selectedAccounts.Clear();

            foreach (AccountList ac in peoplePicker.selectedAccounts)
            {
                selectedAccounts.Add(new AccountList(ac.AccountName, ac.DisplayName));
            }

            if (!AllowMultiple && selectedAccounts.Count > 0)
            {
                UserTextBox.Text = selectedAccounts[0].DisplayName;
                UserTextBox.FontStyle = FontStyles.Italic;


            }

        }

        private void ResolveButton_Click(object sender, RoutedEventArgs e)
        {
            //    autoResetEvent.Reset();
            StartResolve();

        }


        private void StartResolve()
        {
            if (string.IsNullOrEmpty(UserTextBox.Text))
            {
                MessageBox.Show("You must enter a search term.", "Missing Search Term",
                    MessageBoxButton.OK);
                UserTextBox.Focus();
                return;
            }
            try
            {
                this.Cursor = Cursors.Wait;
                PeopleSoapClient ps = new PeopleSoapClient();
                ps.Endpoint.Address =
               new System.ServiceModel.EndpointAddress("https://teams.aexp.com/sites/excel" + "/_vti_bin/People.asmx");

                ps.SearchPrincipalsCompleted += new EventHandler<SearchPrincipalsCompletedEventArgs>(ps_SearchPrincipalsCompleted);
                ps.SearchPrincipalsAsync(UserTextBox.Text, 50, SPPrincipalType.User);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem executing the search; please try again " +
                     "later.", "Search Error",
                    MessageBoxButton.OK);

                this.Cursor = Cursors.Arrow;
            }

        }


        private void HandleResult(ObservableCollection<PrincipalInfo> results)
        {


            if (results.Count == 0)
            {
                nomatch.Visibility = Visibility.Visible;
                UserTextBox.BorderBrush = new SolidColorBrush(Colors.Red);

            }
            else if (results.Count > 1)
            {
                values = new Dictionary<string, PickerEntry>();

                foreach (PrincipalInfo pi in results)
                {
                    String decodedAccount = Utils.checkClaimsUser(pi.AccountName);
                    if (!values.ContainsKey(decodedAccount))
                        values.Add(decodedAccount, new PickerEntry(pi.DisplayName, decodedAccount, pi.Email, pi.Department));
                }

                if (values.Count == 1)
                {
                    SetSingleResult(values);
                    UserTextBox.FontStyle = FontStyles.Italic;
                    nomatch.Visibility = Visibility.Collapsed;
                    UserTextBox.BorderBrush = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    nomatch.Visibility = Visibility.Visible;
                    UserTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                }

            }
            else if (results.Count == 1)
            {
                values = new Dictionary<string, PickerEntry>();

                foreach (PrincipalInfo pi in results)
                {
                    String decodedAccount = Utils.checkClaimsUser(pi.AccountName);
                    if (!values.ContainsKey(decodedAccount))
                        values.Add(decodedAccount, new PickerEntry(pi.DisplayName, decodedAccount, pi.Email, pi.Department));
                }


                SetSingleResult(values);
                UserTextBox.FontStyle = FontStyles.Italic;
                nomatch.Visibility = Visibility.Collapsed;
                UserTextBox.BorderBrush = new SolidColorBrush(Colors.Black);
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

                    HandleResult(results);
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
                //autoResetEvent.Set();
            }
        }

        private void SetSingleResult(Dictionary<String, PickerEntry> values)
        {
            PickerEntry pi = values.Values.ToArray<PickerEntry>()[0];

            UserTextBox.Text = pi.DisplayName;
            UserTextBox.FontStyle = FontStyles.Italic;
            UserTextBox.Foreground = new SolidColorBrush(Colors.Black);
            UserTextBox.BorderBrush = new SolidColorBrush(Colors.Black);
            selectedAccounts.Clear();
            selectedAccounts.Add(new AccountList(pi.AccountName, pi.DisplayName));


        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            peoplePicker.Show();

            if (selectedAccounts.Count > 0)
            {
                peoplePicker.selectedAccounts.Clear();

                foreach (AccountList ac in selectedAccounts)
                {

                    peoplePicker.selectedAccounts.Add(new AccountList(ac.AccountName, ac.DisplayName));

                }

            }
        }

        //private void ShowModalDialog()
        //{
        //    AutoResetEvent waitHandle = new AutoResetEvent(false);
        //    Dispatcher.BeginInvoke(() =>
        //    {
        //        ChildWindow cw = new ChildWindow();
        //        cw.Content = "Modal Dialog";
        //        cw.Closed += (s, e) => waitHandle.Set();
        //        cw.Show();
        //    });
        //    waitHandle.WaitOne();
        //}

        //private void UserTextBox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (values != null)
        //    {
        //        if (values.Count > 1)
        //            e.Handled = true;
        //        else
        //            e.Handled = false;
        //    }
        //    else
        //        e.Handled = false;
        //}

        //private void UserTextBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    Telerik.Windows.Controls.RadContextMenu cmnu = new Telerik.Windows.Controls.RadContextMenu();
        //    cmnu.MaxHeight = 500;
        //    cmnu.HorizontalOffset = 400;
        //    cmnu.Placement = Telerik.Windows.Controls.PlacementMode.MousePoint;

        //    foreach (PickerEntry pi in values.Values)
        //    {

        //        mnuItem = new MenuItem();

        //        mnuItem.Header = pi.DisplayName;
        //        mnuItem.Click += mnuItem_Click;

        //        cmnu.Items.Add(mnuItem);
        //    }

        //    cmnu.IsOpen = true;


        //}
        //void mnuItem_Click(object sender, RoutedEventArgs e)
        //{
        //    MenuItem mnu = sender as MenuItem;

        //    UserTextBox.Text = mnu.Header.ToString();
        //    UserTextBox.FontStyle = FontStyles.Italic;
        //    UserTextBox.Foreground = new SolidColorBrush(Colors.Black);
        //    UserTextBox.BorderBrush = new SolidColorBrush(Colors.Black);
        //    nomatch.Visibility = Visibility.Collapsed;


        //}

        private void UserTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (UserTextBox.Text.Length > 0)
            {
                try
                {
                    PeopleSoapClient ps = new PeopleSoapClient();
                    //use the host name property to configure the request against the site in 
                    //which the control is hosted
                    ps.Endpoint.Address =
                   new System.ServiceModel.EndpointAddress("https://teams.aexp.com/sites/excel" + "/_vti_bin/People.asmx");



                    //create the handler for when the call completes
                    ps.SearchPrincipalsCompleted += new EventHandler<SearchPrincipalsCompletedEventArgs>(ps_SearchPrincipalsCompleted);
                    //execute the search
                    ps.SearchPrincipalsAsync(UserTextBox.Text, 50, SPPrincipalType.User);
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
        }

        private void UserTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UserTextBox.Text.Length == 0)
            {
                UserTextBox.FontStyle = FontStyles.Normal;

            }
        }



    }
}
