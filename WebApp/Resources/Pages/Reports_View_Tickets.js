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

        $('.report-title').text(report.Name);

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

            parent.Ts.Services.Reports.GetFields(_report.Def.Subcategory, function (fields) {
                $('#filter-user').reportFilter({ "fields": fields });
                if (_report.OrganizationID != null) {
                    $('#filter-global').reportFilter({ "fields": fields });
                } else {
                    $('.global-filter-tab').remove();
                }
            });


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

            function getDefaultColumns() {
                return [
                    { id: "checked", name: "checked", maxWidth: 24, formatter: checkedFormatter, unselectable: true, resizable: false, sortable: false, headerCssClass: 'no-header-name' },
  	                { id: "openButton", name: "Open Ticket", maxWidth: 24, formatter: openTicketColumnFormatter, unselectable: true, resizable: false, sortable: false, cssClass: 'ticket-grid-cell-sla', headerCssClass: 'no-header-name' },
                    { id: "IsRead", name: "Read", field: "IsRead", maxWidth: 24, sortable: true, formatter: isReadColumnFormatter, unselectable: true, resizeable: false, headerCssClass: 'no-header-name' },
                    { id: "IsFlagged", name: "Flagged", field: "IsFlagged", maxWidth: 24, sortable: true, formatter: isFlaggedColumnFormatter, unselectable: true, resizeable: false, headerCssClass: 'no-header-name' },
                    { id: "IsSubscribed", name: "Subscribed", field: "IsSubscribed", maxWidth: 24, sortable: true, formatter: isSubscribedColumnFormatter, unselectable: true, resizeable: false, headerCssClass: 'no-header-name' },
                    { id: "IsEnqueued", name: "Enqueued", field: "IsEnqueued", maxWidth: 24, sortable: true, formatter: isEnqueuedColumnFormatter, unselectable: true, resizeable: false, headerCssClass: 'no-header-name' }
	                ];
            }

            var columns = new Array();
            columns = getDefaultColumns();

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


    function saveColumns() {
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

    function refreshGrid() {
        if (loadingIndicator) {
            loadingIndicator.remove();
            loadingIndicator = null;
        }
        var vp = _grid.getViewport();
        datamodel.clear();
        datamodel.ensureData(vp.top, vp.bottom, function () {
            hideLoadingIndicator();
        });
    }

    function resizeGrid(paneName, paneElement, paneState, paneOptions, layoutName) {
        if (loadingIndicator) {
            loadingIndicator.remove();
            loadingIndicator = null;
        }
        try {
            var vp = _grid.getViewport();
            var t = vp.top;
            datamodel.clear();
            datamodel.ensureData(vp.top, vp.bottom + 50, function () {
                if (t > 10) _grid.scrollRowIntoView(t + 10, false);
                _grid.resizeCanvas();
            });
        } catch (e) {

        }

    }

    var dateFormatter = function (row, cell, value, columnDef, dataContext) {
        var date = dataContext[columnDef.id];
        return date ? parent.Ts.Utils.getDateString(date, true, true, _report.ReportType == 3) : '';
    };

    var bitFormatter = function (row, cell, value, columnDef, dataContext) {
        return dataContext[columnDef.id] == true ? "True" : "False";
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

    var checkedFormatter = function (row, cell, value, columnDef, ticket) {
        return '<i class="fa fa-square-o"></i>'
    };

    var moveFormatter = function (row, cell, value, columnDef, ticket) {
        return '<i class="fa fa-bars"></i>'
    };

    var openTicketColumnFormatter = function (row, cell, value, columnDef, dataContext) {
        return '<i class="fa fa-external-link-square color-green" title="Click to open this ticket"></i>';
    };

    var isReadColumnFormatter = function (row, cell, value, columnDef, ticket) {
        return value == false ? '<i class="fa fa-circle color-blue" title="Click to mark this ticket as read"></i>' : '<i class="fa fa-circle color-lightgray" title="Click to mark this ticket as unread"></i>'
    };

    var isFlaggedColumnFormatter = function (row, cell, value, columnDef, dataContext) {
        return value == false ? '<i class="fa fa-flag color-lightgray" title="Click to flag this ticket for follow up"></i>' : '<i class="fa fa-flag color-red" title="Click to unflag this ticket"></i>'
    };

    var isEnqueuedColumnFormatter = function (row, cell, value, columnDef, dataContext) {
        return value == false ? '<i class="ts-text-icon bgcolor-lightgray color-white" title="Click to add this ticket to your queue">Q</i>' : '<i class="ts-text-icon bgcolor-green color-white" title="Click to remove this ticket from your queue">Q</i>';
    };

    var isSubscribedColumnFormatter = function (row, cell, value, columnDef, dataContext) {
        return value == false ? '<i class="fa fa-rss color-lightgray" title="Click to subscribe to this ticket"></i>' : '<i class="fa fa-rss color-amber" title="Click to unsubscribe to this ticket"></i>'
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

    function deselectRows() {
        var cell = _grid.getActiveCell();
        if (cell) {
            _grid.setSelectedRows([cell.row]);
        }
        else {
            _grid.setSelectedRows([]);
        }
    }


    function initGrid(columns) {
        var options = {
            rowHeight: 32,
            editable: false,
            enableAddRow: false,
            enableCellNavigation: true,
            multiSelect: true,
            enableColumnReorder: true
        };

        _grid = new Slick.Grid("#reports-tabview-grid-container", datamodel.data, columns, options);
        _grid.setSelectionModel(new Slick.RowSelectionModel());
        _grid.onViewportChanged.subscribe(function (e, args) {
            var vp = _grid.getViewport();
            datamodel.ensureData(vp.top, vp.bottom);
        });

        _grid.onSort.subscribe(function (e, args) {
            datamodel.setSort(args.sortCol.field, args.sortAsc ? 1 : -1);
            _report.Settings.SortField = args.sortCol.field;
            _report.Settings.IsSortAsc = args.sortAsc;
            saveUserSettings();
            var vp = _grid.getViewport();
            datamodel.ensureData(vp.top, vp.bottom);
        });

        _grid.onColumnsReordered.subscribe(function (e, args) { saveColumns(); });
        _grid.onColumnsResized.subscribe(function (e, args) { saveColumns(); });

        _grid.onClick.subscribe(function (e, args) {
            var cell = args.cell;
            var row = args.row;
            var ticket = datamodel.data[row];
            var ids = getSelectedIDs();
            var data = JSON.stringify(ids);
            switch (_grid.getColumns()[cell].id) {
                case "IsRead":
                    var setRead = !ticket.IsRead;
                    if (ids.length > 1) {
                        showLoadingIndicator();
                        parent.Ts.Services.Tickets.SetTicketReads(data, setRead, function () { refreshGrid(); deselectRows(); });
                    }
                    else {
                        ticket.IsRead = setRead;
                        parent.Ts.Services.Tickets.SetTicketRead(ticket.hiddenTicketID, ticket.IsRead, function () {
                            parent.Ts.MainPage.updateMyOpenTicketReadCount();
                        });
                        if (ticket.IsRead) {
                            $('.slick-row[row="' + row + '"]').addClass('ticket-grid-row-read');
                        }
                        else {
                            $('.slick-row[row="' + row + '"]').removeClass('ticket-grid-row-read');
                        }
                        _grid.invalidateRow(row);
                        _grid.updateRow(row);
                        _grid.render();

                    }

                    parent.Ts.System.logAction('Ticket Grid - Changed Read Status');
                    e.stopPropagation();
                    e.stopImmediatePropagation();

                    return true;
                case "IsFlagged":
                    var setIsFlagged = !ticket.IsFlagged;
                    if (ids.length > 1) {
                        showLoadingIndicator();
                        parent.Ts.Services.Tickets.SetTicketFlags(data, setIsFlagged, function () { refreshGrid(); deselectRows(); });
                    }
                    else {
                        ticket.IsFlagged = setIsFlagged;
                        parent.Ts.Services.Tickets.SetTicketFlag(ticket.hiddenTicketID, ticket.IsFlagged, function () {
                            _grid.invalidateRow(row);
                            _grid.updateRow(row);
                            _grid.render();
                        });
                    }

                    parent.Ts.System.logAction('Ticket Grid - Changed Flagged Status');
                    e.stopPropagation();
                    e.stopImmediatePropagation();
                    return true;
                case "IsEnqueued":
                    var setIsEnqueued = !ticket.IsEnqueued;
                    if (ids.length > 1) {
                        showLoadingIndicator();
                        parent.Ts.Services.Tickets.SetUserQueues(data, setIsEnqueued, function () { refreshGrid(); deselectRows(); });
                    }
                    else {
                        ticket.IsEnqueued = setIsEnqueued;
                        parent.Ts.Services.Tickets.SetUserQueue(ticket.hiddenTicketID, setIsEnqueued, function () {
                            _grid.invalidateRow(row);
                            _grid.updateRow(row);
                            _grid.render();
                        });
                    }
                    parent.Ts.System.logAction('Ticket Grid - Changed Queue Status');
                    e.stopPropagation();
                    e.stopImmediatePropagation();
                    return true;
                case "IsSubscribed":
                    var setIsSubscribed = !ticket.IsSubscribed;
                    if (ids.length > 1) {
                        showLoadingIndicator();
                        parent.Ts.Services.Tickets.SetTicketSubcribes(data, setIsSubscribed, function () { refreshGrid(); deselectRows(); });
                    }
                    else {
                        ticket.IsSubscribed = setIsSubscribed;
                        parent.Ts.Services.Tickets.SetSubscribed(ticket.hiddenTicketID, ticket.IsSubscribed, null, function () {
                            _grid.invalidateRow(row);
                            _grid.updateRow(row);
                            _grid.render();
                        });
                    }
                    parent.Ts.System.logAction('Ticket Grid - Changed Subscribed Status');
                    e.stopPropagation();
                    e.stopImmediatePropagation();
                    return true;
                case "openButton":
                    parent.Ts.MainPage.openTicket(datamodel.data[row]['Ticket Number']);
                    _grid.invalidateRow(row);
                    _grid.updateRow(row);
                    _grid.render();
                    e.stopPropagation();
                    e.stopImmediatePropagation();
                    return true;
                case "checked":
                    var rows = _grid.getSelectedRows();

                    for (var i = 0; i < rows.length; i++) {
                        if (rows[i] == row) {
                            if (rows.length == 1) return;
                            rows.splice(i, 1);
                            _grid.setSelectedRows(rows);

                            var activeCell = _grid.getActiveCell();
                            if (activeCell && activeCell.row == row) {
                                _grid.setActiveCell(rows[0], 0);
                                _grid.setSelectedRows(rows);
                            }
                            _grid.invalidateRow(row);
                            _grid.updateRow(row);
                            _grid.render();

                            e.stopPropagation();
                            e.stopImmediatePropagation();

                            return true;
                        }
                    }

                    rows.push(row);
                    _grid.setActiveCell(rows[0], 0);
                    _grid.setSelectedRows(rows);
                    e.stopPropagation();
                    e.stopImmediatePropagation();
                    return true;
                default:

            }
            return false;
        });

        datamodel.onDataLoading.subscribe(function () {
            showLoadingIndicator(250);
        });

        datamodel.onDataLoaded.subscribe(function (e, args) {
            for (var i = args.from; i <= args.to; i++) {
                _grid.invalidateRow(i);
            }
            $('.reports-count').text(datamodel.data.length + ' Rows');
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

    function refresh() {

        _layout.resizeAll();
    }

    function getSelectedIDs() {
        selectedRowIds = [];
        var rows = _grid.getSelectedRows();
        for (var i = 0, l = rows.length; i < l; i++) {
            var ticket = datamodel.data[rows[i]];
            if (ticket) selectedRowIds.push(ticket.hiddenTicketID);
        }
        return selectedRowIds;

    }

    $('.reports-schedule').click(function (e) {
    	e.preventDefault();
    	var button = $(this);
    	if (button.hasClass('disabled')) return;

    	window.location.assign("Reports_Schedule.html?ReportId=" + _reportID + "&ReportName=" + _report.Name + "&ReportTypeOpened=" + _report.ReportType);
    });
});