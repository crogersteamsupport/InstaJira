var wikiPage = null;
var _wikiArticles = null;
var _wikiID = null;
var _wikiParentID = null;
var _wikiInternalLLinkBase = parent.Ts.System.AppDomain + "?articleid={ArticleID}";
var _wikiExternalLinkBase = parent.Ts.System.AppDomain + "/wiki/justarticle.aspx?Organizationid={ORGID}&ArticleID={ArticleID}";
var _wikiInternalLink = null;
var _wikiExternalLink = null;
var _wikiTitle = null;
var _wikiBody = null;
var _wikiSubArticleList = null;
var _wikiPortalView = false;
var _wikiPublicView = false;
var _wikiPrivateView = false;
var _wikiVersion = 0;
var _wikiModifiedDate = null;
var _wikiParentsList = null;
var _wikiMenuLITemplate = '<li class="wiki-menu-item"><a id="{ID}" href="#">{Title}</a>';
var _wikiMenuLIWithChildrenTemplate = '<li class="wiki-menu-item"><a id="{ID}" href="#">{Title}<span class="caret wiki-sidebar-caret wiki-sidebar-caret-right"></span></a>';
var _wikiSubMenuULTemplate = '<ul class="nav wiki-sidebar-subitem">';
var _wikiSubMenuLITemplate = '<li class="wiki-menu-subitem"><a id="{ID}" href="#">{Title}<span class="caret wiki-sidebar-caret  wiki-sidebar-caret-right"></span></a></li>';

$(document).ready(function() {
    wikiPage = new WikiPage();
    wikiPage.refresh();
});

function onShow() {};

WikiPage = function() {
    this.refresh = function() {
        BuildWikiPage();
        BuildWikiEditEvents();
    }
}

//Build Page Functions
function BuildWikiPage() {
    $('.page-loading').show().next().hide();
    parent.Ts.Services.Wiki.GetWikiMenuItems(function(menuItems) {
        _wikiArticles = menuItems;
        if (menuItems !== null) {
            if (_wikiID == null) {
                var articleID = parent.Ts.Utils.getQueryValue("ArticleID", window);
                if (articleID == null) {
                    parent.Ts.Services.Wiki.GetDefaultWikiID(function(wikiID) {
                        if (wikiID == null) {
                            _wikiID = menuItems[0].ID
                        } else {
                            _wikiID = wikiID;
                        }
                        GetWiki(_wikiID);
                        BuildWikiMenuItems();
                    });
                } else {
                    GetWiki(articleID);
                    BuildWikiMenuItems();
                }
            } else {
                GetWiki(_wikiID);
                BuildWikiMenuItems();
            };
            $("#EditWiki").show();
        } else {
            $('.page-loading').hide().next().show();
            $("#EditWiki").show();
        };
    });
    parent.Ts.System.logAction('Wiki - Wiki Viewed');
};


function BuildWikiView() {
    var internalLink = new ZeroClipboard(document.getElementById("wiki-internal-link"));
    $("#wiki-internal-link").data('copy', _wikiInternalLink);
    if (!_wikiPublicView) {
        $("#wiki-external-link").hide();
    } else {
        var externalLink = new ZeroClipboard(document.getElementById("wiki-external-link"));
        $("#wiki-external-link").data('copy', _wikiExternalLink);
        $("#wiki-external-link").show();
    }

    $(".wiki-tools").tooltip({
        placement: 'bottom',
        animation: false
    })

    $("#Wiki-Title").text(_wikiTitle);
    $("#Wiki-Body").html(_wikiBody);
    $("#WikiViewArea").show();
    $("#wiki-view-toolbar").show();
    $('#Wiki-Title').show();

    $('a[href*="JustArticle.aspx"]').click(function(e) {
        e.preventDefault();
        var url = ($(this).attr('href'));
        var ArticleID = getURLParameter(url, 'ArticleID');
        parent.Ts.MainPage.openWiki(ArticleID, true)
    });
};

function getURLParameter(url, name) {
    return (RegExp(name + '=' + '(.+?)(&|$)').exec(url) || [, null])[1];
}

function BuildWikiMenuItems() {
    $("#wiki-sidebar").empty();

    parent.Ts.Services.Wiki.GetWikis(function(wikis) {
        for (i = 0; i < wikis.length; i++) {
            $('#Wiki-Edit-Parent').append($("<option></option>").attr("value", wikis[i].ID).text(wikis[i].Title));
        }
    });

    parent.Ts.Services.Wiki.GetWikiParents(function(wikis) {
        if (wikis !== null) {
            var menuParents = "";
            var subMenuItems = "";

            for (i = 0; i < wikis.length; i++) {
                var parent = wikis[i];
                $("#wiki-sidebar").append(_wikiMenuLIWithChildrenTemplate.replace("{ID}", parent.ID).replace("{Title}", parent.Title));
                recursiveFunction(parent.ID, parent);
                $("#" + parent.ID).on('click', function(e) {
                    e.preventDefault();
                    SidebarFunction($(this));
                });
            }
            $('.page-loading').hide().next().show();
        }
    });
};

function recursiveFunction(key, parent) {
    window.parent.Ts.Services.Wiki.GetWikiAndChildren(parent.ID, function(children) {
        if (children.SubArticles !== null) {
            $("#" + parent.ID).parent().append(_wikiSubMenuULTemplate);
            $.each(children.SubArticles, function(key, child) {
                $("#" + parent.ID).parent().children("ul").append(_wikiSubMenuLITemplate.replace("{ID}", child.ID).replace("{Title}", child.Title));
                recursiveFunction(key, child)
                $("#wiki-sidebar").append("</li>");
                $("#" + child.ID).on('click', function(e) {
                    e.preventDefault();
                    SidebarFunction($(this));
                });
            });
        } else {
            $("#" + parent.ID + " > span.wiki-sidebar-caret").remove();
        }
        if (parent.ID == _wikiID) {
            var wikiMenuItem = $("#" + _wikiID);
            wikiMenuItem.addClass('active');
            wikiMenuItem.closest('li').children("ul").show();
            wikiMenuItem.find("span.wiki-sidebar-caret").removeClass('wiki-sidebar-caret-right');
            wikiMenuItem.parents('li.wiki-menu-item').children('a').children('span.wiki-sidebar-caret-right').removeClass('wiki-sidebar-caret-right');
            wikiMenuItem.parents('li.wiki-menu-subitem').children('a').children('span.wiki-sidebar-caret-right').removeClass('wiki-sidebar-caret-right');
            wikiMenuItem.parents('.wiki-sidebar-subitem').show();
        }
    });
}

function SidebarFunction(element) {
    $('.wiki-menu-item').children("a").removeClass('active');
    $('.wiki-menu-subitem').children("a").removeClass('active');
    element.addClass('active');
    element.closest('li').children("ul").toggle();
    element.closest('li').find("span.wiki-sidebar-caret").first().toggleClass('wiki-sidebar-caret-right');
    GetWiki(element[0].id);
};

function BuildWikiEditEvents() {
    $("#EditWiki").click(function() {
        parent.Ts.MainPage.editWiki(_wikiID)
    });

    $("#NewWiki").click(function() {
        parent.Ts.MainPage.editWiki(-1);
    });

    $("#WikiLink").click(function(e) {
        e.preventDefault();
    });
};

function MapWikiProperties(wiki) {
    _wikiID = wiki.ArticleID;
    _wikiParentID = wiki.ParentID;
    _wikiTitle = wiki.ArticleName;
    _wikiBody = wiki.Body;
    _wikiVersion = wiki.Version;
    _wikiPortalView = wiki.PortalView;
    _wikiPublicView = wiki.PublicView;
    _wikiPrivateView = wiki.Private;
    _wikiModifiedDate = wiki.ModifiedDate;
    _wikiExternalLink = _wikiExternalLinkBase.replace("{ORGID}", wiki.OrganizationID).replace("{ArticleID}", wiki.ArticleID);
    _wikiInternalLink = _wikiInternalLLinkBase.replace("{ArticleID}", wiki.ArticleID);
    _editingWiki = false;
    _isWikiOwner = wiki.IsOwner;
    _canDeleteWiki = wiki.CanDelete;
};

function GetWiki(wikiID) {
    parent.Ts.Services.Wiki.GetWiki(wikiID, function(wiki) {
        if (wiki !== null) {
            MapWikiProperties(wiki);
            BuildWikiView();
        } else {
            alert('You do not have access to this wiki or the wiki no longer exists.  Check your default wiki settings to ensure you are not using a old deleted wiki or select a wiki from the menu below. ')
        }
    });
};
