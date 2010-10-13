Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Lynxy.Network
    Public NotInheritable Class PacketBuffer
        Private Sub New()
        End Sub
        Public Shared Function GetNextPacket(ByRef packet__1 As Byte()) As Byte()
            Dim ret As Byte() = Packet.Parse(packet__1)
            If ret Is Nothing Then
                Return Nothing
            End If
            Dim newPacket As Byte() = New Byte(packet__1.Length - ret.Length - 5) {}
            Array.Copy(packet__1, 4 + ret.Length, newPacket, 0, newPacket.Length)
            packet__1 = newPacket
            Return ret
        End Function
    End Class

    Public NotInheritable Class Packet
        Public Delegate Function EncryptorDelegate(ByVal data As Byte()) As Byte()

        Private buffer As StringBuilder

        Public Sub New()
            buffer = New StringBuilder()
        End Sub

        Public Shared Function Parse(ByVal packet As Byte()) As Byte()
            Dim length As Integer = BitConverter.ToInt32(packet, 0)
            If length > packet.Length Then
                Return Nothing
            End If
            Dim data As Byte() = New Byte(length - 1) {}
            Array.Copy(packet, 4, data, 0, length)
            Return data
        End Function

        Public Shared Widening Operator CType(ByVal pkt As Packet) As Byte()
            Dim data As Byte() = CharToByte(pkt.buffer.ToString().ToCharArray())
            If pkt.Encryptor IsNot Nothing Then
                data = pkt.Encryptor(data)
            End If

            Dim ret As Byte() = New Byte(data.Length + 3) {}

            BitConverter.GetBytes(data.Length).CopyTo(ret, 0)
            Array.Copy(data, 0, ret, 4, data.Length)
            pkt.Clear()
            Return ret
        End Operator

        Private Shared Function CharToByte(ByVal chr As Char()) As Byte()
            Dim ret As Byte() = New Byte(chr.Length - 1) {}
            For i As Integer = 0 To chr.Length - 1
                ret(i) = CByte(AscW(chr(i)))
            Next
            Return ret
        End Function

        Public Overrides Function ToString() As String
            Return buffer.ToString()
        End Function

        Public Function Clear() As Packet
            If buffer.Length > 0 Then
                buffer.Clear()
            End If
            Return Me
        End Function

        Public ReadOnly Property Length() As Integer
            Get
                Return buffer.Length
            End Get
        End Property
        Public Property Encryptor() As EncryptorDelegate
            Get
                Return m_Encryptor
            End Get
            Set(ByVal value As EncryptorDelegate)
                m_Encryptor = Value
            End Set
        End Property
        Private m_Encryptor As EncryptorDelegate





#Region "String"
        Public Function InsertString(ByVal data As String) As Packet
            Return InsertString(data, buffer.Length)
        End Function

        Public Function InsertString(ByVal data As String, ByVal position As Integer) As Packet
            buffer.Insert(position, data)
            Return Me
        End Function

        Public Function InsertStringNT(ByVal data As String) As Packet
            Return InsertStringNT(data, buffer.Length)
        End Function

        Public Function InsertStringNT(ByVal data As String, ByVal position As Integer) As Packet
            buffer.Insert(position, data & ChrW(0))
            Return Me
        End Function
#End Region

#Region "Byte"
        Public Function InsertByte(ByVal b As Byte) As Packet
            Return InsertByte(b, buffer.Length)
        End Function

        Public Function InsertByte(ByVal b As Byte, ByVal position As Integer) As Packet
            buffer.Insert(position, ChrW(b))
            Return Me
        End Function

        Public Function InsertByte(ByVal bytes As Byte()) As Packet
            Return InsertByte(bytes, buffer.Length)
        End Function

        Public Function InsertByte(ByVal bytes As Byte(), ByVal position As Integer) As Packet
            For Each b As Byte In bytes
                buffer.Insert(System.Math.Max(System.Threading.Interlocked.Increment(position), position - 1), ChrW(b))
            Next
            Return Me
        End Function
#End Region

#Region "Word"
        Public Function InsertWord(ByVal word As Short) As Packet
            Return InsertWord(word, buffer.Length)
        End Function

        Public Function InsertWord(ByVal word As Short, ByVal position As Integer) As Packet
            Return InsertByte(BitConverter.GetBytes(word), position)
        End Function

        Public Function InsertWord(ByVal words As Short()) As Packet
            Return InsertWord(words, buffer.Length)
        End Function

        Public Function InsertWord(ByVal words As Short(), ByVal position As Integer) As Packet
            For Each word As Short In words
                InsertByte(BitConverter.GetBytes(word), position)
            Next
            Return Me
        End Function
#End Region

#Region "DWord"
        Public Function InsertDWord(ByVal dword As Integer) As Packet
            Return InsertDWord(dword, buffer.Length)
        End Function

        Public Function InsertDWord(ByVal dword As Integer, ByVal position As Integer) As Packet
            Return InsertByte(BitConverter.GetBytes(dword), position)
        End Function

        Public Function InsertDWord(ByVal dwords As Integer()) As Packet
            Return InsertDWord(dwords, buffer.Length)
        End Function

        Public Function InsertDWord(ByVal dwords As Integer(), ByVal position As Integer) As Packet
            For Each dword As Integer In dwords
                InsertByte(BitConverter.GetBytes(dword), position)
            Next
            Return Me
        End Function
#End Region
    End Class
End Namespace
