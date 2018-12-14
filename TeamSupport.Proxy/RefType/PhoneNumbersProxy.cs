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
    [KnownType(typeof(PhoneNumberProxy))]
    public class PhoneNumberProxy
    {
        public PhoneNumberProxy() { }
        public PhoneNumberProxy(ReferenceType referenceType) { }
        [DataMember] public int PhoneID { get; set; }
        [DataMember] public int? PhoneTypeID { get; set; }
        [DataMember] public int RefID { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
        [DataMember] public string Number { get; set; }
        [DataMember] public string Extension { get; set; }
        [DataMember] public string OtherTypeName { get; set; }
        [DataMember] public DateTime DateCreated { get; set; }
        [DataMember] public DateTime DateModified { get; set; }
        [DataMember] public int CreatorID { get; set; }
        [DataMember] public int ModifierID { get; set; }
        [DataMember] public string PhoneTypeName { get; set; }
        [DataMember] public int? ImportFileID { get; set; }

        public static PhoneNumberProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Users:
                    return new UserPhoneNumberProxy();
                case References.Contacts:
                    return new ContactsPhoneNumberProxy();
                case References.Organizations:
                    return new OrganizationsPhoneNumberProxy();
                default: throw new Exception("Invalid CustomFieldCategory Reference Type");
            }
        }
        public enum References
        {
            Organizations = ReferenceType.Organizations,
            Contacts = ReferenceType.Contacts,
            Users = ReferenceType.Users
        }
    }

    public class UserPhoneNumberProxy : PhoneNumberProxy
    {
        public UserPhoneNumberProxy() : base(ReferenceType.Users)
        {

        }
    }

    public class OrganizationsPhoneNumberProxy : PhoneNumberProxy
    {
        public OrganizationsPhoneNumberProxy() : base(ReferenceType.Organizations)
        {

        }
    }

    public class ContactsPhoneNumberProxy : PhoneNumberProxy
    {
        public ContactsPhoneNumberProxy() : base(ReferenceType.Contacts)
        {

        }
    }

}
