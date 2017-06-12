using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SksChat.Lib
{
    // todo: co najviac rozdelit do dalsich class a tak
    public static class Utils
    {
        public static ProtocolType DetermineProtocol(User user)
        {
            if (user.Key != null && user.Key.Length > 0)
            {
                return ProtocolType.Akep2;
            }

            if (!string.IsNullOrEmpty(user.Password))
            {
                return ProtocolType.LamportScheme;
            }

            if (user.Ttps.Length > 0)
            {
                return ProtocolType.OtwayRees;
            }

            return ProtocolType.Rsa;
        }

        public static string ToBase64(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static byte[] FromBase64(string str)
        {
            return Convert.FromBase64String(str);
        }

        public static byte[] GenerateRandom16()
        {
            var rngCsp = new RNGCryptoServiceProvider();
            var buffer = new byte[16];
            rngCsp.GetBytes(buffer);
            return buffer;
        }

        public static byte[] StringToBytes(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public static string BytesToString(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        public static bool CompareByteArrays(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                    return false;
            }

            return true;
        }
    }
}
