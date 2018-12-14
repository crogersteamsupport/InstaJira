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
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, true);
      _fieldMap.AddMap("Name", "Name", true, true, true);
      _fieldMap.AddMap("Description", "Description", true, true, true);
      _fieldMap.AddMap("Website", "Website", true, true, true);
      _fieldMap.AddMap("IsActive", "IsActive", true, true, true);
      _fieldMap.AddMap("InActiveReason", "InActiveReason", false, false, true);
      _fieldMap.AddMap("PrimaryUserID", "PrimaryUserID", true, true, true);
      _fieldMap.AddMap("PrimaryContact", "PrimaryContact", true, true, true);
      _fieldMap.AddMap("ParentID", "ParentID", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, true);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, true);
      _fieldMap.AddMap("HasPortalAccess", "HasPortalAccess", true, true, true);
      _fieldMap.AddMap("CreatedBy", "CreatedBy", false, false, true);
      _fieldMap.AddMap("LastModifiedBy", "LastModifiedBy", false, false, true);
      _fieldMap.AddMap("SAExpirationDate", "SAExpirationDate", true, true, true);
      _fieldMap.AddMap("SlaName", "SlaName", false, false, true);
      _fieldMap.AddMap("CRMLinkID", "CRMLinkID", false, false, true);
      _fieldMap.AddMap("PortalGuid", "PortalGuid", false, false, false);
      _fieldMap.AddMap("SlaLevelID", "SlaLevelID", false, false, true);
      _fieldMap.AddMap("DefaultWikiArticleID", "DefaultWikiArticleID", false, false, true);
      _fieldMap.AddMap("DefaultSupportGroupID", "DefaultSupportGroupID", false, false, true);
      _fieldMap.AddMap("DefaultSupportGroup", "DefaultSupportGroup", false, false, true);
      _fieldMap.AddMap("DefaultSupportUserID", "DefaultSupportUserID", false, false, true);
      _fieldMap.AddMap("DefaultSupportUser", "DefaultSupportUser", false, false, true);
      _fieldMap.AddMap("CompanyDomains", "Domains", false, false, true);
      _fieldMap.AddMap("SupportHoursMonth", "SupportHoursMonth", false, false, true);
      _fieldMap.AddMap("SupportHoursUsed", "SupportHoursUsed", false, false, true);
      _fieldMap.AddMap("SupportHoursRemaining", "SupportHoursRemaining", false, false, true);
      _fieldMap.AddMap("SupportHoursUsed", "SupportHoursUsed", false, false, false);
      _fieldMap.AddMap("SupportHoursRemaining", "SupportHoursRemaining", false, false, false);
      _fieldMap.AddMap("NeedsIndexing", "NeedsIndexing", false, false, true);
	  _fieldMap.AddMap("CustDisIndex", "CustDisIndex", false, false, true);
	  _fieldMap.AddMap("CustDistIndexTrend", "CustDistIndexTrend", false, false, true);
	}
  }
  
}
