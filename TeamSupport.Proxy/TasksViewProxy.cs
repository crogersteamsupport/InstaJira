using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
    [DataContract(Namespace = "http://teamsupport.com/")]
    [KnownType(typeof(TasksViewItemProxy))]
    public class TasksViewItemProxy
    {
        public TasksViewItemProxy() { }
        [DataMember]
        public int TaskID { get; set; }
        [DataMember]
        public int OrganizationID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public DateTime? DueDate { get; set; }
        [DataMember]
        public int? UserID { get; set; }
        [DataMember]
        public bool IsComplete { get; set; }
        [DataMember]
        public DateTime? DateCompleted { get; set; }
        [DataMember]
        public string CompletionComment { get; set; }
        [DataMember]
        public int? ParentID { get; set; }
        [DataMember]
        public bool? IsDismissed { get; set; }
        [DataMember]
        public bool? HasEmailSent { get; set; }
        [DataMember]
        public DateTime? ReminderDueDate { get; set; }
        [DataMember]
        public string TaskParentName { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Creator { get; set; }
        [DataMember]
        public int CreatorID { get; set; }
        [DataMember]
        public DateTime DateCreated { get; set; }
        [DataMember]
        public int ModifierID { get; set; }
        [DataMember]
        public DateTime DateModified { get; set; }
        [DataMember]
        public bool NeedsIndexing { get; set; }

    }
}
