Module modOtherCode
    Public Sub LoadConfig()
        With Config
            .hostedByName = ReadINI("Main", "Hoster", "Config")
        End With
    End Sub

    Public Sub SaveConfig()

    End Sub
End Module
