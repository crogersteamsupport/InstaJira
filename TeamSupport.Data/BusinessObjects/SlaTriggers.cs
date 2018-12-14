using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class SlaTrigger
  {
        public void ClonePropertiesTo(SlaTrigger clone)
        {
            foreach (System.Reflection.PropertyInfo sourcePropertyInfo in this.GetType().GetProperties())
            {
                if (sourcePropertyInfo.CanWrite
                    && sourcePropertyInfo.Name.ToLower() != "basecollection"
                    && sourcePropertyInfo.PropertyType != typeof(DateTime)
                    && sourcePropertyInfo.PropertyType != typeof(DateTime?))
                {
                    System.Reflection.PropertyInfo destPropertyInfo = clone.GetType().GetProperty(sourcePropertyInfo.Name);
                    destPropertyInfo.SetValue(
                        clone,
                        sourcePropertyInfo.GetValue(this, null),
                        null);
                }
            }

            //DateTime properties are special it needs to be the UTC value. The DateTime (and DateTime?) properties always have a UTC version
            foreach (System.Reflection.PropertyInfo sourcePropertyInfo in this.GetType().GetProperties())
            {
                if (sourcePropertyInfo.CanRead
                    && sourcePropertyInfo.Name.Substring(sourcePropertyInfo.Name.Length - 3).ToLower() == "utc"
                    && (sourcePropertyInfo.PropertyType == typeof(DateTime)
                        || sourcePropertyInfo.PropertyType == typeof(DateTime?)))
                {
                    System.Reflection.PropertyInfo destPropertyInfo = clone.GetType().GetProperty(sourcePropertyInfo.Name.Substring(0, sourcePropertyInfo.Name.Length - 3));
                    destPropertyInfo.SetValue(
                        clone,
                        sourcePropertyInfo.GetValue(this, null),
                        null);
                }
            }
        }
    }
  
  public partial class SlaTriggers
  {

    public void LoadByTicketType(int organizationID, int slaLevelID, int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT st.*, ts.Position, ts.Name AS Severity  
FROM SlaTriggers st 
LEFT JOIN TicketSeverities ts ON ts.TicketSeverityID = st.TicketSeverityID
WHERE st.TicketType = @TicketType
AND st.OrganizationID = @OrganizationID
AND st.SlaLevelID = @SlaLevelID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@SlaLevelID", slaLevelID);
        command.Parameters.AddWithValue("@TicketType", ticketTypeID);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByTicketTypeAndSeverity(int slaLevelID, int ticketTypeID, int ticketSeverityID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"

SELECT * 
FROM SlaTriggers st 
WHERE st.TicketTypeID = @TicketType
AND st.TicketSeverityID = @TicketSeverityID
AND st.SlaLevelID = @SlaLevelID";

        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@SlaLevelID", slaLevelID);
        command.Parameters.AddWithValue("@TicketType", ticketTypeID);
        command.Parameters.AddWithValue("@TicketSeverityID", ticketSeverityID);
        Fill(command);
      }
    }

        public void LoadBySlaLevel(int organizationID, int slaLevelID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT *
FROM SlaTriggers
WHERE
    SlaLevelID = @SlaLevelID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@SlaLevelID", slaLevelID);
                Fill(command);
            }
        }

        public static void DeleteByTicketTypeID(LoginUser loginUser, int ticketTypeID)
    {
      SlaTriggers triggers = new SlaTriggers(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM SlaTriggers WHERE (TicketTypeID = @ticketTypeID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@ticketTypeID", ticketTypeID);
        triggers.ExecuteNonQuery(command, "SlaTriggers");
      }
    }
    public static void DeleteByTicketSeverityID(LoginUser loginUser, int ticketSeverityID)
    {
      SlaTriggers triggers = new SlaTriggers(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM SlaTriggers WHERE (TicketSeverityID = @ticketSeverityID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@ticketSeverityID", ticketSeverityID);
        triggers.ExecuteNonQuery(command, "SlaTriggers");
      }
    }


        public static List<DateTime> GetSpecificDaysToPause(int triggerId)
        {
            List<DateTime> daysToPause = new List<DateTime>();

            using (SqlConnection connection = new SqlConnection(LoginUser.GetConnectionString(-1)))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = @"SELECT DateToPause FROM SlaPausedDays WHERE SlaTriggerId = @triggerId";
                command.Parameters.AddWithValue("@triggerId", triggerId);
                command.CommandType = CommandType.Text;

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    daysToPause.Add((DateTime)reader["DateToPause"]);
                }

                reader.Close();
                connection.Close();
            }

            return daysToPause;
        }

        public static bool IsOrganizationHoliday(int organizationId, DateTime date)
        {
            bool isHoliday = false;

            using (SqlConnection connection = new SqlConnection(LoginUser.GetConnectionString(-1)))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = @"SELECT IsHoliday FROM CalendarEvents WHERE OrganizationID = @organizationId AND @date BETWEEN StartDateUtc AND EndDateUTC";
                command.Parameters.AddWithValue("@organizationId", organizationId);
                command.Parameters.AddWithValue("@date", date);
                command.CommandType = CommandType.Text;

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    isHoliday = (bool)reader["IsHoliday"];
                }

                reader.Close();
                connection.Close();
            }

            return isHoliday;
        }
    }

 
  
}
