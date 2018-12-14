using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
	public partial class KBRating
	{
	}

	public partial class KBRatings
	{
		public virtual void LoadByTicketID(int ticketID, int userID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "SET NOCOUNT OFF; SELECT [KBRatingID], [TicketID], [UserID], [IP], [Rating], [DateUpdated], [Comment] FROM [dbo].[KBRatings] WHERE ([TicketID] = @TicketID AND [UserID] = @UserID);";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@TicketID", ticketID);
				command.Parameters.AddWithValue("@UserID", userID);
				Fill(command);
			}
		}

		public virtual void LoadByTicketID(int ticketID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "SET NOCOUNT OFF; SELECT [KBRatingID], [TicketID], [UserID], [IP], [Rating], [DateUpdated], [Comment] FROM [dbo].[KBRatings] WHERE ([TicketID] = @TicketID);";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@TicketID", ticketID);
				Fill(command);
			}
		}

	}

}
