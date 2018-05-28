using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Policardiograph_App.Settings;

namespace Policardiograph_App.DeviceModel.Modules.TCPMessages
{
    public class SendSettingECGMessage: ECGMessage
    {
        private static char[] byte2ascii = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        public SendSettingECGMessage(SettingECG ecgSetting): base(parse(ecgSetting)) { 

        }
        private static string parse(SettingECG ecgSetting) {
            byte temp_byte = 0;
            string message = "|HEAD|SET_PARAMETERS";
            string dummyMessage = "0000|END|xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_";
            StringBuilder sb = new StringBuilder(2);
            temp_byte = (byte)ecgSetting.Gain;
            sb.Append(byte2ascii[temp_byte & 0x0F]);
            if (String.Compare(ecgSetting.CH4Mode, "NORMAL") == 0) temp_byte = 0;
            else if (String.Compare(ecgSetting.CH4Mode, "RLD_IN") == 0) temp_byte = 2;
            else if (String.Compare(ecgSetting.CH4Mode, "MVDD") == 0) temp_byte = 3;
            else if (String.Compare(ecgSetting.CH4Mode, "TEST") == 0) temp_byte = 5;           
            sb.Append(byte2ascii[temp_byte & 0x0F]);
            temp_byte = (byte)ecgSetting.SensP;
            sb.Append(byte2ascii[temp_byte & 0x0F]);
            temp_byte = (byte)ecgSetting.SensN;
            sb.Append(byte2ascii[temp_byte & 0x0F]);
           /* if (micSetting.MuteMIC1) temp_byte |= 0x01;
            if (micSetting.MuteMIC2) temp_byte |= 0x02;
            if (micSetting.MuteMIC3) temp_byte |= 0x04;
            if (micSetting.MuteMIC4) temp_byte |= 0x08;
            if (micSetting.HighPassFilter) temp_byte |= 0x10;*/
           

            message = message + sb;
            message = message + dummyMessage;

            return message;
        }
    }
}
