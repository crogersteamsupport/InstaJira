using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace TeamSupport.Data
{
  public partial class AssetsViewItem
  {
    public string DisplayName
    {
      get
      {
        if (String.IsNullOrEmpty(this.Name))
        {
          if (String.IsNullOrEmpty(this.SerialNumber))
          {
            return this.AssetID.ToString();
          }
          else
          {
            return this.SerialNumber;
          }
        }
        else
        {
          return this.Name;
        }
      }
    }
  }
  
  public partial class AssetsView
  {
    public void LoadByRefID(int refID, ReferenceType refType, bool includeCompanyChildren = false)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
          SELECT
            a.* 
          FROM
            AssetsView a
          WHERE
            a.AssetID IN
            (
              SELECT
                a.AssetID
              FROM
                  Assets a
                  JOIN AssetHistory h
                    ON a.AssetID = h.AssetID
                  JOIN AssetAssignments aa
                    ON h.HistoryID = aa.HistoryID
              WHERE
                h.RefType = @RefType
                AND h.ShippedTo IN
                (
                    SELECT
                        @RefID
                    UNION
                    SELECT
                        CustomerID
                    FROM
                        CustomerRelationships
                    WHERE
                        RelatedCustomerID = @RefID
                        AND @IncludeCompanyChildren = 1
                )
            )
          ORDER BY
            a.AssetID DESC
          ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefID", refID);
        command.Parameters.AddWithValue("@RefType", refType);
        command.Parameters.AddWithValue("@IncludeCompanyChildren", includeCompanyChildren);
        Fill(command);
      }
    }

    public void LoadAssignedToContactsByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
          SELECT
            a.* 
          FROM
            AssetsView a
            JOIN AssetHistory h
              ON a.AssetID = h.AssetID
            JOIN AssetAssignments aa
              ON h.HistoryID = aa.HistoryID
            JOIN Users u
              ON h.ShippedTo = u.UserID
              AND h.RefType = 32
            JOIN Organizations o
              ON u.OrganizationID = o.OrganizationID
	            AND o.Name <> '_Unknown Company'	
          WHERE 
            o.OrganizationID = @OrganizationID
          ORDER BY 
            aa.AssetAssignmentsID DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByLikeAssetDisplayName(int organizationID, string name, int maxRows)
    {
      using (SqlCommand command = new SqlCommand())
      {
        StringBuilder text = new StringBuilder(@"
        SELECT 
          TOP (@MaxRows) * 
        FROM 
          Assets 
        WHERE 
          OrganizationID = @OrganizationID
          AND Location = 2
          AND (Name LIKE '%'+@Name+'%' OR SerialNumber LIKE '%'+@Name+'%')
        ");
        command.CommandText = text.ToString();
        command.CommandType = CommandType.Text;

        command.Parameters.AddWithValue("@Name", name.Trim());
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@MaxRows", maxRows);
        Fill(command);
      }
    }

    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM AssetsView WHERE OrganizationID = @OrganizationID ORDER BY DateCreated DESC";
        command.CommandText = InjectCustomFields(command.CommandText, "AssetID", ReferenceType.Assets);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadOneByOrganizationID(int organizationId)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP 1 * FROM AssetsView WHERE (organizationId = @OrganizationId)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationId", organizationId);

        Fill(command);
      }
    }

    public void LoadByOrganizationID(int organizationId, NameValueCollection filters, string orderBy = "DateCreated DESC", int? limitNumber = null)
    {
      //Get the column names, this row will be deleted before getting the actual data
      this.LoadOneByOrganizationID(organizationId);

      using (SqlCommand command = new SqlCommand())
      {
        string limit = string.Empty;

        if (limitNumber != null)
        {
          limit = "TOP " + limitNumber.ToString();
        }

        string sql = BuildLoadByParentOrganizationIdSql(limit, organizationId, orderBy, filters, command.Parameters);
        sql = InjectCustomFields(sql, "AssetID", ReferenceType.Assets);
        command.CommandType = CommandType.Text;
        command.CommandText = sql;
        command.Parameters.AddWithValue("@OrganizationId", organizationId);
        this.DeleteAll();

        Fill(command);
      }
    }

    /// <summary>
    /// Build the sql statement including its filters to avoid using the .NET filtering. This improves performance greatly.
    /// </summary>
    /// <param name="limit">Return the specified number of rows. E.g. "TOP 1"</param>
    /// <param name="parentID">Organization Id.</param>
    /// <param name="orderBy">Fields in sql format to sort the results by. E.g. "LastName, FirstName"</param>
    /// <param name="filters">Filters to be applied. Specified in the URL request.</param>
    /// <param name="filterParameters">SqlParamenterCollection for the input parameters of the sql query.</param>
    /// <returns>A string with the full sql statement.</returns>
		public string BuildLoadByParentOrganizationIdSql(string limit, int organizationParentId, string orderBy, NameValueCollection filters, SqlParameterCollection filterParameters)
		{
			StringBuilder result = new StringBuilder();

			result.Append("SELECT " + limit + " * ");
			result.Append("FROM AssetsView ");
			result.Append("WHERE OrganizationID = @OrganizationId ");
			result.Append(DataUtils.BuildWhereClausesFromFilters(this.LoginUser, this, organizationParentId, filters, ReferenceType.Assets, "AssetID", null, ref filterParameters) + " ");
			result.Append("ORDER BY " + orderBy);

			return result.ToString();
		}

		public virtual void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SET NOCOUNT OFF; 
        SELECT 
          av.* 
        FROM 
          AssetsView av
          JOIN AssetTickets at 
            ON av.AssetID = at.AssetID
        WHERE 
          at.TicketID = @TicketID";
        command.CommandText = InjectCustomFields(command.CommandText, "av.AssetID", ReferenceType.Assets);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketID", ticketID);
        Fill(command);
      }
    }

    public virtual void LoadByTicketNumber(int ticketNumber, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SET NOCOUNT OFF; 
        SELECT 
          av.* 
        FROM 
          AssetsView av
          JOIN AssetTickets at 
            ON av.AssetID = at.AssetID
          JOIN Tickets t
            ON at.TicketID = t.TicketID
        WHERE 
          t.TicketNumber = @TicketNumber
          AND t.OrganizationID = @OrganizationID";
        command.CommandText = InjectCustomFields(command.CommandText, "av.AssetID", ReferenceType.Assets);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketNumber", ticketNumber);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByProductID(int productID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
          SELECT
            a.* 
          FROM
            AssetsView a
          WHERE
            a.ProductID = @ProductID
          ORDER BY
            a.AssetID DESC
          ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        Fill(command);
      }
    }

    public void LoadByProductIDLimit(int productID, int start)
    {
        int end = start + 10;
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
        WITH OrderedAsset AS
        (
	        SELECT 
		        AssetID, 
		        ROW_NUMBER() OVER (ORDER BY AssetID DESC) AS rownum
	        FROM 
		        AssetsView 
	        WHERE 
		        ProductID = @ProductID 
        ) 
        SELECT 
          a.*
        FROM
          AssetsView a
          JOIN OrderedAsset oa
            ON a.AssetID = oa.AssetID
        WHERE 
	        oa.rownum BETWEEN @start and @end
        ORDER BY
          a.AssetID DESC";
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
        command.CommandText = @"
          SELECT
            a.* 
          FROM
            AssetsView a
          WHERE
            a.ProductVersionID = @ProductVersionID
          ORDER BY
            a.AssetID DESC
          ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
        Fill(command);
      }
    }

    public void LoadByProductVersionIDLimit(int productVersionID, int start)
    {
        int end = start + 10;
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
        WITH OrderedAsset AS
        (
	        SELECT 
		        AssetID, 
		        ROW_NUMBER() OVER (ORDER BY AssetID DESC) AS rownum
	        FROM 
		        AssetsView 
	        WHERE 
		        ProductVersionID = @ProductVersionID 
        ) 
        SELECT 
          a.*
        FROM
          AssetsView a
          JOIN OrderedAsset oa
            ON a.AssetID = oa.AssetID
        WHERE 
	        oa.rownum BETWEEN @start and @end
        ORDER BY
          a.AssetID DESC";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
            command.Parameters.AddWithValue("@start", start);
            command.Parameters.AddWithValue("@end", end);
            Fill(command);
        }
    }
  }
  
  public class InventorySearchAsset
  {
    public InventorySearchAsset() { }
    public InventorySearchAsset(AssetsViewItem item)
    {
      assetID               = item.AssetID;
      organizationID        = item.OrganizationID;
      productName           = item.ProductName;
      productVersionNumber  = item.ProductVersionNumber;
      serialNumber          = item.SerialNumber;
      name                  = item.Name;
      location              = item.Location;  
      notes                 = item.Notes;
      warrantyExpiration    = item.WarrantyExpiration;
      dateCreated           = item.DateCreated;
      dateModified          = item.DateModified;
      creatorName           = item.CreatorName;
      modifierName          = item.ModifierName;

    }

    public int assetID { get; set; }
    public int organizationID { get; set; }
    public string productName { get; set; }
    public string productVersionNumber { get; set; }
    public string serialNumber { get; set; }
    public string name { get; set; }
    public string location { get; set; }
    public string notes { get; set; }
    public DateTime? warrantyExpiration { get; set; }
    public DateTime? dateCreated { get; set; }
    public DateTime? dateModified { get; set; }
    public string creatorName { get; set; }
    public string modifierName { get; set; }
  }
}
