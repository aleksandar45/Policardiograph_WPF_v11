using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.DeviceModel.Modules.TCPMessages
{
    public class ECGMessage: TCPMessage
    {
        public ECGMessage(string message) : base(message) { }
    }
}
