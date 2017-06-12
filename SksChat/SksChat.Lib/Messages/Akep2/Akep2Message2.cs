using System.Collections.Generic;
using SksChat.Lib.Encodings.Asn1;
using SksChat.Lib.Encodings.Pem;
using SksChat.Lib.Log;
using SksChat.Lib.Security.Encryption;
using SksChat.Lib.Security.Hash;

namespace SksChat.Lib.Messages.Akep2
{
    public class Akep2Message2
    {
        private const PemMessageType MessageType = PemMessageType.Akep2Message2;

        public byte[] EncryptedPart { get; set; }
        public byte[] HmacPart { get; set; }

        public byte[] Iv { get; set; }
        public string B { get; set; }
        public string A { get; set; }
        public byte[] NonceA { get; set; }
        public byte[] NonceB { get; set; }
        
        public Akep2Message2(string b, string a, byte[] nonceA, byte[] nonceB, byte[] longTermKey)
        {
            Iv = Utils.GenerateRandom16();

            B = b;
            A = a;
            NonceA = nonceA;
            NonceB = nonceB;

            var objects = new List<object> {B, A, NonceA, NonceB};

            var sequence = SksAsn1Encoder.EncodeSequence(objects);
            EncryptedPart = SksAes.EncryptBytes_Aes(sequence, longTermKey, Iv);
            HmacPart = SksHmac.Hash(longTermKey, sequence);
        }

        private byte[] CreateContent()
        {
            var objects = new List<object> { Iv, EncryptedPart, HmacPart };

            return SksAsn1Encoder.EncodeSequence(objects);
        }

        public override string ToString()
        {
            var sequence = CreateContent();

            return PemEncoder.CreatePemMessage(MessageType, sequence);
        }

        public static Akep2Message2 FromString(string str, byte[] longTermKey)
        {
            var contentBytes = PemParser.GetMessageContentBytes(str);
            var contentObjectsList = (List<object>)SksAsn1Parser.Parse(contentBytes);

            var iv = (byte[]) contentObjectsList[0];
            var encryptedPart = (byte[]) contentObjectsList[1];
            var hmacPart = (byte[])contentObjectsList[2];

            var decryptedSequence = SksAes.DecryptBytesFromBytes_Aes(encryptedPart, longTermKey, iv);

            var myHmac = SksHmac.Hash(longTermKey, decryptedSequence);
            if (!Utils.CompareByteArrays(hmacPart, myHmac))
            {
                Logger.Log("AKEP2MESSAGE2:", "Hmac doesn't match");
                return null;
            }

            contentObjectsList = (List<object>) SksAsn1Parser.Parse(decryptedSequence);

            var b = (string)contentObjectsList[0];
            var a = (string)contentObjectsList[1];
            var nonceA = (byte[])contentObjectsList[2];
            var nonceB = (byte[])contentObjectsList[3];
            
            return new Akep2Message2(b, a, nonceA, nonceB, longTermKey);
        }
    }
}
