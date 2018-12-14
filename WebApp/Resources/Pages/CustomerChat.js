var _activeChatID = null;
var _participantID = null;
var _timer;
var _toktimer;
var showAvatars = true;
var isTyping = false;
var _agentName;
var _isChatWindowActive = true;
var _isChatWindowPotentiallyHidden = false;
var siteUrl;
var _agentHasJoined = false;
var _typingTimer;                //timer identifier
var _eventsList;
var isSubmittingAlready = false;

$(document).ready(function () {
    var windowUrl = window.location.href;
    var arr = windowUrl.split("/");
    siteUrl = arr[0] + "//" + arr[2];
    var chatID = Ts.Utils.getQueryValue("chatid", window);
    _activeChatID = chatID;
    var participantID = Ts.Utils.getQueryValue("pid", window);
    _participantID = participantID;
    var chatObject;
    var channel;
    var chatInfoObject = {};
    var isSafari = Object.prototype.toString.call(window.HTMLElement).indexOf('Constructor') > 0 || (function (p) { return p.toString() === "[object SafariRemoteNotification]"; })(!window['safari'] || safari.pushNotification);
    var isIE = /*@cc_on!@*/false || !!document.documentMode;
    var isEdge = !isIE && !!window.StyleMedia;

    IssueAjaxRequest("GetPusherKey", null,
        function (result) {
            var pusherKey = result;
            setupChat(chatID, participantID, pusherKey, function (channelObject) {
                channel = channelObject;

                _timer = setTimeout(function () {
                    var data = { chatID: chatID }

                    if (!_agentHasJoined) {
                        IssueAjaxRequest("MissedChat", data,
                            function (result) {
                                window.location.replace('ChatThankYou.html');
                            },
                            function (error) {
                                console.log(error)
                            });
                    }
                }, 180000);

                channel.bind('agent-joined', function (data) {
                    _agentHasJoined = true;
                    $('#operator-message').remove();
                    clearTimeout(_timer);

                    var isCustomerTOKEnabled = true;

                    if (isSafari || isEdge) {
                        isCustomerTOKEnabled = false;
                    }

                    DisplayTOKButtons(data.isAgentTOKEnabled && isCustomerTOKEnabled);
                    GetChatSettings(chatID);

                    if (data.isAgentTOKEnabled && !isCustomerTOKEnabled) {
                        _toktimer = setTimeout(function () {
                            var triggered = presenceChannel.trigger('client-tok-enabled', { isCustomerTOKEnabled: isCustomerTOKEnabled });
                            clearTimeout(_toktimer);
                        }, 1000);
                    }
                });
            });
        },
        function (error) {
            console.log(error)
        }
	);

	IssueAjaxRequest("GetHTMLGlobalEventAttributes", null,
		function (result) {
			_eventsList = result;
		},
		function (error) {
			console.log(error)
		}
	);

    //The order of the following 3 statements matter, do not change.
    SetupTOK();
    DisplayTOKButtons(!isSafari && !isEdge);
    GetChatSettings(chatID);

    loadInitialMessages(chatID, participantID);

    $("#message-form").submit(function (e) {
        e.preventDefault();
        if ($('#message').val() !== '' && !isSubmittingAlready) {
            isSubmittingAlready = true;
            $('#send-message').prop("disabled", true);
            clearTimeout(_typingTimer);
            doneTyping();
            var messageString = replaceURLs($('#message').val());
            var messageData = { channelName: 'presence-' + chatID, message: messageString, chatID: chatID, userID: participantID };

            IssueAjaxRequest("AddMessage", messageData,
            function (result) {
                $('#message').val('');
            },
            function (error) {
                console.log(error)
            });
        }
    });

    function replaceURLs(text) {
        var urlRegex = /(\b(https?|ftp|file):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/ig;

        return text.replace(urlRegex, function (url) {
            return '<a target="_blank" href="' + url + '">' + url + '</a>';
        })
    }

    //TODO:  Not centering correclty
    //$('#chat-tok-audio').tooltip({
    //    container: 'body'
    //});

    $("#jquery_jplayer_1").jPlayer({
        ready: function (event) {
            $(this).jPlayer("setMedia", {
                mp3: "../vcr/1_9_0/Audio/chime.mp3"
            });
        },
        loop: false,
        swfPath: ""
    });

    window.onbeforeunload = function (event) {
        if (!_agentHasJoined) {
            var chatData = { chatID: chatID, userID: participantID };
            IssueAjaxRequest("CustomerAbandonedChatRequest", chatData,
                function (result) {
                },
                function (error) {
                    console.log(error);
                });
        }
    };
});

function GetChatSettings(chatID) {
    var chatObject = { chatID: chatID };
    
    IssueAjaxRequest("GetClientChatPropertiesByChatID", chatObject,
    function (result) {
		if (!result.TOKScreenEnabled) {
			$('.dropdown-menu li:contains(Screen)').hide();
			$('#chat-tok-screen-Icon').hide();
		}
		if (!result.TOKVideoEnabled) {
			$('.dropdown-menu li:contains(Video)').hide();
			$('#chat-tok-video-Icon').hide();
		}
		if (!result.TOKVoiceEnabled) {
			$('.dropdown-menu li:contains(Audio)').hide();
			$('#chat-tok-audio-Icon').hide();
		}

        showAvatars = result.ChatAvatarsEnabled
        $('.panel-heading').text(result.ChatIntro);
    },
    function (error) {
        console.log(error)
    });
}

function createMessage(message, agent)
{
    if (!$("#agent" + agent).length) {
        $('.chat-intro').append('<p id="agent' + agent + '">' + message + '</p>');
    }
}

function createMessageElement(messageData, direction) {
    var userAvatar = '../vcr/1_9_0/images/blank_avatar.png';

    if (messageData.CreatorID !== null && showAvatars) {
        userAvatar = '../dc/' + chatInfoObject.OrganizationID + '/UserAvatar/' + messageData.CreatorID + '/48/1469829040429';
    } else if (messageData.CreatorID !== null && !showAvatars) {
        userAvatar = '../dc/' + chatInfoObject.OrganizationID + '/InitialAvatar/' + messageData.CreatorInitials + '/48/1469829040429';
    }

    var hasLeftChatClass = "";

    //If the message comes from the Agent
    if (direction == 'left') {
        _agentHasJoined = true;

        //did Agent left the chat?. Also if memberCount is 2 then it means that there is only the customer in the chat now (the agent member is removed off the channel after, if there were more than 2 members then there are still agent(s) on the other side chatting.
        if (messageData.HasLeft) {
            hasLeftChatClass = " hasLeft";
        }
    }

	var displayMessage = $("<div>").text(messageData.Message).html();

	var containsProhibitedText = false;
	var i = 0;

	while (i < _eventsList.length && !containsProhibitedText) {
		containsProhibitedText = messageData.Message.trim().indexOf("<script ") >= 0 || messageData.Message.trim().indexOf(" " + _eventsList[i] + "=") >= 0;
		i++;
	}

	if ((messageData.Message.trim().indexOf("<img ") == 0 && !containsProhibitedText)
		|| (messageData.Message.trim().indexOf("/chatattachments/") > 0 && !containsProhibitedText)
		|| (messageData.Message.trim().indexOf('<a target="_blank" href=') >= 0 && !containsProhibitedText)) {
		displayMessage = messageData.Message;
	}

	var escapedCreator = $("<div>").text(messageData.CreatorDisplayName).html();
    $('#chat-body').append('<div class="answer ' + direction + '"> <div class="avatar"> <img src="'+ userAvatar +'" alt="User name">  </div>' +
                        '<div class="name">' + escapedCreator + '</div>  <div class="text' + hasLeftChatClass + '">' + displayMessage + '</div> <div class="time">' + moment(messageData.DateCreated).format('MM/DD/YYYY hh:mm A') + '</div></div>');

    $('#typing').remove();

    //If currenty in Screenshare session then attempt to catch the attention when a new message is received by the other side of the chat
    if (direction == 'left' && (!_isChatWindowActive || _isChatWindowPotentiallyHidden)) {
        if (screenSharingPublisher !== undefined) {
            BlinkWindowTitle();
            NewChatMessageAlert();
        }
	}
}

var presenceChannel;
function setupChat(chatID, participantID, pusherKey, callback) {
    var channelName = 'presence-' + chatID;
    var pusher = new Pusher(pusherKey, {
        authEndpoint: service + 'Auth',
        auth: {
            params: {
                chatID: chatID,
                participantID: participantID
            }
        }
    });
    presenceChannel = pusher.subscribe(channelName);

    //presenceChannel.bind('pusher:subscription_succeeded', function () {
    //    //console.log(channel.members);
    //});

    presenceChannel.bind('pusher:member_added', function (member) {
        $('#operator-message').remove();
        createMessage(member.info.name + ' joined the chat.', member.id);
    });

    presenceChannel.bind('pusher:member_removed', function (member) {
        if (presenceChannel.members.count == 1) {
            $('#send-message').addClass("disabled");
            $('#message').prop("disabled", true);
        }
    });

    //presenceChannel.bind('pusher:subscription_error', function (status) {
    //    console.log(status);
    //});

    presenceChannel.bind('new-comment', function (data) {
        $('#typing').remove();
        createMessageElement(data, (data.CreatorType == 0) ? 'left' : 'right');
        $(".panel-body").animate({ scrollTop: $('.panel-body').prop("scrollHeight") }, 1000);
        $('#send-message').prop("disabled", false);
        isSubmittingAlready = false;
    });

    presenceChannel.bind('client-tok-screen', function (data) {
        $('#chat-body').append('<div id="screenRequest" class="answer left"> <div class="avatar"> <img src="../vcr/1_9_0/images/blank_avatar.png" alt="User name">  </div>' +
                    '<div class="name">' + data.userName + '</div>  <div class="text">' + data.userName + ' wants to share a screen with you. <a onClick="subscribeToScreenStream()">Do you Accept?</a><label class="tokWarning">(If you accept you will be seeing the agent\'s screen)</label></div> <div class="time">' + moment().format('MM/DD/YYYY hh:mm A') + '</div></div>');

        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
        _agentName = data.userName;
    });

    presenceChannel.bind('client-tok-video', function (data) {
        $('#chat-body').append('<div id="videoRequest" class="answer left"> <div class="avatar"> <img src="../vcr/1_9_0/images/blank_avatar.png" alt="User name">  </div>' +
                    '<div class="name">' + data.userName + '</div>  <div class="text">' + data.userName + ' wants to share video with you. <a onClick="subscribeToVideoStream()">Do you Accept?</a><label class="tokWarning">(If you accept the agent will see you via your camera)</label></div> <div class="time">' + moment().format('MM/DD/YYYY hh:mm A') + '</div></div>');

        $(".panel-body").animate({ scrollTop: $('.panel-body').prop("scrollHeight") }, 1000);
        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
        _agentName = data.userName;
    });

    presenceChannel.bind('client-tok-audio', function (data) {
        $('#chat-body').append('<div id="audioRequest" class="answer left"> <div class="avatar"> <img src="../vcr/1_9_0/images/blank_avatar.png" alt="User name">  </div>' +
                    '<div class="name">' + data.userName + '</div>  <div class="text">' + data.userName + ' wants to have an audio call with you. <a onClick="subscribeToAudioStream(false)">Do you Accept?</a><label class="tokWarning">(If you accept the agent will hear what you say in your microphone)</label></div> <div class="time">' + moment().format('MM/DD/YYYY hh:mm A') + '</div></div>');

        $(".panel-body").animate({ scrollTop: $('.panel-body').prop("scrollHeight") }, 1000);
        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
        _agentName = data.userName;
    });

    presenceChannel.bind('client-tok-audio-accept', function (data) {
        $('#tokStatusText').text(data.userName + ' has joined live session.');
        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;

        var isIE = /*@cc_on!@*/false || !!document.documentMode;
        var isEdge = !isIE && !!window.StyleMedia;

        if (isIE || isEdge) {
            var tokenURI = encodeURIComponent(sharedToken);
            tokpopup = window.open(siteUrl + '/screenshare/TOKSharedSession.html?sessionid=' + sharedSessionID + '&token=' + tokenURI, 'TSTOKSession', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=250,height=100');

            setTimeout(function () {
                if (!tokpopup || tokpopup.outerHeight === 0) {
                    //First Checking Condition Works For IE & Firefox
                    //Second Checking Condition Works For Chrome
                    alert("Popup Blocker is enabled! Please add this site to your exception list.");
                } else {
    
                }
            }, 500);
        } else {
            ReceiveAudioStream(sharedSessionID, sharedToken);
        }
    });

    presenceChannel.bind('client-tok-video-accept', function (data) {
        $('#tokStatusText').text(data.userName + ' has joined live session.');
        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
        var tokenURI = encodeURIComponent(sharedToken);
        tokpopup = window.open(siteUrl + '/screenshare/TOKSharedSession.html?sessionid=' + sharedSessionID + '&token=' + tokenURI, 'TSTOKSession', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=1250,height=1000');

        setTimeout(function () {
            if (!tokpopup || tokpopup.outerHeight === 0) {
                //First Checking Condition Works For IE & Firefox
                //Second Checking Condition Works For Chrome
                alert("Popup Blocker is enabled! Please add this site to your exception list.");
            } else {

            }
        }, 25);
    });

    presenceChannel.bind('client-agent-tok-ended', function (data) {
        if (data !== undefined) {
            $('#' + data.streamType + 'Request').remove();
        }
    });

    presenceChannel.bind('client-agent-stop-typing', function (data) {
        $('#typing').remove();
    });

    presenceChannel.bind('client-agent-typing', function (data) {
        $('#chat-body').append('<div id="typing" class="answer left"> <div class="avatar"><img src="../vcr/1_9_0/images/blank_avatar.png" alt="User name"></div>' +
                    '<div class="name">' + data.userName + '</div>  <div class="text">' + data.userName + ' is typing...</div> <div class="time">' + moment().format('MM/DD/YYYY hh:mm A') + '</div></div>');
        $(".panel-body").animate({ scrollTop: $('.panel-body').prop("scrollHeight") }, 1000);
    });

    var doneTypingInterval = 5000;  //time in ms, 5 second for example
    var $input = $('#message');

    //on keyup, start the countdown
    $input.on('keyup', function (e) {
        clearTimeout(_typingTimer);

        if (e.which != 13) {
            _typingTimer = setTimeout(doneTyping, doneTypingInterval);
        }
    });

    //on keydown, clear the countdown 
    $input.on('keydown', function (e) {
        if (!isTyping) {
            isTyping = true;
            clearTimeout(_typingTimer);
            var triggered = presenceChannel.trigger('client-user-typing', { userName: presenceChannel.members.me.info.name, chatID: _activeChatID });
        }

        if (e.which == 13) {
            var isIE = /*@cc_on!@*/false || !!document.documentMode;
            var isEdge = !isIE && !!window.StyleMedia;

            if (!isIE && !isEdge) {
                $("#message-form").submit();
            }
        } else {
            //nothing here for now
        }
    });

    callback(presenceChannel);
}

function doneTyping() {
    var triggered = presenceChannel.trigger('client-user-stop-typing', presenceChannel.members.me.info.name + ' is typing...');
    isTyping = false;
}

function loadInitialMessages(chatID, participantID) {
    var chatObject = { chatID: chatID };

    IssueAjaxRequest("GetChatInfo", chatObject,
    function (result) {
        chatInfoObject = result;
        SetupChatUploads(chatID, participantID);
        var messageData = { CreatorID: chatInfoObject.InitiatorUserID, CreatorInitials: chatInfoObject.InitiatorInitials, CreatorDisplayName: chatInfoObject.InitiatorDisplayName, Message: chatInfoObject.Description, DateCreated: chatInfoObject.DateCreated };
        createMessageElement(messageData, 'right');
    },
    function (error) {

    });
}

var service = '../Services/ChatService.asmx/';
function IssueAjaxRequest(method, data, successCallback, errorCallback) {
    $.ajax({
        type: "POST",
        url: service + method,
        data: (data != null ? JSON.stringify(data) : null),
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

function SetupChatUploads(chatID, participantID) {
    var uploadPath = '../../../ChatUpload/ChatAttachments/' + chatInfoObject.OrganizationID + '/';
    $('#hiddenAttachmentInput').fileupload({
        dropZone: $('.panel-default'),
        add: function (e, data) {
            data.url = uploadPath + chatID;
            var jqXHR = data.submit()
                .success(function (result, textStatus, jqXHR) {
                    var attachment = JSON.parse(result)[0];

                    var messageData = { channelName: 'presence-' + chatID, chatID: chatID, attachmentID: attachment.id, userID: participantID };

                    IssueAjaxRequest("AddUserUploadMessage", messageData,
                    function (result) {

                    },
                    function (error) {

                    });
                })
                .error(function (jqXHR, textStatus, errorThrown) { console.log(textStatus) })
        },
    });

    $('#chat-attachment').click(function (e) {
        e.preventDefault();
        $('#hiddenAttachmentInput').click();
    });
}

function DisplayTOKButtons(display) {
    var audio = $('.dropdown-menu li:contains(Audio)');
    var video = $('.dropdown-menu li:contains(Video)');
	var screen = $('.dropdown-menu li:contains(Screen)');
	var audioIcon = $('#chat-tok-audio-Icon');
	var videoIcon = $('#chat-tok-video-Icon');
	var screenIcon = $('#chat-tok-screen-Icon');

    if (display) {
        audio.show();
        video.show();
		screen.show();
		audioIcon.show();
		videoIcon.show();
		screenIcon.show();
    } else {
        audio.hide();
        video.hide();
		screen.hide();
		audioIcon.hide();
		videoIcon.hide();
		screenIcon.hide();
    }
}

function NewChatMessageAlert() {
    // Let's check if the browser supports notifications
    if (!("Notification" in window)) {
        $("#jquery_jplayer_1").jPlayer("setMedia", {
            mp3: "../vcr/1_9_0/Audio/chime.mp3"
        }).jPlayer("play", 0);
    }
    // Let's check whether notification permissions have already been granted
    else if (Notification.permission === "granted") {
        ShowNotificationMessage();
    }
     // Otherwise, we need to ask the user for permission
    else if (Notification.permission !== 'denied') {
        Notification.requestPermission(function (permission) {
            ShowNotificationMessage();
        });
    }
}

function ShowNotificationMessage() {
    var options = {
        body: "New Chat Message!",
        icon: "https://app.teamsupport.com/images/icons/TeamSupportLogo16.png",
        tag: "chat" + _activeChatID
    }
    var notification = new Notification("TeamSupport", options);
    notification.onshow = function () { setTimeout(function () { notification.close(); }, 5000) };
}

$(document).bind('dragover', function (e) {
    var dropZone = $('.panel-default'),
        timeout = window.dropZoneTimeout;
    if (!timeout) {
        dropZone.addClass('in');
    } else {
        clearTimeout(timeout);
    }
    var found = false,
        node = e.target;
    do {
        if (node === dropZone[0]) {
            found = true;
            break;
        }
        node = node.parentNode;
    } while (node != null);
    if (found) {
        dropZone.addClass('hover');
    } else {
        dropZone.removeClass('hover');
    }
    window.dropZoneTimeout = setTimeout(function () {
        window.dropZoneTimeout = null;
        dropZone.removeClass('in hover');
    }, 100);
});

//This function checks the window visibility (if it is minimized or not)
(function () {
    var hidden = "hidden";

    // Standards:
    if (hidden in document)
        document.addEventListener("visibilitychange", onchange);
    else if ((hidden = "mozHidden") in document)
        document.addEventListener("mozvisibilitychange", onchange);
    else if ((hidden = "webkitHidden") in document)
        document.addEventListener("webkitvisibilitychange", onchange);
    else if ((hidden = "msHidden") in document)
        document.addEventListener("msvisibilitychange", onchange);
        // IE 9 and lower:
    else if ("onfocusin" in document)
        document.onfocusin = document.onfocusout = onchange;
        // All others:
    else
        window.onpageshow = window.onpagehide
        = window.onfocus = window.onblur = onchange;

    function onchange(evt) {
        var v = "visible", h = "hidden",
            evtMap = {
                focus: v, focusin: v, pageshow: v, blur: h, focusout: h, pagehide: h
            };

        evt = evt || window.event;
        if (evt.type in evtMap) {
            document.body.className = evtMap[evt.type];
        } else {
            document.body.className = this[hidden] ? "hidden" : "visible";
        }

        _isChatWindowActive = document.body.className == "visible";
    }

    // set the initial state (but only if browser supports the Page Visibility API)
    if (document[hidden] !== undefined)
        onchange({ type: document[hidden] ? "blur" : "focus" });
})();

//These will check if the window has focus or not (minimized or not)
$(window).focus(function () {
    _isChatWindowActive = true;
}).blur(function () {
    _isChatWindowActive = false;
});

function addEvent(obj, evType, fn, isCapturing) {
    if (isCapturing == null) isCapturing = false;
    if (obj.addEventListener) {
        // Firefox
        obj.addEventListener(evType, fn, isCapturing);
        return true;
    } else if (obj.attachEvent) {
        // MSIE
        var r = obj.attachEvent('on' + evType, fn);
        return r;
    } else {
        return false;
    }
}

var potentialPageVisibility = {
    pageVisibilityChangeThreshold: 3 * 3600, // in seconds
    init: function () {
        function setAsNotHidden() {
            var dispatchEventRequired = document.potentialHidden;
            document.potentialHidden = false;
            document.potentiallyHiddenSince = 0;
            if (dispatchEventRequired) dispatchPageVisibilityChangeEvent();
        }

        function initPotentiallyHiddenDetection() {
            if (!hasFocusLocal) {
                // the window does not has the focus => check for  user activity in the window
                lastActionDate = new Date();
                if (timeoutHandler != null) {
                    clearTimeout(timeoutHandler);
                }
                timeoutHandler = setTimeout(checkPageVisibility, potentialPageVisibility.pageVisibilityChangeThreshold * 1000 + 100); // +100 ms to avoid rounding issues under Firefox
            }
        }

        function dispatchPageVisibilityChangeEvent() {
            unifiedVisilityChangeEventDispatchAllowed = false;
            var evt = document.createEvent("Event");
            evt.initEvent("potentialvisilitychange", true, true);
            document.dispatchEvent(evt);
        }

        function checkPageVisibility() {
            var potentialHiddenDuration = (hasFocusLocal || lastActionDate == null ? 0 : Math.floor((new Date().getTime() - lastActionDate.getTime()) / 1000));
            document.potentiallyHiddenSince = potentialHiddenDuration;
            if (potentialHiddenDuration >= potentialPageVisibility.pageVisibilityChangeThreshold && !document.potentialHidden) {
                // page visibility change threshold raiched => raise the even
                document.potentialHidden = true;
                dispatchPageVisibilityChangeEvent();
            }

            _isChatWindowPotentiallyHidden = document.potentialHidden;
        }

        var lastActionDate = null;
        var hasFocusLocal = true;
        var hasMouseOver = true;
        document.potentialHidden = false;
        document.potentiallyHiddenSince = 0;
        var timeoutHandler = null;

        addEvent(document, "pageshow", function (event) {
            document.getElementById("x").innerHTML += "pageshow/doc:<br>";
        });
        addEvent(document, "pagehide", function (event) {
            document.getElementById("x").innerHTML += "pagehide/doc:<br>";
        });
        addEvent(window, "pageshow", function (event) {
            document.getElementById("x").innerHTML += "pageshow/win:<br>"; // raised when the page first shows
        });
        addEvent(window, "pagehide", function (event) {
            document.getElementById("x").innerHTML += "pagehide/win:<br>"; // not raised
        });
        addEvent(document, "mousemove", function (event) {
            lastActionDate = new Date();
        });
        addEvent(document, "mouseover", function (event) {
            hasMouseOver = true;
            setAsNotHidden();
        });
        addEvent(document, "mouseout", function (event) {
            hasMouseOver = false;
            initPotentiallyHiddenDetection();
        });
        addEvent(window, "blur", function (event) {
            hasFocusLocal = false;
            initPotentiallyHiddenDetection();
        });
        addEvent(window, "focus", function (event) {
            hasFocusLocal = true;
            setAsNotHidden();
        });
        setAsNotHidden();
    }
}

potentialPageVisibility.pageVisibilityChangeThreshold = 1;
potentialPageVisibility.init();

BlinkWindowTitle = (function () {
    var oldTitle = document.title;
    var msg = "New Message!";
    var timeoutId;
    var blink = function () { document.title = document.title == msg ? ' ' : msg; };
    var clear = function () {
        clearInterval(timeoutId);
        document.title = oldTitle;
        window.onmousemove = null;
        timeoutId = null;
    };
    return function () {
        if (!timeoutId) {
            timeoutId = setInterval(blink, 1000);
            window.onmousemove = clear;
        }
    };
}());