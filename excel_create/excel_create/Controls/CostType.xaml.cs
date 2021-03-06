﻿using System;
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



namespace excel_create.Controls
{
    public partial class ChildWindow1

    {

        #region Event Handler

        public event EventHandler SubmitClicked;
        
        #endregion

        public ChildWindow1()
        {
            InitializeComponent();
        }

        #region Buttons

 
        private void btn_back2_Click(object sender, RoutedEventArgs e)
        {
            tabcontrol1.SelectedIndex = 0;
            type_4.Visibility = Visibility.Collapsed;
            type_5.Visibility = Visibility.Collapsed;
            type_7.Visibility = Visibility.Collapsed;
            type_9.Visibility = Visibility.Collapsed;
            type_11.Visibility = Visibility.Collapsed;
            type_13.Visibility = Visibility.Collapsed;
            type_16.Visibility = Visibility.Collapsed;
            type_20.Visibility = Visibility.Collapsed;

                typeQ1_no.IsChecked = null;
                typeQ1_yes.IsChecked = null;
                typeQ2_1.IsChecked = null;
                typeQ2_2.IsChecked = null;
                typeQ2_3.IsChecked = null;
                typeQ2_4.IsChecked = null;
                typeQ3_biz.IsChecked = null;
                typeQ3_sqp.IsChecked = null;
                typeQ3_ti.IsChecked = null;
                typeTI_1.IsChecked = null;
                typeTI_1.IsChecked = null;
                typeTI_1.IsChecked = null;
                typeTI_1.IsChecked = null;
                typeTI_1.IsChecked = null;
               
                type11_no.IsChecked = null;
                type11_yes.IsChecked = null;
               
                type13_1.IsChecked = null;
                type13_2.IsChecked = null;
                type16_no.IsChecked = null;
                type16_yes.IsChecked = null;
                type20_1.IsChecked = null;
                type20_2.IsChecked = null;
                type5_1.IsChecked = null;
                type5_2.IsChecked = null;
                type5_3.IsChecked = null;
                type7_no.IsChecked = null;
                type7_yes.IsChecked = null;
                type9_no.IsChecked = null;
                type9_yes.IsChecked = null;
        }


        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            if (SubmitClicked != null)
            {

                SubmitClicked(this, new EventArgs());

            }

            this.DialogResult = true;
        }


        #endregion


        #region Cost Type Query

        private void typeQ1_no_Checked(object sender, RoutedEventArgs e)
        {
            tabcontrol1.SelectedIndex = 2;
            answersTxt.Visibility = Visibility.Collapsed;
            cost_label.Text = "May not be an EXCEL Idea - Save as Draft and Consult with your EXCEL Admin.";


        }

        private void typeQ1_yes_Checked(object sender, RoutedEventArgs e)
        {
            typeQ2_1.IsEnabled = true;
            typeQ2_2.IsEnabled = true;
            typeQ2_3.IsEnabled = true;
            typeQ2_4.IsEnabled = true;
        }

        private void Cost_Avoidance(object sender, RoutedEventArgs e)
        {
            if (typeQ1_yes.IsChecked == true)
            {
                tabcontrol1.SelectedIndex = 2;
                cost_label.Text = "Cost Avoidance";
                result.Text = "1";

            }
        }

        private void typeQ2_4_Checked(object sender, RoutedEventArgs e)
        {
            if (typeQ2_4.IsChecked == true)
            {
                tabcontrol1.SelectedIndex = 2;
                cost_label.Text = "Cost Avoidance";
                result.Text = "1";

            }
        }

        private void typeQ2_34_Checked(object senver, RoutedEventArgs e)
        {

            typeQ3_biz.IsEnabled = true;
            typeQ3_sqp.IsEnabled = true;
            typeQ3_ti.IsEnabled = true;

        }

        private void REE(object sender, RoutedEventArgs e)
        {
            tabcontrol1.SelectedIndex = 2;
            cost_label.Text = "Re-Engineering (REE)";
            cost_txt.Text = "Validate against the MF HLQ report and do an Add Mainframe on CEM REENGINEERING TRACKER.";
            result.Text = "2";
        }



        private void typeQ3_ti_Checked(object sender, RoutedEventArgs e)
        {
            if (typeQ2_3.IsChecked == true)
            {
                tabcontrol1.SelectedIndex = 1;
                type_4.Visibility = Visibility.Visible;
            }
            else if (typeQ2_2.IsChecked == true)
            {
                tabcontrol1.SelectedIndex = 1;
                type_5.Visibility = Visibility.Visible;
            }
        }

        private void typeQ3_sqp_Checked(object sender, RoutedEventArgs e)
        {

            tabcontrol1.SelectedIndex = 1;
            type_7.Visibility = Visibility.Visible;

        }

        private void typeQ3_biz_Checked(object sender, RoutedEventArgs e)
        {

            tabcontrol1.SelectedIndex = 1;
            type_9.Visibility = Visibility.Visible;

        }

        private void typeTI_1_Checked(object sender, RoutedEventArgs e)
        {
            type_4.Visibility = Visibility.Collapsed;
            type_20.Visibility = Visibility.Visible;


        }

        private void typeTI_2_Checked(object sender, RoutedEventArgs e)
        {

            tabcontrol1.SelectedIndex = 2;
            cost_label.Text = "Re-Engineering (REE)";
            cost_txt.Text = "Validate against MF HLQ report and do an Add Mainframe on CEM REENGINEERING TRACKER.";
            result.Text = "2";

        }

        private void typeTI_3_Checked(object sender, RoutedEventArgs e)
        {

            type_5.Visibility = Visibility.Visible;
            type_4.Visibility = Visibility.Collapsed;

        }



        private void typeTI_4_Checked(object sender, RoutedEventArgs e)
        {

            tabcontrol1.SelectedIndex = 2;
            cost_label.Text = "Re-Engineering (REE)";
            cost_txt.Text = "Validate against the Direct Tech Forecast and do an Add Direct Tech on CEM REENGINEERING TRACKER.";
            result.Text = "2";

        }
        private void typeTI_5_Checked(object sender, RoutedEventArgs e)
        {

            tabcontrol1.SelectedIndex = 2;
            cost_label.Text = "Re-Engineering (REE)";
            cost_txt.Text = "Do an Add other REE on CEM REEGINEERING TRACKER.";
            result.Text = "2";

        }

        private void type7_yes_Checked(object sender, RoutedEventArgs e)
        {

            tabcontrol1.SelectedIndex = 2;
            cost_label.Text = "Cost Reduction";
            result.Text = "3";

        }

        private void type7_no_Checked(object sender, RoutedEventArgs e)
        {

            tabcontrol1.SelectedIndex = 2;
            cost_label.Text = "Cost Avoidance";
            result.Text = "1";

        }

        private void type9_yes_Checked(object sender, RoutedEventArgs e)
        {

            tabcontrol1.SelectedIndex = 2;
            cost_label.Text = "Cost Reduction";
            result.Text = "3";

        }

        private void type9_no_Checked(object sender, RoutedEventArgs e)
        {

            tabcontrol1.SelectedIndex = 2;
            cost_label.Text = "Cost Avoidance";
            result.Text = "1";

        }

        private void type5_1_Checked(object sender, RoutedEventArgs e)
        {

            tabcontrol1.SelectedIndex = 2;
            cost_label.Text = "Re-Engineering (REE)";
            cost_txt.Text = "Validated against CEM REE report as entered automatically.";
            result.Text = "2";

        }

        private void type5_2_Checked(object sender, RoutedEventArgs e)
        {

            tabcontrol1.SelectedIndex = 2;
            cost_label.Text = "Cost Avoidance";
            result.Text = "1";

        }

        private void type5_3_Checked(object sender, RoutedEventArgs e)
        {

            type_5.Visibility = Visibility.Collapsed;
            type_13.Visibility = Visibility.Visible;

        }

        private void type20_1_Checked(object sender, RoutedEventArgs e)
        {

            type_20.Visibility = Visibility.Collapsed;
            type_11.Visibility = Visibility.Visible;

        }

        private void type20_2_Checked(object sender, RoutedEventArgs e)
        {
            type_13.Visibility = Visibility.Visible;
            type_20.Visibility = Visibility.Collapsed;

        }

        private void type13_1_Checked(object sender, RoutedEventArgs e)
        {
            type_13.Visibility = Visibility.Collapsed;
            type_16.Visibility = Visibility.Visible;

        }

        private void type13_2_Checked(object sender, RoutedEventArgs e)
        {

            tabcontrol1.SelectedIndex = 2;
            cost_label.Text = "Re-Engineering (REE)";
            cost_txt.Text = "Validate against the MR report and do an Add Midrange Tape on the CEM REENGINEERING TRACKER.";
            result.Text = "2";
        }




        private void type11_no_Checked(object sender, RoutedEventArgs e)
        {

            tabcontrol1.SelectedIndex = 2;
            answersTxt.Visibility = Visibility.Collapsed;
            cost_label.Text = "May not be an EXCEL Idea - Save as Draft and Consult with your EXCEL Admin.";

        }

        private void type11_yes_Checked(object sender, RoutedEventArgs e)
        {

            tabcontrol1.SelectedIndex = 2;
            cost_label.Text = "Growth Reduction";
            cost_txt.Text = "Attach impacted TI forecast information.";
            result.Text = "4";

        }




        private void type16_yes_Checked(object sender, RoutedEventArgs e)
        {

            tabcontrol1.SelectedIndex = 2;
            cost_label.Text = "Re-Engineering (REE)";
            cost_txt.Text = "Validate against the MR report and do an Add Rate Change on CEM REENGINEERING TRACKER.";
            result.Text = "2";

        }


        private void type16_no_Checked(object sender, RoutedEventArgs e)
        {
            tabcontrol1.SelectedIndex = 2;
            cost_label.Text = "Re-Engineering (REE)";
            cost_txt.Text = "Validated against CEM REE report as entered automatically.";
            result.Text = "2";

        }






        #endregion


    }
}

