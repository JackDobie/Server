namespace Server
{
    partial class ServerForm
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
            this.IPBox = new System.Windows.Forms.TextBox();
            this.SubmitButton = new System.Windows.Forms.Button();
            this.PortBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // IPBox
            // 
            this.IPBox.Location = new System.Drawing.Point(12, 12);
            this.IPBox.Name = "IPBox";
            this.IPBox.Size = new System.Drawing.Size(80, 20);
            this.IPBox.TabIndex = 0;
            this.IPBox.Text = "127.0.0.1";
            this.IPBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SubmitButton
            // 
            this.SubmitButton.Location = new System.Drawing.Point(145, 10);
            this.SubmitButton.Name = "SubmitButton";
            this.SubmitButton.Size = new System.Drawing.Size(88, 23);
            this.SubmitButton.TabIndex = 2;
            this.SubmitButton.Text = "Submit";
            this.SubmitButton.UseVisualStyleBackColor = true;
            this.SubmitButton.Click += new System.EventHandler(this.SubmitButton_Click);
            // 
            // PortBox
            // 
            this.PortBox.Location = new System.Drawing.Point(98, 12);
            this.PortBox.Name = "PortBox";
            this.PortBox.Size = new System.Drawing.Size(41, 20);
            this.PortBox.TabIndex = 3;
            this.PortBox.Text = "4444";
            this.PortBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(245, 45);
            this.Controls.Add(this.PortBox);
            this.Controls.Add(this.SubmitButton);
            this.Controls.Add(this.IPBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ServerForm";
            this.Text = "ServerForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox IPBox;
        private System.Windows.Forms.Button SubmitButton;
        private System.Windows.Forms.TextBox PortBox;
    }
}