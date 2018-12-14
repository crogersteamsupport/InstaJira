<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true"
  CodeFile="Notes.aspx.cs" Inherits="Frames_Notes" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline">
    <AjaxSettings>
      <telerik:AjaxSetting AjaxControlID="gridNotes">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="gridNotes" />
        </UpdatedControls>
      </telerik:AjaxSetting>
    </AjaxSettings>
  </telerik:RadAjaxManager>
  <telerik:RadSplitter ID="RadSplitter1" runat="server" Height="100%" Width="100%"
    BorderSize="0" Orientation="Horizontal">
    <telerik:RadPane ID="paneToolbar" runat="server" Height="32px" Scrolling="None">
      <telerik:RadToolBar ID="tbMain" runat="server" CssClass="NoRoundedCornerEnds" Width="100%"
        OnClientButtonClicked="ButtonClicked">
        <Items>
          <telerik:RadToolBarButton runat="server" Text="Add Note" ImageUrl="~/images/icons/add.png"
            Value="AddNote"></telerik:RadToolBarButton>
        </Items>
      </telerik:RadToolBar>
    </telerik:RadPane>
    <telerik:RadPane ID="paneGrid" runat="server" Scrolling="None" OnClientResized="GridResized">
    <div class="stretchContentHolderDiv">
      <telerik:RadGrid ID="gridNotes" runat="server" Height="100%" Width="100%" AutoGenerateColumns="False"
        GridLines="None" OnNeedDataSource="gridNotes_NeedDataSource"  BorderWidth="0"
        OnDataBound="gridNotes_DataBound" onitemdatabound="gridNotes_ItemDataBound" 
        AllowSorting="True">
        <HeaderContextMenu EnableTheming="True">
          <CollapseAnimation Duration="200" Type="OutQuint" />
        </HeaderContextMenu>
        <MasterTableView DataKeyNames="NoteID" ClientDataKeyNames="NoteID" 
          TableLayout="Auto" CellSpacing="-1">
          <Columns>
            <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="../images/icons/edit.png"
              UniqueName="ButtonEdit" CommandName="EditNote">
              <HeaderStyle Width="32px" />
            </telerik:GridButtonColumn>
            <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="../images/icons/trash.png"
              UniqueName="ButtonDelete" CommandName="DeleteNote" ConfirmText="Are you sure you would like to delete this note?"
              ConfirmDialogType="RadWindow">
              <HeaderStyle Width="32px" />
            </telerik:GridButtonColumn>
            <telerik:GridBoundColumn DataField="Title" HeaderText="Title" UniqueName="Title">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Description" HeaderText="Description" UniqueName="Description"
              Visible="false"></telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="CreatorName" HeaderText="Author" UniqueName="CreatorName">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="DateCreated" HeaderText="Date Written" UniqueName="DateCreated">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="NoteID" UniqueName="NoteID" Visible="False">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="CreatorID" UniqueName="CreatorID" Visible="False">
            </telerik:GridBoundColumn>
          </Columns>
        </MasterTableView>
        <ClientSettings>
          <Selecting AllowRowSelect="True" />
          <ClientEvents OnRowSelected="RowSelected" />
          <Scrolling AllowScroll="True" UseStaticHeaders="false" />
        </ClientSettings>
      </telerik:RadGrid>
      </div>
    </telerik:RadPane>
    <telerik:RadSplitBar ID="RadSplitBar1" runat="server" />
    <telerik:RadPane ID="RadPane3" runat="server" Scrolling="None" BackColor="#ffffff">
      <iframe id="frmNotePreview" runat="server" scrolling="no" src="" frameborder="0"
        width="100%" height="100%" style="overflow:hidden;height:100%;width:100%"></iframe>
    </telerik:RadPane>
  </telerik:RadSplitter>
      <asp:HiddenField ID="fieldRefID" runat="server" />
  <asp:HiddenField ID="fieldRefType" runat="server" />

  <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

    <script type="text/javascript" language="javascript">

      function GridResized(sender, args) {
        top.privateServices.SetUserSetting('CustomerNoteGridHeight', sender.get_height());
      }

      function RowSelected(sender, args) {
        var frame = $get("<%=frmNotePreview.ClientID %>");
        var id = args.getDataKeyValue('NoteID');
        frame.setAttribute('src', 'NotePreview.aspx?NoteID=' + id);
        top.privateServices.SetUserSetting('SelectedCustomerNoteID', id);
      }

      function DialogClosed(sender, args) {
        RefreshGrid();
        sender.remove_close(DialogClosed);
      }

      function RefreshGrid() {
        var grid = $find("<%=gridNotes.ClientID %>").get_masterTableView();
        grid.rebind();
      }

      function ShowDialog(wnd) {
        wnd.add_close(DialogClosed);
        wnd.show();
        top.Ts.System.logAction('Organization Notes - Note Dialog Opened');
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
        if (value == 'AddNote') {
          ShowDialog(top.GetNoteDialog(null, GetRefID(), GetRefType()));
        }
      }



      function EditRow(id) {
        ShowDialog(top.GetNoteDialog(id));
      }
      function DeleteRow(id) {
        if (!confirm('Are you sure you would like to delete this note?')) return;
        top.privateServices.DeleteNote(id, RefreshGrid);
        top.Ts.System.logAction('Organization Notes - Note Deleted');
      }

    </script>
    
    

  </telerik:RadCodeBlock>
</asp:Content>
