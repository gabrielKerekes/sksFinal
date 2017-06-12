using System;
using System.Collections.Generic;
using SksChat.Lib.Encodings.Asn1;
using SksChat.Lib.Encodings.Pem;
using SksChat.Lib.Log;
using SksChat.Lib.Messages.Akep2;
using SksChat.Lib.Security.Encryption;
using SksChat.Lib.Security.Hash;

namespace SksChat.Lib.Messages.Kdc
{
    public class KdcHelloRequest
    {
        private const PemMessageType MessageType = PemMessageType.InitialMessage1;

        public byte[] Iv { get; set; }
        public string Name { get; set; }
        public byte[] EncryptedName { get; set; }


        public KdcHelloRequest(string name, byte[] kdcKey)
        {
            Iv = Utils.GenerateRandom16();

            Name = name;

            EncryptedName = SksAes.EncryptStringToBytes_Aes($"Hi I am \"{Name}\"", kdcKey, Iv);
        }

        private byte[] CreateContent()
        {
            var objects = new List<object> { Iv, Name, EncryptedName };

            return SksAsn1Encoder.EncodeSequence(objects);
        }

        public override string ToString()
        {
            var sequence = CreateContent();

            return PemEncoder.CreatePemMessage(MessageType, sequence);
        }
    }
}
