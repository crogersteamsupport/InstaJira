using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class SAMLSettings
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("Id", "Id", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("IsEnabled", "IsEnabled", false, false, false);
      _fieldMap.AddMap("IdPUrl", "IdPUrl", false, false, false);
      _fieldMap.AddMap("Certificate", "Certificate", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
            
    }
  }
  
}
