using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.Settings
{
    public class SettingECG: SettingBase
    {
        public SettingECG(int noOfChannels, bool aVLeads,int gain, string ch4Mode, byte sensP, byte sensN, List<ModuleChannel> selectedDisplays) {
            SelectedDisplays = selectedDisplays;
            AVLeads = aVLeads;
            Gain = gain;
            CH4Mode = ch4Mode;
            SensP = sensP;
            SensN = sensN;
            /*if(AVLeads){
                if(noOfChannels <=9) NumberOfChannels = noOfChannels + 3;
                else if(noOfChannels>12) NumberOfChannels = 12;
                else NumberOfChannels = noOfChannels;
            }            
            else 
                NumberOfChannels = noOfChannels;*/
            NumberOfChannels = noOfChannels;
            
        }
        public int NumberOfChannels
        {
            get;
            private set;
        }
        public bool AVLeads
        {
            get;
            private set;
        }
        public List<ModuleChannel> SelectedDisplays
        {
            get;
            private set;
        }
        public int Gain
        {
            get;
            set;
        }
        public string CH4Mode
        {
            get;
            set;
        }
        public byte SensP
        {
            get;
            set;
        }
        public byte SensN
        {
            get;
            set;
        }
    }
}
