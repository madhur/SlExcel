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


namespace manage.Controls
{
      public partial class close

    {

        #region Event Handler

        public event EventHandler SubmitClicked;
        
        #endregion

        public close()
        {
            InitializeComponent();
        }

        private void Yesbtn_Click(object sender, RoutedEventArgs e)
        {
           
            if (SubmitClicked != null)
            {

                SubmitClicked(this, new EventArgs());

            }
            this.DialogResult = true;
            
            
        }

        private void Nobtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

