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
  [KnownType(typeof(AssetProxy))]
  public class AssetProxy
  {
    public AssetProxy() {}
    [DataMember] public int AssetID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string SerialNumber { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Location { get; set; }
    [DataMember] public string Notes { get; set; }
    [DataMember] public int? ProductID { get; set; }
    [DataMember] public DateTime? WarrantyExpiration { get; set; }
    [DataMember] public int? AssignedTo { get; set; }
    [DataMember] public DateTime? DateCreated { get; set; }
    [DataMember] public DateTime? DateModified { get; set; }
    [DataMember] public int? CreatorID { get; set; }
    [DataMember] public int? ModifierID { get; set; }
    [DataMember] public int? SubPartOf { get; set; }
    [DataMember] public string Status { get; set; }
    [DataMember] public string ImportID { get; set; }
    [DataMember] public int? ProductVersionID { get; set; }
    [DataMember] public bool NeedsIndexing { get; set; }
    [DataMember] public int? ImportFileID { get; set; }
          
  }
}