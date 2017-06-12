using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SksChat.Lib.Encodings.Asn1;
using SksChat.Lib.Encodings.Pem;
using SksChat.Lib.Messages.Handshake;

namespace SksChat.Lib.Messages
{

    public class ChatMessage
    {
        private const PemMessageType MessageType = PemMessageType.ChatMessage;

        public byte[] Iv { get; set; }
        public byte[] EncryptedMessageBytes { get; set; }
        public byte[] HmacBytes { get; set; }

        public ChatMessage(byte[] iv, byte[] encryptedMessageBytes, byte[] hmacBytes)
        {
            Iv = iv;
            EncryptedMessageBytes = encryptedMessageBytes;
            HmacBytes = hmacBytes;
        }

        private byte[] CreateContent()
        {
            // todo: add TTPs
            var objects = new List<object> { Iv, EncryptedMessageBytes, HmacBytes };

            return SksAsn1Encoder.EncodeSequence(objects);
        }

        public override string ToString()
        {
            var sequence = CreateContent();

            return PemEncoder.CreatePemMessage(MessageType, sequence);
        }

        public static ChatMessage FromString(string str)
        {
            var contentBytes = PemParser.GetMessageContentBytes(str);
            var contentObjectsList = (List<object>)SksAsn1Parser.Parse(contentBytes);

            var iv = (byte[]) contentObjectsList[0];
            var encryptedmessageBytes = (byte[]) contentObjectsList[1];
            var hmacBytes = (byte[]) contentObjectsList[2];
            
            return new ChatMessage(iv, encryptedmessageBytes, hmacBytes);
        }
    }
}
