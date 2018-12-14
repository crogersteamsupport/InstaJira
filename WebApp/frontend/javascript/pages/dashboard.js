var _dashboard = null;
$(document).ready(function() {
    _dashboard = new Dashboard();
    _dashboard.init();
});

function onShow() {
    _dashboard.refreshAll();
};

Dashboard = function() {
    this.reports = null;
}

Dashboard.prototype = {
    init: function() {
        var self = this;

        $('.dashboard-container').on('click', '.report-title a', function(e) {
            e.preventDefault();
            e.stopPropagation();
            var report = $(this).closest('.item').data('report');
            parent.Ts.MainPage.openReport(report, true);
        });

        parent.Ts.Utils.webMethod("ReportService", "GetDashboard", {},
            function(result) {
                parent.Ts.Utils.webMethod("ReportService", "GetDashboardReports", {},
                    function(reports) {
                        _reports = reports;
                        var dashboards = [];
                        try {
                            dashboards = JSON.parse(result);
                        } catch (e) {
                            dashboards = [];
                        }

                        function findReport(reportID) {
                            for (var i = 0; i < reports.length; i++) {
                                if (reports[i].ReportID == reportID) return reports[i];
                            }
                        }
                        var container = $('.dashboard-container').remove('.item');
                        for (var i = 0; i < dashboards.length; i++) {
                            var dashboard = dashboards[i];
                            var report = findReport(dashboard.ReportID);
                            if (report != null) {
                                var item = $('.template-box .item').clone().appendTo(container).data('report', report);
                                item.find('.report-title a').text(report.Name);
                                item.find('.countbubble').attr('id', report.ReportID);
                                if (dashboard.Rows == 2) {
                                    item.addClass('ht2');
                                } else if (dashboard.Rows == 3) {
                                    item.addClass('ht3');
                                }

                                if (dashboard.Columns == 2) {
                                    item.addClass('wd2');
                                } else if (dashboard.Columns == 3) {
                                    item.addClass('wd3');
                                }
                            }
                        }
                        loadDashboard();
                    });
            });

        function loadDashboard() {
            var $container = $('.dashboard-container');
            $container.packery({
                itemSelector: '.item',
                gutter: 0,
                columnWidth: '.grid-sizer',
                rowHeight: 300
            });
            _pckry = $container.data('packery');
            var $itemElems = $($container.packery('getItemElements'));
            $itemElems.draggable({
                delay: 250,
                handle: '.box-header'
            });
            _pckry.layout();
            $container.packery('bindUIDraggableEvents', $itemElems);
            _pckry.on('dragItemPositioned', saveDashboard);
            $container.find('.item').each(function() {
                self.refreshReport($(this));
            });
        }

        function saveDashboard() {
            var items = [];
            var elements = _pckry.getItemElements();
            for (var i = 0; i < elements.length; i++) {
                var el = $(elements[i]);
                var item = {};
                item.ReportID = el.data('report').ReportID;
                item.Rows = el.hasClass('ht2') ? 2 : (el.hasClass('ht3') ? 3 : 1);
                item.Columns = el.hasClass('wd2') ? 2 : (el.hasClass('wd3') ? 3 : 1);
                items.push(item);
            }
            $('.dashboard-container .item').each(function() {});
            parent.Ts.Utils.webMethod("ReportService", "SaveDashboard", {
                data: JSON.stringify(items)
            });
        }

        $('.dashboard-refresh').click(function(e) {
            e.preventDefault();
            if (e.shiftKey) {
                self.refreshAll();
            } else {
                window.location.assign(window.location.href);
            }
        });

        $('.dashboard-container').on('click', '.action-refresh', function(e) {
            e.preventDefault();
            self.refreshReport($(this).closest('.item'));
        });

        $('.dashboard-container').on('click', '.action-remove', function(e) {
            e.preventDefault();
            _pckry.remove($(this).closest('.item'));
            _pckry.layout();
            saveDashboard();
        });

        $('.dashboard-container').on('click', '.action-size a', function(e) {
            e.preventDefault();
            var link = $(this);
            var item = link.closest('.item');
            if (link.hasClass('action-w1')) {
                item.removeClass('wd2').removeClass('wd3');
            } else if (link.hasClass('action-w2')) {
                item.removeClass('wd3').addClass('wd2');
            } else if (link.hasClass('action-w3')) {
                item.removeClass('wd2').addClass('wd3');
            }
            if (link.hasClass('action-h1')) {
                item.removeClass('ht2').removeClass('ht3');
            } else if (link.hasClass('action-h2')) {
                item.removeClass('ht3').addClass('ht2');
            } else if (link.hasClass('action-h3')) {
                item.removeClass('ht2').addClass('ht3');
            }
            _pckry.layout();
            saveDashboard();
            self.refreshReport(item);
        });

        var execGetReports = null;

        function getReports(request, response) {
            if (execGetReports) {
                execGetReports._executor.abort();
            }
            execGetReports = parent.Ts.Services.Reports.FindReport(request.term, function(result) {
                response(result);
                $(this).removeClass('ui-autocomplete-loading');
            });
        }

        $('.dashboard-show-add').click(function(e) {
            e.preventDefault();
            $('#modal-report').val('');
            $('#modal-w1').prop('checked', true);
            $('#modal-h1').prop('checked', true);
            $('.report-modal-add').modal('show');
        });

        $('.report-modal-add').modal({
            show: false,
            "backdrop": 'static'
        });

        $('.dashboard-add').click(function(e) {
            e.preventDefault();
            var reportID = $('#modal-report').data('reportID');
            if (reportID == null) {
                alert('Please select a report.');
                return;
            }

            parent.Ts.Utils.webMethod("ReportService", "GetReport", {
                    "reportID": reportID
                }, function(report) {
                    var $container = $('.dashboard-container');
                    var item = $('.template-box .item').clone().appendTo($container).data('report', report);
                    item.find('.report-title a').text(report.Name);
                    var w = $('.report-modal-add input[name=modal-w]:checked').val();
                    if (w == "w2") {
                        item.addClass('wd2');
                    } else if (w == "w3") {
                        item.addClass('wd3');
                    }
                    var h = $('.report-modal-add input[name=modal-h]:checked').val();
                    if (h == "h2") {
                        item.addClass('ht2');
                    } else if (h == "h3") {
                        item.addClass('ht3');
                    }
                    _pckry.appended(item);
                    $('.report-modal-add').modal('hide');
                    var $itemElems = $($container.packery('getItemElements'));
                    $itemElems.draggable({
                        delay: 250,
                        handle: '.box-header'
                    });
                    $container.packery('bindUIDraggableEvents', $itemElems);
                    self.refreshReport(item);
                    saveDashboard();
                },
                function() {
                    alert('There was an error retrieving that report.');
                    return;
                }
            );
        });

        $('#modal-report').autocomplete({
            minLength: 2,
            source: getReports,
            appendTo: '.report-modal-add',
            select: function(event, ui) {
                $(this).data('reportID', ui.item.id);
            }
        });
    },

    refreshReport: function(el) {
        var report = el.data('report');
        var content = el.find('.box-content');
        if (report.ReportType == 2) {
            content.empty();
            $('<iframe>').attr('height', '100%').attr('width', '100%').attr('frameborder', '0').attr('src', report.ReportDef).css('overflow-x', 'hidden').css('overflow-y', 'hidden').appendTo(content);
        } else if (report.ReportType == 1) {
            report.Def = JSON.parse(report.ReportDef);
            parent.Ts.Utils.webMethod(null, "reportdata/chart", {
                    "reportID": report.ReportID
                },
                function(data) {
                    createChart(content, report.Def.Chart, data);
                },
                function(error) {
                    showChartError(content, error.get_message());
                });
        } else {
            if (report.grid) {
                report.grid.refresh();
            } else {
                el.addClass('box-grid');
                report.grid = new Grid($('<div>').addClass('dashboard-grid').appendTo(content), report);
                report.grid.init();
            }
        }
    },

    refreshAll: function() {
        var self = this;
        $('.dashboard-container .item').each(function() {
            self.refreshReport($(this));
        });
    }
}


Grid = function(element, report) {

    this.report = report;
    this.element = $(element);
    if (!this.report.Def) {
        this.report.Def = JSON.parse(this.report.ReportDef);
    }
    this.report.Settings = this.report.UserSettings == '' ? new Object() : JSON.parse(this.report.UserSettings);
    this.datamodel = new TeamSupport.DataModel(getReportData);
    var reportID = this.report.ReportID;

    function getReportData(from, to, sortcol, isdesc, callback) {
        var params = {
            "reportID": reportID,
            "from": from,
            "to": to,
            "sortField": sortcol,
            "isDesc": isdesc,
            "useUserFilter": true
        };
        //console.log('REQUEST: From: ' + fromPage * PAGESIZE + ', To: ' + ((fromPage * PAGESIZE) + PAGESIZE-1) + "  Page: " + fromPage);
        req = $.ajax({
            type: "POST",
            url: "/reportdata/table",
            data: JSON.stringify(params),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: callback
        });
        return req;
    }
}

Grid.prototype = {
    init: function() {
        var self = this;

        parent.Ts.Utils.webMethod("ReportService", "GetReportColumns", {
                "reportID": self.report.ReportID
            },
            function(repCols) {

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
                    }

                    columns.push(column);
                }

                if (self.report.Settings.Columns) {
                    for (var i = 0; i < self.report.Settings.Columns.length; i++) {
                        var userCol = self.report.Settings.Columns[i];
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

        function saveUserSettings(callback) {
            parent.Ts.Utils.webMethod("ReportService", "SaveUserSettings", {
                    "reportID": self.report.ReportID,
                    "data": JSON.stringify(self.report.Settings)
                },
                function() {
                    if (callback) callback();
                },
                function(error) {
                    alert(error.get_message());
                }
            );
        }

        function saveColumns() {
            var columns = self.grid.getColumns();
            self.report.Settings.Columns = new Array();
            self.report.Settings.forceFitColumns = self.grid.getOptions().forceFitColumns == true;

            for (var i = 0; i < columns.length; i++) {
                var item = new Object();
                item.id = columns[i].id;
                item.width = columns[i].width;
                self.report.Settings.Columns.push(item);
            }
            saveUserSettings();
        }

        var dateFormatter = function(row, cell, value, columnDef, dataContext) {
            var date = dataContext[columnDef.id];
            return date ? parent.Ts.Utils.getDateString(date, true, true, self.report.ReportType == 3) : '';
        };

        var bitFormatter = function(row, cell, value, columnDef, dataContext) {
            return dataContext[columnDef.id] == true ? "True" : "False";
        };

        var floatFormatter = function(row, cell, value, columnDef, dataContext) {
            return value;
        };

        var emailFormatter = function(row, cell, value, columnDef, dataContext) {
            return '<a href="mailto:' + dataContext[columnDef.id] + '">' + dataContext[columnDef.id] + '</a>';
        };

        var linkFormatter = function(row, cell, value, columnDef, dataContext) {
            return '<a href="' + dataContext[columnDef.id] + '" target="_blank">' + dataContext[columnDef.id] + '</a>';
        };

        var ticketNumberFormatter = function(row, cell, value, columnDef, dataContext) {
            return '<a href="#" onclick="parent.Ts.MainPage.openTicket(' + dataContext[columnDef.id] + ', true); return false;">' + dataContext[columnDef.id] + '</a>';
        }

        var openFormatter = function(row, cell, value, columnDef, dataContext) {
            if (columnDef.openField == "TicketID") {
                return '<a href="#" onclick="parent.Ts.MainPage.openTicketByID(' + dataContext["hiddenTicketID"] + ', true); return false;">' + dataContext[columnDef.id] + '</a>';
            } else if (columnDef.openField == "OrganizationID") {
                return '<a href="#" onclick="parent.Ts.MainPage.openCustomer(' + dataContext["hiddenOrganizationID"] + '); return false;">' + dataContext[columnDef.id] + '</a>';
            } else if (columnDef.openField == "UserID") {
                return '<a href="#" onclick="parent.Ts.MainPage.openContact(' + dataContext["hiddenUserID"] + '); return false;">' + dataContext[columnDef.id] + '</a>';
            }
            return value;
        };

        self.tmrDelayIndicator = null;
        self.tmrHideLoading = null;
        self.loadingIndicator = null;

        self.showLoadingIndicator = function(delay) {
            if (!delay) {
                if (!self.loadingIndicator) {
                    self.loadingIndicator = $('<div class="grid-loading"></div>').appendTo(self.element);
                    self.loadingIndicator.position({
                        my: "center center",
                        at: "center center",
                        of: self.element,
                        collision: "none",
                        within: self.element
                    });
                }
                self.loadingIndicator.show();
            } else {
                if (self.tmrDelayIndicator) clearTimeout(self.tmrDelayIndicator);
                self.tmrDelayIndicator = setTimeout(function() {
                    self.showLoadingIndicator();
                }, delay);
            }
            if (self.tmrHideLoading) {
                clearTimeout(self.tmrHideLoading);
            }
            self.tmrHideLoading = setTimeout(function() {
                hideLoadingIndicator();
            }, 60000);
        }

        self.showLoadingIndicator();

        function hideLoadingIndicator() {
            self.tmrHideLoading = null;
            if (self.tmrDelayIndicator) clearTimeout(self.tmrDelayIndicator);
            self.tmrDelayIndicator = null;
            if (self.loadingIndicator) self.loadingIndicator.fadeOut();
        }

        function initGrid(columns) {
            var options = {
                rowHeight: 22,
                editable: false,
                enableAddRow: false,
                enableCellNavigation: true,
                multiSelect: false,
                enableColumnReorder: true,
                forceFitColumns: false
            };

            self.grid = new Slick.Grid(self.element, self.datamodel.data, columns, options);

            self.grid.onViewportChanged.subscribe(function(e, args) {
                var vp = self.grid.getViewport();
                self.datamodel.ensureData(vp.top, vp.bottom);
            });

            self.grid.onSort.subscribe(function(e, args) {
                self.datamodel.setSort(args.sortCol.field, args.sortAsc ? 1 : -1);
                self.report.Settings.SortField = args.sortCol.field;
                self.report.Settings.IsSortAsc = args.sortAsc;
                saveUserSettings();
                var vp = self.grid.getViewport();
                self.datamodel.ensureData(vp.top, vp.bottom);
            });

            self.grid.onColumnsReordered.subscribe(function(e, args) {
                saveColumns();
            });
            self.grid.onColumnsResized.subscribe(function(e, args) {
                saveColumns();
            });


            self.datamodel.onDataLoading.subscribe(function() {
                self.showLoadingIndicator(250);
            });

            self.datamodel.onDataLoaded.subscribe(function(e, args) {
                for (var i = args.from; i <= args.to; i++) {
                    self.grid.invalidateRow(i);
                }

                self.grid.updateRowCount();
                self.grid.render();

                $('.countbubble#'+ self.report.ReportID).text('(' + self.datamodel.data.length + ')');
                $('.countbubble').show();

                hideLoadingIndicator();
            });

            if (self.report.Settings.SortField) {
                self.grid.setSortColumn(self.report.Settings.SortField, self.report.Settings.IsSortAsc);
                self.datamodel.setSort(self.report.Settings.SortField, self.report.Settings.IsSortAsc);
            } else {
                if (columns.length > 0) {
                    self.grid.setSortColumn(columns[0].field, true);
                    self.datamodel.setSort(columns[0].field, true);
                }
            }

            if (!self.report.Settings.Columns) self.grid.autosizeColumns();
            self.refresh();
        }

    },

    refresh: function() {
        var self = this;
        if (self.loadingIndicator) {
            self.loadingIndicator.remove();
            self.loadingIndicator = null;
        }
        try {
            self.showLoadingIndicator();
            var vp = self.grid.getViewport();
            var t = vp.top;
            self.datamodel.clear();
            self.datamodel.ensureData(vp.top, vp.bottom + 50, function() {
                if (t > 10) self.grid.scrollRowIntoView(t + 10, false);
                self.grid.resizeCanvas();
            });
        } catch (e) {
            console.log(e.message);
        }

    }
}
