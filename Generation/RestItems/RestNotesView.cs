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
  
  public class RestNotesView
  {
    public static string GetNotesViewItem(RestCommand command, int noteID)
    {
      NotesViewItem notesViewItem = NotesView.GetNotesViewItem(command.LoginUser, noteID);
      if (notesViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return notesViewItem.GetXml("NotesViewItem", true);
    }
    
    public static string GetNotesView(RestCommand command)
    {
      NotesView notesView = new NotesView(command.LoginUser);
      notesView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return notesView.GetXml("NotesView", "NotesViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
