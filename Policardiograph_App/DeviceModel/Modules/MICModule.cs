using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Policardiograph_App.DeviceModel.RingBuffers;
using Policardiograph_App.DeviceModel.Modules.TCPMessages;
using Policardiograph_App.Settings;

namespace Policardiograph_App.DeviceModel.Modules
{
    public class MICModule: TCPModule
    {
        public MICModule(TcpClient clientSocket,RingBufferByte ringBuffer)
            : base(clientSocket, ringBuffer, "MIC.dat")
        {            
        }

        public void startSyncPlaying()
        {
            base.sendMessage(new StartFullAcqMICMessage());            
        }
        public void startSyncRecording() {
            base.sendMessage(new StartSyncRecMICMessage());
            
        }
        public void sendSetting(SettingMIC micSetting) {
            base.sendMessage(new SendSettingMICMessage(micSetting));

        }
        
   
    }
}
