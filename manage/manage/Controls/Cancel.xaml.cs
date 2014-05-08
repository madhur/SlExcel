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

namespace manage.Controls
{
    public partial class Cancel : ChildWindow
    {
        public event EventHandler cancelSaveClicked;

        public Cancel()
        {
            InitializeComponent();
            
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
                if (cancelSaveClicked != null)
                {
                    cancelSaveClicked(this, new EventArgs());
                }

            
            this.DialogResult = true;
        }

       

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
                     
            this.DialogResult = false;
        }

        private void cancelComments_TextChanged(object sender, TextChangedEventArgs e)
        {
            SubmitButton.IsEnabled = false;

            if (cancelComments.Text.Length > 0)
            {
                SubmitButton.IsEnabled = true;
            }
            
            
        }
    }
}

