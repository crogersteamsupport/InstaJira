/// <reference path="ts/ts.js" />
/// <reference path="ts/window.parent.parent.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />
var slaLevels;
var actionTypes;
var organizationStatuses;
var ticketTypes;
var jiraInstances;
var adminInt = null;
var _selectedJiraInstance = "";
var _isNewJiraInstance = false;
var _isDefaultJiraInstance = false;
var _allowDefaultNewInstanceForJira = false;
var _anyJiraInstance = false;

$(document).ready(function () {
    adminInt = new AdminInt();
    adminInt.refresh();
});

function onShow() {
    adminInt.refresh();
};

AdminInt = function () {
    window.parent.parent.Ts.Services.Organizations.GetSlaLevels(function (result) {
        slaLevels = result;
    });

    actionTypes = window.parent.parent.Ts.Cache.getActionTypes();

    window.parent.parent.Ts.Services.Tickets.GetTicketStatusesOrderedByTicketTypeName(function (result) {
        organizationStatuses = result;
    });

    ticketTypes = window.parent.parent.Ts.Cache.getTicketTypes();

    $('#btnRefresh')
    .click(function (e) {
        e.preventDefault();
        window.location = window.location;
    })
    .toggle(window.location.hostname.indexOf('127.0.0.1') > -1);

    $('button').button();

    $('a').addClass('ui-state-default ts-link');
    $('.int-support').click(function (e) {
        e.preventDefault();
        window.parent.parent.Ts.System.openSupport();
    });

    window.parent.parent.Ts.Services.Organizations.GetCrmLinks(function (result) {
        jiraInstances = result;
        _anyJiraInstance = false;

        //TFS
        $('.int-tfs-update-status').prop('checked', false);
        $('#tfsExclusionTicketStatusList').hide();
        $('#tfsTicketStatusExceptionSpan').hide();
		$('#tfsTicketStatusExceptionSpan').hide();

		//ServiceNow
		$('#snowExclusionTicketStatusList').hide();
		$('#snowTicketStatusExceptionSpan').hide();
		$('#snowTicketStatusExceptionSpan').hide();

        for (var i = 0; i < result.length; i++) {
            //On first load, just load the Default Jira instance.
            if ((result[i].CRMType.toLowerCase() == 'jira' && result[i].InstanceName.toLowerCase() == 'default')
              || result[i].CRMType.toLowerCase() != 'jira') {
                loadPanel($('.' + result[i].CRMType.toLowerCase() + ' .int-panel').data('link', result[i]));
            }

            if (result[i].CRMType.toLowerCase() == 'jira') {
                _allowDefaultNewInstanceForJira = false;
                _anyJiraInstance = true;
            }
        }

        if (result.length == 0 || (result.length > 0 && !_anyJiraInstance)) {
            SetupInitialDefaultInstanceCreate();
		}
    });

    function loadPanel(element) {
        element = $(element);
        var item = element.data('link');
        if (item == null) return;

		element.addClass('crmlinkid_' + item.CRMLinkID);

		element.find('.int-crm-instancename').val(item.InstanceName);
		if (item.InstanceName == 'Default') {
			element.find('.int-crm-instancename').attr('disabled', 'disabled');
			_isDefaultJiraInstance = true;
		} else {
			element.find('.int-crm-instancename').removeAttr('disabled');
			_isDefaultJiraInstance = false;
		}

		element.find('.int-crm-host').val(item.HostName);
		element.find('.int-crm-user').val(item.Username);
		element.find('.int-crm-password').val(item.Password);
		element.find('.int-crm-password-confirm').val(item.Password);

		element.find('.int-crm-use-network-credentials').prop('checked', item.UseNetworkCredentials);

		element.find('.int-crm-token').val(item.SecurityToken1);
		element.find('.int-crm-token-confirm').val(item.SecurityToken1);
		element.find('.int-crm-tag').val(item.TypeFieldMatch);
		element.find('.int-crm-project').val(item.DefaultProject);
		if (item.Active) {
			element.find('.int-crm-active').prop('checked', true);
		}
		else {
			element.find('.int-crm-active').prop('checked', false);
		}
		if (item.PullCasesAsTickets) {
			element.find('.int-crm-pull-cases-as-tickets').prop('checked', true);
		}
		else {
			element.find('.int-crm-pull-cases-as-tickets').prop('checked', false);
		}
		if (item.PushTicketsAsCases) {
			element.find('.int-crm-push-tickets-as-cases').prop('checked', true);
		}
		else {
			element.find('.int-crm-push-tickets-as-cases').prop('checked', false);
		}
		if (item.SendBackTicketData) {
			element.find('.int-crm-push-tickets-as-account-comments').prop('checked', true);
		}
		else {
			element.find('.int-crm-push-tickets-as-account-comments').prop('checked', false);
		}
		if (item.PullCustomerProducts) {
			element.find('.int-crm-pull-customer-products').prop('checked', true);
		}
		else {
			element.find('.int-crm-pull-customer-products').prop('checked', false);
		}
		if (item.SendWelcomeEmail) {
			element.find('.int-crm-email').prop('checked', true);
		}
		else {
			element.find('.int-crm-email').prop('checked', false);
		}
		if (item.AllowPortalAccess) {
			element.find('.int-crm-portal').prop('checked', true);
		}
		else {
			element.find('.int-crm-portal').prop('checked', false);
		}

		if (item.CRMType == 'Jira') {
			loadActionTypes(element);
		}

		if (item.UpdateStatus) {
			if (item.CRMType == 'Jira') {
				element.find('.int-crm-update-status').prop('checked', true);
				loadOrganizationStatusesWithType(element);
				element.find('#exclusionTicketStatusList').show();
				element.find('#ticketStatusExceptionSpan').show();
			} else if (item.CRMType == 'TFS') {
				element.find('.int-tfs-update-status').prop('checked', true);
				loadOrganizationStatusesWithType(element);
				element.find('#tfsExclusionTicketStatusList').show();
				element.find('#tfsTicketStatusExceptionSpan').show();
			} else if (item.CRMType == 'ServiceNow') {
				element.find('.int-snow-update-status').prop('checked', true);
				loadOrganizationStatusesWithType(element);
				element.find('#snowExclusionTicketStatusList').show();
				element.find('#snowTicketStatusExceptionSpan').show();
			}

		}
		else {
			if (item.CRMType == 'Jira') {
				element.find('.int-crm-update-status').prop('checked', false);
				element.find('#exclusionTicketStatusList').hide();
				element.find('#ticketStatusExceptionSpan').hide();
			} else if (item.CRMType == 'TFS') {
				element.find('.int-tfs-update-status').prop('checked', false);
				element.find('#tfsExclusionTicketStatusList').hide();
				element.find('#tfsTicketStatusExceptionSpan').hide();
			} else if (item.CRMType == 'ServiceNow') {
				element.find('.int-snow-update-status').prop('checked', false);
				element.find('#snowExclusionTicketStatusList').hide();
				element.find('#snowTicketStatusExceptionSpan').hide();
			}
		}

		element.find('.int-crm-update-type').prop('checked', item.UpdateTicketType);

		if (item.MatchAccountsByName) {
			element.find('.int-crm-match-accounts-by-name').prop('checked', true);
		}
		else {
			element.find('.int-crm-match-accounts-by-name').prop('checked', false);
		}
		if (item.UseSandBoxServer) {
			element.find('.int-crm-use-sandbox-server').prop('checked', true);
		}
		else {
			element.find('.int-crm-use-sandbox-server').prop('checked', false);
		}
		if (item.AlwaysUseDefaultProjectKey) {
			element.find('.int-crm-always-use-default-project-key').prop('checked', true);
		}
		else {
			element.find('.int-crm-always-use-default-project-key').prop('checked', false);
		}

		if (item.IncludeIssueNonRequired) {
			element.find('.int-crm-IncludeIssueNonRequired').prop('checked', true);
		}
		else {
			element.find('.int-crm-IncludeIssueNonRequired').prop('checked', false);
		}

		if (item.RestrictedToTicketTypes) {
			if (item.CRMType == 'Jira') {
				element.find('.int-crm-ticket-types').prop('checked', false);
				loadTicketTypes(element);
				element.find('#restrictedTicketTypesList').show();
			} else if (item.CRMType == 'TFS') {
				element.find('.int-tfs-ticket-types').prop('checked', false);
				loadTicketTypes(element);
				element.find('#restrictedTicketTypesListTFS').show();
			} else if (item.CRMType == 'ServiceNow') {
				element.find('.int-snow-ticket-types').prop('checked', false);
				loadTicketTypes(element);
				element.find('#restrictedTicketTypesListSnow').show();
			}
		}
		else {
			if (item.CRMType == 'Jira') {
				element.find('.int-crm-ticket-types').prop('checked', true);
				element.find('#restrictedTicketTypesList').hide();
			} else if (item.CRMType == 'TFS') {
				element.find('.int-tfs-ticket-types').prop('checked', true);
				element.find('#restrictedTicketTypesListTFS').hide();
			} else if (item.CRMType == 'ServiceNow') {
				element.find('.int-snow-ticket-types').prop('checked', true);
				element.find('#restrictedTicketTypesListSnow').hide();
			}
		}

		//ZohoReports
		var zohoGrids = element.find('.int-zohoreports-grid');
		zohoGrids.empty();
		var selected = '" selected="selected">';

		if (item.HostName != '' || item.HostName.indexOf('reportsapi.zoho.com') >= 0) {
			zohoGrids.append('<option value="US"' + selected + 'US</option>');
			zohoGrids.append('<option value="EU">EU</option>');
		} else {
			zohoGrids.append('<option value="US">US</option>');
			zohoGrids.append('<option value="EU"' + selected + 'EU</option>');
		}

		$("#AddingInstanceLabel").hide();
		$("#JiraInstacesListWrapper").show();
		$("#NewInstance").show();

		element.find('.int-crm-webhook').val(item.WebHookTokenFullUrl);

			var showModalButton = document.getElementById("btnBusinessRule");
			showModalButton.onclick = function () {
				var rawFile = new XMLHttpRequest();
				rawFile.open("GET", "ServiceNowBusinessRule.txt", false);
				rawFile.onreadystatechange = function () {
					if (rawFile.readyState === 4) {
						if (rawFile.status === 200 || rawFile.status == 0) {
							var allText = rawFile.responseText;
							var textArea = document.querySelector("#businessRuleScript");
							textArea.value = allText;
						}
					}
				}
				rawFile.send(null);

				$('#serviceNowBusinessRuleModal').modal();
			};

			if (item.CRMType == 'ServiceNow') {
			var copyToClipboardButton = document.getElementById("copyBusinessRuleToClipboard");
			copyToClipboardButton.onclick = function () {
				var copyTextArea = document.querySelector("#businessRuleScript").select();

				try {
					var successful = document.execCommand('copy');
				} catch (err) {
					console.log("unable to copy to clipboard");
				}

				this.focus();
			};
		}
	}
	

    $('#int-api-new').click(function (e) {
        e.preventDefault();
        if (confirm('Are you sure you would like to generate a new API token?' + '\n' + 'You will need to replace the token in all the applications that use the TeamSupport API.')) {
            window.parent.parent.Ts.Services.Organizations.GenerateNewApiToken(loadApiInfo);
            window.parent.parent.Ts.System.logAction('Admin Integration - API Token Generated');

        }
    });

    $('#int-api-enable').click(function (e) {
        e.preventDefault();
        window.parent.parent.Ts.Services.Organizations.SetApiEnabled(true, loadApiInfo);
        window.parent.parent.Ts.System.logAction('Admin Integration - API Toggled');
    });

    $('#int-api-disable').click(function (e) {
        e.preventDefault();
        window.parent.parent.Ts.Services.Organizations.SetApiEnabled(false, loadApiInfo);
        window.parent.parent.Ts.System.logAction('Admin Integration - API Toggled');
    });

    $('.int-api-link-users').click(function (e) {
        e.preventDefault();
        var info = $('.ts-api').data('info');
        if (info.IsEnabled === false) {
            if (confirm('You must have your API enabled before you can use it.' + '\n' + 'Would you like to enabled it now?')) {
                window.parent.parent.Ts.Services.Organizations.SetApiEnabled(true, function (result) {
                    loadApiInfo(result);
                    info = $('.ts-api').data('info');
                    window.open($('.int-api-link-users').attr('href'), "TSAPI");
                    window.parent.parent.Ts.System.logAction('Admin Integration - API Toggled');

                });
            }
        }
        else {
            window.open($('.int-api-link-users').attr('href'), "TSAPI");
        }
    });

    function loadApiInfo(info) {
        if (info.IsActive === true) {
            $('.ts-api').data('info', info);
            $('#int-api-active').show();
            $('#int-api-inactive').hide();
            $('#int-api-disable').toggle(info.IsEnabled);
            $('#int-api-enable').toggle(!info.IsEnabled);
            var list = $('.int-api-info').empty();
            $('<dt>').text('TeamSupport API token:').appendTo(list);
            $('<dd>').text(info.Token).appendTo(list);
            $('<dt>').text('Current requests in the last 24 hours:').appendTo(list);
            var current = info.RequestCount;
            if (info.RequestCount >= info.RequestMax) {
                current = '<strong class="ui-state-error">' + info.RequestCount + ' (You have reached your maximum amount of requests for the last 24 hours.)</strong>';
            }
            $('<dd>').html(current).appendTo(list);
            $('<dt>').text('Maximum requests per 24 hours:').appendTo(list);
            $('<dd>').text(info.RequestMax).appendTo(list);
            var link = 'https://' + window.parent.parent.Ts.System.Organization.OrganizationID + ':' + info.Token + '@' + window.parent.parent.Ts.System.AppDomain + '/api/xml/users';
            $('.int-api-link-users').attr('href', link).text(link);
        }
        else {
            $('#int-api-active').hide();
            $('#int-api-inactive').show();
        }
    }

    window.parent.parent.Ts.Services.Organizations.GetApiInfo(loadApiInfo);

    $('.int-list li').addClass('ui-widget-content ui-corner-all');

    $('.int-panel').hide();

    $('.int-list li h1.collapsable').click(function (e) {
        if ($(this).next().is(':visible')) {
            $(this).find('.ui-icon').addClass('ui-icon-triangle-1-e').removeClass('ui-icon-triangle-1-s');
            $(this).next().hide();
        }
        else {
            $(this).find('.ui-icon').addClass('ui-icon-triangle-1-s').removeClass('ui-icon-triangle-1-e');
            loadMaps($(this).next());
            loadSlaLevels($(this).next());
            loadActionTypes($(this).next());
            loadTicketTypes($(this).next());
            loadOrganizationStatusesWithType($(this).next());

            var crmTypeLabel = this.textContent;

            if (crmTypeLabel.toLowerCase() == 'jira') {
                loadJiraInstancesList($(this).next());
            }

            $(this).next().show();
        }
    });

    function loadMaps(element) {
        var data = element.data('link');
        element.find('.mappings').toggle(data != undefined);
        if (data == undefined) return;

        window.parent.parent.Ts.Services.Organizations.GetCrmLinkFields(data.CRMLinkID, function (fields) {
            loadMapFields(element, fields);
        });

        switch (data.CRMType) {
            case 'Salesforce':
                $('.int-map-type option').remove();
                $('<option>')
                  .text('Account')
                  .attr('value', window.parent.parent.Ts.ReferenceTypes.Organizations)
                  .attr('selected', 'selected')
                  .appendTo('.int-map-type');

                $('<option>')
                  .text('Contact')
                  .attr('value', window.parent.parent.Ts.ReferenceTypes.Contacts)
                  .appendTo('.int-map-type');

                $('<option>')
                  .text('Ticket')
                  .attr('value', window.parent.parent.Ts.ReferenceTypes.Tickets)
                  .appendTo('.int-map-type');

                $('.int-map-type').combobox({
                    selected: function (e, ui) {
                        loadFields(element.find('.int-map-tsfield'), element.find('.int-map-type').val());
                    }
                });

                loadFields(element.find('.int-map-tsfield'), element.find('.int-map-type').val());
                break;
            case 'Jira':
                $('.int-map-type option').remove();
                $('<option>')
                  .text('Ticket')
                  .attr('value', window.parent.parent.Ts.ReferenceTypes.Tickets)
                  .attr('selected', 'selected')
                  .appendTo('.int-map-type');

                $('<option>')
                  .text('Ticket Type')
                  .attr('value', window.parent.parent.Ts.ReferenceTypes.TicketTypes)
                  .appendTo('.int-map-type');

                $('.int-map-type').combobox({
                    selected: function (e, ui) {
                        loadFields(element.find('.int-jira-map-tsfield'), element.find('.int-map-type').val());
                }
                });

                loadFields(element.find('.int-jira-map-tsfield'), element.find('.int-map-type').val());
                break;
				case 'ServiceNow':
					if ($('.int-snow-map-field option').size() == 0) {
						loadFields(element.find('.int-snow-map-field'), 17);
					}
					break;
            case 'TFS':
                $('.int-map-type option').remove();
                $('<option>')
                  .text('Ticket')
                  .attr('value', window.parent.parent.Ts.ReferenceTypes.Tickets)
                  .attr('selected', 'selected')
                  .appendTo('.int-map-type');

                $('<option>')
                  .text('Ticket Type')
                  .attr('value', window.parent.parent.Ts.ReferenceTypes.TicketTypes)
                  .appendTo('.int-map-type');

                $('.int-map-type').combobox({
                    selected: function (e, ui) {
                        loadFields(element.find('.int-tfs-map-tsfield'), element.find('.int-map-type').val());
                    }
                });

                loadFields(element.find('.int-tfs-map-tsfield'), element.find('.int-map-type').val());
                break;
            case 'ZohoCrm':
                if ($('.int-zoho-map-type option').length < 1) {
                    $('<option>')
                      .text('Account')
                      .attr('value', window.parent.parent.Ts.ReferenceTypes.Organizations)
                      .attr('selected', 'selected')
                      .appendTo('.int-zoho-map-type');

                    $('<option>')
                      .text('Contact')
                      .attr('value', window.parent.parent.Ts.ReferenceTypes.Contacts)
                      .appendTo('.int-zoho-map-type');

                    $('.int-zoho-map-type').combobox({
                        selected: function (e, ui) {
                            loadFields(element.find('.int-zoho-map-tsfield'), element.find('.int-zoho-map-type').val());
                        }
                    });

                    loadFields(element.find('.int-zoho-map-tsfield'), element.find('.int-zoho-map-type').val());
                }
                break;
        }
    }

    function loadFields(select, refType) {
        select.empty();
        window.parent.parent.Ts.Services.CustomFields.GetAllFields(refType, null, false, function (fields) {
            for (var i = 0; i < fields.length; i++) {
                $('<option>')
                  .text(fields[i].Name)
                  .data('field', fields[i])
                  .appendTo(select);
            }

            select.combobox();
            select.combobox('setValue', fields[0].Name);
        });
    }

    function loadMapFields(element, fields) {
        var account = element.find('.map-account').empty();
        var contact = element.find('.map-contact').empty();
        var ticket = element.find('.map-ticket').empty();
        var ticketType = element.find('.map-ticket-type').empty();

        if (fields.length > 0) {
            for (var i = 0; i < fields.length; i++) {
                switch (fields[i].CRMObjectName) {
                    case 'Account':
                        appendMappedField(account, fields[i]);
                        break;
                    case 'Contact':
                        appendMappedField(contact, fields[i]);
                        break;
                    case 'Ticket':
                        appendMappedField(ticket, fields[i]);
                        break;
                    case 'TicketType':
                        appendMappedField(ticketType, fields[i]);
                        break;
                }
            }
        }
    }

    function loadSlaLevels(panel) {
        var slaLevelsList = panel.find('.int-defaultSla');
        if (slaLevelsList == null) return;

        panel = $(panel);
        var item = panel.data('link');

        if (slaLevels.length > 0) {
            if (slaLevels.length > 1) {
                slaLevelsList.attr('disabled', false);
            }
            else {
                slaLevelsList.attr('disabled', 'disabled');
            }
            slaLevelsList.empty();
            for (var i = 0; i < slaLevels.length; i++) {
                var selected = '">';
                if (item != null && item.DefaultSlaLevelID != null && item.DefaultSlaLevelID == slaLevels[i].SlaLevelID) {
                    selected = '" selected="selected">';
                }
                slaLevelsList.append('<option value="' + slaLevels[i].SlaLevelID + selected + slaLevels[i].Name + '</option>');
            }
        }
        else {
            slaLevelsList.attr('disabled', 'disabled');
        }
    }

    function loadActionTypes(panel) {
        var actionTypesList = panel.find('.int-actionTypeToPush');
        if (actionTypesList == null) return;

        panel = $(panel);
        var item = panel.data('link');

        if (actionTypes.length > 0) {
            actionTypesList.empty();
            var nullSelected = '>';
            if (item != null && item.ActionTypeIDToPush == null) {
                nullSelected = ' selected="selected">';
            }
            actionTypesList.append('<option value=""' + nullSelected + 'All Types</option>');

            for (var i = 0; i < actionTypes.length; i++) {
                var selected = '">';
                if (item != null && item.ActionTypeIDToPush != null && item.ActionTypeIDToPush == actionTypes[i].ActionTypeID) {
                    selected = '" selected="selected">';
                }
                actionTypesList.append('<option value="' + actionTypes[i].ActionTypeID + selected + actionTypes[i].Name + '</option>');
            }
        }
        else {
            actionTypesList.attr('disabled', 'disabled');
        }
    }

    function loadOrganizationStatusesWithType(panel) {
        var ticketStatusList = panel.find('.int-crm-ticketStatusToExclude');
        if (ticketStatusList == null) return;

        panel = $(panel);
        var item = panel.data('link');

        if (ticketStatusList.length > 0) {
            ticketStatusList.empty();

            if (typeof organizationStatuses == "undefined") {
                window.parent.parent.Ts.Services.Tickets.GetTicketStatusesOrderedByTicketTypeName(function (result) {
                    organizationStatuses = result;

                    if (typeof ticketTypes == "undefined") {
                        ticketTypes = window.parent.parent.Ts.Cache.getTicketTypes();
                    }

                    for (var i = 0; i < organizationStatuses.length; i++) {
                        var selected = '">';
                        if (item != null && item.ExcludedTicketStatusUpdate != null) {
                            //replace RestrictedToTicketTypes to the right column name
                            var excludeTicketStatusArray = item.ExcludedTicketStatusUpdate.split(',');
                            var found = jQuery.inArray(organizationStatuses[i].TicketStatusID.toString(), excludeTicketStatusArray);

                            if (found > -1) {
                                selected = '" selected="selected">';
                            }
                        }

                        var statusName = organizationStatuses[i].Name;
                        var typeName = "";

                        for (var x = 0; x < ticketTypes.length; x++) {
                            if (organizationStatuses[i].TicketTypeID == ticketTypes[x].TicketTypeID) {
                                typeName = ticketTypes[x].Name;
                                break;
                            }
                        }

                        if (typeName != "") {
                            ticketStatusList.append('<option value="' + organizationStatuses[i].TicketStatusID + selected + typeName + ' - ' + statusName + '</option>');
                        }
                    }
                });
            } else {
                if (typeof ticketTypes == "undefined") {
                    ticketTypes = window.parent.parent.Ts.Cache.getTicketTypes();
                }

                for (var i = 0; i < organizationStatuses.length; i++) {
                    var selected = '">';
                    if (item != null && item.ExcludedTicketStatusUpdate != null) {
                        //replace RestrictedToTicketTypes to the right column name
                        var excludeTicketStatusArray = item.ExcludedTicketStatusUpdate.split(',');
                        var found = jQuery.inArray(organizationStatuses[i].TicketStatusID.toString(), excludeTicketStatusArray);

                        if (found > -1) {
                            selected = '" selected="selected">';
                        }
                    }

                    var statusName = organizationStatuses[i].Name;
                    var typeName = "";

                    for (var x = 0; x < ticketTypes.length; x++) {
                        if (organizationStatuses[i].TicketTypeID == ticketTypes[x].TicketTypeID) {
                            typeName = ticketTypes[x].Name;
                            break;
                        }
                    }

                    if (typeName != "") {
                        ticketStatusList.append('<option value="' + organizationStatuses[i].TicketStatusID + selected + typeName + ' - ' + statusName + '</option>');
                    }
                }
            }
        }
        else {
            ticketStatusList.attr('disabled', 'disabled');
        }
    }

    function loadTicketTypes(panel) {
        var ticketTypesList = panel.find('.int-ticketTypeToPush');
        if (ticketTypesList == null) return;

        panel = $(panel);
        var item = panel.data('link');

        if (ticketTypesList.length > 0) {
            ticketTypesList.empty();

            if (typeof ticketTypes == "undefined") {
                ticketTypes = window.parent.parent.Ts.Cache.getTicketTypes();
            }

            for (var i = 0; i < ticketTypes.length; i++) {
                var selected = '">';

                if (item != null && item.RestrictedToTicketTypes != null) {
                    var restrictedToTicketTypesArray = item.RestrictedToTicketTypes.split(',');
                    var found = jQuery.inArray(ticketTypes[i].TicketTypeID.toString(), restrictedToTicketTypesArray);

                    if (found > -1) {
                        selected = '" selected="selected">';
                    }
                }

                ticketTypesList.append('<option value="' + ticketTypes[i].TicketTypeID + selected + ticketTypes[i].Name + '</option>');
            }
        }
        else {
            ticketTypesList.attr('disabled', 'disabled');
        }
    }

    function appendMappedField(element, field) {
        var div = $('<div>')
          .addClass('map-fieldset')
          .data('field', field)
          .appendTo(element);

        $('<span>')
          .text(field.CRMFieldName)
          .appendTo(div);

        $('<span>')
          .text(' -> ')
          .appendTo(div);

        $('<span>')
          .text(field.TSFieldName)
          .appendTo(div);

        $('<span>')
          .addClass('ui-icon ui-icon-close')
          .click(function (e) {
              e.preventDefault();
              if (confirm('Are you sure you would like to delete this mapping?')) {
                  window.parent.parent.Ts.Services.Organizations.DeleteCrmLinkField(div.data('field').CRMFieldID, function () {
                      div.remove();
                      window.parent.parent.Ts.System.logAction('Admin Integration - CRM Mapping Deleted');
                  });
              }

          })
          .appendTo(div);
    }

    $('.map-add').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        var parent = $('.map-add').closest('.int-panel');
        var crmField = parent.find('.int-map-crmfield').val();
        if (crmField === '') { alert('Please enter a field value.'); return; }

        var tsField = parent.find('.int-map-tsfield option:selected').data('field');

        window.parent.parent.Ts.Services.Organizations.SaveCrmLinkField(
          parent.data('link').CRMLinkID,
          tsField.ID,
          tsField.IsCustom,
          crmField,
          parent.find('.int-map-type').val(),
          function (fields) {
              loadMapFields(parent, fields);
          }


        );
        window.parent.parent.Ts.System.logAction('Admin Integration - CRM Mapping Added');
        parent.find('.int-map-crmfield').val('');
    });

    $('.jira-map-add').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        var parent = $('.jira-map-add').closest('.int-panel');
        var crmField = parent.find('.int-jira-map-crmfield').val();
        if (crmField === '') { alert('Please enter a field value.'); return; }

        var tsField = parent.find('.int-jira-map-tsfield option:selected').data('field');

        window.parent.parent.Ts.Services.Organizations.SaveCrmLinkField(
          parent.data('link').CRMLinkID,
          tsField.ID,
          tsField.IsCustom,
          crmField,
          parent.find('.int-map-type').val(),
          function (fields) {
              loadMapFields(parent, fields);
          }


        );
        window.parent.parent.Ts.System.logAction('Admin Integration - Jira Mapping Added');
        parent.find('.int-jira-map-crmfield').val('');
	});

	$('.snow-map-add').click(function (e) {
		e.preventDefault();
		e.stopPropagation();
		var parent = $('.snow-map-add').closest('.int-panel');
		var crmField = parent.find('.int-snow-map-crmfield').val();
		if (crmField === '') { alert('Please enter a field value.'); return; }

		var tsField = parent.find('.int-snow-map-field option:selected').data('field');

		window.parent.parent.Ts.Services.Organizations.SaveCrmLinkField(
			parent.data('link').CRMLinkID,
			tsField.ID,
			tsField.IsCustom,
			crmField,
			17,
			function (fields) {
				loadMapFields(parent, fields);
			}


		);
		window.parent.parent.Ts.System.logAction('Admin Integration - ServiceNow Mapping Added');
		parent.find('.int-snow-map-crmfield').val('');
	});

    $('.tfs-map-add').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        var parent = $('.tfs-map-add').closest('.int-panel');
        var crmField = parent.find('.int-tfs-map-crmfield').val();
        if (crmField === '') { alert('Please enter a field value.'); return; }

        var tsField = parent.find('.int-tfs-map-tsfield option:selected').data('field');

        window.parent.parent.Ts.Services.Organizations.SaveCrmLinkField(
          parent.data('link').CRMLinkID,
          tsField.ID,
          tsField.IsCustom,
          crmField,
          parent.find('.int-map-type').val(),
          function (fields) {
              loadMapFields(parent, fields);
          }


        );
        window.parent.parent.Ts.System.logAction('Admin Integration - Tfs Mapping Added');
        parent.find('.int-tfs-map-crmfield').val('');
    });

    $('.zoho-map-add').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        var parent = $('.zoho-map-add').closest('.int-panel');
        var crmField = parent.find('.int-zoho-map-crmfield').val();
        if (crmField === '') { alert('Please enter a field value.'); return; }

        var tsField = parent.find('.int-zoho-map-tsfield option:selected').data('field');

        window.parent.parent.Ts.Services.Organizations.SaveCrmLinkField(
          parent.data('link').CRMLinkID,
          tsField.ID,
          tsField.IsCustom,
          crmField,
          parent.find('.int-zoho-map-type').val(),
          function (fields) {
              loadMapFields(parent, fields);
          }


        );
        window.parent.parent.Ts.System.logAction('Admin Integration - ZohoCRM Mapping Added');
        parent.find('.int-zoho-map-crmfield').val('');
    });

    $('.int-type').delegate('.int-save', 'click', function (e) {
        e.preventDefault();
        var parent = $(this).parents('.int-panel');

        parent.find('.int-message').addClass('ui-state-error').empty();

        var flag = false;
        if (parent.find('.int-crm-password-confirm').length > 0 && parent.find('.int-crm-password').val() != parent.find('.int-crm-password-confirm').val()) {
            parent.find('.int-message').append('<div>Your passwords do not match.</div>').show();
            flag = true;
        }

        if (parent.find('.int-crm-token-confirm').length > 0 && parent.find('.int-crm-token').val() != parent.find('.int-crm-token-confirm').val()) {
            parent.find('.int-message').append('<div>Your tokens do not match.</div>').show();
            flag = true;
        }

		var crmType = '';
		var type = parent.parents('.int-type');

		if (type.hasClass('batchbook')) crmType = 'Batchbook';
		else if (type.hasClass('highrise')) crmType = 'Highrise';
		else if (type.hasClass('jira')) crmType = 'Jira';
		else if (type.hasClass('mailchimp')) crmType = 'MailChimp';
		else if (type.hasClass('oracle')) crmType = 'Oracle';
		else if (type.hasClass('salesforce')) crmType = 'Salesforce';
		else if (type.hasClass('zohocrm')) crmType = 'ZohoCrm';
		else if (type.hasClass('zohoreports')) crmType = 'ZohoReports';
		else if (type.hasClass('hubspot')) crmType = 'HubSpot';
		else if (type.hasClass('slackwebhook')) crmType = 'SlackWebHook';
		else if (type.hasClass('tfs')) crmType = 'TFS';
		else if (type.hasClass('servicenow')) crmType = 'ServiceNow';

		//Only Jira has multiple instances
        var jiraInstanceName = "";
		if (parent.find('#InstanceName').length > 0 && crmType == 'Jira') {
            jiraInstanceName = parent.find('#InstanceName').val();

            if (jiraInstanceName == null || jiraInstanceName == "") {
                parent.find('.int-message').append('<div>Enter a Instance Name.</div>').show();
                flag = true;
            } else if (jiraInstanceName.trim().toLowerCase() == 'default' && !_isDefaultJiraInstance && !_allowDefaultNewInstanceForJira) {
                parent.find('.int-message').append('<div>Please use a different Instance Name, "Default" is a reserved Instance Name.</div>').show();
                flag = true;
            }
        }

        if (flag) return;

        parent.find('.int-action').hide();

        var crmToken = parent.find('.int-crm-token').val();

        if (typeof crmToken == "undefined") {
            crmToken = null
        }

        var defaultSlaLevelId = parent.find('.int-defaultSla').val();

        if (typeof defaultSlaLevelId == "undefined") {
            defaultSlaLevelId = null;
        }

        var useNetworkCredentials = parent.find('.int-crm-use-network-credentials').prop('checked');
        if (!useNetworkCredentials) {
            useNetworkCredentials = false;
        }
        var pullCasesAsTickets = parent.find('.int-crm-pull-cases-as-tickets').prop('checked');
        var pushTicketsAsCases = parent.find('.int-crm-push-tickets-as-cases').prop('checked');
        var pushTicketsAsComments = parent.find('.int-crm-push-tickets-as-account-comments').prop('checked');

        if (typeof pullCasesAsTickets == 'undefined') {
            pullCasesAsTickets = false;
        }
        if (typeof pushTicketsAsCases == 'undefined') {
            pushTicketsAsCases = false;
        }
        if (typeof pushTicketsAsComments == 'undefined') {
            pushTicketsAsComments = true;
        }

        var pullCustomerProducts = parent.find('.int-crm-pull-customer-products').prop('checked');

        if (typeof pullCustomerProducts == 'undefined') {
            pullCustomerProducts = false;
        }

        var actionTypeIDToPush = parent.find('.int-actionTypeToPush').val();

        if (typeof actionTypeIDToPush == 'undefined') {
            actionTypeIDToPush = null;
        }

        var hostName = parent.find('.int-crm-host').val();

        if (typeof hostName == 'undefined') {
            hostName = null;
        }

        if (crmType == 'ZohoReports') {
            var grid = parent.find('.int-zohoreports-grid').val();
            if (grid == 'US') {
                hostName = 'reportsapi.zoho.com';
            } else {
                hostName = 'reportsapi.zoho.eu';
            }
        }

        var defaultProject = parent.find('.int-crm-project').val();

        if (typeof defaultProject == 'undefined') {
            defaultProject = null;
        }

        var matchAccountsByName = parent.find('.int-crm-match-accounts-by-name').prop('checked');
        if (typeof matchAccountsByName == 'undefined') {
            matchAccountsByName = true;
        }

        var useSandBoxServer = parent.find('.int-crm-use-sandbox-server').prop('checked');
        if (typeof useSandBoxServer == 'undefined') {
            useSandBoxServer = false;
        }

        var alwaysUseDefaultProjectKey = parent.find('.int-crm-always-use-default-project-key').prop('checked');
        if (typeof alwaysUseDefaultProjectKey == 'undefined') {
            alwaysUseDefaultProjectKey = false;
        }

        var includeIssueNonRequired = parent.find('.int-crm-IncludeIssueNonRequired').prop('checked');
        if (typeof includeIssueNonRequired == 'undefined') {
            includeIssueNonRequired = false;
        }

        var updateTicketStatus;

        if (crmType == "Jira") {
            updateTicketStatus = parent.find('.int-crm-update-status').prop('checked');
        } else if (crmType == "TFS") {
            updateTicketStatus = parent.find('.int-tfs-update-status').prop('checked');
        } else if (crmType == "ServiceNow") {
            updateTicketStatus = parent.find('.int-snow-update-status').prop('checked');
        }

        if (typeof updateTicketStatus == 'undefined') {
            updateTicketStatus = false;
        }

        var excludedTicketStatuses = null;

        if (updateTicketStatus) {
            var exclusionTicketStatusList;

            if (crmType == "Jira") {
                exclusionTicketStatusList = parent.find('#exclusionTicketStatusList');
            } else if (crmType == "TFS") {
                exclusionTicketStatusList = parent.find('#tfsExclusionTicketStatusList');
			} else if (crmType == "ServiceNow") {
				exclusionTicketStatusList = parent.find('#snowExclusionTicketStatusList');
			}

            if (typeof exclusionTicketStatusList == 'undefined') {
                excludedTicketStatuses = null;
            }
            else {
                excludedTicketStatuses = '';

                for (var x = 0; x < exclusionTicketStatusList[0].length; x++) {
                    if (exclusionTicketStatusList[0][x].selected) {
                        if (excludedTicketStatuses != '') {
                            excludedTicketStatuses = excludedTicketStatuses + ','
                        }

                        excludedTicketStatuses = excludedTicketStatuses + exclusionTicketStatusList[0][x].value;
                    }
                }

                if (excludedTicketStatuses === null || excludedTicketStatuses == '') {
                    excludedTicketStatuses = null;
                }
            }
        }

        var useAllTicketTypes;

        if (crmType == "Jira") {
            useAllTicketTypes = parent.find('.int-crm-ticket-types').prop('checked');
        } else if (crmType == "TFS") {
            useAllTicketTypes = parent.find('.int-tfs-ticket-types').prop('checked');
		} else if (crmType == "ServiceNow") {
			useAllTicketTypes = parent.find('.int-snow-ticket-types').prop('checked');
		}

        if (typeof useAllTicketTypes == 'undefined') {
            useAllTicketTypes = true;
        }

        var restrictedToTicketTypes = null;
        if (useAllTicketTypes) {
            restrictedToTicketTypes = null;
        }
        else {
            var restrictedTicketTypesList;

            if (crmType == "Jira") {
                restrictedTicketTypesList = parent.find('#restrictedTicketTypesList');
            } else if (crmType == "TFS") {
                restrictedTicketTypesList = parent.find('#restrictedTicketTypesListTFS');
			} else if (crmType == "ServiceNow") {
				restrictedTicketTypesList = parent.find('#restrictedTicketTypesListSnow');
			}

            if (typeof restrictedTicketTypesList == 'undefined') {
                restrictedToTicketTypes = null;
            }
            else {
                restrictedToTicketTypes = '';

                for (var x = 0; x < restrictedTicketTypesList[0].length; x++) {
                    if (restrictedTicketTypesList[0][x].selected) {
                        if (restrictedToTicketTypes != '') {
                            restrictedToTicketTypes = restrictedToTicketTypes + ','
                        }

                        restrictedToTicketTypes = restrictedToTicketTypes + restrictedTicketTypesList[0][x].value;
                    }
                }

                if (restrictedToTicketTypes === null || restrictedToTicketTypes == '') {
                    parent.find('.int-crm-ticket-types').prop('checked', true);
                    parent.find('#restrictedTicketTypesList').hide();
                    restrictedToTicketTypes = null;
                }
            }
        }

        var updateTicketType = parent.find('.int-crm-update-type').prop('checked');
        if (typeof updateTicketType == 'undefined') {
            updateTicketType = true;
        }

		var linkID = parent.data('link') == undefined || (_isNewJiraInstance && crmType == 'Jira') ? -1 : parent.data('link').CRMLinkID;

		var webhookToken = parent.find('.int-crm-webhook').val();

		if (webhookToken == undefined) {
			webhookToken = null;
		}

        window.parent.parent.Ts.Services.Organizations.SaveCrmLink(
              linkID,
              parent.find('.int-crm-active').prop('checked'),
              crmType,
              (parent.find('.int-crm-password').length > 0 ? parent.find('.int-crm-password').val() : ''),
              crmToken,
              (parent.find('.int-crm-tag').length > 0 ? parent.find('.int-crm-tag').val() : ''),
              (parent.find('.int-crm-user').length > 0 ? parent.find('.int-crm-user').val() : ''),
              (parent.find('.int-crm-email').length > 0 ? parent.find('.int-crm-email').prop('checked') : false),
              (parent.find('.int-crm-portal').length > 0 ? parent.find('.int-crm-portal').prop('checked') : false),
              defaultSlaLevelId,
              pullCasesAsTickets,
              pushTicketsAsCases,
              pushTicketsAsComments,
              pullCustomerProducts,
              actionTypeIDToPush,
              hostName,
              defaultProject,
              updateTicketStatus,
              updateTicketType,
              matchAccountsByName,
              useSandBoxServer,
              alwaysUseDefaultProjectKey,
              includeIssueNonRequired,
              restrictedToTicketTypes,
              excludedTicketStatuses,
              jiraInstanceName,
              useNetworkCredentials,
			  webhookToken,
              function (result) {
                  parent.data('link', result).find('.int-message').removeClass('ui-state-error').html('Your information was saved.').show().delay(1000).fadeOut('slow');
                  loadMaps(parent);
                  ReLoadJiraInstances(parent);
                  window.parent.parent.Ts.System.logAction('Admin Integration - Settings Saved');
              },
              function () {
                  parent.find('.int-message').addClass('ui-state-error').html('<div>There was an error saving this information, please try again.<div>').show();
                  parent.find('.int-action').show().find('.int-save').addClass('ui-state-active');
              }
            );
    });

    $('.int-type').delegate('.int-cancel', 'click', function (e) {
        e.preventDefault();
        var panel = $(this).parents('.int-panel');
        panel.find('.int-action').hide();
        panel.find('.int-message').hide();
        loadPanel(panel);

        var type = $(this).parents('.int-type');

        if (type.hasClass('jira')) {
            if (_anyJiraInstance) {
                ReLoadJiraInstances(panel);
            } else {
                SetupInitialDefaultInstanceCreate();
            }
        }

        _isNewJiraInstance = false;
	});

	$('.int-type').delegate('.int-generate-token', 'click', function (e) {
		e.preventDefault();
		var panel = $(this).parents('.int-panel');
		var typePanel = $(this).parents('.int-type');
		var buttonObject = $(this);
		var type;

		if (typePanel.hasClass('servicenow')) {
			type = 'servicenow';
		}

		window.parent.parent.Ts.Services.Organizations.GenerateWebHookToken(type,
			function (result) {
				var tokenField = panel.find('.int-crm-webhook');
				tokenField.val(result);
				onButtonClick(buttonObject);
			},
			function () {
				parent.find('.int-message').addClass('ui-state-error').html('<div>There was an error generating the token, please try again.<div>').show();
				parent.find('.int-action').show().find('.int-save').addClass('ui-state-active');
			}
		);
	});

    var onChange = function (e) {
        var footer = $(this).parents('.int-panel').find('.int-footer');

        if (footer.length < 1) {
            footer = $('<div>').addClass('int-footer');
            $('<div>').addClass('int-message ui-helper-hidden ui-corner-all ui-state-active').appendTo(footer);
            var actions = $('<div>').addClass('int-action').appendTo(footer);

            $('<button>')
            .text('Save')
            .addClass('int-save ui-state-active')
            .appendTo(actions)
            .button();

            $('<button>')
            .text('Cancel')
            .addClass('int-cancel')
            .appendTo(actions)
            .button();
            footer.appendTo($(this).parents('.int-panel'));
        }
        else {
            footer.find('.int-action').show();
        }
    }

    $('.int-panel input').change(onChange).keydown(onChange);
    $('.int-defaultSla').change(onChange);
    $('.int-zohoreports-grid').change(onChange);
    $('.int-crm-ticketStatusToExclude').change(onChange);
    $('.int-actionTypeToPush').change(onChange);
    $('.int-content-center input').unbind('change').unbind('keydown');
    $('.int-ticketTypeToPush').change(onChange);

    $('.int-crm-update-status').click(function (e) {
        if ($(this).prop('checked')) {
            $('#exclusionTicketStatusList').show();
            $('#ticketStatusExceptionSpan').show();
        }
        else {
            $('#exclusionTicketStatusList').hide();
            $('#ticketStatusExceptionSpan').hide();
        }
    });

    $('.int-crm-ticket-types').click(function (e) {
        if ($(this).prop('checked')) {
            $('#restrictedTicketTypesList').hide();
        }
        else {
            $('#restrictedTicketTypesList').show();
        }
    });

    $('.int-crm-push-tickets-as-cases').click(function (e) {
        if ($('.int-crm-push-tickets-as-cases').prop('checked')) {
            $('.int-crm-push-tickets-as-account-comments').prop('checked', false);
        }
        else {
            //$('.int-crm-push-tickets-as-account-comments').prop('checked', true);
        }
    });

    $('.int-crm-push-tickets-as-account-comments').click(function (e) {
        if ($('.int-crm-push-tickets-as-account-comments').prop('checked')) {
            $('.int-crm-push-tickets-as-cases').prop('checked', false);
        }
        else {
            //$('.int-crm-push-tickets-as-cases').prop('checked', true);
        }
    });

    //Most of Jira-specific stuff
    var onNewInstanceClick = function (ui) {
        var footer = $(ui).parents('.int-panel').find('.int-footer');

        if (footer.length < 1) {
            footer = $('<div>').addClass('int-footer');
            $('<div>').addClass('int-message ui-helper-hidden ui-corner-all ui-state-active').appendTo(footer);
            var actions = $('<div>').addClass('int-action').appendTo(footer);

            $('<button>')
			.text('Save')
			.addClass('int-save ui-state-active')
			.appendTo(actions)
			.button();

            $('<button>')
			.text('Cancel')
			.addClass('int-cancel')
			.appendTo(actions)
			.button();
            footer.appendTo($(ui).parents('.int-panel'));
        }
        else {
            footer.find('.int-action').show();
        }
    }

    $("#NewInstance").click(function (e, ui) {
        loadPanelNewJiraInstance();
        _isNewJiraInstance = true;
        $(this).hide();
        $("#JiraInstacesListWrapper").hide();
        $("#AddingInstanceLabel").show();
        onNewInstanceClick(this);
    });

    function loadPanelNewJiraInstance() {
        var element = $('.jira .int-panel');
        element.find('.int-crm-instancename').val("");
        element.find('.int-crm-instancename').removeAttr('disabled');
        _isDefaultJiraInstance = false;

        element.find('.int-crm-host').val("");
        element.find('.int-crm-user').val("");
        element.find('.int-crm-password').val("");
        element.find('.int-crm-password-confirm').val("");
        element.find('.int-crm-project').val("");
        element.find('.int-crm-update-status').prop('checked', false);
        element.find('.int-crm-update-type').prop('checked', false);
        element.find('.int-crm-active').prop('checked', false);
        element.find('.int-crm-always-use-default-project-key').prop('checked', false);
        element.find('.int-crm-IncludeIssueNonRequired').prop('checked', false);
        element.find('.int-crm-ticket-types').prop('checked', true);
        $("#ticketStatusExceptionSpan").hide();
        $("#exclusionTicketStatusList option:selected").removeAttr("selected");
        $('#exclusionTicketStatusList').hide();
        $("#restrictedTicketTypesList option:selected").removeAttr("selected");
        $('#restrictedTicketTypesList').hide();
        $('.map-ticket').empty();
    }

    function loadJiraInstancesList(panel) {
        var instancesList = panel.find('.int-jiraInstances');
        if (instancesList == null) return;

        panel = $(panel);
        var item = panel.data('link');

        if (jiraInstances != null && jiraInstances.length > 0) {
            instancesList.empty();

            for (var i = 0; i < jiraInstances.length; i++) {
                var selected = '">';

                if (jiraInstances[i].CRMType.toLowerCase() == 'jira') {
                    if ((item != null && item.CRMLinkID > 0 && item.CRMLinkID == jiraInstances[i].CRMLinkID && item.CRMType == "Jira")
						|| (_selectedJiraInstance == "" && item.InstanceName == "Default")) {
                        selected = '" selected="selected">';
                        _isDefaultJiraInstance = item.InstanceName == "Default";
                        _selectedJiraInstance = item.CRMLinkID;
                    }

                    instancesList.append('<option value="' + jiraInstances[i].CRMLinkID + selected + jiraInstances[i].InstanceName + '</option>');
                    _allowDefaultNewInstanceForJira = false;
                    _anyJiraInstance = true;
                }
            }

			$('#JiraInstances').combobox({
                selected: function (e, ui) {
                    _selectedJiraInstance = $(this).val();

                    for (var i = 0; i < jiraInstances.length; i++) {
                        if (jiraInstances[i].CRMLinkID == _selectedJiraInstance) {
                            loadPanel($('.jira .int-panel').data('link', jiraInstances[i]));
                            loadMaps($('.jira .int-panel').data('link', jiraInstances[i]));
                        }
                    }
                }
            });
        }
        else {
            instancesList.attr('disabled', 'disabled');
        }
    }

    function ReLoadJiraInstances(panel) {
        window.parent.parent.Ts.Services.Organizations.GetCrmLinks(function (result) {
            jiraInstances = result;
            loadJiraInstancesList(panel);

            $("#JiraInstacesListWrapper").show();
            $("#AddingInstanceLabel").hide();
            $("#NewInstance").show();

            _isNewJiraInstance = false;
        });
    }

    function SetupInitialDefaultInstanceCreate() {
        loadPanelNewJiraInstance();
        var element = $('.jira .int-panel');
        element.find('.int-crm-instancename').val('Default');
        element.find('.int-crm-instancename').attr('disabled', 'disabled');
        _isNewJiraInstance = true;
        _allowDefaultNewInstanceForJira = true;
        $("#NewInstance").hide();
        $("#JiraInstacesListWrapper").hide();
        $("#AddingInstanceLabel").show();
        onNewInstanceClick(this);
    }

    //TFS
    $('.int-tfs-update-status').click(function (e) {
        if ($(this).prop('checked')) {
            $('#tfsExclusionTicketStatusList').show();
            $('#tfsTicketStatusExceptionSpan').show();
        }
        else {
            $('#tfsExclusionTicketStatusList').hide();
            $('#tfsTicketStatusExceptionSpan').hide();
        }
    });

    $('.int-tfs-ticket-types').click(function (e) {
        if ($(this).prop('checked')) {
            $('#restrictedTicketTypesListTFS').hide();
        }
        else {
            $('#restrictedTicketTypesListTFS').show();
        }
	});

	//ServiceNow
	$('.int-snow-update-status').click(function (e) {
		if ($(this).prop('checked')) {
			$('#snowExclusionTicketStatusList').show();
			$('#snowTicketStatusExceptionSpan').show();
		}
		else {
			$('#snowExclusionTicketStatusList').hide();
			$('#snowTicketStatusExceptionSpan').hide();
		}
	});

	$('.int-snow-ticket-types').click(function (e) {
		if ($(this).prop('checked')) {
			$('#restrictedTicketTypesListSnow').hide();
		}
		else {
			$('#restrictedTicketTypesListSnow').show();
		}
	});
};

AdminInt.prototype = {
    constructor: AdminInt,
    refresh: function () {

    }
};