using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SksChat.Lib.Security.Asn1
{
    public enum SksAsn1Type
    {
        Integer = 0x02,
        OctetString = 0x04,
        Sequence = 0x10,
        Printablestring = 0x13,
        Set = 0x17,
    }

    public class SksAsn1
    {
        public object Parse(byte[] message)
        {
            return SksAsn1Parser.Parse(message);
        }

        // todo: implement
        public object Encode(string message)
        {
            return null;
        }
    }
}
