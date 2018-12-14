using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TeamSupport.ServiceLibrary.HubSpotSources.Objects
{
  public class Contact
  {
    [DataContract]
    public class Version
    {
      [DataMember(Name = "value")]
      public string value { get; set; }
      [DataMember(Name = "source-type")]
      public string sourceType { get; set; }
      [DataMember(Name = "source-id")]
      public string sourceId { get; set; }
      [DataMember(Name = "source-label")]
      public object sourceLabel { get; set; }
      [DataMember(Name = "timestamp")]
      public long timestamp { get; set; }
      [DataMember(Name = "selected")]
      public bool selected { get; set; }
    }

    public class ObjectValue
    {
      public string value { get; set; }
      public List<Version> versions { get; set; }
    }

    public class Properties
    {
      public ObjectValue firstname { get; set; }
      public ObjectValue lastname { get; set; }
      public ObjectValue title { get; set; }
      public ObjectValue jobtitle { get; set; }
      public ObjectValue createdate { get; set; }
      public ObjectValue phone { get; set; }
      public ObjectValue mobilephone { get; set; }
      public ObjectValue fax { get; set; }
      public ObjectValue company { get; set; }
      public ObjectValue email { get; set; }
      public ObjectValue associatedcompanyid { get; set; }
      public ObjectValue associatedcompanylastupdated { get; set; }
      public ObjectValue website { get; set; }
      public ObjectValue closedate { get; set; }
      public ObjectValue lastmodifieddate { get; set; }
      public ObjectValue days_to_close { get; set; }
      public ObjectValue notes_last_updated { get; set; }
      public ObjectValue lifecyclestage { get; set; }
      public ObjectValue num_notes { get; set; }
      public ObjectValue num_conversion_events { get; set; }
      public ObjectValue hs_analytics_last_url { get; set; }
      public ObjectValue num_unique_conversion_events { get; set; }
      public ObjectValue hs_analytics_revenue { get; set; }
      public ObjectValue hs_social_num_broadcast_clicks { get; set; }
      public ObjectValue hs_analytics_first_referrer { get; set; }
      public ObjectValue hs_analytics_last_timestamp { get; set; }
      public ObjectValue hs_email_optout { get; set; }
      public ObjectValue hs_analytics_num_visits { get; set; }
      public ObjectValue hs_social_linkedin_clicks { get; set; }
      public ObjectValue hs_social_last_engagement { get; set; }
      public ObjectValue hs_analytics_source { get; set; }
      public ObjectValue hs_analytics_num_page_views { get; set; }
      public ObjectValue hs_analytics_first_url { get; set; }
      public ObjectValue hs_email_optout_459371 { get; set; }
      public ObjectValue hs_analytics_first_visit_timestamp { get; set; }
      public ObjectValue hs_analytics_first_timestamp { get; set; }
      public ObjectValue hs_email_optout_459379 { get; set; }
      public ObjectValue hs_social_google_plus_clicks { get; set; }
      public ObjectValue hs_analytics_last_referrer { get; set; }
      public ObjectValue hs_lifecyclestage_subscriber_date { get; set; }
      public ObjectValue hs_analytics_average_page_views { get; set; }
      public ObjectValue hs_social_facebook_clicks { get; set; }
      public ObjectValue hs_analytics_num_event_completions { get; set; }
      public ObjectValue hs_analytics_source_data_2 { get; set; }
      public ObjectValue hs_social_twitter_clicks { get; set; }
      public ObjectValue hs_analytics_source_data_1 { get; set; }
      public ObjectValue hs_lifecyclestage_customer_date { get; set; }
    }

    public class Identity
    {
      public string type { get; set; }
      public string value { get; set; }
      public object timestamp { get; set; }
    }

    [DataContract]
    public class IdentityProfile
    {
      [DataMember(Name = "vid")]
      public int vid { get; set; }
      [DataMember(Name = "saved-at-timestamp")]
      public long savedAtTimestamp { get; set; }
      [DataMember(Name = "deleted-changed-timestamp")]
      public long deletedChangedTimestamp { get; set; }
      [DataMember(Name = "identities")]
      public List<Identity> identities { get; set; }
    }

    [DataContract]
    public class AssociatedCompany
    {
      [DataMember(Name = "company-id")]
      public int companyId { get; set; }
      [DataMember(Name = "portal-id")]
      public int portalId { get; set; }
      [DataMember(Name = "properties")]
      public Company.Properties properties { get; set; }
    }

    [DataContract]
    public class RootObject
    {
      [DataMember(Name = "vid")]
      public int vid { get; set; }
      [DataMember(Name = "canonical-vid")]
      public int canonicalVid { get; set; }
      [DataMember(Name = "merged-vids")]
      public List<object> mergedVids { get; set; }
      [DataMember(Name = "portal-id")]
      public int portalId { get; set; }
      [DataMember(Name = "is-contact")]
      public bool isContact { get; set; }
      [DataMember(Name = "profile-token")]
      public string profileToken { get; set; }
      [DataMember(Name = "profile-url")]
      public string profileUrl { get; set; }
      [DataMember(Name = "properties")]
      public Properties properties { get; set; }
      [DataMember(Name = "form-submissions")]
      public List<object> formSubmissions { get; set; }
      [DataMember(Name = "list-memberships")]
      public List<object> listMemberships { get; set; }
      [DataMember(Name = "identity-profiles")]
      public List<IdentityProfile> identityProfiles { get; set; }
      [DataMember(Name = "merge-audits")]
      public List<object> mergeAudits { get; set; }
      [DataMember(Name = "associated-company")]
      public AssociatedCompany associatedCompany { get; set; }
    }

    //POST body for Create
    public class Property
    {
      public string property { get; set; }
      public string value { get; set; }
    }

    public class ContactProperties
    {
      public List<Property> properties { get; set; }
    }
  }
}
