var userID;
var ratingFilter = '';
var _completeCommentTaskID = 0;


$(document).ready(function () {
    $('body').layout({
        applyDemoStyles: true
    });

    /*initialize */
    userID = parent.Ts.System.User.UserID;
    var activeID;

    var organizationID = parent.Ts.System.User.OrganizationID;
    //if (organizationID != 1078 && organizationID != 13679 && organizationID != 1088) {
    //    $('#ratingsTab').hide();
    //}

    if (parent.Ts.System.User.FilterInactive) {
        $('#cbActive').prop('checked', true);
    }

    if (!parent.Ts.System.User.IsSystemAdmin) {
        $('#historyTab').hide();
    }

    if (parent.Ts.System.Organization.ProductType == parent.Ts.ProductType.Enterprise) {
        $('#taskTab').show();
    }

    if (parent.Ts.Utils.getQueryValue("UserID", window) != null) {
        userID = parent.Ts.Utils.getQueryValue("UserID", window);
        parent.Ts.Services.Users.GetUser(userID, function (user) {
            $('#userName').text(user.FirstName + " " + user.LastName);
        });

    }
    else {
        $('#userName').text(parent.Ts.System.User.FirstName + " " + parent.Ts.System.User.LastName);
    }

    $('#infoIframe').attr("src", "/vcr/1_9_0/Pages/User.html?UserID=" + userID);
    $('#userTabs a:first').tab('show');
    Search();


    $('#infoIframe, #openIframe, #closedIframe, #allIframe, #queueIframe, #historyIframe, #userName').load(function () {
        $('.maincontainer').fadeTo(0, 1);
    });

    if (!parent.Ts.System.User.IsSystemAdmin) {
        $('#userNew').hide();
        $('#userDelete').hide();
    }

    /* functions */

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        if (e.target.innerHTML == "User Information")
            $('#infoIframe').attr("src", "/vcr/1_9_0/Pages/User.html?UserID=" + userID);
        else if (e.target.innerHTML == "Open")
            $('#openIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=false&tf_UserID=" + userID);
        else if (e.target.innerHTML == "Closed")
            $('#closedIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=true&tf_UserID=" + userID);
        else if (e.target.innerHTML == "All Tickets")
            $('#allIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_UserID=" + userID);
        else if (e.target.innerHTML == "Ticket Queue")
            $('#queueIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsEnqueued=true&tf_ViewerID=" + userID);
        else if (e.target.innerHTML == "History")
            $('#historyIframe').attr("src", "../../../Frames/History.aspx?RefType=22&RefID=" + userID);
        else if (e.target.innerHTML == "Ratings")
            LoadRatings('', 1);
        else if (e.target.innerHTML == "Tasks")
            LoadTasks();
    });

    var _tmrSearch = null;
    $('#searchString').keyup(function () {
        if (_tmrSearch != null) {
            clearTimeout(_tmrSearch);
        }
        $('.user-container').fadeTo(200, 0.5);

        _tmrSearch = setTimeout(Search, 500);

    });

    $('.user-container').on('click', '.ts-icon-online-small', function (e) {
        if (parent.Ts.System.User.IsSystemAdmin) {
            var item = $(this);
            parent.Ts.Services.Users.UpdateSpecificUserStatus($(this).attr('userid'), false, function (result) {
                item.removeClass('ts-icon-online-small').addClass('fa fa-circle');
                item.addClass('ts-icon-offline-small').addClass('fa fa-circle');
            });
        }
    });

    $('.user-container').on('click', '.ts-icon-offline-small', function (e) {
        if (parent.Ts.System.User.IsSystemAdmin) {
            var item = $(this);
            parent.Ts.Services.Users.UpdateSpecificUserStatus($(this).attr('userid'), true, function (result) {
                item.removeClass('ts-icon-offline-small').addClass('fa fa-circle');
                item.addClass('ts-icon-online-small').addClass('fa fa-circle');
            });
        }
    });


    //function Search()
    //{
    //    parent.Ts.Services.Users.GetUsersSearch(parent.Ts.System.User.OrganizationID, $('#searchString').val(), function (html) {
    //        $('.user-container').empty();
    //        $('.user-container').fadeTo(0, 1);
    //        $('.user-container').append(html);
    //        $('.user-tooltip').tooltip({ placement: 'bottom', container: 'body' });

    //    });
    //}

    function SearchDelete() {
        parent.Ts.Services.Users.GetUsersSearch(parent.Ts.System.User.OrganizationID, $('#searchString').val(), function (html) {
            $('.user-container').empty();
            $('.user-container').fadeTo(0, 1);
            $('.user-container').append(html);
            $('.user-tooltip').tooltip({ placement: 'bottom', container: 'body' });

            userID = parent.Ts.System.User.UserID;
            $('#infoIframe').attr("src", "/vcr/1_9_0/Pages/User.html?UserID=" + userID);
            $('#openIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=false&tf_UserID=" + userID);
            $('#closedIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=true&tf_UserID=" + userID);
            $('#allIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_UserID=" + userID);
            $('#queueIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsEnqueued=true&tf_ViewerID=" + userID);
            $('#historyIframe').attr("src", "../../../Frames/History.aspx?RefType=22&RefID=" + userID);
            $('#userName').text(parent.Ts.System.User.FirstName + " " + parent.Ts.System.User.LastName);
        });
    }

    $('.user-container').on('click', '.user', function (e) {
        e.preventDefault();
        $('.maincontainer').fadeTo(200, 0.5);
        userID = $(this).attr('uid');
        $('#infoIframe').attr("src", "/vcr/1_9_0/Pages/User.html?UserID=" + userID);
        $('#openIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=false&tf_UserID=" + userID);
        $('#closedIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=true&tf_UserID=" + userID);
        $('#allIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_UserID=" + userID);
        $('#queueIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsEnqueued=true&tf_ViewerID=" + userID);
        $('#historyIframe').attr("src", "../../../Frames/History.aspx?RefType=22&RefID=" + userID);
        $('#userName').text($(this).text());
        LoadRatings('', 1);
        activeID = userID;
        LoadTasks();
    });

    $('.user-container').on('click', '.ts-icon-online-small', function (e) {
        if (parent.Ts.System.User.IsSystemAdmin) {
            var item = $(this);
            parent.Ts.Services.Users.UpdateSpecificUserStatus($(this).attr('userid'), false, function (result) {
                item.removeClass('ts-icon-online-small').addClass('fa fa-circle');
                item.addClass('ts-icon-offline-small').addClass('fa fa-circle');
            });
        }
    });

    $('.user-container').on('click', '.ts-icon-offline-small', function (e) {
        if (parent.Ts.System.User.IsSystemAdmin) {
            var item = $(this);
            parent.Ts.Services.Users.UpdateSpecificUserStatus($(this).attr('userid'), true, function (result) {
                item.removeClass('ts-icon-offline-small').addClass('fa fa-circle');
                item.addClass('ts-icon-online-small').addClass('fa fa-circle');
            });
        }
    });

    $('.user-container').on('click', '.user-chat', function (e) {
        e.preventDefault();
        window.parent.openChat($(this).parent().parent().parent().find('strong').text(), $(this).attr('cid'));

    });

    $('#userNew').click(function () {
        parent.Ts.Services.Users.GetNewUserMessage(function (msg) {
            if (msg != "")
                alert(msg);
            else
                ShowDialog(parent.GetUserDialog(parent.Ts.System.User.OrganizationID));
        });
    });


    $('#userDelete').click(function () {
        if (confirm("Are you sure you would like to PERMANENTLY delete this user?")) {
            parent.privateServices.DeleteUser(activeID, SearchDelete);
            parent.Ts.System.logAction('Users - User Deleted');
        }
    });

    function DialogClosed(sender, args) {
        sender.remove_close(DialogClosed);
        window.location = window.location;
    }

    function ShowDialog(wnd) {
        wnd.add_close(DialogClosed);
        wnd.show();
    }

    $(window).bind('resize', function () {
        if ($("ul#userTabs li.active").text() == "User Information") {
            $('#infoIframe').attr("src", "/vcr/1_9_0/Pages/User.html?UserID=" + userID);
            autoResize();
        }


        ;
    });

    if (parent.Ts.System.Organization.UseProductFamilies) {
        LoadProductFamilies();
        $('.productLineRow').show();
    }

    LoadRatings('', 1);

    $('#tblRatings').delegate('.delete-link', 'click', function (e) {
        var id = $(this).attr('id');
        parent.Ts.Services.Customers.DeleteAgentRating(id, function () {
            LoadRatings('', 1);
        });
    });

    function LoadProductFamilies() {
        parent.Ts.Services.Organizations.LoadOrgProductFamilies(parent.Ts.System.Organization.OrganizationID, function (productFamilies) {
            for (var i = 0; i < productFamilies.length; i++) {
                $('<option>').attr('value', productFamilies[i].ProductFamilyID).text(productFamilies[i].Name).data('o', productFamilies[i]).appendTo('#ddlRatingProductFamily');
            }
        });
    }

    function LoadRatings(ratingOption, start) {
        if (start == 1)
            $('#tblRatings tbody').empty();
        parent.Ts.Services.Customers.LoadAgentRatingsUser2(userID, ratingOption, $('#tblRatings tbody > tr').length + 1, $('#ddlRatingProductFamily').val(), function (ratings) {
            var agents = "";
            var deleteIt = "";
            for (var i = 0; i < ratings.length; i++) {
                for (var j = 0; j < ratings[i].users.length; j++) {
                    if (j != 0)
                        agents = agents + ", ";

                    agents = agents + '<a href="#" target="_blank" onclick="parent.Ts.MainPage.openUser(' + ratings[i].users[j].UserID + '); return false;">' + ratings[i].users[j].FirstName + ' ' + ratings[i].users[j].LastName + '</a>';
                }

                if (parent.Ts.System.User.IsSystemAdmin) {
                    deleteIt = '<a href="#" class="delete-link" id="' + ratings[i].rating.AgentRatingID + '"><span class="fa fa-trash-o"></span></a>';
                }

                var tr = $('<tr>')
                .html('<td><a href="' + parent.Ts.System.AppDomain + '?TicketNumber=' + ratings[i].rating.TicketNumber + '" target="_blank" onclick="parent.Ts.MainPage.openTicket(' + ratings[i].rating.TicketNumber + '); return false;">' + ratings[i].rating.TicketNumber + '</a></td><td>' + agents + '</td><td><a href="#" onclick="parent.Ts.MainPage.openNewContact(' + ratings[i].reporter.UserID + '); return false;">' + ratings[i].reporter.FirstName + ' ' + ratings[i].reporter.LastName + '</a></td><td><a href="#" onclick="parent.Ts.MainPage.openNewCustomer(' + ratings[i].org.OrganizationID + '); return false;">' + ratings[i].org.Name + '</a></td><td>' + ratings[i].rating.DateCreated.toDateString() + '</td><td>' + ratings[i].rating.RatingText + '</td><td>' + (ratings[i].rating.Comment === null ? "None" : ratings[i].rating.Comment) + '</td><td>' + deleteIt + '</td>')
                .appendTo('#tblRatings > tbody:last');
                agents = "";
            }
        });

        parent.Ts.Services.Organizations.GetAgentRatingOptions(parent.Ts.System.Organization.OrganizationID, function (o) {
            if (o != null) {
                if (o.PositiveImage)
                    $('#positiveImage').attr('src', o.PositiveImage);
                if (o.NeutralImage)
                    $('#neutralImage').attr('src', o.NeutralImage);
                if (o.NegativeImage)
                    $('#negativeImage').attr('src', o.NegativeImage);
            }
        });

        parent.Ts.Services.Customers.LoadRatingPercentsUser2(userID, $('#ddlRatingProductFamily').val(), function (results) {
            $('#negativePercent').text(results[0] + "%");
            $('#neutralPercent').text(results[1] + "%");
            $('#positivePercent').text(results[2] + "%");
        });

    }

    function LoadTasks() {
        parent.Ts.Services.Task.GetUserTasks(0, 20, userID, function (tasks) {
            var data = { taskList: tasks };
            var source = $("#user-tasks-template").html();
            var template = Handlebars.compile(source);
            $("#tasks").html(template(data));
        });
    }

    $('#positiveImage').click(function () {
        LoadRatings(1, 1);
        ratingFilter = 1;
    });
    $('#neutralImage').click(function () {
        LoadRatings(0, 1);
        ratingFilter = 0;
    });
    $('#negativeImage').click(function () {
        LoadRatings(-1, 1);
        ratingFilter = -1;
    });
    $('#viewAll').click(function () {
        LoadRatings('', 1);
        ratingFilter = '';
    });

    $('.tab-content').bind('scroll', function () {
        if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
            LoadRatings(ratingFilter, $('#tblRatings tbody > tr').length + 1);
        }
    });

    $('#ddlRatingProductFamily').change(function () {
        LoadRatings(ratingFilter, 1);
    });

    $('#cbActive').click(function (e) {
        parent.Ts.Services.Users.SetInactiveFilter(parent.Ts.System.User.UserID, $('#cbActive').prop('checked'), function (result) {
            parent.Ts.System.logAction('User Info - Changed Filter Inactive Setting');
        },
              function (error) {
                  alert('There was an error saving the user filter inaactive setting.');
              });
        Search();
    });

    $('#taskContainer').on('click', 'a.tasklink', function (e) {
        e.preventDefault();
        var id = $(this).data('taskid');
        parent.Ts.System.logAction('Tasks Page - View Task');
        parent.Ts.MainPage.openNewTask(id);
    });

    $('#taskContainer').on('click', '.change-task-status', function (e) {

        var id = $(this).data('taskid');
        var checkbox = $(this);
        var checked = $(this).prop("checked");
        parent.Ts.System.logAction('Contact Page - Change Task Status');

        parent.Ts.Services.Task.SetTaskIsCompleted(id, checked, function (data) {
            if (data.IncompleteSubtasks) {
                checkbox.prop("checked", false);
                alert('There are subtasks pending completion, please finish them before completing the parent task.')
            }
            else if (checked) {
                _completeCommentTaskID = id;
                $('#modalTaskComment').modal('show');
            }
        });
    });

    $('.task-action-add').click(function (e) {
        e.preventDefault();
        parent.Ts.System.logAction('Contact Page - New Task');
        parent.Ts.MainPage.newTaskFromSource(22, userID, $('#userName').text(), '');
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

function Search() {
    parent.Ts.Services.Users.GetUsersSearch(parent.Ts.System.User.OrganizationID, $('#searchString').val(), !$('#cbActive').prop('checked'), function (html) {
        $('.user-container').empty();
        $('.user-container').fadeTo(0, 1);
        $('.user-container').append(html);
        $('.user-tooltip').tooltip({ placement: 'bottom', container: 'body' });

    });
}

function Update() {
    Search();
}

function autoResize() {
    $('#infoIframe').attr('height', $('.maincontainer').outerHeight() - $('.main-nav').outerHeight());
}

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
        default:
            functionName = null;
    }

    if (functionName != null) {
        result = '<span class="associations"><i target="_blank" class="ts-link fa ' + iconClass + '" href="#" onclick="' + functionName + '" title="' + associationName + '"></i></span>'
    }

    return new Handlebars.SafeString(result);
});
