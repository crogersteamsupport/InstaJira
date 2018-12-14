using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using TeamSupport.Data;
using TeamSupport.IDTree;
using System.Security.Authentication;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Diagnostics;
using TeamSupport.DataAPI;

namespace TeamSupport.ModelAPI
{
    /// <summary> CRUD interface to DataAPI - Create, Read, Update, Delete proxy </summary>
    public static class Model_API
    {
        /// <summary> Find all the organizations associated with a ticket </summary>
        public static OrganizationTicketModel[] GetOrganizationTickets(int ticketID)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    TicketModel ticketModel = new TicketModel(connection, ticketID);
                    return OrganizationTicketModel.GetOrganizationTickets(ticketModel);
                }
            }
            catch (Exception ex)
            {
                Data_API.LogMessage(ActionLogType.Insert, ReferenceType.None, 0, "choke", ex);
            }
            return null;
        }

        /// <summary> 
        /// CREATE
        /// </summary>
        public static void Create<T>(T proxy)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    switch (typeof(T).Name)
                    {
                        case "ActionProxy":
                            {
                                ActionProxy actionProxy = proxy as ActionProxy;
                                Data_API.Create(connection.Ticket(actionProxy.TicketID), actionProxy);
                            }
                            break;
                        case "AttachmentProxy":
                            {
                                AttachmentProxy attachment = proxy as AttachmentProxy;
                                IAttachmentDestination model = AttachmentAPI.ClassFactory(connection, attachment);
                                Data_API.Create(model as IDNode, attachment);
                            }
                            break;
                        default:
                            if (Debugger.IsAttached) Debugger.Break();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO - tell user we failed to read
                Data_API.LogMessage(ActionLogType.Insert, ReferenceType.None, 0, "choke", ex);
            }
        }

        /// <summary>
        /// Read the proxy given the RefID
        /// </summary>
        public static T ReadRefTypeProxyByRefID<T>(int id) where T : class
        {
            T t = null;
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    switch(typeof(T).Name) // alphabetized list
                    {
                        case "UserPhotoAttachmentProxy":
                            t = Data_API.ReadRefTypeProxyByRefID<UserPhotoAttachmentProxy>(connection, id) as T;
                            break;
                        default:
                            if (Debugger.IsAttached) Debugger.Break();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // we use this method to check if it exists
                //int logid = Data_API.LogException(new IDTree.AuthenticationModel(), ex, "ReadByRefID Exception" + ex.Source);
            }
            return t;
        }

        /// <summary>
        /// READ - read the proxy corresponding to the ID 
        /// </summary>
        public static T Read<T>(int id) where T : class
        {
            T t = default(T);
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    switch (typeof(T).Name) // alphabetized list
                    {
                        case "ActionProxy":
                            {
                                Table<ActionProxy> table = connection._db.GetTable<ActionProxy>();
                                var q = from action in table where action.ActionID == id select action;
                                t = q.Single() as T;
                            }

                            //string query = $"SET NOCOUNT OFF; SELECT [ActionID], [ActionTypeID], [SystemActionTypeID], [Name], [TimeSpent], [DateStarted], [IsVisibleOnPortal], [IsKnowledgeBase], [ImportID], [DateCreated], [DateModified], [CreatorID], [ModifierID], [TicketID], [ActionSource], [DateModifiedBySalesForceSync], [SalesForceID], [DateModifiedByJiraSync], [JiraID], [Pinned], [Description], [IsClean], [ImportFileID] FROM [dbo].[Actions] WITH (NOLOCK) WHERE ([ActionID] = {id})";
                            //IEnumerable<ActionProxy> stuff = connection._db.ExecuteQuery<ActionProxy>(query);

                            //ActionProxy proxy = connection._db.ExecuteQuery<ActionProxy>(query).Min();

                            //t = new ActionNode(connection, id).ActionProxy() as T;
                            ////t = DataAPI.DataAPI.Read<T>(new ActionNode(connection, id));
                            break;
                        case "ActionProxy[]":
                            t = new TicketModel(connection, id).ActionProxies() as T;
                            //t = DataAPI.DataAPI.Read<T>(new TicketNode(connection, id));
                            break;
                        case "AttachmentProxy":
                            t = Data_API.ReadRefTypeProxy<AttachmentProxy>(connection, id) as T;
                            break;
                        case "AttachmentProxy[]":
                            t = Data_API.Read<T>(new ActionModel(connection, id));
                            break;
                        //case CustomValueProxy proxy:
                        //    t = Data_API.Read<T>(new TicketModel(connection, id));
                        //    break;
                        case "TicketProxy":
                            t = Data_API.Read<T>(new TicketModel(connection, id));
                            break;
                        case "UserProxy":
                            {
                                Table<UserProxy> table = connection._db.GetTable<UserProxy>();
                                var q = from user in table where user.UserID == id select user;
                                t = q.Single() as T;
                            }                            //t = DataAPI.DataAPI.Read<T>(new UserNode(connection, id));
                            break;
                        default:
                            if (Debugger.IsAttached) Debugger.Break();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                int logid = Data_API.LogException(new IDTree.AuthenticationModel(), ex, "Ticket Merge Exception:" + ex.Source);
            }
            return t;
        }

        /// <summary>
        /// UPDATE
        /// </summary>
        public static void Update<T>(T proxy)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    switch (typeof(T).Name)
                    {
                        case "ActionProxy":
                            ActionProxy actionProxy = proxy as ActionProxy;
                            //DataAPI.DataAPI.Update(new ActionNode(connection, actionProxy.ActionID), actionProxy);
                            break;
                        case "AttachmentProxy":
                            AttachmentProxy attachmentProxy = proxy as AttachmentProxy;
                            UpdateArguments args = new UpdateArguments();
                            args.Append("FileName", attachmentProxy.FileName);
                            args.Append("Path", attachmentProxy.Path);
                            args.Append("FilePathID", attachmentProxy.FilePathID.Value);
                            Data_API.Update(new AttachmentModel(connection, attachmentProxy), args);
                            break;
                        default:
                            if (Debugger.IsAttached) Debugger.Break();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO - tell user we failed to read
                //int logid = DataAPI.Data_API.LogException(connection.Authentication, ex, "Ticket Merge Exception:" + ex.Source);
                //return $"Error merging tickets. Exception #{logid}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.";
            }
        }

        /// <summary> 
        /// DELETE
        /// </summary>
        //public static void Delete<T>(T proxy)
        //{
        //    try
        //    {
        //        using (ConnectionContext connection = new ConnectionContext())
        //        {
        //            switch (typeof(T).Name)
        //            {
        //                case "ActionProxy":
        //                    ActionProxy actionProxy = proxy as ActionProxy;
        //                    Data_API.Delete(new ActionModel(connection, actionProxy.ActionID));
        //                    break;
        //                case "AttachmentProxy":
        //                    AttachmentProxy attachmentProxy = proxy as AttachmentProxy;
        //                    Data_API.Delete(new AttachmentModel(connection, attachmentProxy.AttachmentID));
        //                    break;
        //            }
        //        }
        //    }
        //    catch (AuthenticationException ex)
        //    {
        //        // TODO - tell user they don't have permission
        //        Data_API.LogMessage(ActionLogType.Delete, ReferenceType.None, 0, "choke", ex);
        //    }
        //    catch (System.Data.ConstraintException ex)
        //    {
        //        // TODO - data integrity failure
        //        Data_API.LogMessage(ActionLogType.Delete, ReferenceType.None, 0, "choke", ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        //int logid = DataAPI.Data_API.LogException(new Proxy.AuthenticationModel(authenticationTicket), ex, "Ticket Merge Exception:" + ex.Source);
        //        //return $"Error merging tickets. Exception #{logid}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.";
        //    }
        //}



        #region Tickets
        public static string MergeTickets(int destinationTicketID, int sourceTicketID)
        {

            try
            {
                using (ConnectionContext connection = new ConnectionContext(true))    // use transaction
                {
                    try
                    {
                        TicketMerge merge = new TicketMerge(connection, connection.Ticket(destinationTicketID), connection.Ticket(sourceTicketID));
                        merge.Merge1();
                        connection.Commit();
                        return String.Empty;
                    }
                    catch (Exception ex)
                    {
                        connection.Rollback();
                        int logid = Data_API.LogException(connection.Authentication, ex, "Ticket Merge Exception:" + ex.Source);
                        return $"Error merging tickets. Exception #{logid}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.";
                    }
                }
            }
            catch (Exception ex)
            {
                int logid = Data_API.LogException(new IDTree.AuthenticationModel(), ex, "Ticket Merge Exception:" + ex.Source);
                return $"Error merging tickets. Exception #{logid}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.";
            }
        }
        #endregion


        #region ActionAttachments


        /// <summary>
        /// Get a list of all the action attachments for this ticket - include only the latest for each filename
        /// </summary>
        public static void ReadActionAttachmentsForTicket(int ticketID, ActionAttachmentsByTicketID ticketActionAttachments, out AttachmentProxy[] attachments)
        {
            attachments = null;
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    TicketModel ticketModel = connection.Ticket(ticketID);
                    AttachmentAPI.ReadActionAttachmentsForTicket(ticketModel, ticketActionAttachments, out attachments);
                }
            }
            catch (Exception ex)
            {
                Data_API.LogMessage(ActionLogType.Delete, ReferenceType.Attachments, ticketID, "failed to read action attachments", ex);
            }
        }


        #endregion

    }
}
