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
    [DataMember] public string SecurityToken { get; set; }
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
          
  }
  
  public partial class CRMLinkTableItem : BaseItem
  {
    public CRMLinkTableItemProxy GetProxy()
    {
      CRMLinkTableItemProxy result = new CRMLinkTableItemProxy();
      result.WebHookTokenId = this.WebHookTokenId;
      result.UseNetworkCredentials = this.UseNetworkCredentials;
      result.IncludeIssueNonRequired = this.IncludeIssueNonRequired;
      result.ExcludedTicketStatusUpdate = this.ExcludedTicketStatusUpdate;
      result.InstanceName = this.InstanceName;
      result.UpdateTicketType = this.UpdateTicketType;
      result.RestrictedToTicketTypes = this.RestrictedToTicketTypes;
      result.AlwaysUseDefaultProjectKey = this.AlwaysUseDefaultProjectKey;
      result.UseSandBoxServer = this.UseSandBoxServer;
      result.MatchAccountsByName = this.MatchAccountsByName;
      result.DefaultProject = this.DefaultProject;
      result.HostName = this.HostName;
      result.ActionTypeIDToPush = this.ActionTypeIDToPush;
      result.UpdateStatus = this.UpdateStatus;
      result.PullCustomerProducts = this.PullCustomerProducts;
      result.PushTicketsAsCases = this.PushTicketsAsCases;
      result.PullCasesAsTickets = this.PullCasesAsTickets;
      result.DefaultSlaLevelID = this.DefaultSlaLevelID;
      result.SendWelcomeEmail = this.SendWelcomeEmail;
      result.AllowPortalAccess = this.AllowPortalAccess;
      result.LastTicketID = this.LastTicketID;
      result.SendBackTicketData = this.SendBackTicketData;
      result.TypeFieldMatch = this.TypeFieldMatch;
      result.SecurityToken = this.SecurityToken;
      result.Password = this.Password;
      result.Username = this.Username;
      result.CRMType = this.CRMType;
      result.Active = this.Active;
      result.OrganizationID = this.OrganizationID;
      result.CRMLinkID = this.CRMLinkID;
       
      result.LastProcessed = DateTime.SpecifyKind(this.LastProcessedUtc, DateTimeKind.Utc);
       
      result.LastLink = this.LastLinkUtc == null ? this.LastLinkUtc : DateTime.SpecifyKind((DateTime)this.LastLinkUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
