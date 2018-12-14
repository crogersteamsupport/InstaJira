
$(document).ready(function () {
    var _reportID = parent.Ts.Utils.getQueryValue('ReportID', window);
    var _report = null;
    $('.reports-edit').hide();
    $('.btn-group [data-toggle="tooltip"]').tooltip({ placement: 'bottom', container: 'body' });

    parent.Ts.Utils.webMethod("ReportService", "GetReport", { "reportID": _reportID }, function (report) {
        _report = report;

        if ((parent.Ts.System.User.IsSystemAdmin != false || report.CreatorID == parent.Ts.System.User.UserID) && report.OrganizationID != null) {
            $('.reports-edit').show();
        }

        if (report.OrganizationID == null && (parent.Ts.System.User.UserID == 34 || parent.Ts.System.User.UserID == 43 || parent.Ts.System.User.UserID == 47)) {
            $('.reports-edit').show();
        }

        if (report.IsFavorite) {
            $('.reports-fav i').removeClass('fa-star-o').addClass('fa-star');
        }

        $('#mainFrame').attr('src', report.ReportDef);
        $('.report-title').text(report.Name);
    });

    $('.reports-refresh').click(function (e) {
        e.preventDefault();
        window.location.assign(window.location.href);
    });

    $('.reports-edit').click(function (e) {
        e.preventDefault();
        window.location.assign("reports_edit.html?ReportID=" + _reportID);
    });

    $('.reports-fav').click(function (e) {
        e.preventDefault();
        _report.IsFavorite = !_report.IsFavorite;
        parent.Ts.Utils.webMethod("ReportService", "SetFavorite", { "reportID": _reportID, "value": _report.IsFavorite }, function () {
            if (_report.IsFavorite) $('.reports-fav i').removeClass('fa-star-o').addClass('fa-star'); else $('.reports-fav i').removeClass('fa-star').addClass('fa-star-o');
        });
    });



    $('body').layout({
        resizeNestedLayout: true,
        maskIframesOnResize: true,
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

});


