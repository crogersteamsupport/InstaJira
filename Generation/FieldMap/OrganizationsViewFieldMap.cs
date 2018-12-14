using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class OrganizationsView
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("Name", "Name", false, false, false);
      _fieldMap.AddMap("Description", "Description", false, false, false);
      _fieldMap.AddMap("Website", "Website", false, false, false);
      _fieldMap.AddMap("IsActive", "IsActive", false, false, false);
      _fieldMap.AddMap("InActiveReason", "InActiveReason", false, false, false);
      _fieldMap.AddMap("PrimaryUserID", "PrimaryUserID", false, false, false);
      _fieldMap.AddMap("PrimaryContactEmail", "PrimaryContactEmail", false, false, false);
      _fieldMap.AddMap("PrimaryContact", "PrimaryContact", false, false, false);
      _fieldMap.AddMap("ParentID", "ParentID", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("HasPortalAccess", "HasPortalAccess", false, false, false);
      _fieldMap.AddMap("CreatedBy", "CreatedBy", false, false, false);
      _fieldMap.AddMap("LastModifiedBy", "LastModifiedBy", false, false, false);
      _fieldMap.AddMap("SAExpirationDate", "SAExpirationDate", false, false, false);
      _fieldMap.AddMap("SlaName", "SlaName", false, false, false);
      _fieldMap.AddMap("CRMLinkID", "CRMLinkID", false, false, false);
      _fieldMap.AddMap("PortalGuid", "PortalGuid", false, false, false);
      _fieldMap.AddMap("SlaLevelID", "SlaLevelID", false, false, false);
      _fieldMap.AddMap("DefaultWikiArticleID", "DefaultWikiArticleID", false, false, false);
      _fieldMap.AddMap("DefaultSupportGroupID", "DefaultSupportGroupID", false, false, false);
      _fieldMap.AddMap("DefaultSupportUserID", "DefaultSupportUserID", false, false, false);
      _fieldMap.AddMap("DefaultSupportUser", "DefaultSupportUser", false, false, false);
      _fieldMap.AddMap("DefaultSupportGroup", "DefaultSupportGroup", false, false, false);
      _fieldMap.AddMap("CompanyDomains", "CompanyDomains", false, false, false);
      _fieldMap.AddMap("SupportHoursMonth", "SupportHoursMonth", false, false, false);
      _fieldMap.AddMap("SupportHoursUsed", "SupportHoursUsed", false, false, false);
      _fieldMap.AddMap("SupportHoursRemaining", "SupportHoursRemaining", false, false, false);
      _fieldMap.AddMap("NeedsIndexing", "NeedsIndexing", false, false, false);
      _fieldMap.AddMap("CustDisIndex", "CustDisIndex", false, false, false);
      _fieldMap.AddMap("CustDistIndexTrend", "CustDistIndexTrend", false, false, false);
      _fieldMap.AddMap("PhoneNumbers", "PhoneNumbers", false, false, false);
            
    }
  }
  
}
