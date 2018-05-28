using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.DeviceModel.Modules.TCPMessages
{
    public class ACC_PPGMessage: TCPMessage
    {
        public ACC_PPGMessage(string message) : base(message) { }
    }
}
