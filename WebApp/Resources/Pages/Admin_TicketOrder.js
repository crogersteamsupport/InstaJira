var ticketWidget = null;

$(document).ready(function () {
    LoadOrder();
    CreateDOMEvents();
    LoadPluginTemplate('ticket');

    if (window.parent.parent.parent.Ts.System.Organization.ProductType != window.parent.parent.parent.Ts.ProductType.Enterprise) {
        $('#btnAddTicketPlugin').remove();
    }

    var _codeMirror = CodeMirror.fromTextArea(document.getElementById('plugin-code'), {
        lineNumbers: true,
        mode: "htmlmixed",
        htmlMode: true
    });

    var _pluginID = -1;

    function LoadOrder() {
        //get ticket categories and append them to list
        window.parent.parent.parent.Ts.Services.TicketPage.GetTicketPageOrder('TicketFieldsOrder', function (items) {
            jQuery.each(items, function (i, val) {
                if (val.Disabled == "false") {
                    CreateOrderElement('.admin-ticket-page-fields', 'admin-ticket-page-field', val);
                }
                else {
                    CreateOrderElement('.admin-ticket-page-fields-disabled', 'admin-ticket-page-field', val);
                }
            });
        });

        window.parent.parent.parent.Ts.Services.TicketPage.GetTicketPageOrder('NewTicketFieldsOrder', function (items) {
            jQuery.each(items, function (i, val) {
                if (val.Disabled == "false") {
                    CreateOrderElement('.admin-newticket-page-fields', 'admin-newticket-page-field', val);
                }
                else {
                    CreateOrderElement('.admin-newticket-page-fields-disabled', 'admin-newticket-page-field', val);
                }
            });
        });

        //now make them sortable
        $('.admin-ticket-page-fields, .admin-ticket-page-fields-disabled').sortable({
            items: '.admin-ticket-page-field', connectWith: '.ticket-page-sortable', placeholder: 'admin-to-cat-placeholder', handle: '.fa-bars', update: function (e, ui) {
                //save when updated
                SaveOrder('.admin-ticket-page-fields', '.admin-ticket-page-field', 'TicketFieldsOrder');
            }
        });

        $('.admin-newticket-page-fields, .admin-newticket-page-fields-disabled').sortable({
            items: '.admin-newticket-page-field', connectWith: '.newticket-page-sortable', placeholder: 'admin-to-cat-placeholder', handle: '.fa-bars', update: function (e, ui) {
                //save when updated
                SaveOrder('.admin-newticket-page-fields', '.admin-newticket-page-field', 'NewTicketFieldsOrder');
            }
        });

        $('body').on('click', '.admin-to-cat.admin-linebreak .fa-times', function (e) {
            $(this).parent().remove();
            SaveOrder('.admin-ticket-page-fields', '.admin-ticket-page-field', 'TicketFieldsOrder');
            SaveOrder('.admin-newticket-page-fields', '.admin-newticket-page-field', 'NewTicketFieldsOrder');
        });
    };

    function CreateOrderElement(parent, cssclass, item) {
        if (item.ItemID) {
            $(parent).append('<div data-itemid="' + item.ItemID + '" data-catid="' + item.CatID + '" class="admin-to-cat admin-plugin ' + cssclass + '"><i class="fa fa-bars color-lightgray"></i><span class="admin-to-cat-name">' + item.CatName + '</span><i class="fa fa-pencil color-darkgray"></i></div>');
        }
        else {
            if (item.CatID == "hr") {
                $(parent).append('<div data-catid="' + item.CatID + '" class="admin-to-cat admin-linebreak ' + cssclass + '"><i class="fa fa-bars color-lightgray"></i><span class="admin-to-cat-name">' + item.CatName + '</span><i class="fa fa-times color-darkgray"></i></div>');
            }
            else {
                $(parent).append('<div data-catid="' + item.CatID + '" class="admin-to-cat ' + cssclass + '"><i class="fa fa-bars color-lightgray"></i><span class="admin-to-cat-name">' + item.CatName + '</span></div>');
            }
        }
    };

    function LoadPluginTemplate(type) {

        $('#acc-template').empty();
        var source = $('#entry-template').html();
        var template = Handlebars.compile(source);
        window.parent.parent.parent.Ts.Services.TicketPage.GetTicketPagePluginTemplates(type, function (result) {
            var data = JSON.parse(result);
            for (var i = 0; i < data.length; i++) {
                var content = "";
                for (var j = 0; j < data[i].items.length; j++) {
                    content += '<div class="col-md-3"><a class="placeholder-link" href="#">{{' + data[i].name + '.' + data[i].items[j] + '}}</a></div>'
                }
                var html = template({ "collapse": 'collapse-' + i, "heading": 'heading-' + i, "title": data[i].name, "content": content });
                $('#acc-template').append(html);
            }
        });

    };


    function CreateDOMEvents() {
        //refresh page
        $('#btnRefresh').click(function (e) {
            e.preventDefault();
            window.location = window.location;
        });
        $('button').button();
        $('#btnAddTicketPlugin').click(function (e) {
            e.preventDefault();
            _pluginID = -1;
            $('#plugin-name').val('');
            setCode();
            $('#plugin-name').closest('.form-group').removeClass('has-error');
            $('#btnPluginDelete').addClass('hidden');
            $('#div-main').addClass('hidden');
            $('#div-plugin').removeClass('hidden');
            $('#plugin-show-variables').text('Show Placeholders >>');
            $('#plugin-variables').addClass('hidden');
        });

        $('body').on('click', '.admin-to-cat .fa-pencil', function (e) {
            _pluginID = $(this).closest('div').data('itemid');
            window.parent.parent.parent.Ts.Services.TicketPage.GetTicketPagePlugin(_pluginID, function (result) {
                $('#plugin-name').val(result.Name);
                setCode(result.Code);
                $('#plugin-name').closest('.form-group').removeClass('has-error');
                $('#btnPluginDelete').removeClass('hidden');
                $('#plugin-show-variables').text('Show Placeholders >>');
                $('#plugin-variables').addClass('hidden');
                $('#div-main').addClass('hidden');
                $('#div-plugin').removeClass('hidden');
            });
        });

        $('#plugin-variables').on('click', '.placeholder-link', function (e) {
            e.preventDefault();
            _codeMirror.getDoc().replaceSelection($(this).text());
        });
        $('#btnPluginRefresh').click(function (e) {
            e.preventDefault();

            var ticketNo = $('#sampleTicketNo').val();
            //if (!isNaN(parseFloat(ticketNo)) && isFinite(ticketNo) != true)
            var code = getCode();

            $('#sample').html('<i class="fa fa-refresh fa-spin fa-3x fa-fw"></i><span class="sr-only">Loading...</span>');
            window.parent.parent.parent.Ts.Services.TicketPage.GetTicketPagePluginSample(ticketNo, code, function (result) {
                if (result == null) {
                    alert('Please enter a valid ticket number');
                    return;
                }
                data = JSON.parse(result);
                ticketWidget = new TicketWidget(data.ticket);
                $('#sample').html(data.code);
            }, function () {
                alert('Please enter a valid ticket number');
                $('#sample').html('');
            });
        });


        $('#plugin-show-variables').click(function (e) {
            e.preventDefault();
            var el = $('#plugin-show-variables');
            if (el.text().indexOf('Show') > -1) {
                el.text('<< Hide Placeholders');
                $('#plugin-variables').removeClass('hidden');
            }
            else {
                el.text('Show Placeholders >>');
                $('#plugin-variables').addClass('hidden');
            }
        });

        $('#plugin-hide-variables').click(function (e) {
            e.preventDefault();
        });


        $('#btnPluginCancel').click(function (e) {
            e.preventDefault();
            $('#div-main').removeClass('hidden');
            $('#div-plugin').addClass('hidden');
        });

        $('#btnPluginSave').click(function (e) {
            e.preventDefault();
            if ($('#plugin-name').val().trim() == '') {
                $('#plugin-name').closest('.form-group').addClass('has-error');
                return;
            }

            window.parent.parent.parent.Ts.Services.TicketPage.SaveTicketPagePlugin(_pluginID, $('#plugin-name').val(), getCode(), function (result) {
                if (_pluginID < 0) {
                    var item = {};
                    item.CatID = "plugin";
                    item.CatName = result.Name;
                    item.ItemID = result.PluginID;
                    item.Disabled = "false";
                    CreateOrderElement('.admin-ticket-page-fields', 'admin-ticket-page-field', item);
                    $('.admin-ticket-page-fields, .admin-ticket-page-fields-disabled').sortable("refresh");
                }
                else {
                    $("body").find('[data-itemid="' + _pluginID + '"]').find('.admin-to-cat-name').text($('#plugin-name').val());
                }
                SaveOrder('.admin-ticket-page-fields', '.admin-ticket-page-field', 'TicketFieldsOrder');
                $('#div-main').removeClass('hidden');
                $('#div-plugin').addClass('hidden');
            });

        });

        $('#btnPluginDelete').click(function (e) {
            e.preventDefault();
            if (confirm('Are you sure you would like to delete this plugin?')) {
                window.parent.parent.parent.Ts.Services.TicketPage.DeleteTicketPagePlugin(_pluginID);
                $("body").find('[data-itemid="' + _pluginID + '"]').remove();
                $('#div-main').removeClass('hidden');
                $('#div-plugin').addClass('hidden');
                SaveOrder('.admin-ticket-page-fields', '.admin-ticket-page-field', 'TicketFieldsOrder');
            }
        });

        $('#btnAddSpacerNewTicket').click(function (e) {
            var item = Object();
            item.CatID = 'hr';
            item.CatName = 'Line Break'
            CreateOrderElement('.admin-newticket-page-fields', 'admin-newticket-page-field', item);
            SaveOrder('.admin-newticket-page-fields', '.admin-newticket-page-field', 'NewTicketFieldsOrder');
        });
        $('#btnAddSpacerExistingTicket').click(function (e) {
            var item = Object();
            item.CatID = 'hr';
            item.CatName = 'Line Break'
            CreateOrderElement('.admin-ticket-page-fields', 'admin-ticket-page-field', item);
            SaveOrder('.admin-ticket-page-fields', '.admin-ticket-page-field', 'TicketFieldsOrder');
        });
    };


    function SaveOrder(parent, classname, group) {
        var items = new window.parent.parent.parent.Array();

        $(parent + ' > ' + classname).each(function (index) {
            var item = new Object();
            item.CatID = $(this).data('catid');
            item.ItemID = $(this).data('itemid');
            item.CatName = $(this).find('.admin-to-cat-name').text();
            item.Disabled = "false";
            items.push(item);
        });
        $(parent + '-disabled > ' + classname).each(function (index) {
            var item = new Object();
            item.CatID = $(this).data('catid');
            item.ItemID = $(this).data('itemid');
            item.CatName = $(this).find('.admin-to-cat-name').text();
            item.Disabled = "true";
            items.push(item);
        });
        window.parent.parent.parent.Ts.System.logAction('Admin Ticket Order - ' + group + ' Order Saved');
        window.parent.parent.parent.Ts.Services.TicketPage.SaveTicketPageOrder(group, JSON.stringify(items));

    };

    function setCode(content)
    {
        if (!content) {
            content = "Please enter your HTML, CSS, and JavaScript code here."
        }

        _codeMirror.getDoc().setValue(content);
        setTimeout(function () {
            _codeMirror.refresh();
        }, 1);
    }

    function getCode()
    {
        return _codeMirror.getDoc().getValue();
    }
});
