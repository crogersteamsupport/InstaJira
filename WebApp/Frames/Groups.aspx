<%@ Page Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="Groups.aspx.cs" Inherits="Frames_Groups" Title="Untitled Page" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline"></telerik:RadAjaxManager>
  <telerik:RadWindowManager ID="RadWindowManager1" runat="server"></telerik:RadWindowManager>
  <telerik:RadSplitter ID="RadSplitter3" runat="server" Height="100%" Width="100%"
    BorderSize="0" Orientation="Horizontal">
    <telerik:RadPane ID="RadPane5" runat="server" Height="32px" Scrolling="None">
      <telerik:RadToolBar ID="tbGroup" runat="server" CssClass="NoRoundedCornerEnds" Width="100%" OnClientButtonClicked="ButtonClicked">
      <Items>
        <telerik:RadToolBarButton runat="server" Text="New" ImageUrl="~/images/icons/new.png" Value="NewGroup">
        </telerik:RadToolBarButton>
        <telerik:RadToolBarButton runat="server" Text="Edit" ImageUrl="~/images/icons/edit.png" Value="EditGroup">
        </telerik:RadToolBarButton>
        <telerik:RadToolBarButton runat="server" Text="Delete" ImageUrl="~/images/icons/trash.png" Value="DeleteGroup">
        </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="Help" ImageUrl="~/images/icons/Help.png"
            Text="Help" ToolTip="Help." Visible="false"></telerik:RadToolBarButton>
        
      </Items>
    </telerik:RadToolBar>
    </telerik:RadPane>
    <telerik:RadPane ID="RadPane6" runat="server" Height="100%" Scrolling="None">
      <telerik:RadSplitter ID="RadSplitter1" runat="server" Height="100%" Width="100%"
        BorderSize="0">
        <telerik:RadPane ID="pangGroupGrid" runat="server" Width="250px" Scrolling="None" OnClientResized="GridResized">
        <telerik:RadGrid ID="gridGroups" runat="server" Width="100%" Height="100%" AutoGenerateColumns="False" GridLines="None" BorderWidth="0px" OnDataBound="gridGroups_DataBound" OnNeedDataSource="gridGroups_NeedDataSource" OnItemDataBound="gridGroups_ItemDataBound">
          <MasterTableView ClientDataKeyNames="GroupID" DataKeyNames="GroupID" ShowHeader="false">
            <Columns>
              <telerik:GridBoundColumn DataField="Name" HeaderText="Group" UniqueName="Name" Display="True">
                <ItemStyle BorderWidth="0" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="GroupID" DataType="System.Int32" Display="False" UniqueName="columnGroupID">
                <ItemStyle BorderWidth="0" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="TicketCount" HeaderText="TicketCount" UniqueName="TicketCount" DataType="System.Int32"  Display="False">
                <ItemStyle BorderWidth="0" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="Description" HeaderText="Description" UniqueName="Description" Display="False">
                <ItemStyle BorderWidth="0" />
              </telerik:GridBoundColumn>
            </Columns>
          </MasterTableView>
          <ClientSettings>
            <Selecting AllowRowSelect="True" />
            <Scrolling AllowScroll="true"/>
            <ClientEvents OnRowSelected="RowSelected"/>
          </ClientSettings>
        </telerik:RadGrid>
        </telerik:RadPane>
        <telerik:RadSplitBar ID="RadSplitBar1" runat="server" />
        <telerik:RadPane ID="RadPane2" runat="server" Width="100%" Scrolling="None">
          <telerik:RadSplitter ID="RadSplitter2" runat="server" Height="100%" Width="100%"
            BorderSize="0" Orientation="Horizontal">
            <telerik:RadPane ID="RadPane7" runat="server" Scrolling="None" Height="35px" BackColor="#BFDBFF">
              <div style="width:100%; height: 20px; padding: 10px 15px;">
                <span id="captionSpan" runat="server" style="font-weight:bold; font-size: 16px;">[No group selected]</span>
              </div>
            </telerik:RadPane>
            <telerik:RadPane ID="RadPane3" runat="server" Scrolling="None" Height="29px" BackColor="#BFDBFF">
            <div style="padding-top: 3px;">
            
              <telerik:RadTabStrip ID="tsMain" runat="server" SelectedIndex="0" OnClientTabSelected="TabSelected" ShowBaseLine="True" Width="100%" PerTabScrolling="True" ScrollChildren="True">
                <Tabs>
                  <telerik:RadTab runat="server" Value="GroupInformation.aspx?GroupID=" Selected="True" Text="Group Information"></telerik:RadTab>
                  <telerik:RadTab runat="server" Value="../vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=false&tf_GroupID=" Text="Open Tickets"></telerik:RadTab>
                  <telerik:RadTab runat="server" Value="../vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=true&tf_GroupID=" Text="Closed Tickets"></telerik:RadTab>
                  <telerik:RadTab runat="server" Value="../vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=false&tf_UserID=-2&tf_GroupID=" Text="Unassigned Tickets"></telerik:RadTab>
                  <telerik:RadTab runat="server" Value="../vcr/1_9_0/Pages/TicketGrid.html?tf_GroupID=" Text="All Tickets"></telerik:RadTab>
                  <telerik:RadTab runat="server" Value="History.aspx?RefType=6&RefID=" Text="Group History"></telerik:RadTab>
                </Tabs>
              </telerik:RadTabStrip>
              </div>
            </telerik:RadPane>
            <telerik:RadPane ID="RadPane4" runat="server" Scrolling="None" Height="100%" BackColor="#DBE6F4">
            <iframe id="groupContentFrame" runat="server" scrolling="no" src="" frameborder="0" height="100%" width="100%"></iframe>
            
            </telerik:RadPane>
          </telerik:RadSplitter>
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

        function pageLoad() {
            LoadContentPage();
        }

      function GridResized(sender, args)
      {
        top.privateServices.SetUserSetting('GroupGridWidth', sender.get_width());
      }
    
      function RowSelected(sender, args) {
        var id = args.getDataKeyValue('GroupID');
        top.privateServices.SetUserSetting('SelectedGroupID', id);
        var span = $get("<%=captionSpan.ClientID %>");
        span.innerHTML = GetSelectedCellText('Name');
        LoadContentPage();
      }
      
      function TabSelected(sender, args) {
        var tab = args.get_tab();
        top.privateServices.SetUserSetting('SelectedGroupTabIndex', tab.get_index());
        top.Ts.System.logAction('Groups - Tab Selected (' + tab.get_text() + ')');
        LoadContentPage();
      }
      
      function GetSelectedGroupID()
      {
        var item = $find("<%=gridGroups.ClientID %>").get_masterTableView().get_selectedItems()[0];
        return item.getDataKeyValue('GroupID');
      }
      
      function GetSelectedTabValue()
      {
        var strip = $find("<%=tsMain.ClientID %>");
        return strip.get_selectedTab().get_value();
      }
      
      function LoadContentPage()
      {
        showLoadingPanel("<%=groupContentFrame.ClientID %>");
        var groupID = GetSelectedGroupID();

        var frame = $get("<%=groupContentFrame.ClientID %>");

        if (GetSelectedTabValue().indexOf("Watercooler.html") != -1) {
            var url = GetSelectedTabValue() + 'pagetype=4&pageid=' + groupID;
        } else {
            var url = GetSelectedTabValue() + groupID;
        }

        frame.setAttribute('src', url);
        hideLoadingPanel("<%=groupContentFrame.ClientID %>");
      }
      
      function GetSelectedCellText(columnUniqueName)
      {
        var table = $find("<%=gridGroups.ClientID %>").get_masterTableView();
        var item = table.get_selectedItems()[0];
        return table.getCellByColumnUniqueName(item, columnUniqueName).innerHTML;
      }
      
      function DialogClosed(sender, args)
      {
        RefreshGrid();
        sender.remove_close(DialogClosed);
      }
      
      function RefreshGrid()
      {
        var grid = $find("<%=gridGroups.ClientID %>").get_masterTableView();
        grid.rebind();
      }

      function ButtonClicked(sender, args) {
        var button = args.get_item();
        var value = button.get_value();
        if (value == 'NewGroup') {
          ShowDialog(top.GetGroupDialog());
          top.Ts.System.logAction('Groups - New Group Dialog Opened');
        }
        else if (value == 'EditGroup') {
          ShowDialog(top.GetGroupDialog(GetSelectedGroupID()));
          top.Ts.System.logAction('Groups - Edit Group Dialog Opened');
        }
        else if (value == 'DeleteGroup') {
        radconfirm('Are you sure you would like to PERMANENTLY delete this group?', function (arg) {
          if (arg) {
            top.privateServices.DeleteGroup(GetSelectedGroupID(), RefreshGrid);
            top.Ts.System.logAction('Groups - Group Deleted');

          }
        }, 250, 125, null, 'Delete Group'); 

        
        }
        else if (value == 'Help') {
          top.ShowHelpDialog(120);
        }

      }

      function ShowDialog(wnd) {
        wnd.add_close(DialogClosed);
        wnd.show();
      }
      
    </script>

  
  
  </telerik:RadCodeBlock>
</asp:Content>

