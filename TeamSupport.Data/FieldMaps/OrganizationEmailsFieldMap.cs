using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class OrganizationEmails
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("OrganizationEmailID", "OrganizationEmailID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("EmailTemplateID", "EmailTemplateID", false, false, false);
      _fieldMap.AddMap("Subject", "Subject", false, false, false);
      _fieldMap.AddMap("Body", "Body", false, false, false);
      _fieldMap.AddMap("ProductFamilyID", "ProductLineID", false, false, false);
            
    }
  }
  
}
