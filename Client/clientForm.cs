using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public partial class ClientForm : Form
    {
        private Client client;

        public string userName = "User";

        private string ipAddress = "127.0.0.1";
        private int port = 4444;

        public ClientForm(Client _client)
        {
            InitializeComponent();
            client = _client;

            NameTextBox.Text = userName;
        }

        public void UpdateChatWindow(string message)
        {
            if(MessageWindow.InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    UpdateChatWindow(message);
                }));
            }
            else
            {
                if (message.Contains("@" + userName))
                {
                    SendToChat(message, bold:true);
                }
                else
                {
                    SendToChat(message);
                }
            }
        }

        void SendToChat(string message, bool bold = false)
        {
            if (bold)
            {
                MessageWindow.Text += Environment.NewLine;
                MessageWindow.SelectionStart = MessageWindow.Text.Length;
                MessageWindow.SelectedRtf = @"{\rtf1\ansi \b " + message + @" \b}";
            }
            else
            {
                MessageWindow.Text += Environment.NewLine + message;
            }

            MessageWindow.SelectionStart = MessageWindow.Text.Length;
            MessageWindow.ScrollToCaret();
        }

        void SubmitButton_Click(object sender, EventArgs e)
        {
            if(client.connected)
            {
                if (!string.IsNullOrWhiteSpace(InputField.Text))
                {
                    client.SendMessage(userName, InputField.Text.Trim());
                    InputField.Text = "";
                }
            }
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

        void NameTextBox_GotFocus(object sender, EventArgs e)
        {
            if(NameTextBox.Text == userName)
            {
                NameTextBox.Text = "";
            }
        }
        void NameTextBox_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                NameTextBox.Text = userName;
            else
            {
                client.EditName(userName, NameTextBox.Text);
                userName = NameTextBox.Text;
            }
        }
        void NameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                    NameTextBox.Text = userName;
                else
                {
                    client.EditName(userName, NameTextBox.Text);
                    userName = NameTextBox.Text;
                }
            }
        }

        void UserListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.UserListBox.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                string name = UserListBox.Items[index].ToString();
                if(name != userName)
                    OpenPrivateMessage(name, null);
            }
        }
        public void UserListBox_Add(string user)
        {
            if (UserListBox.InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    UserListBox_Add(user);
                }));
            }
            else
            {
                UserListBox.Items.Add(user);
            }
        }
        public void UserListBox_Edit(List<string> list)
        {
            if (UserListBox.InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    UserListBox_Edit(list);
                }));
            }
            else
            {
                UserListBox.Items.Clear();
                UserListBox.Items.AddRange(list.ToArray());

                if(userName == "User")
                {
                    int id = 1;
                    string name = "User";
                    while (UserListBox.Items.Contains(name))
                    {
                        name = "User" + id++;
                    }
                    client.EditName(userName, name);
                    userName = name;
                    NameTextBox.Text = name;
                }
            }
        }
        public void UserListBox_Remove(int index = 0, bool all = false)
        {
            if(all)
            {
                UserListBox.Items.Clear();
            }
            else
                UserListBox.Items.RemoveAt(index);
        }

        void IPAddressBox_GotFocus(object sender, EventArgs e)
        {
            if (IPAddressBox.Text == "IP Address")
            {
                IPAddressBox.Text = "";
            }
        }
        void IPAddressBox_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IPAddressBox.Text))
                IPAddressBox.Text = ipAddress;
            else
            {
                ipAddress = IPAddressBox.Text;
            }
        }

        void PortBox_GotFocus(object sender, EventArgs e)
        {
            if (PortBox.Text == "Port")
            {
                PortBox.Text = "";
            }
        }
        void PortBox_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PortBox.Text))
                PortBox.Text = port.ToString();
            else
            {
                port = Int32.Parse(PortBox.Text);
            }
        }

        void ConnectButton_Click(object sender, EventArgs e)
        {
            if(!client.connected)
            {
                try
                {
                    Console.WriteLine("Connecting to " + ipAddress + ": " + port + "...");

                    if (client.Connect(ipAddress, port))
                    {
                        Thread.Sleep(50);
                        client.ProcessResponse();
                        client.ConnectPacket(userName);
                        SendToChat("You have connected to the server.", bold: true);
                        NameTextBox.ReadOnly = false;
                    }
                }
                catch (Exception ex)
                {
                    SendToChat("There was an error connecting to the server: " + ex.Message, bold: true);
                    //Console.WriteLine("There was an error connecting to the server: " + ex.Message);
                }
            }
        }

        void DisconnectButton_Click(object sender, EventArgs e)
        {
            if(client.connected)
            {
                try
                {
                    client.DisconnectPacket(userName);
                    NameTextBox.ReadOnly = true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                }
            }
            else
            {
                SendToChat("You are not connected to a server.", bold:true);
            }
        }

        private void ClientForm_VisibleChanged(object sender, EventArgs e)
        {
            client.ConnectPacket(userName);
        }

        public void OpenPrivateMessage(string user, string message)
        {
            if (MessageWindow.InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    OpenPrivateMessage(user, message);
                }));
            }
            else
            {
                if (client.openPrivateMessages.ContainsKey(user))
                {
                    client.openPrivateMessages.TryGetValue(user, out PrivateMessageForm form);
                    form.Focus();
                    if (message != null) form.UpdateChatWindow(user + ": " + message);
                }
                else
                {
                    PrivateMessageForm form = new PrivateMessageForm(client, userName, user, message);
                    client.openPrivateMessages.Add(user, form);
                    form.Show();
                    form.Focus();
                }
            }
        }
    }
}
