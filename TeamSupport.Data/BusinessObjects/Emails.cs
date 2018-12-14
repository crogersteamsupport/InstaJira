using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.IO;

namespace TeamSupport.Data
{
  public partial class Email
  {
    public MailMessage GetMailMessage()
    {
      MailMessage message = new MailMessage();
      message.From = GetEmailAddressFromString(FromAddress);
      AddEmailAddressesFromString(message.To, ToAddress);
      AddEmailAddressesFromString(message.CC, CCAddress);
      AddEmailAddressesFromString(message.Bcc, BCCAddress);
      message.Subject = Subject;
      message.Body = Body;
      message.IsBodyHtml = IsHtml;
      if (IsHtml)
      {
        //string img = "<div><img src=\"http://integrate.teamsupport.com/MailRead.aspx?OrganizationID={0}&EmailID={1}\" alt=\"\" width=\"0\" height=\"0\" style=\"width: 0px; height: 0px, border: 0px;\"/></div>";
        //message.Body = Body + string.Format(img, OrganizationID, EmailID);
      }
      if (!string.IsNullOrEmpty(Attachments))
      {
        foreach (string fileName in Attachments.Split(';'))
        {
          try
          {
            message.Attachments.Add(new System.Net.Mail.Attachment(fileName));
          }
          catch (Exception)
          {

          }
        }
      }

      return message;
    }

    public string[] GetAttachments()
    {
      List<string> results = new List<string>();

      if (!string.IsNullOrEmpty(Attachments))
      {
        foreach (string fileName in Attachments.Split(';'))
        {
          results.Add(fileName);
        }
      }

      return results.ToArray();
    }

    private void AddEmailAddressesFromString(MailAddressCollection collection, string text)
    {
      if (string.IsNullOrEmpty(text.Trim())) return;
      string[] list = text.Split('|');
      foreach (string s in list)
      {
        MailAddress address = GetEmailAddressFromString(s);
        if (address != null) collection.Add(address);
      }

    }

    private MailAddress GetEmailAddressFromString(string text)
    {
      string name = "";
      int start = text.IndexOf('"');
      int end = -1;
      if (start > -1)
      {
        start++;
        end = text.IndexOf('"', start + 1);
        if (end > -1)
        {
          name = text.Substring(start, end - start).Trim();
        }
      }

      if (name == "") return new MailAddress(text);
      string address = text;

      start = text.IndexOf('<');
      end = -1;
      if (start > -1)
      {
        start++;
        end = text.IndexOf('>', start + 1);
        if (end > -1)
        {
          address = text.Substring(start, end - start).Trim();
        }
      }

      return new MailAddress(address, name);
    }

  }

  public partial class Emails
  {
    public void LoadWaiting()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT * FROM Emails WHERE IsWaiting = 1 AND NextAttempt < GETUTCDATE() ORDER BY DateCreated DESC";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }

    public void LoadTop100Waiting()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT TOP 100 * FROM Emails WHERE IsWaiting = 1 AND NextAttempt < GETUTCDATE() ORDER BY DateCreated DESC";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }

    public static bool IsEmailDisabled(LoginUser loginUser, int userID, string setting)
    {
        bool doDisableEmails = false;
        bool.TryParse(UserSettings.ReadString(loginUser, userID, setting, "False", "Email"), out doDisableEmails);
        return doDisableEmails;
    }


    public static Email GetNextWaiting(LoginUser loginUser, string processID)
    {
      Emails emails = new Emails(loginUser);

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
UPDATE Emails
SET LockProcessID = @ProcessID
OUTPUT Inserted.*
WHERE EmailID IN (
  SELECT TOP 1 EmailID FROM Emails WHERE LockProcessID IS NULL AND IsWaiting = 1 AND NextAttempt < GETUTCDATE() ORDER BY Attempts, Size, DateCreated
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

        public static bool IsTrialEmailOverLimit(LoginUser loginUser, int organizationID)
        {
            if (!GetIsTrialLimited(loginUser, organizationID)) return false;
            int maxEmails = Emails.GetMaxEmailsForTrial(loginUser, organizationID);
            int count = Emails.GetSentCount(loginUser, organizationID);
            return maxEmails >= count;
        }

        public static bool GetIsTrialLimited(LoginUser loginUser, int organizationID)
        {
            return OrganizationSettings.ReadString(loginUser, organizationID, "Emails_TrialLimited", "1") == "1";
        }

        public static int GetMaxEmailsForTrial(LoginUser loginUser, int organizationID)
        {
            int maxEmails = 5;
            int.TryParse(OrganizationSettings.ReadString(loginUser, organizationID, "Emails_MaxForTrial", "50"), out maxEmails);
            return maxEmails;
        }

        public static void SetMaxEmailsForTrial(LoginUser loginUser, int organizationID, int value)
        {
            OrganizationSettings.WriteString(loginUser, "Emails_MaxForTrial", value.ToString());
        }

        public static void SetTrialLimited(LoginUser loginUser, int organizationID, bool value)
        {
            OrganizationSettings.WriteString(loginUser, "Emails_TrialLimited", value ? "1" : "0");
        }

        public static int GetSentCount(LoginUser loginUser, int organizationID)
        {
            Emails emails = new Emails(loginUser);
            object o;

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
SELECT COUNT(*) FROM Emails WITH(NOLOCK)
WHERE IsWaiting = 0
AND OrganizationID = @OrganizationID
";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
				o = emails.ExecuteScalar(command);
            }

            if (o == null || o == DBNull.Value) return 0;
            return (int)o;
        }

        public static void UnlockAll(LoginUser loginUser)
    {
      Emails emails = new Emails(loginUser);

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE Emails SET LockProcessID = NULL WHERE IsWaiting=1";
        command.CommandType = CommandType.Text;
        emails.ExecuteNonQuery(command);
      }
    }

    public static void UnlockThread(LoginUser loginUser, int threadNumber)
    {
        Emails emails = new Emails(loginUser);

        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "UPDATE Emails SET LockProcessID = NULL WHERE IsWaiting=1 AND LockProcessID = @id";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("id", threadNumber);
            emails.ExecuteNonQuery(command);
        }
    }

    public static string EmailAddressToString(MailAddressCollection addresses)
    {
        StringBuilder builder = new StringBuilder();

        foreach (MailAddress address in addresses)
        {
        if (builder.Length > 0) builder.Append("|");
        if (!string.IsNullOrEmpty(address.DisplayName))
        {
            builder.Append(string.Format("\"{0}\" <{1}>", address.DisplayName, address.Address));
        }
        else
	        {
            builder.Append(address.Address);
	        }
        }

        return builder.ToString();
    }

    public Email AddEmail(int organizationID, string description, string subject, string body, bool isHtml, string from, string to, string cc = "", string bcc = "", string[] attachmentFileNames = null, DateTime? timeToSend = null)
    {
        Email email = AddNewEmail();

        email.OrganizationID = organizationID;
        email.Description = description;
        email.FromAddress = from;
        email.ToAddress = to;
        email.CCAddress = cc;
        email.BCCAddress = bcc;
        email.Subject = subject;
        email.Body = body;
        email.IsHtml = isHtml;
        email.DateSent = null;
        email.LastFailedReason = "";
        email.IsSuccess = false;
        email.NextAttempt = timeToSend == null ? DateTime.UtcNow : (DateTime)timeToSend;
        email.IsWaiting = !string.IsNullOrWhiteSpace(body);
        email.Attempts = 0;

        List<string> attachments = new List<string>();
        int size = 0;
        if (attachmentFileNames != null)
        {
            foreach (string fileName in attachmentFileNames)
            {
                if (File.Exists(fileName))
                {
                    attachments.Add(fileName);
                    size = size + (int)((new FileInfo(fileName)).Length / 1024);
                }
            }
        }
        if (attachments.Count > 0) email.Attachments = string.Join(";", attachments.ToArray());
        if (!string.IsNullOrWhiteSpace(email.Body))
        {
            size = size + (ASCIIEncoding.ASCII.GetByteCount(email.Body) / 1024);
        }
        email.Size = size;
        return email;


    }


    public Email AddEmail(int organizationID, int? emailPostID, string description, MailMessage message, string[] attachmentFileNames = null, DateTime? timeToSend = null)
    {
        Email email = AddNewEmail();

        email.OrganizationID = organizationID;
        email.Description = description;
        email.FromAddress = message.From.ToString();
        email.ToAddress = EmailAddressToString(message.To);
        email.CCAddress = EmailAddressToString(message.CC);
        email.BCCAddress = EmailAddressToString(message.Bcc);
        email.Subject = message.Subject;
        email.Body = message.Body;
        email.IsHtml = message.IsBodyHtml;
        email.DateSent = null;
        email.LastFailedReason = "";
        email.IsSuccess = false;
        email.NextAttempt = timeToSend == null ? DateTime.UtcNow : (DateTime) timeToSend;
        email.IsWaiting = !string.IsNullOrWhiteSpace(message.Body);
        email.EmailPostID = emailPostID;
        email.Attempts = 0;

        List<string> attachments = new List<string>();
        int size = 0;
        if (attachmentFileNames != null)
        {
        foreach (string fileName in attachmentFileNames)
        {
            if (File.Exists(fileName))
            {
            attachments.Add(fileName);
            size = size + (int)((new FileInfo(fileName)).Length / 1024);
            }
        }
        }
        if (attachments.Count > 0) email.Attachments = string.Join(";", attachments.ToArray());
        if (!string.IsNullOrWhiteSpace(email.Body))
        {
        size = size + (ASCIIEncoding.ASCII.GetByteCount(email.Body) / 1024);
        }
        email.Size = size;
        return email;
    }

    public static Email AddEmail(LoginUser loginUser, int organizationID, int? emailPostID, string description, MailMessage message, string[] attachmentFileNames = null, DateTime? timeToSend = null)
    {
      Emails emails = new Emails(loginUser);
      Email email = emails.AddEmail(organizationID, emailPostID, description, message, attachmentFileNames, timeToSend);
      emails.Save();
      return email;
    }

    public static Email AddEmail(LoginUser loginUser, int organizationID, string description, string subject, string body, bool isHtml, string from, string to, string cc = "", string bcc = "", string[] attachmentFileNames = null, DateTime? timeToSend = null)
    {
            Emails emails = new Emails(loginUser);
            Email email = emails.AddEmail(organizationID, description, subject, body, isHtml, from, to, cc, bcc, attachmentFileNames, timeToSend);
            emails.Save();
            return email;
        }

    }

}
