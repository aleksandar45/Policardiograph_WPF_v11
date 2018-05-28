#ifndef _Sense2020DLL_
#define _Sense2020DLL_


//difine the export type
//#define DLL_PORT_TYPE __declspec(dllexport)
//#define DLL_PORT_FUNCTION_TYPE __stdcall	//this can be used in visual basic program

//----------------------------------Export function declaration----------------------------------
/*function
********************************************************************************
<PRE>
Name:			DLL_Get_DLL_Version

Operation:		Get version of Sense 2020 DLL

Parameter:		None

Return value:	DWORD Version of Sense 2020 DLL

Requirement:	None
-------------------------------------------------------------------------------- 
</PRE>
*******************************************************************************/ 
DLL_PORT_TYPE	DWORD  DLL_PORT_FUNCTION_TYPE DLL_Get_DLL_Version();


/*function
********************************************************************************
<PRE>
Name:			DLL_Connect_System

Operation:		Creat an USB instance and initialize the FBGA devices 

Parameter:		lpszSystemFileDirectory: the file directory of the system configuration and calibration files

Return value:	WORD error code: 
				0: No error
				1: Device is acquiring spectrum in continuous mode
				2: Create USB instance failed
				3. No or multiple FBGA connected to the system 
				4. Initialize device failed
				5. Wrong sensor type
				6. Allocate memory failed
				7. Load system configuration file faile
				8. Load calibration data file faile

Requirement:	1. The system configuration file BS_System_xxxxxxxx.ini should be in the same folder with Sense2020Dll.dll
				2. The calibration data file Calib_LUT_xxxxxxxx.dat should be in the same folder with Sense2020Dll.dll
				3. FBGA device is not in countinuous data acquiring mode
				4. Only one FBGA devices are connected with the host PC

Note:			1. All memories used by Sense2020Dll.dll have been allocated in this function.
				2. DLL_Close_Device must be called to delete these memories  
				3. xxxxxxxx is the FBGA serial number
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/ 
DLL_PORT_TYPE	WORD  DLL_PORT_FUNCTION_TYPE DLL_Connect_System(LPCSTR lpszSystemFileDirectory);


/*function
********************************************************************************
<PRE>
Name:			DLL_Close_Device

Operation:		Delete allocated memories and USB instance

Parameter:		None

Return value:	None

Requirement:	None
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/ 
DLL_PORT_TYPE	void DLL_PORT_FUNCTION_TYPE DLL_Close_Device();

/*function
********************************************************************************
<PRE>
Name:			DLL_Is_Device_OK

Operation:		Check if the device is initialized successfully

Parameter:		None

Return value:	TRUE:  Device initialized successfully
				FALSE: Device initialized failed

Requirement:	None
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/ 
DLL_PORT_TYPE	BOOL DLL_PORT_FUNCTION_TYPE DLL_Is_Device_OK();

/*function
********************************************************************************
<PRE>
Name:			DLL_Get_Device_SN

Operation:		Get device serial number
 
Parameter:		lpszSN: Pointer of char buffer. Buffer size >= 8 byte

Return value:	TRUE:  Operation completed, 
				FALSE: Operation failed

Requirement:	FBGA devices have been initialized successfully
-------------------------------------------------------------------------------- 
</PRE>
*******************************************************************************/ 
DLL_PORT_TYPE	BOOL  DLL_PORT_FUNCTION_TYPE DLL_Get_Device_SN(LPSTR lpszSN);


/*function
********************************************************************************
<PRE>
Name:			DLL_Get_HW_FW_REV

Operation:		Get hardware and firmware revisions

Parameter:		pdwHW:  Pointer of DWORD hardware revision
				pdwFW:  Pointer of DWORD firmware revision

Return value:   TRUE:  Operation completed, 
				FALSE: Operation failed

Requirement:	FBGA devices have been initialized successfully
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/								
DLL_PORT_TYPE BOOL DLL_PORT_FUNCTION_TYPE DLL_Get_HW_FW_REV(DWORD* pdwHW,
														    DWORD* pdwFW); 

/*function
********************************************************************************
<PRE>
Name:			DLL_Get_Pixel_Count

Operation:		Get pixel count of sensor arrary
 
Parameter:		None

Return value:	Pixel Count
				-1:  Operation failed

Requirement:	FBGA devices have been initialized successfully
-------------------------------------------------------------------------------- 
</PRE>
*******************************************************************************/ 
DLL_PORT_TYPE	int  DLL_PORT_FUNCTION_TYPE DLL_Get_Pixel_Count();

/*function
********************************************************************************
<PRE>
Name:			DLL_Get_Device_Configuration

Operation:		Get device configuration
 
Parameter:		pbGPIO:					Pointer of BOOL GPIO option enabled
				pbTrigger:				Pointer of BOOL external triggering option enabled
				pbIRS:					Pointer of BOOL IRS calibration option enabled
				pbSwitch:				Pointer of BOOL optical switch option enabled
				pbSLED:					Pointer of BOOL SLED option enabled
				pwSwitchChannelCount:	Pointer of WORD total channel count of optical switch
				pdblSLEDMinPwr:			Pointer of double minimum output power of SLED, unit: mW
				pdblSLEDMaxPwr:			Pointer of double maximum output power of SLED, unit: mW

Return value:	TRUE:  Operation completed, 
				FALSE: Operation failed	

Requirement:	FBGA devices have been initialized successfully
-------------------------------------------------------------------------------- 
</PRE>
*******************************************************************************/ 
DLL_PORT_TYPE	BOOL DLL_PORT_FUNCTION_TYPE DLL_Get_Device_Configuration(BOOL*   pbGPIO, 
																		 BOOL*   pbTrigger, 
																		 BOOL*   pbIRS, 
																		 BOOL*   pbSwitch, 
																		 BOOL*   pbSLED,
																		 WORD*	 pwSwitchChannelCount, 
																		 double* pdblSLEDMinPwr, 
																		 double* pdblSLEDMaxPwr);

/*function
********************************************************************************
<PRE>
Name:			DLL_Set_IntegrationTime_SampleRate

Operation:		Set integration time and sample rate

Parameter:		dwIntegrationTime: 20 to 300000000 us
				dwSampleRate:		1 to 5000 Hz

Return value:   TRUE:  Operation completed, 
				FALSE: Operation failed

Requirement:	1. FBGA devices have been initialized successfully
				2. FBGA device is not in countinuous acquiring mode

Note:			dwSampleRate < 10E6 / dwIntegrationTime
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/								
DLL_PORT_TYPE BOOL DLL_PORT_FUNCTION_TYPE DLL_Set_IntegrationTime_SampleRate(DWORD dwIntegrationTime, 
																			 DWORD dwSampleRate); 

/*function
********************************************************************************
<PRE>
Name:			DLL_Get_IntegrationTime_SampleRate

Operation:      Get integration time and sample rate

Parameter:		pdwIntegrationTime: Pionter of DWORD integration time; unit: us
				dwSampleRate:		Pionter of DWORD line rate; unit: Hz

Return value:	TRUE:  Operation completed, 
				FALSE: Operation failed

Requirement:	1. FBGA devices have been initialized successfully
				2. FBGA device is not in countinuous acquiring mode
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/ 
DLL_PORT_TYPE	BOOL DLL_PORT_FUNCTION_TYPE   DLL_Get_IntegrationTime_SampleRate(DWORD* pdwIntegrationTime,
																				 DWORD* dwSampleRate);

/*function
********************************************************************************
<PRE>
name:			DLL_Set_Sensor_Mode

operation:      Set device sensor mode

parameter:		wSensorMode: 0 - High sensitivity; 1 - High dynamical range

return value:	TRUE:  Operation completed, 
				FALSE: Operation failed

Requirement:	1. FBGA devices have been initialized successfully
				2. FBGA device is not in countinuous acquiring mode
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/ 
DLL_PORT_TYPE	BOOL DLL_PORT_FUNCTION_TYPE   DLL_Set_Sensor_Mode(WORD wSensorMode); 


/*function
********************************************************************************
<PRE>
name:			DLL_Get_Sensor_Mode

operation:      Get device sensor mode

parameter:		None

return value:	0:  High sensitivity,
				1:  High dynamical range, 
				-1: Operation failed

Requirement:	1. FBGA devices have been initialized successfully
				2. FBGA device is not in countinuous acquiring mode
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/ 
DLL_PORT_TYPE	int DLL_PORT_FUNCTION_TYPE   DLL_Get_Sensor_Mode(); 


/*function
********************************************************************************
<PRE>
name:			DLL_Set_OpticalSwitch

operation:		Set optical switch state

parameter:		bAutoScan:		  TRUE -  Auto scan all channels repeatedly during continuous data acquisition
								  FALSE - Disable auto scan
				wChannelIndex:	  Zero based channel index, wChannelIndex <  Total Channel Count defined in configuration file  								  
 
return value:   TRUE:  Operation completed, 
				FALSE: Operation failed

Requirement:	1. FBGA devices have been initialized successfully
				2. FBGA device is not in countinuous data acquiring mode
				3. The option of optical switch is enabled

Note:			a. When bAutoScan = TRUE, ignore the setting of wChannelIndex  
				b. All settings of optical switches will be lost after device being powered off
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/								
DLL_PORT_TYPE BOOL DLL_PORT_FUNCTION_TYPE DLL_Set_OpticalSwitch(BOOL bAutoScan, WORD wChannelIndex); 


/*function
********************************************************************************
<PRE>
name:			DLL_Get_OpticalSwitch

operation:		Get optical switch status

parameter:		pwChannelCount:	   Pointer of WORD channel count
								   2 - 1x2 optical switch
								   4 - 1x4 optical switch
								   8 - 1x8 optical switch
								   16 - 1x16 optical switch
				pbAutoScan:		   Pointer of BOOL auto scan all channels
								   TRUE  - Auto scan all channels repeatedly during continuous data acquisition
								   FALSE - Disable auto scan
				pwChannelIndex:	   Pointer of WORD zero based channel index						
								   
return value:   TRUE:  Operation completed, 
				FALSE: Operation failed

Requirement:	1. FBGA devices have been initialized successfully
				2. FBGA device is not in countinuous data acquiring mode
				3. The option of optical switch is enabled

Note:			a. All settings of optical switches will be lost after device being powered off
				b. In auto scan mode, the channel index of the optical switch for one acquired spectrum can be calculated 
				   from this spectrum's line stamp
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/								
DLL_PORT_TYPE BOOL DLL_PORT_FUNCTION_TYPE DLL_Get_OpticalSwitch(WORD* pwChannelCount, BOOL* pbAutoScan, WORD* pwChannelIndex); 

/*function
********************************************************************************
<PRE>
name:			DLL_Set_GPIO

operation:      Set device 6 bits GPIO state

parameter:		byteGPIOState: each bit for one GPIO pin 

return value:	TRUE:  Operation completed, 
				FALSE: Operation failed

Requirement:	1. FBGA devices have been initialized successfully
				2. FBGA device is not in countinuous data acquiring mode
				3. The option of GPIO is enabled

Note:			All GPIO settings will be lost after the device being powered off
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/ 
DLL_PORT_TYPE	BOOL DLL_PORT_FUNCTION_TYPE   DLL_Set_GPIO(BYTE byteGPIOState); 


/*function
********************************************************************************
<PRE>
name:			DLL_Get_GPIO

operation:      Get device 6 bits GPIO state

parameter:		pbyteGPIOState: Pionter of byte GPIO state

return value:	TRUE:  Operation completed, 
				FALSE: Operation failed

Requirement:	1. FBGA devices have been initialized successfully
				2. FBGA device is not in countinuous data acquiring mode
				3. The option of GPIO is enabled

Note:			All GPIO settings will be lost after device being powered off
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/ 
DLL_PORT_TYPE	BOOL DLL_PORT_FUNCTION_TYPE   DLL_Get_GPIO(BYTE* pbyteGPIOState); 

/*function
********************************************************************************
<PRE>
name:			DLL_IRS_Calibration

operation:      Perform IRS wavelength calibration

parameter:		None

return value:	TRUE:  Operation completed, 
				FALSE: Operation failed

Requirement:	1. FBGA devices have been initialized successfully
				2. FBGA device is not in countinuous data acquiring mode
				3. The option of IRS calibration is enabled
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/ 
DLL_PORT_TYPE	BOOL DLL_PORT_FUNCTION_TYPE   DLL_IRS_Calibration(); 


/*function
********************************************************************************
<PRE>
name:			DLL_Init_SLED

operation:      Initilize SLED

parameter:		None

return value:	TRUE:  Operation completed, 
				FALSE: Operation failed

Requirement:	1. FBGA devices have been initialized successfully
				2. FBGA device is not in countinuous data acquiring mode
				3. The option of SLED is enabled
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/ 
DLL_PORT_TYPE	BOOL DLL_PORT_FUNCTION_TYPE   DLL_Init_SLED();

/*function
********************************************************************************
<PRE>
name:			DLL_Close_SLED

operation:      Close communication port of SLED

parameter:		None

return value:	None

Requirement:	None 
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/ 
DLL_PORT_TYPE	VOID DLL_PORT_FUNCTION_TYPE   DLL_Close_SLED();


/*function
********************************************************************************
<PRE>
name:			DLL_TurnOn_SLED

operation:      Turn on SLED with required output power

parameter:		dblPower: SLED output power, Unit: mW

return value:	TRUE:  Operation completed, 
				FALSE: Operation failed

Requirement:	1. FBGA devices have been initialized successfully
				2. FBGA device is not in countinuous data acquiring mode
				3. The option of SLED is enabled
				4. SLED initilized successfully
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/ 
DLL_PORT_TYPE	BOOL DLL_PORT_FUNCTION_TYPE   DLL_TurnOn_SLED(double dblPower);

/*function
********************************************************************************
<PRE>
name:			DLL_TurnOff_SLED

operation:      Turn off SLED

parameter:		None

return value:	TRUE:  Operation completed, 
				FALSE: Operation failed

Requirement:	1. FBGA devices have been initialized successfully
				2. FBGA device is not in countinuous data acquiring mode
				3. The option of SLED is enabled
				4. SLED initilized successfully
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/ 
DLL_PORT_TYPE	BOOL DLL_PORT_FUNCTION_TYPE   DLL_TurnOff_SLED();


/*function
********************************************************************************
<PRE>
Name:			DLL_Acquire_Spectrrum

Operation:		Acquire one spectrum 

Parameter:		pdblWaveength:		Pointer of double wavelngth array buffer. Buffer size >= pixel count
				pdblPower:			Pointer of double power array buffer. Buffer size >= pixel count
				pdblTemperature:	Pointer of double device temperature in degree C

Return value:	TRUE:  Operation completed, 
				FALSE: Operation failed

Requirement:	1. FBGA devices have been initialized successfully
				2. FBGA device is not in countinuous data acquiring mode
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/ 
DLL_PORT_TYPE	BOOL  DLL_PORT_FUNCTION_TYPE DLL_Acquire_Spectrum(double* pdblWaveength, double* pdblPower, double* pdblTemperature);

/*function
********************************************************************************
<PRE>
Name:			DLL_SearchPeak

Operation:		Search all peaks in the spectrum

Parameter:		wProcessCode:				Set bit 0 (0x0001), search peak with uniform threshold
											Set bit 1 (0x0002), search peak with profiled threshold
											Set bit 2 (0x0004), Subtract background before peak searching
				wPixelCount:				Pixel count
				pdblWavelength:				Pointer of double wavelength array buffer. Buffer size >= pixel count
				pdblPower:					Pointer to double power array buffer. Buffer size >= pixel count
				pdblBackground:				Pointer to double background array buffer. If set subtract background bit, Buffer size >= pixel count
				dblThresholdUniform:		Uniform threshold for peak search. All peaks should meet two requirements:
											a. The peak power  > dblThresholdUniform 
											b. The peak power increase of adjacent pixels at the rising edge > 0.1 * dblThresholdUniform
				pdblThresholdProfile:		Pointer of double profiled threshold array for peak search. If set search peak with profiled 
											threshold bit, Buffer size >= pixel count. All peaks should meet two requirements
											a. The peak power  > pdblThresholdProfile[this peak pixel]
											b. The peak power increase of adjacent pixels at the rising edge > 0.1 * pdblThresholdProfile[this peak pixel]
				pwPeakCount:				Pointer of WORD spectrum peak count
				pdblPeakWavelength:			Pointer of double spectrum peak wavelengthe array buffer. Buffer size >= pixel count
				pdblPeakPower:				Pointer of double spectrum peak power array buffer. Buffer size >= pixel count
				pdblPeakFWHM:				Pointer of double spectrum peak FWHM array buffer. Buffer size >= pixel count

Return value:   TRUE:  Operation completed, 
				FALSE: Operation failed

Requirement:	None	

Note:			a. Recommend enable subtract background and pass background data during peak search
				b. If a valid peak FWHM can not be found, its value could be -1 or very large number. This may be caused by no background subtraction 
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/								
DLL_PORT_TYPE BOOL DLL_PORT_FUNCTION_TYPE DLL_SearchPeaks(WORD		wProcessCode,
														  WORD		wPixelCount,
														  double*	pdblWavelength,
														  double*	pdblPower,
														  double*	pdblBackground,
														  double	dblThresholdUniform,
														  double*	pdblThresholdProfile,
														  WORD*		pwPeakCount, 
														  double*	pdblPeakWavelength, 
														  double*	pdblPeakPower,
														  double*	pdblPeakFWHM);    

/*function
********************************************************************************
<PRE>
Name:			DLL_Start_Continuously_Acquire_Spectra

Operation:		Start continuous or triggering acquiring spectra

Parameter:		bTrigger	TRUE  - Acquire spectrum at rising edge of external trigger signal
							FALSE - Disable triggering, continuous acquiring spectra
				
Return value:   TRUE:  Operation completed, 
				FALSE: Operation failed

Requirement:	1. FBGA devices have been initialized successfully
				2. FBGA device is not in countinuous data acquiring mode
				3. The option of trigger is enabled if bTrigger = TRUE

Note:			Acquired spectra will be streamed out from the device and stored in DLL internal memory
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/								
DLL_PORT_TYPE BOOL DLL_PORT_FUNCTION_TYPE DLL_Start_Continuously_Acquire_Spectra(BOOL bTrigger);
																		  
/*function
********************************************************************************
<PRE>
Name:			DLL_Stop_Continuously_Acquire_Spectra

Operation:		Stop continuous or triggering acquiring spectra

Parameter:		None
		
Return value:   None

Requirement:	None
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/								
DLL_PORT_TYPE void DLL_PORT_FUNCTION_TYPE DLL_Stop_Continuously_Acquire_Spectra();  


/*function
********************************************************************************
<PRE>
Name:			DLL_Get_TotalSpectrumCount

Operation:		Get total acquired spectrum count in continuous or triggering acquisition

Parameter:		None

Return value:   The total auquired spectrum count
				
Requirement:	None

Note:			The internal memory is ring buffer. It can keep 131072 (512  pixel sensor) or 262144 (256 pixel sensor) spectrum line's raw data
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/								
DLL_PORT_TYPE DWORD DLL_PORT_FUNCTION_TYPE DLL_Get_TotalSpectrumCount(); 

/*function
********************************************************************************
<PRE>
Name:			DLL_Get_SpectrumInformation

Operation:		Get the indexed spectrum information in continuous acquisition or triggering mode

Parameter:		dwSpectrumIndex:			Zero based spectrum index in internal memory
				wProcessCode:				Set bit 0 (0x0001), search peak with uniform threshold
											Set bit 1 (0x0002), search peak with profiled threshold
											Set bit 2 (0x0004), subtract background
											Set bit 5 (0x0020), enable spectrum with ripple correction, it will slow dwon data process 
				pdblBackgroundPower:		Pointer of double backround array buffer. Buffer size >= pixel count
				dblThresholdUniform:		Uniform threshold for peak search. All peaks should meet two requirements:
											a. The peak power  > dblThresholdUniform 
											b. The peak power increase of adjacent pixels at the rising edge > 0.1 * dblThresholdUniform
				pdblThresholdProfile:		Pointer of double profiled threshold array buffer for peak search. If set search peak with profiled 
											threshold bit, Buffer size >= pixel count. All peaks should meet two requirements
											a. The peak power  > pdblThresholdProfile[this peak pixel]
											b. The peak power increase of adjacent pixels at the rising edge > 0.1 * pdblThresholdProfile[this peak pixel]
				pdwSpectrumLineStamp:		Pointer of DWORD spectrum line stamp generated by the device
				pdblTemperature:			Pointer of double device temperature
				pdblWavelength:				Pointer of double wavelength array buffer. Buffer size >= pixel count
				pdblPower:					Pointer to double spectrum power array buffer. Buffer size >= pixel count													
				pwPeakCount:				Pointer of WORD searched peak count
				pdblPeakWavelength:			Pointer of double searched peak wavelength array buffer. Buffer size >= pixel count
				pdblPeakPower:				Pointer of double searched peak power array buffer. Buffer size >= pixel count
				pdblPeakFWHM:				Pointer of double searched peak FWHM array buffer. Buffer size >= pixel count,

Return value:   TRUE:  Operation completed, 
				FALSE: Operation failed

Requirement:	The dwSpectrumIndex < The total auquired spectrum count

Note:			a. Priority of uniform threshold > profiled threshold
				b. The spectrum line stamp is 20 bits counter, its maximum value = 1048576 
--------------------------------------------------------------------------------
</PRE>
*******************************************************************************/								
DLL_PORT_TYPE BOOL DLL_PORT_FUNCTION_TYPE DLL_Get_SpectrumInformation(DWORD		dwSpectrumIndex,
																	  WORD		wProcessCode,
																	  double*	pdblBackgroundPower,
																	  double	dblThresholdUniform,
																	  double*	pdblThresholdProfile,
																	  DWORD*	pdwSpectrumLineStamp,
																	  double*	pdblTemperature,
																	  double*	pdblWavelength,
																	  double*	pdblPower,
																	  WORD*		pwPeakCount,
																	  double*	pdblPeakWavelength,
																	  double*	pdblPeakPower,
																	  double*	pdblPeakFWHM); 
                  

#endif //_USB20_IR_DEV_OP_ATTACH_DLL_