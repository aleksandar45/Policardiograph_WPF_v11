using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Globalization;
using System.Windows.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Policardiograph_App.DeviceModel.Modules;
using Policardiograph_App.ViewModel;
using Policardiograph_App.ViewModel.OpenGLRender;
using Policardiograph_App.DeviceModel.Services;
using Policardiograph_App.DeviceModel.RingBuffers;
using Policardiograph_App.Exceptions;


namespace Policardiograph_App.DeviceModel
{

    public class Device
    {   
        TCPService tcpService;                   
        MainWindowViewModel mainWindowVeiwModel;        
        DispatcherTimer parseTimer; 
        DispatcherTimer playingTimer;
        DispatcherTimer fbgaTimer;
        DispatcherTimer syncRecordingTimer;
        DispatcherTimer tcpConnectionTimer;
        DispatcherTimer usbConnectionTimer;
        DispatcherTimer batteryMeasureTimer;
        TCPDeviceState micState;        
        TCPDeviceState ecgState;
        TCPDeviceState acc_ppgState;
        DateTime batteryDischgStartTime;
        DateTime batteryPrevCalcTime;

        string TAG = "DeviceModel/Device/";
        string path;
        string savePath;
        string saveFileName;
        string directoryName;

        const int MIC_BUFFER_LENGTH = 4000;
        const int ECG_BUFFER_LENGTH = 2000; 
        double fist_sector_start = 0, second_sector_start = MIC_BUFFER_LENGTH / 4, third_sector_start = 2 * MIC_BUFFER_LENGTH / 4, fourth_sector_start = 3 * MIC_BUFFER_LENGTH / 4;
        uint current_block_sector_index;  //0-3
        uint mic_no_data, mic_block_num, mic_previous_block_number = 0, mic_seq_num;
        uint ecg_no_data, ecg_block_num, ecg_previous_block_number, ecg_seq_num;
        uint acc_ppg_no_data, acc_ppg_block_num, acc_ppg_previous_block_number, acc_ppg_seq_num;

        bool micStateUpdated = false;
        bool ecgStateUpdated = false;
        bool acc_ppgStateUpdated = false;
        bool fbgaStateUpdated = false;
        bool playing = false;
        bool syncRecording = false;

        int syncRecordingStatus = 0;
        uint batteryValueUint = 0;
        const float BATTERY_VREG_VOLTAGE = 3.443f;
        const float BATTERY_DIVISION_COEFF = 4.04f;
        /*float[] batteryDischCharacteristic = new float[30] {
            6.20f,6.19f,6.18f,6.17f,6.17f,6.16f,    //1h
            6.15f,6.14f,6.13f,6.12f,6.12f,6.11f,    //2h
            6.10f,6.04f,5.98f,5.92f,5.86f,5.82f,    //3h 
            5.75f,5.66f,5.57f,5.48f,5.39f,5.30f,    //4h
            5.20f,5.10f,5.00f,4.90f,4.80f,4.70f     //5h
        };*/
        float[] batteryDischCharacteristic = new float[30] {
            6.10f,6.08f,6.06f,6.04f,6.03f,6.02f,    //1h
            6.01f,5.99f,5.98f,5.96f,5.95f,5.94f,    //2h
            5.92f,5.89f,5.85f,5.81f,5.77f,5.75f,    //3h 
            5.72f,5.66f,5.57f,5.48f,5.39f,5.30f,    //4h
            5.20f,5.10f,5.00f,4.90f,4.80f,4.70f     //5h
        };
        float batteryTotalDischTime = 0;
        float batteryValue = 0.0f;
        float [] batteryValuesBuff = new float[10];
        int batteryValuesBuffIndex = 0;
        bool batteryDischgStarted = false;
        bool batteryBuffFilled = false;

        public RingBufferByte micRingBuffer { get; set; }
        public RingBufferByte ecgRingBuffer { get; set; }
        public RingBufferByte acc_ppgRingBuffer { get; set; }
        public RingBufferDouble fbgaRingBuffer { get; set; }
        private MICModule _micModule;
        public MICModule micModule {
            get {
                return _micModule;
            }
            set
            {
                if(_micModule != null)
                    _micModule.dispose();
                _micModule = value;
                mainWindowVeiwModel.MicModuleStatus = TCPModuleStatusEnumType.CONNECTED;
                micState = TCPDeviceState.CONNECTED;
                
            }
        }
        private ECGModule _ecgModule;
        public ECGModule ecgModule {
            get{
                return _ecgModule;             
            }
            set {
                if(_ecgModule!=null)
                    _ecgModule.dispose();
                _ecgModule = value;
                mainWindowVeiwModel.ECGModuleStatus = TCPModuleStatusEnumType.CONNECTED;
                ecgState = TCPDeviceState.CONNECTED;                
            }
        }
        private ACC_PPGModule _acc_ppgModule;
        public ACC_PPGModule acc_ppgModule {
            
            get{
                return _acc_ppgModule;
            }
            set
            {
                if(_acc_ppgModule!=null)
                    _acc_ppgModule.dispose();
                _acc_ppgModule = value;
                mainWindowVeiwModel.ACCModuleStatus = TCPModuleStatusEnumType.CONNECTED;
                mainWindowVeiwModel.PPGModuleStatus = TCPModuleStatusEnumType.CONNECTED;
                acc_ppgState = TCPDeviceState.CONNECTED;
            }
            
        }
        private FBGAModule _fbgaModule;
        public FBGAModule fbgaModule {
            get {
                return _fbgaModule;
            }
            set {
                _fbgaModule = value;
            }
        }

#region debug_variables
        uint previous_value;
        bool first_attempt = true;

        int[] rptr_values = new int[1500];
        int[] wptr_values = new int[1500];
        int debug_index = 0;
        long[] stopwatch_values = new long[1500];
        Stopwatch stopwatch = new Stopwatch();
        

#endregion
        
        public Device(MainWindowViewModel mainWindowVeiwModel)
        {
            try
            {

                stopwatch.Start();


                int i;

                this.mainWindowVeiwModel = mainWindowVeiwModel;
                this.mainWindowVeiwModel.WindowClosingEventHandler += dispose;
                this.mainWindowVeiwModel.deviceRecordExecute += StartStopRecording;
                this.mainWindowVeiwModel.deviceSaveFile += SaveFiles;
                this.mainWindowVeiwModel.devicePlayExecute += StartStopPlaying;
                this.mainWindowVeiwModel.deviceSettingUpdate += UpdateSettings;
                this.mainWindowVeiwModel.deviceMWLSTurnOnExecute += TurnOnOffMWLS;

                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (!path.EndsWith("\\")) path += "\\";
                if (!Directory.Exists(path + "PolicardiographApp"))
                {
                    Directory.CreateDirectory(path + "PolicardiographApp");
                }
                path += "PolicardiographApp\\";


                savePath = mainWindowVeiwModel.SettingProgramData.SavePath;
                if (!savePath.EndsWith("\\")) savePath += "\\";
                directoryName = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
                if (!Directory.Exists(savePath + directoryName))
                {
                    Directory.CreateDirectory(savePath + directoryName);
                }
                savePath = savePath + directoryName + "\\";

                for (i = 0; i < 30; i++)
                {
                    if (batteryDischCharacteristic[i] < mainWindowVeiwModel.SettingProgramData.MinBatteryVoltage)
                        break;
                }
                batteryTotalDischTime = (i + 1) * 10;


                micRingBuffer = new RingBufferByte(8192);
                ecgRingBuffer = new RingBufferByte(8192);
                acc_ppgRingBuffer = new RingBufferByte(65536);
                fbgaRingBuffer = new RingBufferDouble(8192);

                fbgaModule = new FBGAModule(fbgaRingBuffer);

                playingTimer = new DispatcherTimer();
                playingTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                playingTimer.Tick += playingTimer_Tick;
                playingTimer.Start();

                syncRecordingTimer = new DispatcherTimer();
                syncRecordingTimer.Interval = new TimeSpan(0, 0, 4);
                syncRecordingTimer.Tick += syncRecordingTimer_Tick;

                tcpConnectionTimer = new DispatcherTimer();
                tcpConnectionTimer.Interval = new TimeSpan(0, 0, 1);
                tcpConnectionTimer.Tick += tcpConnectionTimer_Tick;
                tcpConnectionTimer.Start();

                fbgaTimer = new DispatcherTimer();
                fbgaTimer.Interval = new TimeSpan(0, 0, 4);
                fbgaTimer.Tick += fbgaTimer_Tick;

                usbConnectionTimer = new DispatcherTimer();
                usbConnectionTimer.Interval = new TimeSpan(0, 0, 1);
                usbConnectionTimer.Tick += usbConnectionTimer_Tick;
                usbConnectionTimer.Start();

                batteryMeasureTimer = new DispatcherTimer();
                batteryMeasureTimer.Interval = new TimeSpan(0, 0, 5);
                batteryMeasureTimer.Tick += batteryMeasureTimer_Tick;
                batteryMeasureTimer.Start();

                batteryDischgStartTime = DateTime.Now;


                tcpService = new TCPService(this);
                tcpService.Start();


                parseTimer = new DispatcherTimer();
                parseTimer.Tick += new EventHandler(ParseRingBuffers);
                parseTimer.Interval = new TimeSpan(0, 0, 0, 0, 20);  //20 miliseconds timer period
                parseTimer.Start();
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogMessageToFile(TAG + "Device:" + ex.Message);
            }
            finally { }
            
        }
        public bool TurnOnOffMWLS(string profile, bool vmMWLSTurnedOn) {
            try {
                if(vmMWLSTurnedOn){
                    fbgaModule.TurnOnOffSLED(mainWindowVeiwModel.SettingFBGAData, false);
                }
                else{
                    fbgaModule.Initialize(mainWindowVeiwModel.SettingFBGAData);
                    fbgaModule.TurnOnOffSLED(mainWindowVeiwModel.SettingFBGAData, true);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogMessageToFile("Turn On/Off MWLS attempt failed:");
                log.LogMessageToFile(TAG + "TurnOnOffMWLS:" + ex.Message);
                Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Error, ex.Message);
                Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);
                return false;
            }
        }
        public bool StartStopPlaying(string profile, bool vmPlaying) {
            try {

                if (vmPlaying)
                {
                    if ((String.Compare(profile, "FBGA") == 0) || (String.Compare(profile, "ALL DEVICES") == 0))
                    {
                        fbgaModule.stopAcquisition();
                        Thread.Sleep(500);
                        //fbgaTimer.Start();
                    }
                    if ((String.Compare(profile, "MIC") == 0) || (String.Compare(profile, "ALL DEVICES") == 0)) {
                        if (micModule != null) micModule.stopPlaying();
                    }
                    if ((String.Compare(profile, "ECG") == 0) || (String.Compare(profile, "ALL DEVICES") == 0)) {
                        if (ecgModule != null) ecgModule.stopPlaying();
                    }
                     if ((String.Compare(profile, "ACC") == 0)|| (String.Compare(profile, "PPG") == 0) || (String.Compare(profile, "ALL DEVICES") == 0)) {
                        if (acc_ppgModule != null) acc_ppgModule.stopPlaying();
                    }
                                                       
                    micRingBuffer.Reset();
                    fbgaRingBuffer.Reset();
                    ecgRingBuffer.Reset();                    
                    acc_ppgRingBuffer.Reset();                    

                }
                else
                {
                    if (String.Compare(profile, "ALL DEVICES") == 0) {
                        //if (ecgState != TCPDeviceState.IDLE) throw new MException("It is not possible to start sync recording due to some modules are not connected");
                        //if (acc_ppgState != TCPDeviceState.IDLE) throw new MException("It is not possible to start sync recording due to some modules are not connected");
                        if (micState != TCPDeviceState.IDLE) throw new MException("It is not possible to start sync recording due MIC/Master module is not connected");
                        else
                        {
                            if (fbgaModule.fbgaMwlsStatus == USBDeviceStateEnum.INITIALIZED)
                            {
                                // fbgaModule.TryInitialize(mainWindowVeiwModel.SettingFBGAData);
                                // fbgaTimer.Start();
                                fbgaModule.startAcquisition(true);
                                Thread.Sleep(500);
                            }
                            if (micState == TCPDeviceState.IDLE) micModule.startSyncPlaying();
                            if (ecgState == TCPDeviceState.IDLE) ecgModule.startSyncPlaying();
                            if (acc_ppgState == TCPDeviceState.IDLE) acc_ppgModule.startSyncPlaying();
                           
                        }
                    }
                    if (String.Compare(profile, "MIC") == 0) {
                        if ((micModule != null) && (mainWindowVeiwModel.MicModuleStatus == TCPModuleStatusEnumType.CONNECTED_IDLE)) micModule.startPlaying();
                        else throw new MException("It is not possible to start playing due to MIC module is not connected");
                    }
                    if (String.Compare(profile, "ECG") == 0)
                    {
                        if ((ecgModule != null) && (mainWindowVeiwModel.ECGModuleStatus == TCPModuleStatusEnumType.CONNECTED_IDLE)) ecgModule.startPlaying();
                        else throw new MException("It is not possible to start playing due to ECG module is not connected");
                    }
                    if ((String.Compare(profile, "ACC") == 0)|| (String.Compare(profile, "PPG") == 0))
                    {
                        if ((acc_ppgModule != null) && (mainWindowVeiwModel.ACCModuleStatus == TCPModuleStatusEnumType.CONNECTED_IDLE) && (mainWindowVeiwModel.PPGModuleStatus == TCPModuleStatusEnumType.CONNECTED_IDLE)) acc_ppgModule.startPlaying();
                        else throw new MException("It is not possible to start playing due to ACC/PPG module is not connected");
                    }
                    if ((String.Compare(profile,"FBGA") == 0)){                        
                        fbgaModule.startAcquisition(false);                        
                    }
                   
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogMessageToFile(TAG + "StartStopPlaying:" + ex.Message);
                Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Error, ex.Message);
                Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);
                return false;
            }
        }
        public int StartStopRecording(string profile, bool recording){
            
            int return_value = 0;
            try
            {
                
                if (recording)
                {
                    if (String.Compare(profile, "ALL DEVICES") == 0) {
                        StartStopPlaying("ALL DEVICES", true);
                        Thread.Sleep(500);
                    }
                    if (micModule != null)
                    {                        
                        micModule.stopRecording();
                        StartStopPlaying("MIC", true);
                    }
                    if (ecgModule != null)
                    {
                        ecgModule.stopRecording();
                        StartStopPlaying("ECG", true);
                    }
                    if (acc_ppgModule != null)
                    {
                        acc_ppgModule.stopRecording();
                        StartStopPlaying("ACC_PPG", true);
                    }
                    if (((String.Compare(profile, "FBGA") == 0)) || ((String.Compare(profile, "ALL DEVICES") == 0)))
                    {                      
                        fbgaModule.stopRecording();
                        StartStopPlaying("FBGA", true);
                    }
                    
                }
                else
                {
                    saveFileName = string.Format("{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now);
                    if (String.Compare(profile, "ALL DEVICES") == 0)
                    {
                       // if (ecgState != TCPDeviceState.TRANSFERING) throw new MException("It is not possible to start sync recording due to some modules are not connected");
                       // if (acc_ppgState != TCPDeviceState.TRANSFERING) throw new MException("It is not possible to start sync recording due to some modules are not connected");
                        if (micState != TCPDeviceState.TRANSFERING) throw new MException("It is not possible to start sync recording due to some modules are not connected");
                        else 
                        {
                            fbgaModule.stopAcquisition();
                            
                            Thread.Sleep(500);
                            syncRecording = false;
                            syncRecordingStatus = 0;
                            micModule.startSyncRecording();
                            syncRecordingTimer.Start();
                        }
                    }
                    if (String.Compare(profile,"MIC")==0){
                        syncRecordingStatus = 0x7F;
                        if (micModule != null) micModule.startRecording();
                        else throw new MException("It is not possible to start recording due to MIC module is not connected");
                        syncRecordingStatus = 0x7E;
                    }
                    if (String.Compare(profile,"ECG")==0){
                        syncRecordingStatus = 0x7F;
                        if (ecgModule != null) ecgModule.startRecording();
                        else throw new MException("It is not possible to start recording due to ECG module is not connected");
                        syncRecordingStatus = 0x7B;
                    }
                    if ((String.Compare(profile, "ACC") == 0) || (String.Compare(profile, "PPG") == 0))
                    {
                        syncRecordingStatus = 0x7F;
                        if (acc_ppgModule != null) acc_ppgModule.startRecording();
                        else throw new MException("It is not possible to start recording due to ACC/PPG module is not connected");
                        syncRecordingStatus = 0x77;
                    }
                    if ((String.Compare(profile,"FBGA") == 0)){
                        syncRecordingStatus = 0x7F;
                        fbgaModule.startRecording(savePath, saveFileName);
                        syncRecordingStatus = 0x7D;
                    }
                }
                return return_value = (0x80 | syncRecordingStatus);          //operation comleted properly
               
                
            }
            catch (Exception ex) {
                Log log = new Log();
                log.LogMessageToFile(TAG+ "StartStopRecording:" + ex.Message);
                Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Error, ex.Message);
                Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);
                return 0x00;                        //operation not completed properly
            }
           
        }
        public bool UpdateSettings(string profile) {
            try
            {
                if ((micModule != null) && (micState == TCPDeviceState.IDLE))  //for test only
                {
                    micModule.sendSetting(mainWindowVeiwModel.SettingMICData);
                }
                if ((ecgModule != null) && (ecgState == TCPDeviceState.IDLE))
                {
                    ecgModule.sendSetting(mainWindowVeiwModel.SettingECGData);
                }
                if ((acc_ppgModule != null) && (acc_ppgState == TCPDeviceState.IDLE))
                {
                    acc_ppgModule.sendSetting(mainWindowVeiwModel.SettingACCData, mainWindowVeiwModel.SettingPPGData);
                }
                if (fbgaModule.fbgaMwlsStatus == USBDeviceStateEnum.INITIALIZED)
                {                    
                    fbgaModule.Initialize(mainWindowVeiwModel.SettingFBGAData);
                }
                return true;
            }
            catch (Exception ex) {
                Log log = new Log();
                log.LogMessageToFile(TAG + "UpdateSettings:" + ex.Message);
                Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Error, ex.Message);
                Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);
                return false;
            }
        }
        public void playingTimer_Tick(object obj, EventArgs e) {
            if (!playing)
            {
                mainWindowVeiwModel.Playing = false;
            }
            else {
                mainWindowVeiwModel.Playing = true;
            }
            playing = false;
        }
        public void syncRecordingTimer_Tick(object obj, EventArgs e) {
            try
            {
                if (syncRecording)
                {
                    syncRecordingTimer.Stop();
                    mainWindowVeiwModel.recodingTimerStart();
                    //micRingBuffer.Reset();
                   // ecgRingBuffer.Reset();
                    //acc_ppgRingBuffer.Reset();
                    //fbgaRingBuffer.Reset();

                    //fbgaModule.Reset(mainWindowVeiwModel.SettingFBGAData);
                   /* if ((micState == TCPDeviceState.TRANSFERING) || (ecgState == TCPDeviceState.TRANSFERING) || (acc_ppgState == TCPDeviceState.TRANSFERING))
                    {
                        if (micModule != null)
                            micModule.stopRecording();
                        if (ecgModule != null)
                            ecgModule.stopRecording();
                        if (acc_ppgModule != null)
                            acc_ppgModule.stopRecording();
                    }*/
                }
                else {
                    if (micModule != null)
                        micModule.stopRecording();
                    if (ecgModule != null)
                        ecgModule.stopRecording();
                    if (acc_ppgModule != null)
                        acc_ppgModule.stopRecording();
                }
            }
            catch (Exception ex)
            {
                syncRecordingTimer.Stop();
                Log log = new Log();
                log.LogMessageToFile(TAG + "syncRecordingTimer_Tick:" + ex.Message);
                Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Error, ex.Message);
                Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);                
            }
        }
        public void tcpConnectionTimer_Tick(object obj, EventArgs e) {            
            
            if (!micStateUpdated)
            {
                micState = TCPDeviceState.DISCONNECTED;

                if (batteryDischgStarted == true) {
                    mainWindowVeiwModel.BatteryPercentage = 0;
                    mainWindowVeiwModel.BatteryValue = 0;
                }
                batteryDischgStarted = false;                
                if (mainWindowVeiwModel.MicModuleStatus != TCPModuleStatusEnumType.DISCONNECTED)
                {
                    mainWindowVeiwModel.MicModuleStatus = TCPModuleStatusEnumType.DISCONNECTED;
                }
            }
            else {
                if (batteryDischgStarted == false) {
                    batteryDischgStartTime = DateTime.Now;
                    batteryPrevCalcTime = DateTime.Now;
                }
                batteryDischgStarted = true;
                
                if (micState == TCPDeviceState.IDLE)
                {
                    if (mainWindowVeiwModel.MicModuleStatus != TCPModuleStatusEnumType.CONNECTED_IDLE)
                    {
                        mainWindowVeiwModel.MicModuleStatus = TCPModuleStatusEnumType.CONNECTED_IDLE;
                    }
                    micState = TCPDeviceState.DISCONNECTED;
                }
                if (micState == TCPDeviceState.TRANSFERING)
                {
                    if (mainWindowVeiwModel.MicModuleStatus != TCPModuleStatusEnumType.CONNECTED_TRANSFERING)
                    {
                        mainWindowVeiwModel.MicModuleStatus = TCPModuleStatusEnumType.CONNECTED_TRANSFERING;
                    }
                    micState = TCPDeviceState.DISCONNECTED;
                }
            }                 
                               
            
           if(!ecgStateUpdated){
                if (mainWindowVeiwModel.ECGModuleStatus != TCPModuleStatusEnumType.DISCONNECTED)
                {
                    mainWindowVeiwModel.ECGModuleStatus = TCPModuleStatusEnumType.DISCONNECTED;
                }
                ecgState = TCPDeviceState.DISCONNECTED;                    
           }
           else{
                if (ecgState == TCPDeviceState.IDLE)
                {
                    if (mainWindowVeiwModel.ECGModuleStatus != TCPModuleStatusEnumType.CONNECTED_IDLE)
                    {
                        mainWindowVeiwModel.ECGModuleStatus = TCPModuleStatusEnumType.CONNECTED_IDLE;
                    }
                    ecgState = TCPDeviceState.DISCONNECTED;
                }
                if (ecgState == TCPDeviceState.TRANSFERING)
                {
                    if (mainWindowVeiwModel.ECGModuleStatus != TCPModuleStatusEnumType.CONNECTED_TRANSFERING)
                    {
                        mainWindowVeiwModel.ECGModuleStatus = TCPModuleStatusEnumType.CONNECTED_TRANSFERING;
                    }
                    ecgState = TCPDeviceState.DISCONNECTED;
                }

            }

           if (!acc_ppgStateUpdated)
           {
               if (mainWindowVeiwModel.ACCModuleStatus != TCPModuleStatusEnumType.DISCONNECTED)
               {
                   mainWindowVeiwModel.ACCModuleStatus = TCPModuleStatusEnumType.DISCONNECTED;
               }
               if (mainWindowVeiwModel.PPGModuleStatus != TCPModuleStatusEnumType.DISCONNECTED)
               {
                   mainWindowVeiwModel.PPGModuleStatus = TCPModuleStatusEnumType.DISCONNECTED;
               }
               acc_ppgState = TCPDeviceState.DISCONNECTED;
           }
           else
           {
               if (acc_ppgState == TCPDeviceState.IDLE)
               {
                   if (mainWindowVeiwModel.ACCModuleStatus != TCPModuleStatusEnumType.CONNECTED_IDLE)
                   {
                       mainWindowVeiwModel.ACCModuleStatus = TCPModuleStatusEnumType.CONNECTED_IDLE;
                   }
                   if (mainWindowVeiwModel.PPGModuleStatus != TCPModuleStatusEnumType.CONNECTED_IDLE)
                   {
                       mainWindowVeiwModel.PPGModuleStatus = TCPModuleStatusEnumType.CONNECTED_IDLE;
                   }
                   acc_ppgState = TCPDeviceState.DISCONNECTED;
               }
               if (acc_ppgState == TCPDeviceState.TRANSFERING)
               {
                   if (mainWindowVeiwModel.ACCModuleStatus != TCPModuleStatusEnumType.CONNECTED_TRANSFERING)
                   {
                       mainWindowVeiwModel.ACCModuleStatus = TCPModuleStatusEnumType.CONNECTED_TRANSFERING;
                   }
                   if (mainWindowVeiwModel.PPGModuleStatus != TCPModuleStatusEnumType.CONNECTED_TRANSFERING)
                   {
                       mainWindowVeiwModel.PPGModuleStatus = TCPModuleStatusEnumType.CONNECTED_TRANSFERING;
                   }
                   acc_ppgState = TCPDeviceState.DISCONNECTED;
               }

           }
            
            micStateUpdated = false;
            ecgStateUpdated = false;
            acc_ppgStateUpdated = false;

        }
        public void fbgaTimer_Tick(object obj, EventArgs e) {
            try
            {
                fbgaTimer.Stop();
                //fbgaModule.stopAcquisition();
                fbgaModule.startAcquisition(true);                                
            }
            catch (Exception ex)
            {
                fbgaTimer.Stop();
                Log log = new Log();
                log.LogMessageToFile(TAG + "fbgaTimer_Tick:" + ex.Message);
                Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Error, ex.Message);
                Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);
            }
        }
        public void usbConnectionTimer_Tick(object obj, EventArgs e) {
            try
            {
                if (fbgaStateUpdated)
                {
                    if(mainWindowVeiwModel.FBGAModuleStatus != USBModuleStatusEnumType.TRANSFERING)
                        mainWindowVeiwModel.FBGAModuleStatus = USBModuleStatusEnumType.TRANSFERING;
                    fbgaModule.fbgaMwlsStatus = USBDeviceStateEnum.TRANSFERING;
                }
                else
                {
                    if (fbgaModule.fbgaMwlsStatus == USBDeviceStateEnum.TRANSFERING)
                    {
                        mainWindowVeiwModel.FBGAModuleStatus = USBModuleStatusEnumType.INITIALIZED;
                        fbgaModule.fbgaMwlsStatus = USBDeviceStateEnum.INITIALIZED;
                    }
                }
                fbgaStateUpdated = false;

                if (!mainWindowVeiwModel.Playing)
                {
                    if (fbgaModule.fbgaMwlsStatus == USBDeviceStateEnum.INITIALIZED)
                    {
                        if (!fbgaModule.CheckDeviceConnectionStatus())
                         {
                             fbgaModule.TryInitialize(mainWindowVeiwModel.SettingFBGAData);
                             switch (fbgaModule.fbgaMwlsStatus)
                             {
                                 case USBDeviceStateEnum.NOT_INITIALIZED:
                                     if (mainWindowVeiwModel.FBGAModuleStatus != USBModuleStatusEnumType.NOT_INITIALIZED)
                                         mainWindowVeiwModel.FBGAModuleStatus = USBModuleStatusEnumType.NOT_INITIALIZED;                                    
                                     //fbgaModule.dispose();
                                     break;
                                 case USBDeviceStateEnum.INITIALIZED:
                                     if (mainWindowVeiwModel.FBGAModuleStatus != USBModuleStatusEnumType.INITIALIZED)
                                         mainWindowVeiwModel.FBGAModuleStatus = USBModuleStatusEnumType.INITIALIZED;
                                     break;                                
                             }
                        }
                    }
                    else if ( fbgaModule.fbgaMwlsStatus == USBDeviceStateEnum.NOT_INITIALIZED)
                    {
                        fbgaModule.TryInitialize(mainWindowVeiwModel.SettingFBGAData);
                        switch (fbgaModule.fbgaMwlsStatus)
                        {
                            case USBDeviceStateEnum.NOT_INITIALIZED:
                                if (mainWindowVeiwModel.FBGAModuleStatus != USBModuleStatusEnumType.NOT_INITIALIZED)
                                    mainWindowVeiwModel.FBGAModuleStatus = USBModuleStatusEnumType.NOT_INITIALIZED;
                                //fbgaModule.dispose();
                                break;                  
                            case USBDeviceStateEnum.INITIALIZED:
                                if (mainWindowVeiwModel.FBGAModuleStatus != USBModuleStatusEnumType.INITIALIZED)
                                    mainWindowVeiwModel.FBGAModuleStatus = USBModuleStatusEnumType.INITIALIZED;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex) {
                usbConnectionTimer.Stop();
                Log log = new Log();
                log.LogMessageToFile(TAG + "usbConnectionTimer_Tick:" + ex.Message);
                Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Error, ex.Message);
                Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm); 
            }
            
        }
        public void batteryMeasureTimer_Tick(object obj, EventArgs e){
            
            if ((batteryDischgStarted) && (batteryValueUint!=0))
            {
                //calculate battery value
                
                batteryValuesBuff[batteryValuesBuffIndex++] = (float)batteryValueUint * BATTERY_VREG_VOLTAGE * BATTERY_DIVISION_COEFF / 4096;
                if (batteryValuesBuffIndex >= 10)
                {
                    batteryValuesBuffIndex = 0;
                    batteryBuffFilled = true;
                }

                batteryValue = 0;
                for (int i = 0; i < 10; i++) {
                    batteryValue += batteryValuesBuff[i];
                }
                if (batteryBuffFilled)
                {
                    batteryValue = batteryValue / 10;
                    //calculate battery percentage
                    if (batteryValue < batteryDischCharacteristic[0])
                    {
                        int i;
                        double tempDouble;
                        for (i = 1; i < 30; i++)
                        {
                            if ((batteryValue < batteryDischCharacteristic[i - 1]) && (batteryValue > batteryDischCharacteristic[i]))
                                break;
                        }
                        if (i != 30)
                        {
                            tempDouble = i * 10 - 10 * (batteryValue - batteryDischCharacteristic[i]) / (batteryDischCharacteristic[i - 1] - batteryDischCharacteristic[i]);
                            mainWindowVeiwModel.BatteryPercentage = (Int32)Math.Round(100 * (batteryTotalDischTime - tempDouble) / batteryTotalDischTime);
                        }
                        else{
                            mainWindowVeiwModel.BatteryPercentage = 0;
                        }
                    }
                    else
                    {
                        mainWindowVeiwModel.BatteryPercentage = 100;
                    }
                    
                }
                else {

                    //calculate battery percentage
                    batteryValue = (float)batteryValueUint * BATTERY_VREG_VOLTAGE * BATTERY_DIVISION_COEFF / 4096;
                    if (batteryValue < batteryDischCharacteristic[0])
                    {
                        int i;
                        double tempDouble;
                        for (i = 1; i < 30; i++)
                        {
                            if ((batteryValue < batteryDischCharacteristic[i - 1]) && (batteryValue > batteryDischCharacteristic[i]))
                                break;
                        }
                        if (i != 30)
                        {
                            tempDouble = i * 10 - 10 * (batteryValue - batteryDischCharacteristic[i]) / (batteryDischCharacteristic[i - 1] - batteryDischCharacteristic[i]);
                            mainWindowVeiwModel.BatteryPercentage = (Int32)Math.Round(100 * (batteryTotalDischTime - tempDouble) / batteryTotalDischTime);
                        }
                        else
                        {
                            mainWindowVeiwModel.BatteryPercentage = 0;
                        }
                    }
                    else
                    {
                        mainWindowVeiwModel.BatteryPercentage = 100;
                    }
                }
                
                mainWindowVeiwModel.BatteryValue = batteryValue;
                if (mainWindowVeiwModel.DebugEnabled)
                {
                    TimeSpan diff = DateTime.Now - batteryPrevCalcTime;
                    batteryPrevCalcTime = DateTime.Now;
                    mainWindowVeiwModel.SettingProgramData.BatteryLastMeasuredTime += (int)diff.TotalSeconds;

                    if (mainWindowVeiwModel.SettingProgramData.BatteryLastMeasuredTime >= 60)
                    {
                        

                        //store data to battery log file
                        StreamWriter batteryLogWriter = File.AppendText(path + "Battery_log.txt");
                        try
                        {
                            string line = mainWindowVeiwModel.SettingProgramData.BatteryLastMeasuredTime.ToString() + ";" + batteryValue.ToString(CultureInfo.InvariantCulture);
                            batteryLogWriter.WriteLine(line);
                        }
                        catch (Exception ex)
                        {
                            Log log = new Log();
                            log.LogMessageToFile(TAG + "batteryMeasureTimer_Tick/" + "Battery_log_exception:" + ex.Message);
                        }
                        finally
                        {
                            batteryLogWriter.Close();
                        }

                        mainWindowVeiwModel.SettingProgramData.BatteryLastMeasuredTime = 0;
                    }

                }
                
                
            }
        }
        private int SaveFiles()
        {
            FileStream binaryReader;
           

            byte[] byte_array = new byte[500];         
            string data_string = "|HEAD|DATA_PACKETxxx";
            string line;

            int seq_num = 0, previous_seq_num = 0; 
            int current_block_num = 0, previous_block_num = 0;
            int num_of_data;

            int mic_data_counter = 0, mic_i = 0;                        
            int[] mic_block_lengths = new int[70];
            int[] mic_block_numbers = new int[70];
            int mic_error = 0;
            bool mic_data_read_started = false;

            int ecg_data_counter = 0, ecg_i = 0;
            int ecg_data_index = 0;
            int[] ecg_block_lengths = new int[70];
            int[] ecg_block_numbers = new int[70];
            int[] ecg_data = new int[10000];
            int ecg_error = 0;
            double syncEcgTime = 0;
            bool ecg_data_read_started = false;

            int acc_ppg_data_counter = 0, acc_ppg_i = 0;
            int acc_ppg_data_index = 0;
            int[] acc_ppg_block_lengths = new int[70];
            int[] acc_ppg_block_numbers = new int[70];
            int[] acc_ppg_data = new int[10000];
            int acc_ppg_error = 0;
            double sync_acc_ppgTime = 0;
            bool acc_ppg_data_read_started = false;

            int fbga_line_stamp = 0, fbga_data_counter = 0;
            double[] fbga_data_array = new double[512];
                       

            Int16 int16_temp;
            Int32 int32_temp;

            try
            {

                if ((syncRecordingStatus & 0x01) != 0x01)
                {
                    binaryReader = File.OpenRead(path + "MIC.dat");

                    using (StreamWriter streamWriter = File.CreateText(savePath + saveFileName + ".poli"))
                    {
                        line = "";
                        for (int i = 0; i < 512; i++)
                        {
                            line = line + "=";
                        }
                        streamWriter.WriteLine(line);

                        line = "MIC";
                        streamWriter.WriteLine(line);
                        line = "Tm\tM1\tM2";
                        streamWriter.WriteLine(line);

                        line = "";
                        for (int i = 0; i < 512; i++)
                        {
                            line = line + "=";
                        }
                        streamWriter.WriteLine(line);
                        while (binaryReader.Position < binaryReader.Length)
                        {
                            binaryReader.Read(byte_array, 0, 71);
                            if (compareByteArrayAndString(byte_array, data_string, 20))
                            {
                                //==========================Check number of data=======================//
                                num_of_data = byte_array[20] * 256 + byte_array[21];
                                if (num_of_data != 40)
                                {
                                    mic_error = 1;
                                    break;
                                }

                                //==========================Check block numbers=======================//
                                current_block_num = byte_array[22] * 256 + byte_array[23];
                                if ((previous_block_num != current_block_num) && mic_data_read_started)
                                {
                                    if (((previous_block_num + 1) % 8) != current_block_num)
                                    {
                                        mic_error = 2;
                                        break;
                                    }
                                    mic_block_lengths[mic_i] = seq_num;
                                    mic_block_numbers[mic_i++] = previous_block_num;
                                }

                                //==========================Check sequence numbers=====================//
                                seq_num = byte_array[24] * 256 + byte_array[25];
                                if ((previous_block_num == current_block_num) && mic_data_read_started)
                                {
                                    if ((previous_seq_num + 1) != seq_num)
                                    {
                                        mic_error = 3;
                                        break;
                                    }
                                }

                                previous_block_num = current_block_num;
                                previous_seq_num = seq_num;

                                mic_data_read_started = true;

                                for (int i = 0; i < 20; i = i + 2)
                                {
                                    line = System.String.Format("{0}", (mic_data_counter * 10 + i / 2 + 1));
                                    int16_temp = (Int16)(byte_array[i + 26] * 256 + byte_array[i + 27]);
                                    line = line + "\t" + int16_temp.ToString();
                                    int16_temp = (Int16)(byte_array[i + 46] * 256 + byte_array[i + 47]);
                                    line = line + "\t" + int16_temp.ToString();
                                    streamWriter.WriteLine(line);
                                }
                                mic_data_counter++;
                            }
                        }
                        mic_block_lengths[mic_i] = seq_num;
                        mic_block_numbers[mic_i++] = current_block_num;
                    }
                    binaryReader.Close();
                    if (mic_error != 0)
                    {
                        File.Delete(savePath + saveFileName + ".poli");
                        using (StreamWriter streamWriter = File.CreateText(savePath + saveFileName + "-Bad" + ".poli"))
                        {

                        }
                    }
                }
                seq_num = 0;
                previous_seq_num = 0;
                current_block_num = 0;
                previous_block_num = 0;

                if ((syncRecordingStatus & 0x04) != 0x04)
                {
                    bool sync = false;
                    float ecgTime = 0.0f;
                    if ((syncRecordingStatus & 0x01) != 0x01) sync = true;
                    binaryReader = File.OpenRead(path + "ECG.dat");
                    using (StreamWriter streamWriter = File.AppendText(savePath + saveFileName + ".poli"))
                    {
                        line = "";
                        for (int i = 0; i < 512; i++)
                        {
                            line = line + "=";
                        }
                        streamWriter.WriteLine(line);

                        line = "ECG";
                        streamWriter.WriteLine(line);
                        line = "Tm\tE1\tE2\tE3\tE4\tE5\tE6\tE7\tE8";
                        streamWriter.WriteLine(line);

                        line = "";
                        for (int i = 0; i < 512; i++)
                        {
                            line = line + "=";
                        }
                        streamWriter.WriteLine(line);

                        while (binaryReader.Position < binaryReader.Length)
                        {
                            binaryReader.Read(byte_array, 0, 151);
                            if (compareByteArrayAndString(byte_array, data_string, 20))
                            {
                                //==========================Check number of data=======================//
                                num_of_data = byte_array[20] * 256 + byte_array[21];
                                if (num_of_data != 120)
                                {
                                    ecg_error = 1;
                                    break;
                                }

                                //==========================Check end of block=======================//
                                current_block_num = byte_array[22] * 256 + byte_array[23];
                                if ((previous_block_num != current_block_num) && ecg_data_read_started)
                                {
                                    if (((previous_block_num + 1) % 8) != current_block_num)
                                    {
                                        ecg_error = 2;
                                        break;
                                    }
                                    if (previous_block_num == mic_block_numbers[ecg_i])
                                    {
                                        double dt = (double)(mic_block_lengths[ecg_i] + 1) / (double)(seq_num + 1);

                                        for (int i = 0; i < ecg_data_index / 8; i++)
                                        {
                                            syncEcgTime = syncEcgTime + 2.0 * dt;
                                            line = System.String.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:0.00}", syncEcgTime);
                                            line = line + "\t\t\t" + ecg_data[i * 8].ToString();
                                            line = line + "\t\t\t" + ecg_data[i * 8 + 1].ToString();
                                            line = line + "\t\t\t" + ecg_data[i * 8 + 2].ToString();
                                            line = line + "\t\t\t" + ecg_data[i * 8 + 3].ToString();
                                            line = line + "\t\t\t" + ecg_data[i * 8 + 4].ToString();
                                            line = line + "\t\t\t" + ecg_data[i * 8 + 5].ToString();
                                            line = line + "\t\t\t" + ecg_data[i * 8 + 6].ToString();
                                            line = line + "\t\t\t" + ecg_data[i * 8 + 7].ToString();
                                            streamWriter.WriteLine(line);
                                        }
                                    }
                                    else
                                    {
                                        ecg_error = 4;
                                        break;
                                    }

                                    ecg_block_lengths[ecg_i] = seq_num;
                                    ecg_block_numbers[ecg_i++] = previous_block_num;
                                    ecg_data_index = 0;
                                }

                                //==========================Check sequence numbers=====================//
                                seq_num = byte_array[24] * 256 + byte_array[25];
                                if ((previous_block_num == current_block_num) && ecg_data_read_started)
                                {
                                    if ((previous_seq_num + 1) != seq_num)
                                    {
                                        ecg_error = 3;
                                        break;
                                    }
                                }

                                previous_block_num = current_block_num;
                                previous_seq_num = seq_num;

                                ecg_data_read_started = true;

                                for (int i = 0; i < 15; i = i + 3)
                                {
                                    line = System.String.Format("{0:0.00}", ecgTime);
                                    for (int k = 0; k < 8; k++)
                                    {
                                        int32_temp = (Int32)(byte_array[i + 26 + 15 * k] * 65536 + byte_array[i + 27 + 15 * k] * 256 + byte_array[i + 28 + 15 * k]);
                                        if ((int32_temp & 0x800000) == 0x800000)
                                            int32_temp = int32_temp - 16777216;

                                        if (sync)
                                        {
                                            ecg_data[ecg_data_index++] = int32_temp;
                                        }
                                        else
                                        {
                                            line = line + "\t\t\t" + int32_temp.ToString();

                                        }
                                    }
                                    if (!sync)
                                    {
                                        streamWriter.WriteLine(line);
                                        ecgTime = ecgTime + 2.0f;
                                    }
                                    //try

                                }
                            }

                        }
                        if (previous_block_num == mic_block_numbers[ecg_i])
                        {
                            double dt = (double)(mic_block_lengths[ecg_i] + 1) / (double)(seq_num + 1);
                            for (int i = 0; i < ecg_data_index / 8; i++)
                            {
                                syncEcgTime = syncEcgTime + 2.0 * dt;
                                line = System.String.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:0.00}", syncEcgTime);
                                line = line + "\t\t\t" + ecg_data[i * 8].ToString();
                                line = line + "\t\t\t" + ecg_data[i * 8 + 1].ToString();
                                line = line + "\t\t\t" + ecg_data[i * 8 + 2].ToString();
                                line = line + "\t\t\t" + ecg_data[i * 8 + 3].ToString();
                                line = line + "\t\t\t" + ecg_data[i * 8 + 4].ToString();
                                line = line + "\t\t\t" + ecg_data[i * 8 + 5].ToString();
                                line = line + "\t\t\t" + ecg_data[i * 8 + 6].ToString();
                                line = line + "\t\t\t" + ecg_data[i * 8 + 7].ToString();
                                streamWriter.WriteLine(line);
                            }
                        }
                        else
                        {
                            ecg_error = 4;
                        }
                        ecg_block_lengths[ecg_i] = seq_num;
                        ecg_block_numbers[ecg_i++] = current_block_num;

                    }
                    binaryReader.Close();
                    if (ecg_error != 0)
                    {
                        File.Delete(savePath + saveFileName + ".poli");
                        using (StreamWriter streamWriter = File.CreateText(savePath + saveFileName + "-Bad" + ".poli"))
                        {

                        }
                    }
                }

                seq_num = 0;
                previous_seq_num = 0;
                current_block_num = 0;
                previous_block_num = 0;

                if ((syncRecordingStatus & 0x08) != 0x08)
                {
                    bool sync = false;
                    float acc_ppgTime = 0.0f;
                    if ((syncRecordingStatus & 0x01) != 0x01) sync = true;
                    binaryReader = File.OpenRead(path + "ACC_PPG.dat");
                    using (StreamWriter streamWriter = File.AppendText(savePath + saveFileName + ".poli"))
                    {
                        line = "";
                        for (int i = 0; i < 512; i++)
                        {
                            line = line + "=";
                        }
                        streamWriter.WriteLine(line);

                        line = "ACC_PPG";
                        streamWriter.WriteLine(line);
                        line = "Tm\tA1z\tA2z\tP1g\tP1r\tP2g\tP2r";
                        streamWriter.WriteLine(line);

                        line = "";
                        for (int i = 0; i < 512; i++)
                        {
                            line = line + "=";
                        }
                        streamWriter.WriteLine(line);

                        while (binaryReader.Position < binaryReader.Length)
                        {
                            binaryReader.Read(byte_array, 0, 151);

                            if (compareByteArrayAndString(byte_array, data_string, 20))
                            {
                                //==========================Check number of data=======================//
                                num_of_data = byte_array[20] * 256 + byte_array[21];
                                if (num_of_data != 120)
                                {
                                    acc_ppg_error = 1;
                                    break;
                                }

                                //==========================Check end of block=======================//
                                current_block_num = byte_array[22] * 256 + byte_array[23];
                                if ((previous_block_num != current_block_num) && acc_ppg_data_read_started)
                                {
                                    if (((previous_block_num + 1) % 8) != current_block_num)
                                    {
                                        acc_ppg_error = 2;
                                        break;

                                    }
                                    if (previous_block_num == mic_block_numbers[acc_ppg_i])
                                    {
                                        double dt = (double)(mic_block_lengths[acc_ppg_i] + 1) / (double)(seq_num + 1);
                                        for (int i = 0; i < acc_ppg_data_index / 6; i++)
                                        {
                                            sync_acc_ppgTime = sync_acc_ppgTime + 2.0 * dt;
                                            line = System.String.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:0.00}", sync_acc_ppgTime);
                                            line = line + "\t\t\t" + acc_ppg_data[i * 6].ToString();
                                            line = line + "\t\t\t" + acc_ppg_data[i * 6 + 1].ToString();
                                            line = line + "\t\t\t" + acc_ppg_data[i * 6 + 2].ToString();
                                            line = line + "\t\t\t" + acc_ppg_data[i * 6 + 3].ToString();
                                            line = line + "\t\t\t" + acc_ppg_data[i * 6 + 4].ToString();
                                            line = line + "\t\t\t" + acc_ppg_data[i * 6 + 5].ToString();
                                            streamWriter.WriteLine(line);

                                        }
                                    }
                                    else
                                    {
                                        acc_ppg_error = 4;
                                        break;
                                    }

                                    acc_ppg_block_lengths[acc_ppg_i] = seq_num;
                                    acc_ppg_block_numbers[acc_ppg_i++] = previous_block_num;
                                    acc_ppg_data_index = 0;
                                }

                                //==========================Check sequence numbers=====================//
                                seq_num = byte_array[24] * 256 + byte_array[25];
                                if ((previous_block_num == current_block_num) && acc_ppg_data_read_started)
                                {
                                    if ((previous_seq_num + 1) != seq_num)
                                    {
                                        acc_ppg_error = 3;
                                        break;
                                    }
                                }

                                previous_block_num = current_block_num;
                                previous_seq_num = seq_num;

                                acc_ppg_data_read_started = true;

                                for (int i = 0; i < 10; i = i + 2)
                                {
                                    line = System.String.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:0.00}", acc_ppgTime);
                                    for (int k = 0; k < 15; k++)
                                    {
                                        int16_temp = (Int16)(byte_array[i + 26 + 10 * k] * 256 + byte_array[i + 27 + 10 * k]);
                                        if (sync)
                                        {
                                            if ((k == 2) || (k == 5))
                                            {
                                                acc_ppg_data[acc_ppg_data_index++] = int16_temp;
                                            }
                                            if (k == 9)
                                            {
                                                int16_temp = (Int16)(byte_array[116] * 256 + byte_array[117]);      //PPG1g
                                                acc_ppg_data[acc_ppg_data_index++] = int16_temp;
                                                int16_temp = (Int16)(byte_array[118] * 256 + byte_array[119]);      //PPG1r
                                                acc_ppg_data[acc_ppg_data_index++] = int16_temp;
                                                int16_temp = (Int16)(byte_array[120] * 256 + byte_array[121]);      //PPG2g
                                                acc_ppg_data[acc_ppg_data_index++] = int16_temp;
                                                int16_temp = (Int16)(byte_array[122] * 256 + byte_array[123]);      //PPG2r
                                                acc_ppg_data[acc_ppg_data_index++] =  int16_temp;
                                            }


                                        }
                                        else
                                        {
                                            if ((k == 2) || (k == 5))
                                            {
                                                line = line + "\t\t\t" + int16_temp.ToString();
                                            }
                                            if (k == 9)
                                            {
                                                int16_temp = (Int16)(byte_array[116] * 256 + byte_array[117]);      //PPG1g
                                                line = line + "\t\t\t" + int16_temp.ToString();
                                                int16_temp = (Int16)(byte_array[118] * 256 + byte_array[119]);      //PPG1r
                                                line = line + "\t\t\t" + int16_temp.ToString();
                                                int16_temp = (Int16)(byte_array[120] * 256 + byte_array[121]);      //PPG2g
                                                line = line + "\t\t\t" + int16_temp.ToString();
                                                int16_temp = (Int16)(byte_array[122] * 256 + byte_array[123]);      //PPG2r
                                                line = line + "\t\t\t" + int16_temp.ToString();
                                            }

                                        }
                                    }
                                    if (!sync)
                                    {
                                        streamWriter.WriteLine(line);
                                        acc_ppgTime = acc_ppgTime + 2.0f;
                                    }
                                    //try

                                }
                            }

                        }

                        if (previous_block_num == mic_block_numbers[acc_ppg_i])
                        {
                            double dt = (double)(mic_block_lengths[acc_ppg_i] + 1) / (double)(seq_num + 1);
                            for (int i = 0; i < acc_ppg_data_index / 6; i++)
                            {
                                sync_acc_ppgTime = sync_acc_ppgTime + 2.0 * dt;
                                line = System.String.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:0.00}", sync_acc_ppgTime);
                                line = line + "\t\t\t" + acc_ppg_data[i * 6].ToString();
                                line = line + "\t\t\t" + acc_ppg_data[i * 6 + 1].ToString();
                                line = line + "\t\t\t" + acc_ppg_data[i * 6 + 2].ToString();
                                line = line + "\t\t\t" + acc_ppg_data[i * 6 + 3].ToString();
                                line = line + "\t\t\t" + acc_ppg_data[i * 6 + 4].ToString();
                                line = line + "\t\t\t" + acc_ppg_data[i * 6 + 5].ToString();
                                streamWriter.WriteLine(line);

                            }
                        }
                        else
                        {
                            acc_ppg_error = 4;
                        }
                        acc_ppg_block_lengths[acc_ppg_i] = seq_num;
                        acc_ppg_block_numbers[acc_ppg_i++] = current_block_num;
                    }
                    binaryReader.Close();
                    if (acc_ppg_error != 0)
                    {
                        File.Delete(savePath + saveFileName + ".poli");
                        using (StreamWriter streamWriter = File.CreateText(savePath + saveFileName + "-Bad" + ".poli"))
                        {

                        }
                    }

                }
            }
            catch (Exception ex) {
                Log log = new Log();
                log.LogMessageToFile(TAG + "SaveFiles:" + ex.Message);
                return 1;               //return error
            }
            
            /*if ((syncRecordingStatus & 0x02) != 0x02)
            {
                binaryReader = File.OpenRead(path + "FBGA.dat");
                using (StreamWriter streamWriter = File.AppendText(savePath + saveFileName + ".poli"))
                {
                    line = "";
                    for (int i = 0; i < 512; i++)
                    {
                        line = line + "=";
                    }
                    streamWriter.WriteLine(line);

                    line = "FBGA";
                    streamWriter.WriteLine(line);
                    line = "Tfbga\tFBGA[1:512]";
                    streamWriter.WriteLine(line);

                    line = "";
                    for (int i = 0; i < 512; i++)
                    {
                        line = line + "=";
                    }
                    streamWriter.WriteLine(line);
                    while (binaryReader.Position < binaryReader.Length)
                    {
                        binaryReader.Read(byte_array, 0, 4);
                        fbga_line_stamp = BitConverter.ToInt32(byte_array, 0);
                        line = System.String.Format("{0}", (fbga_data_counter + 1) * 2);

                        for (int i = 0; i < 512; i++)
                        {
                            binaryReader.Read(byte_array, 0, 8);
                            fbga_data_array[i] = BitConverter.ToDouble(byte_array, 0);
                            line = line + "\t" + ((Int32)fbga_data_array[i]).ToString();
                        }
                        fbga_data_counter++;

                        streamWriter.WriteLine(line);
                    }
                }
                binaryReader.Close();
            }*/
            return 0;

        }
        public void ParseRingBuffers(object sender, EventArgs e) {

            OpenGLDispatcher glD = mainWindowVeiwModel.OpenGLDispatcher;

            byte[] tmpBuffer = new byte[500];
            byte[] dataBuffer = new byte[2000];
            double[] tempDouble = new double[512];
            uint ecg_no_data, ecg_block_num, ecg_previous_block_number, ecg_seq_num;
            int error_code = 0;
            Int16 int16_temp;
            UInt16 uint16_temp;
            Int32 int32_temp;

            try
            {

                int cnt = fbgaRingBuffer.ReadSpace();
                bool fbgaDataAvailable = false;
                if ((String.Compare(mainWindowVeiwModel.ComboboxSelectedItem, "FBGA") == 0) || (String.Compare(mainWindowVeiwModel.ComboboxSelectedItem, "ALL DEVICES") == 0))
                {

                    while (cnt > 512)
                    {
                        fbgaRingBuffer.Read(tempDouble, 512);
                        cnt = fbgaRingBuffer.ReadSpace();
                        fbgaDataAvailable = true;
                    }
                    if (fbgaDataAvailable)
                    {
                        playing = true;
                        fbgaModule.fbgaMwlsStatus = USBDeviceStateEnum.TRANSFERING;
                        fbgaStateUpdated = true;
                        for (int i = 0; i < 512; i++)
                        {
                            glD.fbgaChannels.ElementAt(0).intArray[i] = (Int32)tempDouble[i];
                        }
                    }

                }
                while (micRingBuffer.ReadSpace() >= 71)
                {
                    micRingBuffer.Read(tmpBuffer, 6);
                    if (compareByteArrayAndString(tmpBuffer, "|HEAD|", 6))
                    {

                        micRingBuffer.Read(tmpBuffer, 14);
                        if (compareByteArrayAndString(tmpBuffer, "DATA_PACKETxxx", 14))
                        {
                            playing = true;
                            micRingBuffer.Read(tmpBuffer, 6);
                            mic_no_data = ((uint)tmpBuffer[0]) * 256 + (uint)tmpBuffer[1];
                            mic_block_num = ((uint)tmpBuffer[2]) * 256 + (uint)tmpBuffer[3];
                            mic_seq_num = ((uint)tmpBuffer[4]) * 256 + (uint)tmpBuffer[5];
                            //sinchronization block
                            if (mic_block_num != mic_previous_block_number)
                            {
                                mic_previous_block_number = mic_block_num;
                                switch (current_block_sector_index)
                                {
                                    case 0: second_sector_start = 4.0 * glD.micIndex / MIC_BUFFER_LENGTH;
                                        break;
                                    case 1: third_sector_start = 4.0 * glD.micIndex / MIC_BUFFER_LENGTH;
                                        break;
                                    case 2: fourth_sector_start = 4.0 * glD.micIndex / MIC_BUFFER_LENGTH;
                                        break;
                                    default:
                                        break;
                                }
                                current_block_sector_index++;
                                if (current_block_sector_index == 4)
                                {
                                    current_block_sector_index = 0;
                                    glD.micIndex = 0;
                                    glD.ecgIndex = 0; //????????????????
                                    glD.accIndex = 0; //????????????????
                                    glD.ppgIndex = 0; //????????????????
                                }

                            }
                            //sinchronization block
                            micRingBuffer.Read(dataBuffer, (int)mic_no_data);
                            micRingBuffer.Read(tmpBuffer, 5);
                            if (compareByteArrayAndString(tmpBuffer, "|END|", 5))
                            {
                                if ((String.Compare(mainWindowVeiwModel.ComboboxSelectedItem, "ALL DEVICES") == 0) || (String.Compare(mainWindowVeiwModel.ComboboxSelectedItem, "MIC") == 0))
                                {
                                    micState = TCPDeviceState.TRANSFERING;
                                    micStateUpdated = true;
                                    for (int i = 0; i < mic_no_data / 2; i += 2)
                                    {
                                        int16_temp = (Int16)(dataBuffer[i] * 256 + dataBuffer[i + 1]);
                                        glD.micChannels.ElementAt(0).intArray[glD.micIndex] = int16_temp;

                                        int16_temp = (Int16)(dataBuffer[i + mic_no_data / 2] * 256 + dataBuffer[i + mic_no_data / 2 + 1]);
                                        glD.micChannels.ElementAt(1).intArray[glD.micIndex] = int16_temp;
                                        glD.micIndex++;
                                    }
                                }
                            }
                        }
                        else if (compareByteArrayAndString(tmpBuffer, "IDLE_PACKETxxx", 14))
                        {
                            if ((ecgState != TCPDeviceState.TRANSFERING) && (acc_ppgState != TCPDeviceState.TRANSFERING) && (fbgaModule.fbgaMwlsStatus != USBDeviceStateEnum.TRANSFERING)) playing = false;
                            micRingBuffer.Read(tmpBuffer, 6);
                            if ((tmpBuffer[2] == 0x0A))
                            {
                                if (!syncRecording)
                                {
                                    micRingBuffer.Reset();
                                    ecgRingBuffer.Reset();
                                    acc_ppgRingBuffer.Reset();
                                    fbgaRingBuffer.Reset();

                                    try
                                    {
                                        fbgaModule.startRecording(savePath, saveFileName);
                                        fbgaModule.startAcquisition(true);

                                    }
                                    catch (Exception ex)
                                    {
                                        syncRecordingStatus = syncRecordingStatus | 0x02;
                                    }
                                    try
                                    {
                                        micModule.startRecording();
                                    }
                                    catch (Exception ex)
                                    {
                                        syncRecordingStatus = syncRecordingStatus | 0x01;
                                    }

                                    try
                                    {
                                        ecgModule.startRecording();
                                    }
                                    catch (Exception ex)
                                    {
                                        syncRecordingStatus = syncRecordingStatus | 0x04;
                                    }

                                    try
                                    {
                                        acc_ppgModule.startRecording();
                                    }
                                    catch (Exception ex)
                                    {
                                        syncRecordingStatus = syncRecordingStatus | 0x08;
                                    }
                                }
                                syncRecording = true;
                            }
                            if ((tmpBuffer[4] & 0x01) == 0x01) mainWindowVeiwModel.SettingMICData.MuteMIC1 = true;
                            else mainWindowVeiwModel.SettingMICData.MuteMIC1 = false;
                            if ((tmpBuffer[4] & 0x02) == 0x02) mainWindowVeiwModel.SettingMICData.MuteMIC2 = true;
                            else mainWindowVeiwModel.SettingMICData.MuteMIC2 = false;
                            if ((tmpBuffer[4] & 0x04) == 0x04) mainWindowVeiwModel.SettingMICData.MuteMIC3 = true;
                            else mainWindowVeiwModel.SettingMICData.MuteMIC3 = false;
                            if ((tmpBuffer[4] & 0x08) == 0x08) mainWindowVeiwModel.SettingMICData.MuteMIC4 = true;
                            else mainWindowVeiwModel.SettingMICData.MuteMIC4 = false;
                            if ((tmpBuffer[5] & 0x01) == 0x01) mainWindowVeiwModel.SettingMICData.HighPassFilter = true;
                            else mainWindowVeiwModel.SettingMICData.HighPassFilter = false;
                           

                            micRingBuffer.Read(tmpBuffer, 40);
                            batteryValueUint = (uint)(tmpBuffer[0] * 256 + tmpBuffer[1]);
                            if ((tmpBuffer[2] & 0x02) == 0x02) mainWindowVeiwModel.SettingMICData.SyncTest = true;
                            else mainWindowVeiwModel.SettingMICData.SyncTest = false;


                            micRingBuffer.Read(tmpBuffer, 5);
                            if (compareByteArrayAndString(tmpBuffer, "|END|", 5))
                            {
                                micState = TCPDeviceState.IDLE;
                                micStateUpdated = true;
                            }
                        }
                    }
                }

                while (ecgRingBuffer.ReadSpace() >= 151)
                {
                    ecgRingBuffer.Read(tmpBuffer, 6);
                    if (compareByteArrayAndString(tmpBuffer, "|HEAD|", 6))
                    {

                        ecgRingBuffer.Read(tmpBuffer, 14);
                        if (compareByteArrayAndString(tmpBuffer, "DATA_PACKETxxx", 14))
                        {
                            playing = true;
                            ecgRingBuffer.Read(tmpBuffer, 6);
                            ecg_no_data = ((uint)tmpBuffer[0]) * 256 + (uint)tmpBuffer[1];
                            ecg_block_num = ((uint)tmpBuffer[2]) * 256 + (uint)tmpBuffer[3];
                            ecg_seq_num = ((uint)tmpBuffer[4]) * 256 + (uint)tmpBuffer[5];
                            /*                           if (ecg_previous_block_number != ecg_block_num) { 

                                                    }*/
                            ecgRingBuffer.Read(dataBuffer, (int)ecg_no_data);
                            ecgRingBuffer.Read(tmpBuffer, 5);
                            if (compareByteArrayAndString(tmpBuffer, "|END|", 5))
                            {
                                if ((String.Compare(mainWindowVeiwModel.ComboboxSelectedItem, "ALL DEVICES") == 0) || (String.Compare(mainWindowVeiwModel.ComboboxSelectedItem, "ECG") == 0))
                                {
                                    ecgState = TCPDeviceState.TRANSFERING;
                                    ecgStateUpdated = true;
                                    for (int i = 0; i < ecg_no_data / 8; i += 3)
                                    {
                                        // channel I (LA-RA)                     
                                        int32_temp = (Int32)(dataBuffer[i] * 65536 + dataBuffer[i + 1] * 256 + dataBuffer[i + 2]);
                                        if ((int32_temp & 0x800000) == 0x800000)
                                            glD.ecgChannels.ElementAt(0).intArray[glD.ecgIndex] = int32_temp - 16777216;
                                        else
                                            glD.ecgChannels.ElementAt(0).intArray[glD.ecgIndex] = int32_temp;
                                        //channel II (LL-RA)
                                        int32_temp = (Int32)(dataBuffer[i + ecg_no_data / 8] * 65536 + dataBuffer[i + ecg_no_data / 8 + 1] * 256 + dataBuffer[i + ecg_no_data / 8 + 2]);
                                        if ((int32_temp & 0x800000) == 0x800000)
                                            glD.ecgChannels.ElementAt(1).intArray[glD.ecgIndex] = int32_temp - 16777216;
                                        else
                                            glD.ecgChannels.ElementAt(1).intArray[glD.ecgIndex] = int32_temp;
                                        //channel V1 
                                        int32_temp = (Int32)(dataBuffer[i + 2 * ecg_no_data / 8] * 65536 + dataBuffer[i + 2 * ecg_no_data / 8 + 1] * 256 + dataBuffer[i + 2 * ecg_no_data / 8 + 2]);
                                        if ((int32_temp & 0x800000) == 0x800000)
                                            glD.ecgChannels.ElementAt(2).intArray[glD.ecgIndex] = int32_temp - 16777216;
                                        else
                                            glD.ecgChannels.ElementAt(2).intArray[glD.ecgIndex] = int32_temp;
                                        //channel V2
                                        int32_temp = (Int32)(dataBuffer[i + 3 * ecg_no_data / 8] * 65536 + dataBuffer[i + 3 * ecg_no_data / 8 + 1] * 256 + dataBuffer[i + 3 * ecg_no_data / 8 + 2]);
                                        if ((int32_temp & 0x800000) == 0x800000)
                                            glD.ecgChannels.ElementAt(3).intArray[glD.ecgIndex] = int32_temp - 16777216;
                                        else
                                            glD.ecgChannels.ElementAt(3).intArray[glD.ecgIndex] = int32_temp;
                                        //channel V3
                                        int32_temp = (Int32)(dataBuffer[i + 4 * ecg_no_data / 8] * 65536 + dataBuffer[i + 4 * ecg_no_data / 8 + 1] * 256 + dataBuffer[i + 4 * ecg_no_data / 8 + 2]);
                                        if ((int32_temp & 0x800000) == 0x800000)
                                            glD.ecgChannels.ElementAt(4).intArray[glD.ecgIndex] = int32_temp - 16777216;
                                        else
                                            glD.ecgChannels.ElementAt(4).intArray[glD.ecgIndex] = int32_temp;
                                        //channel V4
                                        int32_temp = (Int32)(dataBuffer[i + 5 * ecg_no_data / 8] * 65536 + dataBuffer[i + 5 * ecg_no_data / 8 + 1] * 256 + dataBuffer[i + 5 * ecg_no_data / 8 + 2]);
                                        if ((int32_temp & 0x800000) == 0x800000)
                                            glD.ecgChannels.ElementAt(5).intArray[glD.ecgIndex] = int32_temp - 16777216;
                                        else
                                            glD.ecgChannels.ElementAt(5).intArray[glD.ecgIndex] = int32_temp;
                                        //channel V5
                                        int32_temp = (Int32)(dataBuffer[i + 6 * ecg_no_data / 8] * 65536 + dataBuffer[i + 6 * ecg_no_data / 8 + 1] * 256 + dataBuffer[i + 6 * ecg_no_data / 8 + 2]);
                                        if ((int32_temp & 0x800000) == 0x800000)
                                            glD.ecgChannels.ElementAt(6).intArray[glD.ecgIndex] = int32_temp - 16777216;
                                        else
                                            glD.ecgChannels.ElementAt(6).intArray[glD.ecgIndex] = int32_temp;
                                        //channel V6
                                        int32_temp = (Int32)(dataBuffer[i + 7 * ecg_no_data / 8] * 65536 + dataBuffer[i + 7 * ecg_no_data / 8 + 1] * 256 + dataBuffer[i + 7 * ecg_no_data / 8 + 2]);
                                        if ((int32_temp & 0x800000) == 0x800000)
                                            glD.ecgChannels.ElementAt(7).intArray[glD.ecgIndex] = int32_temp - 16777216;
                                        else
                                            glD.ecgChannels.ElementAt(7).intArray[glD.ecgIndex] = int32_temp;
                                        /*
                                        //channel aVR=-(I+II)/2
                                        glD.ecgChannels.ElementAt(9).intArray[glD.ecgIndex] = -(glD.ecgChannels.ElementAt(0).intArray[glD.ecgIndex] + glD.ecgChannels.ElementAt(1).intArray[glD.ecgIndex]) / 2;
                                        //channel aVL=(I-III)/2
                                        glD.ecgChannels.ElementAt(10).intArray[glD.ecgIndex] = (glD.ecgChannels.ElementAt(0).intArray[glD.ecgIndex] - glD.ecgChannels.ElementAt(2).intArray[glD.ecgIndex]) / 2;
                                        //channel aVF=(II+III)/2
                                        glD.ecgChannels.ElementAt(11).intArray[glD.ecgIndex] = (glD.ecgChannels.ElementAt(1).intArray[glD.ecgIndex] + glD.ecgChannels.ElementAt(2).intArray[glD.ecgIndex]) / 2;*/

                                        glD.ecgIndex++;
                                    }
                                }
                            }
                        }
                        else if (compareByteArrayAndString(tmpBuffer, "IDLE_PACKETxxx", 14))
                        {
                            if ((micState != TCPDeviceState.TRANSFERING) && (acc_ppgState != TCPDeviceState.TRANSFERING) && (fbgaModule.fbgaMwlsStatus != USBDeviceStateEnum.TRANSFERING)) playing = false;
                            ecgRingBuffer.Read(tmpBuffer, 6);
                            mainWindowVeiwModel.SettingECGData.Gain = tmpBuffer[2];

                            if (tmpBuffer[3] == 0) mainWindowVeiwModel.SettingECGData.CH4Mode = "NORMAL";
                            else if (tmpBuffer[3] == 2) mainWindowVeiwModel.SettingECGData.CH4Mode = "RLD_IN";
                            else if (tmpBuffer[3] == 3) mainWindowVeiwModel.SettingECGData.CH4Mode = "MVDD";
                            else if (tmpBuffer[3] == 5) mainWindowVeiwModel.SettingECGData.CH4Mode = "TEST";

                            mainWindowVeiwModel.SettingECGData.SensP = tmpBuffer[4];
                            mainWindowVeiwModel.SettingECGData.SensN = tmpBuffer[5];

                            ecgRingBuffer.Read(tmpBuffer, 120);
                            ecgRingBuffer.Read(tmpBuffer, 5);
                            if (compareByteArrayAndString(tmpBuffer, "|END|", 5))
                            {
                                ecgState = TCPDeviceState.IDLE;
                                ecgStateUpdated = true;
                            }
                        }
                    }

                }
                if (acc_ppgRingBuffer.ReadSpace() >= 10000)
                {
                    error_code = 22;
                }

                // stopwatch_values[debug_index] = stopwatch.ElapsedMilliseconds;
                // rptr_values[debug_index] = acc_ppgRingBuffer.rptr;
                // wptr_values[debug_index++] = acc_ppgRingBuffer.wptr;


                while (acc_ppgRingBuffer.ReadSpace() >= 151)
                {
                    acc_ppgRingBuffer.Read(tmpBuffer, 6);
                    if (compareByteArrayAndString(tmpBuffer, "|HEAD|", 6))
                    {

                        acc_ppgRingBuffer.Read(tmpBuffer, 14);
                        if (compareByteArrayAndString(tmpBuffer, "DATA_PACKETxxx", 14))
                        {
                            playing = true;
                            acc_ppgRingBuffer.Read(tmpBuffer, 6);
                            acc_ppg_no_data = ((uint)tmpBuffer[0]) * 256 + (uint)tmpBuffer[1];
                            acc_ppg_block_num = ((uint)tmpBuffer[2]) * 256 + (uint)tmpBuffer[3];
                            acc_ppg_seq_num = ((uint)tmpBuffer[4]) * 256 + (uint)tmpBuffer[5];

                            if (acc_ppg_no_data != 120)
                                error_code = 21;
                            acc_ppgRingBuffer.Read(dataBuffer, (int)acc_ppg_no_data);

                            acc_ppgRingBuffer.Read(tmpBuffer, 5);
                            if (compareByteArrayAndString(tmpBuffer, "|END|", 5))
                            {
                                if ((String.Compare(mainWindowVeiwModel.ComboboxSelectedItem, "ALL DEVICES") == 0) || (String.Compare(mainWindowVeiwModel.ComboboxSelectedItem, "ACC") == 0) || (String.Compare(mainWindowVeiwModel.ComboboxSelectedItem, "PPG") == 0))
                                {
                                    acc_ppgState = TCPDeviceState.TRANSFERING;
                                    acc_ppgStateUpdated = true;
                                    for (int i = 0; i < acc_ppg_no_data / 12; i += 2)
                                    {
                                        // acc1x
                                        int16_temp = (Int16)(dataBuffer[i] * 256 + dataBuffer[i + 1]);
                                        glD.accChannels.ElementAt(0).intArray[glD.accIndex] = int16_temp;

                                        //acc1y
                                        int16_temp = (Int16)(dataBuffer[i + acc_ppg_no_data / 12] * 256 + dataBuffer[i + acc_ppg_no_data / 12 + 1]);
                                        glD.accChannels.ElementAt(1).intArray[glD.accIndex] = int16_temp;

                                        //acc1z
                                        int16_temp = (Int16)(dataBuffer[i + 2 * acc_ppg_no_data / 12] * 256 + dataBuffer[i + 2 * acc_ppg_no_data / 12 + 1]);
                                        glD.accChannels.ElementAt(2).intArray[glD.accIndex] = int16_temp;

                                        // acc2x
                                        int16_temp = (Int16)(dataBuffer[i + 3 * acc_ppg_no_data / 12] * 256 + dataBuffer[i + 3 * acc_ppg_no_data / 12 + 1]);
                                        glD.accChannels.ElementAt(3).intArray[glD.accIndex] = int16_temp;

                                        //acc2y
                                        int16_temp = (Int16)(dataBuffer[i + 4 * acc_ppg_no_data / 12] * 256 + dataBuffer[i + 4 * acc_ppg_no_data / 12 + 1]);
                                        glD.accChannels.ElementAt(4).intArray[glD.accIndex] = int16_temp;

                                        //acc2z
                                        int16_temp = (Int16)(dataBuffer[i + 5 * acc_ppg_no_data / 12] * 256 + dataBuffer[i + 5 * acc_ppg_no_data / 12 + 1]);
                                        glD.accChannels.ElementAt(5).intArray[glD.accIndex] = int16_temp;

                                        // acc3x
                                       /* int16_temp = (Int16)(dataBuffer[i + 6 * acc_ppg_no_data / 12] * 256 + dataBuffer[i + 6 * acc_ppg_no_data / 12 + 1]);
                                        glD.accChannels.ElementAt(6).intArray[glD.accIndex] = int16_temp;

                                        //acc3y
                                        int16_temp = (Int16)(dataBuffer[i + 7 * acc_ppg_no_data / 12] * 256 + dataBuffer[i + 7 * acc_ppg_no_data / 12 + 1]);
                                        glD.accChannels.ElementAt(7).intArray[glD.accIndex] = int16_temp;

                                        //acc3z
                                        int16_temp = (Int16)(dataBuffer[i + 8 * acc_ppg_no_data / 12] * 256 + dataBuffer[i + 8 * acc_ppg_no_data / 12 + 1]);
                                        glD.accChannels.ElementAt(8).intArray[glD.accIndex] = int16_temp;*/

                                        glD.accIndex++;


                                    }
                                    // ppg_ch1g
                                    uint16_temp = (UInt16)(dataBuffer[9 * acc_ppg_no_data / 12] * 256 + dataBuffer[9 * acc_ppg_no_data / 12 + 1]);
                                    glD.ppgChannels.ElementAt(0).intArray[glD.ppgIndex] = uint16_temp;

                                    /*if (first_attempt)
                                        previous_value = uint16_temp;
                                    else {
                                        if (uint16_temp != 30000) {
                                            if ((uint16_temp - previous_value) != 5) {
                                                bool error = true;
                                            }
                                        }
                                        previous_value = uint16_temp;
                                    }
                                    first_attempt = false;*/

                                    //ppg_ch1r
                                    uint16_temp = (UInt16)(dataBuffer[9 * acc_ppg_no_data / 12 + 2] * 256 + dataBuffer[9 * acc_ppg_no_data / 12 + 3]);
                                    glD.ppgChannels.ElementAt(1).intArray[glD.ppgIndex] = uint16_temp;

                                    //ppg_ch2g
                                    uint16_temp = (UInt16)(dataBuffer[9 * acc_ppg_no_data / 12 + 4] * 256 + dataBuffer[9 * acc_ppg_no_data / 12 + 5]);
                                    glD.ppgChannels.ElementAt(2).intArray[glD.ppgIndex] = uint16_temp;

                                    /*// ppg_ch2r
                                    uint16_temp = (UInt16)(dataBuffer[9 * acc_ppg_no_data / 12 + 6] * 256 + dataBuffer[9 * acc_ppg_no_data / 12 + 7]);
                                    glD.ppgChannels.ElementAt(3).intArray[glD.ppgIndex] = uint16_temp;

                                    //ppg_ch3g
                                    uint16_temp = (UInt16)(dataBuffer[9 * acc_ppg_no_data / 12 + 8] * 256 + dataBuffer[9 * acc_ppg_no_data / 12 + 9]);
                                    glD.ppgChannels.ElementAt(4).intArray[glD.ppgIndex] = uint16_temp;

                                    //ppg_ch3rg
                                    uint16_temp = (UInt16)(dataBuffer[9 * acc_ppg_no_data / 12 + 10] * 256 + dataBuffer[9 * acc_ppg_no_data / 12 + 11]);
                                    glD.ppgChannels.ElementAt(5).intArray[glD.ppgIndex] = uint16_temp;*/

                                    if (glD.ppgIndex == 399)
                                    {
                                        //acc_ppgRingBuffer.Reset();
                                    }
                                    glD.ppgIndex++;

                                }
                            }
                        }
                        else if (compareByteArrayAndString(tmpBuffer, "IDLE_PACKETxxx", 14))
                        {
                            if ((micState != TCPDeviceState.TRANSFERING) && (ecgState != TCPDeviceState.TRANSFERING) && (fbgaModule.fbgaMwlsStatus != USBDeviceStateEnum.TRANSFERING)) playing = false;
                            acc_ppgRingBuffer.Read(tmpBuffer, 6);
                            acc_ppgRingBuffer.Read(tmpBuffer, 120);
                            acc_ppgRingBuffer.Read(tmpBuffer, 5);
                            if (compareByteArrayAndString(tmpBuffer, "|END|", 5))
                            {
                                acc_ppgState = TCPDeviceState.IDLE;
                                acc_ppgStateUpdated = true;
                            }
                        }
                    }
                    else
                    {
                        bool error = false; ;
                    }

                }
                glD.renderDisplays();

            }
            catch (Exception ex) {
                Log log = new Log();
                log.LogMessageToFile(TAG + "ParseRingBuffers:" + ex.Message);
            }
            
        }
        private bool compareByteArrayAndString(byte[] byteArray, string stringArray, int size)
        {
            for (int i = 0; i < size; i++)
            {
                if (byteArray[i] != stringArray.ElementAt(i))
                {
                    return false;
                }
            }
            return true;
        }
        public void dispose(object sender, EventArgs e) {
            try
            {
                if (micModule != null) micModule.dispose();
                if (ecgModule != null) ecgModule.dispose();
                if (acc_ppgModule != null) acc_ppgModule.dispose();
                fbgaModule.dispose();
                tcpService.Stop();
                parseTimer.Stop();
            }
            catch (Exception ex) {
                Log log = new Log();
                log.LogMessageToFile(TAG + "dispose:" + ex.Message);
                Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Error, ex.Message);
                Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);
                
            }
        }                
    }
}
