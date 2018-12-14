using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  
  public partial class Import : BaseItem
  {
    public ImportProxy GetProxy()
    {
      ImportProxy result = new ImportProxy();
      result.FilePathID = this.FilePathID;
      result.IsRolledBack = this.IsRolledBack;
      result.CreatorID = this.CreatorID;
      result.IsRunning = this.IsRunning;
      result.IsDone = this.IsDone;
      result.AuxID = this.AuxID;
      result.RefType = this.RefType;
      result.RefTypeString = this.RefType.ToString();
      result.ImportGUID = this.ImportGUID;
      result.OrganizationID = this.OrganizationID;
      result.FileName = this.FileName;
      result.ImportID = this.ImportID;
      result.CompletedRows = this.CompletedRows;
      result.TotalRows = this.TotalRows;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateStarted = this.DateStartedUtc == null ? this.DateStartedUtc : DateTime.SpecifyKind((DateTime)this.DateStartedUtc, DateTimeKind.Utc); 
       
       
      return result;
    }	
  }
}
