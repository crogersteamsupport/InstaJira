<%@ Page Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="Products.aspx.cs" Inherits="Frames_Products" Title="Untitled Page" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <telerik:RadSplitter ID="splMain" runat="server" Height="100%" Width="100%" VisibleDuringInit="false"
    BorderSize="0" Orientation="Horizontal">
    <telerik:RadPane ID="paneToolBar" runat="server" Height="32px" Scrolling="None">
      <telerik:RadToolBar ID="tbMain" runat="server" CssClass="NoRoundedCornerEnds" Width="100%" OnClientButtonClicked="ButtonClicked">
        <Items>
          <telerik:RadToolBarButton runat="server" Text="New Product" Value="NewProduct" ImageUrl="~/images/icons/new.png"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Edit Product" Value="EditProduct" ImageUrl="~/images/icons/edit.png"> </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Delete Product" Value="DeleteProduct" ImageUrl="~/images/icons/trash.png"> </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" IsSeparator="true"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="New Version" Value="NewVersion" ImageUrl="~/images/icons/new.png"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Edit Version" Value="EditVersion" ImageUrl="~/images/icons/edit.png"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Delete Version" Value="DeleteVersion" ImageUrl="~/images/icons/trash.png"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" IsSeparator="true" Value="CustomerSeparator"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Associate Customer" Value="AssociateCustomer" ImageUrl="~/images/icons/add.png"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Associate Customers" Value="AssociateCustomers" ImageUrl="~/images/icons/add.png" Visible="false"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="Help" ImageUrl="~/images/icons/Help.png"
            Text="Help" ToolTip="Help." Visible="false"></telerik:RadToolBarButton>
        </Items>
      </telerik:RadToolBar>
    </telerik:RadPane>
    <telerik:RadPane ID="paneBody" runat="server" Height="100%" Scrolling="None">
      <telerik:RadSplitter ID="splBody" runat="server" Height="100%" Width="100%"
        BorderSize="0">
        <telerik:RadPane ID="paneGrid" runat="server" Width="250px" Scrolling="None" OnClientResized="GridResized" BackColor="#ffffff">
        <telerik:RadTreeView ID="tvProducts" runat="server" BorderWidth="0px" PersistLoadOnDemandNodes="false" LoadingStatusPosition="BelowNodeText"
        OnClientNodeClicked="NodeClicked" OnClientNodeCollapsed="NodeCollapsed" EnableViewState="false" Width="100%" Height="100%">
        <WebServiceSettings Path="../Services/PrivateServices.asmx" Method="GetVersionNodes"/>
        </telerik:RadTreeView>
        </telerik:RadPane>
        <telerik:RadSplitBar ID="RadSplitBar1" runat="server" />
        <telerik:RadPane ID="paneContent" runat="server" Width="100%" Scrolling="None">
          <telerik:RadSplitter ID="splContent" runat="server" Height="100%" Width="100%"
            BorderSize="0" Orientation="Horizontal">
            <telerik:RadPane ID="paneCaption" runat="server" Scrolling="None" Height="35px" BackColor="#BFDBFF">
              <div style="width:100%; height: 20px; padding: 10px 15px;">
                <span id="captionSpan" runat="server" style="font-weight: bold; font-size: 16px;">Test</span>
                <span id="spanVersionFilter" runat="server">
                  
                <asp:RadioButton ID="rbReported" runat="server" Text="Reported Tickets" Checked="true" GroupName="VersionFilter" onclick="LoadContentPage();" />
                <asp:RadioButton ID="rbResolved" Text="Resolved Tickets" runat="server" GroupName="VersionFilter" onclick="LoadContentPage();"/>
                </span>
              </div>
            </telerik:RadPane>
            <telerik:RadPane ID="paneTabs" runat="server" Scrolling="None" Height="29px" BackColor="#BFDBFF">
              <div style="padding-top: 3px;">
                <telerik:RadTabStrip ID="tsMain" runat="server" SelectedIndex="0" 
                  OnClientTabSelected="TabSelected" ShowBaseLine="True" Width="100%" PerTabScrolling="True" ScrollChildren="True">
                </telerik:RadTabStrip>
              </div>
            </telerik:RadPane>
            <telerik:RadPane ID="paneFrame" runat="server" Scrolling="None" Height="100%" BackColor="#DBE6F4">
              <iframe id="frmOrganizations" runat="server" scrolling="no" src="" frameborder="0"
                height="100%" width="100%"></iframe>
            </telerik:RadPane>
          </telerik:RadSplitter>
        </telerik:RadPane>
      </telerik:RadSplitter>
    </telerik:RadPane>
  </telerik:RadSplitter>
<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

    <script type="text/javascript" language="javascript">


      var _lastRefreshTime = new Date();

      function refreshData(overrideTime) {
        if (_lastRefreshTime != null && !overrideTime) {
          var now = new Date();
          var diff = (now - _lastRefreshTime) / 1000;
          if (diff < 300) return;
        }

        _lastRefreshTime = new Date();

        window.location = window.location;

      }
      function GridResized(sender, args) {
        top.privateServices.SetUserSetting('ProductsTreeWidth', sender.get_width());
      }

      function TabSelected(sender, args) {
        var tab = args.get_tab();
        var index = tab.get_index();
        var text = tab.get_text();

        top.privateServices.SetUserSetting('SelectedProductTabIndex', index);
        if (index < 6) top.Ts.System.logAction('Products - Tab Selected (' + text + ')');
        else if (text == 'Knowledge Base') top.Ts.System.logAction('Products - Tab Selected (KnowledgeBase)');
        else top.Ts.System.logAction('Products - Tab Selected (Ticket Type)');

        LoadContentPage();


      }

      function SetVersionFilters() {
        var index = GetSelectedTab().get_index();
        var span = $get("<%=spanVersionFilter.ClientID %>");
        var node = GetVersionNode(GetSelectedNode());
        if (index > 3 && node != null)
          span.setAttribute('style', 'display:inline;');
        else
          span.setAttribute('style', 'display:none;');
      
      }

      function NodeCollapsed(sender, args) {
        var node = args.get_node();
        node.get_nodes().clear();
        node.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.WebService);
 
        node.set_selected(true);
        SetNode(node);
      }

      function NodeClicked(sender, args) {
        SetNode(args.get_node());
      }

      function SetNode(node) {
        var productNode = GetProductNode(node);
        var versionNode = GetVersionNode(node);

        top.privateServices.SetUserSetting('SelectedProductID', productNode.get_value());

        var toolBar = $find("<%=tbMain.ClientID %>");
        //var items = toolBar.get_allItems();
        toolBar.findItemByValue("EditVersion").set_enabled(versionNode != null);
        toolBar.findItemByValue("DeleteVersion").set_enabled(versionNode != null);

        //items[5].set_enabled(versionNode != null);
        //items[6].set_enabled(versionNode != null);

        if (versionNode != null) {
          top.privateServices.SetUserSetting('SelectedVersionID', versionNode.get_value());
          
        }
        else {
          top.privateServices.SetUserSetting('SelectedVersionID', -1);
        }

        LoadContentPage();
      
      }

      function GetProductNode(node) {
        if (node.get_parent() != node.get_treeView()) {
          return node.get_parent();
        }
        else {
          return node;
        }
      }

      function GetVersionNode(node) {
        if (node.get_parent() != node.get_treeView()) {
          return node;
        }
        else {
          return null;
        }

      }

      function GetSelectedNode() {
        var tree = $find("<%= tvProducts.ClientID %>");
        return tree.get_selectedNode();
      }
      
      function GetSelectedTab() {
        var strip = $find("<%=tsMain.ClientID %>");
        return strip.get_selectedTab();
      }

      function LoadContentPage() {
        showLoadingPanel("<%=frmOrganizations.ClientID %>");

        SetVersionFilters();
        var tab = GetSelectedTab();
        var index = tab.get_index();
        var url = tab.get_value();
        var node = GetSelectedNode();
        var productNode = GetProductNode(node);
        var versionNode = GetVersionNode(node);

        var caption = productNode.get_text();
        if (versionNode != null) {
          caption = caption + ' - ' + versionNode.get_text();
        }
        var span = $get("<%=captionSpan.ClientID %>");
        span.innerHTML = caption;

        if (index == 1) {
          if (versionNode == null) {
            url = 'ProductVersions.aspx?ProductID=' + productNode.get_value();
          }
          else {
            url = '../vcr/1_9_0/Pages/ProductVersion.html?VersionID=' + versionNode.get_value();
          }
        }
        else if (index == 3) {
          url = tab.get_value();
          if (versionNode == null) {
            url = url + 'RefType=13&RefID=' + productNode.get_value();
          }
          else {
            url = url + 'RefType=14&RefID=' + versionNode.get_value();
          }
        }
        else if (index == 4) {
          url = url + 'pagetype=1&pageid=' + productNode.get_value();
        }
        else if (index > 4) {
          url = url + 'tf_ProductID=' + productNode.get_value();

          if (versionNode != null) {
            var radio = $get("<%=rbReported.ClientID %>");
            if (radio.checked)
              url = url + '&tf_ReportedVersionID=' + versionNode.get_value();
            else
              url = url + '&tf_SolvedVersionID=' + versionNode.get_value();
          }
        }
        else {
          url = url + 'ProductID=' + productNode.get_value();

          if (versionNode != null && index > 0) {
            url = url + '&VersionID=' + versionNode.get_value();
          }
        }
        
        var frame = $get("<%=frmOrganizations.ClientID %>");
        frame.setAttribute('src', url);
        hideLoadingPanel("<%=frmOrganizations.ClientID %>");

        return false;
      }
      
      
      function ScrollToSelectedNode() {
        var node = GetSelectedNode();
        if (node != null) {
          window.setTimeout(function() { node.scrollIntoView(); }, 200);
        }
      }


      function DialogClosed(sender, args) {
        RefreshTree();
        sender.remove_close(DialogClosed);
      }

      function RefreshTree() {
        __doPostBack();
      }

      function ShowDialog(wnd) {
        wnd.add_close(DialogClosed);
        wnd.show();
      }

      function ButtonClicked(sender, args) {
        var button = args.get_item();
        var value = button.get_value();
        if (value == 'NewProduct') {
          ShowDialog(top.GetProductDialog(true, RefreshTree));
          top.Ts.System.logAction('Products - New Product Dialog Opened');
        }
        else if (value == 'EditProduct') {
        ShowDialog(top.GetProductDialog(false, GetProductNode(GetSelectedNode()).get_value(), RefreshTree));
        top.Ts.System.logAction('Products - Edit Product Dialog Opened');
      }
      else if (value == 'DeleteProduct') {
        if (confirm('Are you sure you would like to PERMANENTLY delete this product, along with the inventory?')) {
          if (confirm('If you press OK then the product and inventory will be permanently deleted and THERE IS NO WAY TO REVERSE THIS!  Please hit CANCEL if you are at all unsure that you want to do this.')) {
          top.privateServices.DeleteProduct(GetProductNode(GetSelectedNode()).get_value(), RefreshTree);
          top.Ts.System.logAction('Products - Product Deleted');
        }
      }
        }
        else if (value == 'NewVersion') {
        ShowDialog(top.GetVersionDialog(true, GetProductNode(GetSelectedNode()).get_value(), RefreshTree));
        top.Ts.System.logAction('Products - New Version Dialog Opened');
      }
        else if (value == 'EditVersion') {
          ShowDialog(top.GetVersionDialog(false, GetVersionNode(GetSelectedNode()).get_value(), RefreshTree));
          top.Ts.System.logAction('Products - Edit Version Dialog Opened');

        }
        else if (value == 'DeleteVersion') {
        if (confirm('Are you sure you would like to PERMANENTLY delete this version?')) {
          top.privateServices.DeleteVersion(GetVersionNode(GetSelectedNode()).get_value(), RefreshTree);
          top.Ts.System.logAction('Products - Version Deleted');
        }
        }
        else if (value == 'AssociateCustomer') {
          var productNode = GetProductNode(GetSelectedNode());
          var versionNode = GetVersionNode(GetSelectedNode());
          top.Ts.System.logAction('Products - Associate Customer');

          if (versionNode == null) {
            ShowDialog(top.GetOrganizationProductDialog(null, null, productNode.get_value(), null, RefreshTree));
          }
          else {
            ShowDialog(top.GetOrganizationProductDialog(null, null, productNode.get_value(), versionNode.get_value(), RefreshTree));
          }

        }
        else if (value == 'AssociateCustomers') {
          top.Ts.System.logAction('Products - Associate Customers');
          var productNode = GetProductNode(GetSelectedNode());
          var versionNode = GetVersionNode(GetSelectedNode());
          if (versionNode == null) {
            ShowDialog(top.GetSelectOrganizationDialog(13, productNode.get_value(), null, RefreshTree));
          }
          else{
            ShowDialog(top.GetSelectOrganizationDialog(13, productNode.get_value(), versionNode.get_value(), RefreshTree));
          }
        
        }
        else if (value == 'Help') {
          top.ShowHelpDialog(140);
        }
      } 
      
      function reload() {
        window.location.reload(true);
      }
    </script>

  </telerik:RadCodeBlock>  
  
</asp:Content>

