/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
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

  $('button').button();

  $('a').addClass('ui-state-default ts-link');
  $('.loading-section').hide().next().show();
};


AdminPortal.prototype = {
  constructor: AdminPortal,
  refresh: function () {

  }
};
