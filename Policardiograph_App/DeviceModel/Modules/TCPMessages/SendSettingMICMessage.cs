using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Policardiograph_App.Settings;

namespace Policardiograph_App.DeviceModel.Modules.TCPMessages
{
    public class SendSettingMICMessage: MICMessage
    {
        private static char[] byte2ascii= new char[] {'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'};
        public SendSettingMICMessage(SettingMIC micSetting)
            : base(parse(micSetting))
        { 

        }
        private static string parse(SettingMIC micSetting){
            byte temp_byte = 0;
            string message = "|HEAD|SET_PARAMETERS";
            string dummyMessage = "000000|END|xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_";
            if (micSetting.MuteMIC1) temp_byte |= 0x01;
            if (micSetting.MuteMIC2) temp_byte |= 0x02;
            if (micSetting.MuteMIC3) temp_byte |= 0x04;
            if (micSetting.MuteMIC4) temp_byte |= 0x08;
            if (micSetting.HighPassFilter) temp_byte |= 0x10;
            StringBuilder sb = new StringBuilder(2);
            sb.Append(byte2ascii[(temp_byte >> 4) & 0x0F]);
            sb.Append(byte2ascii[temp_byte & 0x0F]);
            
            message = message + sb;
            message = message + dummyMessage;

            return message;
        }
    }
}
