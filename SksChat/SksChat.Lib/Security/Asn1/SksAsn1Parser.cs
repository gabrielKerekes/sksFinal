using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SksChat.Lib.Security.Asn1
{
    // INTEGER - 2
    // OCTET STRING - 4
    // PrintableString - 19
    // SEQUENCE 30
    // SET OF 17

    public static class SksAsn1Parser
    {
        public static object Parse(byte[] message)
        {
            if (message.Length == 0)
                return null;

            var currentType = message[0];

            object obj;
            switch ((int)currentType)
            {
                case (int)SksAsn1Type.Integer:
                    obj = ParseInteger(message, 0);
                    break;
                case (int)SksAsn1Type.OctetString:
                    obj = ParseOctetString(message, 0);
                    break;
                case (int)SksAsn1Type.Printablestring:
                    obj = ParsePrintableString(message, 0);
                    break;
                case (int)SksAsn1Type.Sequence:
                    obj = ParseSequence(message, 0);
                    break;
                case (int)SksAsn1Type.Set:
                    obj = ParseSet(message, 0);
                    break;
                default:
                    obj = null;
                    break;
            }

            return obj;
        }

        private static List<object> ParseSequence(byte[] message, int sequenceStart)
        {
            var mainLength = message[sequenceStart + 1];

            var objects = new List<object>();
            for (var i = sequenceStart + 2; i < mainLength;)
            {
                var currentTypeLength = message[i + 1];

                var submessage = message.Skip(i).Take(currentTypeLength + 2).ToArray();
                objects.Add(Parse(submessage));

                i += currentTypeLength + 2;
            }

            return objects;
        }

        private static List<object> ParseSet(byte[] message, int setStart)
        {
            var mainLength = message[setStart + 1];

            var objects = new List<object>();
            for (var i = setStart + 2; i < mainLength;)
            {
                var currentTypeLength = message[i + 1];

                var submessage = message.Skip(i).Take(currentTypeLength + 2).ToArray();
                objects.Add(Parse(submessage));

                i += currentTypeLength + 2;
            }

            return objects;
        }

        private static int ParseInteger(byte[] message, int integerStart)
        {
            var length = message[integerStart + 1];

            var intBytes = message.Skip(integerStart + 2).Take(length).ToList();

            if (length < 4)
            {
                while (intBytes.Count < 4)
                {
                    intBytes.Add(0);
                }
            }

            return BitConverter.ToInt32(intBytes.ToArray(), 0);
        }

        // todo: GABO - skusit spravit parser classy....
        private static string ParsePrintableString(byte[] message, int stringStart)
        {
            var length = message[stringStart + 1];

            return Encoding.ASCII.GetString(message, stringStart + 2, length);
        }

        private static byte[] ParseOctetString(byte[] message, int octetStringStart)
        {
            var length = message[octetStringStart + 1];

            return message.Skip(octetStringStart + 2).Take(length).ToArray();
        }
    }
}
