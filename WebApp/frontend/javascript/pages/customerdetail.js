/// <reference path="ts/ts.js" />
/// <reference path="ts/_mainFrame.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

var customerDetailPage = null;
var organizationID = null;
var ratingFilter = '';
var _companyName = '';
var _isUnknown = false;
var _execGetAsset = null;
var _execGetCustomer = null;
var _productsSortColumn = 'Date Created';
var _productsSortDirection = 'DESC';
var _slaSortColumn = 'DateViolated';
var _slaSortDirection = 'DESC';
var _productHeadersAdded = false;
var _isParentView = false;
var _isLoadingContacts = false;
var _viewingContacts = false;
var _orgParentId;
var _completeCommentTaskID = 0;
var _dateFormat;

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

Selectize.define('sticky_placeholder', function (options) {
    var self = this;

    self.updatePlaceholder = (function () {
        var original = self.updatePlaceholder;
        return function () {
            original.apply(this, arguments);
            if (!this.settings.placeholder) return;
            var $input = this.$control_input;
            $input.attr('placeholder', this.settings.placeholder);
        };
    })();

});

Selectize.define('no_results', function (options) {
    var self = this;

    options = $
              .extend({
                  message: 'No results found.', html: function (data) {
                      return ('<div class="selectize-dropdown ' + data.classNames + ' dropdown-empty-message">' + '<div class="selectize-dropdown-content" style="padding: 3px 12px">' + data.message + '</div>' + '</div>');
                  }
              }, options);

    self.displayEmptyResultsMessage = function () {
        this.$empty_results_container.css('top', this.$control.outerHeight());
        this.$empty_results_container.show();
    };

    self.refreshOptions = (function () {
        var original = self.refreshOptions;

        return function () {
            original.apply(self, arguments);
            this.hasOptions ? this.$empty_results_container.hide() :
                this.displayEmptyResultsMessage();
        }
    })();

    self.onKeyDown = (function () {
        var original = self.onKeyDown;

        return function (e) {
            original.apply(self, arguments);
            if (e.keyCode === 27) {
                this.$empty_results_container.hide();
            }
        }
    })();

    self.onBlur = (function () {
        var original = self.onBlur;

        return function () {
            original.apply(self, arguments);
            this.$empty_results_container.hide();
        };
    })();

    self.setup = (function () {
        var original = self.setup;
        return function () {
            original.apply(self, arguments);
            self.$empty_results_container = $(options.html($.extend({
                classNames: self.$input.attr('class')
            }, options)));
            self.$empty_results_container.insertBefore(self.$dropdown);
            self.$empty_results_container.hide();
        };
    })();
});

$(document).ready(function () {

    customerDetailPage = new CustomerDetailPage();
    customerDetailPage.refresh();
    $('.customer-tooltip').tooltip({ placement: 'bottom', container: 'body' });


    parent.Ts.Services.Customers.GetDateFormat(false, function (dateformat) {
        $('.datepicker').attr("data-format", dateformat);
        $('.datetimepicker').attr("data-format", dateformat + " hh:mm a");
        _dateFormat = dateformat;
        $('.timepicker').datetimepicker({ pickDate: false });
        $('.datetimepicker').datetimepicker({});
        $('.datepicker').datetimepicker({ pickTime: false });
        //$('#inputExpectedRelease').datetimepicker({ pickTime: false, format: dateformat });
    });

    $('input, textarea').placeholder();
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

    organizationID = _mainFrame.Ts.Utils.getQueryValue("organizationid", window);
    _isParentView = _mainFrame.Ts.Utils.getQueryValue("parentView", window);

    _mainFrame.Ts.Services.Customers.CanAccessCustomer(organizationID, function (result) {
        if (!result) {
            var url = window.location.href;
            if (url.indexOf('.') > -1) {
                url = url.substring(0, url.lastIndexOf('/') + 1);
            }
            window.location = url + 'NoCustomerAccess.html';
            return;
        }

    });

    if (_isParentView) {
        _isParentView = true;
        $('#customerParentView').hide();
        $('#customerNormalView').show();

        $('#customerEdit').hide();
        $('#Company-Merge').hide();
        $('#customerSubscribe').hide();
        $('#customerReminder').hide();
        $('#customerDelete').hide();

        $('.contact-action-add').hide();
        //$('.tickets-new').hide();
        $('#noteToggle').hide();
        $('#fileToggle').hide();
        $('#productToggle').hide();
        $('.asset-action-assign').hide();
        $('#companyTabs a[href="#company-children"]').show();
        $('#companyTabs a[href="#company-watercooler"]').hide();
        $('#companyTabs a[href="#company-ratings"]').hide();
        $('#companyTabs a[href="#company-calendar"]').hide();
    }
    else {
        _isParentView = false;

        if (_mainFrame.Ts.System.Organization.ProductType == _mainFrame.Ts.ProductType.Enterprise) {
            $('#customerReminder').hide();
        }
    }
    noteID = _mainFrame.Ts.Utils.getQueryValue("noteid", window);
    var _isAdmin = _mainFrame.Ts.System.User.IsSystemAdmin && (organizationID != _mainFrame.Ts.System.User.OrganizationID);
    var historyLoaded = 0;
    parent.privateServices.SetUserSetting('SelectedOrganizationID', organizationID);
    parent.privateServices.SetUserSetting('SelectedContactID', -1);

    if (_mainFrame.Ts.System.Organization.UseProductFamilies) {
        LoadProductFamilies();
        $('.productFamilyRow, .productFamilyColumn, .productLineRow').show();
    }

    if (_mainFrame.Ts.System.Organization.ProductType == _mainFrame.Ts.ProductType.Enterprise) {
        $('#taskTab').show();
    }

    LoadNoteActivities();
    LoadNotes();
    LoadNotesAdditional();
    //LoadHistory();
    LoadFiles();
    //LoadRatings();
    LoadPhoneTypes();
    LoadPhoneNumbers();
    LoadAddresses();
    LoadProperties();
    LoadCustomProperties();
    //LoadContacts();
    LoadProductTypes();
    LoadSlaLevels();
    LoadCustomControls(_mainFrame.Ts.ReferenceTypes.OrganizationProducts);
    LoadReminderUsers();
    UpdateRecentView();

    _mainFrame.Ts.Services.Customers.LoadAlerts(organizationID, _mainFrame.Ts.ReferenceTypes.Organizations, function (notes) {
        for (var i = 0; i < notes.length; i++) {
            var note = notes[i];
            var description = $('<div>').html(note.Description);
            //$('#modalAlert').modal('show');
            var buttons = [
                {
                    text:"Close",
                    click:function () {
                        $(this).dialog("close");
                    }
                },
                {
                    text:"Snooze",
                    click:function () {
                        _mainFrame.Ts.Services.Customers.SnoozeAlertByID($(this).data('noteId'), _mainFrame.Ts.ReferenceTypes.Organizations);
                        $(this).dialog("close");
                    }
                }
            ]

            if (!_mainFrame.Ts.System.Organization.HideDismissNonAdmins || _mainFrame.Ts.System.User.IsSystemAdmin) {
                buttons.push({
                    text:"Dismiss",
                    click:function () {
                        _mainFrame.Ts.Services.Customers.DismissAlertByID($(this).data('noteId'), _mainFrame.Ts.ReferenceTypes.Organizations);
                        $(this).dialog("close");
                    }
                });
            }

            var alert = $('<div>').prop('title', 'Alert message').data('noteId', note.NoteID).append(description).appendTo(document.body);
            alert.dialog({
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


    if (!_mainFrame.Ts.System.User.CanEditCompany && !_isAdmin) {
        $('#productToggle').hide();
    }

    if (!_mainFrame.Ts.System.Organization.IsInventoryEnabled) {
        $('#companyTabs a[href="#company-products"]').hide();
        $('#companyTabs a[href="#company-inventory"]').hide();
    }
    $(".maincontainer").on("keypress", "input", (function (evt) {
        //Deterime where our character code is coming from within the event
        var charCode = evt.charCode || evt.keyCode;
        if (charCode == 13) { //Enter key's keycode
            return false;
        }
    }));

    $('#historyToggle').on('click', function () {
        _mainFrame.Ts.System.logAction('Customer Detail - History Toggle');
        if (historyLoaded == 0) {
            historyLoaded = 1;
            LoadHistory(1);
        }
    });

    $('#historyRefresh').on('click', function () {
        _mainFrame.Ts.System.logAction('Customer Detail - History Refresh');
        LoadHistory(1);
    });

    if (noteID != null) {
        $('#companyTabs a:first').tab('show');
        $('#companyTabs a[href="#company-notes"]').tab('show');
        openNote(noteID);
    }
    else {
        $('#companyTabs a:first').tab('show');
    }

    if (!_isAdmin && !_mainFrame.Ts.System.User.CanEditCompany) {
        $('#fieldActive').removeClass('editable');
        $('#groupAPI').hide();
        $('#customerEdit').hide();
        $('#customerPhoneBtn').hide();
        $('#customerAddressBtn').hide();
        $('#fileToggle').hide();

        $('#noteToggle').hide();
        $('#productToggle').hide();
        $('.asset-action-assign').hide();
        $(".task-action-add").hide();
    }

    if (!_isAdmin) {
        $('#customerDelete').hide();
        $('#Company-Merge').hide();
    }

    if (!_isAdmin && !_mainFrame.Ts.System.User.CanCreateContact) {
        $('.contact-action-add').hide();
    }


    if (_mainFrame.Ts.System.User.OrganizationID == organizationID) {
        $('#groupSupportUser').hide();
        $('#groupSupportGroup').hide();
    } else {
        $('#groupTimezone').hide();
        $('#groupPortalGroup').hide();
    }

    $('#customerEdit').click(function (e) {
        _mainFrame.Ts.System.logAction('Customer Detail - Customer Edit');
        if (_isUnknown) {
            $('#fieldActive').toggleClass("editable");
            $('#fieldPortalAccess').toggleClass("editable");
        }
        else {
            $('.userProperties p').toggleClass("editable");
            $('.customProperties p').toggleClass("editable");
            $("#phonePanel #editmenu").toggleClass("hiddenmenu");
            $("#addressPanel #editmenu").toggleClass("hiddenmenu");
        }

        if ($('#fieldWebsite').text() != "Empty")
            $(".userProperties #fieldWebsite").toggleClass("link");

        $(this).toggleClass("btn-primary");
        $(this).toggleClass("btn-success");
        if ($(this).hasClass("btn-primary"))
            $(this).html('<i class="fa fa-pencil"></i> Edit');
        else
            $(this).html('<i class="fa fa-pencil"></i> Save');

        $('#companyTabs a:first').tab('show');
        if ((!_isAdmin && !_mainFrame.Ts.System.User.IsPortalUser) || (!_mainFrame.Ts.System.User.CanEditCompany && !_isAdmin)) {
            $('#fieldPortalAccess').removeClass('editable');
        }

    });

    $('#fieldName').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;

        _mainFrame.Ts.System.logAction('Customer Detail - Edit Name');
        var header = $(this).hide();
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        $('<input type="text">')
          .addClass('col-xs-10 form-control')
          .val($(this).text())
          .appendTo(container1)
          .focus();

        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
              _mainFrame.Ts.System.logAction('Customer Detail - Save Name Edit');
              _mainFrame.Ts.Services.Customers.SetCompanyName(organizationID, $(this).prev().find('input').val(), function (result) {
                  header.text(result);
                  $('#companyName').text(result);
                  $('#customerEdit').removeClass("disabled");
              },
                            function (error) {
                                header.show();
                                alert('There was an error saving the company name.');
                                $('#customerEdit').removeClass("disabled");
                            });
              $('#customerEdit').removeClass("disabled");
              $(this).closest('div').remove();
              header.show();
          })
          .insertAfter(container1);
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldWebsite').click(function (e) {
        if ($(this).hasClass('link')) {
            if ($('#fieldWebsite').text().toLowerCase().lastIndexOf('http://', 0) === 0 || $('#fieldWebsite').text().toLowerCase().lastIndexOf('https://', 0) === 0)
                window.open($('#fieldWebsite').text(), '_blank');
            else
                window.open('http://' + $('#fieldWebsite').text(), '_blank');


            return;
        }
        else {
            e.preventDefault();
            if (!$(this).hasClass('editable'))
                return false;
            var header = $(this).hide();
            var container = $('<div>')
              .insertAfter(header);
            _mainFrame.Ts.System.logAction('Customer Detail - Edit Website');
            var container1 = $('<div>')
                .addClass('col-xs-9')
              .appendTo(container);

            $('<input type="text">')
              .addClass('col-xs-10 form-control')
              .val($(this).text() == "Empty" ? "" : $(this).text())
              .appendTo(container1)
              .focus();

            $('<i>')
              .addClass('col-xs-1 fa fa-times')
              .click(function (e) {
                  $(this).closest('div').remove();
                  header.show();
                  $('#customerEdit').removeClass("disabled");
              })
              .insertAfter(container1);
            $('<i>')
              .addClass('col-xs-1 fa fa-check')
              .click(function (e) {
                  _mainFrame.Ts.System.logAction('Customer Detail - Save Website Edit');
                  _mainFrame.Ts.Services.Customers.SetCompanyWeb(organizationID, $(this).prev().find('input').val(), function (result) {
                      header.text(result);
                      if ($('#fieldWebsite').text() == "Empty")
                          $('#fieldWebsite').removeClass("link");
                      $('#customerEdit').removeClass("disabled");
                  },
                                function (error) {
                                    header.show();
                                    alert('There was an error saving the company website.');
                                    $('#customerEdit').removeClass("disabled");
                                });
                  $(this).closest('div').remove();
                  header.show();
              })
              .insertAfter(container1);
            $('#customerEdit').addClass("disabled");
        }
    });

    $('#fieldDomains').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        var container = $('<div>')
          .insertAfter(header);
        _mainFrame.Ts.System.logAction('Customer Detail - Edit Domain');
        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        $('<input type="text">')
          .addClass('col-xs-10 form-control')
          .val($(this).text() == "Empty" ? "" : $(this).text())
          .appendTo(container1)
          .focus();

        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
              _mainFrame.Ts.System.logAction('Customer Detail - Save Domain Edit');
              _mainFrame.Ts.Services.Customers.SetCompanyDomain(organizationID, $(this).prev().find('input').val(), function (result) {
                  header.text(result);
                  $('#customerEdit').removeClass("disabled");
              },
                            function (error) {
                                header.show();
                                alert('There was an error saving the company domain.');
                                $('#customerEdit').removeClass("disabled");
                            });
              $(this).closest('div').remove();
              header.show();
          })
          .insertAfter(container1);
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldSupportHours').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        var container = $('<div>')
          .insertAfter(header);
        _mainFrame.Ts.System.logAction('Customer Detail - Edit Support Hours');
        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        $('<input type="text">')
          .addClass('col-xs-10 form-control number')
          .val($(this).text())
          .appendTo(container1)
          .focus();

        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
              var value = $(this).prev().find('input').val();
              _mainFrame.Ts.System.logAction('Customer Detail - Save Support Hours Edit');
              _mainFrame.Ts.Services.Customers.SetCompanySupportHours(organizationID, value != "" ? value : 0, function (result) {
                  header.text(result);
                  $('#customerEdit').removeClass("disabled");
              },
                            function (error) {
                                header.show();
                                alert('There was an error saving the company support hours.');
                                $('#customerEdit').removeClass("disabled");
                            });
              $(this).closest('div').remove();
              header.show();
          })
          .insertAfter(container1);
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldDescription').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        var container = $('<div>')
          .insertAfter(header);
        _mainFrame.Ts.System.logAction('Customer Detail - Edit Description');
        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        $('<textarea>')
          .addClass('col-xs-10 form-control')
          .val($(this).text() == "Empty" ? "" : $(this).html().replace(/<br\s?\/?>/g, "\n"))
          .appendTo(container1)
          .focus();

        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
              _mainFrame.Ts.System.logAction('Customer Detail - Save Description Edit');
              _mainFrame.Ts.Services.Customers.SetCompanyDescription(organizationID, $(this).prev().find('textarea').val(), function (result) {
                  header.text(result);
                  $('#customerEdit').removeClass("disabled");
              },
                            function (error) {
                                header.show();
                                alert('There was an error saving the company description.');
                                $('#customerEdit').removeClass("disabled");
                            });
              $(this).closest('div').remove();
              header.show();
          })
          .insertAfter(container1);
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldInactive').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        var container = $('<div>')
          .insertAfter(header);
        _mainFrame.Ts.System.logAction('Customer Detail - Edit Inactive');
        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        $('<textarea>')
          .addClass('col-xs-10 form-control')
          .val($(this).html().replace(/<br\s?\/?>/g, "\n"))
          .appendTo(container1)
          .focus();

        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
              _mainFrame.Ts.System.logAction('Customer Detail - Save Company Inactive Edit');
              _mainFrame.Ts.Services.Customers.SetCompanyInactive(organizationID, $(this).prev().find('textarea').val(), function (result) {
                  header.text(result);
                  $('#customerEdit').removeClass("disabled");
              },
                            function (error) {
                                header.show();
                                alert('There was an error saving the company inactive reason.');
                                $('#customerEdit').removeClass("disabled");
                            });
              $(this).closest('div').remove();
              header.show();
          })
          .insertAfter(container1);
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldSAED').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        var container = $('<div>')
          .insertAfter(header);
        _mainFrame.Ts.System.logAction('Customer Detail - Edit Service Agreement Expiration Date');
        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        var theinput = $('<input type="text">')
          .addClass('col-xs-10 form-control')
          .val($(this).text() == "[None]" ? "" : $(this).text())
            .datetimepicker({ pickTime: false, format: _dateFormat })
          .appendTo(container1)
          .focus();


        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);


        $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
              //var value = _mainFrame.Ts.Utils.getMsDate($(this).prev().find('input').val());
              _mainFrame.Ts.System.logAction('Customer Detail - Save Service Agreement Expiration Date Edit');

              var convertedDate = convertToValidDate($(this).prev().find('input').val())
              var currentdate = $(this).prev().find('input').val();
              _mainFrame.Ts.Services.Customers.SetCompanySAE(organizationID, convertedDate, function (result) {
                  //var date = result === null ? null : _mainFrame.Ts.Utils.getMsDate(result);
                  header.text(result == "" ? "[None]" : currentdate);
                  $('#customerEdit').removeClass("disabled");
              },
                            function (error) {
                                header.show();
                                alert('There was an error saving the company Service Agreement Expiration Date.');
                                $('#customerEdit').removeClass("disabled");
                            });
              $(this).closest('div').remove();
              header.show();
          })
          .insertAfter(container1);
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldPrimaryContact').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        _mainFrame.Ts.System.logAction('Customer Detail - Edit Primary Contact');
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        var select = $('<select>').addClass('form-control').attr('id', 'ddlPrimaryContact').appendTo(container1);
        _mainFrame.Ts.Services.Customers.LoadOrgUsers(organizationID, function (contacts) {
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
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('#ddlPrimaryContact').on('change', function () {
            var value = $(this).val();
            var name = this.options[this.selectedIndex].innerHTML;
            container.remove();
            _mainFrame.Ts.System.logAction('Customer Detail - Save Primary Contact Edit');
            _mainFrame.Ts.Services.Customers.SetCompanyPrimaryContact(organizationID, value, function (result) {
                header.data('field', result);
                header.text(name);
                header.show();
                $('#customerEdit').removeClass("disabled");
            }, function () {
                alert("There was a problem saving your company property.");
                $('#customerEdit').removeClass("disabled");
            });
        });
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldParentCompany').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        _mainFrame.Ts.System.logAction('Customer Detail - Edit Parent Company');
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        var select = $('<select>').addClass('form-control').attr('id', 'ddlParentCompany').appendTo(container1);
        _mainFrame.Ts.Services.Customers.LoadOrgUsers(organizationID, function (contacts) {
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
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('#ddlPrimaryContact').on('change', function () {
            var value = $(this).val();
            var name = this.options[this.selectedIndex].innerHTML;
            container.remove();
            _mainFrame.Ts.System.logAction('Customer Detail - Save Primary Contact Edit');
            _mainFrame.Ts.Services.Customers.SetCompanyPrimaryContact(organizationID, value, function (result) {
                header.data('field', result);
                header.text(name);
                header.show();
                $('#customerEdit').removeClass("disabled");
            }, function () {
                alert("There was a problem saving your company property.");
                $('#customerEdit').removeClass("disabled");
            });
        });
        $('#customerEdit').addClass("disabled");
    });


    $('#fieldDefaultUser').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        _mainFrame.Ts.System.logAction('Customer Detail - Edit Default User');
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        var select = $('<select>').addClass('form-control').attr('id', 'ddlPrimaryDefaultUser').appendTo(container1);
        _mainFrame.Ts.Services.Customers.LoadOrgSupportUsers(organizationID, function (contacts) {
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
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('#ddlPrimaryDefaultUser').on('change', function () {
            var value = $(this).val();
            var name = this.options[this.selectedIndex].innerHTML;
            container.remove();
            _mainFrame.Ts.System.logAction('Customer Detail - Save Default User Edit');
            _mainFrame.Ts.Services.Customers.SetCompanyDefaultSupportUser(organizationID, value, function (result) {
                header.data('field', result);
                header.text(name);
                header.show();
                $('#customerEdit').removeClass("disabled");
            }, function () {
                alert("There was a problem saving your company property.");
                $('#customerEdit').removeClass("disabled");
            });
        });
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldTimeZone').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        _mainFrame.Ts.System.logAction('Customer Detail - Edit Timezone');
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        var select = $('<select>').addClass('form-control').attr('id', 'ddlTimezone').appendTo(container1);
        _mainFrame.Ts.Services.Customers.LoadTimeZones(function (timeZones) {
            for (var i = 0; i < timeZones.length; i++) {
                var opt = $('<option>').attr('value', timeZones[i].Id).text(timeZones[i].DisplayName).data('o', timeZones[i]).appendTo(select);
                if (header.data('field') == timeZones[i].Id)
                    opt.attr('selected', 'selected');
            }
        });

        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('#ddlTimezone').on('change', function () {
            var value = $(this).val();
            var name = this.options[this.selectedIndex].innerHTML;
            container.remove();

            _mainFrame.Ts.System.logAction('Customer Detail - Save Timezone Edit');
            _mainFrame.Ts.Services.Customers.SetCompanyTimezone(organizationID, value, function (result) {
                header.data('field', result);
                header.text(result);
                header.show();
                $('#customerEdit').removeClass("disabled");
            }, function () {
                alert("There was a problem saving your company timezone.");
                $('#customerEdit').removeClass("disabled");
            });
        });
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldDefaultGroup').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        _mainFrame.Ts.System.logAction('Customer Detail - Edit Default Group');
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        var select = $('<select>').addClass('form-control').attr('id', 'ddlfieldDefaultGroup').appendTo(container1);
        _mainFrame.Ts.Services.Customers.LoadOrgGroups(organizationID, function (contacts) {
            $('<option>').attr('value', '-1').text('Unassigned').appendTo(select);
            for (var i = 0; i < contacts.length; i++) {
                var opt = $('<option>').attr('value', contacts[i].GroupID).text(contacts[i].Name).data('o', contacts[i]);
                if (header.data('field') == contacts[i].GroupID)
                    opt.attr('selected', 'selected');
                opt.appendTo(select);
            }
        });


        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('#ddlfieldDefaultGroup').on('change', function () {
            var value = $(this).val();
            var name = this.options[this.selectedIndex].innerHTML;
            container.remove();
            _mainFrame.Ts.System.logAction('Customer Detail - Save Default Group Edit');
            _mainFrame.Ts.Services.Customers.SetCompanyDefaultSupportGroup(organizationID, value, function (result) {
                header.data('field', result);
                header.text(name);
                header.show();
                $('#customerEdit').removeClass("disabled");
            }, function () {
                alert("There was a problem saving your company property.");
                $('#customerEdit').removeClass("disabled");
            });
        });
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldDefaultPortalGroup').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        _mainFrame.Ts.System.logAction('Customer Detail - Edit Default Portal Group');
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        var select = $('<select>').addClass('form-control').attr('id', 'ddlfieldDefaultPortalGroup').appendTo(container1);
        _mainFrame.Ts.Services.Customers.LoadOrgGroups(organizationID, function (contacts) {
            $('<option>').attr('value', '-1').text('Unassigned').appendTo(select);
            for (var i = 0; i < contacts.length; i++) {
                var opt = $('<option>').attr('value', contacts[i].GroupID).text(contacts[i].Name).data('o', contacts[i]);
                if (header.data('field') == contacts[i].GroupID)
                    opt.attr('selected', 'selected');
                opt.appendTo(select);
            }
        });


        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('#ddlfieldDefaultPortalGroup').on('change', function () {
            var value = $(this).val();
            var name = this.options[this.selectedIndex].innerHTML;
            container.remove();
            _mainFrame.Ts.System.logAction('Customer Detail - Save Default Portal Group Edit');
            _mainFrame.Ts.Services.Customers.SetCompanyDefaultPortalGroup(organizationID, value, function (result) {
                header.data('field', result);
                header.text(name);
                header.show();
                $('#customerEdit').removeClass("disabled");
            }, function () {
                alert("There was a problem saving your company property.");
                $('#customerEdit').removeClass("disabled");
            });
        });
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldSLA').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        _mainFrame.Ts.System.logAction('Customer Detail - Edit SLA');
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        var select = $('<select>').addClass('form-control').attr('id', 'ddlfieldSLA').appendTo(container1);
        _mainFrame.Ts.Services.Customers.LoadOrgSlas(function (contacts) {
            $('<option>').attr('value', '-1').text('Unassigned').appendTo(select);
            for (var i = 0; i < contacts.length; i++) {
                var opt = $('<option>').attr('value', contacts[i].SlaLevelID).text(contacts[i].Name).data('o', contacts[i]);
                if (header.data('field') == contacts[i].SlaLevelID)
                    opt.attr('selected', 'selected');
                opt.appendTo(select);
            }
        });


        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('#ddlfieldSLA').on('change', function () {
            var value = $(this).val();
            var name = this.options[this.selectedIndex].innerHTML;
            container.remove();
            _mainFrame.Ts.System.logAction('Customer Detail - Save SLA Edit');
            _mainFrame.Ts.Services.Customers.SetCompanySLA(organizationID, value, function (result) {
                header.data('field', result);
                header.text(name);
                header.show();
                $('#customerEdit').removeClass("disabled");
            }, function () {
                alert("There was a problem saving your company property.");
                $('#customerEdit').removeClass("disabled");
            });
        });
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldActive').click(function (e) {
        if (!$(this).hasClass('editable'))
            return false;

		var isActive = $(this).text() !== 'true';
		var hasChildren = false;

		_mainFrame.Ts.Services.Customers.HasChildren(organizationID, function (result) {
			hasChildren = result === true;

			if (hasChildren && !isActive) {
				var confirmInactiveChildren = $('<div>').prop('title', 'Confirm Inactivate Children').append('This customer has children. Do you want all of them to be set to inactive too?').appendTo(document.body);
				confirmInactiveChildren.dialog({
					resizable: false,
					width: 'auto',
					height: 'auto',
					create: function () {
						$(this).css('maxWidth', '800px');
					},
					modal: true,
					buttons: {
						"Yes": function () {
							SetCompanyAndChildrenInactive(organizationID, hasChildren, isActive, true);
							$(this).dialog('close');
						},
						"No": function () {
							SetCompanyActive(organizationID, isActive);
							$(this).dialog('close');
				}
					},
					close: function (event, ui) {
			}
				});
			} else {
				SetCompanyActive(organizationID, isActive);
			}
		});
	});

	function SetCompanyAndChildrenInactive(organizationID, hasChildren, isActive, inactiveChildren) {
			if (hasChildren && !isActive && inactiveChildren) {
				_mainFrame.Ts.Services.Customers.SetCompanyAndChildrenInactive(organizationID, function () {
					_mainFrame.Ts.System.logAction('Customer Detail - Toggle Active State And Set Children Inactive');
					$('#fieldActive').text('false');
				},
					function (error) {
					alert('There was an error saving the customer active and children.');
					});
		}
	}

	function SetCompanyActive(organizationID, isActive) {
				_mainFrame.Ts.Services.Customers.SetCompanyActive(organizationID, isActive, function (result) {
            _mainFrame.Ts.System.logAction('Customer Detail - Toggle Active State');
            $('#fieldActive').text((result === true ? 'true' : 'false'));
        },
        function (error) {
            header.show();
            alert('There was an error saving the customer active.');
        });
			}

    $('#fieldAPIEnabled').click(function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        _mainFrame.Ts.Services.Customers.SetCompanyAPIEnabled(organizationID, ($(this).text() !== 'true'), function (result) {
            _mainFrame.Ts.System.logAction('Customer Detail - Toggle Enabled State');
            $('#fieldAPIEnabled').text((result === true ? 'true' : 'false'));
        },
        function (error) {
            header.show();
            alert('There was an error saving the customer active.');
        });
    });

    $('#fieldPortalAccess').click(function (e) {
        if (!$(this).hasClass('editable') || (!_isAdmin && !_mainFrame.Ts.System.User.IsPortalUser) || !_mainFrame.Ts.System.User.CanEditCompany)
            return false;
        _mainFrame.Ts.Services.Customers.SetCompanyPortalAccess(organizationID, ($(this).text() !== 'true'), function (result) {
            _mainFrame.Ts.System.logAction('Customer Detail - Toggle Portal Access State');
            $('#fieldPortalAccess').text((result === true ? 'true' : 'false'));
        },
        function (error) {
            header.show();
            alert('There was an error saving the customer portal access.');
        });
    });

    $('#productToggle').click(function (e) {
        _mainFrame.Ts.System.logAction('Customer Detail - Toggle Product Form');
        $('#productForm').toggle();
    });

    $('#noteToggle').click(function (e) {
        _mainFrame.Ts.System.logAction('Customer Detail - Toggle Note Form');
        if ($('#noteForm:visible').length == 0)
        {
            initEditor($('#fieldNoteDesc'), function (ed) {
                $('#activityDate').data("DateTimePicker").setDate(new Date());
            });
        }
        $('#noteForm').toggle();
        $('#fieldNoteTitle').focus();
    });

    $('#fileToggle').click(function (e) {
        _mainFrame.Ts.System.logAction('Customer Detail - Toggle File Form');
        $('#fileForm').toggle();
    });

    $('#productCustomer').val(organizationID);

    _mainFrame.Ts.Services.Organizations.GetOrganization(organizationID, function (org) {
        _companyName = org.Name;
        if (_isParentView) {
            $('#companyName').text(org.Name + ' (Parent View)');
        }
        else {
            $('#companyName').text(org.Name);
        }

        var hasCustomerInsights = _mainFrame.Ts.System.Organization.IsCustomerInsightsActive;

        if (hasCustomerInsights) {
            var companyLogoPath = "../../../dc/" + org.ParentID + "/companylogo/" + organizationID + "/80";
            $('#companyLogo').attr("src", companyLogoPath);

            $("#companyLogo").error(function () {
                $(this).hide();
            })
        }
        else {
            $('#companyLogo').hide();
        }

        _orgParentId = org.ParentID;
    });

    $("input[type=text], textarea").autoGrow();

    $.valHooks.textarea = {
        get: function (elem) {
            return elem.value.replace(/\r\n|\r|\n/g, "<br />");
        }
    };

    //$('#companyTabs a:first').tab('show');

    $('.contact-action-add').click(function (e) {
        e.preventDefault();
        _mainFrame.Ts.MainPage.newCustomer("customer", organizationID);
    });

    $('#productProduct').change(function () {
        LoadProductVersions($(this).val(), -1);
    });

    $('#btnProductSave').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        _mainFrame.Ts.System.logAction('Customer Detail - Save Product');
        var productInfo = new Object();
        var hasError = 0;
        productInfo.OrganizationID = $("#productCustomer").val();
        productInfo.ProductID = $("#productProduct").val();
        productInfo.Version = $("#productVersion").val();
        productInfo.SupportExpiration = $("#productExpiration").val();
        productInfo.OrganizationProductID = $('#fieldProductID').val();
        productInfo.SlaLevelID = $('#slaLevel').val();

        productInfo.Fields = new Array();
        $('.customField:visible').each(function () {
            var field = new Object();
            field.CustomFieldID = $(this).attr("id");

            if ($(this).hasClass("required") && ($(this).val() === null || $.trim($(this).val()) === '')) {
                $(this).parent().addClass('has-error');
                hasError = 1;
            }
            else {
                $(this).parent().removeClass('has-error');
            }

            switch ($(this).attr("type")) {
                case "checkbox":
                    field.Value = $(this).prop('checked');
                    break;
                    //case "date":
                    //    field.Value = $(this).val() == "" ? null : _mainFrame.Ts.Utils.getMsDate($(this).val());
                    //    break;
                    //case "time":
                    //    field.Value = $(this).val() == "" ? null : _mainFrame.Ts.Utils.getMsDate("1/1/1900 " + $(this).val());
                    //    break;
                    //case "datetime":
                    //    field.Value = $(this).val() == "" ? null : _mainFrame.Ts.Utils.getMsDate($(this).val());
                    //    break;
                default:
                    field.Value = $(this).val();
            }
            productInfo.Fields[productInfo.Fields.length] = field;
        });

        if (hasError == 0) {
            _mainFrame.Ts.Services.Customers.SaveProduct(parent.JSON.stringify(productInfo), function (prod) {
                LoadProducts();
                $('#btnProductSave').text("Save Product");
                $('#productExpiration').val('');
                $('#fieldProductID').val('-1');
                $('#slaLevel').val('-1');
                $('#btnProductSave').text("Associate Product");
                $('.customField:visible').each(function () {
                    switch ($(this).attr("type")) {
                        case "checkbox":
                            $(this).prop('checked', false);
                            break;
                        default:
                            $(this).val('');
                    }
                });
                $('#productForm').toggle();
            }, function () {
                alert('There was an error saving this product association. Please try again.');
            });
        }

    });

    //function convertToValidDate(val) {
    //    var value = '';
    //    if (val == "")
    //        return value;

    //    if (_dateFormat.indexOf("M") != 0) {
    //        var dateArr = val.replace(/\./g, '/').replace(/-/g, '/').split('/');
    //        if (_dateFormat.indexOf("D") == 0)
    //            var day = dateArr[0];
    //        if (_dateFormat.indexOf("Y") == 0)
    //            var year = dateArr[0];
    //        if (_dateFormat.indexOf("M") == 3 || _dateFormat.indexOf("M") == 5)
    //            var month = dateArr[1];

    //        var timeSplit = dateArr[2].split(' ');
    //        if (_dateFormat.indexOf("Y") == 6)
    //            var year = timeSplit[0];
    //        else
    //            var day = timeSplit[0];

    //        var theTime = timeSplit[1];

    //        var formattedDate = month + "/" + day + "/" + year;
    //        value = _mainFrame.Ts.Utils.getMsDate(formattedDate);
    //        return formattedDate;
    //    }
    //    else
    //        return val;
    //}

    _mainFrame.Ts.Services.Customers.GetDateFormat(false, function (dateformat) {
        _dateFormat = dateformat.replace('D', 'DD').replace('DDD', 'DD');
        $('.datepicker').attr("data-format", _dateFormat);
        $('.datepicker').datetimepicker({ pickTime: false, format: _dateFormat });

        $('#productExpiration').attr("data-format", dateformat);
        $('.datetimepicker').datetimepicker({});
    });

    $('.userList').on('click', '.contactlink', function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Customer Detail - Open Contact From List');
        _mainFrame.Ts.MainPage.openNewContact(this.id);
    });

    $('.asset-action-assign').click(function (e) {
        e.preventDefault();
    });

    $("#dateShipped").datetimepicker();

    $('.assetList').on('click', '.assetLink', function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Customer Detail - Open Asset From List');
        _mainFrame.Ts.MainPage.openNewAsset(this.id);
    });

    var getAssets = function (request, response) {
        if (_execGetAsset) { _execGetAsset._executor.abort(); }
        _execGetAsset = _mainFrame.Ts.Services.Organizations.GetWarehouseAssets(request.term, function (result) { response(result); });
    }

    $('#inputAsset').autocomplete({
        open: function () {
            $('.ui-menu').width($('#inputAsset').width());
        },
        minLength: 2,
        source: getAssets,
        select: function (event, ui) {
            $(this).data('item', ui.item);
        }
    });

    $('#btnSaveAssign').click(function (e) {
        if ($('#inputAsset').data('item') && $('#dateShipped').val()) {
            var assetAssignmentInfo = new Object();

            assetAssignmentInfo.RefID = organizationID;
            assetAssignmentInfo.RefType = 9;
            assetAssignmentInfo.DateShipped = $('#dateShipped').val();
            assetAssignmentInfo.TrackingNumber = $('#trackingNumber').val();
            assetAssignmentInfo.ShippingMethod = $('#shippingMethod').val();
            assetAssignmentInfo.ReferenceNumber = $('#referenceNumber').val();
            assetAssignmentInfo.Comments = $('#comments').val();
            assetAssignmentInfo.AssigneeName = $('#fieldName').text();

            _mainFrame.Ts.Services.Assets.AssignAsset($('#inputAsset').data('item').id, parent.JSON.stringify(assetAssignmentInfo), function (assetHtml) {
                _mainFrame.Ts.System.logAction('Customer Detail - Asset Assigned');
                $('#modalAssign').modal('hide');
                $('.assetList').prepend(assetHtml);
            }, function () {
                alert('There was an error assigning this asset.  Please try again.');
            });
        }
        else {
            if (!$('#inputAsset').data('item')) {
                alert("Please select a valid asset to assign to this customer.");
            }
            else {
                alert("Please enter a valid date shipped.");
            }
        }
        //    if ($('#reminderDesc').val() != "" && $('#reminderDate').val() != "") {
        //      _mainFrame.Ts.Services.System.EditReminder(null, _mainFrame.Ts.ReferenceTypes.Organizations, organizationID, $('#reminderDesc').val(), _mainFrame.Ts.Utils.getMsDate($('#reminderDate').val()), $('#reminderUsers').val());
        //      $('#modalReminder').modal('hide');
        //    }
        //    else
        //      alert("Please fill in all the fields");
    });

    $('#tblSLATriggers').on('click', '.slaTriggerEdit', function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Customer Detail - Edit SLA Trigger');
    });

    $('#tblProducts').on('click', '.productEdit', function (e) {
        e.preventDefault();
        var product = $(this).parent().parent().attr('id');
        var orgproductID;
        _mainFrame.Ts.System.logAction('Customer Detail - Edit Product');
        _mainFrame.Ts.Services.Customers.LoadProduct(product, function (prod) {
            orgproductID = prod.OrganizationProductID;
            LoadProductVersions(prod.ProductID, prod.VersionNumber);
            $('#productProduct').val(prod.ProductID);
            $('#productExpiration').val(prod.SupportExpiration);
            $('#fieldProductID').val(orgproductID);
            $('#slaLevel').val(prod.SlaLevelID);
            $('#btnProductSave').text("Save");
            _mainFrame.Ts.Services.Customers.LoadCustomProductFields(product, function (custField) {
                for (var i = 0; i < custField.length; i++) {
                    if (custField[i].FieldType == 2) {
                        if (custField[i].Value == "True")
                            $('#' + custField[i].CustomFieldID).prop('checked', true);
                    }
                        //else if (custField[i].FieldType == 5)
                        //{
                        //    var date = field.value == null ? null : _mainFrame.Ts.Utils.getMsDate(field.Value);
                        //    $('#' + custField[i].CustomFieldID).val(date.localeFormat(_mainFrame.Ts.Utils.getDatePattern()));
                        //}

                    else
                        $('#' + custField[i].CustomFieldID).val(custField[i].Value);
                }
            });
        });

        $('#productForm').show();



    });

    $('#tblProducts').on('click', '.productHeader', function (e) {
        e.preventDefault();
        _productsSortColumn = $(this).text();
        var sortIcon = $(this).children(i);
        if (sortIcon.length > 0) {
            if (sortIcon.hasClass('fa-sort-asc')) {
                _productsSortDirection = 'DESC'
            }
            else {
                _productsSortDirection = 'ASC'
            }
            sortIcon.toggleClass('fa-sort-asc fa-sort-desc');
        }
        else {
            $('.productHeader').children(i).remove();
            var newSortIcon = $('<i>')
                .addClass('fa fa-sort-asc')
                .appendTo($(this));
            _customersSortDirection = 'ASC';
            switch (_productsSortColumn.toLowerCase()) {
                case "version":
                case "support expiration":
                case "released date":
                case "sla assigned":
                case "date created":
                    newSortIcon.toggleClass('fa-sort-asc fa-sort-desc');
                    _productsSortDirection = 'DESC';

            }
        }
        LoadProducts();
    });

    $("#btnProductCancel").click(function (e) {
        e.preventDefault();
        LoadProductTypes();
        _mainFrame.Ts.System.logAction('Customer Detail - Cancel Product Edit');
        $('#productExpiration').val('');
        $('#fieldProductID').val('-1');
        $('#btnProductSave').text("Associate Product");
        $('.customField:visible').each(function () {
            switch ($(this).attr("type")) {
                case "checkbox":
                    $(this).prop('checked', false);
                    break;
                default:
                    $(this).val('');
            }
        });
        $('#productForm').toggle();
    });

    $('#tblProducts').on('click', '.productDelete', function (e) {
        e.preventDefault();
        if (confirm('All contact associations with this product and version will also be deleted. Are you sure you would like to remove this product association?')) {
            _mainFrame.Ts.System.logAction('Customer Detail - Delete Product');
            parent.privateServices.DeleteOrganizationProduct($(this).parent().parent().attr('id'), false, function (e) {
                LoadProducts();
            });

        }
    });

    $('#tblProducts').on('click', '.productView', function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Customer Detail - View Product');
        _mainFrame.Ts.MainPage.openOrganizationProduct($(this).parent().parent().attr('id'))
        //parent.location = "../../../Default.aspx?OrganizationProductID=" + ;

    });

    $('#tblProducts').on('click', '.productVersionView', function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Customer Detail - View Product Version');
        _mainFrame.Ts.MainPage.openOrganizationProductVersion($(this).parent().parent().attr('id'))
        //parent.location = "../../../Default.aspx?OrganizationProductID=" + ;

    });

    parent.privateServices.IsSubscribed(9, organizationID, issubbed);

    function issubbed(result) {
        if (result)
            $('#customerSubscribe').html('Unsubscribe');
        else
            $('#customerSubscribe').html('<i class="fa fa-rss"></i>');
    }

    $('#customerSubscribe').click(function (e) {
        parent.privateServices.Subscribe(9, organizationID);
        parent.privateServices.IsSubscribed(9, organizationID, issubbed);
    });

    $('#customerRefresh').click(function (e) {
        _mainFrame.Ts.System.logAction('Customer Detail - Refresh Page');
        window.location = window.location;
    });

    $('#customerParentView').click(function (e) {
        _mainFrame.Ts.System.logAction('Customer Detail - Switch to parent view');
        var href = window.location.href;
        var i = window.location.href.indexOf('parentView');
        if (i != -1) {
            href = href.substring(0, i - 1);
        }
        window.location.href = href + "&parentView=1";
    });

    $('#customerNormalView').click(function (e) {
        _mainFrame.Ts.System.logAction('Customer Detail - Switch to normal view');
        var href = window.location.href;
        var i = window.location.href.indexOf('parentView');
        if (i != -1) {
            href = href.substring(0, i - 1);
        }
        window.location.href = href;
    });

    var getCustomers = function (request, response) {
        if (_execGetCustomer) { _execGetCustomer._executor.abort(); }
        _execGetCustomer = _mainFrame.Ts.Services.Organizations.GetOrganizationForTicket(request.term, function (result) { response(result); });
    }

    $("#Company-Merge-search").autocomplete({
        minLength: 2,
        source: getCustomers,
        select: function (event, ui) {
            if (ui.item.id == organizationID) {
                alert("Sorry, but you can not merge this company into itself.");
                return;
            }

            $(this).data('organizationid', ui.item.id).removeClass('ui-autocomplete-loading');

            try {
                _mainFrame.Ts.Services.Organizations.GetOrganization(ui.item.id, function (info) {
                    var descriptionString = info.Description;

                    if (descriptionString == null) {
                        descriptionString = "";
                    }

                    if (ellipseString(descriptionString, 30).indexOf("<img src") !== -1)
                        descriptionString = "This company description starts off with an embedded/linked image. We have disabled this for the preview.";
                    else if (ellipseString(descriptionString, 30).indexOf(".viewscreencast.com") !== -1)
                        descriptionString = "This company description starts off with an embedded recorded video.  We have disabled this for the preview.";
                    else
                        descriptionString = ellipseString(descriptionString, 30);

                    var companyPreviewName = "<div><strong>Company Name:</strong> " + info.Name + "</div>";
                    var companyPreviewWebsite = "<div><strong>Company Website:</strong> " + info.Website + "</div>";
                    var companyPreviewDesc = "<div><strong>Company Desciption Sample:</strong> " + descriptionString + "</div>";

                    $('#companymerge-preview-details').after(companyPreviewName + companyPreviewWebsite + companyPreviewDesc);
                    $('#dialog-companymerge-preview').show();
                    $('#dialog-companymerge-warning').show();
                    $(".dialog-companymerge").dialog("widget").find(".ui-dialog-buttonpane").find(":button:contains('OK')").prop("disabled", false).removeClass("ui-state-disabled");
                })
            }
            catch (e) {
                alert("Sorry, there was a problem loading the information for that company.");
            }
        },
        position: { my: "right top", at: "right bottom", collision: "fit flip" }
    });

    $('#company-merge-complete').click(function (e) {
        e.preventDefault();
        $('#company-merge-complete').attr('disabled', 'disabled');
        if ($('#Company-Merge-search').val() == "") {
            alert("Please select a valid company to merge");
            $('#company-merge-complete').removeAttr('disabled');
            return;
        }

        if ($('#dialog-companymerge-confirm').prop("checked")) {
            var winningID = $('#Company-Merge-search').data('organizationid');
            //var winningCompanyName = $('#Company-Merge-search').data('organizationname');
            var JSTop = top;
            //var window = window;
            $('.merge-processing').show();
            _mainFrame.Ts.Services.Customers.MergeCompanies(winningID, organizationID, function (result) {
                $('.merge-processing').hide();
                $('#company-merge-complete').removeAttr('disabled');
                if (result != "")
                    alert(result);
                else {
                    $('#MergeModal').modal('hide');
                    JS_mainFrame.Ts.MainPage.closeNewCustomerTab(organizationID);
                    JS_mainFrame.Ts.MainPage.openNewCustomer(winningID);
                    //window.location = window.location;
                    //window.parent.ticketSocket.server.ticketUpdate(organizationID + "," + winningID, "merge", userFullName);
                }
            });
            //_mainFrame.Ts.Services.Tickets.MergeTickets(winningID, _ticketID, MergeSuccessEvent(_ticketNumber, winningTicketNumber),
            //  function () {
            //  $('#merge-error').show();
            //});
        }
        else {
            alert("You did not agree to the conditions of the merge. Please go back and check the box if you would like to merge.")
            $('#company-merge-complete').removeAttr('disabled');
        }
    });

    $('#customerDelete').click(function (e) {
        if (confirm('Are you sure you would like to remove this organization?')) {
            _mainFrame.Ts.System.logAction('Customer Detail - Delete Customer');
            parent.privateServices.DeleteOrganization(organizationID, function (e) {
                if (window.parent.document.getElementById('iframe-mniCustomers'))
                    window.parent.document.getElementById('iframe-mniCustomers').contentWindow.refreshPage();
                _mainFrame.Ts.MainPage.closeNewCustomerTab(organizationID);
            });
        }
    });

    $('#phonePanel').on('click', '.delphone', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you would like to remove this phone number?')) {
            _mainFrame.Ts.System.logAction('Customer Detail - Delete Phone Number');
            parent.privateServices.DeletePhone($(this).attr('id'), function (e) {
                LoadPhoneNumbers(1);
                $("#phonePanel #editmenu").toggleClass("hiddenmenu");
            });

        }
    });

    $("#phonePanel").on("click", '.editphone', function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Customer Detail - Edit Phone Number');
        _mainFrame.Ts.Services.Customers.LoadPhoneNumber($(this).attr('id'), function (phone) {
            $('#phoneType').val(phone[0].PhoneTypeID);
            $('#phoneNumber').val(phone[0].Number);
            $('#phoneExt').val(phone[0].Extension);
            $('#phoneID').val(phone[0].PhoneID);
            $('#modalPhone').modal('show');
        });
    });

    $('#addressPanel').on('click', '.deladdress', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you would like to remove this address?')) {
            _mainFrame.Ts.System.logAction('Customer Detail - Delete Address');
            parent.privateServices.DeleteAddress($(this).attr('id'), function (e) {
                LoadAddresses(1);
                $("#addressPanel #editmenu").toggleClass("hiddenmenu");
            });

        }
    });

    $("#addressPanel").on("click", '.editaddress', function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Customer Detail - Edit Address');
        _mainFrame.Ts.Services.Customers.LoadAddress($(this).attr('id'), function (phone) {
            $('#addressDesc').val(phone[0].Description);
            $('#address1').val(phone[0].Addr1);
            $('#address2').val(phone[0].Addr2);
            $('#address3').val(phone[0].Addr3);
            $('#addressCity').val(phone[0].City);
            $('#addressState').val(phone[0].State);
            $('#addressZip').val(phone[0].Zip);
            $('#addressCountry').val(phone[0].Country);
            $('#addressID').val(phone[0].AddressID);
            $('#modalAddress').modal('show');
        });
    });

    $('#reminderDate').datetimepicker();

    $("#btnSaveReminder").click(function (e) {
        if ($('#reminderDesc').val() != "" && $('#reminderDate').val() != "") {
            _mainFrame.Ts.System.logAction('Customer Detail - Save Reminder');
            _mainFrame.Ts.Services.System.EditReminder(null, _mainFrame.Ts.ReferenceTypes.Organizations, organizationID, $('#reminderDesc').val(), _mainFrame.Ts.Utils.getMsDate($('#reminderDate').val()), $('#reminderUsers').val());
            $('#modalReminder').modal('hide');
        }
        else
            alert("Please fill in all the fields");
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
        parent.Ts.System.logAction('Customer Page - Change Task Status');

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
        parent.Ts.System.logAction('Customer Page - New Task');
        parent.Ts.MainPage.newTaskFromSource(9, organizationID, _companyName, '');
    });

    $("#btnPhoneSave").click(function (e) {
        var phoneInfo = new Object();
        _mainFrame.Ts.System.logAction('Customer Detail - Save Phone Number');
        phoneInfo.PhoneTypeID = $('#phoneType').val();
        phoneInfo.Number = $('#phoneNumber').val();
        phoneInfo.Extension = $('#phoneExt').val();
        phoneInfo.PhoneID = $('#phoneID').val() != "" ? $('#phoneID').val() : "-1";
        var inEditmode = $('#customerEdit').hasClass("btn-success")

        _mainFrame.Ts.Services.Customers.SavePhoneNumber(parent.JSON.stringify(phoneInfo), organizationID, _mainFrame.Ts.ReferenceTypes.Organizations, function (f) {
            $("#phoneType")[0].selectedIndex = 0;
            $('#phoneNumber').val('');
            $('#phoneExt').val('')
            $('#phoneID').val('-1');
            $('#modalPhone').modal('hide');
            if (inEditmode)
                LoadPhoneNumbers(1);
            else
                LoadPhoneNumbers();
        }, function () {
            alert('There was an error saving this phone number.  Please try again.');
        });

    });

    //$("#phonePanel, #addressPanel").on('mouseenter', '.content', function () {
    //    $(this).find(".hiddenmenu").toggle();
    //});
    //$("#phonePanel, #addressPanel").on('mouseleave', '.content', function () {
    //    $(this).find(".hiddenmenu").toggle();
    //});

    $("#btnAddressSave").click(function (e) {
        var addressInfo = new Object();
        _mainFrame.Ts.System.logAction('Customer Detail - Save Address');
        addressInfo.Description = $('#addressDesc').val();
        addressInfo.Addr1 = $('#address1').val();
        addressInfo.Addr2 = $('#address2').val();
        addressInfo.Addr3 = $('#address3').val();
        addressInfo.City = $('#addressCity').val();
        addressInfo.State = $('#addressState').val();
        addressInfo.Zip = $('#addressZip').val();
        addressInfo.Country = $('#addressCountry').val();
        addressInfo.AddressID = $('#addressID').val();
        var inEditmode = $('#customerEdit').hasClass("btn-success")

        _mainFrame.Ts.Services.Customers.SaveAddress(parent.JSON.stringify(addressInfo), organizationID, _mainFrame.Ts.ReferenceTypes.Organizations, function (f) {

            addressInfo.Description = $('#addressDesc').val('');
            addressInfo.Addr1 = $('#address1').val('');
            addressInfo.Addr2 = $('#address2').val('');
            addressInfo.Addr3 = $('#address3').val('');
            addressInfo.City = $('#addressCity').val('');
            addressInfo.State = $('#addressState').val('');
            addressInfo.Zip = $('#addressZip').val('');
            addressInfo.Country = $('#addressCountry').val('');
            addressInfo.AddressID = $('#addressID').val('-1');
            $('#modalAddress').modal('hide');
            if (inEditmode)
                LoadAddresses(1);
            else
                LoadAddresses();
        }, function () {
            alert('There was an error saving this address.  Please try again.');
        });

    });

    $('#tblNotes, #tblNotesAdditional').on('click', '.editNote', function (e) {
        e.stopPropagation();
        _mainFrame.Ts.System.logAction('Customer Detail - Edit Note');
        _mainFrame.Ts.Services.Customers.LoadNote($(this).parent().parent().attr('id'), function (note) {
            $('#fieldNoteTitle').val(note.Title);
            var desc = note.Description;
            desc = desc.replace(/<br\s?\/?>/g, "\n");
            $('#fieldNoteDesc').val(desc);
            $('#fieldNoteID').val(note.NoteID);
            $('#noteCustomerAlert').prop('checked', note.IsAlert);
            $('#btnNotesSave').text("Save");
            $('#btnNotesCancel').show();
            $('#noteAttachmentsRow').show();
            if (note.Attachments != null) {
                for (var i = 0; i < note.Attachments.length ; i++) {
                    var div = $('<div>');

                    var name = $('<span>')
                        .addClass('noteAttachmentName')
                        .text(note.Attachments[i].FileName)
                        .appendTo(div);

                    var attachmentID = note.Attachments[i].AttachmentID;

                    $('<i>')
                        .addClass('fa fa-trash-o')
                        .appendTo(div)
                        .click(function (e) {
                            if (confirm("Are you sure you want to delete this file attachment")) {
                                _mainFrame.Ts.System.logAction('Customer Detail - Delete Note File Attachment');
                                parent.privateServices.DeleteAttachment(attachmentID, function (e) {
                                    div.remove();
                                });
                            }
                        });

                    div.appendTo($('#noteAttachments'));
                };
            }
            $('#noteForm').show();
            initEditor($('#fieldNoteDesc'), function (ed) {
                $('#fieldNoteDesc').tinymce().setContent(desc);
                $('#fieldNoteDesc').tinymce().focus();
            });

            if (note.ProductFamilyID) {
                $('#ddlNoteProductFamily').val(note.ProductFamilyID);
            }
            else {
                $('#ddlNoteProductFamily').val(-1);
            }
            $('#ddlNoteActivityType').val(note.ActivityType);
            $('#activityDate').data("DateTimePicker").setDate(moment(note.DateOccurred));
        });
    });

    $('#tblNotes, #tblNotesAdditional').on('click', '.deleteNote', function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (confirm('Are you sure you would like to DELETE this activity?')) {
            _mainFrame.Ts.System.logAction('Customer Detail - Delete Note');
            parent.privateServices.DeleteNote($(this).parent().parent().attr('id'), function () {
                LoadNotes();
                $('#noteDescBox').toggle(false);
            });
        }
    });

    $('#tblNotes').on('click', '.viewNote', function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Customer Detail - View Note');
        var desc = $(this).data('description');
        $('#tblNotes tbody tr').removeClass("active");

        $(this).addClass("active");

        $('#noteDescBox').toggle();
        if ($('#noteDescBox').is(':visible'))
        {
            $('.noteDesc').html("<h4>Description</h4> <p>" + desc + "</p>");
            BuildFileDescription($(this).data('attachments'));
        }
    });

    $('#tblNotes, #tblNotesAdditional').on('click', '.addTask', function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Customer Detail - Add Note Task');
        var id = $(this).parent().parent().attr('id');
        _mainFrame.Ts.MainPage.newTask(undefined, null, id);
    });

    $('#tblNotesAdditional').on('click', '.viewNote', function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Customer Detail - View Note');
        var desc = $(this).data('description');
        $('#tblNotesAdditional tbody tr').removeClass("active");

        $(this).addClass("active");

        $('#noteDescBox').toggle();
        if ($('#noteDescBox').is(':visible')) {
            $('.noteDesc').html("<h4>Description</h4> <p>" + desc + "</p>");
            BuildFileDescription($(this).data('attachments'));
        }
    });

    $('.noteDesc').on('click', '.activity-file', function (e) {
        e.preventDefault();
        _mainFrame.Ts.MainPage.openNewAttachment($(this).attr('id'));
    });

    $('#tblFiles').on('click', '.viewFile', function (e) {
        e.preventDefault();
        _mainFrame.Ts.MainPage.openNewAttachment($(this).parent().attr('id'));
    });

    $('#tblFiles').on('click', '.delFile', function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (confirm('Are you sure you would like to remove this attachment?')) {
            _mainFrame.Ts.System.logAction('Customer Detail - Delete File');
            parent.privateServices.DeleteAttachment($(this).parent().parent().attr('id'), function (e) {
                LoadFiles();
            });

        }
    });

    $("#btnNotesCancel").click(function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Customer Detail - Note Form Cancel');
        $('.upload-queue-activity').empty();
        $('#ddlNoteActivityType').val('4');
        $('#activityDate').val('');
        $('#activityDate').datetimepicker('setDate', new Date());
        $('#fieldNoteTitle').val('');
        $('#fieldNoteDesc').val('');
        $('#fieldNoteID').val('-1');
        $('#noteCustomerAlert').prop('checked', false);
        $('#noteAttachments').html('');
        $('#noteAttachmentsRow').hide();
        $('#btnNotesSave').text("Save");
        for (var i = tinymce.editors.length - 1 ; i > -1 ; i--) {
            var ed_id = tinymce.editors[i].id;
            tinyMCE.execCommand("mceRemoveEditor", true, ed_id);
        }
        $('#noteForm').toggle();
        _mainFrame.Ts.System.logAction('Customer Detail - Cancel Note Edit / Add');
    });

    $("#btnNotesSave").click(function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Customer Detail - Save Note');
        var title = $('#fieldNoteTitle').val();
        var description = $('#fieldNoteDesc').val();
        var noteID = $('#fieldNoteID').val();
        var isAlert = $('#noteCustomerAlert').prop('checked');
        var activityType = $('#ddlNoteActivityType').val();
        var DateOccurred = convertToValidDate($('#activityDate').val());

        if (DateOccurred == null)
        {
            alert("Invalid date news to be in the format of " + _dateFormat);
            return;
        }

        if ((title.length || description.length) < 1) {
            alert("Please fill in all the required information");
            return;
        }
        $(this).prop('disabled', true);
        var productFamilyID = $("#ddlNoteProductFamily").val();
        _mainFrame.Ts.Services.Customers.SaveNote(title, description, noteID, organizationID, _mainFrame.Ts.ReferenceTypes.Organizations, activityType, DateOccurred, isAlert, productFamilyID, function (note) {
            if ($('.upload-queue-activity li').length > 0) {
                $('.upload-queue-activity li').each(function (i, o) {
                    var data = $(o).data('data');
                    data.url = '../../../Upload/CustomerActivityAttachments/' + note;
                    data.jqXHR = data.submit();
                    $(o).data('data', data);
                });
            }
            else
            {
                LoadNotes();
            }
            $('#fieldNoteTitle').val('');
            $('#fieldNoteDesc').val('');
            $('#fieldNoteID').val('-1');
            $('#ddlNoteActivityType').val('4');
            $('#activityDate').val('');
            $('#activityDate').data("DateTimePicker").setDate(new Date());
            $('#ddlNoteProductFamily').val('-1');
            $('#noteCustomerAlert').prop('checked', false);
            $('#btnNotesSave').text("Save");
            $('#noteForm').toggle();
            for (var i = tinymce.editors.length - 1 ; i > -1 ; i--) {
                var ed_id = tinymce.editors[i].id;
                tinyMCE.execCommand("mceRemoveEditor", true, ed_id);
            }
            $("#btnNotesSave").removeProp('disabled');
        });
    });

    $("#btnFilesCancel").click(function (e) {
        _mainFrame.Ts.System.logAction('Customer Detail - Cancel File Form');
        $('.upload-queue').empty();
        $('#attachmentDescription').val('');
        $('#ddlFileProductFamily').val('-1');
        $('#fileForm').toggle();
    });

    $('#btnFilesSave').click(function (e) {
        _mainFrame.Ts.System.logAction('Customer Detail - Save Files');
        if ($('.upload-queue li').length > 0) {
            $('.upload-queue li').each(function (i, o) {
                var data = $(o).data('data');
                data.formData = {
                    description: $('#attachmentDescription').val().replace(/<br\s?\/?>/g, "\n"),
                    productFamilyID: $("#ddlFileProductFamily").val()
                };
                data.url = '../../../Upload/OrganizationAttachments/' + organizationID;
                data.jqXHR = data.submit();
                $(o).data('data', data);
            });
        }
        //$('#fileForm').toggle();
    });

    $('.file-upload').fileupload({
        namespace: 'custom_attachment',
        dropZone: $('.file-upload'),
        add: function (e, data) {
            for (var i = 0; i < data.files.length; i++) {
                var item = $('<li>')
                  .appendTo($('.upload-queue'));

                data.context = item;
                item.data('data', data);

                var bg = $('<div>')
                  .addClass('ts-color-bg-accent')
                  .appendTo(item);

                $('<div>')
                  .text(data.files[i].name + '  (' + _mainFrame.Ts.Utils.getSizeString(data.files[i].size) + ')')
                  .addClass('filename')
                  .appendTo(bg);

                $('<span>')
                  .addClass('icon-remove')
                  .click(function (e) {
                      e.preventDefault();
                      $(this).closest('li').fadeOut(500, function () { $(this).remove(); });
                  })
                  .appendTo(bg);

                $('<span>')
                  .addClass('icon-remove')
                  .hide()
                  .click(function (e) {
                      e.preventDefault();
                      var data = $(this).closest('li').data('data');
                      data.jqXHR.abort();
                  })
                  .appendTo(bg);

                var progress = $('<div>')
                  .addClass('progress progress-striped active')
                  .hide();

                $('<div>')
                    .addClass('progress-bar')
                    .attr('role', 'progressbar')
                    .appendTo(progress);

                progress.appendTo(bg);
            }

        },
        send: function (e, data) {
            if (data.context && data.dataType && data.dataType.substr(0, 6) === 'iframe') {
                data.context.find('.progress-bar').css('width', '50%');
            }
        },
        fail: function (e, data) {
            if (data.errorThrown === 'abort') return;
            alert('There was an error uploading "' + data.files[0].name + '".');
        },
        progress: function (e, data) {
            data.context.find('.progress-bar').css('width', parseInt(data.loaded / data.total * 100, 10) + '%');
        },
        start: function (e, data) {
            $('.progress').show();
            $('.upload-queue .ui-icon-close').hide();
            $('.upload-queue .ui-icon-cancel').show();
        },
        stop: function (e, data) {
            //data.context.find('.progress-bar').css('width', '100%');
            LoadFiles();
            $('.upload-queue').empty();
            $('#attachmentDescription').val('');
            $('#ddlFileProductFamily').val('-1');
            $('#fileForm').toggle();
        }
    });

    $('.file-upload-activity').fileupload({
        namespace: 'custom_attachment',
        dropZone: $('.file-upload-activity'),
        add: function (e, data) {
            for (var i = 0; i < data.files.length; i++) {
                var item = $('<li>')
                  .appendTo($('.upload-queue-activity'));
                data.context = item;
                item.data('data', data);
                var bg = $('<div>')
                  .addClass('ts-color-bg-accent')
                  .appendTo(item);
                $('<div>')
                  .text(data.files[i].name + '  (' + _mainFrame.Ts.Utils.getSizeString(data.files[i].size) + ')')
                  .addClass('filename')
                  .appendTo(bg);
                $('<span>')
                  .addClass('icon-remove')
                  .click(function (e) {
                      e.preventDefault();
                      $(this).closest('li').fadeOut(500, function () { $(this).remove(); });
                  })
                  .appendTo(bg);
                $('<span>')
                  .addClass('icon-remove')
                  .hide()
                  .click(function (e) {
                      e.preventDefault();
                      var data = $(this).closest('li').data('data');
                      data.jqXHR.abort();
                  })
                  .appendTo(bg);
                var progress = $('<div>')
                  .addClass('progress progress-striped active')
                  .hide();
                $('<div>')
                    .addClass('progress-bar')
                    .attr('role', 'progressbar')
                    .appendTo(progress);
                progress.appendTo(bg);
            }
        },
        send: function (e, data) {
            if (data.context && data.dataType && data.dataType.substr(0, 6) === 'iframe') {
                data.context.find('.progress-bar').css('width', '50%');
            }
        },
        fail: function (e, data) {
            if (data.errorThrown === 'abort') return;
            alert('There was an error uploading "' + data.files[0].name + '".');
        },
        progress: function (e, data) {
            data.context.find('.progress-bar').css('width', parseInt(data.loaded / data.total * 100, 10) + '%');
        },
        start: function (e, data) {
            $('.progress').show();
            $('.upload-queue-activity .ui-icon-close').hide();
            $('.upload-queue-activity .ui-icon-cancel').show();
        },
        stop: function (e, data) {
            $('.upload-queue-activity').empty();
            LoadNotes();
        }
    });

    $('#productCustomer').autocomplete({
        minLength: 2,
        source: getCompany,
        select: function (event, ui) {
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading')
        }
    });

    if (_mainFrame.Ts.System.User.FilterInactive) {
        $('#cbActive').prop('checked', true);
    }

    $('#cbActive').click(function (e) {
        _mainFrame.Ts.Services.Users.SetInactiveFilter(_mainFrame.Ts.System.User.UserID, $('#cbActive').prop('checked') ? true : false, function (result) {
            _mainFrame.Ts.System.logAction('User Info - Changed Filter Inactive Setting');
        },
              function (error) {
                  alert('There was an error saving the user filter inaactive setting.');
                  item.next().hide();
              });
        LoadContacts();
    });

    $("#modalPhone").on('hidden.bs.modal', function () {
        $('#modalPhone input').val('');
    });
    $("#modalAddress").on('hidden.bs.modal', function () {
        $('#modalAddress input').val('');
        $('#modalAddress #addressID').val('-1');
    });
    $("#modalReminder").on('hidden.bs.modal', function () {
        $('#modalReminder input').val('');
    });

    _mainFrame.Ts.Services.Tickets.Load5MostRecentByOrgID2(organizationID, _isParentView, function (tickets) {
        var max = 5;
        if (tickets.length < 5)
            max = tickets.length;


        for (var i = 0; i < max; i++) {
            var div = $('<div>')
          .data('o', tickets[i])
          .addClass('ticket');

            $('<span>')
          .addClass('ts-icon ts-icon-info')
          .attr('rel', '../Tips/Ticket.aspx?TicketID=' + tickets[i].TicketID)
          .appendTo(div);

            var caption = $('<span>')
          .addClass('ticket-name')
          .appendTo(div);

            $('<a>')
          .addClass('ts-link ui-state-defaultx')
          .attr('href', '#')
          .text(tickets[i].TicketNumber + ': ' + ellipseString(tickets[i].Name, 50))
          .appendTo(caption)
          .click(function (e) {
              e.preventDefault();
              _mainFrame.Ts.MainPage.openTicket($(this).closest('.ticket').data('o').TicketNumber, true);
          });


            div.appendTo(tickets[i].IsClosed == false ? '#openTickets' : '#closedTickets');
        }

        if ($('#openTickets .ticket').length < 1) {
            $('<div>')
            .addClass('no-tickets')
            .text('There are no recent tickets to display')
            .appendTo('#openTickets');
        }
    });

    function LoadCustomProperties() {
        _mainFrame.Ts.Services.Customers.GetCustomValues(organizationID, _mainFrame.Ts.ReferenceTypes.Organizations, function (html) {
            //$('#customProperties').append(html);
            appendCustomValues(html);
        });
    }

    function LoadProperties() {
        _mainFrame.Ts.Services.Customers.GetProperties(organizationID, function (result) {
            if (result.SAED == '[None]')
                result.SAED = null;
            var date = result.SAED == null ? null : _mainFrame.Ts.Utils.getMsDate(result.SAED);
            $('#fieldName').text(result.orgproxy.Name);
            $('#fieldWebsite').text(result.orgproxy.Website != null && result.orgproxy.Website != "" ? result.orgproxy.Website : "Empty");
            if ($('#fieldWebsite').text() == "Empty")
                $('#fieldWebsite').removeClass("link");
            $('#fieldDomains').text(result.orgproxy.CompanyDomains != "" ? result.orgproxy.CompanyDomains : "Empty");
            $('#fieldActive').text(result.orgproxy.IsActive);
            $('#fieldPortalAccess').text(result.orgproxy.HasPortalAccess);
            $('#fieldAPIEnabled').text(result.orgproxy.IsApiActive && result.orgproxy.IsApiEnabled);
            $('#fieldSAED').text(result.SAED == null ? "[None]" : date.localeFormat(_mainFrame.Ts.Utils.getDatePattern()));
            $('#fieldSLA').text(result.SLA);
            $('#fieldSLA').data('field', result.orgproxy.SlaLevelID);
            $('#fieldSupportHours').text(result.orgproxy.SupportHoursMonth);
            $('#fieldDescription').html(result.orgproxy.Description != null && result.orgproxy.Description != "" ? result.orgproxy.Description : "Empty");
            $('#fieldAPIToken').text(result.orgproxy.WebServiceID);
            $('#fieldOrgID').text(result.orgproxy.OrganizationID);
            $('#fieldPrimaryContact').text(result.PrimaryUser == "" ? "Empty" : result.PrimaryUser);
            $('#fieldPrimaryContact').data('field', result.orgproxy.PrimaryUserID);
            $('#fieldDefaultUser').text(result.DefaultSupportUser);
            $('#fieldDefaultUser').data('field', result.orgproxy.DefaultSupportUserID);
            $('#fieldDefaultGroup').text(result.SupportGroup);
            $('#fieldDefaultGroup').data('field', result.orgproxy.DefaultSupportGroupID);
            $('#fieldInactive').html(result.orgproxy.InActiveReason != null && result.orgproxy.InActiveReason != "" ? result.orgproxy.InActiveReason : "Empty");

            $('#fieldTimeZone').text(result.orgproxy.TimeZoneID == "" ? "Central Standard Time" : result.orgproxy.TimeZoneID);
            $('#fieldTimeZone').data('field', result.orgproxy.TimeZoneID);

            if (!_isAdmin || result.orgproxy.IsActive == false) {
                $('#groupInactive').hide();
            }

            if (result.orgproxy.Name == "_Unknown Company") {
                _isUnknown = true;
                $('#customPropRow').hide();
                //$('#customerEdit').hide();
                $('#customerDelete').hide();
            }

            SetupParentSection(result.Parents);
            if (result.IsParent == false) {
                $('#customerParentView').hide();
            }
        });
    }

    function LoadAddresses(reload) {
        $('#addressPanel').empty();
        _mainFrame.Ts.Services.Customers.LoadAddresses(organizationID, _mainFrame.Ts.ReferenceTypes.Organizations, function (address) {
            for (var i = 0; i < address.length; i++) {
                $('#addressPanel').append("<div class='form-group content'> \
                                        <label for='inputName' class='col-xs-4 control-label'>" + address[i].Description + "</label> \
                                        <div class='col-xs-5'> \
                                            " + ((address[i].Addr1 != null) ? "<p class='form-control-static'><a href='" + address[i].MapLink + "' target='_blank' id='" + address[i].AddressID + "' class='mapphone'><span class='fa fa-map-marker'></span></a> " + address[i].Addr1 + "</p>" : "") + " \
                                            " + ((address[i].Addr2 != null) ? "<p class='form-control-static pt0'>" + address[i].Addr2 + "</p>" : "") + " \
                                            " + ((address[i].Addr3 != null) ? "<p class='form-control-static pt0'>" + address[i].Addr3 + "</p>" : "") + " \
                                            <p class='form-control-static pt0'> \
                                            " + ((address[i].City != null) ? "<p class='form-control-static pt0'>" + address[i].City : "") + " \
                                            " + ((address[i].State != null && address[i].State != '') ? ", " + address[i].State : "") + " \
                                            " + ((address[i].Zip != null && address[i].Zip != '') ? " " + address[i].Zip : "") + " </p> \
                                            " + ((address[i].Country != null) ? "<p class='form-control-static pt0'>" + address[i].Country + "</p>" : "") + " \
                                        </div> \
                                        <div id='editmenu' class='col-xs-2 hiddenmenu'> \
                                            <a href='#' id='" + address[i].AddressID + "' class='editaddress'><span class='fa fa-pencil'></span></a>\
                                            <a href='#' id='" + address[i].AddressID + "' class='deladdress'><span class='fa fa-trash-o'></span></a/></p>\
                                        </div> \
                                    </div>");
            }
            if (reload != undefined)
                $("#addressPanel #editmenu").toggleClass("hiddenmenu");
        });
    }

    function LoadNotes() {
        if (_mainFrame.Ts.System.Organization.UseProductFamilies) {
            _mainFrame.Ts.Services.Customers.LoadNotesByUserRights(organizationID, _mainFrame.Ts.ReferenceTypes.Organizations, _isParentView, null, function (note) {
                $('#tblNotes tbody').empty();
                var count = (note) ? note.length : 0;
                $('#a-notes').text('Activities (' + count + ')');
                var html;
                for (var i = 0; i < note.length; i++) {
                    var attachmentString = note[i].Attachments.length > 0 ? "   <i class='fa fa-paperclip'></i>" : "";

                    if (!_isParentView && (_isAdmin || note[i].CreatorID == _mainFrame.Ts.System.User.UserID || _mainFrame.Ts.System.User.CanEditCompany)) {
                        html = '<td><i class="fa fa-edit editNote"></i></td><td><i class="fa fa-trash-o deleteNote"></i></td><td>' + note[i].Title + attachmentString + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td>';
                    } else {
                        html = '<td></td><td></td><td>' + note[i].Title + attachmentString + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td>';
                    }
                    if (note[i].ProductFamilyID != null) {
                        html += '<td>' + note[i].ProductFamily + '</td>';
                    } else {
                        html += '<td>Unassigned</td>';
                    }

                    html += '<td>' + note[i].ActivityTypeString + '</td>';
                    if (note[i].DateOccurred != null) {
                        html += '<td>' + note[i].DateOccurred.toDateString() + '</td>';
                    }
                    else {
                        html += '<td></td>';
                    }

                    html += "<td><i class='fa fa-plus addTask' title='Add Task'></i></td>";

                    $('<tr>').addClass("viewNote")
                    .attr("id", note[i].NoteID)
                    .html(html)
                    .data("attachments", note[i].Attachments)
                    .data("description", note[i].Description)
                    .appendTo('#tblNotes > tbody:last');
                    //$('#tblNotes > tbody:last').append('<tr id=' + note[i].NoteID + ' class="viewNote"><td><i class="glyphicon glyphicon-edit editNote"></i></td><td><i class="glyphicon glyphicon-trash deleteNote"></i></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td></tr>').data('description',note[i].Description);
                    if (noteID != null && noteID == note[i].NoteID) {
                        $('.noteDesc').html("<h4>Description</h4> <p>" + note[i].Description + "</p>");
                        BuildFileDescription(note[i].Attachments);
                        $('#noteDescBox').show();
                    }
                }
            });
        } else {
            _mainFrame.Ts.Services.Customers.LoadNotes2(organizationID, _mainFrame.Ts.ReferenceTypes.Organizations, _isParentView, function (note) {
                $('#tblNotes tbody').empty();
                var html;
                for (var i = 0; i < note.length; i++) {
                    var attachmentString = note[i].Attachments.length > 0 ? "   <i class='fa fa-paperclip'></i>" : "";
                    if (!_isParentView && (_isAdmin || note[i].CreatorID == _mainFrame.Ts.System.User.UserID || _mainFrame.Ts.System.User.CanEditCompany)) {
                        html = '<td><i class="fa fa-edit editNote"></i></td><td><i class="fa fa-trash-o deleteNote"></i></td><td>' + note[i].Title + attachmentString + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td>';
                    } else {
                        html = '<td></td><td></td><td>' + note[i].Title + attachmentString + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td>';
                    }

                    html += '<td>' + note[i].ActivityTypeString + '</td>';
                    if (note[i].DateOccurred != null) {
                        html += '<td>' + note[i].DateOccurred.toDateString() + '</td>';
                    }
                    else {
                        html += '<td></td>';
                    }

                    html += "<td><i class='fa fa-plus' title='Add Task'></i></td>";

                    $('<tr>').addClass("viewNote")
                    .attr("id", note[i].NoteID)
                    .data("attachments", note[i].Attachments)
                    .html(html)
                    .data("description", note[i].Description)
                    .appendTo('#tblNotes > tbody:last');
                    //$('#tblNotes > tbody:last').append('<tr id=' + note[i].NoteID + ' class="viewNote"><td><i class="glyphicon glyphicon-edit editNote"></i></td><td><i class="glyphicon glyphicon-trash deleteNote"></i></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td></tr>').data('description',note[i].Description);
                    if (noteID != null && noteID == note[i].NoteID) {
                        $('.noteDesc').html("<h4>Description</h4> <p>" + note[i].Description + "</p>");
                        BuildFileDescription(note[i].Attachments);
                        $('#noteDescBox').show();
                    }
                }
            });
        }
    }

    //Load notes for users under org
    function LoadNotesAdditional() {
        if (_mainFrame.Ts.System.Organization.UseProductFamilies) {
            _mainFrame.Ts.Services.Customers.LoadNotesByUserRights(organizationID, _mainFrame.Ts.ReferenceTypes.Users, _isParentView, organizationID, function (note) {
                $('#tblNotesAdditional tbody').empty();
                var html;
                for (var i = 0; i < note.length; i++) {
                    var attachmentString = note[i].Attachments.length > 0 ? "   <i class='fa fa-paperclip'></i>" : "";

                    if (!_isParentView && (_isAdmin || note[i].CreatorID == _mainFrame.Ts.System.User.UserID || _mainFrame.Ts.System.User.CanEditCompany)) {
                        html = '<td><i class="fa fa-edit editNote"></i></td><td><i class="fa fa-trash-o deleteNote"></i></td><td>' + note[i].Owner + '</td><td>' + note[i].Title + attachmentString + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td>';
                    } else
                    {
                        html = '<td>' + note[i].Owner + '</td><td>' + note[i].Title + attachmentString + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td>';
                    }
                    if (note[i].ProductFamilyID != null) {
                        html += '<td>' + note[i].ProductFamily + '</td>';
                    }
                    else {
                        html += '<td>Unassigned</td>';
                    }

                    html += '<td>' + note[i].ActivityTypeString + '</td>';
                    if (note[i].DateOccurred != null) {
                        html += '<td>' + note[i].DateOccurred.toDateString() + '</td>';
                    }
                    else {
                        html += '<td></td>';
                    }

                    html += "<td><i class='fa fa-plus addTask' title='Add Task'></i></td>";

                    $('<tr>').addClass("viewNote")
                    .attr("id", note[i].NoteID)
                    .html(html)
                    .data("attachments", note[i].Attachments)
                    .data("description", note[i].Description)
                    .appendTo('#tblNotesAdditional > tbody:last');
                    //$('#tblNotes > tbody:last').append('<tr id=' + note[i].NoteID + ' class="viewNote"><td><i class="glyphicon glyphicon-edit editNote"></i></td><td><i class="glyphicon glyphicon-trash deleteNote"></i></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td></tr>').data('description',note[i].Description);
                    if (noteID != null && noteID == note[i].NoteID) {
                        $('.noteDesc').html("<h4>Description</h4> <p>" + note[i].Description + "</p>");
                        BuildFileDescription(note[i].Attachments);
                        $('#noteDescBox').show();
                    }
                }
            });
        }
        else {
            _mainFrame.Ts.Services.Customers.LoadNotes2(organizationID, _mainFrame.Ts.ReferenceTypes.Users, _isParentView, function (note) {
                $('#tblNotesAdditional tbody').empty();
                var html;
                for (var i = 0; i < note.length; i++) {
                    var attachmentString = note[i].Attachments.length > 0 ? "   <i class='fa fa-paperclip'></i>" : "";

                    html = '<td></td><td>' + note[i].Owner + '</td><td>' + note[i].Title + attachmentString + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td>';

                    html += '<td>' + note[i].ActivityTypeString + '</td>';
                    if (note[i].DateOccurred != null) {
                        html += '<td>' + note[i].DateOccurred.toDateString() + '</td>';
                    }
                    else {
                        html += '<td></td>';
                    }

                    html += "<td><i class='fa fa-plus addTask' title='Add Task'></i></td>";

                    $('<tr>').addClass("viewNote")
                    .attr("id", note[i].NoteID)
                    .data("attachments", note[i].Attachments)
                    .html(html)
                    .data("description", note[i].Description)
                    .appendTo('#tblNotesAdditional > tbody:last');

                    //$('#tblNotes > tbody:last').append('<tr id=' + note[i].NoteID + ' class="viewNote"><td><i class="glyphicon glyphicon-edit editNote"></i></td><td><i class="glyphicon glyphicon-trash deleteNote"></i></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td></tr>').data('description',note[i].Description);
                    if (noteID != null && noteID == note[i].NoteID) {
                        $('.noteDesc').html("<h4>Description</h4> <p>" + note[i].Description + "</p>");
                        BuildFileDescription(note[i].Attachments);
                        $('#noteDescBox').show();
                    }
                }
            });
        }
    }

    var ellipseString = function (text, max) { return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text; };

    _mainFrame.Ts.Services.Customers.GetOrganizationTickets2(organizationID, 0, _isParentView, function (e) {
        $('#openTicketCount').text("Open Tickets: " + e);
    });

    function LoadHistory(start) {

        if (start == 1)
            $('#tblHistory tbody').empty();

        _mainFrame.Ts.Services.Customers.LoadHistory(organizationID, start, function (history) {
            for (var i = 0; i < history.length; i++) {
                $('<tr>').html('<td>' + history[i].DateCreated.localeFormat(_mainFrame.Ts.Utils.getDateTimePattern()) + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td>')
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

    function LoadFiles() {
        $('#tblFiles tbody').empty();
        if (_mainFrame.Ts.System.Organization.UseProductFamilies) {
            _mainFrame.Ts.Services.Customers.LoadFilesByUserRights(organizationID, _mainFrame.Ts.ReferenceTypes.Organizations, _isParentView, function (files) {
                var count = (files) ? files.length : 0;
                $('#a-files').text('Files (' + count + ')');
                var html;
                for (var i = 0; i < files.length; i++) {
                    if (!_isParentView) {
                        html = '<td><i class="fa fa-trash-o delFile"></i></td><td class="viewFile">' + files[i].FileName + '</td><td>' + files[i].Description + '</td><td>' + files[i].CreatorName + '</td><td>' + files[i].DateCreated.toDateString() + '</td>';
                    } else {
                        html = '<td></td><td class="viewFile">' + files[i].FileName + '</td><td>' + files[i].Description + '</td><td>' + files[i].CreatorName + '</td><td>' + files[i].DateCreated.toDateString() + '</td>';
                    }
                    if (files[i].ProductFamilyID != null) {
                        html += '<td>' + files[i].ProductFamily + '</td>';
                    }
                    else {
                        html += '<td>Unassigned</td>';
                    }
                    var tr = $('<tr>').attr('id', files[i].AttachmentID).html(html).appendTo('#tblFiles > tbody:last');
                    //$('#tblFiles > tbody:last').appendTo('<tr id=' +  + '></tr>');
                }
            });
        } else {
            _mainFrame.Ts.Services.Customers.LoadFiles2(organizationID, _mainFrame.Ts.ReferenceTypes.Organizations, _isParentView, function (files) {
                var html;
                for (var i = 0; i < files.length; i++) {
                    if (!_isParentView) {
                        html = '<td><i class="fa fa-trash-o delFile"></i></td><td class="viewFile">' + files[i].FileName + '</td><td>' + files[i].Description + '</td><td>' + files[i].CreatorName + '</td><td>' + files[i].DateCreated.toDateString() + '</td>';
                    } else {
                        html = '<td></td><td class="viewFile">' + files[i].FileName + '</td><td>' + files[i].Description + '</td><td>' + files[i].CreatorName + '</td><td>' + files[i].DateCreated.toDateString() + '</td>';
                    }
                    var tr = $('<tr>').attr('id', files[i].AttachmentID).html(html).appendTo('#tblFiles > tbody:last');
                    //$('#tblFiles > tbody:last').appendTo('<tr id=' +  + '></tr>');
                }
            });
        }
    }

    function LoadRatings(ratingOption, start) {

        if (start == 1)
            $('#tblRatings tbody').empty();
        _mainFrame.Ts.Services.Customers.LoadAgentRatings2(organizationID, ratingOption, $('#tblRatings tbody > tr').length + 1, _mainFrame.Ts.ReferenceTypes.Organizations, $('#ddlRatingProductFamily').val(), function (ratings) {
            var agents = "";
            for (var i = 0; i < ratings.length; i++) {
                for (var j = 0; j < ratings[i].users.length; j++) {
                    if (j != 0)
                        agents = agents + ", ";

                    agents = agents + '<a href="#" target="_blank" onclick="_mainFrame.Ts.MainPage.openUser(' + ratings[i].users[j].UserID + '); return false;">' + ratings[i].users[j].FirstName + ' ' + ratings[i].users[j].LastName + '</a>';
                }

                var tr = $('<tr>')
                //.html('<td><a href="' + _mainFrame.Ts.System.AppDomain + '?TicketNumber=' + ratings[i].rating.TicketNumber + '" target="_blank" onclick="_mainFrame.Ts.MainPage.openTicket(' + ratings[i].rating.TicketNumber + '); return false;">Ticket ' + ratings[i].rating.TicketNumber + '</a></td><td>' + agents + '</td><td>' + ratings[i].reporter.FirstName + ' ' + ratings[i].reporter.LastName + '</td><td>' + ratings[i].rating.DateCreated.toDateString() + '</td><td>' + ratings[i].rating.RatingText + '</td><td>' + (ratings[i].rating.Comment === null ? "None" : ratings[i].rating.Comment) + '</td>')
                    .html('<td><a href="' + _mainFrame.Ts.System.AppDomain + '?TicketNumber=' + ratings[i].rating.TicketNumber + '" target="_blank" onclick="_mainFrame.Ts.MainPage.openTicket(' + ratings[i].rating.TicketNumber + '); return false;">' + ratings[i].rating.TicketNumber + '</a></td><td>' + agents + '</td><td><a href="#" onclick="_mainFrame.Ts.MainPage.openNewContact(' + ratings[i].reporter.UserID + '); return false;">' + ratings[i].reporter.FirstName + ' ' + ratings[i].reporter.LastName + '</a></td><td>' + ratings[i].rating.DateCreated.toDateString() + '</td><td>' + ratings[i].rating.RatingText + '</td><td>' + (ratings[i].rating.Comment === null ? "None" : ratings[i].rating.Comment) + '</td>')
                    .appendTo('#tblRatings > tbody:last');

                agents = "";
            }
        });

        _mainFrame.Ts.Services.Organizations.GetAgentRatingOptions(_mainFrame.Ts.System.Organization.OrganizationID, function (o) {
            if (o != null) {
                if (o.PositiveImage)
                    $('#positiveImage').attr('src', o.PositiveImage);
                if (o.NeutralImage)
                    $('#neutralImage').attr('src', o.NeutralImage);
                if (o.NegativeImage)
                    $('#negativeImage').attr('src', o.NegativeImage);
            }
        });

        _mainFrame.Ts.Services.Customers.LoadRatingPercents2(organizationID, _mainFrame.Ts.ReferenceTypes.Organizations, $('#ddlRatingProductFamily').val(), function (results) {
            $('#negativePercent').text(results[0] + "%");
            $('#neutralPercent').text(results[1] + "%");
            $('#positivePercent').text(results[2] + "%");
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

    $('#ddlRatingProductFamily').change(function () {
        LoadRatings(ratingFilter, 1);
    });

    function LoadPhoneNumbers(reload) {
        $('#phonePanel').empty();
        _mainFrame.Ts.Services.Customers.LoadPhoneNumbers(organizationID, _mainFrame.Ts.ReferenceTypes.Organizations, function (phone) {
            for (var i = 0; i < phone.length; i++) {
                $('#phonePanel').append("<div class='form-group content'> \
                                        <label for='inputName' class='col-xs-4 control-label'>" + phone[i].PhoneTypeName + "</label> \
                                        <div class='col-xs-5 '> \
                                            <p class='form-control-static '><a href='tel:"+ phone[i].Number + "'>" + phone[i].Number + "</a>" + ((phone[i].Extension != null && phone[i].Extension != '') ? ' Ext:' + phone[i].Extension : '') + "</p> \
                                        </div> \
                                        <div id='editmenu' class='col-xs-2 hiddenmenu'> \
                                            <p class='form-control-static'> \
                                            <a href='' id='" + phone[i].PhoneID + "' class='editphone'><span class='fa fa-pencil'></span></a>\
                                            <a href='' id='" + phone[i].PhoneID + "' class='delphone'><span class='fa fa-trash-o'></span></a/>\
                                            </p> \
                                        </div> \
                                    </div>");
            }
            if (reload != undefined)
                $("#phonePanel #editmenu").toggleClass("hiddenmenu");
        });
    }

    function LoadPhoneTypes() {
        _mainFrame.Ts.Services.Customers.LoadPhoneTypes(organizationID, function (pt) {
            for (var i = 0; i < pt.length; i++) {
                $('<option>').attr('value', pt[i].PhoneTypeID).text(pt[i].Name).data('o', pt[i]).appendTo('#phoneType');
            }
        });
    }

    function LoadProductTypes() {
        $('#productProduct').empty();
        _mainFrame.Ts.Services.Customers.LoadProductTypes(function (pt) {
            for (var i = 0; i < pt.length; i++) {
                if (i == 0)
                    LoadProductVersions(pt[i].ProductID, -1);
                $('<option>').attr('value', pt[i].ProductID).text(pt[i].Name).data('o', pt[i]).appendTo('#productProduct');
            }
        });
    }

    function LoadProductVersions(productID, selVal) {
        $("#productVersion").empty();

        _mainFrame.Ts.Services.Customers.LoadProductVersions(productID, function (pt) {
            $('<option>').attr('value', '-1').text('Unassigned').appendTo('#productVersion');
            for (var i = 0; i < pt.length; i++) {
                var opt = $('<option>').attr('value', pt[i].ProductVersionID).text(pt[i].VersionNumber).data('o', pt[i]);
                if (pt[i].ProductVersionID == selVal)
                    opt.attr('selected', 'selected');
                opt.appendTo('#productVersion');
            }
        });
    }

    function LoadSlaLevels(productID, selVal) {
        $("#slaLevel").empty();

        _mainFrame.Ts.Services.Customers.LoadSlaLevels(function (pt) {
            $('<option>').attr('value', '-1').text('Unassigned').appendTo('#slaLevel');
            for (var i = 0; i < pt.length; i++) {
                var opt = $('<option>').attr('value', pt[i].SlaLevelID).text(pt[i].Name).data('o', pt[i]);
                //if (pt[i].ProductVersionID == selVal)
                //    opt.attr('selected', 'selected');
                opt.appendTo('#slaLevel');
            }
        });
    }

    function LoadChildren() {
        $(".childrenList").empty();

        _mainFrame.Ts.Services.Customers.LoadChildren(organizationID, function (items) {
            var container = $('.childrenList');
            for (var i = 0; i < items.length; i++) {
                appendItem(container, JSON.parse(items[i]));
            }
        });
    }

    $('.childrenList').on('click', '.companylink', function (e) {
        e.preventDefault();

        var id = $(this).data('organizationid');
        _mainFrame.Ts.System.logAction('Customer Detail - View Child Company');
        _mainFrame.Ts.MainPage.openNewCustomer(id);

        _mainFrame.Ts.Services.Customers.UpdateRecentlyViewed('o' + id, function (resultHtml) {

        });

    });

    function LoadContacts(start) {
        start = start || 0;
        showContactsLoadingIndicator();
        $('.userList').fadeTo(200, 0.5);
        _mainFrame.Ts.Services.Customers.LoadContacts2(organizationID, $('#cbActive').prop('checked'), _isParentView, start, function (users) {
            $('.userList').fadeTo(0, 1);
            if (start == 0) {
                $('.userList').empty();
                if (users.length < 1) {
                    $('.contacts-empty').show();
                } else {
                    $('.userList').append(users);
                }
            }
            else {
                if (users.length < 1) {
                    $('.contacts-done').show();
                } else {
                    $('.userList').append(users);
                }
            }
            //$('.userList').append(users);
            $('.contacts-loading').hide();
            _isLoadingContacts = false;

            //for (var i = 0; i < users.length; i++) {
            //    $('<a>').attr('class', 'list-group-item').text(users[i].FirstName + ' ' + users[i].LastName).appendTo('.userList');
            //}
        });
    }

    function showContactsLoadingIndicator() {
        _isLoadingContacts = true;
        $('.contacts-loading').show();
    }

    $('#tblSLAViolations').on('click', '.slaHeader', function (e) {
        e.preventDefault();
        _slaSortColumn = $(this).text();

        if (_slaSortColumn.toLowerCase() == "ticket #") {
            _slaSortColumn = "TicketNumber";
        } else if (_slaSortColumn.toLowerCase() == "violation date") {
            _slaSortColumn = "DateViolated";
        } else if (_slaSortColumn.toLowerCase() == "sla name") {
            _slaSortColumn = "LevelName";
        } else if (_slaSortColumn.toLowerCase() == "ticket type") {
            _slaSortColumn = "SlaTicketType";
        } else if (_slaSortColumn.toLowerCase() == "severity") {
            _slaSortColumn = "SlaSeverity";
        }

        var sortIcon = $(this).children(i);

        if (sortIcon.length > 0) {
            if (sortIcon.hasClass('fa-sort-asc')) {
                _slaSortDirection = 'DESC'
            }
            else {
                _slaSortDirection = 'ASC'
            }
            sortIcon.toggleClass('fa-sort-asc fa-sort-desc');
        }
        else {
            $('.slaHeader').children(i).remove();
            var newSortIcon = $('<i>')
                .addClass('fa fa-sort-asc')
                .appendTo($(this));
            _slaSortDirection = 'ASC';
            switch (_slaSortColumn.toLowerCase()) {
                case "ticketid":
                case "ticketnumber":
                case "violationdate":
                    newSortIcon.toggleClass('fa-sort-asc fa-sort-desc');
                    _slaSortDirection = 'DESC';
            }
        }
        LoadSLAViolationsGrid();
    });

    function LoadSLATriggersGrid() {
        $('#tblSLATriggers tbody').empty();

        _mainFrame.Ts.Services.Customers.LoadSlaTriggers(_orgParentId, organizationID, _slaSortColumn, _slaSortDirection, function (slaTriggers) {
            for (var i = 0; i < slaTriggers.length; i++) {
                var html;

                if (!_isParentView && (_mainFrame.Ts.System.User.CanEditCompany || _isAdmin)) {
                    html = '<td class="col-md-3">' + slaTriggers[i].LevelName + '</td><td>' + slaTriggers[i].TicketType + '</td><td>' + slaTriggers[i].Severity + '</td><td>' + slaTriggers[i].SLAType + '</td>';
                }
                else {
                    html = '<td>' + slaTriggers[i].LevelName + '</td><td>' + slaTriggers[i].TicketType + '</td><td>' + slaTriggers[i].Severity + '</td><td>' + slaTriggers[i].SLAType + '</td>';
                }
                var tr = $('<tr>')
                .attr('id', slaTriggers[i].SlaLevelId)
                .html(html)
                .appendTo('#tblSLATriggers > tbody:last');
            }

            $('.slatriggers-loading').hide();
            $('.slatriggers-empty').hide();
            if (slaTriggers.length == 0) {
                $('.slatriggers-empty').show();
            }
        });
    }

    function LoadSLAViolationsGrid() {
        $('#tblSLAViolations tbody').empty();

        _mainFrame.Ts.Services.Customers.LoadSlaViolations(organizationID, _slaSortColumn, _slaSortDirection, function (slas) {
            for (var i = 0; i < slas.length; i++) {
                var html;

                if (!_isParentView && (_mainFrame.Ts.System.User.CanEditCompany || _isAdmin)) {
                    html = '<td><a href="#" class="slaView" id="' + slas[i].TicketId + '">' + slas[i].TicketNumber + '</a></td><td>' + slas[i].Violation + '</td><td>' + slas[i].DateViolated + '</td><td>' + slas[i].LevelName + '</td><td>' + slas[i].TicketType + '</td><td>' + slas[i].Severity + '</td>';
                }
                else {
                    html = '<td></td><td></td><td><a href="#" class="slaView" id="' + slas[i].TicketId + '">' + slas[i].TicketNumber + '</a></td><td>' + slas[i].Violation + '</td><td>' + slas[i].DateViolated + '</td><td>' + slas[i].LevelName + '</td><td>' + slas[i].TicketType + '</td><td>' + slas[i].Severity + '</td>';
                }
                var tr = $('<tr>')
                .attr('id', slas[i].TicketId)
                .html(html)
                .appendTo('#tblSLAViolations > tbody:last');
            }

            $('.sla-loading').hide();
            $('.sla-empty').hide();
            if (slas.length == 0) {
                $('.sla-empty').show();
            }
        });
    }

    function LoadTasks() {

        parent.Ts.Services.Task.GetCustomerTasks(0, 20, organizationID, function (tasks) {
            var data = { taskList: tasks };
            var source = $("#company-tasks-template").html();
            var template = Handlebars.compile(source);
            $("#tasks").html(template(data));
        });
    }

    $('#tblSLAViolations').on('click', '.slaView', function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Customer Detail - View Ticket Sla Violated');
        var ticketId = $(this).attr('id');
        openTicketWindow(ticketId);

    });

    function LoadProducts() {
        if (!_productHeadersAdded) {
            _mainFrame.Ts.Services.Customers.LoadcustomProductHeaders(function (headers) {
                for (var i = 0; i < headers.length; i++) {
                    $('#tblProducts th:last').after('<th>' + headers[i] + '</th>');
                }
                _productHeadersAdded = true;
                if (headers.length > 5) {
                    $('#productsContainer').addClass('expandProductsContainer');
                }
            });
        }

        $('#tblProducts tbody').empty();
        _mainFrame.Ts.Services.Customers.LoadProducts2(organizationID, _productsSortColumn, _productsSortDirection, _isParentView, function (product) {
            for (var i = 0; i < product.length; i++) {
                var customfields = "";
                for (var p = 0; p < product[i].CustomFields.length; p++) {
                    customfields = customfields + "<td>" + product[i].CustomFields[p] + "</td>";
                }

                var html;

                if (!_isParentView && (_mainFrame.Ts.System.User.CanEditCompany || _isAdmin)) {
                    html = '<td><i class="fa fa-edit productEdit"></i></td><td><i class="fa fa-trash-o productDelete"></i></td><td><a href="#" class="productView">' + product[i].ProductName + '</a></td><td><a href="#" class="productVersionView">' + product[i].VersionNumber + '</a></td><td>' + product[i].SupportExpiration + '</td><td>' + product[i].VersionStatus + '</td><td>' + product[i].IsReleased + '</td><td>' + product[i].ReleaseDate + '</td><td>' + product[i].SlaAssigned + '</td><td>' + product[i].DateCreated + '</td>' + customfields;
                }
                else {
                    html = '<td></td><td></td><td><a href="#" class="productView">' + product[i].ProductName + '</a></td><td><a href="#" class="productVersionView">' + product[i].VersionNumber + '</a></td><td>' + product[i].SupportExpiration + '</td><td>' + product[i].VersionStatus + '</td><td>' + product[i].IsReleased + '</td><td>' + product[i].SlaAssigned + '</td><td>' + product[i].ReleaseDate + '</td><td>' + product[i].DateCreated + '</td>' + customfields
                }
                var tr = $('<tr>')
                .attr('id', product[i].OrganizationProductID)
                .html(html)
                .appendTo('#tblProducts > tbody:last');


                //$('#tblProducts > tbody:last').append('<tr><td><a href="#" id='+ product.ProductID +'><i class="glyphicon glyphicon-edit productEdit"></i></td><td><i class="glyphicon glyphicon-trash productDelete"></i></td><td><i class="fa fa-folder-open productView"></i></td><td>' + product[i].ProductName + '</td><td>' + product[i].VersionNumber + '</td><td>' + product[i].SupportExpiration + '</td><td>' + product[i].VersionStatus + '</td><td>' + product[i].IsReleased + '</td><td>' + product[i].ReleaseDate + '</td><td></td></tr>');
            }

            $('.products-loading').hide();
            $('.products-empty').hide();
            if (product.length == 0) {
                $('.products-empty').show();
            }
        });

    }

    function LoadInventory() {
        $('.assetList').empty();
        _mainFrame.Ts.Services.Customers.LoadAssets2(organizationID, _mainFrame.Ts.ReferenceTypes.Organizations, _isParentView, function (assets) {
            $('.assetList').append(assets)
            _mainFrame.Ts.Services.Customers.LoadContactAssets2(organizationID, _isParentView, function (assets) {
                $('.assetList').append(assets)
            });
        });
    }

    function LoadCustomControls(refType) {
        _mainFrame.Ts.Services.Customers.LoadCustomControls(refType, function (html) {
            $('#customProductsControls').append(html);

            $('#customProductsControls .datepicker').datetimepicker({ pickTime: false, format: _dateFormat });
            $('#customProductsControls .datetimepicker').datetimepicker({});
            $('#customProductsControls .timepicker ').datetimepicker({ pickDate: false });
        });
    }

    function LoadReminderUsers() {
        var users = _mainFrame.Ts.Cache.getUsers();
        if (users != null) {
            for (var i = 0; i < users.length; i++) {
                var option = $('<option>').attr('value', users[i].UserID).text(users[i].Name).data('o', users[i]).appendTo('#reminderUsers');
                if (_mainFrame.Ts.System.User.UserID === users[i].UserID) { option.attr('selected', 'selected'); }
            }
        }
    }

    function UpdateRecentView() {
        _mainFrame.Ts.Services.Customers.UpdateRecentlyViewed("o" + organizationID, function (resultHtml) {
            if (window.parent.document.getElementById('iframe-mniCustomers'))
                window.parent.document.getElementById('iframe-mniCustomers').contentWindow.refreshPage();
        });

    }

    createTestChart();
    function createTestChart() {
        var greenLimit, yellowLimit;

        _mainFrame.Ts.Services.Customers.LoadChartData2(organizationID, true, _isParentView, function (chartString) {

            var chartData = [];
            var dummy = chartString.split(",");
            var openCount = 0;

            for (var i = 0; i < dummy.length; i++) {
                chartData.push([dummy[i], parseFloat(dummy[i + 1])]);
                i++
            }

            if (dummy.length == 1) {
                //chartData.pop();
                //chartData.push(["No Open Tickets", 0]);
                //$('#openChart').text("No Open Tickes").addClass("text-center");
                $('#openChart').html("No Open Tickets<br/><img class='img-responsive' src=../Images/nochart.jpg>").addClass("text-center  chart-header").attr("title", "No Open Tickets");
            }
            else {
                for (var i = 0; i < chartData.length; i++) {
                    openCount = openCount + chartData[i][1];
                }
                $('#openChart').highcharts({
                    chart: {
                        plotBackgroundColor: null,
                        plotBorderWidth: null,
                        plotShadow: false,
                        height: 250,
                    },
                    credits: {
                        enabled: false
                    },
                    title: {
                        text: 'Open Tickets ' + openCount,
                        style: {
                            "fontSize": "14px"
                        }
                    },
                    tooltip: {
                        pointFormat: '{series.name}: {point.y} - <b>{point.percentage:.0f}%</b>'
                    },
                    plotOptions: {
                        pie: {
                            allowPointSelect: true,
                            cursor: 'pointer',
                            dataLabels: {
                                enabled: false
                            }
                        }
                    },
                    series: [{
                        type: 'pie',
                        name: 'Open Tickets',
                        data: []
                    }]
                });

                var chart = $('#openChart').highcharts();
                chart.series[0].setData(chartData);



            }
        });

        _mainFrame.Ts.Services.Customers.LoadChartData2(organizationID, false, _isParentView, function (chartString) {

            var chartData = [];
            var dummy = chartString.split(",");
            var closedCount = 0;

            for (var i = 0; i < dummy.length; i++) {
                chartData.push([dummy[i], parseFloat(dummy[i + 1])]);
                i++
            }

            if (dummy.length == 1) {
                //chartData.pop();
                //chartData.push(["No Closed Tickets", 0]);
                //$('#closedChart').text("No Closed Tickets").addClass("text-center");
                $('#closedChart').html("No Closed Tickets<br/><img class='img-responsive' src=../Images/nochart.jpg>").addClass("text-center  chart-header").attr("title", "No Closed Tickets");
            }
            else {
                for (var i = 0; i < chartData.length; i++) {
                    closedCount = closedCount + chartData[i][1];
                }
                $('#closedChart').highcharts({
                    chart: {
                        plotBackgroundColor: null,
                        plotBorderWidth: null,
                        plotShadow: false,
                        height: 250,
                    },
                    credits: {
                        enabled: false
                    },
                    title: {
                        text: 'Closed Tickets ' + closedCount,
                        style: {
                            "fontSize": "14px"
                        }

                    },
                    tooltip: {
                        pointFormat: '{series.name}: {point.y} - <b>{point.percentage:.0f}%</b>'
                    },
                    plotOptions: {
                        pie: {
                            allowPointSelect: true,
                            cursor: 'pointer',
                            dataLabels: {
                                enabled: false
                            }
                        }
                    },
                    series: [{
                        type: 'pie',
                        name: 'Closed Tickets',
                        data: []
                    }]
                });

                var chart = $('#closedChart').highcharts();
                chart.series[0].setData(chartData);
            }
        });

        _mainFrame.Ts.Services.Organizations.LoadCDISettings(_mainFrame.Ts.System.Organization.OrganizationID, function (cdi) {
            if (cdi == null) {
                greenLimit = 70;
                yellowLimit = 85
            }
            else {
                greenLimit = cdi.GreenUpperRange == null ? 70 : cdi.GreenUpperRange;
                yellowLimit = cdi.YellowUpperRange == null ? 85 : cdi.YellowUpperRange;
            }
        });

        _mainFrame.Ts.Services.Customers.LoadCDI2(organizationID, _isParentView, function (cdiValue) {
            var chartData = [];
            chartData.push(cdiValue);

            _mainFrame.Ts.Services.Customers.GetCustDistIndexTrend(organizationID, function (trend) {
                $('#csiChart').highcharts({

                    chart: {
                        type: 'gauge',
                        plotBackgroundColor: null,
                        plotBackgroundImage: null,
                        plotBorderWidth: 0,
                        plotShadow: false,
                        height: 250,
                    },

                    title: {
                        useHTML: true,
                        text: 'CDI ' + trend,
                        style: {
                            "fontSize": "14px"
                        }
                    },
                    pane: {
                        startAngle: -150,
                        endAngle: 150,
                        background: [{
                            backgroundColor: {
                                linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                                stops: [
                                    [0, '#FFF'],
                                    [1, '#333']
                                ]
                            },
                            borderWidth: 0,
                            outerRadius: '109%'
                        }, {
                            backgroundColor: {
                                linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                                stops: [
                                    [0, '#333'],
                                    [1, '#FFF']
                                ]
                            },
                            borderWidth: 1,
                            outerRadius: '107%'
                        }, {
                            // default background
                        }, {
                            backgroundColor: '#DDD',
                            borderWidth: 0,
                            outerRadius: '105%',
                            innerRadius: '103%'
                        }]
                    },

                    // the value axis
                    yAxis: {
                        min: 0,
                        max: 100,

                        minorTickInterval: 'auto',
                        minorTickWidth: 1,
                        minorTickLength: 10,
                        minorTickPosition: 'inside',
                        minorTickColor: '#666',

                        tickPixelInterval: 30,
                        tickWidth: 2,
                        tickPosition: 'inside',
                        tickLength: 10,
                        tickColor: '#666',
                        labels: {
                            step: 2,
                            rotation: 'auto'
                        },
                        title: {
                            text: ''
                        },
                        plotBands: [{
                            from: 0,
                            to: greenLimit,
                            color: '#55BF3B' // green
                        }, {
                            from: greenLimit,
                            to: yellowLimit,
                            color: '#DDDF0D' // yellow
                        }, {
                            from: yellowLimit,
                            to: 100,
                            color: '#DF5353' // red
                        }]
                    },
                    credits: {
                        enabled: false
                    },
                    series: [{
                        name: 'CDI',
                        data: [],
                        tooltip: {
                            valueSuffix: ' Rating'
                        }
                    }]

                },
                function (chart) {
                    if (!chart.renderer.forExport) {

                    }
                });

                var chart = $('#csiChart').highcharts();
                chart.series[0].setData(chartData);
            });



        });
    }
    $("[rel='tooltip']").tooltip();
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        if (e.target.innerHTML == "Tickets") {
            $('#ticketIframe').attr("src", "../../../Frames/TicketTabsAll.aspx?tf_CustomerID=" + organizationID + "&tf_IncludeCompanyChildren=" + _isParentView);
            _viewingContacts = false;
        }
        else if (e.target.innerHTML == "Watercooler") {
            $('#watercoolerIframe').attr("src", "WaterCooler.html?pagetype=2&pageid=" + organizationID);
            _viewingContacts = false;
        }
        else if (e.target.innerHTML == "Details") {
            createTestChart();
            _viewingContacts = false;
        }
        else if (e.target.innerHTML == "Children") {
            LoadChildren();
            _viewingContacts = false;
        }
        else if (e.target.innerHTML == "Contacts") {
            _viewingContacts = true;
            if ($('.userList').is(':empty')){
                LoadContacts();
            }
        }
        else if (e.target.innerHTML == "Notes") {
            LoadNotes();
            LoadNotesAdditional();
            _viewingContacts = false;
        }
        else if (e.target.innerHTML == "Files") {
            LoadFiles();
            _viewingContacts = false;
        }
        else if (e.target.innerHTML == "Products") {
            LoadProducts();
            _viewingContacts = false;
        }
        else if (e.target.innerHTML == "Inventory") {
            LoadInventory();
            _viewingContacts = false;
        }
        else if (e.target.innerHTML == "Ratings") {
            _viewingContacts = false;
            LoadRatings('', 1);
        }
        else if (e.target.innerHTML == "Calendar") {
            $('#calendarIframe').attr("src", "Calendar.html?pagetype=2&pageid=" + organizationID);
            _viewingContacts = false;
        }
        else if (e.target.innerHTML == "SLA") {
            LoadSLATriggersGrid();
            LoadSLAViolationsGrid();
        }
        else if (e.target.innerHTML == "Tasks") {
            LoadTasks();
            _viewingContacts = false;
        }
    })

    $("input[type=text], textarea").autoGrow();

    $('.customProperties, .userProperties, #customProductsControls').on('keydown', '.number', function (event) {
        // Allow only backspace and delete
        if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 190 || event.keyCode == 109 || event.keyCode == 173 || (event.keyCode >= 96 && event.keyCode <= 105)) {
            // let it happen, don't do anything
        }
        else {
            // Ensure that it is a number and stop the keypress
            if (event.keyCode < 48 || event.keyCode > 57) {
                event.preventDefault();
            }
        }
    });

    $('#company-overview').on('keydown', '.number', function (event) {
        // Allow only backspace and delete
        if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 190 || event.keyCode == 109 || event.keyCode == 173 || (event.keyCode >= 96 && event.keyCode <= 105)) {
            // let it happen, don't do anything
        }
        else {
            // Ensure that it is a number and stop the keypress
            if (event.keyCode < 48 || event.keyCode > 57) {
                event.preventDefault();
            }
        }
    });

    $('.userProperties p').toggleClass("editable");

    $('#alertSnooze').click(function (e) {
        _mainFrame.Ts.Services.Customers.SnoozeAlert(organizationID, _mainFrame.Ts.ReferenceTypes.Organizations);
        _mainFrame.Ts.System.logAction('Customer Detail - Snooze Alert');
        $('#modalAlert').modal('hide');
    });

    $('#alertDismiss').click(function (e) {
        _mainFrame.Ts.Services.Customers.DismissAlert(organizationID, _mainFrame.Ts.ReferenceTypes.Organizations);
        _mainFrame.Ts.System.logAction('Customer Detail - Dismiss Alert');
        $('#modalAlert').modal('hide');
    });

    $('.maincontainer').bind('scroll', function () {
        if (_viewingContacts) {
            if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
                if (_isLoadingContacts == true) return;
                if ($('.contacts-done').is(':visible')) return;

                LoadContacts($('.list-group-item').length + 1);
            }

            if ($(this).scrollTop() > 100) {
                $('.scrollup').fadeIn();
            } else {
                $('.scrollup').fadeOut();
            }
        }
    });

    $('.scrollup').click(function () {
        $('.frame-container').animate({
            scrollTop: 0
        }, 600);
        return false;
    });

    $('.tab-content').bind('scroll', function () {
        if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
            LoadRatings(ratingFilter, $('#tblRatings tbody > tr').length + 1);
        }
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


    WatsonCustomer(organizationID);

});

//var initEditor = function (element, init) {
//    _mainFrame.Ts.Settings.System.read('EnableScreenR', 'True', function (enableScreenR) {
//        var FontName = '';
//        var FontSize = '';
//        var editorOptions = {
//            plugins: "autoresize paste link code textcolor",
//            toolbar1: "link unlink | undo redo removeformat | cut copy paste pastetext | code | outdent indent | bullist numlist",
//            toolbar2: "alignleft aligncenter alignright alignjustify | forecolor backcolor | fontselect fontsizeselect | bold italic underline strikethrough blockquote",
//            branding: false,
//            statusbar: false,
//            gecko_spellcheck: true,
//            extended_valid_elements: "a[accesskey|charset|class|coords|dir<ltr?rtl|href|hreflang|id|lang|name|onblur|onclick|ondblclick|onfocus|onkeydown|onkeypress|onkeyup|onmousedown|onmousemove|onmouseout|onmouseover|onmouseup|rel|rev|shape<circle?default?poly?rect|style|tabindex|title|target|type],script[charset|defer|language|src|type]",
//            content_css: "../Css/jquery-ui-latest.custom.css,../Css/editor.css",
//            body_class: "ui-widget ui-widget-content",

//            convert_urls: true,
//            remove_script_host: false,
//            relative_urls: false,
//            template_external_list_url: "tinymce/jscripts/template_list.js",
//            external_link_list_url: "tinymce/jscripts/link_list.js",
//            external_image_list_url: "tinymce/jscripts/image_list.js",
//            media_external_list_url: "tinymce/jscripts/media_list.js",
//            menubar: false,
//            moxiemanager_image_settings: {
//                moxiemanager_rootpath: "/" + _mainFrame.Ts.System.Organization.OrganizationID + "/images/",
//                extensions: 'gif,jpg,jpeg,png'
//            },
//            paste_data_images: true,
//            images_upload_url: "/Services/UserService.asmx/SaveTinyMCEPasteImage",
//            setup: function (ed) {
//                ed.on('BeforeSetContent', function (e) {
//                    if (e.content == '' && !e.initial) {
//                        setTimeout(function () {
//                            var content = '<p style="{0} {1}"></p>';
//                            if (FontSize != "")
//                                content = content.replace(/\{0\}/g, "font-size: " + FontSize + ";");
//                            else
//                                content = content.replace(/\{0\}/g, "");
//                            if (FontName != "")
//                                content = content.replace(/\{1\}/g, "font-family: " + FontName + ";");
//                            else
//                                content = content.replace(/\{1\}/g, "");
//                            ed.setContent(ed.getContent() + content, { no_events: true });
//                        }, 10);
//                    }
//                });

//                ed.on('init', function (e) {
//                    _mainFrame.Ts.System.refreshUser(function () {
//                        if (_mainFrame.Ts.System.User.FontFamilyDescription != "Unassigned") {
//                            FontName = GetTinyMCEFontName(_mainFrame.Ts.System.User.FontFamily);
//                        }
//                        else if (_mainFrame.Ts.System.Organization.FontFamily != "Unassigned") {
//                            FontName = GetTinyMCEFontName(_mainFrame.Ts.System.Organization.FontFamily);
//                        }

//                        if (_mainFrame.Ts.System.User.FontSize != "0") {
//                            FontSize = GetTinyMCEFontSize(_mainFrame.Ts.System.User.FontSize);
//                        }
//                        else if (_mainFrame.Ts.System.Organization.FontSize != "0") {
//                            FontSize = GetTinyMCEFontSize(_mainFrame.Ts.System.Organization.FontSize);
//                        }
//                        var content = '<p style="{0} {1}"></p>';
//                        if (FontSize != "")
//                            content = content.replace(/\{0\}/g, "font-size: " + FontSize + ";");
//                        else
//                            content = content.replace(/\{0\}/g, "");
//                        if (FontName != "")
//                            content = content.replace(/\{1\}/g, "font-family: " + FontName + ";");
//                        else
//                            content = content.replace(/\{1\}/g, "");
//                        ed.setContent(ed.getContent() + content, { no_events: true });



//                    });
//                    _insertedKBTicketID = null;
//                });

//                ed.on('paste', function (ed, e) {
//                    setTimeout(function () { ed.execCommand('mceAutoResize'); }, 1000);
//                });
//            }
//            , oninit: init
//        };
//        $(element).tinymce(editorOptions);
//    });
//}

var appendCustomValues = function (fields) {

    _mainFrame.Ts.Services.Customers.GetCustomFieldCategories(function (categories) {
        var noCatfields = 0;
        for (var i = 0; i < fields.length; i++) {

            if (fields[i].CustomFieldCategoryID == -1) {
                noCatfields++;
                var item = null;
                var field = fields[i];
                var div = $('<div>').addClass('form-group').data('field', field);

                $('<label>')
                  .addClass('col-xs-4 control-label')
                  .text(field.Name)
                  .appendTo(div);

                switch (field.FieldType) {
                    case _mainFrame.Ts.CustomFieldType.Text: appendCustomEdit(field, div); break;
                    case _mainFrame.Ts.CustomFieldType.Date: appendCustomEditDate(field, div); break;
                    case _mainFrame.Ts.CustomFieldType.Time: appendCustomEditTime(field, div); break;
                    case _mainFrame.Ts.CustomFieldType.DateTime: appendCustomEditDateTime(field, div); break;
                    case _mainFrame.Ts.CustomFieldType.Boolean: appendCustomEditBool(field, div); break;
                    case _mainFrame.Ts.CustomFieldType.Number: appendCustomEditNumber(field, div); break;
                    case _mainFrame.Ts.CustomFieldType.PickList: appendCustomEditCombo(field, div); break;
                    default:
                }


                //if (i < (fields.length / 2))
                //    containerL.append(div);
                //else
                if (noCatfields % 2)
                    $('#customPropertiesL').append(div);
                else
                    $('#customPropertiesR').append(div);
            }
        }

        if (noCatfields == 0) {
            $('#customPropRow').hide();
        }

        for (var c = 0; c < categories.length; c++) {
            var custom = $('<div>').insertBefore($('#customPropRow'));

            var box = $('<div>').addClass('box').appendTo(custom);
            var header = $('<div>').addClass('box-header').attr('data-toggle', 'collapse').attr('data-target', '#cat' + c).appendTo(box);
            $('<span>').addClass('ui-icon ui-icon-triangle-1-s').appendTo(header);
            var h3title = $('<h3>').text(categories[c].Category).appendTo(header);
            var boxcontent = $('<div>').addClass('box-content in').attr('id', 'cat' + c).appendTo(box);
            var boxrow = $('<div>').addClass('row').appendTo(boxcontent);
            var formh = $('<form>').addClass('form-horizontal').appendTo(boxrow);
            var colxsL = $('<div>').addClass('col-xs-6 customProperties').appendTo(formh);
            var colxsR = $('<div>').addClass('col-xs-6 customProperties').appendTo(formh);
            var fieldcount = 0;

            for (var i = 0; i < fields.length; i++) {

                if (categories[c].CustomFieldCategoryID == fields[i].CustomFieldCategoryID) {
                    var item = null;
                    var field = fields[i];
                    var div = $('<div>').addClass('form-group').data('field', field);

                    $('<label>')
                      .addClass('col-xs-4 control-label')
                      .text(field.Name)
                      .appendTo(div);

                    switch (field.FieldType) {
                        case _mainFrame.Ts.CustomFieldType.Text: appendCustomEdit(field, div); break;
                        case _mainFrame.Ts.CustomFieldType.Date: appendCustomEditDate(field, div); break;
                        case _mainFrame.Ts.CustomFieldType.Time: appendCustomEditTime(field, div); break;
                        case _mainFrame.Ts.CustomFieldType.DateTime: appendCustomEditDateTime(field, div); break;
                        case _mainFrame.Ts.CustomFieldType.Boolean: appendCustomEditBool(field, div); break;
                        case _mainFrame.Ts.CustomFieldType.Number: appendCustomEditNumber(field, div); break;
                        case _mainFrame.Ts.CustomFieldType.PickList: appendCustomEditCombo(field, div); break;
                        default:
                    }

                    if (fieldcount % 2)
                        colxsR.append(div);
                    else
                        colxsL.append(div);
                    fieldcount++;
                    //if (i < (fields.length / 2))
                    //    containerL.append(div);
                    //else
                    //    containerR.append(div);
                }
            }
        }
        $('.customProperties p').toggleClass("editable");
        //$('.box-content').addClass("collapse, in");
        $('[data-toggle="collapse"]').click(function (e) {
            e.preventDefault();
            if ($(this).next().is(':visible')) {
                $(this).find('.ui-icon').addClass('ui-icon-triangle-1-e').removeClass('ui-icon-triangle-1-s');
                $(this).addClass('collapsedCustomCategory');
            }
            else {
                $(this).find('.ui-icon').addClass('ui-icon-triangle-1-s').removeClass('ui-icon-triangle-1-e');
                $(this).removeClass('collapsedCustomCategory');
            }
            var target_element = $(this).attr("data-target");
            $(target_element).collapse('toggle');
            return false;
        });
    });


    if (fields === null || fields.length < 1) {
        $('#customPropertiesL').empty();
        //$('#customPropertiesR').empty();
        return;
    }
    //var containerL = $('#customPropertiesL').empty();
    //var containerR = $('#customPropertiesR').empty();


}

var appendCustomEditCombo = function (field, element) {
    var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);

    var result = $('<p>')
      .text((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : field.Value))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
          e.preventDefault();
          if (!$(this).hasClass('editable'))
              return false;
          var parent = $(this).hide();
          _mainFrame.Ts.System.logAction('Customer Detail - Edit Custom Combobox');
          var container = $('<div>')
            .insertAfter(parent);

          var container1 = $('<div>')
          .addClass('col-xs-9')
          .appendTo(container);

          var fieldValue = parent.closest('.form-group').data('field').Value;
          var select = $('<select>').addClass('form-control').attr('id', field.Name.replace(/\W/g, '')).appendTo(container1);

          var items = field.ListValues.split('|');
          for (var i = 0; i < items.length; i++) {
              var option = $('<option>').text(items[i]).appendTo(select);
              if (fieldValue === items[i]) { option.attr('selected', 'selected'); }
          }

          $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
                $(this).closest('div').remove();
                parent.show();
                $('#customerEdit').removeClass("disabled");
            })
            .insertAfter(container1);

          $('#' + field.Name.replace(/\W/g, '')).on('change', function () {
              var value = $(this).val();
              container.remove();
              _mainFrame.Ts.System.logAction('Customer Detail - Save Custom Combobox Edit');
              if (field.IsRequired && field.IsFirstIndexSelect == true && $(this).find('option:selected').index() < 1) {
                  result.parent().addClass('has-error');
              }
              else {
                  result.parent().removeClass('has-error');
              }
              _mainFrame.Ts.Services.System.SaveCustomValue(field.CustomFieldID, organizationID, value, function (result) {
                  parent.closest('.form-group').data('field', result);
                  parent.text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
                  parent.show();
                  $('#customerEdit').removeClass("disabled");
              }, function () {
                  alert("There was a problem saving your contact property.");
                  $('#customerEdit').removeClass("disabled");
              });
          });
          $('#customerEdit').addClass("disabled");
      });
    var items = field.ListValues.split('|');
    if (field.IsRequired && ((field.IsFirstIndexSelect == true && (items[0] == field.Value || field.Value == null || $.trim(field.Value) === '')) || (field.Value == null || $.trim(field.Value) === ''))) {
        result.parent().addClass('has-error');
    }
}

var appendCustomEditNumber = function (field, element) {
    var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);

    var result = $('<p>')
      .text((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : field.Value))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
          e.preventDefault();
          if (!$(this).hasClass('editable'))
              return false;
          var parent = $(this).hide();
          _mainFrame.Ts.System.logAction('Customer Detail - Edit Custom Number');
          var container = $('<div>')
            .insertAfter(parent);

          var container1 = $('<div>')
          .addClass('col-xs-9')
          .appendTo(container);

          var fieldValue = parent.closest('.form-group').data('field').Value;
          var input = $('<input type="text">')
            .addClass('col-xs-10 form-control number')
            .val(fieldValue)
            .appendTo(container1)
            .focus();

          $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
                $(this).closest('div').remove();
                parent.show();
                $('#customerEdit').removeClass("disabled");
            })
            .insertAfter(container1);
          $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
                var value = input.val();
                container.remove();
                if (field.IsRequired && (value === null || $.trim(value) === '')) {
                    result.parent().addClass('has-error');
                }
                else {
                    result.parent().removeClass('has-error');
                }
                _mainFrame.Ts.System.logAction('Customer Detail - Save Custom Number Edit');
                _mainFrame.Ts.Services.System.SaveCustomValue(field.CustomFieldID, organizationID, value, function (result) {
                    parent.closest('.form-group').data('field', result);
                    parent.text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
                    $('#customerEdit').removeClass("disabled");
                }, function () {
                    alert("There was a problem saving your contact property.");
                    $('#customerEdit').removeClass("disabled");
                });
                parent.show();
            })
            .insertAfter(container1);
          $('#customerEdit').addClass("disabled");
      });
    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        result.parent().addClass('has-error');
    }
}

var appendCustomEditBool = function (field, element) {

    var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);

    var result = $('<p>')
      .text((field.Value === null || $.trim(field.Value) === '' ? 'False' : field.Value))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
          e.preventDefault();
          if (!$(this).hasClass('editable'))
              return false;
          //$('.form-group').prev().show().next().remove();
          _mainFrame.Ts.System.logAction('Customer Detail - Edit Custom Boolean');
          var parent = $(this);
          var value = $(this).text() === 'No' || $(this).text() === 'False' ? true : false;
          _mainFrame.Ts.Services.System.SaveCustomValue(field.CustomFieldID, organizationID, value, function (result) {
              parent.closest('.form-group').data('field', result);
              parent.text((result.Value === null || $.trim(result.Value) === '' ? 'False' : result.Value));
          }, function () {
              alert("There was a problem saving your contact property.");
          });
      });
}

var appendCustomEdit = function (field, element) {

    var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);
	var cssClass = 'form-control-static editable';
	var pattern = /^(?:(?:https?|ftp):\/\/)?(?:(?!(?:10|127)(?:\.\d{1,3}){3})(?!(?:169\.254|192\.168)(?:\.\d{1,3}){2})(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))|(?:(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)(?:\.(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)*(?:\.(?:[a-z\u00a1-\uffff]{2,})))(?::\d{2,5})?(?:\/\S*)?$/;
	var isUrl = false;

	//Check the value is url (valid) and if it does not include a script tag
	if (pattern.test(field.Value) && field.Value.indexOf('<script>') < 0) {
		isUrl = true;
	}

    var result = $('<p>')
      .appendTo(div)
      .click(function (e) {
          if ($(this).has('a') && !$(this).hasClass('editable')) {
              return;
          }
          else {
              e.preventDefault();
              if (!$(this).hasClass('editable'))
                  return false;
              var parent = $(this).hide();
              _mainFrame.Ts.System.logAction('Customer Detail - Edit Custom Textbox');
              var container = $('<div>')
                .insertAfter(parent);

              var container1 = $('<div>')
              .addClass('col-xs-9')
              .appendTo(container);

              var fieldValue = parent.closest('.form-group').data('field').Value;
              var input = $('<input type="text">')
                .addClass('col-xs-10 form-control')
                .val(fieldValue == "Empty" ? "" : fieldValue)
                .appendTo(container1)
                .focus();

              if (field.Mask) {
                  input.mask(field.Mask);
                  input.attr("placeholder", field.Mask);
              }

              $('<i>')
                .addClass('col-xs-1 fa fa-times')
                .click(function (e) {
                    $(this).closest('div').remove();
                    parent.show();
                    $('#customerEdit').removeClass("disabled");
                })
                .insertAfter(container1);
              $('<i>')
                .addClass('col-xs-1 fa fa-check')
                .click(function (e) {
                    var value = input.val();
                    container.remove();
                    if (field.IsRequired && (value === null || $.trim(value) === '')) {
                        result.parent().addClass('has-error');
                    }
                    else {
                        result.parent().removeClass('has-error');
                    }
                    _mainFrame.Ts.System.logAction('Customer Detail - Save Custom Textbox Edit');
                    _mainFrame.Ts.Services.System.SaveCustomValue(field.CustomFieldID, organizationID, value, function (result) {
                        parent.closest('.form-group').data('field', result);
                        parent.html((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : getUrls(result.Value)));
                        $('#customerEdit').removeClass("disabled");
                    }, function () {
                        alert("There was a problem saving your contact property.");
                        $('#customerEdit').removeClass("disabled");
                    });
                    parent.show();
                })
                .insertAfter(container1);
              $('#customerEdit').addClass("disabled");
          }
      });

	//If it is a URL then add the class to it and display it as it is, if not then display the value as text
	if (isUrl) {
		cssClass += ' link';
		result
			.html((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : getUrls(field.Value)))
			.addClass(cssClass);
	} else {
		result.text((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : $("<div>").html(getUrls(field.Value)).text()))
			.addClass(cssClass);
	}

    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        result.parent().addClass('has-error');
    }
}

var appendCustomEditDate = function (field, element) {
    var date = field.Value == null ? null : _mainFrame.Ts.Utils.getMsDate(field.Value);

    var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);

    var result = $('<p>')
      .text((date === null ? 'Unassigned' : date.localeFormat(_mainFrame.Ts.Utils.getDatePattern())))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
          e.preventDefault();
          if (!$(this).hasClass('editable'))
              return false;
          var parent = $(this).hide();
          _mainFrame.Ts.System.logAction('Customer Detail - Edit Custom Date');
          var container = $('<div>')
            .insertAfter(parent);

          var container1 = $('<div>')
          .addClass('col-xs-9')
          .appendTo(container);

          var fieldValue = parent.closest('.form-group').data('field').Value;
          var input = $('<input type="text">')
            .addClass('col-xs-10 form-control')
			.val(fieldValue === null ? '' : moment(fieldValue).format(_mainFrame.Ts.Utils.getDatePattern().toUpperCase()))
            .datetimepicker({ pickTime: false, format: _dateFormat })
            .appendTo(container1)
            .focus();

          $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
                $(this).closest('div').remove();
                parent.show();
                $('#customerEdit').removeClass("disabled");
            })
            .insertAfter(container1);
          $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
                var value = _mainFrame.Ts.Utils.getMsDate(convertToValidDate(input.val()));
                container.remove();
                if (field.IsRequired && (value === null || $.trim(value) === '')) {
                    result.parent().addClass('has-error');
                }
                else {
                    result.parent().removeClass('has-error');
                }
                _mainFrame.Ts.System.logAction('Customer Detail - Save Custom  Date Edit');
                _mainFrame.Ts.Services.System.SaveCustomValue(field.CustomFieldID, organizationID, value, function (result) {
                    parent.closest('.form-group').data('field', result);
                    var date = result.Value === null ? null : _mainFrame.Ts.Utils.getMsDate(result.Value);
                    parent.text((date === null ? 'Unassigned' : date.localeFormat(_mainFrame.Ts.Utils.getDatePattern())))
                    $('#customerEdit').removeClass("disabled");
                }, function () {
                    alert("There was a problem saving your contact property.");
                    $('#customerEdit').removeClass("disabled");
                });
                parent.show();
            })
            .insertAfter(container1);
          $('#customerEdit').addClass("disabled");
      });
    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        result.parent().addClass('has-error');
    }

}

var appendCustomEditDateTime = function (field, element) {
    var date = field.Value == null ? null : _mainFrame.Ts.Utils.getMsDate(field.Value);

    var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);

    var result = $('<p>')
      .text((date === null ? 'Unassigned' : date.format(_mainFrame.Ts.Utils.getDateTimePattern())))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
          e.preventDefault();
          if (!$(this).hasClass('editable'))
              return false;
          var parent = $(this).hide();
          _mainFrame.Ts.System.logAction('Customer Detail - Edit Custom DateTime');
          var container = $('<div>')
            .insertAfter(parent);

          var container1 = $('<div>')
          .addClass('col-xs-9')
          .appendTo(container);

          var fieldValue = parent.closest('.form-group').data('field').Value;
          var input = $('<input type="text">')
            .addClass('col-xs-10 form-control')
            .val(fieldValue === null ? '' : fieldValue.localeFormat(_mainFrame.Ts.Utils.getDateTimePattern()))
            .datetimepicker({
                format: _dateFormat + " hh:mm a"
            })

            .appendTo(container1)
            .focus();

          $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
                $(this).closest('div').remove();
                parent.show();
                $('#customerEdit').removeClass("disabled");
            })
            .insertAfter(container1);
          $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
                var value = _mainFrame.Ts.Utils.getMsDate(convertToValidDateTime(input.val()));
                container.remove();
                if (field.IsRequired && (value === null || $.trim(value) === '')) {
                    result.parent().addClass('has-error');
                }
                else {
                    result.parent().removeClass('has-error');
                }
                _mainFrame.Ts.System.logAction('Customer Detail - Save Custom DateTime Edit');
                _mainFrame.Ts.Services.System.SaveCustomValue(field.CustomFieldID, organizationID, value, function (result) {
                    parent.closest('.form-group').data('field', result);
                    var date = result.Value === null ? null : _mainFrame.Ts.Utils.getMsDate(result.Value);
                    parent.text((date === null ? 'Unassigned' : date.localeFormat(_mainFrame.Ts.Utils.getDateTimePattern())))
                    $('#customerEdit').removeClass("disabled");
                }, function () {
                    alert("There was a problem saving your customer property.");
                    $('#customerEdit').removeClass("disabled");
                });
                parent.show();
            })
            .insertAfter(container1);
          $('#customerEdit').addClass("disabled");
      });
    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        result.parent().addClass('has-error');
    }

}

var appendCustomEditTime = function (field, element) {
    var date = field.Value == null ? null : field.Value;

    var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);

    var result = $('<p>')
      .text((date === null ? 'Unassigned' : date.localeFormat(_mainFrame.Ts.Utils.getTimePattern())))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
          e.preventDefault();
          if (!$(this).hasClass('editable'))
              return false;
          var parent = $(this).hide();
          _mainFrame.Ts.System.logAction('Customer Detail - Edit Custom Time');
          var container = $('<div>')
            .insertAfter(parent);

          var container1 = $('<div>')
          .addClass('col-xs-9')
          .appendTo(container);

          var fieldValue = parent.closest('.form-group').data('field').Value;
          var input = $('<input type="text">')
            .addClass('col-xs-10 form-control')
            .val(fieldValue === null ? '' : fieldValue.localeFormat(_mainFrame.Ts.Utils.getTimePattern()))
            .datetimepicker({ pickDate: false })

            .appendTo(container1)
            .focus();

          $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
                $(this).closest('div').remove();
                parent.show();
                $('#customerEdit').removeClass("disabled");
            })
            .insertAfter(container1);
          $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
                var value = _mainFrame.Ts.Utils.getMsDate("1/1/1900 " + input.val());
                container.remove();
                if (field.IsRequired && (value === null || $.trim(value) === '')) {
                    result.parent().addClass('has-error');
                }
                else {
                    result.parent().removeClass('has-error');
                }
                _mainFrame.Ts.System.logAction('Customer Detail - Save Custom DateTime Edit');
                _mainFrame.Ts.Services.System.SaveCustomValue(field.CustomFieldID, organizationID, value, function (result) {
                    parent.closest('.form-group').data('field', result);
                    var date = result.Value === null ? null : _mainFrame.Ts.Utils.getMsDate(result.Value);
                    parent.text((date === null ? 'Unassigned' : date.localeFormat(_mainFrame.Ts.Utils.getTimePattern())))
                    $('#customerEdit').removeClass("disabled");
                }, function () {
                    alert("There was a problem saving your contact property.");
                    $('#customerEdit').removeClass("disabled");
                });
                parent.show();
            })
            .insertAfter(container1);
          $('#customerEdit').addClass("disabled");
      });
    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        result.parent().addClass('has-error');
    }

}

$.fn.autoGrow = function () {
    return this.each(function () {
        // Variables
        var colsDefault = 130; //this.cols;
        var rowsDefault = this.rows;

        //Functions
        var grow = function () {
            growByRef(this);
        }

        var growByRef = function (obj) {
            var linesCount = 0;
            var lines = obj.value.split('\n');

            for (var i = lines.length - 1; i >= 0; --i) {
                linesCount += Math.floor((lines[i].length / colsDefault) + 1);
            }

            if (linesCount > rowsDefault)
                obj.rows = linesCount + 1;
            else
                obj.rows = rowsDefault;
        }

        var characterWidth = function (obj) {
            var characterWidth = 0;
            var temp1 = 0;
            var temp2 = 0;
            var tempCols = obj.cols;

            obj.cols = 1;
            temp1 = obj.offsetWidth;
            obj.cols = 2;
            temp2 = obj.offsetWidth;
            characterWidth = temp2 - temp1;
            obj.cols = tempCols;

            return characterWidth;
        }

        // Manipulations
        //this.style.width = "auto";
        this.style.height = "auto";
        this.style.overflow = "hidden";
        //this.style.width = ((characterWidth(this) * this.cols) + 6) + "px";
        this.onkeyup = grow;
        this.onfocus = grow;
        this.onblur = grow;
        growByRef(this);
    });
};

function onShow() {
    customerDetailPage.refresh();
};

CustomerDetailPage = function () {

};

CustomerDetailPage.prototype = {
    constructor: CustomerDetailPage,
    refresh: function () {

    }
};

function convertToValidDate(val) {
    var value = '';
    if (val == "")
        return value;

    var date = moment(val, _dateFormat);

    if (date.year() > 2000)
        return date.format('MM/DD/YYYY');
    else
        return null;
}

function convertToValidDateTime(val) {
    var value = '';
    if (val == "")
        return value;

    if (_dateFormat.indexOf("M") != 0) {
        var dateArr = val.replace(/\./g, '/').replace(/-/g, '/').split('/');
        if (_dateFormat.indexOf("D") == 0)
            var day = dateArr[0];
        if (_dateFormat.indexOf("Y") == 0)
            var year = dateArr[0];
        if (_dateFormat.indexOf("M") == 3 || _dateFormat.indexOf("M") == 5)
            var month = dateArr[1];

        var timeSplit = dateArr[2].split(' ');
        if (_dateFormat.indexOf("Y") == 6)
            var year = timeSplit[0];
        else
            var day = timeSplit[0];

        var theTime = timeSplit[1];

        var formattedDate = month + "/" + day + "/" + year + " " + theTime;
        //value = parent.Ts.Utils.getMsDate(formattedDate) + " " + theTime;
        return formattedDate;
    }
    else
        return val;
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
        result = result + '<a target="_blank" class="valueLink" href="' + url + '" title="' + matchArray[0] + '">' + matchArray[0] + '</a>'
    }

    return result == '' ? input : result;
}

function openTicketWindow(ticketID) {
    _mainFrame.Ts.MainPage.openTicketByID(ticketID, true);
}

function openNote(noteID) {
    _mainFrame.Ts.Services.Customers.LoadNote(noteID, function (note) {
        var desc = note.Description;
        desc = desc.replace(/<br\s?\/?>/g, "\n");
        $('#noteDescBox').show();
        $('.noteDesc').html("<h4>Description</h4> <p>" + desc + "</p>");
        BuildFileDescription(note.Attachments);
    });
}

function GetTinyMCEFontSize(fontSize) {
    var result = '';
    switch (fontSize) {
        case 1:
            result = "8pt";
            break;
        case 2:
            result = "10pt";
            break;
        case 3:
            result = "12pt";
            break;
        case 4:
            result = "14pt";
            break;
        case 5:
            result = "18pt";
            break;
        case 6:
            result = "24pt";
            break;
        case 7:
            result = "36pt";
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

function SetupParentSection(parents) {
    AddParents(parents);
    if ($('#company-Parents-Input').length) {
        $('#company-Parents-Input').selectize({
            valueField: 'id',
            labelField: 'label',
            searchField: 'label',
            load: function (query, callback) {
                this.clearOptions();        // clear the data
                this.renderCache = {};      // clear the html template cache
                getCompany(query, callback)
            },
            delimiter: null,
            initData: true,
            preload: false,
            onLoad: function () {
                if (this.settings.initData === true) {
                    this.settings.initData = false;
                }
            },
            //create: function (input, callback) {
            //    $('#NewCustomerModal').modal('show');
            //    callback(null);
            //    $('#company-Parents-Input').closest('.form-group').removeClass('hasError');
            //},
            onItemAdd: function (value, $item) {
                if (this.settings.initData === false) {
                    $('#company-Parents-Input').closest('.form-group').removeClass('hasError');
                    var parentData = $item.data();

                    _mainFrame.Ts.Services.Customers.AddParent(organizationID, value, function (parent) {
                        AddParent(parent, $("#company-Parent"));

                        //if (customerData.type == "u") {
                        //    _mainFrame.Ts.Services.Customers.LoadAlert(value, _mainFrame.Ts.ReferenceTypes.Users, function (note) {
                        //        LoadTicketNotes(note);
                        //    });
                        //}
                        //else {
                        //    _mainFrame.Ts.Services.Customers.LoadAlert(value, _mainFrame.Ts.ReferenceTypes.Organizations, function (note) {
                        //        LoadTicketNotes(note);
                        //    });
                        //}

                        //window.parent.ticketSocket.server.ticketUpdate(_ticketNumber, "addcustomer", userFullName);
                        _mainFrame.Ts.System.logAction('Customer Detail - Add Parent');
                    }, function () {
                        $(this).parent().remove();
                        alert('There was an error adding the parent.');
                    });
                    this.removeItem(value, true);
                }
            },
            plugins: {
                'sticky_placeholder': {},
                'no_results': {}
            },
            score: function (search) {
                return function (option) {
                    return 1;
                }
            },
            render: {
                item: function (item, escape) {
                    return '<div data-value="' + item.value + '" data-type="' + item.data + '" data-selectable="" class="option">' + item.label + '</div>';
                },
                option: function (item, escape) {
                    return '<div data-value="' + escape(item.value) + '" data-type="' + escape(item.data) + '" data-selectable="" class="option">' + item.label + '</div>';
                },
                option_create: function (data, escape) {
                    return '<div class="create">Create <strong>' + escape(data.input) + '</strong></div>';
                }
            },
            onDropdownClose: function ($dropdown) {
                $($dropdown).prev().find('input').blur();
            },
            closeAfterSelect: true
        });
    }

    //$('#customer-company-input').selectize({
    //    valueField: 'label',
    //    labelField: 'label',
    //    searchField: 'label',
    //    load: function (query, callback) {
    //        this.clearOptions();        // clear the data
    //        this.renderCache = {};      // clear the html template cache
    //        getCompany(query, callback)
    //    },
    //    score: function (search) {
    //        return function (option) {
    //            return 1;
    //        }
    //    },
    //    onDropdownClose: function ($dropdown) {
    //        $($dropdown).prev().find('input').blur();
    //    },
    //    create: true,
    //    closeAfterSelect: true,
    //    plugins: {
    //        'sticky_placeholder': {},
    //        'no_results': {}
    //    }
    //});

    //$('#Customer-Create').click(function (e) {
    //    e.preventDefault();
    //    e.stopPropagation();
    //    _mainFrame.Ts.System.logAction('Ticket - New Customer Added');
    //    var email = $('#customer-email-input').val();
    //    var firstName = $('#customer-fname-input').val();
    //    var lastName = $('#customer-lname-input').val();
    //    var phone = $('#customer-phone-input').val();;
    //    var companyName = $('#customer-company-input').val();
    //    _mainFrame.Ts.Services.Users.CreateNewContact(email, firstName, lastName, companyName, phone, false, function (result) {
    //        if (result.indexOf("u") == 0 || result.indexOf("o") == 0) {
    //            _mainFrame.Ts.Services.Tickets.AddTicketCustomer(_ticketID, result.charAt(0), result.substring(1), function (result) {
    //                AddCustomers(result);
    //                $('.ticket-new-customer-email').val('');
    //                $('.ticket-new-customer-first').val('');
    //                $('.ticket-new-customer-last').val('');
    //                $('.ticket-new-customer-company').val('');
    //                $('.ticket-new-customer-phone').val('');
    //                $('#NewCustomerModal').modal('hide');
    //            });
    //        }
    //        else if (result.indexOf("The company you have specified is invalid") !== -1) {
    //            if (_mainFrame.Ts.System.User.CanCreateCompany || _mainFrame.Ts.System.User.IsSystemAdmin) {
    //                if (confirm('Unknown company, would you like to create it?')) {
    //                    _mainFrame.Ts.Services.Users.CreateNewContact(email, firstName, lastName, companyName, phone, true, function (result) {
    //                        _mainFrame.Ts.Services.Tickets.AddTicketCustomer(_ticketID, result.charAt(0), result.substring(1), function (result) {
    //                            AddCustomers(result);
    //                            $('.ticket-new-customer-email').val('');
    //                            $('.ticket-new-customer-first').val('');
    //                            $('.ticket-new-customer-last').val('');
    //                            $('.ticket-new-customer-company').val('');
    //                            $('.ticket-new-customer-phone').val('');
    //                            $('#NewCustomerModal').modal('hide');
    //                        });
    //                    });
    //                }
    //            }
    //            else {
    //                alert("We're sorry, but you do not have the rights to create a new company.");
    //                $('.ticket-new-customer-email').val('');
    //                $('.ticket-new-customer-first').val('');
    //                $('.ticket-new-customer-last').val('');
    //                $('.ticket-new-customer-company').val('');
    //                $('.ticket-new-customer-phone').val('');
    //                $('#NewCustomerModal').modal('hide');
    //            }
    //        }
    //        else {
    //            alert(result);
    //        }
    //    });
    //});

    $('#company-Parent').on('click', 'span.tagRemove', function (e) {
        if (confirm('Are you sure you would like to remove this parent company?')) {
            var self = $(this);
            var data = self.parent().data().tag;

            _mainFrame.Ts.Services.Customers.RemoveParent(data.ChildID, data.ParentID, function () {
                self.parent().remove();
                _mainFrame.Ts.System.logAction('Customer Detail - Remove Parent');
            }, function () {
                alert('There was a problem removing the parent from the company.');
            });
        }
        else {
            _mainFrame.Ts.System.logAction('Customer Detail - Remove Parent - Cancelled.');
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
            default:
                functionName = null;
        }

        if (functionName != null) {
            result = '<span class="associations"><i target="_blank" class="ts-link fa ' + iconClass + '" href="#" onclick="' + functionName + '" title="' + associationName + '"></i></span>'
        }

        return new Handlebars.SafeString(result);
    });
};

function AddParent(parent, div) {
    $("#company-Parents-Input").val('');
    var cssClasses = "tag-item";

    //if (parents[i].Flag) {
    //    cssClasses = cssClasses + " tag-error"
    //}
    //if (parents[i].Contact !== null && parents[i].Company !== null) {
    //    label = '<span class="UserAnchor" data-userid="' + parents[i].UserID + '" data-placement="left" data-ticketid="' + _ticketID + '">' + parents[i].Contact + '</span><br/><span class="OrgAnchor" data-orgid="' + parents[i].OrganizationID + '" data-placement="left">' + parents[i].Company + '</span>';
    //    var newelement = PrependTag(parentsDiv, parents[i].UserID, label, parents[i], cssClasses);
    //}
    //else if (parents[i].Contact !== null) {
    //    label = '<span class="UserAnchor" data-userid="' + parents[i].UserID + '" data-placement="left">' + parents[i].Contact + '</span>';
    //    var newelement = PrependTag(parentsDiv, parents[i].UserID, label, parents[i], cssClasses);
    //    newelement.data('userid', parents[i].UserID).data('placement', 'left').data('ticketid', _ticketID);
    //}
    //else if (parents[i].Company !== null) {
    var label = '<span class="OrgAnchor" data-orgid="' + parent.ParentID + '" data-placement="left">' + parent.ParentName + '</span>';
    var newelement = PrependTag(div, parent.ParentID, label, parent, cssClasses);
    //newelement.data('orgid', parents[i].OrganizationID).data('placement', 'left').data('ticketid', _ticketID);
    newelement.data('orgid', parent.ParentID).data('placement', 'left');
    //}
}

function AddParents(parents) {
    var parentsDiv = $("#company-Parent");
    parentsDiv.empty();
    for (var i = 0; i < parents.length; i++) {
        AddParent(parents[i], parentsDiv);
    };
}

function PrependTag(parent, id, value, data, cssclass) {
    if (cssclass === undefined) cssclass = 'tag-item';
    var _compiledTagTemplate = Handlebars.compile($("#customer-tag").html());
    var tagHTML = _compiledTagTemplate({ id: id, value: value, data: data, css: cssclass });
    return $(tagHTML).prependTo(parent).data('tag', data);
}

function appendItem(container, item) {
    var hasCustomerInsights = _mainFrame.Ts.System.Organization.IsCustomerInsightsActive;
    var organizationId = _mainFrame.Ts.System.Organization.OrganizationID;
    var el = $('<tr>');

    if (!hasCustomerInsights) {
        var circle = $('<i>').addClass('fa fa-circle fa-stack-2x');
        var icon = $('<i>').addClass('fa fa-stack-1x fa-inverse');

        $('<td>').addClass('result-icon').append(
          $('<span>').addClass('fa-stack fa-2x').append(circle).append(icon)
        ).appendTo(el);

        var div = $('<div>')
          .addClass('peopleinfo')
          .append(
            $('<div>')
              .addClass('pull-right')
              .append($('<p>').text(item.openTicketCount + ' open tickets'))
          );

        $('<td>').append(div).appendTo(el);

        if (item.userID) {
            circle.addClass('color-orange');
            icon.addClass('fa-user');
            appendContact(div, item);
        }
        else {
            circle.addClass('color-green');
            icon.addClass('fa-building-o');
            appendCompany(div, item);
        }
    }
    else {
        var image = $('<img>');
        var imagePath;

        if (item.userID) {
            imagePath = "../../../dc/" + item.organizationID + "/contactavatar/" + item.userID + "/48/index";
        }
        else {
            imagePath = "../../../dc/" + organizationId + "/companylogo/" + item.organizationID + "/48/index";
        }

        var imageObject = new Image();
        imageObject.src = imagePath;
        imageObject.className = "user-avatar";

        $('<td>').addClass('result-icon').append(
          $('<span>').addClass('fa-stack fa-2x').append(imageObject)
        ).appendTo(el);

        var div = $('<div>')
          .addClass('peopleinfo')
          .append(
            $('<div>')
              .addClass('pull-right')
              .append($('<p>').text(item.openTicketCount + ' open tickets'))
          );

        $('<td>').append(div).appendTo(el);

        if (item.userID) {
            appendContact(div, item);
        }
        else {
            appendCompany(div, item);
        }
    }


    el.appendTo(container);
}

function appendCompany(el, item) {


    $('<a>')
      .attr('href', '#')
      .addClass('companylink')
      .data('organizationid', item.organizationID)
      .text(!isNullOrWhiteSpace(item.name) ? item.name : 'Unnamed Company')
      .appendTo($('<h4>').appendTo(el));

    var list = $('<ul>').appendTo(el);

    if (!isNullOrWhiteSpace(item.website)) {
        var site = item.website.indexOf('http') != 0 ? 'http://' + item.website : item.website;
        $('<a>')
            .attr('target', '_blank')
            .attr('href', site)
            .text(item.website)
            .appendTo($('<li>').appendTo(list));
    }

    var phones = $('<li>');
    appendPhones(phones, item);
    phones.appendTo(list);

    $('<li>').text(item.isPortal ? 'Has portal access' : '').appendTo(list);
}

function isNullOrWhiteSpace(str) {
    return str === null || str.match(/^ *$/) !== null;
}

function appendPhones(el, item) {
    for (var i = 0; i < item.phones.length; i++) {
        var phone = item.phones[i];
        if (!isNullOrWhiteSpace(phone.number)) {
            $('<span>').text(" " + phone.type + " ").appendTo(el);
            $('<a>')
                .attr('href', 'tel:' + phone.number)
                .attr('target', '_blank')
                .text(phone.number)
                .appendTo(el);
            if (!isNullOrWhiteSpace(phone.ext)) {
                $('<span>').text(" Ext:" + phone.ext).appendTo(el);
            }
        }
    }
}

var execGetCompany = null;
function getCompany(request, response) {
    if (execGetCompany) { execGetCompany._executor.abort(); }
    execGetCompany = _mainFrame.Ts.Services.Organizations.WCSearchOrganization(request, function (result) { response(result); });
}

function LoadProductFamilies() {
    _mainFrame.Ts.Services.Organizations.LoadOrgProductFamilies(_mainFrame.Ts.System.Organization.OrganizationID, function (productFamilies) {
        for (var i = 0; i < productFamilies.length; i++) {
            $('<option>').attr('value', productFamilies[i].ProductFamilyID).text(productFamilies[i].Name).data('o', productFamilies[i]).appendTo('#ddlNoteProductFamily');
            $('<option>').attr('value', productFamilies[i].ProductFamilyID).text(productFamilies[i].Name).data('o', productFamilies[i]).appendTo('#ddlFileProductFamily');
            $('<option>').attr('value', productFamilies[i].ProductFamilyID).text(productFamilies[i].Name).data('o', productFamilies[i]).appendTo('#ddlRatingProductFamily');
        }
    });
}

function LoadNoteActivities(){
    _mainFrame.Ts.Services.Organizations.LoadNoteActivities(function (activities) {
        for (var i = 0; i < activities.length; i++) {
            if (activities[i].Value == 4)
                $('<option>').attr('value', activities[i].Value).attr('selected', true).text(activities[i].Name).appendTo('#ddlNoteActivityType');
            else
                $('<option>').attr('value', activities[i].Value).text(activities[i].Name).appendTo('#ddlNoteActivityType');
        }
    });
}

function BuildFileDescription(attachments) {
    for (var i = 0; i < attachments.length; i++) {
        var files = $('<div>')
        .addClass('activity-file')
        .attr('id', attachments[i].AttachmentID)
        .html(attachments[i].FileName)
        .appendTo('.noteDesc');
    }
}
