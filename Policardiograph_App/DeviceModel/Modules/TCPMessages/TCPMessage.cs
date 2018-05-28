using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.DeviceModel.Modules.TCPMessages
{
    public abstract class TCPMessage
    {
        public string Message{
            get;
            set;
        }
        public TCPMessage(string message) {
            Message = message;
        }
    }
}
