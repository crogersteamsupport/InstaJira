' TS Metrics
' August 2009 - RJ
'
' This is a dashboard for internal TS use only - It shows the number of logins per day and actions per day over a time period.

Imports TeamSupport.WebUtils
Imports System.Data.SqlClient
Imports System.Data

Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Label1.Text = "<div>User ID: " + UserSession.LoginUser.UserID.ToString() + "</div>"
        'Label1.Text = Label1.Text + "<div>Organization ID: " + UserSession.LoginUser.OrganizationID.ToString() + "</div>"


        'To get more info about the logged in user use
        'Label1.Text = Label1.Text + "<div>Organization Name: " + UserSession.CurrentUser.OrganizationName + "</div>"
        'Label1.Text = Label1.Text + "<div>Full Name: " + UserSession.CurrentUser.DisplayName + "</div>"
        'Label1.Text = Label1.Text + "<div>Is Admin: " + UserSession.CurrentUser.IsSystemAdmin.ToString() + "</div>"
    End Sub


    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click



        'LoginsPerDay.SelectParameters("StartDate").DefaultValue = StartDate.SelectedDate
        'LoginsPerDay.SelectParameters("EndDate").DefaultValue = EndDate.SelectedDate

        'ActionsPerDay.SelectParameters("StartDate").DefaultValue = StartDate.SelectedDate
        'ActionsPerDay.SelectParameters("EndDate").DefaultValue = EndDate.SelectedDate


        frm_loginsperday.Attributes.Add("src", DisplayLoginsPerDay(StartDate.SelectedDate, EndDate.SelectedDate))
        frm_ActionsPerDay.Attributes.Add("src", DisplayActionsPerDay(StartDate.SelectedDate, EndDate.SelectedDate))

        'LoginsPerDay.DataBind()
        'RadChart1.DataBind()


        'ActionsPerDay.DataBind()
        'RadChart2.DataBind()



    End Sub


    Public Function CreateLineChartFromTable(ByVal datatable As DataTable, ByVal title As String) As String
        Dim Max As Integer = 0
        Dim min As Integer = 0


        'takes labels and data from a datatable and returns string link to google pie chart represenration

        Dim googlestring As String = "http://chart.apis.google.com/chart?chtt=" + title + "&chf=bg,s,DBE6F4&cht=lc&chs=600x300&chco=0000FF&chl="
        'title= chtt=<chart_title>
        'title style = chts=<color>,<font_size>


        'Lets get the max value first
        For max1 As Integer = 0 To datatable.Rows.Count - 1
            If datatable.Rows(max1)(1) > Max Then Max = datatable.Rows(max1)(1)
        Next

        'labels first column
        'googlestring = googlestring + "&chl="

        For y As Integer = 0 To datatable.Rows.Count - 1
            If y > 0 Then googlestring = googlestring + "|"
            googlestring = googlestring + datatable.Rows(y)(0).ToString
        Next

        'data is second column
        googlestring = googlestring + "&chd=t:"
        For x As Integer = 0 To datatable.Rows.Count - 1
            If x > 0 Then googlestring = googlestring + ","
            googlestring = googlestring + ((datatable.Rows(x)(1) / Max) * 100).ToString
            
        Next
        'chd=t:60,40&chs=250x100&chl=Hello|World&chco=0000FF"

        googlestring = googlestring + "&chxt=x,y&chxr=1,0," + Max.ToString

        Return googlestring

    End Function

    Public Function DisplayLoginsPerDay(ByVal startdate As String, ByVal enddate As String) As String


        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TSConnectionString").ConnectionString)
        Dim command As New SqlCommand("Select CONVERT(VARCHAR,dt.dtime,1) AS 'Date',Count(lh.userid) as 'NumLogins' FROM dbo.udfDateTimes(@StartDate,@enddate, 1, 'day') AS dt LEFT JOIN LoginHistory lh ON dt.dtime = CAST(FLOOR(CAST(lh.DateCreated AS FLOAT)) AS DATETIME)  group by dt.dtime", connection)
        Dim adapter As New SqlDataAdapter(command)
        Dim table As New DataTable


        ' Dim item1 As New Telerik.Web.UI.RadComboBoxItem()

        Try

            connection.Open()
            command.Parameters.AddWithValue("@StartDate", startdate)
            command.Parameters.AddWithValue("@EndDate", enddate)
            adapter.Fill(table)

            Dim HTMLString As String = CreateLineChartFromTable(table, "Logins+per+Day")


            connection.Close()

            Return HTMLString


        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function DisplayActionsPerDay(ByVal startdate As String, ByVal enddate As String) As String


        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TSConnectionString").ConnectionString)
        Dim command As New SqlCommand("Select CONVERT(VARCHAR,dt.dtime,1) AS 'Action Date',Count(a.datecreated) as 'Number of Actions' FROM dbo.udfDateTimes(@StartDate,@enddate, 1, 'day') AS dt LEFT JOIN Actions a ON dt.dtime = CAST(FLOOR(CAST(a.DateCreated AS FLOAT)) AS DATETIME)  group by dt.dtime", connection)
        Dim adapter As New SqlDataAdapter(command)
        Dim table As New DataTable


        ' Dim item1 As New Telerik.Web.UI.RadComboBoxItem()

        Try

            connection.Open()
            command.Parameters.AddWithValue("@StartDate", startdate)
            command.Parameters.AddWithValue("@EndDate", enddate)
            adapter.Fill(table)

            Dim HTMLString As String = CreateLineChartFromTable(table, "Actions+per+Day")


            connection.Close()

            Return HTMLString


        Catch ex As Exception
            Return ""
        End Try
    End Function

End Class
