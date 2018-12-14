using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Net;
using System.Web.SessionState;
using System.Drawing;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Security;

namespace TeamSupport.Handlers
{
  public class UserStatusHandler : IHttpHandler
  {
    public bool IsReusable
    {
      get { return false; }
    }

    private string GetVersion(HttpContext context)
    {
      try
      {
        using (System.IO.StreamReader sr = new System.IO.StreamReader(context.Server.MapPath("~/version.txt")))
        {
          String line = sr.ReadToEnd();
          return line;
        }
      }
      catch (Exception e)
      {
        return "0";
      }
    }
    
    public void ProcessRequest(HttpContext context)
    {
      context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      context.Response.AddHeader("Expires", "-1");
      context.Response.AddHeader("Pragma", "no-cache");

      if (TSAuthentication.Ticket == null)
      {
        context.Response.ContentType = "text/plain";
        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        context.Response.ClearContent();
        context.Response.End();
        return;
      }

      string sessionID = context.Request.Url.Segments[context.Request.Url.Segments.Length - 1];
     
      TsMainPageUpdate update = new TsMainPageUpdate();

      try
      {
        update.IsDebug = TSAuthentication.OrganizationID == 1078 || TSAuthentication.IsBackdoor;
        update.IsExpired = false;

        using (SqlConnection connection = new SqlConnection(LoginUser.Anonymous.ConnectionString))
        {
          connection.Open();
          SqlCommand command = new SqlCommand();
          command.Connection = connection;
          command.CommandText = "SELECT EnforceSingleSession, SessionID, IsActive, MarkDeleted FROM Users WHERE UserID = @UserID";
          command.Parameters.AddWithValue("UserID", TSAuthentication.UserID);
          SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow);
          if (reader.Read())
          {
            if (!(bool)reader[2] || (bool)reader[3])
            {
              update.IsExpired = true;
            }
            else if ((bool)reader[0] && reader[1] != DBNull.Value)
            {
              if (sessionID != null && sessionID.ToLower() != reader[1].ToString().ToLower() && !TSAuthentication.IsBackdoor)
              {
                update.IsExpired = true;
              }
            }

          }
          reader.Close();
        }

        update.RefreshID = int.Parse(SystemSettings.ReadString(LoginUser.Anonymous, "RefreshID", "-1"));
        update.ExpireTime = TSAuthentication.Ticket.Expiration.ToShortTimeString();
        update.Version = GetVersion(context);
        update.MyUnreadTicketCount = Tickets.GetMyOpenUnreadTicketCount(TSAuthentication.GetLoginUser(), TSAuthentication.UserID);
      }
      catch (Exception ex)
      {
        ex.Data["SessionID"] = sessionID;
        ExceptionLogs.LogException(LoginUser.Anonymous, ex, "GetUserStatusUpdate");
      }


      context.Response.ContentType = "application/json; charset=utf-8";
      context.Response.Write(JsonConvert.SerializeObject(update));
    }
  }

  [DataContract]
  public class TsMainPageUpdate
  {
    [DataMember]
    public bool IsExpired { get; set; }
    [DataMember]
    public string ExpireTime { get; set; }
    [DataMember]
    public string Version { get; set; }
    [DataMember]
    public int RefreshID { get; set; }
    [DataMember]
    public bool IsIdle { get; set; }
    [DataMember]
    public bool IsDebug { get; set; }
    [DataMember]
    public int MyUnreadTicketCount { get; set; }
  }
}
