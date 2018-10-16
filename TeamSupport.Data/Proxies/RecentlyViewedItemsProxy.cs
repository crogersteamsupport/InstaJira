using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(RecentlyViewedItemProxy))]
  public class RecentlyViewedItemProxy
  {
    public RecentlyViewedItemProxy() {}
    [DataMember] public int UserID { get; set; }
    [DataMember] public int RefType { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public DateTime DateViewed { get; set; }
          
  }
  
  public partial class RecentlyViewedItem : BaseItem
  {
    public RecentlyViewedItemProxy GetProxy()
    {
      RecentlyViewedItemProxy result = new RecentlyViewedItemProxy();
      result.RefID = this.RefID;
      result.RefType = this.RefType;
      result.UserID = this.UserID;
       
      result.DateViewed = DateTime.SpecifyKind(this.DateViewedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
