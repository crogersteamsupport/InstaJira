using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using HubSpotObject = TeamSupport.ServiceLibrary.HubSpotSources.Objects;

namespace TeamSupport.ServiceLibrary
{
    public class Engagements : HubSpotBaseClass
    {
      protected object ProspectsApiVersion = "v1";

		  public Engagements(string apiKey = null, string accessToken = null, string refreshToken = null, string clientId = null, string logPath = null)
        : base(apiKey, accessToken, refreshToken, clientId, logPath)
        {
        }

        protected override string GetPath(string method)
        {
			    return string.Format("engagements/{0}/{1}", ProspectsApiVersion, method);
        }

        public HubSpotObject.Engagement.RootObject Create(HubSpotObject.Engagement.RootObject engagement)
        {
          string jsonBody = JsonConvert.SerializeObject(engagement, Formatting.Indented);
          JObject jsonString = Call(subpath: "engagements", method: "POST", contentType: "application/json", data: jsonBody);
          HubSpotObject.Engagement.RootObject hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.Engagement.RootObject>(jsonString.ToString());

          return hubspotObject;
        }

        public HubSpotObject.Engagement.RootObject Update(string engagementId, string data)
        {
            var subpath = string.Format("engagements/{0}", engagementId);
            JObject jsonString = Call(subpath: subpath, method: "POST", contentType: "application/json", data: data);
            HubSpotObject.Engagement.RootObject hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.Engagement.RootObject>(jsonString.ToString());

            return hubspotObject;
        }

        public HubSpotObject.Engagement.RootObject GetById(string engagementId)
        {
            var subPath = string.Format("engagements/{0}", engagementId);
            JObject jsonString = Call(subpath: subPath, contentType: "application/json");
            HubSpotObject.Engagement.RootObject hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.Engagement.RootObject>(jsonString.ToString());

            return hubspotObject;
        }

		    public bool Delete(string engagementId)
		    {
          bool wasDeleted = false;
			    var subpath = string.Format("engagements/{0}", engagementId);
          JObject jsonDelete = Call(subpath: subpath, method: "DELETE", contentType: "application/json");

          try
          {
            dynamic stuff = JObject.Parse(jsonDelete.ToString());
            string id = stuff.engagementId;
            string deleted = stuff.deleted;

            if (!string.IsNullOrEmpty(id) && id == engagementId
                && !string.IsNullOrEmpty(deleted) && deleted.ToLower() == true.ToString().ToLower())
            {
              wasDeleted = true;
            }
          }
          catch (Exception ex)
          {
            Logs.Write(string.Format("Error deleting the engagement {0}. Might or might not be deleted, verify.{1}{2}{1}{2}", engagementId, Environment.NewLine, ex.Message, ex.StackTrace));
          }

          return wasDeleted;
		    }
    }
}
