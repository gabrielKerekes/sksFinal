using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SksChat.Lib.Messages
{
    public interface Asn1Message
    {
        byte[] Encode();
        object Decode(byte[] message);
    }
}
