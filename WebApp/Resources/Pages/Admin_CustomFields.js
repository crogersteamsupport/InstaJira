/// <reference path="ts/ts.js" />
/// <reference path="ts/window.parent.parent.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

var adminCustomFields = null;
$(document).ready(function () {
  adminCustomFields = new AdminCustomFields();
  adminCustomFields.refresh();
});

function onShow() {
  adminCustomFields.refresh();
};


AdminCustomFields = function () {
  $('#btnRefresh').click(function (e) {
    e.preventDefault();
    window.location = window.location;
  });

  $('button').button();


  function loadFieldTypes() {
    var select = $('.admin-cf-type');
    $('<option>').text('Ticket').attr('value', 17).appendTo(select);
    $('<option>').text('User').attr('value', 22).appendTo(select);

    if (window.parent.parent.Ts.System.Organization.ProductType != window.parent.parent.Ts.ProductType.Express) {

      if (window.parent.parent.Ts.System.Organization.ProductType != window.parent.parent.Ts.ProductType.HelpDesk) {
        $('<option>').text('Product').attr('value', 13).appendTo(select);
        $('<option>').text('Product Version').attr('value', 14).appendTo(select);
        if (window.parent.parent.Ts.System.Organization.UseProductFamilies) {
          $('<option>').text('Product Line').attr('value', 44).appendTo(select);
        }
        $('<option>').text('Asset').attr('value', 34).appendTo(select);
      }
      $('<option>').text('Customer').attr('value', 9).appendTo(select);

      if (window.parent.parent.Ts.System.Organization.ProductType != window.parent.parent.Ts.ProductType.HelpDesk) {
        $('<option>').text('Customer Product').attr('value', 8).appendTo(select);
        $('<option>').text('Contact Product').attr('value', 46).appendTo(select);
      }
      $('<option>').text('Customer Contact').attr('value', 32).appendTo(select);
    }

    select.combobox({ selected: function (e, ui) {
      $('.admin-cf-tickettype').parent().toggle($(ui.item).val() == 17);
      loadData();
    }
    });
    select.combobox('setValue', 17);

  }

  function loadTicketTypes() {
    var ticketTypes = window.parent.parent.Ts.Cache.getTicketTypes();
    var select = $('.admin-cf-tickettype')
    select.find('option').remove();

    for (var i = 0; i < ticketTypes.length; i++) {
      $('<option>')
        .text(ticketTypes[i].Name)
        .data('TicketType', ticketTypes[i])
        .attr('value', ticketTypes[i].TicketTypeID)
        .appendTo(select);
    }
    select.combobox({ selected: function (e, ui) {
      loadData();
    }
    });


  }

  function loadData() {
    $('.admin-cf-content').hide().empty().prev().show();

    var refType = $('.admin-cf-type').val();
    var auxID = refType == 17 ? $('.admin-cf-tickettype').val() : null;
    window.parent.parent.Ts.Services.CustomFields.GetCustomFields(refType, auxID, function (fields) {
      window.parent.parent.Ts.Services.CustomFields.GetCategories(refType, auxID, function (cats) {
        var div = $('<div>')
            .addClass('ui-widget-content ui-corner-all ts-section admin-cf-cat admin-cf-nocat')
            .data('Category', null)
            .appendTo('.admin-cf-content');

        appendCustomFields(div, fields, null);
        $('<a>')
            .text('Add Custom Field')
            .attr('href', '#')
            .addClass('ts-link ui-state-default')
            .click(function (e) {
              e.preventDefault();
              showFieldDialog(null, null);
            })
            .appendTo(div);

        if (refType == 9 || refType == 17 || refType == 32) {
          var addCat = $('<a>')
            .text('Add Category')
            .attr('href', '#')
            .addClass('ts-link ui-state-default admin-cf-add-cat')
            .click(function (e) {
              e.preventDefault();
              appendCat(null, null);
            });

          $('<div>')
          .addClass('ts-section')
          .append(addCat)
          .appendTo('.admin-cf-content');
        }

        for (var i = 0; i < cats.length; i++) {
          appendCat(cats[i], fields);
        }


        $('.admin-cf-content').show().prev().hide();

        setSortable();
      })
    });
  }

  function setSortable() {
    $('.admin-cf-fields').sortable({ items: '.admin-cf-field', connectWith: '.admin-cf-fields', placeholder: 'ui-state-highlight admin-cf-field ui-corner-all', update: function (e, ui) {
      savePositions();
    }
    });

    $('.admin-cf-content').sortable({ items: '.admin-cf-cat:not(.admin-cf-nocat)', placeholder: 'ui-state-highlight ui-widget-content ui-corner-all ts-section admin-cf-cat', update: function (e, ui) {
      savePositions();
    }
    });
  }

  loadFieldTypes();
  loadTicketTypes();
  loadData();
  function savePositions() {
    var orders = new parent.parent.Array();
    $('.admin-cf-cat').each(function () {
      var item = new parent.parent.TSWebServices.CategoryOrder();
      item.FieldIDs = new parent.parent.Array();
      orders[orders.length] = item;
      var cat = $(this).data('Category');
      item.CatID = (cat === null ? null : cat.CustomFieldCategoryID);
      $(this).find('.admin-cf-field').each(function () {
        var field = $(this).data('CustomField');
        item.FieldIDs[item.FieldIDs.length] = field.CustomFieldID

      });
    });
    window.parent.parent.Ts.System.logAction('Admin Custom Fields - Field Order Saved');
    window.parent.parent.Ts.Services.CustomFields.SaveOrder(JSON.stringify(orders));

  }

  function showFieldDialog(customFieldID, catID) {
    var refType = $('.admin-cf-type').val();
    var auxID = refType == 17 ? $('.admin-cf-tickettype').val() : null;

    var wnd = parent.parent.GetCustomFieldDialog(customFieldID, refType, auxID, catID);
    wnd.add_close(fieldDialogClosed);
    wnd.show();
    window.parent.parent.Ts.System.logAction('Admin Custom Fields - Field Dialog Opened');
    function fieldDialogClosed(sender, args) {
      sender.remove_close(fieldDialogClosed);
      loadData();

    }
  }

  function appendCat(category, fields) {
    var div = $('<div>')
          .addClass('ui-widget-content ui-corner-all ts-section admin-cf-cat')
          .data('Category', category)
          .insertBefore($('.admin-cf-add-cat').parent());

    var header = $('<div>')
      .addClass('admin-cf-cat-header')
      .hover(function () {
        $(this).find('.ts-icon').show();
      }, function () {
        $(this).find('.ts-icon').hide();
      })
      .appendTo(div);

    $('<span>')
          .text((category === null ? '' : category.Category))
          .addClass('caption')
          .appendTo(header);

    if (window.parent.parent.Ts.System.Organization.UseProductFamilies && ($('.admin-cf-type').val() == 9 || $('.admin-cf-type').val() == 32)) {
        var catProductFamilyName = '';
        if (category && category.ProductFamilyID) {
            catProductFamilyName = ' (' + category.ProductFamilyName + ')';
        }

        $('<span>')
              .text(catProductFamilyName)
              .addClass('productFamilyCaption')
              .appendTo(header);
    }

    $('<span>')
          .addClass('ts-icon ts-icon-edit')
          .click(function (e) {
            editCat($(this).closest('.admin-cf-cat'));
          })
          .appendTo(header).hide();

    $('<span>')
          .addClass('ts-icon ts-icon-delete fa fa-trash')
          .click(function (e) {
            if (confirm('Are you sure you would like to delete this category?  Your existing custom fields will not removed.')) {
              window.parent.parent.Ts.Services.CustomFields.DeleteCategory($(this).closest('.admin-cf-cat').data('Category').CustomFieldCategoryID, function () {
                window.parent.parent.Ts.System.logAction('Admin Custom Fields - Category Deleted');

                loadData();
              });
            }
          })
          .appendTo(header).hide();

    $('<div>').css('clear', 'both').appendTo(header);

    $('<div>').addClass('ts-separator ui-widget-content').appendTo(div);

    appendCustomFields(div, fields, (category === null ? null : category.CustomFieldCategoryID));

    $('<a>')
          .text('Add Custom Field')
          .attr('href', '#')
          .addClass('ts-link ui-state-default')
          .click(function (e) {
            e.preventDefault();
            showFieldDialog(null, $(this).closest('.admin-cf-cat').data('Category').CustomFieldCategoryID);
          })
          .appendTo(div);

    if (category === null) {
      div.find('.ts-icon-edit').click();
    }
    setSortable();
  }

  function appendCustomFields(element, fields, catID) {
    var fieldsDiv = $('<div>')
      .addClass('admin-cf-fields')
      .appendTo(element);

    if (fields == null) return;

    for (var i = 0; i < fields.length; i++) {
      var field = fields[i];

      if (catID != null) {
        if (field.CustomFieldCategoryID != null) {
          if (catID != field.CustomFieldCategoryID) continue;
        }
        else {
          continue;
        }
      }
      else {
        if (field.CustomFieldCategoryID != null) continue;
      }

      var typeName = '';
      switch (field.FieldType) {
        case 0: typeName = 'Text'; break;
        case 1: typeName = 'Date and Time'; break;
        case 2: typeName = 'True or False'; break;
        case 3: typeName = 'Number'; break;
        case 4: typeName = 'Pick List'; break;
        case 5: typeName = 'Date'; break;
        case 6: typeName = 'Time'; break;
        default: typeName = 'Unknown';
      }

      if (field.IsRequired) typeName = typeName + ', Required';
      if (field.IsRequiredToClose) typeName = typeName + ', Required to Close';
      if (field.IsVisibleOnPortal) typeName = typeName + ', Visible to Customers';
      if (field.ParentCustomFieldID) typeName = typeName + ', Related to ' + field.ParentFieldName + ' and value ' + field.ParentCustomValue;
      if (field.ParentProductID) typeName = typeName + ', Related to ' + field.ParentProductName;
      typeName = '(' + typeName + ')';

      var fieldDiv = $('<div>')
        .addClass('admin-cf-field')
        .data('CustomField', field)
        .hover(function () {
          $(this).find('.ts-icon').show();
        }, function () {
          $(this).find('.ts-icon').hide();
        })
        .appendTo(fieldsDiv);
      $('<span>')
        .addClass('admin-cf-field-name')
        .text(field.Name)
        .appendTo(fieldDiv);
      $('<span>')
        .addClass('admin-cf-field-type')
        .text(typeName)
        .appendTo(fieldDiv);
      $('<span>')
        .addClass('ts-icon ts-icon-edit')
        .click(function (e) {
          e.preventDefault();
          var cat = $(this).closest('.admin-cf-cat').data('Category');
          if (cat) {
            showFieldDialog($(this).closest('.admin-cf-field').data('CustomField').CustomFieldID, cat.CustomFieldCategoryID);
          }
          else {
            showFieldDialog($(this).closest('.admin-cf-field').data('CustomField').CustomFieldID);
          }
        })
        .appendTo(fieldDiv).hide();
      $('<span>')
        .addClass('ts-icon ts-icon-delete fa fa-trash')
        .click(function (e) {
          e.preventDefault();
          var parent = $(this).closest('.admin-cf-field');
          if (!confirm('WARNING: Are you sure you would like to delete this custom field.  You will lose ALL data associated with this custom field.')) return;
          window.parent.parent.Ts.Services.CustomFields.DeleteCustomField($(this).closest('.admin-cf-field').data('CustomField').CustomFieldID, function (result) {
            window.parent.parent.Ts.System.logAction('Admin Custom Fields - Field Deleted');

            parent.remove();
          });
        }).hide()
        .appendTo(fieldDiv);
      $('<div>').css('clear', 'both').appendTo(fieldDiv);
    }

  }

  function editCat(element) {
    element = $(element);

    var header = element.find('.admin-cf-cat-header').hide();
    var cat = element.closest('.admin-cf-cat').data('Category');
    var div = $('<div>')
      .addClass('admin-cf-cat-edit')
      .insertAfter(header)

    var catNameDiv = $('<div>')
      .addClass('label-block')
      .appendTo(div);

    $('<span>')
      .addClass('label')
      .text('Category Name')
      .appendTo(catNameDiv);

    $('<input>')
      .addClass('ui-widget-content ui-corner-all text')
      .appendTo(catNameDiv)
      .val(header.find('.caption').text());

    if (window.parent.parent.Ts.System.Organization.UseProductFamilies && ($('.admin-cf-type').val() == 9 || $('.admin-cf-type').val() == 32)) {
        var catProductFamilyDiv = $('<div>')
            .addClass('label-block')
            .appendTo(div);

        $('<span>')
            .addClass('label')
            .text('Product Line')
            .appendTo(catProductFamilyDiv);

        var catProductFamilySelect = $('<select>')
            .addClass('admin-cf-cat-productfamily')
            .appendTo(catProductFamilyDiv);

        //build product families ddl dinamycaly
        $('<option>')
            .text('Unassigned')
            .attr('value', -1)
            .appendTo(catProductFamilySelect);

        var productFamilies = window.parent.parent.Ts.Cache.getProductFamilies();
        for (var i = 0; i < productFamilies.length; i++) {
            $('<option>')
              .text(productFamilies[i].Name)
              .data('ProductFamily', productFamilies[i])
              .attr('value', productFamilies[i].ProductFamilyID)
              .appendTo(catProductFamilySelect);
        }

        if (cat && cat.ProductFamilyID)
        {
            catProductFamilySelect.val(cat.ProductFamilyID);
        }
    }

    $('<span>')
    .addClass('ts-icon ts-icon-save')
    .click(function (e) {
      e.preventDefault();

      var catName = '';
      var productFamilyID = -1;
      var productFamilyName = '';

      if (window.parent.parent.Ts.System.Organization.UseProductFamilies && ($('.admin-cf-type').val() == 9 || $('.admin-cf-type').val() == 32)) {
          catName = $(this).prev().prev().find('.text').val();
          productFamilyID = $(this).prev().find('.admin-cf-cat-productfamily').val();
          productFamilyName = ' (' + $(this).prev().find('.admin-cf-cat-productfamily option:selected').text() + ')';
      }
      else {
          catName = $(this).prev().find('.text').val();
      }

      if ($.trim(catName) === '') {
        alert('Please enter a name for the category.');
        return;
      }
      if (cat === null) {
          window.parent.parent.Ts.Services.CustomFields.NewCategory($('.admin-cf-type').val(), ($('.admin-cf-type').val() == 17 ? $('.admin-cf-tickettype').val() : null), catName, productFamilyID, function (result) {
          element.closest('.admin-cf-cat').data('Category', result);
          window.parent.parent.Ts.System.logAction('Admin Custom Fields - Category Created');
        });
      }
      else {
        window.parent.parent.Ts.Services.CustomFields.SaveCategory(cat.CustomFieldCategoryID, catName, productFamilyID);
        window.parent.parent.Ts.System.logAction('Admin Custom Fields - Category Edited');
      }

      header.show().find('.caption').text(catName);
      header.find('.productFamilyCaption').text(productFamilyName);
      $(this).closest('.admin-cf-cat-edit').remove();
    })
    .appendTo(div);

    $('<span>')
    .addClass('ts-icon ts-icon-cancel')
    .click(function (e) {
      e.preventDefault();
      if (cat === null) {
        element.closest('.admin-cf-cat').remove();
      }
      else {
        $(this).closest('.admin-cf-cat-edit').remove();
        header.show();
      }

    })
    .appendTo(div);
    div.find('input').focus().select();
    $('<div>').css('clear', 'both').appendTo(div);
  }



};


AdminCustomFields.prototype = {
  constructor: AdminCustomFields,
  refresh: function () {
  }
};
