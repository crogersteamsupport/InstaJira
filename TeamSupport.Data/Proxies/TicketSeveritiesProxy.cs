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
  [KnownType(typeof(TicketSeverityProxy))]
  public class TicketSeverityProxy
  {
    public TicketSeverityProxy() {}
    [DataMember] public int TicketSeverityID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public int Position { get; set; }
    [DataMember] public bool VisibleOnPortal { get; set; }
    //[DataMember] public int OrganizationID { get; set; }
    //[DataMember] public DateTime DateCreated { get; set; }
    //[DataMember] public DateTime DateModified { get; set; }
    //[DataMember] public int CreatorID { get; set; }
    //[DataMember] public int ModifierID { get; set; }
  }
  
  public partial class TicketSeverity : BaseItem
  {
    public TicketSeverityProxy GetProxy()
    {
      TicketSeverityProxy result = new TicketSeverityProxy();
      //result.ModifierID = this.ModifierID;
      //result.CreatorID = this.CreatorID;
      //result.OrganizationID = this.OrganizationID;
      result.Position = this.Position;
      result.Description = (this.Description);
      result.Name = (this.Name);
      result.TicketSeverityID = this.TicketSeverityID;
      result.VisibleOnPortal = this.VisibleOnPortal;
       
       
      return result;
    }	
  }
}
