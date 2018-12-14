<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ScreenRVideo.aspx.cs" Inherits="ScreenRVideo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <video width="625" height="375" controls>
        <source src="https://s3.amazonaws.com/screenrlegacy/<%=Request.QueryString["VideoID"] %>.mp4" type="video/mp4">
        Your browser does not support the video tag.
      </video>
    </div>
    </form>
</body>
</html>
