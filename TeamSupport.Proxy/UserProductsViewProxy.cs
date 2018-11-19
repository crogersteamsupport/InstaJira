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
  [KnownType(typeof(UserProductsViewItemProxy))]
  public class UserProductsViewItemProxy
  {
    public UserProductsViewItemProxy() {}
    [DataMember] public string Product { get; set; }
    [DataMember] public string VersionStatus { get; set; }
    [DataMember] public bool? IsShipping { get; set; }
    [DataMember] public bool? IsDiscontinued { get; set; }
    [DataMember] public string VersionNumber { get; set; }
    [DataMember] public int? ProductVersionStatusID { get; set; }
    [DataMember] public DateTime? ReleaseDate { get; set; }
    [DataMember] public bool? IsReleased { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public int UserProductID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public string UserName { get; set; }
    [DataMember] public int ProductID { get; set; }
    [DataMember] public int? ProductVersionID { get; set; }
    [DataMember] public bool IsVisibleOnPortal { get; set; }
    [DataMember] public DateTime? SupportExpiration { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
          
  }

}