using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace TeamSupport.WebUtils
{
  [Serializable]
  public class WebDataCache : TeamSupport.Data.IDataCache 
  {

    #region Properties
    private static bool _globalCacheEnabled = true;
    public static bool GlobalCacheEnabled
    {
      get { return _globalCacheEnabled; }
      set { _globalCacheEnabled = value; }
    }

    #endregion

    #region Private Methods

    private static string GetIndexString(SqlCommand command)
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

    private static CacheItem GetCacheItem(string key)
    {
      try
      {
        if (!GlobalCacheEnabled) return null;

        object o = HttpContext.Current.Application[key];
        if (o == null) return null;
        if (!(o is CacheItem)) return null;

        CacheItem item = o as CacheItem;
        if (IsItemExipred(item))
        {
          HttpContext.Current.Application.Remove(key);
          return null;
        }
        else
        {
          return item;
        }
      }
      catch (Exception)
      {
        return null;
      }
    }
    
    private static bool IsItemExipred(CacheItem item)
    {
      return (DateTime.UtcNow.AddSeconds(item.ExpireSeconds) < DateTime.UtcNow);
    }

    #endregion

    #region Public

    public void AddItem(CacheItem item, SqlCommand command, string tableNames, int expireTime, int organizationID)
    {
      item.ExpireSeconds = expireTime;
      item.CreatedOn = DateTime.UtcNow;
      item.TableNames = GetTableNameList(tableNames);
      item.OrganizationID = organizationID;
      HttpContext.Current.Application[GetIndexString(command)] = item;
    
    }

    public void AddScalar(SqlCommand command, string tableNames, int expireTime, object value, int organizationID)
    {
      if (!GlobalCacheEnabled) return;
      CacheScalar item = new CacheScalar();
      item.Value = value;
      AddItem(item, command, tableNames, expireTime, organizationID);
      
    }

    public void AddTable(SqlCommand command, string tableNames, int expireTime, DataTable table, int organizationID)
    {
      if (!GlobalCacheEnabled) return;
      CacheTable item = new CacheTable();
      item.Table = table;
      AddItem(item, command, tableNames, expireTime, organizationID);
    }

    public DataTable GetTable(SqlCommand command)
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

    public object GetScalar(SqlCommand command)
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

    public void InvalidateItem(string tableName, int organizationID)
    {
      try
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
            if ((item.OrganizationID == organizationID && item.TableNames.IndexOf(tableName) > -1) || (IsItemExipred(item)))
            {
              HttpContext.Current.Application.RemoveAt(i);
            }
          }
        }
      }
      catch (Exception)
      {
        
      }
    }
    #endregion

    
  }
}
