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
  
  public class RestAttachments
  {
    public static string GetAttachment(RestCommand command, int attachmentID)
    {
      Attachment attachment = Attachments.GetAttachment(command.LoginUser, attachmentID);
      if (attachment.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return attachment.GetXml("Attachment", true);
    }
    
    public static string GetAttachments(RestCommand command)
    {
      Attachments attachments = new Attachments(command.LoginUser);
      attachments.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return attachments.GetXml("Attachments", "Attachment", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
