using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  internal partial class Attachments
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("AttachmentID", "AttachmentID", true, true, true);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", true, true, true);
      _fieldMap.AddMap("FileName", "FileName", true, true, true);
      _fieldMap.AddMap("FileType", "FileType", true, true, true);
      _fieldMap.AddMap("FileSize", "FileSize", true, true, true);
      _fieldMap.AddMap("Path", "Path", false, false, false);
      _fieldMap.AddMap("Description", "Description", true, true, true);
      _fieldMap.AddMap("DateCreated", "DateCreated", true, true, true);
      _fieldMap.AddMap("DateModified", "DateModified", true, true, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", true, true, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", true, true, true);
      _fieldMap.AddMap("RefType", "RefType", true, true, true);
      _fieldMap.AddMap("RefID", "RefID", true, true, true);
      _fieldMap.AddMap("SentToJira", "SentToJira", false, false, false);
      _fieldMap.AddMap("ProductFamilyID", "ProductFamilyID", false, false, false);
      _fieldMap.AddMap("SentToTFS", "SentToTFS", false, false, false);
      _fieldMap.AddMap("SentToSnow", "SentToSnow", false, false, false);
      _fieldMap.AddMap("FilePathID", "FilePathID", false, false, false);
            
    }
  }
  
}
