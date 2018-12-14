using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public class ParticipantInfo
  {
    public ParticipantInfo(string firstName, string lastName, string email, string companyName, bool isOnline)
    {
      FirstName = firstName;
      LastName = lastName;
      Email = email;
      CompanyName = companyName;
      IsOnline = isOnline;
    }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string CompanyName { get; set; }
    public bool IsOnline { get; set; }
  }

  
  public partial class ChatParticipant
  {
    private ParticipantInfo _participantInfo = null;

    public string FirstName 
    { 
      get 
      {
        if (Row["FirstName"] == null)
        {
          if (_participantInfo == null)
          {
            _participantInfo = GetParticipantInfo(Collection.LoginUser, ParticipantID, ParticipantType);
          }
          return _participantInfo.FirstName;
        }
        return Row["FirstName"].ToString();
      }
    }

    public string LastName 
    { 
      get 
      {
        if (Row["LastName"] == null)
        {
          if (_participantInfo == null)
          {
            _participantInfo = GetParticipantInfo(Collection.LoginUser, ParticipantID, ParticipantType);
          }
          return _participantInfo.LastName;
        }
        return Row["LastName"].ToString();
      }
    }

    public string Email 
    { 
      get 
      {
        if (Row["Email"] == null)
        {
          if (_participantInfo == null)
          {
            _participantInfo = GetParticipantInfo(Collection.LoginUser, ParticipantID, ParticipantType);
          }
          return _participantInfo.Email;
        }
        return Row["Email"].ToString();
      }
    }

    public string CompanyName 
    { 
      get 
      {
        if (Row["CompanyName"] == null)
        {
          if (_participantInfo == null)
          {
            _participantInfo = GetParticipantInfo(Collection.LoginUser, ParticipantID, ParticipantType);
          }
          return _participantInfo.CompanyName;
        }
        return Row["CompanyName"].ToString();
      }
    }

    public bool IsOnline 
    { 
      get 
      {
        if (Row["IsOnline"] == null)
        {
          if (_participantInfo == null)
          {
            _participantInfo = GetParticipantInfo(Collection.LoginUser, ParticipantID, ParticipantType);
          }
          return _participantInfo.IsOnline;
        }
        return (bool)Row["IsOnline"];
      }
    }

    public static ParticipantInfo GetParticipantInfo(LoginUser loginUser, int id, ChatParticipantType type)
    {
      ParticipantInfo result = null;
      switch (type)
      {
        case ChatParticipantType.User:
          UsersViewItem user = UsersView.GetUsersViewItem(loginUser, id);
          if (user != null) result = new ParticipantInfo(user.FirstName, user.LastName, user.Email, user.Organization, user.IsOnline);
          break;
        case ChatParticipantType.External:
          ChatClientsViewItem client = ChatClientsView.GetChatClientsViewItem(loginUser, id);
          if (client != null) result = new ParticipantInfo(client.FirstName, client.LastName, client.Email, client.CompanyName, client.IsOnline);
          break;
        default:
          break;
      }
      return result;
    }

  }
  
  public partial class ChatParticipants
  {

    public static ChatParticipant GetChatParticipant(LoginUser loginUser, int id, ChatParticipantType type, int chatID)
    { 
      ChatParticipants chatParticipants = new ChatParticipants(loginUser);

      using (SqlCommand command = new SqlCommand())
      {
//SELECT * FROM ChatParticipants WHERE ChatID = @ChatID AND ParticipantType = @ParticipantType AND ParticipantID = @ParticipantID
        if (type == ChatParticipantType.External)
        {
          command.CommandText = @"
SELECT cp.*, cc.FirstName, cc.LastName, cc.Email, cc.CompanyName,
CASE WHEN DATEDIFF(second, cc.LastPing, GETUTCDATE()) < 15 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsOnline
FROM ChatParticipants cp
LEFT JOIN ChatClients cc ON cc.ChatClientID = cp.ParticipantID
WHERE cp.ChatID = @ChatID
AND cp.ParticipantType = 1
AND cp.ParticipantID = @ParticipantID
";
        
        }
        else
        {
          command.CommandText = @"
SELECT cp.*, u.FirstName, u.LastName, u.Email, o.Name, 
CASE WHEN DATEDIFF(second, u.LastPing, GETUTCDATE()) < 15 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsOnline
FROM ChatParticipants cp
LEFT JOIN Users u ON u.UserID = cp.ParticipantID
LEFT JOIN Organizations o ON o.OrganizationID = u.OrganizationID
WHERE cp.ChatID = @ChatID
AND cp.ParticipantType = 0
AND cp.ParticipantID = @ParticipantID
";

        }
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ChatID", chatID);
        command.Parameters.AddWithValue("@ParticipantID", id);
        chatParticipants.Fill(command);
      }
      if (chatParticipants.IsEmpty) return null;
      return chatParticipants[0];
    }

    public void LoadByUserID(int userID, int organizationID)
    {
      Chats chats = new Chats(LoginUser);
      chats.LoadByUserID(userID, organizationID);
      if (chats.IsEmpty) return;

      StringBuilder builder = new StringBuilder();

      foreach (Chat chat in chats)
      {
        if (builder.Length > 0) builder.Append(",");
        builder.Append(chat.ChatID.ToString());
      }

      using (SqlCommand command = new SqlCommand())
      {
        string text = @"
(
SELECT cp.*, cc.FirstName, cc.LastName, cc.Email, cc.CompanyName, 
CASE WHEN DATEDIFF(second, cc.LastPing, GETUTCDATE()) < 15 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsOnline,
ISNULL((SELECT TOP 1 cm.ChatMessageID FROM ChatMessages cm WHERE cm.ChatID= cp.ChatID ORDER BY cm.ChatMessageID DESC), 0) AS 'LastPostedMessageID',
ISNULL((SELECT TOP 1 cp2.LastMessageID FROM ChatParticipants cp2 WHERE cp2.ChatID= cp.ChatID AND cp2.ParticipantType=0 AND cp2.ParticipantID=@UserID), 0) AS 'MyLastMessageID'
FROM ChatParticipants cp
LEFT JOIN ChatClients cc ON cc.ChatClientID = cp.ParticipantID
WHERE cp.ChatID IN ({0})
--AND cp.DateLeft IS NULL
AND cp.ParticipantType = 1

UNION

SELECT cp.*, u.FirstName, u.LastName, u.Email, o.Name, 
CASE WHEN DATEDIFF(second, u.LastPing, GETUTCDATE()) < 15 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsOnline,
ISNULL((SELECT TOP 1 cm.ChatMessageID FROM ChatMessages cm WHERE cm.ChatID= cp.ChatID ORDER BY cm.ChatMessageID DESC), 0) AS 'LastPostedMessageID',
ISNULL((SELECT TOP 1 cp2.LastMessageID FROM ChatParticipants cp2 WHERE cp2.ChatID= cp.ChatID AND cp2.ParticipantType=0 AND cp2.ParticipantID=@UserID), 0) AS 'MyLastMessageID'
FROM ChatParticipants cp
LEFT JOIN Users u ON u.UserID = cp.ParticipantID
LEFT JOIN Organizations o ON o.OrganizationID = u.OrganizationID
WHERE NOT (cp.ParticipantID = @UserID AND cp.ParticipantType = 0) 
AND cp.ChatID IN ({0})
--AND cp.DateLeft IS NULL
AND cp.ParticipantType = 0
)
ORDER BY ChatID

";

        command.CommandText = string.Format(text, builder.ToString());
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command);
      }
    }

    public void LoadByChatID(int chatID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string text = @"
(
SELECT cp.*, cc.FirstName, cc.LastName, cc.Email, cc.CompanyName, 
CASE WHEN DATEDIFF(second, cc.LastPing, GETUTCDATE()) < 15 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsOnline
FROM ChatParticipants cp
LEFT JOIN ChatClients cc ON cc.ChatClientID = cp.ParticipantID
WHERE cp.ChatID = @ChatID
AND cp.DateLeft IS NULL
AND cp.ParticipantType = 1

UNION

SELECT cp.*, u.FirstName, u.LastName, u.Email, o.Name, 
CASE WHEN DATEDIFF(second, u.LastPing, GETUTCDATE()) < 15 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsOnline
FROM ChatParticipants cp
LEFT JOIN Users u ON u.UserID = cp.ParticipantID
LEFT JOIN Organizations o ON o.OrganizationID = u.OrganizationID
WHERE cp.ChatID = @ChatID
AND cp.DateLeft IS NULL
AND cp.ParticipantType = 0
)
ORDER BY ChatID

";

        command.CommandText = text;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ChatID", chatID);
        Fill(command);
      }
    }

    public void LoadExternalDisconnected(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string text = @"
SELECT cp.*, cc.FirstName, cc.LastName, cc.Email, cc.CompanyName, 
CASE WHEN DATEDIFF(second, cc.LastPing, GETUTCDATE()) < 15 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsOnline
FROM ChatParticipants cp
LEFT JOIN ChatClients cc ON cc.ChatClientID = cp.ParticipantID AND cp.ParticipantType=1 AND cp.DateLeft IS NULL
WHERE DATEADD(second, 60, cc.LastPing) < GETUTCDATE()
AND cp.ParticipantType = 1
AND cc.OrganizationID=@OrganizationID
";

        command.CommandText = text;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadTypers(int id, ChatParticipantType participantType, int chatID, int seconds)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"  
SELECT cp.*, cc.FirstName, cc.LastName, cc.Email, cc.CompanyName, 
CASE WHEN DATEDIFF(second, cc.LastPing, GETUTCDATE()) < 15 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsOnline
FROM ChatParticipants cp
LEFT JOIN ChatClients cc ON cc.ChatClientID = cp.ParticipantID
WHERE GETUTCDATE() < DATEADD(second, @Seconds, cp.LastTyped)
AND cp.ChatID = @ChatID
AND NOT (cp.ParticipantType = @ParticipantType AND cp.ParticipantID = @ParticipantID)
AND cp.ParticipantTYpe = 1

UNION

SELECT cp.*, u.FirstName, u.LastName, u.Email, o.Name, 
CASE WHEN DATEDIFF(second, u.LastPing, GETUTCDATE()) < 15 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsOnline
FROM ChatParticipants cp
LEFT JOIN Users u ON u.UserID = cp.ParticipantID
LEFT JOIN Organizations o ON o.OrganizationID = u.OrganizationID
WHERE GETUTCDATE() < DATEADD(second, @Seconds, cp.LastTyped)
AND cp.ChatID = @ChatID
AND NOT (cp.ParticipantType = @ParticipantType AND cp.ParticipantID = @ParticipantID)
AND cp.ParticipantTYpe = 0
";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ChatID", chatID);
        command.Parameters.AddWithValue("@ParticipantType", (int)participantType);
        command.Parameters.AddWithValue("@Seconds", seconds);
        command.Parameters.AddWithValue("@ParticipantID", id);
        Fill(command);
      }
    }

    public static void UpdateTyping(LoginUser loginUser, int id, ChatParticipantType participantType, int chatID)
    {
      ChatParticipants chatParticipants = new ChatParticipants(loginUser);

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"  
UPDATE ChatParticipants SET LastTyped = GETUTCDATE()
WHERE ChatID = @ChatID AND ParticipantType = @ParticipantType AND ParticipantID = @ParticipantID
";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ChatID", chatID);
        command.Parameters.AddWithValue("@ParticipantType", (int)participantType);
        command.Parameters.AddWithValue("@ParticipantID", id);
        chatParticipants.ExecuteNonQuery(command, "ChatParticipants");
      }
    }

    public static void UpdateLastMessageID(LoginUser loginUser, int id, ChatParticipantType participantType, int chatID, int messageID)
    {
      ChatParticipants chatParticipants = new ChatParticipants(loginUser);

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"  
UPDATE ChatParticipants SET LastMessageID = @LastMessageID
WHERE ChatID = @ChatID AND ParticipantType = @ParticipantType AND ParticipantID = @ParticipantID
";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ChatID", chatID);
        command.Parameters.AddWithValue("@ParticipantType", (int)participantType);
        command.Parameters.AddWithValue("@ParticipantID", id);
        command.Parameters.AddWithValue("@LastMessageID", messageID);
        chatParticipants.ExecuteNonQuery(command, "ChatParticipants");
      }
    }
  }
  
}
