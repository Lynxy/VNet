Imports System
Imports System.Net.Sockets
Imports VectorNetServer.Lynxy.Network
Imports VectorNetServer.Lynxy.Security

Public Class frmMain
    Dim cs As New CryptographicScheme(1024)
    Dim WithEvents listener As New TcpListenerWrapper(Me, 4800)
    Dim packet As New Packet()

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        WriteINI("Main", "HostedBy", "Vector", "Config.ini")

        'MsgBox("This hoster is: " & ReadINI("Main", "HostedBy", "Config.ini"))

        'Call LoadConfig()

        listener.Listen(10)

        Dim cli As New TcpClient()
        cli.Connect("127.0.0.1", 4800)

    End Sub

    Private Sub listener_OnClientConnected(ByVal newSock As TcpClient) Handles listener.OnClientConnected
        Dim dat As Byte()
        dat = packet.Clear().InsertString("test")
        newSock.GetStream().BeginWrite(dat, 0, dat.Length, Nothing, Nothing)
    End Sub
End Class
