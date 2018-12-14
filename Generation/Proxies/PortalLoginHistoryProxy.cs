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
  [KnownType(typeof(PortalLoginHistoryItemProxy))]
  public class PortalLoginHistoryItemProxy
  {
    public PortalLoginHistoryItemProxy() {}
    [DataMember] public int PortalLoginID { get; set; }
    [DataMember] public string UserName { get; set; }
    [DataMember] public int? OrganizationID { get; set; }
    [DataMember] public string OrganizationName { get; set; }
    [DataMember] public bool? Success { get; set; }
    [DataMember] public DateTime? LoginDateTime { get; set; }
    [DataMember] public string IPAddress { get; set; }
    [DataMember] public string Browser { get; set; }
    [DataMember] public int UserID { get; set; }
          
  }
  
  public partial class PortalLoginHistoryItem : BaseItem
  {
    public PortalLoginHistoryItemProxy GetProxy()
    {
      PortalLoginHistoryItemProxy result = new PortalLoginHistoryItemProxy();
      result.UserID = this.UserID;
      result.Browser = this.Browser;
      result.IPAddress = this.IPAddress;
      result.Success = this.Success;
      result.OrganizationName = this.OrganizationName;
      result.OrganizationID = this.OrganizationID;
      result.UserName = this.UserName;
      result.PortalLoginID = this.PortalLoginID;
       
       
      result.LoginDateTime = this.LoginDateTimeUtc == null ? this.LoginDateTimeUtc : DateTime.SpecifyKind((DateTime)this.LoginDateTimeUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
