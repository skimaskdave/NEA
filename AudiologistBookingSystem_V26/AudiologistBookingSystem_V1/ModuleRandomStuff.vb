Module ModuleRandomStuff

    Sub RandomStuff()
        Dim selection As ConsoleKey = ConsoleKey.NumPad8
        While selection <> ConsoleKey.NumPad1 And selection <> ConsoleKey.NumPad2 And selection <> ConsoleKey.NumPad3 And selection <> ConsoleKey.NumPad0 And selection <> ConsoleKey.D1 And selection <> ConsoleKey.D2 And selection <> ConsoleKey.D3 And selection <> ConsoleKey.D0
            Console.Clear()
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine("Random Stuff")
            Console.ForegroundColor = ConsoleColor.Gray
            Console.WriteLine("
1. Patient Booking (specific audiologist)
2. Annual Leave Booking (or personal appointment)
3. Repairs Booking
0. Back")
            selection = Console.ReadKey(True).Key

            Select Case selection
                Case ConsoleKey.NumPad1, ConsoleKey.D1
                    BookRandomPatApp()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad2, ConsoleKey.D2
                    BookRandomAnnualLeave()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad3, ConsoleKey.D3
                    BookNextRepairs()
                    selection = ConsoleKey.NumPad8
                Case ConsoleKey.NumPad0, ConsoleKey.D0
                    Console.WriteLine("Going back...")
                    System.Threading.Thread.Sleep(500)
            End Select
        End While
    End Sub

    Sub BookRandomPatApp()
        Console.Clear()
        Randomize()
        Dim fName, sName As String
        Dim pats As New List(Of Integer)
        Dim rsPats As Odbc.OdbcDataReader
        Dim sqlPats As New Odbc.OdbcCommand("select patientid from patients", GetConnection)
        rsPats = sqlPats.ExecuteReader()
        While rsPats.Read
            pats.Add(rsPats("patientid"))
        End While
        Dim rsPat As Odbc.OdbcDataReader
        Dim sqlPat As New Odbc.OdbcCommand("select firstname, surname from patients where patientid = ?", GetConnection)
        sqlPat.Parameters.AddWithValue("patientid", pats(Int(Rnd() * pats.Count)))
        rsPat = sqlPat.ExecuteReader()
        If rsPat.Read Then
            fName = rsPat("firstname")
            sName = rsPat("surname")
        End If
        Dim pat As New Patient(fName, sName)
        Dim patBook As New Booking(pat)
        patBook.BookRandomPatientAud()
        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
    End Sub

    Sub BookRandomAnnualLeave()
        Randomize()
        Console.Clear()
        Dim auds As New List(Of Integer)
        Dim fName, sName As String

        Dim startDate, endDate As Date
        Dim startTime, endTime As TimeSpan
        Dim allDay As Boolean = True
        Dim personalAppointment As Integer
        startTime = TimeSpan.Parse("00:00:00")
        endTime = TimeSpan.Parse("23:59:59")

        startDate = Date.Today.AddDays(Int(Rnd() * 280 + 31))
        If startDate.DayOfWeek = 1 Or startDate.DayOfWeek = 7 Then
            startDate = Date.Today.AddDays(Int(Rnd() * 280 + 31))
        End If
        endDate = startDate.AddDays(Int(Rnd() * 6))
        If endDate.DayOfWeek = 1 Or endDate.DayOfWeek = 7 Then
            endDate = Date.Today.AddDays(Int(Rnd() * 280 + 31))
        End If

        Dim rsAuds As Odbc.OdbcDataReader
        Dim sqlAuds As New Odbc.OdbcCommand("select audiologistid from audiologists", Module1.GetConnection())
        rsAuds = sqlAuds.ExecuteReader()
        While rsAuds.Read
            auds.Add(rsAuds("audiologistid"))
        End While
        Dim rsAud As Odbc.OdbcDataReader
        Dim sqlAud As New Odbc.OdbcCommand("select firstname, surname from audiologists where audiologistid = ?", Module1.GetConnection)
        sqlAud.Parameters.AddWithValue("audiologistid", auds(Int(Rnd() * auds.Count)))
        rsAud = sqlAud.ExecuteReader()
        If rsAud.Read Then
            fName = rsAud("firstname")
            sName = rsAud("surname")
        End If
        Dim aud As New Audiologist(fName, sName)
        Dim booking As New Booking(aud)

        Console.Clear()
        If booking.CheckAnnualLeaveCanHappen(startDate, endDate, startTime, endTime) = True Then
            booking.BookAnnualLeave(startTime, endTime, startDate, endDate, personalAppointment)
            're-assign repairs/appointments & cancel meetings
            aud.CancelMeeting(startTime, endTime, startDate, endDate)
            aud.RearrangeRepairs(startTime, endTime, startDate, endDate)
            aud.RearrangeAppointment(startTime, endTime, startDate, endDate)
        Else
            Console.WriteLine("You cannot book annual leave at this time.")
        End If

        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
    End Sub

    Sub BookNextRepairs()
        Console.Clear()
        Dim book As New Booking
        Dim stringHandling As New ErrorHandling
        Dim newDate As Date
        Dim rsReps As Odbc.OdbcDataReader
        Dim sqlReps As New Odbc.OdbcCommand("SELECT date FROM repairs ORDER BY DATE DESC", Module1.GetConnection)
        rsReps = sqlReps.ExecuteReader()
        If rsReps.Read Then
            newDate = rsReps("date")
        End If
        Select Case newDate.DayOfWeek
            Case DayOfWeek.Friday
                newDate = newDate.AddDays(3)
            Case DayOfWeek.Saturday
                newDate = newDate.AddDays(2)
            Case Else
                newDate = newDate.AddDays(1)
        End Select
        If newDate < Date.Today Then
            newDate = Date.Today
        End If
        Dim booking As New Booking()
        Dim aud As Audiologist = booking.RandomAudSelection(TimeSpan.Parse("09:05:00"), TimeSpan.Parse("16:55:00"), book.GetWeekDay(newDate.DayOfWeek)) 'choose an audiologist that is free
        If aud.ReturnAudiologistName <> "error occured" Then
            Dim sqlBookReps As New Odbc.OdbcCommand("insert into repairs(audiologistid, date, starttime, endtime) values(?, ?, ?, ?)", Module1.GetConnection())
            sqlBookReps.Parameters.AddWithValue("audiologistid", aud.ReturnAudiologistID)
            sqlBookReps.Parameters.AddWithValue("date", stringHandling.SQLDate(newDate))
            sqlBookReps.Parameters.AddWithValue("starttime", TimeSpan.Parse("09:05:00"))
            sqlBookReps.Parameters.AddWithValue("endtime", TimeSpan.Parse("17:00:00"))
            sqlBookReps.ExecuteNonQuery()
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("Repairs booked for " & aud.ReturnAudiologistName & " on " & stringHandling.SQLDate(newDate))
            Console.ForegroundColor = ConsoleColor.Gray
        Else
            Console.WriteLine("An error occured.")
        End If
        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
    End Sub

End Module
