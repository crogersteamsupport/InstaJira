var _taskDetailPage = null;
var _taskID = null;
var _Name = null;
var _historyLoaded = 0;
var _subtasksLoaded = 0;
var _completeCommentTaskID = 0;

$(document).ready(function () {
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.async = true;
    script.src = ('https:' === document.location.protocol ? 'https://' : 'http://') + 'www.dropbox.com/static/api/1/dropbox.js';
    var firstScript = document.getElementsByTagName('script')[0];
    script.setAttribute('data-app-key', 'ebdoql1dhyy7l72');
    script.setAttribute('id', 'dropboxjs');
    if (window.parent.Ts.System.User.OrganizationID != 1150007)
        firstScript.parentNode.insertBefore(script, firstScript);

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

    _taskID = window.parent.parent.Ts.Utils.getQueryValue("taskid", window);
    parent.privateServices.SetUserSetting('SelectedTaskID', _taskID);
    var _isAdmin = window.parent.parent.Ts.System.User.IsSystemAdmin;

    LoadProperties();
    initAssociationControls();
    LoadAssociations();
    LoadSubtasks();

    var ellipseString = function (text, max) {
        if (text != null) {
            return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text;
        }
        else return null;
    };

    function LoadProperties() {
        window.parent.parent.Ts.Services.Task.GetTask(_taskID, function (task) {
            if (_isAdmin || task.CreatorID == window.parent.parent.Ts.System.User.UserID) {
                $('#taskDelete').show();
            } else {
                $('#taskDelete').hide();
            }
            if (task.Name) {
                $('#Name').text(ellipseString(task.Name, 73));
            } else if (task.Description) {
                $('#Name').text(ellipseString(task.Description, 73));
            } else {
                $('#Name').text(task.TaskID);
            }

            _Name = $('#Name').text();

            $('#fieldUser').text(task.UserName == null ? "Unassigned" : task.UserName);
            $('#fieldUser').data('field', task.UserID);

            if (task.IsComplete) {
                $('#fieldComplete').text("Yes");
                $('#taskComplete').html("<i class='fa fa-check'></i>");
                $('#taskComplete').addClass("completedButton");
                $('#taskComplete').removeClass("emptyButton");
                $('#taskComplete').attr("data-original-title", "Uncomplete this task");
                $('#taskComplete').tooltip('fixTitle');
                $('#reminderDateGroup').hide();
                $('.completedData').show();
                $('#fieldCompletionDate').html(task.DateCompleted == null ? "None" : window.parent.parent.Ts.Utils.getMsDate(task.DateCompleted).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()));
                $('#fieldCompletionComment').text(!task.CompletionComment ? "None" : task.CompletionComment);
            } else {
                $('#fieldComplete').text("No");
                $('#taskComplete').html("Mark Completed");
                $('#taskComplete').addClass("emptyButton");
                $('#taskComplete').removeClass("completedButton");
                $('#taskComplete').attr("data-original-title", "Complete this task");
                $('#taskComplete').tooltip('fixTitle');
                $('#reminderDateGroup').show();
                $('.completedData').hide();
            }
            $('#fieldDueDate').html(task.DueDate == null ? "None" : window.parent.parent.Ts.Utils.getMsDate(task.DueDate).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()) + '<i id="clearDueDate" class="col-xs-1 fa fa-times clearDate"></i>');
            $('#fieldReminder').html(task.ReminderDueDate == null ? "None" : window.parent.parent.Ts.Utils.getMsDate(task.ReminderDueDate).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()) + '<i id="clearReminderDate" class="col-xs-1 fa fa-times clearDate"></i>');
            $('#fieldCreator').text(task.Creator);
            $('#fieldDateCreated').text(window.parent.parent.Ts.Utils.getMsDate(task.DateCreated).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()));
            $('#fieldDescription').html(task.Description != null && task.Description != "" ? task.Description : "Empty");

            if (task.ParentID) {
                $('#subtasksDiv').hide();
                var parentName = $('<h6>').addClass('parentName');
                $('<a>').attr('href', '#').addClass('parentLink').data('taskID', task.ParentID).text(task.TaskParentName + ' >').appendTo(parentName)
                $('.header-nav').prepend(parentName);
                $('.btn-toolbar').addClass('subtaskButtonsAdjustement');
            }

            teamsupport.page.data = { id:task.TaskID, parent:task.ParentID };
        });
    }

    function initAssociationControls() {
        $("#attachmentinput").show();
        $(".arrow-up").css('left', '7px');
        $("#ticketinput").hide();
        $("#userinput").hide();
        $("#customerinput").hide();
        $("#contactinput").hide();
        $("#groupinput").hide();
        $("#productinput").hide();
        $("#activityinput").hide();
    }

    function resetDisplay() {
        $('#commentatt').find('.upload-queue').empty();
        $('#commentatt').find('.ticket-queue').empty();
        $('#commentatt').find('.group-queue').empty();
        $('#commentatt').find('.customer-queue').empty();
        $('#commentatt').find('.contact-queue').empty();
        $('#commentatt').find('.user-queue').empty();
        $('#commentatt').find('.product-queue').empty();
        $('#commentatt').find('.activity-queue').empty();
        $(".newticket-group").val(-1);
        $(".newticket-product").val(-1);
    }

    function LoadAssociations() {
        window.parent.parent.Ts.Services.Task.GetAttachments(_taskID, function (attachments) {
            var attdiv = $('#associationsContainer');
            attdiv.empty();
            //if (attachments.length > 0) {
            //    var attdiv = $('<div>')
            //    .addClass('attachment-list')
            //    .appendTo($('#associationsContainer'));
            //}
            for (var i = 0; i < attachments.length; i++) {
                var blockDiv = $('<div>')
                .addClass('associations')
                .data('attachmentID', attachments[i].AttachmentID)
                .hover(function (e) {
                    $(this).find('.associationDelete').show();
                    //if ($(this).attr('filetype').indexOf('image') >= 0) {
                    //    $("body").append("<p id='preview'><img src='" + this.href + "' alt='Image preview' style='max-width:400px' /></p>");
                    //    $("#preview")
			        //.css("top", (e.pageY - 10) + "px")
			        //.css("left", (e.pageX + 30) + "px")
			        //.fadeIn("fast");
                    //}
                },
	            function () {
	                $(this).find('.associationDelete').hide();
	                $("#preview").remove();
	            })
                .appendTo(attdiv);

                var atticon = $('<i>')
                .addClass('fa-paperclip fa fa-fw')
                .appendTo(blockDiv);

                $('<a>')
                .attr('target', '_blank')
                .attr('filetype', attachments[i].FileType)
                .text(ellipseString(attachments[i].FileName, 100))
                .addClass('attfilename ui-state-default ts-link preview')
                .attr('href', '../../../dc/1/attachments/' + attachments[i].AttachmentID)
                .appendTo(blockDiv);

                $('<i>')
                .addClass('fa fa-trash-o associationDelete')
                .hide()
                .appendTo(blockDiv);
            }

            window.parent.parent.Ts.Services.Task.LoadAssociations(_taskID, function (associations) {
                for (var i = 0; i < associations.length; i++) {
                    var blockDiv = $('<div>')
                    .addClass('associations')
                    .data('refID', associations[i].RefID)
                    .data('refType', associations[i].RefType)
                    .hover(function (e) {
                        $(this).find('.associationDelete').show();
                        //if ($(this).attr('filetype').indexOf('image') >= 0) {
                        //    $("body").append("<p id='preview'><img src='" + this.href + "' alt='Image preview' style='max-width:400px' /></p>");
                        //    $("#preview")
                        //.css("top", (e.pageY - 10) + "px")
                        //.css("left", (e.pageX + 30) + "px")
                        //.fadeIn("fast");
                        //}
                    },
                    function () {
                        $(this).find('.associationDelete').hide();
                        //$("#preview").remove();
                    })
                    .appendTo(attdiv);

                    var atticon = $('<i>')
                    //.addClass('ts-icon ts-icon-attachment')
                    .appendTo(blockDiv);

                    var link = $('<a>')
                    .attr('target', '_blank')
                    //.attr('filetype', associations[i].RefType)
                    //.text(ellipseString(attachments[i].FileName, 20))
                    .addClass('attfilename ui-state-default ts-link preview attfilenamefix')
                    //.attr('href', '../../../dc/1/attachments/' + attachments[i].AttachmentID)
                    .appendTo(blockDiv);

                    switch (associations[i].RefType) {
                        case window.parent.parent.Ts.ReferenceTypes.Tickets:
                            atticon.addClass('ticketIcon fa fa-ticket fa-fw');
                            link.text(ellipseString(associations[i].TicketNumber + ': ' + associations[i].TicketName, 100));
                            link.attr('href', window.parent.parent.Ts.System.AppDomain + '?TicketID=' + associations[i].RefID);
                            link.attr('target', '_blank');
                            link.attr('ticket', associations[i].TicketNumber);
                            link.attr('onclick', 'window.parent.parent.Ts.MainPage.openTicketByID(' + associations[i].RefID + '); return false;');
                            break;
                        case window.parent.parent.Ts.ReferenceTypes.Users:
                            atticon.addClass('userIcon fa fa-user fa-fw');
                            link.text(ellipseString(associations[i].User, 100));
                            link.attr('href', '#');
                            link.attr('target', '_blank');
                            link.attr('onclick', 'window.parent.parent.Ts.MainPage.openUser(' + associations[i].RefID + '); return false;');
                            break;
                        case window.parent.parent.Ts.ReferenceTypes.Organizations:
                            atticon.addClass('companyIcon fa fa-building fa-fw');
                            link.text(ellipseString(associations[i].Company, 100));
                            link.attr('href', '#');
                            link.attr('target', '_blank');
                            link.attr('onclick', 'window.parent.parent.Ts.MainPage.openNewCustomer(' + associations[i].RefID + '); return false;');
                            break;
                        case window.parent.parent.Ts.ReferenceTypes.Contacts:
                            atticon.addClass('contactIcon fa fa-user-circle-o fa-fw');
                            link.text(ellipseString(associations[i].Contact, 100));
                            link.attr('href', '#');
                            link.attr('target', '_blank');
                            link.attr('onclick', 'window.parent.parent.Ts.MainPage.openNewContact(' + associations[i].RefID + '); return false;');
                            break;
                        case window.parent.parent.Ts.ReferenceTypes.Groups:
                            atticon.addClass('groupIcon fa fa-users fa-fw');
                            link.text(ellipseString(associations[i].Group, 100));
                            link.attr('href', '#');
                            link.attr('target', '_blank');
                            link.attr('onclick', 'window.parent.parent.Ts.MainPage.openGroup(' + associations[i].RefID + '); return false;');
                            break;
                        case window.parent.parent.Ts.ReferenceTypes.Products:
                            atticon.addClass('productIcon fa fa-truck fa-fw');
                            link.text(ellipseString(associations[i].Product, 100));
                            link.attr('href', '#');
                            link.attr('target', '_blank');
                            link.attr('onclick', 'window.parent.parent.Ts.MainPage.openNewProduct(' + associations[i].RefID + '); return false;');
                            break;
                        case window.parent.parent.Ts.ReferenceTypes.CompanyActivity:
                            atticon.addClass('fa fa-sticky-note fa-fw');
                            link.text(ellipseString(associations[i].Activity, 100));
                            link.attr('href', '#');
                            link.attr('target', '_blank');
                            link.attr('onclick', 'window.parent.parent.Ts.MainPage.openNewCustomerNote(' + associations[i].ActivityRefID + ',' + associations[i].ActivityID + '); return false;');
                            break;
                        case window.parent.parent.Ts.ReferenceTypes.ContactActivity:
                            atticon.addClass('fa fa-sticky-note fa-fw');
                            link.text(ellipseString(associations[i].Activity, 100));
                            link.attr('href', '#');
                            link.attr('target', '_blank');
                            link.attr('onclick', 'window.parent.parent.Ts.MainPage.openNewContactNote(' + associations[i].ActivityRefID + ',' + associations[i].ActivityID + '); return false;');
                            break;
                    }

                    $('<i>')
                    .addClass('fa fa-trash-o associationDelete')
                    .hide()
                    .appendTo(blockDiv);
                }
            })
        });
    }

    function LoadSubtasks() {
        $('#tblSubtasks tbody').empty();

        window.parent.parent.Ts.Services.Task.LoadSubtasks(_taskID, function (subtasks) {
            for (var i = 0; i < subtasks.length; i++) {
                var displayName;
                if (subtasks[i].Name) {
                    displayName = ellipseString(subtasks[i].Name, 40);
                }
                else if (subtasks[i].Description) {
                    displayName = ellipseString(subtasks[i].Description, 40);
                }
                else {
                    displayName = subtasks[i].TaskID;
                }

                var row = $('<tr>').appendTo('#tblSubtasks > tbody:last');
                var checkBoxCel = $('<td>').appendTo(row);
                var checkBoxInput = $('<input>')
                    .prop('type', 'checkbox')
                    .prop('checked', subtasks[i].IsComplete)
                    .prop('id', 'st' + subtasks[i].TaskID)
                    .addClass('subtaskCheckBox')
                    .data('taskID', subtasks[i].TaskID)
                    .appendTo(checkBoxCel)

                var nameCel = $('<td>').appendTo(row);
                $('<a>')
                  .attr('href', '#')
                  .addClass('tasklink')
                  .data('taskID', subtasks[i].TaskID)
                  .text(displayName)
                  .appendTo(nameCel)

                var userCel = $('<td>').append(subtasks[i].UserName).appendTo(row);

                var dueDateCel;
                if (subtasks[i].DueDate)
                {
                    dueDateCel = $('<td>').append(subtasks[i].DueDate.localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern())).appendTo(row);
                }
                else
                {
                    dueDateCel = $('<td>').append('None').appendTo(row);
                }

                //$('<tr>').html('<td>' + subtasks[i].Name + '</td><td>' + subtasks[i].UserID + '</td><td>' + subtasks[i].DueDate.localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()) + '</td>')

                //$('#tblHistory tr:last').after('<tr><td>' + history[i].DateCreated.toDateString() + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td></tr>');
            }
            //if (history.length == 50)
            //    $('<button>').text("Load More").addClass('btn-link')
            //    .click(function (e) {
            //        LoadHistory($('#tblSubtasks tbody > tr').length + 1);
            //        $(this).remove();
            //    })
            //   .appendTo('#tblSubtasks > tbody:last');
        });
    }

    function LoadHistory(start) {
        if (start == 1)
            $('#tblHistory tbody').empty();

        window.parent.parent.Ts.Services.Task.LoadHistory(_taskID, start, function (history) {
            for (var i = 0; i < history.length; i++) {
                $('<tr>').html('<td>' + history[i].DateCreated.localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()) + '</td><td>' + history[i].Description + '</td>')
                .appendTo('#tblHistory > tbody:last');
                //$('#tblHistory tr:last').after('<tr><td>' + history[i].DateCreated.toDateString() + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td></tr>');
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

    $('#taskRefresh').click(function (e) {
        e.preventDefault();
        location.reload();
    });

    $('#taskDelete').click(function (e) {
        if (confirm('Are you sure you would like to remove this task and all its subtasks?')) {
            parent.privateServices.DeleteTask(_taskID, function (e) {
                window.parent.parent.Ts.System.logAction('Task Detail - Delete Task');
                //if (window.parent.document.getElementById('iframe-mniCustomers'))
                //    window.parent.document.getElementById('iframe-mniCustomers').contentWindow.refreshPage();
                _mainFrame.Ts.MainPage.closeNewTaskTab(_taskID);
                //_mainFrame.Ts.MainPage.closeNewContact(userID);
            });


        }
    });

    $('#taskComplete').click(function (e) {
        if ($(this).html() !== '<i class="fa fa-check"></i>') {
            window.parent.parent.Ts.Services.Task.GetIncompleteSubtasks(_taskID, function (result) {
                if (result) {
                    alert('Please complete all the subtasks before completing this task.')
                } else {
                    window.parent.parent.Ts.Services.Task.SetTaskIsCompleted(_taskID, true, function (result) {
                        top.Ts.System.logAction('Task Detail - Toggle TaskIsCompleted');
                        $('#fieldComplete').text("Yes");
                        $('#taskComplete').html("<i class='fa fa-check'></i>");
                        $('#taskComplete').addClass("completedButton");
                        $('#taskComplete').removeClass("emptyButton");
                        $('#taskComplete').attr("data-original-title","Uncomplete this task");
                        $('#taskComplete').tooltip('fixTitle');
                        $('#reminderDateGroup').hide();
                        _completeCommentTaskID = _taskID;
                        $('#modalTaskComment').modal('show');
                        $('#fieldCompletionDate').html(result.DateCompleted == null ? "None" : window.parent.parent.Ts.Utils.getMsDate(result.DateCompleted).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()));
                        $('#fieldCompletionComment').text(!result.CompletionComment ? "None" : result.CompletionComment);
                        $('.associations').children('a').each(function () {
                            var ticket = $(this).attr('ticket');
                            if (ticket) {
                                var frame = parent.document.getElementById('ticket-' + ticket);
                                if (frame) {
                                    frame.contentWindow.taskCheckBox(_taskID, true);
                                }
                            }
                        });

                        if (teamsupport.page.data.parent) {
                            var frame = parent.document.getElementById('iframe-o-' + teamsupport.page.data.parent);
                            if (frame) {
                                frame.contentWindow.taskCheckBox(_taskID, true);
                            }
                        }
                    },
                    function (error) {
                        header.show();
                        alert('There was an error saving the task is complete.');
                    });
                }
            },
            function (error) {
                header.show();
                alert('There was an error getting the subtasks.');
            });
        } else {
            window.parent.parent.Ts.Services.Task.SetTaskIsCompleted(_taskID, false, function (result) {
                top.Ts.System.logAction('Task Detail - Toggle TaskIsCompleted');
                $('#fieldComplete').text("No");
                $('#taskComplete').html("Mark Completed");
                $('#taskComplete').addClass("emptyButton");
                $('#taskComplete').removeClass("completedButton");
                $('#taskComplete').attr("data-original-title", "Complete this task");
                $('#taskComplete').tooltip('fixTitle');
                $('#reminderDateGroup').show();
                $('.completedData').hide();
                $('.associations').children('a').each(function () {
                    var ticket = $(this).attr('ticket');
                    if (ticket) {
                        var frame = parent.document.getElementById('ticket-' + ticket);
                        if (frame) {
                            frame.contentWindow.taskCheckBox(_taskID, false);
                        }
                    }
                });

                if (teamsupport.page.data.parent) {
                    var frame = parent.document.getElementById('iframe-o-' + teamsupport.page.data.parent);
                    if (frame) {
                        frame.contentWindow.taskCheckBox(_taskID, false);
                    }
                }
            },
            function (error) {
                header.show();
                alert('There was an error saving the task as complete.');
            });
        }
    });

    //$('#taskEdit').click(function (e) {
    //    $('p, #Name').toggleClass("editable");
    //    $(this).toggleClass("btn-primary");
    //    $(this).toggleClass("btn-success");
    //    if ($(this).hasClass("btn-primary"))
    //        $(this).html('<i class="fa fa-pencil"></i> Edit');
    //    else
    //        $(this).html('<i class="fa fa-pencil"></i> Save');
    //});

    $('#Name').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;

        top.Ts.System.logAction('Task Detail - Edit Name');
        var header = $(this).hide();
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-8')
          .appendTo(container);

        $('<input type="text">')
          .addClass('col-xs-10 form-control')
          .val(_Name)
          .appendTo(container1)
          .focusout(function (e) {
              window.parent.parent.Ts.System.logAction('Task Detail - Save Name');
              window.parent.parent.Ts.Services.Task.SetName(_taskID, $(this).val(), function (result) {
                  _Name = result;
                  header.text(result);
                  $('#Name').text(result);
                  $('#taskEdit').removeClass("disabled");
              },
              function (error) {
                  header.show();
                  alert('There was an error saving the task name.');
                  $('#taskEdit').removeClass("disabled");
              });
              $('#taskEdit').removeClass("disabled");
              $(this).closest('div').remove();
              header.show();
          })
          .keydown(function (e) {
              var code = (e.keyCode ? e.keyCode : e.which);
              if (code == 13) {
                  window.parent.parent.Ts.System.logAction('Task Detail - Save Name');
                  window.parent.parent.Ts.Services.Task.SetName(_taskID, $(this).val(), function (result) {
                      _Name = result;
                      header.text(result);
                      $('#Name').text(result);
                      $('#taskEdit').removeClass("disabled");
                  },
                  function (error) {
                      header.show();
                      alert('There was an error saving the task name.');
                      $('#taskEdit').removeClass("disabled");
                  });
                  $('#taskEdit').removeClass("disabled");
                  $(this).closest('div').remove();
                  header.show();
              }
          })
          .focus();

        $('#taskEdit').addClass("disabled");
    });

    $('#descriptionEdit').click(function (e) {
        e.preventDefault();
        var header = $('#fieldDescription').hide();
        window.parent.parent.Ts.System.logAction('Task Detail - Edit Description');
        window.parent.parent.Ts.Services.Task.GetTask(_taskID, function (task) {
            var desc = task.Description;
            desc = desc.replace(/<br\s?\/?>/g, "\n");
            //        $('#fieldDesc').tinymce().setContent(desc);
            //        $('#fieldDesc').tinymce().focus();
            //$('#fieldDesc').html(desc);

            initScheduledReportEditor($('#fieldDesc'), function (ed) {
                $('#fieldDesc').tinymce().setContent(desc);
                $('#fieldDesc').tinymce().focus();
            });

            $('#descriptionContent').hide();
            $('#descriptionForm').show();
        });

        $('#btnDescriptionCancel').click(function (e) {
            e.preventDefault();
            tinymce.activeEditor.destroy();
            $('#descriptionForm').hide();
            $('#descriptionContent').show();
            header.show();
            $('#taskEdit').removeClass("disabled");
        });

        $('#btnDescriptionSave').click(function (e) {
            e.preventDefault();
            window.parent.parent.Ts.System.logAction('Task Detail - Save Description Edit');
            window.parent.parent.Ts.Services.Task.SetDescription(_taskID, tinyMCE.activeEditor.getContent(), function (result) {
                header.html(result);
                $('#taskEdit').removeClass("disabled");
            },
            function (error) {
                header.show();
                alert('There was an error saving the task description.');
                $('#taskEdit').removeClass("disabled");
            });

            $('#descriptionForm').hide();
            tinymce.activeEditor.destroy();
            $('#descriptionContent').show();
            header.show();
        })
        $('#taskEdit').addClass("disabled");
    });

    $('#fieldUser').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        top.Ts.System.logAction('Task Detail - Edit User');
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        var select = $('<select>').addClass('form-control').attr('id', 'ddlPrimaryContact').appendTo(container1);
        var organizationID = top.Ts.System.Organization.OrganizationID;
        window.parent.parent.Ts.Services.Customers.LoadOrgUsers(organizationID, function (contacts) {
            $('<option>').attr('value', '-1').text('Unassigned').appendTo(select);
            for (var i = 0; i < contacts.length; i++) {
                var opt = $('<option>').attr('value', contacts[i].UserID).text(contacts[i].FirstName + " " + contacts[i].LastName).data('o', contacts[i]);
                if (header.data('field') == contacts[i].UserID)
                    opt.attr('selected', 'selected');
                opt.appendTo(select);
            }
        });


        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#taskEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('#ddlPrimaryContact').on('change', function () {
            var value = $(this).val();
            var name = this.options[this.selectedIndex].innerHTML;
            container.remove();
            window.parent.parent.Ts.System.logAction('Task Detail - Save User');
            window.parent.parent.Ts.Services.Task.SetUser(_taskID, value, function (result) {
                header.data('field', result);
                header.text(name);
                header.show();
                $('#taskEdit').removeClass("disabled");
            }, function () {
                alert("There was a problem saving your task user.");
                $('#taskEdit').removeClass("disabled");
            });
        });
        $('#taskEdit').addClass("disabled");
    });

    $('#fieldComplete').click(function (e) {
        if (!$(this).hasClass('editable')) {
            return false;
        }
        if ($(this).text() !== 'Yes') {
            window.parent.parent.Ts.Services.Task.GetIncompleteSubtasks(_taskID, function (result) {
                if (result) {
                    alert('Please complete all the subtasks before completing this task.')
                } else {
                    window.parent.parent.Ts.Services.Task.SetTaskIsCompleted(_taskID, ($(this).text() !== 'Yes'), function (result) {
                        top.Ts.System.logAction('Task Detail - Toggle TaskIsCompleted');
                        $('#fieldComplete').text("Yes");
                        $('#taskComplete').html("<i class='fa fa-check'></i>");
                        $('#taskComplete').addClass("completedButton");
                        $('#taskComplete').removeClass("emptyButton");
                        $('#taskComplete').attr("data-original-title", "Uncomplete this task");
                        $('#taskComplete').tooltip('fixTitle');
                    },
                    function (error) {
                        header.show();
                        alert('There was an error saving the task is complete.');
                    });
                }
            },
            function (error) {
                header.show();
                alert('There was an error getting the subtasks.');
            });
        } else {
            window.parent.parent.Ts.Services.Task.SetTaskIsCompleted(_taskID, ($(this).text() !== 'Yes'), function (result) {
                top.Ts.System.logAction('Task Detail - Toggle TaskIsCompleted');
                $('#fieldComplete').text("No");
                $('#taskComplete').html("Mark Completed");
                $('#taskComplete').addClass("emptyButton");
                $('#taskComplete').removeClass("completedButton");
                $('#taskComplete').attr("data-original-title", "Complete this task");
                $('#taskComplete').tooltip('fixTitle');
            },
            function (error) {
                header.show();
                alert('There was an error saving the task is complete.');
            });
        }
    });

    $('#fieldDueDate').on('click', '#clearDueDate', function (e) {
        e.stopPropagation();
        window.parent.parent.Ts.Services.Task.ClearDueDate(_taskID, function () {
            top.Ts.System.logAction('Task Detail - Clear Due Date');
            $('#fieldDueDate').text("None");
        },
        function (error) {
            header.show();
            alert('There was an error clearing the due date.');
        });
    });

    $('#fieldDueDate').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        window.parent.parent.Ts.System.logAction('Task Detail - Due Date Clicked');
        var parent = $(this).hide();
        var container = $('<div>')
              .insertAfter(parent);

        var container1 = $('<div>')
                .addClass('col-xs-9')
              .appendTo(container);

        var input = $('<input type="text">')
                .addClass('col-xs-10 form-control')
                .val($(this).val())
                .datetimepicker({ pickTime: true })
                .appendTo(container1)
                .focus();

        $('<i>')
              .addClass('col-xs-1 fa fa-times')
              .click(function (e) {
                  $(this).closest('div').remove();
                  parent.show();
                  $('#taskEdit').removeClass("disabled");
                  top.Ts.System.logAction('Task - Due Date change cancelled');
              })
              .insertAfter(container1);
        $('<i>')
              .addClass('col-xs-1 fa fa-check')
              .click(function (e) {
                  var taskInfo = new Object();
                  taskInfo.DueDate = input.val();
                  container.remove();
                  window.parent.parent.Ts.Services.Task.SetDueDate(_taskID, window.parent.parent.JSON.stringify(taskInfo), function (result) {
                      parent.html((result.DueDate === null ? 'Unassigned' : window.parent.parent.Ts.Utils.getMsDate(result.DueDate).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()) + '<i id="clearDueDate" class="col-xs-1 fa fa-times clearDate"></i>'))
                      $('#taskEdit').removeClass("disabled");
                      top.Ts.System.logAction('Task Detail - Due Date Change');
                  },
                  function (error) {
                      parent.show();
                      alert('There was an error saving the Task Due Date.');
                      $('#taskEdit').removeClass("disabled");
                  });
                  $('#taskEdit').removeClass("disabled");
                  $(this).closest('div').remove();
                  parent.show();
              })
              .insertAfter(container1);
        $('#taskEdit').addClass("disabled");
    });

    $('#fieldReminder').on('click', '#clearReminderDate', function (e) {
        e.stopPropagation();
        window.parent.parent.Ts.Services.Task.ClearReminderDate(_taskID, function () {
            top.Ts.System.logAction('Task Detail - Clear Reminder Date');
            $('#fieldReminder').text("None");
        },
        function (error) {
            header.show();
            alert('There was an error clearing the reminder date.');
        });
    });

    $('#fieldReminder').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        window.parent.parent.Ts.System.logAction('Task Detail - Reminder Date Clicked');
        var parent = $(this).hide();
        var container = $('<div>')
              .insertAfter(parent);

        var container1 = $('<div>')
                .addClass('col-xs-9')
              .appendTo(container);

        var input = $('<input type="text">')
                .addClass('col-xs-10 form-control')
                .val($(this).val())
                .datetimepicker({ pickTime: true })
                .appendTo(container1)
                .focus();

        $('<i>')
              .addClass('col-xs-1 fa fa-times')
              .click(function (e) {
                  $(this).closest('div').remove();
                  parent.show();
                  $('#taskEdit').removeClass("disabled");
                  top.Ts.System.logAction('Task - Reminder Date change cancelled');
              })
              .insertAfter(container1);
        $('<i>')
              .addClass('col-xs-1 fa fa-check')
              .click(function (e) {
                  var taskInfo = new Object();
                  taskInfo.Reminder = input.val();
                  container.remove();
                  window.parent.parent.Ts.Services.Task.SetReminderDueDate(_taskID, window.parent.parent.JSON.stringify(taskInfo), function (result) {
                      parent.html((result.DueDate === null ? 'Unassigned' : window.parent.parent.Ts.Utils.getMsDate(result.DueDate).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()) + '<i id="clearReminderDate" class="col-xs-1 fa fa-times clearDate"></i>'))
                      $('#taskEdit').removeClass("disabled");
                      top.Ts.System.logAction('Task Detail - Reminder Date Change');
                  },
                  function (error) {
                      parent.show();
                      alert('There was an error saving the Task Reminder Date.');
                      $('#taskEdit').removeClass("disabled");
                  });
                  $('#taskEdit').removeClass("disabled");
                  $(this).closest('div').remove();
                  parent.show();
              })
              .insertAfter(container1);
        $('#taskEdit').addClass("disabled");
    });

    $('.addatt').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#contactinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").show();
        $(this).parent().parent().find("#ticketinsert").show();
        $(this).parent().find(".arrow-up").css('left', '6px');
        $('#associationsBreak').addClass('associationsBreakAdjustement');
    }).tooltip();
    $('.addticket').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").show();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#contactinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '37px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();
    $('.adduser').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#contactinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").show();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '70px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();
    $('.addcustomer').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").show();
        $(this).parent().parent().find("#contactinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '101px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();
    $('.addcontact').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#contactinput").show();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '136px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();
    $('.addgroup').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").show();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#contactinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '173px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();
    $('.addproduct').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#contactinput").hide();
        $(this).parent().parent().find("#productinput").show();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '210px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();
    $('.addactivity').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#activityinput").show();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#contactinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '245px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();

    $('#associationsContainer').on('click', '.associationDelete', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you would like to remove this task association?')) {
            window.parent.parent.Ts.System.logAction('Task Detail - Delete Association');
            var blockDiv = $(this).parent();
            if (blockDiv.data('attachmentID')) {
                parent.privateServices.DeleteAttachment(blockDiv.data('attachmentID'), function (e) {
                    blockDiv.hide();
                });
            }
            else {
                window.parent.parent.Ts.Services.Task.DeleteAssociation(_taskID, blockDiv.data('refID'), blockDiv.data('refType'), function (result) {
                    blockDiv.hide();
                });
            }
        }
    });

    $('#subtasksAdd').click(function (e) {
        e.preventDefault();
        parent.Ts.System.logAction('Tasks Detail Page - New Task');
        parent.Ts.MainPage.newTask(_taskID, _Name);

    });

    $('#tblSubtasks').on('click', '.subtaskCheckBox', function (e) {
        e.preventDefault();
        var id = $(this).data('taskID');

        if ($(this).is(':checked')) {
            parent.Ts.System.logAction('Task Detail Page - Complete Subtask');
            _completeCommentTaskID = id;
            $('#modalTaskComment').modal('show');
        } else {
            parent.Ts.System.logAction('Task Detail Page - Uncomplete Subtask');
        }

        window.parent.parent.Ts.Services.Task.SetTaskIsCompleted(id, $(this).is(':checked'), function (result) {
            document.getElementById('st' + id).checked = (result.Value) ? true : false;
            var frame = parent.document.getElementById('iframe-o-' + id);
            if (frame) {
                frame.contentDocument.location.reload(true);
            }
        },
        function (error) {
            header.show();
            alert('There was an error saving the subtask is complete.');
        });
    });


    $('#tblSubtasks').on('click', '.tasklink', function (e) {
        e.preventDefault();

        var id = $(this).data('taskID');
        parent.Ts.System.logAction('Task Detail Page - View Subtask');
        parent.Ts.MainPage.openNewTask(id);

        //parent.Ts.Services.Assets.UpdateRecentlyViewed('o' + id, function (resultHtml) {
        //    $('.recent-container').empty();
        //    $('.recent-container').html(resultHtml);
        //});

    });

    $('.header-nav').on('click', '.parentLink', function (e) {
        e.preventDefault();

        var id = $(this).data('taskID');
        parent.Ts.System.logAction('Task Detail Page - View Parent Task');
        parent.Ts.MainPage.openNewTask(id);
    });

    var execGetCustomer = null;
    function getCustomers(request, response) {
        if (execGetCustomer) { execGetCustomer._executor.abort(); }
        execGetCustomer = window.parent.parent.Ts.Services.Organizations.WCSearchOrganization(request.term, function (result) {
            response(result);
        });
    }

    var execGetContact = null;
    function getContacts(request, response) {
        if (execGetContact) { execGetContact._executor.abort(); }
        execGetContact = parent.parent.Ts.Services.Organizations.GetContacts(request.term, function (result) { response(result); });
    }

    var execGetUsers = null;
    function getUsers(request, response) {
        if (execGetUsers) { execGetUsers._executor.abort(); }
        execGetUsers = window.parent.parent.Ts.Services.Users.SearchUsers(request.term, function (result) { response(result); });
    }

    var execGetTicket = null;
    function getTicketsByTerm(request, response) {
        if (execGetTicket) { execGetTicket._executor.abort(); }
        //execGetTicket = Ts.Services.Tickets.GetTicketsByTerm(request.term, function (result) { response(result); });
        execGetTicket = window.parent.parent.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) {
            $('.main-quick-ticket').removeClass('ui-autocomplete-loading');
            response(result);
        });

    }

    var execGetGroups = null;
    function getGroupsByTerm(request, response) {
        if (execGetGroups) { execGetGroups._executor.abort(); }
        execGetTicket = window.parent.parent.Ts.Services.WaterCooler.GetGroupsByTerm(request.term, function (result) { response(result); });
    }

    var execGetProducts = null;
    function getProductByTerm(request, response) {
        if (execGetProducts) { execGetProducts._executor.abort(); }
        execGetProducts = window.parent.parent.Ts.Services.WaterCooler.GetProductsByTerm(request.term, function (result) { response(result); });
    }

    var execSelectNote = null;
    function selectNote(request, response) {
        if (execSelectNote) {
            execSelectNote._executor.abort();
        }
        execSelectNote = window.parent.parent.Ts.Services.Customers.SearchNotes(request.term, function (result) {
            response(result);
        });
    }

    $('.user-search')
    .focusin(function () { $(this).val('').removeClass('user-search-blur'); })
    .focusout(function () { $(this).val('Search for a user...').addClass('user-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('user-search-blur'); })
    .val('Search for a user...')
    .autocomplete({
        minLength: 3,
        source: getUsers,
        select: function (event, ui) {
            if (ui.item) {
                window.parent.parent.Ts.Services.Task.AddAssociation(_taskID, ui.item.id, window.parent.parent.Ts.ReferenceTypes.Users, function (success) {
                    if (success) {
                        var attdiv = $('#associationsContainer');
                        var blockDiv = $('<div>')
                        .data('refID', ui.item.id)
                        .data('refType', window.parent.parent.Ts.ReferenceTypes.Users)
                        .hover(function (e) {
                            $(this).find('.associationDelete').show();
                        },
	                    function () {
	                        $(this).find('.associationDelete').hide();
	                        $("#preview").remove();
	                    })
                        .appendTo(attdiv);
                        var atticon = $('<span>')
                        .addClass('userIcon')
                        .appendTo(blockDiv);

                        $('<a>')
                        .attr('target', '_blank')
                        .text(ellipseString(ui.item.value, 100))
                        .addClass('attfilename ui-state-default ts-link preview attfilenamefix')
                        .attr('href', '#')
                        .attr('target', '_blank')
                        .attr('onclick', 'window.parent.parent.Ts.MainPage.openNewContact(' + ui.item.id + '); return false;')
                        .appendTo(blockDiv);

                        $('<i>')
                        .addClass('fa fa-trash-o associationDelete')
                        .hide()
                        .appendTo(blockDiv);
                    }
                });
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

    $('.company-search')
    .focusin(function () { $(this).val('').removeClass('company-search-blur'); })
    .focusout(function () { $(this).val('Search for a company...').addClass('company-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('company-search-blur'); })
    .val('Search for a company...')
    .autocomplete({
        minLength: 3,
        source: getCustomers,
        select: function (event, ui) {
            if (ui.item) {
                window.parent.parent.Ts.Services.Task.AddAssociation(_taskID, ui.item.id, window.parent.parent.Ts.ReferenceTypes.Organizations, function (success) {
                    if (success) {
                        var attdiv = $('#associationsContainer');
                        var blockDiv = $('<div>')
                        .data('refID', ui.item.id)
                        .data('refType', window.parent.parent.Ts.ReferenceTypes.Organizations)
                        .hover(function (e) {
                            $(this).find('.associationDelete').show();
                        },
	                    function () {
	                        $(this).find('.associationDelete').hide();
	                        $("#preview").remove();
	                    })
                        .appendTo(attdiv);
                        var atticon = $('<span>')
                        .addClass('companyIcon')
                        .appendTo(blockDiv);

                        $('<a>')
                        .attr('target', '_blank')
                        .text(ellipseString(ui.item.value, 100))
                        .addClass('attfilename ui-state-default ts-link preview attfilenamefix')
                        .attr('href', '#')
                        .attr('target', '_blank')
                        .attr('onclick', 'window.parent.parent.Ts.MainPage.openNewCustomer(' + ui.item.id + '); return false;')
                        .appendTo(blockDiv);

                        $('<i>')
                        .addClass('fa fa-trash-o associationDelete')
                        .hide()
                        .appendTo(blockDiv);
                    }
                });
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

    $('.contact-search')
    .focusin(function () { $(this).val('').removeClass('contact-search-blur'); })
    .focusout(function () { $(this).val('Search for a contact...').addClass('contact-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('contact-search-blur'); })
    .val('Search for a contact...')
    .autocomplete({
        minLength: 3,
        source: getContacts,
        select: function (event, ui) {
            if (ui.item) {
                window.parent.parent.Ts.Services.Task.AddAssociation(_taskID, ui.item.id, window.parent.parent.Ts.ReferenceTypes.Contacts, function (success) {
                    if (success) {
                        var attdiv = $('#associationsContainer');
                        var blockDiv = $('<div>')
                        .data('refID', ui.item.id)
                        .data('refType', window.parent.parent.Ts.ReferenceTypes.Contacts)
                        .hover(function (e) {
                            $(this).find('.associationDelete').show();
                        },
	                    function () {
	                        $(this).find('.associationDelete').hide();
	                        $("#preview").remove();
	                    })
                        .appendTo(attdiv);
                        var atticon = $('<span>')
                        .addClass('contactIcon')
                        .appendTo(blockDiv);

                        $('<a>')
                        .attr('target', '_blank')
                        .text(ellipseString(ui.item.value, 100))
                        .addClass('attfilename ui-state-default ts-link preview attfilenamefix')
                        .attr('href', '#')
                        .attr('target', '_blank')
                        .attr('onclick', 'window.parent.parent.Ts.MainPage.openNewContact(' + ui.item.id + '); return false;')
                        .appendTo(blockDiv);

                        $('<i>')
                        .addClass('fa fa-trash-o associationDelete')
                        .hide()
                        .appendTo(blockDiv);
                    }
                });
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

    $('.main-quick-ticket')
    .focusin(function () { $(this).val('').removeClass('main-quick-ticket-blur'); })
    .focusout(function () { $(this).val('Search for a ticket...').addClass('main-quick-ticket-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('main-quick-ticket-blur'); })
    .val('Search for a ticket...')
    .autocomplete({
        minLength: 2, source: getTicketsByTerm, delay: 300,
        select: function (event, ui) {
            window.parent.parent.Ts.Services.Task.AddAssociation(_taskID, ui.item.data, window.parent.parent.Ts.ReferenceTypes.Tickets, function (success) {
                if (success) {
                    var attdiv = $('#associationsContainer');
                    //if (attachments.length > 0) {
                        //var attdiv = $('<div>')
                        //.addClass('attachment-list')
                        //.appendTo($('#associationsContainer'));
                    //}
                    //for (var i = 0; i < attachments.length; i++) {
                        var blockDiv = $('<div>')
                        .data('refID', ui.item.data)
                        .data('refType', window.parent.parent.Ts.ReferenceTypes.Tickets)
                        .hover(function (e) {
                            $(this).find('.associationDelete').show();
                        },
                        function () {
                            $(this).find('.associationDelete').hide();
                            $("#preview").remove();
                        })
                        .appendTo(attdiv);
                        var atticon = $('<span>')
                        .addClass('ticketIcon')
                        .appendTo(blockDiv);

                        $('<a>')
                        .attr('target', '_blank')
                        //.attr('filetype', attachments[i].FileType)
                        .text(ellipseString(ui.item.value, 100))
                        .addClass('attfilename ui-state-default ts-link preview attfilenamefix')
                        .attr('href', window.parent.parent.Ts.System.AppDomain + '?TicketNumber=' + ui.item.id)
                        .attr('target', '_blank')
                        .attr('onclick', 'window.parent.parent.Ts.MainPage.openTicket(' + ui.item.id + '); return false;')
                        .appendTo(blockDiv);

                        $('<i>')
                        .addClass('fa fa-trash-o associationDelete')
                        .hide()
                        .appendTo(blockDiv);
                    //}
                }
            });
            $('.main-quick-ticket').removeClass('ui-autocomplete-loading');
            return false;
        }
    });

    $('.group-search')
    .focusin(function () { $(this).val('').removeClass('group-search-blur'); })
    .focusout(function () { $(this).val('Search for a group...').addClass('group-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('group-search-blur'); })
    .val('Search for a group...')
    .autocomplete({
        minLength: 2,
        source: getGroupsByTerm,
        select: function (event, ui) {
            if (ui.item) {
                window.parent.parent.Ts.Services.Task.AddAssociation(_taskID, ui.item.id, window.parent.parent.Ts.ReferenceTypes.Groups, function (success) {
                    if (success) {
                        var attdiv = $('#associationsContainer');
                        var blockDiv = $('<div>')
                        .data('refID', ui.item.id)
                        .data('refType', window.parent.parent.Ts.ReferenceTypes.Groups)
                        .hover(function (e) {
                            $(this).find('.associationDelete').show();
                        },
                        function () {
                            $(this).find('.associationDelete').hide();
                            $("#preview").remove();
                        })
                        .appendTo(attdiv);
                        var atticon = $('<span>')
                        .addClass('groupIcon')
                        .appendTo(blockDiv);

                        $('<a>')
                        .attr('target', '_blank')
                        .text(ellipseString(ui.item.value, 100))
                        .addClass('attfilename ui-state-default ts-link preview attfilenamefix')
                        .attr('href', '#')
                        .attr('target', '_blank')
                        .attr('onclick', 'window.parent.parent.Ts.MainPage.openGroup(' + ui.item.id + '); return false;')
                        .appendTo(blockDiv);

                        $('<i>')
                        .addClass('fa fa-trash-o associationDelete')
                        .hide()
                        .appendTo(blockDiv);
                    }
                });
            }
            $(this).val("");
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

    $('.product-search')
    .focusin(function () { $(this).val('').removeClass('product-search-blur'); })
    .focusout(function () { $(this).val('Search for a product...').addClass('product-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('product-search-blur'); })
    .val('Search for a product...')
    .autocomplete({
        minLength: 3,
        source: getProductByTerm,
        select: function (event, ui) {
            if (ui.item) {
                window.parent.parent.Ts.Services.Task.AddAssociation(_taskID, ui.item.id, window.parent.parent.Ts.ReferenceTypes.Products, function (success) {
                    if (success) {
                        var attdiv = $('#associationsContainer');
                        var blockDiv = $('<div>')
                        .data('refID', ui.item.id)
                        .data('refType', window.parent.parent.Ts.ReferenceTypes.Products)
                        .hover(function (e) {
                            $(this).find('.associationDelete').show();
                        },
                        function () {
                            $(this).find('.associationDelete').hide();
                            $("#preview").remove();
                        })
                        .appendTo(attdiv);
                        var atticon = $('<span>')
                        .addClass('productIcon')
                        .appendTo(blockDiv);

                        $('<a>')
                        .attr('target', '_blank')
                        .text(ellipseString(ui.item.value, 100))
                        .addClass('attfilename ui-state-default ts-link preview attfilenamefix')
                        .attr('href', '#')
                        .attr('target', '_blank')
                        .attr('onclick', 'window.parent.parent.Ts.MainPage.openNewProduct(' + ui.item.id + '); return false;')
                        .appendTo(blockDiv);

                        $('<i>')
                        .addClass('fa fa-trash-o associationDelete')
                        .hide()
                        .appendTo(blockDiv);
                    }
                });
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

    $('.activity-search')
    .focusin(function () { $(this).val('').removeClass('activity-search-blur'); })
    .focusout(function () { $(this).val('Search for an activity...').addClass('activity-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('activity-search-blur'); })
    .val('Search for an activity...')
    .autocomplete({
        minLength: 3,
        source: selectNote,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.activity-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Activity')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.activity-queue')).data('Activity', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });



    $('.file-upload').fileupload({
        namespace: 'task_attachment',
        dropZone: $('.file-upload'),
        add: function (e, data) {
            for (var i = 0; i < data.files.length; i++) {

                var bg = $('<div>')
          .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
          .appendTo($(this).parent().parent().find('.upload-queue'));

                data.context = bg;
                bg.data('data', data);

                $('<span>')
          .text(ellipseString(data.files[i].name, 20) + '  (' + window.parent.parent.Ts.Utils.getSizeString(data.files[i].size) + ')')
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
              $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
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

            $('.upload-queue div.ticket-removable-item').each(function (i, o) {
                var data = $(o).data('data');
                data.url = '../../../Upload/Tasks/' + _taskID;
                data.jqXHR = data.submit();
                $(o).data('data', data);
            });
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
            $(this).parent().parent().find('.progress').progressbar().show();
            $(this).parent().parent().find('.upload-queue .ui-icon-close').hide();
            $(this).parent().parent().find('.upload-queue .ui-icon-cancel').show();
        },
        stop: function (e, data) {
            $(this).parent().parent().find('.progress').progressbar('value', 100);
            window.parent.parent.Ts.System.logAction('Task - Attachment Added');
            $('.commentcontainer').hide();
            $('.faketextcontainer').show();
            $('#messagecontents').val('');
            resetDisplay();
            LoadAssociations();
        }
    });

    $('#associationsRefresh').on('click', function (e) {
        e.preventDefault();
        LoadAssociations();
    });

    $('#subtasksAdd').on('click', function () {
        window.parent.parent.Ts.System.logAction('Task - Subtasks Add');
        //Pending implementation
    });

    $('#subtasksRefresh').on('click', function (e) {
        e.preventDefault();
        window.parent.parent.Ts.System.logAction('Task - Subtasks Refresh');
        LoadSubtasks(1);
    });

    $('#historyToggle').on('click', function () {
        window.parent.parent.Ts.System.logAction('Task - History Toggle');
        if (_historyLoaded == 0) {
            _historyLoaded = 1;
            LoadHistory(1);
        }
    });

    $('#historyRefresh').on('click', function (e) {
        e.preventDefault();
        window.parent.parent.Ts.System.logAction('Task - History Refresh');
        LoadHistory(1);
    });

    //$('.taskProperties p, #Name').toggleClass("editable");

    $('#btnTaskCompleteComment').on('click', function (e) {
        e.preventDefault();
        if ($('#taskCompleteComment').val() == ''){
            alert('Please type your comments before clicking on the Yes button.');
        }
        else {
            window.parent.parent.Ts.System.logAction('Task - Add Task Complete Comment');
            window.parent.parent.Ts.Services.Task.AddTaskCompleteComment(_completeCommentTaskID, $('#taskCompleteComment').val(), function (result) {
                if (result.Value) {
                    $('#taskCompleteComment').val('');
                    $('#modalTaskComment').modal('hide');
                    $('#fieldCompletionDate').html(result.DateCompleted == null ? "None" : window.parent.parent.Ts.Utils.getMsDate(result.DateCompleted).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()));
                    $('#fieldCompletionComment').text(!result.CompletionComment ? "None" : result.CompletionComment);
                    $('.completedData').show();
                }
                else {
                    alert('There was an error saving your comment. Please try again.')
                }
            });
        }
    });

    $('#btnNoComment').on('click', function (e) {
        $('.completedData').show();
    })
});

TaskDetailPage = function () { };

TaskDetailPage.prototype = {
    constructor: TaskDetailPage,
    refresh: function () { }
};

// id is the task id, status refers to the 'checked' property.
function taskCheckBox(id, status) {
    document.getElementById('st' + id).checked = (status) ? true : false;
}
