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
    [KnownType(typeof(UserNoteSettingProxy))]
  public class UserNoteSettingProxy
    {
        public UserNoteSettingProxy() { }
        public UserNoteSettingProxy(References refType) : this((ReferenceType) refType) { }
        public UserNoteSettingProxy(ReferenceType referenceType) { }
        [DataMember] public int UserID { get; set; }
        [DataMember] public int RefID { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
        [DataMember] public bool IsDismissed { get; set; }
        [DataMember] public bool IsSnoozed { get; set; }
        [DataMember] public DateTime SnoozeTime { get; set; }

        public static UserNoteSettingProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Users:
                    return new UserUserNoteSetting();
                case References.Organizations:
                    return new OrganizationsUserNoteSetting();
                default: throw new Exception("Invalid UserNoteSetting Reference Type");
            }
        }
        public enum References
        {
            Organizations = ReferenceType.Organizations,
            Contacts = ReferenceType.Contacts,
            Users = ReferenceType.Users
        }
    }

    public class UserUserNoteSetting : UserNoteSettingProxy
    {
        public UserUserNoteSetting() : base(References.Users)
        {

        }
    }

    public class OrganizationsUserNoteSetting : UserNoteSettingProxy
    {
        public OrganizationsUserNoteSetting() : base(References.Organizations)
        {

        }
    }
}
