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
            this.Text = otherUser;
            UpdateChatWindow("Private messages with " + otherUser + Environment.NewLine);

            if (message != null)
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
                MessageWindow.Text += message + Environment.NewLine;
                MessageWindow.SelectionStart = MessageWindow.Text.Length;
                MessageWindow.ScrollToCaret();
            }
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(InputField.Text))
            {
                client.SendPrivateMessage(userName, otherUser, InputField.Text.Trim());
                UpdateChatWindow(userName + ": " + InputField.Text.Trim());
            }

            InputField.Text = "";
        }

        void InputField_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SubmitButton_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else
                e.SuppressKeyPress = false;
        }

        public void DisableChat()
        {
            InputField.ReadOnly = true;
            UpdateChatWindow(otherUser + " has disconnected from the server.");
        }
        public void EnableChat()
        {
            UpdateChatWindow(otherUser + " has connected to the server.");
            InputField.ReadOnly = false;
        }

        private void PMForm_FormClose(object sender, FormClosingEventArgs e)
        {
            if(client.openPrivateMessages.ContainsKey(userName))
            {
                client.openPrivateMessages.Remove(userName);
            }
            else if (client.openPrivateMessages.ContainsKey(otherUser))
            {
                client.openPrivateMessages.Remove(otherUser);
            }
        }
    }
}
