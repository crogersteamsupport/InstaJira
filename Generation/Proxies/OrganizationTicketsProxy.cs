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
  [KnownType(typeof(OrganizationTicketProxy))]
  public class OrganizationTicketProxy
  {
    public OrganizationTicketProxy() {}
    [DataMember] public int TicketID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public DateTime? DateModified { get; set; }
    [DataMember] public int? ModifierID { get; set; }
    [DataMember] public bool? SentToSalesForce { get; set; }
    [DataMember] public int? ImportFileID { get; set; }
          
  }
  
  public partial class OrganizationTicket : BaseItem
  {
    public OrganizationTicketProxy GetProxy()
    {
      OrganizationTicketProxy result = new OrganizationTicketProxy();
      result.ImportFileID = this.ImportFileID;
      result.SentToSalesForce = this.SentToSalesForce;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.OrganizationID = this.OrganizationID;
      result.TicketID = this.TicketID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
      result.DateModified = this.DateModifiedUtc == null ? this.DateModifiedUtc : DateTime.SpecifyKind((DateTime)this.DateModifiedUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
