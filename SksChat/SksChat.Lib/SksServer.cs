using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SksChat.Lib.Log;

namespace SksChat.Lib
{
    public class SksMessageReceivedEventArgs : EventArgs
    {
        public string FromIp { get; set; }
        public string Message { get; set; }
    }

    public class SksServer
    {
        private const string LogTag = "SERVER";
        private const string LocalhostIp = "127.0.0.1";

        public delegate void MessageReceivedEventHandler(object source, SksMessageReceivedEventArgs e);

        public MessageReceivedEventHandler MessageReceived;

        public int Port { get; set; }
        public bool Connected { get; set; }

        private TcpListener tcpListener;

        public SksServer(int port)
        {
            Port = port;
        }

        public void Connect()
        {
            if (Connected)
                return;

            if (tcpListener == null)
                InitTcpListener();

            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(AcceptConnectionCallback, tcpListener);
        }

        public void Disconnect()
        {
            if (!Connected)
                return; 

            tcpListener.Stop();
        }

        private void InitTcpListener()
        {
            var localAddress = IPAddress.Parse(LocalhostIp);
            tcpListener = new TcpListener(localAddress, Port);

            tcpListener.Start();
        }

        private void AcceptConnectionCallback(IAsyncResult ar)
        {
            Logger.Log(LogTag, "Client connected");

            var listener = (TcpListener) ar.AsyncState;
            var client = listener.EndAcceptTcpClient(ar);
            tcpListener.BeginAcceptTcpClient(AcceptConnectionCallback, tcpListener);
            client.ReceiveTimeout = 500;

            ReceiveMessagesLoop(client);
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
    }
}

