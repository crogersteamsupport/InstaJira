$(document).ready(function () {

    $('input, textarea').placeholder();

    function fetchItems(start) {
        start = start || 0;
        showLoadingIndicator();
        $('.searchresults').fadeTo(200, 0.5);
        var term = $('#searchString').val();

        var searchProducts = false;
        var searchProductVersions = false;

        if ($('.products-filter-all').parent().hasClass('active')) {
            searchProducts = true;
            searchProductVersions = true;
        } else if ($('.products-filter-products').parent().hasClass('active')) {
            searchProducts = true;
            searchProductVersions = false;
        } else if ($('.products-filter-product-versions').parent().hasClass('active')) {
            searchProducts = false;
            searchProductVersions = true;
        } else if ($('.products-filter-product-families').parent().hasClass('active')) {
            $('.product-family-new').show();
            $('.product-new').hide();
            fetchProductFamilies(start);
            return;
        }

        parent.Ts.Services.Search.SearchProducts($('#searchString').val(), start, 20, searchProducts, searchProductVersions, function (items) {
            $('.searchresults').fadeTo(0, 1);

            if (start == 0) {
                insertSearchResults(items, searchProducts, searchProductVersions);
                /*$('.frame-container').animate({
                scrollTop: 1
                }, 600);*/
            } else {
                appendSearchResults(items, searchProducts, searchProductVersions);
            }

        });
    }

    function fetchProductFamilies(start) {
        parent.Ts.Services.Search.SearchProductFamilies($('#searchString').val(), start, function (items) {
            $('.searchresults').fadeTo(0, 1);
            $('.results-loading').hide();
            $('.results-done').hide();
            $('.results-empty').hide();

            if (start == 0) {
                $('.searchresults').empty();
                if (items.length < 1) {
                    $('.results-empty').show();
                }
            } else {
                if (items.length < 1) {
                    $('.results-done').show();
                }
            }

            var container = $('.searchresults');
            for (var i = 0; i < items.length; i++) {
                var el = $('<tr>');
                var circle = $('<i>').addClass('fa fa-circle fa-stack-2x');
                var icon = $('<i>').addClass('fa fa-stack-1x fa-inverse');

                $('<td>').addClass('result-icon').append(
                  $('<span>').addClass('fa-stack fa-2x').append(circle).append(icon)
                ).appendTo(el);

                var div = $('<div>')
                  .addClass('productinfo');

                $('<td>').append(div).appendTo(el);

                circle.addClass('color-magenta');
                icon.addClass('fa-umbrella');
                appendProductFamily(div, items[i]);

                el.appendTo(container);
            }

            _isLoading = false;
        });
    }

    function showLoadingIndicator() {
        _isLoading = true;
        $('.results-loading').show();
    }

    function insertSearchResults(items, searchProducts, searchProductVersions) {
        $('.searchresults').empty();

        if (items == null || items.length < 1) {
            $('.results-loading').hide();
            $('.results-done').hide();
            $('.results-empty').show();
        } else {
            appendSearchResults(items, searchProducts, searchProductVersions);
        }
        _isLoading = false;
    }

    function appendSearchResults(items, searchProducts, searchProductVersions) {
        $('.results-loading').hide();
        $('.results-empty').hide();
        $('.results-done').hide();

        if (items.length < 1) {
            $('.results-done').show();
        } else {
            var container = $('.searchresults');
            for (var i = 0; i < items.length; i++) {
                var parsedItem = JSON.parse(items[i]);
                //        here is where we need to find out how to identify a product version from a product by looking at the customer page
                if ((searchProducts && !parsedItem.productVersionID) || (searchProductVersions && parsedItem.productVersionID)) {
                    appendItem(container, parsedItem);
                }
            }
        }
        _isLoading = false;
    }

    function appendItem(container, item) {
        var el = $('<tr>');
        var circle = $('<i>').addClass('fa fa-circle fa-stack-2x');
        var icon = $('<i>').addClass('fa fa-stack-1x fa-inverse');

        $('<td>').addClass('result-icon').append(
          $('<span>').addClass('fa-stack fa-2x').append(circle).append(icon)
        ).appendTo(el);

        var div = $('<div>')
          .addClass('productinfo');

        //if (!item.productVersionID) {
        //    parent.Ts.Services.Products.GetProductOpenTicketCount(item.productID, function (count) {
        //        div.append(
        //            $('<div>')
        //            .addClass('pull-right')
        //            .append($('<p>').text(count + ' open tickets'))
        //        );
        //    });
        //}
        //else {
        //    parent.Ts.Services.Products.GetProductVersionOpenTicketCount(item.productVersionID, function (count) {
        //        div.append(
        //            $('<div>')
        //            .addClass('pull-right')
        //            .append($('<p>').text(count + ' open tickets'))
        //        );
        //    });
        //}

        $('<td>').append(div).appendTo(el);

        if (item.productVersionID) {
            circle.addClass('color-orange');
            icon.addClass('fa-clock-o');
            appendProductVersion(div, item);
        }
        else {
            circle.addClass('color-green');
            icon.addClass('fa-barcode');
            appendProduct(div, item);
        }

        el.appendTo(container);
    }

    function appendProductVersion(el, item) {
        var displayName = item.versionNumber;

        $('<a>')
          .attr('href', '#')
          .addClass('productversionlink')
          .data('productversionid', item.productVersionID)
          .text(displayName)
          .appendTo($('<h4>').appendTo(el));

        var list = $('<ul>').appendTo(el);

        var firstRow = $('<li>').appendTo(list);

        $('<a>')
          .attr('href', '#')
          .addClass('productlink')
          .data('productid', item.productID)
          .text(item.productName)
          .appendTo(firstRow);

        var secondRow = $('<li>').appendTo(list);

        $('<a>')
          .attr('href', '#')
          .addClass('productversionlink')
          .data('productversionid', item.productVersionID)
          .text(item.versionStatus)
          .appendTo(secondRow);

        var thirdRow = $('<li>').appendTo(list);

        var releasedLabel = 'Not released';
        if (item.isReleased) {
            releasedLabel = 'Released';
        }

        $('<a>')
          .attr('href', '#')
          .addClass('productversionlink')
          .data('productversionid', item.productVersionID)
          .text(releasedLabel)
          .appendTo(thirdRow);

        if (item.isReleased) {
            var fourthRow = $('<li>').appendTo(list);

            $('<a>')
          .attr('href', '#')
          .addClass('productversionlink')
          .data('productversionid', item.productVersionID)
          .text(parent.Ts.Utils.getMsDate(item.releaseDate).localeFormat(parent.Ts.Utils.getDatePattern()))
          .appendTo(fourthRow);
        }
    }

    function appendProduct(el, item) {
        var displayName = item.name;

        $('<a>')
          .attr('href', '#')
          .addClass('productlink')
          .data('productid', item.productID)
          .text(displayName)
          .appendTo($('<h4>').appendTo(el));

        var list = $('<ul>').appendTo(el);

        var firstRow = $('<li>').appendTo(list);

        $('<a>')
          .attr('target', '_blank')
          .text(item.description)
          .appendTo(firstRow);
    }

    function appendProductFamily(el, item) {
        var displayName = item.Name;

        $('<a>')
          .attr('href', '#')
          .addClass('productfamilylink')
          .data('productfamilyid', item.ProductFamilyID)
          .text(displayName)
          .appendTo($('<h4>').appendTo(el));

        var list = $('<ul>').appendTo(el);

        var firstRow = $('<li>').appendTo(list);

        $('<a>')
          .attr('target', '_blank')
          .text(item.Description)
          .appendTo(firstRow);
    }

    function isNullOrWhiteSpace(str) {
        return str === null || str.match(/^ *$/) !== null;
    }

    $('.products-filter').on('click', 'a', function (e) {
        e.preventDefault();
        $('.products-filter li.active').removeClass('active');
        $(this).parent().addClass('active');
        parent.Ts.System.logAction('Products Page - Change Filter');
        $('.product-family-new').hide();
        $('.product-new').show();
        fetchItems();
    });

    var _isAdmin = parent.Ts.System.User.IsSystemAdmin;
    if (!_isAdmin && !parent.Ts.System.User.CanCreateProducts) {
        $('.product-new').remove();
        $('.product-family-new').remove();
    }

    if (parent.Ts.System.Organization.UseProductFamilies) {
        $('#productFamiliesFilter').show();
    }

    $('.product-new').click(function (e) {
        e.preventDefault();
        parent.Ts.MainPage.newProduct();

    });

    $('.product-family-new').click(function (e) {
        e.preventDefault();
        parent.Ts.MainPage.newProductFamily();

    });

    $('.searchresults, .recent-container').on('click', '.productversionlink', function (e) {
        e.preventDefault();

        var id = $(this).data('productversionid');
        parent.Ts.System.logAction('Products Page - View Recent Product Version');
        parent.Ts.MainPage.openNewProductVersion(id);

        parent.Ts.Services.Products.UpdateRecentlyViewed('v' + id, function (resultHtml) {
            $('.recent-container').empty();
            $('.recent-container').html(resultHtml);
        });

    });

    $('.searchresults, .recent-container').on('click', '.productlink', function (e) {
        e.preventDefault();

        var id = $(this).data('productid');
        parent.Ts.System.logAction('Products Page - View Recent Product');
        parent.Ts.MainPage.openNewProduct(id);

        parent.Ts.Services.Products.UpdateRecentlyViewed('p' + id, function (resultHtml) {
            $('.recent-container').empty();
            $('.recent-container').html(resultHtml);
        });

    });

    $('.searchresults, .recent-container').on('click', '.productfamilylink', function (e) {
        e.preventDefault();

        var id = $(this).data('productfamilyid');
        parent.Ts.System.logAction('Products Page - View Recent Product Line');
        parent.Ts.MainPage.openNewProductFamily(id);

        parent.Ts.Services.Products.UpdateRecentlyViewed('f' + id, function (resultHtml) {
            $('.recent-container').empty();
            $('.recent-container').html(resultHtml);
        });

    });

    parent.Ts.Services.Products.GetRecentlyViewed(function (resultHtml) {
        $('.recent-container').empty();
        $('.recent-container').html(resultHtml);
    });

    var _tmrSearch = null;
    $('#searchString').keyup(function () {
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
    $('.frame-container').bind('scroll', function () {
        if (_isLoading == true) return;
        if ($('.results-done').is(':visible')) return;

        if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
            fetchItems($('.productinfo').length);
        }

        if ($(this).scrollTop() > 100) {
            $('.scrollup').fadeIn();
        } else {
            $('.scrollup').fadeOut();
        }
    });

    $('.scrollup').click(function () {
        $('.frame-container').animate({
            scrollTop: 0
        }, 600);
        return false;
    });

    fetchItems();
});