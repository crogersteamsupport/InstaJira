<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SelectKBTicket.aspx.cs" Inherits="Editor_SelectKBTicket" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title></title>
  <link href="../Css/ui.css" rel="stylesheet" type="text/css" />
  <style type="text/css">
    body{background-color:#ffffff;}
  </style>
</head>

<body>
  <form id="form1" runat="server">
  <telerik:RadScriptManager ID="RadScriptManager1" runat="server" ScriptMode="Release" OutputCompression="Forced"></telerik:RadScriptManager>
  <telerik:RadFormDecorator ID="RadFormDecorator1" runat="server" />
  <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline">
  </telerik:RadAjaxManager>
  <div style="margin: 10px 0 10px 10px; width: 260px;">
    <div style="padding-bottom: 3px;" >Select a Ticket:</div>
    <telerik:RadComboBox ID="cmbTicket" runat="server" Width="250px" AllowCustomText="True"
      MarkFirstMatch="True" ShowToggleImage="false" EnableLoadOnDemand="True" LoadingMessage="Searching Tickets..."
      OnClientItemsRequesting="OnClientItemsRequesting" OnClientDropDownClosed="OnClientDropDownClosed"
      EmptyMessage="Enter a Ticket Number or Search Phrase" ShowDropDownOnTextboxClick="False" MaxHeight="100">
      <WebServiceSettings Path="~/Services/PrivateServices.asmx" Method="GetKBTicketByDescription" />
      <CollapseAnimation Duration="200" Type="OutQuint" />
    </telerik:RadComboBox>
    <div style="float:right; padding: 20px 10px 0 0; height: 30px;">
          <input type="button" onclick="javascript:okDialog();" value="OK" /> &nbsp
          <input type="button" onclick="javascript:cancelDialog();" value="Cancel" />
    </div>
    <div style="clear:both;"></div>
  </div>
  <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

    <script type="text/javascript">

      function OnClientItemsRequesting(sender, eventArgs) {
        var context = eventArgs.get_context();
        context["FilterString"] = eventArgs.get_text();
      }

      function OnClientDropDownClosed(sender, args) {
        sender.clearItems();
      }

      function GetComboText() {
        var combo = $find("<%=cmbTicket.ClientID %>");
        var result = combo.get_text();
        if (!result) return '';

        if (result == combo.get_emptyMessage()) return '';
        return result;
      }

      function GetSelectedValue() {
        var combo = $find("<%=cmbTicket.ClientID %>");
        return combo.get_value();
      }

      function GetSelectedText() {
        var combo = $find("<%=cmbTicket.ClientID %>");
        return combo.get_text();
      }

      if (window.attachEvent) {
        window.attachEvent("onload", initDialog);
      }
      else if (window.addEventListener) {
        window.addEventListener("load", initDialog, false);
      }

      var url;
      var workLink = null;

      function getRadWindow() {
        if (window.radWindow) {
          return window.radWindow;
        }
        if (window.frameElement && window.frameElement.radWindow) {
          return window.frameElement.radWindow;
        }
        return null;
      }

      function initDialog() {
        var clientParameters = getRadWindow().ClientParameters; //return the arguments supplied from the parent page
      }

      function cancelDialog() {
        getRadWindow().close();
      
      }
      function okDialog() //fires when the Insert Link button is clicked
      {
        var value = GetSelectedValue();
        if (value == '') {
          alert('Please select a valid ticket.');
          return;
        }

        var values = value.split(",");
        var id = values[0];
        getRadWindow().close(id); //use the close function of the getRadWindow to close the dialog and pass the arguments from the dialog to the callback function on the main page.
      }
    </script>

  </telerik:RadScriptBlock>
  </form>
</body>
</html>
