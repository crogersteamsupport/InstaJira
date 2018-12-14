<%@ Page Title="Select Group" Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true" CodeFile="SelectGroup.aspx.cs" Inherits="Dialogs_SelectGroup" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">


<telerik:RadGrid ID="gridGroups" runat="server" AllowMultiRowSelection="True" GridLines="None" Height="100%" Width="100%" AutoGenerateColumns="False" onneeddatasource="gridGroups_NeedDataSource">
  <MasterTableView>
    <Columns>
      <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn">
        <HeaderStyle Width="50px" />
      </telerik:GridClientSelectColumn>
      <telerik:GridBoundColumn DataField="Name" UniqueName="Name" HeaderText="Group Name"></telerik:GridBoundColumn>
      <telerik:GridBoundColumn DataField="GroupID" DataType="System.Int32" Display="False" UniqueName="GroupID"></telerik:GridBoundColumn>
    </Columns>
  </MasterTableView>
  <ClientSettings>
    <Selecting AllowRowSelect="True" />
    <Scrolling AllowScroll="True" UseStaticHeaders="False" />
  </ClientSettings>
</telerik:RadGrid>
</asp:Content>

