' ProductsDashboard
' 9/3/09 (RJ)
'
' This report shows pie charts with the distribution of the various ticket types per product.
'  The layout is not perfect, and the Telerik controls are really twitchy when used in the design mode, but for now it's good enough.

Imports TeamSupport.WebUtils
Imports System.Data.SqlClient
Imports System.Data

Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub SetChartDimentions(ByRef ChartName As Telerik.Web.UI.RadChart)
        'The charts get stupid when you try to set this in the designer, so we're going to set at runtime
        ChartName.PlotArea.Appearance.Dimensions.AutoSize = False
        ChartName.PlotArea.Appearance.Dimensions.Height = 225
        ChartName.PlotArea.Appearance.Dimensions.Width = 330
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Label1.Text = "<div>User ID: " + UserSession.LoginUser.UserID.ToString() + "</div>"
        ' Label1.Text = Label1.Text + "<div>Organization ID: " + UserSession.LoginUser.OrganizationID.ToString() + "</div>"


        'To get more info about the logged in user use
        'Label1.Text = Label1.Text + "<div>Organization Name: " + UserSession.CurrentUser.OrganizationName + "</div>"
        'Label1.Text = Label1.Text + "<div>Full Name: " + UserSession.CurrentUser.DisplayName + "</div>"
        'Label1.Text = Label1.Text + "<div>Is Admin: " + UserSession.CurrentUser.IsSystemAdmin.ToString() + "</div>"


        'OpenTicketsByProduct.SelectParameters("OrganizationID").DefaultValue = UserSession.LoginUser.OrganizationID.ToString()



        'ltr_OpenTicketsByProduct.Text = DisplayOpenTicketsByProduct("1078")


        ' CustomerList.SelectParameters("OrganizationID").DefaultValue = UserSession.LoginUser.OrganizationID.ToString()
        CustomerList.SelectParameters("OrganizationID").DefaultValue = "1078"

        'TicketsByCustomer.Attributes.Add("src", DisplayTicketsByCustomer(UserSession.LoginUser.OrganizationID.ToString()))
        'IssuesByProduct.Attributes.Add("src", DisplayOpenIssuesByProduct("1078"))
        'TasksByProduct.Attributes.Add("src", DisplayOpenTasksByProduct("1078"))
        'OpenFeatures.Attributes.Add("src", DisplayOpenFeaturesByProduct("1078"))
        'OpenBugs.Attributes.Add("src", DisplayOpenBugsByProduct("1078"))



        Dim data(4) As Integer
        Dim labels(4) As String

        data(0) = 5
        data(1) = 7
        data(2) = 12
        data(3) = 3
        data(4) = 9

        labels(0) = "one"
        labels(1) = "two"
        labels(2) = "three"
        labels(3) = "four"
        labels(4) = "five"

        Dim teststr As String
        teststr = CreatePie(labels, data)


        'SetChartDimentions(TicketsByProduct)
        'SetChartDimentions(IssuesByProduct)
        'SetChartDimentions(FeaturesByProduct)
        'SetChartDimentions(BugsByProduct)
        'SetChartDimentions(TasksByProduct)

        'OpenTicketsByProduct.SelectParameters("OrganizationID").DefaultValue = UserSession.LoginUser.OrganizationID.ToString()
        'OpenTicketsByProduct.DataBind()

        'OpenIssuesByProduct.SelectParameters("OrganizationID").DefaultValue = UserSession.LoginUser.OrganizationID.ToString()
        'OpenIssuesByProduct.DataBind()

        'OpenBugsByProduct.SelectParameters("OrganizationID").DefaultValue = UserSession.LoginUser.OrganizationID.ToString()
        'OpenBugsByProduct.DataBind()

        'OpenFeaturesByProduct.SelectParameters("OrganizationID").DefaultValue = UserSession.LoginUser.OrganizationID.ToString()
        'OpenFeaturesByProduct.DataBind()

        'OpenTasksByProduct.SelectParameters("OrganizationID").DefaultValue = UserSession.LoginUser.OrganizationID.ToString()
        'OpenTasksByProduct.DataBind()


    End Sub




  



    Public Function CreatePie(ByVal labels() As String, ByVal data() As Integer) As String

        'takes data and labels as arrays and returns string link to google pie chart represenration

        Dim googlestring As String = "http://chart.apis.google.com/chart?cht=p3&chs=250x100&chco=0000FF&chd=t:"

        'data
        For x As Integer = 0 To data.Length - 1
            If x > 0 Then googlestring = googlestring + ","
            googlestring = googlestring + data(x).ToString

        Next

        'labels
        googlestring = googlestring + "&chl="

        For y As Integer = 0 To labels.Length - 1
            If y > 0 Then googlestring = googlestring + "|"
            googlestring = googlestring + labels(y).ToString

        Next

        'chd=t:60,40&chs=250x100&chl=Hello|World&chco=0000FF"

        Return googlestring

    End Function


    Public Function CreatePiefromTable(ByVal datatable As DataTable, ByVal title As String) As String

        'takes data and labels from a datatable and returns string link to google pie chart represenration

        Dim googlestring As String = "http://chart.apis.google.com/chart?chtt=" + title + "&chf=bg,s,DBE6F4&cht=p3&chs=500x200&chco=0000FF&chl="
        'title= chtt=<chart_title>
        'title style = chts=<color>,<font_size>

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
            googlestring = googlestring + datatable.Rows(x)(1).ToString
        Next
        'chd=t:60,40&chs=250x100&chl=Hello|World&chco=0000FF"

        Return googlestring

    End Function


    Public Function DisplayTicketsByCustomer(ByVal OrganizationID As String) As String


        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TSConnectionString").ConnectionString)
        Dim command As New SqlCommand("select tt.name, count(tt.name) from organizationtickets as ot, tickets as t, tickettypes as tt where ot.ticketid = t.ticketid and t.tickettypeid = tt.tickettypeid and ot.organizationid = @organizationid group by tt.name ", connection)

        Dim adapter As New SqlDataAdapter(command)
        Dim table As New DataTable


        ' Dim item1 As New Telerik.Web.UI.RadComboBoxItem()

        Try

            connection.Open()
            command.Parameters.AddWithValue("@OrganizationID", OrganizationID)
            adapter.Fill(table)

            Dim HTMLString As String = CreatePiefromTable(table, "Customer+Tickets")


            connection.Close()

            Return HTMLString


        Catch ex As Exception
            Return ""
        End Try
    End Function



    Public Function DisplayOpenIssuesByProduct(ByVal OrganizationID As String) As String
        'this is just example code at the moment - Need to clean up and request actual data then chart it

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TSConnectionString").ConnectionString)
        Dim command As New SqlCommand("select p.name as ProductName, count(*) as NumTickets  from tickets as t, products as p, ticketstatuses as ts, tickettypes as tt where t.productid = p.productid   and t.organizationid = @OrganizationID   and t.ticketstatusid = ts.ticketstatusid and ts.isclosed = 0  and t.tickettypeid = tt.tickettypeid and tt.name = 'issues' group by p.name", connection)
        Dim adapter As New SqlDataAdapter(command)
        Dim table As New DataTable


        ' Dim item1 As New Telerik.Web.UI.RadComboBoxItem()

        Try

            connection.Open()
            command.Parameters.AddWithValue("@OrganizationID", OrganizationID)
            adapter.Fill(table)

            Dim HTMLString As String = CreatePiefromTable(table, "Issues+by+Product")


            connection.Close()

            Return HTMLString


        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function DisplayOpenTasksByProduct(ByVal OrganizationID As String) As String
        'this is just example code at the moment - Need to clean up and request actual data then chart it

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TSConnectionString").ConnectionString)
        Dim command As New SqlCommand("select p.name as ProductName, count(*) as NumTickets from tickets as t, products as p, ticketstatuses as ts, tickettypes as tt where t.productid = p.productid   and t.organizationid = @OrganizationID   and t.ticketstatusid = ts.ticketstatusid and ts.isclosed = 0  and t.tickettypeid = tt.tickettypeid and tt.name = 'tasks' group by p.name", connection)
        Dim adapter As New SqlDataAdapter(command)
        Dim table As New DataTable


        ' Dim item1 As New Telerik.Web.UI.RadComboBoxItem()

        Try

            connection.Open()
            command.Parameters.AddWithValue("@OrganizationID", OrganizationID)
            adapter.Fill(table)

            Dim HTMLString As String = CreatePiefromTable(table, "Issues+by+Product")


            connection.Close()

            Return HTMLString


        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function DisplayOpenBugsByProduct(ByVal OrganizationID As String) As String
        'this is just example code at the moment - Need to clean up and request actual data then chart it

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TSConnectionString").ConnectionString)
        Dim command As New SqlCommand("select p.name as ProductName, count(*) as NumTickets from tickets as t, products as p, ticketstatuses as ts, tickettypes as tt where t.productid = p.productid   and t.organizationid = @OrganizationID   and t.ticketstatusid = ts.ticketstatusid and ts.isclosed = 0  and t.tickettypeid = tt.tickettypeid and tt.name = 'bugs' group by p.name", connection)
        Dim adapter As New SqlDataAdapter(command)
        Dim table As New DataTable


        ' Dim item1 As New Telerik.Web.UI.RadComboBoxItem()

        Try

            connection.Open()
            command.Parameters.AddWithValue("@OrganizationID", OrganizationID)
            adapter.Fill(table)

            Dim HTMLString As String = CreatePiefromTable(table, "Issues+by+Product")


            connection.Close()

            Return HTMLString


        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function DisplayOpenFeaturesByProduct(ByVal OrganizationID As String) As String
        'this is just example code at the moment - Need to clean up and request actual data then chart it

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TSConnectionString").ConnectionString)
        Dim command As New SqlCommand("select p.name as ProductName, count(*) as NumTickets from tickets as t, products as p, ticketstatuses as ts, tickettypes as tt where t.productid = p.productid   and t.organizationid = @OrganizationID   and t.ticketstatusid = ts.ticketstatusid and ts.isclosed = 0  and t.tickettypeid = tt.tickettypeid and tt.name = 'features' group by p.name", connection)
        Dim adapter As New SqlDataAdapter(command)
        Dim table As New DataTable


        ' Dim item1 As New Telerik.Web.UI.RadComboBoxItem()

        Try

            connection.Open()
            command.Parameters.AddWithValue("@OrganizationID", OrganizationID)
            adapter.Fill(table)

            Dim HTMLString As String = CreatePiefromTable(table, "Issues+by+Product")


            connection.Close()

            Return HTMLString


        Catch ex As Exception
            Return ""
        End Try
    End Function

    Protected Sub RadDatePicker2_SelectedDateChanged(ByVal sender As Object, ByVal e As Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs) Handles dp_enddate.SelectedDateChanged

    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        'Should create the chart
        TicketsByCustomer.Attributes.Add("src", DisplayTicketsByCustomer(cb_CustomerList.SelectedValue))
    End Sub
End Class
