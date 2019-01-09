using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.EFData.Models
{
    [Table("CrmLinkTable")]
    public class CrmLinkTable
    {
        [Key]
        public int CRMLinkID {get;set;}
        public int OrganizationID { get; set; }
        public bool Active { get; set; }
        public string CRMType { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SecurityToken { get; set; }
        public string TypeFieldMatch { get; set; }
        public DateTime? LastLink { get; set; }
        public bool SendBackTicketData { get; set; }
        public DateTime LastProcessed { get; set; }
        public int LastTicketID { get; set; }
        public bool AllowPortalAccess { get; set; }
        public bool SendWelcomeEmail { get; set; }
        public int? DefaultSlaLevelID { get; set; }
        public bool? PullCasesAsTickets { get; set; }
        public bool? PushTicketsAsCases { get; set; }
        public bool? PullCustomerProducts { get; set; }
        public bool? UpdateStatus { get; set; }
        public int? ActionTypeIDToPush { get; set; }
        public string HostName { get; set; }
        public string DefaultProject { get; set; }
        public bool MatchAccountsByName { get; set; }
        public bool UseSandBoxServer { get; set; }
        public bool AlwaysUseDefaultProjectKey { get; set; }
        public string RestrictedToTicketTypes { get; set; }
        public bool UpdateTicketType { get; set; }
        public string InstanceName { get; set; }
        public string ExcludedTicketStatusUpdate { get; set; }
        public bool IncludeIssueNonRequired { get; set; }
        public bool? UseNetworkCredentials { get; set; }
        public int? WebHookTokenId { get; set; }
    }
}
