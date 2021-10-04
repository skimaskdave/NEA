Module ModuleBooking
    'booking section
    Sub Booking()
        Dim selection As ConsoleKey
        While selection <> ConsoleKey.NumPad1 And selection <> ConsoleKey.NumPad2 And selection <> ConsoleKey.NumPad3 And selection <> ConsoleKey.NumPad4 And selection <> ConsoleKey.NumPad0 And selection <> ConsoleKey.D1 And selection <> ConsoleKey.D2 And selection <> ConsoleKey.D3 And selection <> ConsoleKey.D4 And selection <> ConsoleKey.D0 And selection <> ConsoleKey.D5 And selection <> ConsoleKey.D6 And selection <> ConsoleKey.NumPad5 And selection <> ConsoleKey.NumPad6
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
        Console.WriteLine("Enter patient first name: ")
        fName = stringCheck.TryString(1).ToUpper
        Console.WriteLine("Enter patient surname: ")
        sName = stringCheck.TryString(1).ToUpper

        Dim pat As New Patient(fName, sName)
        pat.GetPatientInfo(pat.CreateOrSearch, Module1.GetConnection())
        pat.PrintHistory(Module1.GetConnection())

        'create audiologist
        Do Until flag = True
            Console.WriteLine("Enter audiologist first name: ")
            fName = stringCheck.TryString(1).ToUpper
            Console.WriteLine("Enter audiologist surname: ")
            sName = stringCheck.TryString(1).ToUpper
            Dim tryAud As New Audiologist(fName, sName)
            flag = tryAud.GetAudiologistInfo(Module1.GetConnection())
        Loop

        Dim aud As New Audiologist(fName, sName)
        aud.GetAudiologistInfo(Module1.GetConnection())

        'create instance of booking class
        Dim bookPatient As New Booking(pat, aud)
        bookPatient.BookPatient2(Module1.GetConnection())

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
        pat.GetPatientInfo(pat.CreateOrSearch, Module1.GetConnection())
        pat.PrintHistory(Module1.GetConnection())

        Dim bookPatient As New Booking(pat)
        bookPatient.BookPatient(Module1.GetConnection())
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
        pat.GetPatientInfo(pat.CreateOrSearch, Module1.GetConnection())
        pat.PrintHistory(Module1.GetConnection())

        Dim bookPatient As New Booking(pat)
        bookPatient.BookPatientUrgent(Module1.GetConnection())
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
            flag = tryAud.GetAudiologistInfo(Module1.GetConnection())
        Loop

        Dim aud As New Audiologist(fName, sName)
        aud.GetAudiologistInfo(Module1.GetConnection())
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
        If alBooking.CheckAnnualLeaveCanHappen(startDate, endDate, startTime, endTime, Module1.GetConnection()) = True Then
            alBooking.BookAnnualLeave(startTime, endTime, startDate, endDate, personalAppointment, Module1.GetConnection())
            're-assign repairs/appointments & cancel meetings
            aud.CancelMeeting(startTime, endTime, startDate, endDate, Module1.GetConnection())
            aud.RearrangeRepairs(startTime, endTime, startDate, endDate, Module1.GetConnection())
            aud.RearrangeAppointment(startTime, endTime, startDate, endDate, Module1.GetConnection())
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
        repBooking.BookRepairs(Module1.GetConnection())
    End Sub

    Sub BookMeeting()
        'meetings cannot start until 09:05:00
        Console.Clear()
        Dim meetingBooking As New Booking()
        meetingBooking.BookMeeting(Module1.GetConnection())
    End Sub

End Module
