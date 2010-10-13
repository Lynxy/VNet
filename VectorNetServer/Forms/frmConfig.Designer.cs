namespace VectorNetServer
{
    partial class frmConfig
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("General Settings");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Idle System");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Advanced");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("General Settings");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Idle System");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Advanced");
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.rbIdleOff = new System.Windows.Forms.RadioButton();
            this.rbIdleOn = new System.Windows.Forms.RadioButton();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.TreeView1 = new System.Windows.Forms.TreeView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.GroupBox2.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.groupBox3);
            this.GroupBox2.Controls.Add(this.Label2);
            this.GroupBox2.Controls.Add(this.rbIdleOff);
            this.GroupBox2.Controls.Add(this.rbIdleOn);
            this.GroupBox2.Location = new System.Drawing.Point(268, 12);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(327, 205);
            this.GroupBox2.TabIndex = 5;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Idle System";
            this.GroupBox2.UseCompatibleTextRendering = true;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(15, 30);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(66, 17);
            this.Label2.TabIndex = 2;
            this.Label2.Text = "Idle System:";
            this.Label2.UseCompatibleTextRendering = true;
            // 
            // rbIdleOff
            // 
            this.rbIdleOff.AutoSize = true;
            this.rbIdleOff.Location = new System.Drawing.Point(157, 27);
            this.rbIdleOff.Name = "rbIdleOff";
            this.rbIdleOff.Size = new System.Drawing.Size(66, 18);
            this.rbIdleOff.TabIndex = 1;
            this.rbIdleOff.TabStop = true;
            this.rbIdleOff.Text = "Disabled";
            this.rbIdleOff.UseCompatibleTextRendering = true;
            this.rbIdleOff.UseVisualStyleBackColor = true;
            // 
            // rbIdleOn
            // 
            this.rbIdleOn.AutoSize = true;
            this.rbIdleOn.Location = new System.Drawing.Point(87, 27);
            this.rbIdleOn.Name = "rbIdleOn";
            this.rbIdleOn.Size = new System.Drawing.Size(64, 18);
            this.rbIdleOn.TabIndex = 0;
            this.rbIdleOn.TabStop = true;
            this.rbIdleOn.Text = "Enabled";
            this.rbIdleOn.UseCompatibleTextRendering = true;
            this.rbIdleOn.UseVisualStyleBackColor = true;
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.Label1);
            this.GroupBox1.Controls.Add(this.TextBox1);
            this.GroupBox1.Location = new System.Drawing.Point(300, 141);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(322, 205);
            this.GroupBox1.TabIndex = 4;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "General Settings";
            this.GroupBox1.UseCompatibleTextRendering = true;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(10, 22);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(61, 17);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "Hosted by: ";
            this.Label1.UseCompatibleTextRendering = true;
            // 
            // TextBox1
            // 
            this.TextBox1.Location = new System.Drawing.Point(71, 19);
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(100, 20);
            this.TextBox1.TabIndex = 0;
            // 
            // TreeView1
            // 
            this.TreeView1.Location = new System.Drawing.Point(-250, -46);
            this.TreeView1.Name = "TreeView1";
            treeNode1.Name = "Node0";
            treeNode1.Text = "General Settings";
            treeNode2.Name = "Node1";
            treeNode2.Text = "Idle System";
            treeNode3.Name = "Node2";
            treeNode3.Text = "Advanced";
            this.TreeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            this.TreeView1.Size = new System.Drawing.Size(253, 205);
            this.TreeView1.TabIndex = 3;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButton1);
            this.groupBox3.Controls.Add(this.radioButton2);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.textBox2);
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(327, 205);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "General Settings";
            this.groupBox3.UseCompatibleTextRendering = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(184, 93);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(66, 18);
            this.radioButton1.TabIndex = 4;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Disabled";
            this.radioButton1.UseCompatibleTextRendering = true;
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(114, 91);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(64, 18);
            this.radioButton2.TabIndex = 3;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Enabled";
            this.radioButton2.UseCompatibleTextRendering = true;
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(34, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "Keep Alives:";
            this.label4.UseCompatibleTextRendering = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 17);
            this.label5.TabIndex = 1;
            this.label5.Text = "Hosted by: ";
            this.label5.UseCompatibleTextRendering = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(71, 19);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 0;
            // 
            // treeView2
            // 
            this.treeView2.Location = new System.Drawing.Point(9, 12);
            this.treeView2.Name = "treeView2";
            treeNode4.Name = "Node0";
            treeNode4.Text = "General Settings";
            treeNode5.Name = "Node1";
            treeNode5.Text = "Idle System";
            treeNode6.Name = "Node2";
            treeNode6.Text = "Advanced";
            this.treeView2.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode5,
            treeNode6});
            this.treeView2.Size = new System.Drawing.Size(253, 205);
            this.treeView2.TabIndex = 6;
            // 
            // frmConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 322);
            this.Controls.Add(this.treeView2);
            this.Controls.Add(this.GroupBox2);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.TreeView1);
            this.Name = "frmConfig";
            this.Text = "Form1";
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox GroupBox2;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.RadioButton rbIdleOff;
        internal System.Windows.Forms.RadioButton rbIdleOn;
        internal System.Windows.Forms.GroupBox GroupBox1;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox TextBox1;
        internal System.Windows.Forms.TreeView TreeView1;
        internal System.Windows.Forms.GroupBox groupBox3;
        internal System.Windows.Forms.RadioButton radioButton1;
        internal System.Windows.Forms.RadioButton radioButton2;
        internal System.Windows.Forms.Label label4;
        internal System.Windows.Forms.Label label5;
        internal System.Windows.Forms.TextBox textBox2;
        internal System.Windows.Forms.TreeView treeView2;
    }
}