Module modText
    Public Sub AddChat(ByVal ParamArray cArray() As Object)
        Dim i As Integer = 0, tStr As String = "[" & TimeOfDay.ToString & "] "

        With frmMain.rtbChat
            .SelectionStart = Len(.Text)
            .Text = tStr
            .SelectionLength = Len(tStr)
            .SelectionColor = Color.White

            For i = 0 To UBound(cArray) Step 2
                .SelectionStart = Len(.Text)
                .Text = cArray(i + 1) & IIf(i + 1 = UBound(cArray), vbCrLf, vbNullString)
                .SelectionLength = Len(cArray(i + 1))
                .SelectionColor = cArray(i)
            Next
        End With
    End Sub
End Module
