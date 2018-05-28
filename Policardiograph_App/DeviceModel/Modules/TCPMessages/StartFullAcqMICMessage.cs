using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.DeviceModel.Modules.TCPMessages
{
    public class StartFullAcqMICMessage: MICMessage
    {
        public StartFullAcqMICMessage() : base("|HEAD|START_ACQ_FULL00000000|END|xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_xxxxxxxxx_") { }
    }
}
