var dateFormat;
var _dueDate = null;
var _ticketGroupID = null;
var _ticketGroupUsers = null;
var _ticketTypeID = null;
var _parentFields = [];
var _productFamilyID = null;
var _lastTicketTypeID = null;
var _ticketID = null;
var _doClose = false;
var canEdit = parent.Ts.System.User.IsSystemAdmin || parent.Ts.System.User.ChangeKbVisibility;

var _timerid;
var _timerElapsed = 0;
var speed = 50, counter = 0, start;
var userFullName = parent.Ts.System.User.FirstName + " " + parent.Ts.System.User.LastName;

var clueTipOptions = parent.Ts.Utils.getClueTipOptions(null);

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
var defaultTemplateText = "";


var prevProduct;


var getCustomers = function (request, response) {
    if (execGetCustomer) { execGetCustomer._executor.abort(); }
    execGetCustomer = parent.Ts.Services.TicketPage.GetUserOrOrganizationForTicket(request, function (result) { response(result); });
}

var getTags = function (request, response) {
    if (execGetTags) { execGetTags._executor.abort(); }
    execGetTags = parent.Ts.Services.Tickets.SearchTags(request.term, function (result) { response(result); });
}

var getAssets = function (request, response) {
    if (execGetAsset) { execGetAsset._executor.abort(); }
    execGetAsset = parent.Ts.Services.Assets.FindAsset(request, function (result) { response(result); });
}

var getUsers = function (request, response) {
    if (execGetUsers) { execGetUsers._executor.abort(); }
    execGetUsers = parent.Ts.Services.TicketPage.SearchUsers(request, function (result) { response(result); });
}

var getRelated = function (request, response) {
    if (execGetRelated) { execGetRelated._executor.abort(); }
    execGetRelated = parent.Ts.Services.Tickets.SearchTickets(request, null, function (result) { response(result); });
}

var getCompany = function (request, response) {
    if (execGetCompany) { execGetCompany._executor.abort(); }
    execGetCompany = parent.Ts.Services.Organizations.WCSearchOrganization(request, function (result) { response(result); });
}

var selectTicket = function (request, response) {
    if (execSelectTicket) { execSelectTicket._executor.abort(); }
    var filter = $(this.element).data('filter');
    if (filter === undefined) {
        execSelectTicket = parent.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) { response(result); });
    } else {
        execSelectTicket = parent.Ts.Services.Tickets.SearchTickets(request.term, filter, function (result) { response(result); });
    }
}

var ellipseString = function (text, max) { return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text; };

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
    if (result !== '') {
        return parentDiv.append(result);
    }
    return result;
};

var tickettimer = function () {
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

Selectize.define('no_results', function (options) {
    var self = this;

    options = $.extend({
                  message: 'No results found.', html: function (data) {
                      return ('<div class="selectize-dropdown ' + data.classNames + ' dropdown-empty-message">' + '<div class="selectize-dropdown-content" style="padding: 3px 12px">' + data.message + '</div>' + '</div>');
                  }
              }, options);

    self.displayEmptyResultsMessage = function () {
        this.$empty_results_container.css('top', this.$control.outerHeight());
        this.$empty_results_container.show();
    };

    self.refreshOptions = (function () {
        var original = self.refreshOptions;

        return function () {
            original.apply(self, arguments);
            this.hasOptions ? this.$empty_results_container.hide() :
                this.displayEmptyResultsMessage();
        }
    })();

    self.onKeyDown = (function () {
        var original = self.onKeyDown;

        return function (e) {
            original.apply(self, arguments);
            if (e.keyCode === 27) {
                this.$empty_results_container.hide();
            }
        }
    })();

    self.onBlur = (function () {
        var original = self.onBlur;

        return function () {
            original.apply(self, arguments);
            this.$empty_results_container.hide();
        };
    })();

    self.setup = (function () {
        var original = self.setup;
        return function () {
            original.apply(self, arguments);
            self.$empty_results_container = $(options.html($.extend({
                classNames: self.$input.attr('class')
            }, options)));
            self.$empty_results_container.insertBefore(self.$dropdown);
            self.$empty_results_container.hide();
        };
    })();
});

$(document).ready(function () {
    apiKey = "45228242";
    SetupDescriptionEditor();
    LoadTicketPageOrder();
    SetupActionTimers();
    $('.page-loading').hide().next().show();
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.async = true;
    script.src = ('https:' === document.location.protocol ? 'https://' : 'http://') + 'www.dropbox.com/static/api/1/dropbox.js';
    var firstScript = document.getElementsByTagName('script')[0];
    script.setAttribute('data-app-key', 'ebdoql1dhyy7l72');
    script.setAttribute('id', 'dropboxjs');
    firstScript.parentNode.insertBefore(script, firstScript);
});

function LoadTicketPageOrder() {
    parent.Ts.Services.TicketPage.GetTicketPageOrder("NewTicketFieldsOrder", function (order) {
        jQuery.each(order, function (i, val) { if (val.Disabled == "false") AddTicketProperty(val); });
        SetupTicketProperties();
    });
};

function AddTicketProperty(item) {
    var hbrs = "ticket-group-" + item.CatID;
    var hbrs = hbrs.toLowerCase();
    var compiledTemplate = Handlebars.templates[hbrs];
    $('#ticket-properties-area').append(compiledTemplate);
};

function UpdateTicketGroups(callback) {
    var selectizeGroup = $('#ticket-group')[0].selectize;
    selectizeGroup.clear(true);
    selectizeGroup.clearOptions();
    selectizeGroup.addOption({ value: -1, text: 'Unassigned' });

    var persistedGroup = false;
    if (_ticketGroupID == null) {
        selectizeGroup.addItem(-1, false);
        persistedGroup = true;
    }
    var groups = parent.Ts.Cache.getGroups();
    if (parent.Ts.System.Organization.UseProductFamilies && _productFamilyID != null) {
        for (var i = 0; i < groups.length; i++) {
            if (groups[i].ProductFamilyID == null || _productFamilyID == groups[i].ProductFamilyID) {
                selectizeGroup.addOption({ value: groups[i].GroupID, text: groups[i].Name });
                if (_ticketGroupID == groups[i].GroupID) {
                    selectizeGroup.addItem(groups[i].GroupID, false);
                    persistedGroup = true;
                }
            }
        }
    } else {
        persistedGroup = true;
        for (var i = 0; i < groups.length; i++) {
            selectizeGroup.addOption({ value: groups[i].GroupID, text: groups[i].Name });
            if (_ticketGroupID == groups[i].GroupID) {
                selectizeGroup.addItem(groups[i].GroupID, false);
            }
        }
    }
    if (!persistedGroup) {
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

    var persistedType = false;
    var firstTypeID = 0;
    var types = parent.Ts.Cache.getTicketTypes();
    if (parent.Ts.System.Organization.UseProductFamilies && _productFamilyID != null) {
        for (var i = 0; i < types.length; i++) {
            if (types[i].ProductFamilyID == null || _productFamilyID == types[i].ProductFamilyID) {
                selectizeType.addOption({ value: types[i].TicketTypeID, text: types[i].Name });
                //_lastTicketTypeID = types[i].TicketTypeID;
                if (firstTypeID == 0) {
                    firstTypeID = types[i].TicketTypeID;
                }
                if (_lastTicketTypeID == types[i].TicketTypeID) {
                    selectizeType.addItem(types[i].TicketTypeID, true);
                    persistedType = true;
                }
            }
        }
    } else {
        persistedType = true;
        for (var i = 0; i < types.length; i++) {
            selectizeType.addOption({ value: types[i].TicketTypeID, text: types[i].Name });
            if (firstTypeID == 0) {
                firstTypeID = types[i].TicketTypeID;
            }
            if (_lastTicketTypeID == types[i].TicketTypeID) {
                selectizeType.addItem(types[i].TicketTypeID, true);
            }
        }
    }
    if (!persistedType) {
        selectizeType.addItem(firstTypeID);
    }
    callback({ Group: persistedGroup, Type: persistedType })
}

function SetupTicketProperties() {
    //Assigned To
    var users = parent.Ts.Cache.getUsers();

    if ($('#ticket-assigned').length) {
        $('#ticket-assigned').selectize({
            dataAttr: 'assigned',
            onDropdownClose: function ($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true,
            render: {
                option: function (item, escape) {
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
            onChange: function (value) {
                parent.Ts.Services.Users.GetUserGroups(value, function (groups) {
                    if (groups !== null && groups.length == 1) {
                        SetGroup(groups[0].GroupID);
                    }
                });
            },
        });

        $('#NewCustomerModal').on('shown.bs.modal', function () {
            if ((top.Ts.System.User.CanCreateContact) || top.Ts.System.User.IsSystemAdmin) {
                return;
            } else {
                $('#customer-email-input').prop("disabled", true);
                $('#customer-fname-input').prop("disabled", true);
                $('#customer-lname-input').prop("disabled", true);
                $('#customer-phone-input').prop("disabled", true);
            }
        });

        var selectize = $("#ticket-assigned")[0].selectize;
        selectize.addOption({ value: -1, text: 'Unassigned', data: '' });

        for (var i = 0; i < users.length; i++) {
            selectize.addOption({ value: users[i].UserID, text: users[i].Name, data: users[i] });
        }
        selectize.setValue(parent.Ts.System.User.UserID);
    }

    //Group
    var groups = parent.Ts.Cache.getGroups();
    AppendSelect('#ticket-group', null, 'group', -1, 'Unassigned', false);
    for (var i = 0; i < groups.length; i++) {
        AppendSelect('#ticket-group', groups[i], 'group', groups[i].GroupID, groups[i].Name);
    }
    if ($('#ticket-group').length) {
        $('#ticket-group').selectize({
            onDropdownClose: function ($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });
    }

    if (window.parent.Ts.System.Organization.RequireGroupAssignmentOnTickets) {
        if ($('#ticket-group').val() == "" || $('#ticket-group').val() == "-1") {
            $('#ticket-group').closest('.form-horizontal').addClass('hasError');
        } else {
            $('#ticket-group').closest('.form-horizontal').removeClass('hasError');
        }
    }

    $('#ticket-group').change(function (e) {
        var self = $(this);
        _ticketGroupID = self.val();
        if (_ticketGroupID == '-1') {
            _ticketGroupID = null;
            if (window.parent.Ts.System.Organization.RequireGroupAssignmentOnTickets) {
                $('#ticket-group').closest('.form-horizontal').addClass('hasError');
            }
        } else {
            $('#ticket-group').closest('.form-horizontal').removeClass('hasError');
        }
    });

    //Type
    var types = parent.Ts.Cache.getTicketTypes();
    for (var i = 0; i < types.length; i++) {
        if (types[i].IsActive) {
            AppendSelect('#ticket-type', types[i], 'type', types[i].TicketTypeID, types[i].Name);
        }
    }

    _lastTicketTypeID = types[0].TicketTypeID;
    if ($('#ticket-type').length) {
        $('#ticket-type').selectize({
            onDropdownClose: function ($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });
        //AppendTicketTypeTemplate(_lastTicketTypeID);
    }

    $('#ticket-type').change(function (e) {
        CacheCurrentCustomValues();  // cache the current custom values
        SetupStatusField();
        showCustomFields();
        _lastTicketTypeID = $(this).val();
        AppendTicketTypeTemplate(_lastTicketTypeID);
        createCustomFields();   // ticket type changed - reload the custom fields
        AppendProductMatchingCustomFields();    // also reload the product custom fields
    });

    //Status
    SetupStatusField();

    //Severity
    var severities = parent.Ts.Cache.getTicketSeverities();
    for (var i = 0; i < severities.length; i++) {
        AppendSelect('#ticket-severity', severities[i], 'severity', severities[i].TicketSeverityID, severities[i].Name);
    }
    if ($('#ticket-severity').length) {
        $('#ticket-severity').selectize({
            onDropdownClose: function ($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });
    }

    if (parent.Ts.System.Organization.SetNewActionsVisibleToCustomers == true) {
        $('#ticket-visible').prop('checked', true);
    }

    if (!parent.Ts.System.User.IsSystemAdmin && !parent.Ts.System.User.ChangeTicketVisibility) {
        $('#ticket-visible').prop('disabled', true);
    }

    $('#ticket-properties-area').on('click', 'span.tagRemove', function (e) {
        var tag = $(this).parent();
        tag.remove();
        if ($(tag).hasClass("OrgAnchor")) ReloadProductList();
    });

    //KB
    SetupKBFields();

    //Community
    if (parent.Ts.System.Organization.UseForums == false) {
        $('#ticket-Community').closest('.form-horizontal').remove();
    }
    else SetupCommunityField();

    //DueDate
    SetupDueDateField();

    //Customer Section
    if (parent.Ts.System.Organization.ProductType == parent.Ts.ProductType.Express) {
        $('#ticket-Customer').closest('.form-group').remove();
    } else {
        SetupCustomerSection();
    }

    //Product Section
    if (parent.Ts.System.Organization.ProductType == parent.Ts.ProductType.Express || parent.Ts.System.Organization.ProductType === parent.Ts.ProductType.HelpDesk) {
        $('#ticket-Product').closest('.form-horizontal').remove();
        $('#ticket-Resolved').closest('.form-horizontal').remove();
        $('#ticket-Versions').closest('.form-horizontal').remove();
    } else {
        SetupProductSection();
    }

    //Inventory Section
    SetupInventorySection();
    //Tags
    SetupTagsSection();
    //Queues Section
    SetupUserQueuesSection();
    //Subscribers Section
    SetupSubscribedUsersSection();
    //Reminders
    SetupRemindersSection();
    //Associate Tickets
    SetupAssociatedTicketsSection();
    createCustomFields();
    setInitialValue();

};

var _customValuesDictionary = new Object();

function UpdateValueFromCache(field) {
    var value = _customValuesDictionary[field.Name];
    if ((field.Value == null) && (value != null))
        field.Value = value;
}

function CacheCurrentCustomValues() { GetCustomValues(); }

// copied from SaveTicket()
function GetCustomValues() {
    // Custom Values
    var customValues = new Array();
    $('.custom-field:visible').each(function () {
        var data = $(this).data('field');
        var field = new Object();
        field.CustomFieldID = data.CustomFieldID;
        switch (data.FieldType) {
            case parent.Ts.CustomFieldType.Boolean:
                field.Value = $(this).find('input').prop('checked');
                break;
            case parent.Ts.CustomFieldType.PickList:
                field.Value = $(this).find('select').val();
                break;
            case parent.Ts.CustomFieldType.Time:
                var text = $(this).find('a').text();
                var value = parent.Ts.Utils.getMsDate("1/1/1900 " + text);
                field.Value = text == null ? null : value.toUTCString();
                break;
            case parent.Ts.CustomFieldType.DateTime:
                var text = $(this).find('a').text();
                var value = parent.Ts.Utils.getMsDate(text);
                field.Value = text == 'unassigned' ? null : value.toUTCString();
                break;
            default:
                field.Value = $(this).find('input').val();
        }
        customValues.push(field);
        _customValuesDictionary[data.Name] = field.Value;   // preserve current values across ticket types
    });
    return customValues;
}

function SaveTicket() {
    if ($("#recorder").length == 0) {
        isFormValid(function (isValid) {
            if (isValid == true) {
                var info = new Object();
                info.Name = $('#ticket-title-input').val();
                info.TicketTypeID = ($('#ticket-type').length) ? $('#ticket-type').val() : '-1';//$('#ticket-type').val();
                info.TicketStatusID = ($('#ticket-status').length) ? $('#ticket-status').val() : '-1';//$('#ticket-status').val();

                if ($('#ticket-status').length) {
                    info.TicketStatusID = $('#ticket-status').val();
                } else {
                    var statuses = parent.Ts.Cache.getTicketStatuses();
                    info.TicketStatusID = statuses[0].TicketStatusID;
                }
                info.TicketSeverityID = ($('#ticket-severity').length) ? $('#ticket-severity').val() : '-1';//$('#ticket-severity').val();
                info.UserID = ($('#ticket-assigned').length && $('#ticket-assigned').val() !== '') ? $('#ticket-assigned').val() : '-1';//($('#ticket-assigned').val() == '') ? '-1' : $('#ticket-assigned').val();
                info.GroupID = ($('#ticket-group').length && $('#ticket-group').val() !== '') ? $('#ticket-group').val() : '-1';//($('#ticket-group').val() == '') ? '-1' : $('#ticket-group').val();
                var dueDate = $('.ticket-action-form-dueDate').datetimepicker('getDate');
                info.DueDate = _dueDate;

                info.CategoryID = ($('#ticket-Community').length) ? $('#ticket-Community').val() : null;//$('#ticket-Category').val();
                info.ProductID = ($('#ticket-Product').length && $('#ticket-Product').val() !== '') ? $('#ticket-Product').val() : '-1';//($('#ticket-Product').val() == '') ? '-1' : $('#ticket-Product').val();
                info.ReportedID = ($('#ticket-Versions').length && $('#ticket-Versions').val() !== '') ? $('#ticket-Versions').val() : '-1';//($('#ticket-Versions').val() == '') ? '-1' : $('#ticket-Versions').val();
                info.ResolvedID = ($('#ticket-Resolved').length && $('#ticket-Resolved').val() !== '') ? $('#ticket-Resolved').val() : '-1';//($('#ticket-Resolved').val() == '') ? '-1' : $('#ticket-Resolved').val();
                info.IsVisibleOnPortal = ($('#ticket-visible').length) ? $('#ticket-visible').prop('checked') : false;//$('#ticket-visible').prop('checked')
                info.IsKnowledgebase = ($('#ticket-isKB').length) ? $('#ticket-isKB').prop('checked') : false;//$('#ticket-isKB').prop('checked');
                info.KnowledgeBaseCategoryID = ($('#ticket-KB-Category').length) ? $('#ticket-KB-Category').val() : '-1'; //($('#ticket-KB-Category').val() == '') ? '-1' : $('#ticket-KB-Category').val();
                info.Description = tinyMCE.activeEditor.getContent();
                info.DateStarted = parent.Ts.Utils.getMsDate(moment($('#action-new-date-started').val(), dateFormat + ' hh:mm A').format('MM/DD/YYYY hh:mm A'));

                var timeSpent = parseInt($('#action-new-hours').val()) * 60 + parseInt($('#action-new-minutes').val());
                info.TimeSpent = timeSpent;

                // Custom Values
                info.Fields = GetCustomValues();

                // Associated Tickets
                info.ChildTickets = new Array();
                info.RelatedTickets = new Array();
                $('#ticket-AssociatedTickets > div.tag-item').each(function () {
                    var data = $(this).data('tag');
                    var IsParent = $(this).data('IsParent');
                    if (IsParent === null) {
                        info.RelatedTickets[info.RelatedTickets.length] = data.TicketID
                    } else if (IsParent) {
                        info.ParentTicketID = data.TicketID;
                    } else {
                        info.ChildTickets[info.ChildTickets.length] = data.TicketID
                    }
                });

                //Tags
                info.Tags = new Array();
                $('#ticket-tags > div.tag-item').each(function () {
                    var data = $(this).data('tag');
                    info.Tags[info.Tags.length] = data.value;
                });

                //Subscribers
                info.Subscribers = new Array();
                $('#ticket-SubscribedUsers > div.tag-item').each(function () {
                    info.Subscribers[info.Subscribers.length] = $(this).attr('id');
                });

                //Reminders
                info.Reminders = new Array();
                $('#ticket-Reminders > div.tag-item').each(function () {
                    info.Reminders[info.Reminders.length] = $(this).attr('id');
                });

                //Queues
                info.Queuers = new Array();
                $('#ticket-UserQueue > div.tag-item').each(function () {
                    info.Queuers[info.Queuers.length] = $(this).attr('id');
                });

                //Inventory
                info.Assets = new Array();
                $('#ticket-Inventory > div.tag-item').each(function () {
                    info.Assets[info.Assets.length] = $(this).attr('id');
                });

                //Customers
                info.Customers = new Array();
                info.Contacts = new Array();

                $('#ticket-Customer > div.tag-item').each(function () {
                    var data = $(this).data('tag');
                    if (data.UserID) {
                        info.Contacts[info.Contacts.length] = data.UserID;
                    } else {
                        info.Customers[info.Customers.length] = data.OrganizationID;
                    }
                });

                //var chatID = parent.Ts.Utils.getQueryValue('chatid', window)
                //if (chatID && chatID != null) {
                //  parent.Ts.Services.Tickets.GetChatCustomer(chatID, function (result) {
                //    AddCustomers(result);
                //  });
                //}

                var chatID = parent.Ts.Utils.getQueryValue('chatid', window)
                if (chatID && chatID != null) {
                    info.ChatID = chatID;
                }

                parent.Ts.Services.Tickets.NewTicket(parent.JSON.stringify(info), function (result) {
                    if (result == null) {
                        alert('There was an error saving your ticket.  Please try again.');
                        $('.new-ticket-save-buttons').removeClass('saving');
                        return;
                    }
                    _ticketID = result[0];

                    parent.Ts.System.logAction('Ticket Created');
                    if ($('.upload-queue li').length > 0) {
                        $('.upload-queue li').each(function (i, o) {
                            var data = $(o).data('data');
                            data.url = '../../../Upload/Actions/' + result[1];
                            data.jqXHR = data.submit();
                            $(o).data('data', data);
                        });
                    } else {
                        parent.Ts.Services.TicketPage.CheckContactEmails(_ticketID, function (isInvalid) {
                            if (!isInvalid && window.parent.Ts.System.Organization.AlertContactNoEmail)
                                alert("At least one of the contacts associated with this ticket does not have an email address defined or is inactive, and will not receive any emails about this ticket.");

                            if (_doClose != true) parent.Ts.MainPage.openTicketByID(result[0]);
                            parent.Ts.MainPage.closeNewTicketTab();
                        });
                    }
                });
            } else {
                $('#ticket-create').prop('disabled', false);
                $('#ticket-createandclose').prop('disabled', false);
            }
        });
    }
};

function isFormValid(callback) {
    $('#newticket-create-errors').empty();
    parent.Ts.Services.Organizations.IsProductRequired(function (isProductRequired) {
        parent.Ts.Services.Organizations.IsProductVersionRequired(function (isProductVersionRequired) {
            parent.Ts.Settings.Organization.read('RequireNewTicketCustomer', false, function (requireNewTicketCustomer) {
                var result = true;
                var product = $('#ticket-Product');
                var reportversion = $('#ticket-Versions');
                var productID = product.val();
                var reportversionID = reportversion.val();
                var status = parent.Ts.Cache.getTicketStatus($('#ticket-status').val());

                //Check if we need a product
                product.closest('.form-horizontal').removeClass('hasError');
                if (isProductRequired && (productID == -1 || productID == "")) {
                    product.closest('.form-horizontal').addClass('hasError');
                    InsertCreateError("Product is a required field.");
                    result = false;
                }

                //See if we need a version
                reportversion.closest('.form-horizontal').removeClass('hasError');
                if (isProductVersionRequired && (reportversionID == -1 || reportversionID == "")) {
                    reportversion.closest('.form-horizontal').addClass('hasError');
                    InsertCreateError("Product version is a required field.");
                    result = false;
                }

                //Ensure there is a name
                $('#ticket-title-input').closest('.form-group').removeClass('has-error hasError');
                if ($.trim($('#ticket-title-input').val()) == '') {
                    $('#ticket-title-input').closest('.form-group').addClass('has-error hasError');
                    InsertCreateError("Please enter a title above.");
                    result = false;
                }
                //Check required custom fields
                var cfHasError = false;
                $('.custom-field:visible').each(function () {
                    $(this).removeClass('hasError');
                    var field = $(this).data('field');
                    if (field.IsRequired) {
                        switch (field.FieldType) {
                            case parent.Ts.CustomFieldType.Text:
                            case parent.Ts.CustomFieldType.Number:
                                if ($.trim($(this).find('input').val()) == '') {
                                    $(this).addClass('hasError');
                                    cfHasError = true;
                                    result = false;
                                }
                                break;
                            case parent.Ts.CustomFieldType.Date:
                            case parent.Ts.CustomFieldType.Time:
                            case parent.Ts.CustomFieldType.DateTime:
                                var date = $.trim($(this).find('a').text());
                                if (date == null || date == '' || date == 'Unassigned') {
                                    $(this).addClass('hasError');
                                    cfHasError = true;
                                    result = false;
                                }
                                break;
                            case parent.Ts.CustomFieldType.PickList:
                                if ($(this).hasClass('isEmpty')) {
                                    $(this).addClass('hasError');
                                    cfHasError = true;
                                    result = false;
                                }
                                break;
                            default:
                        }
                    }

                    if (status.IsClosed) {
                        if (field.IsRequiredToClose) {
                            switch (field.FieldType) {
                                case parent.Ts.CustomFieldType.Text:
                                case parent.Ts.CustomFieldType.Number:
                                    if ($.trim($(this).find('input').val()) == '') {
                                        $(this).addClass('hasError');
                                        cfHasError = true;
                                        result = false;
                                    }
                                    break;
                                case parent.Ts.CustomFieldType.Date:
                                case parent.Ts.CustomFieldType.Time:
                                case parent.Ts.CustomFieldType.DateTime:
                                    var date = $.trim($(this).find('a').text());
                                    if (date == null || date == '' || date == 'Unassigned') {
                                        $(this).addClass('hasError');
                                        cfHasError = true;
                                        result = false;
                                    }
                                    break;
                                case parent.Ts.CustomFieldType.PickList:
                                    if ($(this).hasClass('isEmpty')) {
                                        $(this).addClass('hasError');
                                        cfHasError = true;
                                        result = false;
                                    }
                                    break;
                                default:
                            }
                        }
                    }
                });

                if ($('#ticket-Customer > .tag-error').length > 0) {
                    InsertCreateError("An inactive contact or customer with expired Service Level Agreement can not be added to the ticket");
                    result = false;
                }

                if (cfHasError) { InsertCreateError("Please fill in the red required custom fields."); }

                //Check if we have any errors
                if (window.parent.Ts.System.Organization.RequireGroupAssignmentOnTickets) {
                    if ($('#ticket-group').val() == "-1" || $('#ticket-group').val() == "") {
                        InsertCreateError("A group is required to create a ticket.");
                        result = false;
                    }
                }
                //If custom required check if the ticket is a KB if not then see if we have at least one customer
                if (requireNewTicketCustomer == "True" && $('#ticket-isKB').is(":checked") == false) {
                    if ($('#ticket-Customer > div.tag-item').length < 1) {
                        $('#ticket-Customer').closest('.form-horizontal').addClass('hasError');
                        InsertCreateError("A customer is required to create a ticket.")
                        result = false;
                    } else {
                        $('#ticket-Customer').closest('.form-horizontal').removeClass('hasError');
                    }
                }
                callback(result);
            });
        });
    });
};

function InsertCreateError(message) {
    var alert = $('<div>').addClass('alert alert-danger').text(message).appendTo($('#newticket-create-errors'));
}

function SetupDescriptionEditor() {
    initEditor($('#ticket-description'), true, function (ed) {
        AppendTicketTypeTemplate(_lastTicketTypeID);
        SetupUploadQueue();
        $('#ticket-isKB').click(function (e) {
            if ($('#ticket-isKB').prop('checked')) {
                if (!canEdit) {
                    $('#ticket-visible').prop('checked', false);
                    $('#ticket-visible').prop('disabled', true);
                }
            } else {
                $('#ticket-visible').prop('disabled', false);
            }
        });

        $('#ticket-create').click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            $(this).prop('disabled', true);
            _doClose = false;
            SaveTicket();
        });

        $('#ticket-createandclose').click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            $(this).prop('disabled', true);
            _doClose = true;
            SaveTicket();
        });

        $('#ticket-cancel').click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            if (confirm('Are you sure you would like to cancel this ticket?')) {
                clearTimeout(_timerid);
                _timerElapsed = 0;
                counter = 0;
                parent.Ts.MainPage.closeNewTicketTab();
            }
            parent.Ts.System.logAction('New Ticket - Canceled');
            $('#recorder').remove();
        });

        $('#rcdtokScreen').click(function (e) {
            parent.Ts.Services.Tickets.StartArchivingScreen(sessionId, function (resultID) {
                $('#rcdtokScreen').hide();
                $('#stoptokScreen').show();
                $('#deletetokScreen').hide();
                $('#muteTokScreen').show();
                recordingID = resultID;
                $('#tokScreenCountdown').show();
                setTimeout(function () {
                    update();
                }, 1000);
                //countdown("tokScreenCountdown", 5, 0, element);
                //recordScreenTimer = setTimeout(function () { StopRecording(element); }, 300000);
                $('#statusTextScreen').text("Currently Recording Screen...");
            });
        });

        $('#muteTokScreen').hide();
        $('#muteTokScreen').click(function (e) {
            publisher.publishAudio(false);
            $('#unmuteTokScreen').show();
            $('#muteTokScreen').hide();
        });

        $('#unmuteTokScreen').hide();
        $('#unmuteTokScreen').click(function (e) {
            publisher.publishAudio(true);
            $('#muteTokScreen').show();
            $('#unmuteTokScreen').hide();
        });

        $('#stoptokScreen').hide();
        $('#stoptokScreen').click(function (e) {
            $('#statusTextScreen').text("Saving video...");
            clearTimeout(tokTimer);
            $("#tokScreenCountdown").html("0:00");
            parent.Ts.Services.Tickets.StopArchiving(recordingID, function (result) {
                $('#tokScreenCountdown').hide();
                $('#rcdtokScreen').show();
                $('#stoptokScreen').hide();
                $('#canceltokScreen').show();
                $('#unmuteTokScreen').hide();
                $('#muteTokScreen').hide();
                tokurl = result;
                videoURL = '<video width="100%" controls poster="' + parent.Ts.System.AppDomain + '/dc/1078/images/static/player.jpg"><source src="' + tokurl + '" type="video/mp4"><a href="' + tokurl + '">Please click here to view the video.</a></video>';
                tinyMCE.activeEditor.execCommand('mceInsertContent', false, '<br/><br/>' + videoURL);
                $('#statusTextScreen').text("Your video is currently processing. It may not play in the editor below but should be live within a minute.");
                session.unpublish(screenSharingPublisher);
                session.unpublish(publisher);
            });
        });

        $('#canceltokScreen').click(function (e) {
            $('#statusTextScreen').text("");
            clearTimeout(tokTimer);
            $("#tokScreenCountdown").html("0:00");
            if (recordingID != null) {
                parent.Ts.Services.Tickets.StopArchiving(recordingID, function (result) {
                    $('#tokScreenCountdown').hide();
                    $('#rcdtokScreen').show();
                    $('#stoptokScreen').hide();
                    $('#canceltokScreen').show();
                    $('#unmuteTokScreen').hide();
                    $('#muteTokScreen').hide();
                    recordingID = null;
                });
            }
            session.unpublish(screenSharingPublisher);
            session.unpublish(publisher);
            $('#recordScreenContainer').hide();
        });

        $('#rcdtok').click(function (e) {
            parent.Ts.System.logAction('Ticket - Video Recording Start Clicked');
            parent.Ts.Services.Tickets.StartArchiving(sessionId, function (resultID) {
                $('#rcdtok').hide();
                $('#stoptok').show();
                $('#inserttok').hide();
                $('#deletetok').hide();
                recordingID = resultID;
                $('#statusText').text("Currently Recording ...");
            });
        });

        $('#stoptok').hide();

        $('#stoptok').click(function (e) {
            parent.Ts.System.logAction('Ticket - Video Recording Stop Clicked');
            $('#statusText').text("Processing...");
            parent.Ts.Services.Tickets.StopArchiving(recordingID, function (resultID) {
                $('#rcdtok').show();
                $('#stoptok').hide();
                $('#inserttok').show();
                $('#canceltok').show();
                tokurl = resultID;
                $('#statusText').text("Recording Stopped");
            });
        });

        $('#inserttok').hide();

        $('#inserttok').click(function (e) {
            parent.Ts.System.logAction('Ticket - Video Recording Insert Clicked');
            tinyMCE.activeEditor.execCommand('mceInsertContent', false, '<br/><br/><video width="400" height="400" controls poster="' + parent.Ts.System.AppDomain + '/dc/1078/images/static/videoview1.jpg"><source src="' + tokurl + '" type="video/mp4"><a href="' + tokurl + '">Please click here to view the video.</a></video>');
            session.unpublish(publisher);
            $('#rcdtok').show();
            $('#stoptok').hide();
            $('#inserttok').hide();
            $('#recordVideoContainer').hide();
            $('#statusText').text("");
        });

        $('#deletetok').hide();

        $('#canceltok').click(function (e) {
            parent.Ts.System.logAction('Ticket - Video Recording Cancel Clicked');
            if (recordingID) {
                $('#statusText').text("Cancelling Recording ...");
                parent.Ts.Services.Tickets.DeleteArchive(recordingID, function (resultID) {
                    $('#rcdtok').show();
                    $('#stoptok').hide();
                    $('#inserttok').hide();
                    session.unpublish(publisher);
                    $('#recordVideoContainer').hide();
                    $('#statusText').text("");
                });
            } else {
                session.unpublish(publisher);
                $('#recordVideoContainer').hide();
            }
            $('#statusText').text("");
        });
        $('#recordVideoContainer').hide();
    },
    function (ed) {
        $('#ticket-title-input').focus();
    });
};

function StopRecording() {
    $('#statusTextScreen').text("Processing...");
    clearTimeout(tokTimer);
    $("#tokScreenCountdown").html("0:00");
    parent.Ts.Services.Tickets.StopArchiving(recordingID, function (result) {
        $('#statusText').text("");
        $('#tokScreenCountdown').hide();
        $('#rcdtokScreen').show();
        $('#stoptokScreen').hide();
        $('#canceltokScreen').show();
        $('#unmuteTokScreen').hide();
        $('#muteTokScreen').hide();
        tokurl = result;
        videoURL = '<video controls poster="' + parent.Ts.System.AppDomain + '/dc/1078/images/static/screenview.jpg"><source src="' + tokurl + '" type="video/mp4"><a href="' + tokurl + '">Please click here to view the video.</a></video>';

        tinyMCE.activeEditor.execCommand('mceInsertContent', false, '<br/><br/>' + videoURL);
        $('#statusTextScreen').text("");
        session.unpublish(screenSharingPublisher);
        session.unpublish(publisher);
    });
}

function update() {
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
        StopRecording();
        return;
    }

    $("#tokScreenCountdown").html(ts[1] + ":" + ts[2]);
    tokTimer = setTimeout(function () {
        update();
    }, 1000);
}

function SetupUploadQueue() {
    var element = $('#file-uploads');
    $('#file-upload').fileupload({
        namespace: 'new_action',
        dropZone: element,
        add: function (e, data) {
            for (var i = 0; i < data.files.length; i++) {
                var item = $('<li>').appendTo(element.find('.upload-queue'));
                data.context = item;
                item.data('data', data);

                var bg = $('<div>').appendTo(item);

                $('<div>').text(data.files[i].name + '  (' + parent.Ts.Utils.getSizeString(data.files[i].size) + ')').addClass('filename').appendTo(bg);
                $('<div>').addClass('progress').hide().appendTo(bg);
                $('<span>').addClass('ui-icon ui-icon-close').click(function (e) {
                    e.preventDefault();
                    $(this).closest('li').fadeOut(500, function () { $(this).remove(); });
                }).appendTo(bg);

                //<span class="tagRemove" aria-hidden="true">×</span>

                $('<span>').addClass('ui-icon ui-icon-cancel').hide().click(function (e) {
                    e.preventDefault();
                    var data = $(this).closest('li').data('data');
                    data.jqXHR.abort();
                }).appendTo(bg);
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
            $('.progress').progressbar('value', 100);
            if (_doClose != true) { parent.Ts.MainPage.openTicketByID(_ticketID); }
            parent.Ts.MainPage.closeNewTicketTab();
        }
    });
}

function AppendSelect(parent, data, type, id, name, isSelected) {
    var option = $('<option>').val(id).text(name).appendTo(parent).data(type, data);
    if (isSelected) {
        option.attr('selected', 'selected');
    }
};

function SetupStatusField() {
    if ($('#ticket-status').length) {
        $("#ticket-status").selectize({
            closeAfterSelect: true,
            render: {
                item: function (item, escape) {
                    if (item.data.IsClosed) {
                        return '<div data-value="' + escape(item.value) + '" data-item="' + escape(item.data) + '" data-selectable="" class="option"><s>' + escape(item.text) + '</s></div>';
                    } else {
                        return '<div data-value="' + escape(item.value) + '" data-item="' + escape(item.data) + '" data-selectable="" class="option">' + escape(item.text) + '</div>';
                    }
                }
            },
        });
        var selectize = $("#ticket-status")[0].selectize;
        selectize.clear(true);
        selectize.clearOptions();

        var statuses = parent.Ts.Cache.getTicketStatuses();
        var ticketTypeID = $('#ticket-type').val();

        var flag = true;
        for (var i = 0; i < statuses.length; i++) {
            if (statuses[i].TicketTypeID == ticketTypeID) {
                selectize.addOption({ value: statuses[i].TicketStatusID, text: statuses[i].Name, data: statuses[i] });
                if (flag) {
                    selectize.addItem(statuses[i].TicketStatusID);
                    flag = false;
                }
            }
        }
    }
}

function SetupKBFields() {
    if (canEdit) {
        var categories = parent.Ts.Cache.getKnowledgeBaseCategories();
        for (var i = 0; i < categories.length; i++) {
            var cat = categories[i].Category;
            AppendSelect('#ticket-KB-Category', cat, 'category', cat.CategoryID, cat.CategoryName);
            for (var j = 0; j < categories[i].Subcategories.length; j++) {
                var subcat = categories[i].Subcategories[j];
                AppendSelect('#ticket-KB-Category', subcat, 'subcategory', subcat.CategoryID, cat.CategoryName + ' -> ' + subcat.CategoryName);
            }
        }
        if ($('#ticket-KB-Category').length) {
            $('#ticket-KB-Category').selectize({
                onDropdownClose: function ($dropdown) {
                    $($dropdown).prev().find('input').blur();
                },
                closeAfterSelect: true
            });
        }
    } else {
        //$('#ticket-isKB').closest('.form-horizontal').remove();
        $('#ticket-group-KBCat').parent().remove();
    }
}

function SetupCommunityField() {
    if (parent.Ts.System.Organization.UseForums == true) {
        if (parent.Ts.System.User.CanChangeCommunityVisibility) {
            var forumCategories = parent.Ts.Cache.getForumCategories();
            for (var i = 0; i < forumCategories.length; i++) {
                var cat = forumCategories[i].Category;
                AppendSelect('#ticket-Community', cat, 'community', cat.CategoryID, cat.CategoryName, false);

                for (var j = 0; j < forumCategories[i].Subcategories.length; j++) {
                    var subcat = forumCategories[i].Subcategories[j];
                    AppendSelect('#ticket-Community', subcat, 'subcategory', subcat.CategoryID, cat.CategoryName + ' -> ' + subcat.CategoryName, false);
                }
            }
            if ($('#ticket-Community').length) {
                $('#ticket-Community').selectize({
                    onDropdownClose: function ($dropdown) {
                        $($dropdown).prev().find('input').blur();
                    },
                    allowEmptyOption: true,
                    closeAfterSelect: true
                });
            }
        } else {
            $('#ticket-Community').closest('.form-horizontal').remove();
        }
    } else {
        $('#ticket-Community').closest('.form-horizontal').remove();
    }
};

var SetupDueDateField = function (duedate) {
    var dateContainer = $('#ticket-duedate-container');
    var dateLink = $('<a>').attr('href', '#').addClass('control-label ticket-anchor ticket-nullable-link ticket-duedate-anchor').appendTo(dateContainer);
    if (duedate !== undefined) {
        dateLink.text(duedate.localeFormat(parent.Ts.Utils.getDateTimePattern()));
    }

    dateLink.click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        var header = $(this).hide();
        var container = $('<div>').addClass('row').insertAfter(header);
        var container1 = $('<div>').attr('id','duedate-input').addClass('col-xs-10').appendTo(container);
        var theinput = $('<input type="text">').val(duedate === undefined ? '' : duedate.localeFormat(parent.Ts.Utils.getDateTimePattern())).datetimepicker({ pickTime: true }).appendTo(container1).focus();

        $('<i>').addClass('fa fa-times').click(function (e) {
            $(this).closest('div').remove();
            header.show();
        }).insertAfter(container1);


        $('<i>').addClass('fa fa-check').click(function (e) {
            var currDate = $(this).prev().find('input').val();
            var value = '';
            if (currDate !== '') {
                value = parent.Ts.Utils.getMsDate(currDate);
            }
            _dueDate = value;
            duedate = value;
            theinput.blur().remove();
            dateLink.text((value === '' ? '' : value.localeFormat(parent.Ts.Utils.getDateTimePattern()))).show();

            $(this).closest('div').remove();
            header.show();
        })
        .insertAfter(container1);
    });
}

function AddTags(tag) {
    var tagDiv = $("#ticket-tags");
    $("#ticket-tag-Input").val('');
    PrependTag(tagDiv, tag.id, tag.value, tag);
}

function SetupTagsSection() {
    if ($("#ticket-tag-Input").length) {
        $("#ticket-tag-Input").autocomplete({
            minLength: 2,
            source: getTags,
            response: function (event, ui) {
                var inputValue = $(this).val();
                var filtered = $(ui.content).filter(function () {
                    return this.value == inputValue;
                });
                if (filtered.length === 0) {
                    ui.content.push({ label: inputValue, value: inputValue, id: 0 });
                }
            },
            select: function (event, ui) {
                $(this).data('item', ui.item)
                AddTags(ui.item);
                this.removeItem(ui.item.value, true);
                parent.Ts.System.logAction('Ticket - Added');
            }
        })
        .data("autocomplete")._renderItem = function (ul, item) {
            return $("<li>").append("<a>" + item.label + "</a>").appendTo(ul);
        };
    }
};

function PrependTag(parent, id, value, data, cssclass) {
    if (cssclass === undefined) cssclass = 'tag-item';
    var _compiledTagTemplate = Handlebars.templates['ticket-tag'];
    var tagHTML = _compiledTagTemplate({ id: id, value: value, data: data, css: cssclass });
    return $(tagHTML).prependTo(parent).data('tag', data);
}

function SetupCustomerSection() {
    if ($('#ticket-Customers-Input').length) {
        $('#ticket-Customers-Input').selectize({
            valueField: 'id',
            labelField: 'label',
            searchField: 'label',
            delimiter: null,
            load: function (query, callback) {
                this.clearOptions();        // clear the data
                this.renderCache = {};      // clear the html template cache
                getCustomers(query, callback)
            },
            score: function (search) {
                return function (option) {
                    return 1;
                }
            },
            create: function (input, callback) {
                $('#NewCustomerModal').modal('show');
                callback(null);
            },
            plugins: {
                'sticky_placeholder': {},
                'no_results': {}
            },
            onItemAdd: function (value, $item) {
                $('#ticket-Customer').closest('.form-horizontal').removeClass('hasError');
                AddCustomers($item.data());

                this.removeItem(value, true);
            },
            render: {
                item: function (item, escape) {
                    return '<div data-value="' + item.value + '" data-type="' + item.data + '" data-selectable="" class="option">' + item.label + '</div>';
                },
                option: function (item, escape) {
                    return '<div data-value="' + escape(item.value) + '" data-type="' + escape(item.data) + '" data-selectable="" class="option">' + item.label + '</div>';
                },
                option_create: function (data, escape) {
                    return '<div class="create">Create <strong>' + escape(data.input) + '</strong></div>';
                }
            },
            onDropdownClose: function ($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });

        var canCreateCustomers = false;
        if ((top.Ts.System.User.CanCreateContact && top.Ts.System.User.CanCreateCompany) || top.Ts.System.User.IsSystemAdmin) {
            canCreateCustomers = true;
        }

        $('#customer-company-input').selectize({
            valueField: 'label',
            labelField: 'label',
            searchField: 'label',
            load: function (query, callback) {
                this.clearOptions();        // clear the data
                this.renderCache = {};      // clear the html template cache
                getCompany(query, callback)
            },
            score: function (search) {
                return function (option) {
                    return 1;
                }
            },
            onDropdownClose: function ($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            create: canCreateCustomers,
            closeAfterSelect: true,
            plugins: {
                'sticky_placeholder': {},
                'no_results': {}
            }
        });
    }
    $('#Customer-Create').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        parent.Ts.System.logAction('Ticket - New Customer Added');
        var email = $('#customer-email-input').val();
        var firstName = $('#customer-fname-input').val();
        var lastName = $('#customer-lname-input').val();
        var phone = $('#customer-phone-input').val();;
        var companyName = $('#customer-company-input').val();
        parent.Ts.Services.Users.CreateNewContact(email, firstName, lastName, companyName, phone, false, function (result) {
            if (result.indexOf("u") == 0 || result.indexOf("o") == 0) {
                var customerData = new Object();
                customerData.type = result.substring(0, 1);
                customerData.value = result.substring(1);
                AddCustomers(customerData)
            } else if (result.indexOf("The company you have specified is invalid") !== -1) {
                if (parent.Ts.System.User.CanCreateCompany || parent.Ts.System.User.IsSystemAdmin) {
                    if (confirm('Unknown company, would you like to create it?')) {
                        parent.Ts.Services.Users.CreateNewContact(email, firstName, lastName, companyName, phone, true, function (result) {
                            var customerData = new Object();
                            customerData.type = result.substring(0, 1);
                            customerData.value = result.substring(1);
                            AddCustomers(customerData);
                            $('.ticket-new-customer-email').val('');
                            $('.ticket-new-customer-first').val('');
                            $('.ticket-new-customer-last').val('');
                            $('.ticket-new-customer-company').val('');
                            $('.ticket-new-customer-phone').val('');
                            $('#ticket-Customer').closest('.form-horizontal').removeClass('hasError');
                            $('#NewCustomerModal').modal('hide');
                        });
                    }
                } else {
                    alert("We're sorry, but you do not have the rights to create a new company.");
                    $('.ticket-new-customer-email').val('');
                    $('.ticket-new-customer-first').val('');
                    $('.ticket-new-customer-last').val('');
                    $('.ticket-new-customer-company').val('');
                    $('.ticket-new-customer-phone').val('');
                    $('#ticket-Customer').closest('.form-horizontal').removeClass('hasError');
                    $('#NewCustomerModal').modal('hide');
                }
            } else {
                alert(result);
            }
        });
    });

    parent.Ts.Settings.Organization.read('RequireNewTicketCustomer', false, function (requireNewTicketCustomer) {
        if (requireNewTicketCustomer == "True" && $('#ticket-isKB').is(":checked") == false) {
            if ($('#ticket-Customer > div.tag-item').length < 1) {
                $('#ticket-Customer').closest('.form-horizontal').addClass('hasError');
            } else {
                $('#ticket-Customer').closest('.form-horizontal').removeClass('hasError');
            }
        }
    });
};

function AddCustomers(customer) {
    parent.Ts.Services.Tickets.GetTicketCustomer(customer.type, customer.value, function (result) {
        var customerdata = result;
        if (customer == null) return;
        parent.Ts.System.logAction('New Ticket - Customer Added');
        var customerDiv = $("#ticket-Customer");
        $("#ticket-Customers-Input").val('');

        var label = "";
        var cssClasses = "tag-item";

        if (customerdata.Flag) {
            cssClasses = cssClasses + " tag-error"
        }
        if (customerdata.Contact !== null && customerdata.Company !== null) {
            label = '<span class="UserAnchor" data-userid="' + customerdata.UserID + '" data-placement="left">' + customerdata.Contact + '</span><br/><span class="OrgAnchor" data-orgid="' + customerdata.OrganizationID + '" data-placement="left">' + customerdata.Company + '</span>';
            var newelement = PrependTag(customerDiv, customerdata.UserID, label, customerdata, cssClasses);
            LoadAlerts(window.parent.Ts.ReferenceTypes.Users, customerdata.UserID);
        } else if (customerdata.Contact !== null) {
            label = '<span class="UserAnchor" data-userid="' + customerdata.UserID + '" data-placement="left">' + customerdata.Contact + '</span>';
            var newelement = PrependTag(customerDiv, customerdata.UserID, label, customerdata, cssClasses);
            newelement.data('userid', customerdata.UserID).data('placement', 'left').data('ticketid', _ticketID);
            LoadAlerts(window.parent.Ts.ReferenceTypes.Users, customerdata.UserID);
        } else if (customerdata.Company !== null) {
            label = '<span class="OrgAnchor" data-orgid="' + customerdata.OrganizationID + '" data-placement="left">' + customerdata.Company + '</span>';
            var newelement = PrependTag(customerDiv, customerdata.OrganizationID, label, customerdata, cssClasses);
            newelement.data('orgid', customerdata.OrganizationID).data('placement', 'left').data('ticketid', _ticketID);
            LoadAlerts(window.parent.Ts.ReferenceTypes.Organizations, customerdata.OrganizationID);
        }
        ReloadProductList();
        SetDefaultSupportGroup(customerdata.OrganizationID);
    });
};

function LoadAlerts(refType, refID) {
    window.parent.Ts.Services.Customers.LoadAlerts(refID, refType, function (notes) {
        for (var i = 0; i < notes.length; i++) {
            var note = notes[i];
            LoadTicketNotes(note);
        }
    });
}

function LoadTicketNotes(note) {
    if (note) {
        var description = $('<div>').html(note.Description);

        var buttons = [
            {
                text: "Close",
                click: function () {
                    $(this).dialog("close");
                }
            },
            {
                text: "Snooze",
                click: function () {
                    _mainFrame.Ts.Services.Customers.SnoozeAlertByID($(this).data('noteId'), $(this).data('refType'));
                    $(this).dialog("close");
                }
            }
        ]

        if (!window.parent.Ts.System.Organization.HideDismissNonAdmins || window.parent.Ts.System.User.IsSystemAdmin) {
            buttons.push({
                text: "Dismiss",
                click: function () {
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
            create: function () {
                $(this).css('max-width', '800px');
            },
            buttons: buttons
        });

    }
}

function SetDefaultSupportGroup(customerID) {
    parent.Ts.Services.Organizations.GetOrganization(customerID, function (customer) {
        if (customer.DefaultSupportGroupID && customer.DefaultSupportGroupID != -1) SetGroup(customer.DefaultSupportGroupID);
        if (customer.DefaultSupportUserID) SetAssignedUser(customer.DefaultSupportUserID);
    });
}

function SetupProductSection() {
    var products = parent.Ts.Cache.getProducts();
    if ($('#ticket-Product').length) {
        $('#ticket-Product').selectize({
            onDropdownClose: function ($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            plugins: {
                'sticky_placeholder': {},
                'no_results': {}
            },
            allowEmptyOption: true,
            closeAfterSelect: true
        });
    }

    LoadProductList(products);

    parent.Ts.Services.Organizations.IsProductRequired(function (result) {
        if (result) {
            $('#ticket-Product').closest('.form-horizontal').addClass('hasError');
        } else {
            $('#ticket-Product').closest('.form-horizontal').removeClass('hasError');
        }
    });


    $('#ticket-Product').change(function (e) {
        var self      = $(this);
        var productID = $('#ticket-Product').val();
        var product   = parent.Ts.Cache.getProduct(productID);
        if (productID && productID !== prevProduct) {
            loadVersions(product);
        }
        AppendProductMatchingCustomFields();
        $('#ticket-Product').closest('.form-horizontal').removeClass('hasError');
        if (productID) { prevProduct = productID; }
        if (product === null) { return; }
        if (parent.Ts.System.Organization.UseProductFamilies && _productFamilyID != product.ProductFamilyID) {
            _productFamilyID = product.ProductFamilyID;
            UpdateTicketGroups(function (persistedGroup) {
                UpdateTicketTypes(persistedGroup, function (persistedData) {
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
    });
};

function LoadProductList(products) {
    if ($('#ticket-Product').length) {
        if (products == null) products = parent.Ts.Cache.getProducts();
        var $productselect = $('#ticket-Product').selectize();
        var $productselectInput = $productselect[0].selectize;

        var currProduct = $productselectInput.getValue();
        $productselectInput.clearOptions();

        for (var i = 0; i < products.length; i++) {
            $productselectInput.addOption({ value: products[i].ProductID, text: products[i].Name, data: products[i] });
        }

        SetupProductVersionsControl(null);
        SetProductVersionAndResolved(null, null);

        if (currProduct) {
            $productselectInput.setValue(currProduct);
        }
    }
}

function ReloadProductList() {
    parent.Ts.Settings.Organization.read('ShowOnlyCustomerProducts', false, function (showOnlyCustomers) {
        if (showOnlyCustomers == "True") {
            var organizationIDs = new Array();

            $('#ticket-Customer > div.tag-item').each(function () {
                var data = $(this).data('tag');
                organizationIDs[organizationIDs.length] = data.OrganizationID;
            });

            if (organizationIDs.length < 1) {
                var products = parent.Ts.Cache.getProducts();
                LoadProductList(products);
            } else {
                parent.Ts.Services.Tickets.GetCustomerProductIDs(parent.JSON.stringify(organizationIDs), function (productIDs) {
                    if (!productIDs || productIDs == null || productIDs.length < 1) {
                        var products = parent.Ts.Cache.getProducts();
                        LoadProductList(products);
                    } else {
                        var products = new Array();
                        for (var j = 0; j < productIDs.length; j++) {
                            var product = parent.Ts.Cache.getProduct(productIDs[j]);
                            products.push(product);
                        }
                        LoadProductList(products);
                    }
                });
            }
        }
        //else {
        //  var products = parent.Ts.Cache.getProducts();
        //  LoadProductList(products);
        //}
    });
};

function loadVersions(product) {
    if ($('#ticket-Versions').length) {
        var selectizeVersion = $("#ticket-Versions")[0].selectize;
        selectizeVersion.clear(true);
        selectizeVersion.clearOptions();
    }
    if ($('#ticket-Resolved').length) {
        var selectizeResolved = $("#ticket-Resolved")[0].selectize;
        selectizeResolved.clear(true);
        selectizeResolved.clearOptions();
    }
    if (product !== null) {
        var versions = product.Versions;
        for (var i = 0; i < versions.length; i++) {
            try {
                selectizeVersion.addOption({ value: versions[i].ProductVersionID, text: versions[i].VersionNumber, data: versions[i] });
                selectizeResolved.addOption({ value: versions[i].ProductVersionID, text: versions[i].VersionNumber, data: versions[i] });
            } catch (e) { }
        }
    }
}

function SetupProductVersionsControl(product) {
    if ($('#ticket-Versions').length) {
        var $select = $("#ticket-Versions").selectize({
            onItemAdd: function (value, $item) {
                var reportversion = $('#ticket-Versions');
                reportversion.closest('.form-horizontal').removeClass('hasError');
            },
            onDropdownClose: function ($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });
        var versionInput = $select[0].selectize;
    }

    if ($('#ticket-Resolved').length) {
        var $select = $("#ticket-Resolved").selectize({
            onDropdownClose: function ($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });
        var resolvedInput = $select[0].selectize;
    }

    if (product !== null && product.Versions.length > 0) {
        var versions = product.Versions;
        for (var i = 0; i < versions.length; i++) {
            try {
                AppendSelect('#ticket-Versions', versions[i], 'version', versions[i].ProductVersionID, versions[i].VersionNumber, false);
                AppendSelect('#ticket-Resolved', versions[i], 'resolved', versions[i].ProductVersionID, versions[i].VersionNumber, false);
            } catch (e) { }

        }
    }

    parent.Ts.Services.Organizations.IsProductVersionRequired(function (result) {
        var hasValue = $('#ticket-Versions');
        if (result && !hasValue) {
            $('#ticket-Versions').closest('.form-horizontal').addClass('hasError');
        } else {
            $('#ticket-Versions').closest('.form-horizontal').removeClass('hasError');
        }
    });
}

function SetProductVersionAndResolved(versionId, resolvedId) {
    if ($('#ticket-Versions').length) {
        var $select = $("#ticket-Versions").selectize({
            onDropdownClose: function ($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });
    }
    if ($('#ticket-Resolved').length) {
        var $select = $("#ticket-Resolved").selectize({
            onDropdownClose: function ($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });
    }
};

function SetupInventorySection() {
    if ($('#ticket-Inventory-Input').length) {
        $('#ticket-Inventory-Input').selectize({
            valueField: 'id',
            labelField: 'label',
            searchField: 'label',
            load: function (query, callback) {
                this.clearOptions();        // clear the data
                this.renderCache = {};      // clear the html template cache
                getAssets(query, callback)
            },
            score: function (search) {
                return function (option) {
                    return 1;
                }
            },
            onItemAdd: function (value, $item) {
                AddInventory(value);
                this.removeItem(value, true);
            },
            onDropdownClose: function ($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true,
            plugins: {
                'sticky_placeholder': {},
                'no_results': {}
            }
        });
    }
};

function AddInventory(Inventory) {
    parent.Ts.Services.Assets.GetAsset(Inventory, function (asset) {
        var InventoryDiv = $("#ticket-Inventory");
        $("#ticket-Inventory-Input").val('');

        var newelement = PrependTag(InventoryDiv, asset.AssetID, ellipseString(asset.Name, 30), asset, "tag-item AssetAnchor");
        newelement.data('assetid', asset.AssetID).data('placement', 'left');
    });
};

function SetupUserQueuesSection() {
    if ($('#ticket-UserQueue-Input').length) {
        $('#ticket-UserQueue-Input').selectize({
            valueField: 'id',
            labelField: 'label',
            searchField: 'label',
            load: function (query, callback) {
                this.clearOptions();        // clear the data
                this.renderCache = {};      // clear the html template cache
                getUsers(query, callback)
            },
            score: function (search) { return function (option) { return 1; } },
            onItemAdd: function (value, $item) {
                var item = new Object();
                item.name = $item.text();
                item.id = value;
                AddQueues(item);
                this.removeItem(value, true);
            },
            plugins: {
                'sticky_placeholder': {},
                'no_results': {}
            },
            onDropdownClose: function ($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });
    }
}

function AddQueues(queues) {
    var UserQueueDiv = $("#ticket-UserQueue");
    $("#ticket-UserQueue-Input").val('');
    var newelement = PrependTag(UserQueueDiv, queues.id, queues.name, queues, "tag-item UserAnchor");
    newelement.data('userid', queues.id).data('placement', 'left').data('ticketid', 0);
}

function SetupSubscribedUsersSection() {
    if ($('#ticket-SubscribedUsers-Input').length) {
        $('#ticket-SubscribedUsers-Input').selectize({
            valueField: 'id',
            labelField: 'label',
            searchField: 'label',
            load: function (query, callback) {
                this.clearOptions();        // clear the data
                this.renderCache = {};      // clear the html template cache
                getUsers(query, callback)
            },
            score: function (search) { return function (option) { return 1; } },
            onItemAdd: function (value, $item) {
                var item = new Object();
                item.name = $item.text();
                item.id = value;
                AddSubscribers(item);
                this.removeItem(value, true);
            },
            plugins: {
                'sticky_placeholder': {},
                'no_results': {}
            },
            onDropdownClose: function ($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });
    }
};

function AddSubscribers(Subscribers) {
    var SubscribersDiv = $("#ticket-SubscribedUsers");
    $("#ticket-SubscribedUsers-Input").val('');
    var newelement = PrependTag(SubscribersDiv, Subscribers.id, Subscribers.name, Subscribers, "tag-item UserAnchor");
    newelement.data('userid', Subscribers.id).data('placement', 'left').data('ticketid', 0);
}

function SetupRemindersSection() {
    $('#ticket-reminder-date').datetimepicker({ useCurrent: true, format: 'MM/DD/YYYY hh:mm A', defaultDate: new Date() });

    if ($('#ticket-reminder-who').length) {
        var $reminderSelect = $('#ticket-reminder-who').selectize({
            valueField: 'id',
            labelField: 'label',
            searchField: 'label',
            load: function (query, callback) {
                this.clearOptions();        // clear the data
                this.renderCache = {};      // clear the html template cache
                parent.Ts.Services.TicketPage.SearchUsers(query, function (result) {
                    callback(result);
                });
            },
            score: function (search) { return function (option) { return 1; } },
            onDropdownClose: function ($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });
    }

    $('#ticket-reminder-save').click(function (e) {
        var selectizeControl = $reminderSelect[0].selectize;
        var date = parent.Ts.Utils.getMsDate($('#ticket-reminder-date').val());
        var userid = selectizeControl.getValue();
        if (userid == "") {
            $('#ticket-reminder-who').parent().addClass('has-error').removeClass('has-success');
        } else {
            $('#ticket-reminder-who').closest('form-group').addClass('has-success').removeClass('has-error');
        }
        var title = $('#ticket-reminder-title').val();
        if (title == "") {
            $('#ticket-reminder-title').parent().addClass('has-error').removeClass('has-success');
        } else {
            $('#ticket-reminder-title').parent().addClass('has-success').removeClass('has-error');
        }
        var label = ellipseString(title, 30) + '<br>' + date.localeFormat(parent.Ts.Utils.getDateTimePattern())
        PrependTag($("#ticket-reminder-span"), userid, label, null);
        $('#RemindersModal').modal('hide')
    });
}

function SetupAssociatedTicketsSection() {
    if ($('#ticket-AssociatedTickets-Input').length) {
        $('#ticket-AssociatedTickets-Input').selectize({
            valueField: 'data',
            labelField: 'label',
            searchField: 'label',
            loadThrottle: null,
            load: function (query, callback) {
                this.clearOptions();        // clear the data
                this.renderCache = {};      // clear the html template cache
                getRelated(query, callback)
            },
            score: function (search) {
                return function (option) {
                    return 1;
                }
            },
            onItemAdd: function (value, $item) {
                $('#AssociateTicketModal').data('ticketid', value).modal('show');
                this.removeItem(value, true);
            },
            plugins: {
                'sticky_placeholder': {},
                'no_results': {}
            },
            onDropdownClose: function ($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });

        $('#ticket-AssociatedTickets').on('click', 'div.tag-item', function (e) {
            var self = $(this);
            var data = self.data().tag;
            parent.Ts.MainPage.openTicket(data.TicketNumber, true);
        });

        $('.ticket-association').click(function (e) {
            var IsParent = $(this).data('isparent');
            var TicketID2 = $(this).closest('#AssociateTicketModal').data('ticketid');
            $('#associate-error').hide();

            $("#ticket-AssociatedTickets-Input").val('');
            AddAssociatedTickets(TicketID2, IsParent);
            $('#AssociateTicketModal').modal('hide');
        });
    }
};

function AddAssociatedTickets(ticketid, IsParent) {
    parent.Ts.Services.Tickets.GetTicket(ticketid, function (ticket) {
        if (ticket !== null) {
            var AssociatedTicketsDiv = $("#ticket-AssociatedTickets");
            var caption = 'Related';
            if (IsParent !== null) {
                caption = (IsParent === true ? 'Parent' : 'Child');
            }
            var label = caption + "<br />" + ellipseString(ticket.TicketNumber + ': ' + ticket.Name, 30);
            var newelement = PrependTag(AssociatedTicketsDiv, ticket.TicketID, ticket.IsClosed ? '<s>' + label + '</s>' : label, ticket, 'tag-item TicketAnchor');
            newelement.data('ticketid', ticket.TicketID).data('placement', 'left').data('IsParent', IsParent);
        }
    });
};

function AddCustomField(field, parentContainer, loadConditionalFields) {
    try {
        UpdateValueFromCache(field);     // preserve current values across ticket types
        switch (field.FieldType) {
            case parent.Ts.CustomFieldType.Text:
                AddCustomFieldEdit(field, parentContainer);
                break;
            case parent.Ts.CustomFieldType.Date:
                AddCustomFieldDate(field, parentContainer);
                break;
            case parent.Ts.CustomFieldType.Time:
                AddCustomFieldTime(field, parentContainer);
                break;
            case parent.Ts.CustomFieldType.DateTime:
                AddCustomFieldDateTime(field, parentContainer);
                break;
            case parent.Ts.CustomFieldType.Boolean:
                AddCustomFieldBool(field, parentContainer);
                break;
            case parent.Ts.CustomFieldType.Number:
                AddCustomFieldNumber(field, parentContainer);
                break;
            case parent.Ts.CustomFieldType.PickList:
                AddCustomFieldSelect(field, parentContainer, loadConditionalFields);
                break;
        }
    }
    catch (err) {
        var errorString = '1001 NewTicket.js createCustomFields   FieldType: ' + field.FieldType + '  CustomFieldID: ' + field.CustomFieldID + ' ::  Exception Properties: ';
        for (var property in err) { errorString += property + ': ' + err[property] + '; '; }
        parent.Ts.Services.System.LogException(err.message, errorString);
    }

}

function createCustomFields() {
    var ticketTypeID = $('#ticket-type').val(); // load only the custom fields for this ticket type
    parent.Ts.Services.CustomFields.GetParentCustomFields(parent.Ts.ReferenceTypes.Tickets, ticketTypeID, function (result) {
        var parentContainer = $('#ticket-group-custom-fields');
        if (result === null || result.length < 1) { parentContainer.empty().hide(); return; }
        parentContainer.empty()
        parentContainer.hide();
        _parentFields = [];
        for (var i = 0; i < result.length; i++) {
            if (!result[i].CustomFieldCategoryID) {
                AddCustomField(result[i], parentContainer, true);
            }
        }
        parentContainer.show();
        var container = $('#ticket-group-categorized-custom-fields');
        container.empty();  // clear ticket-group-categorized-custom-fields
        appendCategorizedCustomFields(result, null);
    });
};

var appendCategorizedCustomFields = function (fields, className) {
    parent.Ts.Services.CustomFields.GetAllTypesCategories(parent.Ts.ReferenceTypes.Tickets, function (categories) {
        var container = $('#ticket-group-categorized-custom-fields');
        for (var j = 0; j < categories.length; j++) {
            var catWrap = $('#CFCatWrap-' + categories[j].CustomFieldCategoryID);
            //TODO:  Wrap header and hr together inside a span so they can both be removed easily
            //var container = $('<div>').addClass('cf-category category-' + categories[j].CustomFieldCategoryID).appendTo(parentcontainer);
            if (catWrap.length == 0) {
                catWrap = $('<div id=CFCatWrap-' + categories[j].CustomFieldCategoryID + '>');
                var header = $('<label id=CFCat-' + categories[j].CustomFieldCategoryID + '>').text(categories[j].Category).addClass('customFieldCategoryHeader');
                catWrap.append($('<hr>')).append(header);
                container.append(catWrap);
                catWrap.hide();
            }

            for (var i = 0; i < fields.length; i++) {
                var item = null;
                var field = fields[i];
                if (field.CustomFieldCategoryID == categories[j].CustomFieldCategoryID) {
                    catWrap.show();
                    AddCustomField(field, catWrap, false);
                }
            }
        }
        appendConditionalFields();
        showCustomFields();
        container.show();
    });
}

function showCustomFields() {
    var ticketTypeID = $('#ticket-type').val();
    $('.custom-field').hide().each(function () {
        var field = $(this).data('field');
        if (field.AuxID == ticketTypeID) {
            $(this).show();
            if (field.CustomFieldCategoryID !== null) $('#CFCat-' + field.CustomFieldCategoryID).show().prev().show();
            if (field.ParentProductID !== null) $('#CFGroupProduct-' + field.ParentProductID).show();
        } else {
            $(this).hide();
            if (field.CustomFieldCategoryID !== null) $('#CFCat-' + field.CustomFieldCategoryID).hide().prev().hide();
            if (field.ParentProductID !== null) $('#CFGroupProduct-' + field.ParentProductID).hide();
        }
    });
};

function SetupActionTimers() {
    if ($('#action-new-date-started').data("DateTimePicker")) {
        $('#action-new-date-started').data("DateTimePicker").destroy();
    }
    parent.Ts.Services.Customers.GetDateFormat(false, function (format) {
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
        $('#action-new-date-started').datetimepicker({ format: dateFormat + ' hh:mm A', defaultDate: new Date() });
    });

    $('.spinner .btn:first-of-type').click(function () {
        var spinner = $(this).parent().prev();
        spinner.val(parseInt(spinner.val(), 10) + 1);
    });
    $('.spinner .btn:last-of-type').click(function () {
        var spinner = $(this).parent().prev();
        spinner.val(parseInt(spinner.val(), 10) - 1);
    });
    $('#action-new-timer').click(function (e) {
        var hasStarted = $(this).data('hasstarted');
        if (!hasStarted) {
            start = new Date().getTime();
            tickettimer();
            $(this).find(':first-child').css('color', 'green');
        } else {
            $(this).find(':first-child').css('color', 'red');
            clearTimeout(_timerid);
        }
        $(this).data('hasstarted', !hasStarted);
    });
}

var AddCustomFieldEdit = function (field, parentContainer) {
    var formcontainer  = $('<div>').addClass('form-horizontal custom-field').data('field', field).appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('flexbox').data('field', field).appendTo(formcontainer);
    var labelContainer = $('<div>').addClass('flex1').appendTo(groupContainer);
    var formLabel      = $('<div>').addClass('form-label').text(field.Name).appendTo(labelContainer);

    if (field.ParentProductID) { formcontainer.addClass('product-dependent'); }

    var inputContainer = $('<div>').addClass('flex2').appendTo(groupContainer);
    var inputGroupContainer = $('<div>').addClass('input-group').appendTo(inputContainer);
    var input = $('<input type="text">').addClass('ticket-simple-input').attr("placeholder", "Enter Value").val(field.Value).appendTo(inputGroupContainer).after(getUrls(field.Value));

    if (field.Mask) {
        input.mask(field.Mask);
        input.attr("placeholder", field.Mask);
    }

    input.change(function (e) {
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
        //parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
        //  groupContainer.data('field', result);
        //  groupContainer.find('.external-link').remove();
        //  input.after(getUrls(result.Value));
        //  window.parent.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
        //}, function () {
        //  alert("There was a problem saving your ticket property.");
        //});
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

var AddCustomFieldDate = function (field, parentContainer) {
    var date = field.Value == null ? null : parent.Ts.Utils.getMsDate(field.Value);
    var formcontainer  = $('<div>').addClass('form-horizontal custom-field').data('field', field).appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('flexbox').data('field', field).appendTo(formcontainer);
    var labelContainer = $('<div>').addClass('flex1').appendTo(groupContainer);
    var formLabel      = $('<div>').addClass('form-label').text(field.Name).appendTo(labelContainer);

    if (field.ParentProductID) {
        formcontainer.addClass('product-dependent');
    }
    var dateContainer = $('<div>').addClass('flex2').attr('style','padding-top:3px;').appendTo(groupContainer);
    var dateLink = $('<a>').attr('href', '#').text((date === null ? 'unassigned' : date.localeFormat(parent.Ts.Utils.getDatePattern()))).addClass('control-label').appendTo(dateContainer);

    dateLink.click(function (e) {
        e.preventDefault();
        $(this).hide();
        var input = $('<input type="text">').val(date === null ? '' : date.localeFormat(parent.Ts.Utils.getDatePattern())).datetimepicker({ pickTime: false }).appendTo(dateContainer).focus();

        input.focusout(function (e) {
            var value = parent.Ts.Utils.getMsDate(input.val());
            $(this).datepicker("hide");
            input.hide();
            dateLink.text((value === null ? 'Unassigned' : value.localeFormat(parent.Ts.Utils.getDatePattern()))).show();

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
        })
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

var AddCustomFieldDateTime = function (field, parentContainer) {
    var date = field.Value == null ? null : parent.Ts.Utils.getMsDate(field.Value);
    var formcontainer  = $('<div>').addClass('form-horizonta custom-field ').data('field', field).appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('flexbox').data('field', field).appendTo(formcontainer);
    var labelContainer = $('<div>').addClass('flex1').appendTo(groupContainer);
    var formLabel      = $('<div>').addClass('form-label').text(field.Name).appendTo(labelContainer);

    if (field.ParentProductID) { formcontainer.addClass('product-dependent'); }

    var dateContainer = $('<div>').addClass('flex2').attr('style', 'padding-top: 3px;').appendTo(groupContainer);
    var dateLink = $('<a>').attr('href', '#').text((date === null ? 'unassigned' : date.localeFormat(parent.Ts.Utils.getDateTimePattern()))).addClass('control-label').appendTo(dateContainer);

    dateLink.click(function (e) {
        e.preventDefault();
        $(this).hide();
        var input = $('<input type="text">').val(date === null ? '' : date.localeFormat(parent.Ts.Utils.getDateTimePattern())).datetimepicker().appendTo(dateContainer).focus();

        input.focusout(function (e) {
            var value = parent.Ts.Utils.getMsDate(input.val());
            $(this).datepicker("hide");
            input.hide();
            dateLink.text((value === null ? 'Unassigned' : value.localeFormat(parent.Ts.Utils.getDateTimePattern()))).show();

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
        })
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

var AddCustomFieldTime = function (field, parentContainer) {
    var date = field.Value == null ? null : parent.Ts.Utils.getMsDate(field.Value);
    var formcontainer  = $('<div>').addClass('form-horizontal custom-field').data('field', field).appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('flexbox').data('field', field).appendTo(formcontainer);
    var labelContainer = $('<div>').addClass('flex1').appendTo(groupContainer);
    var formLabel      = $('<div>').addClass('form-label').text(field.Name).appendTo(labelContainer);

    if (field.ParentProductID) { formcontainer.addClass('product-dependent'); }

    var dateContainer = $('<div>').addClass('flex2').attr('style','padding-top:3px;').appendTo(groupContainer);
    var dateLink = $('<a>').attr('href', '#').text((date === null ? 'unassigned' : date.localeFormat(parent.Ts.Utils.getTimePattern()))).addClass('control-label').appendTo(dateContainer);

    dateLink.click(function (e) {
        e.preventDefault();
        $(this).hide();
        var input = $('<input type="text">').val(date === null ? '' : date.localeFormat(parent.Ts.Utils.getTimePattern())).datetimepicker({ pickDate: false }).appendTo(dateContainer).focus();

        input.focusout(function (e) {
            var value = parent.Ts.Utils.getMsDate("1/1/1900 " + input.val());
            $(this).datepicker("hide");
            input.hide();
            dateLink.text((value === null ? 'Unassigned' : value.localeFormat(parent.Ts.Utils.getTimePattern()))).show();

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
        })
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

var AddCustomFieldBool = function (field, parentContainer) {
    var formcontainer  = $('<div>').addClass('form-horizontal custom-field').data('field', field).appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('flexbox').data('field', field).appendTo(formcontainer);
    var labelContainer = $('<div>').addClass('flex1').appendTo(groupContainer);
    var formLabel      = $('<div>').addClass('form-label').text(field.Name).appendTo(labelContainer);

    if (field.ParentProductID) { formcontainer.addClass('product-dependent'); }

    var inputContainer = $('<div>').addClass('flex2').appendTo(groupContainer);
    var inputDiv = $('<div>').addClass('checkbox ticket-checkbox').appendTo(inputContainer);
    var input = $('<input type="checkbox">').appendTo(inputDiv);
    var value = (field.Value === null || $.trim(field.Value) === '' || field.Value.toLowerCase() === 'false' || field.Value.toLowerCase() === '0' ? false : true);
    input.attr("checked", value);
};

var AddCustomFieldNumber = function (field, parentContainer) {
    var formcontainer  = $('<div>').addClass('form-horizontal custom-field').data('field', field).appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('flexbox').data('field', field).appendTo(formcontainer);
    var labelContainer = $('<div>').addClass('flex1').appendTo(groupContainer);
    var formLabel      = $('<div>').addClass('form-label').text(field.Name).appendTo(labelContainer);

    if (field.ParentProductID) { formcontainer.addClass('product-dependent'); }

    var inputContainer = $('<div>').addClass('flex2').appendTo(groupContainer);
    var input = $('<input type="text">').attr("placeholder", "Enter Value").val(field.Value).appendTo(inputContainer).numeric();

    input.change(function (e) {
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

var AddCustomFieldSelect = function (field, parentContainer, loadConditionalFields) {
    var formcontainer  = $('<div>').addClass('form-horizontal custom-field').data('field', field).appendTo(parentContainer);
    var groupContainer = $('<div>').addClass('flexbox').data('field', field).appendTo(formcontainer);
    var labelContainer = $('<div>').addClass('flex1').appendTo(groupContainer);
    var formLabel      = $('<div>').addClass('form-label').text(field.Name).appendTo(labelContainer);

    if (field.ParentProductID) { formcontainer.addClass('product-dependent'); }

    var selectContainer = $('<div>').addClass('flex2').appendTo(groupContainer);
    var select = $('<select>').addClass('hidden-select').attr("placeholder", "Select Value").appendTo(selectContainer);
    var options = field.ListValues.split('|');

    if (field.Value == undefined || field.Value == "") {
        $('<option>').text("unassigned").val("").appendTo(select);
        if (field.IsRequired) formcontainer.addClass('hasError');
    }
    for (var i = 0; i < options.length; i++) {
        var optionValue = options[i];
        var option = $('<option>').text(optionValue).val(optionValue).appendTo(select);
        if (field.Value === options[i]) option.attr('selected', 'selected');
    }

    //appendTemplateText(options[0]);

    select.selectize({
        allowEmptyOption: true,
        onItemAdd: function (value, $item) {
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
            if (field.IsFirstIndexSelect == true && value == "" || (field.IsFirstIndexSelect == true && field.ListValues.split("|")[0] == value)) {
                formcontainer.addClass('isEmpty');
            } else {
                formcontainer.removeClass('isEmpty');
            }
            $('.' + field.CustomFieldID + 'children').remove();
            var childrenContainer = $('<div>').addClass(field.CustomFieldID + 'children form-horizontal').insertAfter(formcontainer);
            appendMatchingParentValueFields(childrenContainer, field, value);
            appendTemplateText(value);
        },
        onDropdownClose: function ($dropdown) {
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
};

var appendTemplateText = function (value) {
    parent.Ts.Services.Tickets.GetValueTemplateText(value, function (result) {
        if (result != null && result != "" && result != "<br>") {
            var currenttext = tinyMCE.activeEditor.getContent();
            tinyMCE.activeEditor.setContent(currenttext + result);
        }
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

var appendMatchingParentValueFields = function (container, field, value) {
    var items = field.ListValues.split('|');
    var ticketTypeID = $('#ticket-type').val();
    if (field.AuxID == ticketTypeID && items.length > 0) {
        if (value == null) value = items[0];
        var productID = $('#ticket-Product').val();
        if (productID == undefined || productID == "") productID = "-1";
        parent.Ts.Services.CustomFields.GetParentValueMatchingCustomFields(field.CustomFieldID, value, productID, function (result) {
            for (var i = 0; i < result.length; i++) {
                var field = result[i];
                var div = $('<div>').addClass('custom-field').data('field', field);
                container.append(div);
                AddCustomField(field, div, true);
            }
        });
    }
    showCustomFields();
};

function AppendProductMatchingCustomFields() {
    $('.product-dependent').remove();
    var productID = $('#ticket-Product').val();
    if (productID == undefined || productID == "") productID = "-1";
    parent.Ts.Services.CustomFields.GetProductMatchingCustomFields(parent.Ts.ReferenceTypes.Tickets, _lastTicketTypeID, productID, function (result) {
        var container = $('#ticket-group-custom-fields');
        for (var i = 0; i < result.length; i++) {
            if (!result[i].CustomFieldCategoryID) {
                AddCustomField(result[i], container, false);
            }
        }
        appendCategorizedCustomFields(result, null);
    });
};

function AppendTicketTypeTemplate(TicketType) {
    parent.Ts.Services.Tickets.GetTicketTypeTemplateText(TicketType, function (result) {
        if (result != null && result != "" && result != "<br>") {
            if (tinyMCE.activeEditor) {
                var currenttext = tinyMCE.activeEditor.getContent();
                tinyMCE.activeEditor.setContent(currenttext + result);
            }
        }
    });
};

function setInitialValue() {
    var menuID = parent.Ts.MainPage.MainMenu.getSelected().getId().toLowerCase();
    switch (menuID) {
        case 'mniusers':
            //parent.Ts.Services.Settings.ReadUserSetting('SelectedUserID', -1, function (result) {
            //  if (result > -1) $('.newticket-user').combobox('setValue', result);
            //});
            break;
        case 'mniproducts':
            parent.Ts.Services.Settings.ReadUserSetting('SelectedProductID', -1, function (productID) {
                if (productID > -1) {
                    var product = parent.Ts.Cache.getProduct(productID);
                    SetProduct(productID)
                    loadVersions(product);
                    AppendProductMatchingCustomFields();
                    parent.Ts.Services.Organizations.IsProductRequired(function (IsRequired) {
                        if (IsRequired)
                            $('#ticket-Product').closest('.form-horizontal').addClass('hasError');
                        else
                            $('#ticket-Product').closest('.form-horizontal').removeClass('hasError');
                    });
                    if (parent.Ts.System.Organization.UseProductFamilies && _productFamilyID != product.ProductFamilyID) {
                        _productFamilyID = product.ProductFamilyID;
                        UpdateTicketGroups(function (persistedGroup) {
                            UpdateTicketTypes(persistedGroup, function (persistedData) {
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
            });
            break;
        case 'mnikb':
            if (canEdit) {
                $('#ticket-isKB').prop('checked', true);
            }
            break;
        case 'mniinventory':
            parent.Ts.Services.Settings.ReadUserSetting('SelectedAssetID', -1, function (assetID) {
                if (assetID > -1) {
                    AddInventory(assetID);
                }
            });
            break;
        default:
            if (menuID.indexOf('tickettype') > -1) {
                var ticketTypeID = menuID.substr(14, menuID.length - 14);
                SetType(ticketTypeID);
                showCustomFields();
                _lastTicketTypeID = ticketTypeID;
            }
            var contactID = top.Ts.Utils.getQueryValue('contactID', window);
            var companyID = top.Ts.Utils.getQueryValue('customerID', window);
            var organizationID = top.Ts.Utils.getQueryValue('organizationID', window);
            var userID = top.Ts.Utils.getQueryValue('user', window);
            if (contactID != null) {
                var org = new Object();
                org.value = contactID;
                org.type = "u";
                AddCustomers(org);
            } else if (companyID != null) {
                var org = new Object();
                org.value = companyID;
                org.type = "o";
                AddCustomers(org);
            }

            if (userID != null) {
                var org = new Object();
                org.value = userID;
                org.type = "u";
                AddCustomers(org);
            } else if (organizationID != null) {
                var org = new Object();
                org.value = organizationID;
                org.type = "o";
                AddCustomers(org);
            }
    }

    var chatID = parent.Ts.Utils.getQueryValue('chatid', window)
    if (chatID && chatID != null) {
        parent.Ts.Services.Tickets.GetChatCustomer(chatID, function (result) {
            if (result.UserID > -1) {
                var user = new Object();
                user.value = result.UserID;
                user.type = "u";
                AddCustomers(user);
            } else if (result.OrganizationID > -1) {
                var org = new Object();
                org.value = result.OrganizationID;
                org.type = "o";
                AddCustomers(org);
            }
        });
    }

	var groupID = parent.Ts.Utils.getQueryValue('groupid', window)
	if (groupID && groupID != null) {
		SetGroup(groupID);
	}
}

var SetType = function (TypeID) {
    var selectField = $('#ticket-type');
    if (selectField.length > 0) {
        var selectize = $('#ticket-type')[0].selectize;
        selectize.addItem(TypeID, false);
    }
};

var SetProduct = function (ProductID) {
    var selectField = $('#ticket-Product');
    if (selectField.length > 0) {
        var selectize = $('#ticket-Product')[0].selectize;
        selectize.addItem(ProductID, false);
    }
};

var SetGroup = function (GroupID) {
    var selectField = $('#ticket-group');
    if (selectField.length > 0) {
        var selectize = $('#ticket-group')[0].selectize;
        selectize.addItem(GroupID, false);
        _ticketGroupID = GroupID;
    }
};

var SetAssignedUser = function (ID) {
    var selectUserField = $('#ticket-assigned');
    if (selectUserField.length > 0) {
        var selectizeUserField = $('#ticket-assigned')[0].selectize;
        selectizeUserField.addItem(ID, false);
    }
};

function CreateHandleBarHelpers() {
    Handlebars.registerHelper('UserImageTag', function () {
        if (this.item.CreatorID !== -1) return '<img class="user-avatar pull-left" src="../../../dc/' + this.item.OrganizationID + '/useravatar/' + this.item.CreatorID + '/48" />';
        return '';
    });

    Handlebars.registerHelper('FormatDateTime', function (Date) {
        return Date.localeFormat(parent.Ts.Utils.getDateTimePattern())
    });

    Handlebars.registerHelper('ActionData', function () {
        return JSON.stringify(this.item);
    });

    Handlebars.registerHelper('TagData', function () {
        return JSON.stringify(this.data);
    });

    Handlebars.registerHelper('CanKB', function (options) {
        if (parent.Ts.System.User.ChangeKbVisibility || parent.Ts.System.User.IsSystemAdmin) { return options.fn(this); }
    });

    Handlebars.registerHelper('CanMakeVisible', function (options) {
        if (parent.Ts.System.User.ChangeTicketVisibility || parent.Ts.System.User.IsSystemAdmin) { return options.fn(this); }
    });
};
