using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
	public partial class CustomerHubView
	{
	}

	public partial class CustomerHubViews
	{
		public void loadAll()
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText =
					@"SELECT [CustomerHubViewID]
					  ,[Name]
					  ,[Route]
					  ,[IsActive]
					  ,[DateCreated]
					FROM [dbo].[CustomerHubViews]";
				command.CommandType = CommandType.Text;
				Fill(command);
			}
		}

		public void LoadByCustomerHubRouteName(string routeName)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "SET NOCOUNT OFF; SELECT [CustomerHubViewID], [Name], [Route], [IsActive], [DateCreated] FROM [dbo].[CustomerHubViews] WHERE ([Route] = @Route);";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("Route", routeName);
				Fill(command);
			}
		}
	}

}
