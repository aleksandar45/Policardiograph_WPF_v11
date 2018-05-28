using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;
using System.IO;
using Policardiograph_App.DeviceModel.RingBuffers;
using Policardiograph_App.DeviceModel.Modules.TCPMessages;

namespace Policardiograph_App.DeviceModel.Modules
{
    public class TCPModule: Module
    {
        Thread thread;
        bool saveToFile = false;
        string fileName;
        string path;
        string TAG = "DeviceModel/TCPModule/";

        protected TcpClient clientSocket;
        protected RingBufferByte ringBuffer;
        protected FileStream binaryWriter;
        
        public TCPModule(TcpClient clientSocket, RingBufferByte ringBuffer, string fileName)
        {
            this.clientSocket = clientSocket;
            this.ringBuffer = ringBuffer;
            this.fileName = fileName;
            path = System.IO.Directory.GetCurrentDirectory();
            if (!path.EndsWith("\\")) path += "\\";

            thread = new Thread(doProcessing);
            thread.IsBackground = true;
            thread.Start();

        }
        public virtual void sendMessage(TCPMessage tcpMessage) {
            BinaryWriter writer = new BinaryWriter(clientSocket.GetStream());
            writer.Write(tcpMessage.Message.ToCharArray());
        }
        public virtual bool isConnected()
        {
            // This is how you can determine whether a socket is still connected.
            //Try with keep alive, all other methods have not been working

            bool part1 = clientSocket.Client.Poll(1000, SelectMode.SelectRead);
            bool part2 = (clientSocket.Client.Available == 0);
            if ((part1 && part2) || !clientSocket.Connected)
                return false;
            else
                return true;

        }
        public virtual void startPlaying() {
            sendMessage(new StartAcqTCPMessage());
        }
        public virtual void stopPlaying()
        {
            sendMessage(new StopAcqTCPMessage());
        }
        public virtual void startRecording()
        {
            try
            {
                if (File.Exists(path + fileName))
                {
                    File.Delete(path + fileName);
                }
                binaryWriter = File.OpenWrite(path + fileName);
                this.saveToFile = true;
            }
            catch (DirectoryNotFoundException ex)
            {
                Log log = new Log();
                log.LogMessageToFile(TAG + "startRecording:" + ex.Message);
                throw ex;  //The specified path is invalid (for example, it is on an unmapped drive).
            }
            catch (FileNotFoundException ex)
            {
                Log log = new Log();
                log.LogMessageToFile(TAG + "startRecording:" + ex.Message);
                throw ex;
            }
            catch (IOException ex)
            {
                Log log = new Log();
                log.LogMessageToFile(TAG + "startRecording:" + ex.Message);
                throw ex;  //specified file already in use
            }
            catch (UnauthorizedAccessException ex)
            {
                Log log = new Log();
                log.LogMessageToFile(TAG + "startRecording:" + ex.Message);
                throw ex;  //The caller does not have the required permission.
            }
            catch (Exception ex) {
                Log log = new Log();
                log.LogMessageToFile(TAG + "startRecording:" + ex.Message);
                throw ex;
            }
           
        }
        public virtual void stopRecording()
        {
            try
            {
                saveToFile = false;
                if (binaryWriter != null)
                {
                    if (binaryWriter.CanWrite)
                        binaryWriter.Flush();
                    if (binaryWriter != null)
                        binaryWriter.Close();
                }
            }
            catch (ObjectDisposedException ex) {
                Log log = new Log();
                log.LogMessageToFile(TAG + "stopRecording:" + ex.Message);
                throw ex; //The stream is closed
            }
            catch (IOException ex)
            {
                Log log = new Log();
                log.LogMessageToFile(TAG + "stopRecording:" + ex.Message);
                throw ex; //An I/O error occurred.
            }
        }
        public void dispose() {
            try
            {
                if (binaryWriter != null) binaryWriter.Close();
                thread.Abort();
            }
            catch(Exception ex) {
                Log log = new Log();
                log.LogMessageToFile(TAG + "dispose:" + ex.Message);
            }
        }
        private void doProcessing() { 
            
            byte[] bytesFrom = new byte[10025];
            byte[] bytesTime = new byte[4];            
                     
            NetworkStream networkStream;
            clientSocket.ReceiveBufferSize = 32768;
            int numberOfBytesRead = 0;

            #region debug_variables
      

            
            int[] wptr_values = new int[1500];
            int debug_index = 0;
            long[] stopwatch_values = new long[1500];
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            #endregion
           
           try
            {
                while (true)
                {
                    
                    networkStream = clientSocket.GetStream();
                    if (!networkStream.DataAvailable) Thread.Sleep(200);
                    else
                    {

                        numberOfBytesRead = networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                        stopwatch_values[debug_index] = stopwatch.ElapsedMilliseconds;
                        wptr_values[debug_index++] = numberOfBytesRead;

                        if ((debug_index == 1500) || (numberOfBytesRead > 453))
                        {
                            debug_index++;
                            debug_index--;
                            if (debug_index == 1500)
                                debug_index = 0;
                        }

                        if (ringBuffer.WriteSpace() > numberOfBytesRead)
                        {
                            ringBuffer.Write(bytesFrom, numberOfBytesRead);
                        }                        

                        if (saveToFile)
                        {
                            binaryWriter.Write(bytesFrom, 0, numberOfBytesRead);                            
                        
                        }
                        Thread.Sleep(20);
                    }
                    

               
                }                               
              
                
            }
            finally
            {
                if(binaryWriter!= null) binaryWriter.Close();

            }
            
            
           
        
        }
    }
}
