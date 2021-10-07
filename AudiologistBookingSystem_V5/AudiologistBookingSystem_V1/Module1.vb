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
4. Annual Leave Booking
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
                    BookAL()
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

    Sub BookPatient()
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
        pat.GetPatientInfo(conn)

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

    Sub BookPatient2()
        Console.Clear()
        Dim fName, sName As String
        Dim flag As Boolean = False
        Dim stringCheck As New ErrorHandling()
        fname = ""
        sname = ""

        Console.Write("Enter patient first name: ")
        fName = stringCheck.TryString(1).ToUpper
        Console.Write("Enter patient surname: ")
        sName = stringCheck.TryString(1).ToUpper

        Dim pat As New Patient(fName, sName)
        pat.GetPatientInfo(conn)

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
        pat.GetPatientInfo(conn)

        Dim bookPatient As New Booking(pat)
        bookPatient.BookPatient(conn)
    End Sub

    Sub BookAL()
        'choose start date
        'choose end date
        'if startdate = enddate then starttime and endtime are 00:00 and 23:59 or they are the times that they want to start/finish
        'else starttime and endtime are null
    End Sub

    Sub BookRepairs()
        'repairs starts at the earliest 09:05:00
    End Sub

    Sub BookMeeting()
        'meetings cannot start until 09:05:00
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

                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad2, ConsoleKey.D2

                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad3, ConsoleKey.D3

                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad4, ConsoleKey.D4

                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad0, ConsoleKey.D0
                    Console.WriteLine("Going back...")
                    System.Threading.Thread.Sleep(500)
            End Select
        End While
    End Sub

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
3. Change Meeting Details (including adding more attendants)
4. Change Repairs Details (including change audiologist)
5. Change Working Hours
6. Change Annual Leave
7. Rebook Appointment
8. Add New Audiologist
9. Add New Patient
0. Exit")
            selection = Console.ReadKey(True).Key

            Select Case selection
                Case ConsoleKey.NumPad1, ConsoleKey.D1

                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad2, ConsoleKey.D2

                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad3, ConsoleKey.D3

                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad4, ConsoleKey.D4

                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad0, ConsoleKey.D0
                    Console.WriteLine("Going back...")
                    System.Threading.Thread.Sleep(500)
            End Select
        End While
    End Sub

End Module
