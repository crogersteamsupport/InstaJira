var loginService = '/Services/LoginService.asmx/';
var resourcesURL = '/vcr/1_9_0/Pages/';
var returnURL = '/default.aspx';
var orgCookie = null;
var emailAddress = null;
var orgId = null;

$(document).ready(function () {
	returnURL = top.Ts.Utils.getQueryValue("ReturnUrl", window);
	if (returnURL == null) returnURL = '/default.aspx';
	emailAddress = top.Ts.Utils.getQueryValue("Email", window);
	orgId = top.Ts.Utils.getQueryValue("OrgID", window);
	$('#email').text(emailAddress);
	supportToken = top.Ts.Utils.getQueryValue("SupportToken", window);
	if (supportToken && supportToken != '')
	{
		IssueAjaxRequest(loginService, "SupportSignIn", { token: supportToken }, function (result) { window.location = '/Default.aspx'; }, function () { });
		return;
	}

  $('#signIn').click(function (e) {
    e.preventDefault();
    var rememberMe = $('#rememberMe').is(":checked"); 
    if (orgId == null) orgId = 1;
    var signInData = { email: emailAddress, password: $('#inputPassword').val(), organizationId: orgId, verificationRequired: true, rememberMe: rememberMe };

    IssueAjaxRequest(loginService, "SignIn", signInData,
    function (result) {
      switch (result.Result) {//Unknown = 0, Success = 1, Fail = 2, VerificationNeeded = 3, VerificationSetupNeeded = 4, ExipredPassword = 5
        case 1:
        	window.location = returnURL;
          break;
        case 3:
          window.location = '/LoginTwoStep.aspx?UserID=' + result.UserId;
          break;
        case 4:
          window.location = resourcesURL + 'LoginTwoStepSetup.html?UserID=' + result.UserId;
          break;
			case 5:
				window.location = resourcesURL + result.RedirectURL;
      		break;
        default:
        	$('#loginError').text(result.Error).show();
        	if (result.LoginFailedAttempts > 0 && result.LoginFailedAttempts <= 10) {
        		$('#numbAttempts').text(result.LoginFailedAttempts).parent().show();
        	}
        	else $('#numbAttempts').parent().hide();
          break;
      }
    },
    function (error) {
      alert('There was a error signing you in.  Please try again. ')
    });
  });

  getRememberMe();
});

function getRememberMe() {
    var cookies = Ts.Utils.getCookie('rm');
    if (cookies != null) {
        $('#inputEmail').val(cookies["a"]).change();
        orgCookie = cookies["b"];
        $('#rememberMe').attr('checked', 'checked');
    }
}

function IssueAjaxRequest(service, method, data, successCallback, errorCallback) {
  $.ajax({
    type: "POST",
    url: service + method,
    data: JSON.stringify(data),
    contentType: "application/json; charset=utf-8",
    dataType: "json",
	 cache: false,
    dataFilter: function (data) {
      var jsonResult = eval('(' + data + ')');
      if (jsonResult.hasOwnProperty('d'))
        return jsonResult.d;
      else
        return jsonResult;
    },
    success: function (jsonResult) {
      successCallback(jsonResult);
      //alert(jsonResult.UserId + ' ' + jsonResult.OrganizationId + ' ' + jsonResult.Error + ' ' + jsonResult.Result + ' ' + jsonResult.ResultValue);
    },
    error: function (error, errorStatus, errorThrown) {
      if (errorCallback) errorCallback(error);
    }
  });
}