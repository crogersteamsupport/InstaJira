using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class WikiArticlesView
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ArticleID", "ArticleID", false, false, true);
      _fieldMap.AddMap("ParentID", "ParentID", false, false, true);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("ArticleName", "ArticleName", true, true, true);
      _fieldMap.AddMap("Body", "Body", true, true, true);
      _fieldMap.AddMap("Version", "Version", true, true, true);
      _fieldMap.AddMap("PublicView", "PublicView", true, true, true);
      _fieldMap.AddMap("PublicEdit", "PublicEdit", true, true, true);
      _fieldMap.AddMap("PortalView", "PortalView", true, true, true);
      _fieldMap.AddMap("PortalEdit", "PortalEdit", true, true, true);
      _fieldMap.AddMap("Private", "Private", true, true, true);
      _fieldMap.AddMap("IsDeleted", "IsDeleted", false, false, false);
      _fieldMap.AddMap("CreatedBy", "CreatedBy", false, false, true);
      _fieldMap.AddMap("CreatedDate", "CreatedDate", false, false, true);
      _fieldMap.AddMap("ModifiedBy", "ModifiedBy", false, false, true);
      _fieldMap.AddMap("ModifiedDate", "ModifiedDate", false, false, true);
      _fieldMap.AddMap("NeedsIndexing", "NeedsIndexing", false, false, false);
      _fieldMap.AddMap("Creator", "Creator", false, false, true);
      _fieldMap.AddMap("Modifier", "Modifier", false, false, true);
      _fieldMap.AddMap("Organization", "Organization", false, false, false);
            
    }
  }
  
}
