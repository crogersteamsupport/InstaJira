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
  [KnownType(typeof(ExceptionLogProxy))]
  public class ExceptionLogProxy
  {
    public ExceptionLogProxy() {}
    [DataMember] public int ExceptionLogID { get; set; }
    [DataMember] public string URL { get; set; }
    [DataMember] public string PageInfo { get; set; }
    [DataMember] public string ExceptionName { get; set; }
    [DataMember] public string Message { get; set; }
    [DataMember] public string StackTrace { get; set; }
    [DataMember] public string Browser { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
          
  }
  
  public partial class ExceptionLog : BaseItem
  {
    public ExceptionLogProxy GetProxy()
    {
      ExceptionLogProxy result = new ExceptionLogProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.Browser = this.Browser;
      result.StackTrace = this.StackTrace;
      result.Message = this.Message;
      result.ExceptionName = this.ExceptionName;
      result.PageInfo = this.PageInfo;
      result.URL = this.URL;
      result.ExceptionLogID = this.ExceptionLogID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
