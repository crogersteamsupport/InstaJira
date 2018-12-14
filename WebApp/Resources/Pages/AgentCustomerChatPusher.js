var isTyping = false;
var channel;
var customerName;
var siteUrl;
var _typingTimer;
var pusherRequestChannel_AgentPage = null;
var pusher = null;

function setupChat(pusherKey, chatID, newCommentCallback, callback) {
    var windowUrl = window.location.href;
    var arr = windowUrl.split("/");
    siteUrl = arr[0] + "//" + arr[2];

    var channelName = 'presence-' + chatID;
	var service = '/Services/ChatService.asmx/';

	if (pusher == null) {
		pusher = new Pusher(pusherKey, {
			authEndpoint: service + 'AgentAuth'
		});
	}

    channel = pusher.subscribe(channelName);

    channel.bind('pusher:member_removed', function (member) {
        var chatId = member.info.chatId;
        parent.Ts.Services.Chat.RemoveUser('presence-' + chatId, chatId, member.id, function (success) { });
    });

    channel.bind('pusher:subscription_error', function (status) {
        console.log(status);
    });

    channel.bind('new-comment', function (data) {
        newCommentCallback(data, true, false);
        isSubmittingAlready = false;

        if (data.HasLeft && data.CreatorID == top.Ts.System.User.UserID) {
            pusher.unsubscribe(channelName);
        }
    });

    var typeTemplate;
    channel.bind('client-user-typing', function (data) {
        if (data.chatID == _activeChatID) {
            var messageTemplate = $("#typing-template").html();
            typeTemplate = messageTemplate
                                    .replace('{{MessageDirection}}', 'left')
                                    .replace('{{UserName}}', data.userName)
                                    .replace('{{Avatar}}', '../images/blank_avatar.png')
                                    .replace('{{message}}', data.userName + ' is typing...')
                                    .replace('{{Date}}', moment().format(dateFormat + ' hh:mm A'));

            $('.media-list').append(typeTemplate);
            ScrollMessages(true);
        }
    });

    channel.bind('client-user-stop-typing', function (data) {
        $('#typing').remove();
    });

    channel.bind('client-tok-enabled', function (data) {
        EnableTOKButtons(data.isCustomerTOKEnabled && isTOKEnabledForBrowser);
    });

    channel.bind('client-tok-screen-user', function (data) {
        var messageTemplate = $("#tok-screen-template").html();
        var compiledTemplate = messageTemplate
                                .replace('{{message}}', data.userName + ' wants to share their screen with you. ')
                                .replace('{{Date}}', moment().format(dateFormat + ' hh:mm A'))
                                .replace('{{UserName}}', data.userName);
        $('.media-list').append(compiledTemplate);
        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
        customerName = data.userName;
        ScrollMessages(true);
    });

    channel.bind('client-tok-video-user', function (data) {
        var messageTemplate = $("#tok-video-template").html();
        var compiledTemplate = messageTemplate
                                .replace('{{message}}', data.userName + ' wants to have a video call with you.  ')
                                .replace('{{Date}}', moment().format(dateFormat + ' hh:mm A'))
                                .replace('{{UserName}}', data.userName);
        $('.media-list').append(compiledTemplate);

        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
        customerName = data.userName;
        ScrollMessages(true);
    });

    channel.bind('client-tok-audio-user', function (data) {
        var messageTemplate = $("#tok-audio-template").html();
        var compiledTemplate = messageTemplate
                                .replace('{{message}}', data.userName + ' wants to have an audio call with you. ')
                                .replace('{{Date}}', moment().format(dateFormat + ' hh:mm A'))
                                .replace('{{UserName}}', data.userName);
        $('.media-list').append(compiledTemplate);

        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
        customerName = data.userName;
        ScrollMessages(true);
    });

    channel.bind('client-tok-audio-user-accept', function (data) {
        if (data.agentRequesting == channel.members.me.info.name) {
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
        }
        
    });

    channel.bind('client-tok-video-user-accept', function (data) {
        if (data.agentRequesting == channel.members.me.info.name) {
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
        }
    });

    channel.bind('client-tok-ended', function (data) {
        if (data !== undefined) {
            $('#' + data.streamType + 'Request').remove();
        }
    });

    //Used for the accepted invitations to the current chat.
    channel.bind('pusher:member_added', function (member) {
        if (member !== null && member.info !== null && member.info.isAgent !== null && member.info.isAgent) {
            newCommentCallback(member, true, true);
        }
    });

    var doneTypingInterval = 5000;
    var $input = $('#message');

    //on keyup, start the countdown
    $input.on('keyup', function (e) {
        clearTimeout(_typingTimer);

        if (e.which != 13) {
            _typingTimer = setTimeout(doneTyping, doneTypingInterval);
        } else {
            isTyping = false;
        }
    });

    //on keydown, clear the countdown 
    $input.on('keydown', function (e) {
        if (!isTyping) {
            isTyping = true;
            clearTimeout(_typingTimer);

            if (channel !== null && e.which != 13)
                var triggered = channel.trigger('client-agent-typing', { userName: channel.members.me.info.name });
        }
    });

    callback(channel);
}

function ScrollMessages(animated) {
    if (animated)
        $(".current-chat-area").animate({ scrollTop: $('.current-chat-area').prop("scrollHeight") }, 1000);
    else
        $(".current-chat-area").scrollTop($('.current-chat-area').prop("scrollHeight"));
}

function doneTyping() {
    if (channel !== null)
        var triggered = channel.trigger('client-agent-stop-typing', channel.members.me.info.name + ' is typing...');
    isTyping = false;
}

function subscribeToNewChatRequest(pusherKey, newRequestCallback) {
	var chatGUID = top.Ts.System.Organization.ChatID;

	if (parent.pusherChatRequests == null || parent.pusherChatRequests.connection.state == 'disconnected') {
		pusherRequestChannel_AgentPage = new Pusher(pusherKey);
	} else {
		pusherRequestChannel_AgentPage = parent.pusherChatRequests;
	}

    var request_channel = pusherRequestChannel_AgentPage.subscribe('chat-requests-' + chatGUID);

    request_channel.bind('new-chat-request', function (data) {
        try {
            newRequestCallback(data);
        } catch (error) {
            console.log(error.message);
        }
    });

    request_channel.bind('chat-request-accepted', function (data) {
        try {
            $('#chats-requests > #' + data).remove();
        } catch (error) {
            console.log(error.message);
        }
    });

    request_channel.bind('chat-request-abandoned', function (data) {
        try {
            $('#chats-requests > #' + data).remove();
        } catch (error) {
            console.log(error.message);
        }
    });
}