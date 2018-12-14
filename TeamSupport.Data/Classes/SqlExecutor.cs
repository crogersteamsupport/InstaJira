using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Security;
using System.Dynamic;
using Newtonsoft.Json;


namespace TeamSupport.Data
{
  public class SqlExecutor
  {
    public static ExpandoObject[] GetExpandoObject(LoginUser loginUser, SqlCommand command)
    {
      return DataUtils.DataTableToExpandoObject(ExecuteQuery(loginUser, command));
    }

    public static string GetJson(LoginUser loginUser, SqlCommand command)
    {
      return JsonConvert.SerializeObject(GetExpandoObject(loginUser, command));
    }

    public static DataTable ExecuteQuery(LoginUser loginUser, SqlCommand command)
    {
      DataTable result = new DataTable();
      BaseCollection.FixCommandParameters(command);
      using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
      {
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
        command.Connection = connection;
        command.Transaction = transaction;
        try
        {
          using (SqlDataAdapter adapter = new SqlDataAdapter(command))
          {
            adapter.Fill(result);
          }
          transaction.Commit();
        }
        catch (Exception e)
        {
          transaction.Rollback();
          e.Data["CommandText"] = command.CommandText;
          ExceptionLogs.LogException(loginUser, e, "SqlExecutor.Fill");
          throw;
        }
        connection.Close();
      }
      return result;
    }

    public static DataTable ExecuteSchema(LoginUser loginUser, SqlCommand command)
    {
        DataTable result = new DataTable();
        using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
        {
            connection.Open();
            command.Connection = connection;
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                adapter.FillSchema(result, SchemaType.Source);
            }
            connection.Close();
        }
        return result;
    }

        public static object ExecuteScalar(LoginUser loginUser, string commandText)
    {
      return ExecuteScalar(loginUser, new SqlCommand(commandText));
    }

    public static object ExecuteScalar(LoginUser loginUser, SqlCommand command)
    {
      BaseCollection.FixCommandParameters(command);
      using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
      {
        object o;
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
        command.Connection = connection;
        command.Transaction = transaction;
        try
        {
          o = command.ExecuteScalar();
          transaction.Commit();
        }
        catch (Exception e)
        {
          transaction.Rollback();
          e.Data["CommandText"] = command.CommandText;
          ExceptionLogs.LogException(loginUser, e, "SqlExecutor.ExecuteScalar");
          throw;
        }

        connection.Close();
        if (o == null || o == DBNull.Value) return null;
        return o;
      }
    }

    public static int ExecuteInt(LoginUser loginUser, string commandText)
    {
      return ExecuteInt(loginUser, new SqlCommand(commandText));
    }

    public static int ExecuteInt(LoginUser loginUser, SqlCommand command)
    {
      int? result = (int?)ExecuteScalar(loginUser, command);
      return result == null ? -1 : (int)result;
    }

    public static int ExecuteNonQuery(LoginUser loginUser, string commandText)
    {
      return ExecuteNonQuery(loginUser, new SqlCommand(commandText));
    }

    public static int ExecuteNonQuery(LoginUser loginUser, SqlCommand command)
    {
      //BaseCollection.FixCommandParameters(command);
      int rows = 0;
      int deadlockCount = 0;
      using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
      {
        command.Connection = connection;

        while (deadlockCount < 4)
        {
          try
          {
            connection.Open();
            rows = command.ExecuteNonQuery();
            connection.Close();
            return rows;
          }
          catch (SqlException ex)
          {
            if (ex.Number == 1205 && deadlockCount < 3)
            {
              deadlockCount++;
              connection.Close();
              System.Threading.Thread.Sleep(10000);
            }
            else
            {
              connection.Close();
              throw ex;
            }
          }
        }
        return rows;
      }
    }
  }

}
