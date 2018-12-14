/// <reference path="ts/ts.services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="~/Default.aspx" />
/// <reference path="ts/ts.pages.main.js" />
var _ticketID = null;
var _ticketNumber = null;
var _ticketCreator = new Object();
var _ticketSender = null;
var _timerid;
var _timerElapsed = 0;
var _ticketGroupID = null;
var _ticketGroupUsers = null;
var _dueDate = null;
var _parentFields = [];
var _ticketTypeID = null;
var _productFamilyID = null;
var speed = 50, counter = 0, start;

var execSelectTicket = null;
var execGetCustomer = null;
var execGetAsset = null;
var execGetUsers = null;
var execGetTags = null;
var execGetRelated = null;
var execGetCompany = null;

var _layout = null;
var tipTimer = null;
var clueTipOptions = top.Ts.Utils.getClueTipOptions(tipTimer);
var userFullName = top.Ts.System.User.FirstName + " " + top.Ts.System.User.LastName;
var canKbEdit = top.Ts.System.User.IsSystemAdmin || top.Ts.System.User.ChangeKbVisibility;
var alertMessage = null;
var dateFormat;
var session;
var token;
var recordingID;
var apiKey;
var sessionId;
var tokurl;
var publisher;

$(document).ready(function () {

  apiKey = "45228242";

  $('video').click(function () { this.paused ? this.play() : this.pause(); });

  $('body').delegate('video', 'click', function (e) { this.paused ? this.play() : this.pause(); });

  if (top.Ts.System.Organization.IsInventoryEnabled != true) $('.ticket-widget-assets').hide();

  $('.page-loading').show().next().hide();

  $(".dialog-emailinput").dialog({
    height: 205,
    width: 400,
    autoOpen: false,
    modal: true,
    buttons: {
      OK: function () {
        $(this).dialog("close");
        top.Ts.Services.Tickets.EmailTicket(_ticketID, $(".dialog-emailinput input").val(), $(".dialog-emailinput textarea").val(), function () {
          alert('Your emails have been sent.');
        }, function () {
          alert('There was an error sending the ticket email');
        });
      },
      Cancel: function () { $(this).dialog("close"); }
    }
  });

  $("#dialog-ticketmerge-search").autocomplete({
    minLength: 2,
    source: selectTicket,
    select: function (event, ui) {
      if (ui.item.data == _ticketID) {
        alert("Sorry, but you can not merge this ticket into itself.");
        return;
      }

      $(this).data('ticketid', ui.item.data).removeClass('ui-autocomplete-loading');
      $(this).data('ticketnumber', ui.item.id);

      try {
        top.Ts.Services.Tickets.GetTicketInfo(ui.item.id, function (info) {
          var descriptionString = info.Actions[0].Action.Description;

          if (ellipseString(info.Actions[0].Action.Description, 30).indexOf("<img src") !== -1)
            descriptionString = "This ticket starts off with an embedded/linked image. We have disabled this for the preview.";
          else if (ellipseString(info.Actions[0].Action.Description, 30).indexOf(".viewscreencast.com") !== -1)
            descriptionString = "This ticket starts off with an embedded recorde video.  We have disabled this for the preview.";
          else
            descriptionString = ellipseString(info.Actions[0].Action.Description, 30);

          var ticketPreviewName = "<div><strong>Ticket Name:</strong> " + info.Ticket.Name + "</div>";
          var ticketPreviewAssigned = "<div><strong>Ticket Assigned To:</strong> " + info.Ticket.UserName + "</div>";
          var ticketPreviewDesc = "<div><strong>Ticket Desciption Sample:</strong> " + descriptionString + "</div>";

          $('#ticketmerge-preview-details').after(ticketPreviewName + ticketPreviewAssigned + ticketPreviewDesc);
          $('#dialog-ticketmerge-preview').show();
          $('#dialog-ticketmerge-warning').show();
          $(".dialog-ticketmerge").dialog("widget").find(".ui-dialog-buttonpane").find(":button:contains('OK')").prop("disabled", false).removeClass("ui-state-disabled");
        })
      }
      catch (e) {
        alert("Sorry, there was a problem loading the information for that ticket.");
      }
    },
    position: { my: "right top", at: "right bottom", collision: "fit flip" }
  });

  $(".dialog-ticketmerge").dialog({
    height: 'auto',
    width: 600,
    autoOpen: false,
    resizable: false,
    modal: true,
    position: {
      my: "top",
      at: "top",
      of: $('.ticket-layout')
    },
    draggable: false,
    buttons: {
      OK: function (e) {
        if ($('#dialog-ticketmerge-search').val() == "") {
          alert("Please select a valid ticket to merge");
          return;
        }

        if ($('#dialog-ticketmerge-confirm').prop("checked")) {
          $(e.target).closest('.ui-dialog-buttonpane').addClass('saving');
          var winningID = $('#dialog-ticketmerge-search').data('ticketid');
          var winningTicketNumber = $('#dialog-ticketmerge-search').data('ticketnumber');
          top.Ts.Services.Tickets.MergeTickets(winningID, _ticketID, function (result) {
            if (result != "")
              alert(result);
            $(this).parent().remove();
          }, function () {
            $(this).parent().remove();
            alert('There was an error merging the tickets.' + result);
          });
          $(this).dialog("close");
          clearDialog();
          top.Ts.MainPage.closeTicketTab(_ticketNumber);
          top.Ts.MainPage.openTicket(winningTicketNumber, true);
          window.location = window.location;
          window.top.ticketSocket.server.ticketUpdate(_ticketNumber + "," + winningTicketNumber, "merge", userFullName);
        }
        else {
          alert("You did not agree to the conditions of the merge. Please go back and check the box if you would like to merge.")
        }
      },
      Cancel: function () { $(this).dialog("close"); clearDialog(); }
    }
  });

  top.Ts.Services.Customers.GetDateFormat(true, function (format) {
    dateFormat = format.replace("yyyy", "yy");
    if (dateFormat.length < 8) {
      var dateArr = dateFormat.split('/');
      if (dateArr[0].length < 2) {
        dateArr[0] = dateArr[0] + dateArr[0];
      }
      if (dateArr[1].length < 2) {
        dateArr[1] = dateArr[1] + dateArr[1];
      }
      if (dateArr[2].length < 2) {
        dateArr[1] = dateArr[1] + dateArr[1];
      }
      dateFormat = dateArr[0] + "/" + dateArr[1] + "/" + dateArr[2];
    }

  });

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

  $('.ticket-history-hide').click(function (e) {
    e.preventDefault();
    $('.ticket-show-history').show().next().hide();
  });

  $('.ticket-history-refresh').click(function (e) {
    e.preventDefault();
    $('.ticket-show-history').click();
  });

  $('.ticket-show-history').click(function (e) {
    e.preventDefault();

    $(this).hide().next().show();
    $('.ticket-history-logs').empty().addClass('ts-loading');

    top.Ts.Services.Tickets.GetTicketHistory(_ticketID, function (logs) {
      var table = $('<table cellpadding="0" cellspacing="0" border="0">');
      for (var i = 0; i < logs.length; i++) {
        var row = $('<tr>').appendTo(table);
        var col1 = $('<td>').appendTo(row);
        $('<span>').addClass('ticket-history-actor').text(logs[i].CreatorName).appendTo(col1);
        $('<span>').addClass('ticket-history-date').text(logs[i].DateCreated.localeFormat(top.Ts.Utils.getDateTimePattern())).appendTo(col1);

        $('<span>')
.addClass('ticket-history-desc')
.html(logs[i].Description)
.appendTo($('<td>').appendTo(row));
      }
      $('.ticket-history-logs').removeClass('ts-loading').append(table);
    }, function () {
      alert('There was a problem retrieving the history for the ticket.');
    });
  });

  $('.ticket-widget').addClass('ui-widget-content ui-corner-all');

  $('#divCustomers').delegate('.ticket-customer-contact .ui-icon-close', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var item = $(this).parent();
    var data = item.data('data');
    top.Ts.Services.Tickets.RemoveTicketContact(_ticketID, data.UserID, function (customers) {
      appendCustomers(customers);
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removecontact", userFullName);
    }, function () {
      alert('There was a problem removing the contact from the ticket.');
    });
    top.Ts.System.logAction('Ticket - Contact Removed');
  });

  $('#divCustomers').delegate('.ticket-customer-company .ui-icon-close', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var item = $(this).parent();
    var data = item.data('data');
    top.Ts.Services.Tickets.RemoveTicketCompany(_ticketID, data.OrganizationID, function (customers) {
      appendCustomers(customers);
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removecompany", userFullName);
    }, function () {
      alert('There was a problem removing the company from the ticket.');
    });
    top.Ts.System.logAction('Ticket - Customer Removed');

  });

  $('.ticket-customer-add')
  .click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.ticket-rail-input').remove();
    $(this).parent().find('.ui-icon').removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');
    $(this).next().show();
    var container = $('<div>').
   addClass('ticket-rail-input')
   .prependTo($(this).parent().next().next().show());
    var input = $('<input type="text">')
    .addClass('ui-corner-all ui-widget-content')
    .autocomplete({
      minLength: 2,
      source: getCustomers,
      select: function (event, ui) {
        $(this)
      .data('item', ui.item)
      .removeClass('ui-autocomplete-loading');
        //.next().show();

        var item = $(this).data('item');
        alertMessage = item;
        top.Ts.Services.Tickets.AddTicketCustomer(_ticketID, item.data, item.id, function (customers) {
          appendCustomers(customers);
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
          window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addcustomer", userFullName);
          $(this).parent().remove();
        }, function () {
          $(this).parent().remove();
          alert('There was an error adding the customer.');
        });
        top.Ts.System.logAction('Ticket - Customer Added');
      }
    })
    .appendTo(container)
    .focus()
    .width(container.width() - 48 - 12);

    //$('<span>')
    //  .addClass('ts-icon ts-icon-save')
    //  .hide()
    //  .click(function (e) {
    //    var item = $(this).prev().data('item');
    //    top.Ts.Services.Tickets.AddTicketCustomer(_ticketID, item.data, item.id, function (customers) {
    //        appendCustomers(customers);
    //        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addcustomer", userFullName);
    //      $(this).parent().remove();
    //    }, function () {
    //      $(this).parent().remove();
    //      alert('There was an error adding the customer.');
    //    });
    //    top.Ts.System.logAction('Ticket - Customer Added');
    //  })
    //  .appendTo(container)

    $('<span>')
    .addClass('ts-icon ts-icon-cancel')
    .click(function (e) {
      $(this).parent().remove();
    })
    .appendTo(container);

    $('<div>').addClass('clearfix').appendTo(container);
  });

  $('#divAssets').delegate('.ticket-asset .ui-icon-close', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var item = $(this).parent();
    var data = item.data('data');
    top.Ts.Services.Tickets.RemoveTicketAsset(_ticketID, data.AssetID, function (assets) {
      appendAssets(assets);
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removeasset", userFullName);
    }, function () {
      alert('There was a problem removing the asset from the ticket.');
    });
    top.Ts.System.logAction('Ticket - Asset Removed');
  });

  $('.ticket-asset-add')
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
      source: getAssets,
      select: function (event, ui) {
        $(this)
    .data('item', ui.item)
    .removeClass('ui-autocomplete-loading')
    .next().show();
        top.Ts.Services.Tickets.AddTicketAsset(_ticketID, ui.item.id, function (assets) {
          appendAssets(assets);
          window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addasset", userFullName);
          $(this).parent().remove();
          //here we need to implement a refresh of the ticket customers and contacts.
          top.Ts.Services.Tickets.GetTicketCustomers(_ticketID, function (customers) {
            appendCustomers(customers);
          });
        }, function () {
          $(this).parent().remove();
          alert('There was an error adding the asset.');
        });
        top.Ts.System.logAction('Ticket - Asset Added');
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

  $('#divSubscribers').delegate('.ticket-subscriber .ui-icon-close', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var item = $(this).parent();
    var user = item.data('data');
    item.remove();
    top.Ts.Services.Tickets.SetSubscribed(_ticketID, false, user.UserID, function (subscribers) {
      appendSubscribers(subscribers);
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removesubscriber", userFullName);
    }, function () {
      alert('There was a problem removing the subscriber from the ticket.');
    });
    top.Ts.System.logAction('Ticket - Subscriber Removed');
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

        var userID = $(this).data('userID');
        top.Ts.Services.Tickets.SetSubscribed(_ticketID, true, userID, function (subscribers) {
          appendSubscribers(subscribers);
          window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addsubscriber", userFullName);
        }, function () {
          $(this).parent().remove();
          alert('There was an error adding the subscriber.');
        });
        top.Ts.System.logAction('Ticket - User Subscribed');
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

  $('#divQueues').delegate('.ticket-queue .ui-icon-close', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var item = $(this).parent();
    var user = item.data('data');
    top.Ts.Services.Tickets.SetQueue(_ticketID, false, user.UserID, function (queues) {
      appendQueues(queues);
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removequeue", userFullName);
    }, function () {
      alert('There was a problem removing the queue from the ticket.');
    });
    top.Ts.System.logAction('Ticket - Dequeued');
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

        var userID = $(this).data('userID');
        top.Ts.Services.Tickets.SetQueue(_ticketID, true, userID, function (queues) {
          appendQueues(queues);
          window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addqueue", userFullName);
        }, function () {
          $(this).parent().remove();
          alert('There was an error adding the queue.');
        });
        top.Ts.System.logAction('Ticket - Enqueued');
        top.Ts.System.logAction('Queued');
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

  $('#divTags').delegate('.ticket-tag .ui-icon-close', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var item = $(this).parent();
    var data = item.data('data');
    top.Ts.Services.Tickets.RemoveTag(_ticketID, data.TagID, function (tags) {
      appendTags(tags);
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removetag", userFullName);
    }, function () {
      alert('There was a problem removing the tag from the ticket.');
    });

    top.Ts.System.logAction('Ticket - Tag Removed');

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

        top.Ts.Services.Tickets.AddTag(_ticketID, ui.item.value, function (tags) {
          if (tags !== null) {
            appendTags(tags);
            window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addtag", userFullName);
          }
          $(this).parent().remove();
        }, function () {
          $(this).parent().remove();
          alert('There was an error adding the tag.');
        });
        top.Ts.System.logAction('Ticket - Added');
      }
    })
    .appendTo(container)
    .focus()
    .width(container.width() - 48 - 12);

    $('<span>')
    .addClass('ts-icon ts-icon-save')
    .click(function (e) {
      top.Ts.Services.Tickets.AddTag(_ticketID, $(this).prev().val(), function (tags) {
        if (tags !== null) {
          appendTags(tags);
          window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addtag", userFullName);
        }
        $(this).parent().remove();
      }, function () {
        $(this).parent().remove();
        alert('There was an error adding the tag.');
      });
      top.Ts.System.logAction('Ticket - Added');


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

  $('#divRelated').delegate('.ticket-related .ui-icon-close', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var item = $(this).parent();
    var data = item.data('data');
    top.Ts.Services.Tickets.RemoveRelated(_ticketID, data.TicketID, function (result) {
      if (result !== null && result === true) item.remove();
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removerelationship", userFullName);
    }, function () {
      alert('There was an error removing the associated ticket.');
    });

    top.Ts.System.logAction('Ticket - Association Removed');
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
    $('<button>').text('Parent').appendTo(buttons).button().click(function (e) { addRelated(false); });
    $('<button>').text('Child').appendTo(buttons).button().click(function (e) { addRelated(true); });
    function addRelated(isParent) {
      var item = container.find('.related-input').data('item');
      top.Ts.Services.Tickets.AddRelated(_ticketID, item.data, isParent, function (tickets) {
        appendRelated(tickets);
        container.remove();
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addrelationship", userFullName);
      }, function (error) {
        //container.remove();
        alert(error.get_message());
      });
      top.Ts.System.logAction('Ticket - Association Added');
    }
  });

  $('#divReminders').delegate('.ticket-reminder .ui-icon-close', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var item = $(this).parent();
    var reminder = item.data('o');
    top.Ts.Services.System.DismissReminder(reminder.ReminderID, function () {
      item.remove();
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removereminder", userFullName);
    }, function () {
      alert('There was a problem removing the reminder from the ticket.');
    });
    top.Ts.System.logAction('Ticket - Reminder Removed');
  });

  $('.ticket-reminder-add')
  .click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.MainPage.editReminder({
      RefType: top.Ts.ReferenceTypes.Tickets,
      RefID: _ticketID
    },
  true,
  function (reminder) {
    top.Ts.System.logAction('Ticket - Reminder Added');
    top.Ts.Services.System.GetItemReminders(top.Ts.ReferenceTypes.Tickets, _ticketID, top.Ts.System.User.UserID, function (reminders) {
      appendReminders(reminders);
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addreminder", userFullName);
    });

  }

  );
  });

  $('body').delegate('.ts-icon-info', 'mouseout', function (e) {
    if (tipTimer != null) clearTimeout(tipTimer);
    tipTimer = setTimeout("$(document).trigger('hideCluetip');", 3000);
  });

  $('body').delegate('.cluetip', 'mouseover', function (e) {
    if (tipTimer != null) clearTimeout(tipTimer);
  });

  $('.ticket-action-add').click(function (e) {
    e.preventDefault();
    var parent = $(this).hide();
    $('.ticket-action-new').show();
    top.Ts.System.logAction('Ticket - New Action Started');
    createActionForm($('<div>').appendTo('.ticket-action-new-form'), null, function (result) {
      if ($('.ticket-action-form').length < 2) {
        top.Ts.MainPage.highlightTicketTab(_ticketNumber, false);
      }
      if (result !== null) {
        /*var actionDiv = createActionElement().prependTo('#divActions');
        loadActionDisplay(actionDiv, result, true);
        $(".ticket-action-order").each(function (index) {
        $(this).html('(' + ($('.ticket-action-order').length - index) + ')');
        });*/
        loadTicket(_ticketNumber, 0);
      }
      parent.show();
      $('.ticket-action-new').hide();
    });
  })
.hover(function (e) {
  $(this).css('textDecoration', 'underline');
}, function (e) {
  $(this).css('textDecoration', '');
});



  $('#divActions').delegate('.ticket-action-edit', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var actionDiv = $(this).parents('.ticket-action');
    //var action = actionDiv.data('action');

    top.Ts.Services.Tickets.GetAction(actionDiv.data('action').ActionID, function (action) {
      var form = $('<div>').addClass('');
      actionDiv.append(form).find('.ticket-action-display').hide();
      createActionForm(form, action, function (actionInfo) {
        if ($('.ticket-action-form').length < 2) {
          top.Ts.MainPage.highlightTicketTab(_ticketNumber, false);
        }
        if (actionInfo !== null) { loadActionDisplay(actionDiv, actionInfo, true); }
        actionDiv.find('.ticket-action-display').show();
      });
    });
  });

  $('#divActions').delegate('.ticket-action-delete', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    if (confirm('Are you sure you would like to delete this action?')) {
      top.Ts.System.logAction('Ticket - Action Deleted');
      var actionDiv = $(this).parents('.ticket-action');
      var action = actionDiv.data('action');
      top.Ts.Services.Tickets.DeleteAction(action.ActionID, function () { actionDiv.remove(); window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "deleteaction", userFullName); }, function () { alert('There was an error deleting this action.'); });
      $(".ticket-action-order").each(function (index) {
        $(this).html('(' + ($('.ticket-action-order').length - index) + ')');
      });
    }
  });

  $('#divActions').delegate('.ticket-action-pin', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    if (top.Ts.System.User.IsSystemAdmin || top.Ts.System.User.UserCanPinAction) {
      var action = $(this).parents('.ticket-action').data('action');
      top.Ts.System.logAction('Ticket - Action Pin Icon Clicked');
      top.Ts.Services.Tickets.SetActionPinned(action.TicketID, action.ActionID, !$(this).hasClass('ts-icon-pin'),
    function (result) {
      window.location = window.location;
    }, function () {
      alert('There was an error editing this action.');
    });
    }
  });

  $('#divActions').delegate('.tag-link', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.System.logAction('Ticket - Tag Linked From Action');
    top.Ts.MainPage.openTag(top.Ts.Utils.getIdFromElement('tagid', $(this)));
  });

  $('#divActions').delegate('.ticket-action-kb', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var icon = $(this);
    var actionDiv = $(this).parents('.ticket-action');
    var action = actionDiv.data('action');
    if (top.Ts.System.User.ChangeKbVisibility || top.Ts.System.User.IsSystemAdmin) {
      top.Ts.System.logAction('Ticket - Action KB Icon Clicked');
      top.Ts.Services.Tickets.SetActionKb(action.ActionID, !$(this).hasClass('ts-icon-kb'),
    function (result) {
      if (result == true) {
        icon.removeClass('ts-icon-kbnot').addClass('ts-icon-kb');
        icon.prop('title', 'Remove Knowledgebase Article');
      }
      else {
        icon.removeClass('ts-icon-kb').addClass('ts-icon-kbnot');
        icon.prop('title', 'Make Knowledgebase Article');
      }
    }, function () {
      alert('There was an error editing this action.');
    });
    }
  });

  $('#divActions').delegate('.ticket-action-portal', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    if (top.Ts.System.User.ChangeTicketVisibility || top.Ts.System.User.IsSystemAdmin) {
      var icon = $(this);
      var actionDiv = $(this).parents('.ticket-action');
      var action = actionDiv.data('action');
      top.Ts.System.logAction('Ticket - Action Visible Icon Clicked');
      top.Ts.Services.Tickets.SetActionPortal(action.ActionID, !$(this).hasClass('ts-icon-portal'),
  function (result) {
    if (result == true) {
      icon.removeClass('ts-icon-portalnot').addClass('ts-icon-portal');
      confirmVisibleToCustomers();
      actionDiv.remove();
      loadTicket(_ticketNumber, 0);
    }
    else {
      icon.removeClass('ts-icon-portal').addClass('ts-icon-portalnot');
      actionDiv.remove();
      loadTicket(_ticketNumber, 0);
    }
  }, function () {
    alert('There was an error editing this action.');
  });
    }

  });

  $('#divActions').delegate('.ticket-action-expand', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var icon = $(this).find('.ticket-action-expand-icon');
    var actionDiv = $(this).parents('.ticket-action');
    var action = actionDiv.data('action');

    if (icon.hasClass('ui-icon-triangle-1-e')) {
      actionDiv.find('.ticket-action-body').show();
      icon.removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');
      if (action.Description == null) {
        top.Ts.Services.Tickets.GetActionInfo(action.ActionID, function (actionInfo) {
          loadActionDisplay(actionDiv, actionInfo, false);

        });

      }
    }
    else {
      actionDiv.find('.ticket-action-body').hide();
      icon.removeClass('ui-icon-triangle-1-s').addClass('ui-icon-triangle-1-e');
    }

  });

  $('#ticketName')
  .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
  .click(function (e) {
    e.preventDefault();
    var header = $(this).parent().hide();
    var container = $('<div>')
    .addClass('ticket-name-edit')
    .css('marginTop', '1em')
    .insertAfter(header);

    $('<input type="text">')
    .addClass('ui-widget-content ui-corner-all')
    .val($(this).text() === '[Untitled Ticket]' ? '' : $(this).text())
    .appendTo(container)
    .focus();

    $('<span>')
    .addClass('ts-icon ts-icon-save')
    .click(function (e) {
      $(this).closest('div').remove();
      header.show().find('img').show();
      top.Ts.Services.Tickets.SetTicketName(_ticketID, $(this).prev().val(), function (result) {
        top.Ts.System.logAction('Ticket - Renamed');
        header.show().find('img').hide().next().show().delay(800).fadeOut(400);
        $('#ticketName').html(result);
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeticketname", userFullName);
      },
    function (error) {
      header.show().find('img').hide();
      alert('There was an error saving the ticket name.');
    });
    })
    .appendTo(container)

    $('<span>')
    .addClass('ts-icon ts-icon-cancel')
    .click(function (e) {
      $(this).closest('div').remove();
      header.show();
    })
    .appendTo(container)
  });

  $('#isTicketPortal')
    .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
    .click(function (e) {
      e.preventDefault();
      if (top.Ts.System.User.ChangeTicketVisibility || top.Ts.System.User.IsSystemAdmin) {
        var item = $(this);
        item.next().show();
        top.Ts.System.logAction('Ticket - Visibility Changed');
        top.Ts.Services.Tickets.SetIsVisibleOnPortal(_ticketID, (item.text() !== 'Yes'),
          function (result) {
            item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
            window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeisportal", userFullName);
          },
          function (error) {
            alert('There was an error saving the ticket portal visible\'s status.');
            item.next().hide();
          });
      }

    });
  $('#isTicketKB')
    .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
    .click(function (e) {
      e.preventDefault();
      if (top.Ts.System.User.ChangeKbVisibility || top.Ts.System.User.IsSystemAdmin) {
        var item = $(this);
        item.next().show();
        top.Ts.System.logAction('Ticket - KB Status Changed');
        top.Ts.Services.Tickets.SetIsKB(_ticketID, (item.text() !== 'Yes'),
        function (result) {
          item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
          if (result === true) {
            $('#knowledgeBaseCategoryDiv').show();
          }
          else {
            $('#knowledgeBaseCategoryDiv').hide();
          }
          window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeiskb", userFullName);
        },
        function (error) {
          alert('There was an error saving the ticket knowlegdgebase\'s status.');
          item.next().hide();
        });
      }
    });
  $('#knowledgeBaseCategoryAnchor')
  .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
  .click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    if (top.Ts.System.User.ChangeKbVisibility || top.Ts.System.User.IsSystemAdmin) {
      removeComboBoxes();
      var parent = $(this).closest('.ticket-name-value').hide();
      var container = $('<div>').addClass('ticket-combobox').insertAfter(parent);
      var select = $('<select>').appendTo(container);

      var option = $('<option>').text('Unassigned').appendTo(select).data('o', null);

      var categories = top.Ts.Cache.getKnowledgeBaseCategories();
      for (var i = 0; i < categories.length; i++) {
        var cat = categories[i].Category;
        option = $('<option>').text(cat.CategoryName).appendTo(select).data('o', cat).data('c', cat.CategoryName);
        if ($(this).text() === cat.CategoryName) { option.attr('selected', 'selected'); }

        for (var j = 0; j < categories[i].Subcategories.length; j++) {
          var sub = categories[i].Subcategories[j];
          var optionSub = $('<option>').text(cat.CategoryName + ' -> ' + sub.CategoryName).appendTo(select).data('o', sub).data('c', cat.CategoryName + ' -> ' + sub.CategoryName);
          if ($(this).text() === sub.CategoryName) { option.attr('selected', 'selected'); }
        }
      }

      select.combobox({
        selected: function (e, ui) {
          parent.show().find('img').show();
          var category = $(ui.item).data('o');
          var categoryString = $(ui.item).data('c');
          top.Ts.System.logAction('Ticket - KnowledgeBase Community Changed');
          top.Ts.Services.Tickets.SetTicketKnowledgeBaseCategory(_ticketID, category == null ? null : category.CategoryID, function (result) {
            $('#knowledgeBaseCategoryAnchor').text(result == null ? 'Unassigned' : categoryString);
            parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
            window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changekbcat", userFullName);
          },
        function (error) {
          parent.show().find('img').hide();
          alert('There was an error setting your ticket knowledgebase category.');
        });
          container.remove();
        },
        close: function (e, ui) {
          removeComboBoxes();
        }
      });
      select.combobox('search', '');
    }
  });

  $('#ticketType')
  .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
  .click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    removeComboBoxes();
    var parent = $(this).closest('.ticket-name-value').hide();
    var container = $('<div>').addClass('ticket-combobox').insertAfter(parent);
    var select = $('<select>').appendTo(container);
    var types = top.Ts.Cache.getTicketTypes();
    if (top.Ts.System.Organization.UseProductFamilies && _productFamilyID != null) {
      for (var i = 0; i < types.length; i++) {
        if (types[i].ProductFamilyID == null || _productFamilyID == types[i].ProductFamilyID) {
          var option = $('<option>').text(types[i].Name).appendTo(select).data('type', types[i]);
          if ($(this).text() === types[i].Name) {
            option.attr('selected', 'selected');
          }
        }
      }

      if (select[0].childElementCount == 0) {
        parent.show().find('img').hide();
        container.remove();
        alert('There are no ticket types available for this product line. Please contact your TeamSupport administrator.');
      }
    }
    else {
      for (var i = 0; i < types.length; i++) {
        var option = $('<option>').text(types[i].Name).appendTo(select).data('type', types[i]);
        if ($(this).text() === types[i].Name) {
          option.attr('selected', 'selected');
        }
      }
    }

    select.combobox({
      selected: function (e, ui) {
        parent.show().find('img').show();
        var type = $(ui.item).data('type');
        top.Ts.System.logAction('Ticket - Type Changed');
        top.Ts.Services.Tickets.SetTicketType(_ticketID, type.TicketTypeID, function (result) {
          if (result !== null) {
            $('#ticketType').text(type.Name);
            _ticketTypeID = type.TicketTypeID;
            $('#ticketStatus')
            .text(result[0].Name)
            .toggleClass('ticket-closed', result[0].IsClosed)
            .data('ticketStatusID', result[0].TicketStatusID);
            appendCustomValues(result[1]);
            parent.show().find('img').hide().next().show().delay(800).fadeOut(400);

            top.Ts.Services.Admin.GetIsJiraLinkActiveForTicket(_ticketID, function (result) {
              if (result) {
                $('#enterIssueKey').hide();
                $('.ticket-widget-jira').show();

                top.Ts.Services.Tickets.GetLinkToJiraSync(_ticketID, function (result) {
                  if (result != null) {
                    if (!result.JiraKey) {
                      $('#issueKeyValue').text('Pending...');
                    }
                    else if (!resultLinkToJira.JiraLinkURL) {
                      $('#issueKeyValue').text(resultLinkToJira.JiraKey);
                      if (inforesult.JiraKey.indexOf('Error') > -1) {
                        $('#issueKeyValue').addClass('nonrequired-field-error ui-corner-all');
                      }
                      else {
                        $('#issueKeyValue').removeClass('nonrequired-field-error ui-corner-all');
                      }
                    }
                    else {
                      var jiraLink = $('<a>')
                        .attr('href', resultLinkToJira.JiraLinkURL)
                        .attr('target', '_blank')
                        .text(result.JiraKey)
                        .addClass('value ui-state-default ts-link')
                        .appendTo($('#issueKeyValue').parent())
                    }

                    $('#issueKey').show();
                    $('.ts-jira-buttons-container').hide();
                  }
                  else {
                    $('#issueKey').hide();
                    $('.ts-jira-buttons-container').show();
                  }
                });
              }
              else {
                $('.ticket-widget-jira').hide();
              }
            });

            window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changetype", userFullName);
          }
          else {
            parent.show().find('img').hide();
          }
        },
  function (error) {
    parent.show().find('img').hide();
    alert('There was an error setting your ticket type.');
  });
        container.remove();
      },
      close: function (e, ui) {
        removeComboBoxes();
      }
    });
    select.combobox('search', '');
  });

  $('#ticketStatus')
  .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
  .click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    removeComboBoxes();
    var id = $(this).data('ticketStatusID');
    var parent = $(this).closest('.ticket-name-value').hide();
    var container = $('<div>').addClass('ticket-combobox').insertAfter(parent);
    var select = $('<select>').appendTo(container);
    var statuses = top.Ts.Cache.getNextStatuses(id);
    for (var i = 0; i < statuses.length; i++) {
      var option = $('<option>').text(statuses[i].Name).appendTo(select).data('status', statuses[i]);
      if ($(this).text() === statuses[i].Name) {
        option.attr('selected', 'selected');
      }
    }

    select.combobox({
      selected: function (e, ui) {
        parent.show().find('img').show();
        var status = $(ui.item).data('status');
        isFormValidToClose(status.IsClosed, function (isValid) {
          if (isValid == true) {
            top.Ts.Services.Tickets.SetTicketStatus(_ticketID, status.TicketStatusID, function (result) {
              if (result !== null) {
                $('#ticketStatus')
    .text(result.Name)
    .toggleClass('ticket-closed', result.IsClosed)
    .data('ticketStatusID', result.TicketStatusID);
                parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
                top.Ts.System.logAction('Ticket - Status Changed');
                window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changestatus", userFullName);
              }
              else {
                parent.show().find('img').hide();
              }
            },
  function (error) {
    parent.show().find('img').hide();
    alert('There was an error setting your ticket status.');
  });
            container.remove();
          }
          else {
            alert("Please fill in the required fields before closing the ticket.");
            parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
            return;
          }
        },
  function (error) {
    parent.show().find('img').hide();
    alert('There was an error validating your ticket status change.');
  });
      },
      close: function (e, ui) {
        removeComboBoxes();
      }
    });
    select.combobox('search', '');
  });

  $('#ticketSeverity')
  .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
  .click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    removeComboBoxes();
    var parent = $(this).closest('.ticket-name-value').hide();
    var container = $('<div>').addClass('ticket-combobox').insertAfter(parent);
    var select = $('<select>').appendTo(container);
    var severities = top.Ts.Cache.getTicketSeverities();
    for (var i = 0; i < severities.length; i++) {
      var option = $('<option>').text(severities[i].Name).appendTo(select).data('severity', severities[i]);
      if ($(this).text() === severities[i].Name) {
        option.attr('selected', 'selected');
      }
    }

    select.combobox({
      selected: function (e, ui) {
        parent.show().find('img').show();
        var severity = $(ui.item).data('severity');
        top.Ts.System.logAction('Ticket - Severity Changed');
        top.Ts.Services.Tickets.SetTicketSeverity(_ticketID, severity.TicketSeverityID, function (result) {
          if (result !== null) {
            $('#ticketSeverity').text(result.Name);
            parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
            window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeseverity", userFullName);

            loadTicket(_ticketNumber, 0);
          }
          else {
            parent.show().find('img').hide();
          }
        },
  function (error) {
    parent.show().find('img').hide();
    alert('There was an error setting your ticket severity.');
  });
        container.remove();
      },
      close: function (e, ui) {
        removeComboBoxes();
      }
    });
    select.combobox('search', '');
  });


  $('#ticketCommunity')
  .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
  .click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    if (!top.Ts.System.User.CanChangeCommunityVisibility) {
      return false;
    }
    removeComboBoxes();
    var parent = $(this).closest('.ticket-name-value').hide();
    var container = $('<div>').addClass('ticket-combobox').insertAfter(parent);
    var select = $('<select>').appendTo(container);

    var option = $('<option>').text('Unassigned').appendTo(select).data('o', null);

    var categories = top.Ts.Cache.getForumCategories();
    for (var i = 0; i < categories.length; i++) {
      var cat = categories[i].Category;
      //option = $('<option>').text(cat.CategoryName).appendTo(select).data('o', cat);
      //if ($(this).text() === cat.CategoryName) { option.attr('selected', 'selected'); }

      for (var j = 0; j < categories[i].Subcategories.length; j++) {
        var sub = categories[i].Subcategories[j];
        var optionSub = $('<option>').text(cat.CategoryName + ' -> ' + sub.CategoryName).appendTo(select).data('o', sub).data('c', cat.CategoryName + ' -> ' + sub.CategoryName);
        if ($(this).text() === sub.CategoryName) { option.attr('selected', 'selected'); }
      }
    }

    select.combobox({
      selected: function (e, ui) {
        parent.show().find('img').show();
        var category = $(ui.item).data('o');
        var categoryString = $(ui.item).data('c');
        top.Ts.System.logAction('Ticket - Community Changed');
        top.Ts.Services.Tickets.SetTicketCommunity(_ticketID, category == null ? null : category.CategoryID, $('#ticketCommunity').text(), category == null ? 'Unassigned' : categoryString, function (result) {
          $('#ticketCommunity').text(result == null ? 'Unassigned' : categoryString);
          parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
          window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecommunity", userFullName);
        },
  function (error) {
    parent.show().find('img').hide();
    alert('There was an error setting your ticket community.');
  });
        container.remove();
      },
      close: function (e, ui) {
        removeComboBoxes();
      }
    });
    select.combobox('search', '');
  });

  $('#dueDate')
  .addClass('value ui-state-default ts-link')
  .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
  .click(function (e) {
    e.preventDefault();
    var parent = $(this).parent().hide();
    var container = $('<div>')
      .css('marginTop', '1em')
      .insertAfter(parent);
    var input = $('<input type="text">')
        .addClass('ui-widget-content ui-corner-all ticket-cutstom-edit-text-input')
        .css('width', '100%')
        .appendTo(container)
        .datetimepicker({ dateFormat: dateFormat })
        .focus();

    if (_dueDate != null) {
      input.datetimepicker('setDate', top.Ts.Utils.getMsDate(_dueDate));
    }

    var buttons = $('<div>')
      .addClass('ticket-custom-edit-buttons')
      .appendTo(container);

    $('<button>')
      .text('Cancel')
      .click(function (e) {
        parent.show();
        container.remove();
      })
      .appendTo(buttons)
      .button();

    $('<button>')
      .text('Save')
      .click(function (e) {
        parent.show().find('img').show();
        var value = '';

        if (input.val()) {
          if (dateFormat.indexOf("m") != 0) {
            var dateArr = input.val().replace(/\./g, '/').replace(/-/g, '/').split('/');
            if (dateFormat.indexOf("d") == 0)
              var day = dateArr[0];
            if (dateFormat.indexOf("y") == 0)
              var year = dateArr[0];
            if (dateFormat.indexOf("m") == 3 || dateFormat.indexOf("m") == 5)
              var month = dateArr[1];

            var timeSplit = dateArr[2].split(' ');
            if (dateFormat.indexOf("y") == 6)
              var year = timeSplit[0];
            else
              var day = timeSplit[0];

            var theTime = timeSplit[1];

            var formattedDate = month + "/" + day + "/" + year + " " + theTime + (timeSplit[2] != null ? " " + timeSplit[2] : "");
            value = top.Ts.Utils.getMsDate(formattedDate);
          }
          else
            value = top.Ts.Utils.getMsDate(input.val());
        }
        container.remove();
        //how can i check if the date is in the past
        //              if (field.IsRequired && (value === null || $.trim(value) === '')) {
        //                item.addClass('nonrequired-field-error ui-corner-all');
        //                var anchor = item.find('a');
        //                anchor.addClass('nonrequired-field-error-font');
        //              }
        //need to test what is this for apparently makes no sense
        //              if (value === null || $.trim(value) === '') {
        //                result.parent().addClass('is-empty');
        //              }
        //              else {
        //                result.parent().removeClass('is-empty');
        //              }
        top.Ts.Services.Tickets.SetDueDate(_ticketID, value, function (result) {
          parent.find('img').hide().next().show().delay(800).fadeOut(400);
          var date = result === null ? null : top.Ts.Utils.getMsDate(result);
          var anchor = parent.find('a');
          anchor.text((date === null ? 'Unassigned' : value.localeFormat(top.Ts.Utils.getDateTimePattern())))
          _dueDate = top.Ts.Utils.getMsDate(value); //result;
          if (date != null && date < Date.now()) {
            parent.addClass('nonrequired-field-error ui-corner-all');
            anchor.addClass('nonrequired-field-error-font');
          }
          else {
            parent.removeClass('nonrequired-field-error ui-corner-all');
            anchor.removeClass('nonrequired-field-error-font');
          }
          window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeduedate", userFullName);
        }, function () {
          alert("There was a problem saving your ticket property.");
        });
      })
      .appendTo(buttons)
      .button();
  });

  $('#userName')
  .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
  .click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    removeComboBoxes();
    var parent = $(this).closest('.ticket-name-value').hide();
    var container = $('<div>').addClass('ticket-combobox').insertAfter(parent);
    var select = $('<select>').appendTo(container);
    var users = top.Ts.Cache.getUsers();
    var unassigned = new Object();
    unassigned.UserID = null;
    unassigned.Name = "Unassigned";
    var option = $('<option>').text(unassigned.Name).appendTo(select).data('user', unassigned);
    if ($(this).text() === unassigned.Name) {
      option.attr('selected', 'selected');
    }

    var senderAdded = false;

    if (_ticketSender != null) {
      var senderSuffix = ' (Sender)';

      if (_ticketCreator.Name == _ticketSender.Name) {
        senderSuffix = ' (Sender and Creator)';
      }

      var sender = new Object();
      sender.UserID = _ticketSender.UserID;
      sender.Name = _ticketSender.Name;
      sender.InOfficeComment = '';

      var senderInUsers = new Array();
      for (var i = 0; i < users.length; i++) {
        if (users[i].UserID == _ticketSender.UserID) {
          senderInUsers[0] = users[i];
          break;
        }
      }

      if (senderInUsers.length > 0) {
        sender.InOfficeComment = senderInUsers[0].InOfficeComment;

        var senderInOfficeComment = '';
        if (sender.InOfficeComment) {
          senderInOfficeComment = ' - ' + sender.InOfficeComment;
        }
        $('<option>').text(sender.Name + senderInOfficeComment + senderSuffix).appendTo(select).data('user', sender);
        senderAdded = true;
      }
    }

    var creatorAdded = false;

    if (_ticketCreator.UserID > 0 && _ticketCreator.Name != $(this).text() && (_ticketSender == null || _ticketCreator.Name != _ticketSender.Name)) {
      var creator = new Object();
      creator.UserID = _ticketCreator.UserID;
      creator.Name = _ticketCreator.Name;
      creator.InOfficeComment = '';

      var creatorInUsers = new Array();
      for (var i = 0; i < users.length; i++) {
        if (users[i].UserID == _ticketCreator.UserID) {
          creatorInUsers[0] = users[i];
          break;
        }
      }

      if (creatorInUsers.length > 0) {
        creator.InOfficeComment = creatorInUsers[0].InOfficeComment;

        var creatorInOfficeComment = '';
        if (creator.InOfficeComment) {
          creatorInOfficeComment = ' - ' + creator.InOfficeComment;
        }
        $('<option>').text(creator.Name + creatorInOfficeComment + ' (Creator)').appendTo(select).data('user', creator);
        creatorAdded = true;
      }
    }

    if (top.Ts.System.Organization.ShowGroupMembersFirstInTicketAssignmentList && _ticketGroupUsers != null) {
      for (var i = 0; i < _ticketGroupUsers.length; i++) {
        // If it has not been added previously as sender or creator
        if ((!senderAdded || _ticketSender.Name != _ticketGroupUsers[i].Name) && (!creatorAdded || _ticketCreator.Name != _ticketGroupUsers[i].Name)) {
          var inOfficeComment = '';
          if (_ticketGroupUsers[i].InOfficeComment) {
            inOfficeComment = ' - ' + _ticketGroupUsers[i].InOfficeComment;
          }
          var option = $('<option>').text(_ticketGroupUsers[i].Name + inOfficeComment).appendTo(select).data('user', _ticketGroupUsers[i]);
          groupUserAdded = true;
          if ($(this).text() === _ticketGroupUsers[i].Name) {
            option.attr('selected', 'selected');
          }
        }
      }
    }

    for (var i = 0; i < users.length; i++) {
      // If it has not been added previously as sender or creator
      if ((!senderAdded || _ticketSender.Name != users[i].Name) && (!creatorAdded || _ticketCreator.Name != users[i].Name)) {
        // If it has not been added previously as group user
        if (_ticketGroupUsers === null || !top.Ts.System.Organization.ShowGroupMembersFirstInTicketAssignmentList || !checkIfUserExistsInArray(users[i], _ticketGroupUsers)) {
          var inOfficeComment = '';
          if (users[i].InOfficeComment) {
            inOfficeComment = ' - ' + users[i].InOfficeComment;
          }
          var option = $('<option>').text(users[i].Name + inOfficeComment).appendTo(select).data('user', users[i]);
          if ($(this).text() === users[i].Name) {
            option.attr('selected', 'selected');
          }
        }
      }
    }

    select.combobox({
      selected: function (e, ui) {
        parent.show().find('img').show();
        _ticketSender = new Object();
        _ticketSender.UserID = top.Ts.System.User.UserID;
        _ticketSender.Name = userFullName;
        var user = $(ui.item).data('user');
        top.Ts.System.logAction('Ticket - Assignment Changed');
        top.Ts.Services.Tickets.SetTicketUser(_ticketID, user.UserID, function (userInfo) {
          top.Ts.System.logAction('Ticket - Assignment Change Completed');
          setUserName(userInfo);
          parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
          window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeassigned", userFullName);
        },
  function (error) {
    top.Ts.System.logAction('Ticket - Assignment Change NOT-Completed');
    parent.show().find('img').hide();
    alert('There was an error setting the user.');
  });
        container.remove();
      },
      close: function (e, ui) {
        removeComboBoxes();
      }
    });
    select.combobox('search', '');
  });

  $('#ticketGroup')
  .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
  .click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    var self = $(this);

    top.Ts.Services.Tickets.GetTicketGroups(_ticketID, function (result) {
      removeComboBoxes();
      var parent = self.closest('.ticket-name-value').hide();
      var container = $('<div>').addClass('ticket-combobox').insertAfter(parent);
      var groups;
      groups = result;
      var unassigned = new Object();
      unassigned.GroupID = null;
      unassigned.Name = "Unassigned";
      var select = $('<select>').appendTo(container);
      var option = $('<option>').text(unassigned.Name).appendTo(select).data('group', unassigned);
      if (self.text() === unassigned.Name) {
        option.attr('selected', 'selected');
      }
      for (var i = 0; i < groups.length; i++) {
        var option = $('<option>').text(groups[i].Name).appendTo(select).data('group', groups[i]);
        if (self.text() === groups[i].Name) {
          option.attr('selected', 'selected');
        }
      }
      select.combobox({
        selected: function (e, ui) {
          parent.show().find('img').show();
          var group = $(ui.item).data('group');
          top.Ts.System.logAction('Ticket - Group Changed');
          top.Ts.Services.Tickets.SetTicketGroup(_ticketID, group.GroupID, function (result) {
            if (result !== null) {
              $('#ticketGroup').text(result == "" ? 'Unassigned' : result);
              parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
              window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changegroup", userFullName);
              _ticketGroupID = group.GroupID;
              if (_ticketGroupID != null) {
                top.Ts.Services.Users.GetGroupUsers(_ticketGroupID, function (result) {
                  _ticketGroupUsers = result;
                });
              }
              else {
                _ticketGroupUsers = null;
              }
            }
            else {
              parent.show().find('img').hide();
            }
            if (top.Ts.System.Organization.UpdateTicketChildrenGroupWithParent) {
              top.Ts.Services.Tickets.SetTicketChildrenGroup(_ticketID, group.GroupID);
            }
          },
    function (error) {
      parent.show().find('img').hide();
      alert('There was an error setting the group.');
    });
          container.remove();
        },
        close: function (e, ui) {
          removeComboBoxes();
        }
      });
      select.combobox('search', '');
    });



  });
  //    $('#ticketGroup')
  //    .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
  //    .click(function (e) {
  //        e.preventDefault();
  //        e.stopPropagation();
  //        removeComboBoxes();
  //        var parent = $(this).closest('.ticket-name-value').hide();
  //        var container = $('<div>').addClass('ticket-combobox').insertAfter(parent);
  //        var select = $('<select>').appendTo(container);
  //        var groups = top.Ts.Cache.getTicketGroups();

  //        var unassigned = new Object();
  //        unassigned.GroupID = null;
  //        unassigned.Name = "Unassigned";
  //        var option = $('<option>').text(unassigned.Name).appendTo(select).data('group', unassigned);
  //        if ($(this).text() === unassigned.Name) {
  //            option.attr('selected', 'selected');
  //        }
  //        for (var i = 0; i < groups.length; i++) {
  //            var option = $('<option>').text(groups[i].Name).appendTo(select).data('group', groups[i]);
  //            if ($(this).text() === groups[i].Name) {
  //                option.attr('selected', 'selected');
  //            }
  //        }

  //        select.combobox({
  //            selected: function (e, ui) {
  //                parent.show().find('img').show();
  //                var group = $(ui.item).data('group');
  //                top.Ts.System.logAction('Ticket - Group Changed');
  //                top.Ts.Services.Tickets.SetTicketGroup(_ticketID, group.GroupID, function (result) {
  //                    if (result !== null) {
  //                        $('#ticketGroup').text(result == "" ? 'Unassigned' : result);
  //                        parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
  //                        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changegroup", userFullName);
  //                        _ticketGroupID = group.GroupID;
  //                        if (_ticketGroupID != null) {
  //                            top.Ts.Services.Users.GetGroupUsers(_ticketGroupID, function (result) {
  //                                _ticketGroupUsers = result;
  //                            });
  //                        }
  //                        else {
  //                            _ticketGroupUsers = null;
  //                        }
  //                    }
  //                    else {
  //                        parent.show().find('img').hide();
  //                    }
  //                    if (top.Ts.System.Organization.UpdateTicketChildrenGroupWithParent) {
  //                        top.Ts.Services.Tickets.SetTicketChildrenGroup(_ticketID, group.GroupID);
  //                    }
  //                },
  //          function (error) {
  //              parent.show().find('img').hide();
  //              alert('There was an error setting the group.');
  //          });
  //                container.remove();
  //            },
  //            close: function (e, ui) {
  //                removeComboBoxes();
  //            }
  //        });
  //        select.combobox('search', '');
  //    });

  $('#product')
  .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
  .click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    removeComboBoxes();
    top.Ts.Settings.Organization.read('ShowOnlyCustomerProducts', false, function (showOnlyCustomers) {
      if (showOnlyCustomers == "True") {
        top.Ts.Services.Tickets.GetTicketCustomerProductIDs(_ticketID, function (ids) {
          loadProducts(ids);
        });
      }
      else {
        loadProducts();

      }
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeproduct", userFullName);
    })
  });

  $('#reported')
  .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
  .click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    removeComboBoxes();
    if ($('#product').data('productID') === null) return;
    var product = top.Ts.Cache.getProduct($('#product').data('productID'));
    if (product === null) return;
    if (product.Versions.length < 1) return;
    var parent = $(this).closest('.ticket-name-value').hide();
    var container = $('<div>').addClass('ticket-combobox').insertAfter(parent);
    var select = $('<select>').appendTo(container);
    var versions = product.Versions;
    var unassigned = new Object();
    unassigned.ProductVersionID = null;
    unassigned.VersionNumber = "Unassigned";
    var option = $('<option>').text(unassigned.VersionNumber).appendTo(select).data('version', unassigned);
    if ($(this).text() === unassigned.VersionNumber) {
      option.attr('selected', 'selected');
    }
    for (var i = 0; i < versions.length; i++) {
      var option = $('<option>').text(versions[i].VersionNumber).appendTo(select).data('version', versions[i]);
      if ($(this).text() === versions[i].VersionNumber) {
        option.attr('selected', 'selected');
      }
    }

    select.combobox({
      selected: function (e, ui) {
        parent.show().find('img').show();
        var version = $(ui.item).data('version');
        top.Ts.System.logAction('Ticket - Reported Version Changed');
        top.Ts.Services.Tickets.SetReportedVersion(_ticketID, version.ProductVersionID, function (result) {
          if (result !== null) {
            setVersion(result.id, result.label, false);
            //$('#reported').text(result === '' ? 'Unassigned' : result);
            parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
            window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changereported", userFullName);
          }
          else {
            parent.show().find('img').hide();
          }
        },
  function (error) {
    parent.show().find('img').hide();
    alert('There was an error setting the reported version.');
  });
        container.remove();
      },
      close: function (e, ui) {
        removeComboBoxes();
      }
    });
    select.combobox('search', '');
  });

  $('#resolved')
  .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
  .click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    removeComboBoxes();
    if ($('#product').data('productID') === null) return;
    var parent = $(this).closest('.ticket-name-value').hide();
    var container = $('<div>').addClass('ticket-combobox').insertAfter(parent);
    var select = $('<select>').appendTo(container);
    var versions = top.Ts.Cache.getProduct($('#product').data('productID')).Versions;
    var unassigned = new Object();
    unassigned.ProductVersionID = null;
    unassigned.VersionNumber = "Unassigned";
    var option = $('<option>').text(unassigned.VersionNumber).appendTo(select).data('version', unassigned);
    if ($(this).text() === unassigned.VersionNumber) {
      option.attr('selected', 'selected');
    }
    for (var i = 0; i < versions.length; i++) {
      var option = $('<option>').text(versions[i].VersionNumber).appendTo(select).data('version', versions[i]);
      if ($(this).text() === versions[i].VersionNumber) {
        option.attr('selected', 'selected');
      }
    }

    select.combobox({
      selected: function (e, ui) {
        parent.show().find('img').show();
        var version = $(ui.item).data('version');
        top.Ts.System.logAction('Ticket - Resolved Version Changed');
        top.Ts.Services.Tickets.SetSolvedVersion(_ticketID, version.ProductVersionID, function (result) {
          if (result !== null) {
            setVersion(result.id, result.label, true);
            //$('#resolved').text(result === '' ? 'Unassigned' : result);
            parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
            window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeresolved", userFullName);
          }
          else {
            parent.show().find('img').hide();
          }
        },
  function (error) {
    parent.show().find('img').hide();
    alert('There was an error setting the resolved version.');
  });
        container.remove();
      },
      close: function (e, ui) {
        removeComboBoxes();
      }
    });
    select.combobox('search', '');
  });

  $('.ticket-jira-sync').hover(function (e) {
    $(this).css('textDecoration', 'underline');
  }, function (e) {
    $(this).css('textDecoration', '');
  });

  $('#savingIssueKeyImg').hide();
  $('#savedIssueKeyImg').hide();

  $('#cancelIssueyKeyButton').click(function (e) {
    $('.ts-jira-buttons-container').show();
    $('#enterIssueKey').hide();
  });

  $('#saveIssueKeyButton').click(function (e) {
    if ($.trim($('#issueKeyInput').val()) === '') {
      $('.ts-jira-buttons-container').show();
      $('#enterIssueKey').hide();
    }
    else {
      $('#issueKeyValue').text($.trim($('#issueKeyInput').val()));
      $('#enterIssueKey').hide();
      $('#issueKey').show();
      $('#savingIssueKeyImg').show();
      var errorMessage = "There was an error setting your Jira Issue Key. Please contact TeamSupport.com";

      top.Ts.Services.Tickets.SetJiraIssueKey(_ticketID, $.trim($('#issueKeyInput').val()), function (result) {
      	if (result != null) {
      		var syncResult = JSON.parse(result);
      		if (syncResult.IsSuccessful === true) {
				$('#savingIssueKeyImg').hide();
				$('#savedIssueKeyImg').show().delay(800).fadeOut(400);;
			}
			else {
				$('.ts-jira-buttons-container').show();
				$('#issueKey').hide();
				alert(syncResult.Error);
			}
      	} else {
      		alert(errorMessage);
      	}
      },
    function (error) {
      $('.ts-jira-buttons-container').show();
      $('#issueKey').hide();
      alert(errorMessage);
    });
    }
  });

  $('#newJiraIssue').click(function (e) {
    e.preventDefault();
    var parent = $(this).parent().hide();
    var errorMessage = "There was an error setting your Jira Issue Key. Please contact TeamSupport.com";
    top.Ts.Services.Tickets.SetSyncWithJira(_ticketID, function (result) {
    	if (result != null) {
    		var syncResult = JSON.parse(result);
    		if (syncResult.IsSuccessful === true) {
				$('#issueKeyValue').text('Pending...');
				$('#issueKey').show();
				$('#issueKey div:first-child').show();
			}
			else {
				$('.ts-jira-buttons-container').show();
				$('#issueKey').hide();
				alert(syncResult.Error);
			}
    	} else {
    		alert(errorMessage);
    	}
    },
  function (error) {
    $('.ts-jira-buttons-container').show();
    $('#issueKey').hide();
    alert('There was an error setting your Jira Issue Key. Please contact TeamSupport.com');
  });
  });

  $('#jiraUnlink').click(function (e) {
    var currentStatus = $("#issueKeyValue").text().toLowerCase();
    var confirmMessage = "Are you sure you want to " + ((currentStatus.indexOf("pending") > -1) ? "cancel" : "remove") + " link to JIRA?";

    if (confirm(confirmMessage)) {
      e.preventDefault();
      $('.ts-jira-buttons-container').show();
      $('#issueKey').hide();
      var parent = $(this).parent().hide();
      top.Ts.Services.Tickets.UnSetSyncWithJira(_ticketID, function (result) {
        if (result === true) {
          //It was successful
        }
        else {
          alert('There was an error setting your Jira Issue Key. Please try again later');
          $('.ts-jira-buttons-container').hide();
          $('#issueKey').show();
        }
      },
function (error) {
  alert('There was an error setting your Jira Issue Key.');
  $('.ts-jira-buttons-container').hide();
  $('#issueKey').show();
});
    }
  });

  $('#existingJiraIssue').click(function (e) {
    e.preventDefault();
    var parent = $(this).parent().hide();
    $('#enterIssueKey').show();
    $('#issueKeyInput').focus();
  });


  function addToolbarButton(id, icon, caption, callback) {
    var html = '<a href="#" id="' + id + '" class="ts-toolbar-button ui-corner-all"><i class="fa ' + icon + '"></i><span class="ts-toolbar-caption">' + caption + '</span></a>';
    $('.ticket-panel-toolbar').append(html).find('#' + id).click(callback).hover(function () { $(this).addClass('ui-state-hover'); }, function () { $(this).removeClass('ui-state-hover'); });
  }

  addToolbarButton('btnRefresh', 'fa-refresh', 'Refresh', function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.System.logAction('Ticket - Refreshed');
    top.Ts.MainPage.highlightTicketTab(_ticketNumber, false);
    window.location = window.location;
  });


  /*
  addToolbarButton('btnLoadOld', 'ts-icon-refresh', 'Load Old', function (e) {
  e.preventDefault();
  e.stopPropagation();
  window.location = '../../Frames/Ticket.aspx?TicketID=' + _ticketID;
  });*/

  if (top.Ts.System.Organization.ProductType == top.Ts.ProductType.Express) {
    $('.ticket-widget-customers').hide();
  }
  if (top.Ts.System.Organization.ProductType == top.Ts.ProductType.Express || top.Ts.System.Organization.ProductType === top.Ts.ProductType.HelpDesk) {
    $('.ticket-widget-products').hide();
  }
  if (top.Ts.System.Organization.UseForums != true) {
    $('#ticketCommunity').closest('.ticket-name-value').hide();
  }


  addToolbarButton('btnOwn', 'fa-anchor', 'Take Ownership', function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.System.logAction('Ticket - Take Ownership');
    top.Ts.Services.Tickets.AssignUser(_ticketID, top.Ts.System.User.UserID, function (userInfo) {
      setUserName(userInfo);
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeassigned", userFullName);
    }, function () {
      alert('There was an error taking ownwership of this ticket.');

    });
  });
  addToolbarButton('btnUpdate', 'fa-bullhorn', 'Get Update', function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.System.logAction('Ticket - Request Update');
    top.Ts.Services.Tickets.RequestUpdate(_ticketID, function (actionInfo) {
      var actionDiv = createActionElement().prependTo('#divActions');
      loadActionDisplay(actionDiv, actionInfo, true);
      alert('An update has been requested for this ticket.');
    }, function () {
      alert('There was an error requesting an update for this ticket.');
    });

  });

  addToolbarButton('btnSubscribe', 'fa-rss', 'Subscribe', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var caption = $(this).find('.ts-toolbar-caption');
    var isSubscribed = caption.text() === 'Unsubscribe';
    caption.text(isSubscribed ? 'Subscribe' : 'Unsubscribe');
    top.Ts.System.logAction('Ticket - Subscribed');
    top.Ts.Services.Tickets.SetSubscribed(_ticketID, !isSubscribed, null, function (subscribers) {
      appendSubscribers(subscribers);
    }, function () {
      alert('There was an error subscribing this ticket.');
    });
  });

  addToolbarButton('btnEnqueue', 'fa-list-ol', 'Enqueue', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var caption = $(this).find('.ts-toolbar-caption');
    var isQueued = caption.text() === 'Dequeue';
    caption.text(isQueued ? 'Enqueue' : 'Dequeue');
    top.Ts.System.logAction('Ticket - Enqueued');
    top.Ts.System.logAction('Queued');
    top.Ts.Services.Tickets.SetQueue(_ticketID, !isQueued, null, function (queues) {
      appendQueues(queues);
    }, function () {
      alert('There was an error queueing this ticket.');
    });
  });

  addToolbarButton('btnFlag', 'fa-flag', 'Flag', function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.System.logAction('Ticket - Flagged');
    var caption = $(this).find('.ts-toolbar-caption');
    if (caption.text() === 'Unflag') {
      top.Ts.Services.Tickets.SetTicketFlag(_ticketID, false, function () {
        caption.text('Flag');
      }, function () {
        alert('There was an error flagging to this ticket.');
      });
    }
    else {
      top.Ts.Services.Tickets.SetTicketFlag(_ticketID, true, function () {
        caption.text('Unflag');
      }, function () {
        alert('There was an error unflagging to this ticket.');
      });
    }
  });

  addToolbarButton('btnPrint', 'fa-print', 'Print', function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.System.logAction('Ticket - Printed');
    window.open('../../../TicketPrint.aspx?ticketid=' + _ticketID, 'TSPrint' + _ticketID);
  });

  addToolbarButton('btnEmail', 'fa-envelope', 'Email', function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.System.logAction('Ticket - Emailed');
    $(".dialog-emailinput").dialog('open');
  });

  addToolbarButton('btnUrl', 'fa-lightbulb-o', 'Url', function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.System.logAction('Ticket - Shown URL');
    $('.ticket-url').toggle();
  });

  addToolbarButton('btnMerge', 'fa-exchange', 'Merge', function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.System.logAction('Ticket - Merge');
    clearDialog();
    $(".dialog-ticketmerge").dialog('open');

  });

  if (top.Ts.System.User.IsSystemAdmin || top.Ts.System.User.UserID === _ticketCreator.UserID) {
    addToolbarButton('btnDelete', 'fa-trash-o', 'Delete', function (e) {
      e.preventDefault();
      e.stopPropagation();
      if (confirm('Are you sure you would like to delete this ticket?')) {
        top.Ts.System.logAction('Ticket - Deleted');
        top.Ts.Services.Tickets.DeleteTicket(_ticketID, function () {
          top.Ts.MainPage.closeTicketTab(_ticketNumber);
          window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "delete", userFullName);
        }, function () {
          alert('There was an error deleting this ticket.');

        });
      }
    });
  }

  loadTicket(top.Ts.Utils.getQueryValue('TicketNumber', window));

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
    top.Ts.System.logAction('Ticket - New Customer Added');
    var email = $('.ticket-new-customer-email').val();
    var firstName = $('.ticket-new-customer-first').val();
    var lastName = $('.ticket-new-customer-last').val();
    var phone = $('.ticket-new-customer-phone').val();
    var companyName = $('.ticket-new-customer-company').val();
    top.Ts.Services.Users.CreateNewContact(email, firstName, lastName, companyName, phone, false, function (result) {
      if (result.indexOf("u") == 0 || result.indexOf("o") == 0) {
        top.Ts.Services.Tickets.AddTicketCustomer(_ticketID, result.charAt(0), result.substring(1), function (result) {
          appendCustomers(result);
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
              top.Ts.Services.Tickets.AddTicketCustomer(_ticketID, result.charAt(0), result.substring(1), function (result) {
                appendCustomers(result);
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

  $('button').button();

  $('#calendarLink').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    if ($('#calendarIframe').attr('src') == "")
      $('#calendarIframe').attr("src", "Calendar.html?pagetype=0&pageid=" + _ticketNumber);
    top.Ts.System.logAction('Ticket - Calendar Clicked');
    $('#divCalendar').show();
    $('#divWaterCooler').hide();
    $('#divActions').hide();
    $('#divFooter').hide();
    $('#calendarLink').addClass("activelink");
    $('#watercoolerLink').removeClass("activelink");
    $('#actionsLink').removeClass("activelink");
    $('.ticket-action-add').hide();
  });

  $('#watercoolerLink').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.System.logAction('Ticket - WC Clicked');
    $('#divCalendar').hide();
    $('#divWaterCooler').show();
    $('#divActions').hide();
    $('#divFooter').hide();
    $('#calendarLink').removeClass("activelink");
    $('#watercoolerLink').addClass("activelink");
    $('#actionsLink').removeClass("activelink");
    $('.ticket-action-add').toggle();
  });
  $('#actionsLink').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#divCalendar').hide();
    $('#divWaterCooler').hide();
    $('#divActions').show();
    $('#divFooter').show();
    $('#calendarLink').removeClass("activelink");
    $('#watercoolerLink').removeClass("activelink");
    $('#actionsLink').addClass("activelink");
    $('.ticket-action-add').show();
  });

  $('.ticket-panel-content').bind('scroll', function () {
    if ($(this).scrollTop() > 100) {
      $('.scrollup').fadeIn();
    } else {
      $('.scrollup').fadeOut();
    }
  });

  $('.scrollup').click(function () {
    $('.ticket-panel-content').animate({ scrollTop: 0 }, 600);
    return false;
  });

  $('#alertSnooze').click(function (e) {
    top.Ts.Services.Customers.SnoozeAlert($('#alertID').val(), $('#alertType').val());
    $('#modalAlert').modal('hide');
  });

  $('#alertDismiss').click(function (e) {
    top.Ts.Services.Customers.DismissAlert($('#alertID').val(), $('#alertType').val());
    $('#modalAlert').modal('hide');
  });

});



//customers
var appendCustomers = function (customers) {
  $('#divCustomers').empty().append('<div class="ts-separator ui-widget-content">');

  for (var i = 0; i < customers.length; i++) {
    appendCustomer(customers[i]);
  }

}

var appendCustomer = function (customer) {debugger
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
        .attr('rel', '../../../Tips/User.aspx?UserID=' + customer.UserID + '&TicketID=' + _ticketID)
        .cluetip(clueTipOptions)
        .appendTo(title);

    var desc = $('<div>').addClass('ticket-removable-item-description').appendTo(item);
    $('<a>')
          .attr('href', '#')
          .addClass('ui-state-default ts-link')
          .click(function (e) {
            e.preventDefault();
            top.Ts.MainPage.openNewCustomer(customer.OrganizationID);
          })
          .text(ellipseString(customer.Company, 30))
          .appendTo(desc);
    $('<span>')
        .addClass('ts-icon ts-icon-info')
        .attr('rel', '../../../Tips/Customer.aspx?CustomerID=' + customer.OrganizationID + '&TicketID=' + _ticketID)
        .cluetip(clueTipOptions)
        .appendTo(desc);

  }
  else {
    $('<span>').addClass('ui-icon ui-icon-close').appendTo(item);
    var title = $('<div>').addClass('ticket-removable-item-title').appendTo(item);
    $('<a>')
          .attr('href', '#')
          .addClass('ui-state-default ts-link')
          .click(function (e) {
            e.preventDefault();
            top.Ts.MainPage.openNewCustomer(customer.OrganizationID);
          })
          .text(ellipseString(customer.Company, 30))
          .appendTo(title);

    $('<span>')
        .addClass('ts-icon ts-icon-info')
        .attr('rel', '../../../Tips/Customer.aspx?CustomerID=' + customer.OrganizationID + '&TicketID=' + _ticketID)
        .cluetip(clueTipOptions)
        .appendTo(title);

  }

  if (customer.Flag) {
    item.addClass('nonrequired-field-error ui-corner-all');
    var anchor = item.find('a');
    anchor.addClass('nonrequired-field-error-font');
  }
  $('#divCustomers').append(item);

}

var selectTicket = function (request, response) {
  if (execSelectTicket) { execSelectTicket._executor.abort(); }
  var filter = $(this.element).data('filter');
  if (filter === undefined) {
    execSelectTicket = top.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) { response(result); });
  }
  else {
    execSelectTicket = top.Ts.Services.Tickets.SearchTickets(request.term, filter, function (result) { response(result); });
  }
}

var getCustomers = function (request, response) {
  if (execGetCustomer) { execGetCustomer._executor.abort(); }
  execGetCustomer = top.Ts.Services.Organizations.GetUserOrOrganizationForTicket(request.term, function (result) { response(result); });
}

//assets
var appendAssets = function (assets) {
  $('#divAssets').empty().append('<div class="ts-separator ui-widget-content">');
  for (var i = 0; i < assets.length; i++) {
    appendAsset(assets[i]);
  }
}

var appendAsset = function (asset) {
  var item = $('<div>')
    .addClass('ticket-removable-item ui-corner-all ts-color-bg-accent ticket-asset')
    .data('data', asset);

  $('<span>').addClass('ui-icon ui-icon-close').appendTo(item);

  var title = $('<div>').addClass('ticket-removable-item-title').appendTo(item);
  $('<a>')
      .attr('href', '#')
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
  $('#divAssets').append(item);

}

var getAssets = function (request, response) {
  if (execGetAsset) { execGetAsset._executor.abort(); }
  execGetAsset = top.Ts.Services.Assets.FindAsset(request.term, function (result) { response(result); });
}

//subscribers
var appendSubscribers = function (subscribers) {
  $('#divSubscribers').empty().append('<div class="ts-separator ui-widget-content">');

  for (var i = 0; i < subscribers.length; i++) {
    appendSubscriber(subscribers[i]);
  }
}

var appendSubscriber = function (subscriber) {
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
      top.Ts.MainPage.openUser(subscriber.UserID);
    })
    .text(ellipseString(subscriber.FirstName + ' ' + subscriber.LastName, 30))
    .appendTo(title);

  $('<span>')
    .addClass('ts-icon ts-icon-info')
    .attr('rel', '../../../Tips/User.aspx?UserID=' + subscriber.UserID + '&TicketID=' + _ticketID)
    .cluetip(clueTipOptions)
    .appendTo(title);
  $('#divSubscribers').append(item);

}

//users
var getUsers = function (request, response) {
  if (execGetUsers) { execGetUsers._executor.abort(); }
  execGetUsers = top.Ts.Services.Users.SearchUsers(request.term, function (result) { response(result); });
}

//queues
var appendQueues = function (queues) {
  $('#divQueues').empty().append('<div class="ts-separator ui-widget-content">');

  for (var i = 0; i < queues.length; i++) {
    appendQueue(queues[i]);
  }
}

var appendQueue = function (queue) {
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
      top.Ts.MainPage.openUser(queue.UserID);
    })
    .text(ellipseString(queue.FirstName + ' ' + queue.LastName, 30))
    .appendTo(title);

  $('<span>')
    .addClass('ts-icon ts-icon-info')
    .attr('rel', '../../../Tips/User.aspx?UserID=' + queue.UserID + '&TicketID=' + _ticketID)
    .cluetip(clueTipOptions)
    .appendTo(title);
  $('#divQueues').append(item);

}

//tags
var getTags = function (request, response) {
  if (execGetTags) { execGetTags._executor.abort(); }
  execGetTags = top.Ts.Services.Tickets.SearchTags(request.term, function (result) { response(result); });
}

var appendTags = function (tags) {
  $('#divTags').empty().append('<div class="ts-separator ui-widget-content">');
  for (var i = 0; i < tags.length; i++) {
    var item = getRemovableItem(tags[i], 'ticket-tag', 'a');
    var link = $('<a>')
      .attr('href', '#')
      .text(ellipseString(tags[i].Value, 30))
      .click(function (e) {
        e.preventDefault();
        top.Ts.MainPage.openTag($(this).closest('.ticket-tag').data('data').TagID);
      });
    item.find('.ticket-removable-item-title').empty().append(link);
    $('#divTags').append(item);
  }
}

//related
var getRelated = function (request, response) {
  if (execGetRelated) { execGetRelated._executor.abort(); }
  execGetRelated = top.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) { response(result); });
}

var appendRelated = function (tickets) {
  $('#divRelated').empty().append('<div class="ts-separator ui-widget-content">');
  for (var i = 0; i < tickets.length; i++) {
    var related = tickets[i];

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

    var relatedTicket = $('<a>')
      .attr('href', '#')
      .text(ellipseString(related.TicketNumber + ': ' + related.Name, 30))
      .data('number', related.TicketNumber)
      .click(function (e) {
        e.preventDefault();
        top.Ts.MainPage.openTicket($(this).data('number'), true);
      })
        .appendTo(item.find('.ticket-removable-item-description').empty());

    if (related.IsClosed) {
      relatedTicket.addClass('ticket-closed');
    }
    else {
      relatedTicket.removeClass('ticket-closed');
    }

    $('#divRelated').append(item);
  }
}

//reminders
var appendReminders = function (reminders) {
  $('#divReminders').empty().append('<div class="ts-separator ui-widget-content">');

  for (var i = 0; i < reminders.length; i++) {
    appendReminder(reminders[i]);
  }
}

var appendReminder = function (reminder) {
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
      top.Ts.MainPage.editReminder({ ReminderID: $(this).closest('.ticket-removable-item').data('o').ReminderID }, true,
        function (reminder) {
          top.Ts.Services.System.GetItemReminders(top.Ts.ReferenceTypes.Tickets, _ticketID, top.Ts.System.User.UserID, function (reminders) {
            appendReminders(reminders);
          })
        });
    })
    .text(ellipseString(reminder.Description, 30))
    .appendTo(title);


  $('<div>')
    .addClass('ticket-removable-item-description')
    .text(reminder.DueDate.localeFormat(top.Ts.Utils.getDateTimePattern()))
    .appendTo(item);

  $('#divReminders').append(item);

}

//tinymce editor
var initEditor = function (element, actionElement, init) {
  top.Ts.Settings.System.read('EnableScreenR', 'True', function (enableScreenR) {
    var editorOptions = {
      plugins: "autoresize paste link code textcolor image moxiemanager table",
      toolbar1: "insertPasteImage insertKb insertTicket image insertimage insertDropBox recordScreen insertUser recordVideo | link unlink | undo redo removeformat | cut copy paste pastetext | outdent indent | bullist numlist",
      toolbar2: "alignleft aligncenter alignright alignjustify | forecolor backcolor | fontselect fontsizeselect | bold italic underline strikethrough blockquote | code | table",
      statusbar: false,
      gecko_spellcheck: true,
      extended_valid_elements: "a[accesskey|charset|class|coords|dir<ltr?rtl|href|hreflang|id|lang|name|onblur|onclick|ondblclick|onfocus|onkeydown|onkeypress|onkeyup|onmousedown|onmousemove|onmouseout|onmouseover|onmouseup|rel|rev|shape<circle?default?poly?rect|style|tabindex|title|target|type],script[charset|defer|language|src|type]",
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
      moxiemanager_image_settings: {
        moxiemanager_rootpath: "/" + top.Ts.System.Organization.OrganizationID + "/images/",
        extensions: 'gif,jpg,jpeg,png'
      },
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
              ed.execCommand("FontSize", false, top.Ts.System.Organization.FontSize + 1);
              ed.getBody().style.fontSize = GetTinyMCEFontSize(top.Ts.System.Organization.FontSize + 1);
            }
          });
        });

        ed.on('paste', function (ed, e) {
          setTimeout(function () { ed.execCommand('mceAutoResize'); }, 1000);
        });

        ed.addButton('insertTicket', {
          title: 'Insert Ticket',
          //image: '../images/nav/16/tickets.png',
          icon: 'awesome fa fa-ticket',
          onclick: function () {
            top.Ts.System.logAction('Ticket - Ticket Inserted');

            top.Ts.MainPage.selectTicket(null, function (ticketID) {
              top.Ts.Services.Tickets.GetTicket(ticketID, function (ticket) {
                ed.focus();
                top.Ts.Services.Tickets.AddRelated(_ticketID, ticketID, null, function (tickets) {
                  appendRelated(tickets);
                  //window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addrelationship", userFullName);
                }, function (error) {
                  //container.remove();
                  alert(error.get_message());
                });
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
              //alert("Sorry, this feature is not supported by " + BrowserDetect.browser);
              top.Ts.MainPage.pasteImage(null, function (result) {
                ed.focus();
                if (result != "") {
                  var html = '<img src="' + top.Ts.System.AppDomain + '/dc/' + result + '"</a>&nbsp;<br/>';
                  ed.selection.setContent(html);
                  setTimeout(function () { ed.execCommand('mceAutoResize'); }, 1000);
                  ed.execCommand('mceAutoResize');
                  ed.focus();
                }
              });
            }
            else {
              top.Ts.MainPage.pasteImage(null, function (result) {
                debugger
                ed.focus();
                if (result != "") {
                  var html = '<img src="' + top.Ts.System.AppDomain + '/dc/' + result + '"</a>&nbsp;<br/>';
                  ed.selection.setContent(html);
                  setTimeout(function () { ed.execCommand('mceAutoResize'); }, 1000);
                  ed.execCommand('mceAutoResize');
                  ed.focus();
                }
              });
            }
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

        ed.addButton('insertDropBox', {
          title: 'Insert DropBox',
          icon: 'awesome fa fa-dropbox',
          //image: '../images/icons/dropbox.png',
          onclick: function () {
            var options = {
              linkType: "preview",
              success: function (files) {
                ed.focus();
                var html = '<a href=' + files[0].link + '>' + files[0].name + '</a>';
                ed.selection.setContent(html);
                ed.execCommand('mceAutoResize');
                ed.focus();
                top.Ts.System.logAction('Ticket - Dropbox Added');
              },
              cancel: function () {
                alert('There was a problem inserting the dropbox file.');
              }
            };
            Dropbox.choose(options);
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
                top.Ts.System.logAction('Ticket - KB Inserted');
                //needs to resize or go to end

              }, function () {
                alert('There was an error inserting your knowledgebase ticket.');
              });
            });
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
2. Navigate to Safari > Preferences > Security > Internet Plugins - Website Settings > Java and change the app.ts.com setting to Run in Unsafe Mode and click on the Trust button: <br>\
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

        ed.addButton('recordVideo', {
          title: 'Record video',
          //image: '../images/icons/Symbol_Record.png',
          icon: 'awesome fa fa-video-camera',
          onclick: function () {

            if (OT.checkSystemRequirements() == 1) {
              var dynamicPub = actionElement.find("#publisher");
              actionElement.find("#recordVideoContainer").show();
              dynamicPub.show();
              dynamicPub.attr("id", "tempContainer");
              dynamicPub.attr("width", "400px");
              dynamicPub.attr("height", "400px");

              if (dynamicPub.length == 0)
                dynamicPub = actionElement.find("#tempContainer");



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

      }
        , oninit: init
    };
    $(element).tinymce(editorOptions);
  });
}

var onScreenRecordStart = function () {
  $('.fa-circle-o-notch').removeClass("fa-circle-o-notch fa-spin").addClass("fa-circle");
  $('.divScreenRecorderMessages').hide();
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
        top.Ts.Services.Settings.WriteUserSetting('ReadScreenRecordingSafariInfo', 1);
      }
      break;
    default:
      top.Ts.Services.Settings.WriteUserSetting('ReadScreenRecordingInfo', 1);
  }
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
    top.Ts.System.logAction('Ticket - Screen Record Cancelled');
  }
};

//actions
var tickettimer = function () {
  var element = $('.ticket-action-form');

  //work out the real and ideal elapsed time
  var real = (counter * speed),
  ideal = (new Date().getTime() - start);

  counter++;
  //element.find('.timer').text(Math.floor(ideal / 60000));

  var diff = (ideal - real);

  if (_timerElapsed != Math.floor(ideal / 60000)) {
    var oldVal = parseInt(element.find('.ticket-action-form-minutes').val()) || 0;
    $('.ticket-action-form-minutes').val(oldVal + 1);
    _timerElapsed = Math.floor(ideal / 60000);
  }
  _timerid = setTimeout(tickettimer, (speed - diff));
}

var createActionForm = function (element, action, callback) {
  top.Ts.MainPage.highlightTicketTab(_ticketNumber, true);
  element = $(element).html($('#divActionForm').html()).addClass('ticket-action-form');
  var selectType = element.find('.ticket-action-form-actiontype');
  var types = top.Ts.Cache.getActionTypes();
  var newAction = null;
  for (var i = 0; i < types.length; i++) {
    $('<option>').attr('value', types[i].ActionTypeID).text(types[i].Name).data('data', types[i]).appendTo(selectType);
  }

  selectType.combobox();
  element.find('.ticket-action-form-date').datetimepicker({ dateFormat: dateFormat }).datetimepicker('setDate', new Date());
  element.find('.ticket-action-form-hours').spinner({ min: 0 }).val(0);
  element.find('.ticket-action-form-minutes').spinner({ min: 0 }).val(0);

  element.find('.timerbutton').click(function (e) {
    var togtext = element.find('.timerlabel').text() == 'Start Timing' ? 'Stop Timing' : 'Start Timing';

    if (togtext == "Stop Timing") {
      start = new Date().getTime()
      tickettimer();
    }
    else {
      clearTimeout(_timerid);
    }

    element.find('.timerlabel').text(togtext);
    element.find('.timerlabel').toggleClass('redText');

  })
      .attr('title', 'Start the Timer');


  if (top.Ts.System.Organization.SetNewActionsVisibleToCustomers == true && action == null) {
    element.find('.ticket-action-form-portal').prop('checked', true);
  }

  if (action != null) {
    element.find('.ticket-action-form-portal').prop('checked', action.IsVisibleOnPortal);
  }
  initEditor(element.find('.ticket-action-form-description'), element, function (ed) {
    if (action != null && action.Description != null) {
      ed.setContent(action.Description);
    }
    element.find('.ticket-action-form-description').tinymce().focus();
  });
  element.find('.ticket-action-form-cancel').click(function () {
    clearTimeout(_timerid);
    _timerElapsed = 0;
    counter = 0;
    if (recordingID)
      session.unpublish(publisher);
    callback(null); element.remove();
    $('#recorder').remove();

  });
  element.find('.ticket-action-form-save').click(function () {
    if ($("#recorder").length == 0) {
      var saveButton = $(this);
      isFormValid(function (isValid) {
        if (isValid == false) {
          alert("Please fill in the required fields before submitting this action.");
          return;
        }

        saveButton.closest('.ticket-action-form-buttons').addClass('saving');
        var saved = saveAction(element, action, function (result) {
          if (element.find('.upload-queue li').length > 0 && result !== null) {
            newAction = result;
            element.find('.upload-queue li').each(function (i, o) {
              var data = $(o).data('data');
              //data.form.prop('action', '../../Upload/Actions/' + newAction.ActionID);
              data.url = '../../../Upload/Actions/' + newAction.Action.ActionID;
              data.jqXHR = data.submit();
              $(o).data('data', data);
            });
            //loadTicket(_ticketNumber, 0);
          }
          else {
            saveButton.closest('.ticket-action-form-buttons').removeClass('saving');
            element.remove();
            loadTicket(_ticketNumber, 0);
            callback(result);
          }
        });
        if (saved == false) saveButton.closest('.ticket-action-form-buttons').removeClass('saving');

      });
    }
  });

  element.find('.ticket-action-form-kb').click(function (e) {
    if (!top.Ts.System.User.ChangeKbVisibility && !top.Ts.System.User.IsSystemAdmin) {
      e.preventDefault();
    }
  });

  var uploadName = 'new_action';
  if (action != null) {
    uploadName = 'action_' + action.ActionID;
    element.find('.ticket-action-form-name').val(action.Name);

    if (top.Ts.SystemActionTypes.Custom != action.SystemActionTypeID) {
      element.find('.ticket-action-form-block-actiontype').hide();
    }
    else {
      selectType.combobox('setValue', action.ActionTypeID);
    }

    if (action.DateStarted != null) {
      element.find('.ticket-action-form-date').datetimepicker({ dateFormat: dateFormat }).datetimepicker('setDate', action.DateStarted);
    }

    if (action.TimeSpent != null) {
      element.find('.ticket-action-form-hours').val(Math.floor(action.TimeSpent / 60));
      element.find('.ticket-action-form-minutes').val(Math.floor(action.TimeSpent % 60));
    }

    if (action.IsKnowledgeBase === true) {
      element.find('.ticket-action-form-kb').prop('checked', true);
    }

    if (action.IsVisibleOnPortal === true) {
      element.find('.ticket-action-form-portal').prop('checked', true);
    }
  }

  if (!canKbEdit) {
    //$('.newticket-kb').attr("disabled", true);
    element.find('#ticket-action-kb-div').hide();
  }

  element.find('.file-upload').fileupload({
    namespace: uploadName,
    dropZone: element,
    add: function (e, data) {
      for (var i = 0; i < data.files.length; i++) {
        var item = $('<li>')
        .appendTo(element.find('.upload-queue'));

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
          -
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
      callback(null);
    },
    progress: function (e, data) {
      data.context.find('.progress').progressbar('value', parseInt(data.loaded / data.total * 100, 10));
    },
    start: function (e, data) {
      element.find('.progress').progressbar().show();
      element.find('.upload-queue .ui-icon-close').hide();
      element.find('.upload-queue .ui-icon-cancel').show();
    },
    stop: function (e, data) {
      element.find('.progress').progressbar('value', 100);
      top.Ts.Services.Tickets.GetActionInfo(newAction.Action.ActionID, function (result) {
        callback(result);
      });
      element.remove();

    }

  });

  element.find('#rcdtok').click(function (e) {
    top.Ts.Services.Tickets.StartArchiving(sessionId, function (resultID) {
      element.find('#rcdtok').hide();
      //element.find('#rcdtok span').text('Re-Record');
      element.find('#stoptok').show();
      element.find('#inserttok').hide();
      element.find('#deletetok').hide();
      recordingID = resultID;
      element.find('#statusText').text("Currently Recording ...");
    });
  });

  element.find('#stoptok').hide();

  element.find('#stoptok').click(function (e) {
      element.find('#statusText').text("Processing...");
    top.Ts.Services.Tickets.StopArchiving(recordingID, function (resultID) {
      element.find('#rcdtok').show();
      element.find('#stoptok').hide();
      element.find('#inserttok').show();
      element.find('#canceltok').show();
      tokurl = "https://s3.amazonaws.com/teamsupportvideos/45228242/" + resultID + "/archive.mp4";
      element.find('#statusText').text("Recording Stopped");
    });
  });

  element.find('#inserttok').hide();

  element.find('#inserttok').click(function (e) {
      tinyMCE.activeEditor.execCommand('mceInsertContent', false, '<br/><br/><video width="400" height="400" controls poster="' + top.Ts.System.AppDomain + '/dc/1078/images/static/videoview1.jpg"><source src="' + tokurl + '" type="video/mp4"><a href="' + tokurl + '">Please click here to view the video.</a></video>');
    session.unpublish(publisher);
    element.find('#rcdtok').show();
    element.find('#stoptok').hide();
    element.find('#inserttok').hide();
    element.find('#recordVideoContainer').hide();
    element.find('#statusText').text("");
  });

  element.find('#deletetok').hide();
  //  element.find('#deletetok').click(function (e) {
  //      top.Ts.Services.Tickets.DeleteArchive(recordingID, function (resultID) {
  //          element.find('#rcdtok').show();
  //          element.find('#stoptok').hide();
  //          element.find('#inserttok').hide();
  //          session.unpublish(publisher);
  //          element.find('#recordVideoContainer').hide();
  //1      });
  //  });

  //element.find('#canceltok').hide();
  element.find('#canceltok').click(function (e) {
      if (recordingID) {
          element.find('#statusText').text("Cancelling Recording ...");
      top.Ts.Services.Tickets.DeleteArchive(recordingID, function (resultID) {
        element.find('#rcdtok').show();
        element.find('#stoptok').hide();
        element.find('#inserttok').hide();
        session.unpublish(publisher);
        element.find('#recordVideoContainer').hide();
        element.find('#statusText').text("");
      });
    }
    else {
      session.unpublish(publisher);
      element.find('#recordVideoContainer').hide();
    }
    element.find('#statusText').text("");
  });
  element.find('#recordVideoContainer').hide();

}

var saveAction = function (form, oldAction, callback) {
  element = $(form);
  element.find('.ts-error-text').hide();
  var actionType = element.find('.ticket-action-form-actiontype option:selected').data('data');
  var timeSpent = parseInt(element.find('.ticket-action-form-hours').val()) * 60 + parseInt(element.find('.ticket-action-form-minutes').val());
  if (timeSpent < 1 && actionType.IsTimed == true && top.Ts.System.Organization.TimedActionsRequired == true) {
    element.find('.ticket-action-form-timespent .ts-error-text').show();
    return false;
  }

  var action = new top.TeamSupport.Data.ActionProxy();
  action.ActionID = oldAction === null ? -1 : oldAction.ActionID;
  action.TicketID = _ticketID;
  if (element.data('action') !== undefined) { action.ActionID = element.data('action').ActionID; }

  action.ActionTypeID = actionType.ActionTypeID

  //action.ActionTypeID = element.find('.ticket-action-form-actiontype').val();
  action.SystemActionTypeID = 0;
  action.DateStarted = top.Ts.Utils.getMsDate(element.find('.ticket-action-form-date').datetimepicker('getDate'));
  action.TimeSpent = timeSpent || 0;
  action.IsKnowledgeBase = element.find('.ticket-action-form-kb').prop('checked');
  action.IsVisibleOnPortal = element.find('.ticket-action-form-portal').prop('checked');
  action.Description = element.find('.ticket-action-form-description').html();

  if (action.IsVisibleOnPortal == true) confirmVisibleToCustomers();
  top.Ts.Services.Tickets.UpdateAction(action, function (result) {
    callback(result)
  }, function (error) {
    callback(null);
  });

  top.Ts.Services.Tickets.GetSubscribers(_ticketID, function (subscribers) {
    appendSubscribers(subscribers);
  });

  clearTimeout(_timerid);
  _timerElapsed = 0;
  counter = 0;

  top.Ts.System.logAction('Action Saved');
  window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addaction", userFullName);

}

var createActionElement = function () {
  return $('<div>')
      .addClass('ticket-action ui-widget-content ui-corner-all ticket-content-section')
      .html($('#divActionDisplay').html())
      .hover(function (e) {
        e.preventDefault();
        $(this).find('.ticket-action-edit').show();
        $(this).find('.ticket-action-delete').show();
        $(this).find('.ts-icon-pinnot').show();
      }, function (e) {
        e.preventDefault();
        $('.ticket-action-edit').hide();
        $('.ticket-action-delete').hide();
        $('.ts-icon-pinnot').hide();

      });
}

var appendActionElement = function (actionInfo, actionNumber) {
  var actionDiv = createActionElement().appendTo('#divActions');
  actionDiv.find(".ticket-action-order").html('(' + actionNumber + ')');
  loadActionDisplay(actionDiv, actionInfo, true);
}

var preappendActionElement = function (actionInfo, actionNumber) {
  var actionDiv = createActionElement().prependTo('#divActions');
  actionDiv.find(".ts-icon-pin").show();
  actionDiv.find(".ticket-action-order").html('(' + actionNumber + ')');
  loadActionDisplay(actionDiv, actionInfo, true);
}

var loadActionDisplay = function (element, actionInfo, doExpand) {
  element = $(element);
  element
  .data('action', actionInfo.Action)
  .data('creator', actionInfo.Creator)
  .data('attachments', actionInfo.Attachments);

  var action = actionInfo.Action
  var attachments = actionInfo.Attachments;
  var creator = actionInfo.Creator;
  var canEdit = top.Ts.System.User.IsSystemAdmin || top.Ts.System.User.UserID === action.CreatorID;
  var restrictedFromEditingAnyActions = !top.Ts.System.User.IsSystemAdmin && top.Ts.System.User.RestrictUserFromEditingAnyActions;

  if (!top.Ts.System.User.AllowUserToEditAnyAction && (!canEdit || restrictedFromEditingAnyActions)) {
    element.find('.ticket-action-edit').remove();
    element.find('.ticket-action-delete').remove();
  }

  if (action.SystemActionTypeID === top.Ts.SystemActionTypes.Description) {
    element.find('.ticket-action-delete').remove();
  }

  if (doExpand) {
    if (action.Description == null) {
      element.find('.ticket-action-expand-icon').removeClass('ui-icon-triangle-1-s').addClass('ui-icon-triangle-1-e');
    }
    else {
      element.find('.ticket-action-expand-icon').removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');
    }
  }
  element.find('.ticket-action-title').text(action.DisplayName);
  element.find('.ticket-action-kb').toggleClass('ts-icon-kb', action.IsKnowledgeBase === true).toggleClass('ts-icon-kbnot', action.IsKnowledgeBase === false);
  if (action.IsKnowledgeBase) {
    element.find('.ticket-action-kb').prop('title', 'Remove Knowledgebase Article');
  }
  else if (!action.IsKnowledgeBase) {
    element.find('.ticket-action-kb').prop('title', 'Make Knowledgebase Article');
  }
  element.find('.ticket-action-portal').toggleClass('ts-icon-portal', action.IsVisibleOnPortal === true).toggleClass('ts-icon-portalnot', action.IsVisibleOnPortal === false);
  if (action.Pinned) {
    element.find('.ticket-action-pin').addClass('ts-icon-pin');
    element.find('.ticket-action-pin').removeClass('ts-icon-pinnot');
    element.find('.ticket-action-pin').prop('title', 'Unpin this action.');
  }

  if (!top.Ts.System.User.IsSystemAdmin && !top.Ts.System.User.UserCanPinAction) {
    element.find('.ts-icon-pinnot').remove();
  }

  if (attachments.length < 1) {
    element.find('.ticket-action-attachment-count').hide();
    element.find('.ticket-action-attachments').hide();
  }
  else {
    element.find('.ticket-action-attachment-count').show();
    element.find('.ticket-action-attachments').show();
  }

  if (creator != null) {

    var creatorIcon = 'ts-icon-customer';
    if (creator !== null && creator.OrganizationID === top.Ts.System.User.OrganizationID) {
      creatorIcon = 'ts-icon-tsuser';
    }

    $('<span>')
    .addClass('ts-icon creator-type ' + creatorIcon)
    .appendTo(element.find('.ticket-action-info').empty());

    $('<a>')
  .attr('href', '#')
  .click(function (e) {
    e.preventDefault();
    top.Ts.MainPage.openUser(creator.UserID);
  })
  .text(creator.FirstName + ' ' + creator.LastName)
  .addClass('name')
  .appendTo($('<span>').appendTo(element.find('.ticket-action-info')));

    $('<span>')
  .addClass('ts-icon ts-icon-info')
  .attr('rel', '../../../Tips/User.aspx?UserID=' + creator.UserID + '&TicketID=' + _ticketID)
  .cluetip(clueTipOptions)
  .appendTo(element.find('.ticket-action-info'));

    $('<span>')
    .addClass('date')
    .text(' - ' + action.DateCreated.localeFormat(top.Ts.Utils.getDateTimePattern()))
    .appendTo(element.find('.ticket-action-info'));

  }
  else {
    $('<span>')
    .addClass('ts-icon creator-type ts-icon-tsuser')
    .appendTo(element.find('.ticket-action-info').empty());

    $('<span>')
    .addClass('date')
    .text(action.DateCreated.localeFormat(top.Ts.Utils.getDateTimePattern()))
    .appendTo(element.find('.ticket-action-info'));
  }

  element.find('.ticket-action-work').empty();

  if (action.TimeSpent !== null && action.TimeSpent > 0) {
    var time = top.Ts.Utils.getTimeSpentText(action.TimeSpent / 60);

    if (action.DateStarted != null) time = time + ' - ' + action.DateStarted.localeFormat(top.Ts.Utils.getDateTimePattern())
    element.find('.ticket-action-work').text(time);
  }

  if (action.Description != null) {
    var attachmentList = element.find('.ticket-action-attachments ul').empty();

    for (var i = 0; i < attachments.length; i++) {
      var item = $('<li>')
      .appendTo(attachmentList)
      .data('attachment', attachments[i])
      .hover(function (e) {
        $(this).find('.ticket-action-attachment-delete').removeClass('ts-hidden-icon');
      }, function (e) {
        $('.ticket-action-attachment-delete').addClass('ts-hidden-icon');
      })
      $('<span>')
      .addClass('ts-icon ts-icon-attachment')
      .appendTo(item);

      $('<a>')
      .attr('target', '_blank')
      .text(attachments[i].FileName)
      .addClass('ui-state-default ts-link')
      .attr('href', '../../../dc/1/attachments/' + attachments[i].AttachmentID)
      .appendTo(item);
      if (canEdit) {
        $('<span>')
          .addClass('ui-icon ui-icon-close ticket-action-attachment-delete ts-hidden-icon')
          .click(function (e) {
            var element = $(this).closest('li');
            var attachment = element.data('attachment');
            if (!confirm('Are you sure you would like to delete "' + attachment.FileName + '."')) return;
            top.Ts.Services.Tickets.DeleteAttachment(
              attachment.AttachmentID,
              function () {
                var parent = element.parent();
                element.remove();
                if (parent.find('li').length < 1) {
                  parent.parent().hide();
                }
              }, function () {
                alert('There was a problem deleting "' + attachment.FileName + '."');
              });
          })
          .appendTo(item);
      }
    }


    var desc = element.find('.ticket-action-description').html(action.Description);
    desc.find('a').attr('target', '_blank');
    desc.find('blockquote').addClass('ui-corner-all');
    desc.find('pre').addClass('ui-corner-all');
    element.find('.ticket-action-body').show();
  }
  element.find('a').addClass('ui-state-default ts-link');

}

var confirmVisibleToCustomers = function () {
  if ($('#isTicketPortal').text() != 'Yes') {
    if (confirm('This ticket is not visible to customers.\n\nWould you like to make it visible to customers now?') == true) {
      $('#isTicketPortal').click();
    }
  }
}

var setUserName = function (user, userID) {
  var isAssigned = user !== null && user !== "";
  var id = -1;

  if (isAssigned) {
    var name = (!userID ? user.FirstName + ' ' + user.LastName : user);
    id = (!userID ? user.UserID : userID);
    $('#userName').text(name);
  }
  else {
    $('#userName').text("Unassigned");
  }
  $('#userName').parent().find('.ts-icon-info')
  .toggle(isAssigned)
  .attr('rel', '../../../Tips/User.aspx?UserID=' + id + '&TicketID=' + _ticketID)
  .cluetip(clueTipOptions)
}

//products
var loadProducts = function (productIDs) {
  var products = top.Ts.Cache.getProducts();
  var parent = $('#product').closest('.ticket-name-value').hide();
  var container = $('<div>').addClass('ticket-combobox').insertAfter(parent);
  var select = $('<select>').appendTo(container);

  var unassigned = new Object();
  unassigned.ProductID = null;
  unassigned.Name = "Unassigned";
  var option = $('<option>').text(unassigned.Name).appendTo(select).data('product', unassigned);
  if ($('#product').text() === unassigned.Name) {
    option.attr('selected', 'selected');
  }

  if (!productIDs || productIDs == null || productIDs.length < 1) {
    for (var i = 0; i < products.length; i++) {
      var option = $('<option>').text(products[i].Name).appendTo(select).data('product', products[i]);
      if ($('#product').text() === products[i].Name) {
        option.attr('selected', 'selected');
      }
    }
  }
  else {
    for (var i = 0; i < products.length; i++) {
      for (var j = 0; j < productIDs.length; j++) {
        if (productIDs[j] == products[i].ProductID) {
          var option = $('<option>').text(products[i].Name).appendTo(select).data('product', products[i]);
          if ($('#product').text() === products[i].Name) {
            option.attr('selected', 'selected');
          }
        }
      }
    }
  }

  select.combobox({
    selected: function (e, ui) {
      parent.show().find('img').show();
      var product = $(ui.item).data('product');
      top.Ts.System.logAction('Ticket - Product Changed');
      top.Ts.Services.Tickets.SetProduct(_ticketID, product.ProductID, function (result) {
        if (result !== null) {
          setProduct(result.id, result.label);
          _productFamilyID = result.data;
          setVersion(null, null, true);
          setVersion(null, null, false);
          //$('#product').text(result[1] === '' ? 'Unassigned' : result[1]).data('productID', result[0]);
          //$('#resolved').text('Unassigned');
          //$('#reported').text('Unassigned');
          parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
        }
        else {
          parent.show().find('img').hide();
        }
        top.Ts.Services.Tickets.GetParentValues(_ticketID, function (fields) {
          appendCustomValues(fields);
        });
      },
        function (error) {
          parent.show().find('img').hide();
          alert('There was an error setting the product.');
        });
      container.remove();
    },
    close: function (e, ui) {
      removeComboBoxes();
    }
  });
  select.combobox('search', '');
}

var removeComboBoxes = function () {
  $('.ticket-combobox').prev().show().next().remove();
}

var ellipseString = function (text, max) { return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text; };

var getRemovableItem = function (data, itemClass, title, description) {
  var item = $('<div>')
  .addClass('ticket-removable-item ui-corner-all ts-color-bg-accent ' + itemClass)
  item.data('data', data);

  $('<span>').addClass('ui-icon ui-icon-close').appendTo(item);
  if (title) $('<div>').addClass('ticket-removable-item-title').text(ellipseString(title, 30)).appendTo(item);
  if (description) $('<div>').addClass('ticket-removable-item-description').text(ellipseString(description, 30)).appendTo(item);
  return item;
}

//ticket
var loadTicket = function (ticketNumber, refresh) {

  top.Ts.Services.Tickets.GetTicketInfo(ticketNumber, function (info) {
    if (info == null) {
      var url = window.location.href;
      if (url.indexOf('.') > -1) {
        url = url.substring(0, url.lastIndexOf('/') + 1);
      }
      window.location = url + 'NoTicketAccess.html';
      return;

    }
    _ticketID = info.Ticket.TicketID;
    _ticketNumber = info.Ticket.TicketNumber;
    top.Ts.Services.Tickets.GetTicketLastSender(_ticketID, function (result) {
      if (result !== null) {
        _ticketSender = new Object();
        _ticketSender.UserID = result.UserID;
        _ticketSender.Name = result.FirstName + ' ' + result.LastName;
      }
    });
    _ticketCreator = new Object();
    _ticketCreator.UserID = info.Ticket.CreatorID;
    _ticketCreator.Name = info.Ticket.CreatorName;
    top.Ts.System.logAction('View Ticket');
    $('.page-loading').hide().next().show();
    if (_layout === null) {
      $('.ticket-layout').layout({
        resizeNestedLayout: true,
        defaults: {
          spacing_open: 0,
          closable: false
        },
        center: {
          paneSelector: ".ticket-panel-content"
        },
        north: {
          paneSelector: ".ticket-panel-toolbar",
          size: 31
        }
      });
    }

    if (info == null) alert('no ticket');

    $('.ticket-sla-status').removeClass('ts-icon-sla-good ts-icon-sla-warning ts-icon-sla-bad')
    if (info.Ticket.SlaViolationTime === null) {
      $('.ticket-sla-status').addClass('ts-icon-sla-good');
      $('#ticketSla').text('None');
    }
    else {
      $('.ticket-sla-status').addClass((info.Ticket.SlaViolationTime < 1 ? 'ts-icon-sla-bad' : (info.Ticket.SlaWarningTime < 1 ? 'ts-icon-sla-warning' : 'ts-icon-sla-good')));
      $('#ticketSla').text(info.Ticket.SlaViolationDate.localeFormat(top.Ts.Utils.getDateTimePattern()));
    }

    $('.ticket-sla-info')
      .attr('rel', '../../../Tips/Sla.aspx?TicketID=' + _ticketID)
      .cluetip(clueTipOptions);

    $('#ticketNumber').text(info.Ticket.TicketNumber);
    $('#ticketName').html($.trim(info.Ticket.Name) === '' ? '[Untitled Ticket]' : $.trim(info.Ticket.Name));
    $('#creator').text(info.Ticket.CreatorName);
    //$('.ticket-dateCreated').text(info.Ticket.DateCreated.localeFormat(top.Ts.Utils.getDateTimePattern()));
    $('#isTicketPortal').text((info.Ticket.IsVisibleOnPortal == true ? 'Yes' : 'No'));


    $('#isTicketKB').text((info.Ticket.IsKnowledgeBase == true ? 'Yes' : 'No'));

    $('#knowledgeBaseCategoryAnchor').text((info.Ticket.KnowledgeBaseCategoryName == null ? 'Unassigned' : info.Ticket.KnowledgeBaseCategoryDisplayString));
    if (info.Ticket.IsKnowledgeBase === false) {
      $('#knowledgeBaseCategoryDiv').hide();
    }
    $('#ticketCommunity').text((info.Ticket.CategoryName == null ? 'Unassigned' : info.Ticket.CategoryDisplayString));

    _dueDate = info.Ticket.DueDate;
    var dueDate = info.Ticket.DueDate == null ? null : info.Ticket.DueDate;
    $('#dueDate').text((dueDate === null ? 'Unassigned' : dueDate.localeFormat(top.Ts.Utils.getDateTimePattern())));
    if (_dueDate != null && _dueDate < Date.now()) {
      $('#dueDate').parent().addClass('nonrequired-field-error ui-corner-all');
      var anchor = $('#dueDate').parent().find('a');
      anchor.addClass('nonrequired-field-error-font');
    }

    $('#ticketType').html(info.Ticket.TicketTypeName);
    _ticketTypeID = info.Ticket.TicketTypeID;
    $('#ticketStatus')
    .text(info.Ticket.Status)
    .toggleClass('ticket-closed', info.Ticket.IsClosed)
    .data('ticketStatusID', info.Ticket.TicketStatusID);
    $('#ticketSeverity').text(info.Ticket.Severity);
    setUserName(info.Ticket.UserName, info.Ticket.UserID);
    $('#ticketGroup').text(info.Ticket.GroupName == null ? 'Unassigned' : info.Ticket.GroupName);
    _ticketGroupID = info.Ticket.GroupID;
    setProduct(info.Ticket.ProductID, info.Ticket.ProductName);
    _productFamilyID = info.Ticket.ProductFamilyID;
    setVersion(info.Ticket.ReportedVersionID, info.Ticket.ReportedVersion, false);
    setVersion(info.Ticket.SolvedVersionID, info.Ticket.SolvedVersion, true);
    //$('#reported').text(info.Ticket.ReportedVersion == null ? 'Unassigned' : info.Ticket.ReportedVersion);
    //$('#resolved').text(info.Ticket.SolvedVersion == null ? 'Unassigned' : info.Ticket.SolvedVersion);
    if (info.Ticket.IsSubscribed === true) $('#btnSubscribe .ts-toolbar-caption').text('Unsubscribe');
    if (info.Ticket.IsEnqueued === true) $('#btnEnqueue .ts-toolbar-caption').text('Dequeue');
    if (info.Ticket.IsFlagged === true) $('#btnFlag .ts-toolbar-caption').text('Unflag');

    $('.ticket-source').css('backgroundImage', "url('../" + top.Ts.Utils.getTicketSourceIcon(info.Ticket.TicketSource) + "')").attr('title', 'Ticket Source: ' + (info.Ticket.TicketSource == null ? 'Agent' : info.Ticket.TicketSource));
    var ticketUrl = window.location.href.replace('vcr/1_9_0/Pages/Ticket.html', '');
    $('<a>')
    .attr('href', ticketUrl)
    .attr('target', '_blank')
    .text(ticketUrl)
    .appendTo($('.ticket-url').empty());


    if (info.Ticket.IsClosed == true) {
      $('#daysOpened').text(info.Ticket.DaysClosed).prev().text('Days Closed:');
    }
    else {
      $('#daysOpened').text(info.Ticket.DaysOpened).prev().text('Days Opened:');
    }

    //if (info.Ticket.ModifierName) $('<div>').text('Last Modified By: ' + info.Ticket.ModifierName).appendTo(details);
    //if (info.Ticket.DateModified) $('<div>').text('Last Modified On: ' + info.Ticket.DateModified.localeFormat(top.Ts.Utils.getDateTimePattern())).appendTo(details);
    $('#timeSpent').text(top.Ts.Utils.getTimeSpentText(info.Ticket.HoursSpent));

    if (!top.Ts.System.User.CanCreateContact && !top.Ts.System.User.IsSystemAdmin) {
      $('.ticket-customer-new').hide();
    }

    appendCustomers(info.Customers);
    appendAssets(info.Assets);
    appendTags(info.Tags);
    appendRelated(info.Related);
    appendCustomValues(info.CustomValues);
    appendSubscribers(info.Subscribers);
    appendQueues(info.Queuers);
    appendReminders(info.Reminders);


    //if the current actions don't equal the # for the ticket , update 
    if ($('.ticket-action').length - 1 != info.Actions.length) {
      if ($('.ticket-action-form').length <= 0) {
        $('#divActions').empty();

        for (var i = 0; i < info.Actions.length; i++) {
          if (info.Actions[i].Action.Pinned) {
            preappendActionElement(info.Actions[i], info.Actions.length - i);
          }
          else {
            appendActionElement(info.Actions[i], info.Actions.length - i);
          }
        }
      }
      else {
        preappendActionElement(info.Actions[0], info.Actions.length - i);
      }
    }

    // This is to show any table borders as TinyMCE only provides the html attribute that is deprecated.
    var $table = $('.ticket-action-description .table');
    var $row = $('.ticket-action-description .table tr');
    var $cell = $('.ticket-action-description .table td');
    $table.css({
      "border": $table.attr('border') + "px solid black",
      "border-spacing": "2px"
    });
    $row.css({
      "border": $table.attr('border') + "px solid black"
    });
    $cell.css({
      "border": $table.attr('border') + "px solid black"
    });

    if ($('.ticket-action-form').length < 1)
      $('.ticket-content a').addClass('ui-state-default ts-link');

    $('.ticket-rail a').addClass('ui-state-default ts-link');
    $('.ts-icon-info').removeClass('ui-state-default ts-link');

    if (!top.Ts.System.User.ChangeTicketVisibility && !top.Ts.System.User.IsSystemAdmin) {
      $('#isTicketPortal').removeClass('ui-state-default ts-link');
      $('#isTicketPortal').addClass('disabledlink');
    }

    if (!top.Ts.System.User.ChangeKbVisibility && !top.Ts.System.User.IsSystemAdmin) {
      $('#isTicketKB').removeClass('ui-state-default ts-link');
      $('#isTicketKB').addClass('disabledlink');
      $('#knowledgeBaseCategoryAnchor').removeClass('ui-state-default ts-link');
      $('#knowledgeBaseCategoryAnchor').addClass('disabledlink');
    }

    $('#watercoolerIframe').attr("src", "WaterCooler.html?pagetype=0&pageid=" + _ticketNumber);

    top.Ts.Services.Tickets.GetTicketWaterCoolerCount(_ticketNumber, function (result) {
      $('#watercoolerLink').html('Water Cooler (' + result + ')')
    });
    top.Ts.MainPage.updateMyOpenTicketReadCount();

    top.Ts.Services.Admin.GetIsJiraLinkActiveForTicket(_ticketID, function (result) {
      if (result) {
        $('#enterIssueKey').hide();
        $('.ticket-widget-jira').show();

        if (info.LinkToJira != null) {
          if (!info.LinkToJira.JiraKey) {
            $('#issueKeyValue').text('Pending...');
          }
          else if (!info.LinkToJira.JiraLinkURL) {
            $('#issueKeyValue').text(info.LinkToJira.JiraKey);
            if (info.LinkToJira.JiraKey.indexOf('Error') > -1) {
              $('#issueKeyValue').addClass('nonrequired-field-error ui-corner-all');
            }
            else {
              $('#issueKeyValue').removeClass('nonrequired-field-error ui-corner-all');
            }
          }
          else {
            var jiraLink = $('<a>')
              .attr('href', info.LinkToJira.JiraLinkURL)
              .attr('target', '_blank')
              .text(info.LinkToJira.JiraKey)
              .addClass('value ui-state-default ts-link')
              .appendTo($('#issueKeyValue').parent())
          }

          $('#issueKey').show();
          $('.ts-jira-buttons-container').hide();
        }
        else {
          $('#issueKey').hide();
          $('.ts-jira-buttons-container').show();
        }
      }
      else {
        $('.ticket-widget-jira').hide();
      }
    });

    if (typeof refresh === "undefined") {
      window.top.ticketSocket.server.getTicketViewing(_ticketNumber);
    }

    if (_ticketGroupID != null) {
      top.Ts.Services.Users.GetGroupUsers(_ticketGroupID, function (result) {
        _ticketGroupUsers = result;
      });
    }

    top.Ts.Services.Customers.LoadTicketAlerts(_ticketID, function (note) {
      if (note) {
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
          modal: true,
          create: function () {
            $(this).css('maxWidth', '800px');
          },
          buttons: buttons
        });

      }
    });

  });
}

var appendCustomValues = function (fields) {
  if (fields === null || fields.length < 1) {
    $('#divProperties').empty();
    $('.ticket-widget-properties').hide();
    return;
  }
  $('.ticket-widget-properties').show();
  var container = $('#divProperties').empty().removeClass('ts-loading');
  _parentFields = [];
  for (var i = 0; i < fields.length; i++) {
    var item = null;

    var field = fields[i];

    if (field.CustomFieldCategoryID == -1) {
      var div = $('<div>').addClass('ticket-name-value').data('field', field);
      $('<span>')
          .addClass('property')
          .text(field.Name + ': ')
          .appendTo(div);

      switch (field.FieldType) {
        case top.Ts.CustomFieldType.Text: appendCustomEdit(field, div); break;
        case top.Ts.CustomFieldType.Date: appendCustomEditDate(field, div); break;
        case top.Ts.CustomFieldType.Time: appendCustomEditTime(field, div); break;
        case top.Ts.CustomFieldType.DateTime: appendCustomEditDateTime(field, div); break;
        case top.Ts.CustomFieldType.Boolean: appendCustomEditBool(field, div); break;
        case top.Ts.CustomFieldType.Number: appendCustomEditNumber(field, div); break;
        case top.Ts.CustomFieldType.PickList: appendCustomEditCombo(field, div, false); break;
        default:
      }

      container.append(div);
    }
  }
  appendCategorizedCustomValues(fields);
}

var appendCustomEditCombo = function (field, element, loadConditionalFields) {
  var result = $('<a>')
      .attr('href', '#')
      .text((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : field.Value))
      .addClass('value ui-state-default ts-link')
      .appendTo(element)
      .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
      .click(function (e) {
        e.preventDefault();
        $('.ticket-cutstom-edit').prev().show().next().remove();
        var parent = $(this).parent().hide();
        var container = $('<div>')
          .addClass('ticket-cutstom-edit ticket-combobox ticket-custom-edit')
          .css('marginTop', '1em')
          .insertAfter(parent);
        var fieldValue = parent.closest('.ticket-name-value').data('field').Value;
        var select = $('<select>').appendTo(container);

        var items = field.ListValues.split('|');
        for (var i = 0; i < items.length; i++) {
          var option = $('<option>').text(items[i]).appendTo(select);
          if (fieldValue === items[i]) { option.attr('selected', 'selected'); }
        }

        select.combobox({
          selected: function (e, ui) {
            parent.show().find('img').first().show();
            var value = $(this).val();
            container.remove();

            if (field.IsRequired && field.IsFirstIndexSelect == true && $(this).find('option:selected').index() < 1) {
              result.parent().addClass('ui-state-error-custom ui-corner-all');
            }
            else {
              result.parent().removeClass('ui-state-error-custom ui-corner-all');
            }
            if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && field.IsFirstIndexSelect == true && $(this).find('option:selected').index() < 1) {
              result.parent().addClass('ui-state-error-to-close-custom ui-corner-all');
              alert("This field can not be cleared in a closed ticket");
              parent.find('img').hide().next().show().delay(800).fadeOut(400);
              return;
            }
            else {
              result.parent().removeClass('ui-state-error-to-close-custom ui-corner-all');
            }
            if (field.IsFirstIndexSelect == true && $(this).find('option:selected').index() < 1) {
              result.parent().addClass('is-empty');
            }
            else {
              result.parent().removeClass('is-empty');
            }
            top.Ts.System.logAction('Ticket - Custom Value Set');
            top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
              parent.find('img').first().hide().next().show().delay(800).fadeOut(400);
              parent.closest('.ticket-name-value').data('field', result);
              parent.find('a').first().text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
              $('.' + field.CustomFieldID + 'children').remove();
              var childrenContainer = $('<div>').addClass(field.CustomFieldID + 'children customFieldIndent').insertAfter(element);
              appendMatchingParentValueFields(childrenContainer, result);
              window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
            }, function () {
              alert("There was a problem saving your ticket property.");
            });


          },
          close: function (e, ui) {
            removeComboBoxes();
          }
        });
        select.combobox('search', '');
      });
  var items = field.ListValues.split('|');
  if (field.IsRequired && ((field.IsFirstIndexSelect == true && (items[0] == field.Value || field.Value == null || $.trim(field.Value) === '')) || (field.Value == null || $.trim(field.Value) === ''))) {
    result.parent().addClass('ui-state-error-custom ui-corner-all');
  }
  if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && ((field.IsFirstIndexSelect == true && (items[0] == field.Value || field.Value == null || $.trim(field.Value) === '')) || (field.Value == null || $.trim(field.Value) === ''))) {
    result.parent().addClass('ui-state-error-to-close-custom ui-corner-all');
  }
  if (field.IsRequiredToClose) {
    result.parent().addClass('is-required-to-close');
  }
  if ((field.IsFirstIndexSelect == true && items[0] == field.Value) || field.Value == null || $.trim(field.Value) === '') {
    result.parent().addClass('is-empty');
  }
  //We could always call the appendMatchingParentValueFields but in this way we limit the impact in case of error in new code.
  if (loadConditionalFields) {
    $('.' + field.CustomFieldID + 'children').remove();
    var childrenContainer = $('<div>').addClass(field.CustomFieldID + 'children customFieldIndent').insertAfter(element);
    appendMatchingParentValueFields(childrenContainer, field);
  }
  else {
    _parentFields.push(element);
  }
}

var appendCustomEditNumber = function (field, element) {
  var result = $('<a>')
    .attr('href', '#')
    .text((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : field.Value))
    .addClass('value ui-state-default ts-link')
    .appendTo(element)
    .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
    .click(function (e) {
      e.preventDefault();
      $('.ticket-cutstom-edit').prev().show().next().remove();
      var parent = $(this).parent().hide();
      var container = $('<div>')
      .addClass('ticket-cutstom-edit')
      .css('marginTop', '1em')
      .insertAfter(parent);
      var fieldValue = parent.closest('.ticket-name-value').data('field').Value;
      var input = $('<input type="text">')
        .addClass('ui-widget-content ui-corner-all ticket-cutstom-edit-text-input')
        .css('width', '100%')
        .val(fieldValue)
        .appendTo(container)
        .numeric()
        .focus();


      var buttons = $('<div>')
      .addClass('ticket-custom-edit-buttons')
      .appendTo(container);

      $('<button>')
      .text('Cancel')
      .click(function (e) {
        parent.show();
        container.remove();
      })
      .appendTo(buttons)
      .button();

      $('<button>')
      .text('Save')
      .click(function (e) {
        parent.show().find('img').show();
        var value = input.val();
        container.remove();
        if (field.IsRequired && (value === null || $.trim(value) === '')) {
          result.parent().addClass('ui-state-error-custom ui-corner-all');
        }
        else {
          result.parent().removeClass('ui-state-error-custom ui-corner-all');
        }
        if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
          result.parent().addClass('ui-state-error-to-close-custom ui-corner-all');
          alert("This field can not be cleared in a closed ticket");
          parent.find('img').hide().next().show().delay(800).fadeOut(400);
          return;
        }
        else {
          result.parent().removeClass('ui-state-error-to-close-custom ui-corner-all');
        }
        if (value === null || $.trim(value) === '') {
          result.parent().addClass('is-empty');
        }
        else {
          result.parent().removeClass('is-empty');
        }
        top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
          parent.find('img').hide().next().show().delay(800).fadeOut(400);
          parent.closest('.ticket-name-value').data('field', result);
          parent.find('a').text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
          window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
        }, function () {
          alert("There was a problem saving your ticket property.");
        });
      })
      .appendTo(buttons)
      .button();
    });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('ui-state-error-custom ui-corner-all');
  }
  if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('ui-state-error-to-close-custom ui-corner-all');
  }
  if (field.IsRequiredToClose) {
    result.parent().addClass('is-required-to-close');
  }
  if (field.Value === null || $.trim(field.Value) === '') {
    result.parent().addClass('is-empty');
  }
}

var appendCustomEditBool = function (field, element) {
  var result = $('<a>')
    .attr('href', '#')
    .text((field.Value === null || $.trim(field.Value) === '' ? 'False' : field.Value))
    .addClass('value ui-state-default ts-link')
    .appendTo(element)
    .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
    .click(function (e) {
      e.preventDefault();
      $('.ticket-cutstom-edit').prev().show().next().remove();

      var parent = $(this).parent();
      var value = $(this).text() === 'No' || $(this).text() === 'False' ? true : false;
      parent.find('img').show();
      top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
        parent.find('img').hide().next().show().delay(800).fadeOut(400);
        parent.closest('.ticket-name-value').data('field', result);
        parent.find('a').text((result.Value === null || $.trim(result.Value) === '' ? 'False' : result.Value));
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
      }, function () {
        alert("There was a problem saving your ticket property.");
      });
    });
}

var appendCustomEdit = function (field, element) {
  var result = $('<a>')
      .attr('href', '#')
      .text((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : field.Value))
      .addClass('value ui-state-default ts-link')
      .appendTo(element)
      .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
      .after(getUrls(field.Value))
      .click(function (e) {
        e.preventDefault();
        $('.ticket-cutstom-edit').prev().show().next().remove();
        var parent = $(this).parent().hide();
        var container = $('<div>')
          .addClass('ticket-cutstom-edit')
          .css('marginTop', '1em')
          .insertAfter(parent);
        var fieldValue = parent.closest('.ticket-name-value').data('field').Value;
        var input = $('<input type="text">')
            .addClass('ui-widget-content ui-corner-all ticket-cutstom-edit-text-input')
            .css('width', '100%')
            .val(fieldValue)
            .appendTo(container)
            .focus();

        var fieldMask = parent.closest('.ticket-name-value').data('field').Mask;
        if (fieldMask) {
          input.mask(fieldMask);
          input.attr("placeholder", fieldMask);
        }

        var buttons = $('<div>')
          .addClass('ticket-custom-edit-buttons')
          .appendTo(container);

        $('<button>')
          .text('Cancel')
          .click(function (e) {
            parent.show();
            container.remove();
          })
          .appendTo(buttons)
          .button();

        $('<button>')
          .text('Save')
          .click(function (e) {
            parent.show().find('img').show();
            var value = input.val();
            container.remove();
            if (field.IsRequired && (value === null || $.trim(value) === '')) {
              result.parent().addClass('ui-state-error-custom ui-corner-all');
            }
            else {
              result.parent().removeClass('ui-state-error-custom ui-corner-all');
            }
            if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
              result.parent().addClass('ui-state-error-to-close-custom ui-corner-all');
              alert("This field can not be cleared in a closed ticket");
              parent.find('img').hide().next().show().delay(800).fadeOut(400);
              return;
            }
            else {
              result.parent().removeClass('ui-state-error-to-close-custom ui-corner-all');
            }
            if (value === null || $.trim(value) === '') {
              result.parent().addClass('is-empty');
            }
            else {
              result.parent().removeClass('is-empty');
            }
            top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
              parent.find('img').hide().next().show().delay(800).fadeOut(400);
              parent.closest('.ticket-name-value').data('field', result);
              parent.find('a.value').text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
              parent.find('.valueLink').remove();
              parent.find('a.value').after(getUrls(result.Value));
              window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
            }, function () {
              alert("There was a problem saving your ticket property.");
            });
          })
          .appendTo(buttons)
          .button();
      });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('ui-state-error-custom ui-corner-all');
  }
  if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('ui-state-error-to-close-custom ui-corner-all');
  }
  if (field.IsRequiredToClose) {
    result.parent().addClass('is-required-to-close');
  }
  if (field.Value === null || $.trim(field.Value) === '') {
    result.parent().addClass('is-empty');
  }
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
    result = result + '<a target="_blank" class="valueLink" href="' + url + '" title="' + matchArray[0] + '"><i class="fa fa-external-link fa-lg customFieldLink"></i></a>'
  }

  return result;
}

var appendCustomEditDate = function (field, element) {
  var date = field.Value == null ? null : top.Ts.Utils.getMsDate(field.Value);
  var result = $('<a>')
      .attr('href', '#')
      .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDatePattern())))
      .addClass('value ui-state-default ts-link')
      .appendTo(element)
      .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
      .click(function (e) {
        e.preventDefault();
        $('.ticket-cutstom-edit').prev().show().next().remove();
        var parent = $(this).parent().hide();
        var container = $('<div>')
          .addClass('ticket-cutstom-edit')
          .css('marginTop', '1em')
          .insertAfter(parent);
        var fieldValue = parent.closest('.ticket-name-value').data('field').Value;
        var input = $('<input type="text">')
            .addClass('ui-widget-content ui-corner-all ticket-cutstom-edit-text-input')
            .css('width', '100%')
            .appendTo(container)
            .datepicker({ dateFormat: dateFormat })
            //.datetimepicker('setDate', top.Ts.Utils.getMsDate(fieldValue))
            .focus();

        var buttons = $('<div>')
          .addClass('ticket-custom-edit-buttons')
          .appendTo(container);

        $('<button>')
          .text('Cancel')
          .click(function (e) {
            parent.show();
            container.remove();
          })
          .appendTo(buttons)
          .button();

        $('<button>')
          .text('Save')
          .click(function (e) {
            parent.show().find('img').show();
            var value = top.Ts.Utils.getMsDate(input.datepicker('getDate'));
            container.remove();
            if (field.IsRequired && (value === null || $.trim(value) === '')) {
              result.parent().addClass('ui-state-error-custom ui-corner-all');
            }
            else {
              result.parent().removeClass('ui-state-error-custom ui-corner-all');
            }
            if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
              result.parent().addClass('ui-state-error-to-close-custom ui-corner-all');
              alert("This field can not be cleared in a closed ticket");
              parent.find('img').hide().next().show().delay(800).fadeOut(400);
              return;
            }
            else {
              result.parent().removeClass('ui-state-error-to-close-custom ui-corner-all');
            }
            if (value === null || $.trim(value) === '') {
              result.parent().addClass('is-empty');
            }
            else {
              result.parent().removeClass('is-empty');
            }
            top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
              parent.find('img').hide().next().show().delay(800).fadeOut(400);
              parent.closest('.ticket-name-value').data('field', result);
              var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
              parent.find('a').text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDatePattern())))
              window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
            }, function () {
              alert("There was a problem saving your ticket property.");
            });
          })
          .appendTo(buttons)
          .button();
      });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('ui-state-error-custom ui-corner-all');
  }
  if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('ui-state-error-to-close-custom ui-corner-all');
  }
  if (field.IsRequiredToClose) {
    result.parent().addClass('is-required-to-close');
  }
  if (field.Value === null || $.trim(field.Value) === '') {
    result.parent().addClass('is-empty');
  }
}

var appendCustomEditTime = function (field, element) {
  var date = field.Value == null ? null : top.Ts.Utils.getMsDate(field.Value);
  var result = $('<a>')
      .attr('href', '#')
      .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getTimePattern())))
      .addClass('value ui-state-default ts-link')
      .appendTo(element)
      .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
      .click(function (e) {
        e.preventDefault();
        $('.ticket-cutstom-edit').prev().show().next().remove();
        var parent = $(this).parent().hide();
        var container = $('<div>')
          .addClass('ticket-cutstom-edit')
          .css('marginTop', '1em')
          .insertAfter(parent);
        var fieldValue = parent.closest('.ticket-name-value').data('field').Value;
        var input = $('<input type="text">')
            .addClass('ui-widget-content ui-corner-all ticket-cutstom-edit-text-input')
            .css('width', '100%')
            .appendTo(container)
            .timepicker()
            .timepicker('setDate', top.Ts.Utils.getMsDate(fieldValue))
            .focus();

        var buttons = $('<div>')
          .addClass('ticket-custom-edit-buttons')
          .appendTo(container);

        $('<button>')
          .text('Cancel')
          .click(function (e) {
            parent.show();
            container.remove();
          })
          .appendTo(buttons)
          .button();

        $('<button>')
          .text('Save')
          .click(function (e) {
            parent.show().find('img').show();
            var time = new Date("January 1, 1970 00:00:00");
            time.setHours(input.timepicker('getDate')[0].value.substring(0, 2));
            time.setMinutes(input.timepicker('getDate')[0].value.substring(3, 5));
            var value = top.Ts.Utils.getMsDate(time);
            container.remove();
            if (field.IsRequired && (value === null || $.trim(value) === '')) {
              result.parent().addClass('ui-state-error-custom ui-corner-all');
            }
            else {
              result.parent().removeClass('ui-state-error-custom ui-corner-all');
            }
            if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
              result.parent().addClass('ui-state-error-to-close-custom ui-corner-all');
              alert("This field can not be cleared in a closed ticket");
              parent.find('img').hide().next().show().delay(800).fadeOut(400);
              return;
            }
            else {
              result.parent().removeClass('ui-state-error-to-close-custom ui-corner-all');
            }
            if (value === null || $.trim(value) === '') {
              result.parent().addClass('is-empty');
            }
            else {
              result.parent().removeClass('is-empty');
            }
            top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
              parent.find('img').hide().next().show().delay(800).fadeOut(400);
              parent.closest('.ticket-name-value').data('field', result);
              var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
              parent.find('a').text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getTimePattern())))
              window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
            }, function () {
              alert("There was a problem saving your ticket property.");
            });
          })
          .appendTo(buttons)
          .button();
      });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('ui-state-error-custom ui-corner-all');
  }
  if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('ui-state-error-to-close-custom ui-corner-all');
  }
  if (field.IsRequiredToClose) {
    result.parent().addClass('is-required-to-close');
  }
  if (field.Value === null || $.trim(field.Value) === '') {
    result.parent().addClass('is-empty');
  }
}

var appendCustomEditDateTime = function (field, element) {
  var date = field.Value == null ? null : top.Ts.Utils.getMsDate(field.Value);
  var result = $('<a>')
    .attr('href', '#')
    .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDateTimePattern())))
    .addClass('value ui-state-default ts-link')
    .appendTo(element)
    .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
    .click(function (e) {
      e.preventDefault();
      $('.ticket-cutstom-edit').prev().show().next().remove();
      var parent = $(this).parent().hide();
      var container = $('<div>')
      .addClass('ticket-cutstom-edit')
      .css('marginTop', '1em')
      .insertAfter(parent);
      var fieldValue = parent.closest('.ticket-name-value').data('field').Value;
      var input = $('<input type="text">')
        .addClass('ui-widget-content ui-corner-all ticket-cutstom-edit-text-input')
        .css('width', '100%')
        .appendTo(container)
        .datetimepicker({ dateFormat: dateFormat })
        .datetimepicker('setDate', top.Ts.Utils.getMsDate(fieldValue))
        .focus();

      var buttons = $('<div>')
      .addClass('ticket-custom-edit-buttons')
      .appendTo(container);

      $('<button>')
      .text('Cancel')
      .click(function (e) {
        parent.show();
        container.remove();
      })
      .appendTo(buttons)
      .button();

      $('<button>')
      .text('Save')
      .click(function (e) {
        parent.show().find('img').show();
        var value = top.Ts.Utils.getMsDate(input.datetimepicker('getDate'));
        container.remove();
        if (field.IsRequired && (value === null || $.trim(value) === '')) {
          result.parent().addClass('ui-state-error-custom ui-corner-all');
        }
        else {
          result.parent().removeClass('ui-state-error-custom ui-corner-all');
        }
        if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
          result.parent().addClass('ui-state-error-to-close-custom ui-corner-all');
          alert("This field can not be cleared in a closed ticket");
          parent.find('img').hide().next().show().delay(800).fadeOut(400);
          return;
        }
        else {
          result.parent().removeClass('ui-state-error-to-close-custom ui-corner-all');
        }
        if (value === null || $.trim(value) === '') {
          result.parent().addClass('is-empty');
        }
        else {
          result.parent().removeClass('is-empty');
        }
        top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
          parent.find('img').hide().next().show().delay(800).fadeOut(400);
          parent.closest('.ticket-name-value').data('field', result);
          var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
          parent.find('a').text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDateTimePattern())))
          window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
        }, function () {
          alert("There was a problem saving your ticket property.");
        });
      })
      .appendTo(buttons)
      .button();
    });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('ui-state-error-custom ui-corner-all');
  }
  if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('ui-state-error-to-close-custom ui-corner-all');
  }
  if (field.IsRequiredToClose) {
    result.parent().addClass('is-required-to-close');
  }
  if (field.Value === null || $.trim(field.Value) === '') {
    result.parent().addClass('is-empty');
  }
}

var appendCategorizedCustomValues = function (fields) {
  top.Ts.Services.CustomFields.GetCategories(top.Ts.ReferenceTypes.Tickets, _ticketTypeID, function (categories) {
    var container = $('#divProperties');
    for (var j = 0; j < categories.length; j++) {
      var isFirstFieldAdded = true;
      for (var i = 0; i < fields.length; i++) {
        var item = null;

        var field = fields[i];

        if (field.CustomFieldCategoryID == categories[j].CustomFieldCategoryID) {
          if (isFirstFieldAdded) {
            isFirstFieldAdded = false;
            var header = $('<h3>').text(categories[j].Category).addClass('customFieldCategoryHeader collapsable');
            //var icon = $('<span>').addClass('ui-icon ui-icon-triangle-1-s');
            //header.append(icon);
            container.append(header);
            var separator = $('<div>').addClass('ts-separator ui-widget-content');
            container.append(separator);
          }
          var div = $('<div>').addClass('ticket-name-value').data('field', field);
          $('<span>')
                .addClass('property')
                .text(field.Name + ': ')
                .appendTo(div);

          switch (field.FieldType) {
            case top.Ts.CustomFieldType.Text: appendCustomEdit(field, div); break;
            case top.Ts.CustomFieldType.Date: appendCustomEditDate(field, div); break;
            case top.Ts.CustomFieldType.Time: appendCustomEditTime(field, div); break;
            case top.Ts.CustomFieldType.DateTime: appendCustomEditDateTime(field, div); break;
            case top.Ts.CustomFieldType.Boolean: appendCustomEditBool(field, div); break;
            case top.Ts.CustomFieldType.Number: appendCustomEditNumber(field, div); break;
            case top.Ts.CustomFieldType.PickList: appendCustomEditCombo(field, div, false); break;
            default:
          }

          container.append(div);
        }
      }
    }
    appendConditionalFields();
  });
}

var loadCustomValues = function () {
  $('#divProperties').empty().addClass('ts-loading');
  top.Ts.Services.Tickets.GetCustomValues(_ticketID, function (values) {



  }, function () { alert('There was a problem retrieving the ticket properties.'); });
}

var appendConditionalFields = function () {
  for (var i = 0; i < _parentFields.length; i++) {
    var field = _parentFields[i].data('field');
    $('.' + field.CustomFieldID + 'children').remove();
    var childrenContainer = $('<div>').addClass(field.CustomFieldID + 'children  customFieldIndent').insertAfter(_parentFields[i]);
    appendMatchingParentValueFields(childrenContainer, field);
  }
}

var appendMatchingParentValueFields = function (container, parentField) {
  top.Ts.Services.Tickets.GetMatchingParentValueFields(_ticketID, parentField.CustomFieldID, parentField.Value, function (fields) {
    for (var i = 0; i < fields.length; i++) {
      var item = null;

      var field = fields[i];

      var div = $('<div>').addClass('ticket-name-value').data('field', field);
      $('<span>')
            .addClass('property')
            .text(field.Name + ': ')
            .appendTo(div);

      container.append(div);

      switch (field.FieldType) {
        case top.Ts.CustomFieldType.Text: appendCustomEdit(field, div); break;
        case top.Ts.CustomFieldType.Date: appendCustomEditDate(field, div); break;
        case top.Ts.CustomFieldType.Time: appendCustomEditTime(field, div); break;
        case top.Ts.CustomFieldType.DateTime: appendCustomEditDateTime(field, div); break;
        case top.Ts.CustomFieldType.Boolean: appendCustomEditBool(field, div); break;
        case top.Ts.CustomFieldType.Number: appendCustomEditNumber(field, div); break;
        case top.Ts.CustomFieldType.PickList: appendCustomEditCombo(field, div, true); break;
        default:
      }
    }
  });
}

var setProduct = function (id, name) {
  $('#product').text((name == null || name == '') ? 'Unassigned' : name).data('productID', id);
  $('.ticket-product-info').hide();
  if (id !== null) {
    $('.ticket-product-info')
      .show()
      .attr('rel', '../../../Tips/Product.aspx?ProductID=' + id + '&TicketID=' + _ticketID)
      .cluetip(clueTipOptions);
  }
  top.Ts.Services.Organizations.IsProductRequired(function (result) {
    if (result && (name == null || name == ''))
      $('#product').parent().addClass('ui-state-error-custom ui-corner-all');
    else
      $('#product').parent().removeClass('ui-state-error-custom ui-corner-all');
  });
}

var setVersion = function (id, name, isResolved) {
  if (isResolved === true) {
    $('#resolved').text(id == null ? 'Unassigned' : name);
    $('.ticket-resolved-info').hide();
    if (id !== null) {
      $('.ticket-resolved-info')
      .show()
      .attr('rel', '../../../Tips/Version.aspx?VersionID=' + id + '&TicketID=' + _ticketID)
      .cluetip(clueTipOptions);
    }
  } else {
    $('.ticket-reported-info').hide();
    if (id !== null) {
      $('.ticket-reported-info')
      .show()
      .attr('rel', '../../../Tips/Version.aspx?VersionID=' + id + '&TicketID=' + _ticketID)
      .cluetip(clueTipOptions);
    }
    $('#reported').text(id == null ? 'Unassigned' : name);

    top.Ts.Services.Organizations.IsProductVersionRequired(function (result) {
      if (result && id == null)
        $('#reported').parent().addClass('ui-state-error-custom ui-corner-all');
      else
        $('#reported').parent().removeClass('ui-state-error-custom ui-corner-all');
    });
  }

}

var isFormValid = function (callback) {
  top.Ts.Settings.Organization.read('RequireNewTicketCustomer', false, function (requireNewTicketCustomer) {
    var result = true;

    if ($('.ui-state-error-custom').length > 0) {
      result = false;
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
}

var isFormValidToClose = function (isClosed, callback) {
  var result = true;
  if (isClosed) {
    $('.is-required-to-close.is-empty').addClass('ui-state-error-to-close-custom ui-corner-all');
    if ($('.ui-state-error-to-close-custom').length > 0) {
      result = false;
    }
  }
  callback(result);
}

var getCompany = function (request, response) {
  if (execGetCompany) { execGetCompany._executor.abort(); }
  execGetCompany = top.Ts.Services.Organizations.WCSearchOrganization(request.term, function (result) { response(result); });
}

var clearDialog = function () {
  $('#dialog-ticketmerge-search').val("");
  $('#dialog-ticketmerge-preview').hide();
  $('#dialog-ticketmerge-warning').hide();
  $('#ticketmerge-preview-details').next().remove();
  $('#ticketmerge-preview-details').next().remove();
  $('#ticketmerge-preview-details').next().remove();
  $('#dialog-ticketmerge-confirm').attr("checked", false);
  $(".dialog-ticketmerge").dialog("widget").find(".ui-dialog-buttonpane").removeClass('saving');
  $(".dialog-ticketmerge").dialog("widget").find(".ui-dialog-buttonpane").find(":button:contains('OK')").prop("disabled", true).addClass("ui-state-disabled");
}

var addUserViewing = function (userID) {
  $('.ticket-now-viewing').show();

  if ($('.ticket-viewer:data(ChatID=' + userID + ')').length < 1) {
    top.Ts.Services.Users.GetUser(userID, function (user) {
      var fullName = user.FirstName + " " + user.LastName;
      var viewuser = $('<div>')
              .data('ChatID', user.UserID)
              .data('Name', fullName)
              .addClass('ticket-viewer')
              .click(function () {
                window.parent.openChat($(this).data('Name'), $(this).data('ChatID'));
                top.Ts.System.logAction('Now Viewing - Chat Opened');
              })
              .html('<a class="ui-state-default ts-link" href="#"><img class="ticket-viewer-avatar" src="' + user.Avatar + '">' + fullName + '</a>')
              .appendTo($('#currentViewingUsers'));
    });
  }




}

var removeUserViewing = function (ticketNum, userID) {

  if (ticketNum != _ticketNumber) {
    if ($('.ticket-viewer:data(ChatID=' + userID + ')').length > 0) {
      $('.ticket-viewer:data(ChatID=' + userID + ')').remove();
      if ($('.ticket-viewer').length < 1) {
        $('.ticket-now-viewing').hide();
      }
    }
  }
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