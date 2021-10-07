Public Class Patient

    Private patientID As Integer
    Private firstName, surname, postcode, houseNumber, phoneNumber, email, additionalDisabilities As String
    Private dob As Date
    Private company, processor, implant As String

    Structure PatientCheckStruc
        Dim exists As Boolean
        Dim numOfPats As Integer
    End Structure

    Public Sub New(ByVal fName As String, ByVal sName As String)
        firstName = fName
        surname = sName
    End Sub


    'getting patient info if they exist/creating a new patient in the database.
    Public Function CheckPatient(ByVal conn As System.Data.Odbc.OdbcConnection) As PatientCheckStruc
        Dim output As PatientCheckStruc
        Dim rsPatientCount As Odbc.OdbcDataReader
        Dim sqlPatientCount As New Odbc.OdbcCommand("SELECT COUNT(*) FROM patients where firstname = ? and surname = ?", conn)
        sqlPatientCount.Parameters.AddWithValue("@firstname", firstName)
        sqlPatientCount.Parameters.AddWithValue("@surname", surname)
        rsPatientCount = sqlPatientCount.ExecuteReader
        If rsPatientCount.Read Then
            If rsPatientCount("Count(*)") > 0 Then
                output.exists = True
                output.numOfPats = rsPatientCount("Count(*)")
            Else
                output.exists = False
                output.numOfPats = 0
            End If
        End If
        Return output
    End Function 'does the patient exist (when searched for in the database)

    Public Sub GetPatientInfo(ByVal conn As System.Data.Odbc.OdbcConnection)
        Dim choice As Integer
        Dim gotInfo As Boolean = False
        Do Until gotInfo = True
            choice = CreateOrSearch()
            Select Case choice
                Case 1 'create new patient in the database
                    CreateNewPatient(conn)
                    Console.ForegroundColor = ConsoleColor.Green
                    Console.WriteLine("Patient added!")
                    Console.ForegroundColor = ConsoleColor.Gray
                Case 2 'search existing patients in the database
                    Console.Clear()
                    Console.WriteLine("Searching Existing Patients...")
                    Dim patCheck As PatientCheckStruc
                    patCheck = CheckPatient(conn)
                    Dim stringHandling As New ErrorHandling()
                    Dim rsSearchPatients As Odbc.OdbcDataReader
                    If patCheck.exists = True Then
                        If patCheck.numOfPats > 1 Then
                            While gotInfo = False
                                Console.WriteLine("Enter postcode: ")
                                postcode = stringHandling.TryString(6, 10).ToUpper
                                Dim sqlSearchPatients1 As New Odbc.OdbcCommand("select * from patients where firstname = ? and surname = ? and postcode = ?", conn) 'find patient with postcode as well
                                sqlSearchPatients1.Parameters.AddWithValue("@firstname", firstName)
                                sqlSearchPatients1.Parameters.AddWithValue("@surname", surname)
                                sqlSearchPatients1.Parameters.AddWithValue("@postcode", postcode)
                                rsSearchPatients = sqlSearchPatients1.ExecuteReader
                                If rsSearchPatients.Read Then 'take patient information in, or return back to start of getting info.
                                    gotInfo = True
                                    patientID = rsSearchPatients("patientID")
                                    houseNumber = rsSearchPatients("houseNumberName")
                                    phoneNumber = rsSearchPatients("phoneNumber").ToString
                                    email = rsSearchPatients("email").ToString
                                    dob = rsSearchPatients("dob")
                                    additionalDisabilities = rsSearchPatients("additionalDisabilities").ToString
                                    company = rsSearchPatients("company")
                                    processor = rsSearchPatients("processor")
                                    implant = rsSearchPatients("implant")
                                Else
                                    Console.WriteLine("Patient does not exist at this postcode.")
                                    Console.WriteLine("Press any key to continue...")
                                    Console.ReadKey()
                                End If
                            End While

                            Console.WriteLine("Patient found.")
                        End If
                        If patCheck.numOfPats = 1 Then
                            While gotInfo = False
                                Dim sqlSearchPatients2 As New Odbc.OdbcCommand("select * from patients where firstname = ? and surname = ?", conn)
                                sqlSearchPatients2.Parameters.AddWithValue("@firstname", firstName)
                                sqlSearchPatients2.Parameters.AddWithValue("@surname", surname)
                                rsSearchPatients = sqlSearchPatients2.ExecuteReader
                                If rsSearchPatients.Read Then
                                    gotInfo = True
                                    patientID = rsSearchPatients("patientID")
                                    postcode = rsSearchPatients("postcode")
                                    houseNumber = rsSearchPatients("houseNumberName")
                                    phoneNumber = rsSearchPatients("phoneNumber").ToString
                                    email = rsSearchPatients("email").ToString
                                    dob = rsSearchPatients("dob")
                                    additionalDisabilities = rsSearchPatients("additionalDisabilities").ToString
                                    company = rsSearchPatients("company")
                                    processor = rsSearchPatients("processor")
                                    implant = rsSearchPatients("implant")
                                Else
                                    Console.WriteLine("Patient does not exist.")
                                    Console.WriteLine("Press any key to continue...")
                                    Console.ReadKey()
                                End If
                            End While
                            Console.WriteLine("Patient found.")
                        End If
                    Else
                        Console.WriteLine("No patients with this name exist.")
                        Console.WriteLine("Press any key to continue...")
                        Console.ReadKey()
                    End If
            End Select
        Loop

    End Sub

    Public Function PrintCreatePatient() As Integer
        Console.CursorVisible = False
        Dim currentChoice As Integer = 1
        Dim choice As ConsoleKey
        Console.Clear()
        Console.WriteLine("Enter (fields with * are required):
   Postcode*
   House Number*
   Phone Number
   Email
   Date Of Birth*
   Additional Disabilities (do not do unless patient has additional disabilties)
   Processor/Implant Manufacturer*
   Processor Model*
   Implant Model*
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
                    If currentChoice < 9 Then
                        Console.SetCursorPosition(0, currentChoice)
                        Console.Write("  ")
                        currentChoice += 1
                        Console.SetCursorPosition(0, currentChoice)
                        Console.Write(" >")
                    End If
            End Select
        Loop Until choice = ConsoleKey.Enter
        Console.CursorVisible = True
        Return currentChoice
    End Function 'printing a menu to get all the patient data

    Public Function CreateOrSearch() As Integer
        Console.CursorVisible = False
        Dim currentChoice As Integer = 1
        Dim choice As ConsoleKey
        Console.Clear()
        Console.WriteLine("Enter:
   Create New Patient
   Search For Existing Patient
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
        Return currentChoice
    End Function 'choosing to search for a patient or creating one

    Public Sub CreateNewPatient(ByVal conn As System.Data.Odbc.OdbcConnection)
        Console.Clear()
        Console.WriteLine("Create New Patient...")
        System.Threading.Thread.Sleep(200)
        Dim stringHandling As New ErrorHandling()
        Dim flags(5) As Boolean 'postcode, house number/name, dob, company, processor, implant
        While flags(0) = False Or flags(1) = False Or flags(2) = False Or flags(3) = False Or flags(4) = False Or flags(5) = False
            Select Case PrintCreatePatient()
                Case 1
                    Console.Clear()
                    Console.WriteLine("Enter postcode: ")
                    postcode = stringHandling.TryString(6, 10)
                    flags(0) = True
                Case 2
                    Console.Clear()
                    Console.WriteLine("Enter house number/name")
                    houseNumber = stringHandling.TryString(1)
                    flags(1) = True
                Case 3
                    Console.Clear()
                    Console.WriteLine("Enter phone number: ")
                    phoneNumber = stringHandling.TryString(11, 14)
                Case 4
                    Console.Clear()
                    Console.WriteLine("Enter email: ")
                    email = stringHandling.TryEmail
                Case 5
                    Console.Clear()
                    Console.WriteLine("Enter date of birth: ")
                    dob = stringHandling.GetDate3()
                    flags(2) = True
                Case 6
                    Console.Clear()
                    Console.WriteLine("Enter additional disabilties: ")
                    additionalDisabilities = stringHandling.TryString(1)
                Case 7
                    Console.Clear()
                    Console.WriteLine("Enter processor/implant manufacturer: ")
                    company = stringHandling.TryString(1)
                    flags(3) = True
                Case 8
                    Console.Clear()
                    Console.WriteLine("Enter processor model: ")
                    processor = stringHandling.TryString(1)
                    flags(4) = True
                Case 9
                    Console.Clear()
                    Console.WriteLine("Enter implant model: ")
                    implant = stringHandling.TryString(1)
                    flags(5) = True
            End Select
        End While
        Dim sqlAddPatient As New Odbc.OdbcCommand("insert into patients(firstname, surname, postcode, housenumbername, phonenumber, email, dob, additionaldisabilities, company, processor, implant)
values('" & firstName & "', '" & surname & "', '" & postcode & "', '" & houseNumber & "', '" & phoneNumber & "', '" & email & "', '" & stringHandling.SQLDate(dob) & "', '" & additionalDisabilities & "', '" & company & "', '" & processor & "', '" & implant & "')", conn)
        Try
            sqlAddPatient.ExecuteNonQuery()
        Catch ex As Exception
            Console.WriteLine("An error occured: " & ex.Message)
            Console.WriteLine("Press any key to continue...")
            Console.ReadKey()
        End Try

        Dim rsGetPatientID As Odbc.OdbcDataReader
        Dim sqlGetPatientID As New Odbc.OdbcCommand("select patientid from patients where firstname = ? and surname = ?", conn)
        sqlGetPatientID.Parameters.AddWithValue("firstname", firstName)
        sqlGetPatientID.Parameters.AddWithValue("surname", surname)
        rsGetPatientID = sqlGetPatientID.ExecuteReader
        If rsGetPatientID.Read Then
            patientID = rsGetPatientID("patientid")
        End If

        If phoneNumber = "" Then
            Dim sqlChangePhoneNumber As New Odbc.OdbcCommand("update patients set phonenumber = NULL", conn)
            sqlChangePhoneNumber.ExecuteNonQuery()
        End If
        If email = "" Then
            Dim sqlChangeEmail As New Odbc.OdbcCommand("update patients set email = NULL", conn)
            sqlChangeEmail.ExecuteNonQuery()
        End If
        If additionalDisabilities = "" Then
            Dim sqlAddDis As New Odbc.OdbcCommand("update patients set additionaldisabilities = NULL", conn)
            sqlAddDis.ExecuteNonQuery()
        End If


    End Sub 'inserting a new patient into the database

    'retrieving patient information and giving it to another part of the program
    Public Function ReturnChildStatus(ByVal appDate As Date) As Boolean 'child = true; adult = false
        If DateDiff(DateInterval.Year, appDate, dob) >= 18 Then
            Return False
        Else
            Return True
        End If
        Return False
    End Function

    Public Function ReturnPatientID(ByVal conn As System.Data.Odbc.OdbcConnection) As Integer
        Dim rsGetPatID As Odbc.OdbcDataReader
        Dim sqlGetPatID As New Odbc.OdbcCommand("select patientid from patients where firstname = ? and surname = ?", conn)
        sqlGetPatID.Parameters.AddWithValue("@firstname", firstName)
        sqlGetPatID.Parameters.AddWithValue("@surname", surname)
        rsGetPatID = sqlGetPatID.ExecuteReader
        If rsGetPatID.Read Then
            patientID = rsGetPatID("patientID")
        End If
        Return patientID
    End Function

    Public Function ReturnPatientName() As String
        Return firstName & " " & surname
    End Function

End Class
