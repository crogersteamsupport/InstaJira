(function ($, window, document, undefined) {
    "use strict";

    var pluginName = "reportFilter",
      dataPlugin = "plugin_" + pluginName,
      defaults = { "filters": null, "fields": null };

    function ReportFilter(element) {
        this.element = element;
        this._defaults = defaults;
        this._name = pluginName;
    }

    var getUniqueFieldClass = function (field) {
    	//var id = field.ID ? field.ID : field.FieldID;
    	var id;
    	if (field.ID) id = field.ID;
    	else if (field.FieldID > 0) id = field.FieldID;
    	else id = field.FieldName;

    	if (field.IsCustom)
    		return 'report-field-id-c' + id;
    	else
    		return 'report-field-id-s' + id;
    }


    ReportFilter.prototype = {
        init: function (options) {
            var self = this;
            var el = $(this.element);

            this.options = $.extend({}, this._defaults, options);
            var options = this.options;

            var execGetFieldValues = null;

            function getFieldValues(request, response) {
                if (execGetFieldValues) {
                    execGetFieldValues._executor.abort();
                }
                execGetFieldValues = top.Ts.Services.Reports.GetLookupDisplayNames($(this.element).data('fieldid'), request.term, function (result) {
                    response(result);
                    $(this).removeClass('ui-autocomplete-loading');
                });
            }


            el.load("ReportFilters.html", function () {
                el.find('.filter-template-comps option').each(function () {
                    $(this).attr('value', $(this).text().toUpperCase());
                });

                $('<div>').addClass('filter').appendTo(el);
                $('<button>')
                        .attr('type', 'button')
                        .text('Add a Filter')
                        .addClass('btn btn-primary filter-new')
                        .appendTo(el)
                        .click(function (e) {
                            e.preventDefault();
                            $(this).hide();
                            self.addGroupCondition(self.addGroup(el.find('.filter')).find('.filter-conds:first'));
                        });

                if (options.fields != null) {
                    self.loadFields(options.fields);

                    if (options.filters != null) {
                        self.loadFilters(options.filters);
                    }
                }


                el.on('click', '.filter-conj', function (e) {
                    e.preventDefault();
                    var btn = $(this);
                    if (btn.text() === 'And') {
                        btn.text('Or');
                    } else {
                        btn.text('And');
                    }
                });

                el.on('click', '.filter-add-cond', function (e) {
                    e.preventDefault();
                    var list = $(this).closest('.filter-content').find('.filter-conds:first');
                    self.addGroupCondition(list);
                });


                el.on('click', '.filter-add-group', function (e) {
                    e.preventDefault();
                    var subs = $(this).closest('.filter-content').find('.filter-subs:first');
                    self.addGroupCondition(self.addGroup(subs).find('.filter-conds:first'));
                });

                el.on('click', '.filter-remove-cond', function (e) {
                    e.preventDefault();
                    if ($(this).closest('ul').find('li').length <= 1) {
                        $(this).closest('table').remove();
                        if (el.find('.filter .filter-group').length < 1) {
                            el.find('.filter-new').show();
                        }

                    } else {
                        $(this).closest('li').remove();
                    }
                });

                el.on('change', '.filter-field', function (e) {
                    e.preventDefault();
                    $(this).closest('li').find('.filter-comp').remove();
                    $(this).closest('li').find('.filter-value').remove();
                    var field = $(this).find(':selected').data('field');
                    var comp = null;
                    switch (field.DataType) {
                        case 'datetime':
                            comp = el.find('.filter-template-comps .filter-comp-datetime').clone().insertAfter($(this));
                            break;
                        case 'bool':
                            comp = el.find('.filter-template-comps .filter-comp-bool').clone().insertAfter($(this));
                            break;
                        case 'number':
                            comp = el.find('.filter-template-comps .filter-comp-number').clone().insertAfter($(this));
                            break;
                        default:
                            comp = el.find('.filter-template-comps .filter-comp-text').clone().insertAfter($(this));
                            break;
                    }
                    comp.trigger('change');
                });

                el.on('change', '.filter-comp', function (e) {
                    e.preventDefault();
                    var field = $(this).closest('li').find('.filter-field').find(':selected').data('field');
                    $(this).closest('li').find('.filter-value').remove();
                    var arg1type = $(this).find(':selected').data('argtype-1');
                    if (arg1type) {
                        switch (arg1type) {
                            case 'int':
                                $('<input type="text">').addClass('form-control filter-value').insertAfter($(this)).numeric({
                                    'decimal': false
                                });
                                break;
                            case 'float':
                                $('<input type="text">').addClass('form-control filter-value').insertAfter($(this)).numeric();
                                break;
                            case 'month':
                                var months = $('<select>').addClass('form-control filter-value').insertAfter($(this));
                                $('<option>').attr('value', 1).text('January').appendTo(months);
                                $('<option>').attr('value', 2).text('February').appendTo(months);
                                $('<option>').attr('value', 3).text('March').appendTo(months);
                                $('<option>').attr('value', 4).text('April').appendTo(months);
                                $('<option>').attr('value', 5).text('May').appendTo(months);
                                $('<option>').attr('value', 6).text('June').appendTo(months);
                                $('<option>').attr('value', 7).text('July').appendTo(months);
                                $('<option>').attr('value', 8).text('August').appendTo(months);
                                $('<option>').attr('value', 9).text('September').appendTo(months);
                                $('<option>').attr('value', 10).text('October').appendTo(months);
                                $('<option>').attr('value', 11).text('November').appendTo(months);
                                $('<option>').attr('value', 12).text('December').appendTo(months);
                                break;
                            case 'day':
                                var days = $('<select>').addClass('form-control filter-value').insertAfter($(this));
                                $('<option>').attr('value', 1).text('Sunday').appendTo(days);
                                $('<option>').attr('value', 2).text('Monday').appendTo(days);
                                $('<option>').attr('value', 3).text('Tuesday').appendTo(days);
                                $('<option>').attr('value', 4).text('Wednesday').appendTo(days);
                                $('<option>').attr('value', 5).text('Thursday').appendTo(days);
                                $('<option>').attr('value', 6).text('Friday').appendTo(days);
                                $('<option>').attr('value', 7).text('Saturday').appendTo(days);
                                break;
                            case 'date':
                                $('<input type="text">').addClass('form-control filter-value').insertAfter($(this)).datepicker({
                                    icons: {
                                        time: "fa fa-clock-o",
                                        date: "fa fa-calendar",
                                        up: "fa fa-arrow-up",
                                        down: "fa fa-arrow-down"
                                    },
                                    pickTime: false
                                });
                                break;
                            default:
                                var input = $('<input type="text">').addClass('form-control filter-value').insertAfter($(this));
                                if (field.LookupTableID) {
                                    input.autocomplete({
                                        minLength: 2,
                                        source: getFieldValues,
                                        appendTo: el.closest('.modal'),
                                        select: function (event, ui) { }
                                    });
                                    input.data('fieldid', field.ID);
                                }
                        }
                    }
                });

            });

        },

        addCondition: function (fieldID, isCustom) {
            var el = $(this.element);
            var list = el.find('.filter .filter-conds:first');
            if (list.length < 1) {
                list = this.addGroup(el.find('.filter')).find('.filter-conds:first');
            }
            var condition = this.addGroupCondition(list);
            var field = new Object;
            field.ID = fieldID
            field.IsCustom = isCustom;
            condition.find('option.' + getUniqueFieldClass(field)).prop('selected', true).closest('select').trigger('change');

        },

        addGroupCondition: function (list) {
            var el = $(this.element);
            el.find('.filter-new').hide();
            var clone = el.find('.filter-template-cond li.filter-cond').clone(true);
            clone.appendTo(list);
            clone.appendTo(list).hide().find('.filter-field').trigger('change');
            clone.fadeIn();
            return clone;
        },

        addGroup: function (container) {
            return $(this.element).find('.filter-template-body table').clone().appendTo(container).hide().fadeIn();
        },

        loadFields: function (fields) {
            var el = $(this.element);
            el.find('.filter').empty();
            el.find('.filter-new').show();

            var select = el.find('.filter-template-cond .filter-field').empty();
            var tableName = "";
            var optGroup = null;
            for (var i = 0; i < fields.length; i++) {
                if (tableName != fields[i].Table) {
                    tableName = fields[i].Table;
                    optGroup = $('<optgroup>').attr('label', tableName).appendTo(select);
                }
                var fieldName = fields[i].Name + (fields[i].AuxName ? " (" + fields[i].AuxName + ")" : "");
                $('<option>').text(fieldName).data('field', fields[i]).addClass(getUniqueFieldClass(fields[i])).appendTo(optGroup);
            }
        },
        getDefaults: function () {
            return this._defaults;
        },

        getOptions: function (options) {
            options = $.extend({}, this.getDefaults(), $(this.element).data(), options)
            return options
        },

        getObject: function () {
            var result = new Object();

            function getFilterObject(el, obj) {
                el = $(el);
                obj.Filters = new Array();

                el.find('>.filter-group').each(function () {
                    var group = new Object();
                    var table = $(this);
                    group.Conjunction = table.find('.filter-conj:first').text().toUpperCase();
                    group.Conditions = new Array();

                    table.find('.filter-conds:first').find('.filter-cond').each(function () {
                        var condition = new Object();
                        var field = $(this).find(':selected').data('field');
                        condition.FieldID = field.ID === parseInt(field.ID) ? field.ID : -1;
                        condition.IsCustom = field.IsCustom;
                        condition.FieldName = field.Name;
                        condition.DataType = field.DataType == 'number' ? 'float' : field.DataType;
                        var comp = $(this).find('.filter-comp');
                        condition.Comparator = comp.val().toUpperCase();
                        var next = comp.nextAll('input, select');
                        condition.Value1 = (next ? next.val() : null);
                        group.Conditions.push(condition);
                    });

                    obj.Filters.push(group);
                    getFilterObject(table.find('.filter-subs:first'), group);
                });

                return obj;
            }

            getFilterObject($(this.element).find('.filter'), result);
            return result.Filters;
        },

        loadFilters: function (filters) {
            this.clear();
            var el = $(this.element);
            var self = this;

            function loadSubFilters(container, filters) {
                for (var i = 0; i < filters.length; i++) {
                    var filter = filters[i];
                    var group = self.addGroup(container);
                    group.find('.filter-conj').text(filter.Conjunction == 'AND' ? 'And' : 'Or');
                    for (var j = 0; j < filter.Conditions.length; j++) {
                        var condition = filter.Conditions[j];
                        var li = self.addGroupCondition(group.find('.filter-conds:first'));
                        var test = '.filter-field option.' + getUniqueFieldClass(condition);
                        li.find('.filter-field option.' + getUniqueFieldClass(condition)).prop('selected', true).closest('select').trigger('change');
                        li.find('.filter-comp').val(condition.Comparator.toUpperCase()).trigger('change');
                        if (condition.Value1) li.find('.filter-value').val(condition.Value1);

                    }
                    loadSubFilters(group.find('.filter-subs:first'), filter.Filters);
                }
            }
            if (filters[0] != null) {
                loadSubFilters(el.find('.filter'), filters);
            } else {
                el.find('.filter-new').show();
            }
        },

        clear: function () {
            var el = $(this.element);
            el.find('.filter').empty();
            el.find('.filter-new').hide();
        }
    }


    $.fn[pluginName] = function (arg) {
        var args, instance;

        if (!(this.data(dataPlugin) instanceof ReportFilter)) {
            this.data(dataPlugin, new ReportFilter(this));
        }

        instance = this.data(dataPlugin);

        instance.element = this;

        if (typeof arg === 'undefined' || typeof arg === 'object') {

            if (typeof instance['init'] === 'function') {
                instance.init(arg);
            }
        } else if (typeof arg === 'string' && typeof instance[arg] === 'function') {
            args = Array.prototype.slice.call(arguments, 1);
            return instance[arg].apply(instance, args);
        } else {
            $.error('Method ' + arg + ' does not exist on jQuery.' + pluginName);
        }
    }

})(jQuery, window, document);