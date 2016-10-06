﻿var _taskDetailPage = null;
var _reminderID = null;

$(document).ready(function () {
    _taskDetailPage = new TaskDetailPage();
    _taskDetailPage.refresh();
    $('.task-tooltip').tooltip({ placement: 'bottom', container: 'body' });

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

    _reminderID = window.parent.parent.Ts.Utils.getQueryValue("reminderid", window);
    parent.privateServices.SetUserSetting('SelectedReminderID', _reminderID);

    LoadProperties();
    initAssociationControls();

    var ellipseString = function (text, max) { return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text; };

    function LoadProperties() {
        window.parent.parent.Ts.Services.Task.GetTask(_reminderID, function (task) {
            if (task.TaskName) {
                $('#taskName').html(ellipseString(task.TaskName, 73));
            }
            else if (task.Description) {
                $('#taskName').html(ellipseString(task.Description, 73));
            }
            else {
                $('#taskName').html(task.ReminderID);
            }

            $('#fieldUser').text(task.UserName == "" ? "Unassigned" : task.UserName);
            $('#fieldUser').data('field', task.UserID);
            $('#fieldComplete').text(task.TaskIsComplete);
            $('#fieldDueDate').text(window.parent.parent.Ts.Utils.getMsDate(task.TaskDueDate).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()));
            $('#fieldReminder').text(!task.IsDismissed);
            $('#fieldReminderDate').text(window.parent.parent.Ts.Utils.getMsDate(task.DueDate).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()));

            $('#fieldCreator').text(task.Creator);
            $('#fieldDateCreated').text(window.parent.parent.Ts.Utils.getMsDate(task.DateCreated).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()));
            //$('#fieldModifier').text(task.ModifierName);
            //$('#fieldDateModified').text(window.parent.parent.Ts.Utils.getMsDate(task.DateModified).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()));

            $('#fieldDescription').html(task.Description != null && task.Description != "" ? task.Description : "Empty");
        });
    }

    function initAssociationControls() {
        $("#attachmentinput").show();
        $(".arrow-up").css('left', '7px');

        $("#ticketinput").hide();
        $("#userinput").hide();
        $("#customerinput").hide();
        $("#groupinput").hide();
        $("#productinput").hide();
    }

    $('#taskEdit').click(function (e) {
        $('.taskProperties p').toggleClass("editable");
        $(this).toggleClass("btn-primary");
        $(this).toggleClass("btn-success");
    });

    $('#fieldDescription').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        window.parent.Ts.System.logAction('Task Detail - Edit Description');
        window.parent.Ts.Services.Task.GetTask(_reminderID, function (task) {
            var desc = task.Description;
            desc = desc.replace(/<br\s?\/?>/g, "\n");
            //        $('#fieldDesc').tinymce().setContent(desc);
            //        $('#fieldDesc').tinymce().focus();
            $('#fieldDesc').html(desc);
            $('#descriptionContent').hide();
            $('#descriptionForm').show();
        });

        $('#btnDescriptionCancel').click(function (e) {
            e.preventDefault();
            $('#descriptionForm').hide();
            $('#descriptionContent').show();
            header.show();
            $('#taskEdit').removeClass("disabled");
        });

        $('#btnDescriptionSave').click(function (e) {
            e.preventDefault();
            window.parent.Ts.System.logAction('Task Detail - Save Description Edit');
            window.parent.Ts.Services.Task.SetDescription(_reminderID, $(this).prev().find('textarea').val(), function (result) {
                header.html(result);
                $('#taskEdit').removeClass("disabled");
            },
            function (error) {
                header.show();
                alert('There was an error saving the task description.');
                $('#taskEdit').removeClass("disabled");
            });

            $('#descriptionForm').hide();
            $('#descriptionContent').show();
            header.show();
        })
        $('#taskEdit').addClass("disabled");
    });

    $('.taskProperties p').toggleClass("editable");
});


TaskDetailPage = function () {

};

TaskDetailPage.prototype = {
    constructor: TaskDetailPage,
    refresh: function () {

    }
};
