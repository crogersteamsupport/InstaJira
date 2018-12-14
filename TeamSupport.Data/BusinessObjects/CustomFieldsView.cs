using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CustomFieldsViewItem
  {
  }
  
  public partial class CustomFieldsView
  {
      public void LoadByReferenceType(int organizationID, ReferenceType refType, int? auxID, string orderBy = "Position")
      {
          if (auxID == null) auxID = -1;
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = "SELECT * FROM CustomFieldsView WHERE (OrganizationID = @OrganizationID) AND (RefType = @RefType) AND (AuxID = @AuxID OR @AuxID < 0) ORDER BY " + orderBy;
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@OrganizationID", organizationID);
              command.Parameters.AddWithValue("@RefType", (int)refType);
              command.Parameters.AddWithValue("@AuxID", auxID);
              Fill(command, "CustomFields");
          }
      }
  }
  
}
