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
}
