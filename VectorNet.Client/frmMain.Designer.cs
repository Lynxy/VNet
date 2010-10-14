namespace VectorNet.Client
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
            this.rtfChat = new System.Windows.Forms.RichTextBox();
            this.lvChannel = new System.Windows.Forms.ListView();
            this.txtSend = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtfChat
            // 
            this.rtfChat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtfChat.BackColor = System.Drawing.Color.Black;
            this.rtfChat.ForeColor = System.Drawing.Color.White;
            this.rtfChat.Location = new System.Drawing.Point(12, 36);
            this.rtfChat.Name = "rtfChat";
            this.rtfChat.Size = new System.Drawing.Size(373, 241);
            this.rtfChat.TabIndex = 0;
            this.rtfChat.Text = "";
            // 
            // lvChannel
            // 
            this.lvChannel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvChannel.BackColor = System.Drawing.Color.Black;
            this.lvChannel.ForeColor = System.Drawing.Color.White;
            this.lvChannel.Location = new System.Drawing.Point(391, 36);
            this.lvChannel.Name = "lvChannel";
            this.lvChannel.Size = new System.Drawing.Size(158, 241);
            this.lvChannel.TabIndex = 1;
            this.lvChannel.UseCompatibleStateImageBehavior = false;
            // 
            // txtSend
            // 
            this.txtSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSend.BackColor = System.Drawing.Color.Black;
            this.txtSend.ForeColor = System.Drawing.Color.White;
            this.txtSend.Location = new System.Drawing.Point(12, 283);
            this.txtSend.Name = "txtSend";
            this.txtSend.Size = new System.Drawing.Size(537, 20);
            this.txtSend.TabIndex = 2;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuConnect});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(561, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuConnect
            // 
            this.mnuConnect.Name = "mnuConnect";
            this.mnuConnect.Size = new System.Drawing.Size(79, 20);
            this.mnuConnect.Text = "mnuConnect";
            this.mnuConnect.Click += new System.EventHandler(this.mnuConnect_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 313);
            this.Controls.Add(this.txtSend);
            this.Controls.Add(this.lvChannel);
            this.Controls.Add(this.rtfChat);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmMain";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtfChat;
        private System.Windows.Forms.ListView lvChannel;
        private System.Windows.Forms.TextBox txtSend;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuConnect;
    }
}

