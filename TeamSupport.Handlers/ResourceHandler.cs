﻿using System;
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
using ICSharpCode.SharpZipLib.Zip;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Web.Security;

namespace TeamSupport.Handlers
{
  public class ResourceHandler : IHttpHandler, IRequiresSessionState
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
          if (s == "resources")
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
