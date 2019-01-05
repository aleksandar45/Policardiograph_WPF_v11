using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using Policardiograph_App.Settings;

namespace Policardiograph_App.Settings
{
    public static class SettingService
    {
        public static SettingProgram LoadSettingProgram() {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!path.EndsWith("\\")) path += "\\";
            path += "PolicardiographApp\\";
            try
            {
                string savePath = "";
                float battMax = 0.0f;
                float battMin = 0.0f;
                float battAlert = 0.0f;
                int battLastTime = 0;

                string line;

                if (!File.Exists(path + "Settings.dat")) {                    
                    savePath = path + "Sync_Capture";
                    if (!Directory.Exists(savePath)) {
                        Directory.CreateDirectory(path);
                    }
                    return new SettingProgram(savePath, 6.2f, 5.0f, 5.2f, 0);
                }
                StreamReader sr = File.OpenText(path + "Settings.dat");

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (String.Compare(line, 0, "ProgramSetting", 0, 14) == 0)
                    {
                        if (String.Compare(line, 0, "ProgramSetting_SavePath", 0, 23) == 0) savePath = line.Substring(24);
                        if (String.Compare(line, 0, "ProgramSetting_BattMax", 0, 22) == 0) battMax = (float)Double.Parse(line.Substring(23), CultureInfo.InvariantCulture);
                        if (String.Compare(line, 0, "ProgramSetting_BattMin", 0, 22) == 0) battMin = (float)Double.Parse(line.Substring(23), CultureInfo.InvariantCulture);
                        if (String.Compare(line, 0, "ProgramSetting_BattAlert", 0, 24) == 0) battAlert = (float)Double.Parse(line.Substring(25), CultureInfo.InvariantCulture);
                        if (String.Compare(line, 0, "ProgramSetting_BattLastTime", 0, 27) == 0) battLastTime = Int32.Parse(line.Substring(28));
                    }

                }
                sr.Close();
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(path + "Sync_Capture");
                    savePath = path + "Sync_Capture";
                }
                // if (!System.IO.Directory.Exists(savePath)) throw new Exception("Save directory declared in Setting.dat does not exist. Create it manualy or change save path!!!");
                return new SettingProgram(savePath, battMax, battMin, battAlert, battLastTime);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public static SettingWindow LoadSettingWindow() {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!path.EndsWith("\\")) path += "\\";
            path += "PolicardiographApp\\";
            try
            {
                int timeAxis = 0;
                int moduleChannel = 0;
                string moduleString = "";
                string moduleAxis = "";
                string moduleDescription = "";
                List<ModuleChannel> selectedUserDisplays = new List<ModuleChannel>();
                List<ModuleChannel> selectedFBGADisplays = new List<ModuleChannel>();
                List<int> channelNumbers = new List<int>() { 2, 1, 12, 2, 2 };

                string line;
                if (!File.Exists(path + "Settings.dat"))
                {
                    selectedUserDisplays.Add(new ModuleChannel("MIC", 1, "", ""));
                    selectedUserDisplays.Add(new ModuleChannel("ECG", 1, "", ""));
                    selectedUserDisplays.Add(new ModuleChannel("FBGA", 1, "", ""));
                    selectedUserDisplays.Add(new ModuleChannel("ACC", 1, "", "z"));
                    selectedUserDisplays.Add(new ModuleChannel("PPG", 1, "", ""));
                    selectedUserDisplays.Add(new ModuleChannel("PPG", 2, "", ""));
                    return new SettingWindow(4, channelNumbers, selectedUserDisplays, true);
                }
                StreamReader sr = File.OpenText(path + "Settings.dat");
               
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (String.Compare(line, 0, "MainWindowSetting", 0, 17) == 0)
                    {
                        if (String.Compare(line, 0, "MainWindowSetting_TimeAxis=", 0, 27) == 0) timeAxis = Int32.Parse(line.Substring(27));
                        //line = sr.ReadLine();
                        if (String.Compare(line, 0, "MainWindowSetting_DisplayAxis", 0, 29) == 0) moduleAxis = line.Substring(31);
                        if (String.Compare(line, 0, "MainWindowSetting_DisplayDescription", 0, 36) == 0) moduleDescription = line.Substring(38);
                        if (String.Compare(line, 0, "MainWindowSetting_DisplayModuleSelection", 0, 40) == 0) moduleString = line.Substring(42);
                        if (String.Compare(line, 0, "MainWindowSetting_DisplayChannelSelection", 0, 41) == 0)
                        {
                            moduleChannel = Int32.Parse(line.Substring(43));
                            selectedUserDisplays.Add(new ModuleChannel(moduleString, moduleChannel, moduleDescription, moduleAxis));
                            
                            moduleChannel = 0;
                            moduleString = "";
                            moduleAxis = "";
                            moduleDescription = "";
                        }
                    }                    
                }


                sr.Close();                
                return new SettingWindow(timeAxis, channelNumbers, selectedUserDisplays, true);


            }
            catch (Exception ex) {
                throw ex;
            }

        }
        public static SettingFBGA LoadSettingFBGA()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!path.EndsWith("\\")) path += "\\";
            path += "PolicardiographApp\\";
            try
            {
                int chNumber = 0;
                int integrationTime = 0;
                int moduleChannel = 0;
                double sledPower = 0;
                bool highDynamicRange = false;
                string moduleString = "";
                string moduleAxis = "";
                string moduleDescription = "";
                List<ModuleChannel> selectedFBGADisplays = new List<ModuleChannel>();

                string line;
                if (!File.Exists(path + "Settings.dat"))
                {
                    selectedFBGADisplays.Add(new ModuleChannel("MIC", 1, "", ""));
                    return new SettingFBGA(1, 500, 2.0, true, selectedFBGADisplays);
                }
                StreamReader sr = File.OpenText(path + "Settings.dat");
                
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();                    
                    if (String.Compare(line, 0, "FBGASetting", 0, 11) == 0)
                    {
                        if (String.Compare(line, 0, "FBGASetting_NoChannel=", 0, 22) == 0) chNumber = Int32.Parse(line.Substring(22));
                        if (String.Compare(line, 0, "FBGASetting_IntegrationTime=", 0, 28) == 0) integrationTime = Int32.Parse(line.Substring(28));
                        if (String.Compare(line, 0, "FBGASetting_SLEDPower=", 0, 22) == 0)
                            sledPower = Double.Parse(line.Substring(22), CultureInfo.InvariantCulture);
                        if (String.Compare(line, 0, "FBGASetting_HighDynamicRange=True", 0, 33) == 0) highDynamicRange = true;                       
                        if (String.Compare(line, 0, "FBGASetting_DisplayAxis", 0, 23) == 0) moduleAxis = line.Substring(25);
                        if (String.Compare(line, 0, "FBGASetting_DisplayDescription", 0, 30) == 0) moduleDescription = line.Substring(32);
                        if (String.Compare(line, 0, "FBGASetting_DisplayModuleSelection", 0, 34) == 0) moduleString = line.Substring(36);
                        if (String.Compare(line, 0, "FBGASetting_DisplayChannelSelection", 0, 35) == 0)
                        {
                            moduleChannel = Int32.Parse(line.Substring(37));
                            selectedFBGADisplays.Add(new ModuleChannel(moduleString, moduleChannel, moduleDescription, moduleAxis));

                            moduleChannel = 0;
                            moduleString = "";
                            moduleAxis = "";
                            moduleDescription = "";
                        }

                    }
                }


                sr.Close();

                return new SettingFBGA(chNumber,integrationTime,sledPower,highDynamicRange, selectedFBGADisplays);


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static SettingMIC LoadSettingMIC()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!path.EndsWith("\\")) path += "\\";
            path += "PolicardiographApp\\";
            try
            {
                int chNumber = 0;
                int moduleChannel = 0;
                bool mic1Mute = false;
                bool mic2Mute = false;
                bool mic3Mute = false;
                bool mic4Mute = false;
                bool highPassFilter = false;
                bool syncTest = false;
                string moduleString = "";
                string moduleAxis = "";
                string moduleDescription = "";
                List<ModuleChannel> selectedMICDisplays = new List<ModuleChannel>();

                string line;
                if (!File.Exists(path + "Settings.dat"))
                {
                    selectedMICDisplays.Add(new ModuleChannel("MIC", 1, "", ""));
                    selectedMICDisplays.Add(new ModuleChannel("MIC", 2, "", ""));
                    return new SettingMIC(2, false, false, true, false, false, false, selectedMICDisplays);
                }
                System.IO.StreamReader sr = System.IO.File.OpenText(path + "Settings.dat");
               
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (String.Compare(line, 0, "MICSetting", 0, 10) == 0)
                    {
                        if (String.Compare(line, 0, "MICSetting_MIC1Mute=True", 0, 24) == 0) mic1Mute = true;
                        if (String.Compare(line, 0, "MICSetting_MIC2Mute=True", 0, 24) == 0) mic2Mute = true;
                        if (String.Compare(line, 0, "MICSetting_MIC3Mute=True", 0, 24) == 0) mic3Mute = true;
                        if (String.Compare(line, 0, "MICSetting_MIC4Mute=True", 0, 24) == 0) mic4Mute = true;
                        if (String.Compare(line, 0, "MICSetting_HighPassFilter=True", 0, 30) == 0) highPassFilter = true;
                        if (String.Compare(line, 0, "MICSetting_SyncTest=True", 0, 24) == 0) syncTest = true;
                        if (String.Compare(line, 0, "MICSetting_NoChannel=", 0, 21) == 0) chNumber = Int32.Parse(line.Substring(21));
                        if (String.Compare(line, 0, "MICSetting_DisplayAxis", 0, 22) == 0) moduleAxis = line.Substring(24);
                        if (String.Compare(line, 0, "MICSetting_DisplayDescription", 0, 29) == 0) moduleDescription = line.Substring(31);
                        if (String.Compare(line, 0, "MICSetting_DisplayModuleSelection", 0, 33) == 0) moduleString = line.Substring(35);
                        if (String.Compare(line, 0, "MICSetting_DisplayChannelSelection", 0, 34) == 0)
                        {
                            moduleChannel = Int32.Parse(line.Substring(36));
                            selectedMICDisplays.Add(new ModuleChannel(moduleString, moduleChannel, moduleDescription, moduleAxis));

                            moduleChannel = 0;
                            moduleString = "";
                            moduleAxis = "";
                            moduleDescription = "";
                        }

                    }
                }


                sr.Close();

                return new SettingMIC(chNumber,mic1Mute,mic2Mute,mic3Mute,mic4Mute,highPassFilter,syncTest, selectedMICDisplays);


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static SettingECG LoadSettingECG()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!path.EndsWith("\\")) path += "\\";
            path += "PolicardiographApp\\";
            try
            {
                int chNumber = 0;
                int moduleChannel = 0;
                int gain = 0;
                byte sensP = 0;
                byte sensN = 0;
                string ch4Mode = "";
                string moduleString = "";
                string moduleAxis = "";
                string moduleDescription = "";
                bool aVLeads = false;
                List<ModuleChannel> selectedECGDisplays = new List<ModuleChannel>();

                string line;
                if (!File.Exists(path + "Settings.dat"))
                {
                    selectedECGDisplays.Add(new ModuleChannel("ECG", 1, "LA-RA", ""));
                    selectedECGDisplays.Add(new ModuleChannel("ECG", 2, "LA-LL", ""));
                    selectedECGDisplays.Add(new ModuleChannel("ECG", 3, "V1", ""));
                    selectedECGDisplays.Add(new ModuleChannel("ECG", 4, "V2", ""));
                    selectedECGDisplays.Add(new ModuleChannel("ECG", 5, "V3", ""));
                    selectedECGDisplays.Add(new ModuleChannel("ECG", 6, "V4", ""));
                    selectedECGDisplays.Add(new ModuleChannel("ECG", 7, "V5", ""));
                    selectedECGDisplays.Add(new ModuleChannel("ECG", 8, "V6", ""));
                    return new SettingECG(8, false, 2, "NORMAL", 3, 3, selectedECGDisplays);
                }
                StreamReader sr = File.OpenText(path + "Settings.dat");
                
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (String.Compare(line, 0, "ECGSetting", 0, 10) == 0)
                    {
                        if (String.Compare(line, 0, "ECGSetting_Gain=", 0, 16) == 0) gain = Int32.Parse(line.Substring(16));
                        if (String.Compare(line, 0, "ECGSetting_CH4Mode=", 0, 19) == 0) ch4Mode = line.Substring(19);
                        if (String.Compare(line, 0, "ECGSetting_SensP=", 0, 17) == 0) sensP = Byte.Parse(line.Substring(17));
                        if (String.Compare(line, 0, "ECGSetting_SensN=", 0, 17) == 0) sensN = Byte.Parse(line.Substring(17));
                        if (String.Compare(line, 0, "ECGSetting_NoChannel=", 0, 21) == 0) chNumber = Int32.Parse(line.Substring(21));
                        if (String.Compare(line, 0, "ECGSetting_DisplayAxis", 0, 22) == 0) moduleAxis = line.Substring(24);
                        if (String.Compare(line, 0, "ECGSetting_DisplayDescription", 0, 29) == 0) moduleDescription = line.Substring(31);
                        if (String.Compare(line, 0, "ECGSetting_DisplayModuleSelection", 0, 33) == 0) moduleString = line.Substring(35);
                        if (String.Compare(line, 0, "ECGSetting_DisplayChannelSelection", 0, 34) == 0)
                        {
                            moduleChannel = Int32.Parse(line.Substring(36));
                            selectedECGDisplays.Add(new ModuleChannel(moduleString, moduleChannel, moduleDescription, moduleAxis));

                            moduleChannel = 0;
                            moduleString = "";
                            moduleAxis = "";
                            moduleDescription = "";
                        }
                        if (String.Compare(line, 0, "ECGSetting_DisplayAVLeads", 0, 25) == 0)
                        {
                            if (String.Compare(line.Substring(26), "True") == 0) aVLeads = true;
                            else aVLeads = false;
                        }

                    }
                }


                sr.Close();

                return new SettingECG(chNumber,aVLeads,gain,ch4Mode,sensP,sensN,selectedECGDisplays);


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static SettingACC LoadSettingACC()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!path.EndsWith("\\")) path += "\\";
            path += "PolicardiographApp\\";
            try
            {
                int chNumber = 0;
                int moduleChannel = 0;
                string moduleString = "";
                string moduleAxis = "";
                string moduleDescription = "";
                List<ModuleChannel> selectedACCDisplays = new List<ModuleChannel>();

                string line;
                if (!File.Exists(path + "Settings.dat"))
                {
                    selectedACCDisplays.Add(new ModuleChannel("ACC", 1, "", "z"));
                    selectedACCDisplays.Add(new ModuleChannel("ACC", 2, "", "z"));
                    return new SettingACC(2, selectedACCDisplays);
                }
                StreamReader sr = File.OpenText(path + "Settings.dat");
               
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (String.Compare(line, 0, "ACCSetting", 0, 10) == 0)
                    {
                        if (String.Compare(line, 0, "ACCSetting_NoChannel=", 0, 21) == 0) chNumber = Int32.Parse(line.Substring(21));
                        if (String.Compare(line, 0, "ACCSetting_DisplayAxis", 0, 22) == 0) moduleAxis = line.Substring(24);
                        if (String.Compare(line, 0, "ACCSetting_DisplayDescription", 0, 29) == 0) moduleDescription = line.Substring(31);
                        if (String.Compare(line, 0, "ACCSetting_DisplayModuleSelection", 0, 33) == 0) moduleString = line.Substring(35);
                        if (String.Compare(line, 0, "ACCSetting_DisplayChannelSelection", 0, 34) == 0)
                        {
                            moduleChannel = Int32.Parse(line.Substring(36));
                            selectedACCDisplays.Add(new ModuleChannel(moduleString, moduleChannel, moduleDescription, moduleAxis));

                            moduleChannel = 0;
                            moduleString = "";
                            moduleAxis = "";
                            moduleDescription = "";
                        }                        

                    }
                }


                sr.Close();

                return new SettingACC(chNumber, selectedACCDisplays);


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static SettingPPG LoadSettingPPG()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!path.EndsWith("\\")) path += "\\";
            path += "PolicardiographApp\\";
            try
            {
                int chNumber = 0;
                int moduleChannel = 0;
                string moduleString = "";
                string moduleAxis = "";
                string moduleDescription = "";
                List<ModuleChannel> selectedPPGDisplays = new List<ModuleChannel>();

                string line;
                if (!File.Exists(path + "Settings.dat"))
                {
                    selectedPPGDisplays.Add(new ModuleChannel("PPG", 1, "", "g"));
                    selectedPPGDisplays.Add(new ModuleChannel("PPG", 1, "", "r"));
                    selectedPPGDisplays.Add(new ModuleChannel("PPG", 2, "", "g"));
                    selectedPPGDisplays.Add(new ModuleChannel("PPG", 2, "", "r"));
                    return new SettingPPG(4, selectedPPGDisplays);
                }
                StreamReader sr = File.OpenText(path + "Settings.dat");
               
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (String.Compare(line, 0, "PPGSetting", 0, 10) == 0)
                    {
                        if (String.Compare(line, 0, "PPGSetting_NoChannel=", 0, 21) == 0) chNumber = Int32.Parse(line.Substring(21));
                        if (String.Compare(line, 0, "PPGSetting_DisplayAxis", 0, 22) == 0) moduleAxis = line.Substring(24);
                        if (String.Compare(line, 0, "PPGSetting_DisplayDescription", 0, 29) == 0) moduleDescription = line.Substring(31);
                        if (String.Compare(line, 0, "PPGSetting_DisplayModuleSelection", 0, 33) == 0) moduleString = line.Substring(35);
                        if (String.Compare(line, 0, "PPGSetting_DisplayChannelSelection", 0, 34) == 0)
                        {
                            moduleChannel = Int32.Parse(line.Substring(36));
                            selectedPPGDisplays.Add(new ModuleChannel(moduleString, moduleChannel, moduleDescription, moduleAxis));

                            moduleChannel = 0;
                            moduleString = "";
                            moduleAxis = "";
                            moduleDescription = "";
                        }

                    }
                }


                sr.Close();

                return new SettingPPG(chNumber, selectedPPGDisplays);


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static void StoreSetting(SettingProgram settingProgram, SettingWindow settingWindow, SettingFBGA settingFBGA, 
            SettingMIC settingMIC, SettingECG settingECG, SettingACC settingACC, SettingPPG settingPPG) {

            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!path.EndsWith("\\")) path += "\\";
            path += "PolicardiographApp\\";
            StreamWriter sw = File.CreateText(path + "Settings.dat");

            try
            {
                string line = String.Format("//ProgramSetting");
                string newline = sw.NewLine;
                sw.WriteLine(line);
                line = "ProgramSetting_SavePath=" + settingProgram.SavePath;
                sw.WriteLine(line);
                line = "ProgramSetting_BattMax=" + settingProgram.MaxBatteryVoltage.ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_BattMin=" + settingProgram.MinBatteryVoltage.ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_BattAlert=" + settingProgram.BatteryAlertVoltage.ToString(CultureInfo.InvariantCulture);
                sw.WriteLine(line);
                line = "ProgramSetting_BattLastTime=" + settingProgram.BatteryLastMeasuredTime.ToString() + newline;
                sw.WriteLine(line);

                line = String.Format("//MainWindowSetting");
                newline = sw.NewLine;
                sw.WriteLine(line);
                if (settingWindow.TimeAxis > 0)
                {
                    line = String.Format("MainWindowSetting_TimeAxis={0}", settingWindow.TimeAxis);
                    sw.WriteLine(line + newline);
                }

                

                for (int i = 0; i < settingWindow.UserTabSelectedDisplays.Count; i++) {
                    ModuleChannel tempModuleChannel = settingWindow.UserTabSelectedDisplays.ElementAt(i);
                    if (tempModuleChannel.Axis.Length > 0)
                    {
                        line = String.Format("MainWindowSetting_DisplayAxis{0}={1}", i + 1, tempModuleChannel.Axis);
                        sw.WriteLine(line);
                    }
                    if (tempModuleChannel.Description.Length > 0)
                    {
                        line = String.Format("MainWindowSetting_DisplayDescription{0}={1}", i + 1, tempModuleChannel.Description);
                        sw.WriteLine(line);
                    }
                    if (tempModuleChannel.ModuleName.Length > 0)
                    {
                        line = String.Format("MainWindowSetting_DisplayModuleSelection{0}={1}", i + 1, tempModuleChannel.ModuleName);
                        sw.WriteLine(line);
                    }
                    if (tempModuleChannel.ChannelNumber > 0)
                    {
                        line = String.Format("MainWindowSetting_DisplayChannelSelection{0}={1}", i + 1, tempModuleChannel.ChannelNumber);
                        sw.WriteLine(line + newline);
                    }
                }

                line = String.Format("//FBGASetting");
                sw.WriteLine(line);
                for (int i = 0; i < settingFBGA.SelectedDisplays.Count; i++)
                {
                    ModuleChannel tempModuleChannel = settingFBGA.SelectedDisplays.ElementAt(i);
                    if (tempModuleChannel.Axis.Length > 0)
                    {
                        line = String.Format("FBGASetting_DisplayAxis{0}={1}", i + 1, tempModuleChannel.Axis);
                        sw.WriteLine(line);
                    }
                    if (tempModuleChannel.Description.Length > 0)
                    {
                        line = String.Format("FBGASetting_DisplayDescription{0}={1}", i + 1, tempModuleChannel.Description);
                        sw.WriteLine(line);
                    }
                    if (tempModuleChannel.ModuleName.Length > 0)
                    {
                        line = String.Format("FBGASetting_DisplayModuleSelection{0}={1}", i + 1, tempModuleChannel.ModuleName);
                        sw.WriteLine(line);
                    }
                    if (tempModuleChannel.ChannelNumber > 0)
                    {
                        line = String.Format("FBGASetting_DisplayChannelSelection{0}={1}", i + 1, tempModuleChannel.ChannelNumber);
                        sw.WriteLine(line + newline);
                    }
                }
                if (settingFBGA.NumberOfChannels > 0)
                {
                    line = String.Format("FBGASetting_NoChannel={0}", settingFBGA.NumberOfChannels);
                    sw.WriteLine(line);
                }                                                                
                if (settingFBGA.IntegrationTime > 0)
                {
                    line = String.Format("FBGASetting_IntegrationTime={0}", settingFBGA.IntegrationTime);
                    sw.WriteLine(line);
                }
                if (settingFBGA.SLEDPower >= 0)
                {
                    line = String.Format(new System.Globalization.CultureInfo("en-GB"), "FBGASetting_SLEDPower={0:#0.0#}", settingFBGA.SLEDPower);
                    sw.WriteLine(line);
                }
                line = String.Format("FBGASetting_HighDynamicRange={0}", settingFBGA.HighDynamicRange);
                sw.WriteLine(line + newline);

                line = String.Format("//MICSetting");
                sw.WriteLine(line);
                for (int i = 0; i < settingMIC.SelectedDisplays.Count; i++)
                {
                    ModuleChannel tempModuleChannel = settingMIC.SelectedDisplays.ElementAt(i);
                    if (tempModuleChannel.Axis.Length > 0)
                    {
                        line = String.Format("MICSetting_DisplayAxis{0}={1}", i + 1, tempModuleChannel.Axis);
                        sw.WriteLine(line);
                    }
                    if (tempModuleChannel.Description.Length > 0)
                    {
                        line = String.Format("MICSetting_DisplayDescription{0}={1}", i + 1, tempModuleChannel.Description);
                        sw.WriteLine(line);
                    }
                    if (tempModuleChannel.ModuleName.Length > 0)
                    {
                        line = String.Format("MICSetting_DisplayModuleSelection{0}={1}", i + 1, tempModuleChannel.ModuleName);
                        sw.WriteLine(line);
                    }
                    if (tempModuleChannel.ChannelNumber > 0)
                    {
                        line = String.Format("MICSetting_DisplayChannelSelection{0}={1}", i + 1, tempModuleChannel.ChannelNumber);
                        sw.WriteLine(line + newline);
                    }
                }
                if (settingMIC.NumberOfChannels > 0)
                {
                    line = String.Format("MICSetting_NoChannel={0}", settingMIC.NumberOfChannels);
                    sw.WriteLine(line);
                }
                line = String.Format("MICSetting_MIC1Mute={0}", settingMIC.MuteMIC1);
                sw.WriteLine(line);
                line = String.Format("MICSetting_MIC2Mute={0}", settingMIC.MuteMIC2);
                sw.WriteLine(line);
                line = String.Format("MICSetting_MIC3Mute={0}", settingMIC.MuteMIC3);
                sw.WriteLine(line);
                line = String.Format("MICSetting_MIC4Mute={0}", settingMIC.MuteMIC4);
                sw.WriteLine(line);
                line = String.Format("MICSetting_HighPassFilter={0}", settingMIC.HighPassFilter);
                sw.WriteLine(line);
                line = String.Format("MICSetting_SyncTest={0}", settingMIC.SyncTest);
                sw.WriteLine(line+newline);               

                line = String.Format("//ECGSetting");
                sw.WriteLine(line);
                for (int i = 0; i < settingECG.SelectedDisplays.Count; i++)
                {
                    ModuleChannel tempModuleChannel = settingECG.SelectedDisplays.ElementAt(i);
                    if (tempModuleChannel.Axis.Length > 0)
                    {
                        line = String.Format("ECGSetting_DisplayAxis{0}={1}", i + 1, tempModuleChannel.Axis);
                        sw.WriteLine(line);
                    }
                    if (tempModuleChannel.Description.Length > 0)
                    {
                        line = String.Format("ECGSetting_DisplayDescription{0}={1}", i + 1, tempModuleChannel.Description);
                        sw.WriteLine(line);
                    }
                    if (tempModuleChannel.ModuleName.Length > 0)
                    {
                        line = String.Format("ECGSetting_DisplayModuleSelection{0}={1}", i + 1, tempModuleChannel.ModuleName);
                        sw.WriteLine(line);
                    }
                    if (tempModuleChannel.ChannelNumber > 0)
                    {
                        line = String.Format("ECGSetting_DisplayChannelSelection{0}={1}", i + 1, tempModuleChannel.ChannelNumber);
                        sw.WriteLine(line + newline);
                    }
                }
                if (settingECG.NumberOfChannels > 0)
                {
                    line = String.Format("ECGSetting_NoChannel={0}", settingECG.NumberOfChannels);
                    sw.WriteLine(line);
                }
                line = String.Format("ECGSetting_Gain={0}", settingECG.Gain);
                sw.WriteLine(line);
                line = "ECGSetting_CH4Mode=" + settingECG.CH4Mode;
                sw.WriteLine(line);
                line = String.Format("ECGSetting_SensP={0}", settingECG.SensP);                
                sw.WriteLine(line);
                line = String.Format("ECGSetting_SensN={0}", settingECG.SensN);
                sw.WriteLine(line);                
                line = String.Format("ECGSetting_DisplayAVLeads={0}", settingECG.AVLeads);
                sw.WriteLine(line + newline);
               

                line = String.Format("//ACCSetting");
                sw.WriteLine(line);
                for (int i = 0; i < settingACC.SelectedDisplays.Count; i++)
                {
                    ModuleChannel tempModuleChannel = settingACC.SelectedDisplays.ElementAt(i);
                    if (tempModuleChannel.Axis.Length > 0)
                    {
                        line = String.Format("ACCSetting_DisplayAxis{0}={1}", i + 1, tempModuleChannel.Axis);
                        sw.WriteLine(line);
                    }
                    if (tempModuleChannel.Description.Length > 0)
                    {
                        line = String.Format("ACCSetting_DisplayDescription{0}={1}", i + 1, tempModuleChannel.Description);
                        sw.WriteLine(line);
                    }
                    if (tempModuleChannel.ModuleName.Length > 0)
                    {
                        line = String.Format("ACCSetting_DisplayModuleSelection{0}={1}", i + 1, tempModuleChannel.ModuleName);
                        sw.WriteLine(line);
                    }
                    if (tempModuleChannel.ChannelNumber > 0)
                    {
                        line = String.Format("ACCSetting_DisplayChannelSelection{0}={1}", i + 1, tempModuleChannel.ChannelNumber);
                        sw.WriteLine(line + newline);
                    }
                }
                if (settingACC.NumberOfChannels > 0)
                {
                    line = String.Format("ACCSetting_NoChannel={0}", settingACC.NumberOfChannels/3);
                    sw.WriteLine(line + newline);
                }

                line = String.Format("//PPGSetting");
                sw.WriteLine(line);
                for (int i = 0; i < settingPPG.SelectedDisplays.Count; i++)
                {
                    ModuleChannel tempModuleChannel = settingPPG.SelectedDisplays.ElementAt(i);
                    if (tempModuleChannel.Axis.Length > 0)
                    {
                        line = String.Format("PPGSetting_DisplayAxis{0}={1}", i + 1, tempModuleChannel.Axis);
                        sw.WriteLine(line);
                    }
                    if (tempModuleChannel.Description.Length > 0)
                    {
                        line = String.Format("PPGSetting_DisplayDescription{0}={1}", i + 1, tempModuleChannel.Description);
                        sw.WriteLine(line);
                    }
                    if (tempModuleChannel.ModuleName.Length > 0)
                    {
                        line = String.Format("PPGSetting_DisplayModuleSelection{0}={1}", i + 1, tempModuleChannel.ModuleName);
                        sw.WriteLine(line);
                    }
                    if (tempModuleChannel.ChannelNumber > 0)
                    {
                        line = String.Format("PPGSetting_DisplayChannelSelection{0}={1}", i + 1, tempModuleChannel.ChannelNumber);
                        sw.WriteLine(line + newline);
                    }
                }
                if (settingPPG.NumberOfChannels > 0)
                {
                    line = String.Format("PPGSetting_NoChannel={0}", settingPPG.NumberOfChannels/2);
                    sw.WriteLine(line + newline);
                }
                sw.Close();
            }
            finally{
                sw.Close();
                
            }

        }
        
    }
}
