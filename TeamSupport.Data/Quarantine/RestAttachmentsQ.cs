using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using TeamSupport.Data;
using System.Net;
using System.Web;
using TeamSupport.Api;

namespace TeamSupport.Data.Quarantine
{
  
  public static class RestAttachmentsQ
  {

    public static string GetAttachments(RestCommand command, int actionID, bool orderByDateCreated = false)
    {
      //AttachmentProxy[] actionAttachments = ModelAPI.ModelAPI.Read<AttachmentProxy[]>(actionID);

      Attachments attachments = new Attachments(command.LoginUser);
      if (orderByDateCreated)
      {
        attachments.LoadByActionID(actionID, "DateCreated DESC");
      }
      else
      {
        attachments.LoadByActionID(actionID);
      }

      return attachments.GetXml("Attachments", "Attachment", true, command.Filters);
    }

    public static string CreateAttachment(RestCommand command, int ticketIDOrNumber, int actionID)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketIDOrNumber);
      if (ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      string path = AttachmentPath.GetPath(command.LoginUser, command.Organization.OrganizationID, AttachmentPath.Folder.Actions, 3);
      path = Path.Combine(path, actionID.ToString());
      if (!Directory.Exists(path)) Directory.CreateDirectory(path);
      HttpFileCollection files = command.Context.Request.Files;

      if (files.Count > 0)
      {
        if (files[0].ContentLength > 0)
        {
          string fileName = RemoveSpecialCharacters(DataUtils.VerifyUniqueUrlFileName(path, Path.GetFileName(files[0].FileName)));

          files[0].SaveAs(Path.Combine(path, fileName));

          Attachment attachment = (new Attachments(command.LoginUser)).AddNewAttachment();
          attachment.RefType = AttachmentProxy.References.Actions;
          attachment.RefID = (int)actionID;
          attachment.OrganizationID = command.Organization.OrganizationID;
          attachment.FileName = fileName;
          attachment.Path = Path.Combine(path, fileName);
          attachment.FileType = files[0].ContentType;
          attachment.FileSize = files[0].ContentLength;
            attachment.FilePathID = 3;
          attachment.Collection.Save();
          return attachment.Collection.GetXml("Attachments", "Attachment", true, command.Filters);
        }
        else
        {
          throw new RestException(HttpStatusCode.BadRequest, "The file to attach is empty.");
        }
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "No file to attach.");
      }
    }

    private static string RemoveSpecialCharacters(string text)
    {
      return Path.GetInvalidFileNameChars().Aggregate(text, (current, c) => current.Replace(c.ToString(), "_"));
    }

    public static string GetAttachmentsByAssetID(RestCommand command, int assetID, bool orderByDateCreated = false)
    {
      Attachments attachments = new Attachments(command.LoginUser);
      if (orderByDateCreated)
      {
        attachments.LoadByReference(ReferenceType.Assets, assetID, "DateCreated DESC");
      }
      else
      {
        attachments.LoadByReference(ReferenceType.Assets, assetID);
      }

      return attachments.GetXml("Attachments", "Attachment", true, command.Filters);
    }

    public static string CreateAttachment(RestCommand command, int assetID)
    {
      Asset asset = Assets.GetAsset(command.LoginUser, assetID);
      if (asset == null || asset.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      string path = AttachmentPath.GetPath(command.LoginUser, command.Organization.OrganizationID, AttachmentPath.Folder.AssetAttachments, 3);
      path = Path.Combine(path, assetID.ToString());
      if (!Directory.Exists(path)) Directory.CreateDirectory(path);
      HttpFileCollection files = command.Context.Request.Files;

      if (files.Count > 0)
      {
        if (files[0].ContentLength > 0)
        {
          string fileName = RemoveSpecialCharacters(DataUtils.VerifyUniqueUrlFileName(path, Path.GetFileName(files[0].FileName)));

          files[0].SaveAs(Path.Combine(path, fileName));

          Attachment attachment = (new Attachments(command.LoginUser)).AddNewAttachment();
          attachment.RefType = AttachmentProxy.References.Assets;
          attachment.RefID = (int)assetID;
          attachment.OrganizationID = command.Organization.OrganizationID;
          attachment.FileName = fileName;
          attachment.Path = Path.Combine(path, fileName);
          attachment.FileType = files[0].ContentType;
          attachment.FileSize = files[0].ContentLength;
          attachment.FilePathID = 3;
          attachment.Collection.Save();
          return attachment.Collection.GetXml("Attachments", "Attachment", true, command.Filters);
        }
        else
        {
          throw new RestException(HttpStatusCode.BadRequest, "The file to attach is empty.");
        }
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "No file to attach.");
      }
    }

    public static string DeleteAttachment(RestCommand command, int assetID, int attachmentID)
    {
      Asset asset = Assets.GetAsset(command.LoginUser, assetID);
      if (asset.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      Attachment attachment = Attachments.GetAttachment(command.LoginUser, attachmentID);
      if (attachment.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      if (attachment.RefID != asset.AssetID) throw new RestException(HttpStatusCode.Unauthorized);
      Attachments.DeleteAttachmentAndFile(command.LoginUser, attachmentID);

      return attachment.Collection.GetXml("Attachments", "Attachment", true, command.Filters);
    }

    public static string GetAttachmentsAsXML(RestCommand _command, int attachmentID)
    {
        string data;
        Attachment attachment = Attachments.GetAttachment(_command.LoginUser, attachmentID);
        data = attachment.Collection.GetXml("Attachments", "Attachment", true, _command.Filters);
        return data;
    }

    }

}





  
