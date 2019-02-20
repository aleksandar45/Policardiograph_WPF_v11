using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace Policardiograph_App.Patients
{
    public static class PatientService
    {
        public static void LoadPatients(ref List<Patient> patients, ref Patient selectedPatient) {

            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!path.EndsWith("\\")) path += "\\";
            path += "PolicardiographApp\\";

            XmlSerializer serializer = new XmlSerializer(typeof(List<Patient>));

            try
            {
                if (File.Exists(path + "Patients.xml"))
                {
                    StreamReader reader = new StreamReader(path + "Patients.xml");
                    List<Patient> tempPatients = null;
                    tempPatients = (List<Patient>)serializer.Deserialize(reader);
                    selectedPatient = tempPatients.ElementAt(0);
                    patients = tempPatients.GetRange(1, tempPatients.Count-1);
                    reader.Close();
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }



        }
        public static void StorePatients(List<Patient> patients, Patient selectedPatient)
        {            

            XmlSerializer XMLwriter = new XmlSerializer(typeof(List<Patient>));

            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!path.EndsWith("\\")) path += "\\";
            path += "PolicardiographApp\\";            
            StreamWriter XMLfile = new StreamWriter(path + "Patients.xml");
            
            try
            {
                if((patients!=null) && (selectedPatient != null))
                {
                    List<Patient> tempPatients = new List<Patient>();
                    tempPatients.Add(selectedPatient);
                    tempPatients.AddRange(patients);
                    XMLwriter.Serialize(XMLfile, tempPatients);

                    XMLfile.Close();
                }
                else
                {
                    XMLfile.Close();
                    File.Delete(path + "Patients.xml");
                }
                
            }
            catch(Exception ex)
            {
                if (XMLfile != null)
                {
                    XMLfile.Close();
                }
            }
            finally
            {
                XMLfile.Close();
            }
        }
    }
}
