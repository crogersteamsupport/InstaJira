$(document).ready(function () {
	var _isEditing = false;
	var _reportIdToSchedule = parent.Ts.Utils.getQueryValue('ReportId', window);
	var _reportIdToScheduleName = parent.Ts.Utils.getQueryValue('ReportName', window);
	var _editingScheduledId = parent.Ts.Utils.getQueryValue('ScheduledReportId', window);
	var _returnTo = parent.Ts.Utils.getQueryValue('ReturnTo', window);
	var _reportTypeOpened = parent.Ts.Utils.getQueryValue('ReportTypeOpened', window);

	$('.schedule-email-subject').siblings('span').hide();
	$('.schedule-email-body').children('span').hide();
	$('.schedule-email-addresses').siblings('span').hide();
	$('#StartOnTime').siblings('span').hide();

	if (_editingScheduledId != undefined
		&& _editingScheduledId != null
		&& _editingScheduledId != "") {
		_isEditing = true;
	}

	OnLoadSetup();

	$('.action-save').click(function (e) {
		e.preventDefault();

		var currentSection = $('.schedule-panel-body');
		var validation = ValidateSection(currentSection);

		if (validation != null) {
			if (validation == "") {
				return;
			}
			else {
				alert(validation);
				return;
			}
		}

		var emailSubject = $('.schedule-email-subject').val();
		var emailBody = $('#email-body-editor').val();
		var emailAddresses = $('.schedule-email-addresses').val();
		var startOnTime = $('#StartOnTime').val();
		var startOn = $('#StartDateTimePicker').data("StartOnDate") + ' ' + startOnTime.replace(/\./g, "");
		var startOnValue = '';

		if ($("#runNow").hasClass('fa-check-square-o')) {
		    runNow = true;
		}

		if (runNow) {
		    var addMinutes = 2;
		    startOn = new Date().today() + " " + new Date().timeNow(addMinutes);
		}

		startOnValue = parent.Ts.Utils.getMsDate(startOn);

		var frequency = $('.frequencyList').val();
		var every = $('.numberList').val();
		var weekday = (($('.frequencyList').val() == 1) ? $('#weeksOn').val() : $('#dayOfMonth').val());
		var dayOfMonth = $('.monthDayList').val() || 0;
		var isActive = false;
		var runNow = false;

		//3: Once
		if (frequency == 3 || frequency == 4) {
			every = 1;
			weekday = 0;
			dayOfMonth = 0;
		}

		if ($("#active").hasClass('fa-check-square-o')) {
			isActive = true;
		}

		parent.Ts.Services.Reports.SaveScheduledReport(_editingScheduledId || 0,
													_reportIdToSchedule || 0,
													emailSubject,
													emailBody,
													emailAddresses,
													startOnValue,
													frequency,
													every,
													weekday,
													dayOfMonth,
													isActive,
													BackToList(true));
	});

	$('.action-cancel').click(function (e) {
		e.preventDefault();
		BackToList(false);
	});

	$('.frequencyList').change(function (e) {
		e.preventDefault();
		
		switch (this.value)
		{
			case "1":
				SetWeeklyOptions();
				break;
			case "2":
				SetMonthlyOptions();
				break;
			case "3":
				SetOnceOptions();
				break;
		    case "4":
		        SetDailyOptions();
		        break;
		}
	});

	$('.monthDayList').change(function (e) {
		e.preventDefault();
		var onlyDayEntry = this.value > 4;

		PopulateWeekDayList(onlyDayEntry, false);
	});

	$(".schedule-email-subject").blur(function () {
		SetRequiredFields();
	});

	$(".schedule-email-addresses").blur(function () {
		SetRequiredFields();
	});

	$("#StartOnTime").change(function () {
		SetRequiredFields();
	});

	$("#runNow").click(function (e) {
	    e.preventDefault();
	    var check = $(this);

	    if (check.hasClass('fa-square-o')) {
	        check.removeClass('fa-square-o').addClass('fa-check-square-o');
	        $("#StartOn").prop('disabled', 'disabled');
	    } else {
	        check.removeClass('fa-check-square-o').addClass('fa-square-o');
	        $("#StartOn").prop('disabled', false);
	    }
	});

	$("#active").click(function (e) {
		e.preventDefault();
		var check = $(this);

		if (check.hasClass('fa-square-o')) {
			check.removeClass('fa-square-o').addClass('fa-check-square-o');
		} else {
			check.removeClass('fa-check-square-o').addClass('fa-square-o');
		}
	});

	function SetRequiredFields() {
		if ($('.schedule-email-subject').val() == '') {
			$('.schedule-email-subject').addClass('scheduledReport-error');
		} else {
			$('.schedule-email-subject').removeClass('scheduledReport-error');
		}

		if ($('#email-body-editor').val() == '') {
			$('.mce-tinymce.mce-container.mce-panel').addClass('scheduledReport-error');
		} else {
			$('.mce-tinymce.mce-container.mce-panel').removeClass('scheduledReport-error');
		}

		if ($('.schedule-email-addresses').val() == '') {
			$('.schedule-email-addresses').addClass('scheduledReport-error');
		} else {
			$('.schedule-email-addresses').removeClass('scheduledReport-error');
		}

		if ($('#StartOn').val() == '') {
			$('#StartOn').addClass('scheduledReport-error');
		} else {
			$('#StartOn').removeClass('scheduledReport-error');
		}

		if ($('#StartOnTime').val() == '') {
			$('#StartOnTime').addClass('scheduledReport-error');
		} else {
			$('#StartOnTime').removeClass('scheduledReport-error');
		}
	}

	function ValidateSection(el) {
		var isValid = null;
		
		if ($('.schedule-email-subject').val() == '') {
			$('.schedule-email-subject').siblings('span').show();
			isValid = false;
		}
		else {
			$('.schedule-email-subject').siblings('span').hide();
		}

		if ($('#email-body-editor').val() == '') {
			$('.schedule-email-body').children('span').show();
			isValid = false;
		}
		else {
			$('.schedule-email-body').children('span').hide();
		}

		if ($('.schedule-email-addresses').val() == '') {
			$('.schedule-email-addresses').siblings('span').show();
			isValid = false;
		}
		else {
			$('.schedule-email-addresses').siblings('span').hide();
		}
	
		if (($('#StartOn').val() == ''
				|| $('#StartOnTime').val() == '')
			&& !$("#runNow").hasClass('fa-check-square-o')) {
			$('#StartOn').siblings('span').show();
			isValid = false;
		}
		else {
			$('#StartOn').siblings('span').hide();
		}

		return isValid;
	}

	function OnLoadSetup() {
		$('#ReportTitleSpan').text(_reportIdToScheduleName);

		if (_isEditing) {
			parent.Ts.Services.Reports.GetScheduledReport(_editingScheduledId, LoadReportData);
			SetRequiredFields();
		} else {
			SetScheduleOptions(1);

			var dateNow = new Date();
			dateNow.setHours(dateNow.getHours() + 1);

			$('#StartDateTimePicker').datetimepicker({
			    defaultDate: dateNow,
			    pickTime: false,
                orientation: "bottom left"
			});

			initScheduledReportEditor($('#email-body-editor'), function (ed) {
				SetRequiredFields();
				ed.on('keyup', function (e) {
					SetRequiredFields();
				});
			});
		}

		$('#StartOnTime').datetimepicker({
		    pickTime: true,
		    pickDate: false
		});

		PopulateFrequencyList();
	}

	$('#StartOn').focus(function () {
	    $('#StartDateTimePicker').datetimepicker({
	        pickTime: false,
	        orientation: "bottom left"
	    });
        
	    $('#StartDateTimePicker').show();
	    $('#StartDateTimePicker').focus();
	});

	$('#StartDateTimePicker').change(function() {
	    var thisDateTime = this.value;
	    $('#StartOn').val(parent.Ts.Utils.getDateString(thisDateTime, true, false, false));
		$('#StartDateTimePicker').data("StartOnDate", thisDateTime);
		SetRequiredFields();
	});

	$('#StartDateTimePicker').focusout(function() {
	    $('#StartDateTimePicker').hide();
	});

	function LoadReportData(report) {
		SetScheduleOptions(report.RecurrencyId);
		$('.schedule-email-subject').val(report.EmailSubject);
		$('.schedule-email-addresses').val(report.EmailRecipients);
		var startOn = report.StartDate.localeFormat(parent.Ts.Utils.getDatePattern());
		$('#StartOn').val(startOn);
		$('#StartDateTimePicker').data("StartOnDate", report.StartDate.toLocaleString().split(',')[0]);

		var startOnTime = report.StartDate.localeFormat(parent.Ts.Utils.getTimePattern());
		$('#StartOnTime').val(startOnTime);

		$('.frequencyList').val(report.RecurrencyId);
		$('.numberList').val(report.Every);
		$('#weeksOn').val(report.Weekday);
		$('#dayOfMonth').val(report.Weekday || 0);
		$('.monthDayList').val(report.Monthday);

		if (report.IsActive) {
			$("#active").removeClass('fa-square-o').addClass('fa-check-square-o');
		} else {
			$("#active").removeClass('fa-check-square-o').addClass('fa-square-o');
		}

		initScheduledReportEditor($('#email-body-editor'), function (ed) {
		    $('#email-body-editor').tinymce().setContent(report.EmailBody);
		});
	}

	function BackToList(isNew) {
		var returnToOption = "";

		if (isNew) {
			_returnTo = "ScheduledReports";
		}

		if (_returnTo != undefined && _returnTo != null && _returnTo != "") {
			returnToOption = "?ReturnFrom=" + _returnTo;
		}

		var result = '/vcr/1_9_0/pages/';

		function getReportUrl() {
		    switch (_reportTypeOpened) {
		        case 0:
		            return 'Reports_View_Tabular.html?ReportID=' + _reportIdToSchedule;
		        case 1:
		            return 'Reports_View_Chart.html?ReportID=' + _reportIdToSchedule;
		        case 2:
		            return 'Reports_View_External.html?ReportID=' + _reportIdToSchedule;
				  case 3:
				  case 4:
		            return 'Reports_View_Tabular.html?ReportID=' + _reportIdToSchedule;
		        case 5:
		            return 'TicketView.html?ReportID=' + _reportIdToSchedule;
		    }
		}

		if (_reportTypeOpened != undefined && _reportTypeOpened != null && _reportTypeOpened != "undefined") // go back to report
		{
		    _reportTypeOpened = parseInt(_reportTypeOpened);
		    result = result + getReportUrl();
		} else { // go back to list
		    result = result + 'reports.html' + returnToOption;
		}

		location.assign(result);
	};

	function SetScheduleOptions(frequency) {
		switch (frequency) {
			case 1:
				SetWeeklyOptions();
				break;
			case 2:
				SetMonthlyOptions();
				break;
			case 3:
				SetOnceOptions();
				break;
		    case 4:
		        SetDailyOptions();
		        break;
		}
	}

	function SetWeeklyOptions() {
	    $('#RunNowOption').hide();
		$('#Every').show();
		$('#WeeklyOptions').show();
		$('#MonthlyOptions').hide();
		$('#WeekdayList').hide();
		PopulateWeekDayList(false, true);
	}

	function SetMonthlyOptions() {
	    $('#RunNowOption').hide();
		$('#Every').show();
		$('#WeeklyOptions').hide();
		$('#MonthlyOptions').show();
		$('#WeekdayList').show();
		PopulateWeekDayList(false, false);
		PopulateMonthDayList();
	}

	function SetOnceOptions() {
	    $('#RunNowOption').show();
		$('#Every').hide();
		$('#WeeklyOptions').hide();
		$('#MonthlyOptions').hide();
		$('#WeekdayList').hide();
	}

	function SetDailyOptions() {
	    $('#RunNowOption').hide();
	    $('#Every').hide();
	    $('#WeeklyOptions').hide();
	    $('#MonthlyOptions').hide();
	    $('#WeekdayList').hide();
	}

	function PopulateFrequencyList() {
		$('.frequencyList').empty();
		var selectValues = { 3: "Once", 4: "Daily", 1: "Weekly", 2: "Monthly" };

		$.each(selectValues, function (key, value) {
			$('.frequencyList')
				.append($("<option></option>")
						   .attr("value", key)
						   .text(value));
		});
	}

	function PopulateWeekDayList(onlyDayEntry, onlyWeekDays) {
		$('.weekdayList').empty();
		var includeDivider = true;

		var selectValues = {
			"0": "Day",
			"1": "Sunday",
			"2": "Monday",
			"3": "Tuesday",
			"4": "Wednesday",
			"5": "Thursday",
			"6": "Friday",
			"7": "Saturday"
		};

		if (onlyDayEntry) {
			selectValues = {
				"0": "Day"
			};

			includeDivider = false;
		}

		if (onlyWeekDays) {
			selectValues = {
				"1": "Sunday",
				"2": "Monday",
				"3": "Tuesday",
				"4": "Wednesday",
				"5": "Thursday",
				"6": "Friday",
				"7": "Saturday"
			};

			includeDivider = false;
		}

		$.each(selectValues, function (key, value) {
			if (includeDivider && key == 1) {
				$('.weekdayList')
							.append($("<option disabled>──────────</option>"));
			}

			$('.weekdayList')
							.append($("<option></option>")
									   .attr("value", key)
									   .text(value));
		});
	}

	function PopulateMonthDayList() {
		$('.monthDayList').empty();
		var selectValues = {
			"1": "1st",
			"2": "2nd",
			"3": "3rd",
			"4": "4th",
			"5": "5th",
			"6": "6th",
			"7": "7th",
			"8": "8th",
			"9": "9th",
			"10": "10th",
			"11": "11th",
			"12": "12th",
			"13": "13th",
			"14": "14th",
			"15": "15th",
			"16": "16th",
			"17": "17th",
			"18": "18th",
			"19": "19th",
			"20": "20th",
			"21": "21st",
			"22": "22nd",
			"23": "23rd",
			"24": "24th",
			"25": "25th",
			"26": "26th",
			"27": "27th",
			"28": "28th",
			"29": "29th",
			"30": "30th",
			"31": "31st",
			"99": "Last"
		};

		$.each(selectValues, function (key, value) {
			$('.monthDayList')
				.append($("<option></option>")
						   .attr("value", key)
						   .text(value));
		});
	}

	Date.prototype.today = function () {
	    return (((this.getMonth() + 1) < 10) ? "0" : "") + (this.getMonth() + 1) + "/" + ((this.getDate() < 10) ? "0" : "") + this.getDate() + "/" + this.getFullYear();
	}

	Date.prototype.timeNow = function (addMinutes) {
	    if (addMinutes == undefined) {
	        addMinutes = 0;
	    }

	    var minutes = this.getMinutes();
	    minutes = minutes + addMinutes;

	    return ((this.getHours() < 10) ? "0" : "") + this.getHours() + ":" + ((minutes < 10) ? "0" : "") + minutes + ":" + ((this.getSeconds() < 10) ? "0" : "") + this.getSeconds();
	}
});