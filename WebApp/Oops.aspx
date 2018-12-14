<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Oops.aspx.cs" Inherits="Oops" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Oops</title>
  <link href="vcr/1_9_0/Css/jquery-ui-latest.custom.css" rel="stylesheet" type="text/css" />
  <link href="vcr/1_9_0/Css/jquery-ui-enhanced.css" rel="stylesheet" type="text/css" />
  <link href="vcr/1_9_0/Css/ts.ui.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div style="text-align:center; border:none;" class="ui-widget-content">
      <h1>Oops</h1>
      <p>It looks like we have encountered an unexpected problem.</p>
      <p><asp:Literal ID="litProblem" runat="server"></asp:Literal></p>  
    </div>
    </form>
</body>
</html>
