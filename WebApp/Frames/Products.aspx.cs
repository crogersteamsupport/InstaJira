using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.UI.HtmlControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Telerik.Web.UI;

public partial class Frames_Products : BaseFramePage
{

  private int SelectedProductID
  {
    get
    {
      return Settings.UserDB.ReadInt("SelectedProductID", -1);
    }
    set
    {
      Settings.UserDB.WriteInt("SelectedProductID", value);
    }
  }
  private int SelectedVersionID
  {
    get
    {
      return Settings.UserDB.ReadInt("SelectedVersionID", -1);
    }
    set
    {
      Settings.UserDB.WriteInt("SelectedVersionID", value);
    }
  }

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);
    CreateNavButtons();

    if (!IsPostBack)
    {
      tsMain.SelectedIndex = Settings.UserDB.ReadInt("SelectedProductTabIndex", 0);
      paneGrid.Width = new Unit(Settings.UserDB.ReadInt("ProductsTreeWidth", 200), UnitType.Pixel);
    }
    Page.Culture = UserSession.LoginUser.CultureInfo.Name;

  }

  protected void Page_Load(object sender, EventArgs e)
  {

    LoadRootNodes();

    HtmlGenericControl body = (HtmlGenericControl)Page.Master.FindControl("bodyFrame");
    body.Attributes["onload"] = "setTimeout('ScrollToSelectedNode()', 20);";

    if (GetVersionID(tvProducts.SelectedNode) < 0)
    {
      tbMain.Items.FindItemByValue("EditVersion").Enabled = false;
      tbMain.Items.FindItemByValue("DeleteVersion").Enabled = false;
    }

    if (!UserSession.CurrentUser.IsSystemAdmin)
    {
      tbMain.Items.FindItemByValue("DeleteProduct").Enabled = false;
      tbMain.Items.FindItemByValue("DeleteVersion").Enabled = false;
    }

    if (UserSession.CurrentUser.ProductType == ProductType.Express || UserSession.CurrentUser.ProductType == ProductType.BugTracking)
    {
      tbMain.Items.FindItemByValue("CustomerSeparator").Enabled = false;
      tbMain.Items.FindItemByValue("AssociateCustomer").Enabled = false;
      tbMain.Items.FindItemByValue("AssociateCustomers").Enabled = false;
    }
  }

  private void SetNode(RadTreeNode node)
  {
    if (node == null)
    {
      captionSpan.InnerHtml = "[No Product Selected]";
      return;
    }
    else if (node.ParentNode != null)
    {
      captionSpan.InnerHtml = node.ParentNode.Text + " - " + node.Text;
    }
    else
    {
      captionSpan.InnerHtml = node.Text;
    }

    if (tsMain.SelectedTab == null) tsMain.SelectedIndex = 0;
    string url = tsMain.SelectedTab.Value;
    int productID = GetProductID(node);
    int versionID = GetVersionID(node);
    
    if (tsMain.SelectedIndex == 1)
    {
      if (versionID < 0)
        url = "ProductVersions.aspx?ProductID=" + productID.ToString();
      else
        url = "../vcr/1_9_0/Pages/ProductVersion.html?VersionID=" + versionID.ToString();
    }
    else if (tsMain.SelectedIndex == 3)
    {
      if (versionID < 0)
        url = url + "RefType=13&RefID=" + productID.ToString();
      else
        url = url + "RefType=14&RefID=" + versionID.ToString();
    }
    else if (tsMain.SelectedIndex == 4)
    {
      url = url + "pagetype=1&pageid=" + productID.ToString();
    }
    else if (tsMain.SelectedIndex > 4)
    {
      url = url + "tf_ProductID=" + productID.ToString();

      if (versionID > -1)
      {
        if (rbReported.Checked)
          url = url + "&tf_ReportedVersionID=" + versionID.ToString();
        else
          url = url + "&tf_SolvedVersionID=" + versionID.ToString();
      }

    }
    else
    {

      url = url + "ProductID=" + productID.ToString();

      if (versionID > -1 && tsMain.SelectedIndex > 0)
        url = url + "&VersionID=" + versionID.ToString();
    }

    frmOrganizations.Attributes["src"] = url;

    if (tsMain.SelectedIndex > 3 && versionID > -1)
      spanVersionFilter.Attributes["style"] = "display:inline;";
    else
      spanVersionFilter.Attributes["style"] = "display:none;";
  }


  private int GetProductID(RadTreeNode node)
  {
    if (node == null) return -1;

    if (node.ParentNode == null)
    {
      return int.Parse(node.Value);
    }
    else
    {
      return int.Parse(node.ParentNode.Value);
    }
  }

  private int GetVersionID(RadTreeNode node)
  {
    if (node == null) return -1;

    if (node.ParentNode == null)
    {
      return -1;
    }
    else
    {
      return int.Parse(node.Value);
    }
  }

  private void SelectNode(int productID, int versionID)
  {
    if (tvProducts.Nodes.Count < 1) return;

    RadTreeNode productNode = null;

    foreach (RadTreeNode node in tvProducts.Nodes)
    {
      if (productID == int.Parse(node.Value))
      {
        productNode = node;
        productNode.Expanded = true;
        break;
      }
    }

    if (productNode == null)
    {
      tvProducts.Nodes[0].Selected = true;
      tvProducts.Nodes[0].Expanded = true;
      return;
    }

    if (versionID > -1)
    {
      foreach (RadTreeNode node in productNode.Nodes)
      {
        if (versionID == int.Parse(node.Value))
        {
          node.Selected = true;
          break;
        }
      }
    }
    else
    {
      productNode.Selected = true;
    }

    SetNode(tvProducts.SelectedNode);

  }

  private void LoadRootNodes()
  {
    int productID = SelectedProductID;// GetProductID(tvProducts.SelectedNode);
    int versionID = SelectedVersionID;// GetVersionID(tvProducts.SelectedNode);

    tvProducts.Nodes.Clear();
    using (Products products = new Products(UserSession.LoginUser))
    {
      products.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);

      foreach (Product product in products)
      {
        RadTreeNode node = new RadTreeNode();
        node.Text = product.Name;
        node.Value = product.ProductID.ToString();
        node.ExpandMode = TreeNodeExpandMode.WebService;
        tvProducts.Nodes.Add(node);

        if (product.ProductID == productID)
        {
          node.ExpandMode = TreeNodeExpandMode.ServerSide;
          node.Expanded = true;
          node.Selected = true;
        }
      }
    }

    if (tvProducts.SelectedNode == null && tvProducts.Nodes.Count > 0)
    {
      tvProducts.Nodes[0].Selected = true;
    }

    LoadVersionNodes(tvProducts.SelectedNode, versionID);

    SetNode(tvProducts.SelectedNode);
  }

  private void LoadVersionNodes(RadTreeNode node, int selectedVersionID)
  {
    if (node == null) return;

    node.Nodes.Clear();

    using (ProductVersions versions = new ProductVersions(UserSession.LoginUser))
    {
      versions.LoadByProductID(GetProductID(node));

      if (!versions.IsEmpty)
      {
        foreach (ProductVersion version in versions)
        {
          RadTreeNode vnode = new RadTreeNode();
          vnode.Text = version.VersionNumber;
          vnode.Value = version.ProductVersionID.ToString();
          vnode.ExpandMode = TreeNodeExpandMode.WebService;
          node.Nodes.Add(vnode);
          if (selectedVersionID == version.ProductVersionID)
          {
            vnode.Selected = true;
            SetNode(vnode);

          }
        }

      }
    }
  }


  private void CreateNavButtons()
  {
    tsMain.Tabs.Clear();

    tsMain.Tabs.Add(new RadTab("Product Information", "ProductInformation.aspx?"));
    tsMain.Tabs.Add(new RadTab("Version Information", "../vcr/1_9_0/Pages/ProductVersion.html?"));
    tsMain.Tabs.Add(new RadTab("Customers", "ProductOrganizations.aspx?"));

    RadTab tab = new RadTab("History", "History.aspx?");
    tsMain.Tabs.Add(tab);
    tsMain.Tabs.Add(new RadTab("Water Cooler", "../vcr/1_9_0/Pages/Watercooler.html?"));
    tsMain.Tabs.Add(new RadTab("All Tickets", "TicketTabsAll.aspx?"));

    TicketTypes ticketTypes = new TicketTypes(UserSession.LoginUser);
    ticketTypes.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, UserSession.CurrentUser.ProductType);
    foreach (TicketType ticketType in ticketTypes)
    {
      tsMain.Tabs.Add(new RadTab(ticketType.Name, "TicketTabsProduct.aspx?tf_TicketTypeID=" + ticketType.TicketTypeID.ToString() + "&"));
    }
    tsMain.Tabs.Add(new RadTab("Knowledge Base", "../vcr/1_9_0/Pages/TicketGrid.html?tf_IsKnowledgeBase=true&"));

  }


}
