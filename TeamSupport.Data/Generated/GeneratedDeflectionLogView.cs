using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class DeflectionLogViewItem : BaseItem
  {
    private DeflectionLogView _deflectionLogView;
    
    public DeflectionLogViewItem(DataRow row, DeflectionLogView deflectionLogView): base(row, deflectionLogView)
    {
      _deflectionLogView = deflectionLogView;
    }
	
    #region Properties
    
    public DeflectionLogView Collection
    {
      get { return _deflectionLogView; }
    }
        
    
    public string UserName
    {
      get { return Row["UserName"] != DBNull.Value ? (string)Row["UserName"] : null; }
    }
    
    public string UserCompany
    {
      get { return Row["UserCompany"] != DBNull.Value ? (string)Row["UserCompany"] : null; }
    }
    
    
    

    
    public string UserEMail
    {
      get { return Row["UserEMail"] != DBNull.Value ? (string)Row["UserEMail"] : null; }
      set { Row["UserEMail"] = CheckValue("UserEMail", value); }
    }
    

    
    public bool Helpfull
    {
      get { return (bool)Row["Helpfull"]; }
      set { Row["Helpfull"] = CheckValue("Helpfull", value); }
    }
    
    public string Portal
    {
      get { return (string)Row["Portal"]; }
      set { Row["Portal"] = CheckValue("Portal", value); }
    }
    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateTime
    {
      get { return DateToLocal((DateTime)Row["DateTime"]); }
      set { Row["DateTime"] = CheckValue("DateTime", value); }
    }

    public DateTime DateTimeUtc
    {
      get { return (DateTime)Row["DateTime"]; }
    }
    

    #endregion
    
    
  }

  public partial class DeflectionLogView : BaseCollection, IEnumerable<DeflectionLogViewItem>
  {
    public DeflectionLogView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "DeflectionLogView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return ""; }
    }



    public DeflectionLogViewItem this[int index]
    {
      get { return new DeflectionLogViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(DeflectionLogViewItem deflectionLogViewItem);
    partial void AfterRowInsert(DeflectionLogViewItem deflectionLogViewItem);
    partial void BeforeRowEdit(DeflectionLogViewItem deflectionLogViewItem);
    partial void AfterRowEdit(DeflectionLogViewItem deflectionLogViewItem);
    partial void BeforeRowDelete(int );
    partial void AfterRowDelete(int );    

    partial void BeforeDBDelete(int );
    partial void AfterDBDelete(int );    

    #endregion

    #region Public Methods

    public DeflectionLogViewItemProxy[] GetDeflectionLogViewItemProxies()
    {
      List<DeflectionLogViewItemProxy> list = new List<DeflectionLogViewItemProxy>();

      foreach (DeflectionLogViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int )
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[DeflectionLogView] WH);";
        deleteCommand.Parameters.Add("", SqlDbType.Int);
        deleteCommand.Parameters[""].Value = ;

        BeforeDBDelete();
        BeforeRowDelete();
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete();
        AfterDBDelete();
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("DeflectionLogViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[DeflectionLogView] SET     [TicketID] = @TicketID,    [Portal] = @Portal,    [Helpfull] = @Helpfull,    [DateTime] = @DateTime,    [UserName] = @UserName,    [UserEMail] = @UserEMail,    [UserCompany] = @UserCompany  WH);";

		
		tempParameter = updateCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Portal", SqlDbType.VarChar, 20);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Helpfull", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserName", SqlDbType.NVarChar, 202);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserEMail", SqlDbType.NVarChar, 256);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserCompany", SqlDbType.NVarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[DeflectionLogView] (    [TicketID],    [Portal],    [Helpfull],    [DateTime],    [UserName],    [UserEMail],    [UserCompany]) VALUES ( @TicketID, @Portal, @Helpfull, @DateTime, @UserName, @UserEMail, @UserCompany); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("UserCompany", SqlDbType.NVarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserEMail", SqlDbType.NVarChar, 256);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserName", SqlDbType.NVarChar, 202);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Helpfull", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Portal", SqlDbType.VarChar, 20);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[DeflectionLogView] WH);";
		deleteCommand.Parameters.Add("", SqlDbType.Int);

		try
		{
		  foreach (DeflectionLogViewItem deflectionLogViewItem in this)
		  {
			if (deflectionLogViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(deflectionLogViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = deflectionLogViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns[""].AutoIncrement = false;
			  Table.Columns[""].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				deflectionLogViewItem.Row[""] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(deflectionLogViewItem);
			}
			else if (deflectionLogViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(deflectionLogViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = deflectionLogViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(deflectionLogViewItem);
			}
			else if (deflectionLogViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)deflectionLogViewItem.Row["", DataRowVersion.Original];
			  deleteCommand.Parameters[""].Value = id;
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

      foreach (DeflectionLogViewItem deflectionLogViewItem in this)
      {
        if (deflectionLogViewItem.Row.Table.Columns.Contains("CreatorID") && (int)deflectionLogViewItem.Row["CreatorID"] == 0) deflectionLogViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (deflectionLogViewItem.Row.Table.Columns.Contains("ModifierID")) deflectionLogViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public DeflectionLogViewItem FindBy(int )
    {
      foreach (DeflectionLogViewItem deflectionLogViewItem in this)
      {
        if (deflectionLogViewItem. == )
        {
          return deflectionLogViewItem;
        }
      }
      return null;
    }

    public virtual DeflectionLogViewItem AddNewDeflectionLogViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("DeflectionLogView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new DeflectionLogViewItem(row, this);
    }
    
    public virtual void LoadBy(int )
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TicketID], [Portal], [Helpfull], [DateTime], [UserName], [UserEMail], [UserCompany] FROM [dbo].[DeflectionLogView] WH);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("", );
        Fill(command);
      }
    }
    
    public static DeflectionLogViewItem GetDeflectionLogViewItem(LoginUser loginUser, int )
    {
      DeflectionLogView deflectionLogView = new DeflectionLogView(loginUser);
      deflectionLogView.LoadBy();
      if (deflectionLogView.IsEmpty)
        return null;
      else
        return deflectionLogView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<DeflectionLogViewItem> Members

    public IEnumerator<DeflectionLogViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new DeflectionLogViewItem(row, this);
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
