<%@ Page Title="" Language="C#" MasterPageFile="~/StandardForm.master" AutoEventWireup="true" CodeFile="SiteDown.aspx.cs" Inherits="SiteDown"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <style type="text/css">
    button { padding: 1em 2em;}
  </style>

  <script language="javascript" type="text/javascript">
    setInterval(tryLogin, 20000);

    function tryLogin() {
    	window.location = location.protocol + '//' + location.host + '/login';
    }
  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <div style="font-size: 2em; padding: 20px 0 20px 0;">The TeamSupport servers are undergoing maintenance.</div>
  <div style="font-size: 2em; padding: 20px 0 20px 0;">Please try again later.</div>
  <button onclick="tryLogin(); return false;">Try Again</button>
  <br />
  <br />
  <br />
  <br />
</asp:Content>
