/// <reference path="ts/ts.js" />
/// <reference path="ts/parent.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />


$(document).ready(function () {
    var _organizatinID = -1;
    var _isAdmin = parent.Ts.System.User.IsSystemAdmin && (_organizatinID != parent.Ts.System.User.OrganizationID);
    var _dateFormat;

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



    var defaultTab = parent.Ts.Utils.getQueryValue("open", window);
    var defaultOrg = parent.Ts.Utils.getQueryValue("organizationID", window);

    $(".maincontainer").on("keypress", "input", (function (evt) {
        //Deterime where our character code is coming from within the event
        var charCode = evt.charCode || evt.keyCode;
        if (charCode == 13) { //Enter key's keycode
            return false;
        }
    }));

    if (defaultTab) {
        $('#customerTabs a:first').tab('show');
        $('#customerTabs a:last').tab('show');
    }
    else
        {
        $('#customerTabs a:first').tab('show');
    }

    if (!parent.Ts.System.User.CanCreateContact && !_isAdmin) {
        $('#customerTabs a:last').hide();
        $('#customerTabs a:first').tab('show');
    }

    if (!parent.Ts.System.User.CanCreateCompany && !_isAdmin) {
        $('#customerTabs a:first').hide();
        $('#customerTabs a:last').tab('show');
    }

    if (defaultOrg) {
        parent.Ts.Services.Organizations.GetOrganization(defaultOrg, function (org) {
            $('#inputContactCompany').val(org.Name);
            $('#inputContactCompany').data('item', defaultOrg);
        });
    }

    // Disable changing active if they are not an api
    // Hide the API
    if (!_isAdmin) {
        $('#cbActive').prop('disabled', true);
        $('#trApi').hide();
    }

    // Can the user modify portal access
    if (_isAdmin && parent.Ts.System.Organization.HasPortalAccess) {
        $('#cbPortalActive').prop('disabled', true);
    }

    if (_organizatinID == parent.Ts.System.User.OrganizationID) {
        $('#trSupportUser').hide();
        $('#trSupportGroup').hide();
    } else {
        $('#trTimeZone').hide();
    }

    var execGetCompany = null;
    function getCompany(request, response) {
        if (execGetCompany) { execGetCompany._executor.abort(); }
        execGetCompany = parent.Ts.Services.Organizations.WCSearchOrganization(request.term, function (result) { response(result); });
    }

    $('#inputContactCompany').autocomplete({
        minLength: 2,
        source: getCompany,
        select: function (event, ui) {
            $(this)
            .data('item', ui.item.id)
            .removeClass('ui-autocomplete-loading')
        }
    });

    $('#cbActive').on('click', function () {
        $('#formInactive').toggle();
    });

    $('#inputPortalUser').on('click', function () {
        $('#contactEmailPortalUser').toggle();
        $('#contactEmailHubUser').toggle();
    });

    LoadTimeZones();
    LoadSlas();
    LoadUsers(_organizatinID);
    LoadGroups(_organizatinID);
    LoadCustomControls(parent.Ts.ReferenceTypes.Organizations);
    LoadCustomContactControls();

    if (parent.Ts.System.Organization.HasPortalAccess && parent.Ts.System.User.IsSystemAdmin)
    {
        $('#contactPortalUserPanel').show();
    }

    if (parent.Ts.System.Organization.ParentID == null)
    {
        $('#contactSysAdminPanel').show();
        $('#contactFinanceAdminPanel').show();
        $('#contactPortalUserPanel').show();
        $('#contactActivePanel').show();
    }

    //$("#customerForm").validate({
    //    rules: {
    //        inputName: "required"
    //    },
    //    highlight: function (element) {
    //        $(element).closest('.col-md-4').addClass('has-error');
    //    },
    //    unhighlight: function (element) {
    //        $(element).closest('.col-md-4').removeClass('has-error');
    //    }
    //});
    //$("#contactForm").validate({
    //    rules: {
    //        inputContactFname: "required",
    //        inputContactEmail: {
    //            required: "#inputPortalUser:checked",
    //            minlength: 2
    //        }   
    //    },
    //    highlight: function (element) {
    //        $(element).closest('.col-md-4').addClass('has-error');
    //    },
    //    unhighlight: function (element) {
    //        $(element).closest('.col-md-4').removeClass('has-error');
    //    }
    //});

    $('#custSaveBtn').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        
        var isValid = $("#customerForm").valid();

        if ($("#inputName").val().length < 1)
        {
            alert("Please enter a name");
            return;
        }

        if(isValid)
        {
                $('#custSaveBtn').prop("disabled", true);
                var customerInfo = new Object();
                parent.Ts.System.logAction('New Customer Page - Added New Customer');
                customerInfo.Name = $("#inputName").val();
                customerInfo.Website = $("#inputWebSite").val();
                customerInfo.CompanyDomains = $("#inputDomains").val();
                customerInfo.DefaultSupportUserID = $("#ddlSUser").val();
                customerInfo.DefaultSupportGroupID = $("#ddlSGroup").val();
                customerInfo.TimeZoneID = $("#ddlTz").val();
                customerInfo.SAExpirationDate = convertToValidDate($("#inputSAE").val());
                customerInfo.SlaLevelID = $("#ddSla").val();
                customerInfo.SupportHoursMonth = $("#inputSupportHours").val();
                customerInfo.Active = $("#cbActive").prop('checked');
                customerInfo.PortalAccess = $("#cbPortalAccess").prop('checked');
                customerInfo.APIEnabled = $("#cbAPIEnabled").prop('checked');
                customerInfo.InactiveReason = $("#inactiveReason").val();
                customerInfo.Description = $("#Description").val();

                customerInfo.Fields = new Array();
                $('.customField:visible').each(function () {
                    var field = new Object();
                    field.CustomFieldID = $(this).attr("id");
                    switch ($(this).attr("type")) {
                        case "checkbox":
                            field.Value = $(this).prop('checked');
                            break;
                        case "_date":
                            //    var dt = $(this).find('input').datepicker('getDate');
                            field.Value = $(this).val() == "" ? null : parent.Ts.Utils.getMsDate(convertToValidDate($(this).val()));
                            break;
                        case "_time":
                            //    var time = new Date("January 1, 1970 00:00:00");
                            //    time.setHours($(this).find('input').timepicker('getDate')[0].value.substring(0, 2));
                            //    time.setMinutes($(this).find('input').timepicker('getDate')[0].value.substring(3, 5));
                            field.Value = $(this).val() == "" ? null : parent.Ts.Utils.getMsDate("1/1/1900 " + $(this).val());
                            break;
                        case "_datetime":
                            //    //field.Value = parent.Ts.Utils.getMsDate($(this).find('input').datetimepicker('getDate'));
                            //    var dt = $(this).find('input').datetimepicker('getDate');
                            //    field.Value = dt == null ? null : dt.toUTCString();
                            field.Value = $(this).val() == "" ? null : parent.Ts.Utils.getMsDate(convertToValidDateTime($(this).val()));
                            break;
                        default:
                            field.Value = $(this).val();
                    }
                    customerInfo.Fields[customerInfo.Fields.length] = field;
                });


                parent.Ts.Services.Customers.SaveCustomer(parent.JSON.stringify(customerInfo), function (f) {
                    $('#custSaveBtn').prop("disabled", false);
                    parent.Ts.MainPage.openNewCustomer(f);
                    parent.Ts.MainPage.closenewCustomerTab();
                }, function () {
                    $('#custSaveBtn').prop("disabled", false);
                    alert('There was an error saving this customer.  Please try again.');
                });
        }
    });
    $('#contactCancelBtn, #custCancelBtn').click(function (e) {
        parent.Ts.MainPage.closenewCustomerTab();
    });

    $('#contactSaveBtn').click(function (e) {
        e.preventDefault();
        e.stopPropagation();

        var isValid = $("#contactForm").valid();

        if(isValid)
        {
            $('#contactSaveBtn').prop("disabled", true);
            var contactInfo = new Object();
            parent.Ts.System.logAction('New Customer Page - Added New Contact');
            contactInfo.FirstName = $("#inputContactFname").val();
            contactInfo.MiddleName = $("#inputContactMname").val();
            contactInfo.LastName = $("#inputContactLname").val();
            contactInfo.Title = $("#inputContactTitle").val();
            contactInfo.Email = $("#inputContactEmail").val();
            contactInfo.Company = $("#inputContactCompany").data('item');
            contactInfo.Active = $("#inputContactActive").prop('checked');
            contactInfo.BlockInboundEmail = $("#inputContactBlockEmail").prop('checked');
            contactInfo.BlockEmailFromCreatingOnly = $("#inputContactBlockEmailCreatingOnly").prop('checked');
            contactInfo.IsPortalUser = $("#inputPortalUser").prop('checked');
            contactInfo.IsSystemAdmin = $("#inputContactSysAdmin").prop('checked');
            contactInfo.IsFinanceAdmin = $("#inputContactFinancialAdmin").prop('checked');
            contactInfo.SyncAddress = $("#inputContactSyncAddress").prop('checked');
            contactInfo.SyncPhone = $("#inputContactSyncPhone").prop('checked');
            contactInfo.EmailPortalPW = $("#inputContactPortalEmail").prop('checked');
            contactInfo.EmailHubPW = $("#inputContactHubEmail").prop('checked');

            contactInfo.Fields = new Array();
            $('.customField:visible').each(function () {
                var field = new Object();
                field.CustomFieldID = $(this).attr("id");
                switch ($(this).attr("type")) {
                    case "checkbox":
                        field.Value = $(this).prop('checked');
                        break;
                    case "picklist":
                        field.Value = $(this).val();
                        break;
                    case "_date":
                    //    var dt = $(this).find('input').datepicker('getDate');
                        field.Value = $(this).val() == "" ? null : convertToValidDate($(this).val());
                        break;
                    case "_time":
                    //    var time = new Date("January 1, 1970 00:00:00");
                    //    time.setHours($(this).find('input').timepicker('getDate')[0].value.substring(0, 2));
                    //    time.setMinutes($(this).find('input').timepicker('getDate')[0].value.substring(3, 5));
                        field.Value = $(this).val() == "" ? null : parent.Ts.Utils.getMsDate("1/1/1900 " + $(this).val());
                        break;
                    case "_datetime":
                    //    //field.Value = parent.Ts.Utils.getMsDate($(this).find('input').datetimepicker('getDate'));
                    //    var dt = $(this).find('input').datetimepicker('getDate');
                    //    field.Value = dt == null ? null : dt.toUTCString();
                        field.Value = $(this).val() == "" ? null : convertToValidDateTime($(this).val());
                        break;

                    //case "date":
                    //    //field.Value = parent.Ts.Utils.getMsDate($(this).find('input').datetimepicker('getDate'));
                    //    //var dt = $(this).find('input').datetimepicker('getDate');
                    //    //field.Value = dt == null ? null : dt.toUTCString();
                    //    //field.Value = $(this).find('input').datetimepicker('getDate');
                    //    field.Value = $(this).val();
                    //    break;
                    default:
                        field.Value = $(this).val();
                }
                contactInfo.Fields[contactInfo.Fields.length] = field;
            });

            parent.Ts.Services.Customers.SaveContact(parent.JSON.stringify(contactInfo), function (f) {
                if (f == -1) {
                    alert("The email you have specified is already in use.  Please choose another email.");
                    $('#contactSaveBtn').prop("disabled", false);
                }
                else {
                    $('#contactSaveBtn').prop("disabled", false);
                    parent.Ts.MainPage.openNewContact(f);
                    parent.Ts.MainPage.closenewCustomerTab();
                }
                });
        }

    });

    function convertToValidDate(val) {
        var value = '';
        if (val == "")
            return value;

        if (_dateFormat.indexOf("M") != 0) {
            var dateArr = val.replace(/\./g, '/').replace(/-/g, '/').split('/');
            if (_dateFormat.indexOf("D") == 0)
                var day = dateArr[0];
            if (_dateFormat.indexOf("Y") == 0)
                var year = dateArr[0];
            if (_dateFormat.indexOf("M") == 3 || _dateFormat.indexOf("M") == 5)
                var month = dateArr[1];

            var timeSplit = dateArr[2].split(' ');
            if (_dateFormat.indexOf("Y") == 6)
                var year = timeSplit[0];
            else
                var day = timeSplit[0];

            var theTime = timeSplit[1];

            var formattedDate = month + "/" + day + "/" + year;
            value = parent.Ts.Utils.getMsDate(formattedDate);
            return formattedDate;
        }
        else
            return val;
    }

    function convertToValidDateTime(val) {
        var value = '';
        if (val == "")
            return value;

        if (_dateFormat.indexOf("M") != 0) {
            var dateArr = val.replace(/\./g, '/').replace(/-/g, '/').split('/');
            if (_dateFormat.indexOf("D") == 0)
                var day = dateArr[0];
            if (_dateFormat.indexOf("Y") == 0)
                var year = dateArr[0];
            if (_dateFormat.indexOf("M") == 3 || _dateFormat.indexOf("M") == 5)
                var month = dateArr[1];

            var timeSplit = dateArr[2].split(' ');
            if (_dateFormat.indexOf("Y") == 6)
                var year = timeSplit[0];
            else
                var day = timeSplit[0];

            var theTime = timeSplit[1] + " " + timeSplit[2];

            var formattedDate = month + "/" + day + "/" + year + " " + theTime;
            //value = parent.Ts.Utils.getMsDate(formattedDate) + " " + theTime;
            return formattedDate;
        }
        else
            return val;
    }

    function LoadSlas() {
        parent.Ts.Services.Customers.LoadSlas(function (sla) {
            for (var i = 0; i < sla.length; i++) {
                $('<option>').attr('value', sla[i].SlaLevelID).text(sla[i].Name).data('o', sla[i]).appendTo('#ddSla');
            }
        });
    }

    function LoadTimeZones() {
        parent.Ts.Services.Customers.LoadTimeZones(function (timeZones) {
            for (var i = 0; i < timeZones.length; i++) {
                $('<option>').attr('value', timeZones[i].Id).text(timeZones[i].DisplayName).data('o', timeZones[i]).appendTo('#ddlTz');
            }
        });
    }

    function LoadGroups(organizationID) {
        parent.Ts.Services.Customers.LoadGroups(function (groups) {
            for (var i = 0; i < groups.length; i++) {
                $('<option>').attr('value', groups[i].GroupID).text(groups[i].Name).data('o', groups[i]).appendTo('#ddlSGroup');
            }
        });
    }

    function LoadUsers(organizationID) {
        parent.Ts.Services.Customers.LoadUsers(function (users) {
            for (var i = 0; i < users.length; i++) {
                $('<option>').attr('value', users[i].UserID).text(users[i].FirstName + ' ' + users[i].LastName).data('o', users[i]).appendTo('#ddlSUser');
            }
        });
    }

    function LoadCustomControls(refType) {
        parent.Ts.Services.Customers.LoadCustomControls(refType,function (html) {
            $('#customerCustomInfo').append(html);
            parent.Ts.Services.Customers.GetDateFormat(false, function (dateformat) {
                //$('.datepicker').datepicker({ format: dateformat });
                //$('.datepicker').datetimepicker({ pickTime: false });
                _dateFormat = dateformat;
                $('.datepicker').attr("data-format", dateformat);
                $('.datetimepicker').attr("data-format", dateformat + " hh:mm a");

                $('.datepicker').datetimepicker({ pickTime: false });
                $('.timepicker').datetimepicker({ pickDate: false });
                $('.datetimepicker').datetimepicker({});
            });
        });
    }

    function LoadCustomContactControls() {
        parent.Ts.Services.Customers.LoadCustomContactControls(function (html) {
            if (html.length < 1)
            {
                $('#customInfoBox').hide();
            }
            else{
                $('#contactCustomInfo').append(html);
            }
            parent.Ts.Services.Customers.GetDateFormat(false, function (dateformat) {
                //$('.datepicker').datepicker({ format: dateformat });
                //$('.datepicker').datetimepicker({ pickTime: false });
                $('.datepicker').attr("data-format", dateformat);
                $('.datetimepicker').attr("data-format", dateformat + " hh:mm a");

                $('.datepicker').datetimepicker({ pickTime: false });
                $('.timepicker').datetimepicker({ pickDate: false });
                $('.datetimepicker').datetimepicker({});
            });
        });
    }



    $('.number').on('keydown', function (event) {
        // Allow only backspace and delete
        if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 190) {
            // let it happen, don't do anything
        }
        else {
            // Ensure that it is a number and stop the keypress
            if (event.keyCode < 48 || event.keyCode > 57) {
                event.preventDefault();
            }
        }
    });
});