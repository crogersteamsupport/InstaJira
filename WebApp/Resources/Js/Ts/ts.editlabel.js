jQuery.editLabel = function (selector, callback, defaultText, alwaysEditor) {
  if (alwaysEditor == null) alwaysEditor = false;

  $(selector).wrap('<span class="edit-label" />').addClass('edit-label-text')
    .after('<span class="ts-clickable ts-icon ts-icon-cancel"></span>')
    .after('<span class="ts-clickable ts-icon ts-icon-save"></span>')
    .after('<input class="text ui-widget-content ui-corner-all" type="text" />')
    .after('<span class="ts-clickable ts-iconx ts-icon-edit ui-helper-hidden"></span>');

  var parent = $(selector).parent();

  if (alwaysEditor) {

    $(parent).children('.ts-icon-cancel, .ts-icon-save').hide();
    $(parent).children('.edit-label-text, .ts-icon-edit').hide();
    $(parent).children('input').val($(parent).children('.edit-label-text').text());
    $(parent).children('input').focus(function (e) {
      $(parent).children('input').select();
      $(parent).children('.ts-icon-cancel, .ts-icon-save').show();
    });

    $(parent).children('.ts-icon-cancel').click(function () {
      $(parent).children('input').val($(parent).children('.edit-label-text').text());
      var text = $(parent).children('.edit-label-text').text();
      $(parent).children('.ts-icon-cancel, .ts-icon-save').hide();
    });

    $(parent).children('.ts-icon-save').click(function () {
      var text = $(parent).children('input').val().trim();
      $(parent).children('.edit-label-text').text(text);
      if (callback != null) callback(text);
      $(parent).children('.ts-icon-cancel, .ts-icon-save').hide();
    });

  }
  else {
    $(parent).children('.ts-icon-cancel, .ts-icon-save, input').hide();

    $(parent).children('.edit-label-text, .ts-icon-edit').click(function (e) {
      $(parent).children('.edit-label-text, .ts-icon-edit').hide();
      $(parent).children('.ts-icon-cancel, .ts-icon-save, input').show();
      var text = $(parent).children('.edit-label-text').text();
      if (text == defaultText) text = '';
      $(parent).children('input').val(text).focus().select();

    });

    $(parent).children('.ts-icon-cancel').click(function () {
      $(parent).children('.edit-label-text, .ts-icon-edit').show();
      $(parent).children('.ts-icon-cancel, .ts-icon-save, input').hide();
    });

    $(parent).children('.ts-icon-save').click(function () {
      $(parent).children('.edit-label-text, .ts-icon-edit').show();
      $(parent).children('.ts-icon-cancel, .ts-icon-save, input').hide();
      $(parent).children('.edit-label-text').text($(parent).children('input').val());
      if (defaultText != null && $(parent).children('input').val().trim() == '') {
        $(parent).children('.edit-label-text').text(defaultText);
      }
      if (callback != null) callback($(parent).children('input').val());
    });
  }
};
