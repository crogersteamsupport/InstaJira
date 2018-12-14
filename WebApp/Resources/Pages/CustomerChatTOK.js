var session;
var token;
var apiKey;
var sessionId;
var publisher;
var screenSharingPublisher;
var tokTimer;
var stream;
var sharedSessionID;
var sharedApiKey;
var sharedToken;
var tokpopup;
var isSafari = Object.prototype.toString.call(window.HTMLElement).indexOf('Constructor') > 0 || (function (p) { return p.toString() === "[object SafariRemoteNotification]"; })(!window['safari'] || safari.pushNotification);
var isIE = /*@cc_on!@*/false || !!document.documentMode;
var isEdge = !isIE && !!window.StyleMedia;
var isScreenShareDisabled = false;
var movingAvg = null;
var siteUrl;
var streamType;

function SetupTOK() {
    var windowUrl = window.location.href;
    var arr = windowUrl.split("/");
    siteUrl = arr[0] + "//" + arr[2];

    $('#agentVoiceMeter').hide();

    $('#chat-tok-video').click(function (e) {
        publishTOKVideo(function () {
            startVideoStreaming();
        });
    });

    $('#chat-tok-audio').click(function (e) {
        publishTOKAudio(function () {
            startAudioStreaming();
        });
    });

    $('#chat-tok-screen').click(function (e) {
        if (!isScreenShareDisabled) {
            publishTOKScreen();
        }
    });
}

function publishTOKVideo(callback) {
    if (OT.checkSystemRequirements() == 1) {
        var dynamicPub = $("#publisher");

        if (dynamicPub.length == 0)
            dynamicPub = $("#tempContainer");

        $('.panel-body').height('calc(100vh - 172px)');
        $('#tokStreamControls').show();
        $('#tokStatusText').text('Requesting Live Session...');

        var data = { chatID: _activeChatID };
        IssueAjaxRequest("GetTOKSessionInfoClient", data,
        function (resultID) {
            sessionId = resultID[0];
            token = resultID[1];
            apiKey = resultID[2];
            session = OT.initSession(apiKey, sessionId);
            session.connect(token, function (error) {
                publisher = OT.initPublisher(dynamicPub.attr('id'), {
                    insertMode: 'append',
                    style: { buttonDisplayMode: 'off' },
                    width: '100%',
                    height: '100%'
                });
                session.publish(publisher);
            });

            session.on("connectionDestroyed", function (event) {
                stopTOKStream(event);
            });

            callback();
        },
        function (error) {
            consol.log(error.message)
        });
    }
    else {
        alert("Your client does not support video recording.")
    }
}

function publishTOKAudio(callback) {
    if (OT.checkSystemRequirements() == 1) {
        var dynamicPub = $("#publisher");

        if (dynamicPub.length == 0)
            dynamicPub = $("#tempContainer");

        $('.panel-body').height('calc(100vh - 172px)');
        $('#tokStreamControls').show();
        var isIE = /*@cc_on!@*/false || !!document.documentMode;
        var isEdge = !isIE && !!window.StyleMedia;

        if (!isIE && !isEdge) {
            $('#agentVoiceMeter').show();
        }

        $('#tokStatusText').text('Requesting Live Session...');

        var data = { chatID: _activeChatID };
        IssueAjaxRequest("GetTOKSessionInfoClient", data,
        function (resultID) {
            sessionId = resultID[0];
            token = resultID[1];
            apiKey = resultID[2];
            session = OT.initSession(apiKey, sessionId);
            session.connect(token, function (error) {
                publisher = OT.initPublisher(dynamicPub.attr('id'), {
                    videoSource: null,
                    insertMode: 'append',
                    style: { buttonDisplayMode: 'off' },
                    width: '100%',
                    height: '100%'
                });
                session.publish(publisher);
            });

            session.on("connectionDestroyed", function (event) {
                stopTOKStream(event);
            });

            callback();
        },
        function (error) {
            console.log(error.message)
        });
    }
    else {
        alert("Your client does not support audio.")
    }
}

function publishTOKScreen() {
    var dynamicPub = $("#screenShare");
    $("#screenRecordingContainer").show();
    dynamicPub.show();

    var clonedScreen = $('#screenOne').clone();
    clonedScreen.attr("id", "screenOneClone");
    var clonedVid = $('#screenTwo').clone();
    clonedVid.attr("id", "screenOneClone");

    OT.registerScreenSharingExtension('chrome', 'dlbbnbankbgdoidhinoomgojjbdamofh', 2);

    OT.checkScreenSharingCapability(function (response) {
        if (!response.supported || response.extensionRegistered === false) {
            alert("This browser does not support screen sharing");
        } else if (response.extensionInstalled === false) {
            var isChrome = !!window.chrome && !!window.chrome.webstore;
            var isFirefox = typeof InstallTrigger !== 'undefined';
            // prompt to install the response.extensionRequired extension
            if (isChrome) {
                $('#ChromeInstallModal').modal('show');
            }
            else if (typeof InstallTrigger !== 'undefined') {
                $('#FireFoxInstallModal').modal('show');
            }

            $('#screenRecordingContainer').hide();
        }
        else {
            $('.panel-body').height('calc(100vh - 172px)');
            $('#tokStreamControls').show();
            $('#tokStatusText').text('Sharing Screen.');
            var data = { chatID: _activeChatID };
            IssueAjaxRequest("GetTOKSessionInfoClient", data,
            function (resultID) {
                sessionId = resultID[0];
                token = resultID[1];
                apiKey = resultID[2];
                session = OT.initSession(apiKey, sessionId);
                var pubOptions = { publishAudio: true, publishVideo: false };
                publisher = OT.initPublisher('screenTwo', pubOptions);

                session.connect(token, function (error) {
                    session.publish(publisher);
                    startScreenStreaming();
                    muteTOKStream();
                });

                // Screen sharing is available. Publish the screen.
                // Create an element, but do not display it in the HTML DOM:
                var screenContainerElement = document.createElement('div');
                var publishOptions = {};
                publishOptions.videoSource = 'screen';
                publishOptions.maxResolution = { width: 1920, height: 1080 };
                publishOptions.mirror = false;
                publishOptions.width = 1920;
                publishOptions.height = 1080;

                screenSharingPublisher = OT.initPublisher(
                      'screenOne',
                      publishOptions,
                      function (error) {
                          if (error) {
                              console.log(error);
                          } else {
                              screenSharingPublisher.element.style.width = screenSharingPublisher.videoWidth() + 'px';
                              screenSharingPublisher.element.style.height = screenSharingPublisher.videoHeight() + 'px';

                              session.publish(
                                screenSharingPublisher,
                                function (error) {
                                    if (error) {
                                        alert('Screen Recording will not start because, ' + error.message);
                                    }
                                });
                        }
                });


                session.on("connectionDestroyed", function (event) {
                    stopTOKStream(event);
                });
            },
            function (error) {

            });
        }
    });
}

function subscribeToVideoStream() {
    if (session !== undefined || publisher !== undefined)
        stopTOKStream();

    var tokenURI = encodeURIComponent(sharedToken);
    tokpopup = window.open(siteUrl + '/screenshare/TOKSharedSession.html?sessionid=' + sharedSessionID + '&token=' + tokenURI, 'TSTOKSharedSession', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=1000,height=800');
    $('#videoRequest').remove();

    setTimeout(function () {
        if (!tokpopup || tokpopup.outerHeight === 0) {
            //First Checking Condition Works For IE & Firefox
            //Second Checking Condition Works For Chrome
            alert("Popup Blocker is enabled! Please add this site to your exception list.");
        } else {
            publishTOKVideo(function () {
                presenceChannel.trigger('client-tok-video-user-accept', { userName: presenceChannel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId, agentRequesting: _agentName });
                $('#tokStatusText').text(_agentName + ' has joined live session.');
            });
        }
    }, 25);
}

function subscribeToAudioStream() {
    if (session !== undefined || publisher !== undefined)
        stopTOKStream();

    if (isIE || isEdge) {
        var tokenURI = encodeURIComponent(sharedToken);
        tokpopup = window.open(siteUrl + '/screenshare/TOKSharedSession.html?sessionid=' + sharedSessionID + '&token=' + tokenURI, 'TSTOKSharedSession', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=1250,height=1000');
        $('#audioRequest').remove();
        setTimeout(function () {
            if (!tokpopup || tokpopup.outerHeight === 0) {
                alert("Popup Blocker is enabled! Please add this site to your exception list.");
            } else {
                publishTOKAudio(function () {
                    presenceChannel.trigger('client-tok-audio-user-accept', { userName: presenceChannel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
                    $('#tokStatusText').text(_agentName + ' has joined live session.');
                });
            }
        }, 500);
    } else {
        ReceiveAudioStream(sharedSessionID, sharedToken);
        $('#audioRequest').remove();

        publishTOKAudio(function () {
            presenceChannel.trigger('client-tok-audio-user-accept', { userName: presenceChannel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId, agentRequesting: _agentName });
            $('#tokStatusText').text(_agentName + ' has joined live session.');
        });
    }
}

function ReceiveAudioStream(sessionID, token) {
    var data = { chatID: 1 };
    IssueAjaxRequest("GetTOKSessionInfoClient", data,
    function (resultID) {
        apiKey = resultID[2];

        var dynamicPub = $("#audioTOKContainer");
        dynamicPub.show();
        dynamicPub.attr("id", "tempContainer");
        dynamicPub.attr("width", "100%");
        dynamicPub.attr("height", "100%");

        if (dynamicPub.length == 0)
            dynamicPub = $("#tempContainer");

        var stream = OT.initSession(apiKey, sessionID);
        stream.connect(token, function (error) {
            var subscribeProperties = {
                insertMode: 'append',
                style: {
                    buttonDisplayMode: 'off',
                    audioLevelDisplayMode: 'off'
                },
                width: '100%',
                height: '0px',
                audioVolume: 100,
                subscribeToAudio: true
            };
            stream.on('streamCreated', function (event) {
                stream.subscribe(event.stream, dynamicPub.attr('id'), subscribeProperties);
                var subscriber = stream.subscribe(event.stream, dynamicPub.attr('id'), subscribeProperties);
                dynamicPub.hide();

                subscriber.on('audioLevelUpdated', function (event) {
                    if (movingAvg === null || movingAvg <= event.audioLevel) {
                        movingAvg = event.audioLevel;
                    } else {
                        movingAvg = 0.7 * movingAvg + 0.3 * event.audioLevel;
                    }

                    // 1.5 scaling to map the -30 - 0 dBm range to [0,1]
                    var logLevel = (Math.log(movingAvg) / Math.LN10) / 1.5 + 1;
                    logLevel = Math.min(Math.max(logLevel, 0), 1);
                    document.getElementById('agentVoiceMeter').value = logLevel;
                });
            });

            stream.on('streamDestroyed', function (event) {
                stopTOKStream(event);
            });
        });
    });
}

function subscribeToScreenStream() {
    if (session !== undefined || publisher !== undefined)
        stopTOKStream();

    var tokenURI = encodeURIComponent(sharedToken);
    tokpopup = window.open(siteUrl + '/screenshare/TOKSharedSession.html?sessionid=' + sharedSessionID + '&token=' + tokenURI, 'TSTOKSharedSession', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=1000,height=800');
    $('#screenRequest').remove();
    DisableScreenShare(true);
    muteTOKStream();

    setTimeout(function () {
        if (!tokpopup || tokpopup.outerHeight === 0) {
            //First Checking Condition Works For IE & Firefox
            //Second Checking Condition Works For Chrome
            alert("Popup Blocker is enabled! Please add this site to your exception list.");
        } else {
            publishTOKAudio(function () {
                presenceChannel.trigger('client-tok-audio-user-accept', { userName: presenceChannel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
                $('#tokStatusText').text(_agentName + ' has joined live session.');
            });
        }
    }, 25);
}

function startVideoStreaming() {
    //Send a signal over Pusher to any parties to notify of screen sharing stream.
    presenceChannel.trigger('client-tok-video-user', { userName: presenceChannel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
    streamType = 'video';
};

function startAudioStreaming() {
    //Send a signal over Pusher to any parties to notify of customer audio sharing stream.
    presenceChannel.trigger('client-tok-audio-user', { userName: presenceChannel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
    streamType = 'audio';
};

function startScreenStreaming() {
    //Send a signal over Pusher to any parties to notify of screen sharing stream.
    presenceChannel.trigger('client-tok-screen-user', { userName: presenceChannel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
    streamType = 'screen';
};

function stopTOKStream(e) {
    $('#tokStatusText').text('Ending live session...');

    if (session !== undefined) {

        if (publisher !== undefined) {
            session.unpublish(publisher);
            publisher.destroy();
            publisher = undefined;
        }

        if (screenSharingPublisher !== undefined) {
            session.unpublish(screenSharingPublisher);
            screenSharingPublisher.destroy();
            screenSharingPublisher = undefined;
        }

        session.disconnect();
        session = undefined;
    }

    $('#tokStreamControls').hide();
    document.getElementById('agentVoiceMeter').value = 0;
    $('#agentVoiceMeter').hide();
    $('.panel-body').height('calc(100vh - 95px)');
    DisableScreenShare(false);

    if (tokpopup)
        tokpopup.close();

    presenceChannel.trigger('client-tok-ended', { streamType: streamType });
};

function muteTOKStream(e) {
    publisher.publishAudio(false);
    $('#unmuteStream').show();
    $('#muteStream').hide();
};

function unmuteTOKStream(e) {
    publisher.publishAudio(true);
    $('#muteStream').show();
    $('#unmuteStream').hide();
};

function DisableScreenShare(disable) {
    var screen = $('.dropdown-menu li:contains(Screen)');
    isScreenShareDisabled = disable;

    if (disable) {
        screen.addClass('disabled');
    } else {
        screen.removeClass('disabled');
    }
}
