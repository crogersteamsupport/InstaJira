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
  public class RestNotes
  {

    public static string GetNote(RestCommand command, ReferenceType refType, int refID, int noteID)
    {
      Note item = Notes.GetNote(command.LoginUser, noteID);
      if (item.RefType != refType && item.RefID != refID) throw new RestException(HttpStatusCode.Unauthorized);
      if (!DataUtils.IsReferenceValid(command.LoginUser, (ReferenceType)refType, refID)) throw new RestException(HttpStatusCode.Unauthorized);
      return item.GetXml("Note", true);
    }


    public static string GetNotes(RestCommand command, ReferenceType refType, int refID, bool orderByDateCreated = false)
    {
      if (!DataUtils.IsReferenceValid(command.LoginUser, (ReferenceType)refType, refID)) throw new RestException(HttpStatusCode.Unauthorized);

      Notes items = new Notes(command.LoginUser);
      if (orderByDateCreated)
      {
        items.LoadByReferenceType(refType, refID, "DateCreated DESC");
      }
      else
      {
        items.LoadByReferenceType(refType, refID);
      }
      return items.GetXml("Notes", "Note", true, command.Filters);
    }

    public static string AddNote(RestCommand command, ReferenceType refType, int refID)
    {
      if (!DataUtils.IsReferenceValid(command.LoginUser, (ReferenceType)refType, refID)) throw new RestException(HttpStatusCode.Unauthorized);
      Notes items = new Notes(command.LoginUser);
      Note item = items.AddNewNote();
      item.ReadFromXml(command.Data, true);
      item.RefType = refType;
      item.RefID = refID;
      item.Collection.Save();
      return item.GetXml("Note", true);
    }

    public static string RemoveNote(RestCommand command, ReferenceType refType, int refID, int noteID)
    {
      Note item = Notes.GetNote(command.LoginUser, noteID);
      if (item.RefType != refType && item.RefID != refID) throw new RestException(HttpStatusCode.Unauthorized);
      if (!DataUtils.IsReferenceValid(command.LoginUser, (ReferenceType)refType, refID)) throw new RestException(HttpStatusCode.Unauthorized);
      item.Delete();
      item.Collection.Save();
      return "";
    }
  }
}
