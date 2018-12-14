using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class Tag
    {

        public int GetLinkCount()
        {
            return TagLinks.GetLinkCount(Collection.LoginUser, TagID);
        }
    }

    public partial class Tags
    {

        public void LoadByOrganization(int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Tags WHERE OrganizationID = @OrganizationID ORDER BY Value";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("OrganizationID", organizationID);
                Fill(command);
            }
        }

        public static Tag GetTag(LoginUser loginUser, string value)
        {
            Tags tags = new Tags(loginUser);

            tags.LoadByValue(loginUser.OrganizationID, value.Trim());

            if (tags.IsEmpty)
                return null;
            else
                return tags[0];
        }


        public void LoadByValue(int organizationID, string value)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Tags WHERE OrganizationID = @OrganizationID AND Value = @Value";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("OrganizationID", organizationID);
                command.Parameters.AddWithValue("Value", value);
                Fill(command);
            }

        }

        public void LoadBySearchTerm(string term)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Tags WHERE OrganizationID = @OrganizationID AND Value LIKE @Value+'%' ORDER BY Value";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("OrganizationID", LoginUser.OrganizationID);
                command.Parameters.AddWithValue("Value", term);
                Fill(command);
            }

        }

        public void LoadByReference(ReferenceType refType, int refID, int? parentOrganizationId = null)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Tags t LEFT JOIN TagLinks tl ON tl.TagID = t.TagID WHERE t.OrganizationID = @OrganizationID AND tl.RefType = @RefType AND tl.RefID = @RefID ORDER BY tl.DateCreated";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("OrganizationID", parentOrganizationId != null ? parentOrganizationId : LoginUser.OrganizationID);
                command.Parameters.AddWithValue("RefType", refType);
                command.Parameters.AddWithValue("RefID", refID);
                Fill(command);
            }

        }

        public Tag FindByValue(string value)
        {
            foreach (Tag tag in this)
            {
                if (tag.Value == value)
                {
                    return tag;
                }
            }
            return null;
        }
    }

}
