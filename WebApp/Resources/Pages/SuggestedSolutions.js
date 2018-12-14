function GetSelectedID() {
    if ($('.selected').length > 0) {
        return $('.selected').find('.ticketID').val();
    }
    else {
        return null;
    }
}

var _firstItemIndex = 0;
var _pageSize = 20;
var _resultsCount = 0;

var mainFrame = getMainFrame();
$(document).ready(function () {

    $('#suggested-solutions-pane').bind('scroll', function () {
        if ($(this).scrollTop() > 0 && $(this).scrollTop() + $(this).innerHeight() >= ($(this)[0].scrollHeight * 0.9)) {
            GetSuggestedSolutionsNextPage();
        }
    });

    GetSearchResults();

    layout = $('body').layout({
        applyDefaultStyles: true
      , west__size: 283
      , west__resizable: false
      , west__closable: false
      , spacing_open: 0
    });
});

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

function GetSearchResults() {
    if (_firstItemIndex == 0) {
        $('.resultsSummary').hide();
        $('#suggested-solutions').hide();
        $('#suggested-solutions-loading').show();
    }
    //var ticketLoadFilter = mainFrame.Ts.Utils.queryToTicketFilter(window);
    //ticketLoadFilter.SearchText2 = window.parent.$('#SuggestedSolutionsInput').val();
    //mainFrame.Ts.Services.Tickets.GetTicketRange(1, 50, ticketLoadFilter, function (results) { showSearchResults(results); });
    var ticketID = 0;debugger
    if (window.parent._ticketID) {
        ticketID = window.parent._ticketID;
    }
    mainFrame.Ts.Services.Tickets.GetSuggestedSolutions(ticketID, _firstItemIndex, _pageSize, function (results) { showSearchResults(results); });
    
    var timLoading = setTimeout(function () { document.getElementById("TicketPreviewIFrame").contentWindow.writeHtml('<div class="ui-widget ticket-preview-none"><h1 class="ui-widget-content">Find a suggested solution you want to look at?</h1><h2>Just click a ticket on the left, and you can preview it here.</h2></div>'); timLoading = null; }, 500);
}

function GetSuggestedSolutionsNextPage() {
    _firstItemIndex += _pageSize;
    if (_resultsCount > _firstItemIndex) {
        GetSearchResults();
    }
}

function showSearchResults(results) {
    if (results == null) return;
    if (_firstItemIndex == 0) {
        _resultsCount = results.Count;
        $('#suggested-solutions-summary').html(
        '<div class="resultsSummary">' +
            results.Count + ' solutions found.' +
          '</div>')
    }
    var html = '';

    if (results.Count > 0) {
        for (var i = 0; i < results.Items.length; i++) {
            var iconPath = "";
            //var onClickHandler = "parent.Ts.MainPage.openTicket(" + results.Items[i].Number + ", true)";
            var onClickHandler = "";
            if (results.Items[i].KBCategory == "") {
                results.Items[i].KBCategory = "Uncategorized";
            }
            var subText = '<h2>Tags: ' + results.Items[i].Tags + ' </h2>' +
                          '<h2>Category: ' + results.Items[i].KBCategory + '</h2>';

            var text = results.Items[i].DisplayName;

            html = html +
            '<div class="resultItem">' +
              '<div class="resultItem-left">' +
                //'<div class="resultItem-icon">' +
                //  '<img alt="Result item icon" src="' + iconPath + '" />' +
                //  '<h2>' + results.Items[i].ScorePercent + '%</h2>' +
                //'</div>' +
                '<div class="resultItem-text">' +
                  '<input type="hidden" class="ticketID" value=' + results.Items[i].ID + '>' +
                  '<input type="hidden" class="previewHtml" value="">' +
                  '<h1>' +
                    '<p>' + text + '<p>' +
                  '</h1>' +
                  subText +
                '</div>' +
              '</div>' +
              '<div class="ticket-right" />' +
              '<div class="ui-helper-clearfix" />' +
            '</div>';
        }
    }

    if (_firstItemIndex == 0) {666
        $('.resultsSummary').show();
        $('#suggested-solutions').html(html).show();
        $('#suggested-solutions-loading').hide();
    }
    else {
        $('#suggested-solutions').append(html);
    }

    $('.resultItem-text').on("click", function (e) {
        e.preventDefault();
        $('.resultItem-text').removeClass('selected');
        $(this).addClass('selected');
        if ($(this).find('.previewHtml').val() == '') {
            var clickedItem = $(this);
            mainFrame.Ts.Services.Tickets.GetKBTicketAndActions($(this).find('.ticketID').val(), function (result) {
                var html = '<div>';

                var actions = result[1];
                if (actions.length == 0) {
                    html = html + '<h2>The selected ticket has no knowledgebase actions.</h2>';
                }
                else {
                    for (var i = 0; i < actions.length; i++) {
                        html = html + '<div>' + actions[i].Description + '</div></br>';
                    }
                }
                html = html + '</div>';
                clickedItem.find('.previewHtml').attr("value", html);
                document.getElementById("TicketPreviewIFrame").contentWindow.writeHtml(html);
            });
        }
        else {
            document.getElementById("TicketPreviewIFrame").contentWindow.writeHtml($(this).find('.previewHtml').val());
        }
        mainFrame.Ts.System.logAction('Suggested Solutions - Preview solution');
    });
}
