using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class Products
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ProductID", "ProductID", false, false, true);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("Name", "Name", true, true, true);
      _fieldMap.AddMap("Description", "Description", true, true, true);
      _fieldMap.AddMap("ImportID", "ImportID", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, true);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, true);
      _fieldMap.AddMap("NeedsIndexing", "NeedsIndexing", false, false, false);
      _fieldMap.AddMap("ProductFamilyID", "ProductLineID", false, false, false);
      _fieldMap.AddMap("JiraProjectKey", "JiraProjectKey", false, false, false);
      _fieldMap.AddMap("ImportFileID", "ImportFileID", false, false, false);
      _fieldMap.AddMap("EmailReplyToAddress", "EmailReplyToAddress", false, false, false);
      _fieldMap.AddMap("SlaLevelID", "SlaLevelID", false, false, false);
      _fieldMap.AddMap("TFSProjectName", "TFSProjectName", false, false, false);
            
    }
  }
  
}
