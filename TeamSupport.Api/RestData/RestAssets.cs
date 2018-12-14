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
  
  public class RestAssets
  {
    public static string GetAsset(RestCommand command, int assetID)
    {
      Asset asset = Assets.GetAsset(command.LoginUser, assetID);
      if (asset.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return asset.GetXml("Asset", true);
    }
    
    public static string GetAssets(RestCommand command)
    {
      Assets assets = new Assets(command.LoginUser);
      assets.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return assets.GetXml("Assets", "Asset", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }

    public static string CreateAsset(RestCommand command)
    {
      DateTime now = DateTime.UtcNow;
      Assets assets = new Assets(command.LoginUser);
      Asset asset = assets.AddNewAsset();
      asset.OrganizationID = command.Organization.OrganizationID;
      asset.FullReadFromXml(command.Data, true);
      if (asset.ProductID == null)
      {
          throw new RestException(HttpStatusCode.BadRequest, "Product required");
      }
      // For consistency all assets are created in the warehouse.
      asset.Location = "2";
      // This is normally not necessary, but as the CreatorID is defined as a null field in this table it is needed.
      asset.CreatorID = command.LoginUser.UserID;
      asset.NeedsIndexing = true;
      asset.DateCreated = now;
      asset.DateModified = now;
      asset.Collection.Save();
      asset.UpdateCustomFieldsFromXml(command.Data);

      string description = String.Format("Asset {0} created via API.", GetAssetReference(asset));
      AssetHistory history = new AssetHistory(command.LoginUser);
      AssetHistoryItem historyItem = history.AddNewAssetHistoryItem();

      historyItem.OrganizationID = command.Organization.OrganizationID;
      historyItem.Actor = command.LoginUser.UserID;
      historyItem.AssetID = asset.AssetID;
      historyItem.ActionTime = DateTime.UtcNow;
      historyItem.ActionDescription = "Asset created via API.";
      historyItem.ShippedFrom = 0;
      historyItem.ShippedTo = 0;
      historyItem.TrackingNumber = string.Empty;
      historyItem.ShippingMethod = string.Empty;
      historyItem.ReferenceNum = string.Empty;
      historyItem.Comments = string.Empty;
      historyItem.DateCreated = now;
      historyItem.DateModified = now;

      history.Save();

      return AssetsView.GetAssetsViewItem(command.LoginUser, asset.AssetID).GetXml("Asset", true);
    }

    private static string GetAssetReference(Asset asset)
    {
      string result = "with AssetID: " + asset.AssetID.ToString();

      if (!String.IsNullOrEmpty(asset.Name))
      {
        result = asset.Name;
      }
      else if (!String.IsNullOrEmpty(asset.SerialNumber))
      {
        result = "with Serial Number: " + asset.SerialNumber;
      }

      return result;
    }

    public static string AddTicketAsset(RestCommand command, int assetID, int ticketID)
    {
      Asset asset = Assets.GetAsset(command.LoginUser, assetID);
      if (asset == null || asset.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      Ticket ticket = Tickets.GetTicket(command.LoginUser, ticketID);
      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      ticket.Collection.AddAsset(assetID, ticketID);

      return RestTickets.GetTicketsByAssetID(command, assetID, true);
    }

    public static string DeleteTicketAsset(RestCommand command, int assetID, int ticketID)
    {
      Asset asset = Assets.GetAsset(command.LoginUser, assetID);
      if (asset == null || asset.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      Ticket ticket = Tickets.GetTicket(command.LoginUser, ticketID);
      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      ticket.Collection.RemoveAsset(assetID, ticketID);

      return RestTickets.GetTicketsByAssetID(command, assetID, true);
    }

    public static string UpdateAsset(RestCommand command, int assetID)
    {
      Asset asset = Assets.GetAsset(command.LoginUser, assetID);
      if (asset == null || asset.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      string originalLocation = asset.Location;
      asset.FullReadFromXml(command.Data, false);
      if (asset.ProductID == null)
      {
          throw new RestException(HttpStatusCode.BadRequest, "Product required");
      }

      if (
        String.IsNullOrEmpty(asset.Location.Trim()) || 
        (
          asset.Location != "1"
          && asset.Location != "2"
          && asset.Location != "3"
        )
      )
      {
        asset.Location = originalLocation;
      }
      //Location can not be changed via the WebApp. The only change we are allowing is to bring an asset back to the Warehouse from the Junkyard.
      else if (asset.Location == "1" && originalLocation != "1")
      {
        throw new RestException(HttpStatusCode.BadRequest, "Please use a POST against the /assets/{id}/assignments/ URL to assign an asset.");
      }
      else if (asset.Location == "2" && originalLocation == "1")
      {
        throw new RestException(HttpStatusCode.BadRequest, "Please use a DELETE against the /assets/{id}/assignments/ URL to return an asset.");
      }
      else if (asset.Location == "3" && originalLocation == "1")
      {
        throw new RestException(HttpStatusCode.BadRequest, "Please return an asset to the Warehouse before sending it to the Junkyard.");
      }
      else if (asset.Location == "3" && originalLocation == "2")
      {
        throw new RestException(HttpStatusCode.BadRequest, "Please use a DELETE against the /assets/{id}/ URL to assign an asset to the Junkyard.");
      }

      asset.Collection.Save();
      asset.UpdateCustomFieldsFromXml(command.Data);

      if (asset.Location == "2" && originalLocation == "3")
      {
        DateTime now = DateTime.UtcNow;
        AssetHistory assetHistory = new AssetHistory(command.LoginUser);
        AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();

        assetHistoryItem.AssetID = assetID;
        assetHistoryItem.OrganizationID = command.LoginUser.OrganizationID;
        assetHistoryItem.ActionTime = DateTime.UtcNow;
        assetHistoryItem.ActionDescription = "Restore asset from Junkyard via API";
        assetHistoryItem.ShippedFrom = -1;
        assetHistoryItem.ShippedFromRefType = -1;
        assetHistoryItem.ShippedTo = -1;
        assetHistoryItem.RefType = -1;
        assetHistoryItem.TrackingNumber = string.Empty;
        assetHistoryItem.ShippingMethod = string.Empty;
        assetHistoryItem.ReferenceNum = string.Empty;

        assetHistoryItem.DateCreated = now;
        assetHistoryItem.Actor = command.LoginUser.UserID;
        assetHistoryItem.DateModified = now;
        assetHistoryItem.ModifierID = command.LoginUser.UserID;

        assetHistory.Save();
      }

      return AssetsView.GetAssetsViewItem(command.LoginUser, assetID).GetXml("Asset", true);
    }

    public static string JunkAsset(RestCommand command, int assetID)
    {
      Asset asset = Assets.GetAsset(command.LoginUser, assetID);
      if (asset.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      if (asset.Location == "1") throw new RestException(HttpStatusCode.BadRequest, "Please return an asset to the Warehouse before sending it to the Junkyard.");
      if (asset.Location == "3") throw new RestException(HttpStatusCode.BadRequest, "This asset is already in the Junkyard.");
      asset.Location = "3";
      asset.AssignedTo = null;
      DateTime now = DateTime.UtcNow;
      asset.DateModified = now;
      asset.ModifierID = command.LoginUser.UserID;
      asset.Collection.Save();

      AssetHistory assetHistory = new AssetHistory(command.LoginUser);
      AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();
      //Html specification does not allow body being send in the DELETE method.
      //Nevertheless, is relevant as we are not really deleting and we need to add a delete comment.
      //If no body is sent an exception will be thrown. We ignore it to allow sending to junkyard without a comment.
      try
      {
        assetHistoryItem.ReadFromXml(command.Data, true);
      }
      catch (Exception)
      {
      }
      
      assetHistoryItem.AssetID = assetID;
      assetHistoryItem.OrganizationID = command.LoginUser.OrganizationID;
      assetHistoryItem.ActionTime = now;
      assetHistoryItem.ActionDescription = "Asset assigned to Junkyard via API.";
      assetHistoryItem.ShippedFrom = -1;
      assetHistoryItem.ShippedFromRefType = -1;
      assetHistoryItem.ShippedTo = -1;
      assetHistoryItem.RefType = -1;
      assetHistoryItem.TrackingNumber = string.Empty;
      assetHistoryItem.ShippingMethod = string.Empty;
      assetHistoryItem.ReferenceNum = string.Empty;
      //This is handled by the ReadFromXml
      //assetHistoryItem.Comments = comments;

      assetHistoryItem.DateCreated = now;
      assetHistoryItem.Actor = command.LoginUser.UserID;
      assetHistoryItem.DateModified = now;
      assetHistoryItem.ModifierID = command.LoginUser.UserID;

      assetHistory.Save();


      return AssetHistoryView.GetAssetHistoryViewItem(command.LoginUser, assetHistoryItem.HistoryID).GetXml("AssetHistoryItem", true);
    }
  }
  
}





  
