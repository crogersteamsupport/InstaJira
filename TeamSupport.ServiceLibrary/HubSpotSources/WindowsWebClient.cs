using System;
using System.IO;
using System.Net;

namespace TeamSupport.ServiceLibrary
{
    internal class WindowsWebClient : IWebClient
    {
        public string UploadString(string uri, string method = "GET", string contentType = "application/text", string data = "")
        {
            string responseString = string.Empty;
            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.Method = method;
            request.ContentType = contentType;

            if (data.Length > 0)
            {
                var writer = new StreamWriter(request.GetRequestStream());
                writer.Write(data);
                writer.Close();
            }

            try
            {
                var response = (HttpWebResponse) request.GetResponse();
                Stream responseStream = response.GetResponseStream();

                if (responseStream != null)
                {
                    var streamReader = new StreamReader(responseStream);
                    responseString = streamReader.ReadToEnd();
                }
            }
            catch (WebException e)
            {
                using (var response = (HttpWebResponse) e.Response)
                {
                    Stream responseStream = response.GetResponseStream();

                    if (responseStream != null)
                    {
                        var reader = new StreamReader(responseStream);
                        string responseText = string.Format("Error code: {0} - {1}{2}{3}", response.StatusCode, response.StatusDescription, Environment.NewLine, reader.ReadToEnd());
                        throw new Exception(responseText);
                    }
                }
            }

            return responseString;
        }
    }
}
