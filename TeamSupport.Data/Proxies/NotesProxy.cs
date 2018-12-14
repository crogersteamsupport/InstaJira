using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;


namespace TeamSupport.Data
{
  
  public partial class Note : BaseItem
  {
    public NoteProxy GetProxy()
    {
      NoteProxy result = new NoteProxy();
      result.ProductFamilyID = this.ProductFamilyID;
      result.ImportFileID = this.ImportFileID;
      result.NeedsIndexing = this.NeedsIndexing;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.Description = (this.Description);
      result.Title = (this.Title);
      result.RefID = this.RefID;
      result.RefType = this.RefType;
      result.NoteID = this.NoteID;
      result.CreatorName = this.CreatorName;
      result.ProductFamily = this.ProductFamily;
      result.IsAlert = this.IsAlert;
      result.ActivityType = this.ActivityType;
      result.DateOccurred = this.DateOccurred == null ? null : this.DateOccurred;
            
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
