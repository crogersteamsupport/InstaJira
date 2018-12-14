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
  [KnownType(typeof(TokStorageItemProxy))]
  public class TokStorageItemProxy
  {
    public TokStorageItemProxy() {}
    [DataMember] public bool Transcoded { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string AmazonPath { get; set; }
    [DataMember] public DateTime CreatedDate { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public string ArchiveID { get; set; }
          
  }
}
