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
using System.Web.Security;

namespace TeamSupport.Handlers
{
  public class ResourceHandler : IHttpHandler
  {
    #region IHttpHandler Members

    public bool IsReusable
    {
      get { return false; }
    }

    public void ProcessRequest(HttpContext context)
    {
      try
      {
        bool flag = false;
        StringBuilder builder = new StringBuilder(context.Server.MapPath("~"));
        for (int i = 0; i < context.Request.Url.Segments.Length; i++)
        {
          string s = context.Request.Url.Segments[i].ToLower().Trim().Replace("/", "");
          if (s == "vcr")
          {

            flag = true;
            i++;
            builder.Append("\\resources");
            continue;

          }
          if (flag)
          {
            builder.Append("\\" + s);
          }

        }

        string fileName = builder.ToString();
        context.Response.ContentType = DataUtils.MimeTypeFromFileName(fileName);
        context.Response.Cache.SetCacheability(HttpCacheability.Public);
        context.Response.Cache.SetExpires(DateTime.Now.AddHours(8));
        context.Response.Cache.SetMaxAge(new TimeSpan(8, 0, 0));
        DateTime lastWriteDate = File.GetLastWriteTimeUtc(fileName);
        context.Response.Headers["Last-Modified"] = lastWriteDate.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'");
        try
        {
          if (context.Request.Headers["If-Modified-Since"] != null && lastWriteDate.Subtract(DateTime.Parse(context.Request.Headers["If-Modified-Since"]).ToUniversalTime()).TotalSeconds < 5)
          {
            context.Response.StatusCode = 304;
            context.Response.SuppressContent = true;
            context.Response.End();
            return;
          }
        }
        catch (Exception)
        {

        }


        context.Response.WriteFile(fileName);
      }
      catch (Exception ex)
      {
        context.Response.ContentType = "text/html";
        context.Response.Write(ex.Message + "<br />" + ex.StackTrace);
      }
      context.Response.End();
    }

    #endregion

  }
}
