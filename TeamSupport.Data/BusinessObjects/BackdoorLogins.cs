using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class BackdoorLogin
  {
  }
  
  public partial class BackdoorLogins
  {

	  public void LoadByToken(string token)
	  {
		  using (SqlCommand command = new SqlCommand())
		  {
			  command.CommandText = "SELECT [BackdoorLoginID], [UserID], [Token], [DateIssued], [ContactID] FROM [dbo].[BackdoorLogins] WHERE ([Token] = @Token);";
			  command.CommandType = CommandType.Text;
			  command.Parameters.AddWithValue("Token", token);
			  Fill(command);
		  }
	  }
  }
  
}
