<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true"
  CodeFile="Attachments.aspx.cs" Inherits="Frames_Attachments" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadSplitter ID="RadSplitter1" runat="server" Height="100%" Width="100%"
    Orientation="Horizontal" BorderSize="0">
    <telerik:RadPane ID="paneToolbar" runat="server" Height="32px" Scrolling="None">
      <telerik:RadToolBar ID="tbMain" runat="server" CssClass="NoRoundedCornerEnds" Width="100%"
        OnClientButtonClicked="ButtonClicked">
        <Items>
          <telerik:RadToolBarButton runat="server" Text="Add Attachment" ImageUrl="~/images/icons/add.png"
            Value="AddAttachment"></telerik:RadToolBarButton>
        </Items>
      </telerik:RadToolBar>
    </telerik:RadPane>
    <telerik:RadPane ID="RadPane2" runat="server" Scrolling="None" Height="100%">
      <div class="stretchContentHolderDiv">
        <telerik:RadGrid ID="gridAttachments" runat="server" Height="100%" Width="100%" AutoGenerateColumns="False"
          BorderWidth="0px" GridLines="None" 
          OnNeedDataSource="gridAttachments_NeedDataSource" 
          onitemdatabound="gridAttachments_ItemDataBound">
          <HeaderContextMenu EnableTheming="True">
            <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
          </HeaderContextMenu>
          <MasterTableView ClientDataKeyNames="AttachmentID" DataKeyNames="AttachmentID"  >
            <Columns>
              <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="../images/icons/trash.png"
                UniqueName="ButtonDelete" CommandName="Delete" ConfirmText="Are you sure you would like to remove this attachment?"
                ConfirmDialogType="RadWindow">
                <ItemStyle Width="20px" />
              </telerik:GridButtonColumn>
              <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="../images/icons/open.png"
                UniqueName="ButtonOpen" CommandName="OpenAttachment">
                <ItemStyle Width="20px" />
              </telerik:GridButtonColumn>
              <telerik:GridBoundColumn DataField="FileName" HeaderText="File Name" UniqueName="FileName">
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="Description" HeaderText="Description" UniqueName="Description">
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="CreatorName" HeaderText="Attached By" UniqueName="CreatorName">
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="DateCreated" HeaderText="Date Attached" UniqueName="DateCreated">
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="AttachmentID" UniqueName="AttachmentID" Visible="False">
              </telerik:GridBoundColumn>
            </Columns>
          </MasterTableView>
      <ClientSettings>
        <Scrolling AllowScroll="True" EnableVirtualScrollPaging="False" UseStaticHeaders="True" />
      </ClientSettings>
          <FilterMenu EnableTheming="True">
            <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
          </FilterMenu>
        </telerik:RadGrid>
      </div>
    </telerik:RadPane>
  </telerik:RadSplitter>
  <asp:HiddenField ID="fieldRefID" runat="server" />
  <asp:HiddenField ID="fieldRefType" runat="server" />
  <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

    <script type="text/javascript" language="javascript">
      function DialogClosed(sender, args) {
        RefreshGrid();
        sender.remove_close(DialogClosed);
      }

      function RefreshGrid() {
        var grid = $find("<%=gridAttachments.ClientID %>").get_masterTableView();
        grid.rebind();
      }

      function ShowDialog(wnd) {
        top.Ts.System.logAction('Organization Attachments - Attachment Dialog Opened');
        wnd.add_close(DialogClosed);
        wnd.show();
      }

      function GetRefID() {
        var field = $get("<%=fieldRefID.ClientID %>");
        return field.value;
      }
      function GetRefType() {
        var field = $get("<%=fieldRefType.ClientID %>");
        return field.value;
      }

      function ButtonClicked(sender, args) {
        var button = args.get_item();
        var value = button.get_value();
        if (value == 'AddAttachment') {
          ShowDialog(top.GetAttachFileDialog(GetRefID(), GetRefType()));
        }
      }



      function OpenRowAttachment(id) {
        top.Ts.MainPage.openAttachment(id);
      }
      function DeleteRow(id) {
        if (!confirm('Are you sure you would like to delete this attachment?')) return;
        top.privateServices.DeleteAttachment(id, RefreshGrid);
        top.Ts.System.logAction('Organization Attachments - Attachment Deleted');

      }
    </script>

  </telerik:RadScriptBlock>
</asp:Content>
