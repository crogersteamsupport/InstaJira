$(document).ready(function () {

  var _isAdmin = parent.Ts.System.User.IsSystemAdmin;

  $('input, textarea').placeholder();

  function fetchItems(start) {
    start = start || 0;
    showLoadingIndicator();
    $('.searchresults').fadeTo(200, 0.5);
    var term = $('#searchString').val();

    var searchAssigned = false;
    var searchWarehouse = false;
    var searchJunkyard = false;

    if ($('.inventory-filter-all').parent().hasClass('active')) {
      searchAssigned = true;
      searchWarehouse = true;
      searchJunkyard = true;
    } else if ($('.inventory-filter-assigned').parent().hasClass('active')) {
      searchAssigned = true;
      searchWarehouse = false;
      searchJunkyard = false;
    } else if ($('.inventory-filter-warehouse').parent().hasClass('active')) {
      searchAssigned = false;
      searchWarehouse = true;
      searchJunkyard = false;
    } else if ($('.inventory-filter-junkyard').parent().hasClass('active')) {
      searchAssigned = false;
      searchWarehouse = false;
      searchJunkyard = true;
    }

    if (!parent.Ts.System.User.CanCreateAsset && !_isAdmin) {
        $('.action-new').remove();
    }

    parent.Ts.Services.Search.SearchAssets($('#searchString').val(), start, 20, searchAssigned, searchWarehouse, searchJunkyard, function (items) {
      $('.searchresults').fadeTo(0, 1);

      if (start == 0) {
        insertSearchResults(items, searchAssigned, searchWarehouse, searchJunkyard);
        /*$('.frame-container').animate({
        scrollTop: 1
        }, 600);*/
      } else {
        appendSearchResults(items, searchAssigned, searchWarehouse, searchJunkyard);
      }

    });
  }

  function showLoadingIndicator() {
    _isLoading = true;
    $('.results-loading').show();
  }

  function insertSearchResults(items, searchAssigned, searchWarehouse, searchJunkyard) {
    $('.searchresults').empty();

    if (items.length < 1) {
      $('.results-loading').hide();
      $('.results-done').hide();
      $('.results-empty').show();
    } else {
      appendSearchResults(items, searchAssigned, searchWarehouse, searchJunkyard);
    }
    _isLoading = false;
  }

  function appendSearchResults(items, searchAssigned, searchWarehouse, searchJunkyard) {
    $('.results-loading').hide();
    $('.results-empty').hide();
    $('.results-done').hide();

    if (items.length < 1) {
      $('.results-done').show();
    } else {
      var container = $('.searchresults');
      for (var i = 0; i < items.length; i++) {
        var parsedItem = JSON.parse(items[i]);
        if ((searchAssigned && parsedItem.location == "1") || (searchWarehouse && parsedItem.location == "2") || (searchJunkyard && parsedItem.location == "3")) {
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
          .addClass('assetinfo');
    //          .append(
    //            $('<div>')
    //              .addClass('pull-right')
    //              .append($('<p>').text(item.openTicketCount + ' open tickets'))
    //          );

    $('<td>').append(div).appendTo(el);

    switch (item.location) {
      case "1":
        circle.addClass('color-green');
        icon.addClass('fa-truck');
        break;
      case "2":
          circle.addClass('color-amber');
        icon.addClass('fa-home');
        break;
      case "3":
        circle.addClass('color-red');
        icon.addClass('fa-trash-o');
        break;
    }
    appendAsset(div, item);

    el.appendTo(container);
  }

  function appendAsset(el, item) {
    var displayName = item.name;
    var displayNameIsSerialNumber = false;
    if (isNullOrWhiteSpace(displayName)) {
      displayName = item.serialNumber;
      if (isNullOrWhiteSpace(displayName)) {
        displayName = item.assetID;
      }
      else {
        displayNameIsSerialNumber = true;
      }
    }

    $('<a>')
          .attr('href', '#')
          .addClass('assetlink')
          .data('assetid', item.assetID)
          .text(displayName)
          .appendTo($('<h4>').appendTo(el));

    var list = $('<ul>').appendTo(el);

    var firstRow = $('<li>').appendTo(list);

    $('<a>')
          .attr('target', '_blank')
          .text(item.productName)
          .appendTo(firstRow);

    if (!isNullOrWhiteSpace(item.productVersionNumber)) {
      firstRow.append(' - ');
      $('<a>')
            .attr('target', '_blank')
            .text(item.productVersionNumber)
            .appendTo(firstRow);
    }

    var secondRow = $('<li>').appendTo(list);
    if (!displayNameIsSerialNumber) {
      secondRow.append('SN: ');
      if (isNullOrWhiteSpace(item.serialNumber)) {
        secondRow.append('Empty');
      }
      else {
        secondRow.append(item.serialNumber);
      }
      secondRow.append(' - ');
    }

    secondRow.append('Warr. Exp.: ');
    if (isNullOrWhiteSpace(item.warrantyExpiration)) {
      secondRow.append('Empty');
    }
    else {
      secondRow.append(parent.Ts.Utils.getMsDate(item.warrantyExpiration).localeFormat(parent.Ts.Utils.getDatePattern()));
    }

  }


  function isNullOrWhiteSpace(str) {
    return str === null || str.match(/^ *$/) !== null;
  }

  $('.inventory-filter').on('click', 'a', function (e) {
    e.preventDefault();
    $('.inventory-filter li.active').removeClass('active');
    $(this).parent().addClass('active');
    parent.Ts.System.logAction('Inventory Page - Change Filter');
    fetchItems();
  });

  $('.action-new').click(function (e) {
    e.preventDefault();
    parent.Ts.MainPage.newAsset();

  });

  $('.searchresults, .recent-container').on('click', '.assetlink', function (e) {
    e.preventDefault();

    var id = $(this).data('assetid');
    parent.Ts.System.logAction('Inventory Page - View Recent Asset');
    parent.Ts.MainPage.openNewAsset(id);

    parent.Ts.Services.Assets.UpdateRecentlyViewed('o' + id, function (resultHtml) {
      $('.recent-container').empty();
      $('.recent-container').html(resultHtml);
    });

  });

  parent.Ts.Services.Assets.GetRecentlyViewed(function (resultHtml) {
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
      fetchItems($('.assetinfo').length);
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