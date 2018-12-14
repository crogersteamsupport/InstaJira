$(document).ready(function () {
    //var _reportID = parent.Ts.Utils.getQueryValue('ReportID', window);
    //var _scheduledReportID = parent.Ts.Utils.getQueryValue('ScheduledReportID', window);
    //var _report = null;

    function loadChart() {

        //parent.Ts.Utils.webMethod("ReportService", "GetReport", { "reportID": _reportID }, function (report) {
        //    _report = report;
        //    _report.Def = JSON.parse(report.ReportDef);

        //    parent.Ts.Utils.webMethod(null, "reportdata/chart",
        //      { "reportID": _reportID },
        //      function (data) {
        //          createChart('.chart-container', _report.Def.Chart, data);
        //          var svg = $('.chart-container').highcharts().getSVG();
        //          //vv exportChart('image/png');
        //      },
        //      function (error) {
        //          showChartError('.chart-container', error.statusText);
        //      });
        //});
    }

    function exportChart(exportType) {
        var options = { type: exportType, url: '../../../chartexport', filename: _report.Name, width: 1900 };
        $('.chart-container').highcharts().exportChart(options);
    }

    //loadChart();
});