using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class OrganizationsViewItem
  {
  }
  
  public partial class OrganizationsView
  {
    public void LoadByParentID(int parentID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM OrganizationsView WHERE (ParentID = @ParentID) ORDER BY Name ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ParentID", parentID);
        Fill(command);
      }
    }

    public void LoadOneByParentID(int parentID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP 1 * FROM OrganizationsView WHERE (ParentID = @ParentID) ORDER BY Name ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ParentID", parentID);
        Fill(command);
      }
    }

    public void LoadByParentID(int parentID, bool includeCustomFields, string orderBy = "Name", int? limitNumber = null)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string limit = string.Empty;
        if (limitNumber != null)
        {
          limit = "TOP " + limitNumber.ToString();
        }
        string sql = "SELECT " + limit + " * FROM OrganizationsView ov WHERE (ParentID = @ParentID) ORDER BY " + orderBy;
        if (includeCustomFields) sql = InjectCustomFields(sql, "ov.OrganizationID", ReferenceType.Organizations);
        command.CommandText = sql;
        command.CommandType = CommandType.Text;
        command.CommandTimeout = 300;
        command.Parameters.AddWithValue("@ParentID", parentID);
        Fill(command);
      }
    }

		public void LoadByParentID(int parentID, bool includeCustomFields, NameValueCollection filters, int? pageNumber = null, int? pageSize = null, string orderBy = "Name")
		{
			this.LoadOneByParentID(parentID);

			if (this.Count > 0)
			{
				using (SqlCommand command = new SqlCommand())
				{
					string sql = BuildLoadByParentIDSql(string.Empty, parentID, orderBy, filters, command.Parameters);
					sql = DataUtils.AddPaging(sql, pageSize, pageNumber, command);

					if (includeCustomFields)
					{
						sql = InjectCustomFields(sql, "OrganizationsView.OrganizationID", ReferenceType.Organizations);
					}

					command.CommandText = sql;
					command.CommandType = CommandType.Text;
					this.DeleteAll();
					Fill(command);
				}
			}
		}

		private string BuildLoadByParentIDSql(string limit, int parentID, string orderBy, NameValueCollection filters, SqlParameterCollection filterParameters)
		{
			StringBuilder whereClauses = new StringBuilder();
			StringBuilder result = new StringBuilder();

			result.Append("SELECT " + limit + " * ");
			result.Append("FROM OrganizationsView ");
			result.Append("WHERE ParentID = @ParentID " + DataUtils.BuildWhereClausesFromFilters(this.LoginUser, this, parentID, filters, ReferenceType.Organizations, "OrganizationID", null, ref filterParameters) + " ");
			result.Append("ORDER BY " + orderBy);

			filterParameters.AddWithValue("@ParentID", parentID);

			return result.ToString();
		}

		public void LoadByTicketID(int ticketID, string orderBy = "Name")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT ov.* FROM OrganizationsView ov LEFT JOIN OrganizationTickets ot ON ot.OrganizationID = ov.OrganizationID WHERE ot.TicketID = @TicketID ORDER BY " + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadByProductID(int productID, string orderBy = "Name")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = 
          @"
            SELECT 
              DISTINCT
              ov.* 
            FROM 
              OrganizationsView ov 
              JOIN OrganizationProducts op 
                ON op.OrganizationID = ov.OrganizationID 
            WHERE 
              op.ProductID = @ProductID 
            ORDER BY 
          " + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        Fill(command);
      }
    }
    public void LoadByVersionID(int versionID, string orderBy = "Name")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT 
          ov.* 
        FROM
          OrganizationsView ov 
        WHERE 
	        ov.OrganizationID IN
	        (
		        SELECT
			        op.OrganizationID
		        FROM
			        OrganizationProducts op
		        WHERE
			        op.ProductVersionID = @VersionID 
          )
        ORDER BY " + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@VersionID", versionID);
        Fill(command);
      }
    }

    public void LoadForIndexing(int organizationID, int max, bool isRebuilding)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string text = @"
        SELECT TOP {0} OrganizationID
        FROM OrganizationsView ov WITH(NOLOCK)
        WHERE ov.NeedsIndexing = 1
        AND ov.ParentID = @OrganizationID
        ORDER BY DateModified DESC";

        if (isRebuilding)
        {
          text = @"
        SELECT OrganizationID
        FROM OrganizationsView ov WITH(NOLOCK)
        WHERE ov.ParentID = @OrganizationID
        ORDER BY DateModified DESC";
        }

        command.CommandText = string.Format(text, max.ToString());
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

	public OrganizationsViewItem LoadByOrganizationIDBySproc(int organizationId)
	{
		using (SqlCommand command = new SqlCommand())
		{
			command.CommandText = "OrganizationsViewGet";
			command.CommandType = CommandType.StoredProcedure;
			command.Parameters.AddWithValue("OrganizationID", organizationId);
			Fill(command);
		}

		if (IsEmpty)
			return null;
		else
			return this[0];
	}
  }
  public class CustomerSearchCompany
  {
    public CustomerSearchCompany() { }
    public CustomerSearchCompany(OrganizationsViewItem item)
    {
      organizationID = item.OrganizationID;
      name = item.Name;
      website = item.Website;
      isPortal = item.HasPortalAccess;
    }

    public int organizationID { get; set; }
    public string name { get; set; }
    public string website { get; set; }
    public bool isPortal { get; set; }
    public int openTicketCount { get; set; }
    public CustomerSearchPhone[] phones { get; set; }
  }  
}
