Public Class Patient

    Private patientID As Integer
    Private firstName, surname, postcode, houseNumber, phoneNumber, email, additionalDisabilities As String
    Private dob As Date
    Private company, processor, implant As String

    Public Sub New(ByVal fName As String, ByVal sName As String)
        firstName = fName
        surname = sName
    End Sub


    'getting patient info if they exist/creating a new patient in the database.
    Public Function CheckPatient(ByVal conn As System.Data.Odbc.OdbcConnection) As Boolean
        Dim output As Boolean
        Dim rsPatientCount As Odbc.OdbcDataReader
        Dim sqlPatientCount As New Odbc.OdbcCommand("SELECT COUNT(*) FROM patients where firstname = ? and surname = ?", conn)
        sqlPatientCount.Parameters.AddWithValue("@firstname", firstName)
        sqlPatientCount.Parameters.AddWithValue("@surname", surname)
        rsPatientCount = sqlPatientCount.ExecuteReader
        If rsPatientCount.Read Then
            If rsPatientCount("Count(*)") > 0 Then
                output = True
            Else
                output = False
            End If
        End If
        Return output
    End Function 'does the patient exist (when searched for in the database)

    Public Sub GetPatientInfo(ByVal choice As Integer, ByVal conn As System.Data.Odbc.OdbcConnection)
        Dim gotInfo As Boolean = False
        Do Until gotInfo = True
            Select Case choice
                Case 1 'create new patient in the database
                    CreateNewPatient(conn)
                    Console.ForegroundColor = ConsoleColor.Green
                    Console.WriteLine("Patient added!")
                    Console.ForegroundColor = ConsoleColor.Gray
                    gotInfo = True
                Case 2 'search existing patients in the database
                    Console.Clear()
                    Console.WriteLine("Searching Existing Patients...")
                    Dim numOfPat As Integer = 0
                    Dim patCheck As Boolean
                    patCheck = CheckPatient(conn)
                    Dim stringHandling As New ErrorHandling()
                    Dim rsSearchPatients As Odbc.OdbcDataReader
                    If patCheck = True Then
                        Dim rsPatCount As Odbc.OdbcDataReader
                        Dim sqlPatCount As New Odbc.OdbcCommand("select count(*) from patients where firstname = ? and surname = ?", conn)
                        sqlPatCount.Parameters.AddWithValue("firstname", firstName)
                        sqlPatCount.Parameters.AddWithValue("surname", surname)
                        rsPatCount = sqlPatCount.ExecuteReader
                        If rsPatCount.Read Then
                            numOfPat = rsPatCount("COUNT(*)")
                        End If

                        If numOfPat > 1 Then
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
                        If numOfPat = 1 Then
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
   Processor/Implant Model*
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
                    If currentChoice < 8 Then
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
        Dim flags(4) As Boolean 'postcode, house number/name, dob, company, processor, implant
        While flags(0) = False Or flags(1) = False Or flags(2) = False Or flags(3) = False Or flags(4) = False
            Select Case PrintCreatePatient()
                Case 1
                    Console.Clear()
                    Console.WriteLine("Enter postcode: ")
                    postcode = stringHandling.TryString(6, 10).ToUpper
                    flags(0) = True
                Case 2
                    Console.Clear()
                    Console.WriteLine("Enter house number/name")
                    houseNumber = stringHandling.TryString(1).ToUpper
                    flags(1) = True
                Case 3
                    Console.Clear()
                    Console.WriteLine("Enter phone number: ")
                    phoneNumber = stringHandling.TryString(11, 14)
                Case 4
                    Console.Clear()
                    Console.WriteLine("Enter email: ")
                    email = stringHandling.TryEmail.ToUpper
                Case 5
                    Console.Clear()
                    Console.WriteLine("Enter date of birth: ")
                    dob = stringHandling.GetDate4()
                    flags(2) = True
                Case 6
                    Console.Clear()
                    Console.WriteLine("Enter additional disabilties: ")
                    additionalDisabilities = stringHandling.TryString(1).ToUpper
                Case 7
                    Console.Clear()
                    GetCompany()
                    flags(3) = True
                Case 8
                    Console.Clear()
                    GetImplant()
                    GetProcessor()
                    flags(4) = True
            End Select
        End While
        Dim sqlAddPatient As New Odbc.OdbcCommand("insert into patients(firstname, surname, postcode, housenumbername, dob, company, processor, implant)
values(?, ?, ?, ?, ?, ?, ?, ?)", conn)
        sqlAddPatient.Parameters.AddWithValue("firstname", firstName)
        sqlAddPatient.Parameters.AddWithValue("surname", surname)
        sqlAddPatient.Parameters.AddWithValue("postcode", postcode)
        sqlAddPatient.Parameters.AddWithValue("housenumbername", houseNumber)
        sqlAddPatient.Parameters.AddWithValue("dob", stringHandling.SQLDate(dob))
        sqlAddPatient.Parameters.AddWithValue("company", company)
        sqlAddPatient.Parameters.AddWithValue("processor", processor)
        sqlAddPatient.Parameters.AddWithValue("implant", implant)
        sqlAddPatient.ExecuteNonQuery()

        Dim rsGetPatientID As Odbc.OdbcDataReader
        Dim sqlGetPatientID As New Odbc.OdbcCommand("select patientid from patients where firstname = ? and surname = ?", conn)
        sqlGetPatientID.Parameters.AddWithValue("firstname", firstName)
        sqlGetPatientID.Parameters.AddWithValue("surname", surname)
        rsGetPatientID = sqlGetPatientID.ExecuteReader
        If rsGetPatientID.Read Then
            patientID = rsGetPatientID("patientid")
        End If

        If phoneNumber = "" Then
            Dim sqlChangePhoneNumber As New Odbc.OdbcCommand("update patients set phonenumber = ?", conn)
            sqlChangePhoneNumber.Parameters.AddWithValue("phonenumber", phoneNumber)
            sqlChangePhoneNumber.ExecuteNonQuery()
        End If
        If email = "" Then
            Dim sqlChangeEmail As New Odbc.OdbcCommand("update patients set email = ?", conn)
            sqlChangeEmail.Parameters.AddWithValue("email", email)
            sqlChangeEmail.ExecuteNonQuery()
        End If
        If additionalDisabilities = "" Then
            Dim sqlAddDis As New Odbc.OdbcCommand("update patients set additionaldisabilities = NULL", conn)
            sqlAddDis.Parameters.AddWithValue("additionaldisabilities", additionalDisabilities)
            sqlAddDis.ExecuteNonQuery()
        End If


    End Sub 'inserting a new patient into the database

    Public Sub GetCompany()
        Console.CursorVisible = False
        Dim currentChoice As Integer = 1
        Dim choice As ConsoleKey
        Console.Clear()
        Select Case company
            Case ""
        End Select
        Console.WriteLine("Enter processor/implant manufacturer: 
   Cochlear
   MED-EL
   Advanced Bionics
   Oticon Medical
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
                    If currentChoice < 4 Then
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
                company = "Cochlear"
            Case 2
                company = "MED-EL"
            Case 3
                company = "Advanced Bionics"
            Case 4
                company = "Oticon Medical"
        End Select
    End Sub

    Public Sub GetImplant()
        Console.CursorVisible = False
        Dim currentChoice As Integer = 1
        Dim choice As ConsoleKey
        Console.Clear()
        Select Case company
            Case "Cochlear"
                Console.WriteLine("Enter implant model:
   Nucleus CI632
   Nucleus CI612
   Nucleus CI522
   Nucleus CI512
   Nucleus Freedom (CI24RE(CA))
   Nucleus Contour (CI24R)
   Nucleus CI24M
   Nucleus CI22M
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
                            If currentChoice < 8 Then
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
                        implant = "Nucleus CI632"
                    Case 2
                        implant = "Nucleus CI612"
                    Case 3
                        implant = "Nucleus CI522"
                    Case 4
                        implant = "Nucleus CI512"
                    Case 5
                        implant = "Nucleus Freedom (CI24RE(CA))"
                    Case 6
                        implant = "Nucleus Contour (CI24R)"
                    Case 7
                        implant = "Nucleus CI24M"
                    Case 8
                        implant = "Nucleus CI22M"
                End Select
            Case "MED-EL"
                Console.WriteLine("Enter implant model:
   Synchrony 2
   Synchrony
   Concerto
   Sonata
   Combi 40+
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
                            If currentChoice < 5 Then
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
                        implant = "Synchrony 2"
                    Case 2
                        implant = "Synchrony"
                    Case 3
                        implant = "Concerto"
                    Case 4
                        implant = "Sonata"
                    Case 5
                        implant = "Combi 40+"
                End Select
            Case "Advanced Bionics"
                Console.WriteLine("Enter implant model:
   HiRes Ultra 3D
   HiRes Ultra
   HiRes90K Advantage
   HiRes90K
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
                            If currentChoice < 4 Then
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
                        implant = "HiRes Ultra 3D"
                    Case 2
                        implant = "HiRes Ultra"
                    Case 3
                        implant = "HiRes90K Advantage"
                    Case 4
                        implant = "HiRes90K"
                End Select
            Case "Oticon Medical"
                Console.WriteLine("Enter implant model:
   Neuro Zti
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
                            If currentChoice < 1 Then
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
                        implant = "Neuro Zti"
                End Select
        End Select

    End Sub

    Public Sub GetProcessor()
        Console.CursorVisible = False
        Dim currentChoice As Integer = 1
        Dim choice As ConsoleKey
        Console.Clear()
        Select Case company
            Case "Cochlear"
                Console.WriteLine("Enter processor:
   CP1000
   Kanso 2
   CP910
   Kanso
   CP810
   Freedom Processor
   ESPrit 3G
   ESPrit
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
                            If currentChoice < 8 Then
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
                        processor = "CP1000"
                    Case 2
                        processor = "Kanso 2"
                    Case 3
                        processor = "CP910"
                    Case 4
                        processor = "Kanso"
                    Case 5
                        processor = "CP810"
                    Case 6
                        processor = "Freedom Processor"
                    Case 7
                        processor = "ESPrit 3G"
                    Case 8
                        processor = "ESPrit"
                End Select
            Case "MED-EL"
                Console.WriteLine("Enter processor:
   Sonnet 2
   Rondo 3
   Sonnet
   Ronet 2
   Opus 2
   Rondo
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
                Select Case currentChoice
                    Case 1
                        processor = "Sonnet 2"
                    Case 2
                        processor = "Rondo 3"
                    Case 3
                        processor = "Sonnet"
                    Case 4
                        processor = "Rondo 2"
                    Case 5
                        processor = "Opus 2"
                    Case 6
                        processor = "Rondo"
                End Select
            Case "Advanced Bionics"
                Console.WriteLine("Enter processor:
   Marvel
   Naida Q90
   Naida Q70
   Neptune
   Harmony
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
                            If currentChoice < 5 Then
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
                        processor = "Marvel"
                    Case 2
                        processor = "Naida Q90"
                    Case 3
                        processor = "Naida Q70"
                    Case 4
                        processor = "Neptune"
                    Case 5
                        processor = "Harmony"
                End Select
            Case "Oticon Medical"
                Console.WriteLine("Enter processor:
   Neuro 2
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
                            If currentChoice < 1 Then
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
                        processor = "Neuro 2"
                End Select
        End Select

    End Sub

    'retrieving patient information and giving it to another part of the program
    Public Function ReturnChildStatus(ByVal appDate As Date) As Boolean 'child = true; adult = false
        If DateDiff(DateInterval.Year, dob, appDate) >= 18 Then
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

    Public Function ReturnPatientID()
        Return patientID
    End Function

    Public Function ReturnPatientName() As String
        Return firstName & " " & surname
    End Function

    Public Sub PrintHistory(ByVal conn As System.Data.Odbc.OdbcConnection)
        Dim rsAddDis As Odbc.OdbcDataReader
        Dim sqlAddDis As New Odbc.OdbcCommand("select additionaldisabilities from patients where patientid = " & patientID & "", conn)
        rsAddDis = sqlAddDis.ExecuteReader
        Console.WriteLine("Disabilities:")
        While rsAddDis.Read
            Console.WriteLine(rsAddDis("additionaldisabilities").ToString)
        End While
        Console.WriteLine()

        Dim rsNotes As Odbc.OdbcDataReader
        Dim sqlNotes As New Odbc.OdbcCommand("select notes, date from patientbooking where patientid = " & patientID & " order by date", conn)
        rsNotes = sqlNotes.ExecuteReader
        Console.WriteLine("Previous patient notes:")
        While rsNotes.Read
            Console.WriteLine(rsNotes("notes").ToString & " - " & rsNotes("date").ToString)
        End While
        Console.WriteLine()

        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
    End Sub

    Public Sub SearchAppointments()

    End Sub

End Class
