using System;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SksChat.Lib.Log;

namespace SksChat.Lib
{
    public class SksClient
    {
        private const string LogTag = "CLIENT";

        public SksServer.MessageReceivedEventHandler MessageReceived;

        public string Username { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public bool Connected { get; set; }

        private TcpClient tcpClient;

        public SksClient(string username, string ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
        }

        public bool Connect()
        {
            Connected = InitTcpClient();

            return Connected;
        }

        public void Disconnect()
        {
            tcpClient?.Close();

            Connected = false;
        }

        private bool InitTcpClient()
        {
            try
            {
                tcpClient = new TcpClient(IpAddress, Port);

                Logger.Log(LogTag, $"connected => {tcpClient.Connected}");

                Task.Factory.StartNew(() => ReceiveMessagesLoop(tcpClient));

                return true;
            }
            catch (Exception e)
            {
                Logger.Log(LogTag, $"connection error {e.Message}");
            }

            return false;
        }

        private void ReceiveMessagesLoop(TcpClient client)
        {
            while (true)
            {
                try
                {
                    var readBuffer = new byte[50];
                    client.GetStream().Read(readBuffer, 0, 50);
                    var readString = Encoding.ASCII.GetString(readBuffer).Replace("\0", "");

                    InvokeMessageReceivedEvent(client, readString);

                    Logger.Log(LogTag, $"Read text => {readString}");
                }
                catch (IOException)
                {
                    // ignore
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        private void InvokeMessageReceivedEvent(TcpClient client, string message)
        {
            var args = new object[] { this, new SksMessageReceivedEventArgs { Message = message } };

            if (MessageReceived == null)
                return;

            foreach (var d in MessageReceived.GetInvocationList())
            {
                var syncer = d.Target as ISynchronizeInvoke;
                if (syncer == null)
                {
                    d.DynamicInvoke(args);
                }
                else
                {
                    syncer.BeginInvoke(d, args);
                }
            }
        }

        public void SendMessage(string message)
        {
            if (!tcpClient.Connected)
                // todo: custom exception
                throw new Exception("Client not connected");

            var data = Encoding.ASCII.GetBytes(message);
            tcpClient.GetStream().Write(data, 0, data.Length);
        }
    }
}
