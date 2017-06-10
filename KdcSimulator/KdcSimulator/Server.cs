using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KdcSimulator
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public string FromIp { get; set; }
        public string Message { get; set; }
    }

    public class Server
    {
        private const string LogTag = "SERVER";
        private const string LocalhostIp = "127.0.0.1";

        public delegate void MessageReceivedEventHandler(object source, MessageReceivedEventArgs e);

        public MessageReceivedEventHandler MessageReceived;

        public int Port { get; set; }
        public bool Connected { get; set; }

        private TcpListener tcpListener;

        public Server(int port)
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
            Console.WriteLine(LogTag + ": Client connected");

            var listener = (TcpListener)ar.AsyncState;
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

                    var message = new byte[] { 0x17, 0x2B, 0x10, 0x29, 0x13, 0x0F, 0x31, 0x31, 0x31, 0x2E, 0x31, 0x31, 0x31, 0x2E, 0x31, 0x31, 0x31, 0x2E, 0x31, 0x31, 0x31, 0x02, 0x01, 0x05, 0x13, 0x0e, 0x41, 0x6e, 0x79, 0x62, 0x6f, 0x64, 0x79, 0x20, 0x74, 0x68, 0x65, 0x72, 0x65, 0x3f, 0x04, 0x00, 0x13, 0x01, 65, };
                    client.GetStream().Write(message, 0, message.Length);

                    Console.WriteLine(LogTag + $": Read text => {readString}");
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
            var args = new object[] { this, new MessageReceivedEventArgs() { Message = message } };
            
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
