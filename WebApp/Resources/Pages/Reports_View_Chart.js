$(document).ready(function () {
    var _reportID = parent.Ts.Utils.getQueryValue('ReportID', window);
    var _report = null;
    $('.reports-edit').hide();
    if (parent.Ts.System.User.DisableExporting == true) { $('.reports-export').remove(); }

    $('.btn-group [data-toggle="tooltip"]').tooltip({ placement: 'bottom', container: 'body' });


    function loadChart() {
        parent.Ts.Utils.webMethod("ReportService", "GetReport", { "reportID": _reportID }, function (report) {
            _report = report;
            _report.Def = JSON.parse(report.ReportDef);

            if (report.IsFavorite) {
                $('.reports-fav i').removeClass('fa-star-o').addClass('fa-star');
            }

            $('#mainFrame').attr('src', report.ReportDef);
            $('.report-title').text(report.Name);

            if ((parent.Ts.System.User.IsSystemAdmin != false || report.CreatorID == parent.Ts.System.User.UserID) && report.OrganizationID != null) {
                $('.reports-edit').show();
            }

            if (report.OrganizationID == null && (parent.Ts.System.User.UserID == 34 || parent.Ts.System.User.UserID == 43 || parent.Ts.System.User.UserID == 47)) {
                $('.reports-edit').show();
            }


            parent.Ts.Utils.webMethod(null, "reportdata/chart",
              { "reportID": _reportID },
              function (data) {
                  
                  createChart('.chart-container', _report.Def.Chart, data);
              },
              function (error) {
                  showChartError('.chart-container', error.statusText);
              });

        });
    }

    $('.reports-refresh').click(function (e) {
        e.preventDefault();
        loadChart();
    });

    $('.reports-edit').click(function (e) {
        e.preventDefault();
        window.location.assign("reports_edit.html?ReportID=" + _reportID);
    });

    $('.reports-export-png').click(function (e) {
        e.preventDefault();
        exportChart('image/png');
    });

    $('.reports-export-jpg').click(function (e) {
        e.preventDefault();
        exportChart('image/jpeg');
    });

    $('.reports-export-svg').click(function (e) {
        e.preventDefault();
        exportChart('image/svg+xml');
    });

    function exportChart(exportType) {
        var options = { type: exportType, url: '../../../chartexport', filename: '"' + _report.Name + '"' , width: 1900 };
        $('.chart-container').highcharts().exportChart(options);
    }

    $('.reports-fav').click(function (e) {
        e.preventDefault();
        _report.IsFavorite = !_report.IsFavorite;
        parent.Ts.Utils.webMethod("ReportService", "SetFavorite", { "reportID": _reportID, "value": _report.IsFavorite }, function () {
            if (_report.IsFavorite) $('.reports-fav i').removeClass('fa-star-o').addClass('fa-star'); else $('.reports-fav i').removeClass('fa-star').addClass('fa-star-o');
        });
    });

    $('.reports-schedule').click(function (e) {
        e.preventDefault();
        var button = $(this);
        if (button.hasClass('disabled')) return;

        window.location.assign("Reports_Schedule.html?ReportId=" + _reportID + "&ReportName=" + _report.Name + "&ReportTypeOpened=" + _report.ReportType);
    });

    loadChart();

});
