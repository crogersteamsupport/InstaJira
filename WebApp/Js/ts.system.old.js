/*http://www.codeproject.com/KB/scripting/jsnamespaces.aspx sss*/
var TSSystem =
{
  Register: function (_NameSpace) {
    var flag = false;
    var name = "";
    var spaces = _NameSpace.split(".");
    for (var i = 0; i < spaces.length; i++) {
      if (name != "") { name += "."; }
      name += spaces[i];
      flag = this.Exists(name);
      if (!flag) { this.Create(name); }
    }
    if (flag) { throw "Namespace: " + _NameSpace + " is already defined."; }
  },

  Create: function (_Source) {
    eval("window." + _Source + " = new Object();");
  },

  Exists: function (_Source) {
    eval("var result = false; try{if(" + _Source + "){result = true;}else{result = false;}} catch(err){result = false;}");
    return result;
  },

  Init: function () {
    loadUtils();

    loadServices();


    function loadServices() {

      var oldInvoke = Sys.Net.WebServiceProxy.invoke;
      Sys.Net.WebServiceProxy.invoke = function (servicePath, methodName, useGet, params, onSuccess, onFailure, userContext, timeout) {
        if (!params) params = {};
        params.SessionID = top.Ts.System.getSessionID();

        return oldInvoke(servicePath, methodName, useGet, params, onSuccess, onFailure, userContext, timeout);
      }

      function DefaultSucceededCallback(result) { }
      function DefaultFailedCallback(error, userContext, methodName) { }

      TSSystem.Register("TSSystem.Services.Tickets");
      TSSystem.Services.Tickets = new TSWebServices.TicketService();
      TSSystem.Services.Tickets.set_defaultSucceededCallback(DefaultSucceededCallback);
      TSSystem.Services.Tickets.set_defaultFailedCallback(DefaultFailedCallback);
    }

    function loadUtils() {
      TSSystem.Register("TSSystem.Utils");

      function Utils() { }

      Utils.prototype =
      {
        constructor: Utils,
        getQueryValue: function (name, window) {
          params = window.location.search.substring(1);
          name = name.toLowerCase();
          param = params.split("&");
          for (i = 0; i < param.length; i++) {
            value = param[i].split("=");
            if (value[0].toLowerCase() == name) { return unescape(value[1]); }
          }
          return null;
        }
      }

      TSSystem.Utils = new Utils();
    }
  }
}






