using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Policardiograph_App.Dialogs.DialogService;
using Policardiograph_App.ViewModel;
using System.Windows;

namespace Policardiograph_App.Dialogs.DialogMessage
{
    public class DialogMessageViewModel : DialogViewModelBase
    {
        public string Message
        {
            get;
            private set;
        }
        private ICommand okCommand = null;
        public ICommand OKCommand
        {
            get { return okCommand; }
            set { okCommand = value; }
        }

        public string ImageSource
        {
            get;
            private set;

        }

        public DialogMessageViewModel(DialogImageTypeEnum imageType,string message): base(ModifyMessage(imageType))
        {
            this.Message = message;
            this.okCommand = new DelegateCommand(OnOKClicked);
            switch (imageType) { 
                case DialogImageTypeEnum.Error:
                    ImageSource = "/Policardiograph_App;component/Images/Error.png";                    
                    break;
                case DialogImageTypeEnum.Info:
                    ImageSource = "/Policardiograph_App;component/Images/Info.png";                    
                    break;
                case DialogImageTypeEnum.Warning:
                    ImageSource = "/Policardiograph_App;component/Images/Warning.png";                    
                    break;
            }
        }
        private static string ModifyMessage(DialogImageTypeEnum imageType) { 
            switch (imageType) { 
                case DialogImageTypeEnum.Error:                    
                    return "Error";
                case DialogImageTypeEnum.Info:
                    return "Info";
                case DialogImageTypeEnum.Warning:
                    return "Warning";
                default:
                    return "Error";
            }
        }
        private void OnOKClicked(object parameter)
        {
            this.CloseDialogWithResult(parameter as Window, new DialogResultMessage());

        }
    }
}
