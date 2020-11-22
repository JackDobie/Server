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

        public ClientForm(Client client1)
        {
            InitializeComponent();
            client = client1;
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
            client.SendMessage(NameTextBox.Text, InputField.Text.Trim());
            InputField.Text = "";
        }

        private List<Keys> keysPressed = new List<Keys>();
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

        /// <summary> Removes the placeholder text for the nickname text box </summary>
        public void RemovePlaceHolderText(object sender, EventArgs e)
        {
            if(NameTextBox.Text == "User")
            {
                NameTextBox.Text = "";
            }
        }

        /// <summary> Adds the placeholder text for the nickname text box </summary>
        public void AddPlaceHolderText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                NameTextBox.Text = "User";
        }

        //public ClientForm()
        //{
        //    InitializeComponent();
        //}
    }
}
