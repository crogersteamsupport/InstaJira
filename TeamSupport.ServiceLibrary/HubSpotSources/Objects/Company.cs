using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TeamSupport.ServiceLibrary.HubSpotSources.Objects
{
  public class Company
  {
    public class Version
    {
      public string name { get; set; }
      public string value { get; set; }
      public long timestamp { get; set; }
      public string sourceId { get; set; }
      public string source { get; set; }
      public List<object> sourceVid { get; set; }
    }

    public class ObjectValue
    {
      public string value { get; set; }
      public long timestamp { get; set; }
      public string source { get; set; }
      public string sourceId { get; set; }
      public List<Version> versions { get; set; }
    }
        
    public class Properties
    {
      public ObjectValue name { get; set; }
      public ObjectValue address { get; set; }
      public ObjectValue address2 { get; set; }
      public ObjectValue city { get; set; }
      public ObjectValue zip { get; set; }
      public ObjectValue state { get; set; }
      public ObjectValue country { get; set; }
      public ObjectValue phone { get; set; }
      public ObjectValue fax { get; set; }
      public ObjectValue lifecyclestage { get; set; }
      public ObjectValue twitterfollowers { get; set; }
      public ObjectValue website { get; set; }
      public ObjectValue num_associated_contacts { get; set; }
      public ObjectValue createdate { get; set; }
      public ObjectValue description { get; set; }
      public ObjectValue industry { get; set; }
      public ObjectValue numberofemployees { get; set; }
      public ObjectValue linkedin_company_page { get; set; }
      public ObjectValue hubspot_owner_id { get; set; }
      public ObjectValue hs_lastmodifieddate { get; set; }
      public ObjectValue hubspot_owner_assigneddate { get; set; }
      public ObjectValue annualrevenue { get; set; }
      public ObjectValue domain { get; set; }
      public ObjectValue facebookfans { get; set; }
    }

    public class RootObject
    {
      public int portalId { get; set; }
      public int companyId { get; set; }
      public bool isDeleted { get; set; }
      public Properties properties { get; set; }
    }
  }
}
