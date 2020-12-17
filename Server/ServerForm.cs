using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class ServerForm : Form
    {
        Server server;

        string currentIP;
        int currentPort;

        public ServerForm(Server _server, string IP, int port)
        {
            InitializeComponent();
            server = _server;

            if(IP != IPBox.Text)
            {
                if (!string.IsNullOrWhiteSpace(IP))
                {
                    IPBox.Text = IP;
                }
            }
            if(port.ToString() != PortBox.Text)
            {
                PortBox.Text = port.ToString();
            }
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            bool changed = false;
            if (IPBox.Text != currentIP)
            {
                if (!string.IsNullOrWhiteSpace(IPBox.Text))
                {
                    currentIP = IPBox.Text;
                    changed = true;
                }
            }
            if (PortBox.Text != currentPort.ToString())
            {
                if(Int32.TryParse(PortBox.Text, out currentPort))
                {
                    changed = true;
                }
            }

            if(changed)
            {
                server.ChangeIP(currentIP, currentPort);
            }
        }
    }
}
