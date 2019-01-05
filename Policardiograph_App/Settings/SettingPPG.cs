using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.Settings
{
    public class SettingPPG: SettingBase
    {
        public SettingPPG(int noOfChannels, List<ModuleChannel> selectedDisplays)
        {
            SelectedDisplays = selectedDisplays;
            NumberOfChannels = noOfChannels*2;
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
    }
}
