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
        pat.GetPatientInfo(conn)
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
        pat.GetPatientInfo(conn)
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
        pat.GetPatientInfo(conn)
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

                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad2, ConsoleKey.D2

                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad3, ConsoleKey.D3

                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad4, ConsoleKey.D4

                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad5, ConsoleKey.D5

                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad6, ConsoleKey.D6

                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad0, ConsoleKey.D0
                    Console.WriteLine("Going back...")
                    System.Threading.Thread.Sleep(500)
            End Select
        End While
    End Sub

    Public Sub SearchAppointments()

    End Sub

    Public Sub SearchAudiologists()

    End Sub

    Public Sub SearchPatients()

    End Sub

    Public Sub SearchAnnualLeave()

    End Sub

    Public Sub SearchRepairs()

    End Sub

    Public Sub SearchMeetings()

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

    End Sub

    Public Sub AddPatientNotes()

    End Sub

    Public Sub AddNewAudiologist()

    End Sub

    Public Sub AddNewPatient()

    End Sub

End Module
