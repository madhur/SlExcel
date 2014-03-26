using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

    public class SelectedAccounts : ObservableCollection<AccountList>
    {
        public SelectedAccounts()
            : base()
        {
            // Add(new AccountList("Willa", "Cather"));
            //   Add(new AccountList("Isak", "Dinesen"));
            // Add(new AccountList("Victor", "Hugo"));
            // Add(new AccountList("Jules", "Verne"));
        }
    }

    public class AccountList : INotifyPropertyChanged
    {
        private string _accountName;
        private string _displayName;

        public String AccountName
        {
            get
            {
                return _accountName;
            }
            set
            {
                _accountName = value;
                OnPropertyChanged("AccountName");
            }
        }

        public String DisplayName
        {
            get
            {
                return _displayName;
            }

            set
            {
                _displayName = value;
                OnPropertyChanged("DisplayName");
            }
        }

        public AccountList(String acName, String dispName)
        {
            this.AccountName = acName;
            this.DisplayName = dispName;

        }

        public override string ToString()
        {
            return DisplayName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                new PropertyChangedEventArgs(property));
            }
        }


    }
}
