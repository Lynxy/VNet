Imports System.Net.Sockets
Imports VectorNetServer.Lynxy.Network
Imports VectorNetServer.Lynxy.Security

Public Class frmMain
    Dim cs As New CryptographicScheme(1024)
    Dim WithEvents socket As New SocketWrapper(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp)
    Dim packet As New Packet()

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        WriteINI("Main", "HostedBy", "Vector", "Config.ini")

        MsgBox("This is a test.")
        MsgBox("This hoster is: " & ReadINI("Main", "HostedBy", "Config.ini"))

        'This is a new comment
        'Call LoadConfig()

        socket.AsyncListen(10, "127.0.0.1", 4800)
    End Sub

    Private Sub socket_OnClientConnected(ByVal origSock As SocketWrapper, ByVal newSock As SocketWrapper) Handles socket.OnClientConnected
        AddHandler newSock.OnDataReceived, AddressOf socket_OnDataReceived
    End Sub

    Private Sub socket_OnConnected(ByVal sock As SocketWrapper) Handles socket.OnConnected

    End Sub

    Private Sub socket_OnDataReceived(ByVal sock As SocketWrapper, ByVal data() As Byte) Handles socket.OnDataReceived

    End Sub
End Class
