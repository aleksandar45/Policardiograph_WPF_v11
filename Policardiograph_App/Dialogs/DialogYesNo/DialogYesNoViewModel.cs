using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using Policardiograph_App.Dialogs.DialogService;
using Policardiograph_App.ViewModel;

namespace Policardiograph_App.Dialogs.DialogYesNo
{
    public class DialogYesNoViewModel: DialogViewModelBase
    {
        private ICommand yesCommand = null;
        public ICommand YesCommand
        {
            get { return yesCommand; }
            set { yesCommand = value; }
        }

        private ICommand noCommand = null;
        public ICommand NoCommand
        {
            get { return noCommand; }
            set { noCommand = value; }
        }
        public string Message
        {
            get;
            private set;
        }
  
        public DialogYesNoViewModel(string dialogTittle, string message) :base (dialogTittle)           
        {
            this.Message = message;
            this.yesCommand = new DelegateCommand(OnYesClicked);
            this.noCommand = new DelegateCommand(OnNoClicked);
        }

        private void OnYesClicked(object parameter)
        {
            this.CloseDialogWithResult(parameter as Window, new DialogResultYesNo(DialogResultYesNoEnum.Yes));  

        }

        private void OnNoClicked(object parameter)
        {
            this.CloseDialogWithResult(parameter as Window, new DialogResultYesNo(DialogResultYesNoEnum.No));  
        } 
    }
}
