using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.Settings
{
    public class SettingFBGA: SettingBase
    {
        public SettingFBGA(int noOfChannels,int integrationTime, double sLEDPower,bool highDynamicRange, List<ModuleChannel> selectedDisplays) {
            SelectedDisplays = selectedDisplays;
            NumberOfChannels = noOfChannels;
            IntegrationTime = integrationTime;
            SLEDPower = sLEDPower;
            HighDynamicRange = highDynamicRange;
        }
        public int NumberOfChannels
        {
            get;
            private set;
        }
        public List<ModuleChannel> SelectedDisplays
        {
            get;
            private set;
        }
        public int IntegrationTime
        {
            get;
            private set;
        }
        public double SLEDPower
        {
            get;
            private set;
        }
        public bool HighDynamicRange
        {
            get;
            private set;
        }
    }
}
