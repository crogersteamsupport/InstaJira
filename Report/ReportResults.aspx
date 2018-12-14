<%@ Page Title="" Language="C#" MasterPageFile="~/Frame.master" AutoEventWireup="true"
  CodeFile="ReportResults.aspx.cs" Inherits="Frames_ReportResults" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="UserControls/ReportFilterControl.ascx" TagName="ReportFilterControl" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <style type="text/css">
    .slaWarning { background: #FFFB00;	}
    .slaViolation { background: #E86868;	}
    .rgSorted { background-color:Transparent;}
    
  </style>
  <!--[if gt IE 7]>
  <style type="text/css">
    .RadGrid_Office2007 .rgGroupHeader TD DIV DIV { top: 0px !important;}
    
  </style>
<![endif]-->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadAjaxManager ID="ajaxManager" runat="server" UpdatePanelsRenderMode="Inline"></telerik:RadAjaxManager>
  <telerik:RadSplitter ID="RadSplitter1" runat="server" Orientation="Horizontal" Height="100%"
    Width="100%" BorderSize="0" FullScreenMode="True">
    <telerik:RadPane ID="paneFilters" runat="server" Scrolling="Both" Height="175px" Visible="true"
      BackColor="#ECF3FC" OnClientResized="FiltersResized">
      <div id="divFilter" runat="server">
        <div style="font-size: 12px; padding: 7px 0 0 10px;">
          <uc1:ReportFilterControl ID="filterControl" runat="server" />
          <div style="padding: 5px 0 0 0;">
            <asp:Button ID="btnRefresh" runat="server" Text="Apply Filters to Report" OnClick="btnRefresh_Click" OnClientClick="top.Ts.System.logAction('Report Results - Filter Applied');"/></div>
        </div>
      </div>
    </telerik:RadPane>
    <telerik:RadSplitBar ID="barFilters" runat="server" Visible="true" />
    <telerik:RadPane ID="paneGrid" runat="server" Scrolling="None" Height="100%" Width="100%">
      <div class="stretchContentHolderDiv">
        <telerik:RadGrid ID="gridReport" runat="server" Height="100%" Width="100%" GridLines="None"
          BorderWidth="0px" ShowGroupPanel="True" AllowSorting="True" 
          AllowPaging="True" AllowFilteringByColumn="True"
          PageSize="20" OnNeedDataSource="gridReport_NeedDataSource" OnColumnCreated="gridReport_ColumnCreated"          
          OnItemDataBound="gridReport_ItemDataBound" AutoGenerateColumns="False"
          OnSortCommand="gridReport_SortCommand">
          <ExportSettings ExportOnlyData="True" IgnorePaging="True" FileName="Report" Excel-Format="ExcelML"
            OpenInNewWindow="True">
            <Excel Format="ExcelML"></Excel>
          </ExportSettings>
          <MasterTableView TableLayout="Auto">
            <PagerStyle AlwaysVisible="True" Mode="NextPrevAndNumeric" />
          </MasterTableView>
          <ClientSettings AllowColumnHide="True" AllowColumnsReorder="true" AllowDragToGroup="True"
            ColumnsReorderMethod="Reorder">
            <Scrolling AllowScroll="True" UseStaticHeaders="True" />
            <Resizing AllowColumnResize="true" ResizeGridOnColumnResize="true" />
            <ClientEvents OnColumnResized="ColumnResized" />
          </ClientSettings>
        </telerik:RadGrid>
      </div>
    </telerik:RadPane>
  </telerik:RadSplitter>
  <asp:HiddenField ID="fieldReportID" runat="server" />
  <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

    <script type="text/javascript" language="javascript">

      function TicketDblClick(sender, args) {
        var id = args.getDataKeyValue('Ticket_Number');
        //window.open('../Ticket.aspx?TicketNumber=' + id, 'Ticket' + id);
        top.Ts.MainPage.openTicket(id, true);
      }

      function OpenTicketTab(ticketNumber) {
        top.Ts.MainPage.openTicket(ticketNumber, true);
      }
      
      
      function ColumnResized(sender, args) {
        var ajaxManager = $find("<%= ajaxManager.ClientID %>");
        ajaxManager.ajaxRequest(args);
      }

      function GetReportID() {
        var field = $get("<%=fieldReportID.ClientID %>");
        return field.value;
      }
      function GetExportGrid() {
        var manager = $find("<%=ajaxManager.ClientID %>");
        manager.set_enableAJAX(false);
        return $find("<%=gridReport.ClientID %>").get_masterTableView();
      }


      function FiltersResized(sender, args) {
        top.privateServices.SetUserSetting('ReportFilterHeight_' + GetReportID(), sender.get_height());
      }

      function ExportPDF() {
        GetExportGrid().exportToPdf();
      }
      function ExportExcel() {
        GetExportGrid().exportToExcel();
      }
      function ExportCSV() {
        GetExportGrid().exportToCsv();
      }
    </script>

  </telerik:RadScriptBlock>
</asp:Content>
