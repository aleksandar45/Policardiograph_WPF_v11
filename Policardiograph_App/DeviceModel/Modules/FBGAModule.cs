using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using Policardiograph_App.Exceptions;
using Policardiograph_App.Settings;
using Policardiograph_App.DeviceModel.RingBuffers;

namespace Policardiograph_App.DeviceModel.Modules
{
    public class FBGAModule: USBModule
    {
        #region DLL_Import
        const string dllRelativePath = "Sense2020Dll.dll";
        //const string dllRelativePath = "..\\..\\Dependencies\\Sense2020Dll.dll";
        //C:\\Users\\Djole\\Documents\\Visual Studio 2010\\Projects\\Policardiograph_WPF\\Policardiograph_WPF\\Dependencies\\Sense2020Dll.dll"
        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int DLL_Get_DLL_Version();

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int DLL_Connect_System([MarshalAs(UnmanagedType.LPStr)] string lpszSystemFileDirectory);

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DLL_Close_Device();

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        public static extern bool DLL_Is_Device_OK();

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        public static extern bool DLL_Get_Device_SN([MarshalAs(UnmanagedType.LPStr)] StringBuilder stringlpszSN);

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DLL_Get_HW_FW_REV([MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] int[] pdwHW, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] int[] pdwFW);//???

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int DLL_Get_Pixel_Count();

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DLL_Get_Device_Configuration([MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] bool[] pbGPIO, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] bool[] pbTrigger, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] bool[] pbIRS, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] bool[] pbSwitch, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] bool[] pbSLED, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] int[] pwSwitchChannelCount, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] double[] pdblSLEDMinPwr, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] double[] pdblSLEDMaxPwr);

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DLL_Set_Sensor_Mode([MarshalAs(UnmanagedType.I4)] int wSensorMode);

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int DLL_Get_Sensor_Mode();

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DLL_Set_IntegrationTime_SampleRate([MarshalAs(UnmanagedType.I4)] int dwIntegrationTime, [MarshalAs(UnmanagedType.I4)] int dwSampleRate);

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DLL_Get_IntegrationTime_SampleRate([MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] int[] pdwIntegrationTime, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] int pdwSampleRate);

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DLL_Set_GPIO([MarshalAs(UnmanagedType.I1)] sbyte byteGPIOState);

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DLL_Get_GPIO([MarshalAs(UnmanagedType.LPArray)] sbyte[] pbyteGPIOState);

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DLL_Acquire_Spectrum([MarshalAs(UnmanagedType.LPArray, SizeConst = 512)] double[] pdblWavelength, [MarshalAs(UnmanagedType.LPArray, SizeConst = 512)]  double[] pdblPower, [MarshalAs(UnmanagedType.LPArray, SizeConst = 512)]  double[] pdblTemperature);

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DLL_Start_Continuously_Acquire_Spectra([MarshalAs(UnmanagedType.Bool)]bool bTrigger);

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        public static extern void DLL_Stop_Continuously_Acquire_Spectra();

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int DLL_Get_TotalSpectrumCount();

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DLL_Get_SpectrumInformation([MarshalAs(UnmanagedType.I4)] int dwSpectrumIndex, [MarshalAs(UnmanagedType.I4)] int wProcessCode, [MarshalAs(UnmanagedType.LPArray, SizeConst = 512)] double[] pdblBackground, double dblThresholdUniform, [MarshalAs(UnmanagedType.LPArray, SizeConst = 512)] double[] pdblThresholdProfile, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] int[] pdwSpectrumLineStamp, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] double[] pdblTemperature, [MarshalAs(UnmanagedType.LPArray, SizeConst = 512)] double[] pdblWavelength, [MarshalAs(UnmanagedType.LPArray, SizeConst = 512)] double[] pdblPower, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] int[] pwPeakCount, [MarshalAs(UnmanagedType.LPArray, SizeConst = 512)] double[] pdblPeakWavelength, [MarshalAs(UnmanagedType.LPArray, SizeConst = 512)] double[] pdblPeakPower, [MarshalAs(UnmanagedType.LPArray, SizeConst = 512)] double[] pdblPeakFWHM);
        //???
        #endregion

        private MWLSModule mwlsModule;
        private Object lockObject;
        private Thread thread;
        private bool acquisition;
        private bool saveToFile;
        private bool dllLibOpened = false;
        private string TAG = "DeviceModel/FBGAModule/";

        protected RingBufferDouble ringBuffer;
        protected FileStream binaryWriter;        

        public USBDeviceStateEnum fbgaMwlsStatus {get; set;}

        public FBGAModule(RingBufferDouble ringBuffer) {
            this.ringBuffer = ringBuffer;
            this.mwlsModule = new MWLSModule();
            this.fbgaMwlsStatus = USBDeviceStateEnum.NOT_INITIALIZED;
            this.lockObject = new System.Object(); 

           
        }
        public void SendSetting(SettingFBGA fbgaSetting) {
            try
            {
                Initialize(fbgaSetting);
            }
            catch (Exception ex) {
                Log log = new Log();
                log.LogMessageToFile(TAG + "SendSetting:" + ex.Message);
            }

        }
        public void Initialize(SettingFBGA fbgaSetting) {
            int a;            
            int[] pdwHW = new int[2];
            int[] pdwFW = new int[2];
            StringBuilder stringlpszSN = new StringBuilder(10);

            string lpszSystemFileDirectory = System.IO.Directory.GetCurrentDirectory();
            fbgaMwlsStatus = USBDeviceStateEnum.NOT_INITIALIZED;

            if (22002082 != DLL_Get_DLL_Version()) throw new FBGAException("FBGA initialization failed: DLL version is not verified");


            a = DLL_Connect_System(lpszSystemFileDirectory);            
            switch (a)
            {
                case 0:
                    break;
                case 1:
                    throw new FBGAException("FBGA initialization failed: Device is acquiring spectrum in continuous mode");                    
                case 2:
                    throw new FBGAException("FBGA initialization failed: Create USB instance failed");                    
                case 3:
                    throw new FBGAException("FBGA initialization failed: No or multiple FBGA connected");                    
                case 4:
                    throw new FBGAException("FBGA initialization failed: Initialize device failed");                    
                case 5:
                    throw new FBGAException("FBGA initialization failed: Wrong sensor type");                    
                case 6:
                    throw new FBGAException("FBGA initialization failed: Allocate memory failed");                    
                case 7:
                    throw new FBGAException("FBGA initialization failed: Load system configuration file faile");                   
                case 8:
                    throw new FBGAException("FBGA initialization failed: Load calibration data file faile");                    
                default:
                    throw new FBGAException("FBGA initialization failed: Device not detected");                    

            }
            if (!DLL_Is_Device_OK())
            {
                throw new FBGAException("FBGA initialization failed: Device not OK");                                
            }
            
            if (!DLL_Get_Device_SN(stringlpszSN)) throw new FBGAException("FBGA initialization failed: Get device serial number error");
            if (!DLL_Get_HW_FW_REV(pdwHW, pdwFW)) throw new FBGAException("FBGA initialization failed: Get HW FW error");
            if (512 != DLL_Get_Pixel_Count())
            {
                throw new FBGAException("FBGA initialization failed: Device pixel count != 512");
            }
            //set device sensor mode to 0(HighSensitive) 1(High Dynamic)
            if (fbgaSetting.HighDynamicRange)
            {
                if (!DLL_Set_Sensor_Mode(1)) throw new FBGAException("FBGA initialization failed: Set sensor mode error");
            }
            else {
                if (!DLL_Set_Sensor_Mode(0)) throw new FBGAException("FBGA initialization failed: Set sensor mode error");
            }
            //set device sample integration time (1000us) and sample rate (500Hz) Tsr>Tint
            if (!DLL_Set_IntegrationTime_SampleRate(fbgaSetting.IntegrationTime, 5000)) throw new FBGAException("FBGA initialization failed: Set integrationTime/samplingRate error");
            fbgaMwlsStatus = USBDeviceStateEnum.INITIALIZED;   
                      
                     
        }
        public void TurnOnOffSLED(SettingFBGA fbgaSetting, bool turnOn) {
            try
            {
                if (turnOn)
                {
                    mwlsModule.InitSLED(fbgaSetting.SLEDPower);
                }
                else {
                    mwlsModule.TurnOffSLED(fbgaSetting.SLEDPower);
                }
                
            }
            catch (InitMWLSException ex)
            {
                throw ex;
            }
            catch (TurnOnMWLSException ex)
            {                
                throw ex;
            }
        }
        public void TryInitialize(SettingFBGA fbgaSetting) { 
            int a;            
            int[] pdwHW = new int[2];
            int[] pdwFW = new int[2];
            StringBuilder stringlpszSN = new StringBuilder(10);

            string lpszSystemFileDirectory = System.IO.Directory.GetCurrentDirectory();
            fbgaMwlsStatus = USBDeviceStateEnum.NOT_INITIALIZED;

            if (22002082 != DLL_Get_DLL_Version()) return;


            a = DLL_Connect_System(lpszSystemFileDirectory);
            if (a != 0) return;
            dllLibOpened = true;

           /* if (!DLL_Is_Device_OK())
            {
                return;                              
            }

            if (!DLL_Get_Device_SN(stringlpszSN)) return;
            if (!DLL_Get_HW_FW_REV(pdwHW, pdwFW)) return;
            if (512 != DLL_Get_Pixel_Count())
            {
                return;
            }
            //set device sensor mode to 0(HighSensitive) 1(High Dynamic)
            if (fbgaSetting.HighDynamicRange)
            {
                if (!DLL_Set_Sensor_Mode(1)) return;
            }
            else
            {
                if (!DLL_Set_Sensor_Mode(0)) return;
            }
            //set device sample integration time (1000us) and sample rate (500Hz) Tsr>Tint
            if (!DLL_Set_IntegrationTime_SampleRate(500, 500)) return;

            */
            /*try{
                 mwlsModule.InitSLED(fbgaSetting.SLEDPower);
            }
            catch (InitMWLSException ex){
                fbgaMwlsStatus = USBDeviceStateEnum.INITIALIZED;
                return;
            }
            catch (TurnOnMWLSException ex){
                fbgaMwlsStatus = USBDeviceStateEnum.INITIALIZED_OFF;
                return;
            }*/
            fbgaMwlsStatus = USBDeviceStateEnum.INITIALIZED;   
        }
        public bool CheckDeviceConnectionStatus() {
            if (DLL_Is_Device_OK())
            {
                if ((!mwlsModule.Initialized) || (!mwlsModule.TurnedOn)) return false;
                return true;
            }
            else
            {               
                return false;
            }
        }
        public virtual void Reset(SettingFBGA fbgaSetting)
        {
            Initialize(fbgaSetting);
            stopAcquisition();
            startAcquisition(false);
        }
        public virtual void startAcquisition(bool trigger)
        {
            try
            {
                //if (fbgaMwlsStatus == USBDeviceStateEnum.INITIALIZED)
                //{
                    if (DLL_Start_Continuously_Acquire_Spectra(trigger))
                    {
                        acquisition = true;
                        thread = new Thread(doProcessing);
                        thread.Start();
                        fbgaMwlsStatus = USBDeviceStateEnum.TRANSFERING;
                    }
                    else throw new FBGAException("FBGA start acquisition failed: FBGA module can not start continuous acquisition");
                //}
                //else throw new FBGAException("FBGA start acquisition failed: FBGA module is not initialized properly");
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        public virtual void stopAcquisition()
        {
            try
            {
                
                //if(fbgaMwlsStatus !=USBDeviceStateEnum.NOT_INITIALIZED) DLL_Stop_Continuously_Acquire_Spectra();
                acquisition = false;
                //if (thread != null) thread.Abort();
                //fbgaMwlsStatus = USBDeviceStateEnum.INITIALIZED_ON;
            }
            catch (Exception ex) {
                throw ex;
            }
           
        }
        public virtual void startRecording(string savePath,string saveFileName) {
            try
            {
                if (File.Exists(savePath + saveFileName + ".polif"))
                {
                    File.Delete(savePath + saveFileName + ".polif");
                }
                binaryWriter = File.OpenWrite(savePath + saveFileName + ".polif");
                this.saveToFile = true;
            }
            catch (DirectoryNotFoundException ex)
            {
                Log log = new Log();
                log.LogMessageToFile(TAG + "startRecording:" + ex.Message);
                throw ex;  //The specified path is invalid (for example, it is on an unmapped drive).
            }
            catch (FileNotFoundException ex)
            {
                Log log = new Log();
                log.LogMessageToFile(TAG + "startRecording:" + ex.Message);
                throw ex;
            }
            catch (IOException ex)
            {
                Log log = new Log();
                log.LogMessageToFile(TAG + "startRecording:" + ex.Message);
                throw ex;  //specified file already in use
            }
            catch (UnauthorizedAccessException ex)
            {
                Log log = new Log();
                log.LogMessageToFile(TAG + "startRecording:" + ex.Message);
                throw ex;  //The caller does not have the required permission.
            }
            catch (Exception ex) {
                Log log = new Log();
                log.LogMessageToFile(TAG + "startRecording:" + ex.Message);
                throw ex;
            }
        }
        public virtual void stopRecording(){
            try
            {
                saveToFile = false;

                lock (lockObject)
                {

                    if (binaryWriter != null)
                    {
                        if(binaryWriter.CanWrite)
                            binaryWriter.Flush();
                        binaryWriter.Close();
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                Log log = new Log();
                log.LogMessageToFile(TAG + "stopRecording:" + ex.Message);
                throw ex; //The stream is closed
            }
            catch (IOException ex)
            {
                Log log = new Log();
                log.LogMessageToFile(TAG + "stopRecording:" + ex.Message);
                throw ex; //An I/O error occurred.
            }
            catch (Exception ex) {
                Log log = new Log();
                log.LogMessageToFile(TAG + "stopRecording:" + ex.Message);
                throw ex;
            }
        }
        public void dispose()
        {
            try
            {
                if (binaryWriter != null) binaryWriter.Close();

                if (acquisition && dllLibOpened) DLL_Stop_Continuously_Acquire_Spectra();                
                dllLibOpened = false;
                acquisition = false;
                DLL_Close_Device();
                
            }
            catch (Exception ex) {
                Log log = new Log();
                log.LogMessageToFile(TAG + "dispose:" + ex.Message);
                throw ex;
            }
            
        }
        
        protected void doProcessing()
        {                        
            int wProcessCode;
            int dwLineIndex, dwLineCount, dwPreviousLineCount;
            int[] pdwLineStamp = new int[2], wPeakCount = new int[2], dwLineStamp = new int[2];
            double[] dblTemp = new double[2];
            double[] pdblWL = new double[512], pdblPwr = new double[512], pdblPeakWL = new double[512], pdblPeakPwr = new double[512], pdblPeakFWHM = new double[512], m_pdblBackground = new double[512];
            

            wPeakCount[0] = 0;
            wProcessCode = 0;
            dwLineIndex = 0;
            dwLineCount = 0;
            dwPreviousLineCount = 0;
            try
            {
                while (acquisition)
                {

                    Thread.Sleep(50);        

                    dwLineCount = DLL_Get_TotalSpectrumCount();
                    if (dwPreviousLineCount > dwLineCount)
                    {
                        DLL_Stop_Continuously_Acquire_Spectra();
                        if (DLL_Start_Continuously_Acquire_Spectra(false))
                        {
                            wPeakCount[0] = 0;
                            wProcessCode = 0;
                            dwLineIndex = 0;
                            dwLineCount = 0;
                            dwPreviousLineCount = 0;
                        }
                        else break;
                           
                    }
                    dwPreviousLineCount = dwLineCount;
                    while (dwLineIndex < dwLineCount)
                    {
                        if (DLL_Get_SpectrumInformation(dwLineIndex, wProcessCode, m_pdblBackground, 0.0, null,
                                                        dwLineStamp, dblTemp, pdblWL, pdblPwr, wPeakCount, pdblPeakWL, pdblPeakPwr, pdblPeakFWHM))
                        {
                            //pdwLineStamp[dwLineIndex] = dwLineStamp[0];
                            if (dwLineIndex == dwLineCount - 1) ringBuffer.Write(pdblPwr, 512);
                            dwLineIndex++;
                            //if (dwLineIndex >= 65535)
                            //{
                                //reverse = false;
                            //}
                            if (saveToFile)
                            {
                                lock (lockObject)
                                {
                                    byte[] byteArray;
                                    byteArray = BitConverter.GetBytes(dwLineStamp[0]);
                                    binaryWriter.Write(byteArray, 0, byteArray.Length);
                                    for (int j = 0; j < 512; j++)
                                    {
                                        byteArray = BitConverter.GetBytes(pdblPwr[j]);
                                        binaryWriter.Write(byteArray, 0, byteArray.Length);
                                    }
                                }
                            }
                        }
                        


                    }                                  
                }
                if ((fbgaMwlsStatus != USBDeviceStateEnum.NOT_INITIALIZED) && dllLibOpened) DLL_Stop_Continuously_Acquire_Spectra();
                fbgaMwlsStatus = USBDeviceStateEnum.INITIALIZED;
                //DLL_Close_Device();
            }
            finally
            {
                if (binaryWriter != null) binaryWriter.Close();
                
            }

        }
    }
}
