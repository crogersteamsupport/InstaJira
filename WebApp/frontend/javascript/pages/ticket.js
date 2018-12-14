var _ticketInfo = null;
var _ticketNumber = null;
var _ticketID = null;
var _ticketCreator = new Object();
var _ticketSender = null;
var _ticketCurrStatus = null;
var _ticketCurrUser = null;
var _plugins = null;
var _completeCommentTaskID = 0;

var _ticketGroupID = null;
var _ticketGroupUsers = null;
var _ticketTypeID = null;
var _parentFields = [];
var _productFamilyID = null;

var _isNewActionPrivate = true;
var _newAction = null;
var _oldActionID = null;

var _timeLine = new Object();
var _currDateSpan = null;
var _compiledActionTemplate = null;
var _actionTotal = 0;
var _workingActionNumer = 0;
var _isLoading = false;
var _isCreatingAction = false;
var dateformat;
var editorInit = false;
var _suggestedSolutionDefaultInput = '';

var _timerid;
var _timerElapsed = 0;
var _insertedKBTicketID = null;
var speed = 50,
    counter = 0,
    start;
var reminderClose = false;
var userFullName = window.parent.Ts.System.User.FirstName + " " + window.parent.Ts.System.User.LastName;

var clueTipOptions = window.parent.Ts.Utils.getClueTipOptions(null);

var execGetCustomer = null;
var execGetTags = null;
var execGetAsset = null;
var execGetUsers = null;
var execGetRelated = null;
var execSelectTicket = null;
var execGetCompany = null;

var session;
var token;
var recordingID;
var apiKey;
var sessionId;
var tokurl;
var publisher;
var screenSharingPublisher;
var videoURL;
var tokTimer;
var pinning;

var slaCheckTimer;
var ticketWidget = null;

var getTicketCustomers = function(request, response) {
    if (execGetCustomer) {
        execGetCustomer._executor.abort();
    }
    execGetCustomer = window.parent.Ts.Services.TicketPage.GetUserOrOrganizationForTicket(request, function(result) {
        response(result);
    });
}

var getTags = function(request, response) {
    if (execGetTags) {
        execGetTags._executor.abort();
    }
    execGetTags = window.parent.Ts.Services.Tickets.SearchTags(request.term, function(result) {
        response(result);
    });
}

var getAssets = function(request, response) {
    if (execGetAsset) {
        execGetAsset._executor.abort();
    }
    execGetAsset = window.parent.Ts.Services.Assets.FindAsset(request, function(result) {
        response(result);
    });
}

var getUsers = function(request, response) {
    if (execGetUsers) {
        execGetUsers._executor.abort();
    }
    execGetUsers = window.parent.Ts.Services.TicketPage.SearchUsers(request, function(result) {
        response(result);
    });
}

var getRelated = function(request, response) {
    if (execGetRelated) {
        execGetRelated._executor.abort();
    }
    execGetRelated = window.parent.Ts.Services.Tickets.SearchTickets(request, null, function(result) {
        response(result);
    });
}

var getCompany = function(request, response) {
    if (execGetCompany) {
        execGetCompany._executor.abort();
    }
    execGetCompany = window.parent.Ts.Services.Organizations.WCSearchOrganization(request, function(result) {
        response(result);
    });
}

var selectTicket = function(request, response) {
    if (execSelectTicket) {
        execSelectTicket._executor.abort();
    }
    var filter = $(this.element).data('filter');
    if (filter === undefined) {
        execSelectTicket = window.parent.Ts.Services.Tickets.SearchTickets(request.term, null, function(result) {
            response(result);
        });
    } else {
        execSelectTicket = window.parent.Ts.Services.Tickets.SearchTickets(request.term, filter, function(result) {
            response(result);
        });
    }
}

var ellipseString = function(text, max) {
    return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text;
};

var isFormValidToClose = function(isClosed, callback) {
    var result = true;
    if (isClosed) {
        $('.isRequiredToClose.isEmpty').addClass('hasCloseError');
        if ($('.hasCloseError').length > 0) {
            result = false;
        }
    }
    callback(result);
}

var isFormValid = function(callback) {
    window.parent.Ts.Settings.Organization.read('RequireNewTicketCustomer', false, function(requireNewTicketCustomer) {
        var result = true;

        if ($('.hasError').length > 0) {
            result = false;
        }

        //If custom required check if the ticket is a KB if not then see if we have at least one customer
        if (requireNewTicketCustomer == "True" && $('#ticket-isKB').is(":checked") == false) {
            if ($('#ticket-Customer > div.tag-item').length < 1) {
                $('#ticket-Customer').closest('.form-horizontal').addClass('hasError');
                result = false;
            } else {
                $('#ticket-Customer').closest('.form-horizontal').removeClass('hasError');
            }
        }

        if (callback) callback(result);
    });
}

Selectize.define('sticky_placeholder', function(options) {
    var self = this;

    self.updatePlaceholder = (function() {
        var original = self.updatePlaceholder;
        return function() {
            original.apply(this, arguments);
            if (!this.settings.placeholder) return;
            var $input = this.$control_input;
            $input.attr('placeholder', this.settings.placeholder);
        }
    })();

});

Selectize.define('no_results', function(options) {
    var self = this;

    options = $.extend({
            message: 'No results found.',
            html: function(data) {
                return ('<div class="selectize-dropdown ' + data.classNames + ' dropdown-empty-message">' + '<div class="selectize-dropdown-content" style="padding: 3px 12px">' + data.message + '</div>' + '</div>');
            }
        }, options);

    self.displayEmptyResultsMessage = function() {
        this.$empty_results_container.css('top', this.$control.outerHeight());
        this.$empty_results_container.show();
    };

    self.refreshOptions = (function() {
        var original = self.refreshOptions;

        return function() {
            original.apply(self, arguments);
            this.hasOptions ? this.$empty_results_container.hide() : this.displayEmptyResultsMessage();
        }
    })();

    self.onKeyDown = (function() {
        var original = self.onKeyDown;

        return function(e) {
            original.apply(self, arguments);
            if (e.keyCode === 27) {
                this.$empty_results_container.hide();
            }
        }
    })();

    self.onBlur = (function() {
        var original = self.onBlur;

        return function() {
            original.apply(self, arguments);
            this.$empty_results_container.hide();
        };
    })();

    self.setup = (function() {
        var original = self.setup;
        return function() {
            original.apply(self, arguments);
            self.$empty_results_container = $(options.html($.extend({
                classNames: self.$input.attr('class')
            }, options)));
            self.$empty_results_container.insertBefore(self.$dropdown);
            self.$empty_results_container.hide();
        };
    })();
});

$(window).load(function(e) {
    $('textarea.autogrow').autogrow();
});

$(window).resize(function(e) {
    pagewidth();
    ticketmenu();
});

$(document).ready(function() {
    pagewidth();
    ticketmenu();
    _ticketNumber = window.parent.Ts.Utils.getQueryValue("TicketNumber", window);
    apiKey = "45228242";
    //Setup Ticket Elements
    SetupTicketPage();
    //Create Handlebar Helpers
    CreateHandleBarHelpers();
    //Create Dom Events
    CreateTicketToolbarDomEvents();
    CreateTimeLineDelegates();
    //Setup WC Area
    SetupWCArea();
    SetupPusher();
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.async = true;
    script.src = ('https:' === document.location.protocol ? 'https://' : 'http://') + 'www.dropbox.com/static/api/1/dropbox.js';
    var firstScript = document.getElementsByTagName('script')[0];
    script.setAttribute('data-app-key', 'ebdoql1dhyy7l72');
    script.setAttribute('id', 'dropboxjs');
    if (window.parent.Ts.System.User.OrganizationID != 1150007) {
        firstScript.parentNode.insertBefore(script, firstScript);
    }
    slaCheckTimer = setInterval(RefreshSlaDisplay, 5000);
    $('.wcTooltip').tipTip({
        defaultPosition: "top",
        edgeOffset: 7,
        keepAlive: true
    });

    $('#btnTaskCompleteComment').on('click', function(e) {
        e.preventDefault();
        if ($('#taskCompleteComment').val() == '') {
            alert('Please type your comments before clicking on the Yes button.');
        } else {
            window.parent.parent.Ts.System.logAction('Task - Add Task Complete Comment');
            window.parent.parent.Ts.Services.Task.AddTaskCompleteComment(_completeCommentTaskID, $('#taskCompleteComment').val(), function(result) {
                if (result.Value) {
                    $('#taskCompleteComment').val('');
                    $('#modalTaskComment').modal('hide');
                } else {
                    alert('There was an error saving your comment. Please try again.')
                }
            });
        }
    });
    $('textarea.autogrow').autogrow();
});

var loadTicket = function(ticketNumber, refresh) {
    window.parent.Ts.Services.Tickets.GetTicketInfo(_ticketNumber, function(info) {
        _ticketInfo = info;
        _ticketID = info.Ticket.TicketID;
        top.Ts.TicketViewing = _ticketNumber;
        window.parent.Ts.Services.Tickets.GetTicketLastSender(_ticketID, function(result) {
            if (result !== null) {
                _ticketSender = new Object();
                _ticketSender.UserID = result.UserID;
                _ticketSender.Name = result.FirstName + ' ' + result.LastName;
            }
        });
        _ticketCreator = new Object();
        _ticketCreator.UserID = info.Ticket.CreatorID;
        _ticketCreator.Name = info.Ticket.CreatorName;
        _productFamilyID = info.Ticket.ProductFamilyID;
        _ticketTypeID = _ticketInfo.Ticket.TicketTypeID;
        _ticketCurrStatus = _ticketInfo.Ticket.TicketStatusID;
        _ticketCurrUser = _ticketInfo.Ticket.UserID;
        _ticketGroupID = info.Ticket.GroupID;

        $('#ticket-title-label').text($.trim(_ticketInfo.Ticket.Name) === '' ? '[Untitled Ticket]' : $.trim(_ticketInfo.Ticket.Name));
        $('#ticket-number').text('Ticket #' + _ticketInfo.Ticket.TicketNumber);
        $('#ticket-number-strip').text('#' + _ticketInfo.Ticket.TicketNumber);
        if (refresh != 0) {
            window.parent.Ts.Services.Customers.LoadAllTicketAlerts(_ticketID, function(notes) {
                for (var i = 0; i < notes.length; i++) {
                    var note = notes[i];
                    LoadTicketNotes(note);
                }
            });
        }

        $('#ticket-status-label').toggleClass('ticket-closed', _ticketInfo.Ticket.IsClosed);
        $('#ticket-visible').prop("checked", _ticketInfo.Ticket.IsVisibleOnPortal);
        $('#ticket-isKB').prop("checked", _ticketInfo.Ticket.IsKnowledgeBase);
        $('#ticket-KB-Category-RO').text(_ticketInfo.Ticket.KnowledgeBaseCategoryName);
        if (_ticketInfo.Ticket.KnowledgeBaseCategoryID != null) {
            SetKBCategory(_ticketInfo.Ticket.KnowledgeBaseCategoryID);
        }
        SetCommunityCategory(_ticketInfo.Ticket.ForumCategory);
        SetDueDate(_ticketInfo.Ticket.DueDate);
        SetProduct(_ticketInfo.Ticket.ProductID);
        SetVersion(_ticketInfo.Ticket.ReportedVersionID);
        SetSolved(_ticketInfo.Ticket.SolvedVersionID);
        SetAssignedUser(_ticketInfo.Ticket.UserID);
        SetGroup(_ticketInfo.Ticket.GroupID);
        SetType(_ticketInfo.Ticket.TicketTypeID);
        SetStatus(_ticketInfo.Ticket.TicketStatusID);
        SetSeverity(_ticketInfo.Ticket.TicketSeverityID);
        CreateNewAction(_ticketInfo.Actions)
        setSLAInfo();
        AddCustomers(_ticketInfo.Customers);
        AddAssociatedTickets(_ticketInfo.Related);
        AddTags(_ticketInfo.Tags);
        AppenCustomValues(_ticketInfo.CustomValues);
        AddSubscribers(_ticketInfo.Subscribers);
        AddQueues(_ticketInfo.Queuers);
        AddReminders(_ticketInfo.Reminders);
        AddTasks(_ticketInfo.Tasks);
        AddInventory(_ticketInfo.Assets);
        LoadTicketHistory();
        SetupJiraFieldValues();
        SetupTFSFieldValues();
        SetupSnowFieldValues();
        LoadGroups();
        LoadPlugins(info);

    });
};

function LoadPlugins(info) {
    ticketWidget = new TicketWidget(info.Ticket);

    if (info.Plugins) {
        for (var i = 0; i < info.Plugins.length; i++) {
            var plugin = $('#ticket-group-plugin-' + info.Plugins[i].PluginID);
            if (plugin.length > 0) {
                try {
                    plugin.html(info.Plugins[i].Code);
                } catch (e) {}
            }
        }
    }
}

function CreateNewAction(actions) {
    var firstAction = $(".action[data-iswc='false']").first();

    var firstActionID = firstAction.data('id');
    if (firstActionID !== actions[0].Action.ActionID) {
        window.parent.Ts.Services.TicketPage.ConvertActionItem(actions[0].Action.ActionID, function(actionInfo) {
            var actionElement = CreateActionElement(actionInfo, false);
            _actionTotal++;
            actionElement.find('.ticket-action-number').text(_actionTotal);
        });
    }
}

function SetupTicketPage() {
    CreateNewActionLI();
    $('textarea.autogrow').autogrow();
    window.parent.Ts.Services.TicketPage.GetTicketPageOrder("TicketFieldsOrder", function(order) {
        SetupTicketProperties(order);
    });
    $('#NewCustomerModal').on('shown.bs.modal', function() {
        if ((top.Ts.System.User.CanCreateContact) || top.Ts.System.User.IsSystemAdmin) {
            return;
        } else {
            $('#customer-email-input').prop("disabled", true);
            $('#customer-fname-input').prop("disabled", true);
            $('#customer-lname-input').prop("disabled", true);
            $('#customer-phone-input').prop("disabled", true);
        }
    })
    window.parent.Ts.Services.Customers.GetDateFormat(false, function(format) {
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
}

function AddTicketProperty(item) {
    if (item.ItemID) {
        var html = '<div class="ticket-plugin" id="ticket-group-plugin-' + item.ItemID + '"></div>';
        $('#ticket-properties-area').append(html);
    } else {
        var hbrs = "ticket-group-" + item.CatID;
        var hbrs = hbrs.toLowerCase();
        var compiledTemplate = Handlebars.templates[hbrs];
        if (item.CatID == "Attachments") {
            var context = {
                Attachments: _ticketInfo.Attachments
            };
            var html = compiledTemplate(context);
            $('#ticket-properties-area').append(html);
        } else {
            $('#ticket-properties-area').append(compiledTemplate);
        }
    }
};

function SetupTicketProperties(order) {
    window.parent.Ts.Services.TicketPage.GetTicketInfo(_ticketNumber, function(info) {
        if (info == null) {
            var url = window.location.href;
            if (url.indexOf('.') > -1) {
                url = url.substring(0, url.lastIndexOf('/') + 1);
            }
            window.location = url + 'NoTicketAccess.html';
            return;
        } else if (info.faults.length > 0) {
            invalidStatus(info.Ticket.TicketStatusID);
        }
        _ticketInfo = info;
        _ticketID = info.Ticket.TicketID;
        window.parent.Ts.Services.Tickets.GetTicketLastSender(_ticketID, function(result) {
            if (result !== null) {
                _ticketSender = new Object();
                _ticketSender.UserID = result.UserID;
                _ticketSender.Name = result.FirstName + ' ' + result.LastName;
            }
        });
        _ticketCreator = new Object();
        _ticketCreator.UserID = info.Ticket.CreatorID;
        _ticketCreator.Name = info.Ticket.CreatorName;
        _productFamilyID = info.Ticket.ProductFamilyID;
        _ticketGroupID = info.Ticket.GroupID;

        window.parent.Ts.System.logAction('View Ticket');

        if (info == null) alert('no ticket');

        jQuery.each(order, function(i, val) {
            if (val.Disabled == "false") {
                AddTicketProperty(val);
            }
        });

        if (!window.parent.Ts.System.User.ChangeKbVisibility && !window.parent.Ts.System.User.IsSystemAdmin) {
            $('#action-new-KB').prop('disabled', true);
        }

        if (window.parent.Ts.System.User.IsSystemAdmin || window.parent.Ts.System.User.UserID === _ticketInfo.UserID) {
            $('.ticket-menu-actions').append('<li><a id="Ticket-Delete">Delete</a></li>');
            $('#Ticket-Delete').click(function(e) {
                e.preventDefault();
                e.stopPropagation();
                if (confirm('Are you sure you would like to delete this ticket?')) {
                    window.parent.Ts.System.logAction('Ticket - Deleted');
                    window.parent.Ts.Services.Tickets.DeleteTicket(_ticketID, function() {
                        window.parent.Ts.MainPage.closeTicketTab(_ticketNumber);
                        window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "delete", userFullName);
                    }, function() {
                        alert('There was an error deleting this ticket.');
                    });
                }
            });
        };

        if (!window.parent.Ts.System.User.IsSystemAdmin && !window.parent.Ts.System.User.ChangeTicketVisibility) {
            $('#ticket-visible').prop('disabled', true);
        }

        if (!window.parent.Ts.System.User.IsSystemAdmin && _ticketInfo.Ticket.IsKnowledgeBase && !window.parent.Ts.System.User.ChangeKbVisibility) {
            $('#ticket-visible').prop('disabled', true);
        }

        //set the url for the copy paste button
        //var ticketURLLink = ""
        var ticketUrl = window.parent.Ts.System.AppDomain + "/?TicketNumber=" + _ticketNumber;
        $("#clipboard").attr("data-copy", ticketUrl);

        //set the ticket title
        $('#ticket-title-label').text($.trim(_ticketInfo.Ticket.Name) === '' ? '[Untitled Ticket]' : $.trim(_ticketInfo.Ticket.Name));
        $('#ticket-number').text('Ticket #' + _ticketInfo.Ticket.TicketNumber);
        $('#ticket-number-strip').text('#' + _ticketInfo.Ticket.TicketNumber);
        $('.ticket-source').css('backgroundImage', "url('../" + window.parent.Ts.Utils.getTicketSourceIcon(_ticketInfo.Ticket.TicketSource) + "')").attr('title', 'Ticket Source: ' + (_ticketInfo.Ticket.TicketSource == null ? 'Agent' : _ticketInfo.Ticket.TicketSource));
        //get total number of actions so we can use it to number each action
        GetActionCount(function() {
            //create timeline now that we have a ticketID and a count
            FetchPinned();
            FetchTimeLineItems(0);
        });

        //action timers
        SetupActionTimers();

        //Setup ToolTips
        SetupToolTips();
        //update ticket property controls with the values needed
        LoadTicketControls();
        //Get Ticket Notes for Customers associated with ticket
        window.parent.Ts.Services.Customers.LoadAllTicketAlerts(_ticketID, function(notes) {
            for (var i = 0; i < notes.length; i++) {
                var note = notes[i];
                LoadTicketNotes(note);
            }
        });

        $('#frame-container').show();
        $('.page-loading').hide().next().show();

        isFormValid();
        LoadPlugins(info);
    });
}

function SetupToolTips() {
    $('#Ticket-Subscribe').attr('data-original-title', (_ticketInfo.Ticket.IsSubscribed) ? 'UnSubscribe to Ticket' : 'Subscribe to Ticket');
    $('#Ticket-Queue').attr('data-original-title', (_ticketInfo.Ticket.IsEnqueued) ? 'Remove from your Ticket Queue' : 'Add to your Ticket Queue');
    $('#Ticket-Flag').attr('data-original-title', (_ticketInfo.Ticket.IsFlagged) ? 'UnFlag Ticket' : 'Flag Ticket');
    $('.btn-group [data-toggle="tooltip"]').tooltip({
        placement: 'bottom',
        container: '.ticket-toolbar-row',
        animation: false
    });
    $('.btn-tooltip').tooltip({
        placement: 'bottom',
        animation: false
    });
}

function CreateNewActionLI() {
    var _compiledNewActionTemplate = Handlebars.templates['newaction'];
    var html = _compiledNewActionTemplate({
        OrganizationID: window.parent.Ts.System.User.OrganizationID,
        UserID: window.parent.Ts.System.User.UserID
    });
    $("#action-timeline").append(html);

    $('#action-add-public, #action-add-public-sm').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        if ($(this).hasClass('click-disabled')) {
            return false;
        } else if (window.parent.Ts.System.User.DisablePublic && !top.Ts.System.User.IsSystemAdmin) {
            alert('Your account cannot post public actions.');
            return false;
        } else {
            $(this).addClass('click-disabled');
            $("a.action-option-edit").each(function() {
                $(this).addClass('click-disabled');
            });
        }
        var editor = $('#action-new-editor');
        if (_suggestedSolutionDefaultInput == '') {
            window.parent.Ts.Services.TicketPage.GetSuggestedSolutionDefaultInput(_ticketID, function(result) {
                _suggestedSolutionDefaultInput = result;
                if (_suggestedSolutionDefaultInput != '') {
                    editor.SuggestedSolutionDefaultInput = _suggestedSolutionDefaultInput;
                }
                SetupActionEditor(editor);
                SetupActionTypeSelect();
                FlipNewActionBadge(false);
                _isNewActionPrivate = false;
                $('#action-new-KB').prop('checked', false);
                $('#action-save-alert').text('').hide();
            });
        } else {
            editor.SuggestedSolutionDefaultInput = _suggestedSolutionDefaultInput;
            SetupActionEditor(editor);
            SetupActionTypeSelect();
            FlipNewActionBadge(false);
            _isNewActionPrivate = false;
            $('#action-new-KB').prop('checked', false);
            $('#action-save-alert').text('').hide();
        }
    });

    $('#action-add-private, #action-add-private-sm').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        if ($(this).hasClass('click-disabled')) {
            return false;
        } else {
            $(this).addClass('click-disabled');
            $("a.action-option-edit").each(function() {
                $(this).addClass('click-disabled');
            });
        }
        var editor = $('#action-new-editor');
        SetupActionEditor(editor);
        SetupActionTypeSelect();
        FlipNewActionBadge(true);
        _isNewActionPrivate = true;
        $('#action-new-KB').prop('checked', false);
        $('#action-save-alert').text('').hide();
    });

    $('#action-add-wc, #action-add-wc-sm').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        $('#newcomment').prop('disabled', false);
        $('#inputDescription').val("");
        $('#associationQueue').find('.upload-queue').empty();
        $('#associationQueue').find('.ticket-queue').empty();
        $('#associationQueue').find('.group-queue').empty();
        $('#associationQueue').find('.customer-queue').empty();
        $('#associationQueue').find('.user-queue').empty();
        $('#associationQueue').find('.product-queue').empty();
        $('.watercooler-new-area').show();
    });

    $('#action-new-cancel').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        $('#action-new-editor').parent().fadeOut('normal', function() {
            tinymce.activeEditor.destroy();
        });
        window.parent.Ts.MainPage.highlightTicketTab(_ticketNumber, false);
        $('#action-add-public').removeClass('click-disabled');
        $('#action-add-private').removeClass('click-disabled');
        $("a.action-option-edit").each(function() {
            $(this).removeClass('click-disabled');
        });
        $('#recorder').remove();
    });

    $('#action-timeline').on('click', '#newcommentcancel', function(e) {
        $('.watercooler-new-area').fadeOut('normal');
    });

    $('#action-new-save').click(function(e) {
        if ($("#recorder").length == 0) {
            e.preventDefault();
            e.stopPropagation();
            DisableCreateBtns();
            $('#action-add-public').removeClass('click-disabled');
            $('#action-add-private').removeClass('click-disabled');
            $("a.action-option-edit").each(function() {
                $(this).removeClass('click-disabled');
            });
            var self = $(this);
            _oldActionID = self.data('actionid');
            isFormValid(function(isValid) {
                if (isValid) {
                    window.parent.Ts.Services.TicketPage.CheckContactEmails(_ticketID, function(result) {
                        if (!result && window.parent.Ts.System.Organization.AlertContactNoEmail)
                            alert("At least one of the contacts associated with this ticket does not have an email address defined or is inactive, and will not receive any emails about this ticket.");
                        SaveAction(_oldActionID, _isNewActionPrivate, function(result) {
                            if (result) {
                                _isCreatingAction = true;
                                if ($('.upload-queue li').length > 0) {
                                    UploadAttachments(result);
                                } else {
                                    _newAction = null;
                                    if (_oldActionID === -1) {
                                        _actionTotal = _actionTotal + 1;
                                        var actionElement = CreateActionElement(result, false);
                                        actionElement.find('.ticket-action-number').text(_actionTotal);
                                    } else {
                                        UpdateActionElement(result, false);
                                    }
                                    clearTicketEditor();
                                }
                            } else {
                                alert("There was a error creating your action.  Please try again.");
                                EnableCreateBtns();
                            }
                        });
                    });
                } else {
                    alert("Please fill in the required fields before submitting this action.");
                    EnableCreateBtns();
                    return;
                }
            });
        }
        //EnableCreateBtns();
    });

    $('#action-timeline').on('click', '.action-create-option', function(e) {
        e.preventDefault();
        e.stopPropagation();
        DisableCreateBtns();
        $('#action-add-public').removeClass('click-disabled');
        $('#action-add-private').removeClass('click-disabled');
        $("a.action-option-edit").each(function() {
            $(this).removeClass('click-disabled');
        });
        var self = $(this);
        var _oldActionID = self.data('actionid');
        isFormValid(function(isValid) {
            if (isValid) {
                SaveAction(_oldActionID, _isNewActionPrivate, function(result) {
                    if (result) {
                        if ($('.upload-queue li').length > 0) {
                            UploadAttachments(result);
                        } else {
                            _newAction = null;
                            if (_oldActionID === -1) {
                                _actionTotal = _actionTotal + 1;
                                var actionElement = CreateActionElement(result, false);
                                actionElement.find('.ticket-action-number').text(_actionTotal);
                            } else {
                                UpdateActionElement(result, false);
                            }
                            clearTicketEditor();
                        }
                        var statusID = self.data("statusid");
                        SetStatus(statusID);
                        EnableCreateBtns();
                    } else {
                        EnableCreateBtns();
                        alert("There was a error creating your action.  Please try again.");
                    }
                });
            } else {
                EnableCreateBtns();
                alert("Please fill in the required fields before submitting this action.");
                return;
            }
        });
    });

    $('#action-timeline').on('click', '.remove-attachment', function(e) {
        e.preventDefault();
        e.stopPropagation();
        var self = $(this);
        var attachmentid = self.data('attachmentid');
        var filename = self.data('name');

        if (!confirm('Are you sure you would like to delete "' + filename + '."')) return;
        window.parent.Ts.Services.Tickets.DeleteAttachment(
            attachmentid,
            function() {
                self.prev().remove();
                self.remove();
            },
            function() {
                alert('There was a problem deleting "' + attachment.FileName + '."');
            });
    });

    $('#action-new-type').change(function(e) {
        var actionID = $(this).val();
        var action = $(this).find(':selected').data('data');
        HideActionTimer(!action.IsTimed);
        window.parent.Ts.Services.TicketPage.GetActionTicketTemplate(actionID, function(result) {
            if (result != null && result != "" && result != "<br>") {
                var currenttext = tinyMCE.activeEditor.getContent();
                tinyMCE.activeEditor.setContent(currenttext + result);
            }
        });
    });

    $('#new-action-avatar').attr("src", $('#new-action-avatar').attr("src") + "/" + (new Date().getTime()).toString());
}

function DisableCreateBtns() {
    if ($('.action-save-group').hasClass('open')) {
        $('#action-new-save-element').dropdown('toggle');
    }
    $('#action-new-save').prop('disabled', true);
    $('#action-new-save-element').prop('disabled', true);
}

function EnableCreateBtns() {
    $('#action-new-save').prop('disabled', false);
    $('#action-new-save-element').prop('disabled', false);
}

function SetupActionEditor(elem, action) {
    $('button.wc-textarea-send').prop('disabled', false);
    $('#newcomment').prop('disabled', false);
    EnableCreateBtns();
    $('.watercooler-new-area').hide();
    if (action) {
        $('#action-new-date-started').val(moment(action.DateStarted).format(dateFormat + ' hh:mm A'));
    } else {
        if ($('#action-new-date-started').data("DateTimePicker")) {
            $('#action-new-date-started').data("DateTimePicker").destroy();
        }
        $('#action-new-date-started').datetimepicker({
            format: dateFormat + ' hh:mm A',
            defaultDate: new Date()
        });
    }

    window.parent.Ts.MainPage.highlightTicketTab(_ticketNumber, true);

    initEditor(elem, true, function(ed) {
        $("#action-new-type").val($("#action-new-type option:first").val());
        $('#action-new-editor').val('');
        if (action) {
            $('#action-new-type').val(action.ActionTypeID);
            if (action.TimeSpent) {
                $('#action-new-hours').val(Math.floor(action.TimeSpent / 60));
                $('#action-new-minutes').val(Math.floor(action.TimeSpent % 60));
            }
            tinyMCE.activeEditor.setContent(action.Message);
        } else {
            var actionTypeID = $('#action-new-type').val();
            $('#action-new-hours').val(0);
            $('#action-new-minutes').val(0);
            window.parent.Ts.Services.TicketPage.GetActionTicketTemplate(actionTypeID, function(result) {
                if (result != null && result != "" && result != "<br>") {
                    var currenttext = tinyMCE.activeEditor.getContent();
                    tinyMCE.activeEditor.setContent(currenttext + result);
                }
            });
        }
        elem.parent().fadeIn('normal');

        $('.ticketpage').animate({
            scrollTop: 0
        }, 600);
    });

    var element = $('.action-new-area');
    $('#action-file-upload').fileupload({
        namespace: 'new_action',
        dropZone: element,
        add: function(e, data) {
            for (var i = 0; i < data.files.length; i++) {
                var item = $('<li>').appendTo(element.find('.upload-queue'));
                data.context = item;
                item.data('data', data);
                var bg = $('<div>').appendTo(item);
                $('<div>').text(data.files[i].name + '  (' + window.parent.Ts.Utils.getSizeString(data.files[i].size) + ')').addClass('filename').appendTo(bg);
                $('<div>').addClass('progress').hide().appendTo(bg);
                $('<span>').addClass('ui-icon ui-icon-close').click(function(e) {
                    e.preventDefault();
                    $(this).closest('li').fadeOut(500, function() {
                        $(this).remove();
                    });
                }).appendTo(bg);
                //<span class="tagRemove" aria-hidden="true">×</span>
                $('<span>').addClass('ui-icon ui-icon-cancel').hide().click(function(e) {
                    e.preventDefault();
                    var data = $(this).closest('li').data('data');
                    data.jqXHR.abort();
                }).appendTo(bg);
                if ((data.files[i].size / 1000000) > 25) {
                    alert("Warning " + data.files[i].name + " is over 25MB");
                }
            }

        },
        send: function(e, data) {
            if (data.context && data.dataType && data.dataType.substr(0, 6) === 'iframe') {
                data.context.find('.progress').progressbar('value', 50);
            }
        },
        fail: function(e, data) {
            if (data.errorThrown === 'abort') return;
            alert('There was an error uploading "' + data.files[0].name + '".');
            callback(null);
        },
        progress: function(e, data) {
            data.context.find('.progress').progressbar('value', parseInt(data.loaded / data.total * 100, 10));
        },
        start: function(e, data) {
            element.find('.progress').progressbar().show();
            element.find('.upload-queue .ui-icon-close').hide();
            element.find('.upload-queue .ui-icon-cancel').show();
        },
        stop: function(e, data) {
            window.parent.Ts.Services.TicketPage.GetActionAttachments(_newAction.item.RefID, function(attachments) {
                _newAction.Attachments = attachments;
                clearTicketEditor();
                if (_oldActionID === -1) {
                    _actionTotal = _actionTotal + 1;
                    var actionElement = CreateActionElement(_newAction, false);
                    actionElement.find('.ticket-action-number').text(_actionTotal);
                } else {
                    UpdateActionElement(_newAction, false);
                }
                _newAction = null;
            });
        }
    });

    element.find('#recordScreenContainer').hide();
    element.find('#ssDiv').hide();
    element.find('#rcdtokScreen').click(function(e) {
        if (window.parent.Ts.System.User.OrganizationID == 0) {
            window.parent.Ts.Services.Tickets.StartArchivingScreen(sessionId, function(resultID) {
                element.find('#rcdtokScreen').hide();
                element.find('#stoptokScreen').show();
                element.find('#deletetokScreen').hide();
                recordingID = resultID;
                element.find('#tokScreenCountdown').show();
                setTimeout(function() {
                    update(element);
                }, 1000);
                element.find('#statusTextScreen').text("Currently Recording Screen...");
            });
        } else {
            window.parent.Ts.Services.Tickets.StartArchivingScreen(sessionId, function(resultID) {
                element.find('#rcdtokScreen').hide();
                element.find('#stoptokScreen').show();
                element.find('#deletetokScreen').hide();
                recordingID = resultID;
                element.find('#tokScreenCountdown').show();
                setTimeout(function() {
                    update(element);
                }, 1000);
                element.find('#statusTextScreen').text("Currently Recording Screen...");
            });
        }
    });

    element.find('#muteTokScreen').click(function(e) {
        publisher.publishAudio(false);
        element.find('#unmuteTokScreen').show();
        element.find('#muteTokScreen').hide();
    });

    element.find('#unmuteTokScreen').hide();
    element.find('#unmuteTokScreen').click(function(e) {
        publisher.publishAudio(true);
        element.find('#muteTokScreen').show();
        element.find('#unmuteTokScreen').hide();
    });

    element.find('#stoptokScreen').hide();
    element.find('#stoptokScreen').click(function(e) {
        element.find('#statusTextScreen').text("Saving video...");
        clearTimeout(tokTimer);
        $("#tokScreenCountdown").html("0:00");
        window.parent.Ts.Services.Tickets.StopArchiving(recordingID, function(result) {
            element.find('#tokScreenCountdown').hide();
            element.find('#rcdtokScreen').show();
            element.find('#stoptokScreen').hide();
            element.find('#canceltokScreen').show();
            element.find('#unmuteTokScreen').hide();
            element.find('#muteTokScreen').hide();
            tokurl = result;
            videoURL = '<video width="100%" controls poster="' + window.parent.Ts.System.AppDomain + '/dc/1078/images/static/player.jpg"><source src="' + tokurl + '" type="video/mp4"><a href="' + tokurl + '">Please click here to view the video.</a></video>';
            tinyMCE.activeEditor.execCommand('mceInsertContent', false, '<br/><br/>' + videoURL);
            element.find('#statusTextScreen').text("Your video is currently processing. It may not play in the editor below but should be live within a minute.");
            session.unpublish(screenSharingPublisher);
            session.unpublish(publisher);
            recordingID = null;
        });
    });

    element.find('#rcdtok').click(function(e) {
        window.parent.Ts.Services.Tickets.StartArchiving(sessionId, function(resultID) {
            element.find('#rcdtok').hide();
            element.find('#stoptok').show();
            element.find('#inserttok').hide();
            element.find('#deletetok').hide();
            recordingID = resultID;
            element.find('#statusText').text("Currently Recording ...");
        });
    });

    element.find('#stoptok').hide();

    element.find('#stoptok').click(function(e) {
        element.find('#statusText').text("Processing...");
        window.parent.Ts.Services.Tickets.StopArchiving(recordingID, function(result) {
            element.find('#rcdtok').show();
            element.find('#stoptok').hide();
            element.find('#inserttok').show();
            element.find('#canceltok').show();
            tokurl = result;
            element.find('#statusText').text("Recording Stopped");
        });
    });

    element.find('#inserttok').hide();

    element.find('#inserttok').click(function(e) {
        tinyMCE.activeEditor.execCommand('mceInsertContent', false, '<br/><br/><video width="400" height="400" controls poster="' + window.parent.Ts.System.AppDomain + '/dc/1078/images/static/videoview1.jpg"><source src="' + tokurl + '" type="video/mp4"><a href="' + tokurl + '">Please click here to view the video.</a></video>');
        session.unpublish(publisher);
        element.find('#rcdtok').show();
        element.find('#stoptok').hide();
        element.find('#inserttok').hide();
        element.find('#recordVideoContainer').hide();
        element.find('#statusText').text("");
    });

    element.find('#canceltokScreen').click(function(e) {
        element.find('#statusTextScreen').text("");
        clearTimeout(tokTimer);
        $("#tokScreenCountdown").html("0:00");
        if (recordingID != null) {
            window.parent.Ts.Services.Tickets.StopArchiving(recordingID, function(result) {
                element.find('#tokScreenCountdown').hide();
                element.find('#rcdtokScreen').show();
                element.find('#stoptokScreen').hide();
                element.find('#canceltokScreen').show();
                element.find('#unmuteTokScreen').hide();
                element.find('#muteTokScreen').hide();
                recordingID = null;
            });
        }
        session.unpublish(screenSharingPublisher);
        session.unpublish(publisher);
        element.find('#recordScreenContainer').hide();
    });

    element.find('#canceltok').click(function(e) {
        if (recordingID) {
            element.find('#statusText').text("Cancelling Recording ...");
            window.parent.Ts.Services.Tickets.DeleteArchive(recordingID, function(resultID) {
                element.find('#rcdtok').show();
                element.find('#stoptok').hide();
                element.find('#inserttok').hide();
                session.unpublish(publisher);
                element.find('#recordVideoContainer').hide();
                element.find('#statusText').text("");
                recordingID = null;
            });
        } else {
            session.unpublish(publisher);
            element.find('#recordVideoContainer').hide();
        }
        element.find('#statusText').text("");
    });
    element.find('#recordVideoContainer').hide();

    var statuses = window.parent.Ts.Cache.getNextStatuses(_ticketInfo.Ticket.TicketStatusID);
    $('#action-new-saveoptions').empty();
    if (action) {
        $('#action-new-save').text('Update').data('actionid', action.RefID);
        for (var i = 0; i < statuses.length; i++) {
            $('#action-new-saveoptions').append('<li><a class="action-create-option" data-actionid=' + action.RefID + ' data-statusid=' + statuses[i].TicketStatusID + ' href="#">Save and Set Status to ' + statuses[i].Name + '</a></li>');
        }
    } else {
        $('#action-new-save').text('Save').data('actionid', -1);
        for (var i = 0; i < statuses.length; i++) {
            $('#action-new-saveoptions').append('<li><a class="action-create-option" data-actionid=-1 data-statusid=' + statuses[i].TicketStatusID + ' href="#">Save and Set Status to ' + statuses[i].Name + '</a></li>');
        }
    }
}

function SetupNewAction(elem, action) {
    $("#action-new-type").val($("#action-new-type option:first").val());
    $('#action-new-editor').val('');
    if (action) {
        $('#action-new-type').val(action.ActionTypeID);
        if (action.TimeSpent) {
            $('#action-new-hours').val(Math.floor(action.TimeSpent / 60));
            $('#action-new-minutes').val(Math.floor(action.TimeSpent % 60));
        }
        elem.summernote('code', action.Message);
    } else {
        var actionTypeID = $('#action-new-type').val();
        $('#action-new-hours').val(0);
        $('#action-new-minutes').val(0);
        window.parent.Ts.Services.TicketPage.GetActionTicketTemplate(actionTypeID, function(result) {
            if (result != null && result != "" && result != "<br>") {
                var currenttext = elem.summernote('code');
                elem.summernote('code', currenttext + result);
            }
        });
    }
    elem.parent().fadeIn('normal');
    $('.ticketpage').animate({
        scrollTop: 0
    }, 600);
}

function StopRecording(element) {
    element.find('#statusTextScreen').text("Processing...");
    clearTimeout(tokTimer);
    $("#tokScreenCountdown").html("0:00");
    window.parent.Ts.Services.Tickets.StopArchiving(recordingID, function(result) {
        element.find('#statusText').text("");
        element.find('#tokScreenCountdown').hide();
        element.find('#rcdtokScreen').show();
        element.find('#stoptokScreen').hide();
        element.find('#canceltokScreen').show();
        element.find('#unmuteTokScreen').hide();
        element.find('#muteTokScreen').hide();
        tokurl = result;
        videoURL = '<video controls poster="' + window.parent.Ts.System.AppDomain + '/dc/1078/images/static/videoview1.jpg"><source src="' + tokurl + '" type="video/mp4"><a href="' + tokurl + '">Please click here to view the video.</a></video>';
        tinyMCE.activeEditor.execCommand('mceInsertContent', false, '<br/><br/>' + videoURL);
        element.find('#statusTextScreen').text("");
        session.unpublish(screenSharingPublisher);
        session.unpublish(publisher);
    });
}

function update(parentElement) {
    var myTime = $("#tokScreenCountdown").html();
    var ss = myTime.split(":");
    var dt = new Date();
    dt.setHours(0);
    dt.setMinutes(ss[0]);
    dt.setSeconds(ss[1]);
    var dt2 = new Date(dt.valueOf() + 1000);
    var temp = dt2.toTimeString().split(" ");
    var ts = temp[0].split(":");
    if (temp[0] == "05") {
        StopRecording(parentElement);
        return;
    }
    $("#tokScreenCountdown").html(ts[1] + ":" + ts[2]);
    tokTimer = setTimeout(function() {
        update(parentElement);
    }, 1000);
}

function countdown(elementName, minutes, seconds, parentElement) {
    var element, endTime, hours, mins, msLeft, time;

    function twoDigits(n) {
        return (n <= 9 ? "0" + n : n);
    }

    function updateTimer(parentElement) {
        msLeft = endTime - (+new Date);
        if (msLeft < 1000) {
            StopRecording(parentElement);
        } else {
            time = new Date(msLeft);
            hours = time.getUTCHours();
            mins = time.getUTCMinutes();
            element.innerHTML = (hours ? hours + ':' + twoDigits(mins) : mins) + ':' + twoDigits(time.getUTCSeconds());
            recordScreenTimer = setTimeout(updateTimer, time.getUTCMilliseconds() + 500);
        }
    }
    element = document.getElementById(elementName);
    endTime = (+new Date) + 1000 * (60 * minutes + seconds) + 500;
    updateTimer(parentElement);
}

function SetupActionTimers() {
    $('.spinner .btn:first-of-type').click(function() {
        var spinner = $(this).parent().prev();
        spinner.val(parseInt(spinner.val(), 10) + 1);
    });
    $('.spinner .btn:last-of-type').click(function() {
        var spinner = $(this).parent().prev();
        spinner.val(parseInt(spinner.val(), 10) - 1);
    });
    $('#action-new-timer').click(function(e) {
        var hasStarted = $(this).data('hasstarted');
        if (!hasStarted) {
            start = new Date().getTime();
            tickettimer();
            $(this).find(':first-child').css('color', 'green');
        } else {
            $(this).find(':first-child').css('color', 'red');
            clearTimeout(_timerid);
            counter = 0;
            _timerElapsed = 0;
        }
        $(this).data('hasstarted', !hasStarted);
    });
}

function SetupActionTypeSelect() {
    var selectType = $('#action-new-type');
    selectType.empty();
    var types = window.parent.Ts.Cache.getActionTypes();
    for (var i = 0; i < types.length; i++) {
        $('<option>').attr('value', types[i].ActionTypeID).text(types[i].Name).data('data', types[i]).appendTo(selectType);
    }
    HideActionTimer(!types[0].IsTimed)
};

function HideActionTimer(ShouldHide) {
    if (ShouldHide) {
        $('#action-new-minutes').closest('.form-group').hide();
        $('#action-new-hours').closest('.form-group').hide();
        $('#action-new-date-started').closest('.form-group').hide();
    } else {
        $('#action-new-minutes').closest('.form-group').show();
        $('#action-new-hours').closest('.form-group').show();
        $('#action-new-date-started').closest('.form-group').show();
    }
}

function FlipNewActionBadge(isPrivate) {
    if (isPrivate) {
        $('#private-badge').show();
        $('#public-badge').hide();
    } else {
        $('#private-badge').hide();
        $('#public-badge').show();
    }
    _isNewActionPrivate = isPrivate;
}

function convertToValidDate(val) {
    var value = '';
    if (val == "") {
        return value;
    }

    if (dateFormat.indexOf("M") != 0) {
        var dateArr = val.replace(/\./g, '/').replace(/-/g, '/').split('/');
        if (dateFormat.indexOf("D") == 0) {
            var day = dateArr[0];
        }
        if (dateFormat.indexOf("Y") == 0) {
            var year = dateArr[0];
        }
        if (dateFormat.indexOf("M") == 3 || dateFormat.indexOf("M") == 5) {
            var month = dateArr[1];
        }
        var timeSplit = dateArr[2].split(' ');
        if (dateFormat.indexOf("Y") == 6) {
            var year = timeSplit[0];
        } else {
            var day = timeSplit[0];
        }
        var theTime = timeSplit[1];
        var formattedDate = month + "/" + day + "/" + year + " " + theTime + (timeSplit[2] != null ? " " + timeSplit[2] : "");
        value = window.parent.Ts.Utils.getMsDate(formattedDate);
        return value;
    } else {
        return val;
    }
}

function SaveAction(_oldActionID, isPrivate, callback) {
    var action = new parent.TeamSupport.Data.ActionProxy();
    var saveError = 0;
    action.ActionID = _oldActionID;
    action.TicketID = _ticketID;
    action.SystemActionTypeID = 0;

    var timeSpent = parseInt($('#action-new-hours').val()) * 60 + parseInt($('#action-new-minutes').val());
    var actionType = $('#action-new-type option:selected').data('data');
    if (actionType !== null) {
        if (timeSpent < 1 && actionType.IsTimed == true && window.parent.Ts.System.Organization.TimedActionsRequired == true) {
            $('#action-save-alert').text('Please enter the time you worked on this action.').show();
            EnableCreateBtns();
            return false;
        }
        action.ActionTypeID = actionType.ActionTypeID;
    }

    action.TimeSpent = timeSpent || 0;
    action.DateStarted = window.parent.Ts.Utils.getMsDate(moment($('#action-new-date-started').val(), dateFormat + ' hh:mm A').format('MM/DD/YYYY hh:mm A'));
    action.IsKnowledgeBase = $('#action-new-KB').prop('checked');
    action.IsVisibleOnPortal = !isPrivate;

    // Get Content Grab and Check with .Get MEthod
    if (tinymce.get('action-new-editor')) {
        try {
            action.Description = tinymce.get('action-new-editor').getContent();
            if (action.Description == "" || action.Description == undefined) {
                saveError = 1;
                window.parent.Ts.Services.System.LogException("TinyMCE save action contains an empty string with getContent Function. ticket " + _ticketID + ",body: " + tinymce.get('action-new-editor').getBody().innerHTML, "TinyMCE Error");
            }
        } catch (ex) {
            saveError = 2;
            window.parent.Ts.Services.System.LogException("TinyMCE save action threw exception : " + ex.message + " . ticket " + _ticketID, "TinyMCE Error");
        }

        // TINYMCE ACTIVE EDITOR CHECK
        if (saveError != 0) {
            if (tinymce.activeEditor == null) {
                saveError = 2;
                window.parent.Ts.Services.System.LogException("TinyMCE active editor is null", "TinyMCE Error");
            }
        }

        if (saveError == 1) {
            alert("The action you tried to save is empty, please try again or cancel");
            EnableCreateBtns();
            return;
        }
        if (saveError == 2) {
            alert("We’re very sorry, but there was an error saving your action.  We’ve logged this error for review, please notify support@teamsupport.com and please include the ticket number.");
            EnableCreateBtns();
            return;
        }
    } else {
        alert("We’re very sorry, but there was an error saving your action. Please copy and save your action text, refresh the ticket and try again. ");
        EnableCreateBtns();
        return;

    }

    if (action.IsVisibleOnPortal == true) {
        confirmVisibleToCustomers();
    }
    if (_insertedKBTicketID) {
        window.parent.Ts.Services.TicketPage.UpdateActionCopyingAttachment(action, _insertedKBTicketID, function(result) {
            _newAction = result;
            window.parent.Ts.MainPage.highlightTicketTab(_ticketNumber, false);
            if (actionType !== null) {
                result.item.MessageType = actionType.Name;
            }
            callback(result);
        }, function(error) {
            callback(null);
        });
    } else {
        window.parent.Ts.Services.TicketPage.UpdateAction(action, function(result) {
            _newAction = result;
            window.parent.Ts.MainPage.highlightTicketTab(_ticketNumber, false);
            if (actionType !== null) {
                result.item.MessageType = actionType.Name;
            }
            callback(result);
        }, function(error) {
            callback(null);
        });
    }
    window.parent.Ts.Services.TicketPage.GetTicketInfo(_ticketNumber, function(info) {
        _ticketInfo = info;

        if (_ticketInfo.SlaTriggerId !== null && _ticketInfo.SlaTriggerId > 0) {
            $('#ticket-SLAStatus').find('i').addClass('color-yellow');
            $('#ticket-SLANote').text('Calculating...');
            slaCheckTimer = setInterval(RefreshSlaDisplay, 5000);
        }
    });

    window.parent.Ts.System.logAction('Action Saved');
    window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "addaction", userFullName);
}

var confirmVisibleToCustomers = function() {
    var visible = $('#ticket-visible').is(":checked");
    if (!visible) {
        if (confirm('This ticket is not visible to customers.\n\nWould you like to make it visible to customers now?') == true) {
            $('#ticket-visible').click();
        }
    }
}

function UploadAttachments(newAction) {
    if ($('.upload-queue li').length > 0 && newAction !== null) {
        $('.upload-queue li').each(function(i, o) {
            var data = $(o).data('data');
            data.url = '../../../Upload/Actions/' + newAction.item.RefID;
            data.jqXHR = data.submit();
            $(o).data('data', data);
        });
    }
}

function tickettimer() {
    var real = (counter * speed),
        ideal = (new Date().getTime() - start);
    counter++;
    var diff = (ideal - real);
    if (_timerElapsed != Math.floor(ideal / 60000)) {
        var oldVal = parseInt($('#action-new-minutes').val()) || 0;
        $('#action-new-minutes').val(oldVal + 1);
        _timerElapsed = Math.floor(ideal / 60000);
    }
    _timerid = setTimeout(tickettimer, (speed - diff));
}

function LoadTicketNotes(note) {
    if (note) {
        var description = $('<div>').html(note.Description);

        var buttons = [{
                text: "Close",
                click: function() {
                    $(this).dialog("close");
                }
            },
            {
                text: "Snooze",
                click: function() {
                    _mainFrame.Ts.Services.Customers.SnoozeAlertByID($(this).data('noteId'), $(this).data('refType'));
                    $(this).dialog("close");
                }
            }
        ]

        if (!window.parent.Ts.System.Organization.HideDismissNonAdmins || window.parent.Ts.System.User.IsSystemAdmin) {
            buttons.push({
                text: "Dismiss",
                click: function() {
                    _mainFrame.Ts.Services.Customers.DismissAlertByID($(this).data('noteId'), $(this).data('refType'));
                    $(this).dialog("close");
                }
            });
        }

        var alert = $('<div>').prop('title', 'Alert message').data('noteId', note.NoteID).data('refType', note.RefType).append(description).appendTo(document.body);
        alert.dialog({
            resizable: false,
            width: 'auto',
            height: 'auto',
            modal: true,
            create: function() {
                $(this).css('max-width', '800px');
            },
            buttons: buttons
        });

    }
}

function GetActionCount(callback) {
    window.parent.Ts.Services.TicketPage.GetActionCount(_ticketID, function(total) {
        _actionTotal = total;
        _workingActionNumer = total;
        callback();
    });
}

function LoadTicketControls() {
    if (_ticketInfo.Ticket.IsFlagged) {
        $('#Ticket-Flag').children().addClass('color-red');
    }
    if (_ticketInfo.Ticket.IsEnqueued) {
        $('#Ticket-Queue').children().addClass('color-green');
    }
    if (_ticketInfo.Ticket.IsSubscribed) {
        $('#Ticket-Subscribe').children().addClass('color-green');
    }

    if ($('#ticket-assigned').length) {
        window.parent.Ts.Services.TicketPage.GetTicketUsers(_ticketID, function(users) {
            var isActive = false;
            $.each(users, function(index, item) {
                if (item.Name === _ticketInfo.Ticket.UserName) {
                    isActive = true;
                }
            });

            if (!isActive && _ticketInfo.Ticket.UserName != null) {
                $("#ticket-assigned").attr('placeholder', _ticketInfo.Ticket.UserName + ' (Inactive)');
                _ticketCurrUser = -99;
            }

            $('#ticket-assigned').selectize({
                dataAttr: 'assigned',
                onDropdownClose: function($dropdown) {
                    $($dropdown).prev().find('input').blur();
                },
                onChange: function(value) {
                    if (value == '-1') value = null;
                    if (value !== ((_ticketCurrUser !== null) ? _ticketCurrUser.toString() : _ticketCurrUser)) {
                        window.parent.Ts.Services.Tickets.SetTicketUser(_ticketID, value, function(userInfo) {
                                window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changeassigned", userFullName);
                                LoadGroups();
                            },
                            function(error) {
                                alert('There was an error setting the assigned user.');
                            });
                    }
                    _ticketSender = new Object();
                    _ticketSender.UserID = window.parent.Ts.System.User.UserID;
                    _ticketSender.Name = userFullName;
                },
                closeAfterSelect: true,
                render: {
                    option: function(item, escape) {
                        var optionlabel = item.text;
                        if (item.data.InOfficeMessage) {
                            optionlabel = optionlabel + ' - ' + item.data.InOfficeMessage;
                        }
                        if (item.data.IsSender && item.data.IsCreator) {
                            return '<div data-value="' + escape(item.value) + '" data-selectable="" class="option">' + optionlabel + ' (Sender and Creator)</div>';
                        } else if (item.data.IsSender) {
                            return '<div data-value="' + escape(item.value) + '" data-selectable="" class="option">' + optionlabel + ' (Sender)</div>';
                        } else if (item.data.IsCreator) {
                            return '<div data-value="' + escape(item.value) + '" data-selectable="" class="option">' + optionlabel + ' (Creator)</div>';
                        } else {
                            return '<div data-value="' + escape(item.value) + '" data-selectable="" class="option">' + optionlabel + '</div>';
                        }
                    }
                },
            });

            var selectize = $("#ticket-assigned")[0].selectize;
            selectize.addOption({
                value: -1,
                text: 'Unassigned',
                data: ''
            });

            for (var i = 0; i < users.length; i++) {
                selectize.addOption({
                    value: users[i].ID,
                    text: users[i].Name,
                    data: users[i]
                });
                if (users[i].IsSelected) {
                    _ticketCurrUser = users[i].ID;
                    SetAssignedUser(users[i].ID);
                }
            }
        });
    }

    if ($('#ticket-group').length) {
        window.parent.Ts.Services.TicketPage.GetTicketGroups(_ticketID, function(groups) {
            AppendSelect('#ticket-group', null, 'group', -1, 'Unassigned', false);
            if (window.parent.Ts.System.Organization.UseProductFamilies && _productFamilyID != null) {
                for (var i = 0; i < groups.length; i++) {
                    if (groups[i].ProductFamilyID == null || _productFamilyID == groups[i].ProductFamilyID || _ticketInfo.Ticket.GroupID === groups[i].ID) {
                        AppendSelect('#ticket-group', groups[i], 'group', groups[i].ID, groups[i].Name, groups[i].IsSelected);
                        if (groups[i].ProductFamilyID != null && _productFamilyID != groups[i].ProductFamilyID) {
                            alert('This ticket group belongs to a different product line. Please set the correct ticket group.');
                        }
                    }
                }
            } else {
                for (var i = 0; i < groups.length; i++) {
                    AppendSelect('#ticket-group', groups[i], 'group', groups[i].ID, groups[i].Name, groups[i].IsSelected);
                }
            }

            $('#ticket-group').selectize({
                onDropdownClose: function($dropdown) {
                    $($dropdown).prev().find('input').blur();
                },
                closeAfterSelect: true
            });

            if (window.parent.Ts.System.Organization.RequireGroupAssignmentOnTickets) {
                if ($('#ticket-group').val() == "") {
                    $('#ticket-group').closest('.form-horizontal').addClass('hasError');
                } else {
                    $('#ticket-group').closest('.form-horizontal').removeClass('hasError');
                }
            }
        });
    }

    _ticketTypeID = _ticketInfo.Ticket.TicketTypeID;
    var types = window.parent.Ts.Cache.getTicketTypes();
    if (window.parent.Ts.System.Organization.UseProductFamilies && _productFamilyID != null) {
        for (var i = 0; i < types.length; i++) {
            if ((types[i].IsActive && (types[i].ProductFamilyID == null || _productFamilyID == types[i].ProductFamilyID || _ticketInfo.Ticket.TicketTypeID === types[i].TicketTypeID)) || (!types[i].IsActive && _ticketInfo.Ticket.TicketTypeID === types[i].TicketTypeID)) {
                AppendSelect('#ticket-type', types[i], 'type', types[i].TicketTypeID, types[i].Name, (_ticketInfo.Ticket.TicketTypeID === types[i].TicketTypeID));
                if (types[i].ProductFamilyID != null && _productFamilyID != types[i].ProductFamilyID) {
                    alert('This ticket type belongs to a different product line. Please set the correct ticket type.');
                }
            }
        }
        if ($('#ticket-type')[0].childElementCount == 0) {
            alert('There are no ticket types available for this product line. Please contact your TeamSupport administrator.');
        }
    } else {
        for (var i = 0; i < types.length; i++) {
            if (types[i].IsActive || (!types[i].IsActive && types[i].TicketTypeID === _ticketTypeID)) {
                AppendSelect('#ticket-type', types[i], 'type', types[i].TicketTypeID, types[i].Name, (_ticketInfo.Ticket.TicketTypeID === types[i].TicketTypeID));
            }
        }
    }

    $('#ticket-type').selectize({
        onDropdownClose: function($dropdown) {
            $($dropdown).prev().find('input').blur();
        },
        closeAfterSelect: true
    });

    SetupStatusField(_ticketInfo.Ticket.TicketStatusID);

    $('#ticket-status-label').toggleClass('ticket-closed', _ticketInfo.Ticket.IsClosed);

    var severities = window.parent.Ts.Cache.getTicketSeverities();
    for (var i = 0; i < severities.length; i++) {
        AppendSelect('#ticket-severity', severities[i], 'severity', severities[i].TicketSeverityID, severities[i].Name, (_ticketInfo.Ticket.TicketSeverityID === severities[i].TicketSeverityID));
    }
    $('#ticket-severity').selectize({
        onDropdownClose: function($dropdown) {
            $($dropdown).prev().find('input').blur();
        },
        closeAfterSelect: true
    });

    $('#ticket-visible').prop("checked", _ticketInfo.Ticket.IsVisibleOnPortal);

    $('#ticket-isKB').prop("checked", _ticketInfo.Ticket.IsKnowledgeBase);

    if (window.parent.Ts.System.User.ChangeKbVisibility || window.parent.Ts.System.User.IsSystemAdmin) {
        if (_ticketInfo.Ticket.IsKnowledgeBase) {
            $('#ticket-isKB').prop("checked", true);
            $('#ticket-group-KBCat').show();
        } else {
            $('#ticket-isKB').prop("checked", false);
            $('#ticket-group-KBCat').hide();
            $('#ticket-KBVisible-RO').hide();
        }
        var categories = window.parent.Ts.Cache.getKnowledgeBaseCategories();
        for (var i = 0; i < categories.length; i++) {
            var cat = categories[i].Category;
            AppendSelect('#ticket-KB-Category', cat, 'category', cat.CategoryID, cat.CategoryName, (_ticketInfo.Ticket.KnowledgeBaseCategoryID === cat.CategoryID));
            for (var j = 0; j < categories[i].Subcategories.length; j++) {
                var subcat = categories[i].Subcategories[j];
                AppendSelect('#ticket-KB-Category', subcat, 'subcategory', subcat.CategoryID, cat.CategoryName + ' -> ' + subcat.CategoryName, (_ticketInfo.Ticket.KnowledgeBaseCategoryID === subcat.CategoryID));
            }
        }
        if ($('#ticket-KB-Category').length) {
            $('#ticket-KB-Category').selectize({
                onDropdownClose: function($dropdown) {
                    $($dropdown).prev().find('input').blur();
                },
                closeAfterSelect: true
            });
        }
    } else {
        $('#ticket-KBInfo').remove();
        $('#ticket-isKB-RO').text(_ticketInfo.Ticket.IsKnowledgeBase ? "True" : "False");
        $('#ticket-isKB').closest('.form-horizontal').remove();
        $('#ticket-KBVisible-RO').show();
        $('#ticket-group-KBCat').hide();
    }

    $('#ticket-TimeSpent').text(window.parent.Ts.Utils.getTimeSpentText(_ticketInfo.Ticket.HoursSpent));

    if (_ticketInfo.Ticket.IsClosed == true) {
        $('#ticket-DaysOpened').text(_ticketInfo.Ticket.DaysClosed);
        $('#label-days').text('Days Closed');
    } else {
        $('#ticket-DaysOpened').text(_ticketInfo.Ticket.DaysOpened);
        $('#label-days').text('Days Opened');
    }

    WatsonTicketField(_ticketInfo.Ticket.TicketID);
    // WatsonTicket(_ticketInfo.Ticket.TicketID);

    var dueDate = _ticketInfo.Ticket.DueDate;
    SetupDueDateField(dueDate);

    if (window.parent.Ts.System.Organization.UseForums == true) {
        if (window.parent.Ts.System.User.CanChangeCommunityVisibility) {
            var forumCategories = window.parent.Ts.Cache.getForumCategories();
            AppendSelect('#ticket-Community', null, 'community', -1, 'Unassigned', false);
            for (var i = 0; i < forumCategories.length; i++) {
                var cat = forumCategories[i].Category;
                AppendSelect('#ticket-Community', cat, 'community', cat.CategoryID, cat.CategoryName, false);

                for (var j = 0; j < forumCategories[i].Subcategories.length; j++) {
                    var subcat = forumCategories[i].Subcategories[j];
                    AppendSelect('#ticket-Community', subcat, 'subcategory', subcat.CategoryID, cat.CategoryName + ' -> ' + subcat.CategoryName, false);
                }
            }

            $('#ticket-Community').val(_ticketInfo.Ticket.ForumCategory);

            if ($('#ticket-Community').length) {
                $('#ticket-Community').selectize({
                    onDropdownClose: function($dropdown) {
                        $($dropdown).prev().find('input').blur();
                    },
                    closeAfterSelect: true
                });
            }
        } else {
            $('#ticket-CommunityInfo-RO').show();
            $('#ticket-Community-RO').text((_ticketInfo.Ticket.CategoryName == null ? 'Unassigned' : _ticketInfo.Ticket.CategoryDisplayString));
            $('#ticket-Community-RO').show();
            $('#ticket-Community').closest('.form-horizontal').remove();
        }
    } else {
        $('#ticket-Community').closest('.form-horizontal').remove();
        //$('#ticket-Community-RO').remove();
    }
    setSLAInfo();
    SetupTicketPropertyEvents();
    if (window.parent.Ts.System.Organization.ProductType == window.parent.Ts.ProductType.Express) {
        $('#ticket-Customer').closest('.form-group').remove();
    } else {
        SetupCustomerSection();
    }
    SetupTagsSection();
    if (window.parent.Ts.System.Organization.ProductType == window.parent.Ts.ProductType.Express || window.parent.Ts.System.Organization.ProductType === window.parent.Ts.ProductType.HelpDesk) {
        $('#ticket-Product').closest('.form-horizontal').remove();
        $('#ticket-Resolved').closest('.form-horizontal').remove();
        $('#ticket-Versions').closest('.form-horizontal').remove();
    } else {
        SetupProductSection();
    }
    if (window.parent.Ts.System.Organization.IsInventoryEnabled === true) {
        SetupInventorySection();
    } else {
        $('#ticket-group-assets').hide();
    }
    SetupUserQueuesSection();
    SetupSubscribedUsersSection();
    SetupAssociatedTicketsSection();
    SetupRemindersSection();
    SetupTasksSection();
    SetupCustomFieldsSection();
    SetupJiraFields();
    SetupJiraFieldValues();
    SetupTFSFields();
    SetupTFSFieldValues();
    SetupSnowFields();
    SetupSnowFieldValues();
}

function AppendSelect(parent, data, type, id, name, isSelected) {
    var option = $('<option>').val(id).text(name).appendTo(parent).data(type, data);
    if (isSelected) {
        option.attr('selected', 'selected');
    }
}

function SetupTicketPropertyEvents() {
    $('#ticket-title-label').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        var self = $(this);
        self.hide();
        var parent = self.parent();
        var input = $('#ticket-title-input');

        var titleInputContainer = $('#ticket-title-input-panel').show();
        $('#ticket-title-input').val(_ticketInfo.Ticket.Name).focus().select();

        $('#ticket-title-save').click(function(e) {
            e.preventDefault();
            e.stopPropagation();
            titleInputContainer.hide();
            window.parent.Ts.Services.Tickets.SetTicketName(_ticketID, input.val(), function(result) {
                    _ticketInfo.Ticket.Name = result;
                    window.parent.Ts.System.logAction('Ticket - Renamed');
                    self.text($.trim(_ticketInfo.Ticket.Name) === '' ? '[Untitled Ticket]' : $.trim(_ticketInfo.Ticket.Name)).show();

                    window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changeticketname", userFullName);
                },
                function(error) {
                    alert('There was an error saving the ticket name.');
                });
        });

        $('#ticket-title-cancel').click(function(e) {
            e.preventDefault();
            e.stopPropagation();
            titleInputContainer.hide();
            self.show();
        });
    });

    $('#ticket-group').change(function(e) {
        var self = $(this);
        var GroupID = self.val();
        .0
        if (GroupID == '-1') {
            GroupID = null;
            if (window.parent.Ts.System.Organization.RequireGroupAssignmentOnTickets) {
                $('#ticket-group').closest('.form-horizontal').addClass('hasError');
            }
        } else {
            $('#ticket-group').closest('.form-horizontal').removeClass('hasError');
        }
        if (GroupID !== ((_ticketGroupID !== null) ? _ticketGroupID.toString() : _ticketGroupID)) {
            window.parent.Ts.Services.Tickets.SetTicketGroup(_ticketID, GroupID, function(result) {
                    if (result !== null) {
                        window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changegroup", userFullName);
                    }
                    _ticketGroupID = GroupID;
                    if (window.parent.Ts.System.Organization.UpdateTicketChildrenGroupWithParent) {
                        window.parent.Ts.Services.Tickets.SetTicketChildrenGroup(_ticketID, GroupID);
                    }
                },
                function(error) {
                    alert('There was an error setting the group.');
                });
        }
    });

    $('#ticket-type').change(function(e) {
        var self = $(this);
        var value = self.val();
        EnableField('ticket-status', false);
        window.parent.Ts.Services.TicketPage.SetTicketType(_ticketID, value, function(result) {
                if (result !== null) {
                    _ticketTypeID = value;
                    SetStatus(result[0].TicketStatusID);
                    $('#ticket-status-label').toggleClass('ticket-closed', result[0].IsClosed);

                    AppenCustomValues(result[1]);
                    SetupJiraFieldValues();
                    SetupTFSFieldValues();
                    SetupSnowFieldValues();

                    _ticketInfo.Ticket = result[2];
                    setSLAInfo();

                    window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changetype", userFullName);
                }
            },
            function(error) {
                alert('There was an error setting your ticket type.');
            });
    });

    $('#ticket-severity').change(function(e) {
        var self = $(this);
        var value = self.val();
        window.parent.Ts.Services.Tickets.SetTicketSeverity(_ticketID, value, function(result) {
                if (result !== null) {
                    resetSLAInfo();
                    slaCheckTimer = setInterval(RefreshSlaDisplay, 5000);
                    window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changeseverity", userFullName);
                }
            },
            function(error) {
                alert('There was an error setting your ticket severity.');
            });
    });

    $('#ticket-visible').change(function(e) {
        var self = $(this);

        if (window.parent.Ts.System.User.ChangeTicketVisibility || window.parent.Ts.System.User.IsSystemAdmin) {
            var value = self.is(":checked");
            window.parent.Ts.System.logAction('Ticket - Visibility Changed');
            window.parent.Ts.Services.Tickets.SetIsVisibleOnPortal(_ticketID, value, function(result) {
                    window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changeisportal", userFullName);
                },
                function(error) {
                    alert('There was an error saving the ticket portal visible\'s status.');
                });
        } else {
            self.prop('checked', false);
            alert("Sorry, you do not have permission to change ticket visibility, please contact your TeamSupport admin.");
        }
    });

    $('#ticket-isKB').change(function(e) {
        var self = $(this);

        if (window.parent.Ts.System.User.ChangeKbVisibility || window.parent.Ts.System.User.IsSystemAdmin) {
            var value = self.is(":checked");
            window.parent.Ts.System.logAction('Ticket - KB Status Changed');
            window.parent.Ts.Services.Tickets.SetIsKB(_ticketID, value,
                function(result) {
                    if (result === true) {
                        $('#ticket-group-KBCat').show();
                    } else {
                        $('#ticket-group-KBCat').hide();
                    }
                    window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changeiskb", userFullName);
                },
                function(error) {
                    alert('There was an error saving the ticket knowlegdgebase\'s status.');
                });
        }
    });

    $('#ticket-KB-Category').change(function(e) {
        var self = $(this);
        var value = self.val();
        window.parent.Ts.System.logAction('Ticket - KnowledgeBase Community Changed');
        window.parent.Ts.Services.Tickets.SetTicketKnowledgeBaseCategory(_ticketID, value, function(result) {
                window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changekbcat", userFullName);
            },
            function(error) {
                alert('There was an error setting your ticket knowledgebase category.');
            });
    });

    $('#ticket-Community').change(function(e) {
        var self = $(this);
        var value = self.val();
        var oldCatName = _ticketInfo.Ticket.CategoryName;
        var newCatName = self.text();
        window.parent.Ts.System.logAction('Ticket - Community Changed');
        window.parent.Ts.Services.Tickets.SetTicketCommunity(_ticketID, value, oldCatName == null ? 'Unassigned' : oldCatName, newCatName == null ? 'Unassigned' : newCatName, function(result) {
                window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changecommunity", userFullName);
            },
            function(error) {
                alert('There was an error setting your ticket community.');
            });
    });
}

function SetupCustomerSection() {
    AddCustomers(_ticketInfo.Customers);
    if ($('#ticket-Customers-Input').length) {
        $('#ticket-Customers-Input').selectize({
            valueField: 'id',
            labelField: 'label',
            searchField: 'label',
            load: function(query, callback) {
                this.clearOptions(); // clear the data
                this.renderCache = {}; // clear the html template cache
                getTicketCustomers(query, callback)
            },
            delimiter: null,
            initData: true,
            preload: false,
            onLoad: function() {
                if (this.settings.initData === true) {
                    this.settings.initData = false;
                }
            },
            create: function(input, callback) {
                $('#NewCustomerModal').modal('show');
                callback(null);
                $('#ticket-Customers-Input').closest('.form-horizontal').removeClass('hasError');
            },
            onItemAdd: function(value, $item) {
                if (this.settings.initData === false) {
                    $('#ticket-Customers-Input').closest('.form-horizontal').removeClass('hasError');
                    var customerData = $item.data();

                    window.parent.Ts.Services.Tickets.AddTicketCustomer(_ticketID, customerData.type, value, function(customers) {
                        AddCustomers(customers);

                        if (customerData.type == "u") {
                            window.parent.Ts.Services.Customers.LoadAlerts(value, window.parent.Ts.ReferenceTypes.Users, function(notes) {
                                for (var i = 0; i < notes.length; i++) {
                                    var note = notes[i];
                                    LoadTicketNotes(note);
                                }
                            });
                        } else {
                            window.parent.Ts.Services.Customers.LoadAlerts(value, window.parent.Ts.ReferenceTypes.Organizations, function(notes) {
                                for (var i = 0; i < notes.length; i++) {
                                    var note = notes[i];
                                    LoadTicketNotes(note);
                                }
                            });
                        }

                        window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "addcustomer", userFullName);
                    }, function() {
                        $(this).parent().remove();
                        alert('There was an error adding the customer.');
                    });
                    this.removeItem(value, true);
                    window.parent.Ts.System.logAction('Ticket - Customer Added');
                }
            },
            plugins: {
                'sticky_placeholder': {},
                'no_results': {}
            },
            score: function(search) {
                return function(option) {
                    return 1;
                }
            },
            render: {
                item: function(item, escape) {
                    return '<div data-value="' + item.value + '" data-type="' + item.data + '" data-selectable="" class="option">' + item.label + '</div>';
                },
                option: function(item, escape) {
                    return '<div data-value="' + escape(item.value) + '" data-type="' + escape(item.data) + '" data-selectable="" class="option">' + item.label + '</div>';
                },
                option_create: function(data, escape) {
                    return '<div class="create">Create <strong>' + escape(data.input) + '</strong></div>';
                }
            },
            onDropdownClose: function($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });
    }

    var canCreateCustomers = false;
    if ((top.Ts.System.User.CanCreateContact && top.Ts.System.User.CanCreateCompany) || top.Ts.System.User.IsSystemAdmin) {
        canCreateCustomers = true;
    }

    $('#customer-company-input').selectize({
        valueField: 'label',
        labelField: 'label',
        searchField: 'label',
        load: function(query, callback) {
            this.clearOptions(); // clear the data
            this.renderCache = {}; // clear the html template cache
            getCompany(query, callback)
        },
        score: function(search) {
            return function(option) {
                return 1;
            }
        },
        onDropdownClose: function($dropdown) {
            $($dropdown).prev().find('input').blur();
        },
        create: canCreateCustomers,
        closeAfterSelect: true,
        plugins: {
            'sticky_placeholder': {},
            'no_results': {}
        }
    });

    $('#Customer-Create').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        window.parent.Ts.System.logAction('Ticket - New Customer Added');
        var email = $('#customer-email-input').val();
        var firstName = $('#customer-fname-input').val();
        var lastName = $('#customer-lname-input').val();
        var phone = $('#customer-phone-input').val();;
        var companyName = $('#customer-company-input').val();
        window.parent.Ts.Services.Users.CreateNewContact(email, firstName, lastName, companyName, phone, false, function(result) {
            if (result.indexOf("u") == 0 || result.indexOf("o") == 0) {
                window.parent.Ts.Services.Tickets.AddTicketCustomer(_ticketID, result.charAt(0), result.substring(1), function(result) {
                    AddCustomers(result);
                    $('#customer-email-input').val('');
                    $('#customer-fname-input').val('');
                    $('#customer-lname-input').val('');
                    $('#customer-phone-input').val('');
                    var $select = $('#customer-company-input').selectize();
                    var control = $select[0].selectize;
                    control.clear();
                    $('#NewCustomerModal').modal('hide');
                });
            } else if (result.indexOf("The company you have specified is invalid") !== -1) {
                if (window.parent.Ts.System.User.CanCreateCompany || window.parent.Ts.System.User.IsSystemAdmin) {
                    if (confirm('Unknown company, would you like to create it?')) {
                        window.parent.Ts.Services.Users.CreateNewContact(email, firstName, lastName, companyName, phone, true, function(result) {
                            window.parent.Ts.Services.Tickets.AddTicketCustomer(_ticketID, result.charAt(0), result.substring(1), function(result) {
                                AddCustomers(result);
                                $('.ticket-new-customer-email').val('');
                                $('.ticket-new-customer-first').val('');
                                $('.ticket-new-customer-last').val('');
                                $('.ticket-new-customer-company').val('');
                                $('.ticket-new-customer-phone').val('');
                                $('#NewCustomerModal').modal('hide');
                            });
                        });
                    }
                } else {
                    alert("We're sorry, but you do not have the rights to create a new company.");
                    $('.ticket-new-customer-email').val('');
                    $('.ticket-new-customer-first').val('');
                    $('.ticket-new-customer-last').val('');
                    $('.ticket-new-customer-company').val('');
                    $('.ticket-new-customer-phone').val('');
                    $('#NewCustomerModal').modal('hide');
                }
            } else {
                alert(result);
            }
        });
    });

    $('#ticket-Customer').on('click', 'span.tagRemove', function(e) {
        var self = $(this);
        var data = self.parent().data().tag;

        if (data.UserID) {
            window.parent.Ts.Services.Tickets.RemoveTicketContact(_ticketID, data.UserID, function(customers) {
                AddCustomers(customers);
                window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "removecontact", userFullName);
            }, function() {
                alert('There was a problem removing the contact from the ticket.');
            });
            window.parent.Ts.System.logAction('Ticket - Contact Removed');
        } else {
            window.parent.Ts.Services.Tickets.RemoveTicketCompany(_ticketID, data.OrganizationID, function(customers) {
                AddCustomers(customers);
                window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "removecompany", userFullName);
            }, function() {
                alert('There was a problem removing the company from the ticket.');
            });
            window.parent.Ts.System.logAction('Ticket - Customer Removed');
        }
    });
}

function AddCustomers(customers) {
    var customerDiv = $("#ticket-Customer");
    customerDiv.empty();
    $("#ticket-Customers-Input").val('');
    for (var i = 0; i < customers.length; i++) {
        var label = "";
        var cssClasses = "tag-item";

        if (customers[i].Flag) {
            cssClasses = cssClasses + " tag-error"
        }
        if (customers[i].Contact !== null && customers[i].Company !== null) {
            label = '<span class="UserAnchor" data-userid="' + customers[i].UserID + '" data-placement="left" data-ticketid="' + _ticketID + '">' + customers[i].Contact + '</span><br/><span class="OrgAnchor" data-orgid="' + customers[i].OrganizationID + '" data-placement="left">' + customers[i].Company + '</span>';
            var newelement = PrependTag(customerDiv, customers[i].UserID, label, customers[i], cssClasses);
        } else if (customers[i].Contact !== null) {
            label = '<span class="UserAnchor" data-userid="' + customers[i].UserID + '" data-placement="left">' + customers[i].Contact + '</span>';
            var newelement = PrependTag(customerDiv, customers[i].UserID, label, customers[i], cssClasses);
            newelement.data('userid', customers[i].UserID).data('placement', 'left').data('ticketid', _ticketID);
        } else if (customers[i].Company !== null) {
            label = '<span class="OrgAnchor" data-orgid="' + customers[i].OrganizationID + '" data-placement="left">' + customers[i].Company + '</span>';
            var newelement = PrependTag(customerDiv, customers[i].OrganizationID, label, customers[i], cssClasses);
            newelement.data('orgid', customers[i].OrganizationID).data('placement', 'left').data('ticketid', _ticketID);
        }
    };
}

function clearTicketEditor() {
    $('#action-new-editor').parent().fadeOut('normal', function() {
        tinymce.activeEditor.destroy();
    });
    $('.upload-queue').empty();
}

function SetupTagsSection() {
    AddTags(_ticketInfo.Tags);
    if ($('#ticket-tag-Input').length) {
        $("#ticket-tag-Input").autocomplete({
                minLength: 2,
                source: getTags,
                response: function(event, ui) {
                    var inputValue = $(this).val();

                    var filtered = $(ui.content).filter(function() {
                        return this.value == inputValue;
                    });

                    if (filtered.length === 0) {
                        ui.content.push({
                            label: inputValue,
                            value: inputValue,
                            id: 0
                        });
                    }
                },
                select: function(event, ui) {
                    $(this).data('item', ui.item);

                    window.parent.Ts.Services.Tickets.AddTag(_ticketID, ui.item.value, function(tags) {
                        if (tags !== null) {
                            AddTags(tags);
                        }

                    }, function() {
                        alert('There was an error adding the tag.');
                    });
                    window.parent.Ts.System.logAction('Ticket - Added');
                }
            })
            .data("autocomplete")._renderItem = function(ul, item) {
                return $("<li>").append("<a>" + item.label + "</a>").appendTo(ul);
            };

        $('#ticket-tags').on('click', 'span.tagRemove', function(e) {
            var tag = $(this).parent()[0];
            if (tag) {
                window.parent.Ts.Services.Tickets.RemoveTag(_ticketID, tag.id, function(tags) {
                    $(tag).remove();
                    window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "removetag", userFullName);
                }, function() {
                    alert('There was a problem removing the tag from the ticket.');
                });
            } else {
                alert('There was a problem removing the reminder from the ticket.');
            }
        });
    }
}

function PrependTask(parent, id, value, data) {
    var _compiledTaskTemplate = Handlebars.templates['taskrecord'];
    var taskHTML = _compiledTaskTemplate({
        id: id,
        value: value,
        IsComplete: data.IsComplete
    });
    return $(taskHTML).prependTo(parent).data('task', data);
}

function AddTags(tags) {
    var tagDiv = $("#ticket-tags");
    tagDiv.empty();
    $("#ticket-tag-Input").val('');
    for (var i = 0; i < tags.length; i++) {
        var label = tags[i].Value
        PrependTag(tagDiv, tags[i].TagID, label, tags[i]);
    };
}

function PrependTag(parent, id, value, data, cssclass) {
    if (cssclass === undefined) cssclass = 'tag-item';
    var _compiledTagTemplate = Handlebars.templates['ticket-tag'];
    var tagHTML = _compiledTagTemplate({
        id: id,
        value: value,
        data: data,
        css: cssclass
    });
    return $(tagHTML).prependTo(parent).data('tag', data);
}

function UpdateTicketGroups(callback) {
    var selectizeGroup = $('#ticket-group')[0].selectize;
    selectizeGroup.clear(true);
    selectizeGroup.clearOptions();
    selectizeGroup.addOption({
        value: -1,
        text: 'Unassigned'
    });

    var persistedGroup = false;
    var groups = window.parent.Ts.Cache.getGroups();
    if (window.parent.Ts.System.Organization.UseProductFamilies && _productFamilyID != null) {
        for (var i = 0; i < groups.length; i++) {
            if (groups[i].ProductFamilyID == null || _productFamilyID == groups[i].ProductFamilyID) {
                selectizeGroup.addOption({
                    value: groups[i].GroupID,
                    text: groups[i].Name
                });
                if (_ticketGroupID == groups[i].GroupID) {
                    selectizeGroup.addItem(groups[i].GroupID, false);
                    persistedGroup = true;
                }
            }
        }
    } else {
        persistedGroup = true;
        for (var i = 0; i < groups.length; i++) {
            selectizeGroup.addOption({
                value: groups[i].GroupID,
                text: groups[i].Name
            });
            if (_ticketGroupID == groups[i].GroupID) {
                selectizeGroup.addItem(groups[i].GroupID, false);
            }
        }
    }
    if (!persistedGroup && _ticketGroupID != null) {
        selectizeGroup.addItem(-1);
        callback(false);
    } else {
        callback(true);
    }
}

function UpdateTicketTypes(persistedGroup, callback) {
    var selectizeType = $('#ticket-type')[0].selectize;
    selectizeType.clear(true);
    selectizeType.clearOptions();

    var firstTypeID = 0;
    var persistedType = false;
    var types = window.parent.Ts.Cache.getTicketTypes();
    if (window.parent.Ts.System.Organization.UseProductFamilies && _productFamilyID != null) {
        for (var i = 0; i < types.length; i++) {
            if (types[i].ProductFamilyID == null || _productFamilyID == types[i].ProductFamilyID) {
                selectizeType.addOption({
                    value: types[i].TicketTypeID,
                    text: types[i].Name
                });
                _lastTicketTypeID = types[i].TicketTypeID;
                if (firstTypeID == 0) {
                    firstTypeID = types[i].TicketTypeID;
                }
                if (_ticketTypeID == types[i].TicketTypeID) {
                    selectizeType.addItem(types[i].TicketTypeID, true);
                    persistedType = true;
                }
            }
        }
    } else {
        persistedType = true;
        for (var i = 0; i < types.length; i++) {
            selectizeType.addOption({
                value: types[i].TicketTypeID,
                text: types[i].Name
            });
            if (firstTypeID == 0) {
                firstTypeID = types[i].TicketTypeID;
            }
            if (_ticketTypeID == types[i].TicketTypeID) {
                selectizeType.addItem(types[i].TicketTypeID, true);
            }
        }
    }

    if (!persistedType) {
        selectizeType.addItem(firstTypeID);
    }

    callback({
        Group: persistedGroup,
        Type: persistedType
    })
}

function SetupProductSection() {
    window.parent.Ts.Settings.Organization.read('ShowOnlyCustomerProducts', false, function(showOnlyCustomers) {
        if (showOnlyCustomers == "True") {
            window.parent.Ts.Services.TicketPage.GetTicketCustomerProducts(_ticketID, function(CustomerProducts) {
                LoadProductList(CustomerProducts);
            });
        } else {
            var products = window.parent.Ts.Cache.getProducts();
            LoadProductList(products);
        }

        var product = window.parent.Ts.Cache.getProduct(_ticketInfo.Ticket.ProductID);
        SetupProductVersionsControl(product);
        SetProductVersionAndResolved(_ticketInfo.Ticket.ReportedVersionID, _ticketInfo.Ticket.SolvedVersionID);

        window.parent.Ts.Services.Organizations.IsProductRequired(function(result) {
            if (result && _ticketInfo.Ticket.ProductID == null) {
                $('#ticket-Product').closest('.form-horizontal').addClass('hasError');
            } else {
                $('#ticket-Product').closest('.form-horizontal').removeClass('hasError');
            }
        });

        $('#ticket-Product').change(function(e) {
            var self = $(this);
            window.parent.Ts.Services.Tickets.SetProduct(_ticketID, self.val(), function(result) {
                    if (result !== null) {
                        var name = result.label;
                        var product = window.parent.Ts.Cache.getProduct(self.val());

                        window.parent.Ts.Services.Organizations.IsProductRequired(function(IsRequired) {
                            if (IsRequired && (name == null || name == '')) {
                                $('#ticket-Product').closest('.form-horizontal').addClass('hasError');
                            } else {
                                $('#ticket-Product').closest('.form-horizontal').removeClass('hasError');
                            }
                        });

                        SetupProductVersionsControl(product);
                        SetProductVersionAndResolved(null, null);
                        if (window.parent.Ts.System.Organization.UseProductFamilies && _productFamilyID != result.data) {
                            _productFamilyID = result.data;
                            UpdateTicketGroups(function(persistedGroup) {
                                UpdateTicketTypes(persistedGroup, function(persistedData) {
                                    if (!persistedData.Group || !persistedData.Type) {
                                        var message = 'The new product belongs to a different line. Please update ';
                                        if (!persistedData.Group) {
                                            message += 'Group';
                                            if (!persistedData.Type) {
                                                message += ' and ';
                                            }
                                        }
                                        if (!persistedData.Type) {
                                            message += 'Type';
                                        }
                                        alert(message += '.');
                                    }
                                });
                            });
                        }
                    }

                    window.parent.Ts.Services.Tickets.GetParentValues(_ticketID, function(fields) {
                        AppenCustomValues(fields);
                    });

                    resetSLAInfo();
                    slaCheckTimer = setInterval(RefreshSlaDisplay, 5000);
                    window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changeproduct", userFullName);
                },
                function(error) {
                    alert('There was an error setting the product.');
                });
            $(".popover").remove();
        });

        $('#ticket-Versions').change(function(e) {
            window.parent.Ts.System.logAction('Ticket - Reported Version Changed');
            window.parent.Ts.Services.Tickets.SetReportedVersion(_ticketID, $(this).val(), function(result) {
                    $('#ticket-Versions').closest('.form-horizontal').removeClass('hasError');
                    window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changereported", userFullName);
                },
                function(error) {
                    alert('There was an error setting the reported version.');
                });
        });

        $('#ticket-Resolved').change(function(e) {
            window.parent.Ts.System.logAction('Ticket - Resolved Version Changed');
            window.parent.Ts.Services.Tickets.SetSolvedVersion(_ticketID, $(this).val(), function(result) {
                    $('#ticket-Resolved').closest('.form-horizontal').removeClass('hasError');
                    window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changeresolved", userFullName);
                },
                function(error) {
                    alert('There was an error setting the reported version.');
                });
        });
    })
}

function LoadProductList(products) {
    if ($('#ticket-Product').length) {
        if (products == null) products = window.parent.Ts.Cache.getProducts();

        if (typeof products !== "undefined") {
            for (var i = 0; i < products.length; i++) {
                AppendSelect('#ticket-Product', products[i], 'product', products[i].ProductID, products[i].Name, (products[i].ProductID === _ticketInfo.Ticket.ProductID));
            }
        }

        var $productselect = $('#ticket-Product').selectize({
            render: {
                item: function(item, escape) {
                    return '<div data-ticketid="' + _ticketID + '" data-productid="' + escape(item.value) + '" data-value="' + escape(item.value) + '" data-type="' + escape(item.data) + '" data-selectable="" data-placement="left" class="option ProductAnchor">' + escape(item.text) + '</div>';
                }
            },
            plugins: {
                'sticky_placeholder': {},
                'no_results': {}
            },
            allowEmptyOption: true,
            loadThrottle: null,
            onDropdownClose: function($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true,
            // placeholder: 'unassigned'
        });

        if (_ticketInfo.Ticket.ProductID == null) {
            var $productselectInput = $productselect[0].selectize;
            $productselectInput.clear();
        }
    }
}

function LoadGroups() {
    var selectField = $('#ticket-group');
    if (selectField.length > 0) {
        window.parent.Ts.Services.TicketPage.GetTicketGroups(_ticketID, function(groups) {
            var selectize = selectField[0].selectize;
            selectize.clear(true);
            selectize.clearOptions();
            selectize.addOption({
                value: -1,
                text: 'Unassigned',
                data: ''
            });

            for (var i = 0; i < groups.length; i++) {
                selectize.addOption({
                    value: groups[i].ID,
                    text: groups[i].Name,
                    data: groups[i]
                });
                if (groups[i].IsSelected) selectize.addItem(groups[i].ID, false);
            }
        });
    }
    if ($('#ticket-group').val() == -1) {
        $('#ticket-group').closest('.form-horizontal').addClass('hasError');
    } else {
        $('#ticket-group').closest('.form-horizontal').removeClass('hasError');
    }
}

function SetupProductVersionsControl(product) {
    if ($('#ticket-Versions').length) {
        var $select = $('#ticket-Versions').selectize({
            render: {
                item: function(item, escape) {
                    return '<div data-ticketid="' + _ticketID + '" data-versionid="' + escape(item.value) + '" data-value="' + escape(item.value) + '" data-type="' + escape(item.data) + '" data-selectable="" data-placement="left" class="option VersionAnchor">' + escape(item.text) + '</div>';
                }
            },
            onDropdownClose: function($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true,
            loadThrottle: null,
        });
        var versionInput = $select[0].selectize;

        if (versionInput) {
            versionInput.destroy();
        }
    }
    if ($('#ticket-Resolved').length) {
        var $select = $('#ticket-Resolved').selectize({
            render: {
                item: function(item, escape) {
                    return '<div data-ticketid="' + _ticketID + '" data-versionid="' + escape(item.value) + '" data-value="' + escape(item.value) + '" data-type="' + escape(item.data) + '" data-selectable="" data-placement="left" class="option VersionAnchor">' + escape(item.text) + '</div>';
                }
            },
            onDropdownClose: function($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true,
            loadThrottle: null
        });
        var resolvedInput = $select[0].selectize;

        if (resolvedInput) {
            resolvedInput.destroy();
        }
    }

    $('#ticket-Versions').empty();
    $('#ticket-Resolved').empty();
    if (product !== null && product.Versions.length > 0) {
        var versions = product.Versions;

        for (var i = 0; i < versions.length; i++) {
            try {
                AppendSelect('#ticket-Versions', versions[i], 'version', versions[i].ProductVersionID, versions[i].VersionNumber, false);
                AppendSelect('#ticket-Resolved', versions[i], 'resolved', versions[i].ProductVersionID, versions[i].VersionNumber, false);
            } catch (e) {}
        }

        if ($('#ticket-Resolved').length) {
            $('#ticket-Versions').selectize({
                render: {
                    item: function(item, escape) {
                        return '<div data-ticketid="' + _ticketID + '" data-versionid="' + escape(item.value) + '" data-value="' + escape(item.value) + '" data-type="' + escape(item.data) + '" data-selectable="" data-placement="left" class="option VersionAnchor">' + escape(item.text) + '</div>';
                    }
                },
                onDropdownClose: function($dropdown) {
                    $($dropdown).prev().find('input').blur();
                },
                closeAfterSelect: true
            });
        }

        if ($('#ticket-Resolved').length) {
            $('#ticket-Resolved').selectize({
                render: {
                    item: function(item, escape) {
                        return '<div data-ticketid="' + _ticketID + '" data-versionid="' + escape(item.value) + '" data-value="' + escape(item.value) + '" data-type="' + escape(item.data) + '" data-selectable="" data-placement="left" class="option VersionAnchor">' + escape(item.text) + '</div>';
                    }
                },
                onDropdownClose: function($dropdown) {
                    $($dropdown).prev().find('input').blur();
                },
                closeAfterSelect: true
            });
        }
    }
}

function SetProductVersionAndResolved(versionId, resolvedId) {
    if ($('#ticket-Versions').length) {
        var $select = $("#ticket-Versions").selectize({
            onDropdownClose: function($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });
        SetVersion(versionId)
    }

    if ($('#ticket-Resolved').length) {
        var $select = $("#ticket-Resolved").selectize({
            onDropdownClose: function($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });
        SetSolved(resolvedId);
    }

    window.parent.Ts.Services.Organizations.IsProductVersionRequired(function(IsProductVersionRequired) {
        if (IsProductVersionRequired && (versionId == null && resolvedId == null)) {
            $('#ticket-Versions').closest('.form-horizontal').addClass('hasError');
        } else {
            $('#ticket-Versions').closest('.form-horizontal').removeClass('hasError');
        }
    });
};

function SetupInventorySection() {
    AddInventory(_ticketInfo.Assets);
    if ($('#ticket-Inventory-Input').length) {
        $('#ticket-Inventory-Input').selectize({
            valueField: 'id',
            labelField: 'label',
            searchField: 'label',
            load: function(query, callback) {
                this.clearOptions(); // clear the data
                this.renderCache = {}; // clear the html template cache
                getAssets(query, callback)
            },
            score: function(search) {
                return function(option) {
                    return 1;
                }
            },
            onItemAdd: function(value, $item) {
                window.parent.Ts.Services.Tickets.AddTicketAsset(_ticketID, value, function(assets) {
                    AddInventory(assets);
                    window.parent.Ts.Services.Tickets.GetTicketCustomers(_ticketID, function(customers) {
                        AddCustomers(customers);
                    });
                    window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "addasset", userFullName);
                }, function() {
                    alert('There was an error adding the asset.');
                });
                window.parent.Ts.System.logAction('Ticket - Asset Added');
                this.removeItem(value, true);
            },
            onDropdownClose: function($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true,
            plugins: {
                'sticky_placeholder': {},
                'no_results': {}
            }
        });

        $('#ticket-Inventory').on('click', 'span.tagRemove', function(e) {
            var self = $(this);
            var data = self.parent().data().tag;
            window.parent.Ts.Services.Tickets.RemoveTicketAsset(_ticketID, data.AssetID, function(assets) {
                AddInventory(assets);
                window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "removeasset", userFullName);
            }, function() {
                alert('There was a problem removing the asset from the ticket.');
            });
        });
    }
}

function AddInventory(Inventory) {
    var InventoryDiv = $("#ticket-Inventory");
    InventoryDiv.empty();
    $("#ticket-Inventory-Input").val('');
    for (var i = 0; i < Inventory.length; i++) {
        var label = '<span class="AssetAnchor" data-assetid="' + Inventory[i].AssetID + '" data-placement="left">' + Inventory[i].Name + '</span>';
        var newelement = PrependTag(InventoryDiv, Inventory[i].AssetID, label, Inventory[i], "tag-item");
        newelement.data('assetid', Inventory[i].AssetID).data('placement', 'left');
    }
}

function SetupUserQueuesSection() {
    AddQueues(_ticketInfo.Queuers);
    if ($('#ticket-UserQueue-Input').length) {
        $('#ticket-UserQueue-Input').selectize({
            valueField: 'id',
            labelField: 'label',
            searchField: 'label',
            load: function(query, callback) {
                this.clearOptions(); // clear the data
                this.renderCache = {}; // clear the html template cache
                getUsers(query, callback)
            },
            score: function(search) {
                return function(option) {
                    return 1;
                }
            },
            onItemAdd: function(value, $item) {
                window.parent.Ts.Services.Tickets.SetQueue(_ticketID, true, value, function(queues) {
                    AddQueues(queues);
                    window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "addqueue", userFullName);
                }, function() {
                    alert('There was an error adding the queue.');
                });
                window.parent.Ts.System.logAction('Ticket - Enqueued');
                window.parent.Ts.System.logAction('Queued');
                this.removeItem(value, true);
            },
            plugins: {
                'sticky_placeholder': {},
                'no_results': {}
            },
            onDropdownClose: function($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });

        $('#ticket-UserQueue').on('click', 'span.tagRemove', function(e) {
            var self = $(this);
            var data = self.parent().data().tag;

            window.parent.Ts.Services.Tickets.SetQueue(_ticketID, false, data.UserID, function(queues) {
                AddQueues(queues);
                window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "removequeue", userFullName);
            }, function() {
                alert('There was a problem removing the queue from the ticket.');
            });
            window.parent.Ts.System.logAction('Ticket - Dequeued');
        });
    }
}

function AddQueues(queues) {
    var UserQueueDiv = $("#ticket-UserQueue");
    UserQueueDiv.empty();
    $("#ticket-UserQueue-Input").val('');

    for (var i = 0; i < queues.length; i++) {
        var label = '<span class="UserAnchor" data-userid="' + queues[i].UserID + '" data-placement="left">' + queues[i].FirstName + " " + queues[i].LastName + '</span>';
        var newelement = PrependTag(UserQueueDiv, queues[i].UserID, label, queues[i], "tag-item");
        newelement.data('userid', queues[i].UserID).data('placement', 'left').data('ticketid', _ticketID);
    };
}

function SetupSubscribedUsersSection() {
    AddSubscribers(_ticketInfo.Subscribers);
    if ($('#ticket-SubscribedUsers-Input').length) {
        $('#ticket-SubscribedUsers-Input').selectize({
            valueField: 'id',
            labelField: 'label',
            searchField: 'label',
            load: function(query, callback) {
                this.clearOptions(); // clear the data
                this.renderCache = {}; // clear the html template cache
                getUsers(query, callback)
            },
            score: function(search) {
                return function(option) {
                    return 1;
                }
            },
            onItemAdd: function(value, $item) {
                window.parent.Ts.Services.Tickets.SetSubscribed(_ticketID, true, value, function(subscribers) {
                    AddSubscribers(subscribers);
                    window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "addsubscriber", userFullName);
                }, function() {
                    alert('There was an error adding the subscriber.');
                });
                window.parent.Ts.System.logAction('Ticket - User Subscribed');
                this.removeItem(value, true);
            },
            plugins: {
                'sticky_placeholder': {},
                'no_results': {}
            },
            onDropdownClose: function($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });

        $('#ticket-SubscribedUsers').on('click', 'span.tagRemove', function(e) {
            var self = $(this);
            var data = self.parent().data().tag;
            window.parent.Ts.Services.Tickets.SetSubscribed(_ticketID, false, data.UserID, function(subscribers) {
                AddSubscribers(subscribers);
                window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "removesubscriber", userFullName);
            }, function() {
                alert('There was a problem removing the subscriber from the ticket.');
            });
            window.parent.Ts.System.logAction('Ticket - Subscriber Removed');
        });
    }
};

function AddSubscribers(Subscribers) {
    var SubscribersDiv = $("#ticket-SubscribedUsers");
    SubscribersDiv.empty();
    $("#ticket-SubscribedUsers-Input").val('');

    for (var i = 0; i < Subscribers.length; i++) {
        var label = '<span class="UserAnchor" data-userid="' + Subscribers[i].UserID + '" data-placement="left">' + Subscribers[i].FirstName + " " + Subscribers[i].LastName + '</span>';
        var newelement = PrependTag(SubscribersDiv, Subscribers[i].UserID, label, Subscribers[i], "tag-item");
        newelement.data('userid', Subscribers[i].UserID).data('placement', 'left').data('ticketid', _ticketID);
    }
}

function SetupAssociatedTicketsSection() {
    AddAssociatedTickets(_ticketInfo.Related);
    if ($('#ticket-AssociatedTickets-Input').length) {
        $('#ticket-AssociatedTickets-Input').selectize({
            valueField: 'data',
            labelField: 'label',
            searchField: 'label',
            loadThrottle: null,
            load: function(query, callback) {
                this.clearOptions(); // clear the data
                this.renderCache = {}; // clear the html template cache
                getRelated(query, callback)
            },
            score: function(search) {
                return function(option) {
                    return 1;
                }
            },
            onItemAdd: function(value, $item) {
                $('#AssociateTicketModal').data('ticketid', value).modal('show');
                this.removeItem(value, true);
            },
            plugins: {
                'sticky_placeholder': {},
                'no_results': {}
            },
            onDropdownClose: function($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });

        $('#ticket-AssociatedTickets').on('click', 'span.tagRemove', function(e) {
            e.preventDefault();
            e.stopPropagation();
            var self = $(this);
            var data = self.parent().data().tag;
            window.parent.Ts.Services.Tickets.RemoveRelated(_ticketID, data.TicketID, function(result) {
                if (result !== null && result === true) self.parent().remove();
                window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "removerelationship", userFullName);
            }, function() {
                alert('There was an error removing the associated ticket.');
            });

            window.parent.Ts.System.logAction('Ticket - Association Removed');
        });

        $('#ticket-AssociatedTickets').on('click', 'div.tag-item', function(e) {
            var self = $(this);
            var data = self.data().tag;
            window.parent.Ts.MainPage.openTicket(data.TicketNumber, true);
        });

        $('.ticket-association').click(function(e) {
            var IsParent = $(this).data('isparent');
            var TicketID2 = $(this).closest('#AssociateTicketModal').data('ticketid');
            $('#associate-error').hide();
            $('#associate-success').hide();
            if (_ticketID == TicketID2) {
                $('#associate-error').text('You picked the ticket you are viewing. Please try again.').show();
                return;
            }
            window.parent.Ts.Services.Tickets.AddRelated(_ticketID, TicketID2, IsParent, function(tickets) {
                $('#AssociateTicketModal').modal('hide');
                AddAssociatedTickets(tickets);
                window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "addrelationship", userFullName);
            }, function(error) {
                $('#associate-error').text(error.get_message()).show();
            });
        });
    }
}

function AddAssociatedTickets(Tickets) {
    if (Tickets !== null) {
        var AssociatedTicketsDiv = $("#ticket-AssociatedTickets");
        AssociatedTicketsDiv.empty();
        $("#ticket-AssociatedTickets-Input").val('');
        for (var i = 0; i < Tickets.length; i++) {
            var related = Tickets[i];
            var caption = 'Related';
            if (related.IsParent !== null) {
                caption = (related.IsParent === true ? 'Parent' : 'Child');
            }
            var label = caption + "<br />" + ellipseString(related.TicketNumber + ': ' + related.Name, 30);

            var newelement = PrependTag(AssociatedTicketsDiv, related.TicketID, related.IsClosed ? '<s>' + label + '</s>' : label, related, 'tag-item TicketAnchor');
            newelement.data('ticketid', related.TicketID).data('placement', 'left').data('IsParent', related.IsParent);
        }
    }
}

function SetupRemindersSection() {
    AddReminders(_ticketInfo.Reminders);
    if ($('#ticket-reminder-who').length) {
        $('#ticket-reminder-date').datetimepicker({
            useCurrent: true,
            format: dateFormat + ' hh:mm A',
            defaultDate: new Date()
        });

        var $reminderSelect = $('#ticket-reminder-who').selectize({
            valueField: 'id',
            labelField: 'label',
            searchField: 'label',
            load: function(query, callback) {
                this.clearOptions(); // clear the data
                this.renderCache = {}; // clear the html template cache
                window.parent.Ts.Services.TicketPage.SearchUsers(query, function(result) {
                    callback(result);
                });
            },
            score: function(search) {
                return function(option) {
                    return 1;
                }
            },
            onDropdownClose: function($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });

        var selectizeControl = $reminderSelect[0].selectize;
        var currUserObj = {
            id: window.parent.Ts.System.User.UserID,
            label: userFullName
        };
        selectizeControl.addOption(currUserObj);
        selectizeControl.addItem(window.parent.Ts.System.User.UserID);

        $('#RemindersModal').on('hidden.bs.modal', function() {
            $('#ticket-reminder-title').val('');
            $('#ticket-reminder-date').val('');
            $('#reminderID').text('');
        })

        $('#ticket-reminder-save').click(function(e) {
            var selectizeControl = $reminderSelect[0].selectize;
            var date = window.parent.Ts.Utils.getMsDate(moment($('#ticket-reminder-date').val(), dateFormat + ' hh:mm A').format('MM/DD/YYYY hh:mm A'));
            var userid = selectizeControl.getValue();
            if (userid == "") {
                $('#ticket-reminder-who').parent().addClass('has-error').removeClass('has-success');
            } else {
                $('#ticket-reminder-who').closest('.form-group').addClass('has-success').removeClass('has-error');
            }
            var title = $('#ticket-reminder-title').val();
            if (title == "") {
                $('#ticket-reminder-title').parent().addClass('has-error').removeClass('has-success');
            } else {
                $('#ticket-reminder-title').parent().addClass('has-success').removeClass('has-error');
            }

            var reminderID = $('#reminderID').text();
            if (reminderID == '') reminderID = null;
            window.parent.Ts.Services.System.EditReminder(reminderID, window.parent.Ts.ReferenceTypes.Tickets, _ticketID, title, date, userid, function(result) {
                    $('#RemindersModal').modal('hide');
                    $('#reminderID').text('');
                    $('#ticket-reminder-title').val('');
                    $('#ticket-reminder-date').val('');
                    $('#reminder-error').hide();
                    selectizeControl.clear();
                    window.parent.Ts.Services.System.GetItemReminders(window.parent.Ts.ReferenceTypes.Tickets, _ticketID, window.parent.Ts.System.User.UserID, function(reminders) {
                        AddReminders(reminders);
                    })
                },
                function() {
                    $('#reminder-error').show();
                });
        });

        $('#ticket-reminder-span').on('click', 'span.tagRemove', function(e) {
            var reminder = $(this).parent()[0];
            reminderClose = true;
            var currentUserID = $(reminder).data().tag.CreatorID;
            if (reminder && currentUserID == window.parent.Ts.System.User.UserID) {
                window.parent.Ts.Services.System.DismissReminder(reminder.id, function() {
                    $(reminder).remove();
                    window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "removereminder", userFullName);
                }, function() {
                    alert('There was a problem removing the reminder from the ticket.');
                });
            } else {
                if (currentUserID != window.parent.Ts.System.User.UserID) {
                    alert('You do not have permission to delete this reminder');
                } else {
                    alert('There was a problem removing the reminder from the ticket.');
                }
            }
        });

        $('#ticket-reminder-span').on('click', '.tag-item', function(e) {
            var reminder = $(this).data('tag');
            $('#reminderID').text(reminder.ReminderID);
            //var selectizeControl = $reminderSelect[0].selectize;
            //selectizeControl.addItem(1839999);
            $('#ticket-reminder-title').val(reminder.Description);
            var date = reminder.DueDate == null ? null : window.parent.Ts.Utils.getMsDate(reminder.DueDate);
            $('#ticket-reminder-date').val(date.localeFormat(window.parent.Ts.Utils.getDateTimePattern()));
            if (!reminderClose) {
                $('#RemindersModal').modal('show');
            }
        });
    }
}

function SetupTasksSection() {
    AddTasks(_ticketInfo.Tasks);
    $('#ticket-task-span').on('click', '.change-task-status', function(e) {
        var id = $(this).data('taskid');
        var checkbox = $(this);
        var checked = $(this).prop("checked");
        parent.Ts.System.logAction('Ticket Page - Change Task Status');
        var iframeName = 'iframe-o-' + id;
        var iframeLink = $(iframeName + ':hidden');
        parent.Ts.Services.Task.SetTaskIsCompleted(id, checked, function(data) {
            if (data.IncompleteSubtasks) {
                checkbox.prop("checked", false);
                alert('There are subtasks pending completion, please finish them before completing the parent task.')
            } else if (data.Value) {
                try {
                    parent.document.getElementById(iframeName).contentDocument.location.reload(true);
                } catch (err) {}
                _completeCommentTaskID = id;
                $('#modalTaskComment').modal('show');
            }
        });
    });

    $('#ticket-task-span').on('click', 'span.tagRemove', function(e) {
        var reminder = $(this).parent()[0];
        reminderClose = true;
        var currentUserID = $(reminder).data().tag.CreatorID;
        if (reminder && currentUserID == window.parent.Ts.System.User.UserID) {
            window.parent.Ts.Services.System.DismissReminder(reminder.id, function() {
                $(reminder).remove();
                window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "removereminder", userFullName);
            }, function() {
                alert('There was a problem removing the reminder from the ticket.');
            });
        } else {
            if (currentUserID != window.parent.Ts.System.User.UserID) {
                alert('You do not have permission to delete this reminder');
            } else {
                alert('There was a problem removing the reminder from the ticket.');
            }
        }
    });

    $('.taskContainer').on('click', 'a.new-task', function(e) {
        e.preventDefault();
        parent.Ts.System.logAction('Tasks Page - New Task');
        parent.Ts.MainPage.newTaskFromSource(17, _ticketID, _ticketInfo.Ticket.Name, _ticketInfo.Ticket.TicketNumber);
    });

    $('.taskContainer').on('click', 'a.tasklink', function(e) {
        e.preventDefault();
        var id = $(this).data('taskid');
        parent.Ts.System.logAction('Tasks Page - View Task');
        parent.Ts.MainPage.openNewTask(id);
    });

    $('#TaskList').on('click', '.change-task-status', function(e) {
        var id = $(this).data('taskid');
        var checked = $(this).prop("checked");
        parent.Ts.System.logAction('Tasks Page - Change Task Status');
        parent.Ts.Services.Task.SetTaskIsCompleted(id, checked, function(data) {
            if (!data.IncompleteSubtasks) {
                _completeCommentTaskID = id;
                $('#modalTaskComment').modal('show');
            } else {
                checkbox.prop("checked", false);
                alert('There are subtasks pending completion, please finish them before completing the parent task.')
            }
        });
    });
}

function AddReminders(reminders) {
    var remindersDiv = $("#ticket-reminder-span");
    remindersDiv.empty();

    for (var i = 0; i < reminders.length; i++) {
        var label = ellipseString(reminders[i].Description, 30) + '<br>' + reminders[i].DueDate.localeFormat(window.parent.Ts.Utils.getDateTimePattern());
        var reminderElem = PrependTag(remindersDiv, reminders[i].ReminderID, label, reminders[i]);
    };
}

function AddTasks(tasks) {
    var tasksDiv = $("#ticket-task-span");
    tasksDiv.empty();
    if (tasks) {
        for (var i = 0; i < tasks.length; i++) {
            var _TaskName = tasks[i].Name;

            if (tasks[i].Name == null) {
                if (tasks[i].Description == null || tasks[i].Description == "") {
                    _TaskName = 'No Title';
                } else {
                    _TaskName = tasks[i].Description;
                }
            }

            if (tasks[i].AssignedTo != null) {
                _TaskName += ' for ' + tasks[i].AssignedTo;
            }
            var reminderElem = PrependTask(tasksDiv, tasks[i].TaskID, _TaskName, tasks[i]);
        }
    }
}

function SetupCustomFieldsSection() {
    AppenCustomValues(_ticketInfo.CustomValues);
}

function AppenCustomValues(fields) {
    var parentContainer = $('#ticket-group-custom-fields');
    if (fields === null || fields.length < 1) {
        parentContainer.empty().hide();
        return;
    }
    parentContainer.empty();

    _parentFields = [];

    for (var i = 0; i < fields.length; i++) {
        var field = fields[i];

        if (field.CustomFieldCategoryID == -1) {
            switch (field.FieldType) {
                case window.parent.Ts.CustomFieldType.Text:
                    AddCustomFieldEdit(field, parentContainer);
                    break;
                case window.parent.Ts.CustomFieldType.Date:
                    AddCustomFieldDate(field, parentContainer);
                    break;
                case window.parent.Ts.CustomFieldType.Time:
                    AddCustomFieldTime(field, parentContainer);
                    break;
                case window.parent.Ts.CustomFieldType.DateTime:
                    AddCustomFieldDateTime(field, parentContainer);
                    break;
                case window.parent.Ts.CustomFieldType.Boolean:
                    AddCustomFieldBool(field, parentContainer);
                    break;
                case window.parent.Ts.CustomFieldType.Number:
                    AddCustomFieldNumber(field, parentContainer);
                    break;
                case window.parent.Ts.CustomFieldType.PickList:
                    AddCustomFieldSelect(field, parentContainer, true);
                    break;
                default:
            }
        }
    }
    appendCategorizedCustomValues(fields);
    parentContainer.show();
}

var appendCategorizedCustomValues = function(fields) {
    window.parent.Ts.Services.CustomFields.GetCategories(window.parent.Ts.ReferenceTypes.Tickets, _ticketTypeID, function(categories) {
        var container = $('#ticket-group-custom-fields');
        for (var j = 0; j < categories.length; j++) {
            var isFirstFieldAdded = true;
            for (var i = 0; i < fields.length; i++) {
                var item = null;
                var field = fields[i];
                if (field.CustomFieldCategoryID == categories[j].CustomFieldCategoryID) {
                    if (isFirstFieldAdded) {
                        isFirstFieldAdded = false;
                        var header = $('<div>').text(categories[j].Category).addClass('customFieldCategoryHeader');
                        container.append($('<hr>')).append(header);
                    }
                    switch (field.FieldType) {
                        case window.parent.Ts.CustomFieldType.Text:
                            AddCustomFieldEdit(field, container);
                            break;
                        case window.parent.Ts.CustomFieldType.Date:
                            AddCustomFieldDate(field, container);
                            break;
                        case window.parent.Ts.CustomFieldType.Time:
                            AddCustomFieldTime(field, container);
                            break;
                        case window.parent.Ts.CustomFieldType.DateTime:
                            AddCustomFieldDateTime(field, container);
                            break;
                        case window.parent.Ts.CustomFieldType.Boolean:
                            AddCustomFieldBool(field, container);
                            break;
                        case window.parent.Ts.CustomFieldType.Number:
                            AddCustomFieldNumber(field, container);
                            break;
                        case window.parent.Ts.CustomFieldType.PickList:
                            AddCustomFieldSelect(field, container, false);
                            break;
                        default:
                    }
                }
            }
        }
        appendConditionalFields();
    });
}

var appendConditionalFields = function() {
    for (var i = 0; i < _parentFields.length; i++) {
        var field = _parentFields[i].data('field');
        $('.' + field.CustomFieldID + 'children').remove();
        var parentContainer = $('#ticket-group-custom-fields');
        var childrenContainer = $('<div>').addClass(field.CustomFieldID + 'children form-horizontal').appendTo(parentContainer);
        appendMatchingParentValueFields(childrenContainer, field);
    }
}

var appendMatchingParentValueFields = function(container, parentField) {
    window.parent.Ts.Services.Tickets.GetMatchingParentValueFields(_ticketID, parentField.CustomFieldID, parentField.Value, function(fields) {
        for (var i = 0; i < fields.length; i++) {
            var field = fields[i];
            var div = $('<div>').addClass('').data('field', field);
            container.append(div);
            switch (field.FieldType) {
                case window.parent.Ts.CustomFieldType.Text:
                    AddCustomFieldEdit(field, div);
                    break;
                case window.parent.Ts.CustomFieldType.Date:
                    AddCustomFieldDate(field, div);
                    break;
                case window.parent.Ts.CustomFieldType.Time:
                    AddCustomFieldTime(field, div);
                    break;
                case window.parent.Ts.CustomFieldType.DateTime:
                    AddCustomFieldDateTime(field, div);
                    break;
                case window.parent.Ts.CustomFieldType.Boolean:
                    AddCustomFieldBool(field, div);
                    break;
                case window.parent.Ts.CustomFieldType.Number:
                    AddCustomFieldNumber(field, div);
                    break;
                case window.parent.Ts.CustomFieldType.PickList:
                    AddCustomFieldSelect(field, div, true);
                    break;
                default:
            }
        }
    });
}

var AddCustomFieldEdit = function(field, parentContainer) {
    var formcontainer = $('<div>').addClass('form-horizontal AddCustomFieldEdit').appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('flexbox').data('field', field).appendTo(formcontainer);
    var labelContainer = $('<div>').addClass('flex1').appendTo(groupContainer);
    var formLabel = $('<div>').addClass('form-label').text(field.Name).appendTo(labelContainer);

    var inputContainer = $('<div>').addClass('flex2 ticket-input-container').appendTo(groupContainer);
    var input = $('<textarea>').addClass('ticket-simple-textarea muted-placeholder autogrow').attr("placeholder", "Enter Value").val(field.Value).appendTo(inputContainer).after(getUrls(field.Value)).autogrow();

    $('textarea.autogrow').autogrow();

    if (field.Mask) {
        input.mask(field.Mask);
        input.attr("placeholder", field.Mask);
    }

    input.change(function(e) {
        var value = input.val();

        if (field.IsRequired && (value === null || $.trim(value) === '')) {
            formcontainer.addClass('hasError');
        } else {
            formcontainer.removeClass('hasError');
        }
        if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
            formcontainer.addClass('hasCloseError');
            alert("This field can not be cleared in a closed ticket");
            return;
        } else {
            formcontainer.removeClass('hasCloseError');
        }
        if (value === null || $.trim(value) === '') {
            formcontainer.addClass('isEmpty');
        } else {
            formcontainer.removeClass('isEmpty');
        }
        window.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function(result) {
            groupContainer.data('field', result);
            groupContainer.find('.external-link').remove();
            input.after(getUrls(result.Value));
            window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changecustom", userFullName);
        }, function() {
            alert("There was a problem saving your ticket property.");
        });
    });

    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        formcontainer.addClass('hasError');
    }
    if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
        formcontainer.addClass('hasCloseError');
    }
    if (field.IsRequiredToClose) {
        formcontainer.addClass('isRequiredToClose');
    }
    if (field.Value === null || $.trim(field.Value) === '') {
        formcontainer.addClass('isEmpty');
    }
}

var AddCustomFieldDate = function(field, parentContainer) {
    var date = field.Value == null ? null : window.parent.Ts.Utils.getMsDate(field.Value);
    var formcontainer = $('<div>').addClass('form-horizontal AddCustomFieldDate').appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('flexbox').data('field', field).appendTo(formcontainer);
    var labelContainer = $('<div>').addClass('flex1').appendTo(groupContainer);
    var formLabel = $('<div>').addClass('form-label').text(field.Name).appendTo(labelContainer);

    var dateContainer = $('<div>').addClass('flex2 ticket-input-container').attr('style', 'padding-top:3px;').appendTo(groupContainer);
    var dateLink = $('<a>').attr('href', '#').addClass('ticket-anchor ticket-nullable-link ticket-duedate-anchor').text((date === null ? 'unassigned' : date.localeFormat(window.parent.Ts.Utils.getDatePattern()))).appendTo(dateContainer);

    dateLink.click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        var header = $(this).hide();
        var container = $('<div>').insertAfter(header);
        var container1 = $('<div>').css('display', 'inline-block').attr('id', 'duedate-input').appendTo(container);
        var theinput = $('<input type="text">').val(date === null ? '' : date.localeFormat(window.parent.Ts.Utils.getDatePattern())).datetimepicker({
            pickTime: false
        }).appendTo(container1).focus();

        $('<i>').addClass('fa fa-times').click(function(e) {
            $(this).closest('div').remove();
            header.show();
        }).insertAfter(container1);

        $('<i>').addClass('fa fa-check').click(function(e) {
            var currDate = $(this).prev().find('input').val();
            var value = null;
            if (currDate !== '') {
                value = window.parent.Ts.Utils.getMsDate(currDate);
            }

            if (field.IsRequired && (value === null || $.trim(value) === '')) {
                formcontainer.addClass('hasError');
            } else {
                formcontainer.removeClass('hasError');
            }
            if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
                formcontainer.addClass('hasCloseErrory');
                alert("This field can not be cleared in a closed ticket");
                return;
            } else {
                formcontainer.removeClass('hasCloseErrory');
            }
            if (value === null || $.trim(value) === '') {
                formcontainer.addClass('isEmpty');
            } else {
                formcontainer.removeClass('isEmpty');
            }

            window.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function(result) {
                var date = result === null ? null : window.parent.Ts.Utils.getMsDate(result);
                dateLink.text((value === null ? 'unassigned' : value.localeFormat(window.parent.Ts.Utils.getDatePattern()))).show();
                window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changecustom", userFullName);
            }, function() {
                alert("There was a problem saving your ticket property.");
            });

            $(this).closest('div').remove();
            header.show();
        }).insertAfter(container1);
    });

    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        formcontainer.addClass('hasError');
    }
    if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
        formcontainer.addClass('hasCloseError');
    }
    if (field.IsRequiredToClose) {
        formcontainer.addClass('isRequiredToClose');
    }
    if (field.Value === null || $.trim(field.Value) === '') {
        formcontainer.addClass('isEmpty');
    }

}

var AddCustomFieldDateTime = function(field, parentContainer) {
    var date = field.Value == null ? null : window.parent.Ts.Utils.getMsDate(field.Value);
    var formcontainer = $('<div>').addClass('form-horizontal AddCustomFieldDateTime').appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('flexbox').data('field', field).appendTo(formcontainer);
    var labelContainer = $('<div>').addClass('flex1').appendTo(groupContainer);
    var formLabel = $('<div>').addClass('form-label').text(field.Name).appendTo(labelContainer);
    var dateContainer = $('<div>').addClass('flex2 ticket-input-container').attr('style', 'padding-top:3px;').appendTo(groupContainer);
    var dateLink = $('<a>').attr('href', '#').addClass('ticket-anchor ticket-nullable-link').text((date === null ? 'unassigned' : date.localeFormat(window.parent.Ts.Utils.getDateTimePattern()))).appendTo(dateContainer);

    dateLink.click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        var header = $(this).hide();
        var container = $('<div>').insertAfter(header);
        var container1 = $('<div>').css('display', 'inline-block').attr('id', 'duedate-input').appendTo(container);
        var theinput = $('<input type="text">').val(date === null ? '' : date.localeFormat(window.parent.Ts.Utils.getDateTimePattern())).datetimepicker({
            pickTime: true
        }).appendTo(container1).focus();

        $('<i>').addClass('fa fa-times').click(function(e) {
            $(this).closest('div').remove();
            header.show();
            $('#customerEdit').removeClass("disabled");
        }).insertAfter(container1);

        $('<i>').addClass('fa fa-check').click(function(e) {
            var currDate = $(this).prev().find('input').val();
            var value = null;
            if (currDate !== '') {
                value = window.parent.Ts.Utils.getMsDate(currDate);
            }
            if (field.IsRequired && (value === null || $.trim(value) === '')) {
                formcontainer.addClass('hasError');
            } else {
                formcontainer.removeClass('hasError');
            }
            if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
                formcontainer.addClass('hasCloseErrory');
                alert("This field can not be cleared in a closed ticket");
                return;
            } else {
                formcontainer.removeClass('hasCloseErrory');
            }
            if (value === null || $.trim(value) === '') {
                formcontainer.addClass('isEmpty');
            } else {
                formcontainer.removeClass('isEmpty');
            }

            window.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function(result) {
                var date = result === null ? null : window.parent.Ts.Utils.getMsDate(result);
                dateLink.text((value === null ? '' : value.localeFormat(window.parent.Ts.Utils.getDateTimePattern()))).show();
                window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changecustom", userFullName);
            }, function() {
                alert("There was a problem saving your ticket property.");
            });

            $(this).closest('div').remove();
            header.show();
        }).insertAfter(container1);
    });

    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        formcontainer.addClass('hasError');
    }
    if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
        formcontainer.addClass('hasCloseError');
    }
    if (field.IsRequiredToClose) {
        formcontainer.addClass('isRequiredToClose');
    }
    if (field.Value === null || $.trim(field.Value) === '') {
        formcontainer.addClass('isEmpty');
    }
}

var AddCustomFieldTime = function(field, parentContainer) {
    var date = field.Value == null ? null : window.parent.Ts.Utils.getMsDate(field.Value);
    var formcontainer = $('<div>').addClass('form-horizontal AddCustomFieldTime').appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('flexbox').data('field', field).appendTo(formcontainer);
    var labelContainer = $('<div>').addClass('flex1').appendTo(groupContainer);
    var formLabel = $('<div>').addClass('form-label').text(field.Name).appendTo(labelContainer);
    var dateContainer = $('<div>').addClass('flex2 ticket-input-container').attr('style', 'padding-top:3px;').appendTo(groupContainer);
    var dateLink = $('<a>').attr('href', '#').addClass('ticket-anchor ticket-nullable-link ticket-duedate-anchor').text((date === null ? 'Unassigned' : date.localeFormat(window.parent.Ts.Utils.getTimePattern()))).appendTo(dateContainer);

    dateLink.click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        var header = $(this).hide();
        var container = $('<div>').insertAfter(header);
        var container1 = $('<div>').css('display', 'inline-block').appendTo(container);
        var theinput = $('<input type="text">').val(date === null ? '' : date.localeFormat(window.parent.Ts.Utils.getTimePattern())).datetimepicker({
            pickDate: false
        }).appendTo(container1).focus();

        $('<i>').addClass('fa fa-times').click(function(e) {
            $(this).closest('div').remove();
            header.show();
            $('#customerEdit').removeClass("disabled");
        }).insertAfter(container1);

        $('<i>').addClass('fa fa-check').click(function(e) {
            var currDate = $(this).prev().find('input').val();
            var value = null;
            if (currDate !== '') {
                value = window.parent.Ts.Utils.getMsDate("1/1/1900 " + currDate);
            }
            if (field.IsRequired && (value === null || $.trim(value) === '')) {
                formcontainer.addClass('hasError');
            } else {
                formcontainer.removeClass('hasError');
            }
            if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
                formcontainer.addClass('hasCloseErrory');
                alert("This field can not be cleared in a closed ticket");
                return;
            } else {
                formcontainer.removeClass('hasCloseErrory');
            }
            if (value === null || $.trim(value) === '') {
                formcontainer.addClass('isEmpty');
            } else {
                formcontainer.removeClass('isEmpty');
            }

            window.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function(result) {
                var date = result === null ? null : window.parent.Ts.Utils.getMsDate(result);
                dateLink.text((value === null ? '' : value.localeFormat(window.parent.Ts.Utils.getTimePattern()))).show();
                window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changecustom", userFullName);
            }, function() {
                alert("There was a problem saving your ticket property.");
            });

            $(this).closest('div').remove();
            header.show();
        }).insertAfter(container1);
    });

    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        formcontainer.addClass('hasError');
    }
    if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
        formcontainer.addClass('hasCloseError');
    }
    if (field.IsRequiredToClose) {
        formcontainer.addClass('isRequiredToClose');
    }
    if (field.Value === null || $.trim(field.Value) === '') {
        formcontainer.addClass('isEmpty');
    }
}

var AddCustomFieldBool = function(field, parentContainer) {
    var formcontainer = $('<div>').addClass('form-horizontal AddCustomFieldBool').appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('flexbox').data('field', field).appendTo(formcontainer);
    var labelContainer = $('<div>').addClass('flex1').appendTo(groupContainer);
    var formLabel = $('<div>').addClass('form-label').text(field.Name).appendTo(labelContainer);

    var inputContainer = $('<div>').addClass('flex2 ticket-input-container').appendTo(groupContainer);
    var input = $('<input type="checkbox">').css('margin', '5px').appendTo(inputContainer);
    var value = (field.Value === null || $.trim(field.Value) === '' || field.Value.toLowerCase() === 'false' || field.Value.toLowerCase() === '0' ? false : true);
    input.prop("checked", value);
    input.change(function(e) {
        var isChecked = input.is(':checked')
        window.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, isChecked, function(result) {
            groupContainer.data('field', result);
            window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changecustom", userFullName);
        }, function() {
            alert("There was a problem saving your ticket property.");
        });
    });
}

var AddCustomFieldNumber = function(field, parentContainer) {
    var formcontainer = $('<div>').addClass('form-horizontal AddCustomFieldNumber').appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('flexbox').data('field', field).appendTo(formcontainer);
    var labelContainer = $('<div>').addClass('flex1').appendTo(groupContainer);
    var formLabel = $('<div>').addClass('form-label').text(field.Name).appendTo(labelContainer);

    var inputContainer = $('<div>').addClass('flex2 ticket-input-container').appendTo(groupContainer);
    var input = $('<input type="text">').addClass('ticket-simple-input muted-placeholder').attr("placeholder", "Enter Value").val(field.Value).appendTo(inputContainer).numeric();

    input.change(function(e) {
        var value = input.val();

        if (field.IsRequired && (value === null || $.trim(value) === '')) {
            formcontainer.addClass('hasError');
        } else {
            formcontainer.removeClass('hasError');
        }
        if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
            formcontainer.addClass('hasCloseError');
            alert("This field can not be cleared in a closed ticket");
            return;
        } else {
            formcontainer.removeClass('hasCloseError');
        }
        if (value === null || $.trim(value) === '') {
            formcontainer.addClass('isEmpty');;
        } else {
            formcontainer.removeClass('isEmpty');
        }
        window.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function(result) {
            groupContainer.data('field', result);
            window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changecustom", userFullName);
        }, function() {
            alert("There was a problem saving your ticket property.");
        });
    });

    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        formcontainer.addClass('hasError');
    }
    if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
        formcontainer.addClass('hasCloseError');
    }
    if (field.IsRequiredToClose) {
        formcontainer.addClass('isRequiredToClose');
    }
    if (field.Value === null || $.trim(field.Value) === '') {
        formcontainer.addClass('isEmpty');
    }
}

var AddCustomFieldSelect = function(field, parentContainer, loadConditionalFields) {
    var formcontainer = $('<div>').addClass('form-horizontal AddCustomFieldSelect').appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('flexbox').data('field', field).appendTo(formcontainer);
    var labelContainer = $('<div>').addClass('flex1').appendTo(groupContainer);
    var formLabel = $('<div>').addClass('form-label').text(field.Name).appendTo(labelContainer);

    var selectContainer = $('<div>').addClass('flex2 ticket-input-container').appendTo(groupContainer);
    var select = $('<select>').addClass('hidden-select muted-placeholder').attr("placeholder", "Select Value").appendTo(selectContainer);
    var options = field.ListValues.split('|');

    if (field.Value == "") {
        $('<option>').text("unassigned").val("").appendTo(select);
        if (field.IsRequired) {
            formcontainer.addClass('hasError');
        }
    }
    for (var i = 0; i < options.length; i++) {
        var optionValue = options[i];
        var option = $('<option>').text(optionValue).val(optionValue).appendTo(select);
        if (field.Value === options[i]) option.attr('selected', 'selected');
    }
    select.selectize({
        allowEmptyOption: true,
        onItemAdd: function(value, $item) {
            if (field.IsRequired && field.IsFirstIndexSelect == true && (value == "" || field.ListValues.split("|")[0] == value)) {
                formcontainer.addClass('hasError');
            } else {
                formcontainer.removeClass('hasError');
            }
            if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && field.IsFirstIndexSelect == true && value == "") {
                formcontainer.addClass('hasCloseError');
                alert("This field can not be cleared in a closed ticket");
                return;
            } else {
                formcontainer.removeClass('hasCloseError');
            }
            if (field.IsFirstIndexSelect == true && value == "") {
                formcontainer.addClass('isEmpty');
            } else {
                formcontainer.removeClass('isEmpty');
            }
            window.parent.Ts.System.logAction('Ticket - Custom Value Set');
            window.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function(result) {
                $('.' + field.CustomFieldID + 'children').remove();
                var childrenContainer = $('<div>').addClass(field.CustomFieldID + 'children').insertAfter(formcontainer);
                appendMatchingParentValueFields(childrenContainer, result);
                window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changecustom", userFullName);
            }, function() {
                alert("There was a problem saving your ticket property.");
            });
        },
        onDropdownClose: function($dropdown) {
            $($dropdown).prev().find('input').blur();
        },
        closeAfterSelect: true
    });

    var items = field.ListValues.split('|');
    if (field.IsRequired && ((field.IsFirstIndexSelect == true && (items[0] == field.Value || field.Value == null || $.trim(field.Value) === '')) || (field.Value == null || $.trim(field.Value) === ''))) {
        formcontainer.addClass('hasError');
    }
    if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && ((field.IsFirstIndexSelect == true && (items[0] == field.Value || field.Value == null || $.trim(field.Value) === '')) || (field.Value == null || $.trim(field.Value) === ''))) {
        formcontainer.addClass('hasCloseError');
    }
    if (field.IsRequiredToClose) {
        formcontainer.addClass('isRequiredToClose');
    }
    if ((field.IsFirstIndexSelect == true && items[0] == field.Value) || field.Value == null || $.trim(field.Value) === '') {
        formcontainer.addClass('isEmpty');
    }

    $('.' + field.CustomFieldID + 'children').remove();
    var childrenContainer = $('<div>').addClass(field.CustomFieldID + 'children form-horizontal').insertAfter(formcontainer);
    appendMatchingParentValueFields(childrenContainer, field);
}

var SetupDueDateField = function(duedate) {
    var dateContainer = $('#ticket-duedate-container');
    var dateLink = $('<a>').attr('href', '#').attr('id', 'ticket-duedate').addClass('ticket-anchor ticket-nullable-link ticket-duedate-anchor').appendTo(dateContainer);

    if (duedate !== null) {
        dateLink.text(duedate.localeFormat(window.parent.Ts.Utils.getDateTimePattern()));
        if (duedate < new Date()) {
            dateLink.addClass('nonrequired-field-error-font');
        }
    }

    dateLink.click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        var header = $(this).hide();
        var container = $('<div>').insertAfter(header);
        var container1 = $('<div>').css('display', 'inline-block').attr('id', 'duedate-input').appendTo(container);
        var theinput = $('<input type="text">').val('').datetimepicker({
            pickTime: true
        }).appendTo(container1).focus();

        $('<i>').addClass('fa fa-times').click(function(e) {
            $(this).closest('div').remove();
            header.show();
            $('#customerEdit').removeClass("disabled");
        }).insertAfter(container1);

        $('<i>').addClass('fa fa-check').click(function(e) {
            var currDate = $(this).prev().find('input').val();
            var value = '';
            if (currDate !== '') {
                value = window.parent.Ts.Utils.getMsDate(currDate);
            }
            window.parent.Ts.Services.Tickets.SetDueDate(_ticketID, value, function(result) {
                var date = result === null ? null : window.parent.Ts.Utils.getMsDate(result);
                dateLink.text((value === '' ? '' : value.localeFormat(window.parent.Ts.Utils.getDateTimePattern()))).show();
                duedate = value === '' ? null : window.parent.Ts.Utils.getMsDate(value); //result;
                if (date != null && date < Date.now()) {
                    dateLink.addClass('nonrequired-field-error-font');
                } else {
                    dateLink.removeClass('nonrequired-field-error-font');
                }
                window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changeduedate", userFullName);
            }, function() {
                alert("There was a problem saving your ticket property.");
            });
            $(this).closest('div').remove();
            header.show();
        }).insertAfter(container1);
    });
}

var SetupStatusField = function(StatusId) {
    var statuses = window.parent.Ts.Cache.getNextStatuses(StatusId);
    _ticketCurrStatus = StatusId;
    if ($('#ticket-status').length) {
        $("#ticket-status").selectize({
            onDropdownClose: function($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true,
            onChange: function(value) {
                if (value !== _ticketCurrStatus.toString()) {
                    var status = window.parent.Ts.Cache.getTicketStatus(value);
                    isFormValidToClose(status.IsClosed, function(isValid) {
                        if (isValid == true) {
                            window.parent.Ts.Services.Tickets.SetTicketStatus(_ticketID, value, function(result) {
                                    if (result !== null) {
                                        _ticketCurrStatus = result.TicketStatusID;
                                        //SetStatus(null);
                                        window.parent.Ts.System.logAction('Ticket - Status Changed');
                                        $('#ticket-status-label').toggleClass('ticket-closed', result.IsClosed);
                                        _ticketInfo.IsSlaPaused = status.PauseSLA;
                                        resetSLAInfo();
                                        slaCheckTimer = setInterval(RefreshSlaDisplay, 5000);
                                        window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "changestatus", userFullName);
                                    }
                                },
                                function(error) {
                                    SetStatus(_ticketCurrStatus);
                                    alert('There was an error setting your ticket status.');
                                });
                        } else {
                            SetStatus(_ticketCurrStatus);
                            alert("Please fill in the required fields before closing the ticket.");
                            return;
                        }
                    });
                }
            },
            render: {
                item: function(item, escape) {
                    if (item.data.IsClosed) {
                        return '<div data-value="' + escape(item.value) + '" data-item="' + escape(item.data) + '" data-selectable="" class="option"><s>' + escape(item.text) + '</s></div>';
                    } else {
                        return '<div data-value="' + escape(item.value) + '" data-item="' + escape(item.data) + '" data-selectable="" class="option">' + escape(item.text) + '</div>';
                    }
                }
            },
        });
        var selectize = $("#ticket-status")[0].selectize;
        if (statuses) {
            for (var i = 0; i < statuses.length; i++) {
                if (statuses[i]) {
                    selectize.addOption({
                        value: statuses[i].TicketStatusID,
                        text: statuses[i].Name,
                        data: statuses[i]
                    });
                }
            }
        }
        selectize.addItem(StatusId, true);
    }
}

var SetupJiraFields = function() {
    $('#newJiraIssue').click(function(e) {
        e.preventDefault();
        $('.ts-jira-buttons-container').hide();
        var errorMessage = "There was an error setting your Jira Issue Key. Please contact TeamSupport.com";
        window.parent.Ts.Services.Tickets.SetSyncWithJira(_ticketID, function(result) {
            if (result != null) {
                var syncResult = JSON.parse(result);
                if (syncResult.IsSuccessful === true) {
                    $('#issueKeyValue').text('Pending...');
                    $('#issueKey').show();
                } else {
                    $('.ts-jira-buttons-container').show();
                    $('#issueKey').hide();
                    alert(syncResult.Error);
                }
            } else {
                alert(errorMessage);
            }
        },
        function(error) {
            $('.ts-jira-buttons-container').show();
            $('#issueKey').hide();
            alert(errorMessage);
        });
    });

    $('#existingJiraIssue').click(function(e) {
        e.preventDefault();
        $('.ts-jira-buttons-container').hide();
        $('#enterIssueKey').show();
        $('#issueKeyInput').focus();
    });

    $('#cancelIssueyKeyButton').click(function(e) {
        $('.ts-jira-buttons-container').show();
        $('#enterIssueKey').hide();
    });

    $('#saveIssueKeyButton').click(function(e) {
        if ($.trim($('#issueKeyInput').val()) === '') {
            $('.ts-jira-buttons-container').show();
            $('#enterIssueKey').hide();
        } else {
            $('#issueKeyValue').text($.trim($('#issueKeyInput').val()));
            $('#enterIssueKey').hide();
            $('#issueKey').show();
            var errorMessage = "There was an error setting your Jira Issue Key. Please contact TeamSupport.com";
            window.parent.Ts.Services.Tickets.SetJiraIssueKey(_ticketID, $.trim($('#issueKeyInput').val()), function(result) {
                if (result != null) {
                    var syncResult = JSON.parse(result);
                    if (syncResult.IsSuccessful === false) {
                        $('.ts-jira-buttons-container').show();
                        $('#issueKey').hide();
                        alert(syncResult.Error);
                    }
                } else {
                    alert(errorMessage);
                }
            },
            function(error) {
                $('.ts-jira-buttons-container').show();
                $('#issueKey').hide();
                alert(errorMessage);
            });
        }
    });

    $('#jiraUnlink').click(function(e) {
        var currentStatus = $("#issueKeyValue").text().toLowerCase();
        var confirmMessage = "Are you sure you want to " + ((currentStatus.indexOf("pending") > -1) ? "cancel" : "remove") + " link to JIRA?";

        if (confirm(confirmMessage)) {
            e.preventDefault();
            window.parent.Ts.Services.Tickets.UnSetSyncWithJira(_ticketID, function(result) {
                if (result === true) {
                    $('.ts-jira-buttons-container').show();
                    $('#issueKey').hide();
                } else {
                    alert('There was an error setting your Jira Issue Key. Please try again later');
                    $('.ts-jira-buttons-container').hide();
                    $('#issueKey').show();
                }
            },
            function(error) {
                alert('There was an error setting your Jira Issue Key.');
                $('.ts-jira-buttons-container').hide();
                $('#issueKey').show();
            });
        }
    });
}

var SetupJiraFieldValues = function() {
    window.parent.Ts.Services.Admin.GetJiraInstanceNameForTicket(_ticketID, function(result) {
        if (result.length > 0) {
            $('#ticket-jirafields').show();

            if (_ticketInfo.LinkToJira != null) {
                if (!_ticketInfo.LinkToJira.JiraKey) {
                    $('#issueKeyValue').text('Pending...');
                } else if (!_ticketInfo.LinkToJira.JiraLinkURL) {
                    $('#issueKeyValue').text(_ticketInfo.LinkToJira.JiraKey);
                    if (_ticketInfo.LinkToJira.JiraKey.indexOf('Error') > -1) {
                        $('#issueKeyValue').closest('.form-group').addClass('fieldError');
                    } else {
                        $('#issueKeyValue').closest('.form-group').addClass('fieldError');
                    }
                } else {
                    if ($(".jiraLink").length) {
                        $(".jiraLink").remove();
                    }
                    var jiraLink = $('<a>').attr('href', _ticketInfo.LinkToJira.JiraLinkURL).attr('target', '_blank').attr('title', result + ' instance').text(_ticketInfo.LinkToJira.JiraKey).addClass('jiraLink ticket-anchor').prependTo($('#ticket-jirakey-container'));
                }

                $('#issueKey').show();
                $('.ts-jira-buttons-container').hide();
            } else {
                $('#ticket-jirafields').show();
                $('#issueKey').hide();
                $('.ts-jira-buttons-container').show();
                $('#newJiraIssue').attr('title', result + ' instance');
            }
        } else {
            $('#ticket-jirafields').hide();
        }
    });
}

var SetupTFSFields = function() {
    $('#newTFSWorkItem').click(function(e) {
        e.preventDefault();
        $('.ts-tfs-buttons-container').hide();
        var errorMessage = "There was an error setting your TFS Work Item Title. Please contact TeamSupport.com";
        window.parent.Ts.Services.Tickets.SetSyncWithTFS(_ticketID, function(result) {
            if (result != null) {
                var syncResult = JSON.parse(result);
                if (syncResult.IsSuccessful === true) {
                    $('#workItemTitleValue').text('Pending...');
                    $('#workItemTitle').show();
                } else {
                    $('.ts-tfs-buttons-container').show();
                    $('#workItemTitle').hide();
                    alert(syncResult.Error);
                }
            } else {
                alert(errorMessage);
            }
        },
        function(error) {
            $('.ts-tfs-buttons-container').show();
            $('#workItemTitle').hide();
            alert(errorMessage);
        });
    });

    $('#existingTFSWorkItem').click(function(e) {
        e.preventDefault();
        $('.ts-tfs-buttons-container').hide();
        $('#enterWorkItemTitle').show();
        $('#workItemIDInput').focus();
    });

    $('#cancelWorkItemTitleButton').click(function(e) {
        $('.ts-tfs-buttons-container').show();
        $('#enterWorkItemTitle').hide();
    });

    $('#saveWorkItemTitleButton').click(function(e) {
        if ($.trim($('#workItemIDInput').val()) === '') {
            $('.ts-tfs-buttons-container').show();
            $('#enterWorkItemTitle').hide();
        } else {
            $('#workItemTitleValue').text($.trim($('#workItemIDInput').val()));
            $('#enterWorkItemTitle').hide();
            $('#workItemTitle').show();
            var errorMessage = "There was an error setting your Work Item Title. Please contact TeamSupport.com";

            window.parent.Ts.Services.Tickets.SetTFSWorkItemID(_ticketID, $.trim($('#workItemIDInput').val()), function(result) {
                if (result != null) {
                    var syncResult = JSON.parse(result);
                    if (syncResult.IsSuccessful === false) {
                        $('.ts-tfs-buttons-container').show();
                        $('#workItemTitle').hide();
                        alert(syncResult.Error);
                    }
                } else {
                    alert(errorMessage);
                }
            },
            function(error) {
                $('.ts-tfs-buttons-container').show();
                $('#workItemTitle').hide();
                alert(errorMessage);
            });
        }
    });

    $('#tfsUnlink').click(function(e) {
        var currentStatus = $("#workItemTitleValue").text().toLowerCase();
        var confirmMessage = "Are you sure you want to " + ((currentStatus.indexOf("pending") > -1) ? "cancel" : "remove") + " link to TFS?";

        if (confirm(confirmMessage)) {
            e.preventDefault();
            window.parent.Ts.Services.Tickets.UnSetSyncWithTFS(_ticketID, function(result) {
                if (result === true) {
                    $('.ts-tfs-buttons-container').show();
                    $('#workItemTitle').hide();
                } else {
                    alert('There was an error setting your TFS Work Item Title. Please try again later');
                    $('.ts-tfs-buttons-container').hide();
                    $('#workItemTitle').show();
                }
            },
            function(error) {
                alert('There was an error setting your TFS Work Item Title.');
                $('.ts-tfs-buttons-container').hide();
                $('#workItemTitle').show();
            });
        }
    });
}

var SetupTFSFieldValues = function() {
    window.parent.Ts.Services.Admin.GetTFSCRMLinkTableRecordForTicket(_ticketID, function(result) {
        if (result.length > 0) {
            $('#ticket-tfsfields').show();
            if (_ticketInfo.LinkToTFS != null) {
                if (!_ticketInfo.LinkToTFS.TFSTitle) {
                    $('#workItemTitleValue').text('Pending...');
                } else if (!_ticketInfo.LinkToTFS.TFSURL) {
                    $('#workItemTitleValue').text(_ticketInfo.LinkToTFS.TFSTitle);
                    if (_ticketInfo.LinkToTFS.TFSTitle.indexOf('Error') > -1) {
                        $('#workItemTitleValue').closest('.form-group').addClass('fieldError');
                    } else {
                        $('#workItemTitleValue').closest('.form-group').addClass('fieldError');
                    }
                } else {
                    if ($(".tfsLink").length) {
                        $(".tfsLink").remove();
                    }
                    var tfsLink = $('<a>').attr('href', _ticketInfo.LinkToTFS.TFSURL).attr('target', '_blank').text(_ticketInfo.LinkToTFS.TFSTitle).addClass('tfsLink ticket-anchor').prependTo($('#ticket-workItemTitle-container'));
                }
                $('#workItemTitleValue').show();
                $('.ts-tfs-buttons-container').hide();
            } else {
                $('#ticket-tfsfields').show();
                $('#workItemTitle').hide();
                $('.ts-tfs-buttons-container').show();
            }
        } else {
            $('#ticket-tfsfields').hide();
        }
    });
}

function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

//click events and logic
var SetupSnowFields = function() {
    $('#newSnowIncident').click(function(e) {
        e.preventDefault();
        $('.ts-snow-buttons-container').hide();
        var errorMessage = "There was an error setting your ServiceNow Incident Number. Please contact TeamSupport.com";
        window.parent.Ts.Services.Tickets.SetSyncWithSnow(_ticketID, function(result) {
            if (result != null) {
                var syncResult = JSON.parse(result);
                if (syncResult.IsSuccessful === true) {
                    $('#incidentNumberValue').text('Pending...');
                    $('#incidentNumber').show();
                } else {
                    $('.ts-snow-buttons-container').show();
                    $('#incidentNumber').hide();
                    alert(syncResult.Error);
                }
            } else {
                alert(errorMessage);
            }
        },
        function(error) {
            $('.ts-snow-buttons-container').show();
            $('#incidentNumber').hide();
            alert(errorMessage);
        });
    });

    $('#existingSnowIncident').click(function(e) {
        e.preventDefault();
        $('.ts-snow-buttons-container').hide();
        $('#enterIncident').show();
        $('#incidentNumberInput').focus();
    });

    $('#cancelIncidentNumberButton').click(function(e) {
        $('.ts-snow-buttons-container').show();
        $('#enterIncident').hide();
    });

    $('#saveIncidentNumberButton').click(function(e) {
        if ($.trim($('#incidentNumberInput').val()) === '') {
            $('.ts-snow-buttons-container').show();
            $('#enterIncident').hide();
        } else {
            $('#incidentNumberValue').text($.trim($('#incidentNumberInput').val()));
            $('#enterIncident').hide();
            $('#incidentNumber').show();
            var errorMessage = "There was an error setting your Incident Number. Please contact TeamSupport.com";

            window.parent.Ts.Services.Tickets.SetSnowWorkItemID(_ticketID, $.trim($('#incidentNumberInput').val()), function(result) {
                if (result != null) {
                    var syncResult = JSON.parse(result);
                    if (syncResult.IsSuccessful === false) {
                        $('.ts-snow-buttons-container').show();
                        $('#incidentNumber').hide();
                        alert(syncResult.Error);
                    }
                } else {
                    alert(errorMessage);
                }
            },
            function(error) {
                $('.ts-snow-buttons-container').show();
                $('#incidentNumber').hide();
                alert(errorMessage);
            });
        }
    });

    $('#snowUnlink').click(function(e) {
        var currentStatus = $("#incidentNumberValue").text().toLowerCase();
        var confirmMessage = "Are you sure you want to " + ((currentStatus.indexOf("pending") > -1) ? "cancel" : "remove") + " link to ServiceNow?";

        if (confirm(confirmMessage)) {
            e.preventDefault();
            window.parent.Ts.Services.Tickets.UnSetSyncWithSnow(_ticketID, function(result) {
                if (result === true) {
                    $('.ts-snow-buttons-container').show();
                    $('#incidentNumber').hide();
                } else {
                    alert('There was an error setting your Incident Number. Please try again later');
                    $('.ts-snow-buttons-container').hide();
                    $('#incidentNumber').show();
                }
            },
            function(error) {
                alert('There was an error setting your Incident Number.');
                $('.ts-snow-buttons-container').hide();
                $('#incidentNumber').show();
            });
        }
    });
};

//Load and display the proper ServiceNow fields/values
var SetupSnowFieldValues = function() {
    window.parent.Ts.Services.Admin.GetSnowCRMLinkTableRecordForTicket(_ticketID, function(result) {
        if (result.length > 0) {
            $('#ticket-snowfields').show();
            if (_ticketInfo.LinkToSnow != null) {
                if (!_ticketInfo.LinkToSnow.Number) {
                    $('#incidentNumberValue').text('Pending...');
                } else if (!_ticketInfo.LinkToSnow.URL) {
                    $('#incidentNumberValue').text(_ticketInfo.LinkToSnow.Number);
                    if (_ticketInfo.LinkToSnow.Number.indexOf('Error') > -1) {
                        $('#incidentNumberValue').closest('.form-group').addClass('fieldError');
                    }
                } else {
                    if ($(".snowLink").length) {
                        $(".snowLink").remove();
                    }
                    var snowLink = $('<a>').attr('href', _ticketInfo.LinkToSnow.URL).attr('target', '_blank').text(_ticketInfo.LinkToSnow.Number).addClass('snowLink control-label ticket-anchor').prependTo($('#ticket-incident-container'));
                }
                $('#incidentNumberValue').show();
                $('.ts-snow-buttons-container').hide();
            } else {
                $('#ticket-snowfields').show();
                $('#incidentNumber').hide();
                $('.ts-snow-buttons-container').show();
            }
        } else {
            $('#ticket-snowfields').hide();
        }
    });
};

var getUrls = function(input) {
    var source = (input || '').toString();
    var parentDiv = $('<div>').addClass('input-group-addon external-link')
    var url;
    var matchArray;
    var result = '';
    // Regular expression to find FTP, HTTP(S) and email URLs. Updated to include urls without http
    var regexToken = /(((ftp|https?|www):?\/?\/?)[\-\w@:%_\+.~#?,&\/\/=]+)|((mailto:)?[_.\w-]+@([\w][\w\-]+\.)+[a-zA-Z]{2,3})/g;
    // Iterate through any URLs in the text.
    while ((matchArray = regexToken.exec(source)) !== null) {
        url = matchArray[0];
        if (url.length > 2 && url.substring(0, 3) == 'www') {
            url = 'http://' + url;
        }
        if (validateEmail(url)) {
            url = "mailto:" + url;
        }
        result = result + '<a target="_blank" class="valueLink" href="' + url + '" title="' + matchArray[0] + '"><i class="fa fa-external-link fa-lg custom-field-link"></i></a>';
    }

    if (result !== '') {
        return parentDiv.append(result);
    }
    return result;
}

var LoadTicketHistory = function() {
    window.parent.Ts.Services.Tickets.GetTicketHistory(_ticketID, function(logs) {
        var historyTable = $('#ticket-history-table > tbody');
        historyTable.empty().addClass('ts-loading');
        for (var i = 0; i < logs.length; i++) {
            var row = $('<tr>').appendTo(historyTable);
            var col1 = $('<td>').text(logs[i].CreatorName).appendTo(row);
            var col2 = $('<td>').text(logs[i].DateCreated.localeFormat(window.parent.Ts.Utils.getDateTimePattern())).appendTo(row);
            var col3 = $('<td>').html(logs[i].Description).appendTo(row);
        }
        historyTable.removeClass('ts-loading');
    }, function() {
        alert('There was a problem retrieving the history for the ticket.');
    });
}

function openTicketWindow(ticketID) {
    window.parent.Ts.MainPage.openTicket(ticketID, true);
}

function FetchPinned() {
    window.parent.Ts.Services.TicketPage.getPinned(_ticketID, function(returned) {
        if (returned.length > 0) {
            var pinned = returned[0];
            pinned.pinning = 'yes';
            var template = Handlebars.templates['action'];
            var html = template(pinned);
            $(html).find('a').attr('target', '_blank');
            $("#pinned-placeholder").html(html);
        }
    });
}

function FetchTimeLineItems(start) {
    _isLoading = true;
    $('.results-loading').show();
    window.parent.Ts.Services.TicketPage.GetTimeLineItems(_ticketID, start, function(TimeLineItems) {
        _timeLine = TimeLineItems;
        if (TimeLineItems.length < 1) {
            $('.results-loading').hide();
            $('.results-done').show();
        } else {
            _compiledActionTemplate = Handlebars.templates['action'];
            var isPublicFiltered = $('.filter-public').hasClass('bgcolor-darkgray');
            var isPrivateFiltered = $('.filter-private').hasClass('bgcolor-darkgray');
            var isWCFiltered = $('.filter-wc').hasClass('bgcolor-darkgray');
            for (i = 0; i < _timeLine.length; i++) {
                var timeLineItem = _timeLine[i];
                var actionElem = CreateActionElement(timeLineItem, !timeLineItem.item.IsPinned);
                if (isPublicFiltered && timeLineItem.item.IsVisibleOnPortal) {
                    actionElem.hide();
                } else if (isPrivateFiltered && !timeLineItem.item.IsVisibleOnPortal) {
                    actionElem.hide();
                } else if (isWCFiltered && timeLineItem.item.IsWC) {
                    actionElem.hide();
                } else if (i === 0 && _timeLine[0].item.IsPinned) {
                    actionElem.hide();
                }
            }
            _isLoading = false;
            maxwidth();
            $('.results-loading').hide();
        };
    });
}

function CreateActionElement(val, ShouldAppend) {
    if (val.item.IsWC) {
        val.item.Message = val.item.Message.replace(/\n\r?/g, '<br />');
        for (wc = 0; wc < val.WaterCoolerReplies.length; wc++) {
            var wcmsgtext = val.WaterCoolerReplies[wc].WaterCoolerReplyProxy.Message;
            val.WaterCoolerReplies[wc].WaterCoolerReplyProxy.Message = wcmsgtext.replace(/\n\r?/g, '<br />');
        }
    }
    if (_currDateSpan == null || _currDateSpan.toDateString() !== val.item.DateCreated.toDateString()) {
        if (!val.item.IsPinned) {
            var dateSpan = '<div class="daystrip"><span class="daybadge">' + val.item.DateCreated.localeFormat(window.parent.Ts.Utils.getDatePattern()) + '</span><div>';
            _currDateSpan = val.item.DateCreated;
        }
    }
    var html = _compiledActionTemplate(val);
    var actionElement = $(html);
    actionElement.find('a').attr('target', '_blank');
    if (ShouldAppend || val.item.IsPinned) {
        $("#action-timeline").append(dateSpan);
        $("#action-timeline").append(actionElement);
    } else {
        $('.action-placeholder').after(actionElement);
        $('.action-placeholder').after(dateSpan);
    }
    if (val.item.IsPinned) {
        // fvar actionCloned = $(actionElement).clone();
        // $("#pinned-placeholder").html(actionCloned);
    }
    _isCreatingAction = false;
    return actionElement;
}

function UpdateActionElement(val) {
    if (_currDateSpan.toDateString() !== val.item.DateCreated.toDateString()) {
        var dateSpan = '<div class="daystrip"><span class="daybadge">' + val.item.DateCreated.localeFormat(window.parent.Ts.Utils.getDatePattern()) + '</span></div>';
        $("#action-timeline").append(dateSpan);
        _currDateSpan = val.item.DateCreated;
    }
    var html = _compiledActionTemplate(val);
    var actionElement = $(html);
    var li = $("#action-timeline div.action[data-id=" + val.item.RefID + "]");
    var actionNumber = li.find('.ticket-action-number').text();
    try {
        li.replaceWith(html);
    } catch (e) {}
    $("#action-timeline div.action[data-id=" + val.item.RefID + "]").find('.ticket-action-number').text(actionNumber);
}

function CreateHandleBarHelpers() {

    Handlebars.registerHelper('WaterCoolerRelationships', function() {
        if (this.WatercoolerReferences) {
            return BuildWaterCoolerAssociationToolTip(this.WatercoolerReferences);
        } else {
            return "";
        }
    });

    Handlebars.registerHelper('UserImageTag', function() {
        if (this.item.CreatorID > 0) {
            return '<img class="user-avatar pull-left" src="/dc/' + this.item.OrganizationID + '/UserAvatar/' + this.item.CreatorID + '/120/' + new Date().getTime() + '" />';
        } else {
            return "";
        }
    });

    Handlebars.registerHelper('FormatDateTime', function(Date) {
        return Date.localeFormat(window.parent.Ts.Utils.getDateTimePattern())
    });

    Handlebars.registerHelper('TimeLineLabel', function() {
        if (this.item.IsVisibleOnPortal) {
            return '<div class="bgcolor-green"><span class="bgcolor-green">&nbsp;</span><a href="#" data-id="' + this.item.RefID + '" class="action-option-visible">Public</a></div>';
        } else if (!this.item.IsWC) {
            return '<div class="bgcolor-orange"><span class="bgcolor-orange">&nbsp;</span><a href="#" data-id="' + this.item.RefID + '" class="action-option-visible">Private</a></div>';
        } else if (this.item.IsWC) {
            return '<div class="bgcolor-blue"><span class="bgcolor-blue">&nbsp;</span><label>WC</label></div>';
        } else {
            return;
        }
    });

    Handlebars.registerHelper('Watson', function() {
        if (this.item.IsWC) {
            return;
        }
        //if (this.item.OrganizationID != '1078') { return; }
        var ticketID = this.item.TicketID;
		var actionID = this.item.RefID;

		if (window.parent.Ts.System.Organization.UseWatson) {
			var output = window.parent.Ts.Services.WatsonTickets.Action(ticketID, actionID, function (result) {
				if (result != 'negative' && result != 'nothing' && result != 'hidden') {
					var data = jQuery.parseJSON(result);
					var sentiments = {
						1: 'Sad',
						2: 'Frustrated',
						3: 'Satisfied',
						4: 'Excited',
						5: 'Polite',
						6: 'Impolite',
						7: 'Sympathetic'
					}
					var display = [];
					$.each(data.watson, function (key, sentiment) {
						if (sentiment.SentimentID > 0) {
							var emotion = sentiments[sentiment.SentimentID];
							var percent = Math.round(sentiment.SentimentScore * 100);
							display.push(emotion + ': ' + percent + '%');
						}
					});
					$('#watson-' + actionID).text(display.join(', '));
				}
			});
		} else {
			$('#ticket-Sentiment').parent().parent().parent().hide();
		}
    });

    Handlebars.registerHelper('Applause', function() {
        if (this.item.IsWC) {
            return;
        }
        var ticketID = this.item.TicketID;
        var actionID = this.item.RefID;
        var display = (this.item.OrganizationID === window.parent.Ts.System.User.OrganizationID && !this.item.IsWC) ? 'inline' : 'none';
        var output = window.parent.Ts.Services.TicketPage.PullReactions(ticketID, actionID, function(result) {
            if (result != 'negative' && result != 'nothing' && result != 'hidden') {
                var data = jQuery.parseJSON(result);
                var tally = data[0].reactions[0].tally;
                var reckoning = data[1].validation[0].reckoning;
                var opacity = (reckoning > 0) ? '1' : '0.2';
                var oldvalue = (reckoning > 0) ? '1' : '0';
                var thacode = '<span id="tally-' + actionID + '" class="listreactions" data-actionid="' + actionID + '" data-ticketid="' + ticketID + '">' + tally + '</span><a href="#" class="updatereaction" data-actionid="' + actionID + '" data-ticketid="' + ticketID + '" data-oldvalue="' + oldvalue + '" style="opacity:' + opacity + ';"><img src="/vcr/480/Images/icons/applause.png" style="margin-left:5px;height:24px;"></a>';
                $('#applause-' + actionID).html(thacode);
            }
        });
        return '<span id="applause-' + actionID + '" class="pull-right" style="display:' + display + '"></span>';
    });

    Handlebars.registerHelper('ActionData', function() {
        return JSON.stringify(this.item);
    });

    Handlebars.registerHelper('TagData', function() {
        return JSON.stringify(this.data);
    });

    Handlebars.registerHelper('ActionNumber', function() {
        if (this.item.IsPinned && this.pinning) {
            var ticketID = this.item.TicketID;
            var actionID = this.item.RefID;
            var output = window.parent.Ts.Services.TicketPage.getPosition(ticketID, actionID, function(result) {
                if (result != 'negative' && result != 'nothing') {
                    var data = jQuery.parseJSON(result);
                    $('#action-number-' + actionID).text(data.position[0].position)
                }
            });
        } else if (!_isCreatingAction) {
            _workingActionNumer = _workingActionNumer - 1;
            return _workingActionNumer + 1;
            // return _workingActionNumer;
        }
    });

    Handlebars.registerHelper('CanPin', function(options) {
        if (window.parent.Ts.System.User.UserCanPinAction || window.parent.Ts.System.User.IsSystemAdmin) {
            return options.fn(this);
        }
    });

    Handlebars.registerHelper('CanEdit', function(options) {
        var action = this.item;
        var canEdit = window.parent.Ts.System.User.IsSystemAdmin || window.parent.Ts.System.User.UserID === action.CreatorID;
        var restrictedFromEditingAnyActions = !window.parent.Ts.System.User.IsSystemAdmin && window.parent.Ts.System.User.RestrictUserFromEditingAnyActions;

        if (!(!window.parent.Ts.System.User.AllowUserToEditAnyAction && (!canEdit || restrictedFromEditingAnyActions))) {
            return options.fn(this);
        }
    });

    Handlebars.registerHelper('CanKB', function(options) {
        if (window.parent.Ts.System.User.ChangeKbVisibility || window.parent.Ts.System.User.IsSystemAdmin) {
            return options.fn(this);
        }
    });

    Handlebars.registerHelper('CanMakeVisible', function(options) {
        if (window.parent.Ts.System.User.ChangeTicketVisibility || window.parent.Ts.System.User.IsSystemAdmin) {
            return options.fn(this);
        }
    });

    Handlebars.registerHelper('TimeSpent', function() {
        var hours = Math.floor(this.item.TimeSpent / 60);
        var mins = Math.floor(this.item.TimeSpent % 60);
        var timeSpentString = "";
        if (hours > 0) timeSpentString = hours + ((hours > 1) ? " hours " : " hour ");
        if (mins > 0) timeSpentString += mins + ((mins > 1) ? " minutes " : " minute ");
        if (timeSpentString == "") {
            return "";
        } else {
            if (this.item.DateStarted !== null) {
                var time = this.item.DateStarted.localeFormat(window.parent.Ts.Utils.getDateTimePattern())
                return timeSpentString + " - " + time
            }
        }
        return timeSpentString;
    });


    Handlebars.registerHelper('WCLikes', function() {
        if (this.Likes > 0) {
            return "+" + this.Likes;
        }
    });

    Handlebars.registerHelper("taskComplete", function(isComplete) {
        return isComplete == true ? ' checked' : '';
    });
}

function BuildWaterCoolerAssociationToolTip(references) {
    var tixatt = references.Tickets;
    var tixattstr = "";
    var tixHasAtt = false;
    if (tixatt.length > 0) {
        tixHasAtt = true;
        for (var i = 0; i < tixatt.length; i++) {
            tixattstr = tixattstr + ' ' + tixatt[i].CreatorName + ' added ticket <a href="' + window.parent.Ts.System.AppDomain + '?TicketNumber=' + tixatt[i].AttachmentID + '" target="_blank" onclick="window.parent.Ts.MainPage.openTicket(' + tixatt[i].AttachmentID + '); return false;">' + tixatt[i].TicketName + '</a><br/>';
        }
    }

    var tixgrp = references.Groups;
    var tixgrpstr = "";
    if (tixgrp.length > 0) {
        tixHasAtt = true;
        for (var i = 0; i < tixgrp.length; i++) {
            tixgrpstr = tixgrpstr + ' ' + tixgrp[i].CreatorName + ' added group <a href="#" target="_blank" onclick="window.parent.Ts.MainPage.openGroup(' + tixgrp[i].AttachmentID + '); return false;">' + tixgrp[i].GroupName + '</a><br/>';
        }
    }

    var tixprod = references.Products;
    var tixprodstr = "";
    if (tixprod.length > 0) {
        tixHasAtt = true;
        for (var i = 0; i < tixprod.length; i++) {
            tixprodstr = tixprodstr + ' ' + tixprod[i].CreatorName + ' added product <a href="#" target="_blank" onclick="window.parent.Ts.MainPage.openNewProduct(' + tixprod[i].AttachmentID + '); return false;">' + tixprod[i].ProductName + '</a><br/>';
        }
    }

    var tixcompany = references.Company;
    var tixcompanystr = "";
    if (tixcompany.length > 0) {
        tixHasAtt = true;
        for (var i = 0; i < tixcompany.length; i++) {
            tixcompanystr = tixcompanystr + ' ' + tixcompany[i].CreatorName + ' added company <a href="#" target="_blank" onclick="window.parent.Ts.MainPage.openNewCustomer(' + tixcompany[i].AttachmentID + '); return false;">' + tixcompany[i].CompanyName + '</a><br/>';
        }
    }

    var tixuser = references.User;
    var tixuserstr = "";
    if (tixuser.length > 0) {
        tixHasAtt = true;
        for (var i = 0; i < tixuser.length; i++) {
            tixuserstr = tixuserstr + ' ' + tixuser[i].CreatorName + ' added user <a href="#" target="_blank" onclick="window.parent.Ts.MainPage.openNewContact(' + tixuser[i].AttachmentID + '); return false;">' + tixuser[i].UserName + '</a><br/>';
        }
    }
    return "<span class='fa fa-info-circle fa-lg wcTooltip' title='' data-info='" + tixattstr + tixgrpstr + tixprodstr + tixcompanystr + tixuserstr + "'></span>";
}

function CreateTimeLineDelegates() {
    $("#action-timeline").on("mouseenter", ".action-options", function(event) {
        $(this).find(".action-options-icon").hide();
        $(this).find(".action-option-items").fadeIn();
    }).on("mouseleave", ".action-options", function(event) {
        $(this).find(".action-option-items").hide();
        $(this).find(".action-options-icon").fadeIn();
    });

    $('#action-timeline').on('click', 'a.action-option-pin', function(e) {
        e.preventDefault();
        e.stopPropagation();

        var self = $(this);
        var nominee = self.closest('div.action');
        var Action = nominee.data().action;
        var whoFirst = $('#action-timeline div.action').first().data('id');
        var isPinned = nominee.hasClass('pinned');

        $(nominee).find('a.ticket-action-pinned').toggleClass('hidden');
        $('.action-option-items').hide();

        if (isPinned) {
            self.get(0).lastChild.nodeValue = "Pin";
        } else {
            self.get(0).lastChild.nodeValue = "Unpin";
        }

        if (window.parent.Ts.System.User.IsSystemAdmin || window.parent.Ts.System.User.UserCanPinAction) {
            window.parent.Ts.System.logAction('Ticket - Action Pin Icon Clicked');
            window.parent.Ts.Services.TicketPage.SetActionPinned(_ticketID, Action.RefID, !isPinned, function(result) {
                if (result) {
                    $('#pinned-placeholder').empty();
                    $(nominee).find('a.action-option-pin span').text('Unpin');
                    $(nominee).addClass('pinned');
                    var cloned = $(nominee).clone();
                    $("#pinned-placeholder").html(cloned);
                    if (Action.RefID === whoFirst) {
                        nominee.hide();
                    }
                } else {
                    $('#action-timeline div.pinned').removeClass('pinned').show();
                    $('#pinned-placeholder').empty();
                    $('a.action-option-pin span').text('Pin');
                    $('a.ticket-action-pinned').toggleClass('hidden', true);
                }
            }, function() {
                alert('There was an error editing this action.');
            });
        }
    });

    $('#action-timeline').on('click', 'a.ticket-action-pinned', function(e) {
        e.preventDefault();
        e.stopPropagation();
        var pinnedAction = $('#pinned-placeholder').find('.pinned').data('id');
        if (window.parent.Ts.System.User.IsSystemAdmin || window.parent.Ts.System.User.UserCanPinAction) {
            window.parent.Ts.System.logAction('Ticket - Action Pin Icon Clicked');
            window.parent.Ts.Services.Tickets.SetActionPinned(_ticketID, pinnedAction, false, function(result) {
                // $('div.action').show();
                $('a.ticket-action-pinned').toggleClass('hidden', true);
                $('a.action-option-pin span').text('Pin');
                $('#pinned-placeholder').empty();
                $('#action-timeline div.pinned').show();
                $('#action-timeline div.pinned').removeClass('pinned');
            }, function() {
                alert('There was an error editing this action.');
            });
        }
    });

    $('#action-timeline').on('click', 'a.action-option-kb', function(e) {
        e.preventDefault();
        e.stopPropagation();

        var self = $(this);
        var action = self.closest('div.action').data().action;

        if (window.parent.Ts.System.User.ChangeKbVisibility || window.parent.Ts.System.User.IsSystemAdmin) {
            window.parent.Ts.System.logAction('Ticket - Action KB Icon Clicked');
            window.parent.Ts.Services.Tickets.SetActionKb(action.RefID, !action.IsKnowledgeBase,
                function(result) {
                    var parentLI = self.closest('div.action');
                    parentLI.data().action.IsKnowledgeBase = result;
                    parentLI.find('a.ticket-action-kb').toggleClass('hidden');
                },
                function() {
                    alert('There was an error editing this action.');
                });
        }
    });

    $('#action-timeline').on('click', 'a.ticket-action-kb', function(e) {
        e.preventDefault();
        e.stopPropagation();

        var self = $(this);
        var action = self.closest('div.action').data().action;

        if (window.parent.Ts.System.User.ChangeKbVisibility || window.parent.Ts.System.User.IsSystemAdmin) {
            window.parent.Ts.System.logAction('Ticket - Action KB Icon Clicked');
            window.parent.Ts.Services.Tickets.SetActionKb(action.RefID, !action.IsKnowledgeBase, function(result) {
                var parentLI = self.closest('div.action');
                parentLI.data().action.IsKnowledgeBase = result;
                parentLI.find('a.ticket-action-kb').toggleClass('hidden');
            }, function() {
                alert('There was an error editing this action.');
            });
        }
    });

    $('#action-timeline').on('click', 'a.updatereaction', function(e) {
        e.preventDefault();
        e.stopPropagation();
        var applause = $(this);
        var actionid = $(this).data('actionid');
        var ticketid = $(this).data('ticketid');
        var oldvalue = $(this).data('oldvalue');
        var newvalue = (oldvalue > 0) ? 0 : 1;
        var oldtally = $('#tally-' + actionid).text();
        window.parent.Ts.Services.TicketPage.UpdateReaction(ticketid, actionid, newvalue, function(result) {
            if (result === 'positive') {
                applause.data('oldvalue', newvalue);
                if (newvalue > 0) {
                    var newtally = +oldtally + 1;
                } else {
                    var newtally = (oldtally > 0) ? +oldtally - 1 : 0;
                }
                $('#tally-' + actionid).text(newtally).popover("destroy");
                if (newvalue === 1) {
                    applause.css('opacity', '1');
                } else {
                    applause.css('opacity', '0.2');
                }
            }
        });
    });

    $('#action-timeline').on('mouseenter', 'span.listreactions', function(e) {
        var location = $(this);
        var actionid = $(this).data('actionid');
        var ticketid = $(this).data('ticketid');
        window.parent.Ts.Services.TicketPage.ListReactions(ticketid, actionid, function(result) {
            if (result == 'negative' || result == 'nothing') {
                location.popover("destroy");
                return;
            }
            var people = jQuery.parseJSON(result);
            var output = '<div class="reactions-head">Applause</div>';
            $.each(people.reactions, function(key, person) {
                var avatar = '/dc/1078/UserAvatar/' + person.UserID + '/32/' + Math.ceil(Math.random() * 10000);
                output += '<div class="reactions-person">';
                output += '<div class="reactions-avatar"><img src="' + avatar + '"></div>';
                output += '<div class="reactions-name"><a href="#" data-userid="' + person.UserID + '" class="UserLink">' + person.Firstname + '</a></div>';
                output += '</div>';
            });
            location.popover({
                html: true,
                trigger: 'hover',
                delay: {
                    "show": 1,
                    "hide": 1
                },
                placement: 'left',
                content: output
            }).popover('show');
        });
    }).on("click", ".UserLink", function(e) {
        var userid = $(this).data('userid');
        top.Ts.MainPage.openUser(userid);
    });

    $('#action-timeline').on('click', 'a.action-option-visible', function(e) {
        e.preventDefault();
        e.stopPropagation();

        if (window.parent.Ts.System.User.DisablePublic && !top.Ts.System.User.IsSystemAdmin) {
            alert('You cannot change action visibility.');
            return false;
        }

        var self = $(this);
        var action = self.closest('div.action').data().action;
        var applause = '#applause-' + action.RefID;

        if (window.parent.Ts.System.User.ChangeTicketVisibility || window.parent.Ts.System.User.IsSystemAdmin) {
            window.parent.Ts.System.logAction('Ticket - Action Visible Icon Clicked');
            if (!action.IsVisibleOnPortal == true) confirmVisibleToCustomers();
            window.parent.Ts.Services.TicketPage.SetActionPortal(action.RefID, !action.IsVisibleOnPortal,
                function(result) {
                    var parentLI = self.closest('div.action');
                    parentLI.data().action.IsVisibleOnPortal = result;
                    var badgeDiv = parentLI.find('div.ticket-badge');
                    badgeDiv.empty();

                    if (result) {
                        badgeDiv.html('<div class="bgcolor-green"><span class="bgcolor-green">&nbsp;</span><a href="#" class="action-option-visible">Public</a></div>');
                        if (window.parent.Ts.System.Organization.AlertContactNoEmail) {
                            window.parent.Ts.Services.TicketPage.CheckContactEmails(_ticketID, function(isInvalid) {
                                if (!isInvalid && window.parent.Ts.System.Organization.AlertContactNoEmail) {
                                    alert("At least one of the contacts associated with this ticket does not have an email address defined or is inactive, and will not receive any emails about this ticket.");
                                }
                            });
                        }
                    } else {
                        badgeDiv.html('<div class="bgcolor-orange"><span class="bgcolor-orange">&nbsp;</span><a href="#" class="action-option-visible">Private</a></div>');
                    }

                    window.parent.Ts.Services.Tickets.GetAction(action.RefID, function(action) {
                        parentLI.find('div.timeline-body').html(action.Description);
                        parentLI.data().action.Message = action.Description;
                    });

                },
                function() {
                    alert('There was an error editing this action.');
                });
        } else {
            alert('Sorry, you do not have permission to change ticket visibility, please contact your TeamSupport admin.')
        }
    });

    $('#action-timeline').on('click', 'a.action-option-edit', function(e) {
        e.preventDefault();
        e.stopPropagation();
        if ($(this).hasClass('click-disabled')) {
            return false;
        } else {
            $(this).addClass('click-disabled');
        }
        var self = $(this);
        var action = self.closest('div.action').data().action;
        var editor = $('#action-new-editor');
        SetupActionTypeSelect();
        SetupActionEditor(editor, action);

        $('#action-new-KB').prop('checked', action.IsKnowledgeBase);

        FlipNewActionBadge(!action.IsVisibleOnPortal);
        _isNewActionPrivate = !action.IsVisibleOnPortal;
        $('#action-save-alert').text('').hide();

    });

    $('#action-timeline').on('click', 'a.action-option-delete', function(e) {
        e.preventDefault();
        e.stopPropagation();

        var self = $(this);
        var action = self.closest('div.action').data().action;

        if (confirm('Are you sure you would like to delete this action?')) {
            window.parent.Ts.System.logAction('Ticket - Action Deleted');
            window.parent.Ts.Services.Tickets.DeleteAction(action.RefID, function() {
                    self.closest('div.action').remove();
                    window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber, "deleteaction", userFullName);
                },
                function() {
                    alert('There was an error deleting this action.');
                });
        }

    });

    $('#frame-container').on('scroll', function() {
        if ($(this).scrollTop() > 100) {
            $('.scrollup').fadeIn();
        } else {
            $('.scrollup').fadeOut();
        }

        if (_isLoading == true) return;
        if ($('.results-done').is(':visible')) return;

        //console.log($(this).scrollTop() + ' / ' + $(this).innerHeight() + ' / ' + $(this)[0].scrollHeight);

        if ($(this).scrollTop() + $(this).innerHeight() + 4 >= $(this)[0].scrollHeight) {
            var count = $('#action-timeline > div.action[data-id]').length;
            FetchTimeLineItems($('#action-timeline > div.action[data-id]').length + 1);
        }
    });

    $('.scrollup').click(function() {
        $('.ticketpage').animate({
            scrollTop: 0
        }, 600);
        return false;
    });

    $('.ticket-filter-public').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        var isVisible = $(this).data('visible');
        if (isVisible) {
            $('div.ticket-badge .bgcolor-green').parents('div.action').hide();
            $('.filter-public').addClass('bgcolor-darkgray');
            $('.filter-public').removeClass('bgcolor-green');
        } else {
            $('div.ticket-badge .bgcolor-green').parents('div.action').show();
            $('.filter-public').removeClass('bgcolor-darkgray');
            $('.filter-public').addClass('bgcolor-green');
        }
        $(this).data('visible', !isVisible)
    });

    $('.ticket-filter-private').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        var isVisible = $(this).data('visible');
        if (isVisible) {
            $('div.ticket-badge .bgcolor-orange').parents('div.action').hide();
            $('.filter-private').addClass('bgcolor-darkgray');
            $('.filter-private').removeClass('bgcolor-orange');
        } else {
            $('div.ticket-badge .bgcolor-orange').parents('div.action').show();
            $('.filter-private').removeClass('bgcolor-darkgray');
            $('.filter-private').addClass('bgcolor-orange');
        }
        $(this).data('visible', !isVisible)
    });

    $('.ticket-filter-wc').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        var isVisible = $(this).data('visible');
        if (isVisible) {
            $('div.ticket-badge .bgcolor-blue').parents('div.action').hide();
            $('.filter-wc').addClass('bgcolor-darkgray');
            $('.filter-wc').removeClass('bgcolor-blue');
        } else {
            $('div.ticket-badge .bgcolor-blue').parents('div.action').show();
            $('.filter-wc').removeClass('bgcolor-darkgray');
            $('.filter-wc').addClass('bgcolor-blue');
        }
        $(this).data('visible', !isVisible)
    });

    $('#action-timeline').on('click', '.wc-option-replyarea', function(e) {
        $(this).hide();
        $(this).parent().find('.wc-textarea').show().find('textarea').focus();
    });

    $('#action-timeline').on('click', 'button.wc-textarea-send', function(e) {
        e.preventDefault();
        e.stopPropagation();
        var self = $(this);
        var action = self.closest('div.action').data().action;
        var replyText = self.closest('.wc-textarea').find('textarea').val();
        if (replyText.length > 0) {
            $(this).prop('disabled', true);
            var commentinfo = new Object();
            commentinfo.Description = replyText;
            commentinfo.Attachments = new Array();
            commentinfo.ParentTicketID = action.RefID;

            commentinfo.PageType = -1;
            commentinfo.PageID = -1;

            commentinfo.Tickets = new Array();
            commentinfo.Groups = new Array();
            commentinfo.Products = new Array();
            commentinfo.Company = new Array();
            commentinfo.User = new Array();

            if (commentinfo.Tickets.length > 0) window.parent.Ts.System.logAction('Water Cooler - Ticket Inserted');
            if (commentinfo.Groups.length > 0) window.parent.Ts.System.logAction('Water Cooler - Group Inserted');
            if (commentinfo.Products.length > 0) window.parent.Ts.System.logAction('Water Cooler - Product Inserted');
            if (commentinfo.Company.length > 0) window.parent.Ts.System.logAction('Water Cooler - Company Inserted');
            if (commentinfo.User.length > 0) window.parent.Ts.System.logAction('Water Cooler - User Inserted');

            window.parent.Ts.Services.WaterCooler.NewComment(parent.JSON.stringify(commentinfo), function(Message) {
                var _compiledWCReplyTemplate = Handlebars.templates['wc-new-reply'];
                Message.Message = Message.Message.replace(/\n\r?/g, '<br />');
                var html = _compiledWCReplyTemplate(Message);
                self.closest('div.action').find('.timeline-wc-responses').append(html);
                self.parent().hide();
                self.parent().parent().find('.wc-option-replyarea').show();
                self.closest('.wc-textarea').find('textarea').val('');
                window.parent.Ts.Services.Dispatch.NewThread(Message.MessageID, window.parent.Ts.System.User.OrganizationID);
            });
        }
    });

    $(document).click(function(e) {
        if ($(e.target).is('.wc-textarea *, .wc-option-replyarea, .wc-option-replyarea *')) {
            return;
        }
        $('.wc-textarea').hide();
        $('.wc-option-replyarea').show();
    });

    $('#action-timeline').on('click', 'a.wc-reply-like-link', function(e) {
        e.preventDefault();
        e.stopPropagation();

        var self = $(this);
        var hasLiked = self.data('liked');
        if (!hasLiked) {
            var messageID = self.closest('div.timeline-wc-reply').data().messageid;
            window.parent.Ts.System.logAction('Water Cooler - Message Liked');
            window.parent.Ts.Services.WaterCooler.AddCommentLike(messageID, function(likes) {
                var countSpan = self.find('.wc-reply-like-total');
                countSpan.html("+" + likes.length);
                self.data('liked', true);
                countSpan.next().hide();
            });
        };
    });

    $('.new-action-option-visible').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        if (window.parent.Ts.System.User.DisablePublic && !top.Ts.System.User.IsSystemAdmin) {
            alert('You cannot change action visibility.');
            return false;
        }
        if (this.text == 'Private') {
            FlipNewActionBadge(false);
        } else {
            FlipNewActionBadge(true);
        }
    })
};

function CreateTicketToolbarDomEvents() {
    $('#Ticket-Owner').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        SetAssignedUser(window.parent.Ts.System.User.UserID);
        window.parent.Ts.System.logAction('Ticket - Take Ownership');
    });

    $('#Ticket-GetUpdate').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        window.parent.Ts.System.logAction('Ticket - Request Update');
        window.parent.Ts.Services.TicketPage.RequestUpdate(_ticketID, function(actionInfo) {
            _actionTotal = _actionTotal + 1;
            var actionElement = CreateActionElement(actionInfo, false);
            actionElement.find('.ticket-action-number').text(_actionTotal);

            alert('An update has been requested for this ticket.');
        }, function() {
            alert('There was an error requesting an update for this ticket.');
        });
    });

    $('#Ticket-Read').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        var self = $(this);
        var isRead = self.find('i').hasClass('color-blue');
        window.parent.Ts.Services.Tickets.SetTicketRead(_ticketID, isRead, function() {
            if (!isRead) {
                self.find('i').addClass('color-blue');
                self.attr('data-original-title', 'Mark Ticket as Read').tooltip('fixTitle');
                window.parent.Ts.System.logAction('Ticket Grid - Mark UnRead');
            } else {
                self.find('i').removeClass('color-blue');
                self.attr('data-original-title', 'Mark Ticket as UnRead').tooltip('fixTitle');
                window.parent.Ts.System.logAction('Ticket Grid - Mark Read');
            }
        });
    });

    $("#Ticket-Merge-search").autocomplete({
        minLength: 2,
        source: selectTicket,
        select: function(event, ui) {
            if (ui.item.data == _ticketID) {
                alert("Sorry, but you can not merge this ticket into itself.");
                return;
            }

            $(this).data('ticketid', ui.item.data).removeClass('ui-autocomplete-loading');
            $(this).data('ticketnumber', ui.item.id);

            try {
                window.parent.Ts.Services.Tickets.GetTicketInfo(ui.item.id, function(info) {
                    var descriptionString = info.Actions[0].Action.Description;
                    if (ellipseString(info.Actions[0].Action.Description, 30).indexOf("<img src") !== -1) {
                        descriptionString = "This ticket starts off with an embedded/linked image. We have disabled this for the preview.";
                    } else if (ellipseString(info.Actions[0].Action.Description, 30).indexOf(".viewscreencast.com") !== -1) {
                        descriptionString = "This ticket starts off with an embedded recorde video.  We have disabled this for the preview.";
                    } else {
                        descriptionString = ellipseString(info.Actions[0].Action.Description, 30);
                    }
                    var ticketPreviewName = "<div><strong>Ticket Name:</strong> " + info.Ticket.Name + "</div>";
                    var ticketPreviewAssigned = "<div><strong>Ticket Assigned To:</strong> " + info.Ticket.UserName + "</div>";
                    var ticketPreviewDesc = "<div><strong>Ticket Desciption Sample:</strong> " + descriptionString + "</div>";
                    $('#ticketmerge-preview-details').after(ticketPreviewName + ticketPreviewAssigned + ticketPreviewDesc);
                    $('#dialog-ticketmerge-preview').show();
                    $('#dialog-ticketmerge-warning').show();
                    $(".dialog-ticketmerge").dialog("widget").find(".ui-dialog-buttonpane").find(":button:contains('OK')").prop("disabled", false).removeClass("ui-state-disabled");
                })
            } catch (e) {
                alert("Sorry, there was a problem loading the information for that ticket.");
            }
        },
        position: {
            my: "right top",
            at: "right bottom",
            collision: "fit flip"
        }
    });

    $('#ticket-merge-complete').click(function(e) {
        e.preventDefault();
        if ($('#Ticket-Merge-search').val() == "") {
            alert("Please select a valid ticket to merge");
            return;
        }

        if ($('#dialog-ticketmerge-confirm').prop("checked")) {
            var winningID = $('#Ticket-Merge-search').data('ticketid');
            var winningTicketNumber = $('#Ticket-Merge-search').data('ticketnumber');
            var JSTop = top;
            window.parent.Ts.Services.Tickets.MergeTickets(winningID, _ticketID, function(result) {
                if (result != "") {
                    alert(result);
                } else {
                    $('#MergeModal').modal('hide');
                    JSwindow.parent.Ts.MainPage.closeTicketTab(_ticketNumber);
                    JSwindow.parent.Ts.MainPage.openTicket(winningTicketNumber, true);
                    //window.location = window.location;
                    window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber + "," + winningTicketNumber, "merge", userFullName);
                }
            });
        } else {
            alert("You did not agree to the conditions of the merge. Please go back and check the box if you would like to merge.")
        }
    });

    $('#Ticket-Refresh').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        window.parent.Ts.System.logAction('Ticket - Refreshed');
        window.parent.Ts.MainPage.highlightTicketTab(_ticketNumber, false);
        Unsubscribe();
        window.location = window.location;
    });

    $('#Ticket-Subscribe').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        var self = $(this);
        var isSubscribed = _ticketInfo.Ticket.IsSubscribed;
        window.parent.Ts.System.logAction('Ticket - Subscribed');
        window.parent.Ts.Services.Tickets.SetSubscribed(_ticketID, !isSubscribed, null, function(subscribers) {
            _ticketInfo.Ticket.IsSubscribed = !isSubscribed;
            self.children().toggleClass('color-green');
            self.attr('data-original-title', (_ticketInfo.Ticket.IsSubscribed) ? 'UnSubscribe to Ticket' : 'Subscribe to Ticket').tooltip('fixTitle');
            AddSubscribers(subscribers);
        }, function() {
            alert('There was an error subscribing this ticket.');
        });
    });

    $('#Ticket-Queue').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        var self = $(this);
        var isQueued = _ticketInfo.Ticket.IsEnqueued;
        window.parent.Ts.System.logAction('Ticket - Enqueued');
        window.parent.Ts.System.logAction('Queued');
        window.parent.Ts.Services.Tickets.SetQueue(_ticketID, !isQueued, null, function(queues) {
            _ticketInfo.Ticket.IsEnqueued = !isQueued;
            self.children().toggleClass('color-green');
            self.attr('data-original-title', (_ticketInfo.Ticket.IsEnqueued) ? 'Remove from your Ticket Queue' : 'Add to your Ticket Queue').tooltip('fixTitle');
            AddQueues(queues);
        }, function() {
            alert('There was an error queueing this ticket.');
        });
    });

    $('#Ticket-Flag').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        var self = $(this);
        var isFlagged = _ticketInfo.Ticket.IsFlagged;
        _ticketInfo.Ticket.IsFlagged = !isFlagged;
        self.children().toggleClass('color-red');
        self.attr('data-original-title', (_ticketInfo.Ticket.IsFlagged) ? 'UnFlag Ticket' : 'Flag Ticket').tooltip('fixTitle');
        window.parent.Ts.System.logAction('Ticket - Flagged');
        window.parent.Ts.Services.Tickets.SetTicketFlag(_ticketID, !isFlagged, null, function() {}, function() {
            alert('There was an error unflagging to this ticket.');
        });
    });

    $('#Ticket-Print').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        window.parent.Ts.System.logAction('Ticket - Printed');
        window.open(window.parent.Ts.System.AppDomain + '/TicketPrint.aspx?ticketid=' + _ticketID, 'TSPrint' + _ticketID);
    });

    $('#EmailModal').on('show.bs.modal', function (e) {
        var element = $('.email-ticket-upload');
        $('#action-file-upload').fileupload({
            namespace: 'new_action',
            dropZone: element,
            add: function (e, data) {
                for (var i = 0; i < data.files.length; i++) {
                    var item = $('<li>').appendTo(element.find('.upload-queue'));
                    data.context = item;
                    item.data('data', data);
                    var bg = $('<div>').appendTo(item);
                    $('<div>').text(data.files[i].name + '  (' + window.parent.Ts.Utils.getSizeString(data.files[i].size) + ')').addClass('filename').appendTo(bg);
                    $('<div>').addClass('progress').hide().appendTo(bg);
                    $('<span>').addClass('ui-icon ui-icon-close').click(function (e) {
                        e.preventDefault();
                        $(this).closest('li').fadeOut(500, function () {
                            $(this).remove();
                        });
                    }).appendTo(bg);
                    $('<span>').addClass('ui-icon ui-icon-cancel').hide().click(function (e) {
                        e.preventDefault();
                        var data = $(this).closest('li').data('data');
                        data.jqXHR.abort();
                    }).appendTo(bg);
                    if ((data.files[i].size / 1000000) > 25) {
                        alert("Warning " + data.files[i].name + " is over 25MB");
                    }
                }

            },
            send: function (e, data) {
                if (data.context && data.dataType && data.dataType.substr(0, 6) === 'iframe') {
                    data.context.find('.progress').progressbar('value', 50);
                }
            },
            fail: function (e, data) {
                if (data.errorThrown === 'abort') return;
                alert('There was an error uploading "' + data.files[0].name + '".');
                callback(null);
            },
            progress: function (e, data) {
                data.context.find('.progress').progressbar('value', parseInt(data.loaded / data.total * 100, 10));
            },
            start: function (e, data) {
                element.find('.progress').progressbar().show();
                element.find('.upload-queue .ui-icon-close').hide();
                element.find('.upload-queue .ui-icon-cancel').show();
            },
            stop: function (e, data) {
                window.parent.Ts.Services.TicketPage.GetActionAttachments(_newAction.item.RefID, function (attachments) {
                    window.parent.Ts.Services.TicketPage.EmailTicket(_ticketID, $("#ticket-email-input").val(), $("#ticket-intro-input").val(), _newAction.item.RefID)
                    _newAction.Attachments = attachments;
                    resetEmailModal();
                    if (_oldActionID === -1) {
                        _actionTotal = _actionTotal + 1;
                        var actionElement = CreateActionElement(_newAction, false);
                        actionElement.find('.ticket-action-number').text(_actionTotal);
                    } else {
                        UpdateActionElement(_newAction, false);
                    }
                    _newAction = null;
                });
            }
        });
    });

    function resetEmailModal() {
        $('#EmailModal').modal('hide');
        $('#ticket-intro-input').val('');
        $('#ticket-email-input').val('');
        $('.upload-queue').empty();
    }

    $('#Ticket-Email').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        window.parent.Ts.System.logAction('Ticket - Emailed');
        window.parent.Ts.Services.TicketPage.CreateEmailTicketAction(_ticketID, $("#ticket-email-input").val(), $("#ticket-intro-input").val(), function(actionInfo) {
            _newAction = actionInfo;

            if ($('.upload-queue li').length > 0) {
                UploadAttachments(_newAction);
            }
            else {
                window.parent.Ts.Services.TicketPage.EmailTicket(_ticketID, $("#ticket-email-input").val(), $("#ticket-intro-input").val(), _newAction.item.RefID, function () {
                    resetEmailModal();
                });
            }
            var actionElement = CreateActionElement(actionInfo, false);
            actionElement.find('.ticket-action-number').text(_actionTotal);
        }, function() {
            $('#email-error').show();
        });
    });

    $('#Ticket-History').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        window.parent.Ts.System.logAction('Ticket - Shown History');
        LoadTicketHistory();
        $('#TicketHistoryModal').modal('show')
    });

    $('#Ticket-Clone').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        $('#CloneModal').modal('hide');
        $('#CloningModal').modal({
            backdrop: 'static',
            keyboard: false
        });

        window.parent.Ts.System.logAction('Ticket - Cloned');
        window.parent.Ts.Services.TicketPage.CloneTicket(_ticketID, function(clonedTicketId) {
            $('#CloningModal').modal('hide');
            if (clonedTicketId > 0) {
                window.parent.Ts.MainPage.openTicketByID(clonedTicketId);
            } else {
                alert("There was an error cloning the ticket.");
            }
        }, function() {
            $('#CloningModal').modal('hide');
            alert("There was an error cloning the ticket.");
        });
    });
};

function SetupWCArea() {
    var execGetCustomer = null;

    function getCustomers(request, response) {
        if (execGetCustomer) {
            execGetCustomer._executor.abort();
        }
        execGetCustomer = window.parent.Ts.Services.Organizations.WCSearchOrganization(request.term, function(result) {
            response(result);
        });
    }
    var execGetUsers = null;

    function getUsers(request, response) {
        if (execGetUsers) {
            execGetUsers._executor.abort();
        }
        execGetUsers = window.parent.Ts.Services.Users.SearchUsers(request.term, function(result) {
            response(result);
        });
    }
    var execGetTicket = null;

    function getTicketsByTerm(request, response) {
        if (execGetTicket) {
            execGetTicket._executor.abort();
        }
        //execGetTicket = Ts.Services.Tickets.GetTicketsByTerm(request.term, function (result) { response(result); });
        execGetTicket = window.parent.Ts.Services.Tickets.SearchTickets(request.term, null, function(result) {
            $('.main-quick-ticket').removeClass('ui-autocomplete-loading');
            response(result);
        });
    }
    var execGetGroups = null;

    function getGroupsByTerm(request, response) {
        if (execGetGroups) {
            execGetGroups._executor.abort();
        }
        execGetTicket = window.parent.Ts.Services.WaterCooler.GetGroupsByTerm(request.term, function(result) {
            response(result);
        });
    }

    var execGetProducts = null;

    function getProductByTerm(request, response) {
        if (execGetProducts) {
            execGetProducts._executor.abort();
        }
        execGetProducts = window.parent.Ts.Services.WaterCooler.GetProductsByTerm(request.term, function(result) {
            response(result);
        });
    }

    $('#associationSearch').attr("placeholder", "Search Tickets").val("");
    $('#associationSearch').autocomplete({
        minLength: 2,
        source: getTicketsByTerm,
        delay: 300,
        select: function(event, ui) {
            if (ui.item) {
                var isDupe;
                $('#associationQueue').find('.ticket-queue').find('.ticket-removable-item').each(function() {
                    if (ui.item.id == $(this).data('Ticket')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>').addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn').appendTo($('#associationQueue').find('.ticket-queue')).data('Ticket', ui.item.id);
                    $('<span>').text(ellipseString(ui.item.value, 20)).addClass('filename').appendTo(bg);
                    $('<span>').addClass('ui-icon ui-icon-close').click(function(e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function() {
                            $(this).remove();
                        });
                    }).appendTo(bg);
                    this.value = "";
                    return false;
                }
            }
        }
    });

    //handle the event association
    $('.addticket').click(function(e) {
        itemAssociation("0");
    });
    $('.adduser').click(function(e) {
        itemAssociation("1");
    });
    $('.addcustomer').click(function(e) {
        itemAssociation("2");
    });
    $('.addgroup').click(function(e) {
        itemAssociation("3");
    });
    $('.addproduct').click(function(e) {
        itemAssociation("4");
    });

    function itemAssociation(associationType) {
        var searchbox = $('#associationSearch');
        switch (associationType) {
            case "0":
                $('#searchGroup').show();
                $(".arrow-up").css('left', '7px');
                $('#associationSearch').attr("placeholder", "Search Tickets").val("");
                $('#associationSearch').autocomplete({
                    minLength: 2,
                    source: getTicketsByTerm,
                    delay: 300,
                    select: function(event, ui) {
                        if (ui.item) {
                            var isDupe;
                            $('#associationQueue').find('.ticket-queue').find('.ticket-removable-item').each(function() {
                                if (ui.item.id == $(this).data('Ticket')) {
                                    isDupe = true;
                                }
                            });
                            if (!isDupe) {
                                var bg = $('<div>').addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn').appendTo($('#associationQueue').find('.ticket-queue')).data('Ticket', ui.item.id);
                                $('<span>').text(ellipseString(ui.item.value, 20)).addClass('filename').appendTo(bg);

                                $('<span>').addClass('ui-icon ui-icon-close').click(function(e) {
                                    e.preventDefault();
                                    $(this).closest('div').fadeOut(500, function() {
                                        $(this).remove();
                                    });
                                }).appendTo(bg);
                                this.value = "";
                                return false;
                            }
                        }
                    }
                });
                break;
            case "1":
                $('#searchGroup').show();
                $(".arrow-up").css('left', '36px');
                $('#associationSearch').attr("placeholder", "Search Users").val("");
                searchbox.autocomplete({
                    minLength: 3,
                    source: getUsers,
                    delay: 300,
                    select: function(event, ui) {
                        if (ui.item) {
                            var isDupe;
                            $('#associationQueue').find('.user-queue').find('.ticket-removable-item').each(function() {
                                if (ui.item.id == $(this).data('User')) {
                                    isDupe = true;
                                }
                            });
                            if (!isDupe) {
                                var bg = $('<div>').addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn').appendTo($('#associationQueue').find('.user-queue')).data('User', ui.item.id);

                                $('<span>').text(ellipseString(ui.item.value, 20)).addClass('filename').appendTo(bg);

                                $('<span>').addClass('ui-icon ui-icon-close').click(function(e) {
                                    e.preventDefault();
                                    $(this).closest('div').fadeOut(500, function() {
                                        $(this).remove();
                                    });
                                }).appendTo(bg);
                                this.value = "";
                                return false;
                            }
                        }
                    }
                });
                break;
            case "2":
                $('#searchGroup').show();
                $(".arrow-up").css('left', '60px');
                $('#associationSearch').attr("placeholder", "Search Companies").val("");
                $('#associationSearch').autocomplete({
                    minLength: 3,
                    source: getCustomers,
                    select: function(event, ui) {
                        if (ui.item) {
                            var isDupe;
                            $('#associationQueue').find('.customer-queue').find('.ticket-removable-item').each(function() {
                                if (ui.item.id == $(this).data('Company')) {
                                    isDupe = true;
                                }
                            });
                            if (!isDupe) {
                                var bg = $('<div>').addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn').appendTo($('#associationQueue').find('.customer-queue')).data('Company', ui.item.id);


                                $('<span>').text(ellipseString(ui.item.value, 20)).addClass('filename').appendTo(bg);

                                $('<span>').addClass('ui-icon ui-icon-close').click(function(e) {
                                    e.preventDefault();
                                    $(this).closest('div').fadeOut(500, function() {
                                        $(this).remove();
                                    });
                                }).appendTo(bg);
                                this.value = "";
                                return false;
                            }
                        }
                    }
                });
                break;
            case "3":
                $('#searchGroup').show();
                $(".arrow-up").css('left', '95px');
                $('#associationSearch').attr("placeholder", "Search Groups").val("");
                $('#associationSearch').autocomplete({
                    minLength: 2,
                    source: getGroupsByTerm,
                    select: function(event, ui) {
                        if (ui.item) {
                            var isDupe;
                            $('#associationQueue').find('.group-queue').find('.ticket-removable-item').each(function() {
                                if (ui.item.id == $(this).data('Group')) {
                                    isDupe = true;
                                }
                            });
                            if (!isDupe) {
                                var bg = $('<div>').addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn').appendTo($('#associationQueue').find('.group-queue')).data('Group', ui.item.id);
                                $('<span>').text(ellipseString(ui.item.value, 20)).addClass('filename').appendTo(bg);

                                $('<span>').addClass('ui-icon ui-icon-close').click(function(e) {
                                    e.preventDefault();
                                    $(this).closest('div').fadeOut(500, function() {
                                        $(this).remove();
                                    });
                                }).appendTo(bg);
                                this.value = "";
                                return false;
                            }
                        }
                    }
                });
                break;
            case "4":
                $('#searchGroup').show();
                $(".arrow-up").css('left', '125px');
                $('#associationSearch').attr("placeholder", "Search Products").val("");
                $('#associationSearch').autocomplete({
                    minLength: 3,
                    source: getProductByTerm,
                    select: function(event, ui) {
                        if (ui.item) {
                            var isDupe;
                            $('#associationQueue').find('.product-queue').find('.ticket-removable-item').each(function() {
                                if (ui.item.id == $(this).data('Product')) {
                                    isDupe = true;
                                }
                            });
                            if (!isDupe) {
                                var bg = $('<div>').addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn').appendTo($('#associationQueue').find('.product-queue')).data('Product', ui.item.id);


                                $('<span>').text(ellipseString(ui.item.value, 20)).addClass('filename').appendTo(bg);

                                $('<span>').addClass('ui-icon ui-icon-close').click(function(e) {
                                    e.preventDefault();
                                    $(this).closest('div').fadeOut(500, function() {
                                        $(this).remove();
                                    });
                                }).appendTo(bg);
                                this.value = "";
                                return false;
                            }
                        }
                    }
                });
                break;
        }
    }

    $('#newcomment').click(function(e) {
        e.preventDefault();
        e.stopPropagation();

        if ($('#inputDescription').val().length > 0) {
            $(this).prop('disabled', true);
            var commentinfo = new Object();
            commentinfo.Description = $('#inputDescription').val();
            commentinfo.Attachments = new Array();
            commentinfo.ParentTicketID = -1;

            commentinfo.PageType = 0;
            commentinfo.PageID = _ticketNumber;

            commentinfo.Tickets = new Array();
            $('#associationQueue:first').find('.ticket-queue').find('.ticket-removable-item').each(function() {
                commentinfo.Tickets[commentinfo.Tickets.length] = $(this).data('Ticket');
            });

            commentinfo.Groups = new Array();
            $('#associationQueue:first').find('.group-queue').find('.ticket-removable-item').each(function() {
                commentinfo.Groups[commentinfo.Groups.length] = $(this).data('Group');
            });

            commentinfo.Products = new Array();
            $('#associationQueue:first').find('.product-queue').find('.ticket-removable-item').each(function() {
                commentinfo.Products[commentinfo.Products.length] = $(this).data('Product');
            });

            commentinfo.Company = new Array();
            $('#associationQueue:first').find('.customer-queue').find('.ticket-removable-item').each(function() {
                commentinfo.Company[commentinfo.Company.length] = $(this).data('Company');
            });

            commentinfo.User = new Array();
            $('#associationQueue:first').find('.user-queue').find('.ticket-removable-item').each(function() {
                commentinfo.User[commentinfo.User.length] = $(this).data('User');
            });

            if (commentinfo.Tickets.length > 0) {
                window.parent.Ts.System.logAction('Water Cooler - Ticket Inserted');
            }
            if (commentinfo.Groups.length > 0) {
                window.parent.Ts.System.logAction('Water Cooler - Group Inserted');
            }
            if (commentinfo.Products.length > 0) {
                window.parent.Ts.System.logAction('Water Cooler - Product Inserted');
            }
            if (commentinfo.Company.length > 0) {
                window.parent.Ts.System.logAction('Water Cooler - Company Inserted');
            }
            if (commentinfo.User.length > 0) {
                window.parent.Ts.System.logAction('Water Cooler - User Inserted');
            }

            window.parent.Ts.Services.TicketPage.NewWCPost(parent.JSON.stringify(commentinfo), function(message) {
                if ($('.wc-attachments li').length > 0) {
                    $('.wc-attachments li').each(function(i, o) {
                        var data = $(o).data('data');
                        data.url = '../../../Upload/WaterCooler/' + message.item.RefID;
                        data.jqXHR = data.submit();
                        $(o).data('data', data);
                    });
                }
                window.parent.Ts.Services.Dispatch.NewThread(message.item.RefID, window.parent.Ts.System.User.OrganizationID);
                $('.wc-attachments').empty();
                CreateActionElement(message, false);
                $('.watercooler-new-area').fadeOut('normal');
            });
        }
    });

    var wcelement = $('.watercooler-new-area');
    $('#wc-file-upload').fileupload({
        namespace: 'new_action',
        dropZone: wcelement,
        add: function(e, data) {
            for (var i = 0; i < data.files.length; i++) {
                var item = $('<li>').appendTo(wcelement.find('.wc-attachments'));

                data.context = item;
                item.data('data', data);

                var bg = $('<div class="ui-corner-all ts-color-bg-accent ticket-removable-item ulfn">').appendTo(item);

                $('<span>').text(data.files[i].name + '  (' + window.parent.Ts.Utils.getSizeString(data.files[i].size) + ')').addClass('filename').appendTo(bg);

                $('<div>').addClass('progress').hide().appendTo(bg);

                $('<span>').addClass('ui-icon ui-icon-close').click(function(e) {
                    e.preventDefault();
                    $(this).closest('li').fadeOut(500, function() {
                        $(this).remove();
                    });
                }).appendTo(bg);

                //<span class="tagRemove" aria-hidden="true">×</span>

                $('<span>').addClass('ui-icon ui-icon-cancel').hide().click(function(e) {
                    e.preventDefault();
                    var data = $(this).closest('li').data('data');
                    data.jqXHR.abort();
                }).appendTo(bg);
            }

        },
        send: function(e, data) {
            if (data.context && data.dataType && data.dataType.substr(0, 6) === 'iframe') {
                data.context.find('.progress').progressbar('value', 50);
            }
        },
        fail: function(e, data) {
            if (data.errorThrown === 'abort') {
                return;
            }
            alert('There was an error uploading "' + data.files[0].name + '".');
            callback(null);
        },
        progress: function(e, data) {
            data.context.find('.progress').progressbar('value', parseInt(data.loaded / data.total * 100, 10));
        },
        start: function(e, data) {
            wcelement.find('.progress').progressbar().show();
            wcelement.find('.wc-attachments .ui-icon-close').hide();
            wcelement.find('.wc-attachments .ui-icon-cancel').show();
        },
        stop: function(e, data) {}
    });
}

function RefreshSlaDisplay() {
    if ($('#ticket-SLANote').text() == 'Calculating...') {
        resetSLAInfo();
    } else {
        clearInterval(slaCheckTimer);
    }
}

var MergeSuccessEvent = function(_ticketNumber, winningTicketNumber) {
    $('#merge-success').show();
    $('#MergeModal').modal('hide');
    window.parent.Ts.MainPage.closeTicketTab(_ticketNumber);
    window.parent.Ts.MainPage.openTicket(winningTicketNumber, true);
    window.location = window.location;
    window.parent.Ts.Services.Dispatch.TicketUpdate(_ticketNumber + "," + winningTicketNumber, "merge", userFullName);
}


var resetSLAInfo = function() {
    window.parent.Ts.Services.TicketPage.GetTicketSLAInfo(_ticketNumber, function(info) {
        _ticketInfo.Ticket.SlaViolationTime = info.Ticket.SlaViolationTime;
        _ticketInfo.Ticket.SlaWarningTime = info.Ticket.SlaWarningTime;
        _ticketInfo.Ticket.SlaViolationDate = info.Ticket.SlaViolationDate;
        _ticketInfo.SlaTriggerId = info.SlaTriggerId;
        _ticketInfo.IsSlaPending = info.IsSlaPending;
        _ticketInfo.IsSlaPaused = info.IsSlaPaused;
        _ticketInfo.Ticket.IsClosed = info.Ticket.IsClosed;
        setSLAInfo();
    });
}

var setSLAInfo = function() {
    $('#ticket-SLAStatus').find('i').removeClass('color-green color-red color-yellow');
    if (_ticketInfo.IsSlaPaused !== undefined &&
        _ticketInfo.IsSlaPaused &&
        !_ticketInfo.Ticket.IsClosed &&
        _ticketInfo.SlaTriggerId !== null &&
        _ticketInfo.SlaTriggerId > 0) {
        $('#ticket-SLAStatus').find('i').removeClass('fa-bomb');
        $('#ticket-SLAStatus').find('i').addClass('fa-pause');
        $('#ticket-SLAStatus').find('i').addClass('slaPausedIcon');
        $('#ticket-SLANote').text('Paused');
    } else if (_ticketInfo.Ticket.SlaViolationTime === null &&
        ((_ticketInfo.SlaTriggerId === null || _ticketInfo.SlaTriggerId == 0) ||
            (_ticketInfo.SlaTriggerId !== null && _ticketInfo.SlaTriggerId > 0 && _ticketInfo.IsSlaPending !== null && !_ticketInfo.IsSlaPending))
    ) {
        $('#ticket-SLAStatus').find('i').addClass('color-green');
        $('#ticket-SLANote').text('');
        $('#ticket-SLAStatus').find('i').addClass('fa-bomb');
        $('#ticket-SLAStatus').find('i').removeClass('fa-pause');
        $('#ticket-SLAStatus').find('i').removeClass('slaPausedIcon');
    } else if (_ticketInfo.SlaTriggerId !== null &&
        _ticketInfo.SlaTriggerId > 0 &&
        _ticketInfo.IsSlaPaused !== undefined &&
        !_ticketInfo.IsSlaPaused &&
        !_ticketInfo.Ticket.IsClosed &&
        _ticketInfo.IsSlaPending !== null &&
        _ticketInfo.IsSlaPending) {
        $('#ticket-SLAStatus').find('i').addClass('color-yellow');
        $('#ticket-SLANote').text('Calculating...');
        $('#ticket-SLAStatus').find('i').addClass('fa-bomb');
        $('#ticket-SLAStatus').find('i').removeClass('fa-pause');
        $('#ticket-SLAStatus').find('i').removeClass('slaPausedIcon');
    } else {
        $('#ticket-SLAStatus')
            .find('i')
            .addClass((_ticketInfo.Ticket.SlaViolationTime < 1 ? 'color-red' : (_ticketInfo.Ticket.SlaWarningTime < 1 ? 'color-yellow' : 'color-green')));
        $('#ticket-SLAStatus').find('i').addClass('fa-bomb');
        $('#ticket-SLAStatus').find('i').removeClass('fa-pause');
        $('#ticket-SLAStatus').find('i').removeClass('slaPausedIcon');

        if (_ticketInfo.Ticket.SlaViolationDate !== null) {
            $('#ticket-SLANote').text(_ticketInfo.Ticket.SlaViolationDate.localeFormat(window.parent.Ts.Utils.getDateTimePattern()));
        } else {
            $('#ticket-SLANote').text('');
        }
    }
    $('#ticket-SLAStatus').data('placement', 'left').data('ticketid', _ticketID);
}

var SetKBCategory = function(KnowledgeBaseCategoryID) {
    var selectField = $('#ticket-KB-Category');
    if (selectField.length > 0) {
        var selectize = $('#ticket-KB-Category')[0].selectize;
        selectize.addItem(KnowledgeBaseCategoryID, false);
    } else {
        $('#ticket-KB-Category-RO').text(_ticketInfo.Ticket.KnowledgeBaseCategoryName);
    }
}

var SetCommunityCategory = function(ForumCategory) {
    var selectField = $('#ticket-Community');
    if (selectField.length > 0) {
        var selectize = $('#ticket-Community')[0].selectize;
        selectize.addItem(ForumCategory, false);
    } else {
        $('#ticket-Community-RO').text((_ticketInfo.Ticket.CategoryName == null ? 'Unassigned' : _ticketInfo.Ticket.CategoryDisplayString));
    }
}

var SetDueDate = function(duedate) {
    $('.ticket-duedate-anchor').text((duedate === null ? '' : duedate.localeFormat(window.parent.Ts.Utils.getDateTimePattern())));
};

var SetAssignedUser = function(ID) {
    var selectUserField = $('#ticket-assigned');
    if (selectUserField.length > 0) {
        var selectizeUserField = $('#ticket-assigned')[0].selectize;
        selectizeUserField.addItem(ID, false);
    }
    _ticketCurrUser = ID;
}

var SetGroup = function(GroupID) {
    var selectField = $('#ticket-group');
    if (selectField.length > 0) {
        var selectize = $('#ticket-group')[0].selectize;
        selectize.addItem(GroupID, false);
    }
    ticketGroupID = GroupID;
}

var SetStatus = function(StatusID) {
    var selectField = $('#ticket-status');
    if (selectField.length > 0) {
        var statuses = window.parent.Ts.Cache.getNextStatuses(StatusID);
        var selectize = selectField[0].selectize;
        selectize.clear(true);
        selectize.clearOptions();

        for (var i = 0; i < statuses.length; i++) {
            selectize.addOption({
                value: statuses[i].TicketStatusID,
                text: statuses[i].Name,
                data: statuses[i]
            });
        }

        if (StatusID !== null) selectize.addItem(StatusID, false);
        _ticketCurrStatus = StatusID;
    }
    EnableField('ticket-status', true);
};

var EnableField = function(fieldToDisable, enableField) {
    var $select = $('#' + fieldToDisable).selectize();
    var selectize = $select[0].selectize;
    if (enableField) {
        selectize.enable();
    } else selectize.disable();
}

var SetType = function(TypeID) {
    var selectField = $('#ticket-type');
    if (selectField.length > 0) {
        var selectize = $('#ticket-type')[0].selectize;
        selectize.addItem(TypeID, false);
    }
};

var SetSeverity = function(SeverityID) {
    var selectField = $('#ticket-severity');
    if (selectField.length > 0) {
        var selectize = $('#ticket-severity')[0].selectize;
        selectize.addItem(SeverityID, false);
    }
};

var SetProduct = function(ProductID) {
    var selectField = $('#ticket-Product');
    if (selectField.length > 0) {
        var selectize = $('#ticket-Product')[0].selectize;
        selectize.addItem(ProductID, false);
    }
};

var SetVersion = function(VersionID) {
    var selectField = $('#ticket-Versions');
    if (selectField.length > 0) {
        var selectize = $('#ticket-Versions')[0].selectize;
        if (VersionID) selectize.addItem(VersionID, false);
        else selectize.clear(true);
    }
};

var SetSolved = function(ResolvedID) {
    var selectField = $('#ticket-Resolved');
    if (selectField.length > 0) {
        var selectize = $('#ticket-Resolved')[0].selectize;
        if (ResolvedID) selectize.addItem(ResolvedID, false);
        else selectize.clear(true);
    }
};

function ticketmenu() {
    var width = $('#menu-container').width();
    // $('#ticketmenu-container').css('width',width);
    if (width > 900) {
        $('#ticketmenu-actions-lg').show();
        $('#ticketmenu-actions-sm').hide();
    } else {
        $('#ticketmenu-actions-lg').hide();
        $('#ticketmenu-actions-sm').show();
    }
}

function maxwidth() {
    var width = $(window).width();
    var limit = width - 400;
    $('#action-timeline img').each(function() {
        var image = $(this).width();
        if (image > limit) {
            $(this).css('max-width', limit).css('width', limit);
        }
    });
}

function pagewidth() {
    var width = $(window).width();
    $('#frame-container, #page-container').css('max-width', width);
    $('#page-container').css('max-width', width).css('min-width', width);
    $('#page-primary').css('max-width', width - 310).css('min-width', width - 310);
}


function SetupPusher() {
    var presenceChannel = null;
    var service = '/Services/DispatchService.asmx/';
    if (top.Ts.TicketViewing != 0) {
        var orgID = top.Ts.System.Organization.OrganizationID;
        var presenceChannelName = 'presence-ticket-' + top.Ts.TicketViewing + '-org-' + orgID;
        top.Ts.Pusher.unsubscribe(presenceChannelName);
    }

    top.Ts.TicketViewing = _ticketNumber;
    top.Ts.Settings.System.read('PusherKey', '1', function(key) {
        var orgID = top.Ts.System.Organization.OrganizationID;
        var userID = top.Ts.System.User.UserID;

        var presenceChannelName = 'presence-ticket-' + _ticketNumber + '-org-' + orgID;
        presenceChannel = top.Ts.Pusher.subscribe(presenceChannelName);

        presenceChannel.bind('pusher:subscription_succeeded', function(members) {
            try {
                addUsersViewing(members);
            } catch (err) {}
        });

        presenceChannel.bind('pusher:member_added', function(member) {
            try {
                addUserViewing(member.id);
            } catch (err) {}
        });

        presenceChannel.bind('pusher:member_removed', function(member) {
            try {
                removeUserViewing(member.id);
            } catch (err) {}
        });

        presenceChannel.bind('ticketViewingRemove', function(data) {
            top.Ts.Pusher.unsubscribe(data.chan);
        });
    });
}
var addUsersViewing = function(members) {
    members.each(function(member) {
        addUserViewing(member.id);
    });
}

var addUserViewing = function(userID) {
    if (userID != top.Ts.System.User.UserID) {
        $('#ticket-now-viewing').show();
        if ($('.ticket-viewer:data(ChatID=' + userID + ')').length < 1) {
            window.parent.Ts.Services.Users.GetUser(userID, function(user) {
                $('.ticket-viewer:data(ChatID=' + user.UserID + ')').remove();
                var fullName = user.FirstName + " " + user.LastName;
                var viewuser = $('<a>')
                    .data('ChatID', user.UserID)
                    .data('Name', fullName)
                    .addClass('ticket-viewer')
                    .click(function() {
                        window.parent.openChat($(this).data('Name'), $(this).data('ChatID'));
                        window.parent.Ts.System.logAction('Now Viewing - Chat Opened');
                    })
                    .html('<img class="user-avatar ticket-viewer-avatar" src="../../../dc/' + user.OrganizationID + '/useravatar/' + user.UserID + '/48">' + fullName + '</a>')
                    .appendTo($('#ticket-viewing-users'));
            });
        }
    }
}

var removeUserViewing = function(userID) {
    if ($('.ticket-viewer:data(ChatID=' + userID + ')').length > 0) {
        $('.ticket-viewer:data(ChatID=' + userID + ')').remove();
        if ($('.ticket-viewer').length < 1) {
            $('#ticket-now-viewing').hide();
        }
    }
}

function Unsubscribe() {
    var orgID = top.Ts.System.Organization.OrganizationID;
    var presenceChannelName = 'presence-ticket-' + _ticketNumber + '-org-' + orgID;
    top.Ts.Pusher.unsubscribe(presenceChannelName);
}

// id is the task id, status refers to the 'checked' property.
function taskCheckBox(id, status) {
    document.getElementById('task-' + id).checked = (status) ? true : false;
}

// Invalid Ticket Status.
function invalidStatus(StatusId) {
    teamsupport.modals.overlay.show();
    $('#modal').html(Handlebars.templates['invalid']);
    teamsupport.modals.modal.show('#modal');
    var statuses = window.parent.Ts.Cache.getNextStatuses(StatusId);
    $.each(statuses, function(key, status) {
        $('<option>').text(status.Name).attr('value', status.TicketStatusID).appendTo('#newStatus');
    });
}

$(document).on('click', '#updateStatus', function(e) {
    var newStatus = $('#newStatus').val();
    window.parent.Ts.Services.Tickets.SetTicketStatus(_ticketID, newStatus, function(result) {
        if (result !== null) {
            parent.document.getElementById(window.frameElement.id).contentDocument.location.reload(true);
        }
    });
});
