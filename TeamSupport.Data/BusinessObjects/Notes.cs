using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class Note 
  {

    public string CreatorName
    {
      get
      {
        if (Row.Table.Columns.Contains("CreatorName") && Row["CreatorName"] != DBNull.Value)
        {
          return (string)Row["CreatorName"];
        }
        else return "";
      }
    }
  }
  
  public partial class Notes
  {
    
    public void LoadByCustomer(int organizationID)
    {
      LoadByReferenceType(ReferenceType.Organizations, organizationID);
    }

    public void LoadByReferenceType(ReferenceType refType, int refID, string orderBy = "DateCreated", bool includeCompanyChildren = false)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
            SELECT 
                n.*
                , u.FirstName + ' ' + u.LastName AS CreatorName
            FROM
                Notes n 
                LEFT JOIN Users u
                    ON n.CreatorID = u.UserID 
            WHERE
                n.RefType = @ReferenceType
                AND n.RefID IN
                (
                    SELECT
                        @ReferenceID
                    UNION
                    SELECT
                        CustomerID
                    FROM
                        CustomerRelationships
                    WHERE
                        RelatedCustomerID = @ReferenceID
                        AND @IncludeCompanyChildren = 1
                )
            ORDER BY
                n." + orderBy + " DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ReferenceType", refType);
        command.Parameters.AddWithValue("@ReferenceID", refID);
        command.Parameters.AddWithValue("@IncludeCompanyChildren", includeCompanyChildren);
        Fill(command);
      }
    }

    public void LoadbyIsAlert(ReferenceType refType, int refID, string orderBy = "DateModified")
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"SELECT n.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM Notes n 
                                LEFT JOIN Users u ON n.CreatorID = u.UserID 
                                WHERE (n.RefID = @ReferenceID)
                                AND n.isAlert = 1
                                AND (n.RefType = @ReferenceType)
                                ORDER BY n." + orderBy;
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ReferenceType", refType);
            command.Parameters.AddWithValue("@ReferenceID", refID);
            Fill(command);
        }
    }   

  }
  
  
}
