using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  
  public partial class PhoneNumber : BaseItem
  {
    public PhoneNumberProxy GetProxy()
    {
      PhoneNumberProxy result = new PhoneNumberProxy();
      result.ImportFileID = this.ImportFileID;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.OtherTypeName = (this.OtherTypeName);
      result.Extension = this.Extension;
      result.Number = this.Number;
      result.RefType = this.RefType;
      result.RefID = this.RefID;
      result.PhoneTypeID = this.PhoneTypeID;
      result.PhoneID = this.PhoneID;
      result.PhoneTypeName = (this.PhoneTypeName);
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
