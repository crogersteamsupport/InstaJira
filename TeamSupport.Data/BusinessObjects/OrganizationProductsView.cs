using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class OrganizationProductsViewItem
  {
  }
  
  public partial class OrganizationProductsView
  {
    public void LoadByOrganizationID(int organizationID, string orderBy = "OrganizationProductID")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM OrganizationProductsView WHERE (OrganizationID = @OrganizationID) ORDER BY " + orderBy;
        command.CommandText = InjectCustomFields(command.CommandText, "OrganizationProductID", ReferenceType.OrganizationProducts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByOrganizationAndProductIDs(int organizationID, int productID, string orderBy = "OrganizationProductID")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = 
          @"
            SELECT 
              * 
            FROM
              OrganizationProductsView 
            WHERE
              OrganizationID = @OrganizationID 
              AND ProductID = @ProductID
            ORDER BY
          " + orderBy;
        command.CommandText = InjectCustomFields(command.CommandText, "OrganizationProductID", ReferenceType.OrganizationProducts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@ProductID", productID);
        Fill(command);
      }
    }

    public void LoadByOrganizationProductAndVersionIDs(int organizationID, int productID, int productVersionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
          @"
            SELECT 
              * 
            FROM
              OrganizationProductsView 
            WHERE
              OrganizationID = @OrganizationID 
              AND ProductID = @ProductID 
              AND ProductVersionID = @ProductVersionID
            ORDER BY 
              OrganizationProductID
          ";
        command.CommandText = InjectCustomFields(command.CommandText, "OrganizationProductID", ReferenceType.OrganizationProducts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
        Fill(command);
      }
    }

    public void LoadByOrganizationProductID(int organizationProductID, bool includeCustomFields)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
          @"
            SELECT 
              * 
            FROM
              OrganizationProductsView 
            WHERE
              OrganizationProductID = @OrganizationProductID 
            ORDER BY 
              OrganizationProductID
          ";
        if (includeCustomFields)
        {
          command.CommandText = InjectCustomFields(command.CommandText, "OrganizationProductID", ReferenceType.OrganizationProducts);
        }
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationProductID", organizationProductID);
        Fill(command);
      }
    }

    public void LoadByProductID(int productID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
          @"
            SELECT 
              * 
            FROM
              OrganizationProductsView 
            WHERE
              ProductID = @ProductID 
            ORDER BY 
              OrganizationProductID DESC
          ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        Fill(command);
      }
    }

    public void LoadByProductIDForExport(int productID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
          @"
            SELECT 
              OrganizationName AS Customer,
              VersionNumber As Version,
              SupportExpiration As 'Support Expiration',
              VersionStatus As Status,
              IsReleased As Released,
              ReleaseDate As 'Release Date',
              DateCreated As 'Date Created'
            FROM
              OrganizationProductsView 
            WHERE
              ProductID = @ProductID 
            ORDER BY 
              OrganizationProductID DESC
          ";
        command.CommandText = InjectCustomFields(command.CommandText, "OrganizationProductID", ReferenceType.OrganizationProducts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        Fill(command);
      }
    }

    public void LoadByProductIDLimit(int productID, int start, string sortColumn, string sortDirection)
    {
        int end = start + 20;
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
		        ProductID = @ProductID 
        ) 
        SELECT 
          v.*
        FROM
          OrganizationProductsView v
          JOIN OrderedOrganizationProduct oop
            ON v.OrganizationProductID = oop.OrganizationProductID
        WHERE 
	        oop.rownum BETWEEN @start and @end
        ORDER BY
          v." + sortColumn + " " + sortDirection;
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ProductID", productID);
            command.Parameters.AddWithValue("@start", start);
            command.Parameters.AddWithValue("@end", end);
            Fill(command);
        }
    }

    public void LoadByProductVersionID(int productVersionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
          @"
            SELECT 
              * 
            FROM
              OrganizationProductsView 
            WHERE
              ProductVersionID = @ProductVersionID 
            ORDER BY 
              OrganizationProductID DESC
          ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
        Fill(command);
      }
    }

    public void LoadByProductVersionIDForExport(int productVersionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
          @"
            SELECT 
              OrganizationName AS Customer,
              SupportExpiration As 'Support Expiration',
              VersionStatus As Status,
              IsReleased As Released,
              ReleaseDate As 'Release Date',
              DateCreated As 'Date Created'
            FROM
              OrganizationProductsView 
            WHERE
              ProductVersionID = @ProductVersionID 
            ORDER BY 
              OrganizationProductID DESC
          ";
        command.CommandText = InjectCustomFields(command.CommandText, "OrganizationProductID", ReferenceType.OrganizationProducts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
        Fill(command);
      }
    }

    public void LoadByProductVersionIDLimit(int productVersionID, int start, string sortColumn, string sortDirection)
    {
        int end = start + 20;
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
		        ProductVersionID = @ProductVersionID 
        ) 
        SELECT 
          v.*
        FROM
          OrganizationProductsView v
          JOIN OrderedOrganizationProduct oop
            ON v.OrganizationProductID = oop.OrganizationProductID
        WHERE 
	      oop.rownum BETWEEN @start and @end
        ORDER BY
          v." + sortColumn + " " + sortDirection;
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
            command.Parameters.AddWithValue("@start", start);
            command.Parameters.AddWithValue("@end", end);
            Fill(command);
        }
    }
  }
  
}
