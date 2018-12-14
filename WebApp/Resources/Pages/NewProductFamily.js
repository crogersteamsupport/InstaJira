/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />


$(document).ready(function () {
    var _organizatinID = -1;
    var _isAdmin = top.Ts.System.User.IsSystemAdmin && (_organizatinID != top.Ts.System.User.OrganizationID);

    $('body').layout({
        defaults: {
            spacing_open: 0,
            closable: false
        },
        north: {
            spacing_open: 1,
            size: 100
        },
        center: {
            maskContents: true
        }
    });

    var defaultTab = top.Ts.Utils.getQueryValue("open", window);
    var defaultProductFamily = top.Ts.Utils.getQueryValue("productFamilyID", window);

    $(".maincontainer").on("keypress", "input", (function (evt) {
        //Deterime where our character code is coming from within the event
        var charCode = evt.charCode || evt.keyCode;
        if (charCode == 13) { //Enter key's keycode
            return false;
        }
    }));

    //if (defaultTab) {
    //    $('#productFamilyTabs a:first').tab('show');
    //    $('#productFamilyTabs a:last').tab('show');
    //    $('#productFamilyTabs a:first').hide();
    //    $('#productFamiliesList').hide();
    //}
    //else {
        $('#productFamilyTabs a:first').tab('show');
    //    $('#productFamilyTabs a:last').hide();
    //}

    initEditor($('#inputDescription'), function (ed) {
        $('#inputDescription').tinymce().focus();
    });

    LoadCustomControls();
    function LoadCustomControls() {
      top.Ts.Services.Assets.LoadCustomControls(top.Ts.ReferenceTypes.ProductFamilies, function (html) {
        if (html.length < 31) {
          $('#productFamilyCustomInfoBox').hide();
        }
        else {
          $('#productFamilyCustomInfo').append(html);
          $('.customField:visible').each(function () {
            var maskValue = $(this).attr("placeholder");
            if (maskValue) {
              $(this).mask(maskValue);
            }
          });
          $('.datetimepicker').datetimepicker({});
        }
      });
    }

    $('#productFamilySaveBtn').click(function (e) {
        e.preventDefault();
        e.stopPropagation();

        var isValid = $("#productFamilyForm").valid();

        if ($("#inputName").val().length < 1) {
            alert("Please enter a name");
            return;
        }

        if (isValid) {
            var productFamilyInfo = new Object();
            top.Ts.System.logAction('New Product Line Page - Added New Product Line');
            productFamilyInfo.Name = $("#inputName").val();
            productFamilyInfo.Description = $("#Description").val();

            productFamilyInfo.Fields = new Array();
            $('.customField:visible').each(function () {
              var field = new Object();
              field.CustomFieldID = $(this).attr("id");
              switch ($(this).attr("type")) {
                case "checkbox":
                  field.Value = $(this).prop('checked');
                  break;
                case "date":
                  field.Value = $(this).val() == "" ? null : top.Ts.Utils.getMsDate($(this).val());
                  break;
                case "time":
                  field.Value = $(this).val() == "" ? null : top.Ts.Utils.getMsDate("1/1/1900 " + $(this).val());
                  break;
                case "datetime":
                  field.Value = $(this).val() == "" ? null : top.Ts.Utils.getMsDate($(this).val());
                  break;
                default:
                  field.Value = $(this).val();
              }
              productFamilyInfo.Fields[productFamilyInfo.Fields.length] = field;
            });

            top.Ts.Services.Products.SaveProductFamily(top.JSON.stringify(productFamilyInfo), function (f) {
                top.Ts.MainPage.openNewProductFamily(f);
                top.Ts.MainPage.closenewProductFamilyTab();
            }, function () {
                alert('There was an error saving this product line.  Please try again.');
            });
        }
    });

    $('#productFamilyCancelBtn').click(function (e) {
        top.Ts.MainPage.closenewProductFamilyTab();
    });

});

var initEditor = function (element, init) {
    top.Ts.Settings.System.read('EnableScreenR', 'True', function (enableScreenR) {
        var editorOptions = {
            plugins: "autoresize paste link code textcolor",
            toolbar1: "link unlink | undo redo removeformat | cut copy paste pastetext | code | outdent indent | bullist numlist",
            toolbar2: "alignleft aligncenter alignright alignjustify | forecolor backcolor | fontselect fontsizeselect | bold italic underline strikethrough blockquote",
            statusbar: false,
            gecko_spellcheck: true,
            extended_valid_elements: "a[accesskey|charset|class|coords|dir<ltr?rtl|href|hreflang|id|lang|name|onblur|onclick|ondblclick|onfocus|onkeydown|onkeypress|onkeyup|onmousedown|onmousemove|onmouseout|onmouseover|onmouseup|rel|rev|shape<circle?default?poly?rect|style|tabindex|title|target|type],script[charset|defer|language|src|type]",
            content_css: "../Css/jquery-ui-latest.custom.css,../Css/editor.css",
            body_class: "ui-widget ui-widget-content",
            branding: false,
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