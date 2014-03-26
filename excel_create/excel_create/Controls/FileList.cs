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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace excel_create.Controls
{
    public class SelectedFiles : ObservableCollection<FileEntry>
    {
        public SelectedFiles()
            : base()
        {
            // Add(new AccountList("Willa", "Cather"));
            //   Add(new AccountList("Isak", "Dinesen"));
            // Add(new AccountList("Victor", "Hugo"));
            // Add(new AccountList("Jules", "Verne"));
        }
    }

    public class FileEntry
    {
        private string _fileName;
        private string _fileDesc;

        public String FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                OnPropertyChanged("AccountName");
            }
        }

        public String FileDesc
        {
            get
            {
                return _fileDesc;
            }

            set
            {
                _fileDesc = value;
                OnPropertyChanged("FileDesc");
            }
        }

        public FileEntry(String _fileName, String _fileDesc)
        {
            this.FileName = _fileName;
            this.FileDesc = _fileDesc;

        }

        public override string ToString()
        {
            return FileName;
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
