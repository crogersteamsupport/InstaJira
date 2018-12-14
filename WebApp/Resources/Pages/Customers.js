$(document).ready(function() {
    var _isAdmin = parent.Ts.System.User.IsSystemAdmin;

    if (!parent.Ts.System.User.CanCreateCompany && !parent.Ts.System.User.CanCreateContact && !_isAdmin) {
        $('.action-new').hide();
    }
    $('input, textarea').placeholder();
    if (parent.Ts.System.User.FilterInactive) {
        $('#cbActive').prop('checked', true);
    }
    $('#cbActive').click(function(e) {
        parent.Ts.Services.Users.SetInactiveFilter(parent.Ts.System.User.UserID, $('#cbActive').prop('checked'), function(result) {
                parent.Ts.System.logAction('User Info - Changed Filter Inactive Setting');
            },
            function(error) {
                alert('There was an error saving the user filter inaactive setting.');
                item.next().hide();
            });
        fetchItems();
    });

    var execFetchItems = null;

    function fetchItems(start) {
        if (execFetchItems) { execFetchItems._executor.abort();  }
        start = start || 0;
        showLoadingIndicator();
        $('.searchresults').fadeTo(200, 0.5);
        var term = $('#searchString').val();

        var searchCompanies = false;
        var parentsOnly = false;
        var searchContacts = false;
        if ($('.customers-filter-all').parent().hasClass('active')) {
            searchContacts = true;
            searchCompanies = true;
        } else if ($('.customers-filter-customers').parent().hasClass('active')) {
            searchCompanies = true;
        } else if ($('.customers-filter-contacts').parent().hasClass('active')) {
            searchContacts = true;
        } else if ($('.customers-filter-parents').parent().hasClass('active')) {
            searchCompanies = true;
            parentsOnly = true;
        }
        parent.Ts.System.logAction('Customer Page - Search Executed');
        execFetchItems = parent.Ts.Services.Search.SearchCompaniesAndContactsDt($('#searchString').val(), start, 10, searchCompanies, searchContacts, $('#cbActive').prop('checked') ? true : null, parentsOnly, function (items) {
            $('.searchresults').fadeTo(0, 1);
            if (start == 0) {
                insertSearchResults(items);
            } else {
                appendSearchResults(items);
            }
        });
    }

    function showLoadingIndicator() {
        _isLoading = true;
        $('.results-loading').show();
    }

    function insertSearchResults(items) {
        $('.searchresults').empty();
        if (items.length < 1) {
            $('.results-loading').hide();
            $('.results-done').hide();
            $('.results-empty').show();
        } else {
            appendSearchResults(items);
        }
        _isLoading = false;
    }

    function appendSearchResults(items) {
        $('.results-loading').hide();
        $('.results-empty').hide();
        $('.results-done').hide();
        if (items.length < 1) {
            $('.results-done').show();
        } else {
            var container = $('.searchresults');
            for (var i = 0; i < items.length; i++) {
                appendItem(container, JSON.parse(items[i]));
            }
        }
        _isLoading = false;
        $('.frame-container').trigger('scroll');
    }

    function appendItem(container, item) {
        var hasCustomerInsights = parent.Ts.System.Organization.IsCustomerInsightsActive;
        var organizationId = parent.Ts.System.Organization.OrganizationID;
        var el = $('<tr>');
        if (!hasCustomerInsights) {
            var circle = $('<i>').addClass('fa fa-circle fa-stack-2x');
            var icon = $('<i>').addClass('fa fa-stack-1x fa-inverse');

            $('<td>').addClass('result-icon').append(
                $('<span>').addClass('fa-stack fa-2x').append(circle).append(icon)
            ).appendTo(el);

            var div = $('<div>').addClass('peopleinfo')
            $('<td>').append(div).appendTo(el);

            if (item.userID) {
                circle.addClass('color-orange');
                icon.addClass('fa-user');
                appendContact(div, item);
            } else {
                circle.addClass('color-green');
                icon.addClass('fa-building-o');
                appendCompany(div, item);
            }
        } else {
            var image = $('<img>');
            var imagePath;
            if (item.userID) {
                imagePath = "../../../dc/" + item.organizationID + "/contactavatar/" + item.userID + "/48/index";
            } else {
                imagePath = "../../../dc/" + organizationId + "/companylogo/" + item.organizationID + "/48/index";
            }
            var imageObject = new Image();
            imageObject.src = imagePath;
            imageObject.className = "user-avatar";
            $('<td>').addClass('result-icon').append(
                $('<span>').addClass('fa-stack fa-2x').append(imageObject)
            ).appendTo(el);
            var div = $('<div>').addClass('peopleinfo')
            $('<td>').append(div).appendTo(el);
            if (item.userID) {
                appendContact(div, item);
            } else {
                appendCompany(div, item);
            }
        }
        el.appendTo(container);
    }

    function appendCompany(el, item) {
        $('<a>').attr('href', '#').addClass('companylink').data('organizationid', item.organizationID).text(!isNullOrWhiteSpace(item.name) ? item.name : 'Unnamed Company').appendTo($('<h4>').appendTo(el));
        var list = $('<ul>').appendTo(el);
        if (!isNullOrWhiteSpace(item.website)) {
            var site = item.website.indexOf('http') != 0 ? 'http://' + item.website : item.website;
            $('<a>').attr('target', '_blank').attr('href', site).text(item.website).appendTo($('<li>').appendTo(list));
        }
        var phones = $('<li>');
        appendPhones(phones, item);
        phones.appendTo(list);
        $('<li>').text(item.isPortal ? 'Has portal access' : '').appendTo(list);
    }

    function appendContact(el, item) {
        $('<a>').attr('href', '#').addClass('contactlink').text(isNullOrWhiteSpace(item.fName) && isNullOrWhiteSpace(item.lName) ? 'Unnamed Contact' : item.lName + ', ' + item.fName).data('userid', item.userID).appendTo($('<h4>').appendTo(el));
        var list = $('<ul>').appendTo(el);
        if (item.organization == '_Unknown Company') {
            if (!isNullOrWhiteSpace(item.title)) {
                $('<li>').text(item.title).appendTo(list);
            }
        } else {
            var li = $('<li>').appendTo(list);
            if (!isNullOrWhiteSpace(item.title)) {
                $('<span>').text(item.title + ' at ').appendTo(li);
            }
            $('<a>').attr('href', '#').addClass('companylink').data('organizationid', item.organizationID).text(item.organization).appendTo(li);
        }
        if (!isNullOrWhiteSpace(item.email)) {
            $('<a>').attr('target', '_blank').attr('href', 'mailto:' + item.email).text(item.email).appendTo($('<li>').appendTo(list));
        }
        var phones = $('<li>');
        appendPhones(phones, item);
        phones.appendTo(list);
        $('<li>').text(item.isPortal ? 'Has portal access' : '').appendTo(list);
    }

    function appendPhones(el, item) {
        for (var i = 0; i < item.phones.length; i++) {
            var phone = item.phones[i];
            if (!isNullOrWhiteSpace(phone.number)) {
                $('<span>').text(" " + phone.type + " ").appendTo(el);
                $('<a>').attr('href', 'tel:' + phone.number).attr('target', '_blank').text(phone.number).appendTo(el);
                if (!isNullOrWhiteSpace(phone.ext)) {
                    $('<span>').text(" Ext:" + phone.ext).appendTo(el);
                }
            }
        }
    }

    function isNullOrWhiteSpace(str) {
        return str === null || str.match(/^ *$/) !== null;
    }

    $('.customers-filter').on('click', 'a', function(e) {
        e.preventDefault();
        if ($(this).parent().hasClass('active')) return; 
        $('.customers-filter li.active').removeClass('active');
        $(this).parent().addClass('active');
        parent.Ts.System.logAction('Customer Page - Change Filter');
        fetchItems();
    });

    $('.action-new').click(function(e) {
        e.preventDefault();
        parent.Ts.System.logAction('Customer Page - New Customer');
        parent.Ts.MainPage.newCustomer();
    });

    $('.searchresults, .recent-container').on('click', '.contactlink', function(e) {
        e.preventDefault();
        var id = $(this).data('userid');
        parent.Ts.System.logAction('Customer Page - View Recent Contact');
        parent.Ts.MainPage.openNewContact(id);
        parent.Ts.Services.Customers.UpdateRecentlyViewed('u' + id, function(resultHtml) {
            $('.recent-container').empty();
            $('.recent-container').html(resultHtml);
        });
    });

    $('.searchresults, .recent-container').on('click', '.companylink', function(e) {
        e.preventDefault();
        var id = $(this).data('organizationid');
        parent.Ts.System.logAction('Customer Page - View Recent Company');
        if ($('.customers-filter-parents').parent().hasClass('active')) {
            parent.Ts.MainPage.openNewCustomerInParentView(id);
        } else {
            parent.Ts.MainPage.openNewCustomer(id);
        }
        parent.Ts.Services.Customers.UpdateRecentlyViewed('o' + id, function(resultHtml) {
            $('.recent-container').empty();
            $('.recent-container').html(resultHtml);
        });
    });

    parent.Ts.Services.Customers.GetRecentlyViewed(function(resultHtml) {
        $('.recent-container').empty();
        $('.recent-container').html(resultHtml);
    });

    var _tmrSearch = null;
    $('#searchString').keyup(function() {
        if (_tmrSearch != null) {
            clearTimeout(_tmrSearch);
        }
        $('.searchresults').fadeTo(200, 0.5);
        function getResults() {
            _isLoading = true;
            $('.results-loading').hide();
            $('.results-empty').hide();
            $('.results-done').hide();
            fetchItems();
        }
        _tmrSearch = setTimeout(getResults, 500);
    });

    var _isLoading = false;
    $('.frame-container').bind('scroll', function() {
        if (_isLoading == true) { return; }
        if ($('.results-done').is(':visible')) { return; }
        if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
            fetchItems($('.peopleinfo').length);
        }
        if ($(this).scrollTop() > 100) {
            $('.scrollup').fadeIn();
        } else {
            $('.scrollup').fadeOut();
        }
    });
    $('.scrollup').click(function() {
        $('.frame-container').animate({
            scrollTop: 0
        }, 600);
        return false;
    });
    fetchItems();
});

function refreshPage() {
    parent.Ts.Services.Customers.GetRecentlyViewed(function(resultHtml) {
        $('.recent-container').empty();
        $('.recent-container').html(resultHtml);
    });
}
