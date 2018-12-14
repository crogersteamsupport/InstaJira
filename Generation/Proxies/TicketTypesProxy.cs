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
  [KnownType(typeof(TicketTypeProxy))]
  public class TicketTypeProxy
  {
    public TicketTypeProxy() {}
    [DataMember] public int TicketTypeID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public int Position { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string IconUrl { get; set; }
    [DataMember] public bool IsVisibleOnPortal { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public int? ProductFamilyID { get; set; }
    [DataMember] public bool IsActive { get; set; }
          
  }
  
  public partial class TicketType : BaseItem
  {
    public TicketTypeProxy GetProxy()
    {
      TicketTypeProxy result = new TicketTypeProxy();
      result.IsActive = this.IsActive;
      result.ProductFamilyID = this.ProductFamilyID;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.IconUrl = this.IconUrl;
      result.OrganizationID = this.OrganizationID;
      result.Position = this.Position;
      result.Description = this.Description;
      result.Name = this.Name;
      result.TicketTypeID = this.TicketTypeID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
