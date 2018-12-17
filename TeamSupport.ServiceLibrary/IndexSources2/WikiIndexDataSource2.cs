using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
    [Serializable]
    class WikiIndexDataSource2 : IndexDataSource2
    {
        protected WikiIndexDataSource2() { }

        public WikiIndexDataSource2(LoginUser loginUser, int organizationID, string table, int[] idList, Logs logs)
            : base(loginUser, organizationID, table, idList, logs)
        {
        }
        override protected void GetNextRecord()
        {
            WikiArticlesViewItem wiki = WikiArticlesView.GetWikiArticlesViewItem(_loginUser, _itemIDList[_rowIndex]);
            _lastItemID = wiki.ArticleID;
            UpdatedItems.Add((int)_lastItemID);

            DocText = HtmlToText.ConvertHtml(wiki.Body == null ? string.Empty : wiki.Body);

            _docFields.Clear();
            foreach (DataColumn column in wiki.Collection.Table.Columns)
            {
                object value = wiki.Row[column];
                string s = value == null || value == DBNull.Value ? "" : value.ToString();
                AddDocField(column.ColumnName, s);
            }
            DocFields = _docFields.ToString();

            DocId = wiki.ArticleID;
            DocIsFile = false;
            DocName = wiki.ArticleID.ToString();
            DocDisplayName = wiki.ArticleName;
            DocCreatedDate = wiki.CreatedDate;
            DocModifiedDate = DateTime.UtcNow;
        }


    }
}
