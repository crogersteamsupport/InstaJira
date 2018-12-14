var loginService = '/Services/LoginService.asmx/';
var returnURL = '/';

$(document).ready(function () {
 
  $('#save').click(function (e) {
    e.preventDefault();
    var userId = Number(top.Ts.Utils.getQueryValue("UserID", window));
    var token = top.Ts.Utils.getQueryValue("Token", window);
    var pw1 = $('#pw1').val();
    var pw2 = $('#pw2').val();
    var userData = { userID: userId, token: token, pw1: pw1, pw2: pw2 };
    IssueAjaxRequest(loginService, "SavePassword", userData,
    function (result) {
    	if (result.length < 1)
    	{
    		alert('Your password has been saved.');
    		window.location = '/.';
    	}
    	else {
    		var err = $('#pageError').hide();
    		var ul = $('#pageError ul').empty();
    		for (var i = 0; i < result.length; i++) {
    			ul.append($('<li>').text(result[i]));
    		}
    		err.fadeIn();
    	}
    },
    function (error) {
      alert('There was a issue saving your password.');
    });
  })
});

function IssueAjaxRequest(service, method, data, successCallback, errorCallback) {
  $.ajax({
    type: "POST",
    url: service + method,
    data: JSON.stringify(data),
    contentType: "application/json; charset=utf-8",
    dataType: "json",
   /* dataFilter: function (data) {
      var jsonResult = eval('(' + data + ')');
      if (jsonResult.hasOwnProperty('d'))
        return jsonResult.d;
      else
        return jsonResult;
    },*/
    success: function (jsonResult) {
      successCallback(jsonResult.d);
    },
    error: function (error, errorStatus, errorThrown) {
      errorCallback(error);
    }
  });
}