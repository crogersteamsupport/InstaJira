<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TSTestAlive.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script type="text/javascript" language="javascript">
      setTimeout(function () {
        window.location = window.location;
      }, 5000);
    
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Label ID="lbl_Status" runat="server" Text="Pending..."></asp:Label>
    
    </div>
    </form>
</body>
</html>
