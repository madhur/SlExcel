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

namespace excel_report
{
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
        public static String[] Admin = new String[] { FINANCE_REVIEW, SUBMIT_APPROVAL, PENDING_ACTUALS };

        public const String STATUS_COLUMN = "ideaStatus";

    }
}
