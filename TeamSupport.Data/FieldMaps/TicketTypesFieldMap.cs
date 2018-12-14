using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class TicketTypes
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("TicketTypeID", "TicketTypeID", true, true, true);
      _fieldMap.AddMap("Name", "Name", true, true, true);
      _fieldMap.AddMap("Description", "Description", true, true, true);
      _fieldMap.AddMap("Position", "Position", true, true, true);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", true, true, true);
      _fieldMap.AddMap("DateCreated", "DateCreated", true, true, true);
      _fieldMap.AddMap("DateModified", "DateModified", true, true, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", true, true, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", true, true, true);
      _fieldMap.AddMap("IconUrl", "IconUrl", true, true, true);
      _fieldMap.AddMap("ProductFamilyID", "ProductLineID", true, true, true);
      _fieldMap.AddMap("IsActive", "IsActive", false, false, false);
            
    }
  }
  
}
