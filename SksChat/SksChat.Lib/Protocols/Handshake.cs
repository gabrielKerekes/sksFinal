using System.Collections.Generic;
using SksChat.Lib.Log;
using SksChat.Lib.Messages.Handshake;

namespace SksChat.Lib.Protocols
{
    public class Handshake
    {
        private const string LogTag = "HANDSHAKE";

        private object countLock = new object();

        public User User { get; set; }
        public int MessageCount { get; set; }
        public byte[] KeyId { get; set; }
        public byte[] PasswordId { get; set; }
        public List<string> Ttps { get; set; }

        public Handshake(User user)
        {
            User = user;
            MessageCount = 0;
            
            KeyId = user.KeyId;
            PasswordId = user.PasswordId;
        }

        private string GetWholeLogTag()
        {
            return $"{LogTag}:{User.Name}";
        }

        public void Start(SksClientType clientType)
        {
            Logger.Log(GetWholeLogTag(), "Start");

            lock (countLock)
            {
                if (MessageCount != 0)
                    // todo: ERROR - nejake spravy uz boli poslane..
                    return;
            }

            if (clientType == SksClientType.Local)
            {
                // todo: add TTPs
                var message = new HandshakeMessage1(Lib.MyUsername, User.Name, KeyId, PasswordId, null);
                User.SendMessage(message.ToString());
            }

            IncrementMessageCount();
        }

        public void IncrementMessageCount()
        {
            lock (countLock)
            {
                MessageCount++;
                Logger.Log(GetWholeLogTag(), $"IncrementMessageCount - MessageCount: {MessageCount}");
            }
        }

        // todo: implement
        public void End()
        {
            lock (countLock)
            {
                if (MessageCount != 2)
                    // todo: ERROR - handshake nbol dokonceny a chcem ho ukoncit
                    return;
            }


        }
    }
}
