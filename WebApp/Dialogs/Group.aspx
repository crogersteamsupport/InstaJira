<%@ Page Title="Group" Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true"
  CodeFile="Group.aspx.cs" Inherits="Dialogs_Group" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div style="padding: 20px 20px;">
    <div>
      Group Name:</div>
    <div>
      <telerik:RadTextBox ID="textName" runat="server" Width="300px"></telerik:RadTextBox>
    </div>
    <br />
    <div>
      Group Description:</div>
    <div>
      <telerik:RadTextBox ID="textDescription" runat="server" Height="100px" TextMode="MultiLine"
        Width="300px"></telerik:RadTextBox>
    </div>
    <div id="divProductFamily" style="display: none" runat="server">
        <div style="padding-bottom: 3px;margin-top: 10px;">
        Product Line:</div>
        <div>
            <div style="margin-bottom: 10px;">
                <telerik:RadComboBox ID="cmbProductFamilies" runat="server" Width="200px" ExpandAnimation-Type="None"
                CollapseAnimation-Type="None">
                </telerik:RadComboBox>
            </div>
        </div>
    </div>
  </div>
</asp:Content>
