
var loginService = '/Services/LoginService.asmx/';
var resourcesURL = '/vcr/1_9_0/Pages/';
var returnURL = '/default.aspx';
var orgCookie = null;

$(document).ready(function() {
    ticketnum = top.Ts.Utils.getQueryValue("TicketNumber", window);
    returnURL = top.Ts.Utils.getQueryValue("ReturnUrl", window);
    if (returnURL == null) returnURL = '/default.aspx';
    var newSignUpID = top.Ts.Utils.getQueryValue("SignUpID", window);
    if (newSignUpID != null) {
        IssueAjaxRequest(loginService, "NewSignUpSignIn", {
            "userID": newSignUpID
        }, signInSuccess, function() {});
    }

    supportToken = top.Ts.Utils.getQueryValue("SupportToken", window);
    if (supportToken && supportToken != '') {
        IssueAjaxRequest(loginService, "SupportSignIn", {
            token: supportToken
        }, function(result) {
            window.location = '/Default.aspx';
        }, function() {});
        return;
    }

    IssueAjaxRequest(loginService, "GetMobileURL", {
        "userID": 1
    }, function(result) {
        $('#mobile-link').prop('href', result);
    });

    $('#signIn').click(function(e) {
        e.preventDefault();
        var email = $('#inputEmail').val();
        var org = $('#orgSelect').val();
        var rememberMe = $('#rememberMe').is(":checked");
        if (org == null) org = 1;
        var signInData = {
            email: email,
            password: $('#inputPassword').val(),
            organizationId: org,
            verificationRequired: true,
            rememberMe: rememberMe
        };

        IssueAjaxRequest(loginService, "SignIn", signInData, signInSuccess,
            function(error) {
                alert('There was a error signing you in.  Please try again. ')
            });
    });

    function signInSuccess(result) {
        switch (result.Result) { //Unknown = 0, Success = 1, Fail = 2, VerificationNeeded = 3, VerificationSetupNeeded = 4, ExipredPassword = 5
            case 1:
                window.location = (ticketnum > 0) ? '/?TicketNumber=' + ticketnum : '/';
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
                } else $('#numbAttempts').parent().hide();
                break;
        }
    }

    $('#inputEmail').change(function(e) {
        CheckEmailForOrgs($(this).val())
    });

    getRememberMe();

    function CheckEmailForOrgs(email) {
        //Code to lookup the email address entered and check for a valid user with multiple orgs.
        var emailLookupData = {
            email: email
        }
        IssueAjaxRequest(loginService, "GetCompanies", emailLookupData,
            function(result) {
                //success
                LoadCompanies(result);
            },
            function(error) {

            });
    }
});

function LoadCompanies(companies) {
    var companySelect = $('#orgSelect');
    companySelect.empty();

    if (companies && companies.length > 0) {
        for (var i = 0; i < companies.length; i++) {
            $('<option>').attr('value', companies[i].ID).text(companies[i].Label).appendTo(companySelect);
        }

        if (companies.length > 1) {
            companySelect.show();
            companySelect.val(orgCookie);
        } else companySelect.hide();
    } else companySelect.hide();
};

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
        dataFilter: function(data) {
            var jsonResult = eval('(' + data + ')');
            if (jsonResult.hasOwnProperty('d'))
                return jsonResult.d;
            else
                return jsonResult;
        },
        success: function(jsonResult) {
            successCallback(jsonResult);
        },
        error: function(error, errorStatus, errorThrown) {
            if (errorCallback) errorCallback(error);
        }
    });

}
