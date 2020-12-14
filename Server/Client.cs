using Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Client
    {
        private Socket socket;
        private NetworkStream stream;
        private BinaryFormatter formatter;
        private BinaryReader reader;
        private BinaryWriter writer;
        private object readLock;
        private object writeLock;
        public string name;

        public Client(Socket clientSocket)
        {
            formatter = new BinaryFormatter();
            readLock = new object();
            writeLock = new object();
            socket = clientSocket;
            stream = new NetworkStream(socket);
            reader = new BinaryReader(stream, Encoding.UTF8);
            writer = new BinaryWriter(stream, Encoding.UTF8);
        }

        public void Close()
        {
            stream.Close();
            reader.Close();
            writer.Close();
            socket.Close();
        }

        public Packet Read()
        {
            lock (readLock)
            {
                int numberOfBytes;
                if ((numberOfBytes = reader.ReadInt32()) != -1)
                {
                    byte[] buffer = reader.ReadBytes(numberOfBytes);
                    MemoryStream memStream = new MemoryStream(buffer);
                    return formatter.Deserialize(memStream) as Packet;
                }
                else
                    return null;
            }
        }

        public void Send(Packet message)
        {
            lock(writeLock)
            {
                MemoryStream memStream = new MemoryStream();
                formatter.Serialize(memStream, message);
                byte[] buffer = memStream.GetBuffer();
                writer.Write(buffer.Length);
                writer.Write(buffer);
                writer.Flush();
            }
        }
    }
}
