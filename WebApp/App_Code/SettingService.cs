﻿using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace TSWebServices
{
  [ScriptService]
  [WebService(Namespace = "http://teamsupport.com/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class SettingService : System.Web.Services.WebService
  {

    public SettingService()
    {

      //Uncomment the following line if using designed components 
      //InitializeComponent(); 
    }

    [WebMethod]
    public string ReadUserSetting(string key, string defaultValue)
    {
      return Settings.UserDB.ReadString(key, defaultValue);
    }

    [WebMethod]
    public void WriteUserSetting(string key, string value)
    {
      Settings.UserDB.WriteString(key, value);
    }

    [WebMethod]
    public string ReadSessionSetting(string key, string defaultValue)
    {
      return Settings.Session.ReadString(key, defaultValue);
    }

    [WebMethod]
    public void WriteSessionSetting(string key, string value)
    {
      Settings.Session.WriteString(key, value);
    }

    [WebMethod]
    public string ReadOrganizationSetting(string key, string defaultValue)
    {
      return Settings.OrganizationDB.ReadString(key, defaultValue);
    }

    [WebMethod]
    public void WriteOrganizationSetting(string key, string value)
    {
      Settings.OrganizationDB.WriteString(key, value);
    }

    [WebMethod]
    public string ReadSystemSetting(string key, string defaultValue)
    {
      return Settings.SystemDB.ReadString(key, defaultValue);
    }

    [WebMethod]
    public void WriteSystemSetting(string key, string value)
    {
      Settings.SystemDB.WriteString(key, value);
    }

    [WebMethod(EnableSession = true)]
    public void SetMoxieManagerSessionVariables()
    {
      if (Session["isLoggedIn"] == null)
      {
        Session["isLoggedIn"] = "true";
        Session["user"] = UserSession.LoginUser.UserID.ToString();
        string root = SystemSettings.ReadString(UserSession.LoginUser, "FilePath", "C:\\TSData");
        root = Path.Combine(root, "WikiDocs\\" + UserSession.LoginUser.OrganizationID);


        Directory.CreateDirectory(Path.Combine(root, "images"));
        Directory.CreateDirectory(Path.Combine(root, "documents"));

        Session["moxiemanager.filesystem.rootpath"] = root;
        Session["moxiemanager.filesystem.local.wwwroot"] = root;
        Session["moxiemanager.filesystem.local.urlprefix"] = "{proto}://{host}/Wiki/WikiDocs/" + UserSession.LoginUser.OrganizationID;
      }
    }


  }
}