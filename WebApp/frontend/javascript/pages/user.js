var userPage = null;
var testing = null;


$(document).ready(function() {
    userPage = new UserPage();
    userPage.refresh();
});

$(document).on('click', 'a.setting', function(e) {
    e.stopPropagation();
    e.preventDefault();
    var option = $(this);
    var key = option.attr('setting');
    var value = (option.text() == 'Yes') ? 1 : 0;
    var opposite = (value == 1) ? 0 : 1;
    var category = option.attr('category');
    window.parent.parent.Ts.Services.Users.UpdateSetting(key, opposite, category, function(r) {
        if (r === 'positive') {
            var replacement = (value == 1) ? 'No' : 'Yes';
            option.text(replacement);
        }
    });
});

function onShow() {
    userPage.refresh();
};

function DialogClosed(sender, args) {
    sender.remove_close(DialogClosed);
    //window.location = window.location;
    $('#userPhoto').attr("src", $('#userPhoto').attr("src") + "/" + (new Date().getTime()).toString());
}

function ShowDialog(wnd) {
    wnd.add_close(DialogClosed);
    wnd.show();
}

UserPage = function() {
    var _user = null;

    $('button').button();
    $('a').addClass('ui-state-default ts-link');

    $('.ts-section').hover(function(e) {
        e.preventDefault();
        $(this).find('.user-profile-edit').show();
    }, function(e) {
        e.preventDefault();
        $('.user-profile-edit').hide();
    });

    var tipTimer = null;
    var clueTipOptions = window.parent.parent.Ts.Utils.getClueTipOptions(tipTimer);

    $('body').delegate('.ts-icon-info', 'mouseout', function(e) {
        if (tipTimer != null) clearTimeout(tipTimer);
        tipTimer = setTimeout("$(document).trigger('hideCluetip');", 3000);
    });

    $('body').delegate('.cluetip', 'mouseover', function(e) {
        if (tipTimer != null) clearTimeout(tipTimer);
    });

    function userRightsToString(value) {
        switch (value) {
            case 0:
                return "All tickets";
                break;
            case 1:
                return "Only assigned tickets";
                break;
            case 2:
                return "Only assigned and user's groups";
                break;
            case 3:
                return "Only assigned and tickets associated with specific customers";
                break;
            default:
                return "";
        }
    }

    function userProductFamiliesRightsToString(value) {
        switch (value) {
            case 0:
                return "All product lines";
                break;
            case 1:
                return "Only specific product lines";
                break;
            default:
                return "";
        }
    }

    $('.ts-section .collapsable').prepend('<span class="ui-icon ui-icon-triangle-1-e">').click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        var icon = $(this).find('.ui-icon');
        if (icon.hasClass('ui-icon-triangle-1-e')) {
            icon.removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');
            $(this).next().hide();
        } else {
            icon.removeClass('ui-icon-triangle-1-s').addClass('ui-icon-triangle-1-e');
            $(this).next().show();
        }
    });

    var userID = window.parent.parent.Ts.Utils.getQueryValue("userid", window);
    var orgID;

    window.parent.parent.Ts.Services.Users.GetUser(userID, function(user) {
        _user = user;
        orgID = user.OrganizationID;

        $('.user-displayname').html(user.FirstName + ' ' + user.LastName);
        $('#userEmail').html('<a class=fleft href="mailto:' + user.Email + '">' + user.Email + '</a>');
        var email = $('#userEmail').parent().hover(function(e) {
            $('#editEmail').show();
        }, function() {
            $('#editEmail').hide();
        });

        $('#userTitle').html(user.Title == '' || user.Title == null ? 'None' : user.Title);
        $('#userTimeZone').html(user.timeZoneDisplay == null ? 'None' : user.timeZoneDisplay);
        $('#userFontFamily').html(user.FontFamilyDescription);
        $('#userFontSize').html(user.FontSizeDescription);
        $('#userRestrictFromEditingAnyActions').html((user.RestrictUserFromEditingAnyActions == true ? 'Yes' : 'No'));
        $('#userCanBulkMerge').html((user.CanBulkMerge == true ? 'Yes' : 'No'));
        $('#userDisablePublic').html((user.DisablePublic == true ? 'Yes' : 'No'));
        $('#userDisableExporting').html((user.DisableExporting == true ? 'Yes' : 'No'));
        $('#userAllowToEditAnyAction').html((user.AllowUserToEditAnyAction == true ? 'Yes' : 'No'));
        $('#userCanPinAction').html((user.UserCanPinAction == true ? 'Yes' : 'No'));
        $('#userTicketVisibility').html((user.ChangeTicketVisibility == true ? 'Yes' : 'No'));
        $('#userCommunityVisibility').html((user.CanChangeCommunityVisibility == true ? 'Yes' : 'No'));
        $('#userCanCreateCompany').html((user.CanCreateCompany == true ? 'Yes' : 'No'));
        $('#userCanEditCompany').html((user.CanEditCompany == true ? 'Yes' : 'No'));
        $('#userCanCreateContacts').html((user.CanCreateContact == true ? 'Yes' : 'No'));
        $('#userCanEditContacts').html((user.CanEditContact == true ? 'Yes' : 'No'));
        $('#userCanCreateAssets').html((user.CanCreateAsset == true ? 'Yes' : 'No'));
        $('#userCanEditAssets').html((user.CanEditAsset == true ? 'Yes' : 'No'));
        $('#userCanCreateProducts').html((user.CanCreateProducts == true ? 'Yes' : 'No'));
        $('#userCanEditProducts').html((user.CanEditProducts == true ? 'Yes' : 'No'));
        $('#userCanCreateVersions').html((user.CanCreateVersions == true ? 'Yes' : 'No'));
        $('#userCanEditVersions').html((user.CanEditVersions == true ? 'Yes' : 'No'));
        $('#userKBVisibility').html((user.ChangeKbVisibility == true ? 'Yes' : 'No'));
        $('#userTicketRights').html(userRightsToString(user.TicketRights)).data('o', user.TicketRights);
        $('#userProductFamiliesRights').html(userProductFamiliesRightsToString(user.ProductFamiliesRights)).data('o', user.ProductFamiliesRights);
        if (window.parent.parent.Ts.System.Organization.UseProductFamilies && window.parent.parent.Ts.System.User.IsSystemAdmin) {
            $('#userProductFamiliesRights').closest('.ui-helper-hidden').show();
            $('#divProductFamiliesContainer').toggle(user.ProductFamiliesRights == 1);
            window.parent.parent.Ts.Services.Users.GetUserProductFamilies(_user.UserID, appendProductFamilies);
        }
        $('#divCustomerContainer').toggle(user.TicketRights == 3 && isSysAdmin == true);
        $('#userLastLogin').text(user.LastLogin.toDateString());
        $('#userActive').text((user.IsActive == true ? 'Yes' : 'No'));
        $('#userEmailNotify').text((user.ReceiveTicketNotifications == true ? 'Yes' : 'No'));
        $('#userSubscribeTickets').text((user.SubscribeToNewTickets == true ? 'Yes' : 'No'));
        $('#userSubscribeActions').text((user.SubscribeToNewActions == true ? 'Yes' : 'No'));
        $('#userAutoSubscribe').text((user.DoNotAutoSubscribe == true ? 'Yes' : 'No'));
        $('#userGroupNotify').text((user.ReceiveAllGroupNotifications == true ? 'Yes' : 'No'));
        $('#userUnassignedGroupNotify').text((user.ReceiveUnassignedGroupEmails == true ? 'Yes' : 'No'));
        $('#userEmailAfterHours').text((user.OnlyEmailAfterHours == true ? 'Yes' : 'No'));
        $('#userDateFormat').text(user.CultureDisplay);
        $('#userSysAdmin').text((user.IsSystemAdmin == true ? 'Yes' : 'No'));
        $('#chatUser').text((user.IsChatUser == true ? 'Yes' : 'No'));
        $('#activatedOn').text(user.ActivatedOn.toDateString());
        $('#userInfo').html((user.UserInformation == '' ? 'No Additional Information' : user.UserInformation.replace(/\n\r?/g, '<br />')));
        if (user.verificationPhoneNumber !== null && user.verificationPhoneNumber !== "") $('#userTwoFactorCell').text(user.verificationPhoneNumber);

        if (user.LinkedIn == '') {
            $('#userWebsite').html('None');
        } else {
            $('#userWebsite').attr("href", user.LinkedIn).attr("target", "_blank").html(user.LinkedIn);
        }

        window.parent.parent.Ts.Services.Users.GetCustomValues(userID, function(customValues) {
            appendCustomValues(customValues);
        });

        window.parent.parent.Ts.Services.Users.PullSettings(userID, function(r) {
            if (r != 'negative' && r != 'nothing') {
                var data = JSON.parse(r);
                $.each(data.userSettings, function(trash, setting) {
                    var target = '[category=' + setting.Category + '][setting=' + setting.SettingKey + ']';
                    var text = (setting.SettingValue == 1) ? 'Yes' : 'No';
                    $(target).text(text);
                });
            }
        });

        $('#userWebsite').parent().parent().hover(function(e) {
            $('#editWebsite').show();
        }, function() {
            $('#editWebsite').hide();
        });

        if (user.MenuItems == null) {
            $('#divMenuItems input').each(function() {
                $(this).prop('checked', true);
            });
        } else {
            var menuItems = user.MenuItems.split(',');
            for (var i = 0; i < menuItems.length; i++) {
                $('#' + menuItems[i]).prop('checked', true);
            }
        }

        if (window.parent.parent.Ts.System.Organization.ProductType == window.parent.parent.Ts.ProductType.Express) {
            $('#mniCustomers').hide();
            $('#mniChat').hide();
            $('#mniWiki').hide();
            $('#mniWC2').hide();
        }

        if (window.parent.parent.Ts.System.Organization.ProductType == window.parent.parent.Ts.ProductType.Express || window.parent.parent.Ts.System.Organization.ProductType == window.parent.parent.Ts.ProductType.HelpDesk) {
            $('#mniProducts').hide();
            $('#mniTasks').hide();
            $('[category=notification][setting=tasks]').closest('.flexbox').hide();
        }

        if (user.IsSystemAdmin == false) {
            $('#mniAdmin').hide();
        }

        var pwText = window.parent.parent.Ts.System.User.UserID == userID && window.parent.parent.Ts.System.User.IsSystemAdmin && (userID > -1);
        $('#userPW').text((pwText == true ? 'Change Password' : 'Reset and Email Password'));

        window.parent.parent.Ts.Services.Users.GetUserPhoto(userID, function(att) {
            $('#userPhoto').attr("src", att);
        });
        window.parent.parent.Ts.Services.Users.GetUserSignature(userID, function(signature) {
            if (signature == '') {
                signature = 'None';
            }
            $('#userSignature').html(signature);
        });
        window.parent.parent.Ts.Services.Users.GetUserCustomers(_user.UserID, appendCustomers);
    });


    var canEdit = window.parent.parent.Ts.System.User.UserID == userID || window.parent.parent.Ts.System.User.IsSystemAdmin;
    var isSysAdmin = window.parent.parent.Ts.System.User.IsSystemAdmin;

    if (isSysAdmin == true) {
        $('#divMenuItems').show();
        $('#userTicketRights').closest('.user-name-value').show();
        var types = window.parent.parent.Ts.Cache.getTicketTypes();
        for (var i = 0; i < types.length; i++) {
            if (types[i].IsActive) {
                var input = $('<input>').attr('type', 'checkbox').attr('id', 'mniTicketType_' + types[i].TicketTypeID);
                var label = $('<label>').addClass('checkbox').text(types[i].Name).append(input);
                label.appendTo('#ulTicketTypes');
            }
        }
    }

    $('#divMenuItems input').click(function(e) {
        var list = "";
        $('#divMenuItems input:checked').each(function() {
            var id = $(this).attr('id');
            list = (list != "") ? list + "," + id : id;
        });
        window.parent.parent.Ts.Services.Users.SetMenuItems(userID, list, function() {
            window.parent.parent.Ts.System.logAction('User Info - Menu Items Changed');
        });
    });

    if (canEdit) {
        if (!isSysAdmin) {
            $('#userActive').removeClass('ui-state-default ts-link');
            $('#userActive').addClass('disabledlink');
            $('#userSysAdmin').removeClass('ui-state-default ts-link');
            $('#userSysAdmin').addClass('disabledlink');
            $('#chatUser').removeClass('ui-state-default ts-link');
            $('#chatUser').addClass('disabledlink');
            $('#userTicketVisibility').removeClass('ui-state-default ts-link');
            $('#userTicketVisibility').addClass('disabledlink');
            $('#userCommunityVisibility').removeClass('ui-state-default ts-link');
            $('#userCommunityVisibility').addClass('disabledlink');
            $('#userRestrictFromEditingAnyActions').removeClass('ui-state-default ts-link');
            $('#userRestrictFromEditingAnyActions').addClass('disabledlink');
            $('#userCanBulkMerge').removeClass('ui-state-default ts-link');
            $('#userCanBulkMerge').addClass('disabledlink');
            $('#userDisableExporting').removeClass('ui-state-default ts-link');
            $('#userDisableExporting').addClass('disabledlink');
            $('#userAllowToEditAnyAction').removeClass('ui-state-default ts-link');
            $('#userAllowToEditAnyAction').addClass('disabledlink');
            $('#userCanPinAction').removeClass('ui-state-default ts-link');
            $('#userCanPinAction').addClass('disabledlink');
            $('#userCanCreateCompany').removeClass('ui-state-default ts-link');
            $('#userCanCreateCompany').addClass('disabledlink');
            $('#userCanEditCompany').removeClass('ui-state-default ts-link');
            $('#userCanEditCompany').addClass('disabledlink');
            $('#userCanCreateContacts').removeClass('ui-state-default ts-link');
            $('#userCanCreateContacts').addClass('disabledlink');
            $('#userCanEditContacts').removeClass('ui-state-default ts-link');
            $('#userCanEditContacts').addClass('disabledlink');
            $('#userCanEditAssets').removeClass('ui-state-default ts-link');
            $('#userCanEditAssets').addClass('disabledlink');
            $('#userCanCreateAssets').removeClass('ui-state-default ts-link');
            $('#userCanCreateAssets').addClass('disabledlink');
            $('#userCanCreatProducts').removeClass('ui-state-default ts-link');
            $('#userCanCreatProducts').addClass('disabledlink');
            $('#userCanEditProducts').removeClass('ui-state-default ts-link');
            $('#userCanEditProducts').addClass('disabledlink');
            $('#userCanCreateVersiosn').removeClass('ui-state-default ts-link');
            $('#userCanCreateVersiosn').addClass('disabledlink');
            $('#userCanEditVersions').removeClass('ui-state-default ts-link');
            $('#userCanEditVersions').addClass('disabledlink');
            $('#userKBVisibility').removeClass('ui-state-default ts-link');
            $('#userKBVisibility').addClass('disabledlink');
        }

        if (canEdit) {
            $('#twoFactorDiv').show();

            $("#mobile-number").intlTelInput({
                defaultCountry: "US",
                //geoIpLookup: function (callback) {
                //  $.get('http://ipinfo.io', function () { }, "jsonp").always(function (resp) {
                //    var countryCode = (resp && resp.country) ? resp.country : "";
                //    callback(countryCode);
                //  });
                //},
                utilsScript: "/frontend/library/utils.js" // just for formatting/placeholders etc
            });

            $('#userTwoFactorCell').click(function(e) {
                e.preventDefault();
                var header = $(this).parent().hide();
                $('#twoStepInputDiv').show();
                $('.intl-tel-input').css("float", "left");
            });

            $('#twoStepSave').click(function(e) {
                e.preventDefault();
                $(this).parent().show().find('img').show();
                var phoneNumb = $("#mobile-number").intlTelInput("getNumber");
                window.parent.parent.Ts.Services.Login.SetupVerificationPhoneNumber(userID, phoneNumb, false, function(result) {
                        $('#twoStepInputDiv').hide();
                        if (phoneNumb !== null && phoneNumb !== "") {
                            $('#userTwoFactorCell').text(phoneNumb).parent().show();
                        } else $('#userTwoFactorCell').parent().show();
                    },
                    function(error) {
                        alert('There was an error updating your record.  Please try again.');
                    });
            });

            $('#twoStepCancel').click(function(e) {
                e.preventDefault();
                $('#twoStepInputDiv').hide();
                $('#userTwoFactorCell').parent().show();
            });
        } else {
            $('#twoFactorDiv').remove();
        }

        $('.user-address-add').click(function(e) {
            e.preventDefault();
            e.stopPropagation();
            ShowDialog(parent.parent.GetAddressDialog(userID, 22));
            window.parent.parent.Ts.System.logAction('User Info - Address Dialog Opened');
        });

        $('.user-phone-add').click(function(e) {
            e.preventDefault();
            e.stopPropagation();
            ShowDialog(parent.parent.GetPhoneDialog(userID, 22));
            window.parent.parent.Ts.System.logAction('User Info - Phone Dialog Opened');
        });

        if (!isSysAdmin) {
            $('.user-group-add').remove();
        } else {
            $('.user-group-add').click(function(e) {
                e.preventDefault();
                e.stopPropagation();
                ShowDialog(parent.parent.GetSelectGroupDialog(userID, 22));
                window.parent.parent.Ts.System.logAction('User Info - Group Dialog Opened');
            });
        }

        $('#userPW').click(function(e) {
            e.preventDefault();
            var pwText = window.parent.parent.Ts.System.User.UserID == userID && window.parent.parent.Ts.System.User.IsSystemAdmin && (userID > -1);
            if (!pwText) {
                window.parent.parent.Ts.Services.Users.ResetEmailPW(_user.UserID, function(result) {
                        alert(result);
                        window.parent.parent.Ts.System.logAction('User Info - User Email Reset');
                    },
                    function(error) {
                        alert('There was an error.');
                    });
            } else {
                window.open("/vcr/1_9_0/pages/LoginNewPassword.html?UserID=" + _user.UserID, "ChangePW");
            }
        });

        $('#userPhotoEdit').click(function(e) {
            e.preventDefault();
            window.parent.parent.Ts.System.logAction('User Info - Photo Dialog Opened');
            ShowDialog(parent.parent.GetProfileImageDialog(orgID, userID));
        });

        $('#UserName').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var header = $(this).parent().hide();
            var container = $('<div>').addClass('user-name-edit').css('marginTop', '1em').insertAfter(header);

            $('<input type="text">').addClass('ui-widget-content ui-corner-all').val(_user.FirstName).appendTo(container).focus();
            $('<input type="text">').addClass('ui-widget-content ui-corner-all').val(_user.MiddleName).appendTo(container).focus();
            $('<input type="text">').addClass('ui-widget-content ui-corner-all').val(_user.LastName).appendTo(container).focus();

            $('<span>').addClass('fa fa-check-square').click(function(e) {
                $(this).closest('div').remove();
                header.show().find('img').show();
                var fname = $(this).prev().prev().prev().val();
                var mname = $(this).prev().prev().val();
                var lname = $(this).prev().val();

                if (fname == '' || lname == '') {
                    header.show().find('img').hide().next().show().delay(800).fadeOut(400);
                    alert('The name you have specified is invalid.  Please enter your name.');
                    return;
                }

                window.parent.parent.Ts.Services.Users.SaveUserName(_user.UserID, fname, mname, lname, function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Name Changed');
                        header.show().find('img').hide().next().show().delay(800).fadeOut(400);
                        $('#UserName').html(result);
                        _user.FirstName = fname;
                        _user.MidddleName = mname;
                        _user.LastName = lname;
                    },
                    function(error) {
                        header.show().find('img').hide();
                        alert('There was an error saving the username.');
                    });
            }).appendTo(container)

            $('<span>').addClass('fa fa-window-close').click(function(e) {
                $(this).closest('div').remove();
                header.show();
            }).appendTo(container)
        });

        $('#userInfo').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var header = $(this).parent().hide();
            $(this).find('.ts-icon-edit').hide();
            var container = $('<div>').addClass('user-info-edit').insertAfter(header);

            $('<textarea cols=50 rows=5>').addClass('ui-widget-content ui-corner-all').val($(this).parent().find('a').html().replace(/<br>/g, '\n')).appendTo(container).focus();

            $('<span>').addClass('fa fa-check-square').click(function(e) {
                $(this).closest('div').remove();
                header.show().find('img').show();
                window.parent.parent.Ts.Services.Users.SaveUserInfo(_user.UserID, $(this).prev().val(), function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Info Changed');
                        header.show().find('img').hide().next().show().delay(800).fadeOut(400);
                        if (result != '') {
                            $('#userInfo').html(result.replace(/\n\r?/g, '<br />'));
                        } else {
                            $('#userInfo').html('No Additional Information');
                        }
                    },
                    function(error) {
                        header.show().find('img').hide();
                        alert('There was an error saving the users information.');
                    });
            }).appendTo(container)

            $('<span>').addClass('fa fa-window-close').click(function(e) {
                $(this).closest('div').remove();
                header.show();
            }).appendTo(container)
        });

        $('#userTitle').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var header = $(this).parent().hide();
            var container = $('<div>').addClass('user-title-edit').insertAfter(header);

            $('<input type="text">').addClass('ui-widget-content ui-corner-all').val($(this).text()).appendTo(container).focus();

            $('<span>').addClass('fa fa-check-square').click(function(e) {
                $(this).closest('div').remove();
                header.show().find('img').show();
                window.parent.parent.Ts.Services.Users.SaveUserTitle(_user.UserID, $(this).prev().val(), function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Title Changed');
                        header.show().find('img').hide().next().show().delay(800).fadeOut(400);
                        $('#userTitle').html(result);
                    },
                    function(error) {
                        header.show().find('img').hide();
                        alert('There was an error saving the users title.');
                    });
            }).appendTo(container)

            $('<span>').addClass('fa fa-window-close').click(function(e) {
                $(this).closest('div').remove();
                header.show();
            }).appendTo(container)
        });

        $('#editEmail').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var header = $(this).parent().hide();
            $(this).find('.ts-icon-edit').hide();
            var container = $('<div>').addClass('user-email-edit').insertAfter(header);

            $('<input type="text">').addClass('ui-widget-content ui-corner-all').val($('#userEmail').text()).appendTo(container).focus();

            $('<span>').addClass('fa fa-check-square').click(function(e) {
                $(this).closest('div').remove();
                header.show().find('img').show();
                window.parent.parent.Ts.Services.Users.SaveUserEmail(_user.UserID, $(this).prev().val(), function(result) {
                        header.show().find('img').hide().next().show().delay(800).fadeOut(400);
                        if (result.substring(0, 6) == "_error") {
                            alert("The email you have specified is invalid.  Please choose another email.");
                        } else {
                            $('#userEmail').html('<a href="mailto:' + result + '">' + result + '</a>');
                            window.parent.parent.Ts.System.logAction('User Info - User Email Changed');
                        }
                    },
                    function(error) {
                        header.show().find('img').hide();
                        alert('There was an error saving the email.');
                    });
            }).appendTo(container)

            $('<span>').addClass('fa fa-window-close').click(function(e) {
                $(this).closest('div').remove();
                header.show();
            }).appendTo(container)
        });

        $('#editWebsite').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var header = $(this).parent().hide();
            $(this).find('.ts-icon-edit').hide();
            var container = $('<div>').addClass('user-linkedin-edit').insertAfter(header);

            $('<input type="text">').addClass('ui-widget-content ui-corner-all').val($('#userWebsite').text()).appendTo(container).focus();

            $('<span>').addClass('fa fa-check-square').click(function(e) {
                    $(this).closest('div').remove();
                    header.show().find('img').show();
                    window.parent.parent.Ts.Services.Users.SaveUserLinkedin(_user.UserID, $(this).prev().val(), function(result) {
                            window.parent.parent.Ts.System.logAction('User Info - User LinkedIn ID Changed');
                            header.show().find('img').hide().next().show().delay(800).fadeOut(400);
                            if (result.substring(0, 6) == "_error")
                                alert("The Website you have specified is invalid.");
                            else
                            if (result == '') {
                                $('#userWebsite').html('None');
                                $('#userWebsite').removeAttr("href", result);
                                $('#userWebsite').removeAttr("target", "_blank");
                            } else {
                                $('#userWebsite').html(result);
                                $('#userWebsite').attr("href", result);
                                $('#userWebsite').attr("target", "_blank");
                            }

                        },
                        function(error) {
                            header.show().find('img').hide();
                            alert('There was an error saving the Website url.');
                        });
                })
                .appendTo(container)

            $('<span>').addClass('fa fa-window-close').click(function(e) {
                $(this).closest('div').remove();
                header.show();
            }).appendTo(container)
        });

        $('#userActive').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);

            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetIsActive(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Active State Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                    },
                    function(error) {
                        alert('There was an error saving the user active status.');
                        item.next().hide();
                    });
            }
        });

        $('#userEmailNotify').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            item.next().show();
            window.parent.parent.Ts.Services.Users.SetEmailNotify(_user.UserID, (item.text() !== 'Yes'),
                function(result) {
                    window.parent.parent.Ts.System.logAction('User Info - User Email Notifications Changed');
                    item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                },
                function(error) {
                    alert('There was an error saving the user email notification status.');
                    item.next().hide();
                });
        });


        $('#userSubscribeTickets').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            item.next().show();
            window.parent.parent.Ts.Services.Users.SetSubscribeTickets(_user.UserID, (item.text() !== 'Yes'),
                function(result) {
                    window.parent.parent.Ts.System.logAction('User Info - User Ticket Subscription Setting Changed');
                    item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                },
                function(error) {
                    alert('There was an error saving the user ticket subscription status.');
                    item.next().hide();
                });
        });

        $('#userSubscribeActions').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            item.next().show();
            window.parent.parent.Ts.Services.Users.SetSubscribeActions(_user.UserID, (item.text() !== 'Yes'),
                function(result) {
                    window.parent.parent.Ts.System.logAction('User Info - User Action Subscription Changed');
                    item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                },
                function(error) {
                    alert('There was an error saving the user ticket action subscription status.');
                    item.next().hide();
                });
        });

        $('#userAutoSubscribe').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            item.next().show();
            window.parent.parent.Ts.Services.Users.SetAutoSubscribe(_user.UserID, (item.text() != 'Yes'), function(result) {
                    window.parent.parent.Ts.System.logAction('User Info - User Auto Subscribe Changed');
                    item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                },
                function(error) {
                    alert('There was an error saving the user auto subscribe status.');
                    item.next().hide();
                });
        });

        $('#userTicketPageVersion').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            item.next().show();
            window.parent.parent.Ts.Services.Users.SetUseClassicTicketPage(_user.UserID, (item.text() == 'No'),
                function(result) {
                    window.parent.parent.Ts.System.logAction('User Info - User Changed Ticket Page Version');
                    item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                },
                function(error) {
                    alert('There was an error saving the user auto subscribe status.');
                    item.next().hide();
                });
        });

        $('#userGroupNotify').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            item.next().show();
            window.parent.parent.Ts.Services.Users.SetGroupNotify(_user.UserID, (item.text() !== 'Yes'),
                function(result) {
                    window.parent.parent.Ts.System.logAction('User Info - Group Notification Changed');
                    item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                },
                function(error) {
                    alert('There was an error saving the user group notification subscription status.');
                    item.next().hide();
                });
        });

        $('#userUnassignedGroupNotify').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            item.next().show();
            window.parent.parent.Ts.Services.Users.SetUnassignedGroupNotify(_user.UserID, (item.text() !== 'Yes'),
                function(result) {
                    window.parent.parent.Ts.System.logAction('User Info - Group Unassigned Notification Changed');
                    item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                },
                function(error) {
                    alert('There was an error saving the user group unassigned notification subscription status.');
                    item.next().hide();
                });
        });

        $('#userEmailAfterHours').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            item.next().show();
            window.parent.parent.Ts.Services.Users.SetOnlyEmailAfterHours(_user.UserID, (item.text() !== 'Yes'),
                function(result) {
                    window.parent.parent.Ts.System.logAction('User Info - Only Email After Hours Changed');
                    item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                },
                function(error) {
                    alert('There was an error saving the Only Email After Hours status.');
                    item.next().hide();
                });
        });


        $('#userSysAdmin').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetSysAdmin(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User System Admin Status Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                    },
                    function(error) {
                        alert('There was an error saving the user system admin status.');
                        item.next().hide();
                    }
                );
            }
        });


        $('#userRestrictFromEditingAnyActions').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetRestrictUserFromEditingAnyActions(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Change Restrict User From Editing Any Actions Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                        if (result === true && $('#userAllowToEditAnyAction').text() == 'Yes') {
                            $('#userAllowToEditAnyAction').click();
                        }
                    },
                    function(error) {
                        alert('There was an error saving the user change restrict user from editing any actions.');
                        item.next().hide();
                    }
                );
            }
        });

        $('#userCanBulkMerge').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetChangeCanBulkMerge(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Change Can Bulk Merge Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                    },
                    function(error) {
                        alert('There was an error saving the user change can bulk merge.');
                        item.next().hide();
                    }
                );
            }
        });

        $('#userDisableExporting').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetRestrictUserFromExportingData(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Change Restrict User From Exporting Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                    },
                    function(error) {
                        alert('There was an error saving the user change restrict user from exporting data.');
                        item.next().hide();
                    }
                );
            }
        });

        $('#userDisablePublic').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetRestrictUserPublicActions(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Change Restrict User From Exporting Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                    },
                    function(error) {
                        alert('There was an error saving the user change restrict user from exporting data.');
                        item.next().hide();
                    }
                );
            }
        });

        $('#userAllowToEditAnyAction').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetAllowUserToEditAnyAction(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Change Allow User To Edit Any Action Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                        if (result === true && $('#userRestrictFromEditingAnyActions').text() == 'Yes') {
                            $('#userRestrictFromEditingAnyActions').click();
                        }
                    },
                    function(error) {
                        alert('There was an error saving the user change allow user to edit any action.');
                        item.next().hide();
                    }
                );
            }
        });

        $('#userCanPinAction').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetUserCanPinAction(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Can Pin Action Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                    },
                    function(error) {
                        alert('There was an error saving the user can pin action.');
                        item.next().hide();
                    }
                );
            }
        });

        $('#userTicketVisibility').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetChangeTicketVisibility(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Change Ticket Visibility Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                    },
                    function(error) {
                        alert('There was an error saving the user change ticket visibility.');
                        item.next().hide();
                    }
                );
            }
        });

        $('#userCommunityVisibility').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetChangeCommunityVisibility(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Change Ticket Community Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                    },
                    function(error) {
                        alert('There was an error saving the user change community visibility.');
                        item.next().hide();
                    }
                );
            }
        });

        $('#userCanCreateCompany').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetChangeCanCreateCompany(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Change Can Create Company Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                    },
                    function(error) {
                        alert('There was an error saving the user change to can create company.');
                        item.next().hide();
                    }
                );
            }
        });

        $('#userCanEditCompany').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetChangeCanEditCompany(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Change Can Edit Company Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                    },
                    function(error) {
                        alert('There was an error saving the user change to can edit company.');
                        item.next().hide();
                    }
                );
            }
        });

        $('#userCanCreateContacts').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetChangeCanCreateContacts(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Change Can Create Contacts Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                    },
                    function(error) {
                        alert('There was an error saving the user change to can create contacts.');
                        item.next().hide();
                    }
                );
            }
        });

        $('#userCanEditContacts').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetChangeCanEditContacts(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Change Can Edit Contacts Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                    },
                    function(error) {
                        alert('There was an error saving the user change to can edit contacts.');
                        item.next().hide();
                    }
                );
            }
        });


        $('#userCanCreateAssets').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetChangeCanCreateAssets(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Change Can Create Assets Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                    },
                    function(error) {
                        alert('There was an error saving the user change to can create assets.');
                        item.next().hide();
                    }
                );
            }
        });

        $('#userCanEditAssets').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetChangeCanEditAssets(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Change Can Edit Assets Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                    },
                    function(error) {
                        alert('There was an error saving the user change to can edit assets.');
                        item.next().hide();
                    }
                );
            }
        });


        $('#userCanCreateProducts').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetChangeCanCreateProducts(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Change Can Create Products Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                    },
                    function(error) {
                        alert('There was an error saving the user change to can create products.');
                        item.next().hide();
                    }
                );
            }
        });

        $('#userCanEditProducts').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetChangeCanEditProducts(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Change Can Edit Products Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                    },
                    function(error) {
                        alert('There was an error saving the user change to can edit products.');
                        item.next().hide();
                    }
                );
            }
        });

        $('#userCanCreateVersions').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetChangeCanCreateVersions(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Change Can Create Versions Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                    },
                    function(error) {
                        alert('There was an error saving the user change to can create versions.');
                        item.next().hide();
                    }
                );
            }
        });

        $('#userCanEditVersions').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetChangeCanEditVersions(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Change Can Edit Versions Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                    },
                    function(error) {
                        alert('There was an error saving the user change to can edit versions.');
                        item.next().hide();
                    }
                );
            }
        });

        $('#userKBVisibility').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetChangeKbVisibility(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        window.parent.parent.Ts.System.logAction('User Info - User Change Knowledgebase Visibility Changed');
                        item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                    },
                    function(error) {
                        alert('There was an error saving the user change knowledgebase visibility.');
                        item.next().hide();
                    }
                );
            }
        });

        $('#chatUser').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            var item = $(this);
            if (isSysAdmin) {
                item.next().show();
                window.parent.parent.Ts.Services.Users.SetChatUser(_user.UserID, (item.text() !== 'Yes'),
                    function(result) {
                        if (result == 'error') {
                            item.text('No').next().hide().next().show().delay(800).fadeOut(400);
                            alert("You have exceeded your chat licenses.  Please purchase more seats to add additional chat users.");
                        } else {
                            window.parent.parent.Ts.System.logAction('User Info - User Chat User Status Changed');
                            item.text((result == "True" ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
                        }
                    },
                    function(error) {
                        alert('There was an error saving the user chat status.');
                        item.next().hide();
                    }
                );
            }
        });

        $('#userProductFamiliesRights').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            e.stopPropagation();
            removeComboBoxes();
            var value = $(this).data('o');
            var parent = $(this).parent().hide();
            var container = $('<div>').addClass('ticket-combobox').insertAfter(parent);
            var select = $('<select>').appendTo(container);

            for (var i = 0; i < 2; i++) {
                var option = $('<option>')
                    .text(userProductFamiliesRightsToString(i))
                    .data('type', i)
                    .appendTo(select);

                if (value == i) {
                    option.attr('selected', 'selected');
                }
            }

            select.combobox({
                selected: function(e, ui) {
                    parent.show().find('img').show();
                    var type = $(ui.item).data('type');
                    window.parent.parent.Ts.Services.Users.SetProductFamiliesRights(_user.UserID, type, function() {
                            window.parent.parent.Ts.System.logAction('User Info - User Product Lines Rights Changed');
                            $('#userProductFamiliesRights').html(userProductFamiliesRightsToString(type)).data('o', type);
                            $('#divProductFamiliesContainer').toggle(type == 1);
                            parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
                        },
                        function(error) {
                            parent.show().find('img').hide();
                            alert('There was an error setting user product lines rights.');
                        });
                    container.remove();
                },
                close: function(e, ui) {
                    removeComboBoxes();
                }
            });
            select.combobox('search', '');
        });

        $('#userTicketRights').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            e.stopPropagation();
            removeComboBoxes();
            var value = $(this).data('o');
            var parent = $(this).parent().hide();
            var container = $('<div>').addClass('ticket-combobox').insertAfter(parent);
            var select = $('<select>').appendTo(container);

            for (var i = 0; i < 4; i++) {
                var option = $('<option>')
                    .text(userRightsToString(i))
                    .data('type', i)
                    .appendTo(select);

                if (value == i) {
                    option.attr('selected', 'selected');
                }
            }

            select.combobox({
                selected: function(e, ui) {
                    parent.show().find('img').show();
                    var type = $(ui.item).data('type');
                    window.parent.parent.Ts.Services.Users.SetTicketRights(_user.UserID, type, function() {
                            window.parent.parent.Ts.System.logAction('User Info - User Ticket Rights Changed');
                            $('#userTicketRights').html(userRightsToString(type)).data('o', type);
                            $('#divCustomerContainer').toggle(type == 3 && isSysAdmin == true);
                            parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
                        },
                        function(error) {
                            parent.show().find('img').hide();
                            alert('There was an error setting user rights.');
                        });
                    container.remove();
                },
                close: function(e, ui) {
                    removeComboBoxes();
                }
            });
            select.combobox('search', '');
        });

        $('#userTimeZone').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            e.stopPropagation();
            removeComboBoxes();
            var value = $(this).text();
            var parent = $(this).parent().hide();
            var container = $('<div>').addClass('ticket-combobox').insertAfter(parent);
            var select = $('<select>').appendTo(container);

            var tz = window.parent.parent.Ts.Cache.getTimeZones();
            for (var i = 0; i < tz.length; i += 2) {
                var option = $('<option>').text(tz[i]).data('type', tz[i + 1]).appendTo(select);
                if ($.trim(value) == $.trim(tz[i])) {
                    option.attr('selected', 'selected');
                }
            }

            select.combobox({
                selected: function(e, ui) {
                    parent.show().find('img').show();
                    var type = $(ui.item).data('type');
                    window.parent.parent.Ts.Services.Users.SetTimezone(_user.UserID, type, function(result) {
                            window.parent.parent.Ts.System.logAction('User Info - User Time Zone Changed');
                            if (result !== null) {
                                $('#userTimeZone').html(result);
                                parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
                            } else {
                                parent.show().find('img').hide();
                            }
                        },
                        function(error) {
                            parent.show().find('img').hide();
                            alert('There was an error setting user timezone.');
                        });
                    container.remove();
                },
                close: function(e, ui) {
                    removeComboBoxes();
                }
            });
            select.combobox('search', '');
        });

        $('#userFontFamily').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            e.stopPropagation();
            removeComboBoxes();
            var value = $(this).text();
            var parent = $(this).parent().hide();
            var container = $('<div>').addClass('ticket-combobox').insertAfter(parent);
            var select = $('<select>').appendTo(container);

            var families = window.parent.parent.Ts.Cache.getFontFamilies();
            for (var i = 0; i < families.length; i += 2) {
                var option = $('<option>').text(families[i]).data('type', families[i + 1]).appendTo(select);
                if ($.trim(value) == $.trim(families[i])) {
                    option.attr('selected', 'selected');
                }
            }

            select.combobox({
                selected: function(e, ui) {
                    parent.show().find('img').show();
                    var type = $(ui.item).data('type');
                    window.parent.parent.Ts.Services.Users.SetFontFamily(_user.UserID, type, function(result) {
                            window.parent.parent.Ts.System.logAction('User Info - User Font Family Changed');
                            if (result !== null) {
                                $('#userFontFamily').html(result);
                                parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
                            } else {
                                parent.show().find('img').hide();
                            }
                        },
                        function(error) {
                            parent.show().find('img').hide();
                            alert('There was an error setting user Font Family.');
                        });
                    container.remove();
                },
                close: function(e, ui) {
                    removeComboBoxes();
                }
            });
            select.combobox('search', '');
        });

        $('#userFontSize').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            e.stopPropagation();
            removeComboBoxes();
            var value = $(this).text();
            var parent = $(this).parent().hide();
            var container = $('<div>').addClass('ticket-combobox').insertAfter(parent);
            var select = $('<select>').appendTo(container);

            var sizes = window.parent.parent.Ts.Cache.getFontSizes();
            for (var i = 0; i < sizes.length; i += 2) {
                var option = $('<option>').text(sizes[i]).data('type', sizes[i + 1]).appendTo(select);
                if ($.trim(value) == $.trim(sizes[i])) {
                    option.attr('selected', 'selected');
                }
            }

            select.combobox({
                selected: function(e, ui) {
                    parent.show().find('img').show();
                    var type = $(ui.item).data('type');
                    window.parent.parent.Ts.Services.Users.SetFontSize(_user.UserID, type, function(result) {
                            window.parent.parent.Ts.System.logAction('User Info - User Font Size Changed');
                            if (result !== null) {
                                $('#userFontSize').html(result);
                                parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
                            } else {
                                parent.show().find('img').hide();
                            }
                        },
                        function(error) {
                            parent.show().find('img').hide();
                            alert('There was an error setting user Font Size.');
                        });
                    container.remove();
                },
                close: function(e, ui) {
                    removeComboBoxes();
                }
            });
            select.combobox('search', '');
        });

        $('#userDateFormat').after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>').click(function(e) {
            e.preventDefault();
            e.stopPropagation();
            removeComboBoxes();
            var value = $(this).text();
            var parent = $(this).parent().hide();
            var container = $('<div>').addClass('ticket-combobox').insertAfter(parent);
            var select = $('<select>').appendTo(container);

            var culture = window.parent.parent.Ts.Cache.getCultures();
            for (var i = 0; i < culture.length; i++) {
                var displayname = culture[i].split("_");
                var option = $('<option>').text(displayname[0]).appendTo(select).data('type', displayname[1]);
                if ($.trim(value) == $.trim(displayname[0])) {
                    option.attr('selected', 'selected');
                }
            }

            select.combobox({
                selected: function(e, ui) {
                    parent.show().find('img').show();
                    var type = $(ui.item).data('type');
                    window.parent.parent.Ts.Services.Users.SetCulture(_user.UserID, type, function(result) {
                            window.parent.parent.Ts.System.logAction('User Info - User Date Format Changed');
                            if (result !== null) {
                                $('#userDateFormat').html(result);
                                parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
                            } else {
                                parent.show().find('img').hide();
                            }
                        },
                        function(error) {
                            parent.show().find('img').hide();
                            alert('There was an error setting user date format.');
                        });
                    container.remove();
                },
                close: function(e, ui) {
                    removeComboBoxes();
                }
            });
            select.combobox('search', '');
        });

        $('#divActionForm').delegate('.signature-cancel', 'click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            $('#divActionForm').hide();
            $('#userSignature').show();
            $('#userSignatureEdit').show();

        });

        $('#divActionForm').delegate('.signature-save', 'click', function(e) {
            e.preventDefault();
            e.stopPropagation();

            window.parent.parent.Ts.Services.Users.SaveUserSignature(_user.UserID, $('.userSignatureText').tinymce().getContent(), function(result) {
                    if (result.substring(0, 6) == "_error")
                        alert("The signature you have specified is invalid.");
                    else {
                        $('#userSignature').html(result);
                        window.parent.parent.Ts.System.logAction('User Info - User Signature Changed');
                    }
                    $('#divActionForm').hide();
                    $('#userSignature').show();
                    $('#userSignatureEdit').show();
                },
                function(error) {
                    alert('There was an error saving the signature.');
                }
            );

        });

        $('#userSignatureEdit').click(function(e) {
            e.preventDefault();
            var element = $('#divActionForm');

            $(this).hide();
            $('#userSignature').hide();

            createTinyMCE($('<div>').appendTo('.userSignature-form'));

        });

        $('#userSignatureClear').click(function(e) {
            e.preventDefault();
            var element = $('#divActionForm');
            if (confirm("Are you sure you want to clear this signature?")) {
                window.parent.parent.Ts.Services.Users.ClearUserSignature(_user.UserID, function() {
                    $('#userSignature').empty();
                });
            }


        });

    } else {
        $('.ts-icon-edit').remove();
        $('.ts-add').remove();
        $('#userPhotoEdit').remove();
        $('#UserName').removeClass('ui-state-default ts-link');
        $('#UserName').addClass('disabledlink');
        $('#userTitle').removeClass('ui-state-default ts-link');
        $('#userTitle').addClass('disabledlink');
        $('#userTimeZone').removeClass('ui-state-default ts-link');
        $('#userTimeZone').addClass('disabledlink');
        $('#userDisableExporting').removeClass('ui-state-default ts-link');
        $('#userDisableExporting').addClass('disabledlink');
        $('#userRestrictFromEditingAnyActions').removeClass('ui-state-default ts-link');
        $('#userRestrictFromEditingAnyActions').addClass('disabledlink');
        $('#userCanBulkMerge').removeClass('ui-state-default ts-link');
        $('#userCanBulkMerge').addClass('disabledlink');
        $('#userAllowToEditAnyAction').removeClass('ui-state-default ts-link');
        $('#userAllowToEditAnyAction').addClass('disabledlink');
        $('#userTicketVisibility').removeClass('ui-state-default ts-link');
        $('#userTicketVisibility').addClass('disabledlink');
        $('#userCommunityVisibility').removeClass('ui-state-default ts-link');
        $('#userCommunityVisibility').addClass('disabledlink');
        $('#userKBVisibility').removeClass('ui-state-default ts-link');
        $('#userKBVisibility').addClass('disabledlink');
        $('#userLastLogin').removeClass('ui-state-default ts-link');
        $('#userLastLogin').addClass('disabledlink');
        $('#userActive').removeClass('ui-state-default ts-link');
        $('#userActive').addClass('disabledlink');
        $('#userEmailNotify').removeClass('ui-state-default ts-link');
        $('#userEmailNotify').addClass('disabledlink');
        $('#userSubscribeTickets').removeClass('ui-state-default ts-link');
        $('#userSubscribeTickets').addClass('disabledlink');
        $('#userSubscribeActions').removeClass('ui-state-default ts-link');
        $('#userSubscribeActions').addClass('disabledlink');
        $('#userAutoSubscribe').removeClass('ui-state-default ts-link');
        $('#userAutoSubscribe').addClass('disabledlink');
        $('#userGroupNotify').removeClass('ui-state-default ts-link');
        $('#userGroupNotify').addClass('disabledlink');
        $('#userEmailAfterHours').removeClass('ui-state-default ts-link');
        $('#userEmailAfterHours').addClass('disabledlink');
        $('#userDateFormat').removeClass('ui-state-default ts-link');
        $('#userDateFormat').addClass('disabledlink');
        $('#userDateFormat').removeClass('ui-state-default ts-link');
        $('#userDateFormat').addClass('disabledlink');
        $('#userSysAdmin').removeClass('ui-state-default ts-link');
        $('#userSysAdmin').addClass('disabledlink');
        $('#chatUser').removeClass('ui-state-default ts-link');
        $('#chatUser').addClass('disabledlink');
        $('#userInfo').removeClass('ui-state-default ts-link');
        $('#userInfo').addClass('disabledlink');
        $('#userSignatureEdit').remove();

        $('#customProperties a').removeClass('ui-state-default ts-link');
        $('#customProperties a').addClass('disabledlink');
    }

    GetAddresses();
    GetPhoneNumbers();
    GetGroups();


    function removeComboBoxes() {
        $('.ticket-combobox').prev().show().next().remove();
    }

    function GetAddresses() {
        window.parent.parent.Ts.Services.Users.GetUserAddresses(userID, function(addresses) {
            var addresslist = '';

            if (addresses.length === 0) {
                addresslist = '<div>There are no addresses to display.</div>';
            }

            for (var i = 0; i < addresses.length; i++) {
                var header = $('<div>').addClass('user-address')
                    .hover(function() {
                        $(this).find('.ts-icon').show();
                    }, function() {
                        $(this).find('.ts-icon').hide();
                    })
                    .data('data', addresses[i].AddressID);
                var desc = $('<span>')
                    .addClass('link-strong')
                    .text(addresses[i].Description)
                    .appendTo(header);
                if (canEdit) {
                    $('<span>')
                        .addClass('fleft ts-icon ts-icon-edit')
                        .click(function(e) {
                            var item = $(this).parent();
                            var data = item.data('data');
                            ShowDialog(parent.parent.GetAddressDialog(data));
                        }).hide()
                        .appendTo(header);


                    $('<span>')
                        .addClass('fleft ts-icon ts-icon-delete fa fa-trash')
                        .click(function(e) {
                            if (confirm('Are you sure you would like to remove this address?')) {
                                var item = $(this).parent();
                                var data = item.data('data');
                                parent.parent.privateServices.DeleteAddress(data);
                                window.location = window.location;
                            } else {
                                $(this).prev().hide();
                                $(this).hide();
                            }
                        }).hide()
                        .appendTo(header);
                }

                var addr1 = $('<div>').addClass('user-address-st1')
                    .text(addresses[i].Addr1)
                    .appendTo(header);

                var addr2 = $('<div>').addClass('user-address-st2')
                    .text(addresses[i].Addr2)
                    .appendTo(header);
                var addr3 = $('<div>').addClass('user-address-st3')
                    .text(addresses[i].Addr3)
                    .appendTo(header);
                var csz = $('<div>').addClass('user-address-csz')
                    .text(addresses[i].City + ', ' + addresses[i].State + ' ' + addresses[i].Zip)
                    .appendTo(header);
                var country = $('<div>').addClass('ser-address-country')
                    .text(addresses[i].Country)
                    .appendTo(header);
                $('#userAddressList').append(header);
            }

            $('#userAddressList').prepend(addresslist);
        });

    }

    function GetPhoneNumbers() {
        window.parent.parent.Ts.Services.Users.GetUserPhoneNumbers(userID, function(phones) {
            var phonelist = '';

            if (phones.length === 0) {
                phonelist = '<div>There are no phone numbers to display.</div>';
            }
            for (var i = 0; i < phones.length; i++) {
                var header = $('<div>').addClass('user-phone')
                    .hover(function() {
                        $(this).find('.ts-icon').show();
                    }, function() {
                        $(this).find('.ts-icon').hide();
                    })
                    .data('data', phones[i].PhoneID);

                var link = $('<span>')
                    .addClass('fleft')
                    .html('<strong>' + phones[i].PhoneTypeName + '</strong>: ' + phones[i].Number + '  ' + (phones[i].Extension == '' ? '' : '<strong>Ext:</strong> ' + phones[i].Extension))
                    .appendTo(header);
                if (canEdit) {
                    $('<span>')
                        .addClass('fleft ts-icon ts-icon-edit')
                        .click(function(e) {
                            var item = $(this).parent();
                            var data = item.data('data');
                            ShowDialog(parent.parent.GetPhoneDialog(data));
                        }).hide()
                        .appendTo(header);

                    $('<span>')
                        .addClass('fleft ts-icon ts-icon-delete fa fa-trash')
                        .click(function(e) {
                            if (confirm('Are you sure you would like to remove this phone number?')) {
                                var item = $(this).parent();
                                var data = item.data('data');
                                parent.parent.privateServices.DeletePhone(data);
                                window.location = window.location;
                            } else {
                                $(this).prev().hide();
                                $(this).hide();
                            }
                        }).hide()
                        .appendTo(header);
                }
                $('#userPhoneList').append(header);
            }
            $('#userPhoneList').prepend(phonelist);
        });
    }

    function GetGroups() {
        window.parent.parent.Ts.Services.Users.GetUserGroups(userID, function(groups) {
            var groupslist = '';
            if (groups.length === 0) {
                groupslist = '<div>There are no groups to display.</div>';
            }

            for (var i = 0; i < groups.length; i++) {
                var header = $('<div>').addClass('user-group')
                    .hover(function() {
                        $(this).find('.ts-icon').show();
                    }, function() {
                        $(this).find('.ts-icon').hide();
                    })
                    .data('data', groups[i].GroupID);
                var link = $('<span>')
                    .addClass('group-name')
                    .text(groups[i].Name)
                    .appendTo(header);
                if (isSysAdmin) {
                    $('<span>')
                        .addClass('fleft ts-icon ts-icon-delete fa fa-trash')
                        .click(function(e) {
                            if (confirm('Are you sure you would like to remove this group?')) {
                                var item = $(this).parent();
                                var data = item.data('data');
                                parent.parent.privateServices.DeleteGroupUser(data, _user.UserID);
                                window.location = window.location;
                            } else
                                $(this).hide();
                        }).hide()
                        .appendTo(header);
                }

                $('#userGroups').append(header);
            }
            $('#userGroups').prepend(groupslist);
        });
    }

    function initEditor(element, init) {
        var editorOptions = {
            plugins: "autoresize link code textcolor image moxiemanager",
            toolbar1: "image insertimage bold italic underline strikethrough bullist numlist fontselect fontsizeselect forecolor backcolor | link unlink | code",
            statusbar: false,
            branding: false,
            gecko_spellcheck: true,
            convert_urls: true,
            remove_script_host: false,
            relative_urls: false,
            content_css: "../Css/jquery-ui-latest.custom.css,../Css/editor.css",
            body_class: "ui-widget",
            template_external_list_url: "tinymce/jscripts/template_list.js",
            external_link_list_url: "tinymce/jscripts/link_list.js",
            external_image_list_url: "tinymce/jscripts/image_list.js",
            media_external_list_url: "tinymce/jscripts/media_list.js",
            menubar: false,
            moxiemanager_leftpanel: false,
            moxiemanager_fullscreen: false,
            moxiemanager_title: window.parent.parent.Ts.System.Organization.Name,
            moxiemanager_hidden_tools: (window.parent.parent.Ts.System.User.IsSystemAdmin == true) ? "" : "manage",
            paste_data_images: true,
            images_upload_url: "/Services/UserService.asmx/SaveTinyMCEPasteImage",

            setup: function(ed) {
                ed.on('init', function(e) {
                    window.parent.parent.Ts.System.refreshUser(function() {
                        if (window.parent.parent.Ts.System.User.FontFamilyDescription != "Unassigned") {
                            ed.execCommand("FontName", false, GetTinyMCEFontName(window.parent.parent.Ts.System.User.FontFamily));
                        } else if (window.parent.parent.Ts.System.Organization.FontFamilyDescription != "Unassigned") {
                            ed.execCommand("FontName", false, GetTinyMCEFontName(window.parent.parent.Ts.System.Organization.FontFamily));
                        }

                        if (window.parent.parent.Ts.System.User.FontSize != "0") {
                            ed.execCommand("FontSize", false, window.parent.parent.Ts.System.User.FontSizeDescription);
                        } else if (window.parent.parent.Ts.System.Organization.FontSize != "0") {
                            ed.execCommand("FontSize", false, window.parent.parent.Ts.System.Organization.FontSizeDescription);
                        }
                    });
                });
            },
            oninit: init
        };
        $(element).tinymce(editorOptions);
    }

    function createTinyMCE(element) {
        element = $(element).html($('#divActionForm').html()).addClass('fleft');

        initEditor(element.find('.userSignatureText'), function(ed) {
            window.parent.parent.Ts.Services.Users.GetUserSignature(userID, function(signature) {
                ed.setContent(signature);
            });
            //element.find('.userSignatureText').tinymce().focus();
        });

        element.find('.signature-cancel').click(function() {
            $('#userSignature').show();
            $('#userSignatureEdit').show();
            element.remove();
        });

        element.find('.signature-save').click(function() {
            window.parent.parent.Ts.Services.Users.SaveUserSignature(_user.UserID, element.find('.userSignatureText').html(), function(result) {
                    if (result.substring(0, 6) == "_error")
                        alert("The signature you have specified is invalid.");
                    else {
                        if (result == "")
                            $('#userSignature').html("None");
                        else
                            $('#userSignature').html(result);
                        $('#userSignature').show();
                        $('#userSignatureEdit').show();
                        element.remove();
                    }
                },
                function(error) {
                    alert('There was an error saving the signature.');
                });
        });

    }

    var execGetProductFamily = null;

    function getProductFamilies(request, response) {
        if (execGetProductFamily) {
            execGetProductFamily._executor.abort();
        }
        execGetProductFamily = window.parent.parent.Ts.Services.Products.SearchProductFamily(request.term, function(result) {
            response(result);
        });
    }

    $('#divAddProductFamily input')
        .autocomplete({
            minLength: 2,
            source: getProductFamilies,
            select: function(event, ui) {
                $(this)
                    .data('o', ui.item)
                    .removeClass('ui-autocomplete-loading');
            }
        })
        .keypress(function(e) {
            if (e.which == 13) $('#divAddProductFamily button').click();
        });

    $('#divAddProductFamily button').click(function(e) {
        e.preventDefault();
        var input = $('#divAddProductFamily input').val('');
        var item = input.data('o');
        if (!item) return;
        window.parent.parent.Ts.Services.Users.AddUserProductFamily(_user.UserID, item.id, appendProductFamilies, function() {
            alert('There was a problem adding the product line.');
        });
        input.removeData();
        window.parent.parent.Ts.System.logAction('User - Product Line Added');
    });

    $('#divProductFamilies').delegate('.removable-item .ui-icon-close', 'click', function(e) {
        e.preventDefault();
        e.stopPropagation();
        var item = $(this).parent();
        var data = item.data('data');
        window.parent.parent.Ts.Services.Users.RemoveUserProductFamily(_user.UserID, data.ProductFamilyID, appendProductFamilies, function() {
            alert('There was a problem removing the product line from the user.');
        });
        window.parent.parent.Ts.System.logAction('User - Product Line Removed');
    });

    function appendProductFamilies(productFamilies) {
        if (productFamilies == null) return;
        $('#divProductFamilies').empty();
        for (var i = 0; i < productFamilies.length; i++) {
            appendProductFamily(productFamilies[i]);
        }
    }

    function appendProductFamily(productFamily) {
        var item = $('<div>')
            .addClass('removable-item ui-corner-all ts-color-bg-accent ')
            .data('data', productFamily);

        $('<span>').addClass('ui-icon ui-icon-close').appendTo(item);
        var title = $('<div>').addClass('removable-item-title').appendTo(item);
        $('<a>')
            .attr('href', '#')
            .addClass('ui-state-default ts-link')
            .click(function(e) {
                e.preventDefault();
                window.parent.parent.Ts.MainPage.openNewProductFamily(productFamily.ProductFamilyID);
            })
            .text(window.parent.parent.Ts.Utils.ellipseString(productFamily.Name, 30))
            .appendTo(title);

        $('<span>')
            .addClass('ts-icon ts-icon-info')
            //.attr('rel', '../../../Tips/ProductFamily.aspx?ProductFamilyID=' + productFamily.ProductFamilyID)
            .cluetip(clueTipOptions)
            .appendTo(title);

        $('#divProductFamilies').append(item);
    }

    var execGetCustomer = null;

    function getCustomers(request, response) {
        if (execGetCustomer) {
            execGetCustomer._executor.abort();
        }
        execGetCustomer = window.parent.parent.Ts.Services.Organizations.SearchOrganization(request.term, function(result) {
            response(result);
        });
    }

    $('#divAddCustomer input')
        .autocomplete({
            minLength: 2,
            source: getCustomers,
            select: function(event, ui) {
                $(this)
                    .data('o', ui.item)
                    .removeClass('ui-autocomplete-loading');
            }
        })
        .keypress(function(e) {
            if (e.which == 13) $('#divAddCustomer button').click();
        });

    $('#divAddCustomer button').click(function(e) {
        e.preventDefault();
        var input = $('#divAddCustomer input').val('');
        var item = input.data('o');
        if (!item) return;
        window.parent.parent.Ts.Services.Users.AddUserCustomer(_user.UserID, item.id, appendCustomers, function() {
            alert('There was a problem adding the customer.');
        });
        input.removeData();
        window.parent.parent.Ts.System.logAction('User - Customer Added');
    });

    $('#divCustomers').delegate('.removable-item .ui-icon-close', 'click', function(e) {
        e.preventDefault();
        e.stopPropagation();
        var item = $(this).parent();
        var data = item.data('data');
        window.parent.parent.Ts.Services.Users.RemoveUserCustomer(_user.UserID, data.OrganizationID, appendCustomers, function() {
            alert('There was a problem removing the customer from the user.');
        });
        window.parent.parent.Ts.System.logAction('User - Customer Removed');
    });

    function appendCustomers(customers) {
        if (customers == null) return;
        $('#divCustomers').empty();
        for (var i = 0; i < customers.length; i++) {
            appendCustomer(customers[i]);
        }
    }

    function appendCustomer(customer) {
        var item = $('<div>')
            .addClass('removable-item ui-corner-all ts-color-bg-accent ')
            .data('data', customer);

        $('<span>').addClass('ui-icon ui-icon-close').appendTo(item);
        var title = $('<div>').addClass('removable-item-title').appendTo(item);
        $('<a>')
            .attr('href', '#')
            .addClass('ui-state-default ts-link')
            .click(function(e) {
                e.preventDefault();
                window.parent.parent.Ts.MainPage.openNewCustomer(customer.OrganizationID);
            })
            .text(window.parent.parent.Ts.Utils.ellipseString(customer.Name, 30))
            .appendTo(title);

        $('<span>')
            .addClass('ts-icon ts-icon-info')
            .attr('rel', '../../../Tips/Customer.aspx?CustomerID=' + customer.OrganizationID)
            .cluetip(clueTipOptions)
            .appendTo(title);

        $('#divCustomers').append(item);
    }

    $('.loading-section').hide().next().show();

    function appendCustomValues(fields) {
        if (fields === null || fields.length < 1) {
            $('.ticket-widget-properties').hide();
            return;
        }
        $('.ticket-widget-properties').show();
        var container = $('#customProperties').empty().removeClass('ts-loading');
        for (var i = 0; i < fields.length; i++) {
            var item = null;

            var field = fields[i];

            var div = $('<div>').addClass('user-name-value').css('height', '20px').data('field', field);
            $('<span>')
                .addClass('property')
                .text(field.Name + ': ')
                .appendTo(div);

            switch (field.FieldType) {
                case window.parent.parent.Ts.CustomFieldType.Text:
                    appendCustomEdit(field, div);
                    break;
                case window.parent.parent.Ts.CustomFieldType.Date:
                    appendCustomEditDate(field, div);
                    break;
                case window.parent.parent.Ts.CustomFieldType.Time:
                    appendCustomEditTime(field, div);
                    break;
                case window.parent.parent.Ts.CustomFieldType.DateTime:
                    appendCustomEditDateTime(field, div);
                    break;
                case window.parent.parent.Ts.CustomFieldType.Boolean:
                    appendCustomEditBool(field, div);
                    break;
                case window.parent.parent.Ts.CustomFieldType.Number:
                    appendCustomEditNumber(field, div);
                    break;
                case window.parent.parent.Ts.CustomFieldType.PickList:
                    appendCustomEditCombo(field, div);
                    break;
                default:
            }

            container.append(div);
        }
    }

    function appendCustomEditCombo(field, element) {
        var result = $('<a>')
            .attr('href', '#')
            .text((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : field.Value))
            .addClass('value ui-state-default ts-link')
            .appendTo(element)
            .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
            .click(function(e) {
                e.preventDefault();
                if (canEdit) {
                    $('.ticket-cutstom-edit').prev().show().next().remove();
                    $(this).hide()
                    var parent = $(this).parent();
                    var container = $('<div>')
                        .addClass('user-cutstom-edit ticket-combobox user-custom-edit')
                        .css('marginTop', '1em')
                        .insertAfter(parent);
                    var fieldValue = parent.closest('.user-name-value').data('field').Value;
                    var select = $('<select>').appendTo(container);

                    var items = field.ListValues.split('|');
                    for (var i = 0; i < items.length; i++) {
                        var option = $('<option>').text(items[i]).appendTo(select);
                        if (fieldValue === items[i]) {
                            option.attr('selected', 'selected');
                        }
                    }

                    select.combobox({
                        selected: function(e, ui) {
                            parent.show().find('img').show();
                            var value = $(this).val();
                            container.remove();

                            if (field.IsRequired && field.IsFirstIndexSelect == true && $(this).find('option:selected').index() < 1) {
                                result.parent().addClass('ui-state-error-custom ui-corner-all');
                            } else {
                                result.parent().removeClass('ui-state-error-custom ui-corner-all');
                            }
                            window.parent.parent.Ts.System.logAction('User - Custom Value Set');
                            window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _user.UserID, value, function(result) {
                                parent.find('img').hide().next().show().delay(800).fadeOut(400);
                                parent.closest('.user-name-value').data('field', result);
                                parent.find('a').text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value)).show();
                            }, function() {
                                alert("There was a problem saving your user property.");
                            });


                        },
                        close: function(e, ui) {
                            removeComboBoxes();
                        }
                    });
                    select.combobox('search', '');
                }
            });
        var items = field.ListValues.split('|');
        if (field.IsRequired && ((field.IsFirstIndexSelect == true && (items[0] == field.Value || field.Value == null || $.trim(field.Value) === '')) || (field.Value == null || $.trim(field.Value) === ''))) {
            result.parent().addClass('ui-state-error-custom ui-corner-all');
        }
    }

    function appendCustomEditNumber(field, element) {
        var result = $('<a>')
            .attr('href', '#')
            .text((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : field.Value))
            .addClass('value ui-state-default ts-link')
            .appendTo(element)
            .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
            .click(function(e) {
                e.preventDefault();
                if (canEdit) {
                    $('.user-custom-edit').prev().show().next().remove();
                    $(this).hide();
                    var parent = $(this).parent();
                    var container = $('<div>')
                        .addClass('user-cutstom-edit')
                        .css('marginTop', '1em')
                        .insertAfter($(this));

                    var fieldValue = parent.closest('.user-name-value').data('field').Value;
                    var input = $('<input type="text">')
                        .addClass('ui-widget-content ui-corner-all ticket-cutstom-edit-text-input')
                        .val(fieldValue)
                        .appendTo(container)
                        .numeric()
                        .focus();

                    $('<span>')
                        .addClass('fa fa-check-square')
                        .click(function(e) {
                            parent.show().find('img').show();
                            var value = input.val();
                            container.remove();
                            if (field.IsRequired && (value === null || $.trim(value) === '')) {
                                result.parent().addClass('ui-state-error-custom ui-corner-all');
                            } else {
                                result.parent().removeClass('ui-state-error-custom ui-corner-all');
                            }

                            window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _user.UserID, value, function(result) {
                                parent.find('img').hide().next().show().delay(800).fadeOut(400);
                                parent.closest('.user-name-value').data('field', result);
                                parent.find('a').text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value)).show();
                            }, function() {
                                alert("There was a problem saving your user property.");
                            });
                        })
                        .appendTo(container);

                    $('<span>')
                        .addClass('fa fa-window-close')
                        .click(function(e) {
                            parent.show();
                            parent.find('a').show();
                            container.remove();
                        })
                        .appendTo(container);
                }
            });
        if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
            result.parent().addClass('ui-state-error-custom ui-corner-all');
        }
    }

    function appendCustomEditBool(field, element) {
        var result = $('<a>')
            .attr('href', '#')
            .text((field.Value === null || $.trim(field.Value) === '' ? 'False' : field.Value))
            .addClass('value ui-state-default ts-link')
            .appendTo(element)
            .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
            .click(function(e) {
                e.preventDefault();
                if (canEdit) {
                    $('.ticket-cutstom-edit').prev().show().next().remove();

                    var parent = $(this).parent();
                    var value = $(this).text() === 'No' || $(this).text() === 'False' ? true : false;
                    parent.find('img').show();
                    window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _user.UserID, value, function(result) {
                        parent.find('img').hide().next().show().delay(800).fadeOut(400);
                        parent.closest('.user-name-value').data('field', result);
                        parent.find('a').text((result.Value === null || $.trim(result.Value) === '' ? 'False' : result.Value));
                    }, function() {
                        alert("There was a problem saving your user property.");
                    });
                }
            });
    }

    function appendCustomEdit(field, element) {
        var result = $('<a>')
            .attr('href', '#')
            .text((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : field.Value))
            .addClass('value ui-state-default ts-link')
            .appendTo(element)
            .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
            .click(function(e) {
                e.preventDefault();
                if (canEdit) {
                    $('.ticket-cutstom-edit').prev().show().next().remove();
                    $(this).hide();
                    var parent = $(this).parent();

                    var container = $('<div>')
                        .addClass('user-cutstom-edit')
                        .css('marginTop', '1em')
                        .insertAfter($(this));

                    var fieldValue = parent.closest('.user-name-value').data('field').Value;
                    var input = $('<input type="text">')
                        .addClass('ui-widget-content ui-corner-all ticket-cutstom-edit-text-input')
                        .val(fieldValue)
                        .appendTo(container)
                        .focus();

                    var fieldMask = parent.closest('.user-name-value').data('field').Mask;
                    if (fieldMask) {
                        input.mask(fieldMask);
                        input.attr("placeholder", fieldMask);
                    }

                    $('<span>')
                        .addClass('fa fa-check-square')
                        .click(function(e) {
                            parent.show().find('img').show();
                            var value = input.val();
                            container.remove();
                            if (field.IsRequired && (value === null || $.trim(value) === '')) {
                                result.parent().addClass('ui-state-error-custom ui-corner-all');
                            } else {
                                result.parent().removeClass('ui-state-error-custom ui-corner-all');
                            }
                            window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _user.UserID, value, function(result) {
                                parent.find('img').hide().next().show().delay(800).fadeOut(400);
                                parent.closest('.user-name-value').data('field', result);
                                parent.find('a').text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value)).show();
                            }, function() {
                                alert("There was a problem saving your user property.");
                            });
                        })
                        .appendTo(container);

                    $('<span>')
                        .addClass('fa fa-window-close')
                        .click(function(e) {
                            parent.show();
                            parent.find('a').show();
                            container.remove();
                        })
                        .appendTo(container);
                }
            });
        if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
            result.parent().addClass('ui-state-error-custom ui-corner-all');
        }
    }

    function appendCustomEditDate(field, element) {
        var date = field.Value == null ? null : window.parent.parent.Ts.Utils.getMsDate(field.Value);
        var result = $('<a>')
            .attr('href', '#')
            .text((date === null ? 'Unassigned' : date.localeFormat(window.parent.parent.Ts.Utils.getDatePattern())))
            .addClass('value ui-state-default ts-link')
            .appendTo(element)
            .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
            .click(function(e) {
                e.preventDefault();
                if (canEdit) {
                    $('.user-cutstom-edit').prev().show().next().remove();
                    $(this).hide();
                    var parent = $(this).parent();

                    var container = $('<div>')
                        .addClass('user-cutstom-edit')
                        .css('marginTop', '1em')
                        .insertAfter($(this));

                    var fieldValue = parent.closest('.user-name-value').data('field').Value;

                    var input = $('<input type="text">')
                        .addClass('ui-widget-content ui-corner-all ticket-cutstom-edit-text-input')
                        .appendTo(container)
                        .datepicker()
                        //.datetimepicker('setDate', window.parent.parent.Ts.Utils.getMsDate(fieldValue))
                        .focus();

                    $('<span>')
                        .addClass('fa fa-check-square')
                        .click(function(e) {
                            parent.show().find('img').show();
                            var value = window.parent.parent.Ts.Utils.getMsDate(input.datepicker('getDate'));
                            container.remove();
                            if (field.IsRequired && (value === null || $.trim(value) === '')) {
                                result.parent().addClass('ui-state-error-custom ui-corner-all');
                            } else {
                                result.parent().removeClass('ui-state-error-custom ui-corner-all');
                            }
                            window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _user.UserID, value, function(result) {
                                parent.find('img').hide().next().show().delay(800).fadeOut(400);
                                parent.closest('.user-name-value').data('field', result);
                                var date = result.Value === null ? null : window.parent.parent.Ts.Utils.getMsDate(result.Value);
                                parent.find('a').text((date === null ? 'Unassigned' : date.localeFormat(window.parent.parent.Ts.Utils.getDatePattern()))).show();

                            }, function() {
                                alert("There was a problem saving your user property.");
                            });
                        })
                        .appendTo(container);

                    $('<span>')
                        .addClass('fa fa-window-close')
                        .click(function(e) {
                            parent.show();
                            parent.find('a').show();
                            container.remove();
                        })
                        .appendTo(container);
                }
            });
        if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
            result.parent().addClass('ui-state-error-custom ui-corner-all');
        }
    }

    function appendCustomEditTime(field, element) {
        var date = field.Value == null ? null : window.parent.parent.Ts.Utils.getMsDate(field.Value);
        var result = $('<a>')
            .attr('href', '#')
            .text((date === null ? 'Unassigned' : date.localeFormat(window.parent.parent.Ts.Utils.getTimePattern())))
            .addClass('value ui-state-default ts-link')
            .appendTo(element)
            .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
            .click(function(e) {
                e.preventDefault();
                if (canEdit) {
                    $('.user-cutstom-edit').prev().show().next().remove();
                    $(this).hide();
                    var parent = $(this).parent();

                    var container = $('<div>')
                        .addClass('user-cutstom-edit')
                        .css('marginTop', '1em')
                        .insertAfter($(this));

                    var fieldValue = parent.closest('.user-name-value').data('field').Value;

                    var input = $('<input type="text">')
                        .addClass('ui-widget-content ui-corner-all ticket-cutstom-edit-text-input')
                        .appendTo(container)
                        .timepicker()
                        .timepicker('setDate', window.parent.parent.Ts.Utils.getMsDate(fieldValue))
                        .focus();

                    $('<span>')
                        .addClass('fa fa-check-square')
                        .click(function(e) {
                            parent.show().find('img').show();
                            var time = new Date("January 1, 1970 00:00:00");
                            time.setHours(input.timepicker('getDate')[0].value.substring(0, 2));
                            time.setMinutes(input.timepicker('getDate')[0].value.substring(3, 5));
                            var value = window.parent.parent.Ts.Utils.getMsDate(time);
                            container.remove();
                            if (field.IsRequired && (value === null || $.trim(value) === '')) {
                                result.parent().addClass('ui-state-error-custom ui-corner-all');
                            } else {
                                result.parent().removeClass('ui-state-error-custom ui-corner-all');
                            }
                            window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _user.UserID, value, function(result) {
                                parent.find('img').hide().next().show().delay(800).fadeOut(400);
                                parent.closest('.user-name-value').data('field', result);
                                var date = result.Value === null ? null : window.parent.parent.Ts.Utils.getMsDate(result.Value);
                                parent.find('a').text((date === null ? 'Unassigned' : date.localeFormat(window.parent.parent.Ts.Utils.getTimePattern()))).show();

                            }, function() {
                                alert("There was a problem saving your user property.");
                            });
                        })
                        .appendTo(container);

                    $('<span>')
                        .addClass('fa fa-window-close')
                        .click(function(e) {
                            parent.show();
                            parent.find('a').show();
                            container.remove();
                        })
                        .appendTo(container);
                }
            });
        if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
            result.parent().addClass('ui-state-error-custom ui-corner-all');
        }
    }

    function appendCustomEditDateTime(field, element) {
        var date = field.Value == null ? null : window.parent.parent.Ts.Utils.getMsDate(field.Value);
        var result = $('<a>')
            .attr('href', '#')
            .text((date === null ? 'Unassigned' : date.localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern())))
            .addClass('value ui-state-default ts-link')
            .appendTo(element)
            .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
            .click(function(e) {
                e.preventDefault();
                if (canEdit) {
                    $('.user-cutstom-edit').prev().show().next().remove();
                    $(this).hide();
                    var parent = $(this).parent();

                    var container = $('<div>')
                        .addClass('user-cutstom-edit')
                        .css('marginTop', '1em')
                        .insertAfter($(this));

                    var fieldValue = parent.closest('.user-name-value').data('field').Value;

                    var input = $('<input type="text">')
                        .addClass('ui-widget-content ui-corner-all ticket-cutstom-edit-text-input')
                        .appendTo(container)
                        .datetimepicker()
                        .datetimepicker('setDate', window.parent.parent.Ts.Utils.getMsDate(fieldValue))
                        .focus();

                    $('<span>')
                        .addClass('fa fa-check-square')
                        .click(function(e) {
                            parent.show().find('img').show();
                            var value = window.parent.parent.Ts.Utils.getMsDate(input.datetimepicker('getDate'));
                            container.remove();
                            if (field.IsRequired && (value === null || $.trim(value) === '')) {
                                result.parent().addClass('ui-state-error-custom ui-corner-all');
                            } else {
                                result.parent().removeClass('ui-state-error-custom ui-corner-all');
                            }
                            window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _user.UserID, value, function(result) {
                                parent.find('img').hide().next().show().delay(800).fadeOut(400);
                                parent.closest('.user-name-value').data('field', result);
                                var date = result.Value === null ? null : window.parent.parent.Ts.Utils.getMsDate(result.Value);
                                parent.find('a').text((date === null ? 'Unassigned' : date.localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()))).show();

                            }, function() {
                                alert("There was a problem saving your user property.");
                            });
                        })
                        .appendTo(container);

                    $('<span>')
                        .addClass('fa fa-window-close')
                        .click(function(e) {
                            parent.show();
                            parent.find('a').show();
                            container.remove();
                        })
                        .appendTo(container);
                }
            });
        if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
            result.parent().addClass('ui-state-error-custom ui-corner-all');
        }
    }
};



UserPage.prototype = {
    constructor: UserPage,
    refresh: function() {

    }
};

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
