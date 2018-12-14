using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class Addresses
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("AddressID", "AddressID", false, false, true);
      _fieldMap.AddMap("RefID", "RefID", false, false, false);
      _fieldMap.AddMap("RefType", "RefType", false, false, false);
      _fieldMap.AddMap("Description", "Description", true, true, true);
      _fieldMap.AddMap("Addr1", "Addr1", true, true, true);
      _fieldMap.AddMap("Addr2", "Addr2", true, true, true);
      _fieldMap.AddMap("Addr3", "Addr3", true, true, true);
      _fieldMap.AddMap("City", "City", true, true, true);
      _fieldMap.AddMap("State", "State", true, true, true);
      _fieldMap.AddMap("Zip", "Zip", true, true, true);
      _fieldMap.AddMap("Country", "Country", true, true, true);
      _fieldMap.AddMap("Comment", "Comment", true, true, true);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, true);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, true);
      _fieldMap.AddMap("ImportFileID", "ImportFileID", false, false, false);
            
    }
  }
  
}
