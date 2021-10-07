Public Class ErrorHandling

    Implements DateHandling

    Public Sub New()

    End Sub

    Public Function TryString(ByVal minLength As Integer) As String
        Dim flag As Boolean = False
        Dim uInput As String = ""
        Do Until flag = True
            Try
                flag = True
                uInput = Console.ReadLine()
                If uInput.Length < minLength Then
                    Console.WriteLine("Input not long enough.")
                    Throw New Exception
                End If
            Catch ex As Exception
                flag = False
                Console.WriteLine("An error occured. " & ex.Message)
            End Try
        Loop
        Return uInput
    End Function

    Public Function TryString(ByVal minLength As Integer, ByVal maxLength As Integer) As String
        Dim flag As Boolean = False
        Dim uInput As String = ""
        Do Until flag = True
            Try
                flag = True
                uInput = Console.ReadLine()
                If uInput.Length < minLength Then
                    Console.WriteLine("Input not long enough.")
                    Throw New Exception
                End If
                If uInput.Length > maxLength Then
                    Console.WriteLine("Too many characters inputted.")
                    Throw New Exception
                End If
            Catch ex As Exception
                flag = False
                Console.WriteLine("An error occured. " & ex.Message)
            End Try
        Loop
        Return uInput
    End Function

    Public Function TryEmail() As String
        Dim flag As Boolean = False
        Dim uInput As String = ""
        Do Until flag = True
            Try
                flag = True
                uInput = Console.ReadLine()
                If uInput.Contains("@") = False Or uInput.Length < 5 Then
                    Console.WriteLine("Make sure to enter a valid email.")
                    Throw New Exception
                End If
            Catch ex As Exception
                flag = False
                Console.WriteLine("An error occured. " & ex.Message)
            End Try
        Loop
        Return uInput
    End Function

    Public Function GetDate2(d1 As Date) As Date Implements DateHandling.GetDate2
        Dim date1 As Date
        Dim dateEdited As String
        Dim dates As String() = Split(d1, "-")
        dateEdited = dates(2) & "/" & dates(1) & "/" & dates(0)

        date1 = Date.Parse(dateEdited, New System.Globalization.CultureInfo("pt-EN"))
        Return date1
    End Function

    Public Function GetDate3() As Date Implements DateHandling.GetDate3
        Dim date1 As Date
        Dim flag As Boolean = False
        Do Until flag = True
            Try
                flag = True
                Console.WriteLine("Please enter a date in the format dd/mm/yyyy")
                date1 = Date.Parse(Console.ReadLine, New System.Globalization.CultureInfo("pt-EN"))
            Catch ex As Exception
                flag = False
                Console.WriteLine("An error occured. " & ex.Message)
            End Try
        Loop
        Return date1
    End Function

    Public Function GetDate() As Date Implements DateHandling.GetDate
        Console.Clear()
        Console.SetCursorPosition(0, 0)
        Dim errorFlag As Boolean = True
        Dim date1 As Date = Date.Now()
        Dim date2 As Date = Date.Now()
        Do Until DateDiff(DateInterval.Day, date2, date1) > 14 And errorFlag = False
            Console.WriteLine("The appointment must be in at least 2 weeks time.")
            Console.WriteLine("Please enter a date in the format dd/mm/yyyy")
            Try
                date1 = Date.Parse(Console.ReadLine, New System.Globalization.CultureInfo("pt-EN"))
                errorFlag = False
            Catch ex As Exception
                Console.WriteLine("An error occured: " & ex.Message)
                errorFlag = True
            End Try
        Loop
        Return date1
    End Function

    Public Function GetDateUrgent() As Date
        Console.Clear()
        Console.SetCursorPosition(0, 0)
        Dim errorFlag As Boolean = True
        Dim date1 As Date = Date.Today()
        Dim date2 As Date = Date.Today()
        Do Until DateDiff(DateInterval.Day, date2, date1) > 0 And errorFlag = False
            Console.WriteLine("The appointment must be in at least tomorrow.")
            Console.WriteLine("Please enter a date in the format dd/mm/yyyy")
            Try
                date1 = Date.Parse(Console.ReadLine, New System.Globalization.CultureInfo("pt-EN"))
                errorFlag = False
            Catch ex As Exception
                Console.WriteLine("An error occured: " & ex.Message)
                errorFlag = True
            End Try
        Loop
        Return date1
    End Function

    Public Function GetDateTimetable() As Date
        Console.Clear()
        Console.SetCursorPosition(0, 0)
        Dim errorFlag As Boolean = True
        Dim date1 As Date = Date.Today()
        Dim date2 As Date = Date.Today()
        Do Until errorFlag = False
            Console.WriteLine("Please enter a date in the format dd/mm/yyyy")
            Try
                date1 = Date.Parse(Console.ReadLine, New System.Globalization.CultureInfo("pt-EN"))
                If date1.DayOfWeek = 0 Or date1.DayOfWeek = 6 Then
                    Throw New Exception("Audiologists do not work weekends, please choose a different date.")
                End If
                errorFlag = False
            Catch ex As Exception
                Console.WriteLine("An error occured: " & ex.Message)
                errorFlag = True
            End Try
        Loop
        Return date1
    End Function

    Public Function SQLDate(d1 As Date) As String Implements DateHandling.SQLDate
        Dim strD1 As String
        strD1 = d1.ToShortDateString
        Dim splitDate As String() = Split(strD1, "/")
        Return splitDate(2) & "-" & splitDate(1) & "-" & splitDate(0)
    End Function
End Class
