<%@ Page Language="C#" MasterPageFile="~/StandardForm.master" AutoEventWireup="true" CodeFile="ResetPassword.aspx.cs" Inherits="ResetPassword" Title="TeamSupport - Reset Password" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <style type="text/css">
    select, input.text { width: 250px; }
    
    .caption { font-size: 1.2em; padding-bottom: 3px; }
    a.login-link { text-decoration: underline !important; font-size: 1.4em; }
  </style>

  <script type="text/javascript">

    $(document).ready(function () {
      $('#companies').combobox();
      $('button').button();

      $('#btnReset').click(function () {
        $('#divAlert').hide();
        PageMethods.ResetPW($('#email').val(), $('#companies').val(), function (result) {
            if (result[0] != null) {
              $('#divAlert').html(result[0]);
              $('#divAlert').show('fast');
            }
            else {
              window.location = result[1];
            }
          }
        );
        return false;

      });

      $('#email')
      .click(function () { loadCompanies($(this).val()); })
      .blur(function () { loadCompanies($(this).val()); })
      .keyup(function () { loadCompanies($(this).val()); });
      loadCompanies($("#email").val());

      function loadCompanies(email) {

        if (isEmailValid(email)) {
          PageMethods.GetCompanies(email, function (result) {

            $("#companies").html('');
            for (var i = 0; i < result.Items.length; i++) {
              $("#companies").append('<option value="' + result.Items[i].ID + '">' + result.Items[i].Label + '</option>');
            }

            $('#companies option:selected').removeAttr('selected');
            var lastID = $('#companies option[value="' + result.SelectedID + '"]').attr('selected', 'selected').html();
            if (lastID) $('#companies').combobox("update");

            if (result.Items.length < 2) {
              $('#divCompany').hide();
            }
            else {
              $('#divCompany').show();
            }

          });

        }
        else {
          $('#divCompany').hide();
        }
      }

      function isEmailValid(email) {
        var pattern = /^([a-zA-Z0-9_.-])+@([a-zA-Z0-9_.-])+\.([a-zA-Z])+([a-zA-Z])+/;
        return pattern.test(email);
      }
    });
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div style="float: left; margin: 20px 0 0 50px;">
    <img src="images/icons/Password_Generator.png" /></div>
  <div style="float: left;">
    <div style="padding: 20px 0 20px 50px; text-align: left;">
      <div style="padding-bottom: 10px;">
        <div style="font-size: 2em; font-weight: normal; padding-bottom: 10px;">
          Reset Password
        </div>
        <div class="ui-widget ui-state-highlight ui-corner-all ui-helper-hidden" id="divAlert"
          style="padding: 5px 10px;">
        </div>
      </div>
      <div style="width: 450px;">
        <div class="caption">
          Email Address:</div>
        <div>
          <input type="text" id="email" class="text ui-widget ui-widget-content ui-corner-all" /></div>
        <br />
        <div id="divCompany" class="ui-helper-hidden">
          <div class="caption">
            Company:</div>
          <div>
            <select id="companies">
              <option value="-1">Select a company...</option>
            </select>
          </div>
          <div style="clear:both;"></div>
        </div>
        <br />
        <br />
        <div>
          <button id="btnReset">Reset Password</button>
        </div>
        <div style="clear: both;">
          &nbsp</div>
      </div>
    </div>
  </div>
</asp:Content>
