using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Client
    {
        private Socket socket;
        private NetworkStream stream;
        private StreamReader reader;
        private StreamWriter writer;
        private object readLock;
        private object writeLock;

        public Client(Socket clientSocket)
        {
            writeLock = new object();
            socket = clientSocket;
            stream = new NetworkStream(socket);
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
        }

        public void Close()
        {
            stream.Close();
            reader.Close();
            writer.Close();
            socket.Close();
        }

        public string Read()
        {

        }

        public void Send(string message)
        {

        }
    }
}
