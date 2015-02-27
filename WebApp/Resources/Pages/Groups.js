﻿var groupID;
$(document).ready(function () {
    $('body').layout({
        applyDemoStyles: true
    });
    
    if (top.Ts.System.User.OrganizationID != 1078 && top.Ts.System.User.OrganizationID != 13679 && top.Ts.System.User.OrganizationID != 1088) {
        $('#calendarTab').hide();
    }

    if (!top.Ts.System.User.IsSystemAdmin) {
        $('#groupDelete').remove();
        //$('#openTab').hide();
        //$('#closedTab').hide();
    }

    if (top.Ts.Utils.getQueryValue("groupID", window) != null) {    
        groupID = top.Ts.Utils.getQueryValue("groupID", window);
        top.Ts.Services.Organizations.GetGroupInfo(groupID, function (group) {
            groupID = group.GroupID;
            $('#groupName').text(group.Name);
            $('#infoIframe').attr("src", "../../../Frames/GroupInformation.aspx?GroupID=" + groupID);
        });


    }
    else {
        top.Ts.Services.Organizations.GetGroupInfo(null, function (group) {
            groupID = group.GroupID;
            $('#groupName').text(group.Name);
            $('#infoIframe').attr("src", "../../../Frames/GroupInformation.aspx?GroupID=" + groupID);
        });

    }


    

    top.Ts.Services.Organizations.GetGroups(function (html) {
        $('.group-container').empty();
        $('.group-container').append(html);
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
        ShowDialog(top.GetGroupDialog());
        top.Ts.System.logAction('Groups - New Group Dialog Opened');
    });

    $('#groupDelete').click(function () {
        if (confirm("Are you sure you would like to PERMANENTLEY delete this group?")) {
            top.Ts.System.logAction('Groups - Group Deleted');
            top.privateServices.DeleteGroup(groupID, function () {
                window.location = window.location;
            });
            
            
        }

    });

    $('#groupEdit').click(function () {
        ShowDialog(top.GetGroupDialog(groupID));
        top.Ts.System.logAction('Groups - Edit Group Dialog Opened');
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