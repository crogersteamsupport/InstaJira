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

        TicketsByProduct.SelectParameters("OrganizationID").DefaultValue = UserSession.LoginUser.OrganizationID.ToString()
        'TicketsByProduct.SelectParameters("OrganizationID").DefaultValue = 1078
        TicketsByProduct.DataBind()

    End Sub


    Protected Sub TicketsCreatedPerDay_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.SqlDataSourceSelectingEventArgs) Handles TicketsByProduct.Selecting

    End Sub
End Class
