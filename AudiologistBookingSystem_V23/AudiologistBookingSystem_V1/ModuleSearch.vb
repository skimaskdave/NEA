Module ModuleSearch
    'search section
    Sub Search()
        Dim selection As ConsoleKey
        While selection <> ConsoleKey.NumPad1 And selection <> ConsoleKey.NumPad2 And selection <> ConsoleKey.NumPad3 And selection <> ConsoleKey.NumPad4 And selection <> ConsoleKey.NumPad0 And selection <> ConsoleKey.D1 And selection <> ConsoleKey.D2 And selection <> ConsoleKey.D3 And selection <> ConsoleKey.D4 And selection <> ConsoleKey.D0 And selection <> ConsoleKey.D5 And selection <> ConsoleKey.D6 And selection <> ConsoleKey.NumPad5 And selection <> ConsoleKey.NumPad6
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

    Sub SearchAppointments()
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
                Dim sqlAppsCount As New Odbc.OdbcCommand("SELECT COUNT(*) FROM patientbooking WHERE DATE = ?", Module1.GetConnection())
                sqlAppsCount.Parameters.AddWithValue("date", stringHandling.SQLDate(appDate))
                rsAppsCount = sqlAppsCount.ExecuteReader

                If rsAppsCount.Read Then
                    Console.WriteLine("Number of appointments: " & rsAppsCount("count(*)"))
                End If

                Dim rsGetApps As Odbc.OdbcDataReader
                Dim sqlGetApps As New Odbc.OdbcCommand("SELECT * FROM patientbooking WHERE DATE = ?", Module1.GetConnection())
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
                    flag = tryAud.GetAudiologistInfo()
                Loop

                Dim aud As New Audiologist(fName, sName)
                aud.GetAudiologistInfo()

                Console.Clear()
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("APPOINTMENTS - " & aud.ReturnAudiologistName)
                Console.ForegroundColor = ConsoleColor.Gray

                Dim rsGetApps As Odbc.OdbcDataReader
                Dim sqlGetApps As New Odbc.OdbcCommand("SELECT * FROM patientbooking WHERE audiologistid = ? AND DATE >= ?", Module1.GetConnection())
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
                    checkPat = patTry.CheckPatient()
                Loop

                Dim pat As New Patient(fName, sName)
                pat.GetPatientInfo(2)

                Console.Clear()
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("APPOINTMENTS - " & pat.ReturnPatientName)
                Console.ForegroundColor = ConsoleColor.Gray

                Dim rsGetApps As Odbc.OdbcDataReader
                Dim sqlGetApps As New Odbc.OdbcCommand("SELECT * FROM patientbooking WHERE patientid = ? AND DATE >= ?", Module1.GetConnection())
                sqlGetApps.Parameters.AddWithValue("patientid", pat.ReturnPatientID())
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
                Dim sqlGetApps As New Odbc.OdbcCommand("SELECT * FROM patientbooking WHERE room = ? AND DATE >= ?", Module1.GetConnection())
                sqlGetApps.Parameters.AddWithValue("room", room)
                sqlGetApps.Parameters.AddWithValue("date", stringHandling.SQLDate(Date.Today))
                PrintAppointment(rsGetApps, sqlGetApps, stringHandling)

        End Select
        Console.WriteLine("
Press any key to continue...")
        Console.ReadKey()
    End Sub

    Sub PrintAppointment(ByVal reader As Odbc.OdbcDataReader, ByVal command As Odbc.OdbcCommand, ByVal stringHandling As ErrorHandling)
        reader = command.ExecuteReader

        While reader.Read
            Console.WriteLine()
            'get audiologist name
            Dim rsGetAudName As Odbc.OdbcDataReader
            Dim sqlGetAudName As New Odbc.OdbcCommand("select firstname, surname from audiologists where audiologistid = ?", Module1.GetConnection())
            sqlGetAudName.Parameters.AddWithValue("audiologistid", reader("audiologistid"))
            rsGetAudName = sqlGetAudName.ExecuteReader

            If rsGetAudName.Read Then
                Console.WriteLine("Audiologist: " & rsGetAudName("firstname") & " " & rsGetAudName("surname"))
            End If

            'get patient name & age
            Dim rsGetPat As Odbc.OdbcDataReader
            Dim sqlGetPat As New Odbc.OdbcCommand("select firstname, surname, dob from patients where patientid = ?", Module1.GetConnection())
            sqlGetPat.Parameters.AddWithValue("patientid", reader("patientid"))
            rsGetPat = sqlGetPat.ExecuteReader

            If rsGetPat.Read Then
                Console.WriteLine("Patient: " & rsGetPat("firstname") & " " & rsGetPat("surname"))
                Console.WriteLine("Age: " & DateDiff(DateInterval.Year, rsGetPat("dob"), Date.Today) & " (" & stringHandling.SQLDate(rsGetPat("dob")) & ")")
            End If

            'get appointment type
            Dim rsGetAppType As Odbc.OdbcDataReader
            Dim sqlGetAppType As New Odbc.OdbcCommand("select type, child from appointment where appointmentid = ?", Module1.GetConnection())
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

    Function ChooseRoom() As String
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

    Sub SearchAudiologists()
        'select an audiologist
        'shows a profile of the audiologist
        Console.Clear()
        Dim aud As Audiologist = Module1.GetAudiologist()
        aud.GetAudiologistInfo()
        aud.PrintAudProfile()
        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
    End Sub

    Sub SearchPatients()
        Console.Clear()
        Dim patsPC As New List(Of String)
        Dim pats As New List(Of Patient)
        Dim rsGetPats As Odbc.OdbcDataReader
        Dim sqlGetPats As New Odbc.OdbcCommand("select firstname, surname, postcode from patients", Module1.GetConnection())
        rsGetPats = sqlGetPats.ExecuteReader
        Console.WriteLine("Select patient: ")
        While rsGetPats.Read
            Dim tempPat As New Patient(rsGetPats("firstname"), rsGetPats("surname"))
            pats.Add(tempPat)
            patsPC.Add(rsGetPats("postcode"))
            Console.WriteLine("   " & rsGetPats("firstname") & " " & rsGetPats("surname") & " - " & rsGetPats("postcode"))
        End While
        Console.CursorVisible = False
        Console.SetCursorPosition(0, 1)
        Console.Write(" >")
        Dim choice As ConsoleKey
        Dim currentChoice As Integer = 1
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
                    If currentChoice < pats.Count Then
                        Console.SetCursorPosition(0, currentChoice)
                        Console.Write("  ")
                        currentChoice += 1
                        Console.SetCursorPosition(0, currentChoice)
                        Console.Write(" >")
                    End If
            End Select
        Loop Until choice = ConsoleKey.Enter
        Console.CursorVisible = True
        Dim pat As Patient = pats(currentChoice - 1)
        pat.GetPatInfoProfile(patsPC(currentChoice - 1))
        pat.PrintPatProfile()
        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
    End Sub

    Sub SearchAnnualLeave()
        Dim stringHandling As New ErrorHandling
        Console.Clear()
        Select Case DateOrAud()
            Case True 'date
                Console.Clear()
                Dim ALDate As Date = stringHandling.GetDate3

                Dim rsGetCount As Odbc.OdbcDataReader
                Dim sqlGetCount As New Odbc.OdbcCommand("select count(*) from annualleave where date = ?", Module1.GetConnection())
                sqlGetCount.Parameters.AddWithValue("date", stringHandling.SQLDate(ALDate))
                rsGetCount = sqlGetCount.ExecuteReader
                If rsGetCount.Read Then
                    Console.WriteLine("Number of audiologistis off: " & rsGetCount("COUNT(*)"))
                End If

                Dim rsSearchALDate As Odbc.OdbcDataReader
                Dim sqlSearchALDate As New Odbc.OdbcCommand("SELECT * FROM annualleave WHERE DATE = ? ORDER BY starttime", Module1.GetConnection())
                sqlSearchALDate.Parameters.AddWithValue("date", stringHandling.SQLDate(ALDate))
                rsSearchALDate = sqlSearchALDate.ExecuteReader

                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("ANNUAL LEAVE - " & stringHandling.SQLDate(ALDate))
                Console.ForegroundColor = ConsoleColor.Gray
                While rsSearchALDate.Read
                    Dim rsGetAudName As Odbc.OdbcDataReader
                    Dim sqlGetAudName As New Odbc.OdbcCommand("select firstname, surname from audiologists where audiologistid = ?", Module1.GetConnection())
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
                    flag = tryAud.GetAudiologistInfo()
                Loop

                Dim aud As New Audiologist(fName, sName)
                aud.GetAudiologistInfo()
                aud.SearchAnnualLeave()
        End Select
        Console.WriteLine("
Press any key to continue...")
        Console.ReadKey()
    End Sub

    Sub SearchRepairs()
        Dim stringHandling As New ErrorHandling
        Console.Clear()
        Select Case DateOrAud()
            Case True 'date
                Console.Clear()
                'finds any audiologists on repairs on that date
                Dim RepsDate As Date = stringHandling.GetDate3
                Console.Clear()
                Dim rsSearchRepsDate As Odbc.OdbcDataReader
                Dim sqlSearchRepsDate As New Odbc.OdbcCommand("SELECT * FROM repairs WHERE DATE = ? ORDER BY starttime", Module1.GetConnection())
                sqlSearchRepsDate.Parameters.AddWithValue("date", RepsDate)
                rsSearchRepsDate = sqlSearchRepsDate.ExecuteReader
                While rsSearchRepsDate.Read
                    Console.ForegroundColor = ConsoleColor.Yellow
                    Console.WriteLine("REPAIRS - " & stringHandling.SQLDate(rsSearchRepsDate("date")))
                    Console.ForegroundColor = ConsoleColor.Gray
                    Dim rsFindAudName As Odbc.OdbcDataReader
                    Dim sqlFindAudName As New Odbc.OdbcCommand("select firstname, surname from audiologists where audiologistid = ?", Module1.GetConnection())
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
                    flag = tryAud.GetAudiologistInfo()
                Loop

                Dim aud As New Audiologist(fName, sName)
                aud.GetAudiologistInfo()
                aud.SearchRepairs()
        End Select
        Console.WriteLine("
Press any key to continue...")
        Console.ReadKey()
    End Sub

    Sub SearchMeetings()
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
                Dim sqlMeetCount As New Odbc.OdbcCommand("select count(*) from meeting where date = ?", Module1.GetConnection())
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
                    flag = tryAud.GetAudiologistInfo()
                Loop

                Dim aud As New Audiologist(fName, sName)
                aud.GetAudiologistInfo()
                aud.SearchMeeting()

            Case 3 'place
                Console.Clear()
                Dim places As New List(Of String)
                Dim rsMeetingPlaces As Odbc.OdbcDataReader
                Dim sqlMeetingPlaces As New Odbc.OdbcCommand("select distinct place from meeting where date >= ?", Module1.GetConnection())
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

    Sub PrintSearchMeetDate(ByVal stringHandling As ErrorHandling, ByVal meetDate As Date)
        Dim rsMeetSearch As Odbc.OdbcDataReader
        Dim sqlMeetSearch As New Odbc.OdbcCommand("SELECT * FROM meeting WHERE DATE = ?", Module1.GetConnection())
        sqlMeetSearch.Parameters.AddWithValue("date", stringHandling.SQLDate(meetDate))
        rsMeetSearch = sqlMeetSearch.ExecuteReader

        While rsMeetSearch.Read
            Console.WriteLine()
            Console.WriteLine("Meeting description: " & rsMeetSearch("description"))
            Console.WriteLine("Meeting place: " & rsMeetSearch("place"))
            Console.WriteLine(rsMeetSearch("starttime").ToString & " - " & rsMeetSearch("endtime").ToString)
            Console.WriteLine("Meeting attendants: ")

            Dim rsFindMeetingAttendants As Odbc.OdbcDataReader
            Dim sqlFindMeetingAttendants As New Odbc.OdbcCommand("select audiologistid from meetingattendants where meetingid = ?", Module1.GetConnection())
            sqlFindMeetingAttendants.Parameters.AddWithValue("meetingid", rsMeetSearch("meetingid"))
            rsFindMeetingAttendants = sqlFindMeetingAttendants.ExecuteReader
            While rsFindMeetingAttendants.Read
                Dim rsGetAudName As Odbc.OdbcDataReader
                Dim sqlGetAudName As New Odbc.OdbcCommand("select firstname, surname from audiologists where audiologistid = ?", Module1.GetConnection())
                sqlGetAudName.Parameters.AddWithValue("audiologistid", rsFindMeetingAttendants("audiologistid"))
                rsGetAudName = sqlGetAudName.ExecuteReader
                If rsGetAudName.Read Then
                    Console.WriteLine(" - " & rsGetAudName("firstname") & " " & rsGetAudName("surname"))
                End If
            End While
        End While
    End Sub

    Sub PrintSearchMeetPlace(ByVal stringHandling As ErrorHandling, ByVal places As List(Of String), ByVal currentChoice As Integer)
        Console.Clear()
        Dim rsSearchMeetPlace As Odbc.OdbcDataReader
        Dim sqlSearchMeetPlace As New Odbc.OdbcCommand("SELECT DISTINCT * FROM meeting WHERE DATE >= ? AND place = ?", Module1.GetConnection())
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
            Dim sqlFindMeetingAttendants As New Odbc.OdbcCommand("select audiologistid from meetingattendants where meetingid = ?", Module1.GetConnection())
            sqlFindMeetingAttendants.Parameters.AddWithValue("meetingid", rsSearchMeetPlace("meetingid"))
            rsFindMeetingAttendants = sqlFindMeetingAttendants.ExecuteReader
            While rsFindMeetingAttendants.Read
                Dim rsGetAudName As Odbc.OdbcDataReader
                Dim sqlGetAudName As New Odbc.OdbcCommand("select firstname, surname from audiologists where audiologistid = ?", Module1.GetConnection())
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
End Module
