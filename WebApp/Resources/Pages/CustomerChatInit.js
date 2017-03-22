﻿var chatOffline = false;

$(document).ready(function () {
    var chatID = Ts.Utils.getQueryValue("uid", window);
    var chatGuid = { chatGuid: chatID };

    IssueAjaxRequest("CheckChatStatus", { chatGuid: chatID },
    function (result) {
        chatOffline = !result;

        if (chatOffline) {
            $('.panel-heading').text("Live Chat is not available at this time.");
            $('.chatOfflineWarning').show();
        } else {
            $('.chatOfflineWarning').hide();
        }
        
        $('.chatRequestForm').show();
        GetChatSettings(chatID);
    },
    function (error) {
        console.log(error)
    });


    $("#newChatForm").submit(function (e) {
        e.preventDefault();
        $(this).prop("disabled", true);
        var contactInfo = { chatGuid: chatID, fName: $('#userFirstName').val(), lName: $('#userLastName').val(), email: $('#userEmail').val(), description: $('#userIssue').val() };

        if (chatOffline) {
            IssueAjaxRequest("OfflineChat", contactInfo,
            function (result) {
                console.log(result)
                window.location.replace('ChatThankYou.html');
            },
            function (error) {
                console.log(error)
            });
        }
        else {
            IssueAjaxRequest("RequestChat", contactInfo,
            function (result) {
                console.log(result)
                window.location.replace('Chat.html?chatid=' + result.ChatID + '&pid=' + result.RequestorID);
            },
            function (error) {
                console.log(error)
            });
        }
    });
});

function GetChatSettings(chatID) {
    var chatObject = { chatGuid: chatID };

    IssueAjaxRequest("GetClientChatPropertiesByChatGUID", chatObject,
    function (result) {
        if (!chatOffline) {
            $('.panel-heading').text(result.ChatIntro);
        }

        $("input:text:visible:first").focus();
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