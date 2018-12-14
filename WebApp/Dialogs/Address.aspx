<%@ Page Title="Address" Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true" CodeFile="Address.aspx.cs" Inherits="Dialogs_Address" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">
  <div style="margin: 0 auto;">
  <table width="250px" border="0" cellpadding="0" cellspacing="5" style="width:250px;">
    <tr>
      <td style="125px;">
        Description:
      </td>
      <td style="250px;">
        <telerik:RadTextBox ID="edtDesicription" runat="server" Width="250px"></telerik:RadTextBox>
      </td>
    </tr>
    <tr>
      <td style="125px;">
        Address Line 1:
      </td>
      <td style="250px;">
        <telerik:RadTextBox ID="edtLine1" runat="server" Width="250px"></telerik:RadTextBox>
      </td>
    </tr>
    <tr>
      <td style="125px;">
        Address Line 2:
      </td>
      <td style="250px;">
        <telerik:RadTextBox ID="edtLine2" runat="server" Width="250px"></telerik:RadTextBox>
      </td>
    </tr>
    <tr>
      <td style="125px;">
        Address Line 3:
      </td>
      <td style="250px;">
        <telerik:RadTextBox ID="edtLine3" runat="server" Width="250px"></telerik:RadTextBox>
      </td>
    </tr>
    <tr>
      <td style="125px;">
        City:
      </td>
      <td style="250px;">
        <telerik:RadTextBox ID="edtCity" runat="server" Width="250px"></telerik:RadTextBox>
      </td>
    </tr>
    <tr>
      <td style="125px;">
        State / Province:
      </td>
      <td style="250px;">
        <telerik:RadTextBox ID="edtState" runat="server" Width="250px"></telerik:RadTextBox>
      </td>
    </tr>
    <tr>
      <td style="125px;">
        Zip / Postal Code:
      </td>
      <td style="250px;">
        <telerik:RadTextBox ID="edtZip" runat="server" Width="250px"></telerik:RadTextBox>
      </td>
    </tr>
    <tr>
      <td style="125px;">
        Country:
      </td>
      <td style="250px;">
        <telerik:RadTextBox ID="edtCountry" runat="server" Width="250px"></telerik:RadTextBox>
      </td>
    </tr>
  </table>
</div>
</div>
</div>
</asp:Content>

