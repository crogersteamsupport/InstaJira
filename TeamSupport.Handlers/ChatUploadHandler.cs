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
    class ChatUploadHandler : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable
        {
            get { return false; }
        }

        private string _result = "";

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            try
            {
                context.Response.Clear();
                context.Response.ContentType = "text/plain";
                List<UploadResult> result = new List<UploadResult>();
                List<AttachmentProxy> proxies = ModelAPI.AttachmentAPI.CreateAttachments(context, out _result);
                foreach (AttachmentProxy attachment in proxies)
                    result.Add(new UploadResult(attachment.FileName, attachment.FileType, attachment.FileSize, attachment.AttachmentID));
                context.Response.ContentType = "text/html";
                    context.Response.Write(DataUtils.ObjectToJson(result.ToArray()));
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.StackTrace);
                context.Response.Write(ex.Message);
            }

            context.Response.End();
        }
    }


}
