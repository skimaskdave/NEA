Module Module1
    Dim conn As New System.Data.Odbc.OdbcConnection("DRIVER={MySQL ODBC 5.3 ANSI Driver};SERVER=localhost;PORT=3306;DATABASE=audiology;USER=root;PASSWORD=root;OPTION=3;")
    Sub Main()
        conn.Open()
        Console.WriteLine("++++++++++++++++++++++++++++")
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("  Audiology Booking System  ")
        Console.ForegroundColor = ConsoleColor.Gray
        Console.WriteLine("++++++++++++++++++++++++++++")
        Console.WriteLine("
Press any key to continue...")
        Console.ReadKey()
        Menu()
    End Sub

    'selection section
    Sub Menu() 'select what user wants to do
        Dim selection As ConsoleKey
        While selection <> ConsoleKey.NumPad1 And selection <> ConsoleKey.NumPad2 And selection <> ConsoleKey.NumPad3 And selection <> ConsoleKey.NumPad4 And selection <> ConsoleKey.NumPad0 And selection <> ConsoleKey.D1 And selection <> ConsoleKey.D2 And selection <> ConsoleKey.D3 And selection <> ConsoleKey.D4 And selection <> ConsoleKey.D0
            Console.Clear()
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("Main Menu")
            Console.ForegroundColor = ConsoleColor.Gray
            Console.WriteLine("
1. Booking (patients, meetings, repairs, annual leave, personal appointment)
2. Check Timetable (for audiologists)
3. Search (meetings, repairs, appointments)
4. Other (edit information, add new patients/audiologists)
0. Exit")
            selection = Console.ReadKey(True).Key

            Select Case selection
                Case ConsoleKey.NumPad1, ConsoleKey.D1
                    Booking()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad2, ConsoleKey.D2
                    CheckTimetable()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad3, ConsoleKey.D3
                    Search()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad4, ConsoleKey.D4
                    Other()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad0, ConsoleKey.D0
                    Console.WriteLine("See you later!")
                    System.Threading.Thread.Sleep(500)
            End Select
        End While
    End Sub

    'booking section
    Sub Booking()
        Dim selection As ConsoleKey
        While selection <> ConsoleKey.NumPad1 And selection <> ConsoleKey.NumPad2 And selection <> ConsoleKey.NumPad3 And selection <> ConsoleKey.NumPad4 And selection <> ConsoleKey.NumPad0 And selection <> ConsoleKey.D1 And selection <> ConsoleKey.D2 And selection <> ConsoleKey.D3 And selection <> ConsoleKey.D4 And selection <> ConsoleKey.D0
            Console.Clear()
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine("Booking")
            Console.ForegroundColor = ConsoleColor.Gray
            Console.WriteLine("
1. Patient Booking (specific audiologist)
2. Patient Booking (non specific audiologist)
3. Urgent Patient Booking
4. Annual Leave Booking (or personal appointment)
5. Repairs Booking
6. Meeting Booking
0. Back")
            selection = Console.ReadKey(True).Key

            Select Case selection
                Case ConsoleKey.NumPad1, ConsoleKey.D1
                    BookPatient()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad2, ConsoleKey.D2
                    BookPatient2()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad3, ConsoleKey.D3
                    BookUrgentPatient()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad4, ConsoleKey.D4
                    BookAnnualLeave()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad5, ConsoleKey.D5
                    BookRepairs()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad6, ConsoleKey.D6
                    BookMeeting()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad0, ConsoleKey.D0
                    Console.WriteLine("Going back...")
                    System.Threading.Thread.Sleep(500)
            End Select
        End While
    End Sub

    Sub BookPatient() 'specific audiologist
        Console.Clear()
        Dim flag As Boolean = False
        Dim stringCheck As New ErrorHandling()

        'create patient
        Dim fName, sName As String
        Console.Write("Enter patient first name: ")
        fName = stringCheck.TryString(1).ToUpper
        Console.Write("Enter patient surname: ")
        sName = stringCheck.TryString(1).ToUpper

        Dim pat As New Patient(fName, sName)
        pat.GetPatientInfo(pat.CreateOrSearch, conn)
        pat.PrintHistory(conn)

        'create audiologist
        Do Until flag = True
            Console.Write("Enter audiologist first name: ")
            fName = stringCheck.TryString(1).ToUpper
            Console.Write("Enter audiologist surname: ")
            sName = stringCheck.TryString(1).ToUpper
            Dim tryAud As New Audiologist(fName, sName)
            flag = tryAud.GetAudiologistInfo(conn)
        Loop

        Dim aud As New Audiologist(fName, sName)
        aud.GetAudiologistInfo(conn)

        'create instance of booking class
        Dim bookPatient As New Booking(pat, aud)
        bookPatient.BookPatient2(conn)

    End Sub

    Sub BookPatient2() 'not specific audiologist
        Console.Clear()
        Dim fName, sName As String
        Dim flag As Boolean = False
        Dim stringCheck As New ErrorHandling()
        fName = ""
        sName = ""

        Console.Write("Enter patient first name: ")
        fName = stringCheck.TryString(1).ToUpper
        Console.Write("Enter patient surname: ")
        sName = stringCheck.TryString(1).ToUpper

        Dim pat As New Patient(fName, sName)
        pat.GetPatientInfo(pat.CreateOrSearch, conn)
        pat.PrintHistory(conn)

        Dim bookPatient As New Booking(pat)
        bookPatient.BookPatient(conn)
    End Sub

    Sub BookUrgentPatient()
        Console.Clear()
        Dim fName, sName As String
        Dim flag As Boolean = False
        Dim stringCheck As New ErrorHandling()
        fName = ""
        sName = ""

        Console.Write("Enter patient first name: ")
        fName = stringCheck.TryString(1).ToUpper
        Console.Write("Enter patient surname: ")
        sName = stringCheck.TryString(1).ToUpper

        Dim pat As New Patient(fName, sName)
        pat.GetPatientInfo(pat.CreateOrSearch, conn)
        pat.PrintHistory(conn)

        Dim bookPatient As New Booking(pat)
        bookPatient.BookPatient(conn)
    End Sub

    Sub BookAnnualLeave()
        Console.Clear()
        Dim fName, sName As String
        fName = ""
        sName = ""
        Dim flag As Boolean = False
        Dim stringHandling As New ErrorHandling

        Do Until flag = True
            Console.Write("Enter audiologist first name: ")
            fName = stringHandling.TryString(1).ToUpper
            Console.Write("Enter audiologist surname: ")
            sName = stringHandling.TryString(1).ToUpper
            Dim tryAud As New Audiologist(fName, sName)
            flag = tryAud.GetAudiologistInfo(conn)
        Loop

        Dim aud As New Audiologist(fName, sName)
        aud.GetAudiologistInfo(conn)
        Dim alBooking As New Booking(aud)

        Dim startDate, endDate As Date
        Dim startTime, endTime As TimeSpan
        Dim allDay As Boolean = True
        Dim personalAppointment As Integer
        startTime = TimeSpan.Parse("00:00:00")
        endTime = TimeSpan.Parse("23:59:59")

        Console.WriteLine("Enter start date of annual leave/personal appointment: ")
        System.Threading.Thread.Sleep(1000)
        startDate = stringHandling.GetDateAnnualLeave
        Console.WriteLine("Enter end date of annual leave/personal appointment: ")
        System.Threading.Thread.Sleep(1000)
        endDate = stringHandling.GetDateAnnualLeave
        Do Until startDate.DayOfWeek <> 0 And startDate.DayOfWeek <> 0 And endDate.DayOfWeek <> 6 And endDate.DayOfWeek <> 0
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("** Start dates and end dates cannot be booked on weekends! **")
            Console.ForegroundColor = ConsoleColor.Gray
            Console.WriteLine("Enter start date of annual leave/personal appointment: ")
            System.Threading.Thread.Sleep(1000)
            startDate = stringHandling.GetDateAnnualLeave
            Console.WriteLine("Enter end date of annual leave/personal appointment: ")
            System.Threading.Thread.Sleep(1000)
            endDate = stringHandling.GetDateAnnualLeave
        Loop 'get start/end date

        If startDate = endDate Then
            allDay = alBooking.YesNo("Is your annual leave all day?")
            If allDay = True Then
                startTime = TimeSpan.Parse("00:00:00")
                endTime = TimeSpan.Parse("23:59:59")
            Else
                Console.WriteLine("Enter start time: ")
                stringHandling.GetTime()
                Console.WriteLine("Enter end time: ")
                stringHandling.GetTime()
            End If
        End If 'get start/end times
        Console.Clear()
        Select Case alBooking.YesNo("Are you booking a personal appointment: ")
            Case True
                personalAppointment = 1
            Case False
                personalAppointment = 0
        End Select
        Console.Clear()
        If alBooking.CheckAnnualLeaveCanHappen(startDate, endDate, startTime, endTime, conn) = True Then
            alBooking.BookAnnualLeave(startTime, endTime, startDate, endDate, personalAppointment, conn)
            're-assign repairs/appointments & cancel meetings
            aud.CancelMeeting(startTime, endTime, startDate, endDate, conn)
            aud.RearrangeRepairs(startTime, endTime, startDate, endDate, conn)
            aud.RearrangeAppointment(startTime, endTime, startDate, endDate, conn)
        Else
            Console.WriteLine("You cannot book annual leave at this time.")
        End If

        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
    End Sub

    Sub BookRepairs()
        'repairs starts at the earliest 09:05:00
        'runs all day
        'if they have 1 meeting less than 3 hours then they can be booked for repairs
        'repairs booked 3 weeks in advance
        Console.Clear()
        Dim repBooking As New Booking()
        repBooking.BookRepairs(conn)
    End Sub

    Sub BookMeeting()
        'meetings cannot start until 09:05:00
        Console.Clear()
        Dim meetingBooking As New Booking()
        meetingBooking.BookMeeting(conn)
    End Sub

    'check timetable section
    Sub CheckTimetable()
        Dim selection As ConsoleKey
        While selection <> ConsoleKey.NumPad1 And selection <> ConsoleKey.NumPad2 And selection <> ConsoleKey.NumPad3 And selection <> ConsoleKey.NumPad4 And selection <> ConsoleKey.NumPad0 And selection <> ConsoleKey.D1 And selection <> ConsoleKey.D2 And selection <> ConsoleKey.D3 And selection <> ConsoleKey.D4 And selection <> ConsoleKey.D0
            Console.Clear()
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine("Check Timetable Menu")
            Console.ForegroundColor = ConsoleColor.Gray
            Console.WriteLine("
1. Check Day Timetable
2. Check Week Timetable
0. Exit")
            selection = Console.ReadKey(True).Key

            Select Case selection
                Case ConsoleKey.NumPad1, ConsoleKey.D1
                    CheckTimetableDay()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad2, ConsoleKey.D2
                    CheckTimetableWeek()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad0, ConsoleKey.D0
                    Console.WriteLine("Going back...")
                    System.Threading.Thread.Sleep(500)
            End Select
        End While
    End Sub

    Sub CheckTimetableDay()
        Console.Clear()
        Console.ForegroundColor = ConsoleColor.Yellow
        Console.WriteLine("Check Timetable")
        Console.ForegroundColor = ConsoleColor.Gray
        Console.WriteLine()

        Dim flag As Boolean = False
        Dim fName, sName As String
        fName = ""
        sName = ""
        Dim stringHandling As New ErrorHandling
        'create audiologist
        Do Until flag = True
            Console.Write("Enter audiologist first name: ")
            fName = stringHandling.TryString(1).ToUpper
            Console.Write("Enter audiologist surname: ")
            sName = stringHandling.TryString(1).ToUpper
            Dim tryAud As New Audiologist(fName, sName)
            flag = tryAud.GetAudiologistInfo(conn)
        Loop

        Dim aud As New Audiologist(fName, sName)
        aud.GetAudiologistInfo(conn)
        Dim day As Date = stringHandling.GetDateTimetable
        Console.Clear()
        aud.GetAudiologistTimetable(day, conn)
    End Sub

    Sub CheckTimetableWeek()
        Console.Clear()
        Console.ForegroundColor = ConsoleColor.Yellow
        Console.WriteLine("Check Timetable")
        Console.ForegroundColor = ConsoleColor.Gray
        Console.WriteLine()

        Dim flag As Boolean = False
        Dim fName, sName As String
        fName = ""
        sName = ""
        Dim stringHandling As New ErrorHandling
        'create audiologist
        Do Until flag = True
            Console.Write("Enter audiologist first name: ")
            fName = stringHandling.TryString(1).ToUpper
            Console.Write("Enter audiologist surname: ")
            sName = stringHandling.TryString(1).ToUpper
            Dim tryAud As New Audiologist(fName, sName)
            flag = tryAud.GetAudiologistInfo(conn)
        Loop

        Dim aud As New Audiologist(fName, sName)
        aud.GetAudiologistInfo(conn)
        Console.WriteLine("Please enter any date from within the week you want to check (Monday - Friday)")
        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
        Dim day As Date = stringHandling.GetDateTimetable
        Console.Clear()
        aud.GetAudiologistTimetableWeek(day, conn)
    End Sub

    'search section
    Sub Search()
        Dim selection As ConsoleKey
        While selection <> ConsoleKey.NumPad1 And selection <> ConsoleKey.NumPad2 And selection <> ConsoleKey.NumPad3 And selection <> ConsoleKey.NumPad4 And selection <> ConsoleKey.NumPad0 And selection <> ConsoleKey.D1 And selection <> ConsoleKey.D2 And selection <> ConsoleKey.D3 And selection <> ConsoleKey.D4 And selection <> ConsoleKey.D0
            Console.Clear()
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine("Search Menu")
            Console.ForegroundColor = ConsoleColor.Gray
            Console.WriteLine("
1. Search Appointments
2. Seach Audiologists
3. Search Patients
4. Search Annual Leave
5. Search Repairs
6. Search Meetings
0. Exit")
            selection = Console.ReadKey(True).Key

            Select Case selection
                Case ConsoleKey.NumPad1, ConsoleKey.D1
                    SearchAppointments()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad2, ConsoleKey.D2
                    SearchAudiologists()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad3, ConsoleKey.D3
                    SearchPatients()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad4, ConsoleKey.D4
                    SearchAnnualLeave()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad5, ConsoleKey.D5
                    SearchRepairs()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad6, ConsoleKey.D6
                    SearchMeetings()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad0, ConsoleKey.D0
                    Console.WriteLine("Going back...")
                    System.Threading.Thread.Sleep(500)
            End Select
        End While
    End Sub

    Public Sub SearchAppointments()
        Dim stringHandling As New ErrorHandling
        Console.Clear()
        Dim dateAudPatRoomChosen As Integer = 0
        Do Until dateAudPatRoomChosen = 1 Or dateAudPatRoomChosen = 2 Or dateAudPatRoomChosen = 3 Or dateAudPatRoomChosen = 4
            dateAudPatRoomChosen = DateAudPatRoom()
        Loop

        Select Case dateAudPatRoomChosen
            Case 1 'date
                Console.Clear()
                Dim appDate As Date = stringHandling.GetDate3

                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("APPOINTMENTS - " & stringHandling.SQLDate(appDate))
                Console.ForegroundColor = ConsoleColor.Gray

                Dim rsAppsCount As Odbc.OdbcDataReader
                Dim sqlAppsCount As New Odbc.OdbcCommand("SELECT COUNT(*) FROM patientbooking WHERE DATE = ?", conn)
                sqlAppsCount.Parameters.AddWithValue("date", stringHandling.SQLDate(appDate))
                rsAppsCount = sqlAppsCount.ExecuteReader

                If rsAppsCount.Read Then
                    Console.WriteLine("Number of appointments: " & rsAppsCount("count(*)"))
                End If

                Dim rsGetApps As Odbc.OdbcDataReader
                Dim sqlGetApps As New Odbc.OdbcCommand("SELECT * FROM patientbooking WHERE DATE = ?", conn)
                sqlGetApps.Parameters.AddWithValue("date", stringHandling.SQLDate(appDate))
                PrintAppointment(rsGetApps, sqlGetApps, stringHandling)

            Case 2 'audiologist
                Console.Clear()
                Dim flag As Boolean = False
                Dim fName, sName As String
                fName = ""
                sName = ""
                Do Until flag = True
                    Console.Write("Enter audiologist first name: ")
                    fName = stringHandling.TryString(1).ToUpper
                    Console.Write("Enter audiologist surname: ")
                    sName = stringHandling.TryString(1).ToUpper
                    Dim tryAud As New Audiologist(fName, sName)
                    flag = tryAud.GetAudiologistInfo(conn)
                Loop

                Dim aud As New Audiologist(fName, sName)
                aud.GetAudiologistInfo(conn)

                Console.Clear()
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("APPOINTMENTS - " & aud.ReturnAudiologistName)
                Console.ForegroundColor = ConsoleColor.Gray

                Dim rsGetApps As Odbc.OdbcDataReader
                Dim sqlGetApps As New Odbc.OdbcCommand("SELECT * FROM patientbooking WHERE audiologistid = ? AND DATE >= ?", conn)
                sqlGetApps.Parameters.AddWithValue("audiologistid", aud.ReturnAudiologistID)
                sqlGetApps.Parameters.AddWithValue("date", stringHandling.SQLDate(Date.Today))
                PrintAppointment(rsGetApps, sqlGetApps, stringHandling)

            Case 3 'patient
                Console.Clear()
                Dim checkPat As Boolean
                Dim fName, sName As String
                fName = ""
                sName = ""
                Do Until checkPat = True
                    Console.Write("Enter patient first name: ")
                    fName = stringHandling.TryString(1).ToUpper
                    Console.Write("Enter patient surname: ")
                    sName = stringHandling.TryString(1).ToUpper
                    Dim patTry As New Patient(fName, sName)
                    checkPat = patTry.CheckPatient(conn)
                Loop

                Dim pat As New Patient(fName, sName)
                pat.GetPatientInfo(2, conn)

                Console.Clear()
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("APPOINTMENTS - " & pat.ReturnPatientName)
                Console.ForegroundColor = ConsoleColor.Gray

                Dim rsGetApps As Odbc.OdbcDataReader
                Dim sqlGetApps As New Odbc.OdbcCommand("SELECT * FROM patientbooking WHERE patientid = ? AND DATE >= ?", conn)
                sqlGetApps.Parameters.AddWithValue("patientid", pat.ReturnPatientID(conn))
                sqlGetApps.Parameters.AddWithValue("date", stringHandling.SQLDate(Date.Today))
                PrintAppointment(rsGetApps, sqlGetApps, stringHandling)

            Case 4 'room
                Console.Clear()
                Dim room As String = ChooseRoom()

                Console.Clear()
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("APPOINTMENTS - " & room)
                Console.ForegroundColor = ConsoleColor.Gray

                Dim rsGetApps As Odbc.OdbcDataReader
                Dim sqlGetApps As New Odbc.OdbcCommand("SELECT * FROM patientbooking WHERE room = ? AND DATE >= ?", conn)
                sqlGetApps.Parameters.AddWithValue("room", room)
                sqlGetApps.Parameters.AddWithValue("date", stringHandling.SQLDate(Date.Today))
                PrintAppointment(rsGetApps, sqlGetApps, stringHandling)

        End Select
        Console.WriteLine("
Press any key to continue...")
        Console.ReadKey()
    End Sub

    Public Sub PrintAppointment(ByVal reader As Odbc.OdbcDataReader, ByVal command As Odbc.OdbcCommand, ByVal stringHandling As ErrorHandling)
        reader = command.ExecuteReader

        While reader.Read
            Console.WriteLine()
            'get audiologist name
            Dim rsGetAudName As Odbc.OdbcDataReader
            Dim sqlGetAudName As New Odbc.OdbcCommand("select firstname, surname from audiologists where audiologistid = ?", conn)
            sqlGetAudName.Parameters.AddWithValue("audiologistid", reader("audiologistid"))
            rsGetAudName = sqlGetAudName.ExecuteReader

            If rsGetAudName.Read Then
                Console.WriteLine("Audiologist: " & rsGetAudName("firstname") & " " & rsGetAudName("surname"))
            End If

            'get patient name & age
            Dim rsGetPat As Odbc.OdbcDataReader
            Dim sqlGetPat As New Odbc.OdbcCommand("select firstname, surname, dob from patients where patientid = ?", conn)
            sqlGetPat.Parameters.AddWithValue("patientid", reader("patientid"))
            rsGetPat = sqlGetPat.ExecuteReader

            If rsGetPat.Read Then
                Console.WriteLine("Patient: " & rsGetPat("firstname") & " " & rsGetPat("surname"))
                Console.WriteLine("Age: " & DateDiff(DateInterval.Year, rsGetPat("dob"), Date.Today) & " (" & stringHandling.SQLDate(rsGetPat("dob")) & ")")
            End If

            'get appointment type
            Dim rsGetAppType As Odbc.OdbcDataReader
            Dim sqlGetAppType As New Odbc.OdbcCommand("select type, child from appointment where appointmentid = ?", conn)
            sqlGetAppType.Parameters.AddWithValue("appointmentid", reader("appointmentid"))
            rsGetAppType = sqlGetAppType.ExecuteReader

            If rsGetAppType.Read Then
                Console.WriteLine("Appointment type: " & rsGetAppType("type"))
                If rsGetAppType("child") = 1 Then
                    Console.BackgroundColor = ConsoleColor.Cyan
                    Console.ForegroundColor = ConsoleColor.Black
                    Console.WriteLine("CHILD APPOINTMENT")
                    Console.ForegroundColor = ConsoleColor.Gray
                    Console.BackgroundColor = ConsoleColor.Black
                End If
            End If

            'show date
            Console.WriteLine("Date: " & stringHandling.SQLDate(reader("date")))

            'show start - end time
            Console.WriteLine(reader("starttime").ToString & " - " & reader("endtime").ToString)

            'room
            Console.WriteLine("Room: " & reader("room"))

            'interpreter
            If reader("interpreter") = 1 Then
                Console.ForegroundColor = ConsoleColor.Green
                Console.WriteLine("Interpreter needed")
            Else
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("Interpreter not needed")
            End If
            Console.ForegroundColor = ConsoleColor.Gray
        End While

    End Sub

    Public Function ChooseRoom() As String
        Dim room As String = ""
        Console.CursorVisible = False
        Dim currentChoice As Integer = 1
        Dim choice As ConsoleKey
        Console.Clear()
        Console.WriteLine("Choose room:
   Seahorse
   Starfish
   Coral
   Dolphin
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
                room = "Seahorse"
            Case 2
                room = "Starfish"
            Case 3
                room = "Coral"
            Case 4
                room = "Dolphin"
        End Select
        Return room
    End Function

    Public Sub SearchAudiologists()

    End Sub

    Public Sub SearchPatients()

    End Sub

    Public Sub SearchAnnualLeave()
        Dim stringHandling As New ErrorHandling
        Console.Clear()
        Select Case DateOrAud()
            Case True 'date
                Console.Clear()
                Dim ALDate As Date = stringHandling.GetDate3

                Dim rsGetCount As Odbc.OdbcDataReader
                Dim sqlGetCount As New Odbc.OdbcCommand("select count(*) from annualleave where date = ?", conn)
                sqlGetCount.Parameters.AddWithValue("date", stringHandling.SQLDate(ALDate))
                rsGetCount = sqlGetCount.ExecuteReader
                If rsGetCount.Read Then
                    Console.WriteLine("Number of audiologistis off: " & rsGetCount("COUNT(*)"))
                End If

                Dim rsSearchALDate As Odbc.OdbcDataReader
                Dim sqlSearchALDate As New Odbc.OdbcCommand("SELECT * FROM annualleave WHERE DATE = ? ORDER BY starttime", conn)
                sqlSearchALDate.Parameters.AddWithValue("date", stringHandling.SQLDate(ALDate))
                rsSearchALDate = sqlSearchALDate.ExecuteReader

                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("ANNUAL LEAVE - " & stringHandling.SQLDate(ALDate))
                Console.ForegroundColor = ConsoleColor.Gray
                While rsSearchALDate.Read
                    Dim rsGetAudName As Odbc.OdbcDataReader
                    Dim sqlGetAudName As New Odbc.OdbcCommand("select firstname, surname from audiologists where audiologistid = ?", conn)
                    sqlGetAudName.Parameters.AddWithValue("audiologistid", rsSearchALDate("audiologistid"))
                    rsGetAudName = sqlGetAudName.ExecuteReader
                    Console.WriteLine()
                    If rsGetAudName.Read Then
                        Console.WriteLine(rsGetAudName("firstname") & " " & rsGetAudName("surname"))
                    End If
                    Console.WriteLine(rsSearchALDate("starttime").ToString & " - " & rsSearchALDate("endtime").ToString)
                    If rsSearchALDate("personalappointment") = 1 Then
                        Console.ForegroundColor = ConsoleColor.Cyan
                        Console.WriteLine("Personal appointment")
                        Console.ForegroundColor = ConsoleColor.Gray
                    End If
                End While
            Case False 'audiologist
                Console.Clear()

                'create audiologist
                Dim flag As Boolean = False
                Dim fName, sName As String
                fName = ""
                sName = ""
                Do Until flag = True
                    Console.Write("Enter audiologist first name: ")
                    fName = stringHandling.TryString(1).ToUpper
                    Console.Write("Enter audiologist surname: ")
                    sName = stringHandling.TryString(1).ToUpper
                    Dim tryAud As New Audiologist(fName, sName)
                    flag = tryAud.GetAudiologistInfo(conn)
                Loop

                Dim aud As New Audiologist(fName, sName)
                aud.GetAudiologistInfo(conn)
                aud.SearchAnnualLeave(conn)
        End Select
        Console.WriteLine("
Press any key to continue...")
        Console.ReadKey()
    End Sub

    Public Sub SearchRepairs()
        Dim stringHandling As New ErrorHandling
        Console.Clear()
        Select Case DateOrAud()
            Case True 'date
                Console.Clear()
                'finds any audiologists on repairs on that date
                Dim RepsDate As Date = stringHandling.GetDate3
                Console.Clear()
                Dim rsSearchRepsDate As Odbc.OdbcDataReader
                Dim sqlSearchRepsDate As New Odbc.OdbcCommand("SELECT * FROM repairs WHERE DATE = ? ORDER BY starttime", conn)
                sqlSearchRepsDate.Parameters.AddWithValue("date", RepsDate)
                rsSearchRepsDate = sqlSearchRepsDate.ExecuteReader
                While rsSearchRepsDate.Read
                    Console.ForegroundColor = ConsoleColor.Yellow
                    Console.WriteLine("REPAIRS - " & stringHandling.SQLDate(rsSearchRepsDate("date")))
                    Console.ForegroundColor = ConsoleColor.Gray
                    Dim rsFindAudName As Odbc.OdbcDataReader
                    Dim sqlFindAudName As New Odbc.OdbcCommand("select firstname, surname from audiologists where audiologistid = ?", conn)
                    sqlFindAudName.Parameters.AddWithValue("audiologistid", rsSearchRepsDate("audiologistid"))
                    rsFindAudName = sqlFindAudName.ExecuteReader
                    If rsFindAudName.Read Then
                        Console.WriteLine("Audiologist: " & rsFindAudName("firstname") & " " & rsFindAudName("surname"))
                    End If
                    Console.WriteLine(rsSearchRepsDate("starttime").ToString & " - " & rsSearchRepsDate("endtime").ToString)
                End While
            Case False 'audiologist
                Console.Clear()
                'finds all the repairs an audiologist is booked into
                'create audiologist
                Dim flag As Boolean = False
                Dim fName, sName As String
                fName = ""
                sName = ""
                Do Until flag = True
                    Console.Write("Enter audiologist first name: ")
                    fName = stringHandling.TryString(1).ToUpper
                    Console.Write("Enter audiologist surname: ")
                    sName = stringHandling.TryString(1).ToUpper
                    Dim tryAud As New Audiologist(fName, sName)
                    flag = tryAud.GetAudiologistInfo(conn)
                Loop

                Dim aud As New Audiologist(fName, sName)
                aud.GetAudiologistInfo(conn)
                aud.SearchRepairs(conn)
        End Select
        Console.WriteLine("
Press any key to continue...")
        Console.ReadKey()
    End Sub

    Public Sub SearchMeetings()
        Dim stringHandling As New ErrorHandling
        Console.Clear()
        Dim dateAudPlaceChosen As Integer = 0
        Do Until dateAudPlaceChosen = 1 Or dateAudPlaceChosen = 2 Or dateAudPlaceChosen = 3
            dateAudPlaceChosen = DateAudPlaceChoice()
        Loop
        Select Case dateAudPlaceChosen
            Case 1 'date
                Console.Clear()
                Dim meetDate As Date = stringHandling.GetDate3

                Console.Clear()
                Dim rsMeetCount As Odbc.OdbcDataReader
                Dim sqlMeetCount As New Odbc.OdbcCommand("select count(*) from meeting where date = ?", conn)
                sqlMeetCount.Parameters.AddWithValue("date", stringHandling.SQLDate(meetDate))
                rsMeetCount = sqlMeetCount.ExecuteReader

                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("MEETINGS - " & stringHandling.SQLDate(meetDate))
                Console.ForegroundColor = ConsoleColor.Gray

                If rsMeetCount.Read Then
                    Console.WriteLine("Number of meetings: " & rsMeetCount("count(*)"))
                End If

                PrintSearchMeetDate(stringHandling, meetDate)

            Case 2 'attendant
                Console.Clear()
                'create audiologist
                Dim flag As Boolean = False
                Dim fName, sName As String
                fName = ""
                sName = ""
                Do Until flag = True
                    Console.Write("Enter audiologist first name: ")
                    fName = stringHandling.TryString(1).ToUpper
                    Console.Write("Enter audiologist surname: ")
                    sName = stringHandling.TryString(1).ToUpper
                    Dim tryAud As New Audiologist(fName, sName)
                    flag = tryAud.GetAudiologistInfo(conn)
                Loop

                Dim aud As New Audiologist(fName, sName)
                aud.GetAudiologistInfo(conn)
                aud.SearchMeeting(conn)

            Case 3 'place
                Console.Clear()
                Dim places As New List(Of String)
                Dim rsMeetingPlaces As Odbc.OdbcDataReader
                Dim sqlMeetingPlaces As New Odbc.OdbcCommand("select distinct place from meeting where date >= ?", conn)
                sqlMeetingPlaces.Parameters.AddWithValue("date", stringHandling.SQLDate(Date.Today))
                rsMeetingPlaces = sqlMeetingPlaces.ExecuteReader
                While rsMeetingPlaces.Read
                        places.Add(rsMeetingPlaces("place"))
                End While
                If places.Count > 0 Then
                    Console.CursorVisible = False
                    Dim currentChoice As Integer = 1
                    Dim choice As ConsoleKey
                    Console.Clear()
                    Console.WriteLine("Select meeting place:")
                    For i = 0 To places.Count - 1
                        Console.WriteLine("   " & places(i))
                    Next
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
                                If currentChoice < places.Count Then
                                    Console.SetCursorPosition(0, currentChoice)
                                    Console.Write("  ")
                                    currentChoice += 1
                                    Console.SetCursorPosition(0, currentChoice)
                                    Console.Write(" >")
                                End If
                        End Select
                    Loop Until choice = ConsoleKey.Enter
                    Console.CursorVisible = True

                    PrintSearchMeetPlace(stringHandling, places, currentChoice - 1)
                Else
                    Console.WriteLine("No meetings are booked for today or in the future in this meeting place.")
                End If
        End Select
        Console.WriteLine("
Press any key to continue...")
        Console.ReadKey()
    End Sub

    Public Sub PrintSearchMeetDate(ByVal stringHandling As ErrorHandling, ByVal meetDate As Date)
        Dim rsMeetSearch As Odbc.OdbcDataReader
        Dim sqlMeetSearch As New Odbc.OdbcCommand("SELECT * FROM meeting WHERE DATE = ?", conn)
        sqlMeetSearch.Parameters.AddWithValue("date", stringHandling.SQLDate(meetDate))
        rsMeetSearch = sqlMeetSearch.ExecuteReader

        While rsMeetSearch.Read
            Console.WriteLine()
            Console.WriteLine("Meeting description: " & rsMeetSearch("description"))
            Console.WriteLine("Meeting place: " & rsMeetSearch("place"))
            Console.WriteLine(rsMeetSearch("starttime").ToString & " - " & rsMeetSearch("endtime").ToString)
            Console.WriteLine("Meeting attendants: ")

            Dim rsFindMeetingAttendants As Odbc.OdbcDataReader
            Dim sqlFindMeetingAttendants As New Odbc.OdbcCommand("select audiologistid from meetingattendants where meetingid = ?", conn)
            sqlFindMeetingAttendants.Parameters.AddWithValue("meetingid", rsMeetSearch("meetingid"))
            rsFindMeetingAttendants = sqlFindMeetingAttendants.ExecuteReader
            While rsFindMeetingAttendants.Read
                Dim rsGetAudName As Odbc.OdbcDataReader
                Dim sqlGetAudName As New Odbc.OdbcCommand("select firstname, surname from audiologists where audiologistid = ?", conn)
                sqlGetAudName.Parameters.AddWithValue("audiologistid", rsFindMeetingAttendants("audiologistid"))
                rsGetAudName = sqlGetAudName.ExecuteReader
                If rsGetAudName.Read Then
                    Console.WriteLine(" - " & rsGetAudName("firstname") & " " & rsGetAudName("surname"))
                End If
            End While
        End While
    End Sub

    Public Sub PrintSearchMeetPlace(ByVal stringHandling As ErrorHandling, ByVal places As List(Of String), ByVal currentChoice As Integer)
        Console.Clear()
        Dim rsSearchMeetPlace As Odbc.OdbcDataReader
        Dim sqlSearchMeetPlace As New Odbc.OdbcCommand("SELECT DISTINCT * FROM meeting WHERE DATE >= ? AND place = ?", conn)
        sqlSearchMeetPlace.Parameters.AddWithValue("date", stringHandling.SQLDate(Date.Today))
        sqlSearchMeetPlace.Parameters.AddWithValue("place", places(currentChoice))
        rsSearchMeetPlace = sqlSearchMeetPlace.ExecuteReader

        Console.ForegroundColor = ConsoleColor.Yellow
        Console.WriteLine("MEETINGS - " & places(currentChoice))
        Console.ForegroundColor = ConsoleColor.Gray

        While rsSearchMeetPlace.Read
            Console.WriteLine()
            Console.WriteLine("Meeting description: " & rsSearchMeetPlace("description"))
            Console.WriteLine("Meeting place: " & rsSearchMeetPlace("place"))
            Console.WriteLine(rsSearchMeetPlace("date") & " " & rsSearchMeetPlace("starttime").ToString & " - " & rsSearchMeetPlace("endtime").ToString)
            Console.WriteLine("Meeting attendants: ")
            Dim rsFindMeetingAttendants As Odbc.OdbcDataReader
            Dim sqlFindMeetingAttendants As New Odbc.OdbcCommand("select audiologistid from meetingattendants where meetingid = ?", conn)
            sqlFindMeetingAttendants.Parameters.AddWithValue("meetingid", rsSearchMeetPlace("meetingid"))
            rsFindMeetingAttendants = sqlFindMeetingAttendants.ExecuteReader
            While rsFindMeetingAttendants.Read
                Dim rsGetAudName As Odbc.OdbcDataReader
                Dim sqlGetAudName As New Odbc.OdbcCommand("select firstname, surname from audiologists where audiologistid = ?", conn)
                sqlGetAudName.Parameters.AddWithValue("audiologistid", rsFindMeetingAttendants("audiologistid"))
                rsGetAudName = sqlGetAudName.ExecuteReader
                If rsGetAudName.Read Then
                    Console.WriteLine(" - " & rsGetAudName("firstname") & " " & rsGetAudName("surname"))
                End If
            End While
        End While
    End Sub

    Function DateOrAud() As Boolean
        Console.CursorVisible = False
        Dim currentChoice As Integer = 1
        Dim choice As ConsoleKey
        Console.Clear()
        Console.WriteLine("Would you like to search for repairs by the date or the audiologist?:
   Date
   Audiologist
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

    Function DateAudPlaceChoice() As Integer
        Console.CursorVisible = False
        Dim currentChoice As Integer = 1
        Dim choice As ConsoleKey
        Console.Clear()
        Console.WriteLine("Would you like to search for meetings by the date, audiologist or place?:
   Date
   Audiologist
   Place
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
        Select Case currentChoice
            Case 1
                Return 1
            Case 2
                Return 2
            Case 3
                Return 3
        End Select
        Return 0
    End Function

    Function DateAudPatRoom() As Integer
        Console.CursorVisible = False
        Dim currentChoice As Integer = 1
        Dim choice As ConsoleKey
        Console.Clear()
        Console.WriteLine("Would you like to search for meetings by the date, audiologist or place?:
   Date
   Audiologist
   Patient
   Room
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
                Return 1
            Case 2
                Return 2
            Case 3
                Return 3
            Case 4
                Return 4
        End Select
        Return 0
    End Function

    'other section
    Sub Other()
        Dim selection As ConsoleKey
        While selection <> ConsoleKey.NumPad1 And selection <> ConsoleKey.NumPad2 And selection <> ConsoleKey.NumPad3 And selection <> ConsoleKey.NumPad4 And selection <> ConsoleKey.NumPad0 And selection <> ConsoleKey.D1 And selection <> ConsoleKey.D2 And selection <> ConsoleKey.D3 And selection <> ConsoleKey.D4 And selection <> ConsoleKey.D0
            Console.Clear()
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine("Other Menu")
            Console.ForegroundColor = ConsoleColor.Gray
            Console.WriteLine("
1. Edit Audiologist Information
2. Edit Patient Information
3. Add Meeting Attendants
4. Change Working Hours
5. Cancel Annual Leave
6. Cancel Meeting
7. Add Patient Notes
8. Add New Audiologist
9. Add New Patient
0. Exit")
            selection = Console.ReadKey(True).Key

            Select Case selection
                Case ConsoleKey.NumPad1, ConsoleKey.D1
                    EditAudInfo()
                    selection = ConsoleKey.BrowserBack
                Case ConsoleKey.NumPad2, ConsoleKey.D2
                    EditPatientInfo()
                    selection = ConsoleKey.BrowserBack
                Case ConsoleKey.NumPad3, ConsoleKey.D3
                    AddMeetingAttendants()
                    selection = ConsoleKey.NumPad9
                Case ConsoleKey.NumPad4, ConsoleKey.D4
                    ChangeWorkingHours()
                    selection = ConsoleKey.BrowserBack
                Case ConsoleKey.NumPad5, ConsoleKey.D5
                    CancelAnnualLeave()
                    selection = ConsoleKey.NumPad9
                Case ConsoleKey.NumPad6, ConsoleKey.D6
                    CancelMeeting()
                    selection = ConsoleKey.BrowserBack
                Case ConsoleKey.NumPad7, ConsoleKey.D7
                    AddPatientNotes()
                    selection = ConsoleKey.BrowserBack
                Case ConsoleKey.NumPad8, ConsoleKey.D8
                    AddNewAudiologist()
                    selection = ConsoleKey.NumPad9
                Case ConsoleKey.NumPad9, ConsoleKey.D9
                    AddNewPatient()
                    selection = ConsoleKey.BrowserBack
                Case ConsoleKey.NumPad0, ConsoleKey.D0
                    Console.WriteLine("Going back...")
                    System.Threading.Thread.Sleep(500)
            End Select
        End While
    End Sub

    Public Sub EditAudInfo()

    End Sub

    Public Sub EditPatientInfo()

    End Sub

    Public Sub AddMeetingAttendants()

    End Sub

    Public Sub ChangeWorkingHours()

    End Sub

    Public Sub CancelAnnualLeave()

    End Sub

    Public Sub CancelMeeting()
        Dim flag As Boolean = True
        Dim cancelMeeting As Integer
        Dim stringHandling As New ErrorHandling
        Dim meetingID As New List(Of Integer)

        Console.Clear()
        Console.WriteLine("Cancel meeting:")

        Dim rsGetAllMeetings As Odbc.OdbcDataReader
        Dim sqlGetAllMeetings As New Odbc.OdbcCommand("select * from meeting where date >= ?", conn)
        sqlGetAllMeetings.Parameters.AddWithValue("date", stringHandling.SQLDate(Date.Today))
        rsGetAllMeetings = sqlGetAllMeetings.ExecuteReader
        While rsGetAllMeetings.Read
            meetingID.Add(rsGetAllMeetings("meetingid"))
            Console.WriteLine(meetingID.Count & ". [" & rsGetAllMeetings("description") & "] - [" & rsGetAllMeetings("place") & "] - [" & rsGetAllMeetings("date") & "] - [" & rsGetAllMeetings("starttime").ToString & " - " & rsGetAllMeetings("endtime").ToString & "]")
        End While

        Console.WriteLine()

        While flag = True
            Try
                flag = False
                Console.Write("Please choose a meeting to cancel: ")
                cancelMeeting = Console.ReadLine()
                If cancelMeeting > meetingID.Count Then
                    Throw New Exception("There is no " & cancelMeeting & " option.")
                End If
            Catch ex As Exception
                flag = True
                Console.WriteLine("An error occured." & ex.Message)
            End Try
        End While

        Dim sqlDeleteMeeting As New Odbc.OdbcCommand("DELETE FROM meeting WHERE meetingid = ?", conn)
        sqlDeleteMeeting.Parameters.AddWithValue("meetingID", meetingID(cancelMeeting - 1))
        sqlDeleteMeeting.ExecuteNonQuery()

        Dim sqlDeleteMeetingAttendants As New Odbc.OdbcCommand("DELETE FROM meetingattendants WHERE meetingid = ?", conn)
        sqlDeleteMeetingAttendants.Parameters.AddWithValue("meetingid", meetingID(cancelMeeting - 1))
        sqlDeleteMeetingAttendants.ExecuteNonQuery()

        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("Meeting cancelled.")
        Console.ForegroundColor = ConsoleColor.Gray
        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
    End Sub

    Public Sub AddPatientNotes()
        'get appointment date
        'get patient
        'select appointment
        'add notes
        'put notes into database
        Console.Clear()
        Dim read As Boolean = False
        Dim stringHandling As New ErrorHandling
        Dim appDate As Date = stringHandling.GetDate4

        Dim checkPat As Boolean
        Dim fName, sName As String
        fName = ""
        sName = ""
        Console.Clear()
        Do Until checkPat = True
            Console.Write("Enter patient first name: ")
            fName = stringHandling.TryString(1).ToUpper
            Console.Write("Enter patient surname: ")
            sName = stringHandling.TryString(1).ToUpper
            Dim patTry As New Patient(fName, sName)
            checkPat = patTry.CheckPatient(conn)
        Loop

        Dim pat As New Patient(fName, sName)
        pat.GetPatientInfo(2, conn)

        Dim appsList As New List(Of Integer)
        Dim audiologistName As String = ""
        Dim appType As String = ""
        Dim rsGetApps As Odbc.OdbcDataReader
        Dim sqlGetApps As New Odbc.OdbcCommand("SELECT DISTINCT * FROM patientbooking WHERE patientid = ? AND DATE = ?", conn)
        sqlGetApps.Parameters.AddWithValue("patientid", pat.ReturnPatientID)
        sqlGetApps.Parameters.AddWithValue("date", stringHandling.SQLDate(appDate))
        rsGetApps = sqlGetApps.ExecuteReader
        While rsGetApps.Read
            read = True
            appsList.Add(rsGetApps("bookingid"))

            Dim rsGetAud As Odbc.OdbcDataReader
            Dim sqlGetAud As New Odbc.OdbcCommand("select firstname, surname from audiologists where audiologistid = ?", conn)
            sqlGetAud.Parameters.AddWithValue("audiologistid", rsGetApps("audiologistid"))
            rsGetAud = sqlGetAud.ExecuteReader
            If rsGetAud.Read Then
                audiologistName = rsGetAud("firstname") & " " & rsGetAud("surname")
            End If

            Dim rsGetAppType As Odbc.OdbcDataReader
            Dim sqlGetAppType As New Odbc.OdbcCommand("select type from appointment where appointmentid = ?", conn)
            sqlGetAppType.Parameters.AddWithValue("appointmentid", rsGetApps("appointmentid"))
            rsGetAppType = sqlGetAppType.ExecuteReader
            If rsGetAppType.Read Then
                appType = rsGetAppType("type")
            End If

            Console.WriteLine(appsList.Count & ". " & pat.ReturnPatientName & " - " & audiologistName & " - " & appType)
        End While
        If read = True Then
            Console.WriteLine()

            Dim flag As Boolean = True
            Dim chooseApp As Integer
            While flag = True
                Try
                    flag = False
                    Console.Write("Please choose an appointment: ")
                    chooseApp = Console.ReadLine()
                    If chooseApp > appsList.Count Then
                        Throw New Exception("There is no " & chooseApp & " option.")
                    End If
                Catch ex As Exception
                    flag = True
                    Console.WriteLine("An error occured." & ex.Message)
                End Try
            End While
            Console.WriteLine("You have selected appointment " & chooseApp)

            Dim notes As String
            Console.WriteLine("Enter patient notes (there is a maximum of 255 characters):")
            notes = stringHandling.TryString(1, 255)

            Dim sqlAddNotes As New Odbc.OdbcCommand("UPDATE patientbooking SET notes = ? WHERE bookingID = ?", conn)
            sqlAddNotes.Parameters.AddWithValue("notes", notes)
            sqlAddNotes.Parameters.AddWithValue("bookingid", appsList(chooseApp - 1))
            sqlAddNotes.ExecuteNonQuery()

            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("Patient notes have been added.")
            Console.ForegroundColor = ConsoleColor.Gray
        Else
            Console.WriteLine("No appointment exists for this patient at this time.")
        End If
        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
    End Sub

    Public Sub AddNewAudiologist()
        Console.Clear()
        Dim flag As Boolean = True
        Dim fName As String = ""
        Dim sName As String = ""
        Dim stringHandling As New ErrorHandling
        'create audiologist
        Do Until flag = False
            Console.Write("Enter audiologist first name: ")
            fName = stringHandling.TryString(1).ToUpper
            Console.Write("Enter audiologist surname: ")
            sName = stringHandling.TryString(1).ToUpper
            Dim tryAud As New Audiologist(fName, sName)
            flag = tryAud.GetAudiologistInfo(conn)
            If flag = True Then
                Select Case YesNo("You already have an audiologist with this name, are you sure you want to add another?")
                    Case True 'yes
                        flag = False
                    Case False 'no
                        flag = True
                End Select
            End If
        Loop

        Dim aud As New Audiologist(fName, sName)
        aud.CreateNewAud(conn)
    End Sub

    Public Sub AddNewPatient()
        Dim stringHandling As New ErrorHandling
        Dim checkPat As Boolean = True
        Dim fName, sName As String
        fName = ""
        sName = ""
        Console.Clear()
        Do Until checkPat = False
            Console.Write("Enter patient first name: ")
            fName = stringHandling.TryString(1).ToUpper
            Console.Write("Enter patient surname: ")
            sName = stringHandling.TryString(1).ToUpper
            Dim patTry As New Patient(fName, sName)
            checkPat = patTry.CheckPatient(conn)
            If checkPat = True Then
                Select Case YesNo("Patient with this name already exists. Are you sure you want to create another one?")
                    Case True 'yes
                        checkPat = False
                    Case False 'no
                        checkPat = True
                End Select
            End If
        Loop

        Dim pat As New Patient(fName, sName)
        pat.GetPatientInfo(1, conn)
    End Sub

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

End Module
