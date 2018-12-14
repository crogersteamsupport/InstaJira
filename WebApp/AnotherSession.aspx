<%@ Page Title="" Language="C#" MasterPageFile="~/StandardForm.master" AutoEventWireup="true" CodeFile="AnotherSession.aspx.cs" Inherits="SessionExpired" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <style>
    .pageDiv { padding:0;}
  </style>
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
  <div class="ui-widget-content">
    <h1 style="margin: 2em 0 1em 0; font-size:38px;">You have been logged out.</h1>
    <p style="margin: 1em 5em 3em 5em; font-size:14px;">Your user name was used to log in somewhere else.  If you have any questions, please feel free to notify us at <a class="ts-link ui-state-default" href="mailto:support@teamsupport.com?subject=Session Expiration">support@teamsupport.com</a>.</p>
    <div runat="server" id="message"></div>
    <button style="margin: 1em 0 10em 0;">Continue</button>
  </div>
</asp:Content>


