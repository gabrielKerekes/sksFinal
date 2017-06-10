using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public static class SksChatLib
    {
        public static List<SksHandshake> Handshakes { get; set; }
        public static List<SksLamportScheme> LamportSchemes { get; set; }
        public static List<SksAkep2> Akep2s { get; set; }
        public static List<SksRsa> Rsas { get; set; }

        public static void InitHandshake(SksUser user)
        {
            if (Handshakes.Any(h => h.User == user))
                // todo: what to do? xception? overwrite?
                return;

            Handshakes.Add(new SksHandshake(user));


        }
    }
}
