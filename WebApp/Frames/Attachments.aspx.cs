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

public partial class Frames_Attachments : BaseFramePage
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
      Response.Write("No attachments to display.");
      Response.End();
      return;
      
    }
    fieldRefID.Value = _refID.ToString();
    fieldRefType.Value = ((int)_refType).ToString();
    Page.Culture = UserSession.LoginUser.CultureInfo.Name;
    paneToolbar.Visible = UserSession.CurrentUser.IsSystemAdmin || !UserSession.CurrentUser.IsAdminOnlyCustomers;
    gridAttachments.MasterTableView.Columns[0].Visible = UserSession.CurrentUser.IsSystemAdmin || !UserSession.CurrentUser.IsAdminOnlyCustomers;

  }

  protected void gridAttachments_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
  {
    gridAttachments.DataSource = TeamSupport.Data.Quarantine.WebAppQ.GetAttachmentsDataSource(UserSession.LoginUser, _refType, _refID);
  }


  protected void gridAttachments_ItemDataBound(object sender, GridItemEventArgs e)
  {
    if (e.Item is GridDataItem)
    {
      GridDataItem item = (GridDataItem)e.Item;
      string key = item.GetDataKeyValue("AttachmentID").ToString();

      ImageButton button = (ImageButton)item["ButtonOpen"].Controls[0];
      button.OnClientClick = "OpenRowAttachment('" + key + "'); return false;";

      button = (ImageButton)item["ButtonDelete"].Controls[0];
      button.OnClientClick = "DeleteRow('" + key + "'); return false;";
    } 
  }
}
