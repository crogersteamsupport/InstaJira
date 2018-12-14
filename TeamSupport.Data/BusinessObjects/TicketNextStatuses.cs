using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TicketNextStatus 
  {

    public string Description
    {
      get { return (string)Row["Description"]; }
      set { Row["Description"] = value; }
    }

    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = value; }
    }
  }

  public partial class TicketNextStatuses
  {

    //BE SURE TO INCLUDE THE NAME AND DESCRIPTION FIELDS IN SELECT STATMENTS


    public void LoadNextStatuses(int currentStatusID)
    { 
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT tns.*, ts.Name, ts.Description FROM TicketNextStatuses tns LEFT JOIN TicketStatuses ts ON tns.NextStatusID = ts.TicketStatusID WHERE (tns.CurrentStatusID = @CurrentStatusID) ORDER BY tns.Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CurrentStatusID", currentStatusID);
        Fill(command, "TicketNextStatuses,TicketStatuses");
      }
    
    }

    public void LoadByPosition(int currentStatusID, int position)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketNextStatuses WHERE (CurrentStatusID = @CurrentStatusID) AND (Position = @Position)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CurrentStatusID", currentStatusID);
        command.Parameters.AddWithValue("Position", position);
        Fill(command);
      }

    }

    public void LoadAllPositions(int currentStatusID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketNextStatuses WHERE (CurrentStatusID = @CurrentStatusID) ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CurrentStatusID", currentStatusID);
        Fill(command);
      }
    }

    public void LoadAll(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"
SELECT * FROM TicketNextStatuses tns
LEFT JOIN TicketStatuses ts ON ts.TicketStatusID = tns.CurrentStatusID
WHERE (ts.OrganizationID = @OrganizationID) 
ORDER BY tns.CurrentStatusID, tns.Position
";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void ValidatePositions(int currentStatusID)
    {
      TicketNextStatuses ticketNextStatuses = new TicketNextStatuses(LoginUser);
      ticketNextStatuses.LoadAllPositions(currentStatusID);
      int i = 0;
      foreach (TicketNextStatus ticketNextStatus in ticketNextStatuses)
      {
        ticketNextStatus.Position = i;
        i++;
      }
      ticketNextStatuses.Save();
    }    

    public void MovePositionUp(int ticketNextStatusID)
    {
      TicketNextStatuses types1 = new TicketNextStatuses(LoginUser);
      types1.LoadByTicketNextStatusID(ticketNextStatusID);
      if (types1.IsEmpty || types1[0].Position < 1) return;

      TicketNextStatuses types2 = new TicketNextStatuses(LoginUser);
      types2.LoadByPosition(types1[0].CurrentStatusID, types1[0].Position - 1);
      if (!types2.IsEmpty)
      {
        types2[0].Position = types2[0].Position + 1;
        types2.Save();
      }

      types1[0].Position = types1[0].Position - 1;
      types1.Save();
    }

    public void MovePositionDown(int ticketNextStatusID)
    {
      TicketNextStatuses types1 = new TicketNextStatuses(LoginUser);
      types1.LoadByTicketNextStatusID(ticketNextStatusID);
      if (types1.IsEmpty || types1[0].Position >= GetMaxPosition(types1[0].CurrentStatusID)) return;

      TicketNextStatuses types2 = new TicketNextStatuses(LoginUser);
      types2.LoadByPosition(types1[0].CurrentStatusID, types1[0].Position + 1);
      if (!types2.IsEmpty)
      {
        types2[0].Position = types2[0].Position - 1;
        types2.Save();
      }

      types1[0].Position = types1[0].Position + 1;
      types1.Save();
    }

    public virtual int GetMaxPosition(int currentStatusID)
    {
      int position = -1;

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT MAX(Position) FROM TicketNextStatuses WHERE CurrentStatusID = @CurrentStatusID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CurrentStatusID", currentStatusID);
        object o = ExecuteScalar(command);
        if (o == DBNull.Value) return -1;
        position = (int)o;
      }
      return position;
    }

    public static int GetMaxPosition(LoginUser loginUser, int currentStatusID)
    {
      int position = -1;
      TicketNextStatuses statuses = new TicketNextStatuses(loginUser);

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT MAX(Position) FROM TicketNextStatuses WHERE CurrentStatusID = @CurrentStatusID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CurrentStatusID", currentStatusID);
        object o = statuses.ExecuteScalar(command);
        return (o == null || o == DBNull.Value) ? -1 : (int)o;
      }
    }

    public void AddNextStatus(TicketStatus currentStatus, TicketStatus nextStatus, int position)
    {
      TicketNextStatus ticketNextStatus = AddNewTicketNextStatus();
      ticketNextStatus.CurrentStatusID = currentStatus.TicketStatusID;
      ticketNextStatus.NextStatusID = nextStatus.TicketStatusID;
      ticketNextStatus.Position = position;
    }


  }
}
