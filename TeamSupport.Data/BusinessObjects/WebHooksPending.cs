using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;


namespace TeamSupport.Data
{
	public partial class WebHooksPending
	{
		public void LoadPending()
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = @"SELECT * FROM WebHooksPending WITH(NOLOCK) WHERE IsProcessing = 0 ORDER BY Id";
				command.CommandType = CommandType.Text;
				Fill(command);
			}
		}
	}
}
