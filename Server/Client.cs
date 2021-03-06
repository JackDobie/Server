﻿using Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

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
        public string name; //for chat clients

        public bool isGame;
        public int ID; //for game clients
        public List<int> connectedPlayers; //for game clients

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
                try
                {
                    MemoryStream memStream = new MemoryStream();
                    formatter.Serialize(memStream, message);
                    byte[] buffer = memStream.GetBuffer();
                    writer.Write(buffer.Length);
                    writer.Write(buffer);
                    writer.Flush();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
