using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class UserProduct
  {
  }
  
  public partial class UserProducts
  {
      public void LoadForContactProductGridSorting(int contactID, string sortColumn, string sortDirection)
      {
          //int end = start + 20;
          if (sortColumn == "OrganizationProductID")
          {
              sortColumn = "UserProductID";
          }

          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = @"
        WITH OrderedUserProduct AS
        (
	        SELECT 
		        UserProductID, 
		        ROW_NUMBER() OVER (ORDER BY " + sortColumn + " " + sortDirection + @") AS rownum
	        FROM 
		        UserProductsView 
	        WHERE 
		        UserID = @UserID 
        ) 
        SELECT 
          v.*
        FROM
          UserProductsView v
          JOIN OrderedUserProduct oup
            ON v.UserProductID = oup.UserProductID
        ORDER BY
          v." + sortColumn + " " + sortDirection;
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@UserID", contactID);
              //command.Parameters.AddWithValue("@start", start);
              //command.Parameters.AddWithValue("@end", end);
              Fill(command);
          }
      }

      public void LoadByOrganizationProductAndVersionID(int organizationID, int productID, int? productVersionID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              string versionClause = "AND up.ProductVersionID IS NULL";
              if (productVersionID != null)
              {
                  versionClause = "AND up.ProductVersionID = @ProductVersionID";
              }
              command.CommandText = @"
                SELECT
                    up.* 
                FROM 
                    UserProducts up
                    JOIN Users c
                        ON up.UserID = c.UserID
                    JOIN Organizations o
                        ON c.OrganizationID = o.OrganizationID
                WHERE 
                    o.OrganizationID = @OrganizationID 
                    AND up.ProductID = @ProductID
                " + versionClause;
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@OrganizationID", organizationID);
              command.Parameters.AddWithValue("@ProductID", productID);
              if (productVersionID != null)
              {
                  command.Parameters.AddWithValue("@ProductVersionID", (int)productVersionID);
              }
              Fill(command);
          }
      }

		public void LoadByContactID(int contactID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = @"
        SELECT
			*
		FROM
			UserProducts
		WHERE
			UserID = @UserID";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@UserID", contactID);
				//command.Parameters.AddWithValue("@start", start);
				//command.Parameters.AddWithValue("@end", end);
				Fill(command);
			}
		}
  }
  
}
