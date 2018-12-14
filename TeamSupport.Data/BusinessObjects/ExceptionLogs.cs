using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace TeamSupport.Data
{
  public partial class ExceptionLog
  {
  }
  
  public partial class ExceptionLogs
  {
    public static ExceptionLog AddLog(LoginUser loginUser, string message) { return AddLog(loginUser, "", message, "", "", "", ""); }

    public static ExceptionLog AddLog(LoginUser loginUser, string name, string message, string location, string trace, string extraInfo, string browser)
    {
      try
      {
        ExceptionLog log = (new ExceptionLogs(loginUser)).AddNewExceptionLog();
        log.ExceptionName = name;
        log.URL = location;
        log.Browser = browser;
        log.Message = message.Replace(Environment.NewLine, "<br />");
        log.PageInfo = extraInfo.Replace(Environment.NewLine, "<br />");
        log.StackTrace = trace.Replace(Environment.NewLine, "<br />");
        log.Collection.Save();
        return log;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public static ExceptionLog LogException(LoginUser loginUser, Exception exception, string location, string extraInfo)
    {
      exception.Data["ExtraInfo"] = extraInfo;
      return LogException(loginUser, exception, location);
    }

    public static ExceptionLog LogException(LoginUser loginUser, Exception exception, string location)
    {
      StringBuilder builder = new StringBuilder();
      if (exception.Data != null)
      {
        foreach (DictionaryEntry item in exception.Data)
        {
          builder.AppendLine(string.Format("<p><strong>{0}</strong> = {1}</p>", item.Key, HttpUtility.HtmlEncode(item.Value.ToString())));
        }
      }

      return AddLog(loginUser, exception.ToString(), exception.Message, location, exception.StackTrace, builder.ToString(), ""); 
    }

    public static ExceptionLog LogException(LoginUser loginUser, Exception exception, string location, DataRow row)
    {
      try
      {
        exception.Data["DataRow"] = DataUtils.DataRowToString(row);
      }
      catch (Exception)
      {
      }
      return LogException(loginUser, exception, location);
    }

    public static void DeleteAll()
    {
      ExceptionLogs logs = new ExceptionLogs(LoginUser.Anonymous);
      SqlCommand command = new SqlCommand("DELETE FROM ExceptionLogs");
      command.CommandType = CommandType.Text;
      logs.ExecuteNonQuery(command, "ExceptionLogs");
    }

    public static int GetCount()
    {
      ExceptionLogs logs = new ExceptionLogs(LoginUser.Anonymous);
      SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM ExceptionLogs");
      command.CommandType = CommandType.Text;
      object o = logs.ExecuteScalar(command, "ExceptionLogs");
      if (o == null || o == DBNull.Value) return 0;
      return (int)o;
    }
  }
  
}
