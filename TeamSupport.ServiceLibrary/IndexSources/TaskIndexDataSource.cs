using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using TeamSupport.Data;
using Newtonsoft.Json;

namespace TeamSupport.ServiceLibrary
{
    [Serializable]
    class TaskIndexDataSource : IndexDataSource
    {
        protected TaskIndexDataSource() { }
        public TaskIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, string table, bool isRebuilding, Logs logs)
          : base(loginUser, maxCount, organizationID, table, isRebuilding, logs)
        {
        }

        override protected void GetNextRecord()
        {
            TasksViewItem task = TasksView.GetTasksViewItem(_loginUser, _itemIDList[_rowIndex]);
            _lastItemID = task.TaskID;
            UpdatedItems.Add((int)_lastItemID);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(task.Description
            + " " + task.Name
            + " " + task.DateCreated
            + " " + task.DateModified);

            DocText = builder.ToString();

            _docFields.Clear();
            AddDocField("TaskID", task.TaskID);
            AddDocField("Name", task.Name);
            DocDisplayName = task.Name;

            TaskSearch taskItem = new TaskSearch(task);
            AddDocField("**JSON", JsonConvert.SerializeObject(taskItem));

            // How do we handle associations indexing?
            //TaskAssociationsView associations = new TaskAssociationsView(_loginUser);
            //associations.LoadByTaskIDOnly(task.TaskID);

            //foreach (TaskAssociationsViewItem association in associations)
            //{
            //  object o = value.Row["CustomValue"];
            //  string s = o == null || o == DBNull.Value ? "" : o.ToString();
            //  AddDocField(value.Row["Name"].ToString(), s);
            //}
            DocFields = _docFields.ToString();
            DocIsFile = false;
            DocName = task.TaskID.ToString();
            try
            {
                DocCreatedDate = (DateTime)task.Row["DateCreated"];
                DocModifiedDate = (DateTime)task.Row["DateModified"];
            }
            catch (Exception)
            {

            }
        }

        override protected void LoadData()
        {
            Tasks tasks = new Tasks(_loginUser);
            tasks.LoadForIndexing(_organizationID, _maxCount, _isRebuilding);
            foreach (Task task in tasks)
            {
                _itemIDList.Add(task.TaskID);
            }
        }
    }
}
