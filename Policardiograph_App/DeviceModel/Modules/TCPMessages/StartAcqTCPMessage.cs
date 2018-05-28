using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.DeviceModel.Modules.TCPMessages
{
    public class StartAcqTCPMessage: TCPMessage
    {
        public StartAcqTCPMessage()
            : base("|HEAD|START_ACQ_PART00000000|END|xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_") { }

    }
}
