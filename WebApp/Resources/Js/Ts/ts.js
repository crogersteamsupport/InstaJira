/// <reference path="ts/ts.js" />
/// <reference path="ts/ts.services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="~/Default.aspx" />

var Ts = {};

(function () {
  Ts._inits = [];
  Ts._addInit = function (fn) {  Ts._inits.push(fn); };
  Ts._init = function (callback) {
    function callFn(){
      var fn = Ts._inits.shift();
      if (fn) { 
        fn(callFn); 
      }
      else {
        callback();
      }
    }
    callFn();
  };
})();


Ts.Pages = {};
Ts.Ui = {};

(function () {
  
  function initServices (callback)
  {
    Ts.Services = {};
    var oldInvoke = Sys.Net.WebServiceProxy.invoke;
    Sys.Net.WebServiceProxy.invoke = function(servicePath, methodName, useGet, params, onSuccess, onFailure, userContext, timeout)
    {
      return oldInvoke(servicePath, methodName, useGet, params, onSuccess, onFailure, userContext, timeout);
    }
    


    function defaultSucceededCallback(result) { }
    function defaultFailedCallback(error, userContext, methodName) { 
      //if (error) { alert("An error occurred: " + error.get_message()); } 
    }

    Ts.Services.System = new TSWebServices.TSSystem();
    Ts.Services.System.set_defaultSucceededCallback(defaultSucceededCallback);
    Ts.Services.System.set_defaultFailedCallback(defaultFailedCallback);

    Ts.Services.Settings = new TSWebServices.SettingService();
    Ts.Services.Settings.set_defaultSucceededCallback(defaultSucceededCallback);
    Ts.Services.Settings.set_defaultFailedCallback(defaultFailedCallback);

    Ts.Services.Tickets = new TSWebServices.TicketService();
    Ts.Services.Tickets.set_defaultSucceededCallback(defaultSucceededCallback);
    Ts.Services.Tickets.set_defaultFailedCallback(defaultFailedCallback);

    Ts.Services.Automation = new TSWebServices.AutomationService();
    Ts.Services.Automation.set_defaultSucceededCallback(defaultSucceededCallback);
    Ts.Services.Automation.set_defaultFailedCallback(defaultFailedCallback);

    Ts.Services.Users = new TSWebServices.UserService();
    Ts.Services.Users.set_defaultSucceededCallback(defaultSucceededCallback);
    Ts.Services.Users.set_defaultFailedCallback(defaultFailedCallback);

    Ts.Services.Organizations = new TSWebServices.OrganizationService();
    Ts.Services.Organizations.set_defaultSucceededCallback(defaultSucceededCallback);
    Ts.Services.Organizations.set_defaultFailedCallback(defaultFailedCallback);

    Ts.Services.Products = new TSWebServices.ProductService();
    Ts.Services.Products.set_defaultSucceededCallback(defaultSucceededCallback);
    Ts.Services.Products.set_defaultFailedCallback(defaultFailedCallback);

    callback();
  }

  Ts._addInit(initServices);
})();


function pageLoad() {
  Ts._init(function () { if (window.teamSupportLoad) {window.teamSupportLoad(); }});
}








