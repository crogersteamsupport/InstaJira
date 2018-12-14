/// <reference path="ts/ts.services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="~/Default.aspx" />
/// <reference path="ts/ts.pages.main.js" />

$(document).ready(function () {
    var _lastTicketTypeID = null;
    var _timerid;
    var _timerElapsed = 0;
    var speed = 50, counter = 0, start;
  var _ticketID = null;
  var _doClose = false;
  var canEdit = top.Ts.System.User.IsSystemAdmin || top.Ts.System.User.ChangeKbVisibility;
  var alertMessage = null;
  var dateFormat;
  top.Ts.System.logAction('New Ticket - Started');
  var session;
  var token;
  var recordingID;
  var apiKey;
  var sessionId;
  var tokurl;
  var publisher;

  apiKey = "45228242";

  $('#knowledgeBaseCategoryDiv').hide();

  if (!canEdit) {
    //$('.newticket-kb').attr("disabled", true);
    $('.new-ticket-div').hide();
  }
  else {
    $('.newticket-kb').click(function (e) {
      if ($('.newticket-kb').prop('checked')) {
        $('#knowledgeBaseCategoryDiv').show();
      }
      else {
        $('#knowledgeBaseCategoryDiv').hide();
      }
    });
  }

  if (top.Ts.System.Organization.SetNewActionsVisibleToCustomers == true) {
      $('.newticket-portal').prop('checked', true);
  }

  var knowledgeBaseCategories = top.Ts.Cache.getKnowledgeBaseCategories();
  var option = $('<option>').text('Unassigned').attr('value', -1).appendTo('.newticket-kbCategory').data('o', null).attr('selected', 'selected');
  for (var i = 0; i < knowledgeBaseCategories.length; i++) {
    var cat = knowledgeBaseCategories[i].Category;
    option = $('<option>').text(cat.CategoryName).attr('value', cat.CategoryID).appendTo('.newticket-kbCategory').data('o', cat);
    if ($(this).text() === cat.CategoryName) { option.attr('selected', 'selected'); }

    for (var j = 0; j < knowledgeBaseCategories[i].Subcategories.length; j++) {
      var sub = knowledgeBaseCategories[i].Subcategories[j];
      option = $('<option>').text(cat.CategoryName + ' -> ' + sub.CategoryName).attr('value', sub.CategoryID).appendTo('.newticket-kbCategory').data('o', sub);
    }
  }

  top.Ts.Services.Customers.GetDateFormat(true, function (format) {
      dateFormat = format.replace("yyyy", "yy");
      $('.ticket-action-form-dueDate').datetimepicker({ dateFormat: dateFormat });
  });

  $('.newticket-kbCategory').combobox();

  $("#recordVideoContainer").hide();

  $('.ticket-content-header h1').click(function (e) {
    e.preventDefault();
    location = location;
  });

  $('button').button();

  if (top.Ts.System.Organization.IsInventoryEnabled != true) $('.ticket-widget-assets').hide();

  if (!top.Ts.System.User.ChangeTicketVisibility)
  {
      $('.community-block').hide();
  }

  $('.ticket-action-form-date').datetimepicker().datetimepicker('setDate', new Date());
  $('.ticket-action-form-hours').spinner({ min: 0 }).val(0);
  $('.ticket-action-form-minutes').spinner({ min: 0 }).val(0);

  var types = top.Ts.Cache.getTicketTypes();
  for (var i = 0; i < types.length; i++) {
    $('<option>').attr('value', types[i].TicketTypeID).text(types[i].Name).data('o', types[i]).appendTo('.newticket-type');
  }
  _lastTicketTypeID = $('.newticket-type option:selected').data('o').TicketTypeID;

  $('.newticket-status').combobox();

  $('.newticket-type').combobox({
    selected: function (e, ui) {
      setTicketStatus();
      showCustomFields();
      copyCustomFieldValues();
      _lastTicketTypeID = $('.newticket-type option:selected').data('o').TicketTypeID;
      setTicketTypeTemplateText();
    }
  });

  var tickettimer = function () {
      var element = $('.ticket-action-form');

      //work out the real and ideal elapsed time
      var real = (counter * speed),
          ideal = (new Date().getTime() - start);

      counter++;
      //element.find('.timer').text(Math.floor(ideal / 60000));

      var diff = (ideal - real);

      if (_timerElapsed != Math.floor(ideal / 60000)) {
          var oldVal = parseInt($('.ticket-action-form-minutes').val()) || 0;
          $('.ticket-action-form-minutes').val(oldVal + 1);
          _timerElapsed = Math.floor(ideal / 60000);
      }
      _timerid = setTimeout(tickettimer, (speed - diff));
  }

  function setupProductType() {
    /*
    Ts.ProductType.Express = 0;
    Ts.ProductType.HelpDesk = 1;
    Ts.ProductType.Enterprise = 2;
    Ts.ProductType.BugTracking= 3;
    */

    if (top.Ts.System.Organization.ProductType == top.Ts.ProductType.Express) {
      $('.no-express').hide();
    }

    if (top.Ts.System.Organization.ProductType == top.Ts.ProductType.HelpDesk) {
      $('.no-helpdesk').hide();
    }

    if (top.Ts.System.Organization.UseForums != true) {
      $('.newticket-community').parent().hide();
    }
  }
  setupProductType();

  function copyCustomFieldValues() {
    var ticketTypeID = $('.newticket-type option:selected').data('o').TicketTypeID;
    $('.newticket-custom-field').each(function () {
      var field1 = $(this);
      if (_lastTicketTypeID == field1.data('o').AuxID) {
        $('.newticket-custom-field').each(function () {
          var field2 = $(this);

          if (field1.data('o').FieldType == field2.data('o').FieldType && field2.data('o').AuxID == ticketTypeID && field1.data('o').Name == field2.data('o').Name) {

            switch (field1.data('o').FieldType) {
              case top.Ts.CustomFieldType.Text:
                field2.find('input').val(field1.find('input').val());
                break;
              case top.Ts.CustomFieldType.Date:
              case top.Ts.CustomFieldType.Time:
              case top.Ts.CustomFieldType.DateTime:
                field2.find('input').val(field1.find('input').val());
                break;
              case top.Ts.CustomFieldType.Boolean:
                break;
              case top.Ts.CustomFieldType.Number:
                field2.find('input').val(field1.find('input').val());
                break;
              case top.Ts.CustomFieldType.PickList:
                field2.find('select').combobox('setValue', field1.find('select  option:selected').val());
                break;
              default:
            }


          }
        });
      }

    });
  }

  setTicketStatus();

  function setTicketTypeTemplateText() {
    var ticketTypeID = $('.newticket-type option:selected').data('o').TicketTypeID;
    top.Ts.Services.Tickets.GetTicketTypeTemplateText(ticketTypeID, function (result) {
      if (result != null && result != "" && result != "<br>") {
          var currenttext = $('.newticket-desc').html();
          tinyMCE.activeEditor.selection.setContent(currenttext + result);
        //$('.newticket-desc').html(currenttext + result);
      }
    });
  }





  function setTicketStatus() {
    var statuses = top.Ts.Cache.getTicketStatuses();
    var ticketTypeID = $('.newticket-type option:selected').data('o').TicketTypeID;
    $('.newticket-status').empty();
    var flag = false;
    for (var i = 0; i < statuses.length; i++) {
      if (statuses[i].TicketTypeID == ticketTypeID) {
        $('<option>').attr('value', statuses[i].TicketStatusID).text(statuses[i].Name).data('o', statuses[i]).appendTo('.newticket-status');
        if (flag == false) {
          $('.newticket-status').combobox('setValue', statuses[i].TicketStatusID);
          flag = true;
        }

      }
    }
  }

  var severities = top.Ts.Cache.getTicketSeverities();
  for (var i = 0; i < severities.length; i++) {
    $('<option>').attr('value', severities[i].TicketSeverityID).text(severities[i].Name).data('o', severities[i]).appendTo('.newticket-severity');
  }
  $('.newticket-severity').combobox();

  var users = top.Ts.Cache.getUsers();
  for (var i = 0; i < users.length; i++) {
    var option = $('<option>').attr('value', users[i].UserID).text(users[i].Name).data('o', users[i]).appendTo('.newticket-user');
  }
  addUnassignedComboItem($('.newticket-user').combobox());
  $('.newticket-user').combobox('setValue', top.Ts.System.User.UserID);

  var groups = top.Ts.Cache.getGroups();
  for (var i = 0; i < groups.length; i++) {
    $('<option>').attr('value', groups[i].GroupID).text(groups[i].Name).data('o', groups[i]).appendTo('.newticket-group');
  }

  function checkIfUserExistsInArray(user, array) {
    var result = false;
    for (var i = 0; i < array.length; i++) {
      if (user.UserID == array[i].UserID) {
        result = true;
        break;
      }
    }
    return result;
  }

  addUnassignedComboItem($('.newticket-group').combobox({
    selected: function (e, ui) {
      if (top.Ts.System.Organization.ShowGroupMembersFirstInTicketAssignmentList) {
        var userID = $('.newticket-user').val();
        $('.newticket-user').empty();
        var groupID = $('.newticket-group').val();
        if (groupID != -1) {
          top.Ts.Services.Users.GetGroupUsers(groupID, function (ticketGroupUsers) {
            for (var i = 0; i < ticketGroupUsers.length; i++) {
              $('<option>').attr('value', ticketGroupUsers[i].UserID).text(ticketGroupUsers[i].Name).data('o', ticketGroupUsers[i]).appendTo('.newticket-user');
            }
            for (var i = 0; i < users.length; i++) {
              if (!checkIfUserExistsInArray(users[i], ticketGroupUsers)) {
                $('<option>').attr('value', users[i].UserID).text(users[i].Name).data('o', users[i]).appendTo('.newticket-user');
              }
            }
            addUnassignedComboItem($('.newticket-user').combobox());
            $('.newticket-user').combobox('setValue', userID);
          });
        }
        else {
          for (var i = 0; i < users.length; i++) {
            $('<option>').attr('value', users[i].UserID).text(users[i].Name).data('o', users[i]).appendTo('.newticket-user');
          }
          addUnassignedComboItem($('.newticket-user').combobox());
          $('.newticket-user').combobox('setValue', userID);
        }
      }
    }
  }));

  var categories = top.Ts.Cache.getForumCategories();
  var option = $('<option>').text('Unassigned').attr('value', -1).appendTo('.newticket-community').data('o', null).attr('selected', 'selected');
  for (var i = 0; i < categories.length; i++) {
    var cat = categories[i].Category;
    //option = $('<option>').text(cat.CategoryName).attr('value', cat.CategoryID).appendTo('.newticket-community').data('o', cat);
    for (var j = 0; j < categories[i].Subcategories.length; j++) {
      var sub = categories[i].Subcategories[j];
      option = $('<option>').text(cat.CategoryName + ' -> ' + sub.CategoryName).attr('value', sub.CategoryID).appendTo('.newticket-community').data('o', sub);
    }
  }

  $('.newticket-community').combobox();

  function loadProducts(doAll) {
    if (doAll != undefined && doAll == true) {
      var products = top.Ts.Cache.getProducts();
      $('.newticket-product option').remove();
      for (var i = 0; i < products.length; i++) {
        $('<option>').attr('value', products[i].ProductID).text(products[i].Name).data('o', products[i]).appendTo('.newticket-product');
      }
      $('<option>').attr('value', -1).text('Unassigned').data('o', null).prependTo('.newticket-product');
      return;
    }
    top.Ts.Settings.Organization.read('ShowOnlyCustomerProducts', false, function (showOnlyCustomers) {
      if (showOnlyCustomers == "True") {

        var organizationIDs = new Array();
        $('.ticket-customer-company').each(function () {
          organizationIDs[organizationIDs.length] = $(this).data('data').OrganizationID;
        });
        $('.ticket-customer-contact').each(function () {
          organizationIDs[organizationIDs.length] = $(this).data('data').OrganizationID;
        });
        if (organizationIDs.length < 1) {
          loadProducts(true);
          return;
        }
        top.Ts.Services.Tickets.GetCustomerProductIDs(top.JSON.stringify(organizationIDs), function (productIDs) {

          if (!productIDs || productIDs == null || productIDs.length < 1) {
            loadProducts(true);
          }
          else {
            var products = top.Ts.Cache.getProducts();
            $('.newticket-product option').remove();
            for (var i = 0; i < products.length; i++) {
              for (var j = 0; j < productIDs.length; j++) {
                if (productIDs[j] == products[i].ProductID) {
                  var option = $('<option>').attr('value', products[i].ProductID).text(products[i].Name).appendTo('.newticket-product').data('o', products[i]);
                }
              }
            }

            $('<option>').attr('value', -1).text('Unassigned').data('o', null).prependTo('.newticket-product');
          }
        });
      }
    });
  }


  loadProducts(true);
  $('.newticket-product').combobox({
      selected: function (e, ui) {
          loadVersions();
          $('.productChild').remove();
          var productID = -1;
          if ($('.newticket-product option:selected').data('o')) {
              productID = $('.newticket-product option:selected').data('o').ProductID;
              top.Ts.Services.CustomFields.GetProductMatchingCustomFields(top.Ts.ReferenceTypes.Tickets, _lastTicketTypeID, productID, function (result) {
                  var container = $('.custom-fields-container');
                  for (var i = 0; i < result.length; i++) {
                      if (!result[i].CustomFieldCategoryID) {
                          try {
                              switch (result[i].FieldType) {
                                  case top.Ts.CustomFieldType.Text: appendCustomEdit(result[i], container, null, 'productChild'); break;
                                  case top.Ts.CustomFieldType.Date: appendCustomEditDate(result[i], container, null, 'productChild'); break;
                                  case top.Ts.CustomFieldType.Time: appendCustomEditTime(result[i], container, null, 'productChild'); break;
                                  case top.Ts.CustomFieldType.DateTime: appendCustomEditDateTime(result[i], container, null, 'productChild'); break;
                                  case top.Ts.CustomFieldType.Boolean: appendCustomEditBool(result[i], container, null, 'productChild'); break;
                                  case top.Ts.CustomFieldType.Number: appendCustomEditNumber(result[i], container, null, 'productChild'); break;
                                  case top.Ts.CustomFieldType.PickList: appendCustomEditCombo(result[i], container, null, 'productChild'); break;
                                  default:
                              }
                          } catch (err) {
                              var errorString = '1001 NewTicket.js createCustomFields   FieldType: ' + result[i].FieldType + '  CustomFieldID: ' + result[i].CustomFieldID + ' ::  Exception Properties: ';
                              for (var property in err) { errorString += property + ': ' + err[property] + '; '; }
                              top.Ts.Services.System.LogException(err.message, errorString);
                              $('.newticket-custom-field').remove();
                              return;
                          }
                      }
                  }
                  container.find('.ui-helper-clearfix').remove();
                  $('<div>').addClass('ui-helper-clearfix').appendTo(container);
                  appendCategorizedCustomFields(result, 'productChild');
              });
          }
      }
  }).combobox('setValue', -1);


  $('.newticket-resolved').combobox();
  $('.newticket-reported').combobox();

  var appendCategorizedCustomFields = function (fields, className) {
      top.Ts.Services.CustomFields.GetAllTypesCategories(top.Ts.ReferenceTypes.Tickets, function (categories) {
          for (var j = 0; j < categories.length; j++) {
              var isFirstFieldAdded = true;
              for (var i = 0; i < fields.length; i++) {
                  var item = null;

                  var field = fields[i];

                  if (field.CustomFieldCategoryID == categories[j].CustomFieldCategoryID) {
                      if ($('.category-' + categories[j].CustomFieldCategoryID).length == 0) {
                          isFirstFieldAdded = false;
                          var header = $('<h3>').text(categories[j].Category).addClass('category');
                          var innerContainer = $('<div>');
                          var container = $('<div>').addClass('ui-widget-content ui-corner-all ticket-content-section custom-fields category-' + categories[j].CustomFieldCategoryID);
                          container.append(header);
                          container.append(innerContainer);
                          container.insertBefore($('.new-ticket-save-buttons').parent());
                      }
                      
                      var container = $('.category-' + categories[j].CustomFieldCategoryID + ' > div');

                      switch (field.FieldType) {
                          case top.Ts.CustomFieldType.Text: appendCustomEdit(field, container, null, className); break;
                          case top.Ts.CustomFieldType.Date: appendCustomEditDate(field, container, null, className); break;
                          case top.Ts.CustomFieldType.Time: appendCustomEditTime(field, container, null, className); break;
                          case top.Ts.CustomFieldType.DateTime: appendCustomEditDateTime(field, container, null, className); break;
                          case top.Ts.CustomFieldType.Boolean: appendCustomEditBool(field, container, null, className); break;
                          case top.Ts.CustomFieldType.Number: appendCustomEditNumber(field, container, null, className); break;
                          case top.Ts.CustomFieldType.PickList: appendCustomEditCombo(field, container, null, className); break;
                          default:
                      }
                  }
              }
              $('.category-' + categories[j].CustomFieldCategoryID + ' > div').find('.ui-helper-clearfix').remove();
              $('<div>').addClass('ui-helper-clearfix').appendTo('.category-' + categories[j].CustomFieldCategoryID + ' > div');
          }
          showCustomFields();
      });
  }

  loadVersions();

  function loadVersions() {
    var product = $('.newticket-product option:selected').data('o');
    function loadVersionCombo(el) {
      el.empty();
      addUnassignedComboItem(el);
      if (product == null) return;
      for (var i = 0; i < product.Versions.length; i++) {
        $('<option>').attr('value', product.Versions[i].ProductVersionID).text(product.Versions[i].VersionNumber).data('o', product.Versions[i]).appendTo(el);
      }

    }
    loadVersionCombo($('.newticket-resolved'));
    loadVersionCombo($('.newticket-reported'));
  }

  setInitialValue();
  createCustomFields();
  function createCustomFields() {
    $('.newticket-custom-field').remove();
    top.Ts.Services.CustomFields.GetParentCustomFields(top.Ts.ReferenceTypes.Tickets, null, function (result) {
        if (result.length == 0) {
            $('.custom-fields').hide();
        }
        else {
            $('.custom-fields').show();
            var container = $('.custom-fields-container');
            for (var i = 0; i < result.length; i++) {
                if (!result[i].CustomFieldCategoryID) {
                    try {
                        switch (result[i].FieldType) {
                            case top.Ts.CustomFieldType.Text: appendCustomEdit(result[i], container); break;
                            case top.Ts.CustomFieldType.Date: appendCustomEditDate(result[i], container); break;
                            case top.Ts.CustomFieldType.Time: appendCustomEditTime(result[i], container); break;
                            case top.Ts.CustomFieldType.DateTime: appendCustomEditDateTime(result[i], container); break;
                            case top.Ts.CustomFieldType.Boolean: appendCustomEditBool(result[i], container); break;
                            case top.Ts.CustomFieldType.Number: appendCustomEditNumber(result[i], container); break;
                            case top.Ts.CustomFieldType.PickList: appendCustomEditCombo(result[i], container); break;
                            default:
                        }
                    } catch (err) {
                        var errorString = '1001 NewTicket.js createCustomFields   FieldType: ' + result[i].FieldType + '  CustomFieldID: ' + result[i].CustomFieldID + ' ::  Exception Properties: ';
                        for (var property in err) { errorString += property + ': ' + err[property] + '; '; }
                        top.Ts.Services.System.LogException(err.message, errorString);
                        $('.newticket-custom-field').remove();
                        return;
                    }
                }
            }
            $('<div>').addClass('ui-helper-clearfix').appendTo(container);
      }
        appendCategorizedCustomFields(result, null);
    });
  }


  function appendCustomEditCombo(field, container, insertAfterElement, idClass) {
    var div = $('<div>')
    .addClass('label-block newticket-custom-field')
    .data('o', field)
    if (!insertAfterElement) {
        div.appendTo(container);
    }
    else {
        div.insertAfter(insertAfterElement);
    }

    if (idClass) {
        div.addClass(idClass);
    }

    $('<span>')
    .addClass('label')
    .text(field.Name)
    .appendTo(div);

    var select = $('<select>')
    .appendTo(div);

    var items = field.ListValues.split('|');
    for (var i = 0; i < items.length; i++) {
      $('<option>').attr('value', items[i]).text(items[i]).appendTo(select);
    }

    var ticketTypeID = $('.newticket-type option:selected').data('o').TicketTypeID;
    if (field.AuxID == ticketTypeID && items.length > 0) {
        var productID = $('.newticket-product option:selected').val();
        top.Ts.Services.CustomFields.GetParentValueMatchingCustomFields(field.CustomFieldID, items[0], productID, function (result) {
            for (var i = 0; i < result.length; i++) {
                try {
                    switch (result[i].FieldType) {
                        case top.Ts.CustomFieldType.Text: appendCustomEdit(result[i], null, div, field.CustomFieldID + 'child'); break;
                        case top.Ts.CustomFieldType.Date: appendCustomEditDate(result[i], null, div, field.CustomFieldID + 'child'); break;
                        case top.Ts.CustomFieldType.Time: appendCustomEditTime(result[i], null, div, field.CustomFieldID + 'child'); break;
                        case top.Ts.CustomFieldType.DateTime: appendCustomEditDateTime(result[i], null, div, field.CustomFieldID + 'child'); break;
                        case top.Ts.CustomFieldType.Boolean: appendCustomEditBool(result[i], null, div, field.CustomFieldID + 'child'); break;
                        case top.Ts.CustomFieldType.Number: appendCustomEditNumber(result[i], null, div, field.CustomFieldID + 'child'); break;
                        case top.Ts.CustomFieldType.PickList: appendCustomEditCombo(result[i], null, div, field.CustomFieldID + 'child'); break;
                        default:
                    }
                } catch (err) {
                    var errorString = '1001 NewTicket.js createCustomFields   FieldType: ' + result[i].FieldType + '  CustomFieldID: ' + result[i].CustomFieldID + ' ::  Exception Properties: ';
                    for (var property in err) { errorString += property + ': ' + err[property] + '; '; }
                    top.Ts.Services.System.LogException(err.message, errorString);
                    $('.newticket-custom-field').remove();
                    return;
                }

            }
            showCustomFields();
        });

    }

    select.combobox({ selected: function (e, ui) {
      top.Ts.Services.Tickets.GetValueTemplateText(ui.item.value, function (result) {
        if (result != null && result != "") {
            tinyMCE.activeEditor.selection.setContent($('.newticket-desc').html() + '<br/><br/>' + result);
          //$('.newticket-desc').html($('.newticket-desc').html() + '<br/><br/>' + result);
        }
      });

      $('.' + field.CustomFieldID + 'child').remove();
      var productID = $('.newticket-product option:selected').val();
      top.Ts.Services.CustomFields.GetParentValueMatchingCustomFields(field.CustomFieldID, ui.item.value, productID, function (result) {
          for (var i = 0; i < result.length; i++) {
              try {
                  switch (result[i].FieldType) {
                      case top.Ts.CustomFieldType.Text: appendCustomEdit(result[i], null, div, div.attr("class") + ' ' + field.CustomFieldID + 'child'); break;
                      case top.Ts.CustomFieldType.Date: appendCustomEditDate(result[i], null, div, div.attr("class") + ' ' + field.CustomFieldID + 'child'); break;
                      case top.Ts.CustomFieldType.Time: appendCustomEditTime(result[i], null, div, div.attr("class") + ' ' + field.CustomFieldID + 'child'); break;
                      case top.Ts.CustomFieldType.DateTime: appendCustomEditDateTime(result[i], null, div, div.attr("class") + ' ' + field.CustomFieldID + 'child'); break;
                      case top.Ts.CustomFieldType.Boolean: appendCustomEditBool(result[i], null, div, div.attr("class") + ' ' + field.CustomFieldID + 'child'); break;
                      case top.Ts.CustomFieldType.Number: appendCustomEditNumber(result[i], null, div, div.attr("class") + ' ' + field.CustomFieldID + 'child'); break;
                      case top.Ts.CustomFieldType.PickList: appendCustomEditCombo(result[i], null, div, div.attr("class") + ' ' + field.CustomFieldID + 'child'); break;
                      default:
                  }
              } catch (err) {
                  var errorString = '1001 NewTicket.js createCustomFields   FieldType: ' + result[i].FieldType + '  CustomFieldID: ' + result[i].CustomFieldID + ' ::  Exception Properties: ';
                  for (var property in err) { errorString += property + ': ' + err[property] + '; '; }
                  top.Ts.Services.System.LogException(err.message, errorString);
                  $('.newticket-custom-field').remove();
                  return;
              }

          }
          showCustomFields();
      });

    }
    });
  }

  function appendCustomEditNumber(field, container, insertAfterElement, idClass) {
    var div = $('<div>')
    .addClass('label-block newticket-custom-field')
    .data('o', field)
    if (!insertAfterElement) {
        div.appendTo(container);
    }
    else {
        div.insertAfter(insertAfterElement);
    }

    if (idClass) {
        div.addClass(idClass);
    }

    $('<span>')
    .addClass('label')
    .text(field.Name)
    .appendTo(div);

    $('<input>')
    .attr('type', 'text')
    .addClass('ui-widget-content ui-corner-all')
    .numeric()
    .appendTo(div);
  }

  function appendCustomEdit(field, container, insertAfterElement, idClass) {
    var div = $('<div>')
    .addClass('label-block newticket-custom-field')
    .data('o', field)
    if (!insertAfterElement) {
        div.appendTo(container);
    }
    else {
        div.insertAfter(insertAfterElement);
    }

    if (idClass) {
        div.addClass(idClass);
    }

    $('<span>')
    .addClass('label')
    .text(field.Name)
    .appendTo(div);

    var customFieldTextBox = $('<input>')
    .attr('type', 'text')
    .addClass('ui-widget-content ui-corner-all')
    .css('width', '150px')
    .appendTo(div);

    if (field.Mask) {
      customFieldTextBox.mask(field.Mask);
      customFieldTextBox.attr("placeholder", field.Mask);
    }
  }

  function appendCustomEditDate(field, container, insertAfterElement, idClass) {
    var div = $('<div>')
    .addClass('label-block newticket-custom-field')
    .data('o', field)
    if (!insertAfterElement) {
        div.appendTo(container);
    }
    else {
        div.insertAfter(insertAfterElement);
    }

    if (idClass) {
        div.addClass(idClass);
    }

    $('<span>')
    .addClass('label')
    .text(field.Name)
    .appendTo(div);

    $('<input>')
    .attr('type', 'text')
    .appendTo(div)
    .addClass('ui-widget-content ui-corner-all newticket-custom-datetime')
    .css('width', '150px')
    .datepicker({ dateFormat: dateFormat });
  }

  function appendCustomEditTime(field, container, insertAfterElement, idClass) {
    var div = $('<div>')
    .addClass('label-block newticket-custom-field')
    .data('o', field)
    if (!insertAfterElement) {
        div.appendTo(container);
    }
    else {
        div.insertAfter(insertAfterElement);
    }

    if (idClass) {
        div.addClass(idClass);
    }

    $('<span>')
    .addClass('label')
    .text(field.Name)
    .appendTo(div);

    $('<input>')
    .attr('type', 'text')
    .appendTo(div)
    .addClass('ui-widget-content ui-corner-all newticket-custom-datetime')
    .css('width', '150px')
    .timepicker();
  }

  function appendCustomEditDateTime(field, container, insertAfterElement, idClass) {
    var div = $('<div>')
    .addClass('label-block newticket-custom-field')
    .data('o', field)
    if (!insertAfterElement) {
        div.appendTo(container);
    }
    else {
        div.insertAfter(insertAfterElement);
    }

    if (idClass) {
        div.addClass(idClass);
    }

    $('<span>')
    .addClass('label')
    .text(field.Name)
    .appendTo(div);

    $('<input>')
    .attr('type', 'text')
    .appendTo(div)
    .addClass('ui-widget-content ui-corner-all newticket-custom-datetime')
    .css('width', '150px')
    .datetimepicker({ dateFormat: dateFormat });
  }


  function appendCustomEditBool(field, container, insertAfterElement, idClass) {
    var div = $('<div>')
    .addClass('label-block newticket-custom-field checkbox')
    .data('o', field)
    if (!insertAfterElement) {
        div.appendTo(container);
    }
    else {
        div.insertAfter(insertAfterElement);
    }

    if (idClass) {
        div.addClass(idClass);
    }


    $('<input>')
    .attr('type', 'checkbox')
    .appendTo(div);

    $('<label>')
    .text(field.Name)
    .appendTo(div);
  }


  function showCustomFields() {
    var ticketTypeID = $('.newticket-type option:selected').data('o').TicketTypeID;
    $('.custom-fields').hide();
    $('.newticket-custom-field').hide().each(

    function () {
      var field = $(this).data('o');
      if (field.AuxID == ticketTypeID) {
          $(this).show();
          $(this).parent().parent().show();
      }
    }
    );

  }

  function addUnassignedComboItem(el) {
    $('<option>').attr('value', -1).text('Unassigned').data('o', null).prependTo(el);
    el.combobox('setValue', -1);
  }

  initEditor($('.newticket-desc'), function (ed) {
    setTicketTypeTemplateText();
    $('.page-loading').hide().next().show();
  });


  function initEditor(element, init) {
    top.Ts.Settings.System.read('EnableScreenR', 'True', function (enableScreenR) {

      var editorOptions = {
        plugins: "autoresize paste link code textcolor image moxiemanager table",
        toolbar1: "insertPasteImage insertKb insertTicket image insertimage insertDropBox recordScreen insertUser recordVideo | link unlink | undo redo removeformat | cut copy paste pastetext | outdent indent | bullist numlist",
        toolbar2: "alignleft aligncenter alignright alignjustify | forecolor backcolor | fontselect fontsizeselect | bold italic underline strikethrough blockquote | code | table",
        statusbar: false,
        gecko_spellcheck: true,
        extended_valid_elements: "a[accesskey|charset|class|coords|dir<ltr?rtl|href|hreflang|id|lang|name|onblur|onclick|ondblclick|onfocus|onkeydown|onkeypress|onkeyup|onmousedown|onmousemove|onmouseout|onmouseover|onmouseup|rel|rev|shape<circle?default?poly?rect|style|tabindex|title|target|type],script[charset|defer|language|src|type],table[class=table|border:1]",
        content_css: "../Css/jquery-ui-latest.custom.css,../Css/editor.css",
        body_class: "ui-widget ui-widget-content",

        convert_urls: true,
        remove_script_host: false,
        relative_urls: false,
        template_external_list_url: "tinymce/jscripts/template_list.js",
        external_link_list_url: "tinymce/jscripts/link_list.js",
        external_image_list_url: "tinymce/jscripts/image_list.js",
        media_external_list_url: "tinymce/jscripts/media_list.js",
        menubar: false,
        moxiemanager_leftpanel: false,
        moxiemanager_fullscreen: false,
        moxiemanager_title: top.Ts.System.Organization.Name,
        moxiemanager_hidden_tools: (top.Ts.System.User.IsSystemAdmin == true) ? "" : "manage",
        paste_data_images: false,

        setup: function (ed) {
          ed.on('init', function (e) {
            top.Ts.System.refreshUser(function () {
              if (top.Ts.System.User.FontFamilyDescription != "Unassigned") {
                  ed.execCommand("FontName", false, GetTinyMCEFontName(top.Ts.System.User.FontFamily));
                  ed.getBody().style.fontFamily = GetTinyMCEFontName(top.Ts.System.User.FontFamily);
              }
              else if (top.Ts.System.Organization.FontFamilyDescription != "Unassigned") {
                  ed.execCommand("FontName", false, GetTinyMCEFontName(top.Ts.System.Organization.FontFamily));
                  ed.getBody().style.fontFamily = GetTinyMCEFontName(top.Ts.System.Organization.FontFamily);
              }

              if (top.Ts.System.User.FontSize != "0") {
                  ed.execCommand("FontSize", false, top.Ts.System.User.FontSizeDescription);
                  ed.getBody().style.fontSize = GetTinyMCEFontSize(top.Ts.System.User.FontSize + 1);
              }
              else if (top.Ts.System.Organization.FontSize != "0") {
                  ed.execCommand("FontSize", false, top.Ts.System.Organization.FontSizeDescription);
                  ed.getBody().style.fontSize = GetTinyMCEFontSize(top.Ts.System.Organization.FontSize + 1);
              }
            });
          });

          ed.addButton('insertTicket', {
            title: 'Insert Ticket',
            //image: '../images/nav/16/tickets.png',
            icon: 'awesome fa fa-ticket',
            onclick: function () {
              top.Ts.System.logAction('New Ticket - Ticket Inserted');
              top.Ts.MainPage.selectTicket(null, function (ticketID) {
                top.Ts.Services.Tickets.GetTicket(ticketID, function (ticket) {
                  ed.focus();
                  ticket.IsParent = null;
                  appendRelated(ticket);
                  var html = '<a href="' + top.Ts.System.AppDomain + '?TicketNumber=' + ticket.TicketNumber + '" target="_blank" onclick="top.Ts.MainPage.openTicket(' + ticket.TicketNumber + '); return false;">Ticket ' + ticket.TicketNumber + '</a>';
                  ed.selection.setContent(html);
                  ed.execCommand('mceAutoResize');
                  ed.focus();
                }, function () {
                  alert('There was a problem inserting the ticket link.');
                });
              });
            }
          });


          ed.addButton('insertPasteImage', {
            title: 'Insert Pasted Image',
            //image: '../images/nav/16/imagepaste.png',
            icon: 'awesome fa fa-paste',
            onclick: function () {
              if (BrowserDetect.browser == 'Safari' || BrowserDetect.browser == 'Explorer') {
                alert("Sorry, this feature is not supported by " + BrowserDetect.browser);
              }
              else {
                top.Ts.MainPage.pasteImage(null, function (result) {
                  ed.focus();
                  if (result != "") {
                    var html = '<img src="' + top.Ts.System.AppDomain + '/dc/' + result + '"</a>&nbsp;<br/>';
                    ed.selection.setContent(html);
                    ed.execCommand('mceAutoResize');
                    ed.focus();
                  }
                });
              }
            }
          });

          ed.addButton('insertDropBox', {
            title: 'Insert DropBox',
            icon: 'awesome fa fa-dropbox',
            //image: '../images/icons/dropbox.png',
            onclick: function () {
              var options = {
                linkType: "preview",
                success: function (files) {
                  top.Ts.System.logAction('New Ticket - DropBox Added');
                  ed.focus();
                  var html = '<a href=' + files[0].link + '>' + files[0].name + '</a>';
                  ed.selection.setContent(html);
                  ed.execCommand('mceAutoResize');
                  ed.focus();
                },
                cancel: function () {
                  alert('There was a problem inserting the dropbox file.');
                }
              };
              Dropbox.choose(options);
            }
          });

          ed.addButton('insertUser', {
              title: 'Insert Userstamp',
              icon: 'awesome fa fa-clock-o',
              //image: '../images/icons/dropbox.png',
              onclick: function () {
                  var html = Date(Date.UTC(Date.Now)) + ' ' + top.Ts.System.User.FirstName + ' ' + top.Ts.System.User.LastName + ' : ';
                  ed.selection.setContent(html);
                  ed.execCommand('mceAutoResize');
                  ed.focus();
              }
          });

          ed.addButton('insertKb', {
            title: 'Insert Knowledgebase',
            //image: '../images/nav/16/knowledge.png',
            icon: 'awesome fa fa-book',
            onclick: function () {
              filter = new top.TeamSupport.Data.TicketLoadFilter();
              filter.IsKnowledgeBase = true;
              top.Ts.MainPage.selectTicket(filter, function (ticketID) {
                top.Ts.Services.Tickets.GetKBTicketAndActions(ticketID, function (result) {
                  if (result === null) {
                    alert('There was an error inserting your knowledgebase ticket.');
                    return;
                  }
                  var ticket = result[0];
                  var actions = result[1];

                  var html = '<div>';

                  for (var i = 0; i < actions.length; i++) {
                    html = html + '<div>' + actions[i].Description + '</div></br>';
                  }
                  html = html + '</div>';

                  ed.focus();
                  ed.selection.setContent(html);
                  ed.execCommand('mceAutoResize');
                  ed.focus();
                  top.Ts.System.logAction('New Ticket - KB Inserted');

                  //needs to resize or go to end

                }, function () {
                  alert('There was an error inserting your knowledgebase ticket.');
                });
              });
            }
          });

          ed.addButton('recordVideo', {
              title: 'Record video',
              //image: '../images/icons/Symbol_Record.png',
              icon: 'awesome fa fa-video-camera',
              onclick: function () {
                  top.Ts.System.logAction('New Ticket - Video Recording Button Clicked');
                  if (OT.checkSystemRequirements() == 1 || BrowserDetect.browser == "Mozilla") {
                      var dynamicPub = $("#publisher");
                      $("#recordVideoContainer").show();
                      dynamicPub.show();
                      dynamicPub.attr("id", "tempContainer");
                      dynamicPub.attr("width", "400px");
                      dynamicPub.attr("height", "400px");

                      if (dynamicPub.length == 0)
                          dynamicPub = $("#tempContainer");



                      top.Ts.Services.Tickets.GetSessionInfo(function (resultID) {
                          sessionId = resultID[0];
                          token = resultID[1];
                          session = OT.initSession(apiKey, sessionId);
                          session.connect(token, function (error) {
                              publisher = OT.initPublisher(dynamicPub.attr('id'), {
                                  insertMode: 'append',
                                  width: '100%',
                                  height: '100%'
                              });
                              session.publish(publisher);
                          });
                      });

                  }
                  else {
                      alert("Your client does not support video recording.")
                  }
              }
          });

          if (enableScreenR.toLowerCase() != 'false' && !(BrowserDetect.browser == 'Safari' && BrowserDetect.OS == "Windows")) {
            ed.addButton('recordScreen', {
              title: 'Record Screen',
              //image: '../images/icons/Symbol_Record.png',
              icon: 'awesome fa fa-circle',
              onclick: function () {
                if ($("#recorder").length == 0) {

                  switch (BrowserDetect.browser) {
                    case "Chrome":
                      top.Ts.Services.Settings.ReadUserSetting('ReadScreenRecordingChromeInfo', 0, function (alreadyReadInfo) {
                        if (alreadyReadInfo == 0) {
                          $(".pAllowPluginsToRunInstructions").html("\
To use screen recording in this browser before September of 2015 \
<a href='https://support.google.com/chrome/answer/6213033' target='_blank' class='ui-state-default ts-link'>these instructions</a> \
need to be followed, otherwise the browser will continue to ask to install Java. \
After September 2015 you’ll need to use an alternate web browser like FireFox or Internet Explorer.<br><br>\
Also, the Java plugins need to be allowed to run by clicking on the \
<img src='../Images/icons/ChromePluginIcon.png' alt='plugins icon'> \
on the right side of the address bar");
                          $('.divScreenRecorderMessages').show();
                        }
                      });
                      break;
                    case "Firefox":
                      top.Ts.Services.Settings.ReadUserSetting('ReadScreenRecordingFirefoxInfo', 0, function (alreadyReadInfo) {
                        if (alreadyReadInfo == 0) {
                          $(".pAllowPluginsToRunInstructions").html("\
Please allow the screen recorder Java plugins to run on your browser by clicking on the \
<img src='../Images/icons/FirefoxPluginIcon.png' alt='plugins icon'> \
on the left side of the address bar.");
                          $('.divScreenRecorderMessages').show();
                        }
                      });
                      break;
                    case "Explorer":
                      top.Ts.Services.Settings.ReadUserSetting('ReadScreenRecordingExplorerInfo', 0, function (alreadyReadInfo) {
                        if (alreadyReadInfo == 0) {
                          $(".pAllowPluginsToRunInstructions").html("\
Please allow the screen recorder Java plugins to run on your browser by clicking on Allow button at the bottom of the page: \
<img src='../Images/icons/IEPluginWindow.png' alt='plugins icon' width='100%' style='margin-top: 10px'>");
                          $('.divScreenRecorderMessages').show();
                        }
                      });
                      break;
                    case "Safari":
                      if (BrowserDetect.OS == "Windows") {
                        top.Ts.Services.Settings.ReadUserSetting('ReadScreenRecordingSafariInWindowsInfo', 0, function (alreadyReadInfo) {
                          if (alreadyReadInfo == 0) {
                            $(".pAllowPluginsToRunInstructions").html("\
This browser in Windows usually fails to detect Java preventing the recorder to start. Read \
<a href='http://stackoverflow.com/questions/11235578/when-viewing-an-applet-why-does-safari-for-windows-display-java-is-unavailable' target='_blank' class='ui-state-default ts-link'>this</a> \
for more information or use an alternate browser like Firefox or Internet Explorer.");
                            $('.divScreenRecorderMessages').show();
                          }
                        });
                      }
                      else {
                        top.Ts.Services.Settings.ReadUserSetting('ReadScreenRecordingSafariInfo', 0, function (alreadyReadInfo) {
                          if (alreadyReadInfo == 0) {
                            $(".pAllowPluginsToRunInstructions").html("\
The following steps will refresh your browser<br><br> \
1. Allow the screen recorder Java plugins to run on your browser by clicking on the Trust button at the top of the page: <br>\
<img src='../Images/icons/SafariInMacPluginDialog.png' alt='plugin dialog' width='30%' style='margin-top: 10px'><br><br> \
2. Navigate to Safari > Preferences > Security > Internet Plugins - Website Settings > Java and change the " + top.Ts.System.AppDomain + " setting to Run in Unsafe Mode and click on the Trust button: <br>\
<img src='../Images/icons/SafariInMacUnsafeModeDialog.png' alt='plugin dialog' width='30%' style='margin-top: 10px'>");
                            $('.divScreenRecorderMessages').show();
                          }
                        });
                      }

                      break;
                    default:
                      top.Ts.Services.Settings.ReadUserSetting('ReadScreenRecordingInfo', 0, function (alreadyReadInfo) {
                        if (alreadyReadInfo == 0) {
                          $(".pAllowPluginsToRunInstructions").html("Please verify java is supported and allowed to run in your browser.");
                          $('.divScreenRecorderMessages').show();
                        }
                      });
                  }

                  //if (deployJava.versionCheck("1.8.0_45+")) {
                    var applet = document.createElement("applet");
                    applet.id = "recorder";
                    applet.archive = "Launch.jar"
                    applet.code = "com.bixly.pastevid.driver.Launch";
                    applet.width = 200;
                    applet.height = 150;
                    var orgId = top.Ts.System.Organization.OrganizationID;
                    var param1 = document.createElement("param");
                    param1.name = "jnlp_href";
                    param1.value = "launch.jnlp";
                    applet.appendChild(param1);
                    var param2 = document.createElement("param");
                    param2.name = "orgId";
                    param2.value = orgId;
                    applet.appendChild(param2);
                    var param3 = document.createElement("param");
                    param3.name = "permissions";
                    param3.value = 'all-permissions';
                    applet.appendChild(param3);

                    $('.fa-circle').removeClass("fa-circle").addClass("fa-circle-o-notch fa-spin");
                    document.getElementsByTagName("body")[0].appendChild(applet);

                  //}
                  //else {
                  //  userInput = confirm(
                  //          "You need the latest Java(TM) Runtime Environment.\n" +
                  //          "Please restart your computer after updating.\n" +
                  //          "Would you like to update now?");
                  //  if (userInput == true) {
                  //    window.open("http://java.com/en/download", '_blank');
                  //  }
                  //}
                }
              }
            });
          }

        }
      , oninit: init
      };
      $(element).tinymce(editorOptions);
    });
  }

  $('.ticket-rail .collapsable')
    .prepend('<span class="ui-icon ui-icon-triangle-1-s">')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      var icon = $(this).find('.ui-icon');
      if (icon.hasClass('ui-icon-triangle-1-s')) {
        icon.removeClass('ui-icon-triangle-1-s').addClass('ui-icon-triangle-1-e');
        $(this).next().hide();
      }
      else {
        icon.removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');
        $(this).next().show();
      }
    });


  function appendCustomer(customer) {
    if (customer == null) return;
    top.Ts.System.logAction('New Ticket - Customer Added');

    var itemClass = (customer.UserID ? 'ticket-customer-contact' : 'ticket-customer-company');
    var item = $('<div>')
    .addClass('ticket-removable-item ui-corner-all ts-color-bg-accent ' + itemClass)
    .data('data', customer);

    if (customer.UserID) {
      $('<span>').addClass('ui-icon ui-icon-close').appendTo(item);
      var title = $('<div>').addClass('ticket-removable-item-title').appendTo(item);
      $('<a>')
    .attr('href', '#')
    .attr('target', '_blank')
    .click(function (e) {
      e.preventDefault();
      top.Ts.MainPage.openNewContact(customer.UserID);
    })
    .text(ellipseString(customer.Contact, 30))
    .appendTo(title);
      $('<span>')
    .addClass('ts-icon ts-icon-info')
    .attr('rel', '../../../Tips/User.aspx?UserID=' + customer.UserID)
    .cluetip(clueTipOptions)
    .appendTo(title);

      var desc = $('<div>').addClass('ticket-removable-item-description').appendTo(item);
      $('<a>')
    .attr('href', '#')
    .click(function (e) {
      e.preventDefault();
      top.Ts.MainPage.openNewCustomer(customer.OrganizationID);
    })
    .text(ellipseString(customer.Company, 30))
    .appendTo(desc);
      $('<span>')
    .addClass('ts-icon ts-icon-info')
    .attr('rel', '../../../Tips/Customer.aspx?CustomerID=' + customer.OrganizationID)
    .cluetip(clueTipOptions)
    .appendTo(desc);
    }
    else {
      $('<span>').addClass('ui-icon ui-icon-close').appendTo(item);
      var title = $('<div>').addClass('ticket-removable-item-title').appendTo(item);
      $('<a>')
    .attr('href', '#')
    .click(function (e) {
      e.preventDefault();
      top.Ts.MainPage.openNewCustomer(customer.OrganizationID);
    })
    .text(ellipseString(customer.Company, 30))
    .appendTo(title);

      $('<span>')
    .addClass('ts-icon ts-icon-info')
    .attr('rel', '../../../Tips/Customer.aspx?CustomerID=' + customer.OrganizationID)
    .cluetip(clueTipOptions)
    .appendTo(title);
    }
    $('#divCustomers .newticket-noitems').hide().after(item);
    loadProducts();

  }

  $('#divCustomers').delegate('.ui-icon-close', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $(this).closest('.ticket-removable-item').remove();
    loadProducts();
    if ($('#divCustomers .ticket-removable-item').length < 1) {
      $('#divCustomers .newticket-noitems').show()
    }
  });

  var execGetCustomer = null;
  function getCustomers(request, response) {
    if (execGetCustomer) { execGetCustomer._executor.abort(); }
    execGetCustomer = top.Ts.Services.Organizations.GetUserOrOrganizationForTicket(request.term, function (result) { response(result); });
  }

  function ellipseString(text, max) { return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text; };

  var execGetCompany = null;
  function getCompany(request, response) {
    if (execGetCompany) { execGetCompany._executor.abort(); }
    execGetCompany = top.Ts.Services.Organizations.WCSearchOrganization(request.term, function (result) { response(result); });
  }

  $('.ticket-new-customer-company')
        .autocomplete({
          minLength: 2,
          source: getCompany,
          select: function (event, ui) {
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading')
          }
        });

  if (!top.Ts.System.User.CanCreateContact && !top.Ts.System.User.IsSystemAdmin) {
    $('.ticket-customer-new').hide();
  }

  $('.ticket-customer-new').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.ticket-new-customer').show();
  });

  $('.ticket-new-customer-cancel').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.ticket-new-customer-email').val('');
    $('.ticket-new-customer-first').val('');
    $('.ticket-new-customer-last').val('');
    $('.ticket-new-customer-company').val('');
    $('.ticket-new-customer').hide();
  });

  $('.ticket-new-customer-save').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.System.logAction('New Ticket - New Customer Added');
    var email = $('.ticket-new-customer-email').val();
    var firstName = $('.ticket-new-customer-first').val();
    var lastName = $('.ticket-new-customer-last').val();
    var companyName = $('.ticket-new-customer-company').val();
    var phone = $('.ticket-new-customer-phone').val();
    top.Ts.Services.Users.CreateNewContact(email, firstName, lastName, companyName, phone, false, function (result) {
      if (result.indexOf("u") == 0 || result.indexOf("o") == 0) {
        top.Ts.Services.Tickets.GetTicketCustomer(result.charAt(0), result.substring(1), function (result) {
          appendCustomer(result);
          $('.ticket-new-customer-email').val('');
          $('.ticket-new-customer-first').val('');
          $('.ticket-new-customer-last').val('');
          $('.ticket-new-customer-company').val('');
          $('.ticket-new-customer-phone').val('');
          $('.ticket-new-customer').hide();
        });
      }
      else if (result.indexOf("The company you have specified is invalid") !== -1) {
        if (top.Ts.System.User.CanCreateCompany || top.Ts.System.User.IsSystemAdmin) {
          if (confirm('Unknown company, would you like to create it?')) {
              top.Ts.Services.Users.CreateNewContact(email, firstName, lastName, companyName, phone, true, function (result) {
              top.Ts.Services.Tickets.GetTicketCustomer(result.charAt(0), result.substring(1), function (result) {
                appendCustomer(result);
                $('.ticket-new-customer-email').val('');
                $('.ticket-new-customer-first').val('');
                $('.ticket-new-customer-last').val('');
                $('.ticket-new-customer-company').val('');
                $('.ticket-new-customer-phone').val('');
                $('.ticket-new-customer').hide();
              });
            });
          }
        }
        else {
          alert("We're sorry, but you do not have the rights to create a new company.");
          $('.ticket-new-customer-email').val('');
          $('.ticket-new-customer-first').val('');
          $('.ticket-new-customer-last').val('');
          $('.ticket-new-customer-company').val('');
          $('.ticket-new-customer-phone').val('');
          $('.ticket-new-customer').hide();
        }
      }
      else {
        alert(result);
      }
    });
  });

  $('.ticket-customer-add')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      //$('.ticket-rail-input').remove();
      $('.ticket-new-customer').hide();
      $(this).parent().find('.ui-icon').removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');

      var container = $('<div>')
        .addClass('ticket-rail-input')
        .prependTo($(this).parent().next().show());

      var input = $('<input type="text">')
        .addClass('ui-corner-all ui-widget-content')
        .autocomplete({
          minLength: 2,
          source: getCustomers,
          select: function (event, ui) {
            $(this).removeClass('ui-autocomplete-loading');
            alertMessage = ui.item;
            top.Ts.Services.Tickets.GetTicketCustomer(ui.item.data, ui.item.id, function (result) {
              appendCustomer(result);
              if (alertMessage.data == "u") {
                top.Ts.Services.Customers.LoadAlert(alertMessage.id, top.Ts.ReferenceTypes.Users, function (note) {
                  if (note != null) {
                    $('#modalAlertMessage').html(note.Description);
                    $('#alertID').val(note.RefID);
                    $('#alertType').val(note.RefType);
                    var buttons = {
                        "Close": function () {
                            $(this).dialog("close");
                        },
                        "Snooze": function () {
                            top.Ts.Services.Customers.SnoozeAlert($('#alertID').val(), $('#alertType').val());
                            $(this).dialog("close");
                        }
                    }

                    if (!top.Ts.System.Organization.HideDismissNonAdmins || top.Ts.System.User.IsSystemAdmin) {
                        buttons["Dismiss"] = function () {
                            top.Ts.Services.Customers.DismissAlert($('#alertID').val(), $('#alertType').val());
                            $(this).dialog("close");
                        }
                    }

                    $("#dialog").dialog({
                      resizable: false,
                      width: 'auto',
                      height: 'auto',
                      create: function () {
                        $(this).css('maxWidth', '800px');
                      },
                      modal: true,
                      buttons: buttons
                    });
                  }
                });
              }
              else {
                top.Ts.Services.Customers.LoadAlert(alertMessage.id, top.Ts.ReferenceTypes.Organizations, function (note) {
                  if (note != null) {
                    $('#modalAlertMessage').html(note.Description);
                    $('#alertID').val(note.RefID);
                    $('#alertType').val(note.RefType);

                    var buttons = {
                        "Close": function () {
                            $(this).dialog("close");
                        },
                        "Snooze": function () {
                            top.Ts.Services.Customers.SnoozeAlert($('#alertID').val(), $('#alertType').val());
                            $(this).dialog("close");
                        }
                    }

                    if (!top.Ts.System.Organization.HideDismissNonAdmins || top.Ts.System.User.IsSystemAdmin) {
                        buttons["Dismiss"] = function () {
                            top.Ts.Services.Customers.DismissAlert($('#alertID').val(), $('#alertType').val());
                            $(this).dialog("close");
                        }
                    }

                    $("#dialog").dialog({
                      resizable: false,
                      width: 'auto',
                      height: 'auto',
                      create: function () {
                        $(this).css('maxWidth', '800px');
                      },
                      modal: true,
                      buttons: buttons
                    });
                  }
                });
              }
            });
          },
          close: function () {
            $(this).val('');
            $(this).removeClass('ui-autocomplete-loading');
          }
        })
        .appendTo(container)
        .focus()
        .width(container.width() - 48 - 12);

      $('<span>')
    .addClass('ts-icon ts-icon-save')
    .hide()
    .click(function (e) {
      var item = $(this).prev().data('item');
      top.Ts.Services.Tickets.GetTicketCustomer(item.data, item.id, function (result) {
        appendCustomer(result);
      });
      $(this).parent().remove();

    })
    .appendTo(container)

      $('<span>')
    .addClass('ts-icon ts-icon-cancel')
    .click(function (e) {
      $(this).parent().remove();
    })
    .appendTo(container);

      $('<div>').addClass('clearfix').appendTo(container);
    });


  var tipTimer = null;

  var clueTipOptions = top.Ts.Utils.getClueTipOptions(tipTimer);

  $('body').delegate('.ts-icon-info', 'mouseout', function (e) {
    if (tipTimer != null) clearTimeout(tipTimer);
    tipTimer = setTimeout("$(document).trigger('hideCluetip');", 1000);
  });

  $('body').delegate('.cluetip', 'mouseover', function (e) {
    if (tipTimer != null) clearTimeout(tipTimer);
  });


  //Assets
  function appendAsset(asset) {
    top.Ts.System.logAction('New Ticket - Asset Added');

    var item = $('<div>')
      .addClass('ticket-removable-item ui-corner-all ts-color-bg-accent ticket-asset')
      .data('data', asset);

    $('<span>').addClass('ui-icon ui-icon-close').appendTo(item);
    var title = $('<div>').addClass('ticket-removable-item-title').appendTo(item);
    $('<a>')
      .attr('href', '#')
      .addClass('value ui-state-default ts-link')
      .click(function (e) {
        e.preventDefault();
        top.Ts.MainPage.openAsset(asset.AssetID);
      })
      .text(ellipseString(asset.Name, 30))
      .appendTo(title);

    $('<span>')
      .addClass('ts-icon ts-icon-info')
      .attr('rel', '../../../Tips/Asset.aspx?AssetID=' + asset.AssetID)
      .cluetip(clueTipOptions)
      .appendTo(title);

    $('#divAssets .newticket-noitems').hide().after(item);
  }

  var execGetAssets = null;
  function getAssets(request, response) {
    if (execGetAssets) { execGetAssets._executor.abort(); }
    execGetAssets = top.Ts.Services.Assets.FindAsset(request.term, function (result) { response(result); });
  }


  $('#divAssets').delegate('.ticket-asset .ui-icon-close', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $(this).closest('.ticket-removable-item').remove();
    if ($('#divAssets .ticket-removable-item').length < 1) {
      $('#divAssets .newticket-noitems').show()
    }
  });

  $('.ticket-asset-add')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      $('.ticket-rail-input').remove();

      $(this).parent().find('.ui-icon').removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');

      var container = $('<div>')
        .addClass('ticket-rail-input')
        .prependTo($(this).parent().next().show());

      var input = $('<input type="text">')
        .addClass('ui-corner-all ui-widget-content')
        .autocomplete({
          minLength: 2,
          source: getAssets,
          select: function (event, ui) {
            $(this)
            .data('assetID', ui.item.id)
            .removeClass('ui-autocomplete-loading')
            .next().show();
            top.Ts.Services.Assets.GetAsset(ui.item.id, function (asset) {
              appendAsset(asset);
            });
            $(this).parent().remove();
          }
        })
        .appendTo(container)
        .focus()
        .width(container.width() - 48 - 12);

      $('<span>')
        .addClass('ts-icon ts-icon-cancel')
        .click(function (e) {
          $(this).parent().remove();
        })
      .appendTo(container);

      $('<div>').addClass('clearfix').appendTo(container);
    });


  //Assets

  var execGetTags = null;
  function getTags(request, response) {
    if (execGetTags) { execGetTags._executor.abort(); }
    execGetTags = top.Ts.Services.Tickets.SearchTags(request.term, function (result) { response(result); });
  }

  function appendTag(tag) {
    top.Ts.System.logAction('New Ticket - Tag Added');

    var item = getRemovableItem(tag, 'ticket-tag', 'a');
    /*var link = $('<a>')
    .attr('href', '#')
    .text(ellipseString(tag, 30))
    .click(function (e) {
    e.preventDefault();
    top.Ts.MainPage.openTag($(this).closest('.ticket-tag').data('data').TagID);
    });
    */
    var link = $('<span>')
        .text(ellipseString(tag, 30));
    item.find('.ticket-removable-item-title').empty().append(link);
    $('#divTags .newticket-noitems').hide().after(item);
  }

  $('#divTags').delegate('.ticket-tag .ui-icon-close', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $(this).closest('.ticket-removable-item').remove();
    if ($('#divTags .ticket-removable-item').length < 1) {
      $('#divTags .newticket-noitems').show()
    }
  });


  $('.ticket-tag-add')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      $('.ticket-rail-input').remove();
      $(this).parent().find('.ui-icon').removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');
      var container = $('<div>').
       addClass('ticket-rail-input')
       .prependTo($(this).parent().next().show());
      var input = $('<input type="text">')
        .addClass('ui-corner-all ui-widget-content')
        .autocomplete({
          minLength: 2,
          source: getTags,
          select: function (event, ui) {
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading')
            appendTag(ui.item.value);
            $(this).parent().remove();
          }
        })
        .appendTo(container)
        .focus()
        .width(container.width() - 48 - 12);

      $('<span>')
        .addClass('ts-icon ts-icon-save')
        .click(function (e) {
          appendTag($(this).prev().val());
          $(this).parent().remove();
        })
        .appendTo(container)

      $('<span>')
        .addClass('ts-icon ts-icon-cancel')
        .click(function (e) {
          $(this).parent().remove();
        })
        .appendTo(container);

      $('<div>').addClass('clearfix').appendTo(container);
    });

  function getRemovableItem(data, itemClass, title, description) {
    var item = $('<div>')
    .addClass('ticket-removable-item ui-corner-all ts-color-bg-accent ' + itemClass)
    item.data('data', data);

    $('<span>').addClass('ui-icon ui-icon-close').appendTo(item);
    if (title) $('<div>').addClass('ticket-removable-item-title').text(ellipseString(title, 30)).appendTo(item);
    if (description) $('<div>').addClass('ticket-removable-item-description').text(ellipseString(description, 30)).appendTo(item);
    return item;
  }

  var execGetRelated = null;
  function getRelated(request, response) {
    if (execGetRelated) { execGetRelated._executor.abort(); }
    execGetRelated = top.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) { response(result); });
  }

  function appendRelated(related) {
    top.Ts.System.logAction('New Ticket - Ticket Associated');

    var item = getRemovableItem(related, 'ticket-related', ' ', ' ');

    var icon = 'ts-icon-ticket-related';
    var caption = 'Related';

    if (related.IsParent !== null) {
      icon = (related.IsParent === true ? 'ts-icon-ticket-parent' : 'ts-icon-ticket-child');
      caption = (related.IsParent === true ? 'Parent' : 'Child');
    }

    $('<span>')
      .addClass('ts-icon ' + icon)
      .appendTo(item.find('.ticket-removable-item-title').empty());

    $('<span>')
      .addClass('ticket-removable-text')
      .text(caption)
      .appendTo(item.find('.ticket-removable-item-title'));

    $('<span>')
      .addClass('ts-icon ts-icon-info')
      .attr('rel', '../../../Tips/Ticket.aspx?TicketID=' + related.TicketID)
      .cluetip(clueTipOptions)
      .appendTo(item.find('.ticket-removable-item-title'));

    $('<a>')
        .attr('href', '#')
        .text(ellipseString(related.TicketNumber + ': ' + related.Name, 30))
        .data('number', related.TicketNumber)
        .click(function (e) {
          e.preventDefault();
          top.Ts.MainPage.openTicket($(this).data('number'), true);
        })
          .appendTo(item.find('.ticket-removable-item-description').empty());

    $('#divRelated .newticket-noitems').hide().after(item);
  }

  $('#divRelated').delegate('.ticket-related .ui-icon-close', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $(this).closest('.ticket-removable-item').remove();
    if ($('#divRelated .ticket-removable-item').length < 1) {
      $('#divRelated .newticket-noitems').show()
    }
  });


  $('.ticket-related-add')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      $('.ticket-rail-input').remove();
      $(this).parent().find('.ui-icon').removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');
      var container = $('<div>').
       addClass('ticket-rail-input')
       .prependTo($(this).parent().next().show());
      var input = $('<input type="text">')
        .addClass('ui-corner-all ui-widget-content related-input')
        .autocomplete({
          minLength: 2,
          source: getRelated,
          select: function (event, ui) {
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading')
          },
          position: { my: "right top", at: "right bottom", collision: "fit flip" }
        })
        .appendTo(container)
        .focus()
        .width(container.width() - 30);

      $('<span>')
        .addClass('ts-icon ts-icon-cancel')
        .click(function (e) {
          $(this).parent().remove();
        })
        .appendTo(container);

      $('<div>').addClass('clearfix').appendTo(container);

      var buttons = $('<div>').addClass('buttons').appendTo(container);
      $('<strong>').text('Add as: ').appendTo(buttons);
      $('<button>').text('Related').appendTo(buttons).button().click(function (e) { addRelated(null); });
      $('<button>').text('Parent').appendTo(buttons).button().click(function (e) { addRelated(true); });
      $('<button>').text('Child').appendTo(buttons).button().click(function (e) { addRelated(false); });
      function addRelated(isParent) {
        var item = container.find('.related-input').data('item');
        top.Ts.Services.Tickets.GetTicket(item.data, function (ticket) {
          ticket.IsParent = isParent;
          appendRelated(ticket);
          container.remove();
        });
      }
    });


  function appendQueue(queue) {
    top.Ts.System.logAction('New Ticket - Queued');
    top.Ts.System.logAction('Queued');

    var item = $('<div>')
      .addClass('ticket-removable-item ui-corner-all ts-color-bg-accent ticket-queue')
      .data('data', queue);

    $('<span>').addClass('ui-icon ui-icon-close').appendTo(item);
    var title = $('<div>').addClass('ticket-removable-item-title').appendTo(item);
    $('<a>')
      .attr('href', '#')
      .addClass('value ui-state-default ts-link')
      .click(function (e) {
        e.preventDefault();
        top.Ts.MainPage.openNewContact(queue.UserID);
      })
      .text(ellipseString(queue.FirstName + ' ' + queue.LastName, 30))
      .appendTo(title);

    $('<span>')
      .addClass('ts-icon ts-icon-info')
      .attr('rel', '../../../Tips/User.aspx?UserID=' + queue.UserID)
      .cluetip(clueTipOptions)
      .appendTo(title);

    $('#divQueues .newticket-noitems').hide().after(item);
  }

  var execGetUsers = null;
  function getUsers(request, response) {
    if (execGetUsers) { execGetUsers._executor.abort(); }
    execGetUsers = top.Ts.Services.Users.SearchUsers(request.term, function (result) { response(result); });
  }


  $('#divQueues').delegate('.ticket-queue .ui-icon-close', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $(this).closest('.ticket-removable-item').remove();
    if ($('#divQueues .ticket-removable-item').length < 1) {
      $('#divQueues .newticket-noitems').show()
    }
  });

  $('.ticket-queue-add')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      $('.ticket-rail-input').remove();

      $(this).parent().find('.ui-icon').removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');

      var container = $('<div>')
        .addClass('ticket-rail-input')
        .prependTo($(this).parent().next().show());

      var input = $('<input type="text">')
        .addClass('ui-corner-all ui-widget-content')
        .autocomplete({
          minLength: 2,
          source: getUsers,
          select: function (event, ui) {
            $(this)
            .data('userID', ui.item.id)
            .removeClass('ui-autocomplete-loading')
            .next().show();
            top.Ts.Services.Users.GetUser(ui.item.id, function (user) {
              appendQueue(user);
            });
            $(this).parent().remove();
          }
        })
        .appendTo(container)
        .focus()
        .width(container.width() - 48 - 12);

      $('<span>')
        .addClass('ts-icon ts-icon-cancel')
        .click(function (e) {
          $(this).parent().remove();
        })
      .appendTo(container);

      $('<div>').addClass('clearfix').appendTo(container);
    });

  function appendSubscriber(subscriber) {
    top.Ts.System.logAction('New Ticket - Subcriber Added');

    var item = $('<div>')
      .addClass('ticket-removable-item ui-corner-all ts-color-bg-accent ticket-subscriber')
      .data('data', subscriber);

    $('<span>').addClass('ui-icon ui-icon-close').appendTo(item);
    var title = $('<div>').addClass('ticket-removable-item-title').appendTo(item);
    $('<a>')
      .attr('href', '#')
      .addClass('value ui-state-default ts-link')
      .click(function (e) {
        e.preventDefault();
        top.Ts.MainPage.openNewContact(subscriber.UserID);
      })
      .text(ellipseString(subscriber.FirstName + ' ' + subscriber.LastName, 30))
      .appendTo(title);

    $('<span>')
      .addClass('ts-icon ts-icon-info')
      .attr('rel', '../../../Tips/User.aspx?UserID=' + subscriber.UserID)
      .cluetip(clueTipOptions)
      .appendTo(title);
    $('#divSubscribers .newticket-noitems').hide().after(item);

  }

  $('#divSubscribers').delegate('.ticket-subscriber .ui-icon-close', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $(this).closest('.ticket-removable-item').remove();
    if ($('#divSubscribers .ticket-removable-item').length < 1) {
      $('#divSubscribers .newticket-noitems').show()
    }
  });

  $('.ticket-subscriber-add')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      $('.ticket-rail-input').remove();

      $(this).parent().find('.ui-icon').removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');

      var container = $('<div>')
        .addClass('ticket-rail-input')
        .prependTo($(this).parent().next().show());

      var input = $('<input type="text">')
        .addClass('ui-corner-all ui-widget-content')
        .autocomplete({
          minLength: 2,
          source: getUsers,
          select: function (event, ui) {
            $(this)
            .data('userID', ui.item.id)
            .removeClass('ui-autocomplete-loading')
            .next().show();
            top.Ts.Services.Users.GetUser(ui.item.id, function (user) {
              appendSubscriber(user);
            });
            $(this).parent().remove();
          }
        })
        .appendTo(container)
        .focus()
        .width(container.width() - 48 - 12);

      $('<span>')
        .addClass('ts-icon ts-icon-cancel')
        .click(function (e) {
          $(this).parent().remove();
        })
      .appendTo(container);

      $('<div>').addClass('clearfix').appendTo(container);
    });



  function appendReminder(reminder) {
    top.Ts.System.logAction('New Ticket - Reminder Added');

    var item = $('<div>')
      .addClass('ticket-removable-item ui-corner-all ts-color-bg-accent ticket-reminder')
      .data('o', reminder);

    $('<span>').addClass('ui-icon ui-icon-close').appendTo(item);
    var title = $('<div>').addClass('ticket-removable-item-title').appendTo(item);
    $('<a>')
      .attr('href', '#')
      .addClass('value ui-state-default ts-link')
      .click(function (e) {
        e.preventDefault();
        top.Ts.MainPage.editReminder(
          $(this).closest('.ticket-removable-item').data('o'),
          false,
          function (reminder) { appendReminder(reminder); }
        );
      })
      .text(ellipseString(reminder.Description, 30))
      .appendTo(title);


    $('<div>')
      .addClass('ticket-removable-item-description')
      .text(reminder.DueDate.localeFormat(top.Ts.Utils.getDateTimePattern()))
      .appendTo(item);

    $('#divReminders').append(item);
    $('#divReminders .newticket-noitems').hide().after(item);

  }

  $('#divReminders').delegate('.ticket-reminder .ui-icon-close', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $(this).closest('.ticket-removable-item').remove();
    if ($('#divReminders .ticket-removable-item').length < 1) {
      $('#divReminders .newticket-noitems').show()
    }
  });

  $('.ticket-reminder-add')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      top.Ts.MainPage.editReminder(
        { RefType: top.Ts.ReferenceTypes.Tickets },
        false,
        function (reminder) { appendReminder(reminder); }
      );
    });

  function isFormValid(callback) {
    top.Ts.Services.Organizations.IsProductRequired(function (isProductRequired) {
      top.Ts.Services.Organizations.IsProductVersionRequired(function (isProductVersionRequired) {
        top.Ts.Settings.Organization.read('RequireNewTicketCustomer', false, function (requireNewTicketCustomer) {
          var result = true;
          var product = $('.newticket-product');
          var reportversion = $('.newticket-reported');

          product.parent().removeClass('ui-state-error ui-corner-all');
          if (isProductRequired && ($('.newticket-product').val() == -1)) {
            product.parent().addClass('ui-state-error ui-corner-all');
            result = false;
          }

          reportversion.parent().removeClass('ui-state-error ui-corner-all');
          if (isProductVersionRequired && ($('.newticket-reported').val() == -1)) {
            reportversion.parent().addClass('ui-state-error ui-corner-all');
            result = false;
          }

          $('.newticket-name').parent().removeClass('ui-corner-all ui-state-error');
          if ($.trim($('.newticket-name').val()) == '') {
            $('.newticket-name').parent().addClass('ui-corner-all ui-state-error');
            result = false;
          }

          $('.newticket-custom-field').removeClass('ui-state-error ui-corner-all');
          $('.newticket-custom-field:visible').each(function () {
            var field = $(this).data('o');
            if (field.IsRequired) {
              switch (field.FieldType) {
                case top.Ts.CustomFieldType.Text:
                  if ($.trim($(this).find('input').val()) == '') {
                    $(this).addClass('ui-state-error ui-corner-all');
                    result = false;
                  }
                  break;
                case top.Ts.CustomFieldType.Date:
                case top.Ts.CustomFieldType.Time:
                case top.Ts.CustomFieldType.DateTime:
                  if ($.trim($(this).find('input').val()) == '') {
                    $(this).addClass('ui-state-error ui-corner-all');
                    result = false;
                  }
                  break;
                case top.Ts.CustomFieldType.Number:
                  if ($.trim($(this).find('input').val()) == '') {
                    $(this).addClass('ui-state-error ui-corner-all');
                    result = false;
                  }
                  break;
                case top.Ts.CustomFieldType.PickList:
                  if (field.IsFirstIndexSelect == true && $(this).find('select option:selected').index() < 1) {
                    $(this).addClass('ui-state-error ui-corner-all');
                    result = false;
                  }
                  break;
                default:
              }
            }
          });

          if ($('.newticket-status option:selected').data('o').IsClosed) {
            $('.newticket-custom-field:visible').each(function () {
              var field = $(this).data('o');
              if (field.IsRequiredToClose) {
                switch (field.FieldType) {
                  case top.Ts.CustomFieldType.Text:
                    if ($.trim($(this).find('input').val()) == '') {
                      $(this).addClass('ui-state-error-to-close-custom ui-corner-all');
                      result = false;
                    }
                    break;
                  case top.Ts.CustomFieldType.Date:
                  case top.Ts.CustomFieldType.Time:
                  case top.Ts.CustomFieldType.DateTime:
                    if ($.trim($(this).find('input').val()) == '') {
                      $(this).addClass('ui-state-error-to-close-custom ui-corner-all');
                      result = false;
                    }
                    break;
                  case top.Ts.CustomFieldType.Number:
                    if ($.trim($(this).find('input').val()) == '') {
                      $(this).addClass('ui-state-error-to-close-custom ui-corner-all');
                      result = false;
                    }
                    break;
                  case top.Ts.CustomFieldType.PickList:
                    if (field.IsFirstIndexSelect == true && $(this).find('select option:selected').index() < 1) {
                      $(this).addClass('ui-state-error-to-close-custom ui-corner-all');
                      result = false;
                    }
                    break;
                  default:
                }
              }
            });
          }
          else {
            $('.ui-state-error-to-close-custom').removeClass('ui-state-error-to-close-custom');
          }
          $('.ticket-widget-customers').removeClass('ui-corner-all ui-state-error');
          if (requireNewTicketCustomer == "True" && $('.newticket-kb').prop('checked') == false) {
            var customerCount = $('.ticket-customer-company').length + $('.ticket-customer-contact').length;
            if (customerCount < 1) {
              $('.ticket-widget-customers').addClass('ui-corner-all ui-state-error');
              result = false;
            }
          }

          callback(result);
        });
      });
    });
  }

  function setInitialValue() {
    var menuID = top.Ts.MainPage.MainMenu.getSelected().getId().toLowerCase();
    switch (menuID) {
      case 'mniusers':
        //top.Ts.Services.Settings.ReadUserSetting('SelectedUserID', -1, function (result) {
        //  if (result > -1) $('.newticket-user').combobox('setValue', result);
        //});
        break;
      case 'mniproducts':
        top.Ts.Services.Settings.ReadUserSetting('SelectedProductID', -1, function (productID) {
          if (productID > -1) {
            $('.newticket-product').combobox('setValue', productID);
            loadVersions();
            top.Ts.Services.Settings.ReadUserSetting('SelectedProductVersionID', -1, function (versionID) {
              if (versionID > -1) $('.newticket-reported').combobox('setValue', versionID);
            });
            top.Ts.Services.CustomFields.GetProductMatchingCustomFields(top.Ts.ReferenceTypes.Tickets, _lastTicketTypeID, productID, function (result) {
                var container = $('.custom-fields-container');
                for (var i = 0; i < result.length; i++) {
                    if (!result[i].CustomFieldCategoryID) {
                        try {
                            switch (result[i].FieldType) {
                                case top.Ts.CustomFieldType.Text: appendCustomEdit(result[i], container, null, 'productChild'); break;
                                case top.Ts.CustomFieldType.Date: appendCustomEditDate(result[i], container, null, 'productChild'); break;
                                case top.Ts.CustomFieldType.Time: appendCustomEditTime(result[i], container, null, 'productChild'); break;
                                case top.Ts.CustomFieldType.DateTime: appendCustomEditDateTime(result[i], container, null, 'productChild'); break;
                                case top.Ts.CustomFieldType.Boolean: appendCustomEditBool(result[i], container, null, 'productChild'); break;
                                case top.Ts.CustomFieldType.Number: appendCustomEditNumber(result[i], container, null, 'productChild'); break;
                                case top.Ts.CustomFieldType.PickList: appendCustomEditCombo(result[i], container, null, 'productChild'); break;
                                default:
                            }
                        } catch (err) {
                            var errorString = '1001 NewTicket.js createCustomFields   FieldType: ' + result[i].FieldType + '  CustomFieldID: ' + result[i].CustomFieldID + ' ::  Exception Properties: ';
                            for (var property in err) { errorString += property + ': ' + err[property] + '; '; }
                            top.Ts.Services.System.LogException(err.message, errorString);
                            $('.newticket-custom-field').remove();
                            return;
                        }
                    }
                }
                container.find('.ui-helper-clearfix').remove();
                $('<div>').addClass('ui-helper-clearfix').appendTo(container);
                appendCategorizedCustomFields(result, 'productChild');
            });
        }
        });
        break;
      case 'mnicustomers':
        top.Ts.Services.Settings.ReadUserSetting('SelectedOrganizationID', -1, function (organizationID) {
          if (organizationID > -1) {
            top.Ts.Services.Settings.ReadUserSetting('SelectedContactID', -1, function (contactID) {
              if (contactID > -1) {
                top.Ts.Services.Tickets.GetTicketCustomer('u', contactID, function (result) {
                  if (!result.Flag) {
                    appendCustomer(result);
                    top.Ts.Services.Customers.LoadAlert(result.UserID, top.Ts.ReferenceTypes.Users, function (note) {
                      if (note != null) {
                        $('#modalAlertMessage').html(note.Description);
                        $('#alertID').val(note.RefID);
                        $('#alertType').val(note.RefType);
                        var buttons = {
                            "Close": function () {
                                $(this).dialog("close");
                            },
                            "Snooze": function () {
                                top.Ts.Services.Customers.SnoozeAlert($('#alertID').val(), $('#alertType').val());
                                $(this).dialog("close");
                            }
                        }

                        if (!top.Ts.System.Organization.HideDismissNonAdmins || top.Ts.System.User.IsSystemAdmin) {
                            buttons["Dismiss"] = function () {
                                top.Ts.Services.Customers.DismissAlert($('#alertID').val(), $('#alertType').val());
                                $(this).dialog("close");
                            }
                        }

                        $("#dialog").dialog({
                          resizable: false,
                          width: 'auto',
                          height: 'auto',
                          create: function () {
                            $(this).css('maxWidth', '800px');
                          },
                          modal: true,
                          buttons: buttons
                        });
                      }
                    });
                  }
                });
              }
              else {
                top.Ts.Services.Tickets.GetTicketCustomer('o', organizationID, function (result) {
                  if (!result.Flag) {
                    appendCustomer(result);
                    top.Ts.Services.Customers.LoadAlert(result.OrganizationID, top.Ts.ReferenceTypes.Organizations, function (note) {
                      if (note != null) {
                        $('#modalAlertMessage').html(note.Description);
                        $('#alertID').val(note.RefID);
                        $('#alertType').val(note.RefType);
                        var buttons = {
                            "Close": function () {
                                $(this).dialog("close");
                            },
                            "Snooze": function () {
                                top.Ts.Services.Customers.SnoozeAlert($('#alertID').val(), $('#alertType').val());
                                $(this).dialog("close");
                            }
                        }

                        if (!top.Ts.System.Organization.HideDismissNonAdmins || top.Ts.System.User.IsSystemAdmin) {
                            buttons["Dismiss"] = function () {
                                top.Ts.Services.Customers.DismissAlert($('#alertID').val(), $('#alertType').val());
                                $(this).dialog("close");
                            }
                        }

                        $("#dialog").dialog({
                          resizable: false,
                          width: 'auto',
                          height: 'auto',
                          create: function () {
                            $(this).css('maxWidth', '800px');
                          },
                          modal: true,
                          buttons: buttons
                        });
                      }
                    });
                  }
                });
              }

            });
          }
        });
        break;
      case 'mnikb':
        if (canEdit) {
          $('.newticket-kb').prop('checked', true);
          $('#knowledgeBaseCategoryDiv').show();
        }


        break;
      case 'mniinventory':
        top.Ts.Services.Settings.ReadUserSetting('SelectedAssetID', -1, function (assetID) {
          if (assetID > -1) {
            top.Ts.Services.Assets.GetAsset(assetID, function (asset) {
              appendAsset(asset);
              if (asset.Location == "1") {
                top.Ts.Services.Assets.GetAssetAssignments(assetID, function (assetAssignments) {
                  for (var i = 0; i < assetAssignments.length; i++) {
                    if (assetAssignments[i].RefType == 32) {
                      top.Ts.Services.Tickets.GetTicketCustomer("u", assetAssignments[i].ShippedTo, function (result) {
                        appendCustomer(result);
                      });
                    }
                    else {
                      top.Ts.Services.Tickets.GetTicketCustomer("o", assetAssignments[i].ShippedTo, function (result) {
                        appendCustomer(result);
                      });
                    }
                  }
                });
              }
            });
          }
        });
        break;
      default:
        if (menuID.indexOf('tickettype') > -1) {
          var ticketTypeID = menuID.substr(14, menuID.length - 14);
          $('.newticket-type').combobox('setValue', ticketTypeID);
          setTicketStatus();
          showCustomFields();
          copyCustomFieldValues();
          _lastTicketTypeID = ticketTypeID;
          //setTicketTypeTemplateText();
        }

    }

    var chatID = top.Ts.Utils.getQueryValue('chatid', window)
    if (chatID && chatID != null) {
      top.Ts.Services.Tickets.GetChatCustomer(chatID, function (result) {
        appendCustomer(result);
      });
    }
  }


  $('.file-upload').fileupload({
    namespace: 'new_ticket',
    dropZone: $('.file-upload'),
    add: function (e, data) {
      for (var i = 0; i < data.files.length; i++) {
        var item = $('<li>')
          .appendTo($('.upload-queue'));

        data.context = item;
        item.data('data', data);

        var bg = $('<div>')
          .addClass('ts-color-bg-accent ui-corner-all')
          .appendTo(item);

        $('<div>')
          .text(data.files[i].name + '  (' + top.Ts.Utils.getSizeString(data.files[i].size) + ')')
          .addClass('filename')
          .appendTo(bg);

        $('<div>')
          .addClass('progress')
          .hide()
          .appendTo(bg);

        $('<span>')
          .addClass('ui-icon ui-icon-close')
          .click(function (e) {
            e.preventDefault();
            $(this).closest('li').fadeOut(500, function () { $(this).remove(); });
          })
          .appendTo(bg);

        $('<span>')
          .addClass('ui-icon ui-icon-cancel')
          .hide()
          .click(function (e) {
            e.preventDefault();
            var data = $(this).closest('li').data('data');
            data.jqXHR.abort();
          })
          .appendTo(bg);
      }

    },
    send: function (e, data) {
      if (data.context && data.dataType && data.dataType.substr(0, 6) === 'iframe') {
        data.context.find('.progress').progressbar('value', 50);
      }
    },
    fail: function (e, data) {
      if (data.errorThrown === 'abort') return;
      alert('There was an error uploading "' + data.files[0].name + '".');
    },
    progress: function (e, data) {
      data.context.find('.progress').progressbar('value', parseInt(data.loaded / data.total * 100, 10));
    },
    start: function (e, data) {
      $('.progress').progressbar().show();
      $('.upload-queue .ui-icon-close').hide();
      $('.upload-queue .ui-icon-cancel').show();
    },
    stop: function (e, data) {
      $('.progress').progressbar('value', 100);

      if (_doClose != true) top.Ts.MainPage.openTicketByID(_ticketID);
      top.Ts.MainPage.closeNewTicketTab();
    }
  });

  $('.newticket-cancel').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    if (confirm('Are you sure you would like to cancel this ticket?')) {
        clearTimeout(_timerid);
        _timerElapsed = 0;
        counter = 0;
        top.Ts.MainPage.closeNewTicketTab();
    }
    top.Ts.System.logAction('New Ticket - Canceled');

    //window.location = window.location;
  });

  $('.newticket-save-close').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    if ($("#recorder").length == 0) {
      _doClose = true;
      $('.newticket-save').click();
      top.Ts.System.logAction('New Ticket - Save & Closed');
    }
  });


  $('.newticket-save').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    if ($("#recorder").length == 0) {
      $('.new-ticket-save-buttons').addClass('saving');
      isFormValid(function (isValid) {
        if (isValid == false) {
          $('.new-ticket-save-buttons').removeClass('saving');
          return;
        }
        //setTimeout(function () { $('.new-ticket-save-buttons').removeClass('saving'); }, 2000); return;

        var info = new Object();
        info.Name = $('.newticket-name').val();
        info.TicketTypeID = $('.newticket-type').val();
        info.TicketStatusID = $('.newticket-status').val();
        info.TicketSeverityID = $('.newticket-severity').val();
        info.UserID = $('.newticket-user').val();
        info.GroupID = $('.newticket-group').val();
        var dueDate = $('.ticket-action-form-dueDate').datetimepicker('getDate');
        if (dueDate != null) {

          if (dateFormat.indexOf("m") != 0) {
            var dateArr = $('.ticket-action-form-dueDate').val().replace(/\./g, '/').replace(/-/g, '/').split('/');
            if (dateFormat.indexOf("d") == 0)
              var day = dateArr[0];
            if (dateFormat.indexOf("y") == 0)
              var year = dateArr[0];
            if (dateFormat.indexOf("m") == 3)
              var month = dateArr[1];

            var timeSplit = dateArr[2].split(' ');
            if (dateFormat.indexOf("y") == 6)
              var year = timeSplit[0];
            else
              var day = timeSplit[0];

            var theTime = timeSplit[1];

            var formattedDate = month + "/" + day + "/" + year + " " + theTime;
            info.DueDate = top.Ts.Utils.getMsDate(formattedDate);
          }
          else
            info.DueDate = top.Ts.Utils.getMsDate(dueDate);
        }
        info.CategoryID = $('.newticket-community').val();
        info.ProductID = $('.newticket-product').val();
        info.ReportedID = $('.newticket-reported').val();
        info.ResolvedID = $('.newticket-resolved').val();
        info.IsVisibleOnPortal = $('.newticket-portal').prop('checked')
        info.IsKnowledgebase = $('.newticket-kb').prop('checked');
        info.KnowledgeBaseCategoryID = $('.newticket-kbCategory').val();
        info.Description = $('.newticket-desc').html();

        info.TimeSpent = parseInt($('.ticket-action-form-hours').val()) * 60 + parseInt($('.ticket-action-form-minutes').val());
        info.DateStarted = top.Ts.Utils.getMsDate($('.ticket-action-form-date').datetimepicker('getDate'));


        // Custom Values
        info.Fields = new Array();
        $('.newticket-custom-field:visible').each(function () {
          var field = new Object();
          field.CustomFieldID = $(this).data('o').CustomFieldID;
          switch ($(this).data('o').FieldType) {
            case top.Ts.CustomFieldType.Boolean:
              field.Value = $(this).find('input').prop('checked');
              break;
            case top.Ts.CustomFieldType.PickList:
              field.Value = $(this).find('select').val();
              break;
            case top.Ts.CustomFieldType.Date:
              var dt = $(this).find('input').datepicker('getDate');
              field.Value = dt == null ? null : dt.toUTCString();
              break;
            case top.Ts.CustomFieldType.Time:
              var time = new Date("January 1, 1970 00:00:00");
              time.setHours($(this).find('input').timepicker('getDate')[0].value.substring(0, 2));
              time.setMinutes($(this).find('input').timepicker('getDate')[0].value.substring(3, 5));
              field.Value = $(this).find('input').timepicker('getDate')[0].value == '' ? null : time.toUTCString();
              break;
            case top.Ts.CustomFieldType.DateTime:
              //field.Value = top.Ts.Utils.getMsDate($(this).find('input').datetimepicker('getDate'));
              var dt = $(this).find('input').datetimepicker('getDate');
              field.Value = dt == null ? null : dt.toUTCString();
              //field.Value = $(this).find('input').datetimepicker('getDate');
              break;
            default:
              field.Value = $(this).find('input').val();
          }
          info.Fields[info.Fields.length] = field;
        });

        // Parent Ticket
        var parentTicket = $('#divRelated .ts-icon-ticket-parent').closest('.ticket-related').data('data');
        if (parentTicket && parentTicket != null) info.ParentTicketID = parentTicket.TicketID;

        // Child Tickets
        info.ChildTickets = new Array();
        $('#divRelated .ts-icon-ticket-child').each(function () {
          info.ChildTickets[info.ChildTickets.length] = $(this).closest('.ticket-related').data('data').TicketID;
        });

        // Related Tickets
        info.RelatedTickets = new Array();
        $('#divRelated .ts-icon-ticket-related').each(function () {
          info.RelatedTickets[info.RelatedTickets.length] = $(this).closest('.ticket-related').data('data').TicketID;
        });

        info.Tags = new Array();
        $('.ticket-tag').each(function () {
          info.Tags[info.Tags.length] = $(this).data('data');
        });

        info.Subscribers = new Array();
        $('.ticket-subscriber').each(function () {
          info.Subscribers[info.Subscribers.length] = $(this).data('data').UserID;
        });

        info.Reminders = new Array();
        $('.ticket-reminder').each(function () {
          info.Reminders[info.Reminders.length] = $(this).data('o');
        });


        info.Queuers = new Array();
        $('.ticket-queue').each(function () {
          info.Queuers[info.Queuers.length] = $(this).data('data').UserID;
        });

        info.Assets = new Array();
        $('.ticket-asset').each(function () {
          info.Assets[info.Assets.length] = $(this).data('data').AssetID;
        });

        info.Customers = new Array();
        $('.ticket-customer-company').each(function () {
          info.Customers[info.Customers.length] = $(this).data('data').OrganizationID;
        });

        info.Contacts = new Array();
        $('.ticket-customer-contact').each(function () {
          info.Contacts[info.Contacts.length] = $(this).data('data').UserID;
        });

        var chatID = top.Ts.Utils.getQueryValue('chatid', window)
        if (chatID && chatID != null) {
          info.ChatID = chatID;
        }

        top.Ts.Services.Tickets.NewTicket(top.JSON.stringify(info), function (result) {

          if (result == null) {
            alert('There was an error saving your ticket.  Please try again.');
            $('.new-ticket-save-buttons').removeClass('saving');
            return;
          }
          _ticketID = result[0];
          top.Ts.System.logAction('Ticket Created');

          if ($('.upload-queue li').length > 0) {
            $('.upload-queue li').each(function (i, o) {
              var data = $(o).data('data');
              data.url = '../../../Upload/Actions/' + result[1];
              data.jqXHR = data.submit();
              $(o).data('data', data);
            });
          }
          else {
            if (_doClose != true) top.Ts.MainPage.openTicketByID(result[0]);
            top.Ts.MainPage.closeNewTicketTab();
          }

        }, function () {
          alert('There was an error saving your ticket.  Please try again.');
          $('.new-ticket-save-buttons').removeClass('saving');
        })
      })
    }
  });

  $('.timerbutton').click(function (e) {
      var togtext = $('.timerlabel').text() == 'Start Timing' ? 'Stop Timing' : 'Start Timing';

      if (togtext == "Stop Timing") {
          start = new Date().getTime()
          tickettimer();
      }
      else {
          clearTimeout(_timerid);
      }

      $('.timerlabel').text(togtext);
      $('.timerlabel').toggleClass('redText');

  }).attr('title', 'Start the Timer');


  $('.ticket-rail a').addClass('ui-state-default ts-link');

  $('#rcdtok').click(function (e) {
      top.Ts.System.logAction('New Ticket - Video Recording Start Clicked');
      top.Ts.Services.Tickets.StartArchiving(sessionId, function (resultID) {
          $('#rcdtok').hide();
          //element.find('#rcdtok span').text('Re-Record');
          $('#stoptok').show();
          $('#inserttok').hide();
          $('#deletetok').hide();
          recordingID = resultID;
          $('#statusText').text("Currently Recording ...");
      });
  });

  $('#stoptok').hide();

  $('#stoptok').click(function (e) {
      top.Ts.System.logAction('New Ticket - Video Recording Stop Clicked');
      top.Ts.Services.Tickets.StopArchiving(recordingID, function (resultID) {
          $('#rcdtok').show();
          $('#stoptok').hide();
          $('#inserttok').show();
          $('#canceltok').show();
          tokurl = "https://s3.amazonaws.com/teamsupportvideos/45228242/" + resultID + "/archive.mp4";
          $('#statusText').text("Recording Stopped");
      });
  });

  $('#inserttok').hide();

  $('#inserttok').click(function (e) {
      top.Ts.System.logAction('New Ticket - Video Recording Insert Clicked');
      tinyMCE.activeEditor.execCommand('mceInsertContent', false, '<br/><br/><video width="400" height="400" controls poster="' + top.Ts.System.AppDomain + '"/dc/1078/images/static/videoview1.jpg"><source src="' + tokurl + '" type="video/mp4"><a href="' + tokurl + '">Please click here to view the video.</a></video>');
      $('#rcdtok').show();
      $('#stoptok').hide();
      $('#inserttok').hide();
      session.unpublish(publisher);
      $('#recordVideoContainer').hide();
      $('#statusText').text("");
  });

  $('#canceltok').click(function (e) {
      top.Ts.System.logAction('New Ticket - Video Recording Cancel Clicked');
      if (recordingID) {
          top.Ts.Services.Tickets.DeleteArchive(recordingID, function (resultID) {
              $('#rcdtok').show();
              $('#stoptok').hide();
              $('#inserttok').hide();
              session.unpublish(publisher);
              $('#recordVideoContainer').hide();
          });
      }
      else {
          session.unpublish(publisher);
          $('#recordVideoContainer').hide();
      }
      $('#statusText').text("");
  });
  $('#recordVideoContainer').hide();


});

function GetTinyMCEFontSize(fontSize) {
    var result = '';
    switch (fontSize) {
        case 1:
            result = "8px";
            break;
        case 2:
            result = "10px";
            break;
        case 3:
            result = "12px";
            break;
        case 4:
            result = "14px";
            break;
        case 5:
            result = "18px";
            break;
        case 6:
            result = "24px";
            break;
        case 7:
            result = "36px";
            break;
    }
    return result;
}

function GetTinyMCEFontName(fontFamily) {
  var result = '';
  switch (fontFamily) {
    case 1:
      result = "'andale mono', times";
      break;
    case 2:
      result = "arial, helvetica, sans-serif";
      break;
    case 3:
      result = "'arial black', 'avant garde'";
      break;
    case 4:
      result = "'book antiqua', palatino";
      break;
    case 5:
      result = "'comic sans ms', sans-serif";
      break;
    case 6:
      result = "'courier new', courier";
      break;
    case 7:
      result = "georgia, palatino";
      break;
    case 8:
      result = "helvetica";
      break;
    case 9:
      result = "impact, chicago";
      break;
    case 10:
      result = "symbol";
      break;
    case 11:
      result = "tahoma, arial, helvetica, sans-serif";
      break;
    case 12:
      result = "terminal, monaco";
      break;
    case 13:
      result = "'times new roman', times";
      break;
    case 14:
      result = "'trebuchet ms', geneva";
      break;
    case 15:
      result = "verdana, geneva";
      break;
    case 16:
      result = "webdings";
      break;
    case 17:
      result = "wingdings, 'zapf dingbats'";
      break;
  }
  return result;
}

var onScreenRecordStart = function () {
  $('.fa-circle-o-notch').removeClass("fa-circle-o-notch fa-spin").addClass("fa-circle");
  switch (BrowserDetect.browser) {
    case "Chrome":
      top.Ts.Services.Settings.WriteUserSetting('ReadScreenRecordingChromeInfo', 1);
      break;
    case "Firefox":
      top.Ts.Services.Settings.WriteUserSetting('ReadScreenRecordingFirefoxInfo', 1);
      break;
    case "Explorer":
      top.Ts.Services.Settings.WriteUserSetting('ReadScreenRecordingExplorerInfo', 1);
      break;
    case "Safari":
      if (BrowserDetect.OS == "Windows") {
        top.Ts.Services.Settings.WriteUserSetting('ReadScreenRecordingSafariInWindowsInfo', 1);
      }
      else {
        top.Services.Settings.WriteUserSetting('ReadScreenRecordingSafariInfo', 1);
      }
      break;
    default:
      top.Ts.Services.Settings.WriteUserSetting('ReadScreenRecordingInfo', 1);
  }
  $('.divScreenRecorderMessages').hide();
};

var onScreenRecordComplete = function (url) {
  $('#recorder').remove();
  if (url) {
    var link = '<a href="' + url + '" target="_blank">Click here to view screen recording video.</a>';
    var html = '<div class="video_holder"><video style="width: 640px; height: 360px;" controls="controls"><source src="' + url + '" type="video/mp4" />' + link + '</video></div>'
    var ed = tinyMCE.activeEditor;
    ed.selection.setContent(html);
    ed.execCommand('mceAutoResize');
    ed.focus();
    top.Ts.System.logAction('Ticket - Screen Recorded');
  }
  else {
    top.Ts.System.logAction('New Ticket - Screen Record Cancelled');
  }
};


