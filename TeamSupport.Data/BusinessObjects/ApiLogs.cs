using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ApiLog
  {
  }
  
  public partial class ApiLogs
  {
    public static int GetDailyRequestCount(LoginUser loginUser, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT COUNT(1)
FROM
	ApiLogs
	JOIN (SELECT organizationId
					FROM
						Organizations
					WHERE
						organizationId = @organizationId
						OR parentId = @organizationId) AS Organizations
		ON ApiLogs.organizationId = Organizations.organizationId
WHERE
	DateCreated > DATEADD(d, -1, GETUTCDATE())
    AND ApiLogs.StatusCode <> 403";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);

        ApiLogs apiLogs = new ApiLogs(loginUser);
        return (int)apiLogs.ExecuteScalar(command);
      }
    }

        public static int GetLastMinuteRequestCount(LoginUser loginUser, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                    SELECT
                        COUNT(1)
                    FROM
	                    ApiLogs
	                    JOIN 
                        (
                            SELECT
                                organizationId
					        FROM
                                Organizations
                            WHERE
						        organizationId = @organizationId
						        OR parentId = @organizationId
                        ) AS Organizations
		                    ON ApiLogs.organizationId = Organizations.organizationId
                    WHERE
	                    DateCreated > DATEADD(mi, -1, GETUTCDATE())
                        AND ApiLogs.StatusCode <> 403";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);

                ApiLogs apiLogs = new ApiLogs(loginUser);
                return (int)apiLogs.ExecuteScalar(command);
            }
        }

        public static bool IsUrlBlackListed(LoginUser loginUser, int organizationID, string url)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                    SELECT COUNT(*)
                    FROM ApiBlacklist
                    WHERE OrganizationID=@OrganizationID
                    AND Url = @url";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@Url", url);

                ApiLogs apiLogs = new ApiLogs(loginUser);
                return (int)apiLogs.ExecuteScalar(command) > 0;
            }
        }
    
  }
  
}
