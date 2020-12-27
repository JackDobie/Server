using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using Packets;

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

            new Thread(() => new ServerForm(this, ipAddress, port).ShowDialog()).Start();
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
                                Console.WriteLine("Exception: " + e.Message);
                                break;
                            }
                            SendClientList();
                            break;
                        case PacketType.Connect:
                            ConnectPacket connectPacket = (ConnectPacket)receivedMessage;
                            clients[index].isGame = false;
                            clients[index].name = connectPacket.userName;
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
                        case PacketType.GameConnect:
                            GameConnectPacket gameConnectPacket = (GameConnectPacket)receivedMessage;
                            clients[index].isGame = true;
                            clients[index].ID = gameConnectPacket.ID;
                            clients[index].connectedPlayers = gameConnectPacket.connectedPlayers;
                            if (clients[index].connectedPlayers.Count >= 2)
                            {
                                break;
                            }
                            foreach (KeyValuePair<int, Client> cli in clients)
                            {
                                if(cli.Value.isGame)
                                {
                                    if (cli.Value.connectedPlayers.Count < 2)
                                    {
                                        if (cli.Value.ID != clients[index].ID)
                                        {
                                            cli.Value.connectedPlayers.Add(gameConnectPacket.ID);
                                            clients[index].connectedPlayers.Add(cli.Value.ID);
                                            cli.Value.Send(new GameConnectPacket(gameConnectPacket.ID, cli.Value.connectedPlayers, GameConnectPacket.PlayerType.Chooser));
                                            clients[index].Send(new GameConnectPacket(gameConnectPacket.ID, clients[index].connectedPlayers, GameConnectPacket.PlayerType.Guesser));
                                            break; //break out of foreach so only connect to one other player
                                        }
                                    }
                                }
                            }
                            break;
                        case PacketType.GameDisconnect:
                            GameDisconnectPacket gameDisconnectPacket = (GameDisconnectPacket)receivedMessage;
                            foreach(KeyValuePair<int, Client> cli in clients)
                            {
                                if (clients[index].connectedPlayers.Contains(cli.Value.ID))
                                {
                                    if (cli.Value.ID != clients[index].ID)
                                    {
                                        cli.Value.Send(gameDisconnectPacket);
                                        break; //send to the other player, not to the player that sent the packet
                                    }
                                }
                            }
                            break;
                        case PacketType.GameResult:
                            GameResultPacket resultPacket = (GameResultPacket)receivedMessage;
                            foreach (KeyValuePair<int, Client> cli in clients)
                            {
                                if(clients[index].connectedPlayers.Contains(cli.Value.ID))
                                {
                                    cli.Value.Send(resultPacket);
                                }
                            }
                            break;
                        case PacketType.GameUpdateDisplayedWord:
                            GameUpdateWordPacket updateWordPacket = (GameUpdateWordPacket)receivedMessage;
                            foreach (KeyValuePair<int, Client> cli in clients)
                            {
                                if (clients[index].connectedPlayers.Contains(cli.Value.ID))
                                {
                                    cli.Value.Send(updateWordPacket);
                                }
                            }
                            break;
                        case PacketType.GameUpdateHangmanState:
                            GameUpdateHangmanPacket updateHangmanPacket = (GameUpdateHangmanPacket)receivedMessage;
                            foreach (KeyValuePair<int, Client> cli in clients)
                            {
                                if (clients[index].connectedPlayers.Contains(cli.Value.ID))
                                {
                                    cli.Value.Send(updateHangmanPacket);
                                }
                            }
                            break;
                        case PacketType.GameSetWord:
                            GameSetWordPacket setWordPacket = (GameSetWordPacket)receivedMessage;
                            foreach(KeyValuePair<int,Client> cli in clients)
                            {
                                if(clients[index].connectedPlayers.Contains(cli.Value.ID))
                                {
                                    cli.Value.Send(setWordPacket);
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
                userList.Add(cli.Value.name);
            }

            UserListPacket userListPacket = new UserListPacket(userList);

            foreach (KeyValuePair<int, Client> cli in clients)
            {
                cli.Value.Send(userListPacket);
            }
        }
    }
}
