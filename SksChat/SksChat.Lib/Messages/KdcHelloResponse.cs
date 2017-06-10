using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SksChat.Lib.Security.Asn1;

namespace SksChat.Lib.Messages
{
    public class KdcHelloResponse
    {
        public List<SksUser> Users { get; set; }

        public byte[] Encode()
        {
            throw new NotImplementedException();
        }

        public static KdcHelloResponse Decode(byte[] message)
        {
            var obj = SksAsn1Parser.Parse(message);
            var set = (List<object>)obj;

            var users = new List<SksUser>();
            foreach (var sequenceObj in set)
            {
                var sequence = (List<object>) sequenceObj;
                var ip = (string) sequence[0];
                var port = (string) sequence[1];
                var name = (string) sequence[2];
                var key = (byte[]) sequence[3];
                var pass = (string) sequence[4];

                var user = new SksUser
                {
                    Password = pass,
                    Key = key,
                    Name = name,
                    Ip = ip,
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
