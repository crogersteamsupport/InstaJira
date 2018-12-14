<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="AccountInformation.aspx.cs" Inherits="Frames_AccountInformation" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<telerik:RadSplitter ID="RadSplitter1" runat="server" Height="100%" Width="100%"
  BorderSize="0">
  <telerik:RadPane ID="RadPane1" runat="server" Height="100%" Width="100%" Scrolling="Both">
    <div id="pnlInfo" runat="server" style="padding: 20px;">
      <h2>Account Information</h2>
      <br />
      <p>
        <table cellpadding="0" cellspacing="5" border="0">
          <asp:Literal ID="litAccount" runat="server"></asp:Literal>
        </table>
        <br />
        <asp:Button ID="btnInfo" runat="server" Text="Change Account Info" />
        <br />
      </p>
      <br />
      <h2>Billing Address</h2>
      <br />
      <p>
        <table cellpadding="0" cellspacing="0" border="0">
          <asp:Literal ID="litAddress" runat="server"></asp:Literal>
        </table>
        <br />
        <asp:Button ID="btnAddress" runat="server" Text="Change Address" />
        <br />
      </p>
      <br />
      <h2>Credit Card Information</h2>
      <br />
      <p>
        <table cellpadding="0" cellspacing="0" border="0">
          <asp:Literal ID="litCredit" runat="server"></asp:Literal>
        </table>
        <br />
        <asp:Button ID="btnCredit" runat="server" Text="Change Credit Card" />
        <br />
      </p>
      <br />
    </div>
  </telerik:RadPane>
</telerik:RadSplitter>

  <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

    <script type="text/javascript" language="javascript">
      function DialogClosed(sender, args) {
        sender.remove_close(DialogClosed);
        RefreshContent();
      }

      function ShowDialog(wnd) {
        wnd.add_close(DialogClosed);
        wnd.show();
      }

      function RefreshContent() {
        __doPostBack();
      }
    
    
    </script>

  </telerik:RadScriptBlock>


</asp:Content>

