using System;
using System.Net;
using System.Text;
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
using Common;

namespace manage
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
            catch (InvalidCastException)
            {
                dateVal = Convert.ToDateTime(null);
            }

            return dateVal;

        }



        private String getImageUrl(string colName)
        {
            string siteUrl = Utils.GetSiteUrl()+"/SiteAssets/Images/{0}";
            double value = getDoubleItem(colName);
            if (value == double.Parse("0.0"))
                return string.Empty;

            int intVal = Convert.ToInt32(value.ToString());

            switch (intVal)
            {
                case 1:
                    return string.Format(siteUrl, "draft60day.png");
                case 2:
                    return string.Format(siteUrl, "inprogress30day.png");

                case 3:
                    return string.Format(siteUrl, "inprogresspastdue.png");

                case 4:
                    return string.Format(siteUrl, "readyforfinancereview.png");

                case 5:
                    return string.Format(siteUrl, "readyforfinancereview14day.png");

                case 6:
                    return string.Format(siteUrl, "canceled.png");

                case 7:
                    return string.Format(siteUrl, "approved.png");

                case 8:
                    return string.Format(siteUrl, "Future30day.png");

                case 9:
                    return string.Format(siteUrl, "FuturePastDue.png");

                default:
                    return string.Empty;

            }


        }


        private String getAlertTip(string colName)
        {

            double value = getDoubleItem(colName);
            if (value == double.Parse("0.0"))
                return string.Empty;

            int intVal = Convert.ToInt32(value.ToString());

            switch (intVal)
            {
                case 1:
                    return Consts.VAL_1;
                case 2:
                    return Consts.VAL_2;

                case 3:
                    return Consts.VAL_3;

                case 4:
                    return Consts.VAL_4;

                case 5:
                    return Consts.VAL_5;

                case 6:
                    return Consts.VAL_6;

                case 7:
                    return Consts.VAL_7;

                case 8:
                    return Consts.VAL_8;

                case 9:
                    return Consts.VAL_9;

                default:
                    return string.Empty;

            }


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
                doubleVal = Convert.ToDouble(null);
            }

            return doubleVal;
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
            catch (InvalidCastException e)
            {
                intVal = Convert.ToInt32(null);
            }

            return intVal;
        }


        bool isFinance = false;

        ClientContext context = ClientContext.Current;



        private ListItem _item;

        public Idea(ListItem item) { _item = item; }

        public Idea(ListItem item, bool isFinance)
        {
            _item = item;
            this.isFinance = isFinance;
        }



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


        public string Alert
        {
            get
            {
                if (!isFinance)
                    return getImageUrl("UserAlert2");
                else
                    return getImageUrl("FinanceAlert2");
            }
        }

        public string AlertTip
        {
            get
            {
                if (!isFinance)
                    return getAlertTip("UserAlert2");
                else
                    return getAlertTip("FinanceAlert2");
            }
        }

        public string LOBT1 { get { return getItem("Line_x0020_Of_x0020_Business_x001"); } }
        public string LOBT2 { get { return getItem("LOB_Tier2"); } }

        public string AppName { get { return (getItem("AIM_x0020_Application_x0020_Name")); } }
        public double AppID { get { return getDoubleItem("AIM_x0020_Application_x0020_ID"); } }
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
