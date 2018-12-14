using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TaskLog
  {
    public string CreatorName
    {
      get
      {
        if (Row.Table.Columns.Contains("CreatorName") && Row["CreatorName"] != DBNull.Value)
        {
          return (string)Row["CreatorName"];
        }
        else return "";
      }
    }
  }
  
  public partial class TaskLogs
  {
        public static void AddTaskLog(LoginUser loginUser, int taskID, string description)
        {
            TaskLogs taskLogs = new TaskLogs(loginUser);
            TaskLog taskLog = taskLogs.AddNewTaskLog();
            taskLog.Description = description;
            taskLog.TaskID = taskID;
            taskLogs.Save();
        }

        public void LoadByTaskID(int taskID, int start)
        {
            int end = start + 49;
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                    SELECT
                        *
                    FROM
                        (
                            SELECT
                                *
                                , ROW_NUMBER() OVER (ORDER BY DateModified Desc) AS rownum 
                            FROM
                                (
                                    SELECT
                                        tl.*
                                        , u.FirstName + ' ' + u.LastName AS CreatorName
                                    FROM
                                        TaskLogs tl
                                        LEFT JOIN Users u 
                                            ON u.UserID = tl.CreatorID
                                    WHERE
                                        tl.TaskID = @TaskID
                                ) as temp
                        ) as results
                    WHERE
                        rownum BETWEEN @start AND @end
                    ORDER BY
                        rownum ASC

                                ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TaskID", taskID);
                command.Parameters.AddWithValue("@start", start);
                command.Parameters.AddWithValue("@end", end);
                Fill(command);
            }
        }
    }

}
