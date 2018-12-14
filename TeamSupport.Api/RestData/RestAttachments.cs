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

namespace TeamSupport.Api
{
  
  public class RestAttachments
  {
    public static string GetAttachment(RestCommand command, int attachmentID)
    {
      AttachmentProxy attachment = ModelAPI.Model_API.Read<AttachmentProxy>(attachmentID);
      if (attachment.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      if (!TeamSupport.Permissions.UserRights.CanOpenAttachment(command.LoginUser, attachment))
        throw new RestException(HttpStatusCode.Unauthorized);

      if (!File.Exists(attachment.Path))
      {
        command.Context.Response.Write("Invalid attachment.");
        command.Context.Response.ContentType = "text/html";
      }
      else
      {
        command.Context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + attachment.FileName + "\"");
        command.Context.Response.AddHeader("Content-Length", attachment.FileSize.ToString());
        command.Context.Response.ContentType = attachment.FileType;
        command.Context.Response.WriteFile(attachment.Path);
      }

      return "attachment";
    }

    public static string GetAttachments(RestCommand command, int actionID, bool orderByDateCreated = false)
    {
        return TeamSupport.Data.Quarantine.RestAttachmentsQ.GetAttachments(command, actionID, orderByDateCreated);
    }

    public static string CreateAttachment(RestCommand command, int ticketIDOrNumber, int actionID)
    {
        return TeamSupport.Data.Quarantine.RestAttachmentsQ.CreateAttachment(command, ticketIDOrNumber, actionID);
    }

    private static string RemoveSpecialCharacters(string text)
    {
      return Path.GetInvalidFileNameChars().Aggregate(text, (current, c) => current.Replace(c.ToString(), "_"));
    }

    public static string GetAttachmentsByAssetID(RestCommand command, int assetID, bool orderByDateCreated = false)
    {
        return TeamSupport.Data.Quarantine.RestAttachmentsQ.GetAttachmentsByAssetID(command, assetID, orderByDateCreated);
    }

    public static string CreateAttachment(RestCommand command, int assetID)
    {
        return TeamSupport.Data.Quarantine.RestAttachmentsQ.CreateAttachment(command, assetID);
    }

    public static string DeleteAttachment(RestCommand command, int assetID, int attachmentID)
    {
        return TeamSupport.Data.Quarantine.RestAttachmentsQ.DeleteAttachment(command, assetID, attachmentID);
    }

    // add a layer to get at Attachments.GetXML
    public static string GetAttachmentsAsXML(RestCommand _command, int attachmentID)
    {
        return TeamSupport.Data.Quarantine.RestAttachmentsQ.GetAttachmentsAsXML(_command, attachmentID);
    }

    }

}





  
