var _productVersionID = null;
var _productID = null;
var _productName = null;
var _versionNumber = null;
var _execGetCustomer = null;
var _headersLoaded = false;
var _isLoadingCustomers = false;
var _isLoadingInventory = false;
var _viewingCustomers = false;
var _viewingInventory = false;
var _customersSortColumn = 'Date Created';
var _customersSortDirection = 'DESC';

$(document).ready(function () {
  _productVersionID = top.Ts.Utils.getQueryValue("productversionid", window);

  $('body').layout({
      defaults: {
          spacing_open: 0,
          resizable: false,
          closable: false
      },
      north: {
          size: 90,
          spacing_open: 1
      },
      center: {
          maskContents: true,
          size: 'auto'
      }
  });

  top.Ts.Services.Products.GetVersion(_productVersionID, function (productVersion) {
    if (productVersion == null)
    {
      //alert('This product version has either been deleted or you do not have permission to view it.');
      //top.Ts.MainPage.closeNewProductVersionTab(_productVersionID);
      var url = window.location.href;
      if (url.indexOf('.') > -1) {
        url = url.substring(0, url.lastIndexOf('/') + 1);
      }
      window.location = url + 'NoTicketAccess.html?type=productversion';
      return;
    }
    else
    {
      _productID = productVersion.ProductID;
      _productName = productVersion.ProductName;
      _versionNumber = productVersion.VersionNumber;
      $('#productVersionNumber').text(productVersion.ProductName + " - " + productVersion.VersionNumber);
      $('#fieldDescription').html(productVersion.Description != null && productVersion.Description != "" ? productVersion.Description : "Empty");
      $('#fieldProduct').html(productVersion.ProductName);
      $('#fieldStatus').html(productVersion.VersionStatus);
      $('#fieldReleased').text((productVersion.IsReleased === true ? 'True' : 'False'));
      $('#fieldReleaseDate').text(top.Ts.Utils.getMsDate(productVersion.ReleaseDateUTC).localeFormat(top.Ts.Utils.getDatePattern()));

      top.Ts.Services.Admin.GetIsJiraLinkActiveForOrganization(function (result) {
        if (result)
        {
          $('#jiraIntegrationBox').show();
          $('#fieldJiraProjectKey').text(productVersion.JiraProjectKey != null && productVersion.JiraProjectKey != "" ? productVersion.JiraProjectKey : "Not Set");
        }
      });

      top.Ts.Services.Admin.GetIsTFSLinkActiveForOrganization(function (result) {
          if (result) {
              $('#TFSIntegrationBox').show();
              $('#fieldTFSProjectName').text(productVersion.TFSProjectName != null && productVersion.TFSProjectName != "" ? productVersion.TFSProjectName : "Not Set");
          }
      });

      top.privateServices.SetUserSetting('SelectedProductID', _productID);
      top.privateServices.SetUserSetting('SelectedProductVersionID', _productVersionID);
    }
  });

  $('.product-version-tooltip').tooltip({ placement: 'bottom', container: 'body' });

  $('.productVersionProperties p').toggleClass("editable");
  $('.productVersionProperties span').toggleClass("editable");

  $('#productVersionEdit').click(function (e) {
    top.Ts.System.logAction('Product Version Detail - Product Version Edit');
    $('.productVersionProperties p').toggleClass("editable");
    $('.productVersionProperties span').toggleClass("editable");
    $('.customProperties p').toggleClass("editable");

    $(this).toggleClass("btn-primary");
    $(this).toggleClass("btn-success");
    if ($(this).hasClass("btn-primary"))
    	$(this).html('<i class="fa fa-pencil"></i> Edit');
    else
    	$(this).html('<i class="fa fa-pencil"></i> Save');
    $('#productVersionTabs a:first').tab('show');
  });

  $('#productVersionRefresh').click(function (e) {
    top.Ts.System.logAction('Product Version Detail - Refresh Page');
    window.location = window.location;
  });


  $('#productVersionDelete').click(function (e) {
    if (confirm('Are you sure you would like to remove this product version?')) {
      top.Ts.System.logAction('Product Version Detail - Delete Product Version');
      top.privateServices.DeleteVersion(_productVersionID, function (e) {
        top.Ts.MainPage.closeNewProductVersionTab(_productVersionID);
      });
    }
  });

  $('#productVersionTabs a:first').tab('show');

  $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    $('.scrollup').fadeOut();
    if (e.target.innerHTML == "Details") {
      createTestChart();
      LoadCustomProperties();
      _viewingCustomers = false;
      _viewingInventory = false;
    }
    else if (e.target.innerHTML == "Customers") {
      LoadCustomers();
      _viewingCustomers = true;
      _viewingInventory = false;
    }
    else if (e.target.innerHTML == "Tickets") {
      LoadTickets();
      _viewingCustomers = false;
      _viewingInventory = false;
    }
    else if (e.target.innerHTML == "Knowledge Base") {
      LoadKbs();
      _viewingCustomers = false;
      _viewingInventory = false;
    }
    else if (e.target.innerHTML == "Files") {
      LoadFiles();
      _viewingCustomers = false;
      _viewingInventory = false;
    }
    else if (e.target.innerHTML == "Watercooler") {
      $('#watercoolerIframe').attr("src", "WaterCooler.html?pagetype=5&pageid=" + _productVersionID);
      _viewingCustomers = false;
      _viewingInventory = false;
    }
    else if (e.target.innerHTML == "Inventory") {
      LoadInventory();
      _viewingCustomers = false;
      _viewingInventory = true;
    }
  })

  function createTestChart() {
    var greenLimit, yellowLimit;

    top.Ts.Services.Products.LoadVersionChartData(_productVersionID, true, function (chartString) {

        var chartData = [];
        var dummy = chartString.split(",");
        var openCount=0;

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

    top.Ts.Services.Products.LoadVersionChartData(_productVersionID, false, function (chartString) {

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
  }

  top.Ts.Services.Products.GetProductVersionTickets(_productVersionID, 0, function (e) {
      $('#openTicketCount').text("Open Tickets: " + e);
  });

  top.Ts.Services.Tickets.Load5MostRecentByProductVersionID(_productVersionID, function (tickets) {
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

            top.Ts.MainPage.openTicket($(this).closest('.ticket').data('o').TicketNumber, true);
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

  var ellipseString = function (text, max) { return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text; };

  function LoadCustomProperties() {
    top.Ts.Services.Assets.GetCustomValues(_productVersionID, top.Ts.ReferenceTypes.ProductVersions, function (html) {
      appendCustomValues(html);
    });
  }

  var historyLoaded = 0;

  $('#historyToggle').on('click', function () {
      top.Ts.System.logAction('Product Version Detail - History Toggle');
      if (historyLoaded == 0) {
          historyLoaded = 1;
          LoadHistory(1);
      }
  });

  $('#historyRefresh').on('click', function () {
      top.Ts.System.logAction('Product Version Detail - History Refresh');
          LoadHistory(1);
  });

  function LoadHistory(start) {

      if(start == 1)
          $('#tblHistory tbody').empty();

          top.Ts.Services.Products.LoadVersionHistory(_productVersionID, start, function (history) {
              for (var i = 0; i < history.length; i++) {
                  $('<tr>').html('<td>' + history[i].DateCreated.localeFormat(top.Ts.Utils.getDateTimePattern()) + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td>')
                  .appendTo('#tblHistory > tbody:last');
                  //$('#tblHistory tr:last').after('<tr><td>' + history[i].DateCreated.toDateString() + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td></tr>');
              }
              if(history.length == 50)
                  $('<button>').text("Load More").addClass('btn-link')
                  .click(function (e){
                      LoadHistory($('#tblHistory tbody > tr').length+1);
                      $(this).remove();
                  })
                  .appendTo('#tblHistory > tbody:last');
          });
  }

  $('#productVersionNumber').click(function (e) {
      e.preventDefault();
      if (!$(this).hasClass('editable'))
          return false;

      top.Ts.System.logAction('Product Version Detail - Edit Version Number');
      var header = $(this).hide();
      var container = $('<div>')
        .insertAfter(header);

      var container1 = $('<div>')
          .addClass('col-xs-9')
        .appendTo(container);

      $('<input type="text">')
        .addClass('col-xs-10 form-control')
        .val(_versionNumber)
        .appendTo(container1)
        .focus();

      $('<i>')
        .addClass('col-xs-1 fa fa-times')
        .click(function (e) {
            $(this).closest('div').remove();
            header.show();
            $('#productVersionEdit').removeClass("disabled");
        })
        .insertAfter(container1);
      $('<i>')
        .addClass('col-xs-1 fa fa-check')
        .click(function (e) {
            top.Ts.System.logAction('Product Version Detail - Save Version Number Edit');
            top.Ts.Services.Products.SetVersionNumber(_productVersionID, $(this).prev().find('input').val(), function (result) {
                header.text(result);
                _versionNumber = result;
                $('#productVersionNumber').text(_productName + " - " + result);
                $('#productVersionEdit').removeClass("disabled");
            },
            function (error) {
                header.show();
                alert('There was an error saving the product version number.');
                $('#productVersionEdit').removeClass("disabled");
            });
            $('#productVersionEdit').removeClass("disabled");
            $(this).closest('div').remove();
            header.show();
        })
        .insertAfter(container1);
      $('#productVersionEdit').addClass("disabled");
  });

  //initEditor($('#fieldDesc'), function (ed) {
  //    $('#fieldDesc').tinymce().focus();
  //});

  $('#fieldDescription').click(function (e) {
      e.preventDefault();
      if (!$(this).hasClass('editable'))
          return false;
      var header = $(this).hide();


      top.Ts.System.logAction('Product Version Detail - Edit Description');
      top.Ts.Services.Products.GetVersion(_productVersionID, function (productVersion) {
          var desc = productVersion.Description;
          desc = desc.replace(/<br\s?\/?>/g, "\n");
          initEditor($('#fieldDesc'), function (ed) {
          	$('#fieldDesc').tinymce().setContent(desc);
          	$('#fieldDesc').tinymce().focus();

          });

        //$('#fieldDesc').html(desc);
        $('#descriptionContent').hide();
        $('#descriptionForm').show();
      });

      $('#btnDescriptionCancel').click(function (e) {
        e.preventDefault();
        $('#descriptionForm').hide();
        $('#descriptionContent').show();
        header.show();
        $('#productVersionEdit').removeClass("disabled");
      });

      $('#btnDescriptionSave').click(function (e) {
        e.preventDefault();
        top.Ts.System.logAction('Product Version Detail - Save Description Edit');
        top.Ts.Services.Products.SetVersionDescription(_productVersionID, $(this).prev().find('textarea').val(), function (result) {
            header.html(result);
            $('#fieldDescription').find('p').addClass('editable');
            $('#productVersionEdit').removeClass("disabled");
        },
        function (error) {
            header.show();
            alert('There was an error saving the product version description.');
            $('#productVersionEdit').removeClass("disabled");
        });
        $('#descriptionForm').hide();
        $('#descriptionContent').show();
        header.show();
      })
      $('#productVersionEdit').addClass("disabled");
  });

  $('#fieldProduct').click(function (e) {
    e.preventDefault();
    top.Ts.System.logAction('Product Version Detail - View Product');
    top.Ts.MainPage.openNewProduct(_productID);

    top.Ts.Services.Products.UpdateRecentlyViewed('p' + _productID, function (resultHtml) {
    });
  });

  $('#fieldStatus').click(function (e) {
      e.preventDefault();
      if (!$(this).hasClass('editable'))
          return false;
      var header = $(this).hide();
      top.Ts.System.logAction('Product Version Detail - Edit Status');
      var container = $('<div>')
        .insertAfter(header);

      var container1 = $('<div>')
          .addClass('col-xs-9')
        .appendTo(container);

      var select = $('<select>').addClass('form-control').attr('id', 'ddlStatus').appendTo(container1);
      top.Ts.Services.Products.LoadVersionStatuses(function (statuses) {
          for (var i = 0; i < statuses.length; i++) {
              var opt = $('<option>').attr('value', statuses[i].ProductVersionStatusID).text(statuses[i].Name).data('o', statuses[i]);
              if (header.data('field') == statuses[i].ProductVersionStatusID)
                  opt.attr('selected', 'selected');
              opt.appendTo(select);
          }
      });


      $('<i>')
        .addClass('col-xs-1 fa fa-times')
        .click(function (e) {
            $(this).closest('div').remove();
            header.show();
            $('#productVersionEdit').removeClass("disabled");
        })
        .insertAfter(container1);
      $('#ddlStatus').on('change', function () {
          var value = $(this).val();
          var name = this.options[this.selectedIndex].innerHTML;
          container.remove();
          top.Ts.System.logAction('Product Version Detail - Save Status');
          top.Ts.Services.Products.SetProductVersionStatus(_productVersionID, value, function (result) {
              header.data('field', result);
              header.text(name);
              header.show();
              $('#productVersionEdit').removeClass("disabled");
          }, function () {
              alert("There was a problem saving your status property.");
              $('#productVersionEdit').removeClass("disabled");
          });
      });
      $('#productVersionEdit').addClass("disabled");
  });

  $('#fieldJiraProjectKey').click(function (e) {
    e.preventDefault();
    if (!$(this).hasClass('editable'))
      return false;

    top.Ts.System.logAction('Product Version Detail - Edit Jira Project Key');
    var header = $(this).hide();
    var container = $('<div>')
      .insertAfter(header);

    var container1 = $('<div>')
        .addClass('col-xs-8')
      .appendTo(container);

    $('<input type="text">')
      .addClass('col-xs-8 form-control')
      .val($(this).text())
      .appendTo(container1)
      .focus();

    $('<i>')
      .addClass('col-xs-1 fa fa-times')
      .click(function (e) {
        $(this).closest('div').remove();
        header.show();
        $('#productVersionEdit').removeClass("disabled");
      })
      .insertAfter(container1);
    $('<i>')
      .addClass('col-xs-1 fa fa-check')
      .click(function (e) {
        top.Ts.System.logAction('Product Version Detail - Save Jira Project Key Edit');
        var isForProductVersion = true;
        top.Ts.Services.Products.SetProductJiraProjectKey(_productVersionID, $(this).prev().find('input').val(), isForProductVersion, function (result) {
          header.text(result);
          $('#fieldJiraProjectKey').text(result);
        },
        function (error) {
          header.show();
          alert('There was an error saving the product version jira project key.');
        });

        $('#productVersionEdit').removeClass("disabled");
        $(this).closest('div').remove();
        header.show();
      })
      .insertAfter(container1);
    $('#productVersionEdit').addClass("disabled");
  });

  $('#fieldTFSProjectName').click(function (e) {
      e.preventDefault();
      if (!$(this).hasClass('editable'))
          return false;

      top.Ts.System.logAction('Product Version Detail - Edit TFS Project Name');
      var header = $(this).hide();
      var container = $('<div>')
        .insertAfter(header);

      var container1 = $('<div>')
          .addClass('col-xs-8')
        .appendTo(container);

      $('<input type="text">')
        .addClass('col-xs-8 form-control')
        .val($(this).text())
        .appendTo(container1)
        .focus();

      $('<i>')
        .addClass('col-xs-1 fa fa-times')
        .click(function (e) {
            $(this).closest('div').remove();
            header.show();
            $('#productVersionEdit').removeClass("disabled");
        })
        .insertAfter(container1);
      $('<i>')
        .addClass('col-xs-1 fa fa-check')
        .click(function (e) {
            top.Ts.System.logAction('Product Version Detail - Save TFS Project Name Edit');
            var isForProductVersion = true;
            top.Ts.Services.Products.SetProductTFSProjectName(_productVersionID, $(this).prev().find('input').val(), isForProductVersion, function (result) {
                header.text(result);
                $('#fieldTFSProjectName').text(result);
            },
            function (error) {
                header.show();
                alert('There was an error saving the product version tfs project name.');
            });

            $('#productVersionEdit').removeClass("disabled");
            $(this).closest('div').remove();
            header.show();
        })
        .insertAfter(container1);
      $('#productVersionEdit').addClass("disabled");
  });

  $('#fieldReleased').click(function (e) {
      if (!$(this).hasClass('editable'))
          return false;
      top.Ts.Services.Products.SetProductVersionReleased(_productVersionID, ($(this).text() !== 'True'), function (result) {
          top.Ts.System.logAction('Product Version Detail - Toggle Released State');
          $('#fieldReleased').text((result === true ? 'True' : 'False'));
      },
      function (error) {
          header.show();
          alert('There was an error saving the product version released.');
      });
  });

  $('#fieldReleaseDate').click(function (e) {
    e.preventDefault();
    if (!$(this).hasClass('editable'))
      return false;
    top.Ts.System.logAction('Product Version Detail - Date Released clicked');
    var parent = $(this).hide();
    var container = $('<div>')
          .insertAfter(parent);

    var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

    var input = $('<input type="text">')
            .addClass('col-xs-10 form-control')
            .val($(this).val())
            .datetimepicker({ pickTime: false })
            .appendTo(container1)
            .focus();

    $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
            $(this).closest('div').remove();
            parent.show();
            $('#productVersionEdit').removeClass("disabled");
            top.Ts.System.logAction('Product Version Detail - Date Released change cancelled');
          })
          .insertAfter(container1);
    $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
            var value = top.Ts.Utils.getMsDate(input.val());
            container.remove();
            top.Ts.Services.Products.SetReleaseDate(_productVersionID, value, function (result) {
              var date = result === null ? null : top.Ts.Utils.getMsDate(result);
              parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDatePattern())))
              $('#productVersionEdit').removeClass("disabled");
              top.Ts.System.logAction('Product Version Detail - Date Released Change');
            },
            function (error) {
              parent.show();
              alert('There was an error saving the Product Version Date Released.');
              $('#productVersionEdit').removeClass("disabled");
            });
            $('#productVersionEdit').removeClass("disabled");
            $(this).closest('div').remove();
            parent.show();
          })
          .insertAfter(container1);
    $('#productVersionEdit').addClass("disabled");
  });

  var _isAdmin = top.Ts.System.User.IsSystemAdmin;
  if (!top.Ts.System.User.CanEditCompany && !_isAdmin) 
  {
      $('#customerToggle').hide();
  }

  if (!top.Ts.System.User.CanEditVersions && !_isAdmin) {
      $('#productVersionEdit').remove();
  }

  if (!_isAdmin) {
      $('#productVersionDelete').hide();
      $('#associateAllToggle').hide();
      $('#unAssociateAllToggle').hide();
  }

  $('#customerToggle').click(function (e) {
      top.Ts.System.logAction('Product Version Detail - Toggle Customer Form');
      $('#customerForm').toggle();
  });

  $('#associateAllToggle').click(function (e) {
      if (confirm('Are you sure you would like to associate All customers to this version?')) {
        top.Ts.System.logAction('Product Version Detail - Toggle Associate All Customer Form');
        top.Ts.Services.Customers.AssignAllCustomersToVersion(_productVersionID, function () {
            LoadCustomers();
        }, function () {
            alert('There was an error associating all customers to this version. Please try again.');
        });
      }
  });

  $('#unAssociateAllToggle').click(function (e) {
      if (confirm('Are you sure you would like to unassociate All customers from this version?')) {
        top.Ts.System.logAction('Product Version Detail - Toggle Unassociate All Customer Form');
        top.Ts.Services.Customers.UnassignAllCustomersFromVersion(_productVersionID, function () {
            LoadCustomers();
        }, function () {
            alert('There was an error unassociating all customers from this version. Please try again.');
        });
      }
  });

  function LoadCustomers(start) {

      if(!_headersLoaded){
        
          top.Ts.Services.Customers.LoadcustomProductHeaders(function (headers) {
              for (var i = 0; i < headers.length; i++) {
                  var header = headers[i];
                  if(header == 'Product Name') {
                    header = 'Customer';
                  }
                  $('#tblCustomers th:last').after('<th>' + header + '</th>');
              }
              _headersLoaded = true;
          });
          }

      start = start || 0;
      showCustomersLoadingIndicator();
      $('#tblCustomers').fadeTo(200, 0.5);

      top.Ts.Services.Products.LoadVersionCustomers(_productVersionID, start, _customersSortColumn, _customersSortDirection, function (customers) {
          $('#tblCustomers').fadeTo(0, 1);
          if (start == 0) {
              insertCustomers(customers);
          } else {
              appendCustomers(customers);
          }
      });

  }

  function showCustomersLoadingIndicator() {
      _isLoadingCustomers = true;
      $('.customers-loading').show();
  }

  function insertCustomers(customers) {
      $('#tblCustomers tbody').empty();

      if (customers.length < 1) {
          $('.customers-loading').hide();
          $('.customers-done').hide();
          $('.customers-empty').show();
      } else {
          appendCustomers(customers);
      }
      _isLoadingCustomers = false;
  }

  function appendCustomers(customers) {
      $('.customers-loading').hide();
      $('.customers-empty').hide();
      $('.customers-done').hide();

      if (customers.length < 1) {
          $('.customers-done').show();
      } else {
          for (var i = 0; i < customers.length; i++) {
              var customfields = "";
              for (var p = 0; p < customers[i].CustomFields.length; p++) {
                  customfields = customfields + "<td>" + customers[i].CustomFields[p] + "</td>";
              }

              var html;

              if (top.Ts.System.User.CanEditCompany || _isAdmin) {
                  html = '<td><i class="fa fa-edit customerEdit"></i></td><td><i class="fa fa-trash-o customerDelete"></i></td><td><a href="#" class="customerView">' + customers[i].Customer + '</a></td><td>' + customers[i].SupportExpiration + '</td><td>' + customers[i].VersionStatus + '</td><td>' + customers[i].IsReleased + '</td><td>' + customers[i].ReleaseDate + '</td><td>' + customers[i].DateCreated + '</td>' + customfields;
              }
              else {
                  html = '<td></td><td></td><td><a href="#" class="customerView">' + customers[i].Customer + '</a></td><td>' + customers[i].SupportExpiration + '</td><td>' + customers[i].VersionStatus + '</td><td>' + customers[i].IsReleased + '</td><td>' + customers[i].ReleaseDate + '</td><td>' + customers[i].DateCreated + '</td>' + customfields
              }
              var tr = $('<tr>')
              .attr('id', customers[i].OrganizationProductID)
              .html(html)
              .appendTo('#tblCustomers > tbody:last');


          }
      }
      _isLoadingCustomers = false;
  }

  var getCustomers = function (request, response) {
    if (_execGetCustomer) { _execGetCustomer._executor.abort(); }
    _execGetCustomer = top.Ts.Services.Organizations.GetOrganizationForTicket(request.term, function (result) { response(result); });
  }

  $('#inputCustomer').autocomplete({
    open: function () {
      $('.ui-menu').width($('#inputCustomer').width());
    },
    minLength: 2,
    source: getCustomers,
    defaultDate: new Date(),
    select: function (event, ui) {
      $(this).data('item', ui.item);
    }
  });

  $('.product-version-customers-export-excel').on('click', function (e) {
      e.preventDefault();

      top.Ts.System.logAction('Product Version Detail Page - Export Customers Excel');

      window.open('../../../dc/' + top.Ts.System.Organization.OrganizationID + '/productversioncustomers/' + _productVersionID + '?Type=EXCEL', 'ProductCustomersDownload');

  });

  $('.product-version-customers-export-csv').on('click', function (e) {
      e.preventDefault();

      top.Ts.System.logAction('Product Version Detail Page - Export Customers CSV');

      window.open('../../../dc/' + top.Ts.System.Organization.OrganizationID + '/productversioncustomers/' + _productVersionID + '?Type=CSV', 'ProductCustomersDownload');

  });

  top.Ts.Services.Customers.GetDateFormat(false, function (dateformat) {
      $('.datepicker').attr("data-format", dateformat);
      $('.datepicker').datetimepicker({ pickTime: false });

      $('#productExpiration').attr("data-format", dateformat);
      $('.datetimepicker').datetimepicker({ });
  });

  LoadCustomControls(top.Ts.ReferenceTypes.OrganizationProducts);
  function LoadCustomControls(refType) {
      top.Ts.Services.Customers.LoadCustomControls(refType, function (html) {
          $('#customProductsControls').append(html);

          $('#customProductsControls .datepicker').datetimepicker({ pickTime: false });
          $('#customProductsControls .datetimepicker').datetimepicker({});
          $('#customProductsControls .timepicker ').datetimepicker({ pickDate: false });
      });
  }

  $('#btnCustomerSave').click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      top.Ts.System.logAction('Product Version Detail - Save Customer');
      var productInfo = new Object();
      var hasError = 0;
      productInfo.OrganizationID = $('#inputCustomer').data('item').id;
      productInfo.ProductID = _productID;
      productInfo.Version = _productVersionID;
      productInfo.SupportExpiration = $("#supportExpiration").val();
      productInfo.OrganizationProductID = $('#fieldOrganizationProductID').val();

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
              //    field.Value = $(this).val() == "" ? null : top.Ts.Utils.getMsDate($(this).val());
              //    break;
              //case "time":
              //    field.Value = $(this).val() == "" ? null : top.Ts.Utils.getMsDate("1/1/1900 " + $(this).val());
              //    break;
              //case "datetime":
              //    field.Value = $(this).val() == "" ? null : top.Ts.Utils.getMsDate($(this).val());
              //    break;
              default:
                  field.Value = $(this).val();
          }
          productInfo.Fields[productInfo.Fields.length] = field;
      });

      if (hasError == 0)
      {
          top.Ts.Services.Customers.SaveProduct(top.JSON.stringify(productInfo), function (prod) {
              LoadCustomers();
              $('#productExpiration').val('');
              $('#fieldOrganizationProductID').val('-1');
              $('.customField:visible').each(function () {
                  switch ($(this).attr("type")) {
                      case "checkbox":
                          $(this).prop('checked', false);
                          break;
                      default:
                          $(this).val('');
                  }
              });
              $('#customerForm').toggle();
          }, function () {
              alert('There was an error saving this product version association. Please try again.');
          });
      }

  });

  $("#btnCustomerCancel").click(function (e) {
      e.preventDefault();
      top.Ts.System.logAction('Product Version Detail - Cancel Customer Edit');
      $('#productExpiration').val('');
      $('#fieldOrganizationProductID').val('-1');
      $('.customField:visible').each(function () {
          switch ($(this).attr("type")) {
              case "checkbox":
                  $(this).prop('checked',false);
                  break;
              default:
                  $(this).val('');
          }
      });
      $('#customerForm').toggle();
  });

  $('#tblCustomers').on('click', '.customerHeader', function (e) {
      e.preventDefault();
      _customersSortColumn = $(this).text();
      var sortIcon = $(this).children(i);
      if (sortIcon.length > 0) {
          if (sortIcon.hasClass('fa-sort-asc')) {
              _customersSortDirection = 'DESC'
          }
          else {
              _customersSortDirection = 'ASC'
          }
          sortIcon.toggleClass('fa-sort-asc fa-sort-desc');
      }
      else {
          $('.customerHeader').children(i).remove();
          var newSortIcon = $('<i>')
              .addClass('fa fa-sort-asc')
              .appendTo($(this));
          _customersSortDirection = 'ASC';
          switch (_customersSortColumn.toLowerCase()) {
              case "version":
              case "support expiration":
              case "released date":
              case "date created":
                  newSortIcon.toggleClass('fa-sort-asc fa-sort-desc');
                  _customersSortDirection = 'DESC';

          }
      }
      LoadCustomers();
  });

  $('#tblCustomers').on('click', '.customerEdit', function (e) {
      e.preventDefault();
      var organizationProductID = $(this).parent().parent().attr('id');
      //var orgproductID;
      top.Ts.System.logAction('Product Version Detail - Edit Customer');
      top.Ts.Services.Products.LoadCustomer(organizationProductID, function (organizationProduct) {
          //orgproductID = prod.OrganizationProductID;
          var item = new Object();
          item.label = organizationProduct.Customer;
          item.value = organizationProduct.Customer;
          item.id = organizationProduct.OrganizationID;
          item.data = "o";
          $('#inputCustomer').data('item', item);
          $('#inputCustomer').val(organizationProduct.Customer);
          $('#supportExpiration').val(organizationProduct.SupportExpiration);
          $('#fieldOrganizationProductID').val(organizationProductID);
          top.Ts.Services.Customers.LoadCustomProductFields(organizationProductID, function (custField) {
              for (var i = 0; i < custField.length; i++) {
                  if (custField[i].FieldType == 2)
                      $('#' + custField[i].CustomFieldID).attr('checked', custField[i].Value);
                  //else if (custField[i].FieldType == 5)
                  //{
                  //    var date = field.value == null ? null : top.Ts.Utils.getMsDate(field.Value);
                  //    $('#' + custField[i].CustomFieldID).val(date.localeFormat(top.Ts.Utils.getDatePattern()));
                  //}
                        
                  else
                      $('#' + custField[i].CustomFieldID).val(custField[i].Value);
              }
          });
      });
      $('#customerForm').show();
  });

  $('#tblCustomers').on('click', '.customerDelete', function (e) {
      e.preventDefault();
      if (confirm('Are you sure you would like to remove this customer association?')) {
          top.Ts.System.logAction('Product Version Detail - Delete Customer');
          top.privateServices.DeleteOrganizationProduct($(this).parent().parent().attr('id'), false, function (e) {
              LoadCustomers();
          });
            
      }
  });

  $('#tblCustomers').on('click', '.customerView', function (e) {
      e.preventDefault();
      top.Ts.System.logAction('Product Version Detail - View Customer');
      top.Ts.MainPage.openProductOrganization($(this).parent().parent().attr('id'))
      //top.location = "../../../Default.aspx?OrganizationProductID=" + ;

  });

  function LoadTickets() {
      var url = "../../../Frames/TicketTabsAll.aspx?";
      if ($("#cbReported").is(":checked")) {
        if ($("#cbResolved").is(":checked")) {
          url += "tf_ProductVersionID=" + _productVersionID;
        }
        else {
          url += "tf_ReportedVersionID=" + _productVersionID;
        }
      }
      else {
        if ($("#cbResolved").is(":checked")) {
          url += "tf_SolvedVersionID=" + _productVersionID;
        }
        else {
          url = "";
        }
      }

      if (url != "") {
        $('#ticketIframe').attr("src", url);
      }
  }

  $('#cbReported, #cbResolved').on('click', function (e) {
    LoadTickets();
  });

  function LoadKbs() {
      var url = "../../../Frames/TicketTabsAll.aspx?tf_IsKnowledgeBase=true&";
      if ($("#cbReportedKB").is(":checked")) {
        if ($("#cbResolvedKB").is(":checked")) {
          url += "tf_ProductVersionID=" + _productVersionID;
        }
        else {
          url += "tf_ReportedVersionID=" + _productVersionID;
        }
      }
      else {
        if ($("#cbResolvedKB").is(":checked")) {
          url += "tf_SolvedVersionID=" + _productVersionID;
        }
        else {
          url = "";
        }
      }

      if (url != "") {
        $('#kbIframe').attr("src", url);
      }
  }

  $('#cbReportedKB, #cbResolvedKB').on('click', function (e) {
    LoadKbs();
  });

  function LoadFiles() {
      $('#tblFiles tbody').empty();
      top.Ts.Services.Customers.LoadFiles(_productVersionID, top.Ts.ReferenceTypes.ProductVersions, function (files) {
          for (var i = 0; i < files.length; i++) {
              var tr = $('<tr>')
              .attr('id', files[i].AttachmentID)
              .html('<td><i class="fa fa-trash-o delFile"></i></td><td class="viewFile">' + files[i].FileName + '</td><td>' + files[i].Description + '</td><td>' + files[i].CreatorName + '</td><td>' + files[i].DateCreated.toDateString() + '</td>')
              .appendTo('#tblFiles > tbody:last');


              //$('#tblFiles > tbody:last').appendTo('<tr id=' +  + '></tr>');
          }
      });
  }

  $('#tblFiles').on('click', '.viewFile', function (e) {
      e.preventDefault();
      top.Ts.MainPage.openNewAttachment($(this).parent().attr('id'));
  });

  $('#tblFiles').on('click', '.delFile', function (e) {
      e.preventDefault();
      e.stopPropagation();
      if (confirm('Are you sure you would like to remove this attachment?')) {
          top.Ts.System.logAction('Customer Detail - Delete File');
          top.privateServices.DeleteAttachment($(this).parent().parent().attr('id'), function (e) {
              LoadFiles();
          });
            
      }
  });

  $('#fileToggle').click(function (e) {
      top.Ts.System.logAction('Product Version Detail - Toggle File Form');
      $('#fileForm').toggle();
  });

  $("#btnFilesCancel").click(function (e) {
      top.Ts.System.logAction('Product Version Detail - Cancel File Form');
      $('.upload-queue').empty();
      $('#attachmentDescription').val('');
      $('#fileForm').toggle();
  });

  $('#btnFilesSave').click(function (e) {
      top.Ts.System.logAction('Product Version Detail - Save Files');
      if ($('.upload-queue li').length > 0) {
          $('.upload-queue li').each(function (i, o) {
              var data = $(o).data('data');
              data.formData = { description: $('#attachmentDescription').val().replace(/<br\s?\/?>/g, "\n") };
              data.url = '../../../Upload/Products/' + _productVersionID;
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
                .text(data.files[i].name + '  (' + top.Ts.Utils.getSizeString(data.files[i].size) + ')')
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
          data.context.find('.progress-bar').css('width', parseInt(data.loaded / data.total * 100, 10) +'%');
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
          $('#fileForm').toggle();
      }
  });

  function LoadInventory(start) {
      start = start || 0;
      showAssetsLoadingIndicator();
      $('.assetList').fadeTo(200, 0.5);

      top.Ts.Services.Products.LoadVersionAssets(_productVersionID, start, function (assets) {
          $('.assetList').fadeTo(0, 1);

          if (start == 0) {
              insertAssets(assets);
          } else {
              appendAssets(assets);
          }
      });
  }

  function showAssetsLoadingIndicator() {
      _isLoadingInventory = true;
      $('.inventory-loading').show();
  }

  function insertAssets(assets) {
      $('.assetList').empty();

      if (assets.length < 1) {
          $('.inventory-loading').hide();
          $('.inventory-done').hide();
          $('.inventory-empty').show();
      } else {
          appendAssets(assets);
      }
      _isLoadingInventory = false;
  }

  function appendAssets(assets) {
      $('.inventory-loading').hide();
      $('.inventory-empty').hide();
      $('.inventory-done').hide();

      if (assets.length < 1) {
          $('.inventory-done').show();
      } else {
          $('.assetList').append(assets)
      }
      _isLoadingInventory = false;
  }

  $('.assetList').on('click', '.assetLink', function (e) {
      e.preventDefault();
      top.Ts.System.logAction('Product Version Detail - Open Asset From List');
      top.Ts.MainPage.openNewAsset(this.id);
  });

  $('.maincontainer').bind('scroll', function () {
      if (_viewingCustomers) {
          if (_isLoadingCustomers == true) return;
          if ($('.customers-done').is(':visible')) return;

          if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
              LoadCustomers($('#tblCustomers > tbody > tr').length + 1);
          }
      }
      else if (_viewingInventory) {
          if (_isLoadingInventory == true) return;
          if ($('.inventory-done').is(':visible')) return;

          if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
              LoadInventory($('.list-group-item').length + 1);
          }
      }

      if ($(this).scrollTop() > 100) {
          $('.scrollup').fadeIn();
      } else {
          $('.scrollup').fadeOut();
      }
  });

  $('.scrollup').click(function () {
      $('.maincontainer').animate({
          scrollTop: 0
      }, 600);
      return false;
  });

});

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

var initEditor = function (element, init) {
    top.Ts.Settings.System.read('EnableScreenR', 'True', function (enableScreenR) {
        var editorOptions = {
            plugins: "autoresize paste link code textcolor lists",
            toolbar1: "link unlink | undo redo removeformat | cut copy paste pastetext | code | outdent indent | bullist numlist",
            toolbar2: "alignleft aligncenter alignright alignjustify | forecolor backcolor | fontselect fontsizeselect | bold italic underline strikethrough blockquote",
            statusbar: false,
            branding: false,
            gecko_spellcheck: true,
            extended_valid_elements: "a[accesskey|charset|class|coords|dir<ltr?rtl|href|hreflang|id|lang|name|onblur|onclick|ondblclick|onfocus|onkeydown|onkeypress|onkeyup|onmousedown|onmousemove|onmouseout|onmouseover|onmouseup|rel|rev|shape<circle?default?poly?rect|style|tabindex|title|target|type],script[charset|defer|language|src|type]",
            content_css: "../Css/jquery-ui-latest.custom.css,../Css/editor.css",
            body_class: "ui-widget ui-widget-content",

            convert_urls: true,
            remove_script_host: false,
            relative_urls: false,
            template_external_list_url: "tinymce/jscripts/template_list.js",
            external_link_list_url: "tinymce/jscripts/link_list.js",
            external_image_list_url: "tinymce/jscripts/image_list.js",
            media_external_list_url: "tinymce/jscripts/media_list.js",
            menubar: false,
            moxiemanager_image_settings: {
                moxiemanager_rootpath: "/" + top.Ts.System.Organization.OrganizationID + "/images/",
                extensions: 'gif,jpg,jpeg,png'
            },
            paste_data_images: true,
            images_upload_url: "/Services/UserService.asmx/SaveTinyMCEPasteImage",
            setup: function (ed) {
                ed.on('init', function (e) {
                    top.Ts.System.refreshUser(function () {
                        if (top.Ts.System.User.FontFamilyDescription != "Unassigned") {
                            ed.execCommand("FontName", false, GetTinyMCEFontName(top.Ts.System.User.FontFamily));
                        }
                        else if (top.Ts.System.Organization.FontFamilyDescription != "Unassigned") {
                            ed.execCommand("FontName", false, GetTinyMCEFontName(top.Ts.System.Organization.FontFamily));
                        }

                        if (top.Ts.System.User.FontSize != "0") {
                            ed.execCommand("FontSize", false, top.Ts.System.User.FontSizeDescription);
                        }
                        else if (top.Ts.System.Organization.FontSize != "0") {
                            ed.execCommand("FontSize", false, top.Ts.System.Organization.FontSizeDescription);
                        }
                    });
                });

                ed.on('paste', function (ed, e) {
                    setTimeout(function () { ed.execCommand('mceAutoResize'); }, 1000);
                });
            }
            , oninit: init
        };
        $(element).tinymce(editorOptions);
    });
}

var appendCustomValues = function (fields) {
  if (fields === null || fields.length < 1) {
    $('.customProperties').empty();
    return;
  }
  var containerL = $('#customPropertiesL').empty();
  var containerR = $('#customPropertiesR').empty();


  for (var i = 0; i < fields.length; i++) {
    var item = null;

    var field = fields[i];

    var div = $('<div>').addClass('form-group').data('field', field);
    $('<label>')
          .addClass('col-md-4 control-label')
          .text(field.Name)
          .appendTo(div);

    switch (field.FieldType) {
      case top.Ts.CustomFieldType.Text: appendCustomEdit(field, div); break;
      case top.Ts.CustomFieldType.Date: appendCustomEditDate(field, div); break;
      case top.Ts.CustomFieldType.Time: appendCustomEditTime(field, div); break;
      case top.Ts.CustomFieldType.DateTime: appendCustomEditDateTime(field, div); break;
      case top.Ts.CustomFieldType.Boolean: appendCustomEditBool(field, div); break;
      case top.Ts.CustomFieldType.Number: appendCustomEditNumber(field, div); break;
      case top.Ts.CustomFieldType.PickList: appendCustomEditCombo(field, div); break;
      default:
    }

    if (i % 2)
      containerR.append(div);
    else
      containerL.append(div);

  }
  $('.customProperties p').toggleClass("editable");
  //$('#contactName').toggleClass("editable");
}

var appendCustomEditCombo = function (field, element) {
  var div = $('<div>')
    .addClass('col-md-8')
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
        top.Ts.System.logAction('Product Version Detail - Edit Custom Combobox');
        var container = $('<div>')
            .insertAfter(parent);

        var container1 = $('<div>')
          .addClass('col-md-9')
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
              $('#productEdit').removeClass("disabled");
            })
            .insertAfter(container1);

        $('#' + field.Name.replace(/\W/g, '')).on('change', function () {
          var value = $(this).val();
          container.remove();

          if (field.IsRequired && field.IsFirstIndexSelect == true && $(this).find('option:selected').index() < 1) {
            alert("This field is required and the first value is not a valid selection for a required field.");
          }
          else {
            top.Ts.System.logAction('Product Version Detail - Save Custom Edit Change');
            top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productVersionID, value, function (result) {
              parent.closest('.form-group').data('field', result);
              parent.text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
              $('#productEdit').removeClass("disabled");
            }, function () {
              alert("There was a problem saving your product property.");
              $('#productEdit').removeClass("disabled");
            });
            result.parent().removeClass('has-error');
            result.removeClass('form-control');
            result.parent().children('.help-block').remove();
          }
          parent.show();
          $('#productEdit').removeClass("disabled");
        });

        $('#productEdit').addClass("disabled");
      });
  var items = field.ListValues.split('|');
  if (field.IsRequired && ((field.IsFirstIndexSelect == true && (items[0] == field.Value || field.Value == null || $.trim(field.Value) === '')) || (field.Value == null || $.trim(field.Value) === ''))) {
    result.parent().addClass('has-error');
    result.addClass('form-control');
    if (result.parent().children('.help-block').length == 0) {
      $('<label>')
        .addClass('help-block')
        .text('This field is required')
        .appendTo(result.parent());
    }
  }
}

var appendCustomEditNumber = function (field, element) {
  var div = $('<div>')
    .addClass('col-md-8')
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
        top.Ts.System.logAction('Product Version Detail - Edit Custom Number');
        var container = $('<div>')
            .insertAfter(parent);

        var container1 = $('<div>')
          .addClass('col-md-9')
          .appendTo(container);

        var fieldValue = parent.closest('.form-group').data('field').Value;
        var input = $('<input type="text">')
            .addClass('col-md-10 form-control number')
            .val(fieldValue)
            .appendTo(container1)
            .focus();

        $('<i>')
            .addClass('col-md-1 fa fa-times')
            .click(function (e) {
              $(this).closest('div').remove();
              parent.show();
              $('#productEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('<i>')
            .addClass('col-md-1 fa fa-check')
            .click(function (e) {
              var value = input.val();
              container.remove();
              if (field.IsRequired && (value === null || $.trim(value) === '')) {
                alert("This field is required");
              }
              else {
                top.Ts.System.logAction('Product Version Detail - Save Custom Number Edit');
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productVersionID, value, function (result) {
                  parent.closest('.form-group').data('field', result);
                  parent.text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
                  $('#productEdit').removeClass("disabled");
                }, function () {
                  alert("There was a problem saving your product property.");
                  $('#productEdit').removeClass("disabled");
                });
                result.parent().removeClass('has-error');
                result.removeClass('form-control');
                result.parent().children('.help-block').remove();
              }
              parent.show();
              $('#productEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('#productEdit').addClass("disabled");
      });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
    result.addClass('form-control');
    if (result.parent().children('.help-block').length == 0) {
      $('<label>')
        .addClass('help-block')
        .text('This field is required')
        .appendTo(result.parent());
    }
  }
}

var appendCustomEditBool = function (field, element) {

  var div = $('<div>')
    .addClass('col-md-8')
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
        top.Ts.System.logAction('Product Version Detail - Edit Custom Boolean Value');
        var parent = $(this);
        var value = $(this).text() === 'No' || $(this).text() === 'False' ? true : false;
        top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productVersionID, value, function (result) {
          parent.closest('.form-group').data('field', result);
          parent.text((result.Value === null || $.trim(result.Value) === '' ? 'False' : result.Value));
        }, function () {
          alert("There was a problem saving your product property.");
        });
      });
}

var appendCustomEdit = function (field, element) {

  var div = $('<div>')
    .addClass('col-md-8')
    .appendTo(element);

  var result = $('<p>')
      .html((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : getUrls(field.Value)))
      .addClass('form-control-static editable')
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
          top.Ts.System.logAction('Product Version Detail - Edit Custom Textbox');
          var container = $('<div>')
                .insertAfter(parent);

          var container1 = $('<div>')
              .addClass('col-md-9')
              .appendTo(container);

          var fieldValue = parent.closest('.form-group').data('field').Value;
          var input = $('<input type="text">')
                .addClass('col-md-10 form-control')
                .val(fieldValue)
                .appendTo(container1)
                .focus();

          if (field.Mask) {
            input.mask(field.Mask);
            input.attr("placeholder", field.Mask);
          }

          $('<i>')
                .addClass('col-md-1 fa fa-times')
                .click(function (e) {
                  $(this).closest('div').remove();
                  parent.show();
                  $('#productEdit').removeClass("disabled");
                })
                .insertAfter(container1);
          $('<i>')
                .addClass('col-md-1 fa fa-check')
                .click(function (e) {
                  var value = input.val();
                  container.remove();
                  if (field.IsRequired && (value === null || $.trim(value) === '')) {
                    alert("This field is required");
                  }
                  else {
                    top.Ts.System.logAction('Product Version Detail - Save Custom Textbox Edit');
                    top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productVersionID, value, function (result) {
                      parent.closest('.form-group').data('field', result);
                      parent.html((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : getUrls(result.Value)));
                      $('#productEdit').removeClass("disabled");
                    }, function () {
                      alert("There was a problem saving your product property.");
                      $('#productEdit').removeClass("disabled");
                    });
                    result.parent().removeClass('has-error');
                    result.removeClass('form-control');
                    result.parent().children('.help-block').remove();
                  }
                  parent.show();
                })
                .insertAfter(container1);
          $('#productEdit').addClass("disabled");
        }
      });

  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
    result.addClass('form-control');
    if (result.parent().children('.help-block').length == 0) {
      $('<label>')
        .addClass('help-block')
        .text('This field is required')
        .appendTo(result.parent());
    }
  }
}

var appendCustomEditDate = function (field, element) {
  var date = field.Value == null ? null : top.Ts.Utils.getMsDate(field.Value);

  var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);

  var result = $('<p>')
      .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDatePattern())))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
          return false;
        var parent = $(this).hide();
        top.Ts.System.logAction('Product Version Detail - Edit Custom Date');
        var container = $('<div>')
            .insertAfter(parent);

        var container1 = $('<div>')
          .addClass('col-xs-9')
          .appendTo(container);

        var fieldValue = parent.closest('.form-group').data('field').Value;
        var input = $('<input type="text">')
            .addClass('col-xs-10 form-control')
			.val(fieldValue === null ? '' : moment(fieldValue).format(top.Ts.Utils.getDatePattern().toUpperCase()))
            .datetimepicker({ pickTime: false })
            .appendTo(container1)
            .focus();

        $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
              $(this).closest('div').remove();
              parent.show();
              $('#productEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
              var value = top.Ts.Utils.getMsDate(input.val());
              container.remove();
              if (field.IsRequired && (value === null || $.trim(value) === '')) {
                // Currently there is no way to clear a Date.
                // If ever implemented this alert will prevent clearing a required date.
                alert("This field is required");
              }
              else {
                top.Ts.System.logAction('Product Version Detail - Save Custom Date Change');
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productVersionID, value, function (result) {
                  parent.closest('.form-group').data('field', result);
                  var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
                  parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDatePattern())))
                  $('#productEdit').removeClass("disabled");
                }, function () {
                  alert("There was a problem saving your product property.");
                  $('#productEdit').removeClass("disabled");
                });
                result.parent().removeClass('has-error');
                result.removeClass('form-control');
                result.parent().children('.help-block').remove();
              }
              parent.show();
            })
            .insertAfter(container1);
        $('#productEdit').addClass("disabled");
      });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
    result.addClass('form-control');
    if (result.parent().children('.help-block').length == 0) {
      $('<label>')
        .addClass('help-block')
        .text('This field is required')
        .appendTo(result.parent());
    }
  }

}

var appendCustomEditDateTime = function (field, element) {
  var date = field.Value == null ? null : top.Ts.Utils.getMsDate(field.Value);

  var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);

  var result = $('<p>')
      .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDateTimePattern())))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
          return false;
        var parent = $(this).hide();
        top.Ts.System.logAction('Product Version Detail - Edit Custom DateTime');
        var container = $('<div>')
            .insertAfter(parent);

        var container1 = $('<div>')
          .addClass('col-xs-9')
          .appendTo(container);

        var fieldValue = parent.closest('.form-group').data('field').Value;
        var input = $('<input type="text">')
            .addClass('col-xs-10 form-control')
            .val(fieldValue === null ? '' : fieldValue.localeFormat(top.Ts.Utils.getDateTimePattern()))
            .datetimepicker({
            })

            .appendTo(container1)
            .focus();

        $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
              $(this).closest('div').remove();
              parent.show();
              $('#productEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
              var value = top.Ts.Utils.getMsDate(input.val());
              container.remove();
              if (field.IsRequired && (value === null || $.trim(value) === '')) {
                // Currently there is no way to clear a Date.
                // If ever implemented this alert will prevent clearing a required date.
                alert("This field is required");
              }
              else {
                top.Ts.System.logAction('Product Version Detail - Save Custom DateTime');
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productVersionID, value, function (result) {
                  parent.closest('.form-group').data('field', result);
                  var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
                  parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDateTimePattern())))
                  $('#productEdit').removeClass("disabled");
                }, function () {
                  alert("There was a problem saving your product property.");
                  $('#productEdit').removeClass("disabled");
                });
                result.parent().removeClass('has-error');
                result.removeClass('form-control');
                result.parent().children('.help-block').remove();
              }
              parent.show();
            })
            .insertAfter(container1);
        $('#productEdit').addClass("disabled");
      });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
    result.addClass('form-control');
    if (result.parent().children('.help-block').length == 0) {
      $('<label>')
        .addClass('help-block')
        .text('This field is required')
        .appendTo(result.parent());
    }
  }

}

var appendCustomEditTime = function (field, element) {
  var date = field.Value == null ? null : field.Value;

  var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);

  var result = $('<p>')
      .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getTimePattern())))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
          return false;
        var parent = $(this).hide();
        top.Ts.System.logAction('Product Version Detail - Edit Custom Time');
        var container = $('<div>')
            .insertAfter(parent);

        var container1 = $('<div>')
          .addClass('col-xs-9')
          .appendTo(container);

        var fieldValue = parent.closest('.form-group').data('field').Value;
        var input = $('<input type="text">')
            .addClass('col-xs-10 form-control')
            .val(fieldValue === null ? '' : fieldValue.localeFormat(top.Ts.Utils.getTimePattern()))
            .datetimepicker({ pickDate: false })

            .appendTo(container1)
            .focus();

        $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
              $(this).closest('div').remove();
              parent.show();
              $('#productEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
              var value = top.Ts.Utils.getMsDate("1/1/1900 " + input.val());
              container.remove();
              if (field.IsRequired && (value === null || $.trim(value) === '')) {
                // Currently there is no way to clear a Date.
                // If ever implemented this alert will prevent clearing a required date.
                alert("This field is required");
              }
              else {
                top.Ts.System.logAction('Product Version Detail - Save Custom Time');
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productVersionID, value, function (result) {
                  parent.closest('.form-group').data('field', result);
                  var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
                  parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getTimePattern())))
                  $('#productEdit').removeClass("disabled");
                }, function () {
                  alert("There was a problem saving your product property.");
                  $('#productEdit').removeClass("disabled");
                });
                result.parent().removeClass('has-error');
                result.removeClass('form-control');
                result.parent().children('.help-block').remove();
              }
              parent.show();
            })
            .insertAfter(container1);
        $('#productEdit').addClass("disabled");
      });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
    result.addClass('form-control');
    if (result.parent().children('.help-block').length == 0) {
      $('<label>')
        .addClass('help-block')
        .text('This field is required')
        .appendTo(result.parent());
    }
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