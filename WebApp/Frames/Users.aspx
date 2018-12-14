<%@ Page Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="Users.aspx.cs" Inherits="Frames_Users" Title="Untitled Page" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadWindowManager ID="RadWindowManager1" runat="server"></telerik:RadWindowManager>
  <telerik:RadSplitter ID="RadSplitter3" runat="server" Height="100%" Width="100%"
    BorderSize="0" Orientation="Horizontal">
    <telerik:RadPane ID="RadPane5" runat="server" Height="32px" Scrolling="None">
      <telerik:RadToolBar ID="tbUser" runat="server" CssClass="NoRoundedCornerEnds" Width="100%" OnClientButtonClicked="ButtonClicked">
      <Items>
        <telerik:RadToolBarButton runat="server" Text="New" ImageUrl="~/images/icons/new.png" Value="NewUser">
        </telerik:RadToolBarButton>
        <telerik:RadToolBarButton runat="server" Text="Edit" ImageUrl="~/images/icons/edit.png" Value="EditUser" Visible="false">
        </telerik:RadToolBarButton>
        <telerik:RadToolBarButton runat="server" Text="Delete" ImageUrl="~/images/icons/trash.png" Value="DeleteUser">
        </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="Help" ImageUrl="~/images/icons/Help.png"
            Text="Help" ToolTip="Help." Visible="false"></telerik:RadToolBarButton>
        
      </Items>
    </telerik:RadToolBar>
    </telerik:RadPane>
    <telerik:RadPane ID="RadPane6" runat="server" Height="100%" Scrolling="None">
      <telerik:RadSplitter ID="RadSplitter1" runat="server" Height="100%" Width="100%"
        BorderSize="0">
        <telerik:RadPane ID="paneUsersGrid" runat="server" Width="250px" Scrolling="None" OnClientResized="GridResized">
          <telerik:RadGrid ID="gridUsers" runat="server" AutoGenerateColumns="False" GridLines="None"
            Height="100%" Width="100%" ShowHeader="False" BorderColor="#FF0066" BorderStyle="None"
            OnNeedDataSource="gridUsers_NeedDataSource" 
            OnItemDataBound="gridUsers_ItemDataBound" ondatabound="gridUsers_DataBound">
            <MasterTableView DataKeyNames="UserID" ClientDataKeyNames="UserID">
              <Columns>
                <telerik:GridButtonColumn UniqueName="Image" ButtonType="ImageButton" ImageUrl="~/images/icons/Available.png">
                  <ItemStyle BorderWidth="0px" Width="16px" />
                </telerik:GridButtonColumn>
                <telerik:GridTemplateColumn UniqueName="UserData">
                  <ItemTemplate>
                    <asp:Label ID="lblName" runat="server" Text="UserName"></asp:Label>
                  </ItemTemplate>
                  <ItemStyle BorderWidth="0px" />
                  <HeaderStyle Width="100%" />
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="UserID" UniqueName="UserID" Display="False">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="FirstName" UniqueName="FirstName" Display="False">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="LastName" UniqueName="LastName" Display="False">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="InOfficeComment" UniqueName="InOfficeComment"
                  Display="False" EmptyDataText=""></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="InOffice" UniqueName="InOffice" Display="False">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="IsOnline" UniqueName="IsOnline" Display="False">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="IsActive" UniqueName="IsActive" Display="False">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="TicketCount" UniqueName="TicketCount" Display="False">
                </telerik:GridBoundColumn>
              </Columns>
            </MasterTableView>
            <ClientSettings>
              <Selecting AllowRowSelect="True"></Selecting>
              <ClientEvents OnRowSelected="RowSelected"/>
              <Scrolling AllowScroll="True"/>
            </ClientSettings>
          </telerik:RadGrid>
        </telerik:RadPane>
        <telerik:RadSplitBar ID="RadSplitBar1" runat="server" />
        <telerik:RadPane ID="RadPane2" runat="server" Width="100%" Scrolling="None">
          <telerik:RadSplitter ID="RadSplitter2" runat="server" Height="100%" Width="100%"
            BorderSize="0" Orientation="Horizontal">
            <telerik:RadPane ID="RadPane7" runat="server" Scrolling="None" Height="35px" BackColor="#BFDBFF">
              <div style="width:100%; height: 20px; padding: 10px 15px;">
                <span id="captionSpan" runat="server" style="font-weight:bold; font-size: 16px;">Test</span>
              </div>
            </telerik:RadPane>
            <telerik:RadPane ID="RadPane3" runat="server" Scrolling="None" Height="29px" BackColor="#BFDBFF">
            <div style="padding-top: 3px;">
            
              <telerik:RadTabStrip ID="tsMain" runat="server" SelectedIndex="0" OnClientTabSelected="TabSelected">
                <Tabs>
                  <telerik:RadTab runat="server" Value="../vcr/1_9_0/Pages/User.html?UserID=" Selected="True" Text="User Information"></telerik:RadTab>
                  <telerik:RadTab runat="server" Value="../vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=false&tf_UserID=" Text="Open"></telerik:RadTab>
                  <telerik:RadTab runat="server" Value="../vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=true&tf_UserID=" Text="Closed"></telerik:RadTab>
                  <telerik:RadTab runat="server" Value="../vcr/1_9_0/Pages/TicketGrid.html?tf_UserID=" Text="All Tickets"></telerik:RadTab>
                  <telerik:RadTab runat="server" Value="../vcr/1_9_0/Pages/TicketGrid.html?tf_IsEnqueued=true&tf_ViewerID=" Text="Ticket Queue"></telerik:RadTab>
                  <telerik:RadTab runat="server" Value="History.aspx?RefType=22&RefID=" Text="History" Visible="false"></telerik:RadTab>
                </Tabs>
              </telerik:RadTabStrip>
              </div>
            </telerik:RadPane>
            <telerik:RadPane ID="RadPane4" runat="server" Scrolling="None" Height="100%" BackColor="#DBE6F4">
            <iframe id="userContentFrame" runat="server" scrolling="no" src="" frameborder="0" height="100%" width="100%"></iframe>
            
            </telerik:RadPane>
          </telerik:RadSplitter>
        </telerik:RadPane>
      </telerik:RadSplitter>
    </telerik:RadPane>
  </telerik:RadSplitter>
  <asp:HiddenField ID="fieldOrganizationID" runat="server" />
  <asp:HiddenField ID="fieldAllowNewUsers" runat="server" />
  <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript" language="javascript">
          function refreshData(overrideTime) {

            window.location = window.location;
            }


          
      function GridResized(sender, args)
      {
        top.privateServices.SetUserSetting('UserGridWidth', sender.get_width());
      }
    
      function RowSelected(sender, args)
      {
        var id = args.getDataKeyValue('UserID');
        top.privateServices.SetUserSetting('SelectedUserID', id);
        var span = $get("<%=captionSpan.ClientID %>");
        span.innerHTML = GetSelectedCellText('FirstName') + ' ' + GetSelectedCellText('LastName');
        LoadContentPage();
      }

      function GetOrganizationID() {
        var field = $get("<%=fieldOrganizationID.ClientID %>");
        return field.value;
      }
      
      function TabSelected(sender, args) {
        var tab = args.get_tab();
        top.Ts.System.logAction('Users - Tab Selected ('+ tab.get_text() +')');
        top.privateServices.SetUserSetting('SelectedUserTabIndex', tab.get_index());
        LoadContentPage();
      }
      
      function GetSelectedUserID()
      {
        var item = $find("<%=gridUsers.ClientID %>").get_masterTableView().get_selectedItems()[0];
        return item.getDataKeyValue('UserID');
      }
      
      function GetSelectedTabValue()
      {
        var strip = $find("<%=tsMain.ClientID %>");
        return strip.get_selectedTab().get_value();
      }
      
      function LoadContentPage()
      {
        showLoadingPanel("<%=userContentFrame.ClientID %>");
        var userID = GetSelectedUserID();
        var url = GetSelectedTabValue() + userID;
        var frame = $get("<%=userContentFrame.ClientID %>");
        frame.setAttribute('src', url);
        hideLoadingPanel("<%=userContentFrame.ClientID %>");
      }
      
      function GetSelectedCellText(columnUniqueName)
      {
        var table = $find("<%=gridUsers.ClientID %>").get_masterTableView();
        var item = table.get_selectedItems()[0];
        return table.getCellByColumnUniqueName(item, columnUniqueName).innerHTML;
      }
      
      function DialogClosed(sender, args) {
        RefreshGrid();
        sender.remove_close(DialogClosed);
      }
      
      function RefreshGrid()
      {
        var grid = $find("<%=gridUsers.ClientID %>").get_masterTableView();
        grid.rebind();
      }

      function ShowDialog(wnd) {
        wnd.add_close(DialogClosed);
        wnd.show();
      }

      function ButtonClicked(sender, args) {
        var button = args.get_item();
        var value = button.get_value();
        if (value == 'NewUser') {
          var field = $get("<%=fieldAllowNewUsers.ClientID %>");
          if (field.value != '') {
            alert(field.value);
            return;
          }

          ShowDialog(top.GetUserDialog(GetOrganizationID()));
          top.Ts.System.logAction('Users - New User Dialog Opened');
        }
        else if (value == 'EditUser') {
          ShowDialog(top.GetUserDialog(GetOrganizationID(), GetSelectedUserID()));
        }
        else if (value == 'DeleteUser') {
        radconfirm('Are you sure you would like to PERMANENTLY delete this user?',
          function (arg) {
            if (arg) {
              top.privateServices.DeleteUser(GetSelectedUserID(), RefreshGrid);
              top.Ts.System.logAction('Users - User Deleted');
            }
          }, 250, 125, null, 'Delete User');

        }
        else if (value == 'Help') {
        top.ShowHelpDialog(30);
        }

      }      
    
      
    </script>

  
  
  </telerik:RadCodeBlock>
  
  
</asp:Content>
