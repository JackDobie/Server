using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class PrivateMessageForm : Form
    {
        private Client client;
        private string userName;
        private string otherUser;

        public PrivateMessageForm(Client _client, string _userName, string _otherUser, string message)
        {
            InitializeComponent();
            client = _client;
            userName = _userName;
            otherUser = _otherUser;
            this.Name = "Private messages with " + otherUser;
            this.MessageWindow.Text = "Private messages with " + otherUser;

            if(message != null)
            {
                UpdateChatWindow((otherUser + ": " + message));
            }
        }

        public void UpdateChatWindow(string message)
        {
            if (MessageWindow.InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    UpdateChatWindow(message);
                }));
            }
            else
            {
                MessageWindow.Text += Environment.NewLine + message;
                MessageWindow.SelectionStart = MessageWindow.Text.Length;
                MessageWindow.ScrollToCaret();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void PMForm_FormClose(object sender, FormClosingEventArgs e)
        {
            client.openPrivateMessages.Remove(userName);
        }
    }
}
