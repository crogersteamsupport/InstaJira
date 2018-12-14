/// <reference path="ts.js" />

Ts.Ui.MenuTree = function (element) {
  this._element = element;
  var self = this;
  $(element).addClass('ts-menutree').disableSelection();
  this._events = [];
  this._callEvent = function (event, sender) { if (self._events[event]) { return self._events[event](sender ? sender : self); } return true; };
}

Ts.Ui.MenuTree.prototype = {
  constructor: Ts.Ui.MenuTree,
  getElement: function () { return this._element; },
  add: function (parent, id, type, caption, imageUrl, data) {
    var html = '<li class="ts-menutree-item menutree-item-' + type + '-' + id + '">' +
              '<div class="ui-corner-allx">' +
              '<img />' +
              '<a href="#' + type + '-' + id + '"></a><span class="ui-icon ts-icon-none"></span></div></li>';
    var list = null;
    if (parent) {
      list = $(parent.getElement()).children('ul');
      if (list.length < 1) {
        list = $('<ul>').addClass('ui-helper-hidden').appendTo(parent.getElement());
        $(parent.getElement()).children('div').children('.ui-icon').removeClass('ts-icon-none').addClass('ui-icon-carat-1-e').click(function (e) {
          e.stopPropagation();
          var item = new Ts.Ui.MenuTree.Item($(this).parents('li'));
          item.setIsExpanded(!item.getIsExpanded());
        });
      }
    }
    else {
      list = $(this._element).children('ul');
      if (list.length < 1) { list = $(this._element).append('<ul></ul').children('ul'); }
  }
    list.append(html);

    var item = this.find(id, type);
    if (item === null) return null;
    $(item.getElement()).children('div').hover(function () {
      //$(this).addClass('ui-state-hover');
    }, function () {
      //$(this).closest('.ts-menutree').find('div').removeClass('ui-state-hover');
    });
    var self = this;
    $(item.getElement()).click(function (e) {
      var mi = new Ts.Ui.MenuTree.Item(this);
      e.stopPropagation();
      e.preventDefault();
      self.onBeforeSelect(mi);

      $(this).parents('.ts-menutree').find('.ui-state-default').removeClass('ui-state-default').removeClass('menutree-selected');
      $(this).children('div').addClass('ui-state-default').addClass('menutree-selected');;

      self.onAfterSelect(item);
    });
    item.setCaption(caption);
    item.setImageUrl(imageUrl);
    item.setData(data);
    return item;
  },
  onBeforeSelect: function (item) {
    this._callEvent('beforeSelect', item);
  },
  onAfterSelect: function (item) {
    this._callEvent('afterSelect', item);
  },

  find: function (id, type) {
    var item = $(this._element).find('.menutree-item-' + type + '-' + id);
    if (item.length < 1) { return null; }
    return new Ts.Ui.MenuTree.Item(item[0]);
  },
  getByIndex: function (index) {
    var items = $(this._element).find('li');
    var result = null;
    for (var i = 0; i < items.length; i++) {
      if (i == index) { return new Ts.Ui.MenuTree.Item(items[i]); }
    }
    return result;
  },
  getByID: function (id) {
    var items = $(this._element).find('li');
    var result = null;
    for (var i = 0; i < items.length; i++) {
      var item = new Ts.Ui.MenuTree.Item(items[i]);
      if (item.getId() == id) return item;
    }
    return result;
  },
  getSelected: function () {
    var result = $(this._element).find('.ui-state-default').parent();
    if (result.length < 1) return null;
    return new Ts.Ui.MenuTree.Item(result);
  },
  bind: function (event, callback) {
    this._events[event] = callback;
  }

};

Ts.Ui.MenuTree.Item = function (element) { this._element = element; }

Ts.Ui.MenuTree.Item.prototype = {
  constructor: Ts.Ui.MenuTree.Item,
  getElement: function () { return this._element; },
  getId: function () { return Ts.Utils.getNameParam('menutree-item', this._element, 0); },
  getType: function () { return Ts.Utils.getNameParam('menutree-item', this._element, 1); },
  getCaption: function () { return $(this._element).children('div').children('a').html(); },
  setCaption: function (value) { $(this._element).children('div').children('a').html(value); },
  getImageUrl: function () { return $(this._element).children('div').children('img').attr('src'); },
  setImageUrl: function (value) {
    if (value == null || value == '')
      $(this._element).children('div').children('img').removeAttr('src');
    else
      $(this._element).children('div').children('img').attr('src', value);
  },
  getIsHighlighted: function () { return $(this._element).children('div').hasClass('menutree-highlighted'); },
  setIsHighlighted: function (value) { if (value) { $(this._element).children('div').addClass('menutree-highlighted'); } else { $(this._element).children('div').removeClass('menutree-highlighted'); } },
  getIsSelected: function () { return $(this._element).children('div').hasClass('ui-state-default'); },
  getIsExpanded: function () { return $(this._element).children('div').children('.ui-icon').hasClass('ui-icon-carat-1-s'); },
  setIsExpanded: function (value) {
    var item = $(this._element).children('div').children('.ui-icon');
    if (value) {
      $(item).addClass('ui-icon-carat-1-s').removeClass('ui-icon-carat-1-e').parents('li').children('ul').show();
    }
    else {
      $(item).addClass('ui-icon-carat-1-e').removeClass('ui-icon-carat-1-s').parents('li').children('ul').hide();
      new Ts.Ui.MenuTree.Item($(item).parents('li')).select();
    }


  },
  getData: function () { return $(this._element).data('data'); },
  setData: function (value) { $(this._element).data('data', value); },
  //getData: function () { return JSON.parse($(this._element).attr('data')); },
  //setData: function (value) { $(this._element).attr('data', typeof (value) == 'string' ? value : JSON.stringify(value)); },
  select: function () { $(this._element).click(); }
};
