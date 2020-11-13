using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Concurrent;

namespace Server
{
    class Server
    {
        private TcpListener tcpListener;

        private ConcurrentBag<Client> clients;

        public Server(string ipAdress, int port)
        {
            IPAddress IP = IPAddress.Parse(ipAdress);
            tcpListener = new TcpListener(IP, port);
        }
        
        public void Start()
        {
            clients = new ConcurrentBag<Client>();
            tcpListener.Start();
            while(true)
            {
                Socket socket = tcpListener.AcceptSocket();
                Console.WriteLine("Connection made");

                Client client = new Client(socket);
                clients.Add(client);

                Thread thread = new Thread(() => { ClientMethod(client); });
                thread.Start();
            }
            //Socket socket = tcpListener.AcceptSocket();
            //ClientMethod(socket);
        }

        public void Stop()
        {
            tcpListener.Stop();
        }

        private void ClientMethod(Client client)
        {
            try
            {
                string receivedMessage;
                client.Send("Send 0 for available options");
                while ((receivedMessage = client.Read()) != null)
                {
                    receivedMessage = GetReturnMessage(receivedMessage);
                    if (receivedMessage.Equals("end", StringComparison.OrdinalIgnoreCase))
                    {
                        client.Send("User has left the server");
                        break;
                    }
                    client.Send(receivedMessage);
                }
                client.Close();
                clients.TryTake(out client);
            }
            catch(Exception e)
            {
                client.Send("Exception: " + e.Message);
            }
        }

        private string GetReturnMessage(string code)
        {
            if(Int32.TryParse(code, out int i))
            {
                switch(i)
                {
                    case 0:
                        return "Press 0 for options\nPress 1 for to say Hello\nPress 2 to stop server";
                    case 1:
                        return "Hello";
                    case 2:
                        Stop();
                        break;
                    default:
                        return "No valid option selected. Press 0 for options.";
                }
            }

            switch(code)
            {
                case "Hello":
                    return "Hi";
                case "End":
                case "end":
                    return "end";
                default:
                    return "oaisdjoiasjd";
            }
        }
    }
}
