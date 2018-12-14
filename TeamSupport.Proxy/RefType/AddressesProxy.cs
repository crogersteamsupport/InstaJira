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
    [KnownType(typeof(AddressProxy))]
    public class AddressProxy
    {
        public AddressProxy() { }
        public AddressProxy(ReferenceType referenceType) { }
        [DataMember] public int AddressID { get; set; }
        [DataMember] public int RefID { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
        [DataMember] public string Description { get; set; }
        [DataMember] public string Addr1 { get; set; }
        [DataMember] public string Addr2 { get; set; }
        [DataMember] public string Addr3 { get; set; }
        [DataMember] public string City { get; set; }
        [DataMember] public string State { get; set; }
        [DataMember] public string Zip { get; set; }
        [DataMember] public string Country { get; set; }
        [DataMember] public string Comment { get; set; }
        [DataMember] public DateTime DateCreated { get; set; }
        [DataMember] public DateTime DateModified { get; set; }
        [DataMember] public int CreatorID { get; set; }
        [DataMember] public int ModifierID { get; set; }
        [DataMember] public string MapLink { get; set; }
        [DataMember] public int? ImportFileID { get; set; }

        public static AddressProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Organizations: return new OrganizationsAddressProxy();
                case References.Users: return new UsersAddressProxy();
                case References.BillingInfo: return new BillingInfoAddressProxy();
                case References.Contacts: return new ContactsAddressProxy();
                default: throw new Exception("Invalid AddressProxy Reference Type");
            }
        }
        public enum References
        {
            Organizations = ReferenceType.Organizations,
            Contacts = ReferenceType.Contacts,
            Users = ReferenceType.Users,
            BillingInfo = ReferenceType.BillingInfo
        }

    }

    public class UsersAddressProxy : AddressProxy
    {
        public UsersAddressProxy() : base(ReferenceType.Users)
        {

        }
    }

    public class OrganizationsAddressProxy : AddressProxy
    {
        public OrganizationsAddressProxy() : base(ReferenceType.Organizations)
        {

        }
    }

    public class ContactsAddressProxy : AddressProxy
    {
        public ContactsAddressProxy() : base(ReferenceType.Contacts)
        {

        }
    }

    public class BillingInfoAddressProxy : AddressProxy
    {
        public BillingInfoAddressProxy() : base(ReferenceType.BillingInfo)
        {

        }
    }

}
