using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class Task
    {
    }

    public partial class Tasks
    {
        public void LoadByTicketID(int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                SELECT
                    ST.CreatorID,
                    ST.DateCreated,
                    ST.Description,
                    ST.DueDate,
                    R.HasEmailSent,
                    R.IsDismissed,
                    ST.OrganizationID,
                    ST.TaskID,
                    ST.DateCompleted,
                    ST.ReminderDueDate,
                    ST.IsComplete,
                    ST.ParentID,
                    CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
                        ELSE ST.Name END AS Name,
                    ST.UserID
                FROM
                    Tasks ST
                    LEFT JOIN Tasks C
                        ON ST.ParentID = T.TaskID
                    JOIN TaskAssociations TA
                        ON ST.TaskID = ta.TaskID
                    LEFT JOIN Reminders R
                        ON T.TaskID = R.RefID
                        AND R.RefType = 61
                WHERE
                    TA.RefType = 17 
                    AND TA.RefID = @TicketID";


                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                Fill(command);
            }
        }

        public void LoadAssignedTasks(int from, int count, int userID, bool searchPending, bool searchComplete)
        {
            string pendingQuery = @"
                SELECT
                    ST.CreatorID,
                    ST.DateCreated,
                    ST.Description,
                    ST.DueDate,
                    R.HasEmailSent,
                    R.IsDismissed,
                    ST.OrganizationID,
                    ST.TaskID,
                    ST.DateCompleted,
                    ST.ReminderDueDate,
                    ST.IsComplete,
                    ST.ParentID,
                    CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
                        ELSE ST.Name END AS Name,
                    ST.UserID
                FROM
                    Tasks ST
                    LEFT JOIN Tasks T 
                        ON ST.ParentID = T.TaskID
                    LEFT JOIN Reminders R
                        ON ST.TaskID = R.RefID
                        AND R.RefType = 61
                WHERE
                    ST.CreatorID = @UserID
                    AND ST.UserID <> @UserID
                    AND ST.IsComplete = 0";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY CASE WHEN DueDate IS NULL THEN 1 ELSE 0 END, IsComplete ASC, DateCompleted DESC, DueDate, ReminderDueDate) AS 'RowNum' FROM q)
            SELECT
                ReminderID
                , OrganizationID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , HasEmailSent
                , CreatorID
                , DateCreated
                , Name
                , ReminderDueDate
                , IsComplete
                , DateCompleted
                , ParentID
            FROM 
                r
            WHERE
                RowNum BETWEEN @From AND @To";

            StringBuilder query;
            query = new StringBuilder(string.Format(pageQuery, pendingQuery));

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = query.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@From", from + 1);
                command.Parameters.AddWithValue("@To", from + count);
                Fill(command);
            }
        }

        public void LoadMyTasks(int from, int count, int userID, bool searchPending, bool searchComplete)
        {
            string pendingQuery =
                @"SELECT
                    ST.CreatorID,
                    ST.DateCreated,
                    ST.Description,
                    ST.DueDate,
                    R.HasEmailSent,
                    R.IsDismissed,
                    ST.OrganizationID,
                    ST.TaskID,
                    ST.DateCompleted,
                    R.DueDate as ReminderDueDate,
                    ST.IsComplete,
                    ST.ParentID,
                    CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
                        ELSE ST.Name END AS Name,
                    ST.UserID,
                    ST.ModifierID,
					ST.DateModified,
                    R.ReminderID
                FROM 
                    Tasks ST
                    LEFT JOIN Tasks T 
                        ON ST.ParentID = T.TaskID
                    LEFT JOIN Reminders R
                        ON ST.TaskID = R.RefID
                        AND R.RefType = 61
                WHERE
                    ST.UserID = @UserID
                    AND ST.IsComplete = 0";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY CASE WHEN DueDate IS NULL THEN 1 ELSE 0 END, IsComplete ASC, DueDate ASC) AS 'RowNum' FROM q)
            SELECT
                TaskID
                , OrganizationID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , HasEmailSent
                , CreatorID
                , DateCreated
                , Name
                , DueDate
                , IsComplete
                , DateCompleted
                , ParentID
                , ReminderID
                , ModifierID
                , DateModified
            FROM 
                r
            WHERE
                RowNum BETWEEN @From AND @To";

            StringBuilder query;

            query = new StringBuilder(string.Format(pageQuery, pendingQuery));

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = query.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@From", from + 1);
                command.Parameters.AddWithValue("@To", from + count);
                Fill(command);
            }
        }

        public void LoadCompleted(int from, int count, int userID, bool searchPending, bool searchComplete)
        {
            string completeQuery =
                 @"SELECT
                    ST.CreatorID,
                    ST.DateCreated,
                    ST.Description,
                    ST.DueDate,
                    R.HasEmailSent,
                    R.IsDismissed,
                    ST.OrganizationID,
                    ST.TaskID,
                    ST.DateCompleted,
                    ST.ReminderDueDate,
                    ST.IsComplete,
                    ST.ParentID,
                    CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
                        ELSE ST.Name END AS Name,
                    ST.UserID
                FROM
                    Tasks ST
                    LEFT JOIN Tasks T 
                        ON ST.ParentID = T.TaskID
                    LEFT JOIN Reminders R
                        ON ST.TaskID = R.RefID
                        AND R.RefType = 61
                WHERE
                    (
                        ST.CreatorID = @UserID
                        OR ST.UserID = @UserID
                    )
                    AND ST.IsComplete = 0";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY IsComplete ASC, DateCompleted DESC, DueDate, ReminderDueDate) AS 'RowNum' FROM q)
            SELECT
                ReminderID
                , OrganizationID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , HasEmailSent
                , CreatorID
                , DateCreated
                , TaskName
                , TaskDueDate
                , TaskIsComplete
                , TaskDateCompleted
                , TaskParentID
            FROM 
                r
            WHERE
                RowNum BETWEEN @From AND @To";

            StringBuilder query;
            query = new StringBuilder(string.Format(pageQuery, completeQuery));

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = query.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@From", from + 1);
                command.Parameters.AddWithValue("@To", from + count);
                Fill(command);
            }
        }

        public void LoadByParentID(int parentID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Tasks WHERE ParentID = @ParentID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ParentID", parentID);
                Fill(command);
            }
        }

        public void LoadIncompleteByParentID(int parentID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Tasks WHERE ParentID = @ParentID AND IsComplete = 0";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ParentID", parentID);
                Fill(command);
            }
        }

        public void LoadByCompany(int from, int count, int organizationID)
        {
            //Paging implemented but currently excluded

            string completeQuery = @"
                SELECT
                    ST.CreatorID,
                    ST.DateCreated,
                    ST.Description,
                    ST.DueDate,
                    R.HasEmailSent,
                    R.IsDismissed,
                    ST.OrganizationID,
                    ST.TaskID,
                    ST.DateCompleted,
                    ST.ReminderDueDate,
                    ST.IsComplete,
                    ST.ParentID,
                    CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
                        ELSE ST.Name END AS Name,
                    ST.UserID
                FROM
                    Tasks ST
                    LEFT JOIN Tasks T 
                        ON ST.ParentID = T.TaskID
                    LEFT JOIN Reminders R
                        ON ST.TaskID = R.RefID
                        AND R.RefType = 61
                    JOIN TaskAssociations ta
                        ON ST.TaskID = ta.TaskID
                WHERE
                    ta.RefType = 9
                    AND ta.RefID = @organizationID";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY IsComplete asc, CASE WHEN DueDate IS NULL THEN 1 ELSE 0 END, IsComplete ASC, DueDate, ReminderDueDate) AS 'RowNum' FROM q)
            SELECT
                ReminderID
                , OrganizationID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , HasEmailSent
                , CreatorID
                , DateCreated
                , TaskName
                , TaskDueDate
                , TaskIsComplete
                , TaskDateCompleted
                , TaskParentID
            FROM 
                r";
            //WHERE
            //    RowNum BETWEEN @From AND @To";

            StringBuilder query;

            query = new StringBuilder(string.Format(pageQuery, completeQuery));

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = query.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@organizationID", organizationID);
                //command.Parameters.AddWithValue("@From", from + 1);
                //command.Parameters.AddWithValue("@To", from + count);
                Fill(command);
            }
        }

        public void LoadByContact(int from, int count, int contactID)
        {
            //Paging has been written for but is currently excluded.

            string completeQuery = @"
                SELECT
                    ST.CreatorID,
                    ST.DateCreated,
                    ST.Description,
                    ST.DueDate,
                    R.HasEmailSent,
                    R.IsDismissed,
                    ST.OrganizationID,
                    ST.TaskID,
                    ST.DateCompleted,
                    ST.ReminderDueDate,
                    ST.IsComplete,
                    ST.ParentID,
                    CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
                        ELSE ST.Name END AS Name,
                    ST.UserID
                FROM
                    Tasks ST
                    LEFT JOIN Tasks T 
                        ON ST.ParentID = T.TaskID
                    LEFT JOIN Reminders R
                        ON ST.TaskID = R.RefID
                        AND R.RefType = 61
                    JOIN TaskAssociations ta
                        ON ST.TaskID = ta.TaskID
                WHERE
                    ta.RefType = 32
                    AND ta.RefID = @contactID";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY IsComplete asc, CASE WHEN DueDate IS NULL THEN 1 ELSE 0 END, IsComplete ASC, DueDate, ReminderDueDate) AS 'RowNum' FROM q)
            SELECT
                ReminderID
                , OrganizationID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , HasEmailSent
                , CreatorID
                , DateCreated
                , TaskName
                , TaskDueDate
                , TaskIsComplete
                , TaskDateCompleted
                , TaskParentID
            FROM 
                r";
            //WHERE
            //    RowNum BETWEEN @From AND @To;

            StringBuilder query;

            query = new StringBuilder(string.Format(pageQuery, completeQuery));

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = query.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@contactID", contactID);
                //command.Parameters.AddWithValue("@From", from + 1);
                //command.Parameters.AddWithValue("@To", from + count);
                Fill(command);
            }
        }

        public void LoadByUser(int from, int count, int userID)
        {
            //Paging has been written for but is currently excluded.

            string completeQuery = @"
                SELECT
                    ST.CreatorID,
                    ST.DateCreated,
                    ST.Description,
                    ST.DueDate,
                    R.HasEmailSent,
                    R.IsDismissed,
                    ST.OrganizationID,
                    ST.TaskID,
                    ST.DateCompleted,
                    ST.ReminderDueDate,
                    ST.IsComplete,
                    ST.ParentID,
                    CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
                        ELSE ST.Name END AS Name,
                    ST.UserID
                FROM
                    Tasks ST
                    LEFT JOIN Tasks T 
                        ON ST.ParentID = T.TaskID
                    LEFT JOIN Reminders R
                        ON ST.TaskID = R.RefID
                        AND R.RefType = 61
                    JOIN TaskAssociations ta
                        ON ST.TaskID = ta.TaskID
                WHERE
                    ta.RefType = 22
                    AND ta.RefID = @userID";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY IsComplete asc, CASE WHEN DueDate IS NULL THEN 1 ELSE 0 END, IsComplete ASC, DueDate, ReminderDueDate) AS 'RowNum' FROM q)
            SELECT
                ReminderID
                , OrganizationID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , HasEmailSent
                , CreatorID
                , DateCreated
                , TaskName
                , TaskDueDate
                , TaskIsComplete
                , TaskDateCompleted
                , TaskParentID
            FROM 
                r";
            //WHERE
            //    RowNum BETWEEN @From AND @To;

            StringBuilder query;

            query = new StringBuilder(string.Format(pageQuery, completeQuery));

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = query.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@userID", userID);
                //command.Parameters.AddWithValue("@From", from + 1);
                //command.Parameters.AddWithValue("@To", from + count);
                Fill(command);
            }
        }

        public void LoadCompleteAssociatedToCompany(int from, int count, int organizationID)
        {
            string completeQuery = @"
            SELECT 
                t.*
            FROM
                Tasks t
                JOIN TaskAssociations ta
                    ON t.TaskID = ta.TaskID
            WHERE
                t.IsComplete = 1
                AND ta.RefType = 9
                AND ta.RefID = @organizationID";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY DueDate, ReminderDueDate) AS 'RowNum' FROM q)
            SELECT
                ReminderID
                , OrganizationID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , HasEmailSent
                , CreatorID
                , DateCreated
                , TaskName
                , TaskDueDate
                , TaskIsComplete
                , TaskDateCompleted
                , TaskParentID
            FROM 
                r
            WHERE
                RowNum BETWEEN @From AND @To";

            StringBuilder query;

            query = new StringBuilder(string.Format(pageQuery, completeQuery));

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = query.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@organizationID", organizationID);
                command.Parameters.AddWithValue("@From", from + 1);
                command.Parameters.AddWithValue("@To", from + count);
                Fill(command);
            }
        }

        public void LoadByUserAndAssociatedToCompany(int from, int count, int organizationID, int userID)
        {
            string completeQuery = @"
            SELECT 
                t.*
            FROM
                Tasks t
                JOIN TaskAssociations ta
                    ON t.TaskID = ta.TaskID
            WHERE
                (t.CreatorID = @UserID OR t.UserID = @UserID)
                AND ta.RefType = 9
                AND ta.RefID = @organizationID";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY DueDate, ReminderDueDate) AS 'RowNum' FROM q)
            SELECT
                ReminderID
                , OrganizationID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , HasEmailSent
                , CreatorID
                , DateCreated
                , TaskName
                , TaskDueDate
                , TaskIsComplete
                , TaskDateCompleted
                , TaskParentID
            FROM 
                r
            WHERE
                RowNum BETWEEN @From AND @To";

            StringBuilder query;

            query = new StringBuilder(string.Format(pageQuery, completeQuery));

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = query.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@organizationID", organizationID);
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@From", from + 1);
                command.Parameters.AddWithValue("@To", from + count);
                Fill(command);
            }
        }
    }
}
