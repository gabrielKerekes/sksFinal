using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SksChat.Lib.Security.Hash;

namespace SksChat.Lib
{
    // todo: ked tak presunut dakam, folder-> model, alebo DBO, alebo iba do Database....
    public class SksUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        // todo: niekde tu bude treba priday KeyBytes a PasswordBytes nejak...
        public byte[] Key { get; set; }
        public string Password { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
        public string[] Ttps { get; set; }

        private byte[] keyId;
        public byte[] KeyId => keyId ?? (keyId = SksSha256.Hash(Key).Take(8).ToArray());

        private byte[] passwordId;
        public byte[] PasswordId => passwordId ?? (passwordId = SksSha256.Hash(Password + Name).Take(8).ToArray());

        public SksUser()
        {
            keyId = null;
            passwordId = null;
        }

        public void SendMessageToUser(string message)
        {
            if (SksChatLib.Handshakes.All(h => h.User != this))
            {
                SksChatLib.InitHandshake(this);
            }
        }
    }
}
