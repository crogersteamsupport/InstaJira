using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ExceptionLogViewItem : BaseItem
  {
    public ExceptionLogViewItemProxy GetProxy()
    {
      ExceptionLogViewItemProxy result = new ExceptionLogViewItemProxy();
      result.Name = this.Name;
      result.LastName = this.LastName;
      result.FirstName = this.FirstName;
      result.CreatorID = this.CreatorID;
      result.StackTrace = this.StackTrace;
      result.Message = this.Message;
      result.ExceptionName = this.ExceptionName;
      result.PageInfo = this.PageInfo;
      result.URL = this.URL;
      result.ExceptionLogID = this.ExceptionLogID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
