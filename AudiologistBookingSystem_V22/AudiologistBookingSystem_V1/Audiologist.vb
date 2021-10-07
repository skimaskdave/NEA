﻿Public Class Audiologist

    Private audiologistID, maxAppointments As Integer
    Private firstName, surname As String
    Private phoneNumber, email As String
    Private annualLeaveLeft As Double
    Private workHours As WorkingHours
    Private stringHandling As ErrorHandling

    Public Sub New(ByVal fName As String, ByVal sName As String)
        firstName = fName
        surname = sName
        stringHandling = New ErrorHandling
        annualLeaveLeft = 20.0
        GetAudID()
    End Sub

    Public Sub GetAudID()
        Dim rsGetAudID As Odbc.OdbcDataReader
        Dim sqlGetAudID As New Odbc.OdbcCommand("select audiologistid from audiologists where firstname = ? and surname = ?", Module1.GetConnection)
        sqlGetAudID.Parameters.AddWithValue("firstname", firstName)
        sqlGetAudID.Parameters.AddWithValue("surname", surname)
        rsGetAudID = sqlGetAudID.ExecuteReader
        If rsGetAudID.Read Then
            audiologistID = rsGetAudID("audiologistid")
        End If
    End Sub

    Public Function GetAudiologistInfo(ByVal conn As System.Data.Odbc.OdbcConnection) As Boolean
        Dim rsGetAudInfo As Odbc.OdbcDataReader
        Dim sqlGetAudInfo As New Odbc.OdbcCommand("select * from audiologists where firstname = ? and surname = ?", conn)
        sqlGetAudInfo.Parameters.AddWithValue("@firstname", firstName)
        sqlGetAudInfo.Parameters.AddWithValue("@surname", surname)
        rsGetAudInfo = sqlGetAudInfo.ExecuteReader
        If rsGetAudInfo.Read Then
            audiologistID = rsGetAudInfo("audiologistID")
            phoneNumber = rsGetAudInfo("phoneNumber")
            email = rsGetAudInfo("email")
            FindAnnualLeaveLeft(conn)
            workHours = New WorkingHours(audiologistID)
            workHours.GetWorkingHours(conn)
            maxAppointments = workHours.FindMaxApps(conn)
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

    Public Sub SearchMeeting(ByVal conn As System.Data.Odbc.OdbcConnection)
        Console.Clear()

        Console.ForegroundColor = ConsoleColor.Yellow
        Console.WriteLine("MEETINGS - " & Me.ReturnAudiologistName)
        Console.ForegroundColor = ConsoleColor.Gray

        Dim rsSearchMeetAud As Odbc.OdbcDataReader
        Dim sqlSearchMeetAud As New Odbc.OdbcCommand("SELECT DISTINCT meeting.meetingid, meeting.place, meeting.description, meeting.date, meeting.starttime, meeting.endtime
FROM meeting, meetingattendants
WHERE meeting.meetingid = meetingattendants.meetingid AND meetingattendants.audiologistid = ? AND meeting.date >= ?", conn)
        sqlSearchMeetAud.Parameters.AddWithValue("meetingattendants.audiologistid", audiologistID)
        sqlSearchMeetAud.Parameters.AddWithValue("meeting.DATE", stringHandling.SQLDate(Date.Today))
        rsSearchMeetAud = sqlSearchMeetAud.ExecuteReader
        While rsSearchMeetAud.Read
            Console.WriteLine()
            Console.WriteLine("Meeting description: " & rsSearchMeetAud("description"))
            Console.WriteLine("Meeting place: " & rsSearchMeetAud("place"))
            Console.WriteLine(rsSearchMeetAud("date") & " " & rsSearchMeetAud("starttime").ToString & " - " & rsSearchMeetAud("endtime").ToString)
            Console.WriteLine("Meeting attendants: ")

            Dim rsFindMeetingAttendants As Odbc.OdbcDataReader
            Dim sqlFindMeetingAttendants As New Odbc.OdbcCommand("select audiologistid from meetingattendants where meetingid = ?", conn)
            sqlFindMeetingAttendants.Parameters.AddWithValue("meetingid", rsSearchMeetAud("meetingid"))
            rsFindMeetingAttendants = sqlFindMeetingAttendants.ExecuteReader
            While rsFindMeetingAttendants.Read
                Dim rsGetAudName As Odbc.OdbcDataReader
                Dim sqlGetAudName As New Odbc.OdbcCommand("select firstname, surname from audiologists where audiologistid = ?", conn)
                sqlGetAudName.Parameters.AddWithValue("audiologistid", rsFindMeetingAttendants("audiologistid"))
                rsGetAudName = sqlGetAudName.ExecuteReader
                If rsGetAudName.Read Then
                    If rsGetAudName("firstname") & " " & rsGetAudName("surname") <> Me.ReturnAudiologistName Then
                        Console.WriteLine(" - " & rsGetAudName("firstname") & " " & rsGetAudName("surname"))
                    Else
                        Console.ForegroundColor = ConsoleColor.Cyan
                        Console.WriteLine(" - " & rsGetAudName("firstname") & " " & rsGetAudName("surname"))
                        Console.ForegroundColor = ConsoleColor.Gray
                    End If
                End If
            End While
        End While

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
        Dim rsSearchALAud As Odbc.OdbcDataReader
        Dim sqlSearchALAud As New Odbc.OdbcCommand("SELECT * FROM annualleave WHERE audiologistid = ? AND DATE >= ? ORDER BY DATE", conn)
        sqlSearchALAud.Parameters.AddWithValue("audiologistid", Me.ReturnAudiologistID)
        sqlSearchALAud.Parameters.AddWithValue("date", stringHandling.SQLDate(Date.Today))
        rsSearchALAud = sqlSearchALAud.ExecuteReader

        Console.ForegroundColor = ConsoleColor.Yellow
        Console.WriteLine("ANNUAL LEAVE - " & Me.ReturnAudiologistName)
        Console.ForegroundColor = ConsoleColor.Gray
        Console.WriteLine("Annual leave left: " & Me.ReturnAnnualLeaveLeft.ToString)
        While rsSearchALAud.Read
            Console.WriteLine()
            Console.WriteLine(stringHandling.SQLDate(rsSearchALAud("date")))
            Console.WriteLine(rsSearchALAud("starttime").ToString & " - " & rsSearchALAud("endtime").ToString)
            If rsSearchALAud("personalappointment") = 1 Then
                Console.ForegroundColor = ConsoleColor.Cyan
                Console.WriteLine("Personal appointment")
                Console.ForegroundColor = ConsoleColor.Gray
            End If
        End While
    End Sub

    Public Sub CancelAnnualLeave(ByVal dateDiff As Integer, ByVal startdate As Date, ByVal conn As System.Data.Odbc.OdbcConnection)
        For i = 0 To dateDiff
            startdate = startdate.AddDays(i)
            Dim sqlCancelAnnualLeave As New Odbc.OdbcCommand("DELETE FROM annualleave WHERE DATE = ? AND audiologistid = ? AND personalappointment = 0", Module1.GetConnection)
            sqlCancelAnnualLeave.Parameters.AddWithValue("date", startdate)
            sqlCancelAnnualLeave.Parameters.AddWithValue("audiologistid", Me.ReturnAudiologistID)
            sqlCancelAnnualLeave.ExecuteNonQuery()
        Next
    End Sub

    Public Sub SearchRepairs(ByVal conn As System.Data.Odbc.OdbcConnection)
        Dim rsSearchRepsAud As Odbc.OdbcDataReader
        Dim sqlSearchRepsAud As New Odbc.OdbcCommand("SELECT * FROM repairs WHERE audiologistid = ? AND DATE >= ? ORDER BY DATE", conn)
        sqlSearchRepsAud.Parameters.AddWithValue("audiologistid", Me.ReturnAudiologistID)
        sqlSearchRepsAud.Parameters.AddWithValue("date", stringHandling.SQLDate(Date.Today))
        rsSearchRepsAud = sqlSearchRepsAud.ExecuteReader
        Console.Clear()
        Console.ForegroundColor = ConsoleColor.Yellow
        Console.WriteLine("REPAIRS - " & Me.ReturnAudiologistName)
        Console.ForegroundColor = ConsoleColor.Gray
        While rsSearchRepsAud.Read
            Console.WriteLine()
            Console.WriteLine(stringHandling.SQLDate(rsSearchRepsAud("date")))
            Console.WriteLine(rsSearchRepsAud("starttime").ToString & " - " & rsSearchRepsAud("endtime").ToString)
        End While
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

    Public Function ReturnHoursForDay(ByVal day As String) As TimeSpan
        Return workHours.ReturnHoursForDay(day)
    End Function

    Public Function ReturnAnnualLeaveLeft() As Double
        Return annualLeaveLeft
    End Function

    Public Sub CreateNewAud(ByVal conn As System.Data.Odbc.OdbcConnection)
        'firstname
        'surname
        'phone number
        'email
        'working hours for each day
        'lunch hours for each day
        'max apps = 7
        'annual leave left = 20:00:00
        Console.Clear()
        Console.WriteLine("Create New Patient...")
        System.Threading.Thread.Sleep(200)
        Dim stringHandling As New ErrorHandling()
        Dim flags(1) As Boolean
        While flags(0) = False Or flags(1) = False
            Select Case PrintCreateAud()
                Case 1
                    Console.Clear()
                    Console.WriteLine("Enter phone number: ")
                    phoneNumber = stringHandling.TryString(11, 14)
                    flags(0) = True
                Case 2
                    Console.Clear()
                    Console.WriteLine("Enter email: ")
                    email = stringHandling.TryEmail
                    flags(1) = True
            End Select
        End While

        Dim sqlInsertAudiologist As New Odbc.OdbcCommand("INSERT INTO audiologists(firstname, surname, phonenumber, email, annualleaveleft, maxappointments) VALUES(?, ?, ?, ?, ?, ?)", conn)
        sqlInsertAudiologist.Parameters.AddWithValue("firstname", Me.firstName)
        sqlInsertAudiologist.Parameters.AddWithValue("surname", Me.surname)
        sqlInsertAudiologist.Parameters.AddWithValue("phonenumber", Me.phoneNumber)
        sqlInsertAudiologist.Parameters.AddWithValue("email", Me.email)
        sqlInsertAudiologist.Parameters.AddWithValue("annualleaveleft", TimeSpan.Parse("20:00:00"))
        sqlInsertAudiologist.Parameters.AddWithValue("maxappointments", 7)
        sqlInsertAudiologist.ExecuteNonQuery()


        Console.Clear()
        Dim rsFindAudID As Odbc.OdbcDataReader
        Dim sqlFindAudID As New Odbc.OdbcCommand("select audiologistid from audiologists where firstname = ? and surname = ?", conn)
        sqlFindAudID.Parameters.AddWithValue("firstname", firstName)
        sqlFindAudID.Parameters.AddWithValue("surname", surname)
        rsFindAudID = sqlFindAudID.ExecuteReader
        If rsFindAudID.Read Then
            audiologistID = rsFindAudID("audiologistid")
        End If

        workHours = New WorkingHours(audiologistID)
        Console.WriteLine("Working hours")
        workHours.CreateWorkingHours()
        workHours.InsertWorkingHours(conn)

        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("Audiologist added.")
        Console.ForegroundColor = ConsoleColor.Gray
        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
    End Sub

    Public Function PrintCreateAud() As Integer
        Console.CursorVisible = False
        Dim currentChoice As Integer = 1
        Dim choice As ConsoleKey
        Console.Clear()
        Console.WriteLine("Enter (fields with * are required):
   Phone Number
   Email
")
        Console.SetCursorPosition(0, 1)
        Console.Write(" >")
        Do
            choice = Console.ReadKey(True).Key
            Select Case choice
                Case ConsoleKey.W, ConsoleKey.UpArrow, ConsoleKey.A, ConsoleKey.LeftArrow
                    If currentChoice > 1 Then
                        Console.SetCursorPosition(0, currentChoice)
                        Console.Write("  ")
                        currentChoice -= 1
                        Console.SetCursorPosition(0, currentChoice)
                        Console.Write(" >")
                    End If
                Case ConsoleKey.S, ConsoleKey.DownArrow, ConsoleKey.D, ConsoleKey.RightArrow
                    If currentChoice < 2 Then
                        Console.SetCursorPosition(0, currentChoice)
                        Console.Write("  ")
                        currentChoice += 1
                        Console.SetCursorPosition(0, currentChoice)
                        Console.Write(" >")
                    End If
            End Select
        Loop Until choice = ConsoleKey.Enter
        Console.CursorVisible = True
        Return currentChoice
    End Function 'printing a menu to get all the audiologist data

    Public Sub PrintAudProfile()
        Console.Clear()
        Console.WriteLine("Audiologist: " & firstName & " " & surname)
        Console.WriteLine("Tel Num: " & phoneNumber)
        Console.WriteLine("Email: " & email)
        Console.WriteLine("Maximum number of appointments per week: " & maxAppointments)
        Console.WriteLine("Annual leave left: " & annualLeaveLeft & " days")
        Console.WriteLine()
        workHours.PrintWorkingHours()
        Console.WriteLine()
    End Sub

    Public Sub AddAnnualLeave(ByVal startDate As Date, ByVal endDate As Date, ByVal conn As System.Data.Odbc.OdbcConnection)
        Dim tempTime As TimeSpan

        Dim rsFindAL As Odbc.OdbcDataReader
        Dim sqlFindAL As New Odbc.OdbcCommand("SELECT TIMEDIFF(endtime, starttime) FROM annualleave WHERE audiologistid = ? AND personalappointment = 0 AND DATE <= ? AND DATE >= ?", conn)
        sqlFindAL.Parameters.AddWithValue("audiologistid", Me.audiologistID)
        sqlFindAL.Parameters.AddWithValue("date", endDate)
        sqlFindAL.Parameters.AddWithValue("date", startDate)
        rsFindAL = sqlFindAL.ExecuteReader
        While rsFindAL.Read
            tempTime = rsFindAL("timediff(endtime, starttime)")
            If tempTime >= TimeSpan.Parse("12:00:00") Then
                annualLeaveLeft += 1
            Else
                annualLeaveLeft += 0.5
            End If
        End While
    End Sub

    Public Sub FindAnnualLeaveLeft(ByVal conn As System.Data.Odbc.OdbcConnection)
        Dim tempTime As TimeSpan

        Dim rsFindALLeft As Odbc.OdbcDataReader
        Dim sqlFindALLeft As New Odbc.OdbcCommand("SELECT TIMEDIFF(endtime, starttime) FROM annualleave WHERE personalappointment = 0 AND audiologistid = ?", conn)
        sqlFindALLeft.Parameters.AddWithValue("audiologistid", audiologistid)
        rsFindALLeft = sqlFindALLeft.ExecuteReader
        While rsFindALLeft.Read
            temptime = rsFindALLeft("TIMEDIFF(endtime, starttime)")
            If temptime >= TimeSpan.Parse("12:00:00") Then
                annualLeaveLeft -= 1
            Else
                annualleaveleft -= 0.5
            End If
        End While
    End Sub

    Public Function ReturnMaxAppointments()
        Return maxAppointments
    End Function

    Public Sub ChangeName(ByVal fName As String, ByVal sName As String, ByVal conn as System.Data.Odbc.OdbcConnection)
        firstName = fName
        surname = sName
        Dim sqlChangeAudName As New Odbc.OdbcCommand("UPDATE audiologists SET firstName = ? AND surname = ? WHERE audiologistID = ?", conn)
        sqlChangeAudName.Parameters.AddWithValue("firstname", firstName)
        sqlChangeAudName.Parameters.AddWithValue("surname", surname)
        sqlChangeAudName.Parameters.AddWithValue("audiologistid", audiologistID)
        sqlChangeAudName.ExecuteNonQuery()
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("Success. Audiologist name changed to " & firstname & " " & surname)
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

    Public Sub ChangePhoneNumber(ByVal telNum As String, ByVal conn As System.Data.Odbc.OdbcConnection)
        phoneNumber = telNum
        Dim sqlChangeAudTel As New Odbc.OdbcCommand("UPDATE audiologists SET phonenumber = ? WHERE audiologistID = ?", conn)
        sqlChangeAudTel.Parameters.AddWithValue("phonenumber", phoneNumber)
        sqlChangeAudTel.Parameters.AddWithValue("audiologistid", audiologistID)
        sqlChangeAudTel.ExecuteNonQuery()
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("Success. Audiologist phone number changed to " & phoneNumber)
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

    Public Sub ChangeEmail(ByVal userEmail As String, ByVal conn As System.Data.Odbc.OdbcConnection)
        email = userEmail
        Dim sqlChangeAudEmail As New Odbc.OdbcCommand("UPDATE audiologists SET email = ? WHERE audiologistID = ?", conn)
        sqlChangeEmail.Parameters.AddWithValue("email", email)
        sqlChangeEmail.Parameters.AddWithValue("audiologistid", audiologistID)
        sqlChangeEmail.ExecuteNonQuery()
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("Success. Audiologist email changed to " & email)
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

    Public Sub EditWorkingHours()
        workHours.EditWorkingHours()
    End Sub

End Class
