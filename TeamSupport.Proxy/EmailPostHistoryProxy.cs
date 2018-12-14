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
  [KnownType(typeof(EmailPostHistoryItemProxy))]
  public class EmailPostHistoryItemProxy
  {
    public EmailPostHistoryItemProxy() {}
    [DataMember] public int EmailPostID { get; set; }
    [DataMember] public int EmailPostType { get; set; }
    [DataMember] public int HoldTime { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public string Param1 { get; set; }
    [DataMember] public string Param2 { get; set; }
    [DataMember] public string Param3 { get; set; }
    [DataMember] public string Param4 { get; set; }
    [DataMember] public string Param5 { get; set; }
    [DataMember] public string Param6 { get; set; }
    [DataMember] public string Param7 { get; set; }
    [DataMember] public string Param8 { get; set; }
    [DataMember] public string Param9 { get; set; }
    [DataMember] public string Param10 { get; set; }
    [DataMember] public string Text1 { get; set; }
    [DataMember] public string Text2 { get; set; }
    [DataMember] public string Text3 { get; set; }
          
  }
}
