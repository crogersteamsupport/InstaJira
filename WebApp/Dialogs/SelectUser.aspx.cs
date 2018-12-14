using System;
using System.Collections.Generic;
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

public partial class Dialogs_SelectUser : BaseDialogPage
{

  private ReferenceType _referenceType;
  private int _referenceID;

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    _manager.AjaxSettings.AddAjaxSetting(gridUsers, gridUsers);

    _referenceID = int.Parse(Request["RefID"]);
    _referenceType = (ReferenceType) int.Parse(Request["RefType"]);
  }

  protected void gridUsers_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
  {
    Users users = new Users(UserSession.LoginUser);

    if (_referenceType == ReferenceType.Groups || _referenceType == ReferenceType.GroupUsers)
    {
      users.LoadByNotGroupID(_referenceID, UserSession.LoginUser.OrganizationID);
    }
    gridUsers.DataSource = users.Table;
  }

  public override bool Save()
  {

    if (_referenceType == ReferenceType.Groups || _referenceType == ReferenceType.GroupUsers)
    {
      Groups groups = new Groups(UserSession.LoginUser);

      foreach (GridDataItem item in gridUsers.Items)
      {
        if (item.Selected)
        {
          groups.AddGroupUser(int.Parse(item["UserID"].Text), _referenceID);
        }
      }
      return true;
    }

    return false;
  }
}
