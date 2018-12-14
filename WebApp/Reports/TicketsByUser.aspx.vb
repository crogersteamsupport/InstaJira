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

        TicketsOpenByUser.SelectParameters("OrganizationID").DefaultValue = UserSession.LoginUser.OrganizationID.ToString()
        'TicketsOpenByUser.SelectParameters("OrganizationID").DefaultValue = 1078
        TicketsOpenByUser.DataBind()
        'RadChart1.PlotArea.Appearance.Position = -200




        '.Dimensions.Margins.Right = 

    End Sub

    
    Protected Sub TicketsCreatedPerDay_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.SqlDataSourceSelectingEventArgs) Handles TicketsOpenByUser.Selecting

    End Sub
End Class
