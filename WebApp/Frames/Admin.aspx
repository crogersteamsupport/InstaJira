<%@ Page Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="Admin.aspx.cs" Inherits="Frames_Admin" Title="Untitled Page" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <telerik:RadSplitter VisibleDuringInit="false" ID="RadSplitter1" runat="server" Height="100%" Width="100%" Orientation="Horizontal" BorderSize="0" BackColor="#FAFAFA">
    <telerik:RadPane ID="RadPane1" runat="server" Height="40px" BackColor="#FCFDFD" Scrolling="None">
      <div style="padding-top: 14px;">
       <telerik:RadTabStrip ID="tsMain" runat="server" SelectedIndex="1"  
          OnClientTabSelected="TabSelected" PerTabScrolling="True" 
          ScrollChildren="True">
         <Tabs>
           <%--<telerik:RadTab runat="server" Text="My Company" Value="AdminCompany.aspx"></telerik:RadTab>--%>
           <telerik:RadTab runat="server" Text="My Company" Value="../vcr/1_9_0/Pages/Admin_Company.html" Visible="true"></telerik:RadTab>
           <telerik:RadTab runat="server" Text="My Portal" Value="../vcr/1_9_0/Pages/Admin_Portal.html" Visible="true"></telerik:RadTab>
<%--           <telerik:RadTab ID="tsHub" runat="server" Text="Customer Hub" Value="../vcr/1_9_0/Pages/Admin_Hub.html"></telerik:RadTab>--%>
           <telerik:RadTab runat="server" Selected="True" Text="Custom Fields" Value="../vcr/1_9_0/Pages/Admin_CustomFields.html"></telerik:RadTab>
           <telerik:RadTab runat="server" Text="Custom Properties" Value="AdminCustomProperties.aspx"></telerik:RadTab>
           <telerik:RadTab runat="server" Text="Ticket Automation" Value="../vcr/1_9_0/Pages/Admin_Automation.html" Visible="true"></telerik:RadTab>
           <telerik:RadTab runat="server" Text="SLA" Value="AdminSla.aspx"></telerik:RadTab>
           <telerik:RadTab runat="server" Text="Workflow" Value="AdminWorkflow.aspx"></telerik:RadTab>
           <telerik:RadTab runat="server" Text="Email" Value="AdminEmails.aspx"></telerik:RadTab>
           <telerik:RadTab runat="server" Text="Integration" Value="../vcr/1_9_0/Pages/Admin_Integration.html" Visible="true"></telerik:RadTab>
           <telerik:RadTab runat="server" Text="Ticket Templates" Value="AdminTicketTemplates.aspx"></telerik:RadTab>
           <%--<telerik:RadTab runat="server" Text="Import" Value="../vcr/1_9_0/Pages/Admin_Import.html" Visible="false"></telerik:RadTab>--%>
           <%--<telerik:RadTab runat="server" Text="Ticket Page Order" Value="../vcr/1_9_0/Pages/Admin_TicketOrder.html"></telerik:RadTab>--%>
         </Tabs>
        </telerik:RadTabStrip>
       </div>

    </telerik:RadPane>
    <telerik:RadPane ID="RadPane2" runat="server" Height="100%" Scrolling="None">
    <iframe id="frmAdmin" runat="server" class="contentFrame" scrolling="no" src="" frameborder="0" height="100%" width="100%"></iframe>
    </telerik:RadPane>
  </telerik:RadSplitter>
  <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
    <script type="text/javascript" language="javascript">

      var _lastRefreshTime = new Date();

      function pageLoad() {
        onShow();
        var tabStrip = $find("<%=tsMain.ClientID %>");
        changeTab(tabStrip.get_selectedTab());
      }

      function onShow() {
        parent.Ts.Settings.Organization.read('SelectedAdminTabText', 'My Company', function (tabText) {
          if (tabText == null) {
            $find("<%=tsMain.ClientID %>").get_allTabs()[0].select();
          }
          else {
            selectTab(tabText);
          }
        });
      }

      function refreshData() {
        if (_lastRefreshTime != null) {
          var now = new Date();
          var diff = (now - _lastRefreshTime) / 1000;
          if (diff < 300) return;
        }

        _lastRefreshTime = new Date();

        window.location = window.location;
      }    

      function TabSelected(sender, args) {
        var tab=args.get_tab();
        changeTab(tab);
        parent.Ts.System.logAction('Admin - Tab Selected (' + tab.get_text() + ')');
      }

      function selectTab(text) {
        var tabStrip = $find("<%=tsMain.ClientID %>");
        var tab = tabStrip.findTabByText(text);
        if (tab != null) {
          tab.select();
        }
        else {
          tabStrip.get_allTabs()[0].select();
        }

        var frame = $get("<%=frmAdmin.ClientID %>");
        try { if (frame.contentWindow.onShow) frame.contentWindow.onShow(); } catch (err) { }

      }

      function changeTab(tab) {
        var frame = $get("<%=frmAdmin.ClientID %>");
        frame.setAttribute('src', tab.get_value());
        parent.Ts.Settings.Organization.write('SelectedAdminTabText', tab.get_text());
      }
    
    </script>
  
  </telerik:RadScriptBlock>

</asp:Content>

