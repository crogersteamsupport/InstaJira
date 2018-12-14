using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TicketAutomationAction
  {
  }
  
  public partial class TicketAutomationActions
  {
    public void LoadByTrigger(int triggerID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketAutomationActions WHERE TriggerID=@TriggerID";
        command.Parameters.AddWithValue("@TriggerID", triggerID);
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }
  }
  
}
