var _productFamilyID = null;
var _execGetCustomer = null;
var _headersLoaded = false;
var _isLoadingProducts = false;
var _isLoadingCustomers = false;
var _isLoadingInventory = false;
var _viewingProducts = false;
var _viewingCustomers = false;
var _viewingInventory = false;
var _customersSortColumn = 'Date Created';
var _customersSortDirection = 'DESC';

$(document).ready(function () {
    _productFamilyID = window.parent.parent.Ts.Utils.getQueryValue("productfamilyid", window);

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

    window.parent.parent.Ts.Services.Products.GetProductFamily(_productFamilyID, function (productFamily) {
        if (productFamily == null) {
            alert('This product line has been deleted.');
            window.parent.parent.Ts.MainPage.closeNewProductFamilyTab(_productFamilyID);
        }
        else {
            $('#productFamilyName').text(productFamily.Name);
            $('#fieldDescription').html(productFamily.Description != null && productFamily.Description != "" ? productFamily.Description : "Empty");
            parent.privateServices.SetUserSetting('SelectedProductFamilyID', _productFamilyID);
        }
    });

    $('.product-family-tooltip').tooltip({ placement: 'bottom', container: 'body' });

    $('.productFamilyProperties p').toggleClass("editable");
    $('.productFamilyProperties span').toggleClass("editable");

    $('#productFamilyEdit').click(function (e) {
        window.parent.parent.Ts.System.logAction('Product Line Detail - Product Edit');
        $('.productFamilyProperties p').toggleClass("editable");
        $('.productFamilyProperties span').toggleClass("editable");
        $('.customProperties p').toggleClass("editable");

        $(this).toggleClass("btn-primary");
        $(this).toggleClass("btn-success");
        $('#productFamilyTabs a:first').tab('show');
    });

    $('#productFamilyRefresh').click(function (e) {
        window.parent.parent.Ts.System.logAction('Product Line Detail - Refresh Page');
        window.location = window.location;
    });


    $('#productFamilyDelete').click(function (e) {
        if (confirm('Are you sure you would like to remove this product line?  Any Customer Hub associations with this product line will be removed as well.')) {
            window.parent.parent.Ts.System.logAction('Product Line Detail - Delete Product Line');
            parent.privateServices.DeleteProductFamily(_productFamilyID, function (e) {
                window.parent.parent.Ts.MainPage.closeNewProductFamilyTab(_productFamilyID);
            });
        }
    });

    $('#productFamilyTabs a:first').tab('show');

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        $('.scrollup').fadeOut();
        if (e.target.innerHTML == "Details") {
            createTestChart();
            LoadCustomProperties();
            _viewingCustomers = false;
            _viewingInventory = false;
            _viewingProducts = false;
        }
        else if (e.target.innerHTML == "Products") {
            LoadProducts();
            _viewingCustomers = false;
            _viewingInventory = false;
            _viewingProducts = true;
        }
        else if (e.target.innerHTML == "Customers") {
            //LoadCustomers();
            _viewingCustomers = true;
            _viewingInventory = false;
            _viewingProducts = false;
        }
        else if (e.target.innerHTML == "Tickets") {
            //$('#ticketIframe').attr("src", "../../../Frames/TicketTabsAll.aspx?tf_ProductFamilyID=" + _productFamilyID);
            _viewingCustomers = false;
            _viewingInventory = false;
            _viewingProducts = false;
        }
        else if (e.target.innerHTML == "Knowledge Base") {
            //$('#kbIframe').attr("src", "../../../Frames/TicketTabsAll.aspx?tf_IsKnowledgeBase=true&tf_ProductFamilyID=" + _productFamilyID);
            _viewingCustomers = false;
            _viewingInventory = false;
            _viewingProducts = false;
        }
        else if (e.target.innerHTML == "Watercooler") {
            //$('#watercoolerIframe').attr("src", "WaterCooler.html?pagetype=1&pageid=" + _productFamilyID);
            _viewingCustomers = false;
            _viewingInventory = false;
            _viewingProducts = false;
        }
        else if (e.target.innerHTML == "Inventory") {
            //LoadInventory();
            _viewingCustomers = false;
            _viewingInventory = true;
            _viewingProducts = false;
        }
    })

    function createTestChart() {
        var greenLimit, yellowLimit;

        window.parent.parent.Ts.Services.Products.LoadFamilyChartData(_productFamilyID, true, function (chartString) {

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

        window.parent.parent.Ts.Services.Products.LoadFamilyChartData(_productFamilyID, false, function (chartString) {

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

    window.parent.parent.Ts.Services.Products.GetProductFamilyTickets(_productFamilyID, 0, function (e) {
        $('#openTicketCount').text("Open Tickets: " + e);
    });

    window.parent.parent.Ts.Services.Tickets.Load5MostRecentByProductFamilyID(_productFamilyID, function (tickets) {
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

              window.parent.parent.Ts.MainPage.openTicket($(this).closest('.ticket').data('o').TicketNumber, true);
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
        window.parent.parent.Ts.Services.Assets.GetCustomValues(_productFamilyID, window.parent.parent.Ts.ReferenceTypes.ProductFamilies, function (html) {
            appendCustomValues(html);
        });
    }

    var historyLoaded = 0;

    $('#historyToggle').on('click', function () {
        window.parent.parent.Ts.System.logAction('Product Line Detail - History Toggle');
        if (historyLoaded == 0) {
            historyLoaded = 1;
            LoadHistory(1);
        }
    });

    $('#historyRefresh').on('click', function () {
        window.parent.parent.Ts.System.logAction('Product Line Detail - History Refresh');
        LoadHistory(1);
    });

    function LoadHistory(start) {

        if (start == 1)
            $('#tblHistory tbody').empty();

        window.parent.parent.Ts.Services.Products.LoadFamilyHistory(_productFamilyID, start, function (history) {
            for (var i = 0; i < history.length; i++) {
                $('<tr>').html('<td>' + history[i].DateCreated.localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()) + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td>')
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

    $('#productFamilyName').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;

        window.parent.parent.Ts.System.logAction('Product Line Detail - Edit Name');
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
              $('#productFamilyEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
              window.parent.parent.Ts.System.logAction('Product Line Detail - Save Name Edit');
              window.parent.parent.Ts.Services.Products.SetFamilyName(_productFamilyID, $(this).prev().find('input').val(), function (result) {
                  header.text(result);
                  $('#productFamilyName').text(result);
                  $('#productFamilyEdit').removeClass("disabled");
              },
                            function (error) {
                                header.show();
                                alert('There was an error saving the product line name.');
                                $('#productFamilyEdit').removeClass("disabled");
                            });
              $('#productFamilyEdit').removeClass("disabled");
              $(this).closest('div').remove();
              header.show();
          })
          .insertAfter(container1);
        $('#productFamilyEdit').addClass("disabled");
    });

    //  initEditor($('#fieldDesc'), function (ed) {
    //      $('#fieldDesc').tinymce().focus();
    //  });

    $('#fieldDescription').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        window.parent.parent.Ts.System.logAction('Product Line Detail - Edit Description');
        window.parent.parent.Ts.Services.Products.GetProductFamily(_productFamilyID, function (productFamily) {
            var desc = productFamily.Description;
            desc = desc.replace(/<br\s?\/?>/g, "\n");
            //        $('#fieldDesc').tinymce().setContent(desc);
            //        $('#fieldDesc').tinymce().focus();
            $('#fieldDesc').html(desc);
            $('#descriptionForm').show();
        });

        $('#btnDescriptionCancel').click(function (e) {
            e.preventDefault();
            $('#descriptionForm').hide();
            header.show();
            $('#productFamilyEdit').removeClass("disabled");
        });

        $('#btnDescriptionSave').click(function (e) {
            e.preventDefault();
            window.parent.parent.Ts.System.logAction('Product Line Detail - Save Description Edit');
            window.parent.parent.Ts.Services.Products.SetFamilyDescription(_productFamilyID, $(this).prev().find('textarea').val(), function (result) {
                header.html(result);
                $('#productFamilyEdit').removeClass("disabled");
            },
            function (error) {
                header.show();
                alert('There was an error saving the product line description.');
                $('#productFamilyEdit').removeClass("disabled");
            });
            $('#descriptionForm').hide();
            header.show();
        })
        $('#productFamilyEdit').addClass("disabled");
    });

    //$('.product-action-add').click(function (e) {
    //    e.preventDefault();
    //    window.parent.parent.Ts.MainPage.newProduct("product", _productFamilyID);
    //});

    function LoadProducts(start) {
        start = start || 0;
        showProductsLoadingIndicator();
        $('.productList').fadeTo(200, 0.5);
        window.parent.parent.Ts.Services.Products.LoadFamilyProducts2(_productFamilyID, start, function (versions) {
            $('.productList').fadeTo(0, 1);

            if (start == 0) {
                insertProduct(versions);
            } else {
                appendProducts(versions);
            }
        });
    }

    function showProductsLoadingIndicator() {
        _isLoadingProducts = true;
        $('.products-loading').show();
    }

    function insertProduct(products) {
        $('.productList').empty();

        if (products.HTML.length < 1) {
            $('.products-loading').hide();
            $('.products-more').hide();
            $('.products-done').hide();
            $('.products-empty').show();
        } else {
            appendProducts(products);
        }
        _isLoadingProducts = false;
    }

    function appendProducts(products) {
        $('.products-loading').hide();
        $('.products-empty').hide();
        $('.products-more').hide();
        $('.products-done').hide();

        if (products.HTML.length < 1) {
            $('.products-done').show();
        } else {
            $('.productList').append(products.HTML)
            if (products.MoreAvailable) {
                $('.products-more').show();
            }
            else {
                //$('.products-done').show();
            }

        }
        _isLoadingProducts = false;
    }

    $('.maincontainer').bind('scroll', function () {
        if (_viewingProducts) {
            if (_isLoadingProducts == true) return;
            if ($('.products-done').is(':visible')) return;

            if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
                LoadProducts($('.list-group-item').length + 1);
            }
        }
        else if (_viewingCustomers) {
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

    $('#moreProducts').click(function (e) {
        LoadProducts($('.list-group-item').length + 1);
    });

    $('.productList').on('click', '.productlink', function (e) {
        e.preventDefault();

        var id = $(this).attr('id');
        window.parent.parent.Ts.System.logAction('Product Line Detail Page - View Product');
        window.parent.parent.Ts.MainPage.openNewProduct(id);

        window.parent.parent.Ts.Services.Products.UpdateRecentlyViewed('p' + id, function (resultHtml) {
            $('.recent-container').empty();
            $('.recent-container').html(resultHtml);
        });

    });

    var _isAdmin = window.parent.parent.Ts.System.User.IsSystemAdmin;
    if (!window.parent.parent.Ts.System.User.CanEditCompany && !_isAdmin) {
        $('#customerToggle').hide();
    }

    if (!window.parent.parent.Ts.System.User.CanEditProducts && !_isAdmin) {
        $('#productFamilyEdit').remove();
        $('.product-action-add').hide();
    }

    //if (!window.parent.parent.Ts.System.User.CanCreateVersions && !_isAdmin) {
    //    $('.version-action-add').hide();
    //}


    if (!_isAdmin) {
        $('#productFamilyDelete').hide();
        $('#associateAllToggle').hide();
        $('#unAssociateAllToggle').hide();
    }

    $('#customerToggle').click(function (e) {
        window.parent.parent.Ts.System.logAction('Product Line Detail - Toggle Customer Form');
        $('#customerForm').toggle();
    });

    $('#associateAllToggle').click(function (e) {
        if (confirm('Are you sure you would like to associate All customers to this product?')) {
            window.parent.parent.Ts.System.logAction('Product Line Detail - Toggle Associate All Customer Form');
            window.parent.parent.Ts.Services.Customers.AssignAllCustomersToProduct(_productFamilyID, function () {
                LoadCustomers();
            }, function () {
                alert('There was an error associating all customers to this product. Please try again.');
            });
        }
    });

    $('#unAssociateAllToggle').click(function (e) {
        if (confirm('Are you sure you would like to unassociate All customers from this product?')) {
            window.parent.parent.Ts.System.logAction('Product Line Detail - Toggle Unassociate All Customer Form');
            window.parent.parent.Ts.Services.Customers.UnassignAllCustomersFromProduct(_productFamilyID, function () {
                LoadCustomers();
            }, function () {
                alert('There was an error unassociating all customers from this product. Please try again.');
            });
        }
    });

    function LoadCustomers(start) {

        if (!_headersLoaded) {

            window.parent.parent.Ts.Services.Customers.LoadcustomProductHeaders(function (headers) {
                for (var i = 0; i < headers.length; i++) {
                    var header = headers[i];
                    if (header == 'Product Name') {
                        header = 'Customer';
                    }
                    $('#tblCustomers th:last').after('<th>' + header + '</th>');
                }
                _headersLoaded = true;
                if (headers.length > 5) {
                    $('#customersContainer').addClass('expandCustomersContainer');
                }
            });
        }

        start = start || 0;
        showCustomersLoadingIndicator();
        $('#tblCustomers').fadeTo(200, 0.5);

        window.parent.parent.Ts.Services.Products.LoadCustomers(_productFamilyID, start, _customersSortColumn, _customersSortDirection, function (customers) {
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

                if (window.parent.parent.Ts.System.User.CanEditCompany || _isAdmin) {
                    html = '<td><i class="fa fa-edit customerEdit"></i></td><td><i class="fa fa-trash-o customerDelete"></i></td><td><a href="#" class="customerView">' + customers[i].Customer + '</a></td><td>' + customers[i].VersionNumber + '</td><td>' + customers[i].SupportExpiration + '</td><td>' + customers[i].VersionStatus + '</td><td>' + customers[i].IsReleased + '</td><td>' + customers[i].ReleaseDate + '</td><td>' + customers[i].DateCreated + '</td>' + customfields;
                }
                else {
                    html = '<td></td><td></td><td><a href="#" class="customerView">' + customers[i].Customer + '</a></td><td>' + customers[i].VersionNumber + '</td><td>' + customers[i].SupportExpiration + '</td><td>' + customers[i].VersionStatus + '</td><td>' + customers[i].IsReleased + '</td><td>' + customers[i].ReleaseDate + '</td><td>' + customers[i].DateCreated + '</td>' + customfields
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
        _execGetCustomer = window.parent.parent.Ts.Services.Organizations.GetOrganizationForTicket(request.term, function (result) { response(result); });
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

    $('.product-customers-export-excel').on('click', function (e) {
        e.preventDefault();

        window.parent.parent.Ts.System.logAction('Product Line Detail Page - Export Customers Excel');

        window.open('../../../dc/' + window.parent.parent.Ts.System.Organization.OrganizationID + '/productcustomers/' + _productFamilyID + '?Type=EXCEL', 'ProductCustomersDownload');

    });

    $('.product-customers-export-csv').on('click', function (e) {
        e.preventDefault();

        window.parent.parent.Ts.System.logAction('Product Line Detail Page - Export Customers CSV');

        window.open('../../../dc/' + window.parent.parent.Ts.System.Organization.OrganizationID + '/productcustomers/' + _productFamilyID + '?Type=CSV', 'ProductCustomersDownload');

    });

    LoadProductVersions();
    function LoadProductVersions() {
        $("#productVersion").empty();

        window.parent.parent.Ts.Services.Customers.LoadProductVersions(_productFamilyID, function (pt) {
            $('<option>').attr('value', '-1').text('Unassigned').appendTo('#productVersion');
            for (var i = 0; i < pt.length; i++) {
                var opt = $('<option>').attr('value', pt[i].ProductVersionID).text(pt[i].VersionNumber).data('o', pt[i]);
                //              if (pt[i].ProductVersionID == selVal)
                //                  opt.attr('selected', 'selected');
                opt.appendTo('#productVersion');
            }
        });
    }

    window.parent.parent.Ts.Services.Customers.GetDateFormat(false, function (dateformat) {
        $('.datepicker').attr("data-format", dateformat);
        $('.datepicker').datetimepicker({ pickTime: false });

        $('#productExpiration').attr("data-format", dateformat);
        $('.datetimepicker').datetimepicker({});
    });

    LoadCustomControls(window.parent.parent.Ts.ReferenceTypes.OrganizationProducts);
    function LoadCustomControls(refType) {
        window.parent.parent.Ts.Services.Customers.LoadCustomControls(refType, function (html) {
            $('#customProductsControls').append(html);

            $('#customProductsControls .datepicker').datetimepicker({ pickTime: false });
            $('#customProductsControls .datetimepicker').datetimepicker({});
            $('#customProductsControls .timepicker ').datetimepicker({ pickDate: false });
        });
    }

    $('#btnCustomerSave').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        window.parent.parent.Ts.System.logAction('Product Line Detail - Save Customer');
        var productInfo = new Object();
        var hasError = 0;
        productInfo.OrganizationID = $('#inputCustomer').data('item').id;
        productInfo.ProductID = _productFamilyID;
        productInfo.Version = $("#productVersion").val();
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
                    //    field.Value = $(this).val() == "" ? null : window.parent.parent.Ts.Utils.getMsDate($(this).val());
                    //    break;
                    //case "time":
                    //    field.Value = $(this).val() == "" ? null : window.parent.parent.Ts.Utils.getMsDate("1/1/1900 " + $(this).val());
                    //    break;
                    //case "datetime":
                    //    field.Value = $(this).val() == "" ? null : window.parent.parent.Ts.Utils.getMsDate($(this).val());
                    //    break;
                default:
                    field.Value = $(this).val();
            }
            productInfo.Fields[productInfo.Fields.length] = field;
        });

        if (hasError == 0) {
            window.parent.parent.Ts.Services.Customers.SaveProduct(parent.JSON.stringify(productInfo), function (prod) {
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
                alert('There was an error saving this product association. Please try again.');
            });
        }

    });

    $("#btnCustomerCancel").click(function (e) {
        e.preventDefault();
        window.parent.parent.Ts.System.logAction('Product Line Detail - Cancel Customer Edit');
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
    });

    $('#tblCustomers').on('click', '.customerEdit', function (e) {
        e.preventDefault();
        var organizationProductID = $(this).parent().parent().attr('id');
        //var orgproductID;
        window.parent.parent.Ts.System.logAction('Product Line Detail - Edit Customer');
        window.parent.parent.Ts.Services.Products.LoadCustomer(organizationProductID, function (organizationProduct) {
            //orgproductID = prod.OrganizationProductID;
            SetVersion(organizationProduct.VersionNumber);
            var item = new Object();
            item.label = organizationProduct.Customer;
            item.value = organizationProduct.Customer;
            item.id = organizationProduct.OrganizationID;
            item.data = "o";
            $('#inputCustomer').data('item', item);
            $('#inputCustomer').val(organizationProduct.Customer);
            $('#supportExpiration').val(organizationProduct.SupportExpiration);
            $('#fieldOrganizationProductID').val(organizationProductID);
            window.parent.parent.Ts.Services.Customers.LoadCustomProductFields(organizationProductID, function (custField) {
                for (var i = 0; i < custField.length; i++) {
                    if (custField[i].FieldType == 2 && custField[i].Value == "True")
                        $('#' + custField[i].CustomFieldID).attr('checked', true);
                        //else if (custField[i].FieldType == 5)
                        //{
                        //    var date = field.value == null ? null : window.parent.parent.Ts.Utils.getMsDate(field.Value);
                        //    $('#' + custField[i].CustomFieldID).val(date.localeFormat(window.parent.parent.Ts.Utils.getDatePattern()));
                        //}

                    else
                        $('#' + custField[i].CustomFieldID).val(custField[i].Value);
                }
            });
        });
        $('#customerForm').show();
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

    function SetVersion(selVal) {
        $("#productVersion").children().removeAttr("selected");
        $("#productVersion > option").each(function () {
            if (this.value == selVal) {
                this.setAttribute('selected', 'selected');
            }
        });
    }

    $('#tblCustomers').on('click', '.customerDelete', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you would like to remove this customer association?')) {
            window.parent.parent.Ts.System.logAction('Product Line Detail - Delete Customer');
            parent.privateServices.DeleteOrganizationProduct($(this).parent().parent().attr('id'), false, function (e) {
                LoadCustomers();
            });

        }
    });

    $('#tblCustomers').on('click', '.customerView', function (e) {
        e.preventDefault();
        window.parent.parent.Ts.System.logAction('Product Line Detail - View Customer');
        window.parent.parent.Ts.MainPage.openProductOrganization($(this).parent().parent().attr('id'))
        //parent.location = "../../../Default.aspx?OrganizationProductID=" + ;

    });

    function LoadInventory(start) {
        start = start || 0;
        showAssetsLoadingIndicator();
        $('.assetList').fadeTo(200, 0.5);

        window.parent.parent.Ts.Services.Products.LoadAssets(_productFamilyID, start, function (assets) {
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
        window.parent.parent.Ts.System.logAction('Product Line Detail - Open Asset From List');
        window.parent.parent.Ts.MainPage.openNewAsset(this.id);
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
    window.parent.parent.Ts.Settings.System.read('EnableScreenR', 'True', function (enableScreenR) {
        var editorOptions = {
            plugins: "autoresize paste link code textcolor",
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
                moxiemanager_rootpath: "/" + window.parent.parent.Ts.System.Organization.OrganizationID + "/images/",
                extensions: 'gif,jpg,jpeg,png'
            },
            paste_data_images: true,
            images_upload_url: "/Services/UserService.asmx/SaveTinyMCEPasteImage",
            setup: function (ed) {
                ed.on('init', function (e) {
                    window.parent.parent.Ts.System.refreshUser(function () {
                        if (window.parent.parent.Ts.System.User.FontFamilyDescription != "Unassigned") {
                            ed.execCommand("FontName", false, GetTinyMCEFontName(window.parent.parent.Ts.System.User.FontFamily));
                        }
                        else if (window.parent.parent.Ts.System.Organization.FontFamilyDescription != "Unassigned") {
                            ed.execCommand("FontName", false, GetTinyMCEFontName(window.parent.parent.Ts.System.Organization.FontFamily));
                        }

                        if (window.parent.parent.Ts.System.User.FontSize != "0") {
                            ed.execCommand("FontSize", false, window.parent.parent.Ts.System.User.FontSizeDescription);
                        }
                        else if (window.parent.parent.Ts.System.Organization.FontSize != "0") {
                            ed.execCommand("FontSize", false, window.parent.parent.Ts.System.Organization.FontSizeDescription);
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
            case window.parent.parent.Ts.CustomFieldType.Text: appendCustomEdit(field, div); break;
            case window.parent.parent.Ts.CustomFieldType.Date: appendCustomEditDate(field, div); break;
            case window.parent.parent.Ts.CustomFieldType.Time: appendCustomEditTime(field, div); break;
            case window.parent.parent.Ts.CustomFieldType.DateTime: appendCustomEditDateTime(field, div); break;
            case window.parent.parent.Ts.CustomFieldType.Boolean: appendCustomEditBool(field, div); break;
            case window.parent.parent.Ts.CustomFieldType.Number: appendCustomEditNumber(field, div); break;
            case window.parent.parent.Ts.CustomFieldType.PickList: appendCustomEditCombo(field, div); break;
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
            window.parent.parent.Ts.System.logAction('Product Line Detail - Edit Custom Combobox');
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
                    $('#productFamilyEdit').removeClass("disabled");
                })
                .insertAfter(container1);

            $('#' + field.Name.replace(/\W/g, '')).on('change', function () {
                var value = $(this).val();
                container.remove();

                if (field.IsRequired && field.IsFirstIndexSelect == true && $(this).find('option:selected').index() < 1) {
                    alert("This field is required and the first value is not a valid selection for a required field.");
                }
                else {
                    window.parent.parent.Ts.System.logAction('Product Line Detail - Save Custom Edit Change');
                    window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productFamilyID, value, function (result) {
                        parent.closest('.form-group').data('field', result);
                        parent.text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
                        $('#productFamilyEdit').removeClass("disabled");
                    }, function () {
                        alert("There was a problem saving your product property.");
                        $('#productFamilyEdit').removeClass("disabled");
                    });
                    result.parent().removeClass('has-error');
                    result.removeClass('form-control');
                    result.parent().children('.help-block').remove();
                }
                parent.show();
                $('#productFamilyEdit').removeClass("disabled");
            });

            $('#productFamilyEdit').addClass("disabled");
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
            window.parent.parent.Ts.System.logAction('Product Line Detail - Edit Custom Number');
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
                    $('#productFamilyEdit').removeClass("disabled");
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
                        window.parent.parent.Ts.System.logAction('Product Line Detail - Save Custom Number Edit');
                        window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productFamilyID, value, function (result) {
                            parent.closest('.form-group').data('field', result);
                            parent.text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
                            $('#productFamilyEdit').removeClass("disabled");
                        }, function () {
                            alert("There was a problem saving your product property.");
                            $('#productFamilyEdit').removeClass("disabled");
                        });
                        result.parent().removeClass('has-error');
                        result.removeClass('form-control');
                        result.parent().children('.help-block').remove();
                    }
                    parent.show();
                    $('#productFamilyEdit').removeClass("disabled");
                })
                .insertAfter(container1);
            $('#productFamilyEdit').addClass("disabled");
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
            window.parent.parent.Ts.System.logAction('Product Line Detail - Edit Custom Boolean Value');
            var parent = $(this);
            var value = $(this).text() === 'No' || $(this).text() === 'False' ? true : false;
            window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productFamilyID, value, function (result) {
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
                window.parent.parent.Ts.System.logAction('Product Line Detail - Edit Custom Textbox');
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
                          $('#productFamilyEdit').removeClass("disabled");
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
                              window.parent.parent.Ts.System.logAction('Product Line Detail - Save Custom Textbox Edit');
                              window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productFamilyID, value, function (result) {
                                  parent.closest('.form-group').data('field', result);
                                  parent.html((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : getUrls(result.Value)));
                                  $('#productFamilyEdit').removeClass("disabled");
                              }, function () {
                                  alert("There was a problem saving your product property.");
                                  $('#productFamilyEdit').removeClass("disabled");
                              });
                              result.parent().removeClass('has-error');
                              result.removeClass('form-control');
                              result.parent().children('.help-block').remove();
                          }
                          parent.show();
                      })
                      .insertAfter(container1);
                $('#productFamilyEdit').addClass("disabled");
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
    var date = field.Value == null ? null : window.parent.parent.Ts.Utils.getMsDate(field.Value);

    var div = $('<div>')
      .addClass('col-xs-8')
      .appendTo(element);

    var result = $('<p>')
        .text((date === null ? 'Unassigned' : date.localeFormat(window.parent.parent.Ts.Utils.getDatePattern())))
        .addClass('form-control-static editable')
        .appendTo(div)
        .click(function (e) {
            e.preventDefault();
            if (!$(this).hasClass('editable'))
                return false;
            var parent = $(this).hide();
            window.parent.parent.Ts.System.logAction('Product Line Detail - Edit Custom Date');
            var container = $('<div>')
                .insertAfter(parent);

            var container1 = $('<div>')
              .addClass('col-xs-9')
              .appendTo(container);

            var fieldValue = parent.closest('.form-group').data('field').Value;
            var input = $('<input type="text">')
                .addClass('col-xs-10 form-control')
				.val(fieldValue === null ? '' : moment(fieldValue).format(window.parent.parent.Ts.Utils.getDatePattern().toUpperCase()))
                .datetimepicker({ pickTime: false })
                .appendTo(container1)
                .focus();

            $('<i>')
                .addClass('col-xs-1 fa fa-times')
                .click(function (e) {
                    $(this).closest('div').remove();
                    parent.show();
                    $('#productFamilyEdit').removeClass("disabled");
                })
                .insertAfter(container1);
            $('<i>')
                .addClass('col-xs-1 fa fa-check')
                .click(function (e) {
                    var value = window.parent.parent.Ts.Utils.getMsDate(input.val());
                    container.remove();
                    if (field.IsRequired && (value === null || $.trim(value) === '')) {
                        // Currently there is no way to clear a Date.
                        // If ever implemented this alert will prevent clearing a required date.
                        alert("This field is required");
                    }
                    else {
                        window.parent.parent.Ts.System.logAction('Product Line Detail - Save Custom Date Change');
                        window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productFamilyID, value, function (result) {
                            parent.closest('.form-group').data('field', result);
                            var date = result.Value === null ? null : window.parent.parent.Ts.Utils.getMsDate(result.Value);
                            parent.text((date === null ? 'Unassigned' : date.localeFormat(window.parent.parent.Ts.Utils.getDatePattern())))
                            $('#productFamilyEdit').removeClass("disabled");
                        }, function () {
                            alert("There was a problem saving your product property.");
                            $('#productFamilyEdit').removeClass("disabled");
                        });
                        result.parent().removeClass('has-error');
                        result.removeClass('form-control');
                        result.parent().children('.help-block').remove();
                    }
                    parent.show();
                })
                .insertAfter(container1);
            $('#productFamilyEdit').addClass("disabled");
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
    var date = field.Value == null ? null : window.parent.parent.Ts.Utils.getMsDate(field.Value);

    var div = $('<div>')
      .addClass('col-xs-8')
      .appendTo(element);

    var result = $('<p>')
        .text((date === null ? 'Unassigned' : date.localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern())))
        .addClass('form-control-static editable')
        .appendTo(div)
        .click(function (e) {
            e.preventDefault();
            if (!$(this).hasClass('editable'))
                return false;
            var parent = $(this).hide();
            window.parent.parent.Ts.System.logAction('Product Line Detail - Edit Custom DateTime');
            var container = $('<div>')
                .insertAfter(parent);

            var container1 = $('<div>')
              .addClass('col-xs-9')
              .appendTo(container);

            var fieldValue = parent.closest('.form-group').data('field').Value;
            var input = $('<input type="text">')
                .addClass('col-xs-10 form-control')
                .val(fieldValue === null ? '' : fieldValue.localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()))
                .datetimepicker({
                })

                .appendTo(container1)
                .focus();

            $('<i>')
                .addClass('col-xs-1 fa fa-times')
                .click(function (e) {
                    $(this).closest('div').remove();
                    parent.show();
                    $('#productFamilyEdit').removeClass("disabled");
                })
                .insertAfter(container1);
            $('<i>')
                .addClass('col-xs-1 fa fa-check')
                .click(function (e) {
                    var value = window.parent.parent.Ts.Utils.getMsDate(input.val());
                    container.remove();
                    if (field.IsRequired && (value === null || $.trim(value) === '')) {
                        // Currently there is no way to clear a Date.
                        // If ever implemented this alert will prevent clearing a required date.
                        alert("This field is required");
                    }
                    else {
                        window.parent.parent.Ts.System.logAction('Product Line Detail - Save Custom DateTime');
                        window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productFamilyID, value, function (result) {
                            parent.closest('.form-group').data('field', result);
                            var date = result.Value === null ? null : window.parent.parent.Ts.Utils.getMsDate(result.Value);
                            parent.text((date === null ? 'Unassigned' : date.localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern())))
                            $('#productFamilyEdit').removeClass("disabled");
                        }, function () {
                            alert("There was a problem saving your product property.");
                            $('#productFamilyEdit').removeClass("disabled");
                        });
                        result.parent().removeClass('has-error');
                        result.removeClass('form-control');
                        result.parent().children('.help-block').remove();
                    }
                    parent.show();
                })
                .insertAfter(container1);
            $('#productFamilyEdit').addClass("disabled");
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
        .text((date === null ? 'Unassigned' : date.localeFormat(window.parent.parent.Ts.Utils.getTimePattern())))
        .addClass('form-control-static editable')
        .appendTo(div)
        .click(function (e) {
            e.preventDefault();
            if (!$(this).hasClass('editable'))
                return false;
            var parent = $(this).hide();
            window.parent.parent.Ts.System.logAction('Product Line Detail - Edit Custom Time');
            var container = $('<div>')
                .insertAfter(parent);

            var container1 = $('<div>')
              .addClass('col-xs-9')
              .appendTo(container);

            var fieldValue = parent.closest('.form-group').data('field').Value;
            var input = $('<input type="text">')
                .addClass('col-xs-10 form-control')
                .val(fieldValue === null ? '' : fieldValue.localeFormat(window.parent.parent.Ts.Utils.getTimePattern()))
                .datetimepicker({ pickDate: false })

                .appendTo(container1)
                .focus();

            $('<i>')
                .addClass('col-xs-1 fa fa-times')
                .click(function (e) {
                    $(this).closest('div').remove();
                    parent.show();
                    $('#productFamilyEdit').removeClass("disabled");
                })
                .insertAfter(container1);
            $('<i>')
                .addClass('col-xs-1 fa fa-check')
                .click(function (e) {
                    var value = window.parent.parent.Ts.Utils.getMsDate("1/1/1900 " + input.val());
                    container.remove();
                    if (field.IsRequired && (value === null || $.trim(value) === '')) {
                        // Currently there is no way to clear a Date.
                        // If ever implemented this alert will prevent clearing a required date.
                        alert("This field is required");
                    }
                    else {
                        window.parent.parent.Ts.System.logAction('Product Line Detail - Save Custom Time');
                        window.parent.parent.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productFamilyID, value, function (result) {
                            parent.closest('.form-group').data('field', result);
                            var date = result.Value === null ? null : window.parent.parent.Ts.Utils.getMsDate(result.Value);
                            parent.text((date === null ? 'Unassigned' : date.localeFormat(window.parent.parent.Ts.Utils.getTimePattern())))
                            $('#productFamilyEdit').removeClass("disabled");
                        }, function () {
                            alert("There was a problem saving your product property.");
                            $('#productFamilyEdit').removeClass("disabled");
                        });
                        result.parent().removeClass('has-error');
                        result.removeClass('form-control');
                        result.parent().children('.help-block').remove();
                    }
                    parent.show();
                })
                .insertAfter(container1);
            $('#productFamilyEdit').addClass("disabled");
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