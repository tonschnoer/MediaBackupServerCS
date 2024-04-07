using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpServerTest
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length > 0)
            {
                if (args[0] == "server")
                {
                    TCPServer.StartServer();
                }

                if (args[0] == "client")
                {
                    TCPClient.connect(args[1]);
                }
            }
            else
            {
                TCPServer.StartServer();
                Console.ReadLine();
            }



        }
    }
}
