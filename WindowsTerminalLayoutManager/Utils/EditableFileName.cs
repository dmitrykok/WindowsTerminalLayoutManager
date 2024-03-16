using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TerminalLayoutManager.Utils
{
    public class EditableFileName(string fileName) : INotifyPropertyChanged
    {
        private string fileName = fileName;
        private bool isEditing = false;
        private string fileNameError = string.Empty;
        private Visibility fileNameErrorVisibility = Visibility.Collapsed;
        private bool fileNameErrorIsOpen = false;

        public string FileNameError
        {
            get => fileNameError;
            set
            {
                fileNameError = value;
                OnPropertyChanged(nameof(FileNameError));
            }
        }

        public Visibility FileNameErrorVisibility
        {
            get => fileNameErrorVisibility;
            set
            {
                fileNameErrorVisibility = value;
                OnPropertyChanged(nameof(FileNameErrorVisibility));
            }
        }

        public bool FileNameErrorIsOpen
        {
            get => fileNameErrorIsOpen;
            set
            {
                fileNameErrorIsOpen = value;
                OnPropertyChanged(nameof(FileNameErrorIsOpen));
            }
        }

        public string FileName
        {
            get { return fileName; }
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }

        public bool IsEditing
        {
            get { return isEditing; }
            set
            {
                if (isEditing != value)
                {
                    isEditing = value;
                    OnPropertyChanged(nameof(IsEditing));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
