using System;
using System.Collections;
using System.Collections.Specialized;
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
  }

  [WebMethod]
  public static string ChangePW(string password, string confirm, string token)
  {
    LoginUser loginUser = LoginUser.Anonymous;
    string invalid = "<li>We could not validate this request.  Please contact TeamSupport.</li>";
    if (!string.IsNullOrWhiteSpace(token))
    {
      Organizations organizations = new Organizations(loginUser);
      organizations.LoadBySignUpToken(token);
      if (organizations.Count < 1) return invalid;
      Organization organization = organizations[0];

      Users users = new Users(loginUser);
      users.LoadByOrganizationID(organizations[0].OrganizationID, true);

      User user = null;

      foreach (User item in users)
      {
        if (item.CryptedPassword == "UNVALIDATED" || FormsAuthentication.HashPasswordForStoringInConfigFile("", "MD5") == item.CryptedPassword)
        {
          user = item;
          break;
        }
      }
      
      if (user == null) return invalid;


      bool result = false;
      StringBuilder builder = new StringBuilder("");
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

      if (!result)
      {
        organization.IsValidated = true;
        organization.Collection.Save();
        user.CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
        user.IsPasswordExpired = false;
                user.PasswordCreatedUtc = DateTime.UtcNow;
        users.Save();
        //EmailPosts.SendChangedTSPassword(loginUser, user.UserID);
        return "";
      }
      else
      {
        return builder.ToString();
      }

    
    }
    else
    {
      return invalid;
    }


  }
  

}
