using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Runtime.Serialization;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class WikiService : System.Web.Services.WebService
    {

        public WikiService() { }

        [WebMethod]
        public WikiArticleProxy GetWiki(int wikiID)
        {
            WikiArticle article = WikiArticles.GetWikiArticle(TSAuthentication.GetLoginUser(), wikiID);
            if (article != null) return article.GetProxy();
            return null;
        }

        [WebMethod]
        public List<WikiArticleListItem2> GetWikis()
        {
            WikiArticles articles = WikiArticles.GetWikiArticles(TSAuthentication.GetLoginUser());
            if (articles != null)
            {
                List<WikiArticleListItem2> articleList = new List<WikiArticleListItem2>();
                foreach (WikiArticle article in articles)
                {
                    articleList.Add(new WikiArticleListItem2
                    {
                        ID = article.ArticleID,
                        Title = article.ArticleName,
                        ParentID = article.ParentID
                    });
                }
                return articleList;
            }
            return null;
        }

        [WebMethod]
        public List<WikiArticleListItem> GetWikiParents()
        {
            WikiArticles articles = WikiArticles.GetWikiParentArticles(TSAuthentication.GetLoginUser());
            List<WikiArticleListItem> parents = new List<WikiArticleListItem>();
            if (articles == null) return null;

            foreach (WikiArticle article in articles)
            {
                parents.Add(new WikiArticleListItem
                {
                    ID = article.ArticleID,
                    Title = article.ArticleName
                });
            }

            return parents;
        }

        [WebMethod]
        public WikiArticleListItem GetWikiAndChildren(int wikiID)
        {
            WikiArticle article = WikiArticles.GetWikiArticle(TSAuthentication.GetLoginUser(), wikiID);
            WikiArticleListItem parent = new WikiArticleListItem
            {
                ID = article.ArticleID,
                Title = article.ArticleName
            };

            WikiArticles subArticles = WikiArticles.GetWikiSubArticles(TSAuthentication.GetLoginUser(), article.ArticleID);
            if (subArticles != null)
            {
                List<WikiArticleListSubItem> children = new List<WikiArticleListSubItem>();
                foreach (WikiArticle subArticle in subArticles)
                {
                    children.Add(new WikiArticleListSubItem
                    {
                        ID = subArticle.ArticleID,
                        Title = subArticle.ArticleName
                    });
                }
                parent.SubArticles = children.ToArray();
            }
            return parent;
        }

        [WebMethod]
        public List<WikiArticleListSubItem> GetWikiParentChildren(int parentID)
        {
            WikiArticles subArticles = WikiArticles.GetWikiSubArticles(TSAuthentication.GetLoginUser(), parentID);
            if (subArticles != null)
            {
                List<WikiArticleListSubItem> children = new List<WikiArticleListSubItem>();
                foreach (WikiArticle subArticle in subArticles)
                {
                    children.Add(new WikiArticleListSubItem
                    {
                        ID = subArticle.ArticleID,
                        Title = subArticle.ArticleName
                    });
                }
                return children;
            }
            else return null;
        }

        [WebMethod]
        public WikiHistoryProxy GetWikiRevision(int historyID)
        {
            WikiHistory article = WikiHistoryCollection.GetWikiHistory(TSAuthentication.GetLoginUser(), historyID);
            if (article != null) return article.GetProxy();
            return null;
        }

        [WebMethod]
        public WikiArticleHisotryItem[] GetWikiHistory(int wikiID)
        {
            LoginUser loggedinUser = TSAuthentication.GetLoginUser();
            List<WikiArticleHisotryItem> wikiHistory = new List<WikiArticleHisotryItem>();
            WikiHistoryCollection history = WikiHistoryCollection.GetWikiHistoryByArticleID(loggedinUser, wikiID);

            if (history != null)
            {
                foreach (WikiHistory wiki in history)
                {
                    WikiArticleHisotryItem item = new WikiArticleHisotryItem();
                    item.HistoryID = wiki.HistoryID;
                    item.RevisionNumber = wiki.Version;
                    if (wiki.ModifiedBy != null)
                    {
                        User user = Users.GetUser(loggedinUser, (int)wiki.ModifiedBy);

                        if (user != null)
                        {
                            item.RevisedBy = user.DisplayName;
                        }
                    }
                    item.RevisedDate = wiki.ModifiedDate;
                    item.Comment = (wiki.Comment != null) ? wiki.Comment : "";

                    wikiHistory.Add(item);
                }

                return wikiHistory.ToArray();
            }
            else return null;
        }

        [WebMethod]
        public WikiArticleListItem[] GetWikiMenuItems()
        {
            WikiArticles articles = WikiArticles.GetWikiParentArticles(TSAuthentication.GetLoginUser());
            List<WikiArticleListItem> wikiList = new List<WikiArticleListItem>();

            if (articles == null) return null;

            foreach (WikiArticle article in articles)
            {
                WikiArticleListItem parent = new WikiArticleListItem
                {
                    ID = article.ArticleID,
                    Title = article.ArticleName
                };

                WikiArticles subArticles = WikiArticles.GetWikiSubArticles(TSAuthentication.GetLoginUser(), article.ArticleID);
                if (subArticles != null)
                {
                    List<WikiArticleListSubItem> children = new List<WikiArticleListSubItem>();
                    foreach (WikiArticle subArticle in subArticles)
                    {
                        children.Add(new WikiArticleListSubItem
                        {
                            ID = subArticle.ArticleID,
                            Title = subArticle.ArticleName
                        });
                    }
                    parent.SubArticles = children.ToArray();
                }

                wikiList.Add(parent);
            }

            return wikiList.ToArray();
        }

        [WebMethod]
        public AutocompleteItem[] SearchWikis(string searchTerm)
        {
            try
            {
                List<AutocompleteItem> items = new List<AutocompleteItem>();
                WikiArticles articles = WikiArticles.GetWikiArticlesBySearchTerm(TSAuthentication.GetLoginUser(), searchTerm);

                foreach (WikiArticle article in articles)
                {
                    items.Add(new AutocompleteItem(article.ArticleName, article.ArticleID.ToString(), article.ArticleID));
                }

                return items.ToArray();
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "WikiService.SearchWikis");
            }
            return null;
        }

        [WebMethod]
        public WikiArticleProxy SaveWiki(int wikiID, int? parent, string wikiBody, string wikiTitle, bool publicView, bool privateView, bool portalView, string comment)
        {
            LoginUser loggedInUser = TSAuthentication.GetLoginUser();
            WikiArticle wiki;
            if (wikiID != 0)
            {
                wiki = WikiArticles.GetWikiArticle(loggedInUser, wikiID);
                LogHistory(loggedInUser, wiki, comment);
                wiki.Version = wiki.Version + 1;
            }
            else
            {
                WikiArticles articles = new WikiArticles(loggedInUser);
                wiki = articles.AddNewWikiArticle();
                wiki.CreatedDate = DateTime.UtcNow;
                wiki.CreatedBy = loggedInUser.UserID;
                wiki.OrganizationID = loggedInUser.OrganizationID;
                wiki.Version = 1;
            }

            wiki.ParentID = parent;
            wiki.Body = wikiBody;
            wiki.ArticleName = (wikiTitle == "") ? "untitled" : wikiTitle;
            wiki.PublicView = publicView;
            wiki.Private = privateView;
            wiki.PortalView = portalView;
            wiki.IsDeleted = false;
            wiki.ModifiedDate = DateTime.UtcNow;
            wiki.ModifiedBy = loggedInUser.UserID;

            wiki.Collection.Save();

            return wiki.GetProxy();
        }

        [WebMethod]
        public void DeleteWiki(int wikiID)
        {
            LoginUser loggedInUser = TSAuthentication.GetLoginUser();
            WikiArticle wiki = WikiArticles.GetWikiArticle(loggedInUser, wikiID);
            wiki.IsDeleted = true;
            wiki.ModifiedDate = DateTime.UtcNow;
            wiki.ModifiedBy = loggedInUser.UserID;
            wiki.Collection.Save();

            ReparentArticles(wikiID);
        }

        [WebMethod]
        public int? GetDefaultWikiID()
        {
            return TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).DefaultWikiArticleID;
        }

        [WebMethod]
        public bool IsWikiOwner(int userID)
        {
            return TSAuthentication.GetLoginUser().UserID == userID;
        }

        #region Private Methods

        public void LogHistory(LoginUser loggedInUser,  WikiArticle wiki, string comment)
        {
            WikiHistoryCollection history = new WikiHistoryCollection(loggedInUser);
            WikiHistory newWikiHistory = history.AddNewWikiHistory();

            newWikiHistory.ModifiedBy = loggedInUser.UserID;
            newWikiHistory.CreatedBy = loggedInUser.UserID;
            newWikiHistory.Version = wiki.Version;
            newWikiHistory.Body = wiki.Body;
            newWikiHistory.ArticleName = wiki.ArticleName;
            newWikiHistory.OrganizationID = wiki.OrganizationID;
            newWikiHistory.ArticleID = wiki.ArticleID;
            newWikiHistory.ModifiedDate = DateTime.UtcNow;
            newWikiHistory.CreatedDate = DateTime.UtcNow;
            newWikiHistory.Comment = comment;

            newWikiHistory.Collection.Save();
        }

        private void ReparentArticles(int parentID)
        {
          WikiArticles articles = WikiArticles.GetWikiSubArticles(TSAuthentication.GetLoginUser(), parentID);
          if (articles != null)
          {
            foreach (WikiArticle article in articles)
            {
              article.ParentID = null;
            }
            articles.Save();
          }
        }

        #endregion

        [DataContract]
        public class WikiArticleListItem
        {
            [DataMember]
            public int ID { get; set; }
            [DataMember]
            public string Title { get; set; }
            [DataMember]
            public WikiArticleListSubItem[] SubArticles { get; set; }
        }

        [DataContract]
        public class WikiArticleListItem2
        {
            [DataMember]
            public int ID { get; set; }
            [DataMember]
            public string Title { get; set; }
            [DataMember]
            public int? ParentID { get; set; }
        }

        [DataContract]
        public class WikiArticleListSubItem
        {
            [DataMember]
            public int ID { get; set; }
            [DataMember]
            public string Title { get; set; }
        }

        [DataContract]
        public class WikiArticleHisotryItem
        {
            [DataMember]
            public int HistoryID { get; set; }
            [DataMember]
            public int? RevisionNumber { get; set; }
            [DataMember]
            public DateTime? RevisedDate { get; set; }
            [DataMember]
            public string RevisedBy { get; set; }
            [DataMember]
            public string Comment { get; set; }
        }
    }
}