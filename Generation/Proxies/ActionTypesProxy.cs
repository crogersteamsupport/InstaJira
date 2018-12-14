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
  [KnownType(typeof(ActionTypeProxy))]
  public class ActionTypeProxy
  {
    public ActionTypeProxy() {}
    [DataMember] public int ActionTypeID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public bool IsTimed { get; set; }
    [DataMember] public int Position { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
          
  }
  
  public partial class ActionType : BaseItem
  {
    public ActionTypeProxy GetProxy()
    {
      ActionTypeProxy result = new ActionTypeProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.Position = this.Position;
      result.IsTimed = this.IsTimed;
      result.Description = this.Description;
      result.Name = this.Name;
      result.OrganizationID = this.OrganizationID;
      result.ActionTypeID = this.ActionTypeID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
