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

public partial class Dialogs_SelectGroup : BaseDialogPage
{

  private ReferenceType _referenceType;
  private int _referenceID;

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    _referenceID = int.Parse(Request["RefID"]);
    _referenceType = (ReferenceType)int.Parse(Request["RefType"]);

  }

  protected void gridGroups_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
  {
    Groups groups = new Groups(UserSession.LoginUser);

    if (_referenceType == ReferenceType.Users || _referenceType == ReferenceType.GroupUsers)
    {
      groups.LoadByNotUserID(_referenceID, UserSession.LoginUser.OrganizationID);
    }

    gridGroups.DataSource = groups.Table;
  }

  public override bool Save()
  {

    if (_referenceType == ReferenceType.Users || _referenceType == ReferenceType.GroupUsers)
    {
      Users users = new Users(UserSession.LoginUser);

      foreach (GridDataItem item in gridGroups.Items)
      {
        if (item.Selected)
        {
          users.AddUserGroup(_referenceID, int.Parse(item["GroupID"].Text));
        }
      }
      return true;
    }

    return false;
  }
}
