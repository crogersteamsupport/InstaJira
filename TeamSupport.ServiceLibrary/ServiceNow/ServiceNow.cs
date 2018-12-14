using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TeamSupport.Data;
using PusherServer;

namespace TeamSupport.ServiceLibrary
{
	public class ServiceNow
	{
		private WebHooksPendingItem _webhook;
		private CRMLinkTableItem _crmLinkItem;
		private Ticket _ticket;
		private LoginUser _loginUser;
		private TicketLinkToSnow _ticketLinks;
		private CRMLinkErrors _crmErrors;
		private Log _log;
		private string _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
		private const string _stateKey = "state";
		private const string _publicActionKey = "publicAction";
		private const string _privateActionKey = "privateAction";
		private const string _teamSupportUrlField = "u_url_teamsupport";

		public ServiceNow()
		{
		}

		public ServiceNow(WebHooksPendingItem webhook, LoginUser loginUser)
		{
			_webhook = webhook;
			_loginUser = loginUser;
		}

		public void Process(int threadNumber)
		{
			_log = new Log(Path.Combine(_logPath, _webhook.OrganizationId.ToString()), Enums.GetDescription(IntegrationType.ServiceNow), threadNumber);
			_log.Write(string.Format("Starting processing ServiceNow {0} webhook", _webhook.Inbound ? "inbound" : "outbound"));

			WebHooksToken organizationWebHookTokens = new WebHooksToken(_loginUser);
			organizationWebHookTokens.LoadByOrganizationID(_webhook.OrganizationId);

			if (_webhook.Inbound)
			{
				JObject jsonObject = JObject.Parse(_webhook.BodyData);

				if (IsInboundValid(jsonObject))
				{
					try
					{
						WebHooksTokenItem webhookToken = organizationWebHookTokens.Where(p => p.Token == _webhook.Token && p.Id == _crmLinkItem.WebHookTokenId).FirstOrDefault();

						if (webhookToken != null && webhookToken.IsEnabled)
						{
							foreach(TicketLinkToSnowItem ticketLink in _ticketLinks)
							{
								bool refreshTicket = false;
								_ticket = Tickets.GetTicket(_loginUser, ticketLink.TicketID);
								_log.Write(string.Format("Ticket Number: {0}", _ticket.TicketNumber));
								_crmErrors = new CRMLinkErrors(_loginUser);
								_crmErrors.LoadByOperationAndObjectId(_ticket.OrganizationID, Enums.GetDescription(IntegrationType.ServiceNow), Enums.GetDescription(IntegrationOrientation.IntoTeamSupport), "ticket", _ticket.TicketID.ToString());
								bool defaultFieldsUpdated = ProcessInboundDefaultFields(jsonObject, ticketLink, ref refreshTicket);
								bool customMappingsUpdated = ProcessInboundCustomMappings(jsonObject, ticketLink);

								if (defaultFieldsUpdated || customMappingsUpdated)
								{
									string actionLogDescription = "Updated Ticket with the Incident '" + ticketLink.Number + "' changes.";
									ActionLogs.AddActionLog(_loginUser, ActionLogType.Update, ReferenceType.Tickets, _ticket.TicketID, actionLogDescription);
								}

								bool actionAdded = ProcessInboundAction(jsonObject);

								if (refreshTicket || customMappingsUpdated || actionAdded)
								{
									string message = string.Format("{0} updated ticket {1}.", Enums.GetDescription(IntegrationType.ServiceNow), _ticket.TicketNumber);
									SendPusherMessage("ticket-dispatch-" + _ticket.OrganizationID, "DisplayTicketUpdate", new { ticket = _ticket.TicketNumber.ToString(), update = message });
								}
							}
						}
						else
						{
							_log.Write(string.Format("The WebHook Token {0} is disabled, does not exist, or is different (or not setup) of the one in the crmlink", _webhook.Token));
						}
					}
					catch (Exception ex)
					{
						_log.Write(ex.Message + Environment.NewLine + ex.StackTrace);
						_log.Write(_webhook.BodyData);
					}
				}
				else
				{
					_log.Write(string.Format("The inbound request from ServiceNow is invalid and was not processed. The request has been removed.{0}{1}", Environment.NewLine, _webhook.BodyData));
				}

				_webhook.Delete();
				_webhook.Collection.Save();
			}
			else
			{
				//Oubound should always have the RefId, but still check
				if (_webhook.RefId != null)
				{
					switch ((ReferenceType)_webhook.RefType)
					{
						case ReferenceType.Actions:
							Data.Action action = Actions.GetActionByID(_loginUser, (int)_webhook.RefId);

							if (action != null)
							{
								_log.Write(string.Format("Processing Action Id {0} of Ticket id {1}", action.ActionID, action.TicketID));
								bool deleteWebhookPending = false;
								ActionLinkToSnow actionLink = new ActionLinkToSnow(_loginUser);
								actionLink.GetByActionID(action.ActionID);
								
								if (actionLink == null || !actionLink.Any())
								{
									CRMLinkTable crmLink = new CRMLinkTable(_loginUser);
									crmLink.LoadByActionIdForServicesNow(action.ActionID);
									_ticketLinks = new TicketLinkToSnow(_loginUser);
									_ticketLinks.LoadByTicketID(action.TicketID);

									if (crmLink.Count > 0 && _ticketLinks.Count > 0 && _ticketLinks[0].AppId != null)
									{
										_crmLinkItem = crmLink[0];
										ProcessOuboundAction(action);
										_log.Write(string.Format("Ticket Number: {0}", _ticket.TicketNumber));
										deleteWebhookPending = true;
									}
								}
								else
								{
									_log.Write("The action was not sent because it is already linked.");
									deleteWebhookPending = true;
								}

								if (deleteWebhookPending)
								{
									_webhook.Delete();
									_webhook.Collection.Save();
								}
							}
							break;
						case ReferenceType.Tickets:
							_ticketLinks = new TicketLinkToSnow(_loginUser);
							_ticketLinks.LoadByTicketID((int)_webhook.RefId);
							_log.Write(string.Format("Processing Ticket id {0}", (int)_webhook.RefId));

							if (_ticketLinks.Any())
							{
								CRMLinkTable crmLink = new CRMLinkTable(_loginUser);
								crmLink.LoadByCRMLinkID((int)_ticketLinks[0].CrmLinkID);

								if (_ticketLinks[0].AppId == null
									&& _ticketLinks[0].Sync
									&& crmLink.Count > 0
									&& crmLink[0].Active)
								{
									_crmLinkItem = crmLink[0];
									_ticket = Tickets.GetTicket(_loginUser, _ticketLinks[0].TicketID);

									bool isNew = string.IsNullOrEmpty(_ticketLinks[0].Number);
									ProcessOutboundTicket(isNew);

									if (!isNew)
									{
										_log.Write(string.Format("Linking to an existing Incident ({0})", _ticketLinks[0].Number));
									}
								}
								else
								{
									_log.Write("Ticket already have an incident linked to it in the TicketLink table or the CRM is not active. There is nothing to process.");
								}
							}
							else
							{
								_log.Write("The TicketLink was not found or has been deleted (unlinked). There is nothing to process.");
							}

							_webhook.Delete();
							_webhook.Collection.Save();
							break;
						default:
							break;
					}
				}
				else
				{
					_log.Write("The webhook id {0} didn't have the RefId to Integrate");
					_log.WriteData(_webhook.Row);
				}
			}

			_ticketLinks = null;
			_crmLinkItem = null;
			_ticket = null;
		}

		private HttpStatusCode MakeHttpRequest(string uri, string encodedCredentials, ref string responseString, string method = "GET", string contentType = "application/text", string data = "")
		{
			HttpStatusCode resultStatusCode = HttpStatusCode.BadRequest;
			var request = (HttpWebRequest)WebRequest.Create(uri);
			request.Method = method;
			request.ContentType = contentType;
			request.Headers.Add("Authorization", "Basic " + encodedCredentials);

			if (data.Length > 0)
			{
				var writer = new StreamWriter(request.GetRequestStream());
				writer.Write(data);
				writer.Close();
			}

			try
			{
				var response = (HttpWebResponse)request.GetResponse();
				resultStatusCode = response.StatusCode;
				Stream responseStream = response.GetResponseStream();

				if (responseStream != null)
				{
					var streamReader = new StreamReader(responseStream);
					responseString = streamReader.ReadToEnd();
				}
			}
			catch (WebException webEx)
			{
				using (var response = (HttpWebResponse)webEx.Response)
				{
					Stream responseStream = response.GetResponseStream();

					if (responseStream != null)
					{
						var reader = new StreamReader(responseStream);
						string responseText = string.Format("Error code: {0} - {1}{2}{3}", response.StatusCode, response.StatusDescription, Environment.NewLine, reader.ReadToEnd());
						throw new Exception(responseText);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message, ex.InnerException);
			}

			return resultStatusCode;
		}


		private string BuildCommentBody(string ticketNumber, string actionDescription, int actionPosition, int creatorId, bool isTicketDescription = false)
		{
			StringBuilder result = new StringBuilder();
			UsersViewItem creatorUser = UsersView.GetUsersViewItem(_loginUser, creatorId);
			string creatorUserName = (creatorUser != null ? string.Format(" added by {0} {1}", creatorUser.FirstName, creatorUser.LastName) : string.Empty);

			if (!isTicketDescription)
			{
				//This format must mostly match the check used in the method ProcessInboundAction.
				result.Append("TeamSupport ticket #" + ticketNumber.ToString() + " comment #" + actionPosition.ToString() + creatorUserName + ":");
			}

			result.Append(Environment.NewLine);
			result.Append(Environment.NewLine);
			//Extract the code tag contents (if exists) and re-add after the html strip
			Dictionary<int, string> codeSamples = new Dictionary<int, string>();
			codeSamples = HtmlCleaner.ExtractCodeSamples(ref actionDescription);
			bool addLines = true;
			actionDescription = HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(actionDescription), addLines);
			HtmlCleaner.AddCodeSamples(ref actionDescription, codeSamples);
			result.Append(actionDescription);
			return result.ToString();
		}

		/// <summary>
		/// Process the fields that are always sync (no custom mapping needed).
		/// Status, Type, Product. As of right now I still don't know what the equivalent in ServiceNow is for Type and Product.
		/// </summary>
		/// <param name="jObject"></param>
		private bool ProcessInboundDefaultFields(JObject jObject, TicketLinkToSnowItem ticketLink, ref bool refreshTicket)
		{
			bool wasUpdated = false;

			if (jObject.SelectProperty(_stateKey) != null)
			{
				string state = jObject[_stateKey].ToString();
				List<int> excludedTicketStatusToUpdate = new List<int>();
				TicketStatuses statuses = new TicketStatuses(_loginUser);
				bool updateStatus = _crmLinkItem.UpdateStatus ?? false;
				statuses.LoadByTicketTypeIDAndName(_ticket.TicketTypeID, state);

				if (!string.IsNullOrEmpty(_crmLinkItem.ExcludedTicketStatusUpdate))
				{
					excludedTicketStatusToUpdate = _crmLinkItem.ExcludedTicketStatusUpdate.Split(',').Select(p => int.Parse(p)).ToList();
				}
				
				if (updateStatus
					&& statuses.Any()
					&& _ticket.TicketStatusID != statuses[0].TicketStatusID
					&& (!excludedTicketStatusToUpdate.Any()
						|| !excludedTicketStatusToUpdate.Contains(_ticket.TicketStatusID)))
				{
					_ticket.TicketStatusID = statuses[0].TicketStatusID;
					wasUpdated = true;
					_log.ClearCrmLinkError(_ticket.TicketID.ToString(), "Status", _crmErrors);
				}
				else if (updateStatus && statuses.Any() && _ticket.TicketStatusID != statuses[0].TicketStatusID && excludedTicketStatusToUpdate.Contains(_ticket.TicketStatusID))
				{
					_log.Write(string.Format("The ticket status was not updated because the status id {0} is selected in the excluded status in the Integration settings.", _ticket.TicketStatusID));
					_log.ClearCrmLinkError(_ticket.TicketID.ToString(), "Status", _crmErrors);
				}
				else if (!updateStatus)
				{
					if (_ticketLinks.Any() && (ticketLink.State == null || ticketLink.State != state))
					{
						int newActionTypeId = GetNewActionType();
						Actions newAction = new Actions(_loginUser);
						newAction.AddNewAction();
						newAction[0].ActionTypeID = newActionTypeId;
						newAction[0].TicketID = _ticket.TicketID;
						newAction[0].Description = "ServiceNow's Incident " + ticketLink.AppId + "'s state changed" + (ticketLink.State != null ? " from " + ticketLink.State : "") + " to '" + state + "'.";
						newAction.Save();

						AddActionLink(newAction[0].ActionID);
						refreshTicket = true;
					}
				}
				else if (statuses.Count == 0)
				{
					TicketTypes ticketTypes = new TicketTypes(_loginUser);
					ticketTypes.LoadByTicketTypeID(_ticket.TicketTypeID);

					if (ticketTypes.Count > 0)
					{
						string errorMessage = string.Format("The ticket status was not updated because the incident state '{0}' does not exist as a status for the ticket type '{1}'.", state, ticketTypes[0].Name);
						_log.WriteToCrmErrorReport(_loginUser, errorMessage, _ticket.OrganizationID, _ticket.TicketID.ToString(), "ticket", "Status", state, "update", IntegrationType.ServiceNow, Enums.GetDescription(IntegrationOrientation.IntoTeamSupport), _crmErrors, logText: true);
					}
				}
			}

			if (wasUpdated)
			{
				_ticket.Collection.Save();
				refreshTicket = true;
			}

			return wasUpdated;
		}

		/// <summary>
		/// Process the updated fields received from ServiceNow checking vs the Custom Mapping fields setup in the ServiceNow integration in TeamSupport.
		/// </summary>
		/// <param name="jObject">The jObject parsed from the json string received from ServiceSnow</param>
		private bool ProcessInboundCustomMappings(JObject jObject, TicketLinkToSnowItem ticketLink)
		{
			bool wasUpdated = false;
			Tickets ticketFieldMaps = new Tickets(_loginUser);
			CustomValue customValue;
			CustomValues customValues = new CustomValues(_loginUser);
			customValues.LoadExistingOnlyByReferenceType(_ticket.OrganizationID, ReferenceType.Tickets, _ticket.TicketID);
			CRMLinkFields customMappingFields = new CRMLinkFields(_loginUser);
			customMappingFields.LoadByObjectTypeAndCustomFieldAuxID("Ticket", _crmLinkItem.CRMLinkID, _ticket.TicketTypeID);

			foreach (KeyValuePair<string, JToken> token in jObject)
			{
				if (token.Key.ToString() != _stateKey && token.Key.ToString() != _privateActionKey && token.Key.ToString() != _publicActionKey)
				{
					CRMLinkField mappedField = customMappingFields.Where(p => p.CRMFieldName == token.Key.ToString()).FirstOrDefault();

					if (mappedField != null)
					{
						if (mappedField.CustomFieldID == null && _ticket.BaseCollection.Table.Columns.Contains(mappedField.TSFieldName) && _ticket.Row[mappedField.TSFieldName] != null)
						{
							FieldMapItem fieldMapItem = ticketFieldMaps.FieldMap.Items.Where(p => p.PrivateName == mappedField.TSFieldName).FirstOrDefault();

							if (fieldMapItem != null && (!fieldMapItem.Insert || !fieldMapItem.Update))
							{
								_log.WriteToCrmErrorReport(_loginUser,
															string.Format("The field {0} is read-only and cannot be updated", mappedField.TSFieldName),
															_crmLinkItem.OrganizationID,
															_ticket.TicketID.ToString(),
															Enums.GetDescription(IntegrationObject.Ticket).ToLower(),
															mappedField.TSFieldName,
															token.Value.ToString(),
															"update",
															IntegrationType.ServiceNow,
															Enums.GetDescription(IntegrationOrientation.IntoTeamSupport),
															_crmErrors);
							}
							else if (_ticket.Row[mappedField.TSFieldName].ToString() != token.Value.ToString())
							{
								_ticket.Row[mappedField.TSFieldName] = token.Value.ToString();
								wasUpdated = true;
								_log.ClearCrmLinkError(_ticket.TicketID.ToString(), mappedField.TSFieldName, _crmErrors);
							}
						}
						else if (mappedField.CustomFieldID == null && !_ticket.BaseCollection.Table.Columns.Contains(mappedField.TSFieldName))
						{
							string mappedFieldName = mappedField.TSFieldName;
							int? ticketsTableRelatedValue = null;
							GetTicketsTableRelatedFieldValue(ref mappedFieldName, token.Value.ToString(), ref ticketsTableRelatedValue);

							if ((int)_ticket.Row[mappedFieldName] != ticketsTableRelatedValue)
							{
								if (!_ticket.Collection.Table.Columns[mappedFieldName].AllowDBNull && ticketsTableRelatedValue == null)
								{
									_log.WriteToCrmErrorReport(_loginUser,
																string.Format("The value '{0}' of the field '{1}' was not found for the mapped TeamSupport field '{2}' and it does not accept NULL therefore the current value was not changed.", token.Value.ToString(), mappedFieldName, mappedField.TSFieldName),
																_crmLinkItem.OrganizationID,
																_ticket.TicketID.ToString(),
																Enums.GetDescription(IntegrationObject.Ticket).ToLower(),
																mappedFieldName,
																token.Value.ToString(),
																"update",
																IntegrationType.ServiceNow,
																Enums.GetDescription(IntegrationOrientation.IntoTeamSupport),
																_crmErrors);
								}
								else
								{
									_ticket.Row[mappedFieldName] = ticketsTableRelatedValue;
									wasUpdated = true;
									_log.ClearCrmLinkError(_ticket.TicketID.ToString(), mappedFieldName, _crmErrors);
								}
							}
						}
						else
						{
							customValue = customValues.Where(p => p.CustomFieldID == mappedField.CustomFieldID && p.RefID == _ticket.TicketID).FirstOrDefault();

							if (customValue != null)
							{
								customValue.Value = token.Value.ToString();
								customValue.DateModified = DateTime.Now;
							}
							else
							{
								//not found. add it.
								customValue = (new CustomValues(_loginUser)).AddNewCustomValue();
								customValue.RefID = _ticket.TicketID;
								customValue.CustomFieldID = (int)mappedField.CustomFieldID;
								customValue.Value = token.Value.ToString();
							}

							try
							{
								customValue.Collection.Save();
								wasUpdated = true;
							}
							catch (Exception ex)
							{
								_log.Write(string.Format("Error updating the custom field {0}{1}{2}", customValue.Name, Environment.NewLine, ex.Message));
								_log.Write(ex.StackTrace);
							}
						}
					}
				}
			}

			if (wasUpdated)
			{
				_ticket.Collection.Save();
				ticketLink.DateModifiedBySync = DateTime.UtcNow;
				ticketLink.Collection.Save();
			}

			return wasUpdated;
		}

		/// <summary>
		/// Create the action received from ServiceNow. It could be either public or private.
		/// It should only be ONE action per request. So if one is found the other shouldn't.
		/// </summary>
		/// <param name="jObject">The jObject parsed from the json string received from ServiceSnow</param>
		private bool ProcessInboundAction(JObject jObject)
		{
			bool wasActionAdded = false;
			Dictionary<string, WorkNoteComment> actions = new Dictionary<string, WorkNoteComment>();
			var expandoConverter = new Newtonsoft.Json.Converters.ExpandoObjectConverter();
			JToken action = jObject.SelectProperty(_privateActionKey);
			dynamic journalObject;

			if (action != null && !string.IsNullOrEmpty(action.ToString()))
			{
				journalObject = JsonConvert.DeserializeObject<ExpandoObject>(action.ToString(), expandoConverter);
				actions.Add(_privateActionKey, new WorkNoteComment { Id = journalObject.id, Value = journalObject.value });
			}

			action = jObject.SelectProperty(_publicActionKey);

			if (action != null && !string.IsNullOrEmpty(action.ToString()))
			{
				journalObject = JsonConvert.DeserializeObject<ExpandoObject>(action.ToString(), expandoConverter);
				actions.Add(_publicActionKey, new WorkNoteComment { Id = journalObject.id, Value = journalObject.value });
			}

			if (actions.Any())
			{
				int newActionTypeId = GetNewActionType();

				string firstLine = "<p><em>Comment added in ServiceNow</em></p> <p>&nbsp;</p>";

				foreach (KeyValuePair<string, WorkNoteComment> singleAction in actions)
				{
					ActionLinkToSnow actionLinkSynced = new ActionLinkToSnow(_loginUser);
					actionLinkSynced.GetByActionAppID(singleAction.Value.Id);

					bool addNewAction = false;

					//is the existing action linked for this ticket?
					if (actionLinkSynced.Any())
					{
						Data.Action ticketAction = Actions.GetActionByID(_loginUser, actionLinkSynced[0].ActionID);
						addNewAction = ticketAction.TicketID != _ticket.TicketID;
					}

					//If this action wasn't added by the creation of the Incident previously then add, else ignore (skip) to avoid loops.
					//Also check if this action wasn't the one sent from TS to ServiceNow already. <-- Should follow the format of the method BuildCommentBody
					if ((actionLinkSynced == null || !actionLinkSynced.Any() || addNewAction)
						&& !singleAction.Value.Value.Contains("TeamSupport ticket #" + _ticket.TicketNumber.ToString() + " comment #"))
					{
						Actions newActions = new Actions(_loginUser);
						Data.Action newAction = newActions.AddNewAction();
						newAction.TicketID = _ticket.TicketID;
						newAction.ActionTypeID = newActionTypeId;
						newAction.Description = firstLine + singleAction.Value.Value;
						newAction.IsVisibleOnPortal = singleAction.Key == _publicActionKey;
						newActions.Save();

						AddActionLink(newActions[0].ActionID, singleAction.Value.Id);
						_log.Write(string.Format("Action created. ActionId: {0}, ServiceNow comment Id: {1}. *This will in turn create an outbound webhook that should be ignored (maybe seen below this entry)", newActions[0].ActionID, singleAction.Value.Id));
						wasActionAdded = true;
					}
					else if (actionLinkSynced.Any())
					{
						_log.Write(string.Format("Inbound action not created because it is an Action that exists in TeamSupport that was sent already to ServiceNow. ActionId: {0}", actionLinkSynced[0].ActionID));
					}
				}
			}

			return wasActionAdded;
		}

		/// <summary>
		/// Validates the posted data received from ServiceNow to check for incidentId and if the ticketlink and crmlink exist and is active.
		/// It also initializes some of the objects used later in the process. _ticketLinks, _crmLinkItem
		/// </summary>
		/// <param name="jobject"></param>
		/// <returns>True/False depending on the result of the validation.</returns>
		private bool IsInboundValid(JObject jobject)
		{
			bool isValid = false;
			_ticketLinks = new TicketLinkToSnow(_loginUser);

			JToken incidentId = jobject.SelectProperty("incidentid");

			if (incidentId != null)
			{
				_ticketLinks.LoadByAppId(incidentId.ToString());

				if (_ticketLinks != null && _ticketLinks.Count > 0)
				{
					_crmLinkItem = CRMLinkTable.GetCRMLinkTableItem(_loginUser, (int)_ticketLinks[0].CrmLinkID);

					if (_crmLinkItem != null && _crmLinkItem.Active)
					{
						isValid = true;
					}
				}
				else
				{
					_log.Write(string.Format("The Incident Id {0} is not linked (or no longer) to any ticket.", incidentId.ToString()));
				}
			}
			else
			{
				_log.Write(string.Format("The IncidentId was found in the posted data from ServiceNow without it we cannot find a link with any Ticket. {0}{1}", Environment.NewLine, _webhook.BodyData));
			}

			return isValid;
		}

		/// <summary>
		/// Create the Work Note or Comment in the incident specified. Then adds the ActionLinkToSnow record.
		/// </summary>
		/// <param name="crmLink">CrmLink object to check for ServiceNow credentials.</param>
		/// <param name="action">Action object that will be sent to ServiceNow</param>
		private void ProcessOuboundAction(Data.Action action)
		{
			if (action.ActionTypeID == _crmLinkItem.ActionTypeIDToPush || _crmLinkItem.ActionTypeIDToPush == null)
			{
				var expandoConverter = new Newtonsoft.Json.Converters.ExpandoObjectConverter();

				if (_ticket == null)
				{
					_ticket = Tickets.GetTicket(_loginUser, action.TicketID);
				}

				_crmErrors = new CRMLinkErrors(_loginUser);
				_crmErrors.LoadByOperationAndObjectId(_ticket.OrganizationID, Enums.GetDescription(IntegrationType.ServiceNow), Enums.GetDescription(IntegrationOrientation.OutToServiceNow), "action", action.ActionID.ToString());

				string url = GetFullUrl(_crmLinkItem.HostName, "api/now/table/incident/" + _ticketLinks[0].AppId);
				string encodedCredentials = DataUtils.GetEncodedCredentials(_crmLinkItem.Username, _crmLinkItem.Password);
				int actionPosition = Actions.GetActionPosition(_loginUser, action.ActionID);

				string comment = BuildCommentBody(_ticket.TicketNumber.ToString(), action.Description, actionPosition, action.CreatorID);
				string bodyData = string.Empty;
				string commentWorkNote = "^element={0}^ORDERBYDESCsys_created_on&sysparm_fields=element_id,value,sys_id";

				if (action.IsVisibleOnPortal)
				{
					bodyData = JsonConvert.SerializeObject(new { comments = comment });
					commentWorkNote = string.Format(commentWorkNote, "comments");
				}
				else
				{
					bodyData = JsonConvert.SerializeObject(new { work_notes = comment });
					commentWorkNote = string.Format(commentWorkNote, "work_notes");
				}

				string result = string.Empty;
				HttpStatusCode resultStatus = MakeHttpRequest(url, encodedCredentials, ref result, "PUT", "application/json", bodyData);

				if (WasRequestSuccessful(resultStatus, result))
				{
					string appId = _ticketLinks[0].AppId;
					dynamic incidentObject = JsonConvert.DeserializeObject<ExpandoObject>(result, expandoConverter);

					if (((IDictionary<string, object>)incidentObject).ContainsKey("result") && ((IDictionary<string, object>)incidentObject.result).ContainsKey("sys_id"))
					{
						//We need to make an additional call (GET) to servicenow to get the commentId.
						url = GetFullUrl(_crmLinkItem.HostName, "api/now/table/sys_journal_field?sysparm_query=element_id=" + appId + commentWorkNote);

						result = string.Empty;
						resultStatus = MakeHttpRequest(url, encodedCredentials, ref result, "GET", "application/json", string.Empty);

						if (resultStatus == HttpStatusCode.OK && !string.IsNullOrEmpty(result))
                        {
                            incidentObject = JsonConvert.DeserializeObject<ExpandoObject>(result, expandoConverter);

                            for (int i = 0; i < incidentObject.result.Count; i++)
                            {
                                string value = incidentObject.result[i].value;
                                string commentWorkNoteId = incidentObject.result[i].sys_id;

                                if (value.ToLower().Contains("teamsupport") && value.Contains(_ticket.TicketNumber.ToString()))
                                {
                                    AddActionLink(action.ActionID, commentWorkNoteId);
                                    break;
                                }
                            }

                            _log.Write("Created comment/work_note in ServiceNow for the action");
                            _log.ClearCrmLinkError(action.ActionID.ToString(), string.Empty, _crmErrors);

                            result = TeamSupport.Data.Quarantine.ServiceQ.UpdloadAttachments(_loginUser, _crmErrors, _log, action, encodedCredentials, result, appId, _crmLinkItem.HostName);
                        }
                    }
					else
					{
						_log.Write(string.Format("The comment/work_note was added to the incident id ({0}) but didn't get the response (GET) back to link the comment/work_note Id to the action id ({1})", appId, action.ActionID));
					}
				}
			}
			else
			{
				_log.Write(string.Format("The action (id: {0}, type: {1}) was not synced to ServiceNow because it is not of the type to push selected ({2}) in the integration settings.", action.ActionID, action.ActionTypeID, _crmLinkItem.ActionTypeIDToPush));
			}
		}


        private void ProcessOutboundTicket(bool isNew = true)
		{
			_crmErrors = new CRMLinkErrors(_loginUser);
			_crmErrors.LoadByOperationAndObjectId(_ticket.OrganizationID, Enums.GetDescription(IntegrationType.ServiceNow), Enums.GetDescription(IntegrationOrientation.OutToServiceNow), "ticket", _ticket.TicketID.ToString());

			dynamic dynamicBodyData = new ExpandoObject();
			string bodyData = string.Empty;
			string url = GetFullUrl(_crmLinkItem.HostName, "api/now/table/incident");
			string encodedCredentials = DataUtils.GetEncodedCredentials(_crmLinkItem.Username, _crmLinkItem.Password);
			TicketsViewItem ticketView = TicketsView.GetTicketsViewItem(_loginUser, _ticket.TicketID);
			Actions ticketActions = new Actions(_loginUser);
			ticketActions.LoadByTicketID(ticketView.TicketID);
			int actionDescriptionId = 0;
			string result = string.Empty;
			HttpStatusCode resultStatus = HttpStatusCode.Unused;
			var expandoConverter = new Newtonsoft.Json.Converters.ExpandoObjectConverter();
			bool createOrUpdate = false;

			if (isNew)
			{
				dynamicBodyData.short_description = _ticket.Name;
				dynamicBodyData.state = ticketView.Status;

				if (ticketActions.Any())
				{
					Data.Action actionDescription = ticketActions.OrderBy(p => p.ActionID).Where(p => p.SystemActionTypeID == SystemActionType.Description).FirstOrDefault();

					if (actionDescription != null)
					{
						actionDescriptionId = actionDescription.ActionID;

						if (actionDescription.IsVisibleOnPortal)
						{
							dynamicBodyData.comments = BuildCommentBody(_ticket.TicketNumber.ToString(), actionDescription.Description, 1, actionDescription.CreatorID, isTicketDescription: true);
						}
						else
						{
							dynamicBodyData.work_notes = BuildCommentBody(_ticket.TicketNumber.ToString(), actionDescription.Description, 1, actionDescription.CreatorID, isTicketDescription: true);
						}
					}
				}

				ProcessOutboundCustomMappings(ticketView, ref dynamicBodyData);
				createOrUpdate = true;
			}
			else
			{
				//Incident already exists, we are not updating them so we'll just link the ticket to it. For that we need to GET it first to get its sys_id for the PATCH
				string findExistingUrl = url + "?sysparm_query=number=" + _ticketLinks[0].Number;
				resultStatus = MakeHttpRequest(findExistingUrl, encodedCredentials, ref result, "GET", "application/json", string.Empty);

				if (WasRequestSuccessful(resultStatus, result))
				{
					dynamic existingIncidentObject = JsonConvert.DeserializeObject<ExpandoObject>(result, expandoConverter);

					if (((IDictionary<string, object>)existingIncidentObject).ContainsKey("result"))
					{
						url += "/" + existingIncidentObject.result[0].sys_id;
						createOrUpdate = true;
					}
				}
			}

			if (createOrUpdate)
			{
				string domain = SystemSettings.ReadStringForCrmService(_loginUser, "AppDomain", "https://app.teamsupport.com");
				string teamSupportUrl = string.Format("{0}/?TicketNumber={1}", domain, _ticket.TicketNumber);
				((IDictionary<string, object>)dynamicBodyData)[_teamSupportUrlField] = teamSupportUrl;

				bodyData = JsonConvert.SerializeObject(dynamicBodyData);
				resultStatus = MakeHttpRequest(url, encodedCredentials, ref result, isNew ? "POST" : "PATCH", "application/json", bodyData);

				if (WasRequestSuccessful(resultStatus, result))
				{
					dynamic incidentObject = JsonConvert.DeserializeObject<ExpandoObject>(result, expandoConverter);
						
					if (((IDictionary<string, object>)incidentObject).ContainsKey("result") 
						&& ((IDictionary<string, object>)incidentObject.result).ContainsKey("sys_id"))
					{
						_ticketLinks[0].AppId = incidentObject.result.sys_id;
						_ticketLinks[0].Number = incidentObject.result.number;
						_ticketLinks[0].State = incidentObject.result.state;
						_ticketLinks[0].URL = GetFullUrl(_crmLinkItem.HostName, string.Format("nav_to.do?uri=incident.do?sys_id={0}", incidentObject.result.sys_id));
						_ticketLinks[0].DateModifiedBySync = DateTime.UtcNow;
						_ticketLinks.Save();

						if (isNew)
						{
							_log.Write(string.Format("The incident was created successfully (id: {0}) and linked to the ticket, sending the actions now.", incidentObject.result.sys_id));
						}
						else
						{
							_log.Write(string.Format("The incident was linked successfully (id: {0}) to the ticket, sending the actions now.", incidentObject.result.sys_id));
						}

						_log.ClearCrmLinkError(_ticketLinks[0].TicketID.ToString(), string.Empty, _crmErrors);

						foreach (Data.Action action in ticketActions.OrderBy(p => p.ActionID))
						{
							//The action that is the description of the ticket was sent at the incident creation already.
							if (action.ActionID != actionDescriptionId)
							{
								_log.Write(string.Format("Processing Action Id {0}", action.ActionID));
								ProcessOuboundAction(action);
							}
						}

						if (_crmLinkItem.UpdateStatus ?? false)
						{
							TicketStatuses statuses = new TicketStatuses(_loginUser);
							statuses.LoadByOrganizationID(_crmLinkItem.OrganizationID);
							List<int> excludedTicketStatusToUpdate = _crmLinkItem.ExcludedTicketStatusUpdate.Split(',').Select(p => int.Parse(p)).ToList();
							TicketStatus newStatus = statuses.FindByName(_ticketLinks[0].State, _ticket.TicketTypeID);

							if (newStatus != null)
							{
								_ticket.TicketStatusID = newStatus.TicketStatusID;
								_ticket.Collection.Save();
							}
							else
							{
								TicketType ticketType = TicketTypes.GetTicketType(_loginUser, _ticket.TicketTypeID);
								_log.Write(String.Format("The Incident state -status- '{0}' in ServiceNow does not exist for the Ticket Type {1} (id: {2}).", _ticketLinks[0].State, ticketType.Name, _ticket.TicketTypeID));
							}
						}
						else
						{
							int newActionTypeId = GetNewActionType();
							Actions newAction = new Actions(_loginUser);
							newAction.AddNewAction();
							newAction[0].ActionTypeID = newActionTypeId;
							newAction[0].TicketID = _ticket.TicketID;
							newAction[0].Description = "Ticket has been synced with ServiceNow's incident " + incidentObject.result.number + " with state '" + _ticketLinks[0].State + "'.";
							newAction.Save();

							AddActionLink(newAction[0].ActionID);
							_log.Write("Added comment indicating linked incident state.");
						}

						TeamSupportUrlCheckAndCreate(incidentObject, encodedCredentials, teamSupportUrl);

						//Send pusher message to refresh ticket
						string message = string.Format("{0} updated ticket {1}.", Enums.GetDescription(IntegrationType.ServiceNow), _ticket.TicketNumber);
						SendPusherMessage("ticket-dispatch-" + _ticket.OrganizationID, "DisplayTicketUpdate", new { ticket = _ticket.TicketNumber.ToString(), update = message });
					}
				}
			}
			else
			{
				_log.Write("The Incident was not created nor updated (linked), check for possible error or exceptions.");
			}
		}

		/// <summary>
		/// Process the updated fields received from ServiceNow checking vs the Custom Mapping fields setup in the ServiceNow integration in TeamSupport.
		/// </summary>
		/// <param name="jObject">The jObject parsed from the json string received from ServiceSnow</param>
		private void ProcessOutboundCustomMappings(TicketsViewItem ticketView, ref dynamic dynamicBodyData)
		{
			Tickets ticketFieldMaps = new Tickets(_loginUser);
			CustomValue customValue;
			CustomValues customValues = new CustomValues(_loginUser);
			customValues.LoadExistingOnlyByReferenceType(ticketView.OrganizationID, ReferenceType.Tickets, ticketView.TicketID);
			CRMLinkFields customMappingFields = new CRMLinkFields(_loginUser);
			customMappingFields.LoadByObjectTypeAndCustomFieldAuxID("Ticket", _crmLinkItem.CRMLinkID, ticketView.TicketTypeID);

			foreach(CRMLinkField mappedField in customMappingFields)
			{
				if (mappedField.CustomFieldID == null && ticketView.BaseCollection.Table.Columns.Contains(mappedField.TSFieldName) && ticketView.Row[mappedField.TSFieldName] != null)
				{
					string value = customValues.Where(p => p.CustomFieldID == mappedField.CRMFieldID).Select(s => s.Value).FirstOrDefault();
					((IDictionary<string, object>)dynamicBodyData)[mappedField.TSFieldName] = value;
				}
				else
				{
					customValue = customValues.Where(p => p.CustomFieldID == mappedField.CustomFieldID && p.RefID == _ticket.TicketID).FirstOrDefault();

					if (customValue != null)
					{
						((IDictionary<string, object>)dynamicBodyData)[mappedField.TSFieldName] = customValue.Value;
					}
					else
					{
						_log.Write(string.Format("The custom mapped field {0} did not have any value in the ticket, not sent.", mappedField.TSFieldName));
					}
				}
			}
		}

		private void AddActionLink(int actionId, string appId = "-1")
		{
			ActionLinkToSnow actionLinkToSnow = new ActionLinkToSnow(_loginUser);
			ActionLinkToSnowItem newActionLinkToSnowItem = actionLinkToSnow.AddNewActionLinkToSnowItem();
			newActionLinkToSnowItem.ActionID = actionId;
			newActionLinkToSnowItem.AppId = appId;
			newActionLinkToSnowItem.DateModifiedBySync = DateTime.UtcNow;
			actionLinkToSnow.Save();
		}

		private int GetNewActionType()
		{
			ActionTypes actionTypes = new ActionTypes(_loginUser);
			actionTypes.LoadByOrganizationID(_ticket.OrganizationID);
			ActionType newActionType = actionTypes.Where(p => p.Name == "Comment").SingleOrDefault();
			int actionTypeId = 0;

			if (newActionType != null && newActionType.ActionTypeID > 0)
			{
				actionTypeId = newActionType.ActionTypeID;
			}
			else
			{
				actionTypes.LoadByPosition(_ticket.OrganizationID, 0);
				actionTypeId = actionTypes[0].ActionTypeID;
			}

			return actionTypeId;
		}

		/// <summary>
		/// Check if the TeamSupport URL exists in ServiceNow, if it doesn't then create it and update it.
		/// </summary>
		/// <param name="incidentObject">The result incident data when it was created/updated.</param>
		/// <param name="encodedCredentials">The ServiceNow encoded credentials for the API REST calls.</param>
		/// <param name="teamSupportUrl">Optional parameter, if empty the url of that link field is created.</param>
		private void TeamSupportUrlCheckAndCreate(dynamic incidentObject, string encodedCredentials, string teamSupportUrl = "")
		{
			if (!((IDictionary<string, object>)incidentObject.result).ContainsKey(_teamSupportUrlField))
			{
				if (string.IsNullOrEmpty(teamSupportUrl))
				{
					string domain = SystemSettings.ReadStringForCrmService(_loginUser, "AppDomain", "https://app.teamsupport.com");
					teamSupportUrl = string.Format("{0}/?TicketNumber={1}", domain, _ticket.TicketNumber);
				}

				_log.Write("The TeamSupport URL field does not exist in ServiceNow we'll create it now and update the incident with its value.");
				dynamic dynamicBodyData = new ExpandoObject();
				HttpStatusCode resultStatus = HttpStatusCode.Unused;
				string result = string.Empty;
				string url = GetFullUrl(_crmLinkItem.HostName, "api/now/table/sys_dictionary");

				dynamicBodyData.name = "incident";
				dynamicBodyData.column_label = "TeamSupport URL";
				dynamicBodyData.element = _teamSupportUrlField;
				dynamicBodyData.internal_type = "url";
				string bodyData = JsonConvert.SerializeObject(dynamicBodyData);
				resultStatus = MakeHttpRequest(url, encodedCredentials, ref result, "POST", "application/json", bodyData);

				if (resultStatus == HttpStatusCode.Created)
				{
					url = GetFullUrl(_crmLinkItem.HostName, "api/now/table/incident/" + incidentObject.result.sys_id);
					dynamicBodyData = new ExpandoObject();
					((IDictionary<string, object>)dynamicBodyData)[_teamSupportUrlField] = teamSupportUrl;
					bodyData = JsonConvert.SerializeObject(dynamicBodyData);
					resultStatus = MakeHttpRequest(url, encodedCredentials, ref result, "PATCH", "application/json", bodyData);

					if (resultStatus == HttpStatusCode.OK)
					{
						_log.Write("The TeamSupport URL field was created in ServiceNow and its value was updated for this incident. The creation of this field should only happen in the event the field is deleted in ServiceNow, at that point we will have issues with existing ticket/incidents linked.");
						_log.ClearCrmLinkError(_ticket.TicketID.ToString(), "TeamSupport Link", _crmErrors);
					}
					else
					{
						_log.WriteToCrmErrorReport(_loginUser,
													string.Format("The TeamSupport URL field was created successfully but was not able to update it. This will cause problems as the incident and ticket are not really linked so going forward there will not be anything synced between them. Ticket: {0} (id: {1}). Incident: {2} (id: {3})", _ticket.TicketNumber, _ticket.TicketID, incidentObject.result.number, incidentObject.result.sys_id),
													_ticket.OrganizationID,
													_ticket.TicketID.ToString(),
													Enums.GetDescription(IntegrationObject.Ticket).ToLower(),
													"TeamSupport Link",
													JsonConvert.SerializeObject(dynamicBodyData),
													"create",
													IntegrationType.ServiceNow,
													Enums.GetDescription(IntegrationOrientation.OutToServiceNow),
													_crmErrors,
													logText: true);
					}
				}
				else
				{
					_log.WriteToCrmErrorReport(_loginUser,
												string.Format("The TeamSupport URL field failed to be created. This will cause problems as the incident and ticket are not really linked so going forward there will not be anything synced between them. Ticket: {0} (id: {1}). Incident: {2} (id: {3})", _ticket.TicketNumber, _ticket.TicketID, incidentObject.result.number, incidentObject.result.sys_id),
												_ticket.OrganizationID,
												_ticket.TicketID.ToString(),
												Enums.GetDescription(IntegrationObject.Ticket).ToLower(),
												"TeamSupport Link",
												JsonConvert.SerializeObject(dynamicBodyData),
												"create",
												IntegrationType.ServiceNow,
												Enums.GetDescription(IntegrationOrientation.OutToServiceNow),
												_crmErrors,
												logText: true);
				}
			}
		}

		private bool WasRequestSuccessful(HttpStatusCode resultStatus, string result)
		{
			bool isSuccessful = false;

			if ((resultStatus == HttpStatusCode.OK || resultStatus == HttpStatusCode.Created) && !string.IsNullOrEmpty(result) && !result.Contains("fell asleep") && !result.Contains("sleepy instances"))
			{
				isSuccessful = true;
				_log.ClearCrmLinkError(_ticket.TicketID.ToString(), string.Empty, _crmErrors);
			}
			else if (resultStatus == HttpStatusCode.OK && !string.IsNullOrEmpty(result) && result.Contains("fell asleep") && result.Contains("sleepy instances"))
			{
				InstanceIsHibernating();
			}
			else if (resultStatus != HttpStatusCode.OK)
			{
				_log.WriteToCrmErrorReport(_loginUser,
											string.Format("The httpRequest to ServiceNow failed with a status code of '{0}', the pending request was deleted.", resultStatus),
											_ticket.OrganizationID,
											_ticket.TicketID.ToString(),
											Enums.GetDescription(IntegrationObject.Ticket).ToLower(),
											string.Empty,
											string.Empty,
											"insert",
											IntegrationType.ServiceNow,
											Enums.GetDescription(IntegrationOrientation.OutToServiceNow),
											_crmErrors,
											logText: true);
			}

			return isSuccessful;
		}

		private void InstanceIsHibernating()
		{
			_log.Write("The ServiceNow instance is Hibernating, the request cannot be completed. Re-Inserting the webhook request to attempt it later again.");
			WebHooksPending reInserted = new WebHooksPending(_loginUser);
			WebHooksPendingItem reInsertedItem = reInserted.AddNewWebHooksPendingItem();
			reInsertedItem.RefId = _webhook.RefId;
			reInsertedItem.RefType = _webhook.RefType;
			reInsertedItem.Type = _webhook.Type;
			reInsertedItem.BodyData = _webhook.BodyData;
			reInsertedItem.Inbound = _webhook.Inbound;
			reInsertedItem.IsProcessing = false;
			reInserted.Save();
		}

		private void GetTicketsTableRelatedFieldValue(ref string mappedFieldName, string mappedFieldValue, ref int? ticketsTableRelatedValue)
		{
			switch (mappedFieldName.ToLower())
			{
				case "severity":
					mappedFieldName = "TicketSeverityID";
					TicketSeverities severities = new TicketSeverities(_loginUser);
					severities.LoadByName(_ticket.OrganizationID, mappedFieldValue);

					if (severities.Any())
					{
						ticketsTableRelatedValue = severities[0].TicketSeverityID;
					}

					break;
				case "groupname":
					mappedFieldName = "GroupID";
					Groups groups = new Groups(_loginUser);
					groups.LoadByGroupName(_ticket.OrganizationID, mappedFieldValue);

					if (groups.Any())
					{
						ticketsTableRelatedValue = groups[0].GroupID;
					}

					break;
				case "username":
					mappedFieldName = "UserID";
					Users users = new Users(_loginUser);
					users.LoadByName(mappedFieldValue, _ticket.OrganizationID, false, false, false);

					if (users.Any())
					{
						ticketsTableRelatedValue = users[0].UserID;
					}

					break;
				default:
					break;
			}
		}

		private string GetFullUrl(string hostName, string path)
		{
			if (!hostName.EndsWith("/"))
			{
				hostName += "/";
			}

			return hostName + path;
		}

		private void SendPusherMessage(string channelName, string eventName, object message)
		{
			try
			{
				PusherOptions options = new PusherOptions();
				options.Encrypted = true;
				string pusherKey = SystemSettings.GetPusherKey();
				string pusherAppId = SystemSettings.GetPusherAppId();
				string pusherSecret = SystemSettings.GetPusherSecret();
				Pusher pusher = new Pusher(pusherAppId, pusherKey, pusherSecret, options);
				var result = pusher.Trigger(channelName, eventName, message);
			}
			catch (Exception ex)
			{
				_log.Write(string.Format("SendPusherMessage. {0}", ex.Message));
			}
		}


		private class WorkNoteComment
		{
			public string Id { get; set; }
			public string Value { get; set; }
		}
	}

	public static class JsonExtensions
	{
		public static IEnumerable<JToken> CaseSelectPropertyValues(this JToken token, string name)
		{
			var obj = token as JObject;

			if (obj == null)
				yield break;

			foreach (var property in obj.Properties())
			{
				if (name == null)
					yield return property.Value;
				else if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
					yield return property.Value;
			}
		}

		public static IEnumerable<JToken> CaseSelectPropertyValues(this IEnumerable<JToken> tokens, string name)
		{
			if (tokens == null)
				throw new ArgumentNullException();

			return tokens.SelectMany(t => t.CaseSelectPropertyValues(name));
		}

		public static JToken SelectProperty(this JToken token, string name)
		{
			var obj = token as JObject;

			foreach (var property in obj.Properties())
			{
				if (name == null)
					return property.Value;
				else if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
					return property.Value;
			}

			return null;
		}
	}
}
