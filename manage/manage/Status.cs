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

namespace manage
{
    public enum TAB
    {
        MY_IDEAS, TEAM_IDEAS, ADMIN, FINANCE,

        OVERVIEW,
        SCOPE,
        FINANCIAL

    };

    public class Status
    {

        public const String APPROVED = "Approved";
        public const String IN_PROGRESS = "In Progress";
        public const String FINANCE_REVIEW = "Finance Review Completed";
        public const String READY_FINANCE_REVIEW = "Ready for Finance Review";
        public const String SUBMIT_APPROVAL = "Submit for Approval";
        public const String CANCELLED = "Canceled";
        public const String FUTURE_PIPELINE = "Future Pipeline";
        public const String DRAFT = "Draft";
        public const String PENDING_ACTUALS = "Pending Actuals";

        public static String[] Active = new String[] { IN_PROGRESS, FINANCE_REVIEW, READY_FINANCE_REVIEW, SUBMIT_APPROVAL, FUTURE_PIPELINE };
        public static String[] Draft = new String[] { DRAFT };
        public static String[] Admin = new String[] { FINANCE_REVIEW, SUBMIT_APPROVAL, PENDING_ACTUALS};

        public const String STATUS_COLUMN = "ideaStatus";

    }


    public class Consts
    {
        public const String FILE_SIZE_ERROR = "File size cannot exceed 3MB";

        public const String PAST_DATE_ERROR = "1st Month Saves Date cannot be in the past.  Please update the date or if the date is correct, please Submit for Approval";
        public const String PAST_DATE_ERROR_REVISED = "Revised 1st Month Saves Date cannot be in the past.  Please update the date or if the date is correct, please Submit for Approval";
        public const String OVERVIEW_TAB = "Project Overview\n";
        public const String SCOPE_TAB = "Project Scope\n";
        public const String FINANCIALS_TAB = "Financials\n";
        public const String ERROR_MSG = "\n Please complete all required fields on the following tabs: \n";

        public const String LOB_COLUMN = "LOBT1";

        public const String VAL_1 = "Pre-Cancellation Notice: Submit as In Progress, Future Pipeline or Cancel Idea.";
        public const String FIL_VAL_1 = "Pre-Cancellation Notice";
        public const String IMG_VAL_1 = "draft60day.png";

        public const String VAL_2 = "1st Mo Saves Date Approaching: Revise 1st Month Saves Date, Submit for Approval or Cancel Idea.";
        public const String FIL_VAL_2 = "In Progress Saves Date Approaching";
        public const String IMG_VAL_2 = "inprogress30day.png";

        public const String VAL_3 = "1st Mo Saves Date Past Due: Revise 1st Month Saves Date, Submit for Approval or Cancel Idea.";
        public const String FIL_VAL_3 = "In Progress Saves Date Past Due";
        public const String IMG_VAL_3 = "inprogresspastdue.png";


        public const string VAL_4 = "Provide Financial Review";
        public const string FIL_VAL_4 = "Provide Financial Review";
        public const String IMG_VAL_4 = "readyforfinancereview.png";

        public const string VAL_5 = "SLA Past Due: Provide Financial Review";
        public const string FIL_VAL_5 = "SLA Past Due";
        public const String IMG_VAL_5 = "readyforfinancereview14day.png";

        public const String VAL_6 = "This record is available to action for 365 days after cancellation. Please contact your EXCEL Admin to reactivate.";
        public const String FIL_VAL_6 = "Canceled";
        public const String IMG_VAL_6 = "canceled.png";

        public const string VAL_7 = "Approved";
        public const string FIL_VAL_7 = "Approved";
        public const String IMG_VAL_7 = "approved.png";

        public const string VAL_8 = "1st Mo Saves Date Approaching: Revise 1st Month Saves Date, Submit in Progress or Cancel Idea";
        public const string FIL_VAL_8 = "Future Saves Date Approaching";
        public const String IMG_VAL_8 = "Future30day.png";

        public const string VAL_9 = "1st Mo Saves Date Past Due: Revise 1st Month Saves Date, Submit in Progress or Cancel Idea";
        public const string FIL_VAL_9 = "Future Saves Date Past Due";
        public const String IMG_VAL_9 = "FuturePastDue.png";


        public class LOB1
        {
            public const String GCP = "GCP";
            public const String GBT = "GBT";
            public const String GBS = "GBS";
            public const String PBMT = "PBMT";
            public const String HR = "HR";
            public const String WSGCAT = "WSGCAT";

            public static String[] GBSHR = new String[] { GBS, HR };

        }

        public class LOB2
        {
            public const String GCP = "GCP";
            public const String GBT = "GBT";
            public const String GBS = "GBS";
            public const String PBMT = "PBMT";

        }



    }

    public class MyItem
    {
        public String AIM_ID { get; set; }
        public String AIM_NAME { get; set; }

        public String Content
        {
            get
            {
                return AIM_NAME;
            }
            set
            {
                AIM_NAME = value;
            }
        }

        public override string ToString()
        {
            return AIM_NAME;
        }

    }
}
