Imports System.Net
Imports System.Net.Sockets
Imports System.Threading


Namespace Lynxy.Network
    Public Class TcpClientWrapper
        Protected _base As TcpClient

        Public Sub New(ByVal base As TcpClient)
            _base = base
        End Sub
    End Class
End Namespace