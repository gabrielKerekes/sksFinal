using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SksChat.Lib.Security.Hash
{
    public static class SksSha256
    {
        public static byte[] Hash(byte[] input)
        {
            var sha256 = SHA256Managed.Create();
            return sha256.ComputeHash(input);
        }

        public static byte[] Hash(string input)
        {
            return Hash(Encoding.ASCII.GetBytes(input));
        }
    }
}
