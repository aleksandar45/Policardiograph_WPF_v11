using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.Settings
{
    public class SettingMIC: SettingBase
    {
        public SettingMIC(int noOfChannels,bool mic1Mute, bool mic2Mute, bool mic3Mute, bool mic4Mute, bool highPassFilter, List<ModuleChannel> selectedDisplays) {
            SelectedDisplays = selectedDisplays;
            NumberOfChannels = noOfChannels;
            MuteMIC1 = mic1Mute;
            MuteMIC2 = mic2Mute;
            MuteMIC3 = mic3Mute;
            MuteMIC4 = mic4Mute;
            HighPassFilter = highPassFilter;
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
        public bool MuteMIC1
        {
            get;
            set;
        }
        public bool MuteMIC2
        {
            get;
            set;
        }
        public bool MuteMIC3
        {
            get;
            set;
        }
        public bool MuteMIC4
        {
            get;
            set;
        }
        public bool HighPassFilter
        {
            get;
            set;
        }
    }
}
