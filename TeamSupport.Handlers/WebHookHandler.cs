using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TeamSupport.Data;
using log4net; //vv logging stuff. installed with NuGet. log4net by the Apache Software Foundation. version 2.0.8

namespace TeamSupport.Handlers
{
	public class WebHookHandler : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(WebHookHandler));
		Random rnd = new Random();

		#region IHttpHandler Members

		public bool IsReusable
		{
			get { return false; }
		}

		public WebHookHandler()
		{
			log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "logging.config"));
		}

		/// <summary>
		/// Url example: http://localhost/webhook/{specific_handler_for_app}/{options}
		/// parsed to array:
		///     [0]: "/"
		///     [1]: "webhook/"
		///     [2]: "{specific_handler_for_app}/"
		///     [n]: "{options, querystrings, etc, depending on app [2]}"
		/// </summary>
		/// <param name="context"></param>
		public void ProcessRequest(HttpContext context)
		{
			try
			{
				NameValueCollection queryString = context.Request.QueryString;

				//Parse the URL and get the route and ParentID
				if (context.Request.Url.Segments.Count() >= 3)
				{
					Log(context, string.Format("URL: {0}{1}", context.Request.Url.AbsolutePath, (queryString.Count == 0) ? "" : "?" + context.Request.Url.AbsoluteUri.Split('?')[1]));

					//check what type of webhook we will deal with
					WebHookType webhookType;
					string webhook = context.Request.Url.Segments[2].TrimEnd('/');

					if (Enum.TryParse(webhook, true, out webhookType))
					{
						switch (webhookType)
						{
							case WebHookType.Integration:
								ProcessIntegration(context);
								break;
							default:
								break;
						}
					}
					else
					{
						Log(context, string.Format("The webhook type \"{0}\" is not found", webhook));
					}
				}
			}
			catch (Exception ex)
			{
				//Honestly just arbitrarily I'm using 100-999 for a random number to use as the error ref, this way we can find quicker (I hope) the error in the logs.
				int randomNumber = rnd.Next(100, 999);
				string errorRef = DateTime.Now.ToString("yyyyMMdd") + randomNumber.ToString();
				Log(context, String.Format("Exception (ref: {0}): {1}:{2}{3}", errorRef, ex.Message, Environment.NewLine, ex.StackTrace), isError: true);
				Log(context, string.Format("An internal error ocurred with your request, please contact TeamSupport. Error ref: {0}", errorRef), LogType.Client, isError: true, httpStatusCode: HttpStatusCode.InternalServerError);
			}
			context.Response.End();
		}

		#endregion

		#region classes

		private enum IntegrationApps : byte
		{
			Unknown = 0,
			ServiceNow = 1
		}

		private enum LogType : byte
		{
			Internal = 0,
			Client = 1,
			Both = 2
		}

		#endregion

		#region Utility Methods

		private void ProcessIntegration(HttpContext context)
		{
			const int orgIdSegment = 4;
			const int webhookTokenSegment = 5;
			IntegrationApps processApp;
			string app = string.Empty;

			if (context.Request.Url.Segments.Count() > 3)
			{
				app = context.Request.Url.Segments[3].TrimEnd('/');

				if (Enum.TryParse(app, true, out processApp))
				{
					CRMLinkTableItem crmLink = null;

					switch (processApp)
					{
						case IntegrationApps.ServiceNow:
							if (ValidateIntegrationRequest(processApp, context, ref crmLink))
							{
								int organizationId = -1;
								string webhookToken = string.Empty;
								int.TryParse(context.Request.Url.Segments[orgIdSegment].TrimEnd('/'), out organizationId);
								webhookToken = context.Request.Url.Segments[webhookTokenSegment].TrimEnd('/');

								string jsonData = ReadJsonData(context);

								if (!string.IsNullOrEmpty(jsonData))
								{
									log.InfoFormat("Body: {0}", jsonData);

									WebHooksPendingItem newPendingWebHook = (new WebHooksPending(LoginUser.Anonymous)).AddNewWebHooksPendingItem();
									newPendingWebHook.OrganizationId = organizationId;
									newPendingWebHook.RefType = (int)ReferenceType.Tickets;
									newPendingWebHook.Type = (short)WebHookType.Integration;
									newPendingWebHook.Url = context.Request.Url.AbsolutePath;
									newPendingWebHook.BodyData = jsonData;
									newPendingWebHook.Token = webhookToken;
									newPendingWebHook.Inbound = true;
									newPendingWebHook.IsProcessing = false;
									newPendingWebHook.DateCreated = DateTime.UtcNow;
									newPendingWebHook.Collection.Save();

									Log(context, "Queued", LogType.Client);
								}
								else
								{
									Log(context, "This integration needs data to process and it was not found.", isError: true);
									Log(context, "Body data is expected", LogType.Client, isError: true, httpStatusCode: HttpStatusCode.BadRequest);
								}
							}
							break;
						case IntegrationApps.Unknown:
						default:
							break;
					}
				}
				else
				{
					Log(context, string.Format("Integration webhook for \"{0}\" not found", app), isError: true);
					Log(context, "Webhook requested not implemented", LogType.Client, isError: true, httpStatusCode: HttpStatusCode.NotImplemented);
				}
			}
			else
			{
				Log(context, "Integration type not found", LogType.Both, isError: true, httpStatusCode: HttpStatusCode.NotImplemented);
			}
		}

		private bool ValidateIntegrationRequest(IntegrationApps app, HttpContext context, ref CRMLinkTableItem crmLink)
		{
			const int orgIdSegment = 4;
			const int webhookTokenSegment = 5;
			List<string> errors = new List<string>();

			//As of right now all webhook calls for integration apps should have orgid and the token in the 3rd and 4th segments respectively. And the token should be valid.
			int orgId = 0;
			string webhookToken = string.Empty;

			if (!int.TryParse(context.Request.Url.Segments[orgIdSegment].TrimEnd('/'), out orgId))
			{
				errors.Add("OrganizationId is missing or invalid in the URL request");
			}

			if (context.Request.Url.Segments.Count() < 6 || string.IsNullOrEmpty(context.Request.Url.Segments[webhookTokenSegment].TrimEnd('/')))
			{
				errors.Add("WebHook token is missing in the URL request");
			}
			else
			{
				webhookToken = context.Request.Url.Segments[webhookTokenSegment].TrimEnd('/');
			}

			//Specific validation, if any.
			switch (app)
			{
				case IntegrationApps.ServiceNow:
					break;
			}

			Log(context, errors, LogType.Both, isError: true, httpStatusCode: HttpStatusCode.BadRequest);

			return errors.Count == 0;
		}

		private string ReadJsonData(HttpContext context)
		{
			string result = "";
			using (Stream receiveStream = context.Request.InputStream)
			{
				using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
				{
					result = readStream.ReadToEnd();
				}
			}
			return result;
		}

		private string ReadAuthenticationHeader(HttpContext context)
		{
			string authHeader = string.Empty;

			if (context.Request.Headers.AllKeys.Contains("Authorization"))
			{
				authHeader = context.Request.Headers["Authorization"];
				authHeader = authHeader.Replace("Basic ", "");
			}

			return authHeader;
		}

		private void Log(HttpContext context, List<string> messages, LogType logType = LogType.Internal, bool isError = false, HttpStatusCode httpStatusCode = HttpStatusCode.Accepted)
		{
			if (messages.Any())
			{
				if (logType == LogType.Internal || logType == LogType.Both)
				{
					foreach (string message in messages)
					{
						if (isError)
						{
							log.Error(message);
						}
						else
						{
							log.Info(message);
						}
					}
				}

				if (logType == LogType.Client || logType == LogType.Both)
				{
					string jsonMessage = string.Empty;

					if (isError)
					{
						jsonMessage = JsonConvert.SerializeObject(new { Error = messages });
					}
					else
					{
						jsonMessage = JsonConvert.SerializeObject(new { Message = messages });
					}

					context.Response.StatusCode = (int)httpStatusCode;
					context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
					context.Response.AddHeader("Expires", "-1");
					context.Response.AddHeader("Pragma", "no-cache");
					context.Response.ContentType = "application/json; charset=utf-8";
					context.Response.Write(jsonMessage);
				}
			}
		}

		private void Log(HttpContext context, string message, LogType logType = LogType.Internal, bool isError = false, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
		{
			List<string> messages = new List<string>() { message };
			Log(context, messages, logType, isError, httpStatusCode);
		}

		#endregion
	}
}
