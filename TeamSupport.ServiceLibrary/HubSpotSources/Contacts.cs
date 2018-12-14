using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using HubSpotObject = TeamSupport.ServiceLibrary.HubSpotSources.Objects;

namespace TeamSupport.ServiceLibrary
{
    public class Contacts : HubSpotBaseClass
    {
        protected object ProspectsApiVersion = "v1";

        public Contacts(string apiKey = null, string accessToken = null, string refreshToken = null, string clientId = null, string logPath = null)
          : base(apiKey, accessToken, refreshToken, clientId, logPath)
        {
        }

        protected override string GetPath(string method)
        {
            return string.Format("contacts/{0}/{1}", ProspectsApiVersion, method);
        }

        public HubSpotObject.Company.RootObject Create(string contact)
        {
            JObject jsonString = Call(subpath: "contact", method: "POST", contentType: "application/json", data: contact);
            HubSpotObject.Company.RootObject hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.Company.RootObject>(jsonString.ToString());

            return hubspotObject;
        }

        public HubSpotObject.Contact.RootObject Update(string contactId, string data)
        {
            var subpath = string.Format("contact/vid/{0}/profile", contactId);
            JObject jsonString = Call(subpath: subpath, method: "POST", contentType: "application/json", data: data);
            HubSpotObject.Contact.RootObject hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.Contact.RootObject>(jsonString.ToString());

            return hubspotObject;
        }

        public bool Delete(string contactId)
        {
          bool wasDeleted = false;
	        var subpath = string.Format("contact/vid/{0}", contactId);
          JObject jsonDelete = Call(subpath: subpath, method: "DELETE", contentType: "application/json");

          try
          {
            dynamic stuff = JObject.Parse(jsonDelete.ToString());
            string vid = stuff.vid;
            string deleted = stuff.deleted;

            if (!string.IsNullOrEmpty(vid) && vid == contactId
                && !string.IsNullOrEmpty(deleted) && deleted.ToLower() == true.ToString().ToLower())
            {
              wasDeleted = true;
            }
          }
          catch (Exception ex)
          {
            Logs.Write(string.Format("Error deleting the contact {0}. Might or might not be deleted, verify.{1}{2}{1}{2}", contactId, Environment.NewLine, ex.Message, ex.StackTrace));
          }

          return wasDeleted;
        }

        public HubSpotObject.Contacts.RootObject GetAll(string count = "", string property = "", string contactOffset = "")
        {
            var optionalParams = new Dictionary<string, string>();
            if (count.Length > 0)
            {
                optionalParams["count"] = count;
            }
            if (property.Length > 0)
            {
                optionalParams["property"] = property;
            }
            if (count.Length > 0)
            {
                optionalParams["vidOffset"] = contactOffset;
            }

            JObject jsonString = Call(subpath: "lists/all/contacts/all", optionalParams: optionalParams, contentType: "application/json");
            HubSpotObject.Contacts.RootObject hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.Contacts.RootObject>(jsonString.ToString());

            return hubspotObject;
        }

        public HubSpotObject.Contacts.RootObject GetAllRecentlyModified(string count = "", string timeOffset = "", string contactOffset = "")
        {
            var optionalParams = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(count) && count.Length > 0)
            {
                optionalParams["count"] = count;
            }

            if (!string.IsNullOrEmpty(timeOffset) && timeOffset.Length > 0)
            {
                optionalParams["timeOffset"] = timeOffset;
            }

            if (!string.IsNullOrEmpty(contactOffset) && contactOffset.Length > 0)
            {
                optionalParams["vidOffset"] = contactOffset;
            }

            JObject jsonString = Call(subpath: "lists/recently_updated/contacts/recent", optionalParams: optionalParams, contentType: "application/json");
            HubSpotObject.Contacts.RootObject hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.Contacts.RootObject>(jsonString.ToString());

            return hubspotObject;
        }

        public HubSpotObject.Contact.RootObject GetContactById(string contactId)
        {
            var subPath = string.Format("contact/vid/{0}/profile", contactId);
            JObject jsonString = Call(subpath: subPath, contentType: "application/json");
            HubSpotObject.Contact.RootObject hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.Contact.RootObject>(jsonString.ToString());

            return hubspotObject;
        }

        public HubSpotObject.Contact.RootObject GetContactByEmailAddress(string emailAddress)
        {
            var subPath = string.Format("contact/email/{0}/profile", emailAddress);
            JObject jsonString = Call(subpath: subPath, contentType: "application/json");
            HubSpotObject.Contact.RootObject hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.Contact.RootObject>(jsonString.ToString());

            return hubspotObject;
        }

        public HubSpotObject.Contact.RootObject GetContactByUserToken(string token)
        {
            var subPath = string.Format("contact/utk/{0}/profile", token);
            JObject jsonString = Call(subpath: subPath, contentType: "application/json");
            HubSpotObject.Contact.RootObject hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.Contact.RootObject>(jsonString.ToString());

            return hubspotObject;
        }

        public HubSpotObject.Contacts.RootObject SearchContacts(string query, string count = "", string offset = "")
        {
            var optionalParams = new Dictionary<string, string>();

            if (count.Length > 0)
            {
                optionalParams["count"] = count;
            }

            if (offset.Length > 0)
            {
                optionalParams["offset"] = offset;
            }

            JObject jsonString = Call(subpath: "search/query", query: query, optionalParams: optionalParams, contentType: "application/json");
            HubSpotObject.Contacts.RootObject hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.Contacts.RootObject>(jsonString.ToString());

            return hubspotObject;
        }

        public HubSpotObject.Contacts.Statistics GetContactStatistics()
        {
            JObject jsonString = Call(subpath: "contacts/statistics", contentType: "application/json");
            HubSpotObject.Contacts.Statistics hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.Contacts.Statistics>(jsonString.ToString());

            return hubspotObject;
        }
    }
}