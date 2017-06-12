using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SksChat.Lib.Log;

namespace SksChat.Lib
{
    public class SksServer
    {
        private const string LogTag = "SERVER";
        private const string LocalhostIp = "127.0.0.1";

        private Action<object, SksMessageReceivedEventArgs> callback;

        public int Port { get; set; }
        public bool Connected { get; set; }

        private TcpListener tcpListener;

        public SksServer(int port)
        {
            Port = port;
        }

        public void Connect(Action<object, SksMessageReceivedEventArgs> callback)
        {
            if (Connected)
                return;

            this.callback = callback;

            if (tcpListener == null)
                InitTcpListener();
            
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

            var sksClient = new SksClient(client);

            var user = Lib.GetUserByIpAndPort(sksClient.IpAddress, sksClient.Port);
            if (user.Client == null)
                user.Client = sksClient;

            user.Client.ChatMessageReceived += (source, args) => callback(source, args);
            user.Client.ReceiveMessagesLoop();
        }
    }
}

