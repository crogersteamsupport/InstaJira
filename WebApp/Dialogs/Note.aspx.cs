using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using TeamSupport.WebUtils;
using TeamSupport.Data;
using Telerik.Web.UI;
using System.IO;

public partial class Dialogs_Note : BaseDialogPage
{
  private ReferenceType _referenceType;
  private int _referenceID;
  private int _noteID = -1;

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    if (Request["NoteID"] != null)
    {
      _noteID = int.Parse(Request["NoteID"]);
    }
    else
    {
      _referenceID = int.Parse(Request["RefID"]);
      _referenceType = (ReferenceType)int.Parse(Request["RefType"]);

    }
    
    if (!IsPostBack && _noteID > -1) LoadNote(_noteID);
  }
  
  private void LoadNote(int noteID)
  {
    Note note = (Note)Notes.GetNote(UserSession.LoginUser, noteID);
    if (note == null) return;
    
    textTitle.Text = note.Title;
    editorDescription.Content = note.Description;
  }

  public override bool Save()
  {
    if (string.IsNullOrEmpty(textTitle.Text))
    {
      _manager.Alert("Please enter a title for this note.");
      return false;
    }

    if (string.IsNullOrEmpty(editorDescription.Content))
    {
      _manager.Alert("Please enter a note.");
      return false;
    }
    
    
    Note note = null;
    
    if (_noteID > -1)
    {
      note = (Note)Notes.GetNote(UserSession.LoginUser, _noteID);
    }
    else
    {
      note = (new Notes(UserSession.LoginUser)).AddNewNote();
      note.RefID = _referenceID;
      note.RefType = _referenceType;
    }
    
    if (note != null)
    {
      note.Description = editorDescription.Content;
      note.Title = textTitle.Text;
      note.Collection.Save();
    }
    

    return true;
    
  }
}

