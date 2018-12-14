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
  public partial class ContactsViewItem
  {
  }
  
  public partial class ContactsView
  {
	public void LoadByParentOrganizationID(int organizationID, string orderBy = "LastName, FirstName", int? limitNumber = null, bool isCustomer = false)
	{
		using (SqlCommand command = new SqlCommand())
		{
			string limit = string.Empty;

			if (limitNumber != null)
			{
				limit = "TOP " + limitNumber.ToString();
			}

			command.CommandText = "SELECT " + limit + " * FROM ContactsView WHERE " + (isCustomer ? "OrganizationID" : "OrganizationParentID") + " = @OrganizationID AND (MarkDeleted = 0) ORDER BY " + orderBy;
			command.CommandText = InjectCustomFields(command.CommandText, "UserID", ReferenceType.Contacts);
			command.CommandType = CommandType.Text;
			command.Parameters.AddWithValue("@OrganizationID", organizationID);
			Fill(command);
		}
	}

	public void LoadOneByParentOrganizationID(int organizationParentId)
	{
		using (SqlCommand command = new SqlCommand())
		{
			command.CommandText = "SELECT TOP 1 * FROM ContactsView WHERE (organizationParentId = @OrganizationParentId)";
			command.CommandType = CommandType.Text;
			command.Parameters.AddWithValue("@OrganizationParentId", organizationParentId);

			Fill(command);
		}
	}

		public void LoadByParentOrganizationID(int organizationParentId, NameValueCollection filters, int? pageNumber = null, int? pageSize = null, string orderBy = "LastName, FirstName", int? limitNumber = null, bool isCustomer = false, bool useMaxDop = false)
		{
			//Get the column names, this row will be deleted before getting the actual data
			this.LoadOneByParentOrganizationID(organizationParentId);

			using (SqlCommand command = new SqlCommand())
			{
				string limit = string.Empty;

				if (limitNumber != null)
				{
					limit = "TOP " + limitNumber.ToString();
				}

				string sql = BuildLoadByParentOrganizationIdSql(limit, organizationParentId, orderBy, filters, command.Parameters, isCustomer, useMaxDop);
				sql = InjectCustomFields(sql, "UserID", ReferenceType.Contacts);
				sql = DataUtils.AddPaging(sql, pageSize, pageNumber, command);
				command.CommandType = CommandType.Text;
				command.CommandText = sql;
				command.Parameters.AddWithValue("@OrganizationParentId", organizationParentId);

                //Hack: ContactsView has the column Email size of nvarchar(1024). We are applying this here specifically for this column, the better way should be to do it for all everytime but that might be a massive change to the base code for the data processing.
                bool hasEmailParameter = (filters.AllKeys.Where(p => p.ToLower() == "email").Any() || filters.AllKeys.Where(p => p.ToLower().Contains("email[")).Any()) && command.Parameters.Contains("@email");

                if (hasEmailParameter)
                {
                    command.Parameters["@email"].Size = 1024;
                }

                this.DeleteAll();

				Fill(command);
			}
		}

    public void LoadByOrganizationID(int organizationID, string orderBy = "LastName, FirstName")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ContactsView WHERE OrganizationID = @OrganizationID AND (MarkDeleted = 0) ORDER BY " + orderBy;
        command.CommandText = InjectCustomFields(command.CommandText, "UserID", ReferenceType.Contacts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
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
		public string BuildLoadByParentOrganizationIdSql(string limit, int organizationParentId, string orderBy, NameValueCollection filters, SqlParameterCollection filterParameters, bool isCustomer = false, bool useMaxDop = false)
		{
			StringBuilder result = new StringBuilder();

			//check if it has the PhoneNumber filter by itself and also if it has a square bracket for the contains, lt, gt,... extra operators
			bool hasPhoneNumberFilter = filters.AllKeys.Where(p => p.ToLower() == "phonenumber").Any() || filters.AllKeys.Where(p => p.ToLower().Contains("phonenumber[")).Any();

			//If phoneNumber filter is used then SELECT needs to be DISTINCT to avoid identity exceptions on the DataTable when a contact has multiple phonenumbers (one-to-many relationship between ContactsView and PhoneNumbers)
			if (hasPhoneNumberFilter)
			{
				result.Append("SELECT DISTINCT " + limit + " ContactsView.* ");
			}
			else
			{
				result.Append("SELECT " + limit + " * ");
			}
		
			result.Append("FROM ContactsView ");

			if (hasPhoneNumberFilter)
			{
				result.Append("LEFT JOIN PhoneNumbers ON ContactsView.UserID = PhoneNumbers.RefID ");
			}

			result.Append("WHERE " + (isCustomer ? "OrganizationID" : "OrganizationParentID") + " = @OrganizationParentId AND (MarkDeleted = 0) ");
			result.Append(DataUtils.BuildWhereClausesFromFilters(this.LoginUser, this, organizationParentId, filters, ReferenceType.Contacts, "UserID", null, ref filterParameters) + " ");
			result.Append("ORDER BY " + orderBy);

            if (useMaxDop)
            {
                result.Append(" OPTION (MAXDOP 1);");
            }

			return result.ToString();
		}

		public void LoadByTicketID(int ticketID, string orderBy = "LastName, FirstName")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT cv.* FROM ContactsView cv LEFT JOIN UserTickets ut ON ut.UserID = cv.UserID WHERE ut.TicketID = @TicketID AND (cv.MarkDeleted = 0) ORDER BY " + orderBy;
        command.CommandText = InjectCustomFields(command.CommandText, "cv.UserID", ReferenceType.Contacts);

                // optimization to reduce load time for custom values
        command.CommandText = @"Select cvs.CustomValue, cvs.CustomFieldID, RefID
            INTO #CustomValues
            FROM ContactsView cv
            JOIN CustomValues cvs on cvs.RefID = cv.UserID
            JOIN CustomFields cf on cf.CustomFieldID = cvs.CustomFieldID
            LEFT JOIN UserTickets ut ON ut.UserID = cv.UserID
            WHERE ut.TicketID = " + ticketID + " AND cf.RefType = 32 AND cf.OrganizationID = " + LoginUser.OrganizationID + "; " + command.CommandText.Replace("CustomValues", "#CustomValues");

        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        //Fill(command);
        Fill(command,"", false);
      }
    }

         public ContactsViewItem FindBySalesForceID(string salesForceID, string organizationSalesForceID)
    {
      foreach (ContactsViewItem contactsViewItem in this)
      {
        string contactsViewItemOrganizationSalesForceID = Organizations.GetOrganization(this.LoginUser, contactsViewItem.OrganizationID).CRMLinkID;
        if (contactsViewItemOrganizationSalesForceID != null)
        {
          if (contactsViewItem.SalesForceID != null && contactsViewItem.SalesForceID.Trim().ToLower() == salesForceID.Trim().ToLower())
          {
            if (contactsViewItemOrganizationSalesForceID == organizationSalesForceID)
            {
              return contactsViewItem;
            }
          }
        }
      }
      return null;
    }

    public ContactsViewItem FindByEmail(string email, string organizationSalesForceID)
    {
      foreach (ContactsViewItem contactsViewItem in this)
      {
        string contactsViewItemOrganizationSalesForceID = Organizations.GetOrganization(this.LoginUser, contactsViewItem.OrganizationID).CRMLinkID;
        if (contactsViewItemOrganizationSalesForceID != null)
        {
          if (contactsViewItem.Email.Trim().ToLower() == email.Trim().ToLower())
          {
            if (contactsViewItemOrganizationSalesForceID == organizationSalesForceID)
            {
              return contactsViewItem;
            }
          }
        }
      }
      return null;
    }

    public void LoadByTicketIDOrderedByDateCreated(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT cv.* FROM ContactsView cv LEFT JOIN UserTickets ut ON ut.UserID = cv.UserID WHERE ut.TicketID = @TicketID AND (cv.MarkDeleted = 0) ORDER BY ut.DateCreated ";
        command.CommandText = InjectCustomFields(command.CommandText, "cv.UserID", ReferenceType.Contacts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadNameAndIdByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT u.UserID, u.FirstName + ' ' + u.LastName AS Name FROM [Users] u LEFT JOIN UserTickets ut ON ut.UserID = u.UserID WHERE ut.TicketID = @TicketID AND (u.MarkDeleted = 0)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadSentToSalesForce(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT cv.* FROM ContactsView cv LEFT JOIN UserTickets ut ON ut.UserID = cv.UserID WHERE ut.SentToSalesForce = 1 AND ut.TicketID = @TicketID AND (cv.MarkDeleted = 0) ORDER BY ut.DateCreated ";
        command.CommandText = InjectCustomFields(command.CommandText, "cv.UserID", ReferenceType.Contacts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadForIndexing(int organizationID, int max, bool isRebuilding)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string text = @"
        SELECT TOP {0} cv.UserID
        FROM ContactsView cv WITH(NOLOCK)
        JOIN Organizations o WITH(NOLOCK)
          ON cv.OrganizationID = o.OrganizationID
        WHERE cv.NeedsIndexing = 1
        AND cv.MarkDeleted = 0
        AND o.ParentID = @OrganizationID
        ORDER BY cv.DateModified DESC";

        if (isRebuilding)
        {
          text = @"
        SELECT cv.UserID
        FROM ContactsView cv WITH(NOLOCK)
        JOIN Organizations o WITH(NOLOCK)
          ON cv.OrganizationID = o.OrganizationID
        WHERE o.ParentID = @OrganizationID
        AND cv.MarkDeleted = 0
        ORDER BY cv.DateModified DESC";
        }

        command.CommandText = string.Format(text, max.ToString());
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByCustomerInsightsNewOrModifiedByDate(DateTime lastProcessed, int waitBeforeNewUpdate)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT ContactsView.*
FROM
	ContactsView WITH(NOLOCK)
	JOIN Organizations WITH(NOLOCK)
		ON ContactsView.organizationParentId = Organizations.organizationId
	LEFT JOIN FullContactUpdates WITH(NOLOCK)
		ON ContactsView.userID = FullContactUpdates.userId
WHERE
	ContactsView.isActive = 1
	AND Organizations.IsCustomerInsightsActive = 1
	AND ContactsView.organizationParentId != 1
	AND
	(
		--recently updated, never processed
		(
			(
				ContactsView.dateCreated > @lastProcessed
				OR ContactsView.dateModified > @lastProcessed
			)
			AND FullContactUpdates.id IS NULL
		)
		--recently updated, processed more than @waitBeforeNewUpdate ago
		OR
		(
			(
				ContactsView.dateCreated > @lastProcessed
				OR ContactsView.dateModified > @lastProcessed
			)
			AND FullContactUpdates.id IS NOT NULL
			AND FullContactUpdates.dateModified <= DATEADD(HOUR, @waitBeforeNewUpdate * -1, @lastProcessed)
		)
		--updated, skipped because at that time was processed recently, but now it has been more than @waitBeforeNewUpdate
		OR
		(
			(
				ContactsView.dateCreated > DATEADD(HOUR, @waitBeforeNewUpdate * -1, @lastProcessed)
				OR ContactsView.dateModified > DATEADD(HOUR, @waitBeforeNewUpdate * -1, @lastProcessed)
			)
			AND FullContactUpdates.id IS NOT NULL
			AND FullContactUpdates.dateModified <= DATEADD(HOUR, @waitBeforeNewUpdate * -1, @lastProcessed)
		)
	)
ORDER BY ContactsView.dateModified";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@lastProcessed", lastProcessed);
        command.Parameters.AddWithValue("@waitBeforeNewUpdate", waitBeforeNewUpdate);

        Fill(command);
      }
    }

    public void LoadByCustomerInsightsByContactTotalTickets(int waitBeforeNewUpdate, int? top = null)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string sql = @"SELECT {0} ContactsView.*
FROM
	ContactsView WITH(NOLOCK)
	JOIN (
		SELECT COUNT(1) AS Total, UserTickets.userId
		FROM
			TicketsView WITH(NOLOCK)
			JOIN UserTickets WITH(NOLOCK)
				ON TicketsView.ticketId = UserTickets.ticketId
		WHERE
			TicketsView.isClosed = 0
		GROUP BY
			UserTickets.userId
		) AS TicketCount
		ON ContactsView.userId = TicketCount.userId
  LEFT JOIN FullContactUpdates WITH(NOLOCK)
    ON ContactsView.userID = FullContactUpdates.userId
  JOIN Organizations AS Parent WITH(NOLOCK)
		ON ContactsView.OrganizationParentId = Parent.organizationId
WHERE
  Parent.isCustomerInsightsActive = 1
  AND (FullContactUpdates.id IS NULL
  OR DATEADD(HOUR, @waitBeforeNewUpdate, FullContactUpdates.dateModified) < GETDATE())
ORDER BY
	TicketCount.Total DESC,
	ContactsView.lastName";
        command.CommandText = string.Format(sql, top == null ? "" : "TOP " + top.ToString());
        command.Parameters.AddWithValue("@waitBeforeNewUpdate", waitBeforeNewUpdate);

        Fill(command);
      }
    }
  }

  public class CustomerSearchContact
  {
    public CustomerSearchContact() { }
    public CustomerSearchContact(ContactsViewItem item)
    {
      userID = item.UserID;
      organizationID = item.OrganizationID;
      email = item.Email;
      title = item.Title;
      organization = item.Organization;
      fName = item.FirstName;
      lName = item.LastName;
      isPortal = item.IsPortalUser;
    }

    public int userID { get; set; }
    public int organizationID { get; set; }
    public string email { get; set; }
    public string title { get; set; }
    public string organization { get; set; }
    public string fName { get; set; }
    public string lName { get; set; }
    public bool isPortal { get; set; }
    public int openTicketCount { get; set; }
    public CustomerSearchPhone[] phones { get; set; }
  }
  
}
