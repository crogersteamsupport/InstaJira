using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using TeamSupport.Data;
using System.Net;

namespace TeamSupport.Api
{

    public class RestDeletedTickets
    {
        public static string GetDeletedTicket(RestCommand command, int iD)
        {
            DeletedTicket deletedTicket = DeletedTickets.GetDeletedTicket(command.LoginUser, iD);
            if (deletedTicket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
            return deletedTicket.GetXml("DeletedTicket", true);
        }

        public static string GetDeletedTickets(RestCommand command)
        {
            string xml = "";
            bool hasBeenFiltered = false;
            int totalRecords = 0;

            DeletedTickets deletedTickets = new DeletedTickets(command.LoginUser);
            if (command.IsPaging)
            {
                try
                {
                    deletedTickets.LoadByOrganizationID(command.Organization.OrganizationID, command.Filters, command.PageNumber, command.PageSize);
                    hasBeenFiltered = true;
                    XmlTextWriter writer = DeletedTickets.BeginXmlWrite("DeletedTicketx");
                    try
                    {
                        foreach (DataRow row in deletedTickets.Table.Rows)
                        {
                            int recordId = (int)row["ID"];
                            Tags tags = new Tags(command.LoginUser);
                            tags.LoadByReference(ReferenceType.DeletedTickets, recordId);
                            tags = tags ?? new Tags(command.LoginUser);
                            deletedTickets.WriteXml(writer, row, "DeletedTicket", true, !hasBeenFiltered ? command.Filters : new System.Collections.Specialized.NameValueCollection(), tags);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogs.LogException(command.LoginUser, ex, "API", "RestDeletedTickets. GetDeletedTickets(). Paging.");
                    }

                    if (deletedTickets.Count > 0)
                    {
                        totalRecords = deletedTickets[0].TotalRecords;
                    }

                    writer.WriteElementString("TotalRecords", totalRecords.ToString());
                    xml = DeletedTickets.EndXmlWrite(writer);
                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(command.LoginUser, ex, "API", "RestDeletedTickets. GetDeletedTickets(). Paging. SQL filtering generation failed.");
                    throw new RestException(HttpStatusCode.InternalServerError, "There was an error processing your request. Please contact TeamSupport.com", ex);
                }
            }
            else
            {
                //No Paging
                try
                {
                    deletedTickets.LoadByOrganizationID(command.Organization.OrganizationID, command.Filters);
                    xml = deletedTickets.GetXml("DeletedTickets", "DeletedTicket", true, command.Filters);
                    //xml = AddTagsToDeletedTickets(xml, command);
                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(command.LoginUser, ex, "API", "RestDeletedTickets. GetDeletedTickets(). No Paging. SQL filtering generation failed.");
                }
            }

            return xml;
        }

        /// <summary>
        /// Add the Tag tags to the final xml of the deleted tickets
        /// </summary>
        /// <param name="deletedTicketsXml">Deleted Tickets xml string.</param>
        /// <returns>An xml string with the Deleted Tickets data and its tags.</returns>
        private static string AddTagsToDeletedTickets(string deletedTicketsXml, RestCommand command, bool isCustomerToken = false)
        {
            string xml = string.Empty;
            //Add the tag nodes to each deleted ticket object
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;
            xmlDoc.LoadXml(deletedTicketsXml);

            //Do we have paging?
            XmlNode totalRecordsElement = xmlDoc.SelectSingleNode("/DeletedTickets/TotalRecords");

            XmlNodeList nodeList = xmlDoc.SelectNodes("/DeletedTickets/DeletedTicket");
            string xmlResultString = string.Empty;

            using (var xmlTextWriter = Tickets.BeginXmlWrite("DeletedTickets"))
            {
                if (totalRecordsElement != null)
                {
                    totalRecordsElement.WriteTo(xmlTextWriter);
                    totalRecordsElement = null;
                }

                foreach (XmlNode node in nodeList)
                {
                    //get the tags for the deleted ticket using the Id
                    try
                    {
                        string recordIdString = node["ID"].InnerText;
                        Tags tags = new Tags(command.LoginUser);
                        tags.LoadByReference(ReferenceType.DeletedTickets, int.Parse(recordIdString), isCustomerToken ? command.Organization.ParentID : null);
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

                xml = DeletedTickets.EndXmlWrite(xmlTextWriter);
            }

            return xml;
        }
    }

}






