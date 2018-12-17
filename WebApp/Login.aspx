<%@ Page Language="C#" %>
<!DOCTYPE html>
<script runat="server"></script>
<html>
<head>
	<meta charset="utf-8">
	<title>TeamSupport - Login</title>
	<meta name="viewport" content="width=device-width, initial-scale=1" />
	<meta name="robots" content="noindex, nofollow" />
	<link href="vcr/1_9_0/Css/bootstrap3.min.css" rel="stylesheet" type="text/css" />
	<link href="vcr/1_9_0/Pages/Login.css?1544641635" rel="stylesheet" type="text/css" />
	<script src="/frontend/library/jquery-latest.min.js" type="text/javascript"></script>
	<script src="/frontend/library/bootstrap3.min.js" type="text/javascript"></script>
	<script src="/vcr/1_9_0/Pages/Login.js?1517848257" type="text/javascript"></script>
	<script src="/vcr/1_9_0/Js/Ts/ts.utils.js"></script>
</head>
<body>
	<div class="form-signin">
		<header><img src="/frontend/images/logo-ltbg.svg" alt="TeamSupport" /></header>
		<h1>Sign in to TeamSupport</h1>
		<p class="text-muted text-center" id="loginAttempts">Login attempt <span id="numbAttempts">2</span> of 10</p>
		<form role="form">
			<input type="email" id="inputEmail" class="form-control input-lg" placeholder="Email address" autocomplete="off" required autofocus />
			<input type="password" id="inputPassword" class="form-control input-lg" placeholder="Password" autocomplete="off" required />
			<select id="orgSelect" class="form-control input-lg"></select>
			<div class="table-2col">
				<div class="colA">
					<div id="remember" class="checkbox">
						<label><input id="rememberMe" type="checkbox" value="remember-me"> Remember me</label>
					</div>
				</div>
				<div class="colB">
					<a id="forgotPW" href="ResetPassword.aspx?reason=forgot" class="forgot-password">Forgot Your Password?</a>
				</div>
			</div>
			<button id="signIn" class="btn btn-lg btn-primary btn-block btn-signin">Sign in</button>
			<div id="loginError" class="alert alert-danger" role="alert"></div>
		</form>
		<footer>
			<p>Don't have an account? <a href="http://www.teamsupport.com/customer-support-software-free-trial">Create one for free.</a></p>			
		</footer>
	</div>
</body>
</html>