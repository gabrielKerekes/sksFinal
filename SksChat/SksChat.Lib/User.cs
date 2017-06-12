using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SksChat.Lib.Encodings.Asn1;
using SksChat.Lib.Log;
using SksChat.Lib.Messages;
using SksChat.Lib.Protocols;
using SksChat.Lib.Security.Encryption;
using SksChat.Lib.Security.Hash;

namespace SksChat.Lib
{
    // todo: log tag...
    // todo: ked tak presunut dakam, folder-> model, alebo DBO, alebo iba do Database....
    public class User
    {
        private const string LogTag = "USER";
        public string Name { get; set; }
        public byte[] Key { get; set; }
        public string Password { get; set; }
        public string IpAddress { get; set; }
        public string Port { get; set; }
        public string[] Ttps { get; set; }

        public ProtocolType Protocol { get; set; }

        private byte[] keyId;
        public byte[] KeyId
        {
            get
            {
                if (Key == null || Key.Length < 8)
                    return new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                
                return keyId = SksSha256.Hash(Key).Take(8).ToArray();
            }
        }

        private byte[] passwordId;
        public byte[] PasswordId
        {
            get
            {
                if (string.IsNullOrEmpty(Password) || Password.Length < 8)
                    return new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                return SksSha256.Hash(Password + Name).Take(8).ToArray();
            }
        }
        public SksClient Client { get; set; }

        public User()
        {
            keyId = null;
            passwordId = null;
        }

        public string GetWholeLogTag()
        {
            return $"{LogTag}:{Name}";
        }

        public void SendMessage(string message)
        {
            Logger.Log(GetWholeLogTag(), $"SendMessage - Message: {message}");

            Client.SendMessage(message);
        }

        public void SendChatMessage(string message)
        {
            Logger.Log(GetWholeLogTag(), $"SendChatMessage - Message: {message}");

            var secret = GetSecretFromProtocol();

            if (secret == null)
            {
                Logger.Log("USER:", "Secret is null");
                return;
            }


            var iv = Utils.GenerateRandom16();
            var encryptedMessageBytes = SksAes.EncryptStringToBytes_Aes(message, secret, iv);
            var hmacBytes = SksHmac.Hash(secret, Utils.StringToBytes(message));
            
            Logger.Log(GetWholeLogTag(), $"SendChatMessage - EcnryptedBytes: {string.Join(",", encryptedMessageBytes)}");
            Logger.Log(GetWholeLogTag(), $"SendChatMessage - Hmac: {string.Join(",", hmacBytes)}");

            var chatMessage = new ChatMessage(iv, encryptedMessageBytes, hmacBytes);
            var finalMessage = chatMessage.ToString();

            Client.SendMessage(finalMessage);
        }

        public byte[] GetSecretFromProtocol()
        {
            byte[] secret = null;

            switch (Protocol)
            {
                case ProtocolType.None:
                    break;
                case ProtocolType.Akep2:
                    var akep2 = Lib.GetAkep2(this);
                    secret = akep2.GetSecret();
                    break;
                case ProtocolType.LamportScheme:
                    var lamportScheme = Lib.GetLamportScheme(this);
                    secret = lamportScheme.GetSecret();
                    break;
                case ProtocolType.OtwayRees:
                    var otwayReese = Lib.GetOtwayReese(this);
                    secret = otwayReese.GetSecret();
                    break;
                case ProtocolType.Rsa:
                    var rsa = Lib.GetRsa(this);
                    secret = rsa.GetSecret();
                    break;
            }

            return secret;
        }
    }
}
