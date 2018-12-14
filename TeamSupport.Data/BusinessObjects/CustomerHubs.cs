using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CustomerHub
  {
  }
  
  public partial class CustomerHubs
  {
		public void LoadByOrganizationID(int organizationID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = @"SELECT * FROM [CustomerHubs] WHERE OrganizationID = @OrganizationID 
                    ORDER BY CASE WHEN ProductFamilyID IS NULL THEN 0 ELSE 1 END, PortalName";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@OrganizationID", organizationID);
				Fill(command);
			}
		}

        public static int? LoadFirstNonProductAssociatedHubID(int organizationID)
        {
            CustomerHubs customerHubHelper = new CustomerHubs(LoginUser.Anonymous);
            customerHubHelper.LoadByOrganizationID(organizationID);

            CustomerHub returnHub = customerHubHelper.Where(hub => hub.ProductFamilyID == null && hub.IsActive == true).First();

            return returnHub?.CustomerHubID; 
        }


        public void LoadByHubName(string name, string cnameURL, bool isActive = true)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT 
	                                        *
                                        FROM [CustomerHubs] 
                                        WHERE 
	                                        (Lower(PortalName) = @Name 
	                                        OR Lower(CNameURL) = @cnameURL) 
	                                        AND IsActive = @IsActive
                                        ORDER BY
	                                        case when CNameURL = @cnameURL then 1 else 0 end + 
                                            case when PortalName = @Name  then 2 else 0 end";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@CNameURL", cnameURL);
                command.Parameters.AddWithValue("@IsActive", isActive);
                Fill(command);
            }
        }

        public void LoadByProductFamilyID(int productFamilyID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT * FROM [CustomerHubs] WHERE (ProductFamilyID = @ProductFamilyID AND IsActive = 1)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ProductFamilyID", productFamilyID);
                Fill(command);
            }
        }

        public void LoadByContactID(int userID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"Select Distinct CH.*
                                        from dbo.users as U
                                        inner join dbo.Organizations as O on U.OrganizationID = O.OrganizationID
                                        left join dbo.OrganizationProducts as OP on U.OrganizationID = OP.OrganizationID
                                        left join dbo.Products as P on P.ProductID = OP.ProductID
                                        inner join dbo.CustomerHubs as CH on O.ParentID = CH.OrganizationID
	                                        and (P.ProductFamilyID = CH.ProductFamilyID OR CH.ProductFamilyID is Null)
                                        Where UserID = @UserID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                Fill(command);
            }
        }

        public static void DeleteByCustomerHubID(LoginUser loginUser, int customerHubID)
        {
            CustomerHubs customerHubs = new CustomerHubs(loginUser);

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "Delete FROM dbo.CustomerHubs WHERE CustomerHubID = @CustomerHubID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@CustomerHubID", customerHubID);
                customerHubs.ExecuteNonQuery(command, "CustomerHubs");
            }
        }

    }
  
}
