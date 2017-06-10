using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SksChat.Lib
{
    public class SksHandshake
    {
        public SksUser User { get; set; }
        public int MessageCount { get; set; }
        public byte[] KeyId { get; set; }
        public byte[] PasswordId { get; set; }
        public List<string> Ttps { get; set; }

        public SksHandshake(SksUser user)
        {
            User = user;
            MessageCount = 0;

            KeyId = user.KeyId;
            PasswordId = user.PasswordId;
        }
    }
}
