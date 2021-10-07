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
                    System.Threading.Thread.Sleep(1000)
            End Select

        End While
    End Sub

    'booking section
    Sub Booking()
        Dim selection As ConsoleKey
        While selection <> ConsoleKey.NumPad1 And selection <> ConsoleKey.NumPad2 And selection <> ConsoleKey.NumPad3 And selection <> ConsoleKey.NumPad4 And selection <> ConsoleKey.NumPad0 And selection <> ConsoleKey.D1 And selection <> ConsoleKey.D2 And selection <> ConsoleKey.D3 And selection <> ConsoleKey.D4 And selection <> ConsoleKey.D0
            Console.WriteLine()
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
                    System.Threading.Thread.Sleep(1000)
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

        Dim bookPatient As New Booking(pat, False)
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

        Dim bookPatient As New Booking(pat, True)
        bookPatient.BookPatient(conn)
    End Sub

    Sub BookAL()

    End Sub

    Sub BookRepairs()

    End Sub

    Sub BookMeeting()

    End Sub

    'check timetable section
    Sub CheckTimetable()
        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.Yellow
        Console.WriteLine("Check Timetable")
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

    'search section
    Sub Search()
        Dim selection As String = ""
        While selection <> "0" And selection <> "1" And selection <> "2" And selection <> "3" And selection <> "4"
            Console.WriteLine()
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine("Search")
            Console.ForegroundColor = ConsoleColor.Gray
            Console.WriteLine("
1. Search Patients
2. Search Annual Leave
3. Search Repairs
4. Search Meetings
0. Exit")
            selection = Console.ReadLine()

            Select Case selection
                Case "1"
                    '
                Case "2"
                    '
                Case "3"
                    '
                Case "4"
                    '
                Case "0"
                    Console.WriteLine()
                    Console.WriteLine("Returning to Main Menu.")
                    Console.WriteLine()
            End Select
        End While
    End Sub

    'other section
    Sub Other()
        Dim selection As String = ""
        While selection <> "0" And selection <> "1" And selection <> "2" And selection <> "3" And selection <> "4"
            Console.WriteLine()
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine("Other")
            Console.ForegroundColor = ConsoleColor.Gray
            Console.WriteLine("
1. Patient Booking
2. Annual Leave Booking
3. Repairs Booking
4. Meeting Booking
0. Exit")
            selection = Console.ReadLine()

            Select Case selection
                Case "1"
                    '
                Case "2"
                    '
                Case "3"
                    '
                Case "4"
                    '
                Case "0"
                    Console.WriteLine()
                    Console.WriteLine("Returning to Main Menu.")
                    Console.WriteLine()
            End Select
        End While
    End Sub

End Module
