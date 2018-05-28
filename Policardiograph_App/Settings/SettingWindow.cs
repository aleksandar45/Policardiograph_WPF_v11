using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.Settings
{
    public class SettingWindow: SettingBase
    {
        public SettingWindow(int timeAxis, List<int> userTabDisplays, List<ModuleChannel> userTabSelectedDisplays, bool enabled)
        {
            UserTabDisplays = userTabDisplays;
            TimeAxis = timeAxis;
            UserTabSelectedDisplays = userTabSelectedDisplays;
            TabEnabled = enabled;
        }
        public int TimeAxis{
            get; 
            private set;
        }
        public List<int> UserTabDisplays
        {
            get;
            private set;
        }
        public List<ModuleChannel> UserTabSelectedDisplays
        {
            get;
            private set;
        }
        public bool TabEnabled
        {
            get;
            private set;
        }
    }
}
