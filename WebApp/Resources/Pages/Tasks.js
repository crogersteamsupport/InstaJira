﻿var _pageSize = 20;
var _allAssignedLoaded = false;
var _allCreatedLoaded = false;
var _taskCount = 0;
var _start = 0;
var _currentTab = 0;

function showLoadingIndicator() {
    $('.results-loading').show();
}

function hideLoadingIndicator() {
    $('.results-loading').hide();
}

function fetchTasks(callback) {

    showLoadingIndicator();

    parent.Ts.Services.Task.LoadPage(_start + 1, _pageSize, _currentTab, function (pageData) {

        $('.searchresults').fadeTo(0, 1);

        switch (_currentTab) {
            case 0:
                LoadMyTasks(pageData.AssignedItems);
                if (callback) callback(pageData.AssignedItems);
                break;
            case 1:
                LoadCreated(pageData.CreatedItems);
                if (callback) callback(pageData.CreatedItems);
                break;
            case 2:
                LoadCompleted(pageData.CompletedItems);
                if (callback) callback(pageData.CompletedItems);
                break;
            default:
                LoadMyTasks(pageData.AssignedItems);
                if (callback) callback(pageData.AssignedItems);
        }

        $('.results-loading').hide();

    });
}

function LoadMyTasks(tasks) {
    var container = $('.assignedresults');
    if (tasks.length < _pageSize) {
        _allAssignedLoaded = true;
    }
    if (_start == 0) {
        insertSearchResults(container, tasks);
        if (tasks.length == 0) {
            $('.results-empty').show();
            $('.results').hide();
        }
    }
    else {
        appendSearchResults(container, tasks);
    }
}

function LoadCreated(tasks) {
    var container = $('.createdresults');
    if (tasks.length < _pageSize) {
        _allCreatedLoaded = true;
    }
    if (_start == 0) {
        insertSearchResults(container, tasks);
        if (tasks.length == 0) {
            $('.results-empty').show();
            $('.results').hide();
        }
    }
    else {
        appendSearchResults(container, tasks);
    }
}

function LoadCompleted(tasks) {
    var container = $('.completedresults');
    if (tasks.length < _pageSize) {
        _allCreatedLoaded = true;
    }
    if (_start == 0) {
        insertSearchResults(container, tasks);
        if (tasks.length == 0) {
            $$('.results-empty').show();
            $('.results').hide();
        }
    }
    else {
        appendSearchResults(container, tasks);
    }
}

function insertSearchResults(container, tasks) {
    container.empty();

    $('.results-loading').hide();
    $('.results-empty').hide();

    if (tasks.length < 1) {
        $('.results').hide();
        $('.results-empty').show();
    } else {
        var source;

        switch (_currentTab) {
            case 0:
                source = $("#mytask-table-template").html();
                break;
            case 1:
                source = $("#assigned-table-template").html();
                break;
            case 2:
                source = $("#closed-table-template").html();
                break;
            default:
                source = $("#mytask-table-template").html();
        }

        //var source = $("#task-template").html();
        var template = Handlebars.compile(source);
        data = { taskList: tasks };
        console.log(data);

        $("#handlebarsTaskList").html(template(data));

        appendSearchResults(null, tasks)

        $('.results').show();
    }
    _isLoading = false;
}

function appendSearchResults(container, tasks) {

    data = { taskList: tasks };

    var template;
    var destination;

    switch (_currentTab) {
        case 0:
            template = $("#mytasks-template").html();
            destination = $("#my-tasks");
            break;
        case 1:
            template = $("#assigned-tasks-template").html();
            destination = $("#assigned-tasks");
            break;
        case 2:
            template = $("#closed-tasks-template").html();
            destination = $("#closed-tasks");
            break;
        default:
            template = $("#pending-tasks-template").html();
            destination = $("#pending-tasks");
    }

    var compiledTemplate = Handlebars.compile(template);
    destination.append(compiledTemplate(data));
}

function isNullOrWhiteSpace(str) {
    return str === null || String(str).match(/^ *$/) !== null;
}

function LoadUsers() {
    parent.Ts.Services.Customers.LoadUsers(function (users) {
        for (var i = 0; i < users.length; i++) {
            $('<option>').attr('value', users[i].UserID).text(users[i].FirstName + ' ' + users[i].LastName).data('o', users[i]).appendTo('#ddlUser');
        }
    });
}

function onShow() {
    $('#handlebarsTaskList').empty();
    _start = 0;
    fetchTasks(function () {

    });
}

$(document).ready(function () {
    $('#tasks-Refresh').click(function (e) {
        _start = 0;
        fetchTasks(function () {

        });
    });

    $('.action-new').click(function (e) {
        e.preventDefault();
        parent.Ts.System.logAction('Tasks Page - New Task');
        parent.Ts.MainPage.newTask();

    });

    $('#TaskList').on('click', 'a.tasklink', function (e) {
        e.preventDefault();
        var id = $(this).data('reminderid');
        parent.Ts.System.logAction('Tasks Page - View Task');
        parent.Ts.MainPage.openNewTask(id);

    });

    $('.load-more-tasks').on('click', function (e) {
        e.preventDefault();
        fetchTasks(function (tasks) {
            _start += tasks.length;

            if (tasks.length < _pageSize) {
                $('.more-tasks').hide();
            }
        });
    })

    $('.tab-assigned-tasks').on('click', function (e) {
        e.preventDefault();
        $('.tabs button').removeClass('active');
        $(this).addClass('active');
        parent.Ts.System.logAction('Tasks Page - Change Filter');
        _allAssignedLoaded = false;
        _currentTab = 0;
        _assignedTab = 0;
        _createdTab = 0;
        _start = 0;

        fetchTasks(function (tasks) {
            _start += tasks.length;

            if (tasks.length == _pageSize) {
                $('.more-tasks').show();
            }
        });
    });

    $('.tab-created-tasks').on('click', function (e) {
        e.preventDefault();
        $('.tabs button').removeClass('active');
        $(this).addClass('active');
        parent.Ts.System.logAction('Tasks Page - Change Filter');
        _currentTab = 1;
        _allCreatedLoaded = false;
        _createdTab = 1;
        _assignedTab = 0;
        _start = 0;

        fetchTasks(function (tasks) {
            _start += tasks.length;

            if (tasks.length == _pageSize) {
                $('.more-tasks').show();
            }
        });
    });

    $('.tab-closed-tasks').on('click', function (e) {
        e.preventDefault();
        $('.tabs button').removeClass('active');
        $(this).addClass('active');

        parent.Ts.System.logAction('Tasks Page - Change Filter');
        _assignedTab = 0;
        _currentTab = 2
        _allCreatedLoaded = false;
        _createdTab = 1
        _assignedTab = 0;
        _start = 0;

        fetchTasks(function (tasks) {
            _start += tasks.length;

            if (tasks.length == _pageSize) {
                $('.more-tasks').show();
            }
        });
    });

    $('#TaskList').on('click', '.change-task-status', function (e) {
        var id = $(this).data('reminderid');
        var checked = $(this).prop("checked");
        parent.Ts.System.logAction('Tasks Page - Change Task Status');

        parent.Ts.Services.Task.SetTaskIsCompleted(id, checked);

        $(this).parent().parent().fadeOut(600, function () { $(this).remove() });
    });

    fetchTasks(function (tasks) {
        _start += tasks.length;

        if (tasks.length == _pageSize) {
            $('.more-tasks').show();
        }
    });

    Handlebars.registerHelper("formatDate", function (datetime) {
        if (datetime != null) {
            return parent.Ts.Utils.getMsDate(datetime).localeFormat(parent.Ts.Utils.getDatePattern());
        }
        else return null;
    });

    Handlebars.registerHelper("formatTaskName", function (Task) {
        var name = Task.TaskName;

        if (Task.TaskName == null) {
            if (Task.Description == null || Task.Description == "") {
                name = 'No Title';
            }
            else {
                name = Task.Description;
            }
        }

        return name;
    });

    Handlebars.registerHelper("formatRow", function (task) {
        var cssClasses = null;

        if (task.TaskDueDate != null) {
            if (task.TaskIsComplete != true && new Date() > new Date(task.TaskDueDate)) {
                cssClasses = 'danger';
            }
            else {
                return null;
            }
        }

        return cssClasses;
    });

    Handlebars.registerHelper("taskComplete", function (taskdate) {
        return taskdate != null ? ' checked="checked"' : '';
    });

    Handlebars.registerHelper("mapAssociation", function (association) {
        var result = '';
        var functionName = '';
        var associationName = '';
        var iconClass = '';

        switch (association.RefType) {
            //case 3: leaving attachments off for now
            //    associationName = association.Attachment;
            //    iconClass = attIcon;
            //    refcode = '<i class="fa fa-paperclip" title="' + association.Attachment + '"></i>'
            //    break;
            case 6:
                associationName = association.Group;
                iconClass = "groupIcon";
                functionName = 'window.parent.parent.Ts.MainPage.openGroup(' + association.RefID + '); return false;';
                break;
            case 9:
                associationName = association.Company;
                iconClass = "companyIcon";
                functionName = 'window.parent.parent.Ts.MainPage.openNewCustomer(' + association.RefID + '); return false;';
                break;
            case 13:
                associationName = association.Product;
                iconClass = "productIcon";
                functionName = 'window.parent.parent.Ts.MainPage.openNewProduct(' + association.RefID + '); return false;';
                break;
            case 17:
                associationName = association.TicketName;
                iconClass = "ticketIcon";
                functionName = 'window.parent.parent.Ts.MainPage.openTicketByID(' + association.RefID + '); return false;'
                break;
            case 22:
                associationName = association.User;
                iconClass = "userIcon";
                functionName = 'window.parent.parent.Ts.MainPage.openNewContact(' + association.RefID + '); return false;'
                break;
            default:
                functionName = null;
        }

        if (functionName != null) {
            result = '<span><a target="_blank" class="ui-state-default ts-link ' + iconClass + '" href="#" onclick="' + functionName + '" title="' + associationName + '"></a></span>'
        }

        return new Handlebars.SafeString(result);
    });
});

