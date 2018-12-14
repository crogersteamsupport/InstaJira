using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TeamSupport.Data;

/// <summary>
/// Class dedicated to calling and bundling the payload for the Deflector API
/// </summary>
namespace TeamSupport.Data
{
    public class DeflectorAPI
    {
        private string BaseURL;
        public DeflectorAPI()
        {
            BaseURL = ConfigurationManager.AppSettings["DeflectorBaseURL"] ?? String.Empty;
        }

        public string FetchDeflections(int organization, string phrase)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + "/fetch/" + organization + "/" + phrase);
            request.Method = "GET";
            request.KeepAlive = false;
            request.ContentType = "application/json";

            return SendAPIRequest(request);
        }

        public async Task<string> IndexDeflectorAsync(string deflectionIndex)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + "/index/index");
            request.Method = "POST";
            request.KeepAlive = false;
            request.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(deflectionIndex);
                streamWriter.Flush();
                streamWriter.Close();
            }

            return await SendAPIAsyncRequest(request);
        }

        public async Task<string> BulkIndexDeflectorAsync(string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + "/index/bulkindex");
            request.Method = "POST";
            request.Timeout = 800000;
            request.KeepAlive = false;
            request.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            return await SendAPIAsyncRequest(request);
        }

        public async Task<string> DeleteTicketAsync(int ticketID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + "/delete/ticket/" + ticketID);
            request.Method = "DELETE";
            request.KeepAlive = false;
            request.ContentType = "application/json";

            return await SendAPIAsyncRequest(request);
        }

        public async Task<string> DeleteTagAsync(int organizationID, string value)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + "/delete/organization/" + organizationID + "/tag/" + value);
            request.Method = "DELETE";
            request.KeepAlive = false;
            request.ContentType = "application/json";

            return await SendAPIAsyncRequest(request);
        }

        public async Task<string> RemoveTagAsync(int organizationID, int ticketID, string value)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + "/delete/organization/" + organizationID + "/ticketid/" + ticketID + "/tag/" + value);
            request.Method = "DELETE";
            request.KeepAlive = false;
            request.ContentType = "application/json";

            return await SendAPIAsyncRequest(request);
        }

        public async Task<string> RenameTagAsync(int organizationID, string oldTag, string newTag) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + "/update/organization/" + organizationID + "/tag/rename");
            request.Method = "POST";
            request.KeepAlive = false;
            request.ContentType = "application/json";
            string Output = "{\"OldTag\":\"" + oldTag + "\",\"NewTag\":\"" + newTag + "\"}";
            using (var streamWriter = new StreamWriter(request.GetRequestStream())) {
                streamWriter.Write(Output);
                streamWriter.Flush();
                streamWriter.Close();
            }
            return await SendAPIAsyncRequest(request);
        }

        public async Task<string> TestDeflectorAPIAsync(string tag)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + "/deflector/check");
            request.Method = "GET";
            request.KeepAlive = false;
            request.ContentType = "application/json";

            return await SendAPIAsyncRequest(request);
        }

        private async Task<string> SendAPIAsyncRequest(HttpWebRequest request)
        {
            string ResponseText = "";

            try
            {
                using (WebResponse response = await request.GetResponseAsync().ConfigureAwait(false))
                {
                    if (request.HaveResponse && response != null)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8))
                        {
                            ResponseText = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
            }

            return ResponseText;
        }

        private string SendAPIRequest(HttpWebRequest request)
        {
            string ResponseText = "";

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    if (request.HaveResponse && response != null)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8))
                        {
                            ResponseText = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
            }

            return ResponseText;
        }


        //private async Task<string> RenameTag(string jsonUpdate) {
        //    string ResponseText    = null;
        //    string PingUrl         = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/update/organization/" + OrganizationId + "/tag/" + TagId + "/rename";

        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
        //    request.Method         = "POST";
        //    request.KeepAlive      = false;
        //    request.ContentType    = "application/json";

        //    using (var streamWriter = new StreamWriter(request.GetRequestStream())) {
        //        streamWriter.Write(jsonUpdate);
        //        streamWriter.Flush();
        //        streamWriter.Close();
        //    }

        //    using (WebResponse response = request.GetResponse()) {
        //        if (request.HaveResponse && response != null) {
        //            using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
        //                ResponseText = reader.ReadToEnd();
        //            }
        //        }
        //    }
        //    return ResponseText;
        //}

        //private async Task<string> MergeTag (int OrganizationId, int TagId, string Value) {
        //    var item = new DeflectorItem {
        //        OrganizationID = OrganizationId,
        //        TagID = TagId,
        //        Value = Value
        //    };
        //    string json = Newtonsoft.Json.JsonConvert.SerializeObject(item);
        //    string ResponseText    = null;
        //    string PingUrl         = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/update/organization/" + OrganizationId + "/tag/" + TagId + "/merge";

        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
        //    request.Method         = "POST";
        //    request.KeepAlive      = false;
        //    request.ContentType    = "application/json";

        //    using (var streamWriter = new StreamWriter(request.GetRequestStream())) {
        //        streamWriter.Write(json);
        //        streamWriter.Flush();
        //        streamWriter.Close();
        //    }

        //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
        //        if (request.HaveResponse && response != null) {
        //            using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
        //                ResponseText = reader.ReadToEnd();
        //            }
        //        }
        //    }
        //    return ResponseText;
        //}
    }
}
