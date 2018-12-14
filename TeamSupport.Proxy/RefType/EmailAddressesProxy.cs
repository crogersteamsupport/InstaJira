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
    [KnownType(typeof(EmailAddressProxy))]
    public class EmailAddressProxy
    {
        public EmailAddressProxy() { }
        public EmailAddressProxy(ReferenceType referenceType) { }
        [DataMember] public int Id { get; set; }
        [DataMember] public int RefID { get; set; }
    [DataMember] public int RefType { get; set; }
        [DataMember] public string Email { get; set; }
        [DataMember] public DateTime DateCreated { get; set; }
        [DataMember] public DateTime DateModified { get; set; }
        [DataMember] public int CreatorID { get; set; }
        [DataMember] public int ModifierID { get; set; }
        [DataMember] public int? ImportFileID { get; set; }

        public static EmailAddressProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Users:
                    return new UserEmailAddressProxy();
                default: throw new Exception("Invalid EmailAddressProxy Reference Type");
            }
        }
        public enum References
        {
            Users = ReferenceType.Users
        }
    }

    public class UserEmailAddressProxy : EmailAddressProxy
    {
        public UserEmailAddressProxy() : base(ReferenceType.Users)
        {

        }
    }

}
