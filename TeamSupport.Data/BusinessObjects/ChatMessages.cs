using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ChatMessage
  {
    public string PosterName
    {
      get
      {
        return Row["PosterName"] == null ? "" : Row["PosterName"].ToString();
      }
    }
  }
  
  public partial class ChatMessages
  {

    partial void AfterRowInsert(ChatMessage message)
    {
      Chats.UpdateAction(LoginUser, message.ChatID);
    }

    partial void AfterRowEdit(ChatMessage message)
    {
    }

    public int GetLastMessageID(int chatID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP 1 cm.ChatMessageID FROM ChatMessages cm WHERE cm.ChatID=@ChatID ORDER BY ChatMessageID DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ChatID", chatID);
        object o = ExecuteScalar(command);
        if (o == null || o == DBNull.Value) return -1;
        return (int)o;
      }
    }

    public int GetUnreadMessageCount(int participantID, ChatParticipantType participantType)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
SELECT COUNT(*) FROM ChatMessages cm
LEFT JOIN ChatParticipants cp ON cp.ChatID = cm.ChatID 
WHERE cm.ChatMessageID > cp.LastMessageID
AND cp.ParticipantType = @ParticipantType 
AND cp.ParticipantID = @ParticipantID 
AND cp.DateLeft IS NULL
AND NOT (cm.PosterID = @ParticipantID AND cm.PosterType = @ParticipantType)

";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ParticipantType", (int)participantType);
        command.Parameters.AddWithValue("@ParticipantID", participantID);

        object o = ExecuteScalar(command);
        return o == DBNull.Value ? 0 : (int)o;
      }
    }

    public void LoadUnpreviewedMessages(int userID, int lastPreviewedMessageID)
    {

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
(
SELECT cm.*, u.FirstName + ' ' + u.LastName AS PosterName FROM ChatMessages cm
LEFT JOIN Users u ON cm.PosterID = u.UserID
LEFT JOIN ChatParticipants cp ON cp.ChatID = cm.ChatID 
LEFT JOIN ChatUserSettings cus ON cus.UserID = @UserID
WHERE cm.ChatMessageID > @LastPreviewedMessageID
AND cp.ParticipantType = 0 
AND cp.ParticipantID = @UserID 
AND cp.DateLeft IS NULL
AND NOT (cm.PosterID = @UserID AND cm.PosterType = 0)
AND cm.ChatID <> cus.CurrentChatID
AND cm.PosterType = 0

UNION 

SELECT cm.*, cc.FirstName + ' ' + cc.LastName AS PosterName FROM ChatMessages cm
LEFT JOIN ChatClients cc ON cm.PosterID = cc.ChatClientID
LEFT JOIN ChatParticipants cp ON cp.ChatID = cm.ChatID 
LEFT JOIN ChatUserSettings cus ON cus.UserID = @UserID
WHERE cm.ChatMessageID > @LastPreviewedMessageID
AND cp.ParticipantType = 0 
AND cp.ParticipantID = @UserID 
AND cp.DateLeft IS NULL
AND NOT (cm.PosterID = @UserID AND cm.PosterType = 0)
AND cm.ChatID <> cus.CurrentChatID
AND PosterType = 1
)

ORDER BY DateCreated ASC, PosterName
";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@LastPreviewedMessageID", lastPreviewedMessageID);
        Fill(command);
      }
    }

    public void LoadUnpreviewedMessagesAndMarkPreviewed(int userID)
    { 

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
(
SELECT cm.*, u.FirstName + ' ' + u.LastName AS PosterName FROM ChatMessages cm
LEFT JOIN Users u ON cm.PosterID = u.UserID
LEFT JOIN ChatParticipants cp ON cp.ChatID = cm.ChatID AND cp.ParticipantType = 0 AND cp.ParticipantID = @UserID AND cp.DateLeft IS NULL
LEFT JOIN ChatUserSettings cus ON cus.UserID = @UserID
WHERE cm.ChatMessageID > cp.LastPreviewedMessageID
AND NOT (cm.PosterID = @UserID AND cm.PosterType = 0)
AND cm.ChatID <> cus.CurrentChatID
AND cm.PosterType = 0

UNION 

SELECT cm.*, cc.FirstName + ' ' + cc.LastName AS PosterName FROM ChatMessages cm
LEFT JOIN ChatClients cc ON cm.PosterID = cc.ChatClientID
LEFT JOIN ChatParticipants cp ON cp.ChatID = cm.ChatID AND cp.ParticipantType = 0 AND cp.ParticipantID = @UserID AND cp.DateLeft IS NULL
LEFT JOIN ChatUserSettings cus ON cus.UserID = @UserID
WHERE cm.ChatMessageID > cp.LastPreviewedMessageID
AND NOT (cm.PosterID = @UserID AND cm.PosterType = 0)
AND cm.ChatID <> cus.CurrentChatID
AND PosterType = 1
)

ORDER BY DateCreated, PosterName
";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command);
      }

      SetAllMessagesPreviewed(userID);
    }

    public static int GetLastMessageID(LoginUser loginUser)
    {
      ChatMessages messages = new ChatMessages(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT MAX(ChatMessageID) FROM ChatMessages";  
        command.CommandType = CommandType.Text;
        object o = messages.ExecuteScalar(command, "ChatParticipants");
        if (o == null || o == DBNull.Value) return -1;
        return (int)o;
      }
      
    }

    public void SetAllMessagesPreviewed(int userID)
    { 
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"  
UPDATE ChatParticipants 
SET LastPreviewedMessageID = 
  (SELECT TOP 1 cm.ChatMessageID FROM ChatMessages cm WHERE cm.ChatID = ChatID ORDER BY cm.ChatMessageID DESC) 
WHERE ParticipantType = 0 AND ParticipantID = @UserID
";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        ExecuteNonQuery(command, "ChatParticipants");
      }
    }

    public void LoadByChatID(int chatID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
(SELECT cm.*, u.FirstName + ' ' + u.LastName AS PosterName FROM ChatMessages cm
LEFT JOIN Users u ON cm.PosterID = u.UserID
WHERE cm.ChatID=@ChatID 
AND PosterType = 0

UNION

SELECT cm.*, cc.FirstName + ' ' + cc.LastName AS PosterName FROM ChatMessages cm
LEFT JOIN ChatClients cc ON cm.PosterID = cc.ChatClientID
WHERE cm.ChatID=@ChatID 
AND PosterType = 1)

ORDER BY DateCreated, PosterName
";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ChatID", chatID);
        Fill(command);
      }
    }

        public void LoadAllByChatID(int chatID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT * FROM CHATMESSAGES WHERE ChatID = @ChatID ORDER BY DATECREATED";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ChatID", chatID);
                Fill(command);
            }
        }
    }
  
}
