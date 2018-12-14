<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="TicketTabsProduct.aspx.cs" Inherits="Frames_TicketTabsProduct" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <telerik:RadSplitter VisibleDuringInit="false" ID="RadSplitter1" runat="server" Height="100%" Width="100%" Orientation="Horizontal" BorderSize="0" BackColor="#DBE6F4">
    <telerik:RadPane ID="RadPane1" runat="server" Height="40px" BackColor="#BFDBFF" Scrolling="None">
      <div style="padding-top: 14px;">
       <telerik:RadTabStrip ID="tsMain" runat="server" SelectedIndex="0" OnClientTabSelected="TabSelected"></telerik:RadTabStrip>
       </div>

    </telerik:RadPane>
    <telerik:RadPane ID="RadPane2" runat="server" Height="100%" Scrolling="None">
    <iframe id="ticketsFrame" runat="server" class="contentFrame" scrolling="no" src="" frameborder="0" height="100%" width="100%"></iframe>
    </telerik:RadPane>
  </telerik:RadSplitter>
  <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
    <script type="text/javascript" language="javascript">
      function TabSelected(sender, args)
      {
        var tab = args.get_tab();
        var frame = $get("<%=ticketsFrame.ClientID %>");
        frame.setAttribute('src', tab.get_value());

        top.privateServices.SetUserSetting('SelectedProductTicketTabIndex' + window.location, tab.get_index());
        top.Ts.System.logAction('Product Tickets - Tab Selected (' + tab.get_text() + ')');
      }
      function onShow() { try { var frame = $get("<%=ticketsFrame.ClientID %>").contentWindow.onShow() } catch (err) { }; }
      
    
            
  
    
    
    </script>
  
  </telerik:RadScriptBlock>

</asp:Content>

