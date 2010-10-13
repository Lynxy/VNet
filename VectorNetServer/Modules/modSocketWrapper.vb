Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Imports System.Net
Imports System.Net.Sockets

Namespace Lynxy.Network
    Public Class StateObject
        Public workSocket As SocketWrapper = Nothing
        Public Const BufferSize As Integer = 1024
        Public buffer As Byte() = New Byte(BufferSize - 1) {}
        Public largeBuffer As Byte() = New Byte(-1) {}
    End Class

    Public Class SocketWrapper
        Inherits Socket
        Public Delegate Sub ClientConnectedDelegate(ByVal origSock As SocketWrapper, ByVal newSock As SocketWrapper)
        Public Event OnClientConnected As ClientConnectedDelegate
        Public Delegate Sub ConnectedDelegate(ByVal sock As SocketWrapper)
        Public Event OnConnected As ConnectedDelegate
        Public Delegate Sub DisconnectDelegate(ByVal sock As SocketWrapper)
        Public Event OnDisconnect As DisconnectDelegate
        Public Delegate Sub DataReceivedDelegate(ByVal sock As SocketWrapper, ByVal data As Byte())
        Public Event OnDataReceived As DataReceivedDelegate
        Public Delegate Sub DataSentDelegate(ByVal sock As SocketWrapper, ByVal bytesSent As Integer)
        Public Event OnDataSent As DataSentDelegate

        Public wasConnected As Boolean = False

        Public Sub New(ByVal addressFamily As AddressFamily, ByVal socketType As SocketType, ByVal protocolType As ProtocolType)
            MyBase.New(addressFamily, socketType, protocolType)
        End Sub
        Public Sub New(ByVal sock As Socket)
            MyBase.New(sock.DuplicateAndClose(System.Diagnostics.Process.GetCurrentProcess().Id))
        End Sub

        Public Sub AsyncListen(ByVal backlog As Integer, ByVal address As String, ByVal port As Integer)
            Try
                Dim IPHost As IPHostEntry = Dns.GetHostEntry(address)
                Dim addr As IPAddress() = IPHost.AddressList
                Dim ep As EndPoint = New IPEndPoint(addr(0), port)
                MyBase.Bind(ep)

                MyBase.Listen(backlog)
                MyBase.BeginAccept(AddressOf AsyncListenCallback, Me)
            Catch ex As Exception
                Me.HandleSocketError(ex)
            End Try
        End Sub

        Protected Sub AsyncListenCallback(ByVal res As IAsyncResult)
            Dim origSock As SocketWrapper = TryCast(res.AsyncState, SocketWrapper)
            Dim newSock As New SocketWrapper(origSock.EndAccept(res))

            Try
                newSock.wasConnected = True
                origSock.BeginAccept(AddressOf AsyncListenCallback, origSock)

                WaitForData(newSock)
                RaiseEvent OnClientConnected(origSock, newSock)
            Catch ex As Exception
                newSock.HandleSocketError(ex)
            End Try
        End Sub

        Public Sub AsyncConnect(ByVal host As String, ByVal port As Integer)
            Try
                MyBase.BeginConnect(host, port, AddressOf AsyncConnectCallback, Me)
            Catch ex As Exception
                Me.HandleSocketError(ex)
            End Try
        End Sub

        Protected Sub AsyncConnectCallback(ByVal res As IAsyncResult)
            Dim sock As SocketWrapper = TryCast(res.AsyncState, SocketWrapper)

            Try
                sock.wasConnected = True
                WaitForData(sock)
                RaiseEvent OnConnected(sock)
            Catch ex As Exception
                sock.HandleSocketError(ex)
            End Try
        End Sub

        Public Sub WaitForData(ByVal sock As SocketWrapper)
            Try
                Dim state As New StateObject()
                state.workSocket = sock
                sock.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, New AsyncCallback(AddressOf WaitForDataCallBack), state)
            Catch ex As Exception
                sock.HandleSocketError(ex)
            End Try
        End Sub

        Protected Sub WaitForDataCallBack(ByVal res As IAsyncResult)
            Dim state As StateObject = DirectCast(res.AsyncState, StateObject)
            Dim sock As SocketWrapper = state.workSocket

            Try
                Dim read As Integer = sock.EndReceive(res)
                If read > 0 Then
                    Dim allBuffer As Byte() = New Byte(state.largeBuffer.Length + (read - 1)) {}
                    Array.Copy(state.largeBuffer, 0, allBuffer, 0, state.largeBuffer.Length)
                    Array.Copy(state.buffer, 0, allBuffer, state.largeBuffer.Length, read)
                    state.largeBuffer = allBuffer
                    allBuffer = Nothing
                    Dim packet As Byte() = PacketBuffer.GetNextPacket(state.largeBuffer)
                    If packet IsNot Nothing Then
                        If sock.OnDataReceivedEvent IsNot Nothing Then
                            sock.OnDataReceivedEvent(sock, packet)
                        End If
                    End If

                    If state.largeBuffer.Length > 0 Then
                        Throw New Exception("unhandled socket thingy")
                    End If
                Else
                    Throw New Exception("omg wtfh")
                End If
                WaitForData(sock)
            Catch ex As Exception
                sock.HandleSocketError(ex)
            End Try
        End Sub

        Public Sub AsyncSend(ByVal data As Byte())
            Try
                MyBase.BeginSend(data, 0, data.Length, SocketFlags.None, AddressOf AsyncSendCallback, Me)
            Catch ex As Exception
                Me.HandleSocketError(ex)
            End Try
        End Sub

        Protected Sub AsyncSendCallback(ByVal res As IAsyncResult)
            Dim sock As SocketWrapper = TryCast(res.AsyncState, SocketWrapper)

            Try
                Dim sent As Integer = sock.EndSend(res)
                RaiseEvent OnDataSent(sock, sent)
            Catch ex As Exception
                sock.HandleSocketError(ex)
            End Try
        End Sub

        Public Sub HandleSocketError(ByVal ex As Exception)
            Console.WriteLine("[ERR] Connected=" & (If(wasConnected, "Yes", "No")) & " " & ex.Message)
            MyBase.Close()
        End Sub
    End Class
End Namespace
