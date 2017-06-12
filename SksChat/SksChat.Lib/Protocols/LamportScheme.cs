using System.Collections.Generic;
using System.Text;
using SksChat.Lib.Log;
using SksChat.Lib.Security.Hash;

namespace SksChat.Lib.Protocols
{
    // todo: asi vytvorit v db tabulku na toto, nech sa tam tie kluce ukladaju.
    public class LamportScheme
    {
        private const string LogTag = "LAMPORT";
        private const int NumOfSecrets = 1000;

        private object counterLock = new object();
        private int counter = 1000;

        public User User { get; set; }

        public List<byte[]> Secrets { get; set; }

        public LamportScheme(User user)
        {
            User = user;

            Secrets = GenerateSecrets(user.Password, NumOfSecrets);
        }

        private string GetWholeLogTag()
        {
            return $"{LogTag}:{User.Name}";
        }

        public List<byte[]> GenerateSecrets(string password, int count)
        {
            Logger.Log(GetWholeLogTag(), $"Generating secrets");

            var passwordBytes = Encoding.ASCII.GetBytes(password);

            if (count == 0)
                return new List<byte[]> { passwordBytes };

            var hashes = new List<byte[]>();

            var firstHash = SksSha256.Hash(passwordBytes);
            hashes.Add(firstHash);

            var currentHash = firstHash;
            for (var i = 1; i < count; i++)
            {
                currentHash = SksSha256.Hash(currentHash);
                hashes.Add(currentHash);
            }

            return hashes;
        }

        public void DecrementCounter()
        {
            lock (counterLock)
            {
                counter--;
                Logger.Log(GetWholeLogTag(), $"DecrementCounter - counter = {counter}");
            }
        }

        public byte[] GetSecret()
        {
            DecrementCounter();

            Logger.Log(GetWholeLogTag(), $"GetSecret - counter = {counter}");

            var secret = Secrets[counter];

            Logger.Log(GetWholeLogTag(), $"GetSecret - secret = {string.Join(",", secret)}");

            return secret;
        }
    }
}
