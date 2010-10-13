Imports System.Net
Imports System.Net.Sockets
Imports System.Threading


Namespace Lynxy.Network
    Public Class TcpListenerWrapper
        Inherits TcpListener

        Public Delegate Sub ClientConnectedDelegate(ByVal newSock As TcpClient)
        Public Event OnClientConnected As ClientConnectedDelegate

        Protected _invokeObject As Control

        Public Sub New(ByVal invokeObject As Control, ByVal port As Integer)
            MyBase.New(IPAddress.Parse("127.0.0.1"), port)
            _invokeObject = invokeObject
        End Sub

        Public Sub Listen(ByVal backlog As Integer)
            MyBase.Start(backlog)
            StartListening()
        End Sub

        Protected Sub StartListening()
            MyBase.BeginAcceptTcpClient(New AsyncCallback(AddressOf AcceptTcpClientCallback), Me)
        End Sub

        Public Sub AcceptTcpClientCallback(ByVal ar As IAsyncResult)
            Dim listener As TcpListenerWrapper = CType(ar.AsyncState, TcpListenerWrapper)
            Dim client As TcpClient = listener.EndAcceptTcpClient(ar)
            listener._invokeObject.Invoke( _
                DirectCast(Sub()
                               listener.OnClientConnectedEvent(client)
                           End Sub, Action))
            listener.StartListening()
        End Sub


    End Class
End Namespace

