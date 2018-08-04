﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using TeamSupport.Proxy;

namespace TeamSupport.Model
{
    /// <summary>
    /// The logical model (TeamSupport.Model) assumes correctness and throws exceptions when it is not.
    /// </summary>
    public static class ModelAPI
    {
        /// <summary> Save Action Attachments - Save the uploaded files and insert an action attachment record </summary>
        public static void SaveActionAttachments(FormsAuthenticationTicket authentication, HttpContext context, int? ticketID, int? actionID)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authentication))
                {
                    if (!ticketID.HasValue)
                        ticketID = DataAPI.DataAPI.ActionGetTicketID(connection._db, actionID.Value);
                    List<ActionAttachment> attachments = connection.Ticket(ticketID.Value).Action(actionID.Value).InsertActionAttachments(context.Request);
                    DataAPI.DataAPI.LogMessage(authentication, Data.ActionLogType.Insert, Data.ReferenceType.Attachments, ticketID, "Attachments Saved");
                    //return attachments;
                }
            }
            catch (Exception ex)
            {
                ConnectionContext.LogMessage(authentication, Data.ActionLogType.Insert, Data.ReferenceType.Attachments, ticketID, "Unable to save attachments", ex);
                //return null;
            }
        }

        /// <summary> Delete Action Attachment /// </summary>
        public static void DeleteActionAttachment(FormsAuthenticationTicket authentication, int? ticketID, int? actionID, int attachmentID)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authentication))
                {
                    if (!actionID.HasValue)
                        actionID = DataAPI.DataAPI.ActionAttachmentActionID(connection._db, attachmentID);
                    if(!ticketID.HasValue)
                        ticketID = DataAPI.DataAPI.ActionGetTicketID(connection._db, actionID.Value);
                    connection.Ticket(ticketID.Value).Action(actionID.Value).Attachment(attachmentID).Delete();
                    ConnectionContext.LogMessage(authentication, Data.ActionLogType.Delete, Data.ReferenceType.Attachments, attachmentID, "Attachment deleted");
                }
            }
            catch (Exception ex)
            {
                ConnectionContext.LogMessage(authentication, Data.ActionLogType.Delete, Data.ReferenceType.Attachments, attachmentID, "Unable to delete attachments", ex);
            }
        }

        public static AttachmentProxy[] SelectActionAttachments(FormsAuthenticationTicket authentication, int? ticketID, int actionID)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authentication))
                {
                    if(!ticketID.HasValue)
                        ticketID = DataAPI.DataAPI.ActionGetTicketID(connection._db, actionID);
                    return connection.Ticket(ticketID.Value).Action(actionID).SelectAttachments();
                }
            }
            catch (Exception ex)
            {
                ConnectionContext.LogMessage(authentication, Data.ActionLogType.Delete, Data.ReferenceType.Attachments, actionID, "failed to read attachments", ex);
                return null;
            }
        }

        public static void MergeTickets(FormsAuthenticationTicket authentication, int destinationTicketID, int sourceTicketID)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authentication))
                {
                    connection.Ticket(destinationTicketID).Merge(connection.Ticket(sourceTicketID));
                }
            }
            catch (Exception ex)
            {
                ConnectionContext.LogMessage(authentication, Data.ActionLogType.Update, Data.ReferenceType.Attachments, destinationTicketID, $"failed to merge {destinationTicketID} <= {sourceTicketID}", ex);
            }
        }

        ///// <summary> Insert Action </summary>
        //public static ActionModel InsertAction(FormsAuthenticationTicket authentication, ActionProxy actionProxy)
        //{
        //    try
        //    {
        //        using (ConnectionContext connection = new ConnectionContext(authentication))
        //        {
        //            ActionModel action = connection.Ticket(actionProxy.TicketID).InsertAction(actionProxy);
        //            ConnectionContext.LogMessage(authentication, Data.ActionLogType.Insert, Data.ReferenceType.Actions, action.ActionID, "InsertAction successful");
        //            return action;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ConnectionContext.LogMessage(authentication, Data.ActionLogType.Insert, Data.ReferenceType.Actions, actionProxy.ActionID, "Unable to insert action", ex);
        //        return null;
        //    }
        //}

        ///// <summary> Insert Action on new Ticket </summary>
        //public static ActionModel InsertAction(FormsAuthenticationTicket authentication, ActionProxy proxy, Data.Ticket ticket, Data.User user)
        //{
        //    try
        //    {
        //        using (ConnectionContext connection = new ConnectionContext(authentication))
        //        {
        //            ActionModel action = connection.Ticket(ticket.TicketID).InsertAction(proxy, ticket, user);
        //            ConnectionContext.LogMessage(authentication, Data.ActionLogType.Insert, Data.ReferenceType.Actions, action.ActionID, "InsertAction successful");
        //            return action;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ConnectionContext.LogMessage(authentication, Data.ActionLogType.Insert, Data.ReferenceType.Actions, ticket.TicketID, "Unable to insert action", ex);
        //        return null;
        //    }
        //}


    }
}
