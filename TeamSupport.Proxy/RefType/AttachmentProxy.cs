using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Data.Linq.Mapping;

namespace TeamSupport.Data
{

    [DataContract(Namespace = "http://teamsupport.com/")]
    [KnownType(typeof(AttachmentProxy))]
    [Table(Name = "Attachments")]
    [InheritanceMapping(Code = References.Actions, Type = typeof(ActionAttachmentProxy), IsDefault = true)]
    [InheritanceMapping(Code = References.Assets, Type = typeof(AssetAttachmentProxy))]
    [InheritanceMapping(Code = References.ChatAttachments, Type = typeof(ChatAttachmentProxy))]
    [InheritanceMapping(Code = References.CompanyActivity, Type = typeof(CompanyActivityAttachmentProxy))]
    [InheritanceMapping(Code = References.ContactActivity, Type = typeof(ContactActivityAttachmentProxy))]
    [InheritanceMapping(Code = References.Contacts, Type = typeof(ContactAttachmentProxy))]
    [InheritanceMapping(Code = References.CustomerHubLogo, Type = typeof(CustomerHubLogoAttachmentProxy))]
    [InheritanceMapping(Code = References.Organizations, Type = typeof(OrganizationAttachmentProxy))]
    [InheritanceMapping(Code = References.ProductVersions, Type = typeof(ProductVersionAttachmentProxy))]
    [InheritanceMapping(Code = References.Tasks, Type = typeof(TaskAttachmentProxy))]
    [InheritanceMapping(Code = References.UserPhoto, Type = typeof(UserPhotoAttachmentProxy))]
    [InheritanceMapping(Code = References.Users, Type = typeof(UserAttachmentProxy))]
    [InheritanceMapping(Code = References.WaterCooler, Type = typeof(WatercoolerMsgAttachmentProxy))]
    public abstract class AttachmentProxy
    {
        public AttachmentProxy() { }
        protected AttachmentProxy(References type)
        {
            RefType = type;
        }

        [DataMember, Column(DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int AttachmentID { get; set; }
        [DataMember, Column] public int OrganizationID { get; set; }
        [DataMember, Column] public string FileName { get; set; }
        [DataMember, Column] public string FileType { get; set; }
        [DataMember, Column] public long FileSize { get; set; }
        [DataMember, Column] public string Path { get; set; }
        [DataMember, Column] public string Description { get; set; }
        [DataMember, Column] public DateTime DateCreated { get; set; }
        [DataMember, Column] public DateTime DateModified { get; set; }
        [DataMember, Column] public int CreatorID { get; set; }
        [DataMember, Column] public int ModifierID { get; set; }
        [DataMember, Column(IsDiscriminator = true)] public References RefType { get; protected set; }
        [DataMember, Column] public int RefID { get; set; }
        [DataMember] public string CreatorName { get; set; }
        [DataMember, Column] public bool SentToJira { get; set; }
        [DataMember, Column] public int? ProductFamilyID { get; set; }
        [DataMember] public string ProductFamily { get; set; }
        [DataMember, Column] public bool SentToTFS { get; set; }
        [DataMember, Column] public bool SentToSnow { get; set; }
        [DataMember, Column] public int? FilePathID { get; set; }

        public const int AttachmentPathIndex = 3;

        public static AttachmentProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Actions: return new ActionAttachmentProxy();
                case References.Assets: return new AssetAttachmentProxy();
                case References.ChatAttachments: return new ChatAttachmentProxy();
                case References.CompanyActivity: return new CompanyActivityAttachmentProxy();
                case References.ContactActivity: return new ContactActivityAttachmentProxy();
                case References.Contacts: return new ContactAttachmentProxy();
                case References.CustomerHubLogo: return new CustomerHubLogoAttachmentProxy();
                case References.Organizations: return new OrganizationAttachmentProxy();
                case References.ProductVersions: return new ProductVersionAttachmentProxy();
                case References.Tasks: return new TaskAttachmentProxy();
                case References.UserPhoto: return new UserPhotoAttachmentProxy();
                case References.Users: return new UserAttachmentProxy();
                case References.WaterCooler: return new WatercoolerMsgAttachmentProxy();
                default:
                    if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break();
                    throw new Exception($"bad ReferenceType {refType}");
            }
        }

        public enum References
        {
            None = ReferenceType.None,
            Actions = ReferenceType.Actions,
            Assets = ReferenceType.Assets,
            ChatAttachments = ReferenceType.ChatAttachments,
            CompanyActivity = ReferenceType.CompanyActivity,
            ContactActivity = ReferenceType.ContactActivity,
            Contacts = ReferenceType.Contacts,
            CustomerHubLogo = ReferenceType.CustomerHubLogo,
            Organizations = ReferenceType.Organizations,
            ProductVersions = ReferenceType.ProductVersions,
            Tasks = ReferenceType.Tasks,
            UserPhoto = ReferenceType.UserPhoto,
            Users = ReferenceType.Users,
            WaterCooler = ReferenceType.WaterCooler,
        };

    }

    public class ActionAttachmentProxy : AttachmentProxy
    {
        public ActionAttachmentProxy() : base(References.Actions) { }
        public int ActionID { get { return RefID; } set { RefID = value; } }
    }

    public class OrganizationAttachmentProxy : AttachmentProxy
    {
        public OrganizationAttachmentProxy() : base(References.Organizations) { }
        //public int OrganizationID { get { return RefID; } }   // Hides base class OrganizationID?
    }

    public class ContactAttachmentProxy : AttachmentProxy
    {
        public ContactAttachmentProxy() : base(References.Contacts) { }
        public int UserID { get { return RefID; } }
    }

    public class WatercoolerMsgAttachmentProxy : AttachmentProxy
    {
        public WatercoolerMsgAttachmentProxy() : base(References.WaterCooler) { }
        public int MessageID { get { return RefID; } }
    }

    public class AssetAttachmentProxy : AttachmentProxy
    {
        public AssetAttachmentProxy() : base(References.Assets) { }
        public int AssetID { get { return RefID; } }
    }

    public class ChatAttachmentProxy : AttachmentProxy
    {
        public ChatAttachmentProxy() : base(References.ChatAttachments) { }
        public int ChatID { get { return RefID; } }
    }

    public class CompanyActivityAttachmentProxy : AttachmentProxy
    {
        public CompanyActivityAttachmentProxy() : base(References.CompanyActivity) { }
    }


    public class ContactActivityAttachmentProxy : AttachmentProxy
    {
        public ContactActivityAttachmentProxy() : base(References.ContactActivity) { }
    }


    public class CustomerHubLogoAttachmentProxy : AttachmentProxy
    {
        public CustomerHubLogoAttachmentProxy() : base(References.CustomerHubLogo) { }
    }


    public class ProductVersionAttachmentProxy : AttachmentProxy
    {
        public ProductVersionAttachmentProxy() : base(References.ProductVersions) { }
    }

    public class TaskAttachmentProxy : AttachmentProxy
    {
        public TaskAttachmentProxy() : base(References.Tasks) { }
        public int TaskID { get { return RefID; } }
    }


    public class UserPhotoAttachmentProxy : AttachmentProxy
    {
        public UserPhotoAttachmentProxy() : base(References.UserPhoto) { }
    }


    public class UserAttachmentProxy : AttachmentProxy
    {
        public UserAttachmentProxy() : base(References.Users) { }
    }

}
