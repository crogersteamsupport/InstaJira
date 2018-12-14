using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using dtSearch.Engine;

namespace TeamSupport.Data
{
    public partial class TasksViewItem
    {
        public string TaskUrl { get { return SystemSettings.GetAppUrl() + "?TaskID=" + TaskID.ToString(); } }
    }

    public partial class TasksView
    {
        public void LoadByParentID(int ParentID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM TasksView WHERE ParentID = @ParentID ORDER BY DueDate";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ParentID", ParentID);
                Fill(command);
            }
        }

        public static List<SqlDataRecord> GetSearchResultsList(string searchTerm, LoginUser loginUser)
        {
            SqlMetaData recordIDColumn = new SqlMetaData("recordID", SqlDbType.Int);
            SqlMetaData relevanceColumn = new SqlMetaData("relevance", SqlDbType.Int);

            SqlMetaData[] columns = new SqlMetaData[] { recordIDColumn, relevanceColumn };

            List<SqlDataRecord> result = new List<SqlDataRecord>();

            SearchResults results = GetSearchTasksResults(searchTerm, loginUser);

            List<int> items = new List<int>();
            for (int i = 0; i < results.Count; i++)
            {
                results.GetNthDoc(i);

                SqlDataRecord record = new SqlDataRecord(columns);
                record.SetInt32(0, int.Parse(results.CurrentItem.Filename));
                record.SetInt32(1, results.CurrentItem.ScorePercent);

                result.Add(record);
            }

            return result;
        }

        public static SearchResults GetSearchTasksResults(string searchTerm, LoginUser loginUser)
        {
            Options options = new Options();
            options.TextFlags = TextFlags.dtsoTfRecognizeDates;
            using (SearchJob job = new SearchJob())
            {

                searchTerm = searchTerm.Trim();
                job.Request = searchTerm;
                //job.FieldWeights = "ArticleName: 1000";

                //StringBuilder conditions = new StringBuilder();
                //job.BooleanConditions = conditions.ToString();


                //job.MaxFilesToRetrieve = 1000;
                //job.AutoStopLimit = 1000000;
                job.TimeoutSeconds = 30;
                job.SearchFlags =
                    //SearchFlags.dtsSearchSelectMostRecent |
                    SearchFlags.dtsSearchDelayDocInfo;

                int num = 0;
                if (!int.TryParse(searchTerm, out num))
                {
                    //job.Fuzziness = 1;
                    job.SearchFlags = job.SearchFlags |
                        //SearchFlags.dtsSearchFuzzy |
                        //SearchFlags.dtsSearchStemming |
                        SearchFlags.dtsSearchPositionalScoring |
                        SearchFlags.dtsSearchAutoTermWeight;
                }

                if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;
                job.IndexesToSearch.Add(DataUtils.GetTasksIndexPath(loginUser));
                job.Execute();

                return job.Results;
            }

        }

        public static string GetSearchResultsWhereClause(LoginUser loginUser)
        {
            StringBuilder resultBuilder = new StringBuilder();

            SearchCustomFilters searchCustomFilters = new SearchCustomFilters(loginUser);
            searchCustomFilters.LoadByUserID(loginUser.UserID);
            resultBuilder.Append(searchCustomFilters.ConvertToProductVersionEquivalentWhereClause());

            return resultBuilder.ToString();
        }
    }

    public class TaskSearch
    {
        public TaskSearch() { }
        public TaskSearch(TasksViewItem item)
        {
            taskID = item.TaskID;
            organizationID = item.OrganizationID;
            name = item.Name;
            description = item.Description;
            dueDate = item.DueDate;
            user = item.UserName;
            isComplete = item.IsComplete;
            dateCompleted = item.DateCompleted;
            reminderDueDate = item.ReminderDueDate;
            parent = item.TaskParentName;
            creator = item.Creator;
            dateCreated = item.DateCreated;
            dateModified = item.DateModified;

        }

        public int taskID { get; set; }
        public int organizationID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime? dueDate { get; set; }
        public string user { get; set; }
        public bool isComplete { get; set; }
        public DateTime? dateCompleted { get; set; }
        public DateTime? reminderDueDate { get; set; }
        public string parent { get; set; }
        public string creator { get; set; }
        public DateTime? dateCreated { get; set; }
        public DateTime? dateModified { get; set; }
    }
}
