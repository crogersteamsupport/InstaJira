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
  [KnownType(typeof(TicketAutomationTriggerProxy))]
  public class TicketAutomationTriggerProxy
  {
    public TicketAutomationTriggerProxy() {}
    [DataMember] public int TriggerID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public bool Active { get; set; }
    [DataMember] public int Position { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public bool UseCustomSQL { get; set; }
    [DataMember] public string CustomSQL { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public string LastSQLExecuted { get; set; }
          
  }
  
  public partial class TicketAutomationTrigger : BaseItem
  {
    public TicketAutomationTriggerProxy GetProxy()
    {
      TicketAutomationTriggerProxy result = new TicketAutomationTriggerProxy();
      result.LastSQLExecuted = this.LastSQLExecuted;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.CustomSQL = this.CustomSQL;
      result.UseCustomSQL = this.UseCustomSQL;
      result.OrganizationID = this.OrganizationID;
      result.Position = this.Position;
      result.Active = this.Active;
      result.Name = this.Name;
      result.TriggerID = this.TriggerID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
