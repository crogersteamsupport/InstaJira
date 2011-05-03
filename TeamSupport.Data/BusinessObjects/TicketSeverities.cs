using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TicketSeverity 
  {
  }

  public partial class TicketSeverities 
  {
    public TicketSeverity FindByName(string name)
    {
      foreach (TicketSeverity ticketSeverity in this)
      {
        if (ticketSeverity.Name == name)
        {
          return ticketSeverity;
        }
      }
      return null;
    }

    partial void BeforeDBDelete(int ticketSeverityID)
    {
      SlaTriggers.DeleteByTicketSeverityID(LoginUser, ticketSeverityID);
    }
    
    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketSeverities WHERE OrganizationID = @OrganizationID ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadNotSlaTriggers(int organizationID, int slaLevelID, int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT * 
FROM TicketSeverities 
WHERE OrganizationID = @OrganizationID 
AND TicketSeverityID NOT IN 
(
  SELECT TicketSeverityID FROM SlaTriggers
  WHERE TicketTypeID = @TicketTypeID
  AND SlaLevelID = @SlaLevelID
)
ORDER BY Position";

        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        command.Parameters.AddWithValue("TicketTypeID", ticketTypeID);
        command.Parameters.AddWithValue("SlaLevelID", slaLevelID);
        Fill(command);
      }
    
    }

    public void LoadTopOne(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP 1 * FROM TicketSeverities WHERE OrganizationID = @OrganizationID ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }

    public static TicketSeverity GetTop(LoginUser loginUser, int organizationID)
    {
      TicketSeverities items = new TicketSeverities(loginUser);
      items.LoadTopOne(organizationID);
      if (items.IsEmpty) return null; else return items[0];
    }


  }
}
