using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class AgentRating : BaseItem
  {
  }
  
  public partial class AgentRatings : BaseCollection, IEnumerable<AgentRating>
    {
        public virtual void LoadByOrganizationIDFilter(int organizationID, string filter, int start, int productFamilyID)
        {
            int end = start + 50;
            using (SqlCommand command = new SqlCommand())
            {
                switch (productFamilyID)
                {
                    // All product families
                    case -1:
                        if (start != -1)
                        {
                            if (filter != "")
                                command.CommandText = "SET NOCOUNT OFF; select * from (SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID], ROW_NUMBER() OVER (ORDER BY DateCreated Desc) AS rownum FROM [dbo].[AgentRatings] WHERE ([CompanyID] = @CompanyID and [Rating]=@filter)) as temp where rownum between @start and @end order by rownum asc;";
                            else
                                command.CommandText = "SET NOCOUNT OFF; select * from (SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID], ROW_NUMBER() OVER (ORDER BY DateCreated Desc) AS rownum FROM [dbo].[AgentRatings] WHERE ([CompanyID] = @CompanyID)) as temp where rownum between @start and @end order by rownum asc;";
                        }
                        else
                        {
                            if (filter != "")
                                command.CommandText = "SET NOCOUNT OFF; SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID] FROM [dbo].[AgentRatings] WHERE ([CompanyID] = @CompanyID and [Rating]=@filter);";
                            else
                                command.CommandText = "SET NOCOUNT OFF; SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID] FROM [dbo].[AgentRatings] WHERE ([CompanyID] = @CompanyID)";

                        }
                        break;
                    // Without product family
                    case 0:
                        if (start != -1)
                        {
                            if (filter != "")
                                command.CommandText = @"SET NOCOUNT OFF; 
                                SELECT
                                    *
                                FROM 
                                    (
                                        SELECT 
                                            ar.AgentRatingID
                                            , ar.OrganizationID
                                            , ar.CompanyID
                                            , ar.ContactID
                                            , ar.Rating
                                            , ar.Comment
                                            , ar.DateCreated
                                            , ar.TicketID
                                            , ROW_NUMBER() OVER (ORDER BY ar.DateCreated Desc) AS rownum 
                                        FROM
                                            AgentRatings ar
                                            JOIN Tickets t
                                                ON ar.TicketID = t.TicketID
                                            LEFT JOIN Products p
                                                ON t.ProductID = p.ProductID
                                        WHERE
                                            ar.CompanyID = @CompanyID 
                                            AND ar.Rating = @filter
                                            AND 
                                            (
                                                t.ProductID IS NULL
                                                OR p.ProductFamilyID IS NULL
                                            )
                                    ) AS temp 
                                WHERE
                                    rownum BETWEEN @start AND @end
                                ORDER BY 
                                    rownum ASC;";
                            else
                                command.CommandText = @"SET NOCOUNT OFF; 
                                SELECT
                                    *
                                FROM 
                                    (
                                        SELECT
                                            ar.AgentRatingID
                                            , ar.OrganizationID
                                            , ar.CompanyID
                                            , ar.ContactID
                                            , ar.Rating
                                            , ar.Comment
                                            , ar.DateCreated
                                            , ar.TicketID
                                            , ROW_NUMBER() OVER (ORDER BY ar.DateCreated DESC) AS rownum 
                                        FROM
                                            AgentRatings ar
                                            JOIN Tickets t
                                                ON ar.TicketID = t.TicketID
                                            LEFT JOIN Products p
                                                ON t.ProductID = p.ProductID
                                        WHERE
                                            ar.CompanyID = @CompanyID
                                            AND 
                                            (
                                                t.ProductID IS NULL
                                                OR p.ProductFamilyID IS NULL
                                            )
                                    ) as temp
                                WHERE
                                    rownum BETWEEN @start AND @end
                                ORDER BY 
                                    rownum ASC;";
                        }
                        else
                        {
                            if (filter != "")
                                command.CommandText = @"SET NOCOUNT OFF; 
                                SELECT
                                    ar.AgentRatingID
                                    , ar.OrganizationID
                                    , ar.CompanyID
                                    , ar.ContactID
                                    , ar.Rating
                                    , ar.Comment
                                    , ar.DateCreated
                                    , ar.TicketID
                                FROM
                                    AgentRatings ar
                                    JOIN Tickets t
                                        ON ar.TicketID = t.TicketID
                                    LEFT JOIN Products p
                                        ON t.ProductID = p.ProductID
                                WHERE
                                    ar.CompanyID = @CompanyID
                                    AND 
                                    (
                                        t.ProductID IS NULL
                                        OR p.ProductFamilyID IS NULL
                                    )
                                    AND ar.Rating = @filter;";
                            else
                                command.CommandText = @"SET NOCOUNT OFF; 
                                SELECT
                                    ar.AgentRatingID
                                    , ar.OrganizationID
                                    , ar.CompanyID
                                    , ar.ContactID
                                    , ar.Rating
                                    , ar.Comment
                                    , ar.DateCreated
                                    , ar.TicketID
                                FROM
                                    AgentRatings ar
                                    JOIN Tickets t
                                        ON ar.TicketID = t.TicketID
                                    LEFT JOIN Products p
                                        ON t.ProductID = p.ProductID
                                WHERE
                                    ar.CompanyID = @CompanyID
                                    AND 
                                    (
                                        t.ProductID IS NULL
                                        OR p.ProductFamilyID IS NULL
                                    )";

                        }
                        break;
                    // Specific product family
                    default:
                        if (start != -1)
                        {
                            if (filter != "")
                                command.CommandText = @"SET NOCOUNT OFF; 
                                SELECT
                                    *
                                FROM
                                    (
                                        SELECT
                                            ar.AgentRatingID
                                            , ar.OrganizationID
                                            , ar.CompanyID
                                            , ar.ContactID
                                            , ar.Rating
                                            , ar.Comment
                                            , ar.DateCreated
                                            , ar.TicketID
                                            , ROW_NUMBER() OVER (ORDER BY ar.DateCreated DESC) AS rownum 
                                        FROM
                                            AgentRatings ar
                                            JOIN Tickets t
                                                ON AgentRatings.TicketID = t.TicketID
                                            JOIN Products p
                                                ON t.ProductID = p.ProductID
                                        WHERE
                                            ar.CompanyID = @CompanyID
                                            AND ar.Rating = @filter
                                            AND p.ProductFamilyID = @productFamilyID
                                    ) as temp
                                WHERE
                                    rownum BETWEEN @start AND @end
                                ORDER BY
                                    rownum ASC;";
                            else
                                command.CommandText = @"SET NOCOUNT OFF;
                                SELECT
                                    *
                                FROM
                                    (
                                        SELECT
                                            ar.AgentRatingID
                                            , ar.OrganizationID
                                            , ar.CompanyID
                                            , ar.ContactID
                                            , ar.Rating
                                            , ar.Comment
                                            , ar.DateCreated
                                            , ar.TicketID
                                            , ROW_NUMBER() OVER (ORDER BY ar.DateCreated DESC) AS rownum 
                                        FROM
                                            AgentRatings ar
                                            JOIN Tickets t
                                                ON ar.TicketID = t.TicketID
                                            JOIN Products p
                                                ON t.ProductID = p.ProductID
                                        WHERE
                                            ar.CompanyID = @CompanyID
                                            AND p.ProductFamilyID = @productFamilyID
                                    ) AS temp 
                                WHERE
                                    rownum BETWEEN @start AND @end
                                ORDER BY
                                    rownum ASC;";
                        }
                        else
                        {
                            if (filter != "")
                                command.CommandText = @"SET NOCOUNT OFF; 
                                SELECT
                                    ar.AgentRatingID
                                    , ar.OrganizationID
                                    , ar.CompanyID
                                    , ar.ContactID
                                    , ar.Rating
                                    , ar.Comment
                                    , ar.DateCreated
                                    , ar.TicketID
                                FROM   
                                    AgentRatings ar
                                    JOIN Tickets t
                                        ON ar.TicketID = t.TicketID
                                    JOIN Products p
                                        ON t.ProductID = p.ProductID
                                WHERE
                                    ar.CompanyID = @CompanyID
                                    AND ar.Rating = @filter
                                    AND p.ProductFamilyID = @productFamilyID;";
                            else
                                command.CommandText = @"SET NOCOUNT OFF; 
                                SELECT
                                    ar.AgentRatingID
                                    , ar.OrganizationID
                                    , ar.CompanyID
                                    , ar.ContactID
                                    , ar.Rating
                                    , ar.Comment
                                    , ar.DateCreated
                                    , ar.TicketID
                                FROM
                                    AgentRatings ar
                                    JOIN Tickets t
                                        ON ar.TicketID = t.TicketID
                                    JOIN Products p
                                        ON t.ProductID = p.ProductID
                                WHERE
                                    ar.CompanyID = @CompanyID
                                    AND p.ProductFamilyID = @productFamilyID";
                        }
                        break;
                }

                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("CompanyID", organizationID);
                command.Parameters.AddWithValue("filter", filter);
                command.Parameters.AddWithValue("start", start);
                command.Parameters.AddWithValue("end", end);
                command.Parameters.AddWithValue("productFamilyID", productFamilyID);
                Fill(command);
            }
        }

        public virtual void LoadByContactIDFilter(int userID, string filter, int start, int productFamilyID)
        {
            int end = start + 50;
            using (SqlCommand command = new SqlCommand())
            {
                switch (productFamilyID)
                {
                    // All product families
                    case -1:
                        if (start != -1)
                        {
                            if (filter != "")
                                command.CommandText = "SET NOCOUNT OFF; select * from (SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID], ROW_NUMBER() OVER (ORDER BY DateCreated Desc) AS rownum FROM [dbo].[AgentRatings] WHERE ([ContactID] = @userID and [Rating]=@filter)) as temp where rownum between @start and @end order by rownum asc;";
                            else
                                command.CommandText = "SET NOCOUNT OFF; select * from (SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID], ROW_NUMBER() OVER (ORDER BY DateCreated Desc) AS rownum FROM [dbo].[AgentRatings] WHERE ([ContactID] = @userID)) as temp where rownum between @start and @end order by rownum asc;";
                        }
                        else
                        {
                            if (filter != "")
                                command.CommandText = "SET NOCOUNT OFF; SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID] FROM [dbo].[AgentRatings] WHERE ([ContactID] = @userID and [Rating]=@filter);";
                            else
                                command.CommandText = "SET NOCOUNT OFF; SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID] FROM [dbo].[AgentRatings] WHERE ([ContactID] = @userID)";

                        }
                        break;
                    // Without product family
                    case 0:
                        if (start != -1)
                        {
                            if (filter != "")
                                command.CommandText = @"SET NOCOUNT OFF; 
                                SELECT
                                    *
                                FROM
                                    (
                                        SELECT
                                            ar.AgentRatingID
                                            , ar.OrganizationID
                                            , ar.CompanyID
                                            , ar.ContactID
                                            , ar.Rating
                                            , ar.Comment
                                            , ar.DateCreated
                                            , ar.TicketID
                                            , ROW_NUMBER() OVER (ORDER BY ar.DateCreated DESC) AS rownum 
                                        FROM
                                            AgentRatings ar
                                            JOIN Tickets t
                                                ON ar.TicketID = t.TicketID
                                            LEFT JOIN Products p
                                                ON t.ProductID = p.ProductID
                                        WHERE
                                            ar.ContactID = @userID 
                                            AND ar.Rating = @filter
                                            AND 
                                            (
                                                t.ProductID IS NULL
                                                OR p.ProductFamilyID IS NULL
                                            )
                                    ) as temp
                                WHERE
                                    rownum BETWEEN @start AND @end
                                ORDER BY
                                    rownum ASC;";
                            else
                                command.CommandText = @"SET NOCOUNT OFF;
                                    SELECT
                                        *
                                    FROM
                                        (
                                            SELECT
                                                ar.AgentRatingID
                                                , ar.OrganizationID
                                                , ar.CompanyID
                                                , ar.ContactID
                                                , ar.Rating
                                                , ar.Comment
                                                , ar.DateCreated
                                                , ar.TicketID
                                                , ROW_NUMBER() OVER (ORDER BY ar.DateCreated DESC) AS rownum 
                                            FROM
                                                AgentRatings ar
                                                JOIN Tickets t
                                                    ON ar.TicketID = t.TicketID
                                                LEFT JOIN Products p
                                                    ON t.ProductID = p.ProductID
                                            WHERE
                                                ar.ContactID = @userID
                                                AND 
                                                (
                                                    t.ProductID IS NULL
                                                    OR p.ProductFamilyID IS NULL
                                                )
                                        ) as temp
                                    WHERE
                                        rownum BETWEEN @start AND @end
                                    ORDER BY
                                        rownum ASC;";
                        }
                        else
                        {
                            if (filter != "")
                                command.CommandText = @"SET NOCOUNT OFF;
                                    SELECT
                                        ar.AgentRatingID
                                        , ar.OrganizationID
                                        , ar.CompanyID
                                        , ar.ContactID
                                        , ar.Rating
                                        , ar.Comment
                                        , ar.DateCreated
                                        , ar.TicketID
                                    FROM
                                        AgentRatings ar
                                        JOIN Tickets t
                                            ON ar.TicketID = t.TicketID
                                        LEFT JOIN Products p
                                            ON t.ProductID = p.ProductID
                                    WHERE
                                        ar.ContactID = @userID
                                        AND 
                                        (
                                            t.ProductID IS NULL
                                            OR p.ProductFamilyID IS NULL
                                        )
                                        AND ar.Rating = @filter;";
                            else
                                command.CommandText = @"SET NOCOUNT OFF; 
                                    SELECT
                                        ar.AgentRatingID
                                        , ar.OrganizationID
                                        , ar.CompanyID
                                        , ar.ContactID
                                        , ar.Rating
                                        , ar.Comment
                                        , ar.DateCreated
                                        , ar.TicketID
                                    FROM
                                        AgentRatings ar
                                        JOIN Tickets t
                                            ON ar.TicketID = t.TicketID
                                        LEFT JOIN Products p
                                            ON t.ProductID = p.ProductID
                                    WHERE
                                        ar.ContactID = @userID
                                        AND 
                                        (
                                            t.ProductID IS NULL
                                            OR p.ProductFamilyID IS NULL
                                        )";

                        }
                        break;
                    // Specific product family
                    default:
                        if (start != -1)
                        {
                            if (filter != "")
                                command.CommandText = @"SET NOCOUNT OFF;
                                    SELECT
                                        *
                                    FROM
                                        (
                                            SELECT
                                                ar.AgentRatingID
                                                , ar.OrganizationID
                                                , ar.CompanyID
                                                , ar.ContactID
                                                , ar.Rating
                                                , ar.Comment
                                                , ar.DateCreated
                                                , ar.TicketID
                                                , ROW_NUMBER() OVER (ORDER BY ar.DateCreated DESC) AS rownum 
                                            FROM
                                                AgentRatings ar
                                                JOIN Tickets t
                                                    ON ar.TicketID = t.TicketID
                                                JOIN Products p
                                                    ON t.ProductID = p.ProductID
                                            WHERE
                                                ar.ContactID = @userID
                                                AND ar.Rating = @filter
                                                AND p.ProductFamilyID = @productFamilyID
                                        ) as temp 
                                    WHERE
                                        rownum BETWEEN @start AND @end
                                    ORDER BY
                                        rownum ASC;";
                            else
                                command.CommandText = @"SET NOCOUNT OFF; 
                                    SELECT
                                        * 
                                    FROM
                                        (
                                            SELECT
                                                ar.AgentRatingID
                                                , ar.OrganizationID
                                                , ar.CompanyID
                                                , ar.ContactID
                                                , ar.Rating
                                                , ar.Comment
                                                , ar.DateCreated
                                                , ar.TicketID
                                                , ROW_NUMBER() OVER (ORDER BY ar.DateCreated DESC) AS rownum 
                                            FROM
                                                AgentRatings ar
                                                JOIN Tickets t
                                                    ON ar.TicketID = t.TicketID
                                                JOIN Products p
                                                    ON t.ProductID = p.ProductID
                                            WHERE
                                                ar.ContactID = @userID
                                                AND p.ProductFamilyID = @productFamilyID
                                        ) as temp
                                    WHERE
                                        rownum BETWEEN @start AND @end
                                    ORDER BY
                                        rownum ASC;";
                        }
                        else
                        {
                            if (filter != "")
                                command.CommandText = @"SET NOCOUNT OFF; 
                                    SELECT
                                        ar.AgentRatingID
                                        , ar.OrganizationID
                                        , ar.CompanyID
                                        , ar.ContactID
                                        , ar.Rating
                                        , ar.Comment
                                        , ar.DateCreated
                                        , ar.TicketID
                                    FROM
                                        AgentRatings ar
                                        JOIN Tickets t
                                            ON ar.TicketID = t.TicketID
                                        JOIN Products p
                                            ON t.ProductID = p.ProductID
                                    WHERE
                                        ar.ContactID = @userID
                                        AND ar.Rating = @filter
                                        AND p.ProductFamilyID = @productFamilyID;";
                            else
                                command.CommandText = @"SET NOCOUNT OFF; 
                                    SELECT
                                        ar.AgentRatingID
                                        , ar.OrganizationID
                                        , ar.CompanyID
                                        , ar.ContactID
                                        , ar.Rating
                                        , ar.Comment
                                        , ar.DateCreated
                                        , ar.TicketID
                                    FROM
                                        AgentRatings ar
                                        JOIN Tickets t
                                            ON ar.TicketID = t.TicketID
                                        JOIN Products p
                                            ON t.ProductID = p.ProductID
                                    WHERE 
                                        ar.ContactID = @userID
                                        AND p.ProductFamilyID = @productFamilyID";

                        }
                        break;
                }


                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("userID", userID);
                command.Parameters.AddWithValue("filter", filter);
                command.Parameters.AddWithValue("start", start);
                command.Parameters.AddWithValue("end", end);
                command.Parameters.AddWithValue("productFamilyID", productFamilyID);
                Fill(command);
            }
        }

        public virtual void LoadByAgentRatingIDFilter(int[] agentRatingIDs, string filter, int start, int productFamilyID)
        {
            int end = start + 50;
            using (SqlCommand command = new SqlCommand())
            {
                switch (productFamilyID)
                {
                    // All product families
                    case -1:
                        if (start != -1)
                        {
                            if (filter != "")
                                command.CommandText = "SET NOCOUNT OFF; select * from (SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID], ROW_NUMBER() OVER (ORDER BY DateCreated Desc) AS rownum FROM [dbo].[AgentRatings] WHERE ([AgentRatingID] in (" + DataUtils.IntArrayToCommaString(agentRatingIDs) + ") and [Rating]=@filter)) as temp where rownum between @start and @end order by rownum asc;";
                            else
                                command.CommandText = "SET NOCOUNT OFF; select * from (SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID], ROW_NUMBER() OVER (ORDER BY DateCreated Desc) AS rownum FROM [dbo].[AgentRatings] WHERE ([AgentRatingID] in (" + DataUtils.IntArrayToCommaString(agentRatingIDs) + "))) as temp where rownum between @start and @end order by rownum asc;";
                        }
                        else
                        {
                            if (filter != "")
                                command.CommandText = "SET NOCOUNT OFF; SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID], ROW_NUMBER() OVER (ORDER BY DateCreated Desc) AS rownum FROM [dbo].[AgentRatings] WHERE ([AgentRatingID] in (" + DataUtils.IntArrayToCommaString(agentRatingIDs) + ") and [Rating]=@filter);";
                            else
                                command.CommandText = "SET NOCOUNT OFF; SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID], ROW_NUMBER() OVER (ORDER BY DateCreated Desc) AS rownum FROM [dbo].[AgentRatings] WHERE ([AgentRatingID] in (" + DataUtils.IntArrayToCommaString(agentRatingIDs) + "));";
                        }
                        break;
                    // Without product family
                    case 0:
                        if (start != -1)
                        {
                            if (filter != "")
                                command.CommandText = @"SET NOCOUNT OFF; 
                                SELECT
                                    *
                                FROM 
                                    (
                                        SELECT 
                                            ar.AgentRatingID
                                            , ar.OrganizationID
                                            , ar.CompanyID
                                            , ar.ContactID
                                            , ar.Rating
                                            , ar.Comment
                                            , ar.DateCreated
                                            , ar.TicketID
                                            , ROW_NUMBER() OVER (ORDER BY ar.DateCreated Desc) AS rownum 
                                        FROM
                                            AgentRatings ar
                                            JOIN Tickets t
                                                ON ar.TicketID = t.TicketID
                                            LEFT JOIN Products p
                                                ON t.ProductID = p.ProductID
                                        WHERE
                                            ar.AgentRatingID in (" + DataUtils.IntArrayToCommaString(agentRatingIDs) + @") 
                                            AND ar.Rating = @filter
                                            AND 
                                            (
                                                t.ProductID IS NULL
                                                OR p.ProductFamilyID IS NULL
                                            )
                                    ) AS temp 
                                WHERE
                                    rownum BETWEEN @start AND @end
                                ORDER BY 
                                    rownum ASC;";
                            else
                                command.CommandText = @"SET NOCOUNT OFF; 
                                SELECT
                                    *
                                FROM 
                                    (
                                        SELECT
                                            ar.AgentRatingID
                                            , ar.OrganizationID
                                            , ar.CompanyID
                                            , ar.ContactID
                                            , ar.Rating
                                            , ar.Comment
                                            , ar.DateCreated
                                            , ar.TicketID
                                            , ROW_NUMBER() OVER (ORDER BY ar.DateCreated DESC) AS rownum 
                                        FROM
                                            AgentRatings ar
                                            JOIN Tickets t
                                                ON ar.TicketID = t.TicketID
                                            LEFT JOIN Products p
                                                ON t.ProductID = p.ProductID
                                        WHERE
                                            ar.AgentRatingID in (" + DataUtils.IntArrayToCommaString(agentRatingIDs) + @") 
                                            AND 
                                            (
                                                t.ProductID IS NULL
                                                OR p.ProductFamilyID IS NULL
                                            )
                                    ) as temp
                                WHERE
                                    rownum BETWEEN @start AND @end
                                ORDER BY 
                                    rownum ASC;";
                        }
                        else
                        {
                            if (filter != "")
                                command.CommandText = @"SET NOCOUNT OFF; 
                                SELECT
                                    ar.AgentRatingID
                                    , ar.OrganizationID
                                    , ar.CompanyID
                                    , ar.ContactID
                                    , ar.Rating
                                    , ar.Comment
                                    , ar.DateCreated
                                    , ar.TicketID
                                FROM
                                    AgentRatings ar
                                    JOIN Tickets t
                                        ON ar.TicketID = t.TicketID
                                    LEFT JOIN Products p
                                        ON t.ProductID = p.ProductID
                                WHERE
                                    ar.AgentRatingID in (" + DataUtils.IntArrayToCommaString(agentRatingIDs) + @") 
                                    AND 
                                    (
                                        t.ProductID IS NULL
                                        OR p.ProductFamilyID IS NULL
                                    )
                                    AND ar.Rating = @filter;";
                            else
                                command.CommandText = @"SET NOCOUNT OFF; 
                                SELECT
                                    ar.AgentRatingID
                                    , ar.OrganizationID
                                    , ar.CompanyID
                                    , ar.ContactID
                                    , ar.Rating
                                    , ar.Comment
                                    , ar.DateCreated
                                    , ar.TicketID
                                FROM
                                    AgentRatings ar
                                    JOIN Tickets t
                                        ON ar.TicketID = t.TicketID
                                    LEFT JOIN Products p
                                        ON t.ProductID = p.ProductID
                                WHERE
                                    ar.AgentRatingID in (" + DataUtils.IntArrayToCommaString(agentRatingIDs) + @") 
                                    AND 
                                    (
                                        t.ProductID IS NULL
                                        OR p.ProductFamilyID IS NULL
                                    )";

                        }
                        break;
                    // Specific product family
                    default:
                        if (start != -1)
                        {
                            if (filter != "")
                                command.CommandText = @"SET NOCOUNT OFF; 
                                SELECT
                                    *
                                FROM
                                    (
                                        SELECT
                                            ar.AgentRatingID
                                            , ar.OrganizationID
                                            , ar.CompanyID
                                            , ar.ContactID
                                            , ar.Rating
                                            , ar.Comment
                                            , ar.DateCreated
                                            , ar.TicketID
                                            , ROW_NUMBER() OVER (ORDER BY ar.DateCreated DESC) AS rownum 
                                        FROM
                                            AgentRatings ar
                                            JOIN Tickets t
                                                ON AgentRatings.TicketID = t.TicketID
                                            JOIN Products p
                                                ON t.ProductID = p.ProductID
                                        WHERE
                                            ar.AgentRatingID in (" + DataUtils.IntArrayToCommaString(agentRatingIDs) + @") 
                                            AND ar.Rating = @filter
                                            AND p.ProductFamilyID = @productFamilyID
                                    ) as temp
                                WHERE
                                    rownum BETWEEN @start AND @end
                                ORDER BY
                                    rownum ASC;";
                            else
                                command.CommandText = @"SET NOCOUNT OFF;
                                SELECT
                                    *
                                FROM
                                    (
                                        SELECT
                                            ar.AgentRatingID
                                            , ar.OrganizationID
                                            , ar.CompanyID
                                            , ar.ContactID
                                            , ar.Rating
                                            , ar.Comment
                                            , ar.DateCreated
                                            , ar.TicketID
                                            , ROW_NUMBER() OVER (ORDER BY ar.DateCreated DESC) AS rownum 
                                        FROM
                                            AgentRatings ar
                                            JOIN Tickets t
                                                ON ar.TicketID = t.TicketID
                                            JOIN Products p
                                                ON t.ProductID = p.ProductID
                                        WHERE
                                            ar.AgentRatingID in (" + DataUtils.IntArrayToCommaString(agentRatingIDs) + @") 
                                            AND p.ProductFamilyID = @productFamilyID
                                    ) AS temp 
                                WHERE
                                    rownum BETWEEN @start AND @end
                                ORDER BY
                                    rownum ASC;";
                        }
                        else
                        {
                            if (filter != "")
                                command.CommandText = @"SET NOCOUNT OFF; 
                                SELECT
                                    ar.AgentRatingID
                                    , ar.OrganizationID
                                    , ar.CompanyID
                                    , ar.ContactID
                                    , ar.Rating
                                    , ar.Comment
                                    , ar.DateCreated
                                    , ar.TicketID
                                FROM   
                                    AgentRatings ar
                                    JOIN Tickets t
                                        ON ar.TicketID = t.TicketID
                                    JOIN Products p
                                        ON t.ProductID = p.ProductID
                                WHERE
                                    ar.AgentRatingID in (" + DataUtils.IntArrayToCommaString(agentRatingIDs) + @") 
                                    AND ar.Rating = @filter
                                    AND p.ProductFamilyID = @productFamilyID;";
                            else
                                command.CommandText = @"SET NOCOUNT OFF; 
                                SELECT
                                    ar.AgentRatingID
                                    , ar.OrganizationID
                                    , ar.CompanyID
                                    , ar.ContactID
                                    , ar.Rating
                                    , ar.Comment
                                    , ar.DateCreated
                                    , ar.TicketID
                                FROM
                                    AgentRatings ar
                                    JOIN Tickets t
                                        ON ar.TicketID = t.TicketID
                                    JOIN Products p
                                        ON t.ProductID = p.ProductID
                                WHERE
                                    ar.AgentRatingID in (" + DataUtils.IntArrayToCommaString(agentRatingIDs) + @") 
                                    AND p.ProductFamilyID = @productFamilyID";
                        }
                        break;
                }

                command.CommandType = CommandType.Text;
                //command.Parameters.AddWithValue("AgentRatingID", );
                command.Parameters.AddWithValue("filter", filter);
                command.Parameters.AddWithValue("start", start);
                command.Parameters.AddWithValue("end", end);
                command.Parameters.AddWithValue("productFamilyID", productFamilyID);
                Fill(command);

            }
        }
    }

}
