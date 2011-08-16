﻿<%@ Page Title="Organization" Language="C#" MasterPageFile="~/Dialogs/Dialog.master"
  AutoEventWireup="true" CodeFile="MyCompany.aspx.cs" Inherits="Dialogs_Organization" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">
      <table id="tblCustomControls" runat="server" width="775px" cellpadding="0" cellspacing="5"
        border="0">
        <tr>
          <td class="labelColTD">
            Primary Contact:
          </td>
          <td class="inputColTD">
            <telerik:RadComboBox ID="cmbUsers" runat="server" Width="200px"></telerik:RadComboBox>
          </td>
          <td class="labelColTD">
            Website:
          </td>
          <td class="inputColTD">
            <telerik:RadTextBox ID="textWebSite" runat="server" Width="200px"></telerik:RadTextBox>
          </td>
        </tr>
        <tr>
          <td class="labelColTD">
            Time Zone:
          </td>
          <td class="inputColTD">
            <telerik:RadComboBox ID="cmbTimeZones" runat="server" Width="200px"></telerik:RadComboBox>
          </td>
          <td class="labelColTD">
            Internal SLA:
          </td>
          <td class="inputColTD">
            <telerik:RadComboBox ID="cmbSla" runat="server" Width="200px"></telerik:RadComboBox>
          </td>
        </tr>
        <tr>
          <td class="labelColTD">
            Default Wiki Article:
          </td>
          <td class="inputColTD">
            <telerik:RadComboBox ID="cmbWikiArticle" runat="server" Width="200px">
            </telerik:RadComboBox>
          </td>
          <td>Date Time Format:</td>
          <td>
            <telerik:RadComboBox ID="cmbDateFormat" runat="server" Width="200px">
            </telerik:RadComboBox>
          </td>
        </tr>
        <tr>
          <td class="labelColTD">
            Domains:
          </td>
          <td class="inputColTD">
            <telerik:RadTextBox ID="textDomains" runat="server" Width="200px"></telerik:RadTextBox>
          </td>
          <td class="labelColTD" colspan="2">
            <asp:CheckBox ID="cbTimeRequired" runat="server" Text="Require Time Spent on Timed Actions"/>
          </td>
        </tr>
        <tr>
          <td class="labelColTD" colspan="2">
            <asp:CheckBox ID="cbAdminCustomers" runat="server" Text="Only Admin Can Modify Customers"/>
          </td>
          <td class="labelColTD" colspan="2">
            <asp:CheckBox ID="cbAdminReports" runat="server" Text="Only Admin Can View Reports"/>
          </td>
        
        </tr>
        <tr>
          <td class="labelColTD" colspan="2">
            <asp:CheckBox ID="cbLinkTicketCustomersWithProducts" runat="server" Text="Only show products for the customers of a ticket."/>
          </td>
          <td class="labelColTD" colspan="2">
            <asp:CheckBox ID="cbRequireCustomer" runat="server" Text="Require customer for new ticket."/>
          </td>
        </tr>

        <tr>
          <td class="labelColTD" colspan="2">
            <asp:CheckBox ID="cbDisableStatusNotifications" runat="server" Text="Disable ticket status update emails"/>
          </td>
        </tr>
        <tr>
          <td>Business Day Start:</td>
          <td>
            <telerik:RadTimePicker ID="timeBDStart" runat="server">
            </telerik:RadTimePicker>
          </td>
          <td>Business Day End:</td>
          <td>
            <telerik:RadTimePicker ID="timeBDEnd" runat="server">
            </telerik:RadTimePicker>
          </td>
        </tr>
        <tr>
          <td>Business Days:</td>
          <td colspan="3">
            <asp:CheckBox ID="cbBDSunday" runat="server" Text="Sunday" />
            <asp:CheckBox ID="cbBDMonday" runat="server" Text="Monday" />
            <asp:CheckBox ID="cbBDTuesday" runat="server" Text="Tuesday" />
            <asp:CheckBox ID="cbBDWednesday" runat="server" Text="Wednesday" />
            <asp:CheckBox ID="cbBDThursday" runat="server" Text="Thursday" />
            <asp:CheckBox ID="cbBDFriday" runat="server" Text="Friday" />
            <asp:CheckBox ID="cbBDSaturday" runat="server" Text="Saturday" />
          </td>
        </tr>
        <tr>
          <td colspan="2">
            <asp:Button ID="btnResetEmail" runat="server" Text="Reset System Email" 
              Visible="false" onclick="btnResetEmail_Click"/>
          </td>
        </tr>
      </table>
      <div style="padding: 0 0 0 5px;">
        <div style="padding-bottom: 5px;">
          Description:
        </div>
        <div>
          <telerik:RadTextBox ID="textDescription" runat="server" Width="750px" TextMode="MultiLine"
            Height="75px"></telerik:RadTextBox>
        </div>
      </div>
    </div>
  </div>
</asp:Content>
