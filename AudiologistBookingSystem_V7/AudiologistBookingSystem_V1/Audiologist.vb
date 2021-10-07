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

    Public Sub GetAudiologistTimetableWeek(ByVal day As Date, ByVal conn As System.Data.Odbc.OdbcConnection)
        Dim stringHandling As New ErrorHandling
        Dim startEndWeekDates As Date() = stringHandling.GetStartEndWeekDates(day)
        Dim date1 As Date
        Console.WriteLine("Week Beginning: " & startEndWeekDates(0).ToString)
        Dim curPos As Integer = Console.CursorTop - 1
        For i = 0 To 8
            Console.SetCursorPosition(26 + i, curPos)
            Console.WriteLine(" ")
        Next
        For i = 0 To 4
            date1 = startEndWeekDates(0).AddDays(i)
            GetAudiologistTimetable(date1, conn)
        Next
    End Sub

    Public Sub GetAudiologistTimetable(ByVal day As Date, ByVal conn As System.Data.Odbc.OdbcConnection)
        Dim annualLeave As Boolean = False
        Dim stringHandling As New ErrorHandling
        Dim rsGetTimetable As Odbc.OdbcDataReader
        Dim sqlGetTimetable As New Odbc.OdbcCommand("SELECT DISTINCT starttime, endtime, personalappointment FROM annualleave, audiologists
WHERE audiologists.audiologistid = annualleave.audiologistid
AND annualleave.date = '" & stringHandling.SQLDate(day) & "'
AND annualleave.audiologistid = " & audiologistID & "

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
        Console.WriteLine("---------------------")
        While rsGetTimetable.Read And annualLeave = False
            If rsGetTimetable("personalappointment").ToString = "1" Or rsGetTimetable("personalappointment").ToString = "0" Then
                If rsGetTimetable("starttime").ToString = "00:00:00" And rsGetTimetable("endtime").ToString = "23:59:59" Then
                    annualLeave = True
                    If rsGetTimetable("personalappointment").ToString = "1" Then
                        Console.ForegroundColor = ConsoleColor.Yellow
                        Console.WriteLine(GetWeekDay(day.DayOfWeek) & " - " & day.ToString)
                        Dim curPos As Integer = Console.CursorTop - 1
                        For i = 0 To 8
                            Console.SetCursorPosition(16 + i, curPos)
                            Console.WriteLine(" ")
                        Next
                        Console.WriteLine("Personal appointment: " & rsGetTimetable("starttime").ToString & " - " & rsGetTimetable("endtime").ToString)
                        Console.ForegroundColor = ConsoleColor.Gray
                    Else
                        Console.WriteLine(GetWeekDay(day.DayOfWeek) & " - " & day.ToString)
                        Dim curPos As Integer = Console.CursorTop - 1
                        For i = 0 To 8
                            Console.SetCursorPosition(16 + i, curPos)
                            Console.WriteLine(" ")
                        Next
                        Console.ForegroundColor = ConsoleColor.Yellow
                        Console.WriteLine("Annual leave: " & rsGetTimetable("starttime").ToString & " - " & rsGetTimetable("endtime").ToString)
                        Console.ForegroundColor = ConsoleColor.Gray
                    End If
                End If
            End If

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
                If annualLeave = False Then
                    If rsGetTimetable("personalappointment").ToString = "1" Then
                        Console.ForegroundColor = ConsoleColor.Yellow
                        Console.WriteLine("Personal appointment: " & rsGetTimetable("starttime").ToString & " - " & rsGetTimetable("endtime").ToString)
                        Console.ForegroundColor = ConsoleColor.Gray
                    Else
                        Console.ForegroundColor = ConsoleColor.Yellow
                        Console.WriteLine("Annual leave: " & rsGetTimetable("starttime").ToString & " - " & rsGetTimetable("endtime").ToString)
                        Console.ForegroundColor = ConsoleColor.Gray
                    End If
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
        Console.WriteLine("---------------------")
        Console.WriteLine()
        Console.WriteLine("Press any key to continue...")
        Dim top As Integer = Console.CursorTop
        Dim left As Integer = Console.CursorLeft
        Console.ReadKey()
        Console.SetCursorPosition(left, top)
        Console.WriteLine(" ")
        top -= 1
        For i = 0 To Console.WindowWidth - 1
            Console.SetCursorPosition(left + i, top)
            Console.WriteLine(" ")
        Next
    End Sub

    Public Sub SearchMeeting(ByVal startTime As TimeSpan, ByVal endTime As TimeSpan, ByVal sDate As Date, ByVal conn As System.Data.Odbc.OdbcConnection)

    End Sub

    Public Sub SearchMeeting(ByVal startDate As Date, ByVal endDate As Date, ByVal conn As System.Data.Odbc.OdbcConnection)

    End Sub

    Public Sub CancelMeeting(ByVal startTime As TimeSpan, ByVal endTime As TimeSpan, ByVal startDate As Date, ByVal endDate As Date, ByVal conn As System.Data.Odbc.OdbcConnection)
        Dim stringHandling As New ErrorHandling

        Dim rsFindMeetings As Odbc.OdbcDataReader
        Dim sqlFindMeetings As New Odbc.OdbcCommand("SELECT DISTINCT meeting.meetingid FROM meeting, meetingattendants WHERE meeting.date >= '" & stringHandling.SQLDate(startDate) & "' AND meeting.date <= '" & stringHandling.SQLDate(endDate) & "' AND meetingattendants.audiologistid = " & audiologistID & "", conn)
        rsFindMeetings = sqlFindMeetings.ExecuteReader
        While rsFindMeetings.Read
            Dim sqlCancelMeeting As New Odbc.OdbcCommand("delete from meetingattendants where meetingid = " & rsFindMeetings("meetingid") & " and audiologistid = " & audiologistID & " ", conn)
            sqlCancelMeeting.ExecuteNonQuery()
        End While
    End Sub

    Public Sub SearchAnnualLeave(ByVal conn As System.Data.Odbc.OdbcConnection)

    End Sub

    Public Sub CancelAnnualLeave(ByVal conn As System.Data.Odbc.OdbcConnection)

    End Sub

    Public Sub SearchRepairs(ByVal conn As System.Data.Odbc.OdbcConnection)

    End Sub

    Public Sub RearrangeRepairs(ByVal startTime As TimeSpan, ByVal endTime As TimeSpan, ByVal startDate As Date, ByVal endDate As Date, ByVal conn As System.Data.Odbc.OdbcConnection)
        Dim stringHandling As New ErrorHandling
        Dim ChooseNewAud As New Booking
        Dim audChange As Audiologist

        Dim rsFindRepairs As Odbc.OdbcDataReader
        Dim sqlFindRepairs As New Odbc.OdbcCommand("SELECT repairsid, DATE, starttime, endtime FROM repairs WHERE audiologistid = " & audiologistID & " AND DATE >= '" & stringHandling.SQLDate(startDate) & "' AND DATE <= '" & stringHandling.SQLDate(endDate) & "'", conn)
        rsFindRepairs = sqlFindRepairs.ExecuteReader
        Dim repairsID As Integer
        Dim repDate As Date
        Dim sTime, eTime As TimeSpan
        While rsFindRepairs.Read
            repairsID = rsFindRepairs("repairsid")
            repDate = rsFindRepairs("date")
            sTime = rsFindRepairs("starttime")
            eTime = rsFindRepairs("endtime")
            audChange = ChooseNewAud.RandomAudSelection(sTime, eTime, ChooseNewAud.GetWeekDay(repDate.DayOfWeek), True, conn)
            audChange.GetAudiologistInfo(conn)
            Dim sqlChangeRepairs As New Odbc.OdbcCommand("update repairs set audiologistid = " & audChange.ReturnAudiologistID & " where repairsid = " & repairsID & "", conn)
            sqlChangeRepairs.ExecuteNonQuery()
        End While
    End Sub

    Public Sub SearchAppointments(ByVal conn As System.Data.Odbc.OdbcConnection)

    End Sub

    Public Sub CancelAppointment(ByVal conn As System.Data.Odbc.OdbcConnection)

    End Sub

    Public Sub RearrangeAppointment(ByVal startTime As TimeSpan, ByVal endTime As TimeSpan, ByVal startDate As Date, ByVal endDate As Date, ByVal conn As System.Data.Odbc.OdbcConnection)
        Dim stringHandling As New ErrorHandling
        Dim ChooseNewAud As New Booking
        Dim audChange As Audiologist

        Dim rsFindAppointments As Odbc.OdbcDataReader
        Dim sqlFindAppointments As New Odbc.OdbcCommand("SELECT bookingid, date, starttime, endtime FROM patientbooking WHERE audiologistid = 1 AND DATE >= '" & stringHandling.SQLDate(startDate) & "' AND DATE <= '" & stringHandling.SQLDate(endDate) & "'", conn)
        rsFindAppointments = sqlFindAppointments.ExecuteReader
        Dim bookingID As Integer
        Dim appDate As Date
        Dim sTime, eTime As TimeSpan
        While rsFindAppointments.Read
            bookingID = rsFindAppointments("bookingid")
            appDate = rsFindAppointments("date")
            sTime = rsFindAppointments("starttime")
            eTime = rsFindAppointments("endtime")
            audChange = ChooseNewAud.RandomAudSelection(sTime, eTime, ChooseNewAud.GetWeekDay(appDate.DayOfWeek), True, conn)
            audChange.GetAudiologistInfo(conn)
            Dim sqlChangeApp As New Odbc.OdbcCommand("update patientbooking set audiologistid = " & audChange.ReturnAudiologistID & " where bookingid = " & bookingID & "", conn)
            sqlChangeApp.ExecuteNonQuery()
        End While
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
