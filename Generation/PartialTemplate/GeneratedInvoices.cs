using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class Invoice : BaseItem
  {
    private Invoices _invoices;
    
    public Invoice(DataRow row, Invoices invoices): base(row, invoices)
    {
      _invoices = invoices;
    }
	
    #region Properties
    
    public Invoices Collection
    {
      get { return _invoices; }
    }
        
    
    
    
    public int InvoiceID
    {
      get { return (int)Row["InvoiceID"]; }
    }
    

    
    public int? CreditCardID
    {
      get { return Row["CreditCardID"] != DBNull.Value ? (int?)Row["CreditCardID"] : null; }
      set { Row["CreditCardID"] = CheckValue("CreditCardID", value); }
    }
    
    public string PaymentMethod
    {
      get { return Row["PaymentMethod"] != DBNull.Value ? (string)Row["PaymentMethod"] : null; }
      set { Row["PaymentMethod"] = CheckValue("PaymentMethod", value); }
    }
    
    public string PaymentFailedReason
    {
      get { return Row["PaymentFailedReason"] != DBNull.Value ? (string)Row["PaymentFailedReason"] : null; }
      set { Row["PaymentFailedReason"] = CheckValue("PaymentFailedReason", value); }
    }
    

    
    public int ModifierID
    {
      get { return (int)Row["ModifierID"]; }
      set { Row["ModifierID"] = CheckValue("ModifierID", value); }
    }
    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public bool IsPaymentFailed
    {
      get { return (bool)Row["IsPaymentFailed"]; }
      set { Row["IsPaymentFailed"] = CheckValue("IsPaymentFailed", value); }
    }
    
    public bool IsPaid
    {
      get { return (bool)Row["IsPaid"]; }
      set { Row["IsPaid"] = CheckValue("IsPaid", value); }
    }
    
    public decimal TotalAmount
    {
      get { return (decimal)Row["TotalAmount"]; }
      set { Row["TotalAmount"] = CheckValue("TotalAmount", value); }
    }
    
    public decimal TotalTaxPrice
    {
      get { return (decimal)Row["TotalTaxPrice"]; }
      set { Row["TotalTaxPrice"] = CheckValue("TotalTaxPrice", value); }
    }
    
    public decimal TotalStoragePrice
    {
      get { return (decimal)Row["TotalStoragePrice"]; }
      set { Row["TotalStoragePrice"] = CheckValue("TotalStoragePrice", value); }
    }
    
    public decimal TotalPortalPrice
    {
      get { return (decimal)Row["TotalPortalPrice"]; }
      set { Row["TotalPortalPrice"] = CheckValue("TotalPortalPrice", value); }
    }
    
    public decimal TotalUserPrice
    {
      get { return (decimal)Row["TotalUserPrice"]; }
      set { Row["TotalUserPrice"] = CheckValue("TotalUserPrice", value); }
    }
    
    public decimal TaxRate
    {
      get { return (decimal)Row["TaxRate"]; }
      set { Row["TaxRate"] = CheckValue("TaxRate", value); }
    }
    
    public bool IsPortalBilled
    {
      get { return (bool)Row["IsPortalBilled"]; }
      set { Row["IsPortalBilled"] = CheckValue("IsPortalBilled", value); }
    }
    
    public decimal StoragePrice
    {
      get { return (decimal)Row["StoragePrice"]; }
      set { Row["StoragePrice"] = CheckValue("StoragePrice", value); }
    }
    
    public decimal PortalPrice
    {
      get { return (decimal)Row["PortalPrice"]; }
      set { Row["PortalPrice"] = CheckValue("PortalPrice", value); }
    }
    
    public decimal UserPrice
    {
      get { return (decimal)Row["UserPrice"]; }
      set { Row["UserPrice"] = CheckValue("UserPrice", value); }
    }
    
    public decimal ExtraStorageUnits
    {
      get { return (decimal)Row["ExtraStorageUnits"]; }
      set { Row["ExtraStorageUnits"] = CheckValue("ExtraStorageUnits", value); }
    }
    
    public decimal PortalSeats
    {
      get { return (decimal)Row["PortalSeats"]; }
      set { Row["PortalSeats"] = CheckValue("PortalSeats", value); }
    }
    
    public decimal UserSeats
    {
      get { return (decimal)Row["UserSeats"]; }
      set { Row["UserSeats"] = CheckValue("UserSeats", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateModified
    {
      get { return DateToLocal((DateTime)Row["DateModified"]); }
      set { Row["DateModified"] = CheckValue("DateModified", value); }
    }

    public DateTime DateModifiedUtc
    {
      get { return (DateTime)Row["DateModified"]; }
    }
    
    public DateTime DateCreated
    {
      get { return DateToLocal((DateTime)Row["DateCreated"]); }
      set { Row["DateCreated"] = CheckValue("DateCreated", value); }
    }

    public DateTime DateCreatedUtc
    {
      get { return (DateTime)Row["DateCreated"]; }
    }
    
    public DateTime DateDue
    {
      get { return DateToLocal((DateTime)Row["DateDue"]); }
      set { Row["DateDue"] = CheckValue("DateDue", value); }
    }

    public DateTime DateDueUtc
    {
      get { return (DateTime)Row["DateDue"]; }
    }
    
    public DateTime DateBilled
    {
      get { return DateToLocal((DateTime)Row["DateBilled"]); }
      set { Row["DateBilled"] = CheckValue("DateBilled", value); }
    }

    public DateTime DateBilledUtc
    {
      get { return (DateTime)Row["DateBilled"]; }
    }
    
    public DateTime DateEnd
    {
      get { return DateToLocal((DateTime)Row["DateEnd"]); }
      set { Row["DateEnd"] = CheckValue("DateEnd", value); }
    }

    public DateTime DateEndUtc
    {
      get { return (DateTime)Row["DateEnd"]; }
    }
    
    public DateTime DateStart
    {
      get { return DateToLocal((DateTime)Row["DateStart"]); }
      set { Row["DateStart"] = CheckValue("DateStart", value); }
    }

    public DateTime DateStartUtc
    {
      get { return (DateTime)Row["DateStart"]; }
    }
    

    #endregion
    
    
  }

  public partial class Invoices : BaseCollection, IEnumerable<Invoice>
  {
    public Invoices(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Invoices"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "InvoiceID"; }
    }



    public Invoice this[int index]
    {
      get { return new Invoice(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(Invoice invoice);
    partial void AfterRowInsert(Invoice invoice);
    partial void BeforeRowEdit(Invoice invoice);
    partial void AfterRowEdit(Invoice invoice);
    partial void BeforeRowDelete(int invoiceID);
    partial void AfterRowDelete(int invoiceID);    

    partial void BeforeDBDelete(int invoiceID);
    partial void AfterDBDelete(int invoiceID);    

    #endregion

    #region Public Methods

    public InvoiceProxy[] GetInvoiceProxies()
    {
      List<InvoiceProxy> list = new List<InvoiceProxy>();

      foreach (Invoice item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int invoiceID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Invoices] WHERE ([InvoiceID] = @InvoiceID);";
        deleteCommand.Parameters.Add("InvoiceID", SqlDbType.Int);
        deleteCommand.Parameters["InvoiceID"].Value = invoiceID;

        BeforeDBDelete(invoiceID);
        BeforeRowDelete(invoiceID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(invoiceID);
        AfterDBDelete(invoiceID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("InvoicesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Invoices] SET     [OrganizationID] = @OrganizationID,    [CreditCardID] = @CreditCardID,    [UserSeats] = @UserSeats,    [PortalSeats] = @PortalSeats,    [ExtraStorageUnits] = @ExtraStorageUnits,    [UserPrice] = @UserPrice,    [PortalPrice] = @PortalPrice,    [StoragePrice] = @StoragePrice,    [IsPortalBilled] = @IsPortalBilled,    [TaxRate] = @TaxRate,    [TotalUserPrice] = @TotalUserPrice,    [TotalPortalPrice] = @TotalPortalPrice,    [TotalStoragePrice] = @TotalStoragePrice,    [TotalTaxPrice] = @TotalTaxPrice,    [TotalAmount] = @TotalAmount,    [DateStart] = @DateStart,    [DateEnd] = @DateEnd,    [DateBilled] = @DateBilled,    [DateDue] = @DateDue,    [IsPaid] = @IsPaid,    [IsPaymentFailed] = @IsPaymentFailed,    [PaymentMethod] = @PaymentMethod,    [PaymentFailedReason] = @PaymentFailedReason,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID  WHERE ([InvoiceID] = @InvoiceID);";

		
		tempParameter = updateCommand.Parameters.Add("InvoiceID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("CreditCardID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserSeats", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = updateCommand.Parameters.Add("PortalSeats", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = updateCommand.Parameters.Add("ExtraStorageUnits", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserPrice", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = updateCommand.Parameters.Add("PortalPrice", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = updateCommand.Parameters.Add("StoragePrice", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsPortalBilled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TaxRate", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = updateCommand.Parameters.Add("TotalUserPrice", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = updateCommand.Parameters.Add("TotalPortalPrice", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = updateCommand.Parameters.Add("TotalStoragePrice", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = updateCommand.Parameters.Add("TotalTaxPrice", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = updateCommand.Parameters.Add("TotalAmount", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateStart", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateEnd", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateBilled", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateDue", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsPaid", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsPaymentFailed", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PaymentMethod", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PaymentFailedReason", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Invoices] (    [OrganizationID],    [CreditCardID],    [UserSeats],    [PortalSeats],    [ExtraStorageUnits],    [UserPrice],    [PortalPrice],    [StoragePrice],    [IsPortalBilled],    [TaxRate],    [TotalUserPrice],    [TotalPortalPrice],    [TotalStoragePrice],    [TotalTaxPrice],    [TotalAmount],    [DateStart],    [DateEnd],    [DateBilled],    [DateDue],    [IsPaid],    [IsPaymentFailed],    [PaymentMethod],    [PaymentFailedReason],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID]) VALUES ( @OrganizationID, @CreditCardID, @UserSeats, @PortalSeats, @ExtraStorageUnits, @UserPrice, @PortalPrice, @StoragePrice, @IsPortalBilled, @TaxRate, @TotalUserPrice, @TotalPortalPrice, @TotalStoragePrice, @TotalTaxPrice, @TotalAmount, @DateStart, @DateEnd, @DateBilled, @DateDue, @IsPaid, @IsPaymentFailed, @PaymentMethod, @PaymentFailedReason, @DateCreated, @DateModified, @CreatorID, @ModifierID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("PaymentFailedReason", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PaymentMethod", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsPaymentFailed", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsPaid", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateDue", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateBilled", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateEnd", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateStart", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("TotalAmount", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = insertCommand.Parameters.Add("TotalTaxPrice", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = insertCommand.Parameters.Add("TotalStoragePrice", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = insertCommand.Parameters.Add("TotalPortalPrice", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = insertCommand.Parameters.Add("TotalUserPrice", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = insertCommand.Parameters.Add("TaxRate", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsPortalBilled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("StoragePrice", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = insertCommand.Parameters.Add("PortalPrice", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserPrice", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = insertCommand.Parameters.Add("ExtraStorageUnits", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = insertCommand.Parameters.Add("PortalSeats", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserSeats", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreditCardID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Invoices] WHERE ([InvoiceID] = @InvoiceID);";
		deleteCommand.Parameters.Add("InvoiceID", SqlDbType.Int);

		try
		{
		  foreach (Invoice invoice in this)
		  {
			if (invoice.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(invoice);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = invoice.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["InvoiceID"].AutoIncrement = false;
			  Table.Columns["InvoiceID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				invoice.Row["InvoiceID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(invoice);
			}
			else if (invoice.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(invoice);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = invoice.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(invoice);
			}
			else if (invoice.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)invoice.Row["InvoiceID", DataRowVersion.Original];
			  deleteCommand.Parameters["InvoiceID"].Value = id;
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

      foreach (Invoice invoice in this)
      {
        if (invoice.Row.Table.Columns.Contains("CreatorID") && (int)invoice.Row["CreatorID"] == 0) invoice.Row["CreatorID"] = LoginUser.UserID;
        if (invoice.Row.Table.Columns.Contains("ModifierID")) invoice.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Invoice FindByInvoiceID(int invoiceID)
    {
      foreach (Invoice invoice in this)
      {
        if (invoice.InvoiceID == invoiceID)
        {
          return invoice;
        }
      }
      return null;
    }

    public virtual Invoice AddNewInvoice()
    {
      if (Table.Columns.Count < 1) LoadColumns("Invoices");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Invoice(row, this);
    }
    
    public virtual void LoadByInvoiceID(int invoiceID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [InvoiceID], [OrganizationID], [CreditCardID], [UserSeats], [PortalSeats], [ExtraStorageUnits], [UserPrice], [PortalPrice], [StoragePrice], [IsPortalBilled], [TaxRate], [TotalUserPrice], [TotalPortalPrice], [TotalStoragePrice], [TotalTaxPrice], [TotalAmount], [DateStart], [DateEnd], [DateBilled], [DateDue], [IsPaid], [IsPaymentFailed], [PaymentMethod], [PaymentFailedReason], [DateCreated], [DateModified], [CreatorID], [ModifierID] FROM [dbo].[Invoices] WHERE ([InvoiceID] = @InvoiceID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("InvoiceID", invoiceID);
        Fill(command);
      }
    }
    
    public static Invoice GetInvoice(LoginUser loginUser, int invoiceID)
    {
      Invoices invoices = new Invoices(loginUser);
      invoices.LoadByInvoiceID(invoiceID);
      if (invoices.IsEmpty)
        return null;
      else
        return invoices[0];
    }
    
    
    

    #endregion

    #region IEnumerable<Invoice> Members

    public IEnumerator<Invoice> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Invoice(row, this);
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
