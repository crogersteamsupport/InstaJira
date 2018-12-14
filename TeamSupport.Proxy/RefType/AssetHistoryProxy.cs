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
    [KnownType(typeof(AssetHistoryItemProxy))]
    public class AssetHistoryItemProxy
    {
        public AssetHistoryItemProxy() { }
        public AssetHistoryItemProxy(ReferenceType referenceType) { }
        [DataMember] public int HistoryID { get; set; }
        [DataMember] public int AssetID { get; set; }
        [DataMember] public int OrganizationID { get; set; }
        [DataMember] public DateTime? ActionTime { get; set; }
        [DataMember] public string ActionDescription { get; set; }
        [DataMember] public int? ShippedFrom { get; set; }
        [DataMember] public int? ShippedTo { get; set; }
        [DataMember] public string TrackingNumber { get; set; }
        [DataMember] public string ShippingMethod { get; set; }
        [DataMember] public string ReferenceNum { get; set; }
        [DataMember] public string Comments { get; set; }
        [DataMember] public DateTime? DateCreated { get; set; }
        [DataMember] public int? Actor { get; set; }
    [DataMember] public int? RefType { get; set; }
        [DataMember] public DateTime? DateModified { get; set; }
        [DataMember] public int? ModifierID { get; set; }
        [DataMember] public int? ShippedFromRefType { get; set; }
        [DataMember] public int? ImportFileID { get; set; }

        public static AssetHistoryItemProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Contacts:
                    return new ContactsAssetHistoryItemProxy();
                case References.Organizations:
                    return new OrganizationsAssetHistoryItemProxy();
                default: throw new Exception("Invalid AssetHistory Reference Type");
            }
        }
        public enum References
        {
            Organizations = ReferenceType.Organizations,
            Contacts = ReferenceType.Contacts
        }

    }

    public class OrganizationsAssetHistoryItemProxy : AssetHistoryItemProxy
    {
        public OrganizationsAssetHistoryItemProxy() : base(ReferenceType.Organizations)
        {

        }
    }

    public class ContactsAssetHistoryItemProxy : AssetHistoryItemProxy
    {
        public ContactsAssetHistoryItemProxy() : base(ReferenceType.Contacts)
        {

        }
    }

}
