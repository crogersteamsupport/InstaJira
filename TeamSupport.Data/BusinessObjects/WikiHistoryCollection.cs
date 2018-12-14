using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class WikiHistory
  {
  }
  
  public partial class WikiHistoryCollection
  {
    public virtual void LoadByArticleID(int articleID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [HistoryID], [ArticleID], [OrganizationID], [ArticleName], [Body], [Version], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Comment] FROM [dbo].[WikiHistory] WHERE ([ArticleID] = @ArticleID) ORDER BY VERSION DESC;";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ArticleID", articleID);
        Fill(command);
      }
    }

    public static WikiHistoryCollection GetWikiHistoryByArticleID(LoginUser loginUser, int articleID)
    {
      WikiHistoryCollection wikiHistoryCollection = new WikiHistoryCollection(loginUser);
      wikiHistoryCollection.LoadByArticleID(articleID);
      if (wikiHistoryCollection.IsEmpty)
        return null;
      else
        return wikiHistoryCollection;
    }


  }
  
}
