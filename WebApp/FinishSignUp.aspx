<%@ Page Language="C#" MasterPageFile="~/StandardForm.master" AutoEventWireup="true" CodeFile="FinishSignUp.aspx.cs" Inherits="ChangePassword" Title="TeamSupport - Change Password" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <script language="javascript" type="text/javascript">
    $(document).ready(function () {
      $('#divAlert').hide();
      var token = Ts.Utils.getQueryValue('token');
      $('#btnChange').button().click(function (e) {
        $('#divAlert').hide();
        e.preventDefault();
        PageMethods.ChangePW($('#textPassword').val(), $('#textConfirm').val(), token, function (result) {
          if (result != "") {
            $('#divAlert ul').html(result);
            $('#divAlert').show('fast');
          }
          else {
            window.location = '/login'
          }
        });
      });
    });
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div id="divPasswordContent">
  <h1 style="padding: 20px 0;">Choose a New Password</h1>
  <div style="float: left; margin-left: 50px;"><img src="images/icons/Password_Generator.png" /></div>
  <div style="float: left;">
    <div style="padding-left: 50px; padding-bottom: 50px; text-align: left;">
      <div id="divAlert" class="ui-state-highlight ui-helper-hidden ui-corner-all" style="width: 300px; padding: 1em 1em; margin: 2em 0 2em 0; text-align:left;"><ul></ul></div>
      <br />
      <div>Your new password must be at least 6 characters long.</div>
      <br />
      <div style="width: 200px;">
        <div>New Password:</div>
        <div><input class="text" type="password" id="textPassword" style="width:100%"/></div>
        <br />
        <div>Confirm Password:</div>
        <div><input class="text" type="password" id="textConfirm" style="width:100%"/></div>
        <br />
        <button id="btnChange">Save Password</button>
      </div>
    </div>
  </div>
  </div>
</asp:Content>
