using System;
using System.Collections.Generic;

namespace TeamSupport.ServiceLibrary.HubSpotSources.Objects
{
  public class Engagement
  {
    public class EngagementItem
    {
      public int id { get; set; }
      public int portalId { get; set; }
      public bool active { get; set; }
      public long createdAt { get; set; }
      public long lastUpdated { get; set; }
      public string type { get; set; }
      public long timestamp { get; set; }
    }

    public class Associations
    {
      public List<int> contactIds { get; set; }
      public List<int> companyIds { get; set; }
      public List<int> dealIds { get; set; }
      public List<int> ownerIds { get; set; }
    }

    public class Metadata
    {
      public string body { get; set; }
    }

    public class RootObject
    {
      public EngagementItem engagement { get; set; }
      public Associations associations { get; set; }
      public Metadata metadata { get; set; }
    }
  }
}
