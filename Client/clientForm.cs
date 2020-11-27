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
    public partial class ClientForm : Form
    {
        private Client client;

        public string userName = "User";

        private string ipAddress = "127.0.0.1";
        private int port = 4444;

        private bool disconnected = false;

        public ClientForm(Client client1)
        {
            InitializeComponent();
            client = client1;

            UserListBox_Add(NameTextBox.Text);
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
                MessageWindow.SelectionStart = MessageWindow.Text.Length;
                if (message.Contains("@" + userName))
                {
                    SendToChat_Bold(message);
                }
                else
                {
                    MessageWindow.Text += message + Environment.NewLine;
                }
                MessageWindow.ScrollToCaret();
            }
        }

        void SendToChat_Bold(string message)
        {
            MessageWindow.SelectionStart = MessageWindow.Text.Length;
            MessageWindow.SelectedRtf = @"{\rtf1\ansi \b " + message + Environment.NewLine + @" \b}";
        }

        void SubmitButton_Click(object sender, EventArgs e)
        {
            if(!disconnected)
            {
                client.SendMessage(userName, InputField.Text.Trim());
                InputField.Text = "";
            }
        }

        List<Keys> keysPressed = new List<Keys>(); //stores any keys currently pressed
        void InputField_KeyDown(object sender, KeyEventArgs e)
        {
            keysPressed.Add(e.KeyCode); //adds and removes keys pressed to check for multiple key presses
            if(e.KeyCode == Keys.Enter && !(keysPressed.Contains(Keys.ShiftKey)))//if the user presses enter, click the button. if user presses shift+enter, make new line
            {
                if (!string.IsNullOrWhiteSpace(InputField.Text))
                {
                    SubmitButton_Click(null, null);
                }
            }
        }
        void InputField_KeyUp(object sender, KeyEventArgs e)
        {
            keysPressed.Remove(e.KeyCode);
        }

        void NameTextBox_GotFocus(object sender, EventArgs e)
        {
            if(NameTextBox.Text == "User")
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
                UserListBox_Edit(UserListBox.Items.IndexOf(userName), NameTextBox.Text);
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
                    UserListBox_Edit(UserListBox.Items.IndexOf(userName), NameTextBox.Text);
                    userName = NameTextBox.Text;
                }
            }
        }

        void UserListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.UserListBox.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                InputField.Text += ("@" + UserListBox.Items[index].ToString());
            }
        }
        void UserListBox_Add(string user)
        {
            UserListBox.Items.Add(user);
        }
        void UserListBox_Edit(int index, string newUser)
        {
            try
            {
                if (userName == (string)UserListBox.Items[index])
                    userName = newUser;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            UserListBox.Items[index] = newUser;
        }
        void UserListBox_Remove(int index)
        {
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
            if (client.Disconnect())
            {
                disconnected = true;

                SendToChat_Bold("Connecting to " + ipAddress + ": " + port + "...");
                if (client.Connect(ipAddress, port))
                {
                    SendToChat_Bold("You have connected to the server.");
                    disconnected = false;
                }
                else
                {
                    SendToChat_Bold("Could not connect to the server.");
                }
            }
            else
            {
                SendToChat_Bold("There was an error disconnecting from the server.");
            }
        }

        void DisconnectButton_Click(object sender, EventArgs e)
        {
            if(!disconnected)
            {
                if (client.Disconnect())
                {
                    disconnected = true;
                    SendToChat_Bold("You have disconnected from the server.");
                }
                else
                {
                    SendToChat_Bold("There was an error disconnecting from the server.");
                }
            }
            else
            {
                SendToChat_Bold("You are not connected to a server.");
            }
        }
    }
}
