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
            this.MessageWindow = new System.Windows.Forms.TextBox();
            this.SubmitButton = new System.Windows.Forms.Button();
            this.InputField = new System.Windows.Forms.TextBox();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // MessageWindow
            // 
            this.MessageWindow.Location = new System.Drawing.Point(12, 12);
            this.MessageWindow.Multiline = true;
            this.MessageWindow.Name = "MessageWindow";
            this.MessageWindow.ReadOnly = true;
            this.MessageWindow.Size = new System.Drawing.Size(601, 359);
            this.MessageWindow.TabIndex = 0;
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
            this.InputField.KeyUp += new System.Windows.Forms.KeyEventHandler(this.InputField_KeyUp);
            // 
            // NameTextBox
            // 
            this.NameTextBox.Location = new System.Drawing.Point(619, 12);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.Size = new System.Drawing.Size(169, 20);
            this.NameTextBox.TabIndex = 3;
            this.NameTextBox.Text = "User";
            this.NameTextBox.GotFocus += new System.EventHandler(this.RemovePlaceHolderText);
            this.NameTextBox.LostFocus += new System.EventHandler(this.AddPlaceHolderText);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(619, 146);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(169, 225);
            this.textBox2.TabIndex = 4;
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.NameTextBox);
            this.Controls.Add(this.InputField);
            this.Controls.Add(this.SubmitButton);
            this.Controls.Add(this.MessageWindow);
            this.Name = "ClientForm";
            this.Text = "ClientForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox MessageWindow;
        private System.Windows.Forms.Button SubmitButton;
        private System.Windows.Forms.TextBox InputField;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.TextBox textBox2;
    }
}