/// <reference path="ts/ts.js" />
/// <reference path="ts/parent.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />
$(document).ready(function () {
    var _reportID = parent.Ts.Utils.getQueryValue('ReportID', window);
    var _reportType = parent.Ts.Utils.getQueryValue('ReportType', window);
    var _chartData = null;
    if (_reportType != null) _reportType = parseInt(_reportType);
    var _tempReport = new Object();
    var _typeClass = '';
    var location = window.location;
    var _report = null;
    var _cats = null;
    var _fields = null;
    var _subID = -1;

    $('.report-filter').reportFilter();
    $('#chartFilter').reportFilter();

    $('#cbStock').change(function (e) {
        if ($('#cbStock').prop('checked')) {
            $('.report-section .panel').removeClass('panel-default').addClass('panel-danger');
            $('.action-save').removeClass('btn-primary').addClass('btn-danger').text('SAVE STOCK REPORT');
        }
        else {
            $('.report-section .panel').addClass('panel-default').removeClass('panel-danger');
            $('.action-save').addClass('btn-primary').removeClass('btn-danger').text('Save');
        }

    });

    if (_reportID != null) {
        parent.Ts.Utils.webMethod("ReportService", "GetReport", {
            "reportID": _reportID
        }, function (report) {
            _report = report;
            if (_report.OrganizationID == null) {
                if (parent.Ts.System.User.UserID != 34 && parent.Ts.System.User.UserID != 43 && parent.Ts.System.User.UserID != 47) return;
                $('#cbStock').prop('checked', true).trigger('change');

            }

            $('.report-name').val(report.Name);
            
            initReport();
            $('.report-privacy').val(report.IsPrivate.toString());
        });
    } else if (_reportType != null) {
        initReport();
    } else {
        return;
    }

    if (parent.Ts.System.User.UserID == 34 || parent.Ts.System.User.UserID == 43 || parent.Ts.System.User.UserID == 47) {
        $('.checkbox-stock').removeClass('hidden');
    }


    function initReport() {
        $('.report-class-item').hide();
        if (_reportType == null) { _reportType = _report.ReportType; }
        var type = _reportType;

        switch (type) {
            case 0:
                _typeClass = 'report-class-tabular';
                $('.reports-header i').addClass('fa-table color-red');
                $('.report-title').text(_report ? _report.Name : 'New Tabular Report');
                break;
            case 1:
                _typeClass = 'report-class-chart';
                $('.reports-header i').addClass('fa-bar-chart-o color-green');
                $('.report-title').text(_report ? _report.Name : 'New Chart');
                $('.report-type-radio-chart').prop('checked', true);
                break;
            case 2:
                _typeClass = 'report-class-external';
                $('.reports-header i').addClass('fa-globe color-blue');
                $('.report-title').text(_report ? _report.Name : 'New External Report');
                break;
            case 4:
                _typeClass = 'report-class-summary';
                $('.reports-header i').addClass('fa-tasks color-amber');
                $('.report-title').text(_report ? _report.Name : 'New Summary Report');
                break;
            case 5:
                _typeClass = 'report-class-tickets';
                $('.reports-header i').addClass('fa-th-list');
                $('.report-title').text(_report ? _report.Name : 'Ticket Views');
                break;
            default:
        }
        $('.report-class-item.' + _typeClass).show();
        $('.report-setup').show();
        $('.report-class-tickets-privacy').show();
        if (type == 2) {
            if (_report && _report.ReportDef) {
                $('#external-url').val(_report.ReportDef);
            }
            $('.action-back, .action-next').hide();
            $('.action-save').show();
            $('.action-cancel').css('float', 'none');
            $('.report-privacy').val('false');
        }
        else if (type == 5) {

            if (_report && _report.ReportDef) {
                _report.Def = JSON.parse(_report.ReportDef);
            }

            var option = $('<option/>').attr('value', 32).text('Tickets');
            option.prop('selected', true);
            $('#selectCat').append(option);

            var subOption = $('<option/>').attr('value', 70).text('Tickets2');
            subOption.prop('selected', true);
            $('#selectSubCat').append(subOption);

            $('#selectCat').val(32);
            $('#selectSubCat').val(70);

            if (parent.Ts.System.User.IsSystemAdmin == true) {
                $('.report-class-tickets-privacy').show();
            }
            else {
                $('.report-privacy').val('true');
            }
        }
        else {
            if (_report && _report.ReportDef) {
                _report.Def = JSON.parse(_report.ReportDef);
            }
            parent.Ts.Services.Reports.GetCategories(loadCats);
            $('.report-privacy').val('false');
        }

        $('.action-cancel').click(function (e) {
            e.preventDefault();
            closeReport();
        });

        function validateSection(el) {
            if (el.hasClass('report-setup')) {
                if ($('.report-name').val() == '') {
                    $('.report-name').popover('show').parent('.form-group').addClass('has-error');
                    return false;
                }
                else {
                    $('.report-name').popover('hide').parent('.form-group').removeClass('has-error');
                }
                return null;
            }
            if (el.hasClass('report-fields')) {
                return $('li.report-field input:checked').length > 0 ? null : "Please select some field.";
            }
        }

        function loadNewSection(el, callback) {
            if (el.hasClass('report-fields') || el.hasClass('report-summary-fields')) {
                if (_report && _report.Def.Subcategory != $('#selectSubCat').val()) { _report = null; }
                if (!(_tempReport.Subcategory && _tempReport.Subcategory == $('#selectSubCat').val())) { loadFields(callback); }
            }
            else if (el.hasClass('report-filters')) {
                initFilters();
                if (callback) callback();
            }
            else if (el.hasClass('report-chartproperties')) {

                if (_report && _report.Def.Subcategory != $('#selectSubCat').val()) { _report = null; }
                if (!(_tempReport.Subcategory && _tempReport.Subcategory == $('#selectSubCat').val())) {
                    loadFields(function () {
                        initFilters();
                        initChart(callback);
                    });

                }

            }
            else {
                if (callback) callback();
            }
        }

        $('.action-next').click(function (e) {
            e.preventDefault();
            var currentSection = $('.report-section:visible');
            var validation = validateSection(currentSection);
            if (validation != null) {
                if (validation == "") {
                    return;
                }
                else {
                    alert(validation);
                    return;
                }
            }
            var nextSection = currentSection.nextAll('.' + _typeClass + ':first');

            loadNewSection(nextSection, function () {
                currentSection.fadeOut(200, function () {
                    $('.action-back').removeClass('disabled').prop('disabled', false);
                    $('.action-next').removeClass('disabled').prop('disabled', false);
                    if (nextSection.nextAll('.' + _typeClass + ':first').length < 1) {
                        $('.action-next').hide();
                        $('.action-save').show();
                    }
                    nextSection.show();
                });
            });

        });

        $('.action-back').click(function (e) {
            e.preventDefault();
            $('.action-save').hide();
            $('.action-next').show();

            var currentSection = $('.report-section:visible').hide();
            var prevSection = currentSection.prevAll('.' + _typeClass + ':first').show();

            if (prevSection.prevAll('.' + _typeClass + ':first').length < 1) {
                $('.action-back').addClass('disabled').prop('disabled', false);
            }

        });
        function loadCats(cats) {
            _cats = cats;
            for (var i = 0; i < cats.length; i++) {
                var option = $('<option/>').attr('value', cats[i].ReportTableID).text(cats[i].Name);
                if (_report != null && findSubCat(cats[i].Subcategories, _report.Def.Subcategory) != null) {
                    option.prop('selected', true);
                }
                $('#selectCat').append(option);
            }
            loadSubCats();
        }


        $('.summary-add-field').click(function (e) {
            e.preventDefault();
            var list = $(this).prev();
            var item = list.find('li:first').clone(true).hide();
            item.find('.summary-desc-val1').hide();
            if (_reportType == 1) {
                $('.summary-add-descfield').hide();
                item.find('label').text('Series');
                item.prependTo(list).fadeIn(function () { initSummaryRow(item); });
            }
            else {
                item.appendTo(list).fadeIn(function () { initSummaryRow(item); });
            }


        });

        $('.summary-fields').on('click', 'li a', function (e) {
            e.preventDefault();
            $(this).closest('li').fadeOut(function () {
                $(this).remove();
                if (_reportType == 1) $('.summary-add-descfield').fadeIn();
            });

        });

        $('.summary-fields').on('change', '.summary-desc-field', function (e) {
            e.preventDefault();
            var item = $(this);
            var field = item.find(':selected').data('field');
            if (field.DataType == 'datetime') {
                item.next().show();
            }
            else {
                item.next().hide();
            }
        });

        $('.summary-fields').on('change', '.summary-calc-field', function (e) {
            e.preventDefault();
            var item = $(this);
            var field = item.find(':selected').data('field');
            if (field.DataType == 'number') {
                item.next().show().next().hide();
            }
            else {
                item.next().hide().next().show();
            }
        });

        $('.summary-fields').on('change', '.summary-calc-comp', function (e) {
            e.preventDefault();
            var value = $(this).val();
            if (value == 'none') {
                $(this).next().hide().next().hide();
            }
            else if (value == 'bet') {
                $(this).next().show().next().show();
            } else {
                $(this).next().show().next().hide();
            }
        });

        function initSummaryRow(el) {
            el.find('.summary-desc-field').trigger('change');
            el.find('.summary-calc-field').trigger('change');
            el.find('.summary-calc-comp').trigger('change');
        }

        function loadSummaryFields(el) {
            clearSummaryFields();
            var list = el.find('.summary-desc');
            for (var i = 0; i < _report.Def.Fields.Descriptive.length; i++) {
                var field = _report.Def.Fields.Descriptive[i];
                if (i > 0) list.find('li:first').clone(true).appendTo(list);
                var item = list.find('li:last');
                item.find('.' + getUniqueFieldClass(field.Field)).prop('selected', true).trigger('change');
                item.find('.summary-desc-val1').val(field.Value1);
            }

            list = el.find('.summary-calc');
            for (var i = 0; i < _report.Def.Fields.Calculated.length; i++) {
                var field = _report.Def.Fields.Calculated[i];
                if (i > 0) list.find('li:first').clone(true).appendTo(list);
                var item = list.find('li:last');
                item.find('.' + getUniqueFieldClass(field.Field)).prop('selected', true).trigger('change');
                item.find('.summary-calc-arg').val(field.Aggregate);
                item.find('.summary-calc-text-arg').val(field.Aggregate);
                item.find('.summary-calc-comp').val(field.Comparator).trigger('change');
                item.find('.summary-calc-val1').val(field.Value1);
                item.find('.summary-calc-val2').val(field.Value2);

            }

        }

        function clearSummaryFields() {
            $('.summary-fields').each(function () {
                $(this).find('li').not(':first').remove();
            });
            $('.summary-add-descfield').show();
        }

        function findSubCat(subcats, subcatID) {
            for (var i = 0; i < subcats.length; i++) {
                if (subcats[i].SubCatID == subcatID) return subcats[i];
            }
            return null;
        }

        $('#selectCat').change(loadSubCats);

        function loadSubCats() {
            var reportTableID = $('#selectCat').val();
            $('#selectSubCat').empty();
            //$('#selectSubCat').append($('<option/>').attr('value', '-1').text('None'));
            for (var i = 0; i < _cats.length; i++) {
                if (_cats[i].ReportTableID == reportTableID) {
                    for (var j = 0; j < _cats[i].Subcategories.length; j++) {
                        var option = $('<option/>').attr('value', _cats[i].Subcategories[j].SubCatID).text(_cats[i].Subcategories[j].Name);
                        if (_report != null && _cats[i].Subcategories[j].SubCatID == _report.Def.Subcategory) {
                            option.prop('selected', true);
                        }
                        $('#selectSubCat').append(option);
                    }
                }
            }
        }

        function loadFields(callback) {
            var subID = $('#selectSubCat').val();

            if (_subID == subID) {
                if (callback) callback();
                return;
            }

            $('.summary-desc .summary-field').empty();
            $('.summary-calc .summary-field').empty();

            _subID = subID;
            _fields = null;
            $('.report-fields-available ul').empty();
            parent.Ts.Services.Reports.GetFields(subID, function (fields) {
                var primaryFields = jQuery.grep(fields, function (elem) {
                    return (elem.IsPrimary == true && elem.IsCustom == false);
                });
                var primaryFieldsCF = jQuery.grep(fields, function (elem) {
                    return (elem.IsPrimary == true && elem.IsCustom == true);
                });

                var secondaryFields = jQuery.grep(fields, function (elem) {
                    return (elem.IsPrimary == false && elem.IsCustom == false);
                });
                var secondaryFieldsCF = jQuery.grep(fields, function (elem) {
                    return (elem.IsPrimary == false && elem.IsCustom == true);
                }); 

                $('.report-fields-primary').find('.panel-title > a').text(primaryFields[0].Table);
                addFieldsToSection(primaryFields, 'primary');

						 if (primaryFieldsCF.length > 0) {
                		addFieldsToSection(primaryFieldsCF, 'primary-cf');
                		if (_typeClass == 'report-class-tabular' || _typeClass == 'report-class-tickets') {
                			$('.report-fields-primary-cf').find('.panel-title  > a').text(primaryFieldsCF[0].Table + ' (Custom Fields)');
                			$('.report-fields-primary-cf').show();
                		}
						};

                	if (secondaryFields.length > 0) {
                		addFieldsToSection(secondaryFields, 'secondary');
                		if (_typeClass == 'report-class-tabular' || _typeClass == 'report-class-tickets') {
                			$('.report-fields-secondary').find('.panel-title > a').text(secondaryFields[0].Table);
                			$('.report-fields-secondary').show();
                		}
                	};

                	if (secondaryFieldsCF.length > 0) {
                		addFieldsToSection(secondaryFieldsCF, 'secondary-cf');
                		if (_typeClass == 'report-class-tabular' || _typeClass == 'report-class-tickets') {
                			$('.report-fields-secondary-cf').find('.panel-title > a').text(secondaryFieldsCF[0].Table + ' (Custom Fields)');
                			$('.report-fields-secondary-cf').show();
                		}
                	};

                _fields = fields;

                clearSummaryFields();
                initSummaryRow($('.summary-fields'));
                if (_report != null) {
                    loadSelectedFields();
                    if (_reportType == 1) { loadSummaryFields($('.report-chartproperties')); }
                    if (_reportType == 4) { loadSummaryFields($('.report-summary-fields')); }
                }
                $('.report-filter').reportFilter('loadFields', fields);
                $('#chartFilter').reportFilter('loadFields', fields);
                if (callback) callback();
            });
        }

        function addFieldsToSection(fields, sectionName) {
            var tableName = "";
            var optGroup = null;
            var optGroupx = null;
            var optGroupy = null;
            var max = Math.floor(fields.length / 3);
            var rem = fields.length % 3;
            var iteration = 1;
            var cnt = rem > 0 ? -1 : 0;
            var list = $('.report-fields-available.report-fields-' + sectionName + '  ul.report-fields-1');

            for (var i = 0; i < fields.length; i++) {
                if (cnt > max - 1) {
                    iteration++;
                    cnt = rem == iteration ? -1 : 0;
                    list = $('.report-fields-available.report-fields-' + sectionName + '  ul.report-fields-' + iteration);
                }
                cnt++;


                delete fields[i]['__type'];
                var fieldName = fields[i].Name + (fields[i].AuxName ? " (" + fields[i].AuxName + ")" : "");
                if (_typeClass == 'report-class-tabular' || _typeClass == 'report-class-tickets') {
                	$('<li>')
										.addClass('report-field ' + getUniqueFieldClass(fields[i]))
										.data('field', fields[i])
										.append($('<div>', {
											'class': 'checkbox'
										}).append($('<label>', {
											'html': fieldName
										}).prepend('<input type="checkbox" />')))
										.appendTo(list);
                }
                if (tableName != fields[i].Table) {
                    tableName = fields[i].Table;
                    optGroupx = $('<optgroup>').attr('label', tableName).appendTo('.summary-desc .summary-field');
                    optGroupy = $('<optgroup>').attr('label', tableName).appendTo('.summary-calc .summary-field');
                }

                $('<option>').text(fieldName).data('field', fields[i]).addClass(getUniqueFieldClass(fields[i])).appendTo(optGroupx);
                $('<option>').text(fieldName).data('field', fields[i]).addClass(getUniqueFieldClass(fields[i])).appendTo(optGroupy);
            }
        }

        function loadSelectedFields() {
            for (var i = 0; i < _report.Def.Fields.length; i++) {
                $('.' + getUniqueFieldClass(_report.Def.Fields[i]) + ' input').prop('checked', true).trigger('change');
            }
        }

        function getUniqueFieldClass(field) {
            var id = field.ID ? field.ID : field.FieldID;
            if (field.IsCustom)
                return 'report-field-id-c' + id;
            else
                return 'report-field-id-s' + id;
        }

        $('.action-save').click(function (e) {
            e.preventDefault();
            var data = null;
            var isPrivate = $('.report-privacy').val();
            switch (_reportType) {
                case 0: data = JSON.stringify(getTabularObject()); break;
                case 1: data = JSON.stringify(getChartObject()); break;
                case 4: data = JSON.stringify(getSummaryObject('.report-summary-fields', $('.report-filter').reportFilter('getObject'))); break;
                case 5: data = JSON.stringify(getTabularObject()); break;
                case 2:
                    data = $('#external-url').val();
                    if (data.indexOf('https://') < 0) data = 'https://' + data;

                    break;
                default: break;
            }

            parent.Ts.Utils.webMethod("ReportService", "SaveReport", {
                "reportID": _reportID,
                "name": $('.report-name').val(),
                "reportType": _reportType,
                "data": data,
                "isStock": $('#cbStock').prop('checked'),
                "isPrivate": isPrivate
            },
            closeReport,
            function (error) { alert(error.get_message()); });
        });

        function getTabularObject() {
            var tabData = new Object();
            tabData.Fields = new Array();
            $('li.report-field input:checked').each(function (index) {
                var field = $(this).closest('li').data('field');
                var fieldItem = new Object();
                fieldItem.FieldID = field.ID;
                fieldItem.IsCustom = field.IsCustom;
                tabData.Fields.push(fieldItem);
            });
            tabData.Subcategory = $('#selectSubCat').val();
            tabData.Filters = $('.report-filter').reportFilter('getObject');
            return tabData;
        }

        function getChartObject() {
            var chartData = new Object();
            chartData.Filters = $('#chartFilter').reportFilter('getObject');
            chartData.Subcategory = $('#selectSubCat').val();
            chartData.Fields = getSummaryFieldsObject('.report-chartproperties');
            chartData.Chart = getHighChartOptions();
            return chartData;
        }

        function getSummaryObject(el, filters) {
            var sumData = new Object();
            sumData.Filters = filters;
            sumData.Subcategory = $('#selectSubCat').val();
            sumData.Fields = getSummaryFieldsObject(el);
            return sumData;
        }

        function getSummaryFieldsObject(el) {
            var fields = new Object();
            var $el = $(el);
            descs = new Array();
            calcs = new Array();

            $el.find('.summary-desc li').each(function () {
                var desc = new Object();
                var item = $(this);
                var data = item.find('.summary-desc-field option:selected').data('field');
                desc.Field = new Object();
                desc.Field.FieldID = data.ID;
                desc.Field.IsCustom = data.IsCustom;
                desc.Field.FieldType = data.DataType;
                desc.Value1 = item.find('.summary-desc-val1').val();
                descs.push(desc);
            });

            $el.find('.summary-calc li').each(function () {
                var calc = new Object();
                var item = $(this);
                var data = item.find('.summary-calc-field option:selected').data('field');
                calc.Field = new Object();
                calc.Field.FieldID = data.ID;
                calc.Field.IsCustom = data.IsCustom;
                calc.Field.FieldType = data.DataType;
                calc.Aggregate = data.DataType == 'number' ? item.find('.summary-calc-arg').val() : item.find('.summary-calc-text-arg').val();
                calc.Comparator = item.find('.summary-calc-comp').val();
                calc.Value1 = item.find('.summary-calc-val1').val();
                calc.Value2 = item.find('.summary-calc-val2').val();
                calcs.push(calc);
            });


            fields.Descriptive = descs;
            fields.Calculated = calcs;
            return fields;
        }

        function closeReport(report) {
            var result = '/vcr/1_9_0/pages/';
            function getReportUrl() {
                switch (_reportType) {
                    case 0:
                        return 'Reports_View_Tabular.html?ReportID=' + _reportID;
                    case 1:
                        return 'Reports_View_Chart.html?ReportID=' + _reportID;
                    case 2:
                        return 'Reports_View_External.html?ReportID=' + _reportID;
                    case 4:
                        return 'Reports_View_Tabular.html?ReportID=' + _reportID;
                    case 5:
                        return 'TicketView.html?ReportID=' + _reportID;
                }
            }
            if (!report) // canceled
            {
                if (_reportID) // go back to report
                {
                    result = result + getReportUrl();
                } else { // go back to list
                    result = result + 'reports.html';
                }
            } else { // saved
                if (_reportType == 5) {
                    if (_reportID) {
                        result = result + getReportUrl();
                        var isPrivate = $('.report-privacy').val();
                        parent.Ts.MainPage.updateTicketViewItem(report, isPrivate, true);
                    }
                    else {
                        result = result + 'reports.html';
                        var isPrivate = $('.report-privacy').val();
                        parent.Ts.MainPage.addNewTicketView(report, isPrivate, true);
                    }


                }
                else if (_reportID) // go back to report
                {
                    result = result + getReportUrl();
                } else { // go back to list AND open tab
                    result = result + 'reports.html';
                    parent.Ts.MainPage.openReport(report);
                }
            }

            location.assign(result);
        }

        function initFilters() {
            if ($('.filter .filter-group').length < 1) {
                if (_report != null && _report.Def.Filters && _report.Def.Filters.length > 0) {
                    $('.report-filter').reportFilter('loadFilters', _report.Def.Filters);
                    $('#chartFilter').reportFilter('loadFilters', _report.Def.Filters);

                }
            }
        }


        function getHighChartOptions() {
            var options = {};
            var showDataLabels = ($('#chart-data-labels').val() === 'true');
            options.chart = { zoomType: 'x' }
            options.ts = { chartType: $('#chart-type').val(), seriesTitle: $('#chart-series-title').val() }
            options.credits = { enabled: false }
            options.title = { text: $('#chart-title').val(), x: -20 };
            options.subtitle = { text: $('#chart-subtitle').val(), x: -20 };
            options.tooltip = { valueSuffix: ' ' + $('#chart-tip-suffix').val() };
            options.legend = {};
            options.legend.layout = $('#chart-legend-layout').val();
            options.legend.align = $('#chart-legend-align').val();
            options.legend.verticalAlign = $('#chart-legend-valign').val();
            options.yAxis = {};
            options.yAxis.title = { text: $('#chart-series-title').val() };
            options.yAxis.plotLines = [{ value: 0, width: 1, color: '#808080'}];
            switch ($('#chart-type').val()) {
                case 'line':
                    options.plotOptions = {};
                    options.plotOptions.line = { turboThreshold: 100000, dataLabels: { enabled: showDataLabels} };
                    break;
                case 'area':
                    options.chart = { type: 'area' };
                    options.plotOptions = {};
                    options.plotOptions.area = { stacking: 'normal', dataLabels: { enabled: showDataLabels} };
                    break;
                case 'stackedarea':
                    options.chart = { type: 'area' };
                    options.plotOptions = {};
                    options.plotOptions.area = { stacking: 'normal', dataLabels: { enabled: showDataLabels} };
                    break;
                case 'bar':
                    options.chart = { type: 'bar' };
                    options.plotOptions = {};
                    options.plotOptions.bar = { dataLabels: { enabled: showDataLabels} };
                    break;
                case 'stackedbar':
                    options.chart = { type: 'bar' };
                    options.plotOptions = {};
                    options.plotOptions.bar = { stacking: 'normal', dataLabels: { enabled: showDataLabels, color: 'white'} };
                    break;
                case 'column':
                    options.chart = { type: 'column' };
                    options.plotOptions = {};
                    options.plotOptions.column = { dataLabels: { enabled: showDataLabels} };
                    break;
                case 'stackedcolumn':
                    options.chart = { type: 'column' };
                    options.plotOptions = {};
                    options.plotOptions.column = { stacking: 'normal', dataLabels: { enabled: showDataLabels, color: 'white'} };
                    break;
                case 'pie':
                    options.chart = { plotBackgroundColor: null, plotBorderWidth: null, plotShadow: false };
                    options.plotOptions = {
                        pie: {
                            allowPointSelect: true,
                            cursor: 'pointer',
                            dataLabels: { enabled: showDataLabels, format: '<b>{point.name}</b>: {point.y}' },
                            showInLegend: true
                        }
                    }

                    break;
                default:
            }

            return options;
        }


        function buildChart() {
            if (_chartData) {
                var options = getHighChartOptions();
                if (options) createChart('.chart-container', options, _chartData);
            }
        }

        function getChartData(data, callback) {
            parent.Ts.Utils.webMethod("ReportService", "GetChartData",
              { "summaryReportFields": data },
              callback,
              function (error) {
                  alert(error.statusText);
              });
        }

        function initChart(callback) {
            _chartData = null;
            $('.chart-container').empty();
            if (_report != null && _report.Def.Chart) {
                var options = _report.Def.Chart;
                $('#chart-title').val(options.title.text);
                $('#chart-type').val(options.ts.chartType);
                $('#chart-subtitle').val(options.subtitle.text);
                $('#chart-series-title').val(options.ts.seriesTitle);
                $('#chart-tip-suffix').val(options.tooltip.valueSuffix);
                $('#chart-legend-layout').val(options.legend.layout);
                $('#chart-legend-align').val(options.legend.align);
                $('#chart-legend-valign').val(options.legend.verticalAlign);

                if (options.plotOptions.hasOwnProperty(options.ts.chartType) && options.plotOptions[options.ts.chartType].hasOwnProperty('dataLabels.endabled')) {
                    $('#chart-data-labels').val(options.plotOptions[options.ts.chartType].dataLabels.enabled.toString());
                }
                else if (options.plotOptions.hasOwnProperty(options.chart.type) && options.plotOptions[options.chart.type].hasOwnProperty('dataLabels.endabled')) {
                    $('#chart-data-labels').val(options.plotOptions[options.chart.type].dataLabels.enabled.toString());
                }
                else {
                    $('#chart-data-labels').val(false.toString());
                }

            }
            if (callback) callback();
        }

        $('.chart-generate').click(function (e) {
            e.preventDefault();
            buildChart();
        });

        $('.chart-options input, .chart-options select').change(function (e) {
            buildChart();
        });

        $('.filter-modal').modal({
            "show": false,
            "backdrop": 'static'
        });

        $('.chart-apply-filter').click(function (e) {
            e.preventDefault();
            $('.filter-modal').modal('show');
        });

        $('.filter-modal').on('hidden.bs.modal', function (e) {
            $('.chart-apply-data').trigger('click');
        })

        $('.chart-apply-data').click(function (e) {
            e.preventDefault();

            getChartData(JSON.stringify(getSummaryObject('.report-chartproperties', $('#chartFilter').reportFilter('getObject'))),
                function (data) {
                    if (data) {
                        _chartData = JSON.parse(data);
                        buildChart();
                    }
                });

        });

    }

});