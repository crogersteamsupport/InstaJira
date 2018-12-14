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
  [KnownType(typeof(ReportFolderProxy))]
  public class ReportFolderProxy
  {
    public ReportFolderProxy() {}
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public int FolderID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public int? ParentID { get; set; }
    [DataMember] public int? CreatorID { get; set; }
    [DataMember] public DateTime? DateCreated { get; set; }
          
  }
  
  public partial class ReportFolder : BaseItem
  {
    public ReportFolderProxy GetProxy()
    {
      ReportFolderProxy result = new ReportFolderProxy();
      result.CreatorID = this.CreatorID;
      result.ParentID = this.ParentID;
      result.Name = (this.Name);
      result.FolderID = this.FolderID;
      result.OrganizationID = this.OrganizationID;
       
       
      result.DateCreated = this.DateCreatedUtc == null ? this.DateCreatedUtc : DateTime.SpecifyKind((DateTime)this.DateCreatedUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
