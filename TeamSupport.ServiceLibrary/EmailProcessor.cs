using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using System.Globalization;
using System.Net.Mail;
using TeamSupport.Data.WebHooks;

namespace TeamSupport.ServiceLibrary
{
	[Serializable]
	public class EmailProcessor : ServiceThreadPoolProcess
	{
		class UserEmail
		{
			public UserEmail(int userID, string fname, string lname, string address, bool sendTicketsOnlyAfterHours)
			{
				this.UserID = userID;
				this.Name = fname + " " + lname;
				this.FirstName = fname;
				this.LastName = lname;
				this.Address = address;
				this.SendTicketsOnlyAfterHours = sendTicketsOnlyAfterHours;
			}

			public bool SendTicketsOnlyAfterHours { get; set; }
			public string Address { get; set; }
			public string Name { get; set; }
			public string FirstName { get; set; }
			public string LastName { get; set; }
			public int UserID { get; set; }

			public override string ToString()
			{
				return string.Format("{0} <{1}> ({2}) SendTicketsOnlyAfterHours: {3}", Name, Address, UserID.ToString(), SendTicketsOnlyAfterHours.ToString());
			}
		}

		private bool _isDebug = false;
		private bool _logEnabled = false;
		private int _currentEmailPostID;


		private static object _staticLock = new object();

		private static EmailPost GetNextEmailPost(string connectionString, int lockID, bool isDebug)
		{
			EmailPost result;
			LoginUser loginUser = new LoginUser(connectionString, -1, -1, null);
			lock (_staticLock)
			{
				if (isDebug)
				{
					SqlExecutor.ExecuteNonQuery(loginUser, "UPDATE EmailPosts SET HoldTime=30 WHERE HoldTime > 30");
				}

				result = EmailPosts.GetNextWaiting(loginUser, lockID.ToString());
			}
			return result;
		}

		public override void ReleaseAllLocks()
		{
			EmailPosts.UnlockAll(LoginUser);
		}

		public override void Run()
		{
			EmailPosts.UnlockThread(LoginUser, (int)_threadPosition);
			while (!IsStopped)
			{
				EmailPost emailPost = GetNextEmailPost(LoginUser.ConnectionString, (int)_threadPosition, _isDebug);
				if (emailPost == null)
				{
					System.Threading.Thread.Sleep(10000);
					continue;
				}

				try
				{
					try
					{
						if (emailPost.CreatorID != -5)
						{
							_isDebug = Settings.ReadBool("Debug", false);
							_logEnabled = Settings.ReadInt("LoggingEnabled", 0) == 1;

							Logs.WriteLine();
							Logs.WriteEvent("***********************************************************************************");
							Logs.WriteEvent("Processing Email Post  EmailPostID: " + emailPost.EmailPostID.ToString());
							Logs.WriteData(emailPost.Row);
							Logs.WriteEvent("***********************************************************************************");
							SetTimeZone(emailPost);
							ProcessEmail(emailPost);
						}
					}
					catch (Exception ex)
					{
						ExceptionLogs.LogException(LoginUser, ex, "Email", emailPost.Row);
					}
					finally
					{
						Logs.WriteEvent("Deleting from DB");
						emailPost.Collection.DeleteFromDB(emailPost.EmailPostID);
					}

					Logs.WriteEvent("Updating Health");
					UpdateHealth();
				}
				catch (Exception ex)
				{
					ExceptionLogs.LogException(LoginUser, ex, "Email", emailPost.Row);
				}
			}
		}

		public void SetTimeZone(EmailPost emailPost)
		{
			LoginUser.TimeZoneInfo = null;
			Organization organization = Users.GetTSOrganization(LoginUser, emailPost.CreatorID);
			if (organization != null)
			{
				try
				{
					LoginUser.TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(organization.TimeZoneID);
					Logs.WriteEvent("TimeZoneID: " + organization.TimeZoneID);
				}
				catch (Exception)
				{
					LoginUser.TimeZoneInfo = null;
				}

				try
				{
					LoginUser.OrganizationCulture = new CultureInfo(organization.CultureName);
					Logs.WriteEvent("Culture: " + organization.CultureName);
				}
				catch (Exception)
				{
					LoginUser.OrganizationCulture = new CultureInfo("en-US");
				}
			}

			if (LoginUser.TimeZoneInfo == null)
			{
				LoginUser.TimeZoneInfo = TimeZoneInfo.Utc;
			}

		}

		public void ProcessEmail(EmailPost emailPost)
		{
			_currentEmailPostID = emailPost.EmailPostID;
			Logs.WriteEvent("EmailPostType: " + emailPost.EmailPostType.ToString());

			switch (emailPost.EmailPostType)
			{
				case EmailPostType.TicketModified:
					List<int> actionList = new List<int>();

					if (!string.IsNullOrEmpty(emailPost.Param6))
					{
						Logs.WriteEvent("Actions (Param6): " + emailPost.Param6);
						string[] actions = emailPost.Param6.Split(';');

						foreach (string s in actions)
						{
							try
							{
								actionList.Add(int.Parse(s.Trim()));
							}
							catch (Exception)
							{
							}
						}
					}
					else
					{
						Logs.WriteEvent("Actions: None");
					}

					List<int> userList = new List<int>();

					if (!string.IsNullOrEmpty(emailPost.Param8))
					{
						Logs.WriteEvent("Users (Param8): " + emailPost.Param8);
						string[] users = emailPost.Param8.Split(';');

						foreach (string s in users)
						{
							try
							{
								userList.Add(int.Parse(s.Trim()));
							}
							catch (Exception)
							{
							}
						}
					}
					else
					{
						Logs.WriteEvent("Users: None");
					}

					ProcessTicketModified(GetIntParam(emailPost.Param1),
					  GetNullIntParam(emailPost.Param2),
					  GetNullIntParam(emailPost.Param3),
					  GetNullIntParam(emailPost.Param4),
					  GetNullIntParam(emailPost.Param5),
					  actionList.ToArray(),
					  userList.ToArray(),
					  GetIntParam(emailPost.Param7) == 1
					  );
					break;

				case EmailPostType.Reaction:
					ProcessReaction(emailPost.CreatorID, GetIntParam(emailPost.Param1), GetIntParam(emailPost.Param2), emailPost.Param3);
					break;
				case EmailPostType.TicketUpdateRequest:
					ProcessTicketUpdateRequest(GetIntParam(emailPost.Param1), GetIntParam(emailPost.Param2));
					break;
				case EmailPostType.TicketSendEmail:
					ProcessTicketSendEmail(GetIntParam(emailPost.Param1), GetIntParam(emailPost.Param2), emailPost.Param3, emailPost.Text1, GetIntParam(emailPost.Param4));
					break;
				case EmailPostType.WelcomeNewSignup:
					ProcessWelcomeNewSignup(GetIntParam(emailPost.Param1), emailPost.Param2);
					break;
				case EmailPostType.WelcomeTSUser:
					ProcessWelcomeTSUser(GetIntParam(emailPost.Param1), emailPost.Param2);
					break;
				case EmailPostType.WelcomePortalUser:
					ProcessWelcomePortalUser(GetIntParam(emailPost.Param1), emailPost.Param2);
					break;
				case EmailPostType.ResetTSPassword:
					ProcessResetTSPassword(GetIntParam(emailPost.Param1), emailPost.Param2);
					break;
				case EmailPostType.ResetPortalPassword:
					ProcessResetPortalPassword(GetIntParam(emailPost.Param1), emailPost.Param2);
					break;
				case EmailPostType.ChangedTSPassword:
					ProcessChangedTSPassword(GetIntParam(emailPost.Param1));
					break;
				case EmailPostType.ChangedPortalPassword:
					ProcessChangedPortalPassword(GetIntParam(emailPost.Param1));
					break;
				case EmailPostType.WelcomeCustomerHubUser:
					ProcessWelcomeCustomerHubUser(GetIntParam(emailPost.Param1), emailPost.Param2, GetIntParam(emailPost.Param3));
					break;
				case EmailPostType.ResetCustomerHubPassword:
					ProcessResetHubPassword(GetIntParam(emailPost.Param1), emailPost.Param2, GetIntParam(emailPost.Param3));
					break;
				case EmailPostType.InternalSignupNotification:
					ProcessSignUpNotification(GetIntParam(emailPost.Param1), emailPost.Param2, emailPost.Param3);
					break;
				case EmailPostType.NewDevice:
					ProcessNewDevice(GetIntParam(emailPost.Param1));
					break;
				case EmailPostType.TooManyAttempts:
					ProcessTooManyAttempts(GetIntParam(emailPost.Param1));
					break;

				default:
					break;
			}
		}

		private void AddMessage(int organizationID, string description, MailMessage message, string replyToAddress = null, string[] attachments = null, DateTime? timeToSend = null, Ticket ticket = null, List<UserEmail> recipientList = null)
		{
			Organization organization = Organizations.GetOrganization(LoginUser, organizationID);
			string replyAddress = organization.GetReplyToAddress(replyToAddress).Trim();

			int i = 0;
			while (i < message.To.Count)
			{
				MailAddress address = message.To[i];
				if (address.Address.ToLower().Trim() == message.From.Address.ToLower().Trim() || address.Address.ToLower().Trim() == replyAddress || address.Address.ToLower().Trim() == organization.SystemEmailID.ToString().Trim().ToLower() + "@teamsupport.com")
				{
					message.To.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}

			if (message.To.Count < 1) return;

			StringBuilder builder = new StringBuilder();
			builder.AppendLine(message.Body);
			message.HeadersEncoding = Encoding.UTF8;
			message.Body = builder.ToString();
			List<MailAddress> addresses = message.To.ToList();
			string body = message.Body;
			string subject = message.Subject;
			foreach (MailAddress address in addresses)
			{
				message.To.Clear();
				Logs.WriteEvent(string.Format("Adding email address [{0}]", address.ToString()));
				message.To.Add(GetMailAddress(address.Address, address.DisplayName));
				Logs.WriteEvent(string.Format("Successfuly added email address [{0}]", address.ToString()));
				message.HeadersEncoding = Encoding.UTF8;
				message.Body = body;
				message.Subject = subject;

				if (ticket != null && recipientList != null && recipientList.Any())
				{
					UserEmail userEmail = recipientList.Where(p => p.Address.ToLower() == address.Address.ToLower()).FirstOrDefault();
					EmailTemplates.ReplaceEmailRecipientParameters(LoginUser, message, ticket, userEmail != null ? userEmail.UserID : 0, userEmail != null ? userEmail.SendTicketsOnlyAfterHours : false);
				}

				Logs.WriteEvent(string.Format("Adding ReplyTo Address[{0}]", replyAddress.Replace("<", "").Replace(">", "")));
				MailAddress replyMailAddress = null;
				try
				{
					replyMailAddress = GetMailAddress(replyAddress);
				}
				catch (Exception)
				{
					replyMailAddress = GetMailAddress(organization.GetReplyToAddress());
				}

				message.From = replyMailAddress;
				EmailTemplates.ReplaceMailAddressParameters(message);

				Emails.AddEmail(LoginUser, organizationID, _currentEmailPostID, description, message, attachments, timeToSend);

				if (message.Subject == null) message.Subject = "";
				Logs.WriteEvent(string.Format("Queueing email [{0}] - {1}  Subject: {2}", description, address.ToString(), message.Subject));
			}
		}

		#region Ticket Processing

		public void ProcessTicketModified(int ticketID, int? oldUserID, int? oldGroupID, int? oldTicketStatusID, int? oldTicketSeverityID, int[] modifiedActions, int[] users, bool isNew)
		{
			Ticket ticket = Tickets.GetTicket(LoginUser, ticketID);
			int modifierID = ticket.ModifierID;

			Logs.WriteEvent(string.Format("TicketID: {0}", ticketID.ToString()));
			Logs.WriteEvent(string.Format("OldUserID: {0}", (oldUserID == null ? "NULL" : oldUserID.ToString())));
			Logs.WriteEvent(string.Format("OldGroupID: {0}", (oldGroupID == null ? "NULL" : oldGroupID.ToString())));
			Logs.WriteEvent(string.Format("OldTicketStatusID: {0}", (oldTicketStatusID == null ? "NULL" : oldTicketStatusID.ToString())));
			Logs.WriteEvent(string.Format("OldTicketSeverityID: {0}", (oldTicketSeverityID == null ? "NULL" : oldTicketSeverityID.ToString())));
			Logs.WriteEvent(string.Format("ModifierID: {0}", modifierID.ToString()));
			Logs.WriteEvent(string.Format("IsNew: {0}", isNew.ToString()));

			try
			{
				if (ticket == null)
				{
					Logs.WriteEvent("Ticket is NULL, canceling.");
					return;
				}

				Logs.WriteLine();
				Logs.WriteEvent("***********************************************************************************");
				Logs.WriteEvent("Processing Ticket Email Post  TicketID: " + ticket.TicketID.ToString());
				Logs.WriteData(ticket.Row);
				Logs.WriteEvent("***********************************************************************************");

				User modifier = Users.GetUser(LoginUser, modifierID);
				string modifierName = modifier == null ? GetOrganizationName(ticket.OrganizationID) : modifier.FirstLastName;
				string modifierTitle = modifier == null ? string.Empty : modifier.Title;
				Logs.WriteEvent("Modifier: " + modifierName);

				Organization ticketOrganization = Organizations.GetOrganization(LoginUser, ticket.OrganizationID);
				Logs.WriteEvent("OrganizationID: " + ticketOrganization.OrganizationID.ToString());

				TicketStatus status = TicketStatuses.GetTicketStatus(LoginUser, ticket.TicketStatusID);
				Logs.WriteEvent("DisableStatusNotification: " + OrganizationSettings.ReadString(LoginUser, ticket.OrganizationID, "DisableStatusNotification", "False"));
				Logs.WriteEventFormat("NoAttachmentsInOutboundEmail: {0} ({1} them)", ticketOrganization.NoAttachmentsInOutboundEmail.ToString(), ticketOrganization.NoAttachmentsInOutboundEmail ? "exclude" : "include");

				if (ticketOrganization.NoAttachmentsInOutboundEmail && !string.IsNullOrEmpty(ticketOrganization.NoAttachmentsInOutboundExcludeProductLine))
				{
					Logs.WriteEventFormat("Except for the following Product Lines ids (still include attachments): {0}", ticketOrganization.NoAttachmentsInOutboundExcludeProductLine);
				}

				if (oldTicketStatusID != null && bool.Parse(OrganizationSettings.ReadString(LoginUser, ticket.OrganizationID, "DisableStatusNotification", "False")))
				{
					Logs.WriteEvent(string.Format("TicketStatusID: {0}  IsClosedEmail: {1}", ticket.TicketStatusID.ToString(), status.IsClosed.ToString()));
					if (!status.IsClosedEmail) oldTicketStatusID = null;
				}

				Actions actions = new Actions(LoginUser);
				actions.LoadByActionIDs(modifiedActions);
				Logs.WriteEvent(string.Format("{0} Actions Loaded", actions.Count.ToString()));

				int publicActionCount = 0;

				foreach (Data.Action action in actions)
				{
					if (action.IsVisibleOnPortal) publicActionCount++;
				}

				Logs.WriteEvent(string.Format("{0} Public Actions Loaded", publicActionCount.ToString()));
				Logs.WriteEvent("Processing Ticket Assignment");
				AddMessageTicketAssignment(ticket, oldUserID, oldGroupID, isNew, modifier, ticketOrganization);

				Logs.WriteEvent("Processing Advanced Portal");
				AddMessagePortalTicketModified(ticket, isNew, oldTicketStatusID, publicActionCount > 0, users, modifierName, modifierID, modifierTitle, ticketOrganization, false);

				Logs.WriteEvent("Processing Basic Portal");
				AddMessagePortalTicketModified(ticket, isNew, oldTicketStatusID, publicActionCount > 0, users, modifierName, modifierID, modifierTitle, ticketOrganization, true);

				Logs.WriteEvent("Processing Internal Modified");
				AddMessageInternalTicketModified(ticket, oldUserID, oldGroupID, isNew, oldTicketStatusID, oldTicketSeverityID, !actions.IsEmpty, modifierName, modifier == null ? -1 : modifier.UserID, ticketOrganization);

				if (isNew)
				{
					Logs.WriteEvent("Processing New Ticket");
					User creator = Users.GetUser(LoginUser, ticket.CreatorID);
					AddMessageNewTicket(ticket, creator, ticketOrganization);
				}

				try
				{
					// web hooks
					if (!_isDebug && ticket.OrganizationID == 1078)
					{
						if (ticket.TicketSeverityID == 3169 && (oldTicketSeverityID == null || oldTicketSeverityID != 3169))
						{
							SendUrgentTicketToSlack(ticket.GetTicketView());
						}

						if (status.IsEmailResponse)
						{
							if (oldTicketStatusID == null)
							{
								SendCustomerRespondedToSlack(ticket.GetTicketView());
							}
							else
							{
								TicketStatus oldStatus = TicketStatuses.GetTicketStatus(LoginUser, (int)oldTicketStatusID);

								if (oldStatus != null && !oldStatus.IsEmailResponse)
								{
									SendCustomerRespondedToSlack(ticket.GetTicketView());
								}
							}
						}

						if (ticket.TicketNumber == 8743)
						{
							SendSchemaUpdateToSlack(ticket.GetTicketView());
						}
					}
				}
				catch (Exception exHooks)
				{
					Logs.WriteException(exHooks);
				}
			}
			catch (Exception ex)
			{
				Logs.WriteException(ex);
				throw;
			}
		}

		private void SendCustomerRespondedToSlack(TicketsViewItem ticket)
		{
			try
			{
				string user = string.IsNullOrWhiteSpace(ticket.UserName) ? "" : ticket.UserName;
				string customers = string.IsNullOrWhiteSpace(ticket.Customers) ? "" : ticket.Customers;
				SlackMessage message = new SlackMessage(LoginUser);
				message.TextPlain = string.Format("A customer responded to Ticket {0} - {1}\nSeverity is {2}\nCustomers are {3}\nAssigned to {4}", ticket.TicketNumber.ToString(), ticket.Name, ticket.Severity, customers, user);
				message.TextRich = string.Format("A customer responded to Ticket {0} - {1}", ticket.TicketNumber.ToString(), ticket.Name);
				message.Color = "#D00000";
				message.AddField("Customers", customers, true);
				message.AddField("Assigned To", user, true);
				message.AddField("Severity", ticket.Severity, true);

				// send to channel
				message.Send("#customer-responded");
				// send to user
				message.Send(ticket.UserID);
			}
			catch (Exception ex)
			{
				Logs.WriteEvent("Error sending customer responded");
				Logs.WriteException(ex);
				ExceptionLogs.LogException(LoginUser, ex, "Webhooks", ticket.Row);
			}
		}

		private void SendUrgentTicketToSlack(TicketsViewItem ticket)
		{
			try
			{
				string user = string.IsNullOrWhiteSpace(ticket.UserName) ? "" : ticket.UserName;
				string customers = string.IsNullOrWhiteSpace(ticket.Customers) ? "" : ticket.Customers;
				SlackMessage message = new SlackMessage(LoginUser);
				message.TextPlain = string.Format("Urgent Ticket {0} - {1}\nSeverity is {2}\nCustomers are {3}\nAssigned to {4}", ticket.TicketNumber.ToString(), ticket.Name, ticket.Severity, customers, user);
				message.TextRich = string.Format("Urgent Ticket {0} - {1}", ticket.TicketNumber.ToString(), ticket.Name);
				message.Color = "#D00000";
				message.AddField("Customers", customers, true);
				message.AddField("Assigned To", user, true);
				message.AddField("Severity", ticket.Severity, true);

				// send to channel
				message.Send("#urgent-tickets");
				// send to user
				message.Send(ticket.UserID);
			}
			catch (Exception ex)
			{
				Logs.WriteEvent("Error sending urgent ticket");
				Logs.WriteException(ex);
				ExceptionLogs.LogException(LoginUser, ex, "Webhooks", ticket.Row);
			}
		}

		private void SendSchemaUpdateToSlack(TicketsViewItem ticket)
		{
			try
			{
				SlackMessage message = new SlackMessage(LoginUser);
				message.TextPlain = string.Format("{0} added a schema update to: {1}.", ticket.ModifierName, ticket.TicketNumber);
				message.TextRich = string.Format("{0} added a schema update to: {1}.", ticket.ModifierName, ticket.TicketNumber);
				message.Color = "#D00000";

				// send to channel
				message.Send("#schema-changes");
			}
			catch (Exception ex)
			{
				Logs.WriteEvent("Error sending schema update");
				Logs.WriteException(ex);
				ExceptionLogs.LogException(LoginUser, ex, "Webhooks", ticket.Row);
			}
		}


		private string GetOrganizationName(int organizationID)
		{
			Organization org = Organizations.GetOrganization(LoginUser, organizationID);
			return org == null ? "" : org.Name;
		}

		private void AddMessageTicketAssignment(Ticket ticket, int? oldUserID, int? oldGroupID, bool isNew, User modifier, Organization ticketOrganization)
		{
			try
			{
				if (oldUserID == null && oldGroupID == null && !isNew)
				{
					Logs.WriteEvent("OldUserID = NULL AND OldGroupID = NULL && NOT IsNew, returning");
					return;
				}

				int modifierID = modifier == null ? -1 : modifier.UserID;
				ticket.UserID = (ticket.UserID == null) ? -1 : ticket.UserID;
				string modifierName = modifier == null ? GetOrganizationName(ticket.OrganizationID) : modifier.FirstLastName;
				Logs.WriteEventFormat("Modifier Name: {0}", modifierName);

				Actions actions = new Actions(LoginUser);
				actions.LoadLatestByTicket(ticket.TicketID, false);
				Logs.WriteEventFormat("{0} Latest actions loaded", actions.Count.ToString());

				string subject = " [pvt]";
				Logs.WriteEvent("Subject = [pvt]");
				if (ticket.IsVisibleOnPortal && !actions.IsEmpty && actions[0].IsVisibleOnPortal) subject = "";
				Logs.WriteEvent("Subject = '', VisibleOnPortal=True AND Actions not empty AND Action[0] is visible on portal");

				List<string> fileNames = new List<string>();

                if (CanIncludeAttachments(ticket, actions, ticketOrganization))
                    TeamSupport.Data.Quarantine.ServiceQ.AddAttachmentFileNames2(actions, fileNames, Logs);

                string emailReplyToAddress = GetEmailReplyToAddress(LoginUser, ticket);

				if (ticket.UserID != null && oldUserID != null)
				{
					Logs.WriteEvent("Ticket.UserID is not null AND old userid is not null");

					User owner = Users.GetUser(LoginUser, (int)ticket.UserID);
					if (owner != null)
					{
						Logs.WriteEventFormat("Owner: {0} ({1}) - ReceiveTicketNotifications:{2}", owner.DisplayName, owner.UserID.ToString(), owner.ReceiveTicketNotifications.ToString());
						Logs.WriteParam("ModifierID", modifierID.ToString());
						if (modifierID != owner.UserID && owner.ReceiveTicketNotifications)
						{
							Logs.WriteEvent("Getting Ticket Assignment Email");
							MailMessage message = EmailTemplates.GetTicketAssignmentUser(LoginUser, modifierName, ticket.GetTicketView());
							message.To.Add(GetMailAddress(owner.Email, owner.FirstLastName));
							message.Subject = message.Subject + subject;
							EmailTemplates.ReplaceEmailRecipientParameters(LoginUser, message, ticket, owner.UserID, owner.OnlyEmailAfterHours);

							Logs.WriteEvent("Added to ticket log");
							ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, "Ticket assignment email sent to " + message.To[0].DisplayName);
							UserEmail userEmail = new UserEmail(owner.UserID, owner.FirstName, owner.LastName, "", owner.OnlyEmailAfterHours);
							AddMessage(ticketOrganization.OrganizationID, "Ticket Assignment [" + ticket.TicketNumber.ToString() + "]", message, emailReplyToAddress, fileNames.ToArray());
						}
					}
				}

				if (ticket.GroupID != null && oldGroupID != null)
				{
					Logs.WriteEvent("Ticket.GroupID is not null");
					Group owner = Groups.GetGroup(LoginUser, (int)ticket.GroupID);
					Logs.WriteEventFormat("Group Owner: {0} ({1})", owner.Name, owner.GroupID.ToString());
					List<UserEmail> list = new List<UserEmail>();
					AddTicketOwners(list, ticket);
					if (ticket.UserID != null)
					{
						RemoveUser(list, (int)ticket.UserID);
						Logs.WriteParam("Removing UserID from list: ", ticket.UserID.ToString());
					}
					if (ticket.ModifierID != null)
					{
						RemoveUser(list, (int)modifierID); //User Who Made Modifications should not get email about modifications RemoveUser(users, (int)modifierID); //User Who Made Modifications should not get email about modifications
						Logs.WriteParam("Removing Modifier from list: ", ticket.ModifierID.ToString());
					}

					RemoveUser(list, (int)modifierID); //User Who Made Modifications should not get email about modifications RemoveUser(users, (int)modifierID); //User Who Made Modifications should not get email about modifications

					MailMessage message = EmailTemplates.GetTicketAssignmentGroup(LoginUser, modifierName, ticket.GetTicketView());
					AddUsersToAddresses(message.To, list, modifierID);
					message.Subject = message.Subject + subject;
					foreach (MailAddress mailAddress in message.To)
					{
						ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, "Ticket assignment email sent to " + mailAddress.Address);
					}

					AddMessage(ticketOrganization.OrganizationID, "Ticket Assignment [" + ticket.TicketNumber.ToString() + "]", message, emailReplyToAddress, fileNames.ToArray(), ticket: ticket, recipientList: list);
				}

			}
			catch (Exception ex)
			{
				Logs.WriteEvent("Error with AddMessageTicketAssignment");
				Logs.WriteException(ex);
				ExceptionLogs.LogException(LoginUser, ex, "AddMessageTicketAssignment", ticket.Row);
			}


		}

        private void AddMessageInternalTicketModified(Ticket ticket, int? oldUserID, int? oldGroupID, bool isNew, int? oldTicketStatusID, int? oldTicketSeverityID, bool includeActions, string modifierName, int modifierID, Organization ticketOrganization)
		{
			try
			{
				if (oldTicketSeverityID == null && oldTicketStatusID == null && !includeActions) return;
				List<UserEmail> userList = new List<UserEmail>();
				AddTicketOwners(userList, ticket);
				AddTicketSubscribers(userList, ticket);
				List<string> fileNames = new List<string>();

				if (oldGroupID != null) // group assignment already sent
				{
					List<UserEmail> removeList = new List<UserEmail>();
					AddTicketOwners(removeList, ticket, true);
					foreach (UserEmail item in removeList)
					{
						if (ticket.UserID != null && ticket.UserID == item.UserID)
						{
							// We're only removing the group users.
							// The ticket user persist.
							// It will be removed if necessary, below.
						}
						else
						{
							RemoveUser(userList, item.UserID);
						}
					}
				}

				if (oldUserID != null && ticket.UserID != null) RemoveUser(userList, (int)ticket.UserID);
				if (userList.Count < 1) return;

				StringBuilder builder = new StringBuilder();
				if (oldTicketStatusID != null) builder.Append(string.Format("<div>The status changed from {0} to {1}.</div>", TicketStatuses.GetTicketStatus(LoginUser, (int)oldTicketStatusID).Name, TicketStatuses.GetTicketStatus(LoginUser, (int)ticket.TicketStatusID).Name));
				if (oldTicketSeverityID != null) builder.Append(string.Format("<div>The severity changed from {0} to {1}.</div>", TicketSeverities.GetTicketSeverity(LoginUser, (int)oldTicketSeverityID).Name, TicketSeverities.GetTicketSeverity(LoginUser, (int)ticket.TicketSeverityID).Name));

				MailMessage message = EmailTemplates.GetTicketUpdateUser(LoginUser, modifierName, ticket.GetTicketView(), builder.ToString(), includeActions);

				if (includeActions)
				{
					Actions actions = new Actions(LoginUser);
					actions.LoadLatestByTicket(ticket.TicketID, false);

                    if (CanIncludeAttachments(ticket, actions, ticketOrganization))
                        TeamSupport.Data.Quarantine.ServiceQ.AddAttachmentFileNames3(fileNames, actions);

                    foreach (Data.Action item in actions)
					{
						if (!item.IsVisibleOnPortal)
						{
							message.Subject = message.Subject + " [pvt]";
							break;
						}
					}
				}
				AddUsersToAddresses(message.To, userList, modifierID);
				foreach (MailAddress mailAddress in message.To)
				{
					ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, "Ticket modified email sent to " + mailAddress.Address);
				}

				string emailReplyToAddress = GetEmailReplyToAddress(LoginUser, ticket);
				AddMessage(ticketOrganization.OrganizationID, "Internal Ticket Modified [" + ticket.TicketNumber.ToString() + "]", message, emailReplyToAddress, fileNames.ToArray(), ticket: ticket, recipientList: userList);
			}
			catch (Exception ex)
			{
				Logs.WriteEvent("Error with AddMessageInternalTicketModified");
				Logs.WriteException(ex);
				ExceptionLogs.LogException(LoginUser, ex, "AddMessageInternalTicketModified", ticket.Row);
			}
		}

        private bool IsModifierTSUser(int modifierID)
		{
			bool result = false;
			User modifier = Users.GetUser(LoginUser, modifierID);
			if (modifier != null)
			{
				Organization organization = Organizations.GetOrganization(LoginUser, modifier.OrganizationID);
				result = organization != null && organization.ParentID == 1;
			}

			return result;
		}

		private void AddMessagePortalTicketModified(Ticket ticket,
													bool isNew,
													int? oldTicketStatusID,
													bool includeActions,
													int[] users,
													string modifierName,
													int modifierID,
													string modifierTitle,
													Organization ticketOrganization,
													bool isBasic)
		{
			try
			{
				if (!ticket.IsVisibleOnPortal)
				{
					Logs.WriteEvent("Ticket not visible on portal, returning.");
					return;
				}

				if (oldTicketStatusID == null && !includeActions && users.Length < 1)
				{
					Logs.WriteEvent("Old status id is null AND there are no actions AND no users specified, returning.");
					return;
				}

				//If is IsClosedEmail and modifier is Portal user then do not exclude on email
				//vv: change for ticket 25530. Not QA approved. bool removeModifier = true;
				List<UserEmail> userList;
				List<UserEmail> advUsers = new List<UserEmail>();
				AddAdvancedPortalUsers(advUsers, ticket);

				if (isBasic)
				{
					Logs.WriteEvent("Processing basic portal code");
					userList = new List<UserEmail>();
					AddBasicPortalUsers(userList, ticket);
					Logs.WriteEvent("Adding Basic Portal Users");

					foreach (UserEmail basicUserLog in userList)
					{
						Logs.WriteEvent("  + " + basicUserLog.ToString());
					}

					if (!string.IsNullOrEmpty(ticket.PortalEmail))
					{
						Logs.WriteEvent(string.Format("Ticket.PortalEmail: {0}", ticket.PortalEmail));
						UserEmail email = FindByUserEmail(ticket.PortalEmail, userList);

						if (email == null)
						{
							Logs.WriteEvent("Adding portal email to list");
							userList.Add(new UserEmail(-1, "", "", ticket.PortalEmail, false));
						}
					}
					else
					{
						Logs.WriteEvent("Ticket.PortalEmail: None");
					}

					foreach (UserEmail item in advUsers)
					{
						UserEmail email = FindByUserEmail(item.Address, userList);

						if (email != null)
						{
							userList.Remove(email);
							Logs.WriteEvent(string.Format("Removing Advance Portal User Email From List: {0}", email));
						}
					}
				}
				else
				{
					userList = advUsers;
					Logs.WriteEvent("Adding Advanced Portal Users");

					foreach (UserEmail advuserlog in advUsers)
					{
						Logs.WriteEvent("  + " + advuserlog.ToString());
					}
				}

				if (oldTicketStatusID == null && !includeActions)
				{
					Logs.WriteEvent("Old Status ID is NULL AND no actions included, making new list from EmailPost.Param8 (Users)");
					List<UserEmail> newList = new List<UserEmail>();

					foreach (UserEmail email in userList)
					{
						if (users.Contains(email.UserID))
						{
							newList.Add(email);
							Logs.WriteEvent(string.Format("Added Email: {0}", email));
						}
					}

					userList = newList;
				}

				TicketStatus status = TicketStatuses.GetTicketStatus(LoginUser, ticket.TicketStatusID);
				//vv: change for ticket 25530. Not QA approved. Following lines commented out and re-added the original ones
				//User modifierUser = AddPortalModifierIfClosing(userList, ticket, isBasic, status);
				//removeModifier = modifierUser == null;

				//if (removeModifier)
				//{
				//    RemoveUser(userList, modifierID);
				//    Logs.WriteEvent(string.Format("Removing Modifier user from list: {0}", modifierID.ToString()));
				//}

				//vv re-added
				RemoveUser(userList, modifierID);
				Logs.WriteEvent(string.Format("Removing Modifier user from list: {0}", modifierID.ToString()));


				if (userList.Count < 1)
				{
					Logs.WriteEvent("User list has count of 0, returning");
					return;
				}

				List<string> fileNames = new List<string>();

				if (includeActions)
				{
					Actions actions = new Actions(LoginUser);
					actions.LoadLatestByTicket(ticket.TicketID, true);

                    if (CanIncludeAttachments(ticket, actions, ticketOrganization))
                        TeamSupport.Data.Quarantine.ServiceQ.AddAttachmentFileNames1(fileNames, actions, Logs);
                }

				MailMessage message = null;
				string messageType;

				if (status.IsClosedEmail)
				{
					Logs.WriteEvent("Status is closed email, getting template");
					messageType = "Ticket closed email";
					message = EmailTemplates.GetTicketClosed(LoginUser, modifierName, modifierTitle, ticket.GetTicketView(), includeActions);
				}
				else
				{
					Logs.WriteEvent("Getting portal email template");
					message = isBasic ? EmailTemplates.GetTicketUpdateBasicPortal(LoginUser, modifierName, modifierTitle, ticket.GetTicketView(), includeActions) :
										EmailTemplates.GetTicketUpdateAdvPortal(LoginUser, modifierName, modifierTitle, ticket.GetTicketView(), includeActions);
					messageType = isBasic ? "Basic portal email" : "Advanced portal email";
				}

				if (string.IsNullOrWhiteSpace(message.Body))
				{
					Logs.WriteEvent("Exiting portal email because it was blank.");
					return;
				}

				if (ticketOrganization.AddEmailViaTS && !string.IsNullOrWhiteSpace(message.Body))
				{
					message.Body = message.Body + GetViaTSHtmlAd(ticketOrganization.Name);
				}

				string subject = message.Subject;
				string body = message.Body;

				foreach (UserEmail userEmail in userList)
				{
					Logs.WriteEvent("Adding address to email message.");

					try
					{
						//Exclude the creator if this is a new ticket emailpost.
						bool excludeCreator = false;
						excludeCreator = isNew && userEmail.UserID == ticket.CreatorID;

						if (excludeCreator)
						{
							Logs.WriteEvent("Excluding creator off the modified ticket email when it is New.");
						}

						//vv: change for ticket 25530. Not QA approved. if (userEmail != null && (modifierID != userEmail.UserID || (modifierID == userEmail.UserID && !removeModifier)) && !excludeCreator)
						if (userEmail != null && modifierID != userEmail.UserID && !excludeCreator)
						{
							message.Body = body;
							message.Subject = subject;
							message.To.Clear();
							MailAddress mailAddress = GetMailAddress(userEmail.Address, userEmail.Name);
							message.To.Add(mailAddress);
							message.HeadersEncoding = Encoding.UTF8;
							ContactsViewItem contact = ContactsView.GetContactsViewItem(_loginUser, userEmail.UserID);
							EmailTemplate.ReplaceMessageFields(_loginUser, "Recipient", contact, message, -1, ticket.OrganizationID);

							if (ticketOrganization.AgentRating == true)
							{
								AgentRatingsOptions options = new AgentRatingsOptions(_loginUser);
								options.LoadByOrganizationID(ticket.OrganizationID);
								string baseUrl = SystemSettings.GetAppUrl();
								string externalUrl = SystemSettings.GetPortalUrl() + "/rating.aspx";
								string positiveUrl = baseUrl + "/vcr/1_9_0/images/face-positive.png";
								string neutralUrl = baseUrl + "/vcr/1_9_0/images/face-neutral.png";
								string negativeUrl = baseUrl + "/vcr/1_9_0/images/face-negative.png";

								if (!options.IsEmpty)
								{
									AgentRatingsOption option = options[0];
									if (!string.IsNullOrWhiteSpace(option.ExternalPageLink)) externalUrl = option.ExternalPageLink;
									if (!string.IsNullOrWhiteSpace(option.PositiveImage)) positiveUrl = baseUrl + option.PositiveImage;
									if (!string.IsNullOrWhiteSpace(option.NeutralImage)) neutralUrl = baseUrl + option.NeutralImage;
									if (!string.IsNullOrWhiteSpace(option.NegativeImage)) negativeUrl = baseUrl + option.NegativeImage;
								}

								StringBuilder ratingLink = new StringBuilder();
								ratingLink.Append("<a href=\"");
								ratingLink.Append(externalUrl);
								ratingLink.Append(externalUrl.IndexOf("?") > -1 ? "&" : "?");
								ratingLink.Append("OrganizationID=" + ticket.OrganizationID.ToString());
								ratingLink.Append("&TicketID=" + ticket.TicketID.ToString());
								ratingLink.Append("&Rating={0}");
								ratingLink.Append("&CustomerID=" + (contact == null ? "-1" : contact.UserID.ToString()));
								ratingLink.Append("\"><img src=\"{1}\" width=\"50\" height=\"50\" title=\"{2}\" alt=\"{2}\" /></a>");
								string link = ratingLink.ToString();
								EmailTemplate.ReplaceMessageParameter(_loginUser, "AgentRatingsImageLink.Positive", string.Format(link, "1", positiveUrl, "Positive"), message, -1, ticket.OrganizationID);
								EmailTemplate.ReplaceMessageParameter(_loginUser, "AgentRatingsImageLink.Neutral", string.Format(link, "0", neutralUrl, "Neutral"), message, -1, ticket.OrganizationID);
								EmailTemplate.ReplaceMessageParameter(_loginUser, "AgentRatingsImageLink.Negative", string.Format(link, "-1", negativeUrl, "Negative"), message, -1, ticket.OrganizationID);
							}
							else
							{
								EmailTemplate.ReplaceMessageParameter(_loginUser, "AgentRatingsImageLink.Positive", "", message, -1, ticket.OrganizationID);
								EmailTemplate.ReplaceMessageParameter(_loginUser, "AgentRatingsImageLink.Neutral", "", message, -1, ticket.OrganizationID);
								EmailTemplate.ReplaceMessageParameter(_loginUser, "AgentRatingsImageLink.Negative", "", message, -1, ticket.OrganizationID);
							}

							Logs.WriteEvent(string.Format("Email: {0} <{1}>", userEmail.Name, userEmail.Address));

							string emailReplyToAddress = GetEmailReplyToAddress(LoginUser, ticket);
							AddMessage(ticketOrganization.OrganizationID, "Portal Ticket Modified [" + ticket.TicketNumber.ToString() + "]", message, emailReplyToAddress, fileNames.ToArray());

							Logs.WriteEvent("Adding action log to ticket");
							ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, messageType + " sent to " + mailAddress.Address);
						}
					}
					catch (Exception ex)
					{
						Logs.WriteException(ex);
						ExceptionLogs.LogException(LoginUser, ex, "AddMessagePortalTicketModified", ticket.Row);
					}
				}
			}
			catch (Exception ex)
			{
				Logs.WriteEvent("Error with AddMessagePortalTicketModified");
				Logs.WriteException(ex);
				ExceptionLogs.LogException(LoginUser, ex, "AddMessagePortalTicketModified", ticket.Row);
			}
		}

        private void ProcessTicketUpdateRequest(int ticketID, int modifierID)
		{
			Ticket ticket = Tickets.GetTicket(LoginUser, ticketID);
			try
			{

				User modifier = Users.GetUser(LoginUser, modifierID);
				if (ticket == null)
				{
					Logs.WriteEvent("Unable to find Ticket, TicketID: " + ticketID.ToString());
					return;
				}

				if (modifier == null)
				{
					Logs.WriteEvent("Unable to find Modifying User, UserID: " + modifierID.ToString());
					return;
				}

				if (ticket.UserID == null)
				{
					Logs.WriteEvent("Assigned Ticket User is null");
					return;
				}

				Organization ticketOrganization = Organizations.GetOrganization(LoginUser, ticket.OrganizationID);

				if (ticketOrganization == null)
				{
					Logs.WriteEvent("Ticket's Organization IS NULL!!!!");
					return;
				}

				User owner = Users.GetUser(LoginUser, (int)ticket.UserID);

				MailMessage message = EmailTemplates.GetTicketUpdateRequest(LoginUser, UsersView.GetUsersViewItem(LoginUser, modifierID), ticket.GetTicketView(), true);
				message.To.Add(GetMailAddress(owner.Email, owner.FirstLastName));
				message.Subject = message.Subject + " [pvt]";
				EmailTemplates.ReplaceEmailRecipientParameters(LoginUser, message, ticket, owner.UserID, owner.OnlyEmailAfterHours);

				foreach (MailAddress mailAddress in message.To)
				{
					ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, "Ticket update request email sent to " + mailAddress.Address);
				}

				string emailReplyToAddress = GetEmailReplyToAddress(LoginUser, ticket);
				AddMessage(ticketOrganization.OrganizationID, "Ticket Update Request [" + ticket.TicketNumber.ToString() + "]", message, emailReplyToAddress);
			}
			catch (Exception ex)
			{
				Logs.WriteEvent("Error with ProcessTicketUpdateRequest");
				Logs.WriteException(ex);
				ExceptionLogs.LogException(LoginUser, ex, "ProcessTicketUpdateRequest", ticket.Row);
			}
		}

		private void ProcessReaction(int creatorID, int receiverID, int ticketID, string hostName)
		{
			try
			{
                if (Emails.IsEmailDisabled(LoginUser, receiverID, "Applause")) return;

                User receiver = Users.GetUser(LoginUser, receiverID);
                User sender = Users.GetUser(LoginUser, creatorID);
				Ticket ticket = Tickets.GetTicket(LoginUser, ticketID);
				MailMessage message = EmailTemplates.GetReaction(LoginUser, sender, receiver, ticket.GetTicketView(), hostName);

				message.To.Add(GetMailAddress(receiver.Email, receiver.FirstLastName));
				// message.Subject = message.Subject;
				string replyAddress = sender.Email;
				AddMessage(receiver.OrganizationID, "Action Reaction (Applause) [" + ticketID.ToString() + "]", message, replyAddress);
			}

			catch (Exception ex)
			{
				Logs.WriteEvent("Error with ProcessReaction");
				Logs.WriteException(ex);
				ExceptionLogs.LogException(LoginUser, ex, "ProcessReaction");
			}
		}

		private void ProcessTicketSendEmail(int userID, int ticketID, string addresses, string introduction, int actionID)
        {
            Ticket ticket = Tickets.GetTicket(LoginUser, ticketID);
            User sender = Users.GetUser(LoginUser, userID);

            if (ticket == null || sender == null || ticket.OrganizationID != sender.OrganizationID) return;
            LoginUser loginUser = new LoginUser(LoginUser.ConnectionString, sender.UserID, sender.OrganizationID, null);

            Actions actions = new Actions(LoginUser);
            actions.LoadByTicketID(ticket.TicketID);

            List<string> fileNames = TeamSupport.Data.Quarantine.ServiceQ.GetAtachmentFileNames(LoginUser, actionID, actions, Logs);

            Organizations organization = new Organizations(LoginUser);
            organization.LoadByOrganizationID(ticket.OrganizationID);
            bool includeAttachments = true;

            if (organization != null && organization.Count > 0)
            {
                includeAttachments = CanIncludeAttachments(ticket, actions, organization[0]);
            }

            foreach (Data.Action action in actions)
            {
                if (!action.IsVisibleOnPortal) continue;

                if (includeAttachments)
                    TeamSupport.Data.Quarantine.ServiceQ.AddFileNames(actions, fileNames, action, Logs);
            }

            char split = ';';
            if (addresses.IndexOf(';') < 0) split = ',';

            string[] list = addresses.Split(split);

            foreach (string item in list)
            {
                try
                {
                    MailMessage message = EmailTemplates.GetTicketSendEmail(LoginUser, sender.GetUserView(), ticket.GetTicketView(), item.Trim(), introduction);
                    message.To.Add(GetMailAddress(item.Trim()));
                    message.Subject = message.Subject;

                    foreach (MailAddress mailAddress in message.To)
                    {
                        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, "Ticket email sent to " + mailAddress.Address);
                    }

                    string emailReplyToAddress = GetEmailReplyToAddress(LoginUser, ticket);
                    AddMessage(sender.OrganizationID, "Ticket Email [" + ticket.TicketNumber.ToString() + "] to " + item, message, emailReplyToAddress, fileNames.ToArray());
                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(loginUser, ex, "Email Processor", "Email Ticket");
                }
            }
        }

        private void AddMessageNewTicket(Ticket ticket, User modifier, Organization ticketOrganization)
		{
			if (string.IsNullOrEmpty(ticket.TicketSource))
			{
				Logs.WriteEvent("No Ticket Source");
				return;
			}

			string source = ticket.TicketSource.ToLower().Trim();
			if (source != "web" && source != "email" && source != "chatoffline")
			{
				Logs.WriteEvent("Source not (web, email, chatoffline)");
				return;
			}


			if (!ticket.IsVisibleOnPortal || ticket.GetTicketView().IsClosed)
			{
				Logs.WriteEvent("Not visible on portal or closed");
				return;
			}


			if (modifier == null && string.IsNullOrEmpty(ticket.PortalEmail))
			{
				Logs.WriteEvent("No modifier or portal email to send");
				return;
			}

			if (modifier != null)
			{
				Organization modifierOrg = Organizations.GetOrganization(_loginUser, modifier.OrganizationID);
				if (!modifierOrg.IsActive)
				{
					Logs.WriteEvent("Modifier Org is inactive");
					return;
				}

			}

			MailAddress modifierAddress = (modifier == null) ? GetMailAddress(ticket.PortalEmail) : GetMailAddress(modifier.Email, modifier.FirstLastName);

			string emailReplyToAddress = GetEmailReplyToAddress(LoginUser, ticket);

			if (modifier != null && modifier.OrganizationID == ticketOrganization.OrganizationID) // internal
			{
				MailMessage message = EmailTemplates.GetNewTicketInternal(LoginUser, modifier.GetUserView(), ticket.GetTicketView());
				message.To.Add(modifierAddress);
				foreach (MailAddress mailAddress in message.To)
				{
					ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, "New ticket email sent to " + mailAddress.Address);
				}

				AddMessage(ticket.OrganizationID, "Internal New Ticket [" + ticket.TicketNumber.ToString() + "]", message, emailReplyToAddress);
			}
			else // portal
			{
				MailMessage message = modifier != null && modifier.IsPortalUser ? EmailTemplates.GetNewTicketAdvPortal(LoginUser, modifier.GetUserView(), ticket.GetTicketView()) :
																				  EmailTemplates.GetNewTicketBasicPortal(LoginUser, modifierAddress, ticket.GetTicketView());
				message.To.Add(modifierAddress);
				foreach (MailAddress mailAddress in message.To)
				{
					ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, "New ticket portal email sent to " + mailAddress.Address);
				}

				AddMessage(ticketOrganization.OrganizationID, "New Ticket [" + ticket.TicketNumber.ToString() + "]", message, emailReplyToAddress);
			}
		}

		#endregion

		public void ProcessSignUpNotification(int userID, string url, string referrer)
		{
			User user = Users.GetUser(LoginUser, userID);
			Organization organization = Organizations.GetOrganization(LoginUser, user.OrganizationID);
			MailMessage message = EmailTemplates.GetSignUpNotification(LoginUser, user, url, referrer);
			message.From = GetMailAddress("sales@teamsupport.com", "TeamSupport.com");

			string[] addresses = SystemSettings.ReadString(LoginUser, "SignUpNotifications", "").Split('|');
			if (addresses != null && addresses.Length < 1) return;
			foreach (string address in addresses)
			{
				message.To.Add(GetMailAddress(address));
			}

			AddMessage(1078, "New Internal Sign Up", message);
		}

		public void ProcessWelcomeNewSignup(int userID, string password)
		{/*
            User user = Users.GetUser(LoginUser, userID);
            Organization organization = Organizations.GetOrganization(LoginUser, user.OrganizationID);

            string from = "sales@teamsupport.com";
            MailMessage message = EmailTemplates.GetWelcomeNewSignUp(LoginUser, user.GetUserView(), password, DateTime.Now.AddDays(14).ToString("MMMM d, yyyy"), 10);
            message.To.Add(GetMailAddress(user.Email));
            message.Bcc.Add(GetMailAddress("dropbox@79604342.murocsystems.highrisehq.com"));
            message.From = GetMailAddress(from);
            AddMessage(1078, "New Sign Up - Welcome [" + organization.Name + "]", message);
            /*
            message = EmailTemplates.GetWelcomeNewSignUp(LoginUser, user.GetUserView(), password, DateTime.Now.AddDays(14).ToString("MMMM d, yyyy"), 27);
            message.To.Add(new MailAddress(user.Email));
            message.Bcc.Add(new MailAddress("eharrington@teamsupport.com"));
            message.From = new MailAddress("eharrington@teamsupport.com", "Eric Harrington");
            AddMessage(1078, "New Sign Up - Check In[" + organization.Name + "]", message, null, null, DateTime.UtcNow.AddDays(1));
            */
		 /*
		 message = EmailTemplates.GetWelcomeNewSignUp(LoginUser, user.GetUserView(), password, DateTime.Now.AddDays(14).ToString("MMMM d, yyyy"), 28);
		 message.To.Add(GetMailAddress(user.Email));
		 message.Bcc.Add(GetMailAddress("dropbox@79604342.murocsystems.highrisehq.com"));
		 //message.Bcc.Add(new MailAddress("eharrington@teamsupport.com"));
		 message.From = GetMailAddress(from);
		 AddMessage(1078, "New Sign Up - Notice [" + organization.Name + "]", message, null, null, DateTime.UtcNow.AddDays(10));
			*/
		}

		public void ProcessWelcomeTSUser(int userID, string password)
		{
			User user = Users.GetUser(LoginUser, userID);
			MailMessage message = EmailTemplates.GetWelcomeTSUser(LoginUser, user.GetUserView(), password);
			message.To.Add(GetMailAddress(user.Email, user.FirstLastName));
			AddMessage(user.OrganizationID, "Welcome New User [" + user.FirstLastName + "]", message);
		}

		public void ProcessWelcomePortalUser(int userID, string password)
		{
			User user = Users.GetUser(LoginUser, userID);
			Organization organization = (Organization)Organizations.GetOrganization(LoginUser, (int)Organizations.GetOrganization(LoginUser, user.OrganizationID).ParentID);
			MailMessage message = EmailTemplates.GetWelcomePortalUser(LoginUser, user.GetUserView(), password);
			message.To.Add(GetMailAddress(user.Email, user.FirstLastName));
			AddMessage(organization.OrganizationID, "Welcome New Portal User [" + user.FirstLastName + "]", message);
		}


		public void ProcessNewDevice(int userID)
		{
			User user = Users.GetUser(LoginUser, userID);

			MailMessage message = EmailTemplates.GetNewDeviceEmail(LoginUser, user.GetUserView());
			message.To.Add(GetMailAddress(user.Email, user.FirstLastName));
			message.From = GetMailAddress("support@teamsupport.com", "TeamSupport.com");
			AddMessage(user.OrganizationID, "New Device Login [" + user.FirstLastName + "]", message);
		}

		public void ProcessTooManyAttempts(int userID)
		{
			User user = Users.GetUser(LoginUser, userID);
			Users users = new Users(LoginUser);

			users.LoadByOrganizationID(user.OrganizationID, true);

			MailMessage message = EmailTemplates.GetTooManyAttempts(LoginUser, user.GetUserView());
			message.To.Add(GetMailAddress(user.Email, user.FirstLastName));

			foreach (User admin in users)
			{
				if (admin.IsSystemAdmin) message.To.Add(GetMailAddress(admin.Email, admin.FirstLastName));
			}


			message.From = GetMailAddress("support@teamsupport.com", "TeamSupport.com");
			AddMessage(user.OrganizationID, "Too Many Login Attempts [" + user.FirstLastName + "]", message);
		}
		public void ProcessResetTSPassword(int userID, string password)
		{
			User user = Users.GetUser(LoginUser, userID);

			MailMessage message = EmailTemplates.GetResetPasswordTS(LoginUser, user.GetUserView(), password);
			message.To.Add(GetMailAddress(user.Email, user.FirstLastName));
			message.From = GetMailAddress("support@teamsupport.com", "TeamSupport.com");
			AddMessage(user.OrganizationID, "Reset TS Password [" + user.FirstLastName + "]", message);
		}

		public void ProcessResetPortalPassword(int userID, string password)
		{
			User user = (User)Users.GetUser(LoginUser, userID);
			Organization organization = (Organization)Organizations.GetOrganization(LoginUser, (int)Organizations.GetOrganization(LoginUser, user.OrganizationID).ParentID);
			MailMessage message = EmailTemplates.GetResetPasswordPortal(LoginUser, user.GetUserView(), password);
			message.To.Add(GetMailAddress(user.Email, user.FirstLastName));
			AddMessage(organization.OrganizationID, "Reset Portal Password [" + user.FirstLastName + "]", message);
		}

		public void ProcessChangedPortalPassword(int userID)
		{
			User user = (User)Users.GetUser(LoginUser, userID);
			Organization organization = (Organization)Organizations.GetOrganization(LoginUser, (int)Organizations.GetOrganization(LoginUser, user.OrganizationID).ParentID);
			MailMessage message = EmailTemplates.GetChangedPasswordPortal(LoginUser, user.GetUserView());
			message.To.Add(GetMailAddress(user.Email, user.FirstLastName));
			AddMessage(organization.OrganizationID, "Portal Password Changed [" + user.FirstLastName + "]", message);
		}
		public void ProcessResetHubPassword(int userID, string password, int productFamilyID)
		{
			User user = (User)Users.GetUser(LoginUser, userID);
			Organization organization = (Organization)Organizations.GetOrganization(LoginUser, (int)Organizations.GetOrganization(LoginUser, user.OrganizationID).ParentID);
			CustomerHubs hubs = new CustomerHubs(LoginUser);

			//first try by product line
			if (productFamilyID != -1)
			{
				hubs.LoadByProductFamilyID(productFamilyID);
			}
			if (hubs.IsEmpty)
			{
				hubs.LoadByOrganizationID(organization.OrganizationID);
			}

			if (hubs.IsEmpty) throw new Exception("No customer hub found for user: " + userID.ToString());
			MailMessage message = EmailTemplates.GetResetPasswordHub(LoginUser, user.GetUserView(), hubs[0], password, productFamilyID);
			message.To.Add(GetMailAddress(user.Email, user.FirstLastName));
			AddMessage(organization.OrganizationID, "Reset Portal Password [" + user.FirstLastName + "]", message);
		}

		public void ProcessWelcomeCustomerHubUser(int userID, string password, int productFamilyID)
		{
			User user = (User)Users.GetUser(LoginUser, userID);
			Organization organization = (Organization)Organizations.GetOrganization(LoginUser, (int)Organizations.GetOrganization(LoginUser, user.OrganizationID).ParentID);
			CustomerHubs hubs = new CustomerHubs(LoginUser);

			if (productFamilyID != -1)
			{
				hubs.LoadByProductFamilyID(productFamilyID);
			}
			if (hubs.IsEmpty)
			{
				hubs.LoadByOrganizationID(organization.OrganizationID);
			}
			if (hubs.IsEmpty) throw new Exception("No customer hub found for user: " + userID.ToString());

			MailMessage message = EmailTemplates.GetWelcomeCustomerHub(LoginUser, user.GetUserView(), hubs[0], password, productFamilyID);
			message.To.Add(GetMailAddress(user.Email, user.FirstLastName));
			AddMessage(organization.OrganizationID, "Portal Password Changed [" + user.FirstLastName + "]", message);
		}

		public void ProcessChangedTSPassword(int userID)
		{
			User user = (User)Users.GetUser(LoginUser, userID);
			MailMessage message = EmailTemplates.GetChangedPasswordTS(LoginUser, user.GetUserView());
			message.To.Add(GetMailAddress(user.Email, user.FirstLastName));
			message.From = GetMailAddress("support@teamsupport.com", "TeamSupport.com");
			AddMessage(user.OrganizationID, "TS Password Changed [" + user.FirstLastName + "]", message);
		}

		private bool CanIncludeAttachments(Ticket ticket, Actions actions, Organization ticketOrganization)
		{
			bool include = false;

			if (!actions.IsEmpty && !ticketOrganization.NoAttachmentsInOutboundEmail)
			{
				include = true;
			}
			else if (!actions.IsEmpty && ticketOrganization.NoAttachmentsInOutboundEmail && !string.IsNullOrEmpty(ticketOrganization.NoAttachmentsInOutboundExcludeProductLine) && ticket.ProductID != null)
			{
				Product product = Products.GetProduct(LoginUser, (int)ticket.ProductID);

				if (product != null && product.ProductFamilyID != null)
				{
					List<string> except = ticketOrganization.NoAttachmentsInOutboundExcludeProductLine.Split(',').ToList();
					include = except.Contains(product.ProductFamilyID.ToString());
				}
			}

			return include;
		}

		#region Utility Methods

		private int GetIntParam(string param)
		{
			if (string.IsNullOrEmpty(param)) return -1;
			try
			{
				return int.Parse(param);
			}
			catch (Exception)
			{
				return -1;
			}
		}

		private int? GetNullIntParam(string param)
		{
			if (string.IsNullOrEmpty(param)) return null;
			try
			{
				return int.Parse(param);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private bool AddUser(List<UserEmail> list, User user)
		{
			return AddUser(list, user, false);
		}

		private bool AddUser(List<UserEmail> list, User user, bool honorTicketNotifications)
		{
			if (user == null || user.Email == null) return false;
			if ((IsUserAlreadyInList(list, user.UserID) || (!user.ReceiveTicketNotifications && honorTicketNotifications))) return false;
			list.Add(new UserEmail(user.UserID, user.FirstName, user.LastName, user.Email, user.OnlyEmailAfterHours));
			return true;
		}

		private void RemoveUser(List<UserEmail> list, int userID)
		{
			UserEmail email = null;
			foreach (UserEmail item in list)
			{
				if (item.UserID == userID)
				{
					email = item;
					break;
				}
			}

			if (email != null) list.Remove(email);
		}

		private void RemoveBusinessHoursUsers(List<UserEmail> list, Ticket ticket)
		{
			Logs.WriteEvent("Removing Bussiness Hours User");
			Organization organization = Organizations.GetOrganization(LoginUser, ticket.OrganizationID);

			Logs.WriteParam("Ticket Organization", ticket.OrganizationID.ToString());
			Logs.WriteParam("Ticket DateCreated UTC", ticket.DateCreatedUtc.ToString());
			Logs.WriteParam("Business Day End", organization.BusinessDayEndUtc.ToString());
			Logs.WriteParam("Business Day Start", organization.BusinessDayStartUtc.ToString());
			Logs.WriteParam("Business Days", organization.BusinessDaysText);
			Logs.WriteParam("Now", DateTime.UtcNow.ToString());
			if (!organization.IsInBusinessHours(ticket.DateCreatedUtc))
			{
				Logs.WriteEvent("Ticket not created in business hours, returning");
				return;
			}
			List<UserEmail> removals = new List<UserEmail>();
			foreach (UserEmail item in list)
			{
				if (item.SendTicketsOnlyAfterHours && (ticket.UserID == null || item.UserID != ticket.UserID))
				{
					ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, "Email not sent to " + item.Name + " during business hours");
					removals.Add(item);
					Logs.WriteEventFormat("Removing Business Hours User: {0} ({1}) <{2}> -> (item.SentTicketsOnlyAfterHours: {3} AND (ticket.UserID == null OR item.UserID != ticket.UserID))", item.Name, item.UserID.ToString(),
					  item.Address, item.SendTicketsOnlyAfterHours.ToString());
				}
			}

			foreach (UserEmail item in removals)
			{
				list.Remove(item);
			}
		}

		private bool IsUserAlreadyInList(List<UserEmail> list, int userID)
		{
			foreach (UserEmail item in list)
			{
				if (item.UserID == userID)
				{
					return true;
				}
			}
			return false;
		}

		private void AddTicketOwners(List<UserEmail> userList, Ticket ticket, bool? receiveTicketNotifications = null)
		{
			Logs.WriteEvent("Adding Ticket Owners");
			//It has been identified that unassigned tickets are passing the ticket.UserID != null condition.
			//Therefore we'll check against the user object instead. To do so, we are renaming and moving this variable declaration outside the condition check.
			User ticketAssignedUser = null;
			if (ticket.UserID != null)
			{
				ticketAssignedUser = Users.GetUser(LoginUser, (int)ticket.UserID);
				if (ticketAssignedUser != null)
				{
					if (receiveTicketNotifications != null)
					{
						if (ticketAssignedUser.ReceiveTicketNotifications == (bool)receiveTicketNotifications)
						{
							if (AddUser(userList, ticketAssignedUser, true))
							{
								Logs.WriteEventFormat("Added Assigned User: {0} ({1})", ticketAssignedUser.DisplayName, ticketAssignedUser.UserID.ToString());
							}
							else
							{
								Logs.WriteEventFormat("Didn't add Assigned User: {0} ({1})", ticketAssignedUser.DisplayName, ticketAssignedUser.UserID.ToString());
							}
						}
						Logs.WriteEventFormat("Owner Skipped due to receiveTicketNotifications: {0} ({1})", ticketAssignedUser.DisplayName, ticketAssignedUser.UserID.ToString());
					}
					else
					{
						if (AddUser(userList, ticketAssignedUser, true))
						{
							Logs.WriteEventFormat("Added Assigned User: {0} ({1})", ticketAssignedUser.DisplayName, ticketAssignedUser.UserID.ToString());
						}
						else
						{
							Logs.WriteEventFormat("Didn't add Assigned User: {0} ({1})", ticketAssignedUser.DisplayName, ticketAssignedUser.UserID.ToString());
						}
					}
				}
			}


			if (ticket.GroupID != null)
			{
				Logs.WriteEvent("Ticket.GroupID is NOT NULL");
				Logs.WriteEvent("Loading ticket group users");
				Users users = new Users(LoginUser);
				users.LoadByGroupID((int)ticket.GroupID);

				foreach (User user in users)
				{
					if (ticket.UserHasRights(user) && ((ticketAssignedUser != null && user.ReceiveAllGroupNotifications) || (ticketAssignedUser == null && user.ReceiveUnassignedGroupEmails)))
					{
						if (AddUser(userList, user, true))
						{
							Logs.WriteEventFormat("{0} ({1}) <{2}> was added to the list", user.DisplayName, user.UserID.ToString(), user.Email);
						}
						else
						{
							Logs.WriteEventFormat("{0} ({1}) <{2}> was not added to the list", user.DisplayName, user.UserID.ToString(), user.Email);
						}
					}
					else
					{
						Logs.WriteEventFormat("{0} ({1}) <{2}> was DENIED to the list. || Ticket.UserHasRights = {3} AND (ReceiveAllGroupNotifications = {4} OR ticket.UserID is NULL)", user.DisplayName, user.UserID.ToString(), user.Email, ticket.UserHasRights(user).ToString(), user.ReceiveAllGroupNotifications.ToString(), (ticket.UserID == null).ToString());
					}
				}

				if (ticket.UserID != null)
				{
					RemoveBusinessHoursUsers(userList, ticket);
				}
			}
		}

		private void AddTicketSubscribers(List<UserEmail> userList, Ticket ticket)
		{
			Users users;

			// Ticket Subscribers
			users = new Users(LoginUser);
			users.LoadByTicketSubscription(ticket.TicketID);
			foreach (User user in users)
			{
				if (ticket.UserHasRights(user))
				{
					if (AddUser(userList, user, true))
					{
						Logs.WriteEventFormat("{0} ({1}) <{2}> ticket subscriber was added to the list", user.DisplayName, user.UserID.ToString(), user.Email);
					}
					else
					{
						Logs.WriteEventFormat("{0} ({1}) <{2}> ticket subscriber was not added to the list", user.DisplayName, user.UserID.ToString(), user.Email);
					}
				}
			}

			// Customer Subscribers
			users = new Users(LoginUser);
			users.LoadByCustomerSubscription(ticket.TicketID);
			foreach (User user in users)
			{
				if (ticket.UserHasRights(user))
				{
					if (AddUser(userList, user))
					{
						Logs.WriteEventFormat("{0} ({1}) <{2}> customer subscriber was added to the list", user.DisplayName, user.UserID.ToString(), user.Email);
					}
					else
					{
						Logs.WriteEventFormat("{0} ({1}) <{2}> customer subscriber was not added to the list", user.DisplayName, user.UserID.ToString(), user.Email);
					}
				}
			}




		}

		private void AddBasicPortalUsers(List<UserEmail> userList, Ticket ticket)
		{
			Users users;
			users = new Users(LoginUser);
			users.LoadBasicPortalUsers(ticket.TicketID);
			foreach (User user in users)
			{
				if (AddUser(userList, user))
				{
					Logs.WriteEventFormat("{0} ({1}) <{2}> basic portal user was added to the list", user.DisplayName, user.UserID.ToString(), user.Email);
				}
				else
				{
					Logs.WriteEventFormat("{0} ({1}) <{2}> basic portal user was not added to the list", user.DisplayName, user.UserID.ToString(), user.Email);
				}
			}
		}

		private void AddAdvancedPortalUsers(List<UserEmail> userList, Ticket ticket)
		{
			Users users;
			users = new Users(LoginUser);
			users.LoadAdvancedPortalUsers(ticket.TicketID);
			foreach (User user in users)
			{
				if (AddUser(userList, user))
				{
					Logs.WriteEventFormat("{0} ({1}) <{2}> advanced portal user was added to the list", user.DisplayName, user.UserID.ToString(), user.Email);
				}
				else
				{
					Logs.WriteEventFormat("{0} ({1}) <{2}> advanced portal user was not added to the list", user.DisplayName, user.UserID.ToString(), user.Email);
				}
			}
		}

		private User AddPortalModifierIfClosing(List<UserEmail> userList, Ticket ticket, bool isProcessingBasicPortal, TicketStatus status)
		{
			Users users;
			User user = null;

			if (status.IsClosedEmail && ticket.ModifierID > 0)
			{
				users = new Users(LoginUser);
				users.LoadByUserID(ticket.ModifierID);

				if (users != null && users.Count > 0)
				{
					user = users.FirstOrDefault();

					if ((user.IsPortalUser && !isProcessingBasicPortal) || (!user.IsPortalUser && isProcessingBasicPortal))
					{
						AddUser(userList, user);
						Logs.WriteEventFormat("{0} ({1}) <{2}> modifier user was added to the list if it wasn't there already.", user.DisplayName, user.UserID.ToString(), user.Email);
					}
					else
					{
						Logs.WriteEventFormat("{0} ({1}) <{2}> modifier user is not a Portal User, not added to the list.", user.DisplayName, user.UserID.ToString(), user.Email);
					}
				}
				else
				{
					Logs.WriteEventFormat("ModifierId {0} was not found", ticket.ModifierID.ToString());
				}
			}

			return user;
		}

		private void AddUsersToAddresses(MailAddressCollection collection, List<UserEmail> users, int modifierID)
		{
			Logs.WriteEvent("Adding addresses to email message.");
			foreach (UserEmail user in users)
			{
				try
				{
					if (user != null && modifierID != user.UserID)
					{
						collection.Add(new MailAddress(user.Address, user.Name));
						Logs.WriteEvent(string.Format("Email: {0} <{1}>", user.Name, user.Address));

					}
					//if (modifierID != -1 && modifierID != user.UserID) collection.Add(new MailAddress(user.Address, user.Name));
				}
				catch
				{
				}
			}
		}

		private UserEmail FindByUserEmail(string address, List<UserEmail> list)
		{
			address = address.Trim();
			foreach (UserEmail item in list)
			{
				if (item.Address.ToLower().Trim() == address.ToLower()) return item;
			}
			return null;
		}

		private string GetViaTSHtmlAd(string orgName)
		{
			return string.Format(@"
<hr style=""border:none 0;border-top:1px solid #dedede;min-height:1px;margin:15px 0 10px 0"">
<div style=""margin:0px auto;text-align:left;font-size:12px;color:#aaaaaa;padding-bottom:6px;font-family:Arial,Helvetica,sans-serif;"">This email was sent from {0} via <a href=""http://www.teamsupport.com/?utm_source=tsemail&utm_medium=email&utm_campaign=custmail"" title=""TeamSupport.com"" style=""color:#2774a6;text-decoration:none;font-family:Arial,Helvetica,sans-serif;font-size:12px;"" target=""_blank"">TeamSupport</a></div>
", orgName);


		}

		private MailAddress GetMailAddress(string address)
		{
			return new MailAddress(FixMailAddress(address));
		}
		private MailAddress GetMailAddress(string address, string displayName)
		{
			return new MailAddress(FixMailAddress(address), FixMailName(displayName));
		}
		private MailAddress GetMailAddress(string address, string displayName, Encoding displayNameEncoding)
		{
			return new MailAddress(FixMailAddress(address), FixMailName(displayName), displayNameEncoding);
		}

		private string FixMailAddress(string address)
		{
			return address.Replace("<", "").Replace(">", "").Replace("|", " ");
		}

		private string FixMailName(string displayName)
		{
			return displayName.Replace("<", "").Replace(">", "").Replace("|", " ");
		}

		private string GetEmailReplyToAddress(LoginUser loginUser, Ticket ticket)
		{
			string emailReplyToAddress = ticket.EmailReplyToAddress;

			if (string.IsNullOrEmpty(emailReplyToAddress) && ticket.ProductID != null && ticket.ProductID > 0)
			{
				Product product = Products.GetProduct(loginUser, (int)ticket.ProductID);

				if (product != null)
				{
					emailReplyToAddress = product.EmailReplyToAddress;
					Logs.WriteEvent(string.Format("EmailReplyToAddress {0} from Product {1}", emailReplyToAddress, product.Name));
				}
				else
				{
					Logs.WriteEvent(string.Format("Product {0} associated to the ticket was not found.", ticket.ProductID));
				}
			}

			return emailReplyToAddress;
		}

		#endregion


	}
}
