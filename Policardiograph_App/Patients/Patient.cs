using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.Patients
{
    public class Patient: PatientBase, IComparable
    {
        public Patient()
        {

        }

        public Patient(Patient patient)
        {
            this.Name = patient.Name;
            this.Surname = patient.Surname;
            this.ParentName = patient.ParentName;
            this.JMBG = patient.JMBG;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            Patient otherPatient = obj as Patient;
            if (otherPatient != null)
            {
                return this.Name.CompareTo(otherPatient.Name);
            }
            else
            {
                throw new ArgumentException("Object is not a Patient");
            }
        }

        public string Name { get; set; } = "";        
        public string Surname { get; set; } = "";
        public string ParentName { get; set; } = "";
        public string JMBG { get; set; } = "";
        [System.Xml.Serialization.XmlIgnore]
        public string Comment { get; set; } = "";
    }
}
