Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Imports System.Security
Imports System.Security.Cryptography

Namespace Lynxy.Security
    Public Class CryptographicScheme
        Protected RSA_Local As RSACryptoServiceProvider
        Protected RSA_Foreign As RSACryptoServiceProvider
        Protected HashMD5 As MD5CryptoServiceProvider
        Protected HashSHA As SHA256
        Private CombinedKey As New SecureString()

        Protected KeysCombined As Boolean = False

        Public Sub New(ByVal keySize As Integer)
            RSA_Local = New RSACryptoServiceProvider(keySize)
            RSA_Foreign = New RSACryptoServiceProvider(keySize)
            HashMD5 = New MD5CryptoServiceProvider()
            HashSHA = SHA256.Create()
        End Sub
        Public Sub New()
            Me.New(256)
        End Sub


        Public ReadOnly Property PublicKey() As Byte()
            Get
                Return RSA_Local.ExportCspBlob(False)
            End Get
        End Property
        Public Property LocalEncryptionKey() As Byte()
            Get
                Return m_LocalEncryptionKey
            End Get
            Set(ByVal value As Byte())
                m_LocalEncryptionKey = Value
            End Set
        End Property
        Private m_LocalEncryptionKey As Byte()
        Public Property ForeignEncryptionKey() As Byte()
            Get
                Return m_ForeignEncryptionKey
            End Get
            Set(ByVal value As Byte())
                m_ForeignEncryptionKey = Value
            End Set
        End Property
        Private m_ForeignEncryptionKey As Byte()


        Public Sub ImportForeignPublicKey(ByVal publicKey As Byte())
            RSA_Foreign.ImportCspBlob(publicKey)
        End Sub

        Public Function Encrypt(ByVal data As Byte()) As Byte()
            If KeysCombined Then
                Return AESEncryption.Encrypt(data, CombinedKey.ToString(), "Lynxy", "SHA1", 2, "1234567890123456", _
                 256)
            Else
                Return RSA_Foreign.Encrypt(data, True)
            End If
        End Function

        Public Function Decrypt(ByVal data As Byte()) As Byte()
            If KeysCombined Then
                Return AESEncryption.Decrypt(data, CombinedKey.ToString(), "Lynxy", "SHA1", 2, "1234567890123456", _
                 256)
            Else
                Return RSA_Local.Decrypt(data, True)
            End If
        End Function

        Public Sub CombineKeys(ByVal localFirst As Boolean)
            If LocalEncryptionKey Is Nothing OrElse ForeignEncryptionKey Is Nothing Then
                Throw New Exception("Not all encryption keys were set")
            End If

            Dim combinedEncryptionKey As Byte() = New Byte(LocalEncryptionKey.Length + (ForeignEncryptionKey.Length - 1)) {}
            If localFirst Then
                LocalEncryptionKey.CopyTo(combinedEncryptionKey, 0)
                ForeignEncryptionKey.CopyTo(combinedEncryptionKey, LocalEncryptionKey.Length)
            Else
                ForeignEncryptionKey.CopyTo(combinedEncryptionKey, 0)
                LocalEncryptionKey.CopyTo(combinedEncryptionKey, ForeignEncryptionKey.Length)
            End If

            Dim keyArray As Byte() = HashSHA.ComputeHash(combinedEncryptionKey)
            combinedEncryptionKey = Nothing
            HashSHA.Clear()
            For i As Integer = 0 To keyArray.Length - 1
                CombinedKey.AppendChar(ChrW(keyArray(i)))
            Next
            KeysCombined = True
        End Sub
    End Class
End Namespace
