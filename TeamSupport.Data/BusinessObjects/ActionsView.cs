using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ActionsViewItem
  {
    public TicketsViewItem GetTicket()
    {
      return TicketsView.GetTicketsViewItem(Collection.LoginUser, TicketID);
    }

    public string DisplayName
    {
      get
      {
        string title = "";

        switch (SystemActionTypeID)
        {
          case SystemActionType.Description: title = "Description"; break;
          case SystemActionType.Resolution: title = "Resolution"; break;
          case SystemActionType.Email: title = "Email: " + Name; break;
          case SystemActionType.UpdateRequest: title = "Update Request"; break;
          case SystemActionType.Chat: title = "Chat"; break;
          case SystemActionType.Reminder: title = "Reminder"; break;
          default:
            title = ActionType == "" ? "[No Action Type]" : ActionType;
            if (!string.IsNullOrEmpty(Name)) title += ": " + Name;
            break;
        }
        return title;
      }

    }

  }
  
  public partial class ActionsView
  {
    public void LoadByTicketID(int ticketID, bool onlyVisibleOnPortal, int? limitNumber = null)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string limit = string.Empty;
        if (limitNumber != null)
        {
          limit = "TOP " + limitNumber.ToString();
        }
        command.CommandType = CommandType.Text;
        if (onlyVisibleOnPortal)
          command.CommandText = "SELECT * FROM ActionsView WHERE TicketID = @TicketID AND IsVisibleOnPortal=1 ORDER BY DateCreated DESC";
        else
          command.CommandText = "SELECT " + limit + " * FROM ActionsView WHERE TicketID = @TicketID ORDER BY DateCreated DESC";

        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    
    }

    public void LoadKBByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandType = CommandType.Text;
        command.CommandText = "SELECT * FROM ActionsView WHERE TicketID = @TicketID AND IsKnowledgeBase=1 ORDER BY DateCreated ASC";
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }

    }

		public void LoadVisibleKBByTicketID(int ticketID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandType = CommandType.Text;
				command.CommandText = "SELECT * FROM ActionsView WHERE TicketID = @TicketID AND IsKnowledgeBase=1 and IsVisibleOnPortal = 1 ORDER BY DateCreated ASC";
				command.Parameters.AddWithValue("@TicketID", ticketID);
				Fill(command);
			}

		}

        public void LoadVisibleForumByTicketID(int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT * FROM ActionsView WHERE TicketID = @TicketID and IsVisibleOnPortal = 1 ORDER BY DateCreated ASC";
                command.Parameters.AddWithValue("@TicketID", ticketID);
                Fill(command);
            }

        }



        public void LoadLatestByTicket(int ticketID, bool onlyPublic)
    {
      using (SqlCommand command = new SqlCommand())
      {
        if (onlyPublic)
          command.CommandText = "SELECT TOP 1 a.* FROM ActionsView a WHERE a.TicketID = @TicketID AND IsVisibleOnPortal = 1 ORDER BY a.DateCreated DESC";
        else
          command.CommandText = "SELECT TOP 1 a.* FROM ActionsView a WHERE a.TicketID = @TicketID ORDER BY a.DateCreated DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadByTicketID(int ticketID, int? limitNumber = null)
    {
      LoadByTicketID(ticketID, false, limitNumber);
    }

		public void LoadPortalActionsByTicketID(int ticketID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandType = CommandType.Text;
				command.CommandText = @"SELECT * 
																FROM ActionsView
																WHERE TicketID = @TicketID
																AND IsVisibleOnPortal = 1
																ORDER BY DateCreated ASC";
				command.Parameters.AddWithValue("@TicketID", ticketID);
				Fill(command);
			}

		}
	}
  
}
