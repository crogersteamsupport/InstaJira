using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class SlaPausedDay
    {
    }
  
    public partial class SlaPausedDays
    {
        public void LoadByTriggerID(int triggerId)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM SlaPausedDays WHERE SlaTriggerId = @SlaTriggerId";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@SlaTriggerId", triggerId);
                Fill(command);
            }
        }
    }
}
