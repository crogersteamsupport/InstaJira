using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Web;
using System.Web.SessionState;

namespace TeamSupport.Data
{
  public static class Cache
  {
    public static bool GlobalCacheEnabled = true;
    
    public class CacheItem
    {
      private DateTime _expiresOn;
      public DateTime ExpiresOn
      {
        get { return _expiresOn; }
        set { _expiresOn = value; }
      }

      private ArrayList _tableNames;
      public ArrayList TableNames
      {
        get { return _tableNames; }
        set { _tableNames = value; }
      }

      private int organizationID;
      public int OrganizationID
      {
        get { return organizationID; }
        set { organizationID = value; }
      }


    }

    public class CacheTable : CacheItem
    {
      private DataTable _table;
      public DataTable Table
      {
        get { return _table; }
        set { _table = value; }
      }
    }

    public class CacheScalar : CacheItem
    {
      private Object _value;
      public Object Value
      {
        get { return _value; }
        set { _value = value; }
      }
    }
    
    public static string GetIndexString(SqlCommand command)
    {
      StringBuilder sb = new StringBuilder(command.CommandText);
      foreach (SqlParameter param in command.Parameters)
      {
        sb.Append(param.Value.ToString());
      }
      
      return sb.ToString();
    }

    private static ArrayList GetTableNameList(string tableNames)
    {
      string[] names = tableNames.Split(',');
      ArrayList list = new ArrayList();

      foreach (string s in names)
      {
        list.Add(s.Trim().ToUpper());
      }

      return list;
    }

    public static void AddScalar(SqlCommand command, string tableNames, int expireTime, object value, int organizationID)
    {
      if (!GlobalCacheEnabled) return;
      CacheScalar item = new CacheScalar();
      item.ExpiresOn = DateTime.Now.AddSeconds(expireTime);
      item.TableNames = GetTableNameList(tableNames);
      item.Value = value;
      item.OrganizationID = organizationID;
      HttpContext.Current.Application[GetIndexString(command)] = item;
    }

    public static void AddTable(SqlCommand command, string tableNames, int expireTime, DataTable table, int organizationID)
    {
      if (!GlobalCacheEnabled) return;
      CacheTable item = new CacheTable();
      item.ExpiresOn = DateTime.Now.AddSeconds(expireTime);
      item.TableNames = GetTableNameList(tableNames);
      item.Table = table;
      item.OrganizationID = organizationID;
      HttpContext.Current.Application[GetIndexString(command)] = item;
    }

    public static DataTable GetTable(SqlCommand command)
    {
      CacheTable item = GetCacheItem(GetIndexString(command)) as CacheTable;
      if (item != null)
	    {
        return item.Table;
	    }
      else
      {
        return null;
      }
    }

    public static CacheItem GetCacheItem(string key)
    {
      if (!GlobalCacheEnabled) return null;
      
      object o = HttpContext.Current.Application[key];
      if (o == null) return null;
      if (!(o is CacheItem)) return null;

      CacheItem item = o as CacheItem;
      if (item.ExpiresOn < DateTime.Now) 
      {
        HttpContext.Current.Application.Remove(key);
        return null;
      }
      else
	    {
        return item;
	    }
      
    }

    public static object GetScalar(SqlCommand command)
    {
      
      CacheScalar item = GetCacheItem(GetIndexString(command)) as CacheScalar;
      if (item != null)
      {
        return item.Value;
      }
      else
      {
        return null;
      }
    }

    public static void InvalidateItem(string tableName, int organizationID)
    {
      if (!GlobalCacheEnabled) return;
      tableName = tableName.ToUpper();

      for (int i = HttpContext.Current.Application.Count - 1; i >= 0; i--)
			{
        if (i < 0 || i >= HttpContext.Current.Application.Count) break;

        object o = HttpContext.Current.Application[i];
        if (o is CacheItem)
        {
          CacheItem item = o as CacheItem;
          if ((item.OrganizationID == organizationID && item.TableNames.IndexOf(tableName) > -1) || (item.ExpiresOn < DateTime.Now))
          {
            HttpContext.Current.Application.RemoveAt(i);
          }
        }
			}

    }


  }
}
