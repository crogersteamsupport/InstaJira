$(document).ready(function () {
    var _layout = null;
    var _reportID = parseInt(parent.Ts.Utils.getQueryValue('ReportID', window));
    var _grid;
    var datamodel = new TeamSupport.DataModel(getReportData);
    var _report = null;
    $('.reports-edit').hide();
    if (parent.Ts.System.User.DisableExporting == true) { $('.reports-export').remove(); }

    function getReportData(from, to, sortcol, isdesc, callback) {
        var params = { "reportID":
        _reportID,
            "from": from,
            "to": to,
            "sortField": sortcol,
            "isDesc": isdesc,
            "useUserFilter": true
        };

        req = $.ajax({
            type: "POST",
            url: "/reportdata/table",
            data: JSON.stringify(params),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: callback,
            error: function (xhr, status, error) { }
        });

        return req;

    }

    $('.btn-group .toolitp').tooltip({ placement: 'bottom', container: 'body' });

    parent.Ts.Utils.webMethod("ReportService", "GetReport", {
        "reportID": _reportID
    }, function (report) {
        _report = report;
        _report.Def = JSON.parse(report.ReportDef);
        _report.Settings = report.UserSettings == '' ? new Object() : JSON.parse(report.UserSettings);

        //if (_report.ReportType == 3) $('.btn.reports-filter').remove();

        if (report.IsFavorite) {
            $('.reports-fav i').removeClass('fa-star-o').addClass('fa-star');
        }
        $('.report-title').text(report.Name);

        switch (report.ReportType) {
            case 0:
                $('.reports-header i').addClass('fa-table color-red');
                break;
            case 1:
                $('.reports-header i').addClass('fa-bar-chart-o color-green');
                break;
            case 2:
                $('.reports-header i').addClass('fa-globe color-blue');
                break;
            case 3:
                $('.reports-header i').addClass('fa-wrench color-darkorange');
                break;
            case 4:
                $('.reports-header i').addClass('fa-tasks color-amber');
                break;
            default:
        }

        if (_report.Settings && _report.Settings.Filters && _report.Settings.Filters.length == 1) {
            $('.reports-filter i').addClass('color-red');
        }

        if ((parent.Ts.System.User.IsSystemAdmin != false || report.CreatorID == parent.Ts.System.User.UserID) && report.ReportType != 3 && report.OrganizationID != null) {
            $('.reports-edit').show();
        }

        if (report.OrganizationID == null && (parent.Ts.System.User.UserID == 34 || parent.Ts.System.User.UserID == 43 || parent.Ts.System.User.UserID == 47) && report.ReportType != 3) {
            $('.reports-edit').show();
        }

        parent.Ts.Utils.webMethod("ReportService", "GetReportColumns", {
            "reportID": _reportID
        }, function (repCols) {
            //var idx = new Object();idx.id = "index";idx.name = "index";idx.field = "index";columns.push(idx);

            if (report.ReportType == 3) {
                filterFields = [];
                for (var i = 0; i < repCols.length; i++) {
                    filterField = new Object();
                    filterField.ID = repCols[i].Name;
                    repCols[i].FieldID = repCols[i].Name;
                    filterField.Name = repCols[i].Name;
                    filterField.Table = "Report Fields";
                    filterField.LookupTableID = null;
                    filterField.ListValues = []
                    filterField.IsCustom = false;
                    filterField.IsPrimary = true;
                    filterField.AuxName = "";
                    switch (repCols[i].DataType) {
                        case "bit": filterField.DataType = "bool"; break;
                        case "float": filterField.DataType = "number"; break;
                        case "int": filterField.DataType = "number"; break;
                        default:
                            filterField.DataType = repCols[i].DataType;
                    }

                    filterFields.push(filterField);
                }
                $('#filter-user').reportFilter({ "fields": filterFields });
                $('.global-filter-tab').remove();
            }
            else if (_report.Def.Subcategory) {
                parent.Ts.Services.Reports.GetFields(_report.Def.Subcategory, function (fields) {
                    $('#filter-user').reportFilter({ "fields": fields });
                    if (_report.OrganizationID != null) {
                        $('#filter-global').reportFilter({ "fields": fields });
                    } else {
                        $('.global-filter-tab').remove();
                    }
                });
            }

            function findRepCol(id) {
                for (var i = 0; i < repCols.length; i++) {
                    var repCol = repCols[i];
                    if (repCol.Name == id) {
                        repCols.splice(i, 1);
                        return repCol;
                    }
                }
                return null;
            }

            var columns = new Array();

            function addRepCol(repCol) {
                var column = new Object();
                column.id = repCol.Name;
                column.name = repCol.Name.replace(/[_]/g, ' ');
                column.field = repCol.Name;
                column.width = repCol.Width || 250;
                column.sortable = true;
                column.fieldID = repCol.FieldID;
                column.isCustomField = repCol.IsCustomField;
                var low = repCol.Name.toLowerCase().replace(/[_ ]/g, '');
                if (repCol.DataType == "datetime") {
                    column.formatter = dateTimeFormatter;
                }
                else if (repCol.DataType == "date") {
                    column.formatter = dateFormatter;
                } else if (repCol.DataType == "float") {
                    column.formatter = floatFormatter;
                } else if (repCol.DataType == "bit") {
                    column.formatter = bitFormatter;
                } else if (repCol.IsEmail == true) {
                    column.formatter = emailFormatter;
                } else if (repCol.IsLink == true) {
                    column.formatter = linkFormatter;
                } else if (repCol.IsOpenable == true) {
                    column.formatter = openFormatter;
                    column.openField = repCol.OpenField;
                } else if (low == 'ticketnumber') {
                    column.formatter = ticketNumberFormatter;
                } else if (low == 'companyname') {
                    column.formatter = companyFormatter;
                }

                columns.push(column);
            }

            if (_report.Settings.Columns) {
                for (var i = 0; i < _report.Settings.Columns.length; i++) {
                    var userCol = _report.Settings.Columns[i];
                    var repCol = findRepCol(userCol.id);
                    if (repCol) {
                        repCol.Width = userCol.width;
                        addRepCol(repCol);
                    }
                }
            }

            for (var i = 0; i < repCols.length; i++) {
                addRepCol(repCols[i]);
            }



            initGrid(columns);
        });

    });

    $('.filter-modal').modal({
        "show": false,
        "backdrop": 'static'
    });

    $('.reports-filter').click(function (e) {
        e.preventDefault();
        $('.filter-modal').modal('show');
        if (_report.ReportType != 3 && _report.OrganizationID != null) { $('#filter-global').reportFilter("loadFilters", _report.Def.Filters); }
        if (_report.Settings.Filters) $('#filter-user').reportFilter("loadFilters", _report.Settings.Filters);
    });

    $('.filter-save').click(function (e) {
        e.preventDefault();
        if (_report.ReportType != 3 && _report.OrganizationID != null) { _report.Def.Filters = $('#filter-global').reportFilter('getObject'); }
        _report.Settings.Filters = $('#filter-user').reportFilter('getObject');
        if (_report.Settings.Filters.length == 1) {
            $('.reports-filter i').addClass('color-red');
        } else {
            $('.reports-filter i').removeClass('color-red');
        }

        parent.Ts.Utils.webMethod("ReportService", "SaveReportDef",
            {
                "reportID": _report.ReportID,
                "data": JSON.stringify(_report.Def)
            },
            function () {
                saveUserSettings(function () {
                    refresh();
                }
            );
            },
            function (error) { }
        );

        $('.filter-modal').modal('hide');

    });

    function saveUserSettings(callback) {
        parent.Ts.Utils.webMethod("ReportService", "SaveUserSettings",
            {
                "reportID": _report.ReportID,
                "data": JSON.stringify(_report.Settings)
            },
            function () { if (callback) callback(); },
            function (error) { }
        );
    }


    function saveColumns() {debugger
        var columns = _grid.getColumns();
        _report.Settings.Columns = new Array();
        _report.Settings.forceFitColumns = _grid.getOptions().forceFitColumns == true;

        for (var i = 0; i < columns.length; i++) {
            var item = new Object();
            item.id = columns[i].id;
            item.width = columns[i].width;
            _report.Settings.Columns.push(item);
        }

        saveUserSettings();
    }


    $('.reports-refresh').click(function (e) {
        e.preventDefault();
        window.location.assign(window.location.href);
    });

    $('.reports-edit').click(function (e) {
        e.preventDefault();
        window.location.assign("reports_edit.html?ReportID=" + _reportID + '&ReturnURL=' + encodeURIComponent(location.href));
    });

    $('.reports-back').click(function (e) {
        e.preventDefault();
        window.location.assign('reports.html');
    });

    $('.reports-fav').click(function (e) {
        e.preventDefault();
        _report.IsFavorite = !_report.IsFavorite;
        parent.Ts.Utils.webMethod("ReportService", "SetFavorite", {
            "reportID": _reportID,
            "value": _report.IsFavorite
        }, function () {
            if (_report.IsFavorite) $('.reports-fav i').removeClass('fa-star-o').addClass('fa-star');
            else $('.reports-fav i').removeClass('fa-star').addClass('fa-star-o');
        });
    });

    $('.reports-export-excel').click(function (e) {
        e.preventDefault();
        //'../dc/1078/reports/95'
		window.open('../../../dc/' + parent.Ts.System.Organization.OrganizationID + '/reports/' + _report.ReportID + '?Type=EXCEL', 'ReportDownload');
		return false;
    });

    $('.reports-export-csv').click(function (e) {
        e.preventDefault();
        //'../dc/1078/reports/95'
		window.open('../../../dc/' + parent.Ts.System.Organization.OrganizationID + '/reports/' + _report.ReportID + '?Type=CSV', 'ReportDownload');
		return false;
    });
    _layout = $('#reports-tabview-layout').layout({
        resizeNestedLayout: true,
        maskIframesOnResize: true,
        defaults: {
            spacing_open: 5,
            closable: false
        },
        center: {

            onresize: resizeGrid,
            triggerEventsOnLoad: false,
            minSize: 500
        },
        north: {
            size: 100,
            spacing_open: 0,
            resizable: false
        }
    });


    function resizeGrid(paneName, paneElement, paneState, paneOptions, layoutName) {
        if (loadingIndicator) {
            loadingIndicator.remove();
            loadingIndicator = null;
        }
        try {
            _grid.scrollRowIntoView(0);
            var vp = _grid.getViewport();
            datamodel.clear();
            datamodel.ensureData(vp.top, vp.bottom + 50, function () {
                _grid.resizeCanvas();
            });
        } catch (e) {

        }

    }

    var dateTimeFormatter = function (row, cell, value, columnDef, dataContext) {
        var date = dataContext[columnDef.id];
        return date ? parent.Ts.Utils.getDateString(date, true, !(_report.ReportType == 4), _report.ReportType == 3) : '';
    };

    var dateFormatter = function (row, cell, value, columnDef, dataContext) {
        var date = dataContext[columnDef.id];
        return date ? parent.Ts.Utils.getDateString(date, true, false, _report.ReportType == 3) : '';
    };

    var bitFormatter = function (row, cell, value, columnDef, dataContext) {
        return dataContext[columnDef.id] == true ? "True" : "False";
    };

    var stringFormatter = function (row, cell, value, columnDef, dataContext) {
      var tmp = document.createElement("DIV");
      tmp.innerHTML = value;
      return tmp.textContent || tmp.innerText || "";
    };
  
    var floatFormatter = function (row, cell, value, columnDef, dataContext) {
        return value;
    };

    var emailFormatter = function (row, cell, value, columnDef, dataContext) {
        return '<a href="mailto:' + dataContext[columnDef.id] + '">' + dataContext[columnDef.id] + '</a>';
    };

    var linkFormatter = function (row, cell, value, columnDef, dataContext) {
        return '<a href="' + dataContext[columnDef.id] + '" target="_blank">' + dataContext[columnDef.id] + '</a>';
    };

    var ticketNumberFormatter = function (row, cell, value, columnDef, dataContext) {
        return '<a href="#" onclick="parent.Ts.MainPage.openTicket(' + dataContext[columnDef.id] + ', true); return false;">' + dataContext[columnDef.id] + '</a>';
    }

    var companyFormatter = function (row, cell, value, columnDef, dataContext) {
        return '<a href="#" onclick="parent.Ts.MainPage.openCustomerByExactName(\'' + dataContext[columnDef.id] + '\', true); return false;">' + dataContext[columnDef.id] + '</a>';
    }


    var openFormatter = function (row, cell, value, columnDef, dataContext) {
        if (columnDef.openField == "TicketID") {
            return '<a href="#" onclick="parent.Ts.MainPage.openTicketByID(' + dataContext["hiddenTicketID"] + ', true); return false;">' + dataContext[columnDef.id] + '</a>';
        } else if (columnDef.openField == "OrganizationID") {
            return '<a href="#" onclick="parent.Ts.MainPage.openCustomer(' + dataContext["hiddenOrganizationID"] + '); return false;">' + dataContext[columnDef.id] + '</a>';
        } else if (columnDef.openField == "UserID") {
            return '<a href="#" onclick="parent.Ts.MainPage.openContact(' + dataContext["hiddenUserID"] + '); return false;">' + dataContext[columnDef.id] + '</a>';
        }
        return value;
    };

    var tmrDelayIndicator = null;
    var tmrHideLoading = null;
    var loadingIndicator = null;
    showLoadingIndicator();

    function showLoadingIndicator(delay) {
        if (!delay) {
            if (!loadingIndicator) {
                loadingIndicator = $("<div class='grid-loading'></div>").appendTo(document.body);
                loadingIndicator.position({
                    my: "center center",
                    at: "center center",
                    of: _layout.panes.center,
                    collision: "none"
                });
            }
            loadingIndicator.show();
        } else {
            if (tmrDelayIndicator) clearTimeout(tmrDelayIndicator);
            tmrDelayIndicator = setTimeout(function () {
                showLoadingIndicator();
            }, delay);
        }
        if (tmrHideLoading) {
            clearTimeout(tmrHideLoading);
        }
        tmrHideLoading = setTimeout(function () {
            hideLoadingIndicator();
        }, 60000);
    }

    function hideLoadingIndicator() {
        tmrHideLoading = null;
        if (tmrDelayIndicator) clearTimeout(tmrDelayIndicator);
        tmrDelayIndicator = null;
        if (loadingIndicator) loadingIndicator.fadeOut();
    }


    function initGrid(columns) {
        var options = {
            rowHeight: 32,
            editable: false,
            enableAddRow: false,
            enableCellNavigation: true,
            multiSelect: false,
            enableColumnReorder: true,
            forceFitColumns: false
        };

        _grid = new Slick.Grid("#reports-tabview-grid-container", datamodel.data, columns, options);

        _grid.onViewportChanged.subscribe(function (e, args) {
            var vp = _grid.getViewport();
            datamodel.ensureData(vp.top, vp.bottom);
        });

        _grid.onSort.subscribe(function (e, args) {
            datamodel.setSort(args.sortCol.field, args.sortAsc ? 1 : -1);
            _report.Settings.SortField = args.sortCol.field;
            _report.Settings.IsSortAsc = args.sortAsc;
            saveUserSettings();
            _grid.scrollRowIntoView(0);
            var vp = _grid.getViewport();
            datamodel.clear();
            datamodel.ensureData(vp.top, vp.bottom);

        });

        _grid.onColumnsReordered.subscribe(function (e, args) { saveColumns(); });
        _grid.onColumnsResized.subscribe(function (e, args) { saveColumns(); });


        datamodel.onDataLoading.subscribe(function () {
            showLoadingIndicator(250);
        });

        datamodel.onDataLoaded.subscribe(function (e, args) {
            for (var i = args.from; i <= args.to; i++) {
                _grid.invalidateRow(i);
            }

            if (_report.ReportType == 4) {
                $('.reports-count').text(datamodel.data.length + ' Rows');
            }
            else {
                if (args.total || args.total == 0) {
                    if (args.total < 1) {
                        if (args.from > 0) {
                            $('.reports-count').text(args.to + ' Rows');
                            datamodel.setEndTotal(args.to);
                        }
                        else $('.reports-count').text('0 Rows');
                    }
                    else if (args.total <= args.to) {
                        $('.reports-count').text(args.total + ' Rows');
                        datamodel.setEndTotal(args.total);
                    }
                    else {
                        $('.reports-count').text(args.total - 99 + ' displayed, scroll to see more.');
                    }
                }
            }

            _grid.updateRowCount();
            _grid.render();

            hideLoadingIndicator();
        });

        // filter header buttons
        $('.slick-sort-indicator').each(function () {
            var button = $('<div>')
              .addClass('report-header-filter')
              .insertAfter(this)
              .click(function (e) {
                  $(this).addClass('color-darkorange');
                  var columnDef = $(this).parent().data("column");
                  $('.filter-modal').modal('show');
                  e.preventDefault();
                  e.stopPropagation();
                  $('#filter-user').reportFilter('addCondition', columnDef.fieldID, columnDef.isCustomField);
              });
            $('<i>').addClass('fa fa-filter').appendTo(button);

        });
        if (_report.Settings.SortField) {
            _grid.setSortColumn(_report.Settings.SortField, _report.Settings.IsSortAsc);
            datamodel.setSort(_report.Settings.SortField, _report.Settings.IsSortAsc);
        }
        else {
            if (columns.length > 0) {
                _grid.setSortColumn(columns[0].field, true);
                datamodel.setSort(columns[0].field, true);
            }
        }

        if (!_report.Settings.Columns) _grid.autosizeColumns();
        refresh();
    }

    $('.reports-schedule').click(function (e) {
    	e.preventDefault();
    	var button = $(this);
    	if (button.hasClass('disabled')) return;

    	window.location.assign("Reports_Schedule.html?ReportId=" + _reportID + "&ReportName=" + _report.Name + "&ReportTypeOpened=" + _report.ReportType);
    });

    function refresh() {
        _grid.scrollRowIntoView(0);
        var vp = _grid.getViewport();
        datamodel.clear();
        datamodel.ensureData(vp.top, vp.bottom);

        _layout.resizeAll();
    }
});