﻿var _ticketInfo = null;
var _ticketNumber = null;
var _ticketID = null;
var _ticketCreator = new Object();
var _ticketSender = null;

//TODO:  Need to figure out wth these do. 
var _ticketGroupID = null;
var _ticketGroupUsers = null;
var _ticketTypeID = null;
var _parentFields = [];

var _isNewActionPrivate = true;

var _timeLine = new Object();
var _currDateSpan = null;
var _compiledActionTemplate = null;
var _actionTotal = 0;
var _workingActionNumer = 0;
var _isLoading = false;
var dateformat;

var _timerid;
var _timerElapsed = 0;
var speed = 50, counter = 0, start;

var userFullName = top.Ts.System.User.FirstName + " " + top.Ts.System.User.LastName;

var execGetCustomer = null;
var execGetTags = null;
var execGetAsset = null;
var execGetUsers = null;
var execGetRelated = null;
var execSelectTicket = null;

var getCustomers = function (request, response) {
    if (execGetCustomer) { execGetCustomer._executor.abort(); }
    execGetCustomer = top.Ts.Services.TicketPage.GetUserOrOrganizationForTicket(request, function (result) { response(result); });
}

var getTags = function (request, response) {
    if (execGetTags) { execGetTags._executor.abort(); }
    execGetTags = top.Ts.Services.Tickets.SearchTags(request.term, function (result) { response(result); });
}

var getAssets = function (request, response) {
    if (execGetAsset) { execGetAsset._executor.abort(); }
    execGetAsset = top.Ts.Services.Assets.FindAsset(request, function (result) { response(result); });
}

var getUsers = function (request, response) {
    if (execGetUsers) { execGetUsers._executor.abort(); }
    execGetUsers = top.Ts.Services.TicketPage.SearchUsers(request, function (result) { response(result); });
}

var getRelated = function (request, response) {
    if (execGetRelated) { execGetRelated._executor.abort(); }
    execGetRelated = top.Ts.Services.Tickets.SearchTickets(request, null, function (result) { response(result); });
}

var selectTicket = function (request, response) {
    if (execSelectTicket) { execSelectTicket._executor.abort(); }
    var filter = $(this.element).data('filter');
    if (filter === undefined) {
        execSelectTicket = top.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) { response(result); });
    }
    else {
        execSelectTicket = top.Ts.Services.Tickets.SearchTickets(request.term, filter, function (result) { response(result); });
    }
}

var ellipseString = function (text, max) { return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text; };

var isFormValidToClose = function (isClosed, callback) {
    var result = true;
    if (isClosed) {
        var test = $('.isRequiredToClose.isEmpty');
        $('.isRequiredToClose.isEmpty').addClass('hasCloseError');
        if ($('.hasCloseError').length > 0) {
            result = false;
        }
    }
    callback(result);
}

Selectize.define('sticky_placeholder', function (options) {
    var self = this;

    self.updatePlaceholder = (function () {
        var original = self.updatePlaceholder;
        return function () {
            original.apply(this, arguments);
            if (!this.settings.placeholder) return;
            var $input = this.$control_input;
            $input.attr('placeholder', this.settings.placeholder);
        };
    })();

});

$(document).ready(function () {
    _ticketNumber = top.Ts.Utils.getQueryValue("TicketNumber", window);

    //Setup Ticket Elements
    SetupTicketPage();

    //Create Handlebar Helpers
    CreateHandleBarHelpers();

    //Create Dom Events
    CreateTicketToolbarDomEvents();
    CreateTimeLineDelegates();
});

function SetupTicketPage() {
    //Create the new action LI element
    CreateNewActionLI();

    top.Ts.Services.TicketPage.GetTicketPageOrder("TicketFieldsOrder", function (order) {
        jQuery.each(order, function (i, val) { AddTicketProperty(val); });
        SetupTicketProperties();
    });

    top.Ts.Services.Customers.GetDateFormat(true, function (format) {
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

};

function AddTicketProperty(item) {
    if ($("#ticket-group-" + item.CatID).length > 0) {
        var compiledTemplate = Handlebars.compile($("#ticket-group-" + item.CatID).html());
        $('#ticket-properties-area').append(compiledTemplate);
    }
};

function SetupTicketProperties() {
    top.Ts.Services.TicketPage.GetTicketInfo(_ticketNumber, function (info) {
        if (info == null) {
            var url = window.location.href;
            if (url.indexOf('.') > -1) {
                url = url.substring(0, url.lastIndexOf('/') + 1);
            }
            window.location = url + 'NoTicketAccess.html';
            return;
        }
        _ticketInfo = info;
        _ticketID = info.Ticket.TicketID;
        top.Ts.Services.Tickets.GetTicketLastSender(_ticketID, function (result) {
            if (result !== null) {
                _ticketSender = new Object();
                _ticketSender.UserID = result.UserID;
                _ticketSender.Name = result.FirstName + ' ' + result.LastName;
            }
        });
        _ticketCreator = new Object();
        _ticketCreator.UserID = info.Ticket.CreatorID;
        _ticketCreator.Name = info.Ticket.CreatorName;
        top.Ts.System.logAction('View Ticket');

        if (info == null) alert('no ticket');

        if (top.Ts.System.User.IsSystemAdmin || top.Ts.System.User.UserID === _ticketInfo.UserID) {
            $('.ticket-menu-actions').append('<li><a id="Ticket-Delete">Delete</a></li>');
            $('#Ticket-Delete').click(function (e) {
                e.preventDefault();
                e.stopPropagation();
                if (confirm('Are you sure you would like to delete this ticket?')) {
                    top.Ts.System.logAction('Ticket - Deleted');
                    top.Ts.Services.Tickets.DeleteTicket(_ticketID, function () {
                        top.Ts.MainPage.closeTicketTab(_ticketNumber);
                        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "delete", userFullName);
                    }, function () {
                        alert('There was an error deleting this ticket.');

                    });
                }
            });
        };

        //update ticket property controls with the values needed
        LoadTicketControls();
        //Get Ticket Notes for Customers associated with ticket
        top.Ts.Services.Customers.LoadTicketAlerts(_ticketID, function (note) {
            LoadTicketNotes(note);
        });
        //get total number of actions so we can use it to number each action
        GetActionCount();
        //create timeline now that we have a ticketID
        FetchTimeLineItems(0);
    });
};

function CreateNewActionLI() {
    var _compiledNewActionTemplate = Handlebars.compile($("#new-action-template").html());
    var html = _compiledNewActionTemplate({ OrganizationID: top.Ts.System.User.OrganizationID, UserID: top.Ts.System.User.UserID });
    $("#action-timeline").append(html);

    $('#action-add-public').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        var editor = $('#action-new-editor');
        SetupActionEditor(editor);
        SetupActionTypeSelect();
        FlipNewActionBadge(false);
        _isNewActionPrivate = false;
    });

    $('#action-add-private').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        var editor = $('#action-new-editor');
        SetupActionEditor(editor);
        SetupActionTypeSelect();
        FlipNewActionBadge(true);
        _isNewActionPrivate = true;
    });

    $('#action-new-cancel').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        $('#action-new-editor').parent().fadeOut('normal');
        tinymce.activeEditor.destroy();
    });

    $('#action-timeline').on('click', '#action-new-cancel', function (e) {
        e.preventDefault();
        e.stopPropagation();
        $('#action-new-editor').parent().fadeOut('normal');
        tinymce.activeEditor.destroy();
        $('#action-new-save-element').dropdown('toggle')
    });

    $('#action-new-save').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        SaveAction(null, _isNewActionPrivate, function (result) {
            UploadAttachments(result);
            $('#action-new-editor').val('').parent().fadeOut('normal');
            tinymce.activeEditor.destroy();
            top.Ts.Services.TicketPage.GetActionAttachments(result.item.RefID, function (attachments) {
                result.Attachments = attachments;
                CreateActionElement(result, false);
            });
        });
    });

    $('#action-timeline').on('click', '.action-create-option', function (e) {
        e.preventDefault();
        e.stopPropagation();
        var self = $(this);
        SaveAction(null, _isNewActionPrivate, function (result) {
            UploadAttachments(result);
            $('#action-new-editor').val('').parent().fadeOut('normal');
            tinymce.activeEditor.destroy();

            var statusID = self.data("statusid");
            top.Ts.Services.Tickets.SetTicketStatus(_ticketID, statusID, function () {
                $('#ticket-status').val(statusID);
                top.Ts.System.logAction('Ticket - Status Changed');
                window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changestatus", userFullName);
            });

            top.Ts.Services.TicketPage.GetActionAttachments(result.item.RefID, function (attachments) {
                result.Attachments = attachments;
                CreateActionElement(result, false);
            });
        });
    });

    $('#action-new-type').change(function (e) {
        var action = $(this).val();
        top.Ts.Services.TicketPage.GetActionTicketTemplate(action, function (result) {
            if (result != null && result != "" && result != "<br>") {
                var currenttext = tinyMCE.activeEditor.getContent();
                tinyMCE.activeEditor.setContent(currenttext + result);
            }
            elem.parent().fadeIn('normal');
        });
    });
};

function SetupActionEditor(elem) {
    initEditor(elem, true, function (ed) {
        var action = $('#action-new-type').val();
        top.Ts.Services.TicketPage.GetActionTicketTemplate(action, function (result) {
            if (result != null && result != "" && result != "<br>") {
                var currenttext = tinyMCE.activeEditor.getContent();
                tinyMCE.activeEditor.setContent(currenttext + result);
            }
            elem.parent().fadeIn('normal');
        });
    });

    var element = $('.action-new-area');
    $('.file-upload').fileupload({
        namespace: 'new_action',
        dropZone: element,
        add: function (e, data) {
            for (var i = 0; i < data.files.length; i++) {
                var item = $('<li>')
                .appendTo(element.find('.upload-queue'));

                data.context = item;
                item.data('data', data);

                var bg = $('<div>')
                .appendTo(item);

                $('<div>')
                .text(data.files[i].name + '  (' + top.Ts.Utils.getSizeString(data.files[i].size) + ')')
                .addClass('filename')
                .appendTo(bg);

                $('<div>')
                .addClass('progress')
                .hide()
                .appendTo(bg);

                $('<span>')
                .addClass('ui-icon ui-icon-close')
                .click(function (e) {
                    e.preventDefault();
                    $(this).closest('li').fadeOut(500, function () { $(this).remove(); });
                })
                .appendTo(bg);

                //<span class="tagRemove" aria-hidden="true">×</span>

                $('<span>')
                .addClass('ui-icon ui-icon-cancel')
                .hide()
                .click(function (e) {
                    -
                        e.preventDefault();
                    var data = $(this).closest('li').data('data');
                    data.jqXHR.abort();
                })
                .appendTo(bg);
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

        }
    });

    var statuses = top.Ts.Cache.getNextStatuses(_ticketInfo.Ticket.TicketStatusID);
    $('#action-new-saveoptions').empty();
    for (var i = 0; i < statuses.length; i++) {
        $('#action-new-saveoptions').append('<li><a class="action-create-option" data-statusid=' + statuses[i].TicketStatusID + ' href="#">Create and Set Status to ' + statuses[i].Name + '</a></li>'); 
    }
    $('#action-new-saveoptions').append('<li class="divider"></li>').append('<li><a id="action-new-cancel" href="#">Cancel</a></li>');

    $('#action-new-date-started').datetimepicker({ useCurrent: true, format: 'MM/DD/YYYY hh:mm A', defaultDate: new Date() });

    $('.spinner .btn:first-of-type').click( function () {
        var spinner = $(this).parent().prev();
        spinner.val(parseInt(spinner.val(), 10) + 1);
    });

    $('.spinner .btn:last-of-type').click( function () {
        var spinner = $(this).parent().prev();
        spinner.val(parseInt(spinner.val(), 10) - 1);
    });
    $('#action-new-timer').click(function (e) {
        var hasStarted = $(this).data('hasstarted');

        if (!hasStarted) {
            start = new Date().getTime();
            tickettimer();
            $(this).find(':first-child').css('color', 'green');
        }
        else {
            $(this).find(':first-child').css('color', 'red');
            clearTimeout(_timerid);
        }
        $(this).data('hasstarted', !hasStarted);
    });
}

function SetupActionTypeSelect() {
    var selectType = $('#action-new-type');
    selectType.empty();
    var types = top.Ts.Cache.getActionTypes();
    for (var i = 0; i < types.length; i++) {
        $('<option>').attr('value', types[i].ActionTypeID).text(types[i].Name).data('data', types[i]).appendTo(selectType);
    }
};

function FlipNewActionBadge(isPrivate) {
    if (isPrivate)
    {
        $('#private-badge').show();
        $('#public-badge').hide();
    }
    else
    {
        $('#private-badge').hide();
        $('#public-badge').show();
    }
    _isNewActionPrivate = isPrivate;
}

function SaveAction(oldAction, isPrivate, callback) {
    var action = new top.TeamSupport.Data.ActionProxy();
    action.ActionID = oldAction === null ? -1 : oldAction.ActionID;
    action.TicketID = _ticketID;

    var actionType = $('#action-new-type option:selected').data('data');
    action.ActionTypeID = actionType.ActionTypeID

    action.SystemActionTypeID = 0;

    var timeSpent = parseInt($('#action-new-hours').val()) * 60 + parseInt($('#action-new-minutes').val());

    if (timeSpent < 1 && actionType.IsTimed == true && top.Ts.System.Organization.TimedActionsRequired == true) {
        //TODO: Need to add validation to indicate that a timed value is required
        return false;
    }

    action.TimeSpent = timeSpent || 0;
    action.DateStarted = top.Ts.Utils.getMsDate($('#action-new-date-started').val());
    action.IsKnowledgeBase = false;
    action.IsVisibleOnPortal = !isPrivate;

    action.Description = tinymce.activeEditor.getContent();

    //TODO:  Need confirmation
    //if (action.IsVisibleOnPortal == true) confirmVisibleToCustomers();
    top.Ts.Services.TicketPage.UpdateAction(action, function (result) {
        callback(result)
    }, function (error) {
        callback(null);
    });
}

function UploadAttachments(newAction) {
    if ($('.upload-queue li').length > 0 && newAction !== null) {
        $('.upload-queue li').each(function (i, o) {
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
        $('#modalAlertMessage').html(note.Description);
        $('#alertID').val(note.RefID);
        $('#alertType').val(note.RefType);

        var buttons = {
            "Close": function () {
                $(this).dialog("close");
            },
            "Snooze": function () {
                top.Ts.Services.Customers.SnoozeAlert($('#alertID').val(), $('#alertType').val());
                $(this).dialog("close");
            }
        }

        if (!top.Ts.System.Organization.HideDismissNonAdmins || top.Ts.System.User.IsSystemAdmin) {
            buttons["Dismiss"] = function () {
                top.Ts.Services.Customers.DismissAlert($('#alertID').val(), $('#alertType').val());
                $(this).dialog("close");
            }
        }

        $("#dialog").dialog({
            resizable: false,
            width: 'auto',
            height: 'auto',
            modal: true,
            create: function () {
                $(this).css('maxWidth', '800px');
            },
            buttons: buttons
        });

    }
};

function GetActionCount() {
    top.Ts.Services.TicketPage.GetActionCount(_ticketID, function (total) {
        _actionTotal = total;
        _workingActionNumer = total;
    });
};

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

    var AssignedSelectize = $('#ticket-assigned').selectize({
        valueField: 'id',
        labelField: 'label',
        searchField: 'label',
        preload: true,
        load: function (query, callback) {
            top.Ts.Services.TicketPage.SearchUsers(query, function (result) {
                callback(result);
            });

        },
        initData: true,
        onLoad: function () {
            if (this.settings.initData === true) {
                this.setValue(_ticketInfo.Ticket.UserID);
                this.settings.initData = false;
            }
        },
        onItemAdd: function (value, $item) {
            if (this.settings.initData === false) {
                top.Ts.Services.Tickets.SetTicketUser(_ticketID, value, function () {
                    window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeassigned", userFullName);
                },
                function (error) {
                    alert('There was an error setting the user.');
                });
            }
        }
    });

    top.Ts.Services.TicketPage.GetTicketGroups(_ticketID, function (groups) {
        for (var i = 0; i < groups.length; i++) {
            AppendSelect('#ticket-group', groups[i], 'group', groups[i].ID, groups[i].Name, groups[i].IsSelected);
        }
        $('#ticket-group').selectize();
    });

    _ticketTypeID = _ticketInfo.Ticket.TicketTypeID;
    var types = top.Ts.Cache.getTicketTypes();
    for (var i = 0; i < types.length; i++) {
        AppendSelect('#ticket-type', types[i], 'type', types[i].TicketTypeID, types[i].Name, (_ticketInfo.Ticket.TicketTypeID === types[i].TicketTypeID));
    }

    var statuses = top.Ts.Cache.getNextStatuses(_ticketInfo.Ticket.TicketStatusID);
    for (var i = 0; i < statuses.length; i++) {
        AppendSelect('#ticket-status', statuses[i], 'status', statuses[i].TicketStatusID, statuses[i].Name, (_ticketInfo.Ticket.TicketStatusID === statuses[i].TicketStatusID));
    }

    $('#ticket-status-label').toggleClass('ticket-closed', _ticketInfo.Ticket.IsClosed);

    var severities = top.Ts.Cache.getTicketSeverities();
    for (var i = 0; i < severities.length; i++) {
        AppendSelect('#ticket-severity', severities[i], 'severity', severities[i].TicketSeverityID, severities[i].Name, (_ticketInfo.Ticket.TicketSeverityID === severities[i].TicketSeverityID));
    }

    $('#ticket-visible').attr("checked", _ticketInfo.Ticket.IsVisibleOnPortal)

    $('#ticket-isKB').attr("checked", _ticketInfo.Ticket.IsKnowledgeBase)

    if (top.Ts.System.User.ChangeKbVisibility || top.Ts.System.User.IsSystemAdmin) {
        if (_ticketInfo.Ticket.IsKnowledgeBase) {
            $('#ticket-isKB').attr("checked", true);
            $('#ticket-group-KBCat').show();
        }
        else {
            $('#ticket-isKB').attr("checked", false);
            $('#ticket-group-KBCat').hide();
        }

        var categories = top.Ts.Cache.getKnowledgeBaseCategories();
        for (var i = 0; i < categories.length; i++) {
            var cat = categories[i].Category;
            AppendSelect('#ticket-KB-Category', cat, 'category', cat.CategoryID, cat.CategoryName, (_ticketInfo.Ticket.KnowledgeBaseCategoryID === cat.CategoryID));

            for (var j = 0; j < categories[i].Subcategories.length; j++) {
                var subcat = categories[i].Subcategories[j];
                AppendSelect('#ticket-KB-Category', subcat, 'subcategory', subcat.CategoryID, cat.CategoryName + ' -> ' + subcat.CategoryName, (_ticketInfo.Ticket.KnowledgeBaseCategoryID === subcat.CategoryID));
            }
        }

        $('#ticket-KB-Category').selectize();
    }
    else {
        $('#ticket-KBInfo').remove();
        $('#ticket-isKB-RO').text('Visible');
        $('#ticket-KB-Category-RO').text(_ticketInfo.Ticket.KnowledgeBaseCategoryName);
        $('#ticket-KBVisible-RO').show();
        $('#ticket-KBCat-RO').show();
    }

    $('#ticket-TimeSpent').text(top.Ts.Utils.getTimeSpentText(_ticketInfo.Ticket.HoursSpent));

    if (_ticketInfo.Ticket.IsClosed == true) {
        $('#ticket-DaysOpened').text(_ticketInfo.Ticket.DaysClosed).parent().prev().html('Days Closed');
    }
    else {
        $('#ticket-DaysOpened').text(_ticketInfo.Ticket.DaysOpened).parent().prev().html('Days Opened');
    }

    var dueDate = _ticketInfo.Ticket.DueDate;
    $('#ticket-DueDate').text((dueDate === null ? 'Unassigned' : dueDate.localeFormat(top.Ts.Utils.getDateTimePattern())));

    if (dueDate != null && dueDate < Date.now()) {
        $('#ticket-DueDate').addClass('nonrequired-field-error-font');
        $('#ticket-DueDate').parent().prev().addClass('nonrequired-field-error-font');
    }

    if (top.Ts.System.Organization.UseForums == true) {
        if (top.Ts.System.User.CanChangeCommunityVisibility) {
            var forumCategories = top.Ts.Cache.getForumCategories();
            for (var i = 0; i < forumCategories.length; i++) {
                var cat = forumCategories[i].Category;
                AppendSelect('#ticket-Community', cat, 'community', cat.CategoryID, cat.CategoryName, false);

                for (var j = 0; j < forumCategories[i].Subcategories.length; j++) {
                    var subcat = forumCategories[i].Subcategories[j];
                    AppendSelect('#ticket-Community', subcat, 'subcategory', subcat.CategoryID, cat.CategoryName + ' -> ' + subcat.CategoryName,false);
                }
            }
            $('#ticket-Community').val(_ticketInfo.Ticket.ForumCategory);
            $('#ticket-Community').selectize();
        }
        else {
            $('#ticket-Community-RO').text((_ticketInfo.Ticket.CategoryName == null ? 'Unassigned' : _ticketInfo.Ticket.CategoryDisplayString));
            $('#ticket-Community-RO').show();
            $('#ticket-Community').remove();
        }
    }
    else {
        $('#ticket-Community').remove();
        $('#ticket-Community-RO').remove();
    }
    
    $('.ticket-select').selectize();


    if (_ticketInfo.Ticket.SlaViolationTime === null) {
        $('#ticket-SLAStatus').addClass('ts-icon-sla-good');
        $('#ticket-SLANote').text('None');
    }
    else {
        $('#ticket-SLAStatus').addClass((_ticketInfo.Ticket.SlaViolationTime < 1 ? 'ts-icon-sla-bad' : (_ticketInfo.Ticket.SlaWarningTime < 1 ? 'ts-icon-sla-warning' : 'ts-icon-sla-good')));
        $('#ticket-SLANote').text(_ticketInfo.Ticket.SlaViolationDate.localeFormat(top.Ts.Utils.getDateTimePattern()));
    }

    SetupTicketPropertyEvents();
    SetupCustomerSection();
    SetupTagsSection();
    SetupProductSection();
    
    if (top.Ts.System.Organization.IsInventoryEnabled === true) {
        SetupInventorySection();
    }
    else 
    {
        $('#ticket-group-assets').hide();
    }

    SetupUserQueuesSection();
    SetupSubscribedUsersSection();
    SetupAssociatedTicketsSection();
    SetupRemindersSection();
    SetupCustomFieldsSection();
};

function AppendSelect(parent, data, type, id, name, isSelected) {
    var option = $('<option>').val(id).text(name).appendTo(parent).data(type, data);
    if (isSelected) {
        option.attr('selected', 'selected');
    }
};

function SetupTicketPropertyEvents() {
    $('#ticket-group').change(function (e) {
        var self = $(this);
        var GroupID = self.val();
        top.Ts.Services.Tickets.SetTicketGroup(_ticketID, GroupID, function (result) {
            if (result !== null) {
                window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changegroup", userFullName);
     
                if (GroupID != null) {
                    top.Ts.Services.Users.GetGroupUsers(GroupID, function (result) {
                        _ticketGroupUsers = result;
                    });
                }
                else {
                    _ticketGroupUsers = null;
                }
            }

            if (top.Ts.System.Organization.UpdateTicketChildrenGroupWithParent) {
                top.Ts.Services.Tickets.SetTicketChildrenGroup(_ticketID, GroupID);
            }
        },
        function (error) {
            alert('There was an error setting the group.');
        });
    });

    $('#ticket-type').change(function (e) {
        var self = $(this);
        var value = self.val();
        top.Ts.Services.Tickets.SetTicketType(_ticketID, value, function (result) {
            if (result !== null) {
                _ticketTypeID = value;
                //TODO:  This is not working.  It never changes the value of the control. 
                $('#ticket-status').val(result[0].TicketStatusID); 
                $('#ticket-status-label').toggleClass('ticket-closed', result[0].IsClosed);
                //TODO:  Add Custom Results Fields Logic here
                //appendCustomValues(result[1]);
                window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changetype", userFullName);
            }
        },
        function (error) {
            alert('There was an error setting your ticket type.');
        });
    });

    $('#ticket-status').change(function (e) {
        var self = $(this);
        var value = self.val();
        var status = top.Ts.Cache.getTicketStatus(value);
        isFormValidToClose(status.IsClosed, function (isValid) {
            if (isValid == true) {
                top.Ts.Services.Tickets.SetTicketStatus(_ticketID, value, function (result) {
                    if (result !== null) {
                        top.Ts.System.logAction('Ticket - Status Changed');
                        $('#ticket-status-label').toggleClass('ticket-closed', result.IsClosed);
                        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changestatus", userFullName);
                    }
                },
                function (error) {
                    alert('There was an error setting your ticket status.');
                });
            }
            else {
                alert("Please fill in the required fields before closing the ticket.");
                return;
            }
        });
    });

    $('#ticket-severity').change(function (e) {
        var self = $(this);
        var value = self.val();
        top.Ts.Services.Tickets.SetTicketSeverity(_ticketID, value, function (result) {
            if (result !== null) {
                window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeseverity", userFullName);
            }
        },
        function (error) {
            alert('There was an error setting your ticket severity.');
        });
    });

    $('#ticket-visible').change(function (e) {
        var self = $(this);
        
        if (top.Ts.System.User.ChangeTicketVisibility || top.Ts.System.User.IsSystemAdmin) {
            var value = self.is(":checked");
            top.Ts.System.logAction('Ticket - Visibility Changed');
            top.Ts.Services.Tickets.SetIsVisibleOnPortal(_ticketID, value, function (result) {
                  window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeisportal", userFullName);
            },
            function (error) {
                alert('There was an error saving the ticket portal visible\'s status.');
            });
        }
    });

    $('#ticket-isKB').change(function (e) {
        var self = $(this);

        if (top.Ts.System.User.ChangeTicketVisibility || top.Ts.System.User.IsSystemAdmin) {
            var value = self.is(":checked");
            top.Ts.System.logAction('Ticket - Visibility Changed');
            top.Ts.Services.Tickets.SetIsVisibleOnPortal(_ticketID, value, function (result) {
                window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeisportal", userFullName);
            },
            function (error) {
                alert('There was an error saving the ticket portal visible\'s status.');
            });
        }

        if (top.Ts.System.User.ChangeKbVisibility || top.Ts.System.User.IsSystemAdmin) {
            var value = self.is(":checked");
            top.Ts.System.logAction('Ticket - KB Status Changed');
            top.Ts.Services.Tickets.SetIsKB(_ticketID, value,
            function (result) {
                if (result === true) {
                    $('#ticket-group-KBCat').show();
                }
                else {
                    $('#ticket-group-KBCat').hide();
                }
                window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeiskb", userFullName);
            },
            function (error) {
                alert('There was an error saving the ticket knowlegdgebase\'s status.');
            });
        }
    });

    $('#ticket-KB-Category').change(function (e) {
        var self = $(this);
        var value = self.val();
        top.Ts.System.logAction('Ticket - KnowledgeBase Community Changed');
        top.Ts.Services.Tickets.SetTicketKnowledgeBaseCategory(_ticketID, value, function (result) {
            window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changekbcat", userFullName);
        },
        function (error) {
            alert('There was an error setting your ticket knowledgebase category.');
        });
    });

    $('#ticket-Community').change(function (e) {
        var self = $(this);
        var value = self.val();
        var oldCatName = _ticketInfo.Ticket.CategoryName;
        var newCatName = self.text();
        top.Ts.System.logAction('Ticket - Community Changed');
        top.Ts.Services.Tickets.SetTicketCommunity(_ticketID, value, oldCatName == null ? 'Unassigned' : oldCatName, newCatName == null ? 'Unassigned' : newCatName, function (result) {
            window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecommunity", userFullName);
        },
        function (error) {
            alert('There was an error setting your ticket community.');
        });
    });
}

function SetupCustomerSection() {
    AddCustomers(_ticketInfo.Customers);

    $('#ticket-Customers-Input').selectize({
        valueField: 'id',
        labelField: 'label',
        searchField: 'label',
        load: function (query, callback) {
            getCustomers(query, callback)
        },
        initData: true,
        preload: true,
        onLoad: function () {
            if (this.settings.initData === true) {
                this.settings.initData = false;
            }
        },
        onItemAdd: function (value, $item) {
            if (this.settings.initData === false) {
                var customerData = $item.data();

                top.Ts.Services.Tickets.AddTicketCustomer(_ticketID, customerData.type, value, function (customers) {
                    AddCustomers(customers);

                    if (customerData.type == "u") {
                        top.Ts.Services.Customers.LoadAlert(value, top.Ts.ReferenceTypes.Users, function (note) {
                            LoadTicketNotes(note);
                        });
                    }
                    else {
                        top.Ts.Services.Customers.LoadAlert(value, top.Ts.ReferenceTypes.Organizations, function (note) {
                            LoadTicketNotes(note);
                        });
                    }
                    
                    window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addcustomer", userFullName);
                }, function () {
                    $(this).parent().remove();
                    alert('There was an error adding the customer.');
                });
                this.removeItem(value, true);
                top.Ts.System.logAction('Ticket - Customer Added');
            }
        },
        plugins: {
            'sticky_placeholder': {}
        },
        render: {
            item: function (item, escape) {
                return '<div data-value="' + item.value + '" data-type="' + item.data + '" data-selectable="" class="option">'+ item.label+'</div>';
            },
            option: function (item, escape) {
                return '<div data-value="' + escape(item.value) + '" data-type="' + escape(item.data) + '" data-selectable="" class="option">' + item.label + '</div>';
            }
        }
    });

    //$("#ticket-Customers-Input").autocomplete({
    //    minLength: 2,
    //    source: getCustomers,
    //    select: function (event, ui) {
    //        $(this)
    //        .data('item', ui.item)

    //        var item = $(this).data('item');
    //        alertMessage = item;
    //        top.Ts.Services.Tickets.AddTicketCustomer(_ticketID, item.data, item.id, function (customers) {
    //            AddCustomers(customers);

    //            if (alertMessage.data == "u") {
    //                top.Ts.Services.Customers.LoadAlert(alertMessage.id, top.Ts.ReferenceTypes.Users, function (note) {
    //                    LoadTicketNotes(note);
    //                });
    //            }
    //            else {
    //                top.Ts.Services.Customers.LoadAlert(alertMessage.id, top.Ts.ReferenceTypes.Organizations, function (note) {
    //                    LoadTicketNotes(note);
    //                });
    //            }

    //            window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addcustomer", userFullName);
    //        }, function () {
    //            $(this).parent().remove();
    //            alert('There was an error adding the customer.');
    //        });
    //        top.Ts.System.logAction('Ticket - Customer Added');
    //    }
    //})
    //.data("autocomplete")._renderItem = function (ul, item) {
    //    return $("<li>")
    //        .append("<a>" + item.label + "</a>")
    //        .appendTo(ul);
    //};

    $('#ticket-Customer').on('click', 'a > span.tagRemove', function (e) {
        var self = $(this);
        var data = self.closest('a').data().tag;

        if (data.UserID) {
            top.Ts.Services.Tickets.RemoveTicketContact(_ticketID, data.UserID, function (customers) {
                AddCustomers(customers);
                window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removecontact", userFullName);
            }, function () {
                alert('There was a problem removing the contact from the ticket.');
            });
            top.Ts.System.logAction('Ticket - Contact Removed');
        }
        else
        {
            top.Ts.Services.Tickets.RemoveTicketCompany(_ticketID, data.OrganizationID, function (customers) {
                AddCustomers(customers);
                window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removecompany", userFullName);
            }, function () {
                alert('There was a problem removing the company from the ticket.');
            });
            top.Ts.System.logAction('Ticket - Customer Removed');
        }
    });

};

function AddCustomers(customers) {
    var customerDiv = $("#ticket-Customer");
    customerDiv.empty();
    $("#ticket-Customers-Input").val('');

    for (var i = 0; i < customers.length; i++) {
        var label = "";

        if (customers[i].Contact !== null && customers[i].Company !== null) {
            label = '<strong>' + customers[i].Contact + '</strong>' + '<br/>' + customers[i].Company;
        }
        else if (customers[i].Contact !== null) {
            label = '<strong>' + customers[i].Contact + '</strong>';
        }
        else if (customers[i].Company !== null) {
            label = '<strong>' + customers[i].Company + '</strong>';
        }

        PrependTag(customerDiv, customers[i].UserID, label, customers[i]);
    };
}

function SetupTagsSection() {
    AddTags(_ticketInfo.Tags);

    $("#ticket-tag-Input").autocomplete({
        minLength: 2,
        source: getTags,
        response: function (event, ui) {
            var inputValue = $(this).val();

            var filtered = $(ui.content).filter(function(){
                return this.value == inputValue;
            });

            if(filtered.length === 0){
                ui.content.push({
                    label: inputValue,
                    value: inputValue,
                    id: 0
                });
            }
        },
        select: function (event, ui) {
            $(this)
        .data('item', ui.item)

            top.Ts.Services.Tickets.AddTag(_ticketID, ui.item.value, function (tags) {
                if (tags !== null) {
                    AddTags(tags);
                    //window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addtag", userFullName);
                }

            }, function () {
                alert('There was an error adding the tag.');
            });
            top.Ts.System.logAction('Ticket - Added');
        }
    })
    .data("autocomplete")._renderItem = function (ul, item) {
        return $("<li>")
            .append("<a>" + item.label + "</a>")
            .appendTo(ul);
    };

    $('#ticket-tags').on('click', 'a > span.tagRemove', function (e) {
        var tag = $(this).parent()[0];
        if (tag) {
            top.Ts.Services.Tickets.RemoveTag(_ticketID, tag.id, function (tags) {
                tag.remove();
                window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removetag", userFullName);
            }, function () {
                alert('There was a problem removing the tag from the ticket.');
            });
        }
        else {
            alert('There was a problem removing the reminder from the ticket.');
        }

    });

};

function AddTags(tags) {
    var tagDiv = $("#ticket-tags");
    tagDiv.empty();
    $("#ticket-tag-Input").val('');

    for (var i = 0; i < tags.length; i++) {
        var label = label = '<strong>' + tags[i].Value + '</strong>'
        PrependTag(tagDiv, tags[i].TagID, label, tags[i]);
    };
}

function PrependTag(parent, id, value, data) {
    var _compiledTagTemplate = Handlebars.compile($("#ticket-tag").html());
    var tagHTML = _compiledTagTemplate({ id: id, value: value, data: data });
    parent.prepend(tagHTML);
}

function SetupProductSection() {
    top.Ts.Settings.Organization.read('ShowOnlyCustomerProducts', false, function (showOnlyCustomers) {
        if (showOnlyCustomers == "True") {
            top.Ts.Services.TicketPage.GetTicketCustomerProducts(_ticketID, function (CustomerProducts) {
                for (var i = 0; i < CustomerProducts.length; i++) {
                    AppendSelect('#ticket-Product', CustomerProducts[i], 'product', CustomerProducts[i].ProductID, CustomerProducts[i].Name, (CustomerProducts[i].ProductID === _ticketInfo.Ticket.ProductID));
                }
                $('#ticket-Product').selectize();
            });
        }
        else {
            var products = top.Ts.Cache.getProducts();
            for (var i = 0; i < products.length; i++) {
                AppendSelect('#ticket-Product', products[i], 'product', products[i].ProductID, products[i].Name, (products[i].ProductID === _ticketInfo.Ticket.ProductID));
            }
            $('#ticket-Product').selectize();
        }

        var product = top.Ts.Cache.getProduct(_ticketInfo.Ticket.ProductID);
        if (product !== null && product.Versions.length > 0)
        {
            var versions = product.Versions;
            for (var i = 0; i < versions.length; i++) {
                AppendSelect('#ticket-Versions', versions[i], 'version', versions[i].ProductVersionID, versions[i].VersionNumber, (versions[i].ProductVersionID === _ticketInfo.Ticket.ReportedVersionID));
                AppendSelect('#ticket-Resolved', versions[i], 'resolved', versions[i].ProductVersionID, versions[i].VersionNumber, (versions[i].ProductVersionID === _ticketInfo.Ticket.ResolvedVersionID));
            }
            $('#ticket-Versions').selectize();
            $('#ticket-Resolved').selectize();
        }
    })
}

function SetupInventorySection() {
    $('#ticket-Inventory-Input').selectize({
        valueField: 'id',
        labelField: 'label',
        searchField: 'label',
        load: function (query, callback) {
            getAssets(query, callback)

        },
        initData: true,
        preload: true,
        onLoad: function () {
            var assets = _ticketInfo.assets;
            if (this.settings.initData === true) {
                for (var i = 0; i < _ticketInfo.Assets.length ; i++) {
                    this.addItem(_ticketInfo.Assets[i].AssetID, false);
                }
                this.settings.initData = false;
            }
        },
        //render: {
        //    option: function (item, escape) {
        //        return '<div data-value="'+escape(item.id)+'" data-selectable="" class="option">'+escape(item.label)+'</div>';
        //    }
        //},
        onItemAdd: function (value, $item) {
            if (this.settings.initData === false) {
                top.Ts.Services.Tickets.AddTicketAsset(_ticketID, value, function (assets) {
                    //window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addasset", userFullName);
                    top.Ts.Services.Tickets.GetTicketCustomers(_ticketID, function (customers) {
                        AddCustomers(customers);
                    });
                }, function () {
                    alert('There was an error adding the asset.');
                });
                top.Ts.System.logAction('Ticket - Asset Added');
                $('#ticket-Inventory-Input').attr("placeholder", "Type your answer here");
            }
        },
        onItemRemove: function (value) {
            top.Ts.Services.Tickets.RemoveTicketAsset(_ticketID, value, function (assets) {
                //window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removeasset", userFullName);
            }, function () {
                alert('There was a problem removing the asset from the ticket.');
            });
        },
        plugins: {
            'sticky_placeholder': {}
        }
    });
}

function SetupUserQueuesSection() {
    $('#ticket-UserQueue-Input').selectize({
        valueField: 'id',
        labelField: 'label',
        searchField: 'label',
        load: function (query, callback) {
            getUsers(query, callback)

        },
        initData: true,
        preload: true,
        onLoad: function () {
            if (this.settings.initData === true) {
                for (var i = 0; i < _ticketInfo.Queuers.length ; i++) {
                    this.addItem(_ticketInfo.Queuers[i].UserID);
                }
                this.settings.initData = false;
            }
        },
        //render: {
        //    option: function (item, escape) {
        //        return '<div data-value="'+escape(item.id)+'" data-selectable="" class="option">'+escape(item.label)+'</div>';
        //    }
        //},
        onItemAdd: function (value, $item) {
            if (this.settings.initData === false) {
                top.Ts.Services.Tickets.SetQueue(_ticketID, true, value, function (queues) {
                    window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addqueue", userFullName);
                }, function () {
                    alert('There was an error adding the queue.');
                });
                top.Ts.System.logAction('Ticket - Enqueued');
                top.Ts.System.logAction('Queued');
            }
        },
        onItemRemove: function (value) {
            top.Ts.Services.Tickets.SetQueue(_ticketID, false, value, function (queues) {
                window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removequeue", userFullName);
            }, function () {
                alert('There was a problem removing the queue from the ticket.');
            });
            top.Ts.System.logAction('Ticket - Dequeued');
        },
        plugins: {
            'sticky_placeholder': {}
        }
    });
}

function SetupSubscribedUsersSection() {
    $('#ticket-SubscribedUsers-Input').selectize({
        valueField: 'id',
        labelField: 'label',
        searchField: 'label',
        load: function (query, callback) {
            getUsers(query, callback)

        },
        initData: true,
        preload: true,
        onLoad: function () {
            if (this.settings.initData === true) {
                for (var i = 0; i < _ticketInfo.Subscribers.length ; i++) {
                    this.addItem(_ticketInfo.Subscribers[i].UserID);
                }
                this.settings.initData = false;
            }
        },
        //render: {
        //    option: function (item, escape) {
        //        return '<div data-value="'+escape(item.id)+'" data-selectable="" class="option">'+escape(item.label)+'</div>';
        //    }
        //},
        onItemAdd: function (value, $item) {
            if (this.settings.initData === false) {
                top.Ts.Services.Tickets.SetSubscribed(_ticketID, true, value, function (subscribers) {
                    //window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addsubscriber", userFullName);
                }, function () {
                    alert('There was an error adding the subscriber.');
                });
                top.Ts.System.logAction('Ticket - User Subscribed');
            }
        },
        onItemRemove: function (value) {
            top.Ts.Services.Tickets.SetSubscribed(_ticketID, false, value, function (subscribers) {
                //window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removesubscriber", userFullName);
            }, function () {
                alert('There was a problem removing the subscriber from the ticket.');
            });
            top.Ts.System.logAction('Ticket - Subscriber Removed');
        },
        plugins: {
            'sticky_placeholder': {}
        }
    });
}

//TODO:  Not working but may just be the enviroment with DTSearch
function SetupAssociatedTicketsSection() {
    $('#ticket-AssociatedTickets-Input').selectize({
        valueField: 'id',
        labelField: 'label',
        searchField: 'label',
        load: function (query, callback) {
            getRelated(query, callback)
        },
        initData: true,
        preload: true,
        onLoad: function () {
            if (this.settings.initData === true) {
                for (var i = 0; i < _ticketInfo.Related.length ; i++) {
                    this.addItem(_ticketInfo.Related[i].TicketNumber);
                }
                this.settings.initData = false;
            }
        },
        //render: {
        //    option: function (item, escape) {
        //        return '<div data-value="'+escape(item.id)+'" data-selectable="" class="option">'+escape(item.label)+'</div>';
        //    }
        //},
        onItemAdd: function (value, $item) {
            
        },
        onItemRemove: function (value) {

        },
        plugins: {
            'sticky_placeholder': {}
        }
    });
}

function SetupRemindersSection() {
    AddReminders(_ticketInfo.Reminders);

    $('#ticket-reminder-date').datetimepicker({ useCurrent: true, format: 'MM/DD/YYYY hh:mm A', defaultDate: new Date() });

    var $reminderSelect = $('#ticket-reminder-who').selectize({
        valueField: 'id',
        labelField: 'label',
        searchField: 'label',
        load: function (query, callback) {
            top.Ts.Services.TicketPage.SearchUsers(query, function (result) {
                callback(result);
            });

        }
    });

    $('#ticket-reminder-save').click(function (e) {
        var selectizeControl = $reminderSelect[0].selectize;
        var date = top.Ts.Utils.getMsDate($('#ticket-reminder-date').val());
        var userid = selectizeControl.getValue();
        if (userid == "") {
            $('#ticket-reminder-who').parent().addClass('has-error').removeClass('has-success');
        }
        else {
            $('#ticket-reminder-who').closest('form-group').addClass('has-success').removeClass('has-error');
        }
        var title = $('#ticket-reminder-title').val();
        if (title == "") {
            $('#ticket-reminder-title').parent().addClass('has-error').removeClass('has-success');
        }
        else {
            $('#ticket-reminder-title').parent().addClass('has-success').removeClass('has-error');
        }

        top.Ts.Services.System.EditReminder(null, top.Ts.ReferenceTypes.Tickets, _ticketID, title, date, userid, function (result) {
            $('#reminder-success').show();
            var label = label = '<strong>' + ellipseString(result.Description, 30) + '</strong><br>' + result.DueDate.localeFormat(top.Ts.Utils.getDateTimePattern())
            PrependTag($("#ticket-reminder-span"), result.ReminderID, label, result);
            setTimeout(function () { $('#RemindersModal').modal('hide'); }, 2000);
        },
        function () {
            $('#reminder-error').show();
        });
    });

    $('#ticket-reminder-span').on('click', 'a > span.tagRemove', function (e) {
        var reminder = $(this).parent()[0];
        if (reminder) {
            top.Ts.Services.System.DismissReminder(reminder.id, function () {
                reminder.remove();
                window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removereminder", userFullName);
            }, function () {
                alert('There was a problem removing the reminder from the ticket.');
            });
        }
        else
        {
            alert('There was a problem removing the reminder from the ticket.');
        }

    });
}

function AddReminders(reminders) {
    var remindersDiv = $("#ticket-reminder-span");
    remindersDiv.empty();

    for (var i = 0; i < reminders.length; i++) {
        var label = label = '<strong>' + ellipseString(reminders[i].Description, 30) + '</strong><br>' + reminders[i].DueDate.localeFormat(top.Ts.Utils.getDateTimePattern())
        PrependTag(remindersDiv, reminders[i].ReminderID, label, reminders[i]);
    };
}

function SetupCustomFieldsSection() {
    AppenCustomValues(_ticketInfo.CustomValues);
}

function AppenCustomValues(fields) {
    var parentContainer = $('#ticket-group-custom-fields');
    if (fields === null || fields.length < 1) { parentContainer.empty().hide(); return; }
    parentContainer.show();
    _parentFields = [];

    for (var i = 0; i < fields.length; i++) {
        var field = fields[i];

        if (field.CustomFieldCategoryID == -1) {
            switch (field.FieldType) {
                case top.Ts.CustomFieldType.Text: AddCustomFieldEdit(field, parentContainer); break;
                case top.Ts.CustomFieldType.Date: AddCustomFieldDate(field, parentContainer); break;
                case top.Ts.CustomFieldType.Time: AddCustomFieldTime(field, parentContainer); break;
                case top.Ts.CustomFieldType.DateTime: AddCustomFieldDateTime(field, parentContainer); break;
                case top.Ts.CustomFieldType.Boolean: AddCustomFieldBool(field, parentContainer); break;
                case top.Ts.CustomFieldType.Number: AddCustomFieldNumber(field, parentContainer); break;
                case top.Ts.CustomFieldType.PickList: AddCustomFieldSelect(field, parentContainer, false); break;
                default:
            }
        }
    }
    appendCategorizedCustomValues(fields);
}

var appendCategorizedCustomValues = function (fields) {
    top.Ts.Services.CustomFields.GetCategories(top.Ts.ReferenceTypes.Tickets, _ticketTypeID, function (categories) {
        var container = $('#ticket-group-custom-fields');
        for (var j = 0; j < categories.length; j++) {
            var isFirstFieldAdded = true;
            for (var i = 0; i < fields.length; i++) {
                var item = null;

                var field = fields[i];

                if (field.CustomFieldCategoryID == categories[j].CustomFieldCategoryID) {
                    if (isFirstFieldAdded) {
                        isFirstFieldAdded = false;
                        var header = $('<label>').text(categories[j].Category).addClass('customFieldCategoryHeader');
                        container.append(header);
                    }

                    switch (field.FieldType) {
                        case top.Ts.CustomFieldType.Text: AddCustomFieldEdit(field, container); break;
                        case top.Ts.CustomFieldType.Date: AddCustomFieldDate(field, container); break;
                        case top.Ts.CustomFieldType.Time: AddCustomFieldTime(field, container); break;
                        case top.Ts.CustomFieldType.DateTime: AddCustomFieldDateTime(field, container); break;
                        case top.Ts.CustomFieldType.Boolean: AddCustomFieldBool(field, container); break;
                        case top.Ts.CustomFieldType.Number: AddCustomFieldNumber(field, container); break;
                        case top.Ts.CustomFieldType.PickList: AddCustomFieldSelect(field, container, false); break;
                        default:
                    }
                }
            }
        }
        appendConditionalFields();
    });
}

var appendConditionalFields = function () {
    for (var i = 0; i < _parentFields.length; i++) {
        var field = _parentFields[i].data('field');
        $('.' + field.CustomFieldID + 'children').remove();
        var parentContainer = $('#ticket-group-custom-fields');
        var childrenContainer = $('<div>').addClass(field.CustomFieldID + 'children form-horizontal').appendTo(parentContainer);
        appendMatchingParentValueFields(childrenContainer, field);
    }
}

var appendMatchingParentValueFields = function (container, parentField) {
    top.Ts.Services.Tickets.GetMatchingParentValueFields(_ticketID, parentField.CustomFieldID, parentField.Value, function (fields) {
        for (var i = 0; i < fields.length; i++) {
            var field = fields[i];
            var div = $('<div>').addClass('form-group form-group-sm').data('field', field);
            $('<label>').addClass('col-sm-4 control-label select-label').text(field.Name).appendTo(div);

            container.append(div);

            switch (field.FieldType) {
                case top.Ts.CustomFieldType.Text: AddCustomFieldEdit(field, div); break;
                case top.Ts.CustomFieldType.Date: AddCustomFieldDate(field, div); break;
                case top.Ts.CustomFieldType.Time: AddCustomFieldTime(field, div); break;
                case top.Ts.CustomFieldType.DateTime: AddCustomFieldDateTime(field, div); break;
                case top.Ts.CustomFieldType.Boolean: AddCustomFieldBool(field, div); break;
                case top.Ts.CustomFieldType.Number: AddCustomFieldNumber(field, div); break;
                case top.Ts.CustomFieldType.PickList: AddCustomFieldSelect(field, div, true); break;
                default:
            }
        }
    });
}

var AddCustomFieldEdit = function (field, parentContainer) {
    var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('form-group form-group-sm')
                            .data('field', field)
                            .appendTo(formcontainer)
                            .append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
    var inputContainer = $('<div>').addClass('col-sm-8 ticket-input-container').appendTo(groupContainer);
    var inputGroupContainer = $('<div>').addClass('input-group').appendTo(inputContainer);
    var input = $('<input type="text">')
                    .addClass('form-control ticket-simple-input')
                    .val(field.Value)
                    .appendTo(inputGroupContainer)
                    .after(getUrls(field.Value));


    if (field.Mask) {
        input.mask(field.Mask);
        input.attr("placeholder", field.Mask);
    }

    input.change(function (e) {
        var value = input.val();

        if (field.IsRequired && (value === null || $.trim(value) === '')) {
            groupContainer.addClass('hasError');
        }
        else {
            groupContainer.removeClass('hasError');
        }
        if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
            groupContainer.addClass('hasCloseError');
            alert("This field can not be cleared in a closed ticket");
            return;
        }
        else {
            groupContainer.removeClass('hasCloseError');
        }
        if (value === null || $.trim(value) === '') {
            groupContainer.addClass('isEmpty');
        }
        else {
            groupContainer.removeClass('isEmpty');
        }
        top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
            groupContainer.data('field', result);
            groupContainer.find('.external-link').remove();
            input.after(getUrls(result.Value));
            window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
        }, function () {
            alert("There was a problem saving your ticket property.");
        });
    });

    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        groupContainer.addClass('hasError');
    }
    if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
        groupContainer.addClass('hasCloseError');
    }
    if (field.IsRequiredToClose) {
        groupContainer.addClass('isRequiredToClose');
    }
    if (field.Value === null || $.trim(field.Value) === '') {
        groupContainer.addClass('isEmpty');
    }
}

var AddCustomFieldDate = function (field, parentContainer) {
    var date = field.Value == null ? null : top.Ts.Utils.getMsDate(field.Value);
    var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('form-group form-group-sm').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
    var dateContainer = $('<div>').addClass('col-sm-8 ticket-input-container').attr('style', 'padding-top: 3px;').appendTo(groupContainer);
    var dateLink = $('<a>')
                        .attr('href', '#')
                        .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDatePattern())))
                        .addClass('control-label')
                        .attr('style', 'padding-left: 5px;')
                        .appendTo(dateContainer);

    dateLink.click(function (e) {
        e.preventDefault();
        $(this).hide();
        var input = $('<input type="text">')
                        .addClass('form-control')
                        .val(date === null ? '' : date.localeFormat(top.Ts.Utils.getDatePattern()))
                        .datetimepicker({ pickTime: false })
                        .appendTo(dateContainer)
                        .focus();

        input.focusout(function (e) {
            var value = top.Ts.Utils.getMsDate(input.val());
            this.remove();
            dateLink.text((value === null ? 'Unassigned' : value.localeFormat(top.Ts.Utils.getDatePattern()))).show();

            if (field.IsRequired && (value === null || $.trim(value) === '')) {
                groupContainer.addClass('hasError');
            }
            else {
                groupContainer.removeClass('hasError');
            }
            if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
                groupContainer.addClass('hasCloseErrory');
                alert("This field can not be cleared in a closed ticket");
                return;
            }
            else {
                groupContainer.removeClass('hasCloseErrory');
            }
            if (value === null || $.trim(value) === '') {
                groupContainer.addClass('isEmpty');
            }
            else {
                groupContainer.removeClass('isEmpty');
            }

            top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
                window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
            }, function () {
                alert("There was a problem saving your ticket property.");
            });
        })
    });

    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        groupContainer.addClass('hasError');
    }
    if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
        groupContainer.addClass('hasCloseError');
    }
    if (field.IsRequiredToClose) {
        groupContainer.addClass('isRequiredToClose');
    }
    if (field.Value === null || $.trim(field.Value) === '') {
        groupContainer.addClass('isEmpty');
    }
}

var AddCustomFieldDateTime = function (field, parentContainer) {
    var date = field.Value == null ? null : top.Ts.Utils.getMsDate(field.Value);
    var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('form-group form-group-sm').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
    var dateContainer = $('<div>').addClass('col-sm-8 ticket-input-container').attr('style', 'padding-top: 3px;').appendTo(groupContainer);
    var dateLink = $('<a>')
                        .attr('href', '#')
                        .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDateTimePattern())))
                        .addClass('control-label')
                        .attr('style', 'padding-left: 5px;')
                        .appendTo(dateContainer);

    dateLink.click(function (e) {
        e.preventDefault();
        $(this).hide();
        var input = $('<input type="text">')
                        .addClass('form-control')
                        .val(date === null ? '' : date.localeFormat(top.Ts.Utils.getDateTimePattern()))
                        .datetimepicker()
                        .appendTo(dateContainer)
                        .focus();

        input.focusout(function (e) {
            var value = top.Ts.Utils.getMsDate(input.val());
            this.remove();
            dateLink.text((value === null ? 'Unassigned' : value.localeFormat(top.Ts.Utils.getDateTimePattern()))).show();

            if (field.IsRequired && (value === null || $.trim(value) === '')) {
                groupContainer.addClass('hasError');
            }
            else {
                groupContainer.removeClass('hasError');
            }
            if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
                groupContainer.addClass('hasCloseErrory');
                alert("This field can not be cleared in a closed ticket");
                return;
            }
            else {
                groupContainer.removeClass('hasCloseErrory');
            }
            if (value === null || $.trim(value) === '') {
                groupContainer.addClass('isEmpty');
            }
            else {
                groupContainer.removeClass('isEmpty');
            }

            top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
                window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
            }, function () {
                alert("There was a problem saving your ticket property.");
            });
        })
    });

    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        groupContainer.addClass('hasError');
    }
    if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
        groupContainer.addClass('hasCloseError');
    }
    if (field.IsRequiredToClose) {
        groupContainer.addClass('isRequiredToClose');
    }
    if (field.Value === null || $.trim(field.Value) === '') {
        groupContainer.addClass('isEmpty');
    }
}

var AddCustomFieldTime = function (field, parentContainer) {
    var date = field.Value == null ? null : top.Ts.Utils.getMsDate(field.Value);
    var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('form-group form-group-sm').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
    var dateContainer = $('<div>').addClass('col-sm-8 ticket-input-container').attr('style', 'padding-top: 3px;').appendTo(groupContainer);
    var dateLink = $('<a>')
                        .attr('href', '#')
                        .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getTimePattern())))
                        .addClass('control-label')
                        .attr('style', 'padding-left: 5px;')
                        .appendTo(dateContainer);

    dateLink.click(function (e) {
        e.preventDefault();
        $(this).hide();
        var input = $('<input type="text">')
                        .addClass('form-control')
                        .val(date === null ? '' : date.localeFormat(top.Ts.Utils.getTimePattern()))
                        .datetimepicker({ pickDate: false })
                        .appendTo(dateContainer)
                        .focus();

        input.focusout(function (e) {
            var value = top.Ts.Utils.getMsDate("1/1/1900 " + input.val());
            this.remove();
            dateLink.text((value === null ? 'Unassigned' : value.localeFormat(top.Ts.Utils.getTimePattern()))).show();

            if (field.IsRequired && (value === null || $.trim(value) === '')) {
                groupContainer.addClass('hasError');
            }
            else {
                groupContainer.removeClass('hasError');
            }
            if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
                groupContainer.addClass('hasCloseErrory');
                alert("This field can not be cleared in a closed ticket");
                return;
            }
            else {
                groupContainer.removeClass('hasCloseErrory');
            }
            if (value === null || $.trim(value) === '') {
                groupContainer.addClass('isEmpty');
            }
            else {
                groupContainer.removeClass('isEmpty');
            }

            top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
                window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
            }, function () {
                alert("There was a problem saving your ticket property.");
            });
        })
    });

    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        groupContainer.addClass('hasError');
    }
    if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
        groupContainer.addClass('hasCloseError');
    }
    if (field.IsRequiredToClose) {
        groupContainer.addClass('isRequiredToClose');
    }
    if (field.Value === null || $.trim(field.Value) === '') {
        groupContainer.addClass('isEmpty');
    }
}

var AddCustomFieldBool = function (field, parentContainer) {
    var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
    var groupContainer = $('<div>')
                            .addClass('form-group form-group-sm')
                            .data('field', field)
                            .appendTo(formcontainer)
                            .append($('<label>').addClass('col-sm-4 control-label').text(field.Name));
    var inputContainer = $('<div>').addClass('col-sm-8 ticket-input-container').appendTo(groupContainer);
    var inputDiv = $('<div>').addClass('checkbox ticket-checkbox').appendTo(inputContainer);
    var input = $('<input type="checkbox">').appendTo(inputDiv);
    var value = (field.Value === null || $.trim(field.Value) === '' || field.Value === 'False' ? false : true);
    input.attr("checked", value);

    input.change(function (e) {
        var isChecked = input.is(':checked')
        top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, isChecked, function (result) {
            groupContainer.data('field', result);
            window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
        }, function () {
            alert("There was a problem saving your ticket property.");
        });
    });
}

var AddCustomFieldNumber = function (field, parentContainer) {
    var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('form-group form-group-sm').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
    var inputContainer = $('<div>').addClass('col-sm-8 ticket-input-container').appendTo(groupContainer);
    var input = $('<input type="text">')
                    .addClass('form-control ticket-simple-input')
                    .val(field.Value)
                    .appendTo(inputContainer)
                    .numeric();

    input.change(function (e) {
        var value = input.val();

        if (field.IsRequired && (value === null || $.trim(value) === '')) {
            groupContainer.addClass('hasError');
        }
        else {
            groupContainer.removeClass('hasError');
        }
        if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
            groupContainer.addClass('hasCloseError');
            alert("This field can not be cleared in a closed ticket");
            return;
        }
        else {
            groupContainer.removeClass('hasCloseError');
        }
        if (value === null || $.trim(value) === '') {
            groupContainer.addClass('isEmpty');;
        }
        else {
            groupContainer.removeClass('isEmpty');
        }
        top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
            groupContainer.data('field', result);
            window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
        }, function () {
            alert("There was a problem saving your ticket property.");
        });
    });

    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        groupContainer.addClass('hasError');
    }
    if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
        groupContainer.addClass('hasCloseError');
    }
    if (field.IsRequiredToClose) {
        groupContainer.addClass('isRequiredToClose');
    }
    if (field.Value === null || $.trim(field.Value) === '') {
        groupContainer.addClass('isEmpty');
    }
}

var AddCustomFieldSelect = function (field, parentContainer, loadConditionalFields) {
    var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('form-group form-group-sm').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
    var selectContainer = $('<div>').addClass('col-sm-8 ticket-input-container').appendTo(groupContainer);
    var select = $('<select>').addClass('hidden-select').appendTo(selectContainer);
    var options = field.ListValues.split('|');
    
    if (field.Value == "") {
        $('<option>').text("unassigned").val("").appendTo(select);
        if (field.IsRequired) groupContainer.addClass('hasError');

    }
    for (var i = 0; i < options.length; i++) {
        var optionValue = options[i];
        var option = $('<option>').text(optionValue).val(optionValue).appendTo(select);
        if (field.Value === options[i]) option.attr('selected', 'selected');
    }
    select.selectize({
        allowEmptyOption: true,
        onItemAdd: function (value, $item) {
            if (field.IsRequired && field.IsFirstIndexSelect == true && value == "" ) {
                groupContainer.addClass('hasError');
            }
            else {
                groupContainer.removeClass('hasError');
            }

            if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && field.IsFirstIndexSelect == true && value == "") {
                groupContainer.addClass('hasCloseError');
                alert("This field can not be cleared in a closed ticket");
                return;
            }
            else {
                groupContainer.removeClass('hasCloseError');
            }

            if (field.IsFirstIndexSelect == true && value == "") {
                groupContainer.addClass('isEmpty');
            }
            else {
                groupContainer.removeClass('isEmpty');
            }

            top.Ts.System.logAction('Ticket - Custom Value Set');
            top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
                $('.' + field.CustomFieldID + 'children').remove();
                var childrenContainer = $('<div>').addClass(field.CustomFieldID + 'children form-horizontal').appendTo(parentContainer);
                appendMatchingParentValueFields(childrenContainer, result);
                window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
            }, function () {
                alert("There was a problem saving your ticket property.");
            });
        }
    });

    var items = field.ListValues.split('|');
    if (field.IsRequired && ((field.IsFirstIndexSelect == true && (items[0] == field.Value || field.Value == null || $.trim(field.Value) === '')) || (field.Value == null || $.trim(field.Value) === ''))) {
        groupContainer.addClass('hasError');
    }
    if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && ((field.IsFirstIndexSelect == true && (items[0] == field.Value || field.Value == null || $.trim(field.Value) === '')) || (field.Value == null || $.trim(field.Value) === ''))) {
        groupContainer.addClass('hasCloseError');
    }
    if (field.IsRequiredToClose) {
        groupContainer.addClass('isRequiredToClose');
    }
    if ((field.IsFirstIndexSelect == true && items[0] == field.Value) || field.Value == null || $.trim(field.Value) === '') {
        groupContainer.addClass('isEmpty');
    }

    if (loadConditionalFields) {
        $('.' + field.CustomFieldID + 'children').remove();
        var childrenContainer = $('<div>').addClass(field.CustomFieldID + 'children form-horizontal').appendTo(parentContainer);
        appendMatchingParentValueFields(childrenContainer, field);
    }
    else {
        _parentFields.push(groupContainer);
    }
}

var getUrls = function (input) {
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
        result = result + '<a target="_blank" class="valueLink" href="' + url + '" title="' + matchArray[0] + '"><i class="fa fa-external-link fa-lg custom-field-link"></i></a>';
    }

    if (result !== '')
    {
        return parentDiv.append(result);
    }

    return result;
}

function FetchTimeLineItems(start) {
    _isLoading = true;
    $('.results-loading').show();
    top.Ts.Services.TicketPage.GetTimeLineItems(_ticketID, start, function (TimeLineItems) {
        _timeLine = TimeLineItems;

        if (TimeLineItems.length < 1) {
            $('.results-loading').hide();
            $('.results-done').show();
        }
        else {
            //compile action template
            _compiledActionTemplate = Handlebars.compile($("#action-template").html());

            //create first timeline date marker if needed
            if (_currDateSpan == null) {
                _currDateSpan = _timeLine[0].item.DateCreated;
                var dateSpan = '<span class="label bgcolor-bluegray daybadge">' + _currDateSpan.localeFormat(top.Ts.Utils.getDatePattern()) + '</span>';
                $("#action-timeline").append(dateSpan);
            };

            jQuery.each(_timeLine, function (i, val) { CreateActionElement(val, true); });
            _isLoading = false;
            $('.results-loading').hide();
        };
    });

};

function CreateActionElement(val, ShouldAppend) {
    if (_currDateSpan.toDateString() !== val.item.DateCreated.toDateString()) {
        var dateSpan = '<span class="label bgcolor-bluegray daybadge">' + val.item.DateCreated.localeFormat(top.Ts.Utils.getDatePattern()) + '</span>';
        $("#action-timeline").append(dateSpan);
        _currDateSpan = val.item.DateCreated;
    }
    var html = _compiledActionTemplate(val);

    if (ShouldAppend) {
        $("#action-timeline").append(html);
    }
    else
    {
        $('#action-timeline li:nth-child(1)').after(html);
    }

};

function CreateHandleBarHelpers() {
    Handlebars.registerHelper('FormatDateTime', function (Date) {
        return Date.localeFormat(top.Ts.Utils.getDateTimePattern())
    });

    Handlebars.registerHelper('TimeLineLabel', function () {
        if (this.item.IsVisibleOnPortal) {
            return '<div class="bgcolor-green"><span class="bgcolor-green">&nbsp;</span><a href="#" class="action-option-visible">Public</a></div>';
        }
        else if (!this.item.IsWC) {
            return '<div class="bgcolor-orange"><span class="bgcolor-orange">&nbsp;</span><a href="#" class="action-option-visible">Private</a></div>';
        }
        else if (this.item.IsWC) {
            return '<div class="bgcolor-blue"><span class="bgcolor-blue">&nbsp;</span><label>WC</label></div>';
        }

        return '';
    });

    Handlebars.registerHelper('ActionData', function () {
        return JSON.stringify(this.item);
    });

    Handlebars.registerHelper('TagData', function () {
        return JSON.stringify(this.data);
    });

    Handlebars.registerHelper('ActionNumber', function () {
        _workingActionNumer = _workingActionNumer - 1;
        return _workingActionNumer + 1;
    });

    Handlebars.registerHelper('CanPin', function (options) {
        if (top.Ts.System.User.UserCanPinAction || top.Ts.System.User.IsSystemAdmin) { return options.fn(this); }
    });

    Handlebars.registerHelper('CanEdit', function (options) {
        var action = this.item;
        var canEdit = top.Ts.System.User.IsSystemAdmin || top.Ts.System.User.UserID === action.CreatorID;
        var restrictedFromEditingAnyActions = !top.Ts.System.User.IsSystemAdmin && top.Ts.System.User.RestrictUserFromEditingAnyActions;

        if (!(!top.Ts.System.User.AllowUserToEditAnyAction && (!canEdit || restrictedFromEditingAnyActions))) { return options.fn(this); }
    });

    Handlebars.registerHelper('CanKB', function (options) {
        if (top.Ts.System.User.ChangeKbVisibility || top.Ts.System.User.IsSystemAdmin) { return options.fn(this); }
    });

    Handlebars.registerHelper('CanMakeVisible', function (options) {
        if (top.Ts.System.User.ChangeTicketVisibility || top.Ts.System.User.IsSystemAdmin) { return options.fn(this); }
    });

    Handlebars.registerHelper('WCLikes', function () {
        if (this.Likes > 0) {
            return "+" + this.Likes;
        }
    });
};

function CreateTimeLineDelegates() {
    $("#action-timeline").on("mouseenter", ".action-options", function (event) {
        $(this).find(".action-options-icon").hide();
        $(this).find(".action-option-items").fadeIn();
    }).on("mouseleave", ".action-options", function (event) {
        $(this).find(".action-option-items").hide();
        $(this).find(".action-options-icon").fadeIn();
    });

    $('#action-timeline').on('click', 'a.action-option-pin', function (e) {
        e.preventDefault();
        e.stopPropagation();

        var self = $(this);
        var Action = self.closest('li').data().action;

        if (top.Ts.System.User.IsSystemAdmin || top.Ts.System.User.UserCanPinAction) {
            top.Ts.System.logAction('Ticket - Action Pin Icon Clicked');
            top.Ts.Services.Tickets.SetActionPinned(_ticketID, Action.RefID, !Action.IsPinned,
            function (result) {
                var parentLI = self.closest('li');
                parentLI.data().action.IsPinned = result;
                parentLI.find('a.ticket-action-pinned').toggleClass('hidden');
            }, function () {
                alert('There was an error editing this action.');
            });
        }

    });

    $('#action-timeline').on('click', 'a.ticket-action-pinned', function (e) {
        e.preventDefault();
        e.stopPropagation();

        var self = $(this);
        var action = self.closest('li').data().action;

        if (top.Ts.System.User.IsSystemAdmin || top.Ts.System.User.UserCanPinAction) {
            top.Ts.System.logAction('Ticket - Action Pin Icon Clicked');
            top.Ts.Services.Tickets.SetActionPinned(_ticketID, action.RefID, !action.IsPinned,
            function (result) {
                var parentLI = self.closest('li');
                parentLI.data().action.IsPinned = result;
                parentLI.find('a.ticket-action-pinned').toggleClass('hidden');
            }, function () {
                alert('There was an error editing this action.');
            });
        }
    });

    $('#action-timeline').on('click', 'a.action-option-kb', function (e) {
        e.preventDefault();
        e.stopPropagation();

        var self = $(this);
        var action = self.closest('li').data().action;

        if (top.Ts.System.User.ChangeKbVisibility || top.Ts.System.User.IsSystemAdmin) {
            top.Ts.System.logAction('Ticket - Action KB Icon Clicked');
            top.Ts.Services.Tickets.SetActionKb(action.RefID, !action.IsKnowledgeBase,
          function (result) {
              var parentLI = self.closest('li');
              parentLI.data().action.IsKnowledgeBase = result;
              parentLI.find('a.ticket-action-kb').toggleClass('hidden');
          }, function () {
              alert('There was an error editing this action.');
          });
        }
    });

    $('#action-timeline').on('click', 'a.ticket-action-kb', function (e) {
        e.preventDefault();
        e.stopPropagation();

        var self = $(this);
        var action = self.closest('li').data().action;

        if (top.Ts.System.User.ChangeKbVisibility || top.Ts.System.User.IsSystemAdmin) {
            top.Ts.System.logAction('Ticket - Action KB Icon Clicked');
            top.Ts.Services.Tickets.SetActionKb(action.RefID, !action.IsKnowledgeBase,
          function (result) {
              var parentLI = self.closest('li');
              parentLI.data().action.IsKnowledgeBase = result;
              parentLI.find('a.ticket-action-kb').toggleClass('hidden');
          }, function () {
              alert('There was an error editing this action.');
          });
        }
    });

    $('#action-timeline').on('click', 'a.action-option-visible', function (e) {
        e.preventDefault();
        e.stopPropagation();

        var self = $(this);
        var action = self.closest('li').data().action;

        if (top.Ts.System.User.ChangeTicketVisibility || top.Ts.System.User.IsSystemAdmin) {
            top.Ts.System.logAction('Ticket - Action Visible Icon Clicked');
            top.Ts.Services.Tickets.SetActionPortal(action.RefID, !action.IsVisibleOnPortal,
            function (result) {
                var parentLI = self.closest('li');
                parentLI.data().action.IsVisibleOnPortal = result;
                var badgeDiv = parentLI.find('div.ticket-badge');
                badgeDiv.empty();

                if (result) {
                    badgeDiv.html('<div class="bgcolor-green"><span class="bgcolor-green">&nbsp;</span><a href="#" class="action-option-visible">Public</a></div>');
                }
                else {
                    badgeDiv.html('<div class="bgcolor-orange"><span class="bgcolor-orange">&nbsp;</span><a href="#" class="action-option-visible">Private</a></div>');
                }

                top.Ts.Services.Tickets.GetAction(action.RefID, function (action) {
                    parentLI.find('div.timeline-body').html(action.Description);
                });

            }, function () {
                alert('There was an error editing this action.');
            });
        }
    });

    $('#action-timeline').on('click', 'a.ticket-action-edit', function (e) {
        e.preventDefault();
        e.stopPropagation();

        var self = $(this);
        var action = self.closest('li').data().action;

    });

    $('#action-timeline').on('click', 'a.action-option-delete', function (e) {
        e.preventDefault();
        e.stopPropagation();

        var self = $(this);
        var action = self.closest('li').data().action;

        if (confirm('Are you sure you would like to delete this action?')) {
            top.Ts.System.logAction('Ticket - Action Deleted');
            top.Ts.Services.Tickets.DeleteAction(action.RefID, function () {
                self.closest('li').remove();
                window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "deleteaction", userFullName);
            },
            function () { alert('There was an error deleting this action.'); });
        }

    });

    $('.frame-container').bind('scroll', function () {
        if ($(this).scrollTop() > 100) {
            $('.scrollup').fadeIn();
        } else {
            $('.scrollup').fadeOut();
        }

        if (_isLoading == true) return;
        if ($('.results-done').is(':visible')) return;

        if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
            var count = $('#action-timeline > li').length;
            FetchTimeLineItems($('#action-timeline > li').length);
        }
    });

    $('.scrollup').click(function () {
        $('.frame-container').animate({
            scrollTop: 0
        }, 600);
        return false;
    });

    $('.ticket-filter-public').click(function (e) {
        e.preventDefault();
        e.stopPropagation();

        var isVisible = $(this).data('visible');

        if (isVisible) {
            $('li > div.timeline-panel > div.ticket-badge > div.bgcolor-green').closest('li').hide();
            $('.filter-public').addClass('bgcolor-darkgray');
            $('.filter-public').removeClass('bgcolor-green');
        }
        else {
            $('li > div.timeline-panel > div.ticket-badge > div.bgcolor-green').closest('li').show();
            $('.filter-public').removeClass('bgcolor-darkgray');
            $('.filter-public').addClass('bgcolor-green');
        }

        $(this).data('visible', !isVisible)
    });

    $('.ticket-filter-private').click(function (e) {
        e.preventDefault();
        e.stopPropagation();

        var isVisible = $(this).data('visible');

        if (isVisible) {
            $('li > div.timeline-panel > div.ticket-badge > div.bgcolor-orange').closest('li').hide();
            $('.filter-private').addClass('bgcolor-darkgray');
            $('.filter-private').removeClass('bgcolor-orange');
        }
        else {
            $('li > div.timeline-panel > div.ticket-badge > div.bgcolor-orange').closest('li').show();
            $('.filter-private').removeClass('bgcolor-darkgray');
            $('.filter-private').addClass('bgcolor-orange');
        }

        $(this).data('visible', !isVisible)
    });

    $('.ticket-filter-wc').click(function (e) {
        e.preventDefault();
        e.stopPropagation();

        var isVisible = $(this).data('visible');

        if (isVisible) {
            $('li > div.timeline-panel > div.ticket-badge > div.bgcolor-blue').closest('li').hide();
            $('.filter-wc').addClass('bgcolor-darkgray');
            $('.filter-wc').removeClass('bgcolor-blue');
        }
        else {
            $('li > div.timeline-panel > div.ticket-badge > div.bgcolor-blue').closest('li').show();
            $('.filter-wc').removeClass('bgcolor-darkgray');
            $('.filter-wc').addClass('bgcolor-blue');
        }

        $(this).data('visible', !isVisible)
    });

    $('#action-timeline').on('click', '.wc-option-replyarea', function (e) {
        $(this).hide();
        $(this).parent().find('.wc-textarea').slideToggle("fast").find('textarea').focus();
    });

    $('#action-timeline').on('click', 'button.wc-textarea-send', function (e) {
        e.preventDefault();
        e.stopPropagation();
        var self = $(this);
        var action = self.closest('li').data().action;
        var replyText = self.closest('.wc-textarea').find('textarea').val();
        if (replyText.length > 0) {
            var commentinfo = new Object();
            commentinfo.Description = replyText;
            commentinfo.Attachments = new Array();
            commentinfo.ParentTicketID = action.RefID;

            commentinfo.PageType = -1;
            commentinfo.PageID = -1;

            commentinfo.Tickets = new Array();
            //$('#commentatt:first').find('.ticket-queue').find('.ticket-removable-item').each(function () {
            //    commentinfo.Tickets[commentinfo.Tickets.length] = $(this).data('Ticket');
            //});

            commentinfo.Groups = new Array();
            //$('#commentatt:first').find('.group-queue').find('.ticket-removable-item').each(function () {
            //    commentinfo.Groups[commentinfo.Groups.length] = $(this).data('Group');
            //});

            commentinfo.Products = new Array();
            //$('#commentatt:first').find('.product-queue').find('.ticket-removable-item').each(function () {
            //    commentinfo.Products[commentinfo.Products.length] = $(this).data('Product');
            //});

            commentinfo.Company = new Array();
            //$('#commentatt:first').find('.customer-queue').find('.ticket-removable-item').each(function () {
            //    commentinfo.Company[commentinfo.Company.length] = $(this).data('Company');
            //});

            commentinfo.User = new Array();
            //$('#commentatt:first').find('.user-queue').find('.ticket-removable-item').each(function () {
            //    commentinfo.User[commentinfo.User.length] = $(this).data('User');
            //});

            if (commentinfo.Tickets.length > 0) top.Ts.System.logAction('Water Cooler - Ticket Inserted');
            if (commentinfo.Groups.length > 0) top.Ts.System.logAction('Water Cooler - Group Inserted');
            if (commentinfo.Products.length > 0) top.Ts.System.logAction('Water Cooler - Product Inserted');
            if (commentinfo.Company.length > 0) top.Ts.System.logAction('Water Cooler - Company Inserted');
            if (commentinfo.User.length > 0) top.Ts.System.logAction('Water Cooler - User Inserted');

            //var attcontainer = $(this).parent().parent().find('#commentatt').find('.upload-queue div.ticket-removable-item');

            top.Ts.Services.WaterCooler.NewComment(top.JSON.stringify(commentinfo), function (Message) {
                var _compiledWCReplyTemplate = Handlebars.compile($("#wc-new-reply-template").html());
                var html = _compiledWCReplyTemplate(Message);
                self.closest('li').find('.timeline-wc-responses').append(html);
                self.parent().hide();
                self.parent().parent().find('.wc-option-replyarea').show();
                self.closest('.wc-textarea').find('textarea').val('');
            });
        }

    });

    $(document).click(function (e) {
        if ($(e.target).is('.wc-textarea *, .wc-option-replyarea, .wc-option-replyarea *')) return;
        $('.wc-textarea').hide();
        $('.wc-option-replyarea').show();
    });

    $('#action-timeline').on('click', 'a.wc-reply-like-link', function (e) {
        e.preventDefault();
        e.stopPropagation();

        var self = $(this);
        var hasLiked = self.data('liked');
        if (!hasLiked) {
            var messageID = self.closest('div.timeline-wc-reply').data().messageid;

            top.Ts.System.logAction('Water Cooler - Message Liked');
            top.Ts.Services.WaterCooler.AddCommentLike(messageID, function (likes) {
                var countSpan = self.find('.wc-reply-like-total');
                countSpan.html("+" + likes.length);
                self.data('liked', true);
                countSpan.next().hide();

                //window.top.chatHubClient.server.addLike(likes, messageID, thread.Message.MessageParent, _orgID);
            });
        };
    });

    $('.new-action-option-visible').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        if(this.text == 'Private')
        {
            FlipNewActionBadge(false);
        }
        else
        {
            FlipNewActionBadge(true);
        }

    })
};

function CreateTicketToolbarDomEvents() {
    $('#Ticket-Owner').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        top.Ts.System.logAction('Ticket - Take Ownership');
        top.Ts.Services.Tickets.AssignUser(_ticketID, top.Ts.System.User.UserID, function (userInfo) {
          var $select = $("#ticket-assigned").selectize();
          var selectize = $select[0].selectize;
          selectize.setValue(userInfo.UserID);
          window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeassigned", userFullName);
        }, function () {
            alert('There was an error taking ownwership of this ticket.');
        });
    });

    $('#Ticket-GetUpdate').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        top.Ts.System.logAction('Ticket - Request Update');
        top.Ts.Services.TicketPage.RequestUpdate(_ticketID, function (actionInfo) {
          //TODO:  add in update
          CreateActionElement(actionInfo, false);
            alert('An update has been requested for this ticket.');
        }, function () {
            alert('There was an error requesting an update for this ticket.');
        });
    });

    $("#Ticket-Merge-search").autocomplete({
        minLength: 2,
        source: selectTicket,
        select: function (event, ui) {
            if (ui.item.data == _ticketID) {
                alert("Sorry, but you can not merge this ticket into itself.");
                return;
            }

            $(this).data('ticketid', ui.item.data).removeClass('ui-autocomplete-loading');
            $(this).data('ticketnumber', ui.item.id);

            try {
                top.Ts.Services.Tickets.GetTicketInfo(ui.item.id, function (info) {
                    var descriptionString = info.Actions[0].Action.Description;

                    if (ellipseString(info.Actions[0].Action.Description, 30).indexOf("<img src") !== -1)
                        descriptionString = "This ticket starts off with an embedded/linked image. We have disabled this for the preview.";
                    else if (ellipseString(info.Actions[0].Action.Description, 30).indexOf(".viewscreencast.com") !== -1)
                        descriptionString = "This ticket starts off with an embedded recorde video.  We have disabled this for the preview.";
                    else
                        descriptionString = ellipseString(info.Actions[0].Action.Description, 30);

                    var ticketPreviewName = "<div><strong>Ticket Name:</strong> " + info.Ticket.Name + "</div>";
                    var ticketPreviewAssigned = "<div><strong>Ticket Assigned To:</strong> " + info.Ticket.UserName + "</div>";
                    var ticketPreviewDesc = "<div><strong>Ticket Desciption Sample:</strong> " + descriptionString + "</div>";

                    $('#ticketmerge-preview-details').after(ticketPreviewName + ticketPreviewAssigned + ticketPreviewDesc);
                    $('#dialog-ticketmerge-preview').show();
                    $('#dialog-ticketmerge-warning').show();
                    $(".dialog-ticketmerge").dialog("widget").find(".ui-dialog-buttonpane").find(":button:contains('OK')").prop("disabled", false).removeClass("ui-state-disabled");
                })
            }
            catch (e) {
                alert("Sorry, there was a problem loading the information for that ticket.");
            }
        },
        position: { my: "right top", at: "right bottom", collision: "fit flip" }
    });

    $('#ticket-merge-complete').click(function (e) {
      e.preventDefault();
      if ($('#Ticket-Merge-search').val() == "") {
        alert("Please select a valid ticket to merge");
        return;
      }

      if ($('#dialog-ticketmerge-confirm').prop("checked")) {
        var winningID = $('#Ticket-Merge-search').data('ticketid');
        var winningTicketNumber = $('#Ticket-Merge-search').data('ticketnumber');
        var JSTop = top;
        top.Ts.Services.Tickets.MergeTickets(winningID, _ticketID, MergeSuccessEvent(_ticketNumber, winningTicketNumber),
          function () {
          $('#merge-error').show();
        });
      }
      else {
        alert("You did not agree to the conditions of the merge. Please go back and check the box if you would like to merge.")
      }
    });

    $('#Ticket-Refresh').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        top.Ts.System.logAction('Ticket - Refreshed');
        top.Ts.MainPage.highlightTicketTab(_ticketNumber, false);
        window.location = window.location;
    });

    $('#Ticket-Subscribe').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        var self = $(this);
        var isSubscribed = _ticketInfo.Ticket.IsSubscribed;
        top.Ts.System.logAction('Ticket - Subscribed');
        top.Ts.Services.Tickets.SetSubscribed(_ticketID, !isSubscribed, null, function (subscribers) {
          _ticketInfo.Ticket.IsSubscribed = !isSubscribed;
          self.children().toggleClass('color-green');
          //TODO:  add append
            //appendSubscribers(subscribers);
        }, function () {
            alert('There was an error subscribing this ticket.');
        });
    });

    $('#Ticket-Queue').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        var self = $(this);
        var isQueued = _ticketInfo.Ticket.IsEnqueued;
        top.Ts.System.logAction('Ticket - Enqueued');
        top.Ts.System.logAction('Queued');
        top.Ts.Services.Tickets.SetQueue(_ticketID, !isQueued, null, function (queues) {
          _ticketInfo.Ticket.IsEnqueued = !isQueued;
          self.children().toggleClass('color-green');
          //TODO:  add append
            //appendQueues(queues);
        }, function () {
            alert('There was an error queueing this ticket.');
        });
    });

    $('#Ticket-Flag').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        var self = $(this);
        var isFlagged = _ticketInfo.Ticket.IsFlagged;
        _ticketInfo.Ticket.IsFlagged = !isFlagged;
        self.children().toggleClass('color-red');
        top.Ts.System.logAction('Ticket - Flagged');
        top.Ts.Services.Tickets.SetTicketFlag(_ticketID, !isFlagged, null, function () {
        }, function () {
            alert('There was an error unflagging to this ticket.');
        });
    });

    $('#Ticket-Print').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        top.Ts.System.logAction('Ticket - Printed');
        window.open('../../../TicketPrint.aspx?ticketid=' + _ticketID, 'TSPrint' + _ticketID);
    });

    $('#Ticket-Email').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        top.Ts.System.logAction('Ticket - Emailed');
        //$(".dialog-emailinput").dialog('open');
    });

    $('#Ticket-URL').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        top.Ts.System.logAction('Ticket - Shown URL');
        //$('.ticket-url').toggle();
    });
};

var MergeSuccessEvent = function(_ticketNumber, winningTicketNumber)
{
  $('#merge-success').show();
  setTimeout(function () { $('#MergeModal').modal('hide'); }, 2000);
  top.Ts.MainPage.closeTicketTab(_ticketNumber);
  top.Ts.MainPage.openTicket(winningTicketNumber, true);
  window.location = window.location;
  window.top.ticketSocket.server.ticketUpdate(_ticketNumber + "," + winningTicketNumber, "merge", userFullName);
}