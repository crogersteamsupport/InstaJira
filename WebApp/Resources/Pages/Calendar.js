$(document).ready(function () {
    var mainFrame = getMainFrame();
    //check if the calendar is on a sub section or not
    var pageType = mainFrame.Ts.Utils.getQueryValue("pagetype", window);
    var pageID = mainFrame.Ts.Utils.getQueryValue("pageid", window);
    var theTempEvent = null;
    var dateFormat;
    var isNewButton = false;
    var clicks = 0, timeout;

    if (pageType == null)
        pageType = -1;

    if (pageID == null)
        pageID = -1;

    $('body').layout({
        applyDemoStyles: true
    });

    mainFrame.Ts.System.logAction('Calendar - Loaded');

    //setup the dateformat
    mainFrame.Ts.Services.Customers.GetDateFormat(false, function (format) {
        dateFormat = format.replace("yyyy", "yy");

        if (dateFormat.indexOf("DD") == -1)
            dateFormat = dateFormat.replace("D", "DD");

        if (dateFormat.indexOf("MM") == -1)
            dateFormat = dateFormat.replace("M", "MM");

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


    //test function
    var date = new Date();
    var day = date.getDate();        // yields day
    var month = date.getMonth()+1;    // yields month
    var year = date.getFullYear();  // yields year
    // After this construct a string with the above results as below
    var time = day + "/" + month + "/" + year;
    var tempVar = "";
    //var e = mainFrame.Ts.Services.Users.GetCalendarEvents($.fullCalendar.moment('2014-05-01'));

    //initialize the calendar
    $('#calendar').fullCalendar({
        header: {
            left: 'prev,next today custom',
            center: 'title',
            right: 'month,agendaWeek,agendaDay'
        },
        editable: true,
        eventLimit: true,
        height: '200',
        aspectRatio: 2.3,
        nextDayThreshold: '00.00.01',
        viewRender: function (view, element, date)
        {
        },
        dayRender: function(date, element, view){
            //element.bind('dblclick', function() {
            //    clearModal();
            //    isNewButton = false;
            //    $('#inputStartTime').val(moment(date).format(dateFormat + ' hh:mm a'));

            //    if ($('#inputStartTime').data("DateTimePicker"))
            //        $('#inputStartTime').data("DateTimePicker").destroy();
            //    $('#inputStartTime').datetimepicker({ format: dateFormat + ' hh:mm a' , pickDate: false, minuteStepping: 30});
            //    $('#inputStartTime').data("DateTimePicker").setDate(moment(date));

            //    $('#inputEndTime').datetimepicker({ format: dateFormat + ' hh:mm a', minuteStepping: 30 });
            //    //$('#inputEndTime').data("DateTimePicker").setDate(moment(date));

            //    theTempEvent = null;
            //    $('#fullCalModal').modal();
            //});
        },
        dayClick: function (date, jsEvent, view) {

            //if (tempVar == "") {
            //    $(this).css('background-color', '#c6dcf7');
            //    tempVar = this;

            //}
            //else {
            //    $(tempVar).css('background-color', 'white');
            //    $(this).css('background-color', '#c6dcf7');
            //    tempVar = this;
            //}

            $('#calendar').fullCalendar('gotoDate', date);
                clicks++;
                if (clicks == 1) {
                    timeout = setTimeout(function () { clicks = 0; }, 400);
                } else {
                    timeout && clearTimeout(timeout);
                    showModal(date);
                    clicks = 0;
                }
            $('.popover').hide();
        },
        timezone: 'local',
        ignoreTimezone: false,
        events: function (start, end, timezone, callback) {
            var thestart = start.toISOString();
            $.ajax({
                type: "POST",
                url: '/Services/UserService.asmx/GetCalendarEvents',
                data: JSON.stringify({ 'startdate': thestart, 'pageType' : pageType, 'pageID' : pageID }),
                cache: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    var events = [];
                    $(data.d).each(function () {
                        var editable;
                        if (this.creatorID == mainFrame.Ts.System.User.UserID || mainFrame.Ts.System.User.IsSystemAdmin)
                            editable = true;
                        else
                            editable = false;
                        events.push({
                            title: this.title,
                            start: this.start,
                            textColor: this.color,
                            color: '#fff',
                            borderColor: '#ddd',
                            type: this.type,
                            id: this.id,
                            description: this.description,
                            validend: this.end,
                            end: this.displayend,
                            allDay: false,
                            isallDay: this.allday,
                            isHoliday: this.isHoliday,
                            references: this.references,
                            creatorID: this.creatorID,
                            editable: editable
                        });
                    });
                    $("#calendar .holiday").removeClass("holiday");
                    callback(events);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('There was an error loading calendar events');
                }
            });
        },
        eventDrop: function (event, delta, revertFunc) {
            mainFrame.Ts.Services.Users.ChangeEventDate(event.id, event.start.format(), event.end.format(), event.type, event.allDay, function () {
                $('#calendar').fullCalendar('refetchEvents');
            });

        },
        eventRender: function (event, element) {
            var evttitle;
            switch(event.type)
            {
                case "reminder-ticket":
                    evttitle = "Ticket Reminder : " + event.id;
                    break;
                case "reminder-org":
                    evttitle = "Company Reminder";
                    break;
                case "reminder-user":
                    evttitle = "Customer Reminder";
                    break;
                case "ticket":
                    evttitle = "Ticket Due Date : " + event.id;
                    break;
                case "cal":
                    evttitle = event.title;
                    break;
                case "reminder-task":
                    evttitle = "Task Reminder";
                    break;
                case "task":
                    evttitle = "Task Due Date : ";
                    break;
                default:
                    break;

            }
            element.popover({
                title: evttitle,
                placement:'auto',
                html:true,
                trigger: 'click',
                container: 'body',
                animation: 'true',
                content: buildPopover(event, element)

            });
            $('body').on('click', function (e) {
                if (!element.is(e.target) && element.has(e.target).length === 0 && $('.popover').has(e.target).length === 0)
                {
                    //$('.popover').hide();
                    element.popover('hide');
                }

            });

            if (event.isHoliday) {
                try {
                    if (event.hasOwnProperty("start") && event.hasOwnProperty("validend")) {
                        var startDate = new Date(event.start._i);
                        startDate.setTime(startDate.getTime() + (Math.abs(startDate.getTimezoneOffset()) * 60 * 1000));
                        var endDate = new Date(event.end._d);
                        endDate = new Date(endDate.getFullYear(), endDate.getMonth(), endDate.getDate(), 0, 0, 0, 0);
                        startDate = new Date(startDate.getFullYear(), startDate.getMonth(), startDate.getUTCDate(), 0, 0, 0, 0);
                        var today = new Date();
                        today = new Date(today.getFullYear(), today.getMonth(), today.getUTCDate(), 0, 0, 0, 0);
                        var stretchDate = new Date(startDate);
                        var year;
                        var month;
                        var day;
                        var holidayCss = 'holiday';

                        while (stretchDate.getTime() <= endDate.getTime()) {
                            if (stretchDate.getTime() !== today.getTime()) {
                                year = stretchDate.getFullYear();
                                month = (((stretchDate.getMonth() + 1).toString().length < 2) ? "0" + parseInt(stretchDate.getMonth() + 1) : stretchDate.getMonth() + 1);
                                day = ((stretchDate.getDate().toString().length < 2) ? "0" + stretchDate.getDate() : stretchDate.getDate());
                                var dayCell = $('.fc-day[data-date="' + year + '-' + month + '-' + day + '"]');

                                if (!$(dayCell).hasClass(holidayCss)) {
                                    $(dayCell).addClass(holidayCss);
                                }
                            }

                            stretchDate.setDate(stretchDate.getDate() + 1);
                        }
                    }
                }
                catch (e) {
                    //Couldn't mark the day as Holiday.
                }
            }
        },
        eventClick: function (calEvent, jsEvent, view) {
            theTempEvent = calEvent;
        }

    });

    // edit event delagate
    $('body').on('click', '.eventEdit', function (e) {
        var event = theTempEvent;
        mainFrame.Ts.System.logAction('Calendar Event - Edit Event Clicked');
        $('.popover').not(this).hide();
        switch (event.type)
        {
            case "reminder-ticket":
                mainFrame.Ts.MainPage.openTicket(event.id); return false;
                break;
            case "reminder-org":
                mainFrame.Ts.MainPage.openNewCustomer(event.id); return false;
                break;
            case "reminder-user":
                mainFrame.Ts.MainPage.openNewContact(event.id); return false;
                break;
            case "ticket":
                mainFrame.Ts.MainPage.openTicket(event.id); return false;
                break;
            case "cal":

                clearModal();
                if (event.creatorID != mainFrame.Ts.System.User.UserID && !mainFrame.Ts.System.User.IsSystemAdmin)
                {
                    loadModal(event, false);
                    readOnlyModal();
                }
                else
                    loadModal(event, true);



                $('#fullCalModal').modal('show');
                break;
            case "reminder-task":
            case "task":
                mainFrame.Ts.MainPage.openNewTask(event.id); return false;
                break;
            default:
                break;
        }
    });

    // delete event delegate
    $('body').on('click', '.eventDelete', function (e) {
        var eventid = theTempEvent;
        if (confirm("Are you sure you want to delete this event")) {
            mainFrame.Ts.Services.Users.DeleteCalEvent(eventid.id, function () {
                $('.popover').hide();
                $("#calendar .holiday").removeClass("holiday");
                $("#calendar").fullCalendar('refetchEvents');
                mainFrame.Ts.System.logAction('Calendar Event - Deleted');
                theTempEvent = null;
            });
        }
    });

    //Convert buttons to bootstrap classes
    $('.fc-toolbar').find('.fc-button').addClass('btn btn-curiousblue');

    //toggle for allday input
    $('#inputAllDay').click(function (e) {
        if($('#inputAllDay').is(':checked'))
        {
            //set start time to time only format
            if ($('#inputStartTime').data("DateTimePicker"))
                $('#inputStartTime').data("DateTimePicker").destroy();
            $('#inputStartTime').datetimepicker({ format: dateFormat, pickTime: false });
            $('#inputStartTime').val(moment($('#inputStartTime').val(), dateFormat).format(dateFormat));

            //set end time to time only format
            if ($('#inputEndTime').data("DateTimePicker"))
                $('#inputEndTime').data("DateTimePicker").destroy();
            $('#inputEndTime').datetimepicker({ format: dateFormat, pickTime: false });
            $('#inputEndTime').val(moment($('#inputEndTime').val(), dateFormat).format(dateFormat));
            //$('#inputEndTime').data("DateTimePicker").setDate($('#inputStartTime').val());

        } else {


            if ($('#inputStartTime').data("DateTimePicker"))
                $('#inputStartTime').data("DateTimePicker").destroy();

            if ($('#inputEndTime').data("DateTimePicker"))
                $('#inputEndTime').data("DateTimePicker").destroy();

            //if new event leave it date/time event
            if (isNewButton)
            {
                $('#inputStartTime').datetimepicker({ format: dateFormat + ' hh:mm a', minuteStepping: 30 });
                $('#inputStartTime').val(moment($('#inputStartTime')).add(moment().hours(), 'hour').format(dateFormat + ' hh:mm a'));

                $('#inputEndTime').datetimepicker({ format: dateFormat + ' hh:mm a', minuteStepping: 30 });
                $('#inputEndTime').val(moment($('#inputEndTime')).add(moment().hours()+1, 'hour').format(dateFormat + ' hh:mm a'));
            }
            else //if existing event
            {
                //set dates to time only
                $('#inputStartTime').datetimepicker({ format: dateFormat + ' hh:mm a', pickDate: false, minuteStepping: 30 });
                $('#inputStartTime').val(moment($('#inputStartTime').val(), dateFormat).add(moment().hours(), 'hour').format(dateFormat + ' hh:mm a'));

                $('#inputEndTime').datetimepicker({ format: dateFormat + ' hh:mm a', pickDate: false, minuteStepping: 30 });
                $('#inputEndTime').val(moment($('#inputEndTime').val(), dateFormat).add(moment().hours()+1, 'hour').format(dateFormat + ' hh:mm a'));
            }


        }

    });

    $('#inputIsHoliday').click(function (e) {
        if ($('#inputIsHoliday').is(':checked')) {
            $('#inputAllDay').prop('checked', true).triggerHandler('click');
            $('#inputAllDay').attr('disabled', true);
        } else {
            $('#inputAllDay').attr('disabled', false);
        }
    });

    function showModal(date)
    {
        clearModal();
        isNewButton = false;
        //$('#inputStartTime').val(moment(date).format(dateFormat + ' hh:mm a'));

        if ($('#inputStartTime').data("DateTimePicker"))
            $('#inputStartTime').data("DateTimePicker").destroy();
        $('#inputStartTime').datetimepicker({ format: dateFormat + ' hh:mm a', pickDate: false, minuteStepping: 30 });
        $('#inputStartTime').data("DateTimePicker").setDate(moment(date).add(moment().hours(),'hour'));

        if ($('#inputEndTime').data("DateTimePicker"))
            $('#inputEndTime').data("DateTimePicker").destroy();
        $('#inputEndTime').datetimepicker({ format: dateFormat + ' hh:mm a', pickDate: false, minuteStepping: 30 });
        $('#inputEndTime').data("DateTimePicker").setDate(moment(date).add(moment().hours()+1, 'hour'));

        theTempEvent = null;
        $('#fullCalModal').modal();
    }

    //build popover content
    function buildPopover(event, element)
    {
        var theTime = $("<div>")
            .text(moment(event.start).format(dateFormat + ' hh:mm a'));
        if (event.validend)
        {
            var endTime = $("<div>")
                .text("to " + moment(event.validend).format(dateFormat + ' hh:mm a')).appendTo(theTime);
        }

        if (event.type != "cal")
            var descType = $("<div>")
                            .text(event.description)
                            .appendTo(theTime);


        var lb = $("<hr>")
            .appendTo(theTime);

        if (event.references)
        {
            var refstring = "";
            for (i = 0; i < event.references.length; i++)
            {
                switch (event.references[i].RefType) {
                    case 0:
                        refstring += '<a href="#" target="_blank" onclick="parent.Ts.MainPage.openTicket(' + event.references[i].RefID + '); return false;">' + event.references[i].displayName + '</a><br/>';
                        break;
                    case 1:
                        refstring += '<a href="#" target="_blank" onclick="parent.Ts.MainPage.openNewProduct(' + event.references[i].RefID + '); return false;">' + event.references[i].displayName + '</a><br/>';
                        break;
                    case 2:
                        refstring += '<a href="#" target="_blank" onclick="parent.Ts.MainPage.openNewCustomer(' + event.references[i].RefID + '); return false;">' + event.references[i].displayName + '</a><br/>';
                        break;
                    case 3:
                        refstring += '<a href="#" target="_blank" onclick="parent.Ts.MainPage.openUser(' + event.references[i].RefID + '); return false;">' + event.references[i].displayName + '</a><br/>';
                        break;
                    case 4:
                        refstring += '<a href="#" target="_blank" onclick="parent.Ts.MainPage.openGroup(' + event.references[i].RefID + '); return false;">' + event.references[i].displayName + '</a><br/>';
                        break;
                }
            }

            var attachments = $('<div>')
            .html(refstring)
            .appendTo(theTime);

            var lb2 = $("<hr>")
                .appendTo(theTime);
        }



        var goButton = $("<a>")
            .addClass("eventEdit")
            .data('event', event)
            .text("View")
            .appendTo(theTime);

        if (event.type == "cal" && (mainFrame.Ts.System.User.IsSystemAdmin || event.creatorID == mainFrame.Ts.System.User.UserID))
        {
            var delButton = $("<a>")
                .addClass("pull-right eventDelete")
                .data('eventid', event.id)
                .text('Delete')
                .appendTo(theTime);
        }

        return theTime;
    }

    //default setup functions
    //$('.datetimepicker').datetimepicker({ format: 'MM/DD/YYYY hh:mm a'});
    $('#associationSearch').attr("placeholder", "Search Tickets").val("");
    $('#associationSearch').autocomplete({
        minLength: 2, source: getTicketsByTerm, delay: 300,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $('#associationQueue').find('.ticket-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Ticket')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($('#associationQueue').find('.ticket-queue')).data('Ticket', ui.item.id);


                    $('<span>')
                    .text(ellipseString(ui.item.value, 20))
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                    this.value = "";
                    return false;
                }
            }
        }
    });
    //handle the event association
    $('.addticket').click(function ()
    { calAssociation("0"); });
    $('.adduser').click(function ()
    { calAssociation("1"); });
    $('.addcustomer').click(function ()
    { calAssociation("2"); });
    $('.addgroup').click(function ()
    { calAssociation("3"); });
    $('.addproduct').click(function ()
    { calAssociation("4"); });

    function calAssociation(associationType)
    {
        var searchbox = $('#associationSearch');
        switch (associationType)
        {
            case "0":
                $('#searchGroup').show();
                $(".arrow-up").css('left', '7px');
                $('#associationSearch').attr("placeholder", "Search Tickets").val("");
                $('#associationSearch').autocomplete({
                    minLength: 2, source: getTicketsByTerm, delay: 300,
                    select: function (event, ui) {
                        if (ui.item) {
                            var isDupe;
                            $('#associationQueue').find('.ticket-queue').find('.ticket-removable-item').each(function () {
                                if (ui.item.id == $(this).data('Ticket')) {
                                    isDupe = true;
                                }
                            });
                            if (!isDupe) {
                                var bg = $('<div>')
                                .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                                .appendTo($('#associationQueue').find('.ticket-queue')).data('Ticket', ui.item.id);


                                $('<span>')
                                .text(ellipseString(ui.item.value,20))
                                .addClass('filename')
                                .appendTo(bg);

                                $('<span>')
                                .addClass('ui-icon ui-icon-close')
                                .click(function (e) {
                                    e.preventDefault();
                                    $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                                })
                                .appendTo(bg);
                                this.value = "";
                                return false;
                            }
                        }
                    }
                });
                break;
            case "1":
                $('#searchGroup').show();
                $(".arrow-up").css('left', '30px');
                $('#associationSearch').attr("placeholder", "Search Users").val("");
                searchbox.autocomplete({
                    minLength: 3, source: getUsers, delay: 300,
                    select: function (event, ui) {
                        if (ui.item) {
                            var isDupe;
                            $('#associationQueue').find('.user-queue').find('.ticket-removable-item').each(function () {
                                if (ui.item.id == $(this).data('User')) {
                                    isDupe = true;
                                }
                            });
                            if (!isDupe) {
                                var bg = $('<div>')
                            .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                            .appendTo($('#associationQueue').find('.user-queue')).data('User', ui.item.id);


                                $('<span>')
                            .text(ellipseString(ui.item.value, 20))
                            .addClass('filename')
                            .appendTo(bg);

                                $('<span>')
                            .addClass('ui-icon ui-icon-close')
                            .click(function (e) {
                                e.preventDefault();
                                $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                            })
                            .appendTo(bg);
                                this.value = "";
                                return false;
                            }
                        }
                    }
                });
                break;
            case "2":
                $('#searchGroup').show();
                $(".arrow-up").css('left', '53px');
                $('#associationSearch').attr("placeholder", "Search Companies").val("");
                $('#associationSearch').autocomplete({
                    minLength: 3,
                    source: getCustomers,
                    select: function (event, ui) {
                        if (ui.item) {
                            var isDupe;
                            $('#associationQueue').find('.customer-queue').find('.ticket-removable-item').each(function () {
                                if (ui.item.id == $(this).data('Company')) {
                                    isDupe = true;
                                }
                            });
                            if (!isDupe) {
                                var bg = $('<div>')
                                .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                                .appendTo($('#associationQueue').find('.customer-queue')).data('Company', ui.item.id);


                                $('<span>')
                                .text(ellipseString(ui.item.value, 20))
                                .addClass('filename')
                                .appendTo(bg);

                                $('<span>')
                                .addClass('ui-icon ui-icon-close')
                                .click(function (e) {
                                    e.preventDefault();
                                    $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                                })
                                .appendTo(bg);
                                this.value = "";
                                return false;
                            }
                        }
                    }
                });
                break;
            case "3":
                $('#searchGroup').show();
                $(".arrow-up").css('left', '78px');
                $('#associationSearch').attr("placeholder", "Search Groups").val("");
                $('#associationSearch').autocomplete({
                    minLength: 2,
                    source: getGroupsByTerm,
                    select: function (event, ui) {
                        if (ui.item) {
                            var isDupe;
                            $('#associationQueue').find('.group-queue').find('.ticket-removable-item').each(function () {
                                if (ui.item.id == $(this).data('Group')) {
                                    isDupe = true;
                                }
                            });
                            if (!isDupe) {
                                var bg = $('<div>')
                                .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                                .appendTo($('#associationQueue').find('.group-queue')).data('Group', ui.item.id);


                                $('<span>')
                                .text(ellipseString(ui.item.value, 20))
                                .addClass('filename')
                                .appendTo(bg);

                                $('<span>')
                                .addClass('ui-icon ui-icon-close')
                                .click(function (e) {
                                    e.preventDefault();
                                    $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                                })
                                .appendTo(bg);
                                this.value = "";
                                return false;
                            }
                        }
                    }
                });
                break;
            case "4":
                $('#searchGroup').show();
                $(".arrow-up").css('left', '104px');
                $('#associationSearch').attr("placeholder", "Search Products").val("");
                $('#associationSearch').autocomplete({
                    minLength: 3,
                    source: getProductByTerm,
                    select: function (event, ui) {
                        if (ui.item) {
                            var isDupe;
                            $('#associationQueue').find('.product-queue').find('.ticket-removable-item').each(function () {
                                if (ui.item.id == $(this).data('Product')) {
                                    isDupe = true;
                                }
                            });
                            if (!isDupe) {
                                var bg = $('<div>')
                                .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                                .appendTo($('#associationQueue').find('.product-queue')).data('Product', ui.item.id);


                                $('<span>')
                                .text(ellipseString(ui.item.value, 20))
                                .addClass('filename')
                                .appendTo(bg);

                                $('<span>')
                                .addClass('ui-icon ui-icon-close')
                                .click(function (e) {
                                    e.preventDefault();
                                    $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                                })
                                .appendTo(bg);
                                this.value = "";
                                return false;
                            }
                        }
                    }
                });
                break;
        }
    }

    function convertToValidDate(val)
    {
        var value = '';
        if (val == "")
            return value;

        if (dateFormat.indexOf("M") != 0) {
            var dateArr = val.replace(/\./g, '/').replace(/-/g, '/').split('/');
            if (dateFormat.indexOf("D") == 0)
                var day = dateArr[0];
            if (dateFormat.indexOf("Y") == 0)
                var year = dateArr[0];
            if (dateFormat.indexOf("M") == 3 || dateFormat.indexOf("M") == 5)
                var month = dateArr[1];

            var timeSplit = dateArr[2].split(' ');
            if (dateFormat.indexOf("Y") == 6)
                var year = timeSplit[0];
            else
                var day = timeSplit[0];

            var theTime = timeSplit[1];

            var formattedDate = month + "/" + day + "/" + year + " " + theTime + (timeSplit[2] != null ? " " + timeSplit[2] : "");
            value = mainFrame.Ts.Utils.getMsDate(formattedDate);
            return value;
        }
        else
            return val;
    }

    //save the new calendar event
    $('#btnSaveEvent').click(function (e) {
        e.preventDefault();
        e.stopPropagation();

        if ($('#inputTitle').val() == "")
        {
            alert("Please enter a valid event title.");
            return;
        }

        if (moment($('#inputStartTime').val(), dateFormat).isValid() == false)
        {
            alert("A valid start date must be entered.");
        }


        if ($('#inputEndTime').val() != "")
        {
            var start = moment($('#inputStartTime').val(), dateFormat);
            var end = moment($('#inputEndTime').val(), dateFormat);
            if (moment(end).isBefore(start))
            {
                alert("The end date needs to be after the start date");
                return;
            }
        }


        var calendarinfo = new Object();

        if (theTempEvent)
            calendarinfo.ID = theTempEvent.id;
        else
            calendarinfo.ID = -1;
        calendarinfo.title = $('#inputTitle').val();

        Date.prototype.stdTimezoneOffset = function () {
        	var jan = new Date(this.getFullYear(), 0, 1);
        	var jul = new Date(this.getFullYear(), 6, 1);
        	return Math.max(jan.getTimezoneOffset(), jul.getTimezoneOffset());
        }

        Date.prototype.dst = function () {
        	return this.getTimezoneOffset() < this.stdTimezoneOffset();
        }

        var today = new Date();
        var startDateTz =  $('#inputStartTime').val() + (today.dst() == true ? " 1:00 am" : " 12:00 am");
        var endDateTz = $('#inputEndTime').val() + (today.dst() == true ? " 12:00 am" : " 1:00 am");

        calendarinfo.start = $('#inputAllDay').is(':checked') ? moment(convertToValidDate(startDateTz)).valueOf() : moment(convertToValidDate($('#inputStartTime').val())).valueOf();
        calendarinfo.end = $('#inputAllDay').is(':checked') ? moment(convertToValidDate(endDateTz)).valueOf() : moment(convertToValidDate($('#inputEndTime').val())).valueOf();
        calendarinfo.description = $('#inputDescription').val();
        calendarinfo.allday = $('#inputAllDay').is(':checked');
        calendarinfo.isHoliday = $('#inputIsHoliday').is(':checked');

        calendarinfo.PageType = pageType;
        calendarinfo.PageID = pageID;

        calendarinfo.Tickets = new Array();
        $('#associationQueue').find('.ticket-queue').find('.ticket-removable-item').each(function () {
            calendarinfo.Tickets[calendarinfo.Tickets.length] = $(this).data('Ticket');
        });

        calendarinfo.Groups = new Array();
        $('#associationQueue').find('.group-queue').find('.ticket-removable-item').each(function () {
            calendarinfo.Groups[calendarinfo.Groups.length] = $(this).data('Group');
        });

        calendarinfo.Products = new Array();
        $('#associationQueue').find('.product-queue').find('.ticket-removable-item').each(function () {
            calendarinfo.Products[calendarinfo.Products.length] = $(this).data('Product');
        });

        calendarinfo.Company = new Array();
        $('#associationQueue').find('.customer-queue').find('.ticket-removable-item').each(function () {
            calendarinfo.Company[calendarinfo.Company.length] = $(this).data('Company');
        });

        calendarinfo.User = new Array();
        $('#associationQueue').find('.user-queue').find('.ticket-removable-item').each(function () {
            calendarinfo.User[calendarinfo.User.length] = $(this).data('User');
        });

        if (calendarinfo.Tickets.length > 0) mainFrame.Ts.System.logAction('Calendar Event - Ticket Inserted');
        if (calendarinfo.Groups.length > 0) mainFrame.Ts.System.logAction('Calendar Event - Group Inserted');
        if (calendarinfo.Products.length > 0) mainFrame.Ts.System.logAction('Calendar Event - Product Inserted');
        if (calendarinfo.Company.length > 0) mainFrame.Ts.System.logAction('Calendar Event - Company Inserted');
        if (calendarinfo.User.length > 0) mainFrame.Ts.System.logAction('Calendar Event - User Inserted');

        mainFrame.Ts.Services.Users.SaveCalendarEvent(mainFrame.JSON.stringify(calendarinfo), function (result) {
            if (result)
            {
                $('#fullCalModal').modal('hide');
                clearModal();
                $("#calendar").fullCalendar('refetchEvents');
            }
            else
            {
                alert("A valid start date must be entered.");
            }
            mainFrame.Ts.System.logAction('Calendar Event - Event Inserted');
            //window.mainFrame.ticketSocket.server.calendarUpdate();
        });


    });

    addCalButton("right", "refresh", "fa fa-refresh", "Refresh Calendar");

    if (pageID == 0)
        $('#calendar').fullCalendar('render');

    if (pageID == -1)
        addCalButton("right", "calURL", "fa fa-rss", "Get Calendar Feed URL");


    addCalButton("right", "newEvent", "fa fa-plus", "Add Calendar Event");



    function addCalButton(where, id, css, title) {
        var my_button = '<button data-toggle="tooltip" class="fc-button fc-state-default fc-corner-right btn btn-curiousblue" id="' + id + '" title="' + title + '"><i class="' + css + '"></i></button>';
        $(".fc-" + where + " .fc-button-group").append(my_button);
        $("#" + id).button();
    }

    $("#refresh").click(function () { $('#calendar').fullCalendar('refetchEvents') });

    $("#calURL").click(function () { $('#subscribeURL').val(mainFrame.Ts.System.AppDomain + "/dc/" + mainFrame.Ts.System.User.OrganizationID + "/calendarfeed/" + mainFrame.Ts.System.User.CalGUID); $('#subscribeModal').modal(); mainFrame.Ts.System.logAction('Calendar Event - Subscription Button Clicked'); });
    $("#newEvent").click(function () {
        clearModal();
        theTempEvent = null;
        isNewButton = true;
        mainFrame.Ts.System.logAction('Calendar Event - New Event Button Clicked');
        $('#fullCalModal').modal();
        if ($('#inputStartTime').data("DateTimePicker"))
            $('#inputStartTime').data("DateTimePicker").destroy();

        $('#inputStartTime').datetimepicker({ format: dateFormat + ' hh:mm a', pickDate: true, minuteStepping: 30 });
        $('#inputStartTime').data("DateTimePicker").setDate(moment(new (Date)).set('minute', 00));

        if ($('#inputEndTime').data("DateTimePicker"))
            $('#inputEndTime').data("DateTimePicker").destroy();
        $('#inputEndTime').datetimepicker({ format: dateFormat + ' hh:mm a', pickDate: true, minuteStepping: 30 });
        $('#inputEndTime').data("DateTimePicker").setDate(moment(new (Date)).set('minute', 00).add(1,'hour'));
    });

    //search functions for the associations
    var execGetCustomer = null;
    function getCustomers(request, response) {
        if (execGetCustomer) { execGetCustomer._executor.abort(); }
        execGetCustomer = mainFrame.Ts.Services.Organizations.WCSearchOrganization(request.term, function (result) {
            response(result);
        });
    }

    var execGetUsers = null;
    function getUsers(request, response) {
        if (execGetUsers) { execGetUsers._executor.abort(); }
        execGetUsers = mainFrame.Ts.Services.Users.SearchUsers(request.term, function (result) { response(result); });
    }

    var execGetTicket = null;
    function getTicketsByTerm(request, response) {
        if (execGetTicket) { execGetTicket._executor.abort(); }
        //execGetTicket = Ts.Services.Tickets.GetTicketsByTerm(request.term, function (result) { response(result); });
        execGetTicket = mainFrame.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) {
            $('.main-quick-ticket').removeClass('ui-autocomplete-loading');
            response(result);
        });

    }

    var execGetGroups = null;
    function getGroupsByTerm(request, response) {
        if (execGetGroups) { execGetGroups._executor.abort(); }
        execGetTicket = mainFrame.Ts.Services.WaterCooler.GetGroupsByTerm(request.term, function (result) { response(result); });
    }

    var execGetProducts = null;
    function getProductByTerm(request, response) {
        if (execGetProducts) { execGetProducts._executor.abort(); }
        execGetProducts = mainFrame.Ts.Services.WaterCooler.GetProductsByTerm(request.term, function (result) { response(result); });
    }

    //text shortening
    function ellipseString(text, max) {
        return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text;
    };

    //json date parser
    function parseJsonDate(jsonDateString) {
        return new Date(parseInt(jsonDateString.replace('/Date(', '')));
    }

    //clears the modal
    function clearModal()
    {
        $('#inputTitle').val("");
        $('#inputStartTime').val("");
        $('#inputEndTime').val("");
        $('#inputDescription').val("");
        $('#calAssociation').val("-1");
        $('#associationQueue').find('.upload-queue').empty();
        $('#associationQueue').find('.ticket-queue').empty();
        $('#associationQueue').find('.group-queue').empty();
        $('#associationQueue').find('.customer-queue').empty();
        $('#associationQueue').find('.user-queue').empty();
        $('#associationQueue').find('.product-queue').empty();
        $('.form-control').prop('disabled', false);
        $('#inputAllDay').prop('disabled', false);
        $('#inputAllDay').prop('checked', false);
        $('#inputIsHoliday').prop('disabled', false);
        $('#inputIsHoliday').prop('checked', false);

        $('#inputRecurring').prop('disabled', false);
        $('#btnSaveEvent').show();
    }

    function loadModal(event, editable)
    {
        $('#inputTitle').val(event.title);

        if (event.isallDay)
        {
            $('#inputStartTime').val(moment(event.start).format(dateFormat));
            $('#inputEndTime').val(event.validend == null ? event.validend : moment(event.validend).format(dateFormat));

            if ($('#inputStartTime').data("DateTimePicker"))
                $('#inputStartTime').data("DateTimePicker").destroy();

            $('#inputStartTime').datetimepicker({ format: dateFormat, pickTime: false });
            if ($('#inputEndTime').data("DateTimePicker"))
                $('#inputEndTime').data("DateTimePicker").destroy();

            $('#inputEndTime').datetimepicker({ format: dateFormat, pickTime: false });
        }
        else
        {
            $('#inputStartTime').val(moment(event.start).format(dateFormat + ' hh:mm a'));
            $('#inputEndTime').val(event.validend == null ? event.validend : moment(event.validend).format(dateFormat + ' hh:mm a'));
            if ($('#inputStartTime').data("DateTimePicker"))
                $('#inputStartTime').data("DateTimePicker").destroy();
            if ($('#inputEndTime').data("DateTimePicker"))
                $('#inputEndTime').data("DateTimePicker").destroy();
            $('#inputStartTime').datetimepicker({ format: dateFormat + ' hh:mm a' });
            $('#inputEndTime').datetimepicker({ format: dateFormat + ' hh:mm a' });
        }




        $('#inputDescription').val(event.description);
        $('#inputAllDay').prop('checked', event.isallDay);
        $('#inputIsHoliday').prop('checked', event.isHoliday);

        if (event.isHoliday) {
            $('#inputAllDay').attr('disabled', true);
        }

        if (event.references) {
            for (i = 0; i < event.references.length; i++)
            {
                switch (event.references[i].RefType)
                {
                    case 0:
                        addTicketAssociation(event.references[i], editable);
                        break;
                    case 1:
                        addProductAssociation(event.references[i], editable);
                        break;
                    case 2:
                        addCompanyAssociation(event.references[i], editable)
                        break;
                    case 3:
                        addUserAssociation(event.references[i], editable);
                        break;
                    case 4:
                        addGroupAssociation(event.references[i], editable);
                        break;
                }

            }
        }

    }

    function addTicketAssociation(event, editable)
    {
        var isDupe;
        $('#associationQueue').find('.ticket-queue').find('.ticket-removable-item').each(function () {
            if (event.RefID == $(this).data('Ticket')) {
                isDupe = true;
            }
        });
        if (!isDupe) {
            var bg = $('<div>')
            .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
            .appendTo($('#associationQueue').find('.ticket-queue')).data('Ticket', event.RefID);


            $('<span>')
            .text(ellipseString(event.displayName, 20))
            .addClass('filename')
            .appendTo(bg);

            if (editable) {
                $('<span>')
                .addClass('ui-icon ui-icon-close')
                .click(function (e) {
                    e.preventDefault();
                    $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                })
                .appendTo(bg);
                this.value = "";
                return false;
            }
        }
    }

    function addUserAssociation(event, editable)
    {
        var isDupe;
        $('#associationQueue').find('.user-queue').find('.ticket-removable-item').each(function () {
            if (event.RefID == $(this).data('User')) {
                isDupe = true;
            }
        });
        if (!isDupe) {
            var bg = $('<div>')
        .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
        .appendTo($('#associationQueue').find('.user-queue')).data('User', event.RefID);


            $('<span>')
        .text(ellipseString(event.displayName, 20))
        .addClass('filename')
        .appendTo(bg);
            if (editable) {
                $('<span>')
            .addClass('ui-icon ui-icon-close')
            .click(function (e) {
                e.preventDefault();
                $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
            })
            .appendTo(bg);
                this.value = "";
                return false;
            }
        }
    }

    function addCompanyAssociation(event, editable)
    {
        var isDupe;
        $('#associationQueue').find('.customer-queue').find('.ticket-removable-item').each(function () {
            if (event.RefID == $(this).data('Company')) {
                isDupe = true;
            }
        });
        if (!isDupe) {
            var bg = $('<div>')
            .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
            .appendTo($('#associationQueue').find('.customer-queue')).data('Company', event.RefID);


            $('<span>')
            .text(ellipseString(event.displayName, 20))
            .addClass('filename')
            .appendTo(bg);

            if (editable) {
                $('<span>')
                .addClass('ui-icon ui-icon-close')
                .click(function (e) {
                    e.preventDefault();
                    $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                })
                .appendTo(bg);
                this.value = "";
                return false;
            }
        }
    }

    function addGroupAssociation(event, editable)
    {
        var isDupe;
        $('#associationQueue').find('.group-queue').find('.ticket-removable-item').each(function () {
            if (event.RefID == $(this).data('Group')) {
                isDupe = true;
            }
        });
        if (!isDupe) {
            var bg = $('<div>')
            .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
            .appendTo($('#associationQueue').find('.group-queue')).data('Group', event.RefID);


            $('<span>')
            .text(ellipseString(event.displayName, 20))
            .addClass('filename')
            .appendTo(bg);

            if (editable) {
                $('<span>')
                .addClass('ui-icon ui-icon-close')
                .click(function (e) {
                    e.preventDefault();
                    $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                })
                .appendTo(bg);
                this.value = "";
                return false;
            }
        }
    }

    function addProductAssociation(event, editable)
    {
        var isDupe;
        $('#associationQueue').find('.product-queue').find('.ticket-removable-item').each(function () {
            if (event.RefID == $(this).data('Product')) {
                isDupe = true;
            }
        });
        if (!isDupe) {
            var bg = $('<div>')
            .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
            .appendTo($('#associationQueue').find('.product-queue')).data('Product', event.RefID);


            $('<span>')
            .text(ellipseString(event.displayName, 20))
            .addClass('filename')
            .appendTo(bg);

            if (editable) {
                $('<span>')
                .addClass('ui-icon ui-icon-close')
                .click(function (e) {
                    e.preventDefault();
                    $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                })
                .appendTo(bg);
                this.value = "";
                return false;
            }
        }
    }

    function readOnlyModal()
    {
        $('.form-control').prop('disabled', true);
        $('#inputAllDay').prop('disabled', true);
        $('#inputIsHoliday').prop('disabled', true);
        $('#inputRecurring').prop('disabled', true);
        $('#btnSaveEvent').hide();
    }

    $('[data-toggle="tooltip"]').tooltip({placement: 'auto'});

});
