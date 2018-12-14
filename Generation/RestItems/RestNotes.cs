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
    public static string GetNote(RestCommand command, int noteID)
    {
      Note note = Notes.GetNote(command.LoginUser, noteID);
      if (note.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return note.GetXml("Note", true);
    }
    
    public static string GetNotes(RestCommand command)
    {
      Notes notes = new Notes(command.LoginUser);
      notes.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return notes.GetXml("Notes", "Note", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
