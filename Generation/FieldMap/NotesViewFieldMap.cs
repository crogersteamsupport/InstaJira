using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class NotesView
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("NoteID", "NoteID", false, false, false);
      _fieldMap.AddMap("RefType", "RefType", false, false, false);
      _fieldMap.AddMap("RefID", "RefID", false, false, false);
      _fieldMap.AddMap("Title", "Title", false, false, false);
      _fieldMap.AddMap("Description", "Description", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("NeedsIndexing", "NeedsIndexing", false, false, false);
      _fieldMap.AddMap("CreatorName", "CreatorName", false, false, false);
      _fieldMap.AddMap("ModifierName", "ModifierName", false, false, false);
      _fieldMap.AddMap("ParentOrganizationID", "ParentOrganizationID", false, false, false);
      _fieldMap.AddMap("OrganizationName", "OrganizationName", false, false, false);
      _fieldMap.AddMap("ContactName", "ContactName", false, false, false);
            
    }
  }
  
}
