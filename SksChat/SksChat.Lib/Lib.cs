using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SksChat.Lib.Encodings.Pem;
using SksChat.Lib.Log;
using SksChat.Lib.Messages;
using SksChat.Lib.Messages.Akep2;
using SksChat.Lib.Messages.Handshake;
using SksChat.Lib.Protocols;
using SksChat.Lib.Security.Encryption;

namespace SksChat.Lib
{
    public enum ProtocolType
    {
        None = 0,
        Akep2,
        LamportScheme,
        OtwayRees,
        Rsa,
    }

    public static class Lib
    {
        private const string LogTag = "LIB";

        public static readonly byte[] KdcKey = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        private static object usersLock = new object();
        public static List<User> Users { get; set; }

        private static object clientsLock = new object();
        public static List<SksClient> Clients { get; set; }


        private static object handshakesLock = new object();
        public static List<Handshake> Handshakes { get; set; }


        private static object lamportSchemesLock = new object();
        public static List<LamportScheme> LamportSchemes { get; set; }


        private static object akep2sLock = new object();
        public static List<Akep2> Akep2s { get; set; }


        private static object otwayReesesLock = new object();
        public static List<OtwayRees> OtwayReeses { get; set; }


        private static object rsasLock = new object();
        public static List<Rsa> Rsas { get; set; }

        public static string MyUsername { get; set; }

        public static void Init(string username, string logFileName)
        {
            MyUsername = username;

            Users = new List<User>();
            Clients = new List<SksClient>();

            Handshakes = new List<Handshake>();
            LamportSchemes = new List<LamportScheme>();
            Akep2s = new List<Akep2>();
            OtwayReeses = new List<OtwayRees>();
            Rsas = new List<Rsa>();

            SksClient.NewClientCreated += SksClient_NewClientCreated;

            Logger.Initialize(logFileName);
        }

        public static void AddUsers(List<User> users)
        {
            foreach (var user in users)
            {
                AddUser(user);
            }
        }

        public static void AddUser(User user)
        {
            lock (usersLock)
            {
                Users.Add(user);
            }
        }

        public static User GetUserByIpAndPort(string ip, int port)
        {
            lock (usersLock)
            {
                return Users.FirstOrDefault(u => u.IpAddress == ip && u.Port == port.ToString());
            }
        }

        public static Akep2 GetAkep2(User user)
        {
            lock (akep2sLock)
            {
                return Akep2s.FirstOrDefault(a => a.User == user);
            }
        }

        public static LamportScheme GetLamportScheme(User user)
        {
            lock (lamportSchemesLock)
            {
                return LamportSchemes.FirstOrDefault(a => a.User == user);
            }
        }

        public static OtwayRees GetOtwayReese(User user)
        {
            lock (otwayReesesLock)
            {
                return OtwayReeses.FirstOrDefault(a => a.User == user);
            }
        }

        public static Rsa GetRsa(User user)
        {
            lock (rsasLock)
            {
                return Rsas.FirstOrDefault(a => a.User == user);
            }
        }

        private static void SksClient_NewClientCreated(object source, SksClientCreatedEventArgs sksClientCreatedEventArgs)
        {
            var client = (SksClient)source;
            client.MessageReceived += Client_MessageReceived;

            var user = GetUserByIpAndPort(client.IpAddress, client.Port);

            lock (clientsLock)
            {
                Clients.Add(client);
            }

            if (client.Type == SksClientType.Local)
            {
                NewLocalClientCreated(user);
            }
            else if (client.Type == SksClientType.Remote)
            {
                NewRemoteClientCreated(user);
            }
        }

        private static void NewLocalClientCreated(User user)
        {
            lock (handshakesLock)
            {
                InitHandshake(user, SksClientType.Local);
            }
        }

        private static void NewRemoteClientCreated(User user)
        {
            // do nothing, wait for message received
        }

        private static void Client_MessageReceived(object source, SksMessageReceivedEventArgs sksMessageReceivedEventArgs)
        {
            var client = (SksClient)source;

            if (client.Type == SksClientType.Local)
            {
                LocalClientMessageReceived(sksMessageReceivedEventArgs);
            }
            else if (client.Type == SksClientType.Remote)
            {
                RemoteClientMessageReceived(sksMessageReceivedEventArgs);
            }
        }

        private static void LocalClientMessageReceived(SksMessageReceivedEventArgs sksMessageReceivedEventArgs)
        {
            HandleMessage(sksMessageReceivedEventArgs);
        }

        private static void RemoteClientMessageReceived(SksMessageReceivedEventArgs sksMessageReceivedEventArgs)
        {
            HandleMessage(sksMessageReceivedEventArgs);
        }

        public static void HandleMessage(SksMessageReceivedEventArgs sksMessageReceivedEventArgs)
        {
            var messageType = PemParser.GetMessageType(sksMessageReceivedEventArgs.Message);

            // todo: doriesit otway reese
            switch (messageType)
            {
                case PemMessageType.LongTermKey:
                    break;
                case PemMessageType.InitialMessage1:
                    break;
                case PemMessageType.InitialMessage2:
                    break;
                case PemMessageType.ErrorMessage:
                    break;
                case PemMessageType.ChatMessage:
                    HandleChatMessage(sksMessageReceivedEventArgs);
                    break;
                case PemMessageType.Akep2Message1:
                    HandleAkep2Message1(sksMessageReceivedEventArgs);
                    break;
                case PemMessageType.Akep2Message2:
                    HandleAkep2Message2(sksMessageReceivedEventArgs);
                    break;
                case PemMessageType.Akep2Message3:
                    HandleAkep2Message3(sksMessageReceivedEventArgs);
                    break;
                case PemMessageType.OtwayReesMessage1:
                    break;
                case PemMessageType.OtwayReesMessage2:
                    //HandleOtwayReeseMessage2(sksMessageReceivedEventArgs);
                    break;
                case PemMessageType.OtwayReesMessage3:
                    break;
                case PemMessageType.OtwayReesMessage4:
                    //HandleOtwayReeseMessage4(sksMessageReceivedEventArgs);
                    break;
                case PemMessageType.HandshakeMessage1:
                    HandleHandshakeMessage1(sksMessageReceivedEventArgs);
                    break;
                case PemMessageType.HandshakeMessage2:
                    HandleHandshakeMessage2(sksMessageReceivedEventArgs);
                    break;
                case PemMessageType.HandshakeMessage3:
                    HandleHandshakeMessage3(sksMessageReceivedEventArgs);
                    break;
                case PemMessageType.HandshakeMessage4:
                    HandleHandshakeMessage4(sksMessageReceivedEventArgs);
                    break;
                case PemMessageType.SshTlsMessage1:
                    break;
                case PemMessageType.SshTlsMessage2:
                    break;
                case PemMessageType.SshTlsMessage3:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private static void HandleChatMessage(SksMessageReceivedEventArgs sksMessageReceivedEventArgs)
        {
            var user = GetUserByIpAndPort(sksMessageReceivedEventArgs.FromIp, sksMessageReceivedEventArgs.FromPort);

            var message = ChatMessage.FromString(sksMessageReceivedEventArgs.Message);

            var secret = user.GetSecretFromProtocol();

            var messageStr = SksAes.DecryptStringFromBytes_Aes(message.EncryptedMessageBytes, secret, message.Iv);

            user.Client.OnChatMessageReceived(user.Client.tcpClient, messageStr);
        }

        public static Handshake InitHandshake(User user, SksClientType clientType)
        {
            Logger.Log(LogTag, $"InitHandshake - User: {user.Name}. ClientType: {clientType}");

            lock (handshakesLock)
            {
                if (Handshakes.Any(h => h.User == user))
                {
                    Logger.Log(LogTag, "ERROR: Handshake already exists");
                    return null;
                }
            }

            var newHandshake = new Handshake(user);
            newHandshake.Start(clientType);

            lock (handshakesLock)
            {
                Handshakes.Add(newHandshake);
            }

            return newHandshake;
        }

        private static void HandleHandshakeMessage1(SksMessageReceivedEventArgs sksMessageReceivedEventArgs)
        {
            var user = GetUserByIpAndPort(sksMessageReceivedEventArgs.FromIp, sksMessageReceivedEventArgs.FromPort);

            Handshake handshake;
            lock (handshakesLock)
            {
                handshake = Handshakes.FirstOrDefault(h => h.User == user);
            }

            if (handshake != null)
            {
                Logger.Log(LogTag, "ERROR: Handshake message 1 - handshake already existed");
                return;
            }

            handshake = InitHandshake(user, SksClientType.Remote);

            if (handshake == null) return;

            var message = HandshakeMessage1.FromString(sksMessageReceivedEventArgs.Message);
            
            user.Protocol = Utils.DetermineProtocol(user);

            if (user.Protocol == ProtocolType.LamportScheme)
                InitLamportScheme(user, SksClientType.Remote);

            var newMessage = new HandshakeMessage2(user.Protocol, null);
            user.SendMessage(newMessage.ToString());

            handshake.IncrementMessageCount();
        }

        private static void HandleHandshakeMessage2(SksMessageReceivedEventArgs sksMessageReceivedEventArgs)
        {
            var user = GetUserByIpAndPort(sksMessageReceivedEventArgs.FromIp, sksMessageReceivedEventArgs.FromPort);

            Handshake handshake;
            lock (handshakesLock)
            {
                handshake = Handshakes.FirstOrDefault(h => h.User == user);
            }

            if (handshake == null)
            {
                Logger.Log(LogTag, "ERROR: Handshake message 2 - handshake doesn't exist");
                return;
            }

            var message = HandshakeMessage2.FromString(sksMessageReceivedEventArgs.Message);

            user.Protocol = message.ProtocolId;

            switch (message.ProtocolId)
            {
                case ProtocolType.None:
                    break;
                case ProtocolType.Akep2:
                    InitAkep2(user, SksClientType.Local);
                    break;
                case ProtocolType.LamportScheme:
                    InitLamportScheme(user, SksClientType.Local);
                    break;
                case ProtocolType.OtwayRees:
                    break;
                case ProtocolType.Rsa:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void HandleHandshakeMessage3(SksMessageReceivedEventArgs sksMessageReceivedEventArgs)
        {
            // todo: ukoncenie komunikacie
        }

        private static void HandleHandshakeMessage4(SksMessageReceivedEventArgs sksMessageReceivedEventArgs)
        {
            // todo: ukoncenie komunikacie
        }

        public static Akep2 InitAkep2(User user, SksClientType clientType)
        {
            lock (akep2sLock)
            {
                if (Akep2s.Any(a => a.User == user))
                    return null;
            }

            var newAkep2 = new Akep2(user);
            newAkep2.Start(clientType);

            lock (akep2sLock)
            {
                Akep2s.Add(newAkep2);
            }

            return newAkep2;
        }

        public static LamportScheme InitLamportScheme(User user, SksClientType clientType)
        {
            lock (lamportSchemesLock)
            {
                if (LamportSchemes.Any(a => a.User == user))
                    return null;
            }

            var newLamportScheme = new LamportScheme(user);

            lock (akep2sLock)
            {
                LamportSchemes.Add(newLamportScheme);
            }

            return newLamportScheme;
        }

        private static void HandleAkep2Message1(SksMessageReceivedEventArgs sksMessageReceivedEventArgs)
        {
            var user = GetUserByIpAndPort(sksMessageReceivedEventArgs.FromIp, sksMessageReceivedEventArgs.FromPort);

            var akep2 = InitAkep2(user, SksClientType.Remote);
            if (akep2 == null)
                return;

            var message = Akep2Message1.FromString(sksMessageReceivedEventArgs.Message);
            akep2.NonceA = message.NonceA;
            akep2.NonceB = Utils.GenerateRandom16();

            var newMessage = new Akep2Message2(MyUsername, user.Name, akep2.NonceA, akep2.NonceB, user.Key);
            user.SendMessage(newMessage.ToString());

            akep2.IncrementMessageCount();
        }

        private static void HandleAkep2Message2(SksMessageReceivedEventArgs sksMessageReceivedEventArgs)
        {
            var user = GetUserByIpAndPort(sksMessageReceivedEventArgs.FromIp, sksMessageReceivedEventArgs.FromPort);
            
            Akep2 akep2;
            lock (akep2sLock)
            {
                akep2 = Akep2s.FirstOrDefault(a => a.User == user);
            }

            if (akep2 == null)
            {
                return;
            }

            var message = Akep2Message2.FromString(sksMessageReceivedEventArgs.Message, user.Key);
            akep2.NonceB = message.NonceB;

            var newMessage = new Akep2Message3(MyUsername, akep2.NonceB, user.Key);
            user.SendMessage(newMessage.ToString());

            akep2.IncrementMessageCount();
        }

        private static void HandleAkep2Message3(SksMessageReceivedEventArgs sksMessageReceivedEventArgs)
        {
            var user = GetUserByIpAndPort(sksMessageReceivedEventArgs.FromIp, sksMessageReceivedEventArgs.FromPort);

            Akep2 akep2;
            lock (akep2sLock)
            {
                akep2 = Akep2s.FirstOrDefault(a => a.User == user);
            }

            if (akep2 == null)
            {
                return;
            }

            var message = Akep2Message3.FromString(sksMessageReceivedEventArgs.Message, user.Key);

            akep2.IncrementMessageCount();
        }
    }
}
