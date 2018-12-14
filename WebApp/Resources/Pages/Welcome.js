/// <reference path="ts/ts.js" />
/// <reference path="ts/parent.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

var welcomePage = null;
$(document).ready(function () {
    welcomePage = new WelcomePage();
    welcomePage.refresh();
});

function onShow() {
    welcomePage.refresh();
};


WelcomePage = function () {
    $('#LoggedInUser').html(parent.Ts.System.User.FirstName);

    $('#SupportHub').click(function (e) {
        e.preventDefault();
        parent.mainFrame.Ts.System.openSupport();
    });

    $('#ChatOnline').click(function (e) {
        e.preventDefault();
        window.open('https://app.teamsupport.com/Chat/ChatInit.aspx?uid=22bd89b8-5162-4509-8b0d-f209a0aa6ee9', 'TSChat', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=450,height=500');
    });

    $('.welcome-hide').click(function (e) {
        e.preventDefault();
        top.Ts.MainPage.hideWelcome();
    });
};



WelcomePage.prototype = {
    constructor: WelcomePage,
    refresh: function () {

    }
};
