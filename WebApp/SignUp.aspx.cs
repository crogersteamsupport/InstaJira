using System;
using System.Collections.Generic;
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
using System.Security.Cryptography;
using Telerik.Web.UI;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Web.Services;

public partial class SignUp : System.Web.UI.Page
{

    protected override void OnLoad(EventArgs e)
    {
        Response.Redirect("https://www.teamsupport.com/customer-support-software-free-trial");
        base.OnLoad(e); 
    }
    [WebMethod]
  public static bool IsCompanyValid(string company)
  {
    Organizations organizations = new Organizations(LoginUser.Anonymous);
    organizations.LoadByOrganizationName(company.Trim());
    if (!organizations.IsEmpty)
    {
      return false;
    }
    return true;
  }

  [WebMethod]
  public static int SignMeUp(string name, string email, string company, string phone, int version, string password, string promo)
  {

        return 1;
    if (!IsCompanyValid(company))
    {
      return -1;
    }
    
    

    string[] names = name.Split(' ');
    string fname = names[0];
    string lname = string.Join(" ", names.Skip(1).ToArray());

   // User user = Organizations.SetupNewAccount(fname, lname, email, company, phone, "", "", (ProductType)version, null);
   // return user.UserID;
  }

  private static void AddToMuroc(Organization tsOrg, User tsUser, string phoneNumber)
  {
    Organization mOrg = (new Organizations(tsOrg.Collection.LoginUser)).AddNewOrganization();
    mOrg.ParentID = 1078;
    mOrg.Name = tsOrg.Name;
    mOrg.ImportID = tsOrg.OrganizationID.ToString();
    mOrg.HasPortalAccess = true;
    mOrg.IsActive = true;
    mOrg.Collection.Save();

    User mUser = (new Users(tsOrg.Collection.LoginUser)).AddNewUser();
    mUser.OrganizationID = mOrg.OrganizationID;
    mUser.FirstName = tsUser.FirstName;
    mUser.LastName = tsUser.LastName;
    mUser.Email = tsUser.Email;
    mUser.IsActive = true;
    mUser.IsPortalUser = true;
    mUser.ImportID = tsUser.UserID.ToString();
    mUser.Collection.Save();

    mOrg.PrimaryUserID = mUser.UserID;
    mOrg.Collection.Save();

    PhoneNumber phone = (new PhoneNumbers(tsOrg.Collection.LoginUser)).AddNewPhoneNumber();
    phone.RefID = mOrg.OrganizationID;
    phone.RefType = ReferenceType.Organizations;
    phone.Number = phoneNumber;
    phone.Collection.Save();

    AddMurocProduct(tsOrg.Collection.LoginUser, mOrg.OrganizationID, 219); //TeamSupport
    AddMurocProduct(tsOrg.Collection.LoginUser, mOrg.OrganizationID, 233); //Email Handler
    AddMurocProduct(tsOrg.Collection.LoginUser, mOrg.OrganizationID, 234); //Adv Portal
    AddMurocProduct(tsOrg.Collection.LoginUser, mOrg.OrganizationID, 1068); //Basic Portal
    AddMurocProduct(tsOrg.Collection.LoginUser, mOrg.OrganizationID, 1970); //Chat
    AddMurocProduct(tsOrg.Collection.LoginUser, mOrg.OrganizationID, 2580); //KB
    AddMurocProduct(tsOrg.Collection.LoginUser, mOrg.OrganizationID, 1877); //API
  }

  private static void AddMurocProduct(LoginUser loginUser, int organizationID, int productID)
  {
    OrganizationProducts ops = new OrganizationProducts(loginUser);

    try
    {
      OrganizationProduct op = ops.AddNewOrganizationProduct();
      op.OrganizationID = organizationID;
      op.ProductID = productID;
      op.ProductVersionID = null;
      op.IsVisibleOnPortal = true;
      ops.Save();
    }
    catch (Exception)
    {
    }
  }

}
