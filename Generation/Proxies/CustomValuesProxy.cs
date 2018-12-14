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
  [KnownType(typeof(CustomValueProxy))]
  public class CustomValueProxy
  {
    public CustomValueProxy() {}
    [DataMember] public int CustomValueID { get; set; }
    [DataMember] public int CustomFieldID { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public string Value { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public int? ImportFileID { get; set; }
          
  }
  
  public partial class CustomValue : BaseItem
  {
    public CustomValueProxy GetProxy()
    {
      CustomValueProxy result = new CustomValueProxy();
      result.ImportFileID = this.ImportFileID;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.Value = this.Value;
      result.RefID = this.RefID;
      result.CustomFieldID = this.CustomFieldID;
      result.CustomValueID = this.CustomValueID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
