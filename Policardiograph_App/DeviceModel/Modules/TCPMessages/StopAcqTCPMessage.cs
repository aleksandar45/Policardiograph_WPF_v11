using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.DeviceModel.Modules.TCPMessages
{
    public class StopAcqTCPMessage: TCPMessage
    {
        public StopAcqTCPMessage() : base("|HEAD|STOP_ACQxxxxxx00000000|END|xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_") { }
    }
}
