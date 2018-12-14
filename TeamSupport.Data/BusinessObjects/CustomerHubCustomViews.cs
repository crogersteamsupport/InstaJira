using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CustomerHubCustomView
  {
  }
  
  public partial class CustomerHubCustomViews
  {
		public void loadAllByHubID(int CustomerHubID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "SET NOCOUNT OFF; SELECT * FROM [dbo].[CustomerHubCustomViews] WHERE ([CustomerHubID] = @CustomerHubID)";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@CustomerHubID", CustomerHubID);
				Fill(command);
			}
		}
		public void loadByHubViewID(int CustomerHubID, int CustomerHubViewID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "SET NOCOUNT OFF; SELECT * FROM [dbo].[CustomerHubCustomViews] WHERE ([CustomerHubID] = @CustomerHubID and [CustomerHubViewID] = @CustomerHubViewID)";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@CustomerHubID", CustomerHubID);
				command.Parameters.AddWithValue("@CustomerHubViewID", CustomerHubViewID);
				Fill(command);
			}
		}

        public static void DeleteByCustomerHubID(LoginUser loginUser, int customerHubID)
        {
            CustomerHubs customerHubs = new CustomerHubs(loginUser);

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "Delete FROM dbo.CustomerHubCustomViews WHERE CustomerHubID = @CustomerHubID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@CustomerHubID", customerHubID);
                customerHubs.ExecuteNonQuery(command, "CustomerHubCustomViews");
            }
        }
    }
  
}
