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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Security;
using OfficeOpenXml;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using DDay.iCal.Serialization;
using DDay.iCal;
using DDay.Collections;
using System.Drawing.Text;
using System.Collections.Specialized;
using System.Dynamic;
using PusherServer;


namespace TeamSupport.Handlers
{
    public class WikiHandler : IHttpHandler
    {

        public bool IsReusable
        {
            get { return false; }
        }


        public void ProcessRequest(HttpContext context)
        {
            try
            {            //https://host/wiki/wikidocs/{path}
                         //https://app.teamsupport.com/Wiki/WikiDocs/1078/images/Misc%20Graphics/BlueBadge.png


                StringBuilder builder = new StringBuilder();
                bool flag = false;
                foreach (string item in context.Request.Url.Segments)
                {
                    string segment = item.ToLower().TrimEnd('/');
                    if (!flag)
                    {
                        if (segment == "wiki") flag = true;
                    }
                    else
                    {
                        builder.Append(segment);
                        builder.Append("\\");
                    }
                }

                string path = HttpUtility.UrlDecode(builder.ToString().TrimEnd('\\'));
                string root = SystemSettings.ReadString("FilePath", "");
                string fileName = Path.Combine(root, path);
                FileInfo info = new FileInfo(fileName);
                context.Response.ContentType = DataUtils.MimeTypeFromFileName(fileName);
                context.Response.AddHeader("Content-Length", info.Length.ToString());
                context.Response.WriteFile(fileName);


            }
            catch (Exception ex)
            {
                context.Response.ContentType = "text/html";
                context.Response.Write(ex.Message + "<br />" + ex.StackTrace);
            }
            context.Response.End();
        }
    }
}