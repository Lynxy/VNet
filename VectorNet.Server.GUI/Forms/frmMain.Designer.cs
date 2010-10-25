namespace VectorNet.Server.GUI
{
    partial class frmMain
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
            this.rtbChat = new System.Windows.Forms.RichTextBox();
            this.lvVNUsers = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // rtbChat
            // 
            this.rtbChat.BackColor = System.Drawing.Color.Black;
            this.rtbChat.ForeColor = System.Drawing.Color.White;
            this.rtbChat.Location = new System.Drawing.Point(12, 12);
            this.rtbChat.Name = "rtbChat";
            this.rtbChat.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.rtbChat.Size = new System.Drawing.Size(471, 274);
            this.rtbChat.TabIndex = 0;
            this.rtbChat.Text = "";
            // 
            // lvVNUsers
            // 
            this.lvVNUsers.BackColor = System.Drawing.Color.Black;
            this.lvVNUsers.ForeColor = System.Drawing.Color.White;
            this.lvVNUsers.Location = new System.Drawing.Point(489, 10);
            this.lvVNUsers.Name = "lvVNUsers";
            this.lvVNUsers.Size = new System.Drawing.Size(248, 274);
            this.lvVNUsers.TabIndex = 1;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 296);
            this.Controls.Add(this.lvVNUsers);
            this.Controls.Add(this.rtbChat);
            this.Name = "frmMain";
            this.Text = "VectorNet Server Listener";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbChat;
        private System.Windows.Forms.TreeView lvVNUsers;
    }
}

