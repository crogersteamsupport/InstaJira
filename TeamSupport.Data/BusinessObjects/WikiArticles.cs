using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class WikiArticle
  {
  }
  
  public partial class WikiArticles
  {
    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM WikiArticles WHERE OrganizationID = @OrganizationID ORDER BY ArticleName";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByOrganizationIDAndUserID()
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT * FROM WikiArticles WHERE ([OrganizationID] = @OrganizationID) and ((IsNull(Private,0)=0) or (CreatedBy=@UserID)) and IsNull(IsDeleted,0)=0 ORDER BY ArticleName";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
            command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
            Fill(command);
        }
    }

    public void LoadParentsByOrganizationID()
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT * FROM WikiArticles WHERE ([OrganizationID] = @OrganizationID) and ((IsNull(Private,0)=0) or (CreatedBy=@UserID)) and IsNull(IsDeleted,0)=0 and ParentID is null ORDER BY ArticleName";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
            command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
            Fill(command);
        }
    }

		public void LoadPublicParentsByOrganizationID(int organizationID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = @"SELECT * 
																FROM WikiArticles 
																WHERE [OrganizationID] = @OrganizationID
																	AND IsNull(IsDeleted,0)=0
																AND PortalView = 1
																	AND ParentID IS NULL
																ORDER BY ArticleName";
        command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@OrganizationID", organizationID);
				Fill(command);
			}
		}

		public void LoadPublicArticlesByOrganizationID(int organizationID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = @"SELECT * 
																FROM WikiArticles 
																WHERE [OrganizationID] = @OrganizationID
																	AND IsNull(IsDeleted,0)=0
																	AND PortalView = 1
																ORDER BY ArticleName";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@OrganizationID", organizationID);
				Fill(command);
			}
		}

		public void LoadPublicChildrenByOrganizationID(int organizationID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = @"SELECT * 
																FROM WikiArticles 
																WHERE [OrganizationID] = @OrganizationID
																	AND IsNull(IsDeleted,0)=0
																	AND PortalView = 1
																	AND ParentID IS NOT NULL
																ORDER BY ArticleName";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@OrganizationID", organizationID);
				Fill(command);
			}
		}

		public virtual void LoadPublicArticleByArticleID(int articleID, int organizationID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = @"SELECT * 
																FROM WikiArticles 
																WHERE [OrganizationID] = @OrganizationID
																	AND ArticleID = @ArticleID
																	AND IsNull(IsDeleted,0)=0
																	AND PortalView = 1
																ORDER BY ArticleName";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@ArticleID", articleID);
				command.Parameters.AddWithValue("@OrganizationID", organizationID);
				Fill(command);
			}
		}

		public void LoadPublicSubArticlesByParentID(int parentID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = @"SELECT * 
																FROM WikiArticles 
																WHERE [ParentID] = @ParentID
																	AND IsNull(IsDeleted,0)=0
																	AND PortalView = 1
																ORDER BY ArticleName";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@ParentID", parentID);
				Fill(command);
			}
		}

		public void LoadSubArticlesByParentID(int parentID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            //command.CommandText = "SELECT * FROM WikiArticles WHERE ParentID = @ParentID ORDER BY ArticleName";
            command.CommandText = "SELECT * FROM WikiArticles WHERE ParentID = @ParentID and ([OrganizationID] = @OrganizationID) and ((IsNull(Private,0)=0) or (CreatedBy=@UserID)) and IsNull(IsDeleted,0)=0 ORDER BY ArticleName";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ParentID", parentID);
            command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
            command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
            Fill(command);
        }
    }


    public void LoadRevisionsByArticleID(int articleID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT * FROM WikiHistory WHERE ArticleID = @ArticleID ORDER BY ArticleName";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ArticleID", articleID);
            Fill(command);
        }
    }

    public void LoadBySearchTerm(string term)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT * FROM WikiArticles WHERE ([OrganizationID] = @OrganizationID) and ((IsNull(Private,0)=0) or (CreatedBy=@UserID)) and IsNull(IsDeleted,0)=0 AND ArticleName LIKE '%'+@Value+'%' ORDER BY ArticleName";
            //command.CommandText = "SELECT * FROM Tags WHERE OrganizationID = @OrganizationID AND Value LIKE @Value+'%' ORDER BY Value";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
            command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
            command.Parameters.AddWithValue("@Value", term);
            Fill(command);
        }

    }

    public virtual void LoadByArticleID(int articleID, int userID, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT * FROM [dbo].[WikiArticles] WHERE ([ArticleID] = @ArticleID) and ([OrganizationID] = @OrganizationID) and ((IsNull(Private,0)=0) or (CreatedBy=@UserID)) and IsNull(IsDeleted,0)=0 ORDER BY ArticleName";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ArticleID", articleID);
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public static WikiArticles GetWikiArticles(LoginUser loginUser)
    {
      WikiArticles wikiArticles = new WikiArticles(loginUser);
      wikiArticles.LoadByOrganizationIDAndUserID();
      if (wikiArticles.IsEmpty)
        return null;
      else
        return wikiArticles;
    }

    public static WikiArticles GetWikiParentArticles(LoginUser loginUser)
    {
      WikiArticles wikiArticles = new WikiArticles(loginUser);
      wikiArticles.LoadParentsByOrganizationID();
      if (wikiArticles.IsEmpty)
        return null;
      else
        return wikiArticles;
    }

    public static WikiArticles GetWikiSubArticles(LoginUser loginUser, int articleID)
    {
      WikiArticles wikiArticles = new WikiArticles(loginUser);
      wikiArticles.LoadSubArticlesByParentID(articleID);
      if (wikiArticles.IsEmpty)
        return null;
      else
        return wikiArticles;
    }

    public static WikiArticles GetWikiArticlesBySearchTerm(LoginUser loginUser, string searchTerm)
    {
      WikiArticles wikiArticles = new WikiArticles(loginUser);
      wikiArticles.LoadBySearchTerm(searchTerm);
      if (wikiArticles.IsEmpty)
        return null;
      else
        return wikiArticles;
    }

  }
  
}
