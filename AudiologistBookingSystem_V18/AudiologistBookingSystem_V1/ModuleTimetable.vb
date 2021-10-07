Module ModuleTimetable
    'check timetable section
    Public Sub CheckTimetable()
        Dim selection As ConsoleKey
        While selection <> ConsoleKey.NumPad1 And selection <> ConsoleKey.NumPad2 And selection <> ConsoleKey.NumPad0 And selection <> ConsoleKey.D1 And selection <> ConsoleKey.D2 And selection <> ConsoleKey.D0
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
            flag = tryAud.GetAudiologistInfo(Module1.GetConnection())
        Loop

        Dim aud As New Audiologist(fName, sName)
        aud.GetAudiologistInfo(Module1.GetConnection())
        Dim day As Date = stringHandling.GetDateTimetable
        Console.Clear()
        aud.GetAudiologistTimetable(day, Module1.GetConnection())
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
            flag = tryAud.GetAudiologistInfo(Module1.GetConnection())
        Loop

        Dim aud As New Audiologist(fName, sName)
        aud.GetAudiologistInfo(Module1.GetConnection())
        Console.WriteLine("Please enter any date from within the week you want to check (Monday - Friday)")
        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
        Dim day As Date = stringHandling.GetDateTimetable
        Console.Clear()
        aud.GetAudiologistTimetableWeek(day, Module1.GetConnection())
    End Sub
End Module
