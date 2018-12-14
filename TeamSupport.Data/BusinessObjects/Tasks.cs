using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Configuration;
using System.Runtime.Serialization;


namespace TeamSupport.Data
{
    public partial class Task
    {


        public bool IsDismissed
        {
            get
            {
                if (Row.Table.Columns.Contains("IsDismissed") && Row["IsDismissed"] != DBNull.Value)
                {
                    return (bool)Row["IsDismissed"];
                }
                else return false;
            }
        }

        public DateTime? ReminderDueDate
        {
            get
            {
                if (Row.Table.Columns.Contains("ReminderDueDate") && Row["ReminderDueDate"] != DBNull.Value)
                {
                    return (DateTime)Row["ReminderDueDate"];
                }
                else return null;
            }
        }
    }

    public partial class Tasks
    {


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

		public void LoadByOrganizationID(int orgId)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "SELECT * FROM Tasks WHERE OrganizationID = @OrganizationID";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@OrganizationID", orgId);
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

     //   public void LoadByCompany(int from, int count, int organizationID)
     //   {
     //       //Paging implemented but currently excluded

     //       string completeQuery = @"
     //           SELECT
     //               ST.CreatorID,
     //               ST.DateCreated,
     //               ST.Description,
     //               ST.DueDate,
     //               R.DueDate AS 'ReminderDueDate',
     //               R.IsDismissed,
     //               ST.OrganizationID,
     //               ST.TaskID,
     //               ST.DateCompleted,
     //               ST.IsComplete,
     //               ST.ParentID,
     //               CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
     //                   ELSE ST.Name END AS Name,
     //               ST.UserID,
     //               ST.ModifierID,
					//ST.DateModified,
     //               R.ReminderID
     //           FROM
     //               Tasks ST
     //               LEFT JOIN Tasks T 
     //                   ON ST.ParentID = T.TaskID
     //               LEFT JOIN Reminders R
     //                   ON ST.TaskID = R.RefID
     //                   AND R.RefType = 61
     //               JOIN TaskAssociations ta
     //                   ON ST.TaskID = ta.TaskID
     //           WHERE
     //               ta.RefType = 9
     //               AND ta.RefID = @organizationID";

     //       string pageQuery = @"
     //       WITH 
     //           q AS ({0}),
     //           t AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY IsComplete asc, CASE WHEN DueDate IS NULL THEN 1 ELSE 0 END, IsComplete ASC, DueDate, ReminderDueDate) AS 'RowNum' FROM q)
     //       SELECT
     //           TaskID
     //           , OrganizationID
     //           , Description
     //           , DueDate
     //           , UserID
     //           , IsDismissed
     //           , CreatorID
     //           , DateCreated
     //           , Name
     //           , ReminderDueDate
     //           , IsComplete
     //           , DateCompleted
     //           , ParentID
     //           , ModifierID
     //           , DateModified
     //           , ReminderID
     //       FROM 
     //           t";
     //       //WHERE
     //       //    RowNum BETWEEN @From AND @To";

     //       StringBuilder query;

     //       query = new StringBuilder(string.Format(pageQuery, completeQuery));

     //       using (SqlCommand command = new SqlCommand())
     //       {
     //           command.CommandText = query.ToString();
     //           command.CommandType = CommandType.Text;
     //           command.Parameters.AddWithValue("@organizationID", organizationID);
     //           //command.Parameters.AddWithValue("@From", from + 1);
     //           //command.Parameters.AddWithValue("@To", from + count);
     //           Fill(command);
     //       }
     //   }

     //   public void LoadByContact(int from, int count, int contactID)
     //   {
     //       //Paging has been written for but is currently excluded.

     //       string completeQuery = @"
     //           SELECT
     //               ST.CreatorID,
     //               ST.DateCreated,
     //               ST.Description,
     //               ST.DueDate,
     //               R.DueDate AS 'ReminderDueDate',
     //               R.IsDismissed,
     //               ST.OrganizationID,
     //               ST.TaskID,
     //               ST.DateCompleted,
     //               ST.IsComplete,
     //               ST.ParentID,
     //               CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
     //                   ELSE ST.Name END AS Name,
     //               ST.UserID,
     //               ST.ModifierID,
					//ST.DateModified,
     //               R.ReminderID
     //           FROM
     //               Tasks ST
     //               LEFT JOIN Tasks T 
     //                   ON ST.ParentID = T.TaskID
     //               LEFT JOIN Reminders R
     //                   ON ST.TaskID = R.RefID
     //                   AND R.RefType = 61
     //               JOIN TaskAssociations ta
     //                   ON ST.TaskID = ta.TaskID
     //           WHERE
     //               ta.RefType = 32
     //               AND ta.RefID = @contactID";

     //       string pageQuery = @"
     //       WITH 
     //           q AS ({0}),
     //           t AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY IsComplete asc, CASE WHEN DueDate IS NULL THEN 1 ELSE 0 END, IsComplete ASC, DueDate, ReminderDueDate) AS 'RowNum' FROM q)
     //       SELECT
     //           TaskID
     //           , OrganizationID
     //           , Description
     //           , DueDate
     //           , UserID
     //           , IsDismissed
     //           , CreatorID
     //           , DateCreated
     //           , Name
     //           , ReminderDueDate
     //           , IsComplete
     //           , DateCompleted
     //           , ParentID
     //           , ModifierID
     //           , DateModified
     //           , ReminderID
     //       FROM 
     //           t";
     //       //WHERE
     //       //    RowNum BETWEEN @From AND @To;

     //       StringBuilder query;

     //       query = new StringBuilder(string.Format(pageQuery, completeQuery));

     //       using (SqlCommand command = new SqlCommand())
     //       {
     //           command.CommandText = query.ToString();
     //           command.CommandType = CommandType.Text;
     //           command.Parameters.AddWithValue("@contactID", contactID);
     //           //command.Parameters.AddWithValue("@From", from + 1);
     //           //command.Parameters.AddWithValue("@To", from + count);
     //           Fill(command);
     //       }
     //   }

     //   public void LoadByUser(int from, int count, int userID)
     //   {
     //       //Paging has been written for but is currently excluded.

     //       string completeQuery = @"
     //           SELECT
     //               ST.CreatorID,
     //               ST.DateCreated,
     //               ST.Description,
     //               ST.DueDate,
     //               R.DueDate AS 'ReminderDueDate',
     //               R.IsDismissed,
     //               ST.OrganizationID,
     //               ST.TaskID,
     //               ST.DateCompleted,
     //               ST.IsComplete,
     //               ST.ParentID,
     //               CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
     //                   ELSE ST.Name END AS Name,
     //               ST.UserID,
     //               ST.ModifierID,
					//ST.DateModified,
     //               R.ReminderID
     //           FROM
     //               Tasks ST
     //               LEFT JOIN Tasks T 
     //                   ON ST.ParentID = T.TaskID
     //               LEFT JOIN Reminders R
     //                   ON ST.TaskID = R.RefID
     //                   AND R.RefType = 61
     //               JOIN TaskAssociations ta
     //                   ON ST.TaskID = ta.TaskID
     //           WHERE
     //               ta.RefType = 22
     //               AND ta.RefID = @userID";

     //       string pageQuery = @"
     //       WITH 
     //           q AS ({0}),
     //           t AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY IsComplete asc, CASE WHEN DueDate IS NULL THEN 1 ELSE 0 END, IsComplete ASC, DueDate, ReminderDueDate) AS 'RowNum' FROM q)
     //       SELECT
     //           TaskID
     //           , OrganizationID
     //           , Description
     //           , DueDate
     //           , UserID
     //           , IsDismissed
     //           , CreatorID
     //           , DateCreated
     //           , Name
     //           , ReminderDueDate
     //           , IsComplete
     //           , DateCompleted
     //           , ParentID
     //           , ModifierID
     //           , DateModified
     //           , ReminderID
     //       FROM 
     //           t";
     //       //WHERE
     //       //    RowNum BETWEEN @From AND @To;

     //       StringBuilder query;

     //       query = new StringBuilder(string.Format(pageQuery, completeQuery));

     //       using (SqlCommand command = new SqlCommand())
     //       {
     //           command.CommandText = query.ToString();
     //           command.CommandType = CommandType.Text;
     //           command.Parameters.AddWithValue("@userID", userID);
     //           //command.Parameters.AddWithValue("@From", from + 1);
     //           //command.Parameters.AddWithValue("@To", from + count);
     //           Fill(command);
     //       }
     //   }

        public void LoadCompleteAssociatedToCompany(int from, int count, int organizationID)
        {
            string completeQuery = @"
            SELECT 
                t.*,
                r.IsDismissed,
                r.DueDate AS 'ReminderDueDate'
            FROM
                Tasks t
                JOIN TaskAssociations ta
                    ON t.TaskID = ta.TaskID
                LEFT JOIN Reminders r
                    ON t.TaskID = r.RefID
                    AND r.RefType = 61
            WHERE
                t.IsComplete = 1
                AND ta.RefType = 9
                AND ta.RefID = @organizationID";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                t AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY DueDate, ReminderDueDate) AS 'RowNum' FROM q)
            SELECT
                TaskID
                , OrganizationID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , CreatorID
                , DateCreated
                , Name
                , ReminderDueDate
                , IsComplete
                , DateCompleted
                , ParentID
                , ModifierID
                , DateModified
                , ReminderID
            FROM 
                t
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
                t.*,
                r.IsDismissed,
                r.DueDate AS 'ReminderDueDate'
            FROM
                Tasks t
                JOIN TaskAssociations ta
                    ON t.TaskID = ta.TaskID
                LEFT JOIN Reminders r
                    ON t.TaskID = r.RefID
                    AND r.RefType = 61
            WHERE
                (t.CreatorID = @UserID OR t.UserID = @UserID)
                AND ta.RefType = 9
                AND ta.RefID = @organizationID";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                t AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY DueDate, ReminderDueDate) AS 'RowNum' FROM q)
            SELECT
                TaskID
                , OrganizationID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , CreatorID
                , DateCreated
                , Name
                , ReminderDueDate
                , IsComplete
                , DateCompleted
                , ParentID
                , ModifierID
                , DateModified
                , ReminderID
            FROM 
                t
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

        public void LoadForIndexing(int organizationID, int max, bool isRebuilding)
        {
            using (SqlCommand command = new SqlCommand())
            {
                string text = @"
                    SELECT 
                      TOP {0} 
                      TaskID
                    FROM 
                      Tasks t WITH(NOLOCK)
                    WHERE 
                      t.NeedsIndexing = 1
                      AND t.OrganizationID = @OrganizationID
                    ORDER BY 
                      t.DateModified DESC";

                if (isRebuilding)
                {
                    text = @"
                      SELECT 
                        TaskID
                      FROM 
                        Tasks t WITH(NOLOCK)
                      WHERE 
                        t.OrganizationID = @OrganizationID
                      ORDER BY 
                        t.DateModified DESC";
                }

                command.CommandText = string.Format(text, max.ToString());
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        public void LoadByItemAll(ReferenceType refType, int refID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                    SELECT
                        t.*
                    FROM
                        Tasks t
                        JOIN TaskAssociations ta
                            ON t.TaskID = ta.TaskID
                    WHERE
                        ta.RefType = @RefType
                        AND ta.RefID = @RefID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@RefType", refType);
                command.Parameters.AddWithValue("@RefID", refID);
                Fill(command);
            }
        }

        public void LoadbyUserMonth(DateTime date, int userID, int orgID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                    SELECT
                        *
                    FROM
                        Tasks
                    WHERE
                        Month(DueDate) = @month
                        AND Year(DueDate) = @year
                        AND OrganizationID = @OrgID
                        AND IsComplete = 0
                        AND
                        (
                            CreatorID = @UserID
                            OR UserID = @UserID
                        )";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@month", date.Month);
                command.Parameters.AddWithValue("@year", date.Year);
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@OrgID", orgID);
                Fill(command);
            }
        }

        public void LoadbyGroupMonth(DateTime date, int groupID, int orgID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                    SELECT
                        t.*
                    FROM
                        Tasks t
                        JOIN TaskAssociations ta
                            ON t.TaskID = ta.TaskID
                    WHERE
                        Month(t.DueDate) = @month
                        AND Year(t.DueDate) = @year
                        AND t.IsComplete = 0
                        AND ta.RefType = 6
                        AND ta.RefID = @groupID
                        AND t.OrganizationID = @OrgID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@month", date.Month);
                command.Parameters.AddWithValue("@year", date.Year);
                command.Parameters.AddWithValue("@groupID", groupID);
                command.Parameters.AddWithValue("@OrgID", orgID);
                Fill(command);
            }
        }

        public void LoadbyCompanyMonth(DateTime date, int companyID, int orgID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                    SELECT
                        t.*
                    FROM
                        Tasks t
                        JOIN TaskAssociations ta
                            ON t.TaskID = ta.TaskID
                    WHERE
                        Month(t.DueDate) = @month
                        AND Year(t.DueDate) = @year
                        AND t.OrganizationID = @OrgID
                        AND t.IsComplete = 0
                        AND ta.RefType = 9
                        AND ta.RefID = @companyID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@month", date.Month);
                command.Parameters.AddWithValue("@year", date.Year);
                command.Parameters.AddWithValue("@companyID", companyID);
                command.Parameters.AddWithValue("@OrgID", orgID);
                Fill(command);
            }
        }
    }
}
