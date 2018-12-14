<%@ Page Title="" Language="C#" MasterPageFile="~/StandardForm.master" AutoEventWireup="true" CodeFile="SessionExpired.aspx.cs" Inherits="SessionExpired" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <script language="javascript" type="text/javascript">
    $(document).ready(function () {
      $('button').button().click(function (e) {
        e.preventDefault();
        window.location = '.';
      });

    });
  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <h1 style="padding: 5em 0 1em 0;">Your session has expired.</h1>
  <div runat="server" id="message"></div>
  <button style="margin: 1em 0 10em 0;">Continue</button>
</asp:Content>


