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

public partial class Dialogs_SelectOrganization : BaseDialogPage
{
  private ReferenceType _referenceType;
  private int _referenceID1;
  private int _referenceID2 = -1;
  private int _parentID;

  protected void Page_Load(object sender, EventArgs e)
  {
    _parentID = UserSession.LoginUser.OrganizationID;
    try
    {
      _referenceID1 = int.Parse(Request["RefID1"]);
      _referenceType = (ReferenceType)int.Parse(Request["RefType"]);

      
      _referenceID2 = Request["RefID2"] != null ? int.Parse(Request["RefID2"]) : -1;

    }
    catch 
    {
    }

  }



  protected void gridOrganizations_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
  {
    Organizations organizations = new Organizations(UserSession.LoginUser);

    if (_referenceType == ReferenceType.Products || _referenceType == ReferenceType.ProductVersions || _referenceType == ReferenceType.OrganizationProducts)
    {
      organizations.LoadByParentID(_parentID, true);
    }
    else if (_referenceType == ReferenceType.Tickets)
    {
      organizations.LoadNotTicketCustomers(_parentID, _referenceID1);
    }

    gridOrganizations.DataSource = organizations.Table;
  }

  public override bool Save()
  {
    if (_referenceType == ReferenceType.Products || _referenceType == ReferenceType.ProductVersions || _referenceType == ReferenceType.OrganizationProducts)
    {
      Products products = new Products(UserSession.LoginUser);

      foreach (GridDataItem item in gridOrganizations.Items)
      {
        if (item.Selected)
        {
          if (_referenceID2 < 0)
            products.AddCustomer(int.Parse(item["OrganizationID"].Text), _referenceID1);
          else
            products.AddCustomer(int.Parse(item["OrganizationID"].Text), _referenceID1, _referenceID2);
        }
      }
      return true;
    }
    else if (_referenceType == ReferenceType.Tickets)
    {
      Tickets tickets = new Tickets(UserSession.LoginUser);

      foreach (GridDataItem item in gridOrganizations.Items)
      {
        if (item.Selected)
        {
          tickets.AddOrganization(int.Parse(item["OrganizationID"].Text), _referenceID1);
        }
      }
      return true;
    }

    return false;
  }
}
