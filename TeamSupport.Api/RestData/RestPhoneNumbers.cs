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
  public class RestPhoneNumbers
  {

    public static string GetPhoneNumber(RestCommand command, ReferenceType refType, int refID, int phoneID)
    {
      PhoneNumbersViewItem item = PhoneNumbersView.GetPhoneNumbersViewItem(command.LoginUser, phoneID);
      if (item.RefType != refType && item.RefID != refID) throw new RestException(HttpStatusCode.Unauthorized);
      if (!DataUtils.IsReferenceValid(command.LoginUser, refType, refID)) throw new RestException(HttpStatusCode.Unauthorized);

      return item.GetXml("PhoneNumber", true);
    }


    public static string GetPhoneNumbers(RestCommand command, ReferenceType refType, int refID, bool orderByDateCreated = false)
    {
      if (!DataUtils.IsReferenceValid(command.LoginUser, refType, refID)) throw new RestException(HttpStatusCode.Unauthorized);

      PhoneNumbersView items = new PhoneNumbersView(command.LoginUser);
      if (orderByDateCreated)
      {
        items.LoadByID(refID, refType, "DateCreated DESC");
      }
      else
      {
        items.LoadByID(refID, refType);
      }
      return items.GetXml("PhoneNumbers", "PhoneNumber", true, command.Filters);
    }

    public static string AddPhoneNumber(RestCommand command, ReferenceType refType, int refID)
    {
      if (!DataUtils.IsReferenceValid(command.LoginUser, refType, refID)) throw new RestException(HttpStatusCode.Unauthorized);
      PhoneNumbers items = new PhoneNumbers(command.LoginUser);
      PhoneNumber item = items.AddNewPhoneNumber();
      item.ReadFromXml(command.Data, true);
      item.RefType = refType;
      item.RefID = refID;
      item.Collection.Save();
      return PhoneNumbersView.GetPhoneNumbersViewItem(command.LoginUser, item.PhoneID).GetXml("PhoneNumber", true);
    }

    public static string UpdatePhoneNumber(RestCommand command, int phoneID)
    {
      PhoneNumber item = PhoneNumbers.GetPhoneNumber(command.LoginUser, phoneID);
      if (!DataUtils.IsReferenceValid(command.LoginUser, item.RefType, item.RefID)) throw new RestException(HttpStatusCode.Unauthorized);
      item.ReadFromXml(command.Data, false);
      item.Collection.Save();
      return "";
    }

    public static string RemovePhoneNumber(RestCommand command, ReferenceType refType, int refID, int phoneID)
    {
      PhoneNumber item = PhoneNumbers.GetPhoneNumber(command.LoginUser, phoneID);
      if (item.RefType != refType && item.RefID != refID) throw new RestException(HttpStatusCode.Unauthorized);
      if (!DataUtils.IsReferenceValid(command.LoginUser, refType, refID)) throw new RestException(HttpStatusCode.Unauthorized);
      item.Delete();
      item.Collection.Save();
      return "";
    }
  }
}
