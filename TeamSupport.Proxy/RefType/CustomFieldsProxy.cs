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
    [KnownType(typeof(CustomFieldProxy))]
    public class CustomFieldProxy
    {
        public CustomFieldProxy() { }
        public CustomFieldProxy(ReferenceType referenceType) { }
        [DataMember] public int CustomFieldID { get; set; }
        [DataMember] public int OrganizationID { get; set; }
        [DataMember] public int? CustomFieldCategoryID { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public string ApiFieldName { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
        [DataMember] public CustomFieldType FieldType { get; set; }
        [DataMember] public int AuxID { get; set; }
        [DataMember] public int Position { get; set; }
        [DataMember] public string ListValues { get; set; }
        [DataMember] public string Description { get; set; }
        [DataMember] public bool? IsVisibleOnPortal { get; set; }
        [DataMember] public bool IsFirstIndexSelect { get; set; }
        [DataMember] public bool IsRequired { get; set; }
        [DataMember] public bool IsRequiredToClose { get; set; }
        [DataMember] public string Mask { get; set; }
        [DataMember] public int? ParentCustomFieldID { get; set; }
        [DataMember] public string ParentCustomValue { get; set; }
        [DataMember] public int? ParentProductID { get; set; }

        public static CustomFieldProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.UserProducts: return new UserProductsCustomFieldProxy();
                case References.Organizations: return new OrganizationsCustomFieldProxy();
                case References.Contacts: return new ContactsCustomFieldProxy();
                case References.ProductFamilies: return new ProductFamiliesCustomFieldProxy();
                case References.Products: return new ProductsCustomFieldProxy();
                case References.Users: return new UsersCustomFieldProxy();
                case References.Tickets: return new TicketsCustomFieldProxy();
                case References.Assets: return new AssetsCustomFieldProxy();
                case References.ProductVersions: return new ProductVersionsCustomFieldProxy();
                case References.OrganizationProducts: return new OrganizationProductsCustomFieldProxy();
                default: throw new Exception("Invalid CustomFieldProxy Reference Type");
            }
        }
        public enum References
        {
            UserProducts = ReferenceType.UserProducts,
            Organizations = ReferenceType.Organizations,
            Contacts = ReferenceType.Contacts,
            ProductFamilies = ReferenceType.ProductFamilies,
            Products = ReferenceType.Products,
            Users = ReferenceType.Users,
            Tickets = ReferenceType.Tickets,
            Assets = ReferenceType.Assets,
            ProductVersions = ReferenceType.ProductVersions,
            OrganizationProducts = ReferenceType.OrganizationProducts,
        }

    }

    internal class OrganizationProductsCustomFieldProxy : CustomFieldProxy
    {
        public OrganizationProductsCustomFieldProxy() : base(ReferenceType.OrganizationProducts)
        {
        }
    }

    internal class ProductVersionsCustomFieldProxy : CustomFieldProxy
    {
        public ProductVersionsCustomFieldProxy() : base(ReferenceType.ProductVersions)
        {
        }
    }

    internal class AssetsCustomFieldProxy : CustomFieldProxy
    {
        public AssetsCustomFieldProxy() : base(ReferenceType.Assets)
        {
        }
    }

    internal class TicketsCustomFieldProxy : CustomFieldProxy
    {
        public TicketsCustomFieldProxy() : base(ReferenceType.Tickets)
        {
        }
    }

    internal class UsersCustomFieldProxy : CustomFieldProxy
    {
        public UsersCustomFieldProxy() : base(ReferenceType.Users)
        {
        }
    }

    internal class ProductsCustomFieldProxy : CustomFieldProxy
    {
        public ProductsCustomFieldProxy() : base(ReferenceType.Products)
        {
        }
    }

    internal class ProductFamiliesCustomFieldProxy : CustomFieldProxy
    {
        public ProductFamiliesCustomFieldProxy() : base(ReferenceType.ProductFamilies)
        {
        }
    }

    internal class ContactsCustomFieldProxy : CustomFieldProxy
    {
        public ContactsCustomFieldProxy() : base(ReferenceType.Contacts)
        {
        }
    }

    internal class OrganizationsCustomFieldProxy : CustomFieldProxy
    {
        public OrganizationsCustomFieldProxy() : base(ReferenceType.Organizations)
        {
        }
    }

    internal class UserProductsCustomFieldProxy : CustomFieldProxy
    {
        public UserProductsCustomFieldProxy() : base(ReferenceType.UserProducts)
        {
        }
    }

}
