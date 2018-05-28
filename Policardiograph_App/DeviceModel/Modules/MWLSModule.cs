using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using Policardiograph_App.Exceptions;

namespace Policardiograph_App.DeviceModel.Modules
{
    public class MWLSModule: USBModule
    {
        #region DLL_Import
        const string dllRelativePath = "..\\..\\Dependencies\\Sense2020Dll.dll";
        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DLL_Init_SLED();

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DLL_Close_SLED();

        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DLL_TurnOn_SLED(double dblPower);
        //???
        [DllImport(dllRelativePath, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DLL_TurnOff_SLED();
        #endregion

        public bool Initialized {get; set;}
        public bool TurnedOn { get; set; }

        public MWLSModule() {            
            Initialized = false;
            TurnedOn = false;
        }
        public void InitSLED(double sledPWR) {
            try
            {
                if (!DLL_Init_SLED())                                                //initialize SLED
                {
                    throw new InitMWLSException("SLED initialization failed: Can not initialize SLED");

                }
                Thread.Sleep(200);
                Initialized = true;
                if (!DLL_TurnOn_SLED(sledPWR))
                    throw new TurnOnMWLSException("SLED initialization failed: Can not turn on SLED");
                TurnedOn = true;
            }
            catch (Exception ex) {
                throw ex;
            }
            
        }
        public void TurnOffSLED(double sledPWR)
        {
            try
            {
                if (!DLL_TurnOff_SLED())
                    throw new TurnOnMWLSException("SLED turn off failed: Can not turn off SLED");
                TurnedOn = false;
                }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}
