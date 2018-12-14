using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;

public partial class Frames_NotePreview : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    int noteID;
    try
    {
      noteID = int.Parse(Request["NoteID"]);
    }
    catch (Exception)
    {
      Response.Write("");
      Response.End();
      return;
    }

    Note note = (Note)Notes.GetNote(UserSession.LoginUser, noteID);
    if (note != null)
    {
      divContent.InnerHtml = note.Description;
    
    }
  }
}
