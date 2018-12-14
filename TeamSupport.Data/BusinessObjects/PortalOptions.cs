using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class PortalOption
  {
  }
  
  public partial class PortalOptions
  {
    public static string ValidatePortalNameChars(string name)
    {

      StringBuilder builder = new StringBuilder();

      foreach (char c in name)
      {
        if (Char.IsLetterOrDigit(c)) builder.Append(c);
      }

      return builder.ToString();
    
    }


    public void LoadByPortalName(string portalName)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM PortalOptions WHERE PortalName = @PortalName";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@PortalName", portalName);
        Fill(command);
      }
    }


  }
  
}
