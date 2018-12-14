<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true"
  CodeFile="OrganizationProducts.aspx.cs" Inherits="Frames_OrganizationProducts" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
 </asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline">
    <AjaxSettings>
      <telerik:AjaxSetting AjaxControlID="gridProducts">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="gridProducts" />
        </UpdatedControls>
      </telerik:AjaxSetting>
    </AjaxSettings>
  </telerik:RadAjaxManager>

 <telerik:RadSplitter ID="RadSplitter1" runat="server" Height="100%" Width="100%"
    Orientation="Horizontal" BorderSize="0">
    <telerik:RadPane ID="paneToolbar" runat="server" Height="32px" Scrolling="None">
      <telerik:RadToolBar ID="tbMain" runat="server" CssClass="NoRoundedCornerEnds" Width="100%" OnClientButtonClicked="ButtonClicked">
      <Items>
        <telerik:RadToolBarButton runat="server" Text="Associate Product" ImageUrl="~/images/icons/add.png" Value="AssociateProduct">
        </telerik:RadToolBarButton>
      </Items>
    </telerik:RadToolBar>
    </telerik:RadPane>
    <telerik:RadPane ID="RadPane2" runat="server" Scrolling="None" Height="100%" Width="100%">
  <div class="stretchContentHolderDiv">
    
        <telerik:RadGrid ID="gridProducts" runat="server" Height="100%" Width="100%" AutoGenerateColumns="False"
        BorderWidth="0px" GridLines="None"  OnNeedDataSource="gridProducts_NeedDataSource1"
        AllowPaging="True" OnPageSizeChanged="gridProducts_PageSizeChanged" 
          OnItemDataBound="gridProducts_ItemDataBound">
        <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true" />
        <MasterTableView DataKeyNames="OrganizationProductID" ClientDataKeyNames="OrganizationProductID" TableLayout="Auto">
          <Columns>
            <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="../images/icons/edit.png"
              UniqueName="ButtonEdit" CommandName="ShowEdit" >
              <HeaderStyle Width="32px" />
            </telerik:GridButtonColumn>
            <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="../images/icons/trash.png"
              UniqueName="ButtonDelete" CommandName="Delete">
              <HeaderStyle Width="32px" />
            </telerik:GridButtonColumn>
            <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="../images/icons/go.png"
              UniqueName="ButtonOpen" CommandName="Open">
              <HeaderStyle Width="32px" />
            </telerik:GridButtonColumn>
            <telerik:GridBoundColumn DataField="ProductName" HeaderText="Product Name" UniqueName="ProductName">
              <HeaderStyle Width="100px" />
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="VersionNumber" HeaderText="Version" UniqueName="VersionNumber">
              <HeaderStyle Width="100px" />
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="SupportExpiration" HeaderText="Support Expiration" UniqueName="SupportExpiration"
            DataFormatString="{0:d}"
            >
              <HeaderStyle Width="100px" />
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="VersionStatus" HeaderText="Status" UniqueName="VersionStatus">
              <HeaderStyle Width="100px" />
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="IsReleased" HeaderText="Released" UniqueName="IsReleased"
              DataType="System.Boolean">              <HeaderStyle Width="100px" />
</telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="ReleaseDate" HeaderText="Release Date" UniqueName="ReleaseDate"
              DataType="System.DateTime">
                            <HeaderStyle Width="100px" />
</telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="OrganizationProductID" UniqueName="OrganizationProductID"
              Visible="False">
                            <HeaderStyle Width="100px" />
</telerik:GridBoundColumn>
          </Columns>
        </MasterTableView><ClientSettings>
        <Resizing AllowColumnResize="true" ResizeGridOnColumnResize="true" />
          <Scrolling AllowScroll="True" UseStaticHeaders="True" />
        </ClientSettings>
      </telerik:RadGrid>
</div>
        
      </telerik:RadPane>
      </telerik:RadSplitter>
      
    <asp:HiddenField ID="fieldOrganizationID" runat="server" />
      
   <telerik:RadScriptBlock runat="server">
     
   <script type="text/javascript" language="javascript">
      function DialogClosed(sender, args)
      {
        RefreshGrid();
        sender.remove_close(DialogClosed);
      }
      
      function RefreshGrid()
      {
        var grid = $find("<%=gridProducts.ClientID %>").get_masterTableView();
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
        if (value == 'AssociateProduct') {
          ShowDialog(top.GetOrganizationProductDialog(null, GetOrganizationID()));
          top.Ts.System.logAction('Organization Products - Product Associated');

        }
      }



      function EditRow(id) {
        ShowDialog(top.GetOrganizationProductDialog(id));
      }
      function DeleteRow(id) {
        if (!confirm('Are you sure you would like to remove this product association?')) return;
        top.privateServices.DeleteOrganizationProduct(id, RefreshGrid);
        top.Ts.System.logAction('Organization Products - Product Removed');

      }
      function OpenProduct(id) {
        top.location = "../Default.aspx?OrganizationProductID=" + id;
        top.Ts.System.logAction('Organization Products - Product Opened');
      }
      </script>
   </telerik:RadScriptBlock> 
  </asp:Content>
