/// <reference path="ts/ts.js" />
/// <reference path="ts/parent.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

var _taskParentID;
var _parentTaskName;
var _newTaskID;
var _noteID;

$(document).ready(function () {
    _newTaskID = null;
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.async = true;
    script.src = ('https:' === document.location.protocol ? 'https://' : 'http://') + 'www.dropbox.com/static/api/1/dropbox.js';
    var firstScript = document.getElementsByTagName('script')[0];
    script.setAttribute('data-app-key', 'ebdoql1dhyy7l72');
    script.setAttribute('id', 'dropboxjs');
    if (window.parent.Ts.System.User.OrganizationID != 1150007)
        firstScript.parentNode.insertBefore(script, firstScript);

    parent.Ts.MainPage.highlightNewTaskTab(true);

    $('body').layout({
        defaults: {
            spacing_open: 0,
            closable: false
        },
        north: {
            spacing_open: 1,
            size: 100
        },
        center: {
            maskContents: true
        }
    });

    setupDescriptionEditor();

    function setupDescriptionEditor() {
        initScheduledReportEditor($('#Description'), function (ed) {
            if (_noteID != null) {
                parent.Ts.Services.Customers.LoadNote(_noteID, function (note) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($('.activity-queue')).data('Activity', note.NoteID);


                    $('<span>')
                    .text(note.Title)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                });
            }
        },
        function (ed) {
            $('#inputName').focus();
        });
    };

    // debugger;
    _taskParentID = window.parent.parent.Ts.Utils.getQueryValue("taskparentid", window);
    _parentTaskName = decodeURI(escape(window.parent.parent.Ts.Utils.getQueryValue("parenttaskname", window)));

    _ticketNumber = window.parent.parent.Ts.Utils.getQueryValue("ticketnumber", window);
    _ticketName = decodeURI(escape(window.parent.parent.Ts.Utils.getQueryValue("ticketname", window)));

    _refType = window.parent.parent.Ts.Utils.getQueryValue("reftype", window);
    _refID = window.parent.parent.Ts.Utils.getQueryValue("refid", window);

    _noteID = window.parent.parent.Ts.Utils.getQueryValue("noteID", window);

    if (_taskParentID) {
        var parentName = $('<h6>')
            .addClass('parentName');

        $('<a>')
          .attr('href', '#')
          .addClass('parentLink')
          .data('taskID', _taskParentID)
          .text(_parentTaskName + ' >')
          .appendTo(parentName)

        $('.parentLinkContainer').prepend(parentName);

        $('#mainHeader').text('Subtask Information');
    }

    $('.parentLinkContainer').on('click', '.parentLink', function (e) {
        e.preventDefault();

        var id = $(this).data('taskID');
        parent.Ts.System.logAction('New Task - View Parent Task');
        parent.Ts.MainPage.openNewTask(id);
    });

    LoadUsers(function () {
        $("#ddlUser").val(window.parent.Ts.System.User.UserID);
    });
    initAssociationControls();
    resetDisplay();

    var ellipseString = function (text, max) { return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text; };

    function LoadUsers(callback) {
        parent.Ts.Services.Customers.LoadUsers(function (users) {
            $('<option>').attr('value', '-1').text('Unassigned').appendTo('#ddlUser');
            for (var i = 0; i < users.length; i++) {
                $('<option>').attr('value', users[i].UserID).text(users[i].FirstName + ' ' + users[i].LastName).data('o', users[i]).appendTo('#ddlUser');
            }
            callback();
        });
    }

    function initAssociationControls() {
        $("#attachmentinput").show();
        $(".arrow-up").css('left', '7px');

        $("#ticketinput").hide();
        $("#userinput").hide();
        $("#customerinput").hide();
        $("#contactinput").hide();
        $("#groupinput").hide();
        $("#productinput").hide();
        $("#activityinput").hide();
    }

    function resetDisplay() {
        $("#inputName").empty();
        $("#Description").empty();
        $("#cbComplete").attr('checked', false);
        $("#DueDate").empty();
        $("#cbReminder").attr('checked', true);
        $("#ReminderDate").empty();
        $('#commentatt').find('.upload-queue').empty();
        $('#commentatt').find('.ticket-queue').empty();
        $('#commentatt').find('.group-queue').empty();
        $('#commentatt').find('.customer-queue').empty();
        $('#commentatt').find('.contact-queue').empty();
        $('#commentatt').find('.user-queue').empty();
        $('#commentatt').find('.product-queue').empty();
        $('#commentatt').find('.activity-queue').empty();
    }

    $(".maincontainer").on("keypress", "input", (function (evt) {
        //Deterime where our character code is coming from within the event
        var charCode = evt.charCode || evt.keyCode;
        if (charCode == 13) { //Enter key's keycode
            return false;
        }
    }));

    window.parent.parent.Ts.Services.Customers.GetDateFormat(false, function (dateformat) {
        //$('.datepicker').attr("data-format", dateformat);
        //$('.datepicker').datetimepicker({ pickTime: false });

        //$('#DueDate').attr("data-format", dateformat);
        //$('#ReminderDate').attr("data-format", dateformat);
        $('.datetimepicker').datetimepicker({});
    });

    $('#clearDueDate').click(function (e) {
        top.Ts.System.logAction('New Task - Clear Due Date');
        $('#DueDate').val('');
    });

    $('#clearReminderDate').click(function (e) {
        top.Ts.System.logAction('New Task - Clear Reminder Date');
        $('#ReminderDate').val('');
    });

    $('#cbReminder').on('click', function () {
        if ($(this).prop("checked") == true) {
            $('#reminderDateForm').show();
        }
        else {
            $('#reminderDateForm').hide();
        }
    });

    $('.addatt').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#activityinput").hide();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#contactinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").show();
        $(this).parent().parent().find("#ticketinsert").show();
        $(this).parent().find(".arrow-up").css('left', '6px');
        $('#associationsBreak').addClass('associationsBreakAdjustement');
    }).tooltip();
    $('.addticket').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#activityinput").hide();
        $(this).parent().parent().find("#ticketinput").show();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#contactinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '37px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();
    $('.adduser').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#activityinput").hide();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#contactinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").show();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '70px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();
    $('.addcustomer').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#activityinput").hide();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").show();
        $(this).parent().parent().find("#contactinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '101px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();
    $('.addcontact').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#activityinput").hide();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#contactinput").show();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '136px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();
    $('.addgroup').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#activityinput").hide();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").show();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#contactinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '173px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();
    $('.addproduct').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#activityinput").hide();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#contactinput").hide();
        $(this).parent().parent().find("#productinput").show();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '210px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();
    $('.addactivity').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#activityinput").show();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#contactinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '245px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();

    var execGetCustomer = null;
    function getCustomers(request, response) {
        if (execGetCustomer) { execGetCustomer._executor.abort(); }
        execGetCustomer = window.parent.parent.Ts.Services.Organizations.WCSearchOrganization(request.term, function (result) {
            response(result);
        });
    }

    var execGetContact = null;
    function getContacts(request, response) {
        if (execGetContact) { execGetContact._executor.abort(); }
        execGetContact = parent.parent.Ts.Services.Organizations.GetContacts(request.term, function (result) { response(result); });
    }

    var execGetUsers = null;
    function getUsers(request, response) {
        if (execGetUsers) { execGetUsers._executor.abort(); }
        execGetUsers = window.parent.parent.Ts.Services.Users.SearchUsers(request.term, function (result) { response(result); });
    }

    var execGetTicket = null;
    function getTicketsByTerm(request, response) {
        if (execGetTicket) { execGetTicket._executor.abort(); }
        //execGetTicket = Ts.Services.Tickets.GetTicketsByTerm(request.term, function (result) { response(result); });
        execGetTicket = window.parent.parent.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) {
            $('.main-quick-ticket').removeClass('ui-autocomplete-loading');
            response(result);
        });

    }

    var execGetGroups = null;
    function getGroupsByTerm(request, response) {
        if (execGetGroups) { execGetGroups._executor.abort(); }
        execGetTicket = window.parent.parent.Ts.Services.WaterCooler.GetGroupsByTerm(request.term, function (result) { response(result); });
    }

    var execGetProducts = null;
    function getProductByTerm(request, response) {
        if (execGetProducts) { execGetProducts._executor.abort(); }
        execGetProducts = window.parent.parent.Ts.Services.WaterCooler.GetProductsByTerm(request.term, function (result) { response(result); });
    }

    var execSelectNote = null;
    function selectNote(request, response) {
        if (execSelectNote) {
            execSelectNote._executor.abort();
        }
        execSelectNote = window.parent.parent.Ts.Services.Customers.SearchNotes(request.term, function (result) {
            response(result);
        });
    }

    $('.user-search')
    .focusin(function () { $(this).val('').removeClass('user-search-blur'); })
    .focusout(function () { $(this).val('Search for a user...').addClass('user-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('user-search-blur'); })
    .val('Search for a user...')
    .autocomplete({
        minLength: 3,
        source: getUsers,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.user-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('User')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.user-queue')).data('User', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

    $('.company-search')
    .focusin(function () { $(this).val('').removeClass('company-search-blur'); })
    .focusout(function () { $(this).val('Search for a company...').addClass('company-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('company-search-blur'); })
    .val('Search for a company...')
    .autocomplete({
        minLength: 3,
        source: getCustomers,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.customer-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Company')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.customer-queue')).data('Company', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

    $('.contact-search')
    .focusin(function () { $(this).val('').removeClass('contact-search-blur'); })
    .focusout(function () { $(this).val('Search for a contact...').addClass('contact-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('contact-search-blur'); })
    .val('Search for a contact...')
    .autocomplete({
        minLength: 3,
        source: getContacts,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.contact-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Contact')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var x = $(this).parent().parent();
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.contact-queue')).data('Contact', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

    $('.main-quick-ticket')
    .focusin(function () { $(this).val('').removeClass('main-quick-ticket-blur'); })
    .focusout(function () { $(this).val('Search for a ticket...').addClass('main-quick-ticket-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('main-quick-ticket-blur'); })
    .val('Search for a ticket...')
    .autocomplete({
        minLength: 2, source: getTicketsByTerm, delay: 300,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.ticket-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Ticket')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.ticket-queue')).data('Ticket', ui.item.data);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $('.main-quick-ticket').removeClass('ui-autocomplete-loading');
            return false;
        }
    });

    $('.group-search')
    .focusin(function () { $(this).val('').removeClass('group-search-blur'); })
    .focusout(function () { $(this).val('Search for a group...').addClass('group-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('group-search-blur'); })
    .val('Search for a group...')
    .autocomplete({
        minLength: 2,
        source: getGroupsByTerm,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.group-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Group')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.group-queue')).data('Group', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $(this).val("");
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

    $('.product-search')
    .focusin(function () { $(this).val('').removeClass('product-search-blur'); })
    .focusout(function () { $(this).val('Search for a product...').addClass('product-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('product-search-blur'); })
    .val('Search for a product...')
    .autocomplete({
        minLength: 3,
        source: getProductByTerm,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.product-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Product')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.product-queue')).data('Product', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

    $('.activity-search')
    .focusin(function () { $(this).val('').removeClass('activity-search-blur'); })
    .focusout(function () { $(this).val('Search for an activity...').addClass('activity-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('activity-search-blur'); })
    .val('Search for an activity...')
    .autocomplete({
        minLength: 3,
        source: selectNote,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.activity-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Activity')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.activity-queue')).data('Activity', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

    $('.file-upload').fileupload({
        namespace: 'task_attachment',
        dropZone: $('.file-upload'),
        add: function (e, data) {
            for (var i = 0; i < data.files.length; i++) {
                var bg = $('<div>')
                .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                .appendTo($(this).parent().parent().find('.upload-queue'));

                data.context = bg;
                bg.data('data', data);

                $('<span>')
                .text(ellipseString(data.files[i].name, 20) + '  (' + window.parent.parent.Ts.Utils.getSizeString(data.files[i].size) + ')')
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
                    $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                })
                .appendTo(bg);

                $('<span>')
                .addClass('ui-icon ui-icon-cancel')
                .hide()
                .click(function (e) {
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
        },
        progress: function (e, data) {
            data.context.find('.progress').progressbar('value', parseInt(data.loaded / data.total * 90, 10));
        },
        start: function (e, data) {
            $(this).parent().parent().find('.progress').progressbar().show();
            $(this).parent().parent().find('.upload-queue .ui-icon-close').hide();
            $(this).parent().parent().find('.upload-queue .ui-icon-cancel').show();
        },
        stop: function (e, data) {
            $(this).parent().parent().find('.progress').progressbar('value', 100);
            window.parent.parent.Ts.System.logAction('New Task - Attachment Added');
            $('.commentcontainer').hide();
            $('.faketextcontainer').show();
            $('#messagecontents').val('');
            resetDisplay();
            parent.Ts.MainPage.openNewTask(_newTaskID);
            parent.Ts.MainPage.highlightNewTaskTab(false);
            parent.Ts.MainPage.closenewTaskTab();
        }
    });

    $('#taskSaveBtn').click(function (e) {
        e.preventDefault();
        e.stopPropagation();

        $('#taskSaveBtn').prop("disabled", true);

        if ($("#inputName").val().length < 1) {
            alert("Please enter a name");
            return;
        }

        parent.Ts.System.logAction('New Task - Save New Task');

        var taskInfo = new Object();
        taskInfo.ParentID = _taskParentID;
        taskInfo.Name = $("#inputName").val();
        taskInfo.Description = $("#Description").val();
        if ($("#ddlUser").val() != -1) {
            taskInfo.UserID = $("#ddlUser").val();
        }
        taskInfo.IsComplete = $("#cbComplete").prop('checked');
        taskInfo.DueDate = $("#DueDate").val();
        taskInfo.IsDismissed = !$("#cbReminder").prop('checked');
        taskInfo.Reminder = $("#Reminder").val();

        taskInfo.Tickets = new Array();
        $('#commentatt:first').find('.ticket-queue').find('.ticket-removable-item').each(function () {
            taskInfo.Tickets[taskInfo.Tickets.length] = $(this).data('Ticket');
        });

        taskInfo.Groups = new Array();
        $('#commentatt:first').find('.group-queue').find('.ticket-removable-item').each(function () {
            taskInfo.Groups[taskInfo.Groups.length] = $(this).data('Group');
        });

        taskInfo.Products = new Array();
        $('#commentatt:first').find('.product-queue').find('.ticket-removable-item').each(function () {
            taskInfo.Products[taskInfo.Products.length] = $(this).data('Product');
        });

        taskInfo.Company = new Array();
        $('#commentatt:first').find('.customer-queue').find('.ticket-removable-item').each(function () {
            taskInfo.Company[taskInfo.Company.length] = $(this).data('Company');
        });

        taskInfo.Contacts = new Array();
        $('#commentatt:first').find('.contact-queue').find('.ticket-removable-item').each(function () {
            taskInfo.Contacts[taskInfo.Contacts.length] = $(this).data('Contact');
        });

        taskInfo.User = new Array();
        $('#commentatt:first').find('.user-queue').find('.ticket-removable-item').each(function () {
            taskInfo.User[taskInfo.User.length] = $(this).data('User');
        });

        taskInfo.Activities = new Array();
        $('#commentatt:first').find('.activity-queue').find('.ticket-removable-item').each(function () {
            taskInfo.Activities[taskInfo.Activities.length] = $(this).data('Activity');
        });

        if (taskInfo.Tickets.length > 0) window.parent.parent.Ts.System.logAction('New Task - Ticket Inserted');
        if (taskInfo.Groups.length > 0) window.parent.parent.Ts.System.logAction('New Task - Group Inserted');
        if (taskInfo.Products.length > 0) window.parent.parent.Ts.System.logAction('New Task - Product Inserted');
        if (taskInfo.Company.length > 0) window.parent.parent.Ts.System.logAction('New Task - Company Inserted');
        if (taskInfo.Contacts.length > 0) window.parent.parent.Ts.System.logAction('New Task - Contact Inserted');
        if (taskInfo.User.length > 0) window.parent.parent.Ts.System.logAction('New Task - User Inserted');
        if (taskInfo.Activities.length > 0) window.parent.parent.Ts.System.logAction('New Task - Activity Inserted');

        var attcontainer = $(this).parent().parent().find('#commentatt').find('.upload-queue div.ticket-removable-item');

        window.parent.parent.Ts.Services.Task.NewTask(parent.JSON.stringify(taskInfo), function (newTask) {
            if (attcontainer.length > 0) {
                _newTaskID = newTask.TaskID;
                attcontainer.each(function (i, o) {
                    var data = $(o).data('data');
                    data.url = '../../../Upload/Tasks/' + newTask.TaskID;
                    data.jqXHR = data.submit();
                    $(o).data('data', data);
                });
            }
            else {
                parent.Ts.MainPage.openNewTask(newTask.TaskID);
                parent.Ts.MainPage.highlightNewTaskTab(false);
                parent.Ts.MainPage.closenewTaskTab();
            }
        });
        $(this).parent().removeClass("saving");
        $('.frame-content').animate({ scrollTop: 0 }, 600);
    });

    $('#taskCancelBtn').click(function (e) {
        parent.Ts.MainPage.closenewTaskTab();
    });

    //if these are passed in the url make the necessary association
    if (_refType && _refID) {
        switch (Number(_refType)) {
            case 6:
                break;
            case 9:
                var bg = $('<div>')
                        .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                        .appendTo($('#taskForm').find('.customer-queue')).data('Company', _refID);

                $('<span>')
                .text(_ticketName)
                .addClass('filename')
                .appendTo(bg);

                $('<span>')
                .addClass('ui-icon ui-icon-close')
                .click(function (e) {
                    e.preventDefault();
                    $('#taskForm').find('#commentatt').closest('div').fadeOut(500, function () { $('#commentatt').remove(); });
                })
                .appendTo(bg);
                break;
            case 13:
                break;
            case 17:
                var bg = $('<div>')
                        .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                        .appendTo($('#taskForm').find('.ticket-queue')).data('Ticket', _refID);

                $('<span>')
                .text(_ticketNumber + ": " + _ticketName)
                .addClass('filename')
                .appendTo(bg);

                $('<span>')
                .addClass('ui-icon ui-icon-close')
                .click(function (e) {
                    e.preventDefault();
                    $('#taskForm').find('#commentatt').closest('div').fadeOut(500, function () { $('#commentatt').remove(); });
                })
                .appendTo(bg);
                break;
            case 22:
                var bg = $('<div>')
                        .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                        .appendTo($('#taskForm').find('.user-queue')).data('User', _refID);

                $('<span>')
                .text(_ticketName)
                .addClass('filename')
                .appendTo(bg);

                $('<span>')
                .addClass('ui-icon ui-icon-close')
                .click(function (e) {
                    e.preventDefault();
                    $('#taskForm').find('#commentatt').closest('div').fadeOut(500, function () { $('#commentatt').remove(); });
                })
                .appendTo(bg);
                break;
            case 32:
            default:
                functionName = null;
        }
    }
});
