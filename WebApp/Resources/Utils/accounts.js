var selectedOrgId = null;


$(document).ready(function() {
    $('.action-refresh').click(function(e) {
        e.preventDefault();
        var org = $('.org-info').data('o');
        showOrgInfo(org.OrganizationID);
    });
    var supportLoginDisabled = false;
    var organizationID = -1;
    $('#inputOrg').typeahead({
        remote: {
            url: '/Services/OrganizationService.asmx/AdminQueryOrganizations?parentID=1&query=%QUERY'
        }
    }).on('typeahead:selected', function(e, datum) {
        organizationID = datum.id;
        $('.users-box').show();
        $('.org-info').show();
        showOrgInfo(organizationID);
    });

    function showOrgInfo(orgID) {
        top.Ts.Services.Organizations.GetOrganization(orgID, function(org) {
            selectedOrgId = orgID;
            $('.org-info').data('o', org);
            $('.action-toggle-active').text(org.IsActive != true ? 'Enable' : 'Disable');
            $('#selectProduct').val(org.ProductType);

            function appendProperty(name, value) {
                var row = $('<tr>');
                row.append($('<td>').html('<strong>' + name + '</strong>'));
                row.append($('<td>').html(value));
                $('.org-props').append(row);
            }

            $('.org-props').empty();
            appendProperty('Organization ID', org.OrganizationID);
            appendProperty('Is Active', org.IsActive.toString());
            supportLoginDisabled = org.DisableSupportLogin;
            appendProperty('Is Support Login Disabled', org.DisableSupportLogin.toString());
            appendProperty('Date Created', org.DateCreated.toString());
            appendProperty('Inventory Enabled', '<a href="#" class="action-toggle-inventory">' + org.IsInventoryEnabled.toString() + '</a>');
            appendProperty('API Request Limit', org.APIRequestLimit);
            appendProperty('User Seats', org.UserSeats);
            appendProperty('Index Last Rebuilt', org.LastIndexRebuilt.toString() + ' <a href="#" class="action-index">Rebuild Indexes</a>');

            top.Ts.Services.Organizations.AdminGetUserCount(orgID, function(count) {
                appendProperty('Active Users', count);
            });
            top.Ts.Services.Organizations.AdminGetStorageUsed(orgID, function(size) {
                appendProperty('Storage Used', size + ' MB');
            });
            top.Ts.Services.Organizations.AdminGetEncryptData(orgID, function(value) {
                appendProperty('Use Encrypted Data', '<a href="#" class="action-toggle-encrypt">' + value.toString() + '</a>');
            });

            appendProperty('Use Watson', '<a href="#" class="action-toggle-watson">' + org.UseWatson.toString() + '</a>');
            showUsers(org);
        });
    }

    $('.org-props').on('click', '.action-toggle-inventory', function(e) {
        e.preventDefault();
        var org = $('.org-info').data('o');
        var item = $(this);
        top.Ts.Services.Organizations.AdminSetInventory(org.OrganizationID, !org.IsInventoryEnabled, function() {
            org.IsInventoryEnabled = !org.IsInventoryEnabled;
            item.text(org.IsInventoryEnabled.toString());
        });
    });

    $('.org-props').on('click', '.action-index', function(e) {
        e.preventDefault();
        var org = $('.org-info').data('o');
        top.Ts.Services.Organizations.AdminRebuildIndexes(org.OrganizationID, function() {
            alert("Indexes are rebuilding.");
        });
    });

    $('.org-props').on('click', '.action-toggle-watson', function(e) {
        e.preventDefault();
        var org = $('.org-info').data('o');
        var item = $(this);
        top.Ts.Services.Organizations.AdminSetWatson(org.OrganizationID, !org.UseWatson, function() {
            org.UseWatson = !org.UseWatson;
            item.text(org.UseWatson.toString());
        });
    });

    $('.org-props').on('click', '.action-toggle-RequireTwoFactor', function(e) {
        e.preventDefault();
        var org = $('.org-info').data('o');
        var item = $(this);
        top.Ts.Services.Organizations.AdminSetRequireTwoFactor(org.OrganizationID, !org.RequireTwoFactor, function() {
            org.RequireTwoFactor = !org.RequireTwoFactor;
            item.text(org.RequireTwoFactor.toString());
        });
    });

    $('.org-props').on('click', '.action-toggle-encrypt', function(e) {
        e.preventDefault();
        var org = $('.org-info').data('o');
        var item = $(this);
        top.Ts.Services.Organizations.AdminToggleEncryptData(org.OrganizationID, function(value) {
            item.text(value.toString());
        });

    });

    function showUsers(org) {
        top.Ts.Services.Users.AdminGetUsers(org.OrganizationID, function(users) {
            function appendUser(user) {
                var row = $('<tr>');
                row.append($('<td>').text(user.UserID));
                row.append($('<td>').html(user.FirstName + ' ' + user.LastName));
                row.append($('<td>').html('<a href="mailto:' + user.Email + '">' + user.Email + '</a>'));
                row.append($('<td>').text(user.LastLogin));
                var checkActive = $('<input />', {
                    type: 'checkbox'
                })
                if (user.IsActive == true) checkActive.prop('checked', true);
                checkActive.change(function(e) {
                    e.preventDefault();
                    var user = $(this).parents('tr').data('o');
                    top.Ts.Services.Users.AdminSetActive(user.UserID, checkActive.prop('checked'));

                });
                row.append($('<td>').append(checkActive));

                var checkAdmin = $('<input />', {
                    type: 'checkbox'
                })
                if (user.IsSystemAdmin == true) checkAdmin.prop('checked', true);
                checkAdmin.change(function(e) {
                    e.preventDefault();
                    var user = $(this).parents('tr').data('o');
                    top.Ts.Services.Users.AdminSetAdmin(user.UserID, checkAdmin.prop('checked'));
                });
                row.append($('<td>').append(checkAdmin));

                var checkBilling = $('<input />', {
                    type: 'checkbox'
                })
                if (user.IsFinanceAdmin == true) checkBilling.prop('checked', true);
                checkBilling.change(function(e) {
                    e.preventDefault();
                    var user = $(this).parents('tr').data('o');
                    top.Ts.Services.Users.AdminSetBilling(user.UserID, checkBilling.prop('checked'));
                });
                row.append($('<td>').append(checkBilling));

                var checkSession = $('<input />', {
                    type: 'checkbox'
                })
                checkSession.change(function(e) {
                    e.preventDefault();
                    var user = $(this).parents('tr').data('o');
                    top.Ts.Services.Users.SetSingleSessionEnforcement(user.UserID, checkSession.prop('checked'));
                });
                if (user.EnforceSingleSession == true) checkSession.prop('checked', true);
                row.append($('<td>').append(checkSession));

                if (supportLoginDisabled === false) {
                    var div = $('<div>');
                    var link = $("<a>").attr("href", "#").addClass("getlink").text("Get Login").click(function(e) {
                        e.preventDefault();
                        top.Ts.Services.Users.AdminGetUserLogin(user.UserID, function(token) {
                            link.parent().empty().text(top.Ts.System.AppDomain + "/Login.html?SupportToken=" + token);
                        });
                    });
                    div.append(link);
                    row.append(div);
                }

                row.data('o', user);

                if (user.UserID == org.PrimaryUserID) {
                    row.addClass('danger');
                } else if (user.IsSystemAdmin == true) {
                    row.addClass('warning');
                }
                $('.users-table tbody').append(row);
            }

            $('.users-table tbody').empty();

            for (var i = 0; i < users.length; i++) {
                appendUser(users[i]);
            }
        });
    }

    $('.action-toggle-active').click(function(e) {
        e.preventDefault();
        var org = $('.org-info').data('o');

        if (org.IsActive) {
            if (!confirm("Are you really sure you would like to DISABLE " + org.Name + "?")) return;
        } else {
            if (!confirm("Are you really sure you would like to ENABLE " + org.Name + "?")) return;
        }

        top.Ts.Services.Organizations.AdminEnable(org.OrganizationID, !org.IsActive, function() {
            showOrgInfo(org.OrganizationID);
        });
    });

    $('.action-rename').click(function(e) {
        e.preventDefault();
        var org = $('.org-info').data('o');
        var name = prompt("Rename company", org.Name);
        if (name != null && name != "") {
            top.Ts.Services.Organizations.AdminRenameCompany(org.OrganizationID, name, function() {
                showOrgInfo(org.OrganizationID);
            });
        }
    });

    $('.action-seats').click(function(e) {
        e.preventDefault();
        var org = $('.org-info').data('o');
        var count = prompt("Change seat count", org.UserSeats);
        if (count != null) {
            top.Ts.Services.Organizations.AdminUpdateSeats(org.OrganizationID, count, function() {
                showOrgInfo(org.OrganizationID);
            });
        }
    });

    $('.action-ticketnumber').click(function(e) {
        e.preventDefault();
        var org = $('.org-info').data('o');
        var num = prompt("What is the next ticket number", 10000);
        if (num != null) {
            top.Ts.Services.Organizations.AdminSetNextTicketNumber(org.OrganizationID, num, function() {
                showOrgInfo(org.OrganizationID);
            });
        }
    });

    $('.action-delete').click(function(e) {
        e.preventDefault();
        var org = $('.org-info').data('o');
        var pw = prompt("Enter your password.", "");
        if (pw != null) {
            top.Ts.Services.Organizations.AdminDeleteOrganization(org.OrganizationID, pw, function() {
                window.location = window.location;
            }, function() {
                alert("Invalid Password.");
            });
        }
    });

    $('.action-api').click(function(e) {
        e.preventDefault();
        var org = $('.org-info').data('o');
        var count = prompt("Change API request count", org.APIRequestLimit);
        if (count != null) {
            top.Ts.Services.Organizations.AdminSetApiCount(org.OrganizationID, count, function() {
                showOrgInfo(org.OrganizationID);
            });
        }
    });

    $('.action-portal').click(function(e) {
        e.preventDefault();
        var sendEmails = confirm("Would you like to send password emails?");
        if (!confirm("Are you sure you would like to turn all the contacts into portal users?")) return;
        var org = $('.org-info').data('o');
        top.Ts.Services.Organizations.AdminSetAllPortalUsers(org.OrganizationID, sendEmails, function() {
            alert("All portal users are now enabled.");
        });
    });

    $('.action-hub').click(function(e) {
        e.preventDefault();
        var sendEmails = confirm("Would you like to send password emails?");
        if (!confirm("Are you sure you would like to turn all the contacts into hub users?")) return;
        var org = $('.org-info').data('o');
        top.Ts.Services.Organizations.AdminSetAllHubUsers(org.OrganizationID, sendEmails, function() {
            alert("All hub users are now enabled.");
        });
    });

    $('#selectProduct').change(function(e) {
        e.preventDefault();
        top.Ts.Services.Organizations.AdminUpdateProductType(organizationID, $('#selectProduct').val(), function() {
            showOrgInfo(organizationID);
        });

    });

});
