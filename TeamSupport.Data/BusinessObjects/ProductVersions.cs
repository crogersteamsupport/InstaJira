using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace TeamSupport.Data
{
  public partial class ProductVersion 
  {
    public static int? GetIDByName(LoginUser loginUser, string versionNumber, int? productID)
    {
      int? result = null;
      if (productID != null)
      {
        ProductVersions productVersions = new ProductVersions(loginUser);
        productVersions.LoadByProductIDAndVersionNumber((int)productID, versionNumber);
        if (!productVersions.IsEmpty)
        {
          result = productVersions[0].ProductVersionID;
        } 
      }
      return result;
    }
  }

  public partial class ProductVersions 
  {

    public ProductVersion FindByImportID(string importID)
    {
      return FindByImportID(importID, null);
    }

    public ProductVersion FindByImportID(string importID, int? productID)
    {
      importID = importID.Trim().ToLower();
      foreach (ProductVersion productVersion in this)
      {
        if (productID == null || productVersion.ProductID == (int)productID)
        {
          if ((productVersion.ImportID != null && productVersion.ImportID.ToLower().Trim() == importID) || productVersion.VersionNumber.ToLower().Trim() == importID)
          {
            return productVersion;
          }
        }
      }
      return null;
    }

    public ProductVersion FindByVersionNumber(string versionNumber, int productID)
    {
      versionNumber = versionNumber.Trim().ToLower();
      foreach (ProductVersion productVersion in this)
      {
        if (productVersion.ProductID == productID)
        {
          if (productVersion.VersionNumber.ToLower().Trim() == versionNumber)
          {
            return productVersion;
          }
        }
      }
      return null;
    }

    partial void BeforeRowDelete(int productVersionID)
    {
      ProductVersion version = (ProductVersion) ProductVersions.GetProductVersion(LoginUser, productVersionID);
      Product product = (Product) Products.GetProduct(LoginUser, version.ProductID);
      string description = "Deleted version '" + version.VersionNumber + "' for product '" + product.Name + "'";
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.ProductVersions, productVersionID, description);
    }

    public string GetDateFormatNormal()
    {
        CultureInfo us = new CultureInfo(LoginUser.CultureInfo.ToString());
        return us.DateTimeFormat.ShortDatePattern;
    }

    partial void BeforeRowEdit(ProductVersion productVersion)
    {
      Product product = (Product)Products.GetProduct(LoginUser, productVersion.ProductID);
      ProductVersion oldVersion = (ProductVersion)GetProductVersion(LoginUser, productVersion.ProductVersionID);
      string description = "";
		string oldVersionString = oldVersion.ReleaseDate == null ? "empty" : ((DateTime)oldVersion.ReleaseDate).ToString(GetDateFormatNormal())
		;
      if (oldVersion.Description != productVersion.Description)
      {
        description = "Changed description for version '" + productVersion.VersionNumber + "' on product '" + product.Name + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.ProductVersions, productVersion.ProductVersionID, description);
      }

      if (oldVersion.IsReleased != productVersion.IsReleased)
      {
        description = "Changed release status for version '" + productVersion.VersionNumber + "' on product '" + product.Name + "' to " + productVersion.IsReleased.ToString();
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.ProductVersions, productVersion.ProductVersionID, description);
      }

      if (oldVersion.ReleaseDate != productVersion.ReleaseDate)
      {
			description = "Changed the release date for version '" + productVersion.VersionNumber + "' on product '" + product.Name + "' from " + oldVersionString + "' to '" + ((DateTime)productVersion.ReleaseDate).ToString(GetDateFormatNormal()) + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.ProductVersions, productVersion.ProductVersionID, description);
      }

      if (oldVersion.VersionNumber != productVersion.VersionNumber)
      {
        description = "Changed the version number for version '" + productVersion.VersionNumber + "' on product '" + product.Name + "' to '" + productVersion.VersionNumber + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.ProductVersions, productVersion.ProductVersionID, description);
      }

      if (oldVersion.ProductVersionStatusID != productVersion.ProductVersionStatusID)
      {
        string oldStatus = ((ProductVersionStatus)ProductVersionStatuses.GetProductVersionStatus(LoginUser, oldVersion.ProductVersionStatusID)).Name;
        string newStatus = ((ProductVersionStatus)ProductVersionStatuses.GetProductVersionStatus(LoginUser, productVersion.ProductVersionStatusID)).Name;
        description = "Changed the status for version '" + productVersion.VersionNumber + "' on product '" + product.Name + "' from " + oldStatus + "' to '" + newStatus + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.ProductVersions, productVersion.ProductVersionID, description);
      }
    }

    partial void AfterRowInsert(ProductVersion productVersion)
    {
      Product product = (Product)Products.GetProduct(LoginUser, productVersion.ProductID);
      string description = "Created new version '" + productVersion.VersionNumber + "' for product '" + product.Name + "'";
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.ProductVersions, productVersion.ProductVersionID, description);
    }

    public void LoadByProductAndTicket(int productID, int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT pv.* FROM ProductVersions pv WHERE pv.ProductID = @ProductID AND pv.ProductVersionID IN 
                                (SELECT DISTINCT op.ProductVersionID FROM OrganizationProducts op 
                                 LEFT JOIN OrganizationTickets ot ON ot.OrganizationID = op.OrganizationID
                                 LEFT JOIN Tickets t ON t.TicketID = ot.TicketID
                                 WHERE t.TicketID = @TicketID)
                                 ORDER BY pv.VersionNumber DESC";

        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadByProductAndCustomer(int productID, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT pv.* FROM ProductVersions pv WHERE pv.ProductID = @ProductID AND pv.ProductVersionID IN 
                                (SELECT DISTINCT op.ProductVersionID FROM OrganizationProducts op 
                                 WHERE op.OrganizationID = @OrganizationID)
                                 ORDER BY pv.VersionNumber DESC";

        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

		public void LoadByPortalProductAndCustomer(int productID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = @"SELECT * 
																FROM ProductVersions pv 
																WHERE pv.ProductID = @ProductID
																AND IsReleased = 1";

				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@ProductID", productID);
				Fill(command);
			}
		}

        public void LoadRestrictedProductVersions(int productID, int organizationID) {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT Distinct pv.*
                                        FROM dbo.[OrganizationProducts] as op
                                        left join dbo.ProductVersions as pv on op.ProductID = pv.ProductID
                                        where op.OrganizationID = @OrganizationID and op.ProductID = @ProductID and op.ProductVersionID is null and IsReleased = 1
                                        union
                                        SELECT Distinct pv.*
                                        FROM dbo.[OrganizationProducts] as op
                                        left join dbo.ProductVersions as pv on op.ProductID = pv.ProductID
                                        	and op.ProductVersionID = pv.ProductVersionID
                                        where op.OrganizationID = @OrganizationID and op.ProductID = @ProductID and op.ProductVersionID is not null and IsReleased = 1";

                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ProductID", productID);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

		public void LoadByProductID(int productID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ProductVersions WHERE ProductID = @ProductID ORDER BY VersionNumber DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        Fill(command);
      }
    }

    public void LoadByProductIDAndVersionNumber(int productID, string versionNumber)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ProductVersions WHERE ProductID = @ProductID AND VersionNumber = @VersionNumber";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@VersionNumber", versionNumber);
        Fill(command);
      }
    }

    public void LoadForGrid(int productID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT pv.*, pvs.Name as VersionStatus FROM ProductVersions pv LEFT JOIN ProductVersionStatuses pvs ON pv.ProductVersionStatusID = pvs.ProductVersionStatusID WHERE ProductID = @ProductID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        Fill(command, "ProductVersions,ProductVersionStatuses");
      }
    }

    public void LoadByParentOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT pv.*
          FROM ProductVersions pv
          LEFT JOIN Products p
          ON p.ProductID = pv.ProductID
          WHERE (p.OrganizationID = @OrganizationID)";

        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command, "ProductVersions");
      }
    }

    public void LoadAll(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT pv.* FROM ProductVersions pv LEFT JOIN Products p ON p.ProductID = pv.ProductID WHERE (p.OrganizationID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command, "ProductVersions,Products");
      }
    }

    public static void DeleteProductVersion(LoginUser loginUser, int productVersionID)
    {
      ProductVersions productVersions = new ProductVersions(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM OrganizationProducts WHERE (ProductVersionID = @ProductVersionID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
        productVersions.ExecuteNonQuery(command, "OrganizationProducts");

        command.CommandText = "UPDATE Tickets SET SolvedVersionID = null WHERE (SolvedVersionID = @ProductVersionID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
        productVersions.ExecuteNonQuery(command, "Tickets");

        command.CommandText = "UPDATE Tickets SET ReportedVersionID = null WHERE (ReportedVersionID = @ProductVersionID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
        productVersions.ExecuteNonQuery(command, "Tickets");
      }


      productVersions.LoadByProductVersionID(productVersionID);
      if (!productVersions.IsEmpty) productVersions[0].Delete();
      productVersions.Save();

    }

    public void ReplaceProductVersionStatus(int oldID, int newID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE ProductVersions SET ProductVersionStatusID = @newID WHERE (ProductVersionStatusID = @oldID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@oldID", oldID);
        command.Parameters.AddWithValue("@newID", newID);
        ExecuteNonQuery(command, "ProductVersions");
      }
    }

    public void LoadByImportID(string importID, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
          SELECT 
            * 
          FROM 
            ProductVersions pv 
            JOIN Products p 
              ON pv.ProductID = p.ProductID 
          WHERE 
            pv.ImportID = @ImportID 
            AND p.OrganizationID = @OrganizationID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ImportID", importID);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

        public static List<SqlDataRecord> GetSearchResultsList(string searchTerm, LoginUser loginUser)
        {
            SqlMetaData recordIDColumn = new SqlMetaData("recordID", SqlDbType.Int);
            SqlMetaData relevanceColumn = new SqlMetaData("relevance", SqlDbType.Int);

            SqlMetaData[] columns = new SqlMetaData[] { recordIDColumn, relevanceColumn };

            List<SqlDataRecord> result = new List<SqlDataRecord>();

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                SELECT 
				    pv.ProductVersionID,
                    1 AS RANK
                FROM
                    dbo.ProductVersions pv
                    JOIN dbo.Products p
                        ON pv.ProductID = p.ProductID
                WHERE 
                    p.OrganizationID = @organizationID";

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    command.CommandText = @"
                    SELECT 
				        pv.ProductVersionID,
                        match.RANK
                    FROM
                        dbo.ProductVersions pv
                        INNER JOIN dbo.Products p
                            ON pv.ProductID = p.ProductID
                        INNER JOIN CONTAINSTABLE(ProductVersions, (VersionNumber, Description), @searchTerm) AS match
                            ON pv.ProductVersionID = match.[KEY]
                    WHERE 
                        p.OrganizationID = @organizationID";
                }

                command.CommandText += Products.GetProductFamiliesRightsClause(loginUser);

                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@organizationID", loginUser.OrganizationID);
                command.Parameters.AddWithValue("@userID", loginUser.UserID);
                command.Parameters.AddWithValue("@searchTerm", string.Format("\"*{0}*\"", searchTerm));

                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                    command.Connection = connection;
                    command.Transaction = transaction;
                    SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                    while (reader.Read())
                    {
                        //crmLinkFieldId = (int?)reader["CrmFieldId"];
                        SqlDataRecord record = new SqlDataRecord(columns);
                        record.SetInt32(0, (int)reader["ProductVersionID"]);
                        record.SetInt32(1, (int)reader["RANK"]);
                        result.Add(record);
                    }
                }
            }

            return result;
        }
  }

  public class ProductVersionsSearch
  {
    public ProductVersionsSearch() { }
    public ProductVersionsSearch(ProductVersionsViewItem item)
    {
      productVersionID        = item.ProductVersionID;
      productID               = item.ProductID;
      productVersionStatusID  = item.ProductVersionStatusID;
      versionNumber           = item.VersionNumber;
      releaseDate             = item.ReleaseDate;
      isReleased              = item.IsReleased;
      description             = item.Description;
      dateCreated             = item.DateCreated;
      dateModified            = item.DateModified;
      creatorID               = item.CreatorID;
      modifierID              = item.ModifierID;
      versionStatus           = item.VersionStatus;
      productName             = item.ProductName;
      organizationID          = item.OrganizationID;
    }

    public int productVersionID { get; set; }
    public int productID { get; set; }
    public int productVersionStatusID { get; set; }
    public string versionNumber { get; set; }
    public DateTime? releaseDate { get; set; }
    public bool isReleased { get; set; }
    public string description { get; set; }
    public DateTime? dateCreated { get; set; }
    public DateTime? dateModified { get; set; }
    public int creatorID { get; set; }
    public int modifierID { get; set; }
    public string versionStatus { get; set; }
    public string productName { get; set; }
    public int organizationID { get; set; }
    public int openTicketCount { get; set; }
  }
}
