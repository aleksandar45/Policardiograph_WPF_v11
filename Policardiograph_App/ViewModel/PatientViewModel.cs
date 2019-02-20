using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.ViewModel
{
    public class PatientViewModel: ViewModelBase
    {
        public PatientViewModel()
        {

        }

        private string _patientName = "";
        public string PatientName
        {
            get
            {
                return _patientName;
            }
            set
            {
                _patientName = value;
                OnPropertyChanged("PatientName");
            }
        }

        private string _patientSurname = "";
        public string PatientSurname
        {
            get
            {
                return _patientSurname;
            }
            set
            {
                _patientSurname = value;
                OnPropertyChanged("PatientSurname");
            }
        }

        private string _measurementComment = "";
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
