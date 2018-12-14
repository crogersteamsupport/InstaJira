var _activeChatID = null;
var dateFormat;
var _intervalUpdateActiveChats = null;
var isTOKEnabledForBrowser;
var _isChatWindowActive = true;
var _isChatWindowPotentiallyHidden = false;
var _eventsList;
var _groupId;
var isSubmittingAlready = false;

$(document).ready(function () {
    var chatInfoObject = {};
    var pusherKey = null;
    var contactID = null;
    var uploadPath = '../../../ChatUpload/ChatAttachments/' + top.Ts.System.User.OrganizationID + '/';
    var isSafari = Object.prototype.toString.call(window.HTMLElement).indexOf('Constructor') > 0 || (function (p) { return p.toString() === "[object SafariRemoteNotification]"; })(!window['safari'] || safari.pushNotification);
    var isIE = /*@cc_on!@*/false || !!document.documentMode;
    var isEdge = !isIE && !!window.StyleMedia;
    isTOKEnabledForBrowser = !isSafari && !isEdge;

    $("#jquery_jplayer_1").jPlayer({
        ready: function () {
            $(this).jPlayer("setMedia", {
                mp3: "../Audio/chime.mp3"
            });
        },
        loop: false,
        swfPath: "vcr/1_9_0/Js"
    });

    window.parent.Ts.Services.Customers.GetDateFormat(false, function (format) {
        dateFormat = format.replace("yyyy", "yy");
        if (dateFormat.length < 8) {
            var dateArr = dateFormat.split('/');
            if (dateArr[0].length < 2) {
                dateArr[0] = dateArr[0] + dateArr[0];
            }
            if (dateArr[1].length < 2) {
                dateArr[1] = dateArr[1] + dateArr[1];
            }
            if (dateArr[2].length < 2) {
                dateArr[1] = dateArr[1] + dateArr[1];
            }
            dateFormat = dateArr[0] + "/" + dateArr[1] + "/" + dateArr[2];
        }
    });

    window.LoadPusherAndSubscribe = function () {
        top.Ts.Settings.System.read('PusherKey', '1', function (key) {
            if (top.Ts.System.ChatUserSettings.IsAvailable) {
                pusherKey = key;
                SetupChatRequests();
                subscribeToNewChatRequest(pusherKey, function (request) {
					var isGroupValidated = parent.Ts.MainPage.ValidateChatForGroup(request.chatRequest.GroupID);

					if ((request.userIdInvited === undefined || request.userIdInvited == top.Ts.System.User.UserID) && isGroupValidated) {
                        SetupPendingRequest(request.chatRequest, true);
                    }
                });
            } else {
				var chatGUID = top.Ts.System.Organization.ChatID;

				if (pusherRequestChannel_AgentPage != null) {
					pusherRequestChannel_AgentPage.disconnect();
				}
            }

            $('.page-loading').hide().next().show();
        });
    }

    if (top.Ts.System.ChatUserSettings.IsAvailable) {
        LoadPusherAndSubscribe();
    } else {
        $('.page-loading').hide().next().show();
    }

    GetChatSettings(true);
    SetupToolbar();
	SetupTOK();

	parent.Ts.Services.Chat.GetHTMLGlobalEventAttributes(function (data) {
		_eventsList = data;
	});

    function GetChatSettings(isInit) {
        if (isInit) {
            _activeChatID = null;
            EnableTOKButtons(false, false, false, isInit);
            EnableDisableTicketMenu();
            $('#chat-invite').addClass("disabled");
            $('#chat-leave').addClass("disabled");
            $('#chat-customer').addClass("disabled");
            $('#chat-customer').show();
            $('#chat-suggestions').addClass("disabled");
            $('#chat-attachment').addClass("disabled");
        } else {
            $('#chat-suggestions').removeClass("disabled");
            $('#chat-attachment').removeClass("disabled");
            var enableAudio = true;
            var enableVideo = true;
            var enableScreen = true;

            if (!isTOKEnabledForBrowser) {
                enableAudio = false;
                enableVideo = false;
                enableScreen = false;
                EnableTOKButtons(enableAudio, enableVideo, enableScreen, isInit);
            } else {
                parent.Ts.Services.Chat.GetAgentChatProperties(function (data) {
                    if (!data.TOKScreenEnabled)
                        enableScreen = false;
                    if (!data.TOKVideoEnabled)
                        enableVideo = false;
                    if (!data.TOKVoiceEnabled)
                        enableAudio = false;

                    EnableTOKButtons(enableAudio, enableVideo, enableScreen, isInit);
                });
            }
        }
    }

    function SetupChatRequests() {
        parent.Ts.Services.Chat.GetChatRequests(function (data) {
            for (i = 0; i < data.length; i++) {
                SetupPendingRequest(data[i], (i == 0));
            }

        });

        parent.Ts.Services.Chat.GetActiveChats(function (data) {
            for (a = 0; a < data.length; a++) {
                SetupActiveRequest(data[a], (a == 0));
            }
        });
    }

    function SetupPendingRequest(chat, shouldTrigger) {
        var innerString = chat.InitiatorDisplayName;
        if (typeof chat.CompanyName !== "undefined" && chat.CompanyName) {
            innerString = innerString + ' - ' + chat.CompanyName;
        }
        var anchor = $('<a id="' + chat.ChatRequestID + '" href="#" class="list-group-item chat-request">' + innerString + '</a>').click(function (e) {
            e.preventDefault();

            CloseRequestAnchor();
            $(this).addClass('list-group-item-info');

            var acceptBtn = $('<button class="btn btn-default">Accept</button>').click(function (e) {
                var parentEl = $(this).parent();
				_groupId = chat.GroupID;
                AcceptRequest(chat.ChatRequestID, innerString, parentEl);
            });

			var displayMessage = $("<div>").text(chat.Description).html();

			if (chat.Description.trim().indexOf("<img ") == 0 && chat.Description.trim().indexOf("<script ") < 0 && chat.Description.trim().indexOf(" onload=") < 0) {
				displayMessage = chat.Description;
			}

			if ((chat.Description.trim().indexOf("/chatattachments/") > 0 && chat.Description.trim().indexOf("<script ") < 0 && chat.Description.trim().indexOf(" onload=") < 0)) {
				displayMessage = chat.Description;
			}

            $(this).html('<p class="userName">' + innerString + '</p>' +
                             '<p>Email:  ' + chat.InitiatorEmail + '</p>' +
                             '<p>Time:  ' + moment(chat.DateCreated).format(dateFormat + ' hh:mm A') + '</p>' +
				'<p>Message:  ' + displayMessage + '</p>')
                             .append(acceptBtn)
                             .addClass('open-request');
        });

        $('#chats-requests').append(anchor);
        if (shouldTrigger) anchor.trigger("click");

    }

    function SetupActiveRequest(chat, shouldTrigger) {
        var initiator = chat.InitiatorDisplayName;
        var activeChatIndicator = '<span class="fa fa-comments fa-1 activeChatIndicator"></span>';

        if (typeof chat.CompanyName !== "undefined" && chat.CompanyName) {
            initiator = initiator + ' - ' + chat.CompanyName;
        }

        if (!shouldTrigger) {
            activeChatIndicator = '';
        }

        var anchor = $('<a id="active-chat_' + chat.ChatID + '" href="#" class="list-group-item">' + initiator + activeChatIndicator + '</a>').click(function (e) {
            e.preventDefault();

            $('.list-group-item-success').removeClass('list-group-item-success');
            $('.fa-comments').remove();
            $(this).addClass('list-group-item-success')
                    .removeClass('list-group-item-info')
                    .append('<span class="fa fa-comments fa-1 activeChatIndicator"></span>');

			      _activeChatID = chat.ChatID;
            SetActiveChat(_activeChatID);
        });

        $('#chats-accepted').append(anchor);

        if (shouldTrigger) {
            anchor.trigger("click");
        }

        setupChat(pusherKey, chat.ChatID, createMessageElement, function (channel) {
            //console.log(channel);
        });

    }

    function doneTyping() {
        if (channel !== null)
            var triggered = channel.trigger('client-agent-stop-typing', channel.members.me.info.name + ' is typing...');
        isTyping = false;
    }

    function CloseRequestAnchor() {
        $('.open-request').html($('.open-request > .userName').text()).removeClass('open-request').removeClass('list-group-item-info');
    }

    function AcceptRequest(ChatRequestID, innerString, parentEl) {
        parent.Ts.Services.Chat.AcceptRequest(ChatRequestID, isTOKEnabledForBrowser, function (chatId) {
            // check chatId to verify the chat has not been accepted already
            if (chatId > 0) {
                setupChat(pusherKey, chatId, createMessageElement, function (channel) {
                    //console.log(channel);
                });

                parentEl.remove();
                MoveAcceptedRequest(innerString, chatId);

                _activeChatID = chatId;
                SetActiveChat(_activeChatID);
            } else {
                alert("The chat has been accepted already by another agent.");
                parentEl.remove();
            }

        });
    }

    function MoveAcceptedRequest(innerString, chatID) {
        $('.list-group-item-success').removeClass('list-group-item-success');
        $('.fa-comments').remove();
        var anchor = $('<a id="active-chat_' + chatID + '" href="#" class="list-group-item list-group-item-success">' + innerString + '<span class="fa fa-comments fa-1 activeChatIndicator"></span></a>').click(function (e) {
            e.preventDefault();

            $('.list-group-item-success').removeClass('list-group-item-success');
            $('.fa-comments').remove();
            $(this).addClass('list-group-item-success')
                    .removeClass('list-group-item-info')
                    .append('<span class="fa fa-comments fa-1 activeChatIndicator"></span>');

            _activeChatID = chatID;
            SetActiveChat(_activeChatID);
        });

        $('#chats-accepted').append(anchor);
    }

    function createMessageElement(messageData, scrollView, isAgentAcceptedInvitation) {
        if (messageData.ChatID == _activeChatID && !isAgentAcceptedInvitation) {
            var chatUserLeft = ((messageData.CreatorID !== null) ? messageData.CreatorID.toString() : 'customer');

            if ((messageData.HasLeft && !$("#" + chatUserLeft + 'HasLeft').length > 0)
				|| (!messageData.HasLeft)) {
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

                var messageTemplate = $("#message-template").html();
                var compiledTemplate = messageTemplate
                                        .replace('{{MessageDirection}}', 'left')
                                        .replace('{{UserName}}', messageData.CreatorDisplayName)
                                        .replace('{{Avatar}}', (messageData.CreatorID !== null)
                                                                        ? '../../../dc/' + chatInfoObject.OrganizationID + '/UserAvatar/' + messageData.CreatorID + '/48/1470773158079'
                                                                        : '../images/blank_avatar.png')
                                        .replace('{{Message}}', displayMessage)
                                        .replace('{{Date}}', moment(messageData.DateCreated).format(dateFormat + ' hh:mm A'));

                if (messageData.HasLeft) {
                    compiledTemplate = compiledTemplate.replace('{{HasLeftChat}}', 'id=' + chatUserLeft + 'HasLeft');
                }

                $('.media-list').append(compiledTemplate);
                if (scrollView) ScrollMessages(true);

                //If message is coming from the customer and we are in screenshare
                if (screenSharingPublisher !== undefined && messageData.CreatorType == 1 && (!_isChatWindowActive || _isChatWindowPotentiallyHidden)) {
                    BlinkWindowTitle();
                    NewChatMessageAlert();
                } else if (messageData.CreatorType == 1) {
                    CustomerMessageSound(false);
                }
            }
        }
		else if (isAgentAcceptedInvitation && messageData != null && messageData.info != null && messageData.info.chatId != null && messageData.info.chatId == _activeChatID) {
			var displayMessage = messageData.info.name;

			if (messageData.Message.trim().indexOf("<img ") != 0) {
				displayMessage = $("<div>").text(messageData.info.name).html();
			}

            var messageTemplate = $("#message-template").html();
            var dateTimeString = new Date().toLocaleString();
            dateTimeString = dateTimeString.replace(",", "");
            var compiledTemplate = messageTemplate
                                    .replace('{{MessageDirection}}', 'left')
                                    .replace('{{UserName}}', messageData.info.name)
                                    .replace('{{Avatar}}', (messageData.id !== null)
                                                                    ? '../../../dc/' + chatInfoObject.OrganizationID + '/UserAvatar/' + messageData.id + '/48/1470773158079'
                                                                    : '../images/blank_avatar.png')
                                    .replace('{{Message}}', displayMessage + " has joined the chat.")
                                    .replace('{{Date}}', dateTimeString);

            $('.media-list').append(compiledTemplate);
            if (scrollView) ScrollMessages(true);
        } else {
            //No active chat, but one of the accepted chats
            $('#active-chat_' + messageData.ChatID).addClass('list-group-item-info');

            if ($('#active-chat_' + messageData.ChatID).length > 0) {
                CustomerMessageSound(true);
            }
        }

        //If we receive a new message (any of the accepted chat requests) but the user is not in the Customer Chat page then highlight this menu item
        if (IsNotInCustomerChatPage()) {
            parent.Ts.MainPage.MainMenu.find('mniChat', 'chat').setIsHighlighted(true);
        }
    }

    function SetActiveChat(chatID) {
        parent.Ts.Services.Chat.GetChatDetails(chatID, function (chat) {
            chatInfoObject = chat;
            if (chat.InitiatorUserID !== null) {
                contactID = chat.InitiatorUserID;
                $('#chat-customer').show();
            }
            else {
                contactID = null;
                $('#chat-customer').hide();
            }

            $('.media-list').empty();
            $('.chat-intro').empty();
            $('.chat-intro').append('<p>Initiated On: ' + moment(chat.DateCreated).format(dateFormat + ' hh:mm A') + '</p>');
            $('.chat-intro').append('<p>Initiated By: ' + chat.InitiatorMessage + '</p>');

            for (i = 0; i < chat.Messages.length; i++) {
                createMessageElement(chat.Messages[i], false, false);
            }
            ScrollMessages(false);

            if ($('#Ticket-Create').hasClass("disabled")) {
                $('#Ticket-Create').removeClass("disabled");
            }

            if (!$('#Ticket-Open').hasClass("disabled")) {
                $('#Ticket-Open').addClass("disabled");
            }

            _intervalUpdateActiveChats = setInterval('EnableDisableTicketMenu();', 5200);
        });

        GetChatSettings(false);

        if ($('#chat-invite').hasClass("disabled")) {
            $('#chat-invite').removeClass("disabled");
        }

        if ($('#chat-leave').hasClass("disabled")) {
            $('#chat-leave').removeClass("disabled");
        }

        if ($('#chat-customer').hasClass("disabled")) {
            $('#chat-customer').removeClass("disabled");
        }

        if ($('#chat-suggestions').hasClass("disabled")) {
            $('#chat-suggestions').removeClass("disabled");
        }

        if ($('#chat-attachment').hasClass("disabled")) {
            $('#chat-attachment').removeClass("disabled");
        }
    }

    function ScrollMessages(animated) {
        if (animated)
            $(".current-chat-area").animate({ scrollTop: $('.current-chat-area').prop("scrollHeight") }, 1000);
        else
            $(".current-chat-area").scrollTop($('.current-chat-area').prop("scrollHeight"));
    }

    $("#message-form").submit(function (e) {
        e.preventDefault();
        var messageString = $('#message').val();
        messageString = messageString.trim();

        if (messageString !== '' && !isSubmittingAlready) {
            isSubmittingAlready = true;
            messageString = replaceURLs(messageString);
            $('#new-message').prop("disabled", true);
            doneTyping();
            parent.Ts.Services.Chat.AddAgentMessage('presence-' + _activeChatID, messageString, _activeChatID, function (data) {
                $('#message').val('');
                $('#new-message').prop("disabled", false);
            });
        }
    });

    function replaceURLs(text) {
        var urlRegex = /(\b(https?|ftp|file):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/ig;

        return text.replace(urlRegex, function (url) {
            return '<a target="_blank" href="' + url + '">' + url + '</a>';
        })
    }

    function SetupToolbar() {
        //Leave Chat and remove from list of active chats
        $('#chat-leave').click(function (e) {
            e.preventDefault();
            if (confirm('Are you sure you want to leave this chat?')) {
                parent.Ts.Services.Chat.CloseChat('presence-' + _activeChatID, _activeChatID, function (success) {
                    if (success) {
                        $('#active-chat_' + _activeChatID).remove();
                        $('.media-list').empty();
                        $('.chat-intro').empty();
                        _activeChatID = null;
                        GetChatSettings(true);
                        $('#chat-invite').addClass("disabled");
                        $('#chat-leave').addClass("disabled");
                        $('#chat-customer').addClass("disabled");
                    }
                    else console.log('Error closing chat.')
                });
            } else {
                // Do nothing!
            }
        });

        $('#chat-invite').click(function (e) {
            e.preventDefault();
            $('#chat-add-user-modal').modal('show');
        });

        var _execGetCustomer = null;
        var getUsers = function (request, response) {
            if (_execGetCustomer) { _execGetCustomer._executor.abort(); }
            _execGetCustomer = parent.Ts.Services.Chat.GetUsers(request.term, function (result) { response(result); });
        };

        $('.chat-user-list').autocomplete({
            minLength: 2,
            source: getUsers,
            defaultDate: new Date(),
            select: function (event, ui) {
                $(this).data('item', ui.item);
            }
        });

        $('#add-user-save').click(function (e) {
            e.preventDefault();
            var userID = $('#chat-invite-user').data('item').id;
            var userIdInvited = userID;
            parent.Ts.Services.Chat.RequestInvite(_activeChatID, userIdInvited, function (data) {
                $('#chat-add-user-modal').modal('hide');
            });
        });

        $('#chat-transfer').click(function (e) {
            e.preventDefault();
            $('#chat-transfer-user-modal').modal('show');
        });

        $('#transfer-user-save').click(function (e) {
            e.preventDefault();
            var userID = $('#chat-transfer-user').data('item').id;
            parent.Ts.Services.Chat.RequestTransfer(_activeChatID, userID, function (data) {
                $('#chat-transfer-user-modal').modal('hide');
            });
        });

        $('#chat-customer').click(function (e) {
            e.preventDefault();
            parent.Ts.MainPage.openContact(contactID);
        });

        var _suggestedSolutionDefaultInput = '';
        $('#chat-suggestions').click(function (e) {
            e.preventDefault();
            suggestedSolutions(function (ticketID, isArticle) {
                if (isArticle) {
                    top.Ts.Services.Tickets.GetKBTicketAndActions(ticketID, function (result) {
                        if (result === null) {
                            alert('There was an error inserting your suggested solution ticket.');
                            return;
                        }
                        var ticket = result[0];
                        var actions = result[1];

                        var html = '<div>';

                        if (actions.length == 0) {
                            alert('The selected ticket has no knowledgebase actions.');
                        }

                        for (var i = 0; i < actions.length; i++) {
                            html = html + '<div>' + actions[i].Description + '</div></br>';
                        }
                        html = html + '</div>';

                        parent.Ts.Services.Chat.AddAgentMessage('presence-' + _activeChatID, html, _activeChatID, function (data) {
                            ;
                        });

                        top.Ts.System.logAction('Chat - Suggested Solution Inserted');
                    }, function () {
                        alert('There was an error inserting your suggested solution ticket.');
                    });
                }
                else {
                    top.Ts.Services.Admin.GetHubURLwithCName(function (url) {
                        var link = "https://" + url + "/knowledgeBase/" + ticketID;
                        var html = '<a href="' + link + '" target="_blank">' + link + '</a></br>';

                        parent.Ts.Services.Chat.AddAgentMessage('presence-' + _activeChatID, html, _activeChatID, function (data) {

                        });
                        top.Ts.System.logAction('Chat - Suggested Solution Link Inserted');
                    });

                }
            });
        });

        $('#hiddenAttachmentInput').fileupload({
            dropZone: $('.current-chat'),
            add: function (e, data) {
                data.url = uploadPath + _activeChatID;
                var jqXHR = data.submit()
                    .success(function (result, textStatus, jqXHR) {
                        var attachment = JSON.parse(result)[0];
                        parent.Ts.Services.Chat.AddAgentUploadtMessage('presence-' + _activeChatID, _activeChatID, attachment.id);
                    })
                    .error(function (jqXHR, textStatus, errorThrown) { console.log(textStatus) })
            },
        });

        $('#chat-attachment').click(function (e) {
            e.preventDefault();
            $('#hiddenAttachmentInput').click();
        });

        //Create a ticket with the associated chat in it.
        $('#Ticket-Create').click(function (e) {
            e.preventDefault();
			var parameters = '?ChatID=' + _activeChatID;

			if (_groupId !== undefined && _groupId > 0) {
				parameters += '&groupId=' + _groupId;
			}

            parent.Ts.MainPage.newTicket(parameters);
            parent.Ts.System.logAction('Chat - Ticket Created');
        });

        var execGetRelated = null;
        var getTickets = function (request, response) {
            if (execGetRelated) { execGetRelated._executor.abort(); }
            execGetRelated = parent.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) { response(result); });
        }

        $('.chat-tickets-list').autocomplete({
            minLength: 2,
            source: getTickets,
            defaultDate: new Date(),
            select: function (event, ui) {
                $(this).data('item', ui.item);
            }
        });

        $('#Ticket-Add').click(function (e) {
            e.preventDefault();
            $('#chat-add-ticket-modal').modal('show');
        });

        $('#ticket-add-save').click(function (e) {
            e.preventDefault();
            var ticketID = $('#chat-add-ticket').data('item').data;
            var ticketNumber = $('#chat-add-ticket').data('item').id;
            parent.Ts.Services.Chat.AddTicket(_activeChatID, ticketID, function () {
                parent.Ts.MainPage.openTicketByID(ticketID, false);
                $('#chat-add-ticket-modal').modal('hide');
                alert("Chat added to ticket " + ticketNumber);
            });
        });

        //Open the Ticket associated with this chat
        $('#Ticket-Open').click(function (e) {
            e.preventDefault();
            parent.Ts.Services.Chat.GetTicketID(_activeChatID, function (ticketID) {
                if (ticketID) {
                    parent.Ts.MainPage.openTicketByID(ticketID, false);
                }
                else console.log('Error opening associated ticket.')
            });
        });

        $('#chat-invite').addClass("disabled");
        $('#chat-leave').addClass("disabled");
        $('#chat-customer').addClass("disabled");
        $('#chat-customer').show();
    }

    var execSuggestedSolutions = null;
    var execSelectTicket = null;
    var selectTicket = function (request, response) {
        if (execSelectTicket) { execSelectTicket._executor.abort(); }
        var filter = $(this.element).data('filter');
        if (filter === undefined) {
            execSelectTicket = window.parent.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) { response(result); });
        }
        else {
            execSelectTicket = window.parent.Ts.Services.Tickets.SearchTickets(request.term, filter, function (result) { response(result); });
        }
    }

    function suggestedSolutions(callback) {
        $('.dialog-select-ticket2').find('input').val('');
        $('#SuggestedSolutionsModal').modal('show');
        $('#SuggestedSolutionsModal').on('shown.bs.modal', function () {
            $("#dialog-select-ticket2-input").focus();
        })
        if (execSuggestedSolutions) {
            return;
        }
        execSuggestedSolutions = true;
        $('#SuggestedSolutionsIFrame').attr('src', '/vcr/1_9_0/Pages/SuggestedSolutionsChat.html');

        $('.afterSearch').show();

        filter = new top.TeamSupport.Data.TicketLoadFilter();
        filter.IsKnowledgeBase = true;
        $('.dialog-select-ticket2').find('input').data('filter', filter);

        $(".dialog-select-ticket2 input").autocomplete({
            minLength: 2,
            source: selectTicket,
            select: function (event, ui) {
                $(this).data('item', ui.item).removeClass('ui-autocomplete-loading')
                top.Ts.Services.Tickets.GetKBTicketAndActions(ui.item.data, function (result) {
                    var html = '<div>';

                    var actions = result[1];
                    if (actions.length == 0) {
                        html = html + '<h2>The selected ticket has no knowledgebase actions.</h2>';
                    }
                    else {
                        for (var i = 0; i < actions.length; i++) {
                            html = html + '<div>' + actions[i].Description + '</div></br>';
                        }
                    }
                    html = html + '</div>';
                    //clickedItem.find('.previewHtml').attr("value", html);
                    window.frames[0].document.getElementById("TicketPreviewIFrame").contentWindow.writeHtml(html);
                });
            },
            position: {
                my: "right top",
                at: "right bottom",
                collision: "fit flip"
            }
        });

        $('#InsertSuggestedSolutions').click(function (e) {
            e.preventDefault();

            if ($(".dialog-select-ticket2 input").data('item')) {
                callback($(".dialog-select-ticket2 input").data('item').data, true);
                $('#SuggestedSolutionsModal').modal('hide');
                top.Ts.System.logAction('Inserted kb');
            }
            else {
                var id = document.getElementById("SuggestedSolutionsIFrame").contentWindow.GetSelectedID();
                if (id) {
                    callback(id, true);
                    $('#SuggestedSolutionsModal').modal('hide');
                    top.Ts.System.logAction('Inserted suggested solution');
                }
                else {
                    alert('Select a knowledgebase article.');
                }
            }
        });

        $('#InsertSuggestedSolutionsLink').click(function (e) {
            e.preventDefault();

            if ($(".dialog-select-ticket2 input").data('item')) {
                callback($(".dialog-select-ticket2 input").data('item').data, false);
                $('#SuggestedSolutionsModal').modal('hide');
                top.Ts.System.logAction('Inserted kb');
            }
            else {
                var id = document.getElementById("SuggestedSolutionsIFrame").contentWindow.GetSelectedID();
                if (id) {
                    callback(id, false);
                    $('#SuggestedSolutionsModal').modal('hide');
                    top.Ts.System.logAction('Inserted suggested solution');
                }
                else {
                    alert('Select a knowledgebase article.');
                }
            }
        });
    }

    _intervalUpdateActiveChats = setInterval('EnableDisableTicketMenu();', 5200);

    $('#message').keydown(function (e) {
        if (e.which == 13) {
            $("#message-form").submit();
        } else {
            //nothing here for now
        }
    });
});

function EnableDisableTicketMenu() {
    if (_activeChatID !== null) {
        parent.Ts.Services.Chat.GetTicketID(_activeChatID, function (ticketID) {
            if (ticketID && ticketID > 0) {
                if (!$('#Ticket-Create').hasClass("disabled")) {
                    $('#Ticket-Create').addClass("disabled");
                }

                if ($('#Ticket-Open').hasClass("disabled")) {
                    $('#Ticket-Open').removeClass("disabled");
                }

                clearInterval(_intervalUpdateActiveChats);
            }
        });

        if ($('#Ticket-Add').hasClass("disabled")) {
            $('#Ticket-Add').removeClass("disabled");
        }
    } else {
        $('#Ticket-Create').addClass("disabled");
        $('#Ticket-Open').addClass("disabled");
        $('#Ticket-Add').addClass("disabled");
    }
}

function EnableTOKButtons(enableAudio, enableVideo, enableScreen, isChatInit) {
    var featureDisabledTooltip = 'This feature is disabled.  It can be enabled within the Admin section.'
    var audio = $('#chat-tok-audio > .btn-primary');
    var video = $('#chat-tok-video > .btn-primary');
    var screen = $('#chat-tok-screen > .btn-primary');

    if (enableAudio && audio.hasClass('disabled')) {
        audio.removeClass('disabled');
        $('#chat-tok-audio').attr('title', '');
    } else if (!enableAudio && !audio.hasClass('disabled')) {
        audio.addClass('disabled');
        if (!isChatInit) {
            $('#chat-tok-audio').attr('title', featureDisabledTooltip);
        }
    }

    if (enableVideo && video.hasClass('disabled')) {
        video.removeClass('disabled');
        $('#chat-tok-video').attr('title', '');
    } else if (!enableVideo && !video.hasClass('disabled')) {
        video.addClass('disabled');
        if (!isChatInit) {
            $('#chat-tok-video').attr('title', featureDisabledTooltip);
        }
    }

    if (enableScreen && screen.hasClass('disabled')) {
        screen.removeClass('disabled');
        $('#chat-tok-screen').attr('title', '');
    } else if (!enableScreen && !screen.hasClass('disabled')) {
        screen.addClass('disabled');
        if (!isChatInit) {
            $('#chat-tok-screen').attr('title', featureDisabledTooltip);
        }
    }
}

function NewChatMessageAlert() {
    // Let's check if the browser supports notifications
    if (!("Notification" in window)) {
        $("#jquery_jplayer_1").jPlayer("setMedia", {
            mp3: "../Audio/chime.mp3"
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

function CustomerMessageSound(forceIt) {
    var menuID = parent.Ts.MainPage.MainMenu.getSelected().getId().toLowerCase();
    var isMain = parent.Ts.MainPage.MainTabs.find(0, parent.Ts.Ui.Tabs.Tab.Type.Main).getIsSelected();

    if (forceIt || IsNotInCustomerChatPage()) {
        $("#jquery_jplayer_1").jPlayer("setMedia", {
            mp3: "../Audio/chime.mp3"
        }).jPlayer("play", 0);
    }
}

function ShowNotificationMessage() {
    var TeamSupportLogo = "https://app.teamsupport.com/images/icons/TeamSupportLogo16.png";
    var options = {
        body: "New Chat Message!",
        icon: TeamSupportLogo,
        iconUrl: TeamSupportLogo,
        tag: "chat" + _activeChatID
    }
    var notification = new Notification("TeamSupport", options);
    notification.onshow = function () { setTimeout(function () { notification.close(); }, 5000) };
}

function IsNotInCustomerChatPage() {
    var menuID = parent.Ts.MainPage.MainMenu.getSelected().getId().toLowerCase();
    var isMain = parent.Ts.MainPage.MainTabs.find(0, parent.Ts.Ui.Tabs.Tab.Type.Main).getIsSelected();

    return (menuID !== 'mnichat' || (menuID === 'mnichat' && !isMain));
}

function BlinkWindowTitle() {
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
};

$(document).bind('dragover', function (e) {
    var dropZone = $('.current-chat'),
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

        try {
            if (evt.type in evtMap) {
                document.body.className = evtMap[evt.type];
            } else {
                document.body.className = this[hidden] ? "hidden" : "visible";
            }

            _isChatWindowActive = document.body.className == "visible";
        } catch (err) {
        }
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
