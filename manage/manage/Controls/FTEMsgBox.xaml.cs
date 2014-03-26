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
    public partial class FTEMsgBox
    {
        #region Event Handler

        public event EventHandler NoClicked;
        public event EventHandler YesClicked;

        #endregion

        public FTEMsgBox()
        {
            InitializeComponent();
           
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (YesClicked != null)
            {

                YesClicked(this, new EventArgs());

            }
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (NoClicked != null)
            {

                NoClicked(this, new EventArgs());

            }
            this.DialogResult = true;
        }
    }
}

