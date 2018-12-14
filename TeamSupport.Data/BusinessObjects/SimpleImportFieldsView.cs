using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class SimpleImportFieldsViewItem
  {
  }
  
  public partial class SimpleImportFieldsView
  {
    public void LoadByRefType(int refType)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
        @"
          SELECT 
            * 
          FROM 
            SimpleImportFieldsView
          WHERE
            RefType = @RefType
            AND (IsCustom = 'false' OR OrganizationID = @OrganizationID)
          ORDER BY
            Position,
            IsRequired DESC
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefType", (int)refType);
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);

        Fill(command);
      }
    }
  }
  
}
