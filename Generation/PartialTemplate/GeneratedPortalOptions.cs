using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class PortalOption : BaseItem
  {
    private PortalOptions _portalOptions;
    
    public PortalOption(DataRow row, PortalOptions portalOptions): base(row, portalOptions)
    {
      _portalOptions = portalOptions;
    }
	
    #region Properties
    
    public PortalOptions Collection
    {
      get { return _portalOptions; }
    }
        
    
    
    

    
    public string PortalHTMLHeader
    {
      get { return Row["PortalHTMLHeader"] != DBNull.Value ? (string)Row["PortalHTMLHeader"] : null; }
      set { Row["PortalHTMLHeader"] = CheckValue("PortalHTMLHeader", value); }
    }
    
    public string PortalHTMLFooter
    {
      get { return Row["PortalHTMLFooter"] != DBNull.Value ? (string)Row["PortalHTMLFooter"] : null; }
      set { Row["PortalHTMLFooter"] = CheckValue("PortalHTMLFooter", value); }
    }
    
    public bool? UseRecaptcha
    {
      get { return Row["UseRecaptcha"] != DBNull.Value ? (bool?)Row["UseRecaptcha"] : null; }
      set { Row["UseRecaptcha"] = CheckValue("UseRecaptcha", value); }
    }
    
    public string FontFamily
    {
      get { return Row["FontFamily"] != DBNull.Value ? (string)Row["FontFamily"] : null; }
      set { Row["FontFamily"] = CheckValue("FontFamily", value); }
    }
    
    public string FontColor
    {
      get { return Row["FontColor"] != DBNull.Value ? (string)Row["FontColor"] : null; }
      set { Row["FontColor"] = CheckValue("FontColor", value); }
    }
    
    public string PageBackgroundColor
    {
      get { return Row["PageBackgroundColor"] != DBNull.Value ? (string)Row["PageBackgroundColor"] : null; }
      set { Row["PageBackgroundColor"] = CheckValue("PageBackgroundColor", value); }
    }
    
    public bool? UseCompanyInBasic
    {
      get { return Row["UseCompanyInBasic"] != DBNull.Value ? (bool?)Row["UseCompanyInBasic"] : null; }
      set { Row["UseCompanyInBasic"] = CheckValue("UseCompanyInBasic", value); }
    }
    
    public bool? CompanyRequiredInBasic
    {
      get { return Row["CompanyRequiredInBasic"] != DBNull.Value ? (bool?)Row["CompanyRequiredInBasic"] : null; }
      set { Row["CompanyRequiredInBasic"] = CheckValue("CompanyRequiredInBasic", value); }
    }
    
    public bool? HideUserAssignedTo
    {
      get { return Row["HideUserAssignedTo"] != DBNull.Value ? (bool?)Row["HideUserAssignedTo"] : null; }
      set { Row["HideUserAssignedTo"] = CheckValue("HideUserAssignedTo", value); }
    }
    
    public bool? HideGroupAssignedTo
    {
      get { return Row["HideGroupAssignedTo"] != DBNull.Value ? (bool?)Row["HideGroupAssignedTo"] : null; }
      set { Row["HideGroupAssignedTo"] = CheckValue("HideGroupAssignedTo", value); }
    }
    
    public int? BasicPortalColumnWidth
    {
      get { return Row["BasicPortalColumnWidth"] != DBNull.Value ? (int?)Row["BasicPortalColumnWidth"] : null; }
      set { Row["BasicPortalColumnWidth"] = CheckValue("BasicPortalColumnWidth", value); }
    }
    
    public bool? DisplayGroups
    {
      get { return Row["DisplayGroups"] != DBNull.Value ? (bool?)Row["DisplayGroups"] : null; }
      set { Row["DisplayGroups"] = CheckValue("DisplayGroups", value); }
    }
    
    public string PortalName
    {
      get { return Row["PortalName"] != DBNull.Value ? (string)Row["PortalName"] : null; }
      set { Row["PortalName"] = CheckValue("PortalName", value); }
    }
    
    public bool? KBAccess
    {
      get { return Row["KBAccess"] != DBNull.Value ? (bool?)Row["KBAccess"] : null; }
      set { Row["KBAccess"] = CheckValue("KBAccess", value); }
    }
    
    public bool? DisplayProducts
    {
      get { return Row["DisplayProducts"] != DBNull.Value ? (bool?)Row["DisplayProducts"] : null; }
      set { Row["DisplayProducts"] = CheckValue("DisplayProducts", value); }
    }
    
    public bool? HonorSupportExpiration
    {
      get { return Row["HonorSupportExpiration"] != DBNull.Value ? (bool?)Row["HonorSupportExpiration"] : null; }
      set { Row["HonorSupportExpiration"] = CheckValue("HonorSupportExpiration", value); }
    }
    
    public bool? HideCloseButton
    {
      get { return Row["HideCloseButton"] != DBNull.Value ? (bool?)Row["HideCloseButton"] : null; }
      set { Row["HideCloseButton"] = CheckValue("HideCloseButton", value); }
    }
    
    public string Theme
    {
      get { return Row["Theme"] != DBNull.Value ? (string)Row["Theme"] : null; }
      set { Row["Theme"] = CheckValue("Theme", value); }
    }
    
    public int? AdvPortalWidth
    {
      get { return Row["AdvPortalWidth"] != DBNull.Value ? (int?)Row["AdvPortalWidth"] : null; }
      set { Row["AdvPortalWidth"] = CheckValue("AdvPortalWidth", value); }
    }
    
    public string BasicPortalDirections
    {
      get { return Row["BasicPortalDirections"] != DBNull.Value ? (string)Row["BasicPortalDirections"] : null; }
      set { Row["BasicPortalDirections"] = CheckValue("BasicPortalDirections", value); }
    }
    
    public bool? DeflectionEnabled
    {
      get { return Row["DeflectionEnabled"] != DBNull.Value ? (bool?)Row["DeflectionEnabled"] : null; }
      set { Row["DeflectionEnabled"] = CheckValue("DeflectionEnabled", value); }
    }
    
    public bool? DisplayForum
    {
      get { return Row["DisplayForum"] != DBNull.Value ? (bool?)Row["DisplayForum"] : null; }
      set { Row["DisplayForum"] = CheckValue("DisplayForum", value); }
    }
    
    public string LandingPageHtml
    {
      get { return Row["LandingPageHtml"] != DBNull.Value ? (string)Row["LandingPageHtml"] : null; }
      set { Row["LandingPageHtml"] = CheckValue("LandingPageHtml", value); }
    }
    
    public string PublicLandingPageHeader
    {
      get { return Row["PublicLandingPageHeader"] != DBNull.Value ? (string)Row["PublicLandingPageHeader"] : null; }
      set { Row["PublicLandingPageHeader"] = CheckValue("PublicLandingPageHeader", value); }
    }
    
    public string PublicLandingPageBody
    {
      get { return Row["PublicLandingPageBody"] != DBNull.Value ? (string)Row["PublicLandingPageBody"] : null; }
      set { Row["PublicLandingPageBody"] = CheckValue("PublicLandingPageBody", value); }
    }
    
    public int? RequestType
    {
      get { return Row["RequestType"] != DBNull.Value ? (int?)Row["RequestType"] : null; }
      set { Row["RequestType"] = CheckValue("RequestType", value); }
    }
    
    public int? RequestGroup
    {
      get { return Row["RequestGroup"] != DBNull.Value ? (int?)Row["RequestGroup"] : null; }
      set { Row["RequestGroup"] = CheckValue("RequestGroup", value); }
    }
    

    
    public bool EnableVideoRecording
    {
      get { return (bool)Row["EnableVideoRecording"]; }
      set { Row["EnableVideoRecording"] = CheckValue("EnableVideoRecording", value); }
    }
    
    public bool RestrictProductVersion
    {
      get { return (bool)Row["RestrictProductVersion"]; }
      set { Row["RestrictProductVersion"] = CheckValue("RestrictProductVersion", value); }
    }
    
    public bool DisplayLogout
    {
      get { return (bool)Row["DisplayLogout"]; }
      set { Row["DisplayLogout"] = CheckValue("DisplayLogout", value); }
    }
    
    public bool DisplaySettings
    {
      get { return (bool)Row["DisplaySettings"]; }
      set { Row["DisplaySettings"] = CheckValue("DisplaySettings", value); }
    }
    
    public bool EnableSaExpiration
    {
      get { return (bool)Row["EnableSaExpiration"]; }
      set { Row["EnableSaExpiration"] = CheckValue("EnableSaExpiration", value); }
    }
    
    public bool DisablePublicMyTickets
    {
      get { return (bool)Row["DisablePublicMyTickets"]; }
      set { Row["DisablePublicMyTickets"] = CheckValue("DisablePublicMyTickets", value); }
    }
    
    public bool RequestAccess
    {
      get { return (bool)Row["RequestAccess"]; }
      set { Row["RequestAccess"] = CheckValue("RequestAccess", value); }
    }
    
    public bool AutoRegister
    {
      get { return (bool)Row["AutoRegister"]; }
      set { Row["AutoRegister"] = CheckValue("AutoRegister", value); }
    }
    
    public string TermsAndConditions
    {
      get { return (string)Row["TermsAndConditions"]; }
      set { Row["TermsAndConditions"] = CheckValue("TermsAndConditions", value); }
    }
    
    public bool DisplayTandC
    {
      get { return (bool)Row["DisplayTandC"]; }
      set { Row["DisplayTandC"] = CheckValue("DisplayTandC", value); }
    }
    
    public bool DisplayAdvArticles
    {
      get { return (bool)Row["DisplayAdvArticles"]; }
      set { Row["DisplayAdvArticles"] = CheckValue("DisplayAdvArticles", value); }
    }
    
    public bool TwoColumnFields
    {
      get { return (bool)Row["TwoColumnFields"]; }
      set { Row["TwoColumnFields"] = CheckValue("TwoColumnFields", value); }
    }
    
    public bool EnableScreenr
    {
      get { return (bool)Row["EnableScreenr"]; }
      set { Row["EnableScreenr"] = CheckValue("EnableScreenr", value); }
    }
    
    public bool DisplayLandingPage
    {
      get { return (bool)Row["DisplayLandingPage"]; }
      set { Row["DisplayLandingPage"] = CheckValue("DisplayLandingPage", value); }
    }
    
    public bool DisplayProductVersion
    {
      get { return (bool)Row["DisplayProductVersion"]; }
      set { Row["DisplayProductVersion"] = CheckValue("DisplayProductVersion", value); }
    }
    
    public bool DisplayAdvKB
    {
      get { return (bool)Row["DisplayAdvKB"]; }
      set { Row["DisplayAdvKB"] = CheckValue("DisplayAdvKB", value); }
    }
    
    public bool DisplayAdvProducts
    {
      get { return (bool)Row["DisplayAdvProducts"]; }
      set { Row["DisplayAdvProducts"] = CheckValue("DisplayAdvProducts", value); }
    }
    
    public bool DisplayPortalPhone
    {
      get { return (bool)Row["DisplayPortalPhone"]; }
      set { Row["DisplayPortalPhone"] = CheckValue("DisplayPortalPhone", value); }
    }
    
    public bool DisplayFooter
    {
      get { return (bool)Row["DisplayFooter"]; }
      set { Row["DisplayFooter"] = CheckValue("DisplayFooter", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class PortalOptions : BaseCollection, IEnumerable<PortalOption>
  {
    public PortalOptions(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "PortalOptions"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "OrganizationID"; }
    }



    public PortalOption this[int index]
    {
      get { return new PortalOption(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(PortalOption portalOption);
    partial void AfterRowInsert(PortalOption portalOption);
    partial void BeforeRowEdit(PortalOption portalOption);
    partial void AfterRowEdit(PortalOption portalOption);
    partial void BeforeRowDelete(int organizationID);
    partial void AfterRowDelete(int organizationID);    

    partial void BeforeDBDelete(int organizationID);
    partial void AfterDBDelete(int organizationID);    

    #endregion

    #region Public Methods

    public PortalOptionProxy[] GetPortalOptionProxies()
    {
      List<PortalOptionProxy> list = new List<PortalOptionProxy>();

      foreach (PortalOption item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int organizationID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[PortalOptions] WHERE ([OrganizationID] = @OrganizationID);";
        deleteCommand.Parameters.Add("OrganizationID", SqlDbType.Int);
        deleteCommand.Parameters["OrganizationID"].Value = organizationID;

        BeforeDBDelete(organizationID);
        BeforeRowDelete(organizationID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(organizationID);
        AfterDBDelete(organizationID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("PortalOptionsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[PortalOptions] SET     [PortalHTMLHeader] = @PortalHTMLHeader,    [PortalHTMLFooter] = @PortalHTMLFooter,    [UseRecaptcha] = @UseRecaptcha,    [FontFamily] = @FontFamily,    [FontColor] = @FontColor,    [PageBackgroundColor] = @PageBackgroundColor,    [UseCompanyInBasic] = @UseCompanyInBasic,    [CompanyRequiredInBasic] = @CompanyRequiredInBasic,    [HideUserAssignedTo] = @HideUserAssignedTo,    [HideGroupAssignedTo] = @HideGroupAssignedTo,    [BasicPortalColumnWidth] = @BasicPortalColumnWidth,    [DisplayGroups] = @DisplayGroups,    [PortalName] = @PortalName,    [KBAccess] = @KBAccess,    [DisplayProducts] = @DisplayProducts,    [HonorSupportExpiration] = @HonorSupportExpiration,    [HideCloseButton] = @HideCloseButton,    [Theme] = @Theme,    [AdvPortalWidth] = @AdvPortalWidth,    [BasicPortalDirections] = @BasicPortalDirections,    [DeflectionEnabled] = @DeflectionEnabled,    [DisplayForum] = @DisplayForum,    [DisplayFooter] = @DisplayFooter,    [DisplayPortalPhone] = @DisplayPortalPhone,    [DisplayAdvProducts] = @DisplayAdvProducts,    [DisplayAdvKB] = @DisplayAdvKB,    [DisplayProductVersion] = @DisplayProductVersion,    [LandingPageHtml] = @LandingPageHtml,    [DisplayLandingPage] = @DisplayLandingPage,    [EnableScreenr] = @EnableScreenr,    [PublicLandingPageHeader] = @PublicLandingPageHeader,    [PublicLandingPageBody] = @PublicLandingPageBody,    [TwoColumnFields] = @TwoColumnFields,    [DisplayAdvArticles] = @DisplayAdvArticles,    [DisplayTandC] = @DisplayTandC,    [TermsAndConditions] = @TermsAndConditions,    [RequestType] = @RequestType,    [RequestGroup] = @RequestGroup,    [AutoRegister] = @AutoRegister,    [RequestAccess] = @RequestAccess,    [DisablePublicMyTickets] = @DisablePublicMyTickets,    [EnableSaExpiration] = @EnableSaExpiration,    [DisplaySettings] = @DisplaySettings,    [DisplayLogout] = @DisplayLogout,    [RestrictProductVersion] = @RestrictProductVersion,    [EnableVideoRecording] = @EnableVideoRecording  WHERE ([OrganizationID] = @OrganizationID);";

		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("PortalHTMLHeader", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PortalHTMLFooter", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UseRecaptcha", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("FontFamily", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("FontColor", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PageBackgroundColor", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UseCompanyInBasic", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CompanyRequiredInBasic", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("HideUserAssignedTo", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("HideGroupAssignedTo", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("BasicPortalColumnWidth", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayGroups", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PortalName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("KBAccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayProducts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("HonorSupportExpiration", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("HideCloseButton", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Theme", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("AdvPortalWidth", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("BasicPortalDirections", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DeflectionEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayForum", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayFooter", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayPortalPhone", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayAdvProducts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayAdvKB", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayProductVersion", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LandingPageHtml", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayLandingPage", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableScreenr", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PublicLandingPageHeader", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PublicLandingPageBody", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TwoColumnFields", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayAdvArticles", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayTandC", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TermsAndConditions", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("RequestType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RequestGroup", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("AutoRegister", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("RequestAccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisablePublicMyTickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableSaExpiration", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplaySettings", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayLogout", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("RestrictProductVersion", SqlDbType.Bit, 1);
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
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[PortalOptions] (    [OrganizationID],    [PortalHTMLHeader],    [PortalHTMLFooter],    [UseRecaptcha],    [FontFamily],    [FontColor],    [PageBackgroundColor],    [UseCompanyInBasic],    [CompanyRequiredInBasic],    [HideUserAssignedTo],    [HideGroupAssignedTo],    [BasicPortalColumnWidth],    [DisplayGroups],    [PortalName],    [KBAccess],    [DisplayProducts],    [HonorSupportExpiration],    [HideCloseButton],    [Theme],    [AdvPortalWidth],    [BasicPortalDirections],    [DeflectionEnabled],    [DisplayForum],    [DisplayFooter],    [DisplayPortalPhone],    [DisplayAdvProducts],    [DisplayAdvKB],    [DisplayProductVersion],    [LandingPageHtml],    [DisplayLandingPage],    [EnableScreenr],    [PublicLandingPageHeader],    [PublicLandingPageBody],    [TwoColumnFields],    [DisplayAdvArticles],    [DisplayTandC],    [TermsAndConditions],    [RequestType],    [RequestGroup],    [AutoRegister],    [RequestAccess],    [DisablePublicMyTickets],    [EnableSaExpiration],    [DisplaySettings],    [DisplayLogout],    [RestrictProductVersion],    [EnableVideoRecording]) VALUES ( @OrganizationID, @PortalHTMLHeader, @PortalHTMLFooter, @UseRecaptcha, @FontFamily, @FontColor, @PageBackgroundColor, @UseCompanyInBasic, @CompanyRequiredInBasic, @HideUserAssignedTo, @HideGroupAssignedTo, @BasicPortalColumnWidth, @DisplayGroups, @PortalName, @KBAccess, @DisplayProducts, @HonorSupportExpiration, @HideCloseButton, @Theme, @AdvPortalWidth, @BasicPortalDirections, @DeflectionEnabled, @DisplayForum, @DisplayFooter, @DisplayPortalPhone, @DisplayAdvProducts, @DisplayAdvKB, @DisplayProductVersion, @LandingPageHtml, @DisplayLandingPage, @EnableScreenr, @PublicLandingPageHeader, @PublicLandingPageBody, @TwoColumnFields, @DisplayAdvArticles, @DisplayTandC, @TermsAndConditions, @RequestType, @RequestGroup, @AutoRegister, @RequestAccess, @DisablePublicMyTickets, @EnableSaExpiration, @DisplaySettings, @DisplayLogout, @RestrictProductVersion, @EnableVideoRecording); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("EnableVideoRecording", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RestrictProductVersion", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayLogout", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplaySettings", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableSaExpiration", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisablePublicMyTickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RequestAccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("AutoRegister", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RequestGroup", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("RequestType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TermsAndConditions", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayTandC", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayAdvArticles", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TwoColumnFields", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PublicLandingPageBody", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PublicLandingPageHeader", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableScreenr", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayLandingPage", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LandingPageHtml", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayProductVersion", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayAdvKB", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayAdvProducts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayPortalPhone", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayFooter", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayForum", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DeflectionEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("BasicPortalDirections", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("AdvPortalWidth", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Theme", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("HideCloseButton", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("HonorSupportExpiration", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayProducts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("KBAccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PortalName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayGroups", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("BasicPortalColumnWidth", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("HideGroupAssignedTo", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("HideUserAssignedTo", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CompanyRequiredInBasic", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UseCompanyInBasic", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PageBackgroundColor", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("FontColor", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("FontFamily", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UseRecaptcha", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PortalHTMLFooter", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PortalHTMLHeader", SqlDbType.VarChar, -1);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[PortalOptions] WHERE ([OrganizationID] = @OrganizationID);";
		deleteCommand.Parameters.Add("OrganizationID", SqlDbType.Int);

		try
		{
		  foreach (PortalOption portalOption in this)
		  {
			if (portalOption.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(portalOption);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = portalOption.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["OrganizationID"].AutoIncrement = false;
			  Table.Columns["OrganizationID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				portalOption.Row["OrganizationID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(portalOption);
			}
			else if (portalOption.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(portalOption);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = portalOption.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(portalOption);
			}
			else if (portalOption.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)portalOption.Row["OrganizationID", DataRowVersion.Original];
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

      foreach (PortalOption portalOption in this)
      {
        if (portalOption.Row.Table.Columns.Contains("CreatorID") && (int)portalOption.Row["CreatorID"] == 0) portalOption.Row["CreatorID"] = LoginUser.UserID;
        if (portalOption.Row.Table.Columns.Contains("ModifierID")) portalOption.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public PortalOption FindByOrganizationID(int organizationID)
    {
      foreach (PortalOption portalOption in this)
      {
        if (portalOption.OrganizationID == organizationID)
        {
          return portalOption;
        }
      }
      return null;
    }

    public virtual PortalOption AddNewPortalOption()
    {
      if (Table.Columns.Count < 1) LoadColumns("PortalOptions");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new PortalOption(row, this);
    }
    
    public virtual void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [OrganizationID], [PortalHTMLHeader], [PortalHTMLFooter], [UseRecaptcha], [FontFamily], [FontColor], [PageBackgroundColor], [UseCompanyInBasic], [CompanyRequiredInBasic], [HideUserAssignedTo], [HideGroupAssignedTo], [BasicPortalColumnWidth], [DisplayGroups], [PortalName], [KBAccess], [DisplayProducts], [HonorSupportExpiration], [HideCloseButton], [Theme], [AdvPortalWidth], [BasicPortalDirections], [DeflectionEnabled], [DisplayForum], [DisplayFooter], [DisplayPortalPhone], [DisplayAdvProducts], [DisplayAdvKB], [DisplayProductVersion], [LandingPageHtml], [DisplayLandingPage], [EnableScreenr], [PublicLandingPageHeader], [PublicLandingPageBody], [TwoColumnFields], [DisplayAdvArticles], [DisplayTandC], [TermsAndConditions], [RequestType], [RequestGroup], [AutoRegister], [RequestAccess], [DisablePublicMyTickets], [EnableSaExpiration], [DisplaySettings], [DisplayLogout], [RestrictProductVersion], [EnableVideoRecording] FROM [dbo].[PortalOptions] WHERE ([OrganizationID] = @OrganizationID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }
    
    public static PortalOption GetPortalOption(LoginUser loginUser, int organizationID)
    {
      PortalOptions portalOptions = new PortalOptions(loginUser);
      portalOptions.LoadByOrganizationID(organizationID);
      if (portalOptions.IsEmpty)
        return null;
      else
        return portalOptions[0];
    }
    
    
    

    #endregion

    #region IEnumerable<PortalOption> Members

    public IEnumerator<PortalOption> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new PortalOption(row, this);
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
