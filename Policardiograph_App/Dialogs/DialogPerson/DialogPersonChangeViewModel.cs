using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Policardiograph_App.ViewModel;
using Policardiograph_App.Dialogs.DialogService;

namespace Policardiograph_App.Dialogs.DialogPerson
{
    public class DialogPersonChangeViewModel:DialogViewModelBase
    {

        public DialogPersonChangeViewModel(DialogPersonViewModel dialogPersonViewModel) : base("Choose Patient")
        {
            this.dialogPersonViewModel = dialogPersonViewModel;

            _comboBoxPatientCredentials = new List<string>();
            if (dialogPersonViewModel.patients != null)
            {
                for (int i = 0; i < dialogPersonViewModel.patients.Count; i++)
                {
                    _comboBoxPatientCredentials.Add(dialogPersonViewModel.patients.ElementAt(i).Name + " (" + dialogPersonViewModel.patients.ElementAt(i).ParentName + ") " + dialogPersonViewModel.patients.ElementAt(i).Surname);
                    if (dialogPersonViewModel.selectedPatient != null)
                    {
                        if (dialogPersonViewModel.patients.ElementAt(i).Name.CompareTo(dialogPersonViewModel.selectedPatient.Name) == 0)
                        {
                            if (dialogPersonViewModel.patients.ElementAt(i).Surname.CompareTo(dialogPersonViewModel.selectedPatient.Surname) == 0)
                            {
                                if (dialogPersonViewModel.patients.ElementAt(i).ParentName.CompareTo(dialogPersonViewModel.selectedPatient.ParentName) == 0)
                                {
                                    if (dialogPersonViewModel.patients.ElementAt(i).JMBG.CompareTo(dialogPersonViewModel.selectedPatient.JMBG) == 0)
                                    {
                                        ComboboxSelectedIndex= i;
                                    }
                                }
                            }
                        }
                    }
                }                                                    
            }
            else
            {
                _comboBoxPatientCredentials.Add("<No Patients in Base>");
                ComboboxSelectedItem = "<No Patients in Base>";
            }
        }

        private readonly DialogPersonViewModel dialogPersonViewModel;

        #region COMBOBOX_PROFILE
        private List<string> _comboBoxPatientCredentials;
        public List<string> ComboBoxPatientCredentials
        {
            get
            {
                return _comboBoxPatientCredentials;
            }
            set
            {
                _comboBoxPatientCredentials = value;
                OnPropertyChanged("ComboBoxPatientCredentials");
            }
        }
        private String _comboboxSelectedItem = "";
        public String ComboboxSelectedItem
        {
            get
            {
                return _comboboxSelectedItem;
            }
            set
            {
                _comboboxSelectedItem = value;
                OnPropertyChanged("ComboboxSelectedItem");
            }
        }
        private int _comboboxSelectedIndex;
        public int ComboboxSelectedIndex { 
            get
            {
                return _comboboxSelectedIndex;
            }
            set
            {
                _comboboxSelectedIndex = value;
                OnPropertyChanged("ComboboxSelectedIndex");
            }
        }     
        #endregion
    }
}
