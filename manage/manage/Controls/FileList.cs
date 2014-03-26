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

namespace manage.Controls
{
    public class SelectedFiles : ObservableCollection<FileEntry>
    {
        public SelectedFiles()
            : base()
        {
        }
    }

    public class FileEntry
    {
        private string _fileName;
        private string _fileDesc;
        private bool isTemp;
        private bool isTempDelete;

        public bool IsTempDelete
        {
            get
            {
                return isTempDelete;
            }
            set
            {
                isTempDelete = value;
            }

        }
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

        public bool IsTemp
        {
            get
            {
                return isTemp;
            }
            set
            {
                isTemp = value;
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

        public FileEntry(String _fileName, String _fileDesc, bool _isTemp)
        {
            this.FileName = _fileName;
            this.FileDesc = _fileDesc;
            this.isTemp = _isTemp;

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
