﻿var Ts = {};
Ts.Pages = {};
Ts.Ui = {};
Ts.Services = {};

(function () {

  Ts._inits = [];
  Ts._addInit = function (o) {  Ts._inits.push(o); };
  Ts._init = function (callback) {
    function callFn(){
      var o = Ts._inits.shift();
      if (o && o._init) { 
        o._init(callFn); 
      }
      else {
        callback();
      }
    }
    callFn();
  };

  function initServices (callback)
  {

    var oldInvoke = Sys.Net.WebServiceProxy.invoke;
    Sys.Net.WebServiceProxy.invoke = function(servicePath, methodName, useGet, params, onSuccess, onFailure, userContext, timeout)
    {
      if (!params) params = {};
      params._sessionID = Ts.System.getSessionID();

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

    Ts.Services.CustomFields = new TSWebServices.CustomFieldsService();
    Ts.Services.CustomFields.set_defaultSucceededCallback(defaultSucceededCallback);
    Ts.Services.CustomFields.set_defaultFailedCallback(defaultFailedCallback);
    callback();
  }

  function TsSystem() {
    this.User = null;
    this.Organization = null;
    this.Culture = null;
    this.ChatUserSettings = null;
    this.AppDomain = null;
    //this.TicketTypes = null;

    var self = this;

    this._init = function (callback) {
      initServices(function(){
        self.refreshUser(callback);
        /*Ts.Services.Tickets.GetTicketTypes(function (result) {
          self.TicketTypes = result;
          self.refreshUser(callback);
        });*/
      });
    }
  }

  TsSystem.prototype =
  {
    constructor: TsSystem,
    refreshUser: function (callback) {
      var self = this;
      Ts.Services.System.GetCurrentUser(function (result) {
        self.User = result;
        Ts.Services.System.GetCurrentOrganization(function (result) {
          self.Organization = result;
          Ts.debug = result.OrganizationID == 13679 || result.OrganizationID == 1078 || result.OrganizationID == 1088;
          Ts.Services.System.GetCulture(function(result){
            self.Culture = result;
            Ts.Services.System.GetCurrentUserChatSettings(function (result) {
              self.ChatUserSettings = result;
              Ts.Services.System.GetAppDomain(function(result) { 
                self.AppDomain = result;
                if (callback) { callback(self.User); }
              });
            });
          });
        });
      });
    },

    openSupport: function () {
      window.open("http://www.teamsupport.com/customer_portal_login.php?OrganizationID=1078&UserName=" + this.User.Email + "&Password=57EE1F58-5C8B-4B47-B629-BE7C702A2022", "TSPortal");
    },

    signOut: function (callback) {
      Ts.Services.System.SignOut(function () { window.location = window.location; });
    },


    getSessionID: function () { return $('#fieldSID').val(); }
  };

  Ts.System = new TsSystem();
  Ts._addInit(Ts.System);

  Ts.ReferenceTypes = {}
  Ts.ReferenceTypes.None = -1;
  Ts.ReferenceTypes.Actions = 0; 
  Ts.ReferenceTypes.ActionTypes = 1; 
  Ts.ReferenceTypes.Addresses = 2; 
  Ts.ReferenceTypes.Attachments = 3; 
  Ts.ReferenceTypes.CustomFields = 4; 
  Ts.ReferenceTypes.CustomValues = 5;
  Ts.ReferenceTypes.Groups = 6; 
  Ts.ReferenceTypes.GroupUsers = 7; 
  Ts.ReferenceTypes.OrganizationProducts = 8; 
  Ts.ReferenceTypes.Organizations = 9; 
  Ts.ReferenceTypes.OrganizationTickets = 10;
  Ts.ReferenceTypes.PhoneNumbers = 11; 
  Ts.ReferenceTypes.PhoneTypes = 12; 
  Ts.ReferenceTypes.Products = 13; 
  Ts.ReferenceTypes.ProductVersions = 14; 
  Ts.ReferenceTypes.ProductVersionStatuses = 15;
  Ts.ReferenceTypes.TechDocs = 16; 
  Ts.ReferenceTypes.Tickets = 17; 
  Ts.ReferenceTypes.TicketSeverities = 18; 
  Ts.ReferenceTypes.TicketStatuses = 19; 
  Ts.ReferenceTypes.Subscriptions = 20; 
  Ts.ReferenceTypes.TicketTypes = 21; 
  Ts.ReferenceTypes.Users = 22; 
  Ts.ReferenceTypes.ActionLogs = 23; 
  Ts.ReferenceTypes.BillingInfo = 24; 
  Ts.ReferenceTypes.ExceptionLogs = 25; 
  Ts.ReferenceTypes.Invoices = 26; 
  Ts.ReferenceTypes.SystemSettings = 27; 
  Ts.ReferenceTypes.TicketNextStatuses = 28; 
  Ts.ReferenceTypes.UserSettings = 29; 
  Ts.ReferenceTypes.TicketQueue = 30;
  Ts.ReferenceTypes.CreditCards = 31;
  Ts.ReferenceTypes.Contacts = 32;
  Ts.ReferenceTypes.Chat = 33;
  Ts.ReferenceTypes.Assets = 34;
  Ts.ReferenceTypes.EmailPost = 35;

  Ts.SystemActionTypes = {}
  Ts.SystemActionTypes.Custom = 0;
  Ts.SystemActionTypes.Description = 1;
  Ts.SystemActionTypes.Resolution = 2;
  Ts.SystemActionTypes.Email = 3;
  Ts.SystemActionTypes.PingUpdate = 4;
  Ts.SystemActionTypes.Chat = 5;

  Ts.CustomFieldType = {}
  Ts.CustomFieldType.Text = 0;
  Ts.CustomFieldType.DateTime = 1;
  Ts.CustomFieldType.Boolean = 2;
  Ts.CustomFieldType.Number = 3;
  Ts.CustomFieldType.PickList = 4;


})();



function pageLoad() {
  Ts._init(function () { if (window.teamSupportLoad) {window.teamSupportLoad(); }});
}
