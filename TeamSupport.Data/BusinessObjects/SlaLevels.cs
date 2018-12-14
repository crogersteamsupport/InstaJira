using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class SlaLevel
  {
    public static int? GetIDByName(LoginUser loginUser, string name, int? parentID)
    {
      SlaLevels slaLevels = new SlaLevels(loginUser);
      slaLevels.LoadByName(loginUser.OrganizationID, name);
      if (slaLevels.IsEmpty) return null;
      else return slaLevels[0].SlaLevelID;
    }

        public SlaLevel Clone(string cloneName = null)
        {
            int cloneSlaLevelId = 0;
            LoginUser loginUser = Collection.LoginUser;
            SlaLevels slaLevels = new SlaLevels(loginUser);
            SlaLevel clone = slaLevels.AddNewSlaLevel();
            clone.OrganizationID = OrganizationID;

            if (string.IsNullOrEmpty(cloneName))
            {
                clone.Name = Name + " (Clone)";
            }
            else
            {
                clone.Name = cloneName;
            }

            clone.Collection.Save();
            cloneSlaLevelId = clone.SlaLevelID;

            string actionLog = string.Format("{0} cloned SLA Level {1} into {2}.", loginUser.GetUserFullName(), this.Name, clone.Name);
            ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Sla, cloneSlaLevelId, actionLog);

            //Clone SLA's triggers
            SlaTriggers clonedSlaTriggers = new SlaTriggers(loginUser);
            SlaTriggers originalSlaTriggers = new SlaTriggers(loginUser);

            try
            {
                originalSlaTriggers.LoadBySlaLevel(OrganizationID, SlaLevelID);

                foreach (SlaTrigger slaTrigger in originalSlaTriggers.OrderBy(o => o.SlaTriggerID).ToList())
                {
                    SlaTrigger clonedSlaTrigger = clonedSlaTriggers.AddNewSlaTrigger();
                    slaTrigger.ClonePropertiesTo(clonedSlaTrigger);
                    clonedSlaTrigger.SlaLevelID = cloneSlaLevelId;
                }

                clonedSlaTriggers.BulkSave();
            }
            catch (Exception ex)
            {
                actionLog = string.Format("Failed to clone sla {0} Triggers into {1}.", this.SlaLevelID, clone.SlaLevelID);
                ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Sla, cloneSlaLevelId, actionLog);
                ExceptionLogs.LogException(loginUser, ex, "Cloning Sla Triggers", "SlaLevels.Clone - Triggers");
            }

            //Clone SLA's Pause on specific days
            try
            {
                clonedSlaTriggers = new SlaTriggers(loginUser);
                clonedSlaTriggers.LoadBySlaLevel(OrganizationID, cloneSlaLevelId);

                foreach (SlaTrigger slaTrigger in originalSlaTriggers)
                {
                    SlaPausedDays clonedSlaPausedDays = new SlaPausedDays(loginUser);
                    SlaPausedDays originalSlaPausedDays = new SlaPausedDays(loginUser);
                    originalSlaPausedDays.LoadByTriggerID(slaTrigger.SlaTriggerID);
                    int newTriggerId = clonedSlaTriggers.Where(p => p.TicketTypeID == slaTrigger.TicketTypeID && p.TicketSeverityID == slaTrigger.TicketSeverityID && p.SlaLevelID == cloneSlaLevelId).First().SlaTriggerID;

                    foreach (SlaPausedDay slaPausedDay in originalSlaPausedDays.OrderBy(o => o.DateToPause).ToList())
                    {
                        SlaPausedDay clonedSlaPausedDay = clonedSlaPausedDays.AddNewSlaPausedDay();
                        clonedSlaPausedDay.SlaTriggerId = newTriggerId;
                        clonedSlaPausedDay.DateToPause = slaPausedDay.DateToPauseUtc;
                    }

                    clonedSlaPausedDays.BulkSave();
                }
            }
            catch (Exception ex)
            {
                actionLog = string.Format("Failed to clone sla {0} DaysToPause into {1}.", this.SlaLevelID, clone.SlaLevelID);
                ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Sla, cloneSlaLevelId, actionLog);
                ExceptionLogs.LogException(loginUser, ex, "Cloning Sla Pause On Days", "SlaLevels.Clone - Pause On Days");
            }

            return clone;
        }
    }
  
  public partial class SlaLevels
  {
    partial void BeforeRowDelete(int slaLevelID)
    {
      SlaLevel level = GetSlaLevel(LoginUser, slaLevelID);
      if (level == null) return;
      Organizations organizations = new Organizations(LoginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE Organizations SET SlaLevelID = NULL WHERE SlaLevelID = @SlaLevelID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@SlaLevelID", slaLevelID);
        organizations.ExecuteNonQuery(command, "Organizations");
      }
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE Organizations SET InternalSlaLevelID = NULL WHERE InternalSlaLevelID = @SlaLevelID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@SlaLevelID", slaLevelID);
        organizations.ExecuteNonQuery(command, "Organizations");
      }
    }

    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM SlaLevels WHERE OrganizationID = @OrganizationID ORDER BY Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByName(int organizationID, string name)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM SlaLevels WHERE OrganizationID = @OrganizationID AND Name = @Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@Name", name);
        Fill(command);
      }
    }

        public virtual string GetSlaveLevelName(int slaTriggerID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SET NOCOUNT OFF; select SlaLevels.Name from SlaLevels left join SlaTriggers on SlaTriggers.SlaLevelID=SlaLevels.SlaLevelID WHERE ([SlaTriggerID] = @SlaTriggerID);";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("SlaTriggerID", slaTriggerID);
                object o = ExecuteScalar(command);
                return (string)o;
            }
        }
    }
  
}
