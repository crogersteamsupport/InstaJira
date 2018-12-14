var execSuggestedSolutions = null;

var initEditorV2 = function (element, callback) {
    _insertedKBTicketID = null;
    execSuggestedSolutions = null;
	top.Ts.Settings.System.read('EnableScreenR', 'True', function (enableScreenR) {
		element.summernote({
			height: 150,
			focus: true,
			toolbar: [
				['media', ['InsertPastedImage', 'InsertDocuments', 'InsertTicket', 'InsertKB', 'InsertDropBox', 'InsertUser', 'InsertVideo', 'InsertScreenRecording', 'link']],
				['style', ['bold', 'italic', 'strikethrough', 'underline', 'clear']],
				['undo', ['undo', 'redo']],
				['font', ['fontname', 'fontsize', 'color']],
				['para', ['ul', 'ol', 'paragraph', 'hr', 'table']],
				['other', ['fullscreen', 'codeview']]
			],
			buttons: {
				InsertUser: insertUser,
				InsertDropBox: insertDropBox,
				InsertTicket: insertTicket,
				InsertVideo: insertVideo,
				InsertKB: insertKB,
				InsertPastedImage: insertPastedImage,
				InsertDocuments: insertDocuments,
				InsertScreenRecording: insertScreenRecording
			},
			callbacks: {
				onInit: function () {
					top.Ts.System.refreshUser(function () {
						if (top.Ts.System.User.FontFamilyDescription != "Unassigned") {
							$('.note-editable').css('font-family', GetTinyMCEFontName(top.Ts.System.User.FontFamily));
						}
						else if (top.Ts.System.Organization.FontFamilyDescription != "Unassigned") {
							$('.note-editable').css('font-family', GetTinyMCEFontName(top.Ts.System.Organization.FontFamily));
						}

						if (top.Ts.System.User.FontSize != "0") {
							$('.note-editable').css('font-size', GetTinyMCEFontSize(top.Ts.System.User.FontSize));
						}
						else if (top.Ts.System.Organization.FontSize != "0") {
							$('.note-editable').css('font-size', GetTinyMCEFontSize(top.Ts.System.Organization.FontSize));
						}
					});
					callback();
				},
				onImageUpload: function (files, editor, welEditable) {
					sendFile(files[0], editor, welEditable);
				}
			}
		});
	});

	var insertUser = function (context) {
		var ui = $.summernote.ui;
		var button = ui.button({
			contents: '<i class="fa fa-clock-o"/>',
			tooltip: 'Insert User',
			click: function () {
				context.invoke('editor.insertText', Date(Date.UTC(Date.Now)) + ' ' + top.Ts.System.User.FirstName + ' ' + top.Ts.System.User.LastName + ' : ');
			}
		});
		return button.render();
	};

	var insertDropBox = function (context) {
		var ui = $.summernote.ui;
		var button = ui.button({
			contents: '<i class="fa fa-dropbox"/>',
			tooltip: 'Insert Dropbox File',
			click: function () {
				var options = {
					linkType: "preview",
					success: function (files) {
						var html = $('<a href=' + files[0].link + '>' + files[0].name + '</a>')[0];
						context.invoke('insertNode', html);
						top.Ts.System.logAction('Ticket - Dropbox Added');
					},
					cancel: function () {
						alert('There was a problem inserting the dropbox file.');
					}
				};
				Dropbox.choose(options);
			}
		});
		return button.render();
	};

	var insertTicket = function (context) {
		var ui = $.summernote.ui;
		var button = ui.button({
			contents: '<i class="fa fa-ticket"/>',
			tooltip: 'Insert Ticket',
			click: function () {
				top.Ts.System.logAction('Ticket - Ticket Inserted');

				top.Ts.MainPage.selectTicket(null, function (ticketID) {
					top.Ts.Services.Tickets.GetTicket(ticketID, function (ticket) {
						if (_ticketID) {
							top.Ts.Services.Tickets.AddRelated(_ticketID, ticketID, null, function (tickets) {
								appendRelated(tickets);
							}, function (error) {
								alert(error.get_message());
							});
						}
						var html = $('<a href="' + top.Ts.System.AppDomain + '?TicketNumber=' + ticket.TicketNumber + '" target="_blank" onclick="top.Ts.MainPage.openTicket(' + ticket.TicketNumber + '); return false;">Ticket ' + ticket.TicketNumber + '</a>')[0];
						context.invoke('insertNode', html);
					}, function () {
						alert('There was a problem inserting the ticket link.');
					});
				});
			}
		});
		return button.render();
	};

	var insertVideo = function (context) {
		var ui = $.summernote.ui;
		var button = ui.button({
			contents: '<i class="fa fa-video-camera"/>',
			tooltip: 'Record Video',
			click: function () {
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
		return button.render();
	};

	var insertKB = function (context) {
		var ui = $.summernote.ui;
		var button = ui.button({
			contents: '<i class="fa fa-book"/>',
			tooltip: 'Suggested Solutions',
			click: function () {
			    suggestedSolutions(element.SuggestedSolutionDefaultInput, function (ticketID, isArticle) {
			        if (isArticle) {
			            top.Ts.Services.Tickets.GetKBTicketAndActions(ticketID, function (result) {
			                if (result === null) {
			                    alert('There was an error inserting your suggested solution ticket.');
			                    return;
			                }
			                var ticket = result[0];
			                var actions = result[1];

			                _insertedKBTicketID = ticket.TicketID;
			                var html = '<div>';

			                if (actions.length == 0) {
			                    alert('The selected ticket has no knowledgebase actions.');
			                }

			                for (var i = 0; i < actions.length; i++) {
			                    html = html + '<div>' + actions[i].Description + '</div></br>';
			                }
			                html = html + '</div>';

			                context.invoke('insertNode', $(html)[0])
			                top.Ts.System.logAction('Ticket - Suggested Solution Inserted');
			            }, function () {
			                alert('There was an error inserting your suggested solution ticket.');
			            });
			        }
			        else {
			            top.Ts.Services.Admin.GetHubURLwithCName(function (url) {
			                var link = "https://" + url + "/knowledgeBase/" + ticketID;
			                var html = $('<a href="' + link + '" target="_blank">' + link + '</a></br>')[0];
			                context.invoke('insertNode', html);
			                top.Ts.System.logAction('Ticket - Suggested Solution Link Inserted');
			            });

			        }
				});

				//top.Ts.MainPage.selectTicket(filter, function (ticketID) {
				//	top.Ts.Services.Tickets.GetKBTicketAndActions(ticketID, function (result) {
				//		if (result === null) {
				//			alert('There was an error inserting your knowledgebase ticket.');
				//			return;
				//		}
				//		var ticket = result[0];
				//		var actions = result[1];

				//		var html = '<div>';

				//		for (var i = 0; i < actions.length; i++) {
				//			html = html + '<div>' + actions[i].Description + '</div></br>';
				//		}
				//		html = html + '</div>';

				//		context.invoke('insertNode', $(html)[0])
				//		top.Ts.System.logAction('Ticket - KB Inserted');
				//	}, function () {
				//		alert('There was an error inserting your knowledgebase ticket.');
				//	});
				//});
			}
		});
		return button.render();
	};

	var insertPastedImage = function (context) {
		var ui = $.summernote.ui;
		var button = ui.button({
			contents: '<i class="fa fa-paste"/>',
			tooltip: 'Insert Knowledgebase',
			click: function () {
				if (BrowserDetect.browser == 'Safari' || BrowserDetect.browser == 'Explorer' || (BrowserDetect.browser == 'Mozilla' && BrowserDetect.version < 20)) {
					alert("Sorry, this feature is not supported by your browser");
				}
				else {
					top.Ts.MainPage.pasteImage(null, function (result) {
						if (result != "") {
							var html = '<img src="' + top.Ts.System.AppDomain + '/dc/' + result + '">';
							context.invoke('insertNode', $(html)[0])
						}
					});
				}
			}
		});
		return button.render();
	};

	var insertDocuments = function (context) {
		var ui = $.summernote.ui;
		var button = ui.button({
			contents: '<i class="fa fa-image"/>',
			tooltip: 'Insert Documents',
			click: function () {
				moxman.browse({
					oninsert: function (args) {
						for (i = 0; i < args.files.length; i++)
						{
							var html = '<img src="' + args.files[i].url + '"><br/>';
							context.invoke('insertNode', $(html)[0])
						}
					}
				});

			}
		});
		return button.render();
	};

	var insertScreenRecording = function (context) {
		var ui = $.summernote.ui;
		var button = ui.button({
			contents: '<i class="fa fa-desktop"/>',
			tooltip: 'Screen Video Recording',
			click: function () {
				top.Ts.System.logAction('Ticket - Video Screen Recording Button Clicked');
				if (OT.checkSystemRequirements() == 1 || BrowserDetect.browser == "Mozilla") {
					var dynamicPub = element.parent().find("#screenShare");
					element.parent().find("#recordScreenContainer").show();
					dynamicPub.show();

					var clonedScreen = $('#ourPubTest').clone();
					clonedScreen.attr("id", "ourPubTestClone");
					$('#ourPubTest').after(clonedScreen);
					var clonedVid = $('#ourPubTest2').clone();
					clonedVid.attr("id", "ourPubTest2Clone");
					$('#ourPubTest2').after(clonedVid);

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
							element.parent().find('#rcdtokScreen').hide();
							element.parent().find('#canceltokScreen').hide();
						} else {
							// Screen sharing is available
							top.Ts.Services.Tickets.GetSessionInfo(function (resultID) {
								sessionId = resultID[0];
								token = resultID[1];
								apiKey = resultID[2];
								session = OT.initSession(apiKey, sessionId);
								var pubOptions = { publishAudio: true, publishVideo: false };
								publisher = OT.initPublisher('ourPubTest2Clone', pubOptions);

								session.connect(token, function (error) {
									// publish a stream using the camera and microphone:
									session.publish(publisher);
								});


								// Screen sharing is available. Publish the screen.
								// Create an element, but do not display it in the HTML DOM:
								var screenContainerElement = document.createElement('div');
								screenSharingPublisher = OT.initPublisher(
								  'ourPubTestClone',
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
		return button.render();
	};
};

function sendFile(file, editor, welEditable) {
	data = new FormData();
	data.append("file", file);
	$.ajax({
		data: data,
		type: "POST",
		url: '/Services/UserService.asmx/SaveTinyMCEPasteImage',
		cache: false,
		contentType: false,
		processData: false,
		success: function (filePath) {
			var html = '<img src="'+filePath.location+'">';
			$('#action-new-editor').summernote('insertNode', $(html)[0]);
		},
		error: function () {
			alert('error uploading image')
		}
	});
}

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

function suggestedSolutions(defaultInput, callback) {
    $('.dialog-select-ticket2').find('input').val('');
    $('#SuggestedSolutionsModal').modal('show');
    $('#SuggestedSolutionsModal').on('shown.bs.modal', function () {
        $("#dialog-select-ticket2-input").focus();
    })
    if (execSuggestedSolutions) {
        return;
    }
    execSuggestedSolutions = true;

    $('#SuggestedSolutionsIFrame').attr('src', '/vcr/1_9_0/Pages/SuggestedSolutions.html');
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

        if ($(".dialog-select-ticket2 input").data('item')){
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