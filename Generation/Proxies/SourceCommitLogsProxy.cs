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
  [KnownType(typeof(SourceCommitLogProxy))]
  public class SourceCommitLogProxy
  {
    public SourceCommitLogProxy() {}
    [DataMember] public int CommitID { get; set; }
    [DataMember] public DateTime? CommitDateTime { get; set; }
    [DataMember] public int? OrganizationID { get; set; }
    [DataMember] public int? ProductID { get; set; }
    [DataMember] public int? VersionID { get; set; }
    [DataMember] public string UserName { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public int? Revision { get; set; }
    [DataMember] public string Tickets { get; set; }
    [DataMember] public string RawCommitText { get; set; }
    [DataMember] public string Status { get; set; }
          
  }
  
  public partial class SourceCommitLog : BaseItem
  {
    public SourceCommitLogProxy GetProxy()
    {
      SourceCommitLogProxy result = new SourceCommitLogProxy();
      result.Status = this.Status;
      result.RawCommitText = this.RawCommitText;
      result.Tickets = this.Tickets;
      result.Revision = this.Revision;
      result.Description = this.Description;
      result.UserName = this.UserName;
      result.VersionID = this.VersionID;
      result.ProductID = this.ProductID;
      result.OrganizationID = this.OrganizationID;
      result.CommitID = this.CommitID;
       
       
      result.CommitDateTime = this.CommitDateTimeUtc == null ? this.CommitDateTimeUtc : DateTime.SpecifyKind((DateTime)this.CommitDateTimeUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
