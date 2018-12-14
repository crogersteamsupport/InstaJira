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
    [KnownType(typeof(AssetAssignmentsViewItemProxy))]
    public class AssetAssignmentsViewItemProxy
    {
        public AssetAssignmentsViewItemProxy() { }
        public AssetAssignmentsViewItemProxy(ReferenceType referenceType) { }
        [DataMember] public int AssetAssignmentsID { get; set; }
        [DataMember] public int HistoryID { get; set; }
        [DataMember] public int AssetID { get; set; }
        [DataMember] public int OrganizationID { get; set; }
        [DataMember] public DateTime? ActionTime { get; set; }
        [DataMember] public string ActionDescription { get; set; }
        [DataMember] public int? ShippedFrom { get; set; }
        [DataMember] public string NameAssignedFrom { get; set; }
        [DataMember] public int? ShippedTo { get; set; }
        [DataMember] public string NameAssignedTo { get; set; }
        [DataMember] public string TrackingNumber { get; set; }
        [DataMember] public string ShippingMethod { get; set; }
        [DataMember] public string ReferenceNum { get; set; }
        [DataMember] public string Comments { get; set; }
        [DataMember] public DateTime? DateCreated { get; set; }
        [DataMember] public int? Actor { get; set; }
        [DataMember] public string ActorName { get; set; }
    [DataMember] public int? RefType { get; set; }
        [DataMember] public DateTime? DateModified { get; set; }
        [DataMember] public int? ModifierID { get; set; }
        [DataMember] public string ModifierName { get; set; }
        [DataMember] public int? ShippedFromRefType { get; set; }

        public static AssetAssignmentsViewItemProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Organizations: return new OrganizationsAssetAssignmentsViewItemProxy();
                case References.Contacts: return new ContactsAssetAssignmentsViewItemProxy();
                default: throw new Exception("Invalid AssetAssignmentsViewItem Reference Type");
            }
        }
        public enum References
        {
            Organizations = ReferenceType.Organizations,
            Contacts = ReferenceType.Contacts,
        }

    }

    internal class OrganizationsAssetAssignmentsViewItemProxy : AssetAssignmentsViewItemProxy
    {
        public OrganizationsAssetAssignmentsViewItemProxy() : base(ReferenceType.Organizations)
        {
        }
    }
    internal class ContactsAssetAssignmentsViewItemProxy : AssetAssignmentsViewItemProxy
    {
        public ContactsAssetAssignmentsViewItemProxy() : base(ReferenceType.Contacts)
        {
        }
    }
}
