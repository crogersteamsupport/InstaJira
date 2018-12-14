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
  [KnownType(typeof(TaskEmailPostProxy))]
  public class TaskEmailPostProxy
  {
    public TaskEmailPostProxy() {}
    [DataMember] public int TaskEmailPostID { get; set; }
    [DataMember] public int TaskEmailPostType { get; set; }
    [DataMember] public int HoldTime { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public int TaskID { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public string LockProcessID { get; set; }
    [DataMember] public int? OldUserID { get; set; }
          
  }
  
  public partial class TaskEmailPost : BaseItem
  {
    public TaskEmailPostProxy GetProxy()
    {
      TaskEmailPostProxy result = new TaskEmailPostProxy();
      result.OldUserID = this.OldUserID;
      result.LockProcessID = this.LockProcessID;
      result.CreatorID = this.CreatorID;
      result.TaskID = this.TaskID;
      result.HoldTime = this.HoldTime;
      result.TaskEmailPostType = this.TaskEmailPostType;
      result.TaskEmailPostID = this.TaskEmailPostID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
