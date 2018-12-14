<%@ Page Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true" CodeFile="SelectOrganization.aspx.cs" Inherits="Dialogs_SelectOrganization" Title="Select Organizations" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<div style="height: 200px; width: 370px; padding: 5px 0 0 5px;"> 
<div class="stretchContentHolderDiv">
<telerik:RadGrid ID="gridOrganizations" runat="server" AllowMultiRowSelection="True" GridLines="None" Height="100%" Width="100%" 
 AutoGenerateColumns="False" OnNeedDataSource="gridOrganizations_NeedDataSource"   ShowHeader="false">
  <MasterTableView TableLayout="Fixed">
    <Columns>
      <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn">
        <HeaderStyle Width="25px" />
      <ItemStyle BorderWidth="0px" />
      </telerik:GridClientSelectColumn>
      <telerik:GridBoundColumn DataField="Name" UniqueName="Name" HeaderText="Company Name">
      <ItemStyle BorderWidth="0px" />
      </telerik:GridBoundColumn>
      <telerik:GridBoundColumn DataField="OrganizationID" DataType="System.Int32" Display="False" UniqueName="OrganizationID">
      <ItemStyle BorderWidth="0px" />
      </telerik:GridBoundColumn>
    </Columns>
  </MasterTableView>
  <ClientSettings>
    <Selecting AllowRowSelect="True" />
    <Scrolling AllowScroll="True" UseStaticHeaders="True" />
  </ClientSettings>
</telerik:RadGrid>
</div>
</div>
</asp:Content>

