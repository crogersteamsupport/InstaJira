using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class Plugin : BaseItem
  {
    private Plugins _plugins;
    
    public Plugin(DataRow row, Plugins plugins): base(row, plugins)
    {
      _plugins = plugins;
    }
	
    #region Properties
    
    public Plugins Collection
    {
      get { return _plugins; }
    }
        
    
    
    
    public int PluginID
    {
      get { return (int)Row["PluginID"]; }
    }
    

    

    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public string Code
    {
      get { return (string)Row["Code"]; }
      set { Row["Code"] = CheckValue("Code", value); }
    }
    
    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckValue("Name", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateCreated
    {
      get { return DateToLocal((DateTime)Row["DateCreated"]); }
      set { Row["DateCreated"] = CheckValue("DateCreated", value); }
    }

    public DateTime DateCreatedUtc
    {
      get { return (DateTime)Row["DateCreated"]; }
    }
    

    #endregion
    
    
  }

  public partial class Plugins : BaseCollection, IEnumerable<Plugin>
  {
    public Plugins(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Plugins"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "PluginID"; }
    }



    public Plugin this[int index]
    {
      get { return new Plugin(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(Plugin plugin);
    partial void AfterRowInsert(Plugin plugin);
    partial void BeforeRowEdit(Plugin plugin);
    partial void AfterRowEdit(Plugin plugin);
    partial void BeforeRowDelete(int pluginID);
    partial void AfterRowDelete(int pluginID);    

    partial void BeforeDBDelete(int pluginID);
    partial void AfterDBDelete(int pluginID);    

    #endregion

    #region Public Methods

    public PluginProxy[] GetPluginProxies()
    {
      List<PluginProxy> list = new List<PluginProxy>();

      foreach (Plugin item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int pluginID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Plugins] WHERE ([PluginID] = @PluginID);";
        deleteCommand.Parameters.Add("PluginID", SqlDbType.Int);
        deleteCommand.Parameters["PluginID"].Value = pluginID;

        BeforeDBDelete(pluginID);
        BeforeRowDelete(pluginID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(pluginID);
        AfterDBDelete(pluginID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("PluginsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Plugins] SET     [OrganizationID] = @OrganizationID,    [Name] = @Name,    [Code] = @Code  WHERE ([PluginID] = @PluginID);";

		
		tempParameter = updateCommand.Parameters.Add("PluginID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Code", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Plugins] (    [OrganizationID],    [Name],    [Code],    [DateCreated],    [CreatorID]) VALUES ( @OrganizationID, @Name, @Code, @DateCreated, @CreatorID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Code", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Plugins] WHERE ([PluginID] = @PluginID);";
		deleteCommand.Parameters.Add("PluginID", SqlDbType.Int);

		try
		{
		  foreach (Plugin plugin in this)
		  {
			if (plugin.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(plugin);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = plugin.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["PluginID"].AutoIncrement = false;
			  Table.Columns["PluginID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				plugin.Row["PluginID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(plugin);
			}
			else if (plugin.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(plugin);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = plugin.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(plugin);
			}
			else if (plugin.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)plugin.Row["PluginID", DataRowVersion.Original];
			  deleteCommand.Parameters["PluginID"].Value = id;
			  BeforeRowDelete(id);
			  deleteCommand.ExecuteNonQuery();
			  AfterRowDelete(id);
			}
		  }
		  //transaction.Commit();
		}
		catch (Exception)
		{
		  //transaction.Rollback();
		  throw;
		}
		Table.AcceptChanges();
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public void BulkSave()
    {

      foreach (Plugin plugin in this)
      {
        if (plugin.Row.Table.Columns.Contains("CreatorID") && (int)plugin.Row["CreatorID"] == 0) plugin.Row["CreatorID"] = LoginUser.UserID;
        if (plugin.Row.Table.Columns.Contains("ModifierID")) plugin.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Plugin FindByPluginID(int pluginID)
    {
      foreach (Plugin plugin in this)
      {
        if (plugin.PluginID == pluginID)
        {
          return plugin;
        }
      }
      return null;
    }

    public virtual Plugin AddNewPlugin()
    {
      if (Table.Columns.Count < 1) LoadColumns("Plugins");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Plugin(row, this);
    }
    
    public virtual void LoadByPluginID(int pluginID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [PluginID], [OrganizationID], [Name], [Code], [DateCreated], [CreatorID] FROM [dbo].[Plugins] WHERE ([PluginID] = @PluginID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("PluginID", pluginID);
        Fill(command);
      }
    }
    
    public static Plugin GetPlugin(LoginUser loginUser, int pluginID)
    {
      Plugins plugins = new Plugins(loginUser);
      plugins.LoadByPluginID(pluginID);
      if (plugins.IsEmpty)
        return null;
      else
        return plugins[0];
    }
    
    
    

    #endregion

    #region IEnumerable<Plugin> Members

    public IEnumerator<Plugin> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Plugin(row, this);
      }
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion
  }
}
