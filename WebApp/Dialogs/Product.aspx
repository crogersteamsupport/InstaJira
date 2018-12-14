<%@ Page Title="Product" Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true" CodeFile="Product.aspx.cs" Inherits="Dialogs_Product" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <script src="/frontend/library/jquery-latest.min.js" type="text/javascript"></script>
  <script src="/frontend/library/jquery.maskedinput.min.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

  <div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">
<div>Product Name:</div>
<div>
  <telerik:RadTextBox ID="textName" runat="server" Width="745px">
  </telerik:RadTextBox>
</div>
<br />
<div>Product Description:</div>
<div>
  <telerik:RadTextBox ID="textDescription" runat="server" 
    TextMode="MultiLine" Width="745px">
  </telerik:RadTextBox>
</div>
<br />

<table id="tblCustomControls" runat="server" width="775px" cellpadding="0" cellspacing="5" border="0">
</table>
</div>

</div>



</asp:Content>

