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
      _fieldMap.AddMap("AddressID", "AddressID", false, false, false);
      _fieldMap.AddMap("RefID", "RefID", false, false, false);
      _fieldMap.AddMap("RefType", "RefType", false, false, false);
      _fieldMap.AddMap("Description", "Description", false, false, false);
      _fieldMap.AddMap("Addr1", "Addr1", false, false, false);
      _fieldMap.AddMap("Addr2", "Addr2", false, false, false);
      _fieldMap.AddMap("Addr3", "Addr3", false, false, false);
      _fieldMap.AddMap("City", "City", false, false, false);
      _fieldMap.AddMap("State", "State", false, false, false);
      _fieldMap.AddMap("Zip", "Zip", false, false, false);
      _fieldMap.AddMap("Country", "Country", false, false, false);
      _fieldMap.AddMap("Comment", "Comment", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("ImportFileID", "ImportFileID", false, false, false);
            
    }
  }
  
}
