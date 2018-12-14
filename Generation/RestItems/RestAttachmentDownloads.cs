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
  
  public class RestAttachmentDownloads
  {
    public static string GetAttachmentDownload(RestCommand command, int attachmentDownloadID)
    {
      AttachmentDownload attachmentDownload = AttachmentDownloads.GetAttachmentDownload(command.LoginUser, attachmentDownloadID);
      if (attachmentDownload.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return attachmentDownload.GetXml("AttachmentDownload", true);
    }
    
    public static string GetAttachmentDownloads(RestCommand command)
    {
      AttachmentDownloads attachmentDownloads = new AttachmentDownloads(command.LoginUser);
      attachmentDownloads.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return attachmentDownloads.GetXml("AttachmentDownloads", "AttachmentDownload", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
