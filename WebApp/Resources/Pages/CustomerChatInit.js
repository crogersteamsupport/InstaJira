var chatOffline = false;
var _groupName = null;

$(document).ready(function () {
    var chatID = Ts.Utils.getQueryValue("uid", window);
    var fname = Ts.Utils.getQueryValue("fname", window);
    var lname = Ts.Utils.getQueryValue("lname", window);
    var email = Ts.Utils.getQueryValue("email", window);
    var customerHubID = Ts.Utils.getQueryValue("HubID", window);
    var msg = Ts.Utils.getQueryValue("msg", window);
    _groupName = Ts.Utils.getQueryValue("Group", window);
    _groupID = Ts.Utils.getQueryValue("GroupID", window);
    var chatGuid = { chatGuid: chatID };

	IssueAjaxRequest("CheckChatStatus", { chatGuid: chatID, groupName: _groupName, groupID: _groupID },
    function (result) {
        chatOffline = !result;

        if (chatOffline) {
            $('.panel-heading').text("Live Chat is not available at this time.");
            $('.chatOfflineWarning').show();
        } else {
            $('.chatOfflineWarning').hide();
        }
        
        $('.chatRequestForm').show();
        GetChatSettings(chatID, customerHubID);
    },
    function (error) {
        console.log(error);
    });


    $("#newChatForm").submit(function (e) {
        e.preventDefault();
        $(this).prop("disabled", true);
		var contactInfo = { chatGuid: chatID, fName: $('#userFirstName').val(), lName: $('#userLastName').val(), email: $('#userEmail').val(), description: $('#userIssue').val(), groupName: _groupName, groupID: _groupID };

        if (chatOffline) {
            IssueAjaxRequest("OfflineChat", contactInfo,
            function (result) {
                console.log(result);
                window.location.replace('ChatThankYou.html');
            },
            function (error) {
                console.log(error);
            });
        }
        else {
            IssueAjaxRequest("RequestChat", contactInfo,
            function (result) {
                console.log(result);
                window.location.replace('Chat.html?chatid=' + result.ChatID + '&pid=' + result.RequestorID);
            },
            function (error) {
                console.log(error);
            });
        }
    });

    if (fname != null && fname != 'undefined') {
        $("#userFirstName").val(fname);
    }
    
    if (lname != null && lname != 'undefined') {
        $("#userLastName").val(lname);
    }

    if (email != null && email != 'undefined') {
        $("#userEmail").val(email);
    }

    if (msg != null && msg != 'undefined') {
        $("#userIssue").val(msg);
    }
});

function SetupDeflectionListener(organizationID, customerHubID) {
    var deflector = new TSWebServices.DeflectorService();
    //var returnURL = Ts.Utils.getQueryValue("ReturnURL", window);

    if (customerHubID) {
        var typingTimer;
        var doneTypingInterval = 500;  //time in ms
        var $input = $('#userIssue');

        $input.on('keyup', function () {
            clearTimeout(typingTimer);
            typingTimer = setTimeout(doneTyping, doneTypingInterval);
        });

        $input.on('keydown', function () {
            clearTimeout(typingTimer);
        });
    }

    function doneTyping() {
        $('#deflection-results').html('');
        if ($input.val()) {
            deflector.FetchDeflections(organizationID, $input.val(), customerHubID, function (result) {
                debugger;
                var deflectionResults = JSON.parse(result.Result);
                if (deflectionResults.length > 0) {
                    $('#deflection-results').append('<h4>Suggested Solutions</h4>');
                }

                for (x = 0; x < deflectionResults.length; x++) {
                    $('#deflection-results').append('<a target="_blank" class="list-group-item" href="' + returnURL + '/knowledgeBase/' + deflectionResults[x].TicketId + '">' + deflectionResults[x].Name +'</a>');
                }
            });
        }
    }
}

function GetChatSettings(chatID, customerHubID) {
    var chatObject = { chatGuid: chatID };

    IssueAjaxRequest("GetClientChatPropertiesByChatGUID", chatObject,
    function (result) {

        if (!chatOffline) {
            $('.panel-heading').text(result.ChatIntro);
        }

        $("input:text:visible:first").focus();
		
		var imageUrl = '/dc/' + result.OrganizationID + '/chat/logo';
        $('.chat-logo').css('background-image', 'url(' + imageUrl + ')');

        SetupDeflectionListener(result.OrganizationID, customerHubID);
    },
    function (error) {

    });
}

var service = '../Services/ChatService.asmx/';
function IssueAjaxRequest(method, data, successCallback, errorCallback) {
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
        },
        error: function (error, errorStatus, errorThrown) {
            if (errorCallback) errorCallback(error);
        }
    });
}