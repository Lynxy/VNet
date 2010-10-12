Imports System.Net.Sockets

Public Class frmMain
    Private sckListen As TcpListener

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        WriteINI("Main", "HostedBy", "Vector", "Config.ini")

        MsgBox("This is a test.")
        MsgBox("This hoster is: " & ReadINI("Main", "HostedBy", "Config.ini"))

        'This is a new comment
        'Call LoadConfig()


        sckListen.Start()
    End Sub
End Class
