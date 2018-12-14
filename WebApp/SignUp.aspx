<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SignUp.aspx.cs" Inherits="SignUp"
  EnableViewState="false" Title="TeamSupport - Sign Up" %>

<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <meta name="description" content="">
  <meta name="author" content="">
  <title>TeamSupport - Sign Up</title>
  <link rel="SHORTCUT ICON" href="~/favicon.ico" />
  <link href="vcr/1_9_0/Css/bootstrap3.min.css" rel="stylesheet" type="text/css" />

  <script src="/frontend/library/jquery-latest.min.js" type="text/javascript"></script>

  <script src="/vcr/1_9_0/Js/bootstrap3.min.js" type="text/javascript"></script>

  <style type="text/css">
    .callout { background-color: #f4f8fa; padding: 20px; }
    .callout h3 { color: #3a87ad; }
    .callout blockquote { border-color: #bce8f1; }
    .container { margin-top: 10px; }
    .form-groupx label { display: none; }
    .errors { display: none; }
    .col-xs-offset-4 { margin-left: 33.333333%;}
    .form-group label { white-space: nowrap;}
    .btn, .btn:hover, .btn:focus, .btn:active {
    background-color: #B6311B;
    border-color: #B6311B;
    color: #FFFFFF;
    }
    body { color: #333333;}
  </style>

  <script type="text/javascript">
    var _resultID = -1;
    function getID() { return 34; } // _resultID; }
    function getURLParameter(name) { return decodeURIComponent((RegExp(name + '=' + '(.+?)(&|$)').exec(location.search) || [, null])[1]); }

    $(document).ready(function () {
      var _userID = -1;
      var submitting = false;
      var isCompact = getURLParameter('compact');
      var noLabels = getURLParameter('nolabels');
      //signup.aspx?compact=1&nolabels=1
      if (isCompact && isCompact == 1) {
        $('.main-column').removeClass('col-xs-8').addClass('col-xs-12');
        $('.right-column').remove();
      }

      if (noLabels && noLabels == 1) {
        $('.form-group label').hide();
      }

      $('#submit').click(function (e) {
        e.preventDefault();
        if (submitting == true) return;
        $('.errors ul').empty();
        var flag = false;

        $('input.required').each(function (index, el) {
          if ($.trim($(el).val()) == '') {
            $(el).closest('.form-group').addClass('has-warning');
            flag = true;
          }
        });

        if (flag == true) $('<li>').text('Please fill out the required fields in red.').appendTo('.errors ul');

        var pw = $.trim($('#password').val());

        if (pw.length < 6) {
          $('#password').closest('.form-group').addClass('has-warning');
          $('<li>').text('Your password must be at least 6 characters.').appendTo('.errors ul');
          flag = true;
        }

        if (flag == false) {
          $('.errors').hide();

          PageMethods.IsCompanyValid($('#company').val(), function (result) {
            if (result == false) {
              $('#company').closest('.form-group').addClass('has-warning');
              $('<li>').text('The company name already exists.  Please choose another name.').appendTo('.errors ul');
              $('.errors').show();
            }
            else {
              //public static bool SignMeUp(string firstName, string lastName, string email, string company, string phone, int version, string password, string promo, string interest, string seats, string process)
              submitting = true;
              $('#submit').hide();

              PageMethods.SignMeUp(
                $('#name').val(),
                $('#email').val(),
                $('#company').val(),
                $('#phone').val(),
                $('#product').val(),
                $('#password').val(),
                $('#promo').val(),
                function (result) {
                  if (result > 0) {
                    window.top.location = 'http://www.teamsupport.com/thank-you-for-trying-teamsupport/?userid=' + result;
                  }
                  else {
                    submitting = false;
                    $('#submit').show();
                  }
                });
            }
          });
        }
        else {
          $('.errors').show();
        }

      });
    });
  </script>

</head>
<body>
  <form id="form1" runat="server">
  <asp:ScriptManager ID="ScriptManager" runat="server" EnablePageMethods="true">
  </asp:ScriptManager>
  </form>
  <div class="">
    <div class="row">
      <div class="col-xs-8 main-column">
        <form role="form" class="form-horizontal">
        <div class="form-group">
          <label for="name" class="col-xs-4 control-label">Name</label>
          <div class="col-xs-8">
            <input type="text" class="form-control required" id="name" placeholder="Enter your full Name">
          </div>
        </div>
        <div class="form-group">
          <label for="email" class="col-xs-4 control-label">Email</label>
          <div class="col-xs-8">
            <input type="text" class="form-control required" id="email" placeholder="Enter your email address">
          </div>
        </div>
        <div class="form-group">
          <label for="company" class="col-xs-4 control-label">Company</label>
          <div class="col-xs-8">
            <input type="text" class="form-control required" id="company" placeholder="Enter your company name">
          </div>
        </div>
        <div class="form-group">
          <label for="phone" class="col-xs-4 control-label">Phone</label>
          <div class="col-xs-8">
            <input type="text" class="form-control required" id="phone" placeholder="Enter your phone number">
          </div>
        </div>
        <div class="form-group">
          <label for="product" class="col-xs-4 control-label">Product</label>
          <div class="col-xs-8">
            <select id="product" class="form-control">
              <option value="2" selected="selected">Enterprise</option>
              <option value="1">Support Desk</option>
            </select>
          </div>
        </div>
        <div class="form-group">
          <label for="password" class="col-xs-4 control-label">Password</label>
          <div class="col-xs-8">
            <input type="password" class="form-control required" id="password" placeholder="Enter your password">
          </div>
        </div>
        <div class="form-group">
          <label for="promo" class="col-xs-4 control-label">Promo Code</label>
          <div class="col-xs-8">
            <input type="text" class="form-control" id="promo" placeholder="Enter your promo code">
          </div>
        </div>
        <div class="errors alert alert-warning">
          <ul></ul>
        </div>
        <div class="form-group">
          <div class="col-xs-offset-4 col-xs-8">
            <button id="submit" type="submit" class="btn">Sign Up</button>
          </div>
        </div>
        <br />
        <div class="form-group">
          <div class="col-xs-offset-4 col-xs-8">
            <p>By clicking Sign Up, you agree to the <a href="http://www.teamsupport.com/help-desk-subscription-use-service-terms/"
              target="TSTermsOfService">Terms of Service</a> & <a href="http://www.teamsupport.com/privacy/"
                target="TSPrivacyPolicy">Privacy Policy</a></p>
          </div>
        </div>
        </form>
      </div>
      <div class="col-xs-4 right-column">
        <div class="callout">
          <h3>No Credit Card Required</h3>
          <p>Before your trial expires, we will notify you by email at which time you may elect
            to sign up and pay for your account. Otherwise, your account will expire 14 days
            from today. </p>
          <blockquote>
            <p>"I've used lots of different ticketing tools and I can say hands down that TeamSupport
              is the best I've used. If you're looking for quick and easy setup, flexibility and
              overall ease of use for support reps TeamSupport is for you." </p>
            <small><cite>Sean Wilson - PrintFleet Inc</cite></small> </blockquote>
          <blockquote>
            <p>"Delight your customers with excellent service using TeamSupport - it's customer
              support software done right!" </p>
            <small><cite>Eric Pita - ClinicSource</cite></small> </blockquote>
        </div>
      </div>
    </div>
  </div>
</body>
</html>
