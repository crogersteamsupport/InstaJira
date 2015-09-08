using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Ganss.XSS;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(PhoneNumberProxy))]
  public class PhoneNumberProxy
  {
    public PhoneNumberProxy() {}
    [DataMember] public int PhoneID { get; set; }
    [DataMember] public int? PhoneTypeID { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
    [DataMember] public string Number { get; set; }
    [DataMember] public string Extension { get; set; }
    [DataMember] public string OtherTypeName { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public string PhoneTypeName { get; set; }
    [DataMember] public int? ImportFileID { get; set; }
          
  }
  
  public partial class PhoneNumber : BaseItem
  {
    public PhoneNumberProxy GetProxy()
    {
      PhoneNumberProxy result = new PhoneNumberProxy();
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      result.ImportFileID = this.ImportFileID;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.OtherTypeName = sanitizer.Sanitize(this.OtherTypeName);
      result.Extension = this.Extension;
      result.Number = this.Number;
      result.RefType = this.RefType;
      result.RefID = this.RefID;
      result.PhoneTypeID = this.PhoneTypeID;
      result.PhoneID = this.PhoneID;
      result.PhoneTypeName = sanitizer.Sanitize(this.PhoneTypeName);
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
