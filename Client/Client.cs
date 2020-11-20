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
            tcpClient = new TcpClient();
        }

        public bool Connect(string ipAddress, int port)
        {
            try
            {
                tcpClient.Connect(ipAddress, port);
                stream = tcpClient.GetStream();
                reader = new StreamReader(stream, Encoding.UTF8);
                writer = new StreamWriter(stream, Encoding.UTF8);
                return true;
            }
            catch(Exception e)
            {
                Console.Write("Exception: " + e.Message);
                return false;
            }
        }

        public void Run()
        {
            clientForm = new ClientForm(this);
            Thread thread = new Thread(ProcessServerResponse);
            thread.Start();
            clientForm.ShowDialog();

            //string userInput;
            //ProcessServerResponse();

            //while((userInput = Console.ReadLine()) != null)
            //{
            //    writer.WriteLine(userInput);
            //    writer.Flush();
            //    if (userInput.ToLower() == "end")
            //        break;
            //    ProcessServerResponse();
            //}
            tcpClient.Close();
        }

        private void ProcessServerResponse()
        {
            Console.WriteLine("Server says: " + reader.ReadLine() + " \n");
        }

        public void SendMessage(string message)
        {
            writer.WriteLine(message);
            writer.Flush();
        }
    }
}
