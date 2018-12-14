/// <reference path="ts/ts.js" />
/// <reference path="ts/window.parent.parent.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

var _assetDetailPage = null;
var _assetID = null;
var _execGetCustomer = null;
var _assetAssignments = null;
var _dateFormat;

$(document).ready(function () {
  _assetDetailPage = new AssetDetailPage();
  _assetDetailPage.refresh();
  $('.asset-tooltip').tooltip({ placement: 'bottom', container: 'body' });

  var _isAdmin = window.parent.parent.Ts.System.User.IsSystemAdmin;

  parent.Ts.Services.Customers.GetDateFormat(false, function (dateformat) {
      $('.datepicker').attr("data-format", dateformat);
      $('.datetimepicker').attr("data-format", dateformat + " hh:mm a");
      _dateFormat = dateformat;
      $('.timepicker').datetimepicker({ pickDate: false });
      $('.datetimepicker').datetimepicker({});
      $('.datepicker').datetimepicker({ pickTime: false });
      //$('#inputExpectedRelease').datetimepicker({ pickTime: false, format: dateformat });
  });

  $('body').layout({
    defaults: {
      spacing_open: 0,
      resizable: false,
      closable: false
    },
    north: {
      size: 100,
      spacing_open: 1
    },
    center: {
      maskContents: true,
      size: 'auto'
    }
  });

  _assetID = window.parent.parent.Ts.Utils.getQueryValue("assetid", window);
  var historyLoaded = 0;
  var actionsHistoryLoaded = 0;
  parent.privateServices.SetUserSetting('SelectedAssetID', _assetID);

  LoadProperties();
  LoadCustomProperties();
  LoadFiles();


  if (!window.parent.parent.Ts.System.User.CanEditAsset && !_isAdmin) {
    $('#assetEdit').hide();
  }


  $(".maincontainer").on("keypress", "input", (function (evt) {
    //Deterime where our character code is coming from within the event
    var charCode = evt.charCode || evt.keyCode;
    if (charCode == 13) { //Enter key's keycode
      return false;
    }
  }));

  $('#historyToggle').on('click', function () {
    if (historyLoaded == 0) {
      historyLoaded = 1;
      LoadHistory(1);
    }
  });

  $('#actionsHistoryToggle').on('click', function () {
    if (actionsHistoryLoaded == 0) {
      actionsHistoryLoaded = 1;
      LoadActionsHistory(1);
    }
  });

  $('#historyRefresh').on('click', function () {
    LoadHistory(1);
  });

  $('#actionsHistoryRefresh').on('click', function () {
    LoadActionsHistory(1);
  });

  $('#assetTabs a:first').tab('show');

  $('#assetEdit').click(function (e) {
    $('.assetProperties p').toggleClass("editable");
    $('.customProperties p').toggleClass("editable");
    $(this).toggleClass("btn-primary");
    $(this).toggleClass("btn-success");
    $('#assetTabs a:first').tab('show');
  });

  $('#fieldName').click(function (e) {
    e.preventDefault();
    if (!$(this).hasClass('editable'))
      return false;
    window.parent.parent.Ts.System.logAction('Asset Detail - Name clicked');
    var header = $(this).hide();
    var container = $('<div>')
          .insertAfter(header);

    var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

    $('<input type="text">')
          .addClass('col-xs-10 form-control')
          .val($(this).text())
          .appendTo(container1)
          .focus();

    $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
            $(this).closest('div').remove();
            header.show();
            $('#assetEdit').removeClass("disabled");
            window.parent.parent.Ts.System.logAction('Asset Detail - Change Name cancelled');
          })
          .insertAfter(container1);
    $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
            window.parent.parent.Ts.Services.Assets.SetAssetName(_assetID, $(this).prev().find('input').val(), function (result) {
              header.text(result);
              $('#assetTitle').text(result);
              $('#assetEdit').removeClass("disabled");
              window.parent.parent.Ts.System.logAction('Asset Detail - Name changed');
            },
            function (error) {
              header.show();
              alert('There was an error saving the asset name.');
              $('#assetEdit').removeClass("disabled");
            });
            $('#assetEdit').removeClass("disabled");
            $(this).closest('div').remove();
            header.show();
          })
          .insertAfter(container1);
    $('#assetEdit').addClass("disabled");
  });

  $('#fieldProduct').click(function (e) {
    e.preventDefault();
    if (!$(this).hasClass('editable'))
      return false;
    window.parent.parent.Ts.System.logAction('Asset Detail - Product clicked');
    var header = $(this).hide();

    var container = $('<div>')
        .insertAfter(header);

    var container1 = $('<div>')
          .addClass('col-xs-9')
        .appendTo(container);

    var select = $('<select>').addClass('form-control').attr('id', 'ddlProduct').appendTo(container1);

    var products = window.parent.parent.Ts.Cache.getProducts();

    for (var i = 0; i < products.length; i++) {
      var opt = $('<option>').attr('value', products[i].ProductID).text(products[i].Name).data('o', products[i]);
      if (header.data('productID') == products[i].ProductID)
        opt.attr('selected', 'selected');
      opt.appendTo(select);
    }


    $('<i>')
        .addClass('col-xs-1 fa fa-times')
        .click(function (e) {
          $(this).closest('div').remove();
          header.show();
          $('#assetEdit').removeClass("disabled");
          window.parent.parent.Ts.System.logAction('Asset Detail - Product changed cancelled');
        })
        .insertAfter(container1);
    $('#ddlProduct').on('change', function () {
      var value = $(this).val();
      var name = this.options[this.selectedIndex].innerHTML;
      container.remove();

      window.parent.parent.Ts.Services.Assets.SetAssetProduct(_assetID, value, $('#fieldProduct').text(), name, function (result) {
        header.data('productID', result);
        header.text(name);
        header.show();
        $('#assetEdit').removeClass("disabled");
        window.parent.parent.Ts.System.logAction('Asset Detail - Product changed.');
        var product = window.parent.parent.Ts.Cache.getProduct(result);
        if (product.Versions.length == 0) {
          $('#fieldProductVersion').text('N/A');
          $('#fieldProductVersion').removeClass("editable");
        }
        else {
          $('#fieldProductVersion').text('Unassigned');
          $('#fieldProductVersion').addClass("editable");
        }
      });
    });
    $('#assetEdit').addClass("disabled");
  });

  $('#fieldProductVersion').click(function (e) {
    e.preventDefault();
    if (!$(this).hasClass('editable'))
      return false;
    window.parent.parent.Ts.System.logAction('Asset Detail - Product Version clicked');
    var header = $(this).hide();

    var container = $('<div>')
        .insertAfter(header);

    var container1 = $('<div>')
          .addClass('col-xs-9')
        .appendTo(container);

    var select = $('<select>').addClass('form-control').attr('id', 'ddlProductVersion').appendTo(container1);

    var product = window.parent.parent.Ts.Cache.getProduct($('#fieldProduct').data('productID'));
    for (var i = 0; i < product.Versions.length; i++) {
      var opt = $('<option>').attr('value', product.Versions[i].ProductVersionID).text(product.Versions[i].VersionNumber).data('o', product.Versions[i]);
      if (header.data('productVersionID') == product.Versions[i].ProductVersionID)
        opt.attr('selected', 'selected');
      opt.appendTo(select);
    }

    $('<i>')
        .addClass('col-xs-1 fa fa-times')
        .click(function (e) {
          $(this).closest('div').remove();
          header.show();
          $('#assetEdit').removeClass("disabled");
          window.parent.parent.Ts.System.logAction('Asset Detail - Product Version changed cancelled');
        })
        .insertAfter(container1);

    $('<i>')
      .addClass('col-xs-1 fa fa-check')
      .click(function (e) {
        var value = $('#ddlProductVersion').val();
        var name = $('#ddlProductVersion').text();
        container.remove();

        window.parent.parent.Ts.Services.Assets.SetAssetProductVersion(_assetID, value, $('#fieldProductVersion').text(), name, function (result) {
          header.data('productVersionID', result);
          header.text(name);
          header.show();
          window.parent.parent.Ts.System.logAction('Asset Detail - Product Version changed.');
        },
        function (error) {
          header.show();
          alert('There was an error saving the asset product version.');
        });
        $('#assetEdit').removeClass("disabled");
        $('#ddlProductVersion').closest('div').remove();
        header.show();
      })
      .insertAfter(container1);

    $('#ddlProductVersion').on('change', function () {
      var value = $(this).val();
      var name = this.options[this.selectedIndex].innerHTML;
      container.remove();

      window.parent.parent.Ts.Services.Assets.SetAssetProductVersion(_assetID, value, $('#fieldProductVersion').text(), name, function (result) {
        header.data('productVersionID', result);
        header.text(name);
        header.show();
        $('#assetEdit').removeClass("disabled");
        window.parent.parent.Ts.System.logAction('Asset Detail - Product Version changed.');
      });
    });
    $('#assetEdit').addClass("disabled");
  });

  $('#fieldSerialNumber').click(function (e) {
    e.preventDefault();
    if (!$(this).hasClass('editable'))
      return false;
    window.parent.parent.Ts.System.logAction('Asset Detail - Serial Number clicked');
    var header = $(this).hide();
    var container = $('<div>')
          .insertAfter(header);

    var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

    $('<input type="text">')
          .addClass('col-xs-10 form-control')
          .val($(this).text())
          .appendTo(container1)
          .focus();

    $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
            $(this).closest('div').remove();
            header.show();
            $('#assetEdit').removeClass("disabled");
            window.parent.parent.Ts.System.logAction('Asset Detail - Serial Number change cancelled');
          })
          .insertAfter(container1);
    $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
            window.parent.parent.Ts.Services.Assets.SetAssetSerialNumber(_assetID, $(this).prev().find('input').val(), function (result) {
              header.text(result);
              $('#assetEdit').removeClass("disabled");
              window.parent.parent.Ts.System.logAction('Asset Detail - Serial Number changed');
            },
            function (error) {
              header.show();
              alert('There was an error saving the asset name.');
              $('#assetEdit').removeClass("disabled");
            });
            $('#assetEdit').removeClass("disabled");
            $(this).closest('div').remove();
            header.show();
          })
          .insertAfter(container1);
    $('#assetEdit').addClass("disabled");
  });

  $('#fieldWarrantyExpiration').click(function (e) {
    e.preventDefault();
    if (!$(this).hasClass('editable'))
      return false;
    window.parent.parent.Ts.System.logAction('Asset Detail - Warranty Expiration clicked');
    var parent = $(this).hide();
    var container = $('<div>')
          .insertAfter(parent);

    var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

    var input = $('<input type="text">')
            .addClass('col-xs-10 form-control')
            .val($(this).val())
            .datetimepicker({ pickTime: false })
            .appendTo(container1)
            .focus();

    $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
            $(this).closest('div').remove();
            parent.show();
            $('#assetEdit').removeClass("disabled");
            window.parent.parent.Ts.System.logAction('Asset Detail - Warranty Expiration change cancelled');
          })
          .insertAfter(container1);
    $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
            var value = window.parent.parent.Ts.Utils.getMsDate(input.val());
            container.remove();
            window.parent.parent.Ts.Services.Assets.SetAssetWarrantyExpiration(_assetID, value, function (result) {
              var date = result === null ? null : window.parent.parent.Ts.Utils.getMsDate(result);
              parent.text((date === null ? 'Unassigned' : date.localeFormat(window.parent.parent.Ts.Utils.getDatePattern())))
              $('#assetEdit').removeClass("disabled");
              window.parent.parent.Ts.System.logAction('Asset Detail - Warranty Expiration Change');
            },
            function (error) {
              parent.show();
              alert('There was an error saving the asset Warranty Expiration.');
              $('#assetEdit').removeClass("disabled");
            });
            $('#assetEdit').removeClass("disabled");
            $(this).closest('div').remove();
            parent.show();
          })
          .insertAfter(container1);
    $('#assetEdit').addClass("disabled");
  });

  $('#fieldNotes').click(function (e) {
    e.preventDefault();
    if (!$(this).hasClass('editable'))
      return false;
    window.parent.parent.Ts.System.logAction('Asset Detail - Notes clicked');
    var header = $(this).hide();
    var container = $('<div>')
          .insertAfter(header);

    var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

    $('<textarea>')
          .addClass('col-xs-10 form-control')
          .val($(this).text())
          .appendTo(container1)
          .focus();

    $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
            $(this).closest('div').remove();
            header.show();
            $('#assetEdit').removeClass("disabled");
            window.parent.parent.Ts.System.logAction('Asset Detail - Notes change cancelled');
          })
          .insertAfter(container1);
    $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
            window.parent.parent.Ts.Services.Assets.SetAssetNotes(_assetID, $(this).prev().find('textarea').val(), function (result) {
              header.text(result);
              $('#assetEdit').removeClass("disabled");
              window.parent.parent.Ts.System.logAction('Asset Detail - Notes changed');
            },
            function (error) {
              header.show();
              alert('There was an error saving the asset notes.');
              $('#assetEdit').removeClass("disabled");
            });
            $('#assetEdit').removeClass("disabled");
            $(this).closest('div').remove();
            header.show();
          })
          .insertAfter(container1);
    $('#assetEdit').addClass("disabled");
  });

  window.parent.parent.Ts.Services.Customers.GetDateFormat(false, function (dateformat) {
    $('.datepicker').attr("data-format", dateformat);
    $('.datepicker').datetimepicker({ pickTime: false });

    $('#productExpiration').attr("data-format", dateformat);
    $('.datetimepicker').datetimepicker({});
  });

  $('#assetRefresh').click(function (e) {
    window.location = window.location;
  });

  $('#assetAssign').click(function (e) {
    $('#modalAssignTitle').text('Assign asset');
    $('#btnSaveReturn').hide();
    $('#btnSaveAssign').show();
    $('#comments').attr('placeholder', 'Comments about assigning this asset.');
  });

  $('#returnAsset').click(function (e) {
    $('#modalAssignTitle').text('Return asset');
    $('#btnSaveAssign').hide();
    $('#btnSaveReturn').show();
    $('#inputCustomerDiv').hide();
    $('#comments').attr('placeholder', 'Comments about returning this asset.');
  });

  $('#fileToggle').click(function (e) {
    window.parent.parent.Ts.System.logAction('Asset Detail - Toggle File Form');
    $('#fileForm').toggle();
  });

  //  As below lets do if in Junkyard then set delete button to delete for admins only
  //  if (!_isAdmin)
  //    $('#customerDelete').hide();


  // To implement once we've figure out the two types of asset functionality
  //  $('#customerDelete').click(function (e) {
  //    if (confirm('Are you sure you would like to remove this organization?')) {
  //      parent.privateServices.DeleteOrganization(organizationID, function (e) {
  //        if (window.parent.document.getElementById('iframe-mniCustomers'))
  //          window.parent.document.getElementById('iframe-mniCustomers').contentWindow.refreshPage();
  //        window.parent.parent.Ts.MainPage.closeNewCustomerTab(organizationID);
  //      });
  //    }
  //  });

  $('#btnSaveAssign').click(function (e) {
    if ($('#inputCustomer').data('item') && $('#dateShipped').val()) {
      var refType = 9;
      if ($('#inputCustomer').data('item').data == 'u') {
        refType = 32;
      }

      var assetAssignmentInfo = new Object();

      assetAssignmentInfo.RefID = $('#inputCustomer').data('item').id;
      assetAssignmentInfo.RefType = refType;
      assetAssignmentInfo.DateShipped = $('#dateShipped').val();
      assetAssignmentInfo.TrackingNumber = $('#trackingNumber').val();
      assetAssignmentInfo.ShippingMethod = $('#shippingMethod').val();
      assetAssignmentInfo.ReferenceNumber = $('#referenceNumber').val();
      assetAssignmentInfo.Comments = $('#comments').val();
      assetAssignmentInfo.AssigneeName = $('#inputCustomer').data('item').value;

      window.parent.parent.Ts.Services.Assets.AssignAsset(_assetID, parent.JSON.stringify(assetAssignmentInfo), function (assetID) {
        window.parent.parent.Ts.System.logAction('Asset Assigned');
        $('#modalAssign').modal('hide');
        window.location = window.location;
      }, function () {
        alert('There was an error assigning this asset.  Please try again.');
      });
    }
    else {
      if (!$('#inputCustomer').data('item')) {
        alert("Please select a valid customer or contact to assign this asset to.");
      }
      else {
        alert("Please enter a valid date shipped.");
      }
    }
    //    if ($('#reminderDesc').val() != "" && $('#reminderDate').val() != "") {
    //      window.parent.parent.Ts.Services.System.EditReminder(null, window.parent.parent.Ts.ReferenceTypes.Organizations, organizationID, $('#reminderDesc').val(), window.parent.parent.Ts.Utils.getMsDate($('#reminderDate').val()), $('#reminderUsers').val());
    //      $('#modalReminder').modal('hide');
    //    }
    //    else
    //      alert("Please fill in all the fields");
  });

  $('#btnSaveReturn').click(function (e) {
    if ($('#dateShipped').val()) {
      var confirmed = true;
      if (_assetAssignments && _assetAssignments.length > 1) {
        confirmed = confirm("Returning the asset will unassigned it from all customers. Are you sure you want to proceed?");
      }
      if (confirmed) {
        var assetReturnInfo = new Object();

        assetReturnInfo.DateShipped = $('#dateShipped').val();
        assetReturnInfo.TrackingNumber = $('#trackingNumber').val();
        assetReturnInfo.ShippingMethod = $('#shippingMethod').val();
        assetReturnInfo.ReferenceNumber = $('#referenceNumber').val();
        assetReturnInfo.Comments = $('#comments').val();

        window.parent.parent.Ts.Services.Assets.ReturnAsset(_assetID, parent.JSON.stringify(assetReturnInfo), function (assetID) {
          window.parent.parent.Ts.System.logAction('Asset Returned');
          $('#modalAssign').modal('hide');
          window.location = window.location;
        }, function () {
          alert('There was an error returning this asset.  Please try again.');
        });
      }
      else {
        window.location = window.location;
      }
    }
    else {
      alert("Please enter a valid date shipped.");
    }
  });

  $('#btnJunkSave').click(function (e) {
    window.parent.parent.Ts.Services.Assets.JunkAsset(_assetID, $('#junkingComments').val(), function (assetID) {
      window.parent.parent.Ts.System.logAction('Asset Junked');
      $('#modalJunk').modal('hide');
      window.location = window.location;
    }, function () {
      alert('There was an error assigning this asset to the Junkyard.  Please try again.');
    });
  });

  $('#tblFiles').on('click', '.viewFile', function (e) {
    e.preventDefault();
    window.parent.parent.Ts.MainPage.openNewAttachment($(this).parent().attr('id'));
  });

  $('#tblFiles').on('click', '.delFile', function (e) {
    e.preventDefault();
    e.stopPropagation();
    if (confirm('Are you sure you would like to remove this attachment?')) {
      window.parent.parent.Ts.System.logAction('Asset Detail - Delete File');
      parent.privateServices.DeleteAttachment($(this).parent().parent().attr('id'), function (e) {
        LoadFiles();
      });

    }
  });

  $("#btnFilesCancel").click(function (e) {
    window.parent.parent.Ts.System.logAction('Asset Detail - Cancel File Form');
    $('.upload-queue').empty();
    $('#attachmentDescription').val('');
    $('#fileForm').toggle();
  });

  $('#btnFilesSave').click(function (e) {
    window.parent.parent.Ts.System.logAction('Asset Detail - Save Files');
    if ($('.upload-queue li').length > 0) {
      $('.upload-queue li').each(function (i, o) {
        var data = $(o).data('data');
        data.formData = { description: $('#attachmentDescription').val().replace(/<br\s?\/?>/g, "\n") };
        data.url = '../../../Upload/AssetAttachments/' + _assetID;
        data.jqXHR = data.submit();
        $(o).data('data', data);
      });
    }
    //$('#fileForm').toggle();
  });

  $('#assignedTable > tbody').on('click', '.companylink', function (e) {
    e.preventDefault();

    window.parent.parent.Ts.System.logAction('Asset Detail - View assigned company');
    window.parent.parent.Ts.MainPage.openNewCustomer(this.id);
  });

  $('#assignedTable > tbody').on('click', '.contactlink', function (e) {
    e.preventDefault();

    window.parent.parent.Ts.System.logAction('Asset Detail - View assigned contact');
    window.parent.parent.Ts.MainPage.openNewContact(this.id);
  });

  $('.file-upload').fileupload({
    namespace: 'asset_attachment',
    dropZone: $('.file-upload'),
    add: function (e, data) {
      for (var i = 0; i < data.files.length; i++) {
        var item = $('<li>')
                  .appendTo($('.upload-queue'));

        data.context = item;
        item.data('data', data);

        var bg = $('<div>')
                  .addClass('ts-color-bg-accent')
                  .appendTo(item);

        $('<div>')
                  .text(data.files[i].name + '  (' + window.parent.parent.Ts.Utils.getSizeString(data.files[i].size) + ')')
                  .addClass('filename')
                  .appendTo(bg);

        $('<span>')
                  .addClass('icon-remove')
                  .click(function (e) {
                    e.preventDefault();
                    $(this).closest('li').fadeOut(500, function () { $(this).remove(); });
                  })
                  .appendTo(bg);

        $('<span>')
                  .addClass('icon-remove')
                  .hide()
                  .click(function (e) {
                    e.preventDefault();
                    var data = $(this).closest('li').data('data');
                    data.jqXHR.abort();
                  })
                  .appendTo(bg);

        var progress = $('<div>')
                  .addClass('progress progress-striped active')
                  .hide();

        $('<div>')
                    .addClass('progress-bar')
                    .attr('role', 'progressbar')
                    .appendTo(progress);

        progress.appendTo(bg);
      }

    },
    send: function (e, data) {
      if (data.context && data.dataType && data.dataType.substr(0, 6) === 'iframe') {
        data.context.find('.progress-bar').css('width', '50%');
      }
    },
    fail: function (e, data) {
      if (data.errorThrown === 'abort') return;
      alert('There was an error uploading "' + data.files[0].name + '".');
    },
    progress: function (e, data) {
      data.context.find('.progress-bar').css('width', parseInt(data.loaded / data.total * 100, 10) + '%');
    },
    start: function (e, data) {
      $('.progress').show();
      $('.upload-queue .ui-icon-close').hide();
      $('.upload-queue .ui-icon-cancel').show();
    },
    stop: function (e, data) {
      //data.context.find('.progress-bar').css('width', '100%');
      LoadFiles();
      $('.upload-queue').empty();
      $('#attachmentDescription').val('');
      $('#fileForm').toggle();
    }
  });

  var getCustomers = function (request, response) {
    if (_execGetCustomer) { _execGetCustomer._executor.abort(); }
    _execGetCustomer = window.parent.parent.Ts.Services.Organizations.GetUserOrOrganizationForTicket(request.term, function (result) { response(result); });
  }

  $('#inputCustomer').autocomplete({
    open: function () {
      $('.ui-menu').width($('#inputCustomer').width());
    },
    minLength: 2,
    source: getCustomers,
    defaultDate: new Date(),
    select: function (event, ui) {
      $(this).data('item', ui.item);
    }
  });

  function LoadCustomProperties() {
    window.parent.parent.Ts.Services.Assets.GetCustomValues(_assetID, window.parent.parent.Ts.ReferenceTypes.Assets, function (html) {
      //$('#customProperties').append(html);
      appendCustomValues(html);

    });
  }

  function LoadProperties() {
    window.parent.parent.Ts.Services.Assets.GetAsset(_assetID, function (asset) {
      if (asset.Name) {
        $('#assetTitle').html(asset.Name);
      }
      else if (asset.SerialNumber) {
        $('#assetTitle').html(asset.SerialNumber);
      }
      else {
        $('#assetTitle').html(asset.AssetID);
      }

      $('#fieldName').text((asset.Name === null || $.trim(asset.Name) === '' ? 'Unassigned' : asset.Name));
      $('#fieldProduct').data('productID', asset.ProductID);
      $('#fieldProduct').text((asset.ProductName === null || $.trim(asset.ProductName) === '' ? 'Unassigned' : asset.ProductName));
      $('#fieldProductVersion').data('productVersionID', asset.ProductVersionID);
      if (asset.ProductVersionID == null) {
        var product = window.parent.parent.Ts.Cache.getProduct(asset.ProductID);
        if (product.Versions.length == 0) {
          $('#fieldProductVersion').text('N/A');
          $('#fieldProductVersion').removeClass("editable");
        }
        else {
          $('#fieldProductVersion').text('Unassigned');
          $('#fieldProductVersion').addClass("editable");
        }
      }
      else {
        $('#fieldProductVersion').text(asset.ProductVersionNumber);
      }
      $('#fieldSerialNumber').text((asset.SerialNumber === null || $.trim(asset.SerialNumber) === '' ? 'Unassigned' : asset.SerialNumber));
      $('#fieldWarrantyExpiration').text(window.parent.parent.Ts.Utils.getMsDate(asset.WarrantyExpiration).localeFormat(window.parent.parent.Ts.Utils.getDatePattern()));
      $('#fieldNotes').text((asset.Notes === null || $.trim(asset.Notes) === '' ? 'Unassigned' : asset.Notes));

      $('#fieldAssetID').text(asset.AssetID);
      $('#fieldCreator').text(asset.CreatorName);
      $('#fieldDateCreated').text(window.parent.parent.Ts.Utils.getMsDate(asset.DateCreated).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()));
      $('#fieldModifier').text(asset.ModifierName);
      $('#fieldDateModified').text(window.parent.parent.Ts.Utils.getMsDate(asset.DateModified).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()));

      switch (asset.Location) {
        case '1':
          $('#locationHeader').text('Assigned');
          $('.assignedDetails').show();
          $('.junkyardDetails').hide();
          $('#assetToJunkyard').hide();
          window.parent.parent.Ts.Services.Assets.GetAssetAssignments(_assetID, function (assetAssignments) {
            _assetAssignments = assetAssignments;
            for (var i = 0; i < assetAssignments.length; i++) {
              var refTypeClass = 'contactlink';
              if (assetAssignments[i].RefType == 9) {
                refTypeClass = 'companylink'
              }
              $('<tr>').html('<td>' +
              "<a href='#' id='" + assetAssignments[i].ShippedTo + "' class='" + refTypeClass + "'>" + assetAssignments[i].NameAssignedTo + '</a></td><td>' +
              window.parent.parent.Ts.Utils.getMsDate(assetAssignments[i].ActionTime).localeFormat(window.parent.parent.Ts.Utils.getDatePattern()) + '</td><td>' +
              assetAssignments[i].ActorName + '</td><td>' +
              assetAssignments[i].Comments + '</td><td>' +
              '</td>').appendTo('#assignedTable > tbody:last');
            }
          });
          break;
        case '2':
          $('#locationHeader').text('Warehouse');
          $('#locationDetails').hide();
          $('#returnAsset').hide();
          break;
        case '3':
          $('#locationHeader').text('Junkyard');
          $('.assignedDetails').hide();
          $('.junkyardDetails').show();
          $('#assetAssign').hide();
          $('#returnAsset').hide();
          $('#assetToJunkyard').hide();
          break;
      }
    });
  }

  function LoadHistory(start) {

    if (start == 1)
      $('#tblHistory tbody').empty();

    window.parent.parent.Ts.Services.Assets.LoadHistory(_assetID, start, function (history) {
      for (var i = 0; i < history.length; i++) {
        var nameAssignedFromValue = history[i].NameAssignedFrom == null ? '' : history[i].NameAssignedFrom;
        var nameAssignedToValue = history[i].NameAssignedTo == null ? '' : history[i].NameAssignedTo;
        $('<tr>').html('<td>' +
        history[i].ActionDescription + '</td><td>' +
        history[i].ActionTime.localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()) + '</td><td>' +
        history[i].ActorName + '</td><td>' +
        history[i].Comments + '</td><td>' +
        nameAssignedFromValue + '</td><td>' +
        nameAssignedToValue + '</td><td>' +
        history[i].ShippingMethod + '</td><td>' +
        history[i].TrackingNumber + '</td><td>' +
        history[i].ReferenceNum + '</td><td>' +
        '</td>').appendTo('#tblHistory > tbody:last');
      }
      if (history.length == 50)
        $('<button>').text("Load More").addClass('btn-link')
                    .click(function (e) {
                      LoadHistory($('#tblHistory tbody > tr').length + 1);
                      $(this).remove();
                    })
                   .appendTo('#tblHistory > tbody:last');
    });
  }

  function LoadActionsHistory(start) {

    if (start == 1)
      $('#tblActionsHistory tbody').empty();

    window.parent.parent.Ts.Services.Assets.LoadActionsHistory(_assetID, start, function (history) {
      for (var i = 0; i < history.length; i++) {
        $('<tr>').html('<td>' + history[i].DateCreated.localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()) + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td>')
        .appendTo('#tblActionsHistory > tbody:last');
      }
      if (history.length == 50)
        $('<button>').text("Load More").addClass('btn-link')
                    .click(function (e) {
                      LoadHistory($('#tblActionsHistory tbody > tr').length + 1);
                      $(this).remove();
                    })
                   .appendTo('#tblActionsHistory > tbody:last');
    });
  }

  function LoadFiles() {
    $('#tblFiles tbody').empty();
    window.parent.parent.Ts.Services.Assets.LoadFiles(_assetID, window.parent.parent.Ts.ReferenceTypes.Assets, function (files) {
      for (var i = 0; i < files.length; i++) {
        var tr = $('<tr>')
                .attr('id', files[i].AttachmentID)
                .html('<td><i class="fa fa-trash-o delFile"></i></td><td class="viewFile">' + files[i].FileName + '</td><td>' + files[i].Description + '</td><td>' + files[i].CreatorName + '</td><td>' + files[i].DateCreated.toDateString() + '</td>')
                .appendTo('#tblFiles > tbody:last');


        //$('#tblFiles > tbody:last').appendTo('<tr id=' +  + '></tr>');
      }
    });
  }

  $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    if (e.target.innerHTML == "Tickets")
      $('#ticketIframe').attr("src", "../../../Frames/TicketTabsAll.aspx?tf_AssetID=" + _assetID);
    //    else if (e.target.innerHTML == "Watercooler")
    //      $('#watercoolerIframe').attr("src", "WaterCooler.html?pagetype=2&pageid=" + organizationID);
    //    else if (e.target.innerHTML == "Details")
    //      createTestChart();
    //    else if (e.target.innerHTML == "Contacts")
    //      LoadContacts();
    //    else if (e.target.innerHTML == "Notes")
    //      LoadNotes();
    else if (e.target.innerHTML == "Files")
      LoadFiles();
    //    else if (e.target.innerHTML == "Ratings")
    //      LoadRatings('', 1);
  })

  $("#dateShipped").datetimepicker();

  $('.customProperties, .assetProperties').on('keydown', '.number', function (event) {
    // Allow only backspace and delete
    if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 190 || event.keyCode == 109 || event.keyCode == 173 || (event.keyCode >= 96 && event.keyCode <= 105)) {
      // let it happen, don't do anything
    }
    else {
      // Ensure that it is a number and stop the keypress
      if (event.keyCode < 48 || event.keyCode > 57) {
        event.preventDefault();
      }
    }
  });

  $('#asset-overview').on('keydown', '.number', function (event) {
    // Allow only backspace and delete
    if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 190 || event.keyCode == 109 || event.keyCode == 173 || (event.keyCode >= 96 && event.keyCode <= 105)) {
      // let it happen, don't do anything
    }
    else {
      // Ensure that it is a number and stop the keypress
      if (event.keyCode < 48 || event.keyCode > 57) {
        event.preventDefault();
      }
    }
  });

  $('.assetProperties p').toggleClass("editable");
});

function convertToValidDate(val) {
    var value = '';
    if (val == "")
        return value;

    if (_dateFormat.indexOf("M") != 0) {
        var dateArr = val.replace(/\./g, '/').replace(/-/g, '/').split('/');
        if (_dateFormat.indexOf("D") == 0)
            var day = dateArr[0];
        if (_dateFormat.indexOf("Y") == 0)
            var year = dateArr[0];
        if (_dateFormat.indexOf("M") == 3 || _dateFormat.indexOf("M") == 5)
            var month = dateArr[1];

        var timeSplit = dateArr[2].split(' ');
        if (_dateFormat.indexOf("Y") == 6)
            var year = timeSplit[0];
        else
            var day = timeSplit[0];

        var theTime = timeSplit[1];

        var formattedDate = month + "/" + day + "/" + year;
        value = parent.Ts.Utils.getMsDate(formattedDate);
        return formattedDate;
    }
    else
        return val;
}

function convertToValidDateTime(val) {
    var value = '';
    if (val == "")
        return value;

    if (_dateFormat.indexOf("M") != 0) {
        var dateArr = val.replace(/\./g, '/').replace(/-/g, '/').split('/');
        if (_dateFormat.indexOf("D") == 0)
            var day = dateArr[0];
        if (_dateFormat.indexOf("Y") == 0)
            var year = dateArr[0];
        if (_dateFormat.indexOf("M") == 3 || _dateFormat.indexOf("M") == 5)
            var month = dateArr[1];

        var timeSplit = dateArr[2].split(' ');
        if (_dateFormat.indexOf("Y") == 6)
            var year = timeSplit[0];
        else
            var day = timeSplit[0];

        var theTime = timeSplit[1];

        var formattedDate = month + "/" + day + "/" + year + " " + theTime;
        //value = parent.Ts.Utils.getMsDate(formattedDate) + " " + theTime;
        return formattedDate;
    }
    else
        return val;
}

var getUrls = function (input) {
  var source = (input || '').toString();
  var url;
  var matchArray;
  var result = '';

  // Regular expression to find FTP, HTTP(S) and email URLs. Updated to include urls without http
  var regexToken = /(((ftp|https?|www):?\/?\/?)[\-\w@:%_\+.~#?,&\/\/=]+)|((mailto:)?[_.\w-]+@([\w][\w\-]+\.)+[a-zA-Z]{2,3})/g;

  // Iterate through any URLs in the text.
  while ((matchArray = regexToken.exec(source)) !== null) {
    url = matchArray[0];
    if (url.length > 2 && url.substring(0, 3) == 'www') {
      url = 'http://' + url;
    }
    result = result + '<a target="_blank" class="valueLink" href="' + url + '" title="' + matchArray[0] + '">' + matchArray[0] + '</a>'
  }

  return result == '' ? input : result;
}

var appendCustomValues = function (fields) {
  if (fields === null || fields.length < 1) {
    $('.customProperties').empty();
    return;
  }
  var containerL = $('#customPropertiesL').empty();
  var containerR = $('#customPropertiesR').empty();


  for (var i = 0; i < fields.length; i++) {
    var item = null;

    var field = fields[i];

    var div = $('<div>').addClass('form-group').data('field', field);
    $('<label>')
          .addClass('col-md-4 control-label')
          .text(field.Name)
          .appendTo(div);

    switch (field.FieldType) {
      case window.parent.parent.Ts.CustomFieldType.Text: appendCustomEdit(field, div); break;
      case window.parent.parent.Ts.CustomFieldType.Date: appendCustomEditDate(field, div); break;
      case window.parent.parent.Ts.CustomFieldType.Time: appendCustomEditTime(field, div); break;
      case window.parent.parent.Ts.CustomFieldType.DateTime: appendCustomEditDateTime(field, div); break;
      case window.parent.parent.Ts.CustomFieldType.Boolean: appendCustomEditBool(field, div); break;
      case window.parent.parent.Ts.CustomFieldType.Number: appendCustomEditNumber(field, div); break;
      case window.parent.parent.Ts.CustomFieldType.PickList: appendCustomEditCombo(field, div); break;
      default:
    }

    if (i % 2)
      containerR.append(div);
    else
      containerL.append(div);

  }
  $('.customProperties p').toggleClass("editable");
  //$('#contactName').toggleClass("editable");
}

var appendCustomEditCombo = function (field, element) {
  var div = $('<div>')
    .addClass('col-md-8')
    .appendTo(element);

  var result = $('<p>')
      .text((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : field.Value))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
          return false;
        var parent = $(this).hide();
        window.parent.parent.Ts.System.logAction('Asset Detail - Edit Custom Combobox');
        var container = $('<div>')
            .insertAfter(parent);

        var container1 = $('<div>')
          .addClass('col-md-9')
          .appendTo(container);

        var fieldValue = parent.closest('.form-group').data('field').Value;
        var select = $('<select>').addClass('form-control').attr('id', field.Name.replace(/\W/g, '')).appendTo(container1);

        var items = field.ListValues.split('|');
        for (var i = 0; i < items.length; i++) {
          var option = $('<option>').text(items[i]).appendTo(select);
          if (fieldValue === items[i]) { option.attr('selected', 'selected'); }
        }

        $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
              $(this).closest('div').remove();
              parent.show();
              $('#assetEdit').removeClass("disabled");
            })
            .insertAfter(container1);

        $('#' + field.Name.replace(/\W/g, '')).on('change', function () {
          var value = $(this).val();
          container.remove();

          if (field.IsRequired && field.IsFirstIndexSelect == true && $(this).find('option:selected').index() < 1) {
            alert("This field is required and the first value is not a valid selection for a required field.");
          }
          else {
            window.parent.parent.Ts.System.logAction('Asset Detail - Save Custom Edit Change');
            window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _assetID, value, function (result) {
              parent.closest('.form-group').data('field', result);
              parent.text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
              $('#assetEdit').removeClass("disabled");
            }, function () {
              alert("There was a problem saving your asset property.");
              $('#assetEdit').removeClass("disabled");
            });
            result.parent().removeClass('has-error');
            result.removeClass('form-control');
            result.parent().children('.help-block').remove();
          }
          parent.show();
          $('#assetEdit').removeClass("disabled");
        });

        $('#assetEdit').addClass("disabled");
      });
  var items = field.ListValues.split('|');
  if (field.IsRequired && ((field.IsFirstIndexSelect == true && (items[0] == field.Value || field.Value == null || $.trim(field.Value) === '')) || (field.Value == null || $.trim(field.Value) === ''))) {
    result.parent().addClass('has-error');
    result.addClass('form-control');
    if (result.parent().children('.help-block').length == 0) {
      $('<label>')
        .addClass('help-block')
        .text('This field is required')
        .appendTo(result.parent());
    }
  }
}

var appendCustomEditNumber = function (field, element) {
  var div = $('<div>')
    .addClass('col-md-8')
    .appendTo(element);

  var result = $('<p>')
      .text((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : field.Value))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
          return false;
        var parent = $(this).hide();
        window.parent.parent.Ts.System.logAction('Asset Detail - Edit Custom Number');
        var container = $('<div>')
            .insertAfter(parent);

        var container1 = $('<div>')
          .addClass('col-md-9')
          .appendTo(container);

        var fieldValue = parent.closest('.form-group').data('field').Value;
        var input = $('<input type="text">')
            .addClass('col-md-10 form-control number')
            .val(fieldValue)
            .appendTo(container1)
            .focus();

        $('<i>')
            .addClass('col-md-1 fa fa-times')
            .click(function (e) {
              $(this).closest('div').remove();
              parent.show();
              $('#assetEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('<i>')
            .addClass('col-md-1 fa fa-check')
            .click(function (e) {
              var value = input.val();
              container.remove();
              if (field.IsRequired && (value === null || $.trim(value) === '')) {
                alert("This field is required");
              }
              else {
                window.parent.parent.Ts.System.logAction('Asset Detail - Save Custom Number Edit');
                window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _assetID, value, function (result) {
                  parent.closest('.form-group').data('field', result);
                  parent.text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
                  $('#assetEdit').removeClass("disabled");
                }, function () {
                  alert("There was a problem saving your asset property.");
                  $('#assetEdit').removeClass("disabled");
                });
                result.parent().removeClass('has-error');
                result.removeClass('form-control');
                result.parent().children('.help-block').remove();
              }
              parent.show();
              $('#assetEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('#assetEdit').addClass("disabled");
      });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
    result.addClass('form-control');
    if (result.parent().children('.help-block').length == 0) {
      $('<label>')
        .addClass('help-block')
        .text('This field is required')
        .appendTo(result.parent());
    }
  }
}

var appendCustomEditBool = function (field, element) {

  var div = $('<div>')
    .addClass('col-md-8')
    .appendTo(element);

  var result = $('<p>')
      .text((field.Value === null || $.trim(field.Value) === '' ? 'False' : field.Value))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
          return false;
        //$('.form-group').prev().show().next().remove();
        window.parent.parent.Ts.System.logAction('Asset Detail - Edit Custom Boolean Value');
        var parent = $(this);
        var value = $(this).text() === 'No' || $(this).text() === 'False' ? true : false;
        window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _assetID, value, function (result) {
          parent.closest('.form-group').data('field', result);
          parent.text((result.Value === null || $.trim(result.Value) === '' ? 'False' : result.Value));
        }, function () {
          alert("There was a problem saving your asset property.");
        });
      });
}

var appendCustomEdit = function (field, element) {

  var div = $('<div>')
    .addClass('col-md-8')
    .appendTo(element);

  var result = $('<p>')
      .html((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : getUrls(field.Value)))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
        if ($(this).has('a') && !$(this).hasClass('editable')) {
          return;
        }
        else {
          e.preventDefault();
          if (!$(this).hasClass('editable'))
            return false;
          var parent = $(this).hide();
          window.parent.parent.Ts.System.logAction('Asset Detail - Edit Custom Textbox');
          var container = $('<div>')
                .insertAfter(parent);

          var container1 = $('<div>')
              .addClass('col-md-9')
              .appendTo(container);

          var fieldValue = parent.closest('.form-group').data('field').Value;
          var input = $('<input type="text">')
                .addClass('col-md-10 form-control')
                .val(fieldValue)
                .appendTo(container1)
                .focus();

          if (field.Mask) {
            input.mask(field.Mask);
            input.attr("placeholder", field.Mask);
          }

          $('<i>')
                .addClass('col-md-1 fa fa-times')
                .click(function (e) {
                  $(this).closest('div').remove();
                  parent.show();
                  $('#assetEdit').removeClass("disabled");
                })
                .insertAfter(container1);
          $('<i>')
                .addClass('col-md-1 fa fa-check')
                .click(function (e) {
                  var value = input.val();
                  container.remove();
                  if (field.IsRequired && (value === null || $.trim(value) === '')) {
                    alert("This field is required");
                  }
                  else {
                    window.parent.parent.Ts.System.logAction('Asset Detail - Save Custom Textbox Edit');
                    window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _assetID, value, function (result) {
                      parent.closest('.form-group').data('field', result);
                      parent.html((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : getUrls(result.Value)));
                      $('#assetEdit').removeClass("disabled");
                    }, function () {
                      alert("There was a problem saving your asset property.");
                      $('#assetEdit').removeClass("disabled");
                    });
                    result.parent().removeClass('has-error');
                    result.removeClass('form-control');
                    result.parent().children('.help-block').remove();
                  }
                  parent.show();
                })
                .insertAfter(container1);
          $('#assetEdit').addClass("disabled");
        }
      });

  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
    result.addClass('form-control');
    if (result.parent().children('.help-block').length == 0) {
      $('<label>')
        .addClass('help-block')
        .text('This field is required')
        .appendTo(result.parent());
    }
  }
}

var appendCustomEditDate = function (field, element) {
  var date = field.Value == null ? null : window.parent.parent.Ts.Utils.getMsDate(field.Value);

  var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);

  var result = $('<p>')
      .text((date === null ? 'Unassigned' : date.localeFormat(window.parent.parent.Ts.Utils.getDatePattern())))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
          return false;
        var parent = $(this).hide();
        window.parent.parent.Ts.System.logAction('Asset Detail - Edit Custom Date');
        var container = $('<div>')
            .insertAfter(parent);

        var container1 = $('<div>')
          .addClass('col-xs-9')
          .appendTo(container);

        var fieldValue = parent.closest('.form-group').data('field').Value;
        var input = $('<input type="text">')
            .addClass('col-xs-10 form-control')
			.val(fieldValue === null ? '' : moment(fieldValue).format(window.parent.parent.Ts.Utils.getDatePattern().toUpperCase()))
            .datetimepicker({ pickTime: false, format: _dateFormat })
            .appendTo(container1)
            .focus();

        $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
              $(this).closest('div').remove();
              parent.show();
              $('#assetEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
                var value = window.parent.parent.Ts.Utils.getMsDate(convertToValidDate(input.val()));
              container.remove();
              if (field.IsRequired && (value === null || $.trim(value) === '')) {
                // Currently there is no way to clear a Date.
                // If ever implemented this alert will prevent clearing a required date.
                alert("This field is required");
              }
              else {
                window.parent.parent.Ts.System.logAction('Asset Detail - Save Custom Date Change');
                window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _assetID, value, function (result) {
                  parent.closest('.form-group').data('field', result);
                  var date = result.Value === null ? null : window.parent.parent.Ts.Utils.getMsDate(result.Value);
                  parent.text((date === null ? 'Unassigned' : date.localeFormat(window.parent.parent.Ts.Utils.getDatePattern())))
                  $('#assetEdit').removeClass("disabled");
                }, function () {
                  alert("There was a problem saving your asset property.");
                  $('#assetEdit').removeClass("disabled");
                });
                result.parent().removeClass('has-error');
                result.removeClass('form-control');
                result.parent().children('.help-block').remove();
              }
              parent.show();
            })
            .insertAfter(container1);
        $('#assetEdit').addClass("disabled");
      });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
    result.addClass('form-control');
    if (result.parent().children('.help-block').length == 0) {
      $('<label>')
        .addClass('help-block')
        .text('This field is required')
        .appendTo(result.parent());
    }
  }

}

var appendCustomEditDateTime = function (field, element) {
  var date = field.Value == null ? null : window.parent.parent.Ts.Utils.getMsDate(field.Value);

  var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);

  var result = $('<p>')
      .text((date === null ? 'Unassigned' : date.localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern())))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
          return false;
        var parent = $(this).hide();
        window.parent.parent.Ts.System.logAction('Asset Detail - Edit Custom DateTime');
        var container = $('<div>')
            .insertAfter(parent);

        var container1 = $('<div>')
          .addClass('col-xs-9')
          .appendTo(container);

        var fieldValue = parent.closest('.form-group').data('field').Value;
        var input = $('<input type="text">')
            .addClass('col-xs-10 form-control')
            .val(fieldValue === null ? '' : fieldValue.localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()))
            .datetimepicker({
                format: _dateFormat + " hh:mm a"
            })

            .appendTo(container1)
            .focus();

        $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
              $(this).closest('div').remove();
              parent.show();
              $('#assetEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
                var value = window.parent.parent.Ts.Utils.getMsDate(convertToValidDateTime(input.val()));
              container.remove();
              if (field.IsRequired && (value === null || $.trim(value) === '')) {
                // Currently there is no way to clear a Date.
                // If ever implemented this alert will prevent clearing a required date.
                alert("This field is required");
              }
              else {
                window.parent.parent.Ts.System.logAction('Asset Detail - Save Custom DateTime');
                window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _assetID, value, function (result) {
                  parent.closest('.form-group').data('field', result);
                  var date = result.Value === null ? null : window.parent.parent.Ts.Utils.getMsDate(result.Value);
                  parent.text((date === null ? 'Unassigned' : date.localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern())))
                  $('#assetEdit').removeClass("disabled");
                }, function () {
                  alert("There was a problem saving your asset property.");
                  $('#assetEdit').removeClass("disabled");
                });
                result.parent().removeClass('has-error');
                result.removeClass('form-control');
                result.parent().children('.help-block').remove();
              }
              parent.show();
            })
            .insertAfter(container1);
        $('#assetEdit').addClass("disabled");
      });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
    result.addClass('form-control');
    if (result.parent().children('.help-block').length == 0) {
      $('<label>')
        .addClass('help-block')
        .text('This field is required')
        .appendTo(result.parent());
    }
  }

}

var appendCustomEditTime = function (field, element) {
  var date = field.Value == null ? null : field.Value;

  var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);

  var result = $('<p>')
      .text((date === null ? 'Unassigned' : date.localeFormat(window.parent.parent.Ts.Utils.getTimePattern())))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
          return false;
        var parent = $(this).hide();
        window.parent.parent.Ts.System.logAction('Asset Detail - Edit Custom Time');
        var container = $('<div>')
            .insertAfter(parent);

        var container1 = $('<div>')
          .addClass('col-xs-9')
          .appendTo(container);

        var fieldValue = parent.closest('.form-group').data('field').Value;
        var input = $('<input type="text">')
            .addClass('col-xs-10 form-control')
            .val(fieldValue === null ? '' : fieldValue.localeFormat(window.parent.parent.Ts.Utils.getTimePattern()))
            .datetimepicker({ pickDate: false })

            .appendTo(container1)
            .focus();

        $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
              $(this).closest('div').remove();
              parent.show();
              $('#assetEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
              var value = window.parent.parent.Ts.Utils.getMsDate("1/1/1900 " + input.val());
              container.remove();
              if (field.IsRequired && (value === null || $.trim(value) === '')) {
                // Currently there is no way to clear a Date.
                // If ever implemented this alert will prevent clearing a required date.
                alert("This field is required");
              }
              else {
                window.parent.parent.Ts.System.logAction('Asset Detail - Save Custom Time');
                window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _assetID, value, function (result) {
                  parent.closest('.form-group').data('field', result);
                  var date = result.Value === null ? null : window.parent.parent.Ts.Utils.getMsDate(result.Value);
                  parent.text((date === null ? 'Unassigned' : date.localeFormat(window.parent.parent.Ts.Utils.getTimePattern())))
                  $('#assetEdit').removeClass("disabled");
                }, function () {
                  alert("There was a problem saving your asset property.");
                  $('#assetEdit').removeClass("disabled");
                });
                result.parent().removeClass('has-error');
                result.removeClass('form-control');
                result.parent().children('.help-block').remove();
              }
              parent.show();
            })
            .insertAfter(container1);
        $('#assetEdit').addClass("disabled");
      });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
    result.addClass('form-control');
    if (result.parent().children('.help-block').length == 0) {
      $('<label>')
        .addClass('help-block')
        .text('This field is required')
        .appendTo(result.parent());
    }
  }

}

function onShow() {
  _assetDetailPage.refresh();
};

AssetDetailPage = function () {

};

AssetDetailPage.prototype = {
  constructor: AssetDetailPage,
  refresh: function () {

  }
};

