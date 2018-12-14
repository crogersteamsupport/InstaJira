using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  
  public partial class PhoneNumbersViewItem : BaseItem
  {
    public PhoneNumbersViewItemProxy GetProxy()
    {
      PhoneNumbersViewItemProxy result = new PhoneNumbersViewItemProxy();
      result.ModifierName = this.ModifierName;
      result.CreatorName = this.CreatorName;
      result.PhoneType =  (this.PhoneType);
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.OtherTypeName = (this.OtherTypeName);
      result.Extension = (this.Extension);
      result.PhoneNumber = (this.PhoneNumber);
      result.RefType = this.RefType;
      result.RefID = this.RefID;
      result.PhoneTypeID = this.PhoneTypeID;
      result.PhoneID = this.PhoneID;
		result.FormattedPhoneNumber = this.FormattedPhoneNumber;

      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
