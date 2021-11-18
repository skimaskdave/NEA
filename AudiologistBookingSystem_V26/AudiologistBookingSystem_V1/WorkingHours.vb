Public Class WorkingHours

    Private audiologistID As Integer
    Private startTime(4) As TimeSpan
    Private endTime(4) As TimeSpan
    Private lunchLength(4) As TimeSpan

    Public Sub New(ByVal audID As Integer)
        audiologistID = audID
        For i = 0 To 4
            startTime(i) = TimeSpan.Parse("00:00:00")
            endTime(i) = TimeSpan.Parse("00:00:00")
            lunchLength(i) = TimeSpan.Parse("00:00:00")
        Next
    End Sub

    Public Sub GetWorkingHours()
        Dim rsGetWorkHours As Odbc.OdbcDataReader
        Dim sqlGetWorkHours As New Odbc.OdbcCommand("select * from workinghours where audiologistid = ?", Module1.GetConnection)
        sqlGetWorkHours.Parameters.AddWithValue("@audiologistid", audiologistID)
        rsGetWorkHours = sqlGetWorkHours.ExecuteReader
        While rsGetWorkHours.Read
            Select Case rsGetWorkHours("day")
                Case "Mon"
                    startTime(0) = rsGetWorkHours("startTime")
                    endTime(0) = rsGetWorkHours("endTime")
                    lunchLength(0) = rsGetWorkHours("lunchLength")
                Case "Tue"
                    startTime(1) = rsGetWorkHours("startTime")
                    endTime(1) = rsGetWorkHours("endTime")
                    lunchLength(1) = rsGetWorkHours("lunchLength")
                Case "Wed"
                    startTime(2) = rsGetWorkHours("startTime")
                    endTime(2) = rsGetWorkHours("endTime")
                    lunchLength(2) = rsGetWorkHours("lunchLength")
                Case "Thu"
                    startTime(3) = rsGetWorkHours("startTime")
                    endTime(3) = rsGetWorkHours("endTime")
                    lunchLength(3) = rsGetWorkHours("lunchLength")
                Case "Fri"
                    startTime(4) = rsGetWorkHours("startTime")
                    endTime(4) = rsGetWorkHours("endTime")
                    lunchLength(4) = rsGetWorkHours("lunchLength")
            End Select
        End While
    End Sub

    Public Function ReturnLunchLength(ByVal day As String) As String
        Select Case day
            Case "Mon"
                Return lunchLength(0).ToString
            Case "Tue"
                Return lunchLength(1).ToString
            Case "Wed"
                Return lunchLength(2).ToString
            Case "Thu"
                Return lunchLength(3).ToString
            Case "Fri"
                Return lunchLength(4).ToString
        End Select
        Return "<>"
    End Function

    Public Function ReturnHoursForDay(ByVal day As String) As TimeSpan
        Dim tempHours As TimeSpan
        Dim officialHours As TimeSpan

        Select Case day
            Case "Mon"
                tempHours = endTime(0).Subtract(startTime(0))
                officialHours = tempHours.Subtract(lunchLength(0))
            Case "Tue"
                tempHours = endTime(1).Subtract(startTime(1))
                officialHours = tempHours.Subtract(lunchLength(1))
            Case "Wed"
                tempHours = endTime(2).Subtract(startTime(2))
                officialHours = tempHours.Subtract(lunchLength(2))
            Case "Thu"
                tempHours = endTime(3).Subtract(startTime(3))
                officialHours = tempHours.Subtract(lunchLength(3))
            Case "Fri"
                tempHours = endTime(4).Subtract(startTime(4))
                officialHours = tempHours.Subtract(lunchLength(4))
        End Select

        Return officialHours
    End Function

    Public Sub CreateWorkingHours()
        Dim stringHandling As New ErrorHandling
        For i = 0 To 4
            lunchLength(i) = TimeSpan.Parse("12:00:00")
        Next
        For i = 0 To 4
            Select Case i
                Case 0
                    If YesNo("Are you working on Monday") = True Then
                        Console.WriteLine("Monday")
                        While startTime(0) >= endTime(0)
                            Console.WriteLine("Enter start time:")
                            startTime(0) = stringHandling.GetTime
                            Console.WriteLine("Enter end time:")
                            endTime(0) = stringHandling.GetTime
                        End While
                        While lunchLength(0) > TimeSpan.Parse("01:00:00")
                            Console.WriteLine("Enter lunch length:")
                            lunchLength(0) = stringHandling.GetTime
                        End While
                    End If
                Case 1
                    If YesNo("Are you working on Tuesday") = True Then
                        Console.WriteLine("Tuesday")
                        While startTime(1) >= endTime(1)
                            Console.WriteLine("Enter start time:")
                            startTime(1) = stringHandling.GetTime
                            Console.WriteLine("Enter end time:")
                            endTime(1) = stringHandling.GetTime
                        End While
                        While lunchLength(1) > TimeSpan.Parse("01:00:00")
                            Console.WriteLine("Enter lunch length:")
                            lunchLength(1) = stringHandling.GetTime
                        End While
                    End If
                Case 2
                    If YesNo("Are you working on Wednesday") = True Then
                        Console.WriteLine("Wednesday")
                        While startTime(2) >= endTime(2)
                            Console.WriteLine("Enter start time:")
                            startTime(2) = stringHandling.GetTime
                            Console.WriteLine("Enter end time:")
                            endTime(2) = stringHandling.GetTime
                        End While
                        While lunchLength(2) > TimeSpan.Parse("01:00:00")
                            Console.WriteLine("Enter lunch length:")
                            lunchLength(2) = stringHandling.GetTime
                        End While
                    End If
                Case 3
                    If YesNo("Are you working on Thursday") = True Then
                        Console.WriteLine("Thursday")
                        While startTime(3) >= endTime(3)
                            Console.WriteLine("Enter start time:")
                            startTime(3) = stringHandling.GetTime
                            Console.WriteLine("Enter end time:")
                            endTime(3) = stringHandling.GetTime
                        End While
                        While lunchLength(3) > TimeSpan.Parse("01:00:00")
                            Console.WriteLine("Enter lunch length:")
                            lunchLength(3) = stringHandling.GetTime
                        End While
                    End If
                Case 4
                    If YesNo("Are you working on Friday") = True Then
                        Console.WriteLine("Friday")
                        While startTime(4) >= endTime(4)
                            Console.WriteLine("Enter start time:")
                            startTime(4) = stringHandling.GetTime
                            Console.WriteLine("Enter end time:")
                            endTime(4) = stringHandling.GetTime
                        End While
                        While lunchLength(4) > TimeSpan.Parse("01:00:00")
                            Console.WriteLine("Enter lunch length:")
                            lunchLength(4) = stringHandling.GetTime
                        End While
                    End If
            End Select
        Next
    End Sub

    Public Sub InsertWorkingHours()
        For i = 0 To 4
            If lunchLength(i).ToString <> "12:00:00" Then
                Dim sqlInsertWH As New Odbc.OdbcCommand("INSERT INTO workinghours(audiologistid, DAY, starttime, endtime, lunchlength) VALUES (?, ?, ?, ?, ?)", Module1.GetConnection)
                sqlInsertWH.Parameters.AddWithValue("audiologistid", audiologistID)
                Select Case i
                    Case 0
                        sqlInsertWH.Parameters.AddWithValue("day", "Mon")
                    Case 1
                        sqlInsertWH.Parameters.AddWithValue("day", "Tue")
                    Case 2
                        sqlInsertWH.Parameters.AddWithValue("day", "Wed")
                    Case 3
                        sqlInsertWH.Parameters.AddWithValue("day", "Thu")
                    Case 4
                        sqlInsertWH.Parameters.AddWithValue("day", "Fri")
                    Case Else
                        sqlInsertWH.Parameters.AddWithValue("day", "Err")
                End Select
                sqlInsertWH.Parameters.AddWithValue("starttime", startTime(i))
                sqlInsertWH.Parameters.AddWithValue("endtime", endTime(i))
                sqlInsertWH.Parameters.AddWithValue("lunchlength", lunchLength(i))
                sqlInsertWH.ExecuteNonQuery()
            End If
        Next

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
        Console.Clear()
        Select Case currentChoice
            Case 1
                Return True
            Case 2
                Return False
        End Select
        Return False
    End Function

    Public Sub PrintWorkingHours()
        For i = 0 To 4
            Select Case i
                Case 0
                    If startTime(i).ToString <> "00:00:00" And endTime(i).ToString <> "00:00:00" Then
                        Console.WriteLine("Monday")
                    End If
                Case 1
                    If startTime(i).ToString <> "00:00:00" And endTime(i).ToString <> "00:00:00" Then
                        Console.WriteLine("Tuesday")
                    End If
                Case 2
                    If startTime(i).ToString <> "00:00:00" And endTime(i).ToString <> "00:00:00" Then
                        Console.WriteLine("Wednesday")
                    End If
                Case 3
                    If startTime(i).ToString <> "00:00:00" And endTime(i).ToString <> "00:00:00" Then
                        Console.WriteLine("Thursday")
                    End If
                Case 4
                    If startTime(i).ToString <> "00:00:00" And endTime(i).ToString <> "00:00:00" Then
                        Console.WriteLine("Friday")
                    End If
            End Select
            If startTime(i).ToString <> "00:00:00" And endTime(i).ToString <> "00:00:00" Then
                Console.WriteLine(startTime(i).ToString & " - " & endTime(i).ToString)
            End If

        Next
    End Sub

    Public Function FindMaxApps() As Integer
        Dim totalHours As Double = 0
        Dim tempHours As TimeSpan
        Dim rsFindHours As Odbc.OdbcDataReader
        Dim sqlFindHours As New Odbc.OdbcCommand("SELECT TIMEDIFF(endtime, starttime) FROM workinghours WHERE audiologistid = ?", Module1.GetConnection)
        sqlFindHours.Parameters.AddWithValue("audiologistid", audiologistID)
        rsFindHours = sqlFindHours.ExecuteReader
        While rsFindHours.Read
            tempHours = rsFindHours("timediff(endtime, starttime)")
            totalHours += tempHours.TotalHours
        End While
        Return totalHours \ 5
    End Function

    Public Sub EditWorkingHours()
        Dim stringHandling As New ErrorHandling()
        Dim rsGetWHDay As Odbc.OdbcDataReader
        Dim dayOfWork As Integer = PrintEditWorkingHours()
        Dim day1 As String = ""
        While dayOfWork <> 6
            Console.Clear()
            Select Case dayOfWork
                Case 1
                    day1 = "Mon"
                    Console.WriteLine("Monday")
                Case 2
                    day1 = "Tue"
                    Console.WriteLine("Tuesday")
                Case 3
                    day1 = "Wed"
                    Console.WriteLine("Wednesday")
                Case 4
                    day1 = "Thu"
                    Console.WriteLine("Thursday")
                Case 5
                    day1 = "Fri"
                    Console.WriteLine("Friday")
            End Select
            Dim sqlGetWH As New Odbc.OdbcCommand("SELECT starttime, endtime, lunchlength FROM workinghours WHERE audiologistid = ? AND DAY = ?", Module1.GetConnection)
            sqlGetWH.Parameters.AddWithValue("audiologistid", audiologistID)
            sqlGetWH.Parameters.AddWithValue("day", day1)
            rsGetWHDay = sqlGetWH.ExecuteReader
            If rsGetWHDay.Read() Then
                Console.WriteLine("Current working hours: " & rsGetWHDay("starttime").ToString & " - " & rsGetWHDay("endtime").ToString)
                Console.WriteLine("Current lunch hours: " & rsGetWHDay("lunchlength").ToString)
            End If
            startTime(dayOfWork - 1) = TimeSpan.Parse("23:59:59")
            lunchLength(dayOfWork - 1) = TimeSpan.Parse("12:00:00")
            While startTime(dayOfWork - 1) >= endTime(dayOfWork - 1)
                Console.WriteLine("Enter new start time:")
                startTime(dayOfWork - 1) = stringHandling.GetTime
                Console.WriteLine("Enter new end time:")
                endTime(dayOfWork - 1) = stringHandling.GetTime
            End While
            While lunchLength(dayOfWork - 1) > TimeSpan.Parse("01:00:00")
                Console.WriteLine("Enter new lunch length:")
                lunchLength(dayOfWork - 1) = stringHandling.GetTime
            End While
            Dim sqlChangeHours As New Odbc.OdbcCommand("UPDATE workinghours SET startTime = ? , endTime = ?, lunchlength = ? WHERE audiologistID = ? AND DAY = ?", Module1.GetConnection)
            sqlChangeHours.Parameters.AddWithValue("starttime", startTime(dayOfWork - 1))
            sqlChangeHours.Parameters.AddWithValue("endtime", endTime(dayOfWork - 1))
            sqlChangeHours.Parameters.AddWithValue("lunchlength", lunchLength(dayOfWork - 1))
            sqlChangeHours.Parameters.AddWithValue("audiologistid", audiologistID)
            sqlChangeHours.Parameters.AddWithValue("day", day1)
            sqlChangeHours.ExecuteNonQuery()
            dayOfWork = PrintEditWorkingHours()
        End While
        Console.Clear()
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("Success. Audiologist working hours have been changed.")
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

    Public Function PrintEditWorkingHours() As Integer
        Console.CursorVisible = False
        Dim currentChoice As Integer = 1
        Dim choice As ConsoleKey
        Console.Clear()
        Console.WriteLine("Choose day to edit:
   Monday
   Tuesday
   Wednesday
   Thursday
   Friday
   FINISH
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
                    If currentChoice < 6 Then
                        Console.SetCursorPosition(0, currentChoice)
                        Console.Write("  ")
                        currentChoice += 1
                        Console.SetCursorPosition(0, currentChoice)
                        Console.Write(" >")
                    End If
            End Select
        Loop Until choice = ConsoleKey.Enter
        Console.CursorVisible = True
        Console.Clear()
        Return currentChoice
    End Function

End Class
