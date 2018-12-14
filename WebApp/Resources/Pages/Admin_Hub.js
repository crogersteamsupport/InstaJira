
$(document).ready(function () {
    var hubList = [];

    function pageLoad(callback) {
        parent.parent.Ts.Services.Admin.GetHubURL(function (result) {
            hubList = result;
            var enableNewHubs = false;

            if (parent.parent.Ts.System.Organization.UseProductFamilies == true && parent.parent.Ts.System.Organization.ProductType == parent.parent.Ts.ProductType.Enterprise) {
                enableNewHubs = true;
            }

            var template = Handlebars.compile($("#hub-list-template").html());
            data = { hubList: hubList, enableNewHubs: enableNewHubs };

            $("#HubList").html(template(data));

            if (!callback) {
                GetHubURL(hubList[0], function (url) {
                    $('#hub_admin').attr('src', url);
                });
                $('#HubList > .hub-container > [data-hubid="' + hubList[0].HubID + '"]').parent('.hub-container').addClass('active');
            }

            if (callback) {
                callback();
            }
        });

        Handlebars.registerHelper("ProductFamilyEnabled", function (ProductFamilyID) {
            for (i = 0; i < hubList.length; i++) {
                if (hubList[i].ProductFamilyID == ProductFamilyID) {
                    return "disabled";
                }
            }
        });
    }



    function GetHubURL(hubRecord, callback) {
        var encrypted = CryptoJS.AES.encrypt(Date.now() + "," + parent.parent.Ts.System.User.Email + "", parent.parent.Ts.System.Organization.PortalGuid);
        callback("https://" + hubRecord.URL + "/#/sso/" + encrypted);
    };

    function ClearNewHubSettings() {
        $('#HubName').val("");
    }

    pageLoad();

    $('#HubList').on('click', '#btnDisabledNewHub', function (e) {
        e.preventDefault();

        $('#hub_admin').fadeOut();
        $('#disabledNewHub').fadeIn();
    });

    $('#HubList').on('click', '.btnLoadHub', function (e) {
        e.preventDefault();
        var HubID = $(this).data('hubid');

        $('#HubList .hub-container').removeClass('active');
        $(this).parent('.hub-container').addClass('active');

        for (i = 0; i < hubList.length; i++) {
            if (hubList[i].HubID == HubID) {
                GetHubURL(hubList[i], function (url) {
                    $('#hub_admin').attr('src', url);
                    $('#newHub').fadeOut();
                    $('#disabledNewHub').fadeOut();
                    $('#hub_admin').delay(400).fadeIn();
                });
            }
        }
    });

    $('#HubList').on('click', '.btnDeleteHub', function (e) {
        e.preventDefault();
        var HubID = $(this).data('hubid');

        if (confirm('Are you sure you want to delete this hub? All customized hub page data will be lost.')) {
            if (confirm('Final Warning: Deleting your hub means you will lose all of your customizations and can result in data loss, please verify this is your intention.  After deletion hub customizations CANNOT be recovered.')) {
                parent.parent.Ts.Services.Admin.DeleteHub(HubID, function () {
                    pageLoad();
                });
            } else {
                // Do nothing!
            }
        } else {

        }

    });

    $('#HubList').on('click', '#btnNewHub', function (e) {
        e.preventDefault();

        $('#HubList .hub-container').removeClass('active');
        $(this).parent('.hub-container').addClass('active');

        $.validator.addMethod("url", function (value, element) {
            return this.optional(element) || /^[A-Za-z0-9-]+$/i.test(value);
        }, "Your hub name must be one alphanumeric word with no special characters");

        validator = $("#newHubForm").validate({
            rules: {
                HubName: "required url"
            },
        });

        var copyHubTemplate = Handlebars.compile($("#CopyHubTemplate").html());
        data = { Hubs: hubList };

        $("#CopyHubList").html(copyHubTemplate(data));

        parent.parent.Ts.Services.Products.GetProductFamilies(function (data) {
            var productLineTemplate = Handlebars.compile($("#ProductLineTemplate").html());
            data = { ProductLines: data };

            $("#ProductLineList").html(productLineTemplate(data));

            $('#hub_admin').fadeOut();
            $('#newHub').delay(400).fadeIn();
        });

        

    });

    $('#newHub').on('click', '#btnFinalizeNewHub', function (e) {
        if ($("#newHubForm").valid() == true) {
            parent.parent.Ts.Services.Admin.CreateNewHub($('#HubName').val(), $('#CopyHubList').val(), $('#ProductLineList').val(), function (creationResult) {
                if (creationResult.IsSuccess) {
                    pageLoad(function () {
                        GetHubURL(creationResult.HubLinkModel, function (url) {
                            $('#hub_admin').attr('src', url);
                            $('#newHub').fadeOut();
                            $('#disabledNewHub').fadeOut();
                            $('#hub_admin').delay(400).fadeIn();
                            ClearNewHubSettings();
                            validator.destroy();
                            $('#HubList > .hub-container > [data-hubid="' + creationResult.HubLinkModel.HubID + '"]').parent('.hub-container').addClass('active');
                        })
                    });
                }
                else {
                    validator.showErrors({
                        "HubName": creationResult.ErrorMessage
                    });
                }
            });
        }
    });

    $('#newHub').on('click', '#btnCancelNewHub', function (e) {
        pageLoad(function () {
            GetHubURL(hubList[0], function (url) {
                $('#hub_admin').attr('src', url);
                $('#newHub').fadeOut();
                $('#disabledNewHub').fadeOut();
                $('#hub_admin').delay(400).fadeIn();
                ClearNewHubSettings();
                validator.destroy();
                $('#HubList > .hub-container > [data-hubid="' + hubList[0].HubID + '"]').parent('.hub-container').addClass('active');
            })
        });
    });

});

