var _firstItemIndex = 0;
var _pageSize = 20;
var _advancedSearchOptions = null;
var _standardFilterID = null;
var _addingFilter = false;
var _addingSorter = false;
//var _completeCommentTaskID = 0;

$(document).ready(function () {

    layout = $('body').layout({
        applyDefaultStyles: true
      , east__size: 283
      , east__resizable: false
      , east__closable: false
      , spacing_open: 0
    });

    LoadAdvancedOptions();

    $('#search-results-loading').hide();

    $('#search-button').click(function () {
        _firstItemIndex = 0; GetSearchResults();
        parent.Ts.System.logAction('Search - Searched');
    });

    $('#search-results-pane').bind('scroll', function () {
        if ($(this).scrollTop() > 0 && $(this).scrollTop() + $(this).innerHeight() >= ($(this)[0].scrollHeight * 0.9)) {
            GetSearchResultsNextPage();
        }
    });

    // The include-all is also a checkbox. When clicked first will handle the IncludeAllClickEvent and afterwards the StandardFilter click event.
    $('#include-all').click(function () { HandleIncludeAllClickEvent(); });
    $('.checkbox').click(function () { HandleStandardFilterClickEvent(); });
    $('#search-options-add-filter').click(function () { HandleAddFilterClickEvent(); });
    $('#search-options-add-sorter').click(function () { HandleAddSorterClickEvent(); });
    $('#search-results').on('click', '.change-task-status', function () {
        parent.Ts.System.logAction('Search - Change Task Status');
        var id = $(this).attr('id');
        var checkbox = $(this);
        var checked = $(this).prop('checked');
        var name = $(this).attr('name');

        parent.Ts.Services.Task.SetTaskIsCompleted(id, checked, function (data) {
            if (data.IncompleteSubtasks) {
                checkbox.prop("checked", false);
                alert('There are subtasks pending completion, please finish them before completing the parent task.')
            }
            else if (checked) {
                $('#task' + id).html('<strike>' + name + '</strike>');
                //_completeCommentTaskID = id;
                //$('#modalTaskComment').modal('show');
            }
            else {
                $('#task' + id).html(name);
            }
        });
    });

    //$('#btnTaskCompleteComment').on('click', function (e) {
    //    e.preventDefault();
    //    if ($('#taskCompleteComment').val() == '') {
    //        alert('Please type your comments before clicking on the Yes button.');
    //    }
    //    else {
    //        window.parent.parent.Ts.System.logAction('Task - Add Task Complete Comment');
    //        window.parent.parent.Ts.Services.Task.AddTaskCompleteComment(_completeCommentTaskID, $('#taskCompleteComment').val(), function (result) {
    //            if (result.Value) {
    //                $('#taskCompleteComment').val('');
    //                $('#modalTaskComment').modal('hide');
    //            }
    //            else {
    //                alert('There was an error saving your comment. Please try again.')
    //            }
    //        });
    //    }
    //});

});

function LoadAdvancedOptions() {
    if (_advancedSearchOptions == null) {
        parent.Ts.Services.Search.GetAdvancedSearchOptions(function (advancedSearchOptions) {

            _advancedSearchOptions = advancedSearchOptions;
            _standardFilterID = advancedSearchOptions.StandardFilterID;

            LoadStandardFilters();
            LoadCustomFilters();
            LoadCustomSorters();

        });
    }
    else {
        LoadStandardFilters();
        LoadCustomFilters();
        LoadCustomSorters();
    }
}

function LoadStandardFilters() {
    if (parent.Ts.System.Organization.ProductType > 0) {
        $('#search-standard-filters').append($('<p style="margin-bottom:5px;"><input id="include-notes" class="checkbox" type="checkbox" onclick="HandleStandardFilterClickEvent()" style="margin-right:5px;"> Customer Activities</p>'));

        if (parent.Ts.System.Organization.ProductType > 1) {
            $('#search-standard-filters').append($('<p style="margin-bottom:5px;"><input id="include-product-versions" class="checkbox" type="checkbox" onclick="HandleStandardFilterClickEvent()" style="margin-right:5px;"> Product Versions</p>'));
            $('#search-standard-filters').append($('<p style="margin-bottom:5px;"><input id="include-tasks" class="checkbox" type="checkbox" onclick="HandleStandardFilterClickEvent()" style="margin-right:5px;"> Tasks</p>'));
        }

        $('#search-standard-filters').append($('<p style="margin-bottom:5px;"><input id="include-water-cooler" class="checkbox" type="checkbox" onclick="HandleStandardFilterClickEvent()" style="margin-right:5px;"> Water Cooler</p>'));
    }
    else {
        $('#include-wikis-span').hide();
    }

    if (_standardFilterID != null) {
        $('#include-tickets').prop('checked', _advancedSearchOptions.Tickets);
        $('#include-knowledge-base').prop('checked', _advancedSearchOptions.KnowledgeBase);
        $('#include-wikis').prop('checked', _advancedSearchOptions.Wikis);
        $('#include-notes').prop('checked', _advancedSearchOptions.Notes);
        $('#include-product-versions').prop('checked', _advancedSearchOptions.ProductVersions);
        $('#include-tasks').prop('checked', _advancedSearchOptions.Tasks);
        $('#include-water-cooler').prop('checked', _advancedSearchOptions.WaterCooler);

        if (
          $('#include-tickets').prop('checked')
          && $('#include-knowledge-base').prop('checked')
          && $('#include-wikis').prop('checked')
          && ($('#include-notes').length == 0 || $('#include-notes').prop('checked'))
          && ($('#include-product-versions').length == 0 || $('#include-product-versions').prop('checked'))
          && ($('#include-tasks').length == 0 || $('#include-tasks').prop('checked'))
          && ($('#include-water-cooler').length == 0 || $('#include-water-cooler').prop('checked'))
        ) {
            $('#include-all').prop('checked', true);
        }
    }
    else {
        $('.checkbox').prop('checked', true);
    }
}

function LoadCustomFilters() {

    if (_advancedSearchOptions.CustomFilters.length > 0) {
        for (var i = 0; i < _advancedSearchOptions.CustomFilters.length; i++) {

            var customFilterID = _advancedSearchOptions.CustomFilters[i].CustomFilterID;
            var reportTableFieldID = _advancedSearchOptions.CustomFilters[i].FieldID;
            var measure = _advancedSearchOptions.CustomFilters[i].Measure;
            var testValue = _advancedSearchOptions.CustomFilters[i].TestValue;

            AddCustomFilter(customFilterID, reportTableFieldID, measure, testValue);

        }
    }
    else {
        AddCustomFilter();
        _addingFilter = true;
    }
}

function LoadCustomSorters() {

    if (_advancedSearchOptions.Sorters.length > 0) {

        for (var i = 0; i < _advancedSearchOptions.Sorters.length; i++) {

            var sorterID = _advancedSearchOptions.Sorters[i].SorterID;
            var fieldID = _advancedSearchOptions.Sorters[i].FieldID;
            var descending = _advancedSearchOptions.Sorters[i].Descending;
            AddCustomSorter(sorterID, fieldID, descending);

        }
    }
    else {
        AddCustomSorter();
        _addingSorter = true;
    }
}

function AddCustomFilter(customFilterID, reportTableFieldID, measure, testValue) {
    if (_addingFilter) {
        return;
    }

    var div = $('<div>').addClass('condition');
    var displayFieldSet = $('<fieldset>').addClass('display-condition');

    var fieldText = $('<p>');
    var measureText = $('<p>');
    var valueText = $('<p>');

    if (customFilterID) {
        div.addClass('filter-' + customFilterID);
        displayFieldSet.addClass('filter-' + customFilterID);
    }

    var fields = $('<select>').addClass('condition-field').addClass('form-control').appendTo(div).width('180px');
    loadComboFields(fields, true).combobox({
        selected: function (e, ui) {
            createConditionValue(div, fields.find('option:selected').data('field'));
        }
    });

    if (reportTableFieldID) {
        fields.combobox('setValue', reportTableFieldID);
        fieldText.html(fields.find('option:selected').text());
    }

    var measures = $('<select>').addClass('condition-measure').addClass('form-control').appendTo(div).width('150px');
    loadComboMeasures(measures).combobox({
        selected: function (e, ui) {
        }
    });

    if (measure) {
        measures.combobox('setValue', measure);
        measureText.html(measures.find('option:selected').text());
    }

    $('<span>').addClass('condition-value-container').appendTo(div);

    if (testValue) {
        createConditionValue(div, fields.find('option:selected').data('field'), testValue);
        valueText.html(testValue)
    }
    else {
        createConditionValue(div, fields.find('option:selected').data('field'));
    }

    var deleteIcon = $('<span>')
      .addClass('ui-icon ui-icon-close')
      .click(function (e) {
          var clickedCustomFilterID = Ts.Utils.getIdFromElement('filter', $(this).parent());
          parent.Ts.Services.Search.DeleteCustomFilter(clickedCustomFilterID, function () { });
          div.remove();
          displayFieldSet.remove();
          setTimeout(function () { $('#search-button').trigger('click'); }, 5);
      });
    deleteIcon.css('float', 'right');
    deleteIcon.appendTo(displayFieldSet);

    var editIcon = $('<span>')
      .addClass('ui-icon ui-icon-pencil')
      .click(function (e) {
          div.show();
          displayFieldSet.hide();
      });
    editIcon.css('float', 'right');
    editIcon.appendTo(displayFieldSet);

    fieldText.appendTo(displayFieldSet);
    measureText.appendTo(displayFieldSet);
    valueText.appendTo(displayFieldSet);


    var iconsDiv = $('<div>');
    iconsDiv.css('margin-top', '3px');

    var saveIcon = $('<span>')
      .addClass('fa fa-check-square')
      .click(function (e) {
          parent.Ts.System.logAction('Search - Filter Added');
          var fieldID = $(this).parent().parent().find('.condition-field').val();
          var measure = $(this).parent().parent().find('.condition-measure').val();
          var value = $(this).parent().parent().find('.condition-value').val();
          var isCustom = $(this).parent().parent().find('.condition-field option:selected').data('field').IsCustom;
          var clickedCustomFilterID = Ts.Utils.getIdFromElement('filter', $(this).parent().parent());
          if (clickedCustomFilterID == null) {
              parent.Ts.Services.Search.AddCustomFilter(fieldID, measure, value, isCustom, function (result) {
                  div.addClass('filter-' + result);
                  displayFieldSet.addClass('filter-' + result);
                  _addingFilter = false;
              },
                function () {
                    alert('An error occurred saving filter. Please try again later.');
                });
          }
          else {
              parent.Ts.Services.Search.UpdateCustomFilter(clickedCustomFilterID, fieldID, measure, value, isCustom);
          }
          fieldText.html(fields.find('option:selected').text());
          measureText.html(measures.find('option:selected').text());
          valueText.html(value);

          if (clickedCustomFilterID == null) {
              $(div).hide().appendTo('#search-options-filters');
              displayFieldSet.show("slow");
          }
          else {
              div.hide();
              displayFieldSet.show();
          }

          setTimeout(function () { $('#search-button').trigger('click'); }, 5);
      });
    saveIcon.css('float', 'left');
    saveIcon.css('margin-left', '5px');
    saveIcon.css('margin-right', '5px');
    saveIcon.appendTo(iconsDiv);

    var cancelIcon = $('<span>')
      .addClass('fa fa-window-close')
      .click(function (e) {
          if (Ts.Utils.getIdFromElement('filter', $(this).parent().parent()) == null) {
              $(this).parent().parent().remove();
              _addingFilter = false;
          }
          else {
              fields.combobox('setValue', reportTableFieldID);
              measures.combobox('setValue', measure);
              createConditionValue(div, fields.find('option:selected').data('field'), testValue);

              div.hide();
              displayFieldSet.show();
          }
      });
    cancelIcon.css('float', 'left');
    cancelIcon.css('margin-right', '5px');
    cancelIcon.appendTo(iconsDiv);

    iconsDiv.appendTo(div);

    $(displayFieldSet).appendTo('#search-options-filters');

    if (reportTableFieldID) {
        $(div).appendTo('#search-options-filters').hide();
        displayFieldSet.show();
    }
    else {
        $(div).appendTo('#search-options-new-filter').show();
        displayFieldSet.hide();
    }
}

function AddCustomSorter(sorterID, fieldID, descending) {
    if (_addingSorter) {
        return;
    }

    var div = $('<div>').addClass('condition');
    var displayFieldSet = $('<fieldset>').addClass('display-condition');

    var fieldText = $('<p>');
    var directionText = $('<p>');

    if (sorterID) {
        div.addClass('sorter-' + sorterID);
        displayFieldSet.addClass('sorter-' + sorterID);
    }

    var fields = $('<select>').addClass('condition-field').appendTo(div).width('180px');
    loadComboFields(fields, false).combobox({
        selected: function (e, ui) {
        }
    });

    if (fieldID) {
        fields.combobox('setValue', fieldID);
        fieldText.html(fields.find('option:selected').text());
    }

    var directions = $('<select>').addClass('condition-direction').appendTo(div);
    loadComboDirections(directions).combobox({
        selected: function (e, ui) {
        }
    });

    if (descending) {
        directions.combobox('setValue', descending);
        directionText.html('Descending');
    }
    else {
        directionText.html('Ascending');
    }

    var deleteIcon = $('<span>')
      .addClass('ui-icon ui-icon-close')
      .click(function (e) {
          var clickedSorterID = Ts.Utils.getIdFromElement('sorter', $(this).parent());
          parent.Ts.Services.Search.DeleteSorter(clickedSorterID, function () { });
          div.remove();
          displayFieldSet.remove();
          setTimeout(function () { $('#search-button').trigger('click'); }, 5);
      });
    deleteIcon.css('float', 'right');
    deleteIcon.appendTo(displayFieldSet);

    var editIcon = $('<span>')
      .addClass('ui-icon ui-icon-pencil')
      .click(function (e) {
          div.show();
          displayFieldSet.hide();
      });
    editIcon.css('float', 'right');
    editIcon.appendTo(displayFieldSet);

    fieldText.appendTo(displayFieldSet);
    directionText.appendTo(displayFieldSet);

    var iconsDiv = $('<div>').addClass('condition-sorter-icons');

    var saveIcon = $('<span>')
          .addClass('fa fa-check-square')
          .click(function (e) {
              parent.Ts.System.logAction('Search - Sort Condition Added');
              var fieldID = $(this).parent().parent().find('.condition-field').val();
              var descending = $(this).parent().parent().find('.condition-direction').val();
              var isCustom = $(this).parent().parent().find('.condition-field option:selected').data('field').IsCustom;
              var clickedSorterID = Ts.Utils.getIdFromElement('sorter', $(this).parent().parent());
              if (clickedSorterID == null) {
                  parent.Ts.Services.Search.AddSorter(fieldID, descending, isCustom, function (result) {
                      div.addClass('sorter-' + result);
                      displayFieldSet.addClass('sorter-' + result);
                      _addingSorter = false;
                  },
                  function () {
                      alert('An error occurred saving sorter. Please try again later.');
                  });
              }
              else {
                  parent.Ts.Services.Search.UpdateSorter(clickedSorterID, fieldID, descending, isCustom);
              }

              fieldText.html(fields.find('option:selected').text());

              if (descending == 'true') {
                  directionText.html('Descending');
              }
              else {
                  directionText.html('Ascending');
              }

              if (clickedSorterID == null) {
                  $(div).hide().appendTo('#search-options-sorters');
                  displayFieldSet.show("slow");
              }
              else {
                  div.hide();
                  displayFieldSet.show();
              }

              setTimeout(function () { $('#search-button').trigger('click'); }, 5);
          });
    saveIcon.css('float', 'left');
    saveIcon.css('margin-right', '5px');
    saveIcon.appendTo(iconsDiv);

    var cancelIcon = $('<span>')
          .addClass('fa fa-window-close')
          .click(function (e) {
              if (Ts.Utils.getIdFromElement('sorter', $(this).parent().parent()) == null) {
                  $(this).parent().parent().remove();
                  _addingSorter = false;
              }
              else {
                  fields.combobox('setValue', fieldID);
                  directions.combobox('setValue', descending);

                  div.hide();
                  displayFieldSet.show();

              }
          });

    cancelIcon.css('float', 'left');
    cancelIcon.css('margin-right', '5px');
    cancelIcon.appendTo(iconsDiv);

    iconsDiv.appendTo(div);

    $(displayFieldSet).appendTo('#search-options-sorters');

    if (fieldID) {
        $(div).appendTo('#search-options-sorters').hide();
        displayFieldSet.show();
    }
    else {
        $(div).appendTo('#search-options-new-sorter').show();
        displayFieldSet.hide();
    }
}

function loadComboFields(select, withCustomFields) {
    select.html('');
    $('<option>').attr('value', '-1').text('-- Select a Field --').appendTo(select).attr('selected', 'selected');
    if (_advancedSearchOptions == null) {
        parent.Ts.Services.Search.GetAdvancedSearchOptions(function (advancedSearchOptions) {
            _advancedSearchOptions = advancedSearchOptions;
            PopulateComboFields(select, withCustomFields);
        });
    }
    else {
        PopulateComboFields(select, withCustomFields);
    }

    return select;
}

function PopulateComboFields(select, withCustomFields) {
    for (var i = 0; i < _advancedSearchOptions.Fields.length; i++) {
        if (withCustomFields || !_advancedSearchOptions.Fields[i].IsCustom) {
            $('<option>').attr('value', _advancedSearchOptions.Fields[i].FieldID).text(_advancedSearchOptions.Fields[i].Alias).appendTo(select).data('field', _advancedSearchOptions.Fields[i]);
        }
    }
}

function loadComboMeasures(select) {
    select.html('');
    $('<option>').attr('value', '<').text('Less Than').appendTo(select);
    $('<option>').attr('value', '<=').text('Less Than or Equal To').appendTo(select);
    $('<option>').attr('value', '=').attr('selected', 'selected').text('Equal To').appendTo(select);
    $('<option>').attr('value', '<>').text('Not Equal To').appendTo(select);
    $('<option>').attr('value', '>=').text('Greater Than or Equal To').appendTo(select);
    $('<option>').attr('value', '>').text('Greater Than').appendTo(select);
    $('<option>').attr('value', 'contains').text('Contains').appendTo(select);
    return select;
}

function loadComboDirections(select) {
    select.html('');
    $('<option>').attr('value', false).attr('selected', 'selected').text('Ascending').appendTo(select);
    $('<option>').attr('value', true).text('Descending').appendTo(select);
    return select;
}

function createConditionValue(condition, field, value) {
    var container = condition.find('.condition-value-container').empty();
    if (!field) {
        var input = $('<input>')
          .addClass('text ui-corner-all ui-widget-content condition-value')
          .attr('type', 'text')
          .width('150px')
          .height('20px')
          .css('padding','5px')
          .keydown(function () { isModified(true, condition); })
          .appendTo(container);
        return;
    }

    var execGetFieldValues = null;
    function getFieldValues(request, response) {
        if (execGetFieldValues) { execGetFieldValues._executor.abort(); }
        execGetFieldValues = parent.Ts.Services.System.GetLookupDisplayNames(condition.find('.condition-field').val(), request.term, function (result) { response(result); $(this).removeClass('ui-autocomplete-loading'); });
    }

    if (field.DataType == 'bit') {
        var select = $('<select>')
            .addClass('condition-value')
            .width('125px')
            .height('20px')
            .appendTo(container);
        $('<option>').attr('value', 'true').text('True').appendTo(select);
        $('<option>').attr('value', 'false').text('False').appendTo(select);

        if (value && value == 'false') select.find('option:last').attr('selected', 'selected'); else select.find('option:first').attr('selected', 'selected');
        select.combobox({ selected: function (e, ui) { isModified(true, condition); } })
    }
    else if (field.DataType == 'list') {
        var select = $('<select>')
            .addClass('condition-value')
            .height('20px')
            .appendTo(container);
        for (var i = 0; i < field.ListValues.length; i++) {
            $('<option>').attr('value', field.ListValues[i]).text(field.ListValues[i]).appendTo(select);
        }

        if (value) select.find('option[value="' + value + '"]').attr('selected', 'selected'); else select.find('option:first').attr('selected', 'selected');
        select.combobox({ selected: function (e, ui) { isModified(true, condition); } })
    }
    else {
        var input = $('<input>')
            .addClass('text ui-corner-all ui-widget-content condition-value')
            .attr('type', 'text')
            .width('150px')
            .height('20px')
            .css('padding','5px')
            .keydown(function () { isModified(true, condition); })
            .appendTo(container)
            .val((value ? value : ""));
        if (field.LookupTableID != null) {
            input.autocomplete({ minLength: 2, source: getFieldValues, select: function (event, ui) { } });
        }
        else if (field.DataType == 'datetime') {
            input.datetimepicker().change(function () { isModified(true, condition); });
        }
    }
}

function isModified(value, div) {
}

$(document).keypress(function (e) {
    if (e.which == 13) {
        setTimeout(function () { $('#search-button').trigger('click'); }, 5);
        $(document).blur();
    }
});

function GetSearchResults() {
    if (_firstItemIndex == 0) {
        $('.resultsSummary').hide();
        $('#search-results').hide();
        $('#search-results-loading').show();
    }
    parent.Ts.Services.Search.GetSearchResults($('#search-input').val(), _firstItemIndex, _pageSize, function (results) { showSearchResults(results); });
}

function GetSearchResultsNextPage() {
    _firstItemIndex += _pageSize;
    GetSearchResults();
}

var _ticketTypes = parent.Ts.Cache.getTicketTypes();

function showSearchResults(results) {
    if (results == null) return;
    $('#search-results-summary').html(
    '<div class="resultsSummary">' +
        results.Count + ' items found.' +
      '</div>' +
      '<div class="ui-helper-clearfix" />')
    var html = '';
    for (var i = 0; i < results.Items.length; i++) {
        var iconPath = "";
        var onClickHandler = "";
        var subText = "";

        var text = '>' + results.Items[i].DisplayName;

        switch (results.Items[i].TypeID) {
            case 1: //Tickets
                iconPath = '<img alt="Result item icon" src="/vcr/1_9_0/images/nav/16/tickets.png" />';
                onClickHandler = "parent.Ts.MainPage.openTicket(" + results.Items[i].Number + ", true)";
                subText = '<h2>Status: ' + results.Items[i].Status + ' </h2>' +
                        '<h2>Severity: ' + results.Items[i].Severity + '</h2>';
                break;
            case 2: //KnowledgeBase
                iconPath = '<img alt="Result item icon" src="/vcr/1_9_0/images/nav/16/knowledge.png" />';
                onClickHandler = "parent.Ts.MainPage.openTicket(" + results.Items[i].Number + ", true)";
                subText = '<h2>Status: ' + results.Items[i].Status + ' </h2>' +
                        '<h2>Severity: ' + results.Items[i].Severity + '</h2>';
                break;
            case 3: //Wikis
                iconPath = '<img alt="Result item icon" src="/vcr/1_9_0/images/nav/16/wiki.png" />';
                onClickHandler = "parent.Ts.MainPage.openWiki(" + results.Items[i].ID + ", true)";
                subText = '<h2>Created by: ' + results.Items[i].Creator + ' </h2>' +
                        '<h2>Modified by: ' + results.Items[i].Modifier + '</h2>';
                break;
            case 4: //CustomerNotes
                iconPath = '<img alt="Result item icon" src="/vcr/1_9_0/images/nav/16/customers.png" />';
                //onClickHandler = "parent.Ts.MainPage.openCustomerNote(" + results.Items[i].CustomerID + ", " + results.Items[i].ID + ", true)";
                onClickHandler = "parent.Ts.MainPage.openNewCustomerNote(" + results.Items[i].CustomerID + ", " + results.Items[i].ID + ")";
                subText = '<h2>Created by: ' + results.Items[i].Creator + ' </h2>' +
                        '<h2>Modified on: ' + results.Items[i].DateModified + '</h2>';
                break;
            case 5: //ProductVersions
                iconPath = '<img alt="Result item icon" src="/vcr/1_9_0/images/nav/16/products.png" />';
                onClickHandler = "parent.Ts.MainPage.openNewProductVersion(" + results.Items[i].ID + ")";
                subText = '<h2>Status: ' + results.Items[i].Status + ' </h2>' +
                        '<h2>Modified on: ' + results.Items[i].DateModified + '</h2>';
                break;
            case 6: //WaterCooler
                iconPath = '<img alt="Result item icon" src="/vcr/1_9_0/images/nav/16/watercooler.png" />';
                onClickHandler = "parent.Ts.MainPage.openWaterCoolerInstance(" + results.Items[i].ID + ", " + results.Items[i].RefType + ", " + results.Items[i].AttachmentID + ")";
                subText = '<h2>Posted by: ' + results.Items[i].Creator + ' </h2>' +
                        '<h2>Posted on: ' + results.Items[i].DateModified + '</h2>';
                break;
            case 7: //ContactNotes
                iconPath = '<img alt="Result item icon" src="/vcr/1_9_0/images/nav/16/customers.png" />';
                onClickHandler = "parent.Ts.MainPage.openNewContactNote(" + results.Items[i].CustomerID + ", " + results.Items[i].ID + ")";
                subText = '<h2>Created by: ' + results.Items[i].Creator + ' </h2>' +
                        '<h2>Modified on: ' + results.Items[i].DateModified + '</h2>';
                break;
            case 8: //Tasks
                if (results.Items[i].IsComplete) {
                    iconPath = '<input type="checkbox" class="change-task-status" id="' + results.Items[i].ID + '" name="' + results.Items[i].DisplayName + '" checked />';
                    text = ' id="task' + results.Items[i].ID + '"><strike>' + results.Items[i].DisplayName + '</strike>';
                    subText = '<h2>Assigned to: ' + results.Items[i].UserName + ' </h2>' +
                            '<h2>Completed on: ' + window.parent.parent.Ts.Utils.getMsDate(results.Items[i].DateCompleted).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()) + '</h2>';
                }
                else {
                    iconPath = '<input type="checkbox" class="change-task-status" id="' + results.Items[i].ID + '" name="' + results.Items[i].DisplayName + '" />';
                    text = ' id="task' + results.Items[i].ID + '">' + results.Items[i].DisplayName;
                    if (results.Items[i].IsPastDue) {
                        subText = '<h2>Assigned to: ' + results.Items[i].UserName + ' </h2>' +
                                '<h2>Due Date: <span  style="color:red;">' + window.parent.parent.Ts.Utils.getMsDate(results.Items[i].DueDate).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()) + '</span></h2>';
                    }
                    else if (results.Items[i].DueDate) {
                        subText = '<h2>Assigned to: ' + results.Items[i].UserName + ' </h2>' +
                                '<h2>Due Date: ' + window.parent.parent.Ts.Utils.getMsDate(results.Items[i].DueDate).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()) + '</h2>';
                    }
                    else {
                        subText = '<h2>Assigned to: ' + results.Items[i].UserName + ' </h2>' +
                                '<h2>Due Date: None</h2>';
                    }
                }
                onClickHandler = "parent.Ts.MainPage.openNewTask(" + results.Items[i].ID + ")";
                break;
        }

        html = html +
        '<div class="resultItem">' +
          '<div class="resultItem-left">' +
            '<div class="resultItem-icon">' +
              iconPath +
              '<h2>' + results.Items[i].ScorePercent + '%</h2>' +
            '</div>' +
            '<div class="resultItem-text">' +
              '<h1>' +
                '<a href="#" onclick="' + onClickHandler + '; return false;" class="ts-link"' + text + '</a>' +
              '</h1>' +
              subText +
            '</div>' +
          '</div>' +
          '<div class="ticket-right" />' +
          '<div class="ui-helper-clearfix" />' +
        '</div>';
    }

    if (_firstItemIndex == 0) {
        $('.resultsSummary').show();
        $('#search-results').html(html).show();
        $('#search-results-loading').hide();
    }
    else {
        $('#search-results').append(html);
    }
}

function getTicketIconUrl(ticketTypeID) {
    for (var i = 0; i < _ticketTypes.length; i++) {
        if (_ticketTypes[i].TicketTypeID == ticketTypeID) {
            return _ticketTypes[i].IconUrl;
        }
    }
    return '';
}

function HandleAddFilterClickEvent() {
    //  $('#search-options-filters').find('.ts-icon-save:visible').trigger('click');
    AddCustomFilter();
    _addingFilter = true;
}

function HandleAddSorterClickEvent() {
    //  $('#search-options-sorters').find('.ts-icon-save:visible').trigger('click');
    AddCustomSorter();
    _addingSorter = true;
}

function HandleIncludeAllClickEvent() {
    if ($('#include-all').prop('checked')) {
        $('.checkbox').prop('checked', true);
    }
    else {
        $('.checkbox').prop('checked', false);
    }
}

function HandleStandardFilterClickEvent() {
    parent.Ts.System.logAction('Search - Display Item Changed');
    if (
      $('#include-tickets').prop('checked')
      && $('#include-knowledge-base').prop('checked')
      && $('#include-wikis').prop('checked')
      && ($('#include-notes').length == 0 || $('#include-notes').prop('checked'))
      && ($('#include-product-versions').length == 0 || $('#include-product-versions').prop('checked'))
      && ($('#include-tasks').length == 0 || $('#include-tasks').prop('checked'))
      && ($('#include-water-cooler').length == 0 || $('#include-water-cooler').prop('checked'))
    ) {
        $('#include-all').prop('checked', true)
    }
    else {
        $('#include-all').prop('checked', false)
    }

    var includeTickets = $('#include-tickets').prop('checked');
    var includeKnowledgeBase = $('#include-knowledge-base').prop('checked');
    var includeWikis = $('#include-wikis').prop('checked');

    var includeNotes = false;
    var includeProductVersions = false;
    var includeTasks = false;
    var includeWaterCooler = false;

    if ($('#include-notes').length > 0) {
        includeNotes = $('#include-notes').prop('checked');
    }

    if ($('#include-product-versions').length > 0) {
        includeProductVersions = $('#include-product-versions').prop('checked');
    }

    if ($('#include-tasks').length > 0) {
        includeTasks = $('#include-tasks').prop('checked');
    }

    if ($('#include-water-cooler').length > 0) {
        includeWaterCooler = $('#include-water-cooler').prop('checked');
    }

    if (_standardFilterID != null) {
        parent.Ts.Services.Search.UpdateStandardFilters(
          _standardFilterID,
          includeTickets,
          includeKnowledgeBase,
          includeWikis,
          includeNotes,
          includeProductVersions,
          includeTasks,
          includeWaterCooler
        );
    }
    else {
        parent.Ts.Services.Search.AddStandardFilters(
          includeTickets,
          includeKnowledgeBase,
          includeWikis,
          includeNotes,
          includeProductVersions,
          includeTasks,
          includeWaterCooler,
          function (result) {
              _standardFilterID = result;
          }
        );
    }

    setTimeout(function () { $('#search-button').trigger('click'); }, 5);

}
