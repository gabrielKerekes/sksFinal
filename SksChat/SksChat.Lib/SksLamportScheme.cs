using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SksChat.Lib.Log;
using SksChat.Lib.Security.Hash;

namespace SksChat.Lib
{
    // todo: asi vytvorit v db tabulku na toto, nech sa tam tie kluce ukladaju.
    public class SksLamportScheme
    {
        private const string LogTag = "LAMPORT";

        private int counter = 1000;

        public List<byte[]> GenerateSecrets(string password, int count)
        {
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

        // todo: GABO - ked tak tak return aj password maju byt bytes
        public string GetNextSecret(string password)
        {
            Logger.Log(LogTag, $"GetNextSecret - counter = {counter}");

            return "";
        }
    }
}
