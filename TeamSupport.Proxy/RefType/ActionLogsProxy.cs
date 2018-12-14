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
    [KnownType(typeof(ActionLogProxy))]
    public class ActionLogProxy
    {
        public ActionLogProxy() { }
        public ActionLogProxy(ReferenceType referenceType) { }
        [DataMember] public int ActionLogID { get; set; }
        [DataMember] public int? OrganizationID { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
        [DataMember] public int RefID { get; set; }
        [DataMember] public ActionLogType ActionLogType { get; set; }
        [DataMember] public string Description { get; set; }
        [DataMember] public DateTime DateCreated { get; set; }
        [DataMember] public DateTime DateModified { get; set; }
        [DataMember] public int CreatorID { get; set; }
        [DataMember] public int ModifierID { get; set; }
        [DataMember] public string CreatorName { get; set; }

        public static ActionLogProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Actions: return new ActionsActionLogProxy();
                case References.CustomFields: return new CustomFieldsActionLogProxy();
                case References.CustomValues: return new CustomValuesActionLogProxy();
                case References.Groups: return new GroupsActionLogProxy();
                case References.OrganizationProducts: return new OrganizationProductsActionLogProxy();
                case References.Organizations: return new OrganizationsActionLogProxy();
                case References.PhoneNumbers: return new PhoneNumbersActionLogProxy();
                case References.Products: return new ProductsActionLogProxy();
                case References.ProductVersions: return new ProductVersionsActionLogProxy();
                case References.Tickets: return new TicketsActionLogProxy();
                case References.Users: return new UsersActionLogProxy();
                case References.SystemSettings: return new SystemSettingsActionLogProxy();
                case References.Contacts: return new ContactsActionLogProxy();
                case References.Assets: return new AssetsActionLogProxy();
                case References.WaterCooler: return new WaterCoolerActionLogProxy();
                case References.Notes: return new NotesActionLogProxy();
                case References.Tags: return new TagsActionLogProxy();
                case References.Reports: return new ReportsActionLogProxy();
                case References.ProductFamilies: return new ProductFamiliesActionLogProxy();
                case References.UserProducts: return new UserProductsActionLogProxy();
                case References.AgentRating: return new AgentRatingActionLogProxy();
                case References.Sla: return new SlaActionLogProxy();
                case References.Tasks: return new TasksActionLogProxy();
                default: throw new Exception("Invalid ActionLog Reference Type");
            }
        }
        public enum References
        {
            Actions = ReferenceType.Actions,
            CustomFields = ReferenceType.CustomFields,
            CustomValues = ReferenceType.CustomValues,
            Groups = ReferenceType.Groups,
            OrganizationProducts = ReferenceType.OrganizationProducts,
            Organizations = ReferenceType.Organizations,
            PhoneNumbers = ReferenceType.PhoneNumbers,
            Products = ReferenceType.Products,
            ProductVersions = ReferenceType.ProductVersions,
            Tickets = ReferenceType.Tickets,
            Users = ReferenceType.Users,
            SystemSettings = ReferenceType.SystemSettings,
            Contacts = ReferenceType.Contacts,
            Assets = ReferenceType.Assets,
            WaterCooler = ReferenceType.WaterCooler,
            Notes = ReferenceType.Notes,
            Tags = ReferenceType.Tags,
            Reports = ReferenceType.Reports,
            ProductFamilies = ReferenceType.ProductFamilies,
            UserProducts = ReferenceType.UserProducts,
            AgentRating = ReferenceType.AgentRating,
            Sla = ReferenceType.Sla,
            Tasks = ReferenceType.Tasks,
        }
    }

    internal class TasksActionLogProxy : ActionLogProxy
    {
        public TasksActionLogProxy() : base(ReferenceType.Tasks)
        {
        }
    }

    internal class SlaActionLogProxy : ActionLogProxy
    {
        public SlaActionLogProxy() : base(ReferenceType.Sla)
        {
        }
    }

    internal class AgentRatingActionLogProxy : ActionLogProxy
    {
        public AgentRatingActionLogProxy() : base(ReferenceType.AgentRating)
        {
        }
    }

    internal class UserProductsActionLogProxy : ActionLogProxy
    {
        public UserProductsActionLogProxy() : base(ReferenceType.UserProducts)
        {
        }
    }

    internal class ProductFamiliesActionLogProxy : ActionLogProxy
    {
        public ProductFamiliesActionLogProxy() : base(ReferenceType.ProductFamilies)
        {
        }
    }

    internal class ReportsActionLogProxy : ActionLogProxy
    {
        public ReportsActionLogProxy() : base(ReferenceType.Reports)
        {
        }
    }

    internal class TagsActionLogProxy : ActionLogProxy
    {
        public TagsActionLogProxy() : base(ReferenceType.Tags)
        {
        }
    }

    internal class NotesActionLogProxy : ActionLogProxy
    {
        public NotesActionLogProxy() : base(ReferenceType.Notes)
        {
        }
    }

    internal class WaterCoolerActionLogProxy : ActionLogProxy
    {
        public WaterCoolerActionLogProxy() : base(ReferenceType.WaterCooler)
        {
        }
    }

    internal class AssetsActionLogProxy : ActionLogProxy
    {
        public AssetsActionLogProxy() : base(ReferenceType.Assets)
        {
        }
    }

    internal class ContactsActionLogProxy : ActionLogProxy
    {
        public ContactsActionLogProxy() : base(ReferenceType.Contacts)
        {
        }
    }

    internal class SystemSettingsActionLogProxy : ActionLogProxy
    {
        public SystemSettingsActionLogProxy() : base(ReferenceType.SystemSettings)
        {
        }
    }

    internal class UsersActionLogProxy : ActionLogProxy
    {
        public UsersActionLogProxy() : base(ReferenceType.Users)
        {
        }
    }

    internal class TicketsActionLogProxy : ActionLogProxy
    {
        public TicketsActionLogProxy() : base(ReferenceType.Tickets)
        {
        }
    }

    internal class ProductVersionsActionLogProxy : ActionLogProxy
    {
        public ProductVersionsActionLogProxy() : base(ReferenceType.ProductVersions)
        {
        }
    }

    internal class ProductsActionLogProxy : ActionLogProxy
    {
        public ProductsActionLogProxy() : base(ReferenceType.Products)
        {
        }
    }

    internal class PhoneNumbersActionLogProxy : ActionLogProxy
    {
        public PhoneNumbersActionLogProxy() : base(ReferenceType.PhoneNumbers)
        {
        }
    }

    internal class OrganizationsActionLogProxy : ActionLogProxy
    {
        public OrganizationsActionLogProxy() : base(ReferenceType.Organizations)
        {
        }
    }

    internal class OrganizationProductsActionLogProxy : ActionLogProxy
    {
        public OrganizationProductsActionLogProxy() : base(ReferenceType.OrganizationProducts)
        {
        }
    }

    internal class GroupsActionLogProxy : ActionLogProxy
    {
        public GroupsActionLogProxy() : base(ReferenceType.Groups)
        {
        }
    }

    internal class CustomValuesActionLogProxy : ActionLogProxy
    {
        public CustomValuesActionLogProxy() : base(ReferenceType.CustomValues)
        {
        }
    }

    internal class CustomFieldsActionLogProxy : ActionLogProxy
    {
        public CustomFieldsActionLogProxy() : base(ReferenceType.CustomFields)
        {
        }
    }

    internal class ActionsActionLogProxy : ActionLogProxy
    {
        public ActionsActionLogProxy() : base(ReferenceType.Actions)
        {
        }
    }

}
