using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  
  public partial class NotesViewItem : BaseItem
  {
    public NotesViewItemProxy GetProxy()
    {
      NotesViewItemProxy result = new NotesViewItemProxy();

      result.ContactName = (this.ContactName);
      result.OrganizationName = (this.OrganizationName);
      result.ParentOrganizationID = this.ParentOrganizationID;
      result.ModifierName = this.ModifierName;
      result.CreatorName = this.CreatorName;
      result.NeedsIndexing = this.NeedsIndexing;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.Description = (this.Description);
      result.Title = (this.Title);
      result.RefID = this.RefID;
      result.RefType = this.RefType;
      result.NoteID = this.NoteID;
       
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
