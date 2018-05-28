using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.DeviceModel
{
    public enum TCPDeviceState
    {
        DISCONNECTED, 
        CONNECTED, 
        IDLE, 
        TRANSFERING,         
    }
}
