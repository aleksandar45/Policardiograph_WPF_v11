using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Globalization;
using Policardiograph_App.ViewModel;
using Policardiograph_App.Dialogs.DialogService;
using Policardiograph_App.Settings;

namespace Policardiograph_App.Dialogs.DialogSetting
{
    public class DialogSettingViewModel: DialogViewModelBase
    {
       
        public DialogSettingViewModel(SettingWindow settingWindow, SettingFBGA settingFBGA, SettingMIC settingMIC, SettingECG settingECG) : base("Settings") {
            this.okCommand = new DelegateCommand(OnOKClicked);
            this.cancelCommand = new DelegateCommand(OnCancelClicked);

            TimeAxisSelectedItem = settingWindow.TimeAxis.ToString();
            #region DISPLAY_CH_NUMBERS
            int micNoChannels =  settingWindow.UserTabDisplays.ElementAt(0);
            int fbgaNoChannels = settingWindow.UserTabDisplays.ElementAt(1);
            int ecgNoChannels = settingWindow.UserTabDisplays.ElementAt(2);
            int accNoChannels = settingWindow.UserTabDisplays.ElementAt(3);
            int ppgNoChannels = settingWindow.UserTabDisplays.ElementAt(4);
            string[] items = new string[micNoChannels + fbgaNoChannels + ecgNoChannels + 3*accNoChannels + ppgNoChannels];
            int index = 0;
            for (int i = 0; i < micNoChannels; i++) {
                items[index++] = "MIC_CH" + (i + 1).ToString();
            }
            for (int i = 0; i < fbgaNoChannels; i++)
            {
                items[index++] = "FBGA_CH" + (i + 1).ToString();
            }
            for (int i = 0; i < ecgNoChannels; i++)
            {
                items[index++] = "ECG_CH" + (i + 1).ToString();
            }
            for (int i = 0; i < accNoChannels; i++)
            {
                items[index++] = "ACC_CH" + (i + 1).ToString() + "x";
            }
            for (int i = 0; i < accNoChannels; i++)
            {
                items[index++] = "ACC_CH" + (i + 1).ToString() + "y";
            }
            for (int i = 0; i < accNoChannels; i++)
            {
                items[index++] = "ACC_CH" + (i + 1).ToString() + "z";
            }
            for (int i = 0; i < ppgNoChannels; i++)
            {
                items[index++] = "PPG_CH" + (i + 1).ToString();
            }
            UserTabDisplay1 = new List<string>(items);
            UserTabDisplay2 = new List<string>(items);
            UserTabDisplay3 = new List<string>(items);
            UserTabDisplay4 = new List<string>(items);
            UserTabDisplay5 = new List<string>(items);
            UserTabDisplay6 = new List<string>(items);

            #endregion
            #region DISPLAY_SELECTION
            display1Selection = new ModuleChannel( settingWindow.UserTabSelectedDisplays.ElementAt(0));
            if (String.Compare("MIC", display1Selection.ModuleName) == 0) UserTabDisplay1SelectedItem = "MIC_CH" + display1Selection.ChannelNumber.ToString() + display1Selection.Axis;
            if (String.Compare("FBGA", display1Selection.ModuleName) == 0) UserTabDisplay1SelectedItem = "FBGA_CH" + display1Selection.ChannelNumber.ToString() + display1Selection.Axis;
            if (String.Compare("ECG", display1Selection.ModuleName) == 0) UserTabDisplay1SelectedItem = "ECG_CH" + display1Selection.ChannelNumber.ToString() + display1Selection.Axis;
            if (String.Compare("ACC", display1Selection.ModuleName) == 0) UserTabDisplay1SelectedItem = "ACC_CH" + display1Selection.ChannelNumber.ToString() + display1Selection.Axis;
            if (String.Compare("PPG", display1Selection.ModuleName) == 0) UserTabDisplay1SelectedItem = "PPG_CH" + display1Selection.ChannelNumber.ToString() + display1Selection.Axis;

            display2Selection = new ModuleChannel(settingWindow.UserTabSelectedDisplays.ElementAt(1));
            if (String.Compare("MIC", display2Selection.ModuleName) == 0) UserTabDisplay2SelectedItem = "MIC_CH" + display2Selection.ChannelNumber.ToString() + display2Selection.Axis;
            if (String.Compare("FBGA", display2Selection.ModuleName) == 0) UserTabDisplay2SelectedItem = "FBGA_CH" + display2Selection.ChannelNumber.ToString() + display2Selection.Axis;
            if (String.Compare("ECG", display2Selection.ModuleName) == 0) UserTabDisplay2SelectedItem = "ECG_CH" + display2Selection.ChannelNumber.ToString() + display2Selection.Axis;
            if (String.Compare("ACC", display2Selection.ModuleName) == 0) UserTabDisplay2SelectedItem = "ACC_CH" + display2Selection.ChannelNumber.ToString() + display2Selection.Axis;
            if (String.Compare("PPG", display2Selection.ModuleName) == 0) UserTabDisplay2SelectedItem = "PPG_CH" + display2Selection.ChannelNumber.ToString() + display2Selection.Axis;

            display3Selection = new ModuleChannel( settingWindow.UserTabSelectedDisplays.ElementAt(2));
            if (String.Compare("MIC", display3Selection.ModuleName) == 0) UserTabDisplay3SelectedItem = "MIC_CH" + display3Selection.ChannelNumber.ToString() + display3Selection.Axis;
            if (String.Compare("FBGA", display3Selection.ModuleName) == 0) UserTabDisplay3SelectedItem = "FBGA_CH" + display3Selection.ChannelNumber.ToString() + display3Selection.Axis;
            if (String.Compare("ECG", display3Selection.ModuleName) == 0) UserTabDisplay3SelectedItem = "ECG_CH" + display3Selection.ChannelNumber.ToString() + display3Selection.Axis;
            if (String.Compare("ACC", display3Selection.ModuleName) == 0) UserTabDisplay3SelectedItem = "ACC_CH" + display3Selection.ChannelNumber.ToString() + display3Selection.Axis;
            if (String.Compare("PPG", display3Selection.ModuleName) == 0) UserTabDisplay3SelectedItem = "PPG_CH" + display3Selection.ChannelNumber.ToString() + display3Selection.Axis;

            display4Selection = new ModuleChannel( settingWindow.UserTabSelectedDisplays.ElementAt(3));
            if (String.Compare("MIC", display4Selection.ModuleName) == 0) UserTabDisplay4SelectedItem = "MIC_CH" + display4Selection.ChannelNumber.ToString() + display4Selection.Axis;
            if (String.Compare("FBGA", display4Selection.ModuleName) == 0) UserTabDisplay4SelectedItem = "FBGA_CH" + display4Selection.ChannelNumber.ToString() + display4Selection.Axis;
            if (String.Compare("ECG", display4Selection.ModuleName) == 0) UserTabDisplay4SelectedItem = "ECG_CH" + display4Selection.ChannelNumber.ToString() + display4Selection.Axis;
            if (String.Compare("ACC", display4Selection.ModuleName) == 0) UserTabDisplay4SelectedItem = "ACC_CH" + display4Selection.ChannelNumber.ToString() + display4Selection.Axis;
            if (String.Compare("PPG", display4Selection.ModuleName) == 0) UserTabDisplay4SelectedItem = "PPG_CH" + display4Selection.ChannelNumber.ToString() + display4Selection.Axis;

            display5Selection = new ModuleChannel(settingWindow.UserTabSelectedDisplays.ElementAt(4));
            if (String.Compare("MIC", display5Selection.ModuleName) == 0) UserTabDisplay5SelectedItem = "MIC_CH" + display5Selection.ChannelNumber.ToString() + display5Selection.Axis;
            if (String.Compare("FBGA", display5Selection.ModuleName) == 0) UserTabDisplay5SelectedItem = "FBGA_CH" + display5Selection.ChannelNumber.ToString() + display5Selection.Axis;
            if (String.Compare("ECG", display5Selection.ModuleName) == 0) UserTabDisplay5SelectedItem = "ECG_CH" + display5Selection.ChannelNumber.ToString() + display5Selection.Axis;
            if (String.Compare("ACC", display5Selection.ModuleName) == 0) UserTabDisplay5SelectedItem = "ACC_CH" + display5Selection.ChannelNumber.ToString() + display5Selection.Axis;
            if (String.Compare("PPG", display5Selection.ModuleName) == 0) UserTabDisplay5SelectedItem = "PPG_CH" + display5Selection.ChannelNumber.ToString() + display5Selection.Axis;

            display6Selection = new ModuleChannel(settingWindow.UserTabSelectedDisplays.ElementAt(5));
            if (String.Compare("MIC", display6Selection.ModuleName) == 0) UserTabDisplay6SelectedItem = "MIC_CH" + display6Selection.ChannelNumber.ToString() + display6Selection.Axis;
            if (String.Compare("FBGA", display6Selection.ModuleName) == 0) UserTabDisplay6SelectedItem = "FBGA_CH" + display6Selection.ChannelNumber.ToString() + display6Selection.Axis;
            if (String.Compare("ECG", display6Selection.ModuleName) == 0) UserTabDisplay6SelectedItem = "ECG_CH" + display6Selection.ChannelNumber.ToString() + display6Selection.Axis;
            if (String.Compare("ACC", display6Selection.ModuleName) == 0) UserTabDisplay6SelectedItem = "ACC_CH" + display6Selection.ChannelNumber.ToString() + display6Selection.Axis;
            if (String.Compare("PPG", display6Selection.ModuleName) == 0) UserTabDisplay6SelectedItem = "PPG_CH" + display6Selection.ChannelNumber.ToString() + display6Selection.Axis;
            #endregion       
            this.settingWindow = settingWindow;            
            TabWindowEnabled = settingWindow.TabEnabled;
            if (!TabWindowEnabled) TabFBGASelected = true;

            this.settingFBGA = settingFBGA;
            FbgaIntegrationTimeSelectedItem = settingFBGA.IntegrationTime.ToString();
            SLEDPowerSelectedItem = String.Format(new System.Globalization.CultureInfo("en-GB"), "{0:#0.0#}", settingFBGA.SLEDPower);
            HighDynamicRangeModeChecked = settingFBGA.HighDynamicRange;

            this.settingMIC = settingMIC;
            MIC1MuteSelected = settingMIC.MuteMIC1;
            MIC2MuteSelected = settingMIC.MuteMIC2;
            MIC3MuteSelected = settingMIC.MuteMIC3;
            MIC4MuteSelected = settingMIC.MuteMIC4;
            HighPassFilterSelected = settingMIC.HighPassFilter;

            this.settingECG = settingECG;
            EcgGainSelectedItem = settingECG.Gain.ToString();
            ECGCH4modeSelectedItem = settingECG.CH4Mode;
            #region SENSP_SELECTION
            if ((settingECG.SensP & 0x80) == 0x80) SensP8Selected = true;
            else SensP8Selected = false;
            if ((settingECG.SensP & 0x40) == 0x40) SensP7Selected = true;
            else SensP7Selected = false;
            if ((settingECG.SensP & 0x20) == 0x20) SensP6Selected = true;
            else SensP6Selected = false;
            if ((settingECG.SensP & 0x10) == 0x10) SensP5Selected = true;
            else SensP5Selected = false;
            if ((settingECG.SensP & 0x08) == 0x08) SensP4Selected = true;
            else SensP4Selected = false;
            if ((settingECG.SensP & 0x04) == 0x04) SensP3Selected = true;
            else SensP3Selected = false;
            if ((settingECG.SensP & 0x02) == 0x02) SensP2Selected = true;
            else SensP2Selected = false;
            if ((settingECG.SensP & 0x01) == 0x01) SensP1Selected = true;
            else SensP1Selected = false;
            #endregion
            #region SENSN_SELECTION
            if ((settingECG.SensN & 0x80) == 0x80) SensN8Selected = true;
            else SensN8Selected = false;
            if ((settingECG.SensN & 0x40) == 0x40) SensN7Selected = true;
            else SensN7Selected = false;
            if ((settingECG.SensN & 0x20) == 0x20) SensN6Selected = true;
            else SensN6Selected = false;
            if ((settingECG.SensN & 0x10) == 0x10) SensN5Selected = true;
            else SensN5Selected = false;
            if ((settingECG.SensN & 0x08) == 0x08) SensN4Selected = true;
            else SensN4Selected = false;
            if ((settingECG.SensN & 0x04) == 0x04) SensN3Selected = true;
            else SensN3Selected = false;
            if ((settingECG.SensN & 0x02) == 0x02) SensN2Selected = true;
            else SensN2Selected = false;
            if ((settingECG.SensN & 0x01) == 0x01) SensN1Selected = true;
            else SensN1Selected = false;
            #endregion
        }

        #region TAB_WINDOW
        private SettingWindow settingWindow=null;

        private bool _tabWindowEnabled;
        public bool TabWindowEnabled {
            get {
                return _tabWindowEnabled;
            }
            set {
                _tabWindowEnabled = value;
                OnPropertyChanged("TabWindowEnabled");
            }
        }

        private bool _tabWindowSelected = true;
        public bool TabWindowSelected
        {
            get {
                return _tabWindowSelected;
            }
            set {
                _tabWindowSelected = value;
                OnPropertyChanged("TabWindowSelected");
            }
        }

        
        #region COMBOBOX_TIME_AXIS
        private List<string> _timeAxis = new List<string>(new string[] { "4", "6", "8", "10"});
        public List<string> TimeAxis { 
            get{
                return _timeAxis;
            }
            set{
                _timeAxis = value;
                OnPropertyChanged("TimeAxis");
            }
        }
        private string _timeAxisselectedItem;
        public string TimeAxisSelectedItem{
            get {
                return _timeAxisselectedItem;
            }
            set {
                _timeAxisselectedItem = value;
                OnPropertyChanged("TimeAxisSelectedItem");
            }
        }
        #endregion

        #region COMBOBOx_USER_TAB_DISPLAY1
        private ModuleChannel display1Selection;
        private List<string> _userTabDisplay1;
        public List<string> UserTabDisplay1 {
            get {
                return _userTabDisplay1;
            }
            set {
                _userTabDisplay1 = value;
                OnPropertyChanged("UserTabDisplay1");
            }
        }
        private string __userTabDisplay1selectedItem;
        public string UserTabDisplay1SelectedItem
        {
            get
            {
                return __userTabDisplay1selectedItem;
            }
            set
            {
                __userTabDisplay1selectedItem = value;
                if (String.Compare(__userTabDisplay1selectedItem,0, "MIC_CH", 0, 6 ) == 0) {
                    display1Selection.ModuleName = "MIC";
                    display1Selection.ChannelNumber = Int32.Parse(__userTabDisplay1selectedItem.Substring(6));
                    display1Selection.Axis = "";
                    display1Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay1selectedItem, 0, "FBGA_CH", 0, 7) == 0)
                {
                    display1Selection.ModuleName = "FBGA";
                    display1Selection.ChannelNumber = Int32.Parse(__userTabDisplay1selectedItem.Substring(7));
                    display1Selection.Axis = "";
                    display1Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay1selectedItem, 0, "ECG_CH", 0, 6) == 0)
                {
                    display1Selection.ModuleName = "ECG";
                    display1Selection.ChannelNumber = Int32.Parse(__userTabDisplay1selectedItem.Substring(6));
                    display1Selection.Axis = "";
                    display1Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay1selectedItem, 0, "ACC_CH", 0, 6) == 0)
                {
                    display1Selection.ModuleName = "ACC";
                    display1Selection.ChannelNumber = Int32.Parse(__userTabDisplay1selectedItem.Substring(6,1));
                    display1Selection.Axis = __userTabDisplay1selectedItem.Substring(7, 1);                    
                    display1Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay1selectedItem, 0, "PPG_CH", 0, 6) == 0)
                {
                    display1Selection.ModuleName = "PPG";
                    display1Selection.ChannelNumber = Int32.Parse(__userTabDisplay1selectedItem.Substring(6));
                    display1Selection.Axis = "";
                    display1Selection.Description = "";
                }
                OnPropertyChanged("UserTabDisplay1SelectedItem");
            }
        }
        #endregion

        #region COMBOBOx_USER_TAB_DISPLAY2
        private ModuleChannel display2Selection;
        private List<string> _userTabDisplay2;
        public List<string> UserTabDisplay2
        {
            get
            {
                return _userTabDisplay2;
            }
            set
            {
                _userTabDisplay2 = value;                
                OnPropertyChanged("UserTabDisplay2");
            }
        }
        private string __userTabDisplay2selectedItem;
        public string UserTabDisplay2SelectedItem
        {
            get
            {
                return __userTabDisplay2selectedItem;
            }
            set
            {
                __userTabDisplay2selectedItem = value;
                if (String.Compare(__userTabDisplay2selectedItem, 0, "MIC_CH", 0, 6) == 0)
                {
                    display2Selection.ModuleName = "MIC";
                    display2Selection.ChannelNumber = Int32.Parse(__userTabDisplay2selectedItem.Substring(6));
                    display2Selection.Axis = "";
                    display2Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay2selectedItem, 0, "FBGA_CH", 0, 7) == 0)
                {
                    display2Selection.ModuleName = "FBGA";
                    display2Selection.ChannelNumber = Int32.Parse(__userTabDisplay2selectedItem.Substring(7));
                    display2Selection.Axis = "";
                    display2Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay2selectedItem, 0, "ECG_CH", 0, 6) == 0)
                {
                    display2Selection.ModuleName = "ECG";
                    display2Selection.ChannelNumber = Int32.Parse(__userTabDisplay2selectedItem.Substring(6));
                    display2Selection.Axis = "";
                    display2Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay2selectedItem, 0, "ACC_CH", 0, 6) == 0)
                {
                    display2Selection.ModuleName = "ACC";
                    display2Selection.ChannelNumber = Int32.Parse(__userTabDisplay2selectedItem.Substring(6,1));
                    display2Selection.Axis = __userTabDisplay2selectedItem.Substring(7, 1);                    
                    display2Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay2selectedItem, 0, "PPG_CH", 0, 6) == 0)
                {
                    display2Selection.ModuleName = "PPG";
                    display2Selection.ChannelNumber = Int32.Parse(__userTabDisplay2selectedItem.Substring(6));
                    display2Selection.Axis = "";
                    display2Selection.Description = "";
                }
                OnPropertyChanged("UserTabDisplay2SelectedItem");
            }
        }
        #endregion

        #region COMBOBOx_USER_TAB_DISPLAY3
        private ModuleChannel display3Selection;
        private List<string> _userTabDisplay3;
        public List<string> UserTabDisplay3
        {
            get
            {
                return _userTabDisplay3;
            }
            set
            {
                _userTabDisplay3 = value;                
                OnPropertyChanged("UserTabDisplay3");
            }
        }
        private string __userTabDisplay3selectedItem;
        public string UserTabDisplay3SelectedItem
        {
            get
            {
                return __userTabDisplay3selectedItem;
            }
            set
            {
                __userTabDisplay3selectedItem = value;
                if (String.Compare(__userTabDisplay3selectedItem, 0, "MIC_CH", 0, 6) == 0)
                {
                    display3Selection.ModuleName = "MIC";
                    display3Selection.ChannelNumber = Int32.Parse(__userTabDisplay3selectedItem.Substring(6));
                    display3Selection.Axis = "";
                    display3Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay3selectedItem, 0, "FBGA_CH", 0, 7) == 0)
                {
                    display3Selection.ModuleName = "FBGA";
                    display3Selection.ChannelNumber = Int32.Parse(__userTabDisplay3selectedItem.Substring(7));
                    display3Selection.Axis = "";
                    display3Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay3selectedItem, 0, "ECG_CH", 0, 6) == 0)
                {
                    display3Selection.ModuleName = "ECG";
                    display3Selection.ChannelNumber = Int32.Parse(__userTabDisplay3selectedItem.Substring(6));
                    display3Selection.Axis = "";
                    display3Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay3selectedItem, 0, "ACC_CH", 0, 6) == 0)
                {
                    display3Selection.ModuleName = "ACC";
                    display3Selection.ChannelNumber = Int32.Parse(__userTabDisplay3selectedItem.Substring(6,1));
                    display3Selection.Axis = __userTabDisplay3selectedItem.Substring(7, 1);                    
                    display3Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay3selectedItem, 0, "PPG_CH", 0, 6) == 0)
                {
                    display3Selection.ModuleName = "PPG";
                    display3Selection.ChannelNumber = Int32.Parse(__userTabDisplay3selectedItem.Substring(6));
                    display3Selection.Axis = "";
                    display3Selection.Description = "";
                }
                OnPropertyChanged("UserTabDisplay3SelectedItem");
            }
        }
        #endregion

        #region COMBOBOx_USER_TAB_DISPLAY4
        private ModuleChannel display4Selection;
        private List<string> _userTabDisplay4;
        public List<string> UserTabDisplay4
        {
            get
            {
                return _userTabDisplay4;
            }
            set
            {
                _userTabDisplay4 = value;                
                OnPropertyChanged("UserTabDisplay4");
            }
        }
        private string __userTabDisplay4selectedItem;
        public string UserTabDisplay4SelectedItem
        {
            get
            {
                return __userTabDisplay4selectedItem;
            }
            set
            {
                __userTabDisplay4selectedItem = value;
                if (String.Compare(__userTabDisplay4selectedItem, 0, "MIC_CH", 0, 6) == 0)
                {
                    display4Selection.ModuleName = "MIC";
                    display4Selection.ChannelNumber = Int32.Parse(__userTabDisplay4selectedItem.Substring(6));
                    display4Selection.Axis = "";
                    display4Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay4selectedItem, 0, "FBGA_CH", 0, 7) == 0)
                {
                    display4Selection.ModuleName = "FBGA";
                    display4Selection.ChannelNumber = Int32.Parse(__userTabDisplay4selectedItem.Substring(7));
                    display4Selection.Axis = "";
                    display4Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay4selectedItem, 0, "ECG_CH", 0, 6) == 0)
                {
                    display4Selection.ModuleName = "ECG";
                    display4Selection.ChannelNumber = Int32.Parse(__userTabDisplay4selectedItem.Substring(6));
                    display4Selection.Axis = "";
                    display4Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay4selectedItem, 0, "ACC_CH", 0, 6) == 0)
                {
                    display4Selection.ModuleName = "ACC";
                    display4Selection.ChannelNumber = Int32.Parse(__userTabDisplay4selectedItem.Substring(6,1));
                    display4Selection.Axis = __userTabDisplay4selectedItem.Substring(7, 1);                    
                    display4Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay4selectedItem, 0, "PPG_CH", 0, 6) == 0)
                {
                    display4Selection.ModuleName = "PPG";
                    display4Selection.ChannelNumber = Int32.Parse(__userTabDisplay4selectedItem.Substring(6));
                    display4Selection.Axis = "";
                    display4Selection.Description = "";
                }
                OnPropertyChanged("UserTabDisplay4SelectedItem");
            }
        }
        #endregion

        #region COMBOBOx_USER_TAB_DISPLAY5
        private ModuleChannel display5Selection;
        private List<string> _userTabDisplay5;
        public List<string> UserTabDisplay5
        {
            get
            {
                return _userTabDisplay5;
            }
            set
            {
                _userTabDisplay5 = value;                
                OnPropertyChanged("UserTabDisplay5");
            }
        }
        private string __userTabDisplay5selectedItem;
        public string UserTabDisplay5SelectedItem
        {
            get
            {
                return __userTabDisplay5selectedItem;
            }
            set
            {
                __userTabDisplay5selectedItem = value;
                if (String.Compare(__userTabDisplay5selectedItem, 0, "MIC_CH", 0, 6) == 0)
                {
                    display5Selection.ModuleName = "MIC";
                    display5Selection.ChannelNumber = Int32.Parse(__userTabDisplay5selectedItem.Substring(6));
                    display5Selection.Axis = "";
                    display5Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay5selectedItem, 0, "FBGA_CH", 0, 7) == 0)
                {
                    display5Selection.ModuleName = "FBGA";
                    display5Selection.ChannelNumber = Int32.Parse(__userTabDisplay5selectedItem.Substring(7));
                    display5Selection.Axis = "";
                    display5Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay5selectedItem, 0, "ECG_CH", 0, 6) == 0)
                {
                    display5Selection.ModuleName = "ECG";
                    display5Selection.ChannelNumber = Int32.Parse(__userTabDisplay5selectedItem.Substring(6));
                    display5Selection.Axis = "";
                    display5Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay5selectedItem, 0, "ACC_CH", 0, 6) == 0)
                {
                    display5Selection.ModuleName = "ACC";
                    display5Selection.ChannelNumber = Int32.Parse(__userTabDisplay5selectedItem.Substring(6,1));
                    display5Selection.Axis = __userTabDisplay5selectedItem.Substring(7, 1);                    
                    display5Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay5selectedItem, 0, "PPG_CH", 0, 6) == 0)
                {
                    display5Selection.ModuleName = "PPG";
                    display5Selection.ChannelNumber = Int32.Parse(__userTabDisplay5selectedItem.Substring(6));
                    display5Selection.Axis = "";
                    display5Selection.Description = "";
                }
                OnPropertyChanged("UserTabDisplay5SelectedItem");
            }
        }
        #endregion

        #region COMBOBOx_USER_TAB_DISPLAY6
        private ModuleChannel display6Selection;
        private List<string> _userTabDisplay6;
        public List<string> UserTabDisplay6
        {
            get
            {
                return _userTabDisplay6;
            }
            set
            {
                _userTabDisplay6 = value;                
                OnPropertyChanged("UserTabDisplay6");
            }
        }
        private string __userTabDisplay6selectedItem;
        public string UserTabDisplay6SelectedItem
        {
            get
            {
                return __userTabDisplay6selectedItem;
            }
            set
            {
                __userTabDisplay6selectedItem = value;
                if (String.Compare(__userTabDisplay6selectedItem, 0, "MIC_CH", 0, 6) == 0)
                {
                    display6Selection.ModuleName = "MIC";
                    display6Selection.ChannelNumber = Int32.Parse(__userTabDisplay6selectedItem.Substring(6));
                    display6Selection.Axis = "";
                    display6Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay6selectedItem, 0, "FBGA_CH", 0, 7) == 0)
                {
                    display6Selection.ModuleName = "FBGA";
                    display6Selection.ChannelNumber = Int32.Parse(__userTabDisplay6selectedItem.Substring(7));
                    display6Selection.Axis = "";
                    display6Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay6selectedItem, 0, "ECG_CH", 0, 6) == 0)
                {
                    display6Selection.ModuleName = "ECG";
                    display6Selection.ChannelNumber = Int32.Parse(__userTabDisplay6selectedItem.Substring(6));
                    display6Selection.Axis = "";
                    display6Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay6selectedItem, 0, "ACC_CH", 0, 6) == 0)
                {
                    display6Selection.ModuleName = "ACC";
                    display6Selection.ChannelNumber = Int32.Parse(__userTabDisplay6selectedItem.Substring(6,1));
                    display6Selection.Axis = __userTabDisplay6selectedItem.Substring(7, 1);                    
                    display6Selection.Description = "";
                }
                if (String.Compare(__userTabDisplay6selectedItem, 0, "PPG_CH", 0, 6) == 0)
                {
                    display6Selection.ModuleName = "PPG";
                    display6Selection.ChannelNumber = Int32.Parse(__userTabDisplay6selectedItem.Substring(6));
                    display6Selection.Axis = "";
                    display6Selection.Description = "";
                }
                OnPropertyChanged("UserTabDisplay6SelectedItem");
            }
        }
        #endregion

        #endregion

        #region TAB_FBGA

        private SettingFBGA settingFBGA = null;

        private bool _tabFBGAEnabled = true;
        public bool TabFBGAEnabled
        {
            get
            {
                return _tabFBGAEnabled;
            }
            set
            {
                _tabFBGAEnabled = value;
                OnPropertyChanged("TabFBGAEnabled");
            }
        }

        private bool _tabFBGASelected = false;
        public bool TabFBGASelected
        {
            get
            {
                return _tabFBGASelected;
            }
            set
            {
                _tabFBGASelected = value;
                OnPropertyChanged("TabFBGASelected");
            }
        }

        #region COMBOBOx_FBGA_INTEGRATION_TIME
        private int integrationTime;
        private List<string> _fbgaIntegrationTime = new List<string>(new string[] { "50", "100", "500", "1000"});
        public List<string> FbgaIntegrationTime
        {
            get
            {
                return _fbgaIntegrationTime;
            }
            set
            {
                _fbgaIntegrationTime = value;
                OnPropertyChanged("FbgaIntegrationTime");
            }
        }
        private string _fbgaIntegrationTimeSelectedItem;
        public string FbgaIntegrationTimeSelectedItem
        {
            get
            {
                return _fbgaIntegrationTimeSelectedItem;
            }
            set
            {
                _fbgaIntegrationTimeSelectedItem = value;
                integrationTime = Int32.Parse(_fbgaIntegrationTimeSelectedItem);                
                OnPropertyChanged("FbgaIntegrationTimeSelectedItem");
            }
        }
        #endregion

        #region COMBOBOx_FBGA_SLED_POWER
        private double sledPower;
        private List<string> _sledPower = new List<string>(new string[] { "0.2", "0.5", "1.0", "2.0", "3.0", "4.0", "5.0"});
        public List<string> SLEDPower
        {
            get
            {
                return _sledPower;
            }
            set
            {
                _sledPower = value;
                OnPropertyChanged("SLEDPower");
            }
        }
        private string _sledPowerSelectedItem;
        public string SLEDPowerSelectedItem
        {
            get
            {
                return _sledPowerSelectedItem;
            }
            set
            {
                _sledPowerSelectedItem = value;
                sledPower = Double.Parse(_sledPowerSelectedItem, CultureInfo.InvariantCulture);
                OnPropertyChanged("FbgaIntegrationTimeSelectedItem");
            }
        }
        #endregion

        #region RADIO_BUTTON_HIGH_DYNAMIC_MODE
        private bool _highDynamicRangeMode = false;
        public bool HighDynamicRangeModeChecked {
            get {
                return _highDynamicRangeMode;
            }
            set {
                _highDynamicRangeMode = value;
                OnPropertyChanged("HighDynamicRangeModeChecked");
            }
        }
        public bool HighSensitivityModeChecked
        {
            get
            {
                return !_highDynamicRangeMode;
            }
            set
            {
                _highDynamicRangeMode = !value;
                OnPropertyChanged("HighSensitivityModeChecked");
            }
        }
        #endregion
        #endregion

        #region TAB_MIC

        private SettingMIC settingMIC = null;

        private bool _mic1Mute;
        public bool MIC1MuteSelected
        {
            get {
                return _mic1Mute;
            }
            set {
                _mic1Mute = value;
                OnPropertyChanged("MIC1MuteSelected");
            }
        }
        private bool _mic2Mute;
        public bool MIC2MuteSelected
        {
            get
            {
                return _mic2Mute;
            }
            set
            {
                _mic2Mute = value;
                OnPropertyChanged("MIC2MuteSelected");
            }
        }
        private bool _mic3Mute;
        public bool MIC3MuteSelected
        {
            get
            {
                return _mic3Mute;
            }
            set
            {
                _mic3Mute = value;
                OnPropertyChanged("MIC3MuteSelected");
            }
        }
        private bool _mic4Mute;
        public bool MIC4MuteSelected
        {
            get
            {
                return _mic4Mute;
            }
            set
            {
                _mic4Mute = value;
                OnPropertyChanged("MIC4MuteSelected");
            }
        }
        private bool _highPassFilter;
        public bool HighPassFilterSelected
        {
            get
            {
                return _highPassFilter;
            }
            set
            {
                _highPassFilter = value;
                OnPropertyChanged("HighPassFilterSelected");
            }
        }

        #endregion

        #region TAB_ECG
        private SettingECG settingECG = null;

        private int ecgGain;
        private List<string> _ecgGain = new List<string>(new string[] { "1", "2", "3","4" ,"6" ,"8","12"});
        public List<string> EcgGain
        {
            get
            {
                return _ecgGain;
            }
            set
            {
                _ecgGain = value;
                OnPropertyChanged("EcgGain");
            }
        }
        private string _ecgGainSelectedItem;
        public string EcgGainSelectedItem
        {
            get
            {
                return _ecgGainSelectedItem;
            }
            set
            {
                _ecgGainSelectedItem = value;
                ecgGain = Int32.Parse(_ecgGainSelectedItem);
                OnPropertyChanged("EcgGainSelectedItem");
            }
        }
        
        private List<string> _ecgCH4mode = new List<string>(new string[] { "NORMAL", "RLD_IN", "TEST", "MVDD" });
        public List<string> ECGCH4mode
        {
            get
            {
                return _ecgCH4mode;
            }
            set
            {
                _ecgCH4mode = value;
                OnPropertyChanged("ECGCH4mode");
            }
        }
        private string _ecgCH4modeSelectedItem;
        public string ECGCH4modeSelectedItem
        {
            get
            {
                return _ecgCH4modeSelectedItem;
            }
            set
            {
                _ecgCH4modeSelectedItem = value;                
                OnPropertyChanged("ECGCH4modeSelectedItem");
            }
        }

        private byte sensP;
        private bool _sensP8;
        public bool SensP8Selected {
            get {
                return _sensP8;
            }
            set {
                _sensP8 = value;
                if (_sensP8) sensP = (byte)(sensP | 0x80);
                else sensP = (byte)(sensP & 0x7F);
                OnPropertyChanged("SensP8Selected");
            }
        }
        private bool _sensP7;
        public bool SensP7Selected
        {
            get
            {
                return _sensP7;
            }
            set
            {
                _sensP7 = value;
                if (_sensP7) sensP = (byte)(sensP | 0x40);
                else sensP = (byte)(sensP & 0xBF);
                OnPropertyChanged("SensP7Selected");
            }
        }
        private bool _sensP6;
        public bool SensP6Selected
        {
            get
            {
                return _sensP6;
            }
            set
            {
                _sensP6 = value;
                if (_sensP6) sensP = (byte)(sensP | 0x20);
                else sensP = (byte)(sensP & 0xDF);
                OnPropertyChanged("SensP6Selected");
            }
        }
        private bool _sensP5;
        public bool SensP5Selected
        {
            get
            {
                return _sensP5;
            }
            set
            {
                _sensP5 = value;
                if (_sensP5) sensP = (byte)(sensP | 0x10);
                else sensP = (byte)(sensP & 0xEF);
                OnPropertyChanged("SensP5Selected");
            }
        }
        private bool _sensP4;
        public bool SensP4Selected
        {
            get
            {
                return _sensP4;
            }
            set
            {
                _sensP4 = value;
                if (_sensP4) sensP = (byte)(sensP | 0x08);
                else sensP = (byte)(sensP & 0xF7);
                OnPropertyChanged("SensP4Selected");
            }
        }
        private bool _sensP3;
        public bool SensP3Selected
        {
            get
            {
                return _sensP3;
            }
            set
            {
                _sensP3 = value;
                if (_sensP3) sensP = (byte)(sensP | 0x04);
                else sensP = (byte)(sensP & 0xFB);
                OnPropertyChanged("SensP3Selected");
            }
        }
        private bool _sensP2;
        public bool SensP2Selected
        {
            get
            {
                return _sensP2;
            }
            set
            {
                _sensP2 = value;
                if (_sensP2) sensP = (byte)(sensP | 0x02);
                else sensP = (byte)(sensP & 0xFD);
                OnPropertyChanged("SensP2Selected");
            }
        }
        private bool _sensP1;
        public bool SensP1Selected
        {
            get
            {
                return _sensP1;
            }
            set
            {
                _sensP1 = value;
                if (_sensP1) sensP = (byte)(sensP | 0x01);
                else sensP = (byte)(sensP & 0xFE);
                OnPropertyChanged("SensP1Selected");
            }
        }

        private byte sensN;
        private bool _sensN8;
        public bool SensN8Selected
        {
            get
            {
                return _sensN8;
            }
            set
            {
                _sensN8 = value;
                if (_sensN8) sensN = (byte)(sensN | 0x80);
                else sensN = (byte)(sensN & 0x7F);
                OnPropertyChanged("SensN8Selected");
            }
        }
        private bool _sensN7;
        public bool SensN7Selected
        {
            get
            {
                return _sensN7;
            }
            set
            {
                _sensN7 = value;
                if (_sensN7) sensN = (byte)(sensN | 0x40);
                else sensN = (byte)(sensN & 0xBF);
                OnPropertyChanged("SensN7Selected");
            }
        }
        private bool _sensN6;
        public bool SensN6Selected
        {
            get
            {
                return _sensN6;
            }
            set
            {
                _sensN6 = value;
                if (_sensN6) sensN = (byte)(sensN | 0x20);
                else sensN = (byte)(sensN & 0xDF);
                OnPropertyChanged("SensN6Selected");
            }
        }
        private bool _sensN5;
        public bool SensN5Selected
        {
            get
            {
                return _sensN5;
            }
            set
            {
                _sensN5 = value;
                if (_sensN5) sensN = (byte)(sensN | 0x10);
                else sensN = (byte)(sensN & 0xEF);
                OnPropertyChanged("SensN5Selected");
            }
        }
        private bool _sensN4;
        public bool SensN4Selected
        {
            get
            {
                return _sensN4;
            }
            set
            {
                _sensN4 = value;
                if (_sensN4) sensN = (byte)(sensN | 0x08);
                else sensN = (byte)(sensN & 0xF7);
                OnPropertyChanged("SensN4Selected");
            }
        }
        private bool _sensN3;
        public bool SensN3Selected
        {
            get
            {
                return _sensN3;
            }
            set
            {
                _sensN3 = value;
                if (_sensN3) sensN = (byte)(sensN | 0x04);
                else sensN = (byte)(sensN & 0xFB);
                OnPropertyChanged("SensN3Selected");
            }
        }
        private bool _sensN2;
        public bool SensN2Selected
        {
            get
            {
                return _sensN2;
            }
            set
            {
                _sensN2 = value;
                if (_sensN2) sensN = (byte)(sensN | 0x02);
                else sensN = (byte)(sensN & 0xFD);
                OnPropertyChanged("SensN2Selected");
            }
        }
        private bool _sensN1;
        public bool SensN1Selected
        {
            get
            {
                return _sensN1;
            }
            set
            {
                _sensN1 = value;
                if (_sensN1) sensN = (byte)(sensN | 0x01);
                else sensN = (byte)(sensN & 0xFE);
                OnPropertyChanged("SensN1Selected");
            }
        }

        #endregion


        #region BUTTON_OK
        private ICommand okCommand = null;
        public ICommand OKCommand
        {
            get { return okCommand; }
            set { okCommand = value; }
        }
        private void OnOKClicked(object parameter)
        {
         
            List<ModuleChannel> displaySelection = new  List<ModuleChannel>(){display1Selection,display2Selection,display3Selection,display4Selection,display5Selection,display6Selection};
            SettingWindow windowResult = new SettingWindow(Int32.Parse(TimeAxisSelectedItem), settingWindow.UserTabDisplays, displaySelection, settingWindow.TabEnabled);

            SettingFBGA fbgaResult = new SettingFBGA(settingFBGA.NumberOfChannels, integrationTime, sledPower, _highDynamicRangeMode, settingFBGA.SelectedDisplays);
            SettingMIC micResult = new SettingMIC(settingMIC.NumberOfChannels, MIC1MuteSelected, MIC2MuteSelected, MIC3MuteSelected, MIC4MuteSelected, HighPassFilterSelected, settingMIC.SelectedDisplays);
            SettingECG ecgResult = new SettingECG(settingECG.NumberOfChannels, false, ecgGain, ECGCH4modeSelectedItem, sensP, sensN,settingECG.SelectedDisplays);
            this.CloseDialogWithResult(parameter as Window, new DialogResultSetting(windowResult, fbgaResult,micResult,ecgResult));

        }
        #endregion

        #region BUTTON_Cancel
        private ICommand cancelCommand = null;
        public ICommand CancelCommand
        {
            get { return cancelCommand; }
            set { cancelCommand = value; }
        }
        private void OnCancelClicked(object parameter)
        {
            this.CloseDialogWithResult(parameter as Window, new DialogResultSetting(settingWindow,settingFBGA,settingMIC,settingECG));

        }
        #endregion
    }
}
