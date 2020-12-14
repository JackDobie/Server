using Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace Client
{
    public class Client
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private BinaryFormatter formatter;
        private BinaryReader reader;
        private BinaryWriter writer;

        private ClientForm clientForm;

        public Dictionary<string, PrivateMessageForm> openPrivateMessages = new Dictionary<string, PrivateMessageForm>();

        internal bool connected = false;

        public Client()
        {
        }

        public bool Connect(string ipAddress, int port)
        {
            try
            {
                connected = true;
                tcpClient = new TcpClient(ipAddress, port);
                stream = tcpClient.GetStream();
                formatter = new BinaryFormatter();
                reader = new BinaryReader(stream, Encoding.UTF8);
                writer = new BinaryWriter(stream, Encoding.UTF8);
                Console.WriteLine("Connected to " + ipAddress + ": " + port);
                return true;
            }
            catch(Exception e)
            {
                connected = false;
                Console.Write("Exception: " + e.Message);
                return false;
            }
        }

        public bool Disconnect()
        {
            try
            {
                connected = false;
                reader.Close();
                writer.Close();
                stream.Close();
                tcpClient.Close();
                Console.WriteLine("Disconnected.");
                clientForm.UserListBox_Remove(all: true);
                return true;
            }
            catch(Exception ex)
            {
                connected = true;
                Console.WriteLine("Exception: " + ex.Message);
                return false;
            }
        }

        public void Run()
        {
            clientForm = new ClientForm(this);
            ProcessResponse();
            clientForm.ShowDialog();
        }

        public void ProcessResponse()
        {
            Thread thread = new Thread(ProcessServerResponse);
            thread.Start();
        }

        private void ProcessServerResponse()
        {
            int byteNum;
            try
            {
                while (connected)
                {
                    if ((byteNum = reader.ReadInt32()) != 0)
                    {
                        byte[] buffer = reader.ReadBytes(byteNum);
                        MemoryStream memstream = new MemoryStream(buffer);

                        Packet packet = formatter.Deserialize(memstream) as Packet;

                        switch (packet.packetType)
                        {
                            case PacketType.ChatMessage:
                                ChatMessagePacket chatPacket = (ChatMessagePacket)packet;
                                clientForm.UpdateChatWindow(chatPacket.message);
                                break;
                            case PacketType.UserListPacket:
                                UserListPacket listPacket = (UserListPacket)packet;
                                clientForm.UserListBox_Edit(listPacket.userList);
                                break;
                            case PacketType.PrivateMessage:
                                PrivateMessagePacket msgPacket = (PrivateMessagePacket)packet;
                                clientForm.OpenPrivateMessage(msgPacket.sender, msgPacket.message);
                                break;
                        }
                    }
                    else
                        break;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void SendMessage(string sender, string message)
        {
            string msg = (sender + ": " + message);
            clientForm.UpdateChatWindow(msg);
            ChatMessagePacket packet = new ChatMessagePacket(msg);
            SendPacket(packet);
        }
        public void SendPrivateMessage(string sender, string receiver, string message)
        {
            PrivateMessagePacket packet = new PrivateMessagePacket(sender, receiver, message);
            SendPacket(packet);
        }
        public void EditName(string oldName, string newName)
        {
            NewNamePacket packet = new NewNamePacket(oldName, newName);
            SendPacket(packet);
        }
        public void ConnectPacket(string userName)
        {
            ConnectPacket packet = new ConnectPacket(userName);
            SendPacket(packet);
        }

        public void DisconnectPacket(string userName)
        {
            DisconnectPacket packet = new DisconnectPacket(userName);
            SendPacket(packet);
            Thread.Sleep(50);
            Disconnect();
        }

        void SendPacket(Packet packet)
        {
            MemoryStream memstream = new MemoryStream();
            formatter.Serialize(memstream, packet);
            byte[] buffer = memstream.GetBuffer();
            writer.Write(buffer.Length);
            writer.Write(buffer);
            writer.Flush();
        }
    }
}
