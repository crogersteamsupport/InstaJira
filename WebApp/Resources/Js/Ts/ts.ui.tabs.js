/// <reference path="ts.js" />

if (typeof Ts == "undefined") Ts = {};
if (typeof Ts.Ui == "undefined") Ts.Ui = {};

function getNameParam(idClass, element, indexFromEnd, splitter) {
    function findClass(classString, term) {
        if (!classString || classString.length < 1) return '';
        classes = classString.split(' ');
        for (var i = 0; i < classes.length; i++) {
            if (classes[i].indexOf(term) === 0) {
                return classes[i];
            }
        }
        return '';
    }

    var item = '';
    if (!splitter) splitter = '-';
    idClass = idClass + splitter;
    if (element.id && element.id.indexOf(idClass) === 0) {
        item = element.id;
    } else {
        item = findClass($(element).attr('class'), idClass);
    }

    if (item === '') {
        return null;
    }
    var result = item.split(splitter);
    return result[result.length - (indexFromEnd + 1)];
}

Ts.Ui.Tabs = function (element) {
    this._element = element;
    var self = this;
    this._history = [];
    this._events = [];
    this._list = $(this._element).addClass('ts-tabs ui-widget-header').html('<ul class="ui-helper-reset ui-helper-clearfix"></ul>').disableSelection().find('ul');
    this._sortStop = function (event, ui) {
        if (self._events.sort) {
            self._events.sort(self);
        }
    };
    this._callEvent = function (event, sender) {
        if (self._events[event]) {
            return self._events[event](sender ? sender : self);
        }
        return true;
    };
    $(this._list).sortable({
        items: 'li.ts-tab-sortable',
        stop: this._sortStop
    });
}

Ts.Ui.Tabs.prototype = {
    constructor: Ts.Ui.Tabs,
    getElement: function () {
        return this._element;
    },
    prepend: function (isSelected, tabType, id, caption, isClosable, isSortable, isHighlighted, icon, imageUrl, data, title) {
        console.log(title);
        var tab = this.find(id, tabType);
        if (tab) {
            tab.select();
            $(tab.getElement()).insertAfter($(this._list).find('li:first'));
            return tab;
        }

        while (true) {
            var tablist = this.getTabs();
            if (tablist.length > 15) tablist[tablist.length - 1].remove();
            else break;
        }

        var self = this;
        if (self._callEvent('beforeAdd') === false) {
            return null;
        }
        // var html = '<li class="ui-state-default ts-tab-' + tabType + '-' + id + '"><span class="ts-tabs-hide tab-icon ts-icon"></span><img class="ts-tabs-hide tab-image"/>    <a href="#"></a><span class="ts-tabs-hide tab-close ui-icon ui-icon-close"></span></li>';
        var li = $('<li>').addClass('ui-state-default ts-tab-' + tabType + '-' + id).hover(
            function () {
                // $(this).addClass('ui-state-hover').find('.ui-icon-close').show();
            },
            function () {
                // $('.ts-tab').removeClass('ui-state-hover');
                // $('.ts-tab .ui-icon-close').hide();
            }
        );
        if (title && title != null) li.attr('title', title);
        var contentSpan = $('<span>').addClass('ts-tab-content').appendTo(li);
        $('<span>').addClass('ts-tabs-hide tab-icon ts-icon').appendTo(contentSpan);
        $('<img>').addClass('ts-tabs-hide tab-image').appendTo(contentSpan);
        $('<a>').attr('href', '#').appendTo(contentSpan);
        // $('<span>').addClass('tab-close ui-icon ui-icon-close').appendTo(contentSpan).hide();
        if (isClosable) {
            $('<img>').attr('src','/vcr/1_9_0/images/icons/close.png').addClass('close tab-close').appendTo(contentSpan);
        }

        li.insertAfter($(this._list).find('li:first'));

        var element = $(this._list).find('.ts-tab-' + tabType + '-' + id);
        tab = new Ts.Ui.Tabs.Tab(element);

        tab.setCaption(caption);
        tab.setIsClosable(isClosable);
        tab.setIcon(icon);
        tab.setImageUrl(imageUrl);
        tab.setIsHighlighted(isHighlighted);
        tab.setData(data);
        if (isSortable) {
            $(element).addClass('ts-tab-sortable');
        }
        $(element).find('.tab-close').click(function (e) {
            e.preventDefault();
            if ($(element).find('.tab-close').hasClass('ts-tabs-noclose')) return;
            e.stopPropagation();

            if (tab.getIsHighlighted() && !confirm('Are you sure you would like to close this tab?')) {
                return false;
            }
            if (self._callEvent('beforeRemove', tab) === false) {
                return;
            }
            var selected = tab.getIsSelected();
            $(element).remove();
            var flag = false;
            if (selected) {
                var lastTab = self._history.pop();
                while (lastTab) {
                    var item = $(self._element).find('.ts-tab-' + lastTab);
                    if (item.length > 0) {
                        (new Ts.Ui.Tabs.Tab(item)).select();
                        flag = true;
                        break;
                    }
                    lastTab = self._history.pop();
                }

                if (flag === false) {
                    var first = $(self._element).find('.ts-tab:first');
                    if (first.length > 0) {
                        (new Ts.Ui.Tabs.Tab(first)).select();
                    }
                }
            }

            self._callEvent('afterRemove');
            return true;
        });

        $(element).click(function (e) {
            e.stopPropagation();
            e.preventDefault();
            if (self._callEvent('beforeSelect', tab) === false) {
                return;
            }
            $(element).parents('ul').find('.ui-state-error-hidden').removeClass('ui-state-error-hidden').addClass('ui-state-error');
            $(element).parents('ul').find('.ui-state-active').removeClass('ui-state-active');
            $(element).addClass('ui-state-active');
            if (tab.getIsHighlighted()) {
                $(element).addClass('ui-state-error-hidden').removeClass('ui-state-error');
            }
            self._history.push(tabType + '-' + id);
            self._callEvent('afterSelect', tab);
        });

        self._callEvent('afterAdd');

        if (isSelected) {
            tab.select();
        }

        return tab;

    },

    add: function (isSelected, tabType, id, caption, isClosable, isSortable, isHighlighted, icon, imageUrl, data, title) {
        var tab = this.find(id, tabType);
        if (tab) {
            tab.select();
            return tab;
        }
        var self = this;
        if (self._callEvent('beforeAdd') === false) {
            return null;
        }
        // var html = '<li class="ui-state-default ts-tab-' + tabType + '-' + id + '"><span class="ts-tabs-hide tab-icon ts-icon"></span><img class="ts-tabs-hide tab-image"/> <a href="#"></a> <span class="fa fa-close ts-tabs-hide tab-close ui-icon ui-icon-close"></span></li>';
        var li = $('<li>').addClass('ui-state-default ts-tab-' + tabType + '-' + id).hover(
            function () {
                // $(this).addClass('ui-state-hover').find('.ui-icon-close').show();
            },
            function () {
                // $('.ts-tab').removeClass('ui-state-hover');
                // $('.ts-tab .ui-icon-close').hide();
            }
        );
        if (title && title != null) li.attr('title', title);
        var contentSpan = $('<span>').addClass('ts-tab-content').appendTo(li);
        $('<span>').addClass('ts-tabs-hide tab-icon ts-icon').appendTo(contentSpan);
        $('<img>').addClass('ts-tabs-hide tab-image').appendTo(contentSpan);
        $('<a>').attr('href', '#').appendTo(contentSpan);
        // $('<span>').addClass('tab-close ui-icon ui-icon-close').appendTo(contentSpan);
        if (isClosable) {
            $('<img>').attr('src','/vcr/1_9_0/images/icons/close.png').addClass('close tab-close').appendTo(contentSpan);
        }

        $(this._list).append(li);
        var element = $(this._list).find('.ts-tab-' + tabType + '-' + id);
        tab = new Ts.Ui.Tabs.Tab(element);

        tab.setCaption(caption);
        tab.setIsClosable(isClosable);
        tab.setIcon(icon);
        tab.setImageUrl(imageUrl);
        tab.setIsHighlighted(isHighlighted);
        tab.setData(data);
        if (isSortable) {
            $(element).addClass('ts-tab-sortable');
        }
        $(element).find('.tab-close').click(function (e) {
            e.preventDefault();
            if ($(element).find('.tab-close').hasClass('ts-tabs-noclose')) return;
            e.stopPropagation();

            if (tab.getIsHighlighted() && !confirm('Are you sure you would like to close this tab?')) {
                return false;
            }
            if (self._callEvent('beforeRemove', tab) === false) {
                return;
            }
            var selected = tab.getIsSelected();
            $(element).remove();
            var flag = false;
            if (selected) {
                var lastTab = self._history.pop();
                while (lastTab) {
                    var item = $(self._element).find('.ts-tab-' + lastTab);
                    if (item.length > 0) {
                        (new Ts.Ui.Tabs.Tab(item)).select();
                        flag = true;
                        break;
                    }
                    lastTab = self._history.pop();
                }
                if (flag === false) {
                    var first = $(self._element).find('.ts-tab:first');
                    if (first.length > 0) {
                        (new Ts.Ui.Tabs.Tab(first)).select();
                    }
                }
            }
            self._callEvent('afterRemove');
            return true;
        });

        $(element).click(function (e) {
            e.stopPropagation();
            e.preventDefault();
            if (self._callEvent('beforeSelect', tab) === false) {
                return;
            }
            $(element).parents('ul').find('.ui-state-error-hidden').removeClass('ui-state-error-hidden').addClass('ui-state-error');
            $(element).parents('ul').find('.ui-state-active').removeClass('ui-state-active');
            $(element).addClass('ui-state-active');
            if (tab.getIsHighlighted()) {
                $(element).addClass('ui-state-error-hidden').removeClass('ui-state-error');
            }
            self._history.push(tabType + '-' + id);
            self._callEvent('afterSelect', tab);
        });
        self._callEvent('afterAdd');

        if (isSelected) {
            tab.select();
        }
        return tab;

    },
    find: function (id, type) {
        var item = $(this._element).find('.ts-tab-' + type + '-' + id);
        if (item.length < 1) {
            return null;
        }
        return new Ts.Ui.Tabs.Tab(item[0]);
    },
    getTabs: function () {
        var items = $(this._element).find('li');
        var result = [];
        for (var i = 0; i < items.length; i++) {
            result[result.length] = new Ts.Ui.Tabs.Tab(items[i]);
        }
        return result;
    },
    getByIndex: function (index) {
        var items = $(this._element).find('li');
        var result = null;
        for (var i = 0; i < items.length; i++) {
            if (i == index) {
                return new Ts.Ui.Tabs.Tab(items[i]);
            }
        }
        return result;

    },
    getSelected: function () {
        var element = $(this._element).find('.ui-state-active');
        if (element.length < 1) return null;
        return new Ts.Ui.Tabs.Tab(element[0]);
    },
    bind: function (event, callback) {
        this._events[event] = callback;
    }

};



Ts.Ui.Tabs.Tab = function (element) {
    this._element = element;
}

Ts.Ui.Tabs.Tab.prototype = {
    constructor: Ts.Ui.Tabs.Tab,
    //
    getElement: function () {
        return this._element;
    },
    getId: function () {
        return getNameParam('ts-tab', this._element, 0);
    },
    getTabType: function () {
        return getNameParam('ts-tab', this._element, 1);
    },
    getCaption: function () {
        return $(this._element).find('a').html();
    },
    setCaption: function (value) {
        $(this._element).find('a').html(value);
    },

    getIsClosable: function () {
        return $(this._element).find('.tab-close').hasClass('ts-tabs-noclose');
    },
    setIsClosable: function (value) {
        if (value && value === true) {
            $(this._element).find('.tab-close').removeClass('ts-tabs-noclose');
        } else {
            $(this._element).find('.tab-close').addClass('ts-tabs-noclose');
        }
    },
    setIcon: function (value) {
        var clss = (value && value !== '') ? 'tab-icon ts-icon ' + value : 'tab-icon ts-tabs-hide';
        $(this._element).find('.tab-icon').attr('class', clss);
    },
    setImageUrl: function (value) {
        if (value && value !== '') {
            $(this._element).find('.tab-image').removeClass('ts-tabs-hide').attr('src', value);
        } else {
            $(this._element).find('.tab-image').addClass('ts-tabs-hide');
        }

    },
    getIsHighlighted: function () {
        return $(this._element).hasClass('ui-state-error');
    },
    setIsHighlighted: function (value) {
        if (value && value === true) {
            $(this._element).addClass('ui-state-error');
        } else {
            $(this._element).removeClass('ui-state-error ui-state-error-hidden');
        }
    },
    getIsSelected: function () {
        return $(this._element).hasClass('ui-state-active');
    },
    getData: function () {
        return $(this._element).data('data');
    },
    setData: function (value) {
        $(this._element).data('data', value);
    },
    select: function () {
        $(this._element).click();
    },
    remove: function () {
        return $(this._element).find('.tab-close').click();
    }
};

Ts.Ui.Tabs.Tab.Type = {};
Ts.Ui.Tabs.Tab.Type.None = '';
Ts.Ui.Tabs.Tab.Type.Main = 'main';
Ts.Ui.Tabs.Tab.Type.Ticket = 'ticket';
Ts.Ui.Tabs.Tab.Type.NewTicket = 'new_ticket';
Ts.Ui.Tabs.Tab.Type.Company = 'company';
Ts.Ui.Tabs.Tab.Type.NewCompany = 'new_company';
Ts.Ui.Tabs.Tab.Type.Contact = 'contact';
Ts.Ui.Tabs.Tab.Type.NewContact = 'new_contact';
Ts.Ui.Tabs.Tab.Type.Report = 'report';
Ts.Ui.Tabs.Tab.Type.Asset = 'asset';
Ts.Ui.Tabs.Tab.Type.NewAsset = 'new_asset';
Ts.Ui.Tabs.Tab.Type.Product = 'product';
Ts.Ui.Tabs.Tab.Type.NewProduct = 'new_product';
Ts.Ui.Tabs.Tab.Type.ProductVersion = 'product_version';
Ts.Ui.Tabs.Tab.Type.NewProductVersion = 'new_product_version';
Ts.Ui.Tabs.Tab.Type.NewProductFamily = 'new_product_family';
Ts.Ui.Tabs.Tab.Type.ProductFamily = 'product_family';
Ts.Ui.Tabs.Tab.Type.Task = 'task';
Ts.Ui.Tabs.Tab.Type.NewTask = 'new_task';
Ts.Ui.Tabs.Tab.Type.NewTaskFromSource = 'new_task_from_source';
