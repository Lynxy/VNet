Imports VectorNetServer

Module modVars
    Public Structure ConfigStruct
        Public isKeepAlive As Boolean
        Public hostedByName As String
        Public IdleTime As Integer
        Public IdleEnabled As Boolean
    End Structure
    Public Config As ConfigStruct
End Module
