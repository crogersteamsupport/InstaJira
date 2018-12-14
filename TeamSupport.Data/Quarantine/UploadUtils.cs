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
//using TeamSupport.WebUtils;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace TeamSupport.Handlers
{
    public class UploadUtils
    {
        public static List<string> GetUrlSegments(HttpContext context, string uploadFlag = "upload")
        {
            List<string> segments = new List<string>();
            bool flag = false;
            for (int i = 0; i < context.Request.Url.Segments.Length; i++)
            {
                string s = context.Request.Url.Segments[i].ToLower().Trim().Replace("/", "");
                if (flag) segments.Add(s);
                if (s == uploadFlag) flag = true;
            }
            return segments;
        }

        public static string RemoveSpecialCharacters(string text)
        {
            return Path.GetInvalidFileNameChars().Aggregate(text, (current, c) => current.Replace(c.ToString(), "_"));
            //StringBuilder builder = new StringBuilder();
            //foreach (char c in text)
            //  {
            //  if (!char.IsLetterOrDigit(c) && c != '.' && c != '@') builder.Append("_");
            //  else builder.Append(c);
            //  }
            //return builder.ToString();

        }


    }

    [DataContract]
    public class UploadResult
    {
        public UploadResult(string name, string type, long size)
        {
            this.name = name;
            this.size = size;
            this.type = type;
            this.id = 0;
        }

        public UploadResult(string name, string type, long size, int id)
        {
            this.name = name;
            this.size = size;
            this.type = type;
            this.id = id;
        }

        [DataMember]
        public string name { get; set; }
        [DataMember]
        public long size { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public int id { get; set; }
    }
}
