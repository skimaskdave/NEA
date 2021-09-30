Module ModuleOther
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

    Sub EditAudInfo()

    End Sub

    Sub PrintEditAudInfo()
        Console.CursorVisible = False
        Dim currentChoice As Integer = 1
        Dim choice As ConsoleKey
        Console.Clear()
        Console.WriteLine(":
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
    End Sub

    Sub EditPatientInfo()

    End Sub

    Sub AddMeetingAttendants()

    End Sub

    Sub ChangeWorkingHours()

    End Sub

    Sub CancelAnnualLeave()
        Console.Clear()
        Dim stringHandling As New ErrorHandling
        Dim aud As Audiologist = Module1.GetAudiologist
        aud.GetAudiologistInfo(Module1.GetConnection())
        'enter first date you want to cancel
        'enter last date you want to cancel
        'find out number of hours and give them back
        '*program cancels annual leave on those dates*
        Console.Clear()
        Dim startDate, endDate As Date
        Console.WriteLine("Enter first date you want to cancel.")
        startDate = stringHandling.GetDate3
        endDate = Date.Parse("01/01/0001", New System.Globalization.CultureInfo("pt-EN"))
        While DateDiff(DateInterval.Day, startDate, endDate) < 0
            Console.WriteLine("Enter last date you want to cancel. Please note, the end date has to be after or the same as the first date.")
            endDate = stringHandling.GetDate3
        End While


        aud.AddAnnualLeave()
        Dim tempTime As TimeSpan
        Dim daysBack As Double = 0
        Dim rsFindHours As Odbc.OdbcDataReader
        Dim sqlFindHours As New Odbc.OdbcCommand("SELECT TIMEDIFF(endtime, starttime) FROM annualleave WHERE personalappointment = 0 AND DATE <= ? AND DATE >= ? AND audiologistid = ?", Module1.GetConnection)
        sqlFindHours.Parameters.AddWithValue("date", stringHandling.SQLDate(endDate))
        sqlFindHours.Parameters.AddWithValue("date", stringHandling.SQLDate(startDate))
        sqlFindHours.Parameters.AddWithValue("audiologistid", aud.ReturnAudiologistID)
        rsFindHours = sqlFindHours.ExecuteReader
        While rsFindHours.Read
            temptime = rsFindHours("TIMEDIFF(endtime, starttime)")
            If temptime >= TimeSpan.Parse("12:00:00") Then
                daysBack += 1
            Else
                daysBack += 0.5
            End If
        End While

        aud.cancelannualleave(module1.getconnection)
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("Success.")
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

    Sub CancelMeeting()
        Dim flag As Boolean = True
        Dim cancelMeeting As Integer
        Dim stringHandling As New ErrorHandling
        Dim meetingID As New List(Of Integer)

        Console.Clear()
        Console.WriteLine("Cancel meeting:")

        Dim rsGetAllMeetings As Odbc.OdbcDataReader
        Dim sqlGetAllMeetings As New Odbc.OdbcCommand("select * from meeting where date >= ?", Module1.GetConnection())
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

        Dim sqlDeleteMeeting As New Odbc.OdbcCommand("DELETE FROM meeting WHERE meetingid = ?", Module1.GetConnection())
        sqlDeleteMeeting.Parameters.AddWithValue("meetingID", meetingID(cancelMeeting - 1))
        sqlDeleteMeeting.ExecuteNonQuery()

        Dim sqlDeleteMeetingAttendants As New Odbc.OdbcCommand("DELETE FROM meetingattendants WHERE meetingid = ?", Module1.GetConnection())
        sqlDeleteMeetingAttendants.Parameters.AddWithValue("meetingid", meetingID(cancelMeeting - 1))
        sqlDeleteMeetingAttendants.ExecuteNonQuery()

        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("Meeting cancelled.")
        Console.ForegroundColor = ConsoleColor.Gray
        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
    End Sub

    Sub AddPatientNotes()
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
            checkPat = patTry.CheckPatient(Module1.GetConnection())
        Loop

        Dim pat As New Patient(fName, sName)
        pat.GetPatientInfo(2, Module1.GetConnection())

        Dim appsList As New List(Of Integer)
        Dim audiologistName As String = ""
        Dim appType As String = ""
        Dim rsGetApps As Odbc.OdbcDataReader
        Dim sqlGetApps As New Odbc.OdbcCommand("SELECT DISTINCT * FROM patientbooking WHERE patientid = ? AND DATE = ?", Module1.GetConnection())
        sqlGetApps.Parameters.AddWithValue("patientid", pat.ReturnPatientID)
        sqlGetApps.Parameters.AddWithValue("date", stringHandling.SQLDate(appDate))
        rsGetApps = sqlGetApps.ExecuteReader
        While rsGetApps.Read
            read = True
            appsList.Add(rsGetApps("bookingid"))

            Dim rsGetAud As Odbc.OdbcDataReader
            Dim sqlGetAud As New Odbc.OdbcCommand("select firstname, surname from audiologists where audiologistid = ?", Module1.GetConnection())
            sqlGetAud.Parameters.AddWithValue("audiologistid", rsGetApps("audiologistid"))
            rsGetAud = sqlGetAud.ExecuteReader
            If rsGetAud.Read Then
                audiologistName = rsGetAud("firstname") & " " & rsGetAud("surname")
            End If

            Dim rsGetAppType As Odbc.OdbcDataReader
            Dim sqlGetAppType As New Odbc.OdbcCommand("select type from appointment where appointmentid = ?", Module1.GetConnection())
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

            Dim sqlAddNotes As New Odbc.OdbcCommand("UPDATE patientbooking SET notes = ? WHERE bookingID = ?", Module1.GetConnection())
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

    Sub AddNewAudiologist()
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
            flag = tryAud.GetAudiologistInfo(Module1.GetConnection())
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
        aud.CreateNewAud(Module1.GetConnection())
    End Sub

    Sub AddNewPatient()
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
            checkPat = patTry.CheckPatient(Module1.GetConnection())
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
        pat.GetPatientInfo(1, Module1.GetConnection())
    End Sub

    Function YesNo(ByVal message As String) As Boolean
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
