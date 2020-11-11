using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            if (client.Connect("127.0.0.1", 4444))
            {
                client.Run();
                Console.WriteLine("Connection success.");
            }
            else
                Console.WriteLine("Connection failed.");
        }
    }
}
