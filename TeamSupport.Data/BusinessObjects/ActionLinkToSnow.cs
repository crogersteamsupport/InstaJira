using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.Data
{
	public partial class ActionLinkToSnow
	{
		public void GetByActionID(int actionID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText =
				@"SELECT s.* 
          FROM 
            Actions a
            LEFT JOIN ActionLinkToSnow s
              ON a.ActionID = s.ActionID
          WHERE
            a.ActionID = @actionId
			AND s.Id IS NOT NULL
        ";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@actionId", actionID);

				Fill(command, "ActionLinkToSnow");
			}
		}

		public void GetByActionAppID(string actionAppID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText =
				@"SELECT * 
          FROM 
            ActionLinkToSnow
          WHERE
            AppId = @actionAppId
        ";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@actionAppId", actionAppID);

				Fill(command, "ActionLinkToSnow");
			}
		}
	}
}
