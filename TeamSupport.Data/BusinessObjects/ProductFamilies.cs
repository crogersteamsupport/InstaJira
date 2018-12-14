using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class ProductFamily
    {
    }

    public partial class ProductFamilies
    {
        public void LoadBySearchTerm(string searchTerm, int start, int organizationID, int userID)
        {
            int end = start + 20;
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                SELECT
                  *
                FROM
                (
                  SELECT
                    *, 
                    ROW_NUMBER() OVER (ORDER BY Name) AS rownum  
                  FROM
                  (
                    SELECT
                      pf.*
                    FROM
                      ProductFamilies pf
                    WHERE 
                      pf.OrganizationID = @OrganizationID
                      AND (@SearchTerm = '' OR (pf.Name LIKE '%' + @SearchTerm + '%') OR (pf.Description LIKE '%' + @SearchTerm + '%'))
                      AND (
                        0 = (SELECT ProductFamiliesRights FROM Users WHERE UserID = @UserID)
                        OR pf.ProductFamilyID IN (SELECT ProductFamilyID FROM UserRightsProductFamilies WHERE UserID = @UserID)
                      )
                  ) as temp
                ) as results
                WHERE
                  rownum between @start and @end
				        ORDER BY
                  rownum ASC
                ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@SearchTerm", searchTerm);
                command.Parameters.AddWithValue("@start", start + 1);
                command.Parameters.AddWithValue("@end", end);
                command.Parameters.AddWithValue("@UserID", userID);
                Fill(command);
            }
        }

        public static void DeleteProductFamily(LoginUser loginUser, int productFamilyID)
        {
            ProductFamilies productFamilies = new ProductFamilies(loginUser);
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE Products SET ProductFamilyID = null WHERE ProductFamilyID = @ProductFamilyID AND OrganizationID = @OrganizationID";
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@ProductFamilyID", productFamilyID);
                command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
                productFamilies.ExecuteNonQuery(command, "Products");

                command.CommandText = "DELETE RecentlyViewedItems WHERE RefType = 44 AND RefID = @ProductFamilyID";
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@ProductFamilyID", productFamilyID);
                productFamilies.ExecuteNonQuery(command, "Products");

                command.CommandText = "DELETE ProductFamilies WHERE ProductFamilyID = @ProductFamilyID AND OrganizationID = @OrganizationID";
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@ProductFamilyID", productFamilyID);
                command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
                productFamilies.ExecuteNonQuery(command, "ProductFamilies");

                command.CommandText = "UPDATE KnowledgeBaseCategories SET ProductFamilyID = -1 WHERE ProductFamilyID = @ProductFamilyID AND OrganizationID = @OrganizationID";
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@ProductFamilyID", productFamilyID);
                command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
                productFamilies.ExecuteNonQuery(command, "ProductFamilies");

                command.CommandText = "UPDATE ForumCategories SET ProductFamilyID = -1 WHERE ProductFamilyID = @ProductFamilyID AND OrganizationID = @OrganizationID";
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@ProductFamilyID", productFamilyID);
                command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
                productFamilies.ExecuteNonQuery(command, "ProductFamilies");

                command.CommandText = "UPDATE CustomerHubs SET ProductFamilyID = NULL WHERE ProductFamilyID = @ProductFamilyID AND OrganizationID = @OrganizationID";
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@ProductFamilyID", productFamilyID);
                command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
                productFamilies.ExecuteNonQuery(command, "ProductFamilies");
            }


            productFamilies.LoadByProductFamilyID(productFamilyID);
            if (!productFamilies.IsEmpty) productFamilies[0].Delete();
            productFamilies.Save();

        }

        public void LoadByOrganizationID(int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM ProductFamilies WHERE OrganizationID = @OrganizationID ORDER BY Name";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        public void LoadByUserRights(int userID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                SELECT 
                    * 
                FROM 
                    ProductFamilies pf
                WHERE 
                    pf.ProductFamilyID IN 
                    (
                        SELECT 
                            urpf.ProductFamilyID 
                        FROM 
                            UserRightsProductFamilies urpf 
                        WHERE 
                            urpf.UserID = @UserID
                    )";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                Fill(command, "Organizations");
            }
        }

        public void LoadByLikeProductFamilyName(int parentID, string name)
        {
            LoadByLikeProductFamilyName(parentID, name, int.MaxValue, false);
        }

        public void LoadByLikeProductFamilyName(int organizationID, string name, int maxRows, bool filterByUserRights)
        {
            User user = Users.GetUser(LoginUser, LoginUser.UserID);
            bool doFilter = filterByUserRights && (ProductFamiliesRightType)user.ProductFamiliesRights == ProductFamiliesRightType.SomeFamilies;

            using (SqlCommand command = new SqlCommand())
            {
                StringBuilder text = new StringBuilder(@"
                SELECT
                    TOP (@MaxRows) * 
                FROM 
                    ProductFamilies 
                WHERE 
                    (OrganizationID = @OrganizationID) 
                    AND (@UseFilter=0 OR (ProductFamilyID IN (SELECT ProductFamilyID FROM UserRightsProductFamilies WHERE UserID = @UserID)))
                ");
                if (name.Trim() != "")
                {
                    text.Append(" AND ((Name LIKE '%'+@Name+'%') OR (Description LIKE '%'+@Name+'%')) ");
                }

                text.Append(" ORDER BY Name ");
                command.CommandText = text.ToString();
                command.CommandType = CommandType.Text;

                command.Parameters.AddWithValue("@Name", name.Trim());
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@MaxRows", maxRows);
                command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
                command.Parameters.AddWithValue("@UseFilter", doFilter);
                Fill(command);
            }
        }

        public void LoadByName(int organizationID, string name)
        {
            using (SqlCommand command = new SqlCommand())
            {
                StringBuilder text = new StringBuilder(@"
                SELECT
                    * 
                FROM 
                    ProductFamilies 
                WHERE 
                    OrganizationID = @OrganizationID
                    AND Name = @Name
                ");

                command.CommandText = text.ToString();
                command.CommandType = CommandType.Text;

                command.Parameters.AddWithValue("@Name", name.Trim());
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

		public void LoadByIds(List<int> productFamilyIds, int organizationId)
		{
			using (SqlCommand command = new SqlCommand())
			{
				StringBuilder text = new StringBuilder(@"
                SELECT
                    * 
                FROM 
                    ProductFamilies 
                WHERE 
                    OrganizationID = @OrganizationID
                    AND ProductFamilyID IN (" + string.Join(",", productFamilyIds.ToArray()) + ")");

				command.CommandText = text.ToString();
				command.CommandType = CommandType.Text;

				command.Parameters.AddWithValue("@OrganizationID", organizationId);
				Fill(command);
			}
		}

        public ProductFamily FindByName(string name)
        {
            foreach (ProductFamily productFamily in this)
            {
                if (productFamily.Name.Trim().ToLower() == name.Trim().ToLower())
                {
                    return productFamily;
                }
            }
            return null;
        }

        public ProductFamily FindByImportID(string importID)
        {
            foreach (ProductFamily productFamily in this)
            {
                if (productFamily.ImportID.Trim().ToLower() == importID.Trim().ToLower())
                {
                    return productFamily;
                }
            }
            return null;
        }
	}

}
