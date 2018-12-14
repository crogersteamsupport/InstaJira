using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class CRMLinkSynchedOrganizations
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CRMLinkSynchedOrganizationsID", "CRMLinkSynchedOrganizationsID", false, false, false);
      _fieldMap.AddMap("CRMLinkTableID", "CRMLinkTableID", false, false, false);
      _fieldMap.AddMap("OrganizationCRMID", "OrganizationCRMID", false, false, false);
            
    }
  }
  
}
