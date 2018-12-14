using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using dtSearch.Engine;

namespace TeamSupport.Data
{
    public partial class ProductVersionsViewItem
    {
    }

    public partial class ProductVersionsView
    {
        public void LoadByProductID(int productID, string orderBy = "VersionNumber DESC")
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM ProductVersionsView WHERE ProductID = @ProductID ORDER BY " + orderBy;
                command.CommandText = InjectCustomFields(command.CommandText, "ProductVersionID", ReferenceType.ProductVersions);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ProductID", productID);
                Fill(command);
            }
        }

        public void LoadByProductIDLimit(int productID, int start)
        {
            int end = start + 10;
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
        WITH OrderedVersion AS
        (
	        SELECT 
		        ProductVersionID, 
		        ROW_NUMBER() OVER (ORDER BY VersionNumber DESC) AS rownum
	        FROM 
		        ProductVersions 
	        WHERE 
		        ProductID = @ProductID 
        ) 
        SELECT 
          v.ProductVersionID,
          v.VersionNumber,
          v.ReleaseDate,
          v.IsReleased,
          v.VersionStatus
        FROM
          ProductVersionsView v
          JOIN OrderedVersion ov
            ON v.ProductVersionID = ov.ProductVersionID
        WHERE 
	        ov.rownum BETWEEN @start and @end
        ORDER BY
          v.VersionNumber DESC";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ProductID", productID);
                command.Parameters.AddWithValue("@start", start);
                command.Parameters.AddWithValue("@end", end);
                Fill(command);
            }
        }

        public void LoadByProductAndCustomer(int productID, int organizationID, string orderBy = "VersionNumber")
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT pv.* FROM ProductVersionsView pv WHERE pv.ProductID = @ProductID AND pv.ProductVersionID IN 
                                (SELECT DISTINCT op.ProductVersionID FROM OrganizationProducts op 
                                 WHERE op.OrganizationID = @OrganizationID) ORDER BY pv." + orderBy;
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ProductID", productID);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        public void LoadByProductAndCustomerReleased(int productID, int organizationID, string orderBy = "VersionNumber")
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT pv.* 
																FROM ProductVersionsView pv 
																WHERE pv.ProductID = @ProductID 
																	AND IsReleased = 1
																	AND pv.ProductVersionID IN 
																	(SELECT DISTINCT op.ProductVersionID 
																		FROM OrganizationProducts op 
																		WHERE op.OrganizationID = @OrganizationID) 
																	ORDER BY pv." + orderBy;
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ProductID", productID);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        public void LoadByProductAndReleased(int productID, string orderBy = "VersionNumber")
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT pv.* 
																FROM ProductVersionsView pv 
																WHERE pv.ProductID = @ProductID 
																	AND IsReleased = 1
																	ORDER BY pv." + orderBy;
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ProductID", productID);
                Fill(command);
            }
        }

        public void LoadForIndexing(int organizationID, int max, bool isRebuilding)
        {
            using (SqlCommand command = new SqlCommand())
            {
                string text = @"
        SELECT 
          TOP {0} 
          pvv.ProductVersionID
        FROM 
          ProductVersionsView pvv WITH(NOLOCK)
        WHERE 
          pvv.NeedsIndexing = 1
          AND pvv.OrganizationID= @OrganizationID
        ORDER BY 
          DateModified DESC";

                if (isRebuilding)
                {
                    text = @"
          SELECT 
            pvv.ProductVersionID
          FROM 
            ProductVersionsView pvv WITH(NOLOCK)
          WHERE pvv.OrganizationID= @OrganizationID
          ORDER BY 
            DateModified DESC";
                }

                command.CommandText = string.Format(text, max.ToString());
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        public void LoadRestrictedProductVersions(int productID, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"select DISTINCT PVV.* from
                                        dbo.ProductVersionsView as PVV 
                                        inner join dbo.OrganizationProducts as op on PVV.ProductID = op.ProductID
                                        left join dbo.ProductVersions as pv on PVV.ProductID = pv.ProductID
                                        where op.OrganizationID = @OrganizationID and op.ProductID = @ProductID and op.ProductVersionID is null and PVV.IsReleased = 1
                                        union
                                        select DISTINCT PVV.* from
                                        dbo.ProductVersionsView as PVV 
                                        inner join dbo.OrganizationProducts as op on PVV.ProductID = op.ProductID
                                        	and PVV.ProductVersionID = op.ProductVersionID
                                        where op.OrganizationID = @OrganizationID and op.ProductID = @ProductID and op.ProductVersionID is not null and PVV.IsReleased = 1";

                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ProductID", productID);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        public static List<SqlDataRecord> GetSearchResultsList(string searchTerm, LoginUser loginUser)
        {
            SqlMetaData recordIDColumn = new SqlMetaData("recordID", SqlDbType.Int);
            SqlMetaData relevanceColumn = new SqlMetaData("relevance", SqlDbType.Int);

            SqlMetaData[] columns = new SqlMetaData[] { recordIDColumn, relevanceColumn };

            List<SqlDataRecord> result = new List<SqlDataRecord>();

            SearchResults results = GetSearchProductVersionsResults(searchTerm, loginUser);

            List<int> items = new List<int>();
            for (int i = 0; i < results.Count; i++)
            {
                results.GetNthDoc(i);

                SqlDataRecord record = new SqlDataRecord(columns);
                record.SetInt32(0, int.Parse(results.CurrentItem.Filename));
                record.SetInt32(1, results.CurrentItem.ScorePercent);

                result.Add(record);
            }

            return result;
        }

        public static SearchResults GetSearchProductVersionsResults(string searchTerm, LoginUser loginUser)
        {
            Options options = new Options();
            options.TextFlags = TextFlags.dtsoTfRecognizeDates;
            using (SearchJob job = new SearchJob())
            {

                searchTerm = searchTerm.Trim();
                job.Request = searchTerm;
                //job.FieldWeights = "ArticleName: 1000";

                //StringBuilder conditions = new StringBuilder();
                //job.BooleanConditions = conditions.ToString();


                //job.MaxFilesToRetrieve = 1000;
                //job.AutoStopLimit = 1000000;
                job.TimeoutSeconds = 30;
                job.SearchFlags =
                  //SearchFlags.dtsSearchSelectMostRecent |
                  SearchFlags.dtsSearchDelayDocInfo;

                int num = 0;
                if (!int.TryParse(searchTerm, out num))
                {
                    //job.Fuzziness = 1;
                    job.SearchFlags = job.SearchFlags |
                      //SearchFlags.dtsSearchFuzzy |
                      //SearchFlags.dtsSearchStemming |
                      SearchFlags.dtsSearchPositionalScoring |
                      SearchFlags.dtsSearchAutoTermWeight;
                }

                if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;
                job.IndexesToSearch.Add(DataUtils.GetProductVersionsIndexPath(loginUser));
                job.Execute();

                return job.Results;
            }

        }

        public static string GetSearchResultsWhereClause(LoginUser loginUser)
        {
            StringBuilder resultBuilder = new StringBuilder();

            SearchCustomFilters searchCustomFilters = new SearchCustomFilters(loginUser);
            searchCustomFilters.LoadByUserID(loginUser.UserID);
            resultBuilder.Append(searchCustomFilters.ConvertToProductVersionEquivalentWhereClause());

            return resultBuilder.ToString();
        }

        public static ProductVersionsViewItem GetProductVersionsViewItemByUserRights(LoginUser loginUser, int productVersionID)
        {
            ProductVersionsView productVersionsView = new ProductVersionsView(loginUser);
            productVersionsView.LoadByProductVersionIDAndUserRights(productVersionID, loginUser.UserID);
            if (productVersionsView.IsEmpty)
                return null;
            else
                return productVersionsView[0];
        }

        public virtual void LoadByProductVersionIDAndUserRights(int productVersionID, int userID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                SET NOCOUNT OFF; 
                SELECT
                    pvv.*
                FROM 
                    [dbo].[ProductVersionsView] pvv
                    JOIN Products p
                        ON pvv.ProductID = p.ProductID
                    JOIN Organizations o
                        ON p.OrganizationID = o.OrganizationID
                WHERE 
                    pvv.[ProductVersionID] = @ProductVersionID
                    AND
                    (
                        o.UseProductFamilies = 0
                        OR (EXISTS (SELECT * FROM Users WHERE UserID = @UserID AND (ProductFamiliesRights < 1)))
                        OR pvv.ProductFamilyID IN (SELECT ProductFamilyID FROM UserRightsProductFamilies WHERE UserID = @UserID)
                    );
            ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("ProductVersionID", productVersionID);
                command.Parameters.AddWithValue("UserID", userID);
                Fill(command);
            }
        }
    }

}
