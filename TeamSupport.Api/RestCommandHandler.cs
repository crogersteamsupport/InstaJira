using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using TeamSupport.Data;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Net;
using System.Web.SessionState;

namespace TeamSupport.Api
{
    public class RestCommandHandler : IHttpHandler, IRequiresSessionState
    {
        private LoginUser _loginUser;
        private Organization _organization;

        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            int organizationID = int.Parse(context.Request.Headers["OrganizationID"]);
            try
            {
                _loginUser = new LoginUser(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString, (int)SystemUser.API, organizationID, null);
                _organization = Organizations.GetOrganization(_loginUser, organizationID);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(_loginUser, ex, "OrgID: " + organizationID.ToString());
                throw new RestException(HttpStatusCode.Unauthorized);
            }

            ApiLog log = new ApiLogs(_loginUser).AddNewApiLog();
            DateTime timeStart = DateTime.Now;

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "text/plain";
            try
            {
                try
                {
                    RestCommand command = new RestCommand(_loginUser, _organization, context);
                    log.OrganizationID = _organization.OrganizationID;
                    log.RequestBody = command.Data;
                    log.Verb = context.Request.HttpMethod.ToUpper();
                    log.Url = context.Request.Url.OriginalString;
                    log.IPAddress = context.Request.UserHostAddress;
                    RestProcessor processor = new RestProcessor(command);

                    int companyId = command.IsCustomerOnly ? (int)command.Organization.ParentID : _loginUser.OrganizationID;
                    int apiRequestLimit = command.IsCustomerOnly ? Organizations.GetOrganization(_loginUser, companyId).APIRequestLimit : _organization.APIRequestLimit;
                    int apiRequestCount = ApiLogs.GetDailyRequestCount(_loginUser, companyId);

                    if (ApiLogs.IsUrlBlackListed(_loginUser, _organization.OrganizationID, log.Url) || SystemSettings.GetIsApiDisabled())
                    {
                        string blacklistError = "{ \"Error\": \"This resource is not accessible at this time.\"}";

                        if (command.Format == RestFormat.XML)
                        {
                            System.Xml.XmlDocument xmlDoc = Newtonsoft.Json.JsonConvert.DeserializeXmlNode(blacklistError);
                            xmlDoc.XmlResolver = null;
                            blacklistError = xmlDoc.InnerXml;
                        }

                        throw new RestException(HttpStatusCode.BadRequest, blacklistError);
                    }
                    if (apiRequestCount >= apiRequestLimit)
					{
						string requestLimitError = "{ \"Error\": \"You have exceeded your 24 hour API request limit of " + _organization.APIRequestLimit.ToString() + ".\"}";

						if (command.Format == RestFormat.XML)
						{
							System.Xml.XmlDocument xmlDoc = Newtonsoft.Json.JsonConvert.DeserializeXmlNode(requestLimitError);
                            xmlDoc.XmlResolver = null;
							requestLimitError = xmlDoc.InnerXml;
						}

						throw new RestException(HttpStatusCode.Forbidden, requestLimitError);
					}
                    else
                    {
                        int apiRequestMinuteLimit = command.IsCustomerOnly ? Organizations.GetOrganization(_loginUser, companyId).APIRequestMinuteLimit : _organization.APIRequestMinuteLimit;
                        int apiRequestLastMinuteCount = ApiLogs.GetLastMinuteRequestCount(_loginUser, companyId);

                        if (apiRequestLastMinuteCount > apiRequestMinuteLimit)
                        {
                            string requestLimitErrorPerMinute = "{ \"Error\": \"You have exceeded your minute API request limit of " + apiRequestMinuteLimit.ToString() + ".\"}";

                            if (command.Format == RestFormat.XML)
                            {
                                System.Xml.XmlDocument xmlDoc = Newtonsoft.Json.JsonConvert.DeserializeXmlNode(requestLimitErrorPerMinute);
                                xmlDoc.XmlResolver = null;
                                requestLimitErrorPerMinute = xmlDoc.InnerXml;
                            }

                            throw new RestException(HttpStatusCode.Forbidden, requestLimitErrorPerMinute);
                        }
                    }

                    processor.Process();

                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(_loginUser, ex, "API", string.Format("OrgID: {0}{1}Verb: {2}{1}Url: {3}{1}Body: {4}", log.OrganizationID, Environment.NewLine, log.Verb, log.Url, log.RequestBody));
                    if (!(ex is RestException))
                    {
                        throw new RestException(HttpStatusCode.InternalServerError, "Internal Server Error: " + ex.Message + ex.StackTrace, ex);
                    }
                    else
                    {
                        throw ex;
                    }
                }
                
            }
            catch (RestException rex)
            {
                context.Response.ContentType = "text/plain";
                context.Response.StatusCode = (int)rex.HttpStatusCode;
                log.TimeToComplete =  (int)(DateTime.Now - timeStart).TotalSeconds;
                log.StatusCode = context.Response.StatusCode;
                log.Collection.Save();
                context.Response.ClearContent();
                context.Response.Write(rex.Message);
                context.Response.End();
            }
            log.TimeToComplete = (int)(DateTime.Now - timeStart).TotalSeconds;
            log.StatusCode = context.Response.StatusCode;
            log.Collection.Save();
            context.Response.End();
        }
        #endregion
    }
}
