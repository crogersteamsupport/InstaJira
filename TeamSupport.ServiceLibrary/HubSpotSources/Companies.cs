using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HubSpotObject = TeamSupport.ServiceLibrary.HubSpotSources.Objects;

namespace TeamSupport.ServiceLibrary
{
    public class Companies : HubSpotBaseClass
    {
        protected object CompaniesApiVersion = "v2";

        public Companies(string apiKey = null, string accessToken = null, string refreshToken = null, string clientId = null, string logPath = null)
            : base(apiKey, accessToken, refreshToken, clientId, logPath)
        {
        }

        protected override string GetPath(string method)
        {
            return string.Format("companies/{0}/{1}", CompaniesApiVersion, method);
        }

        public HubSpotObject.Company.RootObject Create(string company)
        {
            JObject jsonString = Call(subpath: "companies", method: "POST", contentType: "application/json", data: company);
            HubSpotObject.Company.RootObject hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.Company.RootObject>(jsonString.ToString());

            return hubspotObject;
        }

        public HubSpotObject.Company.RootObject Update(string companyId, string data)
        {
            var subpath = string.Format("companies/{0}", companyId);
            JObject jsonString = Call(subpath: subpath, method: "POST", contentType: "application/json", data: data);
            HubSpotObject.Company.RootObject hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.Company.RootObject>(jsonString.ToString());

            return hubspotObject;
        }

        public bool Delete(string companyId)
        {
            bool wasDeleted = false;
            var subpath = string.Format("companies/{0}", companyId);
            JObject jsonDelete = Call(subpath: subpath, method: "DELETE");

            try
            {
              dynamic stuff = JObject.Parse(jsonDelete.ToString());
              string deletedCompanyId = stuff.companyId;
              string deleted = stuff.deleted;

              if (!string.IsNullOrEmpty(deletedCompanyId) && deletedCompanyId == companyId
                  && !string.IsNullOrEmpty(deleted) && deleted.ToLower() == true.ToString().ToLower())
              {
                wasDeleted = true;
              }
            }
            catch (Exception ex)
            {
              Logs.Write(string.Format("Error deleting the company {0}. Might or might not be deleted, verify.{1}{2}{1}{2}", companyId, Environment.NewLine, ex.Message, ex.StackTrace));
            }

            return wasDeleted;
        }

        public HubSpotObject.Company.RootObject GetById(string companyId, bool log = true)
        {
            var subPath = string.Format("companies/{0}", companyId);

            JObject jsonString = Call(subpath: subPath, contentType: "application/json", log: log);
            HubSpotObject.Company.RootObject hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.Company.RootObject>(jsonString.ToString());

            return hubspotObject;
        }

        public HubSpotObject.Companies.RootObject GetAllPaged(string portalId = "", string count = "", string offset = "") {
            var subPath = "companies/paged";
            Dictionary<string, string> optionalParameters = new Dictionary<string,string>();

			optionalParameters.Add("properties", "name&properties=address&properties=address2&properties=city&properties=state&properties=zip&properties=country&properties=phone&properties=fax&properties=lifecyclestage&properties=hs_lastmodifieddate");

            if (!string.IsNullOrEmpty(portalId)) {
                optionalParameters.Add("portalId", portalId);
            }

            if (!string.IsNullOrEmpty(count)) {
                optionalParameters.Add("limit", count);
            }

            if (!string.IsNullOrEmpty(offset)) {
                optionalParameters.Add("offset", offset);
            }

            if (!optionalParameters.Any() || optionalParameters.Count == 0) {
                optionalParameters = null;
            }

            JObject jsonString = Call(subpath: subPath, optionalParams: optionalParameters, contentType: "application/json");
            HubSpotObject.Companies.RootObject hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.Companies.RootObject>(jsonString.ToString());

            return hubspotObject;
        }

        public HubSpotObject.Companies.RootObject GetAllRecentlyModified(string portalId = "", string count = "", string offset = "")
        {
          var subPath = "companies/recent/modified";
          Dictionary<string, string> optionalParameters = new Dictionary<string,string>();

          if (!string.IsNullOrEmpty(portalId))
          {
            optionalParameters.Add("portalId", portalId);
          }

          if (!string.IsNullOrEmpty(count))
          {
            optionalParameters.Add("count", count);
          }

          if (!string.IsNullOrEmpty(offset))
          {
            optionalParameters.Add("offset", offset);
          }

          if (!optionalParameters.Any() || optionalParameters.Count == 0)
          {
            optionalParameters = null;
          }

          JObject jsonString = Call(subpath: subPath, optionalParams: optionalParameters, contentType: "application/json");
          HubSpotObject.Companies.RootObject hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.Companies.RootObject>(jsonString.ToString());

          return hubspotObject;
        }

        public HubSpotObject.Companies.RootObject GetAllRecentlyCreated(string count = "", string offset = "")
        {
          var subPath = "companies/recent/created";
          Dictionary<string, string> optionalParameters = new Dictionary<string, string>();

          if (!string.IsNullOrEmpty(count))
          {
            optionalParameters.Add("count", count);
          }

          if (!string.IsNullOrEmpty(offset))
          {
            optionalParameters.Add("offset", offset);
          }

          if (!optionalParameters.Any() || optionalParameters.Count == 0)
          {
            optionalParameters = null;
          }

          JObject jsonString = Call(subpath: subPath, optionalParams: optionalParameters);
          HubSpotObject.Companies.RootObject hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.Companies.RootObject>(jsonString.ToString());

          return hubspotObject;
        }

        public HubSpotObject.CompanyContacts.RootObject GetContactsOfCompany(string companyId, string count = "", string contactOffset = "")
        {
            var subPath = string.Format("companies/{0}/contacts", companyId);
            var optionalParams = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(count) && count.Length > 0)
            {
              optionalParams["count"] = count;
            }

            if (!string.IsNullOrEmpty(contactOffset) && contactOffset.Length > 0)
            {
              optionalParams["vidOffset"] = contactOffset;
            }

            JObject jsonString = Call(subpath: subPath, optionalParams: optionalParams, contentType: "application/json");
            HubSpotObject.CompanyContacts.RootObject hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.CompanyContacts.RootObject>(jsonString.ToString());

            return hubspotObject;
        }

        public HubSpotObject.CompanyContacts.RootObject GetContactIdsByCompany(string companyId)
        {
            var subPath = string.Format("companies/{0}/vids", companyId);
            JObject jsonString = Call(subpath: subPath, contentType: "application/json");
            HubSpotObject.CompanyContacts.RootObject hubspotObject = JsonConvert.DeserializeObject<HubSpotObject.CompanyContacts.RootObject>(jsonString.ToString());

            return hubspotObject;
        }
    }
}
