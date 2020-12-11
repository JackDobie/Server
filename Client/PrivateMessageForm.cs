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

        public PrivateMessageForm(Client _client, string _userName, string _otherUser)
        {
            InitializeComponent();
            client = _client;
            userName = _userName;
            otherUser = _otherUser;
            Name = "Private messages with " + _otherUser;
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
