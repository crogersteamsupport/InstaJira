using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using TeamSupport.Data;

namespace TeamSupport.Handlers
{
    public class TSHubSpot
    {
        public static void HubspotPost(string fname, string lname, string email, string company, string phone, string promo, string seats, string source, string utkCookie, ProductType version)
        {

            Dictionary<string, string> dictFormValues = new Dictionary<string, string>();
            dictFormValues.Add("firstname", fname);
            dictFormValues.Add("lastname", lname);
            dictFormValues.Add("email", email);
            dictFormValues.Add("phone", phone);
            dictFormValues.Add("company", company);
            dictFormValues.Add("campaign", promo);
            dictFormValues.Add("marketingsource", source);
            dictFormValues.Add("lifecyclestage", "salesqualifiedlead");
            dictFormValues.Add("type_of_sql", "Trial");
            int numSeats = 0;
            int.TryParse(seats, out numSeats);
            dictFormValues.Add("no_of_users", numSeats.ToString());

            
            //dictFormValues.Add("recent_conversion_event_name", "TS Trial Sign Up");
            dictFormValues.Add("product_edition", GetProductVersionName(version));
            //dictFormValues.Add("hubspot_owner_id", GetSalesGuyHubSpotID(salesGuy));

            int intPortalID = 448936;
            string strFormGUID = "0ddd21dd-ed3a-4282-afc8-26707a31d04e";

            // Tracking Code Variables
            string strHubSpotUTK = utkCookie;
            string strIpAddress = System.Web.HttpContext.Current.Request.UserHostAddress;

            // Page Variables
            string strPageTitle = "TS";
            string strPageURL = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;

            // Do the post, returns true/false
            string strError = "";
            bool blnRet = Do_Post_To_HubSpot_FormsAPI(intPortalID, strFormGUID, dictFormValues, strHubSpotUTK, strIpAddress, strPageTitle, strPageURL, ref strError);
            if (!blnRet)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, new Exception("Error Posting To HUB SPOT"), "HUB SPOT", dictFormValues.ToDebugString());
            }

        }
        private static bool Do_Post_To_HubSpot_FormsAPI(int intPortalID, string strFormGUID, Dictionary<string, string> dictFormValues, string strHubSpotUTK, string strIpAddress, string strPageTitle, string strPageURL, ref string strMessage)
        {

            // Build Endpoint URL
            string strEndpointURL = string.Format("https://forms.hubspot.com/uploads/form/v2/{0}/{1}", intPortalID, strFormGUID);

            // Setup HS Context Object
            Dictionary<string, string> hsContext = new Dictionary<string, string>();
            hsContext.Add("hutk", strHubSpotUTK);
            hsContext.Add("ipAddress", strIpAddress);
            hsContext.Add("pageUrl", strPageURL);
            hsContext.Add("pageName", strPageTitle);

            // Serialize HS Context to JSON (string)
            System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
            string strHubSpotContextJSON = json.Serialize(hsContext);

            // Create string with post data
            string strPostData = "";

            // Add dictionary values
            foreach (var d in dictFormValues)
            {
                strPostData += d.Key + "=" + HttpUtility.UrlEncode(d.Value) + "&";
            }

            // Append HS Context JSON
            strPostData += "hs_context=" + HttpUtility.UrlEncode(strHubSpotContextJSON);

            // Create web request object
            System.Net.HttpWebRequest r = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(strEndpointURL);

            // Set headers for POST
            r.Method = "POST";
            r.ContentType = "application/x-www-form-urlencoded";
            r.ContentLength = strPostData.Length;
            r.KeepAlive = false;

            // POST data to endpoint
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(r.GetRequestStream()))
            {
                try
                {
                    sw.Write(strPostData);
                }
                catch (Exception ex)
                {
                    // POST Request Failed
                    strMessage = ex.Message;
                    return false;
                }
            }

            return true; //POST Succeeded

        }
        private static string GetProductVersionName(ProductType productVersion)
        {
            string rtnValue = "";
            switch (productVersion)
            {
                case ProductType.Enterprise:
                    rtnValue = "Enterprise";
                    break;
                case ProductType.BugTracking:
                    rtnValue = "Bug Tracking";
                    break;
                case ProductType.Express:
                    rtnValue = "Express";
                    break;
                case ProductType.HelpDesk:
                    rtnValue = "Help Desk";
                    break;
                default:
                    rtnValue = "Unknown";
                    break;
            }
            return rtnValue;
        }

        /*
         * private static string GetSalesGuyHubSpotID(string salesGuy)
        {
            string rtnValue = "";
            switch(salesGuy.Trim().ToLower())
            {
                case "leon":
                    rtnValue = "2396497";
                    break;
                case "jesus":
                    rtnValue = "2396502";
                    break;
                default:
                    rtnValue = "";
                    break;
            }

            return rtnValue;
        }
        */
    }

    public static class ExtensionMethods
    {
        public static string ToDebugString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return "{" + string.Join(",", dictionary.Select(kv => kv.Key.ToString() + "=" + kv.Value.ToString()).ToArray()) + "}";
        }
    }

   
}
