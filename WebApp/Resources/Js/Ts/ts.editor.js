﻿var initEditor = function (element, shouldResize, init, postinit) {
    top.Ts.Settings.System.read('EnableScreenR', 'True', function (enableScreenR) {
        var resizePluginCode = ''; 
        if (shouldResize)
        {
            resizePluginCode = 'autoresize';
        }
        var editorOptions = {
        	plugins: "paste link code textcolor image imagetools moxiemanager table " + resizePluginCode,
        	toolbar1: "insertPasteImage insertKb insertTicket image insertimage insertDropBox insertUser recordVideo recordScreenTok | link unlink | undo redo removeformat | cut copy paste pastetext | outdent indent | bullist numlist",
        	toolbar2: "alignleft aligncenter alignright alignjustify | forecolor backcolor | fontselect fontsizeselect styleselect | bold italic underline strikethrough blockquote | code | table",
            statusbar: true,
            gecko_spellcheck: true,
            extended_valid_elements: "a[accesskey|charset|class|coords|dir<ltr?rtl|href|hreflang|id|lang|name|onblur|onclick|ondblclick|onfocus|onkeydown|onkeypress|onkeyup|onmousedown|onmousemove|onmouseout|onmouseover|onmouseup|rel|rev|shape<circle?default?poly?rect|style|tabindex|title|target|type],script[charset|defer|language|src|type],table[class=table|border:1],iframe[src|width|height|frameborder|webkitallowfullscreen|mozallowfullscreen|allowfullscreen]",
            content_css: "../Css/jquery-ui-latest.custom.css,../Css/editor.css",
            convert_urls: true,
            autoresize_bottom_margin: 20,
            remove_script_host: false,
            relative_urls: false,
            template_external_list_url: "tinymce/jscripts/template_list.js",
            external_link_list_url: "tinymce/jscripts/link_list.js",
            external_image_list_url: "tinymce/jscripts/image_list.js",
            media_external_list_url: "tinymce/jscripts/media_list.js",
            menubar: false,
            moxiemanager_leftpanel: false,
            moxiemanager_fullscreen: false,
            moxiemanager_title: top.Ts.System.Organization.Name,
            moxiemanager_hidden_tools: (top.Ts.System.User.IsSystemAdmin == true) ? "" : "manage",
            paste_data_images: true,
            moxiemanager_image_settings: {
                moxiemanager_rootpath: "/" + top.Ts.System.Organization.OrganizationID + "/images/",
                extensions: 'gif,jpg,jpeg,png'
            },
            images_upload_url: "/Services/UserService.asmx/SaveTinyMCEPasteImage",
            setup: function (ed) {
                ed.on('init', function (e) {
                    top.Ts.System.refreshUser(function () {
                        if (top.Ts.System.User.FontFamilyDescription != "Unassigned") {
                            ed.execCommand("FontName", false, GetTinyMCEFontName(top.Ts.System.User.FontFamily));
                            ed.getBody().style.fontFamily = GetTinyMCEFontName(top.Ts.System.User.FontFamily);
                        }
                        else if (top.Ts.System.Organization.FontFamilyDescription != "Unassigned") {
                            ed.execCommand("FontName", false, GetTinyMCEFontName(top.Ts.System.Organization.FontFamily));
                            ed.getBody().style.fontFamily = GetTinyMCEFontName(top.Ts.System.Organization.FontFamily);
                        }

                        if (top.Ts.System.User.FontSize != "0") {
                            ed.execCommand("FontSize", false, top.Ts.System.User.FontSizeDescription);
                            ed.getBody().style.fontSize = GetTinyMCEFontSize(top.Ts.System.User.FontSize + 1);
                        }
                        else if (top.Ts.System.Organization.FontSize != "0") {
                            ed.execCommand("FontSize", false, top.Ts.System.Organization.FontSize + 1);
                            ed.getBody().style.fontSize = GetTinyMCEFontSize(top.Ts.System.Organization.FontSize + 1);
                        }

                      if(postinit) postinit();
                    });
                    
                });

                //ed.on('paste', function (ed, e) {
                //    setTimeout(function () { ed.execCommand('mceAutoResize'); }, 1000);
                //});

                ed.addButton('insertTicket', {
                    title: 'Insert Ticket',
                    //image: '../images/nav/16/tickets.png',
                    icon: 'awesome fa fa-ticket',
                    onclick: function () {
                        top.Ts.System.logAction('Ticket - Ticket Inserted');

                        top.Ts.MainPage.selectTicket(null, function (ticketID) {
                            top.Ts.Services.Tickets.GetTicket(ticketID, function (ticket) {
                              ed.focus();
                              if (_ticketID) {
                                top.Ts.Services.Tickets.AddRelated(_ticketID, ticketID, null, function (tickets) {
                                  appendRelated(tickets);
                                  //window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addrelationship", userFullName);
                                }, function (error) {
                                  //container.remove();
                                  alert(error.get_message());
                                });
                              }
                                var html = '<a href="' + top.Ts.System.AppDomain + '?TicketNumber=' + ticket.TicketNumber + '" target="_blank" onclick="top.Ts.MainPage.openTicket(' + ticket.TicketNumber + '); return false;">Ticket ' + ticket.TicketNumber + '</a>';
                                ed.selection.setContent(html);
                                ed.execCommand('mceAutoResize');
                                ed.focus();
                            }, function () {
                                alert('There was a problem inserting the ticket link.');
                            });
                        });
                    }
                });

                ed.addButton('insertPasteImage', {
                  title: 'Insert Image from Clipboard',
                    //image: '../images/nav/16/imagepaste.png',
                    icon: 'awesome fa fa-paste',
                    onclick: function () {

                      if (BrowserDetect.browser == 'Safari' || BrowserDetect.browser == 'Explorer' || (BrowserDetect.browser == 'Mozilla' && BrowserDetect.version < 20)) {
                            alert("Sorry, this feature is not supported by your browser");
                        }
                        else {
                            top.Ts.MainPage.pasteImage(null, function (result) {
                                ed.focus();
                                if (result != "") {
                                    var html = '<img src="' + top.Ts.System.AppDomain + '/dc/' + result + '"</a>&nbsp;<br/>';
                                    ed.selection.setContent(html);
                                    setTimeout(function () { ed.execCommand('mceAutoResize'); }, 1000);
                                    ed.execCommand('mceAutoResize');
                                    ed.focus();
                                }
                            });
                        }
                    }
                });

                ed.addButton('insertUser', {
                    title: 'Insert Userstamp',
                    icon: 'awesome fa fa-clock-o',
                    //image: '../images/icons/dropbox.png',
                    onclick: function () {
                        var html = Date(Date.UTC(Date.Now)) + ' ' + top.Ts.System.User.FirstName + ' ' + top.Ts.System.User.LastName + ' : ';
                        ed.selection.setContent(html);
                        ed.execCommand('mceAutoResize');
                        ed.focus();
                    }
                });

                ed.addButton('insertDropBox', {
                    title: 'Insert DropBox',
                    icon: 'awesome fa fa-dropbox',
                    //image: '../images/icons/dropbox.png',
                    onclick: function () {
                        var options = {
                            linkType: "preview",
                            success: function (files) {
                                ed.focus();
                                var html = '<a href=' + files[0].link + '>' + files[0].name + '</a>';
                                ed.selection.setContent(html);
                                ed.execCommand('mceAutoResize');
                                ed.focus();
                                top.Ts.System.logAction('Ticket - Dropbox Added');
                            },
                            cancel: function () {
                                alert('There was a problem inserting the dropbox file.');
                            }
                        };
                        Dropbox.choose(options);
                    }
                });

                ed.addButton('recordVideo', {
                    title: 'Record video',
                    //image: '../images/icons/Symbol_Record.png',
                    icon: 'awesome fa fa-video-camera',
                    onclick: function () {
                        top.Ts.System.logAction('Ticket - Video Recording Button Clicked');
                        if (OT.checkSystemRequirements() == 1 || BrowserDetect.browser == "Mozilla") {
                            var dynamicPub = element.parent().find("#publisher");
                            element.parent().find("#recordVideoContainer").show();
                            dynamicPub.show();
                            dynamicPub.attr("id", "tempContainer");
                            dynamicPub.attr("width", "400px");
                            dynamicPub.attr("height", "400px");

                            if (dynamicPub.length == 0)
                                dynamicPub = element.parent().find("#tempContainer");



                            top.Ts.Services.Tickets.GetSessionInfo(function (resultID) {
                                sessionId = resultID[0];
                                token = resultID[1];
                                session = OT.initSession(apiKey, sessionId);
                                session.connect(token, function (error) {
                                    publisher = OT.initPublisher(dynamicPub.attr('id'), {
                                        insertMode: 'append',
                                        width: '100%',
                                        height: '100%'
                                    });
                                    session.publish(publisher);
                                });
                            });

                        }
                        else {
                            alert("Your client does not support video recording.")
                        }
                    }
                });

                ed.addButton('recordScreenTok', {
                	title: 'Screen Video Recording',
                	//image: '../images/icons/Symbol_Record.png',
                	icon: 'awesome fa fa-circle',
                	onclick: function () {
                		top.Ts.System.logAction('Ticket - Video Screen Recording Button Clicked');
                		if (OT.checkSystemRequirements() == 1 || BrowserDetect.browser == "Mozilla") {
                			var dynamicPub = element.parent().find("#screenShare");
                			element.parent().find("#recordScreenContainer").show();
                			dynamicPub.show();

                			var clonedScreen = $('#ourPubTest').clone();
                			clonedScreen.attr("id", "ourPubTestClone");
                			//$('#ourPubTest').after(clonedScreen);
                			var clonedVid = $('#ourPubTest2').clone();
                			clonedVid.attr("id", "ourPubTest2Clone");
                			//$('#ourPubTest2').after(clonedVid);

                			OT.registerScreenSharingExtension('chrome', 'laehkaldepkacogpkokmimggbepafabg', 2);

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
											  		element.parent().find('#recordScreenContainer').hide();
                				} else {
                					// Screen sharing is available
                					top.Ts.Services.Tickets.GetSessionInfo(function (resultID) {
                						sessionId = resultID[0];
                						token = resultID[1];
                						apiKey = resultID[2];
                						session = OT.initSession(apiKey, sessionId);
                						var pubOptions = { publishAudio: true, publishVideo: false };
                						publisher = OT.initPublisher('ourPubTest2', pubOptions);

                						session.connect(token, function (error) {
                							// publish a stream using the camera and microphone:
                							session.publish(publisher);
                						});


                						// Screen sharing is available. Publish the screen.
                						// Create an element, but do not display it in the HTML DOM:
                						var screenContainerElement = document.createElement('div');
                						screenSharingPublisher = OT.initPublisher(
											  'ourPubTest',
											  { videoSource: 'screen' },
											  function (error) {
											  	if (error) {
											  		if (BrowserDetect.browser == "Chrome") {
											  			$('#ChromeInstallModal').modal('show');
											  		}
											  		else if (BrowserDetect.browser == "Firefox") {
											  			$('#FireFoxInstallModal').modal('show');
											  		}
											  		//alert('Screen Recording will not start because, ' + error.message);
											  		element.parent().find('#recordScreenContainer').hide();
											  		element.parent().find('#rcdtokScreen').hide();
											  		element.parent().find('#canceltokScreen').hide();
											  	} else {
											  		session.publish(
													  screenSharingPublisher,
													  function (error) {
													  	if (error) {
													  		alert('Screen Recording will not statrt because, ' + error.message);
													  		element.parent().find('#recordScreenContainer').hide();
													  		element.parent().find('#rcdtokScreen').hide();
													  		element.parent().find('#canceltokScreen').hide();
													  	}
													  });
											  	}
											  });
                					});
                				}
                			});

               		}
                		else {
                			alert("Your client does not support video recording.")
                		}
                	}
                });

                ed.addButton('insertKb', {
                    title: 'Suggested Solutions',
                    //image: '../images/nav/16/knowledge.png',
                    icon: 'awesome fa fa-book',
                    onclick: function () {
                        suggestedSolutions(element.SuggestedSolutionDefaultInput, function (ticketID, isArticle) {
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

                                    ed.focus();
                                    ed.selection.setContent(html);
                                    ed.execCommand('mceAutoResize');
                                    ed.focus();
                                    top.Ts.System.logAction('Ticket - Suggested Solution Inserted');
                                }, function () {
                                    alert('There was an error inserting your suggested solution ticket.');
                                });
                            }
                            else {
                                top.Ts.Services.Admin.GetHubURLwithCName(function (url) {
                                    var link = "https://" + url + "/knowledgeBase/" + ticketID;
                                    var html = $('<a href="' + link + '" target="_blank">' + link + '</a></br>')[0];
                                    ed.focus();
                                    ed.selection.setContent(html);
                                    ed.execCommand('mceAutoResize');
                                    ed.focus();
                                    top.Ts.System.logAction('Ticket - Suggested Solution Link Inserted');
                                });

                            }
                        });

                        //filter = new top.TeamSupport.Data.TicketLoadFilter();
                        //filter.IsKnowledgeBase = true;
                        //top.Ts.MainPage.selectTicket(filter, function (ticketID) {
                        //    top.Ts.Services.Tickets.GetKBTicketAndActions(ticketID, function (result) {
                        //        if (result === null) {
                        //            alert('There was an error inserting your knowledgebase ticket.');
                        //            return;
                        //        }
                        //        var ticket = result[0];
                        //        var actions = result[1];

                        //        var html = '<div>';

                        //        for (var i = 0; i < actions.length; i++) {
                        //            html = html + '<div>' + actions[i].Description + '</div></br>';
                        //        }
                        //        html = html + '</div>';

                        //        ed.focus();
                        //        ed.selection.setContent(html);
                        //        ed.execCommand('mceAutoResize');
                        //        ed.focus();
                        //        top.Ts.System.logAction('Ticket - KB Inserted');
                        //        //needs to resize or go to end

                        //    }, function () {
                        //        alert('There was an error inserting your knowledgebase ticket.');
                        //    });
                        //});
                    }
                });

                if (enableScreenR.toLowerCase() != 'false' && !(BrowserDetect.browser == 'Safari' && BrowserDetect.OS == "Windows")) {
                  ed.addButton('recordScreen', {
                        title: 'Record Screen',
                        //image: '../images/icons/Symbol_Record.png',
                        icon: 'awesome fa fa-circle',
                        onclick: function () {
                          if ($("#recorder").length == 0) {

                            switch (BrowserDetect.browser) {
                              case "Chrome":
                                top.Ts.Services.Settings.ReadUserSetting('ReadScreenRecordingChromeInfo', 0, function (alreadyReadInfo) {
                                  if (alreadyReadInfo == 0) {
                                    $(".pAllowPluginsToRunInstructions").html("\
To use screen recording in this browser before September of 2015 \
<a href='https://support.google.com/chrome/answer/6213033' target='_blank' class='ui-state-default ts-link'>these instructions</a> \
need to be followed, otherwise the browser will continue to ask to install Java. \
After September 2015 you’ll need to use an alternate web browser like FireFox or Internet Explorer.<br><br>\
Also, the Java plugins need to be allowed to run by clicking on the \
<img src='../Images/icons/ChromePluginIcon.png' alt='plugins icon'> \
on the right side of the address bar");
                                    $('.divScreenRecorderMessages').show();
                                  }
                                });
                                break;
                              case "Firefox":
                                top.Ts.Services.Settings.ReadUserSetting('ReadScreenRecordingFirefoxInfo', 0, function (alreadyReadInfo) {
                                  if (alreadyReadInfo == 0) {
                                    $(".pAllowPluginsToRunInstructions").html("\
Please allow the screen recorder Java plugins to run on your browser by clicking on the \
<img src='../Images/icons/FirefoxPluginIcon.png' alt='plugins icon'> \
on the left side of the address bar.");
                                    $('.divScreenRecorderMessages').show();
                                  }
                                });
                                break;
                              case "Explorer":
                                top.Ts.Services.Settings.ReadUserSetting('ReadScreenRecordingExplorerInfo', 0, function (alreadyReadInfo) {
                                  if (alreadyReadInfo == 0) {
                                    $(".pAllowPluginsToRunInstructions").html("\
Please allow the screen recorder Java plugins to run on your browser by clicking on Allow button at the bottom of the page: \
<img src='../Images/icons/IEPluginWindow.png' alt='plugins icon' width='100%' style='margin-top: 10px'>");
                                    $('.divScreenRecorderMessages').show();
                                  }
                                });
                                break;
                              case "Safari":
                                if (BrowserDetect.OS == "Windows") {
                                  top.Ts.Services.Settings.ReadUserSetting('ReadScreenRecordingSafariInWindowsInfo', 0, function (alreadyReadInfo) {
                                    if (alreadyReadInfo == 0) {
                                      $(".pAllowPluginsToRunInstructions").html("\
This browser in Windows usually fails to detect Java preventing the recorder to start. Read \
<a href='http://stackoverflow.com/questions/11235578/when-viewing-an-applet-why-does-safari-for-windows-display-java-is-unavailable' target='_blank' class='ui-state-default ts-link'>this</a> \
for more information or use an alternate browser like Firefox or Internet Explorer.");
                                      $('.divScreenRecorderMessages').show();
                                    }
                                  });
                                }
                                else {
                                  top.Ts.Services.Settings.ReadUserSetting('ReadScreenRecordingSafariInfo', 0, function (alreadyReadInfo) {
                                    if (alreadyReadInfo == 0) {
                                      $(".pAllowPluginsToRunInstructions").html("\
The following steps will refresh your browser<br><br> \
1. Allow the screen recorder Java plugins to run on your browser by clicking on the Trust button at the top of the page: <br>\
<img src='../Images/icons/SafariInMacPluginDialog.png' alt='plugin dialog' width='30%' style='margin-top: 10px'><br><br> \
2. Navigate to Safari > Preferences > Security > Internet Plugins - Website Settings > Java and change the " + top.Ts.System.AppDomain + " setting to Run in Unsafe Mode and click on the Trust button: <br>\
<img src='../Images/icons/SafariInMacUnsafeModeDialog.png' alt='plugin dialog' width='30%' style='margin-top: 10px'>");
                                      $('.divScreenRecorderMessages').show();
                                    }
                                  });
                                }

                                break;
                              default:
                                top.Ts.Services.Settings.ReadUserSetting('ReadScreenRecordingInfo', 0, function (alreadyReadInfo) {
                                  if (alreadyReadInfo == 0) {
                                    $(".pAllowPluginsToRunInstructions").html("Please verify java is supported and allowed to run in your browser.");
                                    $('.divScreenRecorderMessages').show();
                                  }
                                });
                            }

                            //if (deployJava.versionCheck("1.8.0_45+")) {
                              var applet = document.createElement("applet");
                              applet.id = "recorder";
                              applet.archive = "Launch.jar"
                              applet.code = "com.bixly.pastevid.driver.Launch";
                              applet.width = 200;
                              applet.height = 150;
                              var orgId = top.Ts.System.Organization.OrganizationID;
                              var param1 = document.createElement("param");
                              param1.name = "jnlp_href";
                              param1.value = "launch.jnlp";
                              applet.appendChild(param1);
                              var param2 = document.createElement("param");
                              param2.name = "orgId";
                              param2.value = orgId;
                              applet.appendChild(param2);
                              var param3 = document.createElement("param");
                              param3.name = "permissions";
                              param3.value = 'all-permissions';
                              applet.appendChild(param3);

                              $('.fa-circle').removeClass("fa-circle").addClass("fa-circle-o-notch fa-spin");
                              document.getElementsByTagName("body")[0].appendChild(applet);

                            //}
                            //else {
                            //  userInput = confirm(
                            //          "You need the latest Java(TM) Runtime Environment.\n" +
                            //          "Please restart your computer after updating.\n" +
                            //          "Would you like to update now?");
                            //  if (userInput == true) {
                            //    window.open("http://java.com/en/download", '_blank');
                            //  }
                            //}
                          }
                        }
                    });
                }
            }
            , oninit: init
        };
        $(element).tinymce(editorOptions);
    });
}

var onScreenRecordStart = function () {
  $('.fa-circle-o-notch').removeClass("fa-circle-o-notch fa-spin").addClass("fa-circle");
  switch (BrowserDetect.browser) {
    case "Chrome":
      top.Ts.Services.Settings.WriteUserSetting('ReadScreenRecordingChromeInfo', 1);
      break;
    case "Firefox":
      top.Ts.Services.Settings.WriteUserSetting('ReadScreenRecordingFirefoxInfo', 1);
      break;
    case "Explorer":
      top.Ts.Services.Settings.WriteUserSetting('ReadScreenRecordingExplorerInfo', 1);
      break;
    case "Safari":
      if (BrowserDetect.OS == "Windows") {
        top.Ts.Services.Settings.WriteUserSetting('ReadScreenRecordingSafariInWindowsInfo', 1);
      }
      else {
        top.Services.Settings.WriteUserSetting('ReadScreenRecordingSafariInfo', 1);
      }
      break;
    default:
      top.Ts.Services.Settings.WriteUserSetting('ReadScreenRecordingInfo', 1);
  }
  $('.divScreenRecorderMessages').hide();
};

var onScreenRecordComplete = function (url) {
  $('#recorder').remove();
  if (url) {
    var link = '<a href="' + url + '" target="_blank">Click here to view screen recording video.</a>';
    var html = '<div class="video_holder"><video style="width: 640px; height: 360px;" controls="controls"><source src="' + url + '" type="video/mp4" />' + link + '</video></div>'
    var ed = tinyMCE.activeEditor;
    ed.selection.setContent(html);
    ed.execCommand('mceAutoResize');
    ed.focus();
    top.Ts.System.logAction('Ticket - Screen Recorded');
  }
  else {
    top.Ts.System.logAction('Ticket - Screen Record Cancelled');
  }
};

function GetTinyMCEFontSize(fontSize) {
    var result = '';
    switch (fontSize) {
        case 1:
            result = "8px";
            break;
        case 2:
            result = "10px";
            break;
        case 3:
            result = "12px";
            break;
        case 4:
            result = "14px";
            break;
        case 5:
            result = "18px";
            break;
        case 6:
            result = "24px";
            break;
        case 7:
            result = "36px";
            break;
    }
    return result;
}

function GetTinyMCEFontName(fontFamily) {
    var result = '';
    switch (fontFamily) {
        case 1:
            result = "'andale mono', times";
            break;
        case 2:
            result = "arial, helvetica, sans-serif";
            break;
        case 3:
            result = "'arial black', 'avant garde'";
            break;
        case 4:
            result = "'book antiqua', palatino";
            break;
        case 5:
            result = "'comic sans ms', sans-serif";
            break;
        case 6:
            result = "'courier new', courier";
            break;
        case 7:
            result = "georgia, palatino";
            break;
        case 8:
            result = "helvetica";
            break;
        case 9:
            result = "impact, chicago";
            break;
        case 10:
            result = "symbol";
            break;
        case 11:
            result = "tahoma, arial, helvetica, sans-serif";
            break;
        case 12:
            result = "terminal, monaco";
            break;
        case 13:
            result = "'times new roman', times";
            break;
        case 14:
            result = "'trebuchet ms', geneva";
            break;
        case 15:
            result = "verdana, geneva";
            break;
        case 16:
            result = "webdings";
            break;
        case 17:
            result = "wingdings, 'zapf dingbats'";
            break;
    }
    return result;
}

var execSuggestedSolutions = null;

function suggestedSolutions(defaultInput, callback) {
    $('.dialog-select-ticket2').find('input').val('');
    $('.dialog-select-ticket2').find('input').focus();
    $('#SuggestedSolutionsIFrame').attr('src', '/vcr/1_9_0/Pages/SuggestedSolutions.html');
    $('#SuggestedSolutionsModal').modal('show');
    if (execSuggestedSolutions) {
        return;
    }
    execSuggestedSolutions = true;

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