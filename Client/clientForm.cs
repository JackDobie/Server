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
            client.SendMessage("Jack", InputField.Text);
            InputField.Text = "";
        }

        private List<Keys> keysPressed = new List<Keys>();
        public void InputField_KeyDown(object sender, KeyEventArgs e)
        {
            keysPressed.Add(e.KeyCode);
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if(keysPressed.Contains(Keys.ShiftKey))
                    {

                    }
                    else
                    {
                        SubmitButton_Click(null, null);
                    }
                    break;
            }
        }

        public void InputField_KeyUp(object sender, KeyEventArgs e)
        {
            keysPressed.Remove(e.KeyCode);
        }

        //public ClientForm()
        //{
        //    InitializeComponent();
        //}
    }
}
