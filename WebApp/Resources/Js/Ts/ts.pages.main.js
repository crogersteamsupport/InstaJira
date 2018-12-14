/// <reference path="ts/ts.js" />
/// <reference path="ts/ts.services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="~/Default.aspx" />
/// <reference path="ts/ts.pages.tickets.js" />
/// <reference path="../noty/jquery.noty.js" />
window.name = "TSMain";

function getMainFrame(wnd) {
    if (!wnd) wnd = window;
    var result = wnd;
    var cnt = 0;
    while (!(result.Ts && result.Ts.Services)) {
        result = result.parent;
        cnt++;
        if (cnt > 5) return null;
    }
    return result;
}

var mainFrame = getMainFrame();
var pusherChatRequests = null;

Ts.Pages.Main = function () {
    this.MainLayout = null;
    this.MainTabs = null;
    this.MainMenu = null;
    this.WndScreenR = null;
};

Ts.Pages.Main.prototype = {
    constructor: Ts.Pages.Main,

    getCalcStyle: function () {
        var colorHeader = $('.main-footer').css('background-color');
        var colorBorder = $('.main-footer').css('border-top-color');
        var colorContent = $('.main-nav').css('background-color');
        colorContent = "transparent";
        return '<style type="text/css">.ui-layout-resizer { background-color: #33485e; border: 0px solid ' + colorHeader + '; }' +
            '.ui-layout-toggler { background-color: ' + colorHeader + ';}</style>'; // 'li.ts-menutree-item  div { border:1px solid ' + colorContent + ';}</style>';
    },

    init: function () {
        var self = this;
        var tmrChat = null;
        var chatInterval = 10000;
        var tmrStatus = setInterval(getUserStatusUpdate, 300000);
        var tmrFlash = setInterval(flashTitle, 1250);
        var lastChatMessageID = -1;
        var lastChatRequestID = -1;
        var refreshID = -1;
        var chatRequestCount = 0;
        var chatMessageCount = 0;
        var isDebug = false;
        var notifyNewWC = 0;

        $.pnotify.defaults.styling = "jqueryui";
        $.pnotify.defaults.history = false;

        mainFrame.Ts.Services.System.GetLatestWatercoolerCount(function (result) {
            notifyNewWC = result;
        });

        function loadUserStatus() {
            if (tmrChat) clearInterval(tmrChat);
            if (Ts.System.User.IsChatUser) {
                $('.menu-chatstatus').show();
                if (Ts.System.ChatUserSettings.IsAvailable) {
                    //tmrChat = setInterval(getChatUpdates, chatInterval);
                    setupChatRequestUpdates();
                    $('.main-status-chat').removeClass('ui-state-disabled');
                    $('#icon-chatstatus').attr('src', '/vcr/1_9_0/Images/icon-online.png');
                } else {
                    $('.main-status-chat').addClass('ui-state-disabled');
                    $('#icon-chatstatus').attr('src', '/vcr/1_9_0/Images/icon-offline.png');
                }
            } else {
                $('.menu-chatstatus').hide();

            }

            if (Ts.System.User.InOffice) {
                $('.main-status-online').switchClass('ts-icon-offline', 'ts-icon-online', 0);
                // $('.menu-officestatus .ts-icon').addClass('ts-icon-online-small');
                $('#icon-officestatus').attr('src', '/vcr/1_9_0/Images/icon-online.png');
            } else {
                $('.main-status-online').switchClass('ts-icon-online', 'ts-icon-offline', 0);
                // $('.menu-officestatus .ts-icon').addClass('ts-icon-offline-small');
                $('#icon-officestatus').attr('src', '/vcr/1_9_0/Images/icon-offline.png');
            }

            var status = Ts.System.User.InOfficeComment === '' ? 'What is your status?' : Ts.System.User.InOfficeComment;
            $('.main-header-status-text').text(status).siblings('input').val(status);
            $('.menu-status-text').val(Ts.System.User.InOfficeComment).data('o', Ts.System.User.InOfficeComment);
            $('.main-status-left').html('Logged in as: ' + Ts.System.User.FirstName + ' ' + Ts.System.User.LastName + ' - ' + Ts.System.Organization.Name);
        }

        $('head').append(this.getCalcStyle());
        $('.main-loading').hide();
        $('.main-container').show();
        $('button').button();

        $('.main-quick-panel a').hover(function () {
            $(this).addClass('ui-state-active');
        }, function () {
            $(this).removeClass('ui-state-active');
        });
        $('.main-link-newticket').click(function (e) {
            e.preventDefault();
            self.newTicket();
        });
        $.editLabel('.main-header-status-text', function (result) {
            Ts.Services.Users.UpdateUserStatusComment(result, function (user) {
                Ts.System.User = user;
            });
        }, 'What is your status?', true);
        $('.main-status-chat').click(function () {
            Ts.Services.Users.ToggleUserChatStatus(function (setting) {
                Ts.System.ChatUserSettings = setting;
                loadUserStatus();
            });
        });

        $('.menu-signout').click(function (e) {
            e.preventDefault();
            var result = true;
            var iframes = document.getElementsByTagName('iframe');
            for (var i = 0; i < iframes.length; i++) {
                try {
                    if (iframes[i].contentWindow.tinyMCE.activeEditor)
                        result = confirm("Are you sure you want to leave this page");
                } catch (e) { }
            }
            if (result) {
                window.isSignedOut = true;
                mainFrame.Ts.System.logAction('Main Page - Signed Out');
                mainFrame.Ts.System.signOut();
            }
        });

        $('.menu-help-support').click(function (e) {
            e.preventDefault();
            mainFrame.Ts.System.openSupport();
        });

        $('.menu-help-chat').click(function (e) {
            e.preventDefault();
            var url = 'https://app.teamsupport.com/Chat/ChatInit.aspx?uid=22bd89b8-5162-4509-8b0d-f209a0aa6ee9';
            var fname = mainFrame.Ts.System.User.FirstName;
            var lname = mainFrame.Ts.System.User.LastName;
            var email = mainFrame.Ts.System.User.Email;

            if (fname != null && fname != '') {
                url += '&fname=' + fname;
            }

            if (lname != null && lname != '') {
                url += '&lname=' + lname;
            }

            if (email != null && email != '') {
                url += '&email=' + email;
            }

            window.open(url, 'TSChat', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=450,height=500');
        });



        $('.main-header-menu li')
            .hover(function () {
                $(this).addClass('hover');
            }, function () {
                $('.main-header-menu li').removeClass('hover');
            })
            .click(function (e) {
                e.stopPropagation();
                if (menuBlur != null) clearTimeout(menuBlur);
            });

        $('.menu-popup').click(function (e) {
            e.stopPropagation();
        });

        $('.menu-chatstatus').click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            Ts.Services.Users.ToggleUserChatStatus(function (setting) {
                Ts.System.ChatUserSettings = setting;
                if (tmrChat) clearInterval(tmrChat);
                if (Ts.System.ChatUserSettings.IsAvailable) {
                    //tmrChat = setInterval(getChatUpdates, chatInterval);
                    setupChatRequestUpdates();
                    $('#icon-chatstatus').attr('src', '/vcr/1_9_0/Images/icon-online.png');
                } else {
                    turnOffChatRequestUpdates();
                    $('#icon-chatstatus').attr('src', '/vcr/1_9_0/Images/icon-offline.png');
                }

                var element = $('.main-tab-content-item:visible');
                var contentFrame = $(element).children('iframe')[0];
                if (contentFrame && contentFrame.contentWindow.LoadPusherAndSubscribe) {
                    contentFrame.contentWindow.LoadPusherAndSubscribe();
                }
            });
            mainFrame.Ts.System.logAction('Main Page - Chat Status Changed');
        });

        $('.menu-officestatus').click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            hidePopupMenus();
            $(this).addClass('selected ui-widget-content ui-corner-top').removeClass('hover');
            $('.menu-popup-officestatus').css('left', $(this).offset().left - 1).show().find('.menu-input').focus();

        });

        $('.menu-office-online').click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            Ts.Services.Users.UpdateUserStatus(true, function () {
                // $('.menu-officestatus .ts-icon').addClass('ts-icon-online-small').removeClass('ts-icon-offline-small');
                $('#icon-officestatus').attr('src', '/vcr/1_9_0/Images/icon-online.png');
                mainFrame.Ts.System.logAction('Main Page - Office Status Changed');
            });
            hidePopupMenus();
        });

        $('.menu-office-offline').click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            Ts.Services.Users.UpdateUserStatus(false, function () {
                // $('.menu-officestatus .ts-icon').addClass('ts-icon-offline-small').removeClass('ts-icon-online-small');
                $('#icon-officestatus').attr('src', '/vcr/1_9_0/Images/icon-offline.png');
                mainFrame.Ts.System.logAction('Main Page - Office Status Changed');
            });
            hidePopupMenus();
        });

        $('.menu-input-hidden').css('opacity', '0');

        $('.menu-popup-officestatus li').hover(function () {
            $(this).addClass('ui-state-hover');
        }, function () {
            $('.menu-popup-officestatus li').removeClass('ui-state-hover');
        });

        $('.menu-status-text').keypress(function () {
            $('.menu-office-status-action').show();
        });

        $('.menu-office-save').click(function (e) {
            e.preventDefault();
            $('.menu-office-status-action').hide();
            $('.menu-status-text').data('o', $('.menu-status-text').val())
            Ts.Services.Users.UpdateUserStatusComment($('.menu-status-text').val());
            mainFrame.Ts.System.logAction('Main Page - Office Comment Changed');
        });

        $('.menu-office-cancel').click(function (e) {
            e.preventDefault();
            $('.menu-status-text').val($('.menu-status-text').data('o'));
            $('.menu-office-status-action').hide();
        });

        $('.menu-help-docs').click(function () {
            mainFrame.Ts.System.logAction('Main Page - Help Docs Opened');
        });
        $('.menu-help-chat').click(function () {
            mainFrame.Ts.System.logAction('Main Page - Help Chat Opened');
        });
        $('.menu-help-support').click(function () {
            mainFrame.Ts.System.logAction('Main Page - Help Portal Opened');
        });

        $('.menu-help').click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            hidePopupMenus();
            $(this).addClass('selected ui-widget-content ui-corner-top').removeClass('hover');
            $('.menu-popup-help').show().css('left', $(this).offset().left + $(this).outerWidth() - $('.menu-popup-help').outerWidth() + 1).find('.menu-input').focus();
        });

        var menuBlur = null;
        $('.menu-input').blur(function () {
            try { // fix for dumb ie
                if ($('.main-header-menu:focus').length > 0) return;
            } catch (e) {

            }
            menuBlur = setTimeout(hidePopupMenus, 500);
        });

        $(document).click(function (e) {
            hidePopupMenus();
        });

        function hidePopupMenus() {
            try { // fix for dumb ie
                if ($('.main-header-menu:focus').length > 0) return;
            } catch (e) {

            }
            $('.menu-popup').hide();
            $('.main-header-menu li').removeClass('selected ui-widget-content ui-corner-top');
        }


        $('.old-header-links a').addClass('ui-state-default ui-corner-all').hover(function (e) {
            $(this).addClass('ui-state-hover');
        }, function (e) {
            $(this).removeClass('ui-state-hover');
        });
        $('.main-header-links li').addClass('ui-state-default ui-corner-all').hover(function (e) {
            $(this).addClass('ui-state-hover');
        }, function (e) {
            $(this).removeClass('ui-state-hover');
        });

        $('.main-status-online').click(function () {
            Ts.Services.Users.ToggleUserStatus(function (user) {
                Ts.System.User = user;
                loadUserStatus();
            });
        });


        getUserStatusUpdate();

        Ts.Services.System.GetChatInterval(function (result) {
            chatInterval = result;
            loadUserStatus();
            chatInterval = 10000;
        });


        $(".dialog-select-ticket").dialog({
            height: 150,
            width: 300,
            autoOpen: false,
            modal: true,
            buttons: {
                OK: function () {
                    $(this).dialog("close");
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            }
        });

        $(".dialog-select-activity").dialog({
            height: 150,
            width: 300,
            autoOpen: false,
            modal: true,
            buttons: {
                OK: function () {
                    $(this).dialog("close");
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            }
        });

        $(".dialog-select-wiki").dialog({
            height: 150,
            width: 300,
            autoOpen: false,
            modal: true,
            buttons: {
                OK: function () {
                    $(this).dialog("close");
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            }
        });


        $(".dialog-paste-image").dialog({
            height: 'auto',
            width: 800,
            autoOpen: false,
            modal: true,
            buttons: {
                OK: function () {
                    $(this).dialog("close");
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            },
            open: function () {
                cleardialog();
            }
        });

        var execSelectNote = null;

        function selectNote(request, response) {
            if (execSelectNote) {
                execSelectNote._executor.abort();
            }
            execSelectNote = mainFrame.Ts.Services.Customers.SearchNotes(request.term, function (result) {
                response(result);
            });
        }

        $(".dialog-select-activity input").autocomplete({
            minLength: 2,
            source: selectNote,
            select: function (event, ui) {
                $(this).data('item', ui.item).removeClass('ui-autocomplete-loading')
            },
            position: {
                my: "right top",
                at: "right bottom",
                collision: "fit flip"
            }
        });


        var execSelectTicket = null;

        function selectTicket(request, response) {
            if (execSelectTicket) {
                execSelectTicket._executor.abort();
            }
            var filter = $(this.element).data('filter');
            if (filter === undefined) {
                execSelectTicket = mainFrame.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) {
                    response(result);
                });
            } else {
                execSelectTicket = mainFrame.Ts.Services.Tickets.SearchTickets(request.term, filter, function (result) {
                    response(result);
                });
            }
        }

        $(".dialog-select-ticket input").autocomplete({
            minLength: 2,
            source: selectTicket,
            select: function (event, ui) {
                $(this).data('item', ui.item).removeClass('ui-autocomplete-loading')
            },
            position: {
                my: "right top",
                at: "right bottom",
                collision: "fit flip"
            }
        });

        var execSelectWiki = null;

        function selectWiki(request, response) {
            if (execSelectWiki) {
                execSelectWiki._executor.abort();
            }
            execSelectWiki = mainFrame.Ts.Services.Wiki.SearchWikis(request.term, function (result) {
                response(result);
            });
        }

        $(".dialog-select-wiki input").autocomplete({
            minLength: 2,
            source: selectWiki,
            select: function (event, ui) {
                $(this).data('item', ui.item).removeClass('ui-autocomplete-loading')
            },
            position: {
                my: "right top",
                at: "right bottom",
                collision: "fit flip"
            }
        });

        function setupReminderDialog() {
            $(".dialog-reminder").dialog({
                height: 250,
                width: 300,
                autoOpen: false,
                modal: true,
                buttons: {
                    OK: function () {
                        $(this).dialog("close");
                    },
                    Cancel: function () {
                        $(this).dialog("close");
                    }
                }
            });

            $('.dialog-reminder .reminder-date').datetimepicker({
                "dateFormat": mainFrame.Ts.Utils.getJqueryDateFormat(mainFrame.Sys.CultureInfo.CurrentCulture.dateTimeFormat.ShortDatePattern)
            });
            $('.dialog-reminder .reminder-user').combobox();

        }

        setupReminderDialog();

        this.MainLayout = $('.main-container').layout({
            resizeNestedLayout: true,
            maskIframesOnResize: true,
            fxName: "slide",
            fxSpeed: "slow",
            center: {
                paneSelector: ".main-content"
            },
            north: {
                paneSelector: ".main-header",
                closable: false,
                resizable: false,
                size: 80,
                spacing_open: 1
            },
            south: {
                paneSelector: ".main-footer",
                closable: false,
                resizable: false,
                spacing_open: 0
            },
            //east: {
            //    paneSelector: ".main-info",
            //    size: 200,
            //    minSize: 100,
            //    maxSize: 300,
            //    initClosed: true,
            //    spacing_open: 3,
            //    onopen: function () {
            //        Ts.Services.Settings.WriteUserSetting('main-info-state', true);
            //    },
            //    onclose: function () {
            //        Ts.Services.Settings.WriteUserSetting('main-info-state', false);
            //        mainFrame.Ts.System.logAction('Main Page - Help Frame Hidden');
            //    }
            //},
            west: {
                paneSelector: ".main-nav",
                spacing_open: 2,
                minSize: 150,
                maxSize: 300,
                size: 175,
                closable: false

            }
        });

        function beforeMenuItemSelect(item) {
            var mi = self.MainMenu.getSelected();
            if (mi == null) return;

            var element = $('.main-tab-content-item-' + mi.getId()).children('iframe');
            if (element.length < 1) return;
            var contentFrame = element[0];
            try {
                if (contentFrame.contentWindow.onHide) contentFrame.contentWindow.onHide();
            } catch (err) { }

            if (mi.getType() == 'wc') {
                element.parent().remove();
            }
        }

        function processQuery() {
            var defaultMenuItem = 'mniWelcome';
            if ($('.menutree-item-welcome-mniWelcome').length < 1) defaultMenuItem = 'mniDashboard';

            Ts.Services.Settings.ReadUserSetting('main-menu-selected', defaultMenuItem, function (selectedID) {
                var selectedItem = self.MainMenu.getByID(selectedID);

                var ticketID = Ts.Utils.getQueryValue('ticketid');
                if (ticketID) {
                    self.openTicketByID(ticketID, true);
                }
                var ticketNumber = Ts.Utils.getQueryValue('ticketnumber');
                if (ticketNumber) {
                    self.openTicket(ticketNumber, true);
                }

                var contactID = Ts.Utils.getQueryValue('contactid');
                var customerID = Ts.Utils.getQueryValue('customerid');
                if (contactID) {
                    self.openNewContact(contactID);
                }
                else if (customerID) {
                    self.openNewCustomer(customerID);
                }

                var customerName = Ts.Utils.getQueryValue('customername');
                if (customerName) {
                    Ts.Services.Organizations.GetIDByName(customerName, function (result) {
                        self.openNewCustomer(result);
                    });
                }

                var phoneNumber = Ts.Utils.getQueryValue('phonenumber');
                if (phoneNumber) {
                    Ts.Services.Organizations.GetIDByPhone(phoneNumber, function (result) {
                        if (result[1] > -1) self.openNewContact(result[1], result[0]);
                        else if (result[0] > -1) self.openNewCustomer(result[0]);

                    });
                }

                var articleID = Ts.Utils.getQueryValue('articleid');
                if (articleID) {
                    self.openWiki(articleID);
                }

                var productID = Ts.Utils.getQueryValue('productid');
                var versionID = Ts.Utils.getQueryValue('versionid');
                if (versionID) {
                    self.openNewProductVersion(versionID);
                }
                else if (productID) {
                    self.openNewProduct(productID);
                }

                var organizationProductID = Ts.Utils.getQueryValue('organizationproductid');
                if (organizationProductID) {
                    self.openOrganizationProduct(organizationProductID);
                }

                var assetID = Ts.Utils.getQueryValue('assetid');
                if (assetID) {
                    self.openNewAsset(assetID);
                }

                var reportID = Ts.Utils.getQueryValue('reportID');
                if (reportID) {
                    self.openTicketView(reportID);
                }

                var taskID = Ts.Utils.getQueryValue('taskID');
                if (taskID) {
                    self.openNewTask(taskID);
                }

                if (selectedItem == null) {
                    self.MainMenu.getByIndex(0).select();
                } else {
                    if (window.location.search == "")
                        selectedItem.select();
                }

            });
        }


        function getUserStatusUpdate() {
            req = $.ajax({
                type: "POST",
                url: "/userstatus/" + Ts.System.getSessionID(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: callback,
                error: function (xhr, status, error) { }
            });

            function callback(result) {
                if (result != null) {
                    if (result.IsExpired == true) {
                        window.location = 'AnotherSession.aspx';
                    }

                    if (refreshID > -1 && result.RefreshID != refreshID)
                    {
                      window.location = '.';
                    }

                    if (isDebug != result.IsDebug) {
                        isDebug = result.IsDebug;
                        if (isDebug) {
                            $('.status-frame').show();
                            $('.status-expiration').show();
                            $('.status-debug').show();
                        } else {
                            $('.status-frame').hide();
                            $('.status-expiration').hide();
                            $('.status-debug').hide();
                        }
                    }

                    $('.status-expiration').text('Expires @ ' + result.ExpireTime);
                    $('.status-version').html('Version: ' + result.Version);
                    refreshID = result.RefreshID;

                    if (result.MyUnreadTicketCount > 0) {
                        $('.menutree-item-mytickets-mniMyTickets a').first().text('My Tickets (' + result.MyUnreadTicketCount + ')').css('font-weight', 'bold');
                    } else {
                        $('.menutree-item-mytickets-mniMyTickets a').first().text('My Tickets').css('font-weight', 'normal');
                    }
                }
            }


        }

        function getChatUpdates() {
            var params = { "lastChatMessageID": lastChatMessageID, "lastChatRequestID": lastChatRequestID };

            req = $.ajax({
                type: "POST",
                url: "/chatstatus",
                data: JSON.stringify(params),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: callback,
                error: function (xhr, status, error) { }
            });

            function callback(result)
            {
                var menuID = self.MainMenu.getSelected().getId();
                var isMain = mainTabs.find(0, Ts.Ui.Tabs.Tab.Type.Main).getIsSelected();
                if ((!isMain || menuID != 'mniChat') && (chatMessageCount < result.ChatMessageCount || chatRequestCount < result.ChatRequestCount)) {
                    self.MainMenu.find('mniChat', 'chat').setIsHighlighted(true);
                    for (var i = 0; i < result.NewChatMessages.length; i++) {
                        $("#jquery_jplayer_1").jPlayer("setMedia", { mp3: "vcr/1_9_0/Audio/drop.mp3" }).jPlayer("play", 0);

                        // Let's check if the browser supports notifications
                        if (!("Notification" in window)) {
                            $.jGrowl(result.NewChatMessages[i].Message, {
                                life: 5000,
                                theme: result.NewChatMessages[i].State,
                                header: result.NewChatMessages[i].Title
                            });
                        // Let's check whether notification permissions have already been granted
                        } else if (Notification.permission === "granted") {
                            ShowBrowserNotification(result.NewChatMessages[i].Message);
                        // Otherwise, we need to ask the user for permission
                        } else if (Notification.permission !== 'denied') {
                            Notification.requestPermission(function (permission) {
                                if (Notification.permission === "granted") {
                                    ShowBrowserNotification(result.NewChatMessages[i].Message);
                                } else {
                                    $.jGrowl(result.NewChatMessages[i].Message, {
                                        life: 5000,
                                        theme: result.NewChatMessages[i].State,
                                        header: result.NewChatMessages[i].Title
                                    });
                                }
                            });
                        }
                    }
                }

                for (var i = 0; i < result.NewChatRequests.length; i++) {
                    $("#jquery_jplayer_1").jPlayer("setMedia", { mp3: "vcr/1_9_0/Audio/drop.mp3" }).jPlayer("play", 0);

                    // Let's check if the browser supports notifications
                    if (!("Notification" in window)) {
                        $.jGrowl(result.NewChatRequests[i].Message, {
                            life: 5000,
                            theme: result.NewChatRequests[i].State,
                            header: result.NewChatRequests[i].Title
                        });
                        // Let's check whether notification permissions have already been granted
                    } else if (Notification.permission === "granted") {
                        ShowBrowserNotification(result.NewChatRequests[i].Message);
                        // Otherwise, we need to ask the user for permission
                    } else if (Notification.permission !== 'denied') {
                        Notification.requestPermission(function (permission) {
                            if (Notification.permission === "granted") {
                                ShowBrowserNotification(result.NewChatRequests[i].Message);
                            } else {
                                $.jGrowl(result.NewChatRequests[i].Message, {
                                    life: 5000,
                                    theme: result.NewChatRequests[i].State,
                                    header: result.NewChatRequests[i].Title
                                });
                            }
                        });
                    }
                }

                lastChatMessageID = result.LastChatMessageID;
                lastChatRequestID = result.LastChatRequestID;
                chatMessageCount = result.ChatMessageCount;
                chatRequestCount = result.ChatRequestCount;

                flashTitle();


                if (result.NewChatRequests.length > 0) {
                    window.focus();
                    $("#jquery_jplayer_1").jPlayer("setMedia", { mp3: "vcr/1_9_0/Audio/drop.mp3" }).jPlayer("play", 0);
                    alert(result.NewChatRequests[0].Message);
                    Ts.Services.System.GetChatUpdate(lastChatMessageID, lastChatRequestID, function (result2) {
                        if (result2.ChatRequestCount < 1) {
                            alert('All chat requests are already answered.');
                        }
                    });

                    window.focus();
                }
            }
        }

        var request_channel = null;
        function setupChatRequestUpdates() {
            top.Ts.Settings.System.read('PusherKey', '1', function (key) {
				var chatGUID = top.Ts.System.Organization.ChatID;

				if (pusherChatRequests == null || (pusherChatRequests != null && pusherChatRequests.connection.state == "disconnected")) {
					pusherChatRequests = new Pusher(key);
				}

				request_channel = pusherChatRequests.subscribe('chat-requests-' + chatGUID);

                request_channel.bind('new-chat-request', function (data) {
					var isGroupValidated = mainFrame.Ts.MainPage.ValidateChatForGroup(data.chatRequest.GroupID);

					if ((data.userIdInvited === undefined || data.userIdInvited == top.Ts.System.User.UserID) && isGroupValidated) {
                        var menuID = self.MainMenu.getSelected().getId();
                        var isMain = mainTabs.find(0, Ts.Ui.Tabs.Tab.Type.Main).getIsSelected();
                        if (!isMain || menuID != 'mniChat') self.MainMenu.find('mniChat', 'chat').setIsHighlighted(true);

                        window.focus();

                        $("#jquery_jplayer_1").jPlayer("setMedia", { mp3: "vcr/1_9_0/Audio/chime.mp3" }).jPlayer("play", 0);

                        window.focus();

                        // Let's check if the browser supports notifications
                        if (!("Notification" in window)) {
                            $.jGrowl(data.message, {
                                life: 5000,
                                theme: data.theme,
                                header: data.title
                            });
                        // Let's check whether notification permissions have already been granted
                        } else if (Notification.permission === "granted") {
                            ShowBrowserNotification(data.message);
                        // Otherwise, we need to ask the user for permission
                        } else if (Notification.permission !== 'denied') {
                            Notification.requestPermission(function (permission) {
                                if (Notification.permission === "granted") {
                                    ShowBrowserNotification(data.message);
                                } else {
                                    $.jGrowl(data.message, {
                                        life: 5000,
                                        theme: data.theme,
                                        header: data.title
                                    });
                                }
                            });
                        }

                        $("#jquery_jplayer_1").jPlayer("setMedia", { mp3: "vcr/1_9_0/Audio/chime.mp3" }).jPlayer("play", 0);

                        setTimeout(function () { alert(data.message); }, 100);
                    }
                });

            });
        }

        function ShowBrowserNotification(message) {
            var options = {
                body: message,
                icon: "https://app.teamsupport.com/images/icons/TeamSupportLogo16.png",
                tag: ""
            }
            var notification = new Notification("TeamSupport", options);
            notification.onshow = function () { setTimeout(function () { notification.close(); }, 5000) };
        }

        function turnOffChatRequestUpdates() {
            top.Ts.Settings.System.read('PusherKey', '1', function (key) {
                var chatGUID = top.Ts.System.Organization.ChatID;
				pusherChatRequests.disconnect();
            });
        }

        function flashTitle() {
            if (document.title != 'TeamSupport') {
                document.title = 'TeamSupport';
            } else if (chatRequestCount > 1) {
                document.title = chatRequestCount + ' New Chat Requests';
            } else if (chatRequestCount == 1) {
                document.title = chatRequestCount + ' New Chat Request';
            } else if (chatMessageCount > 1) {
                document.title = chatMessageCount + ' New Chat Messages';
            } else if (chatMessageCount == 1) {
                document.title = chatMessageCount + ' New Chat Message';
            }
            /*else if (wcMessageCount > 1) {
            document.title = wcMessageCount + ' New Water Cooler Messages';
            }
            else if (wcMessageCount == 1) {
            document.title = wcMessageCount + ' New Water Cooler Message';
            }*/

        }

        function openMenuItem(item) {
            var mainTab = mainTabs.find(0, Ts.Ui.Tabs.Tab.Type.Main);
            var caption = item.getCaption();
            if (caption.indexOf('My Tickets') == 0) caption = 'My Tickets';
            if (caption.indexOf('Water Cooler') == 0) caption = 'Water Cooler';
            if (item.getId().indexOf('mniTicketType') == 0) mainFrame.Ts.System.logAction('View Ticket Type Tickets');
            else mainFrame.Ts.System.logAction('View ' + caption);
            mainTab.setCaption(caption);
            mainTab.setImageUrl(item.getImageUrl());
            mainTab.select();
        }

        this.MainMenu = new Ts.Ui.MenuTree($('.main-menutree')[0]);
        this.MainMenu.bind('afterSelect', openMenuItem);
        this.MainMenu.bind('beforeSelect', beforeMenuItemSelect);

        Ts.Services.System.GetMainMenuItems(function (result) {
            function updateMenuItems(items, parent) {
                for (var i = 0; i < items.length; i++) {
                    var item = self.MainMenu.add(parent, items[i].ID, items[i].Type, items[i].Caption, items[i].ImageUrl, JSON.parse(items[i].Data));
                    if (items.length > 0) {
                        updateMenuItems(items[i].Items, item);
                    }
                }
            }
            updateMenuItems(result, null);
            self.updateMyOpenTicketReadCount();

            Ts.Services.System.GetMainTabs(function (result) {
                self.MainTabs.add(false, Ts.Ui.Tabs.Tab.Type.Main, 0, 'Dashboard', false, false, false, '', 'images/nav/16/dashboard.png');
                if (result) {
                    var tabs = JSON.parse(result);
                    for (var i = 0; i < tabs.length; i++) {
                        //isSelected, tabType, id, caption, isClosable, isSortable, isHighlighted, icon, imageUrl, data, title
                        if (tabs[i].TabType.indexOf('new') != 0 && tabs[i].ID != 0) {
                            //(result[i].Caption != null || result[i].Caption != '')
                            self.MainTabs.add(false, tabs[i].TabType, tabs[i].ID, tabs[i].Caption, true, true, false, null, null, tabs[i].Data, tabs[i].Title);
                        }
                    }
                }

                mainTabs.bind('afterSelect', openTab);
                mainTabs.bind('afterAdd', saveTabs);
                mainTabs.bind('beforeRemove', function (tab) {
                    var div = null;
                    switch (tab.getTabType()) {
                        case Ts.Ui.Tabs.Tab.Type.Ticket:
                            div = $('.main-ticket-' + tab.getId());
                            break;
                        case Ts.Ui.Tabs.Tab.Type.NewTicket:
                            div = $('.main-ticket-new');
                            break;
                        case Ts.Ui.Tabs.Tab.Type.NewCompany:
                            div = $('.main-tab-newCompany');
                            break;
                        case Ts.Ui.Tabs.Tab.Type.Company:
                            div = $('.main-tab-Customer');
                            break;
                        case Ts.Ui.Tabs.Tab.Type.Contact:
                            div = $('.main-tab-Contact');
                            break;
                        case Ts.Ui.Tabs.Tab.Type.Report:
                            div = $('.main-report-' + tab.getId());
                            break;
                        case Ts.Ui.Tabs.Tab.Type.NewAsset:
                            div = $('.main-tab-newAsset');
                            break;
                        case Ts.Ui.Tabs.Tab.Type.Asset:
                            div = $('.main-tab-Asset');
                            break;
                        case Ts.Ui.Tabs.Tab.Type.NewProduct:
                            div = $('.main-tab-newProduct');
                            break;
                        case Ts.Ui.Tabs.Tab.Type.Product:
                            div = $('.main-tab-Product');
                            break;
                        case Ts.Ui.Tabs.Tab.Type.NewProductFamily:
                            div = $('.main-tab-newProductFamily');
                            break;
                        case Ts.Ui.Tabs.Tab.Type.ProductFamily:
                            div = $('.main-tab-ProductFamily');
                            break;
                        case Ts.Ui.Tabs.Tab.Type.Task:
                            div = $('.main-tab-Task');
                            break;
                        default:
                    }

                    if (div) {
                        div.children('iframe').remove();
                        div.remove();
                    }

                });
                mainTabs.bind('afterRemove', function () {
                    saveTabs(mainTabs);
                });
                mainTabs.bind('sort', saveTabs);

                processQuery();

                self.MainMenu.find('mniWC2', 'wc2').select();
                if (notifyNewWC > 0) {
                    mainFrame.Ts.MainPage.MainMenu.find('mniWC2', 'wc2').setIsHighlighted(true);
                    mainFrame.Ts.MainPage.MainMenu.find('mniWC2', 'wc2').setCaption("Water Cooler (" + notifyNewWC + ")");
                }

            });


        });

        function openTab(tab) {

            $('.main-tab-content .ts-loading').show();
            $('.main-tab-content-item').hide();
            var div = null;
            switch (tab.getTabType()) {
                case Ts.Ui.Tabs.Tab.Type.Main:
                    var item = self.MainMenu.getSelected();

                    Ts.Services.Settings.WriteUserSetting('main-menu-selected', item.getId());

                    if (item.getType() == 'wc2') {
                        self.MainMenu.find('mniWC2', 'wc2').setIsHighlighted(false);
                        Ts.Services.System.UpdateLastWaterCoolerID();
                        self.MainMenu.find('mniWC2', 'wc2').setCaption("Water Cooler");
                    }

                    if (item.getType() == 'chat') {
                        self.MainMenu.find('mniChat', 'chat').setIsHighlighted(false);
                    }
                    div = $('.main-tab-content-item-' + item.getId());
                    if (div.length < 1) {
                        div = $('<div>')
                        .addClass('main-tab-content-item-' + item.getId() + ' main-tab-content-item main-tab-content-menu')
                        .appendTo('.main-tab-content');

                        var frame = $('<iframe>')
                        .attr('frameborder', 0)
                        .attr('scrolling', 'no')
                        .attr('id', 'iframe-' + item.getId())
                        .appendTo(div)
                        .attr('src', item.getData().ContentUrl);
                    } else {
                        div.show();
                        var data = div.data('pageData');
                        if (data) {
                            data.refresh();
                        } else {
                            var contentFrame = $(div).children('iframe')[0];
                            try {
                                if (contentFrame.contentWindow.refreshData) contentFrame.contentWindow.refreshData();
                            } catch (err) { }
                            try {
                                if (contentFrame.contentWindow.onShow) contentFrame.contentWindow.onShow();
                            } catch (err) { }
                            try {
                                if (contentFrame.contentWindow.ScrollMessages) contentFrame.contentWindow.ScrollMessages(true);
                            } catch (err) { }
                        }
                    }

                    try {
                        if ($('.main-ticket-' + mainFrame.Ts.TicketViewing).length > 0) {

                            $('.main-ticket-' + mainFrame.Ts.TicketViewing).find('iframe')[0].contentWindow.Unsubscribe();
                        }
                        else
                        {
                            var orgID = top.Ts.System.Organization.OrganizationID;
                            var presenceChannelName = 'presence-ticket-' + mainFrame.Ts.TicketViewing + '-org-' + orgID;
                            top.Ts.Pusher.unsubscribe(presenceChannelName);
                        }
                        mainFrame.Ts.TicketViewing = 0;
                    } catch (err) { }
                    $('.main-info-content').load(item.getData().PaneInfoUrl);
                    break;


                case Ts.Ui.Tabs.Tab.Type.Ticket:
                    var ticketID = tab.getId();
                    div = $('.main-tab-content .main-ticket-' + ticketID);
                    if (div.length < 1) {
                        div = $('<div>').addClass('main-tab-content-item main-tab-ticket main-ticket-' + ticketID).appendTo('.main-tab-content');
                        $('<iframe>').attr('id', 'ticket-' + ticketID).attr('frameborder', 0).attr('scrolling','no').attr('webkitallowfullscreen', '').attr('mozallowfullscreen', '').attr('allowfullscreen','').addClass('ticketIframe').appendTo(div).attr('src', (mainFrame.Ts.System.User.IsClassicView) ? 'vcr/1_9_0/Pages/TicketV2.html?TicketNumber=' + ticketID : 'vcr/1_9_0/Pages/Ticket.html?TicketNumber=' + ticketID);
                        //.attr('src', 'vcr/1_9_0/Pages/Ticket.html?TicketNumber=' + ticketID);
                    } else {
                        div.show();
                        try {
                            if (mainFrame.Ts.TicketViewing != 0) {
                                if ($('.main-ticket-' + mainFrame.Ts.TicketViewing).length > 0) {
                                    $('.main-ticket-' + mainFrame.Ts.TicketViewing).find('iframe')[0].contentWindow.Unsubscribe();
                                }
                            }
                            mainFrame.Ts.TicketViewing = 0;
                            mainFrame.Ts.Services.Dispatch.getTicketViewing(ticketID);
                        } catch (err) { }
                    }
                    $('.main-info-content').load('vcr/1_9_0/PaneInfo/ticket.html');
                    break;

                case Ts.Ui.Tabs.Tab.Type.NewTicket:
                    div = $('.main-tab-content .main-ticket-new');
                    if (div.length < 1) {
                        var query = '';
                        if (tab.getData()) query = tab.getData();
                        div = $('<div>')
              .addClass('main-tab-content-item main-tab-newticket main-ticket-new')
              .appendTo('.main-tab-content');

                        $('<iframe>')
              .attr('frameborder', 0)
              .attr('scrolling', 'no')
              .appendTo(div)
                      //.attr('src', 'vcr/1_9_0/Pages/NewTicket.html' + query);
                      .attr('src', (mainFrame.Ts.System.User.IsClassicView) ? 'vcr/1_9_0/Pages/NewTicketV2.html' + query : 'vcr/1_9_0/Pages/NewTicket.html' + query);
                        //.attr('src', 'frames/newticket.aspx' + query);
                    } else {
                        div.show();
                    }
                    try {
                        if ($('.main-ticket-' + mainFrame.Ts.TicketViewing).length > 0) {

                            $('.main-ticket-' + mainFrame.Ts.TicketViewing).find('iframe')[0].contentWindow.Unsubscribe();
                        }
                        mainFrame.Ts.TicketViewing = 0;
                    } catch (err) { }
                    $('.main-info-content').load('vcr/1_9_0/PaneInfo/newticket.html');
                    break;

                case Ts.Ui.Tabs.Tab.Type.NewCompany:
                    div = $('.main-tab-content .main-ticket-newCompany');
                    if (div.length < 1) {
                        var query = '';
                        if (tab.getData()) query = tab.getData();
                        div = $('<div>')
                    .addClass('main-tab-content-item main-tab-newCompany main-ticket-newCompany')
                    .appendTo('.main-tab-content');

                        $('<iframe>')
                    .attr('frameborder', 0)
                    .attr('scrolling', 'no')
                    .appendTo(div)
                    .attr('src', 'vcr/1_9_0/Pages/NewCustomer.html' + query);
                    }
                    else {
                        div.show();
                    }
                    $('.main-info-content').load('vcr/1_9_0/PaneInfo/customers.html');
                    break;
                case Ts.Ui.Tabs.Tab.Type.Company:
                    var OrgID = tab.getId();
                    div = $('.main-tab-content .main-Customer-' + OrgID);
                    if (div.length < 1) {
                        var query = '';
                        if (tab.getData()) query = tab.getData();
                        div = $('<div>')
                    .addClass('main-tab-content-item main-tab-Customer main-Customer-' + OrgID)
                    .appendTo('.main-tab-content');

                        $('<iframe>')
                    .attr('frameborder', 0)
                    .attr('scrolling', 'no')
                    .attr('id', 'iframe-o-' + OrgID)
                    .appendTo(div)
                    .attr('src', 'vcr/1_9_0/Pages/CustomerDetail.html' + query);
                    }
                    else {
                        mainFrame.privateServices.SetUserSetting('SelectedOrganizationID', OrgID);
                        mainFrame.privateServices.SetUserSetting('SelectedContactID', -1);
                        div.show();
                    }
                    $('.main-info-content').load('vcr/1_9_0/PaneInfo/customers.html');
                    break;
                case Ts.Ui.Tabs.Tab.Type.Contact:
                    var contactID = tab.getId();
                    div = $('.main-tab-content .main-Contact-' + contactID);
                    if (div.length < 1) {
                        var query = '';
                        if (tab.getData()) query = tab.getData();
                        div = $('<div>')
                    .addClass('main-tab-content-item main-tab-Contact main-Contact-' + contactID)
                    .appendTo('.main-tab-content');

                        $('<iframe>')
                    .attr('frameborder', 0)
                    .attr('scrolling', 'no')
                    .attr('id', 'iframe-u-' + contactID)
                    .appendTo(div)
                    .attr('src', 'vcr/1_9_0/Pages/ContactDetail.html' + query);
                    }
                    else {
                        mainFrame.Ts.Services.Customers.GetUser(contactID, function (user) {
                            mainFrame.privateServices.SetUserSetting('SelectedOrganizationID', user.OrganizationID);
                            mainFrame.privateServices.SetUserSetting('SelectedContactID', user.UserID);
                        });
                        div.show();
                    }
                    $('.main-info-content').load('vcr/1_9_0/PaneInfo/customers.html');
                    break;
                case Ts.Ui.Tabs.Tab.Type.Report:
                    var reportID = tab.getId();
                    var report = tab.getData();
                    div = $('.main-tab-content .main-report-' + reportID);
                    if (div.length < 1) {
                        div = $('<div>')
                        .addClass('main-tab-content-item main-tab-report main-report-' + reportID)
                        .appendTo('.main-tab-content');

                        var reportUrl = 'vcr/1_9_0/Pages/';
                        switch (report.ReportType) {
                            case 1: reportUrl = reportUrl + 'Reports_View_Chart.html?ReportID=' + reportID; break;
                            case 2: reportUrl = reportUrl + 'Reports_View_External.html?ReportID=' + reportID; break;
                            case 5: reportUrl = reportUrl + 'Reports_View_Tickets.html?ReportID=' + reportID; break;
                            default:
                            case 0: reportUrl = reportUrl + 'Reports_View_Tabular.html?ReportID=' + reportID;

                        }
                        $('<iframe>')
                        .attr('frameborder', 0)
                        .attr('scrolling', 'no')
                        .addClass('ticketIframe')
                        .appendTo(div)
                        .attr('src', reportUrl);
                    } else {
                        div.show();
                    }
                    $('.main-info-content').load('vcr/1_9_0/PaneInfo/reports.html');

                    break;
                case Ts.Ui.Tabs.Tab.Type.NewAsset:
                    div = $('.main-tab-content .main-ticket-newAsset');
                    if (div.length < 1) {
                        var query = '';
                        if (tab.getData()) query = tab.getData();
                        div = $('<div>')
                  .addClass('main-tab-content-item main-tab-newAsset main-ticket-newAsset')
                  .appendTo('.main-tab-content');

                        $('<iframe>')
                  .attr('frameborder', 0)
                  .attr('scrolling', 'no')
                  .appendTo(div)
                  .attr('src', 'vcr/1_9_0/Pages/NewAsset.html' + query);
                    }
                    else {
                        div.show();
                    }
                    $('.main-info-content').load('vcr/1_9_0/PaneInfo/Inventory.html');
                    break;
                case Ts.Ui.Tabs.Tab.Type.Asset:
                    var assetID = tab.getId();
                    div = $('.main-tab-content .main-Asset-' + assetID);
                    if (div.length < 1) {
                        var query = '';
                        if (tab.getData()) query = tab.getData();
                        div = $('<div>')
                    .addClass('main-tab-content-item main-tab-Asset main-Asset-' + assetID)
                    .appendTo('.main-tab-content');

                        $('<iframe>')
                    .attr('frameborder', 0)
                    .attr('scrolling', 'no')
                    .attr('id', 'iframe-o-' + assetID)
                    .appendTo(div)
                    .attr('src', 'vcr/1_9_0/Pages/AssetDetail.html' + query);
                    }
                    else {
                        mainFrame.privateServices.SetUserSetting('SelectedAssetID', assetID);
                        //                    mainFrame.privateServices.SetUserSetting('SelectedContactID', -1);
                        div.show();
                    }
                    $('.main-info-content').load('vcr/1_9_0/PaneInfo/inventory.html');
                    break;

                case Ts.Ui.Tabs.Tab.Type.NewProduct:
                    div = $('.main-tab-content .main-ticket-newProduct');
                    if (div.length < 1) {
                        var query = '';
                        if (tab.getData()) query = tab.getData();
                        div = $('<div>')
                  .addClass('main-tab-content-item main-tab-newProduct main-ticket-newProduct')
                  .appendTo('.main-tab-content');

                        $('<iframe>')
                  .attr('frameborder', 0)
                  .attr('scrolling', 'no')
                  .appendTo(div)
                  .attr('src', 'vcr/1_9_0/Pages/NewProduct.html' + query);
                    }
                    else {
                        div.show();
                    }
                    $('.main-info-content').load('vcr/1_9_0/PaneInfo/Products.html');
                    break;
                case Ts.Ui.Tabs.Tab.Type.Product:
                    var productID = tab.getId();
                    mainFrame.privateServices.SetUserSetting('SelectedProductID', productID);
                    div = $('.main-tab-content .main-Product-' + productID);
                    if (div.length < 1) {
                        var query = '';
                        if (tab.getData()) query = tab.getData();
                        div = $('<div>')
                    .addClass('main-tab-content-item main-tab-Product main-Product-' + productID)
                    .appendTo('.main-tab-content');

                        $('<iframe>')
                    .attr('frameborder', 0)
                    .attr('scrolling', 'no')
                    .attr('id', 'iframe-o-' + productID)
                    .appendTo(div)
                    .attr('src', 'vcr/1_9_0/Pages/ProductDetail.html' + query);
                    }
                    else {
                        div.show();
                    }
                    $('.main-info-content').load('vcr/1_9_0/PaneInfo/Products.html');
                    break;


                case Ts.Ui.Tabs.Tab.Type.NewProductVersion:
                    div = $('.main-tab-content .main-ticket-newProductVersion');
                    if (div.length < 1) {
                        var query = '';
                        if (tab.getData()) query = tab.getData();
                        div = $('<div>')
                  .addClass('main-tab-content-item main-tab-newProductVersion main-ticket-newProductVersion')
                  .appendTo('.main-tab-content');

                        $('<iframe>')
                  .attr('frameborder', 0)
                  .attr('scrolling', 'no')
                  .appendTo(div)
                  .attr('src', 'vcr/1_9_0/Pages/NewProductVersion.html' + query);
                    }
                    else {
                        div.show();
                    }
                    $('.main-info-content').load('vcr/1_9_0/PaneInfo/Products.html');
                    break;
                case Ts.Ui.Tabs.Tab.Type.ProductVersion:
                    var productVersionID = tab.getId();
                    mainFrame.privateServices.SetUserSetting('SelectedProductVersionID', productVersionID);
                    mainFrame.Ts.Services.Products.GetVersion(productVersionID, function (productVersion) {
                        mainFrame.privateServices.SetUserSetting('SelectedProductID', productVersion.ProductID);
                    });
                    div = $('.main-tab-content .main-Product-Version-' + productVersionID);
                    if (div.length < 1) {
                        var query = '';
                        if (tab.getData()) query = tab.getData();
                        div = $('<div>')
                    .addClass('main-tab-content-item main-tab-Product-Version main-Product-Version-' + productVersionID)
                    .appendTo('.main-tab-content');

                        $('<iframe>')
                    .attr('frameborder', 0)
                    .attr('scrolling', 'no')
                    .attr('id', 'iframe-o-' + productID)
                    .appendTo(div)
                    .attr('src', 'vcr/1_9_0/Pages/ProductVersionDetail.html' + query);
                    }
                    else {
                        div.show();
                    }
                    $('.main-info-content').load('vcr/1_9_0/PaneInfo/Products.html');
                    break;
                case Ts.Ui.Tabs.Tab.Type.NewProductFamily:
                    div = $('.main-tab-content .main-ticket-newProductFamily');
                    if (div.length < 1) {
                        var query = '';
                        if (tab.getData()) query = tab.getData();
                        div = $('<div>')
                  .addClass('main-tab-content-item main-tab-newProductFamily main-ticket-newProductFamily')
                  .appendTo('.main-tab-content');

                        $('<iframe>')
                  .attr('frameborder', 0)
                  .attr('scrolling', 'no')
                  .appendTo(div)
                  .attr('src', 'vcr/1_9_0/Pages/NewProductFamily.html' + query);
                    }
                    else {
                        div.show();
                    }
                    $('.main-info-content').load('vcr/1_9_0/PaneInfo/ProductFamilies.html');
                    break;
                case Ts.Ui.Tabs.Tab.Type.ProductFamily:
                    var productFamilyID = tab.getId();
                    mainFrame.privateServices.SetUserSetting('SelectedProductFamilyID', productFamilyID);
                    div = $('.main-tab-content .main-Product-Family-' + productFamilyID);
                    if (div.length < 1) {
                        var query = '';
                        if (tab.getData()) query = tab.getData();
                        div = $('<div>')
                    .addClass('main-tab-content-item main-tab-Product-Family main-Product-Family-' + productFamilyID)
                    .appendTo('.main-tab-content');

                        $('<iframe>')
                    .attr('frameborder', 0)
                    .attr('scrolling', 'no')
                    .attr('id', 'iframe-o-' + productFamilyID)
                    .appendTo(div)
                    .attr('src', 'vcr/1_9_0/Pages/ProductFamilyDetail.html' + query);
                    }
                    else {
                        div.show();
                    }
                    $('.main-info-content').load('vcr/1_9_0/PaneInfo/ProductFamily.html');
                    break;

                case Ts.Ui.Tabs.Tab.Type.NewTask:
                    div = $('.main-tab-content .main-ticket-newTask');
                    if (div.length < 1) {
                        var query = '';
                        if (tab.getData()) query = tab.getData();
                        div = $('<div>')
                        .addClass('main-tab-content-item main-tab-newTask main-ticket-newTask')
                        .appendTo('.main-tab-content');

                        $('<iframe>')
                        .attr('frameborder', 0)
                        .attr('scrolling', 'no')
                        .appendTo(div)
                        .attr('src', 'vcr/1_9_0/Pages/NewTask.html' + query);
                    }
                    else {
                        div.show();
                    }
                    //$('.main-info-content').load('vcr/1_9_0/PaneInfo/Inventory.html');
                    break;
                case Ts.Ui.Tabs.Tab.Type.Task:
                    var reminderID = tab.getId();
                    div = $('.main-tab-content .main-Task-' + reminderID);
                    if (div.length < 1) {
                        var query = '';
                        if (tab.getData()) query = tab.getData();
                        div = $('<div>')
                    .addClass('main-tab-content-item main-tab-Task main-Task-' + reminderID)
                    .appendTo('.main-tab-content');

                        $('<iframe>')
                    .attr('frameborder', 0)
                    .attr('scrolling', 'no')
                    .attr('id', 'iframe-o-' + reminderID)
                    .appendTo(div)
                    .attr('src', 'vcr/1_9_0/Pages/TaskDetail.html' + query);
                    }
                    else {
                        mainFrame.privateServices.SetUserSetting('SelectedTaskID', reminderID);
                        //                    mainFrame.privateServices.SetUserSetting('SelectedContactID', -1);
                        div.show();
                    }
                    //$('.main-info-content').load('vcr/1_9_0/PaneInfo/inventory.html');
                    break;
                case Ts.Ui.Tabs.Tab.Type.NewTaskFromSource:
                    var query = '';
                    if (tab.getData()) query = tab.getData();
                    div = $('<div>')
                    .addClass('main-tab-content-item main-tab-newTask main-ticket-newTask')
                    .appendTo('.main-tab-content');

                    $('<iframe>')
                    .attr('frameborder', 0)
                    .attr('scrolling', 'no')
                    .appendTo(div)
                    .attr('src', 'vcr/1_9_0/Pages/NewTask.html' + query);
                    break;

                default:

            }


            $('.main-tab-content .ts-loading').hide();
            Ts.Services.System.UpdateLastActivity();
            _lastActivity = new Date();
            $('.status-frame').text(div.children('iframe').attr('src'));
        }

        function saveTabs(sender) {
            var tabs = sender.getTabs();
            var items = [];
            for (var i = 0; i < tabs.length; i++) {
                var tab = new Object();
                tab.ID = tabs[i].getId();
                tab.TabType = tabs[i].getTabType();
                tab.Caption = tabs[i].getCaption();
                tab.Data = tabs[i].getData();
                items.push(tab);
            }

            Ts.Services.System.SetMainTabs(JSON.stringify(items));
        }

        this.MainTabs = new Ts.Ui.Tabs($('.main-tabs')[0], '.main-tab-content');
        var mainTabs = this.MainTabs;

        $('.main-info-close').click(function (e) {
            e.preventDefault();
            self.MainLayout.close('east');


        });

        var execGetTicket = null;

        function getTicketsByTerm(request, response) {
            if (execGetTicket) {
                execGetTicket._executor.abort();
            }
            //execGetTicket = Ts.Services.Tickets.GetTicketsByTerm(request.term, function (result) { response(result); });
            execGetTicket = Ts.Services.Tickets.SearchTickets(request.term, null, function (result) {
                $('.main-quick-ticket').removeClass('ui-autocomplete-loading');
                response(result);
            });

        }

        $('.main-quick-ticket').autocomplete({
            minLength: 2,
            source: getTicketsByTerm,
            delay: 300,
            select: function (event, ui) {
                if (ui.item) {
                    self.openTicket(ui.item.id);
                    mainFrame.Ts.System.logAction('Main Page - Quick Search Open Ticket');
                }
                $('.main-quick-ticket').removeClass('ui-autocomplete-loading');
            }
        });

        $('.main-quick-ticket')
            .focusin(function () {
                $(this).val('').removeClass('main-quick-ticket-blur');
            })
            .focusout(function () {
                $(this).val('Search for a ticket...').addClass('main-quick-ticket-blur').removeClass('ui-autocomplete-loading');
            })
            .click(function () {
                $(this).val('').removeClass('main-quick-ticket-blur');
            })
    .val('Search for a ticket...');
        /*
        mainFrame.Ts.Services.Users.ShowIntroVideo(function (result) {
        var overrideIntro = Ts.Utils.getQueryValue('intro');
        if (result === false && !(overrideIntro != null && overrideIntro == 1)) return;
        var div = $('<div>')
        .addClass('dialog-intro')
        .append('<iframe width="420" height="315" src="https://www.youtube.com/embed/BVl7zLVzT7E?rel=0&autoplay=1" frameborder="0" allowfullscreen></iframe>')
        .appendTo('body');

        div.dialog({
        width: 'auto',
        height: 'auto',
        title: 'Introduction',
        resizable: false,
        close: function () {
        div.remove();
        }
        });
        });
        */



        Ts.Services.Settings.ReadUserSetting('main-info-state', true, function (isOpen) {

            if (isOpen == "True") self.MainLayout.open('east');

        });
      /*
        this.WndScreenR = $('<iframe>', {
            id: 'wndScreenR',
            name: 'wndScreenR',
            src: 'ScreenR.html',
            width: '0',
            height: '0',
            frameborder: '0',
            scrolling: 'no'
        }).appendTo('body')[0];
*/


    }, // end init

	ValidateChatForGroup: function (groupRequested) {
		var userGroupFound = top.Ts.System.UserGroups.find(function (group) {
			return group == groupRequested;
		});

		return (userGroupFound != undefined || groupRequested == 0);
	},

    recordScreen: function (params, onComplete, onCancel) {
        if (!params) {
            params = new Object();
            params.userName = mainFrame.Ts.System.User.FirstName + ' ' + mainFrame.Ts.System.User.LastName;
            params.userEmail = mainFrame.Ts.System.User.Email;
            params.hideAllFields = true;
            params.maxTimeLimit = 300;
        }

        params.id = "b67bdeab7c084032bc4f37e5308eae1e";

        var recorder = this.WndScreenR.contentWindow.Screenr.Recorder({
            id: "b67bdeab7c084032bc4f37e5308eae1e",
            hideAllFields: true,
            maxTimeLimit: 300
        }); // this.WndScreenR.contentWindow.Screenr.Recorder(params);
        if (onComplete) recorder.setOnComplete(onComplete);
        if (onCancel) recorder.setOnCancel(onCancel);


        recorder.record();


    },

    openTicketByID: function (ticketID, doSelect) {
        var self = this;
        Ts.Services.Tickets.GetTicketNumber(ticketID, function (number) {
            self.openTicket(number, doSelect);
        });
    },
    openTicket: function (ticketNumber, doSelect) {
        var self = this;
        doSelect = doSelect == null ? true : doSelect;
        Ts.Services.Tickets.GetTicketName(ticketNumber, function (name) {
            self.MainTabs.prepend(doSelect, Ts.Ui.Tabs.Tab.Type.Ticket, ticketNumber, 'Ticket: ' + ticketNumber, true, true, false, null, null, null, name);
        });
    },
    newTicket: function (query) {
        var mi = this.MainTabs.getSelected();
        if (mi.getTabType() == "contact" || mi.getTabType() == "company")
            query = mi.getData();
        this.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.NewTicket, 'new', 'New Ticket', true, true, true, null, null, query, null);
    },
    closeNewTicketTab: function () {
        var tab = this.MainTabs.find('new', Ts.Ui.Tabs.Tab.Type.NewTicket);
        if (tab) {
            this.closeTab(tab);
            tab.remove();
        }
    },
    closeTab: function (tab) {
        return tab.remove();
    },
    closeReportTab: function (reportID) {
        var tab = this.MainTabs.find(reportID, Ts.Ui.Tabs.Tab.Type.Report);
        if (tab) {
            return this.closeTab(tab);
        }
        return false;
    },
    openReport: function (report, doSelect) {
        var self = this;
        if (report.ReportID) {
            var data = { ReportID: report.ReportID, ReportType: report.ReportType, Name: report.Name };
            self.MainTabs.prepend(doSelect || true, Ts.Ui.Tabs.Tab.Type.Report, report.ReportID, report.Name, true, true, false, null, null, data, report.Name);
        } else {
            mainFrame.Ts.Utils.webMethod("ReportService", "GetReport", {
                "reportID": report
            }, function (result) {
                var data = { ReportID: result.ReportID, ReportType: result.ReportType, Name: result.Name };
                self.MainTabs.prepend(doSelect || true, Ts.Ui.Tabs.Tab.Type.Report, result.ReportID, result.Name, true, true, false, null, null, result, result.Name);
            });
        }
    },
    closeTicketTab: function (ticketNumber) {
        var tab = this.MainTabs.find(ticketNumber, Ts.Ui.Tabs.Tab.Type.Ticket);
        if (tab) {
            return this.closeTab(tab);
        }
        return false;
    },
    highlightTicketTab: function (ticketNumber, isHighlighted) {
        var tab = this.MainTabs.find(ticketNumber, Ts.Ui.Tabs.Tab.Type.Ticket);
        if (tab) {
            tab.setIsHighlighted(isHighlighted);
        }
    },
    highlightNewTaskTab: function (isHighlighted) {
        var tab = this.MainTabs.find('newTask', Ts.Ui.Tabs.Tab.Type.NewTask);
        if (tab) {
            tab.setIsHighlighted(isHighlighted);
        }
    },
    installChromePlugin: function ()
    {
    	chrome.webstore.install("https://chrome.google.com/webstore/detail/laehkaldepkacogpkokmimggbepafabg",
function () { }, function (e) { console.log(e) });
    },
    openAttachment: function (attachmentID) {
        window.open('../Attachment.aspx?attachmentID=' + attachmentID, 'Attachment' + attachmentID);
    },
    openNewAttachment: function (attachmentID) {
        window.open('../../../Attachment.aspx?attachmentID=' + attachmentID, 'Attachment' + attachmentID);
    },
    openAsset: function (assetID) {
        this.openNewAsset(assetID);
    },
    openProduct: function (productID) {
        this.openNewProduct(productID);
    },
    openProductVersion: function (productVersionID) {
        this.openNewProductVersion(productVersionID);
    },
    openUser: function (userID) {
        var self = this;
        Ts.Services.Organizations.IsUserContact(userID, function (isContact) {
            if (isContact) {
                self.openNewContact(userID);
                return;
            }
            Ts.Services.Settings.WriteUserSetting('SelectedUserID', userID, function () {
                Ts.Services.Settings.WriteUserSetting('SelectedUserTabIndex', 0, function () {
                    self.MainMenu.find('mniUsers', 'users').select();
                    var element = $('.main-tab-content-item:visible');
                    var contentFrame = $(element).children('iframe')[0];
                    if (contentFrame && contentFrame.contentWindow.refreshData) {
                        contentFrame.contentWindow.refreshData;
                    }
                });
            });


        });
    },
    openContact: function (contactID, customerID) {
        this.openNewContact(contactID, customerID);

    },
    openCustomerByName: function (name) {
        var self = this;
        Ts.Services.Organizations.GetIDByName(name, function (id) {
            self.openNewCustomer(id);
        });
    },
    openCustomerByExactName: function (name) {
        var self = this;
        Ts.Services.Organizations.GetIDByExactNameNoFilter(name, function (id) {
            self.openNewCustomer(id);
        });
    },
    openCustomer: function (customerID) {
        this.openNewCustomer(customerID);
    },
    openCustomerNote: function (customerID, noteID) {
        _selectContactID = -1;
        _selectCustomerID = customerID;

        var self = this;
        Ts.Services.Settings.WriteUserSetting('SelectedOrganizationTabIndex', 4, function () {
            Ts.Services.Settings.WriteUserSetting('SelectedCustomerNoteID', noteID, function () {
                self.MainMenu.find('mniCustomers', 'customers').select();
                var element = $('.main-tab-content-item:visible');
                var contentFrame = $(element).children('iframe')[0];
                if (contentFrame && contentFrame.contentWindow.reload) {
                    contentFrame.contentWindow.reload();
                }
            });
        });
    },
    openGroup: function (groupID) {
        var self = this;
        Ts.Services.Settings.WriteUserSetting('SelectedGroupID', groupID, function () {
            Ts.Services.Settings.WriteUserSetting('SelectedGroupTabIndex', 0, function () {
                self.MainMenu.find('mniGroups', 'groups').select();
                var element = $('.main-tab-content-item:visible');
                var contentFrame = $(element).children('iframe')[0];
                if (contentFrame && contentFrame.contentWindow.refreshData) {
                    contentFrame.contentWindow.refreshData;
                }
            });
        });
    },
    openAdmin: function (tabText) {
        var self = this;
        mainFrame.Ts.Settings.Organization.write('SelectedAdminTabText', tabText, function () {
            self.MainMenu.find('mniAdmin', 'admin').select();
        });


    },
    openProduct: function (productID, versionID) {
        var self = this;
        if (versionID == null) versionID = -1;
        Ts.Services.Settings.WriteUserSetting('SelectedProductTabIndex', versionID < 0 ? 0 : 1, function () {
            Ts.Services.Settings.WriteUserSetting('SelectedProductID', productID, function () {
                Ts.Services.Settings.WriteUserSetting('SelectedVersionID', versionID, function () {
                    self.MainMenu.find('mniProducts', 'products').select();
                    var element = $('.main-tab-content-item:visible');
                    var contentFrame = $(element).children('iframe')[0];
                    if (contentFrame && contentFrame.contentWindow.refreshData) {
                        contentFrame.contentWindow.refreshData;
                    }
                    window.parent.document.getElementById('iframe-mniProducts').contentWindow.reload();
                });
            });
        });

    },
    openProductVersion: function (productID, versionID) {
        var self = this;
        if (versionID == null) versionID = -1;
        Ts.Services.Settings.WriteUserSetting('SelectedProductTabIndex', versionID < 0 ? 0 : 1, function () {
            Ts.Services.Settings.WriteUserSetting('SelectedProductID', productID, function () {
                Ts.Services.Settings.WriteUserSetting('SelectedVersionID', versionID, function () {
                    self.MainMenu.find('mniProducts', 'products').select();
                    var element = $('.main-tab-content-item:visible');
                    var contentFrame = $(element).children('iframe')[0];
                    if (contentFrame && contentFrame.contentWindow.reload) {
                        contentFrame.contentWindow.reload();
                    }

                });
            });
        });

    },
    openWiki: function (articleID) {
        this.MainMenu.find('mniWiki', 'wiki').select();
        var element = $('.main-tab-content-item:visible');
        //$(element).children('iframe').attr('src', 'Wiki/ViewPage.aspx?ArticleID=' + articleID);
        $(element).children('iframe').attr('src', 'vcr/1_6_0/Pages/wiki_view.html?ArticleID=' + articleID);
    },
    editWiki: function (articleID) {
      this.MainMenu.find('mniWiki', 'wiki').select();
      var element = $('.main-tab-content-item:visible');
      //$(element).children('iframe').attr('src', 'Wiki/ViewPage.aspx?ArticleID=' + articleID);
      $(element).children('iframe').attr('src', 'vcr/1_6_0/Pages/wiki_edit.html?ArticleID=' + articleID);
    },
    openUser: function (userID) {
      var self = this;
      Ts.Services.Organizations.IsUserContact(userID, function (isContact) {
        if (isContact) {
          self.openNewContact(userID);
        }
        else
        {
          self.MainMenu.find('mniUsers', 'users').select();
          var element = $('.main-tab-content-item:visible');
          $(element).children('iframe').attr('src', 'vcr/1_6_0/Pages/Users.html?UserID=' + userID);
        }
      });
        //this.MainMenu.find('mniUsers', 'users').select();
        //var element = $('.main-tab-content-item:visible');
        //$(element).children('iframe').attr('src', 'vcr/1_6_0/Pages/Users.html?UserID=' + userID);
    },
    hideWelcome: function () {
        this.MainMenu.find('mniDashboard', 'dashboard').select();
        $('.menutree-item-welcome-mniWelcome').remove();

        mainFrame.Ts.Services.Users.HideWelcomePage(function () { });
    },
    openOrganizationProduct: function (organizationProductID) {
        var self = this;
        Ts.Services.Organizations.GetOrganizationProduct(organizationProductID, function (op) {
            //self.openProduct(op.ProductID, op.ProductVersionID == null ? -1 : op.ProductVersionID);
            self.openNewProduct(op.ProductID);
        });
    },
    openOrganizationProductVersion: function (organizationProductID) {
        var self = this;
        Ts.Services.Organizations.GetOrganizationProduct(organizationProductID, function (op) {
            //self.openProduct(op.ProductID, op.ProductVersionID == null ? -1 : op.ProductVersionID);
            self.openNewProductVersion(op.ProductVersionID);
        });
    },
    openProductOrganization: function (organizationProductID) {
        var self = this;
        Ts.Services.Organizations.GetOrganizationProduct(organizationProductID, function (op) {
            self.openCustomer(op.OrganizationID);
        });
    },
    openUserProduct: function (userProductID) {
      var self = this;
      Ts.Services.Users.GetUserProduct(userProductID, function (op) {
        //self.openProduct(op.ProductID, op.ProductVersionID == null ? -1 : op.ProductVersionID);
        self.openNewProduct(op.ProductID);
      });
    },
    openUserProductVersion: function (userProductID) {
      var self = this;
      Ts.Services.Users.GetUserProduct(userProductID, function (op) {
        //self.openProductr(op.ProductID, op.ProductVersionID == null ? -1 : op.ProductVersionID);
        self.openNewProductVersion(op.ProductVersionID);
      });
    },
    openTag: function (tagID) {
        this.MainMenu.find('mniTicketTags', 'tickettags').select();
        var element = $('.main-tab-content-item:visible');
        $(element).children('iframe').attr('src', 'Frames/TicketTags.aspx?TagID=' + tagID);
    },
    openTicketView: function (ReportID, isPrivate) {
        if (isPrivate.toString() == "true") {
            var myMenu = $('.menutree-item-mytickets-mniMyTickets').find('span.ui-icon.ui-icon-carat-1-e');
            myMenu.removeClass("ui-icon-carat-1-e").addClass("ui-icon-carat-1-s");

            var myMenuItems = $('.menutree-item-mytickets-mniMyTickets').find('ul.ui-helper-hidden');
            myMenuItems.show();

        }
        else {
            var allMenu = $('.menutree-item-tickets-mniTickets').find('span.ui-icon.ui-icon-carat-1-e');
            allMenu.removeClass("ui-icon-carat-1-e").addClass("ui-icon-carat-1-s");

            var allMenuItems = $('.menutree-item-tickets-mniTickets').find('ul.ui-helper-hidden');
            allMenuItems.show();
        }
        this.MainMenu.find('mniTicketView_' + ReportID, 'tickettype').select();
        var element = $('.main-tab-content-item:visible');
        $(element).children('iframe').attr('src', 'vcr/1_6_0/Pages/TicketView.html?ReportID=' + ReportID);
    },
    addDebugStatus: function (text) {
        $('.status-debug').text(text);
    },
    selectTicket: function (filter, callback) {
        $('.dialog-select-ticket').find('input').data('filter', filter).val('');
        var buttons = $('.dialog-select-ticket').dialog('option', 'buttons');

        buttons.OK = function () {
            $(this).dialog("close");
            callback($(this).find('input').data('item').data);
        }
        $('.dialog-select-ticket').dialog('option', 'buttons', buttons).dialog('open').find('input').focus();
    },
    selectActivity: function (filter, callback) {
        $('.dialog-select-activity').find('input').data('filter', filter).val('');
        var buttons = $('.dialog-select-activity').dialog('option', 'buttons');

        buttons.OK = function () {
            $(this).dialog("close");
            callback($(this).find('input').data('item').data);
        }
        $('.dialog-select-activity').dialog('option', 'buttons', buttons).dialog('open').find('input').focus();
    },
    selectWiki: function (callback) {
        $('.dialog-select-wiki').find('input').val('');
        var buttons = $('.dialog-select-wiki').dialog('option', 'buttons');

        buttons.OK = function () {
            $(this).dialog("close");
            var test = $(this).find('input').data('item').data;
            callback($(this).find('input').data('item').data);
        }
        $('.dialog-select-wiki').dialog('option', 'buttons', buttons).dialog('open').find('input').focus();
    },
    updateMyOpenTicketReadCount: function () {
        Ts.Services.Tickets.GetMyOpenReadCount(function (result) {
            if (result > 0) {
                $('.menutree-item-mytickets-mniMyTickets a').first().text('My Tickets (' + result + ')').css('font-weight', 'bold');
            } else {
                $('.menutree-item-mytickets-mniMyTickets a').first().text('My Tickets').css('font-weight', 'normal');
            }
        });
    },
    addNewTicketView: function (report, isPrivate, open) {
        var self = this;
        Ts.Services.System.GetNewTicketViewMenuItem(report.ReportID, function (result) {
            var parent;
            if (isPrivate.toString() == "true") {
                parent = self.MainMenu.getByID('mniMyTickets');
            }
            else {
                parent = self.MainMenu.getByID('mniTickets');
            }
            self.MainMenu.add(parent, result.ID, result.Type, result.Caption, result.ImageUrl, JSON.parse(result.Data));

            if (open) {
                mainFrame.Ts.MainPage.openTicketView(report.ReportID, isPrivate);
            }
        });
    },
    updateTicketViewItem: function (report, isPrivate, open) {
        var self = this;
        Ts.Services.System.GetNewTicketViewMenuItem(report.ReportID, function (result) {
            var parent;
            if (isPrivate.toString() == "true") {
                parent = self.MainMenu.getByID('mniMyTickets');
            }
            else {
                parent = self.MainMenu.getByID('mniTickets');
            }

            $(".menutree-item-tickettype-mniTicketView_" + report.ReportID).remove();
            self.MainMenu.add(parent, result.ID, result.Type, result.Caption, result.ImageUrl, JSON.parse(result.Data));

            if (open) {
                mainFrame.Ts.MainPage.openTicketView(report.ReportID, isPrivate);
            }
        });
    },
    updateMenu: function () {
        var self = this;
        $(".main-menutree").empty();
        Ts.Services.System.GetMainMenuItems(function (result) {
            function updateMenuItems(items, parent) {
                for (var i = 0; i < items.length; i++) {
                    var item = self.MainMenu.add(parent, items[i].ID, items[i].Type, items[i].Caption, items[i].ImageUrl, JSON.parse(items[i].Data));
                    if (items.length > 0) {
                        updateMenuItems(items[i].Items, item);
                    }
                }
            }
            updateMenuItems(result, null);
            self.updateMyOpenTicketReadCount();
        });
    },
    editReminder: function (reminder, doSave, callback) {
        $('.dialog-reminder').dialog('option', 'width', 300);
        $('.dialog-reminder').dialog('option', 'height', 300);

        var select = $('.dialog-reminder .reminder-user');
        select.empty();
        var users = mainFrame.Ts.Cache.getUsers();
        if (users != null) {
            for (var i = 0; i < users.length; i++) {
                $('<option>').attr('value', users[i].UserID).text(users[i].Name).data('o', users[i]).appendTo(select);
            }
        }

        var buttons = $('.dialog-reminder').dialog('option', 'buttons');

        buttons.OK = function () {
            if ($.trim($('.dialog-reminder .reminder-description').val()) == '') {
                $('.dialog-reminder .reminder-description').closest('.label-block').addClass('ui-state-error ui-corner-all');
                return;
            }

            reminder.Description = $('.dialog-reminder .reminder-description').val();
            reminder.DueDate = mainFrame.Ts.Utils.getMsDate($('.dialog-reminder .reminder-date').datetimepicker('getDate'));
            reminder.UserID = $('.dialog-reminder .reminder-user').val();
            var dialog = $(this);
            if (doSave == false) {
                dialog.dialog("close");
                if (callback) callback(reminder);
            } else {
                //(int? reminderID, ReferenceType refType, int refID, string description, DateTime dueDate, int userID)
                mainFrame.Ts.Services.System.EditReminder(
        reminder.ReminderID,
        reminder.RefType,
        reminder.RefID,
        reminder.Description,
        reminder.DueDate,
        reminder.UserID,
        function (result) {
            dialog.dialog("close");
            if (callback) callback(result);
        },
        function () {
            alert('There was an error saving your reminder.  Please try again.');
        }
        );
            }
        }

        $('.dialog-reminder').dialog('option', 'buttons', buttons).dialog('open');

        if (!reminder.RefID) {
            reminder.RefID = null;
        }
        if (!reminder.RefType) {
            reminder.RefType = null;
        }

        if (reminder.ReminderID) {
            $('.dialog-reminder').find('.ts-loading').show().next().hide();
            mainFrame.Ts.Services.System.GetReminder(reminder.ReminderID, function (result) {
                reminder = result;
                $('.dialog-reminder .reminder-description').val(reminder.Description);
                $('.dialog-reminder .reminder-date').datetimepicker('setDate', reminder.DueDate),
        $('.dialog-reminder .reminder-user').combobox('setValue', reminder.UserID);
                $('.dialog-reminder').find('.ts-loading').hide().next().show();
            });
        } else {
            reminder.ReminderID = null;
            $('.dialog-reminder').find('.ts-loading').hide().next().show();
            $('.dialog-reminder .reminder-description').val((params.Description ? reminder.Description : ''));
            $('.dialog-reminder .reminder-date').datetimepicker('setDate', (reminder.DueDate ? reminder.DueDate : new Date()));
            select.combobox('setValue', (reminder.UserID ? reminder.UserID : mainFrame.Ts.System.User.UserID));
        }
        //$('.reminder-description').val((params.Description ? params.Description : ''));
        //$('.reminder-description').val((params.Description ? params.Description : ''));
    },

    openWaterCoolerInstance: function (wcID, pagetype, pageid) {
        var self = this;
        self.MainMenu.find('mniWC2', 'wc2').select();
        var element = $('.main-tab-content-item:visible');
        $(element).children('iframe').attr('src', 'vcr/1_6_0/Pages/WaterCooler.html?wcinstanceid=' + wcID + '&pageid=' + pageid + '&pagetype=' + pagetype);
    },

    pasteImage: function (filter, callback) {
        var buttons = $('.dialog-paste-image').dialog('option', 'buttons');

        buttons.OK = function () {
            Ts.Services.Tickets.SavePasteImage($('#img1').val(), $('#testImage').attr('src'), function (result) {
                callback(result);
            });
            $(this).dialog("close");

        };
        $('.dialog-paste-image').dialog('option', 'buttons', buttons).dialog('open').focus();
    },

    newCustomer: function (tab, orgID) {
        var query;
        if (tab != undefined)
            query = "?open=" + tab + "&organizationid=" + orgID;
        this.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.NewCompany, 'newCustomer', 'Add Customer', true, true, true, null, null, query, null);
    },
    closenewCustomerTab: function () {
        var tab = this.MainTabs.find('newCustomer', Ts.Ui.Tabs.Tab.Type.NewCompany);
        if (tab) {
            this.closeTab(tab);
            tab.remove();
        }
    },
    openNewCustomer: function (customerID) {
        var orgname;
        var query = "?organizationid=" + customerID;
        mainFrame.Ts.Services.Organizations.GetShortNameFromID(customerID, function (result) {
            this.Ts.MainPage.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.Company, customerID, result, true, true, false, null, null, query, null);
        });

    },
    openNewCustomerInParentView: function (customerID) {
        var orgname;
        var query = "?organizationid=" + customerID + "&parentView=1";
        mainFrame.Ts.Services.Organizations.GetShortNameFromID(customerID, function (result) {
            this.Ts.MainPage.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.Company, customerID, result, true, true, false, null, null, query, null);
        });

    },
    openNewCustomerNote: function (customerID, noteID) {
        var orgname;
        var query = "?organizationid=" + customerID + "&noteid=" + noteID;
        mainFrame.Ts.Services.Organizations.GetShortNameFromID(customerID, function (result) {
            this.Ts.MainPage.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.Company, customerID, result, true, true, false, null, null, query, null);
            var element = $('.main-tab-content-item:visible');
            $(element).children('iframe').attr('src', 'vcr/1_9_0/Pages/CustomerDetail.html' + query);
        });
    },
    openNewContactNote: function (contactID, noteID) {
        var orgname;
        var query = "?user=" + contactID + "&noteid=" + noteID;
        mainFrame.Ts.Services.Users.GetShortNameFromID(contactID, function (result) {
            this.Ts.MainPage.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.Contact, contactID, result, true, true, false, null, null, query, null);
            var element = $('.main-tab-content-item:visible');
            $(element).children('iframe').attr('src', 'vcr/1_9_0/Pages/ContactDetail.html' + query);
        });
    },
    closeNewCustomer: function (customerID) {
        var div = $('.main-tab-content .main-Customer-' + customerID);
        div.remove();
    },
    closeNewCustomerTab: function (customerID) {
        var tab = this.MainTabs.find(customerID, Ts.Ui.Tabs.Tab.Type.Company);
        if (tab) {
            this.closeTab(tab);
            tab.remove();
        }
    },
    openNewContact: function (contactID, orgID) {
        var query = "?user=" + contactID;
        mainFrame.Ts.Services.Users.GetShortNameFromID(contactID, function (result) {
            this.Ts.MainPage.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.Contact, contactID, result, true, true, false, null, null, query, null);
        });
        Ts.Services.Settings.WriteUserSetting('SelectedUserID', contactID, function () {
            Ts.Services.Settings.WriteUserSetting('SelectedUserTabIndex', 0, function () {
                //self.MainMenu.find('mniUsers', 'users').select();
                var element = $('.main-tab-content-item:visible');
                var contentFrame = $(element).children('iframe')[0];
                if (contentFrame && contentFrame.contentWindow.refreshData) {
                    contentFrame.contentWindow.refreshData;
                }
            });
        });

    },
    closeNewContact: function (contactID) {
        var div = $('.main-tab-content .main-Contact-' + contactID);
        div.remove();
    },
    closeNewContactTab: function (contactID) {
        var tab = this.MainTabs.find(contactID, Ts.Ui.Tabs.Tab.Type.Contact);
        if (tab) {
            this.closeTab(tab);
            tab.remove();
        }
    },

    newAsset: function (tab, orgID) {
        var query;
        if (tab != undefined)
            query = "?open=" + tab + "&organizationid=" + orgID;
        this.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.NewAsset, 'newAsset', 'Add Asset', true, true, true, null, null, query, null);
    },
    closenewAssetTab: function () {
        var tab = this.MainTabs.find('newAsset', Ts.Ui.Tabs.Tab.Type.NewAsset);
        if (tab) {
            this.closeTab(tab);
            tab.remove();
        }
    },
    openNewAsset: function (assetID) {
        var query = "?assetid=" + assetID;
        mainFrame.Ts.Services.Assets.GetShortNameFromID(assetID, function (result) {
            this.Ts.MainPage.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.Asset, assetID, result, true, true, false, null, null, query, null);
        });

    },
    closeNewAsset: function (assetID) {
        var div = $('.main-tab-content .main-Asset-' + assetID);
        div.remove();
    },
    closeNewAssetTab: function (assetID) {
        var tab = this.MainTabs.find(assetID, Ts.Ui.Tabs.Tab.Type.Asset);
        if (tab) {
            this.closeTab(tab);
            tab.remove();
        }
    },
    newProduct: function (tab, productID) {
        var query;
        if (tab != undefined)
            query = "?open=" + tab + "&productid=" + productID;
        this.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.NewProduct, 'newProduct', 'Add Product', true, true, true, null, null, query, null);
    },
    closenewProductTab: function () {
        var tab = this.MainTabs.find('newProduct', Ts.Ui.Tabs.Tab.Type.NewProduct);
        if (tab) {
            this.closeTab(tab);
            tab.remove();
        }
    },
    openNewProduct: function (productID) {
        var query = "?productid=" + productID;
        mainFrame.Ts.Services.Products.GetShortNameFromID(productID, function (result) {
            this.Ts.MainPage.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.Product, productID, result, true, true, false, null, null, query, null);
        });

    },
    closeNewProduct: function (productID) {
        var div = $('.main-tab-content .main-Product-' + productID);
        div.remove();
    },
    closeNewProductTab: function (productID) {
        var tab = this.MainTabs.find(productID, Ts.Ui.Tabs.Tab.Type.Product);
        if (tab) {
            this.closeTab(tab);
            tab.remove();
        }
    },

    newProductVersion: function (tab, orgID) {
        var query;
        if (tab != undefined)
            query = "?open=" + tab + "&organizationid=" + orgID;
        this.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.NewProductVersion, 'newProductVersion', 'Add Product Version', true, true, true, null, null, query, null);
    },
    openNewProductVersion: function (productVersionID) {
        var query = "?productversionid=" + productVersionID;
        mainFrame.Ts.Services.Products.GetVersionShortNameFromID(productVersionID, function (result) {
            this.Ts.MainPage.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.ProductVersion, productVersionID, result, true, true, false, null, null, query, null);
        });

    },
    closeNewProductVersion: function (productVersionID) {
        var div = $('.main-tab-content .main-Product-Version-' + productVersionID);
        div.remove();
    },
    closeNewProductVersionTab: function (productVersionID) {
        var tab = this.MainTabs.find(productVersionID, Ts.Ui.Tabs.Tab.Type.ProductVersion);
        if (tab) {
            this.closeTab(tab);
            tab.remove();
        }
    },

    newProductFamily: function (tab, productFamilyID) {
        var query;
        if (tab != undefined)
            query = "?open=" + tab + "&productfamilyid=" + productFamilyID;
        this.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.NewProductFamily, 'newProductFamily', 'Add Product Line', true, true, true, null, null, query, null);
    },
    closenewProductFamilyTab: function () {
        var tab = this.MainTabs.find('newProductFamily', Ts.Ui.Tabs.Tab.Type.NewProductFamily);
        if (tab) {
            this.closeTab(tab);
            tab.remove();
        }
    },
    openNewProductFamily: function (productFamilyID) {
        var query = "?productfamilyid=" + productFamilyID;
        mainFrame.Ts.Services.Products.GetFamilyShortNameFromID(productFamilyID, function (result) {
            if (result == 'N/A') {
                alert('This product family has been deleted.');
                closeNewProductFamilyTab(productFamilyID);
            }
            else {
                this.Ts.MainPage.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.ProductFamily, productFamilyID, result, true, true, false, null, null, query, null);
            }
        });

    },
    closeNewProductFamily: function (productFamilyID) {
        var div = $('.main-tab-content .main-Product-Family-' + productFamilyID);
        div.remove();
    },
    closeNewProductFamilyTab: function (productFamilyID) {
        var tab = this.MainTabs.find(productFamilyID, Ts.Ui.Tabs.Tab.Type.ProductFamily);
        if (tab) {
            this.closeTab(tab);
            tab.remove();
        }
    },

    newTask: function (taskParentID, parentTaskName, noteID) {
        var query;
        if (taskParentID != undefined)
            query = "?taskparentid=" + taskParentID + "&parenttaskname=" + encodeURI(parentTaskName);
        if (noteID != undefined)
            query = "?noteid=" + noteID;

        this.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.NewTask, 'newTask', 'Add Task', true, true, true, null, null, query, null);
    },
    newTaskFromSource: function (refType, refID, ticketName, ticketNumber)
    {
        var query;
        if (refType && refID) {
            var encodedTicketName = encodeURI(ticketName);
            query = "?reftype=" + refType + "&refid=" + refID + "&ticketname=" + encodedTicketName + "&ticketnumber=" + ticketNumber;
            this.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.NewTaskFromSource, 'newTask', 'Add Task', true, true, true, null, null, query, null);
        };
    },
    closenewTaskTab: function () {
        var tab = this.MainTabs.find('newTask', Ts.Ui.Tabs.Tab.Type.NewTask);
        if (tab) {
            this.closeTab(tab);
            tab.remove();
            var div = $('.main-tab-content .main-ticket-newTask');
            if (div) {
                div.remove();
            }
        }
        else {
            tab = this.MainTabs.find('newTask', Ts.Ui.Tabs.Tab.Type.NewTaskFromSource);
            if (tab) {
                this.closeTab(tab);
                tab.remove();
                var div = $('.main-tab-content .main-ticket-newTask');
                if (div) {
                    div.remove();
                }
            }
        }
    },
    openNewTask: function (taskID) {
        var query = "?taskID=" + taskID;
        mainFrame.Ts.Services.Task.GetShortNameFromID(taskID, function (result) {
            this.Ts.MainPage.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.Task, taskID, result, true, true, false, null, null, query, null);
        });
    },
    //closeNewTask: function (reminderID) {
    //    var div = $('.main-tab-content .main-Task-' + reminderID);
    //    div.remove();
    //},
    closeNewTaskTab: function (reminderID) {
        var tab = this.MainTabs.find(reminderID, Ts.Ui.Tabs.Tab.Type.Task);
        if (tab) {
            this.closeTab(tab);
            tab.remove();
        }
    },

    AppNotify: function (title, message, options) {
        if (options == null)
            options = "info";

        var self = this;
        var TeamSupportLogo = "https://app.teamsupport.com/images/icons/TeamSupportLogo16.png";
        var ticketNumber = "";

        if (title.length > 0 && title.toLowerCase().indexOf("ticket ") == 0) {
            ticketNumber = title.replace(/ticket /i, '');
        }

        // Let's check if the browser supports notifications
        if (!("Notification" in window)) {
            $.pnotify({
                title: title,
                text: message,
                type: options,
                icon: 'ui-icon ui-icon-lightbulb',
                sticker: false
            });
            // Let's check whether notification permissions have already been granted
        } else if (Notification.permission === "granted") {
            var options = {
                body: message,
                icon: TeamSupportLogo,
                iconUrl: TeamSupportLogo,
                tag: title
            }

            if (ticketNumber.length > 0) {
                Ts.Services.Tickets.GetTicketName(ticketNumber, function (name) {
                    title = title + ' - ' + name;

                    var notification = new Notification(title, options);
                    notification.onshow = function () { setTimeout(function () { notification.close(); }, 5000) };

                    notification.onclick = function () {
                        notification.close();
                        self.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.Ticket, ticketNumber, 'Ticket: ' + ticketNumber, true, true, false, null, null, null, name);
                    };
                });
            } else {
                var notification = new Notification(title, options);
                notification.onshow = function () { setTimeout(function () { notification.close(); }, 5000) };
            }

            // Otherwise, we need to ask the user for permission
        } else if (Notification.permission !== 'denied') {
            Notification.requestPermission(function (permission) {
                if (Notification.permission === "granted") {
                    var options = {
                        body: message,
                        icon: TeamSupportLogo,
                        iconUrl: TeamSupportLogo,
                        tag: title
                    }

                    if (ticketNumber.length > 0) {
                        Ts.Services.Tickets.GetTicketName(ticketNumber, function (name) {
                            title = title + ' - ' + name;

                            var notification = new Notification(title, options);
                            notification.onshow = function () { setTimeout(function () { notification.close(); }, 5000) };

                            notification.onclick = function () {
                                notification.close();
                                Ts.Services.Tickets.GetTicketName(ticketNumber, function (name) {
                                    self.MainTabs.prepend(true, Ts.Ui.Tabs.Tab.Type.Ticket, ticketNumber, 'Ticket: ' + ticketNumber, true, true, false, null, null, null, name);
                                });
                            };
                        });
                    } else {
                        var notification = new Notification(title, options);
                        notification.onshow = function () { setTimeout(function () { notification.close(); }, 5000) };
                    }
                } else {
                    $.pnotify({
                        title: title,
                        text: message,
                        type: options,
                        icon: 'ui-icon ui-icon-lightbulb',
                        sticker: false
                    });
                }
            });
        }
    }

};
