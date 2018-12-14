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
  [KnownType(typeof(ImportProxy))]
  public class ImportProxy
  {
    public ImportProxy() {}
    [DataMember] public int ImportID { get; set; }
    [DataMember] public string FileName { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public Guid ImportGUID { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
    [DataMember] public string RefTypeString { get; set; }
    [DataMember] public int? AuxID { get; set; }
    [DataMember] public bool IsDone { get; set; }
    [DataMember] public bool IsRunning { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime? DateStarted { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int CompletedRows { get; set; }
    [DataMember] public int TotalRows { get; set; }
    [DataMember] public bool IsRolledBack { get; set; }
    [DataMember] public int FilePathID { get; set; }
          
  }
  
}
