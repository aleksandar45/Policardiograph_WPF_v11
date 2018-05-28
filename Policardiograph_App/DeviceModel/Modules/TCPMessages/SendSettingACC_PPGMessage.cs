using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Policardiograph_App.Settings;

namespace Policardiograph_App.DeviceModel.Modules.TCPMessages
{
    public class SendSettingACC_PPGMessage: ACC_PPGMessage
    {
        public SendSettingACC_PPGMessage(SettingACC accSetting, SettingPPG ppgSetting): base(parse(accSetting,ppgSetting)) { 

        }
        private static string parse(SettingACC accSetting, SettingPPG ppgSetting) {
            return "ACA";
        }
    }
}
