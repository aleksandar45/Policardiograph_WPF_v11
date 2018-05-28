using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.Settings
{
    public class SettingACC: SettingBase
    {
        public SettingACC(int noOfChannels, List<ModuleChannel> selectedDisplays)
        {
            SelectedDisplays = selectedDisplays;
            NumberOfChannels = noOfChannels*3;
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
