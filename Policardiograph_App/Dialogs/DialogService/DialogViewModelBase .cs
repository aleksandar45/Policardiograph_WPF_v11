using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;


namespace Policardiograph_App.Dialogs.DialogService
{
    public abstract class DialogViewModelBase:INotifyPropertyChanged
    {
        public DialogViewModelBase(string dialogTittle) {
            this.DialogTittle = dialogTittle;
        }
        public string DialogTittle
        {
            get;
            private set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public DialogResult UserDialogResult
        {
            get;
            private set;
        }

        public void CloseDialogWithResult(Window dialog, DialogResult result)
        {
            this.UserDialogResult = result;
            if (dialog != null)
                dialog.DialogResult = true;
        }   
       
    }
}
