using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
	public partial class EmailPost
	{
	}

	public partial class EmailPosts
	{
		public void LoadAll()
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "SELECT * FROM EmailPosts ORDER BY DateCreated";
				command.CommandType = CommandType.Text;
				Fill(command);
			}
		}

		public virtual void LoadByRecentUserID(int userID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "Select *  FROM [dbo].[EmailPosts] WHERE ([Param8] Like @userID AND DateCreated > DATEADD(MINUTE, -1, GETUTCDATE()))";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@userID", "%" + userID + "%");
				try
				{
					Fill(command);
				}
				catch (Exception e)
				{
				}
			}
		}

        public virtual void LoadByRecentUserID(int userID, List<int> exclude)
		{
			using (SqlCommand command = new SqlCommand())
			{
                string sql = "Select *  FROM [dbo].[EmailPosts] WHERE ([Param8] Like @userID AND DateCreated > DATEADD(MINUTE, -1, GETUTCDATE())) ";

                if (exclude != null && exclude.Any())
                {
                    sql += " AND EmailPostID NOT IN (" + string.Join(",", exclude.ToArray()) + ")";
                }

                command.CommandText = sql;
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@userID", "%" + userID + "%");
				try
				{
					Fill(command);
				}
				catch (Exception e)
				{
				}
			}
		}

		public virtual void LoadByTicketId(int ticketId)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "Select *  FROM [dbo].[EmailPosts] WHERE ([Param1] = @ticketId)";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@ticketId", ticketId);

				try
				{
					Fill(command);
				}
				catch (Exception e)
				{
				}
			}
		}

		public static EmailPost GetNextWaiting(LoginUser loginUser, string processID)
		{
			EmailPosts emails = new EmailPosts(loginUser);

			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = @"
UPDATE EmailPosts 
SET LockProcessID = @ProcessID 
OUTPUT Inserted.*
WHERE EmailPostID IN (
  SELECT TOP 1 EmailPostID FROM EmailPosts WHERE LockProcessID IS NULL AND CreatorID <> -5 AND DATEADD(SECOND, HoldTime, DateCreated) < GETUTCDATE() ORDER BY DateCreated
)
";

				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@ProcessID", processID);
				emails.Fill(command);
			}

			if (emails.IsEmpty)
				return null;
			else
				return emails[0];
		}

		public static EmailPost GetDebugNextWaiting(LoginUser loginUser, string processID, int orgID)
		{
			EmailPosts emails = new EmailPosts(loginUser);

			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = @"
UPDATE EmailPosts 
SET LockProcessID = @ProcessID 
OUTPUT Inserted.*
WHERE EmailPostID IN (
  SELECT TOP 1 ep.EmailPostID 
  FROM EmailPosts AS ep
  LEFT JOIN Tickets t ON t.TicketID = CAST(ep.Param1 AS INT)
  LEFT JOIN Users u ON u.UserID = ep.CreatorID
  WHERE ep.LockProcessID IS NULL 
  AND DATEADD(SECOND, 15, ep.DateCreated) < GETUTCDATE() 
  AND (u.OrganizationID = @OrganizationID OR t.OrganizationID = @OrganizationID)
  ORDER BY ep.DateCreated
)
";

				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@ProcessID", processID);
				command.Parameters.AddWithValue("@OrganizationID", orgID);
				emails.Fill(command);
			}

			if (emails.IsEmpty)
				return null;
			else
				return emails[0];
		}

		public static void DeleteImportEmailsWithRetryOnDeadLock(LoginUser loginUser)
		{
			short retryCount = 0;

			while (retryCount < 5)
			{
				try
				{
					DeleteImportEmails(loginUser);
					retryCount = 5;
				}
				catch (System.Data.SqlClient.SqlException ex)
				{
					if (ex.Number == 1205)// Deadlock                         
						retryCount++;
					else
						throw;
				}
			}
		}

		public static void DeleteImportEmails(LoginUser loginUser)
		{
			EmailPosts emailPosts = new EmailPosts(loginUser);

			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "DELETE FROM EmailPosts WHERE CreatorID = -5";
				command.CommandType = CommandType.Text;
				emailPosts.ExecuteNonQuery(command, "EmailPosts");
			}
		}

		public static void UnlockAll(LoginUser loginUser)
		{
			EmailPosts emailPosts = new EmailPosts(loginUser);

			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "UPDATE EmailPosts SET LockProcessID = NULL";
				command.CommandType = CommandType.Text;
				emailPosts.ExecuteNonQuery(command);
			}
		}

        public static void UnlockThread(LoginUser loginUser, int thread)
        {
            EmailPosts emailPosts = new EmailPosts(loginUser);

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE EmailPosts SET LockProcessID = NULL WHERE LockProcessID = @id";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("id", thread);
                emailPosts.ExecuteNonQuery(command);
            }
        }

        private static void PostEmail(LoginUser loginUser, EmailPostType emailPostType, int holdTime, string param1, string param2, string param3, string param4, string param5, string text1, string text2, string text3)
		{
			EmailPosts emailPosts = new EmailPosts(loginUser);
			EmailPost emailPost = emailPosts.AddNewEmailPost();
			emailPost.DateCreated = DateTime.UtcNow;
			emailPost.CreatorID = loginUser.UserID;
			emailPost.EmailPostType = emailPostType;
			emailPost.HoldTime = holdTime;
			emailPost.Param1 = param1;
			emailPost.Param2 = param2;
			emailPost.Param3 = param3;
			emailPost.Param4 = param4;
			emailPost.Param5 = param5;
			emailPost.Text1 = text1;
			emailPost.Text2 = text2;
			emailPost.Text3 = text3;
			emailPosts.Save();
		}

		public static void SendTicketUpdateRequest(LoginUser loginUser, int ticketID)
		{
			PostEmail(loginUser, EmailPostType.TicketUpdateRequest, -1, ticketID.ToString(), loginUser.UserID.ToString(), null, null, null, null, null, null);
		}
		public static void SendWelcomeNewSignup(LoginUser loginUser, int userID, string password)
		{
			PostEmail(loginUser, EmailPostType.WelcomeNewSignup, -1, userID.ToString(), password, null, null, null, null, null, null);
		}
		public static void SendWelcomeTSUser(LoginUser loginUser, int userID, string password)
		{
			PostEmail(loginUser, EmailPostType.WelcomeTSUser, -1, userID.ToString(), password, null, null, null, null, null, null);
		}
		public static void SendWelcomePortalUser(LoginUser loginUser, int userID, string password)
		{
			PostEmail(loginUser, EmailPostType.WelcomePortalUser, 120, userID.ToString(), password, null, null, null, null, null, null);
		}
		public static void SendResetTSPassword(LoginUser loginUser, int userID, string password)
		{
			PostEmail(loginUser, EmailPostType.ResetTSPassword, -1, userID.ToString(), password, null, null, null, null, null, null);
		}
		public static void SendResetPortalPassword(LoginUser loginUser, int userID, string password)
		{
			PostEmail(loginUser, EmailPostType.ResetPortalPassword, -1, userID.ToString(), password, null, null, null, null, null, null);
		}
		public static void SendResetCustomerHubPassword(LoginUser loginUser, int userID, string password, int? productFamilyID = null)
		{
			PostEmail(loginUser, EmailPostType.ResetCustomerHubPassword, -1, userID.ToString(), password, productFamilyID.ToString(), null, null, null, null, null);
		}
        public static void SendWelcomeCustomerHubUser(LoginUser loginUser, int userID, string password, int? productFamilyID = null)
        {
            PostEmail(loginUser, EmailPostType.WelcomeCustomerHubUser, -1, userID.ToString(), password, productFamilyID.ToString(), null, null, null, null, null);
        }
        public static void SendChangedTSPassword(LoginUser loginUser, int userID)
		{
			PostEmail(loginUser, EmailPostType.ChangedTSPassword, -1, userID.ToString(), null, null, null, null, null, null, null);
		}
		public static void SendNewDevice(LoginUser loginUser, int userID)
		{
			PostEmail(loginUser, EmailPostType.NewDevice, -1, userID.ToString(), null, null, null, null, null, null, null);
		}
		public static void SendTooManyAttempts(LoginUser loginUser, int userID)
		{
			PostEmail(loginUser, EmailPostType.TooManyAttempts, -1, userID.ToString(), null, null, null, null, null, null, null);
		}
		public static void SendChangedPortalPassword(LoginUser loginUser, int userID)
		{
			PostEmail(loginUser, EmailPostType.ChangedPortalPassword, -1, userID.ToString(), null, null, null, null, null, null, null);
		}
		public static void SendSignUpNotification(LoginUser loginUser, int userID, string url, string referrer)
		{
			PostEmail(loginUser, EmailPostType.InternalSignupNotification, -1, userID.ToString(), url, referrer, null, null, null, null, null);
		}

	}
}
