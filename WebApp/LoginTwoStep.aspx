<%@ Page Language="C#" %>

<!DOCTYPE html>

<script runat="server">

</script>

<html>
<head>
  <meta charset="utf-8">
  <title>TeamSupport - Login</title>
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <link href="vcr/1_9_0/Css/bootstrap3.min.css" rel="stylesheet" type="text/css" />
  <link href="vcr/1_9_0/Pages/LoginTwoStep.css" rel="stylesheet" type="text/css" />
  <script src="/frontend/library/jquery-latest.min.js" type="text/javascript"></script>
  <script src="/frontend/library/bootstrap3.min.js" type="text/javascript"></script>
  <script src="/vcr/1_9_0/Pages/LoginTwoStep.js" type="text/javascript"></script>
  <script src="/vcr/1_9_0/Js/Ts/ts.utils.js"></script>
</head>
<body>
  <header>
    <div class="headerDiv">
      <img alt="Logo" src="/vcr/1_9_0/images/page_header.png" />
    </div>
  </header>
  <div class="container-fluid">
    <div class="card card-container" style="padding-bottom:5px;">
      <h3 class="pageTitle">Enter your Verification Code</h3>
      <form class="form-signin">
        <div class="form-group">
          <input id="inputVerificationCode" class="form-control" placeholder="Verification Code" autocomplete="off" min="10000000" maxlength="10" required autofocus>
        </div>
        <button id="verify" class="btn btn-lg btn-primary btn-block btn-signin">Verify</button>
        <button id="resendCode" class="btn btn-link">Resend Verification Code</button>
      </form>
	 	<div id="codeResent" class="alert alert-info" style="display:none; margin-top:15px; margin-bottom: 10px;" role="alert">Verification Code resent. <br /></div>
	 	<div id="pageError" class="alert alert-danger" style="display:none; margin-top:15px; margin-bottom: 0px;" role="alert"></div>
        <div id="codeInfo" class="alert alert-info"></div>
    </div>
  </div>
 </body>
</html>