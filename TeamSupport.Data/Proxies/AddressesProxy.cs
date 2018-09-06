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
    public abstract class AddressProxy
    {
        public AddressProxy(ReferenceType referenceType) { }
        [DataMember] public int AddressID { get; set; }
        [DataMember] public int RefID { get; set; }
        [DataMember] protected References RefType { get; set; }
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

    public partial class Address : BaseItem
    {
        public AddressProxy GetProxy()
        {
            AddressProxy result = AddressProxy.ClassFactory((AddressProxy.References)this.RefType);
            result.ImportFileID = this.ImportFileID;
            result.ModifierID = this.ModifierID;
            result.CreatorID = this.CreatorID;
            result.Comment = this.Comment;
            result.Country = this.Country;
            result.Zip = this.Zip;
            result.State = this.State;
            result.City = this.City;
            result.Addr3 = this.Addr3;
            result.Addr2 = this.Addr2;
            result.Addr1 = this.Addr1;
            result.Description = this.Description;
            result.RefID = this.RefID;
            result.AddressID = this.AddressID;

            result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
            result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);

            string link = "http://maps.google.com/maps?q={0}";
            StringBuilder builder = new StringBuilder();
            if (!String.IsNullOrEmpty(this.Addr1))
            {
                builder.Append(this.Addr1.Replace(' ', '+'));
            }

            if (!String.IsNullOrEmpty(this.Addr2))
            {
                if (builder.Length > 0) builder.Append("+");
                builder.Append(this.Addr2.Replace(' ', '+'));
            }

            if (!String.IsNullOrEmpty(this.Addr3))
            {
                if (builder.Length > 0) builder.Append("+");
                builder.Append(this.Addr3.Replace(' ', '+'));
            }

            if (!String.IsNullOrEmpty(this.City))
            {
                if (builder.Length > 0) builder.Append(",");
                builder.Append(this.City.Replace(' ', '+'));
            }

            if (!String.IsNullOrEmpty(this.State))
            {
                if (builder.Length > 0) builder.Append(",");
                builder.Append(this.State.Replace(' ', '+'));
            }

            if (!String.IsNullOrEmpty(this.Country))
            {
                if (builder.Length > 0) builder.Append(",");
                builder.Append(this.Country.Replace(' ', '+'));
            }

            link = String.Format(link, builder.ToString());
            result.MapLink = link;
            return result;
        }
    }
}
