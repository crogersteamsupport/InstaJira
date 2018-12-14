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
  [KnownType(typeof(DeletedIndexItemProxy))]
  public class DeletedIndexItemProxy
  {
    public DeletedIndexItemProxy() {}
    [DataMember] public int DeletedIndexID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
    [DataMember] public DateTime DateDeleted { get; set; }
          
  }
  
  public partial class DeletedIndexItem : BaseItem
  {
    public DeletedIndexItemProxy GetProxy()
    {
      DeletedIndexItemProxy result = new DeletedIndexItemProxy();
      result.RefType = this.RefType;
      result.RefID = this.RefID;
      result.OrganizationID = this.OrganizationID;
      result.DeletedIndexID = this.DeletedIndexID;
       
      result.DateDeleted = DateTime.SpecifyKind(this.DateDeletedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
