using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class OrganizationsViewItem : BaseItem
  {
    private OrganizationsView _organizationsView;
    
    public OrganizationsViewItem(DataRow row, OrganizationsView organizationsView): base(row, organizationsView)
    {
      _organizationsView = organizationsView;
    }
	
    #region Properties
    
    public OrganizationsView Collection
    {
      get { return _organizationsView; }
    }
        
    
    public string PrimaryContact
    {
      get { return Row["PrimaryContact"] != DBNull.Value ? (string)Row["PrimaryContact"] : null; }
    }
    
    public string CreatedBy
    {
      get { return Row["CreatedBy"] != DBNull.Value ? (string)Row["CreatedBy"] : null; }
    }
    
    public string LastModifiedBy
    {
      get { return Row["LastModifiedBy"] != DBNull.Value ? (string)Row["LastModifiedBy"] : null; }
    }
    
    public string DefaultSupportUser
    {
      get { return Row["DefaultSupportUser"] != DBNull.Value ? (string)Row["DefaultSupportUser"] : null; }
    }
    
    public int? SupportHoursUsed
    {
      get { return Row["SupportHoursUsed"] != DBNull.Value ? (int?)Row["SupportHoursUsed"] : null; }
    }
    
    public int? SupportHoursRemaining
    {
      get { return Row["SupportHoursRemaining"] != DBNull.Value ? (int?)Row["SupportHoursRemaining"] : null; }
    }
    
    public string PhoneNumbers
    {
      get { return Row["PhoneNumbers"] != DBNull.Value ? (string)Row["PhoneNumbers"] : null; }
    }
    
    
    

    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public string Website
    {
      get { return Row["Website"] != DBNull.Value ? (string)Row["Website"] : null; }
      set { Row["Website"] = CheckValue("Website", value); }
    }
    
    public string InActiveReason
    {
      get { return Row["InActiveReason"] != DBNull.Value ? (string)Row["InActiveReason"] : null; }
      set { Row["InActiveReason"] = CheckValue("InActiveReason", value); }
    }
    
    public int? PrimaryUserID
    {
      get { return Row["PrimaryUserID"] != DBNull.Value ? (int?)Row["PrimaryUserID"] : null; }
      set { Row["PrimaryUserID"] = CheckValue("PrimaryUserID", value); }
    }
    
    public string PrimaryContactEmail
    {
      get { return Row["PrimaryContactEmail"] != DBNull.Value ? (string)Row["PrimaryContactEmail"] : null; }
      set { Row["PrimaryContactEmail"] = CheckValue("PrimaryContactEmail", value); }
    }
    
    public int? ParentID
    {
      get { return Row["ParentID"] != DBNull.Value ? (int?)Row["ParentID"] : null; }
      set { Row["ParentID"] = CheckValue("ParentID", value); }
    }
    
    public string SlaName
    {
      get { return Row["SlaName"] != DBNull.Value ? (string)Row["SlaName"] : null; }
      set { Row["SlaName"] = CheckValue("SlaName", value); }
    }
    
    public string CRMLinkID
    {
      get { return Row["CRMLinkID"] != DBNull.Value ? (string)Row["CRMLinkID"] : null; }
      set { Row["CRMLinkID"] = CheckValue("CRMLinkID", value); }
    }
    
    public int? SlaLevelID
    {
      get { return Row["SlaLevelID"] != DBNull.Value ? (int?)Row["SlaLevelID"] : null; }
      set { Row["SlaLevelID"] = CheckValue("SlaLevelID", value); }
    }
    
    public int? DefaultWikiArticleID
    {
      get { return Row["DefaultWikiArticleID"] != DBNull.Value ? (int?)Row["DefaultWikiArticleID"] : null; }
      set { Row["DefaultWikiArticleID"] = CheckValue("DefaultWikiArticleID", value); }
    }
    
    public int? DefaultSupportGroupID
    {
      get { return Row["DefaultSupportGroupID"] != DBNull.Value ? (int?)Row["DefaultSupportGroupID"] : null; }
      set { Row["DefaultSupportGroupID"] = CheckValue("DefaultSupportGroupID", value); }
    }
    
    public int? DefaultSupportUserID
    {
      get { return Row["DefaultSupportUserID"] != DBNull.Value ? (int?)Row["DefaultSupportUserID"] : null; }
      set { Row["DefaultSupportUserID"] = CheckValue("DefaultSupportUserID", value); }
    }
    
    public string DefaultSupportGroup
    {
      get { return Row["DefaultSupportGroup"] != DBNull.Value ? (string)Row["DefaultSupportGroup"] : null; }
      set { Row["DefaultSupportGroup"] = CheckValue("DefaultSupportGroup", value); }
    }
    
    public string CompanyDomains
    {
      get { return Row["CompanyDomains"] != DBNull.Value ? (string)Row["CompanyDomains"] : null; }
      set { Row["CompanyDomains"] = CheckValue("CompanyDomains", value); }
    }
    
    public int? CustDistIndexTrend
    {
      get { return Row["CustDistIndexTrend"] != DBNull.Value ? (int?)Row["CustDistIndexTrend"] : null; }
      set { Row["CustDistIndexTrend"] = CheckValue("CustDistIndexTrend", value); }
    }
    

    
    public int CustDisIndex
    {
      get { return (int)Row["CustDisIndex"]; }
      set { Row["CustDisIndex"] = CheckValue("CustDisIndex", value); }
    }
    
    public bool NeedsIndexing
    {
      get { return (bool)Row["NeedsIndexing"]; }
      set { Row["NeedsIndexing"] = CheckValue("NeedsIndexing", value); }
    }
    
    public int SupportHoursMonth
    {
      get { return (int)Row["SupportHoursMonth"]; }
      set { Row["SupportHoursMonth"] = CheckValue("SupportHoursMonth", value); }
    }
    
    public Guid PortalGuid
    {
      get { return (Guid)Row["PortalGuid"]; }
      set { Row["PortalGuid"] = CheckValue("PortalGuid", value); }
    }
    
    public bool HasPortalAccess
    {
      get { return (bool)Row["HasPortalAccess"]; }
      set { Row["HasPortalAccess"] = CheckValue("HasPortalAccess", value); }
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
    
    public bool IsActive
    {
      get { return (bool)Row["IsActive"]; }
      set { Row["IsActive"] = CheckValue("IsActive", value); }
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
    
    
    

    
    public DateTime? SAExpirationDate
    {
      get { return Row["SAExpirationDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["SAExpirationDate"]) : null; }
      set { Row["SAExpirationDate"] = CheckValue("SAExpirationDate", value); }
    }

    public DateTime? SAExpirationDateUtc
    {
      get { return Row["SAExpirationDate"] != DBNull.Value ? (DateTime?)Row["SAExpirationDate"] : null; }
    }
    

    
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
    

    #endregion
    
    
  }

  public partial class OrganizationsView : BaseCollection, IEnumerable<OrganizationsViewItem>
  {
    public OrganizationsView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "OrganizationsView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "OrganizationID"; }
    }



    public OrganizationsViewItem this[int index]
    {
      get { return new OrganizationsViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(OrganizationsViewItem organizationsViewItem);
    partial void AfterRowInsert(OrganizationsViewItem organizationsViewItem);
    partial void BeforeRowEdit(OrganizationsViewItem organizationsViewItem);
    partial void AfterRowEdit(OrganizationsViewItem organizationsViewItem);
    partial void BeforeRowDelete(int organizationID);
    partial void AfterRowDelete(int organizationID);    

    partial void BeforeDBDelete(int organizationID);
    partial void AfterDBDelete(int organizationID);    

    #endregion

    #region Public Methods

    public OrganizationsViewItemProxy[] GetOrganizationsViewItemProxies()
    {
      List<OrganizationsViewItemProxy> list = new List<OrganizationsViewItemProxy>();

      foreach (OrganizationsViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int organizationID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[OrganizationsView] WHERE ([OrganizationID] = @OrganizationID);";
        deleteCommand.Parameters.Add("OrganizationID", SqlDbType.Int);
        deleteCommand.Parameters["OrganizationID"].Value = organizationID;

        BeforeDBDelete(organizationID);
        BeforeRowDelete(organizationID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(organizationID);
        AfterDBDelete(organizationID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("OrganizationsViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[OrganizationsView] SET     [Name] = @Name,    [Description] = @Description,    [Website] = @Website,    [IsActive] = @IsActive,    [InActiveReason] = @InActiveReason,    [PrimaryUserID] = @PrimaryUserID,    [PrimaryContactEmail] = @PrimaryContactEmail,    [PrimaryContact] = @PrimaryContact,    [ParentID] = @ParentID,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [HasPortalAccess] = @HasPortalAccess,    [CreatedBy] = @CreatedBy,    [LastModifiedBy] = @LastModifiedBy,    [SAExpirationDate] = @SAExpirationDate,    [SlaName] = @SlaName,    [CRMLinkID] = @CRMLinkID,    [PortalGuid] = @PortalGuid,    [SlaLevelID] = @SlaLevelID,    [DefaultWikiArticleID] = @DefaultWikiArticleID,    [DefaultSupportGroupID] = @DefaultSupportGroupID,    [DefaultSupportUserID] = @DefaultSupportUserID,    [DefaultSupportUser] = @DefaultSupportUser,    [DefaultSupportGroup] = @DefaultSupportGroup,    [CompanyDomains] = @CompanyDomains,    [SupportHoursMonth] = @SupportHoursMonth,    [SupportHoursUsed] = @SupportHoursUsed,    [SupportHoursRemaining] = @SupportHoursRemaining,    [NeedsIndexing] = @NeedsIndexing,    [CustDisIndex] = @CustDisIndex,    [CustDistIndexTrend] = @CustDistIndexTrend,    [PhoneNumbers] = @PhoneNumbers  WHERE ([OrganizationID] = @OrganizationID);";

		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.NVarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Website", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsActive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("InActiveReason", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PrimaryUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("PrimaryContactEmail", SqlDbType.NVarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PrimaryContact", SqlDbType.NVarChar, 202);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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
		
		tempParameter = updateCommand.Parameters.Add("HasPortalAccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CreatedBy", SqlDbType.NVarChar, 202);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastModifiedBy", SqlDbType.NVarChar, 202);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SAExpirationDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaName", SqlDbType.VarChar, 150);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CRMLinkID", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PortalGuid", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaLevelID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DefaultWikiArticleID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DefaultSupportGroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DefaultSupportUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DefaultSupportUser", SqlDbType.NVarChar, 202);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DefaultSupportGroup", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CompanyDomains", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SupportHoursMonth", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SupportHoursUsed", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SupportHoursRemaining", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("NeedsIndexing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CustDisIndex", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CustDistIndexTrend", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("PhoneNumbers", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[OrganizationsView] (    [OrganizationID],    [Name],    [Description],    [Website],    [IsActive],    [InActiveReason],    [PrimaryUserID],    [PrimaryContactEmail],    [PrimaryContact],    [ParentID],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [HasPortalAccess],    [CreatedBy],    [LastModifiedBy],    [SAExpirationDate],    [SlaName],    [CRMLinkID],    [PortalGuid],    [SlaLevelID],    [DefaultWikiArticleID],    [DefaultSupportGroupID],    [DefaultSupportUserID],    [DefaultSupportUser],    [DefaultSupportGroup],    [CompanyDomains],    [SupportHoursMonth],    [SupportHoursUsed],    [SupportHoursRemaining],    [NeedsIndexing],    [CustDisIndex],    [CustDistIndexTrend],    [PhoneNumbers]) VALUES ( @OrganizationID, @Name, @Description, @Website, @IsActive, @InActiveReason, @PrimaryUserID, @PrimaryContactEmail, @PrimaryContact, @ParentID, @DateCreated, @DateModified, @CreatorID, @ModifierID, @HasPortalAccess, @CreatedBy, @LastModifiedBy, @SAExpirationDate, @SlaName, @CRMLinkID, @PortalGuid, @SlaLevelID, @DefaultWikiArticleID, @DefaultSupportGroupID, @DefaultSupportUserID, @DefaultSupportUser, @DefaultSupportGroup, @CompanyDomains, @SupportHoursMonth, @SupportHoursUsed, @SupportHoursRemaining, @NeedsIndexing, @CustDisIndex, @CustDistIndexTrend, @PhoneNumbers); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("PhoneNumbers", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CustDistIndexTrend", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CustDisIndex", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("NeedsIndexing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SupportHoursRemaining", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("SupportHoursUsed", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("SupportHoursMonth", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CompanyDomains", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DefaultSupportGroup", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DefaultSupportUser", SqlDbType.NVarChar, 202);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DefaultSupportUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DefaultSupportGroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DefaultWikiArticleID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaLevelID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("PortalGuid", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CRMLinkID", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaName", SqlDbType.VarChar, 150);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SAExpirationDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastModifiedBy", SqlDbType.NVarChar, 202);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatedBy", SqlDbType.NVarChar, 202);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("HasPortalAccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
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
		
		tempParameter = insertCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("PrimaryContact", SqlDbType.NVarChar, 202);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PrimaryContactEmail", SqlDbType.NVarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PrimaryUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("InActiveReason", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsActive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Website", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.NVarChar, 255);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[OrganizationsView] WHERE ([OrganizationID] = @OrganizationID);";
		deleteCommand.Parameters.Add("OrganizationID", SqlDbType.Int);

		try
		{
		  foreach (OrganizationsViewItem organizationsViewItem in this)
		  {
			if (organizationsViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(organizationsViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = organizationsViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["OrganizationID"].AutoIncrement = false;
			  Table.Columns["OrganizationID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				organizationsViewItem.Row["OrganizationID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(organizationsViewItem);
			}
			else if (organizationsViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(organizationsViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = organizationsViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(organizationsViewItem);
			}
			else if (organizationsViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)organizationsViewItem.Row["OrganizationID", DataRowVersion.Original];
			  deleteCommand.Parameters["OrganizationID"].Value = id;
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

      foreach (OrganizationsViewItem organizationsViewItem in this)
      {
        if (organizationsViewItem.Row.Table.Columns.Contains("CreatorID") && (int)organizationsViewItem.Row["CreatorID"] == 0) organizationsViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (organizationsViewItem.Row.Table.Columns.Contains("ModifierID")) organizationsViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public OrganizationsViewItem FindByOrganizationID(int organizationID)
    {
      foreach (OrganizationsViewItem organizationsViewItem in this)
      {
        if (organizationsViewItem.OrganizationID == organizationID)
        {
          return organizationsViewItem;
        }
      }
      return null;
    }

    public virtual OrganizationsViewItem AddNewOrganizationsViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("OrganizationsView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new OrganizationsViewItem(row, this);
    }
    
    public virtual void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [OrganizationID], [Name], [Description], [Website], [IsActive], [InActiveReason], [PrimaryUserID], [PrimaryContactEmail], [PrimaryContact], [ParentID], [DateCreated], [DateModified], [CreatorID], [ModifierID], [HasPortalAccess], [CreatedBy], [LastModifiedBy], [SAExpirationDate], [SlaName], [CRMLinkID], [PortalGuid], [SlaLevelID], [DefaultWikiArticleID], [DefaultSupportGroupID], [DefaultSupportUserID], [DefaultSupportUser], [DefaultSupportGroup], [CompanyDomains], [SupportHoursMonth], [SupportHoursUsed], [SupportHoursRemaining], [NeedsIndexing], [CustDisIndex], [CustDistIndexTrend], [PhoneNumbers] FROM [dbo].[OrganizationsView] WHERE ([OrganizationID] = @OrganizationID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }
    
    public static OrganizationsViewItem GetOrganizationsViewItem(LoginUser loginUser, int organizationID)
    {
      OrganizationsView organizationsView = new OrganizationsView(loginUser);
      organizationsView.LoadByOrganizationID(organizationID);
      if (organizationsView.IsEmpty)
        return null;
      else
        return organizationsView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<OrganizationsViewItem> Members

    public IEnumerator<OrganizationsViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new OrganizationsViewItem(row, this);
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
