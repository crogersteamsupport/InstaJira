var _pageSize = 20;
var _allAssignedLoaded = false;
var _allCreatedLoaded = false;
var _taskCount = 0;
var _start = 0;
var _currentTab = 0;
var _completeCommentTaskID = 0;

function showLoadingIndicator() {
    $('.results-loading').show();
}

function hideLoadingIndicator() {
    $('.results-loading').hide();
}

function fetchTasks(callback) {

    showLoadingIndicator();

    parent.Ts.Services.Task.LoadPage(_start, _pageSize, _currentTab, function (pageData) {
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
            $('.results-empty').show();
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

        var template = Handlebars.compile(source);
        data = { taskList: tasks };

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
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.async = true;
    script.src = ('https:' === document.location.protocol ? 'https://' : 'http://') + 'www.dropbox.com/static/api/1/dropbox.js';
    var firstScript = document.getElementsByTagName('script')[0];
    script.setAttribute('data-app-key', 'ebdoql1dhyy7l72');
    script.setAttribute('id', 'dropboxjs');
    //if (window.parent.Ts.System.User.OrganizationID != 1150007)
    //    firstScript.parentNode.insertBefore(script, firstScript);
    //slaCheckTimer = setInterval(RefreshSlaDisplay, 5000);


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
        var id = $(this).data('taskid');
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
        var id = $(this).data('taskid');
        var checkbox = $(this);
        var checked = $(this).prop("checked");
        parent.Ts.System.logAction('Tasks Page - Change Task Status');

        parent.Ts.Services.Task.SetTaskIsCompleted(id, checked, function (data) {
            if (!data.IncompleteSubtasks) {
                if (data.Value) {
                    $('#modalTaskComment').modal('show');
                }
                checkbox.parent().parent().fadeOut(600, function () {
                    _completeCommentTaskID = id;
                    checkbox.remove()
                });
            }
            else {
                checkbox.prop("checked", false);
                alert('There are subtasks pending completion, please finish them before completing the parent task.')
            }
        });
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

    Handlebars.registerHelper("formatRow", function (task) {
        var cssClasses = null;

        if (task.DueDate != null) {
            if (task.IsComplete != true && new Date() > new Date(task.DueDate)) {
                cssClasses = 'over-due';
            }
            else {
                return null;
            }
        }

        return cssClasses;
    });

    Handlebars.registerHelper("taskComplete", function (isComplete) {
        return isComplete == true ? ' checked="checked"' : '';
    });

    Handlebars.registerHelper("mapAssociation", function (association) {
        var result = '';
        var functionName = '';
        var associationName = '';
        var iconClass = '';

        switch (association.RefType) {
            case 6:
                associationName = association.Group;
                iconClass = "fa-users";
                functionName = 'window.parent.parent.Ts.MainPage.openGroup(' + association.RefID + '); return false;';
                break;
            case 9:
                associationName = association.Company;
                iconClass = "fa-building";
                functionName = 'window.parent.parent.Ts.MainPage.openNewCustomer(' + association.RefID + '); return false;';
                break;
            case 13:
                associationName = association.Product;
                iconClass = "fa-truck";
                functionName = 'window.parent.parent.Ts.MainPage.openNewProduct(' + association.RefID + '); return false;';
                break;
            case 17:
                associationName = association.TicketName;
                iconClass = "fa-ticket";
                functionName = 'window.parent.parent.Ts.MainPage.openTicketByID(' + association.RefID + '); return false;'
                break;
            case 22:
                associationName = association.User;
                iconClass = "fa-user-circle-o";
                functionName = 'window.parent.parent.Ts.MainPage.openUser(' + association.RefID + '); return false;'
                break;
            case 32:
                associationName = association.Contact;
                iconClass = "fa-user";
                functionName = 'window.parent.parent.Ts.MainPage.openNewContact(' + association.RefID + '); return false;'
                break;
            case 63:
                associationName = association.Activity;
                iconClass = "fa-sticky-note";
                functionName = 'window.parent.parent.Ts.MainPage.openNewCustomerNote(' + association.ActivityRefID + ',' + association.ActivityID + '); return false;'
                break;
            case 64:
                associationName = association.Activity;
                iconClass = "fa-sticky-note";
                functionName = 'window.parent.parent.Ts.MainPage.openNewContactNote(' + association.ActivityRefID + ',' + association.ActivityID + '); return false;'
                break;
            default:
                functionName = null;
        }

        if (functionName != null) {
            result = '<span class="associations"><i target="_blank" class="ui-state-default ts-link fa ' + iconClass + '" href="#" onclick="' + functionName + '" title="' + associationName + '"></i></span>'
        }

        return new Handlebars.SafeString(result);
    });

    $('#btnTaskCompleteComment').on('click', function (e) {
        e.preventDefault();
        if ($('#taskCompleteComment').val() == '') {
            alert('Please type your comments before clicking on the Yes button.');
        }
        else {
            window.parent.parent.Ts.System.logAction('Task - Add Task Complete Comment');
            window.parent.parent.Ts.Services.Task.AddTaskCompleteComment(_completeCommentTaskID, $('#taskCompleteComment').val(), function (result) {
                if (result.Value) {
                    $('#taskCompleteComment').val('');
                    $('#modalTaskComment').modal('hide');
                }
                else {
                    alert('There was an error saving your comment. Please try again.')
                }
            });
        }
    });
});


