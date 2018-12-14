using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using System.IO;

using System.Dynamic;
using System.Net;
using System.Threading;

namespace TeamSupport.ServiceLibrary
{
	[Serializable]
	public class WebHooks : ServiceThread
	{
		private int _threadNumber;
		public int ThreadNumber
		{
			get
			{
				int number = 0;
				lock (this)
				{
					number = _threadNumber;
				}
				return number;
			}
		}

		private WebHooksPendingItem _webHook;
		public WebHooksPendingItem WebHook
		{
			get
			{
				WebHooksPendingItem result = null;
				lock (this)
				{
					result = _webHook;
				}
				return result;
			}
		}

		public WebHooks()
		{
		}

		public WebHooks(WebHooksPendingItem webhook, int threadNumber)
		{
			_webHook = webhook;
			_threadNumber = threadNumber;
		}

		public override void Run()
		{
			try
			{
				ProcessWebHook(WebHook);
				Thread.Sleep(0);
				_isStopped = true;
			}
			catch (Exception ex)
			{
				Logs.WriteEvent("Error processing the webhook");
				Logs.WriteEventFormat("Id: {0}, Is Inbound: {1}, RefId: {2}", WebHook.Id, WebHook.Inbound, WebHook.RefId);
				Logs.WriteEventFormat("WebHook Data: {0}", WebHook.BodyData);
				Logs.WriteException(ex);
				ExceptionLogs.LogException(LoginUser, ex, "Webhooks", "Error processing the webhook.");

				//ToDo should we re-insert for later process a webhook that threw an in-process unhandled-exception while processing?
				/*
				 WebHooksPending reInserted = new WebHooksPending(LoginUser);
				WebHooksPendingItem reInsertedItem = reInserted.AddNewWebHooksPendingItem();
				reInsertedItem.RefId = WebHook.RefId;
				reInsertedItem.RefType = WebHook.RefType;
				reInsertedItem.Type = WebHook.Type;
				reInsertedItem.BodyData = WebHook.BodyData;
				reInsertedItem.Inbound = WebHook.Inbound;
				reInsertedItem.IsProcessing = false;
				reInserted.Save();
				 */
			}
		}

		private void ProcessWebHook(WebHooksPendingItem webhook)
		{
			UpdateHealth();
			webhook.IsProcessing = true;
			webhook.Collection.Save();

			//check what type of webhook it is:
			switch ((WebHookType)webhook.Type)
			{
				case WebHookType.Integration:
					//Right now only ServiceNow is integrated this way, so we only check for those links, however at some point we'll have to include more integrations here.
					ServiceNow serviceNow = new ServiceNow(webhook, LoginUser);
					serviceNow.Process(_threadNumber);
					break;
				default:
					Logs.WriteEvent("Unknown webhook type. Not implemented yet.");
					Logs.WriteData(webhook.Row);
					webhook.Delete();
					webhook.Collection.Save();
					break;
			}
		}

		//private void ProcessIntegration(LoginUser loginUser, WebHooksPendingItem webhook)
		//{
		//	//Right now only ServiceNow is integrated this way, so we only check for those links, however at some point we'll have to include more integrations here.
		//	IntegrationServiceNow(webhook);
		//}

		//private void IntegrationServiceNow(WebHooksPendingItem webhook)
		//{
		//	/*
		//	 * 1) Check if this is an inbound or outbound webhook
		//	 * 2) If outbound then get the data from CRMLinkTable to where to do the Post to and the credentials
		//	 * 3) If inbound then parse the json into a dynamic object and update the ticket and/or add the action.
		//	 *	3.1) What are the fields to map by default? (no custom mapping needed)
		//	 *	3.2) Check custom mappings. This one uses the TicketViews schema so we'll need to do switch case and go almost case by case (field).
		//	 */
		//	if (webhook.Inbound)
		//	{
		//	}
		//	else
		//	{
		//		string url = string.Empty;
		//		//Oubound should always have the RefId, but still check
		//		if (webhook.RefId != null)
		//		{
		//			switch((ReferenceType)webhook.RefType)
		//			{
		//				case ReferenceType.Actions:
		//					//already linked, just sending a new ticket action
		//					Data.Action action = Actions.GetActionByID(LoginUser, (int)webhook.RefId);

		//					if (action != null)
		//					{
		//						CRMLinkTable crmLink = new CRMLinkTable(LoginUser);
		//						crmLink.LoadByActionIdForServicesNow(action.ActionID);
		//						TicketLinkToSnow ticketLink = new TicketLinkToSnow(LoginUser);
		//						ticketLink.LoadByTicketID(action.TicketID);

		//						if (crmLink.Count > 0 && ticketLink.Count > 0 && ticketLink[0].AppId != null)
		//						{
		//							Ticket ticket = Tickets.GetTicket(LoginUser, action.TicketID);
		//							string hostName = crmLink[0].HostName;

		//							if (!hostName.EndsWith("/"))
		//							{
		//								hostName += "/";
		//							}

		//							url = hostName + "api/now/table/incident/" + ticketLink[0].AppId;
		//							string username = crmLink[0].Username;
		//							string password = crmLink[0].Password;
		//							string encodedCredentials = DataUtils.GetEncodedCredentials(username, password);
		//							int actionPosition = Actions.GetActionPosition(LoginUser, action.ActionID);

		//							string comment = BuildCommentBody(ticket.TicketNumber.ToString(), action.Description, actionPosition, action.CreatorID);
		//							string bodyData = string.Empty;
		//							string commentWorkNote = "^element={0}^ORDERBYDESCsys_created_on&sysparm_fields=element_id,value,sys_id";

		//							if (action.IsVisibleOnPortal)
		//							{
		//								bodyData = Newtonsoft.Json.JsonConvert.SerializeObject(new { comments = comment });
		//								commentWorkNote = string.Format(commentWorkNote, "comments");
		//							}
		//							else
		//							{
		//								bodyData = Newtonsoft.Json.JsonConvert.SerializeObject(new { work_notes = comment });
		//								commentWorkNote = string.Format(commentWorkNote, "work_notes");
		//							}

		//							//***** Create the comment/worknote in ServiceNow
		//							string result = string.Empty;
		//							HttpStatusCode resultStatus = MakeHttpRequest(url, encodedCredentials, ref result, "PUT", "application/json", bodyData);

		//							if (resultStatus == HttpStatusCode.OK && !string.IsNullOrEmpty(result) && !result.Contains("fell asleep") && !result.Contains("sleepy instances"))
		//							{
		//								string appId = ticketLink[0].AppId;
		//								var expandoConverter = new Newtonsoft.Json.Converters.ExpandoObjectConverter();
		//								dynamic incidentObject = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(result, expandoConverter);

		//								if (((IDictionary<string, object>)incidentObject).ContainsKey("result") && ((IDictionary<string, object>)incidentObject.result).ContainsKey("sys_id"))
		//								{
		//									//We need to make an additional call (GET) to servicenow to get the commentId.
		//									url = hostName + "api/now/table/sys_journal_field?sysparm_query=element_id=" + appId + commentWorkNote;

		//									result = string.Empty;
		//									resultStatus = MakeHttpRequest(url, encodedCredentials, ref result, "GET", "application/json", string.Empty);

		//									if (resultStatus == HttpStatusCode.OK && !string.IsNullOrEmpty(result))
		//									{
		//										incidentObject = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(result, expandoConverter);

		//										for (int i = 0; i < incidentObject.result.Count; i++)
		//										{
		//											string value = incidentObject.result[i].value;
		//											string commentWorkNoteId = incidentObject.result[i].sys_id;

		//											if (value.ToLower().Contains("teamsupport") && value.Contains(ticket.TicketNumber.ToString()))
		//											{
		//												ActionLinkToSnow newActionLinkToSnow = new ActionLinkToSnow(LoginUser);
		//												ActionLinkToSnowItem newActionLinkToSnowItem = newActionLinkToSnow.AddNewActionLinkToSnowItem();
		//												newActionLinkToSnowItem.ActionID = action.ActionID;
		//												newActionLinkToSnowItem.AppId = commentWorkNoteId;
		//												newActionLinkToSnowItem.DateModifiedBySync = DateTime.UtcNow;
		//												newActionLinkToSnow.Save();
		//												break;
		//											}
		//										}

		//										Logs.WriteEvent("Created comment for action");
		//									}
		//								}
		//								else
		//								{
		//									Logs.WriteEventFormat("The Comment/WorkNote was added to the incident id ({0}) but didn't get the response (GET) back to link the comment/worknote Id to the action id ({1})", appId, action.ActionID);
		//								}
		//							} else if (resultStatus == HttpStatusCode.OK && !string.IsNullOrEmpty(result) && !result.Contains("fell asleep") && !result.Contains("sleepy instances"))
		//							{
		//								Logs.WriteEventFormat("The ServiceNow instance is Hibernating, the request cannot be completed. Re-Inserting the webhook request to attempt it later again.");
		//								WebHooksPending reInserted = new WebHooksPending(LoginUser);
		//								WebHooksPendingItem reInsertedItem = reInserted.AddNewWebHooksPendingItem();
		//								reInsertedItem.RefId = webhook.RefId;
		//								reInsertedItem.RefType = webhook.RefType;
		//								reInsertedItem.Type = webhook.Type;
		//								reInsertedItem.BodyData = webhook.BodyData;
		//								reInsertedItem.Inbound = webhook.Inbound;
		//								reInsertedItem.IsProcessing = false;
		//							} else if (resultStatus != HttpStatusCode.OK)
		//							{
		//								Logs.WriteEventFormat("The httpRequest to ServiceNow failed with a status code of '{0}', the webhook request was deleted.", resultStatus);
		//							}

		//							webhook.Delete();
		//							webhook.Collection.Save();
		//						}
		//					}
		//					break;
		//				case ReferenceType.Tickets:
		//					//As of right now this should only happen when the ticket is creating a new link (record) in the integration app
		//					TicketLinkToSnow ticketToSnow = new TicketLinkToSnow(LoginUser);
		//					ticketToSnow.LoadByTicketID((int)webhook.RefId);

		//					if (ticketToSnow.Any())
		//					{
		//						CRMLinkTable crmLink = new CRMLinkTable(LoginUser);
		//						crmLink.LoadByCRMLinkID((int)ticketToSnow[0].CrmLinkID);

		//						if (ticketToSnow[0].AppId == null)
		//						{
		//							//ticket to link creating new incident
		//						}
		//					}
		//					break;
		//				default:
		//					break;
		//			}
		//		}
		//		else
		//		{
		//			Logs.WriteEvent("The webhook id {0} didn't have the RefId to Integrate");
		//			Logs.WriteData(webhook.Row);
		//		}
		//	}
		//}

		//private HttpStatusCode MakeHttpRequest(string uri, string encodedCredentials, ref string responseString, string method = "GET", string contentType = "application/text", string data = "")
		//{
		//	HttpStatusCode resultStatusCode = HttpStatusCode.BadRequest;
		//	var request = (HttpWebRequest)WebRequest.Create(uri);
		//	request.Method = method;
		//	request.ContentType = contentType;
		//	request.Headers.Add("Authorization", "Basic " + encodedCredentials);

		//	if (data.Length > 0)
		//	{
		//		var writer = new StreamWriter(request.GetRequestStream());
		//		writer.Write(data);
		//		writer.Close();
		//	}

		//	try
		//	{
		//		var response = (HttpWebResponse)request.GetResponse();
		//		resultStatusCode = response.StatusCode;
		//		Stream responseStream = response.GetResponseStream();

		//		if (responseStream != null)
		//		{
		//			var streamReader = new StreamReader(responseStream);
		//			responseString = streamReader.ReadToEnd();
		//		}
		//	}
		//	catch (WebException e)
		//	{
		//		using (var response = (HttpWebResponse)e.Response)
		//		{
		//			Stream responseStream = response.GetResponseStream();

		//			if (responseStream != null)
		//			{
		//				var reader = new StreamReader(responseStream);
		//				string responseText = string.Format("Error code: {0} - {1}{2}{3}", response.StatusCode, response.StatusDescription, Environment.NewLine, reader.ReadToEnd());
		//				throw new Exception(responseText);
		//			}
		//		}
		//	}

		//	return resultStatusCode;
		//}

		//private string BuildCommentBody(string ticketNumber, string actionDescription, int actionPosition, int creatorId)
		//{
		//	StringBuilder result = new StringBuilder();
		//	UsersViewItem creatorUser = UsersView.GetUsersViewItem(LoginUser, creatorId);
		//	string creatorUserName = (creatorUser != null ? string.Format(" added by {0} {1}", creatorUser.FirstName, creatorUser.LastName) : string.Empty);
		//	result.Append("TeamSupport ticket #" + ticketNumber.ToString() + " comment #" + actionPosition.ToString() + creatorUserName + ":");
		//	result.Append(Environment.NewLine);
		//	result.Append(Environment.NewLine);
		//	//Extract the code tag contents (if exists) and re-add after the html strip
		//	Dictionary<int, string> codeSamples = new Dictionary<int, string>();
		//	codeSamples = HtmlCleaner.ExtractCodeSamples(ref actionDescription);
		//	bool addLines = true;
		//	actionDescription = HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(actionDescription), addLines);
		//	HtmlCleaner.AddCodeSamples(ref actionDescription, codeSamples);
		//	result.Append(actionDescription);
		//	return result.ToString();
		//}
	}

	public class WebHooksPool : ServiceThread
	{
		private static List<WebHooks> _threads = new List<WebHooks>();
		private static bool _webhooksPoolStopped = false;

		public WebHooksPool()
		{
			RunHandlesStop = true;
		}

		public WebHooksPool(SystemUser serviceUser) : base(serviceUser)
		{
			RunHandlesStop = true;
		}

		public override void Run()
		{
			Logs.WriteEvent("WebHooksPool is running...", true);
			int maxThreads = Settings.ReadInt("Max Worker Processes", 1);
			Logs.WriteEventFormat("maxThreads: {0}, IsStopped: {1}, Threads count: {2}", maxThreads.ToString(), IsStopped.ToString(), _threads.Count.ToString());

			if (_threads.Count > 0)
			{
				for (int index = _threads.Count - 1; index >= 0; index += -1)
				{
					Logs.WriteEvent("Checking thread index: " + index.ToString(), true);

					if (IsStopped)
					{
						_threads[index].Stop();
						Logs.WriteEvent("Thread index: " + index.ToString() + " stopped.", true);
						_threads.RemoveAt(index);
						Logs.WriteEvent("Thread index: " + index.ToString() + " removed.", true);
					}
					else if ((_threads[index].IsStopped))
					{
						Logs.WriteEvent("Thread index: " + index.ToString() + " is stopped.", true);
						_threads.RemoveAt(index);
						Logs.WriteEvent("Thread index: " + index.ToString() + " removed.", true);
					}
				}
			}

			if (IsStopped)
			{
				Logs.WriteEvent("WebHooksPool finishing as is stopped", true);
				return;
			}

			//See if we have reached our max thread count
			if (_threads.Count >= maxThreads)
			{
				Logs.WriteEvent("WebHooksPool finishing as its threads count exceeds maxThreads", true);
				return;
			}

			//Load the pending webhooks to process
			WebHooksPending webhooksPending = new WebHooksPending(LoginUser);
			webhooksPending.LoadPending();
			Logs.WriteEventFormat("WebHooksPool to process: {0}", webhooksPending.Count());
			bool isAlreadyProcessingValue = false;

			foreach (WebHooksPendingItem webhook in webhooksPending)
			{
				while (_threads.Count >= maxThreads)
				{
					//loop until one of the threads becomes available to keep processing the rest of the webhooks
					Thread.Sleep(100);

					for (int index = _threads.Count - 1; index >= 0; index += -1)
					{
						if (IsStopped)
						{
							_threads[index].Stop();
							_threads.RemoveAt(index);
						}
						else if ((_threads[index].IsStopped))
						{
							_threads.RemoveAt(index);
						}
					}
				}

				isAlreadyProcessingValue = IsAlreadyProcessing(webhook.Id);

				if (!isAlreadyProcessingValue)
				{
					int numberForLog = GetAvailableNumber(maxThreads);
					WebHooks webHooks = new WebHooks(webhook, numberForLog);
					webHooks.Start();
					_threads.Add(webHooks);
					Logs.WriteEventFormat("Processing WebHookId ({0}), {1}, log file {2}", webhook.Id, webhook.Inbound ? "Inbound" : "Outbound", numberForLog);
				}
			}
		}

		public override void Start()
		{
			Service service = Services.GetService(LoginUser, "WebHooks");
			service.RunCount = 0;
			service.RunTimeAvg = 0;
			service.RunTimeMax = 0;
			service.ErrorCount = 0;
			service.LastError = "";
			service.LastResult = "";
			service.Collection.Save();
			base.Start();
		}

		public override void Stop()
		{
			_webhooksPoolStopped = true;

			lock (this) { _isStopped = true; }

			if (Thread.IsAlive)
			{
				if (!Thread.Join(10000))
				{
					if (Thread.IsAlive)
					{
						Thread.Abort();
					}
				}
			}
		}

		private bool IsAlreadyProcessing(int id)
		{
			foreach (WebHooks thread in _threads)
			{
				if (thread.WebHook.Id == id)
				{
					return true;
				}
			}
			return false;
		}

		private int GetAvailableNumber(int maxThreads)
		{
			int number = 0;
			List<int> inUse = new List<int>();

			foreach (WebHooks thread in _threads)
			{
				inUse.Add(thread.ThreadNumber);
			}

			for (int i = 0; i < maxThreads; i++)
			{
				if (inUse == null || inUse.Count == 0 || !inUse.Exists(x => x == i))
				{
					number = i;
					break;
				}
			}

			return number;
		}

		public static bool WebHooksPoolStopped
		{
			get
			{
				lock (typeof(WebHooksPool))
				{
					return _webhooksPoolStopped;
				}
			}
			set
			{
				lock (typeof(WebHooksPool))
				{
					_webhooksPoolStopped = value;
				}
			}
		}
	}
}
