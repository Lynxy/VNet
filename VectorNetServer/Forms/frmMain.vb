Imports System.Net.Sockets
Imports VectorNetServer.Lynxy.Network
Imports VectorNetServer.Lynxy.Security

Public Class frmMain
    Dim cs As New CryptographicScheme(1024)
    Dim WithEvents listener As New TcpListenerWrapper(Me, 4800)
    Dim packet As New Packet()

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        WriteINI("Main", "HostedBy", "Vector", "Config.ini")

        'MsgBox("This is a test.")
        'MsgBox("This hoster is: " & ReadINI("Main", "HostedBy", "Config.ini"))

        'This is a new comment
        'Call LoadConfig()


        'socket.AsyncListen(10, "127.0.0.1", 4800)
        listener.Listen(10)
        Dim cli As New TcpClient()
        cli.Connect("127.0.0.1", 4800)

    End Sub

    Private Sub socket2_OnClientConnected(ByVal newSock As System.Net.Sockets.TcpClient) Handles listener.OnClientConnected

    End Sub
End Class
