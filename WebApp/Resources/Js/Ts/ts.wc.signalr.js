﻿
var chatHubClient = $.connection.socket;
var ticketSocket = $.connection.ticketSocket;
var notify = false;

function loadSignalR(url) { 
    $("#jquery_jplayer_1").jPlayer({
        ready: function () {
            $(this).jPlayer("setMedia", {
                mp3: "vcr/1_9_0/Audio/chime.mp3"
            });
        },
        loop: false,
        swfPath: "vcr/1_9_0/Js"
    });

    //Debug reasons
    //$.connection.hub.logging = true;
    $.connection.hub.url = url;
    // Start the connection only if on main wc page

    $.connection.hub.qs = "userID=" + top.Ts.System.User.UserID + "&organizationID=" + top.Ts.System.User.OrganizationID;
    //$.connection.hub.qs = "organizationID=" + top.Ts.System.User.OrganizationID;

    var tryingToReconnect = false;

    $.connection.hub.reconnecting(function () {
        var mainWC = $("#iframe-mniWC2");
        try {
            if (mainWC[0].contentWindow.updateStatusReconnecting) { mainWC[0].contentWindow.updateStatusReconnecting(); }
        } catch (err) { }
        tryingToReconnect = true;
    });

    $.connection.hub.reconnected(function () {
        var mainWC = $("#iframe-mniWC2");
        try {
            if (mainWC[0].contentWindow.updateStatusReconnected) { mainWC[0].contentWindow.updateStatusReconnected(); }
        } catch (err) { }
        tryingToReconnect = false;
    });

    $.connection.hub.disconnected(function () {
        if (tryingToReconnect) {
                setTimeout(function () {
                    $.connection.hub.start();
                }, 5000);
            //location.reload(); // Reload the connection if it has disconnected
        }
    });

    chatHubClient.client.chatMessage = function (message, chatID, chatname) {

        chatWith(chatname, chatID);
        chatAddMsg(chatID, message, chatname);

        if (notify) {
            $("#jquery_jplayer_1").jPlayer("setMedia", {
                mp3: "vcr/1_9_0/Audio/chime.mp3"
            }).jPlayer("play", 0);
        }
    };

    function getChildWindows()  {
      var result = [];

      function addWindow(element) {
        for (var i = 0; i < element.length; i++) {
          try {
            if (element[i].contentWindow && element[i].contentWindow != null) result.push(element[i].contentWindow);
          } catch (e) {}
        }
      }

      addWindow($("#iframe-mniWC2"));
      addWindow($("#iframe-mniGroups").contents().find("#ctl00_ContentPlaceHolder1_groupContentFrame"));
        //addWindow($("#iframe-mniCustomers").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations"));
      addWindow($(".customerIframe").contents().find("#watercoolerIframe"));
      addWindow($("#iframe-mniProducts").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations"));
      addWindow($(".ticketIframe").contents().find("#watercoolerIframe"));  
      return result;
    }

    function getTicketWindows() {
        var result = [];

        function addWindow(element) {
            for (var i = 0; i < element.length; i++) {
                try {
                    if (element[i].contentWindow && element[i].contentWindow != null) result.push(element[i].contentWindow);
                } catch (e) { }
            }
        }

        addWindow($("#iframe-mniWC2"));
        addWindow($("#iframe-mniGroups").contents().find("#ctl00_ContentPlaceHolder1_groupContentFrame"));
        //addWindow($("#iframe-mniCustomers").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations"));
        addWindow($(".customerIframe").contents().find("#watercoolerIframe"));
        addWindow($("#iframe-mniProducts").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations"));
        addWindow($(".ticketIframe").contents().find("#watercoolerIframe"));
        return result;
    }

    chatHubClient.addMessage = function (data) {
        //    $("#mainWC").contents().find("#messages").append('<li>' + data + '</li>');
        //    $(".ticketIframe").contents().find("#watercoolerIframe").contents().find("#messages").append('<li>' + data + '</li>');

        //$("#mainWC").contents().teststring(data);
        //$("#mainWC").contentWindow.teststring(data);
        var mainWC = $("#mainWC");
        mainWC[0].contentWindow.teststring(data);
        var ticketWC = $(".ticketIframe").contents().find("#watercoolerIframe");
        for (var i = 0; i < ticketWC.length; i++) {
            ticketWC[i].contentWindow.teststring(data);
        }
    };

    chatHubClient.client.addThread = function (message) {
      var windows = getChildWindows();
      for (var i = 0; i < windows.length; i++) {
        try { if (windows[i].addThread) windows[i].addThread(message); } catch (err) { }
      }
    };

    chatHubClient.client.addComment = function (message) {
      var windows = getChildWindows();
      for (var i = 0; i < windows.length; i++) {
          try { if (windows[i].addComment) windows[i].addComment(message); } catch (err) { }
      }
    };

    chatHubClient.client.deleteMessage = function (messageID, parentID) {
      var windows = getChildWindows();
      for (var i = 0; i < windows.length; i++) {
        try { if (windows[i].deleteMessage) windows[i].deleteMessage(messageID, parentID); } catch (err) { }
      }
    };

    chatHubClient.client.updateLikes = function (likes, messageID, messageParentID) {
      var windows = getChildWindows();
      for (var i = 0; i < windows.length; i++) {
        try { if (windows[i].updateLikes) windows[i].updateLikes(likes, messageID, messageParentID); } catch (err) { }
      }
    };

    chatHubClient.client.updateattachments = function (message) {
      var windows = getChildWindows();
      for (var i = 0; i < windows.length; i++) {
        try { if (windows[i].updateattachments) windows[i].updateattachments(message); } catch (err) { }
      }
    };

    chatHubClient.client.disconnect = function (windowid) {
      var windows = getChildWindows();
      for (var i = 0; i < windows.length; i++) {
        try { if (windows[i].disconnect) windows[i].disconnect(windowid); } catch (err) { }
      }

      var mainWC = $("#iframe-mniUsers");
        try {
            if (mainWC[0].contentWindow.Update) { mainWC[0].contentWindow.Update(); }
        } catch (err) { }
    };

    chatHubClient.client.updateUsers = function () {
      var mainWC = $("#iframe-mniWC2");
      try {
        if (mainWC[0].contentWindow.updateUsers) { mainWC[0].contentWindow.updateUsers(); }
      } catch (err) { }

      var userPage = $("#iframe-mniUsers");
        try {
            if (userPage[0].contentWindow.Update) { userPage[0].contentWindow.Update(); }
        } catch (err) { }

    };

    ticketSocket.client.displayTicketUpdate = function (ticketNum, updateType) {
        var mergeticket;

        if (ticketNum.indexOf(',') != -1)
        {
            var mergeTickets = ticketNum.split(',');
            var losingTicket = mergeTickets[0];
            mergeticket = 1;
            top.Ts.MainPage.AppNotify("Ticket " + ticketNum, updateType);

            if ($('.main-ticket-' + losingTicket).length > 0) {
                if (!$('.main-ticket-' + losingTicket).is(":visible")) {
                    top.Ts.MainPage.closeTicketTab(losingTicket);
                    top.Ts.MainPage.AppNotify("Ticket " + ticketNum, updateType);
                }
                else {
                    top.Ts.MainPage.AppNotify("Ticket " + ticketNum, updateType, "error");
                }
            }

            ticketNum = mergeTickets[1];

        }

        if ($('.main-ticket-' + ticketNum).length > 0) {
            if (!$('.main-ticket-' + ticketNum).is(":visible") && mergeticket != 1){
                top.Ts.MainPage.AppNotify("Ticket " + ticketNum, updateType);

                if (updateType.indexOf('delete') != -1) {
                    top.Ts.MainPage.closeTicketTab(ticketNum);
                }
            }
            else {
                if (updateType.indexOf('delete') != -1) {
                    top.Ts.MainPage.AppNotify("Ticket " + ticketNum, updateType, "error");
                }
            }
            //$('.main-ticket-' + ticketNum).show();
            //$('.main-ticket-' + ticketNum).addClass("tmp-visible");
            $('.main-ticket-' + ticketNum).find('iframe')[0].contentWindow.loadTicket(ticketNum, 0);
            //$('.main-ticket-' + ticketNum).hide();
            //$('.main-ticket-' + ticketNum).removeClass("tmp-visible");
            
        }


        //if ($('.main-tab-content-item-mniDashboard').length > 0 && $('.main-tab-content-item-mniDashboard').is(":visible"))
            //$('.main-tab-content-item-mniDashboard').find('iframe')[0].contentWindow.refresh();
    };

    ticketSocket.client.getTicketViewing = function (ticketNum) {

        if ($('.main-ticket-' + ticketNum).length > 0) {
            if ($('.main-ticket-' + ticketNum).is(":visible")){
            	window.top.ticketSocket.server.ticketViewingAdd(ticketNum, top.Ts.System.User.UserID);
            	console.log("Requesting all the users viewing ticket " + ticketNum);
            }
        }
    };

    ticketSocket.client.consoleDebug = function (msg) {
    			console.log(msg);
    };

    ticketSocket.client.ticketViewingAdd = function (ticketNum, userID) {
        if ($('.main-ticket-' + ticketNum).length > 0) {
        	if ($('.main-ticket-' + ticketNum).is(":visible")) {
        		console.log("adding user to chat window " + userID);
            	$('.main-ticket-' + ticketNum).find('iframe')[0].contentWindow.addUserViewing(userID);
            }
        }

        var ticketWin = $(".ticketIframe");
        for (var i = 0; i < ticketWin.length; i++) {
            ticketWin[i].contentWindow.removeUserViewing(ticketNum,userID);
        }
    };

    ticketSocket.client.ticketViewingRemove = function (ticketNum, userID) {
        var ticketWin = $(".ticketIframe");
        for (var i = 0; i < ticketWin.length; i++) {
            ticketWin[i].contentWindow.removeUserViewing(null, userID);
        }
    };

    $.connection.hub.start(function () {
        chatHubClient.server.login();
        var mainWC = $("#iframe-mniWC2");
        try {
            if (mainWC[0].contentWindow.updateStatusReconnected) { mainWC[0].contentWindow.updateStatusReconnected(); }
        } catch (err) { }
    });

    originalTitle = document.title;

    $(window).blur(function () {
        notify = true;
    });

    $(window).focus(function () {
        notify = false;
    });

}

function openChat(name, chatid) {
    chatWith(name, chatid);
}

function chime(chimeType) {
        $("#jquery_jplayer_1").jPlayer("setMedia", {
            mp3: "vcr/1_9_0/Audio/drop.mp3"
        }).jPlayer("play", 0);
}

