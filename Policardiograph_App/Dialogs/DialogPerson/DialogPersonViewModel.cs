using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Policardiograph_App.Dialogs.DialogService;
using Policardiograph_App.ViewModel;
using Policardiograph_App.Patients;
using Policardiograph_App.Dialogs.DialogYesNo;

namespace Policardiograph_App.Dialogs.DialogPerson
{
    public class DialogPersonViewModel: DialogViewModelBase
    {

        public DialogPersonViewModel(List<Patient>  patients, Patient selectedPatient) : base("Add or choose patient")
        {
            this.patients = patients;
            this.selectedPatient = selectedPatient;

            SelectedViewModel = new DialogPersonChangeViewModel(this);

            OKCommand = new DelegateCommand(OnOKClicked);
            CancelCommand = new DelegateCommand(OnCancelClicked);
            NewCommand = new DelegateCommand(OnNewClicked);
            EditCommand = new DelegateCommand(OnEditClicked);
            DeleteCommand = new DelegateCommand(OnDeleteClicked);            
                        
        }

        public List<Patient> patients;
        public Patient selectedPatient;

        private int _contentRowSpan=1;
        public int ContentRowSpan
        {
            get
            {
                return _contentRowSpan;
            }
            set
            {
                _contentRowSpan = value;
                OnPropertyChanged("ContentRowSpan");
            }
        }
        private bool _panel1IsVisible = true;
        public bool Panel1IsVisible
        {
            get
            {
                return _panel1IsVisible;
            }
            set
            {
                _panel1IsVisible = value;
                OnPropertyChanged("Panel1IsVisible");
            }
        }
        private bool _panel2IsVisible = true;
        public bool Panel2IsVisible
        {
            get
            {
                return _panel2IsVisible;
            }
            set
            {
                _panel2IsVisible = value;
                OnPropertyChanged("Panel2IsVisible");
            }
        }
        private object _selectedViewModel;
        public object SelectedViewModel
        {
            get
            {
                return _selectedViewModel;
            }

            set {
                _selectedViewModel = value;
                OnPropertyChanged("SelectedViewModel");
            }

        }


        #region BUTTON_OK
        public ICommand OKCommand { get; set; } = null;
        private void OnOKClicked(object parameter)
        {
            this.CloseDialogWithResult(parameter as Window, new DialogResultPerson(true, patients, selectedPatient));
        }
        #endregion

        #region BUTTON_Cancel
        public ICommand CancelCommand { get; set; } = null;
        private void OnCancelClicked(object parameter)
        {
            this.CloseDialogWithResult(parameter as Window, new DialogResultPerson(false, patients,selectedPatient));

        }
        #endregion

        #region BUTTON_New
        public ICommand NewCommand { get; set; } = null;
        private void OnNewClicked(object parameter)
        {
           
            SelectedViewModel = new DialogPersonNewViewModel(this);
            ContentRowSpan = 3;
            Panel1IsVisible = false;
            Panel2IsVisible = false;
        }
        #endregion

        #region BUTTON_Edit
        public ICommand EditCommand { get; set; } = null;
        private void OnEditClicked(object parameter)
        {
            if (selectedPatient != null)
            {
                selectedPatient = patients.ElementAt((SelectedViewModel as DialogPersonChangeViewModel).ComboboxSelectedIndex);
                SelectedViewModel = new DialogPersonEditViewModel(this);
                ContentRowSpan = 3;
                Panel1IsVisible = false;
                Panel2IsVisible = false;
            }

        }
        #endregion

        #region BUTTON_Delete
        public ICommand DeleteCommand { get; set; } = null;
        private void OnDeleteClicked(object parameter)
        {
            if (patients != null)
            {
                DialogViewModelBase vm = new DialogYesNoViewModel("Delete patient?", "Are you sure you want to delete patient?");
                DialogResult result = DialogService.DialogService.OpenDialog(vm,null);
                if ((result as DialogResultYesNo).Result== DialogResultYesNoEnum.Yes) {
                    patients.Remove(selectedPatient);
                    if (patients.Count > 0)
                    {
                        selectedPatient = patients.ElementAt(0);
                    }
                    else
                    {
                        patients = null;
                        selectedPatient = null;
                    }

                }                                
            }
            else
            {
                patients = null;
                selectedPatient = null;
            }            
            SelectedViewModel = new DialogPersonChangeViewModel(this);
        }
        #endregion
    }
}
