using System.Collections.Generic;
using SksChat.Lib.Encodings.Asn1;
using SksChat.Lib.Encodings.Pem;

namespace SksChat.Lib.Messages.Handshake
{
    public class HandshakeMessage2
    {
        private const PemMessageType MessageType = PemMessageType.HandshakeMessage2;

        public ProtocolType ProtocolId { get; set; }
        // todo: add TTPs
        public List<string> SharedTtps { get; set; }

        public HandshakeMessage2(ProtocolType protocolId, List<string> sharedTtps)
        {
            ProtocolId = protocolId;
            SharedTtps = sharedTtps;
        }

        private byte[] CreateContent()
        {
            // todo: add TTPs
            var objects = new List<object> { (int) ProtocolId };

            return SksAsn1Encoder.EncodeSequence(objects);
        }

        public override string ToString()
        {
            var sequence = CreateContent();

            return PemEncoder.CreatePemMessage(MessageType, sequence);
        }

        public static HandshakeMessage2 FromString(string str)
        {
            var contentBytes = PemParser.GetMessageContentBytes(str);
            var contentObjectsList = (List<object>) SksAsn1Parser.Parse(contentBytes);

            var protocolId = (ProtocolType) (int) contentObjectsList[0];
            //var ttps = (List<string>) contentObjectsList[1];

            // todo: add TTPs
            return new HandshakeMessage2(protocolId, null);
        }
    }
}
