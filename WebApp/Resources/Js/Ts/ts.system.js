var Ts = {};
Ts.Pages = {};
Ts.Ui = {};
Ts.Services = {};
var _lastActivity = null;
var _startDate = new Date();

(function() {

    Ts._inits = [];
    Ts._addInit = function(o) {
        Ts._inits.push(o);
    };
    Ts._init = function(callback) {
        function callFn() {
            var o = Ts._inits.shift();
            if (o && o._init) {
                o._init(callFn);
            } else {
                callback();
            }
        }
        callFn();
    };

    function initServices(callback) {

        var oldInvoke = Sys.Net.WebServiceProxy.invoke;
        Sys.Net.WebServiceProxy.invoke = function(servicePath, methodName, useGet, params, onSuccess, onFailure, userContext, timeout) {
            if (!params) params = {};
            params._sessionID = Ts.System.getSessionID();
            var oldOnFailure = onFailure;
            return oldInvoke(servicePath, methodName, useGet, params, onSuccess, function(e) {
                    if (e._statusCode == 401) {
                        if (Ts.debug == true) {
                            var now = new Date();
                            var diffs = (now - _lastActivity) / 1000; // milliseconds
                            var diffh = Math.round(diffs / 3600); // hours
                            var diffm = Math.round((diffs % 3600) / 60); // minutes
                            var msg = encodeURIComponent("Session Start: " + _startDate.toTimeString() + "\nLast Activity: " + _lastActivity.toTimeString() + "\nNow: " + now.toTimeString() + "\n" + diffh + " hours, " + diffm + " minutes");
                            window.location = 'SessionExpired.aspx?msg=' + msg;
                            return;
                        } else {
                            window.location = 'SessionExpired.aspx';
                            return;
                        }
                    }

                    if (oldOnFailure) oldOnFailure(e);
                },
                userContext, timeout);
        }



        function defaultSucceededCallback(result) {}

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

        Ts.Services.TicketPage = new TSWebServices.TicketPageService();
        Ts.Services.TicketPage.set_defaultSucceededCallback(defaultSucceededCallback);
        Ts.Services.TicketPage.set_defaultFailedCallback(defaultFailedCallback);

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

        Ts.Services.WaterCooler = new TSWebServices.WaterCoolerService();
        Ts.Services.WaterCooler.set_defaultSucceededCallback(defaultSucceededCallback);
        Ts.Services.WaterCooler.set_defaultFailedCallback(defaultFailedCallback);

        Ts.Services.Admin = new TSWebServices.AdminService();
        Ts.Services.Admin.set_defaultSucceededCallback(defaultSucceededCallback);
        Ts.Services.Admin.set_defaultFailedCallback(defaultFailedCallback);

        Ts.Services.Assets = new TSWebServices.AssetService();
        Ts.Services.Assets.set_defaultSucceededCallback(defaultSucceededCallback);
        Ts.Services.Assets.set_defaultFailedCallback(defaultFailedCallback);

        Ts.Services.Search = new TSWebServices.SearchService();
        Ts.Services.Search.set_defaultSucceededCallback(defaultSucceededCallback);
        Ts.Services.Search.set_defaultFailedCallback(defaultFailedCallback);

        Ts.Services.Reports = new TSWebServices.ReportService();
        Ts.Services.Reports.set_defaultSucceededCallback(defaultSucceededCallback);
        Ts.Services.Reports.set_defaultFailedCallback(defaultFailedCallback);

        Ts.Services.Wiki = new TSWebServices.WikiService();
        Ts.Services.Wiki.set_defaultSucceededCallback(defaultSucceededCallback);
        Ts.Services.Wiki.set_defaultFailedCallback(defaultFailedCallback);

        Ts.Services.Customers = new TSWebServices.CustomerService();
        Ts.Services.Customers.set_defaultSucceededCallback(defaultSucceededCallback);
        Ts.Services.Customers.set_defaultFailedCallback(defaultFailedCallback);

        Ts.Services.Login = new TSWebServices.LoginService();
        Ts.Services.Login.set_defaultSucceededCallback(defaultSucceededCallback);
        Ts.Services.Login.set_defaultFailedCallback(defaultFailedCallback);

        Ts.Services.Chat = new TSWebServices.ChatService();
        Ts.Services.Chat.set_defaultSucceededCallback(defaultSucceededCallback);
        Ts.Services.Chat.set_defaultFailedCallback(defaultFailedCallback);

        Ts.Services.Task = new TSWebServices.TaskService();
        Ts.Services.Task.set_defaultSucceededCallback(defaultSucceededCallback);
        Ts.Services.Task.set_defaultFailedCallback(defaultFailedCallback);

        Ts.Services.Dispatch = new TSWebServices.DispatchService();
        Ts.Services.Dispatch.set_defaultSucceededCallback(defaultSucceededCallback);
        Ts.Services.Dispatch.set_defaultFailedCallback(defaultFailedCallback);

        Ts.Services.WatsonTickets = new TSWebServices.WatsonTickets();
        Ts.Services.WatsonTickets.set_defaultSucceededCallback(defaultSucceededCallback);
        Ts.Services.WatsonTickets.set_defaultFailedCallback(defaultFailedCallback);

        Ts.Services.Deflector = new TSWebServices.DeflectorService();
        Ts.Services.Deflector.set_defaultSucceededCallback(defaultSucceededCallback);
        Ts.Services.Deflector.set_defaultFailedCallback(defaultFailedCallback);

        callback();
    }

    function TsSystem() {
        this.User = null;
        this.Organization = null;
        this.Culture = null;
        this.ChatUserSettings = null;
        this.AppDomain = null;

        var self = this;

        this._init = function(callback) {
            initServices(function() {
                self.refreshUser(function() {
                    callback();
                    setupLogTracking();
                });
            });
        }


        function setupLogTracking() {
            Ts.Services.System.GetPendoOptions(function(options) {
                window.pendo_options = JSON.parse(options);
                (function(p, e, n, d, o) {
                    var v, w, x, y, z;
                    o = p[d] = p[d] || {};
                    o._q = [];
                    v = ['initialize', 'identify', 'updateOptions', 'pageLoad'];
                    for (w = 0, x = v.length; w < x; ++w)(function(m) {
                        o[m] = o[m] || function() {
                            o._q[m === v[0] ? 'unshift' : 'push']([m].concat([].slice.call(arguments, 0)));
                        };
                    })(v[w]);
                    y = e.createElement(n);
                    y.async = !0;
                    y.src = 'https://cdn.pendo.io/agent/static/6af64640-2a96-4767-4bd8-f480c3f1ac37/pendo.js';
                    z = e.getElementsByTagName(n)[0];
                    z.parentNode.insertBefore(y, z);
                })(window, document, 'script', 'pendo');
                if (pendo) pendo.initialize(window.pendo_options);
            });
        }

    }

    TsSystem.prototype = {
        constructor: TsSystem,
        refreshUser: function(callback) {
            var self = this;
            Ts.Services.System.GetCurrentUser(function(result) {
                self.User = result;
                Ts.Services.System.GetCurrentUserGroups(function(result) {
                    self.UserGroups = result;
                    Ts.Services.System.GetCurrentOrganization(function(result) {
                        self.Organization = result;
                        Ts.debug = result.OrganizationID == 13679 || result.OrganizationID == 1078 || result.OrganizationID == 1088;
                        Ts.Services.System.GetCulture(function(result) {
                            self.Culture = result;
                            Ts.Services.System.GetCurrentUserChatSettings(function(result) {
                                self.ChatUserSettings = result;
                                Ts.Services.System.GetDomains(function(result) {
                                    var domains = JSON.parse(result);
                                    self.AppDomain = domains.AppUrl;
                                    self.PortalDomain = domains.PortalUrl;
                                    self.Domain = domains.DomainName;
                                    self.EmailDomain = domains.EmailDomain;
                                    if (callback) {
                                        callback(self.User);
                                    }
                                });
                            });
                        });
                    });
                });
            });
        },

        openSupport: function() {
            var encrypted = CryptoJS.AES.encrypt(Date.now() + "," + this.User.Email + "", "57ee1f58-5c8b-4b47-b629-be7c702a2022");
            window.open("https://support.teamsupport.com/sso/" + encrypted);
            //To test Dev
            //window.open("http://teamsupport.na1.teamsupport.com:90/sso/" + encrypted);
        },

        signOut: function(callback) {
            Ts.Services.System.SignOut(function() {
                window.location = window.location;
            });
        },


        getSessionID: function() {
            return $('#fieldSID').val();
        },
        logAction: function(action, customData) {
            //if (_aaq == null) return;    _aaq.push(['trackAction', action, customData]);
        }

    };

    Ts.System = new TsSystem();
    Ts._addInit(Ts.System);


    function TsUserSettings() {
        this._cache = new Array();
    }
    TsUserSettings.prototype = {
        constructor: TsUserSettings,
        read: function(key, defaultValue, callback) {
            var cached = this._cache[key];
            if (cached != undefined) {
                callback(cached);
                return;
            }
            var self = this;
            Ts.Services.Settings.ReadUserSetting(key, defaultValue, function(result) {
                self._cache[key] = result;
                callback(result);
            });
        },
        write: function(key, value, callback) {
            var cached = this._cache[key];
            if (cached != undefined && cached == value) {
                if (callback) callback();
                return;
            }
            this._cache[key] = value;
            Ts.Services.Settings.WriteUserSetting(key, value, callback);
        }
    };

    function TsOrgSettings() {
        this._cache = new Array();
    }
    TsOrgSettings.prototype = {
        constructor: TsOrgSettings,
        read: function(key, defaultValue, callback) {
            var cached = this._cache[key];
            if (cached != undefined) {
                callback(cached);
                return;
            }
            var self = this;
            Ts.Services.Settings.ReadOrganizationSetting(key, defaultValue, function(result) {
                self._cache[key] = result;
                callback(result);
            });
        },
        write: function(key, value, callback) {
            var cached = this._cache[key];
            if (cached != undefined && cached == value) {
                if (callback) callback();
                return;
            }
            this._cache[key] = value;
            Ts.Services.Settings.WriteOrganizationSetting(key, value, callback);
        }
    };

    function TsSessionSettings() {
        this._cache = new Array();
    }
    TsSessionSettings.prototype = {
        constructor: TsSessionSettings,
        read: function(key, defaultValue, callback) {
            var cached = this._cache[key];
            if (cached != undefined) {
                callback(cached);
                return;
            }
            var self = this;
            Ts.Services.Settings.ReadSessionSetting(key, defaultValue, function(result) {
                self._cache[key] = result;
                callback(result);
            });
        },
        write: function(key, value, callback) {
            var cached = this._cache[key];
            if (cached != undefined && cached == value) {
                if (callback) callback();
                return;
            }
            this._cache[key] = value;
            Ts.Services.Settings.WriteSessionSetting(key, value, callback);
        }
    };

    function TsSystemSettings() {
        this._cache = new Array();
    }
    TsSystemSettings.prototype = {
        constructor: TsSystemSettings,
        read: function(key, defaultValue, callback) {
            var cached = this._cache[key];
            if (cached != undefined) {
                callback(cached);
                return;
            }
            var self = this;
            Ts.Services.Settings.ReadSystemSetting(key, defaultValue, function(result) {
                self._cache[key] = result;
                callback(result);
            });
        },
        write: function(key, value, callback) {
            var cached = this._cache[key];
            if (cached != undefined && cached == value) {
                if (callback) callback();
                return;
            }
            this._cache[key] = value;
            Ts.Services.Settings.WriteSystemSetting(key, value, callback);
        }
    };

    Ts.Settings = {}
    Ts.Settings.User = new TsUserSettings();
    Ts.Settings.Organization = new TsOrgSettings();
    Ts.Settings.Session = new TsSessionSettings();
    Ts.Settings.System = new TsSystemSettings();
    Ts.Settings.clearCache = function() {
        Ts.Settings.User._cache = new Array();
        Ts.Settings.Organization._cache = new Array();
        Ts.Settings.Session._cache = new Array();
        Ts.Settings.System._cache = new Array();
    }

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
    Ts.ReferenceTypes.ForumCategories = 35;
    Ts.ReferenceTypes.KnowledgeBaseCategories = 42;
    Ts.ReferenceTypes.ProductFamilies = 44;
    Ts.ReferenceTypes.UserProducts = 46;
    Ts.ReferenceTypes.CompanyActivity = 63;
    Ts.ReferenceTypes.ContactActivity = 64;

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
    Ts.CustomFieldType.Date = 5;
    Ts.CustomFieldType.Time = 6;

    Ts.ProductType = {}
    Ts.ProductType.Express = 0;
    Ts.ProductType.HelpDesk = 1;
    Ts.ProductType.Enterprise = 2;
    Ts.ProductType.BugTracking = 3;

    Ts.Pusher = null;

    Ts.TicketViewing = 0;
})();

function pageLoad() {
    Ts._init(function() {
        if (window.teamSupportLoad) {
            window.teamSupportLoad();
        }
    });
}
