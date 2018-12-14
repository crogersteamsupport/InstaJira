using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TicketTypesViewItem
  {
  }
  
  public partial class TicketTypesView
  {
      public void LoadAllPositions(int organizationID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = "SELECT * FROM TicketTypesView WHERE (OrganizationID = @OrganizationID) ORDER BY Position";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("OrganizationID", organizationID);
              Fill(command);
          }
      }
  }
  
}
