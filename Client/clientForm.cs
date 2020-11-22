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
        public List<string> userList = new List<string>();

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
                MessageWindow.Text += message + Environment.NewLine;
                MessageWindow.SelectionStart = MessageWindow.Text.Length;
                MessageWindow.ScrollToCaret();
            }
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            client.SendMessage(userName, InputField.Text.Trim());
            InputField.Text = "";
        }

        private List<Keys> keysPressed = new List<Keys>();//stores any keys currently pressed
        public void InputField_KeyDown(object sender, KeyEventArgs e)
        {
            keysPressed.Add(e.KeyCode);//adds and removes keys pressed to check for multiple key presses
            if(e.KeyCode == Keys.Enter && !(keysPressed.Contains(Keys.ShiftKey)))//if the user presses enter, click the button. if user presses shift+enter, make new line
            {
                if (!string.IsNullOrWhiteSpace(InputField.Text))
                {
                    SubmitButton_Click(null, null);
                }
            }
        }
        public void InputField_KeyUp(object sender, KeyEventArgs e)
        {
            keysPressed.Remove(e.KeyCode);
        }

        public void NameTextBox_GotFocus(object sender, EventArgs e)
        {
            if(NameTextBox.Text == "User")
            {
                NameTextBox.Text = "";
            }
        }
        public void NameTextBox_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                NameTextBox.Text = "User";
            else
            {
                UserListBox_Edit(userName, NameTextBox.Text);
                userName = NameTextBox.Text;
            }
        }

        public void UserListBox_Add(string user)
        {
            userList.Add(user);
            UserListBox.Text = ("Users:" + Environment.NewLine + string.Join(Environment.NewLine, userList));
            //UserListBox.Text
        }
        public void UserListBox_Edit(string oldUser, string newUser)
        {
            int index = userList.FindIndex(x => x.StartsWith(oldUser));
            userList[index] = newUser;
            UserListBox.Text = ("Users:" + Environment.NewLine + string.Join(Environment.NewLine, userList));
        }
        public void UserListBox_Remove(string user)
        {
            int index = userList.FindIndex(x => x.StartsWith(user));
            userList.RemoveAt(index);
            UserListBox.Text = ("Users:" + Environment.NewLine + string.Join(Environment.NewLine, userList));
        }
    }
}
