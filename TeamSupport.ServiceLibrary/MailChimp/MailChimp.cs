using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.ServiceLibrary
{
    public class MailChimp
    {
        public static string MakeHttpWebRequestGet(string requestUrl)
        {
            string responseText = string.Empty;
            HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
            request.ContentType = "application/json";

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.ASCII))
                    {
                        responseText = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return responseText;
        }

        public static HttpWebResponse MakeHTTPRequestPost(string encodedCredentials,
                                            string URI,
                                            string method,
                                            string contentType,
                                            string body)
        {
            HttpWebRequest request = WebRequest.Create(URI) as HttpWebRequest;
            request.Headers.Add("Authorization", "Basic " + encodedCredentials);
            request.Method = method;
            request.ContentType = contentType;
            byte[] bodyByteArray = UTF8Encoding.UTF8.GetBytes(body);
            request.ContentLength = bodyByteArray.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bodyByteArray, 0, bodyByteArray.Length);
                requestStream.Close();
            }

            return request.GetResponse() as HttpWebResponse;
        }
    }
}
