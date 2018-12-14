using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TeamSupport.ServiceLibrary
{
    public class ContactProperty : HubSpotBaseClass
    {

        private const string ContactListsApiVersion = "v1";

        public ContactProperty(string apiKey = null, string accessToken = null, string refreshToken = null, string clientId = null, string logPath = null)
            : base(apiKey, accessToken, refreshToken, clientId, logPath)
        {
        }

        protected override string GetPath(string method)
        {
            return string.Format("contacts/{0}/{1}", ContactListsApiVersion, method);
        }

        public JObject GetAllProperties()
        {
            return Call(subpath: "properties");
        }

        public JObject CreateNewCustomProperty(string property, string data)
        {
            var subpath = string.Format("properties/{0}", property);
            return Call(subpath: subpath, method: "PUT", contentType: " application/json", data: data);
        }

        public JObject UpdateExistingProperty(string property, string data)
        {
            var subpath = string.Format("properties/{0}", property);
            return Call(subpath: subpath, method: "POST", contentType: " application/json", data: data);
        }

        public JObject DeleteProperty(string property)
        {
            var subpath = string.Format("properties/{0}", property);
            return Call(subpath: subpath, method: "DELETE");
        }

        public JObject GetContactPropertyGroup(string groupName = "")
        {
            string subpath = groupName.Length > 0 ? string.Format("groups/{0}", groupName) : "groups";
            return Call(subpath: subpath);
        }

        public JObject CreateContactPropertyGroup(string groupName, string properties = "")
        {
            string subpath = string.Format("groups/{0}", groupName);
            return Call(subpath: subpath, method: "PUT", data: properties, contentType: "application/json");
        }

        public JObject UpdateContactPropertyGroup(string groupName, string properties = "")
        {
            string subpath = string.Format("groups/{0}", groupName);
            return Call(subpath: subpath, method: "POST", data: properties, contentType: "application/json");
        }

        public JObject DeleteContactPropertyGroup(string groupName)
        {
            string subpath = string.Format("groups/{0}", groupName);
            return Call(subpath: subpath, method: "DELETE");
        }
    }
}