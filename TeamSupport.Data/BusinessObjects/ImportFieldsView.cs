using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ImportFieldsViewItem
  {
  }
  
  public partial class ImportFieldsView
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
            ImportFieldsView
          WHERE
            RefType = @RefType
            AND (OrganizationID IS NULL OR OrganizationID = @OrganizationID)
            AND
            ( 
              ImportID IS NULL
              OR ImportID = (SELECT TOP 1 ImportID FROM Imports WHERE OrganizationID = @OrganizationID AND RefType = @RefType ORDER BY ImportID DESC)
            )
          ORDER BY
            Position
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefType", (int)refType);
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);

        Fill(command);
      }
    }

    public void LoadByImportID(int importID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
        @"
          SELECT 
            * 
          FROM 
            ImportFieldsView
          WHERE
            ImportID = @ImportID
          ORDER BY
            Position
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ImportID", importID);

        Fill(command);
      }
    }

    public ImportFieldsViewItem FindByFieldName(string fieldName)
    {
      foreach (ImportFieldsViewItem field in this)
      {
        if (field.FieldName.Trim().ToLower() == fieldName.Trim().ToLower())
        {
          return field;
        }
      }
      return null;
    }
  
  }
  
}
