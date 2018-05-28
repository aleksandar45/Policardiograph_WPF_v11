using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Policardiograph_App
{
    class Log
    {
        public string GetTempPath()
        {            
            string path = System.IO.Directory.GetCurrentDirectory();
            if (!path.EndsWith("\\")) path += "\\";
            return path;
        }

        [Conditional("DEBUG")]
        public void LogMessageToFile(string msg)
        {
            try
            {
                System.IO.StreamWriter sw = System.IO.File.AppendText(
                    GetTempPath() + "Log.txt");
                try
                {

                    string logLine = System.String.Format(
                        "{0:G}: {1}.", System.DateTime.Now, msg);
                    sw.WriteLine(logLine);
                }
                catch (Exception ex)
                {
                    Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Error, ex.Message);
                    Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);                    
                }
                finally
                {
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Error, ex.Message);
                Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);                

            }
        }
    }
}
