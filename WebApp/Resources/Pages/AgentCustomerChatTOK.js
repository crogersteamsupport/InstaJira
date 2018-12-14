var session;
var token;
var apiKey;
var sessionId;
var publisher;
var screenSharingPublisher;
var tokTimer;
var sharedSessionID;
var sharedApiKey;
var sharedToken;
var tokpopup;
var movingAvg = null;
var siteUrl;
var streamType;

function SetupTOK() {
    var windowUrl = window.location.href;
    var arr = windowUrl.split("/");
    siteUrl = arr[0] + "//" + arr[2];
    var isSafari = Object.prototype.toString.call(window.HTMLElement).indexOf('Constructor') > 0 || (function (p) { return p.toString() === "[object SafariRemoteNotification]"; })(!window['safari'] || safari.pushNotification);
    var isIE = /*@cc_on!@*/false || !!document.documentMode;
    var isEdge = !isIE && !!window.StyleMedia;

    if (isSafari || isEdge) {
        $('#chat-tok-video').hide();
        $('#chat-tok-audio').hide();
        $('#chat-tok-screen').hide();
    }

    $('#subscriberMeter').hide();

    $('#chat-tok-video').click(function (e) {
        var video = $('#chat-tok-video > .btn-primary');

        if (!video.hasClass('disabled')) {
            top.Ts.System.logAction('Chat - Video Call Button Clicked');
            publishTOKVideo(function () {
                startVideoStreaming();
            });
        }
    });

    $('#chat-tok-audio').click(function (e) {
        var audio = $('#chat-tok-audio > .btn-primary');

        if (!audio.hasClass('disabled')) {
            top.Ts.System.logAction('Chat -Audio Call Button Clicked');
            publishTOKAudio(function () {
                startAudioStreaming();
            });
        }
    });

    $('#chat-tok-screen').click(function (e) {
        var screen = $('#chat-tok-screen > .btn-primary');

        if (!screen.hasClass('disabled')) {
            top.Ts.System.logAction('Chat - Share Screen Button Clicked');
            publishTOKScreen();
        }
    });
}

function publishTOKVideo(callback) {
    //if (session !== undefined || publisher !== undefined)
    //    stopTOKStream();

    if (OT.checkSystemRequirements() == 1) {
        var dynamicPub = $("#publisher");

        if (dynamicPub.length == 0)
            dynamicPub = $("#tempContainer");

        $('.current-chat-area').height('calc(100vh - 225px)');
        $('#tokStreamControls').show();
        $('#tokStatusText').text('Requesting Live Session...');
        $('#muteStream').show();
        $('#unmuteStream').hide();

        top.Ts.Services.Chat.GetTOKSessionInfo(function (resultID) {
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

        $('.current-chat-area').height('calc(100vh - 225px)');
        $('#tokStreamControls').show();
        var isIE = /*@cc_on!@*/false || !!document.documentMode;
        var isEdge = !isIE && !!window.StyleMedia;

        if (!isIE && !isEdge) {
            $('#subscriberMeter').show();
        }

        $('#tokStatusText').text('Requesting Live Session...');
        $('#muteStream').show();
        $('#unmuteStream').hide();

        top.Ts.Services.Chat.GetTOKSessionInfo(function (resultID) {
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
        });
    }
    else {
        alert("Your client does not support audio.")
    }
}

function publishTOKScreen() {
    //if (session !== undefined || publisher !== undefined)
    //    stopTOKStream();

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
        } else if (response.extensionInstalled === false && BrowserDetect.browser != "Mozilla") {
            // prompt to install the response.extensionRequired extension
            if (BrowserDetect.browser == "Chrome") {
                $('#ChromeInstallModal').modal('show');
            }
            else if (BrowserDetect.browser == "Firefox") {
                $('#FireFoxInstallModal').modal('show');
            }

            $('#screenRecordingContainer').hide();
        }
        else {
            $('.current-chat-area').height('calc(100vh - 225px)');
            $('#tokStreamControls').show();
            $('#tokStatusText').text('Sharing Screen.');
            top.Ts.Services.Chat.GetTOKSessionInfo(function (resultID) {
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

                //// Screen sharing is available. Publish the screen.
                //// Create an element, but do not display it in the HTML DOM:
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
                channel.trigger('client-tok-video-accept', { userName: channel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
                $('#tokStatusText').text(customerName + ' has joined live session.');
            });
        }
    }, 25);
}

function subscribeToAudioStream() {
    if (session !== undefined || publisher !== undefined)
        stopTOKStream();

    var isIE = /*@cc_on!@*/false || !!document.documentMode;
    var isEdge = !isIE && !!window.StyleMedia;

    if (isIE || isEdge) {
        var tokenURI = encodeURIComponent(sharedToken);
        tokpopup = window.open(siteUrl + '/screenshare/TOKSharedSession.html?sessionid=' + sharedSessionID + '&token=' + tokenURI, 'TSTOKSharedSession', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=1250,height=1000');
        $('#audioRequest').remove();

        setTimeout(function () {
            if (!tokpopup || tokpopup.outerHeight === 0) {
                alert("Popup Blocker is enabled! Please add this site to your exception list.");
            } else {
                publishTOKAudio(function () {
                    channel.trigger('client-tok-audio-accept', { userName: channel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
                    $('#tokStatusText').text(customerName + ' has joined live session.');
                });
            }
        }, 500);
    } else {
        ReceiveAudioStream(sharedSessionID, sharedToken);
        $('#audioRequest').remove();

        publishTOKAudio(function () {
            channel.trigger('client-tok-audio-accept', { userName: channel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
            $('#tokStatusText').text(customerName + ' has joined live session.');
        });
    }
}

function ReceiveAudioStream(sessionID, token) {
    top.Ts.Services.Chat.GetTOKSessionInfo(function (resultID) {
        apiKey = resultID[2];

        var dynamicPub = $("#audioTOKContainer");
        dynamicPub.show();
        dynamicPub.attr("id", "tempContainer");
        dynamicPub.attr("width", "100%");
        dynamicPub.attr("height", "100%");

        if (dynamicPub.length == 0)
            dynamicPub = $("#tempContainer");

        var session = OT.initSession(apiKey, sessionID);
        session.connect(token, function (error) {
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
            session.on('streamCreated', function (event) {
                session.subscribe(event.stream, dynamicPub.attr('id'), subscribeProperties);
                var subscriber = session.subscribe(event.stream, dynamicPub.attr('id'), subscribeProperties);
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
                    document.getElementById('subscriberMeter').value = logLevel;
                });
            });

            session.on('streamDestroyed', function (event) {
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
    var screen = $('#chat-tok-screen > .btn-primary');
    screen.addClass('disabled');
    muteTOKStream();

    //Account for popup blockers
    setTimeout(function () {
        if (!tokpopup || tokpopup.outerHeight === 0) {
            //First Checking Condition Works For IE & Firefox
            //Second Checking Condition Works For Chrome
            alert("Popup Blocker is enabled! Please add this site to your exception list.");
        } else {

        }
    }, 25);
}

function startVideoStreaming() {
    //Send a signal over Pusher to any parties to notify of screen sharing stream.
    channel.trigger('client-tok-video', { userName: channel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
    streamType = 'video';
};

function startAudioStreaming() {
    //Send a signal over Pusher to any parties to notify of agent audio sharing stream.
    channel.trigger('client-tok-audio', { userName: channel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
    streamType = 'audio';
};

function startScreenStreaming() {
    //Send a signal over Pusher to any parties to notify of screen sharing stream.
    channel.trigger('client-tok-screen', { userName: channel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
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

        if (publisher !== undefined) {
            session.unpublish(screenSharingPublisher);
            screenSharingPublisher.destroy();
            screenSharingPublisher = undefined;
        }

        session.disconnect();
        session = undefined;
    }

    $('#tokStreamControls').hide();
    document.getElementById('subscriberMeter').value = 0;
    $('#subscriberMeter').hide();

    $('.current-chat-area').height('height: calc(100vh - 155px);');

    var enableAudio = true;
    var enableVideo = true;
    var enableScreen = true;

    if (!isTOKEnabledForBrowser) {
        enableAudio = false;
        enableVideo = false;
        enableScreen = false;
        EnableTOKButtons(enableAudio, enableVideo, enableScreen, false);
    } else {
        top.Ts.Services.Chat.GetAgentChatProperties(function (data) {
            if (!data.TOKScreenEnabled)
                enableScreen = false;
            if (!data.TOKVideoEnabled)
                enableVideo = false;
            if (!data.TOKVoiceEnabled)
                enableAudio = false;

            EnableTOKButtons(enableAudio, enableVideo, enableScreen, isInit);
        });
    }

    if(tokpopup)
        tokpopup.close();

    channel.trigger('client-agent-tok-ended', { streamType: streamType });
};

function muteTOKStream() {
    publisher.publishAudio(false);
    $('#unmuteStream').show();
    $('#muteStream').hide();
};

function unmuteTOKStream() {
    publisher.publishAudio(true);
    $('#muteStream').show();
    $('#unmuteStream').hide();
};
