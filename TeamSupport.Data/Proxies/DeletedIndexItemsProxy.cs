using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  
  public partial class DeletedIndexItem : BaseItem
  {
    public DeletedIndexItemProxy GetProxy()
    {
      DeletedIndexItemProxy result = new DeletedIndexItemProxy();
      result.RefType = this.RefType;
      result.RefID = this.RefID;
      result.DeletedIndexID = this.DeletedIndexID;
       
      result.DateDeleted = DateTime.SpecifyKind(this.DateDeletedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
