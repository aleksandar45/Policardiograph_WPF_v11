using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.DeviceModel.Modules.TCPMessages
{
    public class IdleTCPMessage: TCPMessage
    {
        public IdleTCPMessage()
            : base("|HEAD|IDLE_MESSAGExx00000000|END|xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_")
        { 
        }
    }
}
