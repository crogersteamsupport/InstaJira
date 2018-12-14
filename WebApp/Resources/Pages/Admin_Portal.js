/// <reference path="ts/ts.js" />
/// <reference path="ts/parent.parent.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

var adminPortal = null;
$(document).ready(function () {
    adminPortal = new AdminPortal();
    adminPortal.refresh();
});

function onShow() {
    adminPortal.refresh();
};


AdminPortal = function () {
    $('#btnRefresh')
        .click(function (e) {
            e.preventDefault();
            window.location = window.location;
        })
        .toggle(window.location.hostname.indexOf('127.0.0.1') > -1);

    if (parent.parent.Ts.System.Organization.ProductType == parent.parent.Ts.ProductType.Express) {
        $('#tab-community').remove();
        $('#tabs-community').hide();
        $('#tab-advanced').remove();
        $('#tabs-advanced').hide();
    }

    if (new Date(parent.parent.Ts.System.Organization.DateCreated) > new Date("4/21/2017")) {
        $('#tab-general').remove();
        $('#portal-seperator').remove();
        $('#tabs-general').hide();
        $('#tab-advanced').remove();
        $('#tabs-advanced').hide();
        $('#tab-basic').remove();
        $('#tabs-basic').hide();
    }


    $(function () {
        $(".slider").slider({
            range: "min",
            value: 2,
            min: 0,
            max: 10,
            slide: function (event, ui) {
                $(this).next().text("Overall Weight: " + (ui.value * 10) + "%");
            },
            stop: function (event, ui) {
                var total = 0;
                var result;

                $('.slider').each(function () {
                    total = total + $(this).slider("value");
                });

                if (total == 10) {
                    result = "100%";
                    $('#cdi-total').removeClass('red');
                    $('#recalculate-cdi').removeAttr("disabled");
                    $('.portal-save-panel').show();
                }

                if (total > 10) {
                    result = "is greater than 100%, please reconfigure your weights";
                    $('#cdi-total').addClass('red');
                    $('#recalculate-cdi').attr("disabled", "disabled");
                    $('.portal-save-panel').hide();
                }

                if (total < 10) {
                    result = "is less than 100%, please reconfigure your weights";
                    $('#cdi-total').addClass('red');
                    $('#recalculate-cdi').attr("disabled", "disabled");
                    $('.portal-save-panel').hide();
                }

                $('#cdi-total').text("Total Weight: " + result);

            }
        });

        $("#cdi-green").slider({
            range: "min",
            value: 70,
            min: 0,
            max: 100,
            slide: function (event, ui) {
                $(this).next().text("Upper Limit: " + (ui.value));
            },
            stop: function (event, ui) {
                if (ui.value > $("#cdi-yellow").slider('value')) {
                    $(this).next().addClass("red");
                    $('.portal-save-panel').hide();
                }
                else {
                    $("#cdi-yellow").next().removeClass("red");
                    $('.portal-save-panel').show();
                }
            }
        });

        $("#cdi-yellow").slider({
            range: "min",
            value: 85,
            min: 0,
            max: 100,
            slide: function (event, ui) {
                $(this).next().text("Upper Limit: " + (ui.value));
            },
            stop: function (event, ui) {
                if (ui.value < $("#cdi-green").slider("value")) {
                    $(this).next().addClass("red");
                    $('.portal-save-panel').hide();
                }
                else {
                    $("#cdi-green").next().removeClass("red");
                    $(this).next().removeClass("red");
                    $('.portal-save-panel').show();
                }
            }
        });
    });


    //$('a').addClass('ui-state-default ts-link');
    $('button').button();
    $('#tabs').tabs().find('.ui-tabs-panel').addClass('ui-corner-all');
    $('.loading-section').hide().next().show();

    $('.tabs-modifiable input.text, .tabs-modifiable input.checkbox, .tabs-modifiable textarea').click(function (e) {
        $('.portal-save-panel').show();
    });

    $('.com-cat-properties input.text, .com-cat-properties input.checkbox, .com-cat-properties textarea').click(function (e) {
        $('.com-cat-save-panel').show();
    });

    $('.kb-cat-properties input.text, .kb-cat-properties input.checkbox, .kb-cat-properties textarea').click(function (e) {
        $('.kb-cat-save-panel').show();
    });

    $('.dropdownChange').change(function(e)
    {
        $('.portal-save-panel').show();
    })

    var enablingCommunity = false;
    $('.com-enable').click(function (e) {
        e.preventDefault();
        if (enablingCommunity) return;
        enablingCommunity = true;
        parent.parent.Ts.Services.Organizations.UpdateUseCommunity($(this).text().indexOf('Enable') > -1, function (result) {
            if (result != null) {
                parent.parent.Ts.System.logAction('Admin Portal - Community Toggled');
                $('.com-enable').button('option', 'label', (result == true ? "Disable Community" : "Enable Community"));
            }
            enablingCommunity = false;
        });
    });

    $('.portal-save-panel').hide();
    $('.portal-save').click(function (e) { saveValues(_portalOption); });
    $('.portal-cancel').click(function (e) {
        e.preventDefault();
        $('.portal-save-panel').hide();
        getData();
    });


    loadCombos();

    function loadCombos() {
        var groups = parent.parent.Ts.Cache.getGroups();
 
        $('<option>').attr('value', -1).text('Unassigned').data('o', null).appendTo('#portal_req_group');
        for (var i = 0; i < groups.length; i++) {
            $('<option>').attr('value', groups[i].GroupID).text(groups[i].Name).data('o', groups[i]).appendTo('#portal_req_group');
        }
        $('#portal_req_group').combobox({ selected: function (e, ui) { $('.portal-save-panel').show(); } });

        $('<option>').attr('value', -1).text('Unassigned').data('o', null).appendTo('#com_cat_group');
        for (var i = 0; i < groups.length; i++) {
            $('<option>').attr('value', groups[i].GroupID).text(groups[i].Name).data('o', groups[i]).appendTo('#com_cat_group');
        }
        $('#com_cat_group').combobox({ selected: function (e, ui) { $('.com-cat-save-panel').show(); } });

        var ticketTypes = parent.parent.Ts.Cache.getTicketTypes();
        //$('<option>').attr('value', -1).text('Unassigned').data('o', null).appendTo('#com_cat_tickettype');
        for (var i = 0; i < ticketTypes.length; i++) {
            $('<option>').attr('value', ticketTypes[i].TicketTypeID).text(ticketTypes[i].Name).data('o', ticketTypes[i]).appendTo('#com_cat_tickettype');
        }
        $('#com_cat_tickettype').combobox({ selected: function (e, ui) { $('.com-cat-save-panel').show(); } });

        $('<option>').attr('value', -1).text('Unassigned').data('o', null).appendTo('#portal_req_tickettype');
        for (var i = 0; i < ticketTypes.length; i++) {
            $('<option>').attr('value', ticketTypes[i].TicketTypeID).text(ticketTypes[i].Name).data('o', ticketTypes[i]).appendTo('#portal_req_tickettype');
        }
        $('#portal_req_tickettype').combobox({ selected: function (e, ui) { $('.portal-save-panel').show(); } });

        //portal_req_tickettype
        var products = parent.parent.Ts.Cache.getProducts();
        $('<option>').attr('value', -1).text('Unassigned').data('o', null).appendTo('#com_cat_product');
        for (var i = 0; i < products.length; i++) {
            $('<option>').attr('value', products[i].ProductID).text(products[i].Name).data('o', products[i]).appendTo('#com_cat_product');
        }
        $('#com_cat_product').combobox({ selected: function (e, ui) { $('.com-cat-save-panel').show(); } });

        if (parent.parent.Ts.System.Organization.UseProductFamilies == true) {
            $('<option>').attr('value', -1).text('Unassigned').data('o', null).appendTo('#kb_cat_productfamily');
            $('<option>').attr('value', -1).text('Unassigned').data('o', null).appendTo('#com_cat_productfamily');
            getProductFamilies(function (productFamilies) {
                if (productFamilies) {
                    for (var i = 0; i < productFamilies.length; i++) {
                        $('<option>').attr('value', productFamilies[i].ProductFamilyID).text(productFamilies[i].Name).data('o', productFamilies[i]).appendTo('#kb_cat_productfamily');
                        $('<option>').attr('value', productFamilies[i].ProductFamilyID).text(productFamilies[i].Name).data('o', productFamilies[i]).appendTo('#com_cat_productfamily');
                    }
                }
                $('#kb_cat_productfamily').combobox({ selected: function (e, ui) { $('.kb-cat-save-panel').show(); } });
                $('#com_cat_productfamily').combobox({ selected: function (e, ui) { $('.com-cat-save-panel').show(); } });
            });
        }
    }
    $('#portal_theme').combobox({ selected: function (e, ui) { $('.portal-save-panel').show(); } });

    var _portalOption = null;
    var organization = null;

    function getData() {
        parent.parent.Ts.Services.Organizations.GetOrganization(parent.parent.Ts.System.Organization.OrganizationID, function (org) {
            organization = org;
            parent.parent.Ts.Services.Organizations.GetPortalOption(organization.OrganizationID, function (po) {
                parent.parent.Ts.Settings.clearCache();
                parent.parent.Ts.Settings.Organization.read('ExternalPortalLink', '', function (externalLink) {
                    _portalOption = po;
                    loadValues(organization, _portalOption, externalLink);
                    loadHubOptions();
                });
            });
        });
    }

    getData();

    function getProductFamilies(callback) {
        parent.parent.Ts.Services.Products.GetProductFamilies(function (result) {
            callback(result);
        });
    }

    function loadValues(organization, portalOption, externalLink) {
        $('#portal_name').val(portalOption.PortalName);
        $('#portal_deflection').prop('checked', portalOption.DeflectionEnabled == null ? false : portalOption.DeflectionEnabled);
        $('#portal_req_group').combobox('setValue', portalOption.RequestGroup);
        $('#portal_req_tickettype').combobox('setValue', portalOption.RequestType);
        $('#portal_show_grouplist').prop('checked', portalOption.DisplayGroups == null ? false : portalOption.DisplayGroups);
        $('#portal_show_product').prop('checked', portalOption.DisplayProducts == null ? false : portalOption.DisplayProducts);
        $('#portal_show_version').prop('checked', portalOption.DisplayProductVersion == null ? false : portalOption.DisplayProductVersion);
        $('#portal_restrict_version').prop('checked', portalOption.RestrictProductVersion == null ? false : portalOption.RestrictProductVersion);
        $('#portal_show_closed_button').prop('checked', portalOption.HideCloseButton == null ? false : !portalOption.HideCloseButton);
        $('#portal_severity_editing').prop('checked', portalOption.AllowSeverityEditing == null ? false : portalOption.AllowSeverityEditing);
        $('#portal_name_editing').prop('checked', portalOption.AllowNameEditing == null ? false : portalOption.AllowNameEditing);
        $('#portal_theme').combobox('setValue', portalOption.Theme);
        $('#portal_header').val(portalOption.PortalHTMLHeader);
        $('#portal_footer').val(portalOption.PortalHTMLFooter);
        $('#portal_screen_rec').prop('checked', portalOption.EnableScreenr);
        $('#portal_video_rec').prop('checked', portalOption.EnableVideoRecording);
        $('#portal_twocolumn').prop('checked', portalOption.TwoColumnFields);
        $('#portal_poweredby').prop('checked', portalOption.DisplayFooter);

        $('#portal_adv_url').text(parent.parent.Ts.System.PortalDomain + '/' + portalOption.PortalName).attr('href', parent.parent.Ts.System.PortalDomain + '/' + portalOption.PortalName);
        $('#portal_landing_enabled').prop('checked', portalOption.DisplayLandingPage);
        $('#portal_landing_html').val(portalOption.LandingPageHtml);
        $('#portal_external_link').val(externalLink);
        $('#portal_adv_width').val(portalOption.AdvPortalWidth);
        $('#portal_show_user').prop('checked', portalOption.HideUserAssignedTo == null ? true : !portalOption.HideUserAssignedTo);
        $('#portal_show_group').prop('checked', portalOption.HideGroupAssignedTo == null ? true : !portalOption.HideGroupAssignedTo);
        $('#portal_adv_kb').prop('checked', portalOption.DisplayAdvKB == true);
        $('#portal_display_products').prop('checked', portalOption.DisplayAdvProducts == true);
        $('#portal_display_settings').prop('checked', portalOption.DisplaySettings == true);
        $('#portal_display_logout').prop('checked', portalOption.DisplayLogout == true);
        $('#portal_adv_wiki').prop('checked', portalOption.DisplayAdvArticles == true);
        $('#portal_adv_enable_toc').prop('checked', portalOption.DisplayTandC == true);
        $('#portal_adv_enabled_sa_expiration').prop('checked', portalOption.EnableSaExpiration == true);
        $('#portal_adv_toc').val(portalOption.TermsAndConditions);
        $('#portal_adv_autoregister').prop('checked', portalOption.AutoRegister == true);
        $('#portal_adv_showrequestaccess').prop('checked', portalOption.RequestAccess == true);
        $('#portal_adv_honorexpiration').prop('checked', portalOption.HonorSupportExpiration == null ? false : portalOption.HonorSupportExpiration);



        $('#portal_captcha').prop('checked', portalOption.UseRecaptcha == null ? false : portalOption.UseRecaptcha);
        $('#portal_basic_header').val(portalOption.BasicPortalDirections);
        $('#portal_basic_width').val(portalOption.BasicPortalColumnWidth);
        var domain = parent.parent.Ts.System.Domain;
        function buildDomain(zone) { return 'https://' + zone + '.' + domain + '/'; }
        $('#portal_ticket_url').text(buildDomain('ticket') + portalOption.PortalName).attr('href', buildDomain('ticket') + portalOption.PortalName);
        $('#portal_use_company').prop('checked', portalOption.UseCompanyInBasic == null ? false : portalOption.UseCompanyInBasic);
        $('#portal_company_required').prop('checked', portalOption.CompanyRequiredInBasic == null ? false : portalOption.CompanyRequiredInBasic);
        $('#portal_kb_url').text(buildDomain('kb') + portalOption.PortalName).attr('href', buildDomain('kb') + portalOption.PortalName);
        $('#portal_allow_kb').prop('checked', portalOption.KBAccess == null ? false : portalOption.KBAccess);
        $('#portal_allow_mytickets').prop('checked', !portalOption.DisablePublicMyTickets);
        $('#portal_wiki_url').text(buildDomain('articles') + portalOption.PortalName).attr('href', buildDomain('articles') + portalOption.PortalName);
        $('#portal_allow_wiki').prop('checked', organization.IsPublicArticles);
        $('#portal_display_fb_article').prop('checked', portalOption.DisplayFbArticles);
        $('#portal_display_fb_kb').prop('checked', portalOption.DisplayFbKB);
        $('#portal_landing_url').text(buildDomain('publicportal') + portalOption.PortalName).attr('href', buildDomain('publicportal') + portalOption.PortalName);
        $('#portal_public_landing_body').val(portalOption.PublicLandingPageBody);
        $('#portal_public_landing_header').val(portalOption.PublicLandingPageHeader);
        $('#portal_com_enabled').prop('checked', portalOption.DisplayForum == null ? false : portalOption.DisplayForum);
        $('.com-enable').button('option', 'label', (portalOption.DisplayForum == true ? "Disable Community" : "Enable Community"));
        $('#agentrating-enabled').prop('checked', organization.AgentRating);
        $('#portal_forwarding').prop('checked', portalOption.PortalForward == '' ? false : true);
        $('#portal_foward_location').attr('disabled', portalOption.PortalForward == '' ? true : false)

    }

    function saveValues(portalOption) {
        portalOption.PortalNameEditing = $('#portal_name_editing').val();
        portalOption.DeflectionEnabled = $('#portal_deflection').prop('checked');
        portalOption.DisplayGroups = $('#portal_show_grouplist').prop('checked');
        portalOption.DisplayProducts = $('#portal_show_product').prop('checked');
        portalOption.DisplayProductVersion = $('#portal_show_version').prop('checked');
        portalOption.RestrictProductVersion = $('#portal_restrict_version').prop('checked');
        portalOption.HideCloseButton = !$('#portal_show_closed_button').prop('checked');
        portalOption.AllowSeverityEditing = $('#portal_severity_editing').prop('checked');
        portalOption.AllowNameEditing = $('#portal_name_editing').prop('checked');
        portalOption.Theme = $('#portal_theme').val();
        portalOption.RequestGroup = $('#portal_req_group').val();
        portalOption.RequestType = $('#portal_req_tickettype').val();
        portalOption.PortalHTMLHeader = $('#portal_header').val();
        portalOption.PortalHTMLFooter = $('#portal_footer').val();
        portalOption.EnableScreenr = $('#portal_screen_rec').prop('checked');
        portalOption.EnableVideoRecording = $('#portal_video_rec').prop('checked');
        portalOption.TwoColumnFields = $('#portal_twocolumn').prop('checked');
        portalOption.DisplayFooter = $('#portal_poweredby').prop('checked');


        portalOption.DisplayLandingPage = $('#portal_landing_enabled').prop('checked');
        portalOption.LandingPageHtml = $('#portal_landing_html').val();
        //$('#portal_external_link').val();
        portalOption.AdvPortalWidth = $('#portal_adv_width').val();
        portalOption.HideUserAssignedTo = !$('#portal_show_user').prop('checked');
        portalOption.HideGroupAssignedTo = !$('#portal_show_group').prop('checked');
        portalOption.DisplayAdvKB = $('#portal_adv_kb').prop('checked');
        portalOption.DisplayAdvProducts = $('#portal_display_products').prop('checked');
        portalOption.DisplaySettings = $('#portal_display_settings').prop('checked');
        portalOption.DisplayLogout = $('#portal_display_logout').prop('checked');
        portalOption.DisplayAdvArticles = $('#portal_adv_wiki').prop('checked');
        portalOption.DisplayTandC = $('#portal_adv_enable_toc').prop('checked');
        portalOption.EnableSaExpiration = $('#portal_adv_enabled_sa_expiration').prop('checked');
        portalOption.AutoRegister = $('#portal_adv_autoregister').prop('checked');
        portalOption.RequestAccess = $('#portal_adv_showrequestaccess').prop('checked');
        portalOption.HonorSupportExpiration = $('#portal_adv_honorexpiration').prop('checked');

        portalOption.TermsAndConditions = $('#portal_adv_toc').val();

        portalOption.UseRecaptcha = $('#portal_captcha').prop('checked');
        portalOption.BasicPortalDirections = $('#portal_basic_header').val();
        portalOption.BasicPortalColumnWidth = $('#portal_basic_width').val();
        portalOption.UseCompanyInBasic = $('#portal_use_company').prop('checked');
        portalOption.CompanyRequiredInBasic = $('#portal_company_required').prop('checked');
        portalOption.KBAccess = $('#portal_allow_kb').prop('checked');
        portalOption.DisablePublicMyTickets = !$('#portal_allow_mytickets').prop('checked');
        //organization.IsPublicArticles = $('#portal_allow_wiki').prop('checked');
        portalOption.PublicLandingPageBody = $('#portal_public_landing_body').val();
        portalOption.PublicLandingPageHeader = $('#portal_public_landing_header').val();
        portalOption.DisplayForum = $('#portal_com_enabled').prop('checked');
        portalOption.DisplayFbArticles = $('#portal_display_fb_article').prop('checked');
        portalOption.DisplayFbKB = $('#portal_display_fb_kb').prop('checked');

        portalOption.PortalForward = $('#portal_forwarding').prop('checked') ? $('#portal_foward_location').val() : '';
        // show some indicator


        //var _agentratingOption = new parent.parent.TeamSupport.Data.AgentRatingsOptionProxy();;
        //_agentratingOption.PositiveRatingText = $('#agentrating-positive').val();
        //_agentratingOption.NeutralRatingText = $('#agentrating-neutral').val();
        //_agentratingOption.NegativeRatingText = $('#agentrating-negative').val();
        //_agentratingOption.RedirectURL = $('#agentrating-redirecturl').val();
        //_agentratingOption.ExternalPageLink = $('#agentrating-externalurl').val();

        parent.parent.Ts.Services.Organizations.SetPortalOption(portalOption, $('#portal_external_link').val(), $('#portal_allow_wiki').prop('checked'), function (result) {
            if (result != null) {
                alert(result);
            }
            else {
                parent.parent.Ts.System.logAction('Admin Portal - Portal Settings Saved');
                $('.portal-save-panel').hide();
            }
        });




    }

    getForumCats()
    function getForumCats() {
        parent.parent.Ts.Services.Admin.GetForumCategories(function (cats) {
            $('.com-cat-list').empty();
            for (var i = 0; i < cats.length; i++) {
                var cat = cats[i].Category;
                createCategory(cat);
                var subs = cats[i].Subcategories;
                for (var j = 0; j < subs.length; j++) {
                    var sub = subs[j];
                    createSubCategory(sub);
                }
            }
            if ($('.com-cat').length < 1) {
                $('.com-no-cat').show().next().hide();
            }
            else {
                $('.com-no-cat').hide().next().show();
                $('.com-cat:first').click();
            }

        });
    }

    function createCategory(cat) {
        var container = $('<div>')
            .addClass('com-cats');

        var el = $('<div>')
            .addClass('com-cat com-cat-main ui-corner-all com-cat-' + cat.CategoryID)
            .text(cat.CategoryName)
            .data('cat', cat)
            .appendTo(container);

        $('<a>')
            .addClass('ui-state-default ts-link com-new-sub')
            .attr('href', '#')
            .text('Add a new subcategory')
            .appendTo($('<div>').appendTo(container));

        $('.com-cat-list').append(container);

        setSortable();
        return el;
    }

    function createSubCategory(cat) {
        var container = $('.com-cat-list').find('.com-cat-' + cat.ParentID);
        if (container.length < 1) return;
        container = container.parent().find('.com-new-sub');

        var el = $('<div>')
            .addClass('com-cat com-cat-sub ui-corner-all com-cat-' + cat.CategoryID)
            .text(cat.CategoryName)
            .data('cat', cat)
            .insertBefore(container);

        setSortable();
        return el;
    }

    $('.com-cat-list').delegate('.com-cat', 'mouseover mouseout', function (e) {
        $('.com-cat-list .com-cat').removeClass('ui-state-hover');
        if (e.type == 'mouseover') { $(this).addClass('ui-state-hover'); }
    });

    $('.com-cat-list').delegate('.com-cat', 'click', function (e) {
        $('.com-cat-list .com-cat').removeClass('ui-state-active');
        $(this).addClass('ui-state-active');

        var cat = $(this).data('cat');
        $('#com_cat_name').val(cat.CategoryName);
        $('#com_cat_description').val(cat.CategoryDesc);
        $('#com_cat_tickettype').combobox('setValue', cat.TicketType == null ? -1 : cat.TicketType);
        $('#com_cat_group').combobox('setValue', cat.GroupID == null ? -1 : cat.GroupID);
        $('#com_cat_product').combobox('setValue', cat.ProductID == null ? -1 : cat.ProductID);

        if (parent.parent.Ts.System.Organization.UseProductFamilies) {
            $('#com_cat_productfamily').combobox('setValue', cat.ProductFamilyID == null ? -1 : cat.ProductFamilyID);
        }

        if (cat.ParentID < 0) {
            $('.com-delete-cat').text('Delete this category and all its subcategories');
            if (parent.parent.Ts.System.Organization.UseProductFamilies == true) {
                $('.com-cat-productfamily-only').show();
            }
            $('.com-sub-only').hide();
        }
        else {
            $('.com-delete-cat').text('Delete this subcategory');
            if (parent.parent.Ts.System.Organization.UseProductFamilies == true) {
                $('.com-cat-productfamily-only').hide();
            }
            $('.com-sub-only').show();
        }
    });

    $('.com-delete-cat').click(function (e) {
        e.preventDefault();
        var el = $('.com-cat-list .ui-state-active.com-cat');
        if (el.length < 1) return;
        var cat = el.data('cat');
        var isMain = el.hasClass('com-cat-main') == true;

        if (isMain == true) {
            if (!confirm("Are you sure you would like to delete the category '" + cat.CategoryName + "' and ALL its subcategories?")) return;
        }
        else {
            if (!confirm("Are you sure you would like to delete subcategory '" + cat.CategoryName + "'?")) return;
        }

        parent.parent.Ts.System.logAction('Admin Portal - Category Deleted');
        parent.parent.Ts.Services.Admin.DeleteForumCategory(cat.CategoryID, function (result) {
            if (result == false) return;

            if (isMain == true) {
                el.parent().remove();
            }
            else {
                el.remove();
            }


            if ($('.com-cat').length < 1) {
                $('.com-no-cat').show().next().hide();
            }
            else {
                $('.com-no-cat').hide().next().show();
                $('.com-cat:first').click();
            }


        });
    });

    $('.com-save-cat').click(function (e) {
        e.preventDefault();
        var el = $('.com-cat-list .ui-state-active.com-cat');
        if (el.length < 1) return;
        var cat = el.data('cat');
        parent.parent.Ts.System.logAction('Admin Portal - Category Saved');
        parent.parent.Ts.Services.Admin.UpdateForumCategory(
            cat.CategoryID,
            $('#com_cat_name').val(),
            $('#com_cat_description').val(),
            ($('#com_cat_tickettype').val() < 0 ? null : $('#com_cat_tickettype').val()),
            ($('#com_cat_group').val() < 0 ? null : $('#com_cat_group').val()),
            ($('#com_cat_product').val() < 0 ? null : $('#com_cat_product').val()),
            ($('#com_cat_productfamily').val() < 0 ? null : $('#com_cat_productfamily').val()),
            function (result) {
                if (result == null) return;
                $('.com-cat-save-panel').hide();
                updateCat(result);
            });
    });

    function updateCat(cat) {
        var el = $('.com-cat-list').find('.com-cat-' + cat.CategoryID);
        if (el.length < 1) return;
        el.data('cat', cat);
        el.text(cat.CategoryName);
    }

    $('.com-new-cat').click(function (e) {
        e.preventDefault();
        parent.parent.Ts.Services.Admin.AddForumCategory(null, function (result) {
            if (result != null) {

                var el = createCategory(result);
                el.click();
                $('.com-no-cat').hide().next().show();
                parent.parent.Ts.System.logAction('Admin Portal - Category Created');
            }
        });
    });

    $('.com-cat-list').delegate('.com-new-sub', 'click', function (e) {
        e.preventDefault();
        var parentCat = $(this).closest('.com-cats').find('.com-cat-main').data('cat');
        parent.parent.Ts.Services.Admin.AddForumCategory(parentCat.CategoryID, function (result) {
            if (result != null) {
                var el = createSubCategory(result);
                el.click();
                $('.com-no-cat').hide().next().show();
                $('.com-cat-save-panel').show();
                $('#com_cat_name').focus().select();
            }
        });
    });

    function setSortable() {
        $('.com-cats').sortable({
            items: '.com-cat-sub', connectWith: '.com-cats', placeholder: 'ui-state-highlight com-cat-sub ui-corner-all', update: function (e, ui) {
                savePositions();
            }
        });

        $('.com-cat-list').sortable({
            items: '.com-cats', placeholder: 'ui-state-highlight ui-widget-content ui-corner-all ts-section .com-cats', update: function (e, ui) {
                savePositions();
            }
        });
    }

    function savePositions() {
        var orders = new parent.parent.Array();
        $('.com-cats').each(function () {

            var item = new Object(); // parent.parent.TSWebServices.ForumCategoryOrder();
            item.CategoryIDs = new parent.parent.Array();
            orders[orders.length] = item;

            var cat = $(this).find('.com-cat-main').data('cat');

            item.ParentID = cat.CategoryID;

            $(this).find('.com-cat-sub').each(function () {
                var sub = $(this).data('cat');
                item.CategoryIDs[item.CategoryIDs.length] = sub.CategoryID

            });
        });

        parent.parent.Ts.Services.Admin.UpdateForumCategoryOrder(JSON.stringify(orders));
        parent.parent.Ts.System.logAction('Admin Portal - Category Positions Changed');
    }

    getKnowledgeBaseCats()
    function getKnowledgeBaseCats() {
        parent.parent.Ts.Services.Admin.GetKnowledgeBaseCategories(function (cats) {
            $('.kb-cat-list').empty();
            for (var i = 0; i < cats.length; i++) {
                var cat = cats[i].Category;
                createKnowledgeBaseCategory(cat);
                var subs = cats[i].Subcategories;
                for (var j = 0; j < subs.length; j++) {
                    var sub = subs[j];
                    createKnowledgeBaseSubCategory(sub);
                }
            }
            if ($('.kb-cat').length < 1) {
                $('.kb-no-cat').show().next().hide();
            }
            else {
                $('.kb-no-cat').hide().next().show();
                $('.kb-cat:first').click();
            }

        });
    }

    function createKnowledgeBaseCategory(cat) {
        var container = $('<div>')
            .addClass('kb-cats');

        var el = $('<div>')
            .addClass('kb-cat kb-cat-main ui-corner-all kb-cat-' + cat.CategoryID)
            .text(cat.CategoryName)
            .data('cat', cat)
            .appendTo(container);

        $('<a>')
            .addClass('ui-state-default ts-link kb-new-sub')
            .attr('href', '#')
            .text('Add a new subcategory')
            .appendTo($('<div>').appendTo(container));

        $('.kb-cat-list').append(container);

        setKnowledgeBaseSortable();
        return el;
    }

    function createKnowledgeBaseSubCategory(cat) {
        var container = $('.kb-cat-list').find('.kb-cat-' + cat.ParentID);
        if (container.length < 1) return;
        container = container.parent().find('.kb-new-sub');

        var el = $('<div>')
            .addClass('kb-cat kb-cat-sub ui-corner-all kb-cat-' + cat.CategoryID)
            .text(cat.CategoryName)
            .data('cat', cat)
            .insertBefore(container);

        setKnowledgeBaseSortable();
        return el;
    }

    $('.kb-cat-list').delegate('.kb-cat', 'mouseover mouseout', function (e) {
        $('.kb-cat-list .kb-cat').removeClass('ui-state-hover');
        if (e.type == 'mouseover') { $(this).addClass('ui-state-hover'); }
    });

    $('.kb-cat-list').delegate('.kb-cat', 'click', function (e) {
        $('.kb-cat-list .kb-cat').removeClass('ui-state-active');
        $(this).addClass('ui-state-active');

        var cat = $(this).data('cat');
        $('#kb_cat_name').val(cat.CategoryName);
        $('#kb_cat_description').val(cat.CategoryDesc);
        $('#kb_cat_visible').prop('checked', cat.VisibleOnPortal);
        if (parent.parent.Ts.System.Organization.UseProductFamilies) {
            $('#kb_cat_productfamily').combobox('setValue', cat.ProductFamilyID == null ? -1 : cat.ProductFamilyID)
        }

        if (cat.ParentID < 0) {
            $('.kb-delete-cat').text('Delete this category and all its subcategories');
            $('.kb-sub-only').hide();
            if (parent.parent.Ts.System.Organization.UseProductFamilies == true) {
                $('.kb-cat-productfamily-only').show();
            }
        }
        else {
            $('.kb-delete-cat').text('Delete this subcategory');
            $('.kb-sub-only').show();
            $('.kb-cat-productfamily-only').hide();
        }
    });

    $('.kb-delete-cat').click(function (e) {
        e.preventDefault();
        var el = $('.kb-cat-list .ui-state-active.kb-cat');
        if (el.length < 1) return;
        var cat = el.data('cat');
        var isMain = el.hasClass('kb-cat-main') == true;

        if (isMain == true) {
            if (!confirm("Are you sure you would like to delete the category '" + cat.CategoryName + "' and ALL its subcategories?")) return;
        }
        else {
            if (!confirm("Are you sure you would like to delete subcategory '" + cat.CategoryName + "'?")) return;
        }

        parent.parent.Ts.System.logAction('Admin Portal - KnowledgeBase Category Deleted');
        parent.parent.Ts.Services.Admin.DeleteKnowledgeBaseCategory(cat.CategoryID, function (result) {
            if (result == false) return;

            if (isMain == true) {
                el.parent().remove();
            }
            else {
                el.remove();
            }


            if ($('.kb-cat').length < 1) {
                $('.kb-no-cat').show().next().hide();
            }
            else {
                $('.kb-no-cat').hide().next().show();
                $('kb-cat:first').click();
            }


        });
    });

    $('.kb-save-cat').click(function (e) {
        e.preventDefault();
        var el = $('.kb-cat-list .ui-state-active.kb-cat');
        if (el.length < 1) return;
        var cat = el.data('cat');
        parent.parent.Ts.System.logAction('Admin Portal - KnowledgeBase Category Saved');
        parent.parent.Ts.Services.Admin.UpdateKnowledgeBaseCategory(
            cat.CategoryID,
            $('#kb_cat_name').val(),
            $('#kb_cat_description').val(),
            $('#kb_cat_visible').prop('checked'),
            $('#kb_cat_productfamily').val(),
            function (result) {
                if (result == null) return;
                $('.kb-cat-save-panel').hide();
                updateKnowledgeBaseCat(result);
            });
    });

    function updateKnowledgeBaseCat(cat) {
        var el = $('.kb-cat-list').find('.kb-cat-' + cat.CategoryID);
        if (el.length < 1) return;
        el.data('cat', cat);
        el.text(cat.CategoryName);
    }

    $('.kb-new-cat').click(function (e) {
        e.preventDefault();
        parent.parent.Ts.Services.Admin.AddKnowledgeBaseCategory(null, function (result) {
            if (result != null) {
                var el = createKnowledgeBaseCategory(result);
                el.click();
                $('.kb-no-cat').hide().next().show();
                parent.parent.Ts.System.logAction('Admin Portal - KnowledgeBase Category Created');

            }
        });
    });

    $('.kb-cat-list').delegate('.kb-new-sub', 'click', function (e) {
        e.preventDefault();
        var parentCat = $(this).closest('.kb-cats').find('.kb-cat-main').data('cat');
        parent.parent.Ts.Services.Admin.AddKnowledgeBaseCategory(parentCat.CategoryID, function (result) {
            if (result != null) {
                var el = createKnowledgeBaseSubCategory(result);
                el.click();
                $('.kb-no-cat').hide().next().show();
                $('.kb-cat-save-panel').show();
                $('#kb_cat_name').focus().select();
            }
        });
    });

    function setKnowledgeBaseSortable() {
        $('.kb-cats').sortable({
            items: '.kb-cat-sub', connectWith: '.kb-cats', placeholder: 'ui-state-highlight kb-cat-sub ui-corner-all', update: function (e, ui) {
                saveKnowledgeBasePositions();
            }
        });

        $('.kb-cat-list').sortable({
            items: '.kb-cats', placeholder: 'ui-state-highlight ui-widget-content ui-corner-all ts-section .kb-cats', update: function (e, ui) {
                saveKnowledgeBasePositions();
            }
        });
    }

    function saveKnowledgeBasePositions() {
        var orders = new parent.parent.Array();
        $('.kb-cats').each(function () {

            var item = new Object(); // parent.parent.TSWebServices.ForumCategoryOrder();
            item.CategoryIDs = new parent.parent.Array();
            orders[orders.length] = item;

            var cat = $(this).find('.kb-cat-main').data('cat');

            item.ParentID = cat.CategoryID;

            $(this).find('.kb-cat-sub').each(function () {
                var sub = $(this).data('cat');
                item.CategoryIDs[item.CategoryIDs.length] = sub.CategoryID

            });
        });

        parent.parent.Ts.Services.Admin.UpdateKnowledgeBaseCategoryOrder(JSON.stringify(orders));
        parent.parent.Ts.System.logAction('Admin Portal - KnowledgeBase Category Positions Changed');

    }

    loadGridDropDown();
    function loadGridDropDown() {
        parent.parent.Ts.Services.Search.GetAdvancedSearchOptions(function (advancedSearchOptions) {
            for (var i = 0; i < advancedSearchOptions.Fields.length; i++) {
                $('<option>').attr('value', (advancedSearchOptions.Fields[i].IsCustom ? "c" : "s") + advancedSearchOptions.Fields[i].FieldID).text(advancedSearchOptions.Fields[i].Alias).appendTo('.admin-portal-columns').data('field', advancedSearchOptions.Fields[i]);
            }
        });
        $('.admin-portal-columns').combobox();
    }

    loadGridData();
    function loadGridData() {
        parent.parent.Ts.Services.Organizations.LoadCustomPortalColumns(parent.parent.Ts.System.Organization.OrganizationID, function (columns) {
            for (var col in columns)
                appendCustomPortalColumn(columns[col]);
        });
    }

    function appendCustomPortalColumn(col) {
        var container = $('.sort-container');
        var sort = $('<div>').addClass("sort-item").data("fieldID", col.CustomFieldID ? "c" + col.CustomFieldID : "s" + col.StockFieldID).appendTo(container);
        var title = $('<span>').text(col.FieldText).appendTo(sort);
        var trash = $('<span>').addClass('ts-icon ts-icon-delete fa fa-trash').hide().click(function () {
            if (confirm("Do you want to delete this column?")) {
                parent.parent.Ts.Services.Organizations.RemoveCustomPortalColumn(sort.data("fieldID"));
                sort.remove();
                parent.parent.Ts.System.logAction('Admin Portal - Grid Column Removed');
            }
        }).appendTo(sort);
    }

    $('.add-portal-column').click(function () {
        var isDupe = false;
        $('.sort-item').each(function () {
            if ($(this).data('fieldID') == $('.admin-portal-columns option:selected').val())
                isDupe = true;
        });

        if (!isDupe) {
            var container = $('.sort-container');
            var sort = $('<div>').addClass("sort-item").data("fieldID", $('.admin-portal-columns').val()).appendTo(container);
            var title = $('<span>').text($('.admin-portal-columns option:selected').text()).appendTo(sort);
            var trash = $('<span>').addClass('ts-icon ts-icon-delete fa fa-trash').hide().click(function () {
                if (confirm("Do you want to delete this column?")) {
                    parent.parent.Ts.Services.Organizations.RemoveCustomPortalColumn(sort.data("fieldID"));
                    savePortalColPositions();
                    sort.remove();
                    parent.parent.Ts.System.logAction('Admin Portal - Grid Column Removed');
                }
            }).appendTo(sort);
            parent.parent.Ts.Services.Organizations.AddCustomPortalColumn($('.admin-portal-columns').val(), $('.sort-item').length);
            parent.parent.Ts.System.logAction('Admin Portal - Grid Column Added');
        }
        setPortalColSortable();
        savePortalColPositions();
    });


    setPortalColSortable();
    function setPortalColSortable() {
        $('.sort-container').sortable({
            items: '.sort-item', connectWith: '.sort-container', placeholder: 'ui-state-highlight sort-item ui-corner-all', update: function (e, ui) {
                savePortalColPositions();
            }
        });
    }

    function savePortalColPositions() {
        var items = new parent.parent.Array();
        $('.sort-item').each(function () {
            items[items.length] = $(this).data('fieldID');
        });
        parent.parent.Ts.System.logAction('Admin Portal - Grid Column Order Saved');
        parent.parent.Ts.Services.Organizations.SavePortalColOrder(JSON.stringify(items));
    }


    $('.sort-container').on({
        mouseenter: function () {
            $(this).find('.ts-icon').show();
        },
        mouseleave: function () {
            $(this).find('.ts-icon').hide();
        }
    }, '.sort-item');

    loadAgentRating();
    function loadAgentRating() {

        parent.parent.Ts.Services.Organizations.GetAgentRatingOptions(parent.parent.Ts.System.Organization.OrganizationID, function (o) {
            if (o != null) {
                if (o.PositiveImage)
                    $('#agentrating-positive-img').attr('src', o.PositiveImage);
                if (o.NeutralImage)
                    $('#agentrating-neutral-img').attr('src', o.NeutralImage);
                if (o.NegativeImage)
                    $('#agentrating-negative-img').attr('src', o.NegativeImage);

                if (o.PositiveRatingText != null)
                    $('#agentrating-positive').text(o.PositiveRatingText);
                if (o.NeutralRatingText != null)
                    $('#agentrating-neutral').text(o.NeutralRatingText);
                if (o.NegativeRatingText != null)
                    $('#agentrating-negative').text(o.NegativeRatingText);
                if (o.RedirectURL != null)
                    $('#agentrating-redirecturl').val(o.RedirectURL);
                if (o.ExternalPageLink != null)
                    $('#agentrating-externalurl').val(o.ExternalPageLink);
            }
        });
    }

    loadCDISettings();
    function loadCDISettings() {
        parent.parent.Ts.Services.Organizations.LoadCDISettings(parent.parent.Ts.System.Organization.OrganizationID, function (cdi) {

            if (cdi != null) {
                var ttwvalue = cdi.TotalTicketsWeight == null ? '2' : cdi.TotalTicketsWeight * 10;
                $('#ttw-slider').slider('value', ttwvalue);
                $('#ttw-slider').next().text("Overall Weight: " + (ttwvalue * 10) + "%");

                var last30slider = cdi.Last30Weight == null ? '2' : cdi.Last30Weight * 10;
                $('#last30-slider').slider('value', last30slider);
                $('#last30-slider').next().text("Overall Weight: " + (last30slider * 10) + "%");

                var otwslider = cdi.OpenTicketsWeight == null ? '2' : cdi.OpenTicketsWeight * 10;
                $('#otw-slider').slider('value', otwslider);
                $('#otw-slider').next().text("Overall Weight: " + (otwslider * 10) + "%");

                var avgopenweight = cdi.AvgDaysOpenWeight == null ? '2' : cdi.AvgDaysOpenWeight * 10;
                $('#avgopen-weight').slider('value', avgopenweight);
                $('#avgopen-weight').next().text("Overall Weight: " + (avgopenweight * 10) + "%");

                var avgcloseweight = cdi.AvgDaysToCloseWeight == null ? '2' : cdi.AvgDaysToCloseWeight * 10;
                $('#avgclose-weight').slider('value', avgcloseweight);
                $('#avgclose-weight').next().text("Overall Weight: " + (avgcloseweight * 10) + "%");

                var greenlimit = cdi.AvgDaysToCloseWeight == null ? '70' : cdi.GreenUpperRange;
                $('#cdi-green').slider('value', greenlimit);
                $('#cdi-green').next().text("Upper Limit: " + greenlimit);

                var yellowlimit = cdi.YellowUpperRange == null ? '85' : cdi.YellowUpperRange;
                $('#cdi-yellow').slider('value', yellowlimit);
                $('#cdi-yellow').next().text("Upper Limit: " + yellowlimit);
            }
        });

    }

    var isFilevalid = true;
    $('.positive-file-upload').fileupload({
        namespace: 'custom_attachment',
        dropZone: $('.file-upload'),
        previewMaxWidth: 100,
        previewMaxHeight: 100,
        previewCrop: true,
        add: function (e, data) {
            for (var i = 0; i < data.files.length; i++) {

                if (!(/\.(gif|jpg|jpeg|tiff|png)$/i).test(data.files[i].name)) {
                    alert('Please select a valid image file. (jpg, jpeg, gif, tiff, png)');
                    isFilevalid = false;
                    return;
                }

                var item = $('<li>')
                    .appendTo($('.positive-upload-queue'));

                data.context = item;
                item.data('data', data);

                var bg = $('<div>')
                    .addClass('ts-color-bg-accent')
                    .appendTo(item);

                $('<div>')
                    .text(data.files[i].name + '  (' + parent.parent.Ts.Utils.getSizeString(data.files[i].size) + ')')
                    .addClass('filename')
                    .appendTo(bg);

                $('<span>')
                    .addClass('icon-remove')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('li').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);

                $('<span>')
                    .addClass('icon-remove')
                    .hide()
                    .click(function (e) {
                        e.preventDefault();
                        var data = $(this).closest('li').data('data');
                        data.jqXHR.abort();
                    })
                    .appendTo(bg);

                var progress = $('<div>')
                    .addClass('progress progress-striped active')
                    .hide();

                $('<div>')
                    .addClass('progress-bar')
                    .attr('role', 'progressbar')
                    .appendTo(progress);

                progress.appendTo(bg);
            }

        },
        send: function (e, data) {
            if (data.context && data.dataType && data.dataType.substr(0, 6) === 'iframe') {
                data.context.find('.progress-bar').css('width', '50%');
            }
        },
        fail: function (e, data) {
            if (data.errorThrown === 'abort') return;
            alert('There was an error uploading "' + data.files[0].name + '".');
        },
        progress: function (e, data) {
            data.context.find('.progress-bar').css('width', parseInt(data.loaded / data.total * 100, 10) + '%');
        },
        start: function (e, data) {
            $('.progress').show();
            $('.positive-upload-queue .ui-icon-close').hide();
            $('.positive-upload-queue .ui-icon-cancel').show();
        },
        stop: function (e, data) {
            //data.context.find('.progress-bar').css('width', '100%');
            $('.positive-upload-queue').empty();
        }
    });


    $('.neutral-file-upload').fileupload({
        namespace: 'custom_attachment',
        dropZone: $('.neutral-file-upload'),
        previewMaxWidth: 100,
        previewMaxHeight: 100,
        previewCrop: true,
        add: function (e, data) {
            for (var i = 0; i < data.files.length; i++) {

                if (!(/\.(gif|jpg|jpeg|tiff|png)$/i).test(data.files[i].name)) {
                    alert('Please select a valid image file. (jpg, jpeg, gif, tiff, png)');
                    isFilevalid = false;
                    return;
                }

                var item = $('<li>')
                    .appendTo($('.neutral-upload-queue'));

                data.context = item;
                item.data('data', data);

                var bg = $('<div>')
                    .addClass('ts-color-bg-accent')
                    .appendTo(item);

                $('<div>')
                    .text(data.files[i].name + '  (' + parent.parent.Ts.Utils.getSizeString(data.files[i].size) + ')')
                    .addClass('filename')
                    .appendTo(bg);

                $('<span>')
                    .addClass('icon-remove')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('li').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);

                $('<span>')
                    .addClass('icon-remove')
                    .hide()
                    .click(function (e) {
                        e.preventDefault();
                        var data = $(this).closest('li').data('data');
                        data.jqXHR.abort();
                    })
                    .appendTo(bg);

                var progress = $('<div>')
                    .addClass('progress progress-striped active')
                    .hide();

                $('<div>')
                    .addClass('progress-bar')
                    .attr('role', 'progressbar')
                    .appendTo(progress);

                progress.appendTo(bg);
            }

        },
        send: function (e, data) {
            if (data.context && data.dataType && data.dataType.substr(0, 6) === 'iframe') {
                data.context.find('.progress-bar').css('width', '50%');
            }
        },
        fail: function (e, data) {
            if (data.errorThrown === 'abort') return;
            alert('There was an error uploading "' + data.files[0].name + '".');
        },
        progress: function (e, data) {
            data.context.find('.progress-bar').css('width', parseInt(data.loaded / data.total * 100, 10) + '%');
        },
        start: function (e, data) {
            $('.progress').show();
            $('.neutral-upload-queue .ui-icon-close').hide();
            $('.neutral-upload-queue .ui-icon-cancel').show();
        },
        stop: function (e, data) {
            //data.context.find('.progress-bar').css('width', '100%');
            $('.neutral-upload-queue').empty();
        }
    });


    $('.negative-file-upload').fileupload({
        namespace: 'custom_attachment',
        dropZone: $('.negative-file-upload'),
        previewMaxWidth: 100,
        previewMaxHeight: 100,
        previewCrop: true,
        add: function (e, data) {
            for (var i = 0; i < data.files.length; i++) {

                if (!(/\.(gif|jpg|jpeg|tiff|png)$/i).test(data.files[i].name)) {
                    alert('Please select a valid image file. (jpg, jpeg, gif, tiff, png)');
                    isFilevalid = false;
                    return;
                }

                var item = $('<li>')
                    .appendTo($('.negative-upload-queue'));

                data.context = item;
                item.data('data', data);

                var bg = $('<div>')
                    .addClass('ts-color-bg-accent')
                    .appendTo(item);

                $('<div>')
                    .text(data.files[i].name + '  (' + parent.parent.Ts.Utils.getSizeString(data.files[i].size) + ')')
                    .addClass('filename')
                    .appendTo(bg);

                $('<span>')
                    .addClass('icon-remove')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('li').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);

                $('<span>')
                    .addClass('icon-remove')
                    .hide()
                    .click(function (e) {
                        e.preventDefault();
                        var data = $(this).closest('li').data('data');
                        data.jqXHR.abort();
                    })
                    .appendTo(bg);

                var progress = $('<div>')
                    .addClass('progress progress-striped active')
                    .hide();

                $('<div>')
                    .addClass('progress-bar')
                    .attr('role', 'progressbar')
                    .appendTo(progress);

                progress.appendTo(bg);
            }

        },
        send: function (e, data) {
            if (data.context && data.dataType && data.dataType.substr(0, 6) === 'iframe') {
                data.context.find('.progress-bar').css('width', '50%');
            }
        },
        fail: function (e, data) {
            if (data.errorThrown === 'abort') return;
            alert('There was an error uploading "' + data.files[0].name + '".');
        },
        progress: function (e, data) {
            data.context.find('.progress-bar').css('width', parseInt(data.loaded / data.total * 100, 10) + '%');
        },
        start: function (e, data) {
            $('.progress').show();
            $('.neutral-upload-queue .ui-icon-close').hide();
            $('.neutral-upload-queue .ui-icon-cancel').show();
        },
        stop: function (e, data) {
            //data.context.find('.progress-bar').css('width', '100%');
            $('.negative-upload-queue').empty();
        }
    });
    function readURL(input, target) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                target.attr('src', e.target.result);
                //$('<img>').attr('src', e.target.result).appendTo($('#agentrating-positive-image'));
                //$('#agentrating-positive-image').removeClass('rating-postive');
            }

            reader.readAsDataURL(input.files[0]);
        }
    }

    $(".hiddenfile-positive").change(function () {
        if (isFilevalid) {
            var attcontainer = $('.positive-upload-queue li');
            if (attcontainer.length > 0) {
                attcontainer.each(function (i, o) {
                    var data = $(o).data('data');
                    data.url = '../../../Upload/AgentRating/ratingpositive';
                    data.jqXHR = data.submit();
                    $(o).data('data', data);
                });
            }
            readURL(this, $('#agentrating-positive-img'));
        }

    });
    $(".hiddenfile-neutral").change(function () {
        if (isFilevalid) {
            var attcontainer = $('.neutral-upload-queue li');
            if (attcontainer.length > 0) {
                attcontainer.each(function (i, o) {
                    var data = $(o).data('data');
                    data.url = '../../../Upload/AgentRating/ratingneutral';
                    data.jqXHR = data.submit();
                    $(o).data('data', data);
                });
            }
            readURL(this, $('#agentrating-neutral-img'));
        }
    });
    $(".hiddenfile-negative").change(function () {
        if (isFilevalid) {

            var attcontainer = $('.negative-upload-queue li');
            if (attcontainer.length > 0) {
                attcontainer.each(function (i, o) {
                    var data = $(o).data('data');
                    data.url = '../../../Upload/AgentRating/ratingnegative';
                    data.jqXHR = data.submit();
                    $(o).data('data', data);
                });
            }
            readURL(this, $('#agentrating-negative-img'));
        }
    });

    $('#resetPositiveImage').click(function () {
        parent.parent.Ts.Services.Organizations.ResetRatingImage(1, function () {
            $('#agentrating-positive-img').attr("src", "../Images/face-positive.png");
        });
    });
    $('#resetNegativeImage').click(function () {
        parent.parent.Ts.Services.Organizations.ResetRatingImage(-1, function () {
            $('#agentrating-negative-img').attr("src", "../Images/face-negative.png");
        });
    });
    $('#resetNeutralImage').click(function () {
        parent.parent.Ts.Services.Organizations.ResetRatingImage(0, function () {
            $('#agentrating-neutral-img').attr("src", "../Images/face-neutral.png");
        });
    });

    var delay = (function () {
        var timer = 0;
        return function (callback, ms) {
            clearTimeout(timer);
            timer = setTimeout(callback, ms);
        };
    })();

    function buildWidgetCode() {
        var defaultString = "<script src='" + parent.parent.Ts.System.AppDomain + "/widget.js'></script>\n";
        var options = "";
        if ($('#widget-width').val() != '') {
            options = options + ",width:" + $('#widget-width').val();
        }
        if ($('#widget-height').val() != '') {
            options = options + ",height:" + $('#widget-height').val();
        }
        if ($('#widget-button-text').val() != '') {
            options = options + ",buttonText:" + $('#widget-button-text').val();
        }
        if ($('#widget-button-background').val() != '') {
            options = options + ",buttonBgColor:" + $('#widget-button-background').val();
        }
        if ($('#widget-button-text-color').val() != '') {
            options = options + ",buttonTextColor:" + $('#widget-button-text-color').val();
        }
        if ($('#widget-heading-text').val() != '') {
            options = options + ",text:" + $('#widget-heading-text').val();
        }
        if ($('#widget-offset').val() != '') {
            options = options + ",offset:" + $('#widget-offset').val();
        }

        var appendString = "<script>TeamSupport.Widget({orgID: " + parent.parent.Ts.System.Organization.OrganizationID + options + "'}); </script>";
        $('#widget-generated-code').val(defaultString + appendString);

    }

    $('.widget-change').keyup(function () {
        var thisVal = $(this).val();
        delay(function () {
            buildWidgetCode();
        }, 1000);
    });

    //$('.color-picker').minicolors({
    //    hide: function () {
    //        buildWidgetCode();
    //    }
    //});

    $('#recalculate-cdi').click(function () {
        parent.parent.Ts.Services.Organizations.ResetCDI();
        $(this).hide();
    });

    $('#portal_forwarding').click(function () {
        $('#portal_foward_location').attr('disabled', !this.checked)
    });

    function loadHubOptions() {
        parent.parent.Ts.Services.Admin.GetHubRedirectList(function (hubResults) {
            for (var i = 0; i < hubResults.length; i++) {
                $('<option>').attr('value', (hubResults[i])).attr('selected', _portalOption.PortalForward == hubResults[i] ? true : false).text(hubResults[i]).appendTo('#portal_foward_location');
            }
        });
    }

};


AdminPortal.prototype = {
    constructor: AdminPortal,
    refresh: function () {

    }
};
