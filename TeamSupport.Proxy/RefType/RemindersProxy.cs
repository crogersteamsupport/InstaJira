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
  [KnownType(typeof(ReminderProxy))]
  public class ReminderProxy
  {
    public ReminderProxy() { }
    public ReminderProxy(References refType) { RefType = (ReferenceType)refType; }
    [DataMember] public int ReminderID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public DateTime DueDate { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public bool IsDismissed { get; set; }
    [DataMember] public bool HasEmailSent { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }

        public enum References
        {
            Contacts,
            Organizations,
            Tasks,
            Tickets,
            Users
        }

        public static ReminderProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Contacts: return new ContactReminderProxy();
                case References.Organizations: return new OrganizationReminderProxy();
                case References.Tasks: return new TaskReminderProxy();
                case References.Tickets: return new TicketReminderProxy();
                case References.Users: return new UserReminderProxy();
                default:
                    if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break();
                    throw new Exception("Invalid ActionLog Reference Type");
            }
        }

        public class TicketReminderProxy : ReminderProxy
        {
            public TicketReminderProxy() : base(References.Tickets) { }
        }

        public class TaskReminderProxy : ReminderProxy
        {
            public TaskReminderProxy() : base(References.Tasks) { }
        }

        public class ContactReminderProxy : ReminderProxy
        {
            public ContactReminderProxy() : base(References.Contacts) { }
        }

        public class OrganizationReminderProxy : ReminderProxy
        {
            public OrganizationReminderProxy() : base(References.Organizations) { }
        }

        public class UserReminderProxy : ReminderProxy
        {
            public UserReminderProxy() : base(References.Users) { }
        }

          
  }
  
}
