using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class ChatRequest
    {
    }

    public partial class ChatRequests
    {

        public static readonly int StayAliveSeconds = 30;

        public static int GetWaitingRequestCount(LoginUser loginUser, int userID, int organizationID)
        {
            ChatRequests requests = new ChatRequests(loginUser);

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
SELECT COUNT(*) FROM ChatRequests cr
LEFT JOIN ChatClients cc ON cc.ChatClientID = cr.RequestorID
WHERE cr.IsAccepted = 0 
AND cr.OrganizationID = @OrganizationID
AND ((DATEADD(second, @Seconds, cc.LastPing) > GETUTCDATE() OR cc.LastPing IS NULL OR cr.RequestorType = 0) AND DATEADD(minute, 30, cr.DateCreated) > GETUTCDATE())
AND (cr.GroupID IS NULL OR cr.GroupID IN (SELECT GroupID FROM GroupUsers WHERE UserID = @UserID))
AND (cr.TargetUserID IS NULL OR cr.TargetUserID = @UserID)
";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@Seconds", StayAliveSeconds);

                object o = requests.ExecuteScalar(command);
                return o == DBNull.Value ? 0 : (int)o;
            }
        }

        public static bool IsOperatorAvailable(LoginUser loginUser, int organizationID)
        {
			return AreOperatorsAvailable(loginUser, organizationID);
        }

        public static bool AreOperatorsAvailable(LoginUser loginUser, int organizationID, int? groupID = null)
        {
            ChatRequests requests = new ChatRequests(loginUser);
			string sql = @"SELECT COUNT(*) FROM ChatUserSettings cus WITH(NOLOCK)
                            LEFT JOIN Users u WITH(NOLOCK) ON u.UserID = cus.UserID
                            {0}
                            WHERE cus.IsAvailable = 1
                            AND u.IsChatUser = 1
                            AND u.OrganizationID = @OrganizationID
                            {1}";

			string groupJoinClause = "";
			string groupWhereClause = "";

            if (groupID != null && groupID != 0)
            {
                groupJoinClause = @"JOIN GroupUsers ON u.UserID = GroupUsers.UserID";
                groupWhereClause = @"AND GroupUsers.GroupID = @GroupID";
            }

			sql = string.Format(sql, groupJoinClause, groupWhereClause);

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);

                if (groupID != null) { 
                    command.Parameters.AddWithValue("@GroupID", groupID);
                }

                object o = requests.ExecuteScalar(command);
                int count = o == DBNull.Value ? 0 : (int)o;
                return count > 0;
            }
        }

        public void LoadByChatID(int chatID, ChatRequestType type)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"  
SELECT cr.* FROM ChatRequests cr 
WHERE cr.ChatID = @ChatID
AND cr.RequestType = @RequestType
";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@RequestType", (int)type);
                command.Parameters.AddWithValue("@ChatID", chatID);
                Fill(command);
            }
        }


        /// <summary>
        /// Load chat requests for a user
        /// </summary>
        /// <param name="userID">The user</param>
        /// <param name="organizationID">The user's organization</param>
        public void LoadWaitingRequests(int userID, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"  
SELECT cr.*, cc.lastping, cc.chatclientid
FROM ChatRequests cr 
LEFT JOIN ChatClients cc ON cc.ChatClientID = cr.RequestorID
WHERE cr.IsAccepted = 0 
AND cr.OrganizationID = @OrganizationID 
AND ((DATEADD(second, @Seconds, cc.LastPing) > GETUTCDATE() OR cc.LastPing IS NULL  OR cr.RequestorType = 0) AND DATEADD(minute, 30, cr.DateCreated) > GETUTCDATE())
AND (cr.GroupID IS NULL OR cr.GroupID IN (SELECT GroupID FROM GroupUsers WHERE UserID = @UserID))
AND (cr.TargetUserID IS NULL OR cr.TargetUserID = @UserID)
ORDER BY cr.DateCreated ASC
";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@Seconds", StayAliveSeconds);
                Fill(command);
            }
        }

        /// <summary>
        /// Load chat requests for a user
        /// </summary>
        /// <param name="userID">The user</param>
        /// <param name="organizationID">The user's organization</param>
        public void LoadPendingRequests(int userID, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"  
                                        SELECT *
                                        FROM ChatRequests cr
                                            LEFT JOIN ChatClients cc
                                            ON cc.ChatClientID = cr.RequestorID
                                        WHERE cr.IsAccepted = 0
	                                        AND cr.OrganizationID = @OrganizationID
                                            AND ((DATEADD(second, @Seconds, cc.LastPing) > GETUTCDATE() OR cc.LastPing IS NULL  OR cr.RequestorType = 0) AND DATEADD(minute, 30, cr.DateCreated) > GETUTCDATE())
	                                        AND (
		                                        cr.GroupID IS NULL
												OR cr.GroupID = 0
		                                        OR cr.GroupID IN (
			                                        SELECT GroupID
			                                        FROM GroupUsers
			                                        WHERE UserID = @UserID
			                                        )
		                                        )
	                                        AND (
		                                        cr.TargetUserID IS NULL
		                                        OR cr.TargetUserID = @UserID
		                                        )
                                        ORDER BY cr.DateCreated DESC
                                        ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@Seconds", StayAliveSeconds);
                Fill(command);
            }
        }

        /// <summary>
        /// Load chat requests for a user
        /// </summary>
        /// <param name="userID">The user</param>
        /// <param name="organizationID">The user's organization</param>
        public int GetLastRequestID(int userID, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"  
SELECT MAX(cr.ChatRequestID)
FROM ChatRequests cr 
LEFT JOIN ChatClients cc ON cc.ChatClientID = cr.RequestorID
WHERE cr.IsAccepted = 0 
AND cr.OrganizationID = @OrganizationID 
AND ((DATEADD(second, @Seconds, cc.LastPing) > GETUTCDATE() OR cc.LastPing IS NULL  OR cr.RequestorType = 0) AND DATEADD(minute, 30, cr.DateCreated) > GETUTCDATE())
AND (cr.GroupID IS NULL OR cr.GroupID IN (SELECT GroupID FROM GroupUsers WHERE UserID = @UserID))
AND (cr.TargetUserID IS NULL OR cr.TargetUserID = @UserID)
";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@Seconds", StayAliveSeconds);
                object o = ExecuteScalar(command);
                if (o == null || o == DBNull.Value) return -1;
                return (int)o;
            }
        }

        public void LoadNewWaitingRequestsOld(int userID, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"  
SELECT cr.*, 
CASE WHEN cr.RequestorType = 0 THEN u.FirstName ELSE cc.FirstName END AS FirstName, 
CASE WHEN cr.RequestorType = 0 THEN u.LastName ELSE cc.LastName END AS LastName
FROM ChatRequests cr 
LEFT JOIN ChatClients cc ON cc.ChatClientID = cr.RequestorID
LEFT JOIN Users u ON u.UserID = cr.RequestorID
LEFT JOIN ChatUserSettings cus ON cus.UserID = @UserID
WHERE cr.IsAccepted = 0 
AND cr.OrganizationID = @OrganizationID 
AND ((DATEADD(second, @Seconds, cc.LastPing) > GETUTCDATE() OR cc.LastPing IS NULL OR cr.RequestorType = 0) AND DATEADD(minute, 30, cr.DateCreated) > GETUTCDATE())
AND (cr.GroupID IS NULL OR cr.GroupID IN (SELECT GroupID FROM GroupUsers WHERE UserID = @UserID))
AND (cr.TargetUserID IS NULL OR cr.TargetUserID = @UserID)
AND cr.ChatRequestID > cus.LastChatRequestID
ORDER BY cr.DateCreated ASC";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@Seconds", StayAliveSeconds);


                Fill(command);
            }
            SetLastRequestID(userID, organizationID);
        }


        public void LoadNewWaitingRequests(int userID, int organizationID, int lastRequestID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"  
SELECT cr.*, 
CASE WHEN cr.RequestorType = 0 THEN u.FirstName ELSE cc.FirstName END AS FirstName, 
CASE WHEN cr.RequestorType = 0 THEN u.LastName ELSE cc.LastName END AS LastName
FROM ChatRequests cr 
LEFT JOIN ChatClients cc ON cc.ChatClientID = cr.RequestorID
LEFT JOIN Users u ON u.UserID = cr.RequestorID
LEFT JOIN ChatUserSettings cus ON cus.UserID = @UserID
WHERE cr.IsAccepted = 0 
AND cr.OrganizationID = @OrganizationID 
AND ((DATEADD(second, @Seconds, cc.LastPing) > GETUTCDATE() OR cc.LastPing IS NULL OR cr.RequestorType = 0) AND DATEADD(minute, 30, cr.DateCreated) > GETUTCDATE())
AND (cr.GroupID IS NULL OR cr.GroupID IN (SELECT GroupID FROM GroupUsers WHERE UserID = @UserID))
AND (cr.TargetUserID IS NULL OR cr.TargetUserID = @UserID)
AND cr.ChatRequestID > @LastChatRequestID
ORDER BY cr.DateCreated ASC";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@Seconds", StayAliveSeconds);
                command.Parameters.AddWithValue("@LastChatRequestID", lastRequestID);
                Fill(command);
            }
            // SetLastRequestID(userID, organizationID);
        }

        public void LoadActiveChatsByUserId(int userID, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                SELECT DISTINCT c.* FROM ChatRequests c 
                LEFT JOIN ChatParticipants cp ON cp.ChatID = c.ChatID
                WHERE cp.ParticipantID = @UserID
                AND cp.ParticipantType = 0
                AND c.OrganizationID = @OrganizationID
                AND cp.DateLeft IS NULL
                AND c.RequestType = 0
                ORDER BY C.DATECREATED DESC
                ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        public static int GetRequestCountInLastDays(LoginUser loginUser, int organizationID, int days)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT COUNT(*) FROM ChatRequests cr WHERE cr.OrganizationID = @OrganizationID AND DATEDIFF(day, DateCreated, GETUTCDATE()) < @Days";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            command.Parameters.AddWithValue("@Days", days);
            return SqlExecutor.ExecuteInt(loginUser, command);
        }

        public void SetLastRequestID(int userID, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"  
UPDATE ChatUserSettings SET LastChatRequestID = 
ISNULL((
SELECT MAX(ChatRequestID) 
FROM ChatRequests cr 
WHERE (cr.GroupID IS NULL OR cr.GroupID IN (SELECT GroupID FROM GroupUsers WHERE UserID = @UserID))
AND cr.OrganizationID = @OrganizationID 
AND (cr.TargetUserID IS NULL OR cr.TargetUserID = @UserID)
), -1) WHERE UserID = @UserID
";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                ExecuteNonQuery(command, "ChatUserSettings");
            }
        }

        public static int AcceptRequest(LoginUser loginUser, int userID, int chatRequestID, string ipAddress)
        {
            ChatRequest request = ChatRequests.GetChatRequest(loginUser, chatRequestID);
            if (request == null || request.IsAccepted) return -1;
            request.IsAccepted = true;
            request.TargetUserID = userID;
            request.Collection.Save();

            Chats.JoinChat(loginUser, userID, ChatParticipantType.User, request.ChatID, ipAddress);

            if (request.RequestType == ChatRequestType.External && request.Message.Trim() != "")
            {
                ChatMessage chatMessage = (new ChatMessages(loginUser)).AddNewChatMessage();
                chatMessage.ChatID = request.ChatID;
                chatMessage.Message = request.Message.Trim();
                chatMessage.PosterID = request.RequestorID;
                chatMessage.PosterType = request.RequestorType;
                chatMessage.Collection.Save();
            }

            return request.ChatID;

        }

        public static int? CalculateChatGroupID(LoginUser loginUser, int organizationID, int? groupID, string groupName = null) {
            int? result = null;
            Groups groups = new Groups(loginUser);

            if (groupID != null)
            {
                groups.LoadByGroupID((int)groupID);
                result = groups.Any() ? groupID : 0;
            }
            else if (!string.IsNullOrEmpty(groupName)) {
                groups.LoadByGroupName(organizationID, groupName);
                result = groups.Any() ? (int?)groups[0].GroupID : null;
            }

            return result;
        }

        public static ChatRequest RequestChat(LoginUser loginUser, int organizationID, string firstName, string lastName, string email, string message, string ipAddress, int groupID = 0)
        {
            ChatClients clients = new ChatClients(loginUser);
            ChatClient client = clients.IsEmpty ? (new ChatClients(loginUser)).AddNewChatClient() : clients[0];
            client.OrganizationID = organizationID;
            client.FirstName = firstName;
            client.LastName = lastName;
            client.Email = email;
            Users users = new Users(loginUser);

            if (!string.IsNullOrEmpty(email))
            {
                users.LoadByEmail(organizationID, email);

                try
                {
                    if (!users.IsEmpty)
                    {
                        client.LinkedUserID = users[0].UserID;
                        client.CompanyName = Organizations.GetOrganization(loginUser, users[0].OrganizationID).Name;
                    }
                    else
                    {
                        string emailDomain = email.Substring(email.LastIndexOf('@') + 1).Trim();
                        Organization org = Organization.GetCompanyByDomain(organizationID, emailDomain, loginUser);
                        client.CompanyName = (org != null) ? org.Name : "";
                    }
                }
                catch (Exception)
                {
                    client.CompanyName = "";
                }
            }
            else
            {
                Organization org = Organization.GetCompanyByDomain(organizationID, string.Empty, loginUser, forceUnknown: true);
                client.CompanyName = (org != null) ? org.Name : "";
            }
            
            client.Collection.Save();


            Chat chat = (new Chats(loginUser)).AddNewChat();
            chat.InitiatorID = client.ChatClientID;
            chat.InitiatorType = ChatParticipantType.External;
            chat.OrganizationID = organizationID;
            chat.Collection.Save();

            Chats.JoinChat(loginUser, client.ChatClientID, ChatParticipantType.External, chat.ChatID, ipAddress);

            ChatRequest request = (new ChatRequests(loginUser)).AddNewChatRequest();
            request.RequestorID = client.ChatClientID;
            request.RequestorType = ChatParticipantType.External;
            request.OrganizationID = organizationID;
            request.ChatID = chat.ChatID;
            request.Message = message;
            request.IsAccepted = false;
            request.RequestType = ChatRequestType.External;
            request.GroupID = groupID;
            request.Collection.Save();

            return request;
        }

        public static void RequestTransfer(LoginUser loginUser, int chatID, int userID)
        {
            User user = Users.GetUser(loginUser, userID);
            if (user == null || user.OrganizationID != loginUser.OrganizationID) return;
            ChatRequest request = (new ChatRequests(loginUser)).AddNewChatRequest();
            request.RequestorID = loginUser.UserID;
            request.RequestorType = ChatParticipantType.User;
            request.TargetUserID = userID;
            request.OrganizationID = loginUser.OrganizationID;
            request.ChatID = chatID;
            request.Message = "Will you accept this transfer?";
            request.IsAccepted = false;
            request.RequestType = ChatRequestType.Transfer;
            request.GroupID = null;
            request.Collection.Save();
        }

        public static ChatRequest RequestInvite(LoginUser loginUser, int chatID, int userID)
        {
            User user = Users.GetUser(loginUser, userID);
            if (user == null || user.OrganizationID != loginUser.OrganizationID) return null;
            ChatRequest request = (new ChatRequests(loginUser)).AddNewChatRequest();
            request.RequestorID = loginUser.UserID;
            request.RequestorType = ChatParticipantType.User;
            request.TargetUserID = userID;
            request.OrganizationID = loginUser.OrganizationID;
            request.ChatID = chatID;
            request.Message = "Will you accept this invitation?";
            request.IsAccepted = false;
            request.RequestType = ChatRequestType.Invitation;
            request.GroupID = null;
            request.Collection.Save();

            return request;
        }

    }

}
