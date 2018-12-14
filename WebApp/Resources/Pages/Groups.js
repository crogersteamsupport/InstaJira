var groupID;
$(document).ready(function () {
    $('body').layout({
        applyDemoStyles: true
    });

    function getMainFrame() {
        var result = window.parent;
        var cnt = 0;
        while (!(result.Ts && result.Ts.Services)) {
            result = result.parent;
            cnt++;
            if (cnt > 5) return null;
        }
        return result;
    }

    var _mainFrame = getMainFrame();
    
    if (!_mainFrame.Ts.System.User.IsSystemAdmin) {
        $('#groupDelete').remove();
        //$('#openTab').hide();
        //$('#closedTab').hide();
    }

    if (_mainFrame.Ts.Utils.getQueryValue("groupID", window) != null) {    
        groupID = _mainFrame.Ts.Utils.getQueryValue("groupID", window);
        _mainFrame.Ts.Services.Organizations.GetGroupInfo(groupID, function (group) {
            groupID = group.GroupID;
            $('#groupName').text(group.Name);
            $('#infoIframe').attr("src", "../../../Frames/GroupInformation.aspx?GroupID=" + groupID);
        });


    }
    else {
        _mainFrame.Ts.Services.Organizations.GetGroupInfo(null, function (group) {
            groupID = group.GroupID;
            $('#groupName').text(group.Name);
            $('#infoIframe').attr("src", "../../../Frames/GroupInformation.aspx?GroupID=" + groupID);
        });

    }


    

    _mainFrame.Ts.Services.Organizations.GetGroups(function (groupsList) {
        $('.group-container').empty();
        if (groupsList.HasProductFamilies)
        {
            $('.group-container').addClass('productFamily-container');
        }
        $('.group-container').append(groupsList.Html);
    });
    
    $('#infoIframe, #openIframe, #closedIframe, #allIframe, #queueIframe, #historyIframe, #watercoolerIframe, #calendarIframe').load(function () {
        $('.maincontainer').fadeTo(0, 1);
    });

    $('.group-container').on('click', '.group', function (e) {
        e.preventDefault();
        $('.maincontainer').fadeTo(200, 0.5);
        groupID = $(this).attr('gid');
        $('#infoIframe').attr("src", "../../../Frames/GroupInformation.aspx?GroupID=" + groupID);
        $('#openIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=false&tf_GroupID=" + groupID);
        $('#closedIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=true&tf_GroupID=" + groupID);
        $('#unassignedIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=false&tf_UserID=-2&tf_GroupID=" + groupID);
        $('#allIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_GroupID=" + groupID);
        $('#historyIframe').attr("src", "../../../Frames/History.aspx?RefType=6&RefID=" + groupID);
        $('#calendarIframe').attr("src", "/vcr/1_9_0/Pages/Calendar.html?pagetype=4&pageid=" + groupID);
        $('#groupName').text($(this).text());
        activeID = groupID;

    });

    $('#groupTabs a:first').tab('show');

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        if (e.target.innerHTML == "Group Information") {
            $('#infoIframe').attr("src", "../../../Frames/GroupInformation.aspx?GroupID=" + groupID);
            //autoResize();
        }
        else if (e.target.innerHTML == "Open")
            $('#openIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=false&tf_GroupID=" + groupID);
        else if (e.target.innerHTML == "Closed")
            $('#closedIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=true&tf_GroupID=" + groupID);
        else if (e.target.innerHTML == "Unassigned")
            $('#unassignedIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=false&tf_UserID=-2&tf_GroupID=" + groupID);
        else if (e.target.innerHTML == "All Tickets")
            $('#allIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_GroupID=" + groupID);
        else if (e.target.innerHTML == "History")
            $('#historyIframe').attr("src", "../../../Frames/History.aspx?RefType=6&RefID=" + groupID);
        else if (e.target.innerHTML == "WaterCooler")
            $('#watercoolerIframe').attr("src", "/vcr/1_9_0/Pages/Watercooler.html?pagetype=4&pageid=" + groupID);
        else if (e.target.innerHTML == "Calendar")
            $('#calendarIframe').attr("src", "/vcr/1_9_0/Pages/Calendar.html?pagetype=4&pageid=" + groupID);
    });

    $('#groupNew').click(function () {
        ShowDialog(_mainFrame.GetGroupDialog());
        _mainFrame.Ts.System.logAction('Groups - New Group Dialog Opened');
    });

    $('#groupDelete').click(function () {
        if (confirm("Are you sure you would like to PERMANENTLY delete this group?")) {
            _mainFrame.Ts.System.logAction('Groups - Group Deleted');
            _mainFrame.privateServices.DeleteGroup(groupID, function () {
                window.location = window.location;
            });
            
            
        }

    });

    $('#groupEdit').click(function () {
        ShowDialog(_mainFrame.GetGroupDialog(groupID));
        _mainFrame.Ts.System.logAction('Groups - Edit Group Dialog Opened');
    });

    function DialogClosed(sender, args) {
        sender.remove_close(DialogClosed);
        window.location = window.location;
    }

    function ShowDialog(wnd) {
        wnd.add_close(DialogClosed);
        wnd.show();
    }

});

function autoResize() {
    $('#infoIframe').attr('height', $('.maincontainer').outerHeight() - $('.main-nav').outerHeight());
}