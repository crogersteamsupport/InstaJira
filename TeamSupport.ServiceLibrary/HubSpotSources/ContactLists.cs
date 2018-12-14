﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace TeamSupport.ServiceLibrary
{
    public class ContactLists : HubSpotBaseClass
    {

        private const string ContactListsApiVersion = "v1";
      
        public ContactLists(string apiKey = null, string accessToken = null, string refreshToken = null, string clientId = null, string logPath = null)
          : base(apiKey, accessToken, refreshToken, clientId, logPath)
        {
        }

        protected override string GetPath(string method)
        {
            return string.Format("contacts/{0}/{1}", ContactListsApiVersion, method);
        }

        public JObject CreateContactList(string data)
        {
            return Call(subpath: "lists", method:"POST", contentType:"application/json", data: data);
        }

        public void UpdateContactList(string listId, string data)
        {
            var subPath = string.Format("lists/{0}", listId);
            Call(subpath:subPath, method:"POST", contentType:"application/json", data:data);
        }

        public void DeleteContactList(string listId)
        {
            var subPath = string.Format("lists/{0}", listId);
            Call(subpath: subPath, method: "DELETE");
        }

        public JObject GetContactListById(string listId)
        {
            var subPath = string.Format("lists/{0}", listId);
            return Call(subpath: subPath);
        }

        public JObject GetContactLists(string count = "", string offset = "")
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
            return Call(subpath: "lists", optionalParams: optionalParams);
        }

        public JObject GetBatchContactLists(params string[] listIds)
        {
            var ids = listIds.Aggregate("", (current, id) => string.Format("{0}listId={1}&", current, id));
            return Call("lists/batch", other: ids);
        }

        public JObject GetStaticContactLists(string count = "", string offset = "")
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
            return Call(subpath: "lists/static", optionalParams: optionalParams);
        }

        public JObject GetDynamicContactLists(string count = "", string offset = "")
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
            return Call(subpath: "lists/dynamic", optionalParams: optionalParams);
        }

        public JObject GetContactsInList(string listId, string count = "", string offset = "", string property = "")
        {
            var optionalParams = new Dictionary<string, string>();
            if (count.Length > 0)
            {
                optionalParams["count"] = count;
            }
            if (offset.Length > 0)
            {
                optionalParams["vidOffset"] = offset;
            }
            if (property.Length > 0)
            {
                optionalParams["propery"] = property;
            }
            var subpath = string.Format("lists/{0}/contacts/all", listId);
            return Call(subpath: subpath, optionalParams: optionalParams);
        }

        public JObject GetRecentlyAddedContactsInList(string listId, string count = "", string timeOffset = "", string contactOffset = "", string property = "")
        {
            var optionalParams = new Dictionary<string, string>();
            if (count.Length > 0)
            {
                optionalParams["count"] = count;
            }
            if (contactOffset.Length > 0)
            {
                optionalParams["vidOffset"] = contactOffset;
            }
            if (timeOffset.Length > 0)
            {
                optionalParams["timeOffset"] = timeOffset;
            }
            if (property.Length > 0)
            {
                optionalParams["propery"] = property;
            }
            var subpath = string.Format("lists/{0}/contacts/recent", listId);
            return Call(subpath: subpath, optionalParams: optionalParams);
        }

        public void RefreshExistingContactList(string listId)
        {
            var subpath = string.Format("lists/{0}/refresh", listId);
            Call(subpath: subpath, method:"POST");
        }

        public JObject AddExistingContactToList(string listId, string data)
        {
            var subPath = string.Format("lists/{0}/add", listId);
            return Call(subpath: subPath, method: "POST", contentType: "application/json", data: data);   
        }

        public JObject RemoveExistingContactFromList(string listId, string data)
        {
            var subPath = string.Format("lists/{0}/remove", listId);
            return Call(subpath: subPath, method: "POST", contentType: "application/json", data: data);   
        }
    }
}