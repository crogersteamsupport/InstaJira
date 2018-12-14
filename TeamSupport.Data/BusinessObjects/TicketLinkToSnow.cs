using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
	public partial class TicketLinkToSnow
	{
		public void LoadByTicketID(int ticketID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText =
				@"SELECT * 
				FROM 
					TicketLinkToSnow
				WHERE 
					TicketID = @TicketID
				";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@TicketID", ticketID);
				command.CommandTimeout = 60;

				Fill(command);
			}
		}

		public void LoadByAppId(string appId)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText =
				@"SELECT * 
				FROM 
					TicketLinkToSnow
				WHERE 
					AppId = @AppId
				";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@AppId", appId);
				command.CommandTimeout = 60;

				Fill(command);
			}
		}

		public virtual TicketLinkToSnowItem AddNewTicketLinkToSnowItem(int ticketID)
		{
			if (Table.Columns.Count < 1) LoadColumns("TicketLinkToSnow");
			DataRow row = Table.NewRow();
			row["TicketID"] = ticketID;
			row["Sync"] = false;
			Table.Rows.Add(row);
			return new TicketLinkToSnowItem(row, this);
		}
	}
}
