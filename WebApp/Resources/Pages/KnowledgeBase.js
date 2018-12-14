var _knowledgeBaseCategories    = null;
var _firstItemIndex             = 0;
var _pageSize                   = 5;
var _subCategoryfirstItemIndex  = 0;
var _subCategoryPageSize        = 40;
var _subCategoryItemsCount      = 0;
var _homePageIsBuilt            = false;
var _categoryID                 = null;
var _categoryName               = null;
var _parentCategoryName         = null;

$(document).ready(function () {
  GetHomePage();
  BuildSideBar();

  var canKbEdit = parent.Ts.System.User.IsSystemAdmin || parent.Ts.System.User.ChangeKbVisibility;

  if (!canKbEdit) {
      //$('.newticket-kb').attr("disabled", true);
      $('#new-article').hide();
  }

  $('#search-button').click(function () {
    _firstItemIndex = 0; 
    GetSearchResults();
    parent.Ts.System.logAction('Knowledge Base - Searched');
  });

  $('.frame-content').bind('scroll', function () {
    if ($(this).scrollTop() > 0 && $(this).scrollTop() + $(this).innerHeight() >= ($(this)[0].scrollHeight * 0.9)) {
      if ($('#SubCategoryPage').is(":visible")) {
        parent.Ts.System.logAction('Knowledge Base - Get more articles');
        var categoryName = $('#SubCategoryPageHeader').text().substring(0, $('#SubCategoryPageHeader').text().indexOf('(') - 1);
        if (categoryName == 'Uncategorized') {
          categoryName = null;
        }
        GetMoreForSubCategoryPage(categoryName);
      }
    }

    if ($(this).scrollTop() > 100) {
      $('.scrollup').fadeIn();
    } else {
      $('.scrollup').fadeOut();
    }
  });

  $('.scrollup').click(function () {
    $('.frame-content').animate({ scrollTop: 0 }, 600);
    parent.Ts.System.logAction('Knowledge Base - Scrolled to Top');
    return false;
  });

  $('.loading-section').hide().next().show();
});

function onShow() {
  if ($('#HomePage').is(":visible")) {
    GetHomePage();
    BuildSideBar();
  }
  else {
    GetSubCategoryPage(_categoryID, _categoryName, _parentCategoryName);
  }
}

function GetHomePage() {
  $('#HomePageContent').empty();
  parent.Ts.System.logAction('Knowledge Base - Get home page');
  if (_homePageIsBuilt == true) {
    // I added this thinking in implementing click in category name that hide all the rows but the one clicked.
    // Then I decided to hold on that idea for now.
    $('.row-fluid').show();
  }
  else {
    parent.Ts.Services.Admin.GetKnowledgeBaseCategories(function (knowledgeBaseCategories) {
      _knowledgeBaseCategories = knowledgeBaseCategories;
      BuildSections();
      //_homePageIsBuilt = true;
    });
  }

  $('#SubCategoryPage').hide();
  $('#ParentCategoryBreadCrumbItem').remove();
  $('#HomePage').show();
  $('.breadcrumb').hide();
}

function BuildSideBar() {
  parent.Ts.Services.Tickets.GetNewKnowledgeBaseArticles(_firstItemIndex, _pageSize, function (newKnowledgeBaseArticles) {
    BuildNewArticles(newKnowledgeBaseArticles);
  });

  parent.Ts.Services.Tickets.GetRecentlyModifiedKnowledgeBaseArticles(_firstItemIndex, _pageSize, function (recentlyModifiedKnowledgeBaseArticles) {
    BuildRecentlyModifiedArticles(recentlyModifiedKnowledgeBaseArticles);
  });
}

function BuildSections() {
  for (var i = 0; i < _knowledgeBaseCategories.length; i++) {
    BuildSection(_knowledgeBaseCategories[i]);
  }
  //  Add an uncategorized section at the end.
  BuildSection(null);
}

function BuildSection(category) {
  var categorySection = $('<div>').addClass('ts-section ui-widget-content ui-corner-all slim');

  var subCategoriesRow = $('<div>').addClass('row-fluid');
  if (category != null) {
    var parentCategoryName = category.Category.CategoryName;
    categorySection.append($('<h1 class="ts-section-header">' + category.Category.CategoryName + '</h1></div>'));

    var amountOfSubcategories = category.Subcategories.length;
    // We add one more index to add the "Without subcategory" section.
    for (var i = 0; i < amountOfSubcategories + 1; i++) {
      var isEven = false;
      if (i % 2 == 0) {
        isEven = true;
      }

      if (isEven && i > 0) {
        subCategoriesRow = $('<div>').addClass('row-fluid');
      }

      var categoryID;
      var categoryName;

      if (i < amountOfSubcategories) {
        categoryID    = category.Subcategories[i].CategoryID;
        categoryName  = category.Subcategories[i].CategoryName;
      }
      else {
        categoryID    = category.Category.CategoryID;
        categoryName  = 'Without subcategory';
      }

      var subCategorySection = $('<div>').addClass('span6');
      var ul = $('<ul>').addClass('nav nav-list').attr('id', 'catID-' + categoryID);
      subCategorySection.append(ul);

      parent.Ts.Services.Tickets.GetKnowledgeBaseCategoryTickets(
        categoryID,
        categoryName,
        parentCategoryName,
        _firstItemIndex,
        _pageSize,
        function (tickets) {
          addTicketsToSection(tickets);
        }
      );
      subCategoriesRow.append(subCategorySection);
      if (!isEven || i == (amountOfSubcategories)) {
        categorySection.append(subCategoriesRow);
      }

    } // End of iteration through the subcategories
  }
  else {
    var subCategorySection = $('<div>').addClass('span12');
    var ul = $('<ul>').addClass('nav nav-list').attr('id', 'catID-0');
    ul.append($('<li id="UncategorizedHeader"></li>'));
    subCategorySection.append(ul);

    parent.Ts.Services.Tickets.GetKnowledgeBaseCategoryTickets(
      null,
      '',
      '',
      _firstItemIndex,
      _pageSize,
      function (tickets) {
        addTicketsToSection(tickets);
      }
    );
    subCategoriesRow.append(subCategorySection);
    categorySection.append(subCategoriesRow);
  }

  $('#HomePageContent').append(categorySection);

  $('#new-article').click(function (e) {
    e.preventDefault();
    parent.Ts.MainPage.newTicket();
  });

} // End of Build Section

function addTicketsToSection(tickets, inSubcategoryPage) {
  if (tickets == null) return;

  var onShowAllClickHandler = '';
  var categoryID = 0;  
  if (tickets.CategoryID != null) {
    categoryID = tickets.CategoryID;
    if (inSubcategoryPage == null) {
        if (tickets.Count > 0) {
            var catname = tickets.CategoryName.replace("'","\\'");
            var parentname = tickets.ParentCategoryName.replace("'", "\\'");
          onShowAllClickHandler = "GetSubCategoryPage(" + tickets.CategoryID + ", \'" + catname + "\', \'" + parentname + "\'); return false;";
        $('#catID-' + categoryID).append($('<li class="nav-header"><a href="#" class="ts-link" onclick="' + onShowAllClickHandler + '">' + tickets.CategoryName + ' (' + tickets.Count + ')</a></li>'));
      }
      else {
        $('#catID-' + categoryID).append($('<li class="nav-header inactiveSubCategory">' + tickets.CategoryName + ' (' + tickets.Count + ')</li>'));
      }
    }
    else {
      $('#SubCategoryPageHeader').text(tickets.CategoryName + ' (' + tickets.Count + ')');
    }
  }
  else {
    if (inSubcategoryPage == null) {
      if (tickets.Count > 0) {
          onShowAllClickHandler = 'GetSubCategoryPage(null, null, null); return false;';
          $('#UncategorizedHeader').html('<h1 class="ts-section-header"><a href="#" onclick="' + onShowAllClickHandler + '">Uncategorized (' + tickets.Count + ')</a></h1>');
      }
      else {
          $('#UncategorizedHeader').html('<h1 class="ts-section-header">Uncategorized (' + tickets.Count + ')</h1>');
      }
    }
    else {
      if (tickets.CategoryName != null) {
        $('#SubCategoryPageHeader').text(tickets.CategoryName + ' (' + tickets.Count + ')');
      }
      else {
        $('#SubCategoryPageHeader').text('Uncategorized (' + tickets.Count + ')');
      }
    }
  }

  if (tickets.CategoryName == 'Without subcategory' && tickets.Count == 0) {
    $('#catID-' + categoryID).remove();
    return;
  }

  var html = '';
  var iconPath = "/vcr/1_5_6/images/nav/16/file.png";

  for (var i = 0; i < tickets.Items.length; i++) {
      var onClickHandler = "parent.Ts.MainPage.openTicketByID(" + tickets.Items[i].ID + ", true); return false;";
    var text            = tickets.Items[i].Name;

    html =
      '<li class="kb-article-container">' +
        '<a href="#" onclick="' + onClickHandler + '" class="ts-link">' + text + '</a>' +
      '</li>';

    if (inSubcategoryPage == null) {
      $('#catID-' + categoryID).append(html);
    }
    else {
      _subCategoryItemsCount = tickets.Count;
      $('#subcatID-' + categoryID).append(html);
    }
  }

  if ((inSubcategoryPage == null) && (tickets.Count > _pageSize)) {
    html =
      '<li class="kb-see-more">' +
        '<a href="#" onclick="' + onShowAllClickHandler + '" class="ts-link allLink">>> See all...</a>' +
      '</li>';

    $('#catID-' + categoryID).append(html);
  }
}

function BuildNewArticles(tickets) {
  if (tickets == null) return;
  $('#NewArticles').empty();

  var html = 
    '<li>' +
      '<h1 class="ts-section-header">' +
        '<a href="#" onclick="GetSubCategoryPage(null, \'New Articles\', null); return false;">' +
          'New Articles' +
        '</a>' +
      '</h1>' +
    '</li>';
  $('#NewArticles').append(html);

  var iconPath = "/vcr/1_5_6/images/nav/16/file.png";

  for (var i = 0; i < tickets.Items.length; i++) {
      var onClickHandler = "parent.Ts.MainPage.openTicketByID(" + tickets.Items[i].ID + ", true); return false;";
    var text = tickets.Items[i].Name;

    html =
      '<li class="kb-article-container">' +
        '<div class="item-icon">' +
            '<i class="fa fa-file-text"></i>' +
        '</div>' +
        '<a href="#" onclick="' + onClickHandler + '" class="ts-link">' + text + '</a>' +
      '</li>';

    $('#NewArticles').append(html);
  }

  if (tickets.Count > 5) {
    html =
        '<li>' +
          '<a href="#" onclick="GetSubCategoryPage(null, \'New Articles\', null); return false;" class="ts-link allLink">>> See all...</a>' +
        '</li>';

    $('#NewArticles').append(html);
  }
}

function BuildRecentlyModifiedArticles(tickets) {
  if (tickets == null) return;
  $('#RecentlyModifiedArticles').empty();

  var html =
    '<li>' +
      '<h1 class="ts-section-header">' +
        '<a href="#" onclick="GetSubCategoryPage(null, \'Recently Modified Articles\', null); return false;">' +
          'Recently Modified' +
        '</a>' +
      '</h1>' +
    '</li>';

  $('#RecentlyModifiedArticles').append(html);

  var iconPath = "/vcr/1_5_6/images/nav/16/file.png";

  for (var i = 0; i < tickets.Items.length; i++) {
      var onClickHandler = "parent.Ts.MainPage.openTicketByID(" + tickets.Items[i].ID + ", true); return false;";
    var text = tickets.Items[i].Name;

    html =
      '<li>' +
        '<div class="item-icon">' +
        '<i class="fa fa-file-text"></i>' +
        '</div>' +
        '<a href="#" onclick="' + onClickHandler + '" class="ts-link">' + text + '</a>' +
      '</li>';

    $('#RecentlyModifiedArticles').append(html);
  }

  if (tickets.Count > 5) {
    html =
      '<li>' +
        '<a href="#" onclick="GetSubCategoryPage(null, \'Recently Modified Articles\', null); return false;" class="ts-link allLink">>> See all...</a>' +
      '</li>';

    $('#RecentlyModifiedArticles').append(html);
  }
}

function GetSubCategoryPage(categoryID, categoryName, parentCategoryName) {
  parent.Ts.System.logAction('Knowledge Base - Get SubCategory page');

  _categoryID           = categoryID;
  _categoryName         = categoryName;
  _parentCategoryName   = parentCategoryName;

  _subCategoryfirstItemIndex = 0;
  $('#HomePage').hide();
  $('#SubCategoryPage').show();
  $('#ParentCategoryBreadCrumbItem').remove();
  if (parentCategoryName != null) {
    $('.breadcrumb').append($('<li id="ParentCategoryBreadCrumbItem" class="active"><span class="divider">/ </span>' + parentCategoryName + '</li>'));
    $('.active').show();
  }
  $('.breadcrumb').show();
  $('.frame-content').animate({ scrollTop: 0 }, 600);

  $('#SubCategoryPageContent').empty();
  $('#SubCategoryPageContent').append($('<h1 id="SubCategoryPageHeader ts-section-header"></h1>'));

  if (categoryID != null) {
    var ul = $('<ul>').addClass('nav nav-list').attr('id', 'subcatID-' + categoryID);
    $('#SubCategoryPageContent').append(ul);

    parent.Ts.Services.Tickets.GetKnowledgeBaseCategoryTickets(
      categoryID,
      categoryName,
      parentCategoryName,
      _firstItemIndex,
      _subCategoryPageSize,
      function (tickets) {
        addTicketsToSection(tickets, true);
      }
    );
  }
  else {
    var ul = $('<ul>').addClass('nav nav-list').attr('id', 'subcatID-0');
    $('#SubCategoryPageContent').append(ul);

    if (categoryName == 'Search results') {
      $('.loading-section').show().next().hide();
      parent.Ts.Services.Tickets.GetKnowledgeBaseSearchResults(
        $('#search-input').val(),
        _firstItemIndex,
        _subCategoryPageSize,
        function (results) {
          addTicketsToSection(results, true);
          $('.loading-section').hide().next().show();
        }
      );
    }
    else {
      parent.Ts.Services.Tickets.GetKnowledgeBaseCategoryTickets(
        null,
        categoryName,
        '',
        _firstItemIndex,
        _subCategoryPageSize,
        function (tickets) {
          addTicketsToSection(tickets, true);
        }
      );
    }
  }
}

function GetMoreForSubCategoryPage(categoryName) {
  var categoryID = $('ul[id^="subcatID-"]').attr('id').substring(9);
  if (categoryID == 0) {
    categoryID = null;
  }
  _subCategoryfirstItemIndex += _subCategoryPageSize;
  if (_subCategoryItemsCount > _subCategoryfirstItemIndex) {
    parent.Ts.Services.Tickets.GetKnowledgeBaseCategoryTickets(
      categoryID,
      categoryName,
      '',
      _subCategoryfirstItemIndex,
      _subCategoryPageSize,
      function (tickets) {
        appendTicketsToSection(tickets, true);
      }
    );
  }
}

function appendTicketsToSection(tickets, inSubcategoryPage) {
  if (tickets == null) return;

  var categoryID = 0;
  if (tickets.CategoryID != null) {
    categoryID = tickets.CategoryID;
  }

  var html = '';
  var iconPath = "/vcr/1_5_6/images/nav/16/file.png";

  for (var i = 0; i < tickets.Items.length; i++) {
      var onClickHandler = "parent.Ts.MainPage.openTicketByID(" + tickets.Items[i].ID + ", true); return false;";
    var text = tickets.Items[i].Name;

    html =
      '<li class="kb-article-container">' +
        '<div class="item-icon">' +
            '<i class="fa fa-file-text"></i>' +
        '</div>' +
        '<a href="#" onclick="' + onClickHandler + '" class="ts-link">' + text + '</a>' +
      '</li>';

    $('#subcatID-' + categoryID).append(html);
  }
}

$(document).keypress(function (e) {
  if (e.which == 13) {
    setTimeout(function () { $('#search-button').trigger('click'); }, 5);
    $(document).blur();
  }
});

function GetSearchResults() {
//  if (_firstItemIndex == 0) {
//    $('.resultsSummary').hide();
//    $('#search-results').hide();
//    $('#search-results-loading').show();
  //  }
  GetSubCategoryPage(null, 'Search results', null);
}

function showSearchResults(results) {
  if (results == null) return;
//  $('#search-results-summary').html(
//  '<div class="resultsSummary">' +
//      results.Count + ' items found.' +
//    '</div>' +
//    '<div class="ui-helper-clearfix" />')
//  var html = '';
//  for (var i = 0; i < results.Items.length; i++) {
//    var iconPath = "";
//    var onClickHandler = "";
//    var subText = "";
//    switch (results.Items[i].TypeID) {
//      case 1: //Tickets
//        iconPath = "/vcr/1_9_0/images/nav/16/tickets.png";
//        onClickHandler = "parent.Ts.MainPage.openTicket(" + results.Items[i].Number + ", true)";
//        subText = '<h2>Status: ' + results.Items[i].Status + ' </h2>' +
//                '<h2>Severity: ' + results.Items[i].Severity + '</h2>';
//        break;
//      case 2: //KnowledgeBase
//        iconPath = "/vcr/1_9_0/images/nav/16/knowledge.png";
//        onClickHandler = "parent.Ts.MainPage.openTicket(" + results.Items[i].Number + ", true)";
//        subText = '<h2>Status: ' + results.Items[i].Status + ' </h2>' +
//                '<h2>Severity: ' + results.Items[i].Severity + '</h2>';
//        break;
//      case 3: //Wikis
//        iconPath = "/vcr/1_9_0/images/nav/16/wiki.png";
//        onClickHandler = "parent.Ts.MainPage.openWiki(" + results.Items[i].ID + ", true)";
//        subText = '<h2>Created by: ' + results.Items[i].Creator + ' </h2>' +
//                '<h2>Modified by: ' + results.Items[i].Modifier + '</h2>';
//        break;
//      case 4: //Notes
//        iconPath = "/vcr/1_9_0/images/nav/16/customers.png";
//        onClickHandler = "parent.Ts.MainPage.openCustomerNote(" + results.Items[i].CustomerID + ", " + results.Items[i].ID + ", true)";
//        subText = '<h2>Created by: ' + results.Items[i].Creator + ' </h2>' +
//                '<h2>Modified on: ' + results.Items[i].DateModified + '</h2>';
//        break;
//      case 5: //ProductVersions
//        iconPath = "/vcr/1_9_0/images/nav/16/products.png";
//        onClickHandler = "parent.Ts.MainPage.openProductVersion(" + results.Items[i].ProductID + ", " + results.Items[i].ID + ", true)";
//        subText = '<h2>Status: ' + results.Items[i].Status + ' </h2>' +
//                '<h2>Modified on: ' + results.Items[i].DateModified + '</h2>';
//        break;
//    }

//    var text = results.Items[i].DisplayName;

//    html = html +
//    '<div class="resultItem">' +
//      '<div class="resultItem-left">' +
//        '<div class="resultItem-icon">' +
//          '<img alt="Result item icon" src="' + iconPath + '" />' +
//          '<h2>' + results.Items[i].ScorePercent + '%</h2>' +
//        '</div>' +
//        '<div class="resultItem-text">' +
//          '<h1>' +
//            '<a href="#" onclick="' + onClickHandler + '" class="ts-link">' + text + '</a>' +
//          '</h1>' +
//          subText +
//        '</div>' +
//      '</div>' +
//      '<div class="ticket-right" />' +
//      '<div class="ui-helper-clearfix" />' +
//    '</div>';
//  }

//  if (_firstItemIndex == 0) {
//    $('.resultsSummary').show();
//    $('#search-results').html(html).show();
//    $('#search-results-loading').hide();
//  }
//  else {
//    $('#search-results').append(html);
//  }
}