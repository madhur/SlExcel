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


namespace manage.Controls
{
    public partial class ProjComments : ChildWindow
    {
        Web oWebsite;
        ListCollection collList;
        IEnumerable<List> listInfo;
        User user;


        #region Event Handler

        public event EventHandler pc_SubmitClicked;


        #endregion


    
            
        public ProjComments()
        {

            InitializeComponent();
        }
         
        
        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            SilverlightOM();
        }

        public void SilverlightOM()
        {
            ClientContext clientContext = new ClientContext("https://teams.aexp.com/sites/excel");

            oWebsite = clientContext.Web;
            collList = oWebsite.Lists;

            clientContext.Load(oWebsite, s => s.CurrentUser);
            clientContext.ExecuteQueryAsync(onQuerySucceeded, onQueryFailed);
        }

        private void onQuerySucceeded(object sender, ClientRequestSucceededEventArgs args)
        {
            user = oWebsite.CurrentUser;
            UpdateUIMethod updateUI = DisplayInfo;
            this.Dispatcher.BeginInvoke(updateUI);

        }

        private void onQueryFailed(object sender, ClientRequestFailedEventArgs args)
        {
            MessageBox.Show("Request failed");
        }


        private void DisplayInfo()
       {

           chatlist.Items.Add(user.Title + " " + "(" + DateTime.Now + ")" + " " + pcomments.Text);

          
          
          pcomments.Text = string.Empty;
            
        }

        private delegate void UpdateUIMethod();
        

        private void PopUpButton_Click(object sender, RoutedEventArgs e)
        {
            myPopup_comments.IsOpen = false;
        }
        private void imghelp_comments_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            PopUpText_comments.Text = "Please provide any comments or questions for the EXCEL Admins ";
            myPopup_comments.IsOpen = true;
        }

        private void submit_btn_Click(object sender, RoutedEventArgs e)
        {

            if (pc_SubmitClicked != null)
            {

                pc_SubmitClicked(this, new EventArgs());

            }
            this.DialogResult = true;


        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }



     
            }
}

