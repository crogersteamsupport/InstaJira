<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="ProductOrganizations.aspx.cs" Inherits="Frames_ProductOrganizations" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline">
    <AjaxSettings>
      <telerik:AjaxSetting AjaxControlID="gridCustomers">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="gridCustomers" />
        </UpdatedControls>
      </telerik:AjaxSetting>
    </AjaxSettings>
  </telerik:RadAjaxManager>
  <div class="stretchContentHolderDiv">
<telerik:RadGrid ID="gridCustomers" runat="server" Height="100%" Width="100%" OnNeedDataSource="gridCustomers_NeedDataSource"
    AutoGenerateColumns="False" BorderWidth="0px" GridLines="None" 
      AllowPaging="True" onpagesizechanged="gridCustomers_PageSizeChanged" onitemdatabound="gridCustomers_ItemDataBound" 
      
      
      >
    <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true"/>
    <MasterTableView DataKeyNames="OrganizationProductID" TableLayout="Fixed">
      <Columns>
        <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="../images/icons/edit.png"
          UniqueName="ButtonEdit" CommandName="ShowEdit">
          <HeaderStyle Width="32px" />
        </telerik:GridButtonColumn>
        <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="../images/icons/trash.png"
          UniqueName="ButtonDelete" CommandName="Delete" ConfirmText="Are you sure you would like to remove this customer association?"
          ConfirmDialogType="RadWindow">
          <HeaderStyle Width="32px" />
        </telerik:GridButtonColumn>
        <telerik:GridBoundColumn DataField="Name" HeaderText="Customer Name"
          UniqueName="Name">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="VersionNumber" HeaderText="Version"
          UniqueName="VersionNumber">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="OrganizationProductID" UniqueName="OrganizationProductID"
          Visible="False">
        </telerik:GridBoundColumn>
      </Columns>
    </MasterTableView>
    <ClientSettings>
      <Scrolling AllowScroll="True" UseStaticHeaders="True" />
    </ClientSettings>
  </telerik:RadGrid></div>
  
  <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
     
   <script type="text/javascript" language="javascript">
      function DialogClosed(sender, args)
      {
        RefreshGrid();
        sender.remove_close(DialogClosed);
      }
      
      function RefreshGrid()
      {
        var grid = $find("<%=gridCustomers.ClientID %>").get_masterTableView();
        grid.rebind();
      }

      function ShowDialog(wnd) {
        wnd.add_close(DialogClosed);
        wnd.show();
      }

      function EditRow(id) {
        ShowDialog(top.GetOrganizationProductDialog(id));
      }
      function DeleteRow(id) {
        if (!confirm('Are you sure you would like to remove this customer association?')) return;
        top.privateServices.DeleteOrganizationProduct(id, RefreshGrid);
      }
      </script>
   </telerik:RadScriptBlock>
</asp:Content>

