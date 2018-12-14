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
  [KnownType(typeof(TaskProxy))]
  public class TaskProxy
  {
    public TaskProxy() {}
    [DataMember] public int TaskID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public DateTime? DueDate { get; set; }
    [DataMember] public int? UserID { get; set; }
    [DataMember] public bool IsComplete { get; set; }
    [DataMember] public DateTime? DateCompleted { get; set; }
    [DataMember] public int? ParentID { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int? ReminderID { get; set; }
    [DataMember] public bool NeedsIndexing { get; set; }
    [DataMember] public string CompletionComment { get; set; }
          
  }
  
  public partial class Task : BaseItem
  {
    public TaskProxy GetProxy()
    {
      TaskProxy result = new TaskProxy();
      result.CompletionComment = this.CompletionComment;
      result.NeedsIndexing = this.NeedsIndexing;
      result.ReminderID = this.ReminderID;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.ParentID = this.ParentID;
      result.IsComplete = this.IsComplete;
      result.UserID = this.UserID;
      result.Description = this.Description;
      result.Name = this.Name;
      result.OrganizationID = this.OrganizationID;
      result.TaskID = this.TaskID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
      result.DateCompleted = this.DateCompletedUtc == null ? this.DateCompletedUtc : DateTime.SpecifyKind((DateTime)this.DateCompletedUtc, DateTimeKind.Utc); 
      result.DueDate = this.DueDateUtc == null ? this.DueDateUtc : DateTime.SpecifyKind((DateTime)this.DueDateUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
