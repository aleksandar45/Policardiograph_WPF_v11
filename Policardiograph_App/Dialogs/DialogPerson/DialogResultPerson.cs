using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Policardiograph_App.Dialogs.DialogService;

namespace Policardiograph_App.Dialogs.DialogPerson
{
    public class DialogResultPerson: DialogResult
    {
        public DialogResultPerson(bool applyResult, List<Patients.Patient> patients, Patients.Patient selectedPatient)
        {
            ApplyResult = applyResult;
            Patients = patients;
            SelectedPatient = selectedPatient;
        }
        public bool ApplyResult
        {
            get;
            private set;
        }
        public List<Patients.Patient> Patients
        {
            get;
            private set;
        }
        public Patients.Patient SelectedPatient
        {
            get;
            private set;
        }
    }
}
