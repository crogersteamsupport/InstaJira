using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class EmailAddress
  {
  }
  
  public partial class EmailAddresses
  {
    public void LoadByRefID(int refID, ReferenceType referenceType)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT 
            *
        FROM
            EmailAddresses
        WHERE 
            RefID = @RefID
            AND RefType = @RefType";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefID", refID);
        command.Parameters.AddWithValue("@RefType", referenceType);
        Fill(command, "EmailAddresses");
      }
    }
  }
  
}
