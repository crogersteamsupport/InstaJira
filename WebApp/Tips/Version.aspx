<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Version.aspx.cs" Inherits="Tips_Version" EnableViewState="false"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
</head>
<body>
  <div>
    <h1><a id="tipProduct" runat="server" href="#"></a></h1>
    <h2><a id="tipVersion" runat="server" href="#"></a></h1>
    <div class="ui-widget-content ts-separator"></div>
        <dl>
      <dt>Status</dt>
      <dd id="tipStatus" runat="server"></dd>
      <dt>Is Released</dt>
      <dd id="tipReleased" runat="server"></dd>
      <dt>Release Date</dt>
      <dd id="tipDate" runat="server"></dd>
    </dl>

    <p id="tipDesc" runat="server"></p>
    <div class="ui-helper-clearfix"></div>
  </div>
</body>
</html>
