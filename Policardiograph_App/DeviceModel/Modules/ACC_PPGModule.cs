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
    public class ACC_PPGModule: TCPModule
    {
        public ACC_PPGModule(TcpClient clientSocket, RingBufferByte ringBuffer)
            : base(clientSocket, ringBuffer,"ACC_PPG.dat")
        {            
        }
        public void startSyncPlaying()
        {
            base.sendMessage(new StartFullAcqMICMessage());
        }
        public void sendSetting(SettingACC accSetting, SettingPPG ppgSetting)
        {
            base.sendMessage(new SendSettingACC_PPGMessage(accSetting, ppgSetting));
        }
    }
}
