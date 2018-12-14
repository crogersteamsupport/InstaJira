<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="Contacts.aspx.cs" Inherits="Frames_Contacts" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadWindowManager ID="RadWindowManager1" runat="server"></telerik:RadWindowManager>
  <telerik:RadSplitter ID="RadSplitter3" runat="server" Height="100%" Width="100%"
    BorderSize="0" Orientation="Horizontal">
    <telerik:RadPane ID="paneToolbar" runat="server" Height="32px" Scrolling="None">
      <telerik:RadToolBar ID="tbUser" runat="server" CssClass="NoRoundedCornerEnds" Width="100%" OnClientButtonClicked="ButtonClicked">
      <Items>
        <telerik:RadToolBarButton runat="server" Text="New" ImageUrl="~/images/icons/new.png" Value="NewUser">
        </telerik:RadToolBarButton>
        <telerik:RadToolBarButton runat="server" Text="Edit" ImageUrl="~/images/icons/edit.png" Value="EditUser">
        </telerik:RadToolBarButton>
        <telerik:RadToolBarButton runat="server" Text="Delete Contact" ImageUrl="~/images/icons/trash.png" Value="DeleteUser">
        </telerik:RadToolBarButton>
        <telerik:RadToolBarButton runat="server" Text="Add Reminder" ImageUrl="~/images/icons/clock.png" Value="Reminder" Visible="true">
        </telerik:RadToolBarButton>
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
                <telerik:GridTemplateColumn UniqueName="UserData">
                  <ItemTemplate>
                    <asp:Label ID="lblName" runat="server" Text="UserName"></asp:Label>
                  </ItemTemplate>
                  <ItemStyle BorderWidth="0px" />
                  <HeaderStyle Width="100%" />
                </telerik:GridTemplateColumn>
                <telerik:GridButtonColumn UniqueName="Image" ButtonType="ImageButton" ImageUrl="~/images/icons/Available.png" Display="false">
                  <ItemStyle BorderWidth="0px" Width="16px"/>
                </telerik:GridButtonColumn>
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
                      <iframe id="userContentFrame" runat="server" scrolling="no" src="" frameborder="0" height="100%" width="100%"></iframe>

        </telerik:RadPane>
      </telerik:RadSplitter>
    </telerik:RadPane>
  </telerik:RadSplitter>
    <asp:HiddenField ID="fieldOrganizationID" runat="server" />

  <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript" language="javascript">

      function GridResized(sender, args)
      {
        top.privateServices.SetUserSetting('ContactGridWidth', sender.get_width());
      }
    
      function RowSelected(sender, args)
      {
        var id = args.getDataKeyValue('UserID');
        top.privateServices.SetUserSetting('SelectedContactID', id);
        LoadContentPage();
      }
      
      function GetSelectedUserID()
      {
        var item = $find("<%=gridUsers.ClientID %>").get_masterTableView().get_selectedItems()[0];
        return item.getDataKeyValue('UserID');
      }
      
      function LoadContentPage()
      {
        showLoadingPanel("<%=userContentFrame.ClientID %>");
        var userID = GetSelectedUserID();
        var url = 'ContactInformation.aspx?UserID=' + userID;
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
      
      function DialogClosed(sender, args)
      {
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

      function GetOrganizationID() {
        var field = $get("<%=fieldOrganizationID.ClientID %>");
        return field.value;
      }

      function ButtonClicked(sender, args) {
        var button = args.get_item();
        var value = button.get_value();
        if (value == 'NewUser') {
          ShowDialog(top.GetContactDialog(GetOrganizationID()));
          top.Ts.System.logAction('Organization Contacts - New Contact Dialog Opened');
        }
        else if (value == 'EditUser') {
          ShowDialog(top.GetContactDialog(GetOrganizationID(), GetSelectedUserID()));
          top.Ts.System.logAction('Organization Contacts - Edit Contact Dialog Opened');
        }
        else if (value == 'DeleteUser') {
        radconfirm('Are you sure you would like to PERMANENTLY delete this contact?', function (arg) {
          if (arg) {
            top.privateServices.DeleteUser(GetSelectedUserID(), RefreshGrid);
            top.Ts.System.logAction('Organization Contacts - Contact Deleted');

          }
        }, 250, 125, null, 'Delete User');

        }
        else if (value == 'Reminder') {
          top.Ts.MainPage.editReminder({
            RefType: top.Ts.ReferenceTypes.Contacts,
            RefID: GetSelectedUserID()
          },
            true,
            function () {
              top.Ts.System.logAction('Organization Contacts - Reminder Added');

            });
        }

      }   
      
    </script>

  
  
  </telerik:RadCodeBlock>
  
  
</asp:Content>
