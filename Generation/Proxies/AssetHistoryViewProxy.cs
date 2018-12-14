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
  [KnownType(typeof(AssetHistoryViewItemProxy))]
  public class AssetHistoryViewItemProxy
  {
    public AssetHistoryViewItemProxy() {}
    [DataMember] public int HistoryID { get; set; }
    [DataMember] public int AssetID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public DateTime? ActionTime { get; set; }
    [DataMember] public string ActionDescription { get; set; }
    [DataMember] public int? ShippedFrom { get; set; }
    [DataMember] public string NameAssignedFrom { get; set; }
    [DataMember] public int? ShippedTo { get; set; }
    [DataMember] public string NameAssignedTo { get; set; }
    [DataMember] public string TrackingNumber { get; set; }
    [DataMember] public string ShippingMethod { get; set; }
    [DataMember] public string ReferenceNum { get; set; }
    [DataMember] public string Comments { get; set; }
    [DataMember] public DateTime? DateCreated { get; set; }
    [DataMember] public int? Actor { get; set; }
    [DataMember] public string ActorName { get; set; }
    [DataMember] public int? RefType { get; set; }
    [DataMember] public DateTime? DateModified { get; set; }
    [DataMember] public int? ModifierID { get; set; }
    [DataMember] public string ModifierName { get; set; }
    [DataMember] public int? ShippedFromRefType { get; set; }
          
  }
  
  public partial class AssetHistoryViewItem : BaseItem
  {
    public AssetHistoryViewItemProxy GetProxy()
    {
      AssetHistoryViewItemProxy result = new AssetHistoryViewItemProxy();
      result.ShippedFromRefType = this.ShippedFromRefType;
      result.ModifierName = this.ModifierName;
      result.ModifierID = this.ModifierID;
      result.RefType = this.RefType;
      result.ActorName = this.ActorName;
      result.Actor = this.Actor;
      result.Comments = this.Comments;
      result.ReferenceNum = this.ReferenceNum;
      result.ShippingMethod = this.ShippingMethod;
      result.TrackingNumber = this.TrackingNumber;
      result.NameAssignedTo = this.NameAssignedTo;
      result.ShippedTo = this.ShippedTo;
      result.NameAssignedFrom = this.NameAssignedFrom;
      result.ShippedFrom = this.ShippedFrom;
      result.ActionDescription = this.ActionDescription;
      result.OrganizationID = this.OrganizationID;
      result.AssetID = this.AssetID;
      result.HistoryID = this.HistoryID;
       
       
      result.DateModified = this.DateModifiedUtc == null ? this.DateModifiedUtc : DateTime.SpecifyKind((DateTime)this.DateModifiedUtc, DateTimeKind.Utc); 
      result.DateCreated = this.DateCreatedUtc == null ? this.DateCreatedUtc : DateTime.SpecifyKind((DateTime)this.DateCreatedUtc, DateTimeKind.Utc); 
      result.ActionTime = this.ActionTimeUtc == null ? this.ActionTimeUtc : DateTime.SpecifyKind((DateTime)this.ActionTimeUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
