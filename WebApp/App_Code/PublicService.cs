using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Runtime.Serialization;
using System.Globalization;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class PublicService : System.Web.Services.WebService
    {

       public PublicService() { }

      /*
       [WebMethod]
       public bool IsCompanyValid(string company)
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
       public int SignMeUp(string firstName, string lastName, string email, string company, string phone, int version, string password, string promo, string interest, string seats, string process)
       {

         if (!IsCompanyValid(company))
         {
           return -1;
         }


         User user = Organizations.SetupNewAccount(firstName, lastName, email, company, phone, (ProductType)version, password, promo, interest, seats, process);
         return user.UserID;
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

      */
    }
}