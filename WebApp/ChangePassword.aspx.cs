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
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Web.Services;
using System.Collections.Generic;
using System.Text;

public partial class ChangePassword : System.Web.UI.Page
{

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    fieldSessionID.Value = TSAuthentication.SessionID;
  }

  [WebMethod]
  public static string ChangePW(string password, string confirm, string sessionID)
  {
    if (TSAuthentication.SessionID != sessionID) return "Unable to authenticate your session.  Please refresh the page and try again.";

    bool result = false;
    StringBuilder builder = new StringBuilder("<ul>");
    if (password.Trim() != confirm.Trim())
    {
      builder.Append("<li>Your passwords do not match.</li>");
      result = true;
    }

    if (password.Trim().Length < 6)
    {
      builder.Append("<li>Please choose a password that is at least 6 characters long.</li>");
      result = true;
    }

    builder.Append("</ul");

    if (!result)
    {
      
      Users users = new Users(TSAuthentication.GetLoginUser());
      users.LoadByUserID(TSAuthentication.UserID);

      if (!users.IsEmpty)
      {
        users[0].CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
        users[0].IsPasswordExpired = false;
                users[0].PasswordCreatedUtc = DateTime.UtcNow;
        users.Save();
        EmailPosts.SendChangedTSPassword(users.LoginUser, users[0].UserID);
      }
      return "";
    }
    else
    {
      return builder.ToString();
    }
  }
  

}
