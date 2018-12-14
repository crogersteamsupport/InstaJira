using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Telerik.Web.UI;

public partial class Message : System.Web.UI.Page {
    string _message = "";
    string _returnUrl = "";

    protected void Page_Load(object sender, EventArgs e) {
        if (Request["Message"] != null) _message = Request["Message"];
        _returnUrl = "/";

        pnlContinue.Visible = _returnUrl != "";

        switch (_message) {
            case "thanks_signup":
                lblMessage.Text = "You have successfully signed up.<br/>Thank you for taking the time to try TeamSupport.<br/>Please continue to sign in.";
                break;
            case "password_changed":
                lblMessage.Text = "You have successfully changed your password.";
                break;
            case "password_reset":
                lblMessage.Text = "Your new password was sent to your email account.";
                break;
            case "invalid_request":
                lblMessage.Text = "Your request was invalid.";
                break;
            case "unauthorized":
                lblMessage.Text = "You are not authorized to access this page.";
                break;
            case "account_changed":
                lblMessage.Text = "Your account was successfully updated.";
                break;
            default:
                lblMessage.Text = "An unknown error was encountered, please try again.";
                break;
        }
    }

    protected void btnContinue_Click(object sender, EventArgs e) {
        Response.Redirect(_returnUrl);
    }
}
