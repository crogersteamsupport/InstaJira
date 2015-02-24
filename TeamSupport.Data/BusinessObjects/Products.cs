using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class Product 
  {
    public static int? GetIDByName(LoginUser loginUser, string name, int? parentID)
    {
      Products products = new Products(loginUser);
      products.LoadByProductName(loginUser.OrganizationID, name);
      if (products.IsEmpty) return null;
      else return products[0].ProductID;
    }
  }

  public partial class Products
  {

    partial void BeforeRowDelete(int productID)
    {
      Product product = (Product)Products.GetProduct(LoginUser, productID);
      string description = "Deleted product '" + product.Name + "'";
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Products, productID, description);
    }

    partial void BeforeRowEdit(Product product)
    {
      string description;

      Product oldProduct = (Product) Products.GetProduct(LoginUser, product.ProductID);

      if (oldProduct.Description != product.Description)
      {
        description = "Changed description for product '" + product.Description + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Products, product.ProductID, description);
      }

      if (oldProduct.Name != product.Name)
      {
        description = "Changed product name '" + oldProduct.Description + "' to '" + product.Name + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Products, product.ProductID, description);
      }
    }

    partial void AfterRowInsert(Product product)
    {
      string description = "Created product '" + product.Name + "'";
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Products, product.ProductID, description);
    }

    public void LoadByProductForGrid(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "uspSelectProductsForGrid";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }

    /// <summary>
    /// Loads the products for the TS accounts organizationid and orders by name.
    /// </summary>
    /// <param name="organizationID"></param>

    public void LoadByOrganizationID(int organizationID, string orderBy = "Name")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Products WHERE OrganizationID = @OrganizationID ORDER BY " + orderBy;
        command.CommandText = InjectCustomFields(command.CommandText, "ProductID", ReferenceType.Products);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByOrganizationIDAndUserRights(int organizationID, int userID, string orderBy = "Name")
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
                SELECT
                    p.*
                FROM 
                    Products p
                    JOIN Organizations o
                        ON p.OrganizationID = o.OrganizationID
                WHERE 
                    p.OrganizationID = @OrganizationID
                    AND
                    (
                        o.UseProductFamilies = 0
                        OR EXISTS (SELECT UserID FROM Users WHERE UserID = @UserID AND ProductFamiliesRights = 0)
                        OR p.ProductID IN 
                        (
                            SELECT
                                ip.ProductID
                            FROM
                                Products ip
                                JOIN UserRightsProductFamilies urpf
                                    ON ip.ProductFamilyID = urpf.ProductFamilyID
                            WHERE
                                urpf.UserID = @UserID
                        )
                    )
                ORDER BY " + orderBy;
            command.CommandText = InjectCustomFields(command.CommandText, "ProductID", ReferenceType.Products);
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            command.Parameters.AddWithValue("@UserID", userID);
            Fill(command);
        }
    }

    public void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT p.* FROM Products p WHERE p.ProductID IN 
                                (SELECT DISTINCT op.ProductID FROM OrganizationProducts op 
                                 LEFT JOIN OrganizationTickets ot ON ot.OrganizationID = op.OrganizationID
                                 LEFT JOIN Tickets t ON t.TicketID = ot.TicketID
                                 WHERE t.TicketID = @TicketID)
                               UNION 
                               SELECT p.* FROM Products p LEFT JOIN Tickets t ON t.ProductID = p.ProductID WHERE  t.TicketID = @TicketID  
                               ORDER BY p.Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadByCustomerIDs(int[] organizationIDs)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string ids = string.Join(",", organizationIDs.Select(x => x.ToString()).ToArray());
        command.CommandText = @"SELECT p.* FROM Products p WHERE p.ProductID IN 
                                (SELECT DISTINCT op.ProductID FROM OrganizationProducts op 
                                 WHERE op.OrganizationID IN ("+ ids +"))";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }

    public void LoadByCustomerID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT p.* FROM Products p WHERE p.ProductID IN 
                                (SELECT DISTINCT op.ProductID FROM OrganizationProducts op 
                                 WHERE op.OrganizationID = @OrganizationID)
                              ORDER BY p.Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }
    public void RemoveCustomer(int organizationProductID)
    {
      OrganizationProducts organizationProducts = new OrganizationProducts(LoginUser);
      organizationProducts.LoadItemInfo(organizationProductID);
      if (organizationProducts.IsEmpty) return;
      OrganizationProduct organizationProduct = organizationProducts[0];
      
      /*using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM OrganizationProducts WHERE (OrganizationProductID = @OrganizationProductID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationProductID", organizationProductID);
        ExecuteNonQuery(command, "OrganizationProducts");
      }*/

      string description;

      if (organizationProduct.ProductVersionID != null)
      {
        description = "Removed product '" + (string)organizationProduct.Row["ProductName"] + "' version '" + (string)organizationProduct.Row["VersionNumber"] + "' from customer '" +
                              (string)organizationProduct.Row["OrganizationName"] + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.ProductVersions, (int)organizationProduct.ProductVersionID, description);
      }
      else
	    {
        description = "Removed product '" + (string)organizationProduct.Row["ProductName"] + "' from customer '" +
                              (string)organizationProduct.Row["OrganizationName"] + "'";

	    }
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Organizations, organizationProduct.OrganizationID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Products, organizationProduct.ProductID, description);

      organizationProducts.DeleteFromDB(organizationProductID);
      
    }

    public void RemoveCustomer(int organizationID, int productID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM OrganizationProducts WHERE (ProductID = @ProductID) AND (OrganizationID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@ProductID", productID);
        ExecuteNonQuery(command, "OrganizationProducts");
      }
      Organization org = (Organization)Organizations.GetOrganization(LoginUser, organizationID);
      Product product = (Product)Products.GetProduct(LoginUser, productID);
      string description = "Removed '" + product.Name + "' from the customer " + org.Name;
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Products, productID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Organizations, organizationID, description);
    }

    public void AddCustomer(int organizationID, int productID, int versionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"INSERT INTO 
          OrganizationProducts 
          (OrganizationID, ProductID, ProductVersionID, IsVisibleOnPortal, DateCreated, DateModified, CreatorID, ModifierID) 
          VALUES 
          (@OrganizationID, @ProductID, @ProductVersionID, 1, @DateCreated, @DateModified, @CreatorID, @ModifierID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@ProductVersionID", versionID);
        command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
        command.Parameters.AddWithValue("@DateModified", DateTime.UtcNow);
        command.Parameters.AddWithValue("@CreatorID", LoginUser.UserID);
        command.Parameters.AddWithValue("@ModifierID", LoginUser.UserID);
        ExecuteNonQuery(command, "OrganizationProducts");
      }

      string organizationName = ((Organization)Organizations.GetOrganization(LoginUser, organizationID)).Name;
      string productName = ((Product)Products.GetProduct(LoginUser, productID)).Name;
      string versionNumber = ((ProductVersion)ProductVersions.GetProductVersion(LoginUser, (int)versionID)).VersionNumber;

      string description = "Added product '" + productName + "' version '" + versionNumber + "' to customer '" + organizationName + "'";
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.ProductVersions, (int)versionID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Products, productID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Organizations, organizationID, description);


    }

    public void AddCustomer(int organizationID, int productID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"INSERT INTO 
          OrganizationProducts 
          (OrganizationID, ProductID, IsVisibleOnPortal, DateCreated, DateModified, CreatorID, ModifierID) 
          VALUES 
          (@OrganizationID, @ProductID, 1, @DateCreated, @DateModified, @CreatorID, @ModifierID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
        command.Parameters.AddWithValue("@DateModified", DateTime.UtcNow);
        command.Parameters.AddWithValue("@CreatorID", LoginUser.UserID);
        command.Parameters.AddWithValue("@ModifierID", LoginUser.UserID);
        ExecuteNonQuery(command, "OrganizationProducts");
      }

      string organizationName = ((Organization)Organizations.GetOrganization(LoginUser, organizationID)).Name;
      string productName = ((Product)Products.GetProduct(LoginUser, productID)).Name;

      string description = "Added product '" + productName + "' to customer '" + organizationName + "'";

      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Products, productID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Organizations, organizationID, description);
    }

    public Product FindByImportID(string importID)
    {
      importID = importID.Trim().ToLower();
      foreach (Product product in this)
      {
        if ((product.ImportID != null && product.ImportID.ToLower().Trim() == importID) || product.Name.ToLower().Trim() == importID)
        {
          return product;
        }
      }
      return null;
    }

    public Product FindByName(string name) {
        foreach (Product product in this) {
            if (product.Name != null && product.Name.ToLower().Trim() == name.ToLower().Trim()) {
                return product;
            }
        }
        return null;
    }

    public static void DeleteProduct(LoginUser loginUser, int productID)
    {
      Products products = new Products(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM OrganizationProducts WHERE (ProductID = @ProductID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@ProductID", productID);
        products.ExecuteNonQuery(command, "OrganizationProducts");

        command.CommandText = "UPDATE Tickets SET ProductID = null, ReportedVersionID = null, SolvedVersionID = null WHERE (ProductID = @ProductID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@ProductID", productID);
        products.ExecuteNonQuery(command, "Tickets");

      }


      products.LoadByProductID(productID);
      if (!products.IsEmpty) products[0].Delete();
      products.Save();

    }

    public void LoadByProductName(int parentID, string name, int maxRows)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT TOP (@MaxRows) * FROM Products WHERE ((Name LIKE '%'+@Name+'%') OR (Description LIKE '%'+@Name+'%')) AND (OrganizationID = @ParentID) ORDER BY Name";
            command.CommandType = CommandType.Text;

            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@ParentID", parentID);
            command.Parameters.AddWithValue("@MaxRows", maxRows);
            Fill(command);
        }
    }

    public void LoadByProductName(int parentID, string name)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Products WHERE Name = @Name AND OrganizationID = @ParentID";
        command.CommandType = CommandType.Text;

        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@ParentID", parentID);
        Fill(command);
      }
    }

    public void LoadForIndexing(int organizationID, int max, bool isRebuilding)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string text = @"
        SELECT 
          TOP {0} 
          ProductID
        FROM 
          Products p WITH(NOLOCK)
        WHERE 
          p.NeedsIndexing = 1
          AND p.OrganizationID = @OrganizationID
        ORDER BY 
          p.DateModified DESC";

        if (isRebuilding)
        {
          text = @"
          SELECT 
            ProductID
          FROM 
            products p WITH(NOLOCK)
          WHERE 
            p.OrganizationID = @OrganizationID
          ORDER BY 
            p.DateModified DESC";
        }

        command.CommandText = string.Format(text, max.ToString());
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByProductFamilyIDLimit(int productFamilyID, int start)
    {
        int end = start + 10;
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
        WITH OrderedProduct AS
        (
	        SELECT 
		        ProductID, 
		        ROW_NUMBER() OVER (ORDER BY Name ASC) AS rownum
	        FROM 
		        Products 
	        WHERE 
		        ProductFamilyID = @ProductFamilyID 
        ) 
        SELECT 
          p.*
        FROM
          Products p
          JOIN OrderedProduct op
            ON p.ProductID = op.ProductID
        WHERE 
	        op.rownum BETWEEN @start and @end
        ORDER BY
          p.Name ASC";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ProductFamilyID", productFamilyID);
            command.Parameters.AddWithValue("@start", start);
            command.Parameters.AddWithValue("@end", end);
            Fill(command);
        }
    }

    public static Product GetProductByUserRights(LoginUser loginUser, int productID)
    {
        Products products = new Products(loginUser);
        products.LoadByProductIDAndUserRights(productID, loginUser.UserID);
        if (products.IsEmpty)
            return null;
        else
            return products[0];
    }
      
      public virtual void LoadByProductIDAndUserRights(int productID, int userID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
                SET NOCOUNT OFF; 
                SELECT 
                    p.[ProductID]
                    , p.[OrganizationID]
                    , p.[Name]
                    , p.[Description]
                    , p.[ImportID]
                    , p.[DateCreated]
                    , p.[DateModified]
                    , p.[CreatorID]
                    , p.[ModifierID]
                    , p.[NeedsIndexing]
                    , p.[ProductFamilyID] 
                FROM 
                    [dbo].[Products] p
                    JOIN Organizations o
                        ON p.OrganizationID = o.OrganizationID
                WHERE
                    p.[ProductID] = @ProductID
                    AND
                    (
                        o.UseProductFamilies = 0
                        OR (EXISTS (SELECT * FROM Users WHERE UserID = @UserID AND (ProductFamiliesRights < 1)))
                        OR p.ProductFamilyID IN (SELECT ProductFamilyID FROM UserRightsProductFamilies WHERE UserID=@UserID)
                    );
            ";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("ProductID", productID);
            command.Parameters.AddWithValue("UserID", userID);
            Fill(command);
        }
    }    
  }

  public class ProductSearch
  {
    public ProductSearch() { }
    public ProductSearch(Product item)
    {
      productID = item.ProductID;
      organizationID = item.OrganizationID;
      name = item.Name;
      description = item.Description;
      dateCreated = item.DateCreated;
      dateModified = item.DateModified;

    }

    public int productID { get; set; }
    public int organizationID { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public DateTime? dateCreated { get; set; }
    public DateTime? dateModified { get; set; }
    public int openTicketCount { get; set; }
  }
}
