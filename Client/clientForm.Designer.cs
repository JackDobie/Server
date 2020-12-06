namespace Client
{
    partial class ClientForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientForm));
            this.SubmitButton = new System.Windows.Forms.Button();
            this.InputField = new System.Windows.Forms.TextBox();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.UserListBox = new System.Windows.Forms.ListBox();
            this.MessageWindow = new System.Windows.Forms.RichTextBox();
            this.IPAddressBox = new System.Windows.Forms.TextBox();
            this.PortBox = new System.Windows.Forms.TextBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.DisconnectButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SubmitButton
            // 
            this.SubmitButton.Location = new System.Drawing.Point(705, 377);
            this.SubmitButton.Name = "SubmitButton";
            this.SubmitButton.Size = new System.Drawing.Size(83, 61);
            this.SubmitButton.TabIndex = 1;
            this.SubmitButton.Text = "Submit";
            this.SubmitButton.UseVisualStyleBackColor = true;
            this.SubmitButton.Click += new System.EventHandler(this.SubmitButton_Click);
            // 
            // InputField
            // 
            this.InputField.Location = new System.Drawing.Point(12, 377);
            this.InputField.Multiline = true;
            this.InputField.Name = "InputField";
            this.InputField.Size = new System.Drawing.Size(687, 61);
            this.InputField.TabIndex = 2;
            this.InputField.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InputField_KeyDown);
            // 
            // NameTextBox
            // 
            this.NameTextBox.Location = new System.Drawing.Point(637, 120);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.Size = new System.Drawing.Size(151, 20);
            this.NameTextBox.TabIndex = 3;
            this.NameTextBox.Text = "User";
            this.NameTextBox.GotFocus += new System.EventHandler(this.NameTextBox_GotFocus);
            this.NameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NameTextBox_KeyDown);
            this.NameTextBox.LostFocus += new System.EventHandler(this.NameTextBox_LostFocus);
            // 
            // UserListBox
            // 
            this.UserListBox.FormattingEnabled = true;
            this.UserListBox.Location = new System.Drawing.Point(637, 146);
            this.UserListBox.Name = "UserListBox";
            this.UserListBox.Size = new System.Drawing.Size(151, 225);
            this.UserListBox.TabIndex = 5;
            this.UserListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.UserListBox_MouseDoubleClick);
            // 
            // MessageWindow
            // 
            this.MessageWindow.Location = new System.Drawing.Point(12, 12);
            this.MessageWindow.Name = "MessageWindow";
            this.MessageWindow.ReadOnly = true;
            this.MessageWindow.Size = new System.Drawing.Size(619, 359);
            this.MessageWindow.TabIndex = 6;
            this.MessageWindow.Text = resources.GetString("MessageWindow.Text");
            // 
            // IPAddressBox
            // 
            this.IPAddressBox.Location = new System.Drawing.Point(637, 12);
            this.IPAddressBox.Name = "IPAddressBox";
            this.IPAddressBox.Size = new System.Drawing.Size(151, 20);
            this.IPAddressBox.TabIndex = 7;
            this.IPAddressBox.Text = "127.0.0.1";
            this.IPAddressBox.GotFocus += new System.EventHandler(this.IPAddressBox_GotFocus);
            this.IPAddressBox.LostFocus += new System.EventHandler(this.IPAddressBox_LostFocus);
            // 
            // PortBox
            // 
            this.PortBox.Location = new System.Drawing.Point(637, 38);
            this.PortBox.Name = "PortBox";
            this.PortBox.Size = new System.Drawing.Size(151, 20);
            this.PortBox.TabIndex = 8;
            this.PortBox.Text = "4444";
            this.PortBox.GotFocus += new System.EventHandler(this.PortBox_GotFocus);
            this.PortBox.LostFocus += new System.EventHandler(this.PortBox_LostFocus);
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(637, 64);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(71, 46);
            this.ConnectButton.TabIndex = 9;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // DisconnectButton
            // 
            this.DisconnectButton.Location = new System.Drawing.Point(714, 64);
            this.DisconnectButton.Name = "DisconnectButton";
            this.DisconnectButton.Size = new System.Drawing.Size(74, 46);
            this.DisconnectButton.TabIndex = 10;
            this.DisconnectButton.Text = "Disconnect";
            this.DisconnectButton.UseVisualStyleBackColor = true;
            this.DisconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.DisconnectButton);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.PortBox);
            this.Controls.Add(this.IPAddressBox);
            this.Controls.Add(this.MessageWindow);
            this.Controls.Add(this.UserListBox);
            this.Controls.Add(this.NameTextBox);
            this.Controls.Add(this.InputField);
            this.Controls.Add(this.SubmitButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ClientForm";
            this.Text = "ClientForm";
            this.VisibleChanged += new System.EventHandler(this.ClientForm_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button SubmitButton;
        private System.Windows.Forms.TextBox InputField;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.ListBox UserListBox;
        private System.Windows.Forms.RichTextBox MessageWindow;
        private System.Windows.Forms.TextBox IPAddressBox;
        private System.Windows.Forms.TextBox PortBox;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Button DisconnectButton;
    }
}