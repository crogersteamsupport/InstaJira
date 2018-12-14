using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ActionLinkToJiraItem
  {
  }
  
  public partial class ActionLinkToJira
  {
    //Changes to this method needs to be applied to Actions.LoadToPushToJira also.
    public void LoadToPushToJira(CRMLinkTableItem item, int ticketID)
    {
      string actionTypeFilter = "1 = 1";

      if (item.ActionTypeIDToPush != null)
      {
        actionTypeFilter = "a.ActionTypeID = @actionTypeID";
      }

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
        @"
          SELECT 
            j.* 
          FROM 
            Actions a
            JOIN ActionLinkToJira j
              ON a.ActionID = j.ActionID
          WHERE 
            a.SystemActionTypeID <> 1 
            AND a.TicketID = @ticketID
            AND " + actionTypeFilter + @"
            AND 
            (
              j.DateModifiedByJiraSync IS NULL
              OR a.DateModified > DATEADD(s, 2, j.DateModifiedByJiraSync)
            )
          ORDER BY 
            a.DateCreated ASC
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ticketID", ticketID);
        command.Parameters.AddWithValue("@DateModified", item.LastLink == null ? new DateTime(1753, 1, 1) : item.LastLinkUtc.Value.AddHours(-1));
        command.Parameters.AddWithValue("@actionTypeID", item.ActionTypeIDToPush == null ? -1 : item.ActionTypeIDToPush);

        Fill(command, "ActionLinkToJira");
      }
    }

    public ActionLinkToJiraItem FindByActionID(int actionID)
    {
      foreach (ActionLinkToJiraItem item in this)
      {
        if (item.ActionID == actionID)
        {
          return item;
        }
      }
      return null;
    }

    public void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
        @"
          SELECT 
            j.* 
          FROM 
            Actions a
            JOIN ActionLinkToJira j
              ON a.ActionID = j.ActionID
          WHERE 
            a.SystemActionTypeID <> 1 
            AND a.TicketID = @ticketID
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ticketID", ticketID);

        Fill(command, "ActionLinkToJira");
      }
    }

    public ActionLinkToJiraItem FindByJiraID(int? jiraID)
    {
      foreach (ActionLinkToJiraItem item in this)
      {
        if (item.JiraID == jiraID)
        {
          return item;
        }
      }
      return null;
    }

  }
  
}
