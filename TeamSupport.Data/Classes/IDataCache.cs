using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  #region Classes

  public class CacheItem
  {
    private int _expireSeconds;
    public int ExpireSeconds
    {
      get { return _expireSeconds; }
      set { _expireSeconds = value; }
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

    private DateTime _createdOn;
    public DateTime CreatedOn
    {
      get { return _createdOn; }
      set { _createdOn = value; }
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
  
  #endregion

  public interface IDataCache
  {
    void AddScalar(SqlCommand command, string tableNames, int expireTime, object value, int organizationID);
    void AddTable(SqlCommand command, string tableNames, int expireTime, DataTable table, int organizationID); 
    DataTable GetTable(SqlCommand command);
    object GetScalar(SqlCommand command);
    void InvalidateItem(string tableName, int organizationID);

  }
}
