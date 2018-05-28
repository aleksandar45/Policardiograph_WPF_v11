using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Policardiograph_App.DeviceModel.Modules;

namespace Policardiograph_App.DeviceModel.Services
{
    public class TCPService
    {
        TcpListener serverSocket;
        Device device;
        Thread tcpThread;   
        string TAG = "DeviceModel/Services/TCPService/";


        private readonly Action<TcpClient> _action;

        public TCPService(Device device)
        {
            this.device = device;                      
            tcpThread = new Thread(() => Listen());
        }
        public void Start() {
            tcpThread.Start();
        }
        public void Stop()
        {
            serverSocket.Stop();
            tcpThread.Abort();
        }
        public void Listen(){ 

            string serverIPaddress = "192.168.88.10";
            string micIPaddress = "192.168.88.11";
            string ecgIPaddress = "192.168.88.12";
            string accIPaddress = "192.168.88.13";

            
            while(true){                

                TcpClient clSock = default(TcpClient);                
                IPEndPoint endPoint;
                IPAddress srvIPaddress = IPAddress.Parse(serverIPaddress);


                try
                {
                    Thread.Sleep(1000);
                    serverSocket = new TcpListener(srvIPaddress, 11000);
                    serverSocket.Server.NoDelay = true;
                    serverSocket.Start();

                    while (true)
                    {
                        clSock = serverSocket.AcceptTcpClient();
                        endPoint = clSock.Client.RemoteEndPoint as IPEndPoint;
                        if (String.Compare(micIPaddress, endPoint.Address.ToString()) == 0)
                        {
                            //clSock.NoDelay = false;
                            clSock.SendBufferSize = 100;
                            device.micModule = new MICModule(clSock, device.micRingBuffer);
                        }
                        if (String.Compare(ecgIPaddress, endPoint.Address.ToString()) == 0)
                        {
                            clSock.SendBufferSize = 100;
                            device.ecgModule = new ECGModule(clSock, device.ecgRingBuffer);
                        }
                        if (String.Compare(accIPaddress, endPoint.Address.ToString()) == 0)
                        {
                            clSock.SendBufferSize = 100;
                            device.acc_ppgModule = new ACC_PPGModule(clSock, device.acc_ppgRingBuffer);
                        }

                    }
                }
                catch (SocketException e){
                    Log log = new Log();
                    log.LogMessageToFile(TAG + "Listen:" + e.Message);

                }
                catch (Exception ex) {
                    Log log = new Log();
                    log.LogMessageToFile(TAG + "Listen:" + ex.Message);
                }
                
            }
        }
    }
}
