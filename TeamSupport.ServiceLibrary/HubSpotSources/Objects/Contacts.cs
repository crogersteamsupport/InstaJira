using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TeamSupport.ServiceLibrary.HubSpotSources.Objects
{
  public class Contacts
  {
    public class ObjectValue
    {
      public string value { get; set; }
    }

    public class Properties
    {
      public ObjectValue firstname { get; set; }
      public ObjectValue lastname { get; set; }
      public ObjectValue company { get; set; }
      public ObjectValue lastmodifieddate { get; set; }
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
      public object savedAtTimestamp { get; set; }
      [DataMember(Name = "deleted-changed-timestamp")]
      public long deletedChangedTimestamp { get; set; }
      [DataMember(Name = "identities")]
      public List<Identity> identities { get; set; }
    }

    [DataContract]
    public class Contact
    {
      [DataMember(Name = "addedAt")]
      public object addedAt { get; set; }
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
      [DataMember(Name = "identity-profiles")]
      public List<IdentityProfile> identityProfiles { get; set; }
      [DataMember(Name = "merge-audits")]
      public List<object> mergeAudits { get; set; }
    }

    /// <summary>
    /// The count and lastNewContactAt are returned only when pulling contact statistics
    /// </summary>
    [DataContract]
    public class RootObject
    {
      public int offset { get; set; }
      public bool HasMoreRecords {  get; set; }

      [DataMember(Name = "contacts")]
      public List<Contact> contacts { get; set; }
      [DataMember(Name = "time-offset")]
      public long timeOffset { get; set; }
      [DataMember(Name = "query")]
      public string query { get; set; }
      [DataMember(Name = "vid-offset")]
      private int vid_Offset
      {
        set
        {
          this.offset = value;
        }
      }
      [DataMember(Name = "vidoffset")]
      private int vidsOffset
      {
        set
        {
          this.offset = value;
        }
      }
      [DataMember(Name = "offset")]
      private int Offset
      {
        set
        {
          this.offset = value;
        }
      }
      [DataMember(Name = "has-more")]
      private bool has_more
      {
        set
        {
          this.HasMoreRecords = value;
        }
      }
      [DataMember(Name = "hasMore")]
      public bool hasmore
      {
        set
        {
          this.HasMoreRecords = value;
        }
      }
    }

    public class Statistics
    {
      public int contacts { get; set; }
      public long lastNewContactAt { get; set; }
    }
  }
}
