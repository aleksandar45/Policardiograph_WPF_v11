using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using Policardiograph_App.Dialogs.DialogService;
using Policardiograph_App.Patients;
using Policardiograph_App.ViewModel;

namespace Policardiograph_App.Dialogs.DialogPersonComment
{
    public class DialogPersonCommentViewModel: DialogViewModelBase
    {
        public DialogPersonCommentViewModel(Patient patient): base("Add or change measurement comment")
        {
            OKCommand = new DelegateCommand(OnOKClicked);
            MeasurementComment = patient.Comment;
        }

        
        public ICommand OKCommand { get; set; } = null;
        private void OnOKClicked(object parameter)
        {
            this.CloseDialogWithResult(parameter as Window, new DialogResultPersonComment(MeasurementComment));
        }        

        private string _measurementComment;
        public string MeasurementComment
        {
            get
            {
                return _measurementComment;
            }
            set
            {
                _measurementComment = value;
                OnPropertyChanged("MeasurementComment");
            }
        }
    }
}
