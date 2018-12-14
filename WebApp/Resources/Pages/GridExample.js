/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

$(document).ready(function () {
  //var _reportID = top.Ts.Utils.getQueryValue('ReportID');

  var grid;
  var datamodel = new TeamSupport.DataModels.Example();

  var dateFormatter = function (row, cell, value, columnDef, dataContext) {
    return (value.getMonth() + 1) + "/" + value.getDate() + "/" + value.getFullYear();
  };


  var columns = [
    { id: "num", name: "#", field: "index", width: 40 },
    { id: "story", name: "Story", width: 520, field: "title" },
    { id: "date", name: "Date", field: "create_ts", width: 60, formatter: dateFormatter, sortable: true },
    { id: "points", name: "Points", field: "points", width: 60, sortable: true }
  ];

  var options = {
    rowHeight: 64,
    editable: false,
    enableAddRow: false,
    enableCellNavigation: false
  };

  var loadingIndicator = null;


  $(function () {
    grid = new Slick.Grid("#grid-container", datamodel.data, columns, options);

    grid.onViewportChanged.subscribe(function (e, args) {
      var vp = grid.getViewport();
      datamodel.ensureData(vp.top, vp.bottom);
    });

    grid.onSort.subscribe(function (e, args) {
      datamodel.setSort(args.sortCol.field, args.sortAsc ? 1 : -1);
      var vp = grid.getViewport();
      datamodel.ensureData(vp.top, vp.bottom);
    });

    datamodel.onDataLoading.subscribe(function () {
      if (!loadingIndicator) {
        loadingIndicator = $("<span class='loading-indicator'><label>Buffering...</label></span>").appendTo(document.body);
        var $g = $("#grid-container");

        loadingIndicator
            .css("position", "absolute")
            .css("top", $g.position().top + $g.height() / 2 - loadingIndicator.height() / 2)
            .css("left", $g.position().left + $g.width() / 2 - loadingIndicator.width() / 2);
      }

      loadingIndicator.show();
    });

    datamodel.onDataLoaded.subscribe(function (e, args) {
      for (var i = args.from; i <= args.to; i++) {
        grid.invalidateRow(i);
      }

      grid.updateRowCount();
      grid.render();

      loadingIndicator.fadeOut();
    });
    datamodel.setSort("create_ts", -1);
    grid.setSortColumn("date", false);

    // load the first page
    grid.onViewportChanged.notify();
  })
});


