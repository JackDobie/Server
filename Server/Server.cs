using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Server
{
    class Server
    {
        private TcpListener tcpListener;

        public Server(string ipAdress, int port)
        {
            IPAddress IP = IPAddress.Parse(ipAdress);
            tcpListener = new TcpListener(IP, port);
        }
        
        public void Start()
        {
            tcpListener.Start();
            Socket socket = tcpListener.AcceptSocket();
            ClientMethod(socket);
        }

        public void Stop()
        {
            tcpListener.Stop();
        }

        private void ClientMethod(Socket socket)
        {
            string receivedMessage;
            NetworkStream stream = new NetworkStream(socket);
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
            writer.WriteLine("Hello world");
            writer.Flush();

            while((receivedMessage = reader.ReadLine()) != null)
            {
                string response = GetReturnMessage(receivedMessage);
                if (response.Equals("end", StringComparison.OrdinalIgnoreCase))
                {
                    writer.WriteLine("User has left the server");
                    writer.Flush();
                    break;
                }
                writer.WriteLine(response);
                writer.Flush();
            }
            socket.Close();
        }

        private string GetReturnMessage(string code)
        {
            switch(code)
            {
                case "Hello":
                    return "Hi";
                default:
                    return "ooga booga";
            }
        }
    }
}
