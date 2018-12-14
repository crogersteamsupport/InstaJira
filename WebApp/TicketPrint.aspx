<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TicketPrint.aspx.cs" Inherits="TicketPrint" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Ticket Print Preview</title>
  <link href="vcr/1_9_0/Css/jquery-ui-latest.custom.css" rel="stylesheet" type="text/css" />
  <script src="/frontend/library/jquery-latest.min.js" type="text/javascript"></script>
  <script src="/frontend/library/jquery-ui-latest.custom.min.js" type="text/javascript"></script>
  <style type="text/css">
    .sectionDiv
    {
      background-color: #EDF0F5;
      color: #004394;
      padding: 0px 0 0px 5px;
      border: solid 1px #9FB0CF;
      font-size: 18px;
      font-weight: bold;
    }
    .actionHeaderDiv
    {
      color: #004394;
    	font-size: 16px; 
    	font-weight:bold;
    	background-color: #F0F4F7; 
    	padding: 2px 0 2px 5px;
    }
    .actionFooterDiv
    {
      color: #004394;
    	font-style: italic; 
    	border-top: dotted 1px #15428B; 
    	padding: 10px 3px 20px 3px; 
    }
    .actionBodyDiv
    {
    	padding: 10px 10px 10px 10px; 
    	}
    body
    {
      color:#15428B;
      font-size:14px;  
    }
  </style>
</head>
<body>
  <form id="form1" runat="server">
  <div style="padding: 10px 0 10px 0;">
    <div class="sectionDiv ui-corner-all">
        <div><asp:Label ID="lblTitle" runat="server" Text="Label" Font-Bold="True" Font-Size="18px"></asp:Label></div>
        <div><asp:Label ID="lblDescription" runat="server" Text="Label" Font-Bold="false" Font-Size="16px"></asp:Label></div>
    </div>
  </div>
  <table width="100%" cellpadding="0" cellspacing="5" border="0">
    <asp:Literal ID="litProperties" runat="server"></asp:Literal>
  </table>
  <br />
  <br />
  <div class="sectionDiv  ui-corner-all">
    Customers</div>
  <div style="padding: 7px 7px">
    <asp:Literal ID="litCustomers" runat="server"></asp:Literal>
    </div>
    <br />
    <br />

  <div class="sectionDiv ui-corner-all">
    Actions</div>
    <br />
    
    
  <table width="100%" cellpadding="0" cellspacing="5" border="0">
    <asp:Literal ID="litActions" runat="server"></asp:Literal>
  </table>
  </form>
  <script type="text/javascript" language="javascript">
    $(document).ready(function() {
  
  });
</script>
</body>
</html>
