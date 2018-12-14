<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Ticket.aspx.cs" Inherits="Tips_Ticket" EnableViewState="false"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
</head>
<body>
  <div>
    <div>
        <span id="tipSla" runat="server" style="float:left; line-height:16px; margin-right: 3px;"></span>
        <a id="tipNumber" runat="server" href="#" style="float:left; line-height:16px;"></a>
    </div>
    <div class="ui-helper-clearfix"></div>
    <div id="tipName" runat="server" style="padding-top:.5em;"></div>
    <div class="ui-widget-content ts-separator"></div>
    <dl id="tipProps" runat="server"></dl>
    <div class="ui-helper-clearfix"></div>
  </div>
</body>
</html>
