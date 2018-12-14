using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(CRMLinkTableItemProxy))]
  public class CRMLinkTableItemProxy
  {
    public CRMLinkTableItemProxy() {}
    [DataMember] public int CRMLinkID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public bool Active { get; set; }
    [DataMember] public string CRMType { get; set; }
    [DataMember] public string Username { get; set; }
    [DataMember] public string Password { get; set; }
    [DataMember] public string SecurityToken1 { get; set; }
    [DataMember] public string TypeFieldMatch { get; set; }
    [DataMember] public DateTime? LastLink { get; set; }
    [DataMember] public bool SendBackTicketData { get; set; }
    [DataMember] public DateTime LastProcessed { get; set; }
    [DataMember] public int LastTicketID { get; set; }
    [DataMember] public bool AllowPortalAccess { get; set; }
    [DataMember] public bool SendWelcomeEmail { get; set; }
    [DataMember] public int? DefaultSlaLevelID { get; set; }
    [DataMember] public bool? PullCasesAsTickets { get; set; }
    [DataMember] public bool? PushTicketsAsCases { get; set; }
    [DataMember] public bool? PullCustomerProducts { get; set; }
    [DataMember] public bool? UpdateStatus { get; set; }
    [DataMember] public int? ActionTypeIDToPush { get; set; }
    [DataMember] public string HostName { get; set; }
    [DataMember] public string DefaultProject { get; set; }
    [DataMember] public bool MatchAccountsByName { get; set; }
    [DataMember] public bool UseSandBoxServer { get; set; }
    [DataMember] public bool AlwaysUseDefaultProjectKey { get; set; }
    [DataMember] public string RestrictedToTicketTypes { get; set; }
    [DataMember] public bool UpdateTicketType { get; set; }
    [DataMember] public string InstanceName { get; set; }
    [DataMember] public string ExcludedTicketStatusUpdate { get; set; }
    [DataMember] public bool IncludeIssueNonRequired { get; set; }
    [DataMember] public bool UseNetworkCredentials { get; set; }
    [DataMember] public int? WebHookTokenId { get; set; }
	[DataMember] public string WebHookTokenFullUrl { get; set; }
	}
}
