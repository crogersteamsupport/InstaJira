using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class ActivityType
    {
        public static int? GetIDByName(LoginUser loginUser, string name, int? parentID)
        {
            ActivityTypes ActivityTypes = new ActivityTypes(loginUser);
            ActivityTypes.LoadByName(loginUser.OrganizationID, name);
            if (ActivityTypes.IsEmpty) return null;
            else return ActivityTypes[0].ActivityTypeID;
        }
    }

    public partial class ActivityTypes
    {

        public void LoadByOrganizationID(int organizationID, string orderBy = "Position")
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM ActivityTypes WHERE OrganizationID = @OrganizationID  ORDER BY " + orderBy;
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        public void LoadByName(int organizationID, string name)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM ActivityTypes WHERE OrganizationID = @OrganizationID  AND Name = @Name";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@Name", name);
                Fill(command);
            }
        }

        partial void BeforeDBDelete(int phoneTypeID)
        {



        }

        partial void AfterDBDelete(int phoneTypeID)
        {
            ValidatePositions(LoginUser.OrganizationID);

        }

        public ActivityType FindByName(string name)
        {
            foreach (ActivityType activityType in this)
            {
                if (activityType.Name == name)
                {
                    return activityType;
                }
            }
            return null;
        }

    }
}
