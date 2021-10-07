Public Class Audiologist

    Private audiologistID, maxAppointments As Integer
    Private firstName, surname As String
    Private phoneNumber, email As String
    Private annualLeaveLeft As TimeSpan
    Private workHours As WorkingHours

    Public Sub New(ByVal fName As String, ByVal sName As String)
        firstName = fName
        surname = sName
    End Sub

    Public Function GetAudiologistInfo(ByVal conn As System.Data.Odbc.OdbcConnection) As Boolean
        Dim rsGetAudInfo As Odbc.OdbcDataReader
        Dim sqlGetAudInfo As New Odbc.OdbcCommand("select * from audiologists where firstname = ? and surname = ?", conn)
        sqlGetAudInfo.Parameters.AddWithValue("@firstname", firstName)
        sqlGetAudInfo.Parameters.AddWithValue("@surname", surname)
        rsGetAudInfo = sqlGetAudInfo.ExecuteReader
        If rsGetAudInfo.Read Then
            audiologistID = rsGetAudInfo("audiologistID")
            maxAppointments = rsGetAudInfo("maxAppointments")
            phoneNumber = rsGetAudInfo("phoneNumber")
            email = rsGetAudInfo("email")
            annualLeaveLeft = rsGetAudInfo("annualLeaveLeft")
            workHours = New WorkingHours(audiologistID)
            workHours.GetWorkingHours(conn)
            Return True
        Else
            Console.WriteLine("No audiologist with this name exists.")
            Return False
        End If
    End Function

    Public Function ReturnAudiologistName() As String
        Return firstName & " " & surname
    End Function

    Public Function ReturnAudiologistID() As Integer
        Return audiologistID
    End Function

End Class
