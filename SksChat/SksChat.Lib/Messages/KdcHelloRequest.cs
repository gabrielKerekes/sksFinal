using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SksChat.Lib.Messages;
using SksChat.Lib.Security.Asn1;

namespace SksChat.Lib.Messages
{
    public class KdcHelloRequest
    {
        public string Name { get; set; }
        public byte[] EncryptedName { get; set; }


        public KdcHelloRequest(string name, byte[] encryptedName)
        {
            Name = name;
            EncryptedName = encryptedName;
        }

        public string Encode()
        {
            var sequence = SksAsn1Encoder.EncodeSequence(new List<object> { Name, EncryptedName, });

            var base64EncodedMessage = Convert.ToBase64String(sequence);

            return $"-----BEGIN INITIAL MESSAGE 1-----\n{base64EncodedMessage}\n-----END INITIAL MESSAGE 1";
        }
    }
}
