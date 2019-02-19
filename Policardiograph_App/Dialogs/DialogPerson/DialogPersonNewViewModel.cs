using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Policardiograph_App.ViewModel;
using Policardiograph_App.Dialogs.DialogService;
using Policardiograph_App.Patients;

namespace Policardiograph_App.Dialogs.DialogPerson
{
    public class DialogPersonNewViewModel: DialogViewModelBase
    {
        public DialogPersonNewViewModel(DialogPersonViewModel dialogPersonViewModel): base("Add new patient") {
            this.dialogPersonViewModel = dialogPersonViewModel;

            OKCommand = new DelegateCommand(OnOKClicked);
            CancelCommand = new DelegateCommand(OnCancelClicked);
        }

        private readonly DialogPersonViewModel dialogPersonViewModel;


        #region BUTTON_OK
        public ICommand OKCommand { get; set; } = null;
        private void OnOKClicked(object parameter)
        {
            bool error = false;
            if (TextBoxPatientName.CompareTo("") == 0) {
                PatientNameWarningIsVisible = true;
                error = true;
            }
            if (TextBoxPatientSurname.CompareTo("") == 0)
            {
                PatientSurnameWarningIsVisible = true;
                error = true;
            }
            if (TextBoxPatientParentName.CompareTo("") == 0)
            {
                PatientParentNameWarningIsVisible = true;
                error = true;
            }
            if (TextBoxPatientJMBG.CompareTo("") == 0)
            {
                PatientJMBGWarningIsVisible = true;
                error = true;
            }
            if (!error)
            {
                Patient newPatient = new Patient();
                newPatient.Name = TextBoxPatientName;
                newPatient.Surname = TextBoxPatientSurname;
                newPatient.ParentName = TextBoxPatientParentName;
                newPatient.JMBG = TextBoxPatientJMBG;
                if (dialogPersonViewModel.patients == null)
                    dialogPersonViewModel.patients = new List<Patient>();
                dialogPersonViewModel.patients.Add(newPatient);

                dialogPersonViewModel.patients.Sort();
                dialogPersonViewModel.selectedPatient = newPatient;                              
                
                dialogPersonViewModel.SelectedViewModel = new DialogPersonChangeViewModel(dialogPersonViewModel);
                dialogPersonViewModel.ContentRowSpan = 2;
                dialogPersonViewModel.Panel1IsVisible = true;
                dialogPersonViewModel.Panel2IsVisible = true;
            }

        }
        #endregion

        #region BUTTON_Cancel
        public ICommand CancelCommand { get; set; } = null;
        private void OnCancelClicked(object parameter)
        {
            DialogPersonChangeViewModel dialogPersonChangeViewModel = new DialogPersonChangeViewModel(dialogPersonViewModel);
            dialogPersonChangeViewModel.ComboboxSelectedItem = dialogPersonViewModel.selectedPatient.Name + " (" + dialogPersonViewModel.selectedPatient.ParentName + ") " + dialogPersonViewModel.selectedPatient.Surname;
            dialogPersonViewModel.SelectedViewModel = dialogPersonChangeViewModel;
            dialogPersonViewModel.ContentRowSpan = 2;
            dialogPersonViewModel.Panel1IsVisible = true;
            dialogPersonViewModel.Panel2IsVisible = true;

        }
        #endregion

        #region TEXTBOX
        private bool _patientNameWarningIsVisible = false;
        public bool PatientNameWarningIsVisible
        {
            get
            {
                return _patientNameWarningIsVisible;
            }
            set
            {
                _patientNameWarningIsVisible = value;
                OnPropertyChanged("PatientNameWarningIsVisible");
            }
        }
        private bool _patientSurnameWarningIsVisible = false;
        public bool PatientSurnameWarningIsVisible
        {
            get
            {
                return _patientSurnameWarningIsVisible;
            }
            set
            {
                _patientSurnameWarningIsVisible = value;
                OnPropertyChanged("PatientSurnameWarningIsVisible");
            }
        }
        private bool _patientParentNameWarningIsVisible = false;
        public bool PatientParentNameWarningIsVisible
        {
            get
            {
                return _patientParentNameWarningIsVisible;
            }
            set
            {
                _patientParentNameWarningIsVisible = value;
                OnPropertyChanged("PatientParentNameWarningIsVisible");
            }
        }
        private bool _patientJMBGWarningIsVisible = false;
        public bool PatientJMBGWarningIsVisible
        {
            get
            {
                return _patientJMBGWarningIsVisible;
            }
            set
            {
                _patientJMBGWarningIsVisible = value;
                OnPropertyChanged("PatientJMBGWarningIsVisible");
            }
        }
        private string _textBoxPatientName = "";
        public string TextBoxPatientName
        {
            get
            {
                return _textBoxPatientName;
            }
            set
            {
                _textBoxPatientName = value;
                OnPropertyChanged("TextBoxPatientName");
            }
        }
        private string _textBoxPatientSurname = "";
        public string TextBoxPatientSurname
        {
            get
            {
                return _textBoxPatientSurname;
            }
            set
            {
                _textBoxPatientSurname = value;
                OnPropertyChanged("TextBoxPatientSurname");
            }
        }
        private string _textBoxPatientParentName = "";
        public string TextBoxPatientParentName
        {
            get
            {
                return _textBoxPatientParentName;
            }
            set
            {
                _textBoxPatientParentName = value;
                OnPropertyChanged("TextBoxPatientParentName");
            }
        }
        private string _textBoxPatientJMBG = "";
        public string TextBoxPatientJMBG
        {
            get
            {
                return _textBoxPatientJMBG;
            }
            set
            {
                _textBoxPatientJMBG = value;
                OnPropertyChanged("TextBoxPatientJMBG");
            }
        }
        #endregion

    }
}
