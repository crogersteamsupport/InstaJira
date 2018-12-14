<%@ Page Title="Organization" Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true" CodeFile="Organization.aspx.cs" Inherits="Dialogs_Organization" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <script src="/frontend/library/jquery-latest.min.js" type="text/javascript"></script>
  <script src="/frontend/library/jquery.maskedinput.min.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">
<table id="tblCustomControls" runat="server" width="775px" cellpadding="0" cellspacing="5" border="0">
  <tr>
    <td class="labelColTD">Name:</td>
    <td class="inputColTD"><telerik:RadTextBox ID="textName" runat="server" Width="197px"></telerik:RadTextBox></td>
    <td class="labelColTD">Website:</td>
    <td class="inputColTD"><telerik:RadTextBox ID="textWebSite" runat="server"  Width="197px"></telerik:RadTextBox></td>
  </tr>
  <tr id="pnlEdit" runat="server">
    <td class="labelColTD">Primary Contact:</td>
    <td class="inputColTD"><telerik:RadComboBox ID="cmbUsers" runat="server" Width="200px"></telerik:RadComboBox></td>
    <td class="labelColTD"><span id="spanDefaultPortalGroup" runat="server">Default Portal Group:</span></td>
    <td class="inputColTD"><telerik:RadComboBox ID="cmbGroups" runat="server" Width="200px"></telerik:RadComboBox></td>
  </tr>
  <tr id="trSupportRow" runat="server">
    <td class="labelColTD">Default Support User:</td>
    <td class="inputColTD"><telerik:RadComboBox ID="cmbSupportUsers" runat="server" Width="200px"></telerik:RadComboBox></td>
    <td class="labelColTD">Default Support Group:</td>
    <td class="inputColTD"><telerik:RadComboBox ID="cmbSupportGroups" runat="server" Width="200px"></telerik:RadComboBox></td>
  </tr>
  <tr>
    <td class="labelColTD">Domains:</td>
    <td class="inputColTD"><telerik:RadTextBox ID="textDomains" runat="server" Width="200px"></telerik:RadTextBox></td>
    <td></td>
    <td></td>
  </tr>

  <tr id="trTimeZone" runat="server">
    <td class="labelColTD">Time Zone:</td>
    <td class="inputColTD" colspan="3"><telerik:RadComboBox ID="cmbTimeZones" runat="server" Width="350px"></telerik:RadComboBox></td>
  </tr>
  <tr>
    <td class="labelColTD"><span id="spanActive" runat="server">Active:</span></td>
    <td class="inputColTD"><asp:CheckBox ID="cbActive" runat="server" Text="" AutoPostBack="True" Width="200px" /></td>
    <td class="labelColTD" id="tdPortalLabel" runat="server">Portal Access:</td>
    <td class="inputColTD" id="tdPortalInput" runat="server"><asp:CheckBox ID="cbPortal" runat="server" Text="" AutoPostBack="True" Width="200px" /></td>
  </tr>
  <tr id="trApi" runat="server">
    <td>API Enabled:</td>
    <td><asp:CheckBox ID="cbApiEnabled" runat="server" Text="" Width="200px" /></td>
    <td colspan="2">API Token: <span id="spanApiToken" runat="server"></span></td>
  </tr>
  <tr>
    <td class="labelColTD">Service Agreement Expiration</td>
    <td class="inputColTD">
      <telerik:RadDatePicker ID="dpSAExpiration" runat="server" Width="200px"></telerik:RadDatePicker>
    </td>
    <td class="labelColTD">Service Level Agreement</td>
    <td class="inputColTD"><telerik:RadComboBox ID="cmbSlas" runat="server" Width="200px"></telerik:RadComboBox></td>
  </tr>
  <tr>
    <td class="labelColTD">Support hours per month:</td>
    <td class="inputColTD"><telerik:RadTextBox ID="textSupportHoursMonth" runat="server" Width="200px"></telerik:RadTextBox></td>
    <td></td>
    <td></td>
  </tr>
</table>

<div style="padding-left: 5px;">
<div>
  Description:
</div>  
<div>
  <telerik:RadTextBox ID="textDescription" runat="server"  Width="750px" TextMode="MultiLine" Height="75px"></telerik:RadTextBox> 
</div>

<asp:Panel ID="pnlActiveReason" runat="server">
  <div>
    Inactive Reason:</div>
  <div style="margin-bottom: 5px;">
    <telerik:RadTextBox ID="textActiveReason" runat="server" Width="100%"></telerik:RadTextBox>
  </div>
</asp:Panel>
</div>
</div>
</div>

</asp:Content>

