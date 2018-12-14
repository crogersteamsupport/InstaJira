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
  
  public class RestDeletedIndexItems
  {
    public static string GetDeletedIndexItem(RestCommand command, int deletedIndexID)
    {
      DeletedIndexItem deletedIndexItem = DeletedIndexItems.GetDeletedIndexItem(command.LoginUser, deletedIndexID);
      if (deletedIndexItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return deletedIndexItem.GetXml("DeletedIndexItem", true);
    }
    
    public static string GetDeletedIndexItems(RestCommand command)
    {
      DeletedIndexItems deletedIndexItems = new DeletedIndexItems(command.LoginUser);
      deletedIndexItems.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return deletedIndexItems.GetXml("DeletedIndexItems", "DeletedIndexItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
