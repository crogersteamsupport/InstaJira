<%@ Page Language="C#" MasterPageFile="~/StandardForm.master" AutoEventWireup="true" CodeFile="Attachment.aspx.cs" Inherits="Attachment" Title="TeamSupport.com | Download Attachment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <script type="text/javascript" language="javascript">
    var t = setTimeout("openAttachment();", 2000);
  
    function openAttachment()
    {
      __doPostBack();
    }
    
  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <div style="width: 300px; text-align:left;">
  <h2>Attachment Download</h2>
  <br />
  File: <asp:Label ID="lblFileName" runat="server" Text="Label"></asp:Label>
  <br />
  Size: <asp:Label ID="lblSize" runat="server" Text="Label"></asp:Label>
  <br />
  <br />

   Your download should begin shortly.<br />
   Please click <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="clearTimeout(t);">here</asp:LinkButton> if your download does not begin.
   </div>
  
</asp:Content>

