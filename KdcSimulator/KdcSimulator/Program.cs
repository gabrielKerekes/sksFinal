using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KdcSimulator
{
    class Program
    {
        public static void Main(string[] args)
        {
            var server = new Server(56789);
            server.Connect();

            while (true) ;
        }
    }
}
