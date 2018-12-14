using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TicketType 
  {
    public static int? GetIDByName(LoginUser loginUser, string name, int? parentID)
    {
      TicketTypes type = new TicketTypes(loginUser);
      type.LoadByName(loginUser.OrganizationID, name);
      if (type.IsEmpty) return null;
      else return type[0].TicketTypeID;
    }
  }

  public partial class TicketTypes 
  {

    /// <summary>
    /// Loads all the ticket types for an organzation.  Ordered by Position.
    /// </summary>
    /// <param name="organizationID"></param>
    public void LoadByOrganizationID(int organizationID, string orderBy = "Position")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketTypes WHERE OrganizationID = @OrganizationID ORDER BY " + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }

		public void LoadPortalTypesByOrganizationID(int organizationID, string orderBy = "Position")
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "SELECT * FROM TicketTypes WHERE OrganizationID = @OrganizationID AND isVisibleOnPortal = 1 ORDER BY " + orderBy;
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("OrganizationID", organizationID);
				Fill(command);
			}
		}

        public void LoadHubTypesByCustomerHubID(int customerHubID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                    SELECT
	                    tt.*
                    FROM
	                    CustomerHubs h
	                    JOIN TicketTypes tt
		                    ON h.OrganizationID = tt.OrganizationID
                    WHERE
	                    h.CustomerHubID = @CustomerHubID
	                    AND tt.isVisibleOnPortal = 1
	                    AND 
	                    (
		                    h.ProductFamilyID IS NULL
		                    OR tt.ProductFamilyID IS NULL
		                    OR h.ProductFamilyID = tt.ProductFamilyID
	                    )
                    ORDER BY
	                    tt.Position";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@CustomerHubID", customerHubID);
                Fill(command);
            }
        }

        public void LoadByOrganizationID(int organizationID, ProductType productType)
    {
      LoadByOrganizationID(organizationID);
    }
    
   public void LoadByTicketTemplate(int organizationID, int ticketTemplateID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        TicketTemplate template = TicketTemplates.GetTicketTemplate(LoginUser, ticketTemplateID);
        int ticketTypeID = -1;
        if (template != null && template.TicketTypeID != null) ticketTypeID = (int)template.TicketTypeID;

        command.CommandText = "SELECT t.* FROM TicketTypes t WHERE OrganizationID = @OrganizationID AND (NOT EXISTS (SELECT * FROM TicketTemplates tt WHERE t.TicketTypeID = tt.TicketTypeID AND tt.TemplateType = 0) OR t.TicketTypeID = @TicketTypeID) ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        command.Parameters.AddWithValue("TicketTypeID", ticketTypeID);
        Fill(command);
      }
    }

    partial void BeforeDBDelete(int ticketTypeID)
    {
      SlaTriggers.DeleteByTicketTypeID(LoginUser, ticketTypeID);
    }

    public TicketType FindByName(string name)
    {
      foreach (TicketType ticketType in this)
      {
        if (ticketType.Name.ToUpper() == name.ToUpper())
        {
          return ticketType;
        }
      }
      return null;
    }

    public void LoadTopOne(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP 1 * FROM TicketTypes WHERE OrganizationID = @OrganizationID ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }

    public static TicketType GetTop(LoginUser loginUser, int organizationID)
    {
      TicketTypes items = new TicketTypes(loginUser);
      items.LoadTopOne(organizationID);
      if (items.IsEmpty) return null; else return items[0];
    }

    public void LoadByName(int organizationID, string name)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketTypes WHERE OrganizationID = @OrganizationID AND Name = @Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        command.Parameters.AddWithValue("Name", name);
        Fill(command);
      }
    }

    
  }
}
