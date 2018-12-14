' This is a very simple application which is designed to make sure that the database is up and running so that Panopta has a page they can hit and make sure all is OK.
' We are also using this page to write to a basic activity log which should allow us to better track our performance over time.
' Note that we are adding the "=GetUTCDate()" to the various queries to force a new query each time so that we don't get cached data.

Imports System.Data.SqlClient
Imports System.Data

Partial Class _Default
    Inherits System.Web.UI.Page

    Public Function GetActiveUsers() As Integer

        'Determines how many users are "active" (activity in the last 10 minutes) on the system



        Dim sqlStatement As String = "select count(*) from users where datediff(mi,lastactivity,getutcdate()) <= 10 and lastactivity<>getutcdate()"
        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MainConnection").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)

        command.Connection.Open()

        'command.Parameters.AddWithValue("@OrganizationID", OrganizationID)
        'command.Parameters.AddWithValue("@ProductID", ProductID)



        Dim reader As SqlDataReader = command.ExecuteReader
        Try
            'return true if the ProductID exists for ths company
            If reader.Read Then

                Return reader(0)

            Else
                Return 0
            End If


        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try

    End Function

    Public Function GetTotalUsers() As Integer

        'Determines how many total users are logged in based on the LastPing


        Dim sqlStatement As String = "select count(*) from users where datediff(mi,lastping,getutcdate()) <= 2 and lastping <> getutcdate()"
        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MainConnection").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)

        command.Connection.Open()

        'command.Parameters.AddWithValue("@OrganizationID", OrganizationID)
        'command.Parameters.AddWithValue("@ProductID", ProductID)



        Dim reader As SqlDataReader = command.ExecuteReader
        Try
            'return true if the ProductID exists for ths company
            If reader.Read Then

                Return reader(0)

            Else
                Return 0
            End If


        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try

    End Function



    Public Sub UpdateActivityTable(ByVal SQLReadTime As Integer, ByVal SQLWriteTime As Integer, ByVal ActiveUsers As Integer, ByVal TotalUsers As Integer)
        'Writes to the TSActivityTable with the number of active and logged in users and the time it took to read the SQL




        Dim sqlStatement As String = "Insert into TSActivityLog(ReadQueryTime, WriteQueryTime, ActiveUsers, TotalUsers) Values(@ReadQueryTime, @WriteQueryTime, @ActiveUsers, @TotalUsers)"
        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MainConnection").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@ReadQueryTime", SQLReadTime))
        command.Parameters.Add(New SqlParameter("@WriteQueryTime", SQLWriteTime))
        command.Parameters.Add(New SqlParameter("@ActiveUsers", ActiveUsers))
        command.Parameters.Add(New SqlParameter("@TotalUsers", TotalUsers))



        command.Connection.Open()
        command.ExecuteNonQuery()
        command.Parameters.Clear()

        command.Connection.Close()
        command.Dispose()

        If connection.State = ConnectionState.Open Then connection.Close()



    End Sub

    Public Function TestConnection() As Boolean

        'Runs a simple query to make sure the database is alive.  Returns either a defined text string or an error message.





        Dim sqlStatement As String = "select top 1 * from tickets where datemodified <> @TestDate order by datemodified desc"
        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MainConnection").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)

        command.Connection.Open()

        command.Parameters.AddWithValue("@TestDate", Now())
        'command.Parameters.AddWithValue("@ProductID", ProductID)



        Dim reader As SqlDataReader = command.ExecuteReader
        Try
            'return true if the ProductID exists for ths company
            If reader.Read Then

                Return True
            Else
                Return False
            End If


        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try

    End Function

    Protected Sub TestWrite()
        'updates the first record in the TSActivityLog table - Used to test times


        Dim sqlStatement As String = "Update TSActivityLog Set TimeStamp = GetUTCDate(), ReadQueryTime = 0, ActiveUsers = 0, TotalUsers = 0 where CheckID = 1"
        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("MainConnection").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        

        command.Connection.Open()
        command.ExecuteNonQuery()
        command.Parameters.Clear()

        command.Connection.Close()
        command.Dispose()

        If connection.State = ConnectionState.Open Then connection.Close()


    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim Status As Boolean
        Dim StartTime As DateTime = Now
        Dim EndTime As DateTime
        Dim TotalUsers As Integer
        Dim ActiveUsers As Integer
        Dim ReadTime As Integer
        Dim WriteTime As Integer




        Try

            Status = TestConnection()


            TotalUsers = GetTotalUsers()
            ActiveUsers = GetActiveUsers()
            EndTime = Now

            Dim ts As New TimeSpan
            ts = EndTime - StartTime

            ReadTime = ts.TotalMilliseconds




            StartTime = Now
            TestWrite()
            EndTime = Now

            ts = EndTime - StartTime

            WriteTime = ts.TotalMilliseconds


            UpdateActivityTable(ReadTime, WriteTime, ActiveUsers, TotalUsers)



            If Status Then
                lbl_Status.Text = "TeamSupport running normally.  Readtime = " + ReadTime.ToString + ", WriteTime = " + WriteTime.ToString + ", ActiveUsers=" + ActiveUsers.ToString + ", TotalUsers=" + TotalUsers.ToString
            End If

        Catch ex As Exception
            lbl_Status.Text = "Error!  " + ex.Message.ToString

        End Try


    End Sub
End Class
