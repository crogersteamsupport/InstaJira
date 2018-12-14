using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class EmailTemplateTable
  {
  }
  
  public partial class EmailTemplateTables
  {
    public void LoadByTemplate(int emailTemplateID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM EmailTemplateTables WHERE EmailTemplateID = @EmailTemplateID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@EmailTemplateID", emailTemplateID);
        Fill(command);
      }
    }
  }
  
}
