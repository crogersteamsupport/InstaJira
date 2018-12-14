using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace TeamSupport.ServiceLibrary.HubSpotSources.Objects
{
  public class Companies
  {
    public class Version
    {
      public string name { get; set; }
      public string value { get; set; }
      public long timestamp { get; set; }
      public List<object> sourceVid { get; set; }
    }

    public class ObjectValue
    {
      public string value { get; set; }
      public long timestamp { get; set; }
      public object source { get; set; }
      public object sourceId { get; set; }
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
      public ObjectValue website { get; set; }
      public ObjectValue description { get; set; }
      public ObjectValue industry { get; set; }
      public ObjectValue lifecyclestage { get; set; }
      public ObjectValue timezone { get; set; }
      public ObjectValue createdate { get; set; }
      public ObjectValue numberofemployees { get; set; }
      public ObjectValue linkedin_company_page { get; set; }
      public ObjectValue linkedinbio { get; set; }
      public ObjectValue hubspot_owner_id { get; set; }
      public ObjectValue hs_lastmodifieddate { get; set; }
      public ObjectValue hubspot_owner_assigneddate { get; set; }
      public ObjectValue annualrevenue { get; set; }
      public ObjectValue domain { get; set; }
      public ObjectValue founded_year { get; set; }
      public ObjectValue is_public { get; set; }
      public ObjectValue twitterfollowers { get; set; }
      public ObjectValue num_associated_contacts { get; set; }
      public ObjectValue hs_analytics_num_visits { get; set; }
      public ObjectValue hs_analytics_source { get; set; }
      public ObjectValue hs_analytics_num_page_views { get; set; }
      public ObjectValue facebookfans { get; set; }
      public ObjectValue notes_last_updated { get; set; }
      public ObjectValue closedate { get; set; }
      public ObjectValue hs_analytics_first_timestamp { get; set; }
      public ObjectValue first_contact_createdate { get; set; }
      public ObjectValue hs_analytics_source_data_2 { get; set; }
      public ObjectValue hs_analytics_source_data_1 { get; set; }
      public ObjectValue num_notes { get; set; }
    }

    public class Result
    {
      public int portalId { get; set; }
      public int companyId { get; set; }
      public bool isDeleted { get; set; }
      public Properties properties { get; set; }
    }

    public class RootObject
    {
      public List<Result> results { get; set; }
	  public List<Result> companies { get; set; }
      public bool hasMore { get; set; }
	  [JsonProperty("has-more")]
	  public bool hasMorePaged { get; set; }
      public int offset { get; set; }
      public int total { get; set; }
    }
  }
}
