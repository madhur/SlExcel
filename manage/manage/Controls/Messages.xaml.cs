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
    public partial class Messages
    {
        #region Event Handler

        public event EventHandler RequiredOKClicked;


        #endregion
        EditForm editForm;

        public Messages(EditForm editForm)
        {
            InitializeComponent();
            this.editForm = editForm;


        }

        private void SubmitOKButton_Click(object sender, RoutedEventArgs e)
        {
          
            this.DialogResult = true;
            if (editForm != null)
                editForm.Close();

          
        }

        private void RequiredOKButton_Click(object sender, RoutedEventArgs e)
        {
            if (RequiredOKClicked != null)
            {

                RequiredOKClicked(this, new EventArgs());

            }
            this.DialogResult = true;

        }

    }
}

