using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Runtime.Serialization;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Diagnostics;
using ImageResizer;
using System.Net;
using System.IO;
using System.Dynamic;
using System.Text.RegularExpressions;

namespace TSWebServices {

    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class WatsonTickets : System.Web.Services.WebService {

        [WebMethod]
        public string Summary (int ticketID = 0) {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            string json = WatsonScores.PullSummary(loginUser, ticketID);
            return json;
        }

        [WebMethod]
        public string Ticket (int ticketID) {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            string json = WatsonScores.PullTicket(loginUser, ticketID);
            return (json != "nothing" && json != "negative") ? json : "negative";
        }

        [WebMethod]
        public string Action (int ticketID, int actionID) {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            string json = WatsonScores.PullAction(loginUser, ticketID, actionID);
            return (json != "nothing" && json != "negative") ? json : "negative";
        }


    }
}
