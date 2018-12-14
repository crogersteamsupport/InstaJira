<%@ Page Language="C#" MasterPageFile="~/StandardForm.master" AutoEventWireup="true" CodeFile="Message.aspx.cs" Inherits="Message" Title="TeamSupport.com" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">


  <div style="font-size: 30px; padding: 50px 0 50px 0; line-height: 50px;">
    <asp:Literal ID="lblMessage" runat="server"></asp:Literal>
  </div>
  <div ID="pnlContinue" runat="server" style="margin: 0 auto; text-align:center; width: 100%;">
    <asp:Button ID="btnContinue" runat="server" Text="Continue" onclick="btnContinue_Click"/>
  </div>
  <asp:Literal ID="litSignupTracking" runat="server"></asp:Literal>
</asp:Content>

