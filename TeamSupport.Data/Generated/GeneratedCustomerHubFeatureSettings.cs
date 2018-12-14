using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CustomerHubFeatureSetting : BaseItem
  {
    private CustomerHubFeatureSettings _customerHubFeatureSettings;
    
    public CustomerHubFeatureSetting(DataRow row, CustomerHubFeatureSettings customerHubFeatureSettings): base(row, customerHubFeatureSettings)
    {
      _customerHubFeatureSettings = customerHubFeatureSettings;
    }
	
    #region Properties
    
    public CustomerHubFeatureSettings Collection
    {
      get { return _customerHubFeatureSettings; }
    }
        
    
    
    
    public int CustomerHubFeatureSettingID
    {
      get { return (int)Row["CustomerHubFeatureSettingID"]; }
    }
    

    
    public int? DefaultTicketTypeID
    {
      get { return Row["DefaultTicketTypeID"] != DBNull.Value ? (int?)Row["DefaultTicketTypeID"] : null; }
      set { Row["DefaultTicketTypeID"] = CheckValue("DefaultTicketTypeID", value); }
    }
    
    public int? DefaultGroupTypeID
    {
      get { return Row["DefaultGroupTypeID"] != DBNull.Value ? (int?)Row["DefaultGroupTypeID"] : null; }
      set { Row["DefaultGroupTypeID"] = CheckValue("DefaultGroupTypeID", value); }
    }
    
    public int? ModifierID
    {
      get { return Row["ModifierID"] != DBNull.Value ? (int?)Row["ModifierID"] : null; }
      set { Row["ModifierID"] = CheckValue("ModifierID", value); }
    }
    

    
    public int ChatGroupID
    {
      get { return (int)Row["ChatGroupID"]; }
      set { Row["ChatGroupID"] = CheckValue("ChatGroupID", value); }
    }
    
    public bool EnableProductFamilyFiltering
    {
      get { return (bool)Row["EnableProductFamilyFiltering"]; }
      set { Row["EnableProductFamilyFiltering"] = CheckValue("EnableProductFamilyFiltering", value); }
    }
    
    public bool EnableCustomFieldModification
    {
      get { return (bool)Row["EnableCustomFieldModification"]; }
      set { Row["EnableCustomFieldModification"] = CheckValue("EnableCustomFieldModification", value); }
    }
    
    public bool EnableCustomerSpecificKB
    {
      get { return (bool)Row["EnableCustomerSpecificKB"]; }
      set { Row["EnableCustomerSpecificKB"] = CheckValue("EnableCustomerSpecificKB", value); }
    }
    
    public bool EnableAnonymousProductAssociation
    {
      get { return (bool)Row["EnableAnonymousProductAssociation"]; }
      set { Row["EnableAnonymousProductAssociation"] = CheckValue("EnableAnonymousProductAssociation", value); }
    }
    
    public int CommunitySortTypeID
    {
      get { return (int)Row["CommunitySortTypeID"]; }
      set { Row["CommunitySortTypeID"] = CheckValue("CommunitySortTypeID", value); }
    }
    
    public int KnowledgeBaseSortTypeID
    {
      get { return (int)Row["KnowledgeBaseSortTypeID"]; }
      set { Row["KnowledgeBaseSortTypeID"] = CheckValue("KnowledgeBaseSortTypeID", value); }
    }
    
    public bool EnableTicketNameModification
    {
      get { return (bool)Row["EnableTicketNameModification"]; }
      set { Row["EnableTicketNameModification"] = CheckValue("EnableTicketNameModification", value); }
    }
    
    public bool RestrictProductVersions
    {
      get { return (bool)Row["RestrictProductVersions"]; }
      set { Row["RestrictProductVersions"] = CheckValue("RestrictProductVersions", value); }
    }
    
    public bool EnableTicketSeverityModification
    {
      get { return (bool)Row["EnableTicketSeverityModification"]; }
      set { Row["EnableTicketSeverityModification"] = CheckValue("EnableTicketSeverityModification", value); }
    }
    
    public bool EnableTicketSeverity
    {
      get { return (bool)Row["EnableTicketSeverity"]; }
      set { Row["EnableTicketSeverity"] = CheckValue("EnableTicketSeverity", value); }
    }
    
    public bool EnableVideoRecording
    {
      get { return (bool)Row["EnableVideoRecording"]; }
      set { Row["EnableVideoRecording"] = CheckValue("EnableVideoRecording", value); }
    }
    
    public bool EnableScreenRecording
    {
      get { return (bool)Row["EnableScreenRecording"]; }
      set { Row["EnableScreenRecording"] = CheckValue("EnableScreenRecording", value); }
    }
    
    public bool EnableCommunity
    {
      get { return (bool)Row["EnableCommunity"]; }
      set { Row["EnableCommunity"] = CheckValue("EnableCommunity", value); }
    }
    
    public bool EnableChat
    {
      get { return (bool)Row["EnableChat"]; }
      set { Row["EnableChat"] = CheckValue("EnableChat", value); }
    }
    
    public bool EnableCustomerProductAssociation
    {
      get { return (bool)Row["EnableCustomerProductAssociation"]; }
      set { Row["EnableCustomerProductAssociation"] = CheckValue("EnableCustomerProductAssociation", value); }
    }
    
    public bool EnableTicketProductVersionSelection
    {
      get { return (bool)Row["EnableTicketProductVersionSelection"]; }
      set { Row["EnableTicketProductVersionSelection"] = CheckValue("EnableTicketProductVersionSelection", value); }
    }
    
    public bool EnableTicketProductSelection
    {
      get { return (bool)Row["EnableTicketProductSelection"]; }
      set { Row["EnableTicketProductSelection"] = CheckValue("EnableTicketProductSelection", value); }
    }
    
    public bool EnableTicketGroupSelection
    {
      get { return (bool)Row["EnableTicketGroupSelection"]; }
      set { Row["EnableTicketGroupSelection"] = CheckValue("EnableTicketGroupSelection", value); }
    }
    
    public bool EnableWiki
    {
      get { return (bool)Row["EnableWiki"]; }
      set { Row["EnableWiki"] = CheckValue("EnableWiki", value); }
    }
    
    public bool EnableOrganizationTickets
    {
      get { return (bool)Row["EnableOrganizationTickets"]; }
      set { Row["EnableOrganizationTickets"] = CheckValue("EnableOrganizationTickets", value); }
    }
    
    public bool EnableMyTickets
    {
      get { return (bool)Row["EnableMyTickets"]; }
      set { Row["EnableMyTickets"] = CheckValue("EnableMyTickets", value); }
    }
    
    public bool EnableTicketCreation
    {
      get { return (bool)Row["EnableTicketCreation"]; }
      set { Row["EnableTicketCreation"] = CheckValue("EnableTicketCreation", value); }
    }
    
    public bool EnableProducts
    {
      get { return (bool)Row["EnableProducts"]; }
      set { Row["EnableProducts"] = CheckValue("EnableProducts", value); }
    }
    
    public bool EnableKnowledgeBase
    {
      get { return (bool)Row["EnableKnowledgeBase"]; }
      set { Row["EnableKnowledgeBase"] = CheckValue("EnableKnowledgeBase", value); }
    }
    
    public int CustomerHubID
    {
      get { return (int)Row["CustomerHubID"]; }
      set { Row["CustomerHubID"] = CheckValue("CustomerHubID", value); }
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
    

    #endregion
    
    
  }

  public partial class CustomerHubFeatureSettings : BaseCollection, IEnumerable<CustomerHubFeatureSetting>
  {
    public CustomerHubFeatureSettings(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CustomerHubFeatureSettings"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CustomerHubFeatureSettingID"; }
    }



    public CustomerHubFeatureSetting this[int index]
    {
      get { return new CustomerHubFeatureSetting(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CustomerHubFeatureSetting customerHubFeatureSetting);
    partial void AfterRowInsert(CustomerHubFeatureSetting customerHubFeatureSetting);
    partial void BeforeRowEdit(CustomerHubFeatureSetting customerHubFeatureSetting);
    partial void AfterRowEdit(CustomerHubFeatureSetting customerHubFeatureSetting);
    partial void BeforeRowDelete(int customerHubFeatureSettingID);
    partial void AfterRowDelete(int customerHubFeatureSettingID);    

    partial void BeforeDBDelete(int customerHubFeatureSettingID);
    partial void AfterDBDelete(int customerHubFeatureSettingID);    

    #endregion

    #region Public Methods

    public CustomerHubFeatureSettingProxy[] GetCustomerHubFeatureSettingProxies()
    {
      List<CustomerHubFeatureSettingProxy> list = new List<CustomerHubFeatureSettingProxy>();

      foreach (CustomerHubFeatureSetting item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int customerHubFeatureSettingID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomerHubFeatureSettings] WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID);";
        deleteCommand.Parameters.Add("CustomerHubFeatureSettingID", SqlDbType.Int);
        deleteCommand.Parameters["CustomerHubFeatureSettingID"].Value = customerHubFeatureSettingID;

        BeforeDBDelete(customerHubFeatureSettingID);
        BeforeRowDelete(customerHubFeatureSettingID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(customerHubFeatureSettingID);
        AfterDBDelete(customerHubFeatureSettingID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CustomerHubFeatureSettingsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CustomerHubFeatureSettings] SET     [CustomerHubID] = @CustomerHubID,    [EnableKnowledgeBase] = @EnableKnowledgeBase,    [EnableProducts] = @EnableProducts,    [EnableTicketCreation] = @EnableTicketCreation,    [EnableMyTickets] = @EnableMyTickets,    [EnableOrganizationTickets] = @EnableOrganizationTickets,    [EnableWiki] = @EnableWiki,    [EnableTicketGroupSelection] = @EnableTicketGroupSelection,    [EnableTicketProductSelection] = @EnableTicketProductSelection,    [EnableTicketProductVersionSelection] = @EnableTicketProductVersionSelection,    [DefaultTicketTypeID] = @DefaultTicketTypeID,    [DefaultGroupTypeID] = @DefaultGroupTypeID,    [EnableCustomerProductAssociation] = @EnableCustomerProductAssociation,    [EnableChat] = @EnableChat,    [EnableCommunity] = @EnableCommunity,    [EnableScreenRecording] = @EnableScreenRecording,    [EnableVideoRecording] = @EnableVideoRecording,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [EnableTicketSeverity] = @EnableTicketSeverity,    [EnableTicketSeverityModification] = @EnableTicketSeverityModification,    [RestrictProductVersions] = @RestrictProductVersions,    [EnableTicketNameModification] = @EnableTicketNameModification,    [KnowledgeBaseSortTypeID] = @KnowledgeBaseSortTypeID,    [CommunitySortTypeID] = @CommunitySortTypeID,    [EnableAnonymousProductAssociation] = @EnableAnonymousProductAssociation,    [EnableCustomerSpecificKB] = @EnableCustomerSpecificKB,    [EnableCustomFieldModification] = @EnableCustomFieldModification,    [EnableProductFamilyFiltering] = @EnableProductFamilyFiltering,    [ChatGroupID] = @ChatGroupID  WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID);";

		
		tempParameter = updateCommand.Parameters.Add("CustomerHubFeatureSettingID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CustomerHubID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableKnowledgeBase", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableProducts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableTicketCreation", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableMyTickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableOrganizationTickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableWiki", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableTicketGroupSelection", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableTicketProductSelection", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableTicketProductVersionSelection", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DefaultTicketTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DefaultGroupTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableCustomerProductAssociation", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableChat", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableCommunity", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableScreenRecording", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableVideoRecording", SqlDbType.Bit, 1);
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
		
		tempParameter = updateCommand.Parameters.Add("EnableTicketSeverity", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableTicketSeverityModification", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("RestrictProductVersions", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableTicketNameModification", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("KnowledgeBaseSortTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CommunitySortTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableAnonymousProductAssociation", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableCustomerSpecificKB", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableCustomFieldModification", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableProductFamilyFiltering", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ChatGroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CustomerHubFeatureSettings] (    [CustomerHubID],    [EnableKnowledgeBase],    [EnableProducts],    [EnableTicketCreation],    [EnableMyTickets],    [EnableOrganizationTickets],    [EnableWiki],    [EnableTicketGroupSelection],    [EnableTicketProductSelection],    [EnableTicketProductVersionSelection],    [DefaultTicketTypeID],    [DefaultGroupTypeID],    [EnableCustomerProductAssociation],    [EnableChat],    [EnableCommunity],    [EnableScreenRecording],    [EnableVideoRecording],    [DateModified],    [ModifierID],    [EnableTicketSeverity],    [EnableTicketSeverityModification],    [RestrictProductVersions],    [EnableTicketNameModification],    [KnowledgeBaseSortTypeID],    [CommunitySortTypeID],    [EnableAnonymousProductAssociation],    [EnableCustomerSpecificKB],    [EnableCustomFieldModification],    [EnableProductFamilyFiltering],    [ChatGroupID]) VALUES ( @CustomerHubID, @EnableKnowledgeBase, @EnableProducts, @EnableTicketCreation, @EnableMyTickets, @EnableOrganizationTickets, @EnableWiki, @EnableTicketGroupSelection, @EnableTicketProductSelection, @EnableTicketProductVersionSelection, @DefaultTicketTypeID, @DefaultGroupTypeID, @EnableCustomerProductAssociation, @EnableChat, @EnableCommunity, @EnableScreenRecording, @EnableVideoRecording, @DateModified, @ModifierID, @EnableTicketSeverity, @EnableTicketSeverityModification, @RestrictProductVersions, @EnableTicketNameModification, @KnowledgeBaseSortTypeID, @CommunitySortTypeID, @EnableAnonymousProductAssociation, @EnableCustomerSpecificKB, @EnableCustomFieldModification, @EnableProductFamilyFiltering, @ChatGroupID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ChatGroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableProductFamilyFiltering", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableCustomFieldModification", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableCustomerSpecificKB", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableAnonymousProductAssociation", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CommunitySortTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("KnowledgeBaseSortTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableTicketNameModification", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RestrictProductVersions", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableTicketSeverityModification", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableTicketSeverity", SqlDbType.Bit, 1);
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
		
		tempParameter = insertCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableVideoRecording", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableScreenRecording", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableCommunity", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableChat", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableCustomerProductAssociation", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DefaultGroupTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DefaultTicketTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableTicketProductVersionSelection", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableTicketProductSelection", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableTicketGroupSelection", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableWiki", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableOrganizationTickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableMyTickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableTicketCreation", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableProducts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableKnowledgeBase", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CustomerHubID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomerHubFeatureSettings] WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID);";
		deleteCommand.Parameters.Add("CustomerHubFeatureSettingID", SqlDbType.Int);

		try
		{
		  foreach (CustomerHubFeatureSetting customerHubFeatureSetting in this)
		  {
			if (customerHubFeatureSetting.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(customerHubFeatureSetting);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = customerHubFeatureSetting.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CustomerHubFeatureSettingID"].AutoIncrement = false;
			  Table.Columns["CustomerHubFeatureSettingID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				customerHubFeatureSetting.Row["CustomerHubFeatureSettingID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(customerHubFeatureSetting);
			}
			else if (customerHubFeatureSetting.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(customerHubFeatureSetting);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = customerHubFeatureSetting.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(customerHubFeatureSetting);
			}
			else if (customerHubFeatureSetting.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)customerHubFeatureSetting.Row["CustomerHubFeatureSettingID", DataRowVersion.Original];
			  deleteCommand.Parameters["CustomerHubFeatureSettingID"].Value = id;
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

      foreach (CustomerHubFeatureSetting customerHubFeatureSetting in this)
      {
        if (customerHubFeatureSetting.Row.Table.Columns.Contains("CreatorID") && (int)customerHubFeatureSetting.Row["CreatorID"] == 0) customerHubFeatureSetting.Row["CreatorID"] = LoginUser.UserID;
        if (customerHubFeatureSetting.Row.Table.Columns.Contains("ModifierID")) customerHubFeatureSetting.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CustomerHubFeatureSetting FindByCustomerHubFeatureSettingID(int customerHubFeatureSettingID)
    {
      foreach (CustomerHubFeatureSetting customerHubFeatureSetting in this)
      {
        if (customerHubFeatureSetting.CustomerHubFeatureSettingID == customerHubFeatureSettingID)
        {
          return customerHubFeatureSetting;
        }
      }
      return null;
    }

    public virtual CustomerHubFeatureSetting AddNewCustomerHubFeatureSetting()
    {
      if (Table.Columns.Count < 1) LoadColumns("CustomerHubFeatureSettings");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CustomerHubFeatureSetting(row, this);
    }
    
    public virtual void LoadByCustomerHubFeatureSettingID(int customerHubFeatureSettingID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CustomerHubFeatureSettingID], [CustomerHubID], [EnableKnowledgeBase], [EnableProducts], [EnableTicketCreation], [EnableMyTickets], [EnableOrganizationTickets], [EnableWiki], [EnableTicketGroupSelection], [EnableTicketProductSelection], [EnableTicketProductVersionSelection], [DefaultTicketTypeID], [DefaultGroupTypeID], [EnableCustomerProductAssociation], [EnableChat], [EnableCommunity], [EnableScreenRecording], [EnableVideoRecording], [DateModified], [ModifierID], [EnableTicketSeverity], [EnableTicketSeverityModification], [RestrictProductVersions], [EnableTicketNameModification], [KnowledgeBaseSortTypeID], [CommunitySortTypeID], [EnableAnonymousProductAssociation], [EnableCustomerSpecificKB], [EnableCustomFieldModification], [EnableProductFamilyFiltering], [ChatGroupID] FROM [dbo].[CustomerHubFeatureSettings] WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CustomerHubFeatureSettingID", customerHubFeatureSettingID);
        Fill(command);
      }
    }
    
    public static CustomerHubFeatureSetting GetCustomerHubFeatureSetting(LoginUser loginUser, int customerHubFeatureSettingID)
    {
      CustomerHubFeatureSettings customerHubFeatureSettings = new CustomerHubFeatureSettings(loginUser);
      customerHubFeatureSettings.LoadByCustomerHubFeatureSettingID(customerHubFeatureSettingID);
      if (customerHubFeatureSettings.IsEmpty)
        return null;
      else
        return customerHubFeatureSettings[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CustomerHubFeatureSetting> Members

    public IEnumerator<CustomerHubFeatureSetting> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CustomerHubFeatureSetting(row, this);
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
