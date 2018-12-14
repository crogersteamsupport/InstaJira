// <reference path="ts/ts.services.js" />
// <reference path="ts/ts.system.js" />
// <reference path="ts/ts.utils.js" />
// <reference path="~/Default.aspx" />
(function() {
    function TsCache() {
        this._users = null;
        this._groups = null;
        this._products = null;
        this._productFamilies = null;
        this._productVersionStatuses = null;
        this._ticketTypes = null;
        this._ticketStatuses = null;
        this._ticketNextStatuses = null;
        this._ticketSeverities = null;
        this._ticketgroups = null;
        this._forumCategories = null;
        this._actionTypes = null;
        this._timeZones = null;
        this._cultures = null;
        this._knowledgeBaseCategories = null;
        this._isJiraLinkActive = null;
        this._isJiraLinkActiveForOrganization = null;
        this._fontFamilies = null;
        this._fontSizes = null;
        function getMainFrame(wnd) {
            if (!wnd) wnd = window;
            var result = wnd;
            var cnt = 0;
            while (!(result.Ts && result.Ts.Services)) {
                result = result.parent;
                cnt++;
                if (cnt > 5) return null;
            }
            return result;
        }
        this._mainFrame = getMainFrame();
    }
    TsCache.prototype = {
        constructor: TsCache,
        _init: function(callback) {
            this.checkAll();
            callback();
        },
        checkAll: function() {
            this.getUsers();
            this.getGroups();
            this.getTicketGroups();
            this.getProducts();
            this.getProductVersionStatuses();
            this.getTicketNextStatuses();
            this.getTicketSeverities();
            this.getTicketStatuses();
            this.getTicketTypes();
            this.getActionTypes();
            this.getForumCategories();
            this.getTimeZones();
            this.getCultures();
            this.getKnowledgeBaseCategories();
            this.getFontFamilies();
            this.getFontSizes();
            this.getProductFamilies();
        },
        getUsers: function() {
            var self = this;
            Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.Users, function(checksum) {
                if (!self._users || !self._users.CheckSum || checksum != self._users.CheckSum) {
                    Ts.Services.Users.GetUsers(function(result) {
                        self._users = result;
                        self._users.CheckSum = checksum;
                    });
                }
            });
            return self._users;
        },
        getGroups: function() {
            var self = this;
            Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.Groups, function(checksum) {
                if (!self._groups || !self._groups.CheckSum || checksum != self._groups.CheckSum) {
                    Ts.Services.Users.GetGroups(function(result) {
                        self._groups = result;
                        self._groups.CheckSum = checksum;
                    });
                }
            });
            return self._groups;
        },
        getTicketGroups: function() {
            var self = this;
            Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.Groups, function(checksum) {
                if (!self._ticketgroups || !self._ticketgroups.CheckSum || checksum != self._ticketgroups.CheckSum) {
                    Ts.Services.Users.GetTicketGroups(function(result) {
                        self._ticketgroups = result;
                        self._ticketgroups.CheckSum = checksum;
                    });
                }
            });
            return self._ticketgroups;
        },
        getTicketTypes: function() {
            var self = this;
            Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.TicketTypes, function(checksum) {
                if (!self._ticketTypes || !self._ticketTypes.CheckSum || checksum != self._ticketTypes.CheckSum) {
                    Ts.Services.Tickets.GetTicketTypes(function(result) {
                        self._ticketTypes = result;
                        self._ticketTypes.CheckSum = checksum;
                    });
                }
            });
            return self._ticketTypes;
        },
        getTicketStatuses: function() {
            var self = this;
            if (!self._ticketStatuses) {
                Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.TicketStatuses, function(checksum) {
                    if (!self._ticketStatuses || !self._ticketStatuses.CheckSum || checksum != self._ticketStatuses.CheckSum) {
                        Ts.Services.Tickets.GetTicketStatuses(function(result) {
                            self._ticketStatuses = result;
                            self._ticketStatuses.CheckSum = checksum;
                        });
                    }
                });
            }
            return self._ticketStatuses;
        },
        getTicketNextStatuses: function() {
            var self = this;
            Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.TicketNextStatuses, function(checksum) {
                if (!self._ticketNextStatuses || !self._ticketNextStatuses.CheckSum || checksum != self._ticketNextStatuses.CheckSum) {
                    Ts.Services.Tickets.GetNextTicketStatuses(function(result) {
                        self._ticketNextStatuses = result;
                        self._ticketNextStatuses.CheckSum = checksum;
                    });
                }
            });
            return self._ticketNextStatuses;
        },

        getNextStatuses: function(ticketStatusID) {
            var next = this.getTicketNextStatuses();
            var statuses = this.getTicketStatuses();
            var result = [];
            result.push(this.getTicketStatus(ticketStatusID));

            var self = this;
            if (next) {
                for (var i = 0; i < next.length; i++) {
                    if (next[i].CurrentStatusID == ticketStatusID) {
                        var status = this.getTicketStatus(next[i].NextStatusID);
                        if (status !== null) result.push(status);
                    }
                }
            } else {
                var attempts = 0;
                var intvl = setInterval(function() {
                    if (self._ticketNextStatuses) {
                        clearInterval(intvl);
                        for (var i = 0; i < self._ticketNextStatuses.length; i++) {
                            if (self._ticketNextStatuses[i].CurrentStatusID == ticketStatusID) {
                                var status = this.getTicketStatus(self._ticketNextStatuse[i].NextStatusID);
                                if (status !== null) result.push(status);
                            }
                        }
                    } else {
                        if (attempts == 3) {
                            //alert('An error getting the ticket statuses has ocurred. Please try again later.');
                            clearInterval(intvl);
                        } else {
                            attempts += 1;
                        }
                    }
                }, 250);
            }
            if (result.length < 2) return this.getTicketStatuses();
            return result;
        },
        getTicketStatus: function(ticketStatusID) {
            var statuses = this.getTicketStatuses();
            var self = this;
            if (statuses) {
                for (var i = 0; i < statuses.length; i++) {
                    if (statuses[i].TicketStatusID == ticketStatusID) {
                        return statuses[i];
                    }
                }
            } else {
                var attempts = 0;
                var intvl = setInterval(function() {
                    if (self._ticketStatuses) {
                        clearInterval(intvl);
                        for (var i = 0; i < self._ticketStatuses.length; i++) {
                            if (self._ticketStatuses[i].TicketStatusID == ticketStatusID) {
                                return self._ticketStatuses[i];
                            }
                        }
                    } else {
                        if (attempts == 3) {
                            //alert('An error getting the ticket statuses has ocurred. Please try again later.');
                            clearInterval(intvl);
                            return null;
                        } else {
                            attempts += 1;
                        }
                    }
                }, 250);
            }
        },
        getTicketSeverities: function() {
            var self = this;
            Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.TicketSeverities, function(checksum) {
                if (!self._ticketSeverities || !self._ticketSeverities.CheckSum || checksum != self._ticketSeverities.CheckSum) {
                    Ts.Services.Tickets.GetTicketSeverities(function(result) {
                        self._ticketSeverities = result;
                        self._ticketSeverities.CheckSum = checksum;
                    });
                }
            });
            return self._ticketSeverities;
        },
        getKnowledgeBaseCategories: function() {
            var self = this;
            Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.KnowledgeBaseCategories, function(checksum) {
                if (!self._knowledgeBaseCategories || !self._knowledgeBaseCategories.CheckSum || checksum != self._knowledgeBaseCategories.CheckSum) {
                    Ts.Services.Admin.GetKnowledgeBaseCategories(function(result) {
                        self._knowledgeBaseCategories = result;
                        self._knowledgeBaseCategories.CheckSum = checksum;
                    });
                }
            });
            return self._knowledgeBaseCategories;
        },
        getForumCategories: function() {
            var self = this;
            Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.ForumCategories, function(checksum) {
                if (!self._ticketSeverities || !self._forumCategories.CheckSum || checksum != self._forumCategories.CheckSum) {
                    Ts.Services.Admin.GetForumCategories(function(result) {
                        self._forumCategories = result;
                        self._forumCategories.CheckSum = checksum;
                    });
                }
            });
            return self._forumCategories;
        },
        getProducts: function() {
            var self = this;
            Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.ProductVersions, function(checksum) {
                if (!self._products || !self._products.CheckSum || checksum != self._products.CheckSum) {
                    Ts.Services.Products.GetProducts(function(result) {
                        self._products = result;
                        self._products.CheckSum = checksum;
                    });
                }
            });
            return self._products;
        },
        getProduct: function(productID) {
            var products = this.getProducts();
            if (typeof products !== "undefined") {
                for (var i = 0; i < products.length; i++) {
                    if (products[i].ProductID == productID) {
                        return products[i];
                    }
                }
            }
            return null;
        },
        getProductVersionStatuses: function() {
            var self = this;
            Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.ProductVersionStatuses, function(checksum) {
                if (!self._productVersionStatuses || !self._productVersionStatuses.CheckSum || checksum != self._productVersionStatuses.CheckSum) {
                    Ts.Services.Products.GetProductVersionStatuses(function(result) {
                        self._productVersionStatuses = result;
                        self._productVersionStatuses.CheckSum = checksum;
                    });
                }
            });
            return self._productVersionStatuses;
        },
        getActionTypes: function() {
            var self = this;
            Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.ActionTypes, function(checksum) {
                if (!self._actionTypes || !self._actionTypes.CheckSum || checksum != self._actionTypes.CheckSum) {
                    Ts.Services.Tickets.GetActionTypes(function(result) {
                        self._actionTypes = result;
                        self._actionTypes.CheckSum = checksum;
                    });
                }
            });
            return self._actionTypes;
        },
        getTimeZones: function() {
            var self = this;
            this._mainFrame.Ts.Services.Users.GetTimezone(function(result) {
                self._timeZones = result;
            });
            return self._timeZones;
        },
        getCultures: function() {
            var self = this;
            this._mainFrame.Ts.Services.Users.GetCultures(function(result) {
                self._cultures = result;
            });
            return self._cultures;
        },
        getIsJiraLinkActiveForTicket: function(ticketId) {
            var self = this;
            if (self._isJiraLinkActive == null) {
                Ts.Services.Admin.GetIsJiraLinkActiveForTicket(ticketId, function(result) {
                    self._isJiraLinkActive = result;
                });
            }
            return self._isJiraLinkActive
        },
        GetIsJiraLinkActiveForOrganization: function() {
            var self = this;
            if (self._isJiraLinkActiveForOrganization == null) {
                Ts.Services.Admin.GetIsJiraLinkActiveForOrganization(function(result) {
                    self._isJiraLinkActiveForOrganization = result;
                });
            }
            return self._isJiraLinkActiveForOrganization
        },
        getFontFamilies: function() {
            var self = this;
            if (self._fontFamilies == null) {
                Ts.Services.System.GetFontFamilies(function(result) {
                    self._fontFamilies = result;
                });
            }
            return self._fontFamilies
        },
        getFontSizes: function() {
            var self = this;
            if (self._fontSizes == null) {
                Ts.Services.System.GetFontSizes(function(result) {
                    self._fontSizes = result;
                });
            }
            return self._fontSizes
        },
        getProductFamilies: function() {
            var self = this;
            Ts.Services.System.GetCheckSum(Ts.ReferenceTypes.ProductFamilies, function(checksum) {
                if (!self._productFamilies || !self._productFamilies.CheckSum || checksum != self._productFamilies.CheckSum) {
                    Ts.Services.Products.GetProductFamilies(function(result) {
                        self._productFamilies = result;
                        self._productFamilies.CheckSum = checksum;
                    });
                }
            });
            return self._productFamilies;
        }
    };
    Ts.Cache = new TsCache();
    Ts._addInit(Ts.Cache);
})();
