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
  SELECT TOP 1 EmailPostID FROM EmailPosts WHERE LockProcessID IS NULL AND DATEADD(SECOND, HoldTime, DateCreated) < GETUTCDATE() ORDER BY DateCreated
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

    public static void DeleteImportEmails(LoginUser loginUser)
    {
      EmailPosts emailPosts = new EmailPosts(loginUser);

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM EmailPosts WHERE CreatorID = -2";
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
      PostEmail(loginUser, EmailPostType.WelcomePortalUser, -1, userID.ToString(), password, null, null, null, null, null, null);
    }
    public static void SendResetTSPassword(LoginUser loginUser, int userID, string password)
    {
      PostEmail(loginUser, EmailPostType.ResetTSPassword, -1, userID.ToString(), password, null, null, null, null, null, null);
    }
    public static void SendResetPortalPassword(LoginUser loginUser, int userID, string password)
    {
      PostEmail(loginUser, EmailPostType.ResetPortalPassword, -1, userID.ToString(), password, null, null, null, null, null, null);
    }
    public static void SendChangedTSPassword(LoginUser loginUser, int userID)
    {
      PostEmail(loginUser, EmailPostType.ChangedTSPassword, -1, userID.ToString(), null, null, null, null, null, null, null);
    }
    public static void SendChangedPortalPassword(LoginUser loginUser, int userID)
    {
      PostEmail(loginUser, EmailPostType.ChangedPortalPassword, -1, userID.ToString(), null, null, null, null, null, null, null);
    }
    public static void SendSignUpNotification(LoginUser loginUser, int userID)
    {
      PostEmail(loginUser, EmailPostType.InternalSignupNotification, -1, userID.ToString(), null, null, null, null, null, null, null);
    }

  }
}
