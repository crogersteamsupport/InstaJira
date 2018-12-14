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
  [KnownType(typeof(PhoneNumbersViewItemProxy))]
  public class PhoneNumbersViewItemProxy
  {
    public PhoneNumbersViewItemProxy() {}
    [DataMember] public int PhoneID { get; set; }
    [DataMember] public int? PhoneTypeID { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
    [DataMember] public string PhoneNumber { get; set; }
    [DataMember] public string Extension { get; set; }
    [DataMember] public string OtherTypeName { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public string PhoneType { get; set; }
    [DataMember] public string CreatorName { get; set; }
    [DataMember] public string ModifierName { get; set; }
          
  }
  
  public partial class PhoneNumbersViewItem : BaseItem
  {
    public PhoneNumbersViewItemProxy GetProxy()
    {
      PhoneNumbersViewItemProxy result = new PhoneNumbersViewItemProxy();
      result.ModifierName = this.ModifierName;
      result.CreatorName = this.CreatorName;
      result.PhoneType = this.PhoneType;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.OtherTypeName = this.OtherTypeName;
      result.Extension = this.Extension;
      result.PhoneNumber = this.PhoneNumber;
      result.RefType = this.RefType;
      result.RefID = this.RefID;
      result.PhoneTypeID = this.PhoneTypeID;
      result.PhoneID = this.PhoneID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
