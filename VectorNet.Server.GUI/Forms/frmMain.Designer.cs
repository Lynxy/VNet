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
            this.components = new System.ComponentModel.Container();
            this.lvVNUsers = new System.Windows.Forms.TreeView();
            this.tmrTest = new System.Windows.Forms.Timer(this.components);
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.txtChat = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lvVNUsers
            // 
            this.lvVNUsers.BackColor = System.Drawing.Color.Black;
            this.lvVNUsers.ForeColor = System.Drawing.Color.White;
            this.lvVNUsers.Location = new System.Drawing.Point(602, 12);
            this.lvVNUsers.Name = "lvVNUsers";
            this.lvVNUsers.Size = new System.Drawing.Size(135, 272);
            this.lvVNUsers.TabIndex = 1;
            // 
            // tmrTest
            // 
            this.tmrTest.Enabled = true;
            this.tmrTest.Interval = 1000;
            this.tmrTest.Tick += new System.EventHandler(this.tmrTest_Tick);
            // 
            // txtStatus
            // 
            this.txtStatus.BackColor = System.Drawing.Color.Black;
            this.txtStatus.ForeColor = System.Drawing.Color.White;
            this.txtStatus.Location = new System.Drawing.Point(12, 12);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(194, 272);
            this.txtStatus.TabIndex = 2;
            // 
            // txtChat
            // 
            this.txtChat.BackColor = System.Drawing.Color.Black;
            this.txtChat.ForeColor = System.Drawing.Color.White;
            this.txtChat.Location = new System.Drawing.Point(212, 12);
            this.txtChat.Multiline = true;
            this.txtChat.Name = "txtChat";
            this.txtChat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtChat.Size = new System.Drawing.Size(384, 272);
            this.txtChat.TabIndex = 3;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 296);
            this.Controls.Add(this.txtChat);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.lvVNUsers);
            this.Name = "frmMain";
            this.Text = "VectorNet Server Listener";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView lvVNUsers;
        private System.Windows.Forms.Timer tmrTest;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.TextBox txtChat;
    }
}

