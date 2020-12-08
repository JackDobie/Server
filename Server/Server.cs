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

        private ConcurrentDictionary<int, string> userList;
        private int userCount = 1;

        public Server(string ipAdress, int port)
        {
            IPAddress IP = IPAddress.Parse(ipAdress);
            tcpListener = new TcpListener(IP, port);
            clients = new ConcurrentDictionary<int, Client>();
            userList = new ConcurrentDictionary<int, string>();
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
                            try
                            {
                                int key = userList.First(x => x.Value == namePacket.oldName).Key;
                                userList.TryUpdate(key, namePacket.newName, namePacket.oldName);
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine("Exception: " + e);
                                break;
                            }
                            UserListPacket listPacket = new UserListPacket(userList);
                            foreach (KeyValuePair<int, Client> cli in clients)
                            {
                                cli.Value.Send(listPacket);
                            }
                            break;
                        case PacketType.Connect:
                            ConnectPacket conPacket = (ConnectPacket)receivedMessage;
                            userList.TryAdd(userCount++, conPacket.userName);
                            UserListPacket userListPacket = new UserListPacket(userList);
                            foreach (KeyValuePair<int, Client> cli in clients)
                            {
                                cli.Value.Send(userListPacket);
                            }
                            break;
                        case PacketType.Disconnect:
                            DisconnectPacket disconPacket = (DisconnectPacket)receivedMessage;

                            int removeKey = userList.First(x => x.Value == disconPacket.userName).Key;
                            userList.TryRemove(removeKey, out _);

                            UserListPacket uListPacket = new UserListPacket(userList);
                            foreach (KeyValuePair<int, Client> cli in clients)
                            {
                                cli.Value.Send(uListPacket);
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
