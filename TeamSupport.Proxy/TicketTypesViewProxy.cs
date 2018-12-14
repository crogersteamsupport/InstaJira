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
  [KnownType(typeof(TicketTypesViewItemProxy))]
  public class TicketTypesViewItemProxy
  {
    public TicketTypesViewItemProxy() {}
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
    [DataMember] public string ProductFamilyName { get; set; }
    [DataMember] public bool IsActive { get; set; }
    [DataMember] public bool ExcludeFromCDI { get; set; }
        

    }
}
