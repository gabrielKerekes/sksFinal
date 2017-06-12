using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SksChat.Lib.Encodings.Asn1
{
    public class SksAsn1Encoder
    {
        public static byte[] EncodePrintableString(string str)
        {
            var printableStringHeader = CreateHeader(SksAsn1Type.Printablestring, str.Length);
            var printableStringBody = Encoding.ASCII.GetBytes(str);

            return printableStringHeader.Concat(printableStringBody).ToArray();
        }

        public static byte[] EncodeOctetString(byte[] bytes)
        {
            if (bytes == null)
            {
                return CreateHeader(SksAsn1Type.OctetString, 0);
            }

            var printableStringHeader = CreateHeader(SksAsn1Type.OctetString, bytes.Length);
            var printableStringBody = bytes;

            return printableStringHeader.Concat(printableStringBody).ToArray();
        }

        public static byte[] EncodeInteger(int integer)
        {
            var integerBytes = BitConverter.GetBytes(integer);

            var printableStringHeader = CreateHeader(SksAsn1Type.Integer, integerBytes.Length);
            var printableStringBody = integerBytes;

            return printableStringHeader.Concat(printableStringBody).ToArray();
        }

        public static byte[] EncodeSequence(List<object> objects)
        {
            var encodedSequence = new byte[] {};
            foreach (var obj in objects)
            {

                byte[] encodedObject = new byte[] {};
                if (obj == null)
                {
                    encodedObject = EncodeOctetString((byte[])obj);
                }
                else if (obj is int)
                {
                    encodedObject = EncodeInteger((int) obj);
                }
                else if (obj is byte[])
                {
                    encodedObject = EncodeOctetString((byte[]) obj);
                }
                else if (obj is string)
                {
                    encodedObject = EncodePrintableString((string) obj);
                }
                else if (obj is List<object>)
                {
                    encodedObject = EncodeSequence((List<object>) obj);
                }

                encodedSequence = encodedSequence.Concat(encodedObject).ToArray();
            }

            var sequenceHeader = CreateHeader(SksAsn1Type.Sequence, encodedSequence.Length);

            return sequenceHeader.Concat(encodedSequence).ToArray();
        }

        private static byte[] CreateHeader(SksAsn1Type headerType, int bodyLength)
        {
            return new[] { (byte) headerType, (byte) bodyLength, };
        }
    }
}
