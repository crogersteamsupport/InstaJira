using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class Attachments
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("AttachmentID", "AttachmentID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("FileName", "FileName", false, false, false);
      _fieldMap.AddMap("FileType", "FileType", false, false, false);
      _fieldMap.AddMap("FileSize", "FileSize", false, false, false);
      _fieldMap.AddMap("Path", "Path", false, false, false);
      _fieldMap.AddMap("Description", "Description", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("RefType", "RefType", false, false, false);
      _fieldMap.AddMap("RefID", "RefID", false, false, false);
      _fieldMap.AddMap("SentToJira", "SentToJira", false, false, false);
      _fieldMap.AddMap("AttachmentGUID", "AttachmentGUID", false, false, false);
      _fieldMap.AddMap("ProductFamilyID", "ProductFamilyID", false, false, false);
            
    }
  }
  
}
