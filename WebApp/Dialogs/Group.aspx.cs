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

public partial class Dialogs_Group : BaseDialogPage

{
  private int _groupID = -1;

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    if (Request["GroupID"] != null)
    {
      _groupID = int.Parse(Request["GroupID"]);
    }

    if (!IsPostBack)
    {
        Organization organization = Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
        if (organization.UseProductFamilies)
        {
            LoadProductFamilies();
            divProductFamily.Style["display"] = "block";
        }

        if (_groupID > -1) LoadGroup(_groupID);
    }
  }

    public void LoadProductFamilies()
    {
        ProductFamilies productFamilies = new ProductFamilies(UserSession.LoginUser);
        productFamilies.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
        cmbProductFamilies.Items.Add(new RadComboBoxItem("Without Product Line", "-1"));

        foreach (ProductFamily productFamily in productFamilies)
        {
            cmbProductFamilies.Items.Add(new RadComboBoxItem(productFamily.Name, productFamily.ProductFamilyID.ToString()));
        }
    }

    private void LoadGroup(int groupID)
  {
    Groups groups = new Groups(UserSession.LoginUser);
    groups.LoadByGroupID(groupID);
    if (groups.IsEmpty) return;

    if (groups[0].OrganizationID != UserSession.LoginUser.OrganizationID)
    {
      Response.Write("Invalid Request");
      Response.End();
      return;
    }
    textName.Text = groups[0].Name;
    textDescription.Text = groups[0].Description;
    if (groups[0].ProductFamilyID != null)
    {
        cmbProductFamilies.SelectedValue = ((int)groups[0].ProductFamilyID).ToString();
    }
  }

  public override bool Save()
  {
    Group group;
    Groups groups = new Groups(UserSession.LoginUser);;
    if (_groupID < 0)
    {
      group = groups.AddNewGroup();
      group.OrganizationID = UserSession.LoginUser.OrganizationID;
    }
    else
    {
      groups.LoadByGroupID(_groupID);
      if (groups.IsEmpty) return false;
      group = groups[0];
    }

    group.Name = textName.Text;
    group.Description = textDescription.Text;
    if (cmbProductFamilies.Items.Count > 0 && cmbProductFamilies.SelectedValue != "-1")
    {
      group.ProductFamilyID = Convert.ToInt32(cmbProductFamilies.SelectedValue);
    }
    else
    {
        group.ProductFamilyID = null;
    }
    group.Collection.Save();
    return true;
  }
}
