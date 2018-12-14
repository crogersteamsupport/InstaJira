var mainFrame = getMainFrame();

$(document).ready(function () {
    var self = this;
    this.Tabs = new Ts.Ui.Tabs($('.tickets-panel-tabs')[0]);
    var tabs = this.Tabs


    var ticketTypeID = mainFrame.Ts.Utils.getQueryValue('tickettypeid', window);
    var userID = mainFrame.Ts.Utils.getQueryValue('userid', window);
    var groupID = mainFrame.Ts.Utils.getQueryValue('groupid', window);
    var organizationID = mainFrame.Ts.Utils.getQueryValue('organizationid', window);
    var productID = mainFrame.Ts.Utils.getQueryValue('productid', window);
    var versionID = mainFrame.Ts.Utils.getQueryValue('versionid', window);
    var isKB = mainFrame.Ts.Utils.getQueryValue('iskb', window);
    var contactID = mainFrame.Ts.Utils.getQueryValue('contactid', window);

    var filter = new mainFrame.TeamSupport.Data.TicketLoadFilter();
    var url = 'TicketGrid.html?';
    var loggingSection = '';

    if (ticketTypeID) {
        mainFrame.Ts.Services.Tickets.GetTicketType(ticketTypeID, function (ticketType) {
            filter.TicketTypeID = ticketTypeID;
            filter.UserID = mainFrame.Ts.System.User.UserID;
            filter.IsClosed = false;
            tabs.add(true, 'tickettab', 'my', 'My ' + ticketType.Name, false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
            delete filter.UserID;
            tabs.add(true, 'tickettab', 'open', 'Open', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
            filter.IsClosed = true;
            tabs.add(true, 'tickettab', 'closed', 'Closed', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
            filter.IsClosed = false;
            filter.UserID = -1;
            tabs.add(true, 'tickettab', 'unassigned', 'Unassigned', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
            delete filter.IsClosed;
            delete filter.UserID;
            tabs.add(true, 'tickettab', 'all', 'All', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
            loggingSection = 'Ticket Types';
            afterLoad();
        });
    }
    else if (userID) {
        filter.UserID = userID;
        filter.IsClosed = false;
        tabs.add(true, 'tickettab', 'open', 'Open', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
        filter.GroupID = -1;
        tabs.add(true, 'tickettab', 'groups', 'My Groups', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
        delete filter.GroupID;
        filter.IsSubscribed = true;
        delete filter.UserID;
        delete filter.IsClosed;
        tabs.add(true, 'tickettab', 'subscribed', 'Subscribed', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
        delete filter.IsSubscribed;
        filter.IsFlagged = true;
        tabs.add(true, 'tickettab', 'flagged', 'Flagged', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
        delete filter.IsFlagged;
        filter.UserID = userID;
        filter.IsClosed = true;
        tabs.add(true, 'tickettab', 'closed', 'Closed', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
        delete filter.IsClosed;
        tabs.add(true, 'tickettab', 'all', 'All', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
        var queueFilter = new mainFrame.TeamSupport.Data.TicketLoadFilter();
        queueFilter.IsEnqueued = true;
        queueFilter.ViewerID = userID;
        tabs.add(true, 'tickettab', 'queue', 'Queue', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(queueFilter));

        if (mainFrame.Ts.System.Organization.ProductType != mainFrame.Ts.ProductType.Enterprise) {
            tabs.add(true, 'tickettab', 'reminders', 'Reminders', false, false, false, '', '', 'Reminders.html?UserID=' + userID);
        }
        loggingSection = 'My Tickets';
        afterLoad();
    }
    else if (groupID) {

    }
    else if (organizationID) {

    }
    else if (productID) {

    }
    else if (versionID) {

    }
    else if (isKB) {

    }
    else if (contactID) {
        filter.UserID = mainFrame.Ts.System.User.UserID;
        filter.ContactID = contactID;
        filter.IsClosed = false;
        tabs.add(true, 'tickettab', 'my', 'All My Tickets', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
        delete filter.UserID;
        filter.ContactID = contactID;
        filter.IsClosed = false;
        tabs.add(true, 'tickettab', 'open', 'All Open Tickets', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
        filter.IsClosed = true;
        tabs.add(true, 'tickettab', 'closed', 'All Closed Tickets', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
        filter.IsClosed = false;
        filter.UserID = -1;
        tabs.add(true, 'tickettab', 'unassigned', 'All Unassigned Tickets', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
        delete filter.IsClosed;
        delete filter.UserID;
        tabs.add(true, 'tickettab', 'all', 'All Tickets', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
        //loggingSection = 'All Tickets';

        afterLoad();
    }
    else {
        filter.IsClosed = false;
        tabs.add(true, 'tickettab', 'open', 'Open', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
        filter.IsClosed = true;
        tabs.add(true, 'tickettab', 'closed', 'Closed', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
        filter.IsClosed = false;
        filter.UserID = -1;
        tabs.add(true, 'tickettab', 'unassigned', 'Unassigned ', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
        delete filter.IsClosed;
        delete filter.UserID;
        tabs.add(true, 'tickettab', 'all', 'All', false, false, false, '', '', url + mainFrame.Ts.Utils.ticketFilterToQuery(filter));
        loggingSection = 'All Tickets';

        afterLoad();
    }

    function afterLoad() {
        self.Tabs.bind('afterSelect', function (tab) {
            mainFrame.Ts.System.logAction('Ticket Tabs (' + loggingSection + ') - Viewed ' + tab.getCaption());
            $('.tickets-panel-grid .tickets-tab-content').hide();
            var div = $('.tickets-panel-grid .tickets-tab-' + tab.getId());
            if (div.length < 1) {
                div = $('<div class="tickets-tab-content tickets-tab-' + tab.getId() + '"><iframe class="tickets-grid-iframe" src="' + tab.getData() + '" scrolling="no" frameborder="0" height="100%" width="100%"></iframe></div>').appendTo('.tickets-panel-grid');
            }
            div.show();
            mainFrame.Ts.Services.Settings.WriteUserSetting('TicketGrid-tab-' + window.location.search, tab.getId());

            var frame = $('.tickets-grid-iframe:visible')[0];
            if (frame && frame.contentWindow.onShow) frame.contentWindow.onShow();
        });


        $('.ts-loading').hide().next().show();
        $('.tickets-layout').layout({
            resizeNestedLayout: true,
            maskIframesOnResize: true,
            defaults: {
                spacing_open: 0,
                closable: false
            },
            center: {
                paneSelector: ".tickets-panel-grid"
            },
            north: {
                paneSelector: ".tickets-panel-tabs",
                size: 31
            }
        });
        //tabs.getByIndex(0).select();
        mainFrame.Ts.Services.Settings.ReadUserSetting('TicketGrid-tab-' + window.location.search, tabs.getByIndex(0).getId(), function (result) {
            tabs.find(result, 'tickettab').select();


        });


    }


});

function onShow() {
    var frame = $('.tickets-grid-iframe:visible')[0];
    if (frame && frame.contentWindow.onShow) frame.contentWindow.onShow();

}

/*
Ts.Pages.Tickets.prototype =
{
  constructor: Ts.Pages.Tickets,
  refresh: function () {
    var tab = this.Tabs.getSelected();
    if (tab) {
      var data = tab.getData();
      if (data == 'external') {
        var contentFrame = $(this.Element).find('.tickets-panel-grid .tickets-tab-' + tab.getId()).find('iframe')[0];
        try { if (contentFrame.contentWindow.refreshData) contentFrame.contentWindow.refreshData(); } catch (err) { }
        try { if (contentFrame.contentWindow.onShow) contentFrame.contentWindow.onShow(); } catch (err) { }
      }
      else {
        var grid = tab.getData();
        grid.refresh();
      }

    }
  }


};

Ts.Pages.Tickets.Tab = function (caption, ticketLoadFilter, externalUrl) {
  this.Caption = caption;
  this.TicketLoadFilter = ticketLoadFilter;
  this.ExternalUrl = externalUrl;
  //TeamSupport.Data.TicketLoadFilter()
}

*/