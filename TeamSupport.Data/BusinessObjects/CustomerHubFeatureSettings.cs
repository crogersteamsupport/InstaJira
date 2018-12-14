using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CustomerHubFeatureSetting
  {
  }
  
  public partial class CustomerHubFeatureSettings
  {
		public void LoadByCustomerHubID(int customerHubID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "SELECT * FROM [CustomerHubFeatureSettings] WHERE CustomerHubID = @CustomerHubID";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@CustomerHubID", customerHubID);
				Fill(command);
			}
		}

        public static void DeleteByCustomerHubID(LoginUser loginUser, int customerHubID)
        {
            CustomerHubs customerHubs = new CustomerHubs(loginUser);

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "Delete FROM [CustomerHubFeatureSettings] WHERE CustomerHubID = @CustomerHubID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@CustomerHubID", customerHubID);
                customerHubs.ExecuteNonQuery(command, "CustomerHubFeatureSettings");
            }
        }
    }
  
}
