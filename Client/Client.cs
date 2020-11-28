using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class Client
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private StreamWriter writer;
        private StreamReader reader;

        private ClientForm clientForm;

        public Client()
        {
        }

        public bool Connect(string ipAddress, int port)
        {
            try
            {
                tcpClient = new TcpClient(ipAddress, port);
                stream = tcpClient.GetStream();
                reader = new StreamReader(stream, Encoding.UTF8);
                writer = new StreamWriter(stream, Encoding.UTF8);
                Console.WriteLine("Connected to " + ipAddress + ": " + port);
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
            string input;
            while ((input = Console.ReadLine()) != null)
            {
                writer.WriteLine(input);
                writer.Flush();
                clientForm.UpdateChatWindow(reader.ReadLine());
            }
        }

        public void SendMessage(string sender, string message)
        {
            writer.WriteLine(sender + ": " + message);
            clientForm.UpdateChatWindow(sender + ": " + message);
            writer.Flush();
        }
    }
}
