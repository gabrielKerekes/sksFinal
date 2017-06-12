using System.Collections.Generic;
using SksChat.Lib.Encodings.Asn1;
using SksChat.Lib.Encodings.Pem;
using SksChat.Lib.Log;
using SksChat.Lib.Security.Encryption;
using SksChat.Lib.Security.Hash;

namespace SksChat.Lib.Messages.Akep2
{
    public class Akep2Message3
    {
        private const PemMessageType MessageType = PemMessageType.Akep2Message3;

        public byte[] EncryptedPart { get; set; }
        public byte[] HmacPart { get; set; }

        public byte[] Iv { get; set; }
        public string A { get; set; }
        public byte[] NonceB { get; set; }

        public Akep2Message3(string a, byte[] nonceB, byte[] longTermKey)
        {
            Iv = Utils.GenerateRandom16();

            A = a;
            NonceB = nonceB;

            var objects = new List<object> { A, NonceB };

            var sequence = SksAsn1Encoder.EncodeSequence(objects);
            EncryptedPart = SksAes.EncryptBytes_Aes(sequence, longTermKey, Iv);
            HmacPart = SksHmac.Hash(longTermKey, sequence);
        }

        private byte[] CreateContent()
        {
            return SksAsn1Encoder.EncodeSequence(new List<object> { EncryptedPart, HmacPart, });
        }

        public override string ToString()
        {
            var sequence = CreateContent();

            return PemEncoder.CreatePemMessage(MessageType, sequence);
        }

        public static Akep2Message3 FromString(string str, byte[] longTermKey)
        {
            var contentBytes = PemParser.GetMessageContentBytes(str);
            var contentObjectsList = (List<object>)SksAsn1Parser.Parse(contentBytes);

            var iv = (byte[])contentObjectsList[0];
            var encryptedPart = (byte[])contentObjectsList[1];
            var hmacPart = (byte[])contentObjectsList[2];

            var myHmac = SksHmac.Hash(longTermKey, encryptedPart);
            if (!Utils.CompareByteArrays(hmacPart, myHmac))
            {
                Logger.Log("AKEP2MESSAGE2:", "Hmac doesn't match");
                return null;
            }

            var decryptedSequence = SksAes.DecryptBytesFromBytes_Aes(encryptedPart, longTermKey, iv);

            contentObjectsList = (List<object>)SksAsn1Parser.Parse(decryptedSequence);
            
            var a = (string)contentObjectsList[0];
            var nonceB = (byte[])contentObjectsList[1];

            return new Akep2Message3(a, nonceB, longTermKey);
        }
    }
}
