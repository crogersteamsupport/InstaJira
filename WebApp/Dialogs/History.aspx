<%@ Page Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true" CodeFile="History.aspx.cs" Inherits="Dialogs_History" Title="History" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <telerik:RadGrid ID="gridActionLogs" runat="server" Width="100%" Height="100%" AutoGenerateColumns="False" 
      GridLines="None" OnNeedDataSource="gridActionLogs_NeedDataSource" BorderWidth="0px" AllowCustomPaging="True" AllowPaging="True" PageSize="50">
    
      
      <MasterTableView TableLayout="Fixed" Width="100%">
        <RowIndicatorColumn>
          <HeaderStyle Width="20px" />
        </RowIndicatorColumn>
        <ExpandCollapseColumn>
          <HeaderStyle Width="20px" />
        </ExpandCollapseColumn>
        <Columns>
          <telerik:GridBoundColumn DataField="ActionLogID" HeaderText="ActionLogID" UniqueName="ActionLogID" Display="false"></telerik:GridBoundColumn>
          <telerik:GridBoundColumn DataField="DateCreated" HeaderText="Date" UniqueName="DateCreated"></telerik:GridBoundColumn>
          <telerik:GridBoundColumn DataField="CreatorName" HeaderText="Actor" UniqueName="CreatorName"></telerik:GridBoundColumn>
          <telerik:GridBoundColumn DataField="Description" HeaderText="Description" UniqueName="Description"></telerik:GridBoundColumn>
        </Columns>
      </MasterTableView>
      <ClientSettings>
        <Scrolling AllowScroll="True" EnableVirtualScrollPaging="False" UseStaticHeaders="True" />
      </ClientSettings>
    </telerik:RadGrid>
  <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
  
  <script type="text/javascript">

    function openTicketWindow(ticketID) {
      top.Ts.MainPage.openTicket(ticketID, true);
    }

  </script>
  </telerik:RadScriptBlock> 

</asp:Content>

