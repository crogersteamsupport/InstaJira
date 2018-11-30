using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TeamSupport.Data;
using TeamSupport.IDTree;
using System.IO;
using TeamSupport.DataAPI;
using System.Diagnostics;
using System.Data.Linq;

namespace TeamSupport.ModelAPI
{
    public class AttachmentAPI
    {
        /// <summary> Create attachment files </summary>
        public static string ImportFileUploadPath(HttpContext context)
        {
            GetPathMap(context, out PathMap pathMap, out int id, out string ratingImage);
            if (!pathMap._path.Equals("Imports"))
                return String.Empty;

            using (ConnectionContext connection = new ConnectionContext())
            {
                string path = connection.Organization.AttachmentPath + "\\Imports";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        /// <summary> Create attachment files </summary>
        public static List<AttachmentProxy> CreateAttachments(HttpContext context, out string _ratingImage)
        {
            List<AttachmentProxy> result = null;
            try
            {
                GetPathMap(context, out PathMap pathMap, out int refID, out _ratingImage);

                using (ConnectionContext connection = new ConnectionContext())
                {
                    // not really an attachment
                    HttpFileCollection files = context.Request.Files;


                    // valid ID to add attachment
                    IAttachmentDestination model = ClassFactory(connection, pathMap._refType, refID);
                    result = new List<AttachmentProxy>();
                    for (int i = 0; i < files.Count; i++)
                    {
                        if (files[i].ContentLength <= 0)
                            continue;

                        AttachmentFile attachmentFile = new AttachmentFile(model, files[i]);    // create the file
                        AttachmentProxy proxy = AttachmentProxy.ClassFactory(pathMap._refType);  // construct the proxy
                        proxy.RefID = refID;
                        InitializeProxy(context, connection, attachmentFile, proxy);
                        Data_API.Create(model as IDNode, proxy);  // save proxy to DB
                        result.Add(proxy);
                    }
                }
            }
            catch (Exception ex)
            {
                _ratingImage = null;
                // TODO - tell user we failed 
                Data_API.LogMessage(ActionLogType.Insert, ReferenceType.None, 0, "choke", ex);
            }
            return result;
        }

        public static void CreateAttachment(string savePath, int organizationId, AttachmentProxy.References refType, int userID, System.Net.HttpWebResponse httpWebResponse)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {

                    AttachmentProxy attachment = AttachmentProxy.ClassFactory(refType);
                    attachment.RefID = userID;
                    attachment.OrganizationID = organizationId;
                    attachment.FileName = Path.GetFileName(savePath);
                    attachment.Path = savePath;
                    attachment.FileType = string.IsNullOrEmpty(httpWebResponse.ContentType) ? "application/octet-stream" : httpWebResponse.ContentType;
                    attachment.FileSize = httpWebResponse.ContentLength;

                    IAttachmentDestination model = ClassFactory(connection, attachment);
                    Data_API.Create(model as IDNode, attachment);
                }
            }
            catch (Exception ex)
            {
                // TODO - tell user we failed 
                Data_API.LogMessage(ActionLogType.Insert, (ReferenceType)refType, 0, "choke", ex);
            }
        }

        public static void CreateAttachment(AttachmentProxy attachment)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    IAttachmentDestination model = ClassFactory(connection, attachment);
                    Data_API.Create(model as IDNode, attachment);
                }
            }
            catch (Exception ex)
            {
                // TODO - tell user we failed 
                Data_API.LogMessage(ActionLogType.Insert, (ReferenceType)attachment.RefType, 0, "choke", ex);
            }
        }

        public static IAttachmentDestination ClassFactory(ConnectionContext connection, AttachmentProxy proxy)
        {
            return ClassFactory(connection, proxy.RefType, proxy.RefID);
        }

        private static IAttachmentDestination ClassFactory(ConnectionContext connection, AttachmentProxy.References refType, int refID)
        {
            switch (refType)
            {
                case AttachmentProxy.References.Actions: return new ActionModel(connection, refID);
                case AttachmentProxy.References.Assets: return new AssetModel(connection, refID);
                case AttachmentProxy.References.ChatAttachments: return new ChatModel(connection, refID);

                case AttachmentProxy.References.CompanyActivity:
                    {
                        // Attachment => Note => Organization
                        int organizationID = connection._db.ExecuteQuery<int>($"Select RefID from Notes WITH (NOLOCK) WHERE NoteID = {refID}").Min();
                        return new NoteModel(new OrganizationModel(connection, organizationID), refID);
                    }
                case AttachmentProxy.References.ContactActivity:
                    {
                        // Attachment => Note => User
                        int userID = connection._db.ExecuteQuery<int>($"Select RefID from Notes WITH (NOLOCK) WHERE NoteID = {refID}").Min();
                        return new NoteModel(new UserModel(connection, userID), refID);
                    }

                //case AttachmentProxy.References.Contacts: return new ContactModel(connection, refID);
                //case AttachmentProxy.References.CustomerHubLogo: return new CustomerHubLogoModel(connection, refID);
                case AttachmentProxy.References.Organizations: return new OrganizationModel(connection, refID);
                case AttachmentProxy.References.ProductVersions: return new ProductVersionModel(connection, refID);
                case AttachmentProxy.References.Tasks: return new TaskModel(connection.Organization, refID);

                case AttachmentProxy.References.UserPhoto:
                case AttachmentProxy.References.Users:
                    return new UserModel(connection, refID);

                case AttachmentProxy.References.WaterCooler: return new WatercoolerMsgModel(connection, refID);
                default:
                    if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break();
                    return null;
            }
        }

        public static string DeleteAttachment(int attachmentID, AttachmentProxy.References verifyRefType, int? verifyRefID = null)
        {
            string result = null;
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    // validate args
                    AttachmentProxy proxy = Data_API.ReadRefTypeProxy<AttachmentProxy>(connection, attachmentID);
                    result = proxy.FileName;

                    if (verifyRefType == AttachmentProxy.References.None) // support delete no matter the type?
                        verifyRefType = proxy.RefType;

                    if(!verifyRefID.HasValue) // if no explicit refID provided, use the proxy
                        verifyRefID = proxy.RefID;

                    // type safe construction
                    IAttachmentDestination model = null;
                    switch (verifyRefType)
                    {
                        case AttachmentProxy.References.Actions:
                            model = new ActionModel(connection, verifyRefID.Value);
                            if (!(model as ActionModel).CanEdit())  // is user authorized to do the delete?
                                return null;
                            break;
                        case AttachmentProxy.References.Assets:
                            model = new AssetModel(connection, verifyRefID.Value);
                            break;
                        //case AttachmentProxy.References.ChatAttachments:
                        //    break;
                        case AttachmentProxy.References.CompanyActivity:
                            {
                                int organizationID = connection._db.ExecuteQuery<int>($"Select RefID from Notes WITH (NOLOCK) WHERE NoteID = {verifyRefID.Value}").Min();
                                model  = new NoteModel(new OrganizationModel(connection, organizationID), verifyRefID.Value);
                            }
                            break;
                        case AttachmentProxy.References.ContactActivity:
                            {
                                int userID = connection._db.ExecuteQuery<int>($"Select RefID from Notes WITH (NOLOCK) WHERE NoteID = {verifyRefID.Value}").Min();
                                model  = new NoteModel(new UserModel(connection, userID), verifyRefID.Value);
                            }
                            break;
                        //case AttachmentProxy.References.Contacts:
                        //    break;
                        //case AttachmentProxy.References.CustomerHubLogo:
                        //    break;
                        //case AttachmentProxy.References.None:
                        //    break;
                        case AttachmentProxy.References.Organizations:
                            model = new OrganizationModel(connection, verifyRefID.Value);
                            break;
                        case AttachmentProxy.References.ProductVersions:
                            model = new ProductVersionModel(connection, verifyRefID.Value);
                            break;
                        case AttachmentProxy.References.Tasks:
                            model = new TaskModel(connection, verifyRefID.Value);
                            break;
                        case AttachmentProxy.References.UserPhoto:
                        case AttachmentProxy.References.Users:
                            model = new UserModel(connection, verifyRefID.Value);
                            break;
                        case AttachmentProxy.References.WaterCooler:
                            model = new WatercoolerMsgModel(connection, verifyRefID.Value);
                            break;
                        default:
                            if (Debugger.IsAttached) Debugger.Break();
                            throw new Exception($"unrecognized RefType {verifyRefType} in DeleteAttachment");
                    }

                    // do the delete
                    Data_API.Delete(new AttachmentModel(model, attachmentID));
                    AttachmentFile file = new AttachmentFile(model, proxy);
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                Data_API.LogMessage(ActionLogType.Delete, ReferenceType.Attachments, attachmentID, "Unable to delete attachment", ex);
            }
            return result;
        }

        // load action attachments into attachment proxy
        const string SelectActionAttachmentProxy = "SELECT a.*, a.ActionAttachmentID as AttachmentID, a.ActionAttachmentGUID as AttachmentGUID, (u.FirstName + ' ' + u.LastName) AS CreatorName, a.ActionID as RefID " +
            "FROM ActionAttachments a LEFT JOIN Users u ON u.UserID = a.CreatorID ";

        /// <summary> Read most recent filenames for this ticket </summary>
        public static void ReadActionAttachmentsForTicket(TicketModel ticketModel, ActionAttachmentsByTicketID ticketActionAttachments, out AttachmentProxy[] mostRecentByFilename)
        {
            switch (ticketActionAttachments)
            {
                case ActionAttachmentsByTicketID.ByFilename:
                    {
                        DataContext db = ticketModel.Connection._db;
                        Table<ActionProxy> actionTable = db.GetTable<ActionProxy>();
                        int[] actionID = (from a in actionTable where a.TicketID == ticketModel.TicketID select a.ActionID).ToArray();

                        Table<AttachmentProxy> attachmentTable = db.GetTable<AttachmentProxy>();
                        AttachmentProxy[] allAttachments = attachmentTable.Where(a => actionID.Contains(a.RefID)).ToArray();
                        List<AttachmentProxy> tmp = new List<AttachmentProxy>();
                        foreach (AttachmentProxy attachment in allAttachments)
                        {
                            if (!tmp.Exists(a => a.FileName == attachment.FileName))
                                tmp.Add(attachment);
                        }
                        mostRecentByFilename = tmp.ToArray();
                    }
                    break;
                case ActionAttachmentsByTicketID.KnowledgeBase:
                    {
                        string query = SelectActionAttachmentProxy + $"JOIN Actions ac ON a.ActionID = ac.ActionID WHERE ac.TicketID = {ticketModel.TicketID} AND ac.IsKnowledgeBase = 1";
                        mostRecentByFilename = ticketModel.ExecuteQuery<AttachmentProxy>(query).ToArray();
                    }
                    break;
                default:
                    mostRecentByFilename = null;
                    break;
            }
        }

        public static string GetOrganizationAttachmentPath()
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    return connection.Organization.AttachmentPath;
                }
            }
            catch (Exception ex)
            {
                Data_API.LogMessage(ActionLogType.Delete, ReferenceType.Attachments, 0, "Unable to find organization attachment path", ex);
            }
            return String.Empty;
        }

        ///// <summary> Read most recent filenames for this ticket </summary>
        //public static void ReadActionAttachmentsByFilenameAndTicket(TicketModel ticketModel, out AttachmentProxy[] mostRecentByFilename)
        //{
        //    string query = SelectActionAttachmentProxy + $"WHERE ActionID IN (SELECT ActionID FROM Actions WHERE TicketID = {ticketModel.TicketID}) ORDER BY DateCreated DESC";
        //    AttachmentProxy[] allAttachments = ticketModel.ExecuteQuery<AttachmentProxy>(query).ToArray();
        //    List<AttachmentProxy> tmp = new List<AttachmentProxy>();
        //    foreach (AttachmentProxy attachment in allAttachments)
        //    {
        //        if (!tmp.Exists(a => a.FileName == attachment.FileName))
        //            tmp.Add(attachment);
        //    }
        //    mostRecentByFilename = tmp.ToArray();
        //}

        ///// <summary> Read most recent filenames for this ticket </summary>
        //public static void ReadKBActionAttachmentsByTicket(TicketModel ticketModel, out AttachmentProxy[] mostRecentByFilename)
        //{
        //    string query = SelectActionAttachmentProxy + $"JOIN Actions ac ON a.ActionID = ac.ActionID WHERE ac.TicketID = {ticketModel.TicketID} AND ac.IsKnowledgeBase = 1";
        //    mostRecentByFilename = ticketModel.ExecuteQuery<AttachmentProxy>(query).ToArray();
        //}

        #region PathMap
        public struct PathMap
        {
            public string _path;
            public AttachmentProxy.References _refType;
            public PathMap(string path, AttachmentProxy.References refType)
            {
                _path = path;
                _refType = refType;
            }
        }
        static readonly PathMap[] _pathMap;

        /// <summary> From AttachmentPath </summary>
        static AttachmentAPI()
        {
            _pathMap = new PathMap[]{
                    new PathMap("Actions", AttachmentProxy.References.Actions),
                    new PathMap("AgentRating", AttachmentProxy.References.None),
                    new PathMap("AssetAttachments", AttachmentProxy.References.Assets),
                    new PathMap("Images\\Chat", AttachmentProxy.References.None),
                    new PathMap("ChatAttachments", AttachmentProxy.References.ChatAttachments),
                    new PathMap("Styles\\Chat", AttachmentProxy.References.None),
                    new PathMap("CustomerActivityAttachments", AttachmentProxy.References.CompanyActivity),
                    new PathMap("ContactActivityAttachments", AttachmentProxy.References.ContactActivity),
                    new PathMap("Images\\Avatars\\Contacts", AttachmentProxy.References.None),
                    new PathMap("Images\\HubLogo", AttachmentProxy.References.CustomerHubLogo),
                    new PathMap("Images", AttachmentProxy.References.None),
                    new PathMap("Imports", AttachmentProxy.References.None),
                    new PathMap("Imports\\Logs", AttachmentProxy.References.None),
                    new PathMap("Organizations", AttachmentProxy.References.Organizations),
                    new PathMap("OrganizationAttachments", AttachmentProxy.References.Organizations),
                    new PathMap("Images\\CompanyLogo", AttachmentProxy.References.None),
                    new PathMap("Products", AttachmentProxy.References.ProductVersions),
                    new PathMap("Images\\Avatars", AttachmentProxy.References.None),
                    new PathMap("ScheduledReports", AttachmentProxy.References.None),
                    new PathMap("ScheduledReports\\Logs", AttachmentProxy.References.None),
                    new PathMap("Styles", AttachmentProxy.References.None),
                    new PathMap("Tasks", AttachmentProxy.References.Tasks),
                    new PathMap("Images\\Temp", AttachmentProxy.References.None),
                    new PathMap("Images\\TicketTypes", AttachmentProxy.References.None),
                    new PathMap("UserAttachments", AttachmentProxy.References.Users),
                    new PathMap("WaterCooler", AttachmentProxy.References.WaterCooler),
            };
        }

        /// <summary> ...\Upload\Actions\55582993 </summary>
        static void GetPathMap(HttpContext context, out PathMap pathMap, out int id, out string ratingImage)
        {
            List<string> segments = new List<string>();
            foreach (string segment in context.Request.Url.Segments)
                segments.Add(segment.ToLower().Trim().Replace("/", ""));

            // id
            if(int.TryParse(segments[segments.Count - 1], out id))
                segments.RemoveAt(segments.Count - 1);

            // _ratingImage
            if (segments[segments.Count - 1] == "ratingpositive" || segments[segments.Count - 1] == "ratingneutral" || segments[segments.Count - 1] == "ratingnegative")
            {
                ratingImage = segments[segments.Count - 1];
                segments.RemoveAt(segments.Count - 1);
            }
            else
                ratingImage = "";

            // pathMap
            string path = null;
            bool pathStart = false;
            foreach (string segment in segments)
            {
                switch (segment)
                {
                    case "upload":
                    case "chatupload":
                        pathStart = true;
                        break;
                    case "chatattachments":
                        path = segment;
                        pathStart = false;
                        break;
                    default:
                        if(!pathStart)
                            break;
                        if (path == null)
                            path = segment;
                        else
                            path += '\\' + segment;
                        break;
                }
            }
            pathMap = _pathMap.Where(p => path == p._path.ToLower()).First(); // throw if not found
        }

        private static void InitializeProxy(HttpContext context, ConnectionContext connection, AttachmentFile attachmentFile, AttachmentProxy proxy)
        {
            // context
            string tmp = context.Request.Form["description"];
            if (tmp != null)
                proxy.Description = tmp.Replace("\n", "<br />");

            tmp = context.Request.Form["productFamilyID"];
            if ((tmp != null) && !tmp.Equals("-1"))
                proxy.ProductFamilyID = Int32.Parse(tmp);
            proxy.FilePathID = AttachmentProxy.AttachmentPathIndex;

            // file
            proxy.Path = attachmentFile.FilePath;
            proxy.FileSize = attachmentFile.ContentLength;
            proxy.FileType = attachmentFile.ContentType;
            proxy.FileName = attachmentFile.FileName;

            // authentication
            proxy.OrganizationID = connection.OrganizationID;
            proxy.ModifierID = connection.UserID;
            proxy.CreatorID = connection.UserID;
            proxy.DateCreated = proxy.DateModified = DateTime.UtcNow;
        }

        #endregion

    }
}
