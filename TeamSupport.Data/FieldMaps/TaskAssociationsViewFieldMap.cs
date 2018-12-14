using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class TaskAssociationsView
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("TaskID", "TaskID", false, false, false);
      _fieldMap.AddMap("RefID", "RefID", false, false, false);
      _fieldMap.AddMap("RefType", "RefType", false, false, false);
      _fieldMap.AddMap("TicketNumber", "TicketNumber", false, false, false);
      _fieldMap.AddMap("TicketName", "TicketName", false, false, false);
      _fieldMap.AddMap("User", "User", false, false, false);
      _fieldMap.AddMap("Company", "Company", false, false, false);
      _fieldMap.AddMap("Group", "Group", false, false, false);
      _fieldMap.AddMap("Product", "Product", false, false, false);
      _fieldMap.AddMap("Contact", "Contact", false, false, false);
            
    }
  }
  
}
