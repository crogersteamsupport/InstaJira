<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true"
  CodeFile="Organizations.aspx.cs" Inherits="Frames_Organizations" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <style type="text/css">
    .RadGrid_Office2007 .rgAltRow td, .RadGrid_Office2007 .rgRow td , tr.rgRow > td, tr.rgAltRow > td
    {
      border-width: 0px !important;
      border-left-width: 0px !important;
      border-right-width: 0px !important;
      border-top-width: 0px !important;
      border-bottom-width: 0px !important;
      }
      .divCustomerList {
        background:#fff;
        width:100%;
        height:100%;
        overflow:auto;
        position:relative;
      }
      #divResults{
        }
.ui-menutree-item { padding: 1px 0;  margin: 2px 5px 0 5px; clear:both; cursor:pointer; position:relative;  border: solid 1px #fff;}
.ui-menutree-subitems { padding-left: 10px;}
.ui-menutree-state-hover { background: #E1F5FF url('../images/vmenu_hover.gif') repeat-x; border: solid 1px #A9C5E8; }
.ui-menutree-state-default { background-color:#fff; border: solid 1px #fff;}
.ui-menutree-state-selected { background: #D1F1FF url('../images/vmenu_selected.gif') repeat-x; border: solid 1px #8AAEDE; }
.ui-menutree-data { display:none;}
.ui-menutree-text { vertical-align:middle; padding-left: 5px; }

  </style>

  <link href="../Css/ui.css" rel="stylesheet" type="text/css" />

  <script src="/frontend/library/json2.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline">
    <AjaxSettings>
      <telerik:AjaxSetting AjaxControlID="gridCustomers">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="gridCustomers" />
          <telerik:AjaxUpdatedControl ControlID="frmOrganizations" />
        </UpdatedControls>
      </telerik:AjaxSetting>
    </AjaxSettings>
  </telerik:RadAjaxManager>
  <telerik:RadWindowManager ID="RadWindowManager1" runat="server">
  </telerik:RadWindowManager>
  <telerik:RadSplitter ID="splMain" runat="server" Height="100%" Width="100%" VisibleDuringInit="false"
    BorderSize="0" Orientation="Horizontal">
    <telerik:RadPane ID="paneToolBar" runat="server" Height="32px" Scrolling="None">
      <telerik:RadToolBar ID="tbMain" runat="server" CssClass="NoRoundedCornerEnds" Width="100%"
        OnClientButtonClicked="ButtonClicked">
        <Items>
          <telerik:RadToolBarButton runat="server" Text="New" ImageUrl="~/images/icons/new.png"
            Value="NewOrganization">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Edit" ImageUrl="~/images/icons/edit.png"
            Value="EditOrganization">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Delete Company" ImageUrl="~/images/icons/trash.png"
            Value="DeleteOrganization">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Subscribe" ImageUrl="~/images/icons/Subscription.png"
            Value="Subscribe">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="Help" ImageUrl="~/images/icons/Help.png"
            Text="Help" ToolTip="Help." Visible="false">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="Reminder" ImageUrl="~/images/icons/Clock.png"
            Text="Add Reminder" ToolTip="Add a reminder for this customer." Visible="true">
          </telerik:RadToolBarButton>
        </Items>
      </telerik:RadToolBar>
    </telerik:RadPane>
    <telerik:RadPane ID="paneBody" runat="server" Height="100%" Scrolling="None">
      <telerik:RadSplitter ID="splBody" runat="server" Height="100%" Width="100%" BorderSize="0">
        <telerik:RadPane ID="paneGrid" runat="server" Width="250px" Scrolling="None" OnClientResized="GridResized">
          <telerik:RadSplitter ID="splGrid" runat="server" Orientation="Horizontal" Height="100%"
            Width="100%" BorderSize="0">
            <telerik:RadPane ID="paneSearch" runat="server" Height="28px" Scrolling="None">
              <div style="width: 100%; padding: 2px 2px 2px 2px; border-bottom: Solid 1px #ABC1DE;
                background-color: #DDEAFA;">
                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                  <tr>
                    <td style="width: 100%;">
                      <telerik:RadTextBox ID="textFilter" runat="server" Width="99%" EmptyMessage="Enter Contact or Customer">
                        <ClientEvents OnKeyPress="onKeyPress" />
                      </telerik:RadTextBox>
                    </td>
                    <td style="vertical-align:top;">
                      <img alt="Clear" src="../images/icons/Close_2.png" style="margin: 5px 5px 0 5px;
                        cursor: pointer;" onclick="updateGrid(true);" />
                    </td>
                  </tr>
                </table>
              </div>
            </telerik:RadPane>
            <telerik:RadPane ID="paneOrganizations" runat="server" BackColor="#ffffff" Scrolling="None">
              <div class="divCustomerList">
                <div id="divResults">
              
                </div>
              
              </div>
              
            </telerik:RadPane>
          </telerik:RadSplitter>
        </telerik:RadPane>
        <telerik:RadSplitBar ID="RadSplitBar1" runat="server" />
        <telerik:RadPane ID="paneContent" runat="server" Width="100%" Scrolling="None">
          <telerik:RadSplitter ID="splContent" runat="server" Height="100%" Width="100%" BorderSize="0"
            Orientation="Horizontal">
            <telerik:RadPane ID="paneCaption" runat="server" Scrolling="None" Height="35px" BackColor="#BFDBFF">
              <div style="width: 100%; height: 20px; padding: 10px 15px; white-space: nowrap;">
                <span id="captionSpan" style="font-weight: bold; font-size: 16px;">
                </span>
              </div>
            </telerik:RadPane>
            <telerik:RadPane ID="paneTabs" runat="server" Scrolling="None" Height="29px" BackColor="#BFDBFF">
              <div style="padding-top: 3px;">
                <telerik:RadTabStrip ID="tsMain" runat="server" SelectedIndex="0" OnClientTabSelected="TabSelected"
                  ShowBaseLine="True" Width="100%" PerTabScrolling="True" ScrollChildren="True">
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
      var _filterTimeOutID;
      var _loadedID = -1;
      var _selectedTab = -1;
      /*

      var _lastRefreshTime = new Date();

      function refreshData(overrideTime) {
        if (_lastRefreshTime != null && !overrideTime) {
          var now = new Date();
          var diff = (now - _lastRefreshTime) / 1000;
          if (diff < 300) return;
        }

        _lastRefreshTime = new Date();

        window.location = window.location;

      }*/
      

      $(document).ready(function() {
      });


      function pageLoad() {
        updateGrid(false);
      }

      function onKeyPress(sender, args) {
        clearTimeout(_filterTimeOutID);
        _filterTimeOutID = setTimeout('updateGrid(false)', 500);
      }

      function updateGrid(doClear, orgID, usrID) {
        if (doClear && getFilterText() != '') $find("<%=textFilter.ClientID %>").set_value('');

        var id = '';
        if (top._selectContactID > -1) {
          id = 'u' + top._selectContactID;
          PageMethods.GetContact(top._selectContactID, function(results) { loadGrid(results, id) });
        }
        else if (top._selectCustomerID > -1) {
          id = 'o' + top._selectCustomerID;
          PageMethods.GetCustomer(top._selectCustomerID, function(results) { loadGrid(results, id) });
        }
        else {
          id = getSelectedItemID();
          PageMethods.GetResults(getFilterText(), function(results) { loadGrid(results, id) });
        }
        top._selectCustomerID = -1;
        top._selectContactID = -1;

      }

      function loadGrid(results, id) {
        $('#divResults').html(results);
        $('#divResults').children('.ui-menutree-item').click(mainVMenu_OnClick);
        if (id != null) setSelectedItemID(id, true);
        id = getSelectedItemID();
        if (id == null) setSelectedItemID(getFirstItemID(), true);
        LoadContentPage();
      
      }

      function mainVMenu_OnClick() {
        setSelectedItemID(this.id, false);
        var data = getData(this.id);

        top.privateServices.SetUserSetting('SelectedOrganizationID', data.OrganizationID);
        top.privateServices.SetUserSetting('SelectedContactID', data.UserID);
        IsSubscribed();
        if (data.UserID > -1) {
          selectContactsTab();
        }

        LoadContentPage();
      }


      function getFirstItemID() {
        return $('#divResults').children('.ui-menutree-item:first')[0].id;
      }

      function getSelectedItemID() {
        var selected = $('#divResults').children('.ui-menutree-state-selected');
        if (selected.length > 0) return selected[0].id; else return null;
      }
      
      function setSelectedItemID(id, doScroll)
      {
        $('#divResults').children('.ui-menutree-item').removeClass('ui-menutree-state-selected');
        var selected = $('#divResults').children('#' + id + ':first').removeClass('ui-menutree-state-default').addClass('ui-menutree-state-selected');

        if (doScroll && selected != null) {
          $('#divResults').scrollTop(selected.offset().top - 100);
        }
        return selected;
      }

      function getData(id) {
        return JSON.parse($('#' + id + ' .ui-menutree-data:first').text()); 
      }
      function getText(id) {
        return $('#' + id + ' .ui-menutree-text:first').html();
      }

      function selectContact(userID, orgID) {
        selectContactsTab();
        updateGrid(true, orgID, userID);
      }

      function selectCustomer(orgID) {
        var strip = $find("<%=tsMain.ClientID %>");
        var tab = strip.findTabByText('Details');
        tab.select();
        updateGrid(true, orgID);
      }



      function selectContactsTab() {
        var strip = $find("<%=tsMain.ClientID %>");
        var tab = strip.findTabByText('Contacts');
        tab.select();
      }

      function getFilterText() {
        var filter = $find("<%=textFilter.ClientID %>");
        var result = filter.get_value();
        if (!result) return '';

        if (filter.isEmpty() || result == filter.get_emptyMessage()) return '';
        return result;
      }



      function Subscribe() {
        top.privateServices.Subscribe(9, getData(getSelectedItemID()).OrganizationID);
        top.Ts.System.logAction('Organizations - Subscribed to Customer');
        var toolBar = $find("<%=tbMain.ClientID %>");
        var item = toolBar.findItemByValue("Subscribe");
        setTimeout('IsSubscribed()', 2000);
        if (item.get_text() == 'Unsubscribe')
          alert('You have unsubscribed to the selected organization.');
        else
          alert('You have subscribed to the selected organization.');

      }

      function IsSubscribed() {
        top.privateServices.IsSubscribed(9, getData(getSelectedItemID()).OrganizationID, IsSubscribedResult);
      }

      function IsSubscribedResult(result) {
        var toolBar = $find("<%=tbMain.ClientID %>");
        var item = toolBar.findItemByValue("Subscribe");
        if (result)
          item.set_text('Unsubscribe');
        else
          item.set_text('Subscribe');
      }


      function GridResized(sender, args) {
        top.privateServices.SetUserSetting('OrganizationsGridWidth', sender.get_width());
      }



      function TabSelected(sender, args) {

        var tab = args.get_tab();
        var index = tab.get_index();
        top.privateServices.SetUserSetting('SelectedOrganizationTabIndex', index);
        if (index < 9) top.Ts.System.logAction('Organizations - Tab Selected (' + tab.get_text() + ')');
        else top.Ts.System.logAction('Organizations - Tab Selected (Ticket Type)');

        LoadContentPage();
      }

      function GetSelectedTab() {
        var strip = $find("<%=tsMain.ClientID %>");
        return strip.get_selectedTab();
      }

      function LoadContentPage() {
        var id = getData(getSelectedItemID()).OrganizationID;
        if (!id) return;
        var tab = GetSelectedTab();
        var index = tab.get_index();
        if (id == _loadedID && index == _selectedTab) return;
        var tabval = tab.get_value();
        if (tabval.indexOf("Watercooler.html") != -1) {
            var url = tab.get_value() + 'pagetype=2&pageid=' + id;
        }
        else{
            var url = tab.get_value() + id;
        }
        
        if (index == 1) {
          var userID = getData(getSelectedItemID()).UserID;
          if (userID > -1)
            url = url + '&UserID=' + userID;
        }
        
        var frame = $get("<%=frmOrganizations.ClientID %>");
        frame.setAttribute('src', url);

        $('#captionSpan').html(getText(getSelectedItemID()));
        _requestedUsrID = -1;
      }


      function DialogClosed(sender, args) {
        updateGrid(false);
        sender.remove_close(DialogClosed);
      }

      function ShowDialog(wnd) {
        wnd.add_close(DialogClosed);
        wnd.show();
      }

      function ButtonClicked(sender, args) {
        var button = args.get_item();
        var value = button.get_value();
        if (value == 'NewOrganization') {
          top.Ts.System.logAction('Organizations - New Organization Opened');
          ShowDialog(top.GetOrganizationDialog());
        }
        else if (value == 'EditOrganization') {
          top.Ts.System.logAction('Organizations - Edit Organization Opened');
          ShowDialog(top.GetOrganizationDialog(getData(getSelectedItemID()).OrganizationID));
        }
        else if (value == 'DeleteOrganization') {
        radconfirm('Are you sure you would like to PERMANENTLY delete this organization?', function (arg) {
          if (arg) {
            top.Ts.System.logAction('Organizations - Organization Deleted');
            top.privateServices.DeleteOrganization(getData(getSelectedItemID()).OrganizationID, updateGrid(false)); 
          }
        }, 250, 125, null, 'Delete Organization');
        }
      else if (value == "Subscribe") {
          Subscribe();
        }
        else if (value == 'Help') {
          top.ShowHelpDialog(130);
        }
        else if (value == 'Reminder') {
          top.Ts.MainPage.editReminder({
            RefType: top.Ts.ReferenceTypes.Organizations,
            RefID: getData(getSelectedItemID()).OrganizationID
            },
            true,
            function () {
              top.Ts.System.logAction('Organizations - Reminder Added');
            });
        }

      }      
      
      function reload() {
        window.location.reload(true);
      }
    </script>

  </telerik:RadCodeBlock>
</asp:Content>
