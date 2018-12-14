using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
    public class HubSpotBaseClass : IBaseClass
    {
        private readonly string _accessToken;
        private readonly string _apiKey;
        private readonly Dictionary<string, object> _options;
        private readonly string _refreshToken;
        protected SyncLog _logs;
        protected SyncLog Logs
        {
          get { return _logs; }
        }

        public HubSpotBaseClass(string apiKey = null, string accessToken = null, string refreshToken = null, string clientId = null, string logPath = null)
        {
            if (apiKey != null) _apiKey = apiKey;
            if (accessToken != null) _accessToken = accessToken;
            if (refreshToken != null) _refreshToken = refreshToken;

            if (_apiKey != null && _accessToken != null)
            {
                throw new ArgumentException("Cannot use both api_key and access_token");
            }

            if ((_apiKey == null) && (_accessToken == null) && (_refreshToken == null))
            {
                throw new ArgumentException("Missing required credentials");
            }

			_options = new Dictionary<string, object>();
            _options["api_base"] = "api.hubapi.com";
            UserWebClient = new WindowsWebClient();
            _logs = new SyncLog(logPath, IntegrationType.HubSpot);
        }

        public IWebClient UserWebClient { get; set; }

        protected virtual string GetPath(string subPath)
        {
            throw new NotImplementedException();
        }

        protected JObject Call(string subpath, string method = "GET", string query = "", string contentType = "application/text",
                                  string data = "", Dictionary<string, string> optionalParams = null, string other = "", bool log = true)
        {
            string uri;
            JObject jObject = new JObject();

            try
            {
              if (_accessToken != null)
              {
                uri = String.Format("https://{0}/{1}?access_token={2}", _options["api_base"],
                                    GetPath(subpath), _accessToken);
              }
              else
              {
                uri = String.Format("https://{0}/{1}?hapikey={2}", _options["api_base"],
                                    GetPath(subpath), _apiKey);
              }

              if (query.Length > 0)
              {
                uri = string.Format("{0}&q={1}", uri, query);
              }

              if (other.Length > 0)
              {
                uri = string.Format("{0}&{1}", uri, other);
              }

              if (optionalParams != null)
              {
                uri = optionalParams.Aggregate(uri,
                                               (current, optionalParam) =>
                                               string.Format("{0}&{1}={2}", current, optionalParam.Key,
                                                             optionalParam.Value));
              }

              if (log)
                Logs.Write(string.Format("{0}: ContentType {1}. uri: {2}", method, contentType, uri));

              if (log && !string.IsNullOrEmpty(data))
              {
                Logs.Write(string.Format("Data:{0}{1}", Environment.NewLine, data));
              }

              var returnVal = UserWebClient.UploadString(uri, method: method, contentType: contentType, data: data);

              if (returnVal != null)
              {
                if (returnVal.Length > 0)
                {
                  jObject = JObject.Parse(returnVal);
                }
              }
            }
            catch (Exception ex)
            {
              Logs.Write(string.Format("Exception in Call: {0}", ex.Message));
            }

            return jObject;
        }
    }
}