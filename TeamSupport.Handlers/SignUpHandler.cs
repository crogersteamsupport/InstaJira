using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Net;
using System.Web.SessionState;
using System.Drawing;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Security;


namespace TeamSupport.Handlers
{

    public class SignUpHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        /// <summary>
        /// Processes HTTP web requests directed to this HttpHandler.
        /// </summary>
        /// <param name="context">An HttpContext object that provides references
        /// to the intrinsic server objects (for example, Request, Response,
        /// Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
            //http://trunk.tsdev.com/signup/validateCompany?name=Muroc%20Systems,%20Inc.
            string fn = context.Request.Url.Segments[2].ToLower();
            if (fn == "fn/")
            {
                string segment = context.Request.Url.Segments[3].ToLower();
                context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                context.Response.AddHeader("Expires", "-1");
                context.Response.AddHeader("Pragma", "no-cache");


                if (segment == "validatecompany")
                {
                    ValidateCompany(context);
                }
                else if (segment == "syncuser")
                {
                    SyncUser(context);
                }
                else if (segment == "syncuseremail")
                {
                    SyncUserEmail(context);
                }
                else if (segment == "syncneworg")
                {
                    SyncNewOrg(context);
                }
                else if (segment == "syncorg")
                {
                    SyncOrg(context);
                }
                else if (segment == "post")
                {
                    try
                    {
                        using (Stream receiveStream = context.Request.InputStream)
                        {
                            using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                            {
                                string requestContent = readStream.ReadToEnd();
                                NameValueCollection values = HttpUtility.ParseQueryString(requestContent);
                                try
                                {
                                    User user = ProcessSignUp(context, values);
                                    string url = string.Format("{0}://{1}/{2}?userid={3}", context.Request.UrlReferrer.Scheme, context.Request.UrlReferrer.Host, GetValueString(values["success"]), user.UserID.ToString());
                                    context.Response.Redirect(url, false);

                                }
                                catch (Exception ex)
                                {
                                    ExceptionLogs.LogException(LoginUser.Anonymous, ex, "SIGN UP", requestContent);
                                    context.Response.Redirect(GetErrorUrl(context, values), false);
                                }
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                        ExceptionLogs.LogException(LoginUser.Anonymous, ex2, "SIGN UP");
                        context.Response.Redirect(GetErrorUrl(context), false);
                    }
                }
            }
        }

        private static string GetErrorUrl(HttpContext context, NameValueCollection values = null)
        {
            UriBuilder builder = new UriBuilder(context.Request.UrlReferrer.AbsoluteUri);
            NameValueCollection query = HttpUtility.ParseQueryString(builder.Query);
            if (values != null)
            {
                UpdateParam(query, values, "name");
                UpdateParam(query, values, "email");
                UpdateParam(query, values, "company");
                UpdateParam(query, values, "phone");
                UpdateParam(query, values, "product");
                UpdateParam(query, values, "promo");
            }
            query["suerror"] = "1";

            builder.Query = HttpUtility.UrlPathEncode(HttpUtility.UrlDecode(query.ToString()));
            return builder.ToString();
        }

        private static void UpdateParam(NameValueCollection query, NameValueCollection values, string param)
        {
            string value = GetValueString(values[param]);
            string key = "_" + param;
            if (string.IsNullOrWhiteSpace(value))
            {
                query.Remove(key);
            }
            else
            {
                query[key] = value;
            }
        }

        private static void ValidateCompany(HttpContext context)
        {

            context.Response.ContentType = "application/json; charset=utf-8";
            NameValueCollection values = context.Request.QueryString;
            context.Response.Write(string.Format("{0}({{\"isValid\": {1}}})", values["callback"], IsCompanyValid(values["name"]) ? "true" : "false"));

        }

        private static User ProcessSignUp(HttpContext context, NameValueCollection values)
        {
            string name = GetValueString(values["name"]);
            string email = GetValueString(values["email"]);
            string company = GetValueString(values["company"]);
            string phone = GetValueString(values["phone"]);
            string password = GetValueString(values["password"]);
            string promo = GetValueString(values["promo"]);
            string product = GetValueString(values["product"]);
            string eval = GetValueString(values["eval"]);
            string seats = GetValueString(values["seats"]);
            string source = "";

            int version = (int)ProductType.Enterprise;
            if (product != "")
            {
                if (int.TryParse(product, out version))
                {
                    if (version != (int)ProductType.Enterprise && version != (int)ProductType.HelpDesk)
                    {
                        version = (int)ProductType.Enterprise;
                    }
                }
                else
                {
                    if (product.ToLower().Trim() == "support desk") version = (int)ProductType.HelpDesk;
                }
            }


            if (IsCompanyValid(company))
            {

                string[] names = name.Split(' ');
                string fname = names[0];
                string lname = string.Join(" ", names.Skip(1).ToArray());
                HttpCookieCollection cookies = context.Request.Cookies;
                SignUpParams prams = new SignUpParams();
                if (cookies["_tsm"] != null)
                {

                    try
                    {
                        MarketingCookie mc = JsonConvert.DeserializeObject<MarketingCookie>(HttpUtility.UrlDecode(cookies["_tsm"].Value));
                        prams.utmCampaign = mc.Campaign;
                        prams.utmContent = mc.Content;
                        prams.utmMedium = mc.Medium;
                        prams.utmSource = mc.Source;
                        prams.utmTerm = mc.Term;
                        source = (source.Trim().Length > 0) ? source : mc.Source;
                    }
                    catch (Exception)
                    {
                        prams.utmCampaign = "Error";
                    }
                }

                if (cookies["_tsmi"] != null)
                {

                    try
                    {
                        MarketingCookie mc = JsonConvert.DeserializeObject<MarketingCookie>(HttpUtility.UrlDecode(cookies["_tsmi"].Value));
                        prams.initialCampaign = mc.Campaign;
                        prams.initialContent = mc.Content;
                        prams.initialMedium = mc.Medium;
                        prams.initialSource = mc.Source;
                        prams.initialTerm = mc.Term;
                        source = (source.Trim().Length > 0) ? source : mc.Source;
                    }
                    catch (Exception)
                    {
                        prams.initialCampaign = "Error";
                    }
                }

                if (cookies["__utmz"] != null)
                {
                    try
                    {
                        string utmz = cookies["__utmz"].Value;
                        //string utmz = "252527244.1405639771.1.1.utmcsr=GetApp|utmccn=GetApp|utmcmd=cpc";
                        prams.gaCampaign = parseGAString(utmz, "utmccn");
                        prams.gaContent = parseGAString(utmz, "utmcct");
                        prams.gaTerm = parseGAString(utmz, "utmctr");
                        prams.gaMedium = parseGAString(utmz, "utmcmd");
                        prams.gaSource = parseGAString(utmz, "utmcsr");
                        source = (source.Trim().Length > 0) ? source : parseGAString(utmz, "utmcsr");
                        /*
                        if (parseGAString(utmz, "utmgclid") != "")
                        {
                          prams.gaSource = "AdWords";
                          prams.gaMedium = "cpc";
                        }
                         */

                    }
                    catch (Exception)
                    {

                    }
                }

                if (cookies["__utma"] != null)
                {
                    try
                    {
                        string utma = cookies["__utma"].Value;
                        //string utma = "252527244.1199382232.1405639771.1405639771.1405639771.1";
                        string[] sessionValues = utma.Split('.');
                        prams.gaVisits = int.Parse(sessionValues[5]);
                    }
                    catch (Exception)
                    {

                    }
                }
                prams.promo = promo;
                prams.hubspotutk = cookies["hubspotutk"] != null ? cookies["hubspotutk"].Value : "";
                prams.source = source;
                return Organizations.SetupNewAccount(fname, lname, email, password, company, phone, eval, seats, (ProductType)version, prams, context.Request.Url.OriginalString, context.Request.UrlReferrer.OriginalString);
            }
            else
            {
                throw new Exception("Invalid Company: " + company);
            }
        }

        private static string parseGAString(string cookieValue, string key)
        {
            //252527244.1405639771.1.1.utmcsr=GetApp|utmccn=GetApp|utmcmd=cpc
            int i = cookieValue.IndexOf(key + "=");
            if (i < 0) return "";
            string s = cookieValue.Substring(i + key.Length + 1);
            int j = s.IndexOf("|");
            if (j > -1) { s = s.Substring(0, j); }

            return HttpUtility.UrlDecode(s);
        }

        private static string GetValueString(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "" : HttpUtility.UrlDecode(value);
        }

        public static bool IsCompanyValid(string company)
        {
            Organizations organizations = new Organizations(LoginUser.Anonymous);
            organizations.LoadByOrganizationName(company.Trim());
            if (!organizations.IsEmpty)
            {
                return false;
            }
            return true;
        }

        private static void SyncUser(HttpContext context)
        {
            /*
            payload.UserID = userID;
            payload.OrganizationID = orgID;
            payload.FirstName = firstName;
            payload.LastName = lastName;
            payload.Email = email;
            payload.Title = title;
            payload.PodName = SystemSettings.GetPodName();
            payload.Key = "81f4060c-2166-48c3-a126-b12c94f1fd9d";
            */
            try
            {
                dynamic data = JObject.Parse(ReadJsonData(context));
                if (data.Key != "81f4060c-2166-48c3-a126-b12c94f1fd9d") return;
                LoginUser loginUser = LoginUser.Anonymous;
                User tsUser = null;
                Organizations organizations = new Organizations(loginUser);
                organizations.LoadByImportID(data.PodName.ToString() + "-" + data.OrganizationID.ToString(), 1078);
                if (organizations.IsEmpty) return;
                Organization tsOrg = organizations[0];

                Users users = new Users(loginUser);
                users.LoadByImportID(data.UserID.ToString(), tsOrg.OrganizationID);
                if (users.IsEmpty) users.LoadByEmail(data.Email.ToString().Trim(), tsOrg.OrganizationID);

                if (users.IsEmpty)
                {
                    tsUser = (new Users(loginUser)).AddNewUser();
                    tsUser.OrganizationID = tsOrg.OrganizationID;
                    tsUser.IsActive = true;
                    tsUser.IsPortalUser = true;
                }
                else
                {
                    tsUser = users[0];
                }
                tsUser.ImportID = data.UserID.ToString();
                tsUser.FirstName = data.FirstName.ToString().Trim();
                tsUser.LastName = data.LastName.ToString().Trim();
                tsUser.Email = data.Email.ToString().Trim();
                tsUser.Title = data.Title.ToString();
                tsUser.Collection.Save();
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "SyncUser");
                throw;
            }

        }

        private static void SyncUserEmail(HttpContext context)
        {
            /*
            payload.UserID = userID;
            payload.OrganizationID = orgID;
            payload.NewEmail = email;
            payload.OldEmail = email;
            payload.PodName = SystemSettings.GetPodName();
            payload.Key = "81f4060c-2166-48c3-a126-b12c94f1fd9d";
            */
            try
            {
                dynamic data = JObject.Parse(ReadJsonData(context));
                if (data.Key != "81f4060c-2166-48c3-a126-b12c94f1fd9d") return;
                LoginUser loginUser = LoginUser.Anonymous;
                User tsUser = null;
                Organizations organizations = new Organizations(loginUser);
                organizations.LoadByImportID(data.PodName.ToString() + "-" + data.OrganizationID.ToString(), 1078);
                if (organizations.IsEmpty) return;
                Organization tsOrg = organizations[0];

                Users users = new Users(loginUser);
                users.LoadByImportID(data.UserID.ToString(), tsOrg.OrganizationID);
                if (users.IsEmpty) users.LoadByEmail(data.OldEmail.ToString().Trim(), tsOrg.OrganizationID);

                if (users.IsEmpty)
                {
                    tsUser = (new Users(loginUser)).AddNewUser();
                    tsUser.OrganizationID = tsOrg.OrganizationID;
                    tsUser.IsActive = true;
                    tsUser.IsPortalUser = true;
                }
                else
                {
                    tsUser = users[0];
                }
                tsUser.ImportID = data.UserID.ToString();
                tsUser.Email = data.NewEmail.ToString().Trim();
                tsUser.Collection.Save();
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "SyncUserEmail");
                throw;
            }

        }

        private static void SyncNewOrg(HttpContext context)
        {
            /*
            payload.OrganizationID = orgID;
            payload.Company = company;
            payload.UserID = userID;
            payload.FirstName = firstName;
            payload.LastName = lastName;
            payload.Email = email;
            payload.PhoneNumber = phoneNumber;
            payload.ProductType = (int)productType;
            payload.Promo = promo;
            payload.HubSpotUtk = hubSpotUtk;
            payload.Source = source;
            payload.Campaign = campaign;
            payload.PodName = SystemSettings.GetPodName();
            payload.Key = "81f4060c-2166-48c3-a126-b12c94f1fd9d";
            payload.PotentialSeats = 0;

            */
            try
            {
                dynamic data = JObject.Parse(ReadJsonData(context));
                if (data.Key != "81f4060c-2166-48c3-a126-b12c94f1fd9d") return;

                LoginUser loginUser = LoginUser.Anonymous;
                Organization tsOrg = (new Organizations(loginUser)).AddNewOrganization();
                tsOrg.ParentID = 1078;
                tsOrg.Name = data.Company;
                tsOrg.ImportID = data.PodName.ToString() + "-" + data.OrganizationID.ToString();
                tsOrg.HasPortalAccess = true;
                tsOrg.IsActive = true;
                tsOrg.PotentialSeats = data.PotentialSeats;
                tsOrg.Collection.Save();

                User tsUser = (new Users(loginUser)).AddNewUser();
                tsUser.OrganizationID = tsOrg.OrganizationID;
                tsUser.FirstName = data.FirstName.ToString();
                tsUser.LastName = data.LastName.ToString();
                tsUser.Email = data.Email.ToString();
                tsUser.IsActive = true;
                tsUser.IsPortalUser = true;
                tsUser.ImportID = data.UserID.ToString();
                tsUser.Collection.Save();

                tsOrg.PrimaryUserID = tsUser.UserID;
                tsOrg.Collection.Save();

                PhoneNumber phone = (new PhoneNumbers(loginUser)).AddNewPhoneNumber();
                phone.RefID = tsOrg.OrganizationID;
                phone.RefType = ReferenceType.Organizations;
                phone.Number = data.PhoneNumber.ToString();
                phone.Collection.Save();

                OrganizationProducts ops = new OrganizationProducts(loginUser);
                try
                {
                    OrganizationProduct op = ops.AddNewOrganizationProduct();
                    op.OrganizationID = tsOrg.OrganizationID;
                    op.ProductID = 219;
                    op.ProductVersionID = null;
                    op.IsVisibleOnPortal = true;

                    op = ops.AddNewOrganizationProduct();
                    op.OrganizationID = tsOrg.OrganizationID;
                    op.ProductID = 44639;
                    op.ProductVersionID = null;
                    op.IsVisibleOnPortal = true;
                    ops.Save();

                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(loginUser, ex, "signup");
                }

/*                string[] salesGuys = SystemSettings.ReadString(loginUser, "SalesGuys", "Jesus:1045957|Leon:1045958").Split('|');
                int nextSalesGuy = int.Parse(SystemSettings.ReadString(loginUser, "NextSalesGuy", "0"));
                if (nextSalesGuy >= salesGuys.Length || nextSalesGuy < 0) nextSalesGuy = 0;
                string salesGuy = salesGuys[nextSalesGuy].Split(':')[0];
                string salesGuyID = salesGuys[nextSalesGuy].Split(':')[1];
                nextSalesGuy++;
                if (nextSalesGuy >= salesGuys.Length) nextSalesGuy = 0;
               SystemSettings.WriteString(loginUser, "NextSalesGuy", nextSalesGuy.ToString());
*/
                string promo = data.Promo.ToString();
                string hubSpotUtk = data.HubSpotUtk.ToString();
                string source = data.Source.ToString();
                string campaign = data.Campaign.ToString();
                string potentialSeats = data.PotentialSeats.ToString();
                ProductType productType = (ProductType)int.Parse(data.ProductType.ToString());
                string version = productType == ProductType.HelpDesk ? "Support Desk" : "Enterprise";


                try
                {
                    CustomFields customFields = new CustomFields(loginUser);
                    customFields.LoadByOrganization(1078);

                    CustomValues.UpdateByAPIFieldName(loginUser, customFields, tsOrg.OrganizationID, "Version", version);
                    CustomValues.UpdateByAPIFieldName(loginUser, customFields, tsOrg.OrganizationID, "PodName", data.PodName.ToString());
                    CustomValues.UpdateByAPIFieldName(loginUser, customFields, tsOrg.OrganizationID, "TeamSupportOrganizationID", data.OrganizationID.ToString());
                    CustomValues.UpdateByAPIFieldName(loginUser, customFields, tsOrg.OrganizationID, "LifecycleStatus", "Trial");

                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(loginUser, ex, "SyncOrg - Custom Fields");
                }

                try
                {
                    TSHubSpot.HubspotPost(tsUser.FirstName, tsUser.LastName, tsUser.Email, tsOrg.Name, phone.Number, promo, potentialSeats, source, hubSpotUtk, productType);
                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(loginUser, ex, "Sign Up - HubSpot", "UserID: " + tsUser.UserID.ToString());
                }

                try
                {

                    int hrCompanyID = TSHighrise.CreateCompany(tsOrg.Name, phone.Number, tsUser.Email, productType, "", source, campaign, "", new string[] { "trial" });
                    int hrContactID = TSHighrise.CreatePerson(tsUser.FirstName, tsUser.LastName, tsOrg.Name, phone.Number, tsUser.Email, productType, "", source, campaign, "");
                    //1. New Trial Check In:1496359
                    //3. End of trial: 1496361
                    //Eric's ID 159931
                    //TSHighrise.CreateTaskFrame("", "today", "1496359", "Party", hrContactID.ToString(), salesGuyID, true, true);
                    //TSHighrise.CreateTaskDate("", DateTime.Now.AddDays(14), "1496361", "Company", hrCompanyID.ToString(), "159931", true, false);//
                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(loginUser, ex, "Sign Up - Highrise", "UserID: " + tsUser.UserID.ToString());
                }

            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "SyncUser");
                throw;
            }


        }

        private static void SyncOrg(HttpContext context)
        {
            /*
            payload.OrganizationID = orgID;
            payload.Company = company;
            payload.PodName = SystemSettings.GetPodName();
            payload.Key = "81f4060c-2166-48c3-a126-b12c94f1fd9d";
            */
            try
            {
                dynamic data = JObject.Parse(ReadJsonData(context));
                if (data.Key != "81f4060c-2166-48c3-a126-b12c94f1fd9d") return;

                LoginUser loginUser = LoginUser.Anonymous;
                Organizations orgs = new Organizations(loginUser);
                orgs.LoadByImportID(data.PodName.ToString() + "-" + data.OrganizationID.ToString(), 1078);
                Organization tsOrg = orgs[0];
                tsOrg.Name = data.Company;
                tsOrg.Collection.Save();

            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "SyncOrg");
                throw;
            }


        }

        private static string ReadJsonData(HttpContext context)
        {
            string result = "";
            try
            {
                using (Stream receiveStream = context.Request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        result = readStream.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "ReadJsonData-SignUpSync");
            }
            return result;
        }

		private bool IsReCaptchaVerified(NameValueCollection values)
		{
			bool isVerified = false;
			string reCaptchaResponse = GetValueString(values["g-recaptcha-response"]);
			string googleReCaptchaUrl = "https://www.google.com/recaptcha/api/siteverify";
			string googleReCaptchaSecret = System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleReCaptchaSecret"];
			googleReCaptchaUrl += "?secret=" + googleReCaptchaSecret + "&response=" + reCaptchaResponse;

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(googleReCaptchaUrl);
			request.Method = "GET";
			request.KeepAlive = false;
			request.ContentType = "application/json";

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				if (request.HaveResponse && response != null)
				{
					using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8))
					{
						string responseText = reader.ReadToEnd();
						dynamic responseObject = JObject.Parse(responseText);
						string success = responseObject.success;
						isVerified = bool.Parse(success);
					}
				}
			}

			return isVerified;
		}

        public class MarketingCookie
        {
            public string Source { get; set; }
            public string Medium { get; set; }
            public string Term { get; set; }
            public string Content { get; set; }
            public string Campaign { get; set; }

        }
    }



}
