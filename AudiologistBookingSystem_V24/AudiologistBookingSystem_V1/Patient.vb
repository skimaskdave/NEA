Public Class Patient

    Private patientID As Integer
    Private firstName, surname, postcode, houseNumber, phoneNumber, email, additionalDisabilities As String
    Private dob As Date
    Private company, processor, implant As String

    Public Sub New(ByVal fName As String, ByVal sName As String)
        firstName = fName
        surname = sName
    End Sub

    Public Sub ChangePatName()
        Dim stringhandling As New ErrorHandling
        Console.Write("Enter patient first name: ")
        firstName = stringhandling.TryString(1).ToUpper
        Console.Write("Enter patient surname: ")
        surname = stringhandling.TryString(1).ToUpper
    End Sub

    'getting patient info if they exist/creating a new patient in the database.
    Public Function CheckPatient() As Boolean
        Dim output As Boolean
        Dim rsPatientCount As Odbc.OdbcDataReader
        Dim sqlPatientCount As New Odbc.OdbcCommand("SELECT COUNT(*) FROM patients where firstname = ? and surname = ?", Module1.GetConnection)
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

    Public Sub GetPatientInfo(ByVal choice As Integer)
        Dim gotInfo As Boolean = False
        Do Until gotInfo = True
            Select Case choice
                Case 1 'create new patient in the database
                    CreateNewPatient()
                    Console.ForegroundColor = ConsoleColor.Green
                    Console.WriteLine("Patient added!")
                    Console.ForegroundColor = ConsoleColor.Gray
                    gotInfo = True
                Case 2 'search existing patients in the database
                    Console.Clear()
                    Console.WriteLine("Searching Existing Patients...")
                    Dim numOfPat As Integer = 0
                    Dim patCheck As Boolean
                    patCheck = CheckPatient()
                    Dim stringHandling As New ErrorHandling()
                    Dim rsSearchPatients As Odbc.OdbcDataReader
                    If patCheck = True Then
                        Dim rsPatCount As Odbc.OdbcDataReader
                        Dim sqlPatCount As New Odbc.OdbcCommand("select count(*) from patients where firstname = ? and surname = ?", Module1.GetConnection)
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
                                Dim sqlSearchPatients1 As New Odbc.OdbcCommand("select * from patients where firstname = ? and surname = ? and postcode = ?", Module1.GetConnection) 'find patient with postcode as well
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
                                End If
                            End While
                            Console.WriteLine("Patient found.")
                        ElseIf numOfPat = 1 Then
                            While gotInfo = False
                                Dim sqlSearchPatients2 As New Odbc.OdbcCommand("select * from patients where firstname = ? and surname = ?", Module1.GetConnection)
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
                                    ChangePatName()
                                End If
                            End While
                            Console.WriteLine("Patient found.")
                        End If
                    Else
                        Console.WriteLine("No patients with this name exist.")
                        Console.WriteLine("Press any key to continue...")
                        Console.ReadKey()
                        ChangePatName()
                        patCheck = CheckPatient()
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

    Public Sub CreateNewPatient()
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
                    phoneNumber = stringHandling.TryPhone()
                Case 4
                    Console.Clear()
                    Console.WriteLine("Enter email: ")
                    email = stringHandling.TryEmail.ToLower
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
        Dim sqlAddPatient As New Odbc.OdbcCommand("INSERT INTO patients(firstname, surname, postcode, housenumbername, dob, company, processor, implant) VALUES(?, ?, ?, ?, ?, ?, ?, ?)", Module1.GetConnection)
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
        Dim sqlGetPatientID As New Odbc.OdbcCommand("select patientid from patients where firstname = ? and surname = ?", Module1.GetConnection)
        sqlGetPatientID.Parameters.AddWithValue("firstname", firstName)
        sqlGetPatientID.Parameters.AddWithValue("surname", surname)
        rsGetPatientID = sqlGetPatientID.ExecuteReader
        If rsGetPatientID.Read Then
            patientID = rsGetPatientID("patientid")
        End If

        If phoneNumber <> "" Then
            Dim sqlChangePhoneNumber As New Odbc.OdbcCommand("update patients set phonenumber = ?", Module1.GetConnection)
            sqlChangePhoneNumber.Parameters.AddWithValue("phonenumber", phoneNumber)
            sqlChangePhoneNumber.ExecuteNonQuery()
        End If
        If email <> "" Then
            Dim sqlChangeEmail As New Odbc.OdbcCommand("update patients set email = ?", Module1.GetConnection)
            sqlChangeEmail.Parameters.AddWithValue("email", email)
            sqlChangeEmail.ExecuteNonQuery()
        End If
        If additionalDisabilities <> "" Then
            Dim sqlAddDis As New Odbc.OdbcCommand("update patients set additionaldisabilities = ?", Module1.GetConnection)
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

    Public Function ReturnPatientID() As Integer
        Dim rsGetPatID As Odbc.OdbcDataReader
        Dim sqlGetPatID As New Odbc.OdbcCommand("select patientid from patients where firstname = ? and surname = ?", Module1.GetConnection)
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

    Public Sub PrintHistory()
        Dim stringHandling As New ErrorHandling
        Dim rsAddDis As Odbc.OdbcDataReader
        Dim sqlAddDis As New Odbc.OdbcCommand("select additionaldisabilities from patients where patientid = " & patientID & "", Module1.GetConnection)
        rsAddDis = sqlAddDis.ExecuteReader
        Console.WriteLine("Disabilities:")
        While rsAddDis.Read
            Console.WriteLine(rsAddDis("additionaldisabilities").ToString)
        End While
        Console.WriteLine()

        Dim rsNotes As Odbc.OdbcDataReader
        Dim sqlNotes As New Odbc.OdbcCommand("select notes, date from patientbooking where patientid = " & patientID & " order by date", Module1.GetConnection)
        rsNotes = sqlNotes.ExecuteReader
        Console.WriteLine("Previous patient appointments (and notes):")
        While rsNotes.Read
            Console.WriteLine(rsNotes("notes").ToString & " - " & stringHandling.SQLDate(rsNotes("date")))
        End While
        Console.WriteLine()

        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
    End Sub

    Public Sub GetPatInfoProfile(ByVal pc As String)
        postcode = pc
        Dim rsSearchPatients As Odbc.OdbcDataReader
        Dim sqlSearchPatients1 As New Odbc.OdbcCommand("select * from patients where firstname = ? and surname = ? and postcode = ?", Module1.GetConnection) 'find patient with postcode as well
        sqlSearchPatients1.Parameters.AddWithValue("@firstname", firstName)
        sqlSearchPatients1.Parameters.AddWithValue("@surname", surname)
        sqlSearchPatients1.Parameters.AddWithValue("@postcode", postcode)
        rsSearchPatients = sqlSearchPatients1.ExecuteReader
        If rsSearchPatients.Read Then 'take patient information in, or return back to start of getting info.
            patientID = rsSearchPatients("patientID")
            houseNumber = rsSearchPatients("houseNumberName")
            phoneNumber = rsSearchPatients("phoneNumber").ToString
            email = rsSearchPatients("email").ToString
            dob = rsSearchPatients("dob")
            additionalDisabilities = rsSearchPatients("additionalDisabilities").ToString
            company = rsSearchPatients("company")
            processor = rsSearchPatients("processor")
            implant = rsSearchPatients("implant")
        End If
    End Sub

    Public Sub PrintPatProfile()
        Console.Clear()
        Dim stringHandling As New ErrorHandling
        Console.WriteLine("Patient: " & firstName & " " & surname)
        If phoneNumber <> "" Then
            Console.WriteLine("Tel Num: " & phoneNumber)
        End If
        If email <> "" Then
            Console.WriteLine("Email: " & email)
        End If
        Console.WriteLine("Age: " & DateDiff(DateInterval.Year, dob, Date.Today) & " years (DOB: " & stringHandling.SQLDate(dob.ToString) & ")")
        Console.WriteLine("House number/name: " & houseNumber)
        Console.WriteLine("Postcode: " & postcode)
        If additionalDisabilities = "" Then
            Console.WriteLine("Additional disabilties: None")
        Else
            Console.WriteLine("Additional disabilties: " & additionalDisabilities)
        End If
        Console.WriteLine()
        Console.WriteLine(company)
        Console.WriteLine("Implant: " & implant)
        Console.WriteLine("Processor: " & processor)
        Console.WriteLine()
    End Sub

    Public Sub ChangePatNameDB()
        Dim sqlChangePatName As New Odbc.OdbcCommand("UPDATE patients SET firstName = ?, surname = ? WHERE patientID = ?", Module1.GetConnection())
        sqlChangePatName.Parameters.AddWithValue("firstName", firstName)
        sqlChangePatName.Parameters.AddWithValue("surname", surname)
        sqlChangePatName.Parameters.AddWithValue("patientID", patientID)
        sqlChangePatName.ExecuteNonQuery()
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("Success. Patient name has been changed.")
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

    Public Sub ChangePatTel(ByVal telNum As String)
        phoneNumber = telNum
        Dim sqlChangeTelNum As New Odbc.OdbcCommand("UPDATE patients SET phoneNumber = ? WHERE patientID = ?", Module1.GetConnection())
        sqlChangeTelNum.Parameters.AddWithValue("phoneNumber", phoneNumber)
        sqlChangeTelNum.Parameters.AddWithValue("patientID", patientID)
        sqlChangeTelNum.ExecuteNonQuery()
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("Success. Patient phone number has been changed.")
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

    Public Sub ChangePatEmail(ByVal uEmail As String)
        email = uEmail
        Dim sqlChangeEmail As New Odbc.OdbcCommand("UPDATE patients SET email = ? WHERE patientID = ?", Module1.GetConnection())
        sqlChangeEmail.Parameters.AddWithValue("email", email)
        sqlChangeEmail.Parameters.AddWithValue("patientiD", patientID)
        sqlChangeEmail.ExecuteNonQuery()
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("Success. Patient email has been changed.")
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

    Public Sub ChangePatDOB(ByVal newDOB As Date)
        Dim stringHandling As New ErrorHandling()
        dob = newDOB
        Dim sqlChangeDOB As New Odbc.OdbcCommand("UPDATE patients SET dob = ? WHERE patientID = ?", Module1.GetConnection())
        sqlChangeDOB.Parameters.AddWithValue("dob", DOB)
        sqlChangeDOB.Parameters.AddWithValue("patientID", patientID)
        sqlChangeDOB.ExecuteNonQuery()
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("Success. Patient date of birth has been changed to " & stringHandling.SQLDate(DOB) & ".")
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

    Public Sub ChangePatCompany()
        Dim sqlChangeCompany As New Odbc.OdbcCommand("UPDATE patients SET company = ? WHERE patientID = ?", Module1.GetConnection())
        sqlChangeCompany.Parameters.AddWithValue("company", company)
        sqlChangeCompany.Parameters.AddWithValue("patientID", patientID)
        sqlChangeCompany.ExecuteNonQuery()
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("Success. Patient company has been changed.")
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

    Public Sub ChangePatImplant()
        Dim sqlChangeImplant As New Odbc.OdbcCommand("UPDATE patients SET implant = ? WHERE patientID = ?", Module1.GetConnection())
        sqlChangeImplant.Parameters.AddWithValue("implant", implant)
        sqlChangeImplant.Parameters.AddWithValue("patientID", patientID)
        sqlChangeImplant.ExecuteNonQuery()
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("Success. Patient implant has been changed.")
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub
    
    Public Sub ChangePatProcessor()
        Dim sqlChangeProcessor As New Odbc.OdbcCommand("UPDATE patients SET processor = ? WHERE patientID = ?", Module1.GetConnection())
        sqlChangeProcessor.Parameters.AddWithValue("processor", processor)
        sqlChangeProcessor.Parameters.AddWithValue("patientID", patientID)
        sqlChangeProcessor.ExecuteNonQuery
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("Success. Patient processor has been changed.")
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

    Public Sub AddDis()
        Dim stringHandling As New ErrorHandling()
        Console.Writeline("Enter patient additional disabilities: ")
        additionalDisabilities = stringHandling.TryString(1, 255)
    End Sub

    Public Sub ChangePatAddDis()
        Dim sqlChangeAddDis As New Odbc.OdbcCommand("UPDATE patients SET additionaldisabilities = ? WHERE patientID = ?", Module1.GetConnection())
        sqlChangeAddDis.Parameters.AddWithValue("additionaldisabilities", additionalDisabilities)
        sqlChangeAddDis.Parameters.AddWithValue("patientID", patientID)
        sqlChangeAddDis.ExecuteNonQuery()
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("Success. Patient additional disabilities has been changed.")
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub


End Class
