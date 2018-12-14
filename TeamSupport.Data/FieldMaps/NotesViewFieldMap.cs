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
      _fieldMap.AddMap("NoteID", "NoteID", true, true, true);
      _fieldMap.AddMap("RefType", "RefType", true, true, true);
      _fieldMap.AddMap("RefID", "RefID", true, true, true);
      _fieldMap.AddMap("Title", "Title", true, true, true);
      _fieldMap.AddMap("Description", "Description", true, true, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", true, true, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", true, true, true);
      _fieldMap.AddMap("DateModified", "DateModified", true, true, true);
      _fieldMap.AddMap("DateCreated", "DateCreated", true, true, true);
      _fieldMap.AddMap("CreatorName", "CreatorName", true, true, true);
      _fieldMap.AddMap("ModifierName", "ModifierName", true, true, true);
      _fieldMap.AddMap("NeedsIndexing", "NeedsIndexing", false, false, false);
      _fieldMap.AddMap("ParentOrganizationID", "ParentOrganizationID", false, false, false);
      _fieldMap.AddMap("OrganizationName", "OrganizationName", false, false, false);
      _fieldMap.AddMap("ContactName", "ContactName", false, false, false);
            
    }
  }
  
}
