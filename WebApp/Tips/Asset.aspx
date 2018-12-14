<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Asset.aspx.cs" Inherits="Tips_Asset" EnableViewState="false"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
    <style>
      .tip-customer-expired { color: Red;}
      .ui-cluetip-content dd { width: 60%; }
      .ui-cluetip-content dt { width: 35%;}
    </style>
</head>
<body>
  <div>
    <h1><a id="tipAsset" runat="server" href="#"></a></h1>
    <div class="ui-widget-content ts-separator"></div>
    <dl id="tipProps" runat="server">
      <dt>Email</dt>
      <dd><a href="mailto:kevin@kevinjones.ws">kevin@kevinjones.ws</a></dd>
      <dt>Mobile</dt>
      <dd>682-583-8509</dd>
      <dt>Office</dt>
      <dd>817-886-0797</dd>
    </dl>
    <div class="ui-helper-clearfix"></div>
  </div>
</body>
</html>
