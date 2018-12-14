using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using TeamSupport.Data;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Net;
using System.Web.SessionState;
using System.Web.Security;
using System.Security.Principal;


namespace TeamSupport.Api
{
  class RestCommandModule : IHttpModule
  {

    public void Dispose()
    {
    }

    public void Init(HttpApplication app)
    {
        app.AuthenticateRequest += new EventHandler(context_AuthenticateRequest);
        app.EndRequest += new EventHandler(context_EndRequest);
    }

    private bool IsApi(HttpContext context)
    {

       return context.Request.FilePath.ToLower().Trim().IndexOf("/api/") > -1;
    }

    void context_EndRequest(object sender, EventArgs e)
    {
      HttpApplication app = (HttpApplication)sender;

      if (!IsApi(app.Context)) return;

      if (app.Context.Response.StatusCode == (int)HttpStatusCode.Redirect || app.Context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
      {
        string realm = String.Format("Basic Realm=\"{0}\"", "TeamSupport-API");
        app.Response.AppendHeader("WWW-Authenticate", realm);
        app.Response.Write("Unauthorized");
        app.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
      }
    }

    void context_AuthenticateRequest(object sender, EventArgs e)
    {
      HttpApplication app = (HttpApplication)sender;
      if (!IsApi(app.Context)) return;

      try
      {
        string authorization = app.Request.Headers["Authorization"];
        if ((authorization == null) || (authorization.Length == 0))
        {
          AccessDenied(app);
          return;
        }

        authorization = authorization.Trim();

        string userInfo = authorization;
        if (authorization.IndexOf("Basic", 0) == 0)
        {
          byte[] tempConverted = Convert.FromBase64String(authorization.Substring(6));
          userInfo = new ASCIIEncoding().GetString(tempConverted);
        }

        string[] usernamePassword = userInfo.Split(new char[] { ':' });

        string username = usernamePassword[0];
        string password = usernamePassword[1];
                 

        if (Validated(app, username, password))
        {
          app.Context.User = new GenericPrincipal(new GenericIdentity(username, "TeamSuport.Api.RestCommandModule"), null);
        }
        else
        {
          AccessDenied(app);
          return;
        }


      }
      catch (Exception)
      {
        AccessDenied(app);
        return;
      }
    }

    private LoginUser GetLoginUser()
    {
      string connection = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString;
      return new LoginUser(connection, -3, -1, null);
    }
    
    private void AccessDenied(HttpApplication app)
    {
      app.Response.StatusCode = 401;
      app.Response.StatusDescription = "Access Denied";
      app.Response.Write("401 Access Denied");
      app.CompleteRequest();
    }

    private bool Validated(HttpApplication app, string username, string password)
    {

      try
      {
        if (string.IsNullOrEmpty(password)) return false;
        LoginUser loginUser = GetLoginUser();

        

        Organization organization = Organizations.GetOrganization(loginUser, int.Parse(username));
        Guid guid = new Guid(password);

        if (organization == null || 
            !organization.IsActive || 
            !organization.IsApiActive || 
            !organization.IsApiEnabled ||
            guid != organization.WebServiceID
          ) return false;

        if (organization.ParentID > 1)
        {
          Organization parent = Organizations.GetOrganization(organization.Collection.LoginUser, (int)organization.ParentID);
          if (parent == null ||
              !parent.IsActive ||
              !parent.IsApiActive
          ) return false;
        }

        app.Context.Request.Headers["OrganizationID"] = organization.OrganizationID.ToString();
      }
      catch (Exception)
      {
        return false;
      }
      return true;
    
    }

  }
}
