using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TeamSupport.ServiceLibrary.HubSpotSources.Objects
{
  public class CompanyContacts
  {
    public class IdentityItem
    {
      public string value { get; set; }
      public string type { get; set; }
      public object timestamp { get; set; }
    }

    public class Identity
    {
      public int vid { get; set; }
      public List<IdentityItem> identity { get; set; }
      public List<object> linkedVid { get; set; }
    }

    public class Property
    {
      public string name { get; set; }
      public string value { get; set; }
      public List<object> sourceVid { get; set; }
    }

    public class Contact
    {
      public List<Identity> identities { get; set; }
      public List<Property> properties { get; set; }
      public List<object> formSubmissions { get; set; }
      public List<object> listMembership { get; set; }
      public int vid { get; set; }
      public int portalId { get; set; }
      public bool isContact { get; set; }
      public List<object> vids { get; set; }
      public List<object> imports { get; set; }
      public string publicToken { get; set; }
      public int canonicalVid { get; set; }
      public List<object> mergeAudit { get; set; }
      public List<object> mergedVids { get; set; }
      public List<object> campaigns { get; set; }
    }

    public class RootObject
    {
      public List<Contact> contacts { get; set; }
      public List<int> vids { get; set; }
      public int vidOffset { get; set; }
      public bool hasMore { get; set; }
    }
  }
}