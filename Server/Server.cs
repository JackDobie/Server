using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using Packets;
using System.Windows.Forms;

namespace Server
{
    public class Server
    {
        private TcpListener tcpListener;

        private ConcurrentDictionary<int, Client> clients = new ConcurrentDictionary<int, Client>();

        private bool running = true;

        public Server(string ipAddress, int port)
        {
            IPAddress IP = IPAddress.Parse(ipAddress);
            tcpListener = new TcpListener(IP, port);
            //ServerForm form = new ServerForm(this);
            //Application.Run(form);
            new ServerForm(this).ShowDialog();
            //new Thread(() => new ServerForm(this).ShowDialog()).Start();
            //form.Show();
        }

        public void Start()
        {
            try
            {
                running = true;
                tcpListener.Start();

                int index = 0;
                while (running)
                {
                    Socket socket = tcpListener.AcceptSocket();
                    Console.WriteLine("Connection made");

                    index++;

                    Client client = new Client(socket);
                    clients.TryAdd(index, client);

                    Thread thread = new Thread(() => { ClientMethod(index); });
                    thread.Start();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Stop()
        {
            tcpListener.Stop();
            running = false;
        }

        public void ChangeIP(string ipAddress, int port)
        {
            if (IPAddress.TryParse(ipAddress, out IPAddress IP))
            {
                Stop();
                tcpListener = new TcpListener(IP, port);
                //Start();
                new Thread(Start).Start();
                Console.WriteLine("Set tcpListener to " + IP + ":" + port);
            }
            else
                Console.WriteLine("IP could not be parsed.");
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
                                clients[index].name = namePacket.newName;
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine("Exception: " + e);
                                break;
                            }
                            SendClientList();
                            break;
                        case PacketType.Connect:
                            ConnectPacket connectPacket = (ConnectPacket)receivedMessage;
                            clients[index].name = connectPacket.userName;
                            clients[index].ID = connectPacket.ID;
                            SendClientList();
                            break;
                        case PacketType.Disconnect:
                            break;
                        case PacketType.PrivateMessage:
                            PrivateMessagePacket msgPacket = (PrivateMessagePacket)receivedMessage;
                            foreach (KeyValuePair<int, Client> cli in clients)
                            {
                                if(cli.Value.name == msgPacket.receiver)
                                {
                                    cli.Value.Send(msgPacket);
                                }
                            }
                            break;
                        default:
                            Console.WriteLine("Received packet of type " + receivedMessage.packetType);
                            break;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                clients[index].Close();
                clients.TryRemove(index, out Client c);
                SendClientList();
            }
        }

        private void GetEnumerator()
        {
            throw new NotImplementedException();
        }

        void SendClientList()
        {
            List<string> userList = new List<string>();

            foreach (KeyValuePair<int, Client> cli in clients)
            {
                userList.Add(cli.Value.name + "#" + cli.Value.ID);
            }

            UserListPacket userListPacket = new UserListPacket(userList);

            foreach (KeyValuePair<int, Client> cli in clients)
            {
                cli.Value.Send(userListPacket);
            }
        }
    }
}
