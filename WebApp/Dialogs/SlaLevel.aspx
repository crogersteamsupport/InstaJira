<%@ Page Title="Service Level Agreement" Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true"
  CodeFile="SlaLevel.aspx.cs" Inherits="Dialogs_SlaLevel" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">
      <div style="padding-bottom: 10px;">Service Level Agreement Name:</div>
      
      <div>
        <telerik:RadTextBox ID="textName" runat="server" Width="225px"></telerik:RadTextBox>
      </div>
    </div>
  </div>
</asp:Content>
