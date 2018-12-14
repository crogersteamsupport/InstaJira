jQuery(document).ready(function () {

    function getURLParameter(name) {
        var val = (RegExp(name + '=' + '(.+?)(&|$)').exec(location.search) || [, null])[1];
        return val ? decodeURIComponent(val) : null;
    }

    initFromParams();

    if (getURLParameter("suerror") != null) {
        validateCompany();
        validateEmail();
        validateName();
        validatePassword();
        validateConfirm();
        alert("There was an error processing your sign up request.  Please review your information and try again.");
    }

    jQuery('#submit').click(function (e) {
		var reCaptchaResponse = window.grecaptcha.getResponse();
        validateCompany();
        validateEmail();
        validateName();
        validatePassword();
        validateConfirm();

		if (jQuery('.form-group.has-error').length > 0 || reCaptchaResponse == '') {
            e.preventDefault();
            return;
        }

        var btn = jQuery(this).hide();
        if (jQuery('.img-busy').length < 1) {
            jQuery('<img>').attr('src', 'https://www.teamsupport.com/hubfs/icons/signup-loading.gif').insertAfter(btn);
        }
        else {
            jQuery('.img-busy').show();
        }




    });

    jQuery('input[name="name"]').keyup(function (e) { validateName(); }).change(function (e) { validateName(); });
    jQuery('input[name="email"]').keyup(function (e) { validateEmail(); }).change(function (e) { validateEmail(); });
    jQuery('input[name="company"]').keyup(function (e) { validateCompany(); }).change(function (e) { validateCompany(); });
    jQuery('input[name="password"]').keyup(function (e) { validatePassword(); }).change(function (e) { validatePassword(); });
    jQuery('input[name="confirm"]').keyup(function (e) { validateConfirm(); }).change(function (e) { validateConfirm(); });

    function initFromParams() {
        function initParam(param) {
            var val = getURLParameter('_' + param);
            if (val) jQuery('input[name="' + param + '"]').val(val);
        }
        initParam('name');
        initParam('email');
        initParam('company');
        initParam('phone');
        initParam('promo');
        initParam('product');
    }

    function validateName() {
        var el = jQuery('input[name="name"]');
        var nameReg = /\w+\s+\w+/;
        var name = jQuery.trim(el.val());
        if (!nameReg.test(name)) {
            el.closest('.form-group').addClass('has-error');
        } else {
            el.closest('.form-group').removeClass('has-error');
        }
    }

    function validateEmail() {
        var el = jQuery('input[name="email"]');
        var emailReg = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@(?!hotmail.com)(?!me.com)(?!yahoo.)(?!aol.com)(?!live.com)(?!qq.com)((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

        var email = jQuery.trim(el.val());
        if (!emailReg.test(email) || email == '') {
            el.closest('.form-group').addClass('has-error');
        } else {
            el.closest('.form-group').removeClass('has-error');
        }
    }

    function validatePassword() {
        var el = jQuery('input[name="password"]');
        if (el == null) return;
        var pw = jQuery.trim(el.val());
        var confirm = jQuery.trim(jQuery('input[name="confirm"]').val());

        if (pw.length < 8) {
            el.closest('.form-group').addClass('has-error');
        } else {
            el.closest('.form-group').removeClass('has-error');
        }
    }

    function validateConfirm() {
        var el = jQuery('input[name="confirm"]');
        if (el == null) return;

        var pw = jQuery.trim(jQuery('input[name="password"]').val());
        var confirm = jQuery.trim(el.val());

        if (pw != confirm) {
            el.closest('.form-group').addClass('has-error');
        } else {
            el.closest('.form-group').removeClass('has-error');
        }
    }


    function getBaseUrl(url) {
        pathArray = url.split('/');
        protocol = pathArray[0];
        host = pathArray[2];
        return protocol + '//' + host;
    }

    function getScriptUrl() {
        var scripts = document.getElementsByTagName('script');
        for (i = 0; i < scripts.length; i++) {
            if (scripts[i].src.indexOf("ts-signup.js") > -1) {
                return getBaseUrl(scripts[i].src);
            }
        }
        return "";
    }


    function validateCompany(callback) {
        var el = jQuery('input[name="company"]');
        var company = jQuery.trim(el.val());
        var group = el.closest('.form-group');
        var help = group.find('.help-block');
        if (company == '') {
            group.addClass('has-error');
            help.text("Please enter a company name.");
            return;
        }

        jQuery.ajax({
            type: "GET",
            url: getScriptUrl() + "/signup/fn/validatecompany",
            data: { name: company },
            crossDomain: true,
            dataType: 'jsonp'
        })
            .done(function (result) {
                if (result.isValid == false) {
                    group.addClass('has-error');
                    help.text("That company name already exists.  Please choose another one.");
                }
                else {
                    group.removeClass('has-error');
                }
                if (callback) callback();
            });
    }

    function validatePhone() {
        var el = jQuery('input[name="phone"]');
        var intRegex = /[0-9 -()+]+$/;
        var phone = jQuery.trim(el.val());
        if ((phone.length < 6) || (!intRegex.test(phone))) {
            el.closest('.form-group').addClass('has-error');
        } else {
            el.closest('.form-group').removeClass('has-error');
        }

    }




});
