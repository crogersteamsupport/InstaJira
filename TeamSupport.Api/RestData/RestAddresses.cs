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
  public class RestAddresses
  {

    public static string GetAddress(RestCommand command, ReferenceType refType, int refID, int addressID)
    {
      Address item = Addresses.GetAddress(command.LoginUser, addressID);
      if (item.RefType != refType && item.RefID != refID) throw new RestException(HttpStatusCode.Unauthorized);
      if (!DataUtils.IsReferenceValid(command.LoginUser, refType, refID)) throw new RestException(HttpStatusCode.Unauthorized);

      return item.GetXml("Address", true);
    }


    public static string GetAddresses(RestCommand command, ReferenceType refType, int refID, bool orderByDateCreated = false)
    {
      if (!DataUtils.IsReferenceValid(command.LoginUser, refType, refID)) throw new RestException(HttpStatusCode.Unauthorized);

      Addresses items = new Addresses(command.LoginUser);
      if (orderByDateCreated)
      {
        items.LoadByID(refID, refType, "DateCreated DESC");
      }
      else
      {
        items.LoadByID(refID, refType);
      }
      return items.GetXml("Addresses", "Address", true, command.Filters);
    }

    public static string AddAddress(RestCommand command, ReferenceType refType, int refID)
    {
      if (!DataUtils.IsReferenceValid(command.LoginUser, refType, refID)) throw new RestException(HttpStatusCode.Unauthorized);
      Addresses items = new Addresses(command.LoginUser);
      Address item = items.AddNewAddress();
      item.ReadFromXml(command.Data, true);
      item.RefType = refType;
      item.RefID = refID;
      item.Collection.Save();
      return item.GetXml("Address", true);
    }

    public static string UpdateAddress(RestCommand command, int addressID)
    {
      Address item = Addresses.GetAddress(command.LoginUser, addressID);
      if (!DataUtils.IsReferenceValid(command.LoginUser, item.RefType, item.RefID)) throw new RestException(HttpStatusCode.Unauthorized);
      item.ReadFromXml(command.Data, false);
      item.Collection.Save();
      return item.GetXml("Address", true);
    }

    public static string RemoveAddress(RestCommand command, ReferenceType refType, int refID, int addressID)
    {
      Address item = Addresses.GetAddress(command.LoginUser, addressID);
      if (item.RefType != refType && item.RefID != refID) throw new RestException(HttpStatusCode.Unauthorized);
      if (!DataUtils.IsReferenceValid(command.LoginUser, refType, refID)) throw new RestException(HttpStatusCode.Unauthorized);
      string result = item.GetXml("Address", true);
      item.Delete();
      item.Collection.Save();
      return result;
    }
  }
}
