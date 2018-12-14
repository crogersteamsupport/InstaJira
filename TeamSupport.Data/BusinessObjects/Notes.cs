using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class Note
    {

        public string CreatorName
        {
            get
            {
                if (Row.Table.Columns.Contains("CreatorName") && Row["CreatorName"] != DBNull.Value)
                {
                    return (string)Row["CreatorName"];
                }
                else return "";
            }
        }

        public string ProductFamily
        {
            get
            {
                if (Row.Table.Columns.Contains("ProductFamily") && Row["ProductFamily"] != DBNull.Value)
                {
                    return (string)Row["ProductFamily"];
                }
                else return "";
            }
        }
    }

    public partial class Notes
    {

        public void LoadByCustomer(int organizationID)
        {
            LoadByReferenceType(ReferenceType.Organizations, organizationID);
        }

        public void LoadByReferenceType(ReferenceType refType, int refID, string orderBy = "DateCreated", bool includeCompanyChildren = false)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
            SELECT 
                n.*
                , u.FirstName + ' ' + u.LastName AS CreatorName
            FROM
                Notes n 
                LEFT JOIN Users u
                    ON n.CreatorID = u.UserID
            WHERE
                n.RefType = @ReferenceType
                AND n.RefID IN
                (
                    SELECT
                        @ReferenceID
                    UNION
                    SELECT
                        CustomerID
                    FROM
                        CustomerRelationships
                    WHERE
                        RelatedCustomerID = @ReferenceID
                        AND @IncludeCompanyChildren = 1
                )
            ORDER BY
                n." + orderBy + " DESC";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ReferenceType", refType);
                command.Parameters.AddWithValue("@ReferenceID", refID);
                command.Parameters.AddWithValue("@IncludeCompanyChildren", includeCompanyChildren);
                Fill(command);
            }
        }

        //Get all the notes for the users under the org
        public void LoadByReferenceTypeUser(ReferenceType refType, int refID, string orderBy = "DateCreated", bool includeCompanyChildren = false)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
            SELECT 
                n.*
                , u.FirstName + ' ' + u.LastName AS CreatorName
            FROM
                Notes n 
                LEFT JOIN Users u
                    ON n.CreatorID = u.UserID
            WHERE
                n.RefType = @ReferenceType
                AND n.RefID IN
                (
                    SELECT
                        @ReferenceID
                    UNION
                    SELECT
                        CustomerID
                    FROM
                        CustomerRelationships
                    WHERE
                        RelatedCustomerID = @ReferenceID
                        AND @IncludeCompanyChildren = 1
				    UNION
					SELECT UserID
					FROM Users
					WHERE OrganizationID=@organizationID
                )
            ORDER BY
                n." + orderBy + " DESC";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ReferenceType", refType);
                command.Parameters.AddWithValue("@ReferenceID", refID);
                command.Parameters.AddWithValue("@IncludeCompanyChildren", includeCompanyChildren);
                command.Parameters.AddWithValue("@organizationID", refID);
                Fill(command);
            }
        }

        public void LoadByReferenceTypeByUserRights(ReferenceType refType, int refID, int viewerID, string orderBy = "DateCreated", bool includeCompanyChildren = false)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
            SELECT 
                n.*
                , u.FirstName + ' ' + u.LastName AS CreatorName
                , f.Name AS ProductFamily
            FROM
                Notes n 
                LEFT JOIN Users u
                    ON n.CreatorID = u.UserID
                LEFT JOIN ProductFamilies f
                    ON n.ProductFamilyID = f.ProductFamilyID
            WHERE
                n.RefType = @ReferenceType
                AND n.RefID IN
                (
                    SELECT
                        @ReferenceID
                    UNION
                    SELECT
                        CustomerID
                    FROM
                        CustomerRelationships
                    WHERE
                        RelatedCustomerID = @ReferenceID
                        AND @IncludeCompanyChildren = 1
                )
                AND
                (
                    EXISTS (SELECT UserID FROM Users WHERE UserID = @ViewerID AND ProductFamiliesRights = 0)
                    OR n.ProductFamilyID IS NULL
                    OR n.ProductFamilyID IN
                    (
                        SELECT
                            urpf.ProductFamilyID
                        FROM
                            UserRightsProductFamilies urpf
                        WHERE
                            urpf.UserID = @ViewerID
                    )
                )
            ORDER BY
                n." + orderBy + " DESC";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ReferenceType", refType);
                command.Parameters.AddWithValue("@ReferenceID", refID);
                command.Parameters.AddWithValue("@IncludeCompanyChildren", includeCompanyChildren);
                command.Parameters.AddWithValue("@ViewerID", viewerID);
                Fill(command);
            }
        }

        public void LoadByReferenceTypeByUserRightsUsers(ReferenceType refType, int refID, int viewerID, string organizationID, string orderBy = "DateCreated", bool includeCompanyChildren = false)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
            SELECT 
                n.*
                , u.FirstName + ' ' + u.LastName AS CreatorName
                , f.Name AS ProductFamily
            FROM
                Notes n 
                LEFT JOIN Users u
                    ON n.CreatorID = u.UserID
                LEFT JOIN ProductFamilies f
                    ON n.ProductFamilyID = f.ProductFamilyID
            WHERE
                n.RefType = @ReferenceType
                AND n.RefID IN
                (
                    SELECT
                        @ReferenceID
                    UNION
                    SELECT
                        CustomerID
                    FROM
                        CustomerRelationships
                    WHERE
                        RelatedCustomerID = @ReferenceID
                        AND @IncludeCompanyChildren = 1
				    UNION
					SELECT UserID
					FROM Users
					WHERE OrganizationID=@organizationID
                )
                AND
                (
                    EXISTS (SELECT UserID FROM Users WHERE UserID = @ViewerID AND ProductFamiliesRights = 0)
                    OR n.ProductFamilyID IS NULL
                    OR n.ProductFamilyID IN
                    (
                        SELECT
                            urpf.ProductFamilyID
                        FROM
                            UserRightsProductFamilies urpf
                        WHERE
                            urpf.UserID = @ViewerID
                    )
                )
            ORDER BY
                n." + orderBy + " DESC";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ReferenceType", refType);
                command.Parameters.AddWithValue("@ReferenceID", refID);
                command.Parameters.AddWithValue("@IncludeCompanyChildren", includeCompanyChildren);
                command.Parameters.AddWithValue("@ViewerID", viewerID);
                command.Parameters.AddWithValue("@organizationID", organizationID);
                Fill(command);
            }
        }

        public void LoadbyIsAlert(ReferenceType refType, int refID, string orderBy = "DateModified")
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"SELECT n.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM Notes n 
                                LEFT JOIN Users u ON n.CreatorID = u.UserID 
                                WHERE (n.RefID = @ReferenceID)
                                AND n.isAlert = 1
                                AND (n.RefType = @ReferenceType)
                                ORDER BY n." + orderBy;
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ReferenceType", refType);
            command.Parameters.AddWithValue("@ReferenceID", refID);
            Fill(command);
        }
    }

        public void LoadByUserAndActiveAlert(ReferenceType refType, int refID, int userID)
        {
            // If loading contact alerts, include her company alerts too.
            StringBuilder includeCompanyAlertsClause = new StringBuilder();
            if (refType == ReferenceType.Users)
            {
                includeCompanyAlertsClause.Append(@"
                OR
                (
                    n.RefType = 9
                    AND n.RefID = (SELECT OrganizationID FROM Users WHERE UserID = @ReferenceID)
                )");
            }
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                    SELECT
                        n.*,
                        u.FirstName + ' ' + u.LastName AS CreatorName
                    FROM
                        Notes n 
                        LEFT JOIN Users u
                            ON n.CreatorID = u.UserID 
                        LEFT JOIN UserNoteSettings s
                            ON n.NoteID = s.RefID
                            AND n.RefType = s.RefType
                            AND s.UserID = @UserID
                    WHERE
                        (
                            (
                                n.RefID = @ReferenceID
                                AND n.RefType = @ReferenceType
                            )
                            " + includeCompanyAlertsClause.ToString() + @"
                        )
                        AND n.isAlert = 1
                        AND 
                        (
                            s.UserID IS NULL
                            OR 
                            (
                                s.IsDismissed = 0
                                AND s.IsSnoozed = 0
                            )
                            OR
                            (
                                s.IsSnoozed = 1
                                AND s.SnoozeTime < DATEADD(HOUR, -8, GETDATE()) 
                            )
                        )";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ReferenceType", refType);
                command.Parameters.AddWithValue("@ReferenceID", refID);
                command.Parameters.AddWithValue("@UserID", userID);
                Fill(command);
            }
        }

        public void LoadByTicketUserAndActiveAlert(int ticketID, int userID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                    SELECT
                        n.*,
                        u.FirstName + ' ' + u.LastName AS CreatorName
                    FROM
                        Notes n 
                        LEFT JOIN Users u
                            ON n.CreatorID = u.UserID 
                        LEFT JOIN UserNoteSettings s
                            ON n.NoteID = s.RefID
                            AND n.RefType = s.RefType
                            AND s.UserID = @UserID
                    WHERE
                        n.isAlert = 1
                        AND
                        (
                            (
                                n.RefType = 9
                                AND n.RefID IN
                                (
                                    SELECT
                                        OrganizationID
                                    FROM
                                        OrganizationTickets
                                    WHERE
                                        TicketID = @TicketID
                                )
                            )
                            OR
                            (
                                n.RefType = 22
                                AND n.RefID IN
                                (
                                    SELECT
                                        UserID
                                    FROM
                                        UserTickets
                                    WHERE
                                        TicketID = @TicketID
                                )
                            )
                        )
                        AND 
                        (
                            s.UserID IS NULL
                            OR 
                            (
                                s.IsDismissed = 0
                                AND s.IsSnoozed = 0
                            )
                            OR
                            (
                                s.IsSnoozed = 1
                                AND s.SnoozeTime < DATEADD(HOUR, -8, GETDATE()) 
                            )
                        )";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                command.Parameters.AddWithValue("@UserID", userID);
                Fill(command);
            }
        }

        public void SearchNotes(string searchString, string organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"select top 20 n.* from notes n
                    join organizations o on  n.refid = o.OrganizationID and reftype = 9
                    where 
                    n.Title like '%' + @searchString + '%'
                    and  
                    o.parentid = @parentid

                    union

                    select top 20 n.* from notes n
                    join users u on  n.refid = u.userid and reftype = 22
                    join organizations o on o.OrganizationID = u.OrganizationID 
                    where 
                    n.Title like '%' + @searchString + '%'
                    and o.ParentID=@parentid";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@searchString", searchString);
                command.Parameters.AddWithValue("@parentid", organizationID);
                Fill(command);
            }
        }

        public void ReplaceActivityType(int oldID, int newID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE Notes SET ActivityType = @newID WHERE (ActivityType = @oldID)";
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@oldID", oldID);
                command.Parameters.AddWithValue("@newID", newID);
                ExecuteNonQuery(command, "Notes");
            }
        }


    }


}
