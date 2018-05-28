using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Policardiograph_App.Dialogs.DialogService;
using Policardiograph_App.Settings;

namespace Policardiograph_App.Dialogs.DialogSetting
{
    public class DialogResultSetting: DialogResult
    {
        public DialogResultSetting(SettingWindow settingWindow,SettingFBGA settingFBGA, SettingMIC settingMIC, SettingECG settingECG) {
            SettingWindow = settingWindow;
            SettingFBGA = settingFBGA;
            SettingMIC = settingMIC;
            SettingECG = settingECG;
        }

        public SettingWindow SettingWindow
        {
            get;
            private set;
        }
        public SettingMIC SettingMIC
        {
            get;
            private set;
        }
        public SettingFBGA SettingFBGA
        {
            get;
            private set;
        }
        public SettingECG SettingECG
        {
            get;
            private set;
        }
        public SettingACC SettingACC
        {
            get;
            private set;
        }
        public SettingPPG SettingPPG
        {
            get;
            private set;
        }

    }
}
