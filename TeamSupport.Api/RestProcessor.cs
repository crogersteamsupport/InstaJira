using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TeamSupport.Data;
using System.IO;
using System.Net;
using System.Xml;

namespace TeamSupport.Api
{
    public class RestProcessor
    {
        RestCommand _command;
        string _xmlHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n";

        public RestProcessor(RestCommand command)
        {
            _command = command;
        }

        public void Process()
        {
            if (_command.Format == RestFormat.XML || _command.Format == RestFormat.JSON)
            {
                if (_command.Format == RestFormat.JSON && _command.Data != string.Empty)
                {
                    if (_command.Context.Request.Files.Count == 0)
                    {
                        XmlDocument xmlData = new XmlDocument();
                        xmlData.XmlResolver = null;
                        xmlData = JsonConvert.DeserializeXmlNode(_command.Data);

						//Make sure the DTDs references are removed.
						XmlDocumentType XDType = xmlData.DocumentType;

						if (XDType != null)
						{
							xmlData.RemoveChild(XDType);
						}

						_command.Data = xmlData.InnerXml;
                    }
                }

                string uriTemplate = GetURITemplate(_command.Segments);

                string data = "";
                switch (_command.Method)
                {
                    case HttpMethod.Get: data = ProcessGet(uriTemplate); break;
                    case HttpMethod.Put: data = ProcessPut(uriTemplate); break;
                    case HttpMethod.Post: data = ProcessPost(uriTemplate); break;
                    case HttpMethod.Delete: data = ProcessDelete(uriTemplate); break;
                    case HttpMethod.Unsupported: throw new RestException(HttpStatusCode.MethodNotAllowed);
                    default: throw new RestException(HttpStatusCode.MethodNotAllowed);
                }

                if (data != "attachment")
                {
                    _command.Context.Response.ContentType = "text/xml";
                    if (_command.Format == RestFormat.JSON && !string.IsNullOrEmpty(data))
                    {
                        _command.Context.Response.ContentType = "application/json";

                        XmlDocument doc = new XmlDocument();
                        doc.XmlResolver = null;
                        doc.LoadXml(data.Replace(_xmlHeader, string.Empty));

                        if (_command.Method == HttpMethod.Get)
                        {
                            data = FixJsonFormatError(JsonConvert.SerializeXmlNode(doc), uriTemplate, _command.IsPaging);
                        }
                        else
                        {
                            data = JsonConvert.SerializeXmlNode(doc);
                        }
                    }
                    _command.Context.Response.Write(data);
                }
            }
            else
            {
                throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
            }
        }

        public string ProcessGet(string uriTemplate)
        {
            string data = "";
            _command.StatusCode = HttpStatusCode.OK;

            if (!_command.IsCustomerOnly)
            {
                int zapierLimit = 50;

                switch (uriTemplate)
                {
                    case "/deletedtickets/": data = RestDeletedTickets.GetDeletedTickets(_command); break;
                    case "/tickets/": data = RestTickets.GetTickets(_command); break;
                    case "/ticketsreport/": data = RestReportTicketsView.GetTickets(_command); break;
                    case "/tickets/{id}/": data = RestTickets.GetTicket(_command, GetId(1)); break;
                    case "/ticketsreport/{id}/": data = RestReportTicketsView.GetTicket(_command, GetId(1)); break;
                    case "/tickets/{id}/history/": data = RestActionLogs.GetItemsByTicketIDOrNumber(_command, GetId(1)); break;
                    case "/tickets/{id}/customers/": data = RestOrganizations.GetTicketOrganizations(_command, GetId(1)); break;
                    case "/tickets/{id}/customers/{id}/": data = RestOrganizations.GetOrganization(_command, GetId(3)); break;
                    case "/tickets/{id}/contacts/": data = RestContacts.GetTicketContacts(_command, GetId(1)); break;
                    case "/tickets/{id}/contacts/{id}/": data = RestContacts.GetItem(_command, GetId(3)); break;
                    case "/tickets/{id}/actions/": data = RestActions.GetActions(_command, GetId(1)); break;
                    case "/tickets/{id}/actions/{id}/": data = RestActions.GetAction(_command, GetId(3)); break;
                    case "/tickets/{id}/actions/{id}/attachments/": data = RestAttachments.GetAttachments(_command, GetId(3)); break;
                    case "/tickets/{id}/actions/{id}/attachments/{id}/": data = RestAttachments.GetAttachment(_command, GetId(5)); break;
                    case "/tickets/{id}/relatedtickets/": data = RestTickets.GetRelatedTickets(_command, GetId(1)); break;
                    case "/tickets/{id}/assets/": data = RestAssetsView.GetAssetsView(_command, GetId(1)); break;
					case "/tickets/assignmenthistory/": data = RestTickets.GetTicketAssignments(_command); break;
                    case "/customers/": data = RestOrganizations.GetOrganizations(_command); break;
                    case "/customers/{id}/": data = RestOrganizations.GetOrganization(_command, GetId(1)); break;
                    case "/customers/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Organizations, GetId(1)); break;
                    case "/customers/{id}/tickets/": data = RestTickets.GetTicketsByCustomerID(_command, GetId(1)); break;
                    case "/customers/{id}/phonenumbers/": data = RestPhoneNumbers.GetPhoneNumbers(_command, ReferenceType.Organizations, GetId(1)); break;
                    case "/customers/{id}/phonenumbers/{id}/": data = RestPhoneNumbers.GetPhoneNumber(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
                    case "/customers/{id}/addresses/": data = RestAddresses.GetAddresses(_command, ReferenceType.Organizations, GetId(1)); break;
                    case "/customers/{id}/addresses/{id}/": data = RestAddresses.GetAddress(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
                    case "/customers/{id}/notes/": data = RestNotes.GetNotes(_command, ReferenceType.Organizations, GetId(1)); break;
                    case "/customers/{id}/notes/{id}/": data = RestNotes.GetNote(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
                    case "/customers/{id}/contacts/": data = RestContacts.GetItems(_command, GetId(1)); break;
                    case "/customers/{id}/contacts/{id}/": data = RestContacts.GetItem(_command, GetId(3)); break;
                    case "/customers/{id}/contacts/{id}/addresses/": data = RestAddresses.GetAddresses(_command, ReferenceType.Users, GetId(3)); break;
                    case "/customers/{id}/contacts/{id}/addresses/{id}/": data = RestAddresses.GetAddress(_command, ReferenceType.Users, GetId(3), GetId(5)); break;
                    case "/customers/{id}/contacts/{id}/phonenumbers/": data = RestPhoneNumbers.GetPhoneNumbers(_command, ReferenceType.Users, GetId(3)); break;
                    case "/customers/{id}/contacts/{id}/phonenumbers/{id}/": data = RestPhoneNumbers.GetPhoneNumber(_command, ReferenceType.Users, GetId(3), GetId(5)); break;
                    case "/customers/{id}/products/": data = RestOrganizationProducts.GetOrganizationProducts(_command, GetId(1)); break;
                    case "/customers/{id}/products/{id}/": data = RestOrganizationProducts.GetOrganizationProductItems(_command, GetId(1), GetId(3)); break;
                    case "/customers/{id}/products/{id}/customerproduct/{id}/": data = RestOrganizationProducts.GetOrganizationProductItem(_command, GetId(1), GetId(3), GetId(5)); break;
                    case "/customers/{id}/products/{id}/versions/": data = RestVersions.GetOrganizationVersions(_command, GetId(3), GetId(1)); break;
                    case "/customers/{id}/products/{id}/versions/{id}/": data = RestOrganizationProducts.GetOrganizationProductVersionItems(_command, GetId(1), GetId(3), GetId(5)); break;
                    case "/contacts/": data = RestContacts.GetItems(_command); break;
                    case "/contacts/{id}/": data = RestContacts.GetItem(_command, GetId(1)); break;
                    case "/contacts/{id}/tickets/": data = RestTickets.GetTicketsByContactID(_command, GetId(1)); break;
                    case "/contacts/{id}/addresses/": data = RestAddresses.GetAddresses(_command, ReferenceType.Users, GetId(1)); break;
                    case "/contacts/{id}/addresses/{id}/": data = RestAddresses.GetAddress(_command, ReferenceType.Users, GetId(1), GetId(3)); break;
                    case "/contacts/{id}/phonenumbers/": data = RestPhoneNumbers.GetPhoneNumbers(_command, ReferenceType.Users, GetId(1)); break;
                    case "/contacts/{id}/phonenumbers/{id}/": data = RestPhoneNumbers.GetPhoneNumber(_command, ReferenceType.Users, GetId(1), GetId(3)); break;
                    case "/products/": data = RestProducts.GetProducts(_command); break;
                    case "/products/{id}/": data = RestProducts.GetProduct(_command, GetId(1)); break;
                    case "/products/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Products, GetId(1)); break;
                    case "/products/{id}/customers/": data = RestOrganizations.GetProductOrganizations(_command, GetId(1)); break;
                    case "/products/{id}/customers/{id}/": data = RestOrganizationProducts.GetOrganizationProductItems(_command, GetId(3), GetId(1)); break;
                    case "/products/{id}/versions/": data = RestVersions.GetItems(_command, GetId(1)); break;
                    case "/products/{id}/versions/{id}/": data = RestVersions.GetItem(_command, GetId(3)); break;
                    case "/products/{id}/versions/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.ProductVersions, GetId(3)); break;
                    case "/products/{id}/versions/{id}/customers/": data = RestOrganizations.GetVersionOrganizations(_command, GetId(3)); break;
                    case "/products/{id}/versions/{id}/customers/{id}/": data = RestOrganizationProducts.GetOrganizationProductVersionItems(_command, GetId(5), GetId(1), GetId(3)); break;
                    case "/assets/": data = RestAssetsView.GetAssetsView(_command); break;
                    case "/assets/{id}/": data = RestAssetsView.GetAssetsViewItem(_command, GetId(1)); break;
                    case "/assets/{id}/history/": data = RestAssetHistoryView.GetAssetHistoryView(_command, GetId(1)); break;
                    case "/assets/{id}/assignments/": data = RestAssetAssignmentsView.GetAssetAssignmentsView(_command, GetId(1)); break;
                    case "/assets/{id}/tickets/": data = RestTickets.GetTicketsByAssetID(_command, GetId(1), true); break;
                    case "/assets/{id}/attachments/": data = RestAttachments.GetAttachmentsByAssetID(_command, GetId(1), true); break;
                    case "/assets/{id}/attachments/{id}/": data = RestAttachments.GetAttachment(_command, GetId(3)); break;
                    case "/groups/": data = RestGroups.GetGroups(_command); break;
                    case "/groups/{id}/": data = RestGroups.GetGroup(_command, GetId(1)); break;
                    case "/users/": data = RestUsers.GetUsers(_command); break;
                    case "/users/{id}/": data = RestUsers.GetUser(_command, GetId(1)); break;
                    case "/users/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Users, GetId(1)); break;
                    case "/wiki/": data = RestWikiArticles.GetWikiArticles(_command); break;
                    case "/wiki/{id}/": data = RestWikiArticles.GetWikiArticle(_command, GetId(1)); break;
                    case "/properties/actiontypes/": data = RestProperties.GetActionTypes(_command); break;
                    case "/properties/phonetypes/": data = RestProperties.GetPhoneTypes(_command); break;
                    case "/properties/productversionstatuses/": data = RestProperties.GetProductVersionStatuses(_command); break;
                    case "/properties/ticketseverities/": data = RestProperties.GetTicketSeverities(_command); break;
                    case "/properties/ticketstatuses/": data = RestProperties.GetTicketStatuses(_command); break;
                    case "/properties/tickettypes/": data = RestProperties.GetTicketTypes(_command); break;
                    case "/properties/knowledgebasecategories/": data = RestProperties.GetKnowledgeBaseCategories(_command); break;

                    case "/zapier/tickets/": data = RestTickets.GetZapierTickets(_command, zapierLimit); break;
                    case "/zapier/tickets/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Tickets, GetId(2), true); break;
                    case "/zapier/tickets/{id}/customers/": data = RestOrganizations.GetTicketOrganizations(_command, GetId(2), true); break;
                    case "/zapier/tickets/{id}/contacts/": data = RestContacts.GetTicketContacts(_command, GetId(2), true); break;
                    // TicketActions is already sorted by DateCreated DESC nevertheless I added a Zapier accesspoint for consistency.
                    case "/zapier/tickets/{id}/actions/": data = RestActions.GetActions(_command, GetId(2), zapierLimit); break;
                    case "/zapier/tickets/{id}/actions/{id}/attachments/": data = RestAttachments.GetAttachments(_command, GetId(4), true); break;
                    case "/zapier/customers/": data = RestOrganizations.GetOrganizations(_command, true, zapierLimit); break;
                    case "/zapier/customers/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Organizations, GetId(2), true); break;
                    case "/zapier/customers/{id}/tickets/": data = RestTickets.GetTicketsByCustomerID(_command, GetId(2), true); break;
                    case "/zapier/customers/{id}/phonenumbers/": data = RestPhoneNumbers.GetPhoneNumbers(_command, ReferenceType.Organizations, GetId(2), true); break;
                    case "/zapier/customers/{id}/addresses/": data = RestAddresses.GetAddresses(_command, ReferenceType.Organizations, GetId(2), true); break;
                    case "/zapier/customers/{id}/notes/": data = RestNotes.GetNotes(_command, ReferenceType.Organizations, GetId(2), true); break;
                    case "/zapier/customers/{id}/contacts/": data = RestContacts.GetItems(_command, GetId(2), true); break;
                    // I did not declared a trigger for customer-contact-addresses and customer-contact-phonenumbers in zapier because can be done with contact-addresses
                    // and contact-phonenumbers respectively
                    case "/zapier/customers/{id}/contacts/{id}/addresses/": data = RestAddresses.GetAddresses(_command, ReferenceType.Users, GetId(4), true); break;
                    case "/zapier/customers/{id}/contacts/{id}/phonenumbers/": data = RestPhoneNumbers.GetPhoneNumbers(_command, ReferenceType.Users, GetId(4), true); break;

                    case "/zapier/customers/{id}/products/": data = RestOrganizationProducts.GetOrganizationProducts(_command, GetId(2), true); break;

                    // I did not declared a trigger for customer-product and customer-product-versions in zapier because can be done with customer-products
                    // and product-versions respectively.
                    case "/zapier/customers/{id}/products/{id}/": data = RestOrganizationProducts.GetOrganizationProductItems(_command, GetId(2), GetId(4), true); break;
                    case "/zapier/customers/{id}/products/{id}/versions/": data = RestVersions.GetOrganizationVersions(_command, GetId(4), GetId(2), true); break;

                    case "/zapier/contacts/": data = RestContacts.GetItems(_command, true, zapierLimit); break;
                    case "/zapier/contacts/{id}/tickets/": data = RestTickets.GetTicketsByContactID(_command, GetId(2), true); break;
                    case "/zapier/contacts/{id}/addresses/": data = RestAddresses.GetAddresses(_command, ReferenceType.Users, GetId(2), true); break;
                    case "/zapier/contacts/{id}/phonenumbers/": data = RestPhoneNumbers.GetPhoneNumbers(_command, ReferenceType.Users, GetId(2), true); break;
                    case "/zapier/products/": data = RestProducts.GetProducts(_command, true); break;
                    case "/zapier/products/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Products, GetId(2), true); break;
                    // ProductCustomers do a SELECT DISTINCT Organizations and cannot be sorted by OrganizationProducts.DateCreated DESC.
                    //case "/zapier/products/{id}/customers/": data = RestOrganizations.GetProductOrganizations(_command, GetId(2), true); break;
                    case "/zapier/products/{id}/versions/": data = RestVersions.GetItems(_command, GetId(2), true); break;
                    case "/zapier/products/{id}/versions/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.ProductVersions, GetId(4), true); break;
                    case "/zapier/products/{id}/versions/{id}/customers/": data = RestOrganizations.GetVersionOrganizations(_command, GetId(4), true); break;
                    case "/zapier/groups/": data = RestGroups.GetGroups(_command, true); break;
                    case "/zapier/users/": data = RestUsers.GetUsers(_command, true); break;
                    case "/zapier/users/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Users, GetId(2), true); break;
                    case "/zapier/wiki/": data = RestWikiArticles.GetWikiArticles(_command, true, zapierLimit); break;
                    case "/zapier/properties/actiontypes/": data = RestProperties.GetActionTypes(_command, true); break;
                    case "/zapier/properties/phonetypes/": data = RestProperties.GetPhoneTypes(_command, true); break;
                    case "/zapier/properties/productversionstatuses/": data = RestProperties.GetProductVersionStatuses(_command, true); break;
                    case "/zapier/properties/ticketseverities/": data = RestProperties.GetTicketSeverities(_command, true); break;
                    case "/zapier/properties/ticketstatuses/": data = RestProperties.GetTicketStatuses(_command, true); break;
                    case "/zapier/properties/tickettypes/": data = RestProperties.GetTicketTypes(_command, true); break;
                    case "/apiversion/": data = string.Format("<Api><AssemblyDate>{0}</AssemblyDate></Api>", File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString()); break;

                    default: throw new RestException(HttpStatusCode.NotFound);
                }
            }
            else
            {
                switch (uriTemplate)
                {
                    case "/tickets/": data = RestTickets.GetCustomerTickets(_command); break;
                    case "/tickets/{id}/": data = RestTickets.GetCustomerTicket(_command, GetId(1)); break;
                    //case "/tickets/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Tickets, GetId(1)); break;
                    //case "/tickets/{id}/customers/": data = RestOrganizations.GetTicketOrganizations(_command, GetId(1)); break;
                    //case "/tickets/{id}/customers/{id}/": data = RestOrganizations.GetOrganization(_command, GetId(3)); break;
                    //case "/tickets/{id}/contacts/": data = RestContacts.GetTicketContacts(_command, GetId(1)); break;
                    //case "/tickets/{id}/contacts/{id}/": data = RestContacts.GetItem(_command, GetId(3)); break;
                    case "/tickets/{id}/actions/": data = RestActions.GetCustomerActions(_command, GetId(1)); break;
                    case "/tickets/{id}/actions/{id}/": data = RestActions.GetCustomerAction(_command, GetId(3)); break;
                    //case "/customers/": data = RestOrganizations.GetOrganizations(_command); break;
                    //case "/customers/{id}/": data = RestOrganizations.GetOrganization(_command, GetId(1)); break;
                    //case "/customers/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Organizations, GetId(1)); break;
                    //case "/customers/{id}/tickets/": data = RestTickets.GetTicketsByCustomerID(_command, GetId(1)); break;
                    //case "/customers/{id}/phonenumbers/": data = RestPhoneNumbers.GetPhoneNumbers(_command, ReferenceType.Organizations, GetId(1)); break;
                    //case "/customers/{id}/phonenumbers/{id}/": data = RestPhoneNumbers.GetPhoneNumber(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
                    //case "/customers/{id}/addresses/": data = RestAddresses.GetAddresses(_command, ReferenceType.Organizations, GetId(1)); break;
                    //case "/customers/{id}/addresses/{id}/": data = RestAddresses.GetAddress(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
                    //case "/customers/{id}/notes/": data = RestNotes.GetNotes(_command, ReferenceType.Organizations, GetId(1)); break;
                    //case "/customers/{id}/notes/{id}/": data = RestNotes.GetNote(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
                    //case "/customers/{id}/contacts/": data = RestContacts.GetItems(_command, GetId(1)); break;
                    //case "/customers/{id}/contacts/{id}/": data = RestContacts.GetItem(_command, GetId(3)); break;
                    //case "/customers/{id}/contacts/{id}/addresses/": data = RestAddresses.GetAddresses(_command, ReferenceType.Users, GetId(3)); break;
                    //case "/customers/{id}/contacts/{id}/addresses/{id}/": data = RestAddresses.GetAddress(_command, ReferenceType.Users, GetId(3), GetId(5)); break;
                    //case "/customers/{id}/contacts/{id}/phonenumbers/": data = RestPhoneNumbers.GetPhoneNumbers(_command, ReferenceType.Users, GetId(3)); break;
                    //case "/customers/{id}/contacts/{id}/phonenumbers/{id}/": data = RestPhoneNumbers.GetPhoneNumber(_command, ReferenceType.Users, GetId(3), GetId(5)); break;
                    //case "/customers/{id}/products/": data = RestProducts.GetOrganizationProducts(_command, GetId(1)); break;
                    //case "/customers/{id}/products/{id}/": data = RestProducts.GetProduct(_command, GetId(3)); break;
                    //case "/customers/{id}/products/{id}/versions/": data = RestVersions.GetOrganizationVersions(_command, GetId(3), GetId(1)); break;
                    //case "/customers/{id}/products/{id}/versions/{id}/": data = RestVersions.GetItem(_command, GetId(5)); break;
                    case "/contacts/": data = RestContacts.GetItems(_command, false, null, true); break;
                    //case "/contacts/{id}/": data = RestContacts.GetItem(_command, GetId(1)); break;
                    //case "/contacts/{id}/tickets/": data = RestTickets.GetTicketsByContactID(_command, GetId(1)); break;
                    //case "/contacts/{id}/addresses/": data = RestAddresses.GetAddresses(_command, ReferenceType.Users, GetId(1)); break;
                    //case "/contacts/{id}/addresses/{id}/": data = RestAddresses.GetAddress(_command, ReferenceType.Users, GetId(1), GetId(3)); break;
                    //case "/contacts/{id}/phonenumbers/": data = RestPhoneNumbers.GetPhoneNumbers(_command, ReferenceType.Users, GetId(1)); break;
                    //case "/contacts/{id}/phonenumbers/{id}/": data = RestPhoneNumbers.GetPhoneNumber(_command, ReferenceType.Users, GetId(1), GetId(3)); break;
                    case "/products/": data = RestProducts.GetProducts(_command); break;
                    case "/products/{id}/": data = RestProducts.GetProduct(_command, GetId(1)); break;
                    //case "/products/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Products, GetId(1)); break;
                    //case "/products/{id}/customers/": data = RestOrganizations.GetProductOrganizations(_command, GetId(1)); break;
                    //case "/products/{id}/customers/{id}/": data = RestOrganizations.GetOrganization(_command, GetId(3)); break;
                    case "/products/{id}/versions/": data = RestVersions.GetItems(_command, GetId(1)); break;
                    case "/products/{id}/versions/{id}/": data = RestVersions.GetItem(_command, GetId(3)); break;
                    //case "/products/{id}/versions/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.ProductVersions, GetId(3)); break;
                    //case "/products/{id}/versions/{id}/customers/": data = RestOrganizations.GetVersionOrganizations(_command, GetId(3)); break;
                    //case "/products/{id}/versions/{id}/customers/{id}/": data = RestOrganizations.GetOrganization(_command, GetId(5)); break;
                    //case "/groups/": data = RestGroups.GetGroups(_command); break;
                    //case "/groups/{id}/": data = RestGroups.GetGroup(_command, GetId(1)); break;
                    //case "/users/": data = RestUsers.GetUsers(_command); break;
                    //case "/users/{id}/": data = RestUsers.GetUser(_command, GetId(1)); break;
                    //case "/users/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Users, GetId(1)); break;
                    //case "/wiki/": data = RestWikiArticles.GetWikiArticles(_command); break;
                    //case "/wiki/{id}/": data = RestWikiArticles.GetWikiArticle(_command, GetId(1)); break;
                    case "/properties/actiontypes/": data = RestProperties.GetActionTypes(_command); break;
                    case "/properties/phonetypes/": data = RestProperties.GetPhoneTypes(_command); break;
                    case "/properties/productversionstatuses/": data = RestProperties.GetProductVersionStatuses(_command); break;
                    case "/properties/ticketseverities/": data = RestProperties.GetTicketSeverities(_command); break;
                    case "/properties/ticketstatuses/": data = RestProperties.GetTicketStatuses(_command); break;
                    case "/properties/tickettypes/": data = RestProperties.GetTicketTypes(_command); break;
                    default: throw new RestException(HttpStatusCode.NotFound);
                }

            }

            return data;
        }

        public string ProcessPost(string uriTemplate)
        {
            string data = "";
            _command.StatusCode = HttpStatusCode.Created;

            if (!_command.IsCustomerOnly)
            {
                switch (uriTemplate)
                {
                    case "/tickets/": data = RestTickets.CreateTicket(_command); break;
                    case "/tickets/{id}/customers/{id}/": data = RestOrganizations.AddTicketOrganization(_command, GetId(1), GetId(3)); break;
                    case "/tickets/{id}/contacts/{id}/": data = RestContacts.AddTicketContact(_command, GetId(1), GetId(3)); break;
                    case "/tickets/{id}/actions/": data = RestActions.CreateAction(_command, GetId(1)); break;
                    case "/tickets/{id}/actions/{id}/attachments/": data = RestAttachments.CreateAttachment(_command, GetId(1), GetId(3)); break;
                    case "/tickets/{id}/subscribe/{id}/": data = RestSubscriptions.SubscribeToTicket(_command, GetId(1), GetId(3)); break;
                    case "/tickets/{id}/unsubscribe/{id}/": data = RestSubscriptions.UnSubscribeFromTicket(_command, GetId(1), GetId(3)); break;
                    case "/tickets/assignmenthistory/": data = RestTickets.GetTicketAssignments(_command, isPost: true); break;//vv
                    case "/customers/": data = RestOrganizations.CreateOrganization(_command); break;
                    case "/customers/{id}/phonenumbers/": data = RestPhoneNumbers.AddPhoneNumber(_command, ReferenceType.Organizations, GetId(1)); break;
                    case "/customers/{id}/addresses/": data = RestAddresses.AddAddress(_command, ReferenceType.Organizations, GetId(1)); break;
                    case "/customers/{id}/notes/": data = RestNotes.AddNote(_command, ReferenceType.Organizations, GetId(1)); break;
                    case "/customers/{id}/contacts/": data = RestContacts.CreateContact(_command, GetId(1)); break;
                    case "/customers/{id}/products/{id}/": data = RestOrganizationProducts.CreateOrganizationProduct(_command, GetId(1), GetId(3)); break;
                    case "/contacts/": data = RestContacts.CreateContact(_command, null); break;
                    case "/contacts/{id}/addresses/": data = RestAddresses.AddAddress(_command, ReferenceType.Users, GetId(1)); break;
                    case "/contacts/{id}/phonenumbers/": data = RestPhoneNumbers.AddPhoneNumber(_command, ReferenceType.Users, GetId(1)); break;
                    case "/products/": data = RestProducts.CreateProduct(_command); break;
                    case "/products/{id}/versions/": data = RestVersions.CreateVersion(_command, GetId(1)); break;
                    case "/assets/": data = RestAssets.CreateAsset(_command); break;
                    case "/assets/{id}/assignments/": data = RestAssetAssignments.AddAssetAssignment(_command, GetId(1)); break;
                    case "/assets/{id}/tickets/{id}/": data = RestAssets.AddTicketAsset(_command, GetId(1), GetId(3)); break;
                    case "/assets/{id}/attachments/": data = RestAttachments.CreateAttachment(_command, GetId(1)); break;
                    case "/users/": data = RestUsers.CreateUser(_command); break;
                    default: throw new RestException(HttpStatusCode.NotFound);
                }
            }
            else
            {
                switch (uriTemplate)
                {
                    case "/tickets/": data = RestTickets.CreateCustomerTicket(_command); break;
                    //case "/tickets/{id}/customers/{id}/": data = RestOrganizations.AddTicketOrganization(_command, GetId(1), GetId(3)); break;
                    //case "/tickets/{id}/contacts/{id}/": data = RestContacts.AddTicketContact(_command, GetId(1), GetId(3)); break;
                    case "/tickets/{id}/actions/": data = RestActions.CreateCustomerAction(_command, GetId(1)); break;
                    //case "/customers/": data = RestOrganizations.CreateOrganization(_command); break;
                    //case "/customers/{id}/phonenumbers/": data = RestPhoneNumbers.AddPhoneNumber(_command, ReferenceType.Organizations, GetId(1)); break;
                    //case "/customers/{id}/addresses/": data = RestAddresses.AddAddress(_command, ReferenceType.Organizations, GetId(1)); break;
                    //case "/customers/{id}/notes/": data = RestNotes.AddNote(_command, ReferenceType.Organizations, GetId(1)); break;
                    //case "/customers/{id}/contacts/": data = RestContacts.CreateContact(_command, GetId(1)); break;
                    //case "/contacts/{id}/addresses/": data = RestAddresses.AddAddress(_command, ReferenceType.Users, GetId(1)); break;
                    //case "/contacts/{id}/phonenumbers/": data = RestPhoneNumbers.AddPhoneNumber(_command, ReferenceType.Users, GetId(1)); break;
                    //case "/products/": data = RestProducts.CreateProduct(_command); break;
                    //case "/products/{id}/versions/": data = RestVersions.CreateVersion(_command, GetId(1)); break;
                    //case "/users/": data = RestUsers.CreateUser(_command); break;
                    default: throw new RestException(HttpStatusCode.NotFound);
                }
            }
            return data;

        }

        public string ProcessPut(string uriTemplate)
        {
            string data = "";
            _command.StatusCode = HttpStatusCode.Accepted;
            if (!_command.IsCustomerOnly)
            {
                switch (uriTemplate)
                {
                    case "/tickets/{id}/": data = RestTickets.UpdateTicket(_command, GetId(1)); break;
                    case "/tickets/{id}/actions/{id}/": data = RestActions.UpdateAction(_command, GetId(3)); break;
                    case "/customers/{id}/": data = RestOrganizations.UpdateOrganization(_command, GetId(1)); break;
                    case "/customers/{id}/contacts/{id}/": data = RestContacts.UpdateContact(_command, GetId(3)); break;
                    case "/customers/{id}/phonenumbers/{id}/": RestPhoneNumbers.UpdatePhoneNumber(_command, GetId(3)); break;
                    case "/customers/{id}/addresses/{id}/": RestAddresses.UpdateAddress(_command, GetId(3)); break;
                    case "/customers/{id}/products/{id}/customerproduct/{id}/": data = RestOrganizationProducts.UpdateOrganizationProductItem(_command, GetId(1), GetId(3), GetId(5)); break;
                    case "/contacts/{id}/": data = RestContacts.UpdateContact(_command, GetId(1)); break;
                    case "/contacts/{id}/phonenumbers/{id}/": RestPhoneNumbers.UpdatePhoneNumber(_command, GetId(3)); break;
                    case "/contacts/{id}/addresses/{id}/": RestAddresses.UpdateAddress(_command, GetId(3)); break;
                    case "/assets/{id}/": data = RestAssets.UpdateAsset(_command, GetId(1)); break;
                    case "/users/{id}/": data = RestUsers.UpdateUser(_command, GetId(1)); break;
                    case "/products/{id}/": data = RestProducts.UpdateProduct(_command, GetId(1)); break;
                    case "/products/{id}/versions/{id}/": data = RestVersions.UpdateVersion(_command, GetId(3)); break;
                    default: throw new RestException(HttpStatusCode.NotFound);
                }
            }
            else
            {
                switch (uriTemplate)
                {
                    case "/tickets/{id}/": data = RestTickets.UpdateCustomerTicket(_command, GetId(1)); break;
                    case "/tickets/{id}/actions/{id}/": data = RestActions.UpdateCustomerAction(_command, GetId(3)); break;
                    //case "/customers/{id}/": data = RestOrganizations.UpdateOrganization(_command, GetId(1)); break;
                    //case "/customers/{id}/contacts/{id}/": data = RestContacts.UpdateContact(_command, GetId(3)); break;
                    //case "/customers/{id}/phonenumbers/{id}/": RestPhoneNumbers.UpdatePhoneNumber(_command, GetId(3)); break;
                    //case "/customers/{id}/addresses/{id}/": RestAddresses.UpdateAddress(_command, GetId(3)); break;
                    //case "/contacts/{id}/": data = RestContacts.UpdateContact(_command, GetId(1)); break;
                    //case "/contacts/{id}/phonenumbers/{id}/": RestPhoneNumbers.UpdatePhoneNumber(_command, GetId(3)); break;
                    //case "/contacts/{id}/addresses/{id}/": RestAddresses.UpdateAddress(_command, GetId(3)); break;
                    //case "/users/{id}/": data = RestUsers.UpdateUser(_command, GetId(1)); break;
                    //case "/products/{id}/": data = RestProducts.UpdateProduct(_command, GetId(1)); break;
                    //case "/products/{id}/versions/{id}/": data = RestVersions.UpdateVersion(_command, GetId(3)); break;
                    default: throw new RestException(HttpStatusCode.NotFound);
                }
            }
            return data;

        }

        public string ProcessDelete(string uriTemplate)
        {
            string data = "";
            if (!_command.IsCustomerOnly)
            {
                switch (uriTemplate)
                {
                    case "/tickets/{id}/": RestTickets.DeleteTicket(_command, GetId(1)); break;
                    case "/tickets/{id}/customers/{id}/": RestOrganizations.RemoveTicketOrganization(_command, GetId(1), GetId(3)); break;
                    case "/tickets/{id}/contacts/{id}/": RestContacts.RemoveTicketContact(_command, GetId(1), GetId(3)); break;
                    case "/customers/{id}/phonenumbers/{id}/": RestPhoneNumbers.RemovePhoneNumber(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
                    case "/customers/{id}/addresses/{id}/": RestAddresses.RemoveAddress(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
                    case "/customers/{id}/notes/{id}/": RestNotes.RemoveNote(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
                    //Commented in ticket 8867.
                    //case "/customers/{id}/products/{id}/": data = RestProducts.RemoveOrganizationProduct(_command, GetId(1), GetId(3)); break;
                    case "/customers/{id}/products/{id}/customerproduct/{id}/": data = RestOrganizationProducts.DeleteOrganizationProductItem(_command, GetId(1), GetId(3), GetId(5)); break;
					case "/customers/{id}/": throw new RestException(HttpStatusCode.Forbidden, "Deletion at the customer level is temporarily disabled. We apologize for any inconvenience."); //RestOrganizations.DeleteOrganization(_command, GetId(1)); break;
					case "/contacts/{id}/": RestContacts.DeleteContact(_command, GetId(1)); break;
                    case "/contacts/{id}/phonenumbers/{id}/": RestPhoneNumbers.RemovePhoneNumber(_command, ReferenceType.Users, GetId(1), GetId(3)); break;
                    case "/contacts/{id}/addresses/{id}/": RestAddresses.RemoveAddress(_command, ReferenceType.Users, GetId(1), GetId(3)); break;
                    case "/assets/{id}/": data = RestAssets.JunkAsset(_command, GetId(1)); break;
                    case "/assets/{id}/assignments/": data = RestAssetAssignments.ReturnAsset(_command, GetId(1)); break;
                    case "/assets/{id}/tickets/{id}/": data = RestAssets.DeleteTicketAsset(_command, GetId(1), GetId(3)); break;
                    case "/assets/{id}/attachments/{id}/":
                        {
                            int attachmentID = GetId(3);
                            ModelAPI.AttachmentAPI.DeleteAttachment(attachmentID, AttachmentProxy.References.Assets, GetId(1));
                            //data = RestAttachments.DeleteAttachment(_command, GetId(1), GetId(3));
                            data = RestAttachments.GetAttachmentsAsXML(_command, attachmentID);
                        }
                        break;
                    case "/users/{id}/": RestUsers.DeleteUser(_command, GetId(1)); break;
                    case "/products/{id}/": data = RestProducts.DeleteProduct(_command, GetId(1)); break;
                    case "/products/{id}/versions/{id}/": data = RestVersions.DeleteVersion(_command, GetId(3)); break;
                    default: throw new RestException(HttpStatusCode.NotFound);
                }
            }
            return data;

        }

        private int GetId(int index)
        {
            return int.Parse(_command.Segments[index]);
        }

        private string GetURITemplate(List<string> segments)
        {
            StringBuilder builder = new StringBuilder("/");

            foreach (string s in segments)
            {
                int id = -1;
                if (!int.TryParse(s, out id))
                {
                    builder.Append(s + "/");
                }
                else
                {
                    builder.Append("{id}/");
                }
            }

            return builder.ToString();

        }

        private string FixJsonFormatError(string input, string uriTemplate, bool isPaging = false)
        {
            int squareBracketIndex = 0;
            string objectIDLabel = string.Empty;
            string stringToRemove = GetStringToRemove(uriTemplate, input, ref squareBracketIndex, ref objectIDLabel);

            if (stringToRemove != string.Empty && input.Length > squareBracketIndex)
            {
                if (!isPaging)
                {

                    if (input.Substring(squareBracketIndex, 1) != "[")
                    {
                        input = input.Replace(stringToRemove, "[");
                        input = input.Remove(input.Length - 2);
                    }
                    else
                    {
                        input = input.Replace(stringToRemove, string.Empty);
                        input = input.Remove(input.Length - 3);
                    }

                    input = input + "]}";
                }
                else
                {
                    input = input.Replace(stringToRemove, string.Empty);
                    input = input.Substring(input.Length - 1, 1) == "}" ? input.Substring(0, input.Length - 1) : input;
                }

                input = input.Replace(objectIDLabel, "ID");
            }

            return input;
        }

        private string GetStringToRemove(string uriTemplate, string input, ref int squareBracketIndex, ref string objectIDLabel)
        {
            string result = string.Empty;
            switch (uriTemplate)
            {
                case "/zapier/tickets/{id}/actions/":
                case "/tickets/{id}/actions/":
                    result = "{\"Action\":";
                    squareBracketIndex = "{\"Actions\":{\"Action\":".Length;
                    objectIDLabel = "ActionID";
                    break;
                case "/zapier/properties/actiontypes/":
                case "/properties/actiontypes/":
                    result = "{\"ActionType\":";
                    squareBracketIndex = "{\"ActionTypes\":{\"ActionType\":".Length;
                    objectIDLabel = "ActionTypeID";
                    break;
                case "/zapier/customers/{id}/addresses/":
                case "/zapier/customers/{id}/contacts/{id}/addresses/":
                case "/zapier/contacts/{id}/addresses/":
                case "/customers/{id}/addresses/":
                case "/customers/{id}/contacts/{id}/addresses/":
                case "/contacts/{id}/addresses/":
                    result = "{\"Address\":";
                    squareBracketIndex = "{\"Addresses\":{\"Address\":".Length;
                    objectIDLabel = "AddressID";
                    break;
                case "/zapier/tickets/{id}/actions/{id}/attachments/":
                case "/tickets/{id}/actions/{id}/attachments/":
                    result = "{\"Attachment\":";
                    squareBracketIndex = "{\"Attachments\":{\"Attachment\":".Length;
                    objectIDLabel = "AttachmentID";
                    break;
                case "/zapier/tickets/{id}/contacts/":
                case "/zapier/customers/{id}/contacts/":
                case "/zapier/contacts/":
                case "/tickets/{id}/contacts/":
                case "/customers/{id}/contacts/":
                case "/contacts/":
                    result = "{\"Contact\":";
                    squareBracketIndex = "{\"Contacts\":{\"Contact\":".Length;
                    objectIDLabel = "ContactID";
                    break;
                case "/zapier/tickets/{id}/customers/":
                case "/zapier/products/{id}/versions/{id}/customers/":
                case "/zapier/customers/":
                case "/tickets/{id}/customers/":
                case "/products/{id}/versions/{id}/customers/":
                case "/customers/":
                    result = "{\"Customer\":";
                    squareBracketIndex = "{\"Customers\":{\"Customer\":".Length;
                    objectIDLabel = "OrganizationID";
                    break;
                case "/zapier/groups/":
                case "/groups/":
                    result = "{\"Group\":";
                    squareBracketIndex = "{\"Groups\":{\"Group\":".Length;
                    objectIDLabel = "GroupID";
                    break;
                case "/zapier/tickets/{id}/history/":
                case "/zapier/customers/{id}/history/":
                case "/zapier/products/{id}/history/":
                case "/zapier/products/{id}/versions/{id}/history/":
                case "/zapier/users/{id}/history/":
                case "/tickets/{id}/history/":
                case "/customers/{id}/history/":
                case "/products/{id}/history/":
                case "/products/{id}/versions/{id}/history/":
                case "/users/{id}/history/":
                    result = "{\"ActionItem\":";
                    squareBracketIndex = "{\"History\":{\"ActionItem\":".Length;
                    objectIDLabel = "ActionLogID";
                    break;
                case "/zapier/customers/{id}/notes/":
                case "/customers/{id}/notes/":
                    result = "{\"Note\":";
                    squareBracketIndex = "{\"Notes\":{\"Note\":".Length;
                    objectIDLabel = "NoteID";
                    break;
                case "/zapier/customers/{id}/products/":
                case "/zapier/customers/{id}/products/{id}/":
                case "/customers/{id}/products/":
                case "/customers/{id}/products/{id}/":
                    result = "{\"OrganizationProduct\":";
                    squareBracketIndex = "{\"OrganizationProducts\":{\"OrganizationProduct\":".Length;
                    objectIDLabel = "OrganizationProductID";
                    break;
                case "/zapier/customers/{id}/phonenumbers/":
                case "/zapier/customers/{id}/contacts/{id}/phonenumbers/":
                case "/zapier/contacts/{id}/phonenumbers/":
                case "/customers/{id}/phonenumbers/":
                case "/customers/{id}/contacts/{id}/phonenumbers/":
                case "/contacts/{id}/phonenumbers/":
                    result = "{\"PhoneNumber\":";
                    squareBracketIndex = "{\"PhoneNumbers\":{\"PhoneNumber\":".Length;
                    objectIDLabel = "PhoneID";
                    break;
                case "/zapier/properties/phonetypes/":
                case "/properties/phonetypes/":
                    result = "{\"PhoneType\":";
                    squareBracketIndex = "{\"PhoneTypes\":{\"PhoneType\":".Length;
                    objectIDLabel = "PhoneTypeID";
                    break;
                case "/zapier/products/":
                case "/products/":
                    result = "{\"Product\":";
                    squareBracketIndex = "{\"Products\":{\"Product\":".Length;
                    objectIDLabel = "ProductID";
                    break;
                case "/zapier/properties/productversionstatuses/":
                case "/properties/productversionstatuses/":
                    result = "{\"ProductVersionStatus\":";
                    squareBracketIndex = "{\"ProductVersionStatuses\":{\"ProductVersionStatus\":".Length;
                    objectIDLabel = "ProductVersionStatusID";
                    break;
                case "/zapier/tickets/":
                case "/zapier/customers/{id}/tickets/":
                case "/zapier/contacts/{id}/tickets/":
                case "/tickets/":
                case "/customers/{id}/tickets/":
                case "/contacts/{id}/tickets/":
                    result = "{\"Ticket\":";
                    squareBracketIndex = "{\"Tickets\":{\"Ticket\":".Length;
                    objectIDLabel = "TicketID";
                    break;
                case "/zapier/properties/ticketseverities/":
                case "/properties/ticketseverities/":
                    result = "{\"TicketSeverity\":";
                    squareBracketIndex = "{\"TicketSeverities\":{\"TicketSeverity\":".Length;
                    objectIDLabel = "TicketSeverityID";
                    break;
                case "/zapier/properties/ticketstatuses/":
                case "/properties/ticketstatuses/":
                    result = "{\"TicketStatus\":";
                    squareBracketIndex = "{\"TicketStatuses\":{\"TicketStatus\":".Length;
                    objectIDLabel = "TicketStatusID";
                    break;
                case "/zapier/properties/tickettypes/":
                case "/properties/tickettypes/":
                    result = "{\"TicketType\":";
                    squareBracketIndex = "{\"TicketTypes\":{\"TicketType\":".Length;
                    objectIDLabel = "TicketTypeID";
                    break;
                case "/properties/knowledgebasecategories/":
                    result = "{\"KnowledgeBaseCategory\":";
                    squareBracketIndex = "{\"KnowledgeBaseCategories\":{\"KnowledgeBaseCategory\":".Length;
                    objectIDLabel = "CategoryID";
                    break;
                case "/zapier/users/":
                case "/users/":
                    result = "{\"User\":";
                    squareBracketIndex = "{\"Users\":{\"User\":".Length;
                    objectIDLabel = "UserID";
                    break;
                case "/zapier/customers/{id}/products/{id}/versions/":
                case "/zapier/products/{id}/versions/":
                case "/customers/{id}/products/{id}/versions/":
                case "/products/{id}/versions/":
                    result = "{\"Version\":";
                    squareBracketIndex = "{\"Versions\":{\"Version\":".Length;
                    objectIDLabel = "ProductVersionID";
                    break;
                case "/zapier/wiki/":
                case "/wiki/":
                    result = "{\"Article\":";
                    squareBracketIndex = "{\"Wiki\":{\"Article\":".Length;
                    objectIDLabel = "ArticleID";
                    break;
            }
            return result;
        }
    }
}
