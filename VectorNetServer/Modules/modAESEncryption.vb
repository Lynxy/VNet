Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Imports System.Security.Cryptography
Imports System.IO

Namespace Lynxy.Security
    ''' <summary>
    ''' Utility class that handles encryption
    ''' </summary>
    ''' 
    Public Module AESEncryption
#Region "Static Functions"

        ''' <summary>
        ''' Encrypts a string
        ''' </summary>
        ''' <param name="Data">Text to be encrypted</param>
        ''' <param name="Password">Password to encrypt with</param>
        ''' <param name="Salt">Salt to encrypt with</param>
        ''' <param name="HashAlgorithm">Can be either SHA1 or MD5</param>
        ''' <param name="PasswordIterations">Number of iterations to do</param>
        ''' <param name="InitialVector">Needs to be 16 ASCII characters long</param>
        ''' <param name="KeySize">Can be 128, 192, or 256</param>
        ''' <returns>An encrypted string</returns>
        Public Function Encrypt(ByVal Data As Byte(), ByVal Password As String, ByVal Salt As String, ByVal HashAlgorithm As String, ByVal PasswordIterations As Integer, ByVal InitialVector As String, _
         ByVal KeySize As Integer) As Byte()
            Try
                If Data Is Nothing OrElse Data.Length <= 0 Then
                    Return New Byte(-1) {}
                End If
                Dim InitialVectorBytes As Byte() = Encoding.ASCII.GetBytes(InitialVector)
                Dim SaltValueBytes As Byte() = Encoding.ASCII.GetBytes(Salt)
                'Dim DerivedPassword As New PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations)
                Dim DerivedPassword As New Rfc2898DeriveBytes(Password, SaltValueBytes, PasswordIterations)
                Dim KeyBytes As Byte() = DerivedPassword.GetBytes(KeySize \ 8)
                Dim SymmetricKey As New RijndaelManaged()
                SymmetricKey.Mode = CipherMode.CBC
                Dim CipherTextBytes As Byte() = Nothing
                Using Encryptor As ICryptoTransform = SymmetricKey.CreateEncryptor(KeyBytes, InitialVectorBytes)
                    Using MemStream As New MemoryStream()
                        Using CryptoStream As New CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write)
                            CryptoStream.Write(Data, 0, Data.Length)
                            CryptoStream.FlushFinalBlock()
                            CipherTextBytes = MemStream.ToArray()
                            MemStream.Close()
                            CryptoStream.Close()
                        End Using
                    End Using
                End Using
                SymmetricKey.Clear()
                Return CipherTextBytes
            Catch
                Throw
            End Try
        End Function

        ''' <summary>
        ''' Decrypts a string
        ''' </summary>
        ''' <param name="Data">Text to be decrypted</param>
        ''' <param name="Password">Password to decrypt with</param>
        ''' <param name="Salt">Salt to decrypt with</param>
        ''' <param name="HashAlgorithm">Can be either SHA1 or MD5</param>
        ''' <param name="PasswordIterations">Number of iterations to do</param>
        ''' <param name="InitialVector">Needs to be 16 ASCII characters long</param>
        ''' <param name="KeySize">Can be 128, 192, or 256</param>
        ''' <returns>A decrypted string</returns>
        Public Function Decrypt(ByVal Data As Byte(), ByVal Password As String, ByVal Salt As String, ByVal HashAlgorithm As String, ByVal PasswordIterations As Integer, ByVal InitialVector As String, _
         ByVal KeySize As Integer) As Byte()
            Try
                If Data Is Nothing OrElse Data.Length <= 0 Then
                    Return New Byte(-1) {}
                End If
                Dim InitialVectorBytes As Byte() = Encoding.ASCII.GetBytes(InitialVector)
                Dim SaltValueBytes As Byte() = Encoding.ASCII.GetBytes(Salt)
                'Dim DerivedPassword As New PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations)
                Dim DerivedPassword As New Rfc2898DeriveBytes(Password, SaltValueBytes, PasswordIterations)
                Dim KeyBytes As Byte() = DerivedPassword.GetBytes(KeySize \ 8)
                Dim SymmetricKey As New RijndaelManaged()
                SymmetricKey.Mode = CipherMode.CBC
                Dim PlainTextBytes As Byte() = New Byte(Data.Length - 1) {}
                Dim ByteCount As Integer = 0
                Using Decryptor As ICryptoTransform = SymmetricKey.CreateDecryptor(KeyBytes, InitialVectorBytes)
                    Using MemStream As New MemoryStream(Data)
                        Using CryptoStream As New CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read)

                            ByteCount = CryptoStream.Read(PlainTextBytes, 0, PlainTextBytes.Length)
                            MemStream.Close()
                            CryptoStream.Close()
                        End Using
                    End Using
                End Using
                SymmetricKey.Clear()
                Dim ret As Byte() = New Byte(ByteCount - 1) {}
                Array.Copy(PlainTextBytes, 0, ret, 0, ByteCount)
                Return ret
            Catch
                Throw
            End Try
        End Function

#End Region
    End Module
End Namespace
