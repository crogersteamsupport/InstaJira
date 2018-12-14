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
  [KnownType(typeof(ExceptionLogViewItemProxy))]
  public class ExceptionLogViewItemProxy
  {
    public ExceptionLogViewItemProxy() {}
    [DataMember] public int ExceptionLogID { get; set; }
    [DataMember] public string URL { get; set; }
    [DataMember] public string PageInfo { get; set; }
    [DataMember] public string ExceptionName { get; set; }
    [DataMember] public string Message { get; set; }
    [DataMember] public string StackTrace { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public string FirstName { get; set; }
    [DataMember] public string LastName { get; set; }
    [DataMember] public string Name { get; set; }
          
  }
  
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
