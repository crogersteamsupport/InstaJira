using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CRMLinkTableItem : BaseItem
  {
    private CRMLinkTable _cRMLinkTable;
    
    public CRMLinkTableItem(DataRow row, CRMLinkTable cRMLinkTable): base(row, cRMLinkTable)
    {
      _cRMLinkTable = cRMLinkTable;
    }
	
    #region Properties
    
    public CRMLinkTable Collection
    {
      get { return _cRMLinkTable; }
    }
        
    
    
    
    public int CRMLinkID
    {
      get { return (int)Row["CRMLinkID"]; }
    }
    

    
    public string Username
    {
      get { return Row["Username"] != DBNull.Value ? (string)Row["Username"] : null; }
      set { Row["Username"] = CheckValue("Username", value); }
    }
    
    public string Password
    {
      get { return Row["Password"] != DBNull.Value ? (string)Row["Password"] : null; }
      set { Row["Password"] = CheckValue("Password", value); }
    }
    
    public string SecurityToken
    {
      get { return Row["SecurityToken"] != DBNull.Value ? (string)Row["SecurityToken"] : null; }
      set { Row["SecurityToken"] = CheckValue("SecurityToken", value); }
    }
    
    public string TypeFieldMatch
    {
      get { return Row["TypeFieldMatch"] != DBNull.Value ? (string)Row["TypeFieldMatch"] : null; }
      set { Row["TypeFieldMatch"] = CheckValue("TypeFieldMatch", value); }
    }
    
    public int? DefaultSlaLevelID
    {
      get { return Row["DefaultSlaLevelID"] != DBNull.Value ? (int?)Row["DefaultSlaLevelID"] : null; }
      set { Row["DefaultSlaLevelID"] = CheckValue("DefaultSlaLevelID", value); }
    }
    
    public bool? PullCasesAsTickets
    {
      get { return Row["PullCasesAsTickets"] != DBNull.Value ? (bool?)Row["PullCasesAsTickets"] : null; }
      set { Row["PullCasesAsTickets"] = CheckValue("PullCasesAsTickets", value); }
    }
    
    public bool? PushTicketsAsCases
    {
      get { return Row["PushTicketsAsCases"] != DBNull.Value ? (bool?)Row["PushTicketsAsCases"] : null; }
      set { Row["PushTicketsAsCases"] = CheckValue("PushTicketsAsCases", value); }
    }
    
    public bool? PullCustomerProducts
    {
      get { return Row["PullCustomerProducts"] != DBNull.Value ? (bool?)Row["PullCustomerProducts"] : null; }
      set { Row["PullCustomerProducts"] = CheckValue("PullCustomerProducts", value); }
    }
    
    public bool? UpdateStatus
    {
      get { return Row["UpdateStatus"] != DBNull.Value ? (bool?)Row["UpdateStatus"] : null; }
      set { Row["UpdateStatus"] = CheckValue("UpdateStatus", value); }
    }
    
    public int? ActionTypeIDToPush
    {
      get { return Row["ActionTypeIDToPush"] != DBNull.Value ? (int?)Row["ActionTypeIDToPush"] : null; }
      set { Row["ActionTypeIDToPush"] = CheckValue("ActionTypeIDToPush", value); }
    }
    
    public string HostName
    {
      get { return Row["HostName"] != DBNull.Value ? (string)Row["HostName"] : null; }
      set { Row["HostName"] = CheckValue("HostName", value); }
    }
    
    public string DefaultProject
    {
      get { return Row["DefaultProject"] != DBNull.Value ? (string)Row["DefaultProject"] : null; }
      set { Row["DefaultProject"] = CheckValue("DefaultProject", value); }
    }
    
    public string RestrictedToTicketTypes
    {
      get { return Row["RestrictedToTicketTypes"] != DBNull.Value ? (string)Row["RestrictedToTicketTypes"] : null; }
      set { Row["RestrictedToTicketTypes"] = CheckValue("RestrictedToTicketTypes", value); }
    }
    
    public string ExcludedTicketStatusUpdate
    {
      get { return Row["ExcludedTicketStatusUpdate"] != DBNull.Value ? (string)Row["ExcludedTicketStatusUpdate"] : null; }
      set { Row["ExcludedTicketStatusUpdate"] = CheckValue("ExcludedTicketStatusUpdate", value); }
    }
    

    
    public bool UseNetworkCredentials
    {
      get { return (bool)Row["UseNetworkCredentials"]; }
      set { Row["UseNetworkCredentials"] = CheckValue("UseNetworkCredentials", value); }
    }
    
    public bool IncludeIssueNonRequired
    {
      get { return (bool)Row["IncludeIssueNonRequired"]; }
      set { Row["IncludeIssueNonRequired"] = CheckValue("IncludeIssueNonRequired", value); }
    }
    
    public string InstanceName
    {
      get { return (string)Row["InstanceName"]; }
      set { Row["InstanceName"] = CheckValue("InstanceName", value); }
    }
    
    public bool UpdateTicketType
    {
      get { return (bool)Row["UpdateTicketType"]; }
      set { Row["UpdateTicketType"] = CheckValue("UpdateTicketType", value); }
    }
    
    public bool AlwaysUseDefaultProjectKey
    {
      get { return (bool)Row["AlwaysUseDefaultProjectKey"]; }
      set { Row["AlwaysUseDefaultProjectKey"] = CheckValue("AlwaysUseDefaultProjectKey", value); }
    }
    
    public bool UseSandBoxServer
    {
      get { return (bool)Row["UseSandBoxServer"]; }
      set { Row["UseSandBoxServer"] = CheckValue("UseSandBoxServer", value); }
    }
    
    public bool MatchAccountsByName
    {
      get { return (bool)Row["MatchAccountsByName"]; }
      set { Row["MatchAccountsByName"] = CheckValue("MatchAccountsByName", value); }
    }
    
    public bool SendWelcomeEmail
    {
      get { return (bool)Row["SendWelcomeEmail"]; }
      set { Row["SendWelcomeEmail"] = CheckValue("SendWelcomeEmail", value); }
    }
    
    public bool AllowPortalAccess
    {
      get { return (bool)Row["AllowPortalAccess"]; }
      set { Row["AllowPortalAccess"] = CheckValue("AllowPortalAccess", value); }
    }
    
    public int LastTicketID
    {
      get { return (int)Row["LastTicketID"]; }
      set { Row["LastTicketID"] = CheckValue("LastTicketID", value); }
    }
    
    public bool SendBackTicketData
    {
      get { return (bool)Row["SendBackTicketData"]; }
      set { Row["SendBackTicketData"] = CheckValue("SendBackTicketData", value); }
    }
    
    public string CRMType
    {
      get { return (string)Row["CRMType"]; }
      set { Row["CRMType"] = CheckValue("CRMType", value); }
    }
    
    public bool Active
    {
      get { return (bool)Row["Active"]; }
      set { Row["Active"] = CheckValue("Active", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? LastLink
    {
      get { return Row["LastLink"] != DBNull.Value ? DateToLocal((DateTime?)Row["LastLink"]) : null; }
      set { Row["LastLink"] = CheckValue("LastLink", value); }
    }

    public DateTime? LastLinkUtc
    {
      get { return Row["LastLink"] != DBNull.Value ? (DateTime?)Row["LastLink"] : null; }
    }
    

    
    public DateTime LastProcessed
    {
      get { return DateToLocal((DateTime)Row["LastProcessed"]); }
      set { Row["LastProcessed"] = CheckValue("LastProcessed", value); }
    }

    public DateTime LastProcessedUtc
    {
      get { return (DateTime)Row["LastProcessed"]; }
    }
    

    #endregion
    
    
  }

  public partial class CRMLinkTable : BaseCollection, IEnumerable<CRMLinkTableItem>
  {
    public CRMLinkTable(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CRMLinkTable"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CRMLinkID"; }
    }



    public CRMLinkTableItem this[int index]
    {
      get { return new CRMLinkTableItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CRMLinkTableItem cRMLinkTableItem);
    partial void AfterRowInsert(CRMLinkTableItem cRMLinkTableItem);
    partial void BeforeRowEdit(CRMLinkTableItem cRMLinkTableItem);
    partial void AfterRowEdit(CRMLinkTableItem cRMLinkTableItem);
    partial void BeforeRowDelete(int cRMLinkID);
    partial void AfterRowDelete(int cRMLinkID);    

    partial void BeforeDBDelete(int cRMLinkID);
    partial void AfterDBDelete(int cRMLinkID);    

    #endregion

    #region Public Methods

    public CRMLinkTableItemProxy[] GetCRMLinkTableItemProxies()
    {
      List<CRMLinkTableItemProxy> list = new List<CRMLinkTableItemProxy>();

      foreach (CRMLinkTableItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int cRMLinkID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CRMLinkTable] WHERE ([CRMLinkID] = @CRMLinkID);";
        deleteCommand.Parameters.Add("CRMLinkID", SqlDbType.Int);
        deleteCommand.Parameters["CRMLinkID"].Value = cRMLinkID;

        BeforeDBDelete(cRMLinkID);
        BeforeRowDelete(cRMLinkID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(cRMLinkID);
        AfterDBDelete(cRMLinkID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CRMLinkTableSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CRMLinkTable] SET     [OrganizationID] = @OrganizationID,    [Active] = @Active,    [CRMType] = @CRMType,    [Username] = @Username,    [Password] = @Password,    [SecurityToken] = @SecurityToken,    [TypeFieldMatch] = @TypeFieldMatch,    [LastLink] = @LastLink,    [SendBackTicketData] = @SendBackTicketData,    [LastProcessed] = @LastProcessed,    [LastTicketID] = @LastTicketID,    [AllowPortalAccess] = @AllowPortalAccess,    [SendWelcomeEmail] = @SendWelcomeEmail,    [DefaultSlaLevelID] = @DefaultSlaLevelID,    [PullCasesAsTickets] = @PullCasesAsTickets,    [PushTicketsAsCases] = @PushTicketsAsCases,    [PullCustomerProducts] = @PullCustomerProducts,    [UpdateStatus] = @UpdateStatus,    [ActionTypeIDToPush] = @ActionTypeIDToPush,    [HostName] = @HostName,    [DefaultProject] = @DefaultProject,    [MatchAccountsByName] = @MatchAccountsByName,    [UseSandBoxServer] = @UseSandBoxServer,    [AlwaysUseDefaultProjectKey] = @AlwaysUseDefaultProjectKey,    [RestrictedToTicketTypes] = @RestrictedToTicketTypes,    [UpdateTicketType] = @UpdateTicketType,    [InstanceName] = @InstanceName,    [ExcludedTicketStatusUpdate] = @ExcludedTicketStatusUpdate,    [IncludeIssueNonRequired] = @IncludeIssueNonRequired,    [UseNetworkCredentials] = @UseNetworkCredentials  WHERE ([CRMLinkID] = @CRMLinkID);";

		
		tempParameter = updateCommand.Parameters.Add("CRMLinkID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Active", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CRMType", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Username", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Password", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SecurityToken", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TypeFieldMatch", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastLink", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SendBackTicketData", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastProcessed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastTicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("AllowPortalAccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SendWelcomeEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DefaultSlaLevelID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("PullCasesAsTickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PushTicketsAsCases", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PullCustomerProducts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UpdateStatus", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ActionTypeIDToPush", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("HostName", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DefaultProject", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("MatchAccountsByName", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UseSandBoxServer", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("AlwaysUseDefaultProjectKey", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("RestrictedToTicketTypes", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UpdateTicketType", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("InstanceName", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ExcludedTicketStatusUpdate", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IncludeIssueNonRequired", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UseNetworkCredentials", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CRMLinkTable] (    [OrganizationID],    [Active],    [CRMType],    [Username],    [Password],    [SecurityToken],    [TypeFieldMatch],    [LastLink],    [SendBackTicketData],    [LastProcessed],    [LastTicketID],    [AllowPortalAccess],    [SendWelcomeEmail],    [DefaultSlaLevelID],    [PullCasesAsTickets],    [PushTicketsAsCases],    [PullCustomerProducts],    [UpdateStatus],    [ActionTypeIDToPush],    [HostName],    [DefaultProject],    [MatchAccountsByName],    [UseSandBoxServer],    [AlwaysUseDefaultProjectKey],    [RestrictedToTicketTypes],    [UpdateTicketType],    [InstanceName],    [ExcludedTicketStatusUpdate],    [IncludeIssueNonRequired],    [UseNetworkCredentials]) VALUES ( @OrganizationID, @Active, @CRMType, @Username, @Password, @SecurityToken, @TypeFieldMatch, @LastLink, @SendBackTicketData, @LastProcessed, @LastTicketID, @AllowPortalAccess, @SendWelcomeEmail, @DefaultSlaLevelID, @PullCasesAsTickets, @PushTicketsAsCases, @PullCustomerProducts, @UpdateStatus, @ActionTypeIDToPush, @HostName, @DefaultProject, @MatchAccountsByName, @UseSandBoxServer, @AlwaysUseDefaultProjectKey, @RestrictedToTicketTypes, @UpdateTicketType, @InstanceName, @ExcludedTicketStatusUpdate, @IncludeIssueNonRequired, @UseNetworkCredentials); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("UseNetworkCredentials", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IncludeIssueNonRequired", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ExcludedTicketStatusUpdate", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("InstanceName", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UpdateTicketType", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RestrictedToTicketTypes", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("AlwaysUseDefaultProjectKey", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UseSandBoxServer", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("MatchAccountsByName", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DefaultProject", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("HostName", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ActionTypeIDToPush", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("UpdateStatus", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PullCustomerProducts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PushTicketsAsCases", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PullCasesAsTickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DefaultSlaLevelID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("SendWelcomeEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("AllowPortalAccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastTicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastProcessed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("SendBackTicketData", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastLink", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("TypeFieldMatch", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SecurityToken", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Password", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Username", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CRMType", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Active", SqlDbType.Bit, 1);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CRMLinkTable] WHERE ([CRMLinkID] = @CRMLinkID);";
		deleteCommand.Parameters.Add("CRMLinkID", SqlDbType.Int);

		try
		{
		  foreach (CRMLinkTableItem cRMLinkTableItem in this)
		  {
			if (cRMLinkTableItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(cRMLinkTableItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = cRMLinkTableItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CRMLinkID"].AutoIncrement = false;
			  Table.Columns["CRMLinkID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				cRMLinkTableItem.Row["CRMLinkID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(cRMLinkTableItem);
			}
			else if (cRMLinkTableItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(cRMLinkTableItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = cRMLinkTableItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(cRMLinkTableItem);
			}
			else if (cRMLinkTableItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)cRMLinkTableItem.Row["CRMLinkID", DataRowVersion.Original];
			  deleteCommand.Parameters["CRMLinkID"].Value = id;
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

      foreach (CRMLinkTableItem cRMLinkTableItem in this)
      {
        if (cRMLinkTableItem.Row.Table.Columns.Contains("CreatorID") && (int)cRMLinkTableItem.Row["CreatorID"] == 0) cRMLinkTableItem.Row["CreatorID"] = LoginUser.UserID;
        if (cRMLinkTableItem.Row.Table.Columns.Contains("ModifierID")) cRMLinkTableItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CRMLinkTableItem FindByCRMLinkID(int cRMLinkID)
    {
      foreach (CRMLinkTableItem cRMLinkTableItem in this)
      {
        if (cRMLinkTableItem.CRMLinkID == cRMLinkID)
        {
          return cRMLinkTableItem;
        }
      }
      return null;
    }

    public virtual CRMLinkTableItem AddNewCRMLinkTableItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("CRMLinkTable");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CRMLinkTableItem(row, this);
    }
    
    public virtual void LoadByCRMLinkID(int cRMLinkID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CRMLinkID], [OrganizationID], [Active], [CRMType], [Username], [Password], [SecurityToken], [TypeFieldMatch], [LastLink], [SendBackTicketData], [LastProcessed], [LastTicketID], [AllowPortalAccess], [SendWelcomeEmail], [DefaultSlaLevelID], [PullCasesAsTickets], [PushTicketsAsCases], [PullCustomerProducts], [UpdateStatus], [ActionTypeIDToPush], [HostName], [DefaultProject], [MatchAccountsByName], [UseSandBoxServer], [AlwaysUseDefaultProjectKey], [RestrictedToTicketTypes], [UpdateTicketType], [InstanceName], [ExcludedTicketStatusUpdate], [IncludeIssueNonRequired], [UseNetworkCredentials] FROM [dbo].[CRMLinkTable] WHERE ([CRMLinkID] = @CRMLinkID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CRMLinkID", cRMLinkID);
        Fill(command);
      }
    }
    
    public static CRMLinkTableItem GetCRMLinkTableItem(LoginUser loginUser, int cRMLinkID)
    {
      CRMLinkTable cRMLinkTable = new CRMLinkTable(loginUser);
      cRMLinkTable.LoadByCRMLinkID(cRMLinkID);
      if (cRMLinkTable.IsEmpty)
        return null;
      else
        return cRMLinkTable[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CRMLinkTableItem> Members

    public IEnumerator<CRMLinkTableItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CRMLinkTableItem(row, this);
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
