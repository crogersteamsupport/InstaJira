$(document).ready(function () {
    var originalLeave = $.fn.popover.Constructor.prototype.leave;
    $.fn.popover.Constructor.prototype.leave = function (obj) {
        var self = obj instanceof this.constructor ?
            obj : $(obj.currentTarget)[this.type](this.getDelegateOptions()).data('bs.' + this.type)
        var container, timeout;

        originalLeave.call(this, obj);

        if (obj.currentTarget) {

            container = $(obj.currentTarget).siblings('.popover')
            timeout = self.timeout;
            container.one('mouseenter', function () {
                //We entered the actual popover – call off the dogs
                clearTimeout(timeout);
                //Let's monitor popover content instead
                container.one('mouseleave', function () {
                    $.fn.popover.Constructor.prototype.leave.call(self, self);
                });
            })
        }
    };

    $("body").on("mouseenter", ".UserAnchor", function (event) {
        var e = $(this);
        e.unbind('hover');
        var userid = e.data('userid');
        var ticketid = e.data('ticketid');
        e.popover({
            html: true,
            //container: 'body',
            trigger: 'hover',
            delay: { "show": 1, "hide": 1 },
            content: function () {
                return $.ajax({
                    url: '../../../Tips/User.aspx?UserID=' + userid + '&TicketID=' + ticketid,
                    dataType: 'html',
                    async: false
                }).responseText;
            }
        }).popover('show');
    })
  .on("click", ".UserAnchor", function (event) {
      var self = $(this);
      var userid = self.data('userid');
      top.Ts.MainPage.openUser(userid);
  })

    $("body").on("mouseenter", ".ProductAnchor", function (event) {
        var e = $(this);
        e.unbind('hover');
        var ticketid = e.data('ticketid');
        var productid = e.data('productid');
        var b = e.closest("label");
        e.popover({
            html: true,
            container: 'body',
            trigger: 'manual',
            delay: { "show": 1, "hide": 1000 },
            content: function () {
                return $.ajax({
                    url: '../../../Tips/Product.aspx?ProductID=' + productid + '&TicketID=' + ticketid,
                    dataType: 'html',
                    async: false
                }).responseText;
            }
        }).popover('show');
    });

    $("body").on("mouseenter", ".VersionAnchor", function (event) {
        var e = $(this);
        e.unbind('hover');
        var ticketid = e.data('ticketid');
        var versionid = e.data('versionid');
        var b = e.closest("label");
        e.popover({
            html: true,
            container: 'body',
            trigger: 'manual',
            delay: { "show": 1, "hide": 1000 },
            content: function () {
                return $.ajax({
                    url: '../../../Tips/Version.aspx?VersionID=' + versionid + '&TicketID=' + ticketid,
                    dataType: 'html',
                    async: false
                }).responseText;
            }
        }).popover('show');
    });

    $("body").on("mouseenter", ".wcTooltip", function (event) {
        var e = $(this);
        var data = e.data('info');
        e.unbind('hover');
        e.popover({
            html: true,
            container: 'body',
            trigger: 'manual',
            delay: { "show": 1, "hide": 1000 },
            placement: 'left',
            content: data
        }).popover('show');
    });

    $("body").on("mouseenter", ".AssetAnchor", function (event) {
        var e = $(this);
        e.unbind('hover');
        //var userid = e.data('userid');
        var assetid = e.data('assetid');
        e.popover({
            html: true,
            //container: '#ticket-properties-area',
            trigger: 'hover',
            delay: { "show": 1, "hide": 1 },
            content: function () {
                return $.ajax({
                    url: '../../../Tips/Asset.aspx?AssetID=' + assetid,
                    dataType: 'html',
                    async: false
                }).responseText;
            }
        }).popover('show');
    })
  .on("click", ".AssetAnchor", function (event) {
      var self = $(this);
      var assetid = self.data('assetid');
      top.Ts.MainPage.openAsset(assetid);
  })

    $("body").on("mouseenter", ".OrgAnchor", function (event) {
        var e = $(this);
        e.unbind('hover');
        var orgid = e.data('orgid');
        var ticketid = e.data('ticketid');
        e.popover({
            html: true,
            //container: 'body',
            trigger: 'hover',
            delay: { "show": 1, "hide": 1 },
            content: function () {
                return $.ajax({
                    url: '../../../Tips/Customer.aspx?CustomerID=' + orgid + '&TicketID=' + ticketid,
                    dataType: 'html',
                    async: false
                }).responseText;
            }
        }).popover('show');
    })
  .on("click", ".OrgAnchor", function (event) {
      var self = $(this);
      var orgid = self.data('orgid');
      top.Ts.MainPage.openNewCustomer(orgid);
  })

    $("body").on("mouseenter", ".tasklink", function (event) {
        //debugger;
        //var e = $(this);
        //e.unbind('hover');
        //var orgid = e.data('orgid');
        //var ticketid = e.data('ticketid');
        //e.popover({
        //    html: true,
        //    //container: 'body',
        //    trigger: 'hover',
        //    delay: { "show": 1, "hide": 1 },
        //    content: function () {
        //        return $.ajax({
        //            url: '../../../Tips/Customer.aspx?CustomerID=' + orgid + '&TicketID=' + ticketid,
        //            dataType: 'html',
        //            async: false
        //        }).responseText;
        //    }
        //}).popover('show');
    })
  .on("click", ".tasklink", function (e) {
      e.preventDefault();
      var id = $(this).data('taskID');
      parent.Ts.System.logAction('Ticket Page - View Task');
      parent.Ts.MainPage.openNewTask(id);
  })

    $("body").on("mouseenter", ".SLAAnchor", function (event) {
        var e = $(this);
        e.unbind('hover');
        var ticketid = e.data('ticketid');
        e.popover({
            html: true,
            //container: 'body',
            trigger: 'hover',
            delay: { "show": 1, "hide": 1 },
            content: function () {
                return $.ajax({
                    url: '../../../Tips/Sla.aspx?TicketID=' + ticketid,
                    dataType: 'html',
                    async: false
                }).responseText;
            }
        }).popover('show');
        resetSLAInfo();
    });

    $("body").on("mouseenter", ".TicketAnchor", function (event) {
        var e = $(this);
        e.unbind('hover');
        var ticketid = e.data('ticketid');
        e.popover({
            html: true,
            //container: 'body',
            trigger: 'hover',
            delay: { "show": 1, "hide": 1 },
            content: function () {
                return $.ajax({
                    url: '../../../Tips/Ticket.aspx?TicketID=' + ticketid,
                    dataType: 'html',
                    async: false
                }).responseText;
            }
        }).popover('show');
    })
  .on("click", ".TicketAnchor", function (event) {
      var self = $(this);
      var ticketid = self.data('ticketid');
      top.Ts.MainPage.openTicketByID(ticketid, true);
  })

$('body').on('click', function (e) {
    $("*").each(function () {
        // Bootstrap sets a data field with key `bs.popover` on elements that have a popover.
        // Note that there is no corresponding **HTML attribute** on the elements so we cannot
        // perform a search by attribute.
        var popover = $.data(this, "bs.popover");
        if (popover)
            $(this).popover('hide');
    });
});
    $('body').on('click', function (e) {
        $('.ProductAnchor').each(function () {
            //the 'is' for buttons that trigger popups
            //the 'has' for icons within a button that triggers a popup
            if (!$(this).is(e.target) && $(this).has(e.target).length === 0 && $('.popover').has(e.target).length === 0) {
                $(this).popover('hide');
            }
        });
    });

});