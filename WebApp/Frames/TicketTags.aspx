<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TicketTags.aspx.cs" Inherits="Frames_TicketTags" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
  <title></title>
  <link href="../Css/ui.css" rel="stylesheet" type="text/css" />
  <link href="../Css/jquery-ui-latest.custom.css" rel="stylesheet" type="text/css" />
  <link href="../Css/frame.css" rel="stylesheet" type="text/css" />
  <script src="../vcr/1_9_0/Js/Ts/ts.pendo.js" type="text/javascript"></script>
  <script src="/frontend/library/jquery-1.4.2.min.js" type="text/javascript"></script>
  <script src="/frontend/library/jquery-ui-1.8.14.custom.min.js" type="text/javascript"></script>
  <script src="../Js/ts.system.old.js" type="text/javascript"></script>
  <script src="../Js/ts.data.tickets.js" type="text/javascript"></script>
  <script src="/frontend/library/json2.js" type="text/javascript"></script>
  <script src="/frontend/library/jquery.layout.1.3.0.min.js" type="text/javascript"></script>

  <style type="text/css">


    #pane-filters h1 {color:#15428B;  background: #EEEEEE; margin: 0 0 5px 0; padding: 10px 20px; font-size: 1.2em; border-bottom:1px solid #8BAAD9; line-height:20px; clear:left;}
    #pane-filters h1 span.tag-edit { float:right; width: 200px; text-align:right;}
    #pane-filters h1 a, #pane-filters h1 a:link, #pane-filters h1 a:active, #pane-filters h1 a:visited { color:#15428B; text-decoration:underline;  font-weight:normal; line-height:20px;}
    #pane-filters h1 a:hover { background:#15428B; color:#fff;}
    #tickets .ticket { border-bottom: 1px solid #dedede; clear:both; margin: 10px 10px; padding-bottom: 10px;}
    #tickets .ticket-left { float: left; width: 70%; }
    #tickets .ticket-right { float: right; width: 30%; }
    #tickets .ticket h1 { font-size: 1.2em; font-weight: normal; background: none; margin: 0; padding: 0; }
    #tickets .ticket h2 { font-size: 1em; font-weight: normal; margin: 0; padding: 0; color: #777; }
    #tickets .ticket-left .ticket-icon { float:left; width: 32px;}
    #tickets .ticket-left .ticket-icon img { padding: 10px 0 0 0px;}
    #tickets .ticket-left .ticket-text { float:left; width: 90%;}
    #tickets-loading { margin:0 auto; text-align:center; padding-top: 40px;}
    #tickets { overflow-y:auto; overflow-x:hidden; height:100%;}
    #tickets .ui-helper-clearfix { height: 1px; clear:both;}

    #container-main { height: 100%; min-width: 400px; }
    #pane-header { background: #EEEEEE; }
    #pane-filters { background: #EEEEEE;  }
    #filters {overflow-y:auto; overflow-x:hidden; height:100%; width:100%; color:black !important; }
    #tags { width: 90%; margin:0 auto;}
    #tags dl { line-height: 20px;}
    #tags dt { float:left; margin-right: 12px; min-height:100%; text-align:center; width: 15px; font-weight:bold; font-size: 1.1em;}
    #tags dd { border-bottom: 1px solid #C3D1E6; display:block; margin: 0 0 3px 0; padding: 0 0 3px 0;}
    #tags dd span { margin-left: 27px; display:block;}
    #tags dd span a, #tags dd span a:link, #tags dd span a:active, #tags dd span a:visited { white-space:nowrap; color:#15428B; text-decoration:none; padding: 3px 3px;}
    #tags dd span a:hover { text-decoration:underline; }

    #header { margin: 0 10px;}
    /*#header input.text { border:none; font-size: 1.1em; width: 250px; margin-right: 5px;}
    #header label { height: 30px;}
    #header-filter div { margin: 10px 0;}
    #header-filter strong { font-size: 1.2em;}
    #header-pager { margin-bottom: 5px;}
    #header-search { float:left;}
    #header-pager span { }
    #pager { float:right; line-height:30px; }*/
    #header-filter { height: 40px; line-height:40px; }
    #header-tags { font-size: 1.5em; font-weight:bold; color:black; }
    #header-tags strong { font-size: 1em; padding-right: 5px; }
    #header-filter a, #header-filter a:link, #header-filter a:active, #header-filter a:visited { color:#15428B; text-decoration:underline;  font-weight:normal; margin: 0 0 0 7px; font-size: .8em; }
    #header-filter a:hover {  }
    #tag-rename-edit { }
    #tag-rename-edit input { width: 250px; margin-top: 10px;}
    #header-pager { font-size: 1.2em; text-align:center; padding-bottom: 10px;}
    #header-pager strong {padding: 0 3px; font-size: 1.2em;}
    #header-pager a, #header-pager a:link, #header-pager a:active, #header-pager a:visited { color:#15428B; text-decoration:none;  font-weight:normal; padding: 0 3px;}
    #header-pager a:hover { text-decoration:underline; }
    #header-pager a.page-nav, #header-pager a.page-nav:link, #header-pager a.page-nav:visited, #header-pager a.page-nav:active, #header-pager a.page-nav:hover { font-weight:bold;}
    #next { padding-left: 10px;}
    #prev { padding-right: 10px;}
    .tag-link-selected { font-weight:bold; font-size: 1.3em;}

    .ui-layout-resizer { background:#8AAEDE;}
    .ui-layout-toggler { background:#6289BD;}
    .ui-layout-pane { background: #fff;}
	}

  </style>

    <script src="/frontend/javascript/vendors/growthscore.js?1515087127" type="text/javascript"></script>

</head>
<body>

  <script type="text/javascript" language="javascript">
      var _ticketTypes = null;
      var loaded = false;
      function onShow() {
              loadTags();
    }

    function pageLoad() {


      $('#container-main').layout({
        resizeNestedLayout: true,

        center: {
          paneSelector: "#pane-content"
        },
        defaults: {
          spacing_open: 1,
          spacing_closed: 8
        },
        south: {
          paneSelector: "#pane-preview",
          size: 250,
          initClosed: true,
          spacing_open: 8
        },
        east: {
          paneSelector: "#pane-filters",
          size: 300,
          closable: false
        }

      });

      $('#pane-content').layout({


        defaults: {
          spacing_open: 1,
          spacing_closed: 8
        },
        center: {
          paneSelector: "#pane-grid"
        },
        north: {
          paneSelector: "#pane-header",
          resizable: false,
          closable: false,
          size: "auto"
        }
      });


      $('button').button();

      TSSystem.Init();
      TSSystem.Services.Tickets.GetTicketTypes(function (result) { _ticketTypes = result; loadTags(); });

      //$('#btn-search').click(function (e) { loadTickets(); e.preventDefault(); });
      $('#lnk-reset-tags').click(function (e) {
        e.preventDefault();
        clearSelectedTags();
        loadTickets();
      });

      $('#lnk-multiple-tags').click(function (e) {
        e.preventDefault();

        if (isSingleTagMode()) {
          $(this).html('Select single tag');
          $('#lnk-reset-tags').show();
        }
        else {
          $(this).html('Select multiple tags');
          var first = $('.tag-link-selected:first');
          clearSelectedTags();
          if (first != null) first.addClass('tag-link-selected');
          $('#lnk-reset-tags').hide();
          parent.Ts.System.logAction('Ticket Tags - Multiple Tag Mode');

        }
            loadTickets();
      });

      $('#tag-rename').click(function (e) {

        e.preventDefault();
        toggleRenameTag(true);
        $('#tag-rename-edit input').val($('.tag-link-selected').html()).focus().select();
      });

      $('#tag-reload').click(function (e) {

          e.preventDefault();
          loaded = false;
          location.reload();

      });

      $('#tag-rename-save').click(function (e) {
        e.preventDefault();
        toggleRenameTag(false);
        var id = getItemID('tagid', $('.tag-link-selected')[0]);
        TSSystem.Services.Tickets.RenameTag(id, $('#tag-rename-edit input').val(), function (result) { loadTags(result); });
        parent.Ts.System.logAction('Ticket Tags - Tag Renamed');
      });

      $('#tag-rename-cancel').click(function (e) {
        e.preventDefault();
        toggleRenameTag(false);
      });

      $('#tag-delete').click(function (e) {
        e.preventDefault();
        if (!confirm('Are you sure you would like to PERMANENTLY delete this tag?')) return;
        var id = getItemID('tagid', $('.tag-link-selected')[0]);
        TSSystem.Services.Tickets.DeleteTag(id);
        loadTags();
        parent.Ts.System.logAction('Ticket Tags - Tag Deleted');
      });
    }

    function toggleRenameTag(show) {
      if (show) {
        $('#header-tags').hide();
        $('#tag-actions').hide();
        $('#tag-rename-edit').show();
      }
      else {
        $('#header-tags').show();
        $('#tag-actions').show();
        $('#tag-rename-edit').hide();
      }
    }
    function clearSelectedTags() { $('.tag-link-selected').removeClass('tag-link-selected'); }
    function isSingleTagMode() { return $('#lnk-multiple-tags').html() != 'Select single tag'; }

    function loadTickets(pageIndex) {
        loaded = true;
      toggleRenameTag(false);
      $('#tickets').hide();
      $('#tickets-loading').show();
      var filter = new TeamSupport.Data.TicketLoadFilter();

      filter.SearchText = $('#text-search').val();

      filter.Tags = new Array();
      var tags = $('.tag-link-selected');
      for (var i = 0; i < tags.length; i++) { filter.Tags[filter.Tags.length] = getItemID('tagid', tags[i]); }
      var i = pageIndex == null ? 0 : pageIndex;
      TSSystem.Services.Tickets.GetTicketPage(i, 25, filter, showTickets);
    }

    function getTicketIconUrl(ticketTypeID) {
      for (var i = 0; i < _ticketTypes.length; i++) {
        if (_ticketTypes[i].TicketTypeID == ticketTypeID) {
          return _ticketTypes[i].IconUrl;
        }
      }
      return '';
    }

    function showTickets(ticketPage) {
      if (ticketPage == null) return;
      var tickets = ticketPage.Tickets;
      var html = '';
      for (var i = 0; i < tickets.length; i++) {
        var ticket = tickets[i];
        html = html +
        '<div class="ticket ticketid-' + ticket.TicketID + '"><div class="ticket-left"><div class="ticket-icon">' +
        '<img alt="Ticket" src="/' + getTicketIconUrl(ticket.TicketTypeID) + '" /></div><div class="ticket-text">' +
        '<h1><a href="#" class="ts-link ticket-link ticketid-' + ticket.TicketID + '">' + ticket.TicketNumber + ': ' + ticket.Name + '</a></h1>' +
        '<h2>Status: ' + ticket.Status + '</h2>' +
        '<h2>Severity: ' + ticket.Severity + '</h2></div></div><div class="ticket-right">';
        //if (ticket.UserName != null) html = html + '<h2>Assigned To: ' + ticket.UserName + '</h2>';
        //if (ticket.ProductName != null) html = html + '<h2>Product: ' + ticket.ProductName + '</h2>';
        if (ticket.Tags != null) html = html + '<h2>' + ticket.Tags + '</h2>';

        html = html + '</div><div class="ui-helper-clearfix"></div></div>';
      }

      $('#tickets').html(html).show();
      $('#tickets-loading').hide();
      $('#header-pager').html(getPagerHtml(ticketPage));
      $('#header-tags').html(getTagsHtml());
      $('.page-num').click(function (e) { e.preventDefault(); loadTickets($(this).html() - 1); });
      $('#next').click(function (e) { e.preventDefault(); loadTickets($('.page-selected').html()); });
      $('#prev').click(function (e) { e.preventDefault(); loadTickets($('.page-selected').html() - 2); });
      $('.ticket-link').click(function (e) { e.preventDefault(); parent.Ts.MainPage.openTicketByID(getItemID('ticketid', this), true); });


    }

    function getPagerHtml(ticketPage) {
      var html = '';
      if (ticketPage.Count < 1) return html;
      var pageCount = (ticketPage.Count / ticketPage.PageSize) | 0;
      if (ticketPage.Count % ticketPage.PageSize != 0) pageCount++;
      if (pageCount < 2) return '<strong class="page-selected">1 Page</strong>';
      var index = ticketPage.PageIndex + 1;
      var start = index - 5;
      if (start < 1) start = 1;
      var end = start + 11;
      if (end > pageCount) end = pageCount;
      for (var i = start; i <= end; i++) {
        if (i == index)
          html = html + ' <strong class="page-selected">' + i + '</strong>';
        else
          html = html + ' <a href="#" class="page-num">' + i + '</a>';
      }
      if (index != 1) { html = '<a href="#" id="prev" class="page-nav">Previous</a> ' + html; }
      if (index < pageCount) { html = html + ' <a href="#" id="next" class="page-nav">Next</a>'; }
      return html;
    }

    function getTagsHtml() {

      var html = '';
      var cnt = 0;
      $('.tag-link-selected').each(function (index) {
        cnt++;
        if (html == '')
          html = $(this).html();
        else
          html = html + ', ' + $(this).html();

      });

      if (cnt == 1) $('#tag-actions').show(); else $('#tag-actions').hide();

      if (html == '') {
        return '<strong>No tags are selected</strong>';
      }

      return '<strong>Selected Tags: </strong>' + html;

    }

    function getItemID(idClass, element) {
      var classes = $(element).attr('class').toLowerCase().split(' ');
      idClass = idClass.toLowerCase() + '-';
      for (var i = 0; i < classes.length; i++) {
        if (classes[i].indexOf(idClass) > -1) {
          var a = classes[i].split('-');
          return a[a.length - 1];
        }
      }
    }


    function loadTags(tagID) {

      TSSystem.Services.Tickets.GetTags(function (tags) {
        var html = '';
        var c = '';
        var queryValue = null;
        var queryID = null;
        if (tagID == null) {
          queryValue = TSSystem.Utils.getQueryValue('Tag', window);
          if (queryValue != null) queryValue == queryValue.toLowerCase();
          tagID = TSSystem.Utils.getQueryValue('TagID', window);
        }
        for (var i = 0; i < tags.length; i++) {
          var tag = tags[i];
          if (c != tag.Value.toUpperCase().charAt(0)) {
            if (c != '') {
              html = html + '</span></dd>';
            }
            c = tag.Value.toUpperCase().charAt(0);
            html = html + '<dt>' + c + '</dt><dd><span>';
          }
          else {
            html = html + ", ";
          }

          var className = 'tag-link tagid-' + tag.TagID;
          if ((queryValue != null && tag.Value.toLowerCase() == queryValue) || (tagID != null && tag.TagID == tagID)) className = className + ' tag-link-selected';

          html = html + '<a class="' + className + '" href="#">' + tag.Value + '</a>';
        }
        html = html + '</span></dd>';
        $('#tags dl').html(html);

        $('.tag-link').click(function (e) {
          e.preventDefault();
          if (isSingleTagMode()) {
            clearSelectedTags();
          }
          $(this).toggleClass('tag-link-selected');
          loadTickets();
          parent.Ts.System.logAction('Ticket Tags - Tag Selected');

        });
        if(!loaded)
        loadTickets();
      });

    }

  </script>

  <form id="form1" runat="server">
  <asp:ScriptManager ID="ScriptManager1" runat="server">
    <Services>
      <asp:ServiceReference Path="~/Services/TicketService.asmx" />
    </Services>
  </asp:ScriptManager>
  <div id="container-main">
    <div id="pane-content" class="pane">
      <div id="pane-grid" class="pane">
        <div id="tickets-loading"><img alt="loading..." src="../images/loading/loading2.gif"/></div>
        <div id="tickets" class="ui-helper-hidden">

        </div>
      </div>
      <div id="pane-header" class="pane">
        <div id="header">
          <div id="header-search" class="ui-helper-hidden">
            <span>(DEMO - check out speed w/ jquery) Search: </span>
            <input name="text-search" id="text-search" type="text" class="text" />
          </div>
          <div id="header-filter">
            <span id="header-tags"></span>
            <span id="tag-actions"><a href="#" id="tag-rename">Rename</a>&nbsp<a href="#" id="tag-delete">Delete</a>&nbsp<a href="#" id="tag-reload">Clear</a></span>
            <span id="tag-rename-edit" class="ui-helper-hidden"><input type="text" class="text"  /> <a href="#" id="tag-rename-save">Save</a> | <a href="#" id="tag-rename-cancel">Cancel</a></span>
            &nbsp
          </div>
          <div id="header-pager">
            &nbsp
          </div>

        </div>
      </div>
    </div>
    <div id="pane-previewx" class="pane">
      Preview Pane
    </div>
    <div id="pane-filters"class="pane">
      <div id="filters">
      <h1><span class="tag-edit"><a href="#" id="lnk-reset-tags" class="ui-helper-hidden">Clear selected</a> &nbsp; <a href="#" id="lnk-multiple-tags">Select multiple tags</a></span>Tags</h1>
      <div id="tags">
        <dl>

        </dl>
      </div>
      </div>
    </div>

  </div>
  </form>
</body>
</html>
