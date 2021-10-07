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

    Public Sub GetAudiologistTimetable(ByVal conn As System.Data.Odbc.OdbcConnection)
        Dim stringHandling As New ErrorHandling
        Dim day As Date = stringHandling.GetDateTimetable
        Dim rsGetTimetable As Odbc.OdbcDataReader
        Dim sqlGetTimetable As New Odbc.OdbcCommand("SELECT DISTINCT starttime, endtime, personalappointment FROM annualleave, audiologists
WHERE annualleave.date = '" & stringHandling.SQLDate(day) & "'
AND audiologists.audiologistid = " & audiologistID & "

UNION

SELECT startTime, endTime, DESCRIPTION FROM meeting, meetingattendants
WHERE meeting.meetingid = meetingattendants.meetingid
AND meeting.date = '" & stringHandling.SQLDate(day) & "' AND meetingattendants.audiologistid = " & audiologistID & "

UNION

SELECT startTime, endTime, room FROM patientBooking
WHERE DATE = '" & stringHandling.SQLDate(day) & "' AND audiologistid = " & audiologistID & "

UNION

SELECT startTime, endTime, DATE FROM repairs
WHERE DATE = '" & stringHandling.SQLDate(day) & "' AND audiologistid = " & audiologistID & "

UNION

SELECT starttime, endtime, DAY FROM workinghours
WHERE DAY = '" & GetWeekDay(day.DayOfWeek) & "' AND audiologistid = " & audiologistID & "

ORDER BY starttime ASC", conn)

        rsGetTimetable = sqlGetTimetable.ExecuteReader
        Console.Clear()

        While rsGetTimetable.Read
            If rsGetTimetable("personalappointment").ToString.Length = 3 Then
                Console.ForegroundColor = ConsoleColor.Gray
                Console.WriteLine(GetWeekDay(day.DayOfWeek) & " - " & day.ToString)
                Dim curPos As Integer = Console.CursorTop - 1
                For i = 0 To 8
                    Console.SetCursorPosition(16 + i, curPos)
                    Console.WriteLine(" ")
                Next
                Console.WriteLine("Start time: " & rsGetTimetable("starttime").ToString & "   Lunch length: " & workHours.ReturnLunchLength(GetWeekDay(day.DayOfWeek)) & "   End time: " & rsGetTimetable("endtime").ToString)
                Console.ForegroundColor = ConsoleColor.Gray
                Console.WriteLine()
            ElseIf rsGetTimetable("personalappointment").ToString = "Seahorse" Or rsGetTimetable("personalappointment").ToString = "Dolphin" Or rsGetTimetable("personalappointment").ToString = "Starfish" Or rsGetTimetable("personalappointment").ToString = "Coral" Then
                Console.ForegroundColor = ConsoleColor.Green
                Console.WriteLine("Patient appointment: " & rsGetTimetable("starttime").ToString & " - " & rsGetTimetable("endtime").ToString)
                Console.ForegroundColor = ConsoleColor.Gray
            ElseIf rsGetTimetable("personalappointment").ToString = "1" Or rsGetTimetable("personalappointment") = "0" Then
                If rsGetTimetable("personalappointment").ToString = "1" Then
                    Console.ForegroundColor = ConsoleColor.Yellow
                    Console.WriteLine("Personal appointment: " & rsGetTimetable("starttime").ToString & " - " & rsGetTimetable("endtime").ToString)
                    Console.ForegroundColor = ConsoleColor.Gray
                Else
                    Console.ForegroundColor = ConsoleColor.Yellow
                    Console.WriteLine("Annual leave: " & rsGetTimetable("starttime").ToString & " - " & rsGetTimetable("endtime").ToString)
                    Console.ForegroundColor = ConsoleColor.Gray
                End If
            ElseIf Date.TryParse(rsGetTimetable("personalappointment"), day) = True Then
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("Repairs: " & rsGetTimetable("starttime").ToString & " - " & rsGetTimetable("endtime").ToString)
                Console.ForegroundColor = ConsoleColor.Gray
            Else
                Console.ForegroundColor = ConsoleColor.Cyan
                Console.WriteLine("Meeting (" & rsGetTimetable("personalappointment") & "): " & rsGetTimetable("starttime").ToString & " - " & rsGetTimetable("endtime").ToString)
                Console.ForegroundColor = ConsoleColor.Gray
            End If
        End While
        Console.WriteLine("Press any key to return to main menu...")
        Console.ReadKey()
    End Sub

    Public Function ReturnAudiologistName() As String
        Return firstName & " " & surname
    End Function

    Public Function ReturnAudiologistID() As Integer
        Return audiologistID
    End Function

    Public Function GetWeekDay(ByVal dayOfWeek As Integer) As String
        Select Case dayOfWeek
            Case 1
                Return "Mon"
            Case 2
                Return "Tue"
            Case 3
                Return "Wed"
            Case 4
                Return "Thu"
            Case 5
                Return "Fri"
        End Select
        Return ""
    End Function

End Class
