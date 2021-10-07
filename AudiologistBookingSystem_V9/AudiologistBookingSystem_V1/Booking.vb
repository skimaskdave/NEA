Public Class Booking

    Private aud As Audiologist
    Private pat As Patient
    Private day As Date
    Private stringHandling As ErrorHandling

    Public Sub New()
        stringHandling = New ErrorHandling
    End Sub

    Public Sub New(ByVal iPat As Patient)
        pat = iPat
        stringHandling = New ErrorHandling
    End Sub

    Public Sub New(ByVal iPat As Patient, ByVal iAud As Audiologist)
        pat = iPat
        aud = iAud
        stringHandling = New ErrorHandling
    End Sub

    Public Sub New(ByVal iAud As Audiologist)
        aud = iAud
        stringHandling = New ErrorHandling
    End Sub

    Public Sub ShowAllAppointments(ByVal conn As System.Data.Odbc.OdbcConnection)
        Console.Clear()
        Console.WriteLine("Patient appointments on this day:")
        Dim rsShowAllApps As Odbc.OdbcDataReader
        Dim sqlShowAllApps As New Odbc.OdbcCommand("select * from patientbooking where date = '" & stringHandling.SQLDate(day) & "' order by starttime", conn)
        rsShowAllApps = sqlShowAllApps.ExecuteReader
        While rsShowAllApps.Read
            Dim rsFindAudName As Odbc.OdbcDataReader
            Dim sqlFindAudName As New Odbc.OdbcCommand("select firstname, surname from audiologists where audiologistid = " & rsShowAllApps("audiologistid") & "", conn)
            rsFindAudName = sqlFindAudName.ExecuteReader
            If rsFindAudName.Read Then
                Console.WriteLine("Appointment with " & rsFindAudName("firstname") & " " & rsFindAudName("surname") & ": " & rsShowAllApps("starttime").ToString & " - " & rsShowAllApps("endtime").ToString)
            End If
        End While
        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
    End Sub

    Public Sub BookPatient(ByVal conn As System.Data.Odbc.OdbcConnection)
        Dim patientInAppointment As Boolean = True
        Dim t1, t2 As TimeSpan
        Dim appType As String = ChooseAppointmentType()
        Dim dayOfWeek As Integer
        Do Until patientInAppointment = False
            day = stringHandling.GetDate()
            dayOfWeek = day.DayOfWeek
            While dayOfWeek = 0 Or dayOfWeek = 6
                Console.WriteLine("Appointments cannot be on the weekend. Please choose a different date.")
                day = stringHandling.GetDate()
                dayOfWeek = day.DayOfWeek
            End While 'makes sure appointment is between monday and friday

            Select Case ChooseTime()
                Case 1
                    t1 = TimeSpan.Parse("09:15:00")
                Case 2
                    t1 = TimeSpan.Parse("11:30:00")
                Case 3
                    t1 = TimeSpan.Parse("15:00:00")
            End Select 'chooses appointment start time
            t2 = TimeSpan.Parse(GetEndTime(t1, day))
            patientInAppointment = CheckPatientFree(t1, conn)
            If patientInAppointment = True Then
                Console.Clear()
                Console.WriteLine("Patient is already in an appointment at this time. Please choose another date/time.")
                Console.WriteLine("Press any key to continue...")
                Console.ReadKey()
            End If
        Loop

        ShowAllAppointments(conn)
        Console.Clear()
        If YesNo("Would you still like to book this patient appointment?") = True Then
            aud = AudiologistSelection(t1, t2, GetWeekDay(dayOfWeek), conn) 'choose an audiologist that is free
            aud.GetAudiologistInfo(conn)
            Dim child As Integer
            If pat.ReturnChildStatus(day) = True Then
                child = 1
            Else
                child = 0
            End If 'find out if the patient is a child
            Dim rsAppID As Odbc.OdbcDataReader
            Dim sqlAppID As New Odbc.OdbcCommand("select appointmentid from appointment where type = ? and child = ?", conn) 'get appointment id (tells you length, type)
            sqlAppID.Parameters.AddWithValue("@type", appType)
            sqlAppID.Parameters.AddWithValue("@child", child)
            rsAppID = sqlAppID.ExecuteReader
            Dim appID As Integer
            If rsAppID.Read Then
                appID = rsAppID("appointmentid")
            End If
            Dim rsFreeRooms As Odbc.OdbcDataReader
            Dim sqlFreeRooms As New Odbc.OdbcCommand("SELECT room FROM patientbooking WHERE DATE = '" & stringHandling.SQLDate(day) & "' AND starttime = '" & t1.ToString & "'", conn) 'find which rooms are free
            rsFreeRooms = sqlFreeRooms.ExecuteReader
            Dim notFreeRooms As New List(Of String)
            Dim rooms As New List(Of String)
            rooms.Add("Seahorse")
            rooms.Add("Starfish")
            rooms.Add("Dolphin")
            rooms.Add("Coral")
            Dim roomUsed As String = ""
            While rsFreeRooms.Read
                notFreeRooms.Add(rsFreeRooms("room"))
            End While
            Randomize()
            If notFreeRooms.Count = 0 Then
                Select Case Int(Rnd() * 4 + 1)
                    Case 1
                        roomUsed = "Seahorse"
                    Case 2
                        roomUsed = "Starfish"
                    Case 3
                        roomUsed = "Dolphin"
                    Case 4
                        roomUsed = "Coral"
                End Select
            ElseIf notFreeRooms.Count = 4 Then
                Console.WriteLine("No rooms avaliable at this current time.")
            Else
                For Each room In notFreeRooms
                    rooms.Remove(room)
                Next
                Console.Clear()
                Console.WriteLine("Enter:")
                For i = 0 To rooms.Count - 1
                    Console.WriteLine("   " & rooms(i))
                Next
                Console.CursorVisible = False
                Dim currentChoice As Integer = 1
                Dim choice As ConsoleKey
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
                            If currentChoice < rooms.Count Then
                                Console.SetCursorPosition(0, currentChoice)
                                Console.Write("  ")
                                currentChoice += 1
                                Console.SetCursorPosition(0, currentChoice)
                                Console.Write(" >")
                            End If
                    End Select
                Loop Until choice = ConsoleKey.Enter
                Console.CursorVisible = True
                roomUsed = rooms(currentChoice - 1)
            End If 'choose room
            Dim interpreter As Integer = NeedsInterpreter()
            Dim booked As Boolean = True
            Dim sqlBookTogether As New Odbc.OdbcCommand("INSERT INTO patientbooking(audiologistID, patientID, appointmentID, room, DATE, startTime, endTime, interpreter) 
VALUES(" & aud.ReturnAudiologistID & ", " & pat.ReturnPatientID(conn) & ", " & appID & ", '" & roomUsed & "', '" & stringHandling.SQLDate(day) & "', '" & t1.ToString & "', '" & t2.ToString & "', " & interpreter & ")", conn)
            Try
                sqlBookTogether.ExecuteNonQuery()
            Catch ex As Exception
                booked = False
                Console.WriteLine("An error occured: " & ex.Message)
                Console.WriteLine("Press any key to continue...")
                Console.ReadKey()
            End Try
            If booked = True Then
                Console.Clear()
                Console.SetCursorPosition(0, 0)
                Console.ForegroundColor = ConsoleColor.Green
                Console.WriteLine("Booking successful")
                Console.WriteLine("An appointment has been made for " & pat.ReturnPatientName & " with " & aud.ReturnAudiologistName & " between " & t1.ToString & " - " & t2.ToString & " on " & day)
                Console.ForegroundColor = ConsoleColor.Gray
            Else
                Console.Clear()
                Console.SetCursorPosition(0, 0)
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("Booking unsuccessful")
                Console.ForegroundColor = ConsoleColor.Gray
            End If
        End If
        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
    End Sub

    Public Sub BookPatient2(ByVal conn As System.Data.Odbc.OdbcConnection)
        Dim patientInAppointment As Boolean = True
        Dim t1, t2 As TimeSpan
        Dim appType As String = ChooseAppointmentType()
        Dim dayOfWeek As Integer
        Dim audFree As Boolean = False
        While audFree = False Or patientInAppointment = True
            day = stringHandling.GetDate()
            dayOfWeek = day.DayOfWeek
            While dayOfWeek = 0 Or dayOfWeek = 6
                Console.WriteLine("Appointments cannot be on the weekend. Please choose a different date.")
                day = stringHandling.GetDate()
                dayOfWeek = day.DayOfWeek
            End While 'makes sure appointment is between monday and friday

            Select Case ChooseTime()
                Case 1
                    t1 = TimeSpan.Parse("09:15:00")
                Case 2
                    t1 = TimeSpan.Parse("11:30:00")
                Case 3
                    t1 = TimeSpan.Parse("15:00:00")
            End Select 'chooses appointment start time
            t2 = TimeSpan.Parse(GetEndTime(t1, day))
            audFree = CheckAudiologistFree(t1, t2, GetWeekDay(dayOfWeek), conn)
            patientInAppointment = CheckPatientFree(t1, conn)
            If audFree = False Then
                Console.Clear()
                Console.WriteLine("Audiologist is not free at this time. Please choose another date/time.")
                Console.WriteLine("Press any key to continue...")
                Console.ReadKey()
            End If
            If patientInAppointment = True Then
                Console.Clear()
                Console.WriteLine("Patient is already in an appointment at this time, please choose a different date/time.")
                Console.WriteLine("Press any key to continue...")
                Console.ReadKey()
            End If
        End While

        ShowAllAppointments(conn)
        Console.Clear()
        If YesNo("Would you still like to book this patient appointment?") = True Then
            Dim child As Integer
            If pat.ReturnChildStatus(day) = True Then
                child = 1
            Else
                child = 0
            End If 'find out if the patient is a child
            Dim rsAppID As Odbc.OdbcDataReader
            Dim sqlAppID As New Odbc.OdbcCommand("select appointmentid from appointment where type = ? and child = ?", conn) 'get appointment id (tells you length, type)
            sqlAppID.Parameters.AddWithValue("@type", appType)
            sqlAppID.Parameters.AddWithValue("@child", child)
            rsAppID = sqlAppID.ExecuteReader
            Dim appID As Integer
            If rsAppID.Read Then
                appID = rsAppID("appointmentid")
            End If
            Dim rsFreeRooms As Odbc.OdbcDataReader
            Dim sqlFreeRooms As New Odbc.OdbcCommand("SELECT room FROM patientbooking WHERE DATE = '" & stringHandling.SQLDate(day) & "' AND starttime = '" & t1.ToString & "'", conn) 'find which rooms are free
            rsFreeRooms = sqlFreeRooms.ExecuteReader
            Dim notFreeRooms As New List(Of String)
            Dim rooms As New List(Of String)
            rooms.Add("Seahorse")
            rooms.Add("Starfish")
            rooms.Add("Dolphin")
            rooms.Add("Coral")
            Dim roomUsed As String = ""
            While rsFreeRooms.Read
                notFreeRooms.Add(rsFreeRooms("room"))
            End While
            Randomize()
            If notFreeRooms.Count = 0 Then
                Select Case Int(Rnd() * 4 + 1)
                    Case 1
                        roomUsed = "Seahorse"
                    Case 2
                        roomUsed = "Starfish"
                    Case 3
                        roomUsed = "Dolphin"
                    Case 4
                        roomUsed = "Coral"
                End Select
            ElseIf notFreeRooms.Count = 4 Then
                Console.WriteLine("No rooms avaliable at this current time.")
            Else
                For Each room In notFreeRooms
                    rooms.Remove(room)
                Next
                Console.Clear()
                Console.WriteLine("Enter:")
                For i = 0 To rooms.Count - 1
                    Console.WriteLine("   " & rooms(i))
                Next
                Console.CursorVisible = False
                Dim currentChoice As Integer = 1
                Dim choice As ConsoleKey
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
                            If currentChoice < rooms.Count Then
                                Console.SetCursorPosition(0, currentChoice)
                                Console.Write("  ")
                                currentChoice += 1
                                Console.SetCursorPosition(0, currentChoice)
                                Console.Write(" >")
                            End If
                    End Select
                Loop Until choice = ConsoleKey.Enter
                Console.CursorVisible = True
                roomUsed = rooms(currentChoice - 1)
            End If 'choose room
            Dim interpreter As Integer = NeedsInterpreter()
            Dim booked As Boolean = True
            Dim sqlBookTogether As New Odbc.OdbcCommand("INSERT INTO patientbooking(audiologistID, patientID, appointmentID, room, DATE, startTime, endTime, interpreter) 
VALUES(" & aud.ReturnAudiologistID & ", " & pat.ReturnPatientID(conn) & ", " & appID & ", '" & roomUsed & "', '" & stringHandling.SQLDate(day) & "', '" & t1.ToString & "', '" & t2.ToString & "', " & interpreter & ")", conn)
            Try
                sqlBookTogether.ExecuteNonQuery()
            Catch ex As Exception
                booked = False
                Console.WriteLine("An error occured: " & ex.Message)
                Console.WriteLine("Press any key to continue...")
                Console.ReadKey()
            End Try
            If booked = True Then
                Console.Clear()
                Console.SetCursorPosition(0, 0)
                Console.ForegroundColor = ConsoleColor.Green
                Console.WriteLine("Booking successful")
                Console.WriteLine("An appointment has been made for " & pat.ReturnPatientName & " with " & aud.ReturnAudiologistName & " between " & t1.ToString & " - " & t2.ToString & " on " & day)
                Console.ForegroundColor = ConsoleColor.Gray
            Else
                Console.Clear()
                Console.SetCursorPosition(0, 0)
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("Booking unsuccessful")
                Console.ForegroundColor = ConsoleColor.Gray
            End If
        End If
        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
    End Sub

    Public Sub BookPatientUrgent(ByVal conn As System.Data.Odbc.OdbcConnection)
        Dim patientInAppointment As Boolean = True
        Dim stringHandling As New ErrorHandling
        Dim t1, t2 As TimeSpan
        Dim appType As String = ChooseAppointmentType()
        Dim dayOfWeek As Integer
        Do Until patientInAppointment = False
            day = stringHandling.GetDate()
            dayOfWeek = day.DayOfWeek
            While dayOfWeek = 0 Or dayOfWeek = 6
                Console.WriteLine("Appointments cannot be on the weekend. Please choose a different date.")
                day = stringHandling.GetDate()
                dayOfWeek = day.DayOfWeek
            End While 'makes sure appointment is between monday and friday
            Select Case ChooseTime()
                Case 1
                    t1 = TimeSpan.Parse("09:15:00")
                Case 2
                    t1 = TimeSpan.Parse("11:30:00")
                Case 3
                    t1 = TimeSpan.Parse("15:00:00")
            End Select 'chooses appointment start time
            t2 = TimeSpan.Parse(GetEndTime(t1, day))
            patientInAppointment = CheckPatientFree(t1, conn)
            If patientInAppointment = True Then
                Console.Clear()
                Console.WriteLine("Patient is already in an appointment at this time. Please choose another date/time.")
                Console.WriteLine("Press any key to continue...")
                Console.ReadKey()
            End If
        Loop
        aud = AudiologistSelection(t1, t2, GetWeekDay(dayOfWeek), conn) 'choose an audiologist that is free
        aud.GetAudiologistInfo(conn)
        Dim child As Integer
        If pat.ReturnChildStatus(day) = True Then
            child = 1
        Else
            child = 0
        End If 'find out if the patient is a child
        Dim rsAppID As Odbc.OdbcDataReader
        Dim sqlAppID As New Odbc.OdbcCommand("select appointmentid from appointment where type = ? and child = ?", conn) 'get appointment id (tells you length, type)
        sqlAppID.Parameters.AddWithValue("@type", appType)
        sqlAppID.Parameters.AddWithValue("@child", child)
        rsAppID = sqlAppID.ExecuteReader
        Dim appID As Integer
        If rsAppID.Read Then
            appID = rsAppID("appointmentid")
        End If
        Dim rsFreeRooms As Odbc.OdbcDataReader
        Dim sqlFreeRooms As New Odbc.OdbcCommand("SELECT room FROM patientbooking WHERE DATE = '" & stringHandling.SQLDate(day) & "' AND starttime = '" & t1.ToString & "'", conn) 'find which rooms are free
        rsFreeRooms = sqlFreeRooms.ExecuteReader
        Dim notFreeRooms As New List(Of String)
        Dim rooms As New List(Of String)
        rooms.Add("Seahorse")
        rooms.Add("Starfish")
        rooms.Add("Dolphin")
        rooms.Add("Coral")
        Dim roomUsed As String = ""
        While rsFreeRooms.Read
            notFreeRooms.Add(rsFreeRooms("room"))
        End While
        Randomize()
        If notFreeRooms.Count = 0 Then
            Select Case Int(Rnd() * 4 + 1)
                Case 1
                    roomUsed = "Seahorse"
                Case 2
                    roomUsed = "Starfish"
                Case 3
                    roomUsed = "Dolphin"
                Case 4
                    roomUsed = "Coral"
            End Select
        ElseIf notFreeRooms.Count = 4 Then
            Console.WriteLine("No rooms avaliable at this current time.")
        Else
            For Each room In notFreeRooms
                rooms.Remove(room)
            Next
            Console.Clear()
            Console.WriteLine("Enter:")
            For i = 0 To rooms.Count - 1
                Console.WriteLine("   " & rooms(i))
            Next
            Console.CursorVisible = False
            Dim currentChoice As Integer = 1
            Dim choice As ConsoleKey
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
                        If currentChoice < rooms.Count Then
                            Console.SetCursorPosition(0, currentChoice)
                            Console.Write("  ")
                            currentChoice += 1
                            Console.SetCursorPosition(0, currentChoice)
                            Console.Write(" >")
                        End If
                End Select
            Loop Until choice = ConsoleKey.Enter
            Console.CursorVisible = True
            roomUsed = rooms(currentChoice - 1)
        End If 'choose room
        Dim interpreter As Integer = NeedsInterpreter()
        Dim booked As Boolean = True
        Dim sqlBookTogether As New Odbc.OdbcCommand("INSERT INTO patientbooking(audiologistID, patientID, appointmentID, room, DATE, startTime, endTime, interpreter) 
VALUES(" & aud.ReturnAudiologistID & ", " & pat.ReturnPatientID(conn) & ", " & appID & ", '" & roomUsed & "', '" & stringHandling.SQLDate(day) & "', '" & t1.ToString & "', '" & t2.ToString & "', " & interpreter & ")", conn)
        Try
            sqlBookTogether.ExecuteNonQuery()
        Catch ex As Exception
            booked = False
            Console.WriteLine("An error occured: " & ex.Message)
            Console.WriteLine("Press any key to continue...")
            Console.ReadKey()
        End Try
        If booked = True Then
            Console.Clear()
            Console.SetCursorPosition(0, 0)
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("Booking successful")
            Console.WriteLine("An appointment has been made for " & pat.ReturnPatientName & " with " & aud.ReturnAudiologistName & " between " & t1.ToString & " - " & t2.ToString & " on " & day)
            Console.ForegroundColor = ConsoleColor.Gray
            Console.WriteLine("Press any key to continue...")
            Console.ReadKey()
        Else
            Console.Clear()
            Console.SetCursorPosition(0, 0)
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Booking unsuccessful")
            Console.ForegroundColor = ConsoleColor.Gray
            Console.WriteLine("Press any key to continue...")
            Console.ReadKey()
        End If
    End Sub

    Public Sub BookAnnualLeave(ByVal startTime As TimeSpan, ByVal endTime As TimeSpan, ByVal startDate As Date, ByVal endDate As Date, ByVal personalAppointment As Boolean, ByVal conn As System.Data.Odbc.OdbcConnection)
        'choose start date
        'choose end date
        'starttime = 00:00:00 and endtime = 23:59:59 unless specified otherwise.
        'ask if personal appointment
        Dim booked As Boolean = False
        Dim stringHandling As New ErrorHandling
        Console.Clear()
        If startDate = endDate Then 'booking for 1 day
            Dim sqlBookAL As New Odbc.OdbcCommand("INSERT INTO annualleave(audiologistid, DATE, starttime, endtime, personalappointment)
VALUES(" & aud.ReturnAudiologistID & ", '" & stringHandling.SQLDate(startDate) & "', '" & startTime.ToString & "', '" & endTime.ToString & "', " & personalAppointment & ")", conn)
            Try
                booked = True
                sqlBookAL.ExecuteNonQuery()
            Catch ex As Exception
                Console.WriteLine("An error occured: " & ex.Message)
                booked = False
                Console.WriteLine("Press any key to continue...")
                Console.ReadKey()
            End Try
            If booked = True Then
                Dim top, left As Integer
                top = Console.CursorTop
                left = Console.CursorLeft
                Console.WriteLine("Annual leave booked for " & aud.ReturnAudiologistName & " on " & startDate)
                For i = 0 To 9
                    Console.SetCursorPosition(left + i, top)
                    Console.WriteLine(" ")
                Next
            End If
        Else 'booking for more than one day
            Do Until startDate > endDate
                startTime = TimeSpan.Parse("00:00:00")
                endTime = TimeSpan.Parse("23:59:59")
                Dim sqlBookAL As New Odbc.OdbcCommand("INSERT INTO annualleave(audiologistid, DATE, starttime, endtime, personalappointment)
VALUES(" & aud.ReturnAudiologistID & ", '" & stringHandling.SQLDate(startDate) & "', '" & startTime.ToString & "', '" & endTime.ToString & "', " & personalAppointment & ")", conn)
                Try
                    sqlBookAL.ExecuteNonQuery()
                    Console.ForegroundColor = ConsoleColor.Green
                    Console.WriteLine("Success - Annual leave booked on " & startDate.ToString)
                    Console.ForegroundColor = ConsoleColor.Gray
                Catch ex As Exception
                    Console.WriteLine("An error occured: " & ex.Message)
                    Console.WriteLine("Press any key to continue...")
                    Console.ReadKey()
                End Try

                If startDate.DayOfWeek = 6 Then
                    startDate = startDate.AddDays(2)
                ElseIf startDate.DayOfWeek = 5 Then
                    startDate = startDate.AddDays(3)
                Else
                    startDate = startDate.AddDays(1)
                End If
            Loop
        End If 'book the annual leave into the database

        'change database hours remaining
        If personalAppointment = False Then
            Dim hoursOff As TimeSpan = ALHours(startDate, endDate, startTime, endTime)
            Dim oneDay As TimeSpan = New TimeSpan(1, 0, 0, 0)
            Dim newALHours As TimeSpan
            If startDate = endDate Then
                newALHours = ConvertHours(aud.ReturnAnnualLeaveLeft.Subtract(hoursOff))
            Else
                newALHours = ConvertHours(aud.ReturnAnnualLeaveLeft.Subtract(hoursOff.Add(oneDay)))
            End If
            Dim sqlUpdateAudHoursLeft As New Odbc.OdbcCommand("UPDATE audiologists SET annualleaveleft = '" & newALHours.ToString & "' WHERE audiologistid = " & aud.ReturnAudiologistID & "", conn)
            sqlUpdateAudHoursLeft.ExecuteNonQuery()
        End If
    End Sub

    Public Function CheckAudiologistFree(ByVal startTime As TimeSpan, ByVal endTime As TimeSpan, ByVal dayOfWeek As String, ByVal conn As System.Data.Odbc.OdbcConnection) As Boolean
        Dim stringHandling As New ErrorHandling
        Dim startEndDates As Date() = stringHandling.GetStartEndWeekDates(day)
        Dim rsAudiologistSelect As Odbc.OdbcDataReader 'select all audiologists avaliable at this time
        Dim sqlAudiologistSelect As New Odbc.OdbcCommand("SELECT DISTINCT audiologists.*
FROM audiologists
LEFT JOIN patientbooking ON audiologists.audiologistid = patientbooking.audiologistid
LEFT JOIN annualleave ON audiologists.audiologistid = annualleave.audiologistid
LEFT JOIN meetingattendants ON audiologists.audiologistid = meetingattendants.audiologistid
LEFT JOIN repairs ON audiologists.audiologistid = repairs.audiologistid
LEFT JOIN workinghours ON audiologists.audiologistid = workinghours.audiologistid
LEFT JOIN meeting ON meetingattendants.meetingid = meeting.meetingid
WHERE
workinghours.day = '" & dayOfWeek & "' AND workinghours.starttime <= '" & startTime.ToString & "' AND workinghours.endtime >= '" & endTime.ToString & "'

AND (audiologists.audiologistid <> (SELECT repairs.audiologistid FROM repairs WHERE DATE = '" & stringHandling.SQLDate(day) & "') OR (SELECT repairs.audiologistid FROM repairs WHERE DATE = '" & stringHandling.SQLDate(day) & "') IS NULL)

AND (audiologists.audiologistid <> (SELECT annualleave.audiologistid FROM annualleave WHERE annualleave.date = '" & stringHandling.SQLDate(day) & "') 
OR (SELECT annualleave.audiologistid FROM annualleave WHERE annualleave.date = '" & stringHandling.SQLDate(day) & "') IS NULL)

AND (audiologists.audiologistid <> (SELECT meetingattendants.audiologistid FROM meetingattendants, meeting WHERE DATE = '" & stringHandling.SQLDate(day) & "' AND starttime <= '" & startTime.ToString & "' AND endtime >= '" & endTime.ToString & "')
 OR (SELECT meetingattendants.audiologistid FROM meetingattendants, meeting WHERE DATE = '" & stringHandling.SQLDate(day) & "' AND starttime <= '" & startTime.ToString & "' AND endtime >= '" & endTime.ToString & "') IS NULL)

AND (audiologists.audiologistid <> (SELECT patientbooking.audiologistid FROM patientbooking WHERE DATE = '" & stringHandling.SQLDate(day) & "' AND starttime = '" & startTime.ToString & "') 
OR (SELECT patientbooking.audiologistid FROM patientbooking WHERE DATE = '" & stringHandling.SQLDate(day) & "' AND starttime = '" & startTime.ToString & "') IS NULL)
", conn)
        rsAudiologistSelect = sqlAudiologistSelect.ExecuteReader
        Dim auds As New List(Of Audiologist)

        While rsAudiologistSelect.Read
            Dim a1 As New Audiologist(rsAudiologistSelect("firstname"), rsAudiologistSelect("surname"))
            Dim appsCheck As New List(Of Integer)
            Dim rsCheckMaxAppointments As Odbc.OdbcDataReader
            Dim sqlCheckMaxAppointments As New Odbc.OdbcCommand("SELECT COUNT(*) FROM patientbooking
WHERE audiologistid = (SELECT audiologistid FROM audiologists WHERE firstname = '" & rsAudiologistSelect("firstname") & "' AND surname = '" & rsAudiologistSelect("surname") & "')
AND DATE >= '" & stringHandling.SQLDate(startEndDates(0)) & "' AND DATE <= '" & stringHandling.SQLDate(startEndDates(1)) & "'

UNION

SELECT maxappointments FROM audiologists WHERE audiologistid = (SELECT audiologistid FROM audiologists
WHERE firstname = '" & rsAudiologistSelect("firstname") & "' AND surname = '" & rsAudiologistSelect("surname") & "')", conn)
            rsCheckMaxAppointments = sqlCheckMaxAppointments.ExecuteReader
            While rsCheckMaxAppointments.Read
                appsCheck.Add(rsCheckMaxAppointments("count(*)"))
            End While
            If appsCheck.Count > 1 Then 'if the max appointments = the current number of appointments an audiologist has in a week then they cannot have anymore, so they will not be avaliable.
                If appsCheck(0) < appsCheck(1) Then
                    auds.Add(a1)
                    a1.GetAudiologistInfo(conn)
                End If
            End If
        End While


        Dim flag As Boolean
        For i = 0 To auds.Count - 1
            If auds(i).ReturnAudiologistID = aud.ReturnAudiologistID Then
                flag = True
                Exit For
            Else
                flag = False
            End If
        Next
        Return flag
    End Function

    Public Function AudiologistSelection(ByVal startTime As TimeSpan, ByVal endTime As TimeSpan, ByVal dayOfWeek As String, ByVal conn As System.Data.Odbc.OdbcConnection) As Audiologist
        Dim stringHandling As New ErrorHandling
        Dim startEndDates As Date() = stringHandling.GetStartEndWeekDates(day)
        Dim rsAudiologistSelect As Odbc.OdbcDataReader 'select all audiologists avaliable at this time
        Dim sqlAudiologistSelect As New Odbc.OdbcCommand("SELECT DISTINCT audiologists.*
FROM audiologists
LEFT JOIN patientbooking ON audiologists.audiologistid = patientbooking.audiologistid
LEFT JOIN annualleave ON audiologists.audiologistid = annualleave.audiologistid
LEFT JOIN meetingattendants ON audiologists.audiologistid = meetingattendants.audiologistid
LEFT JOIN repairs ON audiologists.audiologistid = repairs.audiologistid
LEFT JOIN workinghours ON audiologists.audiologistid = workinghours.audiologistid
LEFT JOIN meeting ON meetingattendants.meetingid = meeting.meetingid
WHERE
workinghours.day = '" & dayOfWeek & "' AND workinghours.starttime <= '" & startTime.ToString & "' AND workinghours.endtime >= '" & endTime.ToString & "'

AND (audiologists.audiologistid <> (SELECT repairs.audiologistid FROM repairs WHERE DATE = '" & stringHandling.SQLDate(day) & "') OR (SELECT repairs.audiologistid FROM repairs WHERE DATE = '" & stringHandling.SQLDate(day) & "') IS NULL)

AND (audiologists.audiologistid <> (SELECT annualleave.audiologistid FROM annualleave WHERE annualleave.date = '" & stringHandling.SQLDate(day) & "') 
OR (SELECT annualleave.audiologistid FROM annualleave WHERE annualleave.date = '" & stringHandling.SQLDate(day) & "') IS NULL)

AND (audiologists.audiologistid <> (SELECT meetingattendants.audiologistid FROM meetingattendants, meeting WHERE DATE = '" & stringHandling.SQLDate(day) & "' AND starttime <= '" & startTime.ToString & "' AND endtime >= '" & endTime.ToString & "')
 OR (SELECT meetingattendants.audiologistid FROM meetingattendants, meeting WHERE DATE = '" & stringHandling.SQLDate(day) & "' AND starttime <= '" & startTime.ToString & "' AND endtime >= '" & endTime.ToString & "') IS NULL)

AND (audiologists.audiologistid <> (SELECT patientbooking.audiologistid FROM patientbooking WHERE DATE = '" & stringHandling.SQLDate(day) & "' AND starttime = '" & startTime.ToString & "') 
OR (SELECT patientbooking.audiologistid FROM patientbooking WHERE DATE = '" & stringHandling.SQLDate(day) & "' AND starttime = '" & startTime.ToString & "') IS NULL)
", conn)
        rsAudiologistSelect = sqlAudiologistSelect.ExecuteReader
        Dim auds As New List(Of Audiologist)
        While rsAudiologistSelect.Read
            Dim a1 As New Audiologist(rsAudiologistSelect("firstname"), rsAudiologistSelect("surname"))
            Dim appsCheck As New List(Of Integer)
            Dim rsCheckMaxAppointments As Odbc.OdbcDataReader
            Dim sqlCheckMaxAppointments As New Odbc.OdbcCommand("SELECT COUNT(*) FROM patientbooking
WHERE audiologistid = (SELECT audiologistid FROM audiologists WHERE firstname = '" & rsAudiologistSelect("firstname") & "' AND surname = '" & rsAudiologistSelect("surname") & "')
AND DATE >= '" & stringHandling.SQLDate(startEndDates(0)) & "' AND DATE <= '" & stringHandling.SQLDate(startEndDates(1)) & "'

UNION

SELECT maxappointments FROM audiologists WHERE audiologistid = (SELECT audiologistid FROM audiologists
WHERE firstname = '" & rsAudiologistSelect("firstname") & "' AND surname = '" & rsAudiologistSelect("surname") & "')", conn)
            rsCheckMaxAppointments = sqlCheckMaxAppointments.ExecuteReader
            While rsCheckMaxAppointments.Read
                appsCheck.Add(rsCheckMaxAppointments("count(*)"))
            End While
            If appsCheck.Count > 1 Then 'if the max appointments = the current number of appointments an audiologist has in a week then they cannot have anymore, so they will not be avaliable.
                If appsCheck(0) < appsCheck(1) Then
                    auds.Add(a1)
                End If
            End If
        End While
        Console.Clear()
        Console.WriteLine("Enter:")
        For i = 0 To auds.Count - 1
            Console.WriteLine("   " & auds(i).ReturnAudiologistName)
        Next

        Console.CursorVisible = False
        Dim currentChoice As Integer = 1
        Dim choice As ConsoleKey
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
                    If currentChoice < auds.Count Then
                        Console.SetCursorPosition(0, currentChoice)
                        Console.Write("  ")
                        currentChoice += 1
                        Console.SetCursorPosition(0, currentChoice)
                        Console.Write(" >")
                    End If
            End Select
        Loop Until choice = ConsoleKey.Enter
        Console.CursorVisible = True
        Return auds(currentChoice - 1)
    End Function

    Public Function RandomAudSelection(ByVal startTime As TimeSpan, ByVal endTime As TimeSpan, ByVal dayOfWeek As String, ByVal reps As Boolean, ByVal conn As System.Data.Odbc.OdbcConnection) As Audiologist
        Dim stringHandling As New ErrorHandling
        Dim startEndDates As Date() = stringHandling.GetStartEndWeekDates(day)
        Dim rsAudiologistSelect As Odbc.OdbcDataReader 'select all audiologists avaliable at this time
        Dim sqlAudiologistSelect As New Odbc.OdbcCommand("SELECT DISTINCT audiologists.*
FROM audiologists
LEFT JOIN patientbooking ON audiologists.audiologistid = patientbooking.audiologistid
LEFT JOIN annualleave ON audiologists.audiologistid = annualleave.audiologistid
LEFT JOIN meetingattendants ON audiologists.audiologistid = meetingattendants.audiologistid
LEFT JOIN repairs ON audiologists.audiologistid = repairs.audiologistid
LEFT JOIN workinghours ON audiologists.audiologistid = workinghours.audiologistid
LEFT JOIN meeting ON meetingattendants.meetingid = meeting.meetingid
WHERE
workinghours.day = '" & dayOfWeek & "' AND workinghours.starttime <= '" & startTime.ToString & "' AND workinghours.endtime >= '" & endTime.ToString & "'

AND (audiologists.audiologistid <> (SELECT repairs.audiologistid FROM repairs WHERE DATE = '" & stringHandling.SQLDate(day) & "') OR (SELECT repairs.audiologistid FROM repairs WHERE DATE = '" & stringHandling.SQLDate(day) & "') IS NULL)

AND (audiologists.audiologistid <> (SELECT annualleave.audiologistid FROM annualleave WHERE annualleave.date = '" & stringHandling.SQLDate(day) & "') 
OR (SELECT annualleave.audiologistid FROM annualleave WHERE annualleave.date = '" & stringHandling.SQLDate(day) & "') IS NULL)

AND (audiologists.audiologistid <> (SELECT meetingattendants.audiologistid FROM meetingattendants, meeting WHERE DATE = '" & stringHandling.SQLDate(day) & "' AND starttime <= '" & startTime.ToString & "' AND endtime >= '" & endTime.ToString & "')
 OR (SELECT meetingattendants.audiologistid FROM meetingattendants, meeting WHERE DATE = '" & stringHandling.SQLDate(day) & "' AND starttime <= '" & startTime.ToString & "' AND endtime >= '" & endTime.ToString & "') IS NULL)

AND (audiologists.audiologistid <> (SELECT patientbooking.audiologistid FROM patientbooking WHERE DATE = '" & stringHandling.SQLDate(day) & "' AND starttime = '" & startTime.ToString & "') 
OR (SELECT patientbooking.audiologistid FROM patientbooking WHERE DATE = '" & stringHandling.SQLDate(day) & "' AND starttime = '" & startTime.ToString & "') IS NULL)
", conn)
        rsAudiologistSelect = sqlAudiologistSelect.ExecuteReader
        Dim auds As New List(Of Audiologist)
        While rsAudiologistSelect.Read
            Dim a1 As New Audiologist(rsAudiologistSelect("firstname"), rsAudiologistSelect("surname"))
            If reps = False Then
                Dim appsCheck As New List(Of Integer)
                Dim rsCheckMaxAppointments As Odbc.OdbcDataReader
                Dim sqlCheckMaxAppointments As New Odbc.OdbcCommand("SELECT COUNT(*) FROM patientbooking
WHERE audiologistid = (SELECT audiologistid FROM audiologists WHERE firstname = '" & rsAudiologistSelect("firstname") & "' AND surname = '" & rsAudiologistSelect("surname") & "')
AND DATE >= '" & stringHandling.SQLDate(startEndDates(0)) & "' AND DATE <= '" & stringHandling.SQLDate(startEndDates(1)) & "'

UNION

SELECT maxappointments FROM audiologists WHERE audiologistid = (SELECT audiologistid FROM audiologists
WHERE firstname = '" & rsAudiologistSelect("firstname") & "' AND surname = '" & rsAudiologistSelect("surname") & "')", conn)
                rsCheckMaxAppointments = sqlCheckMaxAppointments.ExecuteReader
                While rsCheckMaxAppointments.Read
                    appsCheck.Add(rsCheckMaxAppointments("count(*)"))
                End While
                If appsCheck.Count > 1 Then 'if the max appointments = the current number of appointments an audiologist has in a week then they cannot have anymore, so they will not be avaliable.
                    If appsCheck(0) < appsCheck(1) Then
                        auds.Add(a1)
                    End If
                End If
            Else
                auds.Add(a1)
            End If
        End While
        Return auds(Int(Rnd() * (auds.Count - 1)))
    End Function

    Public Function CheckPatientFree(ByVal startTime As TimeSpan, ByVal conn As System.Data.Odbc.OdbcConnection) As Boolean
        Dim patientInApp As Boolean = False

        Dim rsCheckPatientFree As Odbc.OdbcDataReader
        Dim sqlCheckPatientFree As New Odbc.OdbcCommand("SELECT COUNT(*) FROM patientbooking
WHERE patientid = ? AND DATE = ? AND starttime = ?", conn)
        sqlCheckPatientFree.Parameters.AddWithValue("@patientid", pat.ReturnPatientID(conn))
        sqlCheckPatientFree.Parameters.AddWithValue("@date", day)
        sqlCheckPatientFree.Parameters.AddWithValue("@starttime", startTime.ToString)
        rsCheckPatientFree = sqlCheckPatientFree.ExecuteReader
        If rsCheckPatientFree.Read Then
            If rsCheckPatientFree("count(*)") > 0 Then
                patientInApp = True
            End If
        End If
        Return patientInApp
    End Function

    Public Function ChooseAppointmentType() As String
        Console.CursorVisible = False
        Dim currentChoice As Integer = 1
        Dim choice As ConsoleKey
        Console.Clear()
        Console.WriteLine("Enter:
   Assessment
   Review
   Tuning
   Implant Test
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
                    If currentChoice < 4 Then
                        Console.SetCursorPosition(0, currentChoice)
                        Console.Write("  ")
                        currentChoice += 1
                        Console.SetCursorPosition(0, currentChoice)
                        Console.Write(" >")
                    End If
            End Select
        Loop Until choice = ConsoleKey.Enter
        Console.CursorVisible = True
        Select Case currentChoice
            Case 1
                Return "Assessment"
            Case 2
                Return "Review"
            Case 3
                Return "Tuning"
            Case Else
                Return "Implant Test"
        End Select
    End Function

    Public Function ChooseTime() As Integer
        Console.CursorVisible = False
        Dim currentChoice As Integer = 1
        Dim choice As ConsoleKey
        Console.Clear()
        Console.WriteLine("Enter:
   09:15
   11:30
   15:00
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
                    If currentChoice < 3 Then
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
    End Function

    Public Function YesNo(ByVal message As String) As Boolean
        Console.CursorVisible = False
        Dim currentChoice As Integer = 1
        Dim choice As ConsoleKey
        Console.Clear()
        Console.WriteLine(message & ":
   Yes
   No
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
        Select Case currentChoice
            Case 1
                Return True
            Case 2
                Return False
        End Select
        Return False
    End Function

    Public Function GetEndTime(ByVal t1 As TimeSpan, ByVal appdate As Date) As String
        Select Case t1.ToString
            Case "09:15:00"
                Return "10:45:00"
            Case "11:30:00"
                Select Case pat.ReturnChildStatus(appdate)
                    Case True
                        Return "13:30:00"
                    Case False
                        Return "13:00:00"
                End Select
            Case "15:00:00"
                Select Case pat.ReturnChildStatus(appdate)
                    Case True
                        Return "17:00:00"
                    Case False
                        Return "16:30:00"
                End Select
            Case Else
                Select Case pat.ReturnChildStatus(appdate)
                    Case True
                        Return "17:00:00"
                    Case False
                        Return "16:30:00"
                End Select
        End Select
        Return False
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

    Public Function NeedsInterpreter() As Integer
        Console.CursorVisible = False
        Dim currentChoice As Integer = 1
        Dim choice As ConsoleKey
        Console.Clear()
        Console.WriteLine("Does the patient need an interpreter?
   Yes
   No
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
        Select Case currentChoice
            Case 1
                Return 1
            Case 2
                Return 0
        End Select
        Return 0
    End Function

    Public Function CheckAnnualLeaveCanHappen(ByVal startDate As Date, ByVal endDate As Date, ByVal startTime As TimeSpan, ByVal endTime As TimeSpan, ByVal conn As System.Data.Odbc.OdbcConnection) As Boolean
        'check hours off
        'check number of audiologists off at the same time
        Dim stringHandling As New ErrorHandling
        Dim ALCanHappen As Boolean = True
        Dim hoursAvailable As TimeSpan
        Dim hoursOff As TimeSpan
        hoursOff = ALHours(startDate, endDate, startTime, endTime)
        hoursAvailable = aud.ReturnAnnualLeaveLeft()
        If hoursAvailable < hoursOff Then
            ALCanHappen = False
            Console.WriteLine("You cannot book this annual leave as you do not have enough annual leave left to book it.")
        End If

        Dim numOfAuds As Integer
        Dim numOfAudsOff As Integer
        Dim rsCheckAuds As Odbc.OdbcDataReader
        Dim sqlCheckAuds As New Odbc.OdbcCommand("Select count(*) from audiologists", conn)
        rsCheckAuds = sqlCheckAuds.ExecuteReader
        If rsCheckAuds.Read Then
            numOfAuds = rsCheckAuds("COUNT(*)")
        End If
        Dim rsCheckAudsOff As Odbc.OdbcDataReader
        Dim sqlCheckAudsOff As New Odbc.OdbcCommand("SELECT DISTINCT count(audiologistid) FROM annualleave WHERE DATE >= '" & stringHandling.SQLDate(startDate) & "' AND DATE <= '" & stringHandling.SQLDate(endDate) & "'", conn)
        rsCheckAudsOff = sqlCheckAudsOff.ExecuteReader
        If rsCheckAudsOff.Read Then
            numOfAudsOff = rsCheckAudsOff("count(audiologistid)")
        End If

        If numOfAuds - numOfAudsOff < 5 Then
            ALCanHappen = False
            Console.WriteLine("You cannot book this annual leave as there are not enough audiologists available during this time.")
        End If

        Return ALCanHappen
    End Function

    Public Function ALHours(ByVal startDate As Date, ByVal endDate As Date, ByVal startTime As TimeSpan, ByVal endTime As TimeSpan) As TimeSpan
        Dim hoursOff As TimeSpan
        Dim tempHours As TimeSpan
        If startDate <> endDate Then
            For i = 0 To DateDiff(DateInterval.Day, endDate, startDate) - 1
                startDate = startDate.AddDays(i)
                tempHours = aud.ReturnHoursForDay(GetWeekDay(startDate.DayOfWeek))
                hoursOff = hoursOff.Add(tempHours)
            Next
        Else
            If startTime.ToString = "00:00:00" And endTime.ToString = "23:59:59" Then
                hoursOff = aud.ReturnHoursForDay(GetWeekDay(startDate.DayOfWeek))
            Else
                hoursOff = endTime.Subtract(startTime)
            End If
        End If
        Return hoursOff
    End Function

    Public Function ConvertHours(ByVal hours As TimeSpan) As TimeSpan
        Dim hoursSplit As String()
        hoursSplit = hours.ToString.Split(":")
        Dim newHours As TimeSpan = New TimeSpan(hoursSplit(0), hoursSplit(1), hoursSplit(2))
        Return newhours
    End Function

End Class
