Imports System.Text

Module modFileHandling
    Public Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" _
        (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Integer

    Public Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" _
                            (ByVal lpAppName As String, _
                            ByVal lpKeyName As String, _
                            ByVal lpDefault As String, _
                            ByVal lpReturnedString As String, _
                            ByVal nSize As Integer, _
                            ByVal lpFileName As String) As Integer

    Public Sub WriteINI(ByVal cSection As String, ByVal cKey As String, ByVal cValue As String, ByVal cPath As String)
        cPath = Application.StartupPath & "\" & cPath

        WritePrivateProfileString(cSection, cKey, cValue, cPath)
    End Sub

    Public Function ReadINI(ByVal cSection As String, ByVal cKey As String, ByVal cPath As String) As String
        Dim cBuff As String = New String(" ", 255), cLen As Integer, tempPath As String

        tempPath = Application.StartupPath & "\" & cPath
        cLen = GetPrivateProfileString(cSection, cKey, Chr(0), cBuff, 255, tempPath)

        If cLen > 0 Then
            ReadINI = Split(cBuff, Chr(0))(0)
        Else
            ReadINI = vbNullString
        End If
    End Function
End Module
