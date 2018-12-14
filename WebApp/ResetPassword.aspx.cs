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

public partial class ResetPassword : System.Web.UI.Page
{

  [Serializable]
  public class ComboBoxItem
  {
    public ComboBoxItem(string label, int id)
    {
      Label = label;
      ID = id;
    }
    public string Label { get; set; }
    public int ID { get; set; }
  }

  [Serializable]
  public class CompanyResult
  {
    public ComboBoxItem[] Items { get; set; }
    public int SelectedID { get; set; }
  }

  [WebMethod]
  public static CompanyResult GetCompanies(string email)
  {
    Organizations organizations = new Organizations(LoginUser.Anonymous);
    organizations.LoadByEmail(email);
    List<ComboBoxItem> items = new List<ComboBoxItem>();
    foreach (Organization organization in organizations)
    {
      items.Add(new ComboBoxItem(organization.Name, organization.OrganizationID));
    }


    CompanyResult result = new CompanyResult();
    result.Items = items.ToArray();

    result.SelectedID = int.Parse(SystemSettings.ReadString(LoginUser.Anonymous, "LastCompany-" + email, "-1"));

    return result;
  }

  [WebMethod]
  public static string[] ResetPW(string email, int organizationID)
  {
    string[] result = new string[2];
    result[0] = null;
    result[1] = "Message.aspx?ReturnUrl=Login.html&Message=password_reset";
    email = email.Trim();
    LoginUser ananLoginUser = new LoginUser(UserSession.ConnectionString, -1, -1, null);

    Users users = new Users(ananLoginUser);
    users.LoadByEmail(email);

    if (users.Count == 1)
    {
      DataUtils.ResetPassword(ananLoginUser, users[0], false);
      return result;
    }

    foreach (User user in users)
    {
      if (user.OrganizationID == organizationID)
      {
        DataUtils.ResetPassword(ananLoginUser, user, false);
        return result;
      }
    }

    result[0] = "Invalid email address, please try again.";
    return result;
  }
}
