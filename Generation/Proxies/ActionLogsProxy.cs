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
  [KnownType(typeof(ActionLogProxy))]
  public class ActionLogProxy
  {
    public ActionLogProxy() {}
    [DataMember] public int ActionLogID { get; set; }
    [DataMember] public int? OrganizationID { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public ActionLogType ActionLogType { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
          
  }
  
  public partial class ActionLog : BaseItem
  {
    public ActionLogProxy GetProxy()
    {
      ActionLogProxy result = new ActionLogProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.Description = this.Description;
      result.ActionLogType = this.ActionLogType;
      result.RefID = this.RefID;
      result.RefType = this.RefType;
      result.OrganizationID = this.OrganizationID;
      result.ActionLogID = this.ActionLogID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
