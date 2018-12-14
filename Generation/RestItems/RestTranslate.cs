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
  
  public class RestTranslate
  {
    public static string GetTranslateItem(RestCommand command, int phraseID)
    {
      TranslateItem translateItem = Translate.GetTranslateItem(command.LoginUser, phraseID);
      if (translateItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return translateItem.GetXml("TranslateItem", true);
    }
    
    public static string GetTranslate(RestCommand command)
    {
      Translate translate = new Translate(command.LoginUser);
      translate.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return translate.GetXml("Translate", "TranslateItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
