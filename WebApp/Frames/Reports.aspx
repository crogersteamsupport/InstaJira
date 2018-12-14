<%@ Page Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true"
  CodeFile="Reports.aspx.cs" Inherits="Frames_Reports" Title="Untitled Page" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadAjaxManager ID="ajaxManager" runat="server" UpdatePanelsRenderMode="Inline">
    <AjaxSettings>
      <telerik:AjaxSetting AjaxControlID="reportTree">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="reportTree" />
          <telerik:AjaxUpdatedControl ControlID="gridReport" />
        </UpdatedControls>
      </telerik:AjaxSetting>
      <telerik:AjaxSetting AjaxControlID="gridReport">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="gridReport" />
        </UpdatedControls>
      </telerik:AjaxSetting>
    </AjaxSettings>
  </telerik:RadAjaxManager>
  <telerik:RadSplitter ID="RadSplitter3" runat="server" Height="100%" Width="100%"
    BorderSize="0" Orientation="Horizontal">
    <telerik:RadPane ID="RadPane5" runat="server" Height="32px" Scrolling="None">
      <telerik:RadToolBar ID="tbUser" runat="server" CssClass="NoRoundedCornerEnds" Width="100%"
        OnClientButtonClicked="ButtonClicked">
        <Items>
          <telerik:RadToolBarButton runat="server" Text="New" Value="New" ImageUrl="~/images/icons/new.png">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Edit" Value="Edit" Enabled="false"
            ImageUrl="~/images/icons/edit.png"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Delete" Value="Delete" Enabled="false"
            ImageUrl="~/images/icons/trash.png"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Export to PDF" Enabled="false" ImageUrl="~/images/icons/Export.png"
            Value="ExportPDF" Visible="false"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Export to Excel" Enabled="false" Visible="false" ImageUrl="~/images/icons/Export.png"
            Value="ExportExcel"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Export" Enabled="false" ImageUrl="~/images/icons/Export.png"
            Value="ExportCSV"></telerik:RadToolBarButton>
            <telerik:RadToolBarButton runat="server" Text="Add to Favorites" Value="Favorite" Enabled="false" ImageUrl="~/images/icons/Favorites.png"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="Help" ImageUrl="~/images/icons/Help.png"
            Text="Help" ToolTip="Help." Visible="false"></telerik:RadToolBarButton>
            
        </Items>
      </telerik:RadToolBar>
    </telerik:RadPane>
    <telerik:RadPane ID="RadPane6" runat="server" Height="100%" Scrolling="None">
      <telerik:RadSplitter ID="RadSplitter1" runat="server" Height="100%" Width="100%"
        BorderSize="0">
        <telerik:RadPane ID="paneGrid" runat="server" Width="200px" Scrolling="None" OnClientResized="GridResized">
          <div class="stretchContentHolderDiv">            
        <telerik:RadTreeView ID="reportTree" runat="server" BorderWidth="0px" PersistLoadOnDemandNodes="true" LoadingStatusPosition="BelowNodeText" OnClientNodeClicked="NodeClicked" OnClientNodeCollapsed="NodeCollapsed" EnableViewState="false" Width="100%" Height="100%">
            <Nodes>
                <telerik:RadTreeNode runat="server" Text="Stock Reports" ExpandMode="WebService" Value="0"></telerik:RadTreeNode>
                <telerik:RadTreeNode runat="server" Text="Graphical Reports" ExpandMode="WebService" Value="3" />
                <telerik:RadTreeNode runat="server" Text="Custom Reports" ExpandMode="WebService" Value="1"></telerik:RadTreeNode>
                <telerik:RadTreeNode runat="server" Text="My Favorites" ExpandMode="WebService" Value="2" />
               </Nodes>
            <WebServiceSettings Path="../Services/PrivateServices.asmx" Method="GetReportNodes"/>
        </telerik:RadTreeView>
          </div>
        </telerik:RadPane>
        <telerik:RadSplitBar ID="RadSplitBar1" runat="server" />
        <telerik:RadPane ID="RadPane2" runat="server" Width="100%" Scrolling="None">
          <iframe id="frmReport" runat="server" scrolling="no" src="" frameborder="0" height="100%"
            width="100%"></iframe>
        </telerik:RadPane>
      </telerik:RadSplitter>
    </telerik:RadPane>
  </telerik:RadSplitter>
  <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

    <script type="text/javascript" language="javascript">

      var _lastRefreshTime = new Date();

      function refreshData() {
        if (_lastRefreshTime != null) {
          var now = new Date();
          var diff = (now - _lastRefreshTime) / 1000;
          if (diff < 300) return;
        }

        _lastRefreshTime = new Date();

        window.location = window.location;

    }

    function refreshNode(nodeValue) {
        var tree = $find("<%=reportTree.ClientID %>");
        var thisNode = tree.findNodeByValue(nodeValue);
        
       tree.trackChanges();
       thisNode.get_nodes().clear();
       thisNode.set_expanded(false);
       tree.commitChanges();
       thisNode.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.WebService);

       thisNode.expand();
    }

      function GridResized(sender, args) {
        top.privateServices.SetUserSetting('ReportsGridWidth', sender.get_width());
      }

      function SetCanEdit(result) {
        var toolBar = $find("<%=tbUser.ClientID %>");
        toolBar.findItemByValue("Edit").set_enabled(result);
    }

    function SetCanDelete(result) {
        var toolBar = $find("<%=tbUser.ClientID %>");
        toolBar.findItemByValue("Delete").set_enabled(result);
    }

    function SetCanEditOrDelete(result) {
        SetCanDelete(result);
        SetCanEdit(result);
    }

    function SetIsReport(result) {
        var toolBar = $find("<%=tbUser.ClientID %>");
        var itemFav = toolBar.findItemByValue("Favorite");
        var itemExpCSV = toolBar.findItemByValue("ExportCSV");
        var itemExpEx = toolBar.findItemByValue("ExportExcel");

        itemFav.set_enabled(result);
        itemExpCSV.set_enabled(result);
        if (itemExpEx != null) itemExpEx.set_enabled(result);
    }

    function SetIsFavorite(result) {
        var toolBar = $find("<%=tbUser.ClientID %>");
        var itemFav = toolBar.findItemByValue("Favorite");
        itemFav.set_enabled(true);

        if (result) {
            itemFav.set_text("Remove Favorite");
        }
        else { itemFav.set_text("Add to Favorites"); }
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
          //prevent deletion/editing until we re-enable
          SetCanEditOrDelete(false);
          var typeNode = GetTypeNode(node);
          var reportNode = GetReportNode(node);

          top.privateServices.SetUserSetting('SelectedReportTypeOldID', typeNode.get_value());

          if (reportNode != null) {

              var id = reportNode.get_value();

              if (typeNode.get_value() == 2) { //don't allow deletion when in favorites
                  SetCanDelete(false);
                  top.privateServices.CanEditReport(id, SetCanEdit);
              }
              else {
                  top.privateServices.CanEditReport(id, SetCanEditOrDelete);
              }

              top.privateServices.SetUserSetting('SelectedReportID', id);
              top.privateServices.IsFavoriteReport(id, SetIsFavorite);
              SetIsReport(true);

              PageMethods.GetCsvUrl(function (result) {
                  var toolBar = $find("<%= tbUser.ClientID %>");
                  var button = toolBar.findItemByValue("ExportCSV");
                  button.set_navigateUrl(result);
              });


          }
          else {
              top.privateServices.SetUserSetting('SelectedReportID', -1);
              SetCanEdit(false);
              SetCanDelete(false);
              SetIsReport(false);
          }

          LoadContentPage();
      }

      function GetTypeNode(node) {
          if (node.get_parent() != node.get_treeView()) {
              return node.get_parent();
          }
          else {
              return node;
          }
      }

      function GetReportNode(node) {
          if (node.get_parent() != node.get_treeView()) {
              return node;
          }
          else {
              return null;
          }

      }

      function GetSelectedReportID() {
          var tree = $find("<%= reportTree.ClientID %>");
          return tree.get_selectedNode().get_value();
      }

      function GetSelectedExternalURL() {
          var tree = $find("<%= reportTree.ClientID %>");
          var url = tree.get_selectedNode().get_attributes().getAttribute('ExternalURL');
        if (!url || url == '' || url === undefined) {
          return '../Report/ReportResults.aspx'
        }
        else {
          return url;
        }
      }
      
      function LoadContentPage() {
        showLoadingPanel("<%=frmReport.ClientID %>");

        var id = GetSelectedReportID();
        var frame = $get("<%=frmReport.ClientID %>");

        if (id < 4) { //this tells us it's a report type
            frame.setAttribute('src', '');
        }
        else {
            if (GetSelectedExternalURL().indexOf('?') > 0) {
                frame.setAttribute('src', GetSelectedExternalURL() + '&ReportID=' + id);
            }
            else {
                frame.setAttribute('src', GetSelectedExternalURL() + '?ReportID=' + id);
            }
        }

        hideLoadingPanel("<%=frmReport.ClientID %>");

      }

      function ButtonClicked(sender, args) {
        var button = args.get_item();
        var value = button.get_value();
        if (value == 'ExportPDF') { ExportPDF(); }
        else if (value == 'ExportExcel') { ExportExcel(); }
        else if (value == 'ExportCSV') { ExportCSV(); }
        else if (value == 'New') { NewReport(); }
        else if (value == 'Edit') { EditReport(); }
        else if (value == 'Delete') { DeleteReport(); }
        else if (value == 'Help') { top.ShowHelpDialog(430); }
        else if (value == 'Favorite') {FavoriteReport();}
      }

      function NewReport() {
        var wnd = top.GetReportEditorDialog()
        wnd.add_close(DialogClosed);
        wnd.show();
        top.Ts.System.logAction('Reports - New Report Dialog Opened');
      }

      function EditReport() {
        var wnd = top.GetReportEditorDialog(GetSelectedReportID())
        wnd.add_close(DialogClosed);
        wnd.show();
        top.Ts.System.logAction('Reports - Edit Report Dialog Opened');

      }

      function DeleteReport() {
        if (confirm('Are you sure you would like to delete this report?')) {
          top.privateServices.DeleteReport(GetSelectedReportID());
          top.Ts.System.logAction('Reports - Report Deleted');

      }

      var tree = $find("<%= reportTree.ClientID %>");
      refreshNode(tree.get_selectedNode().get_parent().get_value());
      refreshNode(2);
  }

  function FavoriteReport() {
      top.privateServices.ToggleFavoriteReport(GetSelectedReportID());
      top.Ts.System.logAction('Reports - Favorite Report Toggled');

      top.privateServices.IsFavoriteReport(GetSelectedReportID(), SetIsFavorite);
      refreshNode(2);
  }

      function ExportPDF() {
        var frame = $get("<%=frmReport.ClientID %>");
        frame.contentWindow.ExportPDF();
        top.Ts.System.logAction('Reports - Export to PDF');

      }

      function ExportExcel() {
        var frame = $get("<%=frmReport.ClientID %>");
        frame.contentWindow.ExportExcel();
        top.Ts.System.logAction('Reports - Export to Excel');

      }

      function ExportCSV() {
        var frame = $get("<%=frmReport.ClientID %>");
        frame.contentWindow.ExportCSV();
        top.Ts.System.logAction('Reports - Export to CSV');

      }

      function DialogClosed(sender, args) {
          sender.remove_close(DialogClosed);

          if (args.get_argument() > -1 && args.get_argument() != null) {
              refreshNode(1);
              setTimeout(function () {
                  var tree = $find("<%=reportTree.ClientID %>");
                  var thisNode = tree.findNodeByValue(args.get_argument());
                  thisNode.set_selected(true);
                  SetNode(thisNode);
              }, 500);
          }
      }
      
    </script>

  </telerik:RadCodeBlock>
</asp:Content>
