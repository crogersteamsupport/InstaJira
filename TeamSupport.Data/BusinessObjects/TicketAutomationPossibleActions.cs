using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TicketAutomationPossibleAction
  {
  }
  
  public partial class TicketAutomationPossibleActions
  {
    public void LoadActive()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketAutomationPossibleActions WHERE Active=1";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }
  }
  
}
