using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class OrganizationSetting 
  {
  }

  public partial class OrganizationSettings 
  {

    public static string ReadString(LoginUser loginUser, string key, string defaultValue) { return ReadString(loginUser, loginUser.OrganizationID, key,defaultValue);}
    public static void WriteString(LoginUser loginUser, string key, string value) { WriteString(loginUser, loginUser.OrganizationID, key, value); }

    public static string ReadString(LoginUser loginUser, int organizationID, string key, string defaultValue)
    {
      string result = defaultValue;
      using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT SettingValue FROM OrganizationSettings WHERE (OrganizationID=@OrganizationID) AND (SettingKey=@SettingKey)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
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
    public static void WriteString(LoginUser loginUser, int organizationID, string key, string value)
    {


      using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
      {
        connection.Open();

        using (SqlCommand command = new SqlCommand())
        {
          command.Connection = connection;
          command.CommandText = "uspWriteOrganizationSetting";
          command.CommandType = CommandType.StoredProcedure;
          command.Parameters.AddWithValue("@OrganizationID", organizationID);
          command.Parameters.AddWithValue("@SettingKey", key);
          command.Parameters.AddWithValue("@SettingValue", value);
          command.ExecuteNonQuery();
        }
        connection.Close();
      }
    }
  }
}
