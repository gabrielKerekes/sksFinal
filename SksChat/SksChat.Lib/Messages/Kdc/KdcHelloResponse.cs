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
    public class KdcHelloResponse
    {
        public List<User> Users { get; set; }

        public static KdcHelloResponse FromString(string message, byte[] kdcKey)
        {
            var contentBytes = PemParser.GetMessageContentBytes(message);
            var contentObjectsList = (List<object>)SksAsn1Parser.Parse(contentBytes);

            var iv = (byte[])contentObjectsList[0];
            var encryptedPart = (byte[])contentObjectsList[1];

            var decryptedSequence = SksAes.DecryptBytesFromBytes_Aes(encryptedPart, kdcKey, iv);

            contentObjectsList = (List<object>)SksAsn1Parser.Parse(decryptedSequence);

            var users = new List<User>();
            foreach (var sequenceObj in contentObjectsList)
            {
                var sequence = (List<object>)sequenceObj;
                var ip = (string)sequence[0];
                var port = (string)sequence[1];
                var name = (string)sequence[2];
                var key = (byte[])sequence[3];
                var pass = (string)sequence[4];

                var user = new User
                {
                    Password = pass,
                    Key = key,
                    Name = name,
                    IpAddress = ip,
                    Port = port,
                };

                users.Add(user);
            }

            return new KdcHelloResponse
            {
                Users = users,
            };
        }

        public static KdcHelloResponse Decode(byte[] message)
        {
            var obj = SksAsn1Parser.Parse(message);
            var set = (List<object>)obj;

            var users = new List<User>();
            foreach (var sequenceObj in set)
            {
                var sequence = (List<object>) sequenceObj;
                var ip = (string) sequence[0];
                var port = (string) sequence[1];
                var name = (string) sequence[2];
                var key = (byte[]) sequence[3];
                var pass = (string) sequence[4];

                var user = new User
                {
                    Password = pass,
                    Key = key,
                    Name = name,
                    IpAddress = ip,
                    Port = port,
                };

                users.Add(user);
            }

            return new KdcHelloResponse
            {
                Users = users,
            };
        }
    }
}
