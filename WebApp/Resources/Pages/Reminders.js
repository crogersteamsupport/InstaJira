/// <reference path="ts/ts.services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="~/Default.aspx" />
/// <reference path="ts/ts.pages.main.js" />

var _reminders = null;
$(document).ready(function () {
  _reminders = new Reminders();
  _reminders.loadReminders();


});

function onShow() {
  _reminders.loadReminders();
}

Reminders = function () {

  $('button').button();
  $('.refresh').click(function (e) {
    e.preventDefault();
    location = location;
  }).hide();
  $('.page-loading').show().next().hide();


  function loadReminders() {
    $('.reminders').hide();
    $('.reminders .reminder').remove();
    parent.parent.Ts.Services.System.GetUserReminders(parent.parent.Ts.Utils.getQueryValue('userid', window), function (reminders) {
      $('.no-reminders').toggle(reminders.length < 1);

      for (var i = 0; i < reminders.length; i++) {
        var reminder = reminders[i];
        var ffd = getFutureFriendlyDate(reminder.DueDate);

        var reminderDiv = $('<div>')
        .addClass('reminder')
        .hover(function (e) {
          $('.reminders .ts-icon-edit').hide();
          $(this).find('.ts-icon-edit').show();
        }, function (e) {
          $('.reminders .ts-icon-edit').hide();
        })
        .data('o', reminder);

        $('<input>')
        .attr('type', 'checkbox')
        .change(function (e) {
          var item = $(this).closest('.reminder').addClass('dismissed');
          parent.parent.Ts.Services.System.DismissReminder(item.data('o').ReminderID, function () {
            item.fadeOut(500, function () { $(this).remove(); });
            parent.parent.Ts.System.logAction('Reminders - Reminder Dismissed');


          });
        })
        .appendTo(reminderDiv);

        var itemIcon = $('<span>')
        .addClass('item-icon ts-icon')
        .appendTo(reminderDiv);

        switch (reminder.RefType) {
          case parent.parent.Ts.ReferenceTypes.Contacts:
            itemIcon
            .addClass('ts-icon-contact')
            .attr('rel', '../../../Tips/User.aspx?UserID=' + reminder.RefID);
            break;
          case parent.parent.Ts.ReferenceTypes.Organizations:
            itemIcon
            .addClass('ts-icon-company')
            .attr('rel', '../../../Tips/Customer.aspx?CustomerID=' + reminder.RefID);
            break;
          case parent.parent.Ts.ReferenceTypes.Tickets:
            itemIcon
            .addClass('ts-icon-ticket')
            .attr('rel', '../../../Tips/Ticket.aspx?TicketID=' + reminder.RefID);
            break;
        }
        itemIcon.cluetip(parent.parent.Ts.Utils.getClueTipOptions(tipTimer))


        $('<span>')
        .addClass('date')
        .text(ffd.Text)
        .appendTo(reminderDiv);

        var desc = $('<span>')
          .addClass('description')
          .appendTo(reminderDiv);

        $('<a>')
          .addClass('ts-link ui-state-default')
          .attr('href', '#')
          .text(reminder.Description)
          .click(function (e) {
            e.preventDefault();
            parent.parent.Ts.MainPage.editReminder({ ReminderID: $(this).closest('.reminder').data('o').ReminderID }, true, function () { loadReminders(); });
          })
          .appendTo(desc);


        /*
        $('<span>')
        .addClass('ts-icon ts-icon-edit ui-helper-hidden')
        .click(function (e) {
        parent.parent.Ts.MainPage.editReminder({ ReminderID: $(this).closest('.reminder').data('o').ReminderID }, true, function () {
        loadReminders();
        });
        });
        //.appendTo(reminderDiv);
        */

        $('.reminders.' + ffd.Group).show().append(reminderDiv);
      }
      $('.page-loading').hide().next().show();
    });
  }


  $('body').delegate('.item-icon', 'mouseout', function (e) {
    if (tipTimer != null) clearTimeout(tipTimer);
    tipTimer = setTimeout("$(document).trigger('hideCluetip');", 3000);
  });

  var tipTimer = null;

  $('body').delegate('.cluetip', 'mouseover', function (e) {
    if (tipTimer != null) clearTimeout(tipTimer);
  });

  function getFutureFriendlyDate(date) {
    function getWeekNumber(d) {
      // Copy date so don't modify original
      d = new Date(d);
      // Set to nearest Thursday: current date + 4 - current day number
      // Make Sunday's day number 7
      d.setDate(d.getDate() + 4 - (d.getDay() || 7));
      // Get first day of year
      var yearStart = new Date(d.getFullYear(), 0, 1);
      // Calculate full weeks to nearest Thursday
      var weekNo = Math.ceil((((d - yearStart) / 86400000) + 1) / 7)
      // Return array of year and week number
      return weekNo;
    }

    date = parent.parent.Ts.Utils.getMsDate(date);
    var now = new Date();

    if (date.getTime() < now.getTime()) {
      return { Text: date.localeFormat(parent.parent.Ts.Utils.getDateTimePattern()), Group: 'past' }
    }

    if (date.getFullYear() == now.getFullYear()) {
      if (date.getMonth() == now.getMonth()) {
        if (date.getDate() == now.getDate()) return { Text: date.localeFormat(parent.parent.Sys.CultureInfo.CurrentCulture.dateTimeFormat.ShortTimePattern), Group: 'today' };
        var tomorrow = new Date(now.getFullYear(), now.getMonth(), now.getDate() + 1);
        if (tomorrow.getDate() == date.getDate()) return { Text: date.localeFormat(parent.parent.Sys.CultureInfo.CurrentCulture.dateTimeFormat.ShortTimePattern), Group: 'tomorrow' };
      }
      var w = getWeekNumber(date);
      var week = getWeekNumber(now);
      if (w == week) return { Text: date.localeFormat(parent.parent.Ts.Utils.getDateTimePattern()), Group: 'thisweek' }
      if (w == week + 1) return { Text: date.localeFormat(parent.parent.Ts.Utils.getDateTimePattern()), Group: 'nextweek' }
    }
    return { Text: date.localeFormat(parent.parent.Ts.Utils.getDateTimePattern()), Group: 'later' }
  }

  return {
    "loadReminders": loadReminders
  };
};



