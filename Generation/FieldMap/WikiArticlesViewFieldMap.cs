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
      _fieldMap.AddMap("ArticleID", "ArticleID", false, false, false);
      _fieldMap.AddMap("ParentID", "ParentID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("ArticleName", "ArticleName", false, false, false);
      _fieldMap.AddMap("Body", "Body", false, false, false);
      _fieldMap.AddMap("Version", "Version", false, false, false);
      _fieldMap.AddMap("PublicView", "PublicView", false, false, false);
      _fieldMap.AddMap("PublicEdit", "PublicEdit", false, false, false);
      _fieldMap.AddMap("PortalView", "PortalView", false, false, false);
      _fieldMap.AddMap("PortalEdit", "PortalEdit", false, false, false);
      _fieldMap.AddMap("Private", "Private", false, false, false);
      _fieldMap.AddMap("IsDeleted", "IsDeleted", false, false, false);
      _fieldMap.AddMap("CreatedBy", "CreatedBy", false, false, false);
      _fieldMap.AddMap("CreatedDate", "CreatedDate", false, false, false);
      _fieldMap.AddMap("ModifiedBy", "ModifiedBy", false, false, false);
      _fieldMap.AddMap("ModifiedDate", "ModifiedDate", false, false, false);
      _fieldMap.AddMap("NeedsIndexing", "NeedsIndexing", false, false, false);
      _fieldMap.AddMap("Creator", "Creator", false, false, false);
      _fieldMap.AddMap("Modifier", "Modifier", false, false, false);
      _fieldMap.AddMap("Organization", "Organization", false, false, false);
            
    }
  }
  
}
