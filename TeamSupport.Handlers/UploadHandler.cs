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
using System.IO;
using TeamSupport.WebUtils;
using System.Runtime.Serialization;

namespace TeamSupport.Handlers
{
    class UploadHandler : IHttpHandler, IRequiresSessionState
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return false; }
        }

        private int? _id = null;
        private string _ratingImage = "";

        public void ProcessRequest(HttpContext context)
        {
            using (UnitTest.ScopedElapsedTime.Trace())
                ProcessRequest1(context);
        }

        public void ProcessRequest1(HttpContext context)
        {
            if (ProcessImportUploadRequest(context))
                return;

            context.Response.ContentType = "text/html";
            try
            {
                context.Response.Clear();
                context.Response.ContentType = "text/plain";
                List<UploadResult> result = new List<UploadResult>();
                List<AttachmentProxy> proxies = ModelAPI.AttachmentAPI.CreateAttachments(context, out _ratingImage);
                foreach (AttachmentProxy attachment in proxies)
                    result.Add(new UploadResult(attachment.FileName, attachment.FileType, attachment.FileSize, attachment.AttachmentID));
                context.Response.ContentType = "text/html";
                context.Response.Write(DataUtils.ObjectToJson(result.ToArray()));

                //if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break();
                //List<string> segments = UploadUtils.GetUrlSegments(context);
                //TeamSupport.Data.Quarantine.UploadHandlerQ.ProcessRequest(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID, context, _id, _ratingImage, segments);
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.Message);
            }

            context.Response.End();
        }

        public static string RemoveSpecialCharacters(string text)
        {
            return Path.GetInvalidFileNameChars().Aggregate(text, (current, c) => current.Replace(c.ToString(), "_"));
        }

        public bool ProcessImportUploadRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            try
            {
                string path = ModelAPI.AttachmentAPI.ImportFileUploadPath(context);
                if (String.IsNullOrEmpty(path))
                    return false;

                HttpFileCollection files = context.Request.Files;
                List<UploadResult> result = new List<UploadResult>();
                for (int i = 0; i < files.Count; i++)
                {
                    if (files[i].ContentLength > 0)
                    {
                        string fileName = RemoveSpecialCharacters(DataUtils.VerifyUniqueUrlFileName(path, Path.GetFileName(files[i].FileName)));
                        files[i].SaveAs(Path.Combine(path, fileName));
                        result.Add(new UploadResult(fileName, files[i].ContentType, files[i].ContentLength));
                    }
                }

                context.Response.Clear();
                context.Response.ContentType = "text/html";
                context.Response.Write(DataUtils.ObjectToJson(result.ToArray()));
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.Message);
            }

            context.Response.End();
            return true;
        }


        #endregion
    }

}
