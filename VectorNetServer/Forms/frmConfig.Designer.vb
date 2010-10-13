<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class gGeneral
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim TreeNode1 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("General Settings")
        Dim TreeNode2 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Idle System")
        Dim TreeNode3 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Advanced")
        Me.TreeView1 = New System.Windows.Forms.TreeView()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.rbIdleOn = New System.Windows.Forms.RadioButton()
        Me.rbIdleOff = New System.Windows.Forms.RadioButton()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.rbIdleEnable = New System.Windows.Forms.RadioButton()
        Me.rbIdleDisable = New System.Windows.Forms.RadioButton()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'TreeView1
        '
        Me.TreeView1.Location = New System.Drawing.Point(12, 12)
        Me.TreeView1.Name = "TreeView1"
        TreeNode1.Name = "Node0"
        TreeNode1.Text = "General Settings"
        TreeNode2.Name = "Node1"
        TreeNode2.Text = "Idle System"
        TreeNode3.Name = "Node2"
        TreeNode3.Text = "Advanced"
        Me.TreeView1.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode1, TreeNode2, TreeNode3})
        Me.TreeView1.Size = New System.Drawing.Size(253, 205)
        Me.TreeView1.TabIndex = 0
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.rbIdleDisable)
        Me.GroupBox1.Controls.Add(Me.rbIdleEnable)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.TextBox1)
        Me.GroupBox1.Location = New System.Drawing.Point(271, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(322, 205)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "General Settings"
        Me.GroupBox1.UseCompatibleTextRendering = True
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(71, 19)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(100, 20)
        Me.TextBox1.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(10, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(61, 17)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Hosted by: "
        Me.Label1.UseCompatibleTextRendering = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.rbIdleOff)
        Me.GroupBox2.Controls.Add(Me.rbIdleOn)
        Me.GroupBox2.Location = New System.Drawing.Point(512, 199)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(293, 171)
        Me.GroupBox2.TabIndex = 2
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Idle System"
        Me.GroupBox2.UseCompatibleTextRendering = True
        '
        'rbIdleOn
        '
        Me.rbIdleOn.AutoSize = True
        Me.rbIdleOn.Location = New System.Drawing.Point(87, 27)
        Me.rbIdleOn.Name = "rbIdleOn"
        Me.rbIdleOn.Size = New System.Drawing.Size(64, 18)
        Me.rbIdleOn.TabIndex = 0
        Me.rbIdleOn.TabStop = True
        Me.rbIdleOn.Text = "Enabled"
        Me.rbIdleOn.UseCompatibleTextRendering = True
        Me.rbIdleOn.UseVisualStyleBackColor = True
        '
        'rbIdleOff
        '
        Me.rbIdleOff.AutoSize = True
        Me.rbIdleOff.Location = New System.Drawing.Point(157, 27)
        Me.rbIdleOff.Name = "rbIdleOff"
        Me.rbIdleOff.Size = New System.Drawing.Size(66, 18)
        Me.rbIdleOff.TabIndex = 1
        Me.rbIdleOff.TabStop = True
        Me.rbIdleOff.Text = "Disabled"
        Me.rbIdleOff.UseCompatibleTextRendering = True
        Me.rbIdleOff.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(15, 30)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(66, 17)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Idle System:"
        Me.Label2.UseCompatibleTextRendering = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(34, 93)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(67, 17)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Keep Alives:"
        Me.Label3.UseCompatibleTextRendering = True
        '
        'rbIdleEnable
        '
        Me.rbIdleEnable.AutoSize = True
        Me.rbIdleEnable.Location = New System.Drawing.Point(114, 91)
        Me.rbIdleEnable.Name = "rbIdleEnable"
        Me.rbIdleEnable.Size = New System.Drawing.Size(64, 18)
        Me.rbIdleEnable.TabIndex = 3
        Me.rbIdleEnable.TabStop = True
        Me.rbIdleEnable.Text = "Enabled"
        Me.rbIdleEnable.UseCompatibleTextRendering = True
        Me.rbIdleEnable.UseVisualStyleBackColor = True
        '
        'rbIdleDisable
        '
        Me.rbIdleDisable.AutoSize = True
        Me.rbIdleDisable.Location = New System.Drawing.Point(184, 90)
        Me.rbIdleDisable.Name = "rbIdleDisable"
        Me.rbIdleDisable.Size = New System.Drawing.Size(66, 18)
        Me.rbIdleDisable.TabIndex = 4
        Me.rbIdleDisable.TabStop = True
        Me.rbIdleDisable.Text = "Disabled"
        Me.rbIdleDisable.UseCompatibleTextRendering = True
        Me.rbIdleDisable.UseVisualStyleBackColor = True
        '
        'gGeneral
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(651, 248)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.TreeView1)
        Me.Name = "gGeneral"
        Me.Text = "Form2"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TreeView1 As System.Windows.Forms.TreeView
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents rbIdleOff As System.Windows.Forms.RadioButton
    Friend WithEvents rbIdleOn As System.Windows.Forms.RadioButton
    Friend WithEvents rbIdleDisable As System.Windows.Forms.RadioButton
    Friend WithEvents rbIdleEnable As System.Windows.Forms.RadioButton
    Friend WithEvents Label3 As System.Windows.Forms.Label
End Class
