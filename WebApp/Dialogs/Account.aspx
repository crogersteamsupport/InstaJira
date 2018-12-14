<%@ Page Title="" Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true" CodeFile="Account.aspx.cs" Inherits="Dialogs_Account" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">

<table id="tblCustomControls" runat="server" width="775px" cellpadding="0" cellspacing="5" border="0">
  <tr>
    <td class="labelColTD">Is Cutomer Free:</td>
    <td class="inputColTD"><asp:CheckBox ID="cbFree" runat="server" /></td>
    <td class="labelColTD">Product Type:</td>
    <td class="inputColTD"><telerik:RadComboBox ID="cmbProductType" runat="server" Width="200px"></telerik:RadComboBox></td>
  </tr>
  <tr>
    <td class="labelColTD">Active:</td>
    <td class="inputColTD"><asp:CheckBox ID="cbActive" runat="server" /></td>
    <td class="labelColTD">Automatic Payment:</td>
    <td class="inputColTD"><asp:CheckBox ID="cbAuto" runat="server" /></td>
  </tr>
  <tr>
    <td class="labelColTD">Advanced Portal:</td>
    <td class="inputColTD"><asp:CheckBox ID="cbAdvancedPortal" runat="server" /></td>
    <td class="labelColTD">Basic Portal:</td>
    <td class="inputColTD"><asp:CheckBox ID="cbBasicPortal" runat="server" /></td>
  </tr>
  <tr>
    <td class="labelColTD">User Seats:</td>
    <td class="inputColTD">
      <telerik:RadNumericTextBox ID="numUserSeats" 
        runat="server" Width="197px" DataType="System.Int32" MinValue="0">
      <NumberFormat DecimalDigits="0" />
      </telerik:RadNumericTextBox></td>
    <td class="labelColTD">Price / User:</td>
    <td class="inputColTD"><telerik:RadNumericTextBox ID="numUserPrice" 
        runat="server" Width="197px" Type="Currency"></telerik:RadNumericTextBox></td>
  </tr>
  <tr>
    <td class="labelColTD">Extra Storage Units:</td>
    <td class="inputColTD"><telerik:RadNumericTextBox ID="numStorage" runat="server" Width="197px" DataType="System.Int32" MinValue="0">
      <NumberFormat DecimalDigits="0" />
      </telerik:RadNumericTextBox></td>
    <td class="labelColTD">Price / Storage Unit:</td>
    <td class="inputColTD"><telerik:RadNumericTextBox ID="numStoragePrice" 
        runat="server" Width="197px" Type="Currency"></telerik:RadNumericTextBox></td>
  </tr>
  <tr>
    <td class="labelColTD">Portal Seats:</td>
    <td class="inputColTD"><telerik:RadNumericTextBox ID="numPortalSeats" runat="server" Width="197px" DataType="System.Int32" MinValue="0">
      <IncrementSettings Step="10" />
      <NumberFormat DecimalDigits="0" />
      </telerik:RadNumericTextBox></td>
    <td class="labelColTD">Price / Advanced Portal:</td>
    <td class="inputColTD"><telerik:RadNumericTextBox ID="numPortalPrice" 
        runat="server" Width="197px" Type="Currency"></telerik:RadNumericTextBox></td>
  </tr>
  <tr>
    <td class="labelColTD">Chat Seats:</td>
    <td class="inputColTD"><telerik:RadNumericTextBox ID="numChatSeats" runat="server" Width="197px" DataType="System.Int32" MinValue="0">
      <IncrementSettings Step="10" />
      <NumberFormat DecimalDigits="0" />
      </telerik:RadNumericTextBox></td>
    <td class="labelColTD">Price / Chat:</td>
    <td class="inputColTD"><telerik:RadNumericTextBox ID="numChatPrice" 
        runat="server" Width="197px" Type="Currency"></telerik:RadNumericTextBox></td>
  </tr>
  <tr>
    <td class="labelColTD"></td>
    <td class="inputColTD"></td>
    <td class="labelColTD">Price / Basic Portal:</td>
    <td class="inputColTD"><telerik:RadNumericTextBox ID="numBasicPortalPrice" 
        runat="server" Width="197px" Type="Currency"></telerik:RadNumericTextBox></td>
  </tr>
    <tr>
    <td class="labelColTD">API Active:</td>
    <td class="inputColTD"><asp:CheckBox ID="cbApiActive" runat="server" /></td>
    <td class="labelColTD">API Enabled:</td>
    <td class="inputColTD"><asp:CheckBox ID="cbApiEnabled" runat="server" /></td>
  </tr>

    <tr>
    <td class="labelColTD">Inventory Enabled:</td>
    <td class="inputColTD"><asp:CheckBox ID="cbInventory" runat="server" /></td>
  </tr>

  <tr>
    <td class="labelColTD">Inactive Reason:</td>
    <td class="inputColTD" colspan="3"><telerik:RadTextBox ID="textInactive" runat="server" Width="587px"></telerik:RadTextBox></td>
  </tr>
</table>
</div>

</div>


</asp:Content>

