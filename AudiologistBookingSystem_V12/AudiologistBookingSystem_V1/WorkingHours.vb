﻿Public Class WorkingHours

    Private audiologistID As Integer
    Private startTime(4) As TimeSpan
    Private endTime(4) As TimeSpan
    Private lunchLength(4) As TimeSpan

    Public Sub New(ByVal audID As Integer)
        audiologistID = audID
    End Sub

    Public Sub GetWorkingHours(ByVal conn As System.Data.Odbc.OdbcConnection)
        Dim rsGetWorkHours As Odbc.OdbcDataReader
        Dim sqlGetWorkHours As New Odbc.OdbcCommand("select * from workinghours where audiologistid = ?", conn)
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

End Class
