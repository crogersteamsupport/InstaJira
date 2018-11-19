using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Data;
using TeamSupport.Data;
using System.Net;
using System.Collections.Specialized;
using Microsoft.CSharp;
using Newtonsoft.Json;

namespace TeamSupport.Api
{
    public class RestTickets
    {
        public static string GetTicket(RestCommand command, int ticketID)
        {
            TicketsViewItem ticket = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketID);
            if (ticket.OrganizationID != command.Organization.OrganizationID)
            {
                throw new RestException(HttpStatusCode.Unauthorized);
            }

            Tags tags = new Tags(command.LoginUser);
            tags.LoadByReference(ReferenceType.Tickets, ticket.TicketID);
            string ticketXmlString = ticket.GetXml("Ticket", true, tags);
            return ticketXmlString;
        }

        public static string GetTickets(RestCommand command)
        {
            string xml = "";
            bool hasBeenFiltered = false;
            int totalRecords = 0;

            if (command.IsPaging)
            {
                try
                {
                    TicketsView tickets = new TicketsView(command.LoginUser);
                    tickets.LoadAllTicketIds(command.Organization.OrganizationID, command.Filters, command.PageNumber, command.PageSize);
                    hasBeenFiltered = true;
                    XmlTextWriter writer = Tickets.BeginXmlWrite("Tickets");

                    foreach (int ticketTypeId in tickets.GroupBy(g => g.TicketTypeID).Select(p => p.Key).ToList())
                    {
                        try
                        {
                            TicketsView ticketsResult = new TicketsView(command.LoginUser);
                            ticketsResult.LoadByTicketIDList(command.Organization.OrganizationID, ticketTypeId, tickets.Where(w => w.TicketTypeID == ticketTypeId).Select(p => p.TicketID).ToList());

                            foreach (DataRow row in ticketsResult.Table.Rows)
                            {
                                int ticketId = (int)row["TicketID"];
                                Tags tags = new Tags(command.LoginUser);
                                tags.LoadByReference(ReferenceType.Tickets, ticketId);
                                tags = tags ?? new Tags(command.LoginUser);
                                ticketsResult.WriteXml(writer, row, "Ticket", true, !hasBeenFiltered ? command.Filters : new System.Collections.Specialized.NameValueCollection(), tags);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogs.LogException(command.LoginUser, ex, "API", "RestTickets. GetTickets(). Paging.");
                        }
                    }

                    if (tickets.Count > 0)
                    {
                        totalRecords = tickets[0].TotalRecords;
                    }

                    writer.WriteElementString("TotalRecords", totalRecords.ToString());
                    xml = Tickets.EndXmlWrite(writer);
                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(command.LoginUser, ex, "API", "RestTickets. GetTickets(). Paging. SQL filtering generation failed.");
                    throw new RestException(HttpStatusCode.InternalServerError, "There was an error processing your request. Please contact TeamSupport.com", ex);
                }
            }
            else
            {
                //No Paging
                if (command.Filters["TicketTypeID"] != null)
                {
                    try
                    {
                        TicketsView tickets = new TicketsView(command.LoginUser);
                        int ticketTypeID = int.Parse(command.Filters["TicketTypeID"]);
                        TicketType ticketType = TicketTypes.GetTicketType(command.LoginUser, ticketTypeID);
                        if (ticketType.OrganizationID != command.Organization.OrganizationID) throw new Exception();

                        try
                        {
                            tickets.LoadByTicketTypeID(ticketTypeID, command.Organization.OrganizationID, command.Filters);
                        }
                        catch (Exception ex)
                        {
                            //if something fails use the old method
                            tickets.LoadByTicketTypeID(ticketTypeID);
                            ExceptionLogs.LogException(command.LoginUser, ex, "API", "RestTickets. GetTickets(). No Paging. SQL filtering generation failed, fell into old method.");
                        }

                        xml = tickets.GetXml("Tickets", "Ticket", true, command.Filters);
                        xml = AddTagsToTickets(xml, command);
                    }
                    catch (Exception ex)
                    {
                        throw new RestException(HttpStatusCode.NotAcceptable, "Invalid TicketTypeID to filter.", ex);
                    }
                }
                else
                {
                    TicketTypes ticketTypes = new TicketTypes(command.LoginUser);
                    ticketTypes.LoadByOrganizationID(command.Organization.OrganizationID);

                    TicketsView tickets = new TicketsView(command.LoginUser);
                    XmlTextWriter writer = Tickets.BeginXmlWrite("Tickets");

                    foreach (TicketType ticketType in ticketTypes)
                    {
                        try
                        {
                            tickets.LoadByTicketTypeID(ticketType.TicketTypeID, command.Organization.OrganizationID, command.Filters);
                        }
                        catch (Exception ex)
                        {
							if (ex is System.Data.SqlClient.SqlException && ex.Message.ToLower().Contains("variable names must be unique within a query batch or stored procedure"))
							{
								throw ex;
							}
							else
							{
                            //if something fails use the old method
                            tickets.LoadByTicketTypeID(ticketType.TicketTypeID);
                            ExceptionLogs.LogException(command.LoginUser, ex, "API", "RestTickets. GetTickets(). No Paging. No TicketTypeId filter. SQL filtering generation failed, fell into old method.");
                        }
                        }

                        foreach (DataRow row in tickets.Table.Rows)
                        {
                            int ticketId = (int)row["TicketID"];
                            Tags tags = new Tags(command.LoginUser);
                            tags.LoadByReference(ReferenceType.Tickets, ticketId);
                            tags = tags ?? new Tags(command.LoginUser);
                            tickets.WriteXml(writer, row, "Ticket", true, command.Filters, tags);
                        }
                    }

                    xml = Tickets.EndXmlWrite(writer);
                }
            }

            return xml;
        }

		public static string GetTicketAssignments(RestCommand command, bool isPost = false)
		{
			string ticketIdFilter = "ticketids";
			string ticketNumberFilter = "ticketnumbers";
			string xml = "";
			bool byId = false;
			dynamic errors = new System.Dynamic.ExpandoObject();

			if (!isPost && !command.Filters.AllKeys.Where(p => p.ToLower() == "ticketids").Any() && !command.Filters.AllKeys.Where(p => p.ToLower() == "ticketnumbers").Any())
			{
				errors.Error = "This call requires a filter. TicketIds or TicketNumbers should be used to filter the results.";
				throw new RestException(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(errors));
			}
			else if (isPost && string.IsNullOrEmpty(command.Data))
			{
				errors.Error = "This call requires a filter in the POST body. TicketIds or TicketNumbers should be used to filter the results.";
				throw new RestException(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(errors));
			}

			string tickets = string.Empty;

			if (isPost)
			{
				System.Xml.Linq.XDocument xmlDoc = System.Xml.Linq.XDocument.Parse(command.Data.ToLower());
				string jsonText = JsonConvert.SerializeXNode(xmlDoc);
				dynamic filters = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(jsonText);

				if (((IDictionary<String, object>)filters).ContainsKey(ticketIdFilter))
				{
					tickets = filters.ticketids;
					byId = true;
				}
				else if (((IDictionary<String, object>)filters).ContainsKey(ticketNumberFilter))
				{
					tickets = filters.ticketnumbers;
				}
			}
			else
			{
				foreach (string key in command.Filters)
				{
					if (key.ToLower().Trim() == ticketIdFilter)
					{
						tickets = command.Filters[key.ToLower().Trim()];
						byId = true;
						break;
					}

					if (key.ToLower().Trim() == ticketNumberFilter)
					{
						tickets = command.Filters[key.ToLower().Trim()];
						break;
					}
				}
			}

			TicketUserAssignment userTicketAssignments = new TicketUserAssignment(command.LoginUser);
			userTicketAssignments.LoadByTicketList(command.Organization.OrganizationID, tickets, byId);

			int pageNumber = command.IsPaging && command.PageNumber != null ? (int)--command.PageNumber : 0;
			int pageSize = command.IsPaging && command.PageSize != null ? (int)command.PageSize : userTicketAssignments.History.Select(p => p.TicketID).Distinct().Count();
			TicketUserAssignment.TicketUserAssignmentHistory assignments = userTicketAssignments.GetTicketUserAssignmentHistory(pageNumber, pageSize);

			string jsonResult = JsonConvert.SerializeObject(assignments);
			var doc = JsonConvert.DeserializeXNode(jsonResult, "TicketAssignments");
			xml = doc.ToString();

			return xml;
		}

		public static string GetZapierTickets(RestCommand command, int limitNumber)
        {
            string xml = "";

            TicketsView tickets = new TicketsView(command.LoginUser);
            XmlTextWriter writer = Tickets.BeginXmlWrite("Tickets");

            tickets.LoadByOrganizationIDOrderByNumberDESC(command.Organization.OrganizationID, limitNumber);
            foreach (DataRow row in tickets.Table.Rows)
            {
                tickets.WriteXml(writer, row, "Ticket", true, command.Filters);
            }

            xml = Tickets.EndXmlWrite(writer);

            //return tickets.GetXml("Tickets", "Ticket", command.Filters["TicketTypeID"] != null, command.Filters);
            return xml;
        }

        public static string GetTicketsByCustomerID(RestCommand command, int customerID, bool orderByDateCreated = false)
        {
            TicketsView tickets = new TicketsView(command.LoginUser);
			string orderBy = orderByDateCreated ? "ot.DateCreated DESC" : "TicketNumber";
			string xml = "";

			if (command.IsPaging)
			{
				try
				{
					//remove Paging parameters
					NameValueCollection filters = new NameValueCollection();

					foreach (string key in command.Filters.AllKeys)
					{
						if (key.ToLower() != "pagenumber" && key.ToLower() != "pagesize")
						{
							filters.Add(key, command.Filters[key]);
						}
					}

					tickets.LoadByCustomerID(customerID, filters, (int)command.PageNumber, (int)command.PageSize, orderBy);
					XmlTextWriter writer = Tickets.BeginXmlWrite("Tickets");

					foreach (DataRow row in tickets.Table.Rows)
					{
						int ticketId = (int)row["TicketID"];
						Tags tags = new Tags(command.LoginUser);
						tags.LoadByReference(ReferenceType.Tickets, ticketId);
						tags = tags ?? new Tags(command.LoginUser);
						tickets.WriteXml(writer, row, "Ticket", true, new NameValueCollection(), tags);
					}

					int totalRecords = 0;

					if (tickets.Count > 0)
					{
						totalRecords = tickets[0].TotalRecords;
					}

					writer.WriteElementString("TotalRecords", totalRecords.ToString());
					xml = Tickets.EndXmlWrite(writer);
				}
				catch (Exception ex)
				{
					ExceptionLogs.LogException(command.LoginUser, ex, "API", "RestTickets. GetTicketsByCustomerID(). Paging. SQL filtering generation failed.");
					throw new RestException(HttpStatusCode.InternalServerError, "There was an error processing your request. Please contact TeamSupport.com", ex);
				}
			}
			else
			{
				tickets.LoadByCustomerID(customerID, orderBy);
				xml = tickets.GetXml("Tickets", "Ticket", true, command.Filters, command.IsPaging);
				xml = AddTagsToTickets(xml, command);
			}

			return xml;
        }

        public static string GetTicketsByContactID(RestCommand command, int contactID, bool orderByDateCreated = false)
        {
            TicketsView tickets = new TicketsView(command.LoginUser);

            if (orderByDateCreated)
            {
                tickets.LoadByContactID(contactID, "ut.DateCreated DESC");
            }
            else
            {
                tickets.LoadByContactID(contactID);
            }

            return tickets.GetXml("Tickets", "Ticket", command.Filters["TicketTypeID"] != null, command.Filters);
        }

        public static string CreateTicket(RestCommand command)
        {
            Tickets tickets = new Tickets(command.LoginUser);
            Ticket ticket = tickets.AddNewTicket();
            ticket.OrganizationID = command.Organization.OrganizationID;
            string description = string.Empty;
            int? contactID = null;
            int? customerID = null;
            ticket.FullReadFromXml(command.Data, true, ref description, ref contactID, ref customerID);
            ticket.TicketSource = "API";
            ticket.NeedsIndexing = true;
            ticket.Collection.Save();
            ticket.UpdateCustomFieldsFromXml(command.Data);

            if (contactID != null)
            {
                ticket.Collection.AddContact((int)contactID, ticket.TicketID);
            }

            if (customerID != null)
            {
                ticket.Collection.AddOrganization((int)customerID, ticket.TicketID);
            }

            Actions actions = new Actions(command.LoginUser);
            TeamSupport.Data.Action action = actions.AddNewAction();
            action.ActionTypeID = null;
            action.Name = "Description";
            action.SystemActionTypeID = SystemActionType.Description;
            action.Description = description;
            action.IsVisibleOnPortal = ticket.IsVisibleOnPortal;
            action.IsKnowledgeBase = ticket.IsKnowledgeBase;
            action.TicketID = ticket.TicketID;
            actions.Save();

            UpdateFieldsOfSeparateTable(command, ticket);

            return TicketsView.GetTicketsViewItem(command.LoginUser, ticket.TicketID).GetXml("Ticket", true);
        }

        public static string UpdateTicket(RestCommand command, int ticketIDOrNumber)
        {
            TicketsViewItem ticketViewItem = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketIDOrNumber);
            Ticket ticket = Tickets.GetTicket(command.LoginUser, ticketViewItem.TicketID);
            if (ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
            ticket.ReadFromXml(command.Data, false);
            ticket.Collection.Save();
            ticket.UpdateCustomFieldsFromXml(command.Data);

            ticket = Tickets.GetTicket(command.LoginUser, ticket.TicketID);
            UpdateFieldsOfSeparateTable(command, ticket);

            return TicketsView.GetTicketsViewItem(command.LoginUser, ticket.TicketID).GetXml("Ticket", true);
        }

        public static string DeleteTicket(RestCommand command, int ticketIDOrNumber)
        {
            TicketsViewItem ticketViewItem = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketIDOrNumber);
            Ticket ticket = Tickets.GetTicket(command.LoginUser, ticketViewItem.TicketID);
            string result = ticketViewItem.GetXml("Ticket", true);
            if (ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
            ticket.Delete();
            ticket.Collection.Save();
            return result;
        }

        // Customer Only Methods

        public static string GetCustomerTicket(RestCommand command, int ticketID)
        {
            TicketsViewItem ticket = TicketsView.GetTicketsViewItemByIdOrNumberForCustomer(command.LoginUser, (int)command.Organization.ParentID, ticketID);
            if (ticket.OrganizationID != command.Organization.ParentID || !ticket.GetIsCustomer(command.Organization.OrganizationID))
            {
                throw new RestException(HttpStatusCode.Unauthorized);
            }

            Tags tags = new Tags(command.LoginUser);
            tags.LoadByReference(ReferenceType.Tickets, ticket.TicketID, command.Organization.ParentID);

            return ticket.GetXml("Ticket", true, tags);
        }

        public static string GetCustomerTickets(RestCommand command)
        {
            TicketsView tickets = new TicketsView(command.LoginUser);
			string xml = "";

			if (command.Filters["TicketTypeID"] != null)
            {
                try
                {
                    int ticketTypeID = int.Parse(command.Filters["TicketTypeID"]);
                    TicketType ticketType = TicketTypes.GetTicketType(command.LoginUser, ticketTypeID);
                    if (ticketType.OrganizationID != command.Organization.ParentID) throw new Exception();
                    tickets.LoadByCustomerTicketTypeID(command.Organization.OrganizationID, ticketTypeID);
                }
                catch (Exception ex)
                {
                    throw new RestException(HttpStatusCode.NotAcceptable, ex.Message);
                    throw new RestException(HttpStatusCode.NotAcceptable, "Invalid TicketTypeID to filter.", ex);
                }
            }
            else
            {
				if (command.IsPaging)
				{
					try
					{
						//remove Paging parameters
						NameValueCollection filters = new NameValueCollection();

						foreach (string key in command.Filters.AllKeys)
						{
							if (key.ToLower() != "pagenumber" && key.ToLower() != "pagesize")
							{
								filters.Add(key, command.Filters[key]);
							}
						}

						tickets.LoadByCustomerID(command.Organization.OrganizationID, command.Filters, (int)command.PageNumber, (int)command.PageSize);

						XmlTextWriter writer = Tickets.BeginXmlWrite("Tickets");

						foreach (DataRow row in tickets.Table.Rows)
						{
							int ticketId = (int)row["TicketID"];
							Tags tags = new Tags(command.LoginUser);
							tags.LoadByReference(ReferenceType.Tickets, ticketId);
							tags = tags ?? new Tags(command.LoginUser);
							tickets.WriteXml(writer, row, "Ticket", true, new NameValueCollection(), tags);
						}

						int totalRecords = 0;

						if (tickets.Count > 0)
						{
							totalRecords = tickets[0].TotalRecords;
						}

						writer.WriteElementString("TotalRecords", totalRecords.ToString());
						xml = Tickets.EndXmlWrite(writer);
					}
					catch (Exception ex)
					{
						ExceptionLogs.LogException(command.LoginUser, ex, "API", "RestTickets. GetCustomerTickets(). Paging. SQL filtering generation failed.");
						throw new RestException(HttpStatusCode.InternalServerError, "There was an error processing your request. Please contact TeamSupport.com", ex);
					}
				}
				else
				{
					tickets.LoadByCustomerID(command.Organization.OrganizationID);
					xml = tickets.GetXml("Tickets", "Ticket", true, command.Filters, command.IsPaging);
					xml = AddTagsToTickets(xml, command, true);
				}
            }

			return xml;
        }

        public static string CreateCustomerTicket(RestCommand command)
        {
            Tickets tickets = new Tickets(command.LoginUser);
            Ticket ticket = tickets.AddNewTicket();
            ticket.OrganizationID = (int)command.Organization.ParentID;
			ticket.TicketSource = "API";
			ticket.NeedsIndexing = true;
			string description = string.Empty;
			int? contactID = null;
			int? customerID = null;
			ticket.FullReadFromXml(command.Data, true, ref description, ref contactID, ref customerID);
			ticket.Collection.Save();
			ticket.UpdateCustomFieldsFromXml(command.Data);

			Actions actions = new Actions(command.LoginUser);
            Data.Action action = actions.AddNewAction();
            action.ActionTypeID = null;
            action.Name = "Description";
            action.SystemActionTypeID = SystemActionType.Description;
			action.Description = description;
            action.IsVisibleOnPortal = ticket.IsVisibleOnPortal;
            action.IsKnowledgeBase = ticket.IsKnowledgeBase;
            action.TicketID = ticket.TicketID;
            actions.Save();

            tickets.AddOrganization(command.Organization.OrganizationID, ticket.TicketID);
			UpdateFieldsOfSeparateTable(command, ticket, true);
			return TicketsView.GetTicketsViewItem(command.LoginUser, ticket.TicketID).GetXml("Ticket", true);
        }

        public static string UpdateCustomerTicket(RestCommand command, int ticketID)
        {
            Ticket ticket = Tickets.GetTicket(command.LoginUser, ticketID);
            if (ticket.OrganizationID != command.Organization.ParentID || !ticket.GetTicketView().GetIsCustomer(command.Organization.OrganizationID)) throw new RestException(HttpStatusCode.Unauthorized);
            ticket.ReadFromXml(command.Data, false);
            ticket.Collection.Save();
            ticket.UpdateCustomFieldsFromXml(command.Data);
            ticket = Tickets.GetTicket(command.LoginUser, ticket.TicketID);
            return TicketsView.GetTicketsViewItem(command.LoginUser, ticket.TicketID).GetXml("Ticket", true);
        }

        public static string GetRelatedTickets(RestCommand command, int ticketIDOrTicketNumber)
        {
            TicketsView tickets = new TicketsView(command.LoginUser);
            tickets.LoadRelated(ticketIDOrTicketNumber);
            if (tickets.IsEmpty)
            {
                tickets = new TicketsView(command.LoginUser);
                tickets.LoadRelatedByTicketNumber(ticketIDOrTicketNumber, command.Organization.OrganizationID);
            }
            if (tickets.Count > 0 && tickets[0].OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

            return tickets.GetXml("Tickets", "Ticket", true, command.Filters);
        }

        public static string GetTicketsByAssetID(RestCommand command, int assetID, bool orderByDateCreated = false)
        {
            TicketsView tickets = new TicketsView(command.LoginUser);
            if (orderByDateCreated)
            {
                tickets.LoadByAssetID(assetID, "at.DateCreated DESC");
            }
            else
            {
                tickets.LoadByAssetID(assetID);
            }

            return tickets.GetXml("Tickets", "Ticket", command.Filters["TicketTypeID"] != null, command.Filters);
        }

        /// <summary>
        /// Update the Ticket related fields that live in their own table.
        /// </summary>
        /// <param name="command">Command received in the request to read and process the data in the request body.</param>
        /// <param name="ticketId">TicketId to update its record.</param>
        private static void UpdateFieldsOfSeparateTable(RestCommand command, Ticket ticket, bool isCustomerTicket = false)
        {
            try
            {
                //Add as necessary to the list and then to the switch-case below for the work to update it.
                List<string> fields = new List<string>() { "jirakey", "tags" };

                foreach (string field in fields.Select(p => p.ToLower()).ToList())
                {
                    XmlNode node = GetNode(command, field);

                    if (node != null)
                    {
                        switch (field)
                        {
                            case "jirakey":
                                string jiraKey = node.InnerText;
                                TicketLinkToJira ticketLinkToJira = new TicketLinkToJira(command.LoginUser);
                                ticketLinkToJira.LoadByTicketID(ticket.TicketID);
                                int? crmLinkId = null;

                                //Next line and 2 If statements are the same as in \webapp\app_code\ticketservice.cs SetSyncWithJira(). Might need to consider making a common funcion for both
                                crmLinkId = CRMLinkTable.GetIdBy(ticket.OrganizationID, IntegrationType.Jira.ToString().ToLower(), ticket.ProductID, command.LoginUser);

                                //If product is not associated to an instance then get the 'default' instance
                                if (crmLinkId == null || crmLinkId <= 0)
                                {
                                    CRMLinkTable crmlink = new CRMLinkTable(command.LoginUser);
                                    crmlink.LoadByOrganizationID(ticket.OrganizationID);

                                    crmLinkId = crmlink.Where(p => p.InstanceName == "Default"
                                                                                        && p.CRMType.ToLower() == IntegrationType.Jira.ToString().ToLower())
                                                                                        .Select(p => p.CRMLinkID).FirstOrDefault();
                                }

                                if (ticketLinkToJira != null && ticketLinkToJira.Any())
                                {
                                    string oldJiraKey = ticketLinkToJira[0].JiraKey;
                                    ticketLinkToJira[0].JiraKey = jiraKey.ToLower() == "newjiraissue" ? null : jiraKey;
                                    ticketLinkToJira[0].CrmLinkID = crmLinkId;
                                    ticketLinkToJira.Save();
                                    ActionLogs.AddActionLog(command.LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, string.Format("Changed JiraKey from '{0}' to '{1}'.", oldJiraKey, jiraKey));
                                }
                                else
                                {
                                    TicketLinkToJiraItem newJiraLink = ticketLinkToJira.AddNewTicketLinkToJiraItem();
                                    newJiraLink.TicketID = ticket.TicketID;
                                    newJiraLink.SyncWithJira = true;
                                    newJiraLink.JiraID = null;
                                    newJiraLink.JiraKey = jiraKey.ToLower() == "newjiraissue" ? null : jiraKey;
                                    newJiraLink.JiraLinkURL = null;
                                    newJiraLink.JiraStatus = null;
                                    newJiraLink.CreatorID = command.LoginUser.UserID;
                                    newJiraLink.CrmLinkID = crmLinkId;

                                    if (newJiraLink.CrmLinkID != null && newJiraLink.CrmLinkID > 0)
                                    {
                                        newJiraLink.Collection.Save();
                                        ActionLogs.AddActionLog(command.LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, string.Format("Linked to JiraKey '{0}'.", jiraKey));
                                    }

                                }
                                break;
                            case "tags":
                                TagLinks currentTagLinks = new TagLinks(command.LoginUser);
                                currentTagLinks.LoadByReference(ReferenceType.Tickets, ticket.TicketID);
                                XmlNodeList nodeList = node.ChildNodes;
                                List<int> newTags = new List<int>();

                                foreach (XmlNode tagNode in nodeList)
                                {
                                    string tagValue = tagNode["Value"].InnerText;

                                    Tag newTag = Tags.GetTag(command.LoginUser, tagValue);

                                    if (newTag == null)
                                    {
                                        Tags tag = new Tags(command.LoginUser);
                                        newTag = tag.AddNewTag();
                                        newTag.OrganizationID = isCustomerTicket ? command.Organization.ParentID ?? command.Organization.OrganizationID : command.Organization.OrganizationID;
                                        newTag.Value = tagValue;
                                        tag.Save();
                                    }

                                    newTags.Add(newTag.TagID);
                                }

                                foreach (int tag in newTags)
                                {
                                    TagLink newTagLink = currentTagLinks.Where(p => p.TagID == tag && p.RefID == ticket.TicketID).SingleOrDefault();
                                    if (newTagLink == null)
                                    {
                                        TagLinks tagLink = new TagLinks(command.LoginUser);
                                        newTagLink = tagLink.AddNewTagLink();
                                        newTagLink.TagID = tag;
                                        newTagLink.RefType = ReferenceType.Tickets;
                                        newTagLink.RefID = ticket.TicketID;
                                        tagLink.Save();
                                    }
                                }

                                List<TagLink> deleteTagLinks = currentTagLinks.Where(p => !newTags.Contains(p.TagID)).ToList();

                                foreach (TagLink deleteTagLink in deleteTagLinks)
                                {
                                    deleteTagLink.Delete();
                                    deleteTagLink.Collection.Save();
                                }

                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(command.LoginUser, ex, "API", string.Format("OrgID: {0}{1}Verb: {2}{1}Url: {3}{1}Body: {4}", command.Organization.OrganizationID, Environment.NewLine, command.Method, command.Method, command.Data));
            }
        }

        private static XmlNode GetNode(RestCommand command, string field)
        {
            XmlDocument xml = new XmlDocument();
            xml.XmlResolver = null;
            xml.LoadXml(command.Data);
            string query = "*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='ticket']" +
                            "/*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='{0}']";
            query = string.Format(query, field);
            XmlNode node = xml.SelectSingleNode(query);

            //If node not found and the request includes a top level of collection items then try again with it
            if (node == null)
            {
                query = "*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='tickets']" +
                        "/*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='ticket']" +
                        "/*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='{0}']";
                query = string.Format(query, field);
                XmlNodeList nodeList = xml.SelectNodes(query);

                if (nodeList != null && nodeList.Count > 0)
                {
                    node = nodeList[0];
                }
            }

            return node;
        }

        /// <summary>
        /// Add the Tag tags to the final xml of the tickets
        /// </summary>
        /// <param name="ticketsXml">Tickets xml string.</param>
        /// <returns>An xml string with the Tickets data and its tags.</returns>
        private static string AddTagsToTickets(string ticketsXml, RestCommand command, bool isCustomerToken = false)
        {
            string xml = string.Empty;
            //Add the tag nodes to each ticket object
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;
            xmlDoc.LoadXml(ticketsXml);

            //Do we have paging?
            XmlNode totalRecordsElement = xmlDoc.SelectSingleNode("/Tickets/TotalRecords");

            XmlNodeList nodeList = xmlDoc.SelectNodes("/Tickets/Ticket");
            string xmlResultString = string.Empty;

            using (var xmlTextWriter = Tickets.BeginXmlWrite("Tickets"))
            {
                if (totalRecordsElement != null)
                {
                    totalRecordsElement.WriteTo(xmlTextWriter);
                    totalRecordsElement = null;
                }

                foreach (XmlNode node in nodeList)
                {
                    //get the tags for the ticket using the ticketId
                    try
                    {
                        string ticketIdString = node["TicketID"].InnerText;
                        Tags tags = new Tags(command.LoginUser);
                        tags.LoadByReference(ReferenceType.Tickets, int.Parse(ticketIdString), isCustomerToken ? command.Organization.ParentID : null);
                        XmlTextWriter writer = Tags.BeginXmlWrite("Tags");

                        foreach (DataRow row in tags.Table.Rows)
                        {
                            tags.WriteXml(writer, row, "Tag", false, null);
                        }

                        string tagXmlString = string.Empty;
                        tagXmlString = Tags.EndXmlWrite(writer);
                        XmlDocument xmlTag = new XmlDocument();
                        xmlDoc.XmlResolver = null;
                        xmlTag.LoadXml(tagXmlString);
                        node.AppendChild(node.OwnerDocument.ImportNode(xmlTag.FirstChild.NextSibling, true));
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogs.LogException(command.LoginUser, ex, "API", string.Format("OrgID: {0}{1}Verb: {2}{1}Url: {3}{1}Body: {4}", command.Organization.OrganizationID, Environment.NewLine, command.Method, command.Method, command.Data));
                    }

                    node.WriteTo(xmlTextWriter);
                }

                xml = Tickets.EndXmlWrite(xmlTextWriter);
            }

            return xml;
        }

        private static string AddSingleXmlElement(string ticketsXml, string element, string value)
        {
            string xml = string.Empty;
            //Add the tag nodes to each ticket object
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;
            xmlDoc.LoadXml(ticketsXml);

            //Create a new node.
            XmlElement elem = xmlDoc.CreateElement(element);
            elem.InnerText = value;

            //Add the node to the document.
            xmlDoc.DocumentElement.AppendChild(elem);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.NewLineOnAttributes = true;
            settings.Indent = true;

            string xmlResult = string.Empty;

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter, settings))
            {
                xmlDoc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                xmlResult = stringWriter.GetStringBuilder().ToString();
            }

            return xmlResult;
        }
    }
}
