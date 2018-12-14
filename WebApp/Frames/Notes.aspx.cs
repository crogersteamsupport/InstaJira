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
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Telerik.Web.UI;


public partial class Frames_Notes : BaseFramePage
{
  private int _refID;
  private ReferenceType _refType;

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);

    try
    {
      _refType = (ReferenceType)int.Parse(Request["RefType"]);
      _refID = int.Parse(Request["RefID"]);
    }
    catch (Exception)
    {
      Response.Write("No notes to display.");
      Response.End();
      return;
    }

    if (!IsPostBack)
    {
      paneGrid.Height = new Unit(Settings.UserDB.ReadInt("CustomerNoteGridHeight", 250), UnitType.Pixel);

    }

    fieldRefID.Value = _refID.ToString();
    fieldRefType.Value = ((int)_refType).ToString();

    

  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    Page.Culture = UserSession.LoginUser.CultureInfo.Name;
    paneToolbar.Visible = UserSession.CurrentUser.IsSystemAdmin || !UserSession.CurrentUser.IsAdminOnlyCustomers;
    gridNotes.MasterTableView.Columns[0].Visible = UserSession.CurrentUser.IsSystemAdmin || !UserSession.CurrentUser.IsAdminOnlyCustomers;
    gridNotes.MasterTableView.Columns[1].Visible = UserSession.CurrentUser.IsSystemAdmin || !UserSession.CurrentUser.IsAdminOnlyCustomers;
    
  }
  protected void gridNotes_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
  {
    Notes notes = new Notes(UserSession.LoginUser);
    notes.LoadByReferenceType(_refType, _refID);
    gridNotes.DataSource = notes;
  }

  private int GetNoteID()
  {
    if (gridNotes.SelectedItems.Count < 1) return -1;
    GridItem item = gridNotes.SelectedItems[0];
    return (int)item.OwnerTableView.DataKeyValues[item.ItemIndex]["NoteID"]; ;
  }

  private bool SetNoteID(int id)
  {
    GridDataItem item = gridNotes.MasterTableView.FindItemByKeyValue("NoteID", id);
    if (item != null)
    {
      item.Selected = true;
      return true;
    }
    else
    {
      return false;
    }
  }
    
  protected void gridNotes_DataBound(object sender, EventArgs e)
  {
    if (!SetNoteID(Settings.UserDB.ReadInt("SelectedCustomerNoteID")) && gridNotes.Items.Count > 0)
    {
      gridNotes.Items[0].Selected = true;
    }
    frmNotePreview.Attributes["src"] = "NotePreview.aspx?NoteID=" + GetNoteID();


  }
  protected void gridNotes_ItemDataBound(object sender, GridItemEventArgs e)
  {
    if (e.Item is GridDataItem)
    {
      

      GridDataItem item = (GridDataItem)e.Item;
      string key = item.GetDataKeyValue("NoteID").ToString();

      int id = int.Parse(item["CreatorID"].Text);

      ImageButton button = (ImageButton)item["ButtonEdit"].Controls[0];
      button.OnClientClick = "EditRow('" + key + "'); return false;";

      button = (ImageButton)item["ButtonDelete"].Controls[0];
      button.OnClientClick = "DeleteRow('" + key + "'); return false;";
      if (!UserSession.CurrentUser.IsSystemAdmin && id != UserSession.CurrentUser.UserID)
      {
        button.Attributes["src"] = "";
        button.Attributes["alt"] = "";
      }
    } 

  }
}
