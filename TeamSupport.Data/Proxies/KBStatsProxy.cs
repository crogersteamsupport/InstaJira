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
  [KnownType(typeof(KBStatProxy))]
  public class KBStatProxy
  {
    public KBStatProxy() {}
    [DataMember] public int KBViewID { get; set; }
    [DataMember] public int? OrganizationID { get; set; }
    [DataMember] public int? KBArticleID { get; set; }
    [DataMember] public DateTime? ViewDateTime { get; set; }
    [DataMember] public string ViewIP { get; set; }
    [DataMember] public string SearchTerm { get; set; }
          
  }
  
  public partial class KBStat : BaseItem
  {
    public KBStatProxy GetProxy()
    {
      KBStatProxy result = new KBStatProxy();
      result.SearchTerm = this.SearchTerm;
      result.ViewIP = this.ViewIP;
      result.KBArticleID = this.KBArticleID;
      result.OrganizationID = this.OrganizationID;
      result.KBViewID = this.KBViewID;
       
       
      result.ViewDateTime = this.ViewDateTimeUtc == null ? this.ViewDateTimeUtc : DateTime.SpecifyKind((DateTime)this.ViewDateTimeUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
