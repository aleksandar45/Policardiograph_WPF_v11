using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using SharpGL.SceneGraph;
using SharpGL;
using System.Collections.ObjectModel;
using Policardiograph_App.View;
using Policardiograph_App.Settings;
using Policardiograph_App.Patients;
using Policardiograph_App.Dialogs.DialogService;
using Policardiograph_App.Dialogs.DialogPerson;
using Policardiograph_App.Dialogs.DialogSetting;
using Policardiograph_App.ViewModel.OpenGLRender;

namespace Policardiograph_App.ViewModel
{
    public enum ProfileEnumType{ ALL_DEVICES = 0, MIC, FBGA, ECG, ACC, PPM};
    public enum TCPModuleStatusEnumType { DISCONNECTED = 0,CONNECTED, CONNECTED_IDLE, CONNECTED_TRANSFERING };
    public enum USBModuleStatusEnumType { NOT_INITIALIZED = 0, INITIALIZED, TRANSFERING };
    public class ModuleDescription {
        public ModuleDescription(string moduleName, string status, int numOfChannels, int sampleRate) {
            ModuleName = moduleName;
            Status = status;
            NumberOfChannels = numOfChannels;
            SampleRate = sampleRate;
        }
        public string ModuleName { get; set; }
        public string Status { get; set; }
        public int NumberOfChannels { get; set; }
        public int SampleRate { get; set; }

    }
    public class MainWindowViewModel: ViewModelBase

    {
        private TreeViewViewModel treeViewViewModel;
        private PatientViewModel patientViewModel;
        public MainWindowViewModel(TreeViewViewModel treeViewViewModel, PatientViewModel patientViewModel,  OpenGLDispatcher openGLDispatcher,SettingProgram settingProgramData, SettingWindow settingWindowData, SettingFBGA settingFBGAData, SettingMIC settingMICData,SettingECG settingECGData, SettingACC settingACCData, SettingPPG settingPPGData,List<Patient> patients, Patient selectedPatient)
        {
            this.treeViewViewModel = treeViewViewModel;
            this.patientViewModel = patientViewModel;
            this.OpenGLDispatcher = openGLDispatcher;

            DebugEnableCommand = new DelegateCommand(DebugEnableExecute, CanDebugEnableExecute);
            DebugStartStopCommand = new DelegateCommand(DebugStartStopExecute);
            SaveFileTestCommand = new DelegateCommand(SaveFileTestExecute, CanSaveFileTestExecute);
            PersonCommand = new DelegateCommand(PersonExecute);
            PlayCommand = new DelegateCommand(PlayExecute,CanPlayExecute);            
            RecordCommand = new DelegateCommand(RecordExecute,CanRecordExecute);
            RefreshConnectionCommand = new DelegateCommand(RefreshConnectionExecute);
            SettingCommand = new DelegateCommand(SettingExecute);
            MWLSTurnOnCommand = new DelegateCommand(MWLSTurnOnExecute);
            WindowClosing = new DelegateCommand(WindowClosingExecute);

           // List<int> channelNumbers = new List<int>() { micNoChannels, fbgaNoChannels, ecgNoChannels, accNoChannels, ppgNoChannels };
           // List<ModuleChannel> selectedDisplays = new List<ModuleChannel>() { new ModuleChannel("MIC", 1), new ModuleChannel("FBGA", 1), new ModuleChannel("ECG", 12), new ModuleChannel("ACC", 2), new ModuleChannel("PPG", 1), new ModuleChannel("PPG", 2) };
           // SettingWindowData = new SettingWindow(6, channelNumbers, selectedDisplays, true);
            SettingProgramData = settingProgramData;
            SettingWindowData = settingWindowData;
            ToogleButtonUserIsChecked = true;

            SettingFBGAData = settingFBGAData;
            SettingMICData = settingMICData;
            SettingECGData = settingECGData;
            SettingACCData = settingACCData;
            SettingPPGData = settingPPGData;

            Patients = patients;
            SelectedPatient = selectedPatient;
            ChoosenPatient = null;

            progressBarTimer = new DispatcherTimer();
            progressBarTimer.Interval = new TimeSpan(0, 0, 1);
            progressBarTimer.Tick += progressBarTimer_Tick;

            recordingTimer = new DispatcherTimer();
            recordingTimer.Interval = new TimeSpan(0, 0, 1);
            recordingTimer.Tick += recordingTimer_Tick;
            
        }
        #region DEBUG
        private bool debugEnabled = false;
        public bool DebugEnabled
        {
            get
            {
                return debugEnabled;
            }
            set
            {
                debugEnabled = value;
                if (debugEnabled)
                {
                    BatteryVoltageLabelIsVisible = true;
                    SaveFileButtonIsVisible = true;
                    DebugToggleButtonIsVisible = true;
                    DebugToggleButtonIsChecked = true;                    
                }
                else
                {
                    BatteryVoltageLabelIsVisible = false;
                    SaveFileButtonIsVisible = false;
                    DebugToggleButtonIsVisible = false;
                    DebugToggleButtonIsChecked = false;
                    ToogleButtonUserIsChecked = true;
                   
                }
            }
        }
        private bool debugRichTextBoxIsVisible = false;
        public bool DebugRichTextBoxIsVisible
        {
            get
            {
                return debugRichTextBoxIsVisible;
            }
            set
            {
                debugRichTextBoxIsVisible = value;
                OnPropertyChanged("DebugRichTextBoxIsVisible");
            }
        }
        private string debugTextBlockText = "";
        public string DebugTextBlockText
        {
            get
            {
                return debugTextBlockText;
            }
            set
            {
                debugTextBlockText = value;
                OnPropertyChanged("DebugTextBlockText");
            }
        }
        private string debugStartStopButtonContent = "Start";
        public string DebugStartStopButtonContent
        {
            get
            {
                return debugStartStopButtonContent;
            }
            set
            {
                debugStartStopButtonContent = value;
                OnPropertyChanged("DebugStartStopButtonContent");             
            }
        }
        private bool debugStartStopButtonIsVisible = false;
        public bool DebugStartStopButtonIsVisible
        {
            get
            {
                return debugStartStopButtonIsVisible;
            }
            set
            {
                debugStartStopButtonIsVisible = value;
                OnPropertyChanged("DebugStartStopButtonIsVisible");
            }
        }

        public ICommand DebugStartStopCommand { get; set; }
        private void DebugStartStopExecute(object obj)
        {
            if(String.Compare(DebugStartStopButtonContent,"Start") == 0)
            {
                DebugStartStopButtonContent = "Stop";
            }
            else
            {
                DebugStartStopButtonContent = "Start";
            }
        }
        public ICommand DebugEnableCommand { get; set; }
        private void DebugEnableExecute(object obj)
        {
            if (DebugEnabled)
                DebugEnabled = false;
            else
                DebugEnabled = true;

        }
        private bool CanDebugEnableExecute(object obj)
        {
            return true;
        }

        private bool saveFileButtonIsVisible = false;
        public bool SaveFileButtonIsVisible
        {
            get
            {
                return saveFileButtonIsVisible;
            }
            set
            {
                saveFileButtonIsVisible = value;
                OnPropertyChanged("SaveFileButtonIsVisible");
            }
        }
        public ICommand SaveFileTestCommand { get; set; }
        private void SaveFileTestExecute(object obj)
        {
            if (deviceSaveFile(ChoosenPatient) != 0)
            {
                string warningMessage = "Data NOT recorded successfuly";
                Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Warning, warningMessage);
                Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);
            }

        }
        private bool CanSaveFileTestExecute(object obj)
        {
            return true;
        }
        #endregion

        #region BUTTON_PERSON

        private bool _buttonPersonIsEnabled = true;
        public bool ButtonPersonIsEnabled
        {
            get
            {
                return _buttonPersonIsEnabled;
            }
            set
            {
                _buttonPersonIsEnabled = value;
                if (_buttonPersonIsEnabled == true)
                {
                    PersonImageOpacity = 1.0f;
                }
                else
                {
                    PersonImageOpacity = 0.5f;
                }
                OnPropertyChanged("ButtonPersonIsEnabled");
            }
        }

        private float _personImageOpacity = 1.0F;
        public float PersonImageOpacity
        {
            get
            {
                return _personImageOpacity;
            }
            set
            {
                _personImageOpacity = value;
                OnPropertyChanged("PersonImageOpacity");
            }
        }

        public ICommand PersonCommand { get; set; }
        private void PersonExecute(object obj)
        {
            /*Patient patientTemp = new Patient();
            Patients = new List<Patient>();
                          
            patientTemp.Name = "Aleksandar";
            patientTemp.Surname = "Lazovic";
            patientTemp.ParentName = "Nenad";
            patientTemp.JMBG = "0410992710148";
            Patients.Add(patientTemp);

            patientTemp = new Patient();

            patientTemp.Name = "Teodora";
            patientTemp.Surname = "Djuric-Lazovic";
            patientTemp.ParentName = "Zoran";
            patientTemp.JMBG = "2011996710148";
            Patients.Add(patientTemp);*/


            DialogViewModelBase vm = new DialogPersonViewModel(Patients, SelectedPatient);
            DialogResult result = DialogService.OpenDialog(vm, obj as Window);
            if((result as DialogResultPerson).ApplyResult)
            {
                Patients = (result as DialogResultPerson).Patients;
                SelectedPatient = (result as DialogResultPerson).SelectedPatient;
                ChoosenPatient = SelectedPatient;
                LabelPatientFullName = SelectedPatient.Name + " "+ SelectedPatient.Surname;
                LabelPatientJMBG = SelectedPatient.JMBG;

                patientViewModel.PatientName = SelectedPatient.Name;
                patientViewModel.PatientSurname = SelectedPatient.Surname;

                TabItemPatientIsEnabled = true;
                ButtonPlayIsEnabled = true;
                PlayImageOpacity = 1.0F;
            }
        }

        #endregion

        #region BUTTON_PLAY

        private bool _playing = false;
        public bool Playing
        {
            get
            {
                return _playing;
            }
            set {
                _playing = value;
                OnPropertyChanged("Playing");
            }
        }

        private string _buttonPlayTooltip;
        public string ButtonPlayTooltip 
        {
            get
            {
                return _buttonPlayTooltip ?? "Play";
            }
            set 
            {
                _buttonPlayTooltip = value;
                OnPropertyChanged("ButtonPlayTooltip");
            }
        }

        private bool _buttonPlayIsEnabled = false;
        public bool ButtonPlayIsEnabled { 
            get 
            {
                return _buttonPlayIsEnabled;
            }
            set 
            {
                _buttonPlayIsEnabled = value;
                if (_buttonPlayIsEnabled == true)
                {
                    PlayImageOpacity = 1.0f;
                }
                else
                {
                    PlayImageOpacity = 0.5f;
                }
                OnPropertyChanged("ButtonPlayIsEnabled");
            }
        }

        private float _playImageOpacity = 0.5F;
        public float PlayImageOpacity
        {
            get
            {
                return _playImageOpacity;
            }
            set
            {
                _playImageOpacity = value;
                OnPropertyChanged("PlayImageOpacity");
            }
        }

        public delegate bool DevicePlayExecute(string ComboboxSelectedItem, bool recording);
        public event DevicePlayExecute devicePlayExecute;
        public ICommand PlayCommand { get; set; }
        private void PlayExecute(object obj) {
           
            if (devicePlayExecute(ComboboxSelectedItem, Playing))
            {
                if (Playing)
                {
                    ButtonPlayTooltip = "Play";
                    Playing = false;
                    ProgressBarTimerStart(2,true,false,true,true,true);
                }
                else
                {                    
                    ButtonPlayTooltip = "Stop playing";
                    Playing = true;
                    ProgressBarTimerStart(2,true,true,false,false,true);
                }
                
            }
            
        }
        private bool CanPlayExecute(object obj) {            
            return true;
        }
        
        #endregion

        #region BUTTON_RECORD

        private bool _recording = false;
        public bool Recording 
        {
            get { return _recording; }
            set { 
                _recording = value;
                OnPropertyChanged("Recording");
            }
        }        

        private string _buttonRecordTooltip;
        public string ButtonRecordTooltip
        {
            get 
            {
                return _buttonRecordTooltip ?? "Record";
            }
            set 
            {
                _buttonRecordTooltip = value;
                OnPropertyChanged("ButtonRecordTooltip");
            }
        }

        private bool _buttonRecordIsEnabled = false;
        public bool ButtonRecordIsEnabled {
            get {
                return _buttonRecordIsEnabled;
            }
            set {
                _buttonRecordIsEnabled = value;
                if (_buttonRecordIsEnabled == true)
                {
                    RecordImageOpacity = 1.0f;
                }
                else {
                    RecordImageOpacity = 0.5f;
                }
                OnPropertyChanged("ButtonRecordIsEnabled");
            }
        }

        private float _recordImageOpacity = 0.5F;
        public float RecordImageOpacity
        {
            get
            {
                return _recordImageOpacity;
            }
            set
            {
                _recordImageOpacity = value;
                OnPropertyChanged("RecordImageOpacity");
            }
        }

        public delegate int DeviceRecordExecute(string ComboboxSelectedItem, bool recording);
        public event DeviceRecordExecute deviceRecordExecute;
        public delegate int DeviceSaveFile(Patient patient);
        public event DeviceSaveFile deviceSaveFile;
        public ICommand RecordCommand { get; set; }
        private void RecordExecute(object obj)
        {
            string syncWarningMessage = "";
            int deviceRecordStatus = deviceRecordExecute(ComboboxSelectedItem, Recording);

            ChoosenPatient.Comment = patientViewModel.MeasurementComment;
            Dialogs.DialogPersonComment.DialogPersonCommentViewModel dvmX = new Dialogs.DialogPersonComment.DialogPersonCommentViewModel(ChoosenPatient);
            DialogResult resultX = DialogService.OpenDialog(dvmX);
            ChoosenPatient.Comment = (resultX as Dialogs.DialogPersonComment.DialogResultPersonComment).MeasurementComment;

            if ((deviceRecordStatus & 0x80) == 0x80)          //check if procedure is executed properly
            {
                if (Recording)
                {
                    ButtonRecordTooltip = "Start recording";
                    Recording = false;
                    recordingTimerStop();

                    ProgressBarTimerStart(4, true, false, false, false, true);
                    if (deviceRecordStatus != 0)
                    {
                        if (deviceSaveFile(ChoosenPatient) != 0) {
                            syncWarningMessage = "Data NOT recorded successfuly";
                            Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Warning, syncWarningMessage);
                            Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);
                        }
                    }
                    if (deviceRecordStatus != 0x80)
                    {
                        syncWarningMessage = "Data is not recorded from modules: ";
                        if ((deviceRecordStatus & 0x01) == 0x01)
                            syncWarningMessage += "/MIC ";
                        if ((deviceRecordStatus & 0x02) == 0x02)
                            syncWarningMessage += "/FBGA ";
                        if ((deviceRecordStatus & 0x04) == 0x04)
                            syncWarningMessage += "/ECG ";
                        if ((deviceRecordStatus & 0x08) == 0x08)
                            syncWarningMessage += "/ACC_PPG ";

                        Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Info, syncWarningMessage);
                        Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);
                    }     
                    
                    
                }
                else
                {
                    Recording = true;
                    ButtonRecordTooltip = "Stop recording";
                    ProgressBarTimerStart(4, false, true, false, false, false);
                }
            }
        }
        private bool CanRecordExecute(object obj) {
            return true;
        }
        #endregion

        #region BUTTON_REFRESH_CONNECTION

        private bool _buttonRefreshConnectionIsEnabled = true;
        public bool ButtonRefreshConnectionIsEnabled
        {
            get
            {
                return _buttonRefreshConnectionIsEnabled;
            }
            set
            {
                _buttonRefreshConnectionIsEnabled = value;
                if (_buttonRefreshConnectionIsEnabled == true)
                {
                    RefreshConnectionImageOpacity = 1.0f;
                }
                else
                {
                    RefreshConnectionImageOpacity = 0.5f;
                }
                OnPropertyChanged("ButtonRefreshConnectionIsEnabled");
            }
        }

        private float _refreshConnectionImageOpacity = 1.0F;
        public float RefreshConnectionImageOpacity
        {
            get
            {
                return _refreshConnectionImageOpacity;
            }
            set
            {
                _refreshConnectionImageOpacity = value;
                OnPropertyChanged("RefreshConnectionImageOpacity");
            }
        }

        public ICommand RefreshConnectionCommand { get; set; }
        private void RefreshConnectionExecute(object obj) 
        { 

        }
        
        #endregion

        #region BUTTON_SETTING

        private bool _buttonSettingIsEnabled = true;
        public bool ButtonSettingIsEnabled
        {
            get
            {
                return _buttonSettingIsEnabled;
            }
            set
            {
                _buttonSettingIsEnabled = value;
                if (_buttonSettingIsEnabled == true)
                {
                    SettingImageOpacity = 1.0f;
                }
                else
                {
                    SettingImageOpacity = 0.5f;
                }
                OnPropertyChanged("ButtonSettingIsEnabled");
            }
        }

        private float _settingImageOpacity = 1.0F;
        public float SettingImageOpacity
        {
            get
            {
                return _settingImageOpacity;
            }
            set
            {
                _settingImageOpacity = value;
                OnPropertyChanged("SettingImageOpacity");
            }
        }


        public delegate bool DeviceSettingUpdate(string ComboboxSelectedItem);
        public event DeviceSettingUpdate deviceSettingUpdate;
        public ICommand SettingCommand { get; set; }
        private void SettingExecute(object obj) 
        {
            
           
            //DialogViewModelBase vm = new DialogSettingViewModel(new SettingWindow(6, channelNumbers, selectedDisplays, true));
            DialogViewModelBase vm = new DialogSettingViewModel(SettingWindowData,SettingFBGAData,SettingMICData,SettingECGData);
            DialogResult result = DialogService.OpenDialog(vm, obj as Window);
            SettingWindowData =  (result as DialogResultSetting).SettingWindow;
            if (ToogleButtonUserIsChecked) ToogleButtonUserIsChecked = true;

            SettingFBGAData = (result as DialogResultSetting).SettingFBGA;
            SettingMICData = (result as DialogResultSetting).SettingMIC;
            SettingECGData = (result as DialogResultSetting).SettingECG;
            deviceSettingUpdate(ComboboxSelectedItem);
            ProgressBarTimerStart(4, true, false, true, true, true);
        }        
        #endregion

        #region BUTTON_MWLS_TURN_ON

        private bool _mwlsTurnedOn = false;
        public bool MWLSTurnedOn
        {
            get { return _mwlsTurnedOn; }
            set
            {
                _mwlsTurnedOn = value;
                OnPropertyChanged("MWLSTurnedOn");
            }
        }

        private string _buttonMWLSTurnOnTooltip;
        public string ButtonMWLSTurnOnTooltip
        {
            get
            {
                return _buttonMWLSTurnOnTooltip ?? "Turn on MWLS";
            }
            set
            {
                _buttonMWLSTurnOnTooltip = value;
                OnPropertyChanged("ButtonMWLSTurnOnTooltip");
            }
        }

        private bool _buttonMWLSTurnOnIsEnabled = true;
        public bool ButtonMWLSTurnOnIsEnabled
        {
            get
            {
                return _buttonMWLSTurnOnIsEnabled;
            }
            set
            {
                _buttonMWLSTurnOnIsEnabled = value;
                if (_buttonMWLSTurnOnIsEnabled == true)
                {
                    MWLSTurnOnImageOpacity = 1.0f;
                }
                else
                {
                    MWLSTurnOnImageOpacity = 0.5f;
                }
                OnPropertyChanged("ButtonMWLSTurnOnIsEnabled");
            }
        }

        private float _mwlsTurnOnImageOpacity = 0.5F;
        public float MWLSTurnOnImageOpacity
        {
            get
            {
                return _mwlsTurnOnImageOpacity;
            }
            set
            {
                _mwlsTurnOnImageOpacity = value;
                OnPropertyChanged("MWLSTurnOnImageOpacity");
            }
        }

        public delegate bool DeviceMWLSTurnOnExecute(string ComboboxSelectedItem, bool mwlsTurnedOn);
        public event DeviceMWLSTurnOnExecute deviceMWLSTurnOnExecute;
        public ICommand MWLSTurnOnCommand { get; set; }
        private void MWLSTurnOnExecute(object obj)
        {
            if (deviceMWLSTurnOnExecute(ComboboxSelectedItem, MWLSTurnedOn))
            {
                if (MWLSTurnedOn)
                {
                    ButtonMWLSTurnOnTooltip = "Turn on MWLS";
                    MWLSTurnedOn = false;
                    ProgressBarTimerStart(2, true, false, true, true,true);
                }
                else
                {
                    MWLSTurnedOn = true;
                    ButtonMWLSTurnOnTooltip = "Turn off MWLS";
                    ProgressBarTimerStart(2, true, false, true, true,true);
                }
            }
        }
        private bool CanMWLSTurnOnExecute(object obj)
        {
            return true;
        }
        #endregion        

        #region DATAGRID
        private List<ModuleDescription> _moduleDescriptions = new List<ModuleDescription> {};
        public List<ModuleDescription> ModuleDescriptions
        {
            get {
                return _moduleDescriptions;
            }
            set {
                _moduleDescriptions = value;
                OnPropertyChanged("ModuleDescriptions");
            }
        }
        private bool _dataGridisVisible = false;
        public bool DataGridIsVisible {
            get {
                return _dataGridisVisible;
            }
            set {
                _dataGridisVisible = value;
                OnPropertyChanged("DataGridIsVisible");
            }
        }
        #endregion

        #region COMBOBOX_PROFILE
        private List<string> _comboBoxProfiles = new List<string>(new string[] { "ALL DEVICES", "FBGA", "MIC", "ECG", "ACC", "PPG" });
        public List<string> ComboBoxProfiles
        {
            get
            {
                return _comboBoxProfiles;
            }
            set
            {
                _comboBoxProfiles = value;
                OnPropertyChanged("ComboBoxProfiles");
            }
        }
        private String _comboboxSelectedItem = "ALL DEVICES";
        public String ComboboxSelectedItem 
        {
            get {
                return _comboboxSelectedItem;
            }
            set {
                _comboboxSelectedItem = value;
                OnPropertyChanged("ComboboxSelectedItem");
                // implement profile change
            }
        }
        #endregion

        #region RECORDING_COUNTER
        private DispatcherTimer recordingTimer;
        private const int RECORDING_TIMEOUT = 30;
        private int recordingCounter = 0;
        private void recordingTimer_Tick(object sender, EventArgs e) {
            recordingCounter--;
            RecordingTimerLabelText = recordingCounter.ToString();
            if (recordingCounter == 0)
            {
                recordingTimer.Stop();
                RecordingTimerLabelIsVisible = false;
                string syncWarningMessage = "";
                
                int deviceRecordStatus = deviceRecordExecute(ComboboxSelectedItem, Recording);

                ChoosenPatient.Comment = patientViewModel.MeasurementComment;
                Dialogs.DialogPersonComment.DialogPersonCommentViewModel dvmX = new Dialogs.DialogPersonComment.DialogPersonCommentViewModel(ChoosenPatient);
                DialogResult resultX = DialogService.OpenDialog(dvmX);
                ChoosenPatient.Comment = (resultX as Dialogs.DialogPersonComment.DialogResultPersonComment).MeasurementComment;

                if ((deviceRecordStatus & 0x80) == 0x80)          //check if procedure is executed properly
                {
                    if (Recording)
                    {
                        ButtonRecordTooltip = "Start recording";
                        Recording = false;
                        ProgressBarTimerStart(4, true, false, false, false, true);

                        if (deviceRecordStatus != 0) {
                            if (deviceSaveFile(ChoosenPatient) != 0)
                            {
                                syncWarningMessage = "Data NOT recorded successfuly";
                                Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Warning, syncWarningMessage);
                                Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);
                            }
                        }
                        if (deviceRecordStatus != 0x80)
                        {
                            syncWarningMessage = "Data is not recorded from modules: ";
                            if ((deviceRecordStatus & 0x01) == 0x01)
                                syncWarningMessage += "/MIC ";
                            if ((deviceRecordStatus & 0x02) == 0x02)
                                syncWarningMessage += "/FBGA ";
                            if ((deviceRecordStatus & 0x04) == 0x04)
                                syncWarningMessage += "/ECG ";
                            if ((deviceRecordStatus & 0x08) == 0x08)
                                syncWarningMessage += "/ACC_PPG ";

                            Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Info, syncWarningMessage);
                            Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);
                        }
                        
                    }
                    else
                    {
                        Recording = true;
                        ButtonRecordTooltip = "Stop recording";
                        ProgressBarTimerStart(4, false, true, false, false, false);
                    }
                }

            }
        }
        public void recodingTimerStart() {
            recordingCounter = RECORDING_TIMEOUT;
            RecordingTimerLabelText = RECORDING_TIMEOUT.ToString();
            RecordingTimerLabelIsVisible = true;
            recordingTimer.Start();
        }
        private void recordingTimerStop() {
            recordingTimer.Stop();
            RecordingTimerLabelIsVisible = false;
        }
        private bool _recordingTimerLabelIsVisible = false;
        public bool RecordingTimerLabelIsVisible{
            get {
                return _recordingTimerLabelIsVisible;
            }
            set {
                _recordingTimerLabelIsVisible = value;
                OnPropertyChanged("RecordingTimerLabelIsVisible");
            }
        }
        private string _recordingTimerLabelText = "";
        public string RecordingTimerLabelText
        {
            get
            {
                return _recordingTimerLabelText;
            }
            set
            {
                _recordingTimerLabelText = value;
                OnPropertyChanged("RecordingTimerLabelText");
            }
        }
        #endregion

        #region PROGRESS_BAR
        private DispatcherTimer progressBarTimer;
        private int progressBarTimerCounter = 0;
        private bool buttonPlayState;
        private bool buttonRecordState;
        private bool buttonRefreshConnectionState;
        private bool buttonSettingState;
        private bool buttonMWLSTurnOnState;
        private int progressBarTimerMAXCount = 4;
        private void ProgressBarTimerStart(int timeout, bool buttonPlayState, bool buttonRecordState, bool buttonRefreshConnectionState, bool buttonSettingState, bool buttonMWLSTurnOnState)
        {
            this.buttonPlayState = buttonPlayState;
            this.buttonRecordState=buttonRecordState ;
            this.buttonRefreshConnectionState = buttonRefreshConnectionState;
            this.buttonSettingState =  buttonSettingState;
            this.buttonMWLSTurnOnState = buttonMWLSTurnOnState;
            progressBarTimerMAXCount = timeout;
            ProgressBarIsVisible = true;
            progressBarTimer.Start();
            ButtonPlayIsEnabled = false;
            ButtonRecordIsEnabled = false;
            ButtonRefreshConnectionIsEnabled = false;
            ButtonSettingIsEnabled = false;
            ButtonMWLSTurnOnIsEnabled = false;
        }
        private void progressBarTimer_Tick(object sender, EventArgs e) {
            progressBarTimerCounter++;
            ProgressBarValue = progressBarTimerCounter * 100 / progressBarTimerMAXCount;
            if (progressBarTimerCounter == progressBarTimerMAXCount) {
                progressBarTimer.Stop();
                progressBarTimerCounter = 0;
                ProgressBarIsVisible = false;
                ButtonPlayIsEnabled = buttonPlayState;
                ButtonRecordIsEnabled = buttonRecordState;
                ButtonRefreshConnectionIsEnabled = buttonRefreshConnectionState;
                ButtonSettingIsEnabled = buttonSettingState;
                ButtonMWLSTurnOnIsEnabled = buttonMWLSTurnOnState;
                ProgressBarValue = 0;
            }
        }
        private bool _progressBarIsVisible = false;
        public bool ProgressBarIsVisible
        {
            get
            {
                return _progressBarIsVisible;
            }
            set
            {
                _progressBarIsVisible = value;
                OnPropertyChanged("ProgressBarIsVisible");
            }
        }
        private int _progressBarValue = 0;
        public int ProgressBarValue
        {
            get
            {
                return _progressBarValue;
            }
            set
            {
                _progressBarValue = value;
                OnPropertyChanged("ProgressBarValue");
            }
        }
        #endregion

        #region PATIENT_UI

        private string _labelPatientFullName = "";
        public string LabelPatientFullName
        {
            get
            {
                return _labelPatientFullName;
            }
            set
            {
                _labelPatientFullName = value;
                OnPropertyChanged("LabelPatientFullName");
            }
        }
        private string _labelPatientJMBG = "";
        public string LabelPatientJMBG
        {
            get
            {
                return _labelPatientJMBG;
            }
            set
            {
                _labelPatientJMBG = value;
                OnPropertyChanged("LabelPatientJMBG");
            }
        }

        private bool _tabItemPatientIsEnabled = false;
        public bool TabItemPatientIsEnabled
        {
            get
            {
                return _tabItemPatientIsEnabled;
            }
            set
            {
                _tabItemPatientIsEnabled = value;
                OnPropertyChanged("TabItemPatientIsEnabled");
            }
        }
        #endregion        

        #region TOGGLE_BUTTONS
        private bool debugToggleButtonIsVisible = false;
        public bool DebugToggleButtonIsVisible
        {
            get
            {
                return debugToggleButtonIsVisible;
            }
            set
            {
                debugToggleButtonIsVisible = value;
                OnPropertyChanged("DebugToggleButtonIsVisible");
            }
        }

        private bool _debugToggleButtonIsChecked = false;
        public bool DebugToggleButtonIsChecked
        {
            get
            {
                return _debugToggleButtonIsChecked;
            }
            set
            {
                _debugToggleButtonIsChecked = value;
                if (_debugToggleButtonIsChecked)
                {
                    ToogleButtonUserIsChecked = false;
                    ToogleButtonFBGAIsChecked = false;
                    ToogleButtonMICIsChecked = false;
                    ToogleButtonECG1IsChecked = false;
                    ToogleButtonECG2IsChecked = false;
                    ToogleButtonACCIsChecked = false;
                    ToogleButtonPPGIsChecked = false;

                    DebugRichTextBoxIsVisible = true;
                    DebugStartStopButtonIsVisible = true;

                    OpenGLDispatcher.disableDisplay(1);
                    LabelDisplay1Tittle = "";
                    LabelDisplay1XAxis = "";
                    LabelDisplay1YAxis = "";

                    OpenGLDispatcher.disableDisplay(2);
                    LabelDisplay2Tittle = "";
                    LabelDisplay2XAxis = "";
                    LabelDisplay2YAxis = "";

                    OpenGLDispatcher.disableDisplay(3);
                    LabelDisplay3Tittle = "";
                    LabelDisplay3XAxis = "";
                    LabelDisplay3YAxis = "";

                    OpenGLDispatcher.disableDisplay(4);
                    LabelDisplay4Tittle = "";
                    LabelDisplay4XAxis = "";
                    LabelDisplay4YAxis = "";

                    OpenGLDispatcher.disableDisplay(5);
                    LabelDisplay5Tittle = "";
                    LabelDisplay5XAxis = "";
                    LabelDisplay5YAxis = "";

                    OpenGLDispatcher.disableDisplay(6);
                    LabelDisplay6Tittle = "";
                    LabelDisplay6XAxis = "";
                    LabelDisplay6YAxis = "";


                }
                OnPropertyChanged("DebugToggleButtonIsChecked");
            }
        }
        private bool _toogleButtonUserIsChecked = true;
        public bool ToogleButtonUserIsChecked
        {
            get {
                return _toogleButtonUserIsChecked;
            }
            set {
                _toogleButtonUserIsChecked = value;
                if (_toogleButtonUserIsChecked) {
                    ToogleButtonFBGAIsChecked = false;
                    ToogleButtonMICIsChecked = false;
                    ToogleButtonECG1IsChecked = false;
                    ToogleButtonECG2IsChecked = false;
                    ToogleButtonACCIsChecked = false;
                    ToogleButtonPPGIsChecked = false;
                    DebugToggleButtonIsChecked = false;

                    DebugRichTextBoxIsVisible = false;
                    DebugStartStopButtonIsVisible = false;

                    OpenGLDispatcher.enableDisplay(1);
                    int chNo = SettingWindowData.UserTabSelectedDisplays.ElementAt(0).ChannelNumber;
                    string chName = SettingWindowData.UserTabSelectedDisplays.ElementAt(0).ModuleName;
                    string chAxis = SettingWindowData.UserTabSelectedDisplays.ElementAt(0).Axis;
                    string chDescription = SettingWindowData.UserTabSelectedDisplays.ElementAt(0).Description;
                    string tempDescription;
                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                    else tempDescription = chDescription;
                    LabelDisplay1Tittle = chName + "_CH" + chNo.ToString() + chAxis + tempDescription;
                    if(String.Compare("FBGA",chName)==0){
                        LabelDisplay1XAxis = "Wavelength (nm)";
                        LabelDisplay1YAxis = "Counter";
                    }
                    else{
                        LabelDisplay1XAxis = "Time (s)";
                        LabelDisplay1YAxis = "mV";
                    }
                    if (String.Compare(chName, "ACC")==0) {
                        if(String.Compare(chAxis,"x")==0)
                            OpenGLDispatcher.linkDisplay(1, chName, (chNo - 1) * 3 + 1);
                        else if(String.Compare(chAxis, "y") == 0)                        
                            OpenGLDispatcher.linkDisplay(1, chName, (chNo - 1) * 3 + 2);
                        else if (String.Compare(chAxis, "z") == 0)
                            OpenGLDispatcher.linkDisplay(1, chName, (chNo - 1) * 3 + 3);
                    }
                    else if(String.Compare(chName, "PPG") == 0){
                        if (String.Compare(chAxis, "g") == 0)
                            OpenGLDispatcher.linkDisplay(1, chName, (chNo - 1) * 2 + 1);
                        else if (String.Compare(chAxis, "r") == 0)
                            OpenGLDispatcher.linkDisplay(1, chName, (chNo - 1) * 2 + 2);
                    }
                    else{
                        OpenGLDispatcher.linkDisplay(1, chName, chNo);
                    }

                    OpenGLDispatcher.enableDisplay(2);
                    chNo = SettingWindowData.UserTabSelectedDisplays.ElementAt(1).ChannelNumber;
                    chName = SettingWindowData.UserTabSelectedDisplays.ElementAt(1).ModuleName;
                    chAxis = SettingWindowData.UserTabSelectedDisplays.ElementAt(1).Axis;
                    chDescription = SettingWindowData.UserTabSelectedDisplays.ElementAt(1).Description;                    
                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                    else tempDescription = chDescription;
                    LabelDisplay2Tittle = chName + "_CH" + chNo.ToString() + chAxis + tempDescription;
                    if(String.Compare("FBGA",chName)==0){
                        LabelDisplay2XAxis = "Wavelength (nm)";
                        LabelDisplay2YAxis = "Counter";
                    }
                    else{
                        LabelDisplay2XAxis = "Time (s)";
                        LabelDisplay2YAxis = "mV";
                    }
                    if (String.Compare(chName, "ACC") == 0)
                    {
                        if (String.Compare(chAxis, "x") == 0)
                            OpenGLDispatcher.linkDisplay(2, chName, (chNo - 1) * 3 + 1);
                        else if (String.Compare(chAxis, "y") == 0)
                            OpenGLDispatcher.linkDisplay(2, chName, (chNo - 1) * 3 + 2);
                        else if (String.Compare(chAxis, "z") == 0)
                            OpenGLDispatcher.linkDisplay(2, chName, (chNo - 1) * 3 + 3);
                    }
                    else if (String.Compare(chName, "PPG") == 0)
                    {
                        if (String.Compare(chAxis, "g") == 0)
                            OpenGLDispatcher.linkDisplay(2, chName, (chNo - 1) * 2 + 1);
                        else if (String.Compare(chAxis, "r") == 0)
                            OpenGLDispatcher.linkDisplay(2, chName, (chNo - 1) * 2 + 2);
                    }
                    else
                    {
                        OpenGLDispatcher.linkDisplay(2, chName, chNo);
                    }

                    OpenGLDispatcher.enableDisplay(3);
                    chNo = SettingWindowData.UserTabSelectedDisplays.ElementAt(2).ChannelNumber;
                    chName = SettingWindowData.UserTabSelectedDisplays.ElementAt(2).ModuleName;
                    chAxis = SettingWindowData.UserTabSelectedDisplays.ElementAt(2).Axis;
                    chDescription = SettingWindowData.UserTabSelectedDisplays.ElementAt(2).Description;
                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                    else tempDescription = chDescription;
                    LabelDisplay3Tittle = chName + "_CH" + chNo.ToString() + chAxis + tempDescription;
                    if(String.Compare("FBGA",chName)==0){
                        LabelDisplay3XAxis = "Wavelength (nm)";
                        LabelDisplay3YAxis = "Counter";
                    }
                    else{
                        LabelDisplay3XAxis = "Time (s)";
                        LabelDisplay3YAxis = "mV";
                    }
                    if (String.Compare(chName, "ACC") == 0)
                    {
                        if (String.Compare(chAxis, "x") == 0)
                            OpenGLDispatcher.linkDisplay(3, chName, (chNo - 1) * 3 + 1);
                        else if (String.Compare(chAxis, "y") == 0)
                            OpenGLDispatcher.linkDisplay(3, chName, (chNo - 1) * 3 + 2);
                        else if (String.Compare(chAxis, "z") == 0)
                            OpenGLDispatcher.linkDisplay(3, chName, (chNo - 1) * 3 + 3);
                    }
                    else if (String.Compare(chName, "PPG") == 0)
                    {
                        if (String.Compare(chAxis, "g") == 0)
                            OpenGLDispatcher.linkDisplay(3, chName, (chNo - 1) * 2 + 1);
                        else if (String.Compare(chAxis, "r") == 0)
                            OpenGLDispatcher.linkDisplay(3, chName, (chNo - 1) * 2 + 2);
                    }
                    else
                    {
                        OpenGLDispatcher.linkDisplay(3, chName, chNo);
                    }

                    OpenGLDispatcher.enableDisplay(4);
                    chNo = SettingWindowData.UserTabSelectedDisplays.ElementAt(3).ChannelNumber;
                    chName = SettingWindowData.UserTabSelectedDisplays.ElementAt(3).ModuleName;
                    chAxis = SettingWindowData.UserTabSelectedDisplays.ElementAt(3).Axis;
                    chDescription = SettingWindowData.UserTabSelectedDisplays.ElementAt(3).Description;
                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                    else tempDescription = chDescription;
                    LabelDisplay4Tittle = chName + "_CH" + chNo.ToString() + chAxis + tempDescription;
                    if(String.Compare("FBGA",chName)==0){
                        LabelDisplay4XAxis = "Wavelength (nm)";
                        LabelDisplay4YAxis = "Counter";
                    }
                    else{
                        LabelDisplay4XAxis = "Time (s)";
                        LabelDisplay4YAxis = "mV";
                    }
                    if (String.Compare(chName, "ACC") == 0)
                    {
                        if (String.Compare(chAxis, "x") == 0)
                            OpenGLDispatcher.linkDisplay(4, chName, (chNo - 1) * 3 + 1);
                        else if (String.Compare(chAxis, "y") == 0)
                            OpenGLDispatcher.linkDisplay(4, chName, (chNo - 1) * 3 + 2);
                        else if (String.Compare(chAxis, "z") == 0)
                            OpenGLDispatcher.linkDisplay(4, chName, (chNo - 1) * 3 + 3);                 
                    }
                    else if (String.Compare(chName, "PPG") == 0)
                    {
                        if (String.Compare(chAxis, "g") == 0)
                            OpenGLDispatcher.linkDisplay(4, chName, (chNo - 1) * 2 + 1);
                        else if (String.Compare(chAxis, "r") == 0)
                            OpenGLDispatcher.linkDisplay(4, chName, (chNo - 1) * 2 + 2);
                    }
                    else
                    {
                        OpenGLDispatcher.linkDisplay(4, chName, chNo);
                    }

                    OpenGLDispatcher.enableDisplay(5);
                    chNo = SettingWindowData.UserTabSelectedDisplays.ElementAt(4).ChannelNumber;
                    chName = SettingWindowData.UserTabSelectedDisplays.ElementAt(4).ModuleName;
                    chAxis = SettingWindowData.UserTabSelectedDisplays.ElementAt(4).Axis;
                    chDescription = SettingWindowData.UserTabSelectedDisplays.ElementAt(4).Description;
                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                    else tempDescription = chDescription;
                    LabelDisplay5Tittle = chName + "_CH" + chNo.ToString() + chAxis + tempDescription;
                    if(String.Compare("FBGA",chName)==0){
                        LabelDisplay5XAxis = "Wavelength (nm)";
                        LabelDisplay5YAxis = "Counter";
                    }
                    else{
                        LabelDisplay5XAxis = "Time (s)";
                        LabelDisplay5YAxis = "mV";
                    }
                    if (String.Compare(chName, "ACC") == 0)
                    {
                        if (String.Compare(chAxis, "x") == 0)
                            OpenGLDispatcher.linkDisplay(5, chName, (chNo - 1) * 3 + 1);
                        else if (String.Compare(chAxis, "y") == 0)
                            OpenGLDispatcher.linkDisplay(5, chName, (chNo - 1) * 3 + 2);
                        else if (String.Compare(chAxis, "z") == 0)
                            OpenGLDispatcher.linkDisplay(5, chName, (chNo - 1) * 3 + 3);                  
                    }
                    else if (String.Compare(chName, "PPG") == 0)
                    {
                        if (String.Compare(chAxis, "g") == 0)
                            OpenGLDispatcher.linkDisplay(5, chName, (chNo - 1) * 2 + 1);
                        else if (String.Compare(chAxis, "r") == 0)
                            OpenGLDispatcher.linkDisplay(5, chName, (chNo - 1) * 2 + 2);
                    }
                    else
                    {
                        OpenGLDispatcher.linkDisplay(5, chName, chNo);
                    }

                    OpenGLDispatcher.enableDisplay(6);
                    chNo = SettingWindowData.UserTabSelectedDisplays.ElementAt(5).ChannelNumber;
                    chName = SettingWindowData.UserTabSelectedDisplays.ElementAt(5).ModuleName;
                    chAxis = SettingWindowData.UserTabSelectedDisplays.ElementAt(5).Axis;
                    chDescription = SettingWindowData.UserTabSelectedDisplays.ElementAt(5).Description;
                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                    else tempDescription = chDescription;
                    LabelDisplay6Tittle = chName + "_CH" + chNo.ToString() + chAxis + tempDescription;
                    if(String.Compare("FBGA",chName)==0){
                        LabelDisplay6XAxis = "Wavelength (nm)";
                        LabelDisplay6YAxis = "Counter";
                    }
                    else{
                        LabelDisplay6XAxis = "Time (s)";
                        LabelDisplay6YAxis = "mV";
                    }
                    if (String.Compare(chName, "ACC") == 0)
                    {
                        if (String.Compare(chAxis, "x") == 0)
                            OpenGLDispatcher.linkDisplay(6, chName, (chNo - 1) * 3 + 1);
                        else if (String.Compare(chAxis, "y") == 0)
                            OpenGLDispatcher.linkDisplay(6, chName, (chNo - 1) * 3 + 2);
                        else if (String.Compare(chAxis, "z") == 0)
                            OpenGLDispatcher.linkDisplay(6, chName, (chNo - 1) * 3 + 3);                       
                    }
                    else if (String.Compare(chName, "PPG") == 0)
                    {
                        if (String.Compare(chAxis, "g") == 0)
                            OpenGLDispatcher.linkDisplay(6, chName, (chNo - 1) * 2 + 1);
                        else if (String.Compare(chAxis, "r") == 0)
                            OpenGLDispatcher.linkDisplay(6, chName, (chNo - 1) * 2 + 2);
                    }
                    else
                    {
                        OpenGLDispatcher.linkDisplay(6, chName, chNo);
                    }
                }
                OnPropertyChanged("ToogleButtonUserIsChecked");
            }
        }
        private bool _toogleButtonFBGAIsChecked = false;
        public bool ToogleButtonFBGAIsChecked
        {
            get
            {
                return _toogleButtonFBGAIsChecked;
            }
            set
            {
                _toogleButtonFBGAIsChecked = value;
                if (_toogleButtonFBGAIsChecked)
                {
                    ToogleButtonUserIsChecked = false;
                    ToogleButtonMICIsChecked = false;
                    ToogleButtonECG1IsChecked = false;
                    ToogleButtonECG2IsChecked = false;
                    ToogleButtonACCIsChecked = false;
                    ToogleButtonPPGIsChecked = false;
                    DebugToggleButtonIsChecked = false;

                    DebugRichTextBoxIsVisible = false;
                    DebugStartStopButtonIsVisible = false;

                    for (int i = 0; i < 6; i++) {
                        
                        switch (i + 1) {
                            case 1:
                                if (i < SettingFBGAData.SelectedDisplays.Count)
                                {
                                    OpenGLDispatcher.enableDisplay(1);
                                    string chDescription = SettingFBGAData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay1Tittle = SettingFBGAData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingFBGAData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingFBGAData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay1XAxis = "Wavelength (nm)";
                                    LabelDisplay1YAxis = "Counter";
                                    OpenGLDispatcher.linkDisplay(1, "FBGA", SettingFBGAData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else {
                                    OpenGLDispatcher.disableDisplay(1);
                                    LabelDisplay1Tittle = "";
                                    LabelDisplay1XAxis = "";
                                    LabelDisplay1YAxis = "";
                                }
                                break;
                            case 2:
                                if (i < SettingFBGAData.SelectedDisplays.Count)
                                {
                                    OpenGLDispatcher.enableDisplay(2);
                                    string chDescription = SettingFBGAData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay2Tittle = SettingFBGAData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingFBGAData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingFBGAData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay2XAxis = "Wavelength (nm)";
                                    LabelDisplay2YAxis = "Counter";
                                    OpenGLDispatcher.linkDisplay(2, "FBGA", SettingFBGAData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else {
                                    OpenGLDispatcher.disableDisplay(2);
                                    LabelDisplay2Tittle = "";
                                    LabelDisplay2XAxis = "";
                                    LabelDisplay2YAxis = "";
                                }
                                break;
                            case 3:
                                if (i < SettingFBGAData.SelectedDisplays.Count)
                                {
                                    OpenGLDispatcher.enableDisplay(3);
                                    string chDescription = SettingFBGAData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay3Tittle = SettingFBGAData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingFBGAData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingFBGAData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay3XAxis = "Wavelength (nm)";
                                    LabelDisplay3YAxis = "Counter";
                                    OpenGLDispatcher.linkDisplay(3, "FBGA", SettingFBGAData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(3);
                                    LabelDisplay3Tittle = "";
                                    LabelDisplay3XAxis = "";
                                    LabelDisplay3YAxis = "";
                                }
                                break;
                            case 4:
                                if (i < SettingFBGAData.SelectedDisplays.Count)
                                {
                                    OpenGLDispatcher.enableDisplay(4);
                                    string chDescription = SettingFBGAData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay4Tittle = SettingFBGAData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingFBGAData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingFBGAData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay4XAxis = "Wavelength (nm)";
                                    LabelDisplay4YAxis = "Counter";
                                    OpenGLDispatcher.linkDisplay(4, "FBGA", SettingFBGAData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(4);
                                    LabelDisplay4Tittle = "";
                                    LabelDisplay4XAxis = "";
                                    LabelDisplay4YAxis = "";
                                }
                                break;
                            case 5:
                                if (i < SettingFBGAData.SelectedDisplays.Count)
                                {
                                    OpenGLDispatcher.enableDisplay(5);
                                    string chDescription = SettingFBGAData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay5Tittle = SettingFBGAData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingFBGAData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingFBGAData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay5XAxis = "Wavelength (nm)";
                                    LabelDisplay5YAxis = "Counter";
                                    OpenGLDispatcher.linkDisplay(5, "FBGA", SettingFBGAData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(5);
                                    LabelDisplay5Tittle = "";
                                    LabelDisplay5XAxis = "";
                                    LabelDisplay5YAxis = "";
                                }
                                break;
                            case 6:
                                if (i < SettingFBGAData.SelectedDisplays.Count)
                                {
                                    OpenGLDispatcher.enableDisplay(6);
                                    string chDescription = SettingFBGAData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay6Tittle = SettingFBGAData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingFBGAData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingFBGAData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay6XAxis = "Wavelength (nm)";
                                    LabelDisplay6YAxis = "Counter";
                                    OpenGLDispatcher.linkDisplay(6, "FBGA", SettingFBGAData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(6);
                                    LabelDisplay6Tittle = "";
                                    LabelDisplay6XAxis = "";
                                    LabelDisplay6YAxis = "";
                                }
                                break;
                        }
                        
                    }                         
                }
                OnPropertyChanged("ToogleButtonFBGAIsChecked");
            }
        }
        private bool _toogleButtonMICIsChecked = false;
        public bool ToogleButtonMICIsChecked
        {
            get
            {
                return _toogleButtonMICIsChecked;
            }
            set
            {
                _toogleButtonMICIsChecked = value;
                if (_toogleButtonMICIsChecked)
                {
                    ToogleButtonUserIsChecked = false;
                    ToogleButtonFBGAIsChecked = false;
                    ToogleButtonECG1IsChecked = false;
                    ToogleButtonECG2IsChecked = false;
                    ToogleButtonACCIsChecked = false;
                    ToogleButtonPPGIsChecked = false;
                    DebugToggleButtonIsChecked = false;

                    DebugRichTextBoxIsVisible = false;
                    DebugStartStopButtonIsVisible = false;

                    for (int i = 0; i < 6; i++)
                    {

                        switch (i + 1)
                        {
                            case 1:
                                if (i < SettingMICData.SelectedDisplays.Count)
                                {
                                    OpenGLDispatcher.enableDisplay(1);
                                    string chDescription = SettingMICData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay1Tittle = SettingMICData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingMICData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingMICData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay1XAxis = "Time (s)";
                                    LabelDisplay1YAxis = "mV";
                                    OpenGLDispatcher.linkDisplay(1, "MIC", SettingMICData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(1);
                                    LabelDisplay1Tittle = "";
                                    LabelDisplay1XAxis = "";
                                    LabelDisplay1YAxis = "";
                                }
                                break;
                            case 2:
                                if (i < SettingMICData.SelectedDisplays.Count)
                                {
                                    OpenGLDispatcher.enableDisplay(2);
                                    string chDescription = SettingMICData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay2Tittle = SettingMICData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingMICData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingMICData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay2XAxis = "Time (s)";
                                    LabelDisplay2YAxis = "mV";
                                    OpenGLDispatcher.linkDisplay(2, "MIC", SettingMICData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(2);
                                    LabelDisplay2Tittle = "";
                                    LabelDisplay2XAxis = "";
                                    LabelDisplay2YAxis = "";
                                }
                                break;
                            case 3:
                                if (i < SettingMICData.SelectedDisplays.Count)
                                {
                                    OpenGLDispatcher.enableDisplay(3);
                                    string chDescription = SettingMICData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay3Tittle = SettingMICData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingMICData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingMICData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay3XAxis = "Time (s)";
                                    LabelDisplay3YAxis = "mV";
                                    OpenGLDispatcher.linkDisplay(3, "MIC", SettingMICData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(3);
                                    LabelDisplay3Tittle = "";
                                    LabelDisplay3XAxis = "";
                                    LabelDisplay3YAxis = "";
                                }
                                break;
                            case 4:
                                if (i < SettingMICData.SelectedDisplays.Count)
                                {
                                    OpenGLDispatcher.enableDisplay(4);
                                    string chDescription = SettingMICData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay4Tittle = SettingMICData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingMICData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingMICData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay4XAxis = "Time (s)";
                                    LabelDisplay4YAxis = "mV";
                                    OpenGLDispatcher.linkDisplay(4, "MIC", SettingMICData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(4);
                                    LabelDisplay4Tittle = "";
                                    LabelDisplay4XAxis = "";
                                    LabelDisplay4YAxis = "";
                                }
                                break;
                            case 5:
                                if (i < SettingMICData.SelectedDisplays.Count)
                                {
                                    OpenGLDispatcher.enableDisplay(5);
                                    string chDescription = SettingMICData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay5Tittle = SettingMICData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingMICData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingMICData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay5XAxis = "Time (s)";
                                    LabelDisplay5YAxis = "mV";
                                    OpenGLDispatcher.linkDisplay(5, "MIC", SettingMICData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(5);
                                    LabelDisplay5Tittle = "";
                                    LabelDisplay5XAxis = "";
                                    LabelDisplay5YAxis = "";
                                }
                                break;
                            case 6:
                                if (i < SettingMICData.SelectedDisplays.Count)
                                {
                                    OpenGLDispatcher.enableDisplay(6);
                                    string chDescription = SettingMICData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay6Tittle = SettingMICData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingMICData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingMICData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay6XAxis = "Time (s)";
                                    LabelDisplay6YAxis = "mV";
                                    OpenGLDispatcher.linkDisplay(6, "MIC", SettingMICData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(6);
                                    LabelDisplay6Tittle = "";
                                    LabelDisplay6XAxis = "";
                                    LabelDisplay6YAxis = "";
                                }
                                break;
                        }

                    }  
                }
                OnPropertyChanged("ToogleButtonMICIsChecked");
            }
        }
        private bool _toogleButtonECG1IsChecked = false;
        public bool ToogleButtonECG1IsChecked
        {
            get
            {
                return _toogleButtonECG1IsChecked;
            }
            set
            {
                _toogleButtonECG1IsChecked = value;
                if (_toogleButtonECG1IsChecked)
                {
                    ToogleButtonUserIsChecked = false;
                    ToogleButtonMICIsChecked = false;
                    ToogleButtonFBGAIsChecked = false;
                    ToogleButtonECG2IsChecked = false;
                    ToogleButtonACCIsChecked = false;
                    ToogleButtonPPGIsChecked = false;
                    DebugToggleButtonIsChecked = false;

                    DebugRichTextBoxIsVisible = false;
                    DebugStartStopButtonIsVisible = false;

                    for (int i = 0; i < 6; i++)
                    {

                        switch (i + 1)
                        {
                            case 1:
                                if (i < SettingECGData.NumberOfChannels)
                                {
                                    OpenGLDispatcher.enableDisplay(1);
                                    string chDescription = SettingECGData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay1Tittle = SettingECGData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingECGData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay1XAxis = "Time (s)";
                                    LabelDisplay1YAxis = "mV";
                                    OpenGLDispatcher.linkDisplay(1, "ECG", SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(1);
                                    LabelDisplay1Tittle = "";
                                    LabelDisplay1XAxis = "";
                                    LabelDisplay1YAxis = "";
                                }
                                break;
                            case 2:
                                if (i < SettingECGData.NumberOfChannels)
                                {
                                    OpenGLDispatcher.enableDisplay(2);
                                    string chDescription = SettingECGData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay2Tittle = SettingECGData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingECGData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay2XAxis = "Time (s)";
                                    LabelDisplay2YAxis = "mV";
                                    OpenGLDispatcher.linkDisplay(2, "ECG", SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(2);
                                    LabelDisplay2Tittle = "";
                                    LabelDisplay2XAxis = "";
                                    LabelDisplay2YAxis = "";
                                }
                                break;
                            case 3:
                                if (i < SettingECGData.NumberOfChannels)
                                {
                                    OpenGLDispatcher.enableDisplay(3);
                                    string chDescription = SettingECGData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay3Tittle = SettingECGData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingECGData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay3XAxis = "Time (s)";
                                    LabelDisplay3YAxis = "mV";
                                    OpenGLDispatcher.linkDisplay(3, "ECG", SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(3);
                                    LabelDisplay3Tittle = "";
                                    LabelDisplay3XAxis = "";
                                    LabelDisplay3YAxis = "";
                                }
                                break;
                            case 4:
                                if (i < SettingECGData.NumberOfChannels)
                                {
                                    OpenGLDispatcher.enableDisplay(4);
                                    string chDescription = SettingECGData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay4Tittle = SettingECGData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingECGData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay4XAxis = "Time (s)";
                                    LabelDisplay4YAxis = "mV";
                                    OpenGLDispatcher.linkDisplay(4, "ECG", SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(4);
                                    LabelDisplay4Tittle = "";
                                    LabelDisplay4XAxis = "";
                                    LabelDisplay4YAxis = "";
                                }
                                break;
                            case 5:
                                if (i < SettingECGData.NumberOfChannels)
                                {
                                    OpenGLDispatcher.enableDisplay(5);
                                    string chDescription = SettingECGData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay5Tittle = SettingECGData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingECGData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay5XAxis = "Time (s)";
                                    LabelDisplay5YAxis = "mV";
                                    OpenGLDispatcher.linkDisplay(5, "ECG", SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(5);
                                    LabelDisplay5Tittle = "";
                                    LabelDisplay5XAxis = "";
                                    LabelDisplay5YAxis = "";
                                }
                                break;
                            case 6:
                                if (i < SettingECGData.NumberOfChannels)
                                {
                                    OpenGLDispatcher.enableDisplay(6);
                                    string chDescription = SettingECGData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay6Tittle = SettingECGData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingECGData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay6XAxis = "Time (s)";
                                    LabelDisplay6YAxis = "mV";
                                    OpenGLDispatcher.linkDisplay(6, "ECG", SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(6);
                                    LabelDisplay6Tittle = "";
                                    LabelDisplay6XAxis = "";
                                    LabelDisplay6YAxis = "";
                                }
                                break;
                        }

                    }                  
                }
                OnPropertyChanged("ToogleButtonECG1IsChecked");
            }
        }
        private bool _toogleButtonECG2IsChecked = false;
        public bool ToogleButtonECG2IsChecked
        {
            get
            {
                return _toogleButtonECG2IsChecked;
            }
            set
            {
                _toogleButtonECG2IsChecked = value;
                if (_toogleButtonECG2IsChecked)
                {
                    ToogleButtonUserIsChecked = false;
                    ToogleButtonFBGAIsChecked = false;
                    ToogleButtonMICIsChecked = false;
                    ToogleButtonECG1IsChecked = false;
                    ToogleButtonACCIsChecked = false;
                    ToogleButtonPPGIsChecked = false;
                    DebugToggleButtonIsChecked = false;

                    DebugRichTextBoxIsVisible = false;
                    DebugStartStopButtonIsVisible = false;

                    for (int i = 6; i < 12; i++)
                    {

                        switch (i - 5)
                        {
                            case 1:
                                if (i < SettingECGData.NumberOfChannels)
                                {
                                    OpenGLDispatcher.enableDisplay(1);
                                    string chDescription = SettingECGData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay1Tittle = SettingECGData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingECGData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay1XAxis = "Time (s)";
                                    LabelDisplay1YAxis = "mV";
                                    OpenGLDispatcher.linkDisplay(1, "ECG", SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(1);
                                    LabelDisplay1Tittle = "";
                                    LabelDisplay1XAxis = "";
                                    LabelDisplay1YAxis = "";
                                }
                                break;
                            case 2:
                                if (i < SettingECGData.NumberOfChannels)
                                {
                                    OpenGLDispatcher.enableDisplay(2);
                                    string chDescription = SettingECGData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay2Tittle = SettingECGData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingECGData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay2XAxis = "Time (s)";
                                    LabelDisplay2YAxis = "mV";
                                    OpenGLDispatcher.linkDisplay(2, "ECG", SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(2);
                                    LabelDisplay2Tittle = "";
                                    LabelDisplay2XAxis = "";
                                    LabelDisplay2YAxis = "";
                                }
                                break;
                            case 3:
                                if (i < SettingECGData.NumberOfChannels)
                                {
                                    OpenGLDispatcher.enableDisplay(3);
                                    string chDescription = SettingECGData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay3Tittle = SettingECGData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingECGData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay3XAxis = "Time (s)";
                                    LabelDisplay3YAxis = "mV";
                                    OpenGLDispatcher.linkDisplay(3, "ECG", SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(3);
                                    LabelDisplay3Tittle = "";
                                    LabelDisplay3XAxis = "";
                                    LabelDisplay3YAxis = "";
                                }
                                break;
                            case 4:
                                if (i < SettingECGData.NumberOfChannels)
                                {
                                    OpenGLDispatcher.enableDisplay(4);
                                    string chDescription = SettingECGData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay4Tittle = SettingECGData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingECGData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay4XAxis = "Time (s)";
                                    LabelDisplay4YAxis = "mV";
                                    OpenGLDispatcher.linkDisplay(4, "ECG", SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(4);
                                    LabelDisplay4Tittle = "";
                                    LabelDisplay4XAxis = "";
                                    LabelDisplay4YAxis = "";
                                }
                                break;
                            case 5:
                                if (i < SettingECGData.NumberOfChannels)
                                {
                                    OpenGLDispatcher.enableDisplay(5);
                                    string chDescription = SettingECGData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay5Tittle = SettingECGData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingECGData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay5XAxis = "Time (s)";
                                    LabelDisplay5YAxis = "mV";
                                    OpenGLDispatcher.linkDisplay(5, "ECG", SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(5);
                                    LabelDisplay5Tittle = "";
                                    LabelDisplay5XAxis = "";
                                    LabelDisplay5YAxis = "";
                                }
                                break;
                            case 6:
                                if (i < SettingECGData.NumberOfChannels)
                                {
                                    OpenGLDispatcher.enableDisplay(6);
                                    string chDescription = SettingECGData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay6Tittle = SettingECGData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingECGData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingECGData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay6XAxis = "Time (s)";
                                    LabelDisplay6YAxis = "mV";
                                    OpenGLDispatcher.linkDisplay(6, "ECG", SettingMICData.SelectedDisplays.ElementAt(i).ChannelNumber);
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(6);
                                    LabelDisplay6Tittle = "";
                                    LabelDisplay6XAxis = "";
                                    LabelDisplay6YAxis = "";
                                }
                                break;
                        }

                    }
                }
                OnPropertyChanged("ToogleButtonECG2IsChecked");
            }
        }
        private bool _toogleButtonACCIsChecked = false;
        public bool ToogleButtonACCIsChecked
        {
            get
            {
                return _toogleButtonACCIsChecked;
            }
            set
            {
                _toogleButtonACCIsChecked = value;
                if (_toogleButtonACCIsChecked)
                {
                    ToogleButtonUserIsChecked = false;
                    ToogleButtonFBGAIsChecked = false;
                    ToogleButtonMICIsChecked = false;
                    ToogleButtonECG1IsChecked = false;
                    ToogleButtonECG2IsChecked = false;
                    ToogleButtonPPGIsChecked = false;
                    DebugToggleButtonIsChecked = false;

                    DebugRichTextBoxIsVisible = false;
                    DebugStartStopButtonIsVisible = false;

                    for(int i=0;i< 6; i++) 
                    {

                        switch (i + 1)
                        {
                            case 1:
                                if (i < SettingACCData.SelectedDisplays.Count)
                                {
                                    OpenGLDispatcher.enableDisplay(1);
                                    string chDescription = SettingACCData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay1Tittle = SettingACCData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingACCData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay1XAxis = "Time (s)";
                                    LabelDisplay1YAxis = "mV";
                                    if (String.Compare(SettingACCData.SelectedDisplays.ElementAt(i).Axis, "x") == 0)
                                        OpenGLDispatcher.linkDisplay(1, "ACC", (SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 3 + 1);
                                    else if (String.Compare(SettingACCData.SelectedDisplays.ElementAt(i).Axis, "y") == 0)
                                        OpenGLDispatcher.linkDisplay(1, "ACC", (SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 3 + 2);
                                    else if (String.Compare(SettingACCData.SelectedDisplays.ElementAt(i).Axis, "z") == 0)
                                        OpenGLDispatcher.linkDisplay(1, "ACC", (SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 3 + 3);                                    
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(1);
                                    LabelDisplay1Tittle = "";
                                    LabelDisplay1XAxis = "";
                                    LabelDisplay1YAxis = "";
                                }
                                break;
                            case 2:
                                if (i < SettingACCData.SelectedDisplays.Count)
                                {
                                    OpenGLDispatcher.enableDisplay(2);
                                    string chDescription = SettingACCData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay2Tittle = SettingACCData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingACCData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay2XAxis = "Time (s)";
                                    LabelDisplay2YAxis = "mV";
                                    if (String.Compare(SettingACCData.SelectedDisplays.ElementAt(i).Axis, "x") == 0)
                                        OpenGLDispatcher.linkDisplay(2, "ACC", (SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 3 + 1);
                                    else if (String.Compare(SettingACCData.SelectedDisplays.ElementAt(i).Axis, "y") == 0)
                                        OpenGLDispatcher.linkDisplay(2, "ACC", (SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 3 + 2);
                                    else if (String.Compare(SettingACCData.SelectedDisplays.ElementAt(i).Axis, "z") == 0)
                                        OpenGLDispatcher.linkDisplay(2, "ACC", (SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 3 + 3);                                    
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(2);
                                    LabelDisplay2Tittle = "";
                                    LabelDisplay2XAxis = "";
                                    LabelDisplay2YAxis = "";
                                }
                                break;
                            case 3:
                                if (i < SettingACCData.SelectedDisplays.Count)
                                {
                                    OpenGLDispatcher.enableDisplay(3);
                                    string chDescription = SettingACCData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay3Tittle = SettingACCData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingACCData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay3XAxis = "Time (s)";
                                    LabelDisplay3YAxis = "mV";
                                    if (String.Compare(SettingACCData.SelectedDisplays.ElementAt(i).Axis, "x") == 0)
                                        OpenGLDispatcher.linkDisplay(3, "ACC", (SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 3 + 1);
                                    else if (String.Compare(SettingACCData.SelectedDisplays.ElementAt(i).Axis, "y") == 0)
                                        OpenGLDispatcher.linkDisplay(3, "ACC", (SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 3 + 2);
                                    else if (String.Compare(SettingACCData.SelectedDisplays.ElementAt(i).Axis, "z") == 0)
                                        OpenGLDispatcher.linkDisplay(3, "ACC", (SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 3 + 3);                                    
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(3);
                                    LabelDisplay3Tittle = "";
                                    LabelDisplay3XAxis = "";
                                    LabelDisplay3YAxis = "";
                                }
                                break;
                            case 4:
                                if (i < SettingACCData.SelectedDisplays.Count)
                                {
                                    OpenGLDispatcher.enableDisplay(4);
                                    string chDescription = SettingACCData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay4Tittle = SettingACCData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingACCData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay4XAxis = "Time (s)";
                                    LabelDisplay4YAxis = "mV";
                                    if (String.Compare(SettingACCData.SelectedDisplays.ElementAt(i).Axis, "x") == 0)
                                        OpenGLDispatcher.linkDisplay(4, "ACC", (SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 3 + 1);
                                    else if (String.Compare(SettingACCData.SelectedDisplays.ElementAt(i).Axis, "y") == 0)
                                        OpenGLDispatcher.linkDisplay(4, "ACC", (SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 3 + 2);
                                    else if (String.Compare(SettingACCData.SelectedDisplays.ElementAt(i).Axis, "z") == 0)
                                        OpenGLDispatcher.linkDisplay(4, "ACC", (SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 3 + 3);                                    
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(4);
                                    LabelDisplay4Tittle = "";
                                    LabelDisplay4XAxis = "";
                                    LabelDisplay4YAxis = "";
                                }
                                break;
                            case 5:
                                if (i < SettingACCData.SelectedDisplays.Count)
                                {
                                    OpenGLDispatcher.enableDisplay(5);
                                    string chDescription = SettingACCData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay5Tittle = SettingACCData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingACCData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay5XAxis = "Time (s)";
                                    LabelDisplay5YAxis = "mV";
                                    if (String.Compare(SettingACCData.SelectedDisplays.ElementAt(i).Axis, "x") == 0)
                                        OpenGLDispatcher.linkDisplay(5, "ACC", (SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 3 + 1);
                                    else if (String.Compare(SettingACCData.SelectedDisplays.ElementAt(i).Axis, "y") == 0)
                                        OpenGLDispatcher.linkDisplay(5, "ACC", (SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 3 + 2);
                                    else if (String.Compare(SettingACCData.SelectedDisplays.ElementAt(i).Axis, "z") == 0)
                                        OpenGLDispatcher.linkDisplay(5, "ACC", (SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 3 + 3);                                    
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(5);
                                    LabelDisplay5Tittle = "";
                                    LabelDisplay5XAxis = "";
                                    LabelDisplay5YAxis = "";
                                }
                                break;
                            case 6:
                                if (i < SettingACCData.SelectedDisplays.Count)
                                {
                                    OpenGLDispatcher.enableDisplay(6);
                                    string chDescription = SettingACCData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay6Tittle = SettingACCData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingACCData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay6XAxis = "Time (s)";
                                    LabelDisplay6YAxis = "mV";
                                    if (String.Compare(SettingACCData.SelectedDisplays.ElementAt(i).Axis, "x") == 0)
                                        OpenGLDispatcher.linkDisplay(6, "ACC", (SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 3 + 1);
                                    else if (String.Compare(SettingACCData.SelectedDisplays.ElementAt(i).Axis, "y") == 0)
                                        OpenGLDispatcher.linkDisplay(6, "ACC", (SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 3 + 2);
                                    else if (String.Compare(SettingACCData.SelectedDisplays.ElementAt(i).Axis, "z") == 0)
                                        OpenGLDispatcher.linkDisplay(6, "ACC", (SettingACCData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 3 + 3);                                    
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(6);
                                    LabelDisplay6Tittle = "";
                                    LabelDisplay6XAxis = "";
                                    LabelDisplay6YAxis = "";
                                }
                                break;
                        }

                    }
                }
                OnPropertyChanged("ToogleButtonACCIsChecked");
            }
        }
        private bool _toogleButtonPPGIsChecked = false;
        public bool ToogleButtonPPGIsChecked
        {
            get
            {
                return _toogleButtonPPGIsChecked;
            }
            set
            {
                _toogleButtonPPGIsChecked = value;
                if (_toogleButtonPPGIsChecked)
                {
                    
                    ToogleButtonUserIsChecked = false;
                    ToogleButtonFBGAIsChecked = false;
                    ToogleButtonMICIsChecked = false;
                    ToogleButtonECG1IsChecked = false;
                    ToogleButtonECG2IsChecked = false;
                    ToogleButtonACCIsChecked = false;
                    DebugToggleButtonIsChecked = false;

                    DebugRichTextBoxIsVisible = false;
                    DebugStartStopButtonIsVisible = false;

                    for (int i = 0; i < 6; i++)
                    {

                        switch (i + 1)
                        {
                            case 1:
                                if (i < SettingPPGData.NumberOfChannels)
                                {
                                    OpenGLDispatcher.enableDisplay(1);
                                    string chDescription = SettingPPGData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay1Tittle = SettingPPGData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingPPGData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingPPGData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay1XAxis = "Time (s)";
                                    LabelDisplay1YAxis = "mV";
                                    if (String.Compare(SettingPPGData.SelectedDisplays.ElementAt(i).Axis, "g") == 0)
                                        OpenGLDispatcher.linkDisplay(1, "PPG", (SettingPPGData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 2 + 1);
                                    else if (String.Compare(SettingPPGData.SelectedDisplays.ElementAt(i).Axis, "r") == 0)
                                        OpenGLDispatcher.linkDisplay(1, "PPG", (SettingPPGData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 2 + 2);                                                                       
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(1);
                                    LabelDisplay1Tittle = "";
                                    LabelDisplay1XAxis = "";
                                    LabelDisplay1YAxis = "";
                                }
                                break;
                            case 2:
                                if (i < SettingPPGData.NumberOfChannels)
                                {
                                    OpenGLDispatcher.enableDisplay(2);
                                    string chDescription = SettingPPGData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay2Tittle = SettingPPGData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingPPGData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingPPGData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay2XAxis = "Time (s)";
                                    LabelDisplay2YAxis = "mV";
                                    if (String.Compare(SettingPPGData.SelectedDisplays.ElementAt(i).Axis, "g") == 0)
                                        OpenGLDispatcher.linkDisplay(2, "PPG", (SettingPPGData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 2 + 1);
                                    else if (String.Compare(SettingPPGData.SelectedDisplays.ElementAt(i).Axis, "r") == 0)
                                        OpenGLDispatcher.linkDisplay(2, "PPG", (SettingPPGData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 2 + 2);                                    
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(2);
                                    LabelDisplay2Tittle = "";
                                    LabelDisplay2XAxis = "";
                                    LabelDisplay2YAxis = "";
                                }
                                break;
                            case 3:
                                if (i < SettingPPGData.NumberOfChannels)
                                {
                                    OpenGLDispatcher.enableDisplay(3);
                                    string chDescription = SettingPPGData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay3Tittle = SettingPPGData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingPPGData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingPPGData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay3XAxis = "Time (s)";
                                    LabelDisplay3YAxis = "mV";
                                    if (String.Compare(SettingPPGData.SelectedDisplays.ElementAt(i).Axis, "g") == 0)
                                        OpenGLDispatcher.linkDisplay(3, "PPG", (SettingPPGData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 2 + 1);
                                    else if (String.Compare(SettingPPGData.SelectedDisplays.ElementAt(i).Axis, "r") == 0)
                                        OpenGLDispatcher.linkDisplay(3, "PPG", (SettingPPGData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 2 + 2);                                    
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(3);
                                    LabelDisplay3Tittle = "";
                                    LabelDisplay3XAxis = "";
                                    LabelDisplay3YAxis = "";
                                }
                                break;
                            case 4:
                                if (i < SettingPPGData.NumberOfChannels)
                                {
                                    OpenGLDispatcher.enableDisplay(4);
                                    string chDescription = SettingPPGData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay4Tittle = SettingPPGData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingPPGData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingPPGData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay4XAxis = "Time (s)";
                                    LabelDisplay4YAxis = "mV";
                                    if (String.Compare(SettingPPGData.SelectedDisplays.ElementAt(i).Axis, "g") == 0)
                                        OpenGLDispatcher.linkDisplay(4, "PPG", (SettingPPGData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 2 + 1);
                                    else if (String.Compare(SettingPPGData.SelectedDisplays.ElementAt(i).Axis, "r") == 0)
                                        OpenGLDispatcher.linkDisplay(4, "PPG", (SettingPPGData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 2 + 2);                                    
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(4);
                                    LabelDisplay4Tittle = "";
                                    LabelDisplay4XAxis = "";
                                    LabelDisplay4YAxis = "";
                                }
                                break;
                            case 5:
                                if (i < SettingPPGData.NumberOfChannels)
                                {
                                    OpenGLDispatcher.enableDisplay(5);
                                    string chDescription = SettingPPGData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay5Tittle = SettingPPGData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingPPGData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingPPGData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay5XAxis = "Time (s)";
                                    LabelDisplay5YAxis = "mV";
                                    if (String.Compare(SettingPPGData.SelectedDisplays.ElementAt(i).Axis, "g") == 0)
                                        OpenGLDispatcher.linkDisplay(5, "PPG", (SettingPPGData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 2 + 1);
                                    else if (String.Compare(SettingPPGData.SelectedDisplays.ElementAt(i).Axis, "r") == 0)
                                        OpenGLDispatcher.linkDisplay(5, "PPG", (SettingPPGData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 2 + 2);                                    
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(5);
                                    LabelDisplay5Tittle = "";
                                    LabelDisplay5XAxis = "";
                                    LabelDisplay5YAxis = "";
                                }
                                break;
                            case 6:
                                if (i < SettingPPGData.NumberOfChannels)
                                {
                                    OpenGLDispatcher.enableDisplay(6);
                                    string chDescription = SettingPPGData.SelectedDisplays.ElementAt(i).Description;
                                    string tempDescription;
                                    if (chDescription.Length > 0) tempDescription = " (" + chDescription + ")";
                                    else tempDescription = "";
                                    LabelDisplay6Tittle = SettingPPGData.SelectedDisplays.ElementAt(i).ModuleName + "_CH" + SettingPPGData.SelectedDisplays.ElementAt(i).ChannelNumber + SettingPPGData.SelectedDisplays.ElementAt(i).Axis + tempDescription;
                                    LabelDisplay6XAxis = "Time (s)";
                                    LabelDisplay6YAxis = "mV";
                                    if (String.Compare(SettingPPGData.SelectedDisplays.ElementAt(i).Axis, "g") == 0)
                                        OpenGLDispatcher.linkDisplay(6, "PPG", (SettingPPGData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 2 + 1);
                                    else if (String.Compare(SettingPPGData.SelectedDisplays.ElementAt(i).Axis, "r") == 0)
                                        OpenGLDispatcher.linkDisplay(6, "PPG", (SettingPPGData.SelectedDisplays.ElementAt(i).ChannelNumber - 1) * 2 + 2);                                    
                                }
                                else
                                {
                                    OpenGLDispatcher.disableDisplay(6);
                                    LabelDisplay6Tittle = "";
                                    LabelDisplay6XAxis = "";
                                    LabelDisplay6YAxis = "";
                                }                                    
                                break;
                                    
                        }

                    }
                    
                }
                OnPropertyChanged("ToogleButtonPPGIsChecked");
            }
        }
        #endregion

        #region OPENGL
        public OpenGLDispatcher OpenGLDispatcher { 
            get;
            private set; 
        }
        #endregion

        #region OPENGL_LABELS
        private string _labelDisplay1Tittle;
        public string LabelDisplay1Tittle {
            get {
                return _labelDisplay1Tittle;
            }
            set {
                _labelDisplay1Tittle = value;
                OnPropertyChanged("LabelDisplay1Tittle");
            }
        }
        private string _labelDisplay1YAxis;
        public string LabelDisplay1YAxis
        {
            get
            {
                return _labelDisplay1YAxis;
            }
            set
            {
                _labelDisplay1YAxis = value;
                OnPropertyChanged("LabelDisplay1YAxis");
            }
        }
        private string _labelDisplay1XAxis;
        public string LabelDisplay1XAxis
        {
            get
            {
                return _labelDisplay1XAxis;
            }
            set
            {
                _labelDisplay1XAxis = value;
                OnPropertyChanged("LabelDisplay1XAxis");
            }
        }

        private string _labelDisplay2Tittle;
        public string LabelDisplay2Tittle
        {
            get
            {
                return _labelDisplay2Tittle;
            }
            set
            {
                _labelDisplay2Tittle = value;
                OnPropertyChanged("LabelDisplay2Tittle");
            }
        }
        private string _labelDisplay2YAxis;
        public string LabelDisplay2YAxis
        {
            get
            {
                return _labelDisplay2YAxis;
            }
            set
            {
                _labelDisplay2YAxis = value;
                OnPropertyChanged("LabelDisplay2YAxis");
            }
        }
        private string _labelDisplay2XAxis;
        public string LabelDisplay2XAxis
        {
            get
            {
                return _labelDisplay2XAxis;
            }
            set
            {
                _labelDisplay2XAxis = value;
                OnPropertyChanged("LabelDisplay2XAxis");
            }
        }

        private string _labelDisplay3Tittle;
        public string LabelDisplay3Tittle
        {
            get
            {
                return _labelDisplay3Tittle;
            }
            set
            {
                _labelDisplay3Tittle = value;
                OnPropertyChanged("LabelDisplay3Tittle");
            }
        }
        private string _labelDisplay3YAxis;
        public string LabelDisplay3YAxis
        {
            get
            {
                return _labelDisplay3YAxis;
            }
            set
            {
                _labelDisplay3YAxis = value;
                OnPropertyChanged("LabelDisplay3YAxis");
            }
        }
        private string _labelDisplay3XAxis;
        public string LabelDisplay3XAxis
        {
            get
            {
                return _labelDisplay3XAxis;
            }
            set
            {
                _labelDisplay3XAxis = value;
                OnPropertyChanged("LabelDisplay3XAxis");
            }
        }

        private string _labelDisplay4Tittle;
        public string LabelDisplay4Tittle
        {
            get
            {
                return _labelDisplay4Tittle;
            }
            set
            {
                _labelDisplay4Tittle = value;
                OnPropertyChanged("LabelDisplay4Tittle");
            }
        }
        private string _labelDisplay4YAxis;
        public string LabelDisplay4YAxis
        {
            get
            {
                return _labelDisplay4YAxis;
            }
            set
            {
                _labelDisplay4YAxis = value;
                OnPropertyChanged("LabelDisplay4YAxis");
            }
        }
        private string _labelDisplay4XAxis;
        public string LabelDisplay4XAxis
        {
            get
            {
                return _labelDisplay4XAxis;
            }
            set
            {
                _labelDisplay4XAxis = value;
                OnPropertyChanged("LabelDisplay4XAxis");
            }
        }

        private string _labelDisplay5Tittle;
        public string LabelDisplay5Tittle
        {
            get
            {
                return _labelDisplay5Tittle;
            }
            set
            {
                _labelDisplay5Tittle = value;
                OnPropertyChanged("LabelDisplay5Tittle");
            }
        }
        private string _labelDisplay5YAxis;
        public string LabelDisplay5YAxis
        {
            get
            {
                return _labelDisplay5YAxis;
            }
            set
            {
                _labelDisplay5YAxis = value;
                OnPropertyChanged("LabelDisplay5YAxis");
            }
        }
        private string _labelDisplay5XAxis;
        public string LabelDisplay5XAxis
        {
            get
            {
                return _labelDisplay5XAxis;
            }
            set
            {
                _labelDisplay5XAxis = value;
                OnPropertyChanged("LabelDisplay5XAxis");
            }
        }

        private string _labelDisplay6Tittle;
        public string LabelDisplay6Tittle
        {
            get
            {
                return _labelDisplay6Tittle;
            }
            set
            {
                _labelDisplay6Tittle = value;
                OnPropertyChanged("LabelDisplay6Tittle");
            }
        }
        private string _labelDisplay6YAxis;
        public string LabelDisplay6YAxis
        {
            get
            {
                return _labelDisplay6YAxis;
            }
            set
            {
                _labelDisplay6YAxis = value;
                OnPropertyChanged("LabelDisplay6YAxis");
            }
        }
        private string _labelDisplay6XAxis;
        public string LabelDisplay6XAxis
        {
            get
            {
                return _labelDisplay6XAxis;
            }
            set
            {
                _labelDisplay6XAxis = value;
                OnPropertyChanged("LabelDisplay6XAxis");
            }
        }
   
        #endregion

        #region STATUS_BAR
        
        private bool batteryVoltageAvailable = false;
        private bool batteryLevelLow = false;
        private int batteryPercentage = 0;
        public int BatteryPercentage {
            get
            {
                return batteryPercentage;
            }
            set
            {
                batteryPercentage = value;
                if (batteryPercentage == 0)
                    batteryVoltageAvailable = false;
                if (batteryVoltageAvailable)
                {
                    BatteryPercentageLabelText = "Battery: " + String.Format("{0} %", batteryPercentage);
                }
                else {
                    BatteryPercentageLabelText = "Battery: N/A";
                }
            }
        }

        private float batteryValue = 0.0f;
        public float BatteryValue {
            get {
                return batteryValue;
            }
            set {
                batteryValue = value;
                if (batteryValue == 0)
                    batteryVoltageAvailable = false;
                else 
                    batteryVoltageAvailable = true;
                if (batteryVoltageAvailable)
                {
                    BatteryVoltageLabelText = "Battery voltage: " + String.Format("{0:0.00} V", batteryValue);
                    if ((batteryValue < SettingProgramData.BatteryAlertVoltage) && (!batteryLevelLow))
                    {
                        batteryLevelLow = true;
                        Log log = new Log();
                        log.LogMessageToFile("Warning: Battery level low");
                        Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Warning, "Battery level low!");
                        Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);
                        BatteryPercentageLabelColor = Brushes.Red;
                    }
                    if ((batteryValue > SettingProgramData.BatteryAlertVoltage) && (batteryLevelLow))
                    {
                        batteryLevelLow = false;
                        BatteryPercentageLabelColor = Brushes.Green;
                    }
                    
                }
                else {
                    BatteryVoltageLabelText = "Battery voltage: N/A";
                   
                }
                
            }
        }
        private string batteryVoltageLabelText = "Battery voltage: N/A";
        public string BatteryVoltageLabelText
        {
            get
            {
                return batteryVoltageLabelText;
            }
            set
            {
                batteryVoltageLabelText = value;
                OnPropertyChanged("BatteryVoltageLabelText");
            }
        }
        private string batteryPercentageLabelText = "Battery: N/A";
        public string BatteryPercentageLabelText {
            get {
                return batteryPercentageLabelText;
            }
            set {
                batteryPercentageLabelText = value;
                OnPropertyChanged("BatteryPercentageLabelText");
            }
        }

        private Brush batteryPercentageLabelColor = Brushes.Green;
        public Brush BatteryPercentageLabelColor {
            get {
                return batteryPercentageLabelColor;
            }
            set {
                batteryPercentageLabelColor = value;
                OnPropertyChanged("BatteryPercentageLabelColor");
            }
        }

        private bool batteryVoltageLabelIsVisible = false;
        public bool BatteryVoltageLabelIsVisible {
            get {
                return batteryVoltageLabelIsVisible;
            }
            set {
                batteryVoltageLabelIsVisible = value;
                OnPropertyChanged("BatteryVoltageLabelIsVisible");
            }
        }
        
        

        #endregion

        #region DIALOGS
        private void OnOpenDialog(object parameter)
        {
            /*Dialogs.DialogService.DialogViewModelBase vm =new Dialogs.DialogYesNo.DialogYesNoViewModel("DialogView","Question");
            Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(vm,parameter as Window);*/
           /* Dialogs.DialogService.DialogViewModelBase vm = new Dialogs.DialogMessage.DialogMessageViewModel(DialogImageTypeEnum.Error,"TCP/ip error: TCP/IP protocol is not enabled. Please connect ethernet cable or provide setting");
            Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(vm, parameter as Window);*/
           
        }
        
        #endregion

        #region SINC_DATA
        #region MIC_MODULE
        public int micNoChannels = 4;
        public int micSampleRate = 500;
        public string micModuleStatusString = "Disconnected";
        private TCPModuleStatusEnumType _micModuleStatus = TCPModuleStatusEnumType.DISCONNECTED;
        public TCPModuleStatusEnumType MicModuleStatus
        {
            get {
                return _micModuleStatus;
            }
            set {
                _micModuleStatus = value;
                switch (_micModuleStatus){
                    case TCPModuleStatusEnumType.DISCONNECTED:
                        if (treeViewViewModel.Modules.Count > 0)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(0).ModuleStatusColor = new SolidColorBrush(Colors.Red);
                                treeViewViewModel.Modules.ElementAt(0).IsEnabled = false;
                                treeViewViewModel.Modules.ElementAt(0).IsExpanded = false;
                                micModuleStatusString = "Disconnected";
                            }));
                        }
                        break;
                    case TCPModuleStatusEnumType.CONNECTED:
                        if (treeViewViewModel.Modules.Count > 0) {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(0).ModuleStatusColor = new SolidColorBrush(Colors.Orange);
                                treeViewViewModel.Modules.ElementAt(0).IsEnabled = false;
                                treeViewViewModel.Modules.ElementAt(0).IsExpanded = false;
                                micModuleStatusString = "Connected (No idle data)";
                            }));
                        }
                        break;
                    case TCPModuleStatusEnumType.CONNECTED_IDLE:
                        if (treeViewViewModel.Modules.Count > 0)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(0).ModuleStatusColor = new SolidColorBrush(Colors.Green);
                                treeViewViewModel.Modules.ElementAt(0).IsEnabled = true;
                                treeViewViewModel.Modules.ElementAt(0).IsExpanded = true;
                                micModuleStatusString = "Connected (Idle)";
                            }));
                        }
                        break;
                    case TCPModuleStatusEnumType.CONNECTED_TRANSFERING:
                        if (treeViewViewModel.Modules.Count > 0)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(0).ModuleStatusColor = new SolidColorBrush(Colors.Blue);
                                treeViewViewModel.Modules.ElementAt(0).IsEnabled = true;
                                treeViewViewModel.Modules.ElementAt(0).IsExpanded = true;
                                micModuleStatusString = "Connected (Transfering)";
                            }));
                        }
                        break;
                }
                //OnPropertyChanged("ModuleStatus");
            }
        }
        #endregion

        #region FBGA_MODULE
        public int fbgaNoChannels = 1;
        public int fbgaSampleRate = 500;
        public string fbgaModuleStatusString = "Disconnected (MWLS-NI)"; 
        private USBModuleStatusEnumType _fbgaModuleStatus = USBModuleStatusEnumType.NOT_INITIALIZED;
        public USBModuleStatusEnumType FBGAModuleStatus
        {
            get
            {
                return _fbgaModuleStatus;
            }
            set
            {
                _fbgaModuleStatus = value;
                switch (_fbgaModuleStatus)
                {
                    case USBModuleStatusEnumType.NOT_INITIALIZED:
                        if (treeViewViewModel.Modules.Count > 1)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(1).ModuleStatusColor = new SolidColorBrush(Colors.Red);
                                treeViewViewModel.Modules.ElementAt(1).IsEnabled = false;
                                treeViewViewModel.Modules.ElementAt(1).IsExpanded = false;
                                fbgaModuleStatusString = "Disconnected";
                            }));
                        }
                        break;
                   /* case USBModuleStatusEnumType.INITIALIZED_OFF:
                        if (treeViewViewModel.Modules.Count > 1)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(1).ModuleStatusColor = new SolidColorBrush(Colors.Orange);
                                treeViewViewModel.Modules.ElementAt(1).IsEnabled = true;
                                treeViewViewModel.Modules.ElementAt(1).IsExpanded = false;
                                fbgaModuleStatusString = "Connected (MWLS-OFF)";
                            }));
                        }
                        break;*/
                    case USBModuleStatusEnumType.INITIALIZED:
                        if (treeViewViewModel.Modules.Count > 1)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(1).ModuleStatusColor = new SolidColorBrush(Colors.Green);
                                treeViewViewModel.Modules.ElementAt(1).IsEnabled = true;
                                treeViewViewModel.Modules.ElementAt(1).IsExpanded = true;
                                fbgaModuleStatusString = "Connected";
                            }));
                        }
                        break;
                    case USBModuleStatusEnumType.TRANSFERING:
                        if (treeViewViewModel.Modules.Count > 1)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(1).ModuleStatusColor = new SolidColorBrush(Colors.Blue);
                                treeViewViewModel.Modules.ElementAt(1).IsEnabled = true;
                                treeViewViewModel.Modules.ElementAt(1).IsExpanded = true;
                                fbgaModuleStatusString = "Connected-Transfering";
                            }));
                        }
                        break;
                }                
                //OnPropertyChanged("FBGAModuleStatus");
            }
        }
        #endregion

        #region ECG_MODULE
        public int ecgNoChannels = 13;
        public int ecgSampleRate = 500;
        public string ecgModuleStatusString = "Disconnected";
        private TCPModuleStatusEnumType _ecgModuleStatus = TCPModuleStatusEnumType.DISCONNECTED;
        public TCPModuleStatusEnumType ECGModuleStatus
        {
            get
            {
                return _ecgModuleStatus;
            }
            set
            {
                _ecgModuleStatus = value;
                switch (_ecgModuleStatus)
                {
                    case TCPModuleStatusEnumType.DISCONNECTED:
                        if (treeViewViewModel.Modules.Count > 2)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(2).ModuleStatusColor = new SolidColorBrush(Colors.Red);
                                treeViewViewModel.Modules.ElementAt(2).IsEnabled = false;
                                treeViewViewModel.Modules.ElementAt(2).IsExpanded = false;
                                ecgModuleStatusString = "Disconnected";
                            }));
                        }
                        break;
                    case TCPModuleStatusEnumType.CONNECTED:
                        if (treeViewViewModel.Modules.Count > 2)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(2).ModuleStatusColor = new SolidColorBrush(Colors.Orange);
                                treeViewViewModel.Modules.ElementAt(2).IsEnabled = false;
                                treeViewViewModel.Modules.ElementAt(2).IsExpanded = false;
                                ecgModuleStatusString = "Connected (No idle data)";
                            }));
                        }
                        break;
                    case TCPModuleStatusEnumType.CONNECTED_IDLE:
                        if (treeViewViewModel.Modules.Count > 2)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(2).ModuleStatusColor = new SolidColorBrush(Colors.Green);
                                treeViewViewModel.Modules.ElementAt(2).IsEnabled = true;
                                treeViewViewModel.Modules.ElementAt(2).IsExpanded = true;
                                ecgModuleStatusString = "Connected (Idle)";
                            }));
                        }
                        break;
                    case TCPModuleStatusEnumType.CONNECTED_TRANSFERING:
                        if (treeViewViewModel.Modules.Count > 2)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(2).ModuleStatusColor = new SolidColorBrush(Colors.Blue);
                                treeViewViewModel.Modules.ElementAt(2).IsEnabled = true;
                                treeViewViewModel.Modules.ElementAt(2).IsExpanded = true;
                                ecgModuleStatusString = "Connected (Transfering)";
                            }));
                        }
                        break;
                }
                //OnPropertyChanged("ECGModuleStatus");
            }
        }
        #endregion

        #region ACC_MODULE
        public int accNoChannels = 4;
        public int accSampleRate = 500;
        public string accModuleStatusString = "Disconnected";
        private TCPModuleStatusEnumType _accModuleStatus = TCPModuleStatusEnumType.DISCONNECTED;
        public TCPModuleStatusEnumType ACCModuleStatus
        {
            get
            {
                return _accModuleStatus;
            }
            set
            {
                _accModuleStatus = value;
                switch (_accModuleStatus)
                {
                    case TCPModuleStatusEnumType.DISCONNECTED:
                        if (treeViewViewModel.Modules.Count > 3)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(3).ModuleStatusColor = new SolidColorBrush(Colors.Red);
                                treeViewViewModel.Modules.ElementAt(3).IsEnabled = false;
                                treeViewViewModel.Modules.ElementAt(3).IsExpanded = false;
                                accModuleStatusString = "Disconnected";
                            }));
                        }
                        break;
                    case TCPModuleStatusEnumType.CONNECTED:
                        if (treeViewViewModel.Modules.Count > 3)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(3).ModuleStatusColor = new SolidColorBrush(Colors.Orange);
                                treeViewViewModel.Modules.ElementAt(3).IsEnabled = false;
                                treeViewViewModel.Modules.ElementAt(3).IsExpanded = false;
                                accModuleStatusString = "Connected (No idle data)";
                            }));
                        }
                        break;
                    case TCPModuleStatusEnumType.CONNECTED_IDLE:
                        if (treeViewViewModel.Modules.Count > 3)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(3).ModuleStatusColor = new SolidColorBrush(Colors.Green);
                                treeViewViewModel.Modules.ElementAt(3).IsEnabled = true;
                                treeViewViewModel.Modules.ElementAt(3).IsExpanded = true;
                                accModuleStatusString = "Connected (Idle)";
                            }));
                        }
                        break;
                    case TCPModuleStatusEnumType.CONNECTED_TRANSFERING:
                        if (treeViewViewModel.Modules.Count > 3)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(3).ModuleStatusColor = new SolidColorBrush(Colors.Blue);
                                treeViewViewModel.Modules.ElementAt(3).IsEnabled = true;
                                treeViewViewModel.Modules.ElementAt(3).IsExpanded = true;
                                accModuleStatusString = "Connected (Transfering)";
                            }));
                        }
                        break;
                }
                //OnPropertyChanged("ACCModuleStatus");
            }
        }
        #endregion

        #region PPG_MODULE
        public int ppgNoChannels = 4;
        public int ppgSampleRate = 500;
        public string ppgModuleStatusString = "Disconnected";
        private TCPModuleStatusEnumType _ppgModuleStatus = TCPModuleStatusEnumType.DISCONNECTED;
        public TCPModuleStatusEnumType PPGModuleStatus
        {
            get
            {
                return _ppgModuleStatus;
            }
            set
            {
                _ppgModuleStatus = value;
                switch (_ppgModuleStatus)
                {
                    case TCPModuleStatusEnumType.DISCONNECTED:
                        if (treeViewViewModel.Modules.Count > 4)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(4).ModuleStatusColor = new SolidColorBrush(Colors.Red);
                                treeViewViewModel.Modules.ElementAt(4).IsEnabled = false;
                                treeViewViewModel.Modules.ElementAt(4).IsExpanded = false;
                                ppgModuleStatusString = "Disconnected";
                            }));
                        }
                        break;
                    case TCPModuleStatusEnumType.CONNECTED:
                        if (treeViewViewModel.Modules.Count > 4)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(4).ModuleStatusColor = new SolidColorBrush(Colors.Orange);
                                treeViewViewModel.Modules.ElementAt(4).IsEnabled = false;
                                treeViewViewModel.Modules.ElementAt(4).IsExpanded = false;
                                ppgModuleStatusString = "Connected (No idle data)";
                            }));
                        }
                        break;
                    case TCPModuleStatusEnumType.CONNECTED_IDLE:
                        if (treeViewViewModel.Modules.Count > 4)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(4).ModuleStatusColor = new SolidColorBrush(Colors.Green);
                                treeViewViewModel.Modules.ElementAt(4).IsEnabled = true;
                                treeViewViewModel.Modules.ElementAt(4).IsExpanded = true;
                                ppgModuleStatusString = "Connected (Idle)";
                            }));
                        }
                        break;
                    case TCPModuleStatusEnumType.CONNECTED_TRANSFERING:
                        if (treeViewViewModel.Modules.Count > 4)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                treeViewViewModel.Modules.ElementAt(4).ModuleStatusColor = new SolidColorBrush(Colors.Blue);
                                treeViewViewModel.Modules.ElementAt(4).IsEnabled = true;
                                treeViewViewModel.Modules.ElementAt(4).IsExpanded = true;
                                ppgModuleStatusString = "Connected (Transfering)";
                            }));
                        }
                        break;
                }
                //OnPropertyChanged("PPGModuleStatus");
            }
        }
        #endregion

        #endregion

        #region SETTING_DATA
        public SettingProgram SettingProgramData { get; set;}
        public SettingWindow SettingWindowData { get; set; }
        private SettingMIC _settingMICData;
        public SettingMIC SettingMICData {
            get {
                return _settingMICData;
            }
            set {
                _settingMICData = value;
                micNoChannels = _settingMICData.NumberOfChannels;
            }
        }
        private SettingFBGA _settingFBGAData;
        public SettingFBGA SettingFBGAData
        {
            get
            {
                return _settingFBGAData;
            }
            set
            {
                _settingFBGAData = value;
                fbgaNoChannels = _settingFBGAData.NumberOfChannels;
            }
        }
        private SettingECG _settingECGData;
        public SettingECG SettingECGData {
            get {
                return _settingECGData;
            }
            set {
                _settingECGData = value;
                ecgNoChannels = _settingECGData.NumberOfChannels;
            }
        }
        private SettingACC _settingACCData;
        public SettingACC SettingACCData {
            get {
                return _settingACCData;
            }
            set {
                _settingACCData = value;
                accNoChannels = _settingACCData.NumberOfChannels/3;
            }
        }
        private SettingPPG _settingPPGData;
        public SettingPPG SettingPPGData {
            get {
                return _settingPPGData;
            }
            set {
                _settingPPGData = value;
                ppgNoChannels = _settingPPGData.NumberOfChannels;
            }
        }
        #endregion

        #region PATIENT_DATA

        public List<Patient> Patients { get; set; }
        public Patient SelectedPatient { get; set; }
        public Patient ChoosenPatient { get; set; }

        #endregion

        #region WINDOW_CLOSING
        public event EventHandler WindowClosingEventHandler;
        public ICommand WindowClosing { get; private set; }
        private void WindowClosingExecute(object obj) {
            SettingService.StoreSetting(SettingProgramData, SettingWindowData, SettingFBGAData, SettingMICData, SettingECGData, SettingACCData, SettingPPGData);
            PatientService.StorePatients(Patients, SelectedPatient);
            WindowClosingEventHandler(null, null);
            OpenGLDispatcher.dispose();
            Log log = new Log();
            log.LogMessageToFile("Program closed!!!");
        }
        #endregion



    }
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public DelegateCommand(Action<object> execute,
                       Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
    
}
