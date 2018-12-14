using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class SystemSetting
    {
    }

	public partial class SystemSettings
	{

		public static string ReadString(LoginUser loginUser, string key, string defaultValue)
		{
			return ReadString(key, defaultValue);
		}
		public static string ReadString(string key, string defaultValue)
		{
			string result = defaultValue;
			using (SqlConnection connection = new SqlConnection(LoginUser.GetConnectionString(-1)))
			{
				connection.Open();

				SqlCommand command = new SqlCommand();
				command.Connection = connection;
				command.CommandText = "SELECT SettingValue FROM SystemSettings WHERE (SettingKey=@SettingKey)";
				command.CommandType = CommandType.Text;
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

		/// <summary>
		/// This is only used by the CRM Service, needed to re-created with different name due to some updates to this class and original method that broke it for the CRM Service
		/// </summary>
		public static string ReadStringForCrmService(LoginUser loginUser, string key, string defaultValue)
		{
			string result = defaultValue;
			using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
			{
				connection.Open();

				SqlCommand command = new SqlCommand();
				command.Connection = connection;
				command.CommandText = "SELECT SettingValue FROM SystemSettings WHERE (SettingKey=@SettingKey)";
				command.CommandType = CommandType.Text;
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

		public static void WriteString(LoginUser loginUser, string key, string value)
		{

			using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
			{
				connection.Open();

				using (SqlCommand command = new SqlCommand())
				{
					command.Connection = connection;
					command.CommandText = @"
IF EXISTS(SELECT * FROM SystemSettings WHERE (SettingKey=@SettingKey))
  BEGIN
    UPDATE SystemSettings
  	SET SettingValue = @SettingValue
    WHERE (SettingKey = @SettingKey)
  END
  ELSE
  BEGIN
	  INSERT INTO SystemSettings
	  (
		[SettingKey],
		[SettingValue])
	  VALUES (
		@SettingKey,
		@SettingValue)
  END";
					command.Parameters.AddWithValue("@SettingKey", key);
					command.Parameters.AddWithValue("@SettingValue", value);
					command.ExecuteNonQuery();
				}
				connection.Close();
			}
		}

		public static string GetTokApiKey()
		{
			return ReadString("TokApiKey", "");
		}

		public static string GetTokApiSecret()
		{
			return ReadString("TokApiSecret", "");
		}

		public static string GetAppUrl()
		{
			return ReadString("AppDomain", "https://app.teamsupport.com");
		}
		public static string GetPortalUrl()
		{
			return ReadString("PortalDomain", "https://portal.teamsupport.com");
		}
		public static string GetDomain()
		{
			return ReadString("Domain", "teamsupport.com");
		}

        public static string GetEmailDomain()
        {
            return ReadString("EmailDomain", "teamsupport.com");
        }

        public static string GetPodName()
		{
			return ReadString("PodName", "None");
		}

		public static string GetUserSyncUrl()
		{
			return ReadString("UserSyncUrl", "");
		}

		public static string GetEverageDataset()
		{
			return ReadString("EvergageDataset", "MainApp_Dev");
		}

		public static string GetHubURL()
		{
			return ReadString("HubURL", "na1.teamsupport.com");
		}

        public static string GetMobileURL()
        {
            return ReadString("MobileURL", "https://m.teamsupport.com/");
        }

        public static int GetReportTimeout()
        {
            int value = 60;
            int.TryParse(ReadString("ReportTimeout", "60"), out value);
            return value;
        }

        public static bool GetIsReportsDisabled()
        {
            string value = ReadString("DisableReports", "false").ToLower();
            return value.IndexOf('t') > -1 || value.IndexOf('1') > -1;
        }

        public static bool GetIsApiDisabled()
        {
            string value = ReadString("DisableApi", "false").ToLower();
            return value.IndexOf('t') > -1 || value.IndexOf('1') > -1;
        }

        //Pusher credentials below. If not entered to the SystemSettings table yet then we'll use the original pusher account used in the beginning, named: choice-rock-331-development
        public static string GetPusherKey()
        {
            return ReadString("PusherKey", "0cc6bf2df4f20b16ba4d");
        }

        public static string GetPusherAppId()
        {
            return ReadString("PusherAppId", "223753");
        }

        public static string GetPusherSecret()
        {
            return ReadString("PusherSecret", "119f91ed19272f096383");
        }
	}
}
