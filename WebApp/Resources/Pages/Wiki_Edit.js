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
var _editingWiki = false;
var _isWikiOwner = false;
var _canDeleteWiki = false;
var _isCreatingNewWiki = false;

$(document).ready(function () {
    wikiPage = new WikiPage();
    wikiPage.refresh();
});

function onShow() {
};

WikiPage = function () {
  this.refresh = function () {
    $('.page-loading').show().next().hide();
    $(".wiki-tools").tooltip({ placement: 'bottom', animation: false });
    $('a[href*="JustArticle.aspx"]').click(function (e) {
      e.preventDefault();
      var url = ($(this).attr('href'));
      var ArticleID = getURLParameter(url, 'ArticleID');
      parent.Ts.MainPage.openWiki(ArticleID, true)
    });

    BuildWikiView();
    BuildWikiEditEvents();
  }
};

function BuildWikiView() {
  var articleID = parent.Ts.Utils.getQueryValue("ArticleID", window);debugger
  if (articleID !== null) {
    if (articleID !== '-1') {
      GetWiki(articleID, function (wiki) {
        _wikiID = wiki.ArticleID;
        $("#wiki-title-edit").val(wiki.ArticleName);
        $("#Wiki-Edit-PublicView").prop('checked', wiki.PublicView);
        $("#Wiki-Edit-PrivateView").prop('checked', wiki.Private);
        $("#Wiki-Edit-PortalView").prop('checked', wiki.PortalView);
        GetWikiParents(function (parents) {
          for (i = 0; i < parents.length; i++) {
            $('#Wiki-Edit-Parent')
              .append($("<option></option>")
              .attr("value", parents[i].ID)
              .text(parents[i].Title));
          }
          $("#Wiki-Edit-Parent").val(wiki.ParentID);
        });
        GetWikiHistory(_wikiID);

        var element = $('#Wiki-Edit-Body');
        initEditor(element, function (ed) {
          tinyMCE.activeEditor.setContent(wiki.Body);
          $('#Wiki-Edit-Body').tinymce().focus();
        });

        $('.page-loading').hide().next().show();
      });
    }
    else {
      _isCreatingNewWiki = true;
      GetWikiParents(function (parents) {
        for (i = 0; i < parents.length; i++) {
          $('#Wiki-Edit-Parent')
            .append($("<option></option>")
            .attr("value", parents[i].ID)
            .text(parents[i].Title));
        }
      });

      var element = $('#Wiki-Edit-Body');
      initEditor(element, function (ed) {
        $('#Wiki-Edit-Body').tinymce().focus();
      });

      $('.page-loading').hide().next().show();
    }
  }
  else {
    alert('We are having issues locating the wiki article you are trying to edit.  Please try again.')
  }
};

function GetWikiParents(callback) {
  parent.Ts.Services.Wiki.GetWikis(function (wikis) {
    callback(wikis);
  });
}

function getURLParameter(url, name) {
    return (RegExp(name + '=' + '(.+?)(&|$)').exec(url) || [, null])[1];
}

function BuildWikiEditEvents() {
    $("#wiki-edit-cancel").click(function (e) {
      e.preventDefault();
      parent.Ts.MainPage.openWiki(_wikiID)
    });

    $("#wiki-edit-delete").click(function (e) {
        e.preventDefault();
        if ($("#recorder").length == 0) {
          var confirmation = confirm("Are you sure you want to delete this wiki article?");
          if (confirmation) {
            DeleteWiki(_wikiID);
          }
        }
    });

    $("#wiki-edit-save").click(function () {
      if ($("#recorder").length == 0) {
        var comment = $("#Wiki-Update-Comment").val();
        var title = $("#wiki-title-edit").val();
        var body = tinymce.activeEditor.getContent();
        var public = $("#Wiki-Edit-PublicView").is(':checked');
        var private = $("#Wiki-Edit-PrivateView").is(':checked');
        var portal = $("#Wiki-Edit-PortalView").is(':checked');
        var parentid = $('#Wiki-Edit-Parent').val();
        var wikiID;

        if (title == '')
        {
            alert('Please enter a valid title.')
            return;
        }
       

        if (_isCreatingNewWiki) {
          wikiID = 0;
        }
        else {
          wikiID = _wikiID;
        }
        if (wikiID.toString() !== parentid) {

          SaveWiki(wikiID, parentid, body, title, public, private, portal, comment);
        }
        else { alert('Please select a parent article other than the one you are editing.') };
      }
    });

    $("#wiki-title-edit").on('keydown', function (e) {
        var keyCode = e.keyCode || e.which;
        if (keyCode == 9) {
            e.preventDefault();
            $("#Wiki-Edit-Body").tinymce().focus();
        }
    });

    $('#Wiki-Comment-Modal').on('shown.bs.modal', function (e) {
        $("#Wiki-Update-Comment").focus();
    });
};

function GetWiki(wikiID, callback) {
    parent.Ts.Services.Wiki.GetWiki(wikiID, function (wiki) {
      callback(wiki);
    });
};

function GetWikiHistory(wikiID) {
    parent.Ts.Services.Wiki.GetWikiHistory(wikiID, function (wikiHistory) {
        $('.wiki-revision-history tbody').empty();
        if (wikiHistory !== null) {
            $('.wiki-revision-div').show();
            $.each(wikiHistory, function (key, value) {
                $(".wiki-revision-history tbody").append('<tr><td>' + value.RevisionNumber + '</td><td>' + value.RevisedDate.localeFormat(parent.Ts.Utils.getDateTimePattern()) + '</td><td>' + value.RevisedBy + '</td><td>' + value.Comment + '</td><td><button data-id="' + value.HistoryID + '" class="btn btn-primary btn-xs wiki-restore">Preview</button></td></tr>');
            });
        }
        else {
            $('.wiki-revision-div').hide();
        }
        $(".wiki-restore").click(function () {
            var wikiRevisionID = $(this).data("id");
            $('.wiki-revision-history tbody > tr').removeClass('active');
            $(this).closest('tr').addClass('active');

            if (wikiRevisionID !== "") {
                parent.Ts.Services.Wiki.GetWikiRevision(wikiRevisionID, function (revision) {
                    $("#wiki-title-edit").val(revision.ArticleName);
                    $("#Wiki-Edit-Body").html(revision.Body);
                    $("#Wiki-Edit-PublicView").prop('checked', false);
                    $("#Wiki-Edit-PrivateView").prop('checked', false);
                    $("#Wiki-Edit-PortalView").prop('checked', false);
                });
            }
            else {
                $("#wiki-title-edi").val(_wikiTitle);
                $("#Wiki-Edit-Body").html(_wikiBody);
                $("#Wiki-Edit-PublicView").prop('checked', _wikiPublicView);
                $("#Wiki-Edit-PrivateView").prop('checked', _wikiPrivateView);
                $("#Wiki-Edit-PortalView").prop('checked', _wikiPortalView);
            };
            parent.Ts.System.logAction('Wiki - Viewed Revision');
        });
    });
};

function SaveWiki(wikiID, parentid, wikiBody, wikiTitle, publicView, privateView, portalView, comment) {
  parent.Ts.Services.Wiki.SaveWiki(wikiID, parentid, wikiBody, wikiTitle, publicView, privateView, portalView, comment, function (wiki) {
    parent.Ts.MainPage.openWiki(wiki.ArticleID);
    parent.Ts.System.logAction('Wiki - Wiki Saved');
  });
};

function DeleteWiki(wikiID) {
  debugger
  parent.Ts.Services.Wiki.DeleteWiki(wikiID, function () {
    parent.Ts.MainPage.openWiki(_wikiID)
    parent.Ts.System.logAction('Wiki - Wiki Deleted');
  });
};

var initEditor = function (element, init) {
  parent.Ts.Settings.System.read('EnableScreenR', 'True', function (enableScreenR) {
    var editorOptions = {
      plugins: "autoresize paste link code textcolor image moxiemanager table",
      toolbar1: "insertPasteImage insertKb insertTicket image insertimage insertDropBox recordScreen insertUser insertWiki | link unlink | undo redo removeformat | cut copy paste pastetext | outdent indent | bullist numlist",
      toolbar2: "alignleft aligncenter alignright alignjustify | forecolor backcolor | fontselect fontsizeselect | bold italic underline strikethrough blockquote | code | table",
      branding: false,
      statusbar: false,
      gecko_spellcheck: true,
      extended_valid_elements: "a[accesskey|charset|class|coords|dir<ltr?rtl|href|hreflang|id|lang|name|onblur|onclick|ondblclick|onfocus|onkeydown|onkeypress|onkeyup|onmousedown|onmousemove|onmouseout|onmouseover|onmouseup|rel|rev|shape<circle?default?poly?rect|style|tabindex|title|target|type],script[charset|defer|language|src|type]",
      content_css: "../Css/jquery-ui-latest.custom.css,../Css/editor.css",
      body_class: "ui-widget ui-widget-content wiki-desc-editor",
      convert_urls: true,
      remove_script_host: false,
      relative_urls: false,
      template_external_list_url: "tinymce/jscripts/template_list.js",
      external_link_list_url: "tinymce/jscripts/link_list.js",
      external_image_list_url: "tinymce/jscripts/image_list.js",
      media_external_list_url: "tinymce/jscripts/media_list.js",
      menubar: false,
      moxiemanager_leftpanel: false,
      moxiemanager_fullscreen: false,
      moxiemanager_title: parent.Ts.System.Organization.Name,
      moxiemanager_hidden_tools: (parent.Ts.System.User.IsSystemAdmin == true) ? "" : "manage",
      paste_data_images: true,
      images_upload_url: "/Services/UserService.asmx/SaveTinyMCEPasteImage",
      moxiemanager_image_settings: {
        moxiemanager_rootpath: "/" + parent.Ts.System.Organization.OrganizationID + "/images/",
        extensions: 'gif,jpg,jpeg,png'
      },

      setup: function (ed) {
        ed.on('init', function (e) {
          parent.Ts.System.refreshUser(function () {
            if (parent.Ts.System.User.FontFamilyDescription != "Unassigned") {
              ed.execCommand("FontName", false, GetTinyMCEFontName(parent.Ts.System.User.FontFamily));
              ed.getBody().style.fontFamily = GetTinyMCEFontName(parent.Ts.System.User.FontFamily);
            }
            else if (parent.Ts.System.Organization.FontFamilyDescription != "Unassigned") {
              ed.execCommand("FontName", false, GetTinyMCEFontName(parent.Ts.System.Organization.FontFamily));
              ed.getBody().style.fontFamily = GetTinyMCEFontName(parent.Ts.System.Organization.FontFamily);
            }

            if (parent.Ts.System.User.FontSize != "0") {
              ed.execCommand("FontSize", false, parent.Ts.System.User.FontSizeDescription);
              ed.getBody().style.fontSize = GetTinyMCEFontSize(parent.Ts.System.User.FontSize + 1);
            }
            else if (parent.Ts.System.Organization.FontSize != "0") {
              ed.execCommand("FontSize", false, parent.Ts.System.Organization.FontSize + 1);
              ed.getBody().style.fontSize = GetTinyMCEFontSize(parent.Ts.System.Organization.FontSize + 1);
            }

            if (_isCreatingNewWiki) {
              $("#wiki-title-edit").focus();
            }

          });
        });

        ed.on('paste', function (ed, e) {
          setTimeout(function () { ed.execCommand('mceAutoResize'); }, 1000);
        });

        ed.addButton('insertTicket', {
          title: 'Insert Ticket',
          //image: '../images/nav/16/tickets.png',
          icon: 'awesome fa fa-ticket',
          onclick: function () {
            parent.Ts.System.logAction('Wiki - Ticket Inserted');

            parent.Ts.MainPage.selectTicket(null, function (ticketID) {
              parent.Ts.Services.Tickets.GetTicket(ticketID, function (ticket) {
                ed.focus();
                var html = '<a href="' + parent.Ts.System.AppDomain + '?TicketNumber=' + ticket.TicketNumber + '" target="_blank" onclick="parent.Ts.MainPage.openTicket(' + ticket.TicketNumber + '); return false;">Ticket ' + ticket.TicketNumber + '</a>';
                ed.selection.setContent(html);
                ed.execCommand('mceAutoResize');
                ed.focus();
              }, function () {
                alert('There was a problem inserting the ticket link.');
              });
            });
          }
        });

        ed.addButton('insertPasteImage', {
          title: 'Insert Pasted Image',
          //image: '../images/nav/16/imagepaste.png',
          icon: 'awesome fa fa-paste',
          onclick: function () {
            if (BrowserDetect.browser == 'Safari' || BrowserDetect.browser == 'Explorer') {
              alert("Sorry, this feature is not supported by " + BrowserDetect.browser);
            }
            else {
              parent.Ts.MainPage.pasteImage(null, function (result) {
                ed.focus();
                if (result != "") {
                  var html = '<img src="' + parent.Ts.System.AppDomain + '/dc/' + result + '"</a>&nbsp;<br/>';
                  ed.selection.setContent(html);
                  setTimeout(function () { ed.execCommand('mceAutoResize'); }, 1000);
                  ed.execCommand('mceAutoResize');
                  ed.focus();
                }
              });
            }
          }
        });

        ed.addButton('insertUser', {
          title: 'Insert Userstamp',
          icon: 'awesome fa fa-clock-o',
          //image: '../images/icons/dropbox.png',
          onclick: function () {
            var html = Date(Date.UTC(Date.Now)) + ' ' + parent.Ts.System.User.FirstName + ' ' + parent.Ts.System.User.LastName + ' : ';
            ed.selection.setContent(html);
            ed.execCommand('mceAutoResize');
            ed.focus();
          }
        });

        ed.addButton('insertDropBox', {
          title: 'Insert DropBox',
          icon: 'awesome fa fa-dropbox',
          //image: '../images/icons/dropbox.png',
          onclick: function () {
            var options = {
              linkType: "preview",
              success: function (files) {
                ed.focus();
                var html = '<a href=' + files[0].link + ' target="_blank">' + files[0].name + '</a>';
                ed.selection.setContent(html);
                ed.execCommand('mceAutoResize');
                ed.focus();
                parent.Ts.System.logAction('Wiki - Dropbox Added');
              },
              cancel: function () {
                alert('There was a problem inserting the dropbox file.');
              }
            };
            Dropbox.choose(options);
          }
        });
        ed.addButton('insertKb', {
          title: 'Insert Knowledgebase',
          //image: '../images/nav/16/knowledge.png',
          icon: 'awesome fa fa-book',
          onclick: function () {
            filter = new parent.TeamSupport.Data.TicketLoadFilter();
            filter.IsKnowledgeBase = true;
            parent.Ts.MainPage.selectTicket(filter, function (ticketID) {
              parent.Ts.Services.Tickets.GetKBTicketAndActions(ticketID, function (result) {
                if (result === null) {
                  alert('There was an error inserting your knowledgebase ticket.');
                  return;
                }
                var ticket = result[0];
                var actions = result[1];

                var html = '<div>';

                for (var i = 0; i < actions.length; i++) {
                  html = html + '<div>' + actions[i].Description + '</div></br>';
                }
                html = html + '</div>';

                ed.focus();
                ed.selection.setContent(html);
                ed.execCommand('mceAutoResize');
                ed.focus();
                parent.Ts.System.logAction('Wiki - KB Inserted');
                //needs to resize or go to end

              }, function () {
                alert('There was an error inserting your knowledgebase ticket.');
              });
            });
          }
        });

        ed.addButton('insertWiki', {
          title: 'Insert Wiki Article',
          icon: 'awesome fa fa-file',
          onclick: function () {
            var bookmark = ed.selection.getBookmark(0);
            parent.Ts.MainPage.selectWiki(function (wikiID) {
              parent.Ts.Services.Wiki.GetWiki(wikiID, function (wiki) {
                if (wiki === null) {
                  alert('There was an error inserting your wiki article.');
                  return;
                }
                var html = '<a href="' + parent.Ts.System.AppDomain + '/wiki/JustArticle.aspx?Organizationid=' + wiki.OrganizationID + '&amp;ArticleID=' + wiki.ArticleID + '">' + wiki.ArticleName + '</a>';
                ed.focus();
                ed.selection.moveToBookmark(bookmark);
                ed.selection.setContent(html);
                ed.execCommand('mceAutoResize');
                ed.focus();
                parent.Ts.System.logAction('Wiki - Wiki Inserted');
              }, function () {
                alert('There was an error inserting your wiki article.');
              });
            });
          }
        });

        if (enableScreenR.toLowerCase() != 'false' && !(BrowserDetect.browser == 'Safari' && BrowserDetect.OS == "Windows")) {
          ed.addButton('recordScreen', {
            title: 'Record Screen',
            //image: '../images/icons/Symbol_Record.png',
            icon: 'awesome fa fa-circle',
            onclick: function () {
              if ($("#recorder").length == 0) {

                switch (BrowserDetect.browser) {
                  case "Chrome":
                    parent.Ts.Services.Settings.ReadUserSetting('ReadScreenRecordingChromeInfo', 0, function (alreadyReadInfo) {
                      if (alreadyReadInfo == 0) {
                        $(".pAllowPluginsToRunInstructions").html("\
To use screen recording in this browser before September of 2015 \
<a href='https://support.google.com/chrome/answer/6213033' target='_blank' class='ui-state-default ts-link'>these instructions</a> \
need to be followed, otherwise the browser will continue to ask to install Java. \
After September 2015 you’ll need to use an alternate web browser like FireFox or Internet Explorer.<br><br>\
Also, the Java plugins need to be allowed to run by clicking on the \
<img src='../Images/icons/ChromePluginIcon.png' alt='plugins icon'> \
on the right side of the address bar");
                        $('.divScreenRecorderMessages').show();
                      }
                    });
                    break;
                  case "Firefox":
                    parent.Ts.Services.Settings.ReadUserSetting('ReadScreenRecordingFirefoxInfo', 0, function (alreadyReadInfo) {
                      if (alreadyReadInfo == 0) {
                        $(".pAllowPluginsToRunInstructions").html("\
Please allow the screen recorder Java plugins to run on your browser by clicking on the \
<img src='../Images/icons/FirefoxPluginIcon.png' alt='plugins icon'> \
on the left side of the address bar.");
                        $('.divScreenRecorderMessages').show();
                      }
                    });
                    break;
                  case "Explorer":
                    parent.Ts.Services.Settings.ReadUserSetting('ReadScreenRecordingExplorerInfo', 0, function (alreadyReadInfo) {
                      if (alreadyReadInfo == 0) {
                        $(".pAllowPluginsToRunInstructions").html("\
Please allow the screen recorder Java plugins to run on your browser by clicking on Allow button at the bottom of the page: \
<img src='../Images/icons/IEPluginWindow.png' alt='plugins icon' width='100%' style='margin-top: 10px'>");
                        $('.divScreenRecorderMessages').show();
                      }
                    });
                    break;
                  case "Safari":
                    if (BrowserDetect.OS == "Windows") {
                      parent.Ts.Services.Settings.ReadUserSetting('ReadScreenRecordingSafariInWindowsInfo', 0, function (alreadyReadInfo) {
                        if (alreadyReadInfo == 0) {
                          $(".pAllowPluginsToRunInstructions").html("\
This browser in Windows usually fails to detect Java preventing the recorder to start. Read \
<a href='http://stackoverflow.com/questions/11235578/when-viewing-an-applet-why-does-safari-for-windows-display-java-is-unavailable' target='_blank' class='ui-state-default ts-link'>this</a> \
for more information or use an alternate browser like Firefox or Internet Explorer.");
                          $('.divScreenRecorderMessages').show();
                        }
                      });
                    }
                    else {
                      parent.Ts.Services.Settings.ReadUserSetting('ReadScreenRecordingSafariInfo', 0, function (alreadyReadInfo) {
                        if (alreadyReadInfo == 0) {
                          $(".pAllowPluginsToRunInstructions").html("\
The following steps will refresh your browser<br><br> \
1. Allow the screen recorder Java plugins to run on your browser by clicking on the Trust button at the top of the page: <br>\
<img src='../Images/icons/SafariInMacPluginDialog.png' alt='plugin dialog' width='30%' style='margin-top: 10px'><br><br> \
2. Navigate to Safari > Preferences > Security > Internet Plugins - Website Settings > Java and change the "+parent.Ts.System.AppDomain+" setting to Run in Unsafe Mode and click on the Trust button: <br>\
<img src='../Images/icons/SafariInMacUnsafeModeDialog.png' alt='plugin dialog' width='30%' style='margin-top: 10px'>");
                          $('.divScreenRecorderMessages').show();
                        }
                      });
                    }

                    break;
                  default:
                    parent.Ts.Services.Settings.ReadUserSetting('ReadScreenRecordingInfo', 0, function (alreadyReadInfo) {
                      if (alreadyReadInfo == 0) {
                        $(".pAllowPluginsToRunInstructions").html("Please verify java is supported and allowed to run in your browser.");
                        $('.divScreenRecorderMessages').show();
                      }
                    });
                }

                //if (deployJava.versionCheck("1.8.0_45+")) {
                var applet = document.createElement("applet");
                applet.id = "recorder";
                applet.archive = "Launch.jar"
                applet.code = "com.bixly.pastevid.driver.Launch";
                applet.width = 200;
                applet.height = 150;
                var orgId = parent.Ts.System.Organization.OrganizationID;
                var param1 = document.createElement("param");
                param1.name = "jnlp_href";
                param1.value = "launch.jnlp";
                applet.appendChild(param1);
                var param2 = document.createElement("param");
                param2.name = "orgId";
                param2.value = orgId;
                applet.appendChild(param2);
                var param3 = document.createElement("param");
                param3.name = "permissions";
                param3.value = 'all-permissions';
                applet.appendChild(param3);

                $('.fa-circle').removeClass("fa-circle").addClass("fa-circle-o-notch fa-spin");
                document.getElementsByTagName("body")[0].appendChild(applet);

                //}
                //else {
                //  userInput = confirm(
                //          "You need the latest Java(TM) Runtime Environment.\n" +
                //          "Please restart your computer after updating.\n" +
                //          "Would you like to update now?");
                //  if (userInput == true) {
                //    window.open("http://java.com/en/download", '_blank');
                //  }
                //}
              }
            }
          });
        }
      }
    , oninit: init
    };
    $(element).tinymce(editorOptions);
  });
};

var onScreenRecordStart = function () {
  $('.fa-circle-o-notch').removeClass("fa-circle-o-notch fa-spin").addClass("fa-circle");
  switch (BrowserDetect.browser) {
    case "Chrome":
      parent.Ts.Services.Settings.WriteUserSetting('ReadScreenRecordingChromeInfo', 1);
      break;
    case "Firefox":
      parent.Ts.Services.Settings.WriteUserSetting('ReadScreenRecordingFirefoxInfo', 1);
      break;
    case "Explorer":
      parent.Ts.Services.Settings.WriteUserSetting('ReadScreenRecordingExplorerInfo', 1);
      break;
    case "Safari":
      if (BrowserDetect.OS == "Windows") {
        parent.Ts.Services.Settings.WriteUserSetting('ReadScreenRecordingSafariInWindowsInfo', 1);
      }
      else {
        parent.Services.Settings.WriteUserSetting('ReadScreenRecordingSafariInfo', 1);
      }
      break;
    default:
      parent.Ts.Services.Settings.WriteUserSetting('ReadScreenRecordingInfo', 1);
  }
  $('.divScreenRecorderMessages').hide();
};

var onScreenRecordComplete = function (url) {
  $('#recorder').remove();
  if (url) {
    var link = '<a href="' + url + '" target="_blank">Click here to view screen recording video.</a>';
    var html = '<div class="video_holder"><video style="width: 640px; height: 360px;" controls="controls"><source src="' + url + '" type="video/mp4" />' + link + '</video></div>'
    var ed = tinyMCE.activeEditor;
    ed.selection.setContent(html);
    ed.execCommand('mceAutoResize');
    ed.focus();
    parent.Ts.System.logAction('Wiki - Screen Recorded');
  }
  else {
    parent.Ts.System.logAction('Wiki - Screen Record Cancelled');
  }
};

function GetTinyMCEFontSize(fontSize) {
  var result = '';
  switch (fontSize) {
    case 1:
      result = "8px";
      break;
    case 2:
      result = "10px";
      break;
    case 3:
      result = "12px";
      break;
    case 4:
      result = "14px";
      break;
    case 5:
      result = "18px";
      break;
    case 6:
      result = "24px";
      break;
    case 7:
      result = "36px";
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
};