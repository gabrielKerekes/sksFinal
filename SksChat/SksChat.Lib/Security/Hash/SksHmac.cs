using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SksChat.Lib.Security.Hash
{
    public static class SksHmac
    {
        // todo: implementovat alebo zmazat
        //public static string HashString(byte[] key, string input)
        //{
        //    using (var hmac = new HMACSHA256(key))
        //    {
        //        return hmac.ComputeHash(input);
        //    }
        //}

        public static byte[] Hash(byte[] key, byte[] input)
        {
            using (var hmac = new HMACSHA256(key))
            {
                return hmac.ComputeHash(input);
            }
        }
    }
}
