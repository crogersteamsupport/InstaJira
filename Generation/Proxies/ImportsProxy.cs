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
    [DataMember] public int RefType { get; set; }
    [DataMember] public int? AuxID { get; set; }
    [DataMember] public bool IsDone { get; set; }
    [DataMember] public bool IsRunning { get; set; }
    [DataMember] public bool IsDeleted { get; set; }
    [DataMember] public bool NeedsDeleted { get; set; }
    [DataMember] public int TotalRows { get; set; }
    [DataMember] public int CompletedRows { get; set; }
    [DataMember] public DateTime? DateStarted { get; set; }
    [DataMember] public DateTime? DateEnded { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public bool IsRolledBack { get; set; }
    [DataMember] public int FilePathID { get; set; }
          
  }
  
  public partial class Import : BaseItem
  {
    public ImportProxy GetProxy()
    {
      ImportProxy result = new ImportProxy();
      result.FilePathID = this.FilePathID;
      result.IsRolledBack = this.IsRolledBack;
      result.CreatorID = this.CreatorID;
      result.CompletedRows = this.CompletedRows;
      result.TotalRows = this.TotalRows;
      result.NeedsDeleted = this.NeedsDeleted;
      result.IsDeleted = this.IsDeleted;
      result.IsRunning = this.IsRunning;
      result.IsDone = this.IsDone;
      result.AuxID = this.AuxID;
      result.RefType = this.RefType;
      result.ImportGUID = this.ImportGUID;
      result.OrganizationID = this.OrganizationID;
      result.FileName = this.FileName;
      result.ImportID = this.ImportID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
      result.DateEnded = this.DateEndedUtc == null ? this.DateEndedUtc : DateTime.SpecifyKind((DateTime)this.DateEndedUtc, DateTimeKind.Utc); 
      result.DateStarted = this.DateStartedUtc == null ? this.DateStartedUtc : DateTime.SpecifyKind((DateTime)this.DateStartedUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
