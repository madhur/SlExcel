using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Microsoft.SharePoint.Client;
using Telerik.Windows.Controls;
using System.Text;

namespace excel_report
{


    public class Idea
    {

        private string getItem(string colName)
        {
            string val = _item.FieldValues[colName] as string;
            if (string.IsNullOrEmpty(val))
                return string.Empty;

            return val;

        }

        private DateTime getDateItem(string colName)
        {
            DateTime dateVal;

            try
            {
                dateVal = (DateTime)_item[colName];
            }
            catch (FormatException e)
            {
                dateVal = Convert.ToDateTime(null);
            }
            catch (NullReferenceException)
            {
                dateVal = Convert.ToDateTime(null);
            }

            return dateVal;
            
        }

        private double getDoubleItem(string colName)
        {
            double doubleVal;

            try
            {
                doubleVal = (double)_item[colName];
            }
            catch (FormatException e)
            {
                doubleVal = Convert.ToDouble(null);
            }
            catch (NullReferenceException)
            {
                doubleVal = Convert.ToDouble(null);
            }
            catch (InvalidCastException)
            {
                doubleVal = double.Parse(_item[colName].ToString());
            }

            return doubleVal;
        }

        private decimal getDecimalItem(string colName)
        {
            decimal decimalVal;

            try
            {
                decimalVal = (decimal)_item[colName];
            }
            catch (FormatException e)
            {
                decimalVal = Convert.ToDecimal(null);
            }
            catch (NullReferenceException)
            {
                decimalVal = Convert.ToDecimal(null);
            }
            catch (InvalidCastException)
            {
                decimalVal = decimal.Parse(_item[colName].ToString());
            }

            return decimalVal;
        }

        private int getintItem(string colName)
        {
            int intVal;

            try
            {
                intVal = (int)_item[colName];
            }
            catch (FormatException e)
            {
                intVal = Convert.ToInt32(null);
            }

            return intVal;
        }



        ClientContext context = ClientContext.Current;



        private ListItem _item;

        public Idea(ListItem item) { _item = item; }

        public double ideaID { get { return getintItem("ID"); } }
        public string ideaName
        {
            get
            {
                return getItem("Idea_x0020_Name");

            }
        }

        public string ideaStatus
        {
            get
            {
                return getItem("Idea_x0020_Status");

            }
        }

        public string costType { get { return getItem("Cost_x0020_Type1"); } }
        public double totalSave { get { return getDoubleItem("Total_x0020_Savings"); } }
        public DateTime firstMonth { get { return getDateItem("_x0031_st_x0020_Mo_x0020_Saves_x"); } }
        public DateTime revisedMonth { get { return getDateItem("Revised_x0020_1st_x0020_Mo_x0020"); } }

        public string Executor
        {
            get
            {
                FieldUserValue val = _item.FieldValues["Executor"] as FieldUserValue;
                if (val != null)
                    return val.LookupValue;

                return string.Empty;
            }
        }

        public string Director
        {
            get
            {
                FieldUserValue val = _item.FieldValues["Director"] as FieldUserValue;
                if (val != null)
                    return val.LookupValue;

                return string.Empty;
            }
        }

        public string VP
        {
            get
            {
                FieldUserValue val = _item.FieldValues["VP"] as FieldUserValue;
                if (val != null)
                    return val.LookupValue;
                return string.Empty;
            }
        }

        public string CreatedBy
        {
            get
            {
                FieldUserValue val = _item.FieldValues["Author"] as FieldUserValue;
                if (val != null)
                    return val.LookupValue;
                return string.Empty;
            }
        }

        public string LOBT1 { get { return getItem("Line_x0020_Of_x0020_Business_x001"); } }
        public string LOBT2 { get { return getItem("LOB_Tier2"); } }

        public string AppName { get { return (getItem("AIM_x0020_Application_x0020_Name")); } }
        public decimal AppID { get { return getDecimalItem("AIM_x0020_Application_x0020_ID"); } }
        public string AssumpDepend { get { return (getItem("Assumptions_x0020_or_x0020_Depen")); } }

        public string Description { get { return (getItem("EXCEL_x0020_Idea_x0020_Descripti")); } }
        public string Identify { get { return (getItem("EXCEL_x0020_Identifier")); } }
        public string FTE
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                FieldUserValue[] vals = _item.FieldValues["FTE_x0020_Contributors"] as FieldUserValue[];
                if (vals != null)
                {
                    foreach (FieldUserValue val in vals)
                    {
                        sb.Append(val.LookupValue + ';');
                    }
                }

                return sb.ToString();
            }
        }

        public string Risk { get { return (getItem("Risk_x0020_of_x0020_Implementati")); } }

        public string SH1 { get { return getItem("SavingsHeader1"); } }
        public string SH2 { get { return getItem("SavingsHeader2"); } }
        public string SH3 { get { return getItem("SavingsHeader3"); } }
        public string SH4 { get { return getItem("SavingsHeader4"); } }
        public string SH5 { get { return getItem("SavingsHeader5"); } }

        public double S1 { get { return getDoubleItem("Savings1"); } }
        public double S2 { get { return getDoubleItem("Savings2"); } }
        public double S3 { get { return getDoubleItem("Savings3"); } }
        public double S4 { get { return getDoubleItem("Savings4"); } }
        public double S5 { get { return getDoubleItem("Savings5"); } }
        public string TechImpact { get { return (getItem("Tech_Impact")); } }
        public string Audit { get { return (getItem("Audit")); } }
        public string ProjComments { get { return (getItem("Project_x0020_Comments")); } }





    }
}
