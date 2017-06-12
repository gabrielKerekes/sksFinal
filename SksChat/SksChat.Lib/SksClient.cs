using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SksChat.Lib.Encodings.Asn1;
using SksChat.Lib.Encodings.Pem;
using SksChat.Lib.Log;

namespace SksChat.Lib
{
    public class SksMessageReceivedEventArgs : EventArgs
    {
        public string FromIp { get; set; }
        public int FromPort { get; set; }
        public string Message { get; set; }
    }
    public class SksClientCreatedEventArgs : EventArgs
    {
        //public SksClient Client { get; set; }
    }

    public enum SksClientType
    {
        Local,
        Remote,
    }

    public class SksClient
    {
        private const string LogTag = "CLIENT";

        public delegate void MessageReceivedEventHandler(object source, SksMessageReceivedEventArgs e);
        public delegate void NewClientCreatedEventHandler(object source, SksClientCreatedEventArgs e);

        public MessageReceivedEventHandler MessageReceived;
        public MessageReceivedEventHandler ChatMessageReceived;
        public static NewClientCreatedEventHandler NewClientCreated;

        private object newClientLock = new object();

        public string IpAddress { get; set; }
        public int Port { get; set; }
        public bool Connected { get; set; }
        public SksClientType Type { get; set; }

        public TcpClient tcpClient { get; set; }

        public SksClient(User user)
        {
            IpAddress = user.IpAddress;
            Port = int.Parse(user.Port);

            Type = SksClientType.Local;
        }

        public SksClient(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            tcpClient.SendTimeout = 1;
            tcpClient.ReceiveTimeout = 1;

            IpAddress = ((IPEndPoint) tcpClient.Client.RemoteEndPoint).Address.ToString();
            Port = ((IPEndPoint) tcpClient.Client.LocalEndPoint).Port;

            Type = SksClientType.Remote;
        }

        public bool Connect()
        {
            Connected = InitTcpClient();

            return Connected;
        }

        public void Disconnect()
        {
            tcpClient?.Close();
            tcpClient = null;

            Connected = false;
        }

        public string GetCompleteLogTag()
        {
            return $"{LogTag} - {IpAddress}:{Port}";
        }

        private bool InitTcpClient()
        {
            try
            {
                tcpClient = new TcpClient(IpAddress, Port);

                Logger.Log(GetCompleteLogTag(), $"connected: {tcpClient.Connected}");

                Task.Factory.StartNew(ReceiveMessagesLoop);

                return true;
            }
            catch (Exception e)
            {
                Logger.Log(GetCompleteLogTag(), $"connection error {e.Message}");
            }

            return false;
        }
        
        public void ReceiveMessagesLoop()
        {
            OnNewClientCreated();

            while (true)
            {
                try
                {
                    if (tcpClient == null)
                        return;
                    
                    var readLength = new byte[4];
                    tcpClient.GetStream().Read(readLength, 0, 4);

                    var messageLength = BitConverter.ToInt32(readLength.Reverse().ToArray(), 0);

                    var readBuffer = new byte[messageLength];
                    tcpClient.GetStream().Read(readBuffer, 0, messageLength);

                    var readString = Encoding.ASCII.GetString(readBuffer);

                    OnMessageReceived(tcpClient, readString);

                    Logger.Log(GetCompleteLogTag(), $"MessageReceived: {readString}");
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

        private void OnNewClientCreated()
        {
            var args = new object[] { this, new SksClientCreatedEventArgs { /*Client = this*/ } };

            lock (newClientLock)
            {
                if (NewClientCreated == null)
                    return;

                foreach (var d in NewClientCreated.GetInvocationList())
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

        private void OnMessageReceived(TcpClient client, string message)
        {
            var args = new object[] { this, new SksMessageReceivedEventArgs { FromIp = IpAddress, FromPort = Port, Message = message } };

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

        public void OnChatMessageReceived(TcpClient client, string message)
        {
            var args = new object[] { this, new SksMessageReceivedEventArgs { FromIp = IpAddress, FromPort = Port, Message = message } };

            if (ChatMessageReceived == null)
                return;

            foreach (var d in ChatMessageReceived.GetInvocationList())
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

            Logger.Log(GetCompleteLogTag(), $"Send message: {message}");

            var data = Encoding.ASCII.GetBytes(message);

            SendMessage(data);
        }

        public void SendMessage(byte[] messageBytes)
        {
            if (!tcpClient.Connected)
                // todo: custom exception
                throw new Exception("Client not connected");

            Logger.Log(GetCompleteLogTag(), $"Message data: {string.Join(",", messageBytes)}");

            var dataLengthBytes = BitConverter.GetBytes(messageBytes.Length).Reverse().ToArray();

            var dataWithLength = dataLengthBytes.Concat(messageBytes).ToArray();

            tcpClient.GetStream().Write(dataWithLength, 0, dataWithLength.Length);
        }
    }
}
