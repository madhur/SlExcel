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

namespace excel_create.Controls
{
    public class PickerEntry
    {
        public string DisplayName { get; set; }
        public string AccountName { get; set; }
        public string Email { get; set; }

        public string Department { get; set; }


        public PickerEntry(string displayName, string accountName, string email, string department)
        {
            this.DisplayName = displayName;
            this.AccountName = accountName;
            this.Email = email;
            this.Department = department;
        }

        public override string ToString()
        {

            return DisplayName + " - " + Email + " - " + Department;

        }
    }
}
