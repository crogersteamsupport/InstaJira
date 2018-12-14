/// <reference path="ts/ts.js" />
/// <reference path="ts/parent.parent.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

var adminAuto = null;
$(document).ready(function () {
  adminAuto = new AdminAuto();
  adminAuto.refresh();
});

function onShow() {
  adminAuto.refresh();
};


AdminAuto = function () {
  $('head').append(parent.parent.Ts.MainPage.getCalcStyle());
  layout = $('.admin-auto-layout').layout({
    resizeNestedLayout: true,
    defaults: { spacing_open: 5, closable: false },
    center: { paneSelector: ".admin-auto-center" },
    north: { paneSelector: ".admin-auto-north", size: 31, spacing_open: 0, resizable: false },
    //south: { paneSelector: ".admin-auto-south", size: 50, spacing_open: 0, resizable: false },
    west: { paneSelector: ".admin-auto-west", size: 175, resizable: true }
  });


  var _operators = ['<', '<=', '=', '<>', '>=', '>'];
  var _data = null;
  var _triggerID = null;
  var _isModified = false;
  var _editorCnt = 0;

  $('button').button();
  $('select').combobox({ selected: function (e, ui) { isModified(true); } });

  $('input.text').addClass('ui-widget-content ui-corner-all');
  $('#textName').keydown(function () { isModified(true); });

  function isModified(value) {
    if (value == true) {
      $('.triggers').hide();
      $('.admin-auto-save').show();
      _isModified = true;
    }
    else {
      $('.triggers').show();
      $('.admin-auto-save').hide();
      _isModified = false;
    }
  }

  $('.trigger-save').click(function (e) {
    e.preventDefault();

    var data = new Object();

    data.TriggerID = _triggerID;
    data.Name = $('#textName').val();
    data.IsActive = $('#cmbEnabled').val() == 1;
    data.Actions = getActions();
    data.LogicItems = getLogic();

    parent.parent.Ts.Services.Automation.SaveTrigger(JSON.stringify(data), function (trigger) {
      isModified(false);
      if (trigger && _triggerID < 0) {
        appendTrigger(trigger);
        selectTrigger(trigger.TriggerID);
      }
      else {
        $('.triggers li.trigger-selected').click();
      }
      parent.parent.Ts.System.logAction('Admin Automation - Trigger Saved');
    });
  });

  $('.trigger-cancel').click(function (e) {
    e.preventDefault();
    isModified(false);
    _triggerID = getSelectedTriggerID();
    loadTrigger();
  });

  function hideNoTrigger() {
    if (_triggerID == null) {
      $('.admin-auto-trigger').hide();
      $('.admin-auto-no-trigger').show();
      return;
    }
    $('.admin-auto-trigger').show();
    $('.admin-auto-no-trigger').hide();
  }

  function showNoTrigger() {
    $('.admin-auto-trigger').hide();
    $('.admin-auto-no-trigger').show();
  }
  function addToolbarButton(id, icon, caption, callback) {
      var html = '<i href="#" id="' + id + '" class="automation-button ui-corner-all fa ' + icon + '"><span style="margin-left:6px;">' + caption + '</span></i>';
    $('.admin-auto-north').append(html).find('#' + id).click(callback).hover(function () { $(this).addClass('ui-state-hover'); }, function () { $(this).removeClass('ui-state-hover'); });
  }

  addToolbarButton('btnNew', 'fa-plus', 'New Trigger', newTrigger);
  $('.trigger-new').click(newTrigger);

  function newTrigger(e) {
    if (e) e.preventDefault();
    isModified(true);
    _triggerID = -1;
    hideNoTrigger();
    clearTrigger();
    parent.parent.Ts.System.logAction('Admin Automation - Started New Trigger');
  }

  addToolbarButton('btnDuplicate', 'fa-files-o', 'Duplicate Trigger', duplicateTrigger);
  $('.trigger-duplicate').click(duplicateTrigger);

  function duplicateTrigger(e) {
  	if (e) e.preventDefault();
  	isModified(true);
  	_triggerID = -1;
  	hideNoTrigger();

  	$('#executionsCountLabel').html('');
  	$('#lastModifiedLabel').html('');
  	$('#cmbEnable').val(0);
  	parent.parent.Ts.System.logAction('Admin Automation - Trigger Duplicated');
  }

  addToolbarButton('btnDelete', 'fa-trash', 'Delete Trigger', function (e) {
    e.preventDefault();
    e.stopPropagation();
    if (!confirm('Are you sure you would like to delete this trigger?')) return;
    parent.parent.Ts.Services.Automation.DeleteTrigger(getSelectedTriggerID(), function () {
      var element = $('li.trigger-' + getSelectedTriggerID());
      var prev = element.prev();
      element.remove();

      if (prev.length > 0) {
        prev.click();
      }
      else {
        selectFirstTrigger();
      }
      _triggerID = getSelectedTriggerID();
      if (_triggerID == null) showNoTrigger();
      parent.parent.Ts.System.logAction('Admin Automation - Trigger Deleted');
    });

  });

  addToolbarButton('btnRefresh', 'fa-refresh', 'Refresh', function (e) {
    e.preventDefault();
    e.stopPropagation();
    window.location = window.location;
  });

  function initEditor(id, onEditorInit) {
    $('#' + id).tinymce({
      plugins: "autoresize link code",
      toolbar1: "bold italic underline strikethrough bullist numlist fontselect fontsizeselect forecolor backcolor | link unlink | code",
      statusbar: false,
      gecko_spellcheck: true,
      convert_urls: true,
      relative_urls: false,
      branding: false,
      content_css: "../Css/jquery-ui-latest.custom.css",
      body_class: "ui-widget",
      template_external_list_url: "tinymce/jscripts/template_list.js",
      external_link_list_url: "tinymce/jscripts/link_list.js",
      external_image_list_url: "tinymce/jscripts/image_list.js",
      media_external_list_url: "tinymce/jscripts/media_list.js",
      menubar: false,
      setup: function (ed) { ed.on('change', function (ed, l) { isModified(true); }); },
      oninit: onEditorInit
    });
    /*
    var editor = new tinymce.Editor(id,
    {
    //http://tinymce.moxiecode.com/examples/full.php
    //http://wiki.moxiecode.com/index.php/TinyMCE:Configuration#Advanced_theme
    //mode: "specific_textareas",
    //editor_selector: 'mce-editor',
    theme: "advanced",
    skin: "o2k7",
    plugins: "autoresize",
    theme_advanced_buttons1: "bold,italic,underline,strikethrough,bullist,numlist,fontselect,fontsizeselect,forecolor,backcolor,|,link,unlink,|,code",
    theme_advanced_buttons2: "",
    theme_advanced_buttons3: "",
    theme_advanced_buttons4: "",
    theme_advanced_toolbar_location: "top",
    theme_advanced_toolbar_align: "left",
    theme_advanced_statusbar_location: "none",
    theme_advanced_resizing: true,
    convert_urls: true,
    relative_urls: false,
    content_css: "../Css/jquery-ui-latest.custom.css",
    body_class: "ui-widget",
    template_external_list_url: "tinymce/jscripts/template_list.js",
    external_link_list_url: "tinymce/jscripts/link_list.js",
    external_image_list_url: "tinymce/jscripts/image_list.js",
    media_external_list_url: "tinymce/jscripts/media_list.js",
    setup: function (ed) { ed.onChange.add(function (ed, l) { isModified(true); }); }
    });*/
    //editor.render();
  }

  parent.parent.Ts.Services.Automation.GetData(function (data) {
    _data = data;
    loadTriggers();
  });

  function loadComboActions(select) {
    select.html('');
    for (var i = 0; i < _data.Actions.length; i++) {
      select.append('<option value="' + _data.Actions[i].ActionID + '">' + _data.Actions[i].DisplayName + '</option>');
    }
    select.find('option:first').attr('selected', 'selected');
    return select;
  }

  function loadComboStatuses(select) {
    select.html('');
    for (var i = 0; i < _data.Statuses.length; i++) {
      select.append('<option value="' + _data.Statuses[i].id + '">' + _data.Statuses[i].label + '</option>');
    }
    select.find('option:first').attr('selected', 'selected');
    return select;
  }

  function loadComboSeverities(select) {
    select.html('');
    for (var i = 0; i < _data.Severities.length; i++) {
      select.append('<option value="' + _data.Severities[i].id + '">' + _data.Severities[i].label + '</option>');
    }
    select.find('option:first').attr('selected', 'selected');
    return select;
  }

  function loadComboTicketTypes(select) {
    select.html('');
    for (var i = 0; i < _data.TicketTypes.length; i++) {
      select.append('<option value="' + _data.TicketTypes[i].id + '">' + _data.TicketTypes[i].label + '</option>');
    }
    select.find('option:first').attr('selected', 'selected');
    return select;
  }

  function loadComboUsers(select) {
    select.html('');
    $('<option>').attr('value', '-1').text('Assigned User').appendTo(select).attr('selected', 'selected');
    $('<option>').attr('value', '-2').text('Organization\'s Primary Contact').appendTo(select);
    for (var i = 0; i < _data.Users.length; i++) {
      select.append('<option value="' + _data.Users[i].id + '">' + _data.Users[i].label + '</option>');
    }
    return select;
  }

  function loadComboProducts(select) {
    select.empty();
    var products = parent.parent.Ts.Cache.getProducts();
    for (var i = 0; i < products.length; i++) {
      select.append('<option value="' + products[i].ProductID + '">' + products[i].Name + '</option>');
    }
    select.find('option:first').attr('selected', 'selected');
    return select;
  }

  function loadComboGroups(select) {
    select.html('');
    for (var i = 0; i < _data.Groups.length; i++) {
      select.append('<option value="' + _data.Groups[i].id + '">' + _data.Groups[i].label + '</option>');
    }
    select.find('option:first').attr('selected', 'selected');
    return select;
  }

  function loadComboFields(select) {
    select.html('');
    $('<option>').attr('value', '-1').text('-- Select a Field --').appendTo(select).attr('selected', 'selected');
    for (var i = 0; i < _data.Fields.length; i++) {
        var val = _data.Fields[i].IsCustom ? 'C-' + _data.Fields[i].FieldID : _data.Fields[i].FieldID;
      $('<option>').attr('value', val).text(_data.Fields[i].Alias).appendTo(select).data('field', _data.Fields[i]);
    }
    return select;
  }

  function loadComboMeasure(select) {
    select.html('');
    $('<option>').attr('value', '<').text('Less Than').appendTo(select);
    $('<option>').attr('value', '<=').text('Less Than or Equal To').appendTo(select);
    $('<option>').attr('value', '=').attr('selected', 'selected').text('Equal To').appendTo(select);
    $('<option>').attr('value', '<>').text('Not Equal To').appendTo(select);
    $('<option>').attr('value', '>=').text('Greater Than or Equal To').appendTo(select);
    $('<option>').attr('value', '>').text('Greater Than').appendTo(select);
    $('<option>').attr('value', 'contains').text('Contains').appendTo(select);
    $('<option>').attr('value', 'does not contain').text('Does Not Contain').appendTo(select);
    return select;
  }

  function loadComboCustomFields(select) {
      select.html('');
      //$('<option>').attr('value', '-1').text('-- Select a Field --').appendTo(select).attr('selected', 'selected');
      for (var i = 0; i < _data.CustomFields.length; i++) {
          //$('<option>').attr('value', 'C-' + _data.CustomFields[i].FieldID).text(_data.CustomFields[i].Alias).appendTo(select).data('field', _data.CustomFields[i]);
          $('<option>').attr('value', _data.CustomFields[i].FieldID).text(_data.CustomFields[i].Alias).appendTo(select).data('field', _data.CustomFields[i]);
      }
      return select;
  }

  function loadTriggers(triggerID) {
    parent.parent.Ts.Services.Automation.GetTriggers(function (result) {
      if (result.length > 0) {
        var list = $('.triggers ul').html('');

        for (var i = 0; i < result.length; i++) {
          appendTrigger(result[i], list);
        }

        if (triggerID) {
          selectTrigger(triggerID);
        }
        else {
          selectFirstTrigger();
        }
      }
      else {
        $('.ts-loading').hide();
        showNoTrigger();
      }
    });
  }

  function appendTrigger(trigger, list) {
    if (!list) list = $('.triggers ul');
    var item = $('<li>')
            .addClass('trigger-' + trigger.TriggerID)
            .text(trigger.Name)
            .addClass((trigger.Active) ? '' : 'inactive')
            .disableSelection()
            .hover(function () { $(this).addClass('ui-state-hover'); }, function () { $(this).parent().find('.ui-state-hover').removeClass('ui-state-hover'); })
            .click(function () {
              $('.triggers li').removeClass('ui-state-active trigger-selected');
              $(this).addClass('ui-state-active trigger-selected');
              _triggerID = getSelectedTriggerID();
              loadTrigger();
            })
            .appendTo(list);
    return item;
  }

  function clearTrigger() {
    $('#textName').val('');
    $('#executionsCountLabel').html('');
    $('#lastModifiedLabel').html('');
    $('#cmbEnable').val(0);
    $('.conditions-any').html('');
    $('.conditions-all').html('');
    $('.actions').html('');

  }

  function loadTrigger() {
    clearTrigger();
    _triggerID = getSelectedTriggerID();
    if (_triggerID == null) {
      showNoTrigger();
    }

    parent.parent.Ts.Services.Automation.GetTrigger(_triggerID, function (result) {
      $('#textName').val(result.Trigger.Name);
      $('#executionsCountLabel').html('This automation has modified ' + result.Trigger.ExecutionsCount + ' tickets.');
      $('#lastModifiedLabel').html('Last modified on ' + result.Trigger.DateModified);
      $('#cmbEnabled').combobox("setValue", result.Trigger.Active === true ? "1" : "0");

      for (var i = 0; i < result.LogicItems.length; i++) {
        var selector = result.LogicItems[i].MatchAll === true ? '.conditions-all' : '.conditions-any';
          //addCondition(selector, result.LogicItems[i].FieldID, result.LogicItems[i].Measure, result.LogicItems[i].TestValue);
        addCondition(selector, result.LogicItems[i]);
      }

      for (var i = 0; i < result.Actions.length; i++) {
        addAction('.actions', result.Actions[i].ActionID, result.Actions[i].ActionValue, result.Actions[i].ActionValue2);
      }

      // load logic and actions

      hideNoTrigger();
      $('.ts-loading').hide();
      $('.trigger-' + _triggerID).addClass((result.Trigger.Active) ? '' : 'inactive');
      $('.trigger-' + _triggerID).removeClass((result.Trigger.Active) ? 'inactive' : '');
    });
  }

  $('#btnAddAllCondition').click(function (e) {
    e.preventDefault();
    addCondition('.conditions-all');
    parent.parent.Ts.System.logAction('Admin Automation - All Condition Added');
  });

  $('#btnAddAnyCondition').click(function (e) {
    e.preventDefault();
    addCondition('.conditions-any');
    parent.parent.Ts.System.logAction('Admin Automation - Any Condition Added');
  });

    //addCondition(selector, result.LogicItems[i].FieldID, result.LogicItems[i].Measure, result.LogicItems[i].TestValue);
  //function addCondition(selector, fieldID, measure, value) {

  function addCondition(selector, logicItem) {
    var div = $('<div>').addClass('condition ts-section')
    var fields = $('<select>').addClass('condition-field').appendTo(div).width('150px');
    loadComboFields(fields).combobox({ selected: function (e, ui) {
      isModified(true);
      createConditionValue($(this).parents('.condition'), $(ui.item).data('field'));
    }
    });


    if (logicItem) {
        var id = logicItem.TableID == -1 ? 'C-' + logicItem.FieldID : logicItem.FieldID;

        if (fields.find('option[value="' + id + '"]').length > 0) {
            fields.combobox('setValue', id);
        }
        else {
            fields.combobox('setValue', -999);
        }
    }

    var measures = $('<select>').addClass('condition-measure').appendTo(div).width('125px');
    loadComboMeasure(measures).combobox({ selected: function (e, ui) { isModified(true); } });
    if (logicItem) { measures.combobox('setValue', logicItem.Measure); }
    $('<span>').addClass('condition-value-container').appendTo(div);
    var value = logicItem ? logicItem.TestValue : null;
    createConditionValue(div, fields.find('option:selected').data('field'), value);

    if (logicItem && fields.find('option[value="' + logicItem.FieldID + '"]').length == 0) {
        div.find('.condition-value-container').parent().find('.condition-custom-value').combobox('setValue', logicItem.FieldID)
    }
    

    $('<span>').addClass('ts-icon ts-icon-remove').appendTo(div).click(function (e) {
      $(this).parent().remove(); isModified(true); parent.parent.Ts.System.logAction('Admin Automation - Condition Removed');
    });
    $('<div>').css('clear', 'both').appendTo(div);
    div.appendTo(selector);

  }

  function createConditionValue(condition, field, value) {
      var container = condition.find('.condition-value-container').empty();
      condition.find('.condition-custom-value').remove();

    if (!field) return;

    var execGetFieldValues = null;
    function getFieldValues(request, response) {
      if (execGetFieldValues) { execGetFieldValues._executor.abort(); }
      execGetFieldValues = parent.parent.Ts.Services.System.GetLookupDisplayNames(condition.find('.condition-field').val(), request.term, function (result) { response(result); $(this).removeClass('ui-autocomplete-loading'); });
    }

    if (field.FieldID == -999)
    {
        var select = $('<select>')
          .addClass('condition-custom-value')
          .insertBefore(container.prev());

        for (var i = 0; i < field.ListValues.length; i++) {
            var customField = field.ListValues[i].split(':');
            $('<option>').attr('value', customField[1]).text(customField[0]).appendTo(select);
        }
        select.combobox({ selected: function (e, ui) { isModified(true); } })

    }

    if (field.DataType == 'bit') {
      var select = $('<select>')
        .addClass('condition-value')
        .width('125px')
        .appendTo(container);
      $('<option>').attr('value', 'true').text('True').appendTo(select);
      $('<option>').attr('value', 'false').text('False').appendTo(select);

      if (value && value == 'false') select.find('option:last').attr('selected', 'selected'); else select.find('option:first').attr('selected', 'selected');
      select.combobox({ selected: function (e, ui) { isModified(true); } })
    }
    else if (field.DataType == 'list') {
      var select = $('<select>')
        .addClass('condition-value')
        .appendTo(container);
      for (var i = 0; i < field.ListValues.length; i++) {
        $('<option>').attr('value', field.ListValues[i]).text(field.ListValues[i]).appendTo(select);
      }

      if (value) select.find('option[value="' + value + '"]').attr('selected', 'selected'); else select.find('option:first').attr('selected', 'selected');
      select.combobox({ selected: function (e, ui) { isModified(true); } })
    }
    else {
      var input = $('<input>')
        .addClass('ui-corner-all ui-widget-content condition-value form-control')
        .attr('type', 'text')
        .width('200px')
        .keydown(function () { isModified(true); })
        .appendTo(container)
        .val((value ? value : ""));
      if (field.LookupTableID != null) {
        input.autocomplete({ minLength: 2, source: getFieldValues, select: function (event, ui) { } });
      }
      else if (field.DataType == 'datetime') {
        input.datetimepicker().change(function () { isModified(true); });
      }
    }
  }

  function getLogic() {
    var items = new parent.parent.Array();
    function getItems(selector, isAny, items) {
        $(selector).find('.condition').each(function () {
        var data = $(this).find('.condition-field option:selected').data('field');
        proxy = new parent.parent.TeamSupport.Data.TicketAutomationTriggerLogicItemProxy();

        proxy.TriggerID = -1;
        proxy.TableID = data.TableID;
        if(data.FieldID != "-999")
            proxy.FieldID = data.FieldID;
        else
            proxy.FieldID = $(this).find('.condition-custom-value').val();
        proxy.IsCustom = data.IsCustom;
        proxy.Measure = $(this).find('.condition-measure').val();
        proxy.TestValue = $(this).find('.condition-value').val();
        proxy.MatchAll = isAny;
        proxy.OtherTrigger = data.OtherTrigger;
        items[items.length] = proxy;
      });
    }
    getItems('.conditions-all', true, items);
    getItems('.conditions-any', false, items);
    return items;
  }

  $('#btnAddAction').click(function (e) {
    e.preventDefault();
    addAction('.actions');
    parent.parent.Ts.System.logAction('Admin Automation - Action Added');

  });

  function addAction(selector, actionID, value1, value2) {
    var div = $('<div>').addClass('action ts-section');
    var main = $('<div>')
    .addClass('action-main ui-helper-clearfix')
    .appendTo(div);

    var actions = $('<select>')
      .width('250px')
      .addClass('action-type')
      .appendTo(main)
      .combobox({ selected: function (e, ui) {
        setActionValues(div, getAction(ui.item.value));
        isModified(true);
      }
      });


    var values = $('<select>')
      .addClass('action-value')
      .width('250px')
      .appendTo(main)
      .combobox({ selected: function () { isModified(true); } });

    $('<span>').addClass('ts-icon ts-icon-remove').appendTo(main).click(function (e) {
      $(this).parents('.action').remove(); isModified(true); parent.parent.Ts.System.logAction('Admin Automation - Action Removed');
    });

    var actionEditor = $('<div>')
    .addClass('action-editor ui-helper-hidden')
    .appendTo(div);

    var editorID = 'action-editor-' + _editorCnt++;
    var editor = $('<textarea>')
    .width('100%')
    .height('50px')
    .attr('id', editorID)
    .appendTo(actionEditor);

    div.appendTo(selector);
    loadComboActions(actions).combobox('update');
    function onEditorInit(ed) {
      if (actionID) actions.combobox('setValue', actionID);
      var selectedAction = getAction(actions.val());
      setActionValues(div, selectedAction, value1, value2);
      if (value2) { ed.setContent(value2); }
      if (value1 && values.data('combobox')) {
        values.combobox('setValue', value1);
      } else {
        values.val(value1);
      }
    }
    initEditor(editorID, onEditorInit);

  }

  function getActions() {
    var items = new parent.parent.Array();
    $('.actions .action').each(function (index) {
      proxy = new parent.parent.TeamSupport.Data.TicketAutomationActionProxy();
      proxy.TriggerID = -1;
      proxy.ActionID = $(this).find('.action-type').val();
      var action = getAction(proxy.ActionID);
      var actionValue = $(this).find('.action-value');
      proxy.ActionValue = action.ValueList ? (actionValue.data('itemID') ? actionValue.data('itemID') : actionValue.val()) : null;

      var actionValue2 = null;
      if (action.ValueList2 && action.ValueList2.toLowerCase() === "textbox") {
          actionValue2 = $(this).find('.action-value2').val();
      }
      proxy.ActionValue2 = action.ValueList2 && action.ValueList2.toLowerCase() === "text" ? getEditor($(this).find('.action-editor')).getContent() : actionValue2;
      items[items.length] = proxy;
    });


    return items;
  }

  function getEditor(element) {
    //return tinymce.get(element.find('textarea').attr('id'));
    return element.find('textarea').tinymce();
  }

  function setActionValues(element, action, value1, value2) {
    var select = element.find('.action-value');
    var actionEditor = element.find('.action-editor');
    if (action.ValueList2 && action.ValueList2.toLowerCase() === "text") {
      //select.next().hide();
      actionEditor.show();
      getEditor(actionEditor).focus();
    }
    else {
      actionEditor.hide();
    }

    if (action.ValueList) {
      select.next().show();
    } else {
      select.next().hide();
    }

    element.find('.action-value2').remove();

    switch (action.ValueList) {
      case 'TicketStatuses':
        loadComboStatuses(select).combobox('update');
        break;
      case 'TicketSeverities':
        loadComboSeverities(select).combobox('update');
        break;
      case 'TicketType':
        loadComboTicketTypes(select).combobox('update');
        break;
      case 'Users':
        loadComboUsers(select).combobox('update');
        break;
      case 'Products':
        loadComboProducts(select).combobox('update');
        break;
      case 'Groups':
        loadComboGroups(select).combobox('update');
        break;
      case 'CustomerList':
      case 'ContactList':
      case 'TextBox':
        var input = $('<input type="text">')
          .addClass('action-value ui-widget-content ui-corner-all form-control')
          .width('250px')
          .insertAfter(select);
        select.remove();
        if (action.ValueList == 'CustomerList') {
            input.autocomplete({
                minLength: 2,
                source: getCompanies,
                select: function (event, ui) {
                    $(this).data('itemID', ui.item.id);
                }

            });

            parent.parent.Ts.Services.Organizations.GetOrganization(value1, function (result) {
                if (result) {
                    input.val(result.Name);
                    input.data('itemID', value1);
                }
            });
        }
        else if (action.ValueList == 'ContactList') {
            input.autocomplete({
                minLength: 2,
                source: getContacts,
                select: function (event, ui) {
                    $(this).data('itemID', ui.item.id);
                }

            });

            parent.parent.Ts.Services.Organizations.GetUser(value1, function (result) {
                if (result) {
                    input.val(result.LastName + ', ' + result.FirstName);
                    input.data('itemID', value1);
                }
            });

        }
        else {
            input.val((value1 ? value1 : ''))
        }

        break;
      case 'CustomFieldList':
        loadComboCustomFields(select).combobox('update');
        var input = $('<input type="text">')
          .addClass('action-value2 ui-widget-content ui-corner-all form-control')
          .width('250px')
          .val(value2)
          .insertAfter(select.next());
        break;
      default:
    }
  }

  var execGetCompany = null;
  function getCompanies(request, response) {
      if (execGetCompany) { execGetCompany._executor.abort(); }
      execGetCompany = parent.parent.Ts.Services.Organizations.GetCompanies(request.term, function (result) { response(result); });
      isModified(true);
  }

  var execGetContact = null;
  function getContacts(request, response) {
      if (execGetContact) { execGetContact._executor.abort(); }
      execGetContact = parent.parent.Ts.Services.Organizations.GetContacts(request.term, function (result) { response(result); });
      isModified(true);
  }


  function getAction(actionID) {
    for (var i = 0; i < _data.Actions.length; i++) {
      if (_data.Actions[i].ActionID == actionID) return _data.Actions[i];
    }
    return null;
  }

  function getSelectedTriggerID() {
    return Ts.Utils.getIdFromElement('trigger', $('.triggers .trigger-selected'));
  }

  function selectTrigger(triggerID) {
    return $('.trigger-' + triggerID).click();
  }

  function selectFirstTrigger() {
    return $('.triggers li:first').click();
  }

};


AdminAuto.prototype = {
  constructor: AdminAuto,
  refresh: function () {
  }
};
