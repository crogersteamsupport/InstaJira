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
  [KnownType(typeof(TicketTemplateProxy))]
  public class TicketTemplateProxy
  {
    public TicketTemplateProxy() {}
    [DataMember] public int TicketTemplateID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public TicketTemplateType TemplateType { get; set; }
    [DataMember] public bool IsEnabled { get; set; }
    [DataMember] public bool IsVisibleOnPortal { get; set; }
    [DataMember] public int? TicketTypeID { get; set; }
    [DataMember] public string TriggerText { get; set; }
    [DataMember] public string TemplateText { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
          
  }
  
  public partial class TicketTemplate : BaseItem
  {
    public TicketTemplateProxy GetProxy()
    {
      TicketTemplateProxy result = new TicketTemplateProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.TemplateText = this.TemplateText;
      result.TriggerText = this.TriggerText;
      result.TicketTypeID = this.TicketTypeID;
      result.IsEnabled = this.IsEnabled;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.TemplateType = this.TemplateType;
      result.OrganizationID = this.OrganizationID;
      result.TicketTemplateID = this.TicketTemplateID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
