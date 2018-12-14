<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="ProductVersions.aspx.cs" Inherits="Frames_ProductVersions" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="pnlGrid" runat="server" style="width:100%; height:100%;">
  <telerik:RadGrid ID="gridVersions" runat="server" AutoGenerateColumns="False" 
  GridLines="None" Height="100%" OnItemDataBound="gridVersions_ItemDataBound"
  OnNeedDataSource="gridVersions_NeedDataSource" Width="100%" BorderWidth="0px">
<MasterTableView datakeynames="ProductVersionID">
<RowIndicatorColumn>
<HeaderStyle Width="20px"></HeaderStyle>
</RowIndicatorColumn>

<ExpandCollapseColumn>
<HeaderStyle Width="20px"></HeaderStyle>
</ExpandCollapseColumn>
  <Columns>
    <telerik:GridBoundColumn DataField="VersionNumber" 
      HeaderText="Version Number" UniqueName="VersionNumber">
    </telerik:GridBoundColumn>
    <telerik:GridBoundColumn DataField="VersionStatus" 
      HeaderText="Version Status" UniqueName="VersionStatus">
    </telerik:GridBoundColumn>
    <telerik:GridBoundColumn DataField="IsReleased" DataType="System.Boolean" 
      HeaderText="Released" UniqueName="IsReleased">
    </telerik:GridBoundColumn>
    <telerik:GridDateTimeColumn DataField="ReleaseDate" DataFormatString="{0:d}" 
      DataType="System.DateTime" HeaderText="Release Date" 
      UniqueName="ReleaseDate">
    </telerik:GridDateTimeColumn>
    <telerik:GridBoundColumn DataField="Description" 
      HeaderText="Description" UniqueName="Description">
    </telerik:GridBoundColumn>
  </Columns>
</MasterTableView>

  <ClientSettings>
    <Scrolling AllowScroll="True" UseStaticHeaders="False" />
  </ClientSettings>

<FilterMenu EnableTheming="True">
<CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
</FilterMenu>
</telerik:RadGrid>
</div>
</asp:Content>

