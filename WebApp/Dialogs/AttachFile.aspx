<%@ Page Title="Attach File" Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true" CodeFile="AttachFile.aspx.cs" Inherits="Dialogs_AttachFile" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div style="padding: 10px 10px;">
  <telerik:RadUpload ID="ulFile" runat="server" ControlObjectsVisibility="None" Width="370px"></telerik:RadUpload>
  <div>
    Description:</div>
  <telerik:RadTextBox ID="textDescription" runat="server" Height="100px" TextMode="MultiLine" Width="365px"></telerik:RadTextBox>
</div>

</asp:Content>

