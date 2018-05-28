using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Diagnostics;
using Policardiograph_App.ViewModel;

namespace Policardiograph_App.ViewModel.OpenGLRender
{
    public class OpenGLDispatcher
    {
        public const int MIC_BUFFER_LENGTH = 4000;
        public const int FBGA_BUFFER_LENGTH = 512;
        public const int ECG_BUFFER_LENGTH = 2000;        
        public const int ACC_BUFFER_LENGTH = 2000;
        public const int PPG_BUFFER_LENGTH = 400;        

        OpenGlDisplay openGlDisplay1;
        OpenGlDisplay openGlDisplay2;
        OpenGlDisplay openGlDisplay3;
        OpenGlDisplay openGlDisplay4;
        OpenGlDisplay openGlDisplay5;
        OpenGlDisplay openGlDisplay6;                      

        public List<IntArray> micChannels;                
        private int _micIndex = 0;
        public int micIndex {
            get {
                return _micIndex;
            }
            set {
                _micIndex = value;
                if(micChannels != null){
                    if (_micIndex >= micChannels.ElementAt(0).size)
                        _micIndex = 0;
                    for (int i = 0; i < micChannels.Count; i++)
                    {
                        micChannels.ElementAt(i).index = _micIndex;
                    }
                }
            }
        }

        public List<IntArray> ecgChannels;        
        private int _ecgIndex = 0;
        public int ecgIndex {
            get {
                return _ecgIndex;
            }
            set {
                _ecgIndex = value;
                if (ecgChannels != null)
                {
                    if (_ecgIndex >= ecgChannels.ElementAt(0).size)
                        _ecgIndex = 0;
                    for (int i = 0; i < ecgChannels.Count; i++)
                    {
                        ecgChannels.ElementAt(i).index = _ecgIndex;
                    }
                }
            }
        }

        public List<IntArray> accChannels;
        private int _accIndex = 0;
        public int accIndex {
            get { 
                return _accIndex;
            }
            set{
                _accIndex = value;
                if (accChannels != null) {
                    if (_accIndex >= accChannels.ElementAt(0).size)
                        _accIndex = 0;
                    for (int i = 0; i < accChannels.Count; i++)
                    {
                        accChannels.ElementAt(i).index = _accIndex;
                    }
                }
            }
        }

        public List<IntArray> ppgChannels;
        private int _ppgIndex = 0;
        public int ppgIndex {
            get
            {
                return _ppgIndex;
            }
            set
            {
                _ppgIndex = value;
                if (ppgChannels != null)
                {
                    if (_ppgIndex >= ppgChannels.ElementAt(0).size)
                        _ppgIndex = 0;
                    for (int i = 0; i < ppgChannels.Count; i++)
                    {
                        ppgChannels.ElementAt(i).index = _ppgIndex;
                    }
                }
            }
        }
        
        public List<IntArray> fbgaChannels;        

      

        public ProfileEnumType Profile
        {
            get {
                return Profile;
            }
            set
            {
                micIndex = 0;
                ecgIndex = 0;
                accIndex = 0;
                ppgIndex = 0;
            }
        }

        
        public OpenGLDispatcher(OpenGlDisplay display1,OpenGlDisplay display2,OpenGlDisplay display3,OpenGlDisplay display4,OpenGlDisplay display5,OpenGlDisplay display6,int micNoCh, int fbgaNoCh, int ecgNoCh, int accNoCh, int ppgNoCh)
        {                         
            Profile = ProfileEnumType.ALL_DEVICES;
            openGlDisplay1 = display1;
            openGlDisplay2 = display2;
            openGlDisplay3 = display3;
            openGlDisplay4 = display4;
            openGlDisplay5 = display5;
            openGlDisplay6 = display6;            
            micChannels = new List<IntArray>(micNoCh);
            for (int i = 0; i < micNoCh; i++)
                micChannels.Add(new IntArray(MIC_BUFFER_LENGTH));

            fbgaChannels = new List<IntArray>(fbgaNoCh);
            for (int i = 0; i < fbgaNoCh; i++)
                fbgaChannels.Add(new IntArray(FBGA_BUFFER_LENGTH));
            
            ecgChannels = new List<IntArray>(ecgNoCh);
            for (int i = 0; i<ecgNoCh;i++)
                ecgChannels.Add(new IntArray(ECG_BUFFER_LENGTH));

            accChannels = new List<IntArray>(accNoCh);
            for (int i = 0; i < accNoCh; i++)
                accChannels.Add(new IntArray(ACC_BUFFER_LENGTH));

            ppgChannels = new List<IntArray>(ppgNoCh);
            for (int i = 0; i < ppgNoCh; i++)
                ppgChannels.Add(new IntArray(PPG_BUFFER_LENGTH));


            
        }        
        public void renderDisplays(){

           /* for (int i = 0; i < 200; i++) {
                ppgChannels.ElementAt(1).intArray[ppgIndex++] = i;
            }
            ppgChannels.ElementAt(1).index = ppgIndex;
            if (ppgIndex >= ppgChannels.ElementAt(1).size)
            {
                ppgIndex = 0;
                ppgChannels.ElementAt(1).index = ppgIndex;                    
            }*/
            
            openGlDisplay1.render();
            openGlDisplay2.render();
            openGlDisplay3.render();
            openGlDisplay4.render();
            openGlDisplay5.render();
            openGlDisplay6.render();            

            /*stopWatch.Stop();           
            long ts = stopWatch.ElapsedMilliseconds;
            stopWatch.Reset();
            stopWatch.Start();
            stopWatchArray[stopWatchCounter++] = ts;*/

        }                      
        public void disableDisplay(int i)
        {
            switch (i) {
                case 1: openGlDisplay1.Visible = false;
                    break;
                case 2: openGlDisplay2.Visible = false;
                    break;
                case 3: openGlDisplay3.Visible = false;
                    break;
                case 4: openGlDisplay4.Visible = false;
                    break;
                case 5: openGlDisplay5.Visible = false;
                    break;
                case 6: openGlDisplay6.Visible = false;
                    break;
            }            
        }
        public void enableDisplay(int i)
        {
            switch (i)
            {
                case 1: openGlDisplay1.Visible = true;
                    break;
                case 2: openGlDisplay2.Visible = true;
                    break;
                case 3: openGlDisplay3.Visible = true;
                    break;
                case 4: openGlDisplay4.Visible = true;
                    break;
                case 5: openGlDisplay5.Visible = true;
                    break;
                case 6: openGlDisplay6.Visible = true;
                    break;
            }            
        }       
        public bool linkDisplay(int displayNum, string type, int ch_no)
        {
            OpenGlDisplay tempDisplay;
            switch (displayNum) {
                case 1: tempDisplay = openGlDisplay1;
                    break;
                case 2: tempDisplay = openGlDisplay2;
                    break;
                case 3: tempDisplay = openGlDisplay3;
                    break;
                case 4: tempDisplay = openGlDisplay4;
                    break;
                case 5: tempDisplay = openGlDisplay5;
                    break;
                case 6: tempDisplay = openGlDisplay6;
                    break;
                default:tempDisplay = openGlDisplay1;
                    break;
            }
            if (String.Compare(type, "FBGA") == 0) {
                if (tempDisplay != null)
                {
                    tempDisplay.linkArray(fbgaChannels.ElementAt(ch_no-1));
                    tempDisplay.FBGAMode = true;
                    return true;
                }
                else return false; 
            }
            if (String.Compare(type, "MIC") == 0) {
                if (tempDisplay != null) {
                    tempDisplay.linkArray(micChannels.ElementAt(ch_no-1));
                    tempDisplay.FBGAMode = false;
                }                
            }
            if (String.Compare(type, "ECG") == 0)
            {
                if (tempDisplay != null)
                {
                    tempDisplay.linkArray(ecgChannels.ElementAt(ch_no - 1));
                    tempDisplay.FBGAMode = false;
                }                  
            }
            if (String.Compare(type, "ACC") == 0)
            {
                if (tempDisplay != null)
                {
                    tempDisplay.linkArray(accChannels.ElementAt(ch_no - 1));
                    tempDisplay.FBGAMode = false;
                }                   
            }
            if (String.Compare(type, "PPG") == 0)
            {
                if (tempDisplay != null)
                {
                    tempDisplay.linkArray(ppgChannels.ElementAt(ch_no - 1));
                    tempDisplay.FBGAMode = false;
                }   
            }
            return false;
        }
        public bool linkDisplay(int displayNum, string type, int ch_no, string axis)
        {
            OpenGlDisplay tempDisplay;
            switch (displayNum)
            {
                case 1: tempDisplay = openGlDisplay1;
                    break;
                case 2: tempDisplay = openGlDisplay2;
                    break;
                case 3: tempDisplay = openGlDisplay3;
                    break;
                case 4: tempDisplay = openGlDisplay4;
                    break;
                case 5: tempDisplay = openGlDisplay5;
                    break;
                case 6: tempDisplay = openGlDisplay6;
                    break;
                default: tempDisplay = openGlDisplay1;
                    break;
            }
            if (String.Compare(type, "FBGA") == 0)
            {
                if (tempDisplay != null)
                {
                    tempDisplay.linkArray(fbgaChannels.ElementAt(ch_no - 1));
                    return true;
                }
                else return false;
            }
            if (String.Compare(type, "MIC") == 0)
            {
                if (tempDisplay != null)
                {
                    tempDisplay.linkArray(micChannels.ElementAt(ch_no - 1));
                }
            }
            if (String.Compare(type, "ECG") == 0)
            {
                if (tempDisplay != null)
                {
                    tempDisplay.linkArray(ecgChannels.ElementAt(ch_no - 1));
                }
            }
            if (String.Compare(type, "ACC") == 0)
            {
                if (tempDisplay != null)
                {
                    if (String.Compare(axis, "x") == 0)
                        tempDisplay.linkArray(accChannels.ElementAt((ch_no - 1) * 3));
                    if (String.Compare(axis, "y") == 0)
                        tempDisplay.linkArray(accChannels.ElementAt((ch_no - 1) * 3 + 1));
                    if (String.Compare(axis, "z") == 0)
                        tempDisplay.linkArray(accChannels.ElementAt((ch_no - 1) * 3 + 2));
                }
            }
            if (String.Compare(type, "PPG") == 0)
            {
                if (tempDisplay != null)
                {
                    tempDisplay.linkArray(ppgChannels.ElementAt(ch_no - 1));
                }
            }
            return false;
            
        }
        public void dispose() {
            
        }
    }
}
