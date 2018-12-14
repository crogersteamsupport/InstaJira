Imports TeamSupport.WebUtils

Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Label1.Text = "<div>User ID: " + UserSession.LoginUser.UserID.ToString() + "</div>"
        ' Label1.Text = Label1.Text + "<div>Organization ID: " + UserSession.LoginUser.OrganizationID.ToString() + "</div>"


        'To get more info about the logged in user use
        'Label1.Text = Label1.Text + "<div>Organization Name: " + UserSession.CurrentUser.OrganizationName + "</div>"
        'Label1.Text = Label1.Text + "<div>Full Name: " + UserSession.CurrentUser.DisplayName + "</div>"
        'Label1.Text = Label1.Text + "<div>Is Admin: " + UserSession.CurrentUser.IsSystemAdmin.ToString() + "</div>"
    End Sub

    Protected Sub EndDate_SelectedDateChanged(ByVal sender As Object, ByVal e As Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs) Handles EndDate.SelectedDateChanged
  



    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        TicketsCreatedPerDay.SelectParameters("StartDate").DefaultValue = StartDate.SelectedDate
        TicketsCreatedPerDay.SelectParameters("EndDate").DefaultValue = EndDate.SelectedDate
        TicketsCreatedPerDay.SelectParameters("OrganizationID").DefaultValue = UserSession.LoginUser.OrganizationID.ToString()

        TicketsClosedByDay.SelectParameters("StartDate").DefaultValue = StartDate.SelectedDate
        TicketsClosedByDay.SelectParameters("EndDate").DefaultValue = EndDate.SelectedDate
        TicketsClosedByDay.SelectParameters("OrganizationID").DefaultValue = UserSession.LoginUser.OrganizationID.ToString()


        'TicketsCreatedPerDay.SelectParameters("OrganizationID").DefaultValue = 1078
        
        TicketsCreatedPerDay.DataBind()
        RadChart1.DataBind()

        TicketsClosedByDay.DataBind()
        RadChart2.DataBind()


    End Sub

    Protected Sub TicketsCreatedPerDay_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.SqlDataSourceSelectingEventArgs) Handles TicketsCreatedPerDay.Selecting

    End Sub
End Class
