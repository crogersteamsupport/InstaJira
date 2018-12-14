using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using TeamSupport.Data;
using System.Net;

namespace TeamSupport.Api
{
  
  public class RestAssetAssignments
  {
    public static string AddAssetAssignment(RestCommand command, int assetID)
    {
      Asset asset = Assets.GetAsset(command.LoginUser, assetID);
      if (asset == null || asset.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      if (asset.Location == "3") throw new RestException(HttpStatusCode.Forbidden, "Junkyard assets cannot be assigned. Please move it to the Warehouse before assigning it.");

      AssetHistory assetHistory = new AssetHistory(command.LoginUser);
      AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();
      assetHistoryItem.FullReadFromXml(command.Data, true);

      ValidateAssignment(command.LoginUser, assetHistoryItem);

      DateTime now = DateTime.UtcNow;
      assetHistoryItem.AssetID = assetID;
      assetHistoryItem.OrganizationID = command.LoginUser.OrganizationID;
      assetHistoryItem.ActionTime = now;
      assetHistoryItem.ShippedFrom = command.LoginUser.OrganizationID;
      assetHistoryItem.ShippedFromRefType = (int)ReferenceType.Organizations;
      assetHistoryItem.DateCreated = now;
      assetHistoryItem.Actor = command.LoginUser.UserID;
      assetHistoryItem.DateModified = now;
      assetHistoryItem.ModifierID = command.LoginUser.UserID;
      assetHistory.Save();

      AssetAssignments assetAssignments = new AssetAssignments(command.LoginUser);
      AssetAssignment assetAssignment = assetAssignments.AddNewAssetAssignment();
      assetAssignment.HistoryID = assetHistoryItem.HistoryID;
      assetAssignments.Save();

      asset.Location = "1";
      asset.AssignedTo = assetHistoryItem.ShippedTo;
      asset.Collection.Save();

      return AssetAssignmentsView.GetAssetAssignmentsViewItem(command.LoginUser, assetAssignment.AssetAssignmentsID).GetXml("AssetAssignment", true);
    }

    private static void ValidateAssignment(LoginUser loginUser, AssetHistoryItem assetHistoryItem)
    {
      //To validate the assignment the following must be true
      //The ShippedTo value and record must exist and belong to the processing organization 
      if (assetHistoryItem.ShippedTo == null)
      {
        throw new RestException(HttpStatusCode.BadRequest, "A valid OrganizationID or ContactID is required as a ShippedTo value.");
      }
      else if (assetHistoryItem.RefType == null)
      {
        throw new RestException(HttpStatusCode.BadRequest, "A 9 or a 32 value is required for the RefType field to assign to either a customer or a contact respectively.");
      }
      else if ((ReferenceType)assetHistoryItem.RefType == ReferenceType.Organizations)
      {
        Organization assignedCustomer = Organizations.GetOrganization(loginUser, (int)assetHistoryItem.ShippedTo);
        if (assignedCustomer == null || assignedCustomer.ParentID != loginUser.OrganizationID)
        {
          throw new RestException(HttpStatusCode.BadRequest, "A valid OrganizationID or ContactID is required as a ShippedTo value.");
        }
      }
      else if ((ReferenceType)assetHistoryItem.RefType == ReferenceType.Contacts)
      {
        User assignedContact = Users.GetUser(loginUser, (int)assetHistoryItem.ShippedTo);
        if (assignedContact == null)
        {
          throw new RestException(HttpStatusCode.BadRequest, "A valid OrganizationID or ContactID is required as a ShippedTo value.");
        }
        else
        {
          Organization assignedContactOrganization = Organizations.GetOrganization(loginUser, (int)assignedContact.OrganizationID);
          if (assignedContactOrganization == null || assignedContactOrganization.ParentID != loginUser.OrganizationID)
          {
            throw new RestException(HttpStatusCode.BadRequest, "A valid OrganizationID or ContactID is required as a ShippedTo value.");
          }
        }
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "A 9 or a 32 value is required for the RefType field to assign to either a customer or a contact respectively.");
      }

      //The DateShipped must exist
      //if (assetHistoryItem.
      //There is no DateShipped column. Is embedded in description. So we won't be able to validate.
      //Lets at least check that the description is not null
      if (String.IsNullOrEmpty(assetHistoryItem.ActionDescription))
      {
        throw new RestException(HttpStatusCode.BadRequest, "Please provide an action description indicating when the asset was shipped.");
      }

      //The Shipping method must exist and be one of the existing values
      if (
        String.IsNullOrEmpty(assetHistoryItem.ShippingMethod) || 
        (
          assetHistoryItem.ShippingMethod.ToLower() != "fedex" 
          && assetHistoryItem.ShippingMethod.ToLower() != "ups" 
          && assetHistoryItem.ShippingMethod.ToLower() != "usps"
          && assetHistoryItem.ShippingMethod.ToLower() != "other"
        )
      )
      {
        throw new RestException(HttpStatusCode.BadRequest, "Please provide either FedEx, UPS, USPS or Other as ShippingMethod value.");
      }
    }

    public static string ReturnAsset(RestCommand command, int assetID)
    {
      Asset asset = Assets.GetAsset(command.LoginUser, assetID);
      if (asset == null || asset.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      if (asset.Location != "1") throw new RestException(HttpStatusCode.BadRequest, "Only assigned assets can be returned.");

      AssetAssignmentsView assetAssignmentsView = new AssetAssignmentsView(command.LoginUser);
      assetAssignmentsView.LoadByAssetID(assetID);

      AssetHistory assetHistory = new AssetHistory(command.LoginUser);
      AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();
      //Html specification does not allow body being send in the DELETE method.
      //Nevertheless, is relevant as in addition to deleting the assignments we are also adding a history record indicating shipping data.
      //If no body is sent an exception will be thrown as the Shipping Date is required by the webapp.
      try
      {
        assetHistoryItem.ReadFromXml(command.Data, true);
      }
      catch (Exception)
      {
        throw new RestException(HttpStatusCode.BadRequest, "Please include a request body with an <AssetHistoryItem> node including at least an <ActionDescription> including the Date Shipped and a <ShippingMethod> node with a valid value.");
      }
      
      ValidateReturn(assetHistoryItem);

      //Update Asset.
      asset.Location = "2";
      asset.AssignedTo = null;
      DateTime now = DateTime.UtcNow;
      asset.DateModified = now;
      asset.ModifierID = command.LoginUser.UserID;
      asset.Collection.Save();

      //Add history record.
      assetHistoryItem.AssetID = assetID;
      assetHistoryItem.OrganizationID = command.LoginUser.OrganizationID;
      assetHistoryItem.ActionTime = now;
      assetHistoryItem.ShippedFrom = assetAssignmentsView[0].ShippedTo;
      assetHistoryItem.ShippedFromRefType = assetAssignmentsView[0].RefType;
      assetHistoryItem.ShippedTo = command.LoginUser.OrganizationID;
      assetHistoryItem.RefType = (int)ReferenceType.Organizations;

      assetHistoryItem.DateCreated = now;
      assetHistoryItem.Actor = command.LoginUser.UserID;
      assetHistoryItem.DateModified = now;
      assetHistoryItem.ModifierID = command.LoginUser.UserID;

      assetHistory.Save();

      //Delete assignments
      AssetAssignments assetAssignments = new AssetAssignments(command.LoginUser);
      foreach (AssetAssignmentsViewItem assetAssignmentViewItem in assetAssignmentsView)
      {
        assetAssignments.DeleteFromDB(assetAssignmentViewItem.AssetAssignmentsID);
      }

      return AssetHistoryView.GetAssetHistoryViewItem(command.LoginUser, assetHistoryItem.HistoryID).GetXml("AssetHistoryItem", true);
    }

    private static void ValidateReturn(AssetHistoryItem assetHistoryItem)
    {
      //The DateShipped must exist
      //if (assetHistoryItem.
      //There is no DateShipped column. Is embedded in description. So we won't be able to validate.
      //Lets at least check that the description is not null
      if (String.IsNullOrEmpty(assetHistoryItem.ActionDescription))
      {
        throw new RestException(HttpStatusCode.BadRequest, "Please provide an action description indicating when the asset was shipped.");
      }

      //The Shipping method must exist and be one of the existing values
      if (
        String.IsNullOrEmpty(assetHistoryItem.ShippingMethod) ||
        (
          assetHistoryItem.ShippingMethod.ToLower() != "fedex"
          && assetHistoryItem.ShippingMethod.ToLower() != "ups"
          && assetHistoryItem.ShippingMethod.ToLower() != "usps"
          && assetHistoryItem.ShippingMethod.ToLower() != "other"
        )
      )
      {
        throw new RestException(HttpStatusCode.BadRequest, "Please provide either FedEx, UPS, USPS or Other as ShippingMethod value.");
      }
    }
  }
  
}





  
