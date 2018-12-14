using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TicketAutomationTriggerLogicItem
  {
  }
  
  public partial class TicketAutomationTriggerLogic
  {
    public void LoadByTrigger(int triggerID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT t.*, rtf.FieldName FROM TicketAutomationTriggerLogic t LEFT JOIN ReportTableFields rtf ON rtf.ReportTableFieldID = t.FieldID WHERE t.TriggerID = @TriggerID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TriggerID", triggerID);
        Fill(command);
      }
    }
  }
  
}
