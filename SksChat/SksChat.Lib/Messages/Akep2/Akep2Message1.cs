using System.Collections.Generic;
using SksChat.Lib.Encodings.Asn1;
using SksChat.Lib.Encodings.Pem;

namespace SksChat.Lib.Messages.Akep2
{
    public class Akep2Message1
    {
        private const PemMessageType MessageType = PemMessageType.Akep2Message1;

        public byte[] NonceA { get; set; }

        public Akep2Message1(byte[] nonceA)
        {
            NonceA = nonceA;
        }

        private byte[] CreateContent()
        {
            var objects = new List<object> { NonceA };

            return SksAsn1Encoder.EncodeSequence(objects);
        }

        public override string ToString()
        {
            var sequence = CreateContent();

            return PemEncoder.CreatePemMessage(MessageType, sequence);
        }

        public static Akep2Message1 FromString(string str)
        {
            var contentBytes = PemParser.GetMessageContentBytes(str);
            var contentObjectsList = (List<object>)SksAsn1Parser.Parse(contentBytes);

            var nonceA = (byte[]) contentObjectsList[0];

            return new Akep2Message1(nonceA);
        }
    }
}
