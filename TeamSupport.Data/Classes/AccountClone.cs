using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace TeamSupport.Data
{
  public class AccountClone
  {

    public static Organization Clone(LoginUser loginUser, int sourceOrganizationID, string cloneName)
    {
      Organization sOrg = Organizations.GetOrganization(loginUser, sourceOrganizationID);
      Organization dOrg = (new Organizations(loginUser)).AddNewOrganization();

      dOrg.CopyRowData(sOrg);
      dOrg.Name = cloneName;
      dOrg.Collection.Save();

      //** Groups **//
      Groups sGroups = new Groups(loginUser);
      sGroups.LoadByOrganizationID(sourceOrganizationID);

      Groups dGroups = new Groups(loginUser);

      foreach (Group sGroup in sGroups)
      {
        Group dGroup = dGroups.AddNewGroup();
        dGroup.CopyRowData(sGroup);
        dGroup.OrganizationID = dOrg.OrganizationID;
      }
      dGroups.Save();

      //** Users **//

      Users sUsers = new Users(loginUser);
      sUsers.LoadByOrganizationID(sourceOrganizationID, false);

      Users dUsers = new Users(loginUser);

      foreach (User sUser in sUsers)
      {
        User dUser = dUsers.AddNewUser();
        dUser.CopyRowData(sUser);
        dUser.OrganizationID = dOrg.OrganizationID;
        if (sUser.PrimaryGroupID != null)
        {
          dUser.PrimaryGroupID = dGroups.FindByName(sGroups.FindByGroupID((int)sUser.PrimaryGroupID).Name).GroupID;
        }
      }
      dUsers.Save();


      //** Finish Organization w/ users and groups **//

      dOrg.DefaultPortalGroupID = sOrg.DefaultPortalGroupID == null ? null : (int?)dGroups.FindByName(sGroups.FindByGroupID((int)sOrg.DefaultPortalGroupID).Name).GroupID;
      dOrg.PrimaryUserID = sOrg.PrimaryUserID == null ? null : (int?)dUsers.FindByEmail(sUsers.FindByUserID((int)sOrg.PrimaryUserID).Email).UserID;
      dOrg.DefaultSupportUserID = null;
      dOrg.DefaultSupportGroupID = null;
      dOrg.WebServiceID = Guid.NewGuid();
      dOrg.SystemEmailID = Guid.NewGuid();
      dOrg.PortalGuid = Guid.NewGuid();
      dOrg.Collection.Save();

      //** Ticket Types **//

      TicketTypes sTicketTypes = new TicketTypes(loginUser);
      sTicketTypes.LoadByOrganizationID(sourceOrganizationID);

      TicketTypes dTicketTypes = new TicketTypes(loginUser);

      foreach (TicketType sTicketType in sTicketTypes)
      {
        TicketType dTicketType = dTicketTypes.AddNewTicketType();
        dTicketType.CopyRowData(sTicketType);
        dTicketType.OrganizationID = dOrg.OrganizationID;
      }
      dTicketTypes.Save();

      //** Ticket Severities **//

      TicketSeverities sSeverities = new TicketSeverities(loginUser);
      sSeverities.LoadByOrganizationID(sourceOrganizationID);

      TicketSeverities dSeverities = new TicketSeverities(loginUser);

      foreach (TicketSeverity sSeverity in sSeverities)
      {
        TicketSeverity dSeverity = dSeverities.AddNewTicketSeverity();
        dSeverity.CopyRowData(sSeverity);

        dSeverity.OrganizationID = dOrg.OrganizationID;
      }
      dSeverities.Save();

      //** Ticket Statuses **//

      TicketStatuses sStatuses = new TicketStatuses(loginUser);
      sStatuses.LoadByOrganizationID(sourceOrganizationID);

      TicketStatuses dStatuses = new TicketStatuses(loginUser);

      foreach (TicketStatus sStatus in sStatuses)
      {
        TicketType tempTT = sTicketTypes.FindByTicketTypeID(sStatus.TicketTypeID);
        if (tempTT == null) continue;
        TicketStatus dStatus = dStatuses.AddNewTicketStatus();
        dStatus.CopyRowData(sStatus);
        dStatus.OrganizationID = dOrg.OrganizationID;
        dStatus.TicketTypeID = dTicketTypes.FindByName(tempTT.Name).TicketTypeID;
      }
      dStatuses.Save();

      TicketNextStatuses sTicketNextStatuses = new TicketNextStatuses(loginUser);
      TicketNextStatuses dTicketNextStatuses = new TicketNextStatuses(loginUser);
      sTicketNextStatuses.LoadAll(sourceOrganizationID);

      foreach (TicketNextStatus sTicketNextStatus in sTicketNextStatuses)
      {
        TicketNextStatus dTicketNextStatus = dTicketNextStatuses.AddNewTicketNextStatus();
        dTicketNextStatus.CopyRowData(sTicketNextStatus);
        TicketStatus tempTS = sStatuses.FindByTicketStatusID(sTicketNextStatus.CurrentStatusID);
        TicketType tempTT = dTicketTypes.FindByName(sTicketTypes.FindByTicketTypeID(tempTS.TicketTypeID).Name);
        dTicketNextStatus.CurrentStatusID = dStatuses.FindByName(tempTS.Name, tempTT.TicketTypeID).TicketStatusID;
        tempTS = sStatuses.FindByTicketStatusID(sTicketNextStatus.CurrentStatusID);
        tempTT = dTicketTypes.FindByName(sTicketTypes.FindByTicketTypeID(tempTS.TicketTypeID).Name);
        dTicketNextStatus.NextStatusID = dStatuses.FindByName(tempTS.Name, tempTT.TicketTypeID).TicketStatusID;



















        //WORKING HERE














      }

      TicketTemplates sTicketTemplates = new TicketTemplates(loginUser);
      TicketTemplates dTicketTemplates = new TicketTemplates(loginUser);
      sTicketTemplates.LoadByOrganization(sourceOrganizationID);

      foreach (TicketTemplate sTicketTemplate in sTicketTemplates)
      {
        TicketTemplate dTicketTemplate = dTicketTemplates.AddNewTicketTemplate();
        dTicketTemplate.CopyRowData(sTicketTemplate);
        dTicketTemplate.OrganizationID = dOrg.OrganizationID;
        dTicketTemplate.TicketTypeID = dTicketTemplate.TicketTypeID != null ? (int?)dTicketTypes.FindByName(sTicketTypes.FindByTicketTypeID((int)sTicketTemplate.TicketTypeID).Name).TicketTypeID : null;
      }

      //** ActionTypes **//

      ActionTypes sActionTypes = new ActionTypes(loginUser);
      sActionTypes.LoadByOrganizationID(sourceOrganizationID);
      ActionTypes dActionTypes = new ActionTypes(loginUser);

      foreach (ActionType sActionType in sActionTypes)
      {
        ActionType dActionType = dActionTypes.AddNewActionType();
        dActionType.CopyRowData(sActionType);
        dActionType.OrganizationID = dOrg.OrganizationID;
      }
      dActionTypes.Save();


      //** Products **//
      
      Products sProducts = new Products(loginUser);
      Products dProducts = new Products(loginUser);
      sProducts.LoadByOrganizationID(sourceOrganizationID);

      foreach (Product sProduct in sProducts)
      {
        Product dProduct = dProducts.AddNewProduct();
        dProduct.CopyRowData(sProduct);
        dProduct.OrganizationID = dOrg.OrganizationID;
      }
      dProducts.Save();

      ProductVersionStatuses sProductVersionStatuses = new ProductVersionStatuses(loginUser);
      ProductVersionStatuses dProductVersionStatuses = new ProductVersionStatuses(loginUser);
      sProductVersionStatuses.LoadByOrganizationID(sourceOrganizationID);

      foreach (ProductVersionStatus sProductVersionStatus in sProductVersionStatuses)
      {
        ProductVersionStatus dProductVersionStatus = dProductVersionStatuses.AddNewProductVersionStatus();
        dProductVersionStatus.CopyRowData(sProductVersionStatus);
        dProductVersionStatus.OrganizationID = dOrg.OrganizationID;
      }

      dProductVersionStatuses.Save();

      ProductVersions sProductVersions = new ProductVersions(loginUser);
      ProductVersions dProductVersions = new ProductVersions(loginUser);
      sProductVersions.LoadAll(sourceOrganizationID);

      foreach (ProductVersion sProductVersion in sProductVersions)
      {
        ProductVersion dProductVersion = dProductVersions.AddNewProductVersion();
        dProductVersion.CopyRowData(sProductVersion);
        dProductVersion.ProductID = dProducts.FindByName(sProducts.FindByProductID(sProductVersion.ProductID).Name).ProductID;
      }
      dProductVersions.Save();

      PortalOptions sPortalOptions = new PortalOptions(loginUser);
      PortalOptions dPortalOptions = new PortalOptions(loginUser);
      sPortalOptions.LoadByOrganizationID(sourceOrganizationID);

      foreach (PortalOption sPortalOption in sPortalOptions)
      {
        PortalOption dPortalOption = dPortalOptions.AddNewPortalOption();
        dPortalOption.CopyRowData(sPortalOption);
        dPortalOption.OrganizationID = dOrg.OrganizationID;
        dPortalOption.RequestGroup = sPortalOption.RequestGroup == null ? null : (int?)dGroups.FindByName(sGroups.FindByGroupID((int)sPortalOption.RequestGroup).Name).GroupID;
        dPortalOption.RequestType = sPortalOption.RequestType == null ? null : (int?)dTicketTypes.FindByName(sTicketTypes.FindByTicketTypeID((int)sPortalOption.RequestType).Name).TicketTypeID; ;
      }
      dPortalOptions.Save();


      return dOrg;

      /*
       * Actions
       * Addresses
       * Assets
       * ChatSettings
       * ChatUserSettings
       * CrmLinkTable
       * CrmLinkFields
       * CustomFieldCategories
       * CustomFields
       * CustomValues
       * FacebookOptions
       * ForumCategories
       * ForumPermissions
       * ForumTickets
       * GroupUsers
       * Notes
       * OrganizationEmails
       * OrganizationProducts
       * Organizations
       * OrganizationSettings
       * OrganizationTickets
       * PhoneNumbers
       * PhoneQueue
       * PhoneTypes
       * PortalOptions
       * Reminders
       * Reports
       * SlaLevels
       * SlaTriggers
       * Subscriptions
       * Tags
       * TagLinks
       * Automation Stuff
       * TicketQueue
       * TicketRatings
       * TicketRelationShips
       * Users(Contacts)
       * WaterCooler
       * WikiArticles
       */
    }

  }
}
