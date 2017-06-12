using System.Collections.Generic;
using System.Linq;
using SksChat.Lib.Messages.Akep2;
using SksChat.Lib.Security.Hash;

namespace SksChat.Lib.Protocols
{
    public class Akep2
    {
        private object countLock = new object();
        
        public static byte[] keyBytes = new byte[] { 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02 };

        public User User { get; set; }
        public int MessageCount { get; set; }
        public byte[] NonceA { get; set; }
        public byte[] NonceB { get; set; }

        public Akep2(User user)
        {
            User = user;
            MessageCount = 0;

            NonceA = Utils.GenerateRandom16();
        }

        public void Start(SksClientType clientType)
        {
            lock (countLock)
            {
                if (MessageCount != 0)
                    // todo: ERROR - nejake spravy uz boli poslane..
                    return;
            }

            if (clientType == SksClientType.Local)
            {
                var message = new Akep2Message1(NonceA);
                User.SendMessage(message.ToString());
            }

            IncrementMessageCount();
        }

        public void IncrementMessageCount()
        {
            lock (countLock)
            {
                MessageCount++;
            }
        }

        public byte[] GetSecret()
        {
            var key = keyBytes.ToList();
            key[key.Count - 1]++;
            // todo: nonceB je null
            return SksHmac.Hash(key.ToArray(), NonceB);
        }
    }
}
