using System.Collections.Generic;
using SksChat.Lib.Encodings.Asn1;
using SksChat.Lib.Encodings.Pem;

namespace SksChat.Lib.Messages.Handshake
{
    public class HandshakeMessage1
    {
        private const PemMessageType MessageType = PemMessageType.HandshakeMessage1;

        public string A { get; set; }
        public string B { get; set; }
        public byte[] KeyId { get; set; }
        public byte[] PasswordId { get; set; }
        public List<string> Ttps { get; set; }

        public HandshakeMessage1(string a, string b, byte[] keyId, byte[] passwordId, List<string> ttps)
        {
            A = a;
            B = b;
            KeyId = keyId;
            PasswordId = passwordId;
            Ttps = ttps;
        }

        private byte[] CreateContent()
        {
            // todo: add TTPs
            var objects = new List<object> {A, B, KeyId, PasswordId};

            return SksAsn1Encoder.EncodeSequence(objects);
        }

        public override string ToString()
        {
            var sequence = CreateContent();

            return PemEncoder.CreatePemMessage(MessageType, sequence);
        }

        public static HandshakeMessage1 FromString(string str)
        {
            var contentBytes = PemParser.GetMessageContentBytes(str);
            var contentObjectsList = (List<object>)SksAsn1Parser.Parse(contentBytes);

            var a = (string) contentObjectsList[0];
            var b = (string) contentObjectsList[1];
            var keyId = (byte[]) contentObjectsList[2];
            var passwordId = (byte[]) contentObjectsList[3];

            // todo: add TTPs
            return new HandshakeMessage1(a, b, keyId, passwordId, null);
        }
    }
}
