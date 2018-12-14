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
  [KnownType(typeof(TaskEmailPostHistoryItemProxy))]
  public class TaskEmailPostHistoryItemProxy
  {
    public TaskEmailPostHistoryItemProxy() {}
    [DataMember] public int TaskEmailPostID { get; set; }
    [DataMember] public int TaskEmailPostType { get; set; }
    [DataMember] public int HoldTime { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public int TaskID { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public string LockProcessID { get; set; }
    [DataMember] public int? OldUserID { get; set; }
          
  }
}
