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
using Packets;

namespace Server
{
    class Server
    {
        private TcpListener tcpListener;

        private ConcurrentDictionary<int, Client> clients;

        public Server(string ipAdress, int port)
        {
            IPAddress IP = IPAddress.Parse(ipAdress);
            tcpListener = new TcpListener(IP, port);
            clients = new ConcurrentDictionary<int, Client>();
        }

        public void Start()
        {
            tcpListener.Start();

            int index = 0;
            while(true)
            {
                Socket socket = tcpListener.AcceptSocket();
                Console.WriteLine("Connection made");

                index++;

                Client client = new Client(socket);
                clients.TryAdd(index, client);

                Thread thread = new Thread(() => { ClientMethod(index); });
                thread.Start();
            }
            //Socket socket = tcpListener.AcceptSocket();
            //ClientMethod(socket);
        }

        public void Stop()
        {
            tcpListener.Stop();
        }

        private void ClientMethod(int index)
        {
            try
            {
                Packet receivedMessage;

                while ((receivedMessage = clients[index].Read()) != null)
                {
                    switch(receivedMessage.packetType)
                    {
                        case PacketType.ChatMessage:
                            ChatMessagePacket chatPacket = (ChatMessagePacket)receivedMessage;
                            foreach (KeyValuePair<int, Client> cli in clients)
                            {
                                if(cli.Value != clients[index])
                                {
                                    cli.Value.Send(chatPacket);
                                }
                            }
                            break;
                        case PacketType.NewName:
                            NewNamePacket namePacket = (NewNamePacket)receivedMessage;
                            foreach (KeyValuePair<int, Client> cli in clients)
                            {
                                if (cli.Value != clients[index])
                                {
                                    cli.Value.Send(namePacket);
                                }
                            }
                            break;
                        case PacketType.Connect:
                            ConnectPacket conPacket = (ConnectPacket)receivedMessage;
                            foreach (KeyValuePair<int, Client> cli in clients)
                            {

                            }
                            break;
                        default:
                            Console.WriteLine("Received packet of type " + receivedMessage.packetType);
                            break;
                    }
                }
                clients[index].Close();
                clients.TryRemove(index, out Client c);
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        private void GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
