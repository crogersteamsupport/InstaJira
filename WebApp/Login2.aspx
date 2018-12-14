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
  <link href="vcr/1_9_0/Pages/Login.css" rel="stylesheet" type="text/css" />
  <script src="/frontend/library/jquery-latest.min.js" type="text/javascript"></script>
  <script src="/frontend/library/bootstrap3.min.js" type="text/javascript"></script>
  <script src="/vcr/1_9_0/Pages/Login2.js" type="text/javascript"></script>
  <script src="/vcr/1_9_0/Js/Ts/ts.utils.js"></script>
</head>
<body>
  <header>
    <div class="headerDiv">
      <img alt="Logo" src="/vcr/1_9_0/images/page_header.png" />
    </div>
  </header>
  <div class="container-fluid">
    <div class="card card-container">
      <h3 class="pageTitle">Sign in to TeamSupport</h3>
	 	<h6 class="text-muted" style="text-align: center; margin-bottom: 0px; display:none;">
	 		Login attempt <span id="numbAttempts">2</span> of 10
	 	</h6>
      <form class="form-signin">
        <span id="reauth-email" class="reauth-email"></span>
        <%--<input type="email" id="inputEmail" class="form-control" placeholder="Email address" autocomplete="off" required autofocus>--%>
        <label id="email">email address</label>
        <input type="password" id="inputPassword" class="form-control" placeholder="Password" autocomplete="off" required autofocus>
      <select id="orgSelect" class="form-control" style="display:none;">
      </select>
        <div id="remember" class="checkbox">
          <label>
            <input id="rememberMe" type="checkbox" value="remember-me"> Remember me
          </label>
        </div>
        <button id="signIn" class="btn btn-lg btn-primary btn-block btn-signin">Sign in</button>
      </form>
      <a id="forgotPW" href="ResetPassword.aspx?reason=forgot" class="forgot-password">
        Forgot your password?
      </a>
      <div id="loginError" class="alert alert-danger" style="display:none; margin-top:15px; margin-bottom: 0px;" role="alert"></div>
      <footer class="container">
        <div class="row">
          <div class="col-xs-6" style="border-right: 1px solid rgba(117, 117, 117, 0.15);">
			 	<a href="http://www.teamsupport.com/customer-support-software-free-trial">Create a free account</a>
          </div>
          <div class="col-xs-6">
            <a href="http://m.teamsupport.com">Mobile Site</a>
          </div>
        </div>
      </footer>
    </div>
  </div>
 </body>
</html>