using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Policardiograph_App.DeviceModel.RingBuffers;
using Policardiograph_App.DeviceModel.Modules.TCPMessages;
using Policardiograph_App.Settings;

namespace Policardiograph_App.DeviceModel.Modules
{
    public class ECGModule: TCPModule
    {
        public ECGModule(TcpClient clientSocket, RingBufferByte ringBuffer)
            : base(clientSocket, ringBuffer,"ECG.dat")
        {            
        }
        public void startSyncPlaying()
        {
            base.sendMessage(new StartFullAcqMICMessage());
        }
        public void sendSetting(SettingECG ecgSetting)
        {
            base.sendMessage(new SendSettingECGMessage(ecgSetting));
        }
       
    }
}
