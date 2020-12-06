using Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        private bool connected = false;

        public Client()
        {
        }

        public bool Connect(string ipAddress, int port)
        {
            try
            {
                tcpClient = new TcpClient(ipAddress, port);
                stream = tcpClient.GetStream();
                formatter = new BinaryFormatter();
                reader = new BinaryReader(stream, Encoding.UTF8);
                writer = new BinaryWriter(stream, Encoding.UTF8);
                Console.WriteLine("Connected to " + ipAddress + ": " + port);
                connected = true;
                return true;
            }
            catch(Exception e)
            {
                Console.Write("Exception: " + e.Message);
                return false;
            }
        }

        public bool Disconnect()
        {
            try
            {
                reader.Close();
                writer.Close();
                stream.Close();
                tcpClient.Close();
                Console.WriteLine("Disconnected.");
                connected = false;
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return false;
            }
        }

        public void Run()
        {
            clientForm = new ClientForm(this);
            Thread thread = new Thread(ProcessServerResponse);
            thread.Start();

            clientForm.ShowDialog();
        }

        private void ProcessServerResponse()
        {
            int byteNum;
            while ((byteNum = reader.ReadInt32()) != 0)
            {
                if (!connected)
                    break;

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
                }
            }
        }

        public void SendMessage(string sender, string message)
        {
            string msg = (sender + ": " + message);
            clientForm.UpdateChatWindow(msg);
            ChatMessagePacket packet = new ChatMessagePacket(msg);
            SendPacket(packet);
        }
        public void EditName(string oldName, string newName)
        {
            NewNamePacket packet = new NewNamePacket(oldName, newName);
            SendPacket(packet);
        }
        public void ConnectPacket(string userName)
        {
            Console.WriteLine("Connect");
            ConnectPacket packet = new ConnectPacket(userName);
            SendPacket(packet);
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
