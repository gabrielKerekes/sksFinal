using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SksChat.Lib
{
    // todo: co najviac rozdelit do dalsich class a tak
    public class Utils
    {
        public int DetermineProtocol(SksUser user)
        {
            if (user.Key.Length > 0)
            {
                return 1;
            }

            if (!string.IsNullOrEmpty(user.Password))
            {
                return 2;
            }

            if (user.Ttps.Length > 0)
            {
                return 3;
            }

            return 4;
        }
    }
}
