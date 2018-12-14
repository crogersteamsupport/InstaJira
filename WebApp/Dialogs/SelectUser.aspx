<%@ Page Title="Select User" Language="C#" MasterPageFile="~/Dialogs/Dialog.master"
  AutoEventWireup="true" CodeFile="SelectUser.aspx.cs" Inherits="Dialogs_SelectUser" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div class="stretchContentHolderDiv">
  <telerik:RadGrid ID="gridUsers" runat="server" AllowMultiRowSelection="True" GridLines="None"
    Height="100%" Width="100%" AutoGenerateColumns="False" OnNeedDataSource="gridUsers_NeedDataSource" BorderWidth="0px">
    <MasterTableView>
      <RowIndicatorColumn>
        <HeaderStyle Width="20px"></HeaderStyle>
      </RowIndicatorColumn>
      <ExpandCollapseColumn>
        <HeaderStyle Width="20px"></HeaderStyle>
      </ExpandCollapseColumn>
      <Columns>
        <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn">
          <HeaderStyle Width="50px" />
        </telerik:GridClientSelectColumn>
        <telerik:GridBoundColumn DataField="FirstName" UniqueName="FirstName" HeaderText="First Name">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="LastName" UniqueName="LastName" HeaderText="Last Name">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="UserID" DataType="System.Int32" Display="False"
          UniqueName="UserID"></telerik:GridBoundColumn>
      </Columns>
    </MasterTableView>
    <ClientSettings EnablePostBackOnRowClick="True">
      <Selecting AllowRowSelect="True" />
      <Scrolling AllowScroll="True" UseStaticHeaders="False" />
    </ClientSettings>
    <FilterMenu EnableTheming="True">
      <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
    </FilterMenu>
  </telerik:RadGrid>
</div></asp:Content>
