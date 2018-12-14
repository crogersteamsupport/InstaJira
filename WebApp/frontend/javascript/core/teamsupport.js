jQuery(window).keydown(function(e) { if (e.keyCode == 120) debugger; });

// MODULARIZATION.
this.module = function(names, fn) {
    var name, space;
    if (typeof names === 'string') { names = names.split('.'); }
    space = this[name = names.shift()] || (this[name] = {});
    space.module || (space.module = this.module);
    if (names.length) {
        return space.module(names, fn);
    } else {
        return fn.call(space);
    }
};

this.module('teamsupport', function() {
    // TEAMSUPPORT.PAGE
    this.module('page', function() {
        this.data = new Object();
    });
    // TEAMSUPPORT.TOOLS
    this.module('tools', function() {
        // TEAMSUPPORT.TOOLS.JOURNAL
        this.journal = function(e) {
            console.log(e);
        }
        // TEAMSUPPORT.TOOLS.TRACE
        this.trace = function() {
            var err = new Error();
            console.log(err.stack);
        }
    });
    this.module('modals', function() {
        this.module('overlay', function() {
            this.show = function() {
                if ($('#overlay').is(':hidden')) {
                    $('#overlay').fadeIn("fast");
                }
            }
            this.hide = function() {
                if ($('#overlay').is(':visible')) {
                    $('#overlay').fadeOut("fast");
                }
            }
        });
        this.module('modal', function() {
            this.show = function(id) {
                if ($(id).is(':hidden')) {
                    $(id).slideToggle();
                    if ($(id).is(':visible')) {
                        teamsupport.modals.overlay.show();
                    }
                }
            }
            this.hide = function(id) {
                if ($(id).is(':visible')) {
                    $(id).slideToggle();
                }
            }
        });
    });
});

// CLOSE MODAL.
$(document).on('click tap', '#overlay, .fa.close, #btn-cancel', function(e) {
    teamsupport.modals.overlay.hide();
    $('#modal').empty().slideToggle('fast');
});

// AUTOGROW.
$.fn.autogrow = function (e) {
    return this.each(function (e) {
        if ($(this).hasClass('autogrow') && this.scrollHeight > this.clientHeight) {
            $(this).css('height', this.scrollHeight + 'px');
        }
    });
}

$(document).on('input', 'textarea.autogrow', function (e) {
    $(this).autogrow();
});

$(document).ready(function() {
    // AUTOGROW.
    $('textarea.autogrow').autogrow();
    $('.dropdown-submenu a.expand-submenu').on("click", function(e){
        $(this).next('ul').toggle();
        e.stopPropagation();
        e.preventDefault();
    });
});

// COPY LINK TO CLIPBOARD.
$(document).on('click', '#clipboard, button.clipboard', function (e) {
    var text = $(this).data('copy');
    var $temp = $("<input>");
    $("body").append($temp);
    $temp.val(text).select();
    document.execCommand("copy");
    $temp.remove();
    alert("Copied to your clipboard:\n\n" + text);
});

$(document).on('click', '#file-browse', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#file-input').trigger('click');
});

$(document).on('click','#ticket-tags .tag-item', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var tagid = $(this).attr('id');
    if (!$(e.target).hasClass('tagRemove')) {
        top.Ts.System.logAction('Ticket - Tag Linked From Action');
        top.Ts.MainPage.openTag(tagid);
    }
});
