function TicketWidget(ticket)
{
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

    var _mainFrame = getMainFrame();
    var self = this;
    this.ticket = ticket;

    this.getTicket = function (callback)
    {
        callback(this.ticket);
    }

    this.getCustomFields = function (callback, forceRefresh)
    {
        if (this.customFields && forceRefresh !== true) {
            if (callback) callback(this.customFields);
        }
        else {
            _mainFrame.Ts.Services.TicketPage.GetPluginTicketCustomFields(this.ticket.TicketID, function (result) {
                this.customFields = JSON.parse(result);
                if (callback) callback(this.customFields);
            });
        }
    }

    this.getUser = function (callback, forceRefresh) {
        if (this.user && forceRefresh !== true) {
            if (callback) callback(this.user);
        }
        else {
            _mainFrame.Ts.Services.TicketPage.GetPluginTicketUser(this.ticket.TicketID, function (result) {
                this.user = JSON.parse(result);
                if (callback) callback(this.user);
            });
        }
    }

    this.getActions = function (callback, forceRefresh) {
        if (this.actions && forceRefresh !== true) {
            if (callback) callback(this.actions);
        }
        else {
            _mainFrame.Ts.Services.TicketPage.GetPluginTicketActions(this.ticket.TicketID, function (result) {
                this.actions = JSON.parse(result);
                if (callback) callback(this.actions);
            });
        }
    }

    this.getCustomers = function (callback, forceRefresh) {
        if (this.customers && forceRefresh !== true)
        {
            if (callback) callback(this.customers);
        }
        else {
            _mainFrame.Ts.Services.TicketPage.GetPluginTicketCustomers(this.ticket.TicketID, function (result) {
                this.customers = JSON.parse(result);
                if (callback) callback(this.customers);
            });
        }
    }

    this.getContacts = function (callback, forceRefresh) {
        if (this.contacts && forceRefresh !== true) {
            if (callback) callback(this.contacts);
        }
        else {
            _mainFrame.Ts.Services.TicketPage.GetPluginTicketContacts(this.ticket.TicketID, function (result) {
                this.contacts = JSON.parse(result);
                if (callback) callback(this.contacts);
            });
        }
    }

    this.loadScript = function(src, callback) {
        $.getScript(src, function () {
            if (callback) callback();
        });
    }
}
