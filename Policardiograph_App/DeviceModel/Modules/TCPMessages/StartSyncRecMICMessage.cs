using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.DeviceModel.Modules.TCPMessages
{
    public class StartSyncRecMICMessage: MICMessage
    {
        public StartSyncRecMICMessage() : base("|HEAD|START_REC_SYNC00000000|END|xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_") { }
    }
}
