/// <reference path="ts/ts.js" />
/// <reference path="ts/ts.services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="~/Default.aspx" />
/// <reference path="ts/ts.pages.main.js" />


$(document).ready(function () {
  var self = this;

  $('button').button();
  $('#divProgressBar').progressbar({ value: 0 });
  var versionID = parent.Ts.Utils.getQueryValue('versionid', window);

  $('#file-upload').attr('action', 'Upload/Products/' + versionID);
  var loadTimer = null;
  loadVersion();

  function showLoading() { $('.ts-loading').show().next().hide(); }

  function loadVersion() {
    loadTimer = setTimeout(showLoading, 500);
    parent.Ts.Services.Products.GetVersion(versionID, function (version) {
      parent.Ts.Services.CustomFields.GetValues(14, versionID, function (values) {
        loadAttachments(function () {
          var div = $('#divVersionInfo').html('');
          $('<h1>').text(version.ProductName + ' - ' + version.VersionNumber).appendTo(div);
          var table = $('<table>');
          $('<tr>').append('<td>Version:</td><td>' + version.VersionNumber + '</td>').appendTo(table);
          $('<tr>').append('<td>Status:</td><td>' + version.VersionStatus + '</td>').appendTo(table);
          $('<tr>').append('<td>Released:</td><td>' + (version.IsReleased === false ? 'No' : 'Yes') + '</td>').appendTo(table);
          if (version.ReleaseDate) $('<tr>').append('<td>Date:</td><td>' + version.ReleaseDate.localeFormat(parent.Sys.CultureInfo.CurrentCulture.dateTimeFormat.ShortDatePattern) + '</td>').appendTo(table);
          var formattedValue = '';
          for (var i = 0; i < values.length; i++) {
            if (values[i].Value) {
              formattedValue = values[i].Value;
            }
            else {
              formattedValue = '';
            }

            switch (values[i].FieldType) {
              case parent.Ts.CustomFieldType.Date:
                if (values[i].Value) {
                  formattedValue = parent.Ts.Utils.getMsDate(values[i].Value).localeFormat(parent.Ts.Utils.getDatePattern());
                }
                break;
              case parent.Ts.CustomFieldType.Time:
                if (values[i].Value) {
                  formattedValue = parent.Ts.Utils.getMsDate(values[i].Value).localeFormat(parent.Ts.Utils.getTimePattern());
                }
                break;
              case parent.Ts.CustomFieldType.DateTime:
                if (values[i].Value) {
                  formattedValue = parent.Ts.Utils.getMsDate(values[i].Value).localeFormat(parent.Ts.Utils.getDateTimePattern());
                }
                break;
            }

            $('<tr>').append('<td>' + values[i].Name + ':</td><td>' + formattedValue + '</td>').appendTo(table);
          }

          table.appendTo(div);
          $('#divDescription').html(version.Description == '' ? 'No Description' : version.Description);

          if (loadTimer) clearTimeout(loadTimer);
          $('.ts-loading').hide().next().show();
        });
      });
    });
  }

  function loadAttachments(callback) {
    parent.Ts.Services.Products.GetAttachments(versionID, function (attachments) {
      var list = $('<ul>');

      for (var i = 0; i < attachments.length; i++) {
        $('<li>')
          .html('<span class="ts-icon ts-icon-attachment"></span><a href="../../../dc/attachments/' + attachments[i].AttachmentID + '/" target="_blank">' + attachments[i].FileName + '</a><span class="ui-icon ui-icon-close"></span>')
          .data('id', attachments[i].AttachmentID)
          .data('fileName', attachments[i].FileName)
          .appendTo(list)
          .find('.ui-icon')
          .click(function (e) {
            var item = $(this).parent();
            if (!confirm('Are you sure you would like to remove ' + item.data('fileName') + '?')) return;
            parent.Ts.Services.Products.DeleteAttachment(item.data('id'), function () {
              item.remove();
            });

          });

      }
      $('#divAttachments').empty().append(list);
      if (callback) callback();
    });
  }


  $('.file-upload').fileupload({
    namespace: 'product_version',
    dropZone: $('.file-upload'),
    add: function (e, data) {
      for (var i = 0; i < data.files.length; i++) {
        var item = $('<li>')
          .appendTo($('.upload-queue'));

        data.context = item;
        data.url = '../../../Upload/Products/' + versionID;

        item.data('data', data);

        var bg = $('<div>')
          .addClass('ts-color-bg-accent ui-corner-all')
          .appendTo(item);

        $('<div>')
          .text(data.files[i].name + '  (' + parent.Ts.Utils.getSizeString(data.files[i].size) + ')')
          .addClass('filename')
          .appendTo(bg);

        $('<div>')
          .addClass('progress')
          .appendTo(bg)
          .progressbar();

        $('<span>')
          .addClass('ui-icon ui-icon-cancel')
          .click(function (e) {
            e.preventDefault();
            var data = $(this).closest('li').data('data');
            data.jqXHR.abort();
          })
          .appendTo(bg);

        data.jqXHR = data.submit();

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
    always: function (e, data) {
      data.context.remove();
      loadAttachments();
      parent.Ts.System.logAction('Product Version - Attacment Added');

    }
  });

});

