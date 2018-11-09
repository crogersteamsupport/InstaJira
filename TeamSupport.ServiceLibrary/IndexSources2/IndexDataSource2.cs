using System;
using System.Collections.Generic;
using TeamSupport.Data;
using System.Text;
using System.Data.SqlClient;

namespace TeamSupport.ServiceLibrary
{
    [Serializable]
    class IndexDataSource2 : dtSearch.Engine.DataSource
    {
        protected int _organizationID;
        protected LoginUser _loginUser = null;
        protected Logs _logs;

        protected int[] _itemIDList = null;
        protected List<int> _updatedItems = null;
        protected int _rowIndex = 0;
        protected int? _lastItemID = null;
        protected string _table;
        protected StringBuilder _docFields;

        public List<int> UpdatedItems
        {
            get { lock (this) { return _updatedItems; } }
        }

        protected IndexDataSource2() { }

        public IndexDataSource2(LoginUser loginUser, int organizationID, string table, int[] idList, Logs logs)
        {
            _organizationID = organizationID;
            _table = table;
            _loginUser = new LoginUser(loginUser.ConnectionString, loginUser.UserID, loginUser.OrganizationID, null);
            _logs = logs;
            _docFields = new StringBuilder();
            _itemIDList = idList;
            _updatedItems = new List<int>();

            DocIsFile = false;
            DocName = "";
            DocDisplayName = "";
            DocText = "";
            DocFields = "";
            DocCreatedDate = System.DateTime.UtcNow;
            DocModifiedDate = System.DateTime.UtcNow;
        }

        public override bool GetNextDoc()
        {
            if (_itemIDList == null) { Rewind(); }
            _rowIndex++;
            if (_itemIDList.Length <= _rowIndex) { return false; }
            try
            {
                GetNextRecord();
            }
            catch (Exception ex)
            {
                _logs.WriteEvent($"Table:{_table}, ID:{_itemIDList[_rowIndex]}, OrgID: {_organizationID}");
                _logs.WriteException(ex);
            }
            return true;
        }

        public override bool Rewind()
        {

            try
            {
                //_logs.WriteEvent(string.Format("Rewind {0}, OrgID: {1}", _table, _organizationID.ToString()));
                _lastItemID = null;
                _rowIndex = -1;
                return true;
            }
            catch (Exception ex)
            {
                _logs.WriteException(ex);
                ExceptionLogs.LogException(_loginUser, ex, "Indexer - Rewind - " + _table);
                return false;
            }
        }

        protected void LoadData()
        {
        }

        protected virtual void GetNextRecord()
        {
        }

        protected void AddDocField(string key, string value)
        {
            if (value == null) value = "";
            _docFields.Append(string.Format("{0}\t{1}\t", key.Trim().Replace('\t', ' '), value.Trim().Replace('\t', ' ')));
        }

        protected void AddDocField(string key, int value)
        {
            AddDocField(key, value.ToString());
        }

        protected void AddDocField(string key, bool value)
        {
            AddDocField(key, value.ToString());
        }

        protected void AddDocField(string key, DateTime value)
        {


        }

    }
}

