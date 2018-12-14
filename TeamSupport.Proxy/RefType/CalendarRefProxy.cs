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
  [KnownType(typeof(CalendarRefItemProxy))]
  public class CalendarRefItemProxy
  {
    public CalendarRefItemProxy() {}
    [DataMember] public int CalendarID { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public int RefType { get; set; }
    [DataMember] public string displayName { get; set; }      
  }
  
}
