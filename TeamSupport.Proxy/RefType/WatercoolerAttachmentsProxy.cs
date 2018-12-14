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
    [KnownType(typeof(WatercoolerAttachmentProxy))]
    public class WatercoolerAttachmentProxy
    {
        public WatercoolerAttachmentProxy() { }
        public WatercoolerAttachmentProxy(ReferenceType referenceType) { }
        [DataMember]
        public int MessageID { get; set; }
        [DataMember]
        public int AttachmentID { get; set; }
        [DataMember]
        public WaterCoolerAttachmentType RefType { get; set; }
        [DataMember]
        public int CreatorID { get; set; }
        [DataMember]
        public string CreatorName { get; set; }
        [DataMember]
        public DateTime DateCreated { get; set; }
        [DataMember]
        public string GroupName { get; set; }
        [DataMember]
        public string TicketName { get; set; }
        [DataMember]
        public string ProductName { get; set; }
        [DataMember]
        public string CompanyName { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public ReferenceType ActivityRefType { get; set; }
        [DataMember]
        public string ActivityTitle { get; set; }
        [DataMember]
        public int ActivityRefID { get; set; }

        public static WatercoolerAttachmentProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Ticket: return new TicketWatercoolerAttachmentProxy();
                case References.Product: return new ProductWatercoolerAttachmentProxy();
                case References.Company: return new CompanyWatercoolerAttachmentProxy();
                case References.User: return new UserWatercoolerAttachmentProxy();
                case References.Group: return new GroupWatercoolerAttachmentProxy();
                case References.Activities: return new ActivitiesWatercoolerAttachmentProxy();


                default: throw new Exception("Invalid WatercoolerAttachment Reference Type");
            }
        }

        public enum References
        {
            Ticket = WaterCoolerAttachmentType.Ticket,
            Product = WaterCoolerAttachmentType.Product,
            Company = WaterCoolerAttachmentType.Company,
            User = WaterCoolerAttachmentType.User,
            Group = WaterCoolerAttachmentType.Group,
            Activities = WaterCoolerAttachmentType.Activities
        }

    }

    public class TicketWatercoolerAttachmentProxy : WatercoolerAttachmentProxy
    {
        public TicketWatercoolerAttachmentProxy() : base(ReferenceType.Users)
        {
        }
    }
    public class ProductWatercoolerAttachmentProxy : WatercoolerAttachmentProxy
    {
        public ProductWatercoolerAttachmentProxy() : base(ReferenceType.Users)
        {
        }
    }
    public class CompanyWatercoolerAttachmentProxy : WatercoolerAttachmentProxy
    {
        public CompanyWatercoolerAttachmentProxy() : base(ReferenceType.Users)
        {
        }
    }
    public class UserWatercoolerAttachmentProxy : WatercoolerAttachmentProxy
    {
        public UserWatercoolerAttachmentProxy() : base(ReferenceType.Users)
        {
        }
    }
    public class GroupWatercoolerAttachmentProxy : WatercoolerAttachmentProxy
    {
        public GroupWatercoolerAttachmentProxy() : base(ReferenceType.Users)
        {
        }
    }
    public class ActivitiesWatercoolerAttachmentProxy : WatercoolerAttachmentProxy
    {
        public ActivitiesWatercoolerAttachmentProxy() : base(ReferenceType.Users)
        {
        }
    }
}
