using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace TeamSupport.Data
{
	public partial class UserSetting
	{
	}

	public partial class UserSettings
	{
		public virtual void LoadByUserSettingKey(int userID, string settingKey)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "SET NOCOUNT OFF; SELECT [UserSettingID], [UserID], [SettingKey], [SettingValue], [DateCreated], [DateModified], [CreatorID], [ModifierID] FROM [dbo].[UserSettings] WHERE ([UserID] = @UserID and [SettingKey] = @SettingKey);";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("SettingKey", settingKey);
				command.Parameters.AddWithValue("UserID", userID);
				Fill(command);
			}
		}

		public static string ReadString(LoginUser loginUser, string key, string defaultValue, string category = "General")
		{
			return ReadString(loginUser, loginUser.UserID, key, defaultValue, category);
		}

		public static string ReadString(LoginUser loginUser, int userID, string key, string defaultValue = "", string category = "General")
		{
			string result = defaultValue;
			using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
			{
				connection.Open();

				SqlCommand command = new SqlCommand();
				command.Connection = connection;
                command.CommandType = CommandType.Text;

                if (category.ToLower() == "general")
                {
                    command.CommandText = "SELECT SettingValue FROM UserSettings WHERE (UserID=@UserID) AND (SettingKey=@SettingKey)";
                }
                else
                {
                    command.CommandText = "SELECT SettingValue FROM UserSettings WHERE (UserID=@UserID) AND (SettingKey=@SettingKey) AND (Category=@Category)";
                    command.Parameters.AddWithValue("@Category", category);
                }

				command.Parameters.AddWithValue("@UserID", userID);
				command.Parameters.AddWithValue("@SettingKey", key);

                SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow);
				if (reader.Read())
				{
					result = (string)reader[0];
				}
				reader.Close();
				connection.Close();
			}

			return result;
		}

		public static void DeleteOrganizationSettings(LoginUser loginUser, int organizationID)
		{
			UserSettings settings = new UserSettings(loginUser);

			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "DELETE FROM UserSettings WHERE UserID IN (SELECT UserID FROM Users WHERE OrganizationID = @OrganizationID)";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@OrganizationID", organizationID);
				settings.ExecuteNonQuery(command, "UserSettings");
			}

		}

		public static void WriteString(LoginUser loginUser, string key, string value, string category = "General")
		{
			WriteString(loginUser, loginUser.UserID, key, value, category);
		}

		public static void WriteString(LoginUser loginUser, int userID, string key, string value, string category = "General")
		{


			using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
			{
				connection.Open();

				using (SqlCommand command = new SqlCommand())
				{
					command.Connection = connection;
					command.CommandText = @"
IF EXISTS(SELECT * FROM UserSettings WHERE (UserID=@UserID) AND (SettingKey=@SettingKey))
  BEGIN
    UPDATE [dbo].[UserSettings]
  	SET SettingValue = @SettingValue, Category = @Category
    WHERE (UserID = @UserID)
	  AND (SettingKey = @SettingKey)
  END
  ELSE
  BEGIN
	  INSERT INTO [dbo].[UserSettings]
	  (
		[UserID],
		[SettingKey],
		[SettingValue],
        [Category])
	  VALUES (
		@UserID,
		@SettingKey,
		@SettingValue,
        @Category)
  END";
					command.Parameters.AddWithValue("@UserID", userID);
					command.Parameters.AddWithValue("@SettingKey", key);
					command.Parameters.AddWithValue("@SettingValue", value);
                    command.Parameters.AddWithValue("@Category", category);


                    command.ExecuteNonQuery();
				}
				connection.Close();
			}
		}

        public static string PullSettings(LoginUser loginUser, int userID) {
            try {
                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString)) {
                    using (SqlCommand command = new SqlCommand()) {
                        command.Connection  = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT SettingKey, SettingValue, Category FROM UserSettings WHERE UserID = @UserID AND Category = 'notification' FOR JSON PATH, ROOT('userSettings')";
                        command.Parameters.AddWithValue("@UserID", userID);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows && reader.Read()) {
                            return reader.GetValue(0).ToString();
                        } else {
                            return "nothing";
                        }
                    }
                }
            } catch (SqlException e) {
                return "negative";
            } catch (Exception e) {
                return "negative";
            }
        }

        public static string UpdateSetting(LoginUser loginUser, string key, string value, string category = "general") {
            try {
                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString)) {
                    using (SqlCommand command = new SqlCommand()) {
                        command.Connection   = connection;
                        command.CommandText  = "BEGIN TRAN ";
                        command.CommandText += "IF EXISTS (SELECT * FROM dbo.UserSettings WHERE SettingKey = @key AND UserID = @UserID) ";
                        command.CommandText += "BEGIN UPDATE dbo.UserSettings SET SettingValue = @value, DateModified = @DateTime WHERE UserID = @UserID AND SettingKey = @key END ";
                        command.CommandText += "ELSE BEGIN INSERT dbo.UserSettings (UserID, SettingKey, SettingValue, DateCreated, CreatorID, Category) VALUES (@UserID, @key, @value, @DateTime, @UserID, @Category) END ";
                        command.CommandText += "COMMIT TRAN";
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@UserID", loginUser.UserID);
                        command.Parameters.AddWithValue("@key", key);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@category", category);
                        command.Parameters.AddWithValue("@DateTime", DateTime.UtcNow);
                        connection.Open();
                        Int32 result = command.ExecuteNonQuery();
                        return (result > 0) ? "positive" : "negative";
                    }
                }
            } catch (SqlException e) {
                return "fault";
            } catch (Exception e) {
                return "fault";
            }
        }
	}
}
