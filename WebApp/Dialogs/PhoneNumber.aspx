<%@ Page Title="Phone Number" Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true" CodeFile="PhoneNumber.aspx.cs" Inherits="Dialogs_PhoneNumber" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div style="padding: 10px 10px;">
  <div>
    Phone Type</div>
  <div style="margin-bottom: 10px;">
    <telerik:RadComboBox ID="cmbTypes" runat="server" Width="200px">
      <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
    </telerik:RadComboBox>
  </div>
  <div style="margin-bottom: 10px;">
    <table border="0" cellpadding="0" cellspacing="0">
      <tr>
        <td>
          Phone Number
        </td>
        <td>
          <div style="margin-left: 5px;">
            Ext
          </div>
        </td>
      </tr>
      <tr>
        <td>
          <telerik:RadTextBox ID="edtPhoneNumber" runat="server" Width="200px"></telerik:RadTextBox>
        </td>
        <td>
          <div style="margin-left: 5px;">
            <telerik:RadTextBox ID="edtExtension" runat="server" Width="50px"></telerik:RadTextBox>
          </div>
        </td>
      </tr>
    </table>
  </div>
</div>
</asp:Content>

