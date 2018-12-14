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
  [KnownType(typeof(TechDocProxy))]
  public class TechDocProxy
  {
    public TechDocProxy() {}
    [DataMember] public int TechDocID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public int ProductID { get; set; }
    [DataMember] public int AttachmentID { get; set; }
    [DataMember] public bool IsVisibleOnPortal { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
          
  }
}
