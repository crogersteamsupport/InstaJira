using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class OrganizationProduct 
  {
  }

  public partial class OrganizationProducts 
  {

    public void LoadItemInfo(int organizationProductID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT op.*, o.Name AS OrganizationName, p.Name AS ProductName, pv.VersionNumber FROM OrganizationProducts op
                                LEFT JOIN Organizations o ON o.OrganizationID = op.OrganizationID
                                LEFT JOIN Products p ON p.ProductID = op.ProductID
                                LEFT JOIN ProductVersions pv ON pv.ProductVersionID = op.ProductVersionID
                                WHERE (OrganizationProductID = @OrganizationProductID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationProductID", organizationProductID);
        Fill(command);
      }
    }
    
    public void LoadByParentOrganizationID(int parentID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT op.* FROM OrganizationProducts op
                                LEFT JOIN Organizations o ON o.OrganizationID = op.OrganizationID
                                WHERE (o.ParentID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", parentID);
        Fill(command);
      }
    }

    public void LoadByParentOrganizationIDAndProductID(int parentID, int productID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
                SELECT 
                    op.* 
                FROM 
                    OrganizationProducts op
                    LEFT JOIN Organizations o
                        ON o.OrganizationID = op.OrganizationID
                WHERE
                    o.ParentID = @OrganizationID
                    AND op.ProductID = @ProductID";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", parentID);
            command.Parameters.AddWithValue("@ProductID", productID);
            Fill(command);
        }
    }

    public void LoadByParentOrganizationIDAndVersionID(int parentID, int versionID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
                SELECT 
                    op.* 
                FROM 
                    OrganizationProducts op
                    LEFT JOIN Organizations o
                        ON o.OrganizationID = op.OrganizationID
                WHERE
                    o.ParentID = @OrganizationID
                    AND op.ProductVersionID = @ProductVersionID";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", parentID);
            command.Parameters.AddWithValue("@ProductVersionID", versionID);
            Fill(command);
        }
    }

    public void LoadByProductVersionID(int productVersionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT 
          op.*
        FROM
          OrganizationProducts op
        WHERE 
          op.ProductVersionID = @ProductVersionID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
        Fill(command);
      }
    }


    public void LoadByOrganizationAndProductID(int orgId, int productID) {
        using (SqlCommand command = new SqlCommand()) {
            command.CommandText = "select * from OrganizationProducts where Organizationid = @ClientOrgID and productid=@ProductID";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ClientOrgID", orgId);
            command.Parameters.AddWithValue("@ProductID", productID);
            Fill(command);
        }
    }

	 public void LoadByOrganizationProductIDAndVersionID(int orgId, int productID, int versionID)
	 {
		 using (SqlCommand command = new SqlCommand())
		 {
			 command.CommandText = "select * from OrganizationProducts where Organizationid = @ClientOrgID and productid=@ProductID AND ProductVersionID = @ProductVersionID";
			 command.CommandType = CommandType.Text;
			 command.Parameters.AddWithValue("@ClientOrgID", orgId);
			 command.Parameters.AddWithValue("@ProductID", productID);
			 command.Parameters.AddWithValue("@ProductVersionID", versionID);
			 Fill(command);
		 }
	 }


    public void LoadByContactProductAndVersionID(int contactID, int productID, int? productVersionID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            string versionClause = "AND op.ProductVersionID IS NULL";
            if (productVersionID != null)
            {
                versionClause = "AND op.ProductVersionID = @ProductVersionID";
            }
            command.CommandText = @"
                SELECT
                    op.* 
                FROM 
                    OrganizationProducts op
                    JOIN Organizations o
                        ON op.OrganizationID = o.OrganizationID
                    JOIN Users c
                        ON c.OrganizationID = o.OrganizationID
                WHERE 
                    c.UserID = @ContactID 
                    AND op.ProductID = @ProductID
            " + versionClause;
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ContactID", contactID);
            command.Parameters.AddWithValue("@ProductID", productID);
            if (productVersionID != null)
            {
                command.Parameters.AddWithValue("@ProductVersionID", (int)productVersionID);
            }
            Fill(command);
        }
    }

      public OrganizationProduct FindByImportID(string importID)
    {
      foreach (OrganizationProduct organizationProduct in this)
      {
        if (organizationProduct.ImportID == importID)
        {
          return organizationProduct;
        }
      }
      return null;
    }

    public void LoadForCustomerProductGrid(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT 
	        op.OrganizationProductID, p.Name AS ProductName, pvs.Name AS VersionStatus, pv.VersionNumber, op.SupportExpiration, pv.IsReleased, pv.ReleaseDate"
          + DataUtils.GetCustomFieldColumns(LoginUser, ReferenceType.OrganizationProducts, LoginUser.OrganizationID, "op.OrganizationProductID", 25, true) +
          @" FROM OrganizationProducts op
          LEFT JOIN Products p ON op.ProductID = p.ProductID
          LEFT JOIN ProductVersions pv ON op.ProductVersionID = pv.ProductVersionID
          LEFT JOIN ProductVersionStatuses pvs ON pv.ProductVersionStatusID = pvs.ProductVersionStatusID
          WHERE (op.OrganizationID = @OrganizationID)
          ORDER BY p.Name, VersionNumber";

        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command, "OrganizationProducts,ProductVersions,Products");
      }
    }

    public void LoadForCustomerProductGridSorting(int organizationID, string sortColumn, string sortDirection, bool includeCompanyChildren = false)
    {
        //int end = start + 20;
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
        WITH OrderedOrganizationProduct AS
        (
	        SELECT 
		        OrganizationProductID, 
		        ROW_NUMBER() OVER (ORDER BY " + sortColumn + " " + sortDirection + @") AS rownum
	        FROM 
		        OrganizationProductsView 
            WHERE
                OrganizationID IN
                (
                    SELECT
                        @OrganizationID
                    UNION
                    SELECT
                        CustomerID
                    FROM
                        CustomerRelationships
                    WHERE
                        RelatedCustomerID = @OrganizationID
                        AND @IncludeCompanyChildren = 1
                )
        ) 
        SELECT 
          v.*
        FROM
          OrganizationProductsView v
          JOIN OrderedOrganizationProduct oop
            ON v.OrganizationProductID = oop.OrganizationProductID
        ORDER BY
          v." + sortColumn + " " + sortDirection;
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            command.Parameters.AddWithValue("@IncludeCompanyChildren", includeCompanyChildren);
            //command.Parameters.AddWithValue("@start", start);
            //command.Parameters.AddWithValue("@end", end);
            Fill(command);
        }
    }

    public void LoadForProductCustomerGridByVersion(int versionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT op.OrganizationProductID, o.Name, pv.VersionNumber"
                                + DataUtils.GetCustomFieldColumns(LoginUser, ReferenceType.OrganizationProducts, LoginUser.OrganizationID, "op.OrganizationProductID", 5, true) +
                              @" FROM Organizations o 
                                LEFT JOIN OrganizationProducts op ON op.OrganizationID = o.OrganizationID 
                                LEFT JOIN Products p ON p.ProductID = op.ProductID
                                LEFT JOIN ProductVersions pv ON pv.ProductVersionID = op.ProductVersionID
                                WHERE op.ProductVersionID = @VersionID AND o.IsActive = 1
                                ORDER BY o.Name, pv.VersionNumber";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@VersionID", versionID);
        Fill(command, "Organizations,OrganizationProducts");
      }
    }

    public void LoadForProductCustomerGridByProduct(int productID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT op.OrganizationProductID, o.Name, pv.VersionNumber"
                                + DataUtils.GetCustomFieldColumns(LoginUser, ReferenceType.OrganizationProducts, LoginUser.OrganizationID, "op.OrganizationProductID", 5, true) +
                              @" FROM Organizations o 
                                LEFT JOIN OrganizationProducts op ON op.OrganizationID = o.OrganizationID 
                                LEFT JOIN Products p ON p.ProductID = op.ProductID
                                LEFT JOIN ProductVersions pv ON pv.ProductVersionID = op.ProductVersionID
                                WHERE op.ProductID = @ProductID AND o.IsActive = 1
                                ORDER BY o.Name, pv.VersionNumber";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        Fill(command, "Organizations,OrganizationProducts");
      }
    }

    public void UpdateVersionProduct(int versionID, int newProductID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE OrganizationProducts SET ProductID = @ProductID WHERE (ProductVersionID = @ProductVersionID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductVersionID", versionID);
        command.Parameters.AddWithValue("@ProductID", newProductID);
        ExecuteNonQuery(command, "OrganizationProducts");
      }

    }

    public OrganizationProduct FindByOrganizationID(int organizationID)
    {
      foreach (OrganizationProduct organizationProduct in this)
      {
        if (organizationProduct.OrganizationID == organizationID)
        {
          return organizationProduct;
        }
      }
      return null;
    }

    public static void DeleteAllOrganizationsByProductVersionID(LoginUser loginUser, int productVersionID)
    {
      using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand command = connection.CreateCommand();
        command.Connection = connection;
        command.CommandType = CommandType.Text;
        command.CommandText = "DELETE FROM OrganizationProducts WHERE ProductVersionID = @ProductVersionID";
        command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
        command.ExecuteNonQuery();
      }
    }

    public static void DeleteAllOrganizationsByProductID(LoginUser loginUser, int productID)
    {
      using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand command = connection.CreateCommand();
        command.Connection = connection;
        command.CommandType = CommandType.Text;
        command.CommandText = "DELETE FROM OrganizationProducts WHERE ProductID = @ProductID";
        command.Parameters.AddWithValue("@ProductID", productID);
        command.ExecuteNonQuery();
      }
    }
  }
}
