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
  [KnownType(typeof(AssetHistoryItemProxy))]
  public class AssetHistoryItemProxy
  {
    public AssetHistoryItemProxy() {}
    [DataMember] public int HistoryID { get; set; }
    [DataMember] public int AssetID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public DateTime? ActionTime { get; set; }
    [DataMember] public string ActionDescription { get; set; }
    [DataMember] public int? ShippedFrom { get; set; }
    [DataMember] public int? ShippedTo { get; set; }
    [DataMember] public string TrackingNumber { get; set; }
    [DataMember] public string ShippingMethod { get; set; }
    [DataMember] public string ReferenceNum { get; set; }
    [DataMember] public string Comments { get; set; }
    [DataMember] public DateTime? DateCreated { get; set; }
    [DataMember] public int? Actor { get; set; }
    [DataMember] public int? RefType { get; set; }
    [DataMember] public DateTime? DateModified { get; set; }
    [DataMember] public int? ModifierID { get; set; }
    [DataMember] public int? ShippedFromRefType { get; set; }
    [DataMember] public int? ImportFileID { get; set; }
          
  }
  
  public partial class AssetHistoryItem : BaseItem
  {
    public AssetHistoryItemProxy GetProxy()
    {
      AssetHistoryItemProxy result = new AssetHistoryItemProxy();
      result.ImportFileID = this.ImportFileID;
      result.ShippedFromRefType = this.ShippedFromRefType;
      result.ModifierID = this.ModifierID;
      result.RefType = this.RefType;
      result.Actor = this.Actor;
      result.Comments = this.Comments;
      result.ReferenceNum = this.ReferenceNum;
      result.ShippingMethod = this.ShippingMethod;
      result.TrackingNumber = this.TrackingNumber;
      result.ShippedTo = this.ShippedTo;
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
