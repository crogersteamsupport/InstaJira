using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TagLink
  {
  }
  
  public partial class TagLinks
  {

    public static TagLink GetTagLink(LoginUser loginUser, ReferenceType refType, int refID, int tagID)
    {
      TagLinks tagLinks = new TagLinks(loginUser);

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TagLinks WHERE RefType = @RefType AND RefID = @RefID AND TagID = @TagID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("RefType", refType);
        command.Parameters.AddWithValue("RefID", refID);
        command.Parameters.AddWithValue("TagID", tagID);
        tagLinks.Fill(command);
      }

      if (tagLinks.IsEmpty)
        return null;
      else
        return tagLinks[0];
    }

    public void ReplaceTags(int oldTagID, int newTagID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"UPDATE tl1 SET tl1.TagID = @NewTagID FROM TagLinks tl1 WHERE tl1.TagID=@OldTagID
 AND NOT EXISTS 
 (SELECT * FROM TagLinks tl2 WHERE tl2.TagID = @NewTagID AND tl2.RefType = tl1.RefType AND tl2.RefID = tl1.RefID)
";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("NewTagID", newTagID);
        command.Parameters.AddWithValue("OldTagID", oldTagID);
        ExecuteNonQuery(command, "TagLinks");
      }

    }

    public static int GetLinkCount(LoginUser loginUser, int tagID)
    {
    
      TagLinks tagLinks = new TagLinks(loginUser);

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT COUNT(*) FROM TagLinks WHERE TagID = @TagID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TagID", tagID);
        Object o = tagLinks.ExecuteScalar(command);
        if (o == null || o == DBNull.Value) return 0;
        return (int)o;
      }

    
    }

		public void LoadByReference(ReferenceType refType, int refID, int? parentOrganizationId = null)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "SELECT TagLinks.* FROM TagLinks LEFT JOIN Tags ON TagLinks.TagID = Tags.TagID WHERE Tags.OrganizationID = @OrganizationID AND TagLinks.RefType = @RefType AND TagLinks.RefID = @RefID ORDER BY TagLinks.DateCreated";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("OrganizationID", parentOrganizationId != null ? parentOrganizationId : LoginUser.OrganizationID);
				command.Parameters.AddWithValue("RefType", refType);
				command.Parameters.AddWithValue("RefID", refID);
				Fill(command);
			}

		}
	}
  
}
