using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TeamSupport.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace DataRecovery
{
  public partial class Form1 : Form
  {

    public class ComboboxItem
    {
      public ComboboxItem(object value, string text)
      {
        this.Text = text;
        this.Value = value;
      }

      public string Text { get; set; }
      public object Value { get; set; }

      public override string ToString()
      {
        return Text;
      }
    }


    private Logs _logs;
    private string _importID;
    private Users _usersAndContacts;
    private bool _exceptionOcurred;
    private Products _badProducts;
    private ProductVersions _badProductVersions;
    private Products _goodProducts;
    private ProductVersions _goodProductVersions;
    private Groups _badGroups;
    private Groups _goodGroups;
    private Users _badUsers;
    private Users _goodUsers;
    private TicketTypes _badTicketTypes;
    private TicketTypes _goodTicketTypes;
    private TicketStatuses _badTicketStatuses;
    private TicketStatuses _goodTicketStatuses;
    private TicketSeverities _badTicketSeverities;
    private TicketSeverities _goodTicketSeverities;
    private KnowledgeBaseCategories _goodTicketKBCategories;
    private KnowledgeBaseCategories _badTicketKBCategories;

    public Form1()
    {
      InitializeComponent();
      LoadOrgCombo();

    }

    private void LoadOrgCombo()
    {

      SqlCommand command = new SqlCommand();
      command.CommandText = @"
        SELECT OrganizationID, Name FROM Organizations WHERE ParentID=1 AND IsActive=1 ORDER BY Name";
      DataTable table = SqlExecutor.ExecuteQuery(GetCorrupteLoginUser(), command);
      cmbOrg.BeginUpdate();
      foreach (DataRow row in table.Rows)
      {
        string text = string.Format("{0} ({1})", (string)row[1], ((int)row[0]).ToString());
        cmbOrg.Items.Add(new ComboboxItem((int)row[0], text));
      }
      cmbOrg.EndUpdate();

    }

    private LoginUser GetCorrupteLoginUser()
    {
      return new LoginUser("Data Source=10.42.42.105; Initial Catalog=TeamSupport;Persist Security Info=True;User ID=webuser;Password=3209u@j#*29;Connect Timeout=5000", -5, -1, null);
    }

    private LoginUser GetReviewLoginUser()
    {
      return new LoginUser("Data Source=10.42.42.105; Initial Catalog=TeamSupportTest2;Persist Security Info=True;User ID=webuser;Password=3209u@j#*29;Connect Timeout=5000", -5, -1, null);
    }

    private LoginUser GetPRODUCTIONLoginUser()
    {
      return new LoginUser("Data Source=10.42.42.101; Initial Catalog=TeamSupport;Persist Security Info=True;User ID=webuser;Password=3209u@j#*29;Connect Timeout=5000", -5, -1, null);
    }

    private int GetNextOrg()
    {
      return SqlExecutor.ExecuteInt(GetCorrupteLoginUser(), @"
        SELECT
          TOP 1 OrganizationID 
        FROM
          OrgMoveEvent
        WHERE
          HasExecuted = 0 
        ORDER BY
          Priority
      ");
    }

    private string GetImportID(int orgID)
    {
      SqlCommand command = new SqlCommand();
      command.CommandText = @"
        SELECT
          ImportID
        FROM
          OrgMoveEvent
        WHERE
          OrganizationID = @OrgID";
      command.Parameters.AddWithValue("OrgID", orgID);

      return SqlExecutor.ExecuteScalar(GetCorrupteLoginUser(), command).ToString();

    
    }
    
    private void RollBack(int orgID, LoginUser loginUser)
    {
      _importID = GetImportID(orgID);
      if (!string.IsNullOrEmpty(_importID))
      {

        ExecuteRollback("DELETE Organizations WHERE ImportID = @importID AND ParentID=@orgID", loginUser, orgID);
        ExecuteRollback("DELETE Products WHERE ImportID = @importID AND OrganizationID=@orgID", loginUser, orgID);
        ExecuteRollback(@"
            DELETE FROM Actions 
            WHERE ImportID = @importID 
            AND TicketID IN (
              SELECT TicketID 
              FROM Tickets 
              WHERE 
                OrganizationID=@orgID 
                AND ImportID = @importID 
                AND DateCreated < '2015-09-17 05:56:00')", loginUser, orgID);
        ExecuteRollback(@"
            UPDATE 
              Tickets 
            SET 
              ImportID = '' 
            WHERE
              OrganizationID=@orgID 
              AND DateCreated < '2015-09-17 05:56:00' 
              AND ImportID like @importID", loginUser, orgID);
        ExecuteRollback("DELETE Tickets WHERE ImportID = @importID AND OrganizationID=@orgID AND DateCreated > '2015-09-17 05:56:00'", loginUser, orgID);
        SaveOrgResults(orgID, "RolledBack", _importID, false);

        MessageBox.Show("Rollback Complete");
      }
    }

    private void ExecuteRollback(string commandText, LoginUser loginUser, int orgID)
    {
      SqlCommand rollbackCommand = new SqlCommand(commandText);
      rollbackCommand.CommandTimeout = 5000;
      rollbackCommand.Parameters.AddWithValue("importID", _importID);
      rollbackCommand.Parameters.AddWithValue("orgID", orgID);
      SqlExecutor.ExecuteNonQuery(loginUser, rollbackCommand);
    }

    private void ImportOrg(int orgID, LoginUser loginUser)
    {
      string existingImportID = GetImportID(orgID);
      if (!string.IsNullOrWhiteSpace(existingImportID))
      {
        MessageBox.Show("This company has already been imported");
        return;
      }
      
      try
      {
        _importID = orgID.ToString() + "-" + Guid.NewGuid().ToString();
        _logs = new Logs(orgID.ToString() + " - Org.txt");
        _usersAndContacts = new Users(loginUser);
        _usersAndContacts.LoadContactsAndUsers(orgID, false);

        _badProducts        = new Products(GetCorrupteLoginUser());
        _badProductVersions = new ProductVersions(GetCorrupteLoginUser());
        _badGroups          = new Groups(GetCorrupteLoginUser());
        _badUsers           = new Users(GetCorrupteLoginUser());
        _badTicketTypes     = new TicketTypes(GetCorrupteLoginUser());
        _badTicketStatuses  = new TicketStatuses(GetCorrupteLoginUser());
        _badTicketSeverities = new TicketSeverities(GetCorrupteLoginUser());
        _badTicketKBCategories = new KnowledgeBaseCategories(GetCorrupteLoginUser());
        _badProducts.LoadByOrganizationID(orgID);
        _badProductVersions.LoadByParentOrganizationID(orgID);
        _badGroups.LoadByOrganizationID(orgID);
        _badUsers.LoadByOrganizationID(orgID, false);
        _badTicketTypes.LoadByOrganizationID(orgID);
        _badTicketStatuses.LoadByOrganizationID(orgID);
        _badTicketSeverities.LoadByOrganizationID(orgID);
        _badTicketKBCategories.LoadCategories(orgID);

        _goodProducts         = new Products(loginUser);
        _goodProductVersions  = new ProductVersions(loginUser);
        _goodGroups           = new Groups(loginUser);
        _goodUsers            = new Users(loginUser);
        _goodTicketTypes      = new TicketTypes(loginUser);
        _goodTicketStatuses   = new TicketStatuses(loginUser);
        _goodTicketSeverities = new TicketSeverities(loginUser);
        _goodTicketKBCategories = new KnowledgeBaseCategories(loginUser);

        _goodProducts.LoadByOrganizationID(orgID);
        _goodProductVersions.LoadByParentOrganizationID(orgID);
        _goodGroups.LoadByOrganizationID(orgID);
        _goodUsers.LoadByOrganizationID(orgID, false);
        _goodTicketTypes.LoadByOrganizationID(orgID);
        _goodTicketStatuses.LoadByOrganizationID(orgID);
        _goodTicketSeverities.LoadByOrganizationID(orgID);
        _goodTicketKBCategories.LoadCategories(orgID);


        _exceptionOcurred = false;
        if (cbCompanies.Checked) RecoverCompanies(orgID, loginUser);
        //RecoverContacts(orgID);
        if (cbProducts.Checked) RecoverProducts(orgID, loginUser);
        // RecoverAssets(orgID);
        if (cbOldActions.Checked) RecoverActionsFromOldTickets(orgID, loginUser);
        if (cbTickets.Checked) RecoverTickets(orgID, loginUser);

        if (_exceptionOcurred)
        {
          SaveOrgResults(orgID, "Finished with exceptions", _importID);
        }
        else
        {
          SaveOrgResults(orgID, "Success", _importID);
        }
        SqlExecutor.ExecuteNonQuery(loginUser, "update organizations set LastIndexRebuilt='1/1/2000' where OrganizationID=" + orgID.ToString());
        MessageBox.Show("Import Complete");
      }
      catch (Exception ex)
      {
        SaveOrgResults(orgID, "Failure: " + ex.Message, _importID);
        ExceptionLogs.LogException(GetCorrupteLoginUser(), ex, "recover");
      }


    }

    private void SaveOrgResults(int orgID, string result, string importID, bool hasExectued = true)
    {
      SqlCommand command = new SqlCommand();
      command.CommandText = @"
      UPDATE
        OrgMoveEvent
      SET
        Result = @Result
        , HasExecuted = @HasExecuted 
        , ImportID = @ImportID 
      WHERE
        OrganizationID = @OrganizationID";
      command.Parameters.AddWithValue("Result", result);
      command.Parameters.AddWithValue("HasExecuted", hasExectued);
      command.Parameters.AddWithValue("OrganizationID", orgID);
      command.Parameters.AddWithValue("ImportID", importID);
      SqlExecutor.ExecuteNonQuery(GetCorrupteLoginUser(), command);
    }

    private void RecoverProducts(int orgID, LoginUser loginUser)
    {
      // check corrupt db for different products,if so craete the new products, but do not use ID's
      Products badProducts = new Products(GetCorrupteLoginUser());
      badProducts.LoadByOrganizationID(orgID);

      Products goodProducts = new Products(loginUser);
      goodProducts.LoadByOrganizationID(orgID);

      foreach (Product badProduct in badProducts)
      {
        try
        {
          Product goodProduct = goodProducts.FindByName(badProduct.Name);
          if (goodProduct == null)
          {
            goodProduct = (new Products(loginUser)).AddNewProduct();
            goodProduct.Name = badProduct.Name;
            goodProduct.DateCreated = badProduct.DateCreatedUtc;
            if (badProduct.CreatorID > 0)
            {
              User creator = _usersAndContacts.FindByUserID(badProduct.CreatorID);
              if (creator != null)
              {
                goodProduct.CreatorID = creator.UserID;
              }
              else
              {
                goodProduct.CreatorID = -5;
              }
            }
            else
            {
              goodProduct.CreatorID = badProduct.CreatorID;
            }
            goodProduct.OrganizationID = orgID;
            goodProduct.ImportID = _importID;
            goodProduct.Collection.Save();
          }
        }
        catch (Exception ex)
        {
          _exceptionOcurred = true;
          ExceptionLogs.LogException(GetCorrupteLoginUser(), ex, "recover");
        }
      }
    }

    private void RecoverCompanies(int orgID, LoginUser loginUser)
    {
      Organizations badCompanies = new Organizations(GetCorrupteLoginUser());
      badCompanies.LoadByParentID(orgID, false);

      Organizations goodCompanies = new Organizations(loginUser);
      goodCompanies.LoadByParentID(orgID, false);

      foreach (Organization badCompany in badCompanies)
      {
        try
        {
          Organization goodCompany = goodCompanies.FindByName(badCompany.Name);
          if (goodCompany == null)
          {
            goodCompany = (new Organizations(loginUser)).AddNewOrganization();
            goodCompany.CopyRowData(badCompany);
            goodCompany.DateCreated = badCompany.DateCreatedUtc;
            goodCompany.DateModified = badCompany.DateModifiedUtc;
            goodCompany.ImportID = _importID;
            goodCompany.ParentID = orgID;
            goodCompany.Collection.Save();
          }
        }
        catch (Exception ex)
        {
          _exceptionOcurred = true;
          ExceptionLogs.LogException(GetCorrupteLoginUser(), ex, "recover");
        }
      }
    }

    //private void RecoverContacts(int orgID)
    //{
    //  // check corrupt db for different products,if so craete the new products, but do not use ID's
    //  Users badContacts = new Users(GetCorrupteLoginUser());
    //  badContacts.LoadContacts(orgID, false);

    //  Users goodContacts = new Users(GetReviewLoginUser());
    //  goodContacts.LoadContacts(orgID, false);

    //  foreach (User badContact in badContacts)
    //  {
    //    User goodContact = goodContacts.FindBy(badCompany.Name);
    //    if (goodCompany == null)
    //    {
    //      goodCompany = (new Organizations(GetReviewLoginUser())).AddNewOrganization();
    //      goodCompany.Name = badCompany.Name;
    //      goodCompany.Description = badCompany.Description;
    //      goodCompany.Website = badCompany.Website;
    //      goodCompany.CompanyDomains = badCompany.CompanyDomains;
    //      goodCompany.SAExpirationDate = badCompany.SAExpirationDate;
    //      goodCompany.SlaLevelID = badCompany.SlaLevelID;
    //      goodCompany.DateCreated = badCompany.DateCreated;
    //      goodCompany.SupportHoursMonth = badCompany.SupportHoursMonth;
    //      goodCompany.IsActive = badCompany.IsActive;
    //      goodCompany.HasPortalAccess = badCompany.HasPortalAccess;
    //      goodCompany.IsApiEnabled = badCompany.IsApiEnabled;
    //      goodCompany.IsApiActive = badCompany.IsApiActive;
    //      goodCompany.InActiveReason = badCompany.InActiveReason;

    //      goodCompany.ExtraStorageUnits = badCompany.ExtraStorageUnits;
    //      goodCompany.IsCustomerFree = badCompany.IsCustomerFree;
    //      goodCompany.PortalSeats = badCompany.PortalSeats;
    //      goodCompany.ProductType = badCompany.ProductType;
    //      goodCompany.UserSeats = badCompany.UserSeats;
    //      goodCompany.NeedsIndexing = badCompany.NeedsIndexing;
    //      goodCompany.SystemEmailID = badCompany.SystemEmailID;
    //      goodCompany.WebServiceID = badCompany.WebServiceID;
    //      {
    //        User defaultSupportUser = _usersAndContacts.FindByUserID((int)badCompany.DefaultSupportUserID);
    //        if (defaultSupportUser != null)
    //        {
    //          goodCompany.DefaultSupportUserID = defaultSupportUser.UserID;
    //        }
    //      }
    //      if (badCompany.CreatorID > 0)
    //      {
    //        User creator = _usersAndContacts.FindByUserID(badCompany.CreatorID);
    //        if (creator != null)
    //        {
    //          goodCompany.CreatorID = creator.UserID;
    //        }
    //        else
    //        {
    //          goodCompany.CreatorID = -1;
    //        }
    //      }
    //      else
    //      {
    //        goodCompany.CreatorID = badCompany.CreatorID;
    //      }
    //      goodCompany.ParentID = orgID;
    //      goodCompany.ImportID = _importID;
    //      goodCompany.Collection.Save();
    //    }
    //  }
    //}
    /*
    private void RecoverAssets(int orgID)
    {
      // check corrupt db for different products,if so craete the new products, but do not use ID's
      Assets badAssets = new Assets(GetCorrupteLoginUser());
      badAssets.LoadByOrganizationIDCreatedAfterRestore(orgID);

      Assets goodAssets = new Assets(GetReviewLoginUser());
      goodAssets.LoadByOrganizationID(orgID);

      foreach (Asset badAsset in badAssets)
      {
        Asset goodAsset = goodAssets.FindByName(badAsset.Name);
        try
        {
          if (goodAsset == null)
          {
            goodAsset = (new Assets(GetReviewLoginUser())).AddNewAsset();
            goodAsset.CopyRowData(badAsset);
            goodAsset.DateCreated = badAsset.DateCreatedUtc;
            goodAsset.DateModified = badAsset.DateModifiedUtc;
            goodAsset.OrganizationID = orgID;
            goodAsset.ImportID = _importID;
            goodAsset.Collection.Save();
            if (goodAsset.Location == "1")
            {
              AssetAssignmentsView assetAssignments = new AssetAssignmentsView(GetCorrupteLoginUser());
              assetAssignments.LoadByAssetID(badAsset.AssetID);
              foreach (AssetAssignmentsViewItem assetAssignment in assetAssignments)
              {
                AssetHistory assetHistory = new AssetHistory(GetReviewLoginUser());
                AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();

                DateTime now = DateTime.UtcNow;

                assetHistoryItem.AssetID = goodAsset.AssetID;
                assetHistoryItem.OrganizationID = orgID;
                assetHistoryItem.ActionTime = assetAssignment.ActionTime;
                assetHistoryItem.ActionDescription = assetAssignment.ActionDescription;
                assetHistoryItem.ShippedFrom = assetAssignment.ShippedFrom;
                assetHistoryItem.ShippedFromRefType = assetAssignment.ShippedFromRefType;
                assetHistoryItem.ShippedTo = assetAssignment.ShippedTo;
                assetHistoryItem.TrackingNumber = assetAssignment.TrackingNumber;
                assetHistoryItem.ShippingMethod = assetAssignment.ShippingMethod;
                assetHistoryItem.ReferenceNum = assetAssignment.ReferenceNum;
                assetHistoryItem.Comments = assetAssignment.Comments;

                assetHistoryItem.DateCreated = assetAssignment.DateCreatedUtc;
                assetHistoryItem.Actor = assetAssignment.Actor;
                assetHistoryItem.RefType = assetAssignment.RefType;
                assetHistoryItem.DateModified = now;

                assetHistory.Save();

                AssetAssignments goodAssetAssignments = new AssetAssignments(GetReviewLoginUser());
                AssetAssignment goodAssetAssignment = goodAssetAssignments.AddNewAssetAssignment();

                goodAssetAssignment.HistoryID = assetHistoryItem.HistoryID;

                goodAssetAssignments.Save();
              }
            }
          }

        }
        catch (Exception ex)
        {
          SaveOrgResults(orgID, "Failure: " + ex.Message);
          ExceptionLogs.LogException(GetCorrupteLoginUser(), ex, "recover");
        }
      }
    }
    */
    private void RecoverActionsFromOldTickets(int orgID, LoginUser loginUser)
    {
      Actions badActions = new Actions(GetCorrupteLoginUser());
      SqlCommand command = new SqlCommand();
      command.CommandText = @"SELECT a.*, t.TicketNumber, t.OrganizationID FROM Actions a
LEFT JOIN Tickets t ON t.TicketID = a.TicketID
WHERE (t.OrganizationID = @OrganizationID) 
AND a.DateCreated > '2015-09-17 05:56:00'
AND t.DateCreated < '2015-09-17 05:56:00'";

      command.CommandType = CommandType.Text;
      command.Parameters.AddWithValue("@OrganizationID", orgID);
      badActions.Fill(command, "");

      foreach (TeamSupport.Data.Action badAction in badActions)
      {
        try
        {
          TeamSupport.Data.Action goodAction = new Actions(loginUser).AddNewAction();
          goodAction.CopyRowData(badAction);
          goodAction.DateCreated = badAction.DateCreatedUtc;
          goodAction.DateModified = badAction.DateModifiedUtc;
          goodAction.TicketID = badAction.TicketID;
          if (badAction.CreatorID > 0)
          {
            User creator = _usersAndContacts.FindByUserID(badAction.CreatorID);
            if (creator != null)
            {
              goodAction.CreatorID = creator.UserID;
            }
            else
            {
              goodAction.CreatorID = -5;
            }
          }
          else
          {
            goodAction.CreatorID = badAction.CreatorID;
          }

          if (badAction.ModifierID > 0)
          {
            User modifier = _usersAndContacts.FindByUserID(badAction.ModifierID);
            if (modifier != null)
            {
              goodAction.ModifierID = modifier.UserID;
            }
            else
            {
              goodAction.ModifierID = -5;
            }
          }
          else
          {
            goodAction.ModifierID = badAction.ModifierID;
          }
          goodAction.ImportID = _importID;
          goodAction.Collection.Save();

          Ticket ticket = Tickets.GetTicket(loginUser, goodAction.TicketID);
          ticket.ImportID = _importID;
          ticket.Collection.Save();

          EmailPosts.DeleteImportEmails(loginUser);
        }
        catch (Exception ex)
        {
          _exceptionOcurred = true;
          ExceptionLogs.LogException(GetCorrupteLoginUser(), ex, "recover");
        }

      }

    }

    private void RecoverTickets(int orgID, LoginUser loginUser)
    {
      Tickets badTickets = new Tickets(GetCorrupteLoginUser());
      SqlCommand command = new SqlCommand();
      command.CommandText = @"
        SELECT 
          * 
        FROM 
          Tickets 
        WHERE 
          OrganizationID = @OrganizationID
          AND DateCreated > '2015-09-17 05:56:00'
          AND TicketSource != 'EMail'
          AND IgnoreMe = 0
      ";
      command.CommandType = CommandType.Text;
      command.Parameters.AddWithValue("@OrganizationID", orgID);
      badTickets.Fill(command, "");

      Organizations existingCompanies = new Organizations(loginUser);
      existingCompanies.LoadByParentID(orgID, false);

      foreach (Ticket badTicket in badTickets)
      {
        try
        {
          //Copy Ticket
          Ticket goodTicket = (new Tickets(loginUser)).AddNewTicket();
          goodTicket.CopyRowData(badTicket);
          //Product
          if (badTicket.ProductID != null)
          {
            Product goodProduct = _goodProducts.FindByProductID((int)badTicket.ProductID);
            if (goodProduct == null)
            {
              Product badProduct = _badProducts.FindByProductID((int)badTicket.ProductID);
              goodProduct = _goodProducts.FindByName(badProduct.Name); 
              if (goodProduct != null)
              {
                goodTicket.ProductID = goodProduct.ProductID;
              }
              else
              {
                goodTicket.ProductID = null;
              }
            }
          }
          //Version
          if (goodTicket.ProductID != null && badTicket.ReportedVersionID != null)
          {
            ProductVersion goodProductVersion = _goodProductVersions.FindByProductVersionID((int)badTicket.ReportedVersionID);
            if (goodProductVersion == null)
            {
              ProductVersion badProductVersion = _badProductVersions.FindByProductVersionID((int)badTicket.ReportedVersionID);
              goodProductVersion = _goodProductVersions.FindByVersionNumber(badProductVersion.VersionNumber, (int)goodTicket.ProductID);
              if (goodProductVersion != null)
              {
                goodTicket.ReportedVersionID = goodProductVersion.ProductVersionID;
              }
              else
              {
                goodTicket.ReportedVersionID = null;
              }
            }
          }
          //Solved Version
          if (goodTicket.ProductID != null && badTicket.SolvedVersionID != null)
          {
            ProductVersion goodProductVersion = _goodProductVersions.FindByProductVersionID((int)badTicket.SolvedVersionID);
            if (goodProductVersion == null)
            {
              ProductVersion badProductVersion = _badProductVersions.FindByProductVersionID((int)badTicket.SolvedVersionID);
              goodProductVersion = _goodProductVersions.FindByVersionNumber(badProductVersion.VersionNumber, (int)goodTicket.ProductID);
              if (goodProductVersion != null)
              {
                goodTicket.SolvedVersionID = goodProductVersion.ProductVersionID;
              }
              else
              {
                goodTicket.SolvedVersionID = null;
              }
            }
          }
          //Group
          if (badTicket.GroupID != null)
          {
            Group goodGroup = _goodGroups.FindByGroupID((int)badTicket.GroupID);
            if (goodGroup == null)
            {
              Group badGroup = _badGroups.FindByGroupID((int)badTicket.GroupID);
              goodGroup = _goodGroups.FindByName(badGroup.Name);
              if (goodGroup != null)
              {
                goodTicket.GroupID = goodGroup.GroupID;
              }
              else
              {
                goodTicket.GroupID = null;
              }
            }
          }
          //Assigned User
          if (badTicket.UserID != null)
          {
            User goodUser = _goodUsers.FindByUserID((int)badTicket.UserID);
            if (goodUser == null)
            {
              User badUser = _badUsers.FindByUserID((int)badTicket.UserID);
              goodUser = _goodUsers.FindByEmail(badUser.Email);
              if (goodUser != null)
              {
                goodTicket.UserID = goodUser.UserID;
              }
              else
              {
                goodTicket.UserID = null;
              }
            }
          }
          //Type
          TicketType goodTicketType = _goodTicketTypes.FindByTicketTypeID(badTicket.TicketTypeID);
          if (goodTicketType == null)
          {
            TicketType badTicketType = _badTicketTypes.FindByTicketTypeID(badTicket.TicketTypeID);
            goodTicketType = _goodTicketTypes.FindByName(badTicketType.Name);
            if (goodTicketType != null)
            {
              goodTicket.TicketTypeID = goodTicketType.TicketTypeID;
            }
            else
            {
              goodTicket.TicketTypeID = _goodTicketTypes[0].TicketTypeID;
            }
          }

          //Status
          TicketStatus goodTicketStatus = _goodTicketStatuses.FindByTicketStatusID(badTicket.TicketStatusID);
          if (goodTicketStatus == null)
          {
            TicketStatus badTicketStatus = _badTicketStatuses.FindByTicketStatusID(badTicket.TicketStatusID);
            goodTicketStatus = _goodTicketStatuses.FindByName(badTicketStatus.Name, goodTicket.TicketTypeID);
            if (goodTicketStatus != null)
            {
              goodTicket.TicketStatusID = goodTicketStatus.TicketStatusID;
            }
            else
            {
              goodTicketStatus = _goodTicketStatuses.FindTopOne(goodTicket.TicketTypeID);
              goodTicket.TicketStatusID = goodTicketStatus.TicketStatusID;
            }
          }


          //Severity
          TicketSeverity goodTicketSeverity = _goodTicketSeverities.FindByTicketSeverityID(badTicket.TicketSeverityID);
          if(goodTicketSeverity == null)
          {
            TicketSeverity badTicketSeverity = _badTicketSeverities.FindByTicketSeverityID(badTicket.TicketSeverityID);
            goodTicketSeverity = _goodTicketSeverities.FindByName(badTicketSeverity.Name);
            if (goodTicketSeverity != null)
            {
              goodTicket.TicketSeverityID = goodTicketSeverity.TicketSeverityID;
            }
            else
            {
              goodTicket.TicketSeverityID = _goodTicketSeverities[0].TicketSeverityID;
            }
          }

          //Knowledgebase Cat
          if (badTicket.KnowledgeBaseCategoryID != null)
          {
            KnowledgeBaseCategory goodKBCategory = _goodTicketKBCategories.FindByCategoryID((int)badTicket.KnowledgeBaseCategoryID);
            if (goodKBCategory == null)
            {
              KnowledgeBaseCategory badKBCategory = _badTicketKBCategories.FindByCategoryID((int)badTicket.KnowledgeBaseCategoryID);
              goodKBCategory = _goodTicketKBCategories.FindByName(badKBCategory.CategoryName);
              if (goodKBCategory != null)
              {
                goodTicket.KnowledgeBaseCategoryID = goodKBCategory.CategoryID;
              }
              else
              {
                goodTicket.KnowledgeBaseCategoryID = null;
              }
            }
          }

          //Parent Ticket (NOTE from MT:  we decided as a team to null out this field to ensure no bad relationships can happen since we don't know the ticketID is a preserved field for every ticket.
          goodTicket.ParentID = null;

          //Closing User
          if (badTicket.CloserID != null)
          {
            User goodUser = _goodUsers.FindByUserID((int)badTicket.CloserID);
            if (goodUser == null)
            {
              User badUser = _badUsers.FindByUserID((int)badTicket.CloserID);
              goodUser = _goodUsers.FindByEmail(badUser.Email);
              if (goodUser != null)
              {
                goodTicket.CloserID = goodUser.UserID;
              }
              else
              {
                goodTicket.CloserID = null;
              }
            }
          }


          //Reset ticket dates
          goodTicket.DateCreated = badTicket.DateCreatedUtc;
          goodTicket.DateModified = badTicket.DateModifiedUtc;
          goodTicket.ParentID = null;
          goodTicket.ImportID = _importID;
          if (badTicket.CreatorID > 0)
          {
            User creator = _usersAndContacts.FindByUserID(badTicket.CreatorID);
            if (creator != null)
            {
              goodTicket.CreatorID = creator.UserID;
            }
            else
            {
              goodTicket.CreatorID = -5;
            }
          }
          else
          {
            goodTicket.CreatorID = badTicket.CreatorID;
          }

          if (badTicket.ModifierID > 0)
          {
            User modifier = _usersAndContacts.FindByUserID(badTicket.ModifierID);
            if (modifier != null)
            {
              goodTicket.ModifierID = modifier.UserID;
            }
            else
            {
              goodTicket.ModifierID = -5;
            }
          }
          else
          {
            goodTicket.ModifierID = badTicket.ModifierID;
          }
          goodTicket.TicketNumber = 0;
          goodTicket.Collection.Save();
          EmailPosts.DeleteImportEmails(loginUser);

          Actions badActions = new Actions(GetCorrupteLoginUser());
          badActions.LoadByTicketID(badTicket.TicketID);

          foreach (TeamSupport.Data.Action badAction in badActions)
          {
            TeamSupport.Data.Action goodAction = new Actions(loginUser).AddNewAction();
            goodAction.CopyRowData(badAction);
            goodAction.DateCreated = badAction.DateCreatedUtc;
            goodAction.DateModified = badAction.DateCreatedUtc;
            goodAction.TicketID = goodTicket.TicketID;
            goodAction.ImportID = _importID;
            if (badAction.CreatorID > 0)
            {
              User creator = _usersAndContacts.FindByUserID(badAction.CreatorID);
              if (creator != null)
              {
                goodAction.CreatorID = creator.UserID;
              }
              else
              {
                goodAction.CreatorID = -5;
              }
            }
            else
            {
              goodAction.CreatorID = badAction.CreatorID;
            }

            if (badAction.ModifierID > 0)
            {
              User modifier = _usersAndContacts.FindByUserID(badAction.ModifierID);
              if (modifier != null)
              {
                goodAction.ModifierID = modifier.UserID;
              }
              else
              {
                goodAction.ModifierID = -5;
              }
            }
            else
            {
              goodAction.ModifierID = badAction.ModifierID;
            }
            goodAction.Collection.Save();
            EmailPosts.DeleteImportEmails(loginUser);

          }


          Organizations orgs = new Organizations(GetCorrupteLoginUser());
          orgs.LoadBTicketID(badTicket.TicketID);

          foreach (Organization org in orgs)
          {
            Organization goodCompany = existingCompanies.FindByName(org.Name);
            if (org.ParentID == orgID && goodCompany != null)
            {
              goodTicket.Collection.AddOrganization(goodCompany.OrganizationID, goodTicket.TicketID);
              EmailPosts.DeleteImportEmails(loginUser);

            }
          }

          RecoverTicketCustomValues(orgID, badTicket.TicketID, goodTicket.TicketID);
          EmailPosts.DeleteImportEmails(loginUser);
        }
        catch (Exception ex)
        {
          _exceptionOcurred = true;
          ExceptionLogs.LogException(GetCorrupteLoginUser(), ex, "recover");
        }

      }
    }

    private void RecoverTicketCustomValues(int orgID, int badTicketID, int goodTicketID)
    {
      CustomValues badCustomValues = new CustomValues(GetCorrupteLoginUser());
      badCustomValues.LoadByReferenceTypeModifiedAfterRecovery(orgID, ReferenceType.Tickets, badTicketID);

      foreach (CustomValue badCustomValue in badCustomValues)
      {
        try
        {
          if (badCustomValue == null) continue;
          CustomValue goodCustomValue = CustomValues.GetValue(GetReviewLoginUser(), goodTicketID, badCustomValue.ApiFieldName);
          if (goodCustomValue != null)
          {
            goodCustomValue.Value = badCustomValue.Value;
            goodCustomValue.Collection.Save();
          }
        }
        catch (Exception ex)
        {
          _exceptionOcurred = true;
          ExceptionLogs.LogException(GetCorrupteLoginUser(), ex, "recover");
        }
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      int orgID = GetNextOrg();

      while (orgID > -1)
      {
        if (orgID < 0) return;
        try
        {
          ImportOrg(orgID, GetReviewLoginUser());
        }
        finally
        {
          orgID = GetNextOrg();
        }

      }
    }

    private void btnRollBackAll_Click(object sender, EventArgs e)
    {
      int orgID = GetNextOrg();

      while (orgID > -1)
      {
        if (orgID < 0) return;
        try
        {
          RollBack(orgID, GetReviewLoginUser());
        }
        finally
        {
          orgID = GetNextOrg();
        }

      }
    }

    private void btnImportOrgToReview_Click(object sender, EventArgs e)
    {
      ImportOrg(GetSelectedOrgID(), GetReviewLoginUser());
    }

    private void btnRollbackOrgFromReview_Click(object sender, EventArgs e)
    {
      RollBack(GetSelectedOrgID(), GetReviewLoginUser());
    }

    private void btnImportOrgToProduction_Click(object sender, EventArgs e)
    {
      ComboboxItem orgitem = cmbOrg.SelectedItem as ComboboxItem;

      string msg = string.Format("THIS WILL UPDATE '{0}:{1}' ON PRODUCTION.  Would you like to continue?", orgitem.Text, ((int)orgitem.Value).ToString());
      if (MessageBox.Show(msg, "Confrim", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No) return;
      ImportOrg((int)orgitem.Value, GetPRODUCTIONLoginUser());
    }

    private void btnRollBackOrgFromProduction_Click(object sender, EventArgs e)
    {
      ComboboxItem orgitem = cmbOrg.SelectedItem as ComboboxItem;

      string msg = string.Format("THIS WILL ROLLBACK '{0}:{1}' ON PRODUCTION.  Would you like to continue?", orgitem.Text, ((int)orgitem.Value).ToString());
      if (MessageBox.Show(msg, "Confrim", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No) return;
      RollBack((int)orgitem.Value, GetPRODUCTIONLoginUser());
    }

    private int GetSelectedOrgID()
    {
      return (int)(cmbOrg.SelectedItem as ComboboxItem).Value;
    
    
    }

  }
}
